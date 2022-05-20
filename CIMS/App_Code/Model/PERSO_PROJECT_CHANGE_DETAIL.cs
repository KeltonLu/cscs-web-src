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
/// PERSO_PROJECT_CHANGE_DETAIL
/// Date Created: 2008年12月23日
/// Created By:  BingYiPan
/// </summary>
public class PERSO_PROJECT_CHANGE_DETAIL
{
    public PERSO_PROJECT_CHANGE_DETAIL()
    { }

    #region Model
    private int _RID;
    private int _Perso_Factory;
    private int _CardGroup_RID;
    private DateTime _Project_Date = Convert.ToDateTime("1900-01-01");
    private string _Param_Code;
    private decimal _Price;
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
    public int Perso_Factory
    {
        get { return _Perso_Factory; }
        set { _Perso_Factory = value; }
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
    public DateTime Project_Date
    {
        get { return _Project_Date; }
        set { _Project_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Param_Code
    {
        get { return _Param_Code; }
        set { _Param_Code = value; }
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

