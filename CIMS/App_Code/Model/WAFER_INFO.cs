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
/// WAFER_INFO
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class WAFER_INFO
{
    public WAFER_INFO()
    { }

    #region Model
    private int _RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _Wafer_Name;
    private int _Wafer_Capacity;
    private string _Mark;
    private string _Wafer_Factory;
    private string _Interface;
    private string _Protocle;
    private string _Wafer_Type;
    private string _OS;
    private string _Java_Version;
    private string _Crypto_Cap;
    private int _ROM_Capacity;
    private DateTime _First_Time = Convert.ToDateTime("1900-01-01");
    private decimal _Unit_Price;
    private string _Pre_Program;
    private string _Is_Using;
    private string _Comment_One;
    private string _Comment_Second;
    private string _Comment_Third;

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
    public string Wafer_Name
    {
        get { return _Wafer_Name; }
        set { _Wafer_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Wafer_Capacity
    {
        get { return _Wafer_Capacity; }
        set { _Wafer_Capacity = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Mark
    {
        get { return _Mark; }
        set { _Mark = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Wafer_Factory
    {
        get { return _Wafer_Factory; }
        set { _Wafer_Factory = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Interface
    {
        get { return _Interface; }
        set { _Interface = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Protocle
    {
        get { return _Protocle; }
        set { _Protocle = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Wafer_Type
    {
        get { return _Wafer_Type; }
        set { _Wafer_Type = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string OS
    {
        get { return _OS; }
        set { _OS = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Java_Version
    {
        get { return _Java_Version; }
        set { _Java_Version = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Crypto_Cap
    {
        get { return _Crypto_Cap; }
        set { _Crypto_Cap = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int ROM_Capacity
    {
        get { return _ROM_Capacity; }
        set { _ROM_Capacity = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime First_Time
    {
        get { return _First_Time; }
        set { _First_Time = value; }
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
    public string Pre_Program
    {
        get { return _Pre_Program; }
        set { _Pre_Program = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_Using
    {
        get { return _Is_Using; }
        set { _Is_Using = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Comment_One
    {
        get { return _Comment_One; }
        set { _Comment_One = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Comment_Second
    {
        get { return _Comment_Second; }
        set { _Comment_Second = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Comment_Third
    {
        get { return _Comment_Third; }
        set { _Comment_Third = value; }
    }

    #endregion


}

