﻿using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


/// <summary>
/// ENVELOPE_INFO
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class ENVELOPE_INFO
{
    public ENVELOPE_INFO()
    { }

    #region Model
    private int _RID;
    private string _RST;
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private string _RUU;
    private string _RCU;
    private string _Serial_Number;
    private string _Name;
    private decimal _Unit_Price;
    private int _Wear_Rate;
    private int _Safe_Number;
    private string _Safe_Type;
    private string _Billing_Type;

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
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Unit_Price
    {
        get { return _Unit_Price; }
        set { _Unit_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Wear_Rate
    {
        get { return _Wear_Rate; }
        set { _Wear_Rate = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Safe_Number
    {
        get { return _Safe_Number; }
        set { _Safe_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Safe_Type
    {
        get { return _Safe_Type; }
        set { _Safe_Type = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Billing_Type
    {
        get { return _Billing_Type; }
        set { _Billing_Type = value; }
    }

    #endregion


}

