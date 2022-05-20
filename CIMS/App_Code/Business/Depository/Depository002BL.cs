//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-09-05
//*  修改日期：2008-09-05 10:00
//*  修改記錄：
//*            □2008-09-05
//*              1.創建 潘秉奕
//*******************************************************************
using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using ControlLibrary;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

/// <summary>
/// Depository002BL 的摘要描述
/// </summary>
public class Depository002BL : BaseLogic
{
    #region SQL語句
    public const string SEL_ORDER_FORM = "SELECT ODF.RID,ODF.OrderForm_RID,convert(varchar(20),ODF.Order_Date,111) as Order_Date,convert(varchar(20),ODF.Pass_Date,111) as Pass_Date,ODF.Pass_Status,P.Param_Name"
        + " FROM ORDER_FORM AS ODF"
        + " LEFT JOIN PARAM AS P"
        + " ON P.RST = 'A' AND P.paramtype_code = 'PassStatus' AND P.Param_Code = ODF.Pass_Status"
        + " WHERE ODF.RST = 'A'";

    public const string SEL_PASS_STATUS = "select param_name,param_code"
        + " from param"
        + " where paramtype_code = 'PassStatus' and rst='A'";

    // max 調整，增加空款預算的數量

    public const string SEL_CARD_BUDGET = "SELECT CB.RID, CB.budget_name,CB.budget_id"
        + " FROM CARD_BUDGET AS CB"
        + " INNER JOIN BUDGET_CARDTYPE AS BCT ON BCT.RST = 'A' AND BCT.budget_rid = CB.RID"
        + " AND BCT.cardtype_rid = @cardtype_rid INNER JOIN"
        + " (SELECT MIN(valid_date_from) AS Valid_Date_From, MAX(valid_date_to) AS Valid_Date_To," + " budget_main_rid AS RID FROM CARD_BUDGET AS CB2 WHERE CB2.RST = 'A'"
        + " GROUP BY CB2.budget_main_rid) AS XX ON XX.RID = CB.RID"
        + " AND XX.Valid_Date_From <= @Order_Date AND @Order_Date <= XX.Valid_Date_To"
    //    + " WHERE (CB.RST = 'A') and (CB.total_card_num=0 or CB.remain_total_num >= @number)";
        + " WHERE (CB.RST = 'A') and (CB.total_card_num=0 or CB.remain_total_num >= @number ) "
        + " and  (CB.Total_Card_AMT=0 or CB.Remain_Total_AMT >= (@number * "
//2010/11/19 by ken for one card with more than one agreement and price
        + " ( select  max ((case  when ACG_TYPE='1' then Base_Price when ACG_TYPE='2' then Price end ) ) as Price_int  from  ( "
        + " SELECT distinct  ACG.[type] as ACG_TYPE , ISNULL(ACG.Base_Price,0) as Base_Price,GC.RID,ISNULL(GLP.Price,0) as Price"
        + " FROM AGREEMENT AS A"
        + " INNER JOIN AGREEMENT_CARDTYPE_GROUP AS ACG"
        + " ON ACG.RST = 'A' AND ACG.Agreement_Main_RID = A.RID"
        + " INNER JOIN FACTORY AS F"
        + " ON A.FACTORY_RID=F.RID"
        + " INNER JOIN GROUP_CARDTYPE AS GC"
        + " ON GC.RST = 'A' AND GC.cardtype_rid = @cardtype_rid AND GC.Agreement_Group_RID = ACG.RID"
        + " LEFT JOIN GROUP_LEVEL_PRICE AS GLP"
        + " ON GLP.RST = 'A' AND GLP.Group_CardType_RID = GC.RID AND GLP.Level_Min<= @number AND @number<= GLP.Level_Max"
        + " WHERE A.RST = 'A' and A.begin_time <= @Order_Date AND @Order_Date <= A.end_time AND (A.Card_Number = 0 OR A.Remain_Card_Num >= @number)"
        + "   ) AS ABC )  )  )";

    public const string SEL_CARD_BUDGET2 = "SELECT CB.RID, CB.budget_name,CB.budget_id"
        + " FROM CARD_BUDGET AS CB"
        + " INNER JOIN BUDGET_CARDTYPE AS BCT ON BCT.RST = 'A' AND BCT.budget_rid = CB.RID"
        + " AND BCT.cardtype_rid = @cardtype_rid INNER JOIN"
        + " (SELECT MIN(valid_date_from) AS Valid_Date_From, MAX(valid_date_to) AS Valid_Date_To," + " budget_main_rid AS RID FROM CARD_BUDGET AS CB2 WHERE CB2.RST = 'A'"
        + " GROUP BY CB2.budget_main_rid) AS XX ON XX.RID = CB.RID"
        + " AND XX.Valid_Date_From <= @Order_Date AND @Order_Date <= XX.Valid_Date_To"
        + " WHERE (CB.RST = 'A') and (CB.total_card_num=0 or CB.remain_total_num >= 0)";

    public const string SEL_AGREEMENT = "SELECT distinct A.RID,A.agreement_code,A.agreement_code_main,F.rid AS factory_rid,F.factory_shortname_cn,ACG.RID AS agreement_group_rid,A.agreement_name, ACG.[type], ISNULL(ACG.Base_Price,0) as Base_Price,GC.RID,ISNULL(GLP.Price,0) as Price"
        + " FROM AGREEMENT AS A"
        + " INNER JOIN AGREEMENT_CARDTYPE_GROUP AS ACG"
        + " ON ACG.RST = 'A' AND ACG.Agreement_Main_RID = A.RID"
        + " INNER JOIN FACTORY AS F"
        + " ON A.FACTORY_RID=F.RID"
        + " INNER JOIN GROUP_CARDTYPE AS GC"
        + " ON GC.RST = 'A' AND GC.cardtype_rid = @cardtype_rid AND GC.Agreement_Group_RID = ACG.RID"
        + " LEFT JOIN GROUP_LEVEL_PRICE AS GLP"
        + " ON GLP.RST = 'A' AND GLP.Group_CardType_RID = GC.RID AND GLP.Level_Min<= @number AND @number<= GLP.Level_Max"
        //+ " INNER JOIN AGREEMENT AS A1 ON A1.RST = 'A'"
        //+ " AND (A1.Agreement_Code = A.Agreement_Code_Main OR (A1.Agreement_Code_Main = '' AND A1.Agreement_Code = A.Agreement_Code))"	
        //+ " AND (A1.Card_Number = 0 OR A1.Remain_Card_Num >= @number)"	
        //+ " AND A1.begin_time <= @Order_Date AND @Order_Date <= A1.end_time"
        + " WHERE A.RST = 'A' and A.begin_time <= @Order_Date AND @Order_Date <= A.end_time AND (A.Card_Number = 0 OR A.Remain_Card_Num >= @number)";

    public const string SEL_AGREEMENT2 = "SELECT distinct A.RID,A.agreement_code,A.agreement_code_main,F.rid AS factory_rid,F.factory_shortname_cn,ACG.RID AS agreement_group_rid,A.agreement_name, ACG.[type], ISNULL(ACG.Base_Price,0) as Base_Price,GC.RID,ISNULL(GLP.Price,0) as Price"
        + " FROM AGREEMENT AS A"
        + " INNER JOIN AGREEMENT_CARDTYPE_GROUP AS ACG"
        + " ON ACG.RST = 'A' AND ACG.Agreement_Main_RID = A.RID"
        + " INNER JOIN FACTORY AS F"
        + " ON A.FACTORY_RID=F.RID"
        + " INNER JOIN GROUP_CARDTYPE AS GC"
        + " ON GC.RST = 'A' AND GC.cardtype_rid = @cardtype_rid AND GC.Agreement_Group_RID = ACG.RID"
        + " LEFT JOIN GROUP_LEVEL_PRICE AS GLP"
        + " ON GLP.RST = 'A' AND GLP.Group_CardType_RID = GC.RID AND GLP.Level_Min<= @number AND @number<= GLP.Level_Max"
        + " INNER JOIN AGREEMENT AS A1 ON A1.RST = 'A'"
        //+ " AND (A1.Agreement_Code = A.Agreement_Code_Main OR (A1.Agreement_Code_Main = '' AND A1.Agreement_Code = A.Agreement_Code))"
        //+ " AND (A1.Card_Number = 0 OR A1.Remain_Card_Num >= 0)"
        //+ " AND A1.begin_time <= @Order_Date AND @Order_Date <= A1.end_time"
        + " WHERE A.RST = 'A' AND (A.Card_Number = 0 OR A.Remain_Card_Num >= 0) AND A.begin_time <= @Order_Date AND @Order_Date <= A.end_time";

    public const string SEL_WAFER_CARDTYPE = "SELECT WI.RID AS Value, WI.wafer_name AS Text, WI.wafer_capacity"
        + " FROM WAFER_CARDTYPE AS WC"
        + " INNER JOIN WAFER_INFO AS WI"
        + " ON WI.RST = 'A' AND WI.RID = WC.wafer_rid AND WI.is_using = 'Y'"
        + " inner JOIN WAFER_FACTORY AS wf"
        + " ON wf.RST = 'A' AND wf.wafer_rid=wc.wafer_rid and wf.factory_rid=@factory_rid"
        + " WHERE WC.RST = 'A' AND WC.cardtype_rid = @cardtype_rid";

