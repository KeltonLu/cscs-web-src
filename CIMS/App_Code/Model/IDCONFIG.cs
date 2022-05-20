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
/// IDCONFIG
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class IDCONFIG
{
    public IDCONFIG()
    { }

    #region Model
    private int _ID;
    private string _TypeName;
    private string _FormatString;
    private int _LastNumberLength;
    private DateTime _LastDate = Convert.ToDateTime("1900-01-01");
    private int _LastNumber;

    /// <summary>
    /// 
    /// </summary>
    public int ID
    {
        get { return _ID; }
        set { _ID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string TypeName
    {
        get { return _TypeName; }
        set { _TypeName = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FormatString
    {
        get { return _FormatString; }
        set { _FormatString = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int LastNumberLength
    {
        get { return _LastNumberLength; }
        set { _LastNumberLength = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime LastDate
    {
        get { return _LastDate; }
        set { _LastDate = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int LastNumber
    {
        get { return _LastNumber; }
        set { _LastNumber = value; }
    }

    #endregion


}

