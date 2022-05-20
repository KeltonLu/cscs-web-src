//******************************************************************
//*  作    者：lantaosu
//*  功能說明：庫存成本明細查詢管理邏輯
//*  創建日期：2008-12-10
//*  修改日期：2008-12-11 12:00
//*  修改記錄：
//*            □2008-12-11
//*              1.創建 蘇斕濤
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
/// Finance004_3BL 的摘要描述
/// </summary>
public class Finance004_3BL
{
    #region SQL語句
    public const string SEL_COO_PERSO_FACTORY = "SELECT RID, Factory_ShortName_CN "
                                        + "FROM FACTORY "
                                        + "WHERE  RST = 'A'  AND Is_Perso='Y' AND Is_Cooperate='Y'";
    public const string SEL_STOCK_COST_YEAR = "SELECT distinct  LEFT(Date_Time,4) AS Year "
                                        + "FROM STOCKS_COST "
                                        + "WHERE  RST = 'A' ORDER BY Year DESC";
    public const string SEL_GROUP_BY_USE = "SELECT RID, Group_Name "
                                        + "FROM CARD_GROUP  "
                                        + "WHERE  RST = 'A'  AND Param_Code = @Param_Code ";    
    public const string SEL_ALL_USE = "SELECT Param_Code, Param_Name "
                                       + "FROM PARAM "
                                       + "WHERE  RST = 'A'  AND ParamType_Code='use' ";
    public const string SEL_LAST_DATE_TO = "SELECT  MAX(Date_To) "
                                        + "FROM STOCKS_COST "
                                        + "WHERE RST='A' ";
    public const string SEL_STOCK_DATE = "SELECT distinct Stock_Date "
                                        + "FROM CARDTYPE_STOCKS "
                                        + "WHERE  RST = 'A'  AND (Stock_Date BETWEEN @DateFrom  AND @DateTo) "
                                        + "ORDER BY Stock_Date DESC ";

    public const string SEL_FIRST_WORK_DAY = "SELECT MIN(Date_Time) "
                                        + "FROM WORK_DATE  "
                                        + "WHERE RST = 'A'   AND  Is_WorkDay='Y' AND (Date_Time BETWEEN @DateFrom  AND @DateTo)  ";
    public const string SEL_NEXT_WORK_DAY = "SELECT MIN(Date_Time) "
                                        + "FROM WORK_DATE  "
                                        + "WHERE RST = 'A'   AND  Is_WorkDay='Y' AND Date_Time>@DateFrom ";

    public const string SEL_LAST_WORK_DAY = "SELECT MAX(Date_Time) "
                                        + "FROM WORK_DATE  "
                                        + "WHERE RST = 'A'   AND  Is_WorkDay='Y' AND Date_Time<@DateFrom ";
    public const string SEL_NUMBER = "SELECT  A.Check_Date, SUM(A.Number) AS Number, A.Income_Date AS Income_Date, A.Unit_Price, B.CardType_Move_RID, B.Operate_RID, B.Operate_Type, B.Factory_RID, B.CardType_RID, B.Factory_ShortName_CN, B.Name , A.Uselog_Rid "
                          + " ,'' as log_rid "
                          + " FROM View_WAFER_USELOG_ROLLBACK A";




    // Legend 2017/11/16 查詢清單, 版面簡稱前增加卡片編號欄位CARD TYPE(3碼) AFFINITY (4碼) PHOTO(4碼)
    public const string SEL_MAX_RID = " SELECT  uselog_rid,MAX(W.Check_Date) AS Check_Date, W.CardType_Move_RID, W.Operate_RID, W.Operate_Type, W.Factory_RID, W.CardType_RID, F.Factory_ShortName_CN, C.[TYPE]+'-'+C.AFFINITY + '-' + C.PHOTO + '-' +  C.Name AS Name "
                          + " FROM  View_WAFER_USELOG_ROLLBACK AS W LEFT OUTER JOIN "
                          + " FACTORY AS F ON F.RST = 'A' AND F.RID = W.Factory_RID LEFT OUTER JOIN "
                          + " CARD_TYPE AS C ON C.RST = 'A' AND C.RID = W.CardType_RID LEFT OUTER JOIN "
                          + " GROUP_CARD_TYPE AS G ON G.RST = 'A' AND G.CardType_RID = C.RID "
                          //+ " WHERE (W.RST = 'A') AND (W.Check_Date>@DateFrom AND W.Check_Date<=@DateTo)  ";
                          + " WHERE (W.RST = 'A') AND (W.Check_Date>=@DateFrom AND W.Check_Date<=@DateTo)  ";
   
