//******************************************************************
//*  作    者：bingyipan
//*  功能說明：一般代製費用明細查詢 
//*  創建日期：2008-11-17
//*  修改日期：
//*  修改記錄：
//*            □2008-12-17
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
using System.Collections;

/// <summary>
/// Finance0023BL 的摘要描述
/// </summary>
public class Finance0023BL : BaseLogic
{
    #region SQL語句
    public const string SEL_PARAM_FINANCE = "SELECT * FROM PARAM WHERE RST = 'A' AND Param_Code = '" + GlobalString.Parameter.Finance + "'";

    public const string SEL_CARD_GROUP = "SELECT RID,GROUP_NAME FROM CARD_GROUP WHERE RST='A' ";

    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y' ";

    public const string SEL_SUBTOTAL_PROJECT_COST = "SELECT 1 as Type,CG.Group_Name,F.RID,F.Factory_ShortName_CN," +
                        "SI.Date_Time,CT.RID CardType_RID,CT.Name,SI.Number,PP.Project_Name,PPP.Price " +
        //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/08/31 start
        //                "FROM (SELECT Convert(varchar(10),Date_Time,111) AS Date_Time,Perso_Factory_RID," +
        //                        "TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
        //                        //"FROM SUBTOTAL_IMPORT " +
        //                     "FROM SUBTOTAL_REPLACE_IMPORT " +
        //                        "WHERE RST = 'A'" +
        //                        "GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO) SI " +
        
