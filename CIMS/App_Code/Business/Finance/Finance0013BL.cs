//******************************************************************
//*  作    者：JunWang
//*  功能說明：請款放行作業邏輯 
//*  創建日期：2008-12-03
//*  修改日期：2008-12-03 9:00
//*  修改記錄：
//*            □2008-12-03
//*              1.創建 王俊
//*             2010/12/10  Ge.Song
//*                 RQ-2010-004324-000 空白卡請款-遲繳天數開放負數
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
/// Finance0013BL 的摘要描述
/// </summary>
public class Finance0013BL : BaseLogic
{
    #region SQL語句
    public const string SEL_PARAM_USE = "SELECT * FROM PARAM WHERE RST = 'A' AND ParamType_Code = '" + GlobalString.ParameterType.Use + "'";

    public const string SEL_CARD_GROUP = "SELECT RID,GROUP_NAME FROM CARD_GROUP WHERE RST='A' ";

    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN FROM FACTORY WHERE RST = 'A' AND Is_Blank = 'Y' ";

    public const string SEL_FINANCE = "SELECT A.RID,A.Operate_RID,A.CARDTYPE_RID,A.NAME,A.FRID,A.Factory_ShortName_CN,A.Budget_ID,A.Agreement_Code," +
                        "A.Stock_RID,A.Operate_Type,A.Income_Number,A.Income_Date,A.Unit_Price,A.Unit_Price1,A.Fore_Delivery_Date,A.Delay_Days,A.Comment,A.SAP_ID,A.Check_ID,A.RIC_Number,A.Ask_Date,A.Pay_Date  " +
                    "FROM (SELECT Is_AskFinance,0 AS RID,DS.RID as Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,DS.Stock_RID,OFD.OrderForm_Detail_RID," +
                        "'1' AS Operate_Type,DS.Income_Number,DS.Income_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DS.Comment,'' AS SAP_ID,'' as Check_ID,DS.Income_Number AS RIC_Number" +
                        ",'1900-01-01' AS Ask_Date,'1900-01-01' as Pay_Date,'0' AS Pass_Status,DS.Is_Finance " + //"" as Is_Finance
                          "FROM DEPOSITORY_STOCK DS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DS.Space_Short_RID = CT.RID " +
                            "INNER JOIN Factory F ON DS.Blank_Factory_RID = F.RID " +
                            "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND DS.Budget_RID = CB.RID " +
                            "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND DS.Agreement_RID = AGM.RID " +
                            "INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DS.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                          "WHERE DS.RST = 'A' " +
                            "AND DS.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '1' ) " +
                          "UNION " +
                          "SELECT Is_AskFinance,0 AS RID,DR.RID as Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,DR.Stock_RID,OFD.OrderForm_Detail_RID," +
                            "'2' AS Operate_Type,DR.Reincome_Number,DR.Reincome_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DR.Comment,'' AS SAP_ID,'' as Check_ID,DR.ReIncome_Number AS RIC_Number" +
                            ",'1900-01-01' AS Ask_Date,'1900-01-01' as Pay_Date,'0' AS Pass_Status,DR.Is_Finance " + //"" as Is_Finance
                          "FROM DEPOSITORY_RESTOCK DR INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DR.Space_Short_RID = CT.RID " +
                            "INNER JOIN Factory F ON DR.Blank_Factory_RID = F.RID " +
                            "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND DR.Budget_RID = CB.RID " +
                            "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND DR.Agreement_RID = AGM.RID " +
                            "INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DR.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                          "WHERE DR.RST = 'A'  " +
                            "AND DR.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '2') " +
                          "UNION " +
                          "SELECT Is_AskFinance,0 AS RID,DC.RID as Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,DC.Stock_RID,OFD.OrderForm_Detail_RID," +
                            "'3' AS Operate_Type,DC.Cancel_Number,DC.Cancel_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DC.Comment,'' AS SAP_ID,'' as Check_ID,DC.Cancel_Number AS RIC_Number" +
                            ",'1900-01-01' AS Ask_Date,'1900-01-01' as Pay_Date,'0' AS Pass_Status,DC.Is_Finance " + //"" as Is_Finance
                          "FROM DEPOSITORY_CANCEL DC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DC.Space_Short_RID = CT.RID " +
                            "INNER JOIN Factory F ON DC.Blank_Factory_RID = F.RID " +
                            "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND DC.Budget_RID = CB.RID " +
                            "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND DC.Agreement_RID = AGM.RID " +
                            "INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DC.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                          "WHERE DC.RST = 'A' " +
                            "AND DC.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '3' ) " +
                          "UNION " +
                          "SELECT case isnull(B.SAP_Serial_Number,'') when '' then 'N' else 'Y' end as Is_AskFinance,B.RID,B.Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,B.Stock_RID,B.OrderForm_Detail_RID,B.Operate_Type," +
                            "B.Real_Ask_Number,B.Income_Date,B.Unit_Price,B.Unit_Price_No,OFD.Fore_Delivery_Date,B.Delay_Days,B.Comment,B.SAP_Serial_Number,B.Check_Serial_Number AS Check_ID,B.RIC_Number," +
                            "B.Ask_Date,B.Pay_Date,B.Pass_Status,B.Is_Finance " +
                          "FROM (SELECT CTSD.RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Budget_RID WHEN '2' THEN DR.Budget_RID WHEN '3' THEN DC.Budget_RID END AS Budget_RID," +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Blank_Factory_RID WHEN '2' THEN DR.Blank_Factory_RID WHEN '3' THEN DC.Blank_Factory_RID END AS Blank_Factory_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Agreement_RID WHEN '2' THEN DR.Agreement_RID WHEN '3' THEN DC.Agreement_RID END AS Agreement_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Space_Short_RID WHEN '2' THEN DR.Space_Short_RID WHEN '3' THEN DC.Space_Short_RID END AS Space_Short_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Date WHEN '2' THEN DR.ReIncome_Date WHEN '3' THEN DC.Cancel_Date END AS Income_Date, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Stock_RID WHEN '2' THEN DR.Stock_RID WHEN '3' THEN DC.Stock_RID END AS Stock_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.OrderForm_Detail_RID WHEN '2' THEN DR.OrderForm_Detail_RID WHEN '3' THEN DC.OrderForm_Detail_RID END AS OrderForm_Detail_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Number WHEN '2' THEN DR.Reincome_Number WHEN '3' THEN DC.Cancel_Number END AS RIC_Number, " +
                                    "CTSD.Unit_Price_No,CTSD.Unit_Price,CTSD.Real_Ask_Number,CTSD.Delay_Days,CTSD.Comment,CTSD.Operate_Type,CTSD.Operate_RID,CTS.SAP_Serial_Number,CTSD.Check_Serial_Number," +
                                    "CTS.Ask_Date,CTS.Pay_Date,CTS.Pass_Status,CASE CTSD.SAP_RID WHEN '0' THEN 'N' Else CTS.Is_Finance END as Is_Finance " +
                                "FROM CARD_TYPE_SAP_DETAIL CTSD LEFT JOIN CARD_TYPE_SAP CTS ON CTS.RST = 'A' AND CTSD.SAP_RID = CTS.RID " +
                                    "LEFT JOIN DEPOSITORY_STOCK DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A'  AND CTSD.Operate_RID = DS.RID " +
                                    "LEFT JOIN DEPOSITORY_RESTOCK DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID " +
                                    "LEFT JOIN DEPOSITORY_CANCEL DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID " +
                                "WHERE CTSD.RST = 'A'  ) B INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND B.Space_Short_RID = CT.RID " +
                                    "INNER JOIN Factory F ON B.Blank_Factory_RID = F.RID " +
                                    "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND B.Budget_RID = CB.RID " +
                                    "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND B.Agreement_RID = AGM.RID " +
                                    "INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND B.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID) A " +
                    "WHERE A.Operate_RID<>0 ";

