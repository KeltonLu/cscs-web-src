//******************************************************************
//*  作    者：JunWang
//*  功能說明：請款及總行會計付款資訊作業邏輯 
//*  創建日期：2008-12-11
//*  修改日期：2008-12-11 9:00
//*  修改記錄：
//*            □2008-12-11
//*              1.創建 王俊
//*            □2009-09-03
//*              1.修改 楊昆 for 代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算
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
/// Finance0021BL 的摘要描述
/// </summary>
public class Finance0021BL : BaseLogic
{
    #region SQL語句
    public const string SEL_PARAM_USE = "SELECT * FROM PARAM " +
                        "WHERE RST = 'A' AND Param_Code = '" + GlobalString.Parameter.Finance + "'";

    public const string SEL_CARD_GROUP = "SELECT RID,GROUP_NAME " +
                        "FROM CARD_GROUP WHERE RST='A' ";

    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN " +
                        "FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y' ";

    public const string SEL_PERSO_PROJECT_SAP = "SELECT CG.RID as cgrid,PPS.RID,F.Factory_ShortName_CN AS Factory_ShortName_EN," +
                    "PPS.Begin_Date,PPS.End_Date,(Convert(varchar(20),PPS.Begin_Date,111)+'~'+Convert(varchar(20)," +
                    "PPS.End_Date,111)) as Date,CG.Group_Name,PPS.Sum,PPS.SAP_ID," +
                    "Convert(varchar(20),PPS.Ask_Date,111) as Ask_Date,Convert(varchar(20),PPS.Pay_Date,111) as Pay_Date," +
                    "PPS.Check_Serial_Number,PPS.Perso_Factory_RID,PPS.Card_Group_RID as Group_RID " +
                    "FROM PERSO_PROJECT_SAP PPS LEFT JOIN Factory F ON F.RST = 'A' AND PPS.Perso_Factory_RID = F.RID " +
                        "LEFT JOIN CARD_GROUP CG ON CG.RST = 'A' AND PPS.Card_Group_RID = CG.RID " +
                    "WHERE PPS.RST = 'A' AND PPS.Begin_Date <= @Finish_Date2 AND PPS.End_Date >= @Begin_Date2 ";

    public const string SEL_PARAM_CHANGE = "SELECT * FROM PARAM " +
                    "WHERE RST = 'A' AND ParamType_Code = '" + GlobalString.ParameterType.Finance + "'";

    public const string SEL_PERSO_PROJECT_CHANGE_DETAIL = "SELECT PPCD.Param_Code,P.Param_Name,SUM(Price) " +
                    "FROM PERSO_PROJECT_CHANGE_DETAIL PPCD " +
                        "LEFT JOIN PARAM P ON P.RST = 'A' AND P.ParamType_Code = 'Finance' AND PPCD.Param_Code = P.Param_Code " +
                    "WHERE PPCD.RST = 'A' AND PPCD.Perso_Factory = @perso_factory AND " +
                        "PPCD.CardGroup_RID = @cardgroup_rid AND PPCD.Project_Date >= @Begin_Date " +
                        "AND PPCD.Project_Date <= @End_Date " +
                    "GROUP BY PPCD.Param_Code,P.Param_Name ";

    public const string DEL_PERSO_PROJECT_SAP = "DELETE FROM PERSO_PROJECT_SAP WHERE RID = @RID";

    public const string SEL_WORK_DATE = "SELECT Date_Time FROM WORK_DATE " +
                    "WHERE RST = 'A' AND Is_WorkDay = 'Y' AND Date_Time >=@startdate AND Date_Time<=@enddate";

    public const string SEL_SURPLUS_DATE = "SELECT DISTINCT Stock_Date FROM CARDTYPE_STOCKS " +
                    "WHERE RST = 'A' AND Stock_Date >= @startdate AND Stock_Date<=@enddate";

    public const string CON_AGAIN_SURPLUS = "SELECT COUNT(*) FROM PERSO_PROJECT_SAP " +
                    "WHERE RST = 'A' AND Begin_Date < @enddate AND " +
                    "End_Date > @startdate AND " +
                    "(Convert(varchar(20),Begin_Date,111) != @startdate1 OR " +
                    "Convert(varchar(20),End_Date,111) != @enddate1 )";

    public const string SEL_ASKED_GROUP_RID = "SELECT DISTINCT Card_Group_RID " +
                "FROM PERSO_PROJECT_SAP " +
                "WHERE RST = 'A' AND Begin_Date = @Begin_Date2 AND End_Date = @Finish_Date2";

    public const string DEL_MAKE_COST_FROM_SUBTOTAL_IMPORT = "DELETE FROM PERSO_PROJECT_DETAIL " +
                    "WHERE RST = 'A' AND Use_Date>=@Begin_Date2 AND Use_Date <= @Finish_Date2 ";

    public const string SEL_SUBTOTAL_IMPORT = "SELECT SI.* ,CT.RID AS CTRID,CG.RID AS CGRID " +
                "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' " +
                    "AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                 "INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID " +
                 "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID AND CG.Param_Code = '" + GlobalString.Parameter.Finance + "' " +
                 "WHERE SI.RST = 'A' AND SI.Date_Time >=@Begin_Date2 AND SI.Date_Time<=@Finish_Date2";
  