                         " FROM ( select Date_Time,Perso_Factory_RID, "+
                            " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                            " from (SELECT Convert(varchar(10),Date_Time,111) AS Date_Time,Perso_Factory_RID, "+
                            " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                            " FROM SUBTOTAL_REPLACE_IMPORT " +
                            " WHERE RST = 'A' "+
                            " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO " +
                            " union all"+
                            " SELECT Convert(varchar(10),Date_Time,111) AS Date_Time,Perso_Factory_RID, "+
                            " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                            " FROM (select status_RID,Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO, " +
                            " Case status_RID when '5' then 0-Number when '6' then 0-Number when '7' then Number end as Number "+
                            " from FACTORY_CHANGE_REPLACE_IMPORT where Status_Rid in ('5','6','7') and RST = 'A') fci "+
                            " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO) A " +
                            " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO) SI " +
        //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/08/31 end
                        "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' " +
                            "and SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                        "INNER JOIN FACTORY F ON F.RST = 'A' and SI.Perso_Factory_RID = F.RID " +
                        "INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID " +
                        "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID " +
                        "INNER JOIN PARAM P ON P.RST = 'A' AND CG.Param_Code = P.Param_Code AND P.Param_Code = '" + GlobalString.Parameter.Finance + "' " +
                        "INNER JOIN CARDTYPE_PERSO_PROJECT CPP ON CPP.RST = 'A' AND CT.RID = CPP.CardType_RID " +
                        "INNER JOIN (SELECT RID,PersoProject_RID," +
                                    "Convert(varchar(10),Use_Date_Begin,111) AS Use_Date_Begin," +
                                    "Convert(varchar(10),Use_Date_End,111) AS Use_Date_End " +
                                "FROM CARDTYPE_PROJECT_TIME WHERE RST = 'A' ) CPT ON CPP.ProjectTime_RID = CPT.RID AND CPT.Use_Date_Begin <= SI.Date_Time AND CPT.Use_Date_End >= SI.Date_Time " +
                        "INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' and si.Perso_Factory_RID = pp.factory_rid AND CPT.PersoProject_RID = PP.RID AND PP.Normal_Special = '1' " +
                        "INNER JOIN (SELECT Perso_Project_RID,Price," +
                                "Convert(varchar(10),Use_Date_Begin,111) as Use_Date_Begin," +
                                "Convert(varchar(10),Use_Date_End,111) as Use_Date_End " +
                                "FROM PERSO_PROJECT_PRICE WHERE RST = 'A' ) PPP ON CPT.PersoProject_RID = PPP.Perso_Project_RID AND PPP.Use_Date_Begin <= SI.Date_Time AND PPP.Use_Date_End >= SI.Date_Time " +
                        "WHERE 1=1 ";
    public const string SEL_SUBTOTAL_PROJECT_COST_REPLACE = "SELECT 1 as Type,CG.Group_Name,F.RID,F.Factory_ShortName_CN," +
                       "SI.Date_Time,CT.RID CardType_RID,CT.Name,SI.Number,PP.Project_Name,PPP.Price " +
                           " FROM ( select Date_Time,Perso_Factory_RID, " +
                           " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                           " from (SELECT Convert(varchar(10),Date_Time,111) AS Date_Time,Perso_Factory_RID, " +
                           " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                           " FROM SUBTOTAL_IMPORT " +
                           " WHERE RST = 'A' " +
                           " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO " +
                           " union all" +
                           " SELECT Convert(varchar(10),Date_Time,111) AS Date_Time,Perso_Factory_RID, " +
                           " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                           " FROM (select status_RID,Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO, " +
                           " Case status_RID when '5' then 0-Number when '6' then 0-Number when '7' then Number end as Number " +
                           " from FACTORY_CHANGE_IMPORT where Status_Rid in ('5','6','7') and RST = 'A') fci " +
                           " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO) A " +
                           " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO) SI " +
                         "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' " +
                           "and SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                       "INNER JOIN FACTORY F ON F.RST = 'A' and SI.Perso_Factory_RID = F.RID " +
                       "INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID " +
                       "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID " +
                       "INNER JOIN PARAM P ON P.RST = 'A' AND CG.Param_Code = P.Param_Code AND P.Param_Code = '" + GlobalString.Parameter.Finance + "' " +
                       "INNER JOIN CARDTYPE_PERSO_PROJECT CPP ON CPP.RST = 'A' AND CT.RID = CPP.CardType_RID " +
                       "INNER JOIN (SELECT RID,PersoProject_RID," +
                                   "Convert(varchar(10),Use_Date_Begin,111) AS Use_Date_Begin," +
                                   "Convert(varchar(10),Use_Date_End,111) AS Use_Date_End " +
                               "FROM CARDTYPE_PROJECT_TIME WHERE RST = 'A' ) CPT ON CPP.ProjectTime_RID = CPT.RID AND CPT.Use_Date_Begin <= SI.Date_Time AND CPT.Use_Date_End >= SI.Date_Time " +
                       "INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' and si.Perso_Factory_RID = pp.factory_rid AND CPT.PersoProject_RID = PP.RID AND PP.Normal_Special = '1' " +
                       "INNER JOIN (SELECT Perso_Project_RID,Price," +
                               "Convert(varchar(10),Use_Date_Begin,111) as Use_Date_Begin," +
                               "Convert(varchar(10),Use_Date_End,111) as Use_Date_End " +
                               "FROM PERSO_PROJECT_PRICE WHERE RST = 'A' ) PPP ON CPT.PersoProject_RID = PPP.Perso_Project_RID AND PPP.Use_Date_Begin <= SI.Date_Time AND PPP.Use_Date_End >= SI.Date_Time " +
                       "WHERE 1=1 ";

    public const string SEL_PROJECT_STEP_SUM = "SELECT PP.RID,PP.Project_Name,PPP.Price " +
                "FROM CARDTYPE_PERSO_PROJECT CPP " +
                "INNER JOIN CARDTYPE_PROJECT_TIME CPT ON CPT.RST  = 'A' AND CPP.ProjectTime_RID = CPT.RID " +
                "INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' AND CPT.PersoProject_RID = PP.RID AND PP.Normal_Special = '1' " +
                "INNER JOIN PERSO_PROJECT_PRICE PPP ON PPP.RST = 'A' AND CPT.PersoProject_RID = PPP.Perso_Project_RID " +
            "WHERE CPP.RST = 'A' AND CPP.CardType_RID = @CardType_RID " +
                "AND CPT.Use_Date_Begin<=@Use_Date AND CPT.Use_Date_End>=@Use_Date " +
                "AND PPP.Use_Date_Begin<=@Use_Date AND PPP.Use_Date_End>=@Use_Date " +
                "AND PP.Factory_RID = @perso_factory_rid ";


    //public const string IN_FINANCE0023 = "insert into RPT_Finance0023 (id,Group_Name,Factory_ShortName_CN,Date_Time,Finance,Price,Name,Number,TimeMark) VALUES (@id,@Group_Name,@Factory_ShortName_CN,@Date_Time,@Finance,@Price,@Name,@number,@TimeMark)";
    public const string IN_FINANCE0023 = "insert into RPT_Finance0023 (id,Group_Name,Factory_ShortName_CN,Date_Time,Finance,Price,Name,Number,TimeMark,Replace_Name,Replace_Number) VALUES (@id,@Group_Name,@Factory_ShortName_CN,@Date_Time,@Finance,@Price,@Name,@number,@TimeMark,@Replace_Name,@Replace_Number)";

    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Finance0023BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
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
    /// 查詢一般代製費用明細
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataTable SearchSubTotal(Dictionary<string, object> searchInput,string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    { 
        string strSortField = " order by Group_Name,Factory_ShortName_CN,Date_Time,Project_Name,Price,ct.rid";//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_SUBTOTAL_PROJECT_COST);
        //
        StringBuilder stbCommandReplace = new StringBuilder(SEL_SUBTOTAL_PROJECT_COST_REPLACE);
        //
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();

        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropCard_Group_RID"].ToString().Trim()))
            {
                stbWhere.Append(" AND CG.RID = @Card_Group_RID");
                dirValues.Add("Card_Group_RID", searchInput["dropCard_Group_RID"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND SI.Perso_Factory_RID = @Factory");
                dirValues.Add("Factory", searchInput["dropFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" AND CT.Name like @Name");
                dirValues.Add("Name", "%"+searchInput["txtName"].ToString().Trim()+"%");
            }
            //200909IR增加查詢條件代制項目 ADD by YANGKUN  2009/09/16 start
            if (!StringUtil.IsEmpty(searchInput["txtFinance"].ToString().Trim()))
            {
                stbWhere.Append(" AND PP.Project_Name like @Finance");
                dirValues.Add("Finance", "%" + searchInput["txtFinance"].ToString().Trim() + "%");
            }
            //200909IR增加查詢條件代制項目 ADD by YANGKUN  2009/09/16 end
            if (!StringUtil.IsEmpty(searchInput["txtBDate_Time"].ToString().Trim()))
            {
                stbWhere.Append(" AND SI.Date_Time>= @BDate_Time");
                dirValues.Add("BDate_Time", searchInput["txtBDate_Time"].ToString().Trim());
            }
            else
            {
                stbWhere.Append(" AND SI.Date_Time>= @BDate_Time");
                dirValues.Add("BDate_Time", "1900/01/01");
            }
            if (!StringUtil.IsEmpty(searchInput["txtEDate_Time"].ToString().Trim()))
            {
                stbWhere.Append(" AND SI.Date_Time<= @EDate_Time");
                dirValues.Add("EDate_Time", searchInput["txtEDate_Time"].ToString().Trim());
            }
            else
            {
                stbWhere.Append(" AND SI.Date_Time<= @EDate_Time");
                dirValues.Add("EDate_Time", "9999/12/31");
            }
        }
        
        int intRowCount = 0;
        DataSet dtFinance = null; //替換前數據       
        DataTable resdt = new DataTable();
        //
        DataSet dtFinanceReplace = null;
        DataTable resdtReplace = new DataTable();//替換后數據
        DataTable resdtTemp = new DataTable();//臨時數據
        DataTable resdtAll = new DataTable();//合并后的數據
        //
        try
        {
            //取得替換前的數據
            dtFinance = dao.GetList(stbCommand.ToString() + stbWhere.ToString() + strSortField, dirValues);           
            if (dtFinance != null && dtFinance.Tables[0].Rows.Count > 0)
            {
                //resdt = getGroup_Name(dtFinance.Tables[0]);
                resdt = dtFinance.Tables[0];
                intRowCount = resdt.Rows.Count;
            }
            //取得替換后的數據
            dtFinanceReplace = dao.GetList(stbCommandReplace.ToString() + stbWhere.ToString() + strSortField, dirValues);
            if (dtFinanceReplace != null && dtFinanceReplace.Tables[0].Rows.Count > 0)
            {
                resdtReplace = dtFinanceReplace.Tables[0]; 
            }
            //整理替換前和替換后的數據
            resdtTemp=getReplaceAll(resdt, resdtReplace);
            if (resdtTemp.Rows.Count > 0)
            {
                resdtTemp.DefaultView.Sort = "Group_Name,Factory_ShortName_CN,Date_Time,Project_Name,Replace_CardType_RID,Price";
                resdtAll = getGroup_Name_Replace(resdtTemp.DefaultView.ToTable());
                intRowCount = resdtAll.Rows.Count;
            }

            
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        rowCount = intRowCount;
        //return resdt;
        return resdtAll;
    }
    /// <summary>
    /// 計算群組的合計
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public DataTable getGroup_Name_Replace(DataTable dt)
    {
        DataTable dtTemp = dt.Copy();

        string strGroupName = "";
        string strLastGroupName = "";
        int intSumNumber = 0;
        int intSumNumberReplace = 0;
        DataRow Newdr = null;
        bool blHaveOneGroup = true;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            strGroupName = dt.Rows[i]["Group_Name"].ToString();
            if (strLastGroupName != "")
            {
                if (strGroupName != strLastGroupName)
                {
                    // 加合計
                    Newdr = dtTemp.NewRow();
                    Newdr[0] = 2;
                    Newdr[1] = strLastGroupName + " 合計";
                    Newdr[2] = 0;
                    Newdr[3] = "";
                    Newdr[5] = 0;
                    Newdr[6] = "";
                    Newdr[7] = intSumNumber;
                    Newdr[8] = "";
                    Newdr[9] = 0;
                    Newdr[10] = 0;
                    Newdr[11] = "";
                    Newdr[12] = intSumNumberReplace;
                    dtTemp.Rows.Add(Newdr);

                    strLastGroupName = strGroupName;
                    intSumNumber = 0;
                    intSumNumberReplace = 0;
                    blHaveOneGroup = false;
                    intSumNumber += Convert.ToInt32(dt.Rows[i]["Number"]);
                    intSumNumberReplace += Convert.ToInt32(dt.Rows[i]["Replace_Number"]);
                }
                else
                {
                    intSumNumber += Convert.ToInt32(dt.Rows[i]["Number"]);
                    intSumNumberReplace += Convert.ToInt32(dt.Rows[i]["Replace_Number"]);
                }
            }
            else
            {
                strLastGroupName = strGroupName;
                intSumNumber = Convert.ToInt32(dt.Rows[i]["Number"]);
                intSumNumberReplace = Convert.ToInt32(dt.Rows[i]["Replace_Number"]);
            }
        }

        // 加入最後一個群組的合計
        Newdr = dtTemp.NewRow();
        Newdr[0] = 2;
        if (blHaveOneGroup)   //只有一個群組時
            Newdr[1] = "合計";
        else
            Newdr[1] = strLastGroupName + " 合計";
        Newdr[2] = 0;
        Newdr[3] = "";
        Newdr[5] = 0;
        Newdr[6] = "";
        Newdr[7] = intSumNumber;
        Newdr[8] = "";
        Newdr[9] = 0;
        Newdr[10] = 0;
        Newdr[11] = "";
        Newdr[12] = intSumNumberReplace;
        dtTemp.Rows.Add(Newdr);
        if (blHaveOneGroup)
            dtTemp.DefaultView.Sort = "Type,Factory_ShortName_CN,Date_Time,Project_Name,Price,Replace_CardType_RID ";
        else
            dtTemp.DefaultView.Sort = "Group_Name,Factory_ShortName_CN,Date_Time,Project_Name,Price,Replace_CardType_RID ";

        return dtTemp.DefaultView.ToTable();
    }
    /// <summary>
    /// 計算群組的合計
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public DataTable getGroup_Name(DataTable dt)
    {
        DataTable dtTemp = dt.Copy();

        string strGroupName = "";
        string strLastGroupName = "";
        int intSumNumber = 0;
        DataRow Newdr = null;
        bool blHaveOneGroup = true;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            strGroupName = dt.Rows[i]["Group_Name"].ToString();
            if (strLastGroupName != "")
            {
                if (strGroupName != strLastGroupName)
                {
                    // 加合計
                    Newdr = dtTemp.NewRow();
                    Newdr[0] = 2;
                    Newdr[1] = strLastGroupName + " 合計";
                    Newdr[2] = 0;
                    Newdr[3] = "";
                    Newdr[5] = 0;
                    Newdr[6] = "";
                    Newdr[7] = intSumNumber;
                    Newdr[8] = "";
                    Newdr[9] = 0;
                    dtTemp.Rows.Add(Newdr);

                    strLastGroupName = strGroupName;
                    intSumNumber = 0;
                    blHaveOneGroup = false;
                }
                else
                {
                    intSumNumber += Convert.ToInt32(dt.Rows[i]["Number"]);
                }
            }
            else
            {
                strLastGroupName = strGroupName;
                intSumNumber = Convert.ToInt32(dt.Rows[i]["Number"]);
            }
        }

        // 加入最後一個群組的合計
        Newdr = dtTemp.NewRow();
        Newdr[0] = 2;
        if (blHaveOneGroup)   //只有一個群組時
            Newdr[1] = "合計";
        else
            Newdr[1] = strLastGroupName + " 合計";
        Newdr[2] = 0;
        Newdr[3] = "";
        Newdr[5] = 0;
        Newdr[6] = "";
        Newdr[7] = intSumNumber;
        Newdr[8] = "";
        Newdr[9] = 0;
        dtTemp.Rows.Add(Newdr);
        if (blHaveOneGroup)
            dtTemp.DefaultView.Sort = "Type,Factory_ShortName_CN,Date_Time,Project_Name,Price,CardType_RID ";
        else
            dtTemp.DefaultView.Sort = "Group_Name,Factory_ShortName_CN,Date_Time,Project_Name,Price,CardType_RID ";

        return dtTemp.DefaultView.ToTable();
    }

    /// <summary>
    /// 根據卡種和日期獲得代製項目訊息
    /// </summary>
    /// <param name="strCardType_RID">卡種RID</param>
    /// <param name="UseDate">卡片耗用日期</param>
    /// <returns></returns>    
    public DataTable SearchProjectCost(string strCardType_RID, DateTime UseDate,string strFactoryRID)
    {
        DataSet ds = null;
        dirValues.Clear();
        dirValues.Add("CardType_RID", strCardType_RID);
        dirValues.Add("Use_Date", UseDate.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo));
        dirValues.Add("perso_factory_rid", strFactoryRID);
        ds = dao.GetList(SEL_PROJECT_STEP_SUM, dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0];
        }
        else
        {
            return null;
        }
    }

    //汇出表格时新增資料到資料庫
    public void Export(DataTable dtFinance, string strTime)
    {
        dao.ExecuteNonQuery("delete RPT_Finance0023 where TimeMark<" + DateTime.Now.ToString("yyyyMMdd000000"));
        try
        {
            int intID = 0;
            foreach (DataRow dr in dtFinance.Rows)
            {
                dirValues.Clear();
                dirValues.Add("id", intID.ToString());
                dirValues.Add("Group_Name", dr["Group_Name"].ToString());
                dirValues.Add("Factory_ShortName_CN", dr["Factory_ShortName_CN"].ToString());
                dirValues.Add("Date_Time", dr["Date_Time"].ToString());
                dirValues.Add("Finance", dr["Finance"].ToString());
                dirValues.Add("Price", dr["Price"].ToString());
                dirValues.Add("Name", dr["Name"].ToString());
                dirValues.Add("Number", dr["Number"].ToString().Replace(",", ""));
                dirValues.Add("TimeMark", strTime);
                //
                dirValues.Add("Replace_Name", dr["Replace_Name"].ToString());
                dirValues.Add("Replace_Number", dr["Replace_Number"].ToString().Replace(",", ""));
                //
                dao.ExecuteNonQuery(IN_FINANCE0023, dirValues);
                intID++;
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 整理替換前與替換后的數據
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public DataTable getReplaceAll(DataTable resdt, DataTable resdtReplace)
    {
        resdt.Columns.Add("Replace_CardType_RID", Type.GetType("System.String"));
        resdt.Columns.Add("Replace_Name", Type.GetType("System.String"));
        resdt.Columns.Add("Replace_Number", Type.GetType("System.Int32"));
        DataTable resdtTemp = resdt.Copy();
        resdtTemp.Clear();
        //取所有卡種訊息
        InOut001BL BL001 = new InOut001BL();
        DataTable dtCardType = BL001.getCardType();
        //確定替換前版面對應的替換后版面的信息
        if (dtCardType.Rows.Count > 0 && resdt.Rows.Count > 0)
        {
            foreach (DataRow drowFileImp in resdt.Rows)
            {
                DataRow[] drFileImp = dtCardType.Select("RID='" + drowFileImp["CardType_RID"].ToString() + "'");
                    if (drFileImp.Length < 0)
                        continue;

                    if (Convert.ToInt32(drFileImp[0]["Change_Space_RID"]) != 0)
                    {
                        DataRow[] drCardType = dtCardType.Select("RID='" + drFileImp[0]["Change_Space_RID"].ToString() + "'");
                        if (drCardType.Length < 0)
                            continue;
                        drowFileImp["Replace_CardType_RID"] = drCardType[0]["RID"].ToString();
                        drowFileImp["Replace_Name"] = drCardType[0]["NAME"].ToString();
                       // drowFileImp["Replace_Number"] = "0"; 
                        drowFileImp["Replace_Number"] = drowFileImp["Number"].ToString(); 

                    }
                    else if (Convert.ToInt32(drFileImp[0]["Replace_Space_RID"]) != 0)
                    {
                        DataRow[] drCardType = dtCardType.Select("RID='" + drFileImp[0]["Replace_Space_RID"].ToString() + "'");
                        if (drCardType.Length < 0)
                            continue;
                        drowFileImp["Replace_CardType_RID"] = drCardType[0]["RID"].ToString();
                        drowFileImp["Replace_Name"] = drCardType[0]["NAME"].ToString();
                       // drowFileImp["Replace_Number"] = "0";
                        drowFileImp["Replace_Number"] = drowFileImp["Number"].ToString(); 
                    }
                    else
                    {

                        drowFileImp["Replace_CardType_RID"] = drowFileImp["CardType_RID"].ToString();
                        drowFileImp["Replace_Name"] = drowFileImp["Name"].ToString();
                        //drowFileImp["Replace_Number"] = "0";
                        drowFileImp["Replace_Number"] = drowFileImp["Number"].ToString();
                    }
            }
        }
        //
        //if (resdt.Rows.Count > 0 && resdtReplace.Rows.Count > 1)
        //{
        //    //合并相同待制費用項目的數據
        //    for (int i = 0; i < resdt.Rows.Count; i++)
        //    {
        //        DataRow dr = resdt.Rows[i];
        //        for (int t = 0; t < resdtReplace.Rows.Count; t++)
        //        {
        //            DataRow drReplace = resdtReplace.Rows[t];
        //            if (dr["Group_Name"].ToString() == drReplace["Group_Name"].ToString()
        //                && dr["Project_Name"].ToString() == drReplace["Project_Name"].ToString()
        //                && dr["RID"].ToString() == drReplace["RID"].ToString()
        //                && dr["Date_Time"].ToString() == drReplace["Date_Time"].ToString()
        //                && dr["Replace_CardType_RID"].ToString() == drReplace["CardType_RID"].ToString())
        //            {
        //                DataRow drTemp = resdtTemp.NewRow();
        //                drTemp["Type"] = dr["Type"].ToString();
        //                drTemp["RID"] = dr["RID"].ToString();
        //                drTemp["Group_Name"] = dr["Group_Name"].ToString();
        //                drTemp["Factory_ShortName_CN"] = dr["Factory_ShortName_CN"].ToString();
        //                drTemp["Date_Time"] = dr["Date_Time"].ToString();
        //                drTemp["CardType_RID"] = dr["CardType_RID"].ToString();
        //                drTemp["Name"] = dr["Name"].ToString();
        //                drTemp["Number"] = dr["Number"].ToString();
        //                drTemp["Project_Name"] = dr["Project_Name"].ToString();
        //                drTemp["Price"] = dr["Price"].ToString();
        //                drTemp["Replace_CardType_RID"] = drReplace["CardType_RID"].ToString();
        //                drTemp["Replace_Name"] = drReplace["Name"].ToString();                       
                        
        //                //當替換后的數量與替換前的數量不一致時
        //                if (int.Parse(drReplace["Number"].ToString()) > int.Parse(dr["Number"].ToString()))
        //                {
        //                    drTemp["Replace_Number"] = dr["Number"].ToString();
        //                    drReplace["Number"] = Convert.ToString(int.Parse(drReplace["Number"].ToString()) - int.Parse(dr["Number"].ToString()));
        //                }
        //                else
        //                {
        //                    drTemp["Replace_Number"] = drReplace["Number"].ToString();
        //                    resdtReplace.Rows.Remove(drReplace);
        //                }                            
        //                resdtTemp.Rows.Add(drTemp);
        //                resdt.Rows.Remove(dr);
        //                i--;                        
        //                break;
        //            }

        //        }

        //    }
  
        //}
        //添加待制費用項目不同的數據
        if (resdt.Rows.Count > 0)
        {
            foreach (DataRow dr1 in resdt.Rows)
            {
                DataRow dr = resdtTemp.NewRow();
                dr["Type"] = dr1["Type"].ToString();
                dr["RID"] = dr1["RID"].ToString();
                dr["Group_Name"] = dr1["Group_Name"].ToString();
                dr["Factory_ShortName_CN"] = dr1["Factory_ShortName_CN"].ToString();
                dr["Date_Time"] = dr1["Date_Time"].ToString();
                dr["CardType_RID"] = dr1["CardType_RID"].ToString();
                dr["Name"] = dr1["Name"].ToString();
                dr["Number"] = dr1["Number"].ToString();
                dr["Project_Name"] = dr1["Project_Name"].ToString();
                dr["Price"] = dr1["Price"].ToString();
                dr["Replace_CardType_RID"] = dr1["Replace_CardType_RID"].ToString();
                dr["Replace_Name"] = dr1["Replace_Name"].ToString();
                dr["Replace_Number"] = dr1["Replace_Number"].ToString();
                resdtTemp.Rows.Add(dr);
            }
        }
        //if (resdtReplace.Rows.Count > 0)
        //{
        //    foreach (DataRow dr1 in resdtReplace.Rows)
        //    {
        //        DataRow dr = resdtTemp.NewRow();
        //        dr["Type"] = dr1["Type"].ToString();
        //        dr["RID"] = dr1["RID"].ToString();
        //        dr["Group_Name"] = dr1["Group_Name"].ToString();
        //        dr["Factory_ShortName_CN"] = dr1["Factory_ShortName_CN"].ToString();
        //        dr["Date_Time"] = dr1["Date_Time"].ToString();
        //        dr["CardType_RID"] = "0";
        //        dr["Name"] = "";
        //        dr["Number"] = "0";
        //        dr["Project_Name"] = dr1["Project_Name"].ToString();
        //        dr["Price"] = dr1["Price"].ToString();
        //        dr["Replace_CardType_RID"] = dr1["CardType_RID"].ToString();
        //        dr["Replace_Name"] = dr1["Name"].ToString();
        //        dr["Replace_Number"] = dr1["Number"].ToString();
        //        resdtTemp.Rows.Add(dr);
        //    }
        //}
        return resdtTemp;
    }
}