        //"SELECT distinct W.Income_Date, W.Number, W.CardType_Move_RID, W.Operate_RID, W.Operate_Type,W.Factory_RID, W.CardType_RID, F.Factory_ShortName_CN,C.Name "
        //                                + "FROM WAFER_USELOG_ROLLBACK  W   "
        //                                + "LEFT JOIN FACTORY F ON F.RST='A'  AND F.RID=W.Factory_RID  "
        //                                + "LEFT JOIN CARD_TYPE C ON C.RST='A' AND C.RID=W.CardType_RID  "
        //                                + "LEFT JOIN GROUP_CARD_TYPE G ON G.RST='A'  AND  G.CardType_RID=C.RID  "
        //                                + "WHERE  W.RST = 'A'   AND (W.Income_Date BETWEEN @DateFrom  AND @DateTo)  ";


    //public const string SEL_USABLE_NUMBER = "SELECT  W.RID AS RID_Initial, W.Usable_Number AS Usable_Number_Initial "
    //                                   + " FROM   WAFER_USELOG_ROLLBACK AS W "
    //                                   + " WHERE  W.Check_Date = @Check_Date  "
    //                                   + " AND W.RST='A'  AND   W.Factory_RID=@Factory_RID AND W.CardType_RID=@CardType_RID  "
    //                                   + " AND  W.Operate_RID=@Operate_RID  AND  W.Operate_Type=@Operate_Type  AND  W.CardType_Move_RID=@CardType_Move_RID ";
    public const string SEL_USABLE_NUMBER = "SELECT  W.RID AS RID_Initial, W.Usable_Number AS Usable_Number_Initial "
                                        + " FROM   View_WAFER_USELOG_ROLLBACK AS W "
                                        + " WHERE  W.Check_Date = @Check_Date  "
                                        + " AND W.RST='A'  AND   W.Factory_RID=@Factory_RID AND W.CardType_RID=@CardType_RID  "
                                        + " AND  W.Operate_RID=@Operate_RID  AND  W.Operate_Type=@Operate_Type  AND  W.CardType_Move_RID=@CardType_Move_RID and Uselog_Rid = @Uselog_Rid";
    public const string SEL_USABLE_NUMBER_MOVE = "SELECT  sum(W.Usable_Number) AS Usable_Number_Initial "
                                         + " FROM   View_WAFER_USELOG_ROLLBACK AS W "
                                         + " WHERE  W.Check_Date = @Check_Date  "
                                         + " AND W.RST='A'  AND   W.Factory_RID=@Factory_RID AND W.CardType_RID=@CardType_RID  ";
                                         
        //"SELECT  W.RID AS RID_Initial, W.Usable_Number AS Usable_Number_Initial "
        //                               + " FROM         WAFER_USELOG_ROLLBACK AS W "
        //                               + " WHERE     (Check_Date IN "
        //                               + " (SELECT     MIN(Check_Date) AS min_Check_Date "
        //                               + " FROM          WAFER_USELOG_ROLLBACK "
        //                               + " WHERE      (Check_Date >= @Check_Date) AND (RST = 'A'))) "
        //                               + "AND W.RST='A'  AND   W.Factory_RID=@Factory_RID AND W.CardType_RID=@CardType_RID  "
        //                               + "AND  W.Operate_RID=@Operate_RID  AND  W.Operate_Type=@Operate_Type  AND  W.CardType_Move_RID=@CardType_Move_RID ";

        //"SELECT  W.RID AS RID_Initial, W.Usable_Number AS Usable_Number_Initial " //, W.CardType_Move_RID AS CardType_Move_RID_Initial, W.Operate_RID AS Operate_RID_Initial, W.Operate_Type AS Operate_Type_Initial  "
        //                                + "FROM WAFER_USELOG_ROLLBACK  W "
        //                                + "WHERE W.RST='A'  AND   W.Factory_RID=@Factory_RID AND W.CardType_RID=@CardType_RID  AND W.Check_Date>=@Check_Date "
        //                                + "AND  W.Operate_RID=@Operate_RID  AND  W.Operate_Type=@Operate_Type  AND  W.CardType_Move_RID=@CardType_Move_RID "
        //                                + "ORDER BY W.RID  ";

    //public const string SEL_STOCK_UNIT_PRICE = "SELECT   S.Stock_RID,  S.Is_AskFinance, S.Perso_Factory_RID, S.Space_Short_RID,  "
    //                                    + " case  when SAP.Ask_Date is null then 0 else D.Real_Ask_Number end  as Real_Ask_Number, D.Unit_Price_No, D.Unit_Price, D.SAP_RID, SAP.Ask_Date,"                                 
    //                                    + " O.Unit_Price AS Unit_Price_Order  "
    //                                    + "FROM DEPOSITORY_STOCK S   "
    //                                    + "LEFT JOIN CARD_TYPE_SAP_DETAIL D ON  D.RST = 'A'  AND  D.Operate_RID = S.RID AND D.Operate_Type = '1'  "
    //                                    + "LEFT JOIN CARD_TYPE_SAP SAP ON SAP.RST='A' AND SAP.RID=D.SAP_RID  and SAP.Ask_Date < @Ask_Date  "
    //                                    + "LEFT JOIN ORDER_FORM_DETAIL O ON  O.RST = 'A' AND O.OrderForm_Detail_RID=S.OrderForm_Detail_RID   "
    //                                    + "WHERE S.RST = 'A'  AND S.RID=@RID "
    //                                    + "ORDER BY SAP.Ask_Date DESC";


