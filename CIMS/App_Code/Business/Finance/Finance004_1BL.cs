//******************************************************************
//*  作    者：lantaosu
//*  功能說明：已入庫未請款明細查詢管理邏輯
//*  創建日期：2008-11-21
//*  修改日期：2008-11-25 12:00
//*  修改記錄：
//*            □2008-11-25
//*              1.創建 蘇斕濤
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
using ControlLibrary;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

/// <summary>
/// Finance004_1 的摘要描述
/// </summary>
public class Finance004_1BL
{
    #region SQL語句
    public const string SEL_COO_BLANK_FACTORY = "SELECT RID, Factory_ShortName_CN "
                                        + "FROM FACTORY "
                                        + "WHERE  RST = 'A'  AND Is_Blank='Y' AND Is_Cooperate='Y'";
    public const string SEL_STOCK_UNPAY_YEAR = "SELECT distinct  LEFT(Date_Time,4) AS Year "
                                        + "FROM STOCK_UNPAY "
                                        + "WHERE  RST = 'A' ORDER BY Year DESC";
    public const string SEL_GROUP_BY_USE = "SELECT RID, Group_Name "
                                        + "FROM CARD_GROUP  "
                                        + "WHERE  RST = 'A'  AND Param_Code = @Param_Code ";
    public const string SEL_ACCOUNT_DAYS = "SELECT Param_Code, Param_Name "
                                        + "FROM PARAM "
                                        + "WHERE  RST = 'A'  AND ParamType_Code='CardCost' ";
    public const string SEL_ALL_USE = "SELECT Param_Code, Param_Name "
                                       + "FROM PARAM "
                                       + "WHERE  RST = 'A'  AND ParamType_Code='use' ";


    public const string SEL_WORK_DATE = "SELECT distinct Date_Time "
                                        + "FROM WORK_DATE "
                                        + "WHERE  RST = 'A'  AND Is_WorkDay='Y' AND (Date_Time BETWEEN @DateFrom  AND @DateTo) " 
                                        + "ORDER BY Date_Time ";

    public const string SEL_STOCKS_BY_DATE = "SELECT COUNT(RID) FROM CARDTYPE_STOCKS " 
                                        +"WHERE  RST = 'A'  AND Stock_Date = @Stock_Date";

    public const string SEL_STOCK_DATE = "SELECT distinct Stock_Date "
                                        + "FROM CARDTYPE_STOCKS "
                                        + "WHERE  RST = 'A'  AND (Stock_Date BETWEEN @DateFrom  AND @DateTo) "
                                        + "ORDER BY Stock_Date ";


    public const string SEL_STOCK_SAP = "SELECT  '1' AS Type,  S.Stock_RID,  S.Space_Short_RID, S.Blank_Factory_RID, S.Income_Number, S.Income_Date,S.Is_AskFinance, "
                                        + "D1.Comment, D1.Real_Ask_Number, D1.Unit_Price_No, D1.Unit_Price, D1.SAP_RID,  "
                                        + "S1.Is_Finance, S1.Pay_Date, S1.Real_Pay_Money, S1.Real_Pay_Money_No, "
                                        + "O1.Unit_Price AS Unit_Price_Order,G1.Group_RID "
                                        + ",CB.Budget_ID,AGM.Agreement_Code "//200908IR
                                        + "FROM DEPOSITORY_STOCK S   "
                                        + "LEFT JOIN CARD_TYPE_SAP_DETAIL D1 ON  D1.Operate_RID = S.RID AND D1.Operate_Type = '1' AND D1.RST = 'A'  "
                                        + "LEFT JOIN CARD_TYPE_SAP S1 ON S1.RID=D1.SAP_RID  AND S1.RST = 'A'  "
                                        + "LEFT JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=S.Space_Short_RID  AND G1.RST = 'A'  "
                                        + "LEFT JOIN ORDER_FORM_DETAIL O1 ON  O1.RST = 'A'  AND O1.OrderForm_Detail_RID=S.OrderForm_Detail_RID "
                                        + "LEFT JOIN CARD_BUDGET CB ON CB.RST = 'A' AND S.Budget_RID = CB.RID "//200908IR
                                        + "LEFT JOIN AGREEMENT AGM ON AGM.RST = 'A' AND S.Agreement_RID = AGM.RID "//200908IR
                                        + "WHERE S.RST = 'A' AND (S.Income_Date <= @DateTo)  AND G1.Group_RID=@Group_RID "
                                        + "AND (S1.Is_Finance='N' OR S1.Is_Finance IS NULL OR (S1.Is_Finance='Y' AND S1.Pay_Date>@DateTo)) ";

