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
using ControlLibrary;
using System.Text;
using System.Collections;
using Microsoft.Practices.EnterpriseLibrary.Data;

/// CardType002BL 的摘要说明
/// </summary>
public class CardType002BL : BaseLogic
{
    public const string SEL_CARD_TYPE = "SELECT CT.RID,CONVERT(VARCHAR,CT.TYPE) + '-' + CONVERT(VARCHAR,CT.AFFINITY) + '-' + CONVERT(VARCHAR,CT.PHOTO) AS CardID,CT.Name,CT.BIN,CE.Name as CardExponentName,CT.Begin_Time,CT.display_name FROM CARD_TYPE AS CT LEFT JOIN CARD_EXPONENT CE ON CT.Exponent_RID = CE.RID ";

    public const string SEL_CARD_GROUP_2 = "SELECT CG.RID, CG.group_name FROM CARD_GROUP AS CG INNER JOIN GROUP_CARD_TYPE AS GCT ON GCT.RST = 'A' AND GCT.group_rid = CG.RID AND GCT.CardType_RID = @CardType_RID WHERE CG.RST = 'A' AND CG.param_code ='" + GlobalString.Parameter.Type + "'";

    public const string SEL_CARD_GROUP_BY_PARAM_CODE = "SELECT 0 as RID, null as Group_Name FROM CARD_GROUP as cd  union  "
                                        + "SELECT CG.RID,CG.Group_Name "
                                        + "FROM CARD_GROUP AS CG "
                                        + "WHERE CG.RST = 'A' AND CG.Param_Code = @param_code";
    public const string SEL_CARD_TYPE_NAME = "SELECT RID,Name "
                                        + "FROM CARD_TYPE AS CT "
                                        + "WHERE CT.RST = 'A' and CT.IS_using='Y' ";
    public const string SEL_PARAM_NAME = "SELECT Param_Code,Param_Name "
                                        + "FROM PARAM "
                                        + "WHERE RST = 'A' AND ParamType_Code = @paramtype_code "
                                        + "AND (Param_Code IN "
                                        + "(SELECT CG.Param_Code "
                                        + "FROM CARD_GROUP AS CG "
                                        + "WHERE (CG.RST = 'A')))";
    public const string CON_CARD_TYPE = "SELECT COUNT(*) "
                                        + "FROM CARD_TYPE AS CT "
                                        + "WHERE CT.RST = 'A' AND CT.type = @type AND CT.PHOTO = @photo AND CT.AFFINITY = @affinity";
    public const string CON_CARD_TYPE_1 = "SELECT COUNT(*) "
                                        + "FROM CARD_TYPE AS CT "
                                        + "WHERE CT.RST = 'A' AND CT.TOTAL_TYPE = @total_type AND CT.PHOTO = @photo AND CT.Card_Whole_Name = @card_whole_name AND RID<>@rid";
    public const string CON_CARD_TYPE_2 = "SELECT COUNT(*) "
                                        + "FROM CARD_TYPE AS CT "
                                        + "WHERE CT.RST = 'A' AND CT.Name = @name";
    public const string CON_CARD_TYPE_3 = "SELECT COUNT(*) "
                                        + "FROM CARD_TYPE AS CT "
                                        + "WHERE CT.RST = 'A' AND CT.Name = @name "
                                        + "AND CT.RID <> @rid";
    public const string SEL_CARDTYPE_MATERIAL = "SELECT MS.Material_Name "
                                        + "FROM CARDTYPE_MATERIAL CTM INNER JOIN MATERIAL_SPECIAL MS ON MS.RST = 'A' AND CTM.Material_RID = MS.RID "
                                        + "WHERE CTM.RST = 'A' AND CTM.CardType_RID = @rid";
    public const string SEL_CARDTYPE_EXPONET_ENVELOPE = "SELECT CE.Name "
                                        + "FROM CARD_TYPE CT INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID "
                                        + "WHERE CT.RST = 'A' AND CT.RID = @rid "
                                        + "SELECT EI.Name "
                                        + "FROM CARD_TYPE CT INNER JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID "
                                        + "WHERE CT.RST = 'A' AND CT.RID = @rid ";
    public const string SEL_CARDTYPE_IMG = "SELECT CTI.IMG_File_URL,CTI.File_Name "
                                        + "FROM CARDTYPE_IMG CTI	"
                                        + "WHERE CTI.RST = 'A' AND CTI.CardType_RID = @rid";
    public const string SEL_GROUPRID_BY_CARD_TYPE_RID = "SELECT Group_RID "
                                        + "FROM GROUP_CARD_TYPE "
                                        + "WHERE RST = 'A' AND CardType_RID = @cardtype_rid";
    public const string SEL_FACTORY_BY_CARD_TYPE_RID = "SELECT DISTINCT F.RID,F.Factory_ShortName_CN "
                                        + "FROM PERSO_PROJECT PP INNER JOIN FACTORY F ON F.RST='A' AND PP.Factory_RID = F.RID AND F.Is_Perso = @is_perso "
                                        + "WHERE PP.RST = 'A' AND PP.Normal_Special = @normal_special AND PP.RID IN "
                                        + "(SELECT Perso_Project_RID FROM CARDTYPE_PERSO_PROJECT WHERE RST = 'A' AND CardType_RID = @cardtype_rid)";
    public const String SEL_PERSO_PROJECT_BY_FACTORY_CARDTYPE = "SELECT Project_Name,Unit_Price "
                                        + "FROM PERSO_PROJECT PP "
                                        + "WHERE RST='A' AND Normal_Special = @normal_special AND Factory_RID = @factory_rid AND RID IN "
                                        + "(SELECT Perso_Project_RID FROM CARDTYPE_PERSO_PROJECT WHERE RST = 'A' AND CardType_RID = @cardtype_rid)";
    public const string DEL_CARD_TYPE_IMG = "DELETE FROM CARDTYPE_IMG "
                                        + "WHERE CardType_RID = @cardType_rid ";
    public const string DEL_GROUP_CARD_TYPE = "DELETE FROM GROUP_CARD_TYPE "
                                        + "WHERE CardType_RID = @cardType_rid ";

