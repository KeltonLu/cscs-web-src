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
/// ORDER_BUDGET_LOG
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class ORDER_BUDGET_LOG
{
    public ORDER_BUDGET_LOG()
    { }

    #region Model
    private int _RID;
    private string _RCU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private string _RUU;
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private decimal _Remain_Card_Price;
    private int _Budget_RID;
    private string _OrderForm_Detail_RID;
    private int _Remain_Card_Num;

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

    /// <summary>
    /// 
    /// </summary>
    public decimal Remain_Card_Price
    {
        get { return _Remain_Card_Price; }
        set { _Remain_Card_Price = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Budget_RID
    {
        get { return _Budget_RID; }
        set { _Budget_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string OrderForm_Detail_RID
    {
        get { return _OrderForm_Detail_RID; }
        set { _OrderForm_Detail_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Remain_Card_Num
    {
        get { return _Remain_Card_Num; }
        set { _Remain_Card_Num = value; }
    }

    #endregion


}