    public const string SEL_RESTOCK_SAP = "SELECT   '2' AS Type,   R.Stock_RID, R.Space_Short_RID, R.Blank_Factory_RID, R.Reincome_Number AS Income_Number, R.Reincome_Date AS Income_Date,R.Is_AskFinance, "                                       
                                        + "D2.Comment, D2.Real_Ask_Number, D2.Unit_Price_No, D2.Unit_Price, D2.SAP_RID,  "
                                        + "S2.Is_Finance, S2.Pay_Date, S2.Real_Pay_Money, S2.Real_Pay_Money_No, "
                                        + "O2.Unit_Price AS Unit_Price_Order,G2.Group_RID "
                                        + ",CB.Budget_ID,AGM.Agreement_Code "//200908IR
                                        + "FROM DEPOSITORY_RESTOCK R    "
                                        + "LEFT JOIN CARD_TYPE_SAP_DETAIL D2 ON  D2.Operate_RID = R.RID AND D2.Operate_Type = '2' AND D2.RST = 'A'   "
                                        + "LEFT JOIN CARD_TYPE_SAP S2 ON S2.RID=D2.SAP_RID  AND S2.RST = 'A'  "
                                        + "LEFT JOIN GROUP_CARD_TYPE G2 ON G2.CardType_RID=R.Space_Short_RID  AND G2.RST = 'A'   "
                                        + "LEFT JOIN ORDER_FORM_DETAIL O2 ON  O2.RST = 'A'  AND O2.OrderForm_Detail_RID=R.OrderForm_Detail_RID  "
                                        + "LEFT JOIN CARD_BUDGET CB ON CB.RST = 'A' AND R.Budget_RID = CB.RID "//200908IR
                                        + "LEFT JOIN AGREEMENT AGM ON AGM.RST = 'A' AND R.Agreement_RID = AGM.RID "//200908IR
                                        + "WHERE R.RST = 'A' AND (R.Reincome_Date <= @DateTo)  AND G2.Group_RID=@Group_RID "
                                        + "AND (S2.Is_Finance='N' OR S2.Is_Finance IS NULL OR (S2.Is_Finance='Y' AND S2.Pay_Date>@DateTo)) ";

    public const string SEL_CANCEL_SAP = "SELECT   '3' AS Type,  C.Stock_RID,  C.Space_Short_RID, C.Blank_Factory_RID, C.Cancel_Number AS Income_Number, C.Cancel_Date AS Income_Date, C.Is_AskFinance, "
                                        + "D3.Comment, D3.Real_Ask_Number, D3.Unit_Price_No, D3.Unit_Price,  D3.SAP_RID,    "
                                        + "S3.Is_Finance, S3.Pay_Date, S3.Real_Pay_Money, S3.Real_Pay_Money_No, "
                                        + "O3.Unit_Price AS Unit_Price_Order,G3.Group_RID "
                                        + ",CB.Budget_ID,AGM.Agreement_Code "//200908IR
                                        + "FROM DEPOSITORY_CANCEL C "
                                        + "LEFT JOIN CARD_TYPE_SAP_DETAIL D3 ON  D3.Operate_RID = C.RID AND D3.Operate_Type = '3' AND D3.RST = 'A'   "
                                        + "LEFT JOIN CARD_TYPE_SAP S3 ON S3.RID=D3.SAP_RID  AND S3.RST = 'A'   "
                                        + "LEFT JOIN GROUP_CARD_TYPE G3 ON G3.CardType_RID=C.Space_Short_RID AND G3.RST = 'A'   "
                                        + "LEFT JOIN ORDER_FORM_DETAIL O3 ON  O3.RST = 'A'  AND  O3.OrderForm_Detail_RID=C.OrderForm_Detail_RID "
                                        + "LEFT JOIN CARD_BUDGET CB ON CB.RST = 'A' AND C.Budget_RID = CB.RID "//200908IR
                                        + "LEFT JOIN AGREEMENT AGM ON AGM.RST = 'A' AND C.Agreement_RID = AGM.RID "//200908IR
                                        + "WHERE C.RST = 'A' AND (C.Cancel_Date <= @DateTo)  AND G3.Group_RID=@Group_RID "
                                        + "AND (S3.Is_Finance='N' OR S3.Is_Finance IS NULL OR (S3.Is_Finance='Y' AND S3.Pay_Date>@DateTo)) ";


    public const string SEL_STOCK_ACCOUNT = "SELECT  Type, Stock_RID,  Space_Short_RID, Blank_Factory_RID, Income_Number, Income_Date,  "
                                        + "Comment, Real_Ask_Number, Unit_Price_No, Unit_Price, SAP_RID,  "
                                        //+ "Is_Finance, Pay_Date, Real_Pay_Money, Real_Pay_Money_No,Is_AskFinance, Unit_Price_Order  FROM ( ";
                                        + "Is_Finance, Pay_Date, Real_Pay_Money, Real_Pay_Money_No,Is_AskFinance, Unit_Price_Order ,Budget_ID,Agreement_Code  FROM ( ";
    public const string SEL_CARD_NAME = "SELECT Name  "
                                        + "FROM CARD_TYPE  "
                                        + "WHERE RST = 'A'  AND RID=@RID ";
    public const string SEL_FACTORY_SHORT_CNAME = "SELECT  Factory_ShortName_CN  "
                                       + "FROM FACTORY  "
                                       + "WHERE RST = 'A'  AND RID = @RID ";
    public const string SEL_STOCK_UNPAY_TNUMBER = "SELECT T_Number  "
                                       + "FROM STOCK_UNPAY  "
                                       + "WHERE Date_Time=@DateTime AND Group_RID=@Group_RID  AND RST = 'A'   AND Blank_Factory_RID=@factory_rid";

