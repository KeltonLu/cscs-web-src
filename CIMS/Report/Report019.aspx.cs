//******************************************************************
//*  作    者：lantaosu
//*  功能說明：多功能報表 
//*  創建日期：2008-12-01
//*  修改日期：2008-12-03 18:00
//*  修改記錄：
//*            □2008-12-03
//*              1.創建 蘇斕濤
//*******************************************************************
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

public partial class Report_Report019 : PageBase
{
    Report019BL bl = new Report019BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbFunction.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            ReportView1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report019";

            
            //初始化頁面 
            btnReport.Visible = false;

            DataSet Use = bl.GetUseList();
            dropUse.DataSource = Use;
            dropUse.DataValueField = "Param_Code";
            dropUse.DataTextField = "Param_Name";
            dropUse.DataBind();

            DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
            dropGroup.DataSource = Group;
            dropGroup.DataValueField = "RID";
            dropGroup.DataTextField = "Group_Name";
            dropGroup.DataBind();

            dropFactoryBind();

            if (radl.SelectedValue == "批次")
            {
                CheckBox1.Text = "3D";
                CheckBox2.Text = "DA";
                CheckBox3.Text = "PM";
                CheckBox4.Text = "RN";
                DataSet AllCardStatusList = bl.GetAllCardStatusList();                
                AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='3D'")[0]);
                AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='DA'")[0]);
                AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='PM'")[0]);
                AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='RN'")[0]);

                ViewState["CardStatusList"] = AllCardStatusList;
                CheckBoxList.DataSource = AllCardStatusList;
                CheckBoxList.DataTextField = "Status_Name";
                CheckBoxList.DataValueField="Status_Code";
                CheckBoxList.DataBind();

                //if (CheckBoxList.Items.FindByValue("18") != null)
                //    CheckBoxList.Items.Remove(CheckBoxList.Items.FindByValue("18"));

                for (int i = 0; i < CheckBoxList.Items.Count; i++)
                    CheckBoxList.Items[i].Selected = true;
            }
        }
    }

    private void GenReport()
    {

        this.ReportView1.Visible = true;
        ReportView1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
        ReportView1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report019";
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        DataTable dt = (DataTable)ViewState["dstlFunction"];
        ViewState["Group"] = dropGroup.SelectedItem.Text;
        ViewState["DateFrom"] = txtBeginDate.Text;
        ViewState["DateTo"] = txtEndDate.Text;
        for (int t = 0; t < dt.Rows.Count; t++)
            for (int i = 0; i < dt.Columns.Count; i++)
                if (dt.Rows[t][i].ToString() == "-")
                    dt.Rows[t][i] = "0";
        bl.AddReport(dt, time, radlDate.SelectedValue, dropFactory.SelectedValue);
        string FactoryName;
        if (dropFactory.SelectedValue != "")
        {
            FactoryName = dropFactory.SelectedItem.Text.ToString();
        }
        else FactoryName = "總";

        Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[4];
        Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("TimeMark", time);
        Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("GroupName", dropGroup.SelectedItem.Text);
        Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("FactoryName", FactoryName);
        Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("DateFromTo", txtBeginDate.Text + "~" + txtEndDate.Text);
        ReportView1.ServerReport.SetParameters(Paras);
        //ReportView1.ShowParameterPrompts = false;
    }

    /// <summary>
    /// 選擇批次時變更checklist資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radl_SelectedIndexChanged(object sender, EventArgs e)
    {
        ReportView1.Visible = false;

        if (radl.SelectedValue == "Action")
        {
            CheckBox1.Text = "新卡";
            CheckBox2.Text = "掛補";
            CheckBox3.Text = "毀補";
            CheckBox4.Text = "換卡";
            DataSet CardStatusList = bl.GetCardStatusList();
            ViewState["CardStatusList"] = CardStatusList;
            CheckBoxList.DataSource = CardStatusList;
            CheckBoxList.DataTextField = "Status_Name";
            CheckBoxList.DataValueField = "Status_Code";            
            CheckBoxList.DataBind();
            for (int i = 0; i < CheckBoxList.Items.Count; i++)
                CheckBoxList.Items[i].Selected = true;
        }
        else
        {
            CheckBox1.Text = "3D";
            CheckBox2.Text = "DA";
            CheckBox3.Text = "PM";
            CheckBox4.Text = "RN";
            DataSet AllCardStatusList = bl.GetAllCardStatusList();
            AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='3D'")[0]);
            AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='DA'")[0]);
            AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='PM'")[0]);
            AllCardStatusList.Tables[0].Rows.Remove(AllCardStatusList.Tables[0].Select("Status_Name='RN'")[0]);

            ViewState["CardStatusList"] = AllCardStatusList;
            CheckBoxList.DataSource = AllCardStatusList;
            CheckBoxList.DataTextField = "Status_Name";
            CheckBoxList.DataValueField = "Status_Code";
            CheckBoxList.DataBind();
            for (int i = 0; i < CheckBoxList.Items.Count; i++)
                CheckBoxList.Items[i].Selected = true;
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Report018BL bl = new Report018BL();
        if (!bl.IsDefineExpression())
        {
            ShowMessage("無公式定義，無法計算");
            return;
        }

        if (txtBeginDate.Text.Trim() == "")
        {
            ShowMessage("開始日期不能為空");
            return;
        }
        if (txtEndDate.Text.Trim() == "")
        {
            ShowMessage("結束日期不能為空");
            return;
        }
        if (dropUse.SelectedValue.Trim() == "")
        {
            ShowMessage("用途不能為空");
            return;
        }
        if (dropGroup.SelectedValue.Trim() == "")
        {
            ShowMessage("群組不能為空");
            return;
        }
        gvpbFunction.BindData();

        Report018BL bl018 = new Report018BL();
        string strRiJie = bl018.GetLastRiJie();
        if (!StringUtil.IsEmpty(strRiJie))
        {
            if (Convert.ToDateTime(strRiJie) < Convert.ToDateTime(txtEndDate.Text))
            {
                ShowMessage(strRiJie + "後未日結");
            }
        }


        if (ViewState["dstlFunction"] != null)
        {
            DataTable dt = (DataTable)ViewState["dstlFunction"];
            if (dt.Rows.Count > 0)
                GenReport();
            else
                ShowMessage("查無資料");
            //btnReport.Visible = true;
        }
        else
        {
            this.ReportView1.Visible = false;
            ShowMessage("查無資料");
        }
    }

    #region 列表數據綁定
    /// <summary>
    /// GridView數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbFunction_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;
        int i = 0;
        int j = 0;
        int number = 0;    //用於計算公式

        DataSet dstlFunction = null;
        
        try
        {
            dstlFunction = bl.List(dropFactory.SelectedValue,txtBeginDate.Text, txtEndDate.Text, dropGroup.SelectedValue, radlDate.SelectedValue, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlFunction != null && dstlFunction.Tables[0].Rows.Count > 0)//如果查到了資料
            {
                DataTable dtbl = new DataTable();
                DataRow drow = null;

                dtbl.Columns.Add("日期", Type.GetType("System.String"));
                dtbl.Columns.Add("TYPE", Type.GetType("System.String"));
                dtbl.Columns.Add("AFFINITY", Type.GetType("System.String"));
                dtbl.Columns.Add("PHOTO", Type.GetType("System.String"));
                dtbl.Columns.Add("版面簡稱", Type.GetType("System.String"));
                if (radl.SelectedValue == "批次")
                {
                    dtbl.Columns.Add("3D", Type.GetType("System.String"));
                    dtbl.Columns.Add("DA", Type.GetType("System.String"));
                    dtbl.Columns.Add("PM", Type.GetType("System.String"));
                    dtbl.Columns.Add("RN", Type.GetType("System.String"));
                }
                else
                {
                    dtbl.Columns.Add("Action 1", Type.GetType("System.String"));
                    dtbl.Columns.Add("Action 2", Type.GetType("System.String"));
                    dtbl.Columns.Add("Action 3", Type.GetType("System.String"));
                    dtbl.Columns.Add("Action 5", Type.GetType("System.String"));
                }
                DataSet dt = (DataSet)ViewState["CardStatusList"];
                for (i = 0; i < dt.Tables[0].Rows.Count; i++)
                    dtbl.Columns.Add(dt.Tables[0].Rows[i]["Status_Name"].ToString(), Type.GetType("System.String"));

                foreach (DataRow dr in dstlFunction.Tables[0].Rows)
                    if (!dtbl.Columns.Contains(dr["Status_Name"].ToString()))
                        dtbl.Columns.Add(dr["Status_Name"].ToString(), Type.GetType("System.String"));

                //製成卡、消耗卡、耗損卡、特殊卡、入(出)庫
                dtbl.Columns.Add("製成卡", Type.GetType("System.String"));
                dtbl.Columns.Add("消耗卡", Type.GetType("System.String"));
                dtbl.Columns.Add("耗損卡", Type.GetType("System.String"));
                dtbl.Columns.Add("特殊卡", Type.GetType("System.String"));
                dtbl.Columns.Add("入出庫", Type.GetType("System.String"));

                foreach (DataRow dr1 in dstlFunction.Tables[0].Rows)
                {
                    //modified by Even.Cheng on 20090108
                    string Date = dr1["Date_Time"].ToString();
                    string strDate = "";
                    if (radlDate.SelectedValue == "month")
                        strDate = Date.Substring(0, 4) + "/" + Date.Substring(4, 2);
                    else
                        strDate = Date.Substring(0, 4) + "/" + Date.Substring(4, 2) + "/" + Date.Substring(6, 2);


                    if (dtbl.Select("版面簡稱='" + dr1["Name"] + "' and 日期='" + strDate + "'").Length > 0)
                    {
                        drow = dtbl.Select("版面簡稱='" + dr1["Name"] + "' and 日期='" + strDate + "'")[0];
                        drow[dr1["Status_Name"].ToString()] = dr1["Sum"].ToString();
                    }
                    else
                    {
                        drow = dtbl.NewRow();
                        drow["日期"] = strDate;
                        drow["TYPE"] = dr1["TYPE"].ToString();
                        drow["AFFINITY"] = dr1["AFFINITY"].ToString();
                        drow["PHOTO"] = dr1["PHOTO"].ToString();
                        drow["版面簡稱"] = dr1["Name"].ToString();
                        drow[dr1["Status_Name"].ToString()] = dr1["Sum"].ToString();
                        dtbl.Rows.Add(drow);
                    }
                    //end modify
                }


                //查詢結果列表中的值，如果值是“0”，則在HTML畫面中顯示“-”，先轉換為0，方便計算總合
                for (i = 0; i < dtbl.Rows.Count; i++)
                    for (j = 0; j < dtbl.Columns.Count; j++)
                        if (dtbl.Rows[i][j].ToString() == "" || dtbl.Rows[i][j].ToString() == "0")
                            dtbl.Rows[i][j] = "0";

                //根據計算公式，計算欄位製成卡、消耗卡、耗損卡、特殊卡、入出庫 的卡片數量
                foreach (DataRow dr2 in dtbl.Rows)
                {
                    if (dtbl.Columns.Contains("製成卡"))
                    {
                        DataSet dset = bl.GetExpression(GlobalString.Expression.Made_RID.ToString());//"1"
                        number = 0;
                        for (i = 0; i < dset.Tables[0].Rows.Count; i++)
                        {
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
                            {
                                try
                                {
                                    number += Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
                            {
                                try
                                {
                                    number -= Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                        }
                        dr2["製成卡"] = number.ToString();
                    }
                    if (dtbl.Columns.Contains("消耗卡"))
                    {
                        DataSet dset = bl.GetExpression(GlobalString.Expression.Used_RID.ToString());//"2"
                        number = 0;
                        for (i = 0; i < dset.Tables[0].Rows.Count; i++)
                        {
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
                            {
                                try
                                {
                                    number += Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
                            {
                                try
                                {
                                    number -= Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                        }
                        dr2["消耗卡"] = number.ToString();
                    }
                    if (dtbl.Columns.Contains("耗損卡"))
                    {
                        DataSet dset = bl.GetExpression(GlobalString.Expression.Waste_RID.ToString());//"3"
                        number = 0;
                        for (i = 0; i < dset.Tables[0].Rows.Count; i++)
                        {
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
                            {
                                try
                                {
                                    number += Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
                            {
                                try
                                {
                                    number -= Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                        }
                        dr2["耗損卡"] = number.ToString();
                    }
                    if (dtbl.Columns.Contains("特殊卡"))
                    {
                        DataSet dset = bl.GetExpression(GlobalString.Expression.Special_RID.ToString());//"5"
                        number = 0;
                        for (i = 0; i < dset.Tables[0].Rows.Count; i++)
                        {
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
                            {
                                try
                                {
                                    number += Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
                            {
                                try
                                {
                                    number -= Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                        }
                        dr2["特殊卡"] = number.ToString();
                    }
                    if (dtbl.Columns.Contains("入出庫"))
                    {
                        DataSet dset = bl.GetExpression(GlobalString.Expression.InOut_RID.ToString());//"4"
                        number = 0;
                        for (i = 0; i < dset.Tables[0].Rows.Count; i++)
                        {
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
                            {
                                try
                                {
                                    number += Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                            if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
                            {
                                try
                                {
                                    number -= Convert.ToInt32(dr2[bl.GetStatusName(dset.Tables[0].Rows[i]["Type_RID"].ToString())].ToString());
                                }
                                catch { }
                            }
                        }
                        dr2["入出庫"] = number.ToString();
                    }
                }


                //當某卡種的所有欄位值都是“0”時，不顯示此列卡種信息
                for (i = 0; i < dtbl.Rows.Count; i++)
                    for (j = 5; j < dtbl.Columns.Count; j++)
                    {
                        if (dtbl.Rows[i][j].ToString() != "0")
                        {
                            break;
                        }
                        else
                        {
                            if (j == (dtbl.Columns.Count - 1))
                                dtbl.Rows.Remove(dtbl.Rows[i]);
                        }
                    }

                //資料表中的合計行
                drow = dtbl.NewRow();
                drow["版面簡稱"] = "合計";
                int total = 0;
                for (i = 5; i < dtbl.Columns.Count; i++)
                {
                    total = 0;
                    for (j = 0; j < dtbl.Rows.Count; j++)
                        total += Convert.ToInt32(dtbl.Rows[j][i].ToString());
                    drow[i] = total.ToString();
                }
                dtbl.Rows.Add(drow);

                //查詢結果列表中的值，如果值是“0”，則在HTML畫面中顯示“-”
                for (i = 0; i < dtbl.Rows.Count; i++)
                    for (j = 5; j < dtbl.Columns.Count; j++)
                        if (dtbl.Rows[i][j].ToString() == "" || dtbl.Rows[i][j].ToString() == "0")
                            dtbl.Rows[i][j] = "-";

                //刪除未勾選的“顯示欄位”
                if (radl.SelectedValue == "批次")
                {
                    if (dtbl.Columns.Contains("Action 1"))
                        dtbl.Columns.Remove("Action 1");
                    if (dtbl.Columns.Contains("Action 2"))
                        dtbl.Columns.Remove("Action 2");
                    if (dtbl.Columns.Contains("Action 3"))
                        dtbl.Columns.Remove("Action 3");
                    if (dtbl.Columns.Contains("Action 5"))
                        dtbl.Columns.Remove("Action 5");
                    if (CheckBox1.Checked == false)
                        dtbl.Columns.Remove("3D");
                    if (CheckBox2.Checked == false)
                        dtbl.Columns.Remove("DA");
                    if (CheckBox3.Checked == false)
                        dtbl.Columns.Remove("PM");
                    if (CheckBox4.Checked == false)
                        dtbl.Columns.Remove("RN");
                }
                else
                {
                    if (dtbl.Columns.Contains("3D"))
                        dtbl.Columns.Remove("3D");
                    if (dtbl.Columns.Contains("DA"))
                        dtbl.Columns.Remove("DA");
                    if (dtbl.Columns.Contains("PM"))
                        dtbl.Columns.Remove("PM");
                    if (dtbl.Columns.Contains("RN"))
                        dtbl.Columns.Remove("RN");
                    if (CheckBox1.Checked == false)
                        dtbl.Columns.Remove("Action 1");
                    if (CheckBox2.Checked == false)
                        dtbl.Columns.Remove("Action 2");
                    if (CheckBox3.Checked == false)
                        dtbl.Columns.Remove("Action 3");
                    if (CheckBox4.Checked == false)
                        dtbl.Columns.Remove("Action 5");
                }
                dt = (DataSet)ViewState["CardStatusList"];
                for (j = 0; j < dt.Tables[0].Rows.Count; j++)
                    if (CheckBoxList.Items[j].Selected == false)
                        if (dtbl.Columns.Contains(CheckBoxList.Items[j].Text.Trim()))
                            dtbl.Columns.Remove(CheckBoxList.Items[j].Text.Trim());


                ViewState["dstlFunction"] = dtbl;
                //e.Table = dtbl;//要綁定的資料表
                //e.RowCount = intRowCount;//查到的行數
            }
            else
            {
                ViewState["dstlFunction"] = null;
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// GridView列數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbFunction_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
    }
    #endregion

    /// <summary>
    /// 報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        //查詢結果列表中的值，如果值是“0”，則在HTML畫面中顯示“-”，在匯出EXCEL報表中仍顯示“0”
        DataTable dt = (DataTable)ViewState["dstlFunction"];
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        ViewState["Group"] = dropGroup.SelectedItem.Text;
        ViewState["DateFrom"] = txtBeginDate.Text;
        ViewState["DateTo"] = txtEndDate.Text;
        for (int t = 0; t < dt.Rows.Count; t++)
            for (int i = 0; i < dt.Columns.Count; i++)
                if (dt.Rows[t][i].ToString() == "-" )
                    dt.Rows[t][i] = "0";
        bl.AddReport(dt, time, radlDate.SelectedValue, dropFactory.SelectedValue);
        Response.Write("<script>window.open('Report019Report.aspx?time="+time+",'_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=650');</script>");
    }

    /// <summary>
    /// 查詢該用途的所有卡穜群組，放於【群組】下拉框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropUse_SelectedIndexChanged(object sender, EventArgs e)
    {
        ReportView1.Visible = false;

        DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
        dropGroup.DataSource = Group;
        dropGroup.DataValueField = "RID";
        dropGroup.DataTextField = "Group_Name";
        dropGroup.DataBind();
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
}
