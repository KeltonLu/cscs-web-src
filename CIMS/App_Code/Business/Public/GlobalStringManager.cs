using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 全局常量字符管理
/// </summary>
public class GlobalStringManager
{
    
    public static GlobalStringManager Default = new GlobalStringManager("GlobalStringResource");//默認的管理器
    public static GlobalStringManager BizB = new GlobalStringManager("BusinessBaseResource");//默認的管理器

    private string strResourceName;
    public GlobalStringManager(string resourceName)
    {
        strResourceName = resourceName;
    }
    /// <summary>
    /// 資源文件常量
    /// </summary>
    /// <param name="name">鍵</param>
    /// <returns>值</returns>
    public string this[string name]
    {
        get
        {
            object objReturn="";
            try
            {
                objReturn = HttpContext.GetGlobalResourceObject(strResourceName, name);
            }
            catch 
            {
                objReturn = Format("Alert_NoneResourceItem",name);
            }
            return objReturn.ToString();
        }
    }

    /// <summary>
    /// 整理客戶端警告的代碼串
    /// </summary>
    /// <param name="info">警告信息的鍵</param>
    /// <returns>警告信息的值</returns>
    public string Alert(string infoKey)
    {
        StringBuilder stbAlert = new StringBuilder("alert('");
        stbAlert.Append(GlobalStringManager.Default[infoKey]);
        stbAlert.Append("');");
        return stbAlert.ToString();
    }

    /// <summary>
    /// 在資源字串的基礎上格式化
    /// </summary>
    /// <param name="name">鍵</param>
    /// <param name="value">填充變數</param>
    /// <returns>值</returns>
    public string Format(string name,params object[] value)
    {
        return string.Format(GlobalStringManager.Default[name].ToString(), value);
    }

    public static int PageSize
    {
        get
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);
        }
    }
    public static int ReportCacheTiomOut
    {
        get
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings["ReportCacheTiomOut"]);
        }
    }
    public static string PrintWatermark
    {
        get 
        {
            return System.Configuration.ConfigurationManager.AppSettings["PrintWatermark"];
        }
    }

    public static string ActualDate
    {
        get
        {
            return System.Configuration.ConfigurationManager.AppSettings["ActualDate"];
        }
    }
}
