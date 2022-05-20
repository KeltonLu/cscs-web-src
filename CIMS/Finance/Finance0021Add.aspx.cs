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
using System.Text.RegularExpressions;

public partial class Finance_Finance0021Add : PageBase
{
    Finance0021BL Finance0021BL = new Finance0021BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //(起)(迄)都設為當天
            this.txtDateFrom.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.txtDateTo.Text = DateTime.Now.ToString("yyyy/MM/dd");
            dropFactoryBind();//Perso厰卡廠下拉框綁定
            btnSumbit.Visible = false;
        }

    }

    private void gvpSapBind()
    {
        DataTable dtSAP = null;

        try
        {
            //計算出各Perso廠、各帳務群組之代製費用出帳金額
            string bln = "";
            dtSAP = Finance0021BL.getMakeCost(txtDateFrom.Text,
                txtDateTo.Text,
                dropFactory.SelectedValue,
                ref bln).Tables[0];

            if (bln != "")
            {
                ShowMessage(bln);
            }
            if (dtSAP == null)
            {
                return;
            }
            ViewState["dtSAP"] = dtSAP;
            if (dtSAP.Rows.Count != 0)
            {
                btnSumbit.Visible = true;
                lbmsg.Visible = false;
            }
            else
            {
                lbmsg.Visible = true;
                btnSumbit.Visible = false;
            }

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("Perso廠商");
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
                drow["Perso廠商"] = drowSap["Factory_ShortName_CN"].ToString();
                drow["帳務群組"] = drowSap["Group_Name"].ToString();
                //modify by chaoma start
                //drow["出帳金額"] = drowSap["Sum"].ToString();
                drow["出帳金額"] = Math.Round(Convert.ToDecimal(drowSap["Sum"]), 0, MidpointRounding.AwayFromZero);
                //modify by chaoma end
                DataTable dtSearchChange = Finance0021BL.SearchChange(drowSap["Perso_Factory_RID"].ToString(), drowSap["Group_RID"].ToString(), Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text)).Tables[0];
                foreach (DataRow dr in dtSearchChange.Rows)
                {
                    for (int n = 0; n < i; n++)
                    {
                        string strColname = dtbl.Columns[3 + n].ColumnName.Replace("lb", "");
                        if (strColname == dr["Param_Code"].ToString())
                        {
                            drow[3 + n] = dr[2].ToString();
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

    private void gvpSapBind1()
    {
        if (ViewState["dtbl"] == null)
            return;

        DataTable dtbl = (DataTable)ViewState["dtbl"];

        gvSAP.DataSource = dtbl;//要綁定的資料表
        gvSAP.DataBind();
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

        dropFactory.Items.Insert(0, new ListItem("", ""));
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSumbit_Click(object sender, EventArgs e)
    {
        decimal ctk = 0;//磁條信用卡
        decimal jrk = 0;//晶片金融卡
        decimal xyk = 0;//晶片信用卡
        decimal xjk = 0;//現金卡
        decimal DEBIT = 0;//VISA DEBIT卡

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
            bool nbl = false;
            for (int i = 0; i < gvSAP.Rows.Count; i++)
            {
                TextBox txt1 = (TextBox)gvSAP.Rows[i].FindControl("txt1");
                TextBox txt2 = (TextBox)gvSAP.Rows[i].FindControl("txt2");
                TextBox txt3 = (TextBox)gvSAP.Rows[i].FindControl("txt3");
                TextBox txt4 = (TextBox)gvSAP.Rows[i].FindControl("txt4");

                DataTable dtbl = (DataTable)ViewState["dtSAP"];

                if (txt1.Text.Trim() != "")
                {
                    if (txt2.Text.Trim() == "" || txt4.Text.Trim() == "")
                    {
                        throw new Exception("資料輸入不完整");
                    }
                }
                else
                {
                    continue;
                }

                nbl = true;
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
                   


                    if (IsExist(txt4.Text.Trim(), i))//檢查該行的發票編號是否和頁面上其他行的發票編號相重復
                    {
                        txt4.Focus();
                        throw new Exception("必須不相同的發票編號");
                    }
                    if (Finance0021BL.getAllCheckSerialNumber(txt4.Text.Trim()))//檢查該行的發票編號和系統發票編號相重復
                    {
                        txt4.Focus();
                        throw new Exception("發票編號已經存在");
                    }

                    switch (gvSAP.Rows[i].Cells[1].Text.Trim())
                    {
                        case "磁條信用卡":
                            ctk += Convert.ToDecimal(gvSAP.Rows[i].Cells[2].Text);
                            break;
                        case "晶片金融卡":
                            jrk += Convert.ToDecimal(gvSAP.Rows[i].Cells[2].Text);
                            break;
                        case "晶片信用卡":
                            xyk += Convert.ToDecimal(gvSAP.Rows[i].Cells[2].Text);
                            break;
                        case "現金卡":
                            xjk += Convert.ToDecimal(gvSAP.Rows[i].Cells[2].Text);
                            break;
                        case "VISA DEBIT卡":
                            DEBIT += Convert.ToDecimal(gvSAP.Rows[i].Cells[2].Text);
                            break;
                        default:
                            break;
                    }
                }
            }

            if(!nbl)
                throw new Exception("無添加資料");

            // 1;//代製費用年度預算剩餘金額不足  
            // 2;//代製費用年度預算剩餘金額低於10%
            int FC = Finance0021BL.ForcastCheck(txtDateFrom.Text, ctk, jrk, xyk, xjk, DEBIT);
            if (FC == 1)
            {
                throw new Exception("代製費用年度預算剩餘金額不足");
            }
            else if (FC == 2)
            {
                ShowMessage("代製費用年度預算剩餘金額低於10%");
            }

            Finance0021BL.Save(gvSAP, Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text), (DataTable)ViewState["dtSAP"]);

            Finance0022BL Finance0022BL = new Finance0022BL();
            string strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            Finance0022BL.F0021Print(gvSAP, Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text), (DataTable)ViewState["dtSAP"],strTime);


            string stbCode = "alert('存儲成功');window.location='Finance0021Add.aspx';window.open('Finance0021Print.aspx?Time="+strTime+"','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), stbCode, true);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
            gvpSapBind1();
        }
    }

    protected void btnCanel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0021.aspx?Con=1");
    }

    /// <summary>
    /// 計算代製費用
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCal_Click(object sender, EventArgs e)
    {
        if (Convert.ToDateTime(txtDateFrom.Text).Year != Convert.ToDateTime(txtDateTo.Text).Year)
        {
            ShowMessage("卡片耗用日期區間不能跨年查询");

            return;
        }
        //檢查卡片耗用日期區間所有工作日是否都日結
        DateTime Date = Finance0021BL.CheckEachWorkDateIsSurplus(Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text));
        if (Date != Convert.ToDateTime("1900/01/01"))
        {
            ShowMessage(Date.ToString("yyyy/MM/dd") + "之後未日結");
            return;
        }
        //檢查卡片耗用日期區間是否和其他請款區間相重疊
        if (Finance0021BL.CheckAgainSurplus(Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text)))
        {
            ShowMessage("卡片耗用日期區間重疊");
            return;
        }

        gvpSapBind();

        if (ViewState["dtSAP"] !=null && ((DataTable)ViewState["dtSAP"]).Rows.Count > 0)
        {
            txtDateFrom.Enabled = false;
            txtDateTo.Enabled = false;
            btnCal.Enabled = false;
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
            txt2.Text = DateTime.Now.ToShortDateString();

            if (ViewState["dtblView"] != null)
            {
                DataTable dtblView = (DataTable)ViewState["dtblView"];
                DataRow drow = dtblView.Rows[e.Row.RowIndex];
                txt1.Text = drow["txt1"].ToString();
                txt2.Text = drow["txt2"].ToString();
                txt3.Text = drow["txt3"].ToString();
                txt4.Text = drow["txt4"].ToString();
            }

            e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;

            if (e.Row.Cells[6].Text != "&nbsp;")
            {
                try
                {
                    e.Row.Cells[6].Text = Convert.ToDecimal(e.Row.Cells[6].Text).ToString("N2");
                }
                catch { }
            }

            for (int n = 0; n < Convert.ToInt16(ViewState["rows"]); n++)
            {
                if (e.Row.Cells[n + 7].Text != "&nbsp;")
                {
                    e.Row.Cells[n + 7].Text = "<font color = red>(" + e.Row.Cells[n + 7].Text + ")</font>";
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

    public bool IsExist(string Check_Serial_Number, int j)
    {
        for (int i = 0; i < gvSAP.Rows.Count; i++)
        {
            TextBox txt4 = (TextBox)gvSAP.Rows[i].FindControl("txt4");
            if (i == j)
                continue;
            if (txt4.Text.Trim() == Check_Serial_Number)
            {
                return true;
            }
        }
        return false;
    }
}