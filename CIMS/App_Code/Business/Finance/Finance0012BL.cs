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
/// Finance0012BL 的摘要描述
/// </summary>
public class Finance0012BL : BaseLogic
{
    #region SQL語句
    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Blank = 'Y' ";

    public const string SEL_CARD_TYPE_SAP_DETAIL = "SELECT distinct A.SAP_Serial_Number,A.Blank_Factory_RID,"+
                    "F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,convert(varchar(20),A.Ask_Date,111) as Ask_Date,"+
                    "A.Pass_Status,A.Is_Finance,convert(varchar(20),A.Pay_Date,111) as Pay_Date "+
                    "FROM (SELECT CTSD.Check_Serial_Number,CTS.SAP_Serial_Number,CTS.Pass_Status,"+
                        "CTS.Is_Finance,CTS.Ask_Date,CTS.Pay_Date,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Budget_RID WHEN '2' THEN DR.Budget_RID WHEN '3' THEN DC.Budget_RID END AS Budget_RID,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Blank_Factory_RID WHEN '2' THEN DR.Blank_Factory_RID WHEN '3' THEN DC.Blank_Factory_RID END AS Blank_Factory_RID,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Agreement_RID WHEN '2' THEN DR.Agreement_RID WHEN '3' THEN DC.Agreement_RID END AS Agreement_RID,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Space_Short_RID WHEN '2' THEN DR.Space_Short_RID WHEN '3' THEN DC.Space_Short_RID END AS Space_Short_RID "+
                        "FROM CARD_TYPE_SAP_DETAIL CTSD INNER JOIN CARD_TYPE_SAP CTS ON CTS.RST = 'A' AND CTSD.SAP_RID = CTS.RID "+
                            "LEFT JOIN DEPOSITORY_STOCK DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A'  AND CTSD.Operate_RID = DS.RID "+
                            "LEFT JOIN DEPOSITORY_RESTOCK DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID "+
                            "LEFT JOIN DEPOSITORY_CANCEL DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID "+
                        "WHERE CTSD.RST = 'A' AND CTS.Pass_Status = '4') A "+
                    "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND A.Agreement_RID = AGM.RID "+
                    "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND A.Budget_RID = CB.RID " +
                    "INNER JOIN Factory F ON F.RST = 'A' AND Is_Blank = 'Y' AND A.Blank_Factory_RID = F.RID " +
                    "INNER JOIN Card_Type CT ON CT.RST = 'A' AND A.Space_Short_RID = CT.RID " +
                    "WHERE A.Ask_Date >= @Begin_Date AND A.Ask_Date <= @Finish_Date ";

    public const string SEL_SAP = "SELECT * FROM CARD_TYPE_SAP WHERE RST = 'A' AND SAP_Serial_Number = @sap_serial_number ";

