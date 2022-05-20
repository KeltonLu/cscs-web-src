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

public partial class MainFrame : System.Web.UI.Page
{
    MainFrameManager mfmFrameManager = new MainFrameManager();//框架頁管理

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (Session[GlobalString.SessionAndCookieKeys.USER] == null)//有用戶信息
        {
            string strErr = GlobalStringManager.Default["Alert_NotLogin"];
            ExceptionFactory.CreateAlertException(this, strErr);
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "window.open('Login.aspx','_top');", true);
            return;
        }

        if (HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.ACTIONS] == null)
        {
            string strErr = GlobalStringManager.Default["Alert_NotLogin"];
            //ExceptionFactory.CreateAlertSaveException(this, strErr, strErr);
        }
        else
            mfmFrameManager.GetMenuData(MnuMain);//顯示菜單

        try
        {
            lblUserName.Text = ((USERS)Session[GlobalString.SessionAndCookieKeys.USER]).UserName + "(" + DateTime.Now.ToString("yyyy/MM/dd") + ")";
        }
        catch
        {
        }
    }
}
