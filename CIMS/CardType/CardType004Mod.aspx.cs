using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class CardType_CardType004Mod : PageBase
{
    CardType004BL bl = new CardType004BL();
   
    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbPrice.NoneData = "";

        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];

            DataSet dst = bl.LoadPersoProject(strRID);

            if (dst.Tables[0].Rows.Count > 0)
                SetControlsForDataRow(dst.Tables[0].Rows[0]);

            string strStep = "";
            foreach (DataRow drow in dst.Tables[1].Rows)
            {
                strStep += drow["name"].ToString() + ",";
            }
            if (!StringUtil.IsEmpty(strStep))
                strStep = strStep.Substring(0, strStep.Length - 1);

            lblStepName.Text = strStep;

            if (dst.Tables[0].Rows[0]["Normal_Special"].ToString() == "1")
            {
                tr1.Visible = true;
                tr2.Visible = false;
                ViewState["Step"] = dst.Tables[2];
                gvpbPrice.BindData();
            }
            else
            {
                tr1.Visible = false;
                tr2.Visible = true;
            }
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            // 刪除Perso項目檔
            if (this.chkDel.Checked)
            {
                string strRID = Request.QueryString["RID"];

                bl.Delete(strRID);

                ShowMessageAndGoPage("刪除成功", "CardType004.aspx?Con=1&List=radProject_Name");
            }
            else
            {
                Response.Redirect("CardType004.aspx?Con=1&List=radProject_Name");
            }
           
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #endregion

    protected void gvpbPrice_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtbl = (DataTable)ViewState["Step"];

        e.Table = dtbl;
        e.RowCount = dtbl.Rows.Count;
    }
}