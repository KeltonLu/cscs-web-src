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
/// PERSO_PROJECT
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class PERSO_PROJECT
{
    public PERSO_PROJECT()
    { }

    #region Model
    private int _RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private int _Factory_RID;
    private decimal _Unit_Price;
    private string _Project_Name;
    private string _Normal_Special;
    private string _Project_Code;
    private string _Comment;

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
    public int Factory_RID
    {
        get { return _Factory_RID; }
        set { _Factory_RID = value; }
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
    public string Project_Name
    {
        get { return _Project_Name; }
        set { _Project_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Normal_Special
    {
        get { return _Normal_Special; }
        set { _Normal_Special = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Project_Code
    {
        get { return _Project_Code; }
        set { _Project_Code = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Comment
    {
        get { return _Comment; }
        set { _Comment = value; }
    }

    #endregion


}

