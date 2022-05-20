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
/// Depository012BL 的摘要描述
/// </summary>
public class Depository012BL : BaseLogic
{
 

    #region SQL語句
    public const string SEL_FACTORY_ALL = "SELECT RID,Factory_ShortName_CN "
                        + "FROM FACTORY "
                        + "WHERE RST = 'A' AND Is_Perso = 'Y' ";

    public const string SEL_STOCK_CARDTYPE = "SELECT stocks.Stocks_Number, factory.Factory_ShortName_CN, type.Name,"
                        + " type.TYPE, type.AFFINITY, type.PHOTO, stocks.Stock_Date,"
                        + " stocks.Perso_Factory_RID, stocks.CardType_RID"
                        + " FROM CARDTYPE_STOCKS  AS stocks"
                        + " LEFT OUTER JOIN CARD_TYPE AS type ON stocks.CardType_RID = type.RID"
                        + " LEFT OUTER JOIN FACTORY AS factory ON stocks.Perso_Factory_RID = factory.RID"
                        + " where stocks.rst = 'A'";

    public const string SEL_CARDTYPE_PERSO_SPECIAL = "SELECT PC.* FROM PERSO_CARDTYPE PC WHERE PC.rst='A' and PC.CardType_RID = @cardtype_rid and PC.Percentage_Number = '1' ORDER BY PC.Priority desc";
    public const string SEL_CARDTYPE_PERSO_SPECIAL_2 = "SELECT PC.* FROM PERSO_CARDTYPE PC WHERE PC.rst='A' and PC.CardType_RID = @cardtype_rid and PC.Percentage_Number = '2' ORDER BY PC.Priority";
    public const string SEL_CARDTYPE_PERSO = "SELECT PC.* FROM PERSO_CARDTYPE PC WHERE PC.RST = 'A' AND PC.CardType_RID = @cardtype_rid and pc.Base_Special = '1'";
    
    public const string SEL_CARD_TYPE = "SELECT * FROM CARD_TYPE WHERE RST='A' AND TYPE = @TYPE  AND AFFINITY = @AFFINITY AND PHOTO = @PHOTO";
     
    public const string SEL_SAFE_MANAGE_NUMBER = "SELECT NUMBER FROM SAFE_DAY_MANAGE where Perso_Factory_RID = @PersoRid "
                        + " and CardType_RID =  @cardRid and CONVERT(VARCHAR(10), Date_Time, 111) = @CheckDate ";
       
    public const string CON_CHECK_DATE = "SELECT count(*) from cardtype_stocks where rst = 'A' and Stock_Date = @CheckDate";

    public const string CON_WORK_DATE = "SELECT count(*) from WORK_DATE where rst = 'A' and CONVERT(VARCHAR(10), Date_Time, 111) = @CheckDate and Is_WorkDay = 'Y'";

    public const string CON_SAFE_DATA = "SELECT count(*) from SAFE_DAY_MANAGE where Perso_factory_RID = @perso_rid and cardType_RID = @card_rid and date_time = @datetime";

    public const string SEL_PARAM = "SELECT  Param_Name FROM PARAM WHERE ParamType_Code = @Param ";

    public const string SEL_NEXT12_MONTH = "select * from  FORE_CHANGE_CARD WHERE Change_Date >@begainMonth and Change_Date <@endMonth";

