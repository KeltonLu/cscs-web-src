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

public partial class header : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblUserName.Text = ((USERS)Session[GlobalString.SessionAndCookieKeys.USER]).UserName + "(" + DateTime.Now.ToString("yyyy/MM/dd") + ")";
        }
        catch
        {
        }
    }
}
