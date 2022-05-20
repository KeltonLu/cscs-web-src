//******************************************************************
//*  作    者：bingyipan
//*  功能說明：代製費用異動 
//*  創建日期：2008-11-19
//*  修改日期：
//*  修改記錄：
//*            □2008-12-19
//*              1.創建 潘秉奕
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
/// Finance0024BL 的摘要描述
/// </summary>
public class Finance0024BL : BaseLogic
{
    #region SQL語句
    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y' ";

    public const string SEL_PARAM_FINANCE = "SELECT * FROM PARAM WHERE RST = 'A' AND Param_Code = '" + GlobalString.Parameter.Finance + "'";

    public const string SEL_PARAM_FINANCE_C = "SELECT * FROM PARAM WHERE RST = 'A' AND ParamType_Code='"+
GlobalString.ParameterType.Finance + "'";

    public const string SEL_CARD_GROUP = "SELECT RID,GROUP_NAME FROM CARD_GROUP WHERE RST='A' ";

    public const string SEL_PERSON_BY_PERSONID = "SELECT RID,Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y' AND Factory_ID =@Factory_ID";//匯入文件第一行中廠商編號

    public const string SEL_SPECIAL_PROJECT = "SELECT * FROM PERSO_PROJECT WHERE RST = 'A' AND Normal_Special = '2' AND Factory_RID = @Factory_RID";//廠商RID

    public const string CON_SPECIAL_PERSO_PROJECT_IMPORT = "SELECT COUNT(*) FROM SPECIAL_PERSO_PROJECT_IMPORT WHERE RST = 'A' AND Perso_Factory_RID = @Factory_RID AND Project_Date = @Project_Date";//特殊代製項目作業日期

    public const string CON_SPECIAL_PERSO_PROJECT_IN = "SELECT COUNT(*) FROM SPECIAL_PERSO_PROJECT_IMPORT WHERE RST = 'A' AND Perso_Factory_RID = @Factory_RID AND YEAR(Project_Date) = @year AND MONTH(Project_Date) = @month";

    public const string DEL_SPECIAL_PERSO_PROJECT_IN = "DELETE FROM SPECIAL_PERSO_PROJECT_IMPORT WHERE RST = 'A' AND Perso_Factory_RID = @Factory_RID AND YEAR(Project_Date) = @year AND MONTH(Project_Date) = @month";

    public const string SEL_EXCEPTION_PERSO_PROJECT = "SELECT P.Param_Name,CG.Group_Name,EPP.* "
+ " FROM EXCEPTION_PERSO_PROJECT EPP INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND EPP.CardGroup_RID = CG.RID"
+ " INNER JOIN Param P ON P.RST = 'A' AND CG.Param_Code = P.Param_Code AND"
+ " P.Param_Code = '" + GlobalString.Parameter.Finance + "' WHERE EPP.RST = 'A' AND YEAR(Project_Date) = @year"
+ " AND Month(Project_Date) = @month AND EPP.Perso_Factory_RID = @Factory_RID";

    public const string DEL_EXCEPTION_PERSO_PROJECT = "DELETE FROM EXCEPTION_PERSO_PROJECT WHERE RID = @RID";//例外代製項目RID

    public const string SEL_PERSO_PROJECT_CHANGE_DETAIL = "SELECT P1.Param_Name,CG.Group_Name,"
+ " F.Factory_ShortName_CN,P.Param_Name as ProjectName,PPCD.*"
+ " FROM PERSO_PROJECT_CHANGE_DETAIL PPCD INNER JOIN FACTORY F ON F.RST = 'A' AND PPCD.Perso_Factory = F.RID"
+ " INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND PPCD.CardGroup_RID = CG.RID"
+ " INNER JOIN Param P1 ON P1.RST = 'A' AND CG.Param_Code = P1.Param_Code AND"
+ " P1.Param_Code = '" + GlobalString.Parameter.Finance + "'"//帳務
+ " INNER JOIN Param P ON P.RST = 'A' AND PPCD.Param_Code = P.Param_Code AND"
+ " P.ParamType_Code = '" + GlobalString.ParameterType.Finance + "'"//代製費用帳務異動項目設定
+ " WHERE PPCD.RST = 'A' AND year(PPCD.Project_Date) = @year AND month(PPCD.Project_Date)=@month"
+ " AND PPCD.Perso_Factory = @Factory_RID";

    public const string DEL_PERSO_PROJECT_CHANGE_DETAIL = "DELETE FROM PERSO_PROJECT_CHANGE_DETAIL WHERE RID = @RID";//代製費用帳務異動RID