    public const string SEL_ORDER_DEPOSITORY = "select isnull(sum(order_form_detail.number),0) as number,cardtype_rid,Delivery_Address_RID,sum(a.stock_Number) as stock_Number from order_form_detail"
        +" LEFT JOIN  ORDER_FORM ON ORDER_FORM.OrderForm_RID = order_form_detail.OrderForm_RID"
        +" left join "
        +" (select isnull(sum(stock_Number),0) as stock_Number ,orderform_detail_rid from depository_stock"
        +" where depository_stock.Income_Date<=@check_date"
        +" group by orderform_detail_rid) as a"
        +" on a.orderform_detail_rid=order_form_detail.orderform_detail_rid"
        + " WHERE (ORDER_FORM.Pass_Status = 4) and ORDER_FORM.Pass_Date<=@check_date and order_form_detail.case_status!='Y'"
        +" group by cardtype_rid,Delivery_Address_RID";
    //前十個工作日
    public const string SEL_BEFORE_DATE = "select min(date_time) from( select top 10 * from work_date as table1 where is_workDay='Y' and date_time<=@day order by date_time desc)a";
#endregion 
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Depository012BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    private DateTime getTenDaysBefore(DateTime today) {
        dirValues.Clear();
        dirValues.Add("day", today);
        DateTime tenBefore = today.AddDays(-10);
        DataTable workDayTbl = dao.GetList(SEL_BEFORE_DATE, dirValues).Tables[0];
        if (workDayTbl != null) {
            tenBefore = Convert.ToDateTime(workDayTbl.Rows[0][0]);
        }
        return tenBefore;
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
    /// 判斷是否為日結日
    /// </summary>
    /// <param name="strBudgetID">預算簽呈ID</param>
    /// <returns>true:存在 false:不存在</returns>
    public bool isCheckDate(string strCheckDate)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("CheckDate", Convert.ToDateTime(strCheckDate));
            return dao.Contains(CON_CHECK_DATE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
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
    ///  保存採購量
    /// </summary>
    /// <param name="dtbl"></param>
    /// <param name="date"></param>
    public void save(DataTable dtbl, string date) {
        DateTime dateTime = DateTime.Parse(date);
        foreach (DataRow drow in dtbl.Rows)
        {
            SAFE_DAY_MANAGE sdmModel = new SAFE_DAY_MANAGE();
            sdmModel.CardType_RID = int.Parse(drow["Card_RID"].ToString());
            sdmModel.Perso_Factory_RID = int.Parse(drow["Perso_RID"].ToString());
            sdmModel.Number = int.Parse(drow["Number"].ToString());
            sdmModel.Date_Time = dateTime;
            dirValues.Clear();
            dirValues.Add("card_rid", sdmModel.CardType_RID);
            dirValues.Add("perso_rid", sdmModel.Perso_Factory_RID);
            dirValues.Add("datetime", dateTime);

            if (dao.Contains(CON_SAFE_DATA, dirValues))
            {
                dao.ExecuteNonQuery("update  SAFE_DAY_MANAGE set number = "+sdmModel.Number+" where Perso_factory_RID = @perso_rid and cardType_RID = @card_rid and date_time = @datetime" , dirValues);
               
            }
            else {
                dao.Add<SAFE_DAY_MANAGE>(sdmModel, "RID");
            }            
        }
    
    }

    /// <summary>
    /// 主要計算程序
    /// </summary>
    /// <param name="searchInput"></param>
    /// <returns></returns>
    public DataTable List(Dictionary<string, object> searchInput,DataTable oldTable)
    {
        //取畫面輸入的日結日       
        DateTime today = DateTime.Parse(searchInput["txtCheckDate"].ToString());
        //當前月開始
        string firstDayCurrMonth = today.ToString("yyyy-MM-01");
        //半年前
        string halfYearBefore = today.AddMonths(-6).ToString("yyyy-MM-01");
        //十個工作日前
        DateTime TenWorkDayBefore = getTenDaysBefore(today);
         
        //創建換卡並拆分後的緩存table
        DataTable foreChangeTable = SplitToPerso(today);
        
        //構建滿足UI的table
        DataTable dtblSafeStockInfo = new DataTable();           
        creatNewDataTable(dtblSafeStockInfo);
        //取值類型為當日或者前十日
        string safeType = searchInput["radSaveType"].ToString().Trim();  

        //取換卡百分比 
        string  strPercent = getSafeParam(GlobalString.cardparam.Percent);        
        decimal changePercent = Convert.ToDecimal(strPercent.Substring(0, strPercent.Length - 1)) / 100; 
        try
        {
            //根據查詢條件取庫存表中日結卡種和廠商的組合記錄            
            DataSet dstSafeStockInfo = getAllUseableDateByInput(searchInput);          
           
            //取半年前到查詢日期的所有小計檔記錄
            DataTable recordTbl = dao.GetList("select * from SUBTOTAL_IMPORT where date_time>='" + halfYearBefore + "' and date_time<='" + today.ToString("yyyy-MM-dd") + "'").Tables[0];           
           
            //取所有的訂單及相應的入庫記錄
            dirValues.Clear();
            dirValues.Add("check_date",today);            
            DataTable orderNumberTb = dao.GetList(SEL_ORDER_DEPOSITORY, dirValues).Tables[0];

            //針對查出來的卡種和廠商記錄查詢並計算UI後10個欖位的值
            foreach (DataRow drow in dstSafeStockInfo.Tables[0].Rows)
            {
                string type = drow["TYPE"].ToString();//card Type
                string affinity = drow["AFFINITY"].ToString(); //AFFINITY
                string photo = drow["Photo"].ToString(); //Photo
                int persoRid = int.Parse(drow["Perso_Factory_RID"].ToString()); //Perso_Factory_RID
                int cardRid = int.Parse(drow["CardType_RID"].ToString()); //CardType_RID

                DataRow resultRow = dtblSafeStockInfo.NewRow();
                copyRowToRow(resultRow, drow);

                //當天之前的已下單未到貨數
                dirValues.Clear();
                dirValues.Add("PersoRid", persoRid);
                dirValues.Add("cardRid", cardRid);
                DataRow[] rows0 = orderNumberTb.Select("cardtype_rid='" + dirValues["cardRid"] + "' and Delivery_Address_RID = '" + dirValues["PersoRid"] + "'");
                if (rows0.Length == 1)
                {
                    int orderNumber = 0;
                    int stockNumber = 0;
                    if (!StringUtil.IsEmpty(rows0[0]["number"].ToString() ))
                        orderNumber = Convert.ToInt32(rows0[0]["number"]);
                    if (!StringUtil.IsEmpty(rows0[0]["stock_Number"].ToString()))
                        stockNumber = Convert.ToInt32(rows0[0]["stock_Number"]);
                    resultRow["not_Income"] =Math.Max(orderNumber- stockNumber,0) ;
                }
                else
                {
                    resultRow["not_Income"] = 0;
                }
               
                //採購數量
                int safeBuyNum = getBuyNum(persoRid,cardRid,oldTable);
                resultRow["Buy_Num"] = safeBuyNum;

                
                dirValues.Add("type", type);
                dirValues.Add("affinity", affinity);
                dirValues.Add("photo", photo);
                int prechangeNum = getNextMonthChangeNum(foreChangeTable, dirValues,today);
                //月換卡耗用量
                resultRow["nextMonth_CNumber"] = prechangeNum;
                //次6月換卡耗用量 
                resultRow["next6Months_CNumber"] = getNext6MonthChangeNum(foreChangeTable, dirValues, today);
                //次12月換卡耗用量  
                resultRow["next12Months_CNumber"] = getNext12MonthChangeNum(foreChangeTable, dirValues, today);
                      
                //前半年掛補數            
                int GBNumber = 0;
                DataRow[] rows = recordTbl.Select("affinity='" + dirValues["affinity"] + "' and photo='" + dirValues["photo"]
                    + "' and type='" + dirValues["type"]
                    + "' and perso_factory_rid='" + dirValues["PersoRid"]
                    + "' and Date_Time<='" + firstDayCurrMonth + "' and Date_Time>='" + halfYearBefore + "' and action='2'");
                foreach (DataRow drow1 in rows)
                {
                    GBNumber += Convert.ToInt32(drow1["Number"]);
                }              
                resultRow["halfYear_GBNumber"] =Convert.ToInt32(Math.Ceiling(GBNumber / 6M));
                //前半年毀補數   
                int HBNumber = 0;                
                DataRow[] rows2 = recordTbl.Select("affinity='" + dirValues["affinity"] + "' and photo='" + dirValues["photo"]
                    + "' and type='" + dirValues["type"]
                    + "' and perso_factory_rid='" + dirValues["PersoRid"]
                    +"' and Date_Time<='" + firstDayCurrMonth + "' and Date_Time>='" + halfYearBefore + "' and action='3'");
                foreach (DataRow drow2 in rows2)
                {
                    HBNumber += Convert.ToInt32(drow2["Number"]);
                }
                resultRow["halfYear_HBNumber"] =Convert.ToInt32(Math.Ceiling( HBNumber / 6M));
                //每日新進件
                int todayNew = 0;
                DataRow[] rows3 = recordTbl.Select("affinity='" + dirValues["affinity"] + "' and photo='" + dirValues["photo"]
                    + "' and type='" + dirValues["type"]
                    + "' and perso_factory_rid='" + dirValues["PersoRid"]
                    +"' and Date_Time='" + today.ToString("yyyy-MM-dd") + "' and action='1'");
                foreach (DataRow drow3 in rows3)
                {
                    todayNew += Convert.ToInt32(drow3["Number"]);
                }
                resultRow["today_NewNumber"] = todayNew;

                //前十日平均新進件
                int tenDayAvgNum = 0;
                DataRow[] rows4 = recordTbl.Select("affinity='" + dirValues["affinity"] + "' and photo='" + dirValues["photo"]
                    + "' and type='" + dirValues["type"]
                    + "' and perso_factory_rid='" + dirValues["PersoRid"]
                    +"' and Date_Time>='" + TenWorkDayBefore.ToString("yyyy-MM-dd") + "' and Date_Time<='" + today.ToString("yyyy-MM-dd") + "' and action='1'");
                foreach (DataRow drow4 in rows4)
                {
                    tenDayAvgNum += Convert.ToInt32(drow4["Number"]);
                }
                tenDayAvgNum = Convert.ToInt32(Math.Ceiling(tenDayAvgNum / 10M));
                resultRow["tenDay_NewNumber"] =tenDayAvgNum;

                
                DataRow[] rows5 = recordTbl.Select("affinity='" + dirValues["affinity"] + "' and photo='" + dirValues["photo"]
                    + "' and type='" + dirValues["type"]
                    + "' and perso_factory_rid='" + dirValues["PersoRid"]
                    +"' and Date_Time>='" + firstDayCurrMonth + "' and Date_Time<='" + today.ToString("yyyy-MM-dd") + "' and action='5'");
                //已換卡數
                int changeNum = 0;                
                foreach (DataRow drow5 in rows5)
                {
                   changeNum  += Convert.ToInt32(drow5["Number"]);
                }
                //預計可用月數（當日）                
                //預計可用月數（前十日）
                if ((prechangeNum == 0) || ((prechangeNum != 0) && (changeNum / prechangeNum) >=  changePercent)) {
                    resultRow["expectMonth_Today"] = getUseableMonthB(resultRow, safeBuyNum, todayNew);
                    resultRow["expectMonth_Ten"] = getUseableMonthB(resultRow, safeBuyNum, tenDayAvgNum); 
                }
                else if (((prechangeNum != 0) && (changeNum / prechangeNum) < changePercent)) {
                    resultRow["expectMonth_Today"] = getUseableMonthA(resultRow, safeBuyNum, todayNew);
                    resultRow["expectMonth_Ten"] = getUseableMonthA(resultRow, safeBuyNum, tenDayAvgNum);
                }
                //根據頁面上選擇的安全類型和安全月份決定是否現實在畫面上
                if(!StringUtil.IsEmpty(searchInput["txtMonth"].ToString())){
                    if(safeType == "0"&&Convert.ToDecimal(resultRow["expectMonth_Today"]) <= Convert.ToDecimal(searchInput["txtMonth"]))
                         dtblSafeStockInfo.Rows.Add(resultRow);
                    if(safeType == "1"&&Convert.ToDecimal(resultRow["expectMonth_Ten"]) <= Convert.ToDecimal(searchInput["txtMonth"]))
                        dtblSafeStockInfo.Rows.Add(resultRow); 
                }else{                
                    dtblSafeStockInfo.Rows.Add(resultRow);                
                }
                             
            }
        }
        catch (AlertException ex) {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        } 
        DataView dv = new DataView(dtblSafeStockInfo);
        if (safeType == "0")
            dv.Sort = "expectMonth_Today";
        if (safeType == "1")
            dv.Sort = "expectMonth_Ten";        
        DataTable dt2 = dv.ToTable();
        return dt2;
        
    }

    /// <summary>
    /// 取次月換卡數
    /// </summary>
    /// <param name="dtbl"></param>
    /// <param name="dir"></param>
    /// <param name="currentDay"></param>
    /// <returns></returns>
    public int getNextMonthChangeNum(DataTable dtbl, Dictionary<string, object> dir,DateTime currentDay)
    {
        int number = 0;
        string month = currentDay.AddMonths(1).ToString("yyyyMM");
        DataRow[] rows = dtbl.Select("Perso_rid='" + dir["PersoRid"] + "' and affinity='" + dir["affinity"] + "' and photo='" + dir["photo"]
            + "' and type='" + dir["type"] + "' and Change_Month='" + month+"'");
        foreach(DataRow drow in rows){
            number += Convert.ToInt32(drow["Number"]);
        }
        return number;    
    }

    /// <summary>
    /// 取次六月換卡數量
    /// </summary>
    /// <param name="dtbl"></param>
    /// <param name="dir"></param>
    /// <param name="currentDay"></param>
    /// <returns></returns>
    public int getNext6MonthChangeNum(DataTable dtbl, Dictionary<string, object> dir, DateTime currentDay)
    {
        int number = 0;
        string month1 = currentDay.AddMonths(1).ToString("yyyyMM");
        string month6 = currentDay.AddMonths(6).ToString("yyyyMM");
        DataRow[] rows = dtbl.Select("Perso_rid='" + dir["PersoRid"] + "' and affinity='" + dir["affinity"] + "' and photo='" + dir["photo"]
            + "' and type='" + dir["type"] + "' and Change_Month>='" + month1 + "' and Change_Month<='" + month6+"'");
        foreach (DataRow drow in rows)
        {
            number += Convert.ToInt32(drow["Number"]);
        }
        return number;
    }

    /// <summary>
    /// 取次12月換卡數量
    /// </summary>
    /// <param name="dtbl"></param>
    /// <param name="dir"></param>
    /// <param name="currentDay"></param>
    /// <returns></returns>
    public int getNext12MonthChangeNum(DataTable dtbl, Dictionary<string, object> dir, DateTime currentDay)
    {
        int number = 0;
        string month1 = currentDay.AddMonths(1).ToString("yyyyMM");
        string month12 = currentDay.AddMonths(12).ToString("yyyyMM");
        DataRow[] rows = dtbl.Select("Perso_rid='" + dir["PersoRid"] + "' and affinity='" + dir["affinity"] + "' and photo='" + dir["photo"]
            + "' and type='" + dir["type"] + "' and Change_Month>='" + month1 + "' and Change_Month<='" + month12+"'");
        foreach (DataRow drow in rows)
        {
            number += Convert.ToInt32(drow["Number"]);
        }
        return number;
    }

    /// <summary>
    /// 將每月換卡檔換卡
    /// </summary>
    /// <returns></returns>
    public DataTable SplitToPerso(DateTime searchTime) {
        DataTable perso_fore_change = new DataTable();
        perso_fore_change.Columns.Add("Type");
        perso_fore_change.Columns.Add("Affinity");
        perso_fore_change.Columns.Add("Photo");
        perso_fore_change.Columns.Add("Perso_Rid");
        perso_fore_change.Columns.Add("Number");
        perso_fore_change.Columns.Add("Change_Month");
        dirValues.Clear();
        dirValues.Add("begainMonth", searchTime.ToString("yyyyMM"));
        dirValues.Add("endMonth", searchTime.AddMonths(12).ToString("yyyyMM"));
        DataTable dtbl = dao.GetList(SEL_NEXT12_MONTH,dirValues).Tables[0];
        foreach (DataRow drow in dtbl.Rows)
        {
            FORE_CHANGE_CARD foreChangeCard = dao.GetModelByDataRow<FORE_CHANGE_CARD>(drow);    //原始換卡model

            CARD_TYPE card = getCardtype(foreChangeCard.Affinity, foreChangeCard.Photo, foreChangeCard.Type);//卡種對象
            if (card == null)
                continue;

            int cardRid = card.RID;
            //if(如果該卡種有換卡版面),將卡種替換成換卡版面
            if (card.Change_Space_RID != 0)
            {
                CARD_TYPE changeCard = dao.GetModel<CARD_TYPE, int>("RID", card.Change_Space_RID);
                drow["Photo"] = changeCard.PHOTO;
                drow["Affinity"] = changeCard.AFFINITY;
                drow["Type"] = changeCard.TYPE;
                cardRid = changeCard.RID;
            }
            //else if(如果該卡種有替換卡版面),將卡種替換成替換卡版面
            else if (card.Replace_Space_RID != 0)
            {
                CARD_TYPE replaceCard = dao.GetModel<CARD_TYPE, int>("RID", card.Replace_Space_RID);
                drow["Photo"] = replaceCard.PHOTO;
                drow["Affinity"] = replaceCard.AFFINITY;
                drow["Type"] = replaceCard.TYPE;
                cardRid = replaceCard.RID;
            }
            //else (不做任何替換)     
            #region 拆分卡種到perso廠
            dirValues.Clear();
            dirValues.Add("cardtype_rid", cardRid);
            DataSet dsCARDTYPE_PERSO = dao.GetList(SEL_CARDTYPE_PERSO_SPECIAL, dirValues);
            DataSet dsCARDTYPE_PERSO_2 = dao.GetList(SEL_CARDTYPE_PERSO_SPECIAL_2, dirValues);
            long totalNumber = foreChangeCard.Number;
            //if(有比率分配),將卡種類按照比率進行分配
            if (dsCARDTYPE_PERSO.Tables[0] != null && dsCARDTYPE_PERSO.Tables[0].Rows.Count > 0)
            {
                long leftNumber = totalNumber;
                foreach (DataRow persoRow in dsCARDTYPE_PERSO.Tables[0].Rows)
                {
                    DataRow pero_fore_Row = perso_fore_change.NewRow();
                    decimal percent = Convert.ToDecimal(persoRow["value"]) / 100M;
                    if (persoRow["Priority"].ToString() == "1")
                        pero_fore_Row["Number"] = leftNumber;
                    else
                        pero_fore_Row["Number"] = Convert.ToInt64 (Math.Floor(totalNumber * percent));                  
                    pero_fore_Row["Perso_Rid"] = persoRow["Factory_RID"];
                    pero_fore_Row["Photo"] = drow["Photo"];
                    pero_fore_Row["Affinity"] = drow["Affinity"];
                    pero_fore_Row["Type"] = drow["Type"];
                    pero_fore_Row["Change_Month"] = drow["Change_Date"];
                    perso_fore_change.Rows.Add(pero_fore_Row);
                    leftNumber -= Convert.ToInt64(pero_fore_Row["Number"]);
                }

            }
            else if (dsCARDTYPE_PERSO_2.Tables[0] != null && dsCARDTYPE_PERSO_2.Tables[0].Rows.Count > 0)
            {
                long leftNumber = totalNumber;
                foreach (DataRow persoRow in dsCARDTYPE_PERSO_2.Tables[0].Rows)
                {
                    DataRow pero_fore_Row = perso_fore_change.NewRow();
                    if (persoRow["value"].ToString() == "0")
                        pero_fore_Row["Number"] = leftNumber;
                    else
                        pero_fore_Row["Number"] = Math.Min(Convert.ToInt32(persoRow["value"]), leftNumber);
                    pero_fore_Row["Perso_Rid"] = Convert.ToInt32(persoRow["Factory_RID"]);
                    pero_fore_Row["Photo"] = drow["Photo"].ToString();
                    pero_fore_Row["Affinity"] = drow["Affinity"].ToString();
                    pero_fore_Row["Type"] = drow["Type"].ToString();
                    pero_fore_Row["Change_Month"] = drow["Change_Date"].ToString();
                    perso_fore_change.Rows.Add(pero_fore_Row);
                    leftNumber -= Convert.ToInt64(pero_fore_Row["Number"]);
                }
            }
            else
            {  //else 將卡種直接分配到基本廠
               
                DataTable dtbBasePerso = dao.GetList(SEL_CARDTYPE_PERSO, dirValues).Tables[0];
                if (dtbBasePerso != null && dtbBasePerso.Rows.Count > 0)
                {
                    DataRow pero_fore_Row = perso_fore_change.NewRow();
                    pero_fore_Row["Number"] = totalNumber;
                    pero_fore_Row["Photo"] = drow["Photo"];
                    pero_fore_Row["Affinity"] = drow["Affinity"];
                    pero_fore_Row["Type"] = drow["Type"];
                    pero_fore_Row["Change_Month"] = drow["Change_Date"];
                    pero_fore_Row["Perso_Rid"] = dtbBasePerso.Rows[0]["Factory_RID"];
                    perso_fore_change.Rows.Add(pero_fore_Row);
                }
            }
            #endregion
        }
        return perso_fore_change;
    }

    public CARD_TYPE getCardtype(string affinity, string photo, string cardtype)
    {
        dirValues.Clear();
        dirValues.Add("AFFINITY", affinity);
        dirValues.Add("PHOTO", photo);
        dirValues.Add("TYPE", cardtype);
        DataTable cardTable = dao.GetList(SEL_CARD_TYPE, dirValues).Tables[0];
        if (cardTable.Rows.Count == 0)
            return null;
        CARD_TYPE cardType = dao.GetModelByDataRow<CARD_TYPE>(cardTable.Rows[0]);
        return cardType;
    }

    /// <summary>
    /// 取資料庫中緩存的採購量
    /// </summary>
    /// <param name="persoRid"></param>
    /// <param name="cardRid"></param>
    /// <param name="oldTable"></param>
    /// <returns></returns>
    public int getBuyNum(int persoRid, int cardRid, DataTable oldTable)
    {
        int Number = 0;
        DataRow[] drow =  oldTable.Select("Perso_RID ='" + persoRid + "' and Card_RID ='" + cardRid+"'");
       for (int i = 0; i < drow.Length; i++) {
           Number = Convert.ToInt32(drow[i]["Number"]);
       }
       return Number;    
    }

    /// <summary>
    /// 取CardParam得參數值（換卡白分比，安全庫存月數）
    /// </summary>
    /// <param name="Paramcode"></param>
    /// <returns></returns>
    public string getSafeParam(string Paramcode)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Param", GlobalString.ParameterType.CardParam);
            dirValues.Add("Param_code", Paramcode);
            StringBuilder sql = new StringBuilder(SEL_PARAM);
            sql.Append(" and Param_Code  = @Param_code");
            DataSet dstParam = dao.GetList(sql.ToString(), dirValues);
            return dstParam.Tables[0].Rows[0]["Param_Name"].ToString();
        }
        catch { 
            return "erro";
        }            
    }

