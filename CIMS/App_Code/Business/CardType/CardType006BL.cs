//******************************************************************
//*  作    者：wangxiaoyan
//*  功能說明：製卡類別設定邏輯
//*  創建日期：2008-08-28
//*  修改日期：2008-08-28 12:00
//*  修改記錄：
//*            □2008-08-28
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
/// CardType006BL 的摘要描述
/// </summary>
public class CardType006BL:BaseLogic
{
    #region SQL語句
    public const string SEL_MAKE_CARD_TYPE = "SELECT MCT.RID, CG.Group_Name , MCT.Type_Name, MCT.Is_Report, MCT.Is_Import "
                                        + "FROM MAKE_CARD_TYPE AS MCT LEFT JOIN CARD_GROUP AS CG ON CG.RST = 'A' AND CG.RID = MCT.cardGroup_RID "
                                        + "WHERE MCT.RST = 'A'";
    public const string SEL_CARD_GROUP = "SELECT CG.RID, CG.Group_Name "
                                        + "FROM CARD_GROUP AS CG "
                                        + "WHERE CG.RST = 'A' AND CG.Param_Code = '"+ GlobalString.Parameter.Type+"'";
    public const string CON_MAKE_CARD_TYPE = "SELECT COUNT(*) "
                                    + "FROM MAKE_CARD_TYPE AS MCT "
                                    + "WHERE MCT.RST = 'A'";

    public const string CHK_MAKECARDTYPE_BY_RID = "proc_CHK_DEL_MAKECARDTYPE";

    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public CardType006BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    //新增卡種類別信息
    public void Add(MAKE_CARD_TYPE cardTypeModel)
    {
        try
        {
            //驗證卡種群組+製卡類別重複存在
            StringBuilder stbCommand = new StringBuilder(CON_MAKE_CARD_TYPE);
            StringBuilder stbWhere = new StringBuilder();
            dirValues.Clear();
            stbWhere.Append(" AND MCT.cardGroup_RID = @groupname ");
            dirValues.Add("groupname", cardTypeModel.CardGroup_RID);
            stbWhere.Append(" AND MCT.Type_Name = @typename ");
            dirValues.Add("typename", cardTypeModel.Type_Name);

            Boolean count = dao.Contains(stbCommand.ToString() + stbWhere.ToString(), dirValues);

            if (count)
            {
                throw new AlertException(BizMessage.BizMsg.ALT_CARDTYPE_006_01);
            }
            else
            {
                //事務開始
                dao.OpenConnection();

                dao.Add<MAKE_CARD_TYPE>(cardTypeModel, "RID");

                //操作日誌
                SetOprLog("2");

                //事務提交
                dao.Commit();
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
    }
    //初始化卡種群組下拉框
    public DataSet GetCardTypeGroup()
    {        
        DataSet dtsgroup_Name = null;
        dirValues.Clear();
        try
        {
            dtsgroup_Name = dao.GetList(SEL_CARD_GROUP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dtsgroup_Name;            
    }
    //查詢製卡類別資料
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Type_Name" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_MAKE_CARD_TYPE);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropGroup_Name"].ToString().Trim()))
            {
                stbWhere.Append(" AND CG.RID = @groupname ");
                dirValues.Add("groupname", searchInput["dropGroup_Name"].ToString().Trim());
                
            }
            if (!StringUtil.IsEmpty(searchInput["txtType_Name"].ToString().Trim()))
            {
                stbWhere.Append(" AND MCT.Type_Name like @typename");
                dirValues.Add("typename", "%"+searchInput["txtType_Name"].ToString().Trim()+"%");

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
    //查看一條卡種類別信息
    public MAKE_CARD_TYPE GetMakeCardType(string strRID)
    {
        MAKE_CARD_TYPE cardTypeParam = null;
        try
        {
            cardTypeParam = dao.GetModel<MAKE_CARD_TYPE, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return cardTypeParam;
    }
    //修改卡種類別信息
    public void Update(MAKE_CARD_TYPE cardTypeModel)
    {
        try
        {
            //驗證卡種群組+製卡類別重複存在
            StringBuilder stbCommand = new StringBuilder(CON_MAKE_CARD_TYPE);
            StringBuilder stbWhere = new StringBuilder();
            dirValues.Clear();
            stbWhere.Append(" AND MCT.cardGroup_RID = @groupname ");
            dirValues.Add("groupname", cardTypeModel.CardGroup_RID);
            stbWhere.Append(" AND MCT.Type_Name = @typename ");
            dirValues.Add("typename", cardTypeModel.Type_Name);
            stbWhere.Append(" AND MCT.RID != @RID ");
            dirValues.Add("RID", cardTypeModel.RID);

            Boolean count = dao.Contains(stbCommand.ToString() + stbWhere.ToString(), dirValues);

            if (count)
            {
                throw new AlertException(BizMessage.BizMsg.ALT_CARDTYPE_006_01);
            }
            else
            {
                //事務開始
                dao.OpenConnection();

                //修改卡種類別到資料庫
                dao.Update<MAKE_CARD_TYPE>(cardTypeModel, "RID");

                //操作日誌
                SetOprLog("3");

                //事務提交
                dao.Commit();
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
    }

    public void Delete(string strRID)
    {
        //資料實體
        MAKE_CARD_TYPE cardTypeModel = new MAKE_CARD_TYPE();
        try
        {
            ChkDelMAKECARDTYPE(strRID);


            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("MAKE_CARD_TYPE", dirValues);

            //cardTypeModel = GetMakeCardType(strRID);
            ////進行邏輯刪除處理
            //cardTypeModel.RST = "D";
            ////保存預算記錄
            //dao.Update<MAKE_CARD_TYPE>(cardTypeModel, "RID");

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
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }        
    }

    /// <summary>
    /// 檢查卡種類別是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelMAKECARDTYPE(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("MAKECARDTYPE_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_MAKECARDTYPE_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "卡種類別"));

    }
}
