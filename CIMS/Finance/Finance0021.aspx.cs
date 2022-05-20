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
using System.Text;
using System.Collections.Generic;
using System.Data.Common;

public partial class Finance_Finance0021 : PageBase
{
    Finance0021BL Finance0021BL = new Finance0021BL();
    Finance0022BL Finance0022BL = new Finance0022BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //(起)(迄)都設為當天
            dropCard_PurposeBind();// 用途下拉框綁定
            dropCard_GroupBind();// 群組下拉框綁定
            dropFactoryBind();//Perso厰卡廠下拉框綁定

            this.txtBegin_Date2.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.txtFinish_Date2.Text = DateTime.Now.ToString("yyyy/MM/dd");

            //從Seesion中獲取已保存的查詢條件
            if (SetConData())
                gvpSapBind();

            if (CanUseAction(GlobalString.UserRoleConfig.Finanace021Edit))
                hidIsEdit.Value = "1";
            else
                hidIsEdit.Value = "2";
        }
    }

    /// <summary>
    /// 重新綁定SAP訊息
    /// </summary>
    private void gvpSapBind1()
    {
        // 查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，
        // 控製項可以是TextBox或DropDownList
        if (ViewState["dtbl"] == null)
            return;

        DataTable dtbl = new DataTable();

        dtbl = (DataTable)ViewState["dtbl"];

        gvSAP.DataSource = dtbl;//要綁定的資料表
        gvSAP.DataBind();
    }

    /// <summary>
    /// 綁定SAP單訊息
    /// </summary>
    private void gvpSapBind()
    {
        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (dropCard_Group.SelectedItem.Value != "")
        {
            inputs.Add("dropCard_Group", dropCard_Group.SelectedValue);
        }
        else
        {
            inputs.Add("dropCard_Group", "");
        }
        inputs.Add("dropFactory", dropFactory.SelectedValue);
        inputs.Add("txtSAP_Serial_Number", txtSAP_Serial_Number.Text.Trim());
        inputs.Add("txtBegin_Date1", txtBegin_Date1.Text);
        inputs.Add("txtFinish_Date1", txtFinish_Date1.Text);
        inputs.Add("txtBegin_Date2", txtBegin_Date2.Text);
        inputs.Add("txtFinish_Date2", txtFinish_Date2.Text);
        inputs.Add("txtBegin_Date3", txtBegin_Date3.Text);
        inputs.Add("txtFinish_Date3", txtFinish_Date3.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataTable dtSAP = null;

        try
        {
            // 取代製費用SAP單訊息
            dtSAP = Finance0021BL.SearchSAP(inputs).Tables[0];

            if (dtSAP.Rows.Count > 0)
                lbMsg.Visible = false;
            else
                lbMsg.Visible = true;

            ViewState["dtSAP"] = dtSAP;

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("RID");
            dtbl.Columns.Add("Perso廠商");
            dtbl.Columns.Add("卡片耗用日期");
            dtbl.Columns.Add("用途群組");
            dtbl.Columns.Add("出帳金額");

            int i = 0;

            // 代製費用異動列
            DataTable dtParam_Change = Finance0021BL.getParam_Change().Tables[0];
            foreach (DataRow dr in dtParam_Change.Rows)
            {
                dtbl.Columns.Add("lb" + dr["Param_Code"].ToString());
                i++;
            }
            ViewState["rows"] = i;
            dtbl.Columns.Add("SAP單號");
            dtbl.Columns.Add("請款日");
            dtbl.Columns.Add("出帳日");
            dtbl.Columns.Add("發票號碼");

            foreach (DataRow drowSap in dtSAP.Rows)
            {
                DataRow drow = dtbl.NewRow();
                drow["RID"] = drowSap["RID"].ToString();
                drow["Perso廠商"] = drowSap["Factory_ShortName_EN"].ToString();
                drow["卡片耗用日期"] = drowSap["Date"].ToString();
                drow["用途群組"] = drowSap["Group_Name"].ToString();
                //modify by chaoma start
                //drow["出帳金額"] = drowSap["Sum"].ToString();
                drow["出帳金額"] = Math.Round(Convert.ToDecimal(drowSap["Sum"]), 0, MidpointRounding.AwayFromZero);
                //modify by chaoma end
                drow["SAP單號"] = drowSap["SAP_ID"].ToString();
                drow["請款日"] = drowSap["Ask_Date"].ToString();
                if (Convert.ToDateTime(drowSap["Pay_Date"].ToString()).ToString("yyyy/MM/dd") == "1900/01/01")
                {
                    drow["出帳日"] = "";
                }
                else
                {
                    drow["出帳日"] = drowSap["Pay_Date"].ToString();
                }

                drow["發票號碼"] = drowSap["Check_Serial_Number"].ToString();

                // 代製費用列訊息
                DataTable dtSearchChange = Finance0021BL.SearchChange(drowSap["Perso_Factory_RID"].ToString(),
                            drowSap["Group_RID"].ToString(),
                            Convert.ToDateTime(drowSap["Begin_Date"].ToString()),
                            Convert.ToDateTime(drowSap["End_Date"].ToString())).Tables[0];
                foreach (DataRow dr in dtSearchChange.Rows)
                {
                    for (int n = 0; n < i; n++)
                    {
                        string strColname = dtbl.Columns[4 + n].ColumnName.Replace("lb", "");
                        if (strColname == dr["Param_Code"].ToString())
                        {
                            drow[4 + n] = dr[2].ToString();
                        }
                    }
                }

                dtbl.Rows.Add(drow);
            }

            // 代製費用帳務異動列名稱
            foreach (DataRow dr in dtParam_Change.Rows)
            {
                dtbl.Columns["lb" + dr["Param_Code"].ToString()].ColumnName = dr["Param_Name"].ToString();
            }

            ViewState["dtbl"] = dtbl;

            gvSAP.DataSource = dtbl;//要綁定的資料表
            gvSAP.DataBind();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_PurposeBind()
    {
        dropCard_Purpose.DataTextField = "PARAM_NAME";
        dropCard_Purpose.DataValueField = "Param_Code";
        dropCard_Purpose.DataSource = Finance0021BL.getParam_Use();
        dropCard_Purpose.DataBind();
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_GroupBind()
    {
        dropCard_Group.Items.Clear();

        dropCard_Group.DataTextField = "GROUP_NAME";
        dropCard_Group.DataValueField = "RID";
        dropCard_Group.DataSource = Finance0021BL.getCardGroup(dropCard_Purpose.SelectedValue);
        dropCard_Group.DataBind();

        dropCard_Group.Items.Insert(0, new ListItem("全部", ""));
    }

    /// <summary>
    /// 空白卡廠下拉框綁定
    /// </summary>
    protected void dropFactoryBind()
    {
        dropFactory.Items.Clear();

        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataValueField = "RID";
        dropFactory.DataSource = Finance0021BL.getFactory();
        dropFactory.DataBind();

        dropFactory.Items.Insert(0, new ListItem("全部", ""));

    }

    /// <summary>
    /// 用途改變時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropCard_Purpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_GroupBind();
        gvpSapBind1();
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpSapBind();
    }

    /// <summary>
    /// 添加代製費用請款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("Finance0021Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Finance0021BL.Delete(e.CommandArgument.ToString());
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

        gvpSapBind();
    }

    /// <summary>
    /// 列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Command(object sender, CommandEventArgs e)
    {
        gvpSapBind1();
    }

    /// <summary>
    /// 行綁定SAP單訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvSAP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == e.Row.Cells.Count - 2 || i == e.Row.Cells.Count - 1)
                {
                    e.Row.Cells[i].Text = " ";
                }
                else
                {
                    e.Row.Cells[i].Text = e.Row.Cells[i + 2].Text + " ";
                }
            }

            e.Row.Cells[0].Visible = false;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dtSAP = (DataTable)ViewState["dtSAP"];

            e.Row.Cells[2].Visible = false;

            for (int n = 0; n < Convert.ToInt16(ViewState["rows"]); n++)
            {
                if (e.Row.Cells[n + 7].Text != "&nbsp;")
                {
                    e.Row.Cells[n + 7].Text = "<font color = red>(" + e.Row.Cells[n + 7].Text + ")</font>";
                }
            }

            HyperLink li = new HyperLink();
            li.Text = e.Row.Cells[5].Text;
            li.NavigateUrl = "Finance0021Edit.aspx?RID=" + dtSAP.Rows[e.Row.RowIndex]["RID"].ToString();
            e.Row.Cells[5].Controls.Add(li);

            e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
            if (e.Row.Cells[6].Text != "" && e.Row.Cells[6].Text != "&nbsp;")
            {
                e.Row.Cells[6].Text = Convert.ToDecimal(e.Row.Cells[6].Text).ToString("N2");
            }

            TableCell aa = e.Row.Cells[0];
            TableCell bb = e.Row.Cells[1];
            e.Row.Cells.Remove(aa);
            e.Row.Cells.Remove(bb);
            e.Row.Cells.Add(aa);
            e.Row.Cells.Add(bb);

        }
    }

    /// <summary>
    /// 列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button2_Click(object sender, EventArgs e)
    {
        string strRID = Request.Form[1];
        gvpSapBind1();

        DataTable dtSP = (DataTable)ViewState["dtSAP"];
        DataTable dtbl = (DataTable)ViewState["dtbl"];

        DataRow[] drows = dtSP.Select("RID=" + strRID);
        if (drows.Length > 0)
        {
            DataRow drow = drows[0];

            try
            {
                string strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                Finance0022BL.F0021SearchPrint(drow, drow["SAP_ID"].ToString(), drow["Check_Serial_Number"].ToString(), strTime);
                Response.Write("<script>window.open('Finance0021Print.aspx?Time=" + strTime + "&SAP_Serial_Number=1','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    /// <summary>
    /// 刪除某項請款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            string strcgrid = "";
            string date = "";
            string strRID = Request.Form[2];
            DataTable dtbl = (DataTable)ViewState["dtSAP"];
             DataRow[] drows = dtbl.Select("RID=" + strRID);
             if (drows.Length > 0)
             {
                 DataRow drow = drows[0];
                 strcgrid = drow["cgrid"].ToString();
                 date = drow["Begin_Date"].ToString();
             }
           
            if (!StringUtil.IsEmpty(strRID))
            {
                if (Finance0021BL.Delete(strRID))
                    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);

                int FC = Finance0021BL.ForcastCheck(date, strcgrid);
                if (FC == 1)
                {
                    throw new Exception("代製費用年度預算剩餘金額不足");
                }
                else if (FC == 2)
                {
                    ShowMessage("代製費用年度預算剩餘金額低於10%");
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

        gvpSapBind();
    }
}
