//******************************************************************
//*  作    者：JunWang
//*  功能說明：廠商庫存異動匯入 
//*  創建日期：2008-09-09
//*  修改日期：2008-09-09 9:00
//*  修改記錄：
//*            □2008-09-09
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using CIMSClass.Business;
/// <summary>
/// InOut002BL 的摘要描述
/// </summary>
public class InOut002BL : BaseLogic
{
    #region SQL語句
    public const string SEL_FACTORY = "SELECT F.RID,F.Factory_ID,F.Factory_ShortName_CN "
                        + "FROM FACTORY AS F "
                        + "WHERE F.RST = 'A' AND F.Is_Perso = 'Y' order by RID";

    public const string SEL_FACTORY_RID = "SELECT F.RID,F.Factory_ID,F.Factory_ShortName_CN "
                        + "FROM FACTORY AS F "
                        + "WHERE F.RST = 'A' AND F.Is_Perso = 'Y' AND Factory_ID = @factory_id";

    public const string SEL_FACTORY_ShortName_CN = "SELECT Factory_ShortName_CN "
                        + "FROM FACTORY "
                        + "WHERE Factory_ID = @factory_id";

    public const string SEL_WAFER_INFO_BYCRID = " select WI.RID,WI.WAFER_NAME from WAFER_INFO WI INNER JOIN WAFER_CARDTYPE WC ON WI.RID=WC.WAFER_RID AND WC.RST='A' WHERE WI.RST='A' AND WC.CARDTYPE_RID=@CARDTYPE_RID";


    public const string SEL_CARDTYPE_STATUS = "SELECT RID,Status_Code,Status_Name "
                          + "FROM CARDTYPE_STATUS "
                          + "WHERE RST='A' ";

    public const string SEL_CARDTYPE = "SELECT TYPE,AFFINITY,PHOTO,Name "
                          + "FROM CARD_TYPE "
                          + "WHERE RST='A'";

    public const string SEL_CARDTYPE_ALL = "SELECT * "
                         + "FROM CARD_TYPE "
                         + "WHERE RST='A'";

    public const string SEL_CARDTYPE_RID = "SELECT RID "
                          + "FROM CARD_TYPE "
                          + "WHERE RST='A' AND Name = @name";

    public const string SEL_CARDTYPE_Name = "SELECT Name "
                          + "FROM CARD_TYPE "
                          + "WHERE RST='A' AND RID = @rid";

    public const string SEL_Status_Name = "SELECT Status_Name "
                          + "From CARDTYPE_STATUS  "
                          + "WHERE RST = 'A' AND RID= @rid";

    public const string SEL_FACTORY_CHANGE_IMPORT = "SELECT RID "
                          + "FROM FACTORY_CHANGE_IMPORT "
                          + "WHERE RST='A' AND Space_Short_Name = @space_short_name AND Status_RID = @status AND Date_Time= @date_time AND Perso_Factory_RID = @perso_factory_rid ";

    public const string SEL_FACTORY_CHANGE_IMPORT_ALL = "SELECT FCI.Space_Short_Name,CS.Status_Code,FCI.Perso_Factory_RID,FCI.Date_Time "
                          + "FROM FACTORY_CHANGE_IMPORT FCI LEFT JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID "
                          + "WHERE FCI.RST='A' AND FCI.Perso_Factory_RID = @FactoryRID AND "
                          + " FCI.Date_Time >= @Import_Date_Start AND FCI.Date_Time <= @Import_Date_End";


    public const string DEL_CHANGE_IMPORT = "DELETE FROM FACTORY_CHANGE_IMPORT "
                          + "WHERE RST='A' AND Is_Check='N' AND Perso_Factory_RID = @perso_factory_rid AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end AND Is_Auto_Import = 'Y'";

    public const string DEL_CHANGE_IMPORT_ALL = "DELETE FROM FACTORY_CHANGE_IMPORT "
                          + "WHERE RST='A' AND Is_Check='N' AND Perso_Factory_RID = @perso_factory_rid AND Date_Time >= @check_time_start AND Date_Time <= @check_time_end";

    public const string SEL_CHANGE_IMPORT = "SELECT Space_Short_Name,Status_RID,Number,CardType_Group_RID "
                          + "FROM FACTORY_CHANGE_IMPORT LEFT JOIN CARD_TYPE ON CARD_TYPE.RID=FACTORY_CHANGE_IMPORT.Space_Short_RID "
                          + "WHERE RST='A' AND Is_Check='N' AND Perso_Factory_RID = @perso_factory_rid AND Date_Time = @date_time ";

    public const string SEL_CHANGE_IMPORT_CARDGROUP = "SELECT FCI.RID,FCI.Perso_Factory_RID,FCI.TYPE,FCI.AFFINITY,FCI.PHOTO," +
                         "FCI.Date_Time,CT.RID as Space_Short_RID,FCI.Status_RID,FCI.Number,P.Param_Name,CG.Group_Name,"+
                         "CG.RID AS CGRID,Space_Short_Name,P.Param_Code,FCI.Is_Auto_Import,CS.Status_Name " +
                         "FROM FACTORY_CHANGE_IMPORT FCI LEFT JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO " +
                        "INNER JOIN GROUP_CARD_TYPE GCT ON CT.RID = GCT.CardType_RID "+
                        "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID "+
                        "INNER JOIN PARAM P ON P.RST = 'A' AND CG.Param_Code = P.Param_Code AND P.Param_Code = '" + GlobalString.Parameter.Type +
                        "' INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID " +
                        "WHERE FCI.RST='A' AND FCI.Perso_Factory_RID = @perso_factory_rid AND FCI.Date_Time >= @date_time_start AND FCI.Date_Time <= @date_time_end ";

    public const string SEL_FACTORY_NAME = "SELECT Factory_ShortName_CN "
                         + "From Factory "
                         + "WHERE RID = @rid";

    public const string SEL_CARDTYPE_TYPE_AFFINITY_PHOTO = "SELECT [TYPE],[AFFINITY],[PHOTO] "
                         + "FROM CARD_TYPE "
                         + "WHERE RST='A' AND RID = @rid";

    public const string SEL_PARAM_USE = "SELECT Param_Code,Param_Name "+
                        "FROM PARAM WHERE RST = 'A' AND ParamType_Code = '" + GlobalString.ParameterType.Use + "' "+
                        "order by Param_Code";

    public const string SEL_CARDGROUP1 = "SELECT RID,Group_Name FROM CARD_GROUP WHERE RST = 'A' AND Param_Code = @param_code order by RID";

