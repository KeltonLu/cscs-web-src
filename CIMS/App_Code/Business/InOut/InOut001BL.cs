//******************************************************************
//*  作    者：JunWang
//*  功能說明：小計檔匯入匯出邏輯 
//*  創建日期：2008-11-06
//*  修改日期：2008-11-06 9:00
//*  修改記錄：
//*            □2008-11-06
//*              1.創建 王俊
//*            □2009-09-02
//*                修改 楊昆
//*                      1.插入小計檔資訊(替換前)
//*                      2.耗用量=替換版面前之小計檔數量*（1+耗損率）
//*                
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
using System.IO;
using System.Text.RegularExpressions;
/// <summary>
/// InOut001BL 的摘要描述
/// </summary>
public class InOut001BL : BaseLogic
{
    #region SQL語句
    //選擇所有的PERSO廠商！
    public const string SEL_FACTORY = "SELECT F.RID,F.Factory_ID,F.Factory_ShortName_CN "
                   + "FROM FACTORY AS F "
                   + "WHERE F.RST = 'A' AND F.Is_Perso = 'Y' order by RID";

    public const string SEL_MAKE_CARD_TYPE = "SELECT MCT.RID,CG.Group_Name+MCT.Type_Name as Group_Type_Name,CG.Group_Name,MCT.Type_Name FROM MAKE_CARD_TYPE MCT INNER JOIN CARD_GROUP CG ON MCT.CardGroup_RID = CG.RID AND CG.RST = 'A' WHERE MCT.Is_Import = '" + GlobalString.YNType.Yes + "' AND MCT.RST = 'A' ";

    public const string SEL_FILES_BY_IMPORT_DAY_BATCH = "SELECT DISTINCT STI.Import_FileName " +
                                    " FROM SUBTOTAL_IMPORT STI" +
                                    " WHERE STI.RST = 'A' AND STI.Date_Time >= @date_time_start AND STI.Date_Time <= @date_time_end ";

    public const string CON_SUBTOTAL_PERSO_FACTORY = "SELECT RID FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y' AND Factory_ShortName_EN = @factory_shortname_en";

    public const string CON_CARDTYPE_SURPLUS = "SELECT Stock_Date "
                                       + "FROM CARDTYPE_STOCKS "
                                       + "WHERE RST='A' AND Stock_Date >= @stock_date_start ";

    public const string CON_IMPORT_SUBTOTAL_CHECK = "SELECT RID FROM SUBTOTAL_IMPORT WHERE RST = 'A' AND Import_FileName = @file_name";

    public const string SEL_CARD_TYPE = "SELECT * FROM CARD_TYPE WHERE RST='A' AND TYPE = @type AND AFFINITY = @affinity AND PHOTO = @photo";

    public const string SEL_CARD_TYPE_ALL = "SELECT * FROM CARD_TYPE WHERE RST='A' ";

    public const string DEL_SUBTOTAL = "DELETE FROM SUBTOTAL_IMPORT WHERE Date_Time >= @date_time_start AND Date_Time<=@date_time_end AND Is_Check <> 'Y'";
    //200908CR插入小計檔資訊(替換前) add  by 楊昆 2009/09/02 start
    public const string DEL_SUBTOTAL_REPLACE = "DELETE FROM SUBTOTAL_REPLACE_IMPORT WHERE Date_Time >= @date_time_start AND Date_Time<=@date_time_end AND Is_Check <> 'Y'";
    //200908CR插入小計檔資訊(替換前) add  by 楊昆 2009/09/02 end
    public const string SEL_SUBTOTAL = "select * FROM SUBTOTAL_IMPORT WHERE Date_Time >= @date_time_start AND Date_Time<=@date_time_end AND Is_Check='Y'";

    public const string CON_SUBTOTAL_FILENAME = "SELECT MakeCardType_RID FROM IMPORT_PROJECT WHERE RST= 'A' AND Type = '1' AND File_Name = @file_name";

    public const string SEL_BATCH_MANAGE = "SELECT COUNT(*) FROM BATCH_MANAGE WHERE (RID = 1 OR RID = 5) AND Status = 'Y'";
    public  string UPDATE_BATCH_MANAGE_START = "UPDATE BATCH_MANAGE SET Status = 'Y',RUU='InOut001BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 1 OR RID = 5)";
    public  string UPDATE_BATCH_MANAGE_END = "UPDATE BATCH_MANAGE SET Status = 'N',RUU='InOut001BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 1 OR RID = 5)";

    public const string SEL_CARDGROUP = "select * from dbo.MAKE_CARD_TYPE WHERE RID=@RID";

    public const string SEL_CARDGROUP_BY_CARD = "select group_rid from GROUP_CARD_TYPE where group_rid in (select rid from card_group where param_code='use2') and cardtype_rid =@cardtype_rid";

    public const string SEL_CARDTYPESTOCK = "select count(*) from dbo.CARDTYPE_STOCKS where stock_date >=@stock_date";

    // 物料消耗報警
    public const string SEL_MADE_CARD_WARNNING = "SELECT * FROM (SELECT SI.Perso_Factory_RID,CT.RID,SI.MakeCardType_RID,SUM(SI.Number) AS Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
                        + "WHERE SI.RST = 'A' AND Perso_Factory_RID = @Perso_Factory_RID AND Date_Time>@From_Date_Time AND Date_Time<=@End_Date_Time "
                        + "GROUP BY SI.Perso_Factory_RID,CT.RID,SI.MakeCardType_RID "
                        + "UNION SELECT FCI.Perso_Factory_RID,FCI.CareType_RID,FCI.Status_RID,SUM(FCI.Number) AS Number "
                        + "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID AND CS.Status_Name NOT IN ('3D','DA','PM','RN') "
                        + "WHERE FCI.RST = 'A' AND FCI.Perso_Factory_RID = @Perso_Factory_RID AND FCI.Date_Time>@From_Date_Time AND FCI.Date_Time<=@End_Date_Time "
                        + "GROUP BY FCI.Perso_Factory_RID,FCI.CareType_RID,FCI.Status_RID ) A "
                        + "ORDER BY Perso_Factory_RID,RID,MakeCardType_RID ";

    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 start
    public const string SEL_MADE_CARD_WARNNING_REPLACE = "SELECT * FROM (SELECT SI.Perso_Factory_RID,CT.RID,SI.MakeCardType_RID,SUM(SI.Number) AS Number "
                       + "FROM SUBTOTAL_REPLACE_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                       + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
                       + "WHERE SI.RST = 'A' AND Perso_Factory_RID = @Perso_Factory_RID AND Date_Time>@From_Date_Time AND Date_Time<=@End_Date_Time "
                       + "GROUP BY SI.Perso_Factory_RID,CT.RID,SI.MakeCardType_RID "
                       + "UNION SELECT FCI.Perso_Factory_RID,CT.RID,FCI.Status_RID,SUM(FCI.Number) AS Number "
                       + "FROM FACTORY_CHANGE_REPLACE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID AND CS.Status_Name NOT IN ('3D','DA','PM','RN') "
                       + " INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO "
                       + "WHERE FCI.RST = 'A' AND FCI.Perso_Factory_RID = @Perso_Factory_RID AND FCI.Date_Time>@From_Date_Time AND FCI.Date_Time<=@End_Date_Time "
                       + "GROUP BY FCI.Perso_Factory_RID,CT.RID,FCI.Status_RID ) A "
                       + "ORDER BY Perso_Factory_RID,RID,MakeCardType_RID ";
    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 end
    public const string SEL_EXPRESSIONS_DEFINE_WARNNING = "SELECT ED.Operate,CS.RID "
                        + "FROM EXPRESSIONS_DEFINE ED INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND ED.Type_RID = CS.RID "
                        + "WHERE ED.RST = 'A' AND ED.Expressions_RID = 1";

