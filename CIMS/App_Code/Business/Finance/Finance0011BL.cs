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
/// Finance0011BL 的摘要描述
/// </summary>
public class Finance0011BL : BaseLogic
{
    #region SQL語句
    public const string SEL_PARAM_USE = "SELECT * FROM PARAM WHERE RST = 'A' AND ParamType_Code = '" + GlobalString.ParameterType.Use + "'";

    public const string SEL_CARD_GROUP = "SELECT RID,GROUP_NAME FROM CARD_GROUP WHERE RST='A' ";

    public const string SEL_PERSON = "SELECT RID,Factory_ShortName_CN "+
                            "FROM FACTORY WHERE RST = 'A' AND Is_Blank = 'Y' ";

    // (查詢拆單訊息)
    public const string SEL_DEPOSITORY_SPLIT_RECORD = "SELECT a.Is_AskFinance,A.RID,A.Operate_RID,A.CARDTYPE_RID,A.NAME,A.FRID,A.Factory_ShortName_CN,A.Budget_ID,A.Agreement_Code," +
        // add chaoma by 201005515-0 start
                        //"A.Stock_RID,A.Operate_Type,A.Income_Number,A.Income_Date,A.Unit_Price,A.Unit_Price1,A.Fore_Delivery_Date,A.Delay_Days,A.Comment,A.SAP_ID,A.Check_ID,A.RIC_Number " +
                        "A.Stock_RID,A.Operate_Type,A.Income_Number,A.Income_Date,A.Unit_Price,A.Unit_Price1,A.Fore_Delivery_Date,A.Delay_Days,A.Comment,A.SAP_ID,A.Check_ID,A.RIC_Number,A.Number,A.Change_UnitPrice " +
        //add chaoma end 
                    "FROM (SELECT Is_AskFinance,0 AS RID,DS.RID as Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,DS.Stock_RID,OFD.OrderForm_Detail_RID," +
        // add chaoma by 201005515-0 start
                        //"'1' AS Operate_Type,DS.Income_Number,DS.Income_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DS.Comment,'' AS SAP_ID,'' as Check_ID,DS.Income_Number AS RIC_Number " +
                        "'1' AS Operate_Type,DS.Income_Number,DS.Income_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DS.Comment,'' AS SAP_ID,'' as Check_ID,DS.Income_Number AS RIC_Number,OFD.Number,OFD.Change_UnitPrice " +
        //add chaoma end 
                          "FROM DEPOSITORY_STOCK DS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DS.Space_Short_RID = CT.RID " +
                            "INNER JOIN Factory F ON DS.Blank_Factory_RID = F.RID " +
                            "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND DS.Budget_RID = CB.RID " +
                            "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND DS.Agreement_RID = AGM.RID " +
                            "INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DS.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                          "WHERE DS.RST = 'A' AND DS.Income_Date>=@BGDATE AND DS.Income_Date<=@ENDDATE " +
                            "AND DS.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '1' ) " +
                          "UNION " +
                          "SELECT Is_AskFinance,0 AS RID,DR.RID as Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,DR.Stock_RID,OFD.OrderForm_Detail_RID," +
        // add chaoma by 201005515-0 start
                            //"'2' AS Operate_Type,DR.Reincome_Number,DR.Reincome_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DR.Comment,'' AS SAP_ID,'' as Check_ID,DR.ReIncome_Number AS RIC_Number " +
                            "'2' AS Operate_Type,DR.Reincome_Number,DR.Reincome_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DR.Comment,'' AS SAP_ID,'' as Check_ID,DR.ReIncome_Number AS RIC_Number,OFD.Number,OFD.Change_UnitPrice " +
        //add chaoma end 
                          "FROM DEPOSITORY_RESTOCK DR INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DR.Space_Short_RID = CT.RID " +
                            "INNER JOIN Factory F ON DR.Blank_Factory_RID = F.RID " +
                            "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND DR.Budget_RID = CB.RID " +
                            "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND DR.Agreement_RID = AGM.RID " +
                            "INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DR.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                          "WHERE DR.RST = 'A' AND DR.REIncome_Date>=@BGDATE AND DR.REIncome_Date<=@ENDDATE " +
                            "AND DR.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '2' ) " +
                          "UNION " +
                          "SELECT Is_AskFinance,0 AS RID,DC.RID as Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,DC.Stock_RID,OFD.OrderForm_Detail_RID," +
        // add chaoma by 201005515-0 start
                            //"'3' AS Operate_Type,DC.Cancel_Number,DC.Cancel_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DC.Comment,'' AS SAP_ID,'' as Check_ID,DC.Cancel_Number AS RIC_Number " +
                            "'3' AS Operate_Type,DC.Cancel_Number,DC.Cancel_Date,OFD.Unit_Price,0 as Unit_Price1,OFD.Fore_Delivery_Date,0 as Delay_Days,DC.Comment,'' AS SAP_ID,'' as Check_ID,DC.Cancel_Number AS RIC_Number,OFD.Number,OFD.Change_UnitPrice " +
        //add chaoma end 
                          "FROM DEPOSITORY_CANCEL DC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DC.Space_Short_RID = CT.RID " +
                            "INNER JOIN Factory F ON DC.Blank_Factory_RID = F.RID " +
                            "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND DC.Budget_RID = CB.RID " +
                            "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND DC.Agreement_RID = AGM.RID " +
                            "INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DC.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                          "WHERE DC.RST = 'A' AND DC.Cancel_Date>=@BGDATE AND DC.Cancel_Date<=@ENDDATE " +
                            "AND DC.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '3'  ) " +
                          "UNION " +
                          "SELECT case isnull(B.SAP_Serial_Number,'') when '' then 'N' else 'Y' end as Is_AskFinance,B.RID,B.Operate_RID,CT.RID AS CARDTYPE_RID,CT.Name,F.RID AS FRID,F.Factory_ShortName_CN,CB.Budget_ID,AGM.Agreement_Code,B.Stock_RID,B.OrderForm_Detail_RID,B.Operate_Type," +
                            //"B.Real_Ask_Number,B.Income_Date,B.Unit_Price,B.Unit_Price_No,'1900-01-01' AS Fore_Delivery_Date,B.Delay_Days,B.Comment,B.SAP_Serial_Number,B.Check_Serial_Number AS Check_ID,B.RIC_Number " +
        // add chaoma by 201005515-0 start
                          //"B.Real_Ask_Number,B.Income_Date,B.Unit_Price,B.Unit_Price_No,B.Fore_Delivery_Date,B.Delay_Days,B.Comment,B.SAP_Serial_Number,B.Check_Serial_Number AS Check_ID,B.RIC_Number " +
                          "B.Real_Ask_Number,B.Income_Date,B.Unit_Price,B.Unit_Price_No,B.Fore_Delivery_Date,B.Delay_Days,B.Comment,B.SAP_Serial_Number,B.Check_Serial_Number AS Check_ID,B.RIC_Number,B.Number,B.Change_UnitPrice " +
        //add chaoma end              
                          "FROM (SELECT CTSD.RID,CASE CTSD.Operate_Type WHEN '1' THEN DS.Budget_RID WHEN '2' THEN DR.Budget_RID WHEN '3' THEN DC.Budget_RID END AS Budget_RID," +
                           //200906CR     
                           "CASE CTSD.Operate_Type WHEN '1' THEN OFD.Fore_Delivery_Date else '1900-01-01' END AS Fore_Delivery_Date," +     
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Blank_Factory_RID WHEN '2' THEN DR.Blank_Factory_RID WHEN '3' THEN DC.Blank_Factory_RID END AS Blank_Factory_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Agreement_RID WHEN '2' THEN DR.Agreement_RID WHEN '3' THEN DC.Agreement_RID END AS Agreement_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Space_Short_RID WHEN '2' THEN DR.Space_Short_RID WHEN '3' THEN DC.Space_Short_RID END AS Space_Short_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Date WHEN '2' THEN DR.ReIncome_Date WHEN '3' THEN DC.Cancel_Date END AS Income_Date, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Stock_RID WHEN '2' THEN DR.Stock_RID WHEN '3' THEN DC.Stock_RID END AS Stock_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.OrderForm_Detail_RID WHEN '2' THEN DR.OrderForm_Detail_RID WHEN '3' THEN DC.OrderForm_Detail_RID END AS OrderForm_Detail_RID, " +
                                    "CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Number WHEN '2' THEN DR.Reincome_Number WHEN '3' THEN DC.Cancel_Number END AS RIC_Number, " +
        // add chaoma by 201005515-0 start
                                    //"CTSD.Unit_Price_No,CTSD.Unit_Price,CTSD.Real_Ask_Number,CTSD.Delay_Days,CTSD.Comment,CTSD.Operate_Type,CTSD.Operate_RID,CTS.SAP_Serial_Number,CTSD.Check_Serial_Number " +
                                    "CTSD.Unit_Price_No,CTSD.Unit_Price,CTSD.Real_Ask_Number,CTSD.Delay_Days,CTSD.Comment,CTSD.Operate_Type,CTSD.Operate_RID,CTS.SAP_Serial_Number,CTSD.Check_Serial_Number,OFD.Number,OFD.Change_UnitPrice " +
        //add chaoma end      
                                "FROM CARD_TYPE_SAP_DETAIL CTSD LEFT JOIN CARD_TYPE_SAP CTS ON CTS.RST = 'A' AND CTSD.SAP_RID = CTS.RID " +
                                    "LEFT JOIN DEPOSITORY_STOCK DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A'  AND CTSD.Operate_RID = DS.RID " +
                                   //200906CR 
        // add chaoma by 201005515-0 start
                                    //"LEFT JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DS.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                                    "LEFT JOIN DEPOSITORY_RESTOCK DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID " +
                                    
