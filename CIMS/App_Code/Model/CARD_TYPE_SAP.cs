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
/// CARD_TYPE_SAP
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class CARD_TYPE_SAP
{
    public CARD_TYPE_SAP()
    { }

    #region Model
    private int _RID;
    private string _SAP_Serial_Number;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _Pass_Status;
    private int _Fine;
    private string _Comment;
    private string _Is_Finance;
    private DateTime _Pay_Date = Convert.ToDateTime("1900-01-01");
    private decimal _Real_Pay_Money;
    private decimal _Real_Pay_Money_No;
    private DateTime _Ask_Date = Convert.ToDateTime("1900-01-01");

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
    public string SAP_Serial_Number
    {
        get { return _SAP_Serial_Number; }
        set { _SAP_Serial_Number = value; }
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
    public string Pass_Status
    {
        get { return _Pass_Status; }
        set { _Pass_Status = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Fine
    {
        get { return _Fine; }
        set { _Fine = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Comment
    {
        get { return _Comment; }
        set { _Comment = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_Finance
    {
        get { return _Is_Finance; }
        set { _Is_Finance = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Pay_Date
    {
        get { return _Pay_Date; }
        set { _Pay_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Real_Pay_Money
    {
        get { return _Real_Pay_Money; }
        set { _Real_Pay_Money = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Real_Pay_Money_No
    {
        get { return _Real_Pay_Money_No; }
        set { _Real_Pay_Money_No = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Ask_Date
    {
        get { return _Ask_Date; }
        set { _Ask_Date = value; }
    }

    #endregion


}

