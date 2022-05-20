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
/// RPT_Finance004_4 的摘要描述
/// </summary>
public class RPT_Finance004_4
{
    public RPT_Finance004_4()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    #region Model
    private string _科目;
    private string _摘要;
    private string _原始成本;
    private string _每期需攤金額;
    private string _帳面價值;
    private string _Date;
    private string _Number;
    private string _TimeMark;

    public string TimeMark {
        get { return _TimeMark; }
        set { _TimeMark = value; }
    
    }

    /// <summary>
    /// 
    /// </summary>
    public string 科目
    {
        get { return _科目; }
        set { _科目 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string 摘要
    {
        get { return _摘要; }
        set { _摘要 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string 原始成本
    {
        get { return _原始成本; }
        set { _原始成本 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string 每期需攤金額
    {
        get { return _每期需攤金額; }
        set { _每期需攤金額 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string 帳面價值
    {
        get { return _帳面價值; }
        set { _帳面價值 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Date
    {
        get { return _Date; }
        set { _Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Number
    {
        get { return _Number; }
        set { _Number = value; }
    }

    #endregion
}