    //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/08/31 start
    public const string SEL_SUBTOTAL_REPLACE_IMPORT = "SELECT Date_Time,Perso_Factory_RID,CTRID,CGRID,SUM(Number) AS Number from " +
                    "(SELECT SI.Date_Time,SI.Perso_Factory_RID,SI.Number ,CT.RID AS CTRID,CG.RID AS CGRID " +
                    "FROM SUBTOTAL_REPLACE_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' " +
                    "AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                    "INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID " +
                    "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID AND CG.Param_Code = '" + GlobalString.Parameter.Finance + "' " +
                    "WHERE SI.RST = 'A' AND SI.Date_Time >=@Begin_Date2 AND SI.Date_Time<=@Finish_Date2 " +
                    "union all " +
                    "SELECT FCRI.Date_Time,FCRI.Perso_Factory_RID,Case FCRI.Status_RID when '5' then 0-FCRI.Number when '6' then 0-FCRI.Number when '7' then FCRI.Number end as Number " +
                    " ,CT.RID AS CTRID,CG.RID AS CGRID  " +
                    "FROM FACTORY_CHANGE_REPLACE_IMPORT FCRI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' " +
                    "AND FCRI.TYPE = CT.TYPE AND FCRI.AFFINITY = CT.AFFINITY AND FCRI.PHOTO = CT.PHOTO " +
                    "INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID " +
                    "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID  AND CG.Param_Code = '" + GlobalString.Parameter.Finance + "' " +
                    "WHERE FCRI.RST = 'A' AND FCRI.Status_RID in ('5','6','7') " +
                    "AND FCRI.Date_Time >=@Begin_Date2 AND FCRI.Date_Time<=@Finish_Date2) A "+
                    "where 1=1 ";
               
    //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/08/31 end
    public const string SEL_PROJECT_STEP_SURPLUS = "SELECT PP.RID,PPP.Price " +
                "FROM CARDTYPE_PERSO_PROJECT CPP " +
                "INNER JOIN CARDTYPE_PROJECT_TIME CPT ON CPT.RST  = 'A' AND CPP.ProjectTime_RID = CPT.RID " +
                "INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' AND CPT.PersoProject_RID = PP.RID AND PP.Normal_Special = '1' " +
                "INNER JOIN PERSO_PROJECT_PRICE PPP ON PPP.RST = 'A' AND CPT.PersoProject_RID = PPP.Perso_Project_RID " +
            "WHERE CPP.RST = 'A' AND CPP.CardType_RID = @CTRID " +
                "AND CPT.Use_Date_Begin<=@Date_Time AND CPT.Use_Date_End>=@Date_Time " +
                "AND PPP.Use_Date_Begin<=@Date_Time AND PPP.Use_Date_End>=@Date_Time " +
                "AND PP.Factory_RID = @perso_factory_rid ";

    public const string proc_Finance0021 = "proc_Finance0021";

    public const string CON_SAP = "SELECT COUNT(*) FROM " +
                "(SELECT DISTINCT SAP_Serial_Number " +
                    "FROM CARD_TYPE_SAP WHERE RST = 'A' " +
                    "UNION " +
                    "SELECT DISTINCT SAP_ID FROM MATERIEL_SAP WHERE RST = 'A') A " +
                "WHERE SAP_Serial_Number = @sap_serial_number";

    public const string SEL_ALL_CHECK_SERIAL_NUMBER = "SELECT DISTINCT Check_Serial_Number " +
                "FROM CARD_TYPE_SAP_DETAIL " +
                "WHERE RST = 'A' UNION " +
                "SELECT DISTINCT Check_Serial_Number " +
                "FROM PERSO_PROJECT_SAP WHERE RST = 'A'";

    public const string SEL_SUM_PROJECT = "SELECT CG.RID,CG.Group_Name,sum(Sum) " +
                "FROM PERSO_PROJECT_SAP PPS INNER JOIN CARD_GROUP CG ON CG.RST = 'A' " +
                        "AND PPS.Card_Group_RID = CG.RID " +
        //預算費用計算不一致 by yangkun 2010/4/27 start
        //"WHERE PPS.RST = 'A' AND year(Begin_Date) = @year " +
                "WHERE PPS.RST = 'A' AND year(Ask_Date) = @year " +
        //預算費用計算不一致 by yangkun 2010/4/27 end
                "GROUP BY CG.RID,CG.Group_Name";

    public const string SEL_SUM_PROJECT_Updata = "SELECT CG.RID,CG.Group_Name,sum(Sum) " +
                "FROM PERSO_PROJECT_SAP PPS INNER JOIN CARD_GROUP CG ON CG.RST = 'A' " +
                        "AND PPS.Card_Group_RID = CG.RID " +
        //預算費用計算不一致 by yangkun 2010/4/27 start
        // "WHERE PPS.RST = 'A' AND year(Begin_Date) = @year AND PPS.RID <> @rid AND CG.RID = @cgrid " +
                "WHERE PPS.RST = 'A' AND year(Ask_Date) = @year AND PPS.RID <> @rid AND CG.RID = @cgrid " +
        //預算費用計算不一致 by yangkun 2010/4/27 end
                "GROUP BY CG.RID,CG.Group_Name";

    public const string SEL_SUM_PROJECT_Delete = "SELECT CG.RID,CG.Group_Name,sum(Sum) " +
               "FROM PERSO_PROJECT_SAP PPS INNER JOIN CARD_GROUP CG ON CG.RST = 'A' " +
                       "AND PPS.Card_Group_RID = CG.RID " +
        //預算費用計算不一致 by yangkun 2010/4/27 start
        //"WHERE PPS.RST = 'A' AND year(Begin_Date) = @year AND CG.RID = @cgrid " +
               "WHERE PPS.RST = 'A' AND year(Ask_Date) = @year AND CG.RID = @cgrid " +
        //預算費用計算不一致 by yangkun 2010/4/27 end
               "GROUP BY CG.RID,CG.Group_Name";

    public const string SEL_MATERIEL_BUDGET_CARD = "SELECT Budget " +
                "FROM MATERIEL_BUDGET " +
                "WHERE RST = 'A' AND Budget_Year = @year AND Materiel_Type = '9'";

    public const string SEL_MATERIEL_BUDGET_BANK = "SELECT Budget " +
                "FROM MATERIEL_BUDGET " +
                "WHERE RST = 'A' AND Budget_Year = @year AND Materiel_Type = '10'";

