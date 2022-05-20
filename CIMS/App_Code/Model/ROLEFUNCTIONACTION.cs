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
/// ROLEFUNCTIONACTION
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class ROLEFUNCTIONACTION
{
    public ROLEFUNCTIONACTION()
    { }

    #region Model
    private string _RoleID;
    private string _FunctionID;
    private string _ActionID;

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
    public string FunctionID
    {
        get { return _FunctionID; }
        set { _FunctionID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string ActionID
    {
        get { return _ActionID; }
        set { _ActionID = value; }
    }

    #endregion


}

