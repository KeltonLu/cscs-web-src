//******************************************************************
//*  作    者：QingChen
//*  功能說明：在綫信息管理容器
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.ComponentModel;
using System.Data.Common;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;


/// <summary>
/// 要做權限判斷的頁面基礎類別
/// </summary>
public class PageBase : System.Web.UI.Page
{
    /// <summary>
    /// 建構
    /// </summary>
    public PageBase(): base()
    {

    }

    /// <summary>
    /// 當前頁面察看權限編碼
    /// </summary>
    public string PageActionID
    {
        set
        {
            Session["Action"] = value;
        }
        get
        {
            return Session["Action"].ToString();
        }
    }

    /// <summary>
    /// 當前用戶的權限列表
    /// </summary>
    protected List<string> UserActions
    {
        get { return (List<string>)Session[GlobalString.SessionAndCookieKeys.ACTIONS]; }
    }

    /// <summary>
    /// 頁面異常重寫
    /// </summary>
    protected override void OnError(EventArgs e)
    {
        if (Session[GlobalString.SessionAndCookieKeys.USER] == null)//有用戶信息
        {
            string strErr = BizMessage.BizPublicMsg.ALT_NotLogin;
            ExceptionFactory.CreateAlertException(this, strErr);
            MainFreamGoPage(GlobalString.PageUrl.LOGIN);

            return;
        }

        System.Exception ex = Server.GetLastError();
        Server.ClearError();

        Session["exception"] = ex;
        Response.Redirect("~/Warning.aspx?ErrorPath=" + Request.RawUrl);



        //StringBuilder stbCode = new StringBuilder("<script>alert('");
        //stbCode.Append(E.Message);
        //stbCode.Append("');</script>");
        //this.Response.Write(stbCode.ToString());
        //this.Response.End();
    }

