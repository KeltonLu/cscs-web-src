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
/// MATERIEL_PURCHASE_FORM
/// Date Created: 2008年12月19日
/// Created By:  FangBao
/// </summary>
public class MATERIEL_PURCHASE_FORM
{
    public MATERIEL_PURCHASE_FORM()
    { }

    #region Model
    private string _PurchaseOrder_RID;
    private int _Detail_RID;
    private DateTime _Purchase_Date = Convert.ToDateTime("1900-01-01");
    private string _Serial_Number;
    private int _Factory_RID1;
    private int _Number1;
    private DateTime _Delivery_Date1 = Convert.ToDateTime("1900-01-01");
    private int _Factory_RID2;
    private int _Number2;
    private DateTime _Delivery_Date2 = Convert.ToDateTime("1900-01-01");
    private int _Factory_RID3;
    private int _Number3;
    private DateTime _Delivery_Date3 = Convert.ToDateTime("1900-01-01");
    private int _Factory_RID4;
    private int _Number4;
    private DateTime _Delivery_Date4 = Convert.ToDateTime("1900-01-01");
    private int _Factory_RID5;
    private int _Number5;
    private DateTime _Delivery_Date5 = Convert.ToDateTime("1900-01-01");
    private DateTime _Case_Date = Convert.ToDateTime("1900-01-01");
    private string _SAP_Serial_Number;
    private DateTime _Ask_Date = Convert.ToDateTime("1900-01-01");
    private DateTime _Pay_Date = Convert.ToDateTime("1900-01-01");
    private string _Comment;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private decimal _Unit_Price;
    private long _Total_Num;
    private decimal _Total_Price;

    /// <summary>
    /// 
    /// </summary>
    public string PurchaseOrder_RID
    {
        get { return _PurchaseOrder_RID; }
        set { _PurchaseOrder_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Detail_RID
    {
        get { return _Detail_RID; }
        set { _Detail_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Purchase_Date
    {
        get { return _Purchase_Date; }
        set { _Purchase_Date = value; }
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
    public int Factory_RID1
    {
        get { return _Factory_RID1; }
        set { _Factory_RID1 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Number1
    {
        get { return _Number1; }
        set { _Number1 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Delivery_Date1
    {
        get { return _Delivery_Date1; }
        set { _Delivery_Date1 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Factory_RID2
    {
        get { return _Factory_RID2; }
        set { _Factory_RID2 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Number2
    {
        get { return _Number2; }
        set { _Number2 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Delivery_Date2
    {
        get { return _Delivery_Date2; }
        set { _Delivery_Date2 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Factory_RID3
    {
        get { return _Factory_RID3; }
        set { _Factory_RID3 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Number3
    {
        get { return _Number3; }
        set { _Number3 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Delivery_Date3
    {
        get { return _Delivery_Date3; }
        set { _Delivery_Date3 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Factory_RID4
    {
        get { return _Factory_RID4; }
        set { _Factory_RID4 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Number4
    {
        get { return _Number4; }
        set { _Number4 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Delivery_Date4
    {
        get { return _Delivery_Date4; }
        set { _Delivery_Date4 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Factory_RID5
    {
        get { return _Factory_RID5; }
        set { _Factory_RID5 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Number5
    {
        get { return _Number5; }
        set { _Number5 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Delivery_Date5
    {
        get { return _Delivery_Date5; }
        set { _Delivery_Date5 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Case_Date
    {
        get { return _Case_Date; }
        set { _Case_Date = value; }
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
    public DateTime Ask_Date
    {
        get { return _Ask_Date; }
        set { _Ask_Date = value; }
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
    public string Comment
    {
        get { return _Comment; }
        set { _Comment = value; }
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
    public decimal Unit_Price
    {
        get { return _Unit_Price; }
        set { _Unit_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public long Total_Num
    {
        get { return _Total_Num; }
        set { _Total_Num = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Total_Price
    {
        get { return _Total_Price; }
        set { _Total_Price = value; }
    }

    #endregion


}

