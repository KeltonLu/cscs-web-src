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
/// Depository013BL 的摘要描述
/// </summary>
public class Depository013BL:BaseLogic
{
    #region SQL語句
    public const string SEL_FACTORY_ALL = "SELECT RID,Factory_ShortName_CN "
                                + "FROM FACTORY "
                                + "WHERE RST = 'A' AND Is_Perso = 'Y' ";
    public const string SEL_CARD_TYPE = "SELECT CT.Display_Name,CT.RID, CT.Display_Name AS NAME,CT.IS_USING "
                                + " FROM CARD_TYPE AS CT "
                                + " WHERE CT.RST = 'A'  ";
    //查詢每月批次表
    public const string SEL_MONTH_MONITOR = "SELECT * FROM MONTHLY_MONITOR where XType = @xCode and TYPE=@type and  AFFINITY=@affinity "
                                + " and PHOTO = @photo and Perso_Factory_Rid =@persoRid and CMonth >=@startMonth and CMonth <@endMonth ORDER BY CMonth";
    //月換卡耗用量
    public const string SEL_NEXTMONTH_CHANGE_NUMBER = "SELECT ISNULL(SUM(PFCC.Number),0) FROM PERSO_FORE_CHANGE_CARD AS PFCC"
                                + " WHERE PFCC.RST = 'A' "
                                + " AND PFCC.Perso_Factory_RID = @PersoRid AND PFCC.Change_Date = @Change_Date "
                                + " and PFCC.TYPE = @type and PFCC.PHOTO = @photo and PFCC.AFFINITY = @affinity ";
    //取開始時間到結束時間的Action= @action 的小計數量
    public const string SEL_PREMONTHS_NUMBER = "SELECT ISNULL(sum(SI.Number),0) FROM SUBTOTAL_IMPORT AS SI "
                                + " WHERE SI.RST = 'A' AND SI.Perso_Factory_RID = @PersoRid and SI.Date_Time >= @begin_date and SI.Date_Time < @end_date and SI.action = @action"
                                + " and SI.PHOTO = @photo and SI.TYPE = @type and SI.AFFINITY = @affinity ";

    //已下單未到貨數
    public const string SEL_NOT_INCOME_NUMBER = "SELECT (SELECT ISNULL(SUM(Number), 0)  FROM ORDER_FORM_DETAIL as ofd"
                                + " LEFT JOIN  ORDER_FORM ON ORDER_FORM.OrderForm_RID = ofd.OrderForm_RID"
                                + " WHERE (ORDER_FORM.Pass_Status = 4)"
                                + " AND (ofd.CardType_RID = @cardRid) AND (ofd.Delivery_Address_RID = @PersoRid) and ofd.fore_delivery_date > @beginDate and ofd.fore_delivery_date < @endDate ) -"
                                + " (SELECT ISNULL(SUM(Stock_Number), 0) FROM  DEPOSITORY_STOCK"
                                + " where DEPOSITORY_STOCK.orderform_detail_rid in (select ord.orderform_detail_rid   "
                                + " from ORDER_FORM_DETAIL  as ord "
                                + " LEFT JOIN  ORDER_FORM ON ORDER_FORM.OrderForm_RID =ord.OrderForm_RID "
                                + " WHERE (ORDER_FORM.Pass_Status = 4)"
                                + " and (ord.CardType_RID = @cardRid) AND (ord.Delivery_Address_RID = @PersoRid) and ord.fore_delivery_date > @beginDate and ord.fore_delivery_date < @endDate )"
                                + " and income_date < @endDate ) AS notIncomNum";

