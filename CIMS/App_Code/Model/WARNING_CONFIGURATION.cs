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
/// WARNING_CONFIGURATION
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class WARNING_CONFIGURATION
{
    public WARNING_CONFIGURATION()
    { }

    #region Model
    private int _RID;
    private string _Item_Name;
    private string _Condition;
    private string _Warning_Content;
    private string _System_Show;
    private string _Mail_Show;
    private string _RST;
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private string _RUU;
    private string _RCU;

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
    public string Item_Name
    {
        get { return _Item_Name; }
        set { _Item_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Condition
    {
        get { return _Condition; }
        set { _Condition = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Warning_Content
    {
        get { return _Warning_Content; }
        set { _Warning_Content = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string System_Show
    {
        get { return _System_Show; }
        set { _System_Show = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Mail_Show
    {
        get { return _Mail_Show; }
        set { _Mail_Show = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string RST
    {
        get { return _RST; }
        set { _RST = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime RUT
    {
        get { return _RUT; }
        set { _RUT = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime RCT
    {
        get { return _RCT; }
        set { _RCT = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string RUU
    {
        get { return _RUU; }
        set { _RUU = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string RCU
    {
        get { return _RCU; }
        set { _RCU = value; }
    }

    #endregion


}