    //public const string SEL_RESTOCK_UNIT_PRICE = "SELECT   R.Stock_RID,  R.Is_AskFinance, "
    //                                    + " case  when SAP.Ask_Date is null then 0 else D.Real_Ask_Number end  as Real_Ask_Number, D.Unit_Price_No, D.Unit_Price, D.SAP_RID, SAP.Ask_Date,"
    //                                    + " O.Unit_Price AS Unit_Price_Order  "
    //                                    + "FROM DEPOSITORY_RESTOCK R   "
    //                                    + "LEFT JOIN CARD_TYPE_SAP_DETAIL D ON D.RST = 'A'  AND  D.Operate_RID = R.RID AND D.Operate_Type = '2'    "
    //                                    + "LEFT JOIN CARD_TYPE_SAP SAP ON SAP.RST='A' AND SAP.RID=D.SAP_RID and SAP.Ask_Date < @Ask_Date "
    //                                    + "LEFT JOIN ORDER_FORM_DETAIL O ON  O.RST = 'A'   AND O.OrderForm_Detail_RID=R.OrderForm_Detail_RID   "
    //                                    + "WHERE R.RST = 'A'  AND R.RID=@RID  "
    //                                    + "ORDER BY SAP.Ask_Date  DESC";

    public const string SEL_STOCK_UNIT_PRICE = "SELECT   S.Stock_RID,  S.Is_AskFinance, S.Perso_Factory_RID, S.Space_Short_RID,  "
                                     + " case  when SAP.Ask_Date is null then 0 else D.Real_Ask_Number end  as Real_Ask_Number, D.Unit_Price_No, D.Unit_Price, D.SAP_RID, SAP.Ask_Date,  "
                                     + "O.Unit_Price AS Unit_Price_Order  "
                                     + "FROM DEPOSITORY_STOCK S   "
                                     + "LEFT JOIN CARD_TYPE_SAP_DETAIL D ON  D.RST = 'A'  AND  D.Operate_RID = S.RID AND D.Operate_Type = '1'  "
                                     + "LEFT JOIN CARD_TYPE_SAP SAP ON SAP.RST='A' AND SAP.RID=D.SAP_RID and SAP.Ask_Date < @Ask_Date "
                                     + "LEFT JOIN ORDER_FORM_DETAIL O ON  O.RST = 'A'   AND O.OrderForm_Detail_RID=S.OrderForm_Detail_RID   "
                                     + "WHERE S.RST = 'A'  AND S.RID=@RID "
                                     + "ORDER BY SAP.Ask_Date DESC, D.RID DESC";

    public const string SEL_RESTOCK_UNIT_PRICE = "SELECT   R.Stock_RID,  R.Is_AskFinance, "
                                        + "case  when SAP.Ask_Date is null then 0 else D.Real_Ask_Number end  as Real_Ask_Number, D.Unit_Price_No, D.Unit_Price, D.SAP_RID,  SAP.Ask_Date,   "
                                        + "O.Unit_Price AS Unit_Price_Order   "
                                        + "FROM DEPOSITORY_RESTOCK R   "
                                        + "LEFT JOIN CARD_TYPE_SAP_DETAIL D ON D.RST = 'A'  AND  D.Operate_RID = R.RID AND D.Operate_Type = '2'    "
                                        + "LEFT JOIN CARD_TYPE_SAP SAP ON SAP.RST='A' AND SAP.RID=D.SAP_RID and SAP.Ask_Date < @Ask_Date "
                                        + "LEFT JOIN ORDER_FORM_DETAIL O ON  O.RST = 'A'   AND O.OrderForm_Detail_RID=R.OrderForm_Detail_RID   "
                                        + "WHERE R.RST = 'A'  AND R.RID=@RID  "
                                        + "ORDER BY SAP.Ask_Date DESC, D.RID DESC";