    public const string SEL_EXCEPTION_PERSO_PROJECT_EDIT = "SELECT * FROM EXCEPTION_PERSO_PROJECT WHERE RID = @RID";//例外代製費用RID

    public const string UPDATE_EXCEPTION_PERSO_PROJECT = "UPDATE EXCEPTION_PERSO_PROJECT"
+ " SET Project_Date = @Project_Date,CardGroup_RID = @CardGroup_RID,Perso_Factory_RID = @Factory_RID,Name = @Name,"
+ " Number = @Number,Unit_Price = @Price,Comment = @Comment WHERE RID = @RID";//例外代製項目

    public const string CON_EXCEPTION_PERSO_PROJECT = "SELECT COUNT(*) FROM EXCEPTION_PERSO_PROJECT"
+ " WHERE RST = 'A' AND Project_Date = @Project_Date AND CardGroup_RID = @CardGroup_RID AND"
+ " Perso_Factory_RID = @Factory_RID AND Name = @Name";

    public const string SEL_PERSO_PROJECT_CHANGE_DETAIL_EDIT = "SELECT * FROM PERSO_PROJECT_CHANGE_DETAIL"
+ " WHERE RID = @RID";//代製費用異動

    public const string UPDATE_PERSO_PROJECT_CHANGE_DETAIL = "";

    public const string CON_PERSO_PROJECT_CHANGE_DETAIL = "SELECT COUNT(*) FROM PERSO_PROJECT_CHANGE_DETAIL"
+ " WHERE RST = 'A' AND year(Project_Date) = @year AND month(Project_Date) = @month AND"
+ " CardGroup_RID = @CardGroup_RID AND Perso_Factory = @Factory_RID AND Param_Code = @Finance";//帳務異動項目

    //取指定年月的最後一天工作日
    public const string SEL_WORK_DATE = "select RID,Date_time from WORK_DATE where is_workday='Y' and YEAR(date_time)=@year and MONTH(date_time)=@month order by date_time desc";

    // 特殊代製項目匯入時，如果該時間已經有磁條信用卡群組請款，不能匯入
    public const string CON_ASKED_MONEY_CT_IN = "SELECT COUNT(*) " +
            "FROM PERSO_PROJECT_SAP PPS " +
            "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND PPS.Card_Group_RID = CG.RID AND CG.Group_Name = '磁條信用卡' " +
            "WHERE PPS.RST = 'A' AND PPS.Perso_Factory_RID = @perso_factory_rid AND PPS.Begin_Date<=@in_date AND PPS.End_Date>=@in_date ";

    // 特殊代製項目刪除匯入時，如果該月已經有磁條信用卡群組請款，不能刪除
    public const string CON_ASKED_MONEY_CT_DEL = "SELECT COUNT(*) "+
            "FROM PERSO_PROJECT_SAP PPS " +
            "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND PPS.Card_Group_RID = CG.RID AND CG.Group_Name = '磁條信用卡' " +
            "WHERE PPS.RST = 'A' AND PPS.Perso_Factory_RID = @perso_factory_rid AND (YEAR(PPS.Begin_Date) = @year AND MONTH(PPS.Begin_Date) = @month OR YEAR(PPS.End_Date) = @year AND MONTH(PPS.End_Date) = @month )";

    // 檢查卡種群組是否已經請款
    public const string CON_ASKED_MONEY_GROUP = "SELECT COUNT(*) " +
            "FROM PERSO_PROJECT_SAP PPS " +
            "WHERE PPS.RST = 'A' AND PPS.Perso_Factory_RID = @perso_factory_rid AND Convert(varchar(10),PPS.Begin_Date,111)<=@in_date AND Convert(varchar(10),PPS.End_Date,111)>=@in_date AND PPS.Card_Group_RID = @cgrid ";

    public const string SEL_BATCH_MANAGE = "SELECT COUNT(*) FROM BATCH_MANAGE WHERE (RID=1 OR RID = 4 OR RID = 5) AND Status = 'Y'";
    public  string UPDATE_BATCH_MANAGE_START = "UPDATE BATCH_MANAGE SET Status = 'Y',RUU='Finance0024BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+ "' WHERE RID=1 OR RID = 4 OR RID = 5 ";
    public  string UPDATE_BATCH_MANAGE_END = "UPDATE BATCH_MANAGE SET Status = 'N',RUU='Finance0024BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE RID=1 OR RID = 4 OR RID = 5 ";

