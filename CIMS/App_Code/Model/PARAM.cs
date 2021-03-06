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
/// PARAM
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class PARAM
{
    public PARAM()
    { }

    #region Model
    private int _RID;
    private string _Param_Code;
    private string _Param_Name;
    private string _ParamType_Code;
    private string _ParamType_Name;
    private string _Is_Delete;
    private string _Param_Comment;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    //201005CR 代碼設定-紙品物料庫存原因設定，新增 合併計算 欄位 add by Ian Huang 2010/06/11 start
    private string _Is_Merge;
    //201005CR 代碼設定-紙品物料庫存原因設定，新增 合併計算 欄位 add by Ian Huang 2010/06/11 end

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
    public string Param_Code
    {
        get { return _Param_Code; }
        set { _Param_Code = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Param_Name
    {
        get { return _Param_Name; }
        set { _Param_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string ParamType_Code
    {
        get { return _ParamType_Code; }
        set { _ParamType_Code = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string ParamType_Name
    {
        get { return _ParamType_Name; }
        set { _ParamType_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Is_Delete
    {
        get { return _Is_Delete; }
        set { _Is_Delete = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Param_Comment
    {
        get { return _Param_Comment; }
        set { _Param_Comment = value; }
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

    //201005CR 代碼設定-紙品物料庫存原因設定，新增 合併計算 欄位 add by Ian Huang 2010/06/11 start
    /// <summary>
    /// 
    /// </summary>
    public string Is_Merge
    {
        get { return _Is_Merge; }
        set { _Is_Merge = value; }
    }
    //201005CR 代碼設定-紙品物料庫存原因設定，新增 合併計算 欄位 add by Ian Huang 2010/06/11 end

    #endregion


}

