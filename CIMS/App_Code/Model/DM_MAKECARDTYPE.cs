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
/// DM_MAKECARDTYPE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class DM_MAKECARDTYPE
{
    public DM_MAKECARDTYPE()
    { }

    #region Model
    private int _RID;
    private string _RST;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private int _MakeCardType_RID;
    private int _DM_RID;

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
    public string RST
    {
        get { return _RST; }
        set { _RST = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string RCU
    {
        get { return _RCU; }
        set { _RCU = value; }
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
    public DateTime RCT
    {
        get { return _RCT; }
        set { _RCT = value; }
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
    public int MakeCardType_RID
    {
        get { return _MakeCardType_RID; }
        set { _MakeCardType_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int DM_RID
    {
        get { return _DM_RID; }
        set { _DM_RID = value; }
    }

    #endregion


}