    /// <summary>
    /// 取採購數量
    /// </summary>
    /// <param name="dirValues"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private int getSafeManageNum(Dictionary<string, object> dirValues,string time)
    {
        dirValues.Add("CheckDate", time);
        DataSet dst = dao.GetList(SEL_SAFE_MANAGE_NUMBER, dirValues);
        dirValues.Remove("CheckDate");
        if (dst.Tables[0].Rows.Count > 0 )
        {
            return int.Parse(dst.Tables[0].Rows[0][0].ToString());
        }
        else {
            return 0;
        }  
    }

    /// <summary>
    /// 公式B
    /// </summary>
    /// <param name="resultRow"></param>
    /// <param name="buyNumber"></param>
    /// <param name="avgNewNum"></param>                                                                      
    private decimal getUseableMonthB(DataRow resultRow,int buyNumber,int avgNewNum) {
        //string colName = "expectMonth_Today";
        //if(safeType == "1")
        //    colName = "expectMonth_Ten";
        int member = (valueOf(resultRow["stockNumber"]) + valueOf(resultRow["not_Income"]) + buyNumber);// 分子
        int divitor = ((avgNewNum * 22) + (valueOf(resultRow["next6Months_CNumber"]) - valueOf(resultRow["nextMonth_CNumber"])) / 5 + valueOf(resultRow[
            "halfYear_GBNumber"]) + valueOf(resultRow["halfYear_HBNumber"]));//分母
        if (divitor == 0)
            return 999999999.9M;
        else
            return Math.Round(member / Convert.ToDecimal(divitor),1);    
    }
 
