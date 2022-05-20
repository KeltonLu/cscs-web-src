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
/// PERSO_PROJECT_PRICE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class PERSO_PROJECT_PRICE
{
    public PERSO_PROJECT_PRICE()
    { }

    #region Model
    private int _RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private int _Perso_Project_RID;
    private DateTime _Use_Date_End = Convert.ToDateTime("1900-01-01");
    private DateTime _Use_Date_Begin = Convert.ToDateTime("1900-01-01");
    private decimal _Price;

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
    public int Perso_Project_RID
    {
        get { return _Perso_Project_RID; }
        set { _Perso_Project_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Use_Date_End
    {
        get { return _Use_Date_End; }
        set { _Use_Date_End = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Use_Date_Begin
    {
        get { return _Use_Date_Begin; }
        set { _Use_Date_Begin = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Price
    {
        get { return _Price; }
        set { _Price = value; }
    }

    #endregion


}