    public const string SEL_SAP_DETAIL = "SELECT a.Fore_Delivery_Date,'' as 含稅總金額,'' as 未稅總金額,A.Operate_RID,CT.Name,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,A.Stock_RID,A.Operate_Type,A.Income_Number,A.Income_Date,A.Unit_Price,A.Unit_Price_No,A.Real_Ask_Number,A.Delay_Days,A.Comment,A.Check_Serial_Number,fine FROM (SELECT CASE CTSD.Operate_Type WHEN '1' THEN DS.Fore_Delivery_Date else '1900-01-01' END AS Fore_Delivery_Date,CASE CTSD.Operate_Type WHEN '1' THEN DS.Budget_RID WHEN '2' THEN DR.Budget_RID WHEN '3' THEN DC.Budget_RID END AS Budget_RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Blank_Factory_RID WHEN '2' THEN DR.Blank_Factory_RID WHEN '3' THEN DC.Blank_Factory_RID END AS Blank_Factory_RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Agreement_RID WHEN '2' THEN DR.Agreement_RID WHEN '3' THEN DC.Agreement_RID END AS Agreement_RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Space_Short_RID WHEN '2' THEN DR.Space_Short_RID WHEN '3' THEN DC.Space_Short_RID END AS Space_Short_RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Stock_RID WHEN '2' THEN DR.Stock_RID WHEN '3' THEN DC.Stock_RID END AS Stock_RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Date WHEN '2' THEN DR.ReIncome_Date WHEN '3' THEN DC.Cancel_Date END AS Income_Date,CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Number WHEN '2' THEN DR.ReIncome_Number WHEN '3' THEN DC.Cancel_Number END AS Income_Number,CTSD.Unit_Price,CTSD.Unit_Price_No,CTSD.Real_Ask_Number,CTSD.Delay_Days,CTSD.Comment,CTSD.Check_Serial_Number,CTSD.Operate_Type,CTSD.SAP_RID,CTSD.Operate_RID FROM CARD_TYPE_SAP_DETAIL CTSD LEFT JOIN (select a.*,b.Fore_Delivery_Date from DEPOSITORY_STOCK a inner join Order_form_detail b on a.OrderForm_detail_rid=b.OrderForm_detail_rid) DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A'  AND CTSD.Operate_RID = DS.RID LEFT JOIN DEPOSITORY_RESTOCK DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID LEFT JOIN DEPOSITORY_CANCEL DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID WHERE CTSD.RST = 'A' ) A LEFT JOIN AGREEMENT AGM ON AGM.RST = 'A' AND A.Agreement_RID = AGM.RID LEFT JOIN CARD_BUDGET CB ON CB.RST = 'A' AND A.Budget_RID = CB.RID LEFT JOIN CARD_TYPE CT ON CT.RST = 'A' AND A.Space_Short_RID = CT. RID LEFT JOIN Factory F ON F.RST = 'A' AND A.Blank_Factory_RID = F.RID INNER JOIN CARD_TYPE_SAP CTS ON A.SAP_RID = CTS.RID WHERE CTS.SAP_Serial_Number=@sap_serial_number ";

    public const string SAP_DETAIL_UPDATE = "SELECT A.Operate_Type,A.Income_Number,OFD.Unit_Price,A.Budget_RID,SUM(A.Real_Ask_Number*A.Unit_Price) as 出帳金額 FROM (SELECT CTSD.Real_Ask_Number,CTSD.Unit_Price,CTSD.Unit_Price_No,CTSD.Operate_Type,CASE CTSD.Operate_Type WHEN '1' THEN DS.OrderForm_Detail_RID WHEN '2' THEN DR.OrderForm_Detail_RID WHEN '3' THEN DC.OrderForm_Detail_RID END AS OrderForm_Detail_RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Number WHEN '2' THEN DR.ReIncome_Number WHEN '3' THEN DC.Cancel_Number END AS Income_Number,CASE CTSD.Operate_Type WHEN '1' THEN DS.Budget_RID WHEN '2' THEN DR.Budget_RID WHEN '3' THEN DC.Budget_RID END AS Budget_RID FROM CARD_TYPE_SAP_DETAIL CTSD INNER JOIN CARD_TYPE_SAP CTS ON CTS.RST = 'A' AND CTSD.SAP_RID = CTS.RID LEFT JOIN DEPOSITORY_STOCK DS ON DS.RST = 'A' AND CTSD.Operate_Type = '1' AND CTSD.Operate_RID = DS.RID LEFT JOIN DEPOSITORY_RESTOCK DR ON DR.RST = 'A' AND CTSD.Operate_Type = '2' AND CTSD.Operate_RID = DR.RID LEFT JOIN DEPOSITORY_CANCEL DC ON DC.RST = 'A' AND CTSD.Operate_Type = '3' AND CTSD.Operate_RID = DC.RID WHERE CTSD.RST = 'A' AND CTS.SAP_Serial_Number = @sap_serial_number ) A LEFT JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND A.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID GROUP BY A.Operate_Type,A.Income_Number,OFD.Unit_Price,A.Budget_RID ";