    #endregion SQL語句

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Finance0024BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 將特殊代制項目匯入設置為FALSE
    /// </summary>
    /// <returns></returns>
    public void SpecialProjectImportEnd()
    {
        try
        {
            dao.OpenConnection();
            dao.ExecuteNonQuery(UPDATE_BATCH_MANAGE_END);
            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        finally {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 檢查特殊代制項目匯入是否已經被開起。如果已經開起，返回FALSE
    ///                             如果沒有開起，開起，并返回TRUE
    /// </summary>
    /// <returns></returns>
    public bool SpecialProjectImportStart()
    {
        try
        {
            dao.OpenConnection();
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
                dao.Commit();
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        finally {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 獲取Perso卡廠
    /// </summary>
    /// <returns></returns>
    public DataSet getFactory()
    {
        DataSet dstPurpose = null;

        try
        {
            dstPurpose = dao.GetList(SEL_PERSON);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstPurpose;
    }

    /// <summary>
    /// 獲取代製費用帳務異動項目設定
    /// </summary>
    /// <returns></returns>
    public DataSet getParam_Change()
    { 
        DataSet dstParam = null;

        try
        {
            dstParam = dao.GetList(SEL_PARAM_FINANCE_C);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstParam;
    }

    /// <summary>
    /// 獲取用途
    /// </summary>
    /// <returns></returns>
    public DataTable getParam_Finance()
    {
        DataSet ds = new DataSet();
        ds = dao.GetList(SEL_PARAM_FINANCE);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 獲取群組
    /// </summary>
    /// <param name="strPurposeId">用途ID</param>
    /// <returns></returns>
    public DataSet getCardGroup(string strPurposeId)
    {
        DataSet dstGroup = null;
        try
        {
            dirValues.Clear();

            string strSql = SEL_CARD_GROUP;

            if (!StringUtil.IsEmpty(strPurposeId))
            {
                strSql += " and Param_Code=@Param_Code";
                dirValues.Add("Param_Code", strPurposeId);
            }

            dstGroup = dao.GetList(strSql, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstGroup;
    }

    /// <summary>
    /// 根據項目編號獲得名稱
    /// </summary>
    /// <param name="strCode">編號</param>
    /// <returns>名稱</returns>
    public string getProjectNameByCode(string strCode)
    {
        DataSet dstProject = null;
        try
        {
            dirValues.Clear();

            string strSql = "select project_name from PERSO_PROJECT where rst='A' ";

            if (!StringUtil.IsEmpty(strCode))
            {
                strSql += " and project_code=@project_code";
                dirValues.Add("project_code", strCode);
            }

            dstProject = dao.GetList(strSql, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        if (dstProject != null && dstProject.Tables[0].Rows.Count > 0)
        {
            return dstProject.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 服務器匯入檢查
    /// </summary>
    /// <param name="strFileName">FileName</param>   
    /// <returns></returns>
    public bool FileCheck(string strFileName)
    {        
        string basepath = ConfigurationManager.AppSettings["SpecialProjectFilesPath"].ToString();
        bool Exists = true;
        
        try
        {
            if (!File.Exists(basepath + "\\" + strFileName))
            {
                Exists = false;
            }
            else
            {
                Exists = true;
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
        return Exists;
    }

    public DataSet CheckSpecialIn(string strFileName, string strFactoryRID)
    {
        StreamReader sr = null;
        DataSet dstDataIn = null;
        string Factory_Name = "";
        try
        {
            sr = new StreamReader(strFileName, System.Text.Encoding.Default);
            string[] strLine;
            string strReadLine = "";
            int count = 1;
            string strErr = "";

            DataTable dtFactory = new DataTable();
            dtFactory.Columns.Add(new DataColumn("Factory_RID", Type.GetType("System.Int32")));
            dtFactory.Columns.Add(new DataColumn("Factory_ID", Type.GetType("System.String")));
            dtFactory.Columns.Add(new DataColumn("Factory_Name", Type.GetType("System.String")));

            DataTable dtDataIn = new DataTable();           
            dtDataIn.Columns.Add(new DataColumn("Project_Date", Type.GetType("System.String")));// 日期 
            dtDataIn.Columns.Add(new DataColumn("Project_Code", Type.GetType("System.String")));// 特殊代製項目編號
            dtDataIn.Columns.Add(new DataColumn("Number", Type.GetType("System.Int32")));// 代製數量           

            #region 讀字符串，并檢查字符串格式(列數、每列的字符格式),并保存到臨時DataTable(dtDataIn)中
            while ((strReadLine = sr.ReadLine()) != null)
            {
                if (count == 1)
                {
                    // Perso廠一致性檢查
                    if (getFactoryIDByRID(strFactoryRID) != strReadLine.Trim())
                    {
                        throw new AlertException("選擇的Perso廠和匯入文檔中第一行的Perso廠編號不一致");
                    }

                    // 保存Perso廠商訊息
                    DataRow drFactory = dtFactory.NewRow();
                    drFactory["Factory_RID"] = strFactoryRID;
                    drFactory["Factory_ID"] = strReadLine.Trim();
                    drFactory["Factory_Name"] = getFactoryNameByRID(strFactoryRID);
                    Factory_Name = drFactory["Factory_Name"].ToString();
                    dtFactory.Rows.Add(drFactory);
                }
                else
                {
                    // 不是空的行
                    if (!StringUtil.IsEmpty(strReadLine))
                    {
                        if (StringUtil.GetByteLength(strReadLine) != 23)//列數量檢查
                        {
                            throw new AlertException("第" + count.ToString() + "行列數不正確。");
                        }

                        // 分割字符串
                        int nextBegin = 0;
                        Depository003BL bl003 = new Depository003BL();
                        strLine = new string[3];
                        strLine[0] = bl003.GetSubstringByByte(strReadLine, nextBegin, 8, out nextBegin).Trim();
                        strLine[1] = bl003.GetSubstringByByte(strReadLine, nextBegin, 6, out nextBegin).Trim();
                        strLine[2] = bl003.GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();

                        // 列長度檢查
                        for (int i = 0; i < strLine.Length; i++)
                        {
                            int num = i + 1;
                            if (StringUtil.IsEmpty(strLine[i]))
                                strErr += "第" + count.ToString() + "行第" + num.ToString() + "列為空;\\n";
                            else
                                strErr += CheckFileOneColumn(strLine[i], num, count);
                            if (!StringUtil.IsEmpty(strErr))
                            {
                                string[] arg = new string[1];
                                arg[0] = Factory_Name;
                                Warning.SetWarning(GlobalString.WarningType.PersoProjectChange, arg);
                                throw new AlertException(strErr);
                            }
                        }

                        // 將訊息新增到Table中
                        DataRow drIn = dtDataIn.NewRow();
                        drIn["Project_Date"] = Convert.ToDateTime(strLine[0].Substring(0, 4) + "/" + strLine[0].Substring(4, 2) + "/" + strLine[0].Substring(6, 2)).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                        drIn["Project_Code"] = strLine[1].ToString();
                        drIn["Number"] = Convert.ToInt32(strLine[2]);
                        dtDataIn.Rows.Add(drIn);
                    }
                }
                count++;
            }
            #endregion 讀字符串，并檢查字符串格式(列數、每列的字符格式),并保存到臨時DataTable(dtDataIn)中

            #region 獲得perso廠對應的所有特殊代製項目訊息
            dirValues.Clear();
            dirValues.Add("Factory_RID", strFactoryRID);
            DataSet dstp = dao.GetList(SEL_SPECIAL_PROJECT,dirValues);
            #endregion 獲得perso廠對應的所有特殊代製項目訊息

            string strError = "";

            #region 檢查特殊項目作業日期當天是否有特殊項目            
            for (int i = 0; i < dtDataIn.Rows.Count; i++)
            {
                dirValues.Clear();
                dirValues.Add("Factory_RID", strFactoryRID);
                dirValues.Add("Project_Date",dtDataIn.Rows[i]["Project_Date"].ToString());
                int iscount = Convert.ToInt32(dao.GetList(CON_SPECIAL_PERSO_PROJECT_IMPORT, dirValues).Tables[0].Rows[0][0].ToString());
                if (iscount > 0)
                {
                    strError += "第" + Convert.ToString(i + 1) + "行的特殊項目已經存在，不可重複匯入。\\n";
                }
            }
            #endregion 檢查特殊項目作業日期當天是否有特殊項目
            
            #region 檢查特殊代製項目編號是否存在
            if (dstp != null && dstp.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dtDataIn.Rows.Count; i++)
                {
                    DataRow[] drs = dstp.Tables[0].Select("project_code='" + dtDataIn.Rows[i]["Project_Code"].ToString() + "'");
                    if (drs.Length == 0)
                    {
                        strError += "第" + Convert.ToString(i + 1) + "行的特殊代製項目不存在。\\n";
                    }
                }
            }
            else
            {
                for (int i = 0; i < dtDataIn.Rows.Count; i++)
                {
                    strError += "第" + Convert.ToString(i + 1) + "行的特殊代製項目不存在。\\n";
                }
            }
            #endregion 檢查特殊代製項目編號是否存在

            #region 檢查Perso廠磁條信用卡是否已經有請款，有請款的不能再匯入
            for (int i = 0; i < dtDataIn.Rows.Count; i++)
            {
                dirValues.Clear();
                dirValues.Add("perso_factory_rid", strFactoryRID);
                dirValues.Add("in_date", Convert.ToDateTime(dtDataIn.Rows[i]["Project_Date"]).ToString("yyyy/MM/dd 12:00:00"));
                int iscount = Convert.ToInt32(dao.GetList(CON_ASKED_MONEY_CT_IN, dirValues).Tables[0].Rows[0][0].ToString());
                if (iscount > 0)
                {
                    strError += "第" + Convert.ToString(i + 1) + "行的特殊項目。磁條信用卡群組已經請款，不可再匯入。\\n";
                }
            }
            #endregion 檢查Perso廠磁條信用卡是否已經有請款，有請款的不能再匯入

            if (!StringUtil.IsEmpty(strError))
            {
                string[] arg = new string[1];
                arg[0] = Factory_Name;
                Warning.SetWarning(GlobalString.WarningType.PersoProjectChange, arg);
                throw new AlertException(strError);
            }

            dstDataIn = new DataSet();
            dstDataIn.Tables.Add(dtFactory);
            dstDataIn.Tables.Add(dtDataIn);

            return dstDataIn;

        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception("匯入資料時出錯誤");
        }
        finally
        {
            // 關閉文件
            if (null != sr)
            {
                sr.Close();
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
    private string CheckFileOneColumn(string strColumn, int num, int count)
    {
        string strErr = "";
        string Pattern = "";
        MatchCollection Matches;
        switch (num)
        {            
            case 1:
                if (strColumn.Length != 8)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列時間格式不對;\\n";
                    break;
                }

                string str1 = strColumn.Substring(0, 4) + "/" + strColumn.Substring(4, 2) + "/" + strColumn.Substring(6, 2);
                try
                {
                    DateTime dt = Convert.ToDateTime(str1);
                }
                catch
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列時間格式不對;\\n";
                }
                break;
            case 2:
                Pattern = @"^[A-Z]\d{5}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為6位字符;\\n";
                }
                break;
            case 3:
                Pattern = @"^\d{9}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為9位以內數字;\\n";
                }
                break;
            default:
                break;
        }

        return strErr;
    }

    /// <summary>
    /// 以廠商ID取廠商
    /// </summary>
    /// <param name="Factory_RID">廠商RID</param>
    /// <returns></returns>
    private DataSet getFactoryByID(string Factory_ID)
    {
        DataSet dstFactory = null;
        try
        {
            dirValues.Clear();

            string strSql = SEL_PERSON_BY_PERSONID;
                        
            dirValues.Add("Factory_ID", Factory_ID);

            dstFactory = dao.GetList(strSql, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstFactory;
    }

    /// <summary>
    /// 以廠商RID取廠商名稱
    /// </summary>
    /// <param name="Factory_RID">廠商RID</param>
    /// <returns></returns>
    private string getFactoryNameByRID(string Factory_RID)
    {
        FACTORY mFACTORY = null;
        try
        {
            mFACTORY = dao.GetModel<FACTORY, int>("RID", int.Parse(Factory_RID));
            return mFACTORY.Factory_ShortName_CN;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 以廠商RID取廠商編號
    /// </summary>
    /// <param name="Factory_RID">廠商RID</param>
    /// <returns></returns>
    private string getFactoryIDByRID(string Factory_RID)
    {
        FACTORY mFACTORY = null;
        try
        {
            mFACTORY = dao.GetModel<FACTORY, int>("RID", int.Parse(Factory_RID));
            return mFACTORY.Factory_ID;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    private int getProjectRIDByID(string Project_Code)
    {
        PERSO_PROJECT mPROJECT = null;
        try
        {
            mPROJECT = dao.GetModel<PERSO_PROJECT, string>("Project_Code", Project_Code);
            return mPROJECT.RID;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 根據年月獲得最後一個工作日的日期
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    public DateTime getWorkDate(string year,string month)
    {
        DataSet dstWorkDate = null;
        try
        {
            dirValues.Clear();

            string strSql = SEL_WORK_DATE;

            dirValues.Add("year", year);
            dirValues.Add("month", month);

            dstWorkDate = dao.GetList(strSql, dirValues);
            if (dstWorkDate != null && 
                dstWorkDate.Tables.Count>0 && 
                dstWorkDate.Tables[0].Rows.Count > 0)
            {
                return Convert.ToDateTime(dstWorkDate.Tables[0].Rows[0]["date_time"].ToString());
            }
            else
            {
                throw new AlertException("當月沒有設置工作日！");
            }
        }
        catch (AlertException ae)
        {
            throw ae;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 保存匯入資料
    /// </summary>
    /// <param name="ds">匯入資料（table1：perso廠信息，table2：匯入信息）</param>
    public void SaveSpecialIn(DataSet ds, string strFileName)
    {
        try
        {
            dao.OpenConnection();

            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                SPECIAL_PERSO_PROJECT_IMPORT sppiModel = new SPECIAL_PERSO_PROJECT_IMPORT();
                sppiModel.Perso_Factory_RID = Convert.ToInt32(ds.Tables[0].Rows[0]["Factory_RID"].ToString());
                sppiModel.PersoProject_RID = getProjectRIDByID(ds.Tables[1].Rows[i]["project_code"].ToString());
                sppiModel.Project_Date = Convert.ToDateTime(ds.Tables[1].Rows[i]["Project_Date"].ToString());
                sppiModel.Number = Convert.ToInt64(ds.Tables[1].Rows[i]["Number"].ToString());

                dao.Add<SPECIAL_PERSO_PROJECT_IMPORT>(sppiModel,"RID");
            }

            SetOprLog("11");

            InOut006BL bl1 = new InOut006BL();
            bl1.dao = dao;
            bl1.AddLog("3", strFileName);

            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(ex.Message);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 檢查按輸入的條件是否有匯入的特殊項目資料
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="month">月</param>
    /// <param name="strRID">perso廠RID</param>
    public bool ConSpecialIn(int year, int month, int strRID)
    {         
        try
        {
            DataSet dstmp = null;
            
            dirValues.Clear();
            dirValues.Add("year", year);
            dirValues.Add("month", month);
            dirValues.Add("Factory_RID", strRID);

            dstmp = dao.GetList(CON_SPECIAL_PERSO_PROJECT_IN, dirValues);

            if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dstmp.Tables[0].Rows[0][0].ToString()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("查詢失敗！");
        }
    }

    /// <summary>
    /// 檢查磁條信用卡群組是否已經請過款
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="month">月</param>
    /// <param name="strRID">perso廠RID</param>
    public bool ConAskedMoney(string year, string month, string strRID)
    {
        try
        {
            DataSet dstmp = null;

            dirValues.Clear();
            dirValues.Add("year", year);
            dirValues.Add("month", month);
            dirValues.Add("perso_factory_rid", strRID);

            dstmp = dao.GetList(CON_ASKED_MONEY_CT_DEL, dirValues);

            if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dstmp.Tables[0].Rows[0][0].ToString()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("查詢失敗！");
        }
    }

    /// <summary>
    /// 檢查卡種群組是否已經請過款
    /// </summary>
    /// <param name="date_asked_money"></param>
    /// <param name="perso_factory_rid"></param>
    /// <param name="card_group_rid"></param>
    /// <returns></returns>
    public bool ConAskedMoneyGroup(DateTime date_asked_money, 
                    string perso_factory_rid, 
                    string card_group_rid)
    {
        try
        {
            DataSet dstmp = null;

            dirValues.Clear();
            dirValues.Add("perso_factory_rid", perso_factory_rid);
            dirValues.Add("in_date", date_asked_money.ToString("yyyy/MM/dd"));
            dirValues.Add("cgrid", card_group_rid);

            dstmp = dao.GetList(CON_ASKED_MONEY_GROUP, dirValues);

            if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dstmp.Tables[0].Rows[0][0].ToString()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("查詢失敗！");
        }
    }

    /// <summary>
    /// 刪除匯入資料
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="month">月</param>
    /// <param name="strRID">perso廠RID</param>
    public void DelSpecialIn(int year,int month,int strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("year", year);
            dirValues.Add("month", month);
            dirValues.Add("Factory_RID", strRID);
            //删除特殊代製费用是，应当检查当月是否已请款，如果当月已经请款，无法删除，提示"汇入月份已请款"

            dao.GetList(DEL_SPECIAL_PERSO_PROJECT_IN, dirValues);

            SetOprLog("13");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("刪除資料失敗！");
        }
    }

    /// <summary>
    /// 查詢人工輸入例外代製項目明細
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet SearchExcepProject(Dictionary<string, object> searchInput,string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? " Project_Date" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_EXCEPTION_PERSO_PROJECT);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();

        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["Card_Group_RID"].ToString().Trim()))
            {
                stbWhere.Append(" AND EPP.CardGroup_RID = @Card_Group_RID");
                dirValues.Add("Card_Group_RID", searchInput["Card_Group_RID"].ToString().Trim());
            }           
            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                dirValues.Add("Factory_RID", searchInput["dropFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropYear"].ToString().Trim()))
            {
                dirValues.Add("year", searchInput["dropYear"].ToString().Trim());
            }    
            if (!StringUtil.IsEmpty(searchInput["dropMonth"].ToString().Trim()))
            {
                dirValues.Add("month", searchInput["dropMonth"].ToString().Trim());
            }           
        }

        //執行SQL語句
        DataSet dsEP = null;
        try
        {
            dsEP = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dsEP;
    }

    /// <summary>
    /// 刪除例外代製項目
    /// </summary>
    /// <param name="strRID">例外代製項目RID</param>
    public void DelExcepProject(int strRID)
    { 
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", strRID);
            SetOprLog("4");
            dao.GetList(DEL_EXCEPTION_PERSO_PROJECT, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("刪除資料失敗！");
        }
    }

    /// <summary>
    /// 修改例外代製項目
    /// </summary>
    /// <param name="strRID">例外代製項目RID</param>
    /// <param name="searchInput">例外代製項目信息</param>
    public void UpdateExcepProject(int strRID, Dictionary<string, object> searchInput)
    {
        try
        {
            EXCEPTION_PERSO_PROJECT eppModel = dao.GetModel<EXCEPTION_PERSO_PROJECT, int>("RID", strRID);
            if (ConAskedMoneyGroup(eppModel.Project_Date,
                            eppModel.Perso_Factory_RID.ToString(),
                            eppModel.CardGroup_RID.ToString()))
                throw new AlertException("該時間內已經有代製費用請款，不能修改！");

            eppModel.Project_Date = Convert.ToDateTime(searchInput["Project_Date"].ToString());
            eppModel.CardGroup_RID = Convert.ToInt32(searchInput["CardGroup_RID"].ToString());
            eppModel.Perso_Factory_RID = Convert.ToInt32(searchInput["Perso_Factory_RID"].ToString());
            eppModel.Name = searchInput["Name"].ToString();
            eppModel.Number = Convert.ToInt64(searchInput["Number"].ToString());
            eppModel.Unit_Price = Convert.ToDecimal(searchInput["Unit_Price"].ToString());
            eppModel.Comment = searchInput["Comment"].ToString();
            SetOprLog("3");
            dao.Update<EXCEPTION_PERSO_PROJECT>(eppModel, "RID");
        }
        catch (AlertException ae)
        {
            throw ae;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("修改資料失敗！");
        }
    }

    /// <summary>
    /// 新增例外代製項目
    /// </summary>    
    /// <param name="searchInput">例外代製項目信息</param>
    public int SaveExcepProject(Dictionary<string, object> searchInput)
    {
        try
        {
            EXCEPTION_PERSO_PROJECT eppModel = new EXCEPTION_PERSO_PROJECT();
            eppModel.Project_Date = Convert.ToDateTime(searchInput["Project_Date"].ToString());
            eppModel.CardGroup_RID = Convert.ToInt32(searchInput["CardGroup_RID"].ToString());
            eppModel.Perso_Factory_RID = Convert.ToInt32(searchInput["Perso_Factory_RID"].ToString());
            eppModel.Name = searchInput["Name"].ToString();
            eppModel.Number = Convert.ToInt64(searchInput["Number"].ToString());
            eppModel.Unit_Price = Convert.ToDecimal(searchInput["Unit_Price"].ToString());
            eppModel.Comment = searchInput["Comment"].ToString();
            SetOprLog("2");
            return Convert.ToInt32(dao.AddAndGetID<EXCEPTION_PERSO_PROJECT>(eppModel, "RID"));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("新增資料失敗！");
        }
    }

    /// <summary>
    /// 獲得例外代製項目Model
    /// </summary>
    /// <param name="strRID">例外代製項目RID</param>
    /// <returns></returns>
    public EXCEPTION_PERSO_PROJECT getExcepProject(int strRID)
    {
        try
        {
            EXCEPTION_PERSO_PROJECT eppModel = new EXCEPTION_PERSO_PROJECT();
            eppModel = dao.GetModel<EXCEPTION_PERSO_PROJECT, int>("RID", strRID);
            return eppModel;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// 查詢代製費用帳務異動明細
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet SearchChangeProject(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? " Project_Date" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_PERSO_PROJECT_CHANGE_DETAIL);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();

        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["Card_Group_RID"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPCD.CardGroup_RID = @Card_Group_RID");
                dirValues.Add("Card_Group_RID", searchInput["Card_Group_RID"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                dirValues.Add("Factory_RID", searchInput["dropFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropYear"].ToString().Trim()))
            {
                dirValues.Add("year", searchInput["dropYear"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropMonth"].ToString().Trim()))
            {
                dirValues.Add("month", searchInput["dropMonth"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dsCP = null;
        try
        {
            dsCP = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dsCP;
    }

    /// <summary>
    /// 新增代製費用帳務異動
    /// </summary>    
    /// <param name="searchInput">代製費用帳務異動信息</param>
    public int SaveChangeProject(Dictionary<string, object> searchInput)
    {
        try
        {
            PERSO_PROJECT_CHANGE_DETAIL ppcdModel = new PERSO_PROJECT_CHANGE_DETAIL();
            ppcdModel.Project_Date = Convert.ToDateTime(searchInput["Project_Date"].ToString());
            ppcdModel.CardGroup_RID = Convert.ToInt32(searchInput["CardGroup_RID"].ToString());
            ppcdModel.Perso_Factory = Convert.ToInt32(searchInput["Perso_Factory_RID"].ToString());
            ppcdModel.Param_Code = searchInput["Name"].ToString();           
            ppcdModel.Price = Convert.ToDecimal(searchInput["Unit_Price"].ToString());
            ppcdModel.Comment = searchInput["Comment"].ToString();
            SetOprLog("2");
            return Convert.ToInt32(dao.AddAndGetID<PERSO_PROJECT_CHANGE_DETAIL>(ppcdModel, "RID"));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("新增資料失敗！");
        }
    }

    /// <summary>
    /// 修改代製費用帳務異動
    /// </summary>
    /// <param name="strRID">代製費用帳務異動RID</param>
    /// <param name="searchInput">代製費用帳務異動信息</param>
    public void UpdateChangeProject(int strRID, Dictionary<string, object> searchInput)
    {
        try
        {
            PERSO_PROJECT_CHANGE_DETAIL ppcdModel = dao.GetModel<PERSO_PROJECT_CHANGE_DETAIL, int>("RID", strRID);
            if (ConAskedMoneyGroup(ppcdModel.Project_Date, 
                    ppcdModel.CardGroup_RID.ToString(), 
                    ppcdModel.Perso_Factory.ToString()))
                throw new AlertException("當月已經有請款不能修改代製費用帳務異動。");

            ppcdModel.Project_Date = Convert.ToDateTime(searchInput["Project_Date"].ToString());
            ppcdModel.CardGroup_RID = Convert.ToInt32(searchInput["CardGroup_RID"].ToString());
            ppcdModel.Perso_Factory = Convert.ToInt32(searchInput["Perso_Factory_RID"].ToString());
            ppcdModel.Param_Code = searchInput["Name"].ToString();
            ppcdModel.Price = Convert.ToDecimal(searchInput["Unit_Price"].ToString());
            ppcdModel.Comment = searchInput["Comment"].ToString();
            SetOprLog("3");
            dao.Update<PERSO_PROJECT_CHANGE_DETAIL>(ppcdModel, "RID");
        }
        catch (AlertException ae)
        {
            throw ae;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("修改資料失敗！");
        }
    }

    /// <summary>
    /// 刪除代製費用帳務異動
    /// </summary>
    /// <param name="strRID">代製費用帳務異動RID</param>
    public void DelChangeProject(int strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", strRID);
            SetOprLog("4");
            dao.GetList(DEL_PERSO_PROJECT_CHANGE_DETAIL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception("刪除資料失敗！");
        }
    }

    /// <summary>
    /// 獲得代製費用帳務異動Model
    /// </summary>
    /// <param name="strRID">代製費用帳務異動RID</param>
    /// <returns></returns>
    public PERSO_PROJECT_CHANGE_DETAIL getChangeProject(int strRID)
    {
        try
        {
            PERSO_PROJECT_CHANGE_DETAIL ppcdModel = new PERSO_PROJECT_CHANGE_DETAIL();
            ppcdModel = dao.GetModel<PERSO_PROJECT_CHANGE_DETAIL, int>("RID", strRID);
            return ppcdModel;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(ex.Message);
        }
    }
}
