//*****************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改日期：2021-03-12
//*  修改記錄：新增次月下市預測表匯入 陳永銘
//*****************************************
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
/// <summary>
/// InOut008BL 的摘要说明
/// </summary>
public class InOut008BL : BaseLogic
{
    #region SQL語句
    const string SEL_IMPORT_PROJECT_RR05 = "SELECT Affinity FROM IMPORT_PROJECT_RR05 WHERE Is_Import = 'Y' ";
    const string DEL_PERSO_FORE_CHANGE_CARD_RR05 = "DELETE FROM PERSO_FORE_CHANGE_CARD_RR05 WHERE RST = 'A' AND Fore_RID IN (SELECT RID FROM FORE_CHANGE_CARD_RR05 WHERE RST = 'A' AND Delist_Date = @delist_date AND Type = @type AND Affinity = @affinity AND Photo = @photo AND Disney_Code = @disney_code) ";
    const string SEL_FORE_CHANGE_CARD_RR05 = "SELECT * FROM FORE_CHANGE_CARD_RR05 WHERE RST = 'A' AND Delist_Date = @delist_date AND Type = @type AND Affinity = @affinity AND Photo = @photo AND Disney_Code = @disney_code ";

    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y' ";

    //换卡预测 
    public const string CON_FORE_CHANGE_CARD = "SELECT COUNT(*) FROM FORE_CHANGE_CARD AS FCC WHERE FCC.RST = 'A'";

    public const string SEL_PERSO_FORE_CHANGE_CARD = "SELECT *,'' AS 耗用卡版面 FROM (SELECT CT.RID AS 卡種RID,PFCC.TYPE,PFCC.PHOTO,PFCC.AFFINITY,PFCC.Disney_Code,CT.NAME," +
            "CTH.NAME AS 換卡版面,CTT.NAME AS 替換卡版面,PFCC.Change_Date," +
            "F.RID AS FRID,F.Factory_ShortName_CN,SUM(PFCC.Number) AS Number " +
            "FROM PERSO_FORE_CHANGE_CARD PFCC " +
                "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND PFCC.TYPE = CT.TYPE AND PFCC.PHOTO = CT.PHOTO AND PFCC.AFFINITY = CT.AFFINITY " +
                "LEFT JOIN CARD_TYPE  CTH ON CTH.RST = 'A' AND CT.Change_Space_RID = CTH.RID AND CTH.Is_Using = 'Y' " +
                "LEFT JOIN CARD_TYPE CTT ON CTT.RST = 'A' AND CT.Replace_Space_RID = CTT.RID AND CTT.Is_Using = 'Y'  " +
                "INNER JOIN Factory F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND PFCC.Perso_Factory_RID = F.RID " +
            "WHERE PFCC.RST = 'A' AND PFCC.Change_Date=@change_date and PFCC.FORE_RID IN (select rid from FORE_CHANGE_CARD where ismonth='1')" +
            "GROUP BY CT.RID,PFCC.TYPE,PFCC.PHOTO,PFCC.AFFINITY,PFCC.Disney_Code,CT.NAME," +
            "CTH.NAME,CTT.NAME,PFCC.Change_Date," +
            "F.RID,F.Factory_ShortName_CN ) B ";

