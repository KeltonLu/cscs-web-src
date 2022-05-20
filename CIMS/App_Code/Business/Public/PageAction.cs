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
/// 頁面權限組態信息
/// </summary>
public struct PageAction
{

    private Dictionary<string, string> dirByActions;//頁面權限組態集合
    /// <summary>
    /// 頁面權限組態集合
    /// </summary>
    public Dictionary<string, string> ByActions { get { return dirByActions; } }

    private Dictionary<string, CtrlAction> ditCtrls;//頁面控製項權限組態信息集合
    /// <summary>
    /// 頁面控製項權限組態信息集合
    /// </summary>
    public Dictionary<string, CtrlAction> Ctrls { get { return ditCtrls; } }

    private string strUrl;//頁面地址
    /// <summary>
    /// 頁面地址
    /// </summary>
    public string Url { get { return strUrl; } }

    private string strFunctionID;//功能編碼
    /// <summary>
    /// 功能編碼
    /// </summary>
    public string FunctionID { get { return strFunctionID; } }

    /// <summary>
    /// 建構
    /// </summary>
    /// <param name="baseNode">組態XML節點</param>
    public PageAction(XmlNode baseNode)
    {
        dirByActions = new Dictionary<string, string>();
        ditCtrls = new Dictionary<string, CtrlAction>();
        strUrl = HttpContext.Current.Server.MapPath(baseNode.Attributes["url"].Value).ToUpper();
        strFunctionID = baseNode.Attributes["functionid"].Value;
        string[] strActions = baseNode.Attributes["by"].Value.Split(char.Parse(","));

        for (int index = 0; index < strActions.Length; index++)//整理行爲編碼信息
        {
            if (strActions[index] != "")
            {
                dirByActions.Add(strActions[index], PopedomManager.MainPopedomManager.ActionTypes[strActions[index]]);
            }
        }

        XmlNodeList Ctrls = baseNode.SelectNodes("Contral");
        for (int index = 0; index < Ctrls.Count; index++)//整理控製項信息
        {
            CtrlAction ctaCtrl = new CtrlAction(Ctrls[index]);
            ditCtrls.Add(ctaCtrl.Name, ctaCtrl);
        }

    }



    /// <summary>
    /// 獲得頁面權限組態編碼
    /// </summary>
    /// <param name="page">對應的頁面</param>
    /// <param name="actionTypeName">URL行爲類別參數名稱</param>
    /// <param name="defaultActionName">默認的行爲類別名稱</param>
    /// <returns>頁面訪問權限編碼</returns>
    public string GetPageActionID(Page page, string actionTypeName, string defaultActionName)
    {
        StringBuilder stbID = new StringBuilder(this.FunctionID);
        if (this.ByActions.Count > 1)//如果權限行爲組態為多項，説明需要根據URL參數決定行爲編碼
        {
            string strActionType = page.Request.QueryString[actionTypeName];
            if (StringUtil.IsEmpty(strActionType))//如果未取得行爲名稱
            {
                stbID.Append(this.ByActions[defaultActionName]);//通過默認行爲名稱從行爲組態集合中返回對應的行爲編碼
            }
            else
            {
                stbID.Append(this.ByActions[strActionType]);//從行爲組態集合中返回對應的行爲編碼
            }
        }
        else if (this.ByActions.Count == 1)//如果權限行爲組態為單項，説明不需要通過URL參數決定行爲編碼
        {
            foreach (string value in this.ByActions.Values)//取1個行爲編碼
            {
                stbID.Append(value);
                break;
            }
        }
        else//行爲編碼集合是空的
        {
            return "";
        }
        return stbID.ToString();
    }

}