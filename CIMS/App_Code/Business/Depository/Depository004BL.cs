//******************************************************************
//*  作    者：Macuiyan
//*  功能說明：卡片退貨管理
//*  創建日期：2008-09-02
//*  修改日期：2008-09-02 12:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 馬翠艷
//*******************************************************************
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
/// Depository004 的摘要描述
/// </summary>
public class Depository004BL: BaseLogic
{
    #region SQL語句
    public const string SEL_FACTORY_PERSO = " select RID,Factory_ShortName_cn from FACTORY WHERE RST = 'A' AND IS_Perso ='Y'";
    public const string SEL_DEPOSITORY_CANCEL = "SELECT C.RID,C.Report_RID,TYPE.Name,C.Stock_RID,C.Comment,C.Cancel_Number,C.Cancel_Date"
                                + " FROM DEPOSITORY_CANCEL C LEFT JOIN CARD_TYPE TYPE ON  C.Space_Short_RID = TYPE.RID"
                                + " WHERE C.RST = 'A'";

    //可以退貨的入庫單
    public const string SEL_CANCEl_STOCK_LIST = "SELECT st.Stock_RID, st.Space_Short_RID, st.Stock_Number,  "
                                +" st.Income_Number, st.Income_Date,st.OrderForm_Detail_RID,st.Perso_Factory_RID, st.Blank_Factory_RID, st.Wafer_RID, "
                                +" st.Check_Type, st.Agreement_RID, st.Budget_RID,  st.RID, st.OrderForm_RID,"
                                +" type.Name, '0' AS Cancel_Number,'' AS Comment"
                                +" FROM DEPOSITORY_STOCK st LEFT JOIN CARD_TYPE type ON  st.Space_Short_RID = type.RID"
                                +" WHERE st.RST = 'A'"
                                +" AND Is_Check = 'Y'";
    //查詢前日結庫存量
    public const string SEL_STOCKS ="SELECT Stock_Date,Stocks_Number"
                                +" FROM CARDTYPE_STOCKS"
                                +" WHERE RST='A' AND Perso_Factory_RID = @persoRid AND CardType_RID = @cardRid"
                                +" ORDER BY Stock_Date desc";
    public const string SEL_RID = "Select Cancel_RID From DEPOSITORY_CANCEL"
                                + " WHERE Stock_RID = @stockRid"
                                + " ORDER BY Cancel_RID desc";
    //查詢最近日結后入庫量
    public const string SEL_DEPOSITORY_STOCK = "SELECT Sum(Income_Number) as Number"
                                +" FROM DEPOSITORY_STOCK"
                                +" WHERE RST = 'A' AND Income_Date > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結后退貨量
    public const string SEL_DEPOSITORY_CANCEL_NUMBER = "SELECT Sum(Cancel_Number) as Number"
                                +" FROM DEPOSITORY_CANCEL"
                                +" WHERE RST = 'A' AND Cancel_Date > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結后再入庫量
    public const string SEL_DEPOSITORY_RESTOCK = "SELECT Sum(Reincome_Number) as Number"
                                +" FROM DEPOSITORY_RESTOCK"
                                +" WHERE RST = 'A' AND Reincome_Date > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結后小計檔數量
    public const string SEL_SUBTOTAL_IMPORT = "SELECT Sum(Number) as Number"
                                +" FROM SUBTOTAL_IMPORT"
                                +" WHERE RST = 'A' AND Date_Time > @Stock_Date AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結后廠商移動量
    public const string SEL_FACTORY_CHANGE_IMPORT = "SELECT Sum(Number) as Number"
                                +" FROM FACTORY_CHANGE_IMPORT"
                                +" WHERE RST = 'A' AND Date_Time > @Stock_Date  AND Perso_Factory_RID = @persoRid AND Space_Short_RID = @cardRid";
    //查詢最近日結后卡片轉移量
    public const string CARDTYPE_STOCKS_MOVE_IN = "SELECT sum(Move_Number) as Number"
                                + " FROM CARDTYPE_STOCKS_MOVE"
                                +" WHERE RST = 'A' AND Move_Date > @Stock_Date AND To_Factory_RID = @persoRid AND CardType_RID = @cardRid";

    public const string CARDTYPE_STOCKS_MOVE_OUT = "SELECT sum(Move_Number) as Number"
                                + " FROM CARDTYPE_STOCKS_MOVE"
                                + " WHERE RST = 'A' AND Move_Date > @Stock_Date AND From_Factory_RID = @persoRid AND CardType_RID = @cardRid";

    public const string SEL_CANCEL_DETAIL = "SELECT type.Name,Cancel.Perso_Factory_RID,Cancel.Stock_RID,Cancel.Income_Number,Cancel.Cancel_Date,Cancel.Cancel_Number,Cancel.Comment,Cancel.Is_Finance,Cancel.Is_Check,OFD.Case_Status,Is_AskFinance"
                                + " FROM DEPOSITORY_CANCEL Cancel LEFT JOIN CARD_TYPE type ON type.RID = Cancel.Space_Short_RID"
                                + " left JOIN ORDER_FORM_DETAIL AS OFD ON OFD.OrderForm_Detail_RID = Cancel.OrderForm_Detail_RID "
                                + " WHERE Cancel.RID = @strRid AND Cancel.RST = 'A'";