    /// <summary>
    /// 黨頁面加載時
    /// </summary>
    /// <param name="e">事件參數</param>
    protected override void OnLoad(EventArgs e)
    {
        if (Session[GlobalString.SessionAndCookieKeys.USER] == null)//有用戶信息
        {
            string strErr = BizMessage.BizPublicMsg.ALT_NotLogin;
            ExceptionFactory.CreateAlertException(this,strErr);
            MainFreamGoPage(GlobalString.PageUrl.LOGIN);
            
            return;
        }

        if (!IsPostBack)
        {
            string ActionID = "8888";

            //權限識別--------------------------------------------------------------------------------
            string strPath = this.Server.MapPath(this.Request.Url.AbsolutePath).ToUpper();
            PageAction pgaNow = PopedomManager.MainPopedomManager.PageSettings[strPath];
            ActionID = pgaNow.GetPageActionID(this, GlobalString.PageUrl.ACTION_TYPE_BASE, GlobalString.ActionName.SEARCH);//頁面權限
            this.PageActionID = ActionID;
            foreach (string strCtrl in pgaNow.Ctrls.Keys)//控製項權限
            {
                CtrlAction ctrlaNow = pgaNow.Ctrls[strCtrl];
                if (pgaNow.Ctrls[strCtrl].Column != -1)//控製項是GridView
                {
                    string strCtrlName = strCtrl.Split(char.Parse("."))[0].Trim();
                    object objCtrl = FindCtrl(strCtrlName);
                    if (objCtrl != null)
                    {
                        Type typCtrl = objCtrl.GetType();
                        GridView grvTemp = (GridView)objCtrl;
                        string strID = ctrlaNow.GetActionID(this, GlobalString.PageUrl.ACTION_TYPE_BASE);
                        //this.PageActionID = strID;
                        grvTemp.Columns[pgaNow.Ctrls[strCtrl].Column].Visible = CanUseAction(strID);
                    }
                }
                else
                {
                    string strCtrlName = strCtrl;
                    if (strCtrl.Contains("$"))
                    {
                        strCtrlName = strCtrlName.Split('$')[0].Trim();
                    }
                    object objCtrl = FindCtrl(strCtrlName);
                    if (objCtrl != null)
                    {
                        Type typCtrl = objCtrl.GetType();
                        PropertyInfo pri = typCtrl.GetProperty(pgaNow.Ctrls[strCtrl].Menber);
                        string strID = ctrlaNow.GetActionID(this, GlobalString.PageUrl.ACTION_TYPE_BASE);
                        //this.PageActionID = strID;
                        if (strID != "")
                        {
                            pri.SetValue(objCtrl, CanUseAction(strID), null);
                        }
                        else
                        {
                            //string strErr = BizMessage.BizPublicMsg.ALT_NoCtrlAction;
                            //ExceptionFactory.CreateAlertException(this, strErr);
                        }
                    }
                }

                if (Session[GlobalString.SessionAndCookieKeys.ACTIONS] != null)//有權限信息
                {
                    if (ActionID != "index")
                    {
                        if (!CanUseAction(ActionID))//沒有訪問本頁的權限
                        {
                            string strErr = BizMessage.BizPublicMsg.ALT_HasNoAction;
                            ExceptionFactory.CreateAlertException(this, strErr);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), string.Concat("window.location='", GlobalString.PageUrl.NOPAGEACTION, "';"), true);
                        }
                    }
                }
                else
                {
                    string strErr = BizMessage.BizPublicMsg.ALT_HasNoAuth;
                    ExceptionFactory.CreateAlertException(this, strErr);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), string.Concat("window.location='", GlobalString.PageUrl.NOPAGEACTION, "';"), true);
                }
            }
        }

        //記錄按鈕的ACTION
        SetPageAction();

        base.OnLoad(e);
    }

    /// <summary>
    /// 當前按鈕的ACTION
    /// </summary>
    private void SetPageAction()
    {
        if(Request.Form.AllKeys.Length==0)
            return;

        string strPath = this.Server.MapPath(this.Request.Url.AbsolutePath).ToUpper();
        PageAction pgaNow = PopedomManager.MainPopedomManager.PageSettings[strPath];
        foreach (string strCtrl in pgaNow.Ctrls.Keys)//控製項權限
        {
            if (Request.Form[strCtrl] == null)
                continue;

            CtrlAction ctrlaNow = pgaNow.Ctrls[strCtrl];


            if (pgaNow.Ctrls[strCtrl].Column != -1)//控製項是GridView
            {
                string strCtrlName = strCtrl.Split(char.Parse("."))[0].Trim();
                object objCtrl = FindCtrl(strCtrlName);
                if (objCtrl != null)
                {
                    Type typCtrl = objCtrl.GetType();
                    GridView grvTemp = (GridView)objCtrl;
                    string strID = ctrlaNow.GetActionID(this, GlobalString.PageUrl.ACTION_TYPE_BASE);
                    //this.PageActionID = strID;
                }
            }
            else
            {
                string strCtrlName = strCtrl;
                if (strCtrl.Contains("$"))
                {
                    strCtrlName = strCtrlName.Split('$')[0].Trim();
                }
                object objCtrl = FindCtrl(strCtrlName);
                if (objCtrl != null)
                {
                    Type typCtrl = objCtrl.GetType();
                    PropertyInfo pri = typCtrl.GetProperty(pgaNow.Ctrls[strCtrl].Menber);
                    string strID = ctrlaNow.GetActionID(this, GlobalString.PageUrl.ACTION_TYPE_BASE);
                    this.PageActionID = strID;
                }
            }
        }
    }
 

    #region 變數

    #endregion

    #region 屬性

    #endregion

    #region 事件
    ///// <summary>
    ///// 頁面加載重寫
    ///// </summary>
    ////protected override void OnPreLoad(EventArgs e)
    ////{
    ////    base.OnPreLoad(e);

    ////    // 檢查使用者的Session
    ////    this.ValidateUser();
    ////}
    #endregion



 

    #region 方法

    public bool IsCheck()
    {
        Depository003BL bl = new Depository003BL();
        bool blCheck = false;
        blCheck = bl.IsCheck();
        return blCheck;
    }

    /// <summary>
    /// 綁定卡種停用下拉框
    /// </summary>
    /// <param name="drop">下拉框</param>
    /// <param name="strRID">下拉框ID</param>
    public void ShowIsUsingDrop(DropDownList drop, string strRID)
    {
        if (StringUtil.IsEmpty(strRID))
            return;

        if (drop.Items.FindByValue(strRID) == null)
        {
            CardTypeManager ctm = new CardTypeManager();
            CARD_TYPE ctModel = ctm.GetModel(strRID);
            if (ctModel != null)
            {
                drop.Items.Add(new ListItem(ctModel.Name, ctModel.RID.ToString()));
                drop.SelectedValue = strRID;
            }
        }
    }

    /// <summary>
    /// 綁定卡種停用下拉框
    /// </summary>
    /// <param name="drop">下拉框</param>
    /// <param name="strRID">下拉框ID</param>
    public void ShowIsUsingDrop(DropDownList drop, string strRID, bool IsFullName)
    {
        if (StringUtil.IsEmpty(strRID))
            return;

        if (drop.Items.FindByValue(strRID) == null)
        {
            CardTypeManager ctm = new CardTypeManager();
            CARD_TYPE ctModel = ctm.GetModel(strRID);
            if (ctModel != null)
            {
                if (IsFullName)
                {
                    drop.Items.Add(new ListItem(ctModel.Display_Name, ctModel.RID.ToString()));
                    drop.SelectedValue = strRID;
                }
                else
                {
                    drop.Items.Add(new ListItem(ctModel.Name, ctModel.RID.ToString()));
                    drop.SelectedValue = strRID;
                }
            }
        }
    }



    /// <summary>
    /// 提示並重定向方法
    /// </summary>
    /// <param name="strMessage">要提示的信息</param>
    /// <param name="strUrl">要重訂向德地址</param>
    public void ShowMessageAndGoPage(string strMessage, string strUrl)
    {
        StringBuilder stbCode = new StringBuilder("alert('");
        stbCode.Append(strMessage);
        stbCode.Append("');window.location='");
        stbCode.Append(strUrl);
        stbCode.Append("';");
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), stbCode.ToString(), true);
    }

    /// <summary>
    /// 提示信息
    /// </summary>
    /// <param name="message">提示信息</param>
    public void ShowMessage(string strMessage)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "alert('" + strMessage + "');", true);
    }

    /// <summary>
    /// 提示信息
    /// </summary>
    /// <param name="message">提示信息</param>
    public void ShowWait(string strMessage)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "top.ShowWaitDiv('" + strMessage + "');", true);
    }

    //************************************************************************
    /// <summary>
    /// 建立上傳目錄
    /// </summary>
    /// <returns>true：成功 false：失敗</returns>
    //************************************************************************
    public bool IsFolderExist()
    {
        string basepath = Server.MapPath(ConfigurationSettings.AppSettings["WebName"]);
        try
        {
            // Determine whether the directory exists.	
            if (!Directory.Exists(basepath))
            {
                // Create the directory it does not exist.
                Directory.CreateDirectory(basepath);
            }
            return true;
        }

        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 文件上傳功能
    /// </summary>
    /// <param name="file">上傳文件</param>
    protected bool FileUpload(HttpPostedFile file , Image image)
    {
        string path = "";
        if (IsFolderExist())
        {
            string tmpname = file.FileName;
            int i = tmpname.LastIndexOf("\\");
            string filename = tmpname.Substring(i + 1);
            string filetype = filename.Substring(filename.LastIndexOf(".") + 1);
            filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filetype;
            if (filetype.ToLower().Equals("gif") || filetype.ToLower().Equals("jpg") || filetype.ToLower().Equals("jpeg") || filetype.ToLower().Equals("bmp") || filetype.ToLower().Equals("tif"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a == 0.0)
                    {
                        ShowMessage(BizMessage.BizPublicMsg.ALT_PicUploadFail);
                        return false;
                    }
                    else
                    {

                        if (a <= 10000.00)
                        {
                            file.SaveAs(Server.MapPath(ConfigurationManager.AppSettings["WebName"] + filename));
                            path = ConfigurationManager.AppSettings["WebName"] + filename;
                            image.ImageUrl = path;
                            ShowMessage(BizMessage.BizPublicMsg.ALT_PicUpdSucess);
                            return true;
                        }
                        else
                        {
                            ShowMessage(BizMessage.BizPublicMsg.ALT_PicTooLarge);
                            return false;
                        }
                    }
                }
                catch
                {
                    ShowMessage(BizMessage.BizPublicMsg.ALT_PicUploadFail);
                    return false;
                }
            }
            else
            {
                ShowMessage(BizMessage.BizPublicMsg.ALT_PicWrongFormat);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 文件上傳功能
    /// </summary>
    /// <param name="file">上傳文件</param>
    protected string FileUpload(HttpPostedFile file)
    {
        string path = "";
        if (IsFolderExist())
        {
            string tmpname = file.FileName;
            int i = tmpname.LastIndexOf("\\");
            string filename = tmpname.Substring(i + 1);
            string filetype = filename.Substring(filename.LastIndexOf(".") + 1);
            filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filetype;
            if (filetype.ToLower().Equals("txt"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a <= 10000.00)
                    {
                        file.SaveAs(Server.MapPath(ConfigurationManager.AppSettings["WebName"] + filename));
                        path = Server.MapPath(ConfigurationManager.AppSettings["WebName"] + filename);
                    }
                    else
                    {
                        ShowMessage("大小不能大於10M");
                    }
                }
                catch
                {
                    ShowMessage("上傳失敗");
                }
            }
            else
            {
                ShowMessage("上傳格式請為TXT檔");
            }
        }
        return path;
    }


    /// <summary>
    /// 是否接受不安全資料流
    /// </summary>
    protected virtual bool CanInputHTML
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// 全局使用者查詢條件歷史
    /// </summary>
    protected Dictionary<string, string> UserSearchValues
    {
        get
        {
            if (Session["UserSearchValues"] == null)
            {
                Dictionary<string, string> dirUserSearchValues = new Dictionary<string, string>(2);
                dirUserSearchValues.Add("ID", string.Empty);
                dirUserSearchValues.Add("Name", string.Empty);
                Session["UserSearchValues"] = dirUserSearchValues;
            }
            return (Dictionary<string, string>)Session["UserSearchValues"];
        }
    }


    /// <summary>
    /// 頁面跳轉
    /// </summary>
    /// <param name="url">URL</param>
    public void MainFreamGoPage(string url)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), string.Concat("window.open('", url, "','_top');"), true);
    }

    /// <summary>
    /// 查找控製項
    /// </summary>
    /// <param name="name">控製項名稱</param>
    /// <returns>控製項</returns>
    protected Control FindCtrl(string name)
    {
        //if (this.Master == null)//如果是有主版
        //{
        return this.FindControl(name);
        //}
        //else
        //{
        //    return (Control)(((MasterBase)this.Master).GetContral(name));
        //}
    }


    /// <summary>
    /// 判斷用戶是否有某個權限
    /// </summary>
    /// <param name="actionid">權限編碼</param>
    /// <returns>true：有,false：沒有</returns>
    protected bool CanUseAction(string actionid)
    {
        if (StringUtil.IsEmpty(actionid))
        {
            return false;
        }
        if (actionid.Contains(","))
        {
            return CanUseAction(actionid.Split(",".ToCharArray()));
        }
        else
        {
            return this.UserActions.Contains(actionid);
        }
    }

    /// <summary>
    /// 判斷用戶是否有某個權限
    /// </summary>
    /// <param name="actionid">權限編碼數組</param>
    /// <returns>true：有,false：沒有</returns>
    protected bool CanUseAction(params string[] actionid)
    {

        for (int index = 0; index < actionid.Length; index++)
        {
            if (this.UserActions.Contains(actionid[index].Trim()))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 導出EXCEL
    /// </summary>
    /// <param name="dtblSource">資料源</param>
    /// <param name="FileName">文件名</param>
    public void DataTable2Excel(DataTable dtblSource, string FileName)
    {
        Response.Clear();

        System.Web.UI.WebControls.DataGrid dgData=null;

        dgData.DataSource = dtblSource;

        dgData.DataBind();

        FileName += DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss");

        FileName = HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8);

        Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".xls");

        Response.Charset = "GB2312";

        Response.Write("<meta http-equiv=Content-Type content=text/html;charset=gb2312>");

        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");

        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        Response.ContentType = "application/vnd.xls";

        System.IO.StringWriter stringWrite = new System.IO.StringWriter();

        System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

        dgData.RenderControl(htmlWrite);

        Response.Write(stringWrite.ToString());

        Response.End();
    }


    /// <summary>
    /// 導出EXCEL
    /// </summary>
    /// <param name="dtblSource">資料源</param>
    /// <param name="FileName">文件名</param>
    public void GridView2Excel(GridView dgData, string FileName)
    {
        Response.Clear();

        Response.Buffer = true;

        Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName + DateTime.Now.ToString("yyyyMMddhhmmdd") + ".xls");

        Response.Charset = "GB2312";

        Response.Write("<meta http-equiv=Content-Type content=text/html;charset=gb2312>");

        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");

        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        Response.ContentType = "application/vnd.xls";

        System.IO.StringWriter stringWrite = new System.IO.StringWriter();

        System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

        dgData.RenderControl(htmlWrite);

        Response.Write(stringWrite.ToString());

        Response.End();

        this.EnableViewState = false;　
    }

    public void ExportExcel(string strFileName)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AppendHeader("Content-Disposition", "attachment;filename=" +strFileName+ DateTime.Now.ToString("yyyyMMddhhmmdd") + ".xls");
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
        Response.Write("<meta http-equiv=Content-Type content=text/html;charset=gb2312>");
        Response.ContentType = "application/vnd.ms-excel";
        this.EnableViewState = false;　
    }

    /// <summary>
    /// 将集合类转换成DataTable
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public DataTable ToDataTable(IList list)
    {
        DataTable result = new DataTable();
        if (list.Count > 0)
        {
            PropertyInfo[] propertys = list[0].GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                result.Columns.Add(pi.Name, pi.PropertyType);
            }

            for (int i = 0; i < list.Count; i++)
            {
                ArrayList tempList = new ArrayList();
                foreach (PropertyInfo pi in propertys)
                {
                    object obj = pi.GetValue(list[i], null);
                    tempList.Add(obj);
                }
                object[] array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
        }
        return result;
    }


    #endregion

    #region 頁面綁定
    /// <summary>
    /// 綁定頁面查詢條件
    /// </summary>
    /// <returns>true:查詢 false:不查詢</returns>
    protected bool SetConData()
    {
        string strCon = Request.QueryString["Con"];
        if (!StringUtil.IsEmpty(strCon))
        {
            if (Session["Condition"] != null)
            {
                Dictionary<string, object> dirCon = (Dictionary<string, object>)Session["Condition"];

                foreach (KeyValuePair<string, object> entry in dirCon)
                {
                    Control control = FindCtrl(entry.Key);
                    if (control is System.Web.UI.WebControls.TextBox)
                    {
                        ((TextBox)control).Text = entry.Value.ToString().Trim();
                    }
                    else if (control is System.Web.UI.WebControls.DropDownList)
                    {
                        if (((DropDownList)control).Items.FindByValue(entry.Value.ToString().Trim()) != null)
                            ((DropDownList)control).SelectedValue = entry.Value.ToString().Trim();
                    }
                    else if (control is System.Web.UI.WebControls.RadioButtonList)
                    {
                        if (((RadioButtonList)control).Items.FindByValue(entry.Value.ToString().Trim()) != null)
                            ((RadioButtonList)control).SelectedValue = entry.Value.ToString().Trim();
                    }
                    else if (control is System.Web.UI.WebControls.RadioButton)
                    {
                        ((RadioButton)control).Checked = Convert.ToBoolean(entry.Value);
                    }
                    else if (control is System.Web.UI.WebControls.CheckBox)
                    {
                        ((CheckBox)control).Checked = Convert.ToBoolean(entry.Value);
                    }
                }
            }
            return true;
        }
        else
        {
            Session.Remove("Condition");
            return false;
        }
    }



    /// <summary>
    /// 綁定頁面的值
    /// </summary>
    /// <param name="argData">資料模型</param>
    protected void SetControls(object argData)
    {
        if (argData == null)
            return;
        Type dataType = argData.GetType();
        PropertyInfo[] properties = dataType.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            object argValue = property.GetValue(argData, null);
            if (argValue == null)
                continue;

            Control control=null;
            foreach (string name in controlName)
            {
                control = FindCtrl(name + property.Name);
                if (control != null)
                {
                    break;
                }
            }

            if (control == null)
                continue;

            if (control is System.Web.UI.WebControls.TextBox)
            {
                if (argValue.GetType().FullName == "System.DateTime")
                    argValue = Convert.ToDateTime(argValue).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "") ;
                ((TextBox)control).Text = argValue.ToString().Trim();
            }
            else if (control is System.Web.UI.WebControls.DropDownList)
            {
                if (((DropDownList)control).Items.FindByValue(argValue.ToString().Trim()) != null)
                    ((DropDownList)control).SelectedValue = argValue.ToString();
            }
            else if (control is System.Web.UI.WebControls.RadioButtonList)
            {
                if (((RadioButtonList)control).Items.FindByValue(argValue.ToString().Trim()) != null)
                    ((RadioButtonList)control).SelectedValue = argValue.ToString();
            }
            else if (control is System.Web.UI.WebControls.HyperLink)
                ((HyperLink)control).Text = argValue.ToString().Trim();
            else if (control is System.Web.UI.WebControls.Label)
                ((Label)control).Text = argValue.ToString().Trim();
            else if (control is System.Web.UI.HtmlControls.HtmlInputHidden)
                ((HtmlInputHidden)control).Value = argValue.ToString().Trim();
            else if (control is System.Web.UI.HtmlControls.HtmlInputFile)
                ((HtmlInputFile)control).Value = argValue.ToString().Trim();
            else if (control is System.Web.UI.HtmlControls.HtmlInputText)
                ((HtmlInputText)control).Value = argValue.ToString().Trim();
            else if (control is System.Web.UI.WebControls.CheckBox)
            {
                if (!StringUtil.IsEmpty(argValue.ToString().Trim()))
                {
                    if (argValue.ToString() == "Y")
                        ((CheckBox)control).Checked = true;
                }
            }
            else if (control is System.Web.UI.WebControls.RadioButton)
            {
                if (!StringUtil.IsEmpty(argValue.ToString().Trim()))
                {
                    if (argValue.ToString() == "Y")
                        ((RadioButton)control).Checked = true;
                }
            }
            else if(control is System.Web.UI.WebControls.Image)
                ((Image)control).ImageUrl = argValue.ToString().Trim();
        }
    }


    protected void SetControlsForDataRow(DataRow row)
    {
        foreach (DataColumn col in row.Table.Columns)
        {
            object argValue = row[col.ColumnName];

            Control control = null;

            foreach (string name in controlName)
            {
                control = FindControl(name + col.ColumnName);
                if (control != null)
                {
                    break;
                }
            }

            if (control == null)
                continue;

            if (control is System.Web.UI.WebControls.TextBox)
            {
                if (argValue.GetType().FullName == "System.DateTime")
                    argValue = Convert.ToDateTime(argValue).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "");
                ((TextBox)control).Text = argValue.ToString().Trim();
            }
            else if (control is System.Web.UI.WebControls.DropDownList)
            {
                if (((DropDownList)control).Items.FindByValue(argValue.ToString().Trim()) != null)
                    ((DropDownList)control).SelectedValue = argValue.ToString();
            }
            else if (control is System.Web.UI.WebControls.RadioButtonList)
            {
                if (((RadioButtonList)control).Items.FindByValue(argValue.ToString().Trim()) != null)
                    ((RadioButtonList)control).SelectedValue = argValue.ToString();
            }
            else if (control is System.Web.UI.WebControls.HyperLink)
                ((HyperLink)control).Text = argValue.ToString().Trim();
            else if (control is System.Web.UI.WebControls.Label)
                ((Label)control).Text = argValue.ToString().Trim();
            else if (control is System.Web.UI.HtmlControls.HtmlInputHidden)
                ((HtmlInputHidden)control).Value = argValue.ToString().Trim();
            else if (control is System.Web.UI.HtmlControls.HtmlInputFile)
                ((HtmlInputFile)control).Value = argValue.ToString().Trim();
            else if (control is System.Web.UI.HtmlControls.HtmlInputText)
                ((HtmlInputText)control).Value = argValue.ToString().Trim();
            else if (control is System.Web.UI.WebControls.CheckBox)
            {
                if (!StringUtil.IsEmpty(argValue.ToString().Trim()))
                {
                    if (argValue.ToString() == "Y")
                        ((CheckBox)control).Checked = true;
                }
            }
            else if (control is System.Web.UI.WebControls.RadioButton)
            {
                if (!StringUtil.IsEmpty(argValue.ToString().Trim()))
                {
                    if (argValue.ToString() == "Y")
                        ((RadioButton)control).Checked = true;
                }
            }
            else if (control is System.Web.UI.WebControls.Image)
                ((Image)control).ImageUrl = argValue.ToString().Trim();
        }
    }

    /// <summary>
    /// 清除控件的值
    /// </summary>
    protected void ClearControls()
    {
        foreach (Control control in Controls)
            ClearControls(control);
    }


    /// <summary>
    /// 清除控件的值
    /// </summary>
    /// <param name="control"></param>
    private void ClearControls(Control control)
    {
        if (control.HasControls())
        {
            foreach (Control con in control.Controls)
                ClearControls(con);
        }
        else
        {
            if (control is System.Web.UI.WebControls.TextBox)
                ((TextBox)control).Text = string.Empty;
            else if (control is System.Web.UI.WebControls.HyperLink)
                ((HyperLink)control).Text = string.Empty;
            else if (control is System.Web.UI.WebControls.Label)
                ((Label)control).Text = string.Empty;
            else if (control is System.Web.UI.HtmlControls.HtmlInputHidden)
                ((HtmlInputHidden)control).Value = string.Empty;
            else if (control is System.Web.UI.HtmlControls.HtmlInputFile)
                ((HtmlInputFile)control).Value = string.Empty;
            else if (control is System.Web.UI.HtmlControls.HtmlInputText)
                ((HtmlInputText)control).Value = string.Empty;
            else if (control is System.Web.UI.WebControls.CheckBox)
                ((CheckBox)control).Checked = false;
            else if (control is System.Web.UI.WebControls.RadioButton)
                ((RadioButton)control).Checked = false;
            else if (control is System.Web.UI.WebControls.Image)
                ((Image)control).ImageUrl = string.Empty;
        }
    }

    /// <summary>
    /// 獲取頁面控件的值
    /// </summary>
    /// <param name="argDataRow"></param>
    protected void SetData(object argData)
    {
        try
        {
            Type dataType = argData.GetType();
            PropertyInfo[] properties = dataType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Control control = null;
                foreach (string name in controlName)
                {
                    control = FindCtrl(name + property.Name);
                    if (control != null)
                    {
                        break;
                    }
                }

                if (control == null)
                    continue;

                object argValue = GetValue(control);
                if (argValue != null)
                {
                    SetPropertyValue(argData, property.Name.ToString(), argValue);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 設置屬性值
    /// </summary>
    /// <param name="argPropertyName"></param>
    /// <param name="val"></param>
    private void SetPropertyValue(object argData, string argPropertyName, object val)
    {
        PropertyInfo property = argData.GetType().GetProperty(argPropertyName);

        if (property == null)
            return;

        if (val != null && property.CanWrite)
        {
            if (StringUtil.IsEmpty(val.ToString()))
            {
                if (property.PropertyType == typeof(DateTime))
                    val = Convert.ToDateTime("1900-01-01");
                else if (property.PropertyType == typeof(decimal))
                    val = "0";
                else if (property.PropertyType == typeof(int))
                    val = "0";
                else if (property.PropertyType == typeof(long))
                    val = "0";
            }
            if (property.PropertyType == typeof(bool))
            {
                if ((bool)val)
                    val = "Y";
                else
                    val = "N";
                //val = Convert.ToInt32(val);
            }
            property.SetValue(argData, Convert.ChangeType(val, property.PropertyType), null);
        }
    }

    /// <summary>
    /// 獲取控件的值
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    private object GetValue(Control control)
    {
        object result = null;

        if (control is System.Web.UI.WebControls.TextBox)
            result = ((TextBox)control).Text.Trim();
        else if (control is System.Web.UI.WebControls.DropDownList)
            result = ((DropDownList)control).SelectedValue;
        else if (control is System.Web.UI.WebControls.RadioButtonList)
            result = ((RadioButtonList)control).SelectedValue;
        else if (control is System.Web.UI.WebControls.HyperLink)
            result = ((HyperLink)control).Text;
        else if (control is System.Web.UI.WebControls.Label)
            result = ((Label)control).Text;
        else if (control is System.Web.UI.HtmlControls.HtmlInputHidden)
            result = ((HtmlInputHidden)control).Value;
        else if (control is System.Web.UI.HtmlControls.HtmlInputFile)
            result = ((HtmlInputFile)control).Value;
        else if (control is System.Web.UI.HtmlControls.HtmlInputText)
            result = ((HtmlInputText)control).Value;
        else if (control is System.Web.UI.WebControls.CheckBox)
        {
            if (((CheckBox)control).Checked)
                result = "Y";
            else
                result = "N";
        }
        else if (control is System.Web.UI.WebControls.RadioButton)
        {
            if (((RadioButton)control).Checked)
                result = "Y";
            else
                result = "N";
        }
        else if (control is System.Web.UI.WebControls.Image)
            result = ((Image)control).ImageUrl;
        return result;
    }


    /// <summary>
    /// 控件集合
    /// </summary>
    private static string[] controlName = new string[]
	{   
		"chk",
        "drop",
        "img",
        "lbl",
        "rad",
        "txt",
        "hd",
        "radl"
	};
    #endregion
  
}
