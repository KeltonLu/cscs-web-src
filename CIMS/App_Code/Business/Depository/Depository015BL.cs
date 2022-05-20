//******************************************************************
//*  作    者：lantaosu
//*  功能說明：物料採購作業管理邏輯
//*  創建日期：2008-11-18
//*  修改日期：2008-11-20 12:00
//*  修改記錄：
//*            □2008-11-20
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
/// Depository015BL 的摘要描述
/// </summary>
public class Depository015BL : BaseLogic
{
    #region sql語句    
    public const string SEL_MATERIEL_PURCHASE_FORM = "SELECT M.PurchaseOrder_RID,M.Factory_RID1,M.Number1,M.Delivery_Date1,M.Factory_RID2,M.Number2,M.Delivery_Date2,"
                                            + "M.Factory_RID3,M.Number3,M.Delivery_Date3,M.Factory_RID4,M.Number4,M.Delivery_Date4,M.Factory_RID5,M.Number5,M.Delivery_Date5,"
                                            + "M.Case_Date,M.SAP_Serial_Number,M.Serial_Number,M.Unit_Price,M.Total_Num,M.Total_Price,"
                                            + "A.Name AS AName,A.Unit_Price AS AUnit_Price,B.Name AS BName,B.Unit_Price AS BUnit_Price,C.Name AS CName,C.Unit_Price AS CUnit_Price "
                                            + "FROM MATERIEL_PURCHASE_FORM M "
                                            + "LEFT JOIN ENVELOPE_INFO A ON  A.Serial_Number = M.Serial_Number AND A.RST = 'A' "
                                            + "LEFT JOIN CARD_EXPONENT B ON  B.Serial_Number = M.Serial_Number AND B.RST = 'A' "
                                            + "LEFT JOIN DMTYPE_INFO C ON  C.Serial_Number = M.Serial_Number AND C.RST = 'A' "
                                            + "WHERE M.RST = 'A' ";
    public const string SEL_FACTORY_SHORT_CNAME = "SELECT  Factory_ShortName_CN "
                                            + "FROM FACTORY "
                                            + "WHERE RST = 'A'  AND RID = @Factory_RID ";
    public const string SEL_PURCHASE_DETAIL_LIST = "SELECT PurchaseOrder_RID,Detail_RID,Purchase_Date,Serial_Number,"
                                            + "Factory_RID1,Number1,Delivery_Date1,Factory_RID2,Number2,Delivery_Date2,Factory_RID3,Number3,Delivery_Date3,"
                                            + "Factory_RID4,Number4,Delivery_Date4,Factory_RID5,Number5,Delivery_Date5,"
                                            + "Case_Date,SAP_Serial_Number,Ask_Date,Pay_Date,Comment,Unit_Price,Total_Num,Total_Price "
                                            + "FROM MATERIEL_PURCHASE_FORM "
                                            + "WHERE PurchaseOrder_RID=@PurchaseOrder_RID  AND RST = 'A' ";
    public const string SEL_PURCHASE_ASK = "SELECT PurchaseOrder_RID,Detail_RID "
                                            + "FROM MATERIEL_PURCHASE_FORM "
                                            + "WHERE PurchaseOrder_RID=@PurchaseOrder_RID  AND RST = 'A'  AND SAP_Serial_Number<>''";
    public const string SEL_ENVELOPE_INFO = "SELECT Name,Unit_Price,Billing_Type "
                                            + "FROM ENVELOPE_INFO "
                                            + "WHERE RST = 'A'  AND Serial_Number= @Serial_Number ";
    public const string SEL_DMTYPE_INFO = "SELECT Name,Unit_Price,Billing_Type "
                                            + "FROM DMTYPE_INFO "
                                            + "WHERE RST = 'A'  AND Serial_Number= @Serial_Number ";
    public const string SEL_CARD_EXPONENT = "SELECT Name,Unit_Price,Billing_Type "
                                            + "FROM CARD_EXPONENT "
                                            + "WHERE RST = 'A'  AND Serial_Number= @Serial_Number ";
    public const string SEL_MAX_PurchaseOrder_RID = "SELECT TOP 1 PurchaseOrder_RID "
                                            + "FROM MATERIEL_PURCHASE_FORM "
                                            + "WHERE Purchase_Date >= @Purchase_Date1 AND Purchase_Date<=@Purchase_Date2 "
                                            + "ORDER BY PurchaseOrder_RID DESC ";
    public const string SEL_ALL_MATERIEL = "SELECT Serial_Number, Name "
                                            + "FROM ENVELOPE_INFO "
                                            + "WHERE  RST = 'A'  "
                                            + "UNION SELECT Serial_Number, Name "
                                            + "FROM CARD_EXPONENT "
                                            + "WHERE  RST = 'A'   "
                                            + "UNION SELECT Serial_Number, Name "
                                            + "FROM DMTYPE_INFO "
                                            + "WHERE  RST = 'A'   ";
    public const string SEL_COO_PERSO_FACTORY = "SELECT RID, Factory_ShortName_CN "
                                            + "FROM FACTORY "
                                            + "WHERE  RST = 'A'  AND Is_Perso='Y' AND Is_Cooperate='Y' ";                                       
    public const string SEL_MATERIEL_ORDER = "SELECT Serial_Number,Number1,Number2,Number3,Number4,Number5 "
                                            + "From MATERIEL_PURCHASE_FORM "
                                            + "Where RST = 'A' AND YEAR(Purchase_Date) = @budget_year";
    public const string SEL_MATERIEL_BUDGET = "SELECT RID, Materiel_Type, Budget "
                                            + "FROM MATERIEL_BUDGET "
                                            + "WHERE  RST = 'A'  AND Budget_Year=@Budget_Year ";
    public const string SEL_PURCHASE_BY_YEAR = "SELECT M.PurchaseOrder_RID,M.Factory_RID1,M.Number1,M.Delivery_Date1,M.Factory_RID2,M.Number2,M.Delivery_Date2, "
                                            + "M.Factory_RID3,M.Number3,M.Delivery_Date3,M.Factory_RID4,M.Number4,M.Delivery_Date4,M.Factory_RID5,M.Number5,M.Delivery_Date5, "
                                            + "M.Case_Date,M.SAP_Serial_Number,M.Serial_Number,M.Unit_Price,M.Total_Num,M.Total_Price  "
                                            + "FROM MATERIEL_PURCHASE_FORM M "
                                            + "WHERE M.RST = 'A' AND CONVERT(varchar(8), M.Purchase_Date, 112) LIKE (@year+'%') ";
    public const string SEL_MATERIEL_NAME = "SELECT M.Serial_Number,A.Name AS AName,A.Unit_Price AS AUnit_Price,B.Name AS BName,B.Unit_Price AS BUnit_Price,C.Name AS CName,C.Unit_Price AS CUnit_Price "
                                            + "FROM MATERIEL_PURCHASE_FORM M "
                                            + "LEFT JOIN ENVELOPE_INFO A ON  A.Serial_Number = @Serial_Number AND A.RST = 'A' "
                                            + "LEFT JOIN CARD_EXPONENT B ON  B.Serial_Number = @Serial_Number AND B.RST = 'A' "
                                            + "LEFT JOIN DMTYPE_INFO C ON  C.Serial_Number = @Serial_Number AND C.RST = 'A' "
                                            + "WHERE M.RST = 'A' ";
    public const string DEL_MATERIEL_PURCHASE_FORM = "UPDATE MATERIEL_PURCHASE_FORM "
                                            + "SET RST = 'D' "
                                            + "WHERE RST = 'A' AND PurchaseOrder_RID = @PurchaseOrder_RID ";    
    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Depository015BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢項目資料列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "PurchaseOrder_RID" : sortField);//默認的排序欄位

        if (sortField == "null")
        {
            sortType = "DESC";
        }
        

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_MATERIEL_PURCHASE_FORM);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {

            if (!StringUtil.IsEmpty(searchInput["txtBeginDate"].ToString().Trim()))
            {
                stbWhere.Append(" and M.Purchase_Date >= @BeginDate");
                dirValues.Add("BeginDate", searchInput["txtBeginDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtEndDate"].ToString().Trim()))
            {
                stbWhere.Append(" and M.Purchase_Date <= @EndDate");
                dirValues.Add("EndDate", searchInput["txtEndDate"].ToString().Trim());
            }            
            if (!StringUtil.IsEmpty(searchInput["txtMaterial"].ToString().Trim()))
            {
                stbWhere.Append(" and (A.Name like @Name OR B.Name like @Name OR C.Name like @Name) ");
                dirValues.Add("Name", "%" + searchInput["txtMaterial"].ToString().Trim() + "%");
            }
        }

        //執行SQL語句
        DataSet dstPurchase = null;
        try
        {
            dstPurchase = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstPurchase;
    }

    /// <summary>
    /// 獲取廠商名稱
    /// </summary>
    /// <returns></returns>
    public string GetFactoryShortCName(string Factory_RID)
    {
        DataSet dstFactoryShortCName = null;
        string FactoryShortName = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("Factory_RID", Factory_RID);
            dstFactoryShortCName = dao.GetList(SEL_FACTORY_SHORT_CNAME,dirValues);
            if (dstFactoryShortCName != null && dstFactoryShortCName.Tables[0].Rows.Count>0)
                FactoryShortName = dstFactoryShortCName.Tables[0].Rows[0][0].ToString();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return FactoryShortName;
    }

    /// <summary>
    /// 根據傳入的【採購單號】查詢採購訂單記錄
    /// </summary>
    /// <param name="PurchaseOrder_RID"></param>
    /// <returns></returns>
    public DataSet PurchaseDetailList(string PurchaseOrder_RID)
    {
        DataSet dstPurchaseDetailList = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("PurchaseOrder_RID", PurchaseOrder_RID);
            dstPurchaseDetailList = dao.GetList(SEL_PURCHASE_DETAIL_LIST, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstPurchaseDetailList;

    }

    /// <summary>
    /// 判斷採購單是否已有請款的明細
    /// </summary>
    /// <param name="PurchaseOrder_RID"></param>
    /// <returns></returns>
    public bool CheckPurchaseAsk(string PurchaseOrder_RID)
    {
        DataSet dstCheckPurchaseAsk = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("PurchaseOrder_RID", PurchaseOrder_RID);
            dstCheckPurchaseAsk = dao.GetList(SEL_PURCHASE_ASK, dirValues);
            if (dstCheckPurchaseAsk.Tables[0] != null && dstCheckPurchaseAsk.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }        
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }       

    }

    /// <summary>
    /// 獲取品名和單價
    /// </summary>
    /// <param name="Serial_Number"></param>
    /// <returns></returns>
    public DataSet GetMaterielInfo(string Serial_Number)
    {
        DataSet dstMaterielInfo = null;
        try
        {
            if (Serial_Number.Substring(0,1) == "A")
            {
                dirValues.Clear();
                dirValues.Add("Serial_Number", Serial_Number);
                dstMaterielInfo = dao.GetList(SEL_ENVELOPE_INFO, dirValues);
            }
            else if (Serial_Number.Substring(0, 1) == "B")
            {
                dirValues.Clear();
                dirValues.Add("Serial_Number", Serial_Number);
                dstMaterielInfo = dao.GetList(SEL_CARD_EXPONENT, dirValues);
            }
            else if (Serial_Number.Substring(0, 1) == "C")
            {
                dirValues.Clear();
                dirValues.Add("Serial_Number", Serial_Number);
                dstMaterielInfo = dao.GetList(SEL_DMTYPE_INFO, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstMaterielInfo;

    }

    /// <summary>
    /// 獲取資料表中相同YYYYMMDD的轉移單號的最大ID
    /// </summary>
    /// <param name="Purchase_Date">移動日期</param>
    /// <returns></returns>
    public string GetPurchaseOrder_RID(String Purchase_Date)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Purchase_Date1", Purchase_Date + " 00:00:00");
            dirValues.Add("Purchase_Date2", Purchase_Date + " 23:59:59");
            DateTime dtPurchase_Date = Convert.ToDateTime(Purchase_Date);

            // 取轉移日期當天的最大轉移單號
            DataSet dtsMaxPurchaseOrder_RID = dao.GetList(SEL_MAX_PurchaseOrder_RID, dirValues);
            if (dtsMaxPurchaseOrder_RID.Tables[0].Rows.Count > 0)
            {
                int intMaxID = Convert.ToInt32(dtsMaxPurchaseOrder_RID.Tables[0].Rows[0]["PurchaseOrder_RID"].ToString().Substring(8, 2));
                intMaxID++;
                if (intMaxID > 9)
                {
                    return dtPurchase_Date.ToString("yyyyMMdd") + intMaxID.ToString();
                }
                else
                {
                    return dtPurchase_Date.ToString("yyyyMMdd") + "0" + intMaxID.ToString();
                }
            }
            else
            {
                return dtPurchase_Date.ToString("yyyyMMdd") + "01";
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 採購訂單新增
    /// </summary>
    /// <param name="dtStocksMove">DataTable<卡片庫存轉移明細></param>
    /// <param name="strMove_Date">採購日期</param>
    public void Add(DataTable dtPurchase, String Purchase_Date)
    {
        MATERIEL_PURCHASE_FORM mpfModel = new MATERIEL_PURCHASE_FORM();
        try
        {
            //事務開始
            dao.OpenConnection();

            #region 取採購單號
            String PurchaseOrder_RID = this.GetPurchaseOrder_RID(Purchase_Date);
            #endregion 取採購單號

            #region 新增採購下單檔
            foreach (DataRow dr in dtPurchase.Rows)
            {
                mpfModel.PurchaseOrder_RID = PurchaseOrder_RID;
                mpfModel.Comment = dr["Comment"].ToString();
                mpfModel.Purchase_Date = Convert.ToDateTime(Purchase_Date);
                mpfModel.Serial_Number = dr["Serial_Number"].ToString();
                mpfModel.Total_Num = Convert.ToInt64(dr["Total_Num"].ToString());
                mpfModel.Total_Price = Convert.ToDecimal(dr["Total_Price"].ToString());
                mpfModel.Unit_Price = Convert.ToDecimal(dr["Unit_Price"].ToString());
                if (dr["Factory_RID1"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID1 = Convert.ToInt32(dr["Factory_RID1"].ToString());
                    mpfModel.Number1 = Convert.ToInt32(dr["Number1"].ToString());
                    mpfModel.Delivery_Date1 = Convert.ToDateTime(dr["Delivery_Date1"].ToString());
                }
                if (dr["Factory_RID2"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID2 = Convert.ToInt32(dr["Factory_RID2"].ToString());
                    mpfModel.Number2 = Convert.ToInt32(dr["Number2"].ToString());
                    mpfModel.Delivery_Date2 = Convert.ToDateTime(dr["Delivery_Date2"].ToString());
                }
                if (dr["Factory_RID3"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID3 = Convert.ToInt32(dr["Factory_RID3"].ToString());
                    mpfModel.Number3 = Convert.ToInt32(dr["Number3"].ToString());
                    mpfModel.Delivery_Date3 = Convert.ToDateTime(dr["Delivery_Date3"].ToString());
                }
                if (dr["Factory_RID4"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID4 = Convert.ToInt32(dr["Factory_RID4"].ToString());
                    mpfModel.Number4 = Convert.ToInt32(dr["Number4"].ToString());
                    mpfModel.Delivery_Date4 = Convert.ToDateTime(dr["Delivery_Date4"].ToString());
                }
                if (dr["Factory_RID5"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID5 = Convert.ToInt32(dr["Factory_RID5"].ToString());
                    mpfModel.Number5 = Convert.ToInt32(dr["Number5"].ToString());
                    mpfModel.Delivery_Date5 = Convert.ToDateTime(dr["Delivery_Date5"].ToString());
                }
                if (dr["Case_Date"].ToString().Trim() != "")
                    mpfModel.Case_Date = Convert.ToDateTime(dr["Case_Date"].ToString());
                if (dr["Ask_Date"] != null && dr["Ask_Date"].ToString().Trim()!="")
                    mpfModel.Ask_Date = Convert.ToDateTime(dr["Ask_Date"].ToString());
                if (dr["Pay_Date"] != null && dr["Pay_Date"].ToString().Trim() != "")
                    mpfModel.Pay_Date = Convert.ToDateTime(dr["Pay_Date"].ToString());

                mpfModel.SAP_Serial_Number = dr["SAP_Serial_Number"].ToString().Trim();
                
                dao.Add<MATERIEL_PURCHASE_FORM>(mpfModel, "Detail_RID");
            }
            #endregion 新增物料採購作業

            //操作日誌
           // SetOprLog();

            //Warning.SetWarning(GlobalString.WarningType.PlsAskFinance, null);           

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 查詢所有的信封、寄卡單和DM名稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetAllMaterielList()
    {
        DataSet dstAllMaterielList = null;
        try
        {
            dstAllMaterielList = dao.GetList(SEL_ALL_MATERIEL);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        
        return dstAllMaterielList;
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
    /// 根據採購日期的年份，查詢物料年度預算檔
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public DataSet GetMaterielBudget(string year)
    {
        DataSet dstMaterielBudget = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Budget_Year", year);
            dstMaterielBudget = dao.GetList(SEL_MATERIEL_BUDGET,dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dstMaterielBudget;
    }

    /// <summary>
    /// 根據採購日期的年份，查詢物料採購訂單檔(新增時）
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public DataSet GetPurchaseByYear(string year)
    {
        DataSet dstPurchaseByYear = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("year", year);
            dstPurchaseByYear = dao.GetList(SEL_PURCHASE_BY_YEAR, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dstPurchaseByYear;
    }

    /// <summary>
    /// 根據採購日期的年份，查詢物料採購訂單檔（修改時）
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public DataSet GetPurchaseByYear(string year,string PurchaseOrder_RID)
    {
        DataSet dstPurchaseByYear = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("year", year);
            dirValues.Add("PurchaseOrder_RID", PurchaseOrder_RID);
            dstPurchaseByYear = dao.GetList(SEL_PURCHASE_BY_YEAR+" And M.PurchaseOrder_RID<>@PurchaseOrder_RID ", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dstPurchaseByYear;
    }

    /// <summary>
    /// 根據物料品名編號獲取品名
    /// </summary>
    /// <param name="SerialNumber">物料品名編號</param>
    /// <returns></returns>
    public DataSet GetMaterielName(string SerialNumber)
    {
        DataSet dstMaterielName = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Serial_Number", SerialNumber);
            dstMaterielName = dao.GetList(SEL_MATERIEL_NAME, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dstMaterielName;
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="PurchaseOrder_RID"></param>
    public void delete(string PurchaseOrder_RID)
    {
        dirValues.Clear();
        dirValues.Add("PurchaseOrder_RID", PurchaseOrder_RID);
        try
        {
            dao.Delete("MATERIEL_PURCHASE_FORM", dirValues);

            //操作日誌
            //SetOprLog("4");
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 採購訂單新增
    /// </summary>
    /// <param name="dtStocksMove">DataTable<卡片庫存轉移明細></param>
    /// <param name="strMove_Date">採購日期</param>
    public void Add(DataTable dtPurchase, String Purchase_Date, string PurchaseOrder_RID)
    {
        //IR-20100106有關物料採購的錯誤 by yangkun 2009/01/06 start
        //MATERIEL_PURCHASE_FORM mpfModel = new MATERIEL_PURCHASE_FORM();       
        try
        {
            //事務開始
            dao.OpenConnection();

            #region 取採購單號
            //String PurchaseOrder_RID = this.GetPurchaseOrder_RID(Purchase_Date);
            #endregion 取採購單號

            #region 新增採購下單檔
            foreach (DataRow dr in dtPurchase.Rows)
            {
                MATERIEL_PURCHASE_FORM mpfModel = new MATERIEL_PURCHASE_FORM();
          //IR-20100106有關物料採購的錯誤 by yangkun 2009/01/06 end
                mpfModel.PurchaseOrder_RID = PurchaseOrder_RID;
                mpfModel.Comment = dr["Comment"].ToString();
                mpfModel.Purchase_Date = Convert.ToDateTime(Purchase_Date);
                mpfModel.Serial_Number = dr["Serial_Number"].ToString();
                mpfModel.Total_Num = Convert.ToInt64(dr["Total_Num"].ToString());
                mpfModel.Total_Price = Convert.ToDecimal(dr["Total_Price"].ToString());
                mpfModel.Unit_Price = Convert.ToDecimal(dr["Unit_Price"].ToString());
                if (dr["Factory_RID1"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID1 = Convert.ToInt32(dr["Factory_RID1"].ToString());
                    mpfModel.Number1 = Convert.ToInt32(dr["Number1"].ToString());
                    mpfModel.Delivery_Date1 = Convert.ToDateTime(dr["Delivery_Date1"].ToString());
                }
                if (dr["Factory_RID2"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID2 = Convert.ToInt32(dr["Factory_RID2"].ToString());
                    mpfModel.Number2 = Convert.ToInt32(dr["Number2"].ToString());
                    mpfModel.Delivery_Date2 = Convert.ToDateTime(dr["Delivery_Date2"].ToString());
                }
                if (dr["Factory_RID3"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID3 = Convert.ToInt32(dr["Factory_RID3"].ToString());
                    mpfModel.Number3 = Convert.ToInt32(dr["Number3"].ToString());
                    mpfModel.Delivery_Date3 = Convert.ToDateTime(dr["Delivery_Date3"].ToString());
                }
                if (dr["Factory_RID4"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID4 = Convert.ToInt32(dr["Factory_RID4"].ToString());
                    mpfModel.Number4 = Convert.ToInt32(dr["Number4"].ToString());
                    mpfModel.Delivery_Date4 = Convert.ToDateTime(dr["Delivery_Date4"].ToString());
                }
                if (dr["Factory_RID5"].ToString().Trim() != "")
                {
                    mpfModel.Factory_RID5 = Convert.ToInt32(dr["Factory_RID5"].ToString());
                    mpfModel.Number5 = Convert.ToInt32(dr["Number5"].ToString());
                    mpfModel.Delivery_Date5 = Convert.ToDateTime(dr["Delivery_Date5"].ToString());
                }
                if (dr["Case_Date"].ToString().Trim() != "")
                    mpfModel.Case_Date = Convert.ToDateTime(dr["Case_Date"].ToString());
                if (dr["Ask_Date"] != null && dr["Ask_Date"].ToString().Trim() != "")
                    mpfModel.Ask_Date = Convert.ToDateTime(dr["Ask_Date"].ToString());
                if (dr["Pay_Date"] != null && dr["Pay_Date"].ToString().Trim() != "")
                    mpfModel.Pay_Date = Convert.ToDateTime(dr["Pay_Date"].ToString());

                mpfModel.SAP_Serial_Number = dr["SAP_Serial_Number"].ToString().Trim();

                dao.Add<MATERIEL_PURCHASE_FORM>(mpfModel, "Detail_RID");
            }
            #endregion 新增物料採購作業

            //操作日誌
            // SetOprLog();

            //Warning.SetWarning(GlobalString.WarningType.PlsAskFinance, null);           

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }
    
}