    //edit by Ian Huang start
    public const string IN_CARD_MONTH_FORCAST_PRINT = "INSERT INTO RPT_Finance0013  (Fore_Delivery_Date,Ask_Date,SAP_Serial_Number,Name,Factory_ShortName_CN,Budget_ID,Agreement_Code,Stock_RID,Operate_Type,Income_Number,Income_Date,Unit_Price,Unit_Price_No,Pay_Date,Delay_Days,Check_Serial_Number,TimeMark,Comment,Unit_PriceSum,Unit_Price1Sum) VALUES (@fore_delivery_date,@ask_date,@sAP_serial_number,@name,@factory_shortname_cn,@budget_id,@agreement_code,@stock_rid,@operate_type,@income_number,@income_date,@unit_price,@unit_price_No,@pay_date,@delay_days,@check_serial_number,@TimeMark,@Comment,@Unit_PriceSum,@Unit_Price1Sum) ";
    //edit by Ian Huang end

    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Finance0013BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
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
    /// 獲取空白卡廠
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
    public DataSet SearchSAP(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? " Stock_RID,Ask_Date " : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_FINANCE);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtStock_RIDYear"].ToString()))
            {
                string strStock_RID = searchInput["txtStock_RIDYear"].ToString();

                if (!StringUtil.IsEmpty(searchInput["txtStock_RID1"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID1"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtStock_RID2"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID2"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtStock_RID3"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID3"].ToString();
                }

                stbWhere.Append(" and A.Stock_RID Like @Stock_RID");
                dirValues.Add("Stock_RID", strStock_RID + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["dropCard_Group"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.CARDTYPE_RID IN (SELECT distinct CardType_RID from GROUP_CARD_TYPE Where RST = 'A' AND Group_RID  = @cgrid )");
                dirValues.Add("cgrid", searchInput["dropCard_Group"].ToString().Trim());
            }

            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Name LIKE @name ");
                dirValues.Add("name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["dropBlankFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.FRID = @blankfactory");
                dirValues.Add("blankfactory", searchInput["dropBlankFactory"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["dropPass_Status"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Pass_Status = @pass_status");
                dirValues.Add("pass_status", searchInput["dropPass_Status"].ToString().Trim());
            }

            if (!StringUtil.IsEmpty(searchInput["dropState"].ToString().Trim()))
            {
                if (searchInput["dropState"].ToString() == "2")//已請款狀態
                {
                    stbWhere.Append(" and (sap_id <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') ");
                }
                if (searchInput["dropState"].ToString() == "1")//未請款狀態
                {
                    lastRowNumber = "4000";
                    stbWhere.Append(" and (sap_id='' or sap_id is null) and Convert(datetime,substring(stock_rid,1,8))>='2008-09-01' ");
                }
            }

            if (!StringUtil.IsEmpty(searchInput["dropIs_Finance"].ToString().Trim()))
            {
                lastRowNumber = "4000";
                stbWhere.Append(" AND A.Is_Finance = @is_finance");
                dirValues.Add("is_finance", searchInput["dropIs_Finance"].ToString().Trim());
            }

            if (!StringUtil.IsEmpty(searchInput["txtBUDGET_ID"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Budget_ID Like @buget_id ");
                dirValues.Add("buget_id", "%" + searchInput["txtBUDGET_ID"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtAgreement_Code"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Agreement_Code Like @agreement_code ");
                dirValues.Add("agreement_code", "%" + searchInput["txtAgreement_Code"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                stbWhere.Append(" AND A.Income_Date >= @income_date_begin ");
                dirValues.Add("income_date_begin", DateTime.Parse(searchInput["txtBegin_Date"].ToString()).ToString("yyyy-MM-dd 00:00:00"));
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                stbWhere.Append(" AND A.Income_Date <= @income_date_end ");
                dirValues.Add("income_date_end", DateTime.Parse(searchInput["txtFinish_Date"].ToString()).ToString("yyyy-MM-dd 23:59:59"));
            }
        }

        DataSet dtSAP = null;
        try
        {
            dtSAP = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        rowCount = intRowCount;
        // 請款狀態--未請款
        if (searchInput["dropState"].ToString() == "1" || searchInput["dropIs_Finance"].ToString().Trim() == "N")//searchInput["dropState"].ToString() == "1"|| searchInput["dropIs_Finance"].ToString().Trim()=="N"
        {
            for (int i = 0; i < dtSAP.Tables[0].Rows.Count; i++)
            {
                DataRow tempRows = dtSAP.Tables[0].Rows[i];
                if (tempRows["Budget_ID"].ToString().Trim() == "舊系統預算" && tempRows["Agreement_Code"].ToString().Trim() == "舊系統合約")
                {
                    dtSAP.Tables[0].Rows.Remove(tempRows);
                    i--;
                }

            }
        }
        return dtSAP;
    }

    //汇出表格时新增資料到資料庫
    public void ADD_CARD_YEAR_FORCAST_PRINT(DataTable dtSplitWorkNew, string time)
    {
        dao.ExecuteNonQuery("delete RPT_Finance0013 where TimeMark<" + DateTime.Now.ToString("yyyyMMdd000000"));
        try
        {
            foreach (DataRow dr in dtSplitWorkNew.Rows)
            {
                dirValues.Clear();
                if (dr["Fore_Delivery_Date"].ToString() != "")
                {
                    dirValues.Add("fore_delivery_date", Convert.ToDateTime(dr["Fore_Delivery_Date"]));
                }
                else
                {
                    dirValues.Add("fore_delivery_date", Convert.ToDateTime("1900/01/01"));
                }

                if (dr["Ask_Date"].ToString() != "")
                {
                    dirValues.Add("ask_date", Convert.ToDateTime(dr["Ask_Date"]));
                }
                else
                {
                    dirValues.Add("ask_date", Convert.ToDateTime("1900/01/01"));
                }
                if (dr["Pay_Date"].ToString() != "")
                {
                    dirValues.Add("pay_date", Convert.ToDateTime(dr["Pay_Date"]));
                }
                else
                {
                    dirValues.Add("pay_date", Convert.ToDateTime("1900/01/01"));
                }
                if (dr["Income_Date"].ToString() != "")
                {
                    dirValues.Add("income_date", Convert.ToDateTime(dr["Income_Date"]));
                }
                else
                {
                    dirValues.Add("income_date", Convert.ToDateTime("1900/01/01"));
                }

                dirValues.Add("sAP_serial_number", dr["SAP_ID"].ToString());

                dirValues.Add("name", dr["Name"].ToString());
                dirValues.Add("factory_shortname_cn", dr["Factory_ShortName_CN"].ToString());
                dirValues.Add("budget_id", dr["Budget_ID"].ToString());
                dirValues.Add("agreement_code", dr["Agreement_Code"].ToString());
                //edit by Ian Huang start
                //dirValues.Add("stock_rid", dr["Stock_RID"].ToString());
                dirValues.Add("stock_rid", dr["Stock_RID_STR"].ToString());
                //edit by Ian Huang end
                dirValues.Add("operate_type", dr["Operate_Type"].ToString());
                dirValues.Add("income_number", dr["Income_Number"].ToString());
                dirValues.Add("unit_price", dr["Unit_Price"].ToString());
                dirValues.Add("unit_price_No", dr["Unit_Price1"].ToString());
                string days = null;
                if (dr["Fore_Delivery_Date"].ToString() != "" && dr["Income_Date"].ToString() != "" && dr["Fore_Delivery_Date"].ToString() != "&nbsp;" && dr["Income_Date"].ToString() != "&nbsp;")
                {
                    days = (DateTime.Parse(dr["Income_Date"].ToString()) - DateTime.Parse(dr["Fore_Delivery_Date"].ToString())).Days.ToString();
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                    //if (days.Substring(0, 1) != "-")
                    //{
                    //    dirValues.Add("delay_days", days);
                    //}
                    //else dirValues.Add("delay_days", dr["Delay_Days"].ToString());
                    dirValues.Add("delay_days", days);
                    //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                }
                else dirValues.Add("delay_days", dr["Delay_Days"].ToString());

                dirValues.Add("check_serial_number", dr["Check_ID"].ToString());
                dirValues.Add("TimeMark", time);
                //add by chaoma start 
                dirValues.Add("Comment", dr["Comment"].ToString());
                dirValues.Add("Unit_PriceSum", dr["Unit_PriceSum"].ToString());
                dirValues.Add("Unit_Price1Sum", dr["Unit_Price1Sum"].ToString());
                //add by chaoma end 
                dao.ExecuteNonQuery(IN_CARD_MONTH_FORCAST_PRINT, dirValues);
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
    }
}
