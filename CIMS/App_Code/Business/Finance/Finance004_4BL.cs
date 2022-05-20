//******************************************************************
//*  作    者：lantaosu
//*  功能說明：晶片信用卡資本化攤銷查詢管理邏輯
//*  創建日期：2008-12-11
//*  修改日期：2008-12-12 12:00
//*  修改記錄：
//*            □2008-12-12
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
/// Finance004_4BL 的摘要描述
/// </summary>
public class Finance004_4BL
{
    #region SQL語句    
    public const string SEL_ACCOUNT_DAYS = "SELECT Param_Code, Param_Name "
                                        + "FROM PARAM "
                                        + "WHERE  RST = 'A'  AND ParamType_Code='CardCost' ";
    public const string SEL_STOCK_COST_YEAR = "SELECT distinct  LEFT(Date_Time,4) AS Year "
                                        + "FROM STOCKS_COST "
                                        + "WHERE  RST = 'A' ORDER BY Year DESC";
    //public const string SEL_WORK_DATE = "SELECT distinct Date_Time "
    //                                    + "FROM WORK_DATE "
    //                                    + "WHERE  RST = 'A'  AND Is_WorkDay='Y' AND (Date_Time BETWEEN @DateFrom  AND @DateTo) ";
    //public const string SEL_STOCK_DATE = "SELECT distinct Stock_Date "
    //                                    + "FROM CARDTYPE_STOCKS "
    //                                    + "WHERE  RST = 'A'  AND (Stock_Date BETWEEN @DateFrom  AND @DateTo) "
    //                                    + "ORDER BY Stock_Date DESC ";
    public const string SEL_STOCKS_COST_SNUMBER = "SELECT C.Date_Time, C.S_Number "
                                        + "FROM STOCKS_COST C  "
                                        + "LEFT JOIN CARD_GROUP G ON G.RID = C.Group_RID  "
                                        + "WHERE  C.RST = 'A'  AND (C.Date_Time BETWEEN @DateFrom  AND @DateTo) AND G.Group_Name='晶片信用卡'  ";                                         
    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Finance004_4BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢庫存成本檔，取出其已有資料的年份
    /// </summary>
    /// <returns></returns>
    public DataSet GetYearList()
    {
        DataSet dstYear = null;
        DataSet dstYear1 = null;
        int year;
        try
        {
            dstYear = dao.GetList(SEL_STOCK_COST_YEAR);
            dstYear1 = dstYear.Clone();
            if (dstYear.Tables[0].Rows.Count == 0)
                year = DateTime.Now.Year;
            else
                year = Convert.ToInt32(dstYear.Tables[0].Rows[0][0].ToString());
            //加上未來10年
            for (int i = 10; i > 0; i--)
            {
                DataRow dr = dstYear1.Tables[0].NewRow();
                dr["Year"] = year + i;
                dstYear1.Tables[0].Rows.Add(dr);
            }

            if (dstYear1.Tables[0].Select("Year=" + DateTime.Now.Year.ToString()).Length == 0)
            {
                DataRow dr = dstYear1.Tables[0].NewRow();
                dr["Year"] = DateTime.Now.Year.ToString();
                dstYear1.Tables[0].Rows.Add(dr);
            }

            //過去已有資料年份
            foreach (DataRow dr1 in dstYear.Tables[0].Rows)
            {
                if (StringUtil.IsEmpty(dr1["Year"].ToString().Trim()) || dr1["Year"].ToString().Trim() == DateTime.Now.Year.ToString())
                    continue;

                DataRow dr2 = dstYear1.Tables[0].NewRow();
                dr2["Year"] = dr1["Year"].ToString();
                dstYear1.Tables[0].Rows.Add(dr2);
            }
            return dstYear1;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 查詢系统参数檔PARAM，取得帳務的起、迄日
    /// </summary>
    /// <returns></returns>
    public DataSet GetAccountDays()
    {
        DataSet dstAccountDays = null;

        try
        {
            dstAccountDays = dao.GetList(SEL_ACCOUNT_DAYS);
            return dstAccountDays;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 查詢營業日期資料檔，找到在此工作日區間内的所有工作日期
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    //public DataSet GetWorkDateList(string DateFrom, string DateTo)
    //{
    //    DataSet dstWorkDateList = null;

    //    dirValues.Clear();
    //    dirValues.Add("DateFrom", DateFrom);
    //    dirValues.Add("DateTo", DateTo);

    //    try
    //    {
    //        dstWorkDateList = dao.GetList(SEL_WORK_DATE, dirValues);
    //        return dstWorkDateList;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
    //        throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
    //    }
    //}

    /// <summary>
    /// 查詢卡種庫存檔，找到在此工作日區間内的所有日期
    /// </summary>
    /// <param name="move_id"></param>
    /// <returns></returns>
    //public DataSet GetStockDateList(string DateFrom, string DateTo)
    //{
    //    DataSet dstStockDateList = null;

    //    dirValues.Clear();
    //    dirValues.Add("DateFrom", DateFrom);
    //    dirValues.Add("DateTo", DateTo);

    //    try
    //    {
    //        dstStockDateList = dao.GetList(SEL_STOCK_DATE, dirValues);
    //        return dstStockDateList;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
    //        throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
    //    }
    //}

    /// <summary>
    /// 查詢庫存成本檔中，從起始日期到使用者輸入日期之間的，所有月份的晶片信用卡的製成卡總成本
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List( string DateFrom, string DateTo, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Date_Time" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_STOCKS_COST_SNUMBER);        

        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        
        //執行SQL語句
        DataSet dst = null;
        try
        {
            dst = dao.GetList(stbCommand.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
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
    /// 添加報表數據到數據庫中
    /// </summary>
    /// <param name="dt"></param>
    public void AddReport(DataTable dt,string time)
    {
        RPT_Finance004_4 RPT = new RPT_Finance004_4();
        try
        {
            int t = 0;
            int i = 0;
            DataRow dr=null;
            //事務開始
            dao.OpenConnection();
            dao.ExecuteNonQuery("Delete From RPT_Finance004_4 where TimeMark<"+DateTime.Now.ToString("yyyyMMdd000000"));

            for (t = 0; t < dt.Rows.Count-1; t++)
            {
                dr = dt.Rows[t];
                for (i = 5; i < dt.Columns.Count; i++)
                {
                    RPT.科目 = dr["科目"].ToString();
                    RPT.摘要 = dr["摘 要"].ToString();
                    RPT.原始成本 = dr["原始成本"].ToString();
                    RPT.每期需攤金額 = dr["每期需攤金額"].ToString();
                    RPT.帳面價值 = dr["帳面價值"].ToString();
                    RPT.Date = dt.Columns[i].ColumnName;
                    RPT.Number = dr[i].ToString();
                    RPT.TimeMark = time;

                    dao.Add<RPT_Finance004_4>(RPT);
                }
                
            }
            dr = dt.Rows[t];
            for (i = 5; i < dt.Columns.Count; i++)
            {
                RPT.科目 = "z";
                RPT.摘要 = dr["摘 要"].ToString();
                RPT.原始成本 = dr["原始成本"].ToString();
                RPT.每期需攤金額 = dr["每期需攤金額"].ToString();
                RPT.帳面價值 = dr["帳面價值"].ToString();
                RPT.Date = dt.Columns[i].ColumnName;
                RPT.Number = dr[i].ToString();
                RPT.TimeMark = time;

                dao.Add<RPT_Finance004_4>(RPT);
            }
            
           

            //事務提交
            dao.Commit();

        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }

    }

}