    public const string SEL_CARDTYPE_1 = "SELECT CT.RID,(CT.TYPE+CT.AFFINITY+CT.PHOTO+Name) as Name FROM CARD_TYPE CT "+
                        "INNER JOIN GROUP_CARD_TYPE GCT ON CT.RID = GCT.CardType_RID AND GCT.RST = 'A' "+
                        "WHERE CT.RST = 'A' AND GCT.Group_RID = @group_rid";

    public const string CON_CHECK_DATE = "SELECT count(*) from cardtype_stocks where rst = 'A' and Stock_Date = @CheckDate";

    public const string DEL_FACTORY_CHANGE_IMPORT = "DELETE FROM FACTORY_CHANGE_IMPORT WHERE RID = @RID";

    public const string SEL_BATCH_MANAGE = "SELECT COUNT(*) FROM BATCH_MANAGE WHERE (RID = 1 OR RID = 5 OR RID = 4) AND Status = 'Y'";
    public  string UPDATE_BATCH_MANAGE_START = "UPDATE BATCH_MANAGE SET Status = 'Y',RUU='InOut002BL.cs',RUT='"
                                        + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 1 OR RID = 5 OR RID = 4)";
    public  string UPDATE_BATCH_MANAGE_END = "UPDATE BATCH_MANAGE SET Status = 'N',RUU='InOut002BL.cs',RUT='"
                                        + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 1 OR RID = 5 OR RID = 4)";

    public const string CON_WORKDATE = "SELECT COUNT(*) FROM WORK_DATE WHERE Convert(varchar(10),Date_Time,111) = @date_time AND RST = 'A' AND Is_WorkDay = 'Y'";
    public const string CON_SURPLUS = "SELECT COUNT(*) FROM CARDTYPE_STOCKS WHERE RST='A' AND Stock_Date>=@date_time ";

    public const string SEL_FACTORY_CHANGE_IMPORT_WHERE = "SELECT * "
                          + "FROM FACTORY_CHANGE_IMPORT ";
    // 物料消耗報警
    public const string SEL_MADE_CARD_WARNNING = "SELECT FCI.Perso_Factory_RID,FCI.CareType_RID as RID,FCI.Status_RID,SUM(FCI.Number) AS Number "
                        + "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID "
                        + "WHERE FCI.RST = 'A' AND FCI.Perso_Factory_RID = @Perso_Factory_RID AND FCI.Date_Time>@From_Date_Time AND FCI.Date_Time<=@End_Date_Time "
                        + "GROUP BY FCI.Perso_Factory_RID,FCI.CareType_RID,FCI.Status_RID";
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
                         // "INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID " +
                        //200909IR耗用量=數量*（1+耗損率） add  by 楊昆 2009/09/01 start
                        "LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID " +
                        "LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID " +
                        //200909IR耗用量=數量*（1+耗損率） add  by 楊昆 2009/09/01 END
                        "WHERE TMC.Perso_Factory_RID = @perso_factory_rid";
    public const string SEL_MATERIAL_BY_TEMP_MADE_CARD_DM = " SELECT DI.Serial_Number DI_Number,A.Perso_Factory_RID,A.Number " +
                        "FROM TEMP_MADE_CARD A " +
                        "INNER JOIN DM_CARDTYPE DCT ON DCT.RST = 'A' AND A.CardType_RID = DCT.CardType_RID " +
                        "INNER JOIN DMTYPE_INFO DI ON DI.RST = 'A' AND DCT.DM_RID = DI.RID "+
                        "WHERE A.Perso_Factory_RID = @perso_factory_rid";
    public const string SEL_LAST_WORK_DATE = "SELECT TOP 1 Date_Time " +
                        "FROM WORK_DATE " +
                        "WHERE Date_Time < @date_time AND Is_WorkDay='Y' " +
                        "ORDER BY Date_Time DESC";
    public const string SEL_MATERIEL_STOCKS_MANAGER = "SELECT Top 1 MSM.Stock_Date,MSM.Perso_Factory_RID,MSM.Serial_Number,MSM.Number," +
                        "CASE SUBSTRING(MSM.Serial_Number,1,1) WHEN 'A' THEN EI.NAME WHEN 'B' THEN CE.NAME WHEN 'C' THEN DI.NAME END AS NAME " +
                        "FROM MATERIEL_STOCKS_MANAGE MSM LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MSM.Serial_Number = EI.Serial_Number " +
                            "LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MSM.Serial_Number = CE.Serial_Number " +
                            "LEFT JOIN DMTYPE_INFO DI ON DI.RST = 'A' AND MSM.Serial_Number = DI.Serial_Number " +
                        "WHERE Type = '4' AND MSM.Perso_Factory_RID = @perso_factory_rid AND MSM.Serial_Number = @serial_number " +
                      "ORDER BY Stock_Date Desc";
    public const string SEL_MATERIEL_USED = "SELECT SUM(Number) as Number FROM MATERIEL_STOCKS_USED " +
                        "WHERE RST = 'A' AND Perso_Factory_RID = @perso_factory_rid AND Serial_Number = @serial_number " +
                        " AND Stock_Date>@from_stock_date AND Stock_Date<=@end_stock_date ";

    public const string SEL_LAST_SURPLUS_DAY = "SELECT TOP 1 Stock_Date FROM CARDTYPE_STOCKS WHERE RST = 'A' ORDER BY  Stock_Date DESC";
    #endregion

    Depository010BL bl = new Depository010BL();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public InOut002BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    /// <summary>
    /// 將廠商異動匯入標設置為FALSE
    /// </summary>
    /// <returns></returns>
    public void ImportFactoryChangeEnd()
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
    /// 檢查日期是否正確，必須時營業日期而且該日期後不能有日結日期
    /// </summary>
    /// <param name="dtImportDateTime"></param>
    /// <returns></returns>
    public bool CheckImportDateTime(DateTime dtImportDateTime)
    {
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("date_time", dtImportDateTime.ToString("yyyy/MM/dd"));
            DataSet dsCheck = dao.GetList(CON_WORKDATE, dirValues);
            if (null != dsCheck && dsCheck.Tables.Count > 0 &&  dsCheck.Tables[0].Rows.Count>0)
            {
                if (Convert.ToInt32(dsCheck.Tables[0].Rows[0][0]) == 0)
                {
                    throw new AlertException("日期不是營業日。");
                }
            }
            dsCheck = dao.GetList(CON_SURPLUS, dirValues);
            if (null != dsCheck && dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dsCheck.Tables[0].Rows[0][0]) > 0)
                {
                    throw new AlertException("日期之後有日結日期，不能輸入。");
                }
            }
            return true;
        }
        catch (AlertException ale)
        {
            throw new Exception(ale.Message);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 檢查廠商異動匯入刪除是否已經被開起。如果已經開起，返回FALSE
    ///                                     如果沒有開起，開起，并返回TRUE
    /// </summary>
    /// <returns></returns>
    public bool ImportFactoryChangeStart()
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
    /// 獲得Perso廠商RID
    /// </summary>
    /// <returns>DataSet[Perso廠商]</returns>
    public string GetFactory_RID(string Factory_ID)
    {
        DataSet dstFactory_RID = null;
        string RID = "";
        try
        {
            this.dirValues.Clear();
            dirValues.Add("factory_id", Factory_ID);
            dstFactory_RID = dao.GetList(SEL_FACTORY_RID, dirValues);
            if (dstFactory_RID.Tables[0].Rows.Count != 0)
            {
                RID = dstFactory_RID.Tables[0].Rows[0]["RID"].ToString();
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return RID;
    }

    /// <summary>
    /// 導入資料并處理
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="dtblFileImp"></param>
    /// <returns></returns>
    public string Import(string strPath, 
                    DataTable dtblFileImp, 
                    string Date, 
                    string FactoryRID)
    {
        StreamReader sr = null;

        //判斷狀況代碼資料集
        DataTable dtCardTypeStatus = null;
        //判斷卡種有效期和是否停用資料集
        DataSet dsCARD_TYPE_ALL = null;
        //異動信息資料集
        DataSet dsFACTORY_CHANGE_IMPORT_ALL = null;

        try
        {
            //取出所有卡種卡況字段的信息
            dtCardTypeStatus = CheckFileStatus();
            //取出所有有效期和是否停用字段信息
            dsCARD_TYPE_ALL = getCARD_TYPE_ALL();
            //取出所有廠商異動信息表字段信息判斷是否重復匯入
            dsFACTORY_CHANGE_IMPORT_ALL = getFACTORY_CHANGE_IMPORT_ALL(FactoryRID, Date);
            //狀況名稱
            string strStatus_Name = "";
            int intStatus_RID = 0;

            sr = new StreamReader(strPath, System.Text.Encoding.Default);
            string[] strLine;
            string[] strLineDetail;
            string strReadLine = "";
            int count = 1;
            StringBuilder sbErr = new StringBuilder("");
            StringBuilder sbErrFactory = new StringBuilder("");

            while ((strReadLine = sr.ReadLine()) != null)
            {
                if (count == 1)
                {
                    #region 匯入日期和廠商代號檢查
                    // 列數檢查
                    if (strReadLine.Length != 13)
                    {
                        sbErrFactory.Append("匯入文件缺少匯入日期或Perso厰商，文件無法匯入！\\n");
                    }
                    else
                    {
                        strLine = new string[2];
                        strLine[0] = strReadLine.Substring(0, 8);// 匯入日期
                        strLine[1] = strReadLine.Substring(8, 5);// 廠商代碼

                        // 列內容格式檢查
                        for (int i = 0; i < strLine.Length; i++)
                        {
                            if (StringUtil.IsEmpty(strLine[i]))
                            {
                                if (i == 0)
                                {
                                    sbErrFactory.Append("第" + count.ToString() + "行匯入文件的匯入日期不能為空！\\n");
                                }else if (i == 1)
                                {
                                    sbErrFactory.Append("第" + count.ToString() + "行匯入文件的Perso厰商代號不能為空！\\n");
                                }
                            }
                            else
                            {
                                int num = i + 1;
                                sbErrFactory.Append((string)CheckFileOneColumn(strLine[i], num, count));
                            }
                        }

                        // 匯入日期和Perso廠代碼檢查
                        if (strLine[0] != Date.Replace("/", ""))
                        {
                            sbErrFactory.Append("頁面輸入的匯入日期與匯入文件第1行中的匯入日期不符；\\n");
                        }
                        // 廠商檢測
                        string Factory_RID = GetFactory_RID(strLine[1]);
                        if (Factory_RID == "" || Factory_RID != FactoryRID)
                        {
                            sbErrFactory.Append("頁面輸入的Perso廠商與匯入文件第1行中的廠商代碼不符；\\n");
                        }
                    }
                    #endregion 匯入日期和廠商代號檢查
                }
                else
                {
                    #region 行內容檢查
                    sbErr = new StringBuilder("");
                    
                    if (StringUtil.GetByteLength(strReadLine) != 50)
                    {
                        sbErr.Append("第" + count.ToString() + "行匯入文件格式不正確;\\n");
                        sbErrFactory.Append(sbErr);
                    }
                    else
                    {
                        strLine = new string[3];
                        Depository003BL bl003 = new Depository003BL();
                        int nextBegin = 0;
                        strLine[0] = bl003.GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();
                        strLine[1] = bl003.GetSubstringByByte(strReadLine, nextBegin, 30, out nextBegin).Trim();
                        strLine[2] = bl003.GetSubstringByByte(strReadLine, nextBegin, 11, out nextBegin).Trim();


                        strLineDetail = new string[6];
                        if (strLine[0].Length != 9 || strLine[2].Length != 11)
                        {
                            sbErr.Append("第" + count.ToString() + "行匯入文件格式不正確;\\n");
                        }
                        else
                        {
                            strLineDetail[0] = strLine[0].Substring(0, 3);// Type
                            strLineDetail[1] = strLine[0].Substring(3, 4);// Affinity
                            strLineDetail[2] = strLine[0].Substring(7, 2);// Photo
                            strLineDetail[3] = strLine[1];// Name
                            strLineDetail[4] = strLine[2].Substring(0, 2);// 狀況代碼
                            strLineDetail[5] = strLine[2].Substring(2, 9);// 數量
                            for (int i = 0; i < strLineDetail.Length; i++)
                            {
                                int num = i + 1;
                                if (StringUtil.IsEmpty(strLineDetail[i]))
                                    sbErr.Append("第" + count.ToString() + "行第" + num.ToString() + "列為空;\\n");
                                else
                                    sbErr.Append((string)CheckFileColumn(strLineDetail[i], num, count));
                            }

                            if (sbErr.Length == 0)
                            {
                                // 檢查狀況代碼是否存在
                                bool blnFound = false;

                                strStatus_Name = "";
                                for (int intRow = 0; intRow < dtCardTypeStatus.Rows.Count; intRow++)
                                {
                                    if (Convert.ToInt32(dtCardTypeStatus.Rows[intRow]["Status_Code"]) ==
                                        Convert.ToInt32(strLineDetail[4]))
                                    {
                                        intStatus_RID = Convert.ToInt32(dtCardTypeStatus.Rows[intRow]["RID"].ToString());
                                        strStatus_Name = dtCardTypeStatus.Rows[intRow]["Status_Name"].ToString();
                                        blnFound = true;
                                        break;
                                    }
                                }
                                if (!blnFound)
                                {
                                    sbErr.Append("第" + count.ToString() + "行第5列的狀況代碼不存在;\\n");
                                }

                                // 檢查卡種資訊
                                DataRow[] drs = dsCARD_TYPE_ALL.Tables[0].Select("Type = " + strLineDetail[0] +
                                                                                " AND Affinity = " + strLineDetail[1] +
                                                                                " AND Photo = " + strLineDetail[2]);
                                if (drs.Length == 0)
                                {
                                    // 存在檢查
                                    sbErr.Append("第" + count.ToString() + "行的卡種不存在;\\n");
                                }
                                else
                                {
                                    if (drs[0]["Is_Using"].ToString() != "Y")
                                    {
                                        // 是否停用檢查
                                        sbErr.Append("第" + count.ToString() + "行的卡種已經停用;\\n");
                                    }
                                    if (Convert.ToDateTime(drs[0]["Begin_Time"].ToString()) > Convert.ToDateTime(Date) ||
                                        (Convert.ToDateTime(drs[0]["End_Time"].ToString()) < Convert.ToDateTime(Date) &&
                                            Convert.ToDateTime(drs[0]["End_Time"].ToString()).ToString("yyyy-MM-dd") != "1900-01-01"))
                                    {
                                        // 卡種的有效期檢查
                                        sbErr.Append("第" + count.ToString() + "行的卡種不在有效期內;\\n");
                                    }
                                }

                                // 檢查是否重復匯入檢查
                                DataRow[] drsFCIA = dsFACTORY_CHANGE_IMPORT_ALL.Tables[0].Select("Space_Short_Name = '" + strLineDetail[3] +
                                                                                "' AND Status_Code = " + strLineDetail[4]);
                                if (drsFCIA.Length > 0)
                                {
                                    // 卡種的有效期檢查
                                    sbErr.Append("第" + count.ToString() + "行的廠商庫存異動資訊已經存在;\\n");
                                }

                                if (blnFound)
                                {
                                    if (dtblFileImp.Select("TYPE='" + strLineDetail[0] + "' and AFFINITY='" + strLineDetail[1] + "' and PHOTO='" + strLineDetail[2] + "' and Status_RID='" + intStatus_RID.ToString() + "'").Length > 0)
                                        sbErr.Append("第" + count.ToString() + "行的廠商庫存異動資訊不能重複匯入;\\n");
                                }
                            }
                        }

                        // 檢查是否有錯誤
                        if (sbErr.Length > 0)
                        {
                            // 有錯誤
                            sbErrFactory.Append(sbErr);
                        }
                        else
                        {
                            // 沒有錯誤
                            DataRow dr = dtblFileImp.NewRow();//作為插入資料庫
                            dr["TYPE"] = strLineDetail[0];
                            dr["AFFINITY"] = strLineDetail[1];
                            dr["PHOTO"] = strLineDetail[2];
                            dr["Name"] = strLineDetail[3];
                            dr["Status_RID"] = intStatus_RID;
                            dr["Status_Name"] = strStatus_Name;
                            dr["Number"] = Convert.ToInt32(strLineDetail[5]);



                            dtblFileImp.Rows.Add(dr);
                        }
                    }
                    #endregion 行內容檢查
                }
                count++;
            }

            // 如果沒有錯誤添加廠商庫存異動匯入記錄
            if (sbErrFactory.Length == 0)
            {
                dao.OpenConnection();
                FACTORY_CHANGE_IMPORT fciModel = new FACTORY_CHANGE_IMPORT();
                foreach (DataRow drowFileImp in dtblFileImp.Rows)
                {
                    fciModel.TYPE = drowFileImp["TYPE"].ToString();
                    fciModel.AFFINITY = drowFileImp["AFFINITY"].ToString();
                    fciModel.PHOTO = drowFileImp["PHOTO"].ToString();
                    fciModel.Space_Short_Name = drowFileImp["Name"].ToString();
                    fciModel.Status_RID = Convert.ToInt32(drowFileImp["Status_RID"]);
                    fciModel.Number = Convert.ToInt32(drowFileImp["Number"]);
                    fciModel.Date_Time = Convert.ToDateTime(Date);
                    fciModel.Perso_Factory_RID = Convert.ToInt32(FactoryRID);
                    fciModel.Is_Auto_Import = "Y";
                    dao.Add<FACTORY_CHANGE_IMPORT>(fciModel, "RID");
                }

                //操作日誌
                SetOprLog("11");

                InOut006BL bl1 = new InOut006BL();
                bl1.dao = dao;
                bl1.AddLog("2", strPath.Substring(strPath.LastIndexOf('\\')+1));

                dao.Commit();

                this.SendWarningPerso(dtblFileImp, FactoryRID);
                //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 DEL BY 楊昆 2009/08/31 start
                //物料消耗改用替換前的版面計算，匯入替換后版面的數據時不做檢核
                //// 將廠商異動訊息轉換成物料耗用訊息，并根據物料庫存及耗用訊息，判斷是否需要報警。
                //Material_Used_Warnning(FactoryRID, Convert.ToDateTime(Date));
                //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 DEL BY 楊昆 2009/08/31 end

            }

            return sbErrFactory.ToString();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            if (sr != null)
                sr.Close();
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 根據Perso廠商的記錄計算是否有不足的卡種！
    /// </summary>
    /// <param name="dtImport"></param>
    private void SendWarningPerso(DataTable dtImport, string sFactoryRid)
    {
        try
        {
            DataTable dtCardType = this.getCARD_TYPE_ALL().Tables[0];
            DataTable dtFactory = this.GetFactoryList().Tables[0];
            int iNum = 0;

            DataTable dtCard = new DataTable();
            dtCard.Columns.Add("card");
            dtCard.Columns.Add("factory");

            DataTable dtblXuNi = dao.GetList("select CardType_RID from dbo.GROUP_CARD_TYPE a inner join CARD_GROUP b on a.Group_rid=b.rid where b.Group_Name = '虛擬卡'").Tables[0];

            foreach (DataRow dr in dtImport.Rows)
            {
                DataRow[] drCardType = dtCardType.Select("TYPE='" + dr["TYPE"].ToString() + "' and AFFINITY='"
                    + dr["AFFINITY"].ToString() + "' and PHOTO='" + dr["PHOTO"].ToString() + "'");
                if (drCardType.Length < 0)
                    continue;

                int CardRID = 0;

                if (dr["Status_RID"].ToString() == "4")
                {
                    if (drCardType[0]["Change_Space_RID"].ToString() != "0")
                        CardRID = int.Parse(drCardType[0]["Change_Space_RID"].ToString());
                    else
                    {
                        if (drCardType[0]["Replace_Space_RID"].ToString() != "0")
                            CardRID = int.Parse(drCardType[0]["Replace_Space_RID"].ToString());
                        else
                            CardRID = int.Parse(drCardType[0]["RID"].ToString());
                    }
                }
                else if (dr["Status_RID"].ToString().ToUpper() == "1" || dr["Status_RID"].ToString().ToUpper() == "2" || dr["Status_RID"].ToString().ToUpper() == "3")
                {
                    if (drCardType[0]["Replace_Space_RID"].ToString() != "0")
                        CardRID = int.Parse(drCardType[0]["Replace_Space_RID"].ToString());
                    else
                        CardRID = int.Parse(drCardType[0]["RID"].ToString());
                }
                else
                {
                    CardRID = int.Parse(drCardType[0]["RID"].ToString());
                }


                DataRow[] drFactory = dtFactory.Select("RID='" + sFactoryRid + "'");

                if (dtCard.Select("card='" + CardRID.ToString() + "' and factory='" + sFactoryRid.ToString() + "'").Length > 0)
                    continue;

                if (dtblXuNi.Rows.Count > 0)
                {
                    if (dtblXuNi.Select("CardType_RID = '" + CardRID.ToString() + "'").Length > 0)
                        continue;
                }

                DataRow drcard = dtCard.NewRow();
                drcard[0] = CardRID.ToString();
                drcard[1] = sFactoryRid.ToString();
                dtCard.Rows.Add(drcard);

                CardTypeManager ctm = new CardTypeManager();
                iNum = ctm.getCurrentStockPerso(Convert.ToInt32(sFactoryRid), CardRID, DateTime.Now.Date.AddDays(1).AddSeconds(-1));

                //如果庫存小於零，則發送警訊！
                if (iNum < 0)
                {
                    object[] arg = new object[2];
                    arg[0] = drFactory[0]["Factory_Shortname_CN"];

                    DataRow[] drCardType1 = dtCardType.Select("RID=" + CardRID.ToString());


                    if (drCardType1.Length > 0)
                    {
                        arg[1] = drCardType1[0]["NAME"].ToString();
                    }
                    else
                    {
                        arg[1] = "";
                    }
                    Warning.SetWarning(GlobalString.WarningType.PersoChangeCardInMiss, arg);
                }

            }
        }
        catch
        {
           
        }
    }

    /// <summary>
    ///將卡面簡稱替換成Space_Short_RID為會寫到異動表準備
    /// </summary>
    public string ReutrnSpace_Short_RID(string strLine)
    {
        DataSet dsCARD_TYPE_RID = null;
        string Space_Short_RID = "";
        dirValues.Clear();
        try
        {
            //查詢CARDTYPE表卡面簡稱(根據NAME)返回卡面簡稱"唯一對應"的rid
            dirValues.Add("name", strLine);
            dsCARD_TYPE_RID = dao.GetList(SEL_CARDTYPE_RID, dirValues);
            if (dsCARD_TYPE_RID.Tables[0] != null)
            {
                if (dsCARD_TYPE_RID.Tables[0].Rows.Count != 0)
                {
                    Space_Short_RID = dsCARD_TYPE_RID.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    Space_Short_RID = strLine;
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return Space_Short_RID;
    }

    /// <summary>
    ///將Space_Short_RID替換成卡面簡稱為會寫到異動表準備
    /// </summary>
    public string ReutrnCard_Type_Name(string strRid)
    {
        DataSet dsCARD_TYPE_Name = null;
        string Card_Type_Name = "";
        dirValues.Clear();
        try
        {
            //查詢CARDTYPE表卡面簡稱(根據NAME)返回卡面簡稱"唯一對應"的rid
            dirValues.Add("rid", strRid);
            dsCARD_TYPE_Name = dao.GetList(SEL_CARDTYPE_Name, dirValues);
            if (dsCARD_TYPE_Name.Tables[0] != null)
            {
                if (dsCARD_TYPE_Name.Tables[0].Rows.Count != 0)
                {
                    Card_Type_Name = dsCARD_TYPE_Name.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    Card_Type_Name = strRid;
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return Card_Type_Name;
    }


    /// <summary>
    ///將卡況Param_Code替換成Param_Name
    /// </summary>
    public string ReutrnStatus_Name(string Status_Name)
    {
        DataSet dsStatus_Name = null;
        string Name = "";
        dirValues.Clear();
        try
        {

            dirValues.Add("rid", Status_Name);
            dsStatus_Name = dao.GetList(SEL_Status_Name, dirValues);
            if (dsStatus_Name.Tables[0] != null)
            {
                if (dsStatus_Name.Tables[0].Rows.Count != 0)
                {
                    Name = dsStatus_Name.Tables[0].Rows[0]["Status_Name"].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return Name;
    }


    /// <summary>
    ///根據廠商id返回廠商name
    /// </summary>
    public string ReutrnFactory_ShortName_CN(string Factory_ID)
    {
        DataSet dsName = null;
        string Factory_ShortName_CN = "";
        dirValues.Clear();
        try
        {

            dirValues.Add("factory_id", Factory_ID);
            dsName = dao.GetList(SEL_FACTORY_ShortName_CN, dirValues);
            if (dsName.Tables[0] != null)
            {
                if (dsName.Tables[0].Rows.Count != 0)
                {
                    Factory_ShortName_CN = dsName.Tables[0].Rows[0][0].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return Factory_ShortName_CN;
    }

    /// <summary>
    ///檢查狀況代碼字段對應的參數在【系统参数檔】中是否存在，并正常使用
    /// </summary>
    private DataTable CheckFileStatus()
    {
        DataTable dtCARDTYPE_STATUS = new DataTable();
        try
        {
            this.dirValues.Clear();
            DataSet dsCARDTYPE_STATUS = dao.GetList(SEL_CARDTYPE_STATUS);
            if (null != dsCARDTYPE_STATUS &&
                dsCARDTYPE_STATUS.Tables.Count > 0 &&
                dsCARDTYPE_STATUS.Tables[0].Rows.Count > 0)
            {
                dtCARDTYPE_STATUS = dsCARDTYPE_STATUS.Tables[0];
            }
            return dtCARDTYPE_STATUS;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取所有卡種訊息
    /// </summary>
    private DataSet getCARD_TYPE_ALL()
    {
        DataSet dsCARD_TYPE = null;
        dirValues.Clear();
        try
        {
            dsCARD_TYPE = dao.GetList(SEL_CARDTYPE_ALL);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dsCARD_TYPE;
    }

    /// <summary>
    /// 取系統中所有的庫存異動訊息。以備查詢匯入的廠商庫存異動訊息是否有重復匯入。
    /// </summary>
    ///<returns></returns>
    private DataSet getFACTORY_CHANGE_IMPORT_ALL(string FactoryRID,
                            string Import_Date)
    {
        DataSet dsFACTORY_CHANGE_IMPORT = null;
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("FactoryRID", FactoryRID);
            this.dirValues.Add("Import_Date_Start", Import_Date + " 00:00:00");
            this.dirValues.Add("Import_Date_End", Import_Date + " 23:59:59");
            dsFACTORY_CHANGE_IMPORT = dao.GetList(SEL_FACTORY_CHANGE_IMPORT_ALL,this.dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dsFACTORY_CHANGE_IMPORT;
    }


    /// <summary>
    /// 驗證匯入字段第一行是否滿足格式
    /// </summary>
    /// <param name="strColumn"></param>
    /// <param name="num"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private string CheckFileOneColumn(string strColumn, int num, int count)
    {
        string strErr = "";
        string Pattern = "";
        MatchCollection Matches;
        switch (num)
        {
            case 1:
                if (strColumn.Length == 8)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(strColumn.Substring(0, 4) + "/" + strColumn.Substring(4, 2) + "/" + strColumn.Substring(6, 2));
                    }
                    catch
                    {
                        strErr = "第" + count.ToString() + "行日期格式不正確；\\n";
                    }
                }
                else
                {
                    strErr = "第" + count.ToString() + "行日期格式不正確；\\n";
                }
                break;
            case 2:
                Pattern = @"^\d{5}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行匯入文件的Perso厰商代號必須為5位數字；\\n";
                }
                break;
            default:
                break;
        }

        return strErr;
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
                Pattern = @"^\d{3}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為3位數字;\\n";
                }
                break;
            case 2:
                Pattern = @"^\d{4}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為4位數字;\\n";
                }
                break;
            case 3:
                Pattern = @"^\d{2}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為2位數字;\\n";
                }
                break;
            case 4:
                if (strColumn.Length == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤,匯入文件的卡種必須為30位;\\n";
                }
                break;
            case 5:
                Pattern = @"^\d{2}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為2位數字;\\n";
                }
                break;
            case 6:
                Pattern = @"^\d{9}|[-]\d{8}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為9位數字;\\n";
                }
                break;
        }

        return strErr;
    }

    /// <summary>
    /// 刪除廠商庫存異動匯入資訊
    /// </summary>
    /// <param name="Input">刪除條件</param>
    public int Delete(Dictionary<string, object> Input)
    {
        int i = 0;
        DataSet dsCheck_Date = null;
        try
        {
            //事務開始
            dao.OpenConnection();
            dirValues.Clear();
            dirValues.Add("check_date_start", ((DateTime)Input["date_time"]).ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("check_date_end", ((DateTime)Input["date_time"]).ToString("yyyy/MM/dd 23:59:59"));
            //if (isCheckDate(Convert.ToDateTime(Input["date_time"])))
            //{
            //    throw new AlertException("匯入日期已日結，無法刪除！");
            //}
            // Perso廠RID
            dirValues.Add("perso_factory_rid", Input["perso_factory_rid"]);
            // 刪除廠商庫存異動匯入資訊
            i = dao.ExecuteNonQuery(DEL_CHANGE_IMPORT, dirValues);

            //操作日誌
            SetOprLog("13");

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
    /// 查詢廠商庫存異動匯入
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataTable List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "RID" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_CHANGE_IMPORT_CARDGROUP);

        // 查詢廠商庫存異動資訊，包含自動匯入和手動輸入
        DataTable dtFactory_Change_Import = null;
        
        try
        {
            dirValues.Clear();
            dirValues.Add("date_time_start", Convert.ToDateTime(searchInput["date_time"]).ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("date_time_end", Convert.ToDateTime(searchInput["date_time"]).ToString("yyyy/MM/dd 23:59:59"));
            dirValues.Add("perso_factory_rid", searchInput["perso_factory_rid"].ToString().Trim());
            dtFactory_Change_Import = dao.GetList(stbCommand.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount).Tables[0];

            dtFactory_Change_Import.Columns.Add("IsSurplused", Type.GetType("System.String"));
            if (isCheckDate(Convert.ToDateTime(searchInput["date_time"])))
            {
                for (int intRowIndex = 0; intRowIndex < dtFactory_Change_Import.Rows.Count; intRowIndex++)
                {
                    dtFactory_Change_Import.Rows[intRowIndex]["IsSurplused"] = "yes";
                }
            }
            else {
                for (int intRowIndex = 0; intRowIndex < dtFactory_Change_Import.Rows.Count; intRowIndex++)
                {
                    dtFactory_Change_Import.Rows[intRowIndex]["IsSurplused"] = "no";
                }
            }

            rowCount = intRowCount;
        }
        catch (AlertException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return dtFactory_Change_Import;
    }

    /// <summary>
    /// 獲取狀況綁定到drop
    /// </summary>
    /// <returns>DataSet</returns>
    public DataSet getCardTypeStatus()
    {
        DataSet dtsparam_ChangeStatus = null;
        dirValues.Clear();
        try
        {
            dtsparam_ChangeStatus = dao.GetList(SEL_CARDTYPE_STATUS);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dtsparam_ChangeStatus;
    }


    /// <summary>
    /// 取廠商名稱
    /// </summary>
    /// <returns>DataSet</returns>
    public string getFactoryName(string strRID)
    {
        DataSet dsFactory = null;
        string FactoryName = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("rid", strRID);
            dsFactory = dao.GetList(SEL_FACTORY_NAME,this.dirValues);
            if (null != dsFactory &&
                dsFactory.Tables.Count > 0 &&
                dsFactory.Tables[0].Rows.Count > 0)
            {
                FactoryName = dsFactory.Tables[0].Rows[0]["Factory_ShortName_CN"].ToString();
            }
            return FactoryName;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 廠商庫存異動匯入增加
    /// </summary>
    /// <param name="strChangeReason">異動原因</param>
    public void Update(int rid,Dictionary<string, object> searchInput)
    {
        //資料實體
        //FACTORY_CHANGE_IMPORT fciModel = new FACTORY_CHANGE_IMPORT();

        try
        {
            //事務開始
            dao.OpenConnection();
            FACTORY_CHANGE_IMPORT fciModel = dao.GetModel<FACTORY_CHANGE_IMPORT, int>("RID", rid);

            fciModel.Date_Time = Convert.ToDateTime(searchInput["Date_Time"]);
            fciModel.Perso_Factory_RID = Convert.ToInt32(searchInput["Perso_Factory_RID"]);
            
            fciModel.Status_RID = Convert.ToInt32(searchInput["Status_RID"]);
            fciModel.TYPE = searchInput["TYPE"].ToString();
            fciModel.AFFINITY = searchInput["AFFINITY"].ToString();
            fciModel.PHOTO = searchInput["PHOTO"].ToString();
            fciModel.Number = Convert.ToInt32(searchInput["Number"]);
            fciModel.Space_Short_Name = searchInput["Space_Short_Name"].ToString();
            

            //操作日誌
            SetOprLog("3");
            dao.Update<FACTORY_CHANGE_IMPORT>(fciModel, "RID");
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
    /// 新增人工異動匯入
    /// </summary>    
    /// <param name="searchInput">人工異動匯入信息</param>
    public void SaveFACTORY_CHANGE_IMPORT(Dictionary<string, object> searchInput)
    {
        try
        {
            FACTORY_CHANGE_IMPORT fciModel = new FACTORY_CHANGE_IMPORT();
            fciModel.Date_Time =  Convert.ToDateTime(searchInput["Date_Time"]);
            fciModel.Perso_Factory_RID = Convert.ToInt32(searchInput["Perso_Factory_RID"]);
            fciModel.Is_Check = searchInput["Is_Check"].ToString();
            fciModel.Status_RID = Convert.ToInt32(searchInput["Status_RID"]);
            fciModel.TYPE = searchInput["TYPE"].ToString();
            fciModel.AFFINITY = searchInput["AFFINITY"].ToString();
            fciModel.PHOTO = searchInput["PHOTO"].ToString();
            fciModel.Number = Convert.ToInt32(searchInput["Number"]);
            fciModel.Space_Short_Name = searchInput["Space_Short_Name"].ToString();
            fciModel.Check_Date = Convert.ToDateTime(searchInput["Check_Date"]);
            fciModel.Is_Auto_Import = searchInput["Is_Auto_Import"].ToString();
            SetOprLog("2");
            
            dao.Add<FACTORY_CHANGE_IMPORT>(fciModel,"RID");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("新增資料失敗！");
        }
    }

    /// <summary>
    /// 獲取用途
    /// </summary>
    /// <returns></returns>
    public DataSet GetPurpose()
    {
        DataSet dstPurpose = null;
        try
        {
            dstPurpose = dao.GetList(SEL_PARAM_USE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstPurpose;
    }

    /// <summary>
    /// 獲取群組
    /// </summary>
    /// <returns></returns>
    public DataTable GetGroupByPurposeId(string strParam_Code)
    {
        DataSet dstGroupByPurposeId = null;
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("param_code", strParam_Code);
            dstGroupByPurposeId = dao.GetList(SEL_CARDGROUP1,this.dirValues);
            if (dstGroupByPurposeId != null &&
               dstGroupByPurposeId.Tables.Count > 0 &&
               dstGroupByPurposeId.Tables[0].Rows.Count > 0)
            {
                return dstGroupByPurposeId.Tables[0];
            }else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 獲取卡種
    /// </summary>
    /// <returns></returns>
    public DataTable GetCardTypeByGroupId(string dropCard_Group_RID)
    {
        DataSet dstCardTypeByGroupId = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("group_rid", dropCard_Group_RID);
            dstCardTypeByGroupId = dao.GetList(SEL_CARDTYPE_1 + " order by [TYPE],[AFFINITY],[PHOTO] ", dirValues);
            if (dstCardTypeByGroupId != null && dstCardTypeByGroupId.Tables.Count > 0
                && dstCardTypeByGroupId.Tables[0].Rows.Count > 0)
            {
                return dstCardTypeByGroupId.Tables[0];
            }
            else
            {
                return null;
            }
        }   
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 判斷是否為日結日
    /// </summary>
    /// <returns>true:存在 false:不存在</returns>
    public bool isCheckDate(DateTime CheckDate)
    {
        try
        {
            dirValues.Add("CheckDate", CheckDate);
            return dao.Contains(CON_CHECK_DATE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 刪除人工输入
    /// </summary>
    /// <param name="strRID"></param>
    public void DelFactory_Change_Import(int strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", strRID);
            dao.GetList(DEL_FACTORY_CHANGE_IMPORT, dirValues);
            SetOprLog("4");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("刪除資料失敗！");
        }
    }

    /// <summary>
    /// 檢查是否有重復的卡種及卡種狀況
    /// </summary>
    public DataSet GetFACTORY_CHANGE_IMPORT(string strDateTime)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Date_Time", Convert.ToDateTime(strDateTime));
            return dao.GetList(SEL_FACTORY_CHANGE_IMPORT_WHERE + " where Date_Time=@Date_Time",dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 根據匯入的小計檔，生成物料消耗記錄，并判斷物料庫存是否在安全水位，
    /// 如果不在安全水位，報警。
    /// </summary>
    /// <param name="strFactory_RID"></param>
    /// <param name="importDate"></param>
    //public void Material_Used_Warnning(string strFactory_RID,
    //    DateTime importDate)
    //{
    //    try
    //    {
    //        // 取最後日結日期。
    //        DateTime TheLastestSurplusDate = getLastSurplusDate();

    //        #region 計算從最後一個日結日期的下一天到資料匯入日期的卡片製成數，保存到臨時表(TEMP_MADE_CARD)
    //        dirValues.Clear();
    //        dirValues.Add("Perso_Factory_RID", strFactory_RID);
    //        dirValues.Add("From_Date_Time", TheLastestSurplusDate.ToString("yyyy/MM/dd 23:59:59"));
    //        dirValues.Add("End_Date_Time", importDate.ToString("yyyy/MM/dd 23:59:59"));
    //        DataSet dsMade_Card = dao.GetList(SEL_MADE_CARD_WARNNING, dirValues);
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
    //                DataRow[] drEXPRESSIONS = dsEXPRESSIONS_DEFINE.Tables[0].Select("RID = " + dr["Status_RID"].ToString());
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
    //                DataRow[] drEXPRESSIONS = dsEXPRESSIONS_DEFINE.Tables[0].Select("RID = " + dr["Status_RID"].ToString());
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
    //        dao.ExecuteNonQuery(DEL_TEMP_MADE_CARD, this.dirValues);

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
    //        //200908IR耗用量=小計檔數量*（1+耗損率） add  by 楊昆 2009/09/11 start
    //        InOut001BL BL001 = new InOut001BL();
    //        DataTable dtMATERIAL_USED = BL001.getMaterialUsed(strFactory_RID, importDate);
    //        //200908IR耗用量=小計檔數量*（1+耗損率） add  by 楊昆 2009/09/11 end
    //        //DataTable dtMATERIAL_USED = getMaterialUsed(strFactory_RID, importDate);

    //        // 計算物料剩余數量并警示
    //        getMaterielStocks(TheLastestSurplusDate,
    //            strFactory_RID, 
    //            importDate, 
    //            dtMATERIAL_USED);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
    //    }
    //}

    /// <summary>
    /// 計算物料剩余數量并警示
    /// </summary>
    /// <param name="strFactory_RID"></param>
    /// <param name="importDate"></param>
    /// <param name="dtMATERIAL_USED"></param>
    //public void getMaterielStocks(DateTime dtLastWorkDate,
    //        string strFactory_RID,
    //        DateTime importDate,
    //        DataTable dtMATERIAL_USED)
    //{
    //    try
    //    {
    //        Depository010BL bl010 = new Depository010BL();

    //        #region 根據前一天的庫存及今天的庫存。計算物料剩餘數量，判斷是否報警
    //        foreach (DataRow drMATERIAL_USED in dtMATERIAL_USED.Rows)
    //        {
    //            dirValues.Clear();
    //            dirValues.Add("perso_factory_rid", strFactory_RID);
    //            dirValues.Add("serial_number", drMATERIAL_USED["Serial_Number"].ToString());
    //            DataSet dsMaterielStocksManager = dao.GetList(SEL_MATERIEL_STOCKS_MANAGER, dirValues);
    //            if (null != dsMaterielStocksManager &&
    //                dsMaterielStocksManager.Tables.Count > 0 &&
    //                dsMaterielStocksManager.Tables[0].Rows.Count > 0)
    //            {
    //                // 從盤整日到日結日，耗用
    //                this.dirValues.Clear();
    //                this.dirValues.Add("perso_factory_rid", strFactory_RID);
    //                this.dirValues.Add("serial_number", drMATERIAL_USED["Serial_Number"].ToString());
    //                this.dirValues.Add("from_stock_date", Convert.ToDateTime(dsMaterielStocksManager.Tables[0].Rows[0]["Stock_Date"]).ToString("yyyy/MM/dd 23:59:59"));
    //                this.dirValues.Add("end_stock_date", dtLastWorkDate.ToString("yyyy/MM/dd 23:59:59"));
    //                DataSet dsUsedMaterial = dao.GetList(SEL_MATERIEL_USED, this.dirValues);
    //                if (null != dsUsedMaterial &&
    //                    dsUsedMaterial.Tables.Count > 0 &&
    //                    dsUsedMaterial.Tables[0].Rows.Count > 0)
    //                {
    //                    // 盤整時的庫存
    //                    int intLastStockNumber = Convert.ToInt32(dsMaterielStocksManager.Tables[0].Rows[0]["Number"].ToString());
    //                    // 從盤整日到最結餘日的消耗
    //                    int intUsedMaterialFront = 0;
    //                    if (dsUsedMaterial.Tables[0].Rows[0]["Number"]!=DBNull.Value)
    //                        intUsedMaterialFront = Convert.ToInt32(dsUsedMaterial.Tables[0].Rows[0]["Number"]);

    //                    // 最後結餘日后的消耗
    //                    int intUsedMaterialAfter = Convert.ToInt32(drMATERIAL_USED["Number"]);

    //                    // 庫存為0時，顯示庫存不足
    //                    if (intLastStockNumber <= 0)
    //                    {
    //                        if (bl010.DmNotSafe_Type(drMATERIAL_USED["Serial_Number"].ToString()))
    //                        {
    //                            // 庫存不足
    //                            string[] arg = new string[1];
    //                            arg[0] = dsMaterielStocksManager.Tables[0].Rows[0]["Name"].ToString();
    //                            Warning.SetWarning(GlobalString.WarningType.SubtotalMaterialInMiss, arg);
    //                        }
    //                    }
    //                    // 如果前一天的庫存小余今天的消耗
    //                    else if (intLastStockNumber < (intUsedMaterialFront + intUsedMaterialAfter))
    //                    {
    //                        if (bl010.DmNotSafe_Type(drMATERIAL_USED["Serial_Number"].ToString()))
    //                        {
    //                            // 庫存不足
    //                            string[] arg = new string[1];
    //                            arg[0] = dsMaterielStocksManager.Tables[0].Rows[0]["Name"].ToString();
    //                            Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInMiss, arg);
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
    //                                    Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInSafe, arg);
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
    //                                    Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInSafe, arg);
    //                                    Warning.SetWarning(GlobalString.WarningType.SubtoalMaterialInSafe, arg);
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
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
    /// 用小計檔生成卡片對應的物料耗用記錄//200909IR 可直接調用InOut001BL中的相同方法
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
    //        this.dirValues.Add("perso_factory_rid", strFactory_RID);
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
    //                    drNewCARD_EXPONENT["Number"] = Convert.ToInt32(dr["Number"]);
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
    //                    drNewENVELOPE_INFO["Number"] = Convert.ToInt32(dr["Number"]);
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
    //                    drNewDMTYPE_INFO["Number"] = Convert.ToInt32(dr["Number"]);
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

}