    public const string SEL_SUM_PAY_MONEY = "SELECT isNull(SUM(Real_Pay_Money),0) AS Sum_Money, isNull(SUM(Real_Pay_Money_No),0) AS Sum_Money_No, isnull(SUM(Fine),0) AS Sum_Fine "
                                        + "FROM (SELECT DISTINCT S.RID, S.Real_Pay_Money, S.Real_Pay_Money_No, S.Fine "
                                        + " FROM CARD_TYPE_SAP AS S "
                                        + " LEFT OUTER JOIN   CARD_TYPE_SAP_DETAIL AS D ON D.RST = 'A' AND D.SAP_RID = S.RID "
                                        + " LEFT OUTER JOIN   DEPOSITORY_STOCK AS DS ON DS.RST = 'A' AND DS.RID = D.Operate_RID AND D.Operate_Type = '1' "
                                        + " LEFT OUTER JOIN   DEPOSITORY_RESTOCK AS DR ON DR.RST = 'A' AND DR.RID = D.Operate_RID AND D.Operate_Type = '2' "
                                        + " LEFT OUTER JOIN   DEPOSITORY_CANCEL AS DC ON DC.RST = 'A' AND DC.RID = D.Operate_RID AND D.Operate_Type = '3' "
                                        + " WHERE (S.RST = 'A') AND (S.Is_Finance = 'Y') AND (S.Pay_Date BETWEEN @DateFrom AND @DateTo) "
                                        + " AND ( DS.Space_Short_RID IN "
                                        + " (SELECT G1.CardType_RID FROM GROUP_CARD_TYPE AS G1 WHERE G1.RST='A'  AND G1.Group_RID = @Group_RID) "
                                        + " OR DR.Space_Short_RID IN "
                                        + " (SELECT G2.CardType_RID FROM GROUP_CARD_TYPE AS G2 WHERE G2.RST='A'  AND G2.Group_RID = @Group_RID) "
                                        + " OR DC.Space_Short_RID IN "
                                        + " (SELECT G3.CardType_RID FROM GROUP_CARD_TYPE AS G3 WHERE G3.RST='A'  AND G3.Group_RID = @Group_RID) "
                                        + " ) ) AS tmpTable1";

        //"SELECT SUM(Real_Pay_Money) AS Sum_Money, SUM(Real_Pay_Money_No) AS Sum_Money_No  "
                                       //+ "FROM CARD_TYPE_SAP  "
                                       //+ "WHERE RST = 'A'  AND  Is_Finance='Y' AND (Pay_Date BETWEEN @DateFrom AND @DateTo) ";




    //public const string SEL_SUM_STOCK_MONEY = "select isnull(sum(round(a.Income_Number*b.unit_Price,0)),0) from DEPOSITORY_STOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
    //                                    + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
    //                                    + " WHERE (a.Income_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID ";
    //public const string SEL_SUM_STOCK_MONEY_NO = "select isnull(sum(round(a.Income_Number*round(b.unit_Price/1.05,4),0)),0) from DEPOSITORY_STOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
    //                                  + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
    //                                  + " WHERE (a.Income_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID ";

    //public const string SEL_SUM_RESTOCK_MONEY = "select isnull(sum(round(a.Reincome_Number*b.unit_Price,0)),0) from DEPOSITORY_RESTOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
    //                                    + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
    //                                    + " WHERE (a.Reincome_date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID ";
    //public const string SEL_SUM_RESTOCK_MONEY_NO = "select isnull(sum(round(a.Reincome_Number*round(b.unit_Price/1.05,4),0)),0) from DEPOSITORY_RESTOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
    //                                  + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
    //                                  + " WHERE (a.Reincome_date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID ";

    //public const string SEL_SUM_CANCEL_MONEY = "select isnull(sum(round(a.Cancel_Number*b.unit_Price,0)),0) from DEPOSITORY_CANCEL a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
    //                                    + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
    //                                    + " WHERE (a.Cancel_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID ";
    //public const string SEL_SUM_CANCEL_MONEY_NO = "select isnull(sum(round(a.Cancel_Number*round(b.unit_Price/1.05,4),0)),0) from DEPOSITORY_CANCEL a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
    //                                   + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
    //                                   + " WHERE (a.Cancel_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID ";

    #region 200908IR 從SAP單據表中取單價
    public const string SEL_SUM_STOCK_MONEY = " select sum(A.num) from ( "
                                       + "select isnull(sum(round(a.Income_Number*b.unit_Price,0)),0) as num from DEPOSITORY_STOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
                                       + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                       + " WHERE (a.Income_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID "
                                        + " and a.RID not in ( SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND  SAP_RID<>0 AND Operate_Type = '1' )"
                                        + " union all"
                                        + " select isnull(sum(round(a.Income_Number*b.unit_Price,0)),0) as num  from DEPOSITORY_STOCK a inner join CARD_TYPE_SAP_DETAIL b on a.RID=b.Operate_RID  AND b.Operate_Type = '1' "
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Income_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID and b.SAP_RID<>0  "
                                         + " ) A";


