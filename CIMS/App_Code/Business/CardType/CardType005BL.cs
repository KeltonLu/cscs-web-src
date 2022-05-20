//******************************************************************
//*  作    者：JunWang
//*  功能說明：Perso廠與卡種設定 
//*  創建日期：2008-09-02
//*  修改日期：2008-09-02 9:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 王俊
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
using System.Collections;
using System.Text;
/// <summary>
/// CardType005BL 的摘要描述
/// </summary>
public class CardType005BL : BaseLogic
{
    #region SQL語句
    public const string SEL_FACTORY = "SELECT F.RID,F.Factory_ID,F.Factory_ShortName_CN "
                                        + "FROM FACTORY AS F "
                                        + "WHERE F.RST = 'A' AND F.Is_Perso = 'Y' order by RID";
    public const string SEL_UNSELECTED_CARDTYPE = "SELECT RID,Name,Display_Name "
                                        + "FROM CARD_TYPE "
                                        + "WHERE RST='A' AND RID not in (SELECT CardType_RID FROM PERSO_CARDTYPE WHERE RST='A' AND Base_Special='1')";

    //public const string SEL_UNSELECTED_CARDTYPE_SPECIAL = "SELECT RID,Name "
    //                                    + "FROM CARD_TYPE "
    //                                    + "WHERE RST='A' AND RID not in (SELECT CardType_RID FROM PERSO_CARDTYPE WHERE RST='A' AND Base_Special='2')";

    //public const string SEL_UNSELECTED_CARDTYPE_SPECIAL = "SELECT RID,Name "
    //                                    + "FROM CARD_TYPE "
    //                                    + "WHERE RST='A' AND RID in (SELECT CardType_RID FROM PERSO_CARDTYPE WHERE RST='A' AND Base_Special='1')";

    public const string SEL_UNSELECTED_CARDTYPE_SPECIAL = "SELECT RID,Name,Display_Name FROM CARD_TYPE WHERE RST='A' AND RID in (select CardType_RID from perso_cardtype where rst='a' and base_special='1' and cardtype_rid not in (select cardtype_rid from perso_cardtype where rst='a' and base_special='2')) ";



    public const string SEL_ExistBase = "SELECT CardType_RID "
                                        + "from PERSO_CARDTYPE "
                                        + "where CardType_RID=@cardtype_rid and Base_Special =@base_special ";


    public const string SEL_PERSO_CARDTYPE = "SELECT S_PS.Percentage_Number,B_PS.TYPE,B_PS.AFFINITY,B_PS.PHOTO,B_PS.RID, B_PS.CardType_RID, B_PS.Factory_RID, B_PS.Factory_ShortName_CN AS B_FNAME,FC2.Factory_ShortName_CN AS S_FNAME,B_PS.Factory_ID as B_Factory_ID,FC2.Factory_ID as S_Factory_ID, CASE WHEN S_PS.Percentage_Number = '1' THEN S_PS.Value  ELSE '' END AS RATE_VALUE,CASE WHEN S_PS.Percentage_Number = '2' THEN S_PS.Value ELSE '' END AS NUM_VALUE,S_PS.PRIORITY "
                                        + "FROM (SELECT PC.RID, PC.CardType_RID, CT.Name,CT.TYPE,CT.AFFINITY,CT.PHOTO,PC.Factory_RID, FC.Factory_ID,FC.Factory_ShortName_CN FROM PERSO_CARDTYPE PC INNER JOIN CARD_TYPE CT ON PC.CardType_RID = CT.RID AND CT.RST = 'A' INNER JOIN FACTORY FC ON PC.Factory_RID = FC.RID AND FC.RST = 'A' WHERE pc.RST = 'A' AND pc.Base_Special = '1') AS B_PS "
                                        + "LEFT OUTER JOIN PERSO_CARDTYPE AS S_PS ON B_PS.CardType_RID = S_PS.CardType_RID AND S_PS.Base_Special = '2'  AND S_PS.RST = 'A'  LEFT OUTER JOIN FACTORY FC2 ON S_PS.Factory_RID = FC2.RID AND FC2.RST = 'A' "
                                        + "WHERE 1=1 ";

