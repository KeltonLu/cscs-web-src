//******************************************************************
//*  作    者：FangBao
//*  功能說明：卡片入庫人工增加
//*  創建日期：2008-09-05
//*  修改日期：2008-09-05 12:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 鮑方
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
using System.Text;

public partial class Depository_Depository003Add : PageBase
{
    Depository003BL BL = new Depository003BL();

    #region 事件處理

    protected void Page_Load(object sender, EventArgs e)
    {
        //允許使用DoPostBack
        this.GetPostBackClientEvent(this, "");
        gvpbImportStock.NoneData = "";

        if (!IsPostBack)
        {
            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("Card_Purpose_RID");
            dtbl.Columns.Add("Card_Purpose_NAME");
            dtbl.Columns.Add("Card_Group_RID");
            dtbl.Columns.Add("Card_Group_NAME");
            dtbl.Columns.Add("Space_Short_RID");
            dtbl.Columns.Add("Space_Short_NAME");
            dtbl.Columns.Add("Stock_Number");
            dtbl.Columns.Add("Blemish_Number");
            dtbl.Columns.Add("Sample_Number");
            dtbl.Columns.Add("Income_Number");
            dtbl.Columns.Add("Income_Date");
            dtbl.Columns.Add("Serial_Number");
            dtbl.Columns.Add("Perso_Factory_RID");
            dtbl.Columns.Add("Perso_Factory_NAME");
            dtbl.Columns.Add("Blank_Factory_RID");
            dtbl.Columns.Add("Blank_Factory_NAME");
            dtbl.Columns.Add("Wafer_RID");
            dtbl.Columns.Add("Wafer_NAME");
            dtbl.Columns.Add("Check_Type");
            dtbl.Columns.Add("Comment");
            dtbl.Columns.Add("SendCheck_Status");

            Session["dtblImp"] = dtbl;
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        DataTable dtbl = (DataTable)Session["dtblImp"];
        if (dtbl.Rows.Count == 0)
        {
            ShowMessage("無添加列");
            return;
        }

        try
        {
            string strRID = BL.AddMan(dtbl);


            StringBuilder stbCode = new StringBuilder("<script language=\"javascript\" type=\"text/ecmascript\">alert('儲存成功');");
            stbCode.Append("window.open('Depository003ImpPrint.aspx?RID=" + strRID.Split(',')[0] + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=850,height=650');");
            stbCode.Append("window.location='Depository003Add.aspx';");
            stbCode.Append("</script>");
            this.Response.Write(stbCode.ToString());
            //this.Response.End();

            Session.Remove("DepositoryAdd");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
        
    protected void btnBind_Click(object sender, EventArgs e)
    {
        gvpbImportStock.BindData();
    }
    #endregion

    #region 列表資料綁定
    protected void gvpbImportStock_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtbl = (DataTable)Session["dtblImp"];
        e.Table = dtbl;
        e.RowCount = dtbl.Rows.Count;
    }

    protected void gvpbImportStock_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbImportStock.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtbl.Rows.Count == 0)
                return;

            ImageButton ibtnButton = null;

            // 刪除的邦定事件
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            if (e.Row.Cells[7].Text != "&nbsp;")
            {
                e.Row.Cells[7].Text = Convert.ToDateTime(e.Row.Cells[7].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
            if (e.Row.Cells[12].Text != "&nbsp;")
            {
                e.Row.Cells[12].Text = e.Row.Cells[12].Text.Replace("1", "抽驗").Replace("2", "全驗");
            }
            if (e.Row.Cells[3].Text != "&nbsp;")
            {
                e.Row.Cells[3].Text = Convert.ToInt32(e.Row.Cells[3].Text).ToString("N0");
            }
            if (e.Row.Cells[4].Text != "&nbsp;")
            {
                e.Row.Cells[4].Text = Convert.ToInt32(e.Row.Cells[4].Text).ToString("N0");
            }
            if (e.Row.Cells[5].Text != "&nbsp;")
            {
                e.Row.Cells[5].Text = Convert.ToInt32(e.Row.Cells[5].Text).ToString("N0");
            } 
            if (e.Row.Cells[6].Text != "&nbsp;")
            {
                e.Row.Cells[6].Text = Convert.ToInt32(e.Row.Cells[6].Text).ToString("N0");
            }

            HyperLink hl = (HyperLink)e.Row.FindControl("hlCardType");
            hl.Text = dtbl.Rows[e.Row.RowIndex]["Space_Short_NAME"].ToString();
            hl.NavigateUrl = "#";
            hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository003AddDetail.aspx?ActionType=Add&RID=" + e.Row.RowIndex.ToString() + "','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");

        }
    }

    /// <summary>
    /// 刪除追加預算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtbl = (DataTable)Session["dtblImp"];

        dtbl.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

        Session["dtblImp"] = dtbl;

        gvpbImportStock.BindData();
    }

    #endregion

}
