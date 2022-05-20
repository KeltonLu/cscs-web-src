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
/// CARD_BUDGET
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class CARD_BUDGET
{
    public CARD_BUDGET()
    { }

    #region Model
    private int _RID;
    private string _Budget_ID;
    private string _Budget_Name;
    private string _IMG_File_URL;
    private DateTime _Valid_Date_From = Convert.ToDateTime("1900-01-01");
    private DateTime _Valid_Date_To = Convert.ToDateTime("1900-01-01");
    private decimal _Card_Price;
    private long _Card_Num;
    private decimal _Total_Card_AMT;
    private long _Total_Card_Num;
    private decimal _Remain_Total_AMT;
    private long _Remain_Total_Num;
    private string _RCU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private string _RUU;
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private decimal _Remain_Card_Price;
    private long _Remain_Card_Num;
    private int _Budget_Main_RID;
    private string _Reason;
    private string _IMG_File_Name;

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
    public string Budget_ID
    {
        get { return _Budget_ID; }
        set { _Budget_ID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Budget_Name
    {
        get { return _Budget_Name; }
        set { _Budget_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string IMG_File_URL
    {
        get { return _IMG_File_URL; }
        set { _IMG_File_URL = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Valid_Date_From
    {
        get { return _Valid_Date_From; }
        set { _Valid_Date_From = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Valid_Date_To
    {
        get { return _Valid_Date_To; }
        set { _Valid_Date_To = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Card_Price
    {
        get { return _Card_Price; }
        set { _Card_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public long Card_Num
    {
        get { return _Card_Num; }
        set { _Card_Num = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Total_Card_AMT
    {
        get { return _Total_Card_AMT; }
        set { _Total_Card_AMT = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public long Total_Card_Num
    {
        get { return _Total_Card_Num; }
        set { _Total_Card_Num = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Remain_Total_AMT
    {
        get { return _Remain_Total_AMT; }
        set { _Remain_Total_AMT = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public long Remain_Total_Num
    {
        get { return _Remain_Total_Num; }
        set { _Remain_Total_Num = value; }
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
    public decimal Remain_Card_Price
    {
        get { return _Remain_Card_Price; }
        set { _Remain_Card_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public long Remain_Card_Num
    {
        get { return _Remain_Card_Num; }
        set { _Remain_Card_Num = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Budget_Main_RID
    {
        get { return _Budget_Main_RID; }
        set { _Budget_Main_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Reason
    {
        get { return _Reason; }
        set { _Reason = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string IMG_File_Name
    {
        get { return _IMG_File_Name; }
        set { _IMG_File_Name = value; }
    }

    #endregion


}