    public const string SEL_PERSO_FORE_CHANGE_CARD_RR05 = "SELECT *,'' AS 耗用卡版面 FROM (SELECT CT.RID AS 卡種RID,PFCC.TYPE,PFCC.PHOTO,PFCC.AFFINITY,PFCC.Disney_Code,CT.NAME," +
        "CTH.NAME AS 換卡版面,CTT.NAME AS 替換卡版面,PFCC.Delist_Date," +
        "F.RID AS FRID,F.Factory_ShortName_CN,SUM(PFCC.Number) AS Number " +
        "FROM PERSO_FORE_CHANGE_CARD_RR05 PFCC " +
            "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND PFCC.TYPE = CT.TYPE AND PFCC.PHOTO = CT.PHOTO AND PFCC.AFFINITY = CT.AFFINITY " +
            "LEFT JOIN CARD_TYPE  CTH ON CTH.RST = 'A' AND CT.Change_Space_RID = CTH.RID AND CTH.Is_Using = 'Y' " +
            "LEFT JOIN CARD_TYPE CTT ON CTT.RST = 'A' AND CT.Replace_Space_RID = CTT.RID AND CTT.Is_Using = 'Y'  " +
            "INNER JOIN Factory F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND PFCC.Perso_Factory_RID = F.RID " +
        "WHERE PFCC.RST = 'A' AND PFCC.Delist_Date=@delist_date and PFCC.FORE_RID IN (select rid from FORE_CHANGE_CARD_RR05 where ismonth='1')" +
        "GROUP BY CT.RID,PFCC.TYPE,PFCC.PHOTO,PFCC.AFFINITY,PFCC.Disney_Code,CT.NAME," +
        "CTH.NAME,CTT.NAME,PFCC.Delist_Date," +
        "F.RID,F.Factory_ShortName_CN ) B ";


    //换卡预测 
    public const string DEl_FORE_CHANGE_CARD = "DELETE	FROM FORE_CHANGE_CARD  WHERE RST = 'A'";

    //卡种
    public const string SEL_CARD_TYPE = "SELECT CT.RID	FROM CARD_TYPE AS CT WHERE CT.RST='A' ";

    public const string SEL_FILE_NAME = "SELECT File_Name FROM IMPORT_PROJECT WHERE RST = 'A' AND Type = '2' ";

    public const string SEL_CARDTYPE = "SELECT * FROM CARD_TYPE WHERE RST = 'A' ";

    public const string DEL_PERSO_FORE_CHANGE_CARD = "DELETE FROM PERSO_FORE_CHANGE_CARD WHERE RST = 'A' AND Fore_RID IN (SELECT RID FROM FORE_CHANGE_CARD WHERE RST = 'A' AND Change_Date = @change_date AND Type = @type AND Affinity = @affinity AND Photo = @photo) ";

    public const string DEL_FORE_CHANGE_CARD = "DELETE FROM FORE_CHANGE_CARD WHERE RST = 'A' AND Change_Date = @change_date AND Type = @type AND Affinity = @affinity AND Photo = @photo ";

    public const string SEL_FORE_CHANGE_CARD = "select * FROM FORE_CHANGE_CARD WHERE RST = 'A' AND Change_Date = @change_date AND Type = @type AND Affinity = @affinity AND Photo = @photo ";

    public const string SEL_CARDTYPE_PERSO = "SELECT PC.*,CT.TYPE,CT.AFFINITY,CT.PHOTO FROM PERSO_CARDTYPE PC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND PC.CardType_RID = CT.RID WHERE PC.RST = 'A' AND PC.CardType_RID = @cardtype_rid ORDER BY PC.Base_Special DESC,PC.Priority ASC ";

    public const string IN_CARD_MONTH_FORCAST_PRINT = "INSERT INTO RPT_InOut008  (Factory_ShortName_CN,Disney_Code,Card_Number,Name,Deplete_Name,Number,RCT) VALUES (@factory_shortname_cn,@Disney_Code,@card_number,@name,@deplete_name,@number,@RCT) ";

    public const string SEL_CARDTYPE_PERSO_SPECIAL_1 = "SELECT PC.* FROM PERSO_CARDTYPE PC WHERE PC.rst='A' and PC.CardType_RID = @cardtype_rid and PC.Percentage_Number = '1' ORDER BY PC.Priority desc";
    public const string SEL_CARDTYPE_PERSO_SPECIAL_2 = "SELECT PC.* FROM PERSO_CARDTYPE PC WHERE PC.rst='A' and PC.CardType_RID = @cardtype_rid and PC.Percentage_Number = '2' ORDER BY PC.Priority";

