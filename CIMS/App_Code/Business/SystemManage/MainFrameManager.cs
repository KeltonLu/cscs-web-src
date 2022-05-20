//******************************************************************
//*  作    者：FangBao
//*  功能說明：框架頁面邏輯
//*  創建日期：2008-07-31
//*  修改日期：2008-07-31 12:00
//*  修改記錄：
//*            □2008-07-31
//*              1.創建 鮑方
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
/// <summary>
/// MainFrameManager 的摘要描述
/// </summary>
public class MainFrameManager
{
    /// <summary>
    /// 建構
    /// </summary>
    public MainFrameManager()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 整理Menu資料
    /// </summary>
    /// <param name="node">全局菜單的資料</param>
    /// <param name="actions">當前用戶的權限列表</param>
    private void SetMenuData(XmlNode node, List<string> actions)
    {
        if (node.Attributes["FunctionID"] != null || node.Name == "NewDataSet")//如果資料合法
        {
            List<XmlNode> lstXML = new List<XmlNode>();
            List<XmlNode> lstNotDel = new List<XmlNode>();


            for (int index = 0; index < node.ChildNodes.Count; index++)//搜索未被授權的菜單項資料
            {
                if (!actions.Contains(node.ChildNodes[index].Attributes["FunctionID"].Value.Trim() + "1") && !actions.Contains(node.ChildNodes[index].Attributes["FunctionID"].Value.Trim()))
                {
                    lstXML.Add(node.ChildNodes[index]);
                }
            }

            for (int index = 0; index < lstXML.Count; index++)//清除未被授權的菜單項資料
            {
                node.RemoveChild(lstXML[index]);
            }

            for (int index = 0; index < node.ChildNodes.Count; index++)//遞歸
            {
                SetMenuData(node.ChildNodes[index], actions);
            }
        }


    }


    /// <summary>
    /// 綁定Menu資料
    /// </summary>
    /// <param name="Menu_Main">Menu控製項</param>
    public void GetMenuData(Menu Menu_Main)
    {
        XmlDataSource xdsXmlDataSource_Menu = null;
        XmlDocument xdocMenuData = null;

        xdsXmlDataSource_Menu = new XmlDataSource();
        xdsXmlDataSource_Menu.EnableCaching = false;
        xdocMenuData = ((XmlDocument)(MenuDataProvider.MenuXmlDocument.Clone()));
        SetMenuData(xdocMenuData.FirstChild, (List<string>)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.ACTIONS]);//基於權限來整理資料
        xdsXmlDataSource_Menu.Data = xdocMenuData.InnerXml;
        xdsXmlDataSource_Menu.XPath = "/NewDataSet/*";
        for (int index = 0; index < Menu_Main.DataBindings.Count; index++)//設定綁定目標屬性
        {
            Menu_Main.DataBindings[index].TextField = "FunctionName";
        }

        Menu_Main.DataSource = xdsXmlDataSource_Menu;
        Menu_Main.DataBind();
    }
}
