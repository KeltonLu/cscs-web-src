//******************************************************************
//*  作    者：wangxiaoyan
//*  功能說明：卡種狀況設定
//*  創建日期：2008-10-07
//*  修改日期：2008-10-07 12:00
//*  修改記錄：
//*            □2008-10-07
//*              1.創建 wangxiaoyan
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
/// BasicInfo006 的摘要描述
/// </summary>
public class BaseInfo011BL:BaseLogic
{
    #region SQL語句
    public const string SEL_CARD_TYPE_STATUS = "SELECT * "
                                        + "from CARDTYPE_STATUS "
                                        +"WHERE RST='A' ";
    public const string CHK_STATUS_BY_RID = "proc_CHK_DEL_CARDTYPE_STATUS";
    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo011BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cbModel"></param>
    public void Add(CARDTYPE_STATUS ctsModel)
    {
        try
        {
            String seialNum = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Status_Code");
            ctsModel.Status_Code = seialNum;

            //事務開始
            dao.OpenConnection();

            dao.Add<CARDTYPE_STATUS>(ctsModel, "RID");

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();     
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="firstRowNumber">頁面顯示第一條記錄編號</param>
    /// <param name="lastRowNumber">頁面顯示最後一條記錄編號</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序方式</param>
    /// <param name="rowCount">行數</param>
    /// <returns></returns>
    public DataSet list(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Status_Code" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_CARD_TYPE_STATUS);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtStatus_Name"].ToString().Trim()))
            {
                stbWhere.Append(" AND Status_Name like @Status_Name ");
                dirValues.Add("Status_Name", "%" + searchInput["txtStatus_Name"].ToString().Trim()+"%");
                
            }
        }
        DataSet dtscardType_Status = null;
        try
        {
            dtscardType_Status = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        rowCount = intRowCount;
        return dtscardType_Status;
    }

    /// <summary>
    /// 根據RID取Model
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public CARDTYPE_STATUS GetCardTypeStatus(string strRID)
    {
        CARDTYPE_STATUS ctsModel = null;
        try
        {
            ctsModel = dao.GetModel<CARDTYPE_STATUS, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return ctsModel;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="ctsModel"></param>
    public void Update(CARDTYPE_STATUS ctsModel)
    {
        try
        {
            //保存預算記錄
            dao.Update<CARDTYPE_STATUS>(ctsModel, "RID");

            //操作日誌
            SetOprLog();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void Delete(string strRID)
    {
        //資料實體
        CARDTYPE_STATUS ctsModel = new CARDTYPE_STATUS();
        try
        {
            ChkDelStatus(strRID);

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("CARDTYPE_STATUS", dirValues);


            //ctsModel = GetCardTypeStatus(strRID);
            ////進行邏輯刪除處理
            //ctsModel.RST = "D";
            ////保存預算記錄
            //dao.Update<CARDTYPE_STATUS>(ctsModel, "RID");

            //操作日誌
            SetOprLog("4");
        }
        catch (AlertException ex)
        {
            throw ex;
        }   
        catch (Exception ex)
        {
            //異常處理
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }      
    }

    /// <summary>
    /// 檢查卡種狀況是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelStatus(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("Type_rid", strRID);

        DataSet dstBudget = dao.GetList(CHK_STATUS_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "卡種狀況"));

    }
}
