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
/// ROLE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class ROLE
{
    public ROLE()
    { }

    #region Model
    private string _RoleID;
    private string _RoleName;

    /// <summary>
    /// 
    /// </summary>
    public string RoleID
    {
        get { return _RoleID; }
        set { _RoleID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string RoleName
    {
        get { return _RoleName; }
        set { _RoleName = value; }
    }

    #endregion


}

