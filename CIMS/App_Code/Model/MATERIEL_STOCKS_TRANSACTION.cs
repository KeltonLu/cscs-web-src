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
/// MATERIEL_STOCKS_TRANSACTION 的摘要描述
/// </summary>
public class MATERIEL_STOCKS_TRANSACTION
{
    public MATERIEL_STOCKS_TRANSACTION()
    { }

    #region model

    private int _RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private DateTime _Transaction_Date = Convert.ToDateTime("1900-01-01");
    private string _Transaction_ID;
    private string _Serial_Number;
    private int _Transaction_Amount;
    private int _PARAM_RID;
    private int _Factory_RID;

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
    public DateTime Transaction_Date
    {
        get { return _Transaction_Date; }
        set { _Transaction_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Transaction_ID
    {
        get { return _Transaction_ID; }
        set { _Transaction_ID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Serial_Number
    {
        get { return _Serial_Number; }
        set { _Serial_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Transaction_Amount
    {
        get { return _Transaction_Amount; }
        set { _Transaction_Amount = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int PARAM_RID
    {
        get { return _PARAM_RID; }
        set { _PARAM_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Factory_RID
    {
        get { return _Factory_RID; }
        set { _Factory_RID = value; }
    }
    #endregion model
}
