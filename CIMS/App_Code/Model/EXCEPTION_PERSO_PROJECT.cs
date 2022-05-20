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
/// EXCEPTION_PERSO_PROJECT
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class EXCEPTION_PERSO_PROJECT
{
    public EXCEPTION_PERSO_PROJECT()
    { }

    #region Model
    private int _RID;
    private DateTime _Project_Date = Convert.ToDateTime("1900-01-01");
    private int _CardGroup_RID;
    private int _Perso_Factory_RID;
    private string _Name;
    private long _Number;
    private decimal _Unit_Price;
    private string _Comment;
    private string _RCU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private string _RUU;
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;

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
    public DateTime Project_Date
    {
        get { return _Project_Date; }
        set { _Project_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int CardGroup_RID
    {
        get { return _CardGroup_RID; }
        set { _CardGroup_RID = value; }
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
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public long Number
    {
        get { return _Number; }
        set { _Number = value; }
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

    #endregion


}

