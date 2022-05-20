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
/// MATERIEL_STOCK_HISTORY
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class MATERIEL_STOCK_HISTORY
{
    public MATERIEL_STOCK_HISTORY()
    { }

    #region Model
    private int _RID;
    private DateTime _Stock_Date = Convert.ToDateTime("1900-01-01");
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private int _Card_Exponent_Number;
    private string _Card_Exponent_SAP;
    private int _Envelope_Number;
    private string _Envelope_SAP;
    private int _DM_Number;
    private string _DM_SAP;
    private int _Postage_Price;
    private int _Instead_Price;

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
    public DateTime Stock_Date
    {
        get { return _Stock_Date; }
        set { _Stock_Date = value; }
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
    public int Card_Exponent_Number
    {
        get { return _Card_Exponent_Number; }
        set { _Card_Exponent_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Card_Exponent_SAP
    {
        get { return _Card_Exponent_SAP; }
        set { _Card_Exponent_SAP = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Envelope_Number
    {
        get { return _Envelope_Number; }
        set { _Envelope_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Envelope_SAP
    {
        get { return _Envelope_SAP; }
        set { _Envelope_SAP = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int DM_Number
    {
        get { return _DM_Number; }
        set { _DM_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string DM_SAP
    {
        get { return _DM_SAP; }
        set { _DM_SAP = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Postage_Price
    {
        get { return _Postage_Price; }
        set { _Postage_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Instead_Price
    {
        get { return _Instead_Price; }
        set { _Instead_Price = value; }
    }

    #endregion


}