    public const string SEL_EXPRESSION = "SELECT Type_RID,Operate  "
                                        + "FROM EXPRESSIONS_DEFINE "
                                        + "WHERE RST='A' AND Expressions_RID=@Expressions_RID ";
    public const string SEL_FACTORY_MOVE_FROM = "SELECT  F.RID,F.Factory_ShortName_CN, M.CardType_RID "
                                       + "FROM CARDTYPE_STOCKS_MOVE M "
                                       + "LEFT JOIN FACTORY F ON F.RST='A'  AND F.RID=M.From_Factory_RID   "
                                       + "WHERE M.RST='A'  AND M.RID=@CardType_Move_RID  ";
    public const string SEL_CARD_NUMBER = "SELECT  SUM(Number) AS Sum, Perso_Factory_RID,  CardType_RID, Status_Name,Status_Code,Status_RID "
                                        + "FROM (   "
                                        + "SELECT S1.Date_Time,  S1.Number, S1.Perso_Factory_RID, C1.RID AS CardType_RID,  M.Type_Name AS Status_Name,CS1.Status_Code,CS1.RID AS Status_RID   "
                                        + "FROM SUBTOTAL_IMPORT S1    "
                                        + "LEFT JOIN CARD_TYPE C1 ON C1.RST='A' AND  C1.TYPE= S1.TYPE AND  C1.AFFINITY=S1.AFFINITY  AND C1.PHOTO=S1.PHOTO  "
                                        + "LEFT JOIN GROUP_CARD_TYPE G1 ON G1.RST='A'  AND G1.CardType_RID=C1.RID   "
                                        + "LEFT JOIN MAKE_CARD_TYPE M ON M.RST='A' AND M.RID=S1.MakeCardType_RID AND M.Is_Import='Y' "
                                        + "LEFT JOIN CARDTYPE_STATUS CS1 ON CS1.RST='A' AND CS1.Status_Name=M.Type_Name    "
                                        + "WHERE S1.RST='A'  AND (S1.Date_Time BETWEEN @DateFrom  AND @DateTo) AND M.Type_Name IN ('3D','DA','PM','RN')    ";

   
    public const string SEL_CARD_NUMBER1 = "UNION all "
                                        + "SELECT F.Date_Time,  F.Number, F.Perso_Factory_RID, C3.RID  AS CardType_RID,  CS.Status_Name, CS.Status_Code,CS.RID AS Status_RID   "
                                        + "FROM FACTORY_CHANGE_IMPORT F   "
                                        + "LEFT JOIN CARD_TYPE C3 ON C3.RST='A'   AND C3.TYPE=F.TYPE AND  C3.AFFINITY=F.AFFINITY  AND C3.PHOTO=F.PHOTO   "
                                        + "LEFT JOIN GROUP_CARD_TYPE G3 ON G3.RST='A'  AND G3.CardType_RID=C3.RID   "
                                        + "LEFT JOIN CARDTYPE_STATUS CS ON CS.RST='A' AND CS.Status_Code=F.Status_RID "
                                        + "WHERE F.RST='A'  AND (F.Date_Time BETWEEN @DateFrom  AND @DateTo) AND   CS.Status_Name IN('缺卡','未製卡','補製卡','樣卡','製損卡','感應不良','排卡','銷毀','調整')    ";
    public const string SEL_CARD_NUMBER2 = " ) AS  table1 "
                                        + "GROUP BY Perso_Factory_RID, CardType_RID, Status_Name,Status_Code,Status_RID ";


    // edit by Ian Huang start
    public const string SEL_LAST_CHECK_DATE = "SELECT  MAX(W.Check_Date) FROM  View_WAFER_USELOG_ROLLBACK  W "
                                        + "WHERE W.RST='A'  AND   W.Factory_RID=@Factory_RID  AND W.CardType_RID=@CardType_RID  AND  (W.Check_Date BETWEEN @DateFrom AND @DateTo)  "
                                        + "AND  W.Operate_RID=@Operate_RID  AND  W.Operate_Type=@Operate_Type  AND  W.CardType_Move_RID=@CardType_Move_RID ";

