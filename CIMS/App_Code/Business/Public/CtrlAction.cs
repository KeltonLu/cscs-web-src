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
/// 控製項權限組態信息
/// </summary>
public struct CtrlAction
{

    private Dictionary<string, string> dirByActions;//控製項權限組態集合
    /// <summary>
    /// 控製項權限組態集合
    /// </summary>
    public Dictionary<string, string> ByActions { get { return dirByActions; } }

    private string strName;//控製項物件名稱
    /// <summary>
    /// 控製項物件名稱
    /// </summary>
    public string Name { get { return strName; } }

    private string strMember;//控製項受權限控製的bool型成員
    /// <summary>
    /// 控製項受權限控製的bool型成員
    /// </summary>
    public string Menber { get { return strMember; } }

    private string strFunctionID;//功能編碼
    /// <summary>
    /// 功能編碼
    /// </summary>
    public string FunctionID { get { return strFunctionID; } }

    private int intColumn;//如果控製項是GridView，則表示要控製的列的序號
    /// <summary>
    /// 如果控製項是GridView，則表示要控製的列的序號
    /// </summary>
    public int Column { get { return intColumn; } set { intColumn = value; } }

    /// <summary>
    /// 建構
    /// </summary>
    /// <param name="baseNode">組態XML節點</param>
    /// <returns>控製項訪問權限編碼</returns>
    public CtrlAction(XmlNode baseNode)
    {
        dirByActions = new Dictionary<string, string>();

        if (baseNode.Attributes["member"] != null)//如果組態節點有"成員"設定
        {
            strMember = baseNode.Attributes["member"].Value;
        }
        else//沒有設定則默認成員是“Visible”
        {
            strMember = "Visible";
        }
        if (baseNode.Attributes["name"].Value.Contains("."))//如果組態節點控製項名稱中有"."符號，説明是GridView控製項的列控製項
        {
            string[] strvalues = baseNode.Attributes["name"].Value.Split(char.Parse("."));
            intColumn = int.Parse(strvalues[1].Trim());
        }
        else//否則是一般控製項
        {
            intColumn = -1;

        }
        strName = baseNode.Attributes["name"].Value;
        if (baseNode.Attributes["functionid"] != null)//如果組態節點有功能編號設定
        {
            strFunctionID = baseNode.Attributes["functionid"].Value;
        }
        else//無功能編號描述時繼承父節點的功能編號
        {
            strFunctionID = baseNode.ParentNode.Attributes["functionid"].Value;
        }
        string[] strTemps = baseNode.Attributes["code"].Value.Split(char.Parse(","));
        for (int index = 0; index < strTemps.Length; index++)//整理行爲編碼集合
        {
            if (strTemps[index] != "")
            {
                dirByActions.Add(strTemps[index], PopedomManager.MainPopedomManager.ActionTypes[strTemps[index]]);
            }
        }
    }
    /// <summary>
    /// 獲得控製項權限組態編碼
    /// </summary>
    /// <param name="page">控製項所在頁面</param>
    /// <param name="actionTypeName">URL行爲類別參數名稱</param>
    /// <returns>權限編碼</returns>
    public string GetActionID(Page page, string actionTypeName)
    {
        StringBuilder stbID = new StringBuilder(this.FunctionID);
        if (this.ByActions.Count > 1)//如果權限行爲組態為多項，説明需要根據URL參數決定行爲編碼
        {
            string strActionType = page.Request.QueryString[actionTypeName];
            if (StringUtil.IsEmpty(strActionType))//如果未取得行爲名稱
            {
                return "";
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
