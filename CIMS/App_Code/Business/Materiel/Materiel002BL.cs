//******************************************************************
//*  作    者：James
//*  功能說明：寄卡單種類管理邏輯
//*  創建日期：2008-08-26
//*  修改日期：2008-08-26 12:00
//*  修改記錄：
//*            □2008-08-26
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
/// 寄卡單種類管理作業
/// </summary>
public class Materiel002BL : BaseLogic
{
    #region SQL語句
    public const string SEL_CARD_EXPONET_INFO = "SELECT CE.RID,CE.Wear_Rate,CE.Unit_Price,CE.Name,CE.Serial_Number,"
                                                + "CE.Safe_Type,CE.Safe_Number,CE.Billing_Type "
                                                + "FROM CARD_EXPONENT AS CE "
                                                + "WHERE CE.RST='A'";
    public const string SEL_CARD_TYPE = "SELECT DISTINCT CT.Exponent_RID "
                                        + " FROM CARD_TYPE AS CT "
                                        + " WHERE CT.RST = 'A' "
                                        + "     AND CT.RID IN ";
    public const string SEL_SELECTED_CARDTYPE = "SELECT card.RID AS RID, card.Display_Name AS NAME " 
                                   + "FROM CARD_TYPE AS card INNER JOIN CARD_EXPONENT AS ce ON card.Exponent_RID = ce.RID "
                                   + "WHERE card.RST = 'A' and ce.RST = 'A' and ce.RID = @CARD_EXPONENT_rid";
    public const string QUERY_EXPONET_CARDTYPE_BY_EXPONENT_RID = "SELECT COUNT(*) " 
                                       + "FROM CARD_TYPE " 
                                       + "WHERE RST = 'A' and Exponent_RID = @CARD_EXPONENT_rid";
    public const string SEL_CARD_TYPE_3 = "SELECT CT.Display_Name,CT.RID, CT.Display_Name AS NAME "
                                        + " FROM CARD_TYPE AS CT "
                                        + " WHERE CT.RST = 'A' "
                                        + "     AND CT.Exponent_RID = 0 and CT.IS_USING='Y'";
    public const string SEL_CARD_TYPE_1 = "SELECT CT.Display_Name,CT.RID, CT.Display_Name AS NAME,CT.IS_USING "
                                        + " FROM CARD_TYPE AS CT "
                                        + " WHERE CT.RST = 'A' "
                                        + "     AND CT.Exponent_RID = 0 and CT.IS_USING='Y'"
                                        + "UNION SELECT CT.Display_Name,CT.RID, CT.Display_Name AS NAME,CT.IS_USING "
                                        + " FROM CARD_TYPE AS CT "
                                        + " WHERE CT.RST = 'A' and CT.IS_USING='Y'"
                                        + "     AND CT.Exponent_RID = ";
    public const string CHK_EXPONENT_BY_RID = "proc_CHK_DEL_EXPONENT";

    public const string DEL_CARD_TYPE_IMG = "DELETE FROM CARD_EXPONENT_IMG "
                                       + "WHERE CardExponent_RID = @cardExponent_rid ";

    public const string SEL_CARD_EXPONENT_IMG = "SELECT CTI.IMG_File_URL,CTI.File_Name "
                                       + "FROM CARD_EXPONENT_IMG CTI	"
                                       + "WHERE CTI.RST = 'A' AND CTI.CardExponent_RID = @rid";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Materiel002BL()
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
    /// <returns>DataSet[寄卡單種類]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Serial_Number" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CARD_EXPONET_INFO);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" AND CE.Name like @Name");
                dirValues.Add("Name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }
        }

        //執行SQL語句
        DataSet dstCARD_EXPONENT = null;
        try
        {
            dstCARD_EXPONENT = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstCARD_EXPONENT;
    }

