//******************************************************************
//*  作    者：MinghuiGe
//*  功能說明：DM種類基本檔
//*  創建日期：2008-08-20
//*  修改日期：2008-08-20 17:00
//*  修改記錄：
//*            □2008-08-20
//*              1.創建 MinghuiGe
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
/// DM種類基本檔
/// </summary>
public class Materiel003BL : BaseLogic
{
    #region SQL語句
    public const string SEL_DM = "SELECT *,'' AS Price,'' AS Rate,CONVERT(VARCHAR(20),Begin_Date,111)+' ~ '+CONVERT(VARCHAR(20),End_Date,111) AS DATE FROM DMTYPE_INFO WHERE RST='A'";
    public const string SEL_EXIT_DM = "SELECT * FROM DMTYPE_INFO WHERE RID = @RID";
    public const string SEL_PARAM = "SELECT Param_Code,Param_Name FROM PARAM WHERE ParamType_Code = @paramType";
    public const string SEL_TYPE = "SELECT make.RID AS Value, make.Type_Name + '  ' + cards.Group_Name AS Text " +
                                   "FROM MAKE_CARD_TYPE AS make LEFT OUTER JOIN CARD_GROUP AS cards ON make.cardGroup_RID = cards.RID " +
                                   "WHERE make.RST = 'A'";
    public const string SEL_SELECTED_TYPE = "SELECT make.RID AS Value, make.Type_Name + '  ' + cards.Group_Name AS Text " +
                                   "FROM MAKE_CARD_TYPE AS make LEFT OUTER JOIN CARD_GROUP AS cards ON make.cardGroup_RID = cards.RID " +
                                   "WHERE make.RID in (SELECT MakeCardType_RID from DM_MAKECARDTYPE where DM_RID = @DM_RID)";
    public const string SEL_SELECTED_CARDTYPE = "SELECT card.RID AS RID, card.Display_Name AS NAME " +
                                   "FROM DM_CARDTYPE AS dm LEFT OUTER JOIN CARD_TYPE AS card ON dm.CardType_RID = card.RID " +
                                   "WHERE dm.RST = 'A' and dm.DM_RID = @DM_RID";
    public const string CHK_DM_BY_RID = "proc_CHK_DEL_DM";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Materiel003BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢DM主記錄列表
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
        string strSortField = (sortField == "null" ? "Serial_Number" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_DM);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" and Name like @Name");
                dirValues.Add("Name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }
        }

        //執行SQL語句
        DataSet dstDM = null;
        try
        {
            dstDM = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstDM;
    }

    /// <summary>
    /// 查詢批次記錄列表    
    /// <returns>DataSet[批次]</returns>
    public DataSet MakeTypeList()
    {
        //執行SQL語句
        DataSet dstType = null;
        try
        {
            dstType = dao.GetList(SEL_TYPE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        return dstType;
    }

    /// <summary>
    /// 查詢參數記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[預算]</returns>
    public DataSet paramList(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Param_Sort" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_DM);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["paramType"].ToString().Trim()))
            {
                dirValues.Add("paramType", "%" + searchInput["paramType"].ToString().Trim() + "%");
            }
        }

        //執行SQL語句
        DataSet dstParam = null;
        try
        {
            dstParam = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstParam;
    }

    /// <summary>
    /// 獲取已選擇批次信息
    /// </summary>
    public DataSet SelectedMakeTypeList(String RID)
    {
        //執行SQL語句
        DataSet dstType = null;
        try
        {
            dirValues.Clear();
            if (!StringUtil.IsEmpty(RID.Trim()))
            {
                dirValues.Add("DM_RID", RID);
            }
            dstType = dao.GetList(SEL_SELECTED_TYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        return dstType;
    }

    /// <summary>
    /// 獲取已選擇卡種
    /// </summary>
    public DataSet SelectedCardTypeList(String RID)
    {
        //執行SQL語句
        DataSet dstType = null;
        try
        {
            dirValues.Clear();
            if (!StringUtil.IsEmpty(RID.Trim()))
            {
                dirValues.Add("DM_RID", RID);
            }
            dstType = dao.GetList(SEL_SELECTED_CARDTYPE + " order by Display_Name", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        return dstType;
    }

    /// <summary>
    /// 獲取DM信息
    /// </summary>
    public DMTYPE_INFO SelectedDMInfo(String RID)
    {
        DMTYPE_INFO dmModel = null;
        try
        {
            dmModel = dao.GetModel<DMTYPE_INFO, int>("RID", int.Parse(RID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return dmModel;
    }

    /// <summary>
    /// 新增DM
    /// </summary>
    public void Add(DMTYPE_INFO DMInfo, MoveListBox mlbMakeType, DataTable dtblCardType)
    {
        try
        {
            dao.OpenConnection();
            String serialNum = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Serial_Number_C");
            DMInfo.Serial_Number = serialNum;
            int intRID = Convert.ToInt32(dao.AddAndGetID<DMTYPE_INFO>(DMInfo, "RID"));
            string[] AddedItems = mlbMakeType.GetMastAddValuesArray();
            DM_MAKECARDTYPE makeTypeModel = new DM_MAKECARDTYPE();
            for (int index = 0; index < AddedItems.Length; index++)
            {
                makeTypeModel.DM_RID = intRID;
                makeTypeModel.MakeCardType_RID = Convert.ToInt32(AddedItems[index].Trim());
                dao.Add<DM_MAKECARDTYPE>(makeTypeModel, "RID");
            }

            if (DMInfo.Card_Type_Link_Type.Equals(GlobalString.AllPart.Part))
            {
                DM_CARDTYPE cardTypeModel = new DM_CARDTYPE();
                foreach (DataRow drowCardType in dtblCardType.Rows)
                {
                    cardTypeModel.DM_RID = intRID;
                    cardTypeModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                    dao.Add<DM_CARDTYPE>(cardTypeModel, "RID");
                }
            }

            SetOprLog();

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
    /// <summary>
    /// 修改DM
    /// </summary>
    public void Update(DMTYPE_INFO DMInfo, MoveListBox mlbMakeType, DataTable dtblCardType)
    {
        try
        {
            dao.OpenConnection();

            dao.Update(DMInfo, "RID");
            string[] AddedItems = mlbMakeType.GetMastAddValuesArray();
            string[] DelItems = mlbMakeType.GetMastDelValuesArray();
            DM_MAKECARDTYPE makeTypeModel = new DM_MAKECARDTYPE();                        
            for (int index = 0; index < DelItems.Length; index++)
            {
                this.dirValues.Clear();
                this.dirValues.Add("MakeCardType_RID", Convert.ToInt32(DelItems[index].Trim()));
                this.dirValues.Add("DM_RID", DMInfo.RID);
                dao.Delete("DM_MAKECARDTYPE", dirValues);
            }
            for (int index = 0; index < AddedItems.Length; index++)
            {
                makeTypeModel.DM_RID = DMInfo.RID;
                makeTypeModel.MakeCardType_RID = Convert.ToInt32(AddedItems[index].Trim());
                dao.Add<DM_MAKECARDTYPE>(makeTypeModel, "RID");
            }
            if (DMInfo.Card_Type_Link_Type.Equals(GlobalString.AllPart.Part))
            {
                this.dirValues.Clear();
                this.dirValues.Add("DM_RID", DMInfo.RID);
                dao.Delete("DM_CARDTYPE", dirValues);
                DM_CARDTYPE cardTypeModel = new DM_CARDTYPE();
                foreach (DataRow drowCardType in dtblCardType.Rows)
                {
                    cardTypeModel.DM_RID = DMInfo.RID;
                    cardTypeModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                    dao.Add<DM_CARDTYPE>(cardTypeModel, "RID");
                }
            }

            SetOprLog();

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
    /// <summary>
    /// 刪除DM
    /// </summary>
    public void Delete(DMTYPE_INFO DMInfo)
    {        
        try
        {
            dao.OpenConnection();
            ChkDelDM(DMInfo.RID.ToString());
            
            dirValues.Clear();
            this.dirValues.Add("DM_RID", DMInfo.RID);
            dao.Delete("DM_CARDTYPE", dirValues);
            dao.Delete("DM_MAKECARDTYPE", dirValues);


            dirValues.Clear();
            dirValues.Add("RID", DMInfo.RID);
            dao.Delete("DMType_Info", dirValues);

            SetOprLog("4");

            dao.Commit();
        }
        catch (AlertException ex)
        {
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
    /// 檢查DM種類是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelDM(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("DM_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_DM_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "DM種類"));

    }
}