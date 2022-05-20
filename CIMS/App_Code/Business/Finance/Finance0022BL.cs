//******************************************************************
//*  作    者：JunWang
//*  功能說明：委外製卡費用作業邏輯 
//*  創建日期：2008-12-16
//*  修改日期：2008-12-16 9:00
//*  修改記錄：
//*            □2008-12-16
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
using System.Text;
using System.Collections;

/// <summary>
/// Finance0022BL 的摘要描述
/// </summary>
public class Finance0022BL : BaseLogic
{
    #region SQL語句
    public const string SEL_PARAM_FINANCE = "SELECT * FROM PARAM WHERE RST = 'A' AND Param_Code = '" + GlobalString.Parameter.Finance + "'";

    public const string SEL_CARD_GROUP = "SELECT RID,GROUP_NAME FROM CARD_GROUP WHERE RST='A' ";

    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Perso = 'Y' ";

    public const string SEL_WORK_DATE = "SELECT Date_Time FROM WORK_DATE WHERE RST = 'A' AND Is_WorkDay = 'Y' AND Date_Time >=@startdate AND Date_Time<=@enddate";

    public const string SEL_SURPLUS_DATE = "SELECT DISTINCT Stock_Date FROM CARDTYPE_STOCKS WHERE RST = 'A' AND Stock_Date >= @startdate AND Stock_Date<=@enddate";

    public const string SEL_SUBTOTAL_PROJECT_COST = "SELECT PPD.Use_Date,PP.Project_Name,PPD.Unit_Price,SUM(Number) as Number,SUM(Sum) as Sum FROM PERSO_PROJECT_DETAIL PPD LEFT JOIN PERSO_PROJECT PP ON PP.RST = 'A' AND PPD.Project_RID = PP.RID WHERE PPD.RST = 'A' AND PPD.Use_Date>=@Begin_Date AND PPD.Use_Date<=@Finish_Date AND PPD.Card_Group_RID = @card_group";

    public const string SEL_SPECIAL_PROJECT_COST = "SELECT SPPI.Project_Date,PP.Project_Name,PP.Unit_Price,SUM(SPPI.Number) as Number,SUM(SPPI.Number*PP.Unit_Price) as Sum FROM SPECIAL_PERSO_PROJECT_IMPORT SPPI INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' AND SPPI.PersoProject_RID = PP.RID WHERE SPPI.RST = 'A' AND SPPI.Project_Date >= @Begin_Date AND SPPI.Project_Date <= @Finish_Date";

    public const string SEL_EXCEPTION_PERSO_PROJECT = "SELECT Project_Date,Name,Unit_Price,SUM(Number) as Number,SUM(Number*Unit_Price) as sum FROM EXCEPTION_PERSO_PROJECT WHERE RST = 'A' AND Project_Date >= @Begin_Date AND Project_Date<= @Finish_Date AND CardGroup_RID = @card_group";

    public const string SEL_PERSO_PROJECT_CHANGE_DETAIL = "SELECT PPCD.Project_Date,P.Param_Name,SUM(Price) as Price FROM PERSO_PROJECT_CHANGE_DETAIL PPCD INNER JOIN PARAM P ON P.RST = 'A' AND PPCD.Param_Code = P.Param_Code AND P.ParamType_Code = '"+GlobalString.ParameterType.Finance+"' WHERE PPCD.RST = 'A' AND PPCD.Project_Date >= @Begin_Date AND PPCD.Project_Date<=@Finish_Date AND PPCD.CardGroup_RID = @card_group";

    public const string IN_ProjectCost_PRINT = "INSERT INTO RPT_Finance0022 (Date,Horizontal,Project_name,Number,Valign,TimeMark) VALUES (@date,@horizontal,@project_name,@number,@valign,@TimeMark) ";

