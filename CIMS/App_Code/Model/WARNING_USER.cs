using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


/// <summary>
/// WARNING_USER
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class WARNING_USER
{
    public WARNING_USER()
    { }

    #region Model
    private int _Warning_RID;
    private string _UserID;

    /// <summary>
    /// 
    /// </summary>
    public int Warning_RID
    {
        get { return _Warning_RID; }
        set { _Warning_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string UserID
    {
        get { return _UserID; }
        set { _UserID = value; }
    }

    #endregion


}