    public const string SEL_PERSO_CARDTYPE = "SELECT distinct FP.RID AS [Value], FP.factory_shortname_cn AS [Text]"
        + " FROM PERSO_CARDTYPE AS PC"
        + " INNER JOIN FACTORY AS FP"
        + " ON FP.RST = 'A' AND FP.is_Perso ='Y' AND FP.RID = PC.factory_rid"
        + " WHERE PC.RST = 'A' AND PC.cardtype_rid = @cardtype_rid";

    public const string SEL_ORDER_FORM_RID = "select *"
        + " from order_form a"
        + " where a.rst='A' and a.orderform_rid like @orderform_rid order by orderform_rid desc";

    public const string SEL_ORDER_FORM_DETAIL_RID = "select *"       
        + " from order_form_detail b"
        + " where b.rst='A' and b.orderform_detail_rid like @orderform_detail_rid order by orderform_detail_rid desc";
    //mod chaoma start
    //public const string SEL_ORDER_FORM_DETAIL = "SELECT OFD.orderform_detail_rid,OFD.orderform_rid,convert(varchar(20),[of].order_date,111) as order_date,[of].pass_status,[of].Blank_Factory_RID, OFD.CardType_RID,Display_Name as Space_Short_RID, OFD.Number, OFD.Budget_RID,CB.budget_name,CB.budget_id,OFD.Agreement_RID, AM.agreement_name, convert(varchar(20),OFD.Fore_Delivery_Date,111) as Fore_Delivery_Date, OFD.Wafer_RID, WI.wafer_name,WI.wafer_capacity,OFD.Is_Exigence,P.param_name, OFD.Delivery_Address_RID, F.factory_shortname_cn,F.factory_id,OFD.comment,OFD.Unit_Price"
        public const string SEL_ORDER_FORM_DETAIL = "SELECT OFD.orderform_detail_rid,OFD.orderform_rid,convert(varchar(20),[of].order_date,111) as order_date,[of].pass_status,[of].Blank_Factory_RID, OFD.CardType_RID,Display_Name as Space_Short_RID, OFD.Number, OFD.Budget_RID,CB.budget_name,CB.budget_id,OFD.Agreement_RID, AM.agreement_name, convert(varchar(20),OFD.Fore_Delivery_Date,111) as Fore_Delivery_Date, OFD.Wafer_RID, WI.wafer_name,WI.wafer_capacity,OFD.Is_Exigence,P.param_name, OFD.Delivery_Address_RID, F.factory_shortname_cn,F.factory_id,OFD.comment,OFD.Unit_Price,OFD.Change_UnitPrice"
        + " FROM ORDER_FORM_DETAIL AS OFD"
        + " INNER JOIN ORDER_FORM AS [OF]"
        + " ON [OF].RST = 'A' AND [OF].OrderForm_RID = OFD.OrderForm_RID"
        + " LEFT OUTER JOIN CARD_TYPE AS CT"
        + " ON CT.RST = 'A' AND CT.RID = OFD.CardType_RID"
        + " LEFT OUTER JOIN CARD_BUDGET AS CB"
        + " ON CB.RST = 'A' AND CB.RID = OFD.Budget_RID"
        + " LEFT OUTER JOIN AGREEMENT AS AM"
        + " ON AM.RST = 'A' AND AM.RID = OFD.Agreement_RID"
        + " LEFT OUTER JOIN WAFER_INFO AS WI"
        + " ON WI.RST = 'A' AND WI.RID = OFD.Wafer_RID"
        + " LEFT OUTER JOIN PARAM AS P"
        + " ON P.RST = 'A' AND p.paramtype_code = 'CaseStatus' AND param_code = [of].case_status"
        + " LEFT OUTER JOIN FACTORY AS F"
        + " ON F.RST = 'A' AND F.RID = OFD.Delivery_Address_RID"
        + " WHERE (OFD.RST = 'A') AND ([OF].orderform_rid = @orderform_rid)";
    //mod chaoma end 
    public const string SEL_WAFER_INFO_BYCRID = " select WI.RID AS Value, WI.wafer_name AS Text"
        + " from WAFER_INFO WI"
        + " INNER JOIN WAFER_CARDTYPE WC"
        + " ON WI.RID=WC.WAFER_RID AND WC.RST='A'"
        + " WHERE WI.RST='A' AND WC.CARDTYPE_RID=@CARDTYPE_RID";

    public const string SEL_MATERIAL = "SELECT CGM.Base_Price,agreement_group_rid,MS.material_name"
        + " FROM CARDTYPE_MATERIAL CM"
        + " LEFT JOIN CARDTYPE_GROUP_MATERIAL AS CGM"
        + " ON CM.Material_rid = CGM.Material_rid and CGM.rst='A' and CGM.agreement_group_rid=@agrid"
        + " LEFT JOIN MATERIAL_SPECIAL MS"
        + " on CM.Material_rid=MS.rid and MS.rst='A'"
        + " WHERE CM.RST = 'A' AND CM.cardtype_rid=@cardtype_rid";

    public const string SEL_APPBUDGET_BY_RID = "SELECT *"
        + " FROM CARD_BUDGET"
        + " WHERE rst='A' and RID = @budget_rid";

    public const string SEL_BUDGET_BY_RID = "SELECT *"
        + " FROM CARD_BUDGET"
        + " WHERE rst='A' and  Budget_Main_RID = @budget_rid";

    public const string SEL_ORDER_FORM_DETAIL2 = "SELECT OFD.orderform_detail_rid,OFD.orderform_rid,convert(varchar(20),[of].order_date,111) as order_date,[of].case_status,[of].Blank_Factory_RID,F1.factory_shortname_cn as fsc,OFD.Unit_Price, OFD.CardType_RID,Display_Name as Space_Short_RID, OFD.Number, OFD.Budget_RID,CB.budget_name,CB.budget_id,OFD.Agreement_RID, AM.agreement_name,convert(varchar(20),OFD.Fore_Delivery_Date,111) as Fore_Delivery_Date, OFD.Wafer_RID, WI.wafer_name,WI.wafer_capacity,OFD.Is_Exigence,P.param_name as pm, OFD.Delivery_Address_RID, F2.factory_shortname_cn,F2.rid,P2.param_name,OFD.comment"
        + " FROM ORDER_FORM_DETAIL AS OFD"
        + " INNER JOIN ORDER_FORM AS [OF]"
        + " ON [OF].RST = 'A' AND [OF].OrderForm_RID = OFD.OrderForm_RID"
        + " AND [OF].pass_status = '4'"
        + " LEFT OUTER JOIN CARD_TYPE AS CT"
        + " ON CT.RST = 'A' AND CT.RID = OFD.CardType_RID"
        + " LEFT OUTER JOIN CARD_BUDGET AS CB"
        + " ON CB.RST = 'A' AND CB.RID = OFD.Budget_RID"
        + " LEFT OUTER JOIN AGREEMENT AS AM"
        + " ON AM.RST = 'A' AND AM.RID = OFD.Agreement_RID"
        + " LEFT OUTER JOIN FACTORY AS F1"
        + " ON F1.RST = 'A' AND F1.is_blank ='Y' AND F1.RID = [OF].Blank_Factory_RID"
        + " LEFT OUTER JOIN PARAM AS P"
        + " ON P.RST = 'A' AND p.paramtype_code = 'emergencyLevel' AND param_code = OFD.Is_Exigence"
        + " LEFT OUTER JOIN FACTORY AS F2"
        + " ON F2.RST = 'A' AND F2.is_perso ='Y' AND F2.RID = OFD.Delivery_Address_RID"
        + " LEFT OUTER JOIN PARAM AS P2"
        + " ON P2.RST = 'A' AND P2.paramtype_code = 'CaseStatus' AND P2.param_code = OFD.case_status"
        + " LEFT OUTER JOIN WAFER_INFO AS WI"
        + " ON WI.RST = 'A' AND WI.RID = OFD.Wafer_RID"
        + " WHERE OFD.RST = 'A'";

    public const string SEL_CARD_BUDGET_ALL = "SELECT CB.RID, CB.Budget_name,CB.budget_id FROM CARD_BUDGET AS CB WHERE CB.RST = 'A'";

    public const string SEL_AGREEMENT_ALL = "SELECT * FROM AGREEMENT AS A WHERE A.RST = 'A'";

    public const string SEL_FACTORY_BLANK = "SELECT RID, factory_shortname_cn from factory WHERE RST = 'A' and is_blank='Y'";

    public const string SEL_FACTORY_PERSO = "SELECT RID, factory_shortname_cn from factory WHERE RST = 'A' and is_perso='Y'";

    public const string SEL_CARDBUDGET_RID = "select rid from card_budget where rst='A' and budget_main_rid"         + " in (select budget_rid from ORDER_BUDGET_LOG where rst='A'"
        + " and OrderForm_Detail_RID=@OrderForm_Detail_RID) and rid in"
        + " (select distinct budget_main_rid from card_budget where rst='A')";

    public const string SEL_Space_Short_RID = "select Display_Name as Space_Short_RID from CARD_TYPE where rst='A'";

    public const string SEL_ORDER_FORM_DETAIL_2 = "SELECT ISNULL(AM.Remain_Card_Num,0) AS AMRemain_Card_Num,isnull(AM.Card_Number,0) as AMCard_Number ,isnull(cb.Total_Card_Num,0) as Total_Card_Num,isnull(cb.Remain_Total_AMT,0) as Remain_Total_AMT,isnull(cb.Remain_Total_Num,0) as Remain_Total_Num,OFD.Is_Edit_Budget,OFD.Unit_Price,OFORM.Pass_Status,OFD.ORDERFORM_DETAIL_RID,OFD.Case_Status,OFD.OrderForm_RID,OFD.Budget_RID,OFORM.Order_Date,OFD.Agreement_RID,OFD.Number FROM ORDER_FORM_DETAIL OFD LEFT JOIN  agreement  AM ON AM.RID=OFD.Agreement_RID inner JOIN ORDER_FORM OFORM ON OFORM.RST='A' AND OFORM.OrderForm_RID=OFD.OrderForm_RID left join CARD_BUDGET CB ON OFD.budget_rid=cb.rid and CB.RST='A' where OFD.OrderForm_Detail_RID=@OrderForm_Detail_RID ";

