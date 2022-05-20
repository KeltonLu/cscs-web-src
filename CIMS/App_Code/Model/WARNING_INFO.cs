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
/// WARNING_INFO
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class WARNING_INFO
{
    public WARNING_INFO()
    { }

    #region Model
    private int _RID;
    private int _Warning_RID;
    private string _UserID;
    private string _Is_Show;
    private string _Warning_Content;

    /// <summary>
    /// 
    /// </summary>
    public int RID
    {
        get { return _RID; }
        set { _RID = value; }
    }

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

    /// <summary>
    /// 
    /// </summary>
    public string Is_Show
    {
        get { return _Is_Show; }
        set { _Is_Show = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Warning_Content
    {
        get { return _Warning_Content; }
        set { _Warning_Content = value; }
    }

    #endregion


}

