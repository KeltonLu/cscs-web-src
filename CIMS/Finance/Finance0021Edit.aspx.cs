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

public partial class Finance_Finance0021Edit : PageBase
{
    Finance0021BL Finance0021BL = new Finance0021BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            gvpSapBind();
        }
    }

    protected void gvSAP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == e.Row.Cells.Count - 4)
                {
                    e.Row.Cells[i].Text = "SAP單號";
                }
                else if (i == e.Row.Cells.Count - 3)
                {
                    e.Row.Cells[i].Text = "請款日";
                }
                else if (i == e.Row.Cells.Count - 2)
                {
                    e.Row.Cells[i].Text = "出帳日";
                }
                else if (i == e.Row.Cells.Count - 1)
                {
                    e.Row.Cells[i].Text = "發票號碼";
                }
                else
                {
                    e.Row.Cells[i].Text = e.Row.Cells[i + 4].Text + " ";
                }
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dtbl = (DataTable)ViewState["dtSAP"];

            TextBox txt1 = (TextBox)e.Row.FindControl("txt1");
            TextBox txt2 = (TextBox)e.Row.FindControl("txt2");
            TextBox txt3 = (TextBox)e.Row.FindControl("txt3");
            TextBox txt4 = (TextBox)e.Row.FindControl("txt4");


            txt1.Text = dtbl.Rows[e.Row.RowIndex]["SAP_ID"].ToString().Trim();
            txt2.Text = dtbl.Rows[e.Row.RowIndex]["Ask_Date"].ToString().Trim();
            txt3.Text = dtbl.Rows[e.Row.RowIndex]["Pay_Date"].ToString().Trim();
            txt4.Text = dtbl.Rows[e.Row.RowIndex]["Check_Serial_Number"].ToString().Trim();

            if (ViewState["dtblView"] != null)
            {
                DataTable dtblView = (DataTable)ViewState["dtblView"];
                DataRow drow = dtblView.Rows[e.Row.RowIndex];
                txt1.Text = drow["txt1"].ToString();
                txt2.Text = drow["txt2"].ToString();
                txt3.Text = drow["txt3"].ToString();
                txt4.Text = drow["txt4"].ToString();
            }

            if (txt3.Text == "1900/01/01")
            {
                txt3.Text = "";
            }

            e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Right;

            if(e.Row.Cells[5].Text!="&nbsp;")
            {
                try
                {
                    e.Row.Cells[5].Text = Convert.ToDecimal(e.Row.Cells[5].Text).ToString("N2");
                }
                catch { }
            }

            for (int n = 0; n < Convert.ToInt16(ViewState["rows"]); n++)
            {
                if (e.Row.Cells[n + 6].Text != "&nbsp;")
                {
                    e.Row.Cells[n + 6].Text = "<font color = red>(" + e.Row.Cells[n + 6].Text + ")</font>";
                }
            }

            TableCell aa = e.Row.Cells[0];
            TableCell bb = e.Row.Cells[1];
            TableCell cc = e.Row.Cells[2];
            TableCell dd = e.Row.Cells[3];
            e.Row.Cells.Remove(aa);
            e.Row.Cells.Remove(bb);
            e.Row.Cells.Remove(cc);
            e.Row.Cells.Remove(dd);
            e.Row.Cells.Add(aa);
            e.Row.Cells.Add(bb);
            e.Row.Cells.Add(cc);
            e.Row.Cells.Add(dd);
        }
    }

    private void gvpSapBind1()
    {
        if (ViewState["dtbl"] == null)
            return;

        DataTable dtbl = (DataTable)ViewState["dtbl"];

        gvSAP.DataSource = dtbl;//要綁定的資料表
        gvSAP.DataBind();
    }

    private void gvpSapBind()
    {
        DataTable dtSAP = new DataTable();

        try
        {
            string strRID = Request.QueryString["RID"];
            if (!StringUtil.IsEmpty(strRID))
            {
                dtSAP = Finance0021BL.getAskMoneyEdit(Convert.ToInt32(strRID)).Tables[0];
            }

            ViewState["dtSAP"] = dtSAP;

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("帳務群組");
            dtbl.Columns.Add("出帳金額");

            int i = 0;
            DataTable dtParam_Change = Finance0021BL.getParam_Change().Tables[0];
            foreach (DataRow dr in dtParam_Change.Rows)
            {
                dtbl.Columns.Add("lb" + dr["Param_Code"].ToString());
                i++;
            }

            ViewState["rows"] = i;

            foreach (DataRow drowSap in dtSAP.Rows)
            {
                DataRow drow = dtbl.NewRow();
                drow["帳務群組"] = drowSap["Group_Name"].ToString();
                //modify by chaoma start
                //drow["出帳金額"] = drowSap["Sum"].ToString();
                drow["出帳金額"] = Math.Round(Convert.ToDecimal(drowSap["Sum"]), 0, MidpointRounding.AwayFromZero);
                //modify by chaoma end

                DataTable dtSearchChange = Finance0021BL.SearchChange(drowSap["Perso_Factory_RID"].ToString(), drowSap["Group_RID"].ToString(), Convert.ToDateTime(drowSap["Begin_Date"].ToString()), Convert.ToDateTime(drowSap["End_Date"].ToString())).Tables[0];
                foreach (DataRow dr in dtSearchChange.Rows)
                {
                    for (int n = 0; n < i; n++)
                    {
                        string strColname = dtbl.Columns[2 + n].ColumnName.Replace("lb", "");
                        if (strColname == dr["Param_Code"].ToString())
                        {
                            drow[2 + n] = dr[2].ToString();
                        }
                    }
                }

                dtbl.Rows.Add(drow);
            }

            foreach (DataRow dr in dtParam_Change.Rows)
            {
                dtbl.Columns["lb" + dr["Param_Code"].ToString()].ColumnName = dr["Param_Name"].ToString();
            }

            ViewState["dtbl"] = dtbl;
            ViewState["dtblView"] = null;

            gvSAP.DataSource = dtbl;//要綁定的資料表
            gvSAP.DataBind();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    protected void btnSumbit_Click(object sender, EventArgs e)
    {
        string strRID = "";
        string strcgrid = "";
        string date = "";
        decimal Card = 0;//卡单价
        try
        {
            DataTable dtblView = new DataTable();
            dtblView.Columns.Add("txt1");
            dtblView.Columns.Add("txt2");
            dtblView.Columns.Add("txt3");
            dtblView.Columns.Add("txt4");
            for (int i = 0; i < gvSAP.Rows.Count; i++)
            {
                DataRow drow = dtblView.NewRow();
                TextBox txt1 = (TextBox)gvSAP.Rows[i].FindControl("txt1");
                TextBox txt2 = (TextBox)gvSAP.Rows[i].FindControl("txt2");
                TextBox txt3 = (TextBox)gvSAP.Rows[i].FindControl("txt3");
                TextBox txt4 = (TextBox)gvSAP.Rows[i].FindControl("txt4");
                drow["txt1"] = txt1.Text;
                drow["txt2"] = txt2.Text;
                drow["txt3"] = txt3.Text;
                drow["txt4"] = txt4.Text;
                dtblView.Rows.Add(drow);
            }
            ViewState["dtblView"] = dtblView;

            for (int i = 0; i < gvSAP.Rows.Count; i++)
            {
                TextBox txt1 = (TextBox)gvSAP.Rows[i].FindControl("txt1");
                TextBox txt2 = (TextBox)gvSAP.Rows[i].FindControl("txt2");
                TextBox txt3 = (TextBox)gvSAP.Rows[i].FindControl("txt3");
                TextBox txt4 = (TextBox)gvSAP.Rows[i].FindControl("txt4");

                DataTable dtbl = (DataTable)ViewState["dtSAP"];
                strRID = dtbl.Rows[i]["RID"].ToString();
                strcgrid = dtbl.Rows[i]["cgrid"].ToString();
                date = txt2.Text;
                Card = Convert.ToDecimal(gvSAP.Rows[i].Cells[1].Text);

                if (txt3.Text != "")
                {
                     TimeSpan ts = Convert.ToDateTime(txt2.Text).Subtract(Convert.ToDateTime(txt3.Text));
                    if (ts.Days > 0)
                    {
                        throw new Exception("出帳日期不能小於請款日期");
                    }
                }
                if (txt2.Text != "")
                {
                    //if (txt1.Text.Length != 15)
                    //{
                    //    txt1.Focus();
                    //    throw new Exception("SAP單號必須為15位");
                    //}
                    //if (Convert.ToInt32(Finance0021BL.CONSAP(txt1.Text.Trim()).Tables[0].Rows[0][0]) != 0)
                    //{
                    //    txt1.Focus();
                    //    throw new Exception("SAP單號已經存在");
                    //}

                    if (txt4.Text == "")
                    {
                        txt4.Focus();
                        throw new Exception("必須輸入發票編號");
                    }
                    else if (Finance0021BL.getAllCheckSerialNumber_1(txt4.Text.Trim(), strRID))//檢查該行的發票編號和系統發票編號相重復
                    {
                        txt4.Focus();
                        throw new Exception("發票編號已經存在");
                    }
                }
            }
            if (!StringUtil.IsEmpty(strRID))
            {
                // 1;//代製費用年度預算剩餘金額不足  
                // 2;//代製費用年度預算剩餘金額低於10%
                int FC = Finance0021BL.ForcastCheck(date, Card, strRID, strcgrid);
                if (FC == 1)
                {
                    throw new Exception("代製費用年度預算剩餘金額不足");
                }
                else if (FC == 2)
                {
                    ShowMessage("代製費用年度預算剩餘金額低於10%");
                }

                Finance0021BL.Update(gvSAP, strRID);
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_SaveSuccess"], "Finance0021.aspx?Con=1");
            }
        }
        catch (Exception ex)
        {
            gvpSapBind1();
            ShowMessage(ex.Message);
        }
    }

    protected void btnCanel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0021.aspx?Con=1");
    }
}
