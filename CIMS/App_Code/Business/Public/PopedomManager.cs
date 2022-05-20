//******************************************************************
//*  作    者：QingChen
//*  功能說明：在綫信息管理容器
//*  創建日期：2008/05/27
//*  修改日期：2008/05/27  16:59
//*  修改記錄：
//*            □2008/05/27 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 網站頁面權限組態緩衝邏輯
/// </summary>
public class PopedomManager
{
    /// <summary>
    /// 建構
    /// </summary>
    public PopedomManager()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    private bool blnInited = false;//是否已初始化
    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool Inited { get { return blnInited; } }

    private static volatile PopedomManager popedomManager = new PopedomManager();//網站頁面權限組態緩衝
    /// <summary>
    /// 網站頁面權限組態緩衝
    /// </summary>
    public static PopedomManager MainPopedomManager
    {
        get
        {
            return popedomManager;
        }
    }

    private volatile Dictionary<string, string> dirActionTypes = new Dictionary<string, string>();//行爲類型編碼集合
    /// <summary>
    /// 行爲類型編碼集合
    /// </summary>
    public Dictionary<string, string> ActionTypes { get { return dirActionTypes; } }

    private volatile Dictionary<string, PageAction> dirPageSettings = new Dictionary<string, PageAction>();//頁面權限組態集合
    /// <summary>
    /// 頁面權限組態集合
    /// </summary>
    public Dictionary<string, PageAction> PageSettings { get { return dirPageSettings; } }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {

        XmlDocument xdocConfig = new XmlDocument();
        string strPath = HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["PageActionSettingFile"]);
        try
        {
            xdocConfig.Load(strPath);//讀取頁面權限組態文件
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizPublicMsg.ALT_ReadPageActionFail, ex);
            throw ex;
        }
        XmlNodeList xnlTemp = xdocConfig.SelectNodes("/Root/ActionCodeSettings/Code");
        lock (dirActionTypes)//整理行爲類別基準組態集合
        {
            dirActionTypes.Clear();
            for (int index = 0; index < xnlTemp.Count; index++)
            {
                dirActionTypes.Add(xnlTemp[index].Attributes["key"].Value, xnlTemp[index].Attributes["value"].Value);
            }
        }
        xnlTemp = xdocConfig.SelectNodes("/Root/Pages/Page");
        lock (dirPageSettings)//整理頁面權限組態集合
        {
            dirPageSettings.Clear();
            for (int index = 0; index < xnlTemp.Count; index++)
            {
                PageAction NewSetting = new PageAction(xnlTemp[index]);
                dirPageSettings.Add(NewSetting.Url, NewSetting);
            }
        }
        blnInited = true;
    }
}

