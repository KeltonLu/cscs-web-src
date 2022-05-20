//******************************************************************
//*  作    者：JunWang
//*  功能說明：請款放行作業邏輯 
//*  創建日期：2008-12-03
//*  修改日期：2008-12-03 9:00
//*  修改記錄：
//*            □2008-12-03
//*              1.創建 王俊
//*             2010/12/10  Ge.Song
//*                 RQ-2010-004324-000 空白卡請款-遲繳天數開放負數
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
public partial class Finance_Finance0013 : PageBase
{
    Finance0013BL Finance0013BL = new Finance0013BL();
    //add by Ian Huang start
    double dRIC_Number = 0; //數量合計
    double dUnit_PriceSum = 0;  //含稅合計
    double dUnit_Price1Sum = 0; //未稅合計
    //add by Ian Huang end

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbFinance.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //進貨作業日期(起)和進貨作業日期(迄)都設為當天
            //this.txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //this.txtFinish_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
            dropCard_PurposeBind();// 用途下拉框綁定
            dropCard_GroupBind();// 群組下拉框綁定
            dropBlankFactoryBind();//空白卡廠下拉框綁定

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                }
                gvpbFinance.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }

    /// <summary>
    /// 空白卡廠下拉框綁定
    /// </summary>
    protected void dropBlankFactoryBind()
    {
        dropBlankFactory.Items.Clear();

        dropBlankFactory.DataTextField = "Factory_ShortName_CN";
        dropBlankFactory.DataValueField = "RID";
        dropBlankFactory.DataSource = Finance0013BL.getFactory();
        dropBlankFactory.DataBind();

        dropBlankFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_PurposeBind()
    {
        dropCard_Purpose.DataTextField = "PARAM_NAME";
        dropCard_Purpose.DataValueField = "Param_Code";
        dropCard_Purpose.DataSource = Finance0013BL.getParam_Use();
        dropCard_Purpose.DataBind();

        dropCard_Purpose.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_GroupBind()
    {
        dropCard_Group.Items.Clear();

        dropCard_Group.DataTextField = "GROUP_NAME";
        dropCard_Group.DataValueField = "RID";
        dropCard_Group.DataSource = Finance0013BL.getCardGroup(dropCard_Purpose.SelectedValue);
        dropCard_Group.DataBind();

        dropCard_Group.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    protected void dropCard_Purpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_GroupBind();
    }
    protected void btnSearch1_Click(object sender, EventArgs e)
    {
        gvpbFinance.BindData();
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {

    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        DataTable dtSAP = (DataTable)Session["dsSAP"];
        string strTime = DateTime.Now.ToString("yyyyMMddhhmmss");
        Finance0013BL.ADD_CARD_YEAR_FORCAST_PRINT(dtSAP, strTime);

        Response.Write("<script>window.open('Finance0013Print.aspx?Time=" + strTime + "&inputs=1&Comment=2&Fine=3','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");
    }
    protected void gvpbFinance_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (dropCard_Purpose.SelectedItem.Text != "全部")
        {
            inputs.Add("dropCard_Purpose", dropCard_Purpose.SelectedValue);
        }
        else
        {
            inputs.Add("dropCard_Purpose", "");
        }

        if (dropCard_Group.SelectedItem.Text != "全部")
        {
            inputs.Add("dropCard_Group", dropCard_Group.SelectedValue);
        }
        else
        {
            inputs.Add("dropCard_Group", "");
        }
        if (dropBlankFactory.SelectedItem.Text != "全部")
        {
            inputs.Add("dropBlankFactory", dropBlankFactory.SelectedValue);
        }
        else
        {
            inputs.Add("dropBlankFactory", "");
        }
        if (dropState.SelectedItem.Text != "全部")
        {
            inputs.Add("dropState", dropState.SelectedValue);
        }
        else
        {
            inputs.Add("dropState", "");
        }
        if (dropPass_Status.SelectedItem.Text != "全部")
        {
            inputs.Add("dropPass_Status", dropPass_Status.SelectedValue);
        }
        else
        {
            inputs.Add("dropPass_Status", "");
        }
        if (dropIs_Finance.SelectedItem.Text != "全部")
        {
            inputs.Add("dropIs_Finance", dropIs_Finance.SelectedValue);
        }
        else
        {
            inputs.Add("dropIs_Finance", "");
        }



        inputs.Add("txtBegin_Date", txtBegin_Date.Text);
        inputs.Add("txtFinish_Date", txtFinish_Date.Text);
        inputs.Add("txtBUDGET_ID", txtBUDGET_ID.Text);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("txtAgreement_Code", txtAgreement_Code.Text);

        inputs.Add("txtStock_RIDYear", txtStock_RIDYear.Text.Trim());
        inputs.Add("txtStock_RID1", txtStock_RID1.Text.Trim());
        inputs.Add("txtStock_RID2", txtStock_RID2.Text.Trim());
        inputs.Add("txtStock_RID3", txtStock_RID3.Text.Trim());


        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dsSAP = null;

        try
        {
            dsSAP = Finance0013BL.SearchSAP(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dsSAP != null)//如果查到了資料
            {
                //add by Ian Huang 新增<合計>列 start
                dsSAP.Tables[0].Columns.Add(new DataColumn("Unit_PriceSum", typeof(string)));
                dsSAP.Tables[0].Columns.Add(new DataColumn("Unit_Price1Sum", typeof(string)));
                dsSAP.Tables[0].Columns.Add(new DataColumn("Stock_RID_STR", typeof(string)));
                //add by Ian Huang 新增<合計>列 end 

                Session["dsSAP"] = dsSAP.Tables[0].Copy();

                //add by Ian Huang 新增<合計>列 start
                //有資料才加 合計 row
                if (intRowCount > 0)
                {
                    dsSAP.Tables[0].Rows.Add(dsSAP.Tables[0].NewRow());
                }
                //add by Ian Huang 新增<合計>列 end 

                e.Table = dsSAP.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數                
                //查詢有結果才將匯出按鈕顯示出來
                if (intRowCount == 0)
                {
                    btnExport.Visible = false;
                }
                else
                {
                    btnExport.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
    protected void gvpbFinance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //edit by Ian Huang start
        DataTable dtSAP = (DataTable)this.Session["dsSAP"];

        if (e.Row.RowType == DataControlRowType.Header)
        {
            //e.Row.Cells[17].Visible = false;
            e.Row.Cells[19].Visible = false;
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[19].Visible = false;

            if (e.Row.RowIndex != ((DataTable)gvpbFinance.DataSource).Rows.Count - 1)
            {
                //將 入庫流水編號 由 int col 寫入 string col (for report)
                dtSAP.Rows[e.Row.RowIndex]["Stock_RID_STR"] = dtSAP.Rows[e.Row.RowIndex]["Stock_RID"].ToString();

                // 不是第一行
                if (e.Row.RowIndex != 0)
                {
                    // 如果該行和上一行的RID相同，則不顯示數量。
                    if (dtSAP.Rows[e.Row.RowIndex - 1]["Operate_Type"].ToString() ==
                        dtSAP.Rows[e.Row.RowIndex]["Operate_Type"].ToString() &&
                        dtSAP.Rows[e.Row.RowIndex - 1]["Operate_RID"].ToString() ==
                        dtSAP.Rows[e.Row.RowIndex]["Operate_RID"].ToString()
                        )
                    {
                        e.Row.Cells[7].Text = "";
                    }
                }
            }


            //add by Ian Huang start
            if (e.Row.RowIndex == ((DataTable)gvpbFinance.DataSource).Rows.Count - 1)
            {

                DataRow dr = dtSAP.NewRow();
                dr["Stock_RID_STR"] = "合計";
                dr["Income_Number"] = dRIC_Number;
                dr["Unit_PriceSum"] = dUnit_PriceSum.ToString("N2");
                dr["Unit_Price1Sum"] = dUnit_Price1Sum.ToString("N2");
                dtSAP.Rows.Add(dr);
                Session["dsSAP"] = dtSAP;


                e.Row.Cells[0].Text = "合計";

                e.Row.Cells[7].Text = dRIC_Number.ToString("N2");
                e.Row.Cells[10].Text = dUnit_PriceSum.ToString("N2");
                e.Row.Cells[11].Text = dUnit_Price1Sum.ToString("N2");

                return;
            }
            //add by Ian Huang end

            if (e.Row.RowIndex != ((DataTable)gvpbFinance.DataSource).Rows.Count - 1)
            {
                if (e.Row.Cells[4].Text != "&nbsp;")
                {
                    e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
                if (e.Row.Cells[5].Text != "&nbsp;")
                {
                    e.Row.Cells[5].Text = Convert.ToDateTime(e.Row.Cells[5].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }

                //if (e.Row.Cells[10].Text != "&nbsp;")
                //{
                //    e.Row.Cells[10].Text = Convert.ToDateTime(e.Row.Cells[10].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                //}
                //if (e.Row.Cells[11].Text != "&nbsp;")
                //{
                //    e.Row.Cells[11].Text = Convert.ToDateTime(e.Row.Cells[11].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                //}
                if (e.Row.Cells[12].Text != "&nbsp;")
                {
                    e.Row.Cells[12].Text = Convert.ToDateTime(e.Row.Cells[12].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
                if (e.Row.Cells[13].Text != "&nbsp;")
                {
                    e.Row.Cells[13].Text = Convert.ToDateTime(e.Row.Cells[13].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }


                if (e.Row.Cells[8].Text != "&nbsp;")
                {
                    double dec = Convert.ToDouble(e.Row.Cells[8].Text.Replace(",", "")) / 1.05;
                    e.Row.Cells[9].Text = dec.ToString("N4");
                    //add by Ian Huang start
                    if ("&nbsp;" == e.Row.Cells[7].Text || "" == e.Row.Cells[7].Text.Trim())
                    {
                        e.Row.Cells[10].Text = "0"; //含稅總金額
                        e.Row.Cells[11].Text = "0"; //未稅總金額
                    }
                    else
                    {
                        dec = 0;

                        dec = Convert.ToDouble(e.Row.Cells[7].Text.Replace(",", "")) * Convert.ToDouble(e.Row.Cells[8].Text.Replace(",", ""));
                        e.Row.Cells[10].Text = Convert.ToDouble(Math.Round(dec, MidpointRounding.AwayFromZero)).ToString("N2"); //含稅總金額

                        dec = Convert.ToDouble(e.Row.Cells[7].Text.Replace(",", "")) * Convert.ToDouble(e.Row.Cells[9].Text.Replace(",", ""));
                        e.Row.Cells[11].Text = Convert.ToDouble(Math.Round(dec, MidpointRounding.AwayFromZero)).ToString("N2"); //未稅總金額
                    }
                    dtSAP.Rows[e.Row.RowIndex]["Unit_PriceSum"] = e.Row.Cells[10].Text;
                    dtSAP.Rows[e.Row.RowIndex]["Unit_Price1Sum"] = e.Row.Cells[11].Text;

                    //add by Ian Huang end
                }


                if (e.Row.Cells[3].Text == "1")
                {
                    e.Row.Cells[3].Text = "入庫";
                    dRIC_Number = dRIC_Number + Convert.ToDouble("0" + e.Row.Cells[7].Text.Replace(",", ""));
                    dUnit_PriceSum = dUnit_PriceSum + Convert.ToDouble("0" + e.Row.Cells[10].Text.Replace(",", ""));
                    dUnit_Price1Sum = dUnit_Price1Sum + Convert.ToDouble("0" + e.Row.Cells[11].Text.Replace(",", ""));
                }
                else
                {
                    e.Row.Cells[4].Text = "";
                }

                if (e.Row.Cells[3].Text == "2")
                {
                    e.Row.Cells[3].Text = "再入庫";
                    dRIC_Number = dRIC_Number + Convert.ToDouble("0" + e.Row.Cells[7].Text.Replace(",", ""));
                    dUnit_PriceSum = dUnit_PriceSum + Convert.ToDouble("0" + e.Row.Cells[10].Text.Replace(",", ""));
                    dUnit_Price1Sum = dUnit_Price1Sum + Convert.ToDouble("0" + e.Row.Cells[11].Text.Replace(",", ""));
                }

                if (e.Row.Cells[3].Text == "3")
                {
                    dRIC_Number = dRIC_Number - Convert.ToDouble("0" + e.Row.Cells[7].Text.Replace(",", ""));
                    dUnit_PriceSum = dUnit_PriceSum - Convert.ToDouble("0" + e.Row.Cells[10].Text.Replace(",", ""));
                    dUnit_Price1Sum = dUnit_Price1Sum - Convert.ToDouble("0" + e.Row.Cells[11].Text.Replace(",", ""));

                    e.Row.Cells[3].Text = "<font color='red'>退貨 </font>";
                    e.Row.Cells[3].Text = "<font color='red'>" + e.Row.Cells[3].Text + " </font>";
                    e.Row.Cells[7].Text = "<font color='red'>" + "(" + e.Row.Cells[7].Text + ")" + " </font>";
                    e.Row.Cells[10].Text = "<font color='red'>" + "(" + e.Row.Cells[10].Text + ")" + " </font>";
                    e.Row.Cells[11].Text = "<font color='red'>" + "(" + e.Row.Cells[11].Text + ")" + " </font>";
                }
                if (e.Row.Cells[4].Text != "&nbsp;" && e.Row.Cells[4].Text != "" && e.Row.Cells[5].Text != "&nbsp;" && e.Row.Cells[5].Text != "")
                {
                    e.Row.Cells[6].Text = (DateTime.Parse(e.Row.Cells[5].Text) - DateTime.Parse(e.Row.Cells[4].Text)).Days.ToString();
                }

                if (e.Row.Cells[6].Text.Substring(0, 1) == "-")
                {
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                    e.Row.Cells[6].Text = "<font color='red'>" + "(" + e.Row.Cells[6].Text.Replace("-", "") + ")" + " </font>";
                    //e.Row.Cells[6].Text = "0";
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 End
                }

                //if (e.Row.Cells[10].Text.Trim() == "1900/01/01")
                //{
                //    e.Row.Cells[10].Text = "";
                //}
                //if (e.Row.Cells[11].Text.Trim() == "1900/01/01")
                //{
                //    e.Row.Cells[11].Text = "";
                //}
                //e.Row.Cells[17].Visible = false;

                //if (e.Row.Cells[14].Text == "&nbsp;" && int.Parse(e.Row.Cells[0].Text.Substring(0, 8)) > 20080901)
                //{
                //    e.Row.Cells[15].Text = "&nbsp;";
                //}
                if (e.Row.Cells[12].Text.Trim() == "1900/01/01")
                {
                    e.Row.Cells[12].Text = "";
                }
                if (e.Row.Cells[13].Text.Trim() == "1900/01/01")
                {
                    e.Row.Cells[13].Text = "";
                }
                e.Row.Cells[19].Visible = false;

                if (e.Row.Cells[16].Text == "&nbsp;" && int.Parse(e.Row.Cells[0].Text.Substring(0, 8)) > 20080901)
                {
                    e.Row.Cells[17].Text = "&nbsp;";
                }
            }
            //edit by Ian Huang end
        }
    }
}
