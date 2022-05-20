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
/// STOCKS_COST
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class STOCKS_COST
{
    public STOCKS_COST()
    { }

    #region Model
    private int _RID;
    private string _Date_Time;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private decimal _S_Number;
    private decimal _F_Number;
    private decimal _A_Number;
    private decimal _W_Number;
    private DateTime _Date_From = Convert.ToDateTime("1900-01-01");
    private DateTime _Date_To = Convert.ToDateTime("1900-01-01");
    private int _Group_RID;

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
    public decimal S_Number
    {
        get { return _S_Number; }
        set { _S_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal F_Number
    {
        get { return _F_Number; }
        set { _F_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal A_Number
    {
        get { return _A_Number; }
        set { _A_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal W_Number
    {
        get { return _W_Number; }
        set { _W_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Date_From
    {
        get { return _Date_From; }
        set { _Date_From = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Date_To
    {
        get { return _Date_To; }
        set { _Date_To = value; }
    }

    #endregion


}

