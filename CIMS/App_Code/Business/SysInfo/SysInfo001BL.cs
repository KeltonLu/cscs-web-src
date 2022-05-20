//******************************************************************
//*  作    者：FangBao
//*  功能說明：系統管理模組
//*  創建日期：2008-11-24
//*  修改日期：2008-11-24 12:00
//*  修改記錄：
//*            □2008-11-24
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
/// SysInfo001BL 的摘要描述
/// </summary>
public class SysInfo001BL : BaseLogic
{
    #region SQL語句
    public const string SEL_ACTION_HISTORY = "select ah.*,us.UserName,replace(fn.functionname,'&nbsp;','') as functionname,pm.param_name from ACTION_HISTORY ah"
                                            + " inner join USERS us on ah.UserID=us.UserID"
                                            + " inner join [function] fn on ah.functionid=fn.functionid"
                                            + " inner join param pm on ah.Param_Code=pm.param_code and ParamType_Code='"+GlobalString.ParameterType.OprType+"' ";

    public const string SEL_PARAMTYPE = "select PARAM_CODE,PARAM_NAME from param WHERE ParamType_Code='" + GlobalString.ParameterType.OprType + "'";

    public const string SEL_USERS = "SELECT UserID,UserName FROM USERS";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public SysInfo001BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 下拉框綁定
    /// </summary>
    /// <returns></returns>
    public DataSet LoadDropData()
    {
        //執行SQL語句
        DataSet dstDrop = null;
        try
        {
            dstDrop = dao.GetList(SEL_PARAMTYPE +" "+ SEL_USERS);
            return dstDrop;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
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
        string strSortField = (sortField == "null" ? "Operate_Date" : sortField);//默認的排序欄位

        sortType = (sortField == "null" ? "desc" : sortType);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_ACTION_HISTORY);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtOperate_DateFrom"].ToString().Trim()))
            {
                stbWhere.Append(" and ah.Operate_Date>=@Operate_DateFrom");
                dirValues.Add("Operate_DateFrom", searchInput["txtOperate_DateFrom"].ToString().Trim()+" 00:00:00");
            }
            if (!StringUtil.IsEmpty(searchInput["txtOperate_DateTo"].ToString().Trim()))
            {
                stbWhere.Append(" and ah.Operate_Date<=@Operate_DateTo");
                dirValues.Add("Operate_DateTo", searchInput["txtOperate_DateTo"].ToString().Trim()+" 23:59:59");
            }
            if (!StringUtil.IsEmpty(searchInput["dropUserID"].ToString().Trim()))
            {
                stbWhere.Append(" and ah.UserID=@UserID");
                dirValues.Add("UserID", searchInput["dropUserID"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropParam_Code"].ToString().Trim()))
            {
                stbWhere.Append(" and ah.Param_Code=@Param_Code");
                dirValues.Add("Param_Code", searchInput["dropParam_Code"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dstActionHis = null;
        try
        {
            dstActionHis = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstActionHis;
    }
}