    public const string SEL_PRECARD = "SELECT stocks.Stocks_Number, factory.Factory_ShortName_CN, type.Name,"
                                + " type.TYPE, type.AFFINITY, type.PHOTO, stocks.Stock_Date,"
                                + " stocks.Perso_Factory_RID, stocks.CardType_RID"
                                + " FROM CARDTYPE_STOCKS  AS stocks"
                                + " LEFT OUTER JOIN CARD_TYPE AS type ON stocks.CardType_RID = type.RID"
                                + " LEFT OUTER JOIN FACTORY AS factory ON stocks.Perso_Factory_RID = factory.RID"
                                + " where stocks.rst = 'A' and stock_date = (select Max(stock_date) from cardtype_stocks)";
    public const string SEL_WORKDAY = "Select Max(Date_Time) FROM WORK_DATE where RST = 'A' AND Is_WorkDay = 'Y' AND Date_Time < @Month";    

    public const string SEL_LASTMONTH_STROCK_NUMBER = "Select stocks.Stock_Date,stocks.Stocks_Number "
                                + " FROM CARDTYPE_STOCKS  AS stocks"
                                + " where stocks.Perso_Factory_RID = @perso_rid and stocks.CardType_RID = @cardType_rid and Stock_Date = @stockDate";   
    
    public const string SEL_FACTORY_CHANGE_NUM = "select *"
                                + "from FACTORY_CHANGE_IMPORT "
                                + "where RST = 'A' AND TYPE = @TYPE "
                                + "AND PHOTO = @PHOTO AND AFFINITY = @AFFINITY AND Perso_Factory_RID = @Perso_Factory_RID "
                                + "AND Date_Time > @Stock_Date and Date_Time < @today";  

    public const string SEL_PARAM = "SELECT  Param_Name FROM PARAM WHERE ParamType_Code = @Param ";

    public const string SEL_MONTH_DATA = "SELECT Rid from MONTHLY_MONITOR where Perso_factory_RID = @perso_rid "
                                + " and cmonth = @cMonth and TYPE = @type and PHOTO = @photo and AFFINITY = @affinity and XType = @xType ";

    public const string CON_MONTH_DATA = "SELECT count(*) from MONTHLY_MONITOR where Perso_factory_RID = @perso_rid "
                                + " and cmonth = @cMonth and TYPE = @type and PHOTO = @photo and AFFINITY = @affinity and XType = @xType ";

    public const string IS_GENERATE = "select count(*) from MONTHLY_MONITOR where rst='A' and rut>=@now ";
#endregion
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Depository013BL()
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
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 判斷當天是否執行完批次
    /// </summary>
    /// <returns></returns>
    public Boolean checkBatch() {
        dirValues.Clear();
        dirValues.Add("now", DateTime.Now.ToString("yyyy/MM/dd"));
        return dao.Contains(IS_GENERATE, dirValues);    
    }

    public const string SEL_DAY_MONITOR1 = "SELECT monthT.*,card_type.rid as cardRid FROM  MONTHLY_MONITOR as monthT inner join card_type on card_type.affinity=monthT.affinity"
      + " and card_type.photo=monthT.photo and card_type.[type]=monthT.[type] where CMonth = @cMonth";

