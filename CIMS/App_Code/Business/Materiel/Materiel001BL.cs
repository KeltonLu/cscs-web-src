//******************************************************************
//*  作    者：Su
//*  功能說明：信封基本管理邏輯
//*  創建日期：2008-08-22
//*  修改日期：2008-08-22 12:00
//*  修改記錄：
//*            □2008-08-22
//*              1.創建 蘇傑
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
/// 信封基本管理作業
/// </summary>
public class Materiel001BL : BaseLogic
{
    #region SQL語句
    public const string SEL_ENVELOPE_INFO = "SELECT E.RID, E.Serial_Number, E.Name, E.Unit_Price, '' AS Unit_Price_Dis, CONVERT(VARCHAR, E.Wear_Rate) + '%' AS Wear_Rate, E.Safe_Type, E.Safe_Number,E.Billing_Type "
                                        + " FROM ENVELOPE_INFO AS E "
                                        + " WHERE E.RST = 'A' ";

    public const string CON_ENVELOPE_INFO_BY_NAME = "SELECT COUNT(*) FROM ENVELOPE_INFO WHERE RST='A' AND Name = @Name";

    public const string SEL_CARD_TYPE_2 = "SELECT CT.Display_Name,CT.RID, CT.Display_Name AS NAME,CT.IS_USING "
                                        + " FROM CARD_TYPE AS CT "
                                        + " WHERE CT.RST = 'A' "
                                        + " AND CT.Envelope_RID = ";

    public const string SEL_CARD_TYPE_3 = "SELECT CT.Display_Name,CT.RID, CT.Display_Name AS NAME,CT.IS_USING "
                                        + " FROM CARD_TYPE AS CT "
                                        + " WHERE CT.RST = 'A'  "
                                        + " AND CT.Envelope_RID = '0' ";

    public const string DEL_CARD_TYPE_2 = "UPDATE "
                                        + " CARD_TYPE "
                                        + " SET Envelope_RID = 0 "
                                        + " WHERE RST = 'A' "
                                        + "     AND Envelope_RID = @RID ";

    public const string CHK_ENVELOPE_BY_RID = "proc_CHK_DEL_Envelope";

    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Materiel001BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢信封基本資料列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[信封基本資料]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Name" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_ENVELOPE_INFO);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" AND Name like @Name");
                dirValues.Add("Name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }
        }

        //執行SQL語句
        DataSet dstEnvelope = null;
        try
        {
            dstEnvelope = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstEnvelope;
    }