    public const string CHK_CARD_BY_RID = "proc_CHK_DEL_CardType";

    public const string SEL_PROJECT_STEP = "proc_CardType002_01";

    public const string SEL_REPORT = "proc_CardType002_02";


    Dictionary<string, object> dirValues = new Dictionary<string, object>();


    public CardType002BL()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    
   



    public DataSet GetProjectStep(string strCardRID)
    {
        DataSet dstProjectStep = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("DateTimeNow", DateTime.Now.ToString("yyyy-MM-dd"));
            dirValues.Add("CardTypeRID", strCardRID);
            dstProjectStep = dao.GetList(SEL_PROJECT_STEP, dirValues,true);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstProjectStep;
    }

    /// <summary>
    /// 取製卡卡種群組
    /// </summary>
    /// <returns></returns>
    public DataSet GetCardTypeGroup2(string strRID)
    {
        DataSet dstCardGroup = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("CardType_RID", strRID);
            dstCardGroup = dao.GetList(SEL_CARD_GROUP_2, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstCardGroup;
    }

    /// <summary>
    /// 取製卡卡種群組
    /// </summary>
    /// <returns></returns>
    public DataSet GetCardTypeGroup()
    {
        DataSet dstCardGroup = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("param_code", GlobalString.Parameter.Type);
            dstCardGroup = dao.GetList(SEL_CARD_GROUP_BY_PARAM_CODE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstCardGroup;
    }

    /// <summary>
    /// 取用途參數
    /// </summary>
    /// <returns></returns>
    public DataSet GetParamUse()
    {
        DataSet dstParam = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("paramtype_code", GlobalString.ParameterType.Use);
            dstParam = dao.GetList(SEL_PARAM_NAME, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstParam;
    }

    /// <summary>
    /// 以Param_Code取卡群組
    /// </summary>
    /// <param name="Param_Code"></param>
    /// <returns>DataSet<卡群組></returns>
    public DataSet GetCardGoupByParam_Code(string Param_Code)
    {
        DataSet dstCardGroup = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("param_code", Param_Code);
            dstCardGroup = dao.GetList(SEL_CARD_GROUP_BY_PARAM_CODE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstCardGroup;
    }

    /// <summary>
    /// 取版面簡稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetCardName()
    {
        DataSet dstName = null;
        try
        {
            this.dirValues.Clear();
            dstName = dao.GetList(SEL_CARD_TYPE_NAME+" order by display_name");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstName;
    }

    public DataSet Report(Dictionary<string, object> searchInput)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("GroupRID", searchInput["dropCardType_Group_RID"].ToString());
                dirValues.Add("Type", searchInput["txtTYPE"].ToString());
                dirValues.Add("name", searchInput["txtName"].ToString());
                dirValues.Add("Is_Using", searchInput["dropUseType"].ToString());
                dirValues.Add("DateTimeNow", DateTime.Now.ToString("yyyy-MM-dd"));
            }
            else
            {
                dirValues.Add("GroupRID", "");
                dirValues.Add("Type", "");
                dirValues.Add("name", "");
                dirValues.Add("Is_Using", "");
                dirValues.Add("DateTimeNow","");
            }

            dst = dao.GetList(SEL_REPORT, dirValues, true);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dst;
    }

    /// <summary>
    /// 查詢卡种主記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[卡种]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "display_name" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CARD_TYPE);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            // 群組類型
            dirValues.Add("param_code",GlobalString.Parameter.Type);

            // 群組
            if (searchInput["dropCardType_Group_RID"].ToString() != "")
            {
                stbWhere.Append(" INNER JOIN GROUP_CARD_TYPE AS GCT ON GCT.RST = 'A' AND GCT.cardtype_rid = CT.RID AND GCT.group_rid = @group_rid ");
                dirValues.Add("group_rid", searchInput["dropCardType_Group_RID"].ToString());
            }

            stbWhere.Append(" WHERE CT.RST = 'A' ");

            // 類型
            if (searchInput["txtTYPE"].ToString() != "")
            {
                stbWhere.Append(" AND CT.TYPE=@type ");
                dirValues.Add("type", searchInput["txtTYPE"].ToString());
            }

            // 簡稱
            if (searchInput["txtName"].ToString() != "")
            {
                stbWhere.Append(" AND CT.Name LIKE @name ");
                dirValues.Add("name", "%" + searchInput["txtName"].ToString() + "%");
            }

            // 使用或停止
            if (searchInput["dropUseType"].ToString() != "" )
            {
                stbWhere.Append(" AND CT.Is_Using = @is_using ");
                dirValues.Add("is_using", searchInput["dropUseType"].ToString());
            }
        }

        //執行SQL語句
        DataSet dstCard_Type = null;
        try
        {
            dstCard_Type = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstCard_Type;
    }

    /// <summary>
    /// 查詢此卡種對應的Perso廠(一般)
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DataSet GetPersoFactoryByCardTypeRID(string strRID)
    {
        DataSet dstPersoFactory = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("is_perso", GlobalString.YNType.Yes);
            dirValues.Add("normal_special", GlobalString.BaseSpecial.Base);
            dirValues.Add("cardtype_rid", int.Parse(strRID));
            dstPersoFactory = dao.GetList(SEL_FACTORY_BY_CARD_TYPE_RID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstPersoFactory;
    }

    /// <summary>
    /// 查詢卡種、Perso廠商的Project(一般)
    /// </summary>
    /// <param name="strCardTypeRID"></param>
    /// <param name="strFactory_RID"></param>
    /// <returns></returns>
    public DataSet GetPerso_ProjectByCardTypeFactoryRID(string strCardTypeRID, string strFactory_RID)
    {
        DataSet dstPersoFactory = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("is_perso", GlobalString.YNType.Yes);
            dirValues.Add("normal_special", GlobalString.BaseSpecial.Base);
            dirValues.Add("cardtype_rid", int.Parse(strCardTypeRID));
            dirValues.Add("factory_rid", int.Parse(strFactory_RID));
            dstPersoFactory = dao.GetList(SEL_PERSO_PROJECT_BY_FACTORY_CARDTYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstPersoFactory;
    }


    /// <summary>
    /// 查詢出此卡種的寄卡單和信封
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DataSet GetExponentAndEnvelopeByCardTypeRID(string strRID)
    {
        DataSet dstExponentAndEnvelope = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("rid", int.Parse(strRID));
            dstExponentAndEnvelope = dao.GetList(SEL_CARDTYPE_EXPONET_ENVELOPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstExponentAndEnvelope;
    }

    /// <summary>
    /// 查詢出所有此卡種用到的材質
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DataSet GetMaterialByCardTypeRID(string strRID)
    {
        DataSet dstMaterial = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("rid", int.Parse(strRID));
            dstMaterial = dao.GetList(SEL_CARDTYPE_MATERIAL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstMaterial;
    }

    /// <summary>
    /// 根据卡種ID获得卡片圖檔
    /// </summary>
    /// <param name="strRID">卡種RID</param>
    /// <returns><DataSet>卡片圖檔</returns>
    public DataSet GetImgByCardTypeRID(string strRID)
    {
        DataSet dstImg = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("rid", int.Parse(strRID));
            dstImg = dao.GetList(SEL_CARDTYPE_IMG, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstImg;
    }

    /// <summary>
    /// 根据卡種ID获得卡種群組RID
    /// </summary>
    /// <param name="strRID">卡種RID</param>
    /// <returns><DataSet>卡群組RID</returns>
    public DataSet GetGroupRIDByCardTypeRID(string strRID)
    {
        DataSet dstGroupRID = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("cardtype_rid", int.Parse(strRID));
            dstGroupRID = dao.GetList(SEL_GROUPRID_BY_CARD_TYPE_RID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstGroupRID;
    }

    /// <summary>
    //  依據ID获得CARD_TYPE模型
    /// </summary>
    /// <param name="strRID">CardType表的RID</param>
    /// <returns></returns>
    public CARD_TYPE GetCardType(string strRID)
    {
        CARD_TYPE cardType = null;
        try
        {
            cardType = dao.GetModel<CARD_TYPE, int>("RID", Convert.ToInt32(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return cardType;
    }


    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="ctModel"><Model>卡種Model</param>
    /// <param name="CardTypeGroup"><List>卡種群組</param>
    /// <param name="Img"><List>卡種對應的圖片</param>
    public void Add(CARD_TYPE ctModel, List<string> CardTypeGroup, DataTable dtblImg)
    {
        CARDTYPE_IMG ctiModel = new CARDTYPE_IMG();
        GROUP_CARD_TYPE gctModel = new GROUP_CARD_TYPE();
        try
        {
            // 事務開始
            dao.OpenConnection();

            ctModel.Display_Name = ctModel.TYPE + "-" + ctModel.AFFINITY + "-" + ctModel.PHOTO + "-" + ctModel.Name;
            // 返回要增加关系的种ID
            int intRID = Convert.ToInt32(dao.AddAndGetID<CARD_TYPE>(ctModel, "RID"));

            // 卡種群組
            foreach (string strGroupRID in CardTypeGroup)
            {
                gctModel.CardType_RID = intRID;
                gctModel.Group_RID = Convert.ToInt32(strGroupRID);
                dao.Add<GROUP_CARD_TYPE>(gctModel, "RID");
            }

            //foreach新增卡片圖檔
            if (dtblImg.Rows.Count > 0)
            {
                foreach (DataRow drImg in dtblImg.Rows)
                {
                    ctiModel.CardType_RID = intRID;
                    ctiModel.IMG_File_URL = Convert.ToString(drImg["IMG_File_URL"]);
                    ctiModel.File_Name = Convert.ToString(drImg["File_Name"]);
                    dao.Add<CARDTYPE_IMG>(ctiModel, "RID");
                }
            }

            //操作日誌
            SetOprLog();

            Warning.SetWarning(GlobalString.WarningType.CardTypeAdd, new object[2] { DateTime.Now.ToString("MM/dd"), ctModel.Name });


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
    /// 保存卡種及卡種和其他訊息之間的關系
    /// </summary>
    /// <param name="ctModel"></param>
    /// <param name="CardTypeGroup"></param>
    /// <param name="dtblImg"></param>
    public void Update(CARD_TYPE ctModel, List<string> CardTypeGroup, DataTable dtblImg)
    {
        CARDTYPE_IMG ctiModel = new CARDTYPE_IMG();
        GROUP_CARD_TYPE gctModel = new GROUP_CARD_TYPE();

        try
        {
            //事務開始
            dao.OpenConnection();

            // 刪除卡種圖檔信息
            this.dirValues.Clear();
            this.dirValues.Add("cardType_rid", ctModel.RID);
            dao.ExecuteNonQuery(DEL_CARD_TYPE_IMG, dirValues);

            // 刪除卡種群組訊息
            dao.ExecuteNonQuery(DEL_GROUP_CARD_TYPE, dirValues);

            // 卡種群組
            foreach (string strGroupRID in CardTypeGroup)
            {
                gctModel.CardType_RID = ctModel.RID;
                gctModel.Group_RID = Convert.ToInt32(strGroupRID);
                dao.Add<GROUP_CARD_TYPE>(gctModel, "RID");
            }

            //foreach新增卡片圖檔
            if (dtblImg.Rows.Count > 0)
            {
                foreach (DataRow drImg in dtblImg.Rows)
                {
                    ctiModel.CardType_RID = ctModel.RID;
                    ctiModel.IMG_File_URL = Convert.ToString(drImg["IMG_File_URL"]);
                    ctiModel.File_Name = Convert.ToString(drImg["File_Name"]);
                    dao.Add<CARDTYPE_IMG>(ctiModel, "RID");
                }
            }
            ctModel.Display_Name = ctModel.TYPE + "-" + ctModel.AFFINITY + "-" + ctModel.PHOTO + "-" + ctModel.Name;

            CARD_TYPE ctModel_O = dao.GetModel<CARD_TYPE, int>("RID", ctModel.RID);

            //保存卡種訊息
            dao.Update<CARD_TYPE>(ctModel, "RID");


            dirValues.Clear();
            dirValues.Add("TYPE", ctModel.TYPE);
            dirValues.Add("AFFINITY", ctModel.AFFINITY);
            dirValues.Add("PHOTO", ctModel.PHOTO);
            dirValues.Add("TYPE1", ctModel_O.TYPE);
            dirValues.Add("AFFINITY1", ctModel_O.AFFINITY);
            dirValues.Add("PHOTO1", ctModel_O.PHOTO);

            dao.ExecuteNonQuery("update DAYLY_MONITOR set TYPE=@TYPE,AFFINITY=@AFFINITY,PHOTO=@PHOTO where TYPE=@TYPE1 and AFFINITY=@AFFINITY1 and PHOTO=@PHOTO1",dirValues);
            dao.ExecuteNonQuery("update FACTORY_CHANGE_IMPORT set TYPE=@TYPE,AFFINITY=@AFFINITY,PHOTO=@PHOTO where TYPE=@TYPE1 and AFFINITY=@AFFINITY1 and PHOTO=@PHOTO1", dirValues);
            dao.ExecuteNonQuery("update FORE_CHANGE_CARD set TYPE=@TYPE,AFFINITY=@AFFINITY,PHOTO=@PHOTO where TYPE=@TYPE1 and AFFINITY=@AFFINITY1 and PHOTO=@PHOTO1", dirValues);
            dao.ExecuteNonQuery("update FORE_CHANGE_CARD_DETAIL set TYPE=@TYPE,AFFINITY=@AFFINITY,PHOTO=@PHOTO where TYPE=@TYPE1 and AFFINITY=@AFFINITY1 and PHOTO=@PHOTO1", dirValues);
            dao.ExecuteNonQuery("update MONTHLY_MONITOR set TYPE=@TYPE,AFFINITY=@AFFINITY,PHOTO=@PHOTO where TYPE=@TYPE1 and AFFINITY=@AFFINITY1 and PHOTO=@PHOTO1", dirValues);
            dao.ExecuteNonQuery("update PERSO_FORE_CHANGE_CARD set TYPE=@TYPE,AFFINITY=@AFFINITY,PHOTO=@PHOTO where TYPE=@TYPE1 and AFFINITY=@AFFINITY1 and PHOTO=@PHOTO1", dirValues);
            dao.ExecuteNonQuery("update SUBTOTAL_IMPORT set TYPE=@TYPE,AFFINITY=@AFFINITY,PHOTO=@PHOTO where TYPE=@TYPE1 and AFFINITY=@AFFINITY1 and PHOTO=@PHOTO1", dirValues);

            //操作日誌
            SetOprLog();


            Warning.SetWarning(GlobalString.WarningType.CardTypeEdit, new object[1] { ctModel.Name });

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
    /// 刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void Delete(string strRID)
    {
        CARD_TYPE ctModel = new CARD_TYPE();
        try
        {
            //事務開始
            dao.OpenConnection();

            ChkDelCard(strRID);


            // 刪除卡種圖檔信息
            this.dirValues.Clear();
            this.dirValues.Add("cardType_rid", int.Parse(strRID));
            dao.Delete("DM_CARDTYPE", dirValues);
            dao.ExecuteNonQuery(DEL_CARD_TYPE_IMG, dirValues);

            // 刪除卡種群組訊息
            dao.ExecuteNonQuery(DEL_GROUP_CARD_TYPE, dirValues);

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("CARD_TYPE", dirValues);



            //// 刪除卡種訊息
            //ctModel = dao.GetModel<CARD_TYPE, int>("RID", Convert.ToInt32(strRID));
            //ctModel.RST = GlobalString.RST.DELETE;
            //dao.Update<CARD_TYPE>(ctModel, "RID");

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
    /// 小記Type+Photo+卡片全名需為唯一值，在新增卡種基本資料時，
    /// 系統要校驗資料庫中是否有此資料，沒有才可新增(新增時)
    /// </summary>
    /// <param name="total_type"></param>
    /// <param name="photo"></param>
    /// <returns></returns>
    public bool ContainsTypePhotoWholeName(int Total_Type, int Photo, int Affinity,string strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("type", Total_Type);
            dirValues.Add("photo", Photo);
            dirValues.Add("affinity", Affinity);
            if (!StringUtil.IsEmpty(strRID))
            {
                dirValues.Add("RID", int.Parse(strRID));
                return dao.Contains(CON_CARD_TYPE + " and RID <>@RID", dirValues);

            }
            else
                return dao.Contains(CON_CARD_TYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 版面简称的唯一性(新增時)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool ContainsCardTypeName(string name)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("name", name);
            return dao.Contains(CON_CARD_TYPE_2, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 版面简称的唯一性(修改時)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool ContainsCardTypeName(string name, string RID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("name", name);
            dirValues.Add("rid", Convert.ToInt32(RID));
            return dao.Contains(CON_CARD_TYPE_3, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 檢查卡種是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelCard(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("CardType_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_CARD_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "卡種"));

    }
}