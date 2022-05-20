//******************************************************************
//*  作    者：FangBao
//*  功能說明：案件歷程查詢 
//*  創建日期：2008-11-24
//*  修改日期：2008-11-24 12:00
//*  修改記錄：
//*            □2008-11-24
//*              1.創建 鮑方
//*******************************************************************

using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
/// <summary>
/// Depository001BL 的摘要描述
/// </summary>
public class Depository001BL : BaseLogic
{
    #region SQL語句
    public const string SEL_MAINTREEDATA = "select orderForm_RID as SID,'訂單編號('+orderForm_RID+')' as Name,'' as PID from ORDER_FORM where 1>0 ";

    public const string SEL_TREEDATA = " "
                                    +" select OrderForm_Detail_RID as SID,"
                                    +" '訂單流水編號('+OrderForm_Detail_RID+','+Convert(varchar(20),Fore_Delivery_Date,111)+','+convert(varchar(20),Number)+')' as Name,"
                                    +" orderForm_RID as PID"
                                    +" from ORDER_FORM_DETAIL"
                                    +" union all"
                                    + " select distinct stock_rid as SID,'入庫('+stock_rid+','+Convert(varchar(20),Income_Date,111)+','+convert(varchar(20),Income_Number)+',已請款)' as Name,OrderForm_Detail_RID as PID from DEPOSITORY_STOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=1 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') union all select stock_rid as SID,'入庫('+stock_rid+','+Convert(varchar(20),Income_Date,111)+','+convert(varchar(20),Income_Number)+',未請款)' as Name,OrderForm_Detail_RID as PID from DEPOSITORY_STOCK where rid not in (select tb1.rid from DEPOSITORY_STOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=1 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01')) "
                                    +" union all"
                                    + " select distinct Cancel_RID as SID,'退貨('+Cancel_RID+','+Convert(varchar(20),Cancel_Date,111)+','+convert(varchar(20),Cancel_Number)+',已請款)' as Name,OrderForm_Detail_RID as PID from DEPOSITORY_CANCEL tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=3 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') union all select Cancel_RID as SID,'退貨('+Cancel_RID+','+Convert(varchar(20),Cancel_Date,111)+','+convert(varchar(20),Cancel_Number)+',未請款)' as Name,OrderForm_Detail_RID as PID from DEPOSITORY_CANCEL where rid not in (select tb1.rid from DEPOSITORY_CANCEL tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=3 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01'))"
                                    +" union all"
                                    + " select distinct Restock_RID as SID,'再入庫('+Restock_RID+','+Convert(varchar(20),Reincome_Date,111)+','+convert(varchar(20),Reincome_Number)+',已請款)' as Name,OrderForm_Detail_RID as PID from DEPOSITORY_RESTOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=2 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') union all select Restock_RID as SID,'再入庫('+Restock_RID+','+Convert(varchar(20),Reincome_Date,111)+','+convert(varchar(20),Reincome_Number)+',未請款)' as Name,OrderForm_Detail_RID as PID from DEPOSITORY_RESTOCK where rid not in (select tb1.rid from DEPOSITORY_RESTOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=2 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01'))";

    public const string SEL_DEPOSITORY_STOCK = "select ds.RID,tb.Is_AskFinance,ct.name,OFD.Number,convert(varchar(20),Order_Date,111) as Order_Date,Stock_Number,Blemish_Number,Sample_Number,Income_Number,convert(varchar(20),Income_Date,111) as Income_Date,DS.Serial_Number,F_P.Factory_ShortName_CN AS Perso_Factory_Name,F_B.Factory_ShortName_CN as Blank_Factory_NAME,WI.Wafer_Name,Case_Status,ds.Comment from dbo.DEPOSITORY_STOCK ds inner join card_Type ct on ds.Space_Short_RID=ct.RID inner join ORDER_FORM_DETAIL OFD on ds.OrderForm_Detail_RID=OFD.OrderForm_Detail_RID LEFT join FACTORY F_P ON F_P.RST = 'A' AND F_P.IS_Perso ='Y' AND F_P.RID=DS.Perso_Factory_RID LEFT join FACTORY F_B ON F_B.RST = 'A' AND F_B.IS_BLANK ='Y' AND F_B.RID=DS.Blank_Factory_RID LEFT JOIN WAFER_INFO WI ON WI.RST='A' AND WI.RID=DS.Wafer_RID inner join (select distinct tb1.rid,'Y' as Is_AskFinance from DEPOSITORY_STOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=1 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') union all select rid,'N' as Is_AskFinance from DEPOSITORY_STOCK where rid not in (select tb1.rid from DEPOSITORY_STOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=1 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01'))) tb on ds.rid=tb.rid ";

