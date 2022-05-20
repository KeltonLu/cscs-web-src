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

public partial class Warning : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string ErrorPath = Request.QueryString["ErrorPath"];
            if (ErrorPath != "")
                btnOK.Attributes.Add("onclick", "javascript:window.open('" + ErrorPath + "','_self');");
            LoadErrorMessage();
            this.detail.Visible = false;
        }
    }

    private void LoadErrorMessage()
    {
        if(Session["exception"] != null)
        {
            Exception ex = (Exception)Session["exception"];
            lblError.Text = @""+ex.Message.Replace("<","").Replace(">","");
            txtMessage.Text = @"" + ex.ToString().Replace("<", "").Replace(">", "");
        }
    }
    protected void showdetail_ServerClick(object sender, EventArgs e)
    {
        if (this.detail.Visible)
        {
            this.showdetail.Value = "显示详细 ▼";
            this.detail.Style["display"] = "none";
            this.detail.Visible = false;
        }
        else
        {
            this.showdetail.Value = "隐藏详细 ▲";
            this.detail.Style["display"] = "";
            txtMessage.Height = 200;
            this.detail.Visible = true;
        }
    }

    protected override void OnError(EventArgs e)
    {

        System.Exception ex = Server.GetLastError();
        Server.ClearError();

        Session["exception"] = ex;
        Response.Redirect("~/Warning.aspx?ErrorPath=" + Request.RawUrl);
    }
}