    public const string DEL_TEMP_MADE_CARD = "DELETE FROM TEMP_MADE_CARD "
                        + "WHERE Perso_Factory_RID = @perso_factory_rid";

    public const string INSERT_INTO_TEMP_MADE_CARD = "INSERT INTO TEMP_MADE_CARD(Perso_Factory_RID,CardType_RID,Number)values("
                        + "@Perso_Factory_RID,@CardType_RID,@Number)";

    public const string SEL_MATERIAL_BY_TEMP_MADE_CARD = " SELECT EI.Serial_Number AS EI_Number,CE.Serial_Number as CE_Number,TMC.Perso_Factory_RID,TMC.Number " +
                        "FROM TEMP_MADE_CARD TMC " +
                        "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND TMC.CardType_RID = CT.RID " +
                        //"INNER JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID " +
                        //"INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID " +
        //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 start
        "LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID " +
        "LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID " +
        //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 end
                        "WHERE TMC.Perso_Factory_RID = @perso_factory_rid ";
    public const string SEL_MATERIAL_BY_TEMP_MADE_CARD_DM = " SELECT DI.Serial_Number DI_Number,A.Perso_Factory_RID,A.Number " +
                        "FROM TEMP_MADE_CARD A " +
                        "INNER JOIN DM_CARDTYPE DCT ON DCT.RST = 'A' AND A.CardType_RID = DCT.CardType_RID " +
                        "INNER JOIN DMTYPE_INFO DI ON DI.RST = 'A' AND DCT.DM_RID = DI.RID "+
                        "WHERE A.Perso_Factory_RID = @perso_factory_rid ";
    public const string SEL_LAST_WORK_DATE = "SELECT TOP 1 Date_Time " +
                    "FROM WORK_DATE " +
                    "WHERE Date_Time < @date_time AND Is_WorkDay='Y' " +
                    "ORDER BY Date_Time DESC";
    public const string SEL_MATERIEL_STOCKS_MANAGER = "SELECT Top 1 MSM.Stock_Date,MSM.Perso_Factory_RID,MSM.Serial_Number,MSM.Number," +
                        "CASE SUBSTRING(MSM.Serial_Number,1,1) WHEN 'A' THEN EI.NAME WHEN 'B' THEN CE.NAME WHEN 'C' THEN DI.NAME END AS NAME " +
                    "FROM MATERIEL_STOCKS_MANAGE MSM LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MSM.Serial_Number = EI.Serial_Number " +
                        "LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MSM.Serial_Number = CE.Serial_Number " +
                        "LEFT JOIN DMTYPE_INFO DI ON DI.RST = 'A' AND MSM.Serial_Number = DI.Serial_Number " +
                    "WHERE MSM.Type = '4' AND MSM.Perso_Factory_RID = @perso_factory_rid AND MSM.Serial_Number = @serial_number " +
                      "ORDER BY Stock_Date Desc";
    public const string SEL_MATERIEL_USED = "SELECT SUM(Number) as Number FROM MATERIEL_STOCKS_USED " +
                        "WHERE RST = 'A' AND Perso_Factory_RID = @perso_factory_rid AND Serial_Number = @serial_number " +
                        " AND Stock_Date>@from_stock_date AND Stock_Date<=@end_stock_date ";

    public const string SEL_LAST_SURPLUS_DAY = "SELECT TOP 1 Stock_Date FROM CARDTYPE_STOCKS WHERE RST = 'A' ORDER BY  Stock_Date DESC";

    public const string SEL_UsedMaterial = "select  b.Serial_Number  from SUBTOTAL_IMPORT a "
                        + " inner join "
                        + " (select a.*,b.Serial_Number from card_type a "
                        + " inner join dbo.CARD_EXPONENT b on a.Exponent_rid = b.rid) b on a.type=b.type and a.affinity=b.affinity and a.photo=b.photo"
                        + " where import_fileName = @import_fileName"
                         + " union"
                        + " select  b.Serial_Number  from SUBTOTAL_IMPORT a "
                        + " inner join "
                        + " (select a.*,b.Serial_Number from card_type a "
                        + " inner join dbo.ENVELOPE_INFO b on a.Envelope_rid = b.rid) b on a.type=b.type and a.affinity=b.affinity and a.photo=b.photo"
                        + " where import_fileName = @import_fileName"
                        + " union"
                        + " select  b.Serial_Number  from SUBTOTAL_IMPORT a "
                        + " inner join"
                        + " (select A.MakeCardType_RID,B.Serial_Number from DM_MAKECARDTYPE A inner join DMTYPE_INFO B on A.DM_RID=B.RID) b"
                        + " on a.makecardtype_rid=b.makecardtype_rid"
                        + " where import_fileName = @import_fileName";
    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/02 start
    public const string SEL_UsedMaterial_REPLACE = "select  b.Serial_Number  from SUBTOTAL_REPLACE_IMPORT a "
                       + " inner join "
                       + " (select a.*,b.Serial_Number from card_type a "
                       + " inner join dbo.CARD_EXPONENT b on a.Exponent_rid = b.rid) b on a.type=b.type and a.affinity=b.affinity and a.photo=b.photo"
                       + " where import_fileName = @import_fileName"
                       + " union"
                       + " select  b.Serial_Number  from SUBTOTAL_REPLACE_IMPORT a "
                       + " inner join "
                       + " (select a.*,b.Serial_Number from card_type a "
                       + " inner join dbo.ENVELOPE_INFO b on a.Envelope_rid = b.rid) b on a.type=b.type and a.affinity=b.affinity and a.photo=b.photo"
                       + " where import_fileName = @import_fileName"
                       + " union"
                       + " select  b.Serial_Number  from SUBTOTAL_REPLACE_IMPORT a "
                       + " inner join"
                       + " (select A.MakeCardType_RID,B.Serial_Number from DM_MAKECARDTYPE A inner join DMTYPE_INFO B on A.DM_RID=B.RID) b"
                       + " on a.makecardtype_rid=b.makecardtype_rid"
                       + " where import_fileName = @import_fileName";
    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/02 end
    #endregion