    public const String SEL_ALL_BUGET = "select * "
                                + "FROM CARD_BUDGET"
                                + " WHERE (Budget_Main_RID = @mainBudgetRid)";

    public const string CON_BY_CODE = "SELECT COUNT(*) "
                                + "FROM DEPOSITORY_CANCEL "
                                + "WHERE RST='A' "
                                + "AND Stock_RID = @rid";
    
    public const string SEL_CHECK_DATE = "SELECT MAX(Stock_Date) AS check_date"
                                + " FROM CARDTYPE_STOCKS where rst= 'A'";

    public const string SEL_ORDER_BUDGET_LOG = "SELECT Remain_Card_Price,Budget_RID,Remain_Card_Num"
                                + " FROM ORDER_BUDGET_LOG"
                                + " WHERE RST = 'A' AND OrderForm_Detail_RID = @detailOderNum"
                                + " ORDER BY Budget_RID desc";
    public const string SEL_AGREEMENT_ISEDIT = "Select RID "
                                + "From ORDER_FORM_DETAIL "
                                + "WHERE RST='A' and OrderForm_Detail_RID = @detailOderNum and Is_Edit_Budget = 'Y'";
    public const string SEL_AGREEMENT = "select RID,Agreement_Code_Main,Remain_Card_Num "
                                + "From AGREEMENT "
                                + "where rst = 'A' and RID = @Agreement_RID";
    public const string SEL_AGREEMENT_PARENT = "select RID,Remain_Card_Num "
                                + "from AGREEMENT "
                                + "where rst = 'A' and Agreement_Code_Main = @Agreement_Code_Main";
    public const string UPDATE_Remain_Card_Num = "Update AGREEMENT "
                                + "Set Remain_Card_Num=@Remain_Card_Num "
                                + "where RID=@Agrement_RID";
    public const string SEL_ORDER_UNIT_PRICE = "SELECT Unit_Price "
                                + "FROM ORDER_FORM_DETAIL "
                                + "WHERE OrderForm_Detail_RID = @detailOderNum AND RST = 'A' ";

    //public const string SEL_AGREEMENT_REMAIN = "select tb1.rid as FRID,tb2.factory_rid,tb1.Remain_Card_Num,tb1.Card_Number,tb2.rid from agreement tb1 inner join (select factory_rid,rid,case Agreement_Code_Main when '' then Agreement_Code else Agreement_Code_Main end as Agreement_Code from agreement) tb2 on tb1.Agreement_Code=tb2.Agreement_Code WHERE TB2.RID = @RID";

    public const string SEL_AGREEMENT_REMAIN = "select factory_rid,Remain_Card_Num,Card_Number,rid from agreement WHERE RID = @RID";

    public const string ADD_CHECKWAFER = "select usable_Number from WAFER_CARDTYPE_USELOG where Operate_Type=1 and Operate_RID=@Operate_RID";

    public const string DEL_CHECK = "proc_CHK_DEL_DEPOSITORY_CANCEL";

    public const string SEL_ORDER_FORM_DETAIL = "SELECT ISNULL(AM.Remain_Card_Num,0) AS AMRemain_Card_Num,isnull(AM.Card_Number,0) as AMCard_Number ,isnull(cb.Total_Card_Num,0) as Total_Card_Num,isnull(cb.Remain_Total_AMT,0) as Remain_Total_AMT,isnull(cb.Remain_Total_Num,0) as Remain_Total_Num,OFD.Is_Edit_Budget,OFD.Unit_Price,OFORM.Pass_Status,OFD.ORDERFORM_DETAIL_RID,OFD.Case_Status,OFD.OrderForm_RID,OFD.Budget_RID,OFORM.Order_Date,OFD.Agreement_RID,OFD.Number FROM ORDER_FORM_DETAIL OFD LEFT JOIN  agreement  AM ON AM.RID=OFD.Agreement_RID inner JOIN ORDER_FORM OFORM ON OFORM.RST='A' AND OFORM.OrderForm_RID=OFD.OrderForm_RID left join CARD_BUDGET CB ON OFD.budget_rid=cb.rid and CB.RST='A' where OFD.OrderForm_Detail_RID=@OrderForm_Detail_RID ";

    public const string SEL_DEPOSITORY_STOCK_2 = " SELECT ISNULL(SUM(DS.Income_Number),0) FROM DEPOSITORY_STOCK AS DS WHERE DS.RST = 'A' AND DS.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_CANCEL_2 = " SELECT ISNULL(SUM(DC.cancel_Number),0) FROM DEPOSITORY_CANCEL AS DC WHERE DC.RST = 'A' AND DC.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_RESTOCK_2 = " SELECT ISNULL(SUM(DR.Reincome_Number),0) FROM DEPOSITORY_RESTOCK AS DR WHERE DR.RST = 'A' AND DR.OrderForm_Detail_RID =@OrderForm_Detail_RID";

    public const string SEL_ORDER_BUDGET_LOG_2 = "select * from ORDER_BUDGET_LOG WHERE RST='A' AND OrderForm_Detail_RID=@OrderForm_Detail_RID ";

