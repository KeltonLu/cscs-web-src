//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-09-25
//*  修改日期：2008-09-25 10:00
//*  修改記錄：
//*            □2008-09-25
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
/// Depository002QBL 的摘要描述
/// </summary>
public class Depository002QBL : BaseLogic
{
    #region SQL語句  
    //mod chaoma start
    //edit by Ian HUang start
    //public const string SEL_ORDER_FORM_DETAIL2 = "SELECT case OFD.case_status when 'Y' then 0 else case when T.SumTotal<0 then 0 else T.SumTotal end end as SumTotal,OFD.case_status as case_status1,OFD.orderform_detail_rid,OFD.orderform_rid,convert(varchar(20),[of].order_date,111) as order_date,[of].case_status,[of].Blank_Factory_RID,F1.factory_shortname_cn as fsc,OFD.Unit_Price, OFD.CardType_RID,Display_Name as Space_Short_RID, OFD.Number, OFD.Budget_RID,CB.budget_name,OFD.Agreement_RID, AM.agreement_name,convert(varchar(20),OFD.Fore_Delivery_Date,111) as Fore_Delivery_Date, OFD.Wafer_RID, WI.wafer_name,OFD.Is_Exigence,P.param_name as pm, OFD.Delivery_Address_RID, F2.factory_shortname_cn,F2.rid,P2.param_name,OFD.comment"
    //public const string SEL_ORDER_FORM_DETAIL2 = "SELECT case OFD.case_status when 'Y' then 0 else case when T.SumTotal<0 then 0 else T.SumTotal end end as SumTotal,OFD.case_status as case_status1,OFD.orderform_detail_rid,OFD.orderform_rid,convert(varchar(20),[of].order_date,111) as order_date,[of].case_status,[of].Blank_Factory_RID,F1.factory_shortname_cn as fsc,OFD.Unit_Price, OFD.CardType_RID,Display_Name as Space_Short_RID, OFD.Number, OFD.Budget_RID,CB.budget_name,OFD.Agreement_RID, AM.agreement_name,convert(varchar(20),OFD.Fore_Delivery_Date,111) as Fore_Delivery_Date, OFD.Wafer_RID, WI.wafer_name,OFD.Is_Exigence,P.param_name as pm, OFD.Delivery_Address_RID, F2.factory_shortname_cn,F2.rid,P2.param_name,OFD.comment,OFD.Change_UnitPrice"
    //max edit 2011.03.05
    public const string SEL_ORDER_FORM_DETAIL2 = "SELECT case OFD.case_status when 'Y' then 0 else case when T.SumTotal<0 then 0 else T.SumTotal end end as SumTotal,OFD.case_status as case_status1,OFD.orderform_detail_rid,OFD.orderform_rid,convert(varchar(20),[of].order_date,111) as order_date,[of].case_status,[of].Blank_Factory_RID,F1.factory_shortname_cn as fsc,OFD.Unit_Price, OFD.CardType_RID,Display_Name as Space_Short_RID, OFD.Number, OFD.Budget_RID,CB.budget_name,OFD.Agreement_RID, AM.agreement_name,convert(varchar(20),OFD.Fore_Delivery_Date,111) as Fore_Delivery_Date, OFD.Wafer_RID, WI.wafer_name,OFD.Is_Exigence,P.param_name as pm, OFD.Delivery_Address_RID, F2.factory_shortname_cn,F2.rid,P2.param_name,OFD.comment,OFD.Change_UnitPrice,T.IDate"
        //edit by Ian HUang end
        //mod chaoma end
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
        + " ON P2.RST = 'A' AND P2.paramtype_code = 'closedState' AND P2.param_code = OFD.case_status"
        + " LEFT OUTER JOIN WAFER_INFO AS WI"
        + " ON WI.RST = 'A' AND WI.RID = OFD.Wafer_RID"
        //edit by Ian HUang start
        //+ " INNER join (select isnull(a.num1,0)-isnull(b.num2,0)-isnull(c.num3,0)+isnull(d.num4,0) as SumTotal,a.orderForm_detail_RID from (select isnull(sum(number),0) num1,orderForm_detail_RID from ORDER_FORM_DETAIL group by orderForm_detail_RID) a left join  (select isnull(sum(Income_Number),0) num2,orderForm_detail_RID from DEPOSITORY_STOCK group by orderForm_detail_RID) b on a.orderForm_detail_RID=b.orderForm_detail_RID left join (select isnull(sum(Reincome_Number),0) num3,orderForm_detail_RID from DEPOSITORY_RESTOCK group by orderForm_detail_RID) c on a.orderForm_detail_RID=c.orderForm_detail_RID left join (select isnull(sum(cancel_number),0) num4,orderForm_detail_RID from DEPOSITORY_CANCEL group by orderForm_detail_RID) d on a.orderForm_detail_RID=d.orderForm_detail_RID) as T on T.orderForm_detail_RID=OFD.orderForm_detail_RID "
        + @" INNER join 
            (
            select isnull(a.num1,0)-isnull(b.num2,0)-isnull(c.num3,0)+isnull(d.num4,0) as SumTotal,a.orderForm_detail_RID,
            case when isnull(b.IDate2,'1900/1/1') > isnull(c.IDate3,'1900/1/1') then isnull(b.IDate2,'1900/1/1') else isnull(c.IDate3,'1900/1/1') end as IDate 
            from 
            (
            select isnull(sum(number),0) num1,orderForm_detail_RID 
            from ORDER_FORM_DETAIL 
            group by orderForm_detail_RID
            ) a 
            left join  (
            select isnull(sum(Income_Number),0) num2,orderForm_detail_RID,max(Income_Date) as IDate2 
            from DEPOSITORY_STOCK 
            group by orderForm_detail_RID
            ) b on a.orderForm_detail_RID=b.orderForm_detail_RID 
            left join (
            select isnull(sum(Reincome_Number),0) num3,orderForm_detail_RID,max(Reincome_Date) as IDate3 
            from DEPOSITORY_RESTOCK 
            group by orderForm_detail_RID
            ) c on a.orderForm_detail_RID=c.orderForm_detail_RID 
            left join (
            select isnull(sum(cancel_number),0) num4,orderForm_detail_RID 
            from DEPOSITORY_CANCEL 
            group by orderForm_detail_RID
            ) d on a.orderForm_detail_RID=d.orderForm_detail_RID
            ) as T 
            on T.orderForm_detail_RID=OFD.orderForm_detail_RID "
        //edit by Ian HUang end
        + " WHERE OFD.RST = 'A'";
    public const string SEL_ORDER_FORM_DETAIL2_Order = "SELECT T.SumTotal as SumTotal,OFD.case_status as case_status1,OFD.orderform_detail_rid,OFD.orderform_rid,convert(varchar(20),[of].order_date,111) as order_date,[of].case_status,[of].Blank_Factory_RID,F1.factory_shortname_cn as fsc,OFD.Unit_Price, OFD.CardType_RID,Display_Name as Space_Short_RID, OFD.Number, OFD.Budget_RID,CB.budget_name,OFD.Agreement_RID, AM.agreement_name,convert(varchar(20),OFD.Fore_Delivery_Date,111) as Fore_Delivery_Date, OFD.Wafer_RID, WI.wafer_name,OFD.Is_Exigence,P.param_name as pm, OFD.Delivery_Address_RID, F2.factory_shortname_cn,F2.rid,P2.param_name,OFD.comment,OFD.Change_UnitPrice,T.IDate"

