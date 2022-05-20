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
/// IMPORT_PROJECT
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class IMPORT_PROJECT
{
    public IMPORT_PROJECT()
    { }

    #region Model
    private int _RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _File_Name;
    private int _MakeCardType_RID;
    private string _Type;

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
    public string File_Name
    {
        get { return _File_Name; }
        set { _File_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int MakeCardType_RID
    {
        get { return _MakeCardType_RID; }
        set { _MakeCardType_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Type
    {
        get { return _Type; }
        set { _Type = value; }
    }

    #endregion


}

