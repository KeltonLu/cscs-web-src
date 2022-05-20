//******************************************************************
//*  作    者：James
//*  功能說明：Perso項目種類管理邏輯
//*  創建日期：2008-09-08
//*  修改日期：2008-09-08 12:00
//*  修改記錄：
//*            □2008-09-08
//*              1.創建 占偉林
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
/// 卡片庫存轉移作業
/// </summary>
public class Depository011BL : BaseLogic
{
    #region SQL語句
    public const string SEL_CARDTYPE_STOCKS_MOVE = "SELECT MOVE.Move_Date, MOVE.Move_Number, MOVE.CardType_Move_RID, FROMF.Factory_ShortName_CN AS From_Factory,"
                                        +" TOF.Factory_ShortName_CN AS To_Factory, CARD_TYPE.Name"
                                        +" FROM CARDTYPE_STOCKS_MOVE AS MOVE"
                                        +" LEFT OUTER JOIN FACTORY AS FROMF ON FROMF.RID = MOVE.From_Factory_RID"
                                        +" LEFT OUTER JOIN FACTORY AS TOF ON TOF.RID = MOVE.To_Factory_RID "
                                        +" LEFT OUTER JOIN CARD_TYPE ON CARD_TYPE.RID = MOVE.CardType_RID"
                                        +" WHERE MOVE.RST = 'A'";
    //public const string SEL_CARDTYPE_STOCKS = "SELECT DISTINCT CT.RID,CT.Name "
    //                                    + "FROM CARDTYPE_STOCKS CTS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTS.CardType_RID = CT.RID "
    //                                    + "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND CTS.Perso_Factory_RID = F.RID "
    //                                    + "WHERE CTS.RST = 'A'";

    //查詢前日結庫存量
    public const string SEL_STOCKS = "SELECT Stock_Date,Stocks_Number"
                                + " FROM CARDTYPE_STOCKS"
                                + " WHERE RST='A' AND Perso_Factory_RID = @persoRid AND CardType_RID = @cardRid"
                                + " ORDER BY Stock_Date desc";

    //查詢當前日期是否日結
    public const string SEL_STOCKS_DATE = "SELECT RID"
                                + " FROM CARDTYPE_STOCKS"
                                + " WHERE RST='A' AND Stock_Date = @Stock_Date";

    //查詢最近日結後入庫量
    public const string SEL_DEPOSITORY_STOCK = "SELECT Sum(Income_Number) as Number"
                                + " FROM DEPOSITORY_STOCK"
                                + " WHERE RST = 'A' AND Income_Date > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";

    //查詢最近日結後退貨量
    public const string SEL_DEPOSITORY_CANCEl_NUMBER = "SELECT Sum(Cancel_Number) as Number"
                                + " FROM DEPOSITORY_CANCEL"
                                + " WHERE RST = 'A' AND Cancel_Date > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結後再入庫量
    public const string SEL_DEPOSITORY_RESTOCK = "SELECT Sum(Reincome_Number) as Number"
                                + " FROM DEPOSITORY_RESTOCK"
                                + " WHERE RST = 'A' AND Reincome_Date > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結後小計檔數量
    public const string SEL_SUBTOTAL_IMPORT = "SELECT Sum(Number) as Number"
                                + " FROM SUBTOTAL_IMPORT"
                                + " WHERE RST = 'A' AND Date_Time > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結後廠商移動量
    public const string SEL_FACTORY_CHANGE_IMPORT = "SELECT Sum(Number) as Number"
                                + " FROM FACTORY_CHANGE_IMPORT"
                                + " WHERE RST = 'A' AND Date_Time > @Stock_Date  AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結後卡片轉移量
    public const string CARDTYPE_STOCKS_MOVE_IN = "SELECT sum(Move_Number) as Number"
                                + " FROM CARDTYPE_STOCKS_MOVE"
                                + " WHERE RST = 'A' AND Move_Date > @Stock_Date AND To_Factory_RID = @persoRid AND CardType_RID = @cardRid";

    public const string CARDTYPE_STOCKS_MOVE_OUT = "SELECT Sum(Move_Number) as Number"
                                + " FROM CARDTYPE_STOCKS_MOVE"
                                + " WHERE RST = 'A' AND Move_Date > @Stock_Date AND From_Factory_RID = @persoRid AND CardType_RID = @cardRid";