    public const string SEL_BATCH_MANAGE = "SELECT COUNT(*) FROM BATCH_MANAGE WHERE (RID = 3 OR RID = 6) AND Status = 'Y'";
    public string UPDATE_BATCH_MANAGE_START = "UPDATE BATCH_MANAGE SET Status = 'Y',RUU='InOut005BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 3 OR RID = 6)";
    public string UPDATE_BATCH_MANAGE_END = "UPDATE BATCH_MANAGE SET Status = 'N',RUU='InOut005BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 3 OR RID = 6)";
    #endregion
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public InOut008BL()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 將次月換卡預測匯入標設置為FALSE
    /// </summary>
    /// <returns></returns>
    public void MonthChangeForcastEnd()
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
    /// 檢查次月換卡預測刪除是否已經被開起。如果已經開起，返回FALSE
    ///                                     如果沒有開起，開起，并返回TRUE
    /// </summary>
    /// <returns></returns>
    public bool MonthChangeForcastStart()
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

    //厂商与卡种
    public DataSet GetPerso()
    {
        DataSet dstPerso = null;
        try
        {


            dstPerso = dao.GetList(SEL_PERSON);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstPerso;
    }

    //讀取文件中的數據到數據集中
    public DataSet DetailCheck(string strPath, ref string strErr)
    {
        DataSet dtsReturn = new DataSet();
        #region 驗證文件
        StreamReader sr = null;
        sr = new StreamReader(strPath, System.Text.Encoding.Default);
        DataSet dsCARDTYPE = null;
        DataSet dsIMPORTPROJECTRR05 = null;

        string NowTime = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "").ToString();

        DataTable dtblFileImp = new DataTable();
        dtblFileImp.Columns.Add("Old_Affinity_Code");
        dtblFileImp.Columns.Add("Name");
        dtblFileImp.Columns.Add("Disney_Code");
        dtblFileImp.Columns.Add("Photo_Code");
        dtblFileImp.Columns.Add("Type_Code");
        dtblFileImp.Columns.Add("Affinity_Code");
        dtblFileImp.Columns.Add("Card_SEQ");
        dtblFileImp.Columns.Add("Unit_Code");
        dtblFileImp.Columns.Add("Delist_Number");
        dtblFileImp.Columns.Add("Delist_Date");

        try
        {
            dsCARDTYPE = dao.GetList(SEL_CARDTYPE);
            dsIMPORTPROJECTRR05 = dao.GetList(SEL_IMPORT_PROJECT_RR05);
            string[] strLine;
            string[] strIMPORTPROJECTRR05;
            string strReadLine = "";
            int count = 0;
            int iErrLen = 0;
            string year_month = NowTime.Split('/')[0] + NowTime.Split('/')[1];

            strIMPORTPROJECTRR05 = new string[dsIMPORTPROJECTRR05.Tables[0].Rows.Count];
            for (int i = 0; i < dsIMPORTPROJECTRR05.Tables[0].Rows.Count; i++)
            {
                strIMPORTPROJECTRR05[i] = dsIMPORTPROJECTRR05.Tables[0].Rows[i]["Affinity"].ToString();
            }

            while ((strReadLine = sr.ReadLine()) != null)
            {
                count++;
                if (StringUtil.IsEmpty(strReadLine))
                    continue;

                string Pattern = @"\w+";
                MatchCollection Matches = Regex.Matches(strReadLine.Replace(",", ""), Pattern, RegexOptions.IgnoreCase);

                if (Matches.Count != 9)
                    continue;

                if (!strIMPORTPROJECTRR05.Contains(Matches[0].ToString()))
                    continue;

                DataRow dr = dtblFileImp.NewRow();
                strLine = new string[Matches.Count];
                for (int i = 0; i < Matches.Count; i++)
                {
                    strLine[i] = Matches[i].ToString();
                }


                for (int i = 0; i < strLine.Length; i++)
                {
                    int num = i + 1;
                    if (StringUtil.IsEmpty(strLine[i]))
                        strErr += "第" + count.ToString() + "行第" + num.ToString() + "列為空;\\n";
                    else
                        strErr += CheckFileColumn(strLine[i], num, count);
                    dr[i] = strLine[i];
                }

                dr[strLine.Length] = year_month;

                if (dtblFileImp.Select("Type_Code='" + dr["Type_Code"].ToString() + "' AND Affinity_Code = '" + dr["Affinity_Code"].ToString() + "' AND Photo_Code = '" + dr["Photo_Code"].ToString() + "' AND Disney_Code = '" + dr["Disney_Code"].ToString() + "' ").Length > 0)
                {
                    strErr += "第" + count.ToString() + "行 " + dr["Photo_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Type_Code"].ToString();
                    strErr += "對應的卡種已經存在,不能重複匯入!\\n";
                }

                if (dsCARDTYPE.Tables[0].Select("TYPE='" + dr["Type_Code"].ToString() + "' AND AFFINITY = '" + dr["Affinity_Code"].ToString() + "' AND PHOTO = '" + dr["Photo_Code"].ToString() + "' ").Length == 0)
                {
                    strErr += "第" + count.ToString() + "行 " + dr["Photo_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Type_Code"].ToString();
                    strErr += "對應的卡種不存在!\\n";
                }
                else
                {

                    if (dsCARDTYPE.Tables[0].Select("TYPE='" + dr["Type_Code"].ToString() + "' AND AFFINITY = '" + dr["Affinity_Code"].ToString() + "' AND PHOTO = '" + dr["Photo_Code"].ToString() + "' AND Is_Using = 'N' ").Length >= 1)
                    {
                        strErr += "第" + count.ToString() + "行 " + dr["Photo_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Type_Code"].ToString();
                        strErr += "對應的卡種已停用!\\n";
                    }


                    if (dsCARDTYPE.Tables[0].Select("Begin_Time<='" + NowTime + "' AND (End_Time='1900-01-01' or End_Time >='" + NowTime + "') AND TYPE='" + dr["Type_Code"].ToString() + "' AND AFFINITY = '" + dr["Affinity_Code"].ToString() + "' AND PHOTO = '" + dr["Photo_Code"].ToString() + "' ").Length == 0)
                    {
                        strErr += "第" + count.ToString() + "行 " + dr["Photo_Code"].ToString() + "-" + dr["Affinity_Code"].ToString() + "-" + dr["Type_Code"].ToString();
                        strErr += "第" + count.ToString() + "行對應的卡種不在有效期內!;\\n";
                    }

                }

                //檢查這一行是否有錯誤信息！
                if (strErr.Length > iErrLen)
                {
                    iErrLen = strErr.Length;
                }
                else
                {
                    dtblFileImp.Rows.Add(dr);
                }

            }

            dtsReturn.Tables.Add(dtblFileImp);

            if (!StringUtil.IsEmpty(strErr))
            {
                //throw new AlertException(strErr);
            }

        }
        catch (AlertException ex)
        {
            // 文件格式不正確時，報警示
            object[] arg = new object[1];
            arg[0] = ex.Message;
            Warning.SetWarning(GlobalString.WarningType.MonthChangeCardForeCast, arg);

            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            sr.Close();
        }
        #endregion
        return dtsReturn;
    }