        // 調整退貨訂單數量顯示
                                    "LEFT JOIN DEPOSITORY_CANCEL DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID " +

                                    "LEFT JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND (DS.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID OR DR.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID OR DC.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID) " +
    
                                    //
        // add chaoma END    
                                    
                                "WHERE CTSD.RST = 'A') B INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND B.Space_Short_RID = CT.RID " +
                                    "INNER JOIN Factory F ON B.Blank_Factory_RID = F.RID " +
                                    "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND B.Budget_RID = CB.RID " +
                                    "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND B.Agreement_RID = AGM.RID " +
                          "WHERE B.Income_Date>=@BGDATE AND B.Income_Date<=@ENDDATE) A " +
                    "WHERE A.Operate_RID<>0 ";

    public const string CON_SAP_DETAIL_CANCEL = "SELECT DS.Income_Number,OFD.Unit_Price,'1' AS Operate_Type  " +
                    "FROM DEPOSITORY_STOCK DS INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DS.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                    "WHERE DS.Stock_RID = @stock_rid AND DS.RID NOT IN (DSRID) AND DS.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '1') " +
                    "UNION "+
                    "SELECT DR.ReIncome_Number,OFD.Unit_Price,'2' AS Operate_Type  " +
                    "FROM DEPOSITORY_RESTOCK DR INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DR.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                    "WHERE DR.Stock_RID = @stock_rid AND DR.RID NOT IN (DRRID) AND DR.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '2') " +
                    "UNION "+
                    "SELECT DC.Cancel_Number,OFD.Unit_Price,'3' AS Operate_Type "+
                    "FROM DEPOSITORY_CANCEL DC INNER JOIN ORDER_FORM_DETAIL OFD ON OFD.RST = 'A' AND DC.OrderForm_Detail_RID = OFD.OrderForm_Detail_RID " +
                    "WHERE DC.Stock_RID = @stock_rid AND DC.RID NOT IN (DCRID) AND DC.RID NOT IN (SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND Operate_Type = '3') " +
                    "UNION "+
                    "SELECT B.Real_Ask_Number,B.Unit_Price,B.Operate_Type " +
                    "FROM (SELECT CTSD.Operate_Type,CASE CTSD.Operate_Type WHEN '1' THEN DS.Stock_RID WHEN '2' THEN DR.Stock_RID WHEN '3' THEN DC.Stock_RID END AS Stock_RID,CTSD.Unit_Price,CTSD.Real_Ask_Number,CTSD.RID " +
                    "    FROM CARD_TYPE_SAP_DETAIL CTSD LEFT JOIN DEPOSITORY_STOCK DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A' AND CTSD.Operate_RID = DS.RID " +
                    "       LEFT JOIN DEPOSITORY_RESTOCK DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID "+
                    "       LEFT JOIN DEPOSITORY_CANCEL DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID) B "+
                    "WHERE B.Stock_RID = @stock_rid AND B.RID NOT IN (CTSDRID) ";

    public const string DEL_SAP_DETAIL_BY_RID = "DELETE FROM CARD_TYPE_SAP_DETAIL "+
                    "WHERE RID = @rid ";

    public const string DEL_SAP_DETAIL_BY_SAPRID = "UPDATE CARD_TYPE_SAP_DETAIL SET SAP_RID = 0 " +
                    "WHERE SAP_RID = @sap_rid ";


    public const string DEL_SAP = "DELETE FROM CARD_TYPE_SAP " +
                    "WHERE RID = @sap_rid ";

    // 查詢空白卡請款SAP單訊息---查詢頁面
    public const string SEL_CARD_TYPE_SAP_DETAIL = "SELECT DISTINCT A.SAP_Serial_Number,CB.Budget_ID,AGM.Agreement_Code,A.Ask_Date,A.Pass_Status,A.Is_Finance " +
                    "FROM ("+
                        "SELECT CTSD.Check_Serial_Number,CTS.SAP_Serial_Number,CTS.Pass_Status,CTS.Is_Finance,CTS.Ask_Date,"+
                            "CASE CTSD.Operate_Type WHEN '1' THEN DS.Budget_RID WHEN '2' THEN DR.Budget_RID WHEN '3' THEN DC.Budget_RID END AS Budget_RID,"+
                            "CASE CTSD.Operate_Type WHEN '1' THEN DS.Blank_Factory_RID WHEN '2' THEN DR.Blank_Factory_RID WHEN '3' THEN DC.Blank_Factory_RID END AS Blank_Factory_RID,"+
                            "CASE CTSD.Operate_Type WHEN '1' THEN DS.Agreement_RID WHEN '2' THEN DR.Agreement_RID WHEN '3' THEN DC.Agreement_RID END AS Agreement_RID "+
                        "FROM CARD_TYPE_SAP_DETAIL CTSD INNER JOIN CARD_TYPE_SAP CTS ON CTS.RST = 'A' AND CTSD.SAP_RID = CTS.RID "+
                            "LEFT JOIN DEPOSITORY_STOCK DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A'  AND CTSD.Operate_RID = DS.RID "+
                            "LEFT JOIN DEPOSITORY_RESTOCK DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID "+
                            "LEFT JOIN DEPOSITORY_CANCEL DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID "+
                        "WHERE CTSD.RST = 'A' ) A "+
                        "INNER JOIN AGREEMENT AGM ON AGM.RST = 'A' AND A.Agreement_RID = AGM.RID "+
                        "INNER JOIN CARD_BUDGET CB ON CB.RST = 'A' AND A.Budget_RID = CB.RID " +
                    "WHERE A.Ask_Date >= @Begin_Date AND A.Ask_Date <= @Finish_Date ";

    public const string SEL_SAP = "SELECT * "+
                    "FROM CARD_TYPE_SAP "+
                    "WHERE RST = 'A' AND SAP_Serial_Number = @sap_serial_number ";

    public const string SEL_SAP_DETAIL = "SELECT A.Fore_Delivery_Date,A.RID,A.Operate_RID,CT.Name,F.Factory_ShortName_CN," +
                    "CB.Budget_ID,AGM.Agreement_Code,A.Stock_RID,A.Operate_Type,"+
                    "A.Income_Number as RIC_Number,A.Income_Date,A.Unit_Price,A.Unit_Price_No," +
        // add chaoma by 201005515-0 start
                    //"A.Real_Ask_Number as Income_Number,A.Delay_Days,A.Comment,A.Check_Serial_Number as Check_ID "+
                    "A.Real_Ask_Number as Income_Number,A.Delay_Days,A.Comment,A.Check_Serial_Number as Check_ID,A.Number,A.Change_UnitPrice " +
        // add chaoma end
                "FROM (SELECT CASE CTSD.Operate_Type WHEN '1' THEN DS.Fore_Delivery_Date WHEN '2' THEN DR.Fore_Delivery_Date WHEN '3' THEN DC.Fore_Delivery_Date else '1900-01-01' END AS Fore_Delivery_Date,CASE CTSD.Operate_Type WHEN '1' THEN DS.Budget_RID WHEN '2' THEN DR.Budget_RID WHEN '3' THEN DC.Budget_RID END AS Budget_RID," +
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Blank_Factory_RID WHEN '2' THEN DR.Blank_Factory_RID WHEN '3' THEN DC.Blank_Factory_RID END AS Blank_Factory_RID,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Agreement_RID WHEN '2' THEN DR.Agreement_RID WHEN '3' THEN DC.Agreement_RID END AS Agreement_RID,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Space_Short_RID WHEN '2' THEN DR.Space_Short_RID WHEN '3' THEN DC.Space_Short_RID END AS Space_Short_RID,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Stock_RID WHEN '2' THEN DR.Stock_RID WHEN '3' THEN DC.Stock_RID END AS Stock_RID," +
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Date WHEN '2' THEN DR.ReIncome_Date WHEN '3' THEN DC.Cancel_Date END AS Income_Date,"+
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Income_Number WHEN '2' THEN DR.ReIncome_Number WHEN '3' THEN DC.Cancel_Number END AS Income_Number,"+
                        "CTSD.Unit_Price,CTSD.Unit_Price_No,CTSD.Real_Ask_Number,CTSD.Delay_Days,CTSD.Comment,CTSD.Check_Serial_Number,"+
        // add chaoma by 201005515-0 start
                        //"CTSD.Operate_Type,CTSD.SAP_RID,CTSD.Operate_RID,CTSD.RID " +
                        "CTSD.Operate_Type,CTSD.SAP_RID,CTSD.Operate_RID,CTSD.RID,"+
        // Modify the fileds "Number" and "Change_UnitPrice will get null value when restock and cancel situation; add by wallace in 2011/10/14
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Number WHEN '2' THEN DR.Number WHEN '3' THEN DC.Number END AS Number," +
                        "CASE CTSD.Operate_Type WHEN '1' THEN DS.Change_UnitPrice WHEN '2' THEN DR.Change_UnitPrice WHEN '3' THEN DR.Change_UnitPrice END AS Change_UnitPrice " +
        //add wallace end
//                        "DS.Number," +
//                        "DS.Change_UnitPrice " +
        //"FROM CARD_TYPE_SAP_DETAIL CTSD LEFT JOIN (select a.*,b.Fore_Delivery_Date from DEPOSITORY_STOCK a inner join Order_form_detail b on a.OrderForm_detail_rid=b.OrderForm_detail_rid) DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A' AND CTSD.Operate_RID = DS.RID "+
                        "FROM CARD_TYPE_SAP_DETAIL CTSD LEFT JOIN (select a.*,b.Fore_Delivery_Date,b.Number,b.Change_UnitPrice from DEPOSITORY_STOCK a inner join Order_form_detail b on a.OrderForm_detail_rid=b.OrderForm_detail_rid) DS ON CTSD.Operate_Type = '1' AND DS.RST = 'A' AND CTSD.Operate_RID = DS.RID " +
        // add chaoma end
        // Modify the fileds "Number" and "Change_UnitPrice will get null value when restock and cancel situation; add by wallace in 2011/10/14
                        "LEFT JOIN (select a.*,b.Fore_Delivery_Date,b.Number,b.Change_UnitPrice from DEPOSITORY_RESTOCK a inner join Order_form_detail b on a.OrderForm_detail_rid=b.OrderForm_detail_rid) DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID " +
                        "LEFT JOIN (select a.*,b.Fore_Delivery_Date,b.Number,b.Change_UnitPrice from DEPOSITORY_CANCEL a inner join Order_form_detail b on a.OrderForm_detail_rid=b.OrderForm_detail_rid) DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID " +
        //add wallace end
//                        "LEFT JOIN DEPOSITORY_RESTOCK DR ON CTSD.Operate_Type = '2' AND DR.RST = 'A'  AND CTSD.Operate_RID = DR.RID " +
//                        "LEFT JOIN DEPOSITORY_CANCEL DC ON CTSD.Operate_Type = '3' AND DC.RST = 'A'  AND CTSD.Operate_RID = DC.RID " +
                    "WHERE CTSD.RST = 'A' ) A LEFT JOIN AGREEMENT AGM ON AGM.RST = 'A' AND A.Agreement_RID = AGM.RID " +
                        "LEFT JOIN CARD_BUDGET CB ON CB.RST = 'A' AND A.Budget_RID = CB.RID "+
                        "LEFT JOIN CARD_TYPE CT ON CT.RST = 'A' AND A.Space_Short_RID = CT. RID "+
                        "LEFT JOIN Factory F ON F.RST = 'A' AND A.Blank_Factory_RID = F.RID "+
                        "INNER JOIN CARD_TYPE_SAP CTS ON A.SAP_RID = CTS.RID "+
                "WHERE CTS.SAP_Serial_Number=@sap_serial_number ";
    // add chaoma by 201005515-0 start
    //public const string IN_CARD_MONTH_FORCAST_PRINT = "INSERT INTO RPT_Finance0011Add_2  (Name,Factory_ShortName_CN,Budget_ID,Agreement_Code,Stock_RID,Operate_Type,Income_Number,Income_Date,Unit_Price,Unit_Price_No,Real_Ask_Number,Real_Pay_Money,Real_Pay_Money_No,Delay_Days,Check_Serial_Number,Comment,TimeMark) VALUES (@name,@factory_shortname_cn,@budget_id,@agreement_code,@stock_rid,@operate_type,@income_number,@income_date,@unit_price,@unit_price_No,@real_ask_number,@real_pay_money,@real_pay_money_no,@delay_days,@check_serial_number,@comment,@TimeMark) ";
    public const string IN_CARD_MONTH_FORCAST_PRINT = "INSERT INTO RPT_Finance0011Add_2  (Name,Factory_ShortName_CN,Budget_ID,Agreement_Code,Stock_RID,Operate_Type,Income_Number,Income_Date,Unit_Price,Unit_Price_No,Real_Ask_Number,Real_Pay_Money,Real_Pay_Money_No,Delay_Days,Check_Serial_Number,Comment,TimeMark,Change_UnitPrice,Number) VALUES (@name,@factory_shortname_cn,@budget_id,@agreement_code,@stock_rid,@operate_type,@income_number,@income_date,@unit_price,@unit_price_No,@real_ask_number,@real_pay_money,@real_pay_money_no,@delay_days,@check_serial_number,@comment,@TimeMark,@Change_UnitPrice,@Number) ";
    // add chaoma end

    public const string CON_SAP	= "SELECT COUNT(*) " +
                      "FROM  (SELECT DISTINCT SAP_Serial_Number " +
                            "FROM CARD_TYPE_SAP " +
                            "WHERE RST = 'A' "+
                        "UNION "+
                        "SELECT DISTINCT SAP_ID "+
                            "FROM PERSO_PROJECT_SAP "+
                            "WHERE RST = 'A' "+
                        "UNION "+
                        "SELECT DISTINCT SAP_ID "+
                            "FROM MATERIEL_SAP "+
                            "WHERE RST = 'A') A "+
                     "WHERE  SAP_Serial_Number = @sap_serial_number";

    public const string SEL_ALL_CHECK_SERIAL_NUMBER = "SELECT DISTINCT Check_Serial_Number " +
                        "FROM PERSO_PROJECT_SAP " +
                        "WHERE RST = 'A' ";
    public const string UPDATE_CARD_TYPE_SAP_DETAIL = "UPDATE CARD_TYPE_SAP_DETAIL SET " +
                        "Unit_Price=@unit_price,Real_Ask_Number=@real_ask_number,"+
                        //200909IR 修改已存在的SAP單據時需要修改未稅單價 Add by YangKun 2009/09/24 start
                        " Unit_Price_NO=@unit_price_no, "+
                        //200909IR 修改已存在的SAP單據時需要修改未稅單價 Add by YangKun 2009/09/24 end
                        "Comment=@comment,SAP_RID=@sap_rid,Check_Serial_Number=@check_serial_number " +
                        "WHERE RID = @rid";
    public const string UPDATE_CARD_TYPE_SAP_DETAIL1 = "UPDATE CARD_TYPE_SAP_DETAIL SET " +
                        "Unit_Price=@unit_price,Real_Ask_Number=@real_ask_number," +
        // add chaoma by 201005515-0 start
                        //"Unit_Price_No=@unit_price_no,Comment=@comment " +
                        "Unit_Price_No=@unit_price_no,Comment=@comment,Check_Serial_Number=@Check_Serial_Number " +
        // add chaoma end
                        "WHERE RID = @rid";
    public const string UPDATE_CARD_TYPE_SAP = "UPDATE CARD_TYPE_SAP SET " +
        // add chaoma by 201005515-0 start
                        //"Fine = @fine,Pass_Status = @pass_status,Comment = @comment " +
                        "Fine = @fine,Pass_Status = @pass_status,Comment = @comment,SAP_Serial_Number = @SAP_Serial_Number,Ask_Date = @Ask_Date " +
        // add chaoma end
                        "WHERE RID = @rid";

    public const string UPDATE_DEPOSITORY_STOCK = "UPDATE DEPOSITORY_STOCK SET Is_AskFinance = 'Y' WHERE RID = @rid ";
    public const string UPDATE_DEPOSITORY_RESTOCK = "UPDATE DEPOSITORY_RESTOCK SET Is_AskFinance = 'Y' WHERE RID = @rid ";
    public const string UPDATE_DEPOSITORY_CANCEL = "UPDATE DEPOSITORY_CANCEL SET Is_AskFinance = 'Y' WHERE RID = @rid ";

    public const string SEL_CARDGROUP_BY_CARDTYPE = "SELECT GCT.Group_RID FROM " +
                    "GROUP_CARD_TYPE GCT INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.GROUP_RID = CG.RID AND CG.Param_Code = '" + GlobalString.Parameter.Finance + "'"+
                    "WHERE GCT.CardType_RID = @cardtype_rid ";

    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Finance0011BL()
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
        DataSet dstFactory = null;

        try
        {
            dstFactory = dao.GetList(SEL_PERSON);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstFactory;
    }

    /// <summary>
    /// 通過卡種RID,取卡種的帳務群組RID
    /// </summary>
    /// <param name="intCardTypeRID"></param>
    /// <returns></returns>
    public string getCardGroupRIDByCardTypeRID(string CardTypeRID)
    {
        string strCardGroupRID = "";
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("cardtype_rid", CardTypeRID);
            DataSet dsCardGroupRID = dao.GetList(SEL_CARDGROUP_BY_CARDTYPE,this.dirValues);

            if (null != dsCardGroupRID && 
                    dsCardGroupRID.Tables.Count > 0 && 
                    dsCardGroupRID.Tables[0].Rows.Count > 0)
            {
                strCardGroupRID = dsCardGroupRID.Tables[0].Rows[0]["Group_RID"].ToString();
            }

            return strCardGroupRID;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 查詢入庫再入庫退貨記錄及拆單記錄
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[拆單作業]</returns>
    public DataTable SearchDepositorySplitRecord(Dictionary<string, object> searchInput, 
                        string firstRowNumber, 
                        string lastRowNumber, 
                        string sortField, 
                        string sortType, 
                        out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Income_Date desc,NAME,Factory_ShortName_CN" : sortField);//默認的排序欄位

        sortType = (sortField == "null" ? "desc " : sortType);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_DEPOSITORY_SPLIT_RECORD);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                dirValues.Add("BGDATE", DateTime.Parse(searchInput["txtBegin_Date"].ToString()).ToString("yyyy/MM/dd 00:00:00"));
            }
            else
            {
                dirValues.Add("BGDATE", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                dirValues.Add("ENDDATE", DateTime.Parse(searchInput["txtFinish_Date"].ToString()).ToString("yyyy/MM/dd 23:59:59"));
            }
            else
            {
                dirValues.Add("ENDDATE", "9999/12/31");
            }

            if (!StringUtil.IsEmpty(searchInput["dropCard_Group"].ToString()))
            {
                stbWhere.Append(" AND A.CARDTYPE_RID IN (SELECT CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @group_rid)");
                dirValues.Add("group_rid", searchInput["dropCard_Group"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString()))
            {
                stbWhere.Append(" AND A.Name like @name");
                dirValues.Add("name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["dropBlankFactory"].ToString()))
            {
                stbWhere.Append(" AND A.FRID = @blank_factory_rid ");
                dirValues.Add("blank_factory_rid", searchInput["dropBlankFactory"].ToString());
            }

            // 請款狀態--未請款
            if (searchInput["dropState"].ToString() == "1")
            {
                stbWhere.Append(" and (sap_id='' or sap_id is null) and Convert(datetime,substring(stock_rid,1,8))>='2008-09-01' ");
            // 請款狀態--已請款
            }else if (searchInput["dropState"].ToString() == "2")
            {
                stbWhere.Append(" and (sap_id <>'' or Convert(datetime,substring(stock_rid,1,8))<'2008-09-01') ");
            }

            if (!StringUtil.IsEmpty(searchInput["txtBUDGET_ID"].ToString()))
            {
                stbWhere.Append(" AND A.Budget_ID Like @budget_id");
                dirValues.Add("budget_id", "%" + searchInput["txtBUDGET_ID"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtAgreement_Code"].ToString()))
            {
                stbWhere.Append(" AND A.Agreement_Code Like @agreement_code");
                dirValues.Add("agreement_code", "%" + searchInput["txtAgreement_Code"].ToString().Trim() + "%");
            }

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

                stbWhere.Append(" AND A.OrderForm_Detail_RID Like @Stock_RID");
                dirValues.Add("Stock_RID", strStock_RID + "%");
            }
        }

        //執行SQL語句
        DataTable dtDepository = null;
        try
        {
            dtDepository = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount).Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;

        // 請款狀態--未請款
        if (searchInput["dropState"].ToString() == "1")
        {
            for (int i = 0; i < dtDepository.Rows.Count; i++)
            {
                DataRow tempRows = dtDepository.Rows[i];
                if (tempRows["Budget_ID"].ToString().Trim() == "舊系統預算" && tempRows["Agreement_Code"].ToString().Trim() == "舊系統合約")
                {
                    dtDepository.Rows.Remove(tempRows);
                    i--;
                }
                    
            }
        }
        return dtDepository;
    }

    /// <summary>
    /// 檢查拆分後的每一筆退貨記錄，
    /// 必須至少有一筆入庫拆分後的單價和之相同，
    /// 且數量大於退貨拆分數量
    /// </summary>
    /// <param name="dtSplitWorkNew"></param>
    /// <returns></returns>
    public bool CheckSplitCancel(DataTable dtSplitWorkNew)
    {
        DataSet dsSAP_DETAIL_CANCEL = null;
        string strStock_RID = "";
        // 復製一個拆單訊息記錄
        DataTable dtSplitWorkCheck = dtSplitWorkNew.Copy();
        foreach (DataRow dr1 in dtSplitWorkCheck.Rows)
        {
            // 退貨時
            if (dr1["Stock_RID"].ToString()!="" && 
                dr1["Operate_Type"].ToString() == "3")
            {
                strStock_RID = dr1["Stock_RID"].ToString();
                string strRIDDS = "0";// 入庫
                string strRIDDR = "0";// 再入庫
                string strRIDDC = "0";// 退貨
                string strRIDCTDS = "0";// 拆分記錄
               // DataRow[] dtRows = dtSplitWorkCheck.Select("Stock_RID = " + dr1["Stock_RID"].ToString());
                DataRow[] dtRows = dtSplitWorkCheck.Select("Stock_RID = " + "'" + strStock_RID + "'");
                foreach (DataRow dr2 in dtRows)
                {
                    // 入庫、再入庫、退貨
                    if (dr2["RID"].ToString() == "0")
                    {
                        if (dr2["Operate_Type"].ToString() == "1")
                        {
                            strRIDDS += "," + dr2["Operate_RID"].ToString();
                        }
                        else if (dr2["Operate_Type"].ToString() == "2")
                        {
                            strRIDDR += "," + dr2["Operate_RID"].ToString();
                        }
                        else if (dr2["Operate_Type"].ToString() == "3")
                        {
                            strRIDDC += "," + dr2["Operate_RID"].ToString();
                        }
                    }
                    else
                    {
                        strRIDCTDS += "," + dr2["RID"].ToString();
                    }
                    dr2["stock_rid"] = "";// 以免重復計算
                }

                string strCON_SAP_DETAIL_CANCEL = CON_SAP_DETAIL_CANCEL.Replace("(DSRID)", "(" + strRIDDS + ")");
                strCON_SAP_DETAIL_CANCEL = strCON_SAP_DETAIL_CANCEL.Replace("(DRRID)", "(" + strRIDDR + ")");
                strCON_SAP_DETAIL_CANCEL = strCON_SAP_DETAIL_CANCEL.Replace("(DCRID)", "(" + strRIDDC + ")");
                strCON_SAP_DETAIL_CANCEL = strCON_SAP_DETAIL_CANCEL.Replace("(CTSDRID)", "(" + strRIDCTDS + ")");

                // 取資料庫中相同入庫流水編號的入庫\再入庫\退貨\拆分記錄訊息
                dirValues.Clear();
                dirValues.Add("stock_rid", strStock_RID);
                dsSAP_DETAIL_CANCEL = dao.GetList(strCON_SAP_DETAIL_CANCEL, dirValues);
                DataTable dtCompare = new DataTable();
                if (null != dsSAP_DETAIL_CANCEL
                    && dsSAP_DETAIL_CANCEL.Tables.Count > 0)
                {
                    dtCompare = dsSAP_DETAIL_CANCEL.Tables[0];
                }
                // 合并DataTable
                foreach (DataRow drInsert in dtRows)
                {
                    DataRow dtNew = dtCompare.NewRow();
                    dtNew["Income_Number"] = drInsert["Income_Number"].ToString();
                    dtNew["Unit_Price"] = drInsert["Unit_Price"].ToString();
                    dtNew["Operate_Type"] = drInsert["Operate_Type"].ToString();
                    dtCompare.Rows.Add(dtNew);
                }

                foreach (DataRow drCompare in dtCompare.Rows)
                {
                    // 退貨
                    int intCancelNumber=0;
                    int intIncomeNumer=0;
                    if (drCompare["Operate_Type"].ToString() == "3")
                    {
                        // 計算該種單價的入庫和退貨總量
                        foreach (DataRow drSum in dtCompare.Rows)
                        {
                            if (Decimal.Parse(drSum["Unit_Price"].ToString())==
                                    Decimal.Parse(drCompare["Unit_Price"].ToString()))
                            {
                                if (drSum["Operate_Type"].ToString() == "3")
                                {
                                    intCancelNumber += Convert.ToInt32(drSum["Income_Number"].ToString());
                                }
                                else
                                {
                                    intIncomeNumer += Convert.ToInt32(drSum["Income_Number"].ToString());
                                }
                            }
                        }
                    }

                    if (intCancelNumber > intIncomeNumer)
                    {
                        return false;
                    }
                }
            }
            
        }
        return true;
    }

    /// <summary>
    /// 保存拆分訊息
    /// </summary>
    /// <param name="dtSplitWorkNew"></param>
    public void SaveSplit(DataTable dtSplitWorkNew)
    {
        CARD_TYPE_SAP_DETAIL ctsdModel = new CARD_TYPE_SAP_DETAIL();

        try
        {
            // 開始事務
            dao.OpenConnection();
            // 保存拆分訊息
            foreach (DataRow dr in dtSplitWorkNew.Rows)
            {
                // 不是第一次進行拆分，先刪除SAP明細表中的，拆分原記錄。
                if (dr["RID"].ToString() != "0")
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("rid", dr["RID"].ToString());
                    dao.ExecuteNonQuery(DEL_SAP_DETAIL_BY_RID, dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("rid", dr["Operate_RID"].ToString());
                    // 將已經拆分的進貨記錄標記為已經請款
                    if (dr["Operate_Type"].ToString() == "1")
                    {
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_STOCK, this.dirValues);
                    }
                    else if (dr["Operate_Type"].ToString() == "2")
                    {
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_RESTOCK, this.dirValues);
                    }
                    else if (dr["Operate_Type"].ToString() == "3")
                    {
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_CANCEL, this.dirValues);
                    }
                }

                // 添加拆分記錄
                ctsdModel.Operate_RID = Convert.ToInt32(dr["Operate_RID"].ToString());
                ctsdModel.Operate_Type = dr["Operate_Type"].ToString();
                ctsdModel.Comment = dr["Comment"].ToString();
                ctsdModel.Delay_Days = Convert.ToInt32(dr["Delay_Days"].ToString());
                ctsdModel.Real_Ask_Number = Convert.ToInt32(dr["Split_Num"].ToString());
                ctsdModel.Unit_Price = Convert.ToDecimal(dr["Unit_Price"].ToString());
                ctsdModel.Unit_Price_No = Convert.ToDecimal(dr["Unit_Price1"].ToString());
                ctsdModel.SAP_RID = -1;
                dao.Add<CARD_TYPE_SAP_DETAIL>(ctsdModel, "RID");
            } 

            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 添加空白卡請款訊息（SAP單、SAP單明細）
    /// </summary>
    /// <param name="ctsModelF">SAP單訊息</param>
    /// <param name="dtRequisitionWorkF">SAP單明細訊息</param>
    public void SaveSAP(CARD_TYPE_SAP ctsModelF, 
                    DataTable dtRequisitionWorkF)
    {
        CARD_TYPE_SAP_DETAIL ctsdModel = new CARD_TYPE_SAP_DETAIL();

        try
        {
            // 開始事務
            dao.OpenConnection();

            // 添加SAP訊息到資料庫中
            ctsModelF.Ask_Date = DateTime.Now;
            ctsModelF.Is_Finance = GlobalString.YNType.No;
            ctsModelF.Pay_Date = Convert.ToDateTime("1900-01-01");
            int intRID = Convert.ToInt32(dao.AddAndGetID<CARD_TYPE_SAP>(ctsModelF, "RID"));

            // 添加SAP單明細訊息到資料庫中
            for (int intLoop = 0; intLoop < dtRequisitionWorkF.Rows.Count; intLoop++)
            {
                // 直接添加
                if (dtRequisitionWorkF.Rows[intLoop]["RID"].ToString() == "0")
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("rid", dtRequisitionWorkF.Rows[intLoop]["RID"].ToString());
                    // 將已經拆分的進貨記錄標記為已經請款
                    if (dtRequisitionWorkF.Rows[intLoop]["Operate_Type"].ToString() == "1")
                    {
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_STOCK, this.dirValues);
                    }
                    else if (dtRequisitionWorkF.Rows[intLoop]["Operate_Type"].ToString() == "2")
                    {
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_RESTOCK, this.dirValues);
                    }
                    else if (dtRequisitionWorkF.Rows[intLoop]["Operate_Type"].ToString() == "3")
                    {
                        dao.ExecuteNonQuery(UPDATE_DEPOSITORY_CANCEL, this.dirValues);
                    }

                    ctsdModel.Operate_RID = Convert.ToInt32(dtRequisitionWorkF.Rows[intLoop]["Operate_RID"].ToString());
                    ctsdModel.Operate_Type = dtRequisitionWorkF.Rows[intLoop]["Operate_Type"].ToString();
                    ctsdModel.Check_Serial_Number = dtRequisitionWorkF.Rows[intLoop]["Check_ID"].ToString();
                    ctsdModel.Comment = dtRequisitionWorkF.Rows[intLoop]["Comment"].ToString();
                    ctsdModel.Delay_Days = Convert.ToInt32(dtRequisitionWorkF.Rows[intLoop]["Delay_Days"].ToString());
                    ctsdModel.Real_Ask_Number = Convert.ToInt32(dtRequisitionWorkF.Rows[intLoop]["Income_Number1"].ToString());
                    ctsdModel.Unit_Price_No = Convert.ToDecimal(dtRequisitionWorkF.Rows[intLoop]["Unit_Price1"].ToString());
                    ctsdModel.Unit_Price = Convert.ToDecimal(dtRequisitionWorkF.Rows[intLoop]["Unit_Price"].ToString());
                    ctsdModel.RST = GlobalString.RST.ACTIVED;
                    ctsdModel.SAP_RID = intRID;
                    dao.Add<CARD_TYPE_SAP_DETAIL>(ctsdModel, "RID");
                }
                // 修改
                else if (dtRequisitionWorkF.Rows[intLoop]["RID"].ToString() != "")
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("unit_price", dtRequisitionWorkF.Rows[intLoop]["Unit_Price"].ToString());
                    //200909IR 修改已存在的SAP單據時需要修改未稅單價 Add by YangKun 2009/09/24 start
                    this.dirValues.Add("unit_price_no", dtRequisitionWorkF.Rows[intLoop]["Unit_Price1"].ToString());
                    //200909IR 修改已存在的SAP單據時需要修改未稅單價 Add by YangKun 2009/09/24 end
                    this.dirValues.Add("real_ask_number", dtRequisitionWorkF.Rows[intLoop]["Income_Number1"].ToString());
                    this.dirValues.Add("comment", dtRequisitionWorkF.Rows[intLoop]["Comment"].ToString());
                    this.dirValues.Add("sap_rid", intRID.ToString());
                    this.dirValues.Add("check_serial_number", dtRequisitionWorkF.Rows[intLoop]["Check_ID"].ToString());
                    this.dirValues.Add("rid", dtRequisitionWorkF.Rows[intLoop]["RID"].ToString());
                    dao.ExecuteNonQuery(UPDATE_CARD_TYPE_SAP_DETAIL, this.dirValues);
                }
            }

            // 提交事務
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
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Ask_Date,SAP_Serial_Number" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_CARD_TYPE_SAP_DETAIL);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropBlankFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Blank_Factory_RID = @blankfactory");
                dirValues.Add("blankfactory", searchInput["dropBlankFactory"].ToString().Trim());
            }

            if (!StringUtil.IsEmpty(searchInput["txtSAP_Serial_Number"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.SAP_Serial_Number like @sap_serial_number ");
                dirValues.Add("sap_serial_number", "%" + searchInput["txtSAP_Serial_Number"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtInvoiceNumber"].ToString().Trim()))
            {
                stbWhere.Append(" AND A.Check_Serial_Number like @invoicenumber ");
                dirValues.Add("invoicenumber", "%" + searchInput["txtInvoiceNumber"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString()))
            {
                dirValues.Add("Begin_Date", DateTime.Parse(searchInput["txtBegin_Date"].ToString()).ToString("yyyy/MM/dd 00:00:00"));
            }
            else
            {
                dirValues.Add("Begin_Date", "1900/01/01");
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString()))
            {
                dirValues.Add("Finish_Date", DateTime.Parse(searchInput["txtFinish_Date"].ToString()).ToString("yyyy/MM/dd 23:59:59"));
            }
            else
            {
                dirValues.Add("Finish_Date", "9999/12/31");
            }

            if (!StringUtil.IsEmpty(searchInput["dropPass_Status"].ToString().Trim()) && searchInput["dropPass_Status"].ToString() != "0")
            {
                stbWhere.Append(" AND A.Pass_Status = @pass_status ");
                dirValues.Add("pass_status", searchInput["dropPass_Status"].ToString().Trim());
            }

            if (!StringUtil.IsEmpty(searchInput["dropIs_Finance"].ToString().Trim()) && searchInput["dropIs_Finance"].ToString() != "0")
            {
                stbWhere.Append(" AND A.Is_Finance = @is_finance ");
                dirValues.Add("is_finance", searchInput["dropIs_Finance"].ToString().Trim());
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
        return dtSAP;
    }

    /// <summary>
    /// 取請款SAP單的資訊
    /// </summary>
    /// <param name="strSAP_Serial_Number"></param>
    /// <returns></returns>
    public DataTable getSAP(string strSAP_Serial_Number)
    {
        DataSet dsSAP = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("sap_serial_number", strSAP_Serial_Number);
            dsSAP = dao.GetList(SEL_SAP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dsSAP.Tables[0];
    }

    /// <summary>
    /// 取請款SAP詳細單的資訊
    /// </summary>
    /// <param name="strSAP_Serial_Number"></param>
    /// <returns></returns>
    public DataTable getSAPDetail(string strSAP_Serial_Number)
    {
        DataSet dsSAPDetail = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("sap_serial_number", strSAP_Serial_Number);
            dsSAPDetail = dao.GetList(SEL_SAP_DETAIL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        //將罰款金額移至合計行之下 
        // add chaoma by 201005515-0 start
        DataRow drNew1 = dsSAPDetail.Tables[0].NewRow();
        //drNew1["Name"] = "合計";
        drNew1["Name"] = "罰款金額";
        dsSAPDetail.Tables[0].Rows.Add(drNew1);
        DataRow drNew2 = dsSAPDetail.Tables[0].NewRow();
        //drNew2["Name"] = "罰款金額";
        drNew2["Name"] = "合計";
        dsSAPDetail.Tables[0].Rows.Add(drNew2);
        // add chaoma end
        return dsSAPDetail.Tables[0];
    }

    /// <summary>
    /// 保存SAP單訊息，狀態設置為暫存或待放行
    /// </summary>
    /// <param name="dtSplitWorkNew"></param>
    /// <param name="Pass_Status"></param>
    /// <param name="SAP_RID"></param>
    public void UpdateSAP(CARD_TYPE_SAP ctsModel, DataTable dtRequisitionWork)
    {
        try
        {
            // 開始事務
            dao.OpenConnection();

            // 更新請款SAP訊息
            this.dirValues.Clear();
            this.dirValues.Add("fine", ctsModel.Fine);
            this.dirValues.Add("comment", ctsModel.Comment);
            this.dirValues.Add("pass_status", ctsModel.Pass_Status);
            this.dirValues.Add("rid", ctsModel.RID);
            // add chaoma by 201005515-0 start
            this.dirValues.Add("SAP_Serial_Number", ctsModel.SAP_Serial_Number);
            this.dirValues.Add("Ask_Date", ctsModel.Ask_Date);
            // add chaoma end
            dao.ExecuteNonQuery(UPDATE_CARD_TYPE_SAP, this.dirValues);

            // 更新請款SAP明細訊息
            foreach (DataRow drRequisition in dtRequisitionWork.Rows)
            {
                this.dirValues.Clear();
                this.dirValues.Add("unit_price", drRequisition["Unit_Price"].ToString());
                this.dirValues.Add("unit_price_no", drRequisition["Unit_Price_No"].ToString());
                this.dirValues.Add("real_ask_number", drRequisition["Income_Number1"].ToString());
                this.dirValues.Add("comment", drRequisition["Comment"].ToString());
                this.dirValues.Add("rid", drRequisition["RID"].ToString());
                // add chaoma by 201005515-0 start
                this.dirValues.Add("Check_Serial_Number", drRequisition["Check_ID"].ToString());
                // add chaoma end
                dao.ExecuteNonQuery(UPDATE_CARD_TYPE_SAP_DETAIL1, this.dirValues);
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

    /// <summary>
    /// 刪除請款訊息
    /// </summary>
    /// <param name="strSAP_RID"></param>
    public void Delete(string strSAP_RID)
    {
        try
        {
            // 打開事務
            dao.OpenConnection();

            // 刪除請款明細
            dirValues.Clear();
            dirValues.Add("sap_rid", strSAP_RID);
            dao.ExecuteNonQuery(DEL_SAP_DETAIL_BY_SAPRID, this.dirValues);

            // 刪除請款訊息
            dao.ExecuteNonQuery(DEL_SAP, this.dirValues);

            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally {
            dao.CloseConnection();
        }
    }
    
    /// <summary>
    /// 審核請款單
    /// </summary>
    /// <param name="strSAP_RID"></param>
    /// <param name="strPass_Status"></param>
    public void Pass_Untread(int intRid,string Pass_State)
    {
        try
        {
            CARD_TYPE_SAP ctsModel = dao.GetModel<CARD_TYPE_SAP,int>("RID", intRid);
            ctsModel.Pass_Status = Pass_State;
            dao.Update<CARD_TYPE_SAP>(ctsModel, "RID");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    //汇出表格时新增資料到資料庫
    public void ADD_CARD_YEAR_FORCAST_PRINT(DataTable dtSplitWorkNew, string time)
    {
        dao.ExecuteNonQuery("delete RPT_Finance0011Add_2 where TimeMark<" + DateTime.Now.ToString("yyyyMMdd000000"));
        try
        {
            foreach (DataRow dr in dtSplitWorkNew.Rows)
            {
                // add chaoma by 201005515-0 start
                if (dr["Name"].ToString() == "罰款金額")
                //if (dr["Name"].ToString() == "合計")
                // add chaoma end
                {
                    break;
                }
                dirValues.Clear();
                dirValues.Add("name", dr["Name"].ToString().Trim());
                dirValues.Add("factory_shortname_cn", dr["Factory_ShortName_CN"].ToString().Trim());
                dirValues.Add("budget_id", dr["Budget_ID"].ToString().Trim());
                dirValues.Add("agreement_code", dr["Agreement_Code"].ToString().Trim());
                dirValues.Add("stock_rid", dr["Stock_RID"].ToString().Trim());
                dirValues.Add("operate_type", dr["Operate_Type"].ToString().Trim());
                dirValues.Add("income_number", dr["Income_Number"].ToString().Trim());
                dirValues.Add("income_date", Convert.ToDateTime(dr["Income_Date"]));
                dirValues.Add("unit_price", dr["Unit_Price"].ToString().Trim());
                dirValues.Add("unit_price_No", dr["未稅單價"].ToString().Trim());
                dirValues.Add("real_ask_number", dr["實際請款數量"].ToString().Trim());
                dirValues.Add("real_pay_money", dr["含稅總金額"].ToString().Trim());
                dirValues.Add("real_pay_money_no", dr["未稅總金額"].ToString().Trim());
                dirValues.Add("delay_days", dr["遲交天數"].ToString().Trim());
                dirValues.Add("check_serial_number", dr["Check_ID"].ToString().Trim());
                dirValues.Add("comment", dr["Comment"].ToString().Trim());
                dirValues.Add("TimeMark", time);

                // add chaoma by 201005515-0 start
                dirValues.Add("Change_UnitPrice", dr["Change_UnitPrice"].ToString().Trim());

                dirValues.Add("Number", dr["Number"].ToString().Trim());
                // add chaoma end
                dao.ExecuteNonQuery(IN_CARD_MONTH_FORCAST_PRINT, dirValues);
            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
    }


    //汇出表格时新增資料到資料庫
    public void ADD_CARD_YEAR_FORCAST_PRINTS(DataTable dtSplitWorkNew, string time)
    {
        dao.ExecuteNonQuery("delete RPT_Finance0011Add_2 where TimeMark<" + DateTime.Now.ToString("yyyyMMdd000000"));

        try
        {
            foreach (DataRow dr in dtSplitWorkNew.Rows)
            {
                // add chaoma by 201005515-0 start
                if (dr["Name"].ToString() == "罰款金額")
                //if (dr["Name"].ToString() == "合計")
                // add chaoma end
                {
                    break;
                }
                dirValues.Clear();
                if (dr["Operate_Type"].ToString() == "1")
                {

                   // if (dr["Fore_Delivery_Date"].ToString() != "1900/1/1 00:00:00")
                    if (Convert.ToDateTime(dr["Fore_Delivery_Date"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) != "1900/01/01")
                    {
                        TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(dr["InCome_Date"].ToString()).Ticks);
                        TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(dr["Fore_Delivery_Date"].ToString()).Ticks);
                        TimeSpan ts = ts1 - ts2;
                        //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 Start
                        //if (ts.Days < 0)
                        //    dirValues.Add("delay_days", "0");
                        //else
                            dirValues.Add("delay_days", ts.Days.ToString());
                        //* RQ-2010-004324-000 8.空白卡請款-遲繳天數開放負數 Delete by Ge.Song 2010/12/10 End
                    }
                    else
                        dirValues.Add("delay_days", "0");
                }
                else if (dr["Operate_Type"].ToString() == "2" || dr["Operate_Type"].ToString() == "3")
                {
                    dirValues.Add("delay_days", "0");
                }
                else
                    dirValues.Add("delay_days", dr["Delay_Days"].ToString());


                #region Legend 2017/08/16 調整存入報表的值: 因退貨的值是不加  負號的, 存入報表的時候需要有 負號
                string strOperate_Type = dr["Operate_Type"].ToString();
                string strOperate_TypeDB = "";
                string strIncome_Number = dr["Income_Number"].ToString();
                string strIncome_Number1 = dr["Income_Number1"].ToString();
                string strUnit_Price = dr["Unit_Price"].ToString();
                string strUnit_Price1 = dr["Unit_Price1"].ToString();

                strIncome_Number = (strIncome_Number == "" ? "0" : strIncome_Number);
                strIncome_Number1 = (strIncome_Number1 == "" ? "0" : strIncome_Number1);
                strUnit_Price = (strUnit_Price == "" ? "0.0000" : strUnit_Price);
                strUnit_Price1 = (strUnit_Price1 == "" ? "0.0000" : strUnit_Price1);

                // 3: 退貨退貨的值 加 負號的
                if (strOperate_Type == "3")
                {
                    strIncome_Number = "-" + strIncome_Number;
                    strIncome_Number1 = "-" + strIncome_Number1;
                }

                // 1: 入庫; 2: 再入庫; 3: 退貨
                switch(strOperate_Type)
                {
                    case "1":
                        strOperate_TypeDB = "入庫";
                        break;
                    case "2":
                        strOperate_TypeDB = "再入庫";
                        break;
                    case "3":
                        strOperate_TypeDB = "退貨";
                        break;
                    default:
                        strOperate_TypeDB = strOperate_Type;
                        break;
                }

                #endregion

                dirValues.Add("name", dr["Name"].ToString());
                dirValues.Add("factory_shortname_cn", dr["Factory_ShortName_CN"].ToString());
                dirValues.Add("budget_id", dr["Budget_ID"].ToString());
                dirValues.Add("agreement_code", dr["Agreement_Code"].ToString());
                dirValues.Add("stock_rid", dr["Stock_RID"].ToString());
                dirValues.Add("operate_type", strOperate_TypeDB);
                dirValues.Add("income_number", strIncome_Number);
                dirValues.Add("income_date", Convert.ToDateTime(dr["Income_Date"]));
                dirValues.Add("unit_price", strUnit_Price);
                dirValues.Add("unit_price_No", strUnit_Price1);
                dirValues.Add("real_ask_number", strIncome_Number1);
                //dirValues.Add("real_pay_money", Convert.ToDecimal(dr["Unit_Price"]) * Convert.ToDecimal(dr["Income_Number1"]));
                //dirValues.Add("real_pay_money_no",Math.Round(Convert.ToDouble(Convert.ToDecimal(dr["Unit_Price"]) * Convert.ToDecimal(dr["Income_Number1"]))/1.05,4));
                dirValues.Add("real_pay_money", Math.Round(Convert.ToDecimal(strUnit_Price) * Convert.ToDecimal(strIncome_Number1),MidpointRounding.AwayFromZero));
                dirValues.Add("real_pay_money_no", Math.Round(Convert.ToDecimal(strUnit_Price1) * Convert.ToDecimal(strIncome_Number1),MidpointRounding.AwayFromZero));
                
                dirValues.Add("check_serial_number", dr["Check_ID"].ToString());
                dirValues.Add("comment", dr["Comment"].ToString());
                dirValues.Add("TimeMark", time);
                // add chaoma by 201005515-0 start
                dirValues.Add("Change_UnitPrice", dr["Change_UnitPrice"].ToString().Trim());
                dirValues.Add("Number", dr["Number"].ToString().Trim());
                // add chaoma end
                dao.ExecuteNonQuery(IN_CARD_MONTH_FORCAST_PRINT, dirValues);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 取系統中所有發票編號
    /// </summary>
    /// <returns></returns>
    public DataTable getAllCheckSerialNumber()
    {
        try {
            DataSet dsAllCheckSerialNumber = dao.GetList(SEL_ALL_CHECK_SERIAL_NUMBER);
            if (dsAllCheckSerialNumber != null &&
                    dsAllCheckSerialNumber.Tables.Count > 0 &&
                    dsAllCheckSerialNumber.Tables[0].Rows.Count > 0)
            {
                return dsAllCheckSerialNumber.Tables[0];
            }
            return null;
        }catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }
    /// <summary>
    /// 取系統中所有發票編號
    /// </summary>
    /// <returns></returns>
    public bool ConstrainSAP_ID(string SAP_ID)
    {
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("sap_serial_number",SAP_ID);
            DataSet dstConSapID = dao.GetList(CON_SAP,this.dirValues);
            if (dstConSapID != null &&
                    dstConSapID.Tables.Count > 0 &&
                    dstConSapID.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dstConSapID.Tables[0].Rows[0][0].ToString()) == 0)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }
}
