//******************************************************************
//*  作    者：FangBao
//*  功能說明：卡片入庫自動匯入結案
//*  創建日期：2008-09-04
//*  修改日期：2008-09-04 12:00
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

public partial class Depository_Depository003CaseClose : PageBase
{
    Depository003BL BL = new Depository003BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];
            string strType = Request.QueryString["ActionType"];
            if (!StringUtil.IsEmpty(strRID))
            {
                if (strType == "Add")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "window.open('Depository003ImpPrint.aspx?RID=" + strRID.Split(',')[0] + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=850,height=650');", true);
                }
                gvpbImportStock.BindData();
            }
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string strType = Request.QueryString["ActionType"];

        string strDetail_RID_Y = "";
        string strDetail_RID_N = "";

        for (int i = 0; i < gvpbImportStock.Rows.Count; i++)
        {
            if (((CheckBox)gvpbImportStock.Rows[i].FindControl("cbCase_Status")).Checked)
            {
                strDetail_RID_Y += gvpbImportStock.DataKeys[i].Value.ToString() + ",";
            }
            else
            {
                strDetail_RID_N += gvpbImportStock.DataKeys[i].Value.ToString() + ",";
            }
        }
        try
        {
            BL.Close(strDetail_RID_Y, strDetail_RID_N);

            if (strType == "Add")
                ShowMessageAndGoPage("儲存成功", "Depository003ImpAuto.aspx");
            else
                ShowMessageAndGoPage("儲存成功", "Depository003.aspx?Con=1");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    
    #endregion

    #region 列表資料綁定.
    protected void gvpbImportStock_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        string strRID = Request.QueryString["RID"];

        DataSet dst = BL.GetOrderFormDetail(strRID);

        if (dst != null)
        {
            e.Table = dst.Tables[0];
            e.RowCount = dst.Tables[0].Rows.Count;
        }
    }

    protected void gvpbImportStock_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblImp = (DataTable)gvpbImportStock.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblImp.Rows.Count == 0)
                return;

            Label lbl = (Label)e.Row.FindControl("lbNumber");

            string strOrderForm_Detail_RID = dtblImp.Rows[e.Row.RowIndex]["OrderForm_Detail_RID"].ToString();
            DataSet dst = BL.GetDetailByOrderFormDetailNo(strOrderForm_Detail_RID);
            int CaryNum = int.Parse(dst.Tables[1].Rows[0][0].ToString()) - int.Parse(dst.Tables[2].Rows[0][0].ToString()) + int.Parse(dst.Tables[3].Rows[0][0].ToString());
            lbl.Text = CaryNum.ToString("N0");

            CheckBox cbx = (CheckBox)e.Row.FindControl("cbCase_Status");
            string strCase_Status = dst.Tables[0].Rows[0]["Case_Status"].ToString();
            if (strCase_Status == "Y")
                cbx.Checked = true;
        }
    }
    #endregion

   
}