    public const string SEL_BUDGET = "select Card_Price,Card_Num,RID,Total_Card_AMT,Total_Card_Num,Remain_Total_AMT,Remain_Total_Num,Remain_Card_Price,Remain_Card_Num from dbo.CARD_BUDGET where RST='A' AND Budget_Main_RID= @Budget_Main_RID";


#endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    CardTypeManager manager = new CardTypeManager();
    public Depository004BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 檢查當前日期是否為日結日
    /// </summary>
    /// <returns></returns>
    public Boolean isCheckDate()
    {
        Boolean result = false;
        try
        {
            DataSet dst = dao.GetList(SEL_CHECK_DATE);
            if (dst.Tables[0] != null && dst.Tables[0].Rows.Count > 0) {
               DateTime checkDate = Convert.ToDateTime(dst.Tables[0].Rows[0]["check_date"].ToString());
               if (checkDate.GetDateTimeFormats()[1] == DateTime.Now.GetDateTimeFormats()[1]) {
                   result = true;
               }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return result;
    }

    /// <summary>
    /// 獲取廠商名稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetFactoryData()
    {
        DataSet dstFactoryData = null;
        try
        {
            dstFactoryData = dao.GetList(SEL_FACTORY_PERSO);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstFactoryData;
    }

    /// <summary>
    /// 查詢是否已存在ParamCode
    /// </summary>
    /// <param name="strParamCode"></param>
    /// <returns></returns>
    public bool ContainsID(string strRid)
    {

        try
        {
            dirValues.Clear();
            dirValues.Add("rid", strRid);
            return dao.Contains(CON_BY_CODE, dirValues);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 按條件查詢
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="firstRowNumber"></param>
    /// <param name="lastRowNumber"></param>
    /// <param name="sortField"></param>
    /// <param name="sortType"></param>
    /// <param name="rowCount"></param>
    /// <returns></returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Stock_RID" : sortField);//默認的排序欄位

        if (sortField == "null")
        {
            sortType = "DESC";
        }

        StringBuilder stbCommand = new StringBuilder(SEL_DEPOSITORY_CANCEL);  

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtCancel_RIDYear"].ToString()))
            {
                string strCancel_RID = searchInput["txtCancel_RIDYear"].ToString();

                if (!StringUtil.IsEmpty(searchInput["txtCancel_RID1"].ToString()))
                {
                    strCancel_RID += searchInput["txtCancel_RID1"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtCancel_RID2"].ToString()))
                {
                    strCancel_RID += searchInput["txtCancel_RID2"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtCancel_RID3"].ToString()))
                {
                    strCancel_RID += searchInput["txtCancel_RID3"].ToString();
                }

                stbWhere.Append(" and c.Stock_RID like @Stock_RID");
                dirValues.Add("Stock_RID", strCancel_RID + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["dropPerso_Factory_RID"].ToString()))
            {
                stbWhere.Append(" and c.Perso_Factory_RID=@Perso_Factory_RID");
                dirValues.Add("Perso_Factory_RID", searchInput["dropPerso_Factory_RID"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtCancel_DateFrom"].ToString()))
            {
                stbWhere.Append(" and c.Cancel_Date>=@Cancel_DateFrom");
                dirValues.Add("Cancel_DateFrom", Convert.ToDateTime(searchInput["txtCancel_DateFrom"].ToString()));
            }

            if (!StringUtil.IsEmpty(searchInput["txtCancel_DateTo"].ToString()))
            {
                searchInput["txtCancel_DateTo"] = searchInput["txtCancel_DateTo"].ToString() + " 23:59:59";//string CancelDate=txtCancel_DateTo.Text + " 23:59:59";
                stbWhere.Append(" and c.Cancel_Date<=@Cancel_DateTo");
                dirValues.Add("Cancel_DateTo", Convert.ToDateTime(searchInput["txtCancel_DateTo"].ToString()));
            }


        
        }
        //執行SQL語句
        DataSet dstlCancel = null;
        try
        {
            dstlCancel = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstlCancel;      
    }

    /// <summary>
    /// 查詢當前輸入條件下所有可以退貨的入庫單
    /// 已經日結的入庫單才可以退貨
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="firstRowNumber"></param>
    /// <param name="lastRowNumber"></param>
    /// <param name="sortField"></param>
    /// <param name="sortType"></param>
    /// <param name="rowCount"></param>
    /// <returns></returns>
    public DataSet StockList(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Stock_RID" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CANCEl_STOCK_LIST);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtStock_RIDYear"].ToString()))
            {
                string strStock_RID = searchInput["txtStock_RIDYear"].ToString();

                if (!StringUtil.IsEmpty(searchInput["txtStock_RID1"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID1"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtStock_RID2"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID2"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtStock_RID3"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID3"].ToString();
                }

                stbWhere.Append(" and st.Stock_RID like @Stock_RID");
                dirValues.Add("Stock_RID", strStock_RID + "%");
            }

            if (((DataTable)searchInput["UctrlCardType"]).Rows.Count > 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["UctrlCardType"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";

                stbWhere.Append(" AND st.Space_Short_RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ")");
            }          

            if (!StringUtil.IsEmpty(searchInput["dropPerso_Factory_RID"].ToString()))
            {
                stbWhere.Append(" and st.Perso_Factory_RID=@Perso_Factory_RID");
                dirValues.Add("Perso_Factory_RID", searchInput["dropPerso_Factory_RID"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtStock_DateFrom"].ToString()))
            {
                stbWhere.Append(" and st.Income_Date>=@Stock_DateFrom");
                dirValues.Add("Stock_DateFrom", Convert.ToDateTime(searchInput["txtStock_DateFrom"].ToString()));
            }

            if (!StringUtil.IsEmpty(searchInput["txtStock_DateTo"].ToString()))
            {
                stbWhere.Append(" and st.Income_Date<=@Stock_DateTo");
                dirValues.Add("Stock_DateTo", Convert.ToDateTime(searchInput["txtStock_DateTo"].ToString()));
            }
        }

        //執行SQL語句
        DataSet dstDepository = null;
        try
        {
            dstDepository = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstDepository;
    }

    /// <summary>
    /// 根據Rid取結果集
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DataSet LoadInfoByRid(string strRID)
    {
        DataSet dstCancelDetail = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("strRid", int.Parse(strRID));

            dstCancelDetail = dao.GetList(SEL_CANCEL_DETAIL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCancelDetail;

    }

    /// <summary>
    /// 根據Rid取Model
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DEPOSITORY_CANCEL GetDepositoryCancel(string strRID)
    {
        DEPOSITORY_CANCEL dcModel = new DEPOSITORY_CANCEL();
        try { 
        
            dcModel = dao.GetModel<DEPOSITORY_CANCEL,int>("RID",int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return dcModel;
    }

    /// <summary>
    /// 添加退貨記錄
    /// </summary>
    /// <param name="dtblDepository"></param>
    /// <returns></returns>
    public string add(DataTable dtblDepository) 
    {
        string strReport_RID = "";
        try
        {
            Dictionary<string ,int> dirCancelNum = new Dictionary<string,int>();
            //將當前的退貨記錄按照入庫單號分組，匯總退貨量
            foreach (DataRow drowDeposotory in dtblDepository.Rows)
            {
                string strkey = drowDeposotory["Perso_Factory_RID"].ToString() + "-" + drowDeposotory["Space_Short_RID"].ToString();
                int number = Convert.ToInt32(drowDeposotory["Cancel_Number"].ToString());
                if (dirCancelNum.ContainsKey(strkey))
                {
                    dirCancelNum[strkey] = dirCancelNum[strkey] + number;
                    
                }
                else {
                    dirCancelNum.Add(strkey, number);
                }              
           
           }

           //判斷當前的退貨量是否大於庫存量
           for (int i = 0; i < dtblDepository.Rows.Count; i++)             
           {
               DataRow drowDeposotory = dtblDepository.Rows[i];
               if (Convert.ToInt32(drowDeposotory["Cancel_Number"].ToString()) > 0)
               {
                   int factoryRid = Convert.ToInt32(drowDeposotory["Perso_Factory_RID"].ToString());
                   int cardTypeRid = Convert.ToInt32(drowDeposotory["Space_Short_RID"].ToString());
                   DataSet dstlstock = getCheckStockByPerso(factoryRid, cardTypeRid);
                   string checkDate = "";
                   int storckNumber = 0;
                   if (dstlstock.Tables[0].Rows.Count != 0)
                   {
                       checkDate = dstlstock.Tables[0].Rows[0]["Stock_Date"].ToString();
                       storckNumber = Convert.ToInt32(dstlstock.Tables[0].Rows[0]["Stocks_Number"].ToString());
                   }
                   if (checkDate == null || checkDate == "")
                       throw new AlertException("數據庫中沒有庫存記錄！");
                   int currentStork = manager.getCurrentStock(factoryRid,cardTypeRid,DateTime.Now);
                   //getCurrentStork(storckNumber, factoryRid, cardTypeRid, checkDate);
                   if (dirCancelNum[factoryRid + "-" + cardTypeRid] > currentStork)
                   {
                        int j = i+1;
                       throw new AlertException("第"+j+"行" + BizMessage.BizMsg.ALT_DEPOSITORY_004_01);
                   }
               }

           }
           dao.OpenConnection();
            //創建打印單號
           strReport_RID = IDProvider.MainIDProvider.GetSystemNewID("Report_RID", "-B");
            //添加退貨數據
           foreach (DataRow drowDeposotory in dtblDepository.Rows)
           {
               if (Convert.ToInt32(drowDeposotory["Cancel_Number"].ToString()) > 0)
               {
                   DEPOSITORY_CANCEL model = new DEPOSITORY_CANCEL();
                   model = dao.GetModelByDataRow<DEPOSITORY_CANCEL>(drowDeposotory);

                   

                   //退貨新增需檢查入庫資料晶片耗用量
                   DEPOSITORY_STOCK dsModel = dao.GetModel<DEPOSITORY_STOCK, string>("Stock_RID", model.Stock_RID);
                   if (dsModel != null)
                   {
                       DataTable dtblCancelTotal = dao.GetList("select sum(cancel_number) from DEPOSITORY_CANCEL where stock_rid='" + model.Stock_RID.ToString() + "'").Tables[0];
                       int intCancel = 0;
                       if (!StringUtil.IsEmpty(dtblCancelTotal.Rows[0][0].ToString()))
                           intCancel = int.Parse(dtblCancelTotal.Rows[0][0].ToString());

                       if ((intCancel + model.Cancel_Number) > dsModel.Income_Number)
                           throw new AlertException(model.Stock_RID+"的累計退貨量不能大於入庫量!");
                       //沒有日結的退貨量
                       DataTable dtblCancelNear = dao.GetList("select sum(cancel_number) from DEPOSITORY_CANCEL where stock_rid='" + model.Stock_RID.ToString() + "' and Is_Check <> 'Y'").Tables[0];
                       int nearCancel = 0;
                       if (!StringUtil.IsEmpty(dtblCancelNear.Rows[0][0].ToString()))
                           nearCancel = int.Parse(dtblCancelNear.Rows[0][0].ToString());

                       dirValues.Clear();
                       dirValues.Add("Operate_RID", dsModel.RID);
                       DataTable dtblWaffer = dao.GetList(ADD_CHECKWAFER, dirValues).Tables[0];
                       if (dtblWaffer.Rows.Count > 0)
                       {
                           int intWafferNumber = int.Parse(dtblWaffer.Rows[0][0].ToString());
                           //所有沒有結餘的退貨量必須小於晶片庫存，否則報錯
                           if ((nearCancel + model.Cancel_Number) > intWafferNumber)
                           {
                               throw new AlertException(string.Format(BizMessage.BizMsg.ALT_DEPOSITORY_004_06, new object[2] { model.Stock_RID,intWafferNumber- nearCancel }));
                           }
                       }
                   }



                   string stockRID = model.Stock_RID;
                   dirValues.Clear();
                   dirValues.Add("stockRid", stockRID);
                   DataSet dst = dao.GetList(SEL_RID, dirValues);
                   int seq = 1;
                   //strCancel：退貨編號等於入庫流水編號+序號
                   string strCancel = stockRID;
                   if (dst.Tables[0] != null && dst.Tables[0].Rows.Count > 0)
                   {
                       string str = dst.Tables[0].Rows[0][0].ToString();
                       seq = Convert.ToInt32(str.Substring(str.Length - 2, 2)) + 1;
                   }
                   if (seq < 10)
                   {
                       strCancel += "0" + seq;
                   }
                   else
                   {
                       strCancel += seq;
                   }
                   model.Cancel_RID = strCancel;
                   model.Report_RID = strReport_RID;
                   model.Cancel_Date =Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd"));

                   //回補預算，合約
                   UpdateBudget(model.OrderForm_Detail_RID, model.Cancel_Number);

                   dao.Add<DEPOSITORY_CANCEL>(model, "RID");
               }
           }

            //操作日誌
            SetOprLog("");

            dao.Commit();
        }catch(AlertException ex){
            dao.Rollback();
            throw new Exception(ex.Message);
        }
        catch(Exception ex){
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        finally
        {
            dao.CloseConnection();
        }

        return strReport_RID;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="strRid"></param>
    /// <param name="strComment"></param>
    /// <param name="strNum"></param>
    public void Update(string strRid,String strComment,string strNum) {
        try
        {
            dao.OpenConnection();

            DEPOSITORY_CANCEL dcModel = GetDepositoryCancel(strRid);
            int persoId = dcModel.Perso_Factory_RID;
            int cardTypeId = dcModel.Space_Short_RID;
            int OldNumber = dcModel.Cancel_Number;

            int currNum = manager.getCurrentStock(persoId, cardTypeId, DateTime.Now);

            if (int.Parse(strNum) > currNum + OldNumber)
            {
                throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_004_01);
            }
            else
            {
                dcModel.Cancel_Number = int.Parse(strNum);
                dcModel.Comment = strComment;
            }

            if (OldNumber != dcModel.Cancel_Number)
            {
                ORDER_FORM_DETAIL detailModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", dcModel.OrderForm_Detail_RID);

                if (OldNumber < dcModel.Cancel_Number)
                {
                    //此次退貨量
                    int intCancelNow = dcModel.Cancel_Number - OldNumber;

                    //退貨新增需檢查入庫資料晶片耗用量
                    DEPOSITORY_STOCK dsModel = dao.GetModel<DEPOSITORY_STOCK, string>("Stock_RID", dcModel.Stock_RID);
                    if (dsModel != null)
                    {
                        DataTable dtblCancelTotal = dao.GetList("select sum(cancel_number) from DEPOSITORY_CANCEL where stock_rid='" + dcModel.Stock_RID.ToString() + "'").Tables[0];
                        int intCancel = 0;
                        if (!StringUtil.IsEmpty(dtblCancelTotal.Rows[0][0].ToString()))
                            intCancel = int.Parse(dtblCancelTotal.Rows[0][0].ToString());

                        if ((intCancel + intCancelNow) > dsModel.Income_Number)
                            throw new AlertException(dcModel.Stock_RID + "的累計退貨量不能大於入庫量!");
                        
                        //沒有日結的退貨量
                        DataTable dtblCancelNear = dao.GetList("select sum(cancel_number) from DEPOSITORY_CANCEL where stock_rid='" + dcModel.Stock_RID.ToString() + "' and Is_Check <> 'Y'").Tables[0];
                        int nearCancel = 0;
                        if (!StringUtil.IsEmpty(dtblCancelNear.Rows[0][0].ToString()))
                            nearCancel = int.Parse(dtblCancelNear.Rows[0][0].ToString());

                        dirValues.Clear();
                        dirValues.Add("Operate_RID", dsModel.RID);
                        DataTable dtblWaffer = dao.GetList(ADD_CHECKWAFER, dirValues).Tables[0];
                        if (dtblWaffer.Rows.Count > 0)
                        {
                            int intWafferNumber = int.Parse(dtblWaffer.Rows[0][0].ToString());
                            //當前未日結的退貨量和本次退貨量之和不能大於可用晶片數量    
                            if ((nearCancel + intCancelNow) > intWafferNumber)
                            {
                                throw new AlertException(string.Format(BizMessage.BizMsg.ALT_DEPOSITORY_004_06, new object[2] { dcModel.Stock_RID, intWafferNumber + OldNumber - nearCancel }));
                            }
                        }
                    }

                    //回補預算，合約
                    UpdateBudget(dcModel.OrderForm_Detail_RID, intCancelNow);
                }
                else
                {
                    //扣除合約數量,預算
                    Add_CheckBudget(dcModel.OrderForm_Detail_RID, OldNumber - dcModel.Cancel_Number);
                }
            }

            dao.Update<DEPOSITORY_CANCEL>(dcModel, "RID");

            //操作日誌
            SetOprLog("");

            dao.Commit();
        }
        catch (AlertException ex)
        {

            dao.Rollback();
            throw new Exception(ex.Message);
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

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void Delete(string strRID)
    {
        DEPOSITORY_CANCEL dcModel = new DEPOSITORY_CANCEL();
        try
        {
            //事務開始
            dao.OpenConnection();

            dirValues.Clear();
            dirValues.Add("RID", strRID);
            DataSet dstBudget = dao.GetList(DEL_CHECK, dirValues, true);
            if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
                throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "退貨單"));



            dirValues.Clear();
            //刪除该筆材質基本資料記錄
            dcModel = dao.GetModel<DEPOSITORY_CANCEL, int>("RID", int.Parse(strRID));

            //扣除合約數量,預算
            Add_CheckBudget(dcModel.OrderForm_Detail_RID, dcModel.Cancel_Number);

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("DEPOSITORY_CANCEL", dirValues);

            //操作日誌
            SetOprLog("4");

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            //異常處理
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }

    }

    public int  getCurrentStork(int oldStork, int factoryRid, int cardTypeRid,string strStorkDate)
    {
        int result = oldStork;
        DateTime storkDate = Convert.ToDateTime(strStorkDate);
        dirValues.Clear();
        dirValues.Add("persoRid", factoryRid.ToString());
        dirValues.Add("cardRid", cardTypeRid.ToString());
        dirValues.Add("Stock_Date", storkDate);
        //加上所有入庫記錄
        result += getNumber(SEL_DEPOSITORY_STOCK, dirValues);
        //減去所有退貨記錄
        result -= getNumber(SEL_DEPOSITORY_CANCEL_NUMBER, dirValues);
        //加上所有再入庫記錄
        result += getNumber(SEL_DEPOSITORY_RESTOCK, dirValues);
        //加上所有小計檔匯入
        result += getNumber(SEL_SUBTOTAL_IMPORT, dirValues);
        //加上廠商異動記錄
        result += getNumber(SEL_FACTORY_CHANGE_IMPORT, dirValues);
        //加上所有移轉進入的紀錄
        result += getNumber(CARDTYPE_STOCKS_MOVE_IN, dirValues);
        //減去所有移出記錄
        result -= getNumber(CARDTYPE_STOCKS_MOVE_OUT, dirValues);
        return result;  
    }

    /// <summary>
    /// 取查詢語句的數據
    /// </summary>
    /// <param name="SQL"></param>
    /// <param name="dirValues"></param>
    /// <returns></returns>
    public int getNumber(String SQL, Dictionary<string, object> dirValues)
    {
        int result = 0;
        DataSet dstNumber = dao.GetList(SQL, dirValues);
        if (dstNumber.Tables[0].Rows[0]["Number"].ToString() != "")
        {
            result = int.Parse(dstNumber.Tables[0].Rows[0]["Number"] + "");
        }
        return result;
    } 

    public DataSet getCheckStockByPerso(int factoryRid, int cardTypeRid)
    {
        DataSet dstlstock = null;
        dirValues.Clear();
        dirValues.Add("persoRid",factoryRid.ToString());
        dirValues.Add("cardRid",cardTypeRid.ToString());
        try {        
             dstlstock= dao.GetList(SEL_STOCKS,dirValues);        
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return dstlstock;
    }


    #region 扣除預算，合約

    /// <summary>
    /// 增加入庫時檢查預算
    /// </summary>
    /// <param name="strofdRID">訂單流水ID</param>
    /// <param name="intNowNum">錄入卡數</param>
    private void Add_CheckBudget(string strofdRID, int intNowNum)
    {
        dirValues.Clear();
        dirValues.Add("OrderForm_Detail_RID", strofdRID);

        DataSet dstDepository = dao.GetList(SEL_ORDER_FORM_DETAIL, dirValues);
        if (dstDepository.Tables[0].Rows.Count>0)
        {
            DataSet dst1 = dao.GetList(SEL_DEPOSITORY_STOCK_2, dirValues);
            DataSet dst2 = dao.GetList(SEL_DEPOSITORY_CANCEL_2, dirValues);
            DataSet dst3 = dao.GetList(SEL_DEPOSITORY_RESTOCK_2, dirValues);

            //預算總卡數
            int intTotal_Num = Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Total_Card_Num"]);
            //合約總卡數
            int intAMTotal_Num = Convert.ToInt32(dstDepository.Tables[0].Rows[0]["AMCard_Number"]);
            //合約剩餘卡數
            int intAMTNum = int.Parse(dstDepository.Tables[0].Rows[0]["AMRemain_Card_Num"].ToString());

            //預算總剩餘金額及卡數
            int intBudgetNum = int.Parse(dstDepository.Tables[0].Rows[0]["Remain_Total_Num"].ToString());
            decimal decBudgetAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Remain_Total_AMT"].ToString());

            //用過的金額及卡數
            int intUsedNum = int.Parse(dst1.Tables[0].Rows[0][0].ToString()) - int.Parse(dst2.Tables[0].Rows[0][0].ToString()) + int.Parse(dst3.Tables[0].Rows[0][0].ToString());
            decimal decUsedAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intUsedNum;

            //這次錄入的金額及卡數
            decimal decNowAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intNowNum;


            #region 計算預算
            if (dstDepository.Tables[0].Rows[0]["Is_Edit_Budget"].ToString() == "Y")
            {
                //預算卡數是否可用
                if (intTotal_Num != 0)
                {
                    if (intBudgetNum < intNowNum)
                        throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
                }

                //合約卡數是否可用
                if (intAMTotal_Num != 0)
                {
                    if (intAMTNum < intNowNum)
                        throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_14, strofdRID));
                }

                //預算金額是否可用
                if (decBudgetAmt < decNowAmt)
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
            }
            else
            {
                if ((intUsedNum + intNowNum) > int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()))
                {
                    if (intUsedNum < int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()))
                        intNowNum = intUsedNum + intNowNum - int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString());

                    decNowAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intNowNum;

                    if (intTotal_Num != 0)
                    {
                        if (intBudgetNum < intNowNum)
                            throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
                    }

                    if (intAMTotal_Num != 0)
                    {
                        if (intAMTNum < intNowNum)
                            throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_14, strofdRID));
                    }

                    if (decBudgetAmt < decNowAmt)
                        throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
                }
                else
                    intNowNum = 0;
            }

            if (intNowNum != 0)
            {
                AGREEMENT amModel = dao.GetModel<AGREEMENT, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Agreement_RID"]));

                //減少合約卡數
                if (amModel.Card_Number != 0)
                {
                    amModel.Remain_Card_Num = amModel.Remain_Card_Num - intNowNum;
                    dao.Update<AGREEMENT>(amModel, "RID");
                }

                AddBudget(strofdRID, Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Budget_RID"]), intNowNum, decNowAmt);


                #region 警訊
                CARD_BUDGET cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Budget_RID"]));
                DataTable dtblBudget = dao.GetList("select max(valid_date_to) from CARD_BUDGET where Budget_Main_RID=" + dstDepository.Tables[0].Rows[0]["Budget_RID"]).Tables[0];
                DateTime dtMaxBudget = Convert.ToDateTime(dtblBudget.Rows[0][0]);

                AGREEMENT aModel = dao.GetModel<AGREEMENT, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Agreement_RID"]));

                Warning.SetWarning(GlobalString.WarningType.CancelBudget, new object[6] { cbModel.Budget_ID, cbModel.Total_Card_Num, cbModel.Remain_Total_Num, cbModel.Total_Card_AMT, cbModel.Remain_Total_AMT, dtMaxBudget });
                Warning.SetWarning(GlobalString.WarningType.CancelAgreement, new object[4] { aModel.Agreement_Code, aModel.Card_Number, aModel.Remain_Card_Num, aModel.End_Time });
                #endregion
            }

            #endregion
        }
    }

    /// <summary>
    /// 預算修改
    /// </summary>
    /// <param name="strofdRID">訂單流水號</param>
    /// <param name="intBudgetRID">預算ID</param>
    /// <param name="intNowNum">增加的卡數</param>
    /// <param name="decNowAmt">增加的金額</param>
    public void AddBudget(string strofdRID, int intBudgetRID, int intNowNum, decimal decNowAmt)
    {
        dirValues.Clear();
        dirValues.Add("Budget_Main_RID", intBudgetRID);

        DataTable dtblBudget = dao.GetList(SEL_BUDGET + " order by rid", dirValues).Tables[0];

        CARD_BUDGET cbMainModel = dao.GetModel<CARD_BUDGET, int>("RID", intBudgetRID);
        CARD_BUDGET cbModel = new CARD_BUDGET();


        if (dtblBudget.Rows.Count > 0)
        {

            int intRemain_Total_Num = Convert.ToInt32(dtblBudget.Rows[0]["Total_Card_Num"]);

            if (intRemain_Total_Num == 0)
                intNowNum = 0;



            foreach (DataRow drowBudget in dtblBudget.Rows)
            {
                //預算金額及卡數
                decimal decRemain_Card_Price = Convert.ToDecimal(drowBudget["Remain_Card_Price"]);
                int intRemain_Card_Num = Convert.ToInt32(drowBudget["Remain_Card_Num"]);

                //單次錄入金額及卡數
                decimal decPrice = 0.00M;
                int intNum = 0;

                cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(drowBudget["RID"]));

                if (intNowNum != 0)
                {
                    if (intRemain_Card_Num > intNowNum)
                    {
                        intNum = intNowNum;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = cbModel.Remain_Card_Num - intNum;

                        cbModel.Remain_Card_Num = cbModel.Remain_Card_Num - intNum;

                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = 0;
                    }
                    else if (intRemain_Card_Num == intNowNum)
                    {
                        intNum = intNowNum;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = 0;

                        cbModel.Remain_Card_Num = 0;


                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = 0;
                    }
                    else
                    {
                        intNum = intRemain_Card_Num;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = 0;

                        cbModel.Remain_Card_Num = 0;

                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = intNowNum - intNum;
                    }
                }

                if (decNowAmt != 0.00M)
                {
                    if (decRemain_Card_Price > decNowAmt)
                    {
                        decPrice = decNowAmt;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = decRemain_Card_Price - decPrice;

                        cbModel.Remain_Card_Price = decRemain_Card_Price - decPrice;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;

                        decNowAmt = 0.00M;
                    }
                    else if (decRemain_Card_Price == decNowAmt)
                    {
                        decPrice = decNowAmt;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = 0.00M;

                        cbModel.Remain_Card_Price = 0.00M;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;

                        decNowAmt = 0.00M;
                    }
                    else
                    {
                        decPrice = decRemain_Card_Price;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = 0.00M;

                        cbModel.Remain_Card_Price = 0.00m;

                        decNowAmt = decNowAmt - decPrice;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;
                    }
                }

                if (intNum != 0 || decPrice != 0.00M)
                {
                    MergBUDGET_LOG(strofdRID, Convert.ToInt32(drowBudget["RID"]), decPrice, intNum);
                    dao.Update<CARD_BUDGET>(cbModel, "RID");
                }
            }

            dao.Update<CARD_BUDGET>(cbMainModel, "RID");

            ORDER_FORM_DETAIL ofdModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", strofdRID);
            if (ofdModel != null)
            {
                if (ofdModel.Is_Edit_Budget != "Y")
                {
                    ofdModel.Is_Edit_Budget = "Y";
                    dao.Update<ORDER_FORM_DETAIL>(ofdModel, "RID");
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// 更新預算日誌
    /// </summary>
    /// <param name="strofdRID">訂單流水號</param>
    /// <param name="intBudgetRID">預算ID</param>
    /// <param name="decBudgetAmt">預算金額</param>
    /// <param name="intNowNum">預算卡數</param>
    private void MergBUDGET_LOG(string strofdRID, int intBudgetRID, decimal decBudgetAmt, int intNowNum)
    {
        dirValues.Clear();
        dirValues.Add("Budget_RID", intBudgetRID);
        dirValues.Add("OrderForm_Detail_RID", strofdRID);

        ORDER_BUDGET_LOG orlModel = dao.GetModel<ORDER_BUDGET_LOG>(SEL_ORDER_BUDGET_LOG_2 + " and Budget_RID=@Budget_RID", dirValues);

        if (orlModel == null)
        {
            orlModel = new ORDER_BUDGET_LOG();
            orlModel.Budget_RID = intBudgetRID;
            orlModel.OrderForm_Detail_RID = strofdRID;
            orlModel.Remain_Card_Num = intNowNum;
            orlModel.Remain_Card_Price = decBudgetAmt;
            dao.Add<ORDER_BUDGET_LOG>(orlModel, "RID");
        }
        else
        {
            orlModel.Remain_Card_Num = orlModel.Remain_Card_Num + intNowNum;
            orlModel.Remain_Card_Price = orlModel.Remain_Card_Price + decBudgetAmt;
            if (orlModel.Remain_Card_Num < 0 || orlModel.Remain_Card_Price < 0)
                throw new AlertException("日誌不能為負數,更新失敗");
            dao.Update<ORDER_BUDGET_LOG>(orlModel, "RID");
        }
    }

    #region 回補預算，合約
    /// <summary>
    /// 更新減少的卡數金額
    /// </summary>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intOldNum">舊入庫數量</param>
    /// <param name="intNowNum">新入庫數量</param>
    /// <param name="intRID">入庫ID</param>
    private void UpdateBudget(string strofdRID, int intReduceNum)
    {
        dirValues.Clear();
        dirValues.Add("OrderForm_Detail_RID", strofdRID);

        DataSet dstDepository = dao.GetList(SEL_ORDER_FORM_DETAIL, dirValues);

        if (dstDepository.Tables[0].Rows.Count == 0)
            return;

        //減少的數量
        decimal decReduceAmt = 0.00M;

        //intReduceNum = intOldNum - intNowNum;

        //預算
        if (dstDepository.Tables[0].Rows[0]["Is_Edit_Budget"].ToString() == "Y")
        {
            decReduceAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intReduceNum;
            //預算
            ReduceBudget(strofdRID, Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Budget_RID"]), intReduceNum, decReduceAmt);

            //合約修改
            AGREEMENT amModel = dao.GetModel<AGREEMENT, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Agreement_RID"]));

            //增加合約卡數
            if (amModel.Card_Number != 0)
            {
                amModel.Remain_Card_Num = amModel.Remain_Card_Num + intReduceNum;
                dao.Update<AGREEMENT>(amModel, "RID");
            }
        }
    }

    /// <summary>
    /// 減少的預算
    /// </summary>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intBudgetRID">主預算ID</param>
    /// <param name="intReductNum">減少數量</param>
    /// <param name="decReductAmt">減少金額</param>
    public void ReduceBudget(string strofdRID, int intBudgetRID, int intReductNum, decimal decReductAmt)
    {
        Depository003BL bl = new Depository003BL();
        bl.dao = this.dao;
        bl.ReduceBudget(strofdRID, intBudgetRID, intReductNum, decReductAmt);
    }

    #endregion
}