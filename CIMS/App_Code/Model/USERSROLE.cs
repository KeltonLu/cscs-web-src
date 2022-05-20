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
/// USERSROLE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class USERSROLE
{
    public USERSROLE()
    { }

    #region Model
    private string _UserID;
    private string _RoleID;

    /// <summary>
    /// 
    /// </summary>
    public string UserID
    {
        get { return _UserID; }
        set { _UserID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string RoleID
    {
        get { return _RoleID; }
        set { _RoleID = value; }
    }

    #endregion


}

