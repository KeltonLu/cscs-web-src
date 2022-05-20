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

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //MyLogger ml = new MyLogger(GlobalString.LogType.ErrorCategory);
        //ml.Write("dfdf", GlobalString.LogType.ErrorCategory);

        LogFactory.Write("aaaaa", GlobalString.LogType.ErrorCategory);
        LogFactory.Write("bbbbb", GlobalString.LogType.OpLogCategory);
        LogFactory.Write("ccccc", GlobalString.LogType.ErrorCategory);
        LogFactory.Write("ddddd", GlobalString.LogType.OpLogCategory);

        
       
    }
}
