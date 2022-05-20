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
/// RPT_Finance004_2
/// Date Created: 2008年12月9日
/// Created By:  FangBao
/// </summary>
public class RPT_Finance004_2
{
    public RPT_Finance004_2()
    { }

    #region Model
    private string _Name;
    private string _Factory;
    private int _BeginNumber;
    private string _BeginUnitPrice;
    private int _InNumber;
    private int _MoveIn;
    private int _MoveOut;
    private int _UseOutNumber;
    private int _S_Number;
    private int _F_Number;
    private int _DestoryNumber;
    private int _ChangeNumber;
    private int _EndNumber;
    private string _UnitPrice;
    private decimal _BeginPrice;
    private decimal _InPrice;
    private decimal _MoveInPrice;
    private decimal _MoveOutPrice;
    private decimal _S_Price;
    private decimal _F_Price;
    private decimal _UseOutPrice;
    private decimal _DestoryPrice;
    private decimal _ChangePrice;
    private decimal _ChangeUintPrice;
    private decimal _EndPrice;
    private string _TimeMark;
    private int _Id;


    public string TimeMark {
        get { return _TimeMark; }
        set { _TimeMark = value; }
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
    public string Factory
    {
        get { return _Factory; }
        set { _Factory = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int BeginNumber
    {
        get { return _BeginNumber; }
        set { _BeginNumber = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string BeginUnitPrice
    {
        get { return _BeginUnitPrice; }
        set { _BeginUnitPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int InNumber
    {
        get { return _InNumber; }
        set { _InNumber = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int MoveIn
    {
        get { return _MoveIn; }
        set { _MoveIn = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int MoveOut
    {
        get { return _MoveOut; }
        set { _MoveOut = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int UseOutNumber
    {
        get { return _UseOutNumber; }
        set { _UseOutNumber = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int S_Number
    {
        get { return _S_Number; }
        set { _S_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int F_Number
    {
        get { return _F_Number; }
        set { _F_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int DestoryNumber
    {
        get { return _DestoryNumber; }
        set { _DestoryNumber = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int ChangeNumber
    {
        get { return _ChangeNumber; }
        set { _ChangeNumber = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int EndNumber
    {
        get { return _EndNumber; }
        set { _EndNumber = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string UnitPrice
    {
        get { return _UnitPrice; }
        set { _UnitPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal BeginPrice
    {
        get { return _BeginPrice; }
        set { _BeginPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal InPrice
    {
        get { return _InPrice; }
        set { _InPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal MoveInPrice
    {
        get { return _MoveInPrice; }
        set { _MoveInPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal MoveOutPrice
    {
        get { return _MoveOutPrice; }
        set { _MoveOutPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal S_Price
    {
        get { return _S_Price; }
        set { _S_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal F_Price
    {
        get { return _F_Price; }
        set { _F_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal UseOutPrice
    {
        get { return _UseOutPrice; }
        set { _UseOutPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal DestoryPrice
    {
        get { return _DestoryPrice; }
        set { _DestoryPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal ChangePrice
    {
        get { return _ChangePrice; }
        set { _ChangePrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal ChangeUintPrice
    {
        get { return _ChangeUintPrice; }
        set { _ChangeUintPrice = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal EndPrice
    {
        get { return _EndPrice; }
        set { _EndPrice = value; }
    }

    public int Id
    {
        get { return _Id; }
        set { _Id = value; }
    }
    #endregion


}

