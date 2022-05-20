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
/// PERSO_CARDTYPE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class PERSO_CARDTYPE
{
    public PERSO_CARDTYPE()
    { }

    #region Model
    private int _RID;
    private int _CardType_RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private int _Factory_RID;
    private string _Base_Special;
    private int _Value;
    private string _Percentage_Number;
    private int _Priority;

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
    public int CardType_RID
    {
        get { return _CardType_RID; }
        set { _CardType_RID = value; }
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
    public string Base_Special
    {
        get { return _Base_Special; }
        set { _Base_Special = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Value
    {
        get { return _Value; }
        set { _Value = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Percentage_Number
    {
        get { return _Percentage_Number; }
        set { _Percentage_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Priority
    {
        get { return _Priority; }
        set { _Priority = value; }
    }

    #endregion


}