    /// <summary>
    /// 公式A
    /// </summary>
    /// <param name="resultRow"></param>
    /// <param name="buyNumber"></param>
    /// <param name="AVGNewNum"></param>
    private decimal getUseableMonthA(DataRow resultRow, int buyNumber,int AVGNewNum)
    {
        //string colName = "expectMonth_Today";
        //if (safeType == "1")
        //    colName = "expectMonth_Ten";        
        int member = (valueOf(resultRow["stockNumber"]) + valueOf(resultRow["not_Income"]) + buyNumber - valueOf(resultRow["nextMonth_CNumber"]));// 分子
        int divitor = ((AVGNewNum * 22) + (valueOf(resultRow["next6Months_CNumber"])) / 6 + valueOf(resultRow[
            "halfYear_GBNumber"]) + valueOf(resultRow["halfYear_HBNumber"]));//分母
        if (divitor == 0)
            return 999999999.9M;
        else
            return Math.Round(member / Convert.ToDecimal(divitor),1);  
    
    }

    /// <summary>
    /// 根據查詢條件取庫存表中適當資料
    /// </summary>
    /// <param name="searchInput"></param>
    /// <returns></returns>
    private DataSet getAllUseableDateByInput(Dictionary<string, object> searchInput)
    {
        StringBuilder stbCommand = new StringBuilder(SEL_STOCK_CARDTYPE);        
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtCheckDate"].ToString().Trim()))
            {
                stbWhere.Append(" and stocks.Stock_Date = @stock_date");
                dirValues.Add("stock_date", searchInput["txtCheckDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropPerso"].ToString().Trim()))
            {
                stbWhere.Append(" and  stocks.Perso_Factory_RID = @PersoRid");
                dirValues.Add("PersoRid", int.Parse(searchInput["dropPerso"].ToString().Trim()));
            }
            if (!StringUtil.IsEmpty(searchInput["txtCardType"].ToString().Trim()))
            {
                stbWhere.Append(" and type.TYPE = @cardType");
                dirValues.Add("cardType", searchInput["txtCardType"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtAffinity"].ToString().Trim()))
            {
                stbWhere.Append(" and type.AFFINITY = @Affinity");
                dirValues.Add("Affinity", searchInput["txtAffinity"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtPhoto"].ToString().Trim()))
            {
                stbWhere.Append(" and type.Photo = @Photo");
                dirValues.Add("Photo", searchInput["txtPhoto"].ToString().Trim() );
            }
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" and type.Name like @name");
                dirValues.Add("name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }           
        }
        //執行SQL語句
        DataSet dstSafeStockInfo = null;
        DateTime today = DateTime.Now;   
        dstSafeStockInfo = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        return  dstSafeStockInfo;   
   }

    /// <summary>
    /// 構建新table,滿足畫面需求
    /// </summary>
    /// <param name="dtblSafeStockInfo"></param>
    private void creatNewDataTable(DataTable dtblSafeStockInfo)
    { 
        dtblSafeStockInfo.Columns.Add("persoName");
        dtblSafeStockInfo.Columns.Add("perso_Rid"); 
        dtblSafeStockInfo.Columns.Add("cardType");
        dtblSafeStockInfo.Columns.Add("cardType_Rid"); 
        dtblSafeStockInfo.Columns.Add("affinity");     
        dtblSafeStockInfo.Columns.Add("photo");
        //版面簡稱
        dtblSafeStockInfo.Columns.Add("name");
        //採購數量
        dtblSafeStockInfo.Columns.Add("Buy_Num");
        //可用庫存量
        dtblSafeStockInfo.Columns.Add("stockNumber");
        //已下單未到貨數
        dtblSafeStockInfo.Columns.Add("not_Income");
        //該月換卡耗用量
        dtblSafeStockInfo.Columns.Add("nextMonth_CNumber");
        //次6月換卡數
        dtblSafeStockInfo.Columns.Add("next6Months_CNumber");
        //次12月換卡數
        dtblSafeStockInfo.Columns.Add("next12Months_CNumber");
        //前半年月平均掛補數
        dtblSafeStockInfo.Columns.Add("halfYear_GBNumber");
        //前半年月平均毀補數
        dtblSafeStockInfo.Columns.Add("halfYear_HBNumber");
        //當日新卡數
        dtblSafeStockInfo.Columns.Add("today_NewNumber");
        //前十日平均新卡數
        dtblSafeStockInfo.Columns.Add("tenDay_NewNumber");
        //預計可用月數（當日）
         dtblSafeStockInfo.Columns.Add("expectMonth_Today",typeof(decimal));
        //預計可用月數（前十日）
         dtblSafeStockInfo.Columns.Add("expectMonth_Ten", typeof(decimal));  
    }

    /// <summary>
    /// 複製資料
    /// </summary>
    /// <param name="resultRow"></param>
    /// <param name="drow"></param>
    private void copyRowToRow(DataRow resultRow, DataRow drow)
    {
        resultRow["persoName"] = drow["Factory_ShortName_CN"];
        resultRow["cardType"] = drow["TYPE"];
        resultRow["affinity"] = drow["AFFINITY"];
        resultRow["photo"] = drow["Photo"];
        resultRow["name"] = drow["Name"];
        resultRow["stockNumber"] = drow["Stocks_Number"];
        resultRow["perso_Rid"] = drow["Perso_Factory_RID"];
        resultRow["cardType_Rid"] = drow["CardType_RID"];
    }

    public void inputDateTotemp(DataTable dtb,string time) {       
        try
        {
            dao.OpenConnection();
            
            dao.ExecuteNonQuery("delete RPT_Depository012 where RCT <"+DateTime.Now.ToString("yyyyMMdd000000"));
            foreach (DataRow drow in dtb.Rows)
            {
                dao.ExecuteNonQuery("insert into RPT_Depository012"
             + "(rid, Factory_shortName_CN, card_type, affinity, photo,Name,Stock_Num,Not_Income_Num, change_Num,Next_6Month_Change_Num,"
             + "Next_12Month_Change_Num, HB_Num, GB_Num,New_num, TenDays_New_Num_AVG, Buy_Num, Totay_Pre_Month, TenDays_Per_Month,RCT) VALUES (" + Convert.ToInt32(drow["rowId"]) + ",'" 
             + drow["persoName"].ToString()+"','"
             + drow["cardType"].ToString() +"','"
             + drow["affinity"].ToString()+"','"
             + drow["photo"].ToString()+"','"
             + drow["name"].ToString() +"',"
             + Convert.ToInt32(drow["stockNumber"])+","
             + Convert.ToInt32(drow["not_Income"])+","
             + Convert.ToInt32(drow["nextMonth_CNumber"])+","
             + Convert.ToInt32(drow["next6Months_CNumber"] )+ ","
             + Convert.ToInt32(drow["next12Months_CNumber"])+","
             +  Convert.ToInt32(drow["halfYear_HBNumber"]) + ","
             + Convert.ToInt32(drow["halfYear_GBNumber"])+","
             + Convert.ToInt32( drow["today_NewNumber"]) + ","
             + Convert.ToInt32(drow["tenDay_NewNumber"])+","
             + valueOf(drow["Buy_Num"])+","
             + Convert.ToDecimal(drow["expectMonth_Today"]) + ","
             + Convert.ToDecimal(drow["expectMonth_Ten"]) + ","
             + time + ")"); 
            }
            //事務提交
            dao.Commit();
        }catch(Exception ex){
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);      
        }
        dao.CloseConnection();    
    }


    /// <summary>
    /// 轉化obj->int
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int valueOf(object obj)
    {
        if (obj == null || obj.ToString() == "")
        {
            return 0;
        }
        else
        {
            return int.Parse(obj.ToString().Replace(",",""));
        }
    }

    /// <summary>
    /// 無條件將資料進位到整數
    /// </summary>
    /// <param name="dec"></param>
    /// <returns></returns>
    private int double2Int(double dec)
    {
        if (dec > (int)dec)
        {
            return (int)dec + 1;
        }
        else
        {
            return (int)dec;
        }
    }


}
