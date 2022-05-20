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
/// AGREEMENT_CARDTYPE_GROUP
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class AGREEMENT_CARDTYPE_GROUP
{
    public AGREEMENT_CARDTYPE_GROUP()
    { }

    #region Model
    private int _RID;
    private int _Agreement_Main_RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _Group_Name;
    private string _Type;
    private decimal _Base_Price;

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
    public int Agreement_Main_RID
    {
        get { return _Agreement_Main_RID; }
        set { _Agreement_Main_RID = value; }
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
    public string Group_Name
    {
        get { return _Group_Name; }
        set { _Group_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Type
    {
        get { return _Type; }
        set { _Type = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Base_Price
    {
        get { return _Base_Price; }
        set { _Base_Price = value; }
    }

    #endregion


}

