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
/// PERSO_PROJECT_SAP
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class PERSO_PROJECT_SAP
{
    public PERSO_PROJECT_SAP()
    { }

    #region Model
    private int _RID;
    private DateTime _Ask_Date = Convert.ToDateTime("1900-01-01");
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _SAP_ID;
    private decimal _Sum;
    private DateTime _Pay_Date = Convert.ToDateTime("1900-01-01");
    private string _Check_Serial_Number;
    private int _Perso_Factory_RID;
    private int _Card_Group_RID;
    private DateTime _Begin_Date = Convert.ToDateTime("1900-01-01");
    private DateTime _End_Date = Convert.ToDateTime("1900-01-01");

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
    public DateTime Ask_Date
    {
        get { return _Ask_Date; }
        set { _Ask_Date = value; }
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
    public string SAP_ID
    {
        get { return _SAP_ID; }
        set { _SAP_ID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public decimal Sum
    {
        get { return _Sum; }
        set { _Sum = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Pay_Date
    {
        get { return _Pay_Date; }
        set { _Pay_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Check_Serial_Number
    {
        get { return _Check_Serial_Number; }
        set { _Check_Serial_Number = value; }
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
    public int Card_Group_RID
    {
        get { return _Card_Group_RID; }
        set { _Card_Group_RID = value; }
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
    public DateTime End_Date
    {
        get { return _End_Date; }
        set { _End_Date = value; }
    }

    #endregion


}