    public const string SEL_CARDTYPE_BY_RID = "SELECT Display_Name as Name "
                                    + "FROM CARD_TYPE "
                                    + "WHERE RID=@rid";
    public const string SEL_CARDTYPE_BY_CARDTYPE_RID = "SELECT Factory_RID,perso.RID,Percentage_Number,Value,Priority "
                                   + "FROM PERSO_CARDTYPE perso LEFT JOIN FACTORY f ON perso.Factory_RID = f.RID "
                                   + "WHERE perso.Base_Special = '2' AND perso.RST='A' AND perso.CardType_RID = @cardType_rid";

    public const string UPDATE_PERSO_CARDTYPE_Base = "DELETE FROM PERSO_CARDTYPE "
                                    + "WHERE RST = 'A'  AND  CardType_RID = @cardType_rid AND Base_Special='1'";

    //public const string UPDATE_PERSO_CARDTYPE_SPECIAL = "UPDATE  PERSO_CARDTYPE SET  RST = 'D'"
    //                            + "WHERE RST = 'A'  AND  CardType_RID = @cardType_rid AND Base_Special='2'";
    public const string UPDATE_PERSO_CARDTYPE_SPECIAL = "DELETE  FROM PERSO_CARDTYPE "
                            + "WHERE RST = 'A'  AND  CardType_RID = @cardType_rid AND Base_Special='2'";

    public const string SEL_CARDTYPE_BY_FACTORYRID = "SELECT  perso.CardType_RID RID,type.Display_Name as Name "
                                + "FROM PERSO_CARDTYPE perso LEFT JOIN CARD_TYPE type ON perso.CardType_RID=type.RID "
                                + "WHERE perso.Factory_RID = @Factory_RID AND perso.RST='A'  AND Base_Special='1' ";

    public const string SEL_FACTORY_BY_RID = "SELECT RID,Factory_ShortName_CN "
                            + "FROM FACTORY "
                            + "WHERE RID = @Factory_RID";
    public const string UPDATE_PERSO_CARDTYPE_BASE = "DELETE FROM PERSO_CARDTYPE "
                        + "WHERE RST = 'A' AND Factory_RID = @Factory_RID AND Base_Special=1";

    public const string SEL_PERSO_CARDTYPE_CardType_RID_Special = "select * FROM PERSO_CARDTYPE  "
                        + "WHERE RST = 'A' AND CardType_RID = @cardtype_rid AND Base_Special=2";

    public const string DELETE_PERSO_CARDTYPE_BASE = "DELETE  FROM PERSO_CARDTYPE "
                       + "WHERE RST = 'A' AND Factory_RID = @Factory_RID AND Base_Special=1";