    public const string IN_ProjectCost_PRINT1 = "INSERT INTO RPT_Finance0021 (Date,Horizontal,Project_name,Number,Valign,CardGroupRID,FactoryRID,SAPID,CountNum,TimeMark) VALUES (@date,@horizontal,@project_name,@number,@valign,@CardGroupRID,@FactoryRID,@SAPID,@CountNum,@TimeMark) ";
    public const string SEL_SUBTOTAL_PROJECT_COST_2 = "select t.Date_Time as Use_Date,t.Project_Name,t.Price as Unit_Price,SUM(Number) as Number,SUM(Sum) as Sum from" +
                                                   "(SELECT 1 as Type,CG.Group_Name,F.RID,F.Factory_ShortName_CN,SI.Date_Time,CT.RID CardType_RID,CT.Name,SI.Number,PP.Project_Name,PPP.Price,SI.Number*PPP.Price as Sum " +
                                                   "FROM (SELECT Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                                                          //"FROM SUBTOTAL_IMPORT " +
                                             //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/10/29 start
                                                          //"FROM SUBTOTAL_REPLACE_IMPORT " +
                                                         // "WHERE RST = 'A' " +
                                                             " from (SELECT  Date_Time,Perso_Factory_RID, " +
                                                            " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                                                            " FROM SUBTOTAL_REPLACE_IMPORT " +
                                                            " WHERE RST = 'A' " +
                                                            " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO " +
                                                            " union all " +
                                                            " SELECT  Date_Time,Perso_Factory_RID, " +
                                                            " TYPE,AFFINITY,PHOTO,SUM(Number) AS NUMBER " +
                                                            " FROM (select status_RID,Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO, " +
                                                            " Case status_RID when '5' then 0-Number when '6' then 0-Number when '7' then Number end as Number " +
                                                            " from FACTORY_CHANGE_REPLACE_IMPORT where Status_Rid in ('5','6','7') and RST = 'A') fci " +
                                                            " GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO) A " +
                                                  //200908CR代製費用計算用小計檔的「替換前」版面計算出的〞製成卡〞來計算 ADD BY 楊昆 2009/10/29 end
                                                          "GROUP BY Date_Time,Perso_Factory_RID,TYPE,AFFINITY,PHOTO) SI " +
                                                    "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' and SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                                                    "INNER JOIN FACTORY F ON F.RST = 'A' and SI.Perso_Factory_RID = F.RID " +
                                                    "INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID " +
                                                    "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID " +
                                                    "INNER JOIN PARAM P ON P.RST = 'A' AND CG.Param_Code = P.Param_Code AND P.Param_Code = '" + GlobalString.Parameter.Finance + "' " +
                                                    "INNER JOIN CARDTYPE_PERSO_PROJECT CPP ON CPP.RST = 'A' AND CT.RID = CPP.CardType_RID " +
                                                    "INNER JOIN (SELECT RID,PersoProject_RID,Convert(varchar(10),Use_Date_Begin,111) AS Use_Date_Begin,Convert(varchar(10),Use_Date_End,111) AS Use_Date_End " +
                                                                "FROM CARDTYPE_PROJECT_TIME " +
                                                                "WHERE RST = 'A' ) CPT ON CPP.ProjectTime_RID = CPT.RID AND CPT.Use_Date_Begin <= SI.Date_Time AND CPT.Use_Date_End >= SI.Date_Time " +
                                                    "INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' and si.Perso_Factory_RID = pp.factory_rid AND CPT.PersoProject_RID = PP.RID AND PP.Normal_Special = '1' " +
                                                    "INNER JOIN (SELECT Perso_Project_RID,Price,Convert(varchar(10),Use_Date_Begin,111) as Use_Date_Begin,Convert(varchar(10),Use_Date_End,111) as Use_Date_End " +
                                                                "FROM PERSO_PROJECT_PRICE " +
                                                                "WHERE RST = 'A' ) PPP ON CPT.PersoProject_RID = PPP.Perso_Project_RID AND PPP.Use_Date_Begin <= SI.Date_Time AND PPP.Use_Date_End >= SI.Date_Time " +
                                                    "WHERE 1=1  AND SI.Date_Time>= @BDate_Time AND SI.Date_Time<= @EDate_Time ";
    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Finance0022BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    /// <summary>
    /// 獲取用途
    /// </summary>
    /// <returns></returns>
    public DataSet getParam_Finance()
    {
        DataSet dstPurpose = null;

        try
        {
            dstPurpose = dao.GetList(SEL_PARAM_FINANCE);
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
            dirValues.Add("startdate", StartDate.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("enddate", EndDate.ToString("yyyy/MM/dd 23:59:59"));

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

    public string DataTableColumnNum(string strColName,
                                        Decimal dcPrice, 
                                        ref ArrayList al,
                                        ref ArrayList alPrice)
    {
        string strReturn = "";
        for (int n = 0; n < al.Count; n++)
        {
            if (al[n].ToString().Trim() == strColName)
            {
                if (Convert.ToDecimal(alPrice[n]) == dcPrice)//名稱和單價都相同
                {
                    return "";
                }
                else
                {
                    strReturn += " ";    
                }
            }
        }
        strReturn = strColName + strReturn;
        al.Add(strReturn);
        alPrice.Add(dcPrice);
        return strReturn;
    }

    public string DataTableColumnNum(string strColName, ref ArrayList al, ref ArrayList alPrice)
    {
        string strReturn = "";
        for (int n = 0; n < al.Count; n++)
        {
            if (al[n].ToString().Trim() == strColName)
                strReturn += " ";
        }
        strReturn = strColName + strReturn;
        al.Add(strReturn);
        alPrice.Add(0);
        return strReturn;
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
    public DataSet SearchProjectCost(Dictionary<string, object> searchInput)
    {
        DataSet ds = new DataSet();

        try
        {
            //取一般代製項目費用
            DataSet dsSUBTOTAL_PROJECT_COST = Get_SUBTOTAL_PROJECT_COST(searchInput);
            //如果群組是“磁條信用卡”，取特殊代製項目費用
            DataSet dsSPECIAL_PROJECT_COST = new DataSet();
            if (searchInput["dropCard_Group_txt"].ToString() == "磁條信用卡")
            {
                dsSPECIAL_PROJECT_COST = Get_SPECIAL_PROJECT_COST(searchInput);
            }

            //取例外代製項目費用
            DataSet dsEXCEPTION_PERSO_PROJECT = Get_EXCEPTION_PERSO_PROJECT(searchInput);

            //取帳務異動費用
            DataSet dsPERSO_PROJECT_CHANGE_DETAIL = Get_PERSO_PROJECT_CHANGE_DETAIL(searchInput);

            //動態創建DataTable<代製費用明細>
            DataTable dtPERSO_PROJECT_DETAIL = new DataTable();
            int intNormalNum = 0;//一般項目個數
            int intSpecialNum = 0;//特殊項目個數
            int intExcepNum = 0;//例外項目個數

            ArrayList al = new ArrayList();
            ArrayList alPrice = new ArrayList();

            //1、添加列作業項目日期
            dtPERSO_PROJECT_DETAIL.Columns.Add(new DataColumn(DataTableColumnNum("作業項目日期", ref al,ref alPrice)));

            //2、添加一般代製項目列
            List<object> listGENERIC = new List<object>();
            //string strPROJECT_NAME = "";
            //decimal decUNIT_PRICE = 0;
            if (dsSUBTOTAL_PROJECT_COST.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in dsSUBTOTAL_PROJECT_COST.Tables[0].Rows)
                {

                    string colName = DataTableColumnNum(dr["Project_Name"].ToString(),
                                                        Convert.ToDecimal(dr["Unit_Price"]),
                                                        ref al,
                                                        ref alPrice);
                    if (colName != "")
                    {
                        dtPERSO_PROJECT_DETAIL.Columns.Add(new DataColumn(colName, typeof(string)));
                        listGENERIC.Add(colName + "," + Convert.ToDecimal(dr["Unit_Price"]));

                        intNormalNum++;
                    }
                }
                //3、添加一般代製項目的“總製卡數”
                dtPERSO_PROJECT_DETAIL.Columns.Add(new DataColumn(DataTableColumnNum("總製卡數", ref al,ref alPrice), typeof(int)));
            }
            
            //4、添加特殊代製項目列
            List<object> listSPECIAL = new List<object>();
            if (dsSPECIAL_PROJECT_COST.Tables.Count > 0)
            {
                if (dsSPECIAL_PROJECT_COST.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow dr in dsSPECIAL_PROJECT_COST.Tables[0].Rows)
                    {

                        string colName = DataTableColumnNum(dr["Project_Name"].ToString(),Convert.ToDecimal(dr["Unit_Price"]), ref al,ref alPrice);
                        if (colName == "")
                            continue;
                        dtPERSO_PROJECT_DETAIL.Columns.Add(new DataColumn(colName, typeof(string)));
                        listSPECIAL.Add(colName + "," + Convert.ToDecimal(dr["Unit_Price"]));
                        intSpecialNum++;
                    }
                }
            }

            //5、添加例外代製項目列
            List<object> listEXCEPTION = new List<object>();
            if (dsEXCEPTION_PERSO_PROJECT.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsEXCEPTION_PERSO_PROJECT.Tables[0].Rows)
                {

                    string colName = DataTableColumnNum(dr["Name"].ToString(),Convert.ToDecimal(dr["Unit_Price"]), ref al,ref alPrice);
                    if (colName == "")
                        continue;
                    dtPERSO_PROJECT_DETAIL.Columns.Add(new DataColumn(colName, typeof(string)));
                    listEXCEPTION.Add(colName + "," + Convert.ToDecimal(dr["Unit_Price"]));
                    intExcepNum++;
                }
            }
            //6、添加“帳款總計”列
            dtPERSO_PROJECT_DETAIL.Columns.Add(new DataColumn(DataTableColumnNum("帳款總計", ref al,ref alPrice)));


            //整理代製費用，然後添加至DataTable<代製費用明細>表中
            
            int Sum_Array_Len = listGENERIC.ToArray().Length + listSPECIAL.ToArray().Length + listEXCEPTION.ToArray().Length;
            int[] Number = new int[Sum_Array_Len];//計數量
            Decimal[] Unit_Price = new decimal[Sum_Array_Len];//單價
            Decimal[] Sum = new decimal[Sum_Array_Len];//小計金額 
            DateTime Date = Convert.ToDateTime("1900/01/01");
            DateTime Begin_Date = Convert.ToDateTime(searchInput["txtBegin_Date"].ToString());
            DateTime Finish_Date = Convert.ToDateTime(searchInput["txtFinish_Date"].ToString());
            for (Date = Begin_Date; Date <= Finish_Date; Date = Date.AddDays(1))
            {
                bool IsGENERIC = false;
                //(計算一般代製費用)
                Decimal[] decGENERIC_NUMBER = new Decimal[listGENERIC.ToArray().Length];
                //Decimal[] decGENERIC_NUMBER = new Decimal[listGENERIC.ToArray().Length+1];
                if (listGENERIC.Count > 0)
                {
                    if (dsSUBTOTAL_PROJECT_COST.Tables.Count > 0)
                    {
                        object[] arrGENERIC = (object[])listGENERIC.ToArray();//一般

                        for (int int1 = 0; int1 < decGENERIC_NUMBER.Length; int1++)
                        {
                            DataRow[] dr = dsSUBTOTAL_PROJECT_COST.Tables[0].Select("Use_Date ='" + Convert.ToDateTime(Date).ToString("yyyy-MM-dd") + "' AND Project_Name ='" + arrGENERIC[int1].ToString().Split(',')[0].Trim() + "' AND Unit_Price = '" + arrGENERIC[int1].ToString().Split(',')[1] + "'");
                            if (dr.Length > 0)
                            {
                                IsGENERIC = true;
                                decGENERIC_NUMBER[int1] = Convert.ToDecimal(dr[0]["Number"]);
                                Unit_Price[int1] = Convert.ToDecimal(dr[0]["Unit_Price"]);
                                Number[int1] += Convert.ToInt32(dr[0]["Number"]);
                                Sum[int1] += Convert.ToDecimal(dr[0]["Number"]) * Convert.ToDecimal(dr[0]["Unit_Price"]);
                            }
                            else
                            {
                                decGENERIC_NUMBER[int1] = 0;
                            }
                        }
                    }
                }
                bool IsSPECIAL = false;

                //(計算特殊代製費用)
                Decimal[] decSPECIAL = new Decimal[listSPECIAL.ToArray().Length];
                if (listSPECIAL.Count > 0)
                {
                    if (dsSPECIAL_PROJECT_COST.Tables.Count > 0)
                    {
                        if (listSPECIAL.Count > 0)
                        {
                            object[] arrSPECIAL = (object[])listSPECIAL.ToArray();//特殊

                            if (arrSPECIAL.Length > 0)
                            {
                                for (int int1 = 0; int1 < decSPECIAL.Length; int1++)
                                {
                                    DataRow[] dr = dsSPECIAL_PROJECT_COST.Tables[0].Select("Project_Date = '" + Convert.ToDateTime(Date).ToString("yyyy-MM-dd") + "' AND Project_Name = '" + arrSPECIAL[int1].ToString().Split(',')[0].Trim() + "' AND Unit_Price = '" + arrSPECIAL[int1].ToString().Split(',')[1] + "'");
                                    if (dr.Length > 0)
                                    {
                                        IsSPECIAL = true;
                                        decSPECIAL[int1] = Convert.ToDecimal(dr[0]["Number"]);

                                        Unit_Price[listGENERIC.Count + int1] = Convert.ToDecimal(dr[0]["Unit_Price"]);

                                        Number[listGENERIC.Count + int1] += Convert.ToInt32(dr[0]["Number"]);

                                        Sum[listGENERIC.Count + int1] += Convert.ToDecimal(dr[0]["Number"]) * Convert.ToDecimal(dr[0]["Unit_Price"]);
                                    }
                                    else
                                    {
                                        decSPECIAL[int1] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                bool IsEXCEPTION = false;

                //(計算例外代製費用)
                Decimal[] decEXCEPTION = new Decimal[listEXCEPTION.ToArray().Length];
                if (listEXCEPTION.Count > 0)
                {
                    if (dsEXCEPTION_PERSO_PROJECT.Tables.Count > 0)
                    {
                        object[] arrEXCEPTION = (object[])listEXCEPTION.ToArray();//例外


                        if (arrEXCEPTION.Length > 0)
                        {
                            for (int int1 = 0; int1 < decEXCEPTION.Length; int1++)
                            {
                                DataRow[] dr = dsEXCEPTION_PERSO_PROJECT.Tables[0].Select("Project_Date = '" + Convert.ToDateTime(Date).ToString("yyyy-MM-dd") + "' AND Name = '" + arrEXCEPTION[int1].ToString().Split(',')[0].Trim() + "' AND Unit_Price = '" + arrEXCEPTION[int1].ToString().Split(',')[1] + "'");
                                if (dr.Length > 0)
                                {
                                    IsEXCEPTION = true;
                                    decEXCEPTION[int1] = Convert.ToDecimal(dr[0]["Number"]);

                                    Unit_Price[listGENERIC.Count + listSPECIAL.Count + int1] = Convert.ToDecimal(dr[0]["Unit_Price"]);

                                    Number[listGENERIC.Count + listSPECIAL.Count + int1] += Convert.ToInt32(dr[0]["Number"]);

                                    Sum[listGENERIC.Count + listSPECIAL.Count + int1] += Convert.ToDecimal(dr[0]["Number"]) * Convert.ToDecimal(dr[0]["Unit_Price"]);
                                }
                                else
                                {
                                    decEXCEPTION[int1] = 0;
                                }
                            }
                        }
                    }
                }
                //如果一般代製費用中有記錄 || 特殊代製費用中有記錄 || 例外代製費用中有記錄
                DataRow drow = dtPERSO_PROJECT_DETAIL.NewRow();
                if (IsGENERIC)
                {
                    //將訊息插入DataTable<代製費用明細>中

                    drow[0] = Date.ToShortDateString();
                    int intSumDayNum = 0;
                    for (int intDetail = 1; intDetail < 1 + intNormalNum; intDetail++)
                    {
                        drow[intDetail] = decGENERIC_NUMBER[intDetail - 1];
                        intSumDayNum += Convert.ToInt32(decGENERIC_NUMBER[intDetail - 1]);
                        drow[1 + intNormalNum] = intSumDayNum;                      
                    }
                }
                if (IsSPECIAL)
                {
                    if (intNormalNum == 0)
                    {
                        for (int intDetail = 1 + intNormalNum; intDetail < 1 + intNormalNum + intSpecialNum; intDetail++)
                        {
                            drow[intDetail] = decSPECIAL[intDetail - 1 - intNormalNum];
                        }
                    }
                    else
                    {
                        for (int intDetail = 2 + intNormalNum; intDetail < 2 + intNormalNum + intSpecialNum; intDetail++)
                        {
                            drow[intDetail] = decSPECIAL[intDetail - 2 - intNormalNum];
                        }
                    }
                }
                if (IsEXCEPTION)
                {
                    if (intNormalNum == 0)
                    {
                        for (int intDetail = 1 + intNormalNum + intSpecialNum; intDetail < 1 + intNormalNum + intSpecialNum + intExcepNum; intDetail++)
                        {
                            drow[intDetail] = decEXCEPTION[intDetail - 1 - intNormalNum - intSpecialNum];
                        }
                    }
                    else
                    {
                        for (int intDetail = 2 + intNormalNum + intSpecialNum; intDetail < 2 + intNormalNum + intSpecialNum + intExcepNum; intDetail++)
                        {
                            drow[intDetail] = decEXCEPTION[intDetail - 2 - intNormalNum - intSpecialNum];
                        }
                    }
                }
               // drow["帳款總計"] = 0;  //200907IR
                drow["作業項目日期"] = Date.ToString("yyyy/MM/dd");
                bool IsAdd = false;
                for (int CountNum = 1; CountNum < dtPERSO_PROJECT_DETAIL.Columns.Count - 1; CountNum++)
                {
                    if (drow[CountNum].ToString() != "")
                    {
                        IsAdd = true;
                        break;
                    }
                }
                if (IsAdd)
                    dtPERSO_PROJECT_DETAIL.Rows.Add(drow);
            }
            //添加數量小計
            DataRow dr1 = dtPERSO_PROJECT_DETAIL.NewRow();
            dr1[0] = "數量小計";
            int intSumTotalDayNum = 0;//數量合計 200907IR
            for (int intSumNum = 0; intSumNum < intNormalNum; intSumNum++)
            {
                dr1[intSumNum + 1] = Number[intSumNum];
                intSumTotalDayNum += Number[intSumNum];
            }
            if (intNormalNum == 0)
            {
                for (int intSumNum = 0; intSumNum < intSpecialNum; intSumNum++)
                {
                    dr1[intSumNum + intNormalNum + 1] = Number[intSumNum + intNormalNum];
                }
                for (int intSumNum = 0; intSumNum < intExcepNum; intSumNum++)
                {
                    dr1[intSumNum + intNormalNum + 1 + intSpecialNum] = Number[intSumNum + intNormalNum + intSpecialNum];
                }
            }
            else
            {
                for (int intSumNum = 0; intSumNum < intSpecialNum; intSumNum++)
                {
                    dr1[intSumNum + intNormalNum + 2] = Number[intSumNum + intNormalNum];
                }
                for (int intSumNum = 0; intSumNum < intExcepNum; intSumNum++)
                {
                    dr1[intSumNum + intNormalNum + 2 + intSpecialNum] = Number[intSumNum + intNormalNum + intSpecialNum];
                }
            }
            dtPERSO_PROJECT_DETAIL.Rows.Add(dr1);

            //添加單價行
            DataRow dr2 = dtPERSO_PROJECT_DETAIL.NewRow();
            dr2[0] = "單價";
            for (int intSumNum = 0; intSumNum < intNormalNum; intSumNum++)
            {
                dr2[intSumNum + 1] = Unit_Price[intSumNum];
            }
            if (intNormalNum == 0)
            {
                for (int intSumNum = 0; intSumNum < intSpecialNum; intSumNum++)
                {
                    dr2[intSumNum + 1] = Unit_Price[intSumNum];
                }
                for (int intSumNum = 0; intSumNum < intExcepNum; intSumNum++)
                {
                    dr2[intSumNum + 1 + intSpecialNum] = Unit_Price[intSumNum + intSpecialNum];
                }
            }
            else
            {
                for (int intSumNum = 0; intSumNum < intSpecialNum; intSumNum++)
                {
                    dr2[intSumNum + intNormalNum + 2] = Unit_Price[intSumNum + intNormalNum];
                }
                for (int intSumNum = 0; intSumNum < intExcepNum; intSumNum++)
                {
                    dr2[intSumNum + intNormalNum + 2 + intSpecialNum] = Unit_Price[intSumNum + intNormalNum + intSpecialNum];
                }
            }
            dtPERSO_PROJECT_DETAIL.Rows.Add(dr2);

            //添加“總金額”
            DataRow dr3 = dtPERSO_PROJECT_DETAIL.NewRow();
            dr3[0] = "總金額";
            Decimal decSum = 0;//帳款總計
            for (int intSumNum = 0; intSumNum < intNormalNum; intSumNum++)
            {
                dr3[intSumNum + 1] = Sum[intSumNum];
                decSum += Sum[intSumNum];
            }
            if (intNormalNum == 0)
            {
                for (int intSumNum = 0; intSumNum < intSpecialNum; intSumNum++)
                {
                    dr3[intSumNum + 1] = Sum[intSumNum];
                    decSum += Sum[intSumNum];
                }
                for (int intSumNum = 0; intSumNum < intExcepNum; intSumNum++)
                {
                    dr3[intSumNum + 1 + intSpecialNum] = Sum[intSumNum + intSpecialNum];
                    decSum += Sum[intSumNum + intSpecialNum];
                    dr3[intSpecialNum + intExcepNum+1] = decSum;
                }
            }
            else
            {
                for (int intSumNum = 0; intSumNum < intSpecialNum; intSumNum++)
                {
                    dr3[intSumNum + intNormalNum + 2] = Sum[intSumNum + intNormalNum];
                    decSum += Sum[intSumNum + intNormalNum];
                }
                for (int intSumNum = 0; intSumNum < intExcepNum; intSumNum++)
                {
                    dr3[intSumNum + intNormalNum + 2 + intSpecialNum] = Sum[intSumNum + intNormalNum + intSpecialNum];
                    decSum += Sum[intSumNum + intNormalNum + intSpecialNum];
                    dr3[intNormalNum + intSpecialNum + intExcepNum+2] = decSum;
                }
            }
            dtPERSO_PROJECT_DETAIL.Rows.Add(dr3);

            if (dsPERSO_PROJECT_CHANGE_DETAIL.Tables.Count > 0)
            {
                //代製費用帳務異動添加
                if (dsPERSO_PROJECT_CHANGE_DETAIL.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow drows in dsPERSO_PROJECT_CHANGE_DETAIL.Tables[0].Rows)
                    {
                        DataRow dr4 = dtPERSO_PROJECT_DETAIL.NewRow();
                        dr4[0] = drows["Param_Name"].ToString();
                        dr4[dtPERSO_PROJECT_DETAIL.Columns.Count - 1] = "("+drows[2].ToString()+")";//Price
                        decSum -= Convert.ToDecimal(drows[2]);//Price
                        dtPERSO_PROJECT_DETAIL.Rows.Add(dr4);
                    }
                }
            }

            //添加“合計”
            DataRow dr5 = dtPERSO_PROJECT_DETAIL.NewRow();
            dr5[0] = "合計";
            dr5[dtPERSO_PROJECT_DETAIL.Columns.Count - 1] = decSum;
            dr5[intNormalNum + 1] = intSumTotalDayNum;
            dtPERSO_PROJECT_DETAIL.Rows.Add(dr5);

            ds.Tables.Add(dtPERSO_PROJECT_DETAIL);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return ds;
    }

    //取一般代製項目費用
    private DataSet Get_SUBTOTAL_PROJECT_COST(Dictionary<string, object> searchInput)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                //dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString());                
                dirValues.Add("BDate_Time", searchInput["txtBegin_Date"].ToString().Trim());
            }
            else
            {
                //dirValues.Add("Begin_Date", "1900/01/01");
                dirValues.Add("BDate_Time", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                //dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString());
                dirValues.Add("EDate_Time", searchInput["txtFinish_Date"].ToString());
            }
            else
            {
                //dirValues.Add("Finish_Date", "9999/12/31");
                dirValues.Add("EDate_Time", "9999/12/31");
            }

            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                //stbWhere.Append(" AND PPD.Perso_Factory_RID = @factory");
                //dirValues.Add("factory", searchInput["dropFactory"].ToString().Trim());
                stbWhere.Append(" AND SI.Perso_Factory_RID = @Factory ");
                dirValues.Add("Factory", searchInput["dropFactory"].ToString().Trim());
            }
 
            if (!StringUtil.IsEmpty(searchInput["dropCard_Group"].ToString().Trim()))
            {
                //dirValues.Add("card_group", searchInput["dropCard_Group"].ToString().Trim());
                stbWhere.Append(" AND CG.RID = @Card_Group_RID ");
                dirValues.Add("Card_Group_RID", searchInput["dropCard_Group"].ToString().Trim());
            }
        }
        //DataSet dsProjectCost = dao.GetList(SEL_SUBTOTAL_PROJECT_COST + stbWhere.ToString() + " group by PPD.Use_Date,PP.Project_Name,PPD.Unit_Price", dirValues);
        DataSet dsProjectCost = dao.GetList(SEL_SUBTOTAL_PROJECT_COST_2 + stbWhere.ToString() + " ) t  group by t.Date_Time ,t.Project_Name,t.Price", dirValues);
        return dsProjectCost;
    }

    //取特殊代製項目費用
    private DataSet Get_SPECIAL_PROJECT_COST(Dictionary<string, object> searchInput)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString());
            }
            else
            {
                dirValues.Add("Begin_Date", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString());
            }
            else
            {
                dirValues.Add("Finish_Date", "9999/12/31");
            }

            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND SPPI.Perso_Factory_RID  = @factory");
                dirValues.Add("factory", searchInput["dropFactory"].ToString().Trim());
            }

            //if (!StringUtil.IsEmpty(searchInput["dropCard_Group"].ToString().Trim()))
            //{
            //    dirValues.Add("card_group", searchInput["dropCard_Group"].ToString().Trim());
            //}
        }
        DataSet dsSPECIAL_PROJECT_COST = dao.GetList(SEL_SPECIAL_PROJECT_COST + stbWhere.ToString() + " group by SPPI.Project_Date,PP.Project_Name,PP.Unit_Price", dirValues);
        return dsSPECIAL_PROJECT_COST;
    }

    //取例外代製項目費用
    private DataSet Get_EXCEPTION_PERSO_PROJECT(Dictionary<string, object> searchInput)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString());
            }
            else
            {
                dirValues.Add("Begin_Date", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString());
            }
            else
            {
                dirValues.Add("Finish_Date", "9999/12/31");
            }

            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND Perso_Factory_RID  = @factory");
                dirValues.Add("factory", searchInput["dropFactory"].ToString().Trim());
            }

            if (!StringUtil.IsEmpty(searchInput["dropCard_Group"].ToString().Trim()))
            {
                dirValues.Add("card_group", searchInput["dropCard_Group"].ToString().Trim());
            }
        }
        DataSet dsEXCEPTION_PERSO_PROJECT = dao.GetList(SEL_EXCEPTION_PERSO_PROJECT + stbWhere.ToString() + " group by Project_Date,Name,Unit_Price", dirValues);
        return dsEXCEPTION_PERSO_PROJECT;
    }

    //取帳務異動費用
    private DataSet Get_PERSO_PROJECT_CHANGE_DETAIL(Dictionary<string, object> searchInput)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString());
            }
            else
            {
                dirValues.Add("Begin_Date", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString());
            }
            else
            {
                dirValues.Add("Finish_Date", "9999/12/31");
            }

            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND Perso_Factory  = @factory");
                dirValues.Add("factory", searchInput["dropFactory"].ToString().Trim());
            }

            if (!StringUtil.IsEmpty(searchInput["dropCard_Group"].ToString().Trim()))
            {
                dirValues.Add("card_group", searchInput["dropCard_Group"].ToString().Trim());
            }
        }
        DataSet dsPERSO_PROJECT_CHANGE_DETAIL = dao.GetList(SEL_PERSO_PROJECT_CHANGE_DETAIL + stbWhere.ToString() + " group by PPCD.Project_Date,P.Param_Name", dirValues);
        return dsPERSO_PROJECT_CHANGE_DETAIL;
    }

    //汇出表格时新增資料到資料庫
    public void ADD_CARD_YEAR_FORCAST_PRINT(DataTable dtProjectCost, string strTime)
    {
        dao.ExecuteNonQuery("delete RPT_Finance0022 where TimeMark<" + DateTime.Now.ToString("yyyyMMdd000000"));
        try
        {
            for (int i = 0; i < dtProjectCost.Rows.Count; i++)
            {
                dirValues.Clear();
                dirValues.Add("date", dtProjectCost.Rows[i][0].ToString());
                dirValues.Add("horizontal", i);
                dirValues.Add("TimeMark", strTime);
                for (int j = 1; j < dtProjectCost.Columns.Count; j++)
                {
                    dirValues.Remove("project_name");
                    dirValues.Remove("number");
                    dirValues.Remove("valign");
                    dirValues.Add("project_name", dtProjectCost.Columns[j].ColumnName);                    
                    if (!StringUtil.IsEmpty(dtProjectCost.Rows[i][j].ToString()))
                    {
                        if (dtProjectCost.Rows[i][0].ToString().Contains("/") )
                            dirValues.Add("number", Convert.ToInt32(dtProjectCost.Rows[i][j].ToString()).ToString("N0"));
                        else if (dtProjectCost.Rows[i][0].ToString().Contains("合計") && dtProjectCost.Rows[i][j].ToString().Trim() != "")
                            dirValues.Add("number", Convert.ToDecimal(dtProjectCost.Rows[i][j].ToString()).ToString("N0"));
                        else if (dtProjectCost.Rows[i][j].ToString().Contains("."))
                            dirValues.Add("number", Convert.ToDecimal(dtProjectCost.Rows[i][j].ToString()).ToString("N4"));
                        else
                            dirValues.Add("number", Convert.ToInt32(dtProjectCost.Rows[i][j].ToString()).ToString("N0"));
                    }
                    else
                    {
                        //if (dtProjectCost.Rows[i][0].ToString().Contains("/"))
                        //    dirValues.Add("number", "0");
                        //else
                        //    dirValues.Add("number", " ");
                        dirValues.Add("number", "0");
                    }
                    dirValues.Add("valign", j);
                    
                    dao.ExecuteNonQuery(IN_ProjectCost_PRINT, dirValues);
                }
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
    }

    public void F0021Print(GridView gvSAP, DateTime Begin_Date, DateTime End_Date, DataTable dtSAP, string strTime)
    {
        try
        {
            dao.OpenConnection();

            dao.ExecuteNonQuery("delete RPT_Finance0021 where TimeMark<" + DateTime.Now.ToString("yyyyMMdd000000"));

            for (int i = 0; i < gvSAP.Rows.Count; i++)
            {
                TextBox txt1 = (TextBox)gvSAP.Rows[i].FindControl("txt1");//SAP單號
                TextBox txt4 = (TextBox)gvSAP.Rows[i].FindControl("txt4");//發票號碼

                if (StringUtil.IsEmpty(txt1.Text))
                    continue;

                Dictionary<string, object> inputs = new Dictionary<string, object>();
                inputs.Add("dropCard_Group", dtSAP.Rows[i]["Group_RID"].ToString());
                inputs.Add("dropCard_Group_txt", gvSAP.Rows[i].Cells[1].Text);
                inputs.Add("dropFactory", dtSAP.Rows[i]["Perso_Factory_RID"].ToString());
                inputs.Add("txtBegin_Date", Convert.ToDateTime(Begin_Date).ToString("yyyy-MM-dd"));
                inputs.Add("txtFinish_Date", Convert.ToDateTime(End_Date).ToString("yyyy-MM-dd"));

                DataSet dsProjectCost = SearchProjectCost(inputs);
                if (dsProjectCost != null)
                {
                    if (dsProjectCost.Tables.Count > 0)
                    {
                        ADD_F0021ToPrint(dsProjectCost.Tables[0], dtSAP.Rows[i]["Group_RID"].ToString(), dtSAP.Rows[i]["Perso_Factory_RID"].ToString(), txt1.Text, txt4.Text, strTime);
                    }
                }

            }
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

    public void F0021SearchPrint(DataRow drow, string strSAPID, string strCountNum, string strTime)
    {
        try
        {
            dao.OpenConnection();

            dao.ExecuteNonQuery("delete RPT_Finance0021 where TimeMark<" + DateTime.Now.ToString("yyyyMMdd000000"));

            Dictionary<string, object> inputs = new Dictionary<string, object>();

            inputs.Add("dropCard_Group", drow["Group_RID"].ToString());
            inputs.Add("dropCard_Group_txt", drow["Group_Name"].ToString());
            inputs.Add("dropFactory", drow["Perso_Factory_RID"].ToString());
            inputs.Add("txtBegin_Date", Convert.ToDateTime(drow["Begin_Date"].ToString()).ToString("yyyy-MM-dd"));
            inputs.Add("txtFinish_Date", Convert.ToDateTime(drow["End_Date"].ToString()).ToString("yyyy-MM-dd"));



            DataSet dsProjectCost = SearchProjectCost(inputs);
            if (dsProjectCost != null)
            {
                if (dsProjectCost.Tables.Count > 0)
                {
                    ADD_F0021ToPrint(dsProjectCost.Tables[0], drow["Group_RID"].ToString(), drow["Perso_Factory_RID"].ToString(), strSAPID, strCountNum, strTime);
                }
            }

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


    //汇出表格时新增資料到資料庫
    public void ADD_F0021ToPrint(DataTable dtProjectCost, string strGroupRID, string strFactoryRID, string strSAPID, string strCountNum, string strTime)
    {
        try
        {
            for (int i = 0; i < dtProjectCost.Rows.Count; i++)
            {
                dirValues.Clear();
                dirValues.Add("date", dtProjectCost.Rows[i][0].ToString());
                dirValues.Add("horizontal", i);
                dirValues.Add("CardGroupRID", strGroupRID);
                dirValues.Add("FactoryRID", strFactoryRID);
                dirValues.Add("SAPID", strSAPID);
                dirValues.Add("CountNum", strCountNum);
                dirValues.Add("TimeMark", strTime);

                for (int j = 1; j < dtProjectCost.Columns.Count; j++)
                {
                    dirValues.Remove("project_name");
                    dirValues.Remove("number");
                    dirValues.Remove("valign");

                    dirValues.Add("project_name", dtProjectCost.Columns[j].ColumnName);

                    if (!StringUtil.IsEmpty(dtProjectCost.Rows[i][j].ToString()))
                        dirValues.Add("number", dtProjectCost.Rows[i][j].ToString());
                    else
                        dirValues.Add("number", "0.0");

                    dirValues.Add("valign", j);
                    

                    dao.ExecuteNonQuery(IN_ProjectCost_PRINT1, dirValues);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}


