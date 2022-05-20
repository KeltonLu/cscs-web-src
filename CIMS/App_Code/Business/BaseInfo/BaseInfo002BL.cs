//******************************************************************
//*  作    者：FangBao
//*  功能說明：卡片合約管理邏輯
//*  創建日期：2008-09-09
//*  修改日期：2008-09-09 12:00
//*  修改記錄：
//*            □2008-09-09
//*              1.創建 鮑方
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
using System.Collections;

/// <summary>
/// 卡片合約管理邏輯
/// </summary>
public class BaseInfo002BL : BaseLogic
{
    #region SQL語句
    public const string SEL_AGREEMENT_LIST = " select AM.Remain_Card_Num,AM.Agreement_Code,AM.Agreement_Name,AM.RID,AM.Card_Number,AM.Begin_Time,AM.End_Time,AM.Factory_RID,F_B.Factory_ShortName_CN,AM.Agreement_Code_Main from AGREEMENT AM LEFT JOIN FACTORY F_B ON AM.Factory_RID=F_B.RID and F_B.RST = 'A' AND F_B.IS_BLANK ='Y' where AM.RST='A'";
    public const string SEL_FACTORY_BLANK = " select RID,Factory_ShortName_cn from FACTORY WHERE RST = 'A' AND IS_BLANK ='Y'";
    public const string SEL_GROUP_List = "select * from AGREEMENT_CARDTYPE_GROUP where RST='A' AND Agreement_Main_RID=@Agreement_Main_RID";
    public const string SEL_CARD_List = "select GC.*,CT.Display_Name as Name from dbo.GROUP_CARDTYPE GC left join CARD_TYPE CT ON CT.RID=GC.CardType_RID AND CT.RST='A' where GC.RST='A' AND GC.Agreement_Group_RID=@Agreement_Group_RID";

    public const string SEL_LEVEL_List = "select GLP.*,GC.CARDTYPE_RID,GC.PARAM_RID,GC.PARAM_NAME from dbo.GROUP_LEVEL_PRICE GLP left join (SELECT GC.*,PM.PARAM_NAME FROM GROUP_CARDTYPE GC LEFT JOIN PARAM PM ON GC.PARAM_RID=PM.RID AND PM.RST='A' WHERE GC.RST='A') GC ON GLP.Group_CardType_RID=GC.RID AND GC.RST='A' where Group_CardType_RID in (select rid from dbo.GROUP_CARDTYPE where Agreement_Group_RID=@Agreement_Group_RID) AND GLP.RST='A'";

    public const string SEL_MATERIAL_List = "select CGM.*,MS.Material_name from CARDTYPE_GROUP_MATERIAL CGM left join MATERIAL_SPECIAL MS ON CGM.Material_RID=MS.RID where CGM.Agreement_Group_RID=@Agreement_Group_RID AND CGM.RST='A'";
    public const string SEL_MATERIAL = "select RID,Material_Name from MATERIAL_SPECIAL WHERE RST='A'";
    public const string SEL_CardType = "SELECT rid,param_name FROM PARAM WHERE RST='A' and ParamType_Code=@ParamType_Code";
    public const string SEL_AGREEMENTBAK_List = "select AG.*,FA.Factory_ShortName_CN as Factory_Name from AGREEMENT AG left join FACTORY FA ON FA.RST='A' AND FA.RID=AG.Factory_RID AND FA.Is_Blank='Y' WHERE AG.RST='A' AND AG.Agreement_Code_Main=@Agreement_Code_Main";
    public const string CON_AGREEMENT = "select count(*) from AGREEMENT WHERE Agreement_Code=@Agreement_Code ";
    public const string DEL_GROUP_ByAg_Code = "delete from dbo.AGREEMENT_CARDTYPE_GROUP where Agreement_Main_RID in (select RID from dbo.AGREEMENT where Agreement_Code=@Agreement_Code union select RID from AGREEMENT where Agreement_Code_Main=@Agreement_Code)";
    public const string DEL_CARD_ByAg_Code = "delete FROM dbo.GROUP_CARDTYPE WHERE Agreement_Group_RID IN (select RID from dbo.AGREEMENT_CARDTYPE_GROUP where Agreement_Main_RID in (select RID from dbo.AGREEMENT where Agreement_Code=@Agreement_Code union select RID from AGREEMENT where Agreement_Code_Main=@Agreement_Code))";
    public const string DEL_MATERIAL_ByAg_Code = "delete FROM dbo.CARDTYPE_GROUP_MATERIAL WHERE Agreement_Group_RID IN (select RID from dbo.AGREEMENT_CARDTYPE_GROUP where Agreement_Main_RID in (select RID from dbo.AGREEMENT where Agreement_Code=@Agreement_Code union select RID from AGREEMENT where Agreement_Code_Main=@Agreement_Code))";
    public const string DEL_LEVEL_ByAg_Code = "delete from dbo.GROUP_LEVEL_PRICE where Group_CardType_RID in (select rid FROM dbo.GROUP_CARDTYPE WHERE Agreement_Group_RID IN (select RID from dbo.AGREEMENT_CARDTYPE_GROUP where Agreement_Main_RID in (select RID from dbo.AGREEMENT where Agreement_Code=@Agreement_Code union select RID from AGREEMENT where Agreement_Code_Main=@Agreement_Code)))";
    public const string CHK_AGREEMENT_BY_RID = "proc_CHK_DEL_Agreement";
    public const string SEL_AGREEMENT_USED_BY_RID = "select min(rct) as mintime,max(rut) as maxtime from dbo.ORDER_FORM_DETAIL where agreement_rid in (select tb2.rid from agreement tb1 inner join (select factory_rid,rid,case Agreement_Code_Main when '' then Agreement_Code else Agreement_Code_Main end as Agreement_Code from agreement) tb2 on tb1.Agreement_Code=tb2.Agreement_Code where tb1.rid=@agreement_rid)";