    public const string SEL_SUM_STOCK_MONEY_NO = " select sum(A.num) from ( "
                                       + "select isnull(sum(round(a.Income_Number*round(b.unit_Price/1.05,4),0)),0) as num  from DEPOSITORY_STOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
                                      + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                      + " WHERE (a.Income_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID "
                                        + " and a.RID not in ( SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND  SAP_RID<>0  AND Operate_Type = '1' )"
                                        + " union all"
                                        + " select isnull(sum(round(a.Income_Number*round(b.unit_Price/1.05,4),0)),0) as num  from DEPOSITORY_STOCK a inner join CARD_TYPE_SAP_DETAIL b on a.RID=b.Operate_RID AND b.Operate_Type = '1' "
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Income_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID and b.SAP_RID<>0  "
                                        + " ) A";

    public const string SEL_SUM_RESTOCK_MONEY = " select sum(A.num) from ( "
                                       + "select isnull(sum(round(a.Reincome_Number*b.unit_Price,0)),0) as num  from DEPOSITORY_RESTOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Reincome_date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID "
                                        + " and a.RID not in ( SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND  SAP_RID<>0  AND Operate_Type = '2' )"
                                        + " union all"
                                        + " select isnull(sum(round(a.Reincome_Number*b.unit_Price,0)),0) as num  from DEPOSITORY_RESTOCK a inner join CARD_TYPE_SAP_DETAIL b on a.RID=b.Operate_RID  AND b.Operate_Type = '2'"
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Reincome_date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID and b.SAP_RID<>0 "
                                          + " ) A";

    public const string SEL_SUM_RESTOCK_MONEY_NO = " select sum(A.num) from ( "
                                       + "select isnull(sum(round(a.Reincome_Number*round(b.unit_Price/1.05,4),0)),0) as num  from DEPOSITORY_RESTOCK a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
                                      + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                      + " WHERE (a.Reincome_date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID "
                                       + " and a.RID not in ( SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND  SAP_RID<>0  AND Operate_Type = '2' )"
                                      + " union all"
                                        + " select isnull(sum(round(a.Reincome_Number*round(b.unit_Price/1.05,4),0)),0) as num from DEPOSITORY_RESTOCK a inner join CARD_TYPE_SAP_DETAIL b on a.RID=b.Operate_RID  AND b.Operate_Type = '2'"
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Reincome_date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID and b.SAP_RID<>0 "
                                          + " ) A";

    public const string SEL_SUM_CANCEL_MONEY = " select sum(A.num) from ( "
                                       + "select isnull(sum(round(a.Cancel_Number*b.unit_Price,0)),0) as num  from DEPOSITORY_CANCEL a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Cancel_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID "
                                        + " and a.RID not in ( SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND  SAP_RID<>0   AND Operate_Type = '3' )"
                                        + " union all"
                                        + " select isnull(sum(round(a.Cancel_Number*b.unit_Price,0)),0) as num  from DEPOSITORY_CANCEL a inner join CARD_TYPE_SAP_DETAIL b on a.RID=b.Operate_RID  AND b.Operate_Type = '3'"
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Cancel_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID and b.SAP_RID<>0 "
                                           + " ) A";

    public const string SEL_SUM_CANCEL_MONEY_NO = " select sum(A.num) from ( "
                                       + "select isnull(sum(round(a.Cancel_Number*round(b.unit_Price/1.05,4),0)),0) as num  from DEPOSITORY_CANCEL a inner join ORDER_FORM_DETAIL b on a.OrderForm_Detail_RID=b.OrderForm_Detail_RID"
                                       + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                       + " WHERE (a.Cancel_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID "
                                       + " and a.RID not in ( SELECT Operate_RID FROM CARD_TYPE_SAP_DETAIL WHERE RST = 'A' AND  SAP_RID<>0   AND Operate_Type = '3' )"
                                        + " union all"
                                        + " select isnull(sum(round(a.Cancel_Number*round(b.unit_Price/1.05,4),0)),0) as num  from DEPOSITORY_CANCEL a inner join CARD_TYPE_SAP_DETAIL b on a.RID=b.Operate_RID  AND b.Operate_Type = '3'"
                                        + " inner JOIN GROUP_CARD_TYPE G1 ON G1.CardType_RID=a.Space_Short_RID"
                                        + " WHERE (a.Cancel_Date BETWEEN @DateFrom AND @DateTo)  AND G1.Group_RID=@Group_RID and b.SAP_RID<>0 "
                                        + " ) A";

