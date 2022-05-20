//******************************************************************
//*  作    者：lantaosu
//*  功能說明：多功能報表 
//*  創建日期：2008-12-01
//*  修改日期：2008-12-03 18:00
//*  修改記錄：
//*            □2008-12-03
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
/// Report019BL 的摘要描述
/// </summary>
public class Report019BL
{
    #region SQL語句
    public const string SEL_ALL_USE = "SELECT Param_Code, Param_Name "
                                        + "FROM PARAM "
                                        + "WHERE  RST = 'A'  AND ParamType_Code='use' ";
    public const string SEL_GROUP_BY_USE = "SELECT RID, Group_Name "
                                        + "FROM CARD_GROUP  "
                                        + "WHERE  RST = 'A'  AND Param_Code = @Param_Code ";
    public const string SEL_ALL_CARD_STATUS = "SELECT Status_Code, Status_Name "
                                        + "FROM CARDTYPE_STATUS  "
                                        + "WHERE  RST = 'A'  ";
    public const string SEL_CARD_STATUS = "SELECT Status_Code, Status_Name "
                                        + "FROM CARDTYPE_STATUS "
                                        + "WHERE  RST = 'A'  AND Status_Name NOT IN('3D','DA','PM','RN') ";
    public const string SELECT_BY_DAY = "SELECT  Date_Time, TYPE, AFFINITY, PHOTO, SUM(Number) AS Sum, Name, Status_Name, Status_Code From ";
    public const string GROUP_BY_DAY = "GROUP BY Date_Time, TYPE, AFFINITY, PHOTO, Name, Status_Name, Status_Code ";
    public const string SELECT_BY_MONTH = "SELECT   LEFT(Date_Time, 6) AS Date_Time, TYPE, AFFINITY, PHOTO, SUM(Number) AS Sum, Name, Status_Name, Status_Code From ";
    public const string GROUP_BY_MONTH = "GROUP BY  LEFT(Date_Time, 6), TYPE, AFFINITY, PHOTO, Name, Status_Name, Status_Code ";
    public const string SEL_CARD_NUMBER = "(SELECT CONVERT(varchar(8), S1.Date_Time, 112) AS Date_Time, S1.TYPE, S1.AFFINITY, S1.PHOTO, SUM(S1.Number) AS Number, C1.Name, M.Type_Name AS Status_Name,CS1.Status_Code "
                                        + " ,S1.Perso_Factory_RID   "//20090702CR
                                        + "FROM SUBTOTAL_IMPORT S1   "
                                        + "LEFT JOIN CARD_TYPE C1 ON C1.RST='A' AND  C1.TYPE= S1.TYPE AND  C1.AFFINITY=S1.AFFINITY  AND C1.PHOTO=S1.PHOTO "
                                        + "LEFT JOIN GROUP_CARD_TYPE G1 ON G1.RST='A'  AND G1.CardType_RID=C1.RID "
                                        + "LEFT JOIN MAKE_CARD_TYPE M ON M.RST='A' AND M.RID=S1.MakeCardType_RID AND M.Is_Import='Y'   "
                                        + "LEFT JOIN CARDTYPE_STATUS CS1 ON CS1.RST='A' AND CS1.Status_Name=M.Type_Name  "
                                        + "WHERE S1.RST='A' AND (S1.Date_Time BETWEEN @DateFrom AND @DateTo) AND G1.Group_RID=@Group_RID AND M.Type_Name IN ('3D','DA','PM','RN')  "
                                        + "GROUP BY CONVERT(varchar(8), S1.Date_Time, 112), S1.TYPE, S1.AFFINITY, S1.PHOTO, C1.Name, M.Type_Name, CS1.Status_Code "
                                        + " ,S1.Perso_Factory_RID   "//20090702CR
                                        + "UNION all "
                                        + "SELECT CONVERT(varchar(8), S2.Date_Time, 112) AS Date_Time, S2.TYPE, S2.AFFINITY, S2.PHOTO,  SUM(S2.Number) AS Number,C2.Name,  'Action '+S2.Action AS Status_Name, S2.Action AS Status_Code "
                                        + " ,S2.Perso_Factory_RID   "//20090702CR
                                        + "FROM SUBTOTAL_IMPORT S2  "
                                        + "LEFT JOIN CARD_TYPE C2 ON C2.RST='A'  AND  C2.TYPE= S2.TYPE AND  C2.AFFINITY=S2.AFFINITY  AND C2.PHOTO=S2.PHOTO "
                                        + "LEFT JOIN GROUP_CARD_TYPE G2 ON G2.RST='A'  AND G2.CardType_RID=C2.RID  "
                                        + "WHERE S2.RST='A' AND (S2.Date_Time BETWEEN @DateFrom AND @DateTo)  AND G2.Group_RID=@Group_RID AND S2.Action IN ('1','2','3','5')  "
                                        + "GROUP BY CONVERT(varchar(8), S2.Date_Time, 112), S2.TYPE, S2.AFFINITY, S2.PHOTO,  C2.Name,  'Action '+S2.Action, S2.Action "
                                        + " ,S2.Perso_Factory_RID   "//20090702CR
                                        + "UNION all "
                                        + "SELECT CONVERT(varchar(8), F.Date_Time, 112) AS Date_Time, F.TYPE, F.AFFINITY, F.PHOTO, SUM(F.Number) AS Number, C3.Name, CS.Status_Name, CS.Status_Code  "
                                        + " ,F.Perso_Factory_RID   "//20090702CR
                                        + "FROM FACTORY_CHANGE_IMPORT F  "
                                        + "LEFT JOIN CARD_TYPE C3 ON C3.RST='A'   AND C3.TYPE=F.TYPE AND  C3.AFFINITY=F.AFFINITY  AND C3.PHOTO=F.PHOTO  "
                                        + "LEFT JOIN GROUP_CARD_TYPE G3 ON G3.RST='A'  AND G3.CardType_RID=C3.RID   "
                                        + "LEFT JOIN CARDTYPE_STATUS CS ON CS.RST='A' AND CS.Status_Code=F.Status_RID  "
                                        + "WHERE F.RST='A' AND (F.Date_Time BETWEEN @DateFrom AND @DateTo) AND G3.Group_RID=@Group_RID AND  CS.Status_Name IN('缺卡','未製卡','補製卡','樣卡','製損卡','感應不良','排卡','銷毀','調整') "
                                        + "GROUP BY CONVERT(varchar(8), F.Date_Time, 112) , F.TYPE, F.AFFINITY, F.PHOTO,  C3.Name, CS.Status_Name, CS.Status_Code "
                                        + " ,F.Perso_Factory_RID   "//20090702CR
                                        + " UNION all "
                                        + "SELECT CONVERT(varchar(8), DS.Income_Date, 112)  AS Date_Time, C4.TYPE, C4.AFFINITY, C4.PHOTO, SUM(DS.Income_Number) AS Number, C4.Name, '入庫' AS Status_Name, CS4.Status_Code  "
                                        + " ,DS.Perso_Factory_RID   "//20090702CR
                                        + "FROM DEPOSITORY_STOCK  DS  "
                                        + "LEFT JOIN CARD_TYPE C4 ON C4.RST='A'  AND  C4.RID= DS.Space_Short_RID  "
                                        + "LEFT JOIN GROUP_CARD_TYPE G4 ON G4.RST='A'  AND G4.CardType_RID=C4.RID "
                                        + "LEFT JOIN CARDTYPE_STATUS CS4 ON CS4.RST='A' AND CS4.Status_Name='入庫'   "
                                        + "WHERE DS.RST='A' AND (DS.Income_Date BETWEEN @DateFrom AND @DateTo) AND G4.Group_RID= @Group_RID "
                                        + "GROUP BY  CONVERT(varchar(8), DS.Income_Date, 112), C4.TYPE, C4.AFFINITY, C4.PHOTO,  C4.Name,  Status_Name, CS4.Status_Code "
                                        + " ,DS.Perso_Factory_RID   "//20090702CR
                                        + " UNION all "
                                        + "SELECT CONVERT(varchar(8), DC.Cancel_Date, 112)  AS Date_Time, C5.TYPE, C5.AFFINITY, C5.PHOTO, SUM(DC.Cancel_Number) AS Number, C5.Name, '退貨' AS Status_Name, CS5.Status_Code  "
                                        + " ,DC.Perso_Factory_RID   "//20090702CR
                                        + "FROM DEPOSITORY_CANCEL DC  "
                                        + "LEFT JOIN CARD_TYPE C5 ON C5.RST='A'  AND  C5.RID= DC.Space_Short_RID   "
                                        + "LEFT JOIN GROUP_CARD_TYPE G5 ON G5.RST='A'  AND G5.CardType_RID=C5.RID "
                                        + "LEFT JOIN CARDTYPE_STATUS CS5 ON CS5.RST='A' AND CS5.Status_Name='退貨'    "
                                        + "WHERE DC.RST='A' AND (DC.Cancel_Date BETWEEN @DateFrom AND @DateTo) AND G5.Group_RID= @Group_RID "
                                        + "GROUP BY CONVERT(varchar(8), DC.Cancel_Date, 112), C5.TYPE, C5.AFFINITY, C5.PHOTO,  C5.Name, Status_Name, CS5.Status_Code "
                                        + " ,DC.Perso_Factory_RID   "//20090702CR
                                        + "UNION  all "
                                        + "SELECT CONVERT(varchar(8), DR.Reincome_Date, 112)  AS Date_Time, C6.TYPE, C6.AFFINITY, C6.PHOTO, SUM(DR.Reincome_Number) AS Number, C6.Name,  '再入庫' AS Status_Name, CS6.Status_Code  "
                                        + " ,DR.Perso_Factory_RID   "//20090702CR
                                        + "FROM DEPOSITORY_RESTOCK DR  "
                                        + "LEFT JOIN CARD_TYPE C6 ON C6.RST='A'  AND  C6.RID= DR.Space_Short_RID   "
                                        + "LEFT JOIN GROUP_CARD_TYPE G6 ON G6.RST='A'  AND G6.CardType_RID=C6.RID "
                                        + "LEFT JOIN CARDTYPE_STATUS CS6 ON CS6.RST='A' AND CS6.Status_Name='再入庫'   "
                                        + "WHERE DR.RST='A' AND (DR.Reincome_Date BETWEEN @DateFrom AND @DateTo) AND G6.Group_RID= @Group_RID "
                                        + "GROUP BY CONVERT(varchar(8), DR.Reincome_Date, 112), C6.TYPE, C6.AFFINITY, C6.PHOTO,  C6.Name,  Status_Name, CS6.Status_Code "
                                        + " ,DR.Perso_Factory_RID   "//20090702CR
                                        + ") AS table1 ";
    public const string SEL_EXPRESSION = "SELECT Type_RID,Operate "
                                        + "FROM EXPRESSIONS_DEFINE  "
                                        + "WHERE RST='A' AND Expressions_RID=@Expressions_RID ";
    public const string SEL_CARD_STATUS_NAME = "SELECT Status_Name "
                                        + "FROM CARDTYPE_STATUS  "
                                        + "WHERE  RST = 'A' AND RID=@RID ";
    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Report019BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
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
    /// 預設選擇批次
    /// </summary>
    /// <returns></returns>
    public DataSet GetAllCardStatusList()
    {
        DataSet dstAllCardStatusList = null;

        try
        {
            dstAllCardStatusList = dao.GetList(SEL_ALL_CARD_STATUS + " and Status_Code<>18 ");
            return dstAllCardStatusList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 預設選擇批次
    /// </summary>
    /// <returns></returns>
    public DataSet GetCardStatusList()
    {
        DataSet dstCardStatusList = null;

        try
        {
            dstCardStatusList = dao.GetList(SEL_CARD_STATUS);
            return dstCardStatusList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢多功能報表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List(string Factory_RID, string DateFrom, string DateTo, string Group_RID, string Date, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Date_Time" : sortField);//默認的排序欄位


        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder();
        StringBuilder stbCommand1 = new StringBuilder();
        StringBuilder stbWhere = new StringBuilder();
        StringBuilder stbWhere1 = new StringBuilder();
        if (Date == "day")
        {
            stbCommand = new StringBuilder(SELECT_BY_DAY);
            stbCommand1 = new StringBuilder(SEL_CARD_NUMBER);
            stbWhere.Append( GROUP_BY_DAY ); 
        }
        else
        {
            stbCommand = new StringBuilder(SELECT_BY_MONTH);
            stbCommand1 = new StringBuilder(SEL_CARD_NUMBER);
            stbWhere.Append( GROUP_BY_MONTH ); 
        }        
           
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", Group_RID);
        if(Factory_RID != "")
        {
            stbWhere1.Append( " where  Perso_Factory_RID=@Factory_RID ");
            dirValues.Add("Factory_RID", Factory_RID);
        }
        
        //執行SQL語句
        DataSet dst = null;
        try
        {
            dst = dao.GetList(stbCommand.ToString() + stbCommand1.ToString() + stbWhere1.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
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
    /// 取得特定Status_Code的Status_Name
    /// </summary>
    /// <param name="Status_Code"></param>
    /// <returns></returns>
    public string GetStatusName(string Status_Code)
    {
        DataSet dstStatusName = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", Status_Code);
            dstStatusName = dao.GetList(SEL_CARD_STATUS_NAME, dirValues);
            return dstStatusName.Tables[0].Rows[0][0].ToString().Trim();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 添加報表數據
    /// </summary>
    /// <param name="dt"></param>
    public void AddReport(DataTable dt, string time, string isCheckDay, string Factory_RID)
    {
        RPT_Report019 RPT = new RPT_Report019();
        try
        {

            dao.ExecuteNonQuery("Delete From RPT_Report019 where TimeMark <'" + DateTime.Now.ToString("yyyyMMdd000000") + "'");

            
            for (int t = 0; t < (dt.Rows.Count - 1); t++)
            {
                DataRow dr = dt.Rows[t];
                #region 找出無耗用數值的行
                decimal RowsSum = 0;
                for (int columns = 5; columns < dt.Columns.Count; columns++)
                {
                    RowsSum = RowsSum + Math.Abs(Convert.ToDecimal(dr[columns].ToString().Replace(",","")));
                }
                #endregion              
                if (RowsSum != 0)
                {
                    for (int i = 5; i < dt.Columns.Count; i++)
                    {
                        int m = i + 1;

                        RPT.日期 = dr["日期"].ToString();
                        RPT.TYPE = dr["TYPE"].ToString();
                        RPT.AFFINITY = dr["AFFINITY"].ToString();
                        RPT.PHOTO = dr["PHOTO"].ToString();
                        RPT.版面簡稱 = dr["版面簡稱"].ToString();
                        RPT.Name = m.ToString().PadLeft(2, '0') + "-" + dt.Columns[i].ColumnName.Replace("Action 1", "新卡").Replace("Action 2", "掛補").Replace("Action 3", "毀補").Replace("Action 5", "換卡");
                        RPT.Number = dr[i].ToString();
                        RPT.TimeMark = time;

                        dao.Add<RPT_Report019>(RPT);
                    }
                    if (isCheckDay == "day")
                    {
                        //上一結餘日期
                        DataTable dtbljyDate = dao.GetList("select distinct top 1 stock_date from cardtype_stocks where stock_date<'" + RPT.日期 + "' order by stock_date desc	").Tables[0];

                        if (dtbljyDate.Rows.Count == 0)
                            continue;

                        string strjyDate = Convert.ToDateTime(dtbljyDate.Rows[0][0].ToString()).ToShortDateString();

                        DataTable dtbl = dao.GetList("select dbo.fun_report009_stocks_Card('" + strjyDate + "','" + dr["TYPE"].ToString() + "','" + dr["AFFINITY"].ToString() + "','" + dr["PHOTO"].ToString() + "','"+Factory_RID+"')").Tables[0];

                        if (dtbl.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dtbl.Rows[0][0].ToString()) < 0)
                                dtbl.Rows[0][0] = "0";

                            int n = 5;
                            RPT.日期 = dr["日期"].ToString();
                            RPT.TYPE = dr["TYPE"].ToString();
                            RPT.AFFINITY = dr["AFFINITY"].ToString();
                            RPT.PHOTO = dr["PHOTO"].ToString();
                            RPT.版面簡稱 = dr["版面簡稱"].ToString();
                            RPT.Name = n.ToString().PadLeft(2, '0') + "-前日結餘";
                            RPT.Number = dtbl.Rows[0][0].ToString();
                            RPT.TimeMark = time;

                            dao.Add<RPT_Report019>(RPT);

                            int jyToday = int.Parse(dtbl.Rows[0][0].ToString());
                            n = dt.Columns.Count + 2;
                            if (dt.Columns.Contains("入出庫"))
                            {
                                if (dr["入出庫"].ToString() != "")
                                    jyToday += int.Parse(dr["入出庫"].ToString());
                            }

                            if (dt.Columns.Contains("消耗卡"))
                            {
                                if (dr["消耗卡"].ToString() != "")
                                    jyToday -= int.Parse(dr["消耗卡"].ToString());
                            }

                            //if (jyToday < 0)
                            //    jyToday = 0;

                            RPT.日期 = dr["日期"].ToString();
                            RPT.TYPE = dr["TYPE"].ToString();
                            RPT.AFFINITY = dr["AFFINITY"].ToString();
                            RPT.PHOTO = dr["PHOTO"].ToString();
                            RPT.版面簡稱 = dr["版面簡稱"].ToString();
                            RPT.Name = n.ToString().PadLeft(2, '0') + "-當日結餘";
                            RPT.Number = jyToday.ToString();
                            RPT.TimeMark = time;

                            dao.Add<RPT_Report019>(RPT);
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }

    }

}
