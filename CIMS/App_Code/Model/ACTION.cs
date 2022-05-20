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
/// ACTION
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class ACTION
{
    public ACTION()
    { }

    #region Model
    private string _ActionID;
    private string _FunctionID;
    private string _ActionName;

    /// <summary>
    /// 
    /// </summary>
    public string ActionID
    {
        get { return _ActionID; }
        set { _ActionID = value; }
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
    public string ActionName
    {
        get { return _ActionName; }
        set { _ActionName = value; }
    }

    #endregion


}