    public const string SEL_DEPOSITORY_STOCK_2 = " SELECT ISNULL(SUM(DS.Income_Number),0) FROM DEPOSITORY_STOCK AS DS WHERE DS.RST = 'A' AND DS.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_CANCEL_2 = " SELECT ISNULL(SUM(DC.cancel_Number),0) FROM DEPOSITORY_CANCEL AS DC WHERE DC.RST = 'A' AND DC.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_RESTOCK_2 = " SELECT ISNULL(SUM(DR.Reincome_Number),0) FROM DEPOSITORY_RESTOCK AS DR WHERE DR.RST = 'A' AND DR.OrderForm_Detail_RID =@OrderForm_Detail_RID";

    public const string SEL_ORDER_BUDGET_LOG_2 = "select * from ORDER_BUDGET_LOG WHERE RST='A' AND OrderForm_Detail_RID=@OrderForm_Detail_RID ";

    public const string SEL_BUDGET = "select Card_Price,Card_Num,RID,Total_Card_AMT,Total_Card_Num,Remain_Total_AMT,Remain_Total_Num,Remain_Card_Price,Remain_Card_Num from dbo.CARD_BUDGET where RST='A' AND Budget_Main_RID= @Budget_Main_RID";

    public const string SEL_BUDGET_LOG = "select * from dbo.ORDER_BUDGET_LOG where OrderForm_detail_RID=@OrderForm_detail_RID order by Budget_RID desc";

    public const string SEL_WAFER_INFO_BY_RID = "select * from WAFER_INFO WHERE RID=@RID";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    Depository003BL dep = new Depository003BL();

