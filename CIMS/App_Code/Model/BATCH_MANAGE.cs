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
/// BATCH_MANAGE
/// Date Created: 2008年11月19日
/// Created By:  FangBao
/// </summary>
public class BATCH_MANAGE
{
    public BATCH_MANAGE()
    { }

    #region Model
    private int _RID;
    private string _Comment;
    private string _Status;

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
    public string Comment
    {
        get { return _Comment; }
        set { _Comment = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Status
    {
        get { return _Status; }
        set { _Status = value; }
    }

    #endregion


}

