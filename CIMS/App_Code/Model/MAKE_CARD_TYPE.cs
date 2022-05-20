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
/// MAKE_CARD_TYPE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class MAKE_CARD_TYPE
{
    public MAKE_CARD_TYPE()
    { }

    #region Model
    private int _RID;
    private int _CardGroup_RID;
    private string _Type_Name;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _Is_Report;
    private string _Is_Import;

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
    public int CardGroup_RID
    {
        get { return _CardGroup_RID; }
        set { _CardGroup_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Type_Name
    {
        get { return _Type_Name; }
        set { _Type_Name = value; }
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
    public string Is_Report
    {
        get { return _Is_Report; }
        set { _Is_Report = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_Import
    {
        get { return _Is_Import; }
        set { _Is_Import = value; }
    }

    #endregion


}

