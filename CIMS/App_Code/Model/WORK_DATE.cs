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
/// WORK_DATE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class WORK_DATE
{
    public WORK_DATE()
    { }

    #region Model
    private int _RID;
    private string _RST;
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private string _RUU;
    private string _RCU;
    private DateTime _Date_Time = Convert.ToDateTime("1900-01-01");
    private string _Is_WorkDay;

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
    public string RST
    {
        get { return _RST; }
        set { _RST = value; }
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
    public string RCU
    {
        get { return _RCU; }
        set { _RCU = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Date_Time
    {
        get { return _Date_Time; }
        set { _Date_Time = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_WorkDay
    {
        get { return _Is_WorkDay; }
        set { _Is_WorkDay = value; }
    }

    #endregion


}