    /// <summary>
    /// 查詢寄卡單種類信息
    /// </summary>
    /// <param name="strRID">寄卡單種類ID</param>
    /// <param name="ceModel">寄卡單種類信息</param>
    /// <param name="SelectedCardType">寄卡單關聯卡種</param>
    public void ListModel(String strRID, ref CARD_EXPONENT ceModel, ref DataSet SelectedCardType)
    {
        try
        {
            // 取寄卡單種類信息
            ceModel = dao.GetModel<CARD_EXPONENT, int>("RID", int.Parse(strRID));

            dirValues.Clear();
            dirValues.Add("CARD_EXPONENT_rid", int.Parse(strRID));

            SelectedCardType = dao.GetList(SEL_SELECTED_CARDTYPE + " order by Display_Name", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
    }

    /// <summary>
    /// 獲得寄卡單已選卡種類
    /// </summary>
    /// <param name="strRID">寄卡單種類ID</param>
    /// <returns>DataSet[寄卡單已選卡種]</returns>
    public DataSet SelectedCardTypeList(string strRID)
    {
        DataSet dstCardType = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("CARD_EXPONENT_rid", int.Parse(strRID));

            dstCardType = dao.GetList(SEL_SELECTED_CARDTYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCardType;
    }

    /// <summary>
    /// 判斷當前寄卡單是否和卡種關聯
    /// </summary>
    /// <param name="strCardExponentID">寄卡單種類ID</param>
    /// <returns>true:存在 false:不存在</returns>
    public bool ContainsCardExponentCardType(string strCardExponentRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("CARD_EXPONENT_rid", strCardExponentRID);
            return dao.Contains(QUERY_EXPONET_CARDTYPE_BY_EXPONENT_RID, dirValues);
        }
        catch(Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 寄卡單種類增加
    /// </summary>
    /// <param name="ceModel">寄卡單種類模型</param>
    /// <param name="dtblCardType">選擇的卡種</param>
    public void Add(CARD_EXPONENT ceModel, DataTable dtblCardType, DataTable dtblImg)
    {
        CARD_TYPE ctModel = new CARD_TYPE();
        CARD_EXPONENT_IMG ctiModel = new CARD_EXPONENT_IMG();
        try
        {
            //事務開始
            dao.OpenConnection();
            
            #region 寄卡單種類檔添加
            ceModel.Serial_Number = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Serial_Number_B");
            int intRID = Convert.ToInt32(dao.AddAndGetID<CARD_EXPONENT>(ceModel, "RID"));
            #endregion 寄卡單種類檔添加

            #region 寄卡單種類和卡種關系檔處理
            // 保存新的寄卡單和卡種關系檔
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                ctModel = dao.GetModel<CARD_TYPE, int>("RID", Convert.ToInt32(drowCardType["RID"]));
                ctModel.Exponent_RID = intRID;
                dao.Update(ctModel, "RID");
            }
            #endregion 寄卡單種類和卡種關系檔處理

            //foreach新增卡片圖檔
            if (dtblImg.Rows.Count > 0)
            {
                foreach (DataRow drImg in dtblImg.Rows)
                {
                    ctiModel.CardExponent_RID = intRID;
                    ctiModel.IMG_File_URL = Convert.ToString(drImg["IMG_File_URL"]);
                    ctiModel.File_Name = Convert.ToString(drImg["File_Name"]);
                    dao.Add<CARD_EXPONENT_IMG>(ctiModel, "RID");
                }
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
    /// 寄卡單種類修改
    /// </summary>
    /// <param name="ceModel">寄卡單種類</param>
    /// <param name="dtblCardType">選擇的卡種</param>
    public void Update(CARD_EXPONENT ceModel, DataTable dtblCardType, DataTable dtblImg)
    {
        //資料實體
        CARD_TYPE ctModel = new CARD_TYPE();                //卡種類模型
        CARD_EXPONENT_IMG ctiModel = new CARD_EXPONENT_IMG();
        try
        {
            // 取當前寄卡單已經關聯的卡種
             DataSet dstCardType = SelectedCardTypeList(ceModel.RID.ToString());

            // 事務開始
            dao.OpenConnection();

            // 刪除卡種圖檔信息
            this.dirValues.Clear();
            this.dirValues.Add("cardExponent_rid", ceModel.RID);
            dao.ExecuteNonQuery(DEL_CARD_TYPE_IMG, dirValues);


            #region 寄卡單種類和卡種關系檔處理
            // 刪除卡種類型和寄卡單種類關系檔
            dao.ExecuteNonQuery("update CARD_TYPE set Exponent_RID=0 where Exponent_RID=" + ceModel.RID.ToString());

            //foreach (DataRow drowCardType in dstCardType.Tables[0].Rows)
            //{
            //    ctModel = dao.GetModel<CARD_TYPE, int>("RID", Convert.ToInt32(drowCardType["RID"]));
            //    ctModel.Exponent_RID = 0;
            //    dao.Update(ctModel, "RID");
            //}

            // 保存新的寄卡單和卡種關系檔
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                ctModel = dao.GetModel<CARD_TYPE, int>("RID", Convert.ToInt32(drowCardType["RID"]));
                ctModel.Exponent_RID = ceModel.RID;
                dao.Update(ctModel, "RID");
            }
            #endregion 寄卡單種類和卡種關系檔處理

            #region 寄卡單種類檔處理
            dao.Update(ceModel, "RID");
            #endregion 寄卡單種類檔處理
            //foreach新增卡片圖檔
            if (dtblImg.Rows.Count > 0)
            {
                foreach (DataRow drImg in dtblImg.Rows)
                {
                    ctiModel.CardExponent_RID = ceModel.RID;
                    ctiModel.IMG_File_URL = Convert.ToString(drImg["IMG_File_URL"]);
                    ctiModel.File_Name = Convert.ToString(drImg["File_Name"]);
                    dao.Add<CARD_EXPONENT_IMG>(ctiModel, "RID");
                }
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
    /// 根据卡種ID获得卡片圖檔
    /// </summary>
    /// <param name="strRID">卡種RID</param>
    /// <returns><DataSet>卡片圖檔</returns>
    public DataSet GetImgByRID(string strRID)
    {
        DataSet dstImg = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("rid", int.Parse(strRID));
            dstImg = dao.GetList(SEL_CARD_EXPONENT_IMG, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstImg;
    }

    /// <summary>
    /// 寄卡單種類刪除
    /// </summary>
    /// <param name="strRID">寄卡單種類RID</param>
    public void Delete(String strRID)
    {
        CARD_EXPONENT ceModel = new CARD_EXPONENT();
        try
        {
            dao.OpenConnection();

            //ChkDelExponent(strRID);

            //刪除卡種關聯信息
            dao.ExecuteNonQuery("update CARD_TYPE set Exponent_RID=0 where Exponent_RID=" + strRID);


            //刪除圖檔相關信息
            this.dirValues.Clear();
            this.dirValues.Add("cardExponent_rid", strRID);
            dao.ExecuteNonQuery(DEL_CARD_TYPE_IMG, dirValues);

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("CARD_EXPONENT", dirValues);

            SetOprLog("4");

            dao.Commit();

            //ceModel = dao.GetModel<CARD_EXPONENT, int>("RID", int.Parse(strRID));
            //ceModel.RST = GlobalString.RST.DELETE;
            //dao.Update(ceModel, "RID");
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 檢查寄卡單是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public bool ChkDelExponent(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("CardExponent_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_EXPONENT_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            return true;
        else
            return false;

    }
}
