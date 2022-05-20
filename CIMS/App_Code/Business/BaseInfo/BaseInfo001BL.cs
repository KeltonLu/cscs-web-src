//******************************************************************
//*  作    者：FangBao
//*  功能說明：預算管理邏輯
//*  創建日期：2008-08-08
//*  修改日期：2008-08-08 12:00
//*  修改記錄：
//*            □2008-07-31
//*              1.創建 鮑方
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
/// 預算管理作業
/// </summary>
public class BaseInfo001BL : BaseLogic
{
    #region SQL語句
    public const string SEL_BUDGET = "SELECT mindate,maxdate,tb1.*,CONVERT(VARCHAR(20),mindate,111)+'~'+CONVERT(VARCHAR(20),maxdate,111) AS VALID_DATE FROM CARD_BUDGET tb1 inner join (select Budget_Main_RID,min(VALID_DATE_FROM) as mindate,max(VALID_DATE_TO) as maxdate from CARD_BUDGET group by Budget_Main_RID) tb2 on tb1.Budget_Main_RID=tb2.Budget_Main_RID WHERE tb1.RST='A' ";
    public const string SEL_CARDTYPE_BY_RID = "SELECT B.Display_Name as NAME,B.RID FROM DBO.BUDGET_CARDTYPE A INNER JOIN CARD_TYPE B ON A.CARDTYPE_RID=B.RID  WHERE BUDGET_RID = @budget_rid";
    public const string SEL_APPBUDGET_BY_RID = "SELECT RID,BUDGET_ID,Budget_Name,Card_Price,Remain_Card_Price,Card_Num,Remain_Card_Num,VALID_DATE_FROM,VALID_DATE_TO,IMG_FILE_URL,IMG_FILE_Name,'' AS STATUS FROM CARD_BUDGET WHERE Budget_Main_RID = @budget_rid and RST='A' and Budget_Main_RID<>RID";
    public const string CON_BUDGET_BY_RID = "SELECT COUNT(*) FROM CARD_BUDGET WHERE BUDGET_ID = @budget_id";
    public const string CON_BUDGET_BY_Name = "SELECT COUNT(*) FROM CARD_BUDGET WHERE BUDGET_Name = @budget_Name";
    public const string CHK_BUDGET_BY_RID = "proc_CHK_DEL_Budget";
    public const string CHK_ORDERFORM = "SELECT COUNT(*) FROM dbo.ORDER_FORM_DETAIL WHERE BUDGET_RID=@BUDGET_RID";

    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo001BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢預算主記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[預算]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "BUDGET_ID" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_BUDGET);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        stbWhere.Append(" and tb1.Budget_Main_RID=RID ");

        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBUDGET_ID"].ToString().Trim()))
            {
                stbWhere.Append(" and tb1.BUDGET_ID like @BUDGET_ID");
                dirValues.Add("BUDGET_ID", "%" + searchInput["txtBUDGET_ID"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["txtBudget_Name"].ToString().Trim()))
            {
                stbWhere.Append(" and tb1.Budget_Name like @Budget_Name");
                dirValues.Add("Budget_Name", "%" + searchInput["txtBudget_Name"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["txtVALID_DATE_FROM"].ToString().Trim()))
            {
                stbWhere.Append(" and mindate>=@VALID_DATE_FROM");
                dirValues.Add("VALID_DATE_FROM", searchInput["txtVALID_DATE_FROM"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtVALID_DATE_TO"].ToString().Trim()))
            {
                stbWhere.Append(" and maxdate<=@VALID_DATE_TO");
                dirValues.Add("VALID_DATE_TO", searchInput["txtVALID_DATE_TO"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["UrctrlCardTypeSelect"].ToString().Trim()))
            {
                stbWhere.Append(" AND tb1.RID IN (SELECT DISTINCT BUDGET_RID FROM dbo.BUDGET_CARDTYPE WHERE  CARDTYPE_RID IN (SELECT RID FROM dbo.CARD_TYPE WHERE RID =" + searchInput["UrctrlCardTypeSelect"].ToString().Trim() + "))");
            }
        }

        //執行SQL語句
        DataSet dstcard_Budget = null;
        try
        {
            dstcard_Budget = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstcard_Budget;
    }

    /// <summary>
    /// 查詢預算附加信息：追加預算、卡種
    /// </summary>
    /// <param name="strRID">預算ID</param>
    /// <returns>DataSet[追加預算、卡種]</returns>
    public DataSet LoadBudgetInfoByRID(string strRID)
    {
        DataSet dstBudgetAppend = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("budget_rid", int.Parse(strRID));

            dstBudgetAppend = dao.GetList(SEL_APPBUDGET_BY_RID + " " + SEL_CARDTYPE_BY_RID + " order by Display_Name", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstBudgetAppend;
    }

    /// <summary>
    /// 預算增加
    /// </summary>
    /// <param name="cbModel">預算</param>
    /// <param name="dtblBudgetAppend">追加預算</param>
    /// <param name="dtblCardType">卡種</param>
    public void Add(CARD_BUDGET cbModel, DataTable dtblBudgetAppend, DataTable dtblCardType)
    {
        CARD_BUDGET cbAppModel = new CARD_BUDGET();

        BUDGET_CARDTYPE bcModel = new BUDGET_CARDTYPE();

        try
        {
            //事務開始
            dao.OpenConnection();

            //新增主預算記錄，返回增加預算ID
            cbModel.Remain_Card_Price = cbModel.Card_Price;
            cbModel.Remain_Card_Num = cbModel.Card_Num;
            cbModel.Remain_Total_AMT = cbModel.Total_Card_AMT;
            cbModel.Remain_Total_Num = cbModel.Total_Card_Num;
            int intRID = Convert.ToInt32(dao.AddAndGetID<CARD_BUDGET>(cbModel, "RID"));

            dao.ExecuteNonQuery("update CARD_BUDGET set Budget_Main_RID=RID where RID=" + intRID);
            

            //foreach新增已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                bcModel.BUDGET_RID = intRID;
                bcModel.CARDTYPE_RID = Convert.ToInt32(drowCardType["RID"]);
                dao.Add<BUDGET_CARDTYPE>(bcModel, "RID");
            }

            //foreach新增追加預算
            foreach (DataRow drowBudgetAppend in dtblBudgetAppend.Rows)
            {
                cbAppModel = dao.GetModelByDataRow<CARD_BUDGET>(drowBudgetAppend);
                cbAppModel.Budget_Main_RID = intRID;
                cbAppModel.Remain_Card_Price = cbAppModel.Card_Price;
                cbAppModel.Remain_Card_Num = cbAppModel.Card_Num;
                dao.Add<CARD_BUDGET>(cbAppModel, "RID");
            }

            //操作日誌
            SetOprLog();

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
    /// 預算修改
    /// </summary>
    /// <param name="cbModel">預算</param>
    /// <param name="dtblBudgetAppend">追加預算</param>
    /// <param name="dtblCardType">卡種</param>
    /// <param name="strBADel">刪除追加預算RID組合</param>
    public void Update(CARD_BUDGET cbModel, DataTable dtblBudgetAppend, DataTable dtblCardType, string strBADel)
    {
        //資料實體
        CARD_BUDGET cbAppModel = new CARD_BUDGET();
        CARD_BUDGET cboModel = new CARD_BUDGET();
        BUDGET_CARDTYPE bcModel = new BUDGET_CARDTYPE();

        try
        {
            //事務開始
            dao.OpenConnection();

            long intCardNumApp_T = 0;
            decimal decCardPriceApp_T = 0.00M;

            //判斷預算是否被使用
            dirValues.Clear();
            dirValues.Add("BUDGET_RID",cbModel.RID);
            DataSet dstOrder = dao.GetList(CHK_ORDERFORM, dirValues);
            bool IsOrderUsed = false;
            if (dstOrder.Tables[0].Rows.Count > 0)
            {
                if (dstOrder.Tables[0].Rows[0][0].ToString() != "0")
                    IsOrderUsed = true;
            }

            cboModel = GetBudget(cbModel.RID.ToString());
            long intCardNum = cboModel.Card_Num - cbModel.Card_Num;
            decimal decCardPrice = cboModel.Card_Price - cbModel.Card_Price;

            //如果預算使用過，預算值只能改大
            if (IsOrderUsed)
            {
                if (cboModel.Card_Num > cbModel.Card_Num)
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_02, cbModel.Budget_ID));
                if (cboModel.Card_Price > cbModel.Card_Price)
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_01, cbModel.Budget_ID));
            }


            #region Foreach追加預算處理
            foreach (DataRow drowBudgetAppend in dtblBudgetAppend.Rows)
            {
                long intCardNumApp = 0;
                decimal decCardPriceApp = 0.00M;

                //追加預算記錄爲新增
                if (drowBudgetAppend["STATUS"].ToString() == "A")
                {
                    AddAppendBudget(cbModel, drowBudgetAppend, out intCardNumApp, out decCardPriceApp);
                }
                //追加預算記錄爲被修改
                else if (drowBudgetAppend["STATUS"].ToString() == "U")
                {
                    UpdateAppendBudget(cbModel, drowBudgetAppend, out intCardNumApp, out decCardPriceApp,IsOrderUsed);
                }
                else
                {
                    intCardNumApp = Convert.ToInt64(drowBudgetAppend["Remain_Card_Num"]);
                    decCardPriceApp = Convert.ToDecimal(drowBudgetAppend["Remain_Card_Price"]);
                }

                intCardNumApp_T += intCardNumApp;
                decCardPriceApp_T += decCardPriceApp;
            }

            //刪除追加預算的處理
            DeleteAppendBudgets(cbModel, strBADel);

            #endregion

            #region 更新主預算記錄
            //刪除原有已選擇的卡種
            dirValues.Clear();
            dirValues.Add("BUDGET_RID", cbModel.RID);
            dao.Delete("BUDGET_CARDTYPE", dirValues);

            //卡種新增[預算對應卡種]
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                bcModel.BUDGET_RID = cbModel.RID;
                bcModel.CARDTYPE_RID = Convert.ToInt32(drowCardType["RID"]);
                dao.Add<BUDGET_CARDTYPE>(bcModel, "RID");
            }

           
            //if (cboModel.Remain_Card_Num < intCardNum)
            //    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_02, cbModel.Budget_ID));

            //if (cboModel.Remain_Card_Price < decCardPrice)
            //    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_01, cbModel.Budget_ID));

            cbModel.RCT = cboModel.RCT;
            cbModel.RCU = cboModel.RCU;
            cbModel.RST = cboModel.RST;
            cbModel.Budget_Main_RID = cboModel.Budget_Main_RID;
            cbModel.Remain_Card_Num = cboModel.Remain_Card_Num - intCardNum;
            cbModel.Remain_Card_Price = cboModel.Remain_Card_Price - decCardPrice;
            cbModel.Remain_Total_AMT = decCardPriceApp_T + cbModel.Remain_Card_Price;
            cbModel.Remain_Total_Num = intCardNumApp_T + cbModel.Remain_Card_Num;

            //保存預算記錄
            dao.Update<CARD_BUDGET>(cbModel, "RID");
            #endregion


            //操作日誌
            SetOprLog();

            Warning.SetWarning(GlobalString.WarningType.EditBugdet, new object[2] { cbModel.Budget_ID, "修改" });
            Warning.SetWarning(GlobalString.WarningType.BudgetAmtLower, new object[3] { cbModel.Budget_ID, cbModel.Total_Card_AMT, cbModel.Remain_Total_AMT });
            Warning.SetWarning(GlobalString.WarningType.BudgetCardLower, new object[3] { cbModel.Budget_ID, cbModel.Total_Card_Num, cbModel.Remain_Total_Num });
            

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
    /// 預算刪除
    /// </summary>
    /// <param name="strRID">預算RID</param>
    /// <param name="strBADel">刪除追加預算RID組合</param>
    /// <param name="strBADel">異動原因</param>
    public void Delete(string strRID, string strBADel,string strReason)
    {
        //資料實體
        CARD_BUDGET cbModel = new CARD_BUDGET();

        try
        {
            //以[預算].RID獲得修改前的記錄內容
            cbModel = GetBudget(strRID);

            //連接開始
            dao.OpenConnection();

            ChkDelBudget(strRID);

            #region 更新主預算記錄
            //進行物理刪除處理
            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("CARD_BUDGET", dirValues);


            //cbModel.RST = GlobalString.RST.DELETE;
            //cbModel.Reason = strReason;

            ////保存預算記錄
            //dao.Update(cbModel, "RID");

            #endregion

            #region 刪除預算對應卡種處理
            dirValues.Clear();
            dirValues.Add("BUDGET_RID", cbModel.RID);
            dao.Delete("BUDGET_CARDTYPE", dirValues);
            #endregion

            //刪除追加預算的處理
            DeleteAppendBudgets(cbModel, strBADel);

            //操作日誌
            SetOprLog("4");

            Warning.SetWarning(GlobalString.WarningType.EditBugdet, new object[2] { cbModel.Budget_ID, "刪除" });

            //事務提交
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

    /// <summary>
    /// 獲取Budget資料模型
    /// </summary>
    /// <param name="strRID">預算ID</param>
    /// <returns>Budget模型</returns>
    public CARD_BUDGET GetBudget(string strRID)
    {
        CARD_BUDGET cbModel = null;
        try
        {
            cbModel = dao.GetModel<CARD_BUDGET, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return cbModel;
    }

    /// <summary>
    /// 判斷當前記錄是否存在於資料庫中
    /// </summary>
    /// <param name="strBudgetID">預算簽呈ID</param>
    /// <returns>true:存在 false:不存在</returns>
    public bool ContainsBudgetID(string strBudgetID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("budget_id", strBudgetID);
            return dao.Contains(CON_BUDGET_BY_RID, dirValues);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 判斷當前記錄是否存在於資料庫中
    /// </summary>
    /// <param name="strBudgetID">預算簽呈ID</param>
    /// <returns>true:存在 false:不存在</returns>
    public bool ContainsBudgetName(string strBudgetName,string strBudgetRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("budget_Name", strBudgetName);
            if (StringUtil.IsEmpty(strBudgetRID))
                return dao.Contains(CON_BUDGET_BY_Name, dirValues);
            else
                return dao.Contains(CON_BUDGET_BY_Name + " and RID <>" + strBudgetRID, dirValues);
        }
        catch
        {
            return true;
        }
    }



    /// <summary>
    /// 獲取卡種
    /// </summary>
    /// <param name="strRID">預算ID</param>
    /// <returns>DataSet(卡種)</returns>
    public DataSet GetCardTypeByBRID(string strRID)
    {
        DataSet dstCardName = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("budget_rid", Convert.ToInt32(strRID));

            dstCardName = dao.GetList(SEL_CARDTYPE_BY_RID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCardName;
    }


    /// <summary>
    /// 獲取追加預算
    /// </summary>
    /// <param name="strRID">預算ID</param>
    /// <returns>DataSet(追加預算)</returns>
    public DataSet GetBudgetAppend(string strRID)
    {
        DataSet dstBudgetAppend = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("budget_rid", int.Parse(strRID));

            dstBudgetAppend = dao.GetList(SEL_APPBUDGET_BY_RID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstBudgetAppend;
    }

    /// <summary>
    /// 拼接卡種名稱
    /// </summary>
    /// <param name="dtblCardType">卡種資料</param>
    /// <returns>拼接卡種名稱</returns>
    private string GetCardName(DataTable dtblCardType)
    {
        StringBuilder stbCardName = new StringBuilder();
        Boolean bBegin = true;

        foreach (DataRow drowCardType in dtblCardType.Rows)
        {
            if (bBegin)
            {
                stbCardName.Append(drowCardType["NAME"].ToString());
                bBegin = false;
            }
            else
            {
                stbCardName.Append("," + drowCardType["NAME"].ToString());
            }
        }
        return stbCardName.ToString();
    }

    /// <summary>
    /// 新增追加預算
    /// </summary>
    /// <param name="cbModel">預算主記錄</param>
    /// <param name="intLogRID">預算主記錄歷史記錄之RID</param>
    /// <param name="dtblBudgetAppend_old">修改前追加預算記錄列表</param>
    /// <param name="drowBudgetAppend">追加預算記錄</param>
    private void AddAppendBudget(CARD_BUDGET cbModel, DataRow drowBudgetAppend, out long intCardNumApp, out decimal decCardPriceApp)
    {
        CARD_BUDGET cbAppModel = new CARD_BUDGET();         //追加預算模型
        cbAppModel = dao.GetModelByDataRow<CARD_BUDGET>(drowBudgetAppend);
        cbAppModel.Budget_Main_RID = cbModel.RID;

        cbAppModel.Remain_Card_Price = cbAppModel.Card_Price;
        cbAppModel.Remain_Card_Num = cbAppModel.Card_Num;

        //新增該筆追加預算
        dao.Add<CARD_BUDGET>(cbAppModel, "RID");

        intCardNumApp = cbAppModel.Remain_Card_Num;
        decCardPriceApp = cbAppModel.Remain_Card_Price;
    }

    /// <summary>
    /// 修改追加預算記錄
    /// </summary>
    /// <param name="cbModel">預算主記錄</param>
    /// <param name="intLogRID">預算主記錄歷史記錄之RID</param>
    /// <param name="dtblBudgetAppend_old">修改前追加預算記錄列表</param>
    /// <param name="drowBudgetAppend">追加預算記錄</param>
    private void UpdateAppendBudget(CARD_BUDGET cbModel, DataRow drowBudgetAppend, out long intCardNumApp, out decimal decCardPriceApp, bool IsOrderUsed)
    {
        CARD_BUDGET cbAppModel = new CARD_BUDGET();         //追加預算模型新
        CARD_BUDGET cbAppModel_o = new CARD_BUDGET();       //追加預算模型舊

        cbAppModel_o = GetBudget(drowBudgetAppend["RID"].ToString());
        cbAppModel = dao.GetModelByDataRow<CARD_BUDGET>(drowBudgetAppend);

        long intCardNum = cbAppModel_o.Card_Num - cbAppModel.Card_Num;
        decimal decCardPrice = cbAppModel_o.Card_Price - cbAppModel.Card_Price;

        //如果預算使用過，預算值只能改大
        if (IsOrderUsed)
        {
            if (cbAppModel_o.Card_Num > cbModel.Card_Num)
                throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_02, cbModel.Budget_ID));
            if (cbAppModel_o.Card_Price > cbModel.Card_Price)
                throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_01, cbModel.Budget_ID));
        }


        //if (cbAppModel_o.Remain_Card_Num < intCardNum)
        //    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_02, cbAppModel.Budget_ID));

        //if (cbAppModel_o.Remain_Card_Price < decCardPrice)
        //    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_001_01, cbAppModel.Budget_ID));

        cbAppModel.RID = cbAppModel_o.RID;
        cbAppModel.RCT = cbAppModel_o.RCT;
        cbAppModel.RCU = cbAppModel_o.RCU;
        cbAppModel.RST = cbAppModel_o.RST;
        cbAppModel.Budget_Main_RID = cbAppModel_o.Budget_Main_RID;
        cbAppModel.Remain_Card_Num = cbAppModel_o.Remain_Card_Num - intCardNum;
        cbAppModel.Remain_Card_Price = cbAppModel_o.Remain_Card_Price - decCardPrice;

        intCardNumApp = cbAppModel.Remain_Card_Num;
        decCardPriceApp = cbAppModel.Remain_Card_Price;

        //更改該筆追加預算
        dao.Update<CARD_BUDGET>(cbAppModel, "RID");
    }

    /// <summary>
    /// 刪除追加預算記錄
    /// </summary>
    /// <param name="cbModel">預算主記錄</param>
    /// <param name="intLogRID">預算主記錄歷史記錄之RID</param>
    /// <param name="dtblBudgetAppend_old">修改前追加預算記錄列表</param>
    /// <param name="strBADel">刪除追加預算的RID組合</param>
    private void DeleteAppendBudgets(CARD_BUDGET cbModel,string strBADel)
    {
        CARD_BUDGET cbAppModel = new CARD_BUDGET();         //追加預算模型舊

        if (StringUtil.IsEmpty(strBADel))
            return;

        //追加預算記錄爲被刪除的處理
        foreach (string strappenddel in strBADel.Split(','))
        {
            if (strappenddel != "")
            {
                dirValues.Clear();
                dirValues.Add("RID", int.Parse(strappenddel));
                dao.Delete("CARD_BUDGET", dirValues);

                //cbAppModel = GetBudget(strappenddel);
                //cbAppModel.RST = "D";
                ////更改該筆追加預算
                //dao.Update<CARD_BUDGET>(cbAppModel, "RID");
            }
        }
    }

    /// <summary>
    /// 檢查預算是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelBudget(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("Budget_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_BUDGET_BY_RID, dirValues,true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "預算"));

    }

    public void TEST(string test)
    {
        

    }
}