    public const string SEL_ORDER_DEPOSITORY_STOCK = "SELECT DS.Budget_RID,OFD.Unit_Price FROM DEPOSITORY_STOCK DS INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DS.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID WHERE DS.RID = @rid ";
    public const string SEL_ORDER_DEPOSITORY_RESTOCK = "SELECT DR.Budget_RID,OFD.Unit_Price FROM DEPOSITORY_RESTOCK DR INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DR.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID WHERE DR.RID = @rid ";
    public const string SEL_ORDER_DEPOSITORY_CANCEL = "SELECT DC.Budget_RID,OFD.Unit_Price FROM DEPOSITORY_CANCEL DC INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DC.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID WHERE DC.RID = @rid ";

    // 取所有預算
    public const string SEL_CARD_BUDGET = "SELECT * FROM CARD_BUDGET " +
                        "WHERE RST = 'A' AND Budget_Main_RID = @budget_rid " +
                        "ORDER BY RID";
    // 扣減預算
    public const string UPDATE_CONVER_BUDGET = "UPDATE CARD_BUDGET SET Remain_Card_Price = Remain_Card_Price - @conver_price WHERE RID = @budget_rid";
    // 扣減主預算的剩余金額
    public const string UPDATE_CONVER_BUDGET_MAIN = "UPDATE CARD_BUDGET SET Remain_Total_AMT = Remain_Total_AMT - @conver_price WHERE RID = @main_budget_rid";

