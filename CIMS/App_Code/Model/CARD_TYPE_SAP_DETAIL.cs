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
/// CARD_TYPE_SAP_DETAIL
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class CARD_TYPE_SAP_DETAIL
{
    public CARD_TYPE_SAP_DETAIL()
    { }

    #region Model
    private int _RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private int _Operate_RID;
    private string _Operate_Type;
    private string _Check_Serial_Number;
    private string _Comment;
    private int _Delay_Days;
    private int _Real_Ask_Number;
    private decimal _Unit_Price_No;
    private decimal _Unit_Price;
    private int _Split_RID;
    private int _SAP_RID;

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
    public int Operate_RID
    {
        get { return _Operate_RID; }
        set { _Operate_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Operate_Type
    {
        get { return _Operate_Type; }
        set { _Operate_Type = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Check_Serial_Number
    {
        get { return _Check_Serial_Number; }
        set { _Check_Serial_Number = value; }
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
    public int Delay_Days
    {
        get { return _Delay_Days; }
        set { _Delay_Days = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Real_Ask_Number
    {
        get { return _Real_Ask_Number; }
        set { _Real_Ask_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Unit_Price_No
    {
        get { return _Unit_Price_No; }
        set { _Unit_Price_No = value; }
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
    public int Split_RID
    {
        get { return _Split_RID; }
        set { _Split_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int SAP_RID
    {
        get { return _SAP_RID; }
        set { _SAP_RID = value; }
    }

    #endregion


}