    public const string SEL_PERSO_PROJECT_SAP_EDIT = "SELECT CG.RID as cgrid,PPS.RID,F.Factory_ShortName_EN,Convert(varchar(20),PPS.Begin_Date,111) as Begin_Date,Convert(varchar(20),PPS.End_Date,111) as End_Date,CG.GROUP_NAME,PPS.Sum,PPS.SAP_ID,Convert(varchar(20),PPS.Ask_Date,111) as Ask_Date,Convert(varchar(20),PPS.Pay_Date,111) as Pay_Date,PPS.Check_Serial_Number,PPS.Perso_Factory_RID,PPS.Card_Group_RID as Group_RID FROM PERSO_PROJECT_SAP PPS LEFT JOIN Factory F ON F.RST = 'A' AND PPS.Perso_Factory_RID = F.RID INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND PPS.Card_Group_RID = CG.RID WHERE PPS.RST = 'A' AND PPS.RID = @rid";

    public const string SEL_ALL_CHECK_SERIAL_NUMBER_1 = "SELECT DISTINCT Check_Serial_Number FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' UNION SELECT DISTINCT Check_Serial_Number FROM PERSO_PROJECT_SAP WHERE RST = 'A' AND RID<>@rid";

    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Finance0021BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 計算出各Perso廠、各帳務群組之代製費用出帳金額
    /// </summary>
    /// <param name="strDateFrom"></param>
    /// <param name="strDateTo"></param>
    /// <param name="strFRID"></param>
    /// <returns></returns>
    public DataSet getMakeCost(string StartDate,
        string EndDate,
        string strFRID,
        ref string bln)
    {
        PERSO_PROJECT_DETAIL ppdModel = new PERSO_PROJECT_DETAIL();
        DataSet dsPERSO_PROJECT_SAP = new DataSet();
        DataSet dsASKED_GROUP_RID = null;
        try
        {
            dao.OpenConnection();

            dirValues.Clear();
            dirValues.Add("Finish_Date2", EndDate);
            dirValues.Add("Begin_Date2", StartDate);
            string strPersoFactoryRID = "";//str1
            string strCardGroupRID = "";//str2已經請款群組
            if (!StringUtil.IsEmpty(strFRID))
            {
                strPersoFactoryRID = " and Perso_Factory_RID = @Perso_Factory_RID ";
                dirValues.Add("Perso_Factory_RID", strFRID);
            }

            //此耗用區間已有請款資料
            dsASKED_GROUP_RID = dao.GetList(SEL_ASKED_GROUP_RID + strPersoFactoryRID, dirValues);
            if (dsASKED_GROUP_RID.Tables[0].Rows.Count > 0)
            {
                bln = "此耗用區間已有請款資料";
                foreach (DataRow dr1 in dsASKED_GROUP_RID.Tables[0].Rows)
                {
                    strCardGroupRID += dr1["Card_Group_RID"].ToString() + ",";
                }
                if (strCardGroupRID.Length > 0)
                {
                    strCardGroupRID = strCardGroupRID.Substring(0, strCardGroupRID.Length - 1);
                }
            }

            dirValues.Clear();
            dirValues.Add("Finish_Date2", EndDate + " 23:59:59");
            dirValues.Add("Begin_Date2", StartDate + " 00:00:00");
            if (!StringUtil.IsEmpty(strFRID))
            {
                dirValues.Add("Perso_Factory_RID", strFRID);
            }

            #region 根據小計檔訊息生成卡片耗用明細訊息
            //刪除相應的代製費用訊息
            if (strCardGroupRID.Length > 0)
            {
                dao.ExecuteNonQuery(DEL_MAKE_COST_FROM_SUBTOTAL_IMPORT + strPersoFactoryRID + " AND Card_Group_RID NOT IN (" + strCardGroupRID + ") ", dirValues);
            }
            else
            {
                dao.ExecuteNonQuery(DEL_MAKE_COST_FROM_SUBTOTAL_IMPORT + strPersoFactoryRID, dirValues);
            }

            // 取小計檔
           // string strSQL = SEL_SUBTOTAL_IMPORT;
            //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/08/31 start
            string strSQL = SEL_SUBTOTAL_REPLACE_IMPORT;
            string strGroup = " Group by Date_Time,Perso_Factory_RID,CTRID,CGRID ";
            if (strFRID != "")
                strSQL += " AND Perso_Factory_RID = " + strFRID;
            if (strCardGroupRID.Length != 0)
            {
                strSQL += " AND CGRID NOT IN (" + strCardGroupRID + ")";
            }
            //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/08/31 end
            //if (strFRID != "")
            //    strSQL += " AND SI.Perso_Factory_RID = " + strFRID;
            //if (strCardGroupRID.Length != 0)
            //{
            //    strSQL += " AND CG.RID NOT IN (" + strCardGroupRID + ")";
            //}
            
            // 計算一般代製費用
            DataSet dsSUBTOTAL_IMPORT = dao.GetList(strSQL + strGroup, dirValues);
            //將小計檔和廠商異動檔中卡種按”製成卡”公式(3D+DA+PM+RN-缺卡-未製卡+補製卡)結果計算
            foreach (DataRow dr in dsSUBTOTAL_IMPORT.Tables[0].Rows)
            {
                dirValues.Clear();
                dirValues.Add("Date_Time", Convert.ToDateTime(dr["Date_Time"].ToString()).ToString("yyyy-MM-dd"));
                dirValues.Add("CTRID", dr["CTRID"].ToString());
                dirValues.Add("perso_factory_rid", dr["Perso_Factory_RID"].ToString());

                DataSet dsPROJECT_STEP_SURPLUS = dao.GetList(SEL_PROJECT_STEP_SURPLUS, dirValues);
                decimal Price = 0;//單價
                int Perso_Project_RID = 0;//Perso廠
                decimal SumPrice = 0;//金額
                if (dsPROJECT_STEP_SURPLUS.Tables[0].Rows.Count != 0)
                {
                    Price = Convert.ToDecimal(dsPROJECT_STEP_SURPLUS.Tables[0].Rows[0]["Price"]);
                    SumPrice = Price*Convert.ToInt32(dr["Number"]);
                    Perso_Project_RID = Convert.ToInt32(dsPROJECT_STEP_SURPLUS.Tables[0].Rows[0]["RID"]);
                }

                ppdModel.Use_Date = Convert.ToDateTime(dr["Date_Time"]);
                ppdModel.Sum = SumPrice;
                ppdModel.Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
                ppdModel.Card_Group_RID = Convert.ToInt32(dr["CGRID"]);
                ppdModel.CardType_RID = Convert.ToInt32(dr["CTRID"]);
                ppdModel.Number = Convert.ToInt32(dr["Number"]);
                ppdModel.Unit_Price = Price;
                ppdModel.Project_RID = Perso_Project_RID;
                dao.Add<PERSO_PROJECT_DETAIL>(ppdModel, "RID");
            }

            //查詢代製費（採用存儲過程處理）
            if (strCardGroupRID != "")
            {
                DataTable dtbl = getMakeCost_Proc(StartDate, EndDate, strFRID).Tables[0];
                DataTable dtblPERSO_PROJECT_SAP = new DataTable();
                dtblPERSO_PROJECT_SAP = dtbl.Clone();

                DataRow[] rows = dtbl.Select("Group_RID NOT IN (" + strCardGroupRID + ")");
                foreach (DataRow dr1 in rows)
                {
                    DataRow drow = dtblPERSO_PROJECT_SAP.NewRow();
                    drow.ItemArray = dr1.ItemArray;
                    dtblPERSO_PROJECT_SAP.Rows.Add(drow);
                }
                dsPERSO_PROJECT_SAP.Tables.Add(dtblPERSO_PROJECT_SAP);
            }
            else
            {
                dsPERSO_PROJECT_SAP = getMakeCost_Proc(StartDate, EndDate, strFRID);
            }

            #endregion

            //20100226合計數值自動四捨五入為整數，顯示小數點兩位 by kunyang start 
            if (dsPERSO_PROJECT_SAP != null
           && dsPERSO_PROJECT_SAP.Tables.Count > 0
           && dsPERSO_PROJECT_SAP.Tables[0].Rows.Count > 0)
            {              
                dsPERSO_PROJECT_SAP.Tables[0].Columns["Sum"].ReadOnly = false;
                foreach (DataRow dr in dsPERSO_PROJECT_SAP.Tables[0].Rows)
                {                   
                     dr["Sum"] = Convert.ToString(Math.Round(Convert.ToDecimal(dr["Sum"].ToString()), MidpointRounding.AwayFromZero))+".00";
               
                }
              
            }
            //20100226合計數值自動四捨五入為整數，顯示小數點兩位 by kunyang end
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
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }       
        return dsPERSO_PROJECT_SAP;
    }

