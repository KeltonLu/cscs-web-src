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
/// ORDER_FORM
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class ORDER_FORM
{
    public ORDER_FORM()
    { }

    #region Model
    private string _OrderForm_RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _Pass_Status;
    private DateTime _Pass_Date = Convert.ToDateTime("1900-01-01");
    private DateTime _Order_Date = Convert.ToDateTime("1900-01-01");
    private string _Case_Status;
    private int _Blank_Factory_RID;
    private int _RID;

    /// <summary>
    /// 
    /// </summary>
    public string OrderForm_RID
    {
        get { return _OrderForm_RID; }
        set { _OrderForm_RID = value; }
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
    public string Pass_Status
    {
        get { return _Pass_Status; }
        set { _Pass_Status = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Pass_Date
    {
        get { return _Pass_Date; }
        set { _Pass_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Order_Date
    {
        get { return _Order_Date; }
        set { _Order_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Case_Status
    {
        get { return _Case_Status; }
        set { _Case_Status = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Blank_Factory_RID
    {
        get { return _Blank_Factory_RID; }
        set { _Blank_Factory_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int RID
    {
        get { return _RID; }
        set { _RID = value; }
    }

    #endregion


}