    public const string UPDATE_DEPOSITORY_STOCK = "UPDATE DEPOSITORY_STOCK SET Is_Finance = 'Y' WHERE RID = @rid ";
    public const string UPDATE_DEPOSITORY_RESTOCK = "UPDATE DEPOSITORY_RESTOCK SET Is_Finance = 'Y' WHERE RID = @rid ";
    public const string UPDATE_DEPOSITORY_CANCEL = "UPDATE DEPOSITORY_CANCEL SET Is_Finance = 'Y' WHERE RID = @rid ";
    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Finance0012BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    /// <summary>
    /// 獲取空白卡廠
    /// </summary>
    /// <returns></returns>
    public DataSet getFactory()
    {
        DataSet dstPurpose = null;

        try
        {
            dstPurpose = dao.GetList(SEL_PERSON);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstPurpose;
    }


    /// <summary>
    /// 查詢請款放行作業
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet SearchSAP(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? " SAP_Serial_Number" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_CARD_TYPE_SAP_DETAIL);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropBlankFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Blank_Factory_RID = @blankfactory");
                dirValues.Add("blankfactory", searchInput["dropBlankFactory"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtSAP_Serial_Number"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.SAP_Serial_Number LIKE @sap_serial_number ");
                dirValues.Add("sap_serial_number", "%" + searchInput["txtSAP_Serial_Number"].ToString().Trim() + "%");

            }
            if (!StringUtil.IsEmpty(searchInput["txtInvoiceNumber"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Check_Serial_Number LIKE @invoicenumber ");
                dirValues.Add("invoicenumber", "%" + searchInput["txtInvoiceNumber"].ToString().Trim() + "%");

            }
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString() + " 00:00:00");
            }
            else
            {
                dirValues.Add("Begin_Date", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString() + " 23:59:59");
            }
            else
            {
                dirValues.Add("Finish_Date", "9999/12/31");
            }

            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" AND CT.Name LIKE @name ");
                dirValues.Add("name", searchInput["txtName"].ToString().Trim());
            }

        }
        DataSet dtSAP = null;
        try
        {
            dtSAP = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        rowCount = intRowCount;
        return dtSAP;
    }

    //取請款SAP單的資訊
    public DataTable getSAP(string strSAP_Serial_Number)
    {
        DataSet dsSAP = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("sap_serial_number", strSAP_Serial_Number);
            dsSAP = dao.GetList(SEL_SAP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dsSAP.Tables[0];
    }

    //取請款SAP詳細單的資訊
    public DataTable getSAPDetail(string strSAP_Serial_Number)
    {
        DataSet dsSAPDetail = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("sap_serial_number", strSAP_Serial_Number);
            dsSAPDetail = dao.GetList(SEL_SAP_DETAIL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        //將罰款金額移至總計行之下 總計改為合計
        DataRow drNew1 = dsSAPDetail.Tables[0].NewRow();
        drNew1["Name"] = "合計";
        dsSAPDetail.Tables[0].Rows.Add(drNew1);
        DataRow drNew2 = dsSAPDetail.Tables[0].NewRow();
        drNew2["Name"] = "罰款金額";
        dsSAPDetail.Tables[0].Rows.Add(drNew2);
        return dsSAPDetail.Tables[0];
    }

    /// <summary>
    /// 付款處理（付帳修改，不作回補）
    /// </summary>
    /// <param name="Real_Pay_Money"></param>
    /// <param name="Real_Pay_Money_No"></param>
    /// <param name="Ask_Date"></param>
    /// <param name="SAP_Serial_Number"></param>
    /// <param name="Is_Finance"></param>
    /// <param name="dtRequisitionWork"></param>
    public void Save(string Real_Pay_Money, string Real_Pay_Money_No, string Ask_Date, string SAP_Serial_Number, string Is_Finance)
    {
        CARD_TYPE_SAP ctsModel = new CARD_TYPE_SAP();
        try
        {
            dao.OpenConnection();
            
            dirValues.Clear();
            dirValues.Add("sap_serial_number", SAP_Serial_Number);
            dirValues.Add("real_pay_money", Convert.ToDecimal(Real_Pay_Money));
            dirValues.Add("real_pay_money_no", Convert.ToDecimal(Real_Pay_Money_No));
            dirValues.Add("ask_date", Convert.ToDateTime(Ask_Date));
            dao.ExecuteNonQuery("Update CARD_TYPE_SAP SET Real_Pay_Money = @real_pay_money,Real_Pay_Money_No = @real_pay_money_no,Is_Finance = 'Y',Pay_Date = @ask_date WHERE SAP_Serial_Number = @sap_serial_number", dirValues);

            //日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException ex)
        {
            //事務回滾
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 付款處理（第一次付款，要回補預算）
    /// </summary>
    /// <param name="Real_Pay_Money"></param>
    /// <param name="Real_Pay_Money_No"></param>
    /// <param name="Ask_Date"></param>
    /// <param name="SAP_Serial_Number"></param>
    /// <param name="Is_Finance"></param>
    /// <param name="dtRequisitionWork"></param>
    public void Save(string Real_Pay_Money, 
                    string Real_Pay_Money_No, 
                    string Ask_Date, 
                    string SAP_Serial_Number, 
                    string Is_Finance,
                    DataTable dtRequisitionWork)
    {
        CARD_TYPE_SAP ctsModel = new CARD_TYPE_SAP();
        try
        {
            dao.OpenConnection();

            dirValues.Clear();
            dirValues.Add("sap_serial_number", SAP_Serial_Number);
            dirValues.Add("real_pay_money", Convert.ToDecimal(Real_Pay_Money));
            dirValues.Add("real_pay_money_no", Convert.ToDecimal(Real_Pay_Money_No));
            dirValues.Add("ask_date", Convert.ToDateTime(Ask_Date));
            dao.ExecuteNonQuery("Update CARD_TYPE_SAP SET Real_Pay_Money = @real_pay_money,Real_Pay_Money_No = @real_pay_money_no,Is_Finance = 'Y',Pay_Date = @ask_date WHERE SAP_Serial_Number = @sap_serial_number", dirValues);

            // 進行預算回補
            foreach (DataRow dr1 in dtRequisitionWork.Rows)
            {
                string strOperate_Type = dr1["Operate_Type"].ToString();
                DataSet dsDepository = null;
                if (strOperate_Type != "")
                {
                    Decimal dcOldPrice = 0;
                    int intBudget_RID = 0;
                    if (strOperate_Type == "1")
                    {
                        this.dirValues.Clear();
                        this.dirValues.Add("rid", dr1["Operate_RID"].ToString());
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_STOCK, this.dirValues);//標記出帳
                        dsDepository = dao.GetList(SEL_ORDER_DEPOSITORY_STOCK, this.dirValues);
                    }
                    else if (strOperate_Type == "2")
                    {
                        this.dirValues.Clear();
                        this.dirValues.Add("rid", dr1["Operate_RID"].ToString());
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_RESTOCK, this.dirValues);//標記出帳
                        dsDepository = dao.GetList(SEL_ORDER_DEPOSITORY_RESTOCK, this.dirValues);
                    }
                    else if (strOperate_Type == "3")
                    {
                        this.dirValues.Clear();
                        this.dirValues.Add("rid", dr1["Operate_RID"].ToString());
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_CANCEL, this.dirValues);//標記出帳
                        dsDepository = dao.GetList(SEL_ORDER_DEPOSITORY_CANCEL, this.dirValues);
                    }

                    if (null != dsDepository && dsDepository.Tables.Count > 0 && dsDepository.Tables[0].Rows.Count > 0)
                    {
                        dcOldPrice = Convert.ToDecimal(dsDepository.Tables[0].Rows[0]["Unit_Price"]);
                        intBudget_RID = Convert.ToInt32(dsDepository.Tables[0].Rows[0]["Budget_RID"]);
                    }

                    // 預算回補
                    UpdateBugetPrice(int.Parse(strOperate_Type),
                            dcOldPrice,
                            Convert.ToDecimal(dr1["Unit_Price"]),
                            Convert.ToInt32(dr1["Real_Ask_Number"]),
                            intBudget_RID);

                }
            }

            //日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException ex)
        {
            //事務回滾
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 空白卡請款拆分、請款時
    /// </summary>
    /// <param name="intType">入庫、再入庫、退貨標識：入庫、再入庫為1，退貨為-1</param>
    /// <param name="Old_Price">拆分前單價</param>
    /// <param name="Now_Price">現在單價</param>
    /// <param name="intCardNum">請款數量</param>
    /// <param name="Budget_Main_RID">主預算編號</param>
    public void UpdateBugetPrice(int intType,
            Decimal Old_Price,
            Decimal Now_Price,
            int intCardNum,
            int Budget_Main_RID)
    {
        try
        {
            // 取所有預算
            this.dirValues.Clear();
            this.dirValues.Add("budget_rid", Budget_Main_RID);
            DataSet dsCARD_BUDGET = dao.GetList(SEL_CARD_BUDGET, this.dirValues);
            if (null == dsCARD_BUDGET || dsCARD_BUDGET.Tables.Count == 0 ||
                dsCARD_BUDGET.Tables[0].Rows.Count == 0)
            {
                throw new AlertException("沒有找到要回補的預算。");
            }

            // 應該回補金額
            Decimal dcConverPrice = Now_Price * intCardNum - Old_Price * intCardNum;
            Decimal dcNotCoverPrice = dcConverPrice;
            if (intType == 1 || intType == 2)// 入庫和再入庫
            {
                // 繼續扣減預算，按從預算編號從小大到扣減
                if (dcConverPrice > 0)
                {
                    for (int intLoop = 0; intLoop < dsCARD_BUDGET.Tables[0].Rows.Count; intLoop++)
                    {
                        // 判斷該筆預算是否夠扣減
                        if (Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"]) >= dcNotCoverPrice)
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", dcNotCoverPrice);
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);
                            //警訊
                            CARD_BUDGET cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(dirValues["budget_rid"]));
                            DataTable dtblBudget = dao.GetList("select max(valid_date_to) from CARD_BUDGET where Budget_Main_RID=" + dirValues["budget_rid"]).Tables[0];
                            DateTime dtMaxBudget = Convert.ToDateTime(dtblBudget.Rows[0][0]);
                            Warning.SetWarning(GlobalString.WarningType.AskFinanceBudget, new object[6] { cbModel.Budget_ID, cbModel.Total_Card_Num, cbModel.Remain_Total_Num, cbModel.Total_Card_AMT, cbModel.Remain_Total_AMT, dtMaxBudget });
                            // 扣完，跳出
                            break;
                        }
                        else
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"].ToString());
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);
                            dcNotCoverPrice -= Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"].ToString());
                        }
                    }
                }
                // 回補預算，按從預算編號從大到小回補
                else
                {
                    for (int intLoop = dsCARD_BUDGET.Tables[0].Rows.Count - 1; intLoop >= 0; intLoop--)
                    {
                        // 判斷該筆預算是否夠回補
                        if (Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"]) -
                            Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Card_Price"])
                            <= dcNotCoverPrice)
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", dcNotCoverPrice);
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);
                            // 扣完，跳出
                            break;
                        }
                        else
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"]) -
                                                Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Card_Price"]));
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);
                            dcNotCoverPrice -= Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"]) -
                                        Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Card_Price"]);
                        }
                    }
                }
            }
            else if (intType == 3)//退貨
            {
                // 回補預算，按從預算編號從大到小回補
                if (dcConverPrice > 0)
                {
                    for (int intLoop = dsCARD_BUDGET.Tables[0].Rows.Count - 1; intLoop >= 0; intLoop--)
                    {
                        // 判斷該筆預算是否夠扣減
                        if (Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Card_Price"]) -
                            Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"])
                            >= dcNotCoverPrice)
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", -dcNotCoverPrice);
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);                           
                            // 扣完，跳出
                            break;
                        }
                        else
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"]) -
                                                Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Card_Price"]));
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);
                            dcNotCoverPrice -= Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Card_Price"]) -
                                        Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"]);
                        }
                    }
                }
                // 繼續扣減預算，按從預算編號從小到大扣減
                else
                {
                    for (int intLoop = 0; intLoop < dsCARD_BUDGET.Tables[0].Rows.Count; intLoop++)
                    {
                        // 判斷該筆預算是否夠扣減
                        if (Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"]) >= -dcNotCoverPrice)
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", -dcNotCoverPrice);
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);
                            //警訊
                            CARD_BUDGET cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(dirValues["budget_rid"]));
                            DataTable dtblBudget = dao.GetList("select max(valid_date_to) from CARD_BUDGET where Budget_Main_RID=" + dirValues["budget_rid"]).Tables[0];
                            DateTime dtMaxBudget = Convert.ToDateTime(dtblBudget.Rows[0][0]);
                            Warning.SetWarning(GlobalString.WarningType.AskFinanceBudget, new object[6] { cbModel.Budget_ID, cbModel.Total_Card_Num, cbModel.Remain_Total_Num, cbModel.Total_Card_AMT, cbModel.Remain_Total_AMT, dtMaxBudget });
                            // 扣完，跳出
                            break;
                        }
                        else
                        {
                            this.dirValues.Clear();
                            this.dirValues.Add("budget_rid", dsCARD_BUDGET.Tables[0].Rows[intLoop]["RID"].ToString());
                            this.dirValues.Add("conver_price", dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"].ToString());
                            this.dirValues.Add("main_budget_rid", Budget_Main_RID);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET_MAIN, this.dirValues);
                            dao.ExecuteNonQuery(UPDATE_CONVER_BUDGET, this.dirValues);
                            dcNotCoverPrice -= Convert.ToDecimal(dsCARD_BUDGET.Tables[0].Rows[intLoop]["Remain_Card_Price"].ToString());
                        }
                    }
                }
            }
           
        }
        catch (AlertException ae)
        {
            throw ae;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }
}