    /// <summary>
    /// 查詢代製費
    /// </summary>
    public DataSet getMakeCost_Proc(string Begin_Date, string Finish_Date, string strFRID)
    {
        if (strFRID == "")
            strFRID = "-1";
        dirValues.Clear();
        dirValues.Add("Begin_Date", Begin_Date);
        dirValues.Add("Finish_Date", Finish_Date);
        dirValues.Add("FactoryRID", strFRID);
        DataSet dsMakeCost_Proc = dao.GetList(proc_Finance0021, dirValues, true);
        return dsMakeCost_Proc;
    }


    public bool Delete(string strRID)
    {
        bool n = false;

        try
        {
            if (StringUtil.IsEmpty(strRID))
                return false;

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.ExecuteNonQuery(DEL_PERSO_PROJECT_SAP, dirValues);
            n = true;

            SetOprLog("4");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
        return n;
    }

    /// <summary>
    /// 獲取用途
    /// </summary>
    /// <returns></returns>
    public DataSet getParam_Use()
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
    /// 獲取卡廠
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
    /// 查詢請款放行作業
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet SearchSAP(Dictionary<string, object> searchInput)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;

        StringBuilder stbCommand = new StringBuilder(SEL_PERSO_PROJECT_SAP);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropCard_Group"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPS.Card_Group_RID = @card_group");
                dirValues.Add("card_group", searchInput["dropCard_Group"].ToString().Trim());

            }

            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPS.Perso_Factory_RID = @factory");
                dirValues.Add("factory", searchInput["dropFactory"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtSAP_Serial_Number"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPS.SAP_ID LIKE @sap_serial_number ");
                dirValues.Add("sap_serial_number", "%" + searchInput["txtSAP_Serial_Number"].ToString().Trim() + "%");

            }

            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date1"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPS.Ask_Date >= @Begin_Date1 ");
                dirValues.Add("Begin_Date1", searchInput["txtBegin_Date1"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date1"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPS.Ask_Date <= @Finish_Date1 ");
                dirValues.Add("Finish_Date1", searchInput["txtFinish_Date1"].ToString().Trim());

            }

            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date3"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPS.Pay_Date >= @Begin_Date3 ");
                dirValues.Add("Begin_Date3", searchInput["txtBegin_Date3"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date3"].ToString().Trim()))
            {
                stbWhere.Append(" AND PPS.Pay_Date <= @Finish_Date3 ");
                dirValues.Add("Finish_Date3", searchInput["txtFinish_Date3"].ToString().Trim());

            }


            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date2"].ToString()))
            {
                dirValues.Add("Begin_Date2", searchInput["txtBegin_Date2"].ToString());
            }
            else
            {
                dirValues.Add("Begin_Date2", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date2"].ToString()))
            {
                dirValues.Add("Finish_Date2", searchInput["txtFinish_Date2"].ToString());
            }
            else
            {
                dirValues.Add("Finish_Date2", "9999/12/31");
            }
        }
        DataSet dtSAP = null;
        try
        {
            dtSAP = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }

        return dtSAP;
    }


    /// <summary>
    /// 獲取“罰單”、“這讓”
    /// </summary>
    /// <returns></returns>
    public DataSet getParam_Change()
    {
        DataSet dsPARAM_CHANGE = null;

        try
        {
            dsPARAM_CHANGE = dao.GetList(SEL_PARAM_CHANGE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dsPARAM_CHANGE;
    }

    /// <summary>
    /// 獲取“罰單”、“這讓”對應的金額
    /// </summary>
    /// <returns></returns>
    public DataSet SearchChange(string Perso_Factory_RID, string Card_Group_RID, DateTime Begin_Date, DateTime End_Date)
    {
        DataSet dsPERSO_PROJECT_CHANGE_DETAIL = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("perso_factory", Perso_Factory_RID);
            dirValues.Add("cardgroup_rid", Card_Group_RID);
            dirValues.Add("Begin_Date", Begin_Date);
            dirValues.Add("End_Date", End_Date);
            dsPERSO_PROJECT_CHANGE_DETAIL = dao.GetList(SEL_PERSO_PROJECT_CHANGE_DETAIL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dsPERSO_PROJECT_CHANGE_DETAIL;
    }


    /// <summary>
    /// 檢查卡片耗用日期區間所有工作日是否都日結
    /// </summary>
    /// <returns></returns>
    public DateTime CheckEachWorkDateIsSurplus(DateTime StartDate, DateTime EndDate)
    {
        DataSet dsWORK_DATE = null;
        DataSet dsSURPLUS_DATE = null;
        DateTime Date = Convert.ToDateTime("1900/01/01");
        try
        {
            dirValues.Clear();
            dirValues.Add("startdate", StartDate);
            dirValues.Add("enddate", EndDate);

            dsWORK_DATE = dao.GetList(SEL_WORK_DATE, dirValues);
            dsSURPLUS_DATE = dao.GetList(SEL_SURPLUS_DATE, dirValues);

            foreach (DataRow dr in dsWORK_DATE.Tables[0].Rows)
            {
                DataRow[] drow = dsSURPLUS_DATE.Tables[0].Select("Stock_Date = '" + Convert.ToDateTime(dr["Date_Time"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "' ");

                if (drow.Length == 0)
                {
                    return Convert.ToDateTime(dr["Date_Time"]);
                }
            }

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return Date;
    }


    /// <summary>
    /// 檢查卡片耗用日期區間是否和其他請款區間相重疊
    /// </summary>
    /// <returns></returns>
    public bool CheckAgainSurplus(DateTime StartDate, DateTime EndDate)
    {
        DataSet dsAGAIN_SURPLUS = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("startdate", StartDate.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("enddate", EndDate.ToString("yyyy/MM/dd 23:59:59"));
            dirValues.Add("startdate1", StartDate.ToString("yyyy/MM/dd"));
            dirValues.Add("enddate1", EndDate.ToString("yyyy/MM/dd"));

            dsAGAIN_SURPLUS = dao.GetList(CON_AGAIN_SURPLUS, dirValues);

            if (Convert.ToInt32(dsAGAIN_SURPLUS.Tables[0].Rows[0][0]) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 檢查新的SAP單號和已經存在SAP單號是否重復
    /// </summary>
    public DataSet CONSAP(string SAP_Serial_Number)
    {
        dirValues.Clear();
        dirValues.Add("sap_serial_number", SAP_Serial_Number);
        DataSet dsCON_SAP = dao.GetList(CON_SAP, dirValues);
        return dsCON_SAP;
    }

    /// <summary>
    /// 檢查每行的發票編號是否和數據庫中記錄的發票編號相重復
    /// </summary>
    public bool getAllCheckSerialNumber(string Check_Serial_Number)
    {
        DataSet dsALL_CHECK_SERIAL_NUMBER = dao.GetList(SEL_ALL_CHECK_SERIAL_NUMBER);
        foreach (DataRow dr in dsALL_CHECK_SERIAL_NUMBER.Tables[0].Rows)
        {
            if (dr[0].ToString().Trim() == Check_Serial_Number)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 檢查每行的發票編號是否和數據庫中記錄的發票編號相重復
    /// </summary>
    public bool getAllCheckSerialNumber_1(string Check_Serial_Number, string strRID)
    {
        dirValues.Clear();
        dirValues.Add("rid", strRID);
        DataSet dsALL_CHECK_SERIAL_NUMBER_1 = dao.GetList(SEL_ALL_CHECK_SERIAL_NUMBER_1, dirValues);
        foreach (DataRow dr in dsALL_CHECK_SERIAL_NUMBER_1.Tables[0].Rows)
        {
            if (dr[0].ToString().Trim() == Check_Serial_Number)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 檢查每行的發票編號是否和數據庫中記錄的發票編號相重復
    /// </summary>
    public int ForcastCheck(string DateFrom, decimal ctk,
            decimal jrk, decimal xyk, decimal xjk, decimal DEBIT)
    {
        dirValues.Clear();
        dirValues.Add("year", DateFrom.Substring(0, 4));
        //取本年度磁條信用卡、晶片金融卡、現金卡、晶片信用卡、VISA DEBIT卡的已經耗用的代製費用
        DataSet dsSUM_PROJECT = dao.GetList(SEL_SUM_PROJECT, dirValues);
        //本年度代製費用(卡)已耗用
        Decimal SUM_PROJECT_CARD = 0;
        //本年度代製費用(銀)已耗用
        Decimal SUM_PROJECT_BANK = 0;

        // 取出代製費用年度耗用
        if (dsSUM_PROJECT.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow dr in dsSUM_PROJECT.Tables[0].Rows)
            {
                //if (dr["Group_Name"].ToString() == "磁條信用卡" ||
                //        dr["Group_Name"].ToString() == "晶片信用卡" ||
                //        dr["Group_Name"].ToString() == "晶片金融卡")
                //{
                //    SUM_PROJECT_CARD += Convert.ToDecimal(dr[2]);
                //}
                //if (dr["Group_Name"].ToString() == "現金卡" ||
                //    dr["Group_Name"].ToString() == "VISA DEBIT卡")
                //{
                //    SUM_PROJECT_BANK += Convert.ToDecimal(dr[2]);
                //}
                if (dr["Group_Name"].ToString() == "磁條信用卡" ||
                        dr["Group_Name"].ToString() == "晶片信用卡" ||
                        dr["Group_Name"].ToString() == "VISA DEBIT")
                {
                    SUM_PROJECT_CARD += Convert.ToDecimal(dr[2]);
                }
                if (dr["Group_Name"].ToString() == "現金卡" ||
                    dr["Group_Name"].ToString() == "晶片金融卡")
                {
                    SUM_PROJECT_BANK += Convert.ToDecimal(dr[2]);
                }
            }
        }

        //取本年度代製費用（卡）預算
        decimal MATERIEL_BUDGET_CARD = 0;
        DataSet dsMATERIEL_BUDGET_CARD = dao.GetList(SEL_MATERIEL_BUDGET_CARD, dirValues);
        if (dsMATERIEL_BUDGET_CARD.Tables[0].Rows.Count != 0)
        {
            MATERIEL_BUDGET_CARD = Convert.ToDecimal(dsMATERIEL_BUDGET_CARD.Tables[0].Rows[0][0]);
        }

        //本年度代製費用（銀）預算
        decimal MATERIEL_BUDGET_BANK = 0;
        DataSet dsMATERIEL_BUDGET_BANK = dao.GetList(SEL_MATERIEL_BUDGET_BANK, dirValues);
        if (dsMATERIEL_BUDGET_BANK.Tables[0].Rows.Count != 0)
        {
            MATERIEL_BUDGET_BANK = Convert.ToDecimal(dsMATERIEL_BUDGET_BANK.Tables[0].Rows[0][0]);
        }
        //計算本年度代製費用（卡）預算剩余金額 = 本年度代製費用（卡）預算 - 本年度代製費用(卡)已耗用-int<磁條信用卡>-int<晶片金融卡>-int<晶片信用卡>
        //decimal BudGet_SurPlus_Money_CARD = MATERIEL_BUDGET_CARD - SUM_PROJECT_CARD - ctk - jrk - xyk;
        decimal BudGet_SurPlus_Money_CARD = MATERIEL_BUDGET_CARD - SUM_PROJECT_CARD - ctk - DEBIT - xyk;

        //計算本年度代製費用（銀）預算剩余金額 = 本年度代製費用（銀）預算 - 本年度代製費用(銀)已耗用-int<現金卡>-int<VISA DEBIT卡 >
        //decimal BudGet_SurPlus_Money_BANK = MATERIEL_BUDGET_BANK - SUM_PROJECT_BANK - xjk - DEBIT;
        decimal BudGet_SurPlus_Money_BANK = MATERIEL_BUDGET_BANK - SUM_PROJECT_BANK - xjk - jrk;

        if (MATERIEL_BUDGET_BANK <= 0 || MATERIEL_BUDGET_CARD <= 0)
        {
            return 1;//代製費用年度預算剩餘金額不足
        }
        else if (BudGet_SurPlus_Money_CARD < 0 || BudGet_SurPlus_Money_BANK < 0)
        {
            return 1;//代製費用年度預算剩餘金額不足
        }
        else
        {
            if (Convert.ToDouble(BudGet_SurPlus_Money_CARD / MATERIEL_BUDGET_CARD) < 0.1)
            {
                string[] arg = new string[1];
                arg[0] = "代製費用(卡)";
                Warning.SetWarning(GlobalString.WarningType.PersoProjectSapAskMoney, arg);

            }
            if (Convert.ToDouble(BudGet_SurPlus_Money_BANK / MATERIEL_BUDGET_BANK) < 0.1)
            {
                //todo 提示“代製費用年度預算剩餘金額低於10%”，并觸發1.5警訊提示。
                string[] arg = new string[1];
                arg[0] = "代製費用(銀)";
                Warning.SetWarning(GlobalString.WarningType.PersoProjectSapAskMoney, arg);
                return 2;//代製費用年度預算剩餘金額低於10%
            }
        }
        return 3;
    }


    /// <summary>
    /// 修改时 檢查每行的發票編號是否和數據庫中記錄的發票編號相重復
    /// </summary>
    public int ForcastCheck(string DateFrom, decimal decCard, string strRid, string strcgrid)
    {
        dirValues.Clear();
        dirValues.Add("year", DateFrom.Substring(0, 4));
        dirValues.Add("rid", strRid);
        dirValues.Add("cgrid", strcgrid);
        //取本年度磁條信用卡、晶片金融卡、現金卡、晶片信用卡、VISA DEBIT卡的已經耗用的代製費用
        DataSet dsSUM_PROJECT = dao.GetList(SEL_SUM_PROJECT_Updata, dirValues);
        //本年度代製費用(卡)已耗用
        Decimal SUM_PROJECT_CARD = 0;
        //本年度代製費用(銀)已耗用
        Decimal SUM_PROJECT_BANK = 0;
        if (dsSUM_PROJECT.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow dr in dsSUM_PROJECT.Tables[0].Rows)
            {
                //if (dr["Group_Name"].ToString() == "磁條信用卡" || dr["Group_Name"].ToString() == "晶片信用卡" ||
                //    dr["Group_Name"].ToString() == "晶片金融卡")

                if (dr["Group_Name"].ToString() == "磁條信用卡" || dr["Group_Name"].ToString() == "晶片信用卡" ||
                    dr["Group_Name"].ToString() == "VISA DEBIT")
                {
                    SUM_PROJECT_CARD += Convert.ToDecimal(dr[2]);
                    //取本年度代製費用（卡）預算
                    decimal MATERIEL_BUDGET_CARD = 0;
                    DataSet dsMATERIEL_BUDGET_CARD = dao.GetList(SEL_MATERIEL_BUDGET_CARD, dirValues);
                    if (dsMATERIEL_BUDGET_CARD.Tables[0].Rows.Count != 0)
                    {
                        MATERIEL_BUDGET_CARD = Convert.ToDecimal(dsMATERIEL_BUDGET_CARD.Tables[0].Rows[0][0]);
                    }

                    //計算本年度代製費用（卡）預算剩余金額 = 本年度代製費用（卡）預算 - 本年度代製費用(卡)已耗用-int<磁條信用卡>-int<晶片金融卡>-int<晶片信用卡>
                    decimal BudGet_SurPlus_Money_CARD = MATERIEL_BUDGET_CARD - SUM_PROJECT_CARD - decCard;

                    if (MATERIEL_BUDGET_CARD <= 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (BudGet_SurPlus_Money_CARD < 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (Convert.ToDouble(BudGet_SurPlus_Money_CARD / MATERIEL_BUDGET_CARD) < 0.1)
                    {
                        //todo 提示“代製費用年度預算剩餘金額低於10%”，并觸發1.5警訊提示。
                        string[] arg = new string[1];
                        Warning.SetWarning(GlobalString.WarningType.PersoProjectSapAskMoney, arg);
                        return 2;//代製費用年度預算剩餘金額低於10%
                    }
                }
                else
                {
                    SUM_PROJECT_BANK += Convert.ToDecimal(dr[2]);
                    //本年度代製費用（銀）預算
                    decimal MATERIEL_BUDGET_BANK = 0;
                    DataSet dsMATERIEL_BUDGET_BANK = dao.GetList(SEL_MATERIEL_BUDGET_BANK, dirValues);
                    if (dsMATERIEL_BUDGET_BANK.Tables[0].Rows.Count != 0)
                    {
                        MATERIEL_BUDGET_BANK = Convert.ToDecimal(dsMATERIEL_BUDGET_BANK.Tables[0].Rows[0][0]);
                    }
                    //計算本年度代製費用（銀）預算剩余金額 = 本年度代製費用（銀）預算 - 本年度代製費用(銀)已耗用-int<現金卡>-int<VISA DEBIT卡 >
                    decimal BudGet_SurPlus_Money_BANK = MATERIEL_BUDGET_BANK - SUM_PROJECT_BANK - decCard;

                    if (MATERIEL_BUDGET_BANK <= 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (BudGet_SurPlus_Money_BANK < 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (Convert.ToDouble(BudGet_SurPlus_Money_BANK / MATERIEL_BUDGET_BANK) < 0.1)
                    {
                        //todo 提示“代製費用年度預算剩餘金額低於10%”，并觸發1.5警訊提示。
                        string[] arg = new string[1];
                        Warning.SetWarning(GlobalString.WarningType.PersoProjectSapAskMoney, arg);
                        return 2;//代製費用年度預算剩餘金額低於10%
                    }
                }
            }
        }
        return 3;
    }


    /// <summary>
    /// 删除时 檢查每行的發票編號是否和數據庫中記錄的發票編號相重復
    /// </summary>
    public int ForcastCheck(string DateFrom, string strcgrid)
    {
        dirValues.Clear();
        dirValues.Add("year", DateFrom.Substring(0, 4));
        dirValues.Add("cgrid", strcgrid);
        //取本年度磁條信用卡、晶片金融卡、現金卡、晶片信用卡、VISA DEBIT卡的已經耗用的代製費用
        DataSet dsSUM_PROJECT = dao.GetList(SEL_SUM_PROJECT_Delete, dirValues);
        //本年度代製費用(卡)已耗用
        Decimal SUM_PROJECT_CARD = 0;
        //本年度代製費用(銀)已耗用
        Decimal SUM_PROJECT_BANK = 0;
        if (dsSUM_PROJECT.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow dr in dsSUM_PROJECT.Tables[0].Rows)
            {
                //if (dr["Group_Name"].ToString() == "磁條信用卡" || dr["Group_Name"].ToString() == "晶片信用卡" ||
                //    dr["Group_Name"].ToString() == "晶片金融卡")
                if (dr["Group_Name"].ToString() == "磁條信用卡" || dr["Group_Name"].ToString() == "晶片信用卡" ||
                    dr["Group_Name"].ToString() == "VISA DEBIT")
                {
                    SUM_PROJECT_CARD += Convert.ToDecimal(dr[2]);
                    //取本年度代製費用（卡）預算
                    decimal MATERIEL_BUDGET_CARD = 0;
                    DataSet dsMATERIEL_BUDGET_CARD = dao.GetList(SEL_MATERIEL_BUDGET_CARD, dirValues);
                    if (dsMATERIEL_BUDGET_CARD.Tables[0].Rows.Count != 0)
                    {
                        MATERIEL_BUDGET_CARD = Convert.ToDecimal(dsMATERIEL_BUDGET_CARD.Tables[0].Rows[0][0]);
                    }

                    //計算本年度代製費用（卡）預算剩余金額 = 本年度代製費用（卡）預算 - 本年度代製費用(卡)已耗用-int<磁條信用卡>-int<晶片金融卡>-int<晶片信用卡>
                    decimal BudGet_SurPlus_Money_CARD = MATERIEL_BUDGET_CARD - SUM_PROJECT_CARD;

                    if (MATERIEL_BUDGET_CARD <= 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (BudGet_SurPlus_Money_CARD < 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (Convert.ToDouble(BudGet_SurPlus_Money_CARD / MATERIEL_BUDGET_CARD) < 0.1)
                    {
                        //todo 提示“代製費用年度預算剩餘金額低於10%”，并觸發1.5警訊提示。
                        string[] arg = new string[1];
                        Warning.SetWarning(GlobalString.WarningType.PersoProjectSapAskMoney, arg);
                        return 2;//代製費用年度預算剩餘金額低於10%
                    }
                }
                else
                {
                    SUM_PROJECT_BANK += Convert.ToDecimal(dr[2]);
                    //本年度代製費用（銀）預算
                    decimal MATERIEL_BUDGET_BANK = 0;
                    DataSet dsMATERIEL_BUDGET_BANK = dao.GetList(SEL_MATERIEL_BUDGET_BANK, dirValues);
                    if (dsMATERIEL_BUDGET_BANK.Tables[0].Rows.Count != 0)
                    {
                        MATERIEL_BUDGET_BANK = Convert.ToDecimal(dsMATERIEL_BUDGET_BANK.Tables[0].Rows[0][0]);
                    }
                    //計算本年度代製費用（銀）預算剩余金額 = 本年度代製費用（銀）預算 - 本年度代製費用(銀)已耗用-int<現金卡>-int<VISA DEBIT卡 >
                    decimal BudGet_SurPlus_Money_BANK = MATERIEL_BUDGET_BANK - SUM_PROJECT_BANK;

                    if (MATERIEL_BUDGET_BANK <= 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (BudGet_SurPlus_Money_BANK < 0)
                    {
                        return 1;//代製費用年度預算剩餘金額不足
                    }
                    else if (Convert.ToDouble(BudGet_SurPlus_Money_BANK / MATERIEL_BUDGET_BANK) < 0.1)
                    {
                        //todo 提示“代製費用年度預算剩餘金額低於10%”，并觸發1.5警訊提示。
                        string[] arg = new string[1];
                        Warning.SetWarning(GlobalString.WarningType.PersoProjectSapAskMoney, arg);
                        return 2;//代製費用年度預算剩餘金額低於10%
                    }
                }
            }
        }
        return 3;
    }


    /// <summary>
    /// 檢查每行的發票編號是否和數據庫中記錄的發票編號相重復
    /// </summary>
    public void Save(GridView gvSAP, DateTime Begin_Date, DateTime End_Date, DataTable dtSAP)
    {
        try
        {
            dao.OpenConnection();
            PERSO_PROJECT_SAP ppsModel = new PERSO_PROJECT_SAP();
            for (int i = 0; i < gvSAP.Rows.Count; i++)
            {
                TextBox txt1 = (TextBox)gvSAP.Rows[i].FindControl("txt1");//SAP單號
                TextBox txt2 = (TextBox)gvSAP.Rows[i].FindControl("txt2");//請款日 
                TextBox txt3 = (TextBox)gvSAP.Rows[i].FindControl("txt3");//出帳日
                TextBox txt4 = (TextBox)gvSAP.Rows[i].FindControl("txt4");//發票號碼

                // 沒有請款的。
                if (StringUtil.IsEmpty(txt2.Text) || StringUtil.IsEmpty(txt1.Text))
                    continue;

                ppsModel.Ask_Date = Convert.ToDateTime(txt2.Text);
                ppsModel.SAP_ID = txt1.Text.Trim();
                ppsModel.Sum = Convert.ToDecimal(gvSAP.Rows[i].Cells[2].Text); ;
                if (!StringUtil.IsEmpty(txt3.Text))
                    ppsModel.Pay_Date = Convert.ToDateTime(txt3.Text);
                ppsModel.Check_Serial_Number = txt4.Text.Trim();
                ppsModel.Perso_Factory_RID = int.Parse(dtSAP.Rows[i]["Perso_Factory_RID"].ToString());
                ppsModel.Card_Group_RID = int.Parse(dtSAP.Rows[i]["Group_RID"].ToString());
                ppsModel.Begin_Date = Begin_Date;
                ppsModel.End_Date = End_Date;
                dao.Add<PERSO_PROJECT_SAP>(ppsModel, "RID");
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
    /// 檢查每行的發票編號是否和數據庫中記錄的發票編號相重復
    /// </summary>
    public void Update(GridView gvSAP, string strRID)
    {
        try
        {
            for (int i = 0; i < gvSAP.Rows.Count; i++)
            {
                TextBox txt1 = (TextBox)gvSAP.Rows[i].FindControl("txt1");//SAP單號
                TextBox txt2 = (TextBox)gvSAP.Rows[i].FindControl("txt2");//請款日 
                TextBox txt3 = (TextBox)gvSAP.Rows[i].FindControl("txt3");//出帳日
                TextBox txt4 = (TextBox)gvSAP.Rows[i].FindControl("txt4");//發票號碼

                PERSO_PROJECT_SAP ppsModel = dao.GetModel<PERSO_PROJECT_SAP, int>("RID", int.Parse(strRID));
                if (ppsModel != null)
                {
                    if (!StringUtil.IsEmpty(txt2.Text))
                        ppsModel.Ask_Date = Convert.ToDateTime(txt2.Text);
                    ppsModel.SAP_ID = txt1.Text.Trim();
                    if (!StringUtil.IsEmpty(txt3.Text))
                        ppsModel.Pay_Date = Convert.ToDateTime(txt3.Text);
                    ppsModel.Check_Serial_Number = txt4.Text.Trim();
                    dao.Update<PERSO_PROJECT_SAP>(ppsModel, "RID");
                }
            }

            SetOprLog();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }

    }

    public DataSet getAskMoneyEdit(int rid)
    {
        DataSet dsPERSO_PROJECT_SAP_EDIT = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("rid", rid);
            dsPERSO_PROJECT_SAP_EDIT = dao.GetList(SEL_PERSO_PROJECT_SAP_EDIT, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dsPERSO_PROJECT_SAP_EDIT;
    }
}