    Depository010BL bl = new Depository010BL();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public InOut001BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 將小計檔匯入標設置為FALSE
    /// </summary>
    /// <returns></returns>
    public void ImportSubTotalEnd()
    {
        try
        {
            dao.ExecuteNonQuery(UPDATE_BATCH_MANAGE_END);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 檢查小計檔匯入刪除是否已經被開起。如果已經開起，返回FALSE
    ///                                   如果沒有開起，開起，并返回TRUE
    /// </summary>
    /// <returns></returns>
    public bool ImportSubTotalStart()
    {
        try
        {
            DataSet dsBATCH_MANAGE = dao.GetList(SEL_BATCH_MANAGE);
            if (null != dsBATCH_MANAGE &&
                    dsBATCH_MANAGE.Tables.Count > 0 &&
                    dsBATCH_MANAGE.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dsBATCH_MANAGE.Tables[0].Rows[0][0]) > 0)
                {
                    return false;
                }
                dao.ExecuteNonQuery(UPDATE_BATCH_MANAGE_START);
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 獲得批次
    /// </summary>
    /// <returns>DataSet[批次]</returns>
    public DataSet GetBatchInfo()
    {
        DataSet dsMAKE_CARD_TYPE = null;
        try
        {
            this.dirValues.Clear();

            dsMAKE_CARD_TYPE = dao.GetList(SEL_MAKE_CARD_TYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dsMAKE_CARD_TYPE;
    }

    /// <summary>
    /// 獲得小計檔名稱
    /// </summary>
    /// <returns>DataSet[小計檔名稱]</returns>
    public DataSet getFilesByDayBatch(string Date_Time, string RID)
    {
        DataSet dsFilesByDayBatch = null;
        try
        {
            this.dirValues.Clear();
            dirValues.Add("date_time_start", Date_Time + " 00:00:00");
            dirValues.Add("date_time_end", Date_Time + " 23:59:59");
            dirValues.Add("makecardtype_rid", RID);
            dsFilesByDayBatch = dao.GetList(SEL_FILES_BY_IMPORT_DAY_BATCH + "AND STI.MakeCardType_RID = @makecardtype_rid", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dsFilesByDayBatch;
    }

    /// <summary>
    /// 獲得小計檔名稱
    /// </summary>
    /// <returns>DataSet[小計檔名稱]</returns>
    public DataSet getFilesByDayBatch(string Date_Time)
    {
        DataSet dsFilesByDayBatch = null;
        try
        {
            this.dirValues.Clear();
            dirValues.Add("date_time_start", Date_Time + " 00:00:00");
            dirValues.Add("date_time_end", Date_Time + " 23:59:59");
            dsFilesByDayBatch = dao.GetList(SEL_FILES_BY_IMPORT_DAY_BATCH, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dsFilesByDayBatch;
    }

    /// <summary>
    /// 檢查小計檔名稱格式
    /// </summary>
    /// <returns>DataSet[名稱]</returns>
    public DataSet GetFactory_ShortName_EN(string Factory_ShortName_EN)
    {
        DataSet dsGetFactory_ShortName_EN = null;
        try
        {
            this.dirValues.Clear();
            dirValues.Add("factory_shortname_en", Factory_ShortName_EN);
            dsGetFactory_ShortName_EN = dao.GetList(CON_SUBTOTAL_PERSO_FACTORY, dirValues);
            if (dsGetFactory_ShortName_EN.Tables[0].Rows.Count == 0)
            {
                throw new AlertException("不存在該Perso廠！");
            }
        }
        catch (AlertException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dsGetFactory_ShortName_EN;
    }

    /// <summary>
    /// 獲取廠商代號
    /// </summary>
    /// <returns>DataSet[名稱]</returns>
    public string GetFactory_RID(string Factory_ShortName_EN)
    {
        DataSet dsGetFactory_ShortName_EN = null;
        string Factory_RID = "";
        try
        {
            this.dirValues.Clear();
            dirValues.Add("factory_shortname_en", Factory_ShortName_EN);
            dsGetFactory_ShortName_EN = dao.GetList(CON_SUBTOTAL_PERSO_FACTORY, dirValues);
            if (dsGetFactory_ShortName_EN.Tables[0].Rows.Count != 0)
            {
                Factory_RID = dsGetFactory_ShortName_EN.Tables[0].Rows[0]["RID"].ToString();
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
    /// 是否日結
    /// </summary>
    /// <returns>string[日結日期]</returns>
    public void Is_Check(DateTime Date)
    {
        DataSet dsIs_Check = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("stock_date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("stock_date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            dsIs_Check = dao.GetList(CON_CARDTYPE_SURPLUS, dirValues);
            if (dsIs_Check.Tables[0].Rows.Count != 0)
            {
                throw new AlertException("匯入日期已日結，無法操作！");
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 是否存在重復匯入小計檔
    /// </summary>
    public void Exists_File(string FileName)
    {
        DataSet dsExists_File = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("file_name", FileName);
            dsExists_File = dao.GetList(CON_IMPORT_SUBTOTAL_CHECK, dirValues);
            if (dsExists_File.Tables[0].Rows.Count != 0)
            {
                throw new AlertException("該小計檔已經匯入過，不能重復匯入！");
            }
        }
        catch (AlertException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 取所有卡種訊息
    /// </summary>
    /// <returns></returns>
    public DataTable getCardType()
    {
        DataSet dstCardType = null;
        try
        {
            dirValues.Clear();
            dstCardType = dao.GetList(SEL_CARD_TYPE_ALL, dirValues);
            if (dstCardType != null && dstCardType.Tables.Count > 0)
                return dstCardType.Tables[0];

            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 驗證文件格式，返回正確資訊及錯誤資訊
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="dtblFileImp"></param>
    /// <returns></returns>
    public DataTable ImportCheck(string strMakeCardTypeRID, string strPath,
            string file_name,
            DateTime import_dateTime,
            DataTable dtblCardType,
            List<string> lstErrFunc)
    {
        #region 驗證文件格式，返回正確資訊及錯誤資訊

        StreamReader sr = null;
        string strGroupRID = "";
        try
        {
            DataTable dtFileImport = getDataTable();
            // 檢查文件不存在
            if (!File.Exists(strPath + file_name))
            {
                return dtFileImport;
            }

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strMakeCardTypeRID));
            DataTable dtbl = dao.GetList(SEL_CARDGROUP, dirValues).Tables[0];
            if (dtbl.Rows.Count > 0)
                strGroupRID = dtbl.Rows[0]["cardgroup_rid"].ToString();

            sr = new StreamReader(strPath + file_name, System.Text.Encoding.Default);
            string[] strLine;
            string strReadLine = "";
            int count = 1;

            while ((strReadLine = sr.ReadLine()) != null)
            {
                bool blnTag = true;
                strLine = new string[3];

                string Pattern = @"\w+";
                MatchCollection Matches = Regex.Matches(strReadLine.Replace(",", ""), Pattern, RegexOptions.IgnoreCase);


                //strLine = strReadLine.Split(GlobalString.FileSplit.Split);
                if (strReadLine.Contains("===="))
                {
                    count++;
                    continue;
                }
                else if (strReadLine.Contains("PHOTO"))
                {
                    count++;
                    continue;
                }
                else if (strReadLine.Contains("ACTION"))
                {
                    count++;
                    continue;
                }
                else if (strReadLine.Contains("總卡數"))
                {
                    count++;
                    continue;
                }
                //匯入文件格式不正確,列數不足
                else if (Matches.Count != 3)
                {
                    blnTag = false;
                    lstErrFunc.Add("第" + (count).ToString() + "行資料格式不正確。");
                }
                else
                {
                    for (int i = 0; i < Matches.Count; i++)
                    {
                        strLine[i] = Matches[i].ToString();
                    }

                    DataRow dr = dtFileImport.NewRow();//作為插入資料庫
                    for (int i = 0; i < strLine.Length; i++)
                    {
                        int num = i + 1;
                        // 是否為空檢查
                        if (StringUtil.IsEmpty(strLine[i]))
                        {
                            blnTag = false;
                            lstErrFunc.Add("第" + (count).ToString() + "行第" + num.ToString() + "列為空;");
                        }
                        else
                        {
                            // 檢查每列的格式是否正確
                            string strError = CheckFileColumn(strLine[i], num, count);
                            if (strError.Length != 0)
                            {
                                blnTag = false;
                                lstErrFunc.Add(strError);
                            }
                        }
                    }

                    // 沒有錯誤時，添加到臨時表中
                    DataRow[] drs = null;
                    if (blnTag)
                    {
                        drs = dtblCardType.Select("type = " + strLine[1].Substring(0, 3) + " AND Affinity = " + strLine[1].Substring(3, 4) + " AND Photo = " + strLine[1].Substring(7, 2));
                        if (drs.Length == 0)
                        {
                            blnTag = false;
                            lstErrFunc.Add("第" + (count).ToString() + "行第2列對應的卡種不存在");
                        }
                        else if (Convert.ToString(drs[0]["Is_Using"]) != "Y")
                        {
                            blnTag = false;
                            lstErrFunc.Add("第" + (count).ToString() + "行第2列對應的卡種已經停用");
                        }
                        else if (Convert.ToDateTime(drs[0]["Begin_Time"]) > import_dateTime ||
                            (Convert.ToDateTime(drs[0]["End_Time"]) < import_dateTime &&
                             Convert.ToDateTime(drs[0]["End_Time"]).ToString("yyyy-MM-dd") != "1900-01-01"))
                        {
                            blnTag = false;
                            lstErrFunc.Add("第" + (count).ToString() + "行第2列對應的卡種不在有效期");
                        }
                    }

                    if (blnTag)
                    {
                        //string strCardRID = drs[0]["RID"].ToString();
                        //dirValues.Clear();
                        //dirValues.Add("cardtype_rid", int.Parse(strCardRID));
                        //DataTable dtbl1 = dao.GetList(SEL_CARDGROUP_BY_CARD, dirValues).Tables[0];
                        //if (dtbl1.Rows.Count > 0)
                        //{
                        //    if (dtbl1.Rows[0][0].ToString() != strGroupRID)
                        //    {
                        //        blnTag = false;
                        //        lstErrFunc.Add("第" + (count).ToString() + "行第2列對應的卡種不屬於該群組");
                        //    }
                        //}
                    }

                    if (blnTag)
                    {
                        dr["Action"] = strLine[0];
                        dr["Old_CardType_RID"] = drs[0]["RID"];
                        dr["Type"] = strLine[1].Substring(0, 3);
                        dr["AFFINITY"] = strLine[1].Substring(3, 4);
                        dr["PHOTO"] = strLine[1].Substring(7, 2);
                        dr["Number"] = strLine[2];
                        dr["Change_Space_RID"] = drs[0]["Change_Space_RID"];
                        dr["Replace_Space_RID"] = drs[0]["Replace_Space_RID"];

                        dtFileImport.Rows.Add(dr);
                    }
                }
                count++;
            }

            return dtFileImport;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            if (sr != null)
                sr.Close();
        }
        #endregion
    }

    /// <summary>
    /// 取的匯入文件DataTable
    /// </summary>
    /// <returns></returns>
    private DataTable getDataTable()
    {
        DataTable dtRet = new DataTable();
        dtRet.Columns.Add(new DataColumn("Action", Type.GetType("System.String")));
        dtRet.Columns.Add(new DataColumn("Old_CardType_RID", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Type", Type.GetType("System.String")));
        dtRet.Columns.Add(new DataColumn("Affinity", Type.GetType("System.String")));
        dtRet.Columns.Add(new DataColumn("Photo", Type.GetType("System.String")));
        dtRet.Columns.Add(new DataColumn("Number", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Change_Space_RID", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Replace_Space_RID", Type.GetType("System.Int32")));
        return dtRet;
    }

    /// <summary>
    /// 根據匯入的小計檔，生成物料消耗記錄，并判斷物料庫存是否在安全水位，
    /// 如果不在安全水位，報警。
    /// </summary>
    /// <param name="strFactory_RID"></param>
    /// <param name="importDate"></param>
    //public void Material_Used_Warnning(string strFactory_RID,
    //    DateTime importDate,string strFileName)
    //{
    //    try
    //    {
    //        // 取最後日結日期。
    //        DateTime TheLastestSurplusDate = getLastSurplusDate();

    //        #region 計算從最後一天日結日期第二天到小計檔匯入當天的製成卡數，保存到臨時表(TEMP_MADE_CARD)
    //        dirValues.Clear();
    //        dirValues.Add("Perso_Factory_RID", strFactory_RID);
    //        dirValues.Add("From_Date_Time", TheLastestSurplusDate.ToString("yyyy/MM/dd 23:59:59"));
    //        dirValues.Add("End_Date_Time", importDate.ToString("yyyy/MM/dd 23:59:59"));
    //       // DataSet dsMade_Card = dao.GetList(SEL_MADE_CARD_WARNNING, dirValues);
    //        //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/02 start
    //        DataSet dsMade_Card = dao.GetList(SEL_MADE_CARD_WARNNING_REPLACE, dirValues);
    //        //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/02 end
    //        DataSet dsEXPRESSIONS_DEFINE = dao.GetList(SEL_EXPRESSIONS_DEFINE_WARNNING);

    //        //卡種消耗表
    //        DataTable dtUSE_CARDTYPE = new DataTable();
    //        dtUSE_CARDTYPE.Columns.Add("Perso_Factory_RID");
    //        dtUSE_CARDTYPE.Columns.Add("CardType_RID");
    //        dtUSE_CARDTYPE.Columns.Add("Number");

    //        //按Perso廠、卡種的計算消耗量（循環加總各種狀況的消耗數量）
    //        int Card_Type_Rid = 0;
    //        int Perso_Factory_RID = 0;
    //        int Number = 0;
    //        //todo 此循環可以改進為存儲過程
    //        foreach (DataRow dr in dsMade_Card.Tables[0].Rows)
    //        {
    //            if ((Convert.ToInt32(dr["RID"]) != Card_Type_Rid) ||
    //                (Convert.ToInt32(dr["Perso_Factory_RID"]) != Perso_Factory_RID))
    //            {
    //                if (Card_Type_Rid != 0 && Perso_Factory_RID != 0 && Number != 0)
    //                {
    //                    DataRow drow = dtUSE_CARDTYPE.NewRow();
    //                    drow["Number"] = Number.ToString();
    //                    drow["Perso_Factory_RID"] = Perso_Factory_RID.ToString();
    //                    drow["CardType_RID"] = Card_Type_Rid.ToString();
    //                    dtUSE_CARDTYPE.Rows.Add(drow);
    //                }

    //                #region 取消耗卡公式,計算消耗卡數
    //                Number = 0;
    //                DataRow[] drEXPRESSIONS = dsEXPRESSIONS_DEFINE.Tables[0].Select("RID = " + dr["MakeCardType_RID"].ToString());
    //                if (drEXPRESSIONS.Length > 0)
    //                {
    //                    if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Add_RID)
    //                    {
    //                        Number += Convert.ToInt32(dr["Number"]);
    //                        Card_Type_Rid = Convert.ToInt32(dr["RID"]);
    //                        Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                    }
    //                    else if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Del_RID)
    //                    {
    //                        Number -= Convert.ToInt32(dr["Number"]);
    //                        Card_Type_Rid = Convert.ToInt32(dr["RID"]);
    //                        Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                    }
    //                }
    //                #endregion
    //            }
    //            else
    //            {
    //                #region 取消耗卡公式,計算消耗卡數
    //                DataRow[] drEXPRESSIONS = dsEXPRESSIONS_DEFINE.Tables[0].Select("RID = " + dr["MakeCardType_RID"].ToString());
    //                if (drEXPRESSIONS.Length > 0)
    //                {
    //                    if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Add_RID)
    //                    {
    //                        Number += Convert.ToInt32(dr["Number"]);
    //                    }
    //                    else if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Del_RID)
    //                    {
    //                        Number -= Convert.ToInt32(dr["Number"]);
    //                    }
    //                }
    //                #endregion
    //            }
    //        }
    //        if (Card_Type_Rid != 0 && Perso_Factory_RID != 0 && Number != 0)
    //        {
    //            DataRow drow = dtUSE_CARDTYPE.NewRow();
    //            drow["Number"] = Number.ToString();
    //            drow["Perso_Factory_RID"] = Perso_Factory_RID.ToString();
    //            drow["CardType_RID"] = Card_Type_Rid.ToString();
    //            dtUSE_CARDTYPE.Rows.Add(drow);
    //        }

    //        // 刪除臨時表中的數據
    //        this.dirValues.Clear();
    //        this.dirValues.Add("perso_factory_rid", strFactory_RID);
    //        dao.ExecuteNonQuery(DEL_TEMP_MADE_CARD,this.dirValues);

    //        foreach (DataRow dr in dtUSE_CARDTYPE.Rows)
    //        {
    //            this.dirValues.Clear();
    //            this.dirValues.Add("Perso_Factory_RID", dr["Perso_Factory_RID"].ToString());
    //            this.dirValues.Add("CardType_RID", dr["CardType_RID"].ToString());
    //            this.dirValues.Add("Number", dr["Number"].ToString());
    //            dao.ExecuteNonQuery(INSERT_INTO_TEMP_MADE_CARD, this.dirValues);
    //        }

    //        #endregion 計算當天製成卡數

    //        // 根據製成卡數，計算物料消耗
    //        DataTable dtMATERIAL_USED = getMaterialUsed(strFactory_RID, importDate);

           

    //        // 計算物料剩余數量并警示
    //        getMaterielStocks(TheLastestSurplusDate,
    //                strFactory_RID, 
    //                importDate,
    //                dtMATERIAL_USED, strFileName);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
    //    }
    //}

    /// <summary>
    /// 取最后一次日結日期
    /// </summary>
    /// <returns></returns>
    public DateTime getLastSurplusDate()
    {
        DateTime dtLastSurplusDate = Convert.ToDateTime("1900-01-01");
        try
        {
            DataSet dsLAST_SURPLUS_DAY = dao.GetList(SEL_LAST_SURPLUS_DAY);
            if (dsLAST_SURPLUS_DAY != null
                    && dsLAST_SURPLUS_DAY.Tables.Count > 0
                    && dsLAST_SURPLUS_DAY.Tables[0].Rows.Count > 0)
            {
                dtLastSurplusDate = Convert.ToDateTime(dsLAST_SURPLUS_DAY.Tables[0].Rows[0]["Stock_Date"].ToString());
            }
            return dtLastSurplusDate;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 計算物料剩余數量并警示
    /// </summary>
    /// <param name="dtLastWorkDate"></param>
    /// <param name="strFactory_RID"></param>
    /// <param name="importDate"></param>
    /// <param name="dtMATERIAL_USED"></param>
    //public void getMaterielStocks(DateTime dtLastestSurplus,
    //        string strFactory_RID,
    //        DateTime importDate,
    //        DataTable dtMATERIAL_USED,string strFileName)
    //{
    //    try
    //    {
    //        #region 根據前一天的庫存及今天的庫存。計算物料剩餘數量，判斷是否報警
    //        dirValues.Clear();
    //        dirValues.Add("import_fileName", strFileName);
    //        //DataTable dtblMaterial = dao.GetList(SEL_UsedMaterial, dirValues).Tables[0];
    //        //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/02 start
    //        DataTable dtblMaterial = dao.GetList(SEL_UsedMaterial_REPLACE, dirValues).Tables[0];
    //        //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/02 end
    //        Depository010BL bl010 = new Depository010BL();


    //        foreach (DataRow drMATERIAL_USED in dtMATERIAL_USED.Rows)
    //        {
    //            //if (dtblMaterial.Rows.Count > 0)
    //            //{
    //            //    if (dtblMaterial.Select("Serial_Number='" + drMATERIAL_USED["Serial_Number"].ToString() + "'").Length == 0)
    //            //        continue;
    //            //}
    //            if (dtblMaterial.Select("Serial_Number='" + drMATERIAL_USED["Serial_Number"].ToString() + "'").Length > 0)
    //            {
    //            dirValues.Clear();
    //            dirValues.Add("perso_factory_rid", strFactory_RID);
    //            dirValues.Add("serial_number", drMATERIAL_USED["Serial_Number"].ToString());
    //            DataSet dsMaterielStocksManager = dao.GetList(SEL_MATERIEL_STOCKS_MANAGER,dirValues);
    //            if (null != dsMaterielStocksManager &&
    //                dsMaterielStocksManager.Tables.Count > 0 &&
    //                dsMaterielStocksManager.Tables[0].Rows.Count > 0)
    //            {
    //                // 從盤整日到日結日，耗用
    //                this.dirValues.Clear();
    //                this.dirValues.Add("perso_factory_rid", strFactory_RID);
    //                this.dirValues.Add("serial_number", drMATERIAL_USED["Serial_Number"].ToString());
    //                this.dirValues.Add("from_stock_date", Convert.ToDateTime(dsMaterielStocksManager.Tables[0].Rows[0]["Stock_Date"]).ToString("yyyy/MM/dd 23:59:59"));
    //                this.dirValues.Add("end_stock_date", dtLastestSurplus.ToString("yyyy/MM/dd 23:59:59"));
    //                DataSet dsUsedMaterial = dao.GetList(SEL_MATERIEL_USED, this.dirValues);
    //                if (null != dsUsedMaterial &&
    //                    dsUsedMaterial.Tables.Count > 0 &&
    //                    dsUsedMaterial.Tables[0].Rows.Count > 0)
    //                {
    //                    // 盤整時的庫存
    //                    int intLastStockNumber = Convert.ToInt32(dsMaterielStocksManager.Tables[0].Rows[0]["Number"].ToString());
    //                    // 從盤整日到最結餘日的消耗
    //                    int intUsedMaterialFront = 0;
    //                    if (dsUsedMaterial.Tables[0].Rows[0]["Number"] != DBNull.Value)
    //                        intUsedMaterialFront = Convert.ToInt32(dsUsedMaterial.Tables[0].Rows[0]["Number"]);

    //                    // 最後結餘日后的消耗
    //                    int intUsedMaterialAfter = Convert.ToInt32(drMATERIAL_USED["Number"]);


    //                    // 庫存為0時，顯示庫存不足
    //                    if (intLastStockNumber < 0)
    //                    {
    //                        // 庫存不足
    //                        //等待0不一定有消耗，此處不應該發警訊，看下面的條件，廠商結余數是否小於消耗就好了！
    //                        //string[] arg = new string[1];
    //                        //arg[0] = dsMaterielStocksManager.Tables[0].Rows[0]["Name"].ToString();
    //                        //Warning.SetWarning(GlobalString.WarningType.SubtotalMaterialInMiss, arg);
    //                    }
    //                    // 如果前一天的庫存小余今天的消耗
    //                    else if (intLastStockNumber < (intUsedMaterialFront + intUsedMaterialAfter))
    //                    {
    //                        if (bl010.DmNotSafe_Type(drMATERIAL_USED["Serial_Number"].ToString()))
    //                        {
    //                            // 庫存不足
    //                            string[] arg = new string[1];
    //                            arg[0] = dsMaterielStocksManager.Tables[0].Rows[0]["Name"].ToString();

    //                            //小計檔匯入，為什么要發廠商異動檔的警訊，注釋掉！
    //                            //Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInMiss, arg);

    //                            Warning.SetWarning(GlobalString.WarningType.SubtotalMaterialInMiss, arg);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        // 取物料的安全庫存訊息
    //                        DataSet dtMateriel = bl.GetMateriel(drMATERIAL_USED["Serial_Number"].ToString());
    //                        if (null != dtMateriel &&
    //                            dtMateriel.Tables.Count > 0 &&
    //                            dtMateriel.Tables[0].Rows.Count > 0)
    //                        {
    //                            // 最低安全庫存
    //                            if (GlobalString.SafeType.storage == Convert.ToString(dtMateriel.Tables[0].Rows[0]["Safe_Type"]))
    //                            {
    //                                // 廠商結餘低於最低安全庫存數值時
    //                                if (intLastStockNumber - intUsedMaterialAfter - intUsedMaterialFront <
    //                                    Convert.ToInt32(dtMateriel.Tables[0].Rows[0]["Safe_Number"]))
    //                                {
    //                                    string[] arg = new string[1];
    //                                    arg[0] = dtMateriel.Tables[0].Rows[0]["Name"].ToString();
    //                                    //小計檔匯入，為什么要發廠商異動檔的警訊，注釋掉！
    //                                    //Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInSafe, arg);
    //                                    Warning.SetWarning(GlobalString.WarningType.SubtoalMaterialInSafe, arg);
    //                                }
    //                                // 安全天數
    //                            }
    //                            else if (GlobalString.SafeType.days == Convert.ToString(dtMateriel.Tables[0].Rows[0]["Safe_Type"]))
    //                            {
    //                                // 檢查庫存是否充足
    //                                if (!bl.CheckMaterielSafeDays(drMATERIAL_USED["Serial_Number"].ToString(),
    //                                                        Convert.ToInt32(drMATERIAL_USED["Perso_Factory_RID"].ToString()),
    //                                                        Convert.ToInt32(dtMateriel.Tables[0].Rows[0]["Safe_Number"]),
    //                                                        intLastStockNumber - intUsedMaterialFront - intUsedMaterialAfter))
    //                                {
    //                                    string[] arg = new string[1];
    //                                    arg[0] = dtMateriel.Tables[0].Rows[0]["Name"].ToString();
    //                                    //小計檔匯入，為什么要發廠商異動檔的警訊，注釋掉！
    //                                    //Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInSafe, arg);
    //                                    Warning.SetWarning(GlobalString.WarningType.SubtoalMaterialInSafe, arg);
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            }
    //        }
            
    //        #endregion 根據前一天的庫存及今天的庫存。計算物料剩餘數量，判斷是否報警
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}

    /// <summary>
    /// 用小計檔生成卡片對應的物料耗用記錄
    /// </summary>
    /// <returns></returns>
    //public DataTable getMaterialUsed(string strFactory_RID, DateTime importDate)
    //{
    //    DataTable dtUSE_CARDTYPE = new DataTable();
    //    Depository010BL BL010 = new Depository010BL();
    //    dtUSE_CARDTYPE.Columns.Add("Stock_Date", Type.GetType("System.DateTime"));
    //    dtUSE_CARDTYPE.Columns.Add("Number", Type.GetType("System.Int32"));
    //    dtUSE_CARDTYPE.Columns.Add("Serial_Number", Type.GetType("System.String"));
    //    dtUSE_CARDTYPE.Columns.Add("Perso_Factory_RID", Type.GetType("System.Int32"));

    //    try
    //    {
    //        dirValues.Clear();
    //        dirValues.Add("perso_factory_rid", strFactory_RID);
    //        //取信封和寄卡單耗用記錄，DataSet<物料耗用記錄>
    //        DataSet dsMATERIAL_BY_SUBTOTAL = dao.GetList(SEL_MATERIAL_BY_TEMP_MADE_CARD, dirValues);
    //        foreach (DataRow dr in dsMATERIAL_BY_SUBTOTAL.Tables[0].Rows)
    //        {
    //            if (dr["CE_Number"].ToString() != "")
    //            {
    //                DataRow[] drSelect = dtUSE_CARDTYPE.Select("Serial_Number = '" + dr["CE_Number"].ToString() + "'");
    //                if (drSelect.Length > 0)
    //                {
    //                    drSelect[0]["Number"] = Convert.ToInt32(drSelect[0]["Number"]) + Convert.ToInt32(dr["Number"]); 
    //                }
    //                else
    //                {
    //                    DataRow drNewCARD_EXPONENT = dtUSE_CARDTYPE.NewRow();
    //                    drNewCARD_EXPONENT["Stock_Date"] = importDate;
    //                    //drNewCARD_EXPONENT["Number"] = Convert.ToInt32(dr["Number"]);
    //                    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 start
    //                    drNewCARD_EXPONENT["Number"] = BL010.ComputeMaterialNumber(dr["CE_Number"].ToString(), Convert.ToInt64(dr["Number"]));
    //                    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 end
    //                    drNewCARD_EXPONENT["Serial_Number"] = dr["CE_Number"].ToString();
    //                    drNewCARD_EXPONENT["Perso_Factory_RID"] = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                    dtUSE_CARDTYPE.Rows.Add(drNewCARD_EXPONENT);
    //                }
    //            }

    //            if (dr["EI_Number"].ToString() != "")
    //            {
    //                DataRow[] drSelect = dtUSE_CARDTYPE.Select("Serial_Number = '" + dr["EI_Number"].ToString() + "'");
    //                if (drSelect.Length > 0)
    //                {
    //                    drSelect[0]["Number"] = Convert.ToInt32(drSelect[0]["Number"]) + Convert.ToInt32(dr["Number"]);
    //                }
    //                else
    //                {
    //                    DataRow drNewENVELOPE_INFO = dtUSE_CARDTYPE.NewRow();
    //                    drNewENVELOPE_INFO["Stock_Date"] = importDate;
    //                    //drNewENVELOPE_INFO["Number"] = Convert.ToInt32(dr["Number"]);
    //                    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 start
    //                    drNewENVELOPE_INFO["Number"] = BL010.ComputeMaterialNumber(dr["EI_Number"].ToString(), Convert.ToInt64(dr["Number"]));
    //                    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 end
    //                    drNewENVELOPE_INFO["Serial_Number"] = dr["EI_Number"].ToString();
    //                    drNewENVELOPE_INFO["Perso_Factory_RID"] = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                    dtUSE_CARDTYPE.Rows.Add(drNewENVELOPE_INFO);
    //                }
    //            }
    //        }

    //        //取DM耗用記錄，DataSet<DM物料耗用記錄>
    //        DataSet MATERIAL_BY_SUBTOTAL_DM = dao.GetList(SEL_MATERIAL_BY_TEMP_MADE_CARD_DM, dirValues);
    //        foreach (DataRow dr in MATERIAL_BY_SUBTOTAL_DM.Tables[0].Rows)
    //        {
    //            if (dr["DI_Number"].ToString() != "")
    //            {
    //                DataRow[] drSelect = dtUSE_CARDTYPE.Select("Serial_Number = '" + dr["DI_Number"].ToString() + "'");
    //                if (drSelect.Length > 0)
    //                {
    //                    drSelect[0]["Number"] = Convert.ToInt32(drSelect[0]["Number"]) + Convert.ToInt32(dr["Number"]);
    //                }
    //                else
    //                {
    //                    DataRow drNewDMTYPE_INFO = dtUSE_CARDTYPE.NewRow();
    //                    drNewDMTYPE_INFO["Stock_Date"] = importDate;
    //                    //drNewDMTYPE_INFO["Number"] = Convert.ToInt32(dr["Number"]);
    //                    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 start
    //                    drNewDMTYPE_INFO["Number"] = BL010.ComputeMaterialNumber(dr["DI_Number"].ToString(), Convert.ToInt64(dr["Number"]));
    //                    //200908CR耗用量=替換版面前之小計檔數量*（1+耗損率） add  by 楊昆 2009/09/01 end
    //                    drNewDMTYPE_INFO["Serial_Number"] = dr["DI_Number"].ToString();
    //                    drNewDMTYPE_INFO["Perso_Factory_RID"] = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                    dtUSE_CARDTYPE.Rows.Add(drNewDMTYPE_INFO);
    //                }
    //            }
    //        }

    //        return dtUSE_CARDTYPE;

    //    }
    //    catch (AlertException ex)
    //    {
    //        throw ex;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}

    /// <summary>
    /// 新增匯入記錄
    /// </summary>
    /// <param name="dtblFileImp">小計檔資訊</param>
    /// <param name="Date">匯入日期</param>
    /// <param name="file_name">匯入文件名稱</param>
    /// <param name="MakeCardType_RID">批次</param>
    /// <param name="strFactory_RID">廠商RID</param>
    public void AddImp(DataTable dtblFileImp,
                    string DateImp,
                    string file_name,
                    string MakeCardType_RID,
                    string strFactory_RID,
                    DataTable dtblCardType)
    {
        try
        {
            // 若Action為5（到期換卡），扣“換卡版面”，“換卡版面”沒有輸入則扣“替換卡版面”，
            // 若兩個都沒輸入扣“版面簡稱”。
            // 若Action為“1（新卡）”或“2（掛補）”或“3（毀補）”則扣“替換卡版面”，
            // 若“替換卡版面”沒有輸入，則扣“版面簡稱”。

           
            //200908CR插入小計檔資訊(替換前) add  by 楊昆 2009/09/02 start
            DataTable dtblFileImpReplace = dtblFileImp.Copy();
            //200908CR插入小計檔資訊(替換前) add  by 楊昆 2009/09/02 end

            #region 換卡操作
            foreach (DataRow drowFileImp in dtblFileImp.Rows)
            {
                if (drowFileImp["Action"].ToString() == "5")
                {
                    if (Convert.ToInt32(drowFileImp["Change_Space_RID"]) != 0)
                    {
                        for (int intLoop = 0; intLoop < dtblCardType.Rows.Count; intLoop++)
                        {
                            if (Convert.ToInt32(dtblCardType.Rows[intLoop]["RID"]) ==
                                Convert.ToInt32(drowFileImp["Change_Space_RID"]))
                            {
                                drowFileImp["TYPE"] = dtblCardType.Rows[intLoop]["TYPE"];
                                drowFileImp["AFFINITY"] = dtblCardType.Rows[intLoop]["AFFINITY"];
                                drowFileImp["PHOTO"] = dtblCardType.Rows[intLoop]["PHOTO"];
                                break;
                            }
                        }
                    }
                    else if (Convert.ToInt32(drowFileImp["Replace_Space_RID"]) != 0)
                    {
                        for (int intLoop = 0; intLoop < dtblCardType.Rows.Count; intLoop++)
                        {
                            if (Convert.ToInt32(dtblCardType.Rows[intLoop]["RID"]) ==
                                Convert.ToInt32(drowFileImp["Replace_Space_RID"]))
                            {
                                drowFileImp["TYPE"] = dtblCardType.Rows[intLoop]["TYPE"];
                                drowFileImp["AFFINITY"] = dtblCardType.Rows[intLoop]["AFFINITY"];
                                drowFileImp["PHOTO"] = dtblCardType.Rows[intLoop]["PHOTO"];
                                break;
                            }
                        }
                    }
                }
                else if (drowFileImp["Action"].ToString() == "1" ||
                            drowFileImp["Action"].ToString() == "2" ||
                            drowFileImp["Action"].ToString() == "3")
                {
                    if (Convert.ToInt32(drowFileImp["Replace_Space_RID"]) != 0)
                    {
                        for (int intLoop = 0; intLoop < dtblCardType.Rows.Count; intLoop++)
                        {
                            if (Convert.ToInt32(dtblCardType.Rows[intLoop]["RID"]) ==
                                Convert.ToInt32(drowFileImp["Replace_Space_RID"]))
                            {
                                drowFileImp["TYPE"] = dtblCardType.Rows[intLoop]["TYPE"];
                                drowFileImp["AFFINITY"] = dtblCardType.Rows[intLoop]["AFFINITY"];
                                drowFileImp["PHOTO"] = dtblCardType.Rows[intLoop]["PHOTO"];
                                break;
                            }
                        }
                    }
                }
            }
            #endregion 換卡操作

            //事務開始
            dao.OpenConnection();

            //200908CR插入小計檔資訊(替換前) add  by 楊昆 2009/09/02 start
            SUBTOTAL_REPLACE_IMPORT SRI = new SUBTOTAL_REPLACE_IMPORT();
            foreach (DataRow dr in dtblFileImpReplace.Rows)
            {
                SRI.Action = dr["ACTION"].ToString();
                SRI.Old_CardType_RID = Convert.ToInt32(dr["Old_CardType_RID"]);
                SRI.TYPE = dr["TYPE"].ToString();
                SRI.AFFINITY = dr["AFFINITY"].ToString();
                SRI.PHOTO = dr["PHOTO"].ToString();
                SRI.Number = Convert.ToInt32(dr["Number"].ToString());
                SRI.Date_Time = Convert.ToDateTime(DateImp);
                SRI.Perso_Factory_RID = Convert.ToInt32(strFactory_RID);
                SRI.MakeCardType_RID = Convert.ToInt32(MakeCardType_RID);
                SRI.Change_Space_RID = Convert.ToInt32(dr["Change_Space_RID"]);
                SRI.Replace_Space_RID = Convert.ToInt32(dr["Replace_Space_RID"]);
                SRI.Import_FileName = file_name;
                SRI.Is_Check = "N";
                SRI.Check_Date = Convert.ToDateTime("1900-01-01");
                dao.Add<SUBTOTAL_REPLACE_IMPORT>(SRI, "RID");
            }
            //200908CR插入小計檔資訊(替換前) add  by 楊昆 2009/09/02 end

            // 插入小計檔資訊(替換后)
            SUBTOTAL_IMPORT SI = new SUBTOTAL_IMPORT();
            foreach (DataRow dr in dtblFileImp.Rows)
            {
                SI.Action = dr["ACTION"].ToString();
                SI.Old_CardType_RID = Convert.ToInt32(dr["Old_CardType_RID"]);
                SI.TYPE = dr["TYPE"].ToString();
                SI.AFFINITY = dr["AFFINITY"].ToString();
                SI.PHOTO = dr["PHOTO"].ToString();
                SI.Number = Convert.ToInt32(dr["Number"].ToString());
                SI.Date_Time = Convert.ToDateTime(DateImp);
                SI.Perso_Factory_RID = Convert.ToInt32(strFactory_RID);
                SI.MakeCardType_RID = Convert.ToInt32(MakeCardType_RID);
                SI.Import_FileName = file_name;
                SI.Is_Check = "N";
                SI.Check_Date = Convert.ToDateTime("1900-01-01");
                dao.Add<SUBTOTAL_IMPORT>(SI, "RID");
            }

            //操作日誌
            SetOprLog("11");

            InOut006BL bl1 = new InOut006BL();
            bl1.dao = dao;
            bl1.AddLog("1", file_name);

            // 事務提交
            dao.Commit();
            this.CheckWarnningSend(dtblFileImp, strFactory_RID);
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
            dao.CloseConnection();
        }

    }

    /// <summary>
    /// 檢查看本次匯入的卡種是否有不足的情況！
    /// </summary>
    /// <param name="dtImport"></param>
    private void CheckWarnningSend(DataTable dtImport, string sFactoryRid)
    {
        DataTable dtCardType = this.getCardType();
        DataTable dtFactory = this.GetFactoryList().Tables[0];

        int iNum = 0;

        DataTable dtblXuNi = dao.GetList("select CardType_RID from dbo.GROUP_CARD_TYPE a inner join CARD_GROUP b on a.Group_rid=b.rid where b.Group_Name = '虛擬卡'").Tables[0];

        foreach (DataRow dr in dtImport.Rows)
        {
            DataRow[] drCardType = dtCardType.Select("TYPE='" + dr["TYPE"].ToString() + "' and AFFINITY='"
                + dr["AFFINITY"].ToString() + "' and PHOTO='" + dr["PHOTO"].ToString() + "'");

            if (drCardType.Length == 0)
                continue;

            if (dtblXuNi.Rows.Count > 0)
            {
                if (dtblXuNi.Select("CardType_RID = '" + drCardType[0]["RID"].ToString() + "'").Length > 0)
                    continue;
            }

            DataRow[] drFactory = dtFactory.Select("RID='" + sFactoryRid + "'");


            CardTypeManager ctm = new CardTypeManager();
            iNum = ctm.getCurrentStock(Convert.ToInt32(sFactoryRid), Convert.ToInt32(drCardType[0]["RID"]), DateTime.Now.Date);

            //如果庫存小於零，則發送警訊！
            if (iNum < 0)
            {
                object[] arg = new object[2];
                arg[0] = drFactory[0]["Factory_Shortname_CN"];


                if (drCardType.Length > 0)
                {
                    arg[1] = drCardType[0]["NAME"];
                }
                else
                {
                    arg[1] = "";
                }
                Warning.SetWarning(GlobalString.WarningType.CardTypeNotEnough, arg);
            }

        }
    }

    /// <summary>
    /// 驗證匯入字段是否滿足格式
    /// </summary>
    /// <param name="strColumn"></param>
    /// <param name="num"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private string CheckFileColumn(string strColumn, int num, int count)
    {
        string strErr = "";
        string Pattern = "";
        MatchCollection Matches;
        switch (num)
        {
            case 1:
                Pattern = @"^\d{1}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + (count).ToString() + "行第" + num.ToString() + "列格式必須為1位數字;";
                }
                else
                {
                    if (strColumn == "1" || strColumn == "2" || strColumn == "3" || strColumn == "5")
                        strErr = "";
                    else
                        strErr = "第" + (count).ToString() + "行第" + num.ToString() + "列必須為1、2、3、5;";
                }
                break;
            case 2:
                Pattern = @"^\d{9}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + (count).ToString() + "行第" + num.ToString() + "列格式必須為9位數字;";
                }
                break;
            case 3:
                Pattern = @"^\d{1,5}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + (count).ToString() + "行第" + num.ToString() + "列格式必須為5位以內的數字;";
                }
                break;
            default:
                break;
        }

        return strErr;
    }

    /// <summary>
    ///刪除已匯入資料
    /// </summary>
    public int Delete(string Date_Time, string RID, string FileName)
    {
        int i = 0;        
        string strSql = DEL_SUBTOTAL;
        string strSql1 = SEL_CARDTYPESTOCK;
        string strSql2 = DEL_SUBTOTAL_REPLACE;
        try
        {
            //事務開始
            dao.OpenConnection();
            dirValues.Clear();
            //dirValues.Add("stock_date", Date_Time);

            //if (dao.GetList(strSql1, dirValues).Tables[0].Rows[0][0].ToString() != "0")
            //    return -1;

            // 批次
            if (RID != "")
            {
                dirValues.Add("rid", RID);
                strSql += " AND MakeCardType_RID in (@rid)";
                strSql2 += " AND MakeCardType_RID in (@rid)";
            }
            // 文件名
            if (FileName != "全部")
            {
                dirValues.Add("filename", FileName);
                strSql += " AND Import_FileName = @filename";
                strSql2 += " AND Import_FileName = @filename";
            }



            //dirValues.Clear();
            dirValues.Add("date_time_start", Date_Time + " 00:00:00");
            dirValues.Add("date_time_end", Date_Time + " 23:59:59");

            i = dao.ExecuteNonQuery(strSql, dirValues);
             dao.ExecuteNonQuery(strSql2, dirValues);
            //操作日誌
            SetOprLog("13");

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
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
        return i;
    }

    /// <summary>
    /// 取製卡種類RID
    /// </summary>
    /// <param name="subTotalFileName"> 小計檔檔名</param>
    /// <returns></returns>
    public String getMakeCardTypeRID(string subTotalFileName)
    {
        string MakeCardTypeRID = "";
        try
        {
            this.dirValues.Clear();
            dirValues.Add("file_name", subTotalFileName);
            DataSet dsGetFILENAME = dao.GetList(CON_SUBTOTAL_FILENAME, dirValues);
            if (dsGetFILENAME != null && dsGetFILENAME.Tables.Count > 0
                && dsGetFILENAME.Tables[0].Rows.Count > 0)
            {
                MakeCardTypeRID = dsGetFILENAME.Tables[0].Rows[0]["MakeCardType_RID"].ToString();
            }

            return MakeCardTypeRID;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
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
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// "匯入伺服器資料"綁定小計檔名時檢查是否存在和廠商簡稱
    /// </summary>
    /// <returns>true[存在]</returns>
    public bool getFilesByDownloadDay(string File_Name, string Factory_ShortName_EN)
    {
        DataSet dsGetFILENAME = null;
        DataSet dsGetFactory_ShortName_EN = null;
        try
        {
            this.dirValues.Clear();
            dirValues.Add("file_name", File_Name);
            dsGetFILENAME = dao.GetList(CON_SUBTOTAL_FILENAME, dirValues);

            if (dsGetFILENAME.Tables[0].Rows.Count == 0)
            {
                return false;
            }

            this.dirValues.Clear();
            dirValues.Add("factory_shortName_en", Factory_ShortName_EN);
            dsGetFactory_ShortName_EN = dao.GetList(CON_SUBTOTAL_PERSO_FACTORY, dirValues);

            if (dsGetFactory_ShortName_EN.Tables[0].Rows.Count == 0)
            {
                return false;
            }
        }
        catch (AlertException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return true;
    }

   
    /// <summary>
    ///200908CR 將替換后的小記檔數據轉換成替換前的小記檔數據-
    /// </summary>
    /// 
    public void AddReplaceImp()
    {
        string DelReplace = " Delete SUBTOTAL_REPLACE_IMPORT ";
        string SEL_SUBTOTAL_IMPORT_ALL = "select * from SUBTOTAL_IMPORT where Old_CardType_RID <>'0'";
        try
        {

            DataTable dtblFileImp = new DataTable();
            DataTable dtblCardType = new DataTable();
            dtblFileImp = dao.GetList(SEL_SUBTOTAL_IMPORT_ALL).Tables[0];
            dtblFileImp.Columns.Add("Change_Space_RID",Type.GetType("System.Int32"));
            dtblFileImp.Columns.Add("Replace_Space_RID", Type.GetType("System.Int32"));
            dtblCardType = dao.GetList(SEL_CARD_TYPE_ALL).Tables[0];


            if (dtblCardType.Rows.Count > 0 && dtblFileImp.Rows.Count > 0)
            {
                foreach (DataRow drowFileImp in dtblFileImp.Rows)
                {

                    DataRow[] drCardType = dtblCardType.Select("RID='" + drowFileImp["Old_CardType_RID"].ToString() + "'");
                    if (drCardType.Length < 0)
                        continue;

                    drowFileImp["TYPE"] = drCardType[0]["TYPE"];
                    drowFileImp["AFFINITY"] = drCardType[0]["AFFINITY"];
                    drowFileImp["PHOTO"] = drCardType[0]["PHOTO"];
                    drowFileImp["Change_Space_RID"] = drCardType[0]["Change_Space_RID"];
                    drowFileImp["Replace_Space_RID"] = drCardType[0]["Replace_Space_RID"];
                                
                }
            }
           

            //事務開始
            dao.OpenConnection();
            dao.ExecuteNonQuery(DelReplace);
           
            SUBTOTAL_REPLACE_IMPORT SRI = new SUBTOTAL_REPLACE_IMPORT();
            foreach (DataRow dr in dtblFileImp.Rows)
            {
                SRI.Action = dr["ACTION"].ToString();
                SRI.Old_CardType_RID = Convert.ToInt32(dr["Old_CardType_RID"]);
                SRI.TYPE = dr["TYPE"].ToString();
                SRI.AFFINITY = dr["AFFINITY"].ToString();
                SRI.PHOTO = dr["PHOTO"].ToString();
                SRI.Number = Convert.ToInt32(dr["Number"].ToString());
                SRI.Date_Time = Convert.ToDateTime(dr["Date_Time"]);
                SRI.Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"].ToString());
                SRI.MakeCardType_RID = Convert.ToInt32(dr["MakeCardType_RID"].ToString());
                SRI.Change_Space_RID = Convert.ToInt32(dr["Change_Space_RID"].ToString());
                SRI.Replace_Space_RID = Convert.ToInt32(dr["Replace_Space_RID"].ToString());
                SRI.Import_FileName = dr["Import_FileName"].ToString();
                SRI.Is_Check = dr["Is_Check"].ToString();
                SRI.Check_Date = Convert.ToDateTime(dr["Check_Date"].ToString());
                dao.Add<SUBTOTAL_REPLACE_IMPORT>(SRI, "RID");
            }
          
            // 事務提交
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
            dao.CloseConnection();
        }

    }

}

