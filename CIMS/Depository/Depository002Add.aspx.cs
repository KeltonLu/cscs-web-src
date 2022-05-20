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

public partial class Depository_Depository002Add : PageBase
{
    DataTable dtclone = new DataTable();

    ArrayList al =new ArrayList();

    LoginManager lm = new LoginManager();

    Depository002BL depManager = new Depository002BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.GetPostBackClientEvent(this, "");

        this.gvpbORDERFORMDETAIL.PageSize = GlobalStringManager.PageSize;
        gvpbORDERFORMDETAIL.NoneData = "";

        if (!IsPostBack)
        {
            Session.Remove("detail");
            string str = Request.QueryString["type"];
            if(StringUtil.IsEmpty(str))
                Session.Remove("monitory");
            if (Session["monitory"] != null)
            {
                DataTable dtemp = (DataTable)Session["monitory"];
                Session["detail"] = dtemp;
                gvpbORDERFORMDETAIL.BindData();
            }
        }
    }

    protected void btnAddDetail_Click(object sender, EventArgs e)
    {
        gvpbORDERFORMDETAIL.BindData();
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtdetail = (DataTable)Session["detail"];

        if (dtdetail != null && dtdetail.Rows.Count > 0)
        {
            DataRow dr = dtdetail.Rows[int.Parse(e.CommandArgument.ToString())];
            if (dr["orderform_detail_rid"].ToString() !="")           
                depManager.RD(dr);
            dtdetail.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

            Session["detail"] = dtdetail;
            gvpbORDERFORMDETAIL.BindData();
        }
    }
    
    //提交
    protected void btnCommit_Click(object sender, EventArgs e)
    {
        DataTable dtdetail = (DataTable)Session["detail"];

        if (dtdetail != null && dtdetail.Rows.Count > 0)
        {
            if (depManager.IsSaveDB(dtdetail))
            {
                depManager.Confirm(dtdetail);                
                Response.Redirect("Depository002.aspx?Con=1");
            }
            else
            {
                ShowMessage("有資料未保存入庫！");
            }
        }
        else
        {
            ShowMessage("無可提交資料！");
        }
    }

    //匯出Excel
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        DataTable dtdetail = (DataTable)Session["detail"];
        if (dtdetail != null && dtdetail.Rows.Count > 0)
        {
            string str = "";
            for (int i = 0; i < dtdetail.Rows.Count; i++)
            {
                if (i == 0)
                {
                    str += dtdetail.Rows[i]["orderform_rid"].ToString();
                }
                else
                {
                    str += ","+dtdetail.Rows[i]["orderform_rid"].ToString();
                }
            }
            Response.Write("<script>window.open('Depository002Print.aspx?orderform_rid=" + str + "');</script>");
        }
        else
        {
            ShowMessage("沒有可列印資料！");
        }
    }

    //確定
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dtdetail = (DataTable)Session["detail"];

            if (dtdetail != null && dtdetail.Rows.Count > 0)
            {
                if (ViewState["array"] != null)
                {
                    dtclone = (DataTable)ViewState["array"];
                }

                if (dtclone.Rows.Count > 0)
                {
                    depManager.RDAU(dtclone, dtdetail);
                }
                else
                {
                    depManager.RDAU(dtdetail, dtdetail);
                }
                                
                ViewState["array"] = dtdetail;

                gvpbORDERFORMDETAIL.BindData();
            }
            else
            {
                ShowMessage("沒有訂單記錄需要保存！");
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbORDERFORMDETAIL_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {

        DataTable dtdetail = (DataTable)Session["detail"];
        if (dtdetail != null && dtdetail.Rows.Count > 0)
        {
            e.Table = dtdetail;
            e.RowCount = dtdetail.Rows.Count;
        }
        else
        {
            e.Table = dtdetail;
            e.RowCount = 0;
        }
    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbORDERFORMDETAIL_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblOrderForm = (DataTable)gvpbORDERFORMDETAIL.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblOrderForm.Rows.Count == 0)
                return;

            Label lblis_exigence = null;
            lblis_exigence = (Label)e.Row.FindControl("lblis_exigence");

            if (dtblOrderForm.Rows[e.Row.RowIndex]["is_exigence"].ToString() == GlobalString.Exigence.Urgent)
            {
                lblis_exigence.Text = "Urgent";
            }
            else if (dtblOrderForm.Rows[e.Row.RowIndex]["is_exigence"].ToString() == GlobalString.Exigence.Normal)
            {
                lblis_exigence.Text = "Normal";
            }
            else
            {
                lblis_exigence.Text = "Normal";
            }

            ImageButton ibtnButton = null;

            try
            {
                if (e.Row.Cells[1].Text != "" && e.Row.Cells[1].Text != "&nbsp;")
                {
                    e.Row.Cells[1].Text = int.Parse(e.Row.Cells[1].Text).ToString("N0");
                }
            }
            catch { }

            // 刪除的邦定事件
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            HyperLink hl = (HyperLink)e.Row.FindControl("Space_Short_RID");
            hl.Text = depManager.GetSpace_Short_RID(dtblOrderForm.Rows[e.Row.RowIndex]["Space_Short_RID"].ToString());
            hl.NavigateUrl = "#";
            hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository002Detaila.aspx?ActionType=Edit&RID=" + e.Row.RowIndex.ToString() + "','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");

            // add by Ian Huang start
            if (e.Row.Cells[4].Text.Trim() == e.Row.Cells[5].Text.Trim())
            {
                e.Row.Cells[5].Text = "";
            }
            else
            {
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
            }
            // add by Ian Huang end
        }
    }
    #endregion       
    
    
    
}