    public const string SEL_STOCKS_CARDTYPE = "SELECT DISTINCT CT.RID,CT.Name,0 as Move_Number,0 as From_Factory_RID,0 as To_Factory_RID "
                                        + "FROM CARDTYPE_STOCKS CTS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTS.CardType_RID = CT.RID "                                       
                                        + "WHERE CTS.RST = 'A' and  CTS.stock_date in (select max(stock_date) from CARDTYPE_STOCKS )";

    public const string SEL_CARDTYPE_STOCKS = "SELECT CTS.Perso_Factory_RID,CTS.CardType_RID,CTS.Stock_Date,CTS.Stocks_Number,CT.Name,F.Factory_ShortName_CN "
                                        + " FROM CARDTYPE_STOCKS CTS"
                                        + " INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTS.CardType_RID = CT.RID "
                                        + " INNER JOIN FACTORY F ON F.RST = 'A' and F.is_perso = 'Y' AND CTS.Perso_Factory_RID = F.RID "
                                        + " WHERE CTS.RST = 'A' and CTS.stock_date in (select max(stock_date) from CARDTYPE_STOCKS)";
    public const string SEL_FACTORY_ALL = "SELECT RID,Factory_ShortName_CN "
                                        + "FROM FACTORY "
                                        + "WHERE RST = 'A' AND Is_Perso = 'Y'";
    public const string CON_CARDTYPE_STOCKS_MOVE_DEPOSITROY = "SELECT COUNT(*) "
                                        + "FROM CARDTYPE_STOCKS "
                                        + "WHERE RST = 'A' AND Perso_Factory_RID = @perso_factory_rid AND CardType_RID = @cardtype_rid AND Stocks_Number>=@move_number";
    public const string SEL_MAX_MOVE_ID = "SELECT TOP 1 CardType_Move_RID "
                                        + "FROM CARDTYPE_STOCKS_MOVE "
                                        + "WHERE Move_Date >= @move_date1 AND Move_Date<=@move_date2 "
                                        + "ORDER BY CardType_Move_RID DESC ";
    public const string SEL_CARDTYPE_STOCKS_MOVE_BY_MOVE_ID = "SELECT CTSM.RID,CardType_Move_RID,Move_Date,Move_Number,From_Factory_RID,To_Factory_RID,CardType_RID,"
                                        + "    FF.Factory_ShortName_CN AS From_Factory_Name,TF.Factory_ShortName_CN AS To_Factory_Name,CT.Name,CTSM.IS_CHECK "
                                        + "FROM CARDTYPE_STOCKS_MOVE CTSM LEFT JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTSM.CardType_RID = CT.RID "
                                        + "LEFT JOIN FACTORY FF ON FF.RST = 'A' AND FF.Is_Perso = 'Y' AND CTSM.From_Factory_RID = FF.RID "
                                        + "LEFT JOIN FACTORY TF ON TF.RST = 'A' AND TF.Is_Perso = 'Y' AND CTSM.To_Factory_RID = TF.RID "
                                        + "WHERE CTSM.RST = 'A' AND CTSM.CardType_Move_RID = @move_id "
                                        + "ORDER BY CardType_RID,From_Factory_RID ";
    public const string DEL_CARDTYPE_STOCKS_MOVE = "delete from CARDTYPE_STOCKS_MOVE "
                                        + " WHERE CardType_Move_RID = @move_id ";
    public const string SEL_FACTORY_CHANGE_NUM = "select *"
                                                 + "from FACTORY_CHANGE_IMPORT "
                                                 + "where RST = 'A' AND TYPE = @TYPE "
                                                 + "AND PHOTO = @PHOTO AND AFFINITY = @AFFINITY AND Perso_Factory_RID = @Perso_Factory_RID "
                                                 + "AND Date_Time > @Date_Time";
    public const string SEL_EXPRESSION_INFO = "select * from EXPRESSIONS_DEFINE "
                                              + "where RST = 'A' AND Expressions_RID = @Expressions_RID";

