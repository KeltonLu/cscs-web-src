//******************************************************************
//*  作    者：Ray
//*  功能說明：Report0090 Business
//*  創建日期：2008/11/26
//*  修改日期：
//*  修改記錄：
//*            
//*******************************************************************
using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using ControlLibrary;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

/// <summary>
/// Summary description for Report0090
/// </summary>
public class Report0090 : BaseLogic 
{
	public Report0090()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    const string SEL_FACTORY_PERSO = "SELECT RID, Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y'";
    const string SEL_FACTORY_NAME = "SELECT Factory_ShortName_CN From FACTORY WHERE RST = 'A' AND RID = @RID";
    const string SEL_FACTORY_CHECKNUMBER = "select count(*) as Total  from dbo.CARDTYPE_STOCKS where RST = 'A' and DateDiff(day,Stock_Date,@CheckDate)=0";
    
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    /// <summary>
    /// 返回Perso廠列表
    /// </summary>
    /// <returns>Perso廠資料表</returns>
    public DataSet GetPersoList()
    {
        DataSet dst = null;
        try
        {
            dst = dao.GetList(SEL_FACTORY_PERSO);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dst;
    }

    /// <summary>
    /// 取得Perso中文簡稱
    /// </summary>
    /// <param name="RID"></param>
    /// <returns>中文簡稱</returns>
    public string GetPersoName(int RID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", RID);
            object persoName = dao.ExecuteScalar(SEL_FACTORY_NAME, dirValues);
            return persoName.ToString();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 檢查日期是否已有日結資料
    /// </summary>
    /// <param name="CheckDate"></param>
    /// <returns>有資料返回true</returns>
    public bool GetCheckStatus(string CheckDate)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("CheckDate", CheckDate);
            object checkNum = dao.ExecuteScalar(SEL_FACTORY_CHECKNUMBER, dirValues);
            if (Convert.ToInt32(checkNum) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


}
