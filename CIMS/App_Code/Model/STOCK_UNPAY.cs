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
/// STOCK_UNPAY
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class STOCK_UNPAY
{
    public STOCK_UNPAY()
    { }

    #region Model
    private int _RID;
    private string _Date_Time;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private decimal _P_Number;
    private decimal _U_Number;
    private decimal _D_Number;
    private decimal _T_Number;
    private int _Group_RID;
    private int _Blank_Factory_RID;

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
    public int Group_RID
    {
        get { return _Group_RID; }
        set { _Group_RID = value; }
    }

    public int Blank_Factory_RID
    {
        get { return _Blank_Factory_RID; }
        set { _Blank_Factory_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Date_Time
    {
        get { return _Date_Time; }
        set { _Date_Time = value; }
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
    public string RST
    {
        get { return _RST; }
        set { _RST = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal P_Number
    {
        get { return _P_Number; }
        set { _P_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal U_Number
    {
        get { return _U_Number; }
        set { _U_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal D_Number
    {
        get { return _D_Number; }
        set { _D_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal T_Number
    {
        get { return _T_Number; }
        set { _T_Number = value; }
    }

    #endregion


}