    // edit by Ian Huang end
    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Finance004_3BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢所有已建立的合作Perso廠中文簡稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetCooperatePersoList()
    {
        DataSet dstCooperatePersoList = null;

        try
        {
            dstCooperatePersoList = dao.GetList(SEL_COO_PERSO_FACTORY);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCooperatePersoList;
    }
    

    /// <summary>
    /// 查詢參數檔中的所有用途
    /// </summary>    
    /// <returns></returns>
    public DataSet GetUseList()
    {
        DataSet dstUseList = null;

        try
        {
            dstUseList = dao.GetList(SEL_ALL_USE);
            return dstUseList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢該用途的所有卡穜群組
    /// </summary>
    /// <param name="Param_Code"></param>
    /// <returns></returns>
    public DataSet GetGroupList(string Param_Code)
    {
        DataSet dstGroupList = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Param_Code", Param_Code);
            dstGroupList = dao.GetList(SEL_GROUP_BY_USE, dirValues);
            return dstGroupList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得最近一次執行5.4.2的帳務迄日
    /// </summary>
    /// <returns></returns>
    public string GetLastDate(string strDateFrom)
    {
        DataSet dstLastDate = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("date_to", strDateFrom.Replace("/",""));
            dstLastDate = dao.GetList("select Convert(datetime,StartDate,111) from dbo.BillCycle where convert(int,startdate) <=@date_to and convert(int,enddate)>= @date_to", dirValues);
            if (dstLastDate.Tables[0].Rows.Count == 0 || dstLastDate.Tables[0].Rows[0][0].ToString().Trim() == "")
            {
                dstLastDate = dao.GetList("select dateadd(d,1,Convert(datetime,max(enddate),111)) from BillCycle");
                return Convert.ToDateTime(dstLastDate.Tables[0].Rows[0][0].ToString()).ToString("yyyy/MM/dd");
            }
            else
            {
                return Convert.ToDateTime(dstLastDate.Tables[0].Rows[0][0].ToString()).ToString("yyyy/MM/dd");
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    

    /// <summary>
    /// 查詢進貨作業信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List(string RID, string DateFrom, string DateTo, string Group_RID, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Income_Date" : sortField);//默認的排序欄位
        //DateFrom = "2008/7/1";

        //准備SQL語句
        StringBuilder stbWhere = new StringBuilder();

        if (RID != "")
        {
            stbWhere.Append(" AND  W.Factory_RID IN (" + RID + ") ");
        }
        if (Group_RID != "")
        {
            stbWhere.Append(" AND  G.Group_RID =@Group_RID ");
        }

        // Legend 2017/11/16 查詢清單, 版面簡稱前增加卡片編號欄位CARD TYPE(3碼) AFFINITY (4碼) PHOTO(4碼)
        string strGroupBy = " GROUP BY  uselog_rid,W.CardType_Move_RID, W.Operate_RID, W.Operate_Type, W.Factory_RID, W.CardType_RID, F.Factory_ShortName_CN, C.[TYPE], C.AFFINITY, C.PHOTO, C.Name ";
        string strSEL_MAX_RID = SEL_MAX_RID + stbWhere + strGroupBy;
        string strSQL = SEL_NUMBER + ", (" + strSEL_MAX_RID + ") AS B WHERE  a.uselog_rid=b.uselog_rid and A.Check_Date = B.Check_Date "
                + " and A.CardType_Move_RID=B.CardType_Move_RID and A.Operate_RID=B.Operate_RID and A.Operate_Type=B.Operate_Type "
                + " and A.Factory_RID=B.Factory_RID and A.CardType_RID=B.CardType_RID "
                + " GROUP BY A.Check_Date, A.Income_Date, A.Unit_Price, B.CardType_Move_RID, B.Operate_RID, B.Operate_Type, B.Factory_RID, B.CardType_RID, B.Factory_ShortName_CN, B.Name, A.uselog_rid"
                + " order by A.Income_Date, A.uselog_rid";

        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", Group_RID);

        //執行SQL語句
        DataSet dst = null;
        try
        {
            dst = dao.GetList(strSQL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dst;
    }

    /// <summary>
    /// 取得日期區間内的第一個工作日，作爲期初的日結日期
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public string GetFirstWorkDay(string DateFrom, string DateTo)
    {
        DataSet dstLastWorkDay = null;

        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);

        try
        {
            dstLastWorkDay = dao.GetList(SEL_FIRST_WORK_DAY, dirValues);
            if (dstLastWorkDay.Tables[0].Rows.Count == 0 || dstLastWorkDay.Tables[0].Rows[0][0].ToString().Trim() == "")
                return "1900/01/01";

            return Convert.ToDateTime(dstLastWorkDay.Tables[0].Rows[0][0].ToString()).ToString("yyyy/MM/dd");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得這個日期區間後的第一個工作日
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <returns></returns>
    public string GetNextWorkDay(string strDate)
    {
        DataSet dstLastWorkDay = null;

        dirValues.Clear();
        dirValues.Add("DateFrom", strDate);

        try
        {
            dstLastWorkDay = dao.GetList(SEL_NEXT_WORK_DAY, dirValues);
            if (dstLastWorkDay.Tables[0].Rows.Count == 0 || dstLastWorkDay.Tables[0].Rows[0][0].ToString().Trim() == "")
                return "1900/01/01";

            return Convert.ToDateTime(dstLastWorkDay.Tables[0].Rows[0][0].ToString()).ToString("yyyy/MM/dd");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得日期區間起的前一個工作日，作爲期初的日結日期
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <returns></returns>
    //public string GetLastWorkDay(string DateFrom)
    //{
    //    DataSet dstLastWorkDay = null;

    //    dirValues.Clear();
    //    dirValues.Add("DateFrom", DateFrom);

    //    try
    //    {
    //        dstLastWorkDay = dao.GetList(SEL_LAST_WORK_DAY, dirValues);
    //        if (dstLastWorkDay.Tables[0].Rows.Count == 0 || dstLastWorkDay.Tables[0].Rows[0][0].ToString().Trim() == "")
    //            return "1900/01/01";
    //        return Convert.ToDateTime(dstLastWorkDay.Tables[0].Rows[0][0].ToString()).ToString("yyyy/MM/dd");
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}

    /// <summary>
    /// 取得期末日結日期
    /// </summary>
    /// <param name="strOperate_RID"></param>
    /// <param name="strOperate_Type"></param>
    /// <param name="strCardType_Move_RID"></param>
    /// <param name="Factory_RID"></param>
    /// <param name="CardType_RID"></param>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public string GetLastCheckDate(string strOperate_RID, string strOperate_Type, string strCardType_Move_RID, string Factory_RID, string CardType_RID, string DateFrom, string DateTo)
    {
        DataSet dstLastCheckDate = null;

        dirValues.Clear();
        dirValues.Add("Factory_RID", Factory_RID);
        dirValues.Add("CardType_RID", CardType_RID);
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Operate_RID", strOperate_RID);
        dirValues.Add("Operate_Type", strOperate_Type);
        dirValues.Add("CardType_Move_RID", strCardType_Move_RID);

        try
        {
            dstLastCheckDate = dao.GetList(SEL_LAST_CHECK_DATE, dirValues);
            if (dstLastCheckDate.Tables[0].Rows.Count == 0 || dstLastCheckDate.Tables[0].Rows[0][0].ToString().Trim() == "")
                return "1900/01/01";

            return Convert.ToDateTime(dstLastCheckDate.Tables[0].Rows[0][0].ToString()).ToString("yyyy/MM/dd");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得晶片規格變化回滾檔
    /// </summary>
    /// <param name="Factory_RID"></param>
    /// <param name="CardType_RID"></param>
    /// <param name="Check_Date"></param>
    /// <returns></returns>
    public DataSet GetUsableNumber(string strOperate_RID, string strOperate_Type, string strCardType_Move_RID, string Factory_RID, string CardType_RID, string Check_Date, string strUselog_Rid)
    {
        DataSet dstUsableNumber = null;

        dirValues.Clear();
        dirValues.Add("Factory_RID", Factory_RID);
        dirValues.Add("CardType_RID", CardType_RID);
        dirValues.Add("Check_Date", Check_Date);
        dirValues.Add("Operate_RID", strOperate_RID);
        dirValues.Add("Operate_Type", strOperate_Type);
        dirValues.Add("Uselog_Rid", strUselog_Rid);

        dirValues.Add("CardType_Move_RID", strCardType_Move_RID);

        try
        {
            dstUsableNumber = dao.GetList(SEL_USABLE_NUMBER, dirValues);
            return dstUsableNumber;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得單價的結果集
    /// </summary>
    /// <param name="Operate_RID"></param>
    /// <param name="Operate_Type"></param>
    /// <returns></returns>
    public DataSet GetUnitPrice(string Operate_RID, string Operate_Type, string strCheckDate)
    {
        DataSet dstUnitPrice = null;
        string SQL = "";

        if (Operate_Type == "1")
            SQL = SEL_STOCK_UNIT_PRICE;
        else if (Operate_Type == "2")
            SQL = SEL_RESTOCK_UNIT_PRICE;

        dirValues.Clear();
        dirValues.Add("RID", Operate_RID);
        dirValues.Add("Ask_Date", strCheckDate);

        try
        {
            dstUnitPrice = dao.GetList(SQL, dirValues);
            return dstUnitPrice;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 從公式定義檔中分別取得1.製成卡數;2.消耗卡數;3.耗損卡數;4.入(出)庫;5.特殊卡數 的計算公式。
    /// </summary>
    /// <param name="Expressions_RID"></param>
    /// <returns></returns>
    public DataSet GetExpression(string Expressions_RID)
    {
        DataSet dstExpression = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Expressions_RID", Expressions_RID);
            dstExpression = dao.GetList(SEL_EXPRESSION, dirValues);
            return dstExpression;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得廠商簡稱
    /// </summary>
    /// <param name="CardType_Move_RID"></param>
    /// <returns></returns>
    public DataSet GetFactoryMoveFrom(string CardType_Move_RID)
    {
        DataSet dstFactoryMoveFrom = null;

        dirValues.Clear();
        dirValues.Add("CardType_Move_RID", CardType_Move_RID);

        try
        {
            dstFactoryMoveFrom = dao.GetList(SEL_FACTORY_MOVE_FROM, dirValues);
            return dstFactoryMoveFrom;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得各種狀況('3D','DA','PM','RN','缺卡','未製卡','補製卡','樣卡','製損卡','感應不良','排卡','銷毀','調整')的卡種的數量
    /// </summary>
    /// <param name="Date_Time"></param>
    /// <param name="Perso_Factory_RID"></param>
    /// <param name="RID"></param>
    /// <returns></returns>
    public DataSet GetCardNumber(string RID, string DateFrom, string DateTo, string Group_RID)
    {
        DataSet dstCardNumber = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_CARD_NUMBER);
            StringBuilder stbCommand1 = new StringBuilder(SEL_CARD_NUMBER1);
            StringBuilder stbCommand2 = new StringBuilder(SEL_CARD_NUMBER2);
            StringBuilder stbWhere = new StringBuilder();
            StringBuilder stbWhere1 = new StringBuilder();

            if (RID != "")
            {
                stbWhere.Append(" AND  S1.Perso_Factory_RID IN (" + RID + ") ");
                stbWhere1.Append(" AND  F.Perso_Factory_RID IN (" + RID + ") ");
            }
            if (Group_RID != "all")
            {
                stbWhere.Append(" AND  G1.Group_RID =@Group_RID ");
                stbWhere1.Append(" AND  G3.Group_RID =@Group_RID ");
            }

            dirValues.Clear();
            dirValues.Add("Group_RID", Group_RID);
            dirValues.Add("DateFrom", DateFrom);
            dirValues.Add("DateTo", DateTo);
            dstCardNumber = dao.GetList(stbCommand.ToString() + stbWhere.ToString() + stbCommand1.ToString() + stbWhere1.ToString() + stbCommand2.ToString(), dirValues);
            return dstCardNumber;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 按照先進先出法，依總調整卡數、總銷毀卡數、總耗損卡數、總製成卡數、移轉出卡數 的順序，扣減列出的期初庫存數+進貨數+移轉入數量。
    /// </summary>
    /// <param name="delNumber"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public int GetCardCount(int delNumber, DataRow dr, string Card)
    {
        int CardCount;
        if (delNumber > Convert.ToInt32(dr[Card].ToString()))
        {
            CardCount = Convert.ToInt32(dr[Card].ToString());
            dr[Card] = "0";
        }
        else
        {
            CardCount = delNumber;
            dr[Card] = (Convert.ToInt32(dr[Card].ToString()) - delNumber);
        }
        return CardCount;
    }    

    /// <summary>
    /// 添加報表資料到資料庫中
    /// </summary>
    /// <param name="dt"></param>
    public void AddReport(DataTable dt,string time)
    {
        RPT_Finance004_3 RPT = new RPT_Finance004_3();
        try
        {
            //事務開始
            dao.OpenConnection();
            dao.ExecuteNonQuery("Delete From RPT_Finance004_3 where TimeMark<'" + DateTime.Now.ToString("yyyyMMdd000000") + "'");
            int intID = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dt.Columns.Count == 21)
                {
                    RPT.Name = dr["版面簡稱"].ToString();
                    RPT.Factory = dr["PERSO 廠"].ToString();
                    RPT.BeginNumber = Convert.ToInt32(dr["期初庫存數"].ToString());
                    if (dr[3].ToString().Trim() != "")
                        RPT.BeginUnitPrice = dr[3].ToString();
                    else
                        RPT.BeginUnitPrice = "";
                    RPT.InNumber = Convert.ToInt32(dr["進貨數"].ToString());
                    RPT.MoveIn = Convert.ToInt32(dr["移轉入"].ToString());
                    RPT.MoveOut = Convert.ToInt32(dr["移轉出"].ToString());
                    RPT.UseOutNumber = Convert.ToInt32(dr["消耗卡數"].ToString());
                    RPT.DestoryNumber = Convert.ToInt32(dr["銷毀卡數"].ToString());
                    RPT.ChangeNumber = Convert.ToInt32(dr["調整卡數"].ToString());
                    RPT.EndNumber = Convert.ToInt32(dr["期末庫存數"].ToString());
                    if (dr[11].ToString().Trim() != "")
                        RPT.UnitPrice = dr[11].ToString();
                    else
                        RPT.UnitPrice = "";
                    RPT.BeginPrice = Convert.ToDecimal(dr[12].ToString());
                    RPT.InPrice = Convert.ToDecimal(dr["進貨金額"].ToString());
                    RPT.MoveInPrice = Convert.ToDecimal(dr["移轉入金額"].ToString());
                    RPT.MoveOutPrice = Convert.ToDecimal(dr["移轉出金額"].ToString());
                    RPT.UseOutPrice = Convert.ToDecimal(dr["消耗卡金額"].ToString());
                    RPT.DestoryPrice = Convert.ToDecimal(dr["銷毀卡金額"].ToString());
                    RPT.ChangePrice = Convert.ToDecimal(dr["調整卡金額"].ToString());
                    RPT.ChangeUintPrice = Convert.ToDecimal(dr["單價調整金額"].ToString());
                    RPT.EndPrice = Convert.ToDecimal(dr[20].ToString());
                    RPT.TimeMark = time;
                    RPT.Id = intID;                    
                    intID++;
                }
                else
                {
                    RPT.Name = dr["版面簡稱"].ToString();
                    RPT.Factory = dr["PERSO 廠"].ToString();
                    RPT.BeginNumber = Convert.ToInt32(dr["期初庫存數"].ToString());
                    if (dr[3].ToString().Trim() != "")
                        RPT.BeginUnitPrice = dr[3].ToString();
                    else
                        RPT.BeginUnitPrice = "";
                    RPT.InNumber = Convert.ToInt32(dr["進貨數"].ToString());
                    RPT.MoveIn = Convert.ToInt32(dr["移轉入"].ToString());
                    RPT.MoveOut = Convert.ToInt32(dr["移轉出"].ToString());
                    RPT.S_Number = Convert.ToInt32(dr["製成卡數"].ToString());
                    RPT.F_Number = Convert.ToInt32(dr["耗損卡數"].ToString());
                    RPT.DestoryNumber = Convert.ToInt32(dr["銷毀卡數"].ToString());
                    RPT.ChangeNumber = Convert.ToInt32(dr["調整卡數"].ToString());
                    RPT.EndNumber = Convert.ToInt32(dr["期末庫存數"].ToString());
                    if (dr[12].ToString().Trim() != "")
                        RPT.UnitPrice = dr[12].ToString();
                    else
                        RPT.UnitPrice = "";
                    RPT.BeginPrice = Convert.ToDecimal(dr[13].ToString());
                    RPT.InPrice = Convert.ToDecimal(dr["進貨金額"].ToString());
                    RPT.MoveInPrice = Convert.ToDecimal(dr["移轉入金額"].ToString());
                    RPT.MoveOutPrice = Convert.ToDecimal(dr["移轉出金額"].ToString());
                    RPT.S_Price = Convert.ToDecimal(dr["製成卡金額"].ToString());
                    RPT.F_Price = Convert.ToDecimal(dr["耗損卡金額"].ToString());
                    RPT.DestoryPrice = Convert.ToDecimal(dr["銷毀卡金額"].ToString());
                    RPT.ChangePrice = Convert.ToDecimal(dr["調整卡金額"].ToString());
                    RPT.ChangeUintPrice = Convert.ToDecimal(dr["單價調整金額"].ToString());
                    RPT.EndPrice = Convert.ToDecimal(dr[22].ToString());
                    RPT.TimeMark = time;
                    RPT.Id = intID;
                    intID++;
                }


                dao.Add<RPT_Finance004_3>(RPT);
            }


            //事務提交
            dao.Commit();

        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }

    }
   
    /// <summary>
    /// 上月期末庫存金額
    /// </summary>
    /// <param name="Date_Time"></param>
    /// <returns></returns>
    public string GetLastEndPrice(string strDateFrom, string strUselog_Rid, string UseRid, string GroupRid)
    {
        string SEL_LastEndPrice = "select EndPrice from dbo.RPT_Finance004_2 where Name like @Name and Factory=@Factory AND BeginUnitPrice=@BeginUnitPrice AND UnitPrice=@UnitPrice";
        string Time;
        DataSet dstLastEndPrice = null;
        Time = GetLastBillDate(strDateFrom);
        dirValues.Clear();
        dirValues.Add("Name", '%' + strUselog_Rid + '%');
        dirValues.Add("Factory", Time);
        dirValues.Add("BeginUnitPrice", UseRid);
        dirValues.Add("UnitPrice", GroupRid);

        try
        {
            dstLastEndPrice = dao.GetList(SEL_LastEndPrice, dirValues);
            if (dstLastEndPrice.Tables[0].Rows.Count > 0)
                return dstLastEndPrice.Tables[0].Rows[0][0].ToString();
            else
                return "0.00";
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 取得最近一次的帳務周期
    /// </summary>
    /// <returns></returns>
    public string GetLastBillDate(string strDateFrom)
    {
        DataSet dstLastDate = null;
        string Year;
        string Month;
        string YearMonth = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("date_to", strDateFrom.Replace("/", ""));
            dstLastDate = dao.GetList("select CycleSeq from dbo.BillCycle where convert(int,startdate) <=@date_to and convert(int,enddate)>= @date_to", dirValues);
            if (dstLastDate.Tables[0].Rows.Count != 0 && dstLastDate.Tables[0].Rows[0][0].ToString().Trim() != "")
            {
                Year = dstLastDate.Tables[0].Rows[0][0].ToString().Substring(0,4);
                Month = dstLastDate.Tables[0].Rows[0][0].ToString().Substring(4, 2);
                int month = (Convert.ToInt32(Month) - 1);
                if (month == 0)
                {
                    YearMonth = Convert.ToString(Convert.ToInt32(Year) - 1) + "12";
                }
                else
                {
                    YearMonth = Year + month.ToString().PadLeft(2, '0');
                }
            }
            return YearMonth;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 取得晶片規格變化回滾檔
    /// </summary>
    /// <param name="Factory_RID"></param>
    /// <param name="CardType_RID"></param>
    /// <param name="Check_Date"></param>
    /// <returns></returns>
    public DataSet GetUsableNumberMove(string Factory_RID, string CardType_RID, string Check_Date, string strUselog_Rid)
    {
        DataSet dstUsableNumber = null;

        dirValues.Clear();
        dirValues.Add("Factory_RID", Factory_RID);
        dirValues.Add("CardType_RID", CardType_RID);
        dirValues.Add("Check_Date", Check_Date);

        try
        {
            dstUsableNumber = dao.GetList(SEL_USABLE_NUMBER_MOVE + "  and Uselog_Rid in (" + strUselog_Rid + ")", dirValues);
            return dstUsableNumber;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

}