    public const string CON_WORK_DATE = "SELECT count(*) from WORK_DATE where rst = 'A' and CONVERT(VARCHAR(10), Date_Time, 111) = @CheckDate and Is_WorkDay = 'Y'";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Depository011BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 獲得Perso廠商
    /// </summary>
    /// <returns>DataTable[Perso廠商]</returns>
    public DataTable GetFactoryList()
    {
        DataSet dstFactory = null;
        try
        {
            
            dstFactory = dao.GetList(SEL_FACTORY_ALL);
            return dstFactory.Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
    }

    public DataTable getCardType(Dictionary<string, object> searchInput)
    {
        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_STOCKS_CARDTYPE);
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (((DataTable)searchInput["selectedCardType"]).Rows.Count > 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["selectedCardType"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";
                stbWhere.Append(" AND CTS.CardType_RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ")");
            }
        }
        //執行SQL語句
        DataSet dstCardType_Stocks_Move = null;
        try
        {
            dstCardType_Stocks_Move = dao.GetList(stbCommand.ToString() + stbWhere.ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return dstCardType_Stocks_Move.Tables[0];    
    }

    /// <summary>
    /// 判斷是否為工作日
    /// </summary>
    /// <param name="strCheckDate"></param>
    /// <returns></returns>
    public bool isWorkDay(string strCheckDate)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("CheckDate", strCheckDate);
            return dao.Contains(CON_WORK_DATE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }

    }

    /// <summary>
    /// 在已選卡種中篩選出有庫存的資料
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet SearchCardTypeDepository(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = "RID";//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_STOCKS_CARDTYPE);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (((DataTable)searchInput["selectedCardType"]).Rows.Count > 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["selectedCardType"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";
                stbWhere.Append(" AND CTS.CardType_RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ")");
            }
        }

        //執行SQL語句
        DataSet dstCardType_Stocks_Move = null;
        try
        {
            dstCardType_Stocks_Move = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException( GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstCardType_Stocks_Move;
    }

    /// <summary>
    /// 查詢Perso廠的庫存數量列表
    /// </summary>
    /// <param name="CardType_RID">卡種RID</param>
    /// <param name="Move_Date">轉移日期</param>
    /// <returns>DataSet[Perso廠的庫存數量]</returns>
    public DataSet SearchCardTypeDepositoryNum(string CardType_RID)
    {
        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CARDTYPE_STOCKS);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        this.dirValues.Clear();

        stbWhere.Append(" AND CTS.CardType_RID = @cardtype_rid");
        this.dirValues.Add("cardtype_rid", CardType_RID);

        //執行SQL語句
        DataSet dstCardType_Stocks_Move = null;
        try
        {
            dstCardType_Stocks_Move = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), this.dirValues);

