//******************************************************************
//*  作    者：lantaosu
//*  功能說明：晶片規格變化表 
//*  創建日期：2008-12-03
//*  修改日期：2008-12-04 18:00
//*  修改記錄：
//*            □2008-12-04
//*              1.創建 蘇斕濤
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
/// Report031BL 的摘要描述
/// </summary>
public class Report031BL
{
    #region SQL語句
    public const string SEL_WAFER_INFO = "SELECT RID, (Wafer_Name+'/'+CONVERT(varchar(10),Wafer_Capacity)+'K') AS Wafer "
                                        + "FROM WAFER_INFO "
                                        + "where RST = 'A' ";
    public const string SEL_CARD_TYPE = "SELECT RID "
                                        + "FROM CARD_TYPE "
                                        + "where RST = 'A' ";
    public const string SEL_WAFER_CARDTYPE_USELOG = "SELECT C.Name, (Wafer_Name+'/'+CONVERT(varchar(10),Wafer_Capacity)+'K') AS Wafer, CONVERT(varchar(20), U.Income_Date,111) AS Income_Date "
                                        + ",CASE year(Begin_Date) WHEN 1900 THEN '' ELSE CONVERT(varchar(20), Begin_Date,111) END AS Begin_Date "
                                        +" ,CASE year(End_Date) WHEN 1900 THEN '' ELSE CONVERT(varchar(20), End_Date,111) END AS End_Date "
                                        + "FROM WAFER_CARDTYPE_USELOG U "
                                        + "LEFT JOIN CARD_TYPE C ON C. RST = 'A'   AND  C.RID=U.CardType_RID "
                                        + "LEFT JOIN WAFER_INFO W ON W.RST='A'   AND  W.RID=U.Wafer_RID "
                                        + "WHERE U.RST='A'  ";    
    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Report031BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 初始化"晶片名稱+容量"欄位
    /// </summary>
    /// <returns></returns>
    public DataSet GetWaferList()
    {
        DataSet dstWaferList = null;

        try
        {
            dstWaferList = dao.GetList(SEL_WAFER_INFO);
            return dstWaferList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 獲取所有卡片RID
    /// </summary>
    /// <returns></returns>
    public DataSet GetCardList()
    {
        DataSet dstCardList = null;

        try
        {
            dstCardList = dao.GetList(SEL_CARD_TYPE);
            return dstCardList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢晶片規格變化表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string RID, string DateFrom, string DateTo, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Name" : sortField);//默認的排序欄位
        int i=0;

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder();        
        StringBuilder stbWhere = new StringBuilder();
        DataSet ds=null;          

        stbCommand = new StringBuilder(SEL_WAFER_CARDTYPE_USELOG);

        dirValues.Clear();

        if (((DataTable)searchInput["UctrlCardType"]).Rows.Count > 0)
        {
            string strCardType = "";
            foreach (DataRow drowCardType in ((DataTable)searchInput["UctrlCardType"]).Rows)
                strCardType += drowCardType["RID"].ToString() + ",";

            stbWhere.Append(" AND U.CardType_RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ")");
            dirValues.Add("strCardType", strCardType.Substring(0, strCardType.Length - 1));
        }
        else
        {
            dirValues.Add("strCardType", "");
        }

        if (RID != "")
        {
            stbWhere.Append(" AND U.Wafer_RID IN (" + RID + ") ");
            dirValues.Add("strWafer", RID.Substring (0,RID.Length -1));
        }
        else
        {
            dirValues.Add("strWafer", "");
        }

        if (DateFrom != "")
        {
            stbWhere.Append(" AND U.Income_Date >= @DateFrom ");
            dirValues.Add("strDateFrom", DateFrom);

        }
        else
        {
            dirValues.Add("strDateFrom", "");

        }
        if (DateTo != "")
        {
            stbWhere.Append(" AND U.Begin_Date <= @DateTo ");
            dirValues.Add("strDateTo", DateTo + " 23:59:59");        

        }
        else
        {
            dirValues.Add("strDateTo","");        

        }


        //執行SQL語句
        DataSet dst = null;
        try
        {
            //dst = dao.GetList(stbCommand.ToString() + stbWhere.ToString() + " and Wafer_Name <> 'NA' ", dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
            dst = dao.GetList("Proc_report031", dirValues, true);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        //dst.Tables[0].DefaultView.Sort = "Name, Income_date desc";
        return dst;
    }
}