    public const string CHK_ORDERFORM = "SELECT COUNT(*) FROM dbo.ORDER_FORM_DETAIL WHERE AGREEMENT_RID=@AGREEMENT_RID";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo002BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }


    /// <summary>
    /// 獲取合約使用時間
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DataTable GetAgreementUserdTime(string strRID)
    {
        DataTable dtbl = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("agreement_rid", strRID);
            dtbl = dao.GetList(SEL_AGREEMENT_USED_BY_RID, dirValues).Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dtbl;
    }

    /// <summary>
    /// 卡別下拉框綁定
    /// </summary>
    /// <returns></returns>
    public DataTable CardType()
    {
        DataTable dtbl = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("ParamType_Code", GlobalString.ParameterType.CardType);
            dtbl = dao.GetList(SEL_CardType, dirValues).Tables[0];
            dtbl.PrimaryKey = new DataColumn[] { dtbl.Columns["RID"] };
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dtbl;
    }

    /// <summary>
    /// 材質下拉框綁定
    /// </summary>
    /// <returns></returns>
    public DataSet MaterialList()
    {
        DataSet dst = null;
        try
        {
            dst = dao.GetList(SEL_MATERIAL);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
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
        string strSortField = (sortField == "null" ? "Agreement_Code" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_AGREEMENT_LIST);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtAgreement_Code"].ToString().Trim()))
            {
                stbWhere.Append(" and AM.Agreement_Code like @Agreement_Code");
                dirValues.Add("Agreement_Code", "%" + searchInput["txtAgreement_Code"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["txtAgreement_Name"].ToString().Trim()))
            {
                stbWhere.Append(" and AM.Agreement_Name like @Agreement_Name");
                dirValues.Add("Agreement_Name", "%" + searchInput["txtAgreement_Name"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory_RID"].ToString().Trim()))
            {
                stbWhere.Append(" and AM.Factory_RID=@Factory_RID");
                dirValues.Add("Factory_RID", searchInput["dropFactory_RID"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtCard_Number1"].ToString().Trim()))
            {
                stbWhere.Append(" and AM.Card_Number>=@Card_Number1");
                dirValues.Add("Card_Number1", searchInput["txtCard_Number1"].ToString().Trim().Replace(",",""));
            }
            if (!StringUtil.IsEmpty(searchInput["txtCard_Number2"].ToString().Trim()))
            {
                stbWhere.Append(" and AM.Card_Number<=@Card_Number2");
                dirValues.Add("Card_Number2", searchInput["txtCard_Number2"].ToString().Trim().Replace(",", ""));
            }
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Time"].ToString().Trim()))
            {
                stbWhere.Append(" and Begin_Time>=@Begin_Time");
                dirValues.Add("Begin_Time", searchInput["txtBegin_Time"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtEnd_Time"].ToString().Trim()))
            {
                stbWhere.Append(" and End_Time<=@End_Time");
                dirValues.Add("End_Time", searchInput["txtEnd_Time"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["UrctrlCardTypeSelect"].ToString().Trim()))
            {
                stbWhere.Append(" and AM.RID in (select Agreement_Main_RID from AGREEMENT_CARDTYPE_GROUP where RST='A' AND RID IN (select Agreement_Group_RID from GROUP_CARDTYPE where RST='A' AND CardType_RID=@CardType_RID))");
                dirValues.Add("CardType_RID", searchInput["UrctrlCardTypeSelect"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dstcard_Budget = null;
        try
        {
            dstcard_Budget = dao.GetList(stbCommand.ToString() + stbWhere.ToString() + " and Agreement_Code_Main=''", dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
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
    /// 獲取備源合約
    /// </summary>
    /// <param name="strAgreement_Code"></param>
    /// <returns></returns>
    public DataTable GetListDetail(string strAgreement_Code)
    {
        DataTable dtbl = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("Agreement_Code", strAgreement_Code);
            dtbl = dao.GetList(SEL_AGREEMENT_LIST + " and Agreement_Code_Main=@Agreement_Code", dirValues).Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dtbl;
    }

    /// <summary>
    /// 獲取空白卡廠
    /// </summary>
    /// <returns></returns>
    public DataSet GetFacotry()
    {
        DataSet dst = null;
        try
        {
            dst = dao.GetList(SEL_FACTORY_BLANK);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 判斷合約編號是否存在
    /// </summary>
    /// <param name="strUserID">合約編號</param>
    public bool ContainsAgreement(string strAgreement_Code,string strAg_Code)
    {
        dirValues.Clear();
        dirValues.Add("Agreement_Code", strAgreement_Code);
        StringBuilder stbCommand = new StringBuilder(CON_AGREEMENT);
        if (!StringUtil.IsEmpty(strAg_Code))
        {
            stbCommand.Append(" and Agreement_Code!=@Agreement_Code1 and Agreement_Code_Main !=@Agreement_Code1");
            dirValues.Add("Agreement_Code1", strAg_Code);
        }

        return dao.Contains(stbCommand.ToString(), dirValues);
    }

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="dirAgreement"></param>
    public void Add(Dictionary<string, DataTable> dirAgreement)
    {
        AGREEMENT aModel = new AGREEMENT();
        AGREEMENT aModelBak = new AGREEMENT();
        AGREEMENT_CARDTYPE_GROUP acgModel = new AGREEMENT_CARDTYPE_GROUP();
        AGREEMENT_CARDTYPE_GROUP acgModelBak = new AGREEMENT_CARDTYPE_GROUP();
        GROUP_CARDTYPE gcModel = new GROUP_CARDTYPE();
        GROUP_CARDTYPE gcModelBak = new GROUP_CARDTYPE();
        CARDTYPE_GROUP_MATERIAL cgmModel = new CARDTYPE_GROUP_MATERIAL();
        CARDTYPE_GROUP_MATERIAL cgmModelBak = new CARDTYPE_GROUP_MATERIAL();

        GROUP_LEVEL_PRICE glpModel = new GROUP_LEVEL_PRICE();
        GROUP_LEVEL_PRICE glpBakModel = new GROUP_LEVEL_PRICE();

        try
        {
            dao.OpenConnection();

            #region 增加主合約
            aModel = dao.GetModelByDataRow<AGREEMENT>(dirAgreement["Agreement"].Rows[0]);
            aModel.Remain_Card_Num = aModel.Card_Number;
            int intAgreementRID = (int)dao.AddAndGetID<AGREEMENT>(aModel, "RID");
            #endregion

            #region 增加主合約群組
            foreach (DataRow drowGroup in dirAgreement["Group"].Rows)
            {
                acgModel = dao.GetModelByDataRow<AGREEMENT_CARDTYPE_GROUP>(drowGroup);
                acgModel.Agreement_Main_RID = intAgreementRID;

                int intRID = (int)dao.AddAndGetID<AGREEMENT_CARDTYPE_GROUP>(acgModel, "RID");

                DataRow[] drowCards = dirAgreement["Card"].Select("ID='" + drowGroup["ID"].ToString()+"'");
                DataRow[] drowMaterials = dirAgreement["Material"].Select("ID='" + drowGroup["ID"].ToString()+"'");

                //增加主合約卡種
                foreach (DataRow drowCard in drowCards)
                {
                    gcModel = dao.GetModelByDataRow<GROUP_CARDTYPE>(drowCard);
                    gcModel.Agreement_Group_RID = intRID;
                    

                    //增加級距
                    DataRow[] drowLevels = dirAgreement["Level"].Select("ID='" + drowGroup["ID"].ToString() + "' and CardType_RID='" + gcModel.CardType_RID+"'");
                    if (drowLevels.Length > 0)
                        gcModel.Param_RID = Convert.ToInt32(drowLevels[0]["Param_RID"]);
                    else
                        gcModel.Param_RID = 0;

                    int intCardRID = Convert.ToInt32(dao.AddAndGetID<GROUP_CARDTYPE>(gcModel, "RID"));

                    foreach(DataRow drowLevel in drowLevels)
                    {
                        glpModel = dao.GetModelByDataRow<GROUP_LEVEL_PRICE>(drowLevel);
                        glpModel.Group_CardType_RID = intCardRID;
                        dao.Add<GROUP_LEVEL_PRICE>(glpModel, "RID");
                    }
                }
                //增加主合約材質
                foreach (DataRow drowMaterial in drowMaterials)
                {
                    cgmModel = dao.GetModelByDataRow<CARDTYPE_GROUP_MATERIAL>(drowMaterial);
                    cgmModel.Agreement_Group_RID = intRID;
                    dao.Add<CARDTYPE_GROUP_MATERIAL>(cgmModel, "RID");
                }
            }

            #endregion 
            
            #region 備援合約
            foreach (DataRow drowAgreementBak in dirAgreement["AgreementBak"].Rows)
            {
                aModelBak = dao.GetModelByDataRow<AGREEMENT>(drowAgreementBak);
                aModelBak.Agreement_Code_Main = aModel.Agreement_Code;
                aModelBak.Remain_Card_Num = aModelBak.Card_Number;
                int intAgreementBakRID = (int)dao.AddAndGetID<AGREEMENT>(aModelBak, "RID");

                DataRow[] drowGroupBaks = dirAgreement["GroupBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString()+"'");

                //備援合約群組
                foreach (DataRow drowGroupBak in drowGroupBaks)
                {
                    acgModelBak = dao.GetModelByDataRow<AGREEMENT_CARDTYPE_GROUP>(drowGroupBak);
                    acgModelBak.Agreement_Main_RID = intAgreementBakRID;

                    int intRID = (int)dao.AddAndGetID<AGREEMENT_CARDTYPE_GROUP>(acgModelBak, "RID");

                    DataRow[] drowCardBaks = dirAgreement["CardBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString() + "' and ID='" + drowGroupBak["ID"].ToString()+"'");
                    DataRow[] drowMaterialBaks = dirAgreement["MaterialBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString() + "' and ID='" + drowGroupBak["ID"].ToString()+"'");


                    //增加備援合約卡種
                    foreach (DataRow drowCardBak in drowCardBaks)
                    {
                        if(StringUtil.IsEmpty(drowCardBak[5].ToString()))
                            drowCardBak[5]=0;

                        gcModelBak = dao.GetModelByDataRow<GROUP_CARDTYPE>(drowCardBak);
                        gcModelBak.Agreement_Group_RID = intRID;


                        //增加備援級距
                        DataRow[] drowLevelBaks = dirAgreement["LevelBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString() + "' and ID='" + drowGroupBak["ID"].ToString() + "' and CardType_RID='" + gcModelBak.CardType_RID+"'");
                        if (drowLevelBaks.Length > 0)
                            gcModelBak.Param_RID = Convert.ToInt32(drowLevelBaks[0]["Param_RID"]);
                        else
                            gcModelBak.Param_RID = 0;

                        int intCardBakRID = Convert.ToInt32(dao.AddAndGetID<GROUP_CARDTYPE>(gcModelBak, "RID"));

                        foreach (DataRow drowLevelBak in drowLevelBaks)
                        {
                            glpBakModel = dao.GetModelByDataRow<GROUP_LEVEL_PRICE>(drowLevelBak);
                            glpBakModel.Group_CardType_RID = intCardBakRID;
                            dao.Add<GROUP_LEVEL_PRICE>(glpBakModel, "RID");
                        }
                    }

                    //增加備援合約材質
                    foreach (DataRow drowMaterialBak in drowMaterialBaks)
                    {
                        cgmModelBak = dao.GetModelByDataRow<CARDTYPE_GROUP_MATERIAL>(drowMaterialBak);
                        cgmModelBak.Agreement_Group_RID = intRID;
                        dao.Add<CARDTYPE_GROUP_MATERIAL>(cgmModelBak, "RID");
                    }
                }
            }
            #endregion

            //操作日誌
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

    public Dictionary<string, DataTable> LoadAgreementEditInfo(Dictionary<string, DataTable> dirAgreement, string strRID)
    {
        AGREEMENT aModel = new AGREEMENT();
        
        DataTable dtblAgreement = dirAgreement["Agreement"];
        DataTable dtblAgreementTmp = dirAgreement["AgreementTmp"];
        DataTable dtblAgreementBak = dirAgreement["AgreementBak"];
        DataTable dtblGroup = dirAgreement["Group"];
        DataTable dtblCard = dirAgreement["Card"];
        DataTable dtblGroupBak = dirAgreement["GroupBak"];
        DataTable dtblCardBak = dirAgreement["CardBak"];
        DataTable dtblMaterial = dirAgreement["Material"];
        DataTable dtblMaterialBak = dirAgreement["MaterialBak"];

        DataTable dtblLevel = dirAgreement["Level"];
        DataTable dtblLevelBak = dirAgreement["LevelBak"];

        try
        {
            aModel = dao.GetModel<AGREEMENT, int>("RID", int.Parse(strRID));

            #region 主合約

            dirValues.Clear();
            dirValues.Add("Agreement_RID", aModel.RID);

            DataSet dstDelCount = dao.GetList(CHK_AGREEMENT_BY_RID, dirValues, true);

            
            DataRow drowAgreement = dtblAgreement.NewRow();
            DataRow drowAgreementTmp = dtblAgreementTmp.NewRow();
            drowAgreement["RID"] = aModel.RID;
            drowAgreement["Agreement_Name"] = aModel.Agreement_Name;
            drowAgreement["IMG_File_URL"] = aModel.IMG_File_URL;
            drowAgreement["IMG_File_Name"] = aModel.IMG_File_Name;
            drowAgreement["Factory_RID"] = aModel.Factory_RID;
            drowAgreement["Card_Number"] = aModel.Card_Number;
            drowAgreement["Begin_Time"] = aModel.Begin_Time.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            drowAgreement["End_Time"] = aModel.End_Time.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            drowAgreement["Agreement_Code"] = aModel.Agreement_Code.Trim();
            drowAgreement["Agreement_Code_Main"] = aModel.Agreement_Code_Main.Trim();
            drowAgreement["Reason"] = aModel.Reason;
            if (dstDelCount.Tables[0].Rows[0][0].ToString() != "0")
                drowAgreement["CanEdit"] = "Y";

            drowAgreementTmp.ItemArray = drowAgreement.ItemArray;
            dtblAgreement.Rows.Add(drowAgreement);
            dtblAgreementTmp.Rows.Add(drowAgreementTmp);
           

            #region 主合約群組
            dirValues.Clear();
            dirValues.Add("Agreement_Main_RID", aModel.RID);
            DataSet dstGroup = dao.GetList(SEL_GROUP_List, dirValues);
            int intGroupID = 0;
            foreach (DataRow drowGroup_s in dstGroup.Tables[0].Rows)
            {
                DataRow drowGroup = dtblGroup.NewRow();
                drowGroup["ID"] = intGroupID;
                drowGroup["Group_Name"] = drowGroup_s["Group_Name"];
                drowGroup["Type"] = drowGroup_s["Type"];
                drowGroup["Base_Price"] = drowGroup_s["Base_Price"];
                if (dstDelCount.Tables[0].Rows[0][0].ToString() != "0")
                    drowGroup["CanEdit"] = "Y";
                dtblGroup.Rows.Add(drowGroup);

                dirValues.Clear();
                dirValues.Add("Agreement_Group_RID", (int)drowGroup_s["RID"]);
                DataSet dstCard = dao.GetList(SEL_CARD_List + " order by Display_Name", dirValues);
                DataSet dstLevel = dao.GetList(SEL_LEVEL_List, dirValues);
                foreach (DataRow drowCard_s in dstCard.Tables[0].Rows)
                {
                    DataRow drowCard = dtblCard.NewRow();
                    drowCard["ID"] = intGroupID;
                    drowCard["CardType_RID"] = drowCard_s["CardType_RID"];
                    drowCard["CardType_NAME"] = drowCard_s["Name"];
                    drowCard["Param_RID"] = drowCard_s["Param_RID"];
                    if (dstDelCount.Tables[0].Rows[0][0].ToString() != "0")
                        drowCard["CanEdit"] = "Y";
                    dtblCard.Rows.Add(drowCard);
                }

                foreach (DataRow drowLevel_s in dstLevel.Tables[0].Rows)
                {
                    DataRow drowLevel = dtblLevel.NewRow();
                    drowLevel["ID"] = intGroupID;
                    drowLevel["CardType_RID"] = drowLevel_s["CardType_RID"];
                    drowLevel["Param_RID"] = drowLevel_s["Param_RID"];
                    drowLevel["Param_Name"] = drowLevel_s["Param_Name"];
                    drowLevel["Price"] = drowLevel_s["Price"];
                    drowLevel["Level_Min"] = drowLevel_s["Level_Min"];
                    drowLevel["Level_Max"] = drowLevel_s["Level_Max"];
                    if (dstDelCount.Tables[0].Rows[0][0].ToString() != "0")
                        drowLevel["CanEdit"] = "Y";
                    dtblLevel.Rows.Add(drowLevel);
                }

                DataSet dstMaterial = dao.GetList(SEL_MATERIAL_List, dirValues);
                foreach (DataRow drowMaterial_s in dstMaterial.Tables[0].Rows)
                {
                    DataRow drowMaterial = dtblMaterial.NewRow();
                    drowMaterial["ID"] = intGroupID;
                    drowMaterial["Material_RID"] = drowMaterial_s["Material_RID"];
                    drowMaterial["Material_Name"] = drowMaterial_s["Material_Name"];
                    drowMaterial["Base_Price"] = drowMaterial_s["Base_Price"];
                    if (dstDelCount.Tables[0].Rows[0][0].ToString() != "0")
                        drowMaterial["CanEdit"] = "Y";
                    dtblMaterial.Rows.Add(drowMaterial);
                }


                intGroupID++;
            }

            #endregion

            #endregion

            #region 備援合約
            dirValues.Clear();
            dirValues.Add("Agreement_Code_Main", aModel.Agreement_Code);
            DataSet dstAgreementBak = dao.GetList(SEL_AGREEMENTBAK_List, dirValues);
            int intAgreementBakID = 0;
            foreach (DataRow drowAgreementBak_s in dstAgreementBak.Tables[0].Rows)
            {
                dirValues.Clear();
                dirValues.Add("Agreement_RID", (int)drowAgreementBak_s["RID"]);

                DataSet dstDelCountBak = dao.GetList(CHK_AGREEMENT_BY_RID, dirValues, true);

                DataRow drowAgreementBak = dtblAgreementBak.NewRow();
                drowAgreementBak["RID"] = drowAgreementBak_s["RID"];
                drowAgreementBak["ID"] = intAgreementBakID;
                drowAgreementBak["Agreement_Name"] = drowAgreementBak_s["Agreement_Name"];
                drowAgreementBak["IMG_File_URL"] = drowAgreementBak_s["IMG_File_URL"];
                drowAgreementBak["IMG_File_Name"] = drowAgreementBak_s["IMG_File_Name"];
                drowAgreementBak["Factory_RID"] = drowAgreementBak_s["Factory_RID"];
                drowAgreementBak["Factory_Name"] = drowAgreementBak_s["Factory_Name"];
                drowAgreementBak["Card_Number"] = drowAgreementBak_s["Card_Number"];
                drowAgreementBak["Begin_Time"] = Convert.ToDateTime(drowAgreementBak_s["Begin_Time"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                drowAgreementBak["End_Time"] = Convert.ToDateTime(drowAgreementBak_s["End_Time"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                drowAgreementBak["Agreement_Code"] = drowAgreementBak_s["Agreement_Code"].ToString().Trim();
                drowAgreementBak["Agreement_Code_Main"] = drowAgreementBak_s["Agreement_Code_Main"].ToString().Trim();
                if (dstDelCountBak.Tables[0].Rows[0][0].ToString() != "0")
                    drowAgreementBak["CanEdit"] = "Y";

                dtblAgreementBak.Rows.Add(drowAgreementBak);

                dirValues.Clear();
                dirValues.Add("Agreement_Main_RID", (int)drowAgreementBak_s["RID"]);
                DataSet dstGroupBak = dao.GetList(SEL_GROUP_List, dirValues);
                int intGroupBakID = 0;
                foreach (DataRow drowGroupBak_s in dstGroupBak.Tables[0].Rows)
                {
                    DataRow drowGroupBak = dtblGroupBak.NewRow();
                    drowGroupBak["ID"] = intGroupBakID;
                    drowGroupBak["IDBak"] = intAgreementBakID;
                    drowGroupBak["Group_Name"] = drowGroupBak_s["Group_Name"];
                    drowGroupBak["Type"] = drowGroupBak_s["Type"];
                    drowGroupBak["Base_Price"] = drowGroupBak_s["Base_Price"];
                    if (dstDelCountBak.Tables[0].Rows[0][0].ToString() != "0")
                        drowGroupBak["CanEdit"] = "Y";
                    dtblGroupBak.Rows.Add(drowGroupBak);

                    dirValues.Clear();
                    dirValues.Add("Agreement_Group_RID", (int)drowGroupBak_s["RID"]);
                    DataSet dstCardBak = dao.GetList(SEL_CARD_List + " order by Display_Name", dirValues);
                    DataSet dstLevelBak = dao.GetList(SEL_LEVEL_List, dirValues);
                    foreach (DataRow drowCardBak_s in dstCardBak.Tables[0].Rows)
                    {
                        DataRow drowCardBak = dtblCardBak.NewRow();
                        drowCardBak["ID"] = intGroupBakID;
                        drowCardBak["IDBak"] = intAgreementBakID;
                        drowCardBak["CardType_RID"] = drowCardBak_s["CardType_RID"];
                        drowCardBak["CardType_NAME"] = drowCardBak_s["Name"];
                        drowCardBak["Param_RID"] = drowCardBak_s["Param_RID"];
                        if (dstDelCountBak.Tables[0].Rows[0][0].ToString() != "0")
                            drowCardBak["CanEdit"] = "Y";
                        dtblCardBak.Rows.Add(drowCardBak);
                    }

                    foreach (DataRow drowLevelBak_s in dstLevelBak.Tables[0].Rows)
                    {
                        DataRow drowLevelBak = dtblLevelBak.NewRow();
                        drowLevelBak["ID"] = intGroupBakID;
                        drowLevelBak["IDBak"] = intAgreementBakID;
                        drowLevelBak["CardType_RID"] = drowLevelBak_s["CardType_RID"];
                        drowLevelBak["Param_RID"] = drowLevelBak_s["Param_RID"];
                        drowLevelBak["Param_Name"] = drowLevelBak_s["Param_Name"];
                        drowLevelBak["Price"] = drowLevelBak_s["Price"];
                        drowLevelBak["Level_Min"] = drowLevelBak_s["Level_Min"];
                        drowLevelBak["Level_Max"] = drowLevelBak_s["Level_Max"];
                        if (dstDelCountBak.Tables[0].Rows[0][0].ToString() != "0")
                            drowLevelBak["CanEdit"] = "Y";
                        dtblLevelBak.Rows.Add(drowLevelBak);
                    }

                    DataSet dstMaterialBak = dao.GetList(SEL_MATERIAL_List, dirValues);
                    foreach (DataRow drowMaterialBak_s in dstMaterialBak.Tables[0].Rows)
                    {
                        DataRow drowMaterialBak = dtblMaterialBak.NewRow();
                        drowMaterialBak["ID"] = intGroupBakID;
                        drowMaterialBak["IDBak"] = intAgreementBakID;
                        drowMaterialBak["Material_RID"] = drowMaterialBak_s["Material_RID"];
                        drowMaterialBak["Material_Name"] = drowMaterialBak_s["Material_Name"];
                        drowMaterialBak["Base_Price"] = drowMaterialBak_s["Base_Price"];
                        if (dstDelCountBak.Tables[0].Rows[0][0].ToString() != "0")
                            drowMaterialBak["CanEdit"] = "Y";
                        dtblMaterialBak.Rows.Add(drowMaterialBak);
                    }
                    intGroupBakID++;
                }
                intAgreementBakID++;
            }
            #endregion
        }
        catch(Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dirAgreement;
    }


    /// <summary>
    /// 合約CHECK
    /// </summary>
    /// <param name="strRID"></param>
    public void AgreementCheck(DataRow drowAgreement)
    {
        AGREEMENT aModel = dao.GetModelByDataRow<AGREEMENT>(drowAgreement);

        DataTable dtblAgreementUsedTime = GetAgreementUserdTime(aModel.RID.ToString());
        if (dtblAgreementUsedTime != null)
        {
            if (!StringUtil.IsEmpty(dtblAgreementUsedTime.Rows[0]["mintime"].ToString()))
            {
                if (aModel.Begin_Time > Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["mintime"]))
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_002_02, Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["mintime"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo)));
                if (aModel.End_Time.AddDays(1) < Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["maxtime"]))
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_002_03, Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["maxtime"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo)));
            }
        }
    }

    public void Update(Dictionary<string, DataTable> dirAgreement)
    {
        DataTable dtblAgreement = dirAgreement["Agreement"];
        DataTable dtblAgreementTmp = dirAgreement["AgreementTmp"];
        DataTable dtblAgreementBak = dirAgreement["AgreementBak"];
        DataTable dtblGroup = dirAgreement["Group"];
        DataTable dtblCard = dirAgreement["Card"];
        DataTable dtblGroupBak = dirAgreement["GroupBak"];
        DataTable dtblCardBak = dirAgreement["CardBak"];
        DataTable dtblMaterial = dirAgreement["Material"];
        DataTable dtblMaterialBak = dirAgreement["MaterialBak"];

        DataTable dtblLevel = dirAgreement["Level"];
        DataTable dtblLevelBak = dirAgreement["LevelBak"];

        AGREEMENT aModel = new AGREEMENT();
        AGREEMENT aModel_o = new AGREEMENT();
        AGREEMENT aModelBak = new AGREEMENT();
        AGREEMENT aModelBak_o = new AGREEMENT();
        AGREEMENT_CARDTYPE_GROUP acgModel = new AGREEMENT_CARDTYPE_GROUP();
        AGREEMENT_CARDTYPE_GROUP acgModelBak = new AGREEMENT_CARDTYPE_GROUP();
        GROUP_CARDTYPE gcModel = new GROUP_CARDTYPE();
        GROUP_CARDTYPE gcModelBak = new GROUP_CARDTYPE();
        CARDTYPE_GROUP_MATERIAL cgmModel = new CARDTYPE_GROUP_MATERIAL();
        CARDTYPE_GROUP_MATERIAL cgmModelBak = new CARDTYPE_GROUP_MATERIAL();

        GROUP_LEVEL_PRICE glpModel = new GROUP_LEVEL_PRICE();
        GROUP_LEVEL_PRICE glpModelBak = new GROUP_LEVEL_PRICE();


        try
        {
            dao.OpenConnection();

            #region 更新主合約
            aModel = dao.GetModelByDataRow<AGREEMENT>(dirAgreement["Agreement"].Rows[0]);

           

            aModel_o = dao.GetModel<AGREEMENT, string>("Agreement_Code", aModel.Agreement_Code);

            //判斷合約是否被使用
            dirValues.Clear();
            dirValues.Add("AGREEMENT_RID", aModel_o.RID);
            DataSet dstOrder = dao.GetList(CHK_ORDERFORM, dirValues);
            bool IsOrderUsed = false;
            if (dstOrder.Tables[0].Rows.Count > 0)
            {
                if (dstOrder.Tables[0].Rows[0][0].ToString() != "0")
                    IsOrderUsed = true;
            }
            //如果預算使用過，預算值只能改大
            if (IsOrderUsed)
            {
                if (aModel_o.Card_Number > aModel.Card_Number)
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_002_01, aModel_o.Agreement_Code));
            }



            DataTable dtblAgreementUsedTime = GetAgreementUserdTime(aModel_o.RID.ToString());
            if (dtblAgreementUsedTime != null)
            {
                if (!StringUtil.IsEmpty(dtblAgreementUsedTime.Rows[0]["mintime"].ToString()))
                {
                    if (aModel.Begin_Time > Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["mintime"]))
                        throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_002_02, Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["mintime"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo)));
                    if (aModel.End_Time.AddDays(1) < Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["maxtime"]))
                        throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_002_03, Convert.ToDateTime(dtblAgreementUsedTime.Rows[0]["maxtime"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo)));
                }
            }


            aModel.Remain_Card_Num = aModel.Card_Number - aModel_o.Card_Number + aModel_o.Remain_Card_Num;
            aModel.RCT = aModel_o.RCT;
            aModel.RCU = aModel_o.RCU;
            aModel.RID = aModel_o.RID;

            dao.Update<AGREEMENT>(aModel, "RID");
            #endregion

            #region 刪除群組，卡種，材質
            dirValues.Clear();
            dirValues.Add("Agreement_Code", aModel.Agreement_Code);
            //刪除所有關聯合約級距
            int aa = dao.ExecuteNonQuery(DEL_LEVEL_ByAg_Code, dirValues);
            //刪除所有關聯合約卡種
            dao.ExecuteNonQuery(DEL_CARD_ByAg_Code, dirValues);
            //刪除所有關聯合約群組
            dao.ExecuteNonQuery(DEL_GROUP_ByAg_Code, dirValues);
            //刪除所有關聯合約材質
            dao.ExecuteNonQuery(DEL_MATERIAL_ByAg_Code, dirValues);

            #endregion

            #region 主合約群組
            //刪除合約群組

            foreach (DataRow drowGroup in dirAgreement["Group"].Rows)
            {
                acgModel = dao.GetModelByDataRow<AGREEMENT_CARDTYPE_GROUP>(drowGroup);
                acgModel.Agreement_Main_RID = aModel.RID;

                int intRID = (int)dao.AddAndGetID<AGREEMENT_CARDTYPE_GROUP>(acgModel, "RID");

                DataRow[] drowCards = dirAgreement["Card"].Select("ID='" + drowGroup["ID"].ToString()+"'");
                DataRow[] drowMaterials = dirAgreement["Material"].Select("ID='" + drowGroup["ID"].ToString()+"'");

                //增加主合約卡種
                foreach (DataRow drowCard in drowCards)
                {
                    gcModel = dao.GetModelByDataRow<GROUP_CARDTYPE>(drowCard);
                    gcModel.Agreement_Group_RID = intRID;


                    //增加級距
                    DataRow[] drowLevels = dirAgreement["Level"].Select("ID='" + drowGroup["ID"].ToString() + "' and CardType_RID='" + gcModel.CardType_RID+"'");
                    if (drowLevels.Length > 0)
                        gcModel.Param_RID = Convert.ToInt32(drowLevels[0]["Param_RID"]);
                    else
                        gcModel.Param_RID = 0;

                    int intCardRID = Convert.ToInt32(dao.AddAndGetID<GROUP_CARDTYPE>(gcModel, "RID"));

                    foreach (DataRow drowLevel in drowLevels)
                    {
                        glpModel = dao.GetModelByDataRow<GROUP_LEVEL_PRICE>(drowLevel);
                        glpModel.Group_CardType_RID = intCardRID;
                        dao.Add<GROUP_LEVEL_PRICE>(glpModel, "RID");
                    }
                }

                //增加主合約材質
                foreach (DataRow drowMaterial in drowMaterials)
                {
                    cgmModel = dao.GetModelByDataRow<CARDTYPE_GROUP_MATERIAL>(drowMaterial);
                    cgmModel.Agreement_Group_RID = intRID;
                    dao.Add<CARDTYPE_GROUP_MATERIAL>(cgmModel, "RID");
                }
            }

            #endregion

            #region 備援合約
            dirValues.Clear();
            dirValues.Add("Agreement_Code_Main", aModel.Agreement_Code);
            DataSet dstAgreementBak = dao.GetList(SEL_AGREEMENTBAK_List, dirValues);
            ArrayList al = new ArrayList();

            foreach (DataRow drowAgreementBak in dstAgreementBak.Tables[0].Rows)
            {
                al.Add(drowAgreementBak["Agreement_Code"].ToString().Trim());
            }

            foreach (DataRow drowAgreementBak in dirAgreement["AgreementBak"].Rows)
            {
                int intBakRID = 0;
                if (al.Contains(drowAgreementBak["Agreement_Code"].ToString()))  //修改
                {
                    al.Remove(drowAgreementBak["Agreement_Code"].ToString());
                    aModelBak_o = dao.GetModel<AGREEMENT, string>("Agreement_Code", drowAgreementBak["Agreement_Code"].ToString());
                    aModelBak = dao.GetModelByDataRow<AGREEMENT>(drowAgreementBak);
                    aModelBak.RCT = aModelBak_o.RCT;
                    aModelBak.RCU = aModelBak_o.RCU;
                    aModelBak.RID = aModelBak_o.RID;
                    aModelBak.Agreement_Code_Main = aModelBak_o.Agreement_Code_Main;

                    //判斷合約是否被使用
                    dirValues.Clear();
                    dirValues.Add("AGREEMENT_RID", aModelBak_o.RID);
                    DataSet dstOrder1 = dao.GetList(CHK_ORDERFORM, dirValues);
                    bool IsOrderUsed1 = false;
                    if (dstOrder1.Tables[0].Rows.Count > 0)
                    {
                        if (dstOrder1.Tables[0].Rows[0][0].ToString() != "0")
                            IsOrderUsed1 = true;
                    }
                    //如果預算使用過，預算值只能改大
                    if (IsOrderUsed1)
                    {
                        if (aModelBak_o.Card_Number > aModelBak.Card_Number)
                            throw new AlertException(String.Format(BizMessage.BizMsg.ALT_BASEINFO_002_01, aModelBak_o.Agreement_Code));
                    }

                    aModelBak.Remain_Card_Num = aModelBak.Card_Number - aModelBak_o.Card_Number + aModelBak_o.Remain_Card_Num;


                    dao.Update<AGREEMENT>(aModelBak, "RID");

                    intBakRID = aModelBak_o.RID;
                }
                else    //新增
                {
                    aModelBak = dao.GetModelByDataRow<AGREEMENT>(drowAgreementBak);
                    aModelBak.Agreement_Code_Main = aModel.Agreement_Code;
                    aModelBak.Remain_Card_Num = aModelBak.Card_Number;
                    intBakRID = (int)dao.AddAndGetID<AGREEMENT>(aModelBak, "RID");
                }

                DataRow[] drowGroupBaks = dirAgreement["GroupBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString()+"'");

                //備援合約群組
                foreach (DataRow drowGroupBak in drowGroupBaks)
                {
                    acgModelBak = dao.GetModelByDataRow<AGREEMENT_CARDTYPE_GROUP>(drowGroupBak);
                    acgModelBak.Agreement_Main_RID = intBakRID;

                    int intRID = (int)dao.AddAndGetID<AGREEMENT_CARDTYPE_GROUP>(acgModelBak, "RID");

                    DataRow[] drowCardBaks = dirAgreement["CardBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString() + "' and ID='" + drowGroupBak["ID"].ToString()+"'");
                    DataRow[] drowMaterialBaks = dirAgreement["MaterialBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString() + "' and ID='" + drowGroupBak["ID"].ToString()+"'");

                    //增加備援合約卡種
                    foreach (DataRow drowCardBak in drowCardBaks)
                    {
                        if (StringUtil.IsEmpty(drowCardBak[5].ToString()))
                            drowCardBak[5] = 0;

                        gcModelBak = dao.GetModelByDataRow<GROUP_CARDTYPE>(drowCardBak);
                        gcModelBak.Agreement_Group_RID = intRID;


                        //增加備援級距
                        DataRow[] drowLevelBaks = dirAgreement["LevelBak"].Select("IDBak='" + drowAgreementBak["ID"].ToString() + "' and ID='" + drowGroupBak["ID"].ToString() + "' and CardType_RID='" + gcModelBak.CardType_RID + "'");
                        if (drowLevelBaks.Length > 0)
                            gcModelBak.Param_RID = Convert.ToInt32(drowLevelBaks[0]["Param_RID"]);
                        else
                            gcModelBak.Param_RID = 0;

                        int intCardBakRID = Convert.ToInt32(dao.AddAndGetID<GROUP_CARDTYPE>(gcModelBak, "RID"));

                        foreach (DataRow drowLevelBak in drowLevelBaks)
                        {
                            glpModelBak = dao.GetModelByDataRow<GROUP_LEVEL_PRICE>(drowLevelBak);
                            glpModelBak.Group_CardType_RID = intCardBakRID;
                            dao.Add<GROUP_LEVEL_PRICE>(glpModelBak, "RID");
                        }
                    }

                    //增加備援合約材質
                    foreach (DataRow drowMaterialBak in drowMaterialBaks)
                    {
                        cgmModelBak = dao.GetModelByDataRow<CARDTYPE_GROUP_MATERIAL>(drowMaterialBak);
                        cgmModelBak.Agreement_Group_RID = intRID;
                        dao.Add<CARDTYPE_GROUP_MATERIAL>(cgmModelBak, "RID");
                    }

                }
            }

            //刪除的備援合約
            for (int i = 0; i < al.Count; i++)
            {
                dirValues.Clear();
                dirValues.Add("Agreement_Code", al[i].ToString());
                dao.Delete("AGREEMENT", dirValues);

                //aModelBak_o = dao.GetModel<AGREEMENT, string>("Agreement_Code", al[i].ToString());
                //aModelBak_o.RST = "D";
                //dao.Update<AGREEMENT>(aModelBak_o, "RID");
            }
            #endregion

            //操作日誌
            SetOprLog();


            Warning.SetWarning(GlobalString.WarningType.EditAgreement, new object[2] { aModel.Agreement_Code, "修改" });
            Warning.SetWarning(GlobalString.WarningType.AgreementCardLower, new object[3] { aModel.Agreement_Code, aModel.Card_Number, aModel.Remain_Card_Num });

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
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
    /// 刪除
    /// </summary>
    /// <param name="strAgreement_Code"></param>
    public void Delete(string strAgreement_Code)
    {
        AGREEMENT aModel = new AGREEMENT();
        AGREEMENT aModelBak = new AGREEMENT();

        try
        {
            dao.OpenConnection();

            #region 刪除群組，卡種
            dirValues.Clear();
            dirValues.Add("Agreement_Code", strAgreement_Code);
            //刪除所有關聯合約級距
            dao.ExecuteNonQuery(DEL_LEVEL_ByAg_Code, dirValues);
            //刪除所有關聯合約卡種
            dao.ExecuteNonQuery(DEL_CARD_ByAg_Code, dirValues);
            //刪除所有關聯合約群組
            dao.ExecuteNonQuery(DEL_GROUP_ByAg_Code, dirValues);
            //刪除所有關聯合約材質
            dao.ExecuteNonQuery(DEL_MATERIAL_ByAg_Code, dirValues);
            
            #endregion 

            ChkDelAgreement(strAgreement_Code);

            #region 刪除主合約
            dirValues.Clear();
            dirValues.Add("Agreement_Code", strAgreement_Code);
            dao.Delete("AGREEMENT", dirValues);

            //aModel = dao.GetModel<AGREEMENT, string>("Agreement_Code", strAgreement_Code);
            
            //aModel.RST = "D";
            //dao.Update<AGREEMENT>(aModel, "RID");
            #endregion

            #region 刪除備援合約
            dirValues.Clear();
            dirValues.Add("Agreement_Code_Main", strAgreement_Code);
            DataSet dstAgreementBak = dao.GetList(SEL_AGREEMENTBAK_List, dirValues);

            foreach (DataRow drowAgreementBak in dstAgreementBak.Tables[0].Rows)
            {
                dirValues.Clear();
                dirValues.Add("Agreement_Code", drowAgreementBak["Agreement_Code"].ToString());
                dao.Delete("AGREEMENT", dirValues);

                //aModelBak = dao.GetModel<AGREEMENT, string>("Agreement_Code", drowAgreementBak["Agreement_Code"].ToString());
                //aModelBak.RST = "D";
                //dao.Update<AGREEMENT>(aModelBak, "RID");
            }
            #endregion

            //操作日誌
            SetOprLog("4");

            Warning.SetWarning(GlobalString.WarningType.EditAgreement, new object[2] { strAgreement_Code, "刪除" });

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
    /// 檢查合約是否可以刪除
    /// </summary>
    /// <param name="strAgreement_Code"></param>
    public void ChkDelAgreement(string strAgreement_Code)
    {
        AGREEMENT aModel = dao.GetModel<AGREEMENT, string>("Agreement_Code", strAgreement_Code);
        if (aModel == null)
            return;

        dirValues.Clear();
        dirValues.Add("Agreement_RID", aModel.RID);

        DataSet dstBudget = dao.GetList(CHK_AGREEMENT_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "合約"));

    }
}
