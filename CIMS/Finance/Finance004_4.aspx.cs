//******************************************************************
//*  作    者：lantaosu
//*  功能說明：晶片信用卡資本化攤銷查詢
//*  創建日期：2008-12-11
//*  修改日期：2008-12-12 12:00
//*  修改記錄：
//*            □2008-12-12
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

public partial class Finance_Finance004_4 : PageBase
{
    Finance004_4BL bl = new Finance004_4BL();
    Finance004_1BL bl1 = new Finance004_1BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbStockCostSNumber.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //初始化頁面
            DataSet Year = bl.GetYearList();
            dropYear.DataSource = Year;
            dropYear.DataValueField = "Year";
            dropYear.DataTextField = "Year";
            dropYear.DataBind();
            dropYear.SelectedValue = DateTime.Now.Year.ToString();            

            btnReport.Visible = false;
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string StartDate = null;
        string EndDate = null;
        string[] Date = new string[2];

        //獲取帳務起迄日
        Date = bl1.GetDateFromAndDateTo(dropYear.SelectedItem.Text, dropMonth.SelectedValue);
        if (Date == null)
        {
            ShowMessage("卡片成本帳務期間未設定");
            return;
        }
        StartDate = Date[0];
        EndDate = Date[1];
        ViewState["DateFrom"] = StartDate;
        ViewState["DateTo"] = EndDate;


