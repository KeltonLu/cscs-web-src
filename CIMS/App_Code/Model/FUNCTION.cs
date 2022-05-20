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
/// FUNCTION
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class FUNCTION
{
    public FUNCTION()
    { }

    #region Model
    private string _FunctionID;
    private string _ParentFunctionID;
    private string _FunctionName;
    private int _Level;
    private bool _InMenu;
    private string _FunctionUrl;
    private int _SortOrder;

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
    public string ParentFunctionID
    {
        get { return _ParentFunctionID; }
        set { _ParentFunctionID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FunctionName
    {
        get { return _FunctionName; }
        set { _FunctionName = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Level
    {
        get { return _Level; }
        set { _Level = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool InMenu
    {
        get { return _InMenu; }
        set { _InMenu = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FunctionUrl
    {
        get { return _FunctionUrl; }
        set { _FunctionUrl = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int SortOrder
    {
        get { return _SortOrder; }
        set { _SortOrder = value; }
    }

    #endregion


}