    #endregion
    public const string SEL_SAP_DETAIL = "SELECT D.Operate_Type, D.Real_Ask_Number, D.Unit_Price_No, D.Unit_Price "
                                        + " FROM CARD_TYPE_SAP_DETAIL AS D "
                                        + " LEFT OUTER JOIN   CARD_TYPE_SAP AS S ON S.RST = 'A ' AND S.RID = D.SAP_RID "
                                        + " LEFT OUTER JOIN   DEPOSITORY_STOCK AS DS ON DS.RST = 'A' AND DS.RID = D.Operate_RID AND D.Operate_Type = '1' "
                                        + " LEFT OUTER JOIN   DEPOSITORY_RESTOCK AS DR ON DR.RST = 'A' AND DR.RID = D.Operate_RID AND D.Operate_Type = '2' "
                                        + " LEFT OUTER JOIN   DEPOSITORY_CANCEL AS DC ON DC.RST = 'A' AND DC.RID = D.Operate_RID AND D.Operate_Type = '3' "
                                        + " WHERE (S.RST = 'A')  AND (S.Pay_Date BETWEEN @DateFrom AND @DateTo) "
                                        + " AND ( DS.Space_Short_RID IN "
                                        + " (SELECT G1.CardType_RID FROM GROUP_CARD_TYPE AS G1 WHERE G1.RST='A'  AND G1.Group_RID = @Group_RID) "
                                        + " OR DR.Space_Short_RID IN "
                                        + " (SELECT G2.CardType_RID FROM GROUP_CARD_TYPE AS G2 WHERE G2.RST='A'  AND G2.Group_RID = @Group_RID) "
                                        + " OR DC.Space_Short_RID IN "
                                        + " (SELECT G3.CardType_RID FROM GROUP_CARD_TYPE AS G3 WHERE G3.RST='A'  AND G3.Group_RID = @Group_RID) "
                                        + " )";