            //返回查詢結果
            return dstCardType_Stocks_Move;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
    }

    /// <summary>
    /// 檢查Perso廠的對應卡種的轉移數量是否超過庫存數量
    /// </summary>
    /// <param name="CardType_RID">卡種</param>
    /// <param name="Perso_Factory_RID">轉出Perso廠RID</param>
    /// <param name="Move_Number">轉移數量</param>
    /// <returns>bool---true:轉移數量沒超過庫存數量;false:轉移數量超過庫存數量</returns>
    public bool ContainsPersoCardTypeDepository(String CardType_RID, String Perso_Factory_RID, String Move_Number)
    {
        try
        {
            DataSet dstlstock = getCheckStockByPerso(Perso_Factory_RID, CardType_RID);
            string checkDate = "1900-01-01";
            int storckNumber = 0;
            if (dstlstock.Tables[0].Rows.Count != 0)
            {
                checkDate = dstlstock.Tables[0].Rows[0]["Stock_Date"].ToString();
                storckNumber = Convert.ToInt32(dstlstock.Tables[0].Rows[0]["Stocks_Number"].ToString());
            }        

            int currentStork = getCurrentCardNumber(Perso_Factory_RID, CardType_RID, Convert.ToDateTime(checkDate), storckNumber);     
            if (currentStork >= int.Parse(Move_Number))
            {
                return true;
            }          
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return false;
    }

    /// <summary>
    /// 獲取當前的卡片庫存
    /// </summary>
    /// <param name="factoryRid"></param>
    /// <param name="CardRid"></param>
    /// <param name="storckDate"></param>
    /// <param name="oldStork"></param>
    /// <returns></returns>
    public int getCurrentCardNumber(string  factoryRid, string CardRid,DateTime storckDate,int oldStork)
    {
        int result = oldStork;
        dirValues.Clear();
        dirValues.Add("persoRid", factoryRid);
        dirValues.Add("cardRid", CardRid);
        dirValues.Add("Stock_Date", storckDate);
        //加上所有入庫記錄
        result += getNumber(SEL_DEPOSITORY_STOCK, dirValues);
        //減去所有退貨記錄
        result -= getNumber(SEL_DEPOSITORY_CANCEl_NUMBER, dirValues);
        //加上所有再入庫記錄
        result +=getNumber(SEL_DEPOSITORY_RESTOCK,dirValues);
        //加上所有小計檔匯入
        //result +=getNumber(SEL_SUBTOTAL_IMPORT, dirValues);
        //加上廠商異動記錄
        //result +=getNumber(SEL_FACTORY_CHANGE_IMPORT, dirValues);
        //加上所有移轉進入的紀錄
        result += getNumber(CARDTYPE_STOCKS_MOVE_IN, dirValues);
        //減去所有移出記錄
        result -= getNumber(CARDTYPE_STOCKS_MOVE_OUT, dirValues);
        //加上消耗的卡片數量
        result -= getUseCardNum(factoryRid, CardRid, storckDate);
        return result;
    }

    /// <summary>
    /// 計算消耗卡片數量
    /// </summary>
    /// <param name="factoryRID"></param>
    /// <param name="cardRID"></param>
    /// <param name="storkDate"></param>
    /// <returns></returns>
    public int getUseCardNum(String factoryRID, string cardRID, DateTime storkDate)
    {
        int result = 0;
        //查詢卡种基本信息
        CARD_TYPE cardTypeModel = dao.GetModel<CARD_TYPE, int>("RID", int.Parse(cardRID));
        dirValues.Clear();
        dirValues.Add("TYPE", cardTypeModel.TYPE);
        dirValues.Add("PHOTO", cardTypeModel.PHOTO);
        dirValues.Add("AFFINITY", cardTypeModel.AFFINITY);
        dirValues.Add("Perso_Factory_RID", factoryRID);
        dirValues.Add("Date_Time", storkDate);
        //查詢廠商異動信息
        DataTable factoryTable = dao.GetList(SEL_FACTORY_CHANGE_NUM, dirValues).Tables[0];
        dirValues.Clear();
        dirValues.Add("Expressions_RID", GlobalString.Expression.Used_RID);
        //查詢消耗量公式信息
        DataTable expTable = dao.GetList(SEL_EXPRESSION_INFO, dirValues).Tables[0];
        //按照公式計算消耗卡數量
        foreach (DataRow drowExp in expTable.Rows)
        {
            EXPRESSIONS_DEFINE expModel = new EXPRESSIONS_DEFINE();
            expModel = dao.GetModelByDataRow<EXPRESSIONS_DEFINE>(drowExp);
            //查詢該卡种是否應該在公式中出現，如果不出現，則ＣＯＮＴＩＮＵＥ
            CARDTYPE_STATUS statusModel = dao.GetModel<CARDTYPE_STATUS, int>("RID", expModel.Type_RID);
            if (statusModel.Is_Display.Equals(GlobalString.YNType.Yes))
            {
                continue;
            }
            int SUM = 0;
            foreach (DataRow drowFac in factoryTable.Rows)
            {
                FACTORY_CHANGE_IMPORT importModel = new FACTORY_CHANGE_IMPORT();
                importModel = dao.GetModelByDataRow<FACTORY_CHANGE_IMPORT>(drowFac);
                //計算相同計算卡种狀態的異動數量
                if (importModel.Status_RID == expModel.Type_RID)
                {
                    SUM += importModel.Number;
                }
            }
            //當前卡种狀態為+
            if (expModel.Operate == GlobalString.Operation.Add_RID)
            {
                result += SUM;
            }
            //當前卡种狀態為-
            if (expModel.Operate == GlobalString.Operation.Add_RID)
            {
                result -= SUM;
            }
        }
        return result;
    }    

    /// <summary>
    /// 取getCurrentStork方法中各查詢語句結果中的庫存操作數量
    /// </summary>
    /// <param name="SQL"></param>
    /// <param name="dirValues"></param>
    /// <returns></returns>
    public int getNumber(String SQL,Dictionary<string, object> dirValues){
        int result = 0;
        DataSet dstNumber = dao.GetList(SQL, dirValues);
        if (dstNumber.Tables[0].Rows[0]["Number"].ToString() != "") {
            result = int.Parse(dstNumber.Tables[0].Rows[0]["Number"] + "");
        }
        return result;
    }   

    /// <summary>
    /// 查詢卡片庫存轉移單列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[卡片庫存轉移單]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Move_Date" : sortField);//默認的排序欄位

        sortType = (sortField == "null" ? "desc " : sortType);//默認的排序欄位
        //if (strSortField == "Move_Date1")
        //{
        //    strSortField = "Move_Date";
        //}
        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CARDTYPE_STOCKS_MOVE);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBeginDate"].ToString().Trim()))
            {
                stbWhere.Append(" and Move_Date >= @BeginDate");
                dirValues.Add("BeginDate", searchInput["txtBeginDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtEndDate"].ToString().Trim()))
            {
                stbWhere.Append(" and Move_Date<=@EndDate");
                dirValues.Add("EndDate", searchInput["txtEndDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropFromFactory"].ToString().Trim()))
            {
                stbWhere.Append(" and From_Factory_RID = @From_Factory");
                dirValues.Add("From_Factory", searchInput["dropFromFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropToFactory"].ToString().Trim()))
            {
                stbWhere.Append(" and To_Factory_RID = @To_Factory");
                dirValues.Add("To_Factory", searchInput["dropToFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtCardType"].ToString().Trim()))
            {
                stbWhere.Append(" and Name like @Name");
                dirValues.Add("Name", "%" + searchInput["txtCardType"].ToString().Trim() + "%");
            }
        }    

        //執行SQL語句
        DataSet dstCardType_Stocks_Move = null;
        try
        {
            dstCardType_Stocks_Move = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstCardType_Stocks_Move;
    }

    /// <summary>
    /// 查詢卡片庫存轉移單
    /// </summary>
    /// <param name="strMove_ID">轉移單號</param>
    /// <returns>DataSet[卡片庫存轉移單]</returns>
    public DataSet ListModel(String strMove_ID)
    {
        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CARDTYPE_STOCKS_MOVE_BY_MOVE_ID);

        DataSet dstCardTypeStocksMove = new DataSet();
        try
        {
            this.dirValues.Add("move_id", strMove_ID);
            // 以轉移單號，取卡片庫存轉移單訊息
            dstCardTypeStocksMove = dao.GetList(stbCommand.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        //返回查詢結果
        return dstCardTypeStocksMove;
    }

    public DataSet getCheckStockByPerso(string factoryRid, string cardTypeRid)
    {
        DataSet dstlstock = null;
        dirValues.Clear();
        dirValues.Add("persoRid", factoryRid);
        dirValues.Add("cardRid", cardTypeRid);
        try
        {
            dstlstock = dao.GetList(SEL_STOCKS, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return dstlstock;
    }

    /// <summary>
    /// 卡片庫存轉移單新增
    /// </summary>
    /// <param name="dtStocksMove">DataTable<卡片庫存轉移明細></param>
    /// <param name="strMove_Date">轉移日期</param>
    public void Add(DataTable dtStocksMove,String strMove_Date)
    {
        CARDTYPE_STOCKS_MOVE csmModel = new CARDTYPE_STOCKS_MOVE();
        try
        {
            //事務開始
            dao.OpenConnection();
            
            #region 取移動單號
            String Move_ID = this.GetMove_ID(strMove_Date);
            #endregion 取移動單號           

            #region 新增卡片庫存移動檔
            foreach (DataRow drowStocksMove in dtStocksMove.Rows)
            {
                csmModel.CardType_Move_RID = Move_ID;
                csmModel.Move_Date = Convert.ToDateTime(strMove_Date);
                csmModel.CardType_RID = Convert.ToInt32(drowStocksMove["RID"]);
                csmModel.From_Factory_RID = Convert.ToInt32(drowStocksMove["From_Factory_RID"]);
                csmModel.To_Factory_RID = Convert.ToInt32(drowStocksMove["To_Factory_RID"]);
                csmModel.Move_Number = Convert.ToInt32(drowStocksMove["Move_Number"]);
                csmModel.Is_Check = GlobalString.YNType.No;
                dao.Add<CARDTYPE_STOCKS_MOVE>(csmModel, "RID");
            }
            #endregion 新增卡片庫存移動檔

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException( GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 以轉移日期，取轉移單號，單號格式yyyyMMddAA（AA為兩位數的順序號）
    /// </summary>
    /// <param name="Move_Date">轉移日期</param>
    /// <returns>String<轉移單號></returns>
    public string GetMove_ID(String Move_Date)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("move_date1", Move_Date + " 00:00:00");
            dirValues.Add("move_date2", Move_Date + " 23:59:59");
            DateTime dtMove_Date = Convert.ToDateTime(Move_Date);

            // 取轉移日期當天的最大轉移單號
            DataSet dtsMaxMoveID = dao.GetList(SEL_MAX_MOVE_ID, dirValues);
            if (dtsMaxMoveID.Tables[0].Rows.Count > 0)
            {
                int intMaxID = Convert.ToInt32(dtsMaxMoveID.Tables[0].Rows[0]["CardType_Move_RID"].ToString().Substring(8, 2));
                intMaxID++;
                if (intMaxID > 9)
                {
                    return dtMove_Date.ToString("yyyyMMdd") + intMaxID.ToString();
                }
                else
                {
                    return dtMove_Date.ToString("yyyyMMdd") + "0" + intMaxID.ToString();
                }
            }
            else
            {
                return dtMove_Date.ToString("yyyyMMdd") + "01";
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
    }

    /// <summary>
    /// 卡種庫存轉移修改
    /// </summary>
    /// <param name="dtStocksMove">卡種轉移單明細</param>
    /// <param name="strMove_ID">轉移單</param>
    public void Update(DataTable dtStocksMove, String strMove_ID)
    {
        CARDTYPE_STOCKS_MOVE ctsmModel = new CARDTYPE_STOCKS_MOVE();
        try
        {
            // 事務開始
            dao.OpenConnection();

            #region 刪除庫存轉移單
            this.dirValues.Clear();
            this.dirValues.Add("update_user_id",((USERS)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER]).UserID);
            this.dirValues.Add("update_time",DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            this.dirValues.Add("move_id", strMove_ID);
            dao.ExecuteNonQuery(DEL_CARDTYPE_STOCKS_MOVE,this.dirValues);
            #endregion 刪除庫存轉移單

            #region 修改庫存轉移單
            foreach (DataRow drStocksMove in dtStocksMove.Rows)
            {
                ctsmModel.CardType_Move_RID = drStocksMove["CardType_Move_RID"].ToString();
                ctsmModel.Move_Date = Convert.ToDateTime(drStocksMove["Move_Date"]);
                ctsmModel.CardType_RID = Convert.ToInt32(drStocksMove["CardType_RID"]);
                ctsmModel.Move_Number = Convert.ToInt32(drStocksMove["Move_Number"]);
                ctsmModel.From_Factory_RID = Convert.ToInt32(drStocksMove["From_Factory_RID"]);
                ctsmModel.To_Factory_RID = Convert.ToInt32(drStocksMove["To_Factory_RID"]);
                ctsmModel.Is_Check = GlobalString.YNType.No;
                dao.Add<CARDTYPE_STOCKS_MOVE>(ctsmModel, "RID");
            }
            #endregion 修改庫存轉移單

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();

            ExceptionFactory.CreateCustomSaveException( GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 庫存轉移單刪除
    /// </summary>
    /// <param name="strRID">庫存轉移單號</param>
    public void Delete(String strRID)
    {
        try
        {
            // 事務開始
            dao.OpenConnection();
            
            // 刪除庫存轉移單
            this.dirValues.Clear();
            this.dirValues.Add("update_user_id", ((USERS)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER]).UserID);
            this.dirValues.Add("update_time", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            this.dirValues.Add("move_id", strRID);

            dao.ExecuteNonQuery(DEL_CARDTYPE_STOCKS_MOVE, dirValues);

            //操作日誌
            SetOprLog("4");

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();

            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_DeleteFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_DeleteFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 判斷日期是否日結
    /// </summary>
    /// <param name="checkDate">日結日期</param>
    public bool isCheck(DateTime checkDate)
    {
        dirValues.Clear();
        dirValues.Add("Stock_Date",checkDate);
        DataSet checkDateSet = dao.GetList(SEL_STOCKS_DATE, dirValues);
        if (checkDateSet.Tables.Count == 0)
        {
            return true;
        }
        return false;
    }

}