    //换卡预测中的个数
    public int CheckDuplicate(DataRow drfore_change_card)
    {
        DataSet dstfore_change_card = null;
        try
        {
            StringBuilder stbCommand = new StringBuilder(CON_FORE_CHANGE_CARD);
            StringBuilder stbWhere = new StringBuilder();
            //  foreach (DataRow dr_card_change in dtfore_change_card.Rows)
            {
                dirValues.Clear();
                stbWhere.Append(" and FCC.Card_Type =@card_type");
                dirValues.Add("card_type", drfore_change_card["Card_Type"]);

                stbWhere.Append(" and FCC.Photo_Type =@phtoto_type");
                dirValues.Add("phtoto_type", drfore_change_card["Photo_Type"]);

                stbWhere.Append(" and FCC.Affinity_Code =@affinity_code");
                dirValues.Add("affinity_code", drfore_change_card["Affinity_Code"]);

                stbWhere.Append(" and FCC.Change_Date =@change_date");
                dirValues.Add("change_date", drfore_change_card["Change_Date"]);
            }
            dstfore_change_card = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }

        return Convert.ToInt32(dstfore_change_card.Tables[0].Rows[0][0]);
    }
    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="dtfore_change_card"></param>
    public void DeleteDuplicate(DataRow drfore_change_card)
    {
        try
        {
            dao.OpenConnection();
            StringBuilder stbWhere = new StringBuilder();
            //  foreach (DataRow drfore_change_card in dtfore_change_card.Rows)
            {
                dirValues.Clear();
                stbWhere.Append(" and Card_Type =@card_type");
                dirValues.Add("card_type", drfore_change_card["Card_Type"]);

                stbWhere.Append(" and Photo_Type =@phtoto_type");
                dirValues.Add("phtoto_type", drfore_change_card["Photo_Type"]);

                stbWhere.Append(" and Affinity_Code =@affinity_code");
                dirValues.Add("affinity_code", drfore_change_card["Affinity_Code"]);

                stbWhere.Append(" and Change_Date =@change_date");
                dirValues.Add("change_date", drfore_change_card["Change_Date"]);
                dao.ExecuteNonQuery(DEl_FORE_CHANGE_CARD.ToString() + stbWhere.ToString(), dirValues, false);
                dao.Commit();
            }
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
    //获取卡种表
    public DataTable GetCardType(int Type, int Photo, int Affinity)
    {
        DataSet dstCardType = null;
        try
        {
            StringBuilder stbCommand = new StringBuilder(SEL_CARD_TYPE);
            StringBuilder stbWhere = new StringBuilder();
            dirValues.Clear();
            stbWhere.Append(" and CT.TYPE =@type");
            dirValues.Add("type", Type);

            stbWhere.Append(" and CT.PHOTO =@phtoto");
            dirValues.Add("phtoto", Photo);

            stbWhere.Append(" and CT.AFFINITY =@affinity");
            dirValues.Add("affinity", Affinity);
            dstCardType = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }

        return dstCardType.Tables[0];

    }

    /// <summary>
    /// 刪除PERSO_FORE_CHANGE_CARD_RR05檔中對應的記錄
    /// </summary>
    private void Del_PERSO_FORE_CHANGE_CARD_RR05(string type, string affinity, string photo, string delistdate, string disneycode)
    {
        dirValues.Clear();
        dirValues.Add("type", type);
        dirValues.Add("affinity", affinity);
        dirValues.Add("photo", photo);
        dirValues.Add("delist_date", delistdate);
        dirValues.Add("disney_code", disneycode);
        dao.ExecuteNonQuery(DEL_PERSO_FORE_CHANGE_CARD_RR05, dirValues);
    }

    /// <summary>
    /// 將匯入資料添加到數據庫中
    /// </summary>
    /// <returns></returns>
    public void In(DataSet dsFileImp, string strFileName)
    {
        FORE_CHANGE_CARD_RR05 fccrModel = null;
        DataSet dsCARDTYPE = null;

        try
        {
            dao.OpenConnection();

            foreach (DataRow dr_date in dsFileImp.Tables[0].Rows)
            {
                fccrModel = new FORE_CHANGE_CARD_RR05();

                Del_PERSO_FORE_CHANGE_CARD_RR05(dr_date["Type_Code"].ToString(), dr_date["Affinity_Code"].ToString(), dr_date["Photo_Code"].ToString(), dr_date["Delist_Date"].ToString(), dr_date["Disney_Code"].ToString());
                int intRID = 0;

                if (dao.GetList(SEL_FORE_CHANGE_CARD_RR05, dirValues).Tables[0].Rows.Count > 0)
                {
                    fccrModel = dao.GetModel<FORE_CHANGE_CARD_RR05>(SEL_FORE_CHANGE_CARD_RR05, dirValues);
                    fccrModel.Number = dr_date["Delist_Number"].ToString();
                    fccrModel.IsMonth = "1";
                    dao.Update<FORE_CHANGE_CARD_RR05>(fccrModel, "RID");
                    intRID = fccrModel.RID;
                }
                else
                {
                    //添加次月下市預測訊息。Dao.add(),并取出新添加記錄的RID
                    fccrModel.Delist_Date = dr_date["Delist_Date"].ToString();
                    fccrModel.Old_Affinity = dr_date["Old_Affinity_Code"].ToString();
                    fccrModel.Name = dr_date["Name"].ToString();
                    fccrModel.Disney_Code = dr_date["Disney_Code"].ToString();
                    fccrModel.Type = dr_date["Type_Code"].ToString();
                    fccrModel.Photo = dr_date["Photo_Code"].ToString();
                    fccrModel.Affinity = dr_date["Affinity_Code"].ToString();
                    fccrModel.Card_SEQ = dr_date["Card_SEQ"].ToString();
                    fccrModel.Unit_Code = dr_date["Unit_Code"].ToString();
                    fccrModel.Number = dr_date["Delist_Number"].ToString();
                    fccrModel.IsMonth = "1";
                    fccrModel.IsYear = "2";
                    intRID = Convert.ToInt32(dao.AddAndGetID<FORE_CHANGE_CARD_RR05>(fccrModel, "RID"));
                }

                dsCARDTYPE = dao.GetList(SEL_CARDTYPE + "AND Type = @type AND Affinity = @affinity AND Photo = @photo", dirValues);
                foreach (DataRow dr1 in dsCARDTYPE.Tables[0].Rows)
                {
                    SplitToPerso(Convert.ToInt32(dr1["RID"]), intRID, Convert.ToInt64(dr_date["Delist_Number"].ToString()), dr_date["Delist_Date"].ToString(), fccrModel);
                }
            }
            SetOprLog("11");

            InOut006BL bl1 = new InOut006BL();
            bl1.dao = dao;
            bl1.AddLog("5", strFileName);

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
    public void SplitToPerso(int Space_RID, int intRID, long DtDate_Number, string StartDate, FORE_CHANGE_CARD_RR05 Table_Vaule)
    {
        PERSO_FORE_CHANGE_CARD_RR05 pfccrModel = new PERSO_FORE_CHANGE_CARD_RR05();
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
                    pfccrModel.Delist_Date = StartDate;
                    pfccrModel.Type = dsCARDTYPE_PERSO.Tables[0].Rows[0]["Type"].ToString();
                    pfccrModel.Affinity = dsCARDTYPE_PERSO.Tables[0].Rows[0]["Affinity"].ToString();
                    pfccrModel.Photo = dsCARDTYPE_PERSO.Tables[0].Rows[0]["Photo"].ToString();
                    pfccrModel.Perso_Factory_RID = Convert.ToInt32(dsCARDTYPE_PERSO.Tables[0].Rows[0]["Factory_RID"].ToString());
                    pfccrModel.Number = DtDate_Number.ToString();
                    pfccrModel.Fore_RID = intRID;
                    pfccrModel.Old_Affinity = Table_Vaule.Old_Affinity;
                    pfccrModel.Name = Table_Vaule.Name;
                    pfccrModel.Disney_Code = Table_Vaule.Disney_Code;
                    pfccrModel.Card_SEQ = Table_Vaule.Card_SEQ;
                    pfccrModel.Unit_Code = Table_Vaule.Unit_Code;
                    dao.Add<PERSO_FORE_CHANGE_CARD_RR05>(pfccrModel, "RID");
                }
                else
                {
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
                                pfccrModel.Delist_Date = StartDate;
                                pfccrModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString();
                                pfccrModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString();
                                pfccrModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString();
                                pfccrModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                pfccrModel.Number = Convert.ToInt64(Math.Floor(DtDate_Number * (Convert.ToDouble(drCARDTYPE_PERSO[int1]["Value"]) / 100))).ToString();
                                pfccrModel.Fore_RID = intRID;
                                pfccrModel.Old_Affinity = Table_Vaule.Old_Affinity;
                                pfccrModel.Name = Table_Vaule.Name;
                                pfccrModel.Disney_Code = Table_Vaule.Disney_Code;
                                pfccrModel.Card_SEQ = Table_Vaule.Card_SEQ;
                                pfccrModel.Unit_Code = Table_Vaule.Unit_Code;
                                dao.Add<PERSO_FORE_CHANGE_CARD_RR05>(pfccrModel, "RID");
                            }
                            else
                            {
                                pfccrModel.Delist_Date = StartDate;
                                pfccrModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString();
                                pfccrModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString();
                                pfccrModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString();
                                pfccrModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                pfccrModel.Number = (DtDate_Number - intNumber).ToString();
                                pfccrModel.Fore_RID = intRID;
                                pfccrModel.Old_Affinity = Table_Vaule.Old_Affinity;
                                pfccrModel.Name = Table_Vaule.Name;
                                pfccrModel.Disney_Code = Table_Vaule.Disney_Code;
                                pfccrModel.Card_SEQ = Table_Vaule.Card_SEQ;
                                pfccrModel.Unit_Code = Table_Vaule.Unit_Code;
                                dao.Add<PERSO_FORE_CHANGE_CARD_RR05>(pfccrModel, "RID");
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
                                    pfccrModel.Delist_Date = StartDate;
                                    pfccrModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString(); ;
                                    pfccrModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString(); ;
                                    pfccrModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString(); ;
                                    pfccrModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                    pfccrModel.Number = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Value"]).ToString();
                                    pfccrModel.Fore_RID = intRID;
                                    pfccrModel.Old_Affinity = Table_Vaule.Old_Affinity;
                                    pfccrModel.Name = Table_Vaule.Name;
                                    pfccrModel.Disney_Code = Table_Vaule.Disney_Code;
                                    pfccrModel.Card_SEQ = Table_Vaule.Card_SEQ;
                                    pfccrModel.Unit_Code = Table_Vaule.Unit_Code;
                                    dao.Add<PERSO_FORE_CHANGE_CARD_RR05>(pfccrModel, "RID");
                                }
                                else
                                {
                                    pfccrModel.Delist_Date = StartDate;
                                    pfccrModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString(); ;
                                    pfccrModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString(); ;
                                    pfccrModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString(); ;
                                    pfccrModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                    pfccrModel.Number = (DtDate_Number - intNumber).ToString();
                                    pfccrModel.Fore_RID = intRID;
                                    pfccrModel.Old_Affinity = Table_Vaule.Old_Affinity;
                                    pfccrModel.Name = Table_Vaule.Name;
                                    pfccrModel.Disney_Code = Table_Vaule.Disney_Code;
                                    pfccrModel.Card_SEQ = Table_Vaule.Card_SEQ;
                                    pfccrModel.Unit_Code = Table_Vaule.Unit_Code;
                                    dao.Add<PERSO_FORE_CHANGE_CARD_RR05>(pfccrModel, "RID");
                                    break;
                                }
                            }
                            else
                            {
                                pfccrModel.Delist_Date = StartDate;
                                pfccrModel.Type = drCARDTYPE_PERSO[int1]["Type"].ToString(); ;
                                pfccrModel.Affinity = drCARDTYPE_PERSO[int1]["Affinity"].ToString(); ;
                                pfccrModel.Photo = drCARDTYPE_PERSO[int1]["Photo"].ToString(); ;
                                pfccrModel.Perso_Factory_RID = Convert.ToInt32(drCARDTYPE_PERSO[int1]["Factory_RID"].ToString());
                                pfccrModel.Number = (DtDate_Number - intNumber).ToString();
                                pfccrModel.Fore_RID = intRID;
                                pfccrModel.Old_Affinity = Table_Vaule.Old_Affinity;
                                pfccrModel.Name = Table_Vaule.Name;
                                pfccrModel.Disney_Code = Table_Vaule.Disney_Code;
                                pfccrModel.Card_SEQ = Table_Vaule.Card_SEQ;
                                pfccrModel.Unit_Code = Table_Vaule.Unit_Code;
                                dao.Add<PERSO_FORE_CHANGE_CARD_RR05>(pfccrModel, "RID");
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

    //新增
    public void Add(FORE_CHANGE_CARD fccModel)
    {

        try
        {
            //事務開始
            dao.OpenConnection();

            dao.Add<FORE_CHANGE_CARD>(fccModel, "RID");

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }

    }

    /// 查詢次月換卡預測檔主記錄列表
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
        string strSortField = (sortField == "null" ? "type,affinity,photo" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_PERSO_FORE_CHANGE_CARD_RR05);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropChange_Date"].ToString().Trim()))
            {
                dirValues.Add("delist_date", searchInput["dropChange_Date"].ToString());
            }

            if (searchInput["dropFactory_Name"].ToString() != "")
            {
                stbWhere.Append(" WHERE FRID= @Factory_Name ");
                dirValues.Add("Factory_Name", searchInput["dropFactory_Name"].ToString());
            }
        }

        //執行SQL語句
        DataSet dstfore_change_card = null;
        DataRow[] drows = null;
        try
        {
            dstfore_change_card = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);

            foreach (DataRow dr in dstfore_change_card.Tables[0].Rows)
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


            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                drows = dstfore_change_card.Tables[0].Select("NAME like '%" + searchInput["txtName"].ToString().Trim() + "%' ");

                if (drows.Length != 0)
                {
                    dstfore_change_card = DataRows_Convert_DataSet(drows, ref dstfore_change_card);
                }
                else
                {
                    dstfore_change_card = null;
                }
            }

            if (!StringUtil.IsEmpty(searchInput["txtDeplete"].ToString().Trim()) && dstfore_change_card != null)
            {
                drows = dstfore_change_card.Tables[0].Select("耗用卡版面 like '%" + searchInput["txtDeplete"].ToString().Trim() + "%' ");

                if (drows.Length != 0)
                {
                    dstfore_change_card = DataRows_Convert_DataSet(drows, ref dstfore_change_card);
                }
                else
                {
                    dstfore_change_card = null;
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstfore_change_card;
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
            case 4:
            case 7:
                Pattern = @"^\d{2}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為2位數字;\n";
                }
                break;
            case 5:
                Pattern = @"^\d{3}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為3位數字;\n";
                }

                break;
            case 1:
            case 6:
                Pattern = @"^\d{4}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為4位數字;\n";
                }
                break;
            case 8:
                Pattern = @"^\d{5}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為5位數字;\n";
                }
                break;
            case 9:
                Pattern = @"^\d{7}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為7位數字;\n";
                }
                break;
            case 3:
                Pattern = @"^[a-zA-Z0-9]*$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0 || strColumn.Length != 2)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為2位英數;\n";
                }
                break;
        }

        return strErr;
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

    //汇出表格时新增數據到數據庫
    public void ADD_CARD_YEAR_FORCAST_PRINT(DataSet dstFORE_CHANGE_CARD, string time)
    {
        dao.ExecuteNonQuery("delete RPT_InOut008 where RCT<" + DateTime.Now.ToString("yyyyMMdd000000"));
        try
        {
            foreach (DataRow dr in dstFORE_CHANGE_CARD.Tables[0].Rows)
            {
                dirValues.Clear();
                dirValues.Add("factory_shortname_cn", dr["Factory_ShortName_CN"].ToString());
                dirValues.Add("Disney_Code", dr["Disney_Code"].ToString());
                dirValues.Add("card_number", dr["TYPE"].ToString() + "-" + dr["AFFINITY"].ToString() + "-" + dr["PHOTO"].ToString());
                dirValues.Add("name", dr["NAME"].ToString());
                dirValues.Add("deplete_name", dr["耗用卡版面"].ToString());
                dirValues.Add("number", dr["Number"].ToString());
                dirValues.Add("RCT", time);
                dao.ExecuteNonQuery(IN_CARD_MONTH_FORCAST_PRINT, dirValues);
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
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