    public const string SEL_DEPOSITORY_CANCEL = "select ct.name,Cancel_Number,convert(varchar(20),Cancel_Date,111) as Cancel_Date,Factory_ShortName_CN as Blank_Factory_NAME,WI.Wafer_Name,dc.Comment,tb.Is_AskFinance,dc.rid from dbo.DEPOSITORY_CANCEL dc inner join card_Type ct on dc.Space_Short_RID=ct.RID LEFT join FACTORY F_B ON F_B.IS_BLANK ='Y' AND F_B.RID=dc.Blank_Factory_RID LEFT JOIN WAFER_INFO WI ON WI.RST='A' AND WI.RID=dc.Wafer_RID inner join (select distinct tb1.rid,'Y' as Is_AskFinance from DEPOSITORY_CANCEL tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=3 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') union all select rid,'N' as Is_AskFinance from DEPOSITORY_CANCEL where rid not in (select tb1.rid from DEPOSITORY_CANCEL tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=3 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01'))) tb on dc.rid=tb.rid ";

    public const string SEL_DEPOSITORY_RESTOCK = "select ct.name,convert(varchar(20),Reincome_Date,111) as Reincome_Date,Restock_Number,Blemish_Number,Sample_Number,Reincome_Number,Case_Status,Serial_Number,F_P.Factory_ShortName_CN AS Perso_Factory_Name,F_B.Factory_ShortName_CN as Blank_Factory_NAME,WI.Wafer_Name,dr.Comment,dr.rid,tb.Is_AskFinance from dbo.DEPOSITORY_RESTOCK DR inner join card_Type ct on DR.Space_Short_RID=ct.RID LEFT join FACTORY F_P ON F_P.IS_Perso ='Y' AND F_P.RID=DR.Perso_Factory_RID LEFT join FACTORY F_B ON F_B.IS_BLANK ='Y' AND F_B.RID=DR.Blank_Factory_RID LEFT JOIN WAFER_INFO WI ON WI.RST='A' AND WI.RID=DR.Wafer_RID inner join ORDER_FORM_DETAIL OFD on dr.OrderForm_Detail_RID=OFD.OrderForm_Detail_RID inner join (select distinct tb1.rid,'Y' as Is_AskFinance from DEPOSITORY_RESTOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=2 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') union all select rid,'N' as Is_AskFinance from DEPOSITORY_RESTOCK where rid not in (select tb1.rid from DEPOSITORY_RESTOCK tb1 left join (select a.operate_rid,a.operate_type,b.sap_serial_number,b.rid from CARD_TYPE_SAP_DETAIL a left join CARD_TYPE_SAP b on a.sap_rid=b.rid) tb2 on tb1.rid=tb2.operate_rid and tb2.operate_type=2 where (sap_serial_number <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01'))) tb on dr.rid=tb.rid ";

    public const string SEL_ASKFINANCE = "select * from CARD_TYPE_SAP_DETAIL where Operate_Type=@Operate_Type and Operate_RID=@Operate_RID";

    public const string SEL_CARD_TYPE_SAP = "SELECT * FROM CARD_TYPE_SAP WHERE RID=@RID";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Depository001BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public DataSet GetCARD_TYPE_SAP(string strSAPRID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", strSAPRID);
            dst = dao.GetList(SEL_CARD_TYPE_SAP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    public DataSet GetFinance(string strOprType,string strOprRID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Operate_Type", strOprType);
            dirValues.Add("Operate_RID", strOprRID);
            dst = dao.GetList(SEL_ASKFINANCE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }
    

    /// <summary>
    /// 獲取入庫明細
    /// </summary>
    /// <param name="strOrderForm_Detail_RID"></param>
    /// <returns></returns>
    public DataSet GetDSDetail(string strOrderForm_Detail_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Stock_RID", strOrderForm_Detail_RID);
            dst = dao.GetList(SEL_DEPOSITORY_STOCK + " where DS.Stock_RID=@Stock_RID", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 獲取退貨明細
    /// </summary>
    /// <param name="strOrderForm_Detail_RID"></param>
    /// <returns></returns>
    public DataSet GetDCDetail(string strCancel_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Cancel_RID", strCancel_RID);
            dst = dao.GetList(SEL_DEPOSITORY_CANCEL + " where DC.Cancel_RID=@Cancel_RID", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 獲取在入庫明細
    /// </summary>
    /// <param name="strOrderForm_Detail_RID"></param>
    /// <returns></returns>
    public DataSet GetDRDetail(string strRestock_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Restock_RID", strRestock_RID);
            dst = dao.GetList(SEL_DEPOSITORY_RESTOCK + " where DR.Restock_RID=@Restock_RID", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 獲取數
    /// </summary>
    /// <returns></returns>
    public DataSet GetTreeData(Dictionary<string, object> searchInput)
    {
        DataSet dst = null;
        try
        {
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();
            dirValues.Clear();

            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {

                if (!StringUtil.IsEmpty(searchInput["txtIncome_DateFrom"].ToString()))
                {
                    stbWhere.Append(" and Order_Date>=@txtIncome_DateFrom ");
                    dirValues.Add("txtIncome_DateFrom", searchInput["txtIncome_DateFrom"].ToString()+" 00:00:00");
                }
                if (!StringUtil.IsEmpty(searchInput["txtIncome_DateTo"].ToString()))
                {
                    stbWhere.Append(" and Order_Date<=@txtIncome_DateTo ");
                    dirValues.Add("txtIncome_DateTo", searchInput["txtIncome_DateTo"].ToString()+" 23:59:59");
                }
            }

            dst = dao.GetList(SEL_MAINTREEDATA + stbWhere + SEL_TREEDATA, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }
}
