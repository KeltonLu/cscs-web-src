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
/// SAFE_DAY_MANAGE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class SAFE_DAY_MANAGE
{
    public SAFE_DAY_MANAGE()
    { }

    #region Model
    private int _RID;
    private string _RUU;
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private string _RCU;
    private string _RST;
    private int _Perso_Factory_RID;
    private int _CardType_RID;
    private long _Number;
    private DateTime _Date_Time = Convert.ToDateTime("1900-01-01");

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
    public string RUU
    {
        get { return _RUU; }
        set { _RUU = value; }
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
    public string RCU
    {
        get { return _RCU; }
        set { _RCU = value; }
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
    public int Perso_Factory_RID
    {
        get { return _Perso_Factory_RID; }
        set { _Perso_Factory_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int CardType_RID
    {
        get { return _CardType_RID; }
        set { _CardType_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public long Number
    {
        get { return _Number; }
        set { _Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Date_Time
    {
        get { return _Date_Time; }
        set { _Date_Time = value; }
    }

    #endregion


}

