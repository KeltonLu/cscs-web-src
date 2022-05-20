//******************************************************************
//*  作    者：FangBao
//*  功能說明：警訊功能 
//*  創建日期：2008-11-26
//*  修改日期：2008-11-26 12:00
//*  修改記錄：
//*            □2008-11-26
//*              1.創建 鮑方
//*******************************************************************

using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
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
/// SysInfo002BL 的摘要描述
/// </summary>
public class SysInfo002BL : BaseLogic
{

    #region SQL語句
    public const string SEL_WARNING_CONFIGURATION = "select * from WARNING_CONFIGURATION WHERE Is_Show='Y'";
    public const string SEL_USERS = "select userid,username from UseRS ";
    public const string SEL_USERS_BY_RID = "select WU.USERID,UR.USERNAME,UR.EMAIL from WARNING_USER WU inner join UseRS UR on WU.USERID=UR.USERID WHERE WU.WARNING_RID=@WARNING_RID";
    //2009-6-22 modify by huangping 警訊添加排序
    //Legend 2017/11/07 調整Sql寫法, 提升效能
    public const string SEL_WARNING_INFO = "SELECT wi.*,wc.List FROM WARNING_INFO wi INNER JOIN WARNING_CONFIGURATION wc ON wi.UserID=@UserID and wc.RID=wi.Warning_RID ";

    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public SysInfo002BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }


    /// <summary>
    /// 獲取警訊信息
    /// </summary>
    /// <returns></returns>
    public DataSet getWARNING_INFO()
    {
        DataSet dst = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("UserID", ((USERS)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER]).UserID);
            //2009-6-22 modify by huangping 警訊添加排序
            dst = dao.GetList(SEL_WARNING_INFO + " and wi.Is_Show='Y' and wc.Is_Show='Y' order by wc.List", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
        }

        return dst;
    }

    /// <summary>
    /// 獲取警訊信息(用於分頁方法)
    /// </summary>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>分頁后警訓查詢資料</returns>
    /// <remarks>Legend 2018/4/2 新增</remarks>
    public DataSet getWARNING_INFO_List(string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "List" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_WARNING_INFO + " and wi.Is_Show='Y' and wc.Is_Show='Y'");

        DataSet dst = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("UserID", ((USERS)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER]).UserID);
            
            dst = dao.GetList(stbCommand.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
        }

        //返回查詢結果
        rowCount = intRowCount;
        return dst;
    }

    public void UpdataWarningInfo(string strRIDs)
    {
        try
        {
            foreach (string strRID in strRIDs.Split(','))
            {
                if (!StringUtil.IsEmpty(strRID))
                {
                    WARNING_INFO wiModel = dao.GetModel<WARNING_INFO, int>("RID", int.Parse(strRID));
                    wiModel.Is_Show = "N";
                    dao.Update<WARNING_INFO>(wiModel, "RID");
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            
        }
    }



    /// <summary>
    /// 查詢預算主記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[預算]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "RID" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_WARNING_CONFIGURATION);

        dirValues.Clear();

        //執行SQL語句
        DataSet dstcard_Budget = null;
        try
        {
            dstcard_Budget = dao.GetList(stbCommand.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstcard_Budget;
    }


    /// <summary>
    /// 獲取warning模型
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public WARNING_CONFIGURATION GetModel(string strRID)
    {
        try
        {
            return dao.GetModel<WARNING_CONFIGURATION, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 獲取用戶
    /// </summary>
    /// <returns></returns>
    public DataTable GetUsers()
    {
        try
        {
            return dao.GetList(SEL_USERS).Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 獲取用戶
    /// </summary>
    /// <returns></returns>
    public DataTable GetUsersByRID(string strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("WARNING_RID", int.Parse(strRID));
            return dao.GetList(SEL_USERS_BY_RID, dirValues).Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="wcModel"></param>
    /// <param name="mlbUser"></param>
    public void Updata(WARNING_CONFIGURATION wcModel, MoveListBox mlbUser)
    {
        try
        {
            //事務開始
            dao.OpenConnection();

            WARNING_CONFIGURATION wcModel_o = GetModel(wcModel.RID.ToString());
            wcModel_o.Mail_Show = wcModel.Mail_Show;
            wcModel_o.System_Show = wcModel.System_Show;
            dao.Update<WARNING_CONFIGURATION>(wcModel_o, "RID");

            dao.ExecuteNonQuery("delete from WARNING_USER where Warning_RID =" + wcModel.RID.ToString());

            foreach (ListItem li in mlbUser.RightListBox.Items)
            {
                WARNING_USER wuModel = new WARNING_USER();
                wuModel.Warning_RID = wcModel.RID;
                wuModel.UserID = li.Value;
                dao.Add<WARNING_USER>(wuModel, "RID");
            }

            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();

            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

}