    public const string SEL_FACTORY_Factory_RID = "select Factory_ShortName_CN "
                            + "from FACTORY "
                            + "where RID = @rid";
    public const string CHK_ORDER_FORM_DETAIL_BY_CardType_RID = "proc_CHK_DEL_ORDER_FORM_DETAIL_CardType_RID";
    public const string CHK_ORDER_FORM_DETAIL_BY_Factory_RID = "proc_CHK_DEL_ORDER_FORM_DETAIL_Factory_RID";

    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public CardType005BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }


    /// <summary>
    /// 獲得Perso廠商
    /// </summary>
    /// <returns>DataSet[Perso廠商]</returns>
    public DataSet GetFactoryList()
    {
        DataSet dstFactory = null;
        try
        {
            this.dirValues.Clear();
            dstFactory = dao.GetList(SEL_FACTORY, dirValues);

            return dstFactory;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 當前卡種是否在資料庫中存在基本設定
    /// </summary>
    /// <param name="pcModel">Perso廠與卡種設定檔新增基本資料</param>
    /// <param name="dtblCardType">卡種</param>
    public ArrayList BaseExist(DataTable dtblCardType)
    {
        PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
        DataSet dsExistBase = null;
        ArrayList ExistBase = new ArrayList();
        dirValues.Clear();
        try
        {
            //事務開始
            dao.OpenConnection();

            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                pcModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                pcModel.Base_Special = GlobalString.BaseSpecial.Base;//廠商基本
                dirValues.Add("cardtype_rid", pcModel.CardType_RID);
                dirValues.Add("base_special", pcModel.Base_Special);
                dsExistBase = dao.GetList(SEL_ExistBase, dirValues);
                dirValues.Clear();
                if (dsExistBase.Tables[0].Rows.Count >= 1)
                {
                    ExistBase.Add(dsExistBase.Tables[0].Rows[0]["CardType_RID"].ToString());
                }
            }

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
        return ExistBase;
    }

    /// <summary>
    /// Perso廠與卡種特殊設定新增設定明細比率
    /// </summary>
    /// <param name="pcModel">Perso廠與卡種設定檔新增基本資料</param>
    /// <param name=""></param>
    public void AddSpecialPercentage(string strCardType_RID, DataTable Percentage_Number, string strPercentage_Number)
    {
        PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
        try
        {
            //事務開始
            dao.OpenConnection();
            //ChkDelORDER_FORM_DETAIL_BY_CardType_RID(strCardType_RID);
            dirValues.Clear();
            pcModel.CardType_RID = Convert.ToInt32(strCardType_RID);
            dirValues.Add("cardtype_rid", pcModel.CardType_RID);

            dao.ExecuteNonQuery(UPDATE_PERSO_CARDTYPE_SPECIAL, dirValues);

            string strFactory = "";
            //todo  與設計有出入
            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowPercentage_Number in Percentage_Number.Rows)
            {
                //pcModel.RID = Convert.ToInt32(drowPercentage_Number["RID"]);
                pcModel.Priority = Convert.ToInt32(drowPercentage_Number["Priority"]);
                pcModel.Factory_RID = Convert.ToInt32(drowPercentage_Number["Factory_RID"]);
                pcModel.Value = Convert.ToInt32(drowPercentage_Number["Value"]);
                pcModel.Base_Special = GlobalString.BaseSpecial.Special;//廠商特殊
                pcModel.Percentage_Number = strPercentage_Number;

                dirValues.Clear();
                dirValues.Add("RID", pcModel.Factory_RID);
                DataTable dtbl = dao.GetList(SEL_FACTORY_Factory_RID, dirValues).Tables[0];
                if (dtbl.Rows.Count > 0)
                    strFactory += dtbl.Rows[0][0].ToString()+",";


                dao.Add<PERSO_CARDTYPE>(pcModel, "RID");
            }
            if (strFactory != "")
                strFactory = strFactory.Substring(0, strFactory.Length - 1);

            string strCardName = "";

            dirValues.Clear();
            dirValues.Add("RID", strCardType_RID);
            DataTable dtblCard = dao.GetList(SEL_CARDTYPE_BY_RID, dirValues).Tables[0];
            if (dtblCard.Rows.Count > 0)
                strCardName = dtblCard.Rows[0][0].ToString();


            if (!StringUtil.IsEmpty(strCardName))
            {
                strCardName = strCardName.Substring(12);
                Warning.SetWarning(GlobalString.WarningType.Perso_CardTypeEdit, new object[2] { strCardName, strFactory });
            }


            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
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
    /// Perso廠與卡種特殊設定新增
    /// </summary>
    /// <param name="pcModel"></param>
    /// <param name="dtblCardType">卡種</param>
    public void AddSpecial(DataTable Percentage_Number, DataTable dtblCardType, string strPercentage_Number)
    {
        PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
        try
        {
            //事務開始
            dao.OpenConnection();

            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                foreach (DataRow drowPercentage_Number in Percentage_Number.Rows)
                {
                    pcModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                    pcModel.Priority = Convert.ToInt32(drowPercentage_Number["Priority"]);
                    pcModel.Factory_RID = Convert.ToInt32(drowPercentage_Number["Factory"]);

                    if (drowPercentage_Number["Value"].ToString().Trim() == "")
                    {
                        pcModel.Value = 0;
                    }
                    else
                    {
                        pcModel.Value = Convert.ToInt32(drowPercentage_Number["Value"]);
                    }
                    
                    pcModel.Base_Special = GlobalString.BaseSpecial.Special;//廠商特殊
                    pcModel.Percentage_Number = strPercentage_Number;
                    dao.Add<PERSO_CARDTYPE>(pcModel, "RID");
                }
            }

            //foreach更新已選擇的卡種記錄
            //foreach (DataRow drowCardType in dtblCardType.Rows)
            //{
            //    foreach (DataRow drowPercentage_Number in Percentage_Number.Rows)
            //    {
            //        pcModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
            //        pcModel.Priority = Convert.ToInt32(drowPercentage_Number["Priority"]);
            //        pcModel.Factory_RID = Convert.ToInt32(drowPercentage_Number["Factory"]);
            //        pcModel.Percentage_Number = strPercentage_Number;
            //        pcModel.Base_Special = GlobalString.BaseSpecial.Special;//廠商特殊
            //        if (drowPercentage_Number["Percentage_Number"].ToString().Trim() == "")
            //        {
            //            pcModel.Value = 0;
            //        }
            //        else
            //        {
            //            pcModel.Value = Convert.ToInt32(drowPercentage_Number["Percentage_Number"]);
            //        }
            //        dao.Add<PERSO_CARDTYPE>(pcModel, "RID");
            //    }
            //}

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
    /// 查詢Perso廠與卡種設定
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
        string strSortField = (sortField == "null" ? "type,affinity,photo" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_PERSO_CARDTYPE);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        //string cardtype_rid = "";
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (((DataTable)searchInput["uctrlCARDNAME"]).Rows.Count != 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["uctrlCARDNAME"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";

                stbWhere.Append(" and B_PS.CardType_RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ") ");
            }

            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND B_PS.Factory_RID = @factory_rid ");
                dirValues.Add("factory_rid", searchInput["dropFactory"].ToString().Trim());
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
    /// 獲取卡種名稱
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns>DataSet</returns>
    public string GetCardTypeName(string rid)
    {
        DataSet dtsparam_SendCheck = null;
        string name = "";
        dirValues.Clear();
        try
        {
            dirValues.Add("rid", rid);
            dtsparam_SendCheck = dao.GetList(SEL_CARDTYPE_BY_RID, dirValues);
            if (dtsparam_SendCheck.Tables[0].Rows.Count != 0)
            {
                name = dtsparam_SendCheck.Tables[0].Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return name;
    }

    /// <summary>
    /// 獲取廠商名稱
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns>DataSet</returns>
    public string GetFactory(string strFactory_RID)
    {
        DataSet dtFactory_RID = null;
        string name = "";
        dirValues.Clear();
        try
        {
            dirValues.Add("Factory_RID", strFactory_RID);
            dtFactory_RID = dao.GetList(SEL_FACTORY_BY_RID, dirValues);
            if (dtFactory_RID.Tables[0].Rows.Count != 0)
            {
                name = dtFactory_RID.Tables[0].Rows[0][1].ToString();
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return name;
    }

    /// <summary>
    /// 查詢該PERSO廠對應的所有基本設定的卡種信息列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns>DataSet</returns>
    public DataTable GetSelectedCardType(string strFactory_RID)
    {
        DataSet dsSelectedCardType = null;
        DataTable dtSelectedCardType = null;
        dirValues.Clear();
        try
        {
            dirValues.Add("Factory_RID", strFactory_RID);
            dsSelectedCardType = dao.GetList(SEL_CARDTYPE_BY_FACTORYRID + " order by Display_Name", dirValues);
            dtSelectedCardType = dsSelectedCardType.Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dtSelectedCardType;
    }

    /// <summary>
    /// 獲取該卡種對應的所有設定明細信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns>DataSet</returns>
    public DataSet GetPersoCardTypeList(string cardType_rid)
    {
        DataSet dtsparam_SendCheck = null;
        dirValues.Clear();
        try
        {
            dirValues.Add("cardType_rid", cardType_rid);
            dtsparam_SendCheck = dao.GetList(SEL_CARDTYPE_BY_CARDTYPE_RID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dtsparam_SendCheck;
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="strRID">RID</param>
    public void DeleteSpecial(string strRID)
    {

        PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();

        try
        {
            //事務開始
            dao.OpenConnection();
            //ChkDelORDER_FORM_DETAIL_BY_CardType_RID(strRID);
            dirValues.Clear();
            pcModel.CardType_RID = Convert.ToInt32(strRID);
            dirValues.Add("cardtype_rid", pcModel.CardType_RID);

            dao.ExecuteNonQuery(UPDATE_PERSO_CARDTYPE_SPECIAL, dirValues);

            //操作日誌
            SetOprLog("4");

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            //ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            //throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
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
    /// 刪除基本卡種設定
    /// </summary>
    /// <param name="strRID">RID</param>
    public void DeleteBase(string strFactory_RID,string strFactoryName, DataTable dtblCardType)
    {
        PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
        DataSet dsPERSO_CARDTYPE = null;
        try
        {
            //事務開始
            dao.OpenConnection();

            string strCardName = "";

            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                strCardName += drowCardType["name"].ToString().Substring(10) + "，";
                dirValues.Clear();
                dirValues.Add("cardtype_rid", drowCardType["RID"].ToString().Trim());
                dsPERSO_CARDTYPE = dao.GetList(SEL_PERSO_CARDTYPE_CardType_RID_Special, dirValues);
                if (dsPERSO_CARDTYPE.Tables[0].Rows.Count != 0)
                {
                    throw new AlertException("【" + drowCardType[1].ToString() + "】存在特殊的設定，請先刪除特殊的設定!");
                }
                //ChkDelORDER_FORM_DETAIL_BY_Factory_RID(strFactory_RID, drowCardType["RID"].ToString());
            }

            dirValues.Clear();
            pcModel.Factory_RID = Convert.ToInt32(strFactory_RID);
            dirValues.Add("Factory_RID", pcModel.Factory_RID);

            dao.ExecuteNonQuery(UPDATE_PERSO_CARDTYPE_BASE, dirValues);

            //操作日誌
            SetOprLog("4");

            if (!StringUtil.IsEmpty(strCardName))
            {
                strCardName = strCardName.Substring(0, strCardName.Length - 1);
                Warning.SetWarning(GlobalString.WarningType.Perso_CardTypeEdit, new object[2] { strCardName, strFactoryName });
            }

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            //ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            //throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
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
    /// 修改基本卡種設定
    /// </summary>
    /// <param name="strRID">RID</param>
    public void UpdateBase(string strFactory_RID,string strFactoryName, DataTable dtblCardType, DataTable Old_dtblCardType)
    {

        PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
        bool IsExist = false;
        DataSet dsPERSO_CARDTYPE = null;
        try
        {
            //事務開始
            dao.OpenConnection();

            string strCardName = "";

            //foreach (DataRow drowCardType in dtblCardType.Rows)
            //{
            //    strCardName += drowCardType["name"].ToString().Substring(12) + "，";
            //}

            //foreach比較之前和之後已選擇的卡種記錄
            foreach (DataRow drowOldCardType in Old_dtblCardType.Rows)
            {
                foreach (DataRow drowCardType in dtblCardType.Rows)
                {
                    if (drowOldCardType["RID"].ToString().Trim() == drowCardType["RID"].ToString().Trim())
                    {
                        IsExist = true;
                        break;
                    }
                }
                if (IsExist == false)
                {
                    dirValues.Clear();
                    dirValues.Add("cardtype_rid", drowOldCardType["RID"].ToString());
                    dsPERSO_CARDTYPE = dao.GetList(SEL_PERSO_CARDTYPE_CardType_RID_Special, dirValues);
                    if (dsPERSO_CARDTYPE.Tables[0].Rows.Count != 0)
                    {
                        throw new AlertException("【" + drowOldCardType[1].ToString() + "】存在特殊的設定，請先刪除特殊的設定!");
                    }
                    //ChkDelORDER_FORM_DETAIL_BY_Factory_RID(strFactory_RID, drowOldCardType["RID"].ToString());//廠商地址被使用到
                }
                IsExist = false;
            }

            dirValues.Clear();
            dirValues.Add("Factory_RID", strFactory_RID);
            dao.ExecuteNonQuery(DELETE_PERSO_CARDTYPE_BASE, dirValues);
            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                pcModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                pcModel.Factory_RID = Convert.ToInt32(strFactory_RID);
                pcModel.Base_Special = GlobalString.BaseSpecial.Base;//廠商基本
                dao.Add<PERSO_CARDTYPE>(pcModel, "RID");
            }
            //操作日誌
            SetOprLog();

            //if (!StringUtil.IsEmpty(strCardName))
            //{
            //strCardName = strCardName.Substring(0, strCardName.Length - 1);
            Warning.SetWarning(GlobalString.WarningType.Perso_CardTypeEdit, new object[2] { strCardName, strFactoryName });
            //}


            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            //ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            //throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    //private void DelBaseUp(string strFactory_RID, PERSO_CARDTYPE pcModel)
    //{
    //    //pcModel.Factory_RID = Convert.ToInt32(strFactory_RID);
    //    dirValues.Add("Factory_RID", strFactory_RID);

    //    dao.ExecuteNonQuery(DELETE_PERSO_CARDTYPE_BASE, dirValues);

    //    dirValues.Clear();
    //}

    /// <summary>
    /// Perso廠與卡種基本設定新增
    /// </summary>
    /// <param name="pcModel">Perso廠與卡種設定檔新增基本資料</param>
    /// <param name="dtblCardType">卡種</param>
    public void AddBase(PERSO_CARDTYPE pcModel, DataTable dtblCardType)
    {
        try
        {
            //事務開始
            dao.OpenConnection();

            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                pcModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                pcModel.Base_Special = GlobalString.BaseSpecial.Base;//廠商基本
                dao.Add<PERSO_CARDTYPE>(pcModel, "RID");
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
    /// 獲取廠商編號
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public string GetFactory_RID(string rid)
    {
        DataSet dsFactory_RID = null;
        string Factory_RID = "";
        dirValues.Clear();
        try
        {
            dirValues.Add("rid", rid);
            dsFactory_RID = dao.GetList(SEL_FACTORY_Factory_RID, dirValues);
            if (dsFactory_RID.Tables[0] != null)
            {
                if (dsFactory_RID.Tables[0].Rows.Count != 0)
                {
                    Factory_RID = dsFactory_RID.Tables[0].Rows[0][0].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return Factory_RID;
    }

    /// <summary>
    /// 檢查卡種在採購下單放行是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelORDER_FORM_DETAIL_BY_CardType_RID(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("CardType_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_ORDER_FORM_DETAIL_BY_CardType_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "卡種"));

    }

    /// <summary>
    /// 檢查廠商在採購下單放行是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelORDER_FORM_DETAIL_BY_Factory_RID(string strFactory_RID, string strCardType_RID)
    {
        dirValues.Clear();
        dirValues.Add("Factory_RID", strFactory_RID);
        dirValues.Add("CardType_RID", strCardType_RID);
        DataSet dstBudget = dao.GetList(CHK_ORDER_FORM_DETAIL_BY_Factory_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "卡種"));

    }
}