    public DataTable PreCardType(Dictionary<string, object> searchInput)
    {
        StringBuilder stbCommand = new StringBuilder(SEL_DAY_MONITOR1);
        //取預估開始月份
        DateTime BeginMonth = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01"));
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        dirValues.Add("cMonth", BeginMonth);
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropPerso"].ToString().Trim()))
            {
                stbWhere.Append(" and  monthT.Perso_Factory_RID = @PersoRid");
                dirValues.Add("PersoRid", int.Parse(searchInput["dropPerso"].ToString().Trim()));
            }
            if (!StringUtil.IsEmpty(searchInput["txtCardType"].ToString().Trim()))
            {
                stbWhere.Append(" and monthT.TYPE = @cardType");
                dirValues.Add("cardType", searchInput["txtCardType"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtAffinity"].ToString().Trim()))
            {
                stbWhere.Append(" and monthT.AFFINITY = @Affinity");
                dirValues.Add("Affinity", searchInput["txtAffinity"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtPhoto"].ToString().Trim()))
            {
                stbWhere.Append(" and monthT.Photo = @Photo");
                dirValues.Add("Photo", searchInput["txtPhoto"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" and monthT.Name like @name");
                dirValues.Add("name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }
            if (((DataTable)searchInput["UctrlCardType"]).Rows.Count > 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["UctrlCardType"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";

                stbWhere.Append(" AND card_type.rid IN (" + strCardType.Substring(0, strCardType.Length - 1) + ")");
            }

        }
        //執行SQL語句
        DataSet dstSafeStockInfo = null;
        dstSafeStockInfo = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        return dstSafeStockInfo.Tables[0];
    }

    /// <summary>
    /// 遍歷Perso廠和卡種的組合，構建數據
    /// </summary>
    /// <param name="dtbPersoCardType"></param>
    /// <param name="N">過去X月</param>
    /// <param name="Month">預估月份</param>
    /// <returns></returns>
    public DataSet List(DataRow[] persoCards, int xCode ,int Month)
    {
        //計算預計月份起
        DateTime begin = DateTime.Now;
        DataSet dst = new DataSet();         
        try
        {
            foreach (DataRow drow in persoCards)
            {
            
                DataTable result = new DataTable();
                creatNewDataTable2(result);           

                //取卡種信息
                CARD_TYPE cardModel = new CARD_TYPE();
                cardModel.AFFINITY = drow["AFFINITY"].ToString();
                cardModel.PHOTO = drow["Photo"].ToString(); //Photo
                cardModel.TYPE = drow["TYPE"].ToString();//card Type
                cardModel.RID = int.Parse(drow["CardRID"].ToString());
                cardModel.Name = drow["Name"].ToString();
                //取廠商信息
                int persoRid = int.Parse(drow["Perso_Factory_RID"].ToString());
                //取預估開始月份
                DateTime BeginMonth = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01"));
                //取預估結束月份
                DateTime EndMonth = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(Month);
                dirValues.Clear();
                dirValues.Add("type",cardModel.TYPE);
                dirValues.Add("affinity",cardModel.AFFINITY);
                dirValues.Add("photo",cardModel.PHOTO);
                dirValues.Add("persoRid",persoRid);
                dirValues.Add("xCode",xCode);
                dirValues.Add("startMonth", BeginMonth);
                dirValues.Add("endMonth", EndMonth);
                DataTable dtbMonth = dao.GetList(SEL_MONTH_MONITOR, dirValues).Tables[0];
                foreach (DataRow MonthRow in dtbMonth.Rows)
                {
                    DataRow newRow = result.NewRow();
                    CopyMonth2View(MonthRow, newRow);
                    result.Rows.Add(newRow);
                }
                dst.Tables.Add(result);
            }
        }catch(Exception ex){
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return dst;
    }
    public const string SEL_MONTH_MONITOR2 = "SELECT * FROM MONTHLY_MONITOR where XType = @xCode and TYPE=@type and  AFFINITY=@affinity "
                            + " and PHOTO = @photo and Perso_Factory_Rid =@persoRid and CMonth >@begainDay ORDER BY CMonth";

    public DataTable compute(CARD_TYPE cardModel, int persoRid, DataTable dtbInput, int xCode, string flag)
    {       
        string endDay = Convert.ToDateTime(dtbInput.Rows[dtbInput.Rows.Count - 1]["Month"]).ToString("yyyy/MM/dd");
        //將頁面的table和批次的table數據綜合起來從新計算數據
        DataTable dtbResult = new DataTable();
        creatNewDataTable2(dtbResult);
        foreach (DataRow rdrow in dtbInput.Rows)
        {
            dtbResult.Rows.Add(rdrow.ItemArray);
        }
        dirValues.Clear();
        dirValues.Add("type", cardModel.TYPE);
        dirValues.Add("affinity", cardModel.AFFINITY);
        dirValues.Add("photo", cardModel.PHOTO);
        dirValues.Add("persoRid", persoRid);
        dirValues.Add("xCode", xCode);
        dirValues.Add("begainDay", Convert.ToDateTime(dtbInput.Rows[dtbInput.Rows.Count - 1]["Month"]));
        DataTable dtbMonth = dao.GetList(SEL_MONTH_MONITOR2, dirValues).Tables[0];
        foreach (DataRow computeRow in dtbMonth.Rows)
        {
            DataRow newRow = dtbResult.NewRow();
            CopyMonth2View(computeRow, newRow);
            dtbResult.Rows.Add(newRow);
        }
        getTableAfterChange(dtbResult, flag);
        DataTable dtblCompute = new DataTable();
        creatNewDataTable2(dtblCompute);
        foreach (DataRow row in dtbResult.Select("Month <='" + endDay + "'"))
        {
            dtblCompute.Rows.Add(row.ItemArray);
        }
        return dtblCompute;
    }

    public void inputData2Temp(Dictionary<string, DataTable> dirTable,string time)
    {
        try
        {
            dao.OpenConnection();
            dao.ExecuteNonQuery("delete RPT_Depository013 where TimeMark<'"+DateTime.Now.ToString("yyyyMMddHHmmss")+"'");
            foreach (KeyValuePair<string, DataTable> key in dirTable)
            {
                DataTable datable = (DataTable)key.Value;
                FACTORY factoryModel = getFactory(int.Parse(key.Key.Split('_')[0]));
                CARD_TYPE cardModel = getCardType(int.Parse(key.Key.Split('_')[1]));
                string strNType = paramList(GlobalString.cardparam.NType);
                int nType = int.Parse(strNType.Substring(0, strNType.Length - 1));
                int XType = int.Parse(key.Key.Split('_')[3]);
                int XTypeValue = int.Parse(key.Key.Split('_')[2]);
                foreach (DataRow drow in datable.Rows)
                {
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                        + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                        + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                    + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'A'" + "," + Convert.ToDecimal(drow["A"])
                    + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                    + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'"+time+"')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                        + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                        + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                    + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'A1'" + "," + Convert.ToDecimal(drow["A1"])
                    + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                    + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'B'" + "," + Convert.ToDecimal(drow["B"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'C'" + "," + Convert.ToDecimal(drow["C"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");

                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'D1'" + "," + Convert.ToDecimal(drow["D1"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                     + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                     + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                 + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'D2'" + "," + Convert.ToDecimal(drow["D2"])
                 + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                 + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");

                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'E'" + "," + Convert.ToDecimal(drow["E"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'F'" + "," + Convert.ToDecimal(drow["F"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'G'" + "," + Convert.ToDecimal(drow["G"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')"); 
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'G1'" + "," + Convert.ToDecimal(drow["G1"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'H'" + "," + Convert.ToDecimal(drow["H"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'L'" + "," + Convert.ToDecimal(drow["L"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                       + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                       + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                   + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'J'" + "," + Convert.ToInt32(drow["J"])
                   + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                   + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                    dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013"
                     + "(Perso_Factory_rid, CardType_Rid, XType,XTypeValue, ColName, ColValue, CMonth,"
                     + " Factory_shortName_CN, card_type, affinity, Photo, Name,NValue,TimeMark)"
                 + "VALUES (" + factoryModel.RID + "," + cardModel.RID + ", " + XType + "," + XTypeValue + ",'K'" + "," + Convert.ToInt32(drow["K"])
                 + ",'" + Convert.ToDateTime(drow["Month"]).ToString("yyyy/MM/dd") + "', '" + factoryModel.Factory_ShortName_CN + "','"
                 + cardModel.TYPE + "','" + cardModel.AFFINITY + "','" + cardModel.PHOTO + "','" + cardModel.Name + "'," + nType + ",'" + time + "')");
                }
            }
            //匯總除（Ｈ，Ｌ，Ｋ）以外的數據
            dao.ExecuteNonQuery(" INSERT INTO RPT_Depository013 SELECT 0,0,XType, ColName, SUM(ColValue),CMonth,'','Total','','','',XTypeValue,NValue,TimeMark"
                       + " from   RPT_Depository013 where ColName not in('H','L') and TimeMark ='"+time+"'  GROUP BY ColName, CMonth, XType, XTypeValue, NValue,TimeMark");
            //事務提交
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

    public FACTORY getFactory(int rid) {
       return dao.GetModel<FACTORY,int>("Rid",rid);    
    }

    public CARD_TYPE getCardType(int rid)
    {
        return dao.GetModel<CARD_TYPE, int>("Rid", rid);
    }

    /// <summary>
    /// 轉化數據
    /// </summary>
    /// <param name="drow">MONTHLY_MONITOR 對應的數據行</param>
    /// <param name="newRow">頁面上對應的數據行</param>
    private void CopyMonth2View(DataRow drow, DataRow newRow)
    {        
          newRow["A"] = drow["ANumber"];
          newRow["A1"] = drow["A1Number"];
          newRow["B"] = drow["BNumber"];
          newRow["C"] = drow["CNumber"];
          newRow["D1"] = drow["D1Number"];
          newRow["D2"] = drow["D2Number"];
          newRow["E"] = drow["ENumber"];
          newRow["F"] = drow["FNumber"];
          newRow["totalXH"] = Convert.ToInt32(drow["BNumber"]) 
                        + Convert.ToInt32(drow["CNumber"]) + Convert.ToInt32(drow["D1Number"])
                        + Convert.ToInt32(drow["D2Number"]) + Convert.ToInt32(drow["ENumber"]);
          newRow["G"] = drow["GNumber"];
          newRow["G1"] = drow["G1Number"];
          newRow["H"] = drow["HNumber"];
          newRow["J"] = drow["JNumber"];
          newRow["K"] = drow["KNumber"];
          newRow["L"] = drow["LNumber"];
          newRow["Month"] =Convert.ToDateTime(drow["CMonth"]).ToString("yyyy/MM/dd");      
    }

    /// <summary>
    /// 根據修改C和J值之後的table計算數據
    /// </summary>
    /// <param name="dtbInput"></param>
    /// <param name="flag">0:不做任何修改 1:修改J值為0;2:計算J值</param>
    /// <returns></returns>
    public void getTableAfterChange(DataTable dtbInput,string flag )
    {
        string strYType = paramList(GlobalString.cardparam.NType);
        int xType = int.Parse(strYType.Substring(0, strYType.Length - 1));
        for (int j = 0; j < dtbInput.Rows.Count; j++)
        {
            DataRow drow = dtbInput.Rows[j];
            #region 修改需要變化的數據
            //計算A'的值
            if (j > 0)
            {
                drow["A1"] = dtbInput.Rows[j - 1]["K"];
                drow["A"] = dtbInput.Rows[j - 1]["G"];
            }
            drow["G"] = toInt(drow["A"]) + toInt(drow["F"]) - toInt(drow["TotalXH"]);
            //計算G'的值           
             drow["G1"] = getG1Value(dtbInput, j, drow["A1"]);            
            //如果G值小於0,H值為0
            if (toInt(dtbInput.Rows[j]["G"]) < 0)
            {
                drow["H"] = 0;
            }
            else
            {
                //計算H的值
                drow["H"] = getHValue(dtbInput, j);
            }
            //計算J值
            if (flag == "1")
                drow["J"] = 0;
            else if (flag == "2")
                drow["J"] = getJValue(dtbInput, j, xType, drow["G1"]);
            //計算K值 
            if (drow["G1"].ToString() == "無法計算" || drow["J"].ToString() == "無法計算")
            {
                drow["K"] = "無法計算";
            }
            else
                drow["K"] = toInt(drow["J"]) + toInt(drow["G1"]);
            //計算L檢核欄位的值
            drow["L"] = getLValue(drow["G1"], drow["J"], dtbInput, j);
            #endregion               
        }
    }

    public CARD_TYPE getCardtype(string affinity, string photo, string cardtype)
    {
        dirValues.Clear();
        dirValues.Add("AFFINITY", affinity);
        dirValues.Add("PHOTO", photo);
        dirValues.Add("TYPE", cardtype);
        DataTable cardTable = dao.GetList(SEL_CARD_TYPE, dirValues).Tables[0];
        CARD_TYPE cardType = dao.GetModelByDataRow<CARD_TYPE>(cardTable.Rows[0]);
        return cardType;
    }

    /// <summary>
    /// 保存數據
    /// </summary>
    /// <param name="baseInfoRow">卡種和廠商的消息</param>
    /// <param name="result"></param>
    /// <param name="xCode">X對應的ParamCode</param>
    public DataTable saveDate(CARD_TYPE cardModel,int persoRid,DataTable result,int xCode,int xValue) {
        //將頁面的table和批次的table數據綜合起來從新計算數據
        DataTable dtbResult = new DataTable();
        creatNewDataTable2(dtbResult);
       foreach(DataRow rdrow in result.Rows ){        
            dtbResult.Rows.Add(rdrow.ItemArray); 
        }
        string strN =  paramList(GlobalString.cardparam.NType);
        int N = int.Parse(strN.Substring(0,strN.Length-1));
        DateTime BeginMonth = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01"));
        dirValues.Clear();
        dirValues.Add("type", cardModel.TYPE);
        dirValues.Add("affinity", cardModel.AFFINITY);
        dirValues.Add("photo", cardModel.PHOTO);
        dirValues.Add("persoRid", persoRid);
        dirValues.Add("xCode", xCode);
        dirValues.Add("startMonth", BeginMonth.AddMonths(result.Rows.Count));
        dirValues.Add("endMonth", BeginMonth.AddMonths(16));
        DataTable dtbMonth = dao.GetList(SEL_MONTH_MONITOR, dirValues).Tables[0];        
        foreach (DataRow computeRow in  dtbMonth.Rows)
        {
            DataRow newRow = dtbResult.NewRow();
            CopyMonth2View(computeRow, newRow);
            dtbResult.Rows.Add(newRow);
        }
        getTableAfterChange(dtbResult, "0");
        DataTable dtblCompute = new DataTable();
        creatNewDataTable2(dtblCompute);
        foreach (DataRow row in dtbResult.Select("Month <='" + result.Rows[result.Rows.Count - 1]["Month"].ToString() + "'"))
        {
            dtblCompute.Rows.Add(row.ItemArray);
        }
        getTableAfterChange(dtbResult, "1");
        //將計算完的數據保存
        try
        {
            dao.OpenConnection();
            foreach (DataRow drow in dtbResult.Rows)
            {
                MONTHLY_MONITOR monthModel = new MONTHLY_MONITOR();
                monthModel.AFFINITY = cardModel.AFFINITY;
                monthModel.PHOTO = cardModel.PHOTO;
                monthModel.TYPE = cardModel.TYPE;
                monthModel.Name = cardModel.Name;
                monthModel.NType = N;
                monthModel.XType = xCode;
                monthModel.CMonth = Convert.ToDateTime(drow["Month"].ToString());
                monthModel.ANumber = toInt(drow["A"]);
                monthModel.A1Number = toInt(drow["A1"]);
                monthModel.BNumber = toInt(drow["B"]);
                monthModel.CNumber = toInt(drow["C"]);
                monthModel.D1Number = toInt(drow["D1"]);
                monthModel.D2Number = toInt(drow["D2"]);
                monthModel.ENumber = toInt(drow["E"]);
                monthModel.FNumber = toInt(drow["F"]);
                monthModel.GNumber = toInt(drow["G"]);
                monthModel.G1Number = toInt(drow["G1"]);
                monthModel.HNumber = Convert.ToDecimal(drow["H"]);
                monthModel.JNumber = toInt(drow["J"]);
                monthModel.KNumber = toInt(drow["K"]);
                monthModel.LNumber = Convert.ToDecimal(drow["L"]);
                monthModel.Perso_Factory_Rid = persoRid;
                dirValues.Clear();
                dirValues.Add("affinity", monthModel.AFFINITY);
                dirValues.Add("Photo", monthModel.PHOTO);
                dirValues.Add("type", monthModel.TYPE);
                dirValues.Add("perso_rid", monthModel.Perso_Factory_Rid);
                dirValues.Add("xType", monthModel.XType);
                dirValues.Add("cMonth", monthModel.CMonth);
                if (dao.Contains(CON_MONTH_DATA, dirValues))
                {
                    int rid = Convert.ToInt32(dao.GetList(SEL_MONTH_DATA, dirValues).Tables[0].Rows[0][0]);
                    monthModel.RID = rid;
                    dao.Update<MONTHLY_MONITOR>(monthModel, "RID");
                }
            }
            //事務提交
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
        return dtblCompute;
    }   

    /// <summary>
    /// 取L欄位的值
    /// </summary>
    /// <param name="G1"></param>
    /// <param name="J"></param>
    /// <param name="dtbStep1"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    private object getLValue(object G1,object J,DataTable dtbStep1,int rowIndex) {
    
        decimal lValue = 0.000000M;        
        try
        {
            if (G1.ToString() == "無法計算" || J.ToString() == "無法計算")
            {
                return "無法計算";
            }
            else {
                decimal G = Convert .ToDecimal(G1.ToString())+ Convert .ToDecimal(J.ToString());
                int checkRowIndex = rowIndex + 1;
                while (G > 0)
                {
                    //如果檢核的列超過最後一列,直接用當前庫存(G)除以最後一列的消耗數量(B+C+D1+D2+E)
                    if (checkRowIndex == dtbStep1.Rows.Count)
                    {
                        lValue = lValue + G / Convert.ToDecimal(dtbStep1.Rows[checkRowIndex - 1]["TotalXH"].ToString());
                        G = 0;
                    }
                    else
                    {
                        decimal totalXH = Convert.ToDecimal(dtbStep1.Rows[checkRowIndex]["TotalXH"].ToString());
                        if (G - totalXH > 0)
                            lValue += 1;//纍加檢核月數(H)
                        else
                            lValue = lValue + G / totalXH;//黨不滿一個月數的消耗時,加上計算滿足比率
                        G -= totalXH;
                    }
                    checkRowIndex++;
                }
            }
        }
        catch {
            return 999999999.9;        
        }
        return lValue;
    }

    /// <summary>
    /// 取G'欄位的值
    /// </summary>
    /// <param name="dtbStep1"></param>
    /// <param name="rowIndex"></param>
    /// <param name="A1"></param>
    /// <returns></returns>
    private object getG1Value(DataTable dtbStep1, int rowIndex,object A1)
    {
        int G1Value = 0;
        try
        {
            int A1Value = toInt(A1);
            int FValue = toInt(dtbStep1.Rows[rowIndex]["F"]);
            int XHValue = toInt(dtbStep1.Rows[rowIndex]["TotalXH"]);
            G1Value = A1Value + FValue - XHValue; 
        }
        catch
        {
            return "無法計算";
        }
        return G1Value;
    }

    /// <summary>
    /// 取得系統建議or人員調整採購到貨量(J)
    /// </summary>
    /// <param name="dtbStep1">第一階段獲取數據</param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    private int getJValue(DataTable dtbStep1, int rowIndex, int manageMonth, object G1)
    {
        int JValue = 0;
        try
        {
            for (int m = 1; m <= manageMonth; m++)
            {
                JValue += int.Parse(dtbStep1.Rows[rowIndex + m]["TotalXH"].ToString());
            }
            JValue -= Convert.ToInt32(G1);
            if (JValue < 0)
                JValue = 0;
        }
        catch
        {
            return 0;
        }
        return JValue;
    }

    /// <summary>
    /// 計算H檢核欄位的值
    /// </summary>
    /// <param name="dtbStep1"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    private decimal getHValue(DataTable dtbStep1,int rowIndex) {
        decimal hValue = 0.000000M;
        decimal G = Convert .ToDecimal(dtbStep1.Rows[rowIndex]["G"].ToString());
        try
        {
            int checkRowIndex = rowIndex + 1;
            while (G > 0)
            {
                //如果檢核的列超過最後一列,直接用當前庫存(G)除以最後一列的消耗數量(B+C+D1+D2+E)
                if (checkRowIndex == dtbStep1.Rows.Count)
                {
                    hValue = hValue + G / Convert.ToDecimal(dtbStep1.Rows[checkRowIndex - 1]["TotalXH"].ToString());
                    G = 0;
                }
                else
                {
                    decimal totalXH = Convert.ToDecimal(dtbStep1.Rows[checkRowIndex]["TotalXH"].ToString());
                    if (G - totalXH > 0)
                        hValue += 1;//纍加檢核月數(H)
                    else
                        hValue = hValue + G / totalXH;//黨不滿一個月數的消耗時,加上計算滿足比率
                    G -= totalXH;
                }
                checkRowIndex++;
            }
        }
        catch {
            return 999999999.9M;        
        }     
        return hValue;
    }

    /// <summary>
    /// 將object 轉化成 int
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private int toInt(object obj) {
         int result = 0;
         if (obj != null && !StringUtil.IsEmpty(obj.ToString()))
             result = int.Parse(obj.ToString().Replace(",",""));
         return result;  
    }

    /// <summary>
    /// 取參數表的值
    /// </summary>
    /// <param name="Paramcode"></param>
    /// <returns></returns>
    public string paramList(string Paramcode) {
        dirValues.Clear();
        dirValues.Add("Param", GlobalString.ParameterType.CardParam);
        dirValues.Add("Param_code", Paramcode);
        StringBuilder sql = new StringBuilder(SEL_PARAM);
        sql.Append(" and Param_Code  = @Param_code");
        DataSet dstParam = dao.GetList(sql.ToString(), dirValues);
        return dstParam.Tables[0].Rows[0]["Param_Name"].ToString();    
    } 

    /// <summary>
    /// 將model轉化成dataRow
    /// </summary>
    /// <param name="model"></param>
    /// <param name="drow"></param>
    //private void setModelToDataRow(Depository013Model model, DataRow drow)
    //{
    //    drow["A"] = model.Stock_Number;
    //    drow["A1"] = model.Stock_Number_1;
    //    drow["B"] = model.New_Number_B;
    //    drow["C"] = model.New_Number_C;
    //    drow["D1"] = model.GB_Number_D1;
    //    drow["D2"] = model.HB_Number_D2;
    //    drow["E"] = model.Change_Num_E;
    //    drow["TotalXH"] = model.TotalXH;
    //    drow["F"] = model.Income_Num_F;
    //    drow["G"] = model.Forecast_Stock_Num_G;
    //    drow["G1"] = model.Forecast_Stock_Num_G1; 
    //}

    /// <summary>
    /// 創建第二階段的數據表
    /// </summary>
    /// <param name="dtblSafeStockInfo"></param>
    public void creatNewDataTable2(DataTable dtblSafeStockInfo)
    {

        //可用庫存量
        dtblSafeStockInfo.Columns.Add("A");
        //修正庫存量
        dtblSafeStockInfo.Columns.Add("A1");
        //過去X月平均新卡數
        dtblSafeStockInfo.Columns.Add("B");
        //新進件調整
        dtblSafeStockInfo.Columns.Add("C");
        //過去X月平均掛補數
        dtblSafeStockInfo.Columns.Add("D1");
        //過去X月平均毀補數
        dtblSafeStockInfo.Columns.Add("D2");
        //該月換卡耗用量
        dtblSafeStockInfo.Columns.Add("E");
        //B+C+D1+D2+E
        dtblSafeStockInfo.Columns.Add("TotalXH");
        //已下單未到貨數
        dtblSafeStockInfo.Columns.Add("F");
        //預估月底庫存
        dtblSafeStockInfo.Columns.Add("G");
        //預估月底庫存
        dtblSafeStockInfo.Columns.Add("G1");
        dtblSafeStockInfo.Columns.Add("H");
        dtblSafeStockInfo.Columns.Add("J");
        dtblSafeStockInfo.Columns.Add("K");
        dtblSafeStockInfo.Columns.Add("L");
        dtblSafeStockInfo.Columns.Add("Month");
    } 
  
}