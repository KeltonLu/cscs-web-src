//******************************************************************
//*  作    者：Yanli.Ji
//*  功能說明：年度換卡預測檔邏輯
//*  創建日期：2008-09-11
//*  修改日期：2008-09-11 12:00
//*  修改記錄：
//*            □2008-09-12
//*              1.創建 Yanli.Ji
//               2.修改 JunWang
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
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// InOut004BL 的摘要描述
/// </summary>
public class InOut004BL : BaseLogic
{

    #region SQL語句
    public const string SEL_FORE_CHANGE_CARD = "SELECT CT. RID AS 卡種RID,FCC.TYPE,FCC.PHOTO,FCC.AFFINITY,CT.NAME,CTH.NAME AS 換卡版面,CTT.NAME AS 替換卡版面,FCC.Change_Date,FCC.Number,'' as 耗用卡版面 FROM FORE_CHANGE_CARD FCC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCC.TYPE = CT.TYPE AND FCC.PHOTO = CT.PHOTO AND FCC.AFFINITY = CT.AFFINITY LEFT JOIN CARD_TYPE  CTH ON CTH.RST = 'A' AND CT.Change_Space_RID = CTH.RID AND CTH.Is_Using = 'Y' LEFT JOIN CARD_TYPE CTT ON CTT.RST = 'A' AND CT.Replace_Space_RID = CTT.RID AND CTT.Is_Using = 'Y'  WHERE FCC.RST = 'A' AND FCC.Change_Date>=@BGChange_Date AND FCC.Change_Date<=@EndChange_Date ";

    public const string SEL_FORE_CHANGE_CARD_dateCol = "SELECT distinct FCC.Change_Date FROM FORE_CHANGE_CARD FCC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCC.TYPE = CT.TYPE AND FCC.PHOTO = CT.PHOTO AND FCC.AFFINITY = CT.AFFINITY LEFT JOIN CARD_TYPE  CTH ON CTH.RST = 'A' AND CT.Change_Space_RID = CTH.RID AND CTH.Is_Using = 'Y' LEFT JOIN CARD_TYPE CTT ON CTT.RST = 'A' AND CT.Replace_Space_RID = CTT.RID AND CTT.Is_Using = 'Y' WHERE FCC.RST = 'A' AND FCC.Change_Date>=@BGChange_Date AND FCC.Change_Date<=@EndChange_Date ";

    public const string SEL_FORE_CHANGE_CARD_Name = "SELECT distinct CT.NAME FROM FORE_CHANGE_CARD FCC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCC.TYPE = CT.TYPE AND FCC.PHOTO = CT.PHOTO AND FCC.AFFINITY = CT.AFFINITY LEFT JOIN CARD_TYPE  CTH ON CTH.RST = 'A' AND CT.Change_Space_RID = CTH.RID AND CTH.Is_Using = 'Y' LEFT JOIN CARD_TYPE CTT ON CTT.RST = 'A' AND CT.Replace_Space_RID = CTT.RID AND CTT.Is_Using = 'Y' WHERE FCC.RST = 'A' AND FCC.Change_Date>=@BGChange_Date AND FCC.Change_Date<=@EndChange_Date ";

    public const string SEL_CARD_TYPE = "SELECT CT.RID FROM CARD_TYPE AS CT WHERE CT.RST = 'A' ";

    public const string CON_FORE_CHANGE_CARD = "SELECT COUNT(*) FROM FORE_CHANGE_CARD AS FCC WHERE FCC.RST = 'A' ";

    public const string DEl_FORE_CHANGE_CARD = "DELETE FROM  FORE_CHANGE_CARD WHERE RST = 'A' ";

    public const string SEL_FILE_NAME = "SELECT File_Name FROM IMPORT_PROJECT WHERE RST = 'A' AND Type = '3' ";

    public const string SEL_CARDTYPE = "SELECT * FROM CARD_TYPE WHERE RST = 'A' ";

    public const string DEL_PERSO_FORE_CHANGE_CARD = "DELETE FROM PERSO_FORE_CHANGE_CARD WHERE RST = 'A' AND Fore_RID IN (SELECT RID FROM FORE_CHANGE_CARD WHERE RST = 'A' AND Change_Date = @change_date AND Type = @type AND Affinity = @affinity AND Photo = @photo) ";

    public const string DEL_FORE_CHANGE_CARD = "DELETE FROM FORE_CHANGE_CARD WHERE RST = 'A' AND Change_Date = @change_date AND Type = @type AND Affinity = @affinity AND Photo = @photo ";
    public const string SEL_FORE_CHANGE_CARD1 = "select * FROM FORE_CHANGE_CARD WHERE RST = 'A' AND Change_Date = @change_date AND Type = @type AND Affinity = @affinity AND Photo = @photo ";

    public const string SEL_CARDTYPE_PERSO = "SELECT PC.*,CT.TYPE,CT.AFFINITY,CT.PHOTO FROM PERSO_CARDTYPE PC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND PC.CardType_RID = CT.RID WHERE PC.RST = 'A' AND PC.CardType_RID = @cardtype_rid ORDER BY PC.Base_Special DESC,PC.Priority ASC ";

    public const string IN_CARD_YEAR_FORCAST_PRINT = "INSERT INTO RPT_InOut004  (卡片編號,版面簡稱,耗用卡版面,年月,數量,RCT) VALUES (@卡片編號,@版面簡稱,@耗用卡版面,@年月,@數量,@RCT) ";

    public const string SEL_BATCH_MANAGE = "SELECT COUNT(*) FROM BATCH_MANAGE WHERE (RID = 2 OR RID = 6) AND Status = 'Y'";
    public  string UPDATE_BATCH_MANAGE_START = "UPDATE BATCH_MANAGE SET Status = 'Y',RUU='InOut004BL.cs',RUT='" +DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE RID = 2 OR RID = 6 ";
    public  string UPDATE_BATCH_MANAGE_END = "UPDATE BATCH_MANAGE SET Status = 'N',RUU='InOut004BL.cs',RUT='"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE RID = 2 OR RID = 6 ";

    #endregion

    //數據參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public InOut004BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 將年度換卡設置為FALSE
    /// </summary>
    /// <returns></returns>
    public void YearChangeForcastEnd()
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
    /// 檢查年度換卡是否已經被開起。如果已經開起，返回FALSE
    ///                             如果沒有開起，開起，并返回TRUE
    /// </summary>
    /// <returns></returns>
    public bool YearChangeForcastStart()
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
    /// 服務器匯入檢查
    /// </summary>
    /// <param name="Year">年</param>
    /// <param name="Month">月</param>
    /// <returns></returns>
    public string FileCheck(int Year, int Month)
    {
        //文檔名
        string FileName = "";
        string basepath = ConfigurationManager.AppSettings["YearReplaceCardForecastFilesPath"].ToString();
        bool Exists = true;
        DataSet dsCARDTYPE = null;
        string strFileName = "";

        string sError = "";

        try
        {

            DataSet dsFILE_NAME = dao.GetList(SEL_FILE_NAME);
            foreach (DataRow dr in dsFILE_NAME.Tables[0].Rows)
            {
                FileName = dr[0].ToString() + Year.ToString() + Month.ToString("00") + ".txt";
                if (File.Exists(basepath + "\\" + FileName))
                {
                    Exists = false;
                    strFileName += FileName + ",";
                    dsCARDTYPE = DetailCheck(basepath + "\\" + FileName,ref sError );

                    if (sError == "")
                    {
                        In(dsCARDTYPE, FileName);
                    }

                }
            }
            if (Exists)
            {
                throw new AlertException("沒有找到當前匯入年預測檔！");
            }

            return sError;
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
       
    }

    //讀取文件中的數據到數據集中
    public DataSet DetailCheck(string strPath, ref string strErr)
    {
        DataSet dtsReturn = new DataSet();
        #region 驗證文件
        StreamReader sr = null;
        DataSet dsCARDTYPE = null;
        try
        {
            //新建數據表
            DataTable dtblFileImp = new DataTable();
            dtblFileImp.Columns.Add("Type_Code");
            dtblFileImp.Columns.Add("Affinity_Code");
            dtblFileImp.Columns.Add("StartDate");
            dtblFileImp.Columns.Add("Photo_Code");
            dtblFileImp.Columns.Add("DtDate_Number");

            dsCARDTYPE = dao.GetList(SEL_CARDTYPE);
            sr = new StreamReader(strPath, System.Text.Encoding.Default);
            string[] strLine;
            string strReadLine = "";
            int count = 0;
            //string strErr = "";
            int iErrLen = 0;
            int TableCount = 0;

            DataTable dtData_Number = new DataTable();
            dtData_Number.Columns.Add("Date");
            dtData_Number.Columns.Add("Number");

            DataRow dr = dtblFileImp.NewRow();

            string NowTime = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);


            while ((strReadLine = sr.ReadLine()) != null)
            {
                if (StringUtil.IsEmpty(strReadLine))
                {
                    count++;
                    continue;
                }

                if (strReadLine.Contains("E-TYPE"))
                {
                    strLine = strReadLine.Split(':');
                    if (strLine.Length != 2 || dtData_Number.Rows.Count == 0)
                        throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);

                    string strTemp = strLine[1];

                    string Pattern = @"\w+";
                    MatchCollection Matches = Regex.Matches(strTemp, Pattern, RegexOptions.IgnoreCase);
                    if (Matches.Count != 18)
                        throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);



                    if (Matches[0].Length != 2)
                        strErr += "第" + count.ToString() + "組的E_TYPE代碼 " + Matches[0].ToString() + "不足二位;\\n";


                    dr[3] = Matches[0].ToString();

                    #region 驗證卡種狀況
                    for (int i = 1; i < Matches.Count - 1; i++)
                    {
                        if (i != Matches.Count - 1)
                            strErr += CheckNumberColumn(Matches[i].ToString(), i, count);

                        dtData_Number.Rows[i - 1][1] = Matches[i].ToString().Replace(",", "");
                    }
                    int TableNum = TableCount + 1;
                    if (dtblFileImp.Select("Type_Code='" + dr["Type_Code"].ToString() + "' AND Affinity_Code = '" + dr["Affinity_Code"].ToString() + "' AND Photo_Code = '" + dr["Photo_Code"].ToString() + "' ").Length > 0)
                    {
                        strErr += "第" + TableNum + "組 " + dr["Type_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Photo_Code"].ToString();
                        strErr += "對應的卡種不能重複匯入!\\n";
                    }
                    else
                    {

                        if (dsCARDTYPE.Tables[0].Select("TYPE='" + dr["Type_Code"].ToString() + "' AND AFFINITY = '" + dr["Affinity_Code"].ToString() + "' AND PHOTO = '" + dr["Photo_Code"].ToString() + "' ").Length == 0)
                        {
                            strErr += "第" + TableNum + "組 " + dr["Type_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Photo_Code"].ToString();
                            strErr += "對應的卡種不存在!\\n";
                        }
                        else
                        {

                            if (dsCARDTYPE.Tables[0].Select("Begin_Time<='" + dr["StartDate"].ToString().Substring(0, 4) + "-" + dr["StartDate"].ToString().Substring(4, 2) + "-" + "01" + "' AND (End_Time='1900-01-01' or End_Time >='" + dr["StartDate"].ToString().Substring(0, 4) + "-" + dr["StartDate"].ToString().Substring(4, 2) + "-" + "01" + "') AND TYPE='" + dr["Type_Code"].ToString() + "' AND AFFINITY = '" + dr["Affinity_Code"].ToString() + "' AND PHOTO = '" + dr["Photo_Code"].ToString() + "' ").Length == 0)
                            {
                                strErr += "第" + TableNum + "組 " + dr["Type_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Photo_Code"].ToString();
                                strErr += "對應的卡種不在有效期內!;\\n";
                            }


                            if (dsCARDTYPE.Tables[0].Select("TYPE='" + dr["Type_Code"].ToString() + "' AND AFFINITY = '" + dr["Affinity_Code"].ToString() + "' AND PHOTO = '" + dr["Photo_Code"].ToString() + "' AND Is_Using = 'N' ").Length >= 1)
                            {
                                strErr += "第" + TableNum + "組 " + dr["Type_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Photo_Code"].ToString();
                                strErr += "對應的卡種已停用!\\n";
                            }
                        }
                    }
                    #endregion



                    dr[4] = TableCount;
                    ++TableCount;

                    //檢查這一行是否有錯誤信息！
                    if (strErr.Length > iErrLen)
                    {
                        iErrLen = strErr.Length;
                    }
                    else
                    {
                        DataRow drClone = dtblFileImp.NewRow();
                        drClone[0] = dr[0];
                        drClone[1] = dr[1];
                        drClone[2] = dr[2];
                        drClone[3] = dr[3];
                        drClone[4] = dr[4];
                        dtblFileImp.Rows.Add(drClone);
                    }

                    DataTable dtblTemp = new DataTable();
                    dtblTemp = dtData_Number.Clone();
                    foreach (DataRow drow in dtData_Number.Rows)
                    {
                        DataRow drowTemp = dtblTemp.NewRow();
                        drowTemp.ItemArray = drow.ItemArray;
                        dtblTemp.Rows.Add(drowTemp);
                    }
                    dtsReturn.Tables.Add(dtblTemp);

                    count++;
                }
                else if (strReadLine.Contains("===="))
                {
                    count++;
                    continue;
                }
                else if (strReadLine.Contains("AFFIN"))
                {
                    dtData_Number = new DataTable();

                    dtData_Number.Columns.Add("Date");
                    dtData_Number.Columns.Add("Number");

                    strLine = strReadLine.Split(':');
                    if (strLine.Length != 2 || dr[0].ToString() == "")
                        throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);

                    string strTemp = strLine[1];

                    string Pattern = @"\w+";
                    MatchCollection Matches = Regex.Matches(strTemp, Pattern, RegexOptions.IgnoreCase);
                    if (Matches.Count != 18)
                        throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);

                    if (Matches[0].Length != 4)
                        strErr += "第" + count.ToString() + "組的AFFIN代碼 " + Matches[0].ToString() + "不足四位;\\n";

                    dr[1] = Matches[0].ToString();


                    for (int i = 1; i < Matches.Count - 1; i++)
                    {
                        if (i != Matches.Count - 1)
                            strErr += CheckDateColumn(Matches[i].ToString(), i, count);

                        if (i == 1)
                        {
                            dr[2] = Matches[i].ToString();
                        }

                        DataRow dtDn = dtData_Number.NewRow();
                        dtDn[0] = Matches[i];
                        dtDn[1] = 0;
                        dtData_Number.Rows.Add(dtDn);
                    }
                    count++;
                }
                else if (strReadLine.Contains("TYPE"))
                {
                    dr = dtblFileImp.NewRow();


                    strLine = strReadLine.Split(':');
                    if (strLine.Length != 2)
                        throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);

                    dr[0] = strLine[1].Trim();

                    count++;
                }
                else
                {
                    count++;
                    continue;
                }
            }
            dtsReturn.Tables.Add(dtblFileImp);

            if (dtsReturn.Tables.Count < 2)
                throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);

            if (!StringUtil.IsEmpty(strErr))
            {

                // 拋出異常
                //throw new AlertException(strErr);
            }





            return dtsReturn;

        }
        catch (AlertException ex)
        {
            // 匯入格式不正確，警示
            string[] arg = new string[1];
            arg[0] = ex.Message;
            Warning.SetWarning(GlobalString.WarningType.YearChangeCardForeCast, arg);

            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (sr != null)
                sr.Close();
        }
        #endregion
    }

    /// <summary>
    /// 將匯入資料添加到數據庫中
    /// </summary>
    /// <returns></returns>
    public void In(DataSet dsFileImp,string strFileName)
    {
        FORE_CHANGE_CARD fccModel = null;
        DataSet dsCARDTYPE = null;

        try
        {
            dao.OpenConnection();

            foreach (DataRow dr in dsFileImp.Tables[dsFileImp.Tables.Count - 1].Rows)
            {
                int intNum = int.Parse(dr["DtDate_Number"].ToString());
                foreach (DataRow dr_date in dsFileImp.Tables[intNum].Rows)
                {
                    fccModel = new FORE_CHANGE_CARD();

                    dirValues.Clear();
                    dirValues.Add("change_date", dr_date["Date"].ToString());
                    dirValues.Add("type", dr["Type_Code"].ToString());
                    dirValues.Add("affinity", dr["Affinity_Code"].ToString());
                    dirValues.Add("photo", dr["Photo_Code"].ToString());
                    //刪除換卡記錄拆分檔
                    dao.ExecuteNonQuery(DEL_PERSO_FORE_CHANGE_CARD, dirValues);
                    //刪除換卡記錄檔
                    //dao.ExecuteNonQuery(DEL_FORE_CHANGE_CARD, dirValues);

                    int intRID = 0;

                    if (dao.GetList(SEL_FORE_CHANGE_CARD1, dirValues).Tables[0].Rows.Count > 0)
                    {
                        fccModel = dao.GetModel<FORE_CHANGE_CARD>(SEL_FORE_CHANGE_CARD1, dirValues);
                        fccModel.Number = Convert.ToInt64(dr_date["Number"]);
                        fccModel.IsYear = "1";
                        dao.Update<FORE_CHANGE_CARD>(fccModel, "RID");
                        intRID = fccModel.RID;
                    }
                    else
                    {
                        //添加次月換卡預測訊息。Dao.add(),并取出新添加記錄的RID
                        fccModel.Change_Date = dr_date["Date"].ToString();
                        //fccModel.Type = dr_date["Type_Code"].ToString();
                        //fccModel.Affinity = dr_date["Affinity_Code"].ToString();
                        //fccModel.Photo = dr_date["Photo_Code"].ToString();
                        fccModel.Type = dr["Type_Code"].ToString();
                        fccModel.Affinity = dr["Affinity_Code"].ToString();
                        fccModel.Photo = dr["Photo_Code"].ToString();
                        fccModel.Number = Convert.ToInt64(dr_date["Number"]);
                        fccModel.IsMonth = "2";
                        fccModel.IsYear = "1";
                        intRID = Convert.ToInt32(dao.AddAndGetID<FORE_CHANGE_CARD>(fccModel, "RID"));
                    }

                    //添加次月換卡預測訊息。Dao.add(),并取出新添加記錄的RID
                    //fccModel.Change_Date = dr_date["Date"].ToString();
                    //fccModel.Type = dr["Type_Code"].ToString();
                    //fccModel.Affinity = dr["Affinity_Code"].ToString();
                    //fccModel.Photo = dr["Photo_Code"].ToString();
                    //fccModel.Number = Convert.ToInt64(dr_date["Number"]);
                    //fccModel.IsYear = "1";
                    //int intRID = Convert.ToInt32(dao.AddAndGetID<FORE_CHANGE_CARD>(fccModel, "RID"));

                    dsCARDTYPE = dao.GetList(SEL_CARDTYPE + "AND Type = @type AND Affinity = @affinity AND Photo = @photo", dirValues);
                    foreach (DataRow dr1 in dsCARDTYPE.Tables[0].Rows)
                    {
                        SplitToPerso(Convert.ToInt32(dr1["RID"]), intRID, Convert.ToInt64(dr_date["Number"]), dr_date["Date"].ToString());
                    }
                }

            }

            SetOprLog("11");

            InOut006BL bl1 = new InOut006BL();
            bl1.dao = dao;
            bl1.AddLog("6", strFileName);

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
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 按Perso廠分配原則來拆分
    /// </summary>
    /// <returns></returns>
    public void SplitToPerso(int Space_RID, int intRID, long DtDate_Number, string StartDate)
    {
        PERSO_FORE_CHANGE_CARD pfccModel = new PERSO_FORE_CHANGE_CARD();
        DataSet dsCARDTYPE_PERSO = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("cardtype_rid", Space_RID);
            dsCARDTYPE_PERSO = dao.GetList(SEL_CARDTYPE_PERSO, dirValues);

            if (dsCARDTYPE_PERSO.Tables[0].Rows.Count != 0)
            {
                if (dsCARDTYPE_PERSO.Tables[0].Rows[0]["Base_Special"].ToString() == "1")
                {
                    pfccModel.Change_Date = StartDate;
                    pfccModel.Type = dsCARDTYPE_PERSO.Tables[0].Rows[0]["Type"].ToString();
                    pfccModel.Affinity = dsCARDTYPE_PERSO.Tables[0].Rows[0]["Affinity"].ToString();
                    pfccModel.Photo = dsCARDTYPE_PERSO.Tables[0].Rows[0]["Photo"].ToString();
                    pfccModel.Perso_Factory_RID = Convert.ToInt32(dsCARDTYPE_PERSO.Tables[0].Rows[0]["Factory_RID"].ToString());
                    pfccModel.Number = DtDate_Number;
                    pfccModel.Fore_RID = intRID;
                    dao.Add<PERSO_FORE_CHANGE_CARD>(pfccModel, "RID");
                }
                else {
                    // 取特殊分配訊息
                    DataRow[] drCARDTYPE_PERSO = dsCARDTYPE_PERSO.Tables[0].Select("Base_Special = '2'");
                    // 按比率分配
                    if (drCARDTYPE_PERSO[0]["Percentage_Number"].ToString() == "1")
                    {
                        long intNumber = 0;
                        for (int int1 = 0; int1 < drCARDTYPE_PERSO.Length; int1++)
                        {
                            if (int1 < drCARDTYPE_PERSO.Length - 1)
                            {
                                intNumber += Convert.ToInt64(Math.Floor(DtDate_Number * (Convert.ToDouble(drCARDTYPE_PERSO[int1]["Value"]) / 100)));
                                pfccModel.Change_Date = StartDate;
                                pfccModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString();
                                pfccModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString();
                                pfccModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString();
                                pfccModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                pfccModel.Number = Convert.ToInt64(Math.Floor(DtDate_Number * (Convert.ToDouble(drCARDTYPE_PERSO[int1]["Value"]) / 100)));
                                pfccModel.Fore_RID = intRID;
                                dao.Add<PERSO_FORE_CHANGE_CARD>(pfccModel, "RID");
                            }
                            else
                            {
                                pfccModel.Change_Date = StartDate;
                                pfccModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString();
                                pfccModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString();
                                pfccModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString();
                                pfccModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                pfccModel.Number = DtDate_Number - intNumber;
                                pfccModel.Fore_RID = intRID;
                                dao.Add<PERSO_FORE_CHANGE_CARD>(pfccModel, "RID");
                            }
                        }
                    }
                    // 按數量分配
                    else if (drCARDTYPE_PERSO[0]["Percentage_Number"].ToString() == "2")
                    {
                        long intNumber = 0;
                        for (int int1 = 0; int1 < drCARDTYPE_PERSO.Length; int1++)
                        {
                            if (int1 < drCARDTYPE_PERSO.Length - 1)
                            {
                                if ((DtDate_Number - intNumber) > Convert.ToInt32(drCARDTYPE_PERSO[int1]["Value"]))
                                {
                                    intNumber += Convert.ToInt32(drCARDTYPE_PERSO[int1]["Value"]);
                                    pfccModel.Change_Date = StartDate;
                                    pfccModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString(); ;
                                    pfccModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString(); ;
                                    pfccModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString(); ;
                                    pfccModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                    pfccModel.Number = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Value"]);
                                    pfccModel.Fore_RID = intRID;
                                    dao.Add<PERSO_FORE_CHANGE_CARD>(pfccModel, "RID");
                                }
                                else
                                {
                                    pfccModel.Change_Date = StartDate;
                                    pfccModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString(); ;
                                    pfccModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString(); ;
                                    pfccModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString(); ;
                                    pfccModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                    pfccModel.Number = DtDate_Number - intNumber;
                                    pfccModel.Fore_RID = intRID;
                                    dao.Add<PERSO_FORE_CHANGE_CARD>(pfccModel, "RID");
                                    break;
                                }
                            }
                            else
                            {
                                pfccModel.Change_Date = StartDate;
                                pfccModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString(); ;
                                pfccModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString(); ;
                                pfccModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString(); ;
                                pfccModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                pfccModel.Number = DtDate_Number - intNumber;
                                pfccModel.Fore_RID = intRID;
                                dao.Add<PERSO_FORE_CHANGE_CARD>(pfccModel, "RID");
                            }
                        }
                    }
                }
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    #region 查詢年度換卡記錄列表
    // 查詢年度換卡記錄列表
    public DataSet List(Dictionary<string, object> searchInput, ref DataSet dtsName_Sum)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        StringBuilder stbCommand = new StringBuilder(SEL_FORE_CHANGE_CARD);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropDate_Begin"].ToString().Trim()))
            {
                dirValues.Add("BGChange_Date", searchInput["dropDate_Begin"].ToString());
            }
            if (!StringUtil.IsEmpty(searchInput["dropDate_Over"].ToString().Trim()))
            {
                dirValues.Add("EndChange_Date", searchInput["dropDate_Over"].ToString());
            }

            if (((DataTable)searchInput["uctrlCARDNAME"]).Rows.Count != 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["uctrlCARDNAME"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";
                stbWhere.Append(" AND CT.RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ") ");
            }
        }
        //存放日期列
        DataSet dsCol = null;
        //存放卡種名稱
        DataSet dsName = null;

        DataRow[] drowsName_Sum = null;
        //存放日期列
        DataRow[] drowsdsCol = null;
        //存放卡種名稱
        DataRow[] drowsName = null;
        try
        {
            dtsName_Sum = dao.GetList(stbCommand.ToString() +" and (FCC.IsYear='1' or FCC.IsYear is null) "+ stbWhere.ToString() + " ORDER BY 換卡版面,卡種RID,Change_Date ", dirValues);
            dsCol = dao.GetList(SEL_FORE_CHANGE_CARD_dateCol + " and (FCC.IsYear='1' or FCC.IsYear is null) "+stbWhere.ToString() + " ORDER BY Change_Date ", dirValues);
            dsName = dao.GetList(SEL_FORE_CHANGE_CARD_Name + " and (FCC.IsYear='1' or FCC.IsYear is null) "+stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }

        DataSet dtForOut = null;
        foreach (DataRow dr in dtsName_Sum.Tables[0].Rows)
        {
            if (dr["換卡版面"].ToString() == "")
            {
                if (dr["替換卡版面"].ToString() == "")
                {
                    dr["耗用卡版面"] = dr["NAME"].ToString();
                }
                else
                {
                    dr["耗用卡版面"] = dr["替換卡版面"].ToString();
                }
            }
            else
            {
                dr["耗用卡版面"] = dr["換卡版面"].ToString();
            }
        }

        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                drowsName_Sum = dtsName_Sum.Tables[0].Select("NAME like '%" + searchInput["txtName"].ToString().Trim() + "%' ");
                //drowsdsCol = dsCol.Tables[0].Select("NAME like '%" + searchInput["txtName"].ToString().Trim() + "%' ");
                //drowsName = dsName.Tables[0].Select("NAME like '%" + searchInput["txtName"].ToString().Trim() + "%' ");

                if (drowsName_Sum.Length != 0)
                {
                    dtsName_Sum = DataRows_Convert_DataSet(drowsName_Sum, ref dtsName_Sum);
                }
                else
                {
                    dtsName_Sum = null;
                }
            }
            if (!StringUtil.IsEmpty(searchInput["txtDeplete"].ToString().Trim()) && dtsName_Sum != null)
            {
                drowsName_Sum = dtsName_Sum.Tables[0].Select("耗用卡版面 like '%" + searchInput["txtDeplete"].ToString().Trim() + "%' ");
                //drowsdsCol = dsCol.Tables[0].Select("耗用卡版面 like '%" + searchInput["txtDeplete"].ToString().Trim() + "%' ");
                //drowsName = dsName.Tables[0].Select("耗用卡版面 like '%" + searchInput["txtDeplete"].ToString().Trim() + "%' ");

                if (drowsName_Sum.Length != 0)
                {
                    dtsName_Sum = DataRows_Convert_DataSet(drowsName_Sum, ref dtsName_Sum);
                }
                else
                {
                    dtsName_Sum = null;
                }
            }
            if (!StringUtil.IsEmpty(searchInput["txtAffinity_Code"].ToString().Trim()))
            {
                drowsName_Sum = dtsName_Sum.Tables[0].Select("AFFINITY = '" + searchInput["txtAffinity_Code"].ToString().Trim() + "' ");
                //drowsdsCol = dsCol.Tables[0].Select("AFFINITY = '" + searchInput["txtAffinity_Code"].ToString().Trim() + "' ");
                //drowsName = dsName.Tables[0].Select("AFFINITY = '" + searchInput["txtAffinity_Code"].ToString().Trim() + "' ");

                if (drowsName_Sum.Length != 0)
                {
                    dtsName_Sum = DataRows_Convert_DataSet(drowsName_Sum, ref dtsName_Sum);
                }
                else
                {
                    dtsName_Sum = null;
                }
            }
        }

        //格式化數據到數據集中
        //dtForOut = FormatDataForView(dtsName_Sum, dtsDate_Number);
        dtForOut = FormatDataForView(dtsName_Sum, dsCol, dsName);

        return dtForOut;
    }
    #endregion

    #region 格式化數據表
    private DataSet FormatDataForView(DataSet dtsName_Sum, DataSet dsCol, DataSet dsName)
    {
        if (dtsName_Sum == null)
        {
            return null;
        }
        DataTable dtForOut = new DataTable();

        //為數據表動態添加列名
        DataColumn dcFirst = new DataColumn("卡片編號", typeof(string));
        DataColumn dcFirst1 = new DataColumn("版面簡稱", typeof(string));
        DataColumn dcFirst2 = new DataColumn("耗用版面", typeof(string));

        dtForOut.Columns.Add(dcFirst);
        dtForOut.Columns.Add(dcFirst1);
        dtForOut.Columns.Add(dcFirst2);

        for (int i = 0; i < dsCol.Tables[0].Rows.Count; i++)
        {
            foreach (DataRow dr in dtsName_Sum.Tables[0].Rows)
            {
                if (dr["Change_Date"].ToString() == dsCol.Tables[0].Rows[i]["Change_Date"].ToString())
                {
                    DataColumn dcMiddle = new DataColumn(dsCol.Tables[0].Rows[i]["Change_Date"].ToString(), typeof(string));
                    dtForOut.Columns.Add(dcMiddle);
                    break;
                }
            }
        }
        DataColumn dcLast = new DataColumn("總計", typeof(int));
        dtForOut.Columns.Add(dcLast);
        long sum = 0;
        foreach (DataRow dr in dsName.Tables[0].Rows)
        {
            DataRow newRow = dtForOut.NewRow();
            for (int i = 0; i < dtForOut.Columns.Count; i++)
            {
                if (dtsName_Sum.Tables[0].Select("NAME = '" + dr["NAME"].ToString().Trim() + "' AND Change_Date = '" + dtForOut.Columns[i].ToString() + "' ").Length > 0)
                {
                    DataRow[] drows = dtsName_Sum.Tables[0].Select("NAME = '" + dr["NAME"].ToString().Trim() + "' AND Change_Date = '" + dtForOut.Columns[i].ToString() + "' ");
                    foreach (DataRow dr1 in drows)
                    {
                        newRow[0] = dr1["TYPE"].ToString() + "-" + dr1["AFFINITY"].ToString() + "-" + dr1["PHOTO"].ToString();
                        newRow[1] = dr1["NAME"].ToString();
                        newRow[2] = dr1["耗用卡版面"].ToString();
                        newRow[i] = dr1["Number"].ToString();
                        sum += Convert.ToInt64(dr1["Number"]);
                    }
                }
            }
            newRow["總計"] = sum;
            sum = 0;
            dtForOut.Rows.Add(newRow);
        }

        DataSet dtsForOut = new DataSet();
        dtsForOut.Tables.Add(dtForOut);

        return dtsForOut;

    }
    #endregion

    /// <summary>
    /// 驗證匯入字段是否滿足格式
    /// </summary>
    /// <param name="strColumn"></param>
    /// <param name="num"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private string CheckDateColumn(string strColumn, int num, int count)
    {
        string strErr = "";
        if (strColumn.Length != 6)
        {
            strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤;\\n";
        }
        //else
        //{
        //    try
        //    {
        //        DateTime dt = Convert.ToDateTime(strColumn);
        //    }
        //    catch
        //    {
        //        strErr = "第" + count.ToString() + "行第" + num.ToString() + "列時間格式不對;";
        //    }
        //}
        return strErr;
    }

    private string CheckNumberColumn(string strColumn, int num, int count)
    {
        string strErr = "";
        string Pattern = "";
        MatchCollection Matches;
        Pattern = @"^\d+$";
        Matches = Regex.Matches(strColumn.Replace(",",""), Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        if (Matches.Count == 0)
        {
            strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為數字;\\n";
        }
        return strErr;
    }

    //汇出表格时新增數據到數據庫
    public void ADD_CARD_YEAR_FORCAST_PRINT(DataSet dtsName_Sum,string time)
    {
        dao.ExecuteNonQuery("delete RPT_InOut004 where RCT<"+DateTime.Now.ToString("yyyyMMdd000000"));
        long sum = 0;
        string temp1 = "";
        string temp2 = "";
        if (dtsName_Sum.Tables[0].Rows.Count != 0)
        {
            temp1 = dtsName_Sum.Tables[0].Rows[0]["NAME"].ToString();
        }

        try
        {
            for (int i = 0; i < dtsName_Sum.Tables[0].Rows.Count; i++)
            {
                dirValues.Clear();
                temp2 = dtsName_Sum.Tables[0].Rows[i]["NAME"].ToString();
                dirValues.Add("版面簡稱", dtsName_Sum.Tables[0].Rows[i]["NAME"].ToString());
                dirValues.Add("耗用卡版面", dtsName_Sum.Tables[0].Rows[i][9].ToString());
                dirValues.Add("年月", dtsName_Sum.Tables[0].Rows[i]["Change_Date"].ToString());
                dirValues.Add("數量", dtsName_Sum.Tables[0].Rows[i]["Number"].ToString());
                dirValues.Add("卡片編號", dtsName_Sum.Tables[0].Rows[i]["TYPE"].ToString() + "-" + dtsName_Sum.Tables[0].Rows[i]["AFFINITY"].ToString() + "-" + dtsName_Sum.Tables[0].Rows[i]["PHOTO"].ToString());
                dirValues.Add("RCT", time);
                if (temp2 == temp1)
                {
                    sum += Convert.ToInt64(dtsName_Sum.Tables[0].Rows[i]["Number"]);
                }
                dao.ExecuteNonQuery(IN_CARD_YEAR_FORCAST_PRINT, dirValues);

                if (temp2 != temp1)
                {
                    dirValues.Clear();
                    dirValues.Add("版面簡稱", dtsName_Sum.Tables[0].Rows[i - 1]["NAME"].ToString());
                    dirValues.Add("耗用卡版面", dtsName_Sum.Tables[0].Rows[i - 1][9].ToString());
                    dirValues.Add("卡片編號", dtsName_Sum.Tables[0].Rows[i - 1]["TYPE"].ToString() + "-" + dtsName_Sum.Tables[0].Rows[i - 1]["AFFINITY"].ToString() + "-" + dtsName_Sum.Tables[0].Rows[i - 1]["PHOTO"].ToString());
                    dirValues.Add("年月", "總計");
                    dirValues.Add("數量", sum.ToString());
                    dirValues.Add("RCT", time);
                    dao.ExecuteNonQuery(IN_CARD_YEAR_FORCAST_PRINT, dirValues);
                    sum = Convert.ToInt64(dtsName_Sum.Tables[0].Rows[i]["Number"]);
                    temp1 = temp2;
                }
                if (dtsName_Sum.Tables[0].Rows.Count == i + 1)
                {
                    if (temp2 == temp1)
                    {
                        dirValues.Clear();
                        dirValues.Add("版面簡稱", dtsName_Sum.Tables[0].Rows[i]["NAME"].ToString());
                        dirValues.Add("耗用卡版面", dtsName_Sum.Tables[0].Rows[i][9].ToString());
                        dirValues.Add("卡片編號", dtsName_Sum.Tables[0].Rows[i]["TYPE"].ToString() + "-" + dtsName_Sum.Tables[0].Rows[i]["AFFINITY"].ToString() + "-" + dtsName_Sum.Tables[0].Rows[i]["PHOTO"].ToString());
                        dirValues.Add("年月", "總計");
                        dirValues.Add("數量", sum.ToString());
                        dirValues.Add("RCT", time);
                        dao.ExecuteNonQuery(IN_CARD_YEAR_FORCAST_PRINT, dirValues);
                        sum = 0;
                    }
                }
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 將行集合裝換稱dataset
    /// </summary>
    public DataSet DataRows_Convert_DataSet(DataRow[] drows, ref DataSet dstold)
    {
        //DataSet dstold = new DataSet();
        //DataRow[] drows = dstold.Tables[0].Select("");
        DataTable dtblNew = new DataTable();
        dtblNew = dstold.Tables[0].Clone();
        for (int i = 0; i < drows.Length; i++)
        {
            DataRow drownew = dtblNew.NewRow();

            drownew.ItemArray = drows[i].ItemArray;

            dtblNew.Rows.Add(drownew);
        }
        dstold = new DataSet();
        dstold.Tables.Add(dtblNew);
        return dstold;
    }

    /// <summary>
    /// 選擇年度換卡預測檔的文件名！
    /// </summary>
    /// <returns></returns>
    public DataSet GetFilename()
    {
        DataSet dsFILE_NAME = dao.GetList(SEL_FILE_NAME);
        return dsFILE_NAME;
    }
}