    /// <summary>
    /// 獲取Envelope資料模型
    /// </summary>
    /// <param name="strRID">信封基本資料</param>
    /// <returns>Envelope模型</returns>
    public ENVELOPE_INFO GetEnvelope(string strRID)
    {
        ENVELOPE_INFO eiModel = null;
        try
        {
            eiModel = dao.GetModel<ENVELOPE_INFO, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return eiModel;
    }

    /// <summary>
    /// 查詢信封基本資料之已選擇卡種
    /// </summary>
    /// <param name="strRID">信封基本資料ID</param>
    /// <returns>DataSet[卡種]</returns>
    public DataSet GetCard(string strRID)
    {
        DataSet dstEnvelope = null;
        try
        {
            dstEnvelope = dao.GetList(SEL_CARD_TYPE_2 + strRID + " order by Display_Name");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstEnvelope;
    }

    /// <summary>
    /// 信封基本資料增加
    /// </summary>
    /// <param name="cbModel">信封基本資料</param>
    /// <param name="dtblCardType">卡種</param>
    public void Add(ENVELOPE_INFO eiModel, DataTable dtblCardType)
    {
        //資料實體
        CARD_TYPE ctModel = new CARD_TYPE();

        try
        {
            //事務開始
            dao.OpenConnection();

            //新增信封基本資料記錄，返回增加信封基本資料ID
            eiModel.Serial_Number = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Serial_Number");
            int intRID = Convert.ToInt32(dao.AddAndGetID<ENVELOPE_INFO>(eiModel, "RID"));

            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                ctModel = dao.GetModel<CARD_TYPE, int>("RID", Convert.ToInt32(drowCardType["RID"]));
                ctModel.Envelope_RID = intRID;
                dao.Update<CARD_TYPE>(ctModel, "RID");
            }

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
    /// 信封基本資料修改
    /// </summary>
    /// <param name="eiModel">信封基本資料</param>
    /// <param name="dtblCardType">卡種</param>
    public void Update(ENVELOPE_INFO eiModel, DataTable dtblCardType)
    {
        //資料實體
        CARD_TYPE ctModel = new CARD_TYPE();

        try
        {
            //事務開始
            dao.OpenConnection();
            dirValues.Clear();

            //修改该筆信封基本資料記錄
            ENVELOPE_INFO eiModel2 = dao.GetModel<ENVELOPE_INFO, int>("RID", eiModel.RID);
            eiModel2.Name = eiModel.Name;
            eiModel2.Unit_Price = eiModel.Unit_Price;
            eiModel2.Wear_Rate = eiModel.Wear_Rate;
            eiModel2.Safe_Type = eiModel.Safe_Type;
            eiModel2.Safe_Number = eiModel.Safe_Number;
            eiModel2.Billing_Type = eiModel.Billing_Type;
            dao.Update<ENVELOPE_INFO>(eiModel2, "RID");

            //更新该[信封基本資料].RID对应之[卡種]記錄，刪除卡種和信封關係
            dirValues.Add("RID", eiModel.RID);
            dao.ExecuteNonQuery(DEL_CARD_TYPE_2, dirValues, false);

            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                ctModel = dao.GetModel<CARD_TYPE, int>("RID", Convert.ToInt32(drowCardType["RID"]));
                ctModel.Envelope_RID = eiModel.RID;
                dao.Update<CARD_TYPE>(ctModel, "RID");
            }

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
    /// 信封基本資料刪除
    /// </summary>
    /// <param name="strRID">信封基本資料RID</param>
    public void Delete(string strRID)
    {
        //資料實體
        ENVELOPE_INFO eiModel = new ENVELOPE_INFO();

        try
        {
            //事務開始
            dao.OpenConnection();
            dirValues.Clear();

            

            //檢查卡種基本資料檔中是否已使用此信封
            //if (ChkDelENVELOPE(strRID))
            //    return false;

            ////刪除该筆信封基本資料記錄
            //eiModel = dao.GetModel<ENVELOPE_INFO, int>("RID", int.Parse(strRID));
            //eiModel.RST = "D";
            //dao.Update<ENVELOPE_INFO>(eiModel, "RID");

            //更新该[信封基本資料].RID对应之[卡種]記錄，刪除卡種和信封關係
            dirValues.Add("RID", int.Parse(strRID));
            dao.ExecuteNonQuery(DEL_CARD_TYPE_2, dirValues, false);


            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("ENVELOPE_INFO", dirValues);

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

    /// <summary>
    /// 判斷當前記錄是否存在於資料庫中
    /// </summary>
    /// <param name="strName">信封基本資料品名</param>
    /// <returns>true:存在 false:不存在</returns>
    public bool ContainsName(string strName)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Name", strName);
            return dao.Contains(CON_ENVELOPE_INFO_BY_NAME, dirValues);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 判斷當前記錄是否存在於資料庫中
    /// </summary>
    /// <param name="strName">信封基本資料品名</param>
    /// <param name="strName">信封基本資料RID</param>
    /// <returns>true:存在 false:不存在</returns>
    public bool ContainsName(string strName, string strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Name", strName);
            dirValues.Add("RID", strRID);
            return dao.Contains(CON_ENVELOPE_INFO_BY_NAME + " AND RID <> @RID ", dirValues);
        }
        catch
        {
            return true;
        }
    }


    /// <summary>
    /// 檢查信封是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public bool ChkDelENVELOPE(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("Envelope_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_ENVELOPE_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            return true;
        else
            return false;

    }

}