    //edit by Ian HUang end
    //mod chaoma end
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
        + " ON P2.RST = 'A' AND P2.paramtype_code = 'closedState' AND P2.param_code = OFD.case_status"
        + " LEFT OUTER JOIN WAFER_INFO AS WI"
        + " ON WI.RST = 'A' AND WI.RID = OFD.Wafer_RID"
        //edit by Ian HUang start
        //+ " INNER join (select isnull(a.num1,0)-isnull(b.num2,0)-isnull(c.num3,0)+isnull(d.num4,0) as SumTotal,a.orderForm_detail_RID from (select isnull(sum(number),0) num1,orderForm_detail_RID from ORDER_FORM_DETAIL group by orderForm_detail_RID) a left join  (select isnull(sum(Income_Number),0) num2,orderForm_detail_RID from DEPOSITORY_STOCK group by orderForm_detail_RID) b on a.orderForm_detail_RID=b.orderForm_detail_RID left join (select isnull(sum(Reincome_Number),0) num3,orderForm_detail_RID from DEPOSITORY_RESTOCK group by orderForm_detail_RID) c on a.orderForm_detail_RID=c.orderForm_detail_RID left join (select isnull(sum(cancel_number),0) num4,orderForm_detail_RID from DEPOSITORY_CANCEL group by orderForm_detail_RID) d on a.orderForm_detail_RID=d.orderForm_detail_RID) as T on T.orderForm_detail_RID=OFD.orderForm_detail_RID "
        + @" INNER join 
            (
            select isnull(a.num1,0)-isnull(b.num2,0)-isnull(c.num3,0)+isnull(d.num4,0) as SumTotal,a.orderForm_detail_RID,
            case when isnull(b.IDate2,'1900/1/1') > isnull(c.IDate3,'1900/1/1') then isnull(b.IDate2,'1900/1/1') else isnull(c.IDate3,'1900/1/1') end as IDate 
            from 
            (
            select isnull(sum(number),0) num1,orderForm_detail_RID 
            from ORDER_FORM_DETAIL 
            group by orderForm_detail_RID
            ) a 
            left join  (
            select isnull(sum(Income_Number),0) num2,orderForm_detail_RID,max(Income_Date) as IDate2 
            from DEPOSITORY_STOCK 
            group by orderForm_detail_RID
            ) b on a.orderForm_detail_RID=b.orderForm_detail_RID 
            left join (
            select isnull(sum(Reincome_Number),0) num3,orderForm_detail_RID,max(Reincome_Date) as IDate3 
            from DEPOSITORY_RESTOCK 
            group by orderForm_detail_RID
            ) c on a.orderForm_detail_RID=c.orderForm_detail_RID 
            left join (
            select isnull(sum(cancel_number),0) num4,orderForm_detail_RID 
            from DEPOSITORY_CANCEL 
            group by orderForm_detail_RID
            ) d on a.orderForm_detail_RID=d.orderForm_detail_RID
            ) as T 
            on T.orderForm_detail_RID=OFD.orderForm_detail_RID "
        //edit by Ian HUang end
        + " WHERE OFD.RST = 'A'";

