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
/// CARD_HALLMARK
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class CARD_HALLMARK
{
    public CARD_HALLMARK()
    { }

    #region Model
    private int _RID;
    private string _SendCheck_Status;
    private int _CardType_RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private DateTime _Begin_Date = Convert.ToDateTime("1900-01-01");
    private DateTime _Finish_Date = Convert.ToDateTime("1900-01-01");
    private string _Validate_Number;
    private string _Serial_Number;

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
    public string SendCheck_Status
    {
        get { return _SendCheck_Status; }
        set { _SendCheck_Status = value; }
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
    public DateTime Begin_Date
    {
        get { return _Begin_Date; }
        set { _Begin_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Finish_Date
    {
        get { return _Finish_Date; }
        set { _Finish_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Validate_Number
    {
        get { return _Validate_Number; }
        set { _Validate_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Serial_Number
    {
        get { return _Serial_Number; }
        set { _Serial_Number = value; }
    }

    #endregion


}