        //取得該日期區間内的第一個未日結日
        string strUnCheckDate = bl1.GetUncheckDate(StartDate, EndDate);
        if (strUnCheckDate != "")
        {
            ShowMessage(strUnCheckDate + "後未日結");
            return;
        }
        else
        {
            //計算攤銷的起始日期 = 使用者輸入日期的5年前的日期(年月)
            if (Convert.ToInt32(dropMonth.SelectedValue) > 9)
            {
                Session["DateFrom"] = Convert.ToString((Convert.ToInt32(dropYear.SelectedValue) - 5)) + dropMonth.SelectedValue;
                Session["DateTo"] = dropYear.SelectedValue + dropMonth.SelectedValue;
            }
            else
            {
                Session["DateFrom"] = Convert.ToString((Convert.ToInt32(dropYear.SelectedValue) - 5)) + "0" + dropMonth.SelectedValue;
                Session["DateTo"] = dropYear.SelectedValue + "0" + dropMonth.SelectedValue;
            }

            gvpbStockCostSNumber.BindData();

            if (gvpbStockCostSNumber.DataSource != null)
            {
                btnReport.Visible = true;

                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doSearch();", true);
            }
            else
            {
                ShowMessage("查無資料");
                btnReport.Visible = false;
            }
        }
    }

    #region 列表數據綁定
    /// <summary>
    /// GridView數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbStockCostSNumber_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;        
        int i = 0;
        int j = 0;
        decimal total = 0;
        string DateFrom = Session["DateFrom"].ToString();        
        string DateTo = Session["DateTo"].ToString();

        string Date_Time = "";

        DataSet dstlStockCostSNumber = null;

        try
        {
            dstlStockCostSNumber = bl.List( DateFrom, DateTo, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
                    
            DataTable dtSCS = new DataTable();
            DataRow drSCS = null;
            dtSCS.Columns.Add("科目", Type.GetType("System.String"));
            dtSCS.Columns.Add("摘 要", Type.GetType("System.String"));
            dtSCS.Columns.Add("原始成本", Type.GetType("System.String"));
            dtSCS.Columns.Add("每期需攤金額", Type.GetType("System.String"));
            dtSCS.Columns.Add("帳面價值", Type.GetType("System.String"));

            if (dstlStockCostSNumber != null && dstlStockCostSNumber.Tables[0].Rows.Count > 0)
            {
                Date_Time = dstlStockCostSNumber.Tables[0].Rows[0]["Date_Time"].ToString();
                if (Date_Time.Substring(0, 4) == dropYear.SelectedValue)
                {
                    for (i = Convert.ToInt32(Date_Time.Substring(4, 2)); i <= Convert.ToInt32(dropMonth.SelectedValue); i++)
                    {
                        if (i > 9)
                            dtSCS.Columns.Add(dropYear.SelectedValue + "/" + i.ToString(), Type.GetType("System.String"));
                        else
                            dtSCS.Columns.Add(dropYear.SelectedValue + "/0" + i.ToString(), Type.GetType("System.String"));
                    }
                }
                else
                {
                    for (i = 0; i < Convert.ToInt32(dropYear.SelectedValue) - Convert.ToInt32(Date_Time.Substring(0, 4)); i++)
                        dtSCS.Columns.Add(Convert.ToString(Convert.ToInt32(Date_Time.Substring(0, 4)) + i) + "年", Type.GetType("System.String"));
                    for (i = 1; i <= Convert.ToInt32(dropMonth.SelectedValue); i++)
                    {
                        if (i > 9)
                            dtSCS.Columns.Add(dropYear.SelectedValue + "/" + i.ToString(), Type.GetType("System.String"));
                        else
                            dtSCS.Columns.Add(dropYear.SelectedValue + "/0" + i.ToString(), Type.GetType("System.String"));
                    }
                }
                dtSCS.Columns.Add("合 計", Type.GetType("System.String"));

                foreach (DataRow drStockCostSNumber in dstlStockCostSNumber.Tables[0].Rows)
                {
                    drSCS = dtSCS.NewRow();
                    drSCS["科目"] = "其他遞延費用-晶片卡";
                    drSCS["摘 要"] = drStockCostSNumber["Date_Time"].ToString().Substring(0, 4) + "/" + drStockCostSNumber["Date_Time"].ToString().Substring(4, 2) + "月晶片卡成本";
                    drSCS["原始成本"] = drStockCostSNumber["S_Number"].ToString();
                    drSCS["每期需攤金額"] = Convert.ToString(Math.Round(Convert.ToDecimal(drStockCostSNumber["S_Number"].ToString()) / 60, 2, MidpointRounding.AwayFromZero));
                    //drSCS["每期需攤金額"] = drSCS["每期需攤金額"].ToString().Split('.')[0] + "." + Convert.ToString(Math.Round(Convert.ToDecimal(drSCS["每期需攤金額"].ToString()), 2, MidpointRounding.AwayFromZero)).Split('.')[1].Substring(0, 2);

                    decimal dCount = 0;
                    //攤銷金額/月份
                    if (drStockCostSNumber["Date_Time"].ToString().Substring(0, 4) == dropYear.SelectedValue)
                        for (i = Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(4, 2)); i <= Convert.ToInt32(dropMonth.SelectedValue); i++)
                        {
                            if (dCount < Convert.ToDecimal(drSCS["原始成本"]))
                            {
                                // Legend 2017/05/15 調整最後一期計算方式
                                // 每期需攤金額
                                decimal decPerAmt = Convert.ToDecimal(drSCS["每期需攤金額"].ToString());

                                // 當 [累計攤銷] + [每期需攤金額] > [原始成本]時, 則為[最後一期攤銷]
                                // [最後一期攤銷] =   [原始成本] - [累計攤銷]
                                if((dCount + decPerAmt) > Convert.ToDecimal(drSCS["原始成本"]))
                                {
                                    decPerAmt = Convert.ToDecimal(drSCS["原始成本"]) - dCount;
                                }

                                if (i > 9)
                                    drSCS[dropYear.SelectedValue + "/" + i.ToString()] = decPerAmt.ToString();
                                else
                                    drSCS[dropYear.SelectedValue + "/0" + i.ToString()] = decPerAmt.ToString();

                                dCount += decPerAmt;
                            }
                        }
                    else
                    {
                        for (i = 0; i < Convert.ToInt32(dropYear.SelectedValue) - Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(0, 4)); i++)
                        {
                            if (dCount < Convert.ToDecimal(drSCS["原始成本"]))
                            {
                                if (i == 0)//原本2006年計算出來的數字，改為固定值為"13,427,591.70 "
                                {
                                    if (drStockCostSNumber["Date_Time"].ToString().Substring(0, 4) == "2006")
                                    {
                                        drSCS[Convert.ToString(Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(0, 4)) + i) + "年"] = "13427591.70";
                                    }
                                    else drSCS[Convert.ToString(Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(0, 4)) + i) + "年"] = Convert.ToString(Convert.ToDecimal(drSCS["每期需攤金額"].ToString()) * (13 - Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(4, 2))));
                                }
                                else
                                {
                                    //if (drStockCostSNumber["Date_Time"].ToString().Substring(0, 4) == "2006")
                                    //{
                                    //    drSCS[Convert.ToString(Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(0, 4)) + i) + "年"] = "13427591.70";
                                    //}
                                    //else 
                                    drSCS[Convert.ToString(Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(0, 4)) + i) + "年"] = Convert.ToString(Convert.ToDecimal(drSCS["每期需攤金額"].ToString()) * 12);
                                }

                                dCount += Convert.ToDecimal(drSCS[Convert.ToString(Convert.ToInt32(drStockCostSNumber["Date_Time"].ToString().Substring(0, 4)) + i) + "年"].ToString());
                            }
                        }
                        for (i = 1; i <= Convert.ToInt32(dropMonth.SelectedValue); i++)
                        {
                            if (dCount < Convert.ToDecimal(drSCS["原始成本"]))
                            {
                                // Legend 2017/05/15 調整最後一期計算方式
                                // 每期需攤金額
                                decimal decPerAmt = Convert.ToDecimal(drSCS["每期需攤金額"].ToString());

                                // 當 [累計攤銷] + [每期需攤金額] > [原始成本]時, 則為[最後一期攤銷]
                                // [最後一期攤銷] =   [原始成本] - [累計攤銷]
                                if ((dCount + decPerAmt) > Convert.ToDecimal(drSCS["原始成本"]))
                                {
                                    decPerAmt = Convert.ToDecimal(drSCS["原始成本"]) - dCount;
                                }

                                if (i > 9)
                                    drSCS[dropYear.SelectedValue + "/" + i.ToString()] = decPerAmt.ToString();
                                else
                                    drSCS[dropYear.SelectedValue + "/0" + i.ToString()] = decPerAmt.ToString();

                                dCount += decPerAmt;
                            }
                        }
                    }
                    //（右邊）合 計
                    total = 0;
                    for (i = 5; i < (dtSCS.Columns.Count - 1); i++)
                        if (drSCS[i].ToString().Trim() != "")
                            total += Convert.ToDecimal(drSCS[i].ToString());
                    drSCS["合 計"] = total.ToString();

                    drSCS["帳面價值"] = Convert.ToString(Convert.ToDecimal(drSCS["原始成本"].ToString()) - Convert.ToDecimal(drSCS["合 計"].ToString()));
                    dtSCS.Rows.Add(drSCS);
                }  

                //資料表中的合計行
                drSCS = dtSCS.NewRow();
                drSCS["摘 要"] = "合計";
                for (i = 2; i < dtSCS.Columns.Count; i++)
                {
                    total = 0;
                    for (j = 0; j < dtSCS.Rows.Count; j++)
                        if (dtSCS.Rows[j][i].ToString().Trim() != "")
                            total += Convert.ToDecimal(dtSCS.Rows[j][i].ToString());
                        else
                            total += 0;
                    if (total == 0)
                        drSCS[i] = total.ToString() + ".00";
                    else
                        drSCS[i] = total.ToString();
                }
                drSCS[0] = "";
                dtSCS.Rows.Add(drSCS);

                
                //傳參數，添加報表數據
                Session["StockCostSNumber"] = dtSCS;
            }

            if (dtSCS != null && dtSCS.Rows.Count > 0)//如果查到了資料
            {
                e.Table = dtSCS;//要綁定的資料表
                e.RowCount = dtSCS.Rows.Count;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    /// <summary>
    /// GridView列數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbStockCostSNumber_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            DataTable dt = (DataTable)Session["StockCostSNumber"];

            //创建一个GridViewRow，相当於表格的 TR 一行
            GridViewRow rowHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
            string HeaderBackColor = "#B9BDAA";
            rowHeader.BackColor = System.Drawing.ColorTranslator.FromHtml(HeaderBackColor);
            rowHeader.BorderStyle = BorderStyle.Solid;

            //实现确定要显示的表头样式，也可以通过计算生成

            //    <tr>
            //      <td rowspan='2'>关键单元格</td>
            //      <td colspan='2'>表头文字</td>
            //      <td colspan='2'>表头文字</td>
            //      <td>表头文字</td>
            //      </tr>
            //      <tr bgcolor='#FFF'>
            //      <td colspan='2'>表头文字</td>
            //      <td rowspan='2'>表头文字</td>
            //      <td colspan='2'>表头文字</td>
            //      </tr>
            //      <tr bgcolor='#FFF'>
            //      <td>表头文字</td>
            //      <td>表头文字</td>
            //      <td>表头文字</td>
            //      <td>表头文字</td>
            //      <td>表头文字</td>";
            //   </tr>
            // 上面的样式可以设置斜线

            Literal newCells = new Literal();
            newCells.Text = @"科目</th>
                  <th rowspan='2'>摘 要</th>
                  <th rowspan='2'>原始成本</th>
                  <th rowspan='2'>每期需攤金額</th>
                  <th rowspan='2'>帳面價值(原成本-累積攤銷)</th>
                  <th colspan='" + Convert.ToString(dt.Columns.Count - 6) + "'>攤銷金額/月份</th>";
            newCells.Text += @"<th >合 計</th>
                  </tr>
                  <tr bgcolor='" + HeaderBackColor + "'>";
            for (int i = 5; i < (dt.Columns.Count - 1); i++)
                newCells.Text += "<th >" + dt.Columns[i].ColumnName + "<//th>";

            newCells.Text += @"<th >  ";

            TableCellCollection cells = e.Row.Cells;
            TableHeaderCell headerCell = new TableHeaderCell();
            //下面的属性设置与 <td rowspan='2'>关键单元格</td> 要一致
            headerCell.RowSpan = 2;
            headerCell.Controls.Add(newCells);
            rowHeader.Cells.Add(headerCell);

            rowHeader.Cells.Add(headerCell);
            rowHeader.Visible = true;

            //添加到 GridView
            gvpbStockCostSNumber.Controls[0].Controls.AddAt(0, rowHeader);
        }

    }
    #endregion

    /// <summary>
    /// 匯出報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        gvpbStockCostSNumber.BindData();
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        this.hdTime.Value = time;
        bl.AddReport((DataTable)Session["StockCostSNumber"], time);
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "exportExcel();", true); 
    }
}
