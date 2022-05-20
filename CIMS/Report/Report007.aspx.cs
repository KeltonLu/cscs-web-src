using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Reporting.WebForms;

public partial class Report_Report007 : PageBase
{
    Report007BL bl = new Report007BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            nodata.Visible = false;
            dropCard_Purpose_RIDBind();
            dropCard_Group_RIDBind();
            dropFactoryBind();

            ReportView1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report007_1";

            ReportView2.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView2.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report007_2";

            txtDate_Time.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
    }

    private void GenReport1(string timeMark)
    {

        this.ReportView1.Visible = true;
        this.ReportView2.Visible=false;

        ReportParameter[] reportParam = new ReportParameter[4];
        reportParam[0] = new ReportParameter("Date_Time", txtDate_Time.Text, false);
        reportParam[1] = new ReportParameter("Card_Group_RID", dropCard_Group_RID.SelectedValue, false);
        reportParam[2] = new ReportParameter("Perso_Factory", dropFactory.SelectedValue, false);
        reportParam[3] = new ReportParameter("RCT", timeMark, false);
        ReportView1.ServerReport.SetParameters(reportParam);
        ReportView1.ShowParameterPrompts = false;
    }

    private void GenReport2(string timeMark)
    {

        this.ReportView2.Visible = true;
        this.ReportView1.Visible=false;

        ReportParameter[] reportParam = new ReportParameter[4];
        reportParam[0] = new ReportParameter("Date_Time", txtDate_Time.Text, false);
        reportParam[1] = new ReportParameter("Card_Group_RID", dropCard_Group_RID.SelectedValue, false);
        reportParam[2] = new ReportParameter("Perso_Factory", dropFactory.SelectedValue, false);
        reportParam[3] = new ReportParameter("RCT", timeMark, false);
        ReportView2.ServerReport.SetParameters(reportParam);
        ReportView2.ShowParameterPrompts = false;
    }

    //查詢
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        try
        {
            if (StringUtil.IsEmpty(txtDate_Time.Text.Trim()))
            {
                ShowMessage("日期不能為空！");
                return;
            }
            string timeMark = DateTime.Now.ToString("yyyyMMddHHmmss");
            ViewState["timeMark"] = timeMark;

            // Legend 2017/02/04 添加 防呆驗證
            if (dropFactory.SelectedItem == null)
            {
                ShowMessage("Perso廠不能為空！");
                return;
            }

            // Legend 2017/02/04 添加 防呆驗證
            if (dropCard_Group_RID.SelectedItem == null)
            {
                ShowMessage("群組不能為空！");
                return;
            }

            if (dropCard_Group_RID.SelectedItem.Text != "晶片金融卡")
            {
                GenReport1(timeMark);
            }
            else
            {
                GenReport2(timeMark);
            } 


            return;

            //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
            Dictionary<string, object> inputs = new Dictionary<string, object>();
            inputs.Add("txtDate_Time", txtDate_Time.Text.Trim());
            inputs.Add("dropCard_Group_RID", dropCard_Group_RID.SelectedValue.Trim());
            inputs.Add("dropFactory", dropFactory.SelectedValue.Trim());

            // 取製成卡公式
            DataTable dtExpressionsMakeCardType = bl.getExpressions(1);
            // 取製成卡公式 end

            // 取消耗卡公式
            DataTable dtExpressionsUsedCard = bl.getExpressions(2);
            // 取消耗卡公式 end

            // 取所有異動信息 start
            DataTable dtFactoryChangeImport = bl.getAllFactoryChangeImport(inputs);
            // 取所有異動信息 end

            // 取所有入庫信息 start
            DataTable dtDepositoryStock = bl.getAllDepositoryStock(inputs);
            // 取所有入庫信息 end

            // 取所有退貨信息 start
            DataTable dtDepositoryCancel = bl.getAllDepositoryCancel(inputs);
            // 取所有退貨信息 end

            // 取所有再入庫信息 start
            DataTable dtDepositoryReStock = bl.getAllDepositoryReStock(inputs);
            // 取所有再入庫信息 end

            DataTable dtDepositoryMoveIn = null;
            DataTable dtDepositoryMoveOut = null;
            if (dropFactory.SelectedIndex != 0)// 不是所有廠商時，計算該廠商的該卡種的移轉數量
            {
                // 取所有移入信息 start
                dtDepositoryMoveIn = bl.getAllCardTypeStocksMoveIn(inputs);
                // 取所有移入信息 end 

                // 取所有移出信息 start
                dtDepositoryMoveOut = bl.getAllCardTypeStocksMoveOut(inputs);
                // 取所有移出信息 end  
            }

            // 非晶片金融卡
            if (dropCard_Group_RID.SelectedItem.Text.Trim() != "晶片金融卡")
            {
                // 取所有小計檔信息 start
                DataTable dtSubTotal = bl.getAllSubTotal(inputs);
                // 取所有小計檔信息 end

                #region 初始化gridview的columns
                DataTable dtColumns = new DataTable();
                dtColumns.Columns.Add("column");

                DataRow dr = dtColumns.NewRow();
                dr[0] = "卡片編號";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "版面簡稱";
                dtColumns.Rows.Add(dr);

                //獲得製卡類型
                int intMakeCardTypeColumNum = 0;
                DataSet ds = bl.getType(dropCard_Group_RID.SelectedValue);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    intMakeCardTypeColumNum = ds.Tables[0].Rows.Count;//制卡類型列數
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dtColumns.NewRow();
                        dr[0] = ds.Tables[0].Rows[i]["type_name"].ToString();
                        dtColumns.Rows.Add(dr);
                    }
                }

                dr = dtColumns.NewRow();
                dr[0] = "缺卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "未製卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "補製卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "製成卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "樣卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "製損卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "感應不良";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "排卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "銷毀";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "調整";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "消耗卡";
                dtColumns.Rows.Add(dr);

                //獲得其他卡種狀態
                int intOtherMakeCardTypeColumNum = 0;
                DataSet dso = bl.getOtherCardStatus();
                if (dso != null && dso.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dso.Tables[0].Rows.Count; i++)
                    {
                        if (dso.Tables[0].Rows[i]["status_name"].ToString() != "入庫" && dso.Tables[0].Rows[i]["status_name"].ToString() != "退貨" && dso.Tables[0].Rows[i]["status_name"].ToString() != "再入庫")
                        {
                            dr = dtColumns.NewRow();
                            dr[0] = dso.Tables[0].Rows[i]["status_name"].ToString();
                            dtColumns.Rows.Add(dr);

                            intOtherMakeCardTypeColumNum++;
                        }
                    }
                }

                dr = dtColumns.NewRow();
                dr[0] = "入庫";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "退貨";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "再入庫";
                dtColumns.Rows.Add(dr);
                // 不為全部廠商時，顯示"移轉"列
                if (dropFactory.SelectedIndex != 0)
                {
                    dr = dtColumns.NewRow();
                    dr[0] = "移轉";
                    dtColumns.Rows.Add(dr);    
                }
                #endregion 初始化gridview的columns

                #region 生成gridview要加載的數據table
                DataTable dtMakeCardType = new DataTable();
                for (int i = 0; i < dtColumns.Rows.Count; i++)
                {
                    dtMakeCardType.Columns.Add(dtColumns.Rows[i][0].ToString());
                }

                #region 將小計檔加入gridview要加載的數據table中
                for (int intLoop=0;intLoop<dtSubTotal.Rows.Count;intLoop++)
                {
                    string str卡片編號 = dtSubTotal.Rows[intLoop]["TYPE"].ToString() + "-" +dtSubTotal.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtSubTotal.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 +"'");
                    if (drsSelect.Length>0) //該卡種已經在數據表中
                    {
                        for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1][dtSubTotal.Rows[intLoop]["Type_Name"].ToString()] = 
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1][dtSubTotal.Rows[intLoop]["Type_Name"].ToString()]) + Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }else{ // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtSubTotal.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow[dtSubTotal.Rows[intLoop]["Type_Name"].ToString()] = Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將小計檔加入gridview要加載的數據table中

                #region 將廠商異動加入gridview要加載的數據table中
                for (int intLoop=0;intLoop<dtFactoryChangeImport.Rows.Count;intLoop++)
                {
                    if (dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "廠商結餘" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "入庫" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "再入庫" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "退貨" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "移轉" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "3D" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "DA" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "PM" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "RN")
                        continue;

                    string str卡片編號 = dtFactoryChangeImport.Rows[intLoop]["TYPE"].ToString() + "-" +dtFactoryChangeImport.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtFactoryChangeImport.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 +"'");
                    if (drsSelect.Length>0) //該卡種已經在數據表中
                    {
                        for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1][dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString()] = 
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1][dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString()]) + Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }else{ // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtFactoryChangeImport.Rows[intLoop]["Space_Short_Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow[dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString()] = Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將廠商異動加入gridview要加載的數據table中

                #region 將入庫加入gridview要加載的數據table中
                for (int intLoop=0;intLoop<dtDepositoryStock.Rows.Count;intLoop++)
                {
                    string str卡片編號 = dtDepositoryStock.Rows[intLoop]["TYPE"].ToString() + "-" +dtDepositoryStock.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtDepositoryStock.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 +"'");
                    if (drsSelect.Length>0) //該卡種已經在數據表中
                    {
                        for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1]["入庫"] = 
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["入庫"]) + Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }else{ // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryStock.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow["入庫"] = Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將入庫加入gridview要加載的數據table中

                #region 將退貨加入gridview要加載的數據table中
                for (int intLoop=0;intLoop<dtDepositoryCancel.Rows.Count;intLoop++)
                {
                    string str卡片編號 = dtDepositoryCancel.Rows[intLoop]["TYPE"].ToString() + "-" +dtDepositoryCancel.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtDepositoryCancel.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 +"'");
                    if (drsSelect.Length>0) //該卡種已經在數據表中
                    {
                        for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1]["退貨"] = 
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["退貨"]) + Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }else{ // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryCancel.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow["退貨"] = Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將退貨加入gridview要加載的數據table中

                #region 將再入庫加入gridview要加載的數據table中
                for (int intLoop=0;intLoop<dtDepositoryReStock.Rows.Count;intLoop++)
                {
                    string str卡片編號 = dtDepositoryReStock.Rows[intLoop]["TYPE"].ToString() + "-" +dtDepositoryReStock.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtDepositoryReStock.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 +"'");
                    if (drsSelect.Length>0) //該卡種已經在數據表中
                    {
                        for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1]["再入庫"] = 
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["再入庫"]) + Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }else{ // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryReStock.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow["再入庫"] = Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將再入庫加入gridview要加載的數據table中

                if (dropFactory.SelectedIndex != 0)// 不是所有廠商時，計算該廠商的該卡種的移轉數量
                {
                    #region 將移入加入gridview要加載的數據table中
                    for (int intLoop=0;intLoop<dtDepositoryMoveIn.Rows.Count;intLoop++)
                    {
                        string str卡片編號 = dtDepositoryMoveIn.Rows[intLoop]["TYPE"].ToString() + "-" +dtDepositoryMoveIn.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtDepositoryMoveIn.Rows[intLoop]["PHOTO"].ToString();
                        DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 +"'");
                        if (drsSelect.Length>0) //該卡種已經在數據表中
                        {
                            for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    dtMakeCardType.Rows[intLoop1]["移轉"] = 
                                        Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["移轉"]) + Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    break;
                                }
                            }
                        }else{ // 該卡種不在數據表中
                            DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                            drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                            drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryMoveIn.Rows[intLoop]["Name"].ToString();
                            for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                                drMakeCardTypeNewRow[intLoop2] = 0;
                            drMakeCardTypeNewRow["移轉"] = Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                            dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                        }
                    }
                    #endregion 將移入加入gridview要加載的數據table中

                    #region 將移出加入gridview要加載的數據table中
                    for (int intLoop=0;intLoop<dtDepositoryMoveOut.Rows.Count;intLoop++)
                    {
                        string str卡片編號 = dtDepositoryMoveOut.Rows[intLoop]["TYPE"].ToString() + "-" +dtDepositoryMoveOut.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtDepositoryMoveOut.Rows[intLoop]["PHOTO"].ToString();
                        DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 +"'");
                        if (drsSelect.Length>0) //該卡種已經在數據表中
                        {
                            for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    dtMakeCardType.Rows[intLoop1]["移轉"] = 
                                        Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["移轉"]) - Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    break;
                                }
                            }
                        }else{ // 該卡種不在數據表中
                            DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                            drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                            drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryMoveOut.Rows[intLoop]["Name"].ToString();
                            for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                                drMakeCardTypeNewRow[intLoop2] = 0;
                            drMakeCardTypeNewRow["移轉"] = 0-Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                            dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                        }
                    }
                    #endregion 將移出加入gridview要加載的數據table中
                }

                #region 計算每種卡種的製成卡數
                for (int intLoop2 = 0; intLoop2 < dtExpressionsMakeCardType.Rows.Count; intLoop2++)
                {
                    string strStatus_Name = dtExpressionsMakeCardType.Rows[intLoop2]["Status_Name"].ToString();
                    string strOperate = dtExpressionsMakeCardType.Rows[intLoop2]["Operate"].ToString().Trim();
                    if (strStatus_Name == "3D" || strStatus_Name == "DA" || 
                        strStatus_Name == "PM" || strStatus_Name == "RN")
                    { 
                        #region 根據小計檔計算製成卡
                        for (int intLoop=0;intLoop<dtSubTotal.Rows.Count;intLoop++)
                        {
                            if (dtSubTotal.Rows[intLoop]["Type_Name"].ToString() != strStatus_Name)
                                continue;

                            string str卡片編號 = dtSubTotal.Rows[intLoop]["TYPE"].ToString() + "-" +dtSubTotal.Rows[intLoop]["AFFINITY"].ToString() + "-" +dtSubTotal.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1=0;intLoop1<dtMakeCardType.Rows.Count;intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+")){
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] = 
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] = 
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據小計檔計算製成卡
                    }
                    else if (strStatus_Name == "入庫")
                    {
                        #region 根據入庫計算製成卡
                        for (int intLoop = 0; intLoop < dtDepositoryStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據入庫計算製成卡
                    }
                    else if (strStatus_Name == "退貨")
                    {
                        #region 根據退貨計算製成卡
                        for (int intLoop = 0; intLoop < dtDepositoryCancel.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryCancel.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據退貨計算製成卡
                    }
                    else if (strStatus_Name == "再入庫")
                    {
                        #region 根據再入庫計算製成卡
                        for (int intLoop = 0; intLoop < dtDepositoryReStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryReStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據再入庫計算製成卡
                    }
                    else if (dropFactory.SelectedIndex != 0 && strStatus_Name == "移轉")
                    {
                        #region 根據移轉計算製成卡
                        // 移入
                        for (int intLoop = 0; intLoop < dtDepositoryMoveIn.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveIn.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }

                        // 移出
                        for (int intLoop = 0; intLoop < dtDepositoryMoveOut.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveOut.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據移轉計算製成卡
                    }
                    else
                    {
                        #region 根據廠商異動計算製成卡
                        for (int intLoop = 0; intLoop < dtFactoryChangeImport.Rows.Count; intLoop++)
                        {
                            if (dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() != strStatus_Name)
                                continue;

                            string str卡片編號 = dtFactoryChangeImport.Rows[intLoop]["TYPE"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據廠商異動計算製成卡
                    }
                }
                #endregion 計算每種卡種的製成卡數

                #region 計算每種卡種的消耗卡數
                for (int intLoop2 = 0; intLoop2 < dtExpressionsUsedCard.Rows.Count; intLoop2++)
                {
                    string strStatus_Name = dtExpressionsUsedCard.Rows[intLoop2]["Status_Name"].ToString();
                    string strOperate = dtExpressionsUsedCard.Rows[intLoop2]["Operate"].ToString().Trim();
                    if (strStatus_Name == "3D" || strStatus_Name == "DA" ||
                        strStatus_Name == "PM" || strStatus_Name == "RN")
                    {
                        #region 根據小計檔計算消耗卡
                        for (int intLoop = 0; intLoop < dtSubTotal.Rows.Count; intLoop++)
                        {
                            if (dtSubTotal.Rows[intLoop]["Type_Name"].ToString() != strStatus_Name)
                                continue;

                            string str卡片編號 = dtSubTotal.Rows[intLoop]["TYPE"].ToString() + "-" + dtSubTotal.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtSubTotal.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據小計檔計算消耗卡
                    }
                    else if (strStatus_Name == "入庫")
                    {
                        #region 根據入庫計算消耗卡
                        for (int intLoop = 0; intLoop < dtDepositoryStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據入庫計算消耗卡
                    }
                    else if (strStatus_Name == "退貨")
                    {
                        #region 根據退貨計算消耗卡
                        for (int intLoop = 0; intLoop < dtDepositoryCancel.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryCancel.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據退貨計算消耗卡
                    }
                    else if (strStatus_Name == "再入庫")
                    {
                        #region 根據再入庫計算消耗卡
                        for (int intLoop = 0; intLoop < dtDepositoryReStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryReStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據再入庫計算消耗卡
                    }
                    else if (dropFactory.SelectedIndex != 0 && strStatus_Name == "移轉")
                    {
                        #region 根據移轉計算消耗卡
                        // 移入
                        for (int intLoop = 0; intLoop < dtDepositoryMoveIn.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveIn.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }

                        // 移出
                        for (int intLoop = 0; intLoop < dtDepositoryMoveOut.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveOut.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據移轉計算消耗卡
                    }
                    else
                    {
                        #region 根據廠商異動計算消耗卡
                        for (int intLoop = 0; intLoop < dtFactoryChangeImport.Rows.Count; intLoop++)
                        {
                            if (dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() != strStatus_Name)
                                continue;

                            string str卡片編號 = dtFactoryChangeImport.Rows[intLoop]["TYPE"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據廠商異動計算消耗卡
                    }
                }
                #endregion 計算每種卡種的消耗卡數

                if (dtMakeCardType.Rows.Count > 0)
                {
                    DataRow drc = dtMakeCardType.NewRow();
                    drc[0] = "";
                    drc[1] = "合計";
                    int ic = dtMakeCardType.Columns.Count;
                    for (int ir = 2; ir < ic; ir++)
                    {
                        long co = 0;
                        for (int r = 0; r < dtMakeCardType.Rows.Count; r++)
                        {
                            co += Convert.ToInt64(dtMakeCardType.Rows[r][ir].ToString());
                        }
                        drc[ir] = co;
                        co = 0;
                    }
                    dtMakeCardType.Rows.Add(drc);
                }

                gvpbReport.DataSource = dtMakeCardType;
                gvpbReport.DataBind();
                #endregion 生成gridview要加載的數據table
            }
            else // 晶片金融卡
            {
                // 取所有小計檔信息 start
                DataTable dtSubTotal = bl.getAllSubTotal_JP(inputs);
                // 取所有小計檔信息 end

                #region 初始化gridview的columns
                DataTable dtColumns = new DataTable();
                dtColumns.Columns.Add("column");

                DataRow dr = dtColumns.NewRow();
                dr[0] = "卡片編號";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "版面簡稱";
                dtColumns.Rows.Add(dr);

                DataSet dscard = bl.getCardName(dropCard_Group_RID.SelectedValue);
                if (dscard != null && dscard.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dscard.Tables[0].Rows.Count; i++)
                    {
                        dr = dtColumns.NewRow();
                        dr[0] = dscard.Tables[0].Rows[i]["name"].ToString();
                        dtColumns.Rows.Add(dr);
                    }
                }

                dr = dtColumns.NewRow();
                dr[0] = "缺卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "未製卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "補製卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "製成卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "樣卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "製損卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "感應不良";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "排卡";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "銷毀";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "調整";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "消耗卡";
                dtColumns.Rows.Add(dr);

                //獲得其他卡種狀態
                DataSet dso = bl.getOtherCardStatus();
                if (dso != null && dso.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dso.Tables[0].Rows.Count; i++)
                    {
                        if (dso.Tables[0].Rows[i]["status_name"].ToString() != "入庫" && dso.Tables[0].Rows[i]["status_name"].ToString() != "退貨" && dso.Tables[0].Rows[i]["status_name"].ToString() != "再入庫")
                        {
                            dr = dtColumns.NewRow();
                            dr[0] = dso.Tables[0].Rows[i]["status_name"].ToString();
                            dtColumns.Rows.Add(dr);
                        }
                    }
                }

                dr = dtColumns.NewRow();
                dr[0] = "入庫";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "退貨";
                dtColumns.Rows.Add(dr);
                dr = dtColumns.NewRow();
                dr[0] = "再入庫";
                dtColumns.Rows.Add(dr);
                // 不為全部廠商時，顯示"移轉"列
                if (dropFactory.SelectedIndex != 0)
                {
                    dr = dtColumns.NewRow();
                    dr[0] = "移轉";
                    dtColumns.Rows.Add(dr);
                }

                #endregion 初始化gridview的columns

                #region 生成gridview要加載的數據table
                DataTable dtMakeCardType = new DataTable();
                for (int i = 0; i < dtColumns.Rows.Count; i++)
                {
                    dtMakeCardType.Columns.Add(dtColumns.Rows[i][0].ToString());
                }

                #region 將小計檔加入gridview要加載的數據table中
                for (int intLoop = 0; intLoop < dtSubTotal.Rows.Count; intLoop++)
                {
                    string str卡片編號 = dtSubTotal.Rows[intLoop]["TYPE"].ToString() + "-" + dtSubTotal.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtSubTotal.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 + "'");
                    if (drsSelect.Length > 0) //該卡種已經在數據表中
                    {
                        for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1][dtSubTotal.Rows[intLoop]["Old_Name"].ToString()] =
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1][dtSubTotal.Rows[intLoop]["Old_Name"].ToString()]) + Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }
                    else
                    { // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtSubTotal.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow[dtSubTotal.Rows[intLoop]["Old_Name"].ToString()] = Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將小計檔加入gridview要加載的數據table中

                #region 將廠商異動加入gridview要加載的數據table中
                for (int intLoop = 0; intLoop < dtFactoryChangeImport.Rows.Count; intLoop++)
                {
                    if (dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "廠商結餘" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "入庫" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "再入庫" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "退貨" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "移轉" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "3D" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "DA" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "PM" ||
                        dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() == "RN")
                        continue;

                    string str卡片編號 = dtFactoryChangeImport.Rows[intLoop]["TYPE"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 + "'");
                    if (drsSelect.Length > 0) //該卡種已經在數據表中
                    {
                        for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1][dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString()] =
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1][dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString()]) + Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }
                    else
                    { // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtFactoryChangeImport.Rows[intLoop]["Space_Short_Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow[dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString()] = Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將廠商異動加入gridview要加載的數據table中

                #region 將入庫加入gridview要加載的數據table中
                for (int intLoop = 0; intLoop < dtDepositoryStock.Rows.Count; intLoop++)
                {
                    string str卡片編號 = dtDepositoryStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 + "'");
                    if (drsSelect.Length > 0) //該卡種已經在數據表中
                    {
                        for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1]["入庫"] =
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["入庫"]) + Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }
                    else
                    { // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryStock.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow["入庫"] = Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將入庫加入gridview要加載的數據table中

                #region 將退貨加入gridview要加載的數據table中
                for (int intLoop = 0; intLoop < dtDepositoryCancel.Rows.Count; intLoop++)
                {
                    string str卡片編號 = dtDepositoryCancel.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 + "'");
                    if (drsSelect.Length > 0) //該卡種已經在數據表中
                    {
                        for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1]["退貨"] =
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["退貨"]) + Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }
                    else
                    { // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryCancel.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow["退貨"] = Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將退貨加入gridview要加載的數據table中

                #region 將再入庫加入gridview要加載的數據table中
                for (int intLoop = 0; intLoop < dtDepositoryReStock.Rows.Count; intLoop++)
                {
                    string str卡片編號 = dtDepositoryReStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["PHOTO"].ToString();
                    DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 + "'");
                    if (drsSelect.Length > 0) //該卡種已經在數據表中
                    {
                        for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                        {
                            if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                            {
                                dtMakeCardType.Rows[intLoop1]["再入庫"] =
                                    Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["再入庫"]) + Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                break;
                            }
                        }
                    }
                    else
                    { // 該卡種不在數據表中
                        DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                        drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                        drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryReStock.Rows[intLoop]["Name"].ToString();
                        for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                            drMakeCardTypeNewRow[intLoop2] = 0;
                        drMakeCardTypeNewRow["再入庫"] = Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                        dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                    }
                }
                #endregion 將再入庫加入gridview要加載的數據table中

                if (dropFactory.SelectedIndex != 0)// 不是所有廠商時，計算該廠商的該卡種的移轉數量
                {
                    #region 將移入加入gridview要加載的數據table中
                    for (int intLoop = 0; intLoop < dtDepositoryMoveIn.Rows.Count; intLoop++)
                    {
                        string str卡片編號 = dtDepositoryMoveIn.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["PHOTO"].ToString();
                        DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 + "'");
                        if (drsSelect.Length > 0) //該卡種已經在數據表中
                        {
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    dtMakeCardType.Rows[intLoop1]["移轉"] =
                                        Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["移轉"]) + Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    break;
                                }
                            }
                        }
                        else
                        { // 該卡種不在數據表中
                            DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                            drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                            drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryMoveIn.Rows[intLoop]["Name"].ToString();
                            for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                                drMakeCardTypeNewRow[intLoop2] = 0;
                            drMakeCardTypeNewRow["移轉"] = Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                            dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                        }
                    }
                    #endregion 將移入加入gridview要加載的數據table中

                    #region 將移出加入gridview要加載的數據table中
                    for (int intLoop = 0; intLoop < dtDepositoryMoveOut.Rows.Count; intLoop++)
                    {
                        string str卡片編號 = dtDepositoryMoveOut.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["PHOTO"].ToString();
                        DataRow[] drsSelect = dtMakeCardType.Select("卡片編號 = '" + str卡片編號 + "'");
                        if (drsSelect.Length > 0) //該卡種已經在數據表中
                        {
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    dtMakeCardType.Rows[intLoop1]["移轉"] =
                                        Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["移轉"]) - Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    break;
                                }
                            }
                        }
                        else
                        { // 該卡種不在數據表中
                            DataRow drMakeCardTypeNewRow = dtMakeCardType.NewRow();
                            drMakeCardTypeNewRow["卡片編號"] = str卡片編號;
                            drMakeCardTypeNewRow["版面簡稱"] = dtDepositoryMoveOut.Rows[intLoop]["Name"].ToString();
                            for (int intLoop2 = 2; intLoop2 < dtColumns.Rows.Count; intLoop2++)
                                drMakeCardTypeNewRow[intLoop2] = 0;
                            drMakeCardTypeNewRow["移轉"] = 0 - Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                            dtMakeCardType.Rows.Add(drMakeCardTypeNewRow);
                        }
                    }
                    #endregion 將移出加入gridview要加載的數據table中
                }

                #region 計算每種卡種的製成卡數
                for (int intLoop2 = 0; intLoop2 < dtExpressionsMakeCardType.Rows.Count; intLoop2++)
                {
                    string strStatus_Name = dtExpressionsMakeCardType.Rows[intLoop2]["Status_Name"].ToString();
                    string strOperate = dtExpressionsMakeCardType.Rows[intLoop2]["Operate"].ToString().Trim();
                    if (strStatus_Name == "DA")
                    {
                        #region 根據小計檔計算製成卡
                        for (int intLoop = 0; intLoop < dtSubTotal.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtSubTotal.Rows[intLoop]["TYPE"].ToString() + "-" + dtSubTotal.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtSubTotal.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據小計檔計算製成卡
                    }
                    else if (strStatus_Name == "入庫")
                    {
                        #region 根據入庫計算製成卡
                        for (int intLoop = 0; intLoop < dtDepositoryStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據入庫計算製成卡
                    }
                    else if (strStatus_Name == "退貨")
                    {
                        #region 根據退貨計算製成卡
                        for (int intLoop = 0; intLoop < dtDepositoryCancel.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryCancel.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據退貨計算製成卡
                    }
                    else if (strStatus_Name == "再入庫")
                    {
                        #region 根據再入庫計算製成卡
                        for (int intLoop = 0; intLoop < dtDepositoryReStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryReStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據再入庫計算製成卡
                    }
                    else if (dropFactory.SelectedIndex != 0 && strStatus_Name == "移轉")
                    {
                        #region 根據移轉計算製成卡
                        // 移入
                        for (int intLoop = 0; intLoop < dtDepositoryMoveIn.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveIn.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }

                        // 移出
                        for (int intLoop = 0; intLoop < dtDepositoryMoveOut.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveOut.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據移轉計算製成卡
                    }
                    else
                    {
                        #region 根據廠商異動計算製成卡
                        for (int intLoop = 0; intLoop < dtFactoryChangeImport.Rows.Count; intLoop++)
                        {
                            if (dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() != strStatus_Name)
                                continue;

                            string str卡片編號 = dtFactoryChangeImport.Rows[intLoop]["TYPE"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) + Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["製成卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["製成卡"]) - Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據廠商異動計算製成卡
                    }
                }
                #endregion 計算每種卡種的製成卡數

                #region 計算每種卡種的消耗卡數
                for (int intLoop2 = 0; intLoop2 < dtExpressionsUsedCard.Rows.Count; intLoop2++)
                {
                    string strStatus_Name = dtExpressionsUsedCard.Rows[intLoop2]["Status_Name"].ToString();
                    string strOperate = dtExpressionsUsedCard.Rows[intLoop2]["Operate"].ToString().Trim();
                    if (strStatus_Name == "DA")
                    {
                        #region 根據小計檔計算消耗卡
                        for (int intLoop = 0; intLoop < dtSubTotal.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtSubTotal.Rows[intLoop]["TYPE"].ToString() + "-" + dtSubTotal.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtSubTotal.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtSubTotal.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據小計檔計算消耗卡
                    }
                    else if (strStatus_Name == "入庫")
                    {
                        #region 根據入庫計算消耗卡
                        for (int intLoop = 0; intLoop < dtDepositoryStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據入庫計算消耗卡
                    }
                    else if (strStatus_Name == "退貨")
                    {
                        #region 根據退貨計算消耗卡
                        for (int intLoop = 0; intLoop < dtDepositoryCancel.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryCancel.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryCancel.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryCancel.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據退貨計算消耗卡
                    }
                    else if (strStatus_Name == "再入庫")
                    {
                        #region 根據再入庫計算消耗卡
                        for (int intLoop = 0; intLoop < dtDepositoryReStock.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryReStock.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryReStock.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryReStock.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據再入庫計算消耗卡
                    }
                    else if (dropFactory.SelectedIndex != 0 && strStatus_Name == "移轉")
                    {
                        #region 根據移轉計算消耗卡
                        // 移入
                        for (int intLoop = 0; intLoop < dtDepositoryMoveIn.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveIn.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveIn.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryMoveIn.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }

                        // 移出
                        for (int intLoop = 0; intLoop < dtDepositoryMoveOut.Rows.Count; intLoop++)
                        {
                            string str卡片編號 = dtDepositoryMoveOut.Rows[intLoop]["TYPE"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtDepositoryMoveOut.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtDepositoryMoveOut.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據移轉計算消耗卡
                    }
                    else
                    {
                        #region 根據廠商異動計算消耗卡
                        for (int intLoop = 0; intLoop < dtFactoryChangeImport.Rows.Count; intLoop++)
                        {
                            if (dtFactoryChangeImport.Rows[intLoop]["Status_Name"].ToString() != strStatus_Name)
                                continue;

                            string str卡片編號 = dtFactoryChangeImport.Rows[intLoop]["TYPE"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["AFFINITY"].ToString() + "-" + dtFactoryChangeImport.Rows[intLoop]["PHOTO"].ToString();
                            for (int intLoop1 = 0; intLoop1 < dtMakeCardType.Rows.Count; intLoop1++)
                            {
                                if (dtMakeCardType.Rows[intLoop1]["卡片編號"].ToString() == str卡片編號)
                                {
                                    if (strOperate.Equals("+"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) + Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    else if (strOperate.Equals("-"))
                                    {
                                        dtMakeCardType.Rows[intLoop1]["消耗卡"] =
                                            Convert.ToInt32(dtMakeCardType.Rows[intLoop1]["消耗卡"]) - Convert.ToInt32(dtFactoryChangeImport.Rows[intLoop]["SUM_Number"]);
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion 根據廠商異動計算消耗卡
                    }
                }
                #endregion 計算每種卡種的消耗卡數

                if (dtMakeCardType.Rows.Count > 0)
                {
                    DataRow drc = dtMakeCardType.NewRow();
                    drc[0] = "";
                    drc[1] = "合計";
                    int ic = dtMakeCardType.Columns.Count;
                    for (int ir = 2; ir < ic; ir++)
                    {
                        long co = 0;
                        for (int r = 0; r < dtMakeCardType.Rows.Count; r++)
                        {
                            co += Convert.ToInt64(dtMakeCardType.Rows[r][ir].ToString());
                        }
                        drc[ir] = co;
                        co = 0;
                    }
                    dtMakeCardType.Rows.Add(drc);
                }

                gvpbReport.DataSource = dtMakeCardType;
                gvpbReport.DataBind();
                #endregion 生成gridview要加載的數據table
            }
        }
        catch (AlertException alex)
        {
            ShowMessage(alex.Message);
        }
        catch(Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //匯出Excel
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        string timeMark = ViewState["timeMark"].ToString();
        if (nodata.Visible==false&&gvpbReport.Rows.Count>0)
        {
            if (dropCard_Group_RID.SelectedItem.Text != "晶片金融卡")
            {
                if (dropFactory.SelectedValue != "")
                {
                    Response.Write("<script>window.open('Report007_01Print.aspx?Date_Time=" + Server.UrlEncode(txtDate_Time.Text) + "&Card_Group_RID=" + Server.UrlEncode(dropCard_Group_RID.SelectedValue) + "&Perso_Factory=" + Server.UrlEncode(dropFactory.SelectedItem.Text) + "&strTime="+timeMark+"');</script>");
                }
                else
                {
                    Response.Write("<script>window.open('Report007_01Print.aspx?Date_Time=" + Server.UrlEncode(txtDate_Time.Text) + "&Card_Group_RID=" + Server.UrlEncode(dropCard_Group_RID.SelectedValue) + "&Perso_Factory=" + dropFactory.SelectedValue + "&strTime=" + timeMark + "');</script>");
                }
            }
            else if (dropCard_Group_RID.SelectedItem.Text == "晶片金融卡")
            {
                if (dropFactory.SelectedValue != "")
                {
                    Response.Write("<script>window.open('Report007_02Print.aspx?Date_Time=" + Server.UrlEncode(txtDate_Time.Text) + "&Card_Group_RID=" + Server.UrlEncode(dropCard_Group_RID.SelectedValue) + "&Perso_Factory=" + Server.UrlEncode(dropFactory.SelectedItem.Text) + "&strTime=" + timeMark + "');</script>");
                }
                else
                {
                    Response.Write("<script>window.open('Report007_02Print.aspx?Date_Time=" + Server.UrlEncode(txtDate_Time.Text) + "&Card_Group_RID=" + Server.UrlEncode(dropCard_Group_RID.SelectedValue) + "&Perso_Factory=" + dropFactory.SelectedValue + "&strTime=" + timeMark + "');</script>");
                }
            }
        }
        else
        {
            ShowMessage("無可列印數據！");
        }
    }

    //用途變更
    protected void dropCard_Purpose_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            dropCard_Group_RIDBind();                  
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void gvpbReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtb = (DataTable)gvpbReport.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                int c = 0;
                if (dropCard_Group_RID.SelectedItem.Text == "晶片金融卡")
                {
                    c = 1;
                }
                else
                {
                    c = 2;
                }

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i >= c && i < e.Row.Cells.Count)
                    {
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;

                        if (e.Row.Cells[i].Text.Trim() == "0")
                        {
                            e.Row.Cells[i].Text = "-";
                        }
                        else
                        {
                            if (Convert.ToInt64(e.Row.Cells[i].Text.Trim()) > 0)
                            {
                                if (e.Row.Cells[i].Text.Trim().Length > 3)
                                {
                                    int m = 3;
                                    while (m < e.Row.Cells[i].Text.Trim().Length)
                                    {
                                        e.Row.Cells[i].Text = e.Row.Cells[i].Text.Trim().Insert(e.Row.Cells[i].Text.Trim().Length - m, ",");
                                        m = m + 4;
                                    }
                                }
                            }
                            else
                            {
                                if (e.Row.Cells[i].Text.Trim().Length > 4)
                                {
                                    e.Row.Cells[i].Text = e.Row.Cells[i].Text.Remove(0, 1);

                                    if (e.Row.Cells[i].Text.Trim().Length > 3)
                                    {
                                        int m = 3;
                                        while (m < e.Row.Cells[i].Text.Trim().Length)
                                        {
                                            e.Row.Cells[i].Text = e.Row.Cells[i].Text.Trim().Insert(e.Row.Cells[i].Text.Trim().Length - m, ",");
                                            m = m + 4;
                                        }
                                    }

                                    e.Row.Cells[i].Text = "-" + e.Row.Cells[i].Text;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }

    #region 欄位/數據補充說明
    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_Purpose_RIDBind()
    {
        CardTypeManager ctmManager = new CardTypeManager();
        dropCard_Purpose_RID.DataTextField = "PARAM_NAME";
        dropCard_Purpose_RID.DataValueField = "Param_Code";
        dropCard_Purpose_RID.DataSource = ctmManager.GetPurpose();
        dropCard_Purpose_RID.DataBind();
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_Group_RIDBind()
    {
        CardTypeManager ctmManager = new CardTypeManager();
        dropCard_Group_RID.Items.Clear();

        if (!StringUtil.IsEmpty(dropCard_Purpose_RID.SelectedValue))
        {
            dropCard_Group_RID.DataTextField = "GROUP_NAME";
            dropCard_Group_RID.DataValueField = "RID";
            dropCard_Group_RID.DataSource = ctmManager.GetGroupByPurposeId(dropCard_Purpose_RID.SelectedValue);
            dropCard_Group_RID.DataBind();

        }
    }

    /// <summary>
    /// Perso廠下拉框綁定
    /// </summary>
    protected void dropFactoryBind()
    {
        CardType005BL ctbl = new CardType005BL();
        // 獲取 Perso廠商資料
        DataSet dstFactory = ctbl.GetFactoryList();
        dropFactory.DataValueField = "RID";
        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataSource = dstFactory.Tables[0];
        dropFactory.DataBind();
        dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }
    #endregion
}
