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
/// BUDGET_CARDTYPE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class BUDGET_CARDTYPE
{
    public BUDGET_CARDTYPE()
    { }

    #region Model
    private int _RID;
    private int _BUDGET_RID;
    private int _CARDTYPE_RID;
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
    public int BUDGET_RID
    {
        get { return _BUDGET_RID; }
        set { _BUDGET_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int CARDTYPE_RID
    {
        get { return _CARDTYPE_RID; }
        set { _CARDTYPE_RID = value; }
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

