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
/// ORDER_FORM_DETAIL
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class ORDER_FORM_DETAIL
{
    public ORDER_FORM_DETAIL()
    { }

    #region Model
    private string _OrderForm_Detail_RID;
    private string _OrderForm_RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private int _CardType_RID;
    private int _Number;
    private int _Budget_RID;
    private int _Agreement_RID;
    private DateTime _Fore_Delivery_Date = Convert.ToDateTime("1900-01-01");
    private int _Wafer_RID;
    private string _Is_Exigence;
    private int _Delivery_Address_RID;
    private string _Case_Status;
    private string _Comment;
    private string _Is_Edit;
    private decimal _Unit_Price;
    private string _Is_Edit_Budget;
    private string _Is_Edit_Agreement;
    private int _RID;
    /// add chaoma start
    private decimal _Change_UnitPrice;
    /// add chaoma end

    /// <summary>
    /// 
    /// </summary>
    public string OrderForm_Detail_RID
    {
        get { return _OrderForm_Detail_RID; }
        set { _OrderForm_Detail_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string OrderForm_RID
    {
        get { return _OrderForm_RID; }
        set { _OrderForm_RID = value; }
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
    public int CardType_RID
    {
        get { return _CardType_RID; }
        set { _CardType_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Number
    {
        get { return _Number; }
        set { _Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Budget_RID
    {
        get { return _Budget_RID; }
        set { _Budget_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Agreement_RID
    {
        get { return _Agreement_RID; }
        set { _Agreement_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Fore_Delivery_Date
    {
        get { return _Fore_Delivery_Date; }
        set { _Fore_Delivery_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Wafer_RID
    {
        get { return _Wafer_RID; }
        set { _Wafer_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_Exigence
    {
        get { return _Is_Exigence; }
        set { _Is_Exigence = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Delivery_Address_RID
    {
        get { return _Delivery_Address_RID; }
        set { _Delivery_Address_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Case_Status
    {
        get { return _Case_Status; }
        set { _Case_Status = value; }
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
    public string Is_Edit
    {
        get { return _Is_Edit; }
        set { _Is_Edit = value; }
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
    public string Is_Edit_Budget
    {
        get { return _Is_Edit_Budget; }
        set { _Is_Edit_Budget = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_Edit_Agreement
    {
        get { return _Is_Edit_Agreement; }
        set { _Is_Edit_Agreement = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int RID
    {
        get { return _RID; }
        set { _RID = value; }
    }

    /// <summary>
    /// add chaoma start
    /// </summary>
    public decimal Change_UnitPrice
    {
        get { return _Change_UnitPrice; }
        set { _Change_UnitPrice = value; }
    }
    ///add chaoma end
    #endregion


}

