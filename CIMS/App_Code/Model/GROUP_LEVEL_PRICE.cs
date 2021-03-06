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
/// GROUP_LEVEL_PRICE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class GROUP_LEVEL_PRICE
{
    public GROUP_LEVEL_PRICE()
    { }

    #region Model
    private int _RID;
    private int _Group_CardType_RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private decimal _Price;
    private int _Level_Min;
    private int _Level_Max;

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
    public int Group_CardType_RID
    {
        get { return _Group_CardType_RID; }
        set { _Group_CardType_RID = value; }
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
    public decimal Price
    {
        get { return _Price; }
        set { _Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Level_Min
    {
        get { return _Level_Min; }
        set { _Level_Min = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Level_Max
    {
        get { return _Level_Max; }
        set { _Level_Max = value; }
    }

    #endregion


}

