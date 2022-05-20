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
/// DEPOSITORY_RESTOCK
/// Date Created: 2008年11月24日
/// Created By:  FangBao
/// </summary>
public class DEPOSITORY_RESTOCK
{
    public DEPOSITORY_RESTOCK()
    { }

    #region Model
    private int _RID;
    private string _Stock_RID;
    private string _OrderForm_Detail_RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private int _Space_Short_RID;
    private int _Restock_Number;
    private int _Blemish_Number;
    private int _Sample_Number;
    private int _Reincome_Number;
    private DateTime _Reincome_Date = Convert.ToDateTime("1900-01-01");
    private string _Serial_Number;
    private string _Is_Finance;
    private int _Agreement_RID;
    private int _Budget_RID;
    private string _Is_AskFinance;
    private string _Report_RID;
    private string _Is_Check;
    private int _Perso_Factory_RID;
    private int _Blank_Factory_RID;
    private DateTime _Check_Date = Convert.ToDateTime("1900-01-01");
    private int _Wafer_RID;
    private string _Comment;
    private string _Check_Type;
    private string _Restock_RID;

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
    public string Stock_RID
    {
        get { return _Stock_RID; }
        set { _Stock_RID = value; }
    }

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
    public int Space_Short_RID
    {
        get { return _Space_Short_RID; }
        set { _Space_Short_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Restock_Number
    {
        get { return _Restock_Number; }
        set { _Restock_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Blemish_Number
    {
        get { return _Blemish_Number; }
        set { _Blemish_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Sample_Number
    {
        get { return _Sample_Number; }
        set { _Sample_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Reincome_Number
    {
        get { return _Reincome_Number; }
        set { _Reincome_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Reincome_Date
    {
        get { return _Reincome_Date; }
        set { _Reincome_Date = value; }
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
    public string Is_Finance
    {
        get { return _Is_Finance; }
        set { _Is_Finance = value; }
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
    public int Budget_RID
    {
        get { return _Budget_RID; }
        set { _Budget_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_AskFinance
    {
        get { return _Is_AskFinance; }
        set { _Is_AskFinance = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Report_RID
    {
        get { return _Report_RID; }
        set { _Report_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_Check
    {
        get { return _Is_Check; }
        set { _Is_Check = value; }
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
    public int Blank_Factory_RID
    {
        get { return _Blank_Factory_RID; }
        set { _Blank_Factory_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Check_Date
    {
        get { return _Check_Date; }
        set { _Check_Date = value; }
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
    public string Comment
    {
        get { return _Comment; }
        set { _Comment = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Check_Type
    {
        get { return _Check_Type; }
        set { _Check_Type = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Restock_RID
    {
        get { return _Restock_RID; }
        set { _Restock_RID = value; }
    }

    #endregion


}

