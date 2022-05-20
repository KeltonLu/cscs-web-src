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
/// AGREEMENT
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class AGREEMENT
{
    public AGREEMENT()
    { }

    #region Model
    private int _RID;
    private string _RCU;
    private string _RUU;
    private DateTime _RCT = Convert.ToDateTime("1900-01-01");
    private DateTime _RUT = Convert.ToDateTime("1900-01-01");
    private string _RST;
    private string _Agreement_Name;
    private string _IMG_File_URL;
    private int _Factory_RID;
    private int _Card_Number;
    private int _Remain_Card_Num;
    private DateTime _Begin_Time = Convert.ToDateTime("1900-01-01");
    private DateTime _End_Time = Convert.ToDateTime("1900-01-01");
    private string _Agreement_Code;
    private string _Agreement_Code_Main;
    private string _Reason;
    private string _IMG_File_Name;

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
    public string Agreement_Name
    {
        get { return _Agreement_Name; }
        set { _Agreement_Name = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string IMG_File_URL
    {
        get { return _IMG_File_URL; }
        set { _IMG_File_URL = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Factory_RID
    {
        get { return _Factory_RID; }
        set { _Factory_RID = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Card_Number
    {
        get { return _Card_Number; }
        set { _Card_Number = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Remain_Card_Num
    {
        get { return _Remain_Card_Num; }
        set { _Remain_Card_Num = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Begin_Time
    {
        get { return _Begin_Time; }
        set { _Begin_Time = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime End_Time
    {
        get { return _End_Time; }
        set { _End_Time = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Agreement_Code
    {
        get { return _Agreement_Code; }
        set { _Agreement_Code = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Agreement_Code_Main
    {
        get { return _Agreement_Code_Main; }
        set { _Agreement_Code_Main = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Reason
    {
        get { return _Reason; }
        set { _Reason = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string IMG_File_Name
    {
        get { return _IMG_File_Name; }
        set { _IMG_File_Name = value; }
    }

    #endregion


}