    public Depository002BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public string GetWafer_Capacity(string strRID)
    {
        string str = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            DataTable dtbl = dao.GetList(SEL_WAFER_INFO_BY_RID, dirValues).Tables[0];
            if (dtbl.Rows.Count > 0)
            {
                str = dtbl.Rows[0]["Wafer_Capacity"].ToString();
            }
        }
        catch
        {

        }
        return str;
    }

    /// <summary>
    /// 查詢訂單記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "OrderForm_RID " : sortField);//默認的排序欄位

        sortType = (sortField == "null" ? "desc " : sortType);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_ORDER_FORM);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (searchInput["txtOrderForm_RID_B"].ToString().Trim() != "" && searchInput["txtOrderForm_RID_E"].ToString().Trim() != "")
            {
                stbWhere.Append(" and OrderForm_RID like @OrderForm_RID");
                dirValues.Add("OrderForm_RID", searchInput["txtOrderForm_RID_B"].ToString().Trim() +"%"+ searchInput["txtOrderForm_RID_E"].ToString().Trim()+"%");
            }
            else if (searchInput["txtOrderForm_RID_B"].ToString().Trim() != "" && searchInput["txtOrderForm_RID_E"].ToString().Trim() == "")
            {
                stbWhere.Append(" and OrderForm_RID like @OrderForm_RID_B");
                dirValues.Add("OrderForm_RID_B", searchInput["txtOrderForm_RID_B"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["txtOrder_Date_FROM"].ToString().Trim()))
            {
                stbWhere.Append(" and Order_Date>=@Order_Date_FROM");
                dirValues.Add("Order_Date_FROM", searchInput["txtOrder_Date_FROM"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtOrder_Date_TO"].ToString().Trim()))
            {
                stbWhere.Append(" and Order_Date<=@Order_Date_TO");
                dirValues.Add("Order_Date_TO", searchInput["txtOrder_Date_TO"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtPass_Date_FROM"].ToString().Trim()))
            {
                stbWhere.Append(" and Pass_Date>=@Pass_Date_FROM");
                dirValues.Add("Pass_Date_FROM", searchInput["txtPass_Date_FROM"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtPass_Date_TO"].ToString().Trim()))
            {
                stbWhere.Append(" and Pass_Date<=@Pass_Date_TO");
                dirValues.Add("Pass_Date_TO", searchInput["txtPass_Date_TO"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["drpPass_Status"].ToString().Trim()))
            {
                stbWhere.Append(" and Pass_Status=@Pass_Status");
                dirValues.Add("Pass_Status", searchInput["drpPass_Status"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dstOF = null;
        try
        {
            dstOF = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstOF;
    }

    /// <summary>
    /// 根據編號獲得卡種
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns>Space_Short_RID</returns>
    public string GetSpace_Short_RID(string strRID)
    {
        DataSet dst = null;

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        try
        {
            dirValues.Clear();
            stbWhere.Append(" and rid =@rid");
            dirValues.Add("rid", strRID);
            dst = dao.GetList(SEL_Space_Short_RID+stbWhere.ToString(),dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }

        if (dst != null && dst.Tables[0].Rows.Count > 0)
        {
            return dst.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 放行狀態下拉框綁定
    /// </summary>
    public void dropPassStatusBind(DropDownList dropPassStatus)
    {
        dropPassStatus.DataTextField = "PARAM_NAME";
        dropPassStatus.DataValueField = "Param_Code";
        dropPassStatus.DataSource = GetPassStatus();
        dropPassStatus.DataBind();
    }

    /// <summary>
    /// 獲取放行狀態
    /// </summary>
    /// <returns></returns>
    public DataSet GetPassStatus()
    {
        DataSet dstPassStatus = null;

        try
        {
            dstPassStatus = dao.GetList(SEL_PASS_STATUS);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstPassStatus;
    }

    /// <summary>
    /// 根據合約編號獲得對應記錄的主合約編號
    /// </summary>
    /// <param name="strRID">RID</param>
    /// <returns></returns>
    public string GetAgreementMainCode(string strRID)
    {
        DataSet ds = dao.GetList("select Agreement_Code from agreement where rst='A' and RID='" + strRID + "'");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }

        return strRID;
    }

    /// <summary>
    /// 根據主合約編號獲得對應記錄編號
    /// </summary>
    /// <param name="strCode">Agreement_Code</param>
    /// <returns></returns>
    public string GetAgreementRID(string strCode)
    {
        DataSet ds = dao.GetList("select RID from agreement where rst='A' and Agreement_Code='" + strCode + "'");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 根據合約編號獲得對應合約名稱
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public string GetAgreementName(string strRID)
    {
        DataSet ds = dao.GetList("select agreement_name from agreement where rst='A' and RID='" + strRID + "'");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 根據數量和卡種獲得對應合約、預算、晶片名稱、交貨地點
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="ismod">是否修改頁面</param>
    /// <returns></returns>
    public DataSet GetOrderFormDetailCombo(Dictionary<string, object> searchInput,bool ismod)
    {
        DataSet dsts = new DataSet();

        try
        {
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {                
                dirValues.Clear();
                
                dirValues.Add("cardtype_rid", searchInput["dropSpace_Short_RID"].ToString().Trim());
                
                dirValues.Add("Order_Date", Convert.ToDateTime(searchInput["Order_Date"].ToString()));

                // Legend 2018/01/17 當[數量]欄位有值時, 才轉換類型
                if (string.IsNullOrEmpty(searchInput["txtNumber"].ToString().Trim()))
                {
                    dirValues.Add("number", searchInput["txtNumber"].ToString().Trim());
                }
                else
                {
                    dirValues.Add("number", Convert.ToInt64(searchInput["txtNumber"].ToString().Trim()));
                }

                if (ismod == false)
                {
                    dsts = dao.GetList(SEL_PERSO_CARDTYPE + " " + SEL_AGREEMENT + " " + SEL_CARD_BUDGET, dirValues);
                }
                else
                {
                    dsts = dao.GetList(SEL_PERSO_CARDTYPE + " " + SEL_AGREEMENT2 + " " + SEL_CARD_BUDGET2, dirValues);
                }

                dsts.Tables[0].TableName ="PERSO_CARDTYPE";
                dsts.Tables[1].TableName = "AGREEMENT";
                dsts.Tables[2].TableName = "CARD_BUDGET";
            } 
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }

        return dsts;
    }

    /// <summary>
    /// 根據空白卡廠和卡種編號獲得晶片
    /// </summary>
    /// <param name="cardtype"></param>
    /// <param name="factoryrid"></param>
    /// <returns></returns>
    public DataSet DropWafer(string cardtype,string factoryrid)
    {
        DataSet ds = null;

        dirValues.Clear();

        dirValues.Add("cardtype_rid", cardtype);
        dirValues.Add("factory_rid",factoryrid);
        ds = dao.GetList(SEL_WAFER_CARDTYPE, dirValues);

        return ds;
    }

    /// <summary>
    /// 根據當日日期獲得已有的訂單rid
    /// </summary>
    /// <returns>返回新rid</returns>
    public string GetMaxOFID()
    {
        try
        {
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            string rid = System.DateTime.Now.ToString("yyyyMMdd");

            DataSet dstmp = null;

            dirValues.Clear();
            stbWhere.Append(" and orderform_rid like @orderform_rid");
            dirValues.Add("orderform_rid", rid + "%");

            dstmp = dao.GetList(SEL_ORDER_FORM_RID, dirValues);

            if (dstmp != null&&dstmp.Tables[0].Rows.Count>0)
            {
                int id = int.Parse(dstmp.Tables[0].Rows[0]["orderform_rid"].ToString().Substring(8, 2))+1;
                if (id.ToString().Length == 1)
                {
                    return rid + "0" + id.ToString();
                }
                else
                {
                    return rid + id.ToString();
                }
            }
            else
            {
                return rid+"01";
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
    }

    public string GetMaxOFID(string strRID)
    {
        try
        {
            ////整理查詢條件
            //StringBuilder stbWhere = new StringBuilder();

            //DataSet dstmp = null;

            //dirValues.Clear();
            //stbWhere.Append(" and orderform_rid like @orderform_rid");
            //dirValues.Add("orderform_rid", strRID.Substring(0, 8) + "%");

            //dstmp = dao.GetList("select * from order_form where orderform_rid like @orderform_rid order by orderform_rid desc", dirValues);

            //string str = dstmp.Tables[0].Rows[0]["orderform_rid"].ToString();

            int id = int.Parse(strRID.Substring(8, 2)) + 1;
            if (id.ToString().Length == 1)
            {
                return strRID.Substring(0,8) + "0" + id.ToString();
            }
            else if (id.ToString().Length == 2)
            {
                return strRID.Substring(0, 8) + id.ToString();
            }
            else
            {
                throw new Exception(GlobalStringManager.Default["Alert_MaxNum"]);
            }
        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
    }

    /// <summary>
    /// 根據當日日期獲得已有的訂單詳細rid
    /// </summary>
    /// <returns>返回新rid</returns>
    public string GetMaxOFDID()
    {
        try
        {
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            DataSet dstmp = null;

            string str = GetMaxOFID().Substring(8);

            dirValues.Clear();
            stbWhere.Append(" and orderform_detail_rid like @orderform_detail_rid");
            if (str.Length == 2)
            {
                dirValues.Add("orderform_detail_rid", GetMaxOFID() + "%");
            }
            else
            {
                throw new AlertException(GlobalStringManager.Default["Alert_MaxNum"]);
            }

            dstmp = dao.GetList(SEL_ORDER_FORM_DETAIL_RID, dirValues);

            if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
            {
                int id = int.Parse(dstmp.Tables[0].Rows[0]["orderform_detail_rid"].ToString().Substring(10, 2)) + 1;
                if (id.ToString().Length == 1)
                {
                    return GetMaxOFID() + "0" + id.ToString();
                }
                else
                {
                    return GetMaxOFID() + id.ToString();
                }
            }
            else
            {
                return GetMaxOFID()+"01";
            }
        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
    }

    /// <summary>
    /// 根據訂單編號獲得新的訂單明細編號
    /// </summary>
    /// <param name="strOFID"></param>
    /// <returns></returns>
    public string GetMaxOFDID(string strOFID)
    {
        try
        {
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();
            DataSet dstmp = null;

            dirValues.Clear();
            stbWhere.Append(" and orderform_detail_rid like @orderform_detail_rid");            
            dirValues.Add("orderform_detail_rid", strOFID + "%");

            dstmp = dao.GetList(SEL_ORDER_FORM_DETAIL_RID, dirValues);

            if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
            {
                int id = int.Parse(dstmp.Tables[0].Rows[0]["orderform_detail_rid"].ToString().Substring(10, 2)) + 1;
                if (id.ToString().Length == 1)
                {
                    return strOFID + "0" + id.ToString();
                }
                else
                {
                    return strOFID + id.ToString();
                }
            }
            else
            {
                return strOFID + "01";
            }     
        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
    }

    public string GetMaxOFDID(string strRID,string strOFDID)
    {
        try
        {
            if (strOFDID.Trim()!="")
            {
                int id = int.Parse(strOFDID.Substring(10, 2)) + 1;
                if (id.ToString().Length == 1)
                {
                    return strRID.Substring(0, 10) + "0" + id.ToString();
                }
                else if (id.ToString().Length == 2)
                {
                    return strRID.Substring(0, 10) + id.ToString();
                }
                else
                {
                    throw new Exception(GlobalStringManager.Default["Alert_MaxNum"]);
                }
            }
            else
            {
                return strRID + "01";
            }
        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
    }

    /// <summary>
    /// 根據訂單RID獲得訂單記錄
    /// </summary>
    /// <param name="strOrderForm_RID"></param>
    /// <returns></returns>
    public DataSet GetOrderFormDetail(string strOrderForm_RID)
    {
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        DataSet dstmp = null;

        dirValues.Clear();
        stbWhere.Append(" and orderform_rid = @orderform_rid");
        dirValues.Add("orderform_rid", strOrderForm_RID);

        dstmp = dao.GetList(SEL_ORDER_FORM_RID, dirValues);

        return dstmp;
    }

    /// <summary>
    /// 根據訂單詳細RID獲得訂單詳細記錄
    /// </summary>
    /// <param name="strOrderForm_RID"></param>
    /// <returns></returns>
    public DataSet GetOrderForm_Detail(string strOrderFormDetail_RID)
    {
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        DataSet dstmp = null;

        dirValues.Clear();
        stbWhere.Append(" and orderform_detail_rid = @orderform_detail_rid");
        dirValues.Add("orderform_detail_rid", strOrderFormDetail_RID);

        dstmp = dao.GetList(SEL_ORDER_FORM_DETAIL_RID, dirValues);

        return dstmp;
    }

    /// <summary>
    /// 根據訂單編號獲得對應的訂單詳細記錄（頁面初始化）
    /// </summary>
    /// <param name="strOrderFormRID"></param>
    /// <returns></returns>
    public DataSet GetOrderFormDetail_RID(string strOrderFormRID)
    {
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        DataSet dstmp = null;

        dirValues.Clear();
        stbWhere.Append(" and orderform_rid = @orderform_rid");
        dirValues.Add("orderform_rid", strOrderFormRID);

        dstmp = dao.GetList(SEL_ORDER_FORM_DETAIL, dirValues);

        return dstmp;
    }

    /// <summary>
    /// 根據卡種獲取晶體名稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetWAFER_INFOBYCRID(string strCARDTYPE_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("CARDTYPE_RID", strCARDTYPE_RID);
            dst = dao.GetList(SEL_WAFER_INFO_BYCRID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 根據合約卡種組合編號得到卡種對應所有材質的金額之和
    /// </summary>
    /// <param name="strCardType">卡種編號</param>
    /// <param name="strAGRID">合約卡種組合RID</param>
    /// <returns></returns>
    public DataSet GetMaterialPrice(string strCardType,string strAGRID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("cardtype_rid",strCardType);
            dirValues.Add("agrid",strAGRID);
            dst = dao.GetList(SEL_MATERIAL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dst;
    }

    /// <summary>
    /// 判斷該卡種是否有特殊材質
    /// </summary>
    /// <param name="strCardType"></param>
    /// <returns></returns>
    public bool IsCardtype_Material(string strCardType)
    {
        bool isres = false;

        DataSet dst = null;
        
        dirValues.Clear();
        dirValues.Add("cardtype_rid", strCardType);
        dst = dao.GetList("select * from cardtype_material where rst='A' and cardtype_rid=@cardtype_rid", dirValues);
        if (dst != null && dst.Tables[0].Rows.Count > 0)
        {
            isres = true;
        }
        else
        {
            isres = false;
        }

        return isres;
    }

    /// <summary>
    /// 根據訂單詳細編號判斷是否已經入庫
    /// </summary>
    /// <param name="strOFDRID"></param>
    /// <returns>true入庫/false未入庫</returns>
    public bool IsStock(string strOFDRID)
    {
        DataSet dstKzqz = null;
        try
        {
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();
            stbWhere.Append(" and OrderForm_Detail_RID like @OrderForm_Detail_RID");
            dirValues.Add("OrderForm_Detail_RID", strOFDRID+"%");

            dstKzqz = dao.GetList("select * from DEPOSITORY_STOCK where rst='A'" + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }

        if (dstKzqz != null && dstKzqz.Tables[0].Rows.Count > 0)
            return true;
        return false;
    }

    /// <summary>
    /// 根據預算編號和單價、數量比較剩餘金額
    /// </summary>
    /// <param name="strNum">數量</param>
    /// <param name="strPrice">單價</param>
    /// <param name="strBudgetRID">預算編號</param>
    /// <returns>超過剩餘金額返回false,否則返回true</returns>
    public bool PassAMT(string strNum,string strPrice,string strBudgetRID)
    {
        double num = Convert.ToDouble(strNum);
        double price = Convert.ToDouble(strPrice);
                
        dirValues.Clear();
        dirValues.Add("budget_rid", strBudgetRID);
        DataSet ds = dao.GetList(SEL_APPBUDGET_BY_RID, dirValues);

        //如果預算總卡數為0，需判斷預算剩餘縂卡數是否為0，如果預算剩餘縂卡數為0，則可繼續操作
        if (ds.Tables[0].Rows[0]["Total_Card_Num"].ToString() == "0")
        {
            return true;
        }
        else
        {
            //剩餘總金額-訂單單價*訂單數量
            long total = Convert.ToInt64(Convert.ToDouble(ds.Tables[0].Rows[0]["remain_total_amt"])- num * price);
            if (total >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    /// <summary>
    /// 根據預算編號比較剩餘總數量
    /// </summary>
    /// <param name="strNum">訂單數量</param>
    /// <param name="strBudgetRID">主預算編號</param>
    /// <returns></returns>
    public bool PassNum(string strNum, string strBudgetRID)
    {
        double num = Convert.ToDouble(strNum);

        dirValues.Clear();
        dirValues.Add("budget_rid", strBudgetRID);
        DataSet ds = dao.GetList(SEL_APPBUDGET_BY_RID, dirValues);

        int total = 0;
        int snum = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            total = Convert.ToInt32(Convert.ToDouble(ds.Tables[0].Rows[0]["remain_total_num"]) - num);
            snum = Convert.ToInt32(Convert.ToDouble(ds.Tables[0].Rows[0]["Total_Card_Num"]));
        }

        if (snum == 0)
        {
            return true;
        }
        else
        {
            if (total >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 判斷合約的卡片總數，為0則不管，否則需要調整合約的剩餘數量。同時判斷訂單數量必須小於等於剩餘數量
    /// </summary>
    /// <param name="strNum">訂單數量</param>
    /// <param name="strAgreementRID">合約編號</param>
    /// <returns></returns>
    public bool PassAgreement(string strNum,string strAgreementRID)
    {
        int num = Convert.ToInt32(strNum);
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        stbWhere.Append(" and RID=@RID");
        dirValues.Add("RID", strAgreementRID);

        int snum = 0;
        int total = 0;
        DataSet ds = dao.GetList(SEL_AGREEMENT_ALL+stbWhere.ToString(), dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            total = Convert.ToInt32(ds.Tables[0].Rows[0]["Remain_Card_Num"].ToString())-num;
            snum = Convert.ToInt32(ds.Tables[0].Rows[0]["Card_Number"].ToString());
        }
        else
        {
            return false;
        }

        if (snum == 0)
        {
            return true;
        }
        else
        {
            if (total >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //判斷所有記錄是否都保存入庫
    public bool IsSaveDB(DataTable dt)
    {
        bool isresult = false;

        DataSet dstmp = null;

        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dstmp = dao.GetList("select * from order_form_detail where rst='A' and orderform_detail_rid='" + dt.Rows[i]["orderform_detail_rid"].ToString() + "'");
                if (dstmp == null || dstmp.Tables[0].Rows.Count == 0)
                {
                    isresult = false;
                    break;
                }
                else
                {
                    isresult = true;
                }
            }
        }

        return isresult;
    }

    public bool IsSaveDB(DataRow dr)
    {
        bool isresult = false;

        DataSet dstmp = null;

        if (dr != null)
        {           
            dstmp = dao.GetList("select * from order_form_detail where rst='A' and orderform_detail_rid='" + dr["orderform_detail_rid"].ToString() + "'");
            if (dstmp == null || dstmp.Tables[0].Rows.Count == 0)
            {
                isresult = false;                
            }
            else
            {
                isresult = true;
            }            
        }

        return isresult;
    }

    public void UpdateDate(string strOrdDetail, string strDate)
    {
        try
        {
            ORDER_FORM_DETAIL ofdModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", strOrdDetail);

            if (ofdModel != null)
            {
                ofdModel.Fore_Delivery_Date = Convert.ToDateTime(strDate);
                dao.Update<ORDER_FORM_DETAIL>(ofdModel, "RID");
            }

            //操作日誌
            SetOprLog("3");
        }
        catch(Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 資料保存後進行了刪除操作，然後再保存（回補、刪除、添加、日誌）
    /// </summary>
    /// <param name="dtclone">刪除前的資料</param>
    /// <param name="dtdetail">刪除後要保存的資料</param>
    public void RDAU(DataTable dtclone, DataTable dtdetail)
    {
        try
        {
            dao.OpenConnection();

          

            foreach (DataRow drow in dtclone.Rows)
            {
                UpdateBudget(drow["orderform_detail_rid"].ToString(), int.Parse(drow["number"].ToString()));
            }
            
            Delete(dtclone, 1);
            Adds(dtdetail);
            
            foreach (DataRow drow in dtdetail.Rows)
            {
                Add_CheckBudget(drow["orderform_detail_rid"].ToString(), int.Parse(drow["number"].ToString()));
            }

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException aex)
        {
            dao.Rollback();
            throw aex;
        }
        catch (Exception ex)
        {
            //回滾
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
    /// 鮑方增加，更新的時候增加方法
    /// </summary>
    /// <param name="drow"></param>
    public void AddUpdate(DataRow dr,DateTime dtPassDate)
    {
        try
        {
            ORDER_FORM ofModel = new ORDER_FORM();
            ORDER_FORM_DETAIL ofdModel = new ORDER_FORM_DETAIL();

            if (PassNum(dr["number"].ToString(), dr["budget_rid"].ToString()))
            {
                if (PassAMT(dr["number"].ToString(), dr["base_price"].ToString(), dr["budget_rid"].ToString()))
                {
                    ofdModel.OrderForm_Detail_RID = dr["orderform_detail_rid"].ToString();
                    ofdModel.OrderForm_RID = dr["orderform_rid"].ToString();  
                    //ofdModel.OrderForm_RID = dr["orderform_rid"].ToString();
                    //ofdModel.OrderForm_Detail_RID = GetMaxOFDID(ofdModel.OrderForm_RID);
                    dr["orderform_detail_rid"] = ofdModel.OrderForm_Detail_RID;

                    ofdModel.Number = Convert.ToInt32(dr["number"].ToString());
                    ofdModel.Is_Exigence = dr["is_exigence"].ToString();
                    if (dr["Fore_Delivery_Date"].ToString() != "")
                    {
                        ofdModel.Fore_Delivery_Date = Convert.ToDateTime(dr["Fore_Delivery_Date"].ToString());
                    }
                    if (dr["Delivery_Address_RID"].ToString() != "")
                    {
                        ofdModel.Delivery_Address_RID = Convert.ToInt32(dr["Delivery_Address_RID"].ToString());
                    }
                    if (dr["Agreement_RID"].ToString() != "")
                    {
                        ofdModel.Agreement_RID = Convert.ToInt32(dr["Agreement_RID"].ToString());
                    }
                    if (dr["Budget_RID"].ToString() != "")
                    {
                        ofdModel.Budget_RID = Convert.ToInt32(dr["Budget_RID"].ToString());
                    }
                    if (dr["Space_Short_RID"].ToString() != "")
                    {
                        ofdModel.CardType_RID = Convert.ToInt32(dr["Space_Short_RID"].ToString());
                    }
                    ofdModel.Comment = dr["Comment"].ToString();
                    if (dr["Wafer_RID"].ToString() != "")
                    {
                        ofdModel.Wafer_RID = Convert.ToInt32(dr["Wafer_RID"].ToString());
                    }
                    //add chaoma start
                    //ofdModel.Unit_Price = Convert.ToDecimal(dr["Base_Price"].ToString());
                    ofdModel.Unit_Price = Convert.ToDecimal(dr["Change_UnitPrice"].ToString());
                    ofdModel.Change_UnitPrice = Convert.ToDecimal(dr["Base_Price"].ToString());
                    //add chaoma end
                    dao.Add<ORDER_FORM_DETAIL>(ofdModel, "rid");

                    DataSet dset = dao.GetList("select * from order_form where rst='A' and orderform_rid='" + dr["orderform_rid"].ToString() + "'");

                    if (dset == null || dset.Tables[0].Rows.Count == 0)
                    {
                        ofModel.Blank_Factory_RID = Convert.ToInt32(dr["factory"].ToString());
                        ofModel.Case_Status = "N";
                        ofModel.OrderForm_RID = dr["orderform_rid"].ToString();
                        ofModel.Pass_Status = dr["pass_status"].ToString();
                        ofModel.Pass_Date = dtPassDate;
                        ofModel.Order_Date = System.DateTime.Now;
                        dao.Add<ORDER_FORM>(ofModel, "rid");
                    }
                }
                else
                {
                    throw new AlertException("預算剩餘金額不足！");
                }
            }
            else
            {
                throw new AlertException("預算剩餘總數量不足！");
            }

        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void RDAU(ArrayList al,DataRow drow)
    {
        try
        {
            dao.OpenConnection();

            ORDER_FORM_DETAIL orfModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", al[2].ToString());
            ORDER_FORM ofModel = dao.GetModel<ORDER_FORM, string>("OrderForm_RID", al[3].ToString());

            al[0] = orfModel.Number;
            al[1] = orfModel.Agreement_RID;

            
            UpdateBudget(al[2].ToString(), int.Parse(al[0].ToString()));


            Delete(al, 1);
            AddUpdate(drow, ofModel.Pass_Date);

            Add_CheckBudget(drow["orderform_detail_rid"].ToString(), int.Parse(drow["number"].ToString()));

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException aex)
        {
            dao.Rollback();
            throw aex;
        }
        catch (Exception ex)
        {
            //回滾
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

    public void RDAU(DataRow dr)
    {
        try
        {
            dao.OpenConnection();

        


            UpdateBudget(dr["orderform_detail_rid"].ToString(), int.Parse(dr["number"].ToString()));

            Delete(dr, 1);
            Adds(dr);
            Add_CheckBudget(dr["orderform_detail_rid"].ToString(), int.Parse(dr["number"].ToString()));



            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException aex)
        {
            dao.Rollback();
            throw aex;
        }
        catch (Exception ex)
        {
            //回滾
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
    /// 勾選刪除後進行資料操作（回補、刪除）
    /// </summary>
    /// <param name="dtclone"></param>
    public void RD(DataTable dtclone)
    {
        try
        {
            dao.OpenConnection();

           

            foreach (DataRow dr in dtclone.Rows)
            {
                UpdateBudget(dr["orderform_detail_rid"].ToString(), int.Parse(dr["number"].ToString()));
            }
            Delete(dtclone, 1);

            //操作日誌
            SetOprLog("4");

            dao.Commit();
        }
        catch (AlertException aex)
        {
            dao.Rollback();
            throw aex;
        }
        catch (Exception ex)
        {
            //回滾
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

    public void AUU(DataRow dr)
    {
        try
        {
            dao.OpenConnection();

            Adds(dr);

            Add_CheckBudget(dr["orderform_detail_rid"].ToString(), int.Parse(dr["number"].ToString()));

           

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException aex)
        {
            dao.Rollback();
            throw aex;
        }
        catch (Exception ex)
        {
            //回滾
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

    public void RRD(DataRow dr)
    {
        try
        {
            dao.OpenConnection();

          

            UpdateBudget(dr["orderform_detail_rid"].ToString(), int.Parse(dr["number"].ToString()));
            Delete(dr, 1);

            dao.Commit();
        }
        catch (AlertException aex)
        {
            dao.Rollback();
            throw aex;
        }
        catch (Exception ex)
        {
            //回滾
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

    public void RD(DataRow dr)
    {
        try
        {
            dao.OpenConnection();

            UpdateBudget(dr["orderform_detail_rid"].ToString(), int.Parse(dr["number"].ToString()));

           
            Delete(dr, 1);

            //操作日誌
            SetOprLog("4");

            dao.Commit();
        }
        catch (AlertException aex)
        {
            dao.Rollback();
            throw aex;
        }
        catch (Exception ex)
        {
            //回滾
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
    /// 循環添加dt中的記錄入資料庫
    /// </summary>
    /// <param name="dt"></param>
    public void Adds(DataTable dt)
    {
        try
        {
            ORDER_FORM ofModel = new ORDER_FORM();
            ORDER_FORM_DETAIL ofdModel = new ORDER_FORM_DETAIL();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (PassNum(dt.Rows[i]["number"].ToString(), dt.Rows[i]["budget_rid"].ToString()))
                {
                    if (PassAMT(dt.Rows[i]["number"].ToString(), dt.Rows[i]["base_price"].ToString(), dt.Rows[i]["budget_rid"].ToString()))
                    {
                        //ofdModel.OrderForm_Detail_RID = dt.Rows[i]["orderform_detail_rid"].ToString();
                        //ofdModel.OrderForm_RID = dt.Rows[i]["orderform_rid"].ToString();  
                        ofdModel.OrderForm_RID = dt.Rows[i]["orderform_rid"].ToString();
                        ofdModel.OrderForm_Detail_RID = GetMaxOFDID(ofdModel.OrderForm_RID);
                        DataRow dr = dt.Rows[i];
                        dr["orderform_detail_rid"] = ofdModel.OrderForm_Detail_RID;
                        
                        ofdModel.Number = Convert.ToInt32(dt.Rows[i]["number"].ToString());
                        ofdModel.Is_Exigence = dt.Rows[i]["is_exigence"].ToString();
                        if (dt.Rows[i]["Fore_Delivery_Date"].ToString() != "")
                        {
                            ofdModel.Fore_Delivery_Date = Convert.ToDateTime(dt.Rows[i]["Fore_Delivery_Date"].ToString());
                        }
                        if (dt.Rows[i]["Delivery_Address_RID"].ToString() != "")
                        {
                            ofdModel.Delivery_Address_RID = Convert.ToInt32(dt.Rows[i]["Delivery_Address_RID"].ToString());
                        }
                        if (dt.Rows[i]["Agreement_RID"].ToString() != "")
                        {
                            ofdModel.Agreement_RID = Convert.ToInt32(dt.Rows[i]["Agreement_RID"].ToString());
                        }
                        if (dt.Rows[i]["Budget_RID"].ToString() != "")
                        {
                            ofdModel.Budget_RID = Convert.ToInt32(dt.Rows[i]["Budget_RID"].ToString());
                        }
                        if (dt.Rows[i]["Space_Short_RID"].ToString() != "")
                        {
                            ofdModel.CardType_RID = Convert.ToInt32(dt.Rows[i]["Space_Short_RID"].ToString());
                        }
                        ofdModel.Comment = dt.Rows[i]["Comment"].ToString();
                        if (dt.Rows[i]["Wafer_RID"].ToString() != "")
                        {
                            ofdModel.Wafer_RID = Convert.ToInt32(dt.Rows[i]["Wafer_RID"].ToString());
                        }
                        //add chaoma start
                        //ofdModel.Unit_Price = Convert.ToDecimal(dt.Rows[i]["Base_Price"].ToString());
                        ofdModel.Unit_Price = Convert.ToDecimal(dt.Rows[i]["Change_UnitPrice"].ToString());
                        ofdModel.Change_UnitPrice = Convert.ToDecimal(dt.Rows[i]["Base_Price"].ToString());
                        //add chaoma end
                        dao.Add<ORDER_FORM_DETAIL>(ofdModel, "rid");

                        if (i > 0)
                        {
                            DataSet dstmp = dao.GetList("select * from order_form where OrderForm_RID=" + dt.Rows[i]["orderform_rid"].ToString() + "");
                            if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                            {
                                //break;
                            }
                            else
                            {
                                ofModel.Blank_Factory_RID = Convert.ToInt32(dt.Rows[i]["factory"].ToString());
                                ofModel.Case_Status = "N";
                                ofModel.OrderForm_RID = dt.Rows[i]["orderform_rid"].ToString();
                                ofModel.Pass_Status = dt.Rows[i]["pass_status"].ToString();
                                //ofModel.Pass_Date = System.DateTime.Now;
                                ofModel.Order_Date = System.DateTime.Now;
                                dao.Add<ORDER_FORM>(ofModel, "rid");
                            }
                        }
                        else if (i == 0)
                        {
                            ofModel.Blank_Factory_RID = Convert.ToInt32(dt.Rows[0]["factory"].ToString());
                            ofModel.Case_Status = "N";
                            ofModel.OrderForm_RID = dt.Rows[0]["orderform_rid"].ToString();
                            ofModel.Pass_Status = dt.Rows[0]["pass_status"].ToString();
                            //ofModel.Pass_Date = System.DateTime.Now;
                            ofModel.Order_Date = System.DateTime.Now;
                            dao.Add<ORDER_FORM>(ofModel, "rid");
                        }
                    }
                    else
                    {
                        throw new AlertException("預算剩餘金額不足！");
                    }
                }
                else
                {
                    throw new AlertException("預算剩餘總數量不足！");
                }
            }
         
        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void Adds(DataRow dr)
    {
        try
        {
            ORDER_FORM ofModel = new ORDER_FORM();
            ORDER_FORM_DETAIL ofdModel = new ORDER_FORM_DETAIL();
                        
            if (PassNum(dr["number"].ToString(), dr["budget_rid"].ToString()))
            {
                if (PassAMT(dr["number"].ToString(), dr["base_price"].ToString(), dr["budget_rid"].ToString()))
                {
                    //ofdModel.OrderForm_Detail_RID = dr["orderform_detail_rid"].ToString();
                    //ofdModel.OrderForm_RID = dr["orderform_rid"].ToString();  
                    ofdModel.OrderForm_RID = dr["orderform_rid"].ToString();
                    ofdModel.OrderForm_Detail_RID = GetMaxOFDID(ofdModel.OrderForm_RID);                   
                    dr["orderform_detail_rid"] = ofdModel.OrderForm_Detail_RID;

                    ofdModel.Number = Convert.ToInt32(dr["number"].ToString());
                    ofdModel.Is_Exigence = dr["is_exigence"].ToString();
                    if (dr["Fore_Delivery_Date"].ToString() != "")
                    {
                        ofdModel.Fore_Delivery_Date = Convert.ToDateTime(dr["Fore_Delivery_Date"].ToString());
                    }
                    if (dr["Delivery_Address_RID"].ToString() != "")
                    {
                        ofdModel.Delivery_Address_RID = Convert.ToInt32(dr["Delivery_Address_RID"].ToString());
                    }
                    if (dr["Agreement_RID"].ToString() != "")
                    {
                        ofdModel.Agreement_RID = Convert.ToInt32(dr["Agreement_RID"].ToString());
                    }
                    if (dr["Budget_RID"].ToString() != "")
                    {
                        ofdModel.Budget_RID = Convert.ToInt32(dr["Budget_RID"].ToString());
                    }
                    if (dr["Space_Short_RID"].ToString() != "")
                    {
                        ofdModel.CardType_RID = Convert.ToInt32(dr["Space_Short_RID"].ToString());
                    }
                    ofdModel.Comment = dr["Comment"].ToString();
                    if (dr["Wafer_RID"].ToString() != "")
                    {
                        ofdModel.Wafer_RID = Convert.ToInt32(dr["Wafer_RID"].ToString());
                    }
                    //add chaoma start
                    ofdModel.Unit_Price = Convert.ToDecimal(dr["Change_UnitPrice"].ToString());
                    ofdModel.Change_UnitPrice = Convert.ToDecimal(dr["Base_Price"].ToString());
                    //add chaoma end
                    dao.Add<ORDER_FORM_DETAIL>(ofdModel, "rid");

                    DataSet dset = dao.GetList("select * from order_form where rst='A' and orderform_rid='" + dr["orderform_rid"].ToString() + "'");

                    if (dset == null || dset.Tables[0].Rows.Count == 0)
                    {
                        ofModel.Blank_Factory_RID = Convert.ToInt32(dr["factory"].ToString());
                        ofModel.Case_Status = "N";
                        ofModel.OrderForm_RID = dr["orderform_rid"].ToString();
                        ofModel.Pass_Status = dr["pass_status"].ToString();
                        //ofModel.Pass_Date = System.DateTime.Now;
                        ofModel.Order_Date = System.DateTime.Now;
                        dao.Add<ORDER_FORM>(ofModel, "rid");
                    }
                }
                else
                {
                    throw new AlertException("預算剩餘金額不足！");
                }
            }
            else
            {
                throw new AlertException("預算剩餘總數量不足！");
            }

        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    ///// <summary>
    ///// 從訂單詳細記錄中取出相關資料更新預算及其日誌信息
    ///// </summary>
    ///// <param name="dt">訂單詳細記錄集合</param>
    //public void UpdateBudgetLog(DataTable dt)
    //{
    //    try
    //    {
    //        for (int i = 0; i < dt.Rows.Count; i++)
    //        {
    //            UpdateAMT(dt.Rows[i]["Budget_RID"].ToString(), dt.Rows[i]["number"].ToString(), dt.Rows[i]["Base_Price"].ToString(), dt.Rows[i]["orderform_detail_rid"].ToString());
    //        }
    //    }
    //    catch(Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
    //    }
    //}

  

    /// <summary>
    /// 根據編號獲得預算剩餘總卡數
    /// </summary>
    /// <param name="rid"></param>
    /// <returns></returns>
    public string GetBudget(string strBudgetRID)
    {
        dirValues.Clear();
        dirValues.Add("budget_rid", strBudgetRID);
        DataSet ds = dao.GetList(SEL_BUDGET_BY_RID, dirValues);

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["remain_total_num"].ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 根據編號獲得合約剩餘總卡數
    /// </summary>
    /// <param name="strAgreementRID"></param>
    /// <returns></returns>
    public string GetAgreement(string strAgreementRID)
    {
        DataSet ds = dao.GetList("select remain_card_num from agreement where rid='"+strAgreementRID+"'");

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "0";
        }
    }

  

    public void RollBackAgreement(DataTable dt)
    {
        try
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string AgreementSelectValue = "";
                    if (GetAgreementMainCode(dt.Rows[i]["Agreement_RID"].ToString()).Trim() != "")
                    {
                        AgreementSelectValue = GetAgreementRID(GetAgreementMainCode(dt.Rows[i]["Agreement_RID"].ToString()));
                        RollBackAgreementNum(AgreementSelectValue, dt.Rows[i]["number"].ToString(), dt.Rows[i]["orderform_detail_rid"].ToString());
                    }
                    else
                    {
                        RollBackAgreementNum(dt.Rows[i]["Agreement_RID"].ToString(), dt.Rows[i]["number"].ToString(), dt.Rows[i]["orderform_detail_rid"].ToString());
                    }                 
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void RollBackAgreement(DataRow dr)
    {
        try
        {
            if (dr["Agreement_RID"].ToString() != "")
            {
                string AgreementSelectValue = "";
                if (GetAgreementMainCode(dr["Agreement_RID"].ToString()).Trim() != "")
                {
                    AgreementSelectValue = GetAgreementRID(GetAgreementMainCode(dr["Agreement_RID"].ToString()));
                    RollBackAgreementNum(AgreementSelectValue, dr["number"].ToString(), dr["orderform_detail_rid"].ToString());
                }
                else
                {
                    RollBackAgreementNum(dr["Agreement_RID"].ToString(), dr["number"].ToString(), dr["orderform_detail_rid"].ToString());
                }            
            }
                
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void RollBackAgreement(ArrayList dr)
    {
        try
        {
            if (dr[1].ToString() != "")
            {                
                string AgreementSelectValue = "";
                if (GetAgreementMainCode(dr[1].ToString()).Trim() != "")
                {
                    AgreementSelectValue = GetAgreementRID(GetAgreementMainCode(dr[1].ToString()));
                    RollBackAgreementNum(AgreementSelectValue, dr[0].ToString(), dr[2].ToString());
                }
                else
                {
                    RollBackAgreementNum(dr[1].ToString(), dr[0].ToString(), dr[2].ToString());
                }      
            }

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void RollBackAgreementNum(string strAgreementRID,string strNum,string strOFDRID)
    {
        try
        {
            int num = Convert.ToInt32(strNum);

            DataSet ds = dao.GetList("select orderform_rid from order_form_detail where rst='A' and orderform_detail_rid='" + strOFDRID + "'");

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                dao.ExecuteNonQuery("update agreement set remain_card_num=remain_card_num+'" + num + "' where rid='" + strAgreementRID + "'");
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }


    /// <summary>
    /// 根據訂單編號刪除所有對應訂單詳細記錄
    /// </summary>
    /// <param name="strOrderForm_RID"></param>
    /// <param name="type">1:物理刪除 2:邏輯刪除</param>
    public void Delete(DataTable dt,int type)
    {
        try
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count;i++ )
                {
                    dirValues.Clear();
                    dirValues.Add("OrderForm_RID", dt.Rows[i]["orderform_rid"].ToString());

                    DataSet ds = null;

                    if (type == 1)
                    {                        
                        dao.ExecuteNonQuery("delete from order_form_detail where orderform_rid=@orderform_rid", dirValues);

                        ds = dao.GetList("select * from order_form_detail where rst='A' and orderform_rid=@OrderForm_RID", dirValues);
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            dao.ExecuteNonQuery("delete from order_form where orderform_rid=@orderform_rid", dirValues);
                        }
                    }
                    else
                    {                        
                        dao.ExecuteNonQuery("update order_form_detail set rst='D' where orderform_rid=@orderform_rid", dirValues);

                        ds = dao.GetList("select * from order_form_detail where rst='A' and orderform_rid=@OrderForm_RID", dirValues);
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            dao.ExecuteNonQuery("update order_form set rst='D' where orderform_rid=@orderform_rid", dirValues);
                        }
                    }
                }                
            }

        }
        catch(Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void Delete(ArrayList dr, int type)
    {
        try
        {
            if (dr != null && dr[3].ToString()!="")
            {                
                dirValues.Clear();
                dirValues.Add("OrderForm_RID", dr[3].ToString());
                dirValues.Add("OrderForm_Detail_RID", dr[2].ToString());

                DataSet ds = null;

                if (type == 1)
                {
                    dao.ExecuteNonQuery("delete from order_form_detail where orderform_detail_rid=@OrderForm_Detail_RID", dirValues);

                    ds = dao.GetList("select * from order_form_detail where rst='A' and orderform_rid=@OrderForm_RID",dirValues);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        dao.ExecuteNonQuery("delete from order_form where orderform_rid=@orderform_rid", dirValues);
                    }
                }
                else
                {
                    dao.ExecuteNonQuery("update order_form_detail set rst='D' where OrderForm_Detail_RID=@OrderForm_Detail_RID", dirValues);

                    ds = dao.GetList("select * from order_form_detail where rst='A' and orderform_rid=@OrderForm_RID", dirValues);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        dao.ExecuteNonQuery("update order_form set rst='D' where orderform_rid=@OrderForm_RID", dirValues);
                    }
                }
            }

           
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void Delete(DataRow dr, int type)
    {
        try
        {
            if (dr != null && dr["orderform_rid"].ToString() != "")
            {
                dirValues.Clear();
                dirValues.Add("OrderForm_RID", dr["orderform_rid"].ToString());
                dirValues.Add("OrderForm_Detail_RID", dr["orderform_detail_rid"].ToString());

                DataSet ds = null;

                if (type == 1)
                {
                    dao.ExecuteNonQuery("delete from order_form_detail where orderform_detail_rid=@OrderForm_Detail_RID", dirValues);

                    ds = dao.GetList("select * from order_form_detail where rst='A' and orderform_rid=@OrderForm_RID", dirValues);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        dao.ExecuteNonQuery("delete from order_form where orderform_rid=@orderform_rid", dirValues);
                    }
                }
                else
                {
                    dao.ExecuteNonQuery("update order_form_detail set rst='D' where OrderForm_Detail_RID=@OrderForm_Detail_RID", dirValues);

                    ds = dao.GetList("select * from order_form_detail where rst='A' and orderform_rid=@OrderForm_RID", dirValues);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        dao.ExecuteNonQuery("update order_form set rst='D' where orderform_rid=@OrderForm_RID", dirValues);
                    }
                }
            }

        
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 提交（將訂單放行狀態改為‘待放行’）
    /// </summary>
    /// <param name="strOrderForm_RID"></param>
    public void Confirm(DataTable dt)
    {
        try
        {
            dao.OpenConnection();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ORDER_FORM ofModel = new ORDER_FORM();

                    ofModel = dao.GetModel<ORDER_FORM, string>("OrderForm_RID", dt.Rows[i]["OrderForm_RID"].ToString());
                    ofModel.Pass_Status = "3";

                    dao.Update<ORDER_FORM>(ofModel, "RID");
                }
            }

            Warning.SetWarning(GlobalString.WarningType.OrderFormCommit, null);

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
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 放行（將訂單放行狀態改為‘已放行’）
    /// </summary>
    /// <param name="strOrderForm_RID"></param>    
    public void Pass(DataTable dt)
    {
        try
        {
            dao.OpenConnection();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ORDER_FORM ofModel = new ORDER_FORM();

                    ofModel = dao.GetModel<ORDER_FORM, string>("OrderForm_RID", dt.Rows[i]["OrderForm_RID"].ToString());
                    ofModel.Pass_Status = "4";
                    ofModel.Pass_Date = DateTime.Now;

                    dao.Update<ORDER_FORM>(ofModel, "RID");
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
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 退回（將訂單放行狀態改為‘退回’）
    /// </summary>
    /// <param name="strOrderForm_RID"></param>    
    public void Reject(DataTable dt)
    {
        try
        {  
            dao.OpenConnection();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ORDER_FORM ofModel = new ORDER_FORM();

                    ofModel = dao.GetModel<ORDER_FORM, string>("OrderForm_RID", dt.Rows[i]["OrderForm_RID"].ToString());
                    ofModel.Pass_Status = "2";

                    dao.Update<ORDER_FORM>(ofModel, "RID");
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
            //關閉連接
            dao.CloseConnection();
        }
    }

    #region 扣除預算，合約

    /// <summary>
    /// 增加入庫時檢查預算
    /// </summary>
    /// <param name="strofdRID">訂單流水ID</param>
    /// <param name="intNowNum">錄入卡數</param>
    private void Add_CheckBudget(string strofdRID, int intNowNum)
    {
        

        ORDER_FORM_DETAIL ofdModel = dao.GetModel<ORDER_FORM_DETAIL,string>("OrderForm_Detail_RID",strofdRID);

        if (intNowNum != 0)
        {
            AGREEMENT amModel = dao.GetModel<AGREEMENT, int>("RID", ofdModel.Agreement_RID);

            //減少合約卡數
            if (amModel.Card_Number != 0)
            {
                amModel.Remain_Card_Num = amModel.Remain_Card_Num - intNowNum;
                if (amModel.Remain_Card_Num < 0)
                    throw new AlertException("合約剩餘數量不足");
                dao.Update<AGREEMENT>(amModel, "RID");
            }

            AddBudget(strofdRID, ofdModel.Budget_RID, intNowNum, ofdModel.Unit_Price * intNowNum);


            #region 警訊
            CARD_BUDGET cbModel = dao.GetModel<CARD_BUDGET, int>("RID", ofdModel.Budget_RID);
            DataTable dtblBudget = dao.GetList("select max(valid_date_to) from CARD_BUDGET where Budget_Main_RID=" + ofdModel.Budget_RID.ToString()).Tables[0];
            DateTime dtMaxBudget = Convert.ToDateTime(dtblBudget.Rows[0][0]);

            AGREEMENT aModel = dao.GetModel<AGREEMENT, int>("RID", ofdModel.Agreement_RID);

            Warning.SetWarning(GlobalString.WarningType.OrderBudget, new object[6] { cbModel.Budget_ID, cbModel.Total_Card_Num, cbModel.Remain_Total_Num, cbModel.Total_Card_AMT, cbModel.Remain_Total_AMT, dtMaxBudget });
            Warning.SetWarning(GlobalString.WarningType.OrderAgreement, new object[4] { aModel.Agreement_Code, aModel.Card_Number, aModel.Remain_Card_Num, aModel.End_Time });
            #endregion
        }

    }

    /// <summary>
    /// 預算修改
    /// </summary>
    /// <param name="strofdRID">訂單流水號</param>
    /// <param name="intBudgetRID">預算ID</param>
    /// <param name="intNowNum">增加的卡數</param>
    /// <param name="decNowAmt">增加的金額</param>
    public void AddBudget(string strofdRID, int intBudgetRID, int intNowNum, decimal decNowAmt)
    {
        dirValues.Clear();
        dirValues.Add("Budget_Main_RID", intBudgetRID);

        DataTable dtblBudget = dao.GetList(SEL_BUDGET + " order by rid", dirValues).Tables[0];

        CARD_BUDGET cbMainModel = dao.GetModel<CARD_BUDGET, int>("RID", intBudgetRID);
        CARD_BUDGET cbModel = new CARD_BUDGET();


        if (dtblBudget.Rows.Count > 0)
        {

            int intRemain_Total_Num = Convert.ToInt32(dtblBudget.Rows[0]["Total_Card_Num"]);

            if (intRemain_Total_Num == 0)
                intNowNum = 0;



            foreach (DataRow drowBudget in dtblBudget.Rows)
            {
                //預算金額及卡數
                decimal decRemain_Card_Price = Convert.ToDecimal(drowBudget["Remain_Card_Price"]);
                int intRemain_Card_Num = Convert.ToInt32(drowBudget["Remain_Card_Num"]);

                //單次錄入金額及卡數
                decimal decPrice = 0.00M;
                int intNum = 0;

                cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(drowBudget["RID"]));

                if (intNowNum != 0)
                {
                    if (intRemain_Card_Num > intNowNum)
                    {
                        intNum = intNowNum;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = cbModel.Remain_Card_Num - intNum;

                        cbModel.Remain_Card_Num = cbModel.Remain_Card_Num - intNum;

                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = 0;
                    }
                    else if (intRemain_Card_Num == intNowNum)
                    {
                        intNum = intNowNum;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = 0;

                        cbModel.Remain_Card_Num = 0;


                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = 0;
                    }
                    else
                    {
                        intNum = intRemain_Card_Num;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = 0;

                        cbModel.Remain_Card_Num = 0;

                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = intNowNum - intNum;
                    }
                }

                if (decNowAmt != 0.00M)
                {
                    if (decRemain_Card_Price > decNowAmt)
                    {
                        decPrice = decNowAmt;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = decRemain_Card_Price - decPrice;

                        cbModel.Remain_Card_Price = decRemain_Card_Price - decPrice;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;

                        decNowAmt = 0.00M;
                    }
                    else if (decRemain_Card_Price == decNowAmt)
                    {
                        decPrice = decNowAmt;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = 0.00M;

                        cbModel.Remain_Card_Price = 0.00M;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;

                        decNowAmt = 0.00M;
                    }
                    else
                    {
                        decPrice = decRemain_Card_Price;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = 0.00M;

                        cbModel.Remain_Card_Price = 0.00m;

                        decNowAmt = decNowAmt - decPrice;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;
                    }
                }

                if (intNum != 0 || decPrice != 0.00M)
                {
                    MergBUDGET_LOG(strofdRID, Convert.ToInt32(drowBudget["RID"]), decPrice, intNum);
                    dao.Update<CARD_BUDGET>(cbModel, "RID");
                }
            }

            dao.Update<CARD_BUDGET>(cbMainModel, "RID");
        }
    }
    #endregion

    /// <summary>
    /// 更新預算日誌
    /// </summary>
    /// <param name="strofdRID">訂單流水號</param>
    /// <param name="intBudgetRID">預算ID</param>
    /// <param name="decBudgetAmt">預算金額</param>
    /// <param name="intNowNum">預算卡數</param>
    private void MergBUDGET_LOG(string strofdRID, int intBudgetRID, decimal decBudgetAmt, int intNowNum)
    {
        dirValues.Clear();
        dirValues.Add("Budget_RID", intBudgetRID);
        dirValues.Add("OrderForm_Detail_RID", strofdRID);

        ORDER_BUDGET_LOG orlModel = dao.GetModel<ORDER_BUDGET_LOG>(SEL_ORDER_BUDGET_LOG_2 + " and Budget_RID=@Budget_RID", dirValues);

        if (orlModel == null)
        {
            orlModel = new ORDER_BUDGET_LOG();
            orlModel.Budget_RID = intBudgetRID;
            orlModel.OrderForm_Detail_RID = strofdRID;
            orlModel.Remain_Card_Num = intNowNum;
            orlModel.Remain_Card_Price = decBudgetAmt;
            dao.Add<ORDER_BUDGET_LOG>(orlModel, "RID");
        }
        else
        {
            orlModel.Remain_Card_Num = orlModel.Remain_Card_Num + intNowNum;
            orlModel.Remain_Card_Price = orlModel.Remain_Card_Price + decBudgetAmt;
            if (orlModel.Remain_Card_Num < 0 || orlModel.Remain_Card_Price < 0)
                throw new AlertException("日誌不能為負數,更新失敗");
            dao.Update<ORDER_BUDGET_LOG>(orlModel, "RID");
        }
    }

    #region 回補預算，合約
    /// <summary>
    /// 更新減少的卡數金額
    /// </summary>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intOldNum">舊入庫數量</param>
    /// <param name="intNowNum">新入庫數量</param>
    /// <param name="intRID">入庫ID</param>
    private void UpdateBudget(string strofdRID, int intReduceNum)
    {
        ORDER_FORM_DETAIL ofdModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", strofdRID);

        decimal decReduceAmt = 0.0M;

        decReduceAmt = ofdModel.Unit_Price * intReduceNum;
        //預算
        ReduceBudget(strofdRID, ofdModel.Budget_RID, intReduceNum, decReduceAmt);

        //合約修改
        AGREEMENT amModel = dao.GetModel<AGREEMENT, int>("RID", ofdModel.Agreement_RID);

        //增加合約卡數
        if (amModel.Card_Number != 0)
        {
            amModel.Remain_Card_Num = amModel.Remain_Card_Num + intReduceNum;
            dao.Update<AGREEMENT>(amModel, "RID");
        }

    }

    /// <summary>
    /// 減少的預算
    /// </summary>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intBudgetRID">主預算ID</param>
    /// <param name="intReductNum">減少數量</param>
    /// <param name="decReductAmt">減少金額</param>
    public void ReduceBudget(string strofdRID, int intBudgetRID, int intReductNum, decimal decReductAmt)
    {
        Depository003BL bl = new Depository003BL();
        bl.dao = this.dao;
        bl.ReduceBudget(strofdRID, intBudgetRID, intReductNum, decReductAmt);
    }

    #endregion
}
