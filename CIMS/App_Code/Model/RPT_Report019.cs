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
/// RPT_Report019 的摘要描述
/// </summary>
public class RPT_Report019
{
    public RPT_Report019()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    #region Model
    private string _日期;
    private string _TYPE;
    private string _AFFINITY;
    private string _PHOTO;
    private string _版面簡稱;    
    private string _Name;
    private string _Number;
    private string _TimeMark;


    public string TimeMark
    {
        get { return _TimeMark; }
        set { _TimeMark = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    public string 日期
    {
        get { return _日期; }
        set { _日期 = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string TYPE
    {
        get { return _TYPE; }
        set { _TYPE = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string AFFINITY
    {
        get { return _AFFINITY; }
        set { _AFFINITY = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string PHOTO
    {
        get { return _PHOTO; }
        set { _PHOTO = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string 版面簡稱
    {
        get { return _版面簡稱; }
        set { _版面簡稱 = value; }
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
    public string Number
    {
        get { return _Number; }
        set { _Number = value; }
    }

    #endregion
}