    public const string SEL_CARD_BUDGET_ALL = "SELECT CB.RID, CB.Budget_name FROM CARD_BUDGET AS CB WHERE CB.RST = 'A'";

    public const string SEL_AGREEMENT_ALL = "SELECT A.RID, A.Agreement_name FROM AGREEMENT AS A WHERE A.RST = 'A'";

    public const string SEL_FACTORY_BLANK = "SELECT RID, factory_shortname_cn from factory WHERE RST = 'A' and is_blank='Y'";

    public const string SEL_FACTORY_PERSO = "SELECT RID, factory_shortname_cn from factory WHERE RST = 'A' and is_perso='Y'";

    public const string SEL_CLOSESTATE = "select * from param where rst='A' and paramtype_code='CaseStatus'";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Depository002QBL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
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
        string strSortField = (sortField == "null" ? "OrderForm_Detail_RID" : sortField);//默認的排序欄位

        sortType = (sortField == "null" ? "desc " : sortType);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_ORDER_FORM_DETAIL2);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (searchInput["txtBRID"].ToString().Trim() != "" && searchInput["txtMRID"].ToString().Trim() == "" && searchInput["txtERID"].ToString().Trim() == "")
            {
                stbWhere.Append("AND OFD.orderform_detail_rid like @orderform_detail_rid");
                dirValues.Add("OrderForm_Detail_RID", searchInput["txtBRID"].ToString().Trim() + "%" );
            }
            if (searchInput["txtBRID"].ToString().Trim() != "" && searchInput["txtMRID"].ToString().Trim() != "" && searchInput["txtERID"].ToString().Trim() == "")
            {
                stbWhere.Append("AND OFD.orderform_detail_rid like @orderform_detail_rid");
                dirValues.Add("OrderForm_Detail_RID", searchInput["txtBRID"].ToString().Trim() + searchInput["txtMRID"].ToString().Trim()+"%");
            }
            if (searchInput["txtBRID"].ToString().Trim() != "" && searchInput["txtMRID"].ToString().Trim() != "" && searchInput["txtERID"].ToString().Trim() != "")
            {
                stbWhere.Append("AND OFD.orderform_detail_rid like @orderform_detail_rid");
                dirValues.Add("OrderForm_Detail_RID", searchInput["txtBRID"].ToString().Trim() + searchInput["txtMRID"].ToString().Trim() + searchInput["txtERID"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtOrder_Date_FROM"].ToString().Trim()))
            {
                stbWhere.Append(" and [OF].Order_Date >=@Order_Date_FROM");
                dirValues.Add("Order_Date_FROM", searchInput["txtOrder_Date_FROM"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtOrder_Date_TO"].ToString().Trim()))
            {
                stbWhere.Append(" and [OF].Order_Date <=@Order_Date_TO");
                dirValues.Add("Order_Date_TO", searchInput["txtOrder_Date_TO"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtFore_Delivery_BDate"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.Fore_Delivery_Date >=@Fore_Delivery_FDate");
                dirValues.Add("Fore_Delivery_FDate", searchInput["txtFore_Delivery_BDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtFore_Delivery_EDate"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.Fore_Delivery_Date <=@Fore_Delivery_TDate");
                dirValues.Add("Fore_Delivery_TDate", searchInput["txtFore_Delivery_EDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropAgreement"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.Agreement_RID =@Agreement");
                dirValues.Add("Agreement", searchInput["dropAgreement"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropBudget"].ToString().Trim()))
            {
                stbWhere.Append(" and CB.RID =@Budget_RID");
                dirValues.Add("Budget_RID", searchInput["dropBudget"].ToString().Trim());
            }
            if (searchInput["isdetail"].ToString().Trim() == "訂單")
            {
                if (!StringUtil.IsEmpty(searchInput["dropCase_Status"].ToString().Trim()))
                {
                    if (searchInput["dropCase_Status"].ToString().Trim() == "全部")
                    {
                        stbWhere.Append(" and ([OF].case_status=@Case_Status or [OF].case_status=@Case_Status2)");
                        dirValues.Add("Case_Status", "Y");
                        dirValues.Add("Case_Status2", "N");
                    }
                    else if (searchInput["dropCase_Status"].ToString().Trim() == "N")
                    {
                        stbWhere.Append(" and [OF].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "N");
                    }
                    else
                    {
                        stbWhere.Append(" and [OF].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "Y");
                    }
                }
            }
            else
            {
                if (!StringUtil.IsEmpty(searchInput["dropCase_Status"].ToString().Trim()))
                {
                    if (searchInput["dropCase_Status"].ToString().Trim() == "全部")
                    {
                        stbWhere.Append(" and ([OFD].case_status=@Case_Status or [OFD].case_status=@Case_Status2)");
                        dirValues.Add("Case_Status", "Y");
                        dirValues.Add("Case_Status2", "N");
                    }
                    else if (searchInput["dropCase_Status"].ToString().Trim() == "N")
                    {
                        stbWhere.Append(" and [OFD].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "N");
                    }
                    else
                    {
                        stbWhere.Append(" and [OFD].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "Y");
                    }
                }
            }           
            if (!StringUtil.IsEmpty(searchInput["cardtype"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.CardType_RID =@CardType_RID");
                dirValues.Add("CardType_RID", searchInput["cardtype"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropBlankFactory"].ToString().Trim()))
            {
                stbWhere.Append(" and [OF].Blank_Factory_RID=@Blank_Factory_RID");
                dirValues.Add("Blank_Factory_RID", searchInput["dropBlankFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory_ShortName_CN"].ToString().Trim()))
            {
                stbWhere.Append(" and f2.rid=@Factory_ShortName_CN");
                dirValues.Add("Factory_ShortName_CN", searchInput["dropFactory_ShortName_CN"].ToString().Trim());
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
    /// 查詢訂單記錄列表(專門給下單查詢用)
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet List2(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "OrderForm_Detail_RID" : sortField);//默認的排序欄位

        sortType = (sortField == "null" ? "desc " : sortType);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_ORDER_FORM_DETAIL2_Order);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (searchInput["txtBRID"].ToString().Trim() != "" && searchInput["txtMRID"].ToString().Trim() == "" && searchInput["txtERID"].ToString().Trim() == "")
            {
                stbWhere.Append("AND OFD.orderform_detail_rid like @orderform_detail_rid");
                dirValues.Add("OrderForm_Detail_RID", searchInput["txtBRID"].ToString().Trim() + "%");
            }
            if (searchInput["txtBRID"].ToString().Trim() != "" && searchInput["txtMRID"].ToString().Trim() != "" && searchInput["txtERID"].ToString().Trim() == "")
            {
                stbWhere.Append("AND OFD.orderform_detail_rid like @orderform_detail_rid");
                dirValues.Add("OrderForm_Detail_RID", searchInput["txtBRID"].ToString().Trim() + searchInput["txtMRID"].ToString().Trim() + "%");
            }
            if (searchInput["txtBRID"].ToString().Trim() != "" && searchInput["txtMRID"].ToString().Trim() != "" && searchInput["txtERID"].ToString().Trim() != "")
            {
                stbWhere.Append("AND OFD.orderform_detail_rid like @orderform_detail_rid");
                dirValues.Add("OrderForm_Detail_RID", searchInput["txtBRID"].ToString().Trim() + searchInput["txtMRID"].ToString().Trim() + searchInput["txtERID"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtOrder_Date_FROM"].ToString().Trim()))
            {
                stbWhere.Append(" and [OF].Order_Date >=@Order_Date_FROM");
                dirValues.Add("Order_Date_FROM", searchInput["txtOrder_Date_FROM"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtOrder_Date_TO"].ToString().Trim()))
            {
                stbWhere.Append(" and [OF].Order_Date <=@Order_Date_TO");
                dirValues.Add("Order_Date_TO", searchInput["txtOrder_Date_TO"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtFore_Delivery_BDate"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.Fore_Delivery_Date >=@Fore_Delivery_FDate");
                dirValues.Add("Fore_Delivery_FDate", searchInput["txtFore_Delivery_BDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtFore_Delivery_EDate"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.Fore_Delivery_Date <=@Fore_Delivery_TDate");
                dirValues.Add("Fore_Delivery_TDate", searchInput["txtFore_Delivery_EDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropAgreement"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.Agreement_RID =@Agreement");
                dirValues.Add("Agreement", searchInput["dropAgreement"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropBudget"].ToString().Trim()))
            {
                stbWhere.Append(" and CB.RID =@Budget_RID");
                dirValues.Add("Budget_RID", searchInput["dropBudget"].ToString().Trim());
            }
            if (searchInput["isdetail"].ToString().Trim() == "訂單")
            {
                if (!StringUtil.IsEmpty(searchInput["dropCase_Status"].ToString().Trim()))
                {
                    if (searchInput["dropCase_Status"].ToString().Trim() == "全部")
                    {
                        stbWhere.Append(" and ([OF].case_status=@Case_Status or [OF].case_status=@Case_Status2)");
                        dirValues.Add("Case_Status", "Y");
                        dirValues.Add("Case_Status2", "N");
                    }
                    else if (searchInput["dropCase_Status"].ToString().Trim() == "N")
                    {
                        stbWhere.Append(" and [OF].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "N");
                    }
                    else
                    {
                        stbWhere.Append(" and [OF].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "Y");
                    }
                }
            }
            else
            {
                if (!StringUtil.IsEmpty(searchInput["dropCase_Status"].ToString().Trim()))
                {
                    if (searchInput["dropCase_Status"].ToString().Trim() == "全部")
                    {
                        stbWhere.Append(" and ([OFD].case_status=@Case_Status or [OFD].case_status=@Case_Status2)");
                        dirValues.Add("Case_Status", "Y");
                        dirValues.Add("Case_Status2", "N");
                    }
                    else if (searchInput["dropCase_Status"].ToString().Trim() == "N")
                    {
                        stbWhere.Append(" and [OFD].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "N");
                    }
                    else
                    {
                        stbWhere.Append(" and [OFD].case_status=@Case_Status");
                        dirValues.Add("Case_Status", "Y");
                    }
                }
            }
            if (!StringUtil.IsEmpty(searchInput["cardtype"].ToString().Trim()))
            {
                stbWhere.Append(" and OFD.CardType_RID =@CardType_RID");
                dirValues.Add("CardType_RID", searchInput["cardtype"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropBlankFactory"].ToString().Trim()))
            {
                stbWhere.Append(" and [OF].Blank_Factory_RID=@Blank_Factory_RID");
                dirValues.Add("Blank_Factory_RID", searchInput["dropBlankFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory_ShortName_CN"].ToString().Trim()))
            {
                stbWhere.Append(" and f2.rid=@Factory_ShortName_CN");
                dirValues.Add("Factory_ShortName_CN", searchInput["dropFactory_ShortName_CN"].ToString().Trim());
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


    public void dropCaseStatusBind(DropDownList dropCase_Status)
    {
        dropCase_Status.DataTextField = "param_name";
        dropCase_Status.DataValueField = "param_code";
        dropCase_Status.DataSource = dao.GetList(SEL_CLOSESTATE);
        dropCase_Status.DataBind();
    }

    /// <summary>
    /// 預算下拉框綁定
    /// </summary>
    public void dropBudgetBind(DropDownList dropBudget)
    {
        dropBudget.DataTextField = "Budget_name";
        dropBudget.DataValueField = "RID";
        dropBudget.DataSource = dao.GetList(SEL_CARD_BUDGET_ALL);
        dropBudget.DataBind();
    }

    /// <summary>
    /// 合約下拉框綁定
    /// </summary>
    public void dropAgreementBind(DropDownList dropAgreement)
    {
        dropAgreement.DataTextField = "Agreement_name";
        dropAgreement.DataValueField = "RID";
        dropAgreement.DataSource = dao.GetList(SEL_AGREEMENT_ALL);
        dropAgreement.DataBind();
    }

    /// <summary>
    /// 空白卡廠下拉框綁定
    /// </summary>
    public void dropBlankFactoryBind(DropDownList dropBlankFactory)
    {
        dropBlankFactory.DataTextField = "factory_shortname_cn";
        dropBlankFactory.DataValueField = "RID";
        dropBlankFactory.DataSource = dao.GetList(SEL_FACTORY_BLANK);
        dropBlankFactory.DataBind();
    }

    /// <summary>
    /// Perso卡廠下拉框綁定
    /// </summary>
    public void dropPersoFactoryBind(DropDownList dropPersoFactory)
    {
        dropPersoFactory.DataTextField = "factory_shortname_cn";
        dropPersoFactory.DataValueField = "RID";
        dropPersoFactory.DataSource = dao.GetList(SEL_FACTORY_PERSO);
        dropPersoFactory.DataBind();
    }
}
