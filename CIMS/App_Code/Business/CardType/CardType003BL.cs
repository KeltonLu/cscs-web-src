//******************************************************************
//*  作    者：JunWang
//*  功能說明：卡片版面送審檢核管理邏輯
//*  創建日期：2008-08-29
//*  修改日期：2008-08-29
//*  修改記錄：
//*            □2008-08-29
//*              1.創建 王俊
//*******************************************************************
using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using ControlLibrary;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
/// <summary>
/// CardType003BL 的摘要描述
/// </summary>
public class CardType003BL : BaseLogic
{

    #region SQL語句
    public const string SEL_HALLMARK = "SELECT CARD_HALLMARK.*,CARD_TYPE.Display_Name as Name "
                                        + "FROM CARD_HALLMARK LEFT JOIN CARD_TYPE ON CARD_HALLMARK.CardType_RID = CARD_TYPE.RID "
                                        + "WHERE CARD_HALLMARK.RST='A' ";
    public const string SEL_PARASendCheck = "SELECT PARAM_CODE,PARAM_NAME "
                                        + "From PARAM "
                                        + "WHERE PARAMTYPE_CODE= @sendCheck";
    public const string SEL_PARAParam_Name = "SELECT PARAM_CODE,PARAM_NAME "
                                        + "From PARAM "
                                        + "WHERE PARAM_CODE= @param_code and ParamType_Code=@ParamType_Code";

    public const string CHK_HALLMARK_BY_RID = "proc_CHK_DEL_Hallmark";
    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public CardType003BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// Add
    /// </summary>
    /// <param name="chModel">卡片版面資料</param>
    public void Add(CARD_HALLMARK chModel)
    {
        try
        {
            //事務開始
            dao.OpenConnection();

            dao.Add<CARD_HALLMARK>(chModel, "RID");

            //操作日誌
            SetOprLog();

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

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="strRID">RID</param>
    public void Delete(string strRID)
    {
        //資料實體
        CARD_HALLMARK Card_HallMark = new CARD_HALLMARK();
        try
        {
            //事務開始
            dao.OpenConnection();

            ChkDelHallmark(strRID);


            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("CARD_HALLMARK", dirValues);


            //Card_HallMark = GetParam(strRID);

            ////進行邏輯刪除處理
            //Card_HallMark.RST = "D";

            ////保存記錄
            //dao.Update<CARD_HALLMARK>(Card_HallMark, "RID");

            //操作日誌
            SetOprLog("4");

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
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 獲取GetParam
    /// </summary>
    /// <param name="strRID">記錄ID</param>
    /// <returns>Card_HallMark模型</returns>
    public CARD_HALLMARK GetParam(string strRID)
    {
        CARD_HALLMARK Card_HallMark = new CARD_HALLMARK();
        try
        {
            Card_HallMark = dao.GetModel<CARD_HALLMARK, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return Card_HallMark;
    }

    /// <summary>
    /// 修改
    /// </summary>    
    /// <param name="cbModel">修改</param>  
    public void Update(CARD_HALLMARK chModel)
    {
        try
        {
            //事務開始
            dao.OpenConnection();

            //保存記錄
            dao.Update<CARD_HALLMARK>(chModel, "RID");

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            //異常處理
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 查詢卡片版面送審檢核
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Name" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_HALLMARK);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["CardType_RID"].ToString().Trim()))
            {
                stbWhere.Append(" AND [CARD_HALLMARK].CardType_RID = @CardType_RID ");
                dirValues.Add("CardType_RID", searchInput["CardType_RID"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtSerial_Number"].ToString().Trim()))
            {
                stbWhere.Append(" AND [CARD_HALLMARK].Serial_Number like @Serial_Number ");
                dirValues.Add("Serial_Number", "%" + searchInput["txtSerial_Number"].ToString().Trim() + "%");

            }
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString().Trim()))
            {
                stbWhere.Append(" AND [CARD_HALLMARK].Begin_Date >= @Begin_Date ");
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString().Trim()))
            {
                stbWhere.Append(" AND [CARD_HALLMARK].Begin_Date <= @txtFinish_Date ");
                dirValues.Add("txtFinish_Date", searchInput["txtFinish_Date"].ToString().Trim());
            }


            if (!StringUtil.IsEmpty(searchInput["dropSendCheck_Status"].ToString().Trim()))
            {
                stbWhere.Append(" AND [CARD_HALLMARK].SendCheck_Status = @SendCheck_Status ");
                dirValues.Add("SendCheck_Status", searchInput["dropSendCheck_Status"].ToString().Trim());

            }
        }
        DataSet dtsmake_Card_Type = null;
        try
        {
            dtsmake_Card_Type = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        rowCount = intRowCount;
        return dtsmake_Card_Type;
    }


    /// <summary>
    /// 查詢卡種信息
    /// </summary>
    /// <param name="strRID">卡種ID</param>
    /// <returns>Card_HallMark[卡種]</returns>
    public CARD_HALLMARK GetCardExponentModelByRID(string strRID)
    {
        CARD_HALLMARK Card_HallMark = null;
        try
        {
            Card_HallMark = dao.GetModel<CARD_HALLMARK, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return Card_HallMark;
    }

    /// <summary>
    /// 獲取卡種name
    /// </summary>
    /// <param name="CardType_RID">卡種ID</param>
    /// <returns>Card_Type[卡種]</returns>
    public CARD_TYPE GetCardName(int CardType_RID)
    {
        CARD_TYPE Card_Type = null;
        try
        {
            Card_Type = dao.GetModel<CARD_TYPE, int>("RID",CardType_RID);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return Card_Type;
    }


    public DataSet getParamSendCheck()
    {
        DataSet dtsparam_SendCheck = null;
        dirValues.Clear();
        try
        {
            dirValues.Add("sendCheck", GlobalString.ParameterType.SendCheck);
            dtsparam_SendCheck = dao.GetList(SEL_PARASendCheck, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dtsparam_SendCheck;
    }

    /// <summary>
    /// 獲取Param_Name
    /// </summary>
    /// <param name="Param_Code">送審狀態ID</param>
    /// <returns>Param_Name</returns>
    public string getParam_Name(string Param_Code)
    {
        DataSet dtsParam_Name = null;
        DataRow dr;
        string strParam_Name;
        dirValues.Clear();
        try
        {
            dirValues.Add("param_code", Param_Code);
            dirValues.Add("ParamType_Code", GlobalString.ParameterType.SendCheck);
            dtsParam_Name = dao.GetList(SEL_PARAParam_Name, dirValues);
            dr = dtsParam_Name.Tables[0].Rows[0];
            strParam_Name = dr["Param_Name"].ToString();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return strParam_Name;
    }

    /// <summary>
    /// 檢查卡片版面是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelHallmark(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("Hallmark_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_HALLMARK_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "卡片版面"));

    }
}
