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
/// RPT_Report025 的摘要描述
/// </summary>
public class RPT_Report025
{
    public RPT_Report025()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    #region Model    
    private string _Change_Date;    
    private string _Name;
    private int _Number;
    private string _strType;
    private string _TimeMark;


    public string TimeMark
    {
        get { return _TimeMark; }
        set { _TimeMark = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Change_Date
    {
        get { return _Change_Date; }
        set { _Change_Date = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string strType
    {
        get { return _strType; }
        set { _strType = value; }
    }  

    /// <summary>
    /// 
    /// </summary>
    public int Number
    {
        get { return _Number; }
        set { _Number = value; }
    }

    #endregion
}