        //"SELECT  D.Operate_Type, D.Real_Ask_Number, D.Unit_Price_No, D.Unit_Price "
                                      // + "FROM CARD_TYPE_SAP S  "
                                      // + "LEFT JOIN CARD_TYPE_SAP_DETAIL D ON D.RST = 'A'  AND  D.SAP_RID=S.RID   "
                                      // + "WHERE S.RST = 'A'  AND  (S.Pay_Date BETWEEN @DateFrom AND @DateTo)  ";

    

    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    
    public Finance004_1BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢所有已建立的合作空白卡廠中文簡稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetCooperateBlankList()
    {
        DataSet dstCooperateBlankList = null;       

        try
        {
            dstCooperateBlankList = dao.GetList(SEL_COO_BLANK_FACTORY);            
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCooperateBlankList;
    }

    /// <summary>
    /// 查找所有群組
    /// </summary>
    /// <returns></returns>
    public DataSet GetGroup()
    {
        DataSet dstGroup = null;

        try
        {
            dstGroup = dao.GetList("select RID from CARD_GROUP where RST='A' ");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstGroup;
    }

    /// <summary>
    /// 查詢已入庫未出帳控管檔，取出其已有資料的年份
    /// </summary>
    /// <param name="move_id"></param>
    /// <returns></returns>
    public DataSet GetYearList()
    {
        DataSet dstYear = null;
        DataSet dstYear1 = null;
        int year;
        try
        {
            dstYear = dao.GetList(SEL_STOCK_UNPAY_YEAR);
            dstYear1 = dstYear.Clone();

            if (dstYear.Tables[0].Rows.Count == 0)
            {
                year = DateTime.Now.Year;
                //DataRow dr1 = dstYear.Tables[0].NewRow();
                //dr1["Year"] = year;
                //dstYear.Tables[0].Rows.Add(dr1);
            }
            else
            {
                year = Convert.ToInt32(dstYear.Tables[0].Rows[0][0].ToString());
            }

            for (int i = 10; i >0; i--)
            {
                DataRow dr = dstYear1.Tables[0].NewRow();
                dr["Year"] = year + i;
                dstYear1.Tables[0].Rows.Add(dr);
            }

            if (dstYear1.Tables[0].Select("Year=" + DateTime.Now.Year.ToString()).Length == 0)
            {
                DataRow dr = dstYear1.Tables[0].NewRow();
                dr["Year"] = DateTime.Now.Year.ToString();
                dstYear1.Tables[0].Rows.Add(dr);
            }

            foreach (DataRow dr1 in dstYear.Tables[0].Rows)
            {
                if (StringUtil.IsEmpty(dr1["Year"].ToString().Trim()) || dr1["Year"].ToString().Trim() == DateTime.Now.Year.ToString())
                    continue;

                DataRow dr2 = dstYear1.Tables[0].NewRow();
                dr2["Year"] = dr1["Year"].ToString();
                dstYear1.Tables[0].Rows.Add(dr2);
            }
            return dstYear1;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢該用途的所有卡穜群組
    /// </summary>    
    /// <returns></returns>
    public DataSet GetGroupList(string Param_Code)
    {
        DataSet dstGroupList = null;        
        try
        {
            dirValues.Clear();
            dirValues.Add("Param_Code", Param_Code);
            dstGroupList = dao.GetList(SEL_GROUP_BY_USE,dirValues);
            return dstGroupList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢系统参数檔PARAM，取得帳務的起、迄日
    /// </summary>    
    /// <returns></returns>
    public DataSet GetAccountDays()
    {
        DataSet dstAccountDays = null;

        try
        {
            dstAccountDays = dao.GetList(SEL_ACCOUNT_DAYS);
            return dstAccountDays;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢營業日期資料檔，找到在此工作日區間内的所有工作日期
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public DataSet GetWorkDateList(string DateFrom,string DateTo)
    {
        DataSet dstWorkDateList = null;
        
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);

        try
        {
            dstWorkDateList = dao.GetList(SEL_WORK_DATE, dirValues);
            return dstWorkDateList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    public bool IsCheckDate(string strDate)
    {

        DataSet dstStock = null;
        
        dirValues.Clear();
        dirValues.Add("Stock_Date", strDate);

        try
        {
            dstStock = dao.GetList(SEL_STOCKS_BY_DATE, dirValues);
            if (dstStock.Tables[0].Rows.Count > 0 && Convert.ToInt16(dstStock.Tables[0].Rows[0][0])>0 )
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢卡種庫存檔，找到在此工作日區間内的所有日期
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public DataSet GetStockDateList(string DateFrom, string DateTo)
    {
        DataSet dstStockDateList = null;

        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);

        try
        {
            dstStockDateList = dao.GetList(SEL_STOCK_DATE, dirValues);
            return dstStockDateList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得該日期區間内的第一個未日結日
    /// </summary>
    /// <param name="StartDate"></param>
    /// <param name="EndDate"></param>
    /// <returns></returns>
    public string GetUncheckDate(string StartDate, string EndDate)
    {
        //查詢卡種庫存檔，找到在此工作日區間内的所有日期
        DataSet StockDateList = this.GetStockDateList(StartDate, EndDate);
        string strFirstCheckDate = "";
        string strLastCheckDate = "";
        int iLength = StockDateList.Tables[0].Rows.Count;
        if (iLength > 0)
        {
            strFirstCheckDate = Convert.ToDateTime(StockDateList.Tables[0].Rows[0]["Stock_Date"].ToString()).ToString("yyyy/MM/dd");
            strLastCheckDate = Convert.ToDateTime(StockDateList.Tables[0].Rows[iLength - 1]["Stock_Date"].ToString()).ToString("yyyy/MM/dd");
        }
        else
        {
            return StartDate;
        }

        //查詢營業日期資料檔，找到在此工作日區間内的所有工作日期
        DataSet WorkDateList = this.GetWorkDateList(StartDate, EndDate);
        for (int i = 0; i < WorkDateList.Tables[0].Rows.Count; i++)
        {
            string strDate = Convert.ToDateTime(WorkDateList.Tables[0].Rows[i]["Date_Time"].ToString()).ToString("yyyy/MM/dd");
            if ((strDate.CompareTo(strFirstCheckDate) >= 0) && (!this.IsCheckDate(strDate)))
            {
                return strDate;
            }
        }

        return "";
    }

    /// <summary>
    /// 查詢參數檔中的所有用途
    /// </summary>    
    /// <returns></returns>
    public DataSet GetUseList()
    {
        DataSet dstUseList = null;
        
        try
        {
            dstUseList = dao.GetList(SEL_ALL_USE);
            return dstUseList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 查詢進貨作業信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List(string RID,string DateFrom,string DateTo,string Group_RID, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Income_Date" : sortField);//默認的排序欄位
        

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_STOCK_ACCOUNT);
        StringBuilder stbCommand1 = new StringBuilder(SEL_STOCK_SAP);
        StringBuilder stbCommand2 = new StringBuilder(SEL_RESTOCK_SAP);
        StringBuilder stbCommand3 = new StringBuilder(SEL_CANCEL_SAP);

        //整理查詢條件        
        StringBuilder stbWhere1 = new StringBuilder();
        StringBuilder stbWhere2 = new StringBuilder();
        StringBuilder stbWhere3 = new StringBuilder();        

        dirValues.Clear();
        //dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", Group_RID);

        if (Group_RID != "")
        {
            stbWhere1.Append(" AND  G1.Group_RID =@Group_RID  AND S.Blank_Factory_RID IN (" + RID + ")  UNION ");
            stbWhere2.Append(" AND  G2.Group_RID =@Group_RID  AND R.Blank_Factory_RID IN (" + RID + ")  UNION ");
            stbWhere3.Append(" AND  G3.Group_RID =@Group_RID  AND C.Blank_Factory_RID IN (" + RID + ") ) as table1 ");            
        }
        else
        {
            stbWhere1.Append(" AND S.Blank_Factory_RID IN (" + RID + ")  UNION ");
            stbWhere2.Append(" AND R.Blank_Factory_RID IN (" + RID + ")  UNION ");
            stbWhere3.Append(" AND C.Blank_Factory_RID IN (" + RID + ") ) as table1 ");
        }
        
        //執行SQL語句
        DataSet dst = null;
        try
        {
            dst = dao.GetList(stbCommand.ToString() + stbCommand1.ToString() + stbWhere1.ToString() + stbCommand2.ToString() + stbWhere2.ToString() + stbCommand3.ToString() + stbWhere3.ToString() + " where Convert(datetime,substring(stock_rid,1,8))>='2008-09-01'  and Unit_Price_Order is not null", dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);// and (SAP_RID='' or SAP_RID=0 or SAP_RID is null)
            for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                {
                    DataRow tempRows = dst.Tables[0].Rows[i];
                    if (tempRows["Budget_ID"].ToString().Trim() == "舊系統預算" && tempRows["Agreement_Code"].ToString().Trim() == "舊系統合約")
                    {
                        dst.Tables[0].Rows.Remove(tempRows);
                        i--;
                    }
                }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dst;
    }



    /// <summary>
    /// (MM)月底提列金額
    /// </summary>
    /// <param name="Date_Time"></param>
    /// <returns></returns>
    public DataSet GetStockUnpayTNumber(string Date_Time,string strGroup_RID,string strFactoryRid)
    {
        DataSet dstStockUnpayTNumber = null;
        if(StringUtil.IsEmpty(strFactoryRid))
            strFactoryRid="0";
        dirValues.Clear();
        dirValues.Add("DateTime", Date_Time);
        dirValues.Add("Group_RID", strGroup_RID);
        dirValues.Add("factory_rid", strFactoryRid);


        try
        {
            dstStockUnpayTNumber = dao.GetList(SEL_STOCK_UNPAY_TNUMBER, dirValues);
            return dstStockUnpayTNumber;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得卡種Space_Short_RID，查詢卡種檔
    /// </summary>
    /// <param name="RID"></param>
    /// <returns></returns>
    public DataSet GetCardName(string RID)
    {
        DataSet dstCardName = null;

        dirValues.Clear();
        dirValues.Add("RID", RID);

        try
        {
            dstCardName = dao.GetList(SEL_CARD_NAME, dirValues);
            return dstCardName;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取得空白廠商Blank_Factory_RID，查詢廠商資料
    /// </summary>
    /// <param name="RID"></param>
    /// <returns></returns>
    public DataSet GetFactoryShortCName(string RID)
    {
        DataSet dstFactoryShortCName = null;

        dirValues.Clear();
        dirValues.Add("RID", RID);

        try
        {
            dstFactoryShortCName = dao.GetList(SEL_FACTORY_SHORT_CNAME, dirValues);
            return dstFactoryShortCName;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="P_Number"></param>
    /// <param name="U_Number"></param>
    /// <param name="D_Number"></param>
    /// <param name="T_Number"></param>
    public void Add(decimal P_Number, decimal U_Number, decimal D_Number, decimal T_Number, string Group_RID, string DateTime, string Blank_Factory_RID)
    {
        STOCK_UNPAY suModel = new STOCK_UNPAY();
        try
        {
            //事務開始
            dao.OpenConnection();

            dao.ExecuteNonQuery("Delete from STOCK_UNPAY where Date_Time=" + DateTime + " and Group_RID=" + Group_RID + " and Blank_Factory_RID=" + Blank_Factory_RID);

            suModel.P_Number = P_Number;
            suModel.U_Number = U_Number;
            suModel.D_Number = D_Number;
            suModel.T_Number = T_Number;
            suModel.Group_RID = Convert.ToInt32(Group_RID);
            suModel.Date_Time = DateTime;
            suModel.Blank_Factory_RID = Convert.ToInt32(Blank_Factory_RID);

            dao.Add<STOCK_UNPAY>(suModel, "RID");
            

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

    /// <summary>
    /// 獲取減(MM)月已出帳付款金額
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public DataSet GetSumPayMoney(string DateFrom, string DateTo,string strGroupRID)
    {
        DataSet dstSumPayMoney = null;

        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSumPayMoney = dao.GetList(SEL_SUM_PAY_MONEY, dirValues);
            return dstSumPayMoney;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 獲取加(MM)月已入庫未請款金額
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public double GetSumStockMoney(string DateFrom, string DateTo, string strGroupRID)
    {
        DataSet dstSumStockMoney = null;
        decimal Sum = 0;
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSumStockMoney = dao.GetList(SEL_SUM_STOCK_MONEY, dirValues);


            return Convert.ToDouble(dstSumStockMoney.Tables[0].Rows[0][0].ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 獲取加(MM)月已入庫未請款金額
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public double GetSumStockMoneyNO(string DateFrom, string DateTo, string strGroupRID)
    {
        DataSet dstSumStockMoney = null;
        decimal Sum = 0;
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSumStockMoney = dao.GetList(SEL_SUM_STOCK_MONEY_NO, dirValues);


            return Convert.ToDouble(dstSumStockMoney.Tables[0].Rows[0][0].ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 獲取加(MM)月已入庫未請款金額
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public double GetSumCancelMoney(string DateFrom, string DateTo, string strGroupRID)
    {
        DataSet dstSumCancelMoney = null;
        decimal Sum = 0;
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSumCancelMoney = dao.GetList(SEL_SUM_CANCEL_MONEY, dirValues);

            return Convert.ToDouble(dstSumCancelMoney.Tables[0].Rows[0][0].ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 獲取加(MM)月已入庫未請款金額
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public double GetSumCancelMoneyNO(string DateFrom, string DateTo, string strGroupRID)
    {
        DataSet dstSumCancelMoney = null;
        decimal Sum = 0;
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSumCancelMoney = dao.GetList(SEL_SUM_CANCEL_MONEY_NO, dirValues);

            return Convert.ToDouble(dstSumCancelMoney.Tables[0].Rows[0][0].ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 獲取加(MM)月已入庫未請款金額
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public double GetSumRestockMoney(string DateFrom, string DateTo, string strGroupRID)
    {
        DataSet dstSumRestockMoney = null;
        decimal Sum = 0;
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSumRestockMoney = dao.GetList(SEL_SUM_RESTOCK_MONEY, dirValues);

            return Convert.ToDouble(dstSumRestockMoney.Tables[0].Rows[0][0].ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 獲取加(MM)月已入庫未請款金額
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public double GetSumRestockMoneyNO(string DateFrom, string DateTo, string strGroupRID)
    {
        DataSet dstSumRestockMoney = null;
        decimal Sum = 0;
        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSumRestockMoney = dao.GetList(SEL_SUM_RESTOCK_MONEY_NO, dirValues);

            return Convert.ToDouble(dstSumRestockMoney.Tables[0].Rows[0][0].ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    public void AddbillCycle(string startDate, string endDate, string year)
    {
        try
        {
            dao.ExecuteNonQuery("delete billcycle where CycleSeq='" + year + "'"
            + " insert into billcycle (CycleSeq,StartDate,EndDate)"
            + " values (" + year + ",'" + startDate + "','" + endDate + "')");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 查詢出帳日在日期期間内的請款SAP單明細，得到SAP單請款明細
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public DataSet GetSapDetail(string DateFrom, string DateTo, string strGroupRID)
    {
        DataSet dstSapDetail = null;

        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);
        dirValues.Add("Group_RID", strGroupRID);

        try
        {
            dstSapDetail = dao.GetList(SEL_SAP_DETAIL, dirValues);
            return dstSapDetail;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    public static string SEL_BillCycle = "select * from BillCycle where CycleSeq='";

    public DataSet GetBillCycle(string year, string month) { 
          DataSet dstAccountDays = null;
          try
          {
              string thisCycleSeq = year + month.PadLeft(2, '0');
              string lastMonthCycle = (Convert.ToDateTime(year + "/" + month+"/01").AddMonths(-1)).ToString("yyyyMM");
              dstAccountDays= dao.GetList(SEL_BillCycle + thisCycleSeq + "' " + SEL_BillCycle
                  + lastMonthCycle+"'");
          }
          catch (Exception ex)
          {
              ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
              throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
          }
          return dstAccountDays;
    }

    /// <summary>
    /// 獲取帳務起迄日
    /// </summary>
    public string[] GetDateFromAndDateTo(string year, string month)
    {
        string StartDate = null;
        string EndDate = null;
        string[] Date = new string[2];
        
        DataSet AccountDays = GetAccountDays();
        if (AccountDays.Tables[0].Rows.Count==0)
        {
            return null;
        }
        DataSet MonthBillCycle = GetBillCycle(year, month);
        if (MonthBillCycle.Tables[0].Rows.Count != 0)
        {
            Date[0] = DateTime.ParseExact(MonthBillCycle.Tables[0].Rows[0]["StartDate"].ToString(),"yyyyMMdd",System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy/MM/dd");
            Date[1] = DateTime.ParseExact(MonthBillCycle.Tables[0].Rows[0]["EndDate"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy/MM/dd");
            return Date;
        }
        else
        {       //取参数表中迄日  
                int EndDay = 0;
                foreach (DataRow dr in AccountDays.Tables[0].Rows)
                {                   
                    if (dr["Param_Code"].ToString().Trim() == "2")
                    {
                        EndDay = Convert.ToInt32(dr["Param_Name"].ToString());
                    }
                }
                EndDate = year + "/" + month + "/" + EndDay;
                //闰年二月最大29号
                if (Convert.ToInt32(year) % 4 == 0)
                {
                    if (month == "2")
                        if (EndDay > 29)
                        {
                            EndDate = year + "/2/29"; 
                        }                   
                }
                //非闰年二月最大28号
                else
                {
                    if (month == "2")
                        if (EndDay > 28)
                        {
                            EndDate = year + "/2/28";
                        }                    
                }
                //小月取30
                if (month == "4" || month == "6" || month == "9" || month == "11")
                        if (EndDay > 30)
                        {
                            EndDate = year + "/" + month + "/30";   
                        }
                //大月取31
                if (month == "1" || month == "3" || month == "5" || month == "7"|| month == "8" || month == "10"|| month == "12")
                        if (EndDay > 31)
                        {
                            EndDate = year + "/" + month + "/31";                               
                        }
                //计算起日，上月帐务区间表有数据，取上月迄日的下一日
               if (MonthBillCycle.Tables[1].Rows.Count != 0)
                {
                    StartDate = DateTime.ParseExact(MonthBillCycle.Tables[1].Rows[0]["EndDate"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture).AddDays(1).ToString("yyyy/MM/dd");
                }  
               else{
               
                 StartDate=Convert.ToDateTime(EndDate).AddMonths(-1).AddDays(1).ToString("yyyy/MM/dd");
               }
               Date[0] = Convert.ToDateTime(StartDate).ToString("yyyy/MM/dd");
               Date[1] = Convert.ToDateTime(EndDate).ToString("yyyy/MM/dd");
               //dao.ExecuteNonQuery( " insert into billcycle (CycleSeq,StartDate,EndDate)"
               //+ " values (" + year + month.PadLeft(2, '0') + ",'" + Convert.ToDateTime(StartDate).ToString("yyyyMMdd") + "','" + Convert.ToDateTime(EndDate).ToString("yyyyMMdd") + "')");
               return Date;
            }
               
    }

}
