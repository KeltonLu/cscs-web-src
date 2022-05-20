//******************************************************************
//*  作    者：JunWang
//*  功能說明：核對庫存作業 
//*  創建日期：2008-11-17
//*  修改日期：2008-11-17 9:00
//*  修改記錄：
//*            □2008-11-17
//*              1.創建 王俊
//*            □2009-09-01
//*              修改 楊昆
//*                      1.比對替換前與替換后的廠商異動信息
//*                      2.日結時對替換前版面的小計檔和替換前版面的廠商異動檔作處理
//*                      3.物料消耗與代制費用用替換前版面的小計檔和替換前版面的廠商異動檔計算 
//*            □2009-11-26
//*              修改 楊昆
//*                      1.日結時計算每日物料庫存結余
//*                      2.物料的消耗數統計算法修改，統計耗用量時不再加上耗損率進行計算
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
using CIMSClass.Business;

/// <summary>
/// InOut003BL 的摘要描述
/// </summary>
public class InOut003BL : BaseLogic
{
    #region SQL語句
    public const string SEL_CARD_TYPE = "SELECT RID,Name FROM CARD_TYPE WHERE RST='A' AND TYPE = @type AND AFFINITY = @affinity AND PHOTO = @photo";

    public const string SEL_CARDTYPE_STATUS = "select Status_Name from dbo.CARDTYPE_STATUS where RID = @rid";

    public const string SEL_LAST_SURPLUS_DAY = "SELECT TOP 1 Stock_Date FROM CARDTYPE_STOCKS WHERE RST = 'A' ORDER BY  Stock_Date DESC";

    public const string SEL_LAST_SURPLUS_DAY_NEXT = "SELECT TOP 1 DATE_TIME FROM WORK_DATE WHERE RST='A' AND Is_WorkDay='Y' AND DATE_TIME > @LastSurplusDate ORDER BY DATE_TIME";

    public const string CON_CHECK_WORKDATE = " SELECT COUNT(*) FROM WORK_DATE WHERE RST = 'A' AND Is_WorkDay='Y' AND Date_Time >= @date_start AND Date_Time <= @date_end ";

    public const string CON_DATE_SURPLUS_CHECK = " SELECT COUNT(*) "+
                        "FROM CARDTYPE_STOCKS "+
                        "WHERE RST = 'A' AND Stock_Date >= @date_start AND Stock_Date <= @date_end ";

    public const string CON_CHECK_DATE_SURPLUS_BEFORE = " SELECT COUNT(*) FROM CARDTYPE_STOCKS WHERE RST = 'A' AND CONVERT(char(10), Stock_Date, 111) = ( SELECT TOP 1 CONVERT(char(10), Date_Time, 111) "+
                        "FROM WORK_DATE WHERE RST = 'A' AND  Is_WorkDay='Y' AND Date_Time < @date ORDER BY Date_Time DESC) ";

    #region 系統應該日結的廠商、卡種
    // 廠商庫存異動、入庫、退貨、再入庫、小計檔、移轉（移入、移出）、上一次庫存量不為0的Perso廠的卡種
    public const string SEL_ALL_SHOULD_SURPLUS_CARDTYPE = "SELECT DISTINCT B.Perso_Factory_RID,F.Factory_ShortName_CN,B.TYPE,B.AFFINITY,B.PHOTO,CT.NAME FROM (SELECT FCI.Perso_Factory_RID,FCI.TYPE,FCI.AFFINITY,FCI.PHOTO " +
                            "FROM FACTORY_CHANGE_IMPORT FCI " +
                            "WHERE FCI.RST = 'A' AND FCI.Date_Time >= @date_time_start AND FCI.Date_Time <= @date_time_end " +
                            "UNION " +
                            " SELECT DS.Perso_Factory_RID,CT.TYPE,CT.AFFINITY,CT.PHOTO " +
                            "FROM DEPOSITORY_STOCK DS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DS.Space_Short_RID = CT.RID " +
                            "WHERE DS.RST = 'A' AND DS.Income_Date >= @date_time_start AND DS.Income_Date <= @date_time_end " +
                            "UNION " +
                            " SELECT DC.Perso_Factory_RID,CT.TYPE,CT.AFFINITY,CT.PHOTO " +
                            "FROM DEPOSITORY_CANCEL DC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DC.Space_Short_RID = CT.RID " +
                            "WHERE DC.RST = 'A' AND DC.Cancel_Date >= @date_time_start AND DC.Cancel_Date <= @date_time_end " +
                            "UNION " +
                            " SELECT DR.Perso_Factory_RID,CT.TYPE,CT.AFFINITY,CT.PHOTO " +
                            "FROM DEPOSITORY_RESTOCK DR INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DR.Space_Short_RID = CT.RID " +
                            "WHERE DR.RST = 'A' AND DR.Reincome_Date >= @date_time_start AND DR.Reincome_Date <= @date_time_end " +
                            "UNION " +
                            " SELECT STI.Perso_Factory_RID,STI.TYPE,STI.AFFINITY,STI.PHOTO " +
                            "FROM SUBTOTAL_IMPORT STI " +
                            "WHERE STI.RST = 'A' AND STI.Date_Time >= @date_time_start AND STI.Date_Time <= @date_time_end " +
                            "UNION " +
                            " SELECT CTSM.From_Factory_RID AS Perso_Factory_RID,CT.TYPE,CT.AFFINITY,CT.PHOTO " +
                            "FROM CARDTYPE_STOCKS_MOVE CTSM INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTSM.CardType_RID = CT.RID " +
                            "WHERE CTSM.RST = 'A' AND CTSM.Move_Date >= @date_time_start AND CTSM.Move_Date<=@date_time_end " +
                            "UNION " +
                            " SELECT CTSM.To_Factory_RID AS Perso_Factory_RID,CT.TYPE,CT.AFFINITY,CT.PHOTO " +
                            "FROM CARDTYPE_STOCKS_MOVE CTSM INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTSM.CardType_RID = CT.RID " +
                            "WHERE CTSM.RST = 'A' AND CTSM.Move_Date >= @date_time_start AND CTSM.Move_Date<=@date_time_end " +
                            "UNION " +
                            " SELECT CS.Perso_Factory_RID,CT.TYPE,CT.AFFINITY,CT.PHOTO " +
                            "FROM CARDTYPE_STOCKS CS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CS.CardType_RID = CT.RID " +
                            "WHERE CS.RST = 'A' AND CS.Stock_Date >= @stock_date_start AND CS.Stock_Date<= @stock_date_end AND CS.Stocks_Number >0) B " +
                            "INNER JOIN Factory F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND B.Perso_Factory_RID = F.RID " +
                            "INNER JOIN Card_Type CT ON CT.RST = 'A' AND B.TYPE = CT.TYPE AND B.AFFINITY = CT.AFFINITY AND B.PHOTO = CT.PHOTO ";
    #endregion 系統應該日結的廠商、卡種

    #region 計算系統的廠商結余,并比較廠商異動資訊和系統異動資訊是否相符。
    //(廠商匯入資訊匯總)//20090909刪掉FCI.RID,
    public const string SEL_FACTORY_IMPORT_STOCKS = " SELECT FCI.Perso_Factory_RID,F.Factory_ShortName_CN,FCI.TYPE,FCI.AFFINITY,FCI.PHOTO,CT.NAME,CS.Status_Name,SUM(Number) as Number " +
                            "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID "+
                            "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND FCI.Perso_Factory_RID = F.RID "+
                            "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO " +
                            "WHERE FCI.RST = 'A' AND FCI.Date_Time >= @date_time_start AND FCI.Date_Time <= @date_time_end " +
                            "GROUP BY FCI.Perso_Factory_RID,F.Factory_ShortName_CN,FCI.TYPE,FCI.AFFINITY,FCI.PHOTO,CT.NAME,CS.Status_Name ";
    //(系統入庫統計)
    public const string SEL_SYS_IN_STOCKS = " SELECT F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME,SUM(Income_Number) as Number " +
                            "FROM DEPOSITORY_STOCK DS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DS.Space_Short_RID = CT.RID "+
                            "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND DS.Perso_Factory_RID = F.RID "+
                            "WHERE DS.RST = 'A' AND DS.Income_Date >= @date_time_start AND DS.Income_Date <= @date_time_end "+
                            "GROUP BY F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME ";
    //(系統退貨統計)
    public const string SEL_SYS_RETURN_STOCKS = " SELECT F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME,SUM(Cancel_Number) as Number " +
                            "FROM DEPOSITORY_CANCEL DC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DC.Space_Short_RID = CT.RID "+
                            "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND DC.Perso_Factory_RID = F.RID "+
                            "WHERE DC.RST = 'A' AND DC.Cancel_Date >= @date_time_start AND DC.Cancel_Date <= @date_time_end "+
                            "GROUP BY F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME ";
    //(系統再入庫統計)
    public const string SEL_SYS_DEPOSITORY_RESTOCK = " SELECT F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME,SUM(Reincome_Number) as Number " +
                            "FROM DEPOSITORY_RESTOCK DR INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DR.Space_Short_RID = CT.RID "+
                            "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND DR.Perso_Factory_RID = F.RID "+
                            "WHERE DR.RST = 'A' AND DR.Reincome_Date >= @date_time_start AND DR.Reincome_Date <= @date_time_end "+
                            "GROUP BY F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME ";
    //(系統3D、DA、PM、RN)
    public const string SEL_SYS_SUBTOTAL_TYPE = " SELECT F.RID,F.Factory_ShortName_CN,STI.TYPE,STI.AFFINITY,STI.PHOTO,MCT.Type_Name,CT.NAME,SUM(Number) as Number " +
                            ", CTO.Type as OLDTYPE , CTO.AFFINITY as OLDAFFINITY , CTO.PHOTO as OLDPHOTO , CTO.NAME as OLDNAME " +
                            "FROM SUBTOTAL_IMPORT STI INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND STI.MakeCardType_RID = MCT.RID AND (MCT.Type_Name = '3D' OR MCT.Type_Name = 'DA' OR MCT.Type_Name = 'PM' OR MCT.Type_Name = 'RN') "+
                            "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND STI.Perso_Factory_RID = F.RID "+
                            " inner join Card_Type CTO on STI.Old_CardType_rid = CTO.rid " + 
                            "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND STI.TYPE = CT.TYPE AND STI.AFFINITY = CT.AFFINITY AND STI.PHOTO = CT.PHOTO " +
                            "WHERE STI.RST = 'A' AND STI.Date_Time >= @date_time_start AND STI.Date_Time <= @date_time_end " +
                            "GROUP BY F.RID,F.Factory_ShortName_CN,STI.TYPE,STI.AFFINITY,STI.PHOTO,MCT.Type_Name,CT.NAME,CTO.TYPE,CTO.AFFINITY,CTO.PHOTO,CTO.NAME ";
    //(移出)
    public const string SEL_SYS_MOVEOUT_STOCKS = " SELECT F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME,SUM(CTSM.Move_Number) as Number " +
                            "FROM CARDTYPE_STOCKS_MOVE CTSM INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTSM.CardType_RID = CT.RID "+
                            "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND CTSM.From_Factory_RID = F.RID "+
                            "WHERE CTSM.RST = 'A' AND CTSM.Move_Date >= @date_time_start AND CTSM.Move_Date<=@date_time_end " +
                            "GROUP BY F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME ";
    //(移入)
    public const string SEL_SYS_MOVEIN_STOCKS = " SELECT F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME,SUM(CTSM.Move_Number) as Number " +
                            "FROM CARDTYPE_STOCKS_MOVE CTSM INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CTSM.CardType_RID = CT.RID "+
                            "INNER JOIN FACTORY F ON F.RST = 'A' AND F.Is_Perso = 'Y' AND CTSM.To_Factory_RID = F.RID "+
                            "WHERE CTSM.RST = 'A' AND CTSM.Move_Date >= @date_time_start AND CTSM.Move_Date<=@date_time_end "+
                            "GROUP BY F.RID,F.Factory_ShortName_CN,CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.NAME ";
    //(Perso廠卡種前天結余)
    //public const string SEL_PERSO_CARDTYPE_BEFORE_DATE_SURPLUS = " SELECT TOP 1 Stocks_Number "+
    //                        "FROM CARDTYPE_STOCKS CS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CS.CardType_RID = CT.RID "+
    //                        "WHERE CS.RST = 'A' AND CS.Perso_Factory_RID = @perso_factory_rid AND CT.TYPE = @type AND CT.AFFINITY = @affinity AND CT.Photo = @photo "+
    //                        "ORDER BY Stock_Date DESC ";
    public const string SEL_PERSO_CARDTYPE_BEFORE_DATE_SURPLUS = " SELECT TOP 1 Stocks_Number " +
                           "FROM CARDTYPE_STOCKS CS INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND CS.CardType_RID = CT.RID " +
                           "WHERE CS.RST = 'A' AND CS.Perso_Factory_RID = @perso_factory_rid AND CT.TYPE = @type AND CT.AFFINITY = @affinity AND CT.Photo = @photo " +
                           " AND Stock_Date =@Stock_Date";
    // 取消耗卡公式
    public const string SEL_EXPRESSIONS_DEFINE = " SELECT Type_RID,CS.Status_Code,CS.Status_Name,ED.Operate " +
                            "FROM EXPRESSIONS_DEFINE ED INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND ED.Type_RID = CS.RID "+
                            "WHERE ED.RST = 'A' AND ED.Expressions_RID = 2 ";
    // 卡種是否為虛擬卡檢查
    public const string CON_CARD_TYPE_GROUP = "SELECT COUNT(*) as Num " +
                            "FROM CARD_TYPE CT INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID " +
                            "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID AND CG.Param_Code = '" + GlobalString.Parameter.Type + "'" +
                            "WHERE CT.RST = 'A' AND CG.Group_Name = '" + GlobalString.Virtual_Card_Group.virtual_card_group + "' " +
                            " AND CT.Type = @type AND CT.Affinity = @affinity AND CT.Photo = @photo ";

    #endregion 計算系統的廠商結余

    #region 進行日結
    public const string SEL_SYS_IN_STOCKS_SURPLUS = " SELECT DS.RID,DS.Perso_Factory_RID,DS.Space_Short_RID,DS.Wafer_RID,DS.Income_Number "+
                        "FROM DEPOSITORY_STOCK DS "+
                        "WHERE DS.RST = 'A' AND DS.Income_Date >= @date_time_start AND DS.Income_Date<= @date_time_end ";

    public const string SEL_SYS_DEPOSITORY_RESTOCK_SURPLUS = " SELECT RID,Perso_Factory_RID,Space_Short_RID,Wafer_RID,Reincome_Number "+
                        "FROM DEPOSITORY_RESTOCK "+
                        "WHERE RST = 'A' AND Reincome_Date >= @date_time_start AND Reincome_Date<= @date_time_end";

    public const string SEL_SYS_RETURN_STOCKS_SURPLUS = " SELECT Stock_RID,Cancel_Number "+
                        "FROM DEPOSITORY_CANCEL "+
                        "WHERE RST = 'A' AND Cancel_Date >= @date_time_start AND Cancel_Date<= @date_time_end";

    public const string SEL_WAFER_CARDTYPE_USELOG_RID = " SELECT RID "+
                        "FROM WAFER_CARDTYPE_USELOG "+
                        "WHERE Operate_Type = '1' AND Operate_RID IN ( SELECT RID FROM DEPOSITORY_STOCK WHERE RST = 'A' AND Stock_RID = @stock_rid) ";

    public const string CON_WAFER_USELOG_ROLLBACK = " SELECT COUNT(*) "+
                        "FROM WAFER_USELOG_ROLLBACK "+
                        "WHERE RST = 'A' AND UseLog_RID = @uselog_rid AND Check_Date >= @check_date_start AND Check_Date <= @check_date_end ";

    public const string INSERT_WAFER_USELOG_ROLLBACK = " INSERT INTO WAFER_USELOG_ROLLBACK (RCU,RUU,RCT,RUT,RST,Income_Date,Usable_Number,Factory_RID,CardType_RID,Begin_Date,End_Date,Wafer_RID,Operate_RID,Operate_Type,UseLog_RID,Check_Date,CardType_Move_RID,Number,BackUp_Date,unit_Price) " +
                        "SELECT '1','1',getdate(),getdate(),'A',Income_Date,Usable_Number,Factory_RID,CardType_RID,Begin_Date,End_Date,Wafer_RID,Operate_RID,Operate_Type,RID,@check_date,CardType_Move_RID,Number,@check_date,unit_Price " +
                            "FROM WAFER_CARDTYPE_USELOG "+
                            "WHERE RID = @uselog_rid ";
    //// by zwl 20090108
    ////public const string UPDATE_WAFER_CARDTYPE_USELOG = " UPDATE WAFER_CARDTYPE_USELOG "+
    ////                    "SET Usable_Number = Usable_Number - @cancel_number,Number = Number - @cancel_number " +
    ////                    "WHERE Operate_Type = '1' AND Operate_RID IN ( SELECT RID FROM DEPOSITORY_STOCK WHERE RST = 'A' AND Stock_RID = @stock_rid) ";
    //退貨時扣減晶片規格變化檔
    public const string UPDATE_WAFER_CARDTYPE_USELOG = " UPDATE WAFER_CARDTYPE_USELOG " +
                        "SET Usable_Number = Usable_Number - @cancel_number,Number = Number - @cancel_number , begin_date = case when year(begin_date) = 1900 then @check_date else begin_date end " +
                        "WHERE RID = @rid ";

    public const string SEL_WAFER_CARDTYPE_USELOG = " SELECT Factory_RID,CardType_RID,RID,Usable_Number,Operate_RID,Operate_Type,Wafer_RID,Begin_Date,Income_Date,Unit_Price " +
                        "FROM WAFER_CARDTYPE_USELOG " +
                        "WHERE RST = 'A' AND Usable_Number>0 " +
                        "ORDER BY Factory_RID,CARDTYPE_RID,Income_Date , Rid ";

    //當由USELOG到ROLLBACK時，不能簡單判斷是否可用數量大於0，第一次為0時，應該要轉到ROLLBACK檔
    public const string SEL_WAFER_CARDTYPE_USELOG_FIRST_ZERO = " SELECT Factory_RID,CardType_RID,RID,Usable_Number,Operate_RID,Operate_Type,Wafer_RID,Begin_Date,Income_Date,Unit_Price     " +
                    "FROM WAFER_CARDTYPE_USELOG U " +
                    "WHERE RST = 'A' AND  not exists ( select * from WAFER_USELOG_ROLLBACK W where w.uselog_rid = U.rid and W.usable_number = 0 )  " +
                    "ORDER BY Factory_RID,CARDTYPE_RID,Income_Date ";


    //public const string SEL_USE_CARDTYPE = " SELECT FCI.Perso_Factory_RID,CT.RID,FCI.Status_RID,SUM(Number) as Number " +
    //                     "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO " +
    //                    "WHERE FCI.RST = 'A' AND Date_Time >= @date_time_start AND Date_Time<= @date_time_end "+
    //                    "GROUP BY FCI.Perso_Factory_RID,CT.RID,FCI.Status_RID ";
    public const string SEL_USE_CARDTYPE = " SELECT A.Perso_Factory_RID,A.RID,A.Status_RID,SUM(A.Number) as Number FROM ( " +
                       " SELECT FCI.Perso_Factory_RID,CT.RID,FCI.Status_RID,SUM(Number) as Number FROM FACTORY_CHANGE_IMPORT FCI " +
                       "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO " +
                      "WHERE FCI.RST = 'A' AND Date_Time >= @date_time_start AND Date_Time<= @date_time_end  and FCI.Status_RID not in ('1','2','3','4')" +
                      "GROUP BY FCI.Perso_Factory_RID,CT.RID,FCI.Status_RID " +
                      "union " +
                        "select SI.Perso_Factory_Rid,CT.RID,CS1.RID as Status_RID,SUM(Number) as Number  from SUBTOTAL_IMPORT  SI " +
                        "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                        "INNER JOIN MAKE_CARD_TYPE M ON M.RST='A' AND M.RID=SI.MakeCardType_RID AND M.Is_Import='Y' " +
                        "INNER JOIN CARDTYPE_STATUS CS1 ON CS1.RST='A' AND CS1.Status_Name=M.Type_Name  " +
                        "where SI.RST = 'A' AND SI.Date_Time >= @date_time_start AND SI.Date_Time<= @date_time_end and M.Type_Name IN ('3D','DA','PM','RN') " +
                        "GROUP BY SI.Perso_Factory_RID,CT.RID,CS1.RID  " +
                        ") A  " +
                        "GROUP BY A.Perso_Factory_RID,A.RID,A.Status_RID "+
                        " order by A.Perso_Factory_RID,A.RID,A.Status_RID " ;

    //庫存轉移若轉移量小於可用量時！
    public const string UPDATE_WAFER_CARDTYPE_USELOG_1 = " UPDATE WAFER_CARDTYPE_USELOG SET Usable_Number = Usable_Number - @number , begin_date = case when year(begin_date) = 1900 then @check_date else begin_date end WHERE RID = @rid ";

    //庫存轉移或轉移量大於可用量時！
    public const string UPDATE_WAFER_CARDTYPE_USELOG_2 = " UPDATE WAFER_CARDTYPE_USELOG SET Usable_Number = 0,End_Date = @check_date  , begin_date = case when year(begin_date) = 1900 then @check_date else begin_date end WHERE RID = @rid ";

    public const string SEL_CARD_TYPE_MOVE_SURPLUS = " SELECT * "+
                        "FROM CARDTYPE_STOCKS_MOVE "+
                        "WHERE RST = 'A' AND Move_Date >= @check_date_start AND Move_Date<=@check_date_end ";

    public const string SEL_MATERIAL_BY_SUBTOTAL = " SELECT EI.Serial_Number AS EI_Number,CE.Serial_Number as CE_Number,A.Perso_Factory_RID,A.Number1 " +
                        "FROM (SELECT SI.Perso_Factory_RID,CT.RID,SUM(Number) AS Number1 " +
                        "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                            "WHERE SI.RST = 'A' AND SI.Date_Time >= @check_date_start AND SI.Date_Time <= @check_date_end " +
                            "GROUP BY SI.Perso_Factory_RID,CT.RID ) A " +
                        "INNER JOIN CARD_TYPE CT1 ON CT1.RST = 'A' AND A.RID = CT1.RID " +
                        "left JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT1.Envelope_RID = EI.RID " +
                        "left JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT1.Exponent_RID = CE.RID ";

    public const string SEL_MATERIAL_BY_SUBTOTAL_DM = " SELECT DI.Serial_Number DI_Number,A.Perso_Factory_RID,A.Number1,DI.Card_Type_Link_Type,DCT.CardType_RID"
                        + " FROM (SELECT SI.Perso_Factory_RID,CT.RID,SUM(Number) AS Number1,si.MakeCardType_rid"
                        + " FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + " WHERE SI.RST = 'A' AND SI.Date_Time >=  @check_date_start "
                        + " AND SI.Date_Time <= @check_date_end "
                        + " GROUP BY SI.Perso_Factory_RID,CT.RID,si.MakeCardType_rid ) A "
                        + " inner join DM_MAKECARDTYPE DMM on DMM.MakeCardType_RID=a.MakeCardType_rid"
                        + " inner join DMTYPE_INFO DI on DI.rid=DMM.DM_RID"
                        + " left join  DM_CARDTYPE DCT ON DCT.RST = 'A' AND A.RID = DCT.CardType_RID and DCT.DM_RID=DI.rid"; 

    public const string SEL_SUBTOTAL_PROJECT_COST = " SELECT SI.Perso_Factory_RID,CT.RID,CG.RID AS CARDGROUPRID,SI.Number "+
                        "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO " +
                        "INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID "+
                        "INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID AND Param_Code = '" + GlobalString.Parameter.Finance + "' "+
                        "WHERE SI.RST = 'A' AND SI.Date_Time >= @check_date_start AND SI.Date_Time <= @check_date_end ";
    //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 start
    public const string SEL_SUBTOTAL_REPLACE_PROJECT_COST = "SELECT Perso_Factory_RID,RID,CARDGROUPRID,sum(Number) as  Number from ( "+
            " SELECT SI.Perso_Factory_RID,CT.RID,CG.RID AS CARDGROUPRID,SI.Number "+
            " FROM SUBTOTAL_REPLACE_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "+
            " INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID "+
            " INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID AND Param_Code = '" + GlobalString.Parameter.Finance + "' " +
            " WHERE SI.RST = 'A' AND SI.Date_Time >= @check_date_start AND SI.Date_Time <= @check_date_end "+
            " union all "+
            " SELECT FCRI.Perso_Factory_RID,CT.RID,CG.RID AS CARDGROUPRID,Case FCRI.Status_RID when '5' then 0-FCRI.Number when '6' then 0-FCRI.Number when '7' then FCRI.Number end as Number "+  
            " FROM FACTORY_CHANGE_REPLACE_IMPORT FCRI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCRI.TYPE = CT.TYPE AND FCRI.AFFINITY = CT.AFFINITY AND FCRI.PHOTO = CT.PHOTO "+
            " INNER JOIN GROUP_CARD_TYPE GCT ON GCT.RST = 'A' AND CT.RID = GCT.CardType_RID "+ 
            " INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND GCT.Group_RID = CG.RID AND Param_Code = '" + GlobalString.Parameter.Finance + "' " +
            " WHERE FCRI.RST = 'A'AND FCRI.Status_RID in ('5','6','7')  AND FCRI.Date_Time >= @check_date_start AND FCRI.Date_Time <= @check_date_end "+
            " ) A "+
            " Group by Perso_Factory_RID,RID,CARDGROUPRID";
    //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 end

    public const string SEL_PROJECT_STEP_SURPLUS = "SELECT PP.RID,PPP.Price " +
                "FROM CARDTYPE_PERSO_PROJECT CPP " +
                "INNER JOIN CARDTYPE_PROJECT_TIME CPT ON CPT.RST  = 'A' AND CPP.ProjectTime_RID = CPT.RID " +
                "INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' AND CPT.PersoProject_RID = PP.RID AND PP.Normal_Special = '1' " +
                "INNER JOIN PERSO_PROJECT_PRICE PPP ON PPP.RST = 'A' AND CPT.PersoProject_RID = PPP.Perso_Project_RID " +
            "WHERE CPP.RST = 'A' AND CPP.CardType_RID = @CTRID " +
                "AND CPT.Use_Date_Begin<=@Date_Time AND CPT.Use_Date_End>=@Date_Time " +
                "AND PPP.Use_Date_Begin<=@Date_Time AND PPP.Use_Date_End>=@Date_Time " +
                "AND PP.Factory_RID = @perso_factory_rid ";

    public const string SEL_SPECIAL_PROJECT_COST = " SELECT SUM(PP.Unit_Price*SPPI.Number) "+
                        "FROM SPECIAL_PERSO_PROJECT_IMPORT SPPI INNER JOIN PERSO_PROJECT PP ON PP.RST = 'A' AND SPPI.PersoProject_RID = PP.RID "+
                        "WHERE SPPI.RST = 'A' AND YEAR(SPPI.Project_Date) = @year ";

    public const string SEL_EXCEPTION_PROJECT_COST = " SELECT CardGroup_RID,SUM(Unit_Price*Number),Group_Name "+
                        "FROM EXCEPTION_PERSO_PROJECT EPP INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND EPP.CardGroup_RID = CG.RID "+
                        "WHERE EPP.RST = 'A' AND YEAR(Project_Date) = @year "+
                        "GROUP BY CardGroup_RID,Group_Name ";

    public const string SEL_PERSO_PROJECT_CHANGE_DETAIL = " SELECT CardGroup_RID,SUM(Price),Group_Name "+
                        "FROM PERSO_PROJECT_CHANGE_DETAIL PPC INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND PPC.CardGroup_RID = CG.RID "+
                        "WHERE PPC.RST = 'A' AND YEAR(Project_Date) = @year "+
                        "GROUP BY CardGroup_RID,Group_Name ";

    public const string SEL_PERSO_PROJECT_NORMAL = " SELECT Card_Group_RID,SUM(Sum),Group_Name "+
                        "FROM PERSO_PROJECT_DETAIL PPD INNER JOIN CARD_GROUP CG ON CG.RST = 'A' AND PPD.Card_Group_RID = CG.RID "+
                        "WHERE YEAR(Use_Date) = @year "+
                        "GROUP BY Card_Group_RID,Group_Name ";
    // 日結時標記
    public const string SEL_MATERIEL_BUDGET_SUM_CARD = " SELECT Budget FROM MATERIEL_BUDGET WHERE RST = 'A' AND Budget_Year = @year AND Materiel_Type = '9' ";

    public const string DEL_MAKE_COST_FROM_SUBTOTAL_IMPORT = "DELETE FROM PERSO_PROJECT_DETAIL " +
                        "WHERE RST = 'A' AND Use_Date>=@Begin_Date AND Use_Date <= @Finish_Date ";

    public const string SEL_MATERIEL_BUDGET_SUM_BANK = " SELECT Budget FROM MATERIEL_BUDGET WHERE RST = 'A' AND Budget_Year = @year AND Materiel_Type = '10' ";

    public const string UPDATE_DEPOSITORY_STOCK = " UPDATE DEPOSITORY_STOCK SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Income_Date >= @check_date_start AND Income_Date <= @check_date_end";

    public const string UPDATE_DEPOSITORY_RESTOCK = " UPDATE DEPOSITORY_RESTOCK SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Reincome_Date >= @check_date_start AND Reincome_Date <= @check_date_end";

    public const string UPDATE_DEPOSITORY_CANCEL = " UPDATE DEPOSITORY_CANCEL SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Cancel_Date >= @check_date_start AND Cancel_Date <= @check_date_end ";

    public const string UPDATE_SUBTOTAL_IMPORT = " UPDATE SUBTOTAL_IMPORT SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";

    public const string UPDATE_FACTORY_CHANGE_IMPORT = " UPDATE FACTORY_CHANGE_IMPORT SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";
    //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 start
    public const string UPDATE_SUBTOTAL_REPLACE_IMPORT = " UPDATE SUBTOTAL_REPLACE_IMPORT SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";
    
    public const string UPDATE_FACTORY_CHANGE_REPLACE_IMPORT = " UPDATE FACTORY_CHANGE_REPLACE_IMPORT SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";
    //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 end
    public const string UPDATE_CARDTYPE_STOCKS_MOVE = " UPDATE CARDTYPE_STOCKS_MOVE SET Is_Check = 'Y',Check_Date = @check_date WHERE RST = 'A' AND Move_Date >= @check_date_start AND Move_Date <= @check_date_end ";

    public const string INSERT_CARDTYPE_STOCKS = " INSERT INTO CARDTYPE_STOCKS (Stock_Date,Stocks_Number,Perso_Factory_RID,CardType_RID) VALUES (@Date_Time,@Number,@Perso_Factory_RID,@RID)";


    public const string SEL_CARDTYPE_STOCKS = " SELECT FCI.Date_Time,FCI.Number,FCI.Perso_Factory_RID,CT.RID " +
                     "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO " +
                    "INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID " +
                        "WHERE FCI.RST  = 'A' AND FCI.Date_Time >= @check_date_start AND FCI.Date_Time <= @check_date_end AND CS.Status_Name = '廠商結餘'";

    public const string SEL_LAST_WORK_DATE = "SELECT TOP 1 Date_Time " +
                    "FROM WORK_DATE " +
                    "WHERE Date_Time < @date_time AND Is_WorkDay='Y' " +
                    "ORDER BY Date_Time DESC";

    public const string SEL_MATERIEL_STOCKS_MANAGER = "SELECT MSM.Perso_Factory_RID,MSM.Serial_Number,MSM.Number," +
                        "CASE SUBSTRING(MSM.Serial_Number,1,1) WHEN 'A' THEN EI.NAME WHEN 'B' THEN CE.NAME WHEN 'C' THEN DI.NAME END AS NAME "+
                    "FROM MATERIEL_STOCKS_MANAGE MSM LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MSM.Serial_Number = EI.Serial_Number " +
                        "LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MSM.Serial_Number = CE.Serial_Number "+
                        "LEFT JOIN DMTYPE_INFO DI ON DI.RST = 'A' AND MSM.Serial_Number = DI.Serial_Number " +
                    "WHERE Convert(varchar(10),Stock_Date,111) = @stock_date AND (Type = 4 or Type = 5) " +
                        "AND MSM.Serial_Number IN (";

    public const string SEL_MATERIEL_USED = "SELECT isnull(sum(Number),0) " +
                    "FROM MATERIEL_STOCKS_USED " +
                    "WHERE RST = 'A' AND Perso_Factory_RID = @perso_factory_rid AND "+
                        "Serial_Number = @serial_number AND " +
                        "Convert(varchar(10),Stock_Date,111) = @stock_date ";

    public const string SEL_MATERIEL_STOCKS_USED = "select * from MATERIEL_STOCKS_USED where rst='A' "
                                       + "AND Serial_Number=@Serial_Number "
                                       + "AND Perso_Factory_RID=@Perso_Factory_RID "
                                       + "AND Stock_Date > @lastSurplusDateTime "
                                       + "AND Stock_Date <= @thisSurplusDateTime";

    #endregion

    #region 取消日結
    public const string CON_DATE_SURPLUS_CHECK_MAX = " SELECT COUNT(*) FROM CARDTYPE_STOCKS WHERE RST = 'A' AND Stock_Date > @date_end ";

    public const string DEL_CARDTYPE_STOCKS = " DELETE FROM CARDTYPE_STOCKS WHERE Stock_Date >= @check_date_start AND Stock_Date <= @check_date_end ";

    public const string DEL_MATERIEL_STOCKS_USED = " Delete From MATERIEL_STOCKS_USED where Stock_Date >= @check_date_start AND Stock_Date <= @check_date_end";
    public const string UPDATE_DEPOSITORY_STOCK_CANCEL = " UPDATE DEPOSITORY_STOCK SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Income_Date >= @check_date_start AND Income_Date <= @check_date_end ";

    public const string UPDATE_DEPOSITORY_RESTOCK_CANCEL = " UPDATE DEPOSITORY_RESTOCK SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Reincome_Date >= @check_date_start AND Reincome_Date <= @check_date_end ";

    public const string UPDATE_DEPOSITORY_CANCEL1 = " UPDATE DEPOSITORY_CANCEL SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Cancel_Date >= @check_date_start AND Cancel_Date <= @check_date_end ";

    public const string UPDATE_SUBTOTAL_IMPORT_CANCEL = " UPDATE SUBTOTAL_IMPORT SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";

    public const string UPDATE_FACTORY_CHANGE_IMPORT_CANCEL = " UPDATE FACTORY_CHANGE_IMPORT SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";
    //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 start
    public const string UPDATE_SUBTOTAL_REPLACE_IMPORT_CANCEL = " UPDATE SUBTOTAL_REPLACE_IMPORT SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";
    
    public const string UPDATE_FACTORY_CHANGE_REPLACE_IMPORT_CANCEL = " UPDATE FACTORY_CHANGE_REPLACE_IMPORT SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Date_Time >= @check_date_start AND Date_Time <= @check_date_end ";
    //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 end

    public const string UPDATE_CARDTYPE_STOCKS_MOVE_CANCEL = " UPDATE CARDTYPE_STOCKS_MOVE SET Is_Check = 'N',Check_Date = '1900-01-01' WHERE RST = 'A' AND Move_Date >= @check_date_start AND Move_Date <= @check_date_end ";

    public const string DEL_WAFER_CARDTYPE_USELOG = " DELETE FROM WAFER_CARDTYPE_USELOG WHERE Income_Date >= @check_date_start AND Income_Date <= @check_date_end ";

    public const string UPDATE_WAFER_CARDTYPE_USELOG_CANCEL = " UPDATE WAFER_CARDTYPE_USELOG SET Usable_Number = WUR.Usable_Number,Begin_Date = WUR.Begin_Date,End_Date = WUR.End_Date " +
                    "FROM WAFER_CARDTYPE_USELOG WCTU INNER JOIN WAFER_USELOG_ROLLBACK WUR ON WCTU.RID = WUR.UseLog_RID " +
                    "WHERE WUR.Check_Date>=@check_date_start AND WUR.Check_Date<=@check_date_end ";

    //入庫日期為當天的，不應該放入ROLLBACK檔，所以所以新增完成後，加一步刪除操作！
    public const string DEL_WAFER_USELOG_ROLLBACK_TODAY = " DELETE FROM WAFER_USELOG_ROLLBACK WHERE income_date = @check_date_start";

    public const string DEL_WAFER_USELOG_ROLLBACK_CANCEL = " DELETE FROM WAFER_USELOG_ROLLBACK WHERE Check_Date >= @check_date_start AND Check_Date <= @check_date_end ";
    #endregion 取消日結

    public const string SEL_BATCH_MANAGE = "SELECT COUNT(*) FROM BATCH_MANAGE WHERE RID = 6 AND Status = 'Y'";
    public  string UPDATE_BATCH_MANAGE_START = "UPDATE BATCH_MANAGE SET Status = 'Y',RUU='InOut003BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE RID = 6 ";
    public  string UPDATE_BATCH_MANAGE_END = "UPDATE BATCH_MANAGE SET Status = 'N',RUU='InOut003BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE RID = 6 ";

    public const string SEL_ENVELOPE_INFO = "SELECT * "
                                   + "FROM ENVELOPE_INFO "
                                   + "WHERE RST = 'A' AND Serial_Number = @serial_number";
    public const string SEL_CARD_EXPONENT = "SELECT * "
                                    + "FROM CARD_EXPONENT "
                                    + "WHERE RST = 'A' AND Serial_Number = @serial_number";
    public const string SEL_DMTYPE_INFO = "SELECT * "
                                    + "FROM DMTYPE_INFO "
                                    + "WHERE RST = 'A' AND Serial_Number = @serial_number";

    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    Depository010BL bl = new Depository010BL();

    public InOut003BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 將日結標設置為FALSE
    /// </summary>
    /// <returns></returns>
    public void DaySurplusEnd()
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
    /// 檢查日結是否已經被開起。如果已經開起，返回FALSE
    ///                         如果沒有開起，開起，并返回TRUE
    /// </summary>
    /// <returns></returns>
    public bool DaySurplusStart()
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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="dtSurplus">日結日期</param>
    /// <param name="dtLastSurplus">上次日結日期</param>
    /// <returns></returns>
    public DataTable getAllShouldSurplusCardType(DateTime dtSurplus,
                                DateTime dtLastSurplus)
    {
        DataTable dtRet = null;
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("date_time_start", dtSurplus.ToString("yyyy/MM/dd 00:00:00"));
            this.dirValues.Add("date_time_end", dtSurplus.ToString("yyyy/MM/dd 23:59:59"));
            this.dirValues.Add("stock_date_start", dtLastSurplus.ToString("yyyy/MM/dd 00:00:00"));
            this.dirValues.Add("stock_date_end", dtLastSurplus.ToString("yyyy/MM/dd 23:59:59"));
            DataSet dsAllShouldSurplusCardType = dao.GetList(SEL_ALL_SHOULD_SURPLUS_CARDTYPE, 
                                        this.dirValues);
            if (dsAllShouldSurplusCardType != null
                && dsAllShouldSurplusCardType.Tables.Count > 0
                && dsAllShouldSurplusCardType.Tables[0].Rows.Count>0)
            {
                dtRet = dsAllShouldSurplusCardType.Tables[0];
            }

            return dtRet;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取最后一次日結日期的下一工作日
    /// </summary>
    /// <param name="dtLastSurplusDate">最后一次日結日期</param>
    /// <returns></returns>
    public DateTime getLastSurplusDateNext(DateTime dtLastSurplusDate)
    {
        DateTime dtWorkDateNext = Convert.ToDateTime("1900-01-01");
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("LastSurplusDate", dtLastSurplusDate);
            DataSet dsLAST_SURPLUS_DAY_NEXT = dao.GetList(SEL_LAST_SURPLUS_DAY_NEXT,this.dirValues);
            if (dsLAST_SURPLUS_DAY_NEXT != null 
                && dsLAST_SURPLUS_DAY_NEXT.Tables.Count > 0
                && dsLAST_SURPLUS_DAY_NEXT.Tables[0].Rows.Count > 0)
            {
                dtWorkDateNext = Convert.ToDateTime(dsLAST_SURPLUS_DAY_NEXT.Tables[0].Rows[0]["DATE_TIME"].ToString());
            }
            return dtWorkDateNext;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 取最后一次日結日期
    /// </summary>
    /// <returns></returns>
    public DateTime getLastSurplusDate()
    {
        DateTime dtLastSurplusDate = Convert.ToDateTime("1900-01-01");
        try
        {
            DataSet dsLAST_SURPLUS_DAY = dao.GetList(SEL_LAST_SURPLUS_DAY);
            if (dsLAST_SURPLUS_DAY != null
                    && dsLAST_SURPLUS_DAY.Tables.Count > 0
                    && dsLAST_SURPLUS_DAY.Tables[0].Rows.Count > 0)
            {
                dtLastSurplusDate = Convert.ToDateTime(dsLAST_SURPLUS_DAY.Tables[0].Rows[0]["Stock_Date"].ToString());
            }
            return dtLastSurplusDate;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 檢查日結日期是否正確,如果不正確，報錯誤訊息，不進行后面的操作
    /// </summary>
    /// <returns></returns>
    public void CheckDateTime(DateTime Date)
    {
        DataSet dsCheckDateTime = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("date", Date);
            dirValues.Add("date_start",Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            dsCheckDateTime = dao.GetList(CON_CHECK_WORKDATE +
                                CON_CHECK_DATE_SURPLUS_BEFORE + 
                                CON_DATE_SURPLUS_CHECK, dirValues);
            if (dsCheckDateTime.Tables.Count >= 3)
            {
                //1.是不是工作日？
                if (dsCheckDateTime.Tables[0].Rows[0][0].ToString() == "0")
                    throw new AlertException("輸入的日結日期不是工作日!");

                //2.之前一工作日是否已日結
                if (dsCheckDateTime.Tables[1].Rows[0][0].ToString() == "0")
                    throw new AlertException("輸入的日結日期之前一工作日未日結!");

                //3.輸入的日期是否已經日結
                if (dsCheckDateTime.Tables[2].Rows[0][0].ToString() != "0")
                    throw new AlertException("輸入的日結日期已日結!");
            }
            else {
                throw new AlertException("輸入的日結日期不正確。");
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

    /// <summary>
    /// 比較廠商庫存異動和系統庫存異動資訊
    /// </summary>
    /// <param name="dsFactory_Stock_Number">廠商庫存異動資訊</param>
    /// <param name="dsSystem_Stock_Number">系統庫存異動資訊</param>
    /// <param name="dtStockDiff">異動資訊不符記錄</param>
    /// <param name="dtStockSurplusDiff">異動資訊匯總不符記錄</param>
    public void CompareFactorySys( DataSet dsFactory_Stock_Number,
                                 DataSet dsSystem_Stock_Number,
                                ref DataTable dtStockDiff,
                                ref DataTable dtStockSurplusDiff)
    {

        DataRow[] drUsed = null;
        DataRow[] drUsedMoveIn = null;
        dtStockDiff = getDTStockDiff();
        dtStockSurplusDiff = getDTStockSurplusDiff();

        #region 取廠商異動資訊不符記錄
        foreach (DataRow drFactoryStockNumber in dsFactory_Stock_Number.Tables[0].Rows)
        {
            string strStatus_Name = drFactoryStockNumber["Status_Name"].ToString();
            if (strStatus_Name == "入庫")
            {
                drUsed = dsSystem_Stock_Number.Tables[0].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " +drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "退貨")
            {
                drUsed = dsSystem_Stock_Number.Tables[1].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " +drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "再入庫")
            {
                drUsed = dsSystem_Stock_Number.Tables[2].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " +drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "3D")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND OLDType = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND OLDAffinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND OLDPhoto = " +drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = '3D'");
            }
            else if (strStatus_Name == "DA")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND OLDType = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND OLDAffinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND OLDPhoto = " +drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = 'DA'");
            }
            else if (strStatus_Name == "PM")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND OLDType = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND OLDAffinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND OLDPhoto = " +drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = 'PM'");
            }
            else if (strStatus_Name == "RN")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND OLDType = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND OLDAffinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND OLDPhoto = " +drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = 'RN'");
            }
            else if (strStatus_Name == "移轉")
            {
                drUsed = dsSystem_Stock_Number.Tables[4].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " +drFactoryStockNumber["Photo"].ToString());
                drUsedMoveIn = dsSystem_Stock_Number.Tables[5].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " +drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "廠商結餘")
            {
                drUsed = dsSystem_Stock_Number.Tables[6].Select("Perso_Factory_RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " +drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " +drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " +drFactoryStockNumber["Photo"].ToString());
            }
            // 檢查數量是否相等
            if (strStatus_Name == "移轉")
            {
                int intMoveNumber = 0;
                if (drUsed != null)
                {
                    if (drUsed.Length > 0)
                    {
                        intMoveNumber -= Convert.ToInt32(drUsed[0]["Number"].ToString());
                    }
                    if (drUsedMoveIn.Length > 0)
                    {
                        intMoveNumber += Convert.ToInt32(drUsedMoveIn[0]["Number"].ToString());
                    }

                    // 檢查移轉數量是否相等
                    if (intMoveNumber != Convert.ToInt32(drFactoryStockNumber["Number"].ToString()))
                    {
                        DataRow drStockDiff = dtStockDiff.NewRow();
                        drStockDiff["Factory_ShortName_CN"] = drFactoryStockNumber["Factory_ShortName_CN"];
                        drStockDiff["Name"] = drFactoryStockNumber["Name"];
                        drStockDiff["Type"] = "移轉";
                        drStockDiff["Factory_Number"] = drFactoryStockNumber["Number"];
                        drStockDiff["System_Number"] = intMoveNumber;
                        if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                        dtStockDiff.Rows.Add(drStockDiff);
                    }
                }
                // 檢查入庫、退貨、再入庫、3D、DA、PM、RN、移轉、本日結余
            }else if (strStatus_Name == "入庫" ||
                strStatus_Name == "退貨" ||
                strStatus_Name == "再入庫" ||
                strStatus_Name == "3D" || 
                strStatus_Name == "DA" ||
                strStatus_Name == "PM" ||
                strStatus_Name == "RN" ||
                strStatus_Name == "廠商結餘")
            {
               int  iSystem_Number = 0;
               if (drUsed != null)
               {
                   if (drUsed.Length > 0)
                   {
                       for (int i = 0; i < drUsed.Length; i++)
                       {
                           iSystem_Number += Convert.ToInt32(drUsed[i]["Number"].ToString());
                       }
                   } 
               }
                
                if (iSystem_Number !=
                    Convert.ToInt32(drFactoryStockNumber["Number"].ToString()))
                {
                    // 廠商庫存異動和系統庫存異動不相符時添加保存該記錄
                    DataRow drStockDiff = dtStockDiff.NewRow();
                    drStockDiff["Factory_ShortName_CN"] = drFactoryStockNumber["Factory_ShortName_CN"];
                    drStockDiff["Name"] = drFactoryStockNumber["Name"];
                    drStockDiff["Type"] = strStatus_Name;
                    drStockDiff["Factory_Number"] = drFactoryStockNumber["Number"];
                    drStockDiff["System_Number"] = iSystem_Number;
                    //if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                    dtStockDiff.Rows.Add(drStockDiff);
                }
                else
                {
                    // 如果結余量為負數。除虛擬卡外也要顯示。
                    if (iSystem_Number < 0 && strStatus_Name == "廠商結餘")
                    {
                        this.dirValues.Clear();
                        this.dirValues.Add("type",drFactoryStockNumber["Type"].ToString());
                        this.dirValues.Add("affinity", drFactoryStockNumber["Affinity"].ToString());
                        this.dirValues.Add("photo", drFactoryStockNumber["Photo"].ToString());
                        DataSet dstVirtualCardGroup = dao.GetList(CON_CARD_TYPE_GROUP,this.dirValues);
                        if (null!=dstVirtualCardGroup && dstVirtualCardGroup.Tables.Count>0 &&
                            dstVirtualCardGroup.Tables[0].Rows.Count>0)
                        {
                            if (Convert.ToInt32(dstVirtualCardGroup.Tables[0].Rows[0]["Num"].ToString()) == 0)
                            {   // 不為虛擬卡
                                // 廠商庫存異動和系統庫存異動不相符時添加保存該記錄
                                DataRow drStockDiff = dtStockDiff.NewRow();
                                drStockDiff["Factory_ShortName_CN"] = drFactoryStockNumber["Factory_ShortName_CN"];
                                drStockDiff["Name"] = drFactoryStockNumber["Name"];
                                drStockDiff["Type"] = strStatus_Name;
                                drStockDiff["Factory_Number"] = drFactoryStockNumber["Number"];
                                drStockDiff["System_Number"] = iSystem_Number;
                                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                                dtStockDiff.Rows.Add(drStockDiff);
                            }
                        }
                    }
                }
            }
        }
        #endregion 取異動資訊不符記錄

        #region 取系統訊息中存在，廠商異動不存在訊息
        // 入庫
        foreach (DataRow drSysStockNumber in dsSystem_Stock_Number.Tables[0].Rows)
        {
            drUsed = dsFactory_Stock_Number.Tables[0].Select("Perso_Factory_RID = " + drSysStockNumber["RID"].ToString() +
                                                        " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drSysStockNumber["Photo"].ToString() +
                                                        " AND Status_Name = '入庫'");
            if (drUsed != null && drUsed.Length==0)
            {
                DataRow drStockDiff = dtStockDiff.NewRow();
                drStockDiff["Factory_ShortName_CN"] = drSysStockNumber["Factory_ShortName_CN"];
                drStockDiff["Name"] = drSysStockNumber["Name"];
                drStockDiff["Type"] = "入庫";
                drStockDiff["Factory_Number"] = 0;
                drStockDiff["System_Number"] = drSysStockNumber["Number"];
                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                dtStockDiff.Rows.Add(drStockDiff);                
            }
        }

        // 退貨
        foreach (DataRow drSysStockNumber in dsSystem_Stock_Number.Tables[1].Rows)
        {
            drUsed = dsFactory_Stock_Number.Tables[0].Select("Perso_Factory_RID = " + drSysStockNumber["RID"].ToString() +
                                                        " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drSysStockNumber["Photo"].ToString() +
                                                        " AND Status_Name = '退貨'");
            if (drUsed != null && drUsed.Length == 0)
            {
                DataRow drStockDiff = dtStockDiff.NewRow();
                drStockDiff["Factory_ShortName_CN"] = drSysStockNumber["Factory_ShortName_CN"];
                drStockDiff["Name"] = drSysStockNumber["Name"];
                drStockDiff["Type"] = "退貨";
                drStockDiff["Factory_Number"] = 0;
                drStockDiff["System_Number"] = drSysStockNumber["Number"];
                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                dtStockDiff.Rows.Add(drStockDiff);
            }
        }

        // 再入庫
        foreach (DataRow drSysStockNumber in dsSystem_Stock_Number.Tables[2].Rows)
        {
            drUsed = dsFactory_Stock_Number.Tables[0].Select("Perso_Factory_RID = " + drSysStockNumber["RID"].ToString() +
                                                        " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drSysStockNumber["Photo"].ToString() +
                                                        " AND Status_Name = '再入庫'");
            if (drUsed != null && drUsed.Length == 0)
            {
                DataRow drStockDiff = dtStockDiff.NewRow();
                drStockDiff["Factory_ShortName_CN"] = drSysStockNumber["Factory_ShortName_CN"];
                drStockDiff["Name"] = drSysStockNumber["Name"];
                drStockDiff["Type"] = "再入庫";
                drStockDiff["Factory_Number"] = 0;
                drStockDiff["System_Number"] = drSysStockNumber["Number"];
                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                dtStockDiff.Rows.Add(drStockDiff);
            }
        }

        // 小計檔
        foreach (DataRow drSysStockNumber in dsSystem_Stock_Number.Tables[3].Rows)
        {
            drUsed = dsFactory_Stock_Number.Tables[0].Select("Perso_Factory_RID = "+drSysStockNumber["RID"].ToString()+
                                            " AND Type = "+drSysStockNumber["OLDType"].ToString()+
                                            " AND Affinity="+drSysStockNumber["OLDAffinity"].ToString()+
                                            " AND Photo = "+drSysStockNumber["OLDPhoto"].ToString()+
                                            " AND Status_Name = '"+drSysStockNumber["Type_Name"].ToString() + "'");
            if (drUsed != null && drUsed.Length == 0)
            {
                DataRow drStockDiff = dtStockDiff.NewRow();
                drStockDiff["Factory_ShortName_CN"] = drSysStockNumber["Factory_ShortName_CN"];
                drStockDiff["Name"] = drSysStockNumber["OLDName"];
                drStockDiff["Type"] = drSysStockNumber["Type_Name"].ToString();
                drStockDiff["Factory_Number"] = 0;
                drStockDiff["System_Number"] = drSysStockNumber["Number"];
                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                dtStockDiff.Rows.Add(drStockDiff);
            }
        }

        // 移轉（轉出）
        foreach (DataRow drSysStockNumber in dsSystem_Stock_Number.Tables[4].Rows)
        {
            drUsed = dsFactory_Stock_Number.Tables[0].Select("Perso_Factory_RID = " + drSysStockNumber["RID"].ToString() +
                                                        " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drSysStockNumber["Photo"].ToString() +
                                                        " AND Status_Name = '移轉'");
            if (drUsed != null && drUsed.Length == 0)
            {
                // 計算系統移轉數量
                int intMove = 0;
                DataRow[] drMoveIns = dsSystem_Stock_Number.Tables[5].Select("RID = " + drSysStockNumber["RID"].ToString() +
                                                       " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                       " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                       " AND Photo = " + drSysStockNumber["Photo"].ToString());
                if (null != drMoveIns && drMoveIns.Length > 0)
                {
                    intMove = Convert.ToInt32(drMoveIns[0]["Number"]) - Convert.ToInt32(drSysStockNumber["Number"]);
                }
                else
                {
                    intMove -= Convert.ToInt32(drSysStockNumber["Number"]);
                }
                    
                DataRow drStockDiff = dtStockDiff.NewRow();
                drStockDiff["Factory_ShortName_CN"] = drSysStockNumber["Factory_ShortName_CN"];
                drStockDiff["Name"] = drSysStockNumber["Name"];
                drStockDiff["Type"] = "移轉";
                drStockDiff["Factory_Number"] = 0;
                drStockDiff["System_Number"] = intMove;
                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                dtStockDiff.Rows.Add(drStockDiff);
            }
        }

        // 移轉（轉入）
        foreach (DataRow drSysStockNumber in dsSystem_Stock_Number.Tables[5].Rows)
        {
            drUsed = dsFactory_Stock_Number.Tables[0].Select("Perso_Factory_RID = " + drSysStockNumber["RID"].ToString() +
                                                        " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drSysStockNumber["Photo"].ToString() +
                                                        " AND Status_Name = '移轉'");
            if (drUsed != null && drUsed.Length == 0)
            {
                // 計算系統移轉數量
                int intMove = 0;
                DataRow[] drMoveOuts = dsSystem_Stock_Number.Tables[4].Select("RID = " + drSysStockNumber["RID"].ToString() +
                                                       " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                       " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                       " AND Photo = " + drSysStockNumber["Photo"].ToString());
                if (null != drMoveOuts && drMoveOuts.Length > 0)
                {
                    // 移出時已經檢查
                }
                else
                {
                    intMove = Convert.ToInt32(drSysStockNumber["Number"]);

                    DataRow drStockDiff = dtStockDiff.NewRow();
                    drStockDiff["Factory_ShortName_CN"] = drSysStockNumber["Factory_ShortName_CN"];
                    drStockDiff["Name"] = drSysStockNumber["Name"];
                    drStockDiff["Type"] = "移轉";
                    drStockDiff["Factory_Number"] = 0;
                    drStockDiff["System_Number"] = intMove;
                    if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                    dtStockDiff.Rows.Add(drStockDiff);
                }
            }
        }

        // 廠商結余
        foreach (DataRow drSysStockNumber in dsSystem_Stock_Number.Tables[6].Rows)
        {
            drUsed = dsFactory_Stock_Number.Tables[0].Select("Perso_Factory_RID = " + drSysStockNumber["Perso_Factory_RID"].ToString() +
                                                    " AND Type = " + drSysStockNumber["Type"].ToString() +
                                                    " AND Affinity = " + drSysStockNumber["Affinity"].ToString() +
                                                    " AND Photo = " + drSysStockNumber["Photo"].ToString() +
                                                    " AND Status_Name = '廠商結餘'");
            if (drUsed != null && drUsed.Length == 0)
            {
                DataRow drStockDiff = dtStockDiff.NewRow();
                drStockDiff["Factory_ShortName_CN"] = drSysStockNumber["Factory_ShortName_CN"];
                drStockDiff["Name"] = drSysStockNumber["Name"];
                drStockDiff["Type"] = "廠商結餘";
                drStockDiff["Factory_Number"] = 0;
                drStockDiff["System_Number"] = drSysStockNumber["Number"];
                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                    dtStockDiff.Rows.Add(drStockDiff);
            }
        }

        #endregion 取系統訊息中存在，廠商異動不存在訊息

        #region 取異動資訊匯總不符記錄
        if (dtStockDiff.Rows.Count > 0)
        {
            // 取廠商結余匯總
            DataTable dtFactorySumSurplus = getFactorySumSurplus(dsFactory_Stock_Number.Tables[0]);
            // 取系統結余匯總
            DataTable dtSysSumSurplus = getFactorySumSurplus(dsSystem_Stock_Number.Tables[6]);

            // 對比兩種匯總
            foreach (DataRow drCompareS in dtFactorySumSurplus.Rows)
            {
                bool blnTag = false;
                foreach (DataRow drCompareD in dtSysSumSurplus.Rows)
                {
                    if (drCompareS["Name"].ToString() == drCompareD["Name"].ToString())
                    {
                        blnTag = true;
                        if (drCompareS["SumNumber"].ToString() != drCompareD["SumNumber"].ToString())
                        {
                            DataRow drNew = dtStockSurplusDiff.NewRow();
                            drNew["Name"] = drCompareS["Name"].ToString();
                            drNew["Factory_Number"] = drCompareS["SumNumber"].ToString();
                            drNew["System_Number"] = drCompareD["SumNumber"].ToString();
                            dtStockSurplusDiff.Rows.Add(drNew);
                        }
                        break;
                    }
                }

                if (!blnTag)
                {
                    DataRow drNew = dtStockSurplusDiff.NewRow();
                    drNew["Name"] = drCompareS["Name"].ToString();
                    drNew["Factory_Number"] = drCompareS["SumNumber"].ToString();
                    drNew["System_Number"] = 0;
                    dtStockSurplusDiff.Rows.Add(drNew);
                }
            }

            foreach (DataRow drCompareD in dtSysSumSurplus.Rows)
            {
                bool blnTag = false;
                foreach (DataRow drCompareS in dtFactorySumSurplus.Rows)
                {
                    if (drCompareS["Name"].ToString() == drCompareD["Name"].ToString())
                    {
                        blnTag = true;
                        break;
                    }
                }
                if (!blnTag)
                {
                    DataRow drNew = dtStockSurplusDiff.NewRow();
                    drNew["Name"] = drCompareD["Name"].ToString();
                    drNew["Factory_Number"] = 0;
                    drNew["System_Number"] = drCompareD["SumNumber"].ToString();
                    dtStockSurplusDiff.Rows.Add(drNew);
                }
            }

        }
        #endregion 取異動資訊匯總不符記錄
    }

    /// <summary>
    /// 比較廠商庫存異動和系統庫存異動資訊
    /// </summary>
    /// <param name="dsFactory_Stock_Number">廠商庫存異動資訊</param>
    /// <param name="dsSystem_Stock_Number">系統庫存異動資訊</param>
    /// <param name="dtStockDiff">異動資訊不符記錄</param>
    /// <param name="dtStockSurplusDiff">異動資訊匯總不符記錄</param>
    public void CompareFactorySys(DataSet dsFactory_Stock_Number,
                                DataSet dsSystem_Stock_Number,
                                ref DataTable dtStockDiff)
    {

        DataRow[] drUsed = null;
        DataRow[] drUsedMoveIn = null;
        dtStockDiff = getDTStockDiff();

        #region 取異動資訊不符記錄
        foreach (DataRow drFactoryStockNumber in dsFactory_Stock_Number.Tables[0].Rows)
        {
            string strStatus_Name = drFactoryStockNumber["Status_Name"].ToString();
            if (strStatus_Name == "入庫")
            {
                drUsed = dsSystem_Stock_Number.Tables[0].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "退貨")
            {
                drUsed = dsSystem_Stock_Number.Tables[1].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "再入庫")
            {
                drUsed = dsSystem_Stock_Number.Tables[2].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "3D")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = '3D'");
            }
            else if (strStatus_Name == "DA")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = 'DA'");
            }
            else if (strStatus_Name == "PM")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = 'PM'");
            }
            else if (strStatus_Name == "RN")
            {
                drUsed = dsSystem_Stock_Number.Tables[3].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString() +
                                                        " AND Type_Name = 'RN'");
            }
            else if (strStatus_Name == "移轉")
            {
                drUsed = dsSystem_Stock_Number.Tables[4].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString());
                drUsedMoveIn = dsSystem_Stock_Number.Tables[5].Select("RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString());
            }
            else if (strStatus_Name == "廠商結餘")
            {
                drUsed = dsSystem_Stock_Number.Tables[6].Select("Perso_Factory_RID = " + drFactoryStockNumber["Perso_Factory_RID"].ToString() +
                                                        " AND Type = " + drFactoryStockNumber["Type"].ToString() +
                                                        " AND Affinity = " + drFactoryStockNumber["Affinity"].ToString() +
                                                        " AND Photo = " + drFactoryStockNumber["Photo"].ToString());
            }
            // 檢查數量是否相等
            if (strStatus_Name == "移轉")
            {
                int intMoveNumber = 0;
                if (drUsed != null)
                {
                    if (drUsed.Length > 0)
                    {
                        intMoveNumber -= Convert.ToInt32(drUsed[0]["Number"].ToString());
                    }
                    if (drUsedMoveIn.Length > 0)
                    {
                        intMoveNumber += Convert.ToInt32(drUsedMoveIn[0]["Number"].ToString());
                    }

                    // 檢查移轉數量是否相等
                    if (intMoveNumber != Convert.ToInt32(drFactoryStockNumber["Number"].ToString()))
                    {
                        DataRow drStockDiff = dtStockDiff.NewRow();
                        drStockDiff["Factory_ShortName_CN"] = drFactoryStockNumber["Factory_ShortName_CN"];
                        drStockDiff["Name"] = drFactoryStockNumber["Name"];
                        drStockDiff["Type"] = "移轉";
                        drStockDiff["Factory_Number"] = drFactoryStockNumber["Number"];
                        drStockDiff["System_Number"] = intMoveNumber;
                        if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                        dtStockDiff.Rows.Add(drStockDiff);
                    }
                }
            // 檢查入庫、退貨、再入庫、3D、DA、PM、RN、移轉、本日結余
            }else if (strStatus_Name == "入庫" ||
                strStatus_Name == "退貨" ||
                strStatus_Name == "再入庫" ||
                strStatus_Name == "3D" || 
                strStatus_Name == "DA" ||
                strStatus_Name == "PM" ||
                strStatus_Name == "RN" ||
                strStatus_Name == "廠商結餘")
            {
                int iSystem_Number = 0;
                if (drUsed != null)
                {
                    if (drUsed.Length > 0)
                    {
                        for (int i = 0; i < drUsed.Length; i++)
                        {
                            iSystem_Number += Convert.ToInt32(drUsed[i]["Number"].ToString());
                        }
                    }
                }
                if (iSystem_Number !=
                    Convert.ToInt32(drFactoryStockNumber["Number"].ToString()))
                {
                    // 廠商庫存異動和系統庫存異動不相符時添加保存該記錄
                    DataRow drStockDiff = dtStockDiff.NewRow();
                    drStockDiff["Factory_ShortName_CN"] = drFactoryStockNumber["Factory_ShortName_CN"];
                    drStockDiff["Name"] = drFactoryStockNumber["Name"];
                    drStockDiff["Type"] = strStatus_Name;
                    drStockDiff["Factory_Number"] = drFactoryStockNumber["Number"];
                    drStockDiff["System_Number"] = iSystem_Number;
                    if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                    dtStockDiff.Rows.Add(drStockDiff);
                }
                else
                {
                    // 如果結余量為負數。除虛擬卡外也要顯示。
                    if (iSystem_Number < 0 && strStatus_Name == "廠商結餘")
                    {
                        this.dirValues.Clear();
                        this.dirValues.Add("type", drFactoryStockNumber["Type"].ToString());
                        this.dirValues.Add("affinity", drFactoryStockNumber["Affinity"].ToString());
                        this.dirValues.Add("photo", drFactoryStockNumber["Photo"].ToString());
                        DataSet dstVirtualCardGroup = dao.GetList(CON_CARD_TYPE_GROUP, this.dirValues);
                        if (null != dstVirtualCardGroup && dstVirtualCardGroup.Tables.Count > 0 &&
                            dstVirtualCardGroup.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dstVirtualCardGroup.Tables[0].Rows[0]["Num"].ToString()) == 0)
                            {   // 不為虛擬卡
                                // 廠商庫存異動和系統庫存異動不相符時添加保存該記錄
                                DataRow drStockDiff = dtStockDiff.NewRow();
                                drStockDiff["Factory_ShortName_CN"] = drFactoryStockNumber["Factory_ShortName_CN"];
                                drStockDiff["Name"] = drFactoryStockNumber["Name"];
                                drStockDiff["Type"] = strStatus_Name;
                                drStockDiff["Factory_Number"] = drFactoryStockNumber["Number"];
                                drStockDiff["System_Number"] = iSystem_Number;
                                if (drStockDiff["System_Number"].ToString() != "" && drStockDiff["System_Number"].ToString() != "0")
                                dtStockDiff.Rows.Add(drStockDiff);

                                // XXXPerso廠XXX版面簡稱庫存不足時，警示
                                object[] arg = new object[2];
                                arg[0] = drFactoryStockNumber["Factory_ShortName_CN"].ToString();
                                arg[1] = drFactoryStockNumber["Name"].ToString();
                                Warning.SetWarning(GlobalString.WarningType.PersoChangeCardInMiss,arg);
                            }
                        }
                    }
                }
            }
        }
        #endregion 取異動資訊不符記錄
    }

    /// <summary>
    /// 取厂商結余匯總
    /// </summary>
    /// <param name="dtStock"></param>
    /// <returns></returns>
    public DataTable getFactorySumSurplus(DataTable dtStock)
    {
        DataTable dtRet = new DataTable();
        dtRet.Columns.Add(new DataColumn("Type", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Affinity", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Photo", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
        dtRet.Columns.Add(new DataColumn("SumNumber", Type.GetType("System.Int32")));

        dtStock.DefaultView.Sort = "Type,Affinity,Photo,Name,Status_Name";
        DataTable dtSum = dtStock.DefaultView.ToTable();
        string strCardTypeName = "";
        string strType = "";
        string strAffinity = "";
        string strPhoto = "";
        int intDateSurplusNumber = 0;
        foreach (DataRow dr in dtSum.Rows)
        {
            if (dr["Status_Name"].ToString() == "廠商結餘")
            {
                if (dr["Type"].ToString() != strType || 
                    dr["Affinity"].ToString() != strAffinity ||
                    dr["Photo"].ToString() != strPhoto)
                {
                    if (strType != "")
                    {
                        DataRow drNew = dtRet.NewRow();
                        drNew["Type"] = strType;
                        drNew["Affinity"] = strAffinity;
                        drNew["Photo"] = strPhoto;
                        drNew["Name"] = strCardTypeName;
                        drNew["SumNumber"] = intDateSurplusNumber;
                        dtRet.Rows.Add(drNew);
                    }
                    strType = dr["Type"].ToString();
                    strAffinity = dr["Affinity"].ToString();
                    strPhoto = dr["Photo"].ToString();
                    strCardTypeName = dr["Name"].ToString();
                    intDateSurplusNumber = Convert.ToInt32(dr["Number"].ToString());
                }
                else
                {
                    intDateSurplusNumber += Convert.ToInt32(dr["Number"].ToString());
                }
            }
        }

        if (strType != "")
        {
            DataRow drNew = dtRet.NewRow();
            drNew["Type"] = strType;
            drNew["Affinity"] = strAffinity;
            drNew["Photo"] = strPhoto;
            drNew["Name"] = strCardTypeName;
            drNew["SumNumber"] = intDateSurplusNumber;
            dtRet.Rows.Add(drNew);
        }

        return dtRet;
    }

    /// <summary>
    /// 取系統結余匯總
    /// </summary>
    /// <param name="dtStock"></param>
    /// <returns></returns>
    public DataTable getSystemSumSurplus(DataTable dtStock)
    {
        DataTable dtRet = new DataTable();
        dtRet.Columns.Add(new DataColumn("Type", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Affinity", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Photo", Type.GetType("System.Int32")));
        dtRet.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
        dtRet.Columns.Add(new DataColumn("SumNumber", Type.GetType("System.Int32")));

        dtStock.DefaultView.Sort = "Type,Affinity,Photo";
        DataTable dtSum = dtStock.DefaultView.ToTable();
        int intType = 0;
        int intAffinity = 0;
        int intPhoto = 0;
        string strName = "";
        int intDateSurplusNumber = 0;
        foreach (DataRow dr in dtStock.Rows)
        {
            if (Convert.ToInt32(dr["Type"].ToString()) != intType ||
                Convert.ToInt32(dr["Affinity"].ToString()) != intAffinity ||
                Convert.ToInt32(dr["Photo"].ToString()) != intPhoto)
            {
                if (intType != 0 && intAffinity != 0 && intPhoto != 0)
                {
                    DataRow drNew = dtRet.NewRow();
                    drNew["Type"] = intType;
                    drNew["Affinity"] = intAffinity;
                    drNew["Photo"] = intPhoto;
                    drNew["Name"] = strName;
                    drNew["SumNumber"] = intDateSurplusNumber;
                    dtRet.Rows.Add(drNew);
                }
                intType = Convert.ToInt32(dr["Type"].ToString());
                intAffinity = Convert.ToInt32(dr["Affinity"].ToString());
                intPhoto = Convert.ToInt32(dr["Photo"].ToString());
                strName = dr["Name"].ToString();
                intDateSurplusNumber = Convert.ToInt32(dr["Number"].ToString());
            }
            else
            {
                intDateSurplusNumber += Convert.ToInt32(dr["Number"].ToString());
            }
        }

        if (intType != 0 && intAffinity != 0 && intPhoto != 0)
        {
            DataRow drNew = dtRet.NewRow();
            drNew["Type"] = intType;
            drNew["Affinity"] = intAffinity;
            drNew["Photo"] = intPhoto;
            drNew["Name"] = intPhoto;
            drNew["SumNumber"] = intDateSurplusNumber;
            dtRet.Rows.Add(drNew);
        }

        return dtRet;
    }

    /// <summary>
    /// 對廠商庫存異動資訊和系統庫存異動資訊比較。
    /// </summary>
    /// <param name="dtSurplus">日結日期</param>
    /// <param name="dtLastSurplus">本次日結日期的上一工作日</param>
    /// <param name="dtStockDiff">資訊不相符項</param>
    /// <param name="dtStockSurplusDiff">廠商結余不相符項</param>
    public void Compare(DateTime dtSurplus,
          DateTime dtLastSurplus,
          ref DataTable dtStockDiff,
          ref DataTable dtStockSurplusDiff)
    { 
        DataSet dsFci = new DataSet();
        DataSet dsSys = new DataSet();

        try{
            // 取廠商庫存異動資訊和系統庫存資訊
            getFactorySysStockNumber(dtSurplus,dtLastSurplus, ref dsFci, ref dsSys);

            // 比較廠商庫存異動資訊和系統庫存資訊
            CompareFactorySys(dsFci, dsSys, ref dtStockDiff, ref dtStockSurplusDiff);
        }catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 取廠商庫存異動資訊和系統庫存資訊
    /// </summary>
    /// <param name="Date">日結日期</param>
    /// <param name="dtLastSurplus">本次日結日期的上一工作日</param>
    /// <param name="dsFACTORY_CHANGE_IMPORT">廠商庫存異動資訊</param>
    /// <param name="dsSys_Stock">系統庫存異動資訊</param>
    public void getFactorySysStockNumber(DateTime Date,
           DateTime dtLastSurplus,
           ref DataSet dsFACTORY_CHANGE_IMPORT,
           ref DataSet dsSys_Stock)
    {
        try
        {
            //取系統應該日結的Perso廠、卡種訊息
            DataTable dtShouldSurplusCardType = getAllShouldSurplusCardType(Date, dtLastSurplus);
            if (null == dtShouldSurplusCardType)
            {
                throw new AlertException("沒有需要日結的Perso廠卡種訊息。");
            }

            #region 取消耗卡公式
            this.dirValues.Clear();
            DataSet dsEXPRESSIONS = dao.GetList(SEL_EXPRESSIONS_DEFINE);
            if (!(null != dsEXPRESSIONS &&
                    dsEXPRESSIONS.Tables.Count > 0 &&
                    dsEXPRESSIONS.Tables[0].Rows.Count > 0))
            {
                throw new AlertException("系統的消耗卡公式不正確，請跟管理員確認。");
            }
            #endregion 取消耗卡公式

            #region 廠商庫存異動匯入按Perso廠、卡種、卡種狀況匯總。
            dirValues.Clear();
            dirValues.Add("date_time_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("date_time_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            dsFACTORY_CHANGE_IMPORT = dao.GetList(SEL_FACTORY_IMPORT_STOCKS, dirValues);
            #endregion 廠商庫存異動

            if (dsFACTORY_CHANGE_IMPORT.Tables[0].Rows.Count == 0)
                throw new AlertException(Date.ToShortDateString() + "卡片庫存異動檔未匯入成功，系統不執行日結作業");

            // 取系統廠商結余DataTable
            DataTable dtSys_Stock_Surplus = getDTSys_Stock();

            #region 取系統庫存異動資訊
            // 取系統入庫、退貨、再入庫、(3D、DA、PM、RN)、移出、移入資訊
            DataSet dsSYSTEM_FACTORY_STOCK = dao.GetList(
                                SEL_SYS_IN_STOCKS + 
                                SEL_SYS_RETURN_STOCKS + 
                                SEL_SYS_DEPOSITORY_RESTOCK + 
                                SEL_SYS_SUBTOTAL_TYPE + 
                                SEL_SYS_MOVEOUT_STOCKS + 
                                SEL_SYS_MOVEIN_STOCKS, dirValues);
            // 計算系統的廠商結余
            // 計算方法：系統的前天結余 + 入庫 - 退貨 + 再入庫 - 消耗 + 移入 - 移出
//            foreach (DataRow drFactoryChangeImport in dsFACTORY_CHANGE_IMPORT.Tables[0].Rows)
            //上一日結日期
            DateTime LastSurplusDate = getLastSurplusDate();
            foreach (DataRow drFactoryChangeImport in dtShouldSurplusCardType.Rows)
            {
                //if (drFactoryChangeImport["Status_Name"].ToString() == "廠商結餘")
                //{
                    int intPersoCardTypeBeforeDateSurplus = 0;
                    #region 取系統該卡種、Perso廠前一天的結余
                    this.dirValues.Clear();
                    dirValues.Add("perso_factory_rid", drFactoryChangeImport["Perso_Factory_RID"].ToString());
                    dirValues.Add("type", drFactoryChangeImport["TYPE"].ToString());
                    dirValues.Add("affinity", drFactoryChangeImport["AFFINITY"].ToString());
                    dirValues.Add("photo", drFactoryChangeImport["PHOTO"].ToString());
                    dirValues.Add("Stock_Date", LastSurplusDate);
                    DataSet dsPersoCardTypeLastSurplus = dao.GetList(SEL_PERSO_CARDTYPE_BEFORE_DATE_SURPLUS, dirValues);
                    if (null != dsPersoCardTypeLastSurplus &&
                        dsPersoCardTypeLastSurplus.Tables.Count > 0 &&
                        dsPersoCardTypeLastSurplus.Tables[0].Rows.Count > 0)
                    {
                        intPersoCardTypeBeforeDateSurplus = Convert.ToInt32(dsPersoCardTypeLastSurplus.Tables[0].Rows[0]["Stocks_Number"].ToString());

                        if (intPersoCardTypeBeforeDateSurplus < 0)
                        {
                            intPersoCardTypeBeforeDateSurplus = 0;
                        }
                    }
                    #endregion 取系統該卡種、Perso廠前一天的結余

                    int intPersoCardTypeUsedNumber = 0;
                    #region 依消耗卡公式，計算該Perso廠、卡種的消耗卡數
                    for (int intLoop = 0; intLoop < dsEXPRESSIONS.Tables[0].Rows.Count; intLoop++)
                    {
                        if (dsEXPRESSIONS.Tables[0].Rows[intLoop]["Operate"].ToString() != "捨")
                        { 
                            DataRow[] drUsed = null;
                            DataRow[] drUsedIn = null;
                            string strStatus_Name = dsEXPRESSIONS.Tables[0].Rows[intLoop]["Status_Name"].ToString();
                            switch (strStatus_Name.ToUpper())
                            {
                                case "3D":
                                    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[3].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Type_Name = '3D'");
                                    break;
                                case "DA":
                                    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[3].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Type_Name = 'DA'");
                                    break;
                                case "PM":
                                    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[3].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Type_Name = 'PM'");
                                    break;
                                case "RN":
                                    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[3].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Type_Name = 'RN'");
                                    break;
                                case "樣卡":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '樣卡'");
                                    break;
                                case "未製卡":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '未製卡'");
                                    break;
                                case "補製卡":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '補製卡'");
                                    break;
                                case "製損卡":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '製損卡'");
                                    break;
                                case "排卡":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '排卡'");
                                    break;
                                case "感應不良":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '感應不良'");
                                    break;
                                case "缺卡":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '缺卡'");
                                    break;
                                case "銷毀":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '銷毀'");
                                    break;
                                case "調整":
                                    drUsed = dsFACTORY_CHANGE_IMPORT.Tables[0].Select("Perso_Factory_RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString() +
                                                               " AND Status_Name = '調整'");
                                    break;
                                //case "移轉":
                                //    // 移出
                                //    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[4].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                //                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                //                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                //                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                                //    // 移入
                                //    drUsedIn = dsSYSTEM_FACTORY_STOCK.Tables[5].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                //                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                //                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                //                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                                //    break;
                                //case "入庫":
                                //    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[0].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                //                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                //                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                //                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                                //    break;
                                //case "退貨":
                                //    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[1].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                //                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                //                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                //                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                                //    break;
                                //case "再入庫":
                                //    drUsed = dsSYSTEM_FACTORY_STOCK.Tables[2].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                //                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                //                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                //                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                                //    break;
                            }

                            if (drUsed == null && drUsedIn == null)
                                continue;

                            // 移轉的計算
                            if (strStatus_Name == "移轉")
                            {
                                //if (dsEXPRESSIONS.Tables[0].Rows[intLoop]["Operate"].ToString() == "+")
                                //{
                                //    if (drUsedIn.Length > 0)
                                //    { 
                                //        intPersoCardTypeUsedNumber -= Convert.ToInt32(drUsedIn[0]["Number"].ToString());
                                //    }
                                //    if (drUsed.Length >0)
                                //    {
                                //        intPersoCardTypeUsedNumber += Convert.ToInt32(drUsed[0]["Number"].ToString());
                                //    }
                                //}
                                //else
                                //{
                                //    if (drUsedIn.Length > 0)
                                //    {
                                //        intPersoCardTypeUsedNumber += Convert.ToInt32(drUsedIn[0]["Number"].ToString());
                                //    }
                                //    if (drUsed.Length >0)
                                //    {
                                //        intPersoCardTypeUsedNumber -= Convert.ToInt32(drUsed[0]["Number"].ToString());
                                //    }
                                //}
                            }
                            // 其他非移轉的計算
                            else {
                                if (dsEXPRESSIONS.Tables[0].Rows[intLoop]["Operate"].ToString() == "+")
                                {
                                    for (int i = 0; i < drUsed.Length; i++)
                                    {
                                        intPersoCardTypeUsedNumber += Convert.ToInt32(drUsed[i]["Number"].ToString());
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < drUsed.Length; i++)
                                    {
                                        intPersoCardTypeUsedNumber -= Convert.ToInt32(drUsed[i]["Number"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    #endregion 依消耗卡公式，計算該Perso廠、卡種的消耗卡數

                    int intDepositoryInNumber = 0;
                    #region 該Perso廠、該卡種的入庫數量
                    DataRow[] drDepIn = dsSYSTEM_FACTORY_STOCK.Tables[0].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                    if (drDepIn.Length>0)
                    {
                        intDepositoryInNumber = Convert.ToInt32(drDepIn[0]["Number"].ToString());
                    }
                    #endregion 該Perso廠、該卡種的入庫數量

                    int intDepositoryCancelNumber = 0;
                    #region 該Perso廠、該卡種的退貨數量
                    DataRow[] drDepCancel = dsSYSTEM_FACTORY_STOCK.Tables[1].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                    if (drDepCancel.Length>0)
                    {
                        intDepositoryCancelNumber = Convert.ToInt32(drDepCancel[0]["Number"].ToString());
                    }
                    #endregion 該Perso廠、該卡種的退貨數量

                    int intDepositoryReInNumber = 0;
                    #region 該Perso廠、該卡種的再入庫數量
                    DataRow[] drDepReIn = dsSYSTEM_FACTORY_STOCK.Tables[2].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                    if (drDepReIn.Length>0)
                    {
                        intDepositoryReInNumber = Convert.ToInt32(drDepReIn[0]["Number"].ToString());
                    }
                    #endregion 該Perso廠、該卡種的再入庫數量

                    int intDepositoryMoveNumber = 0;
                    #region 該Perso廠、該卡種的移轉數量
                    DataRow[] drDepMoveOut = dsSYSTEM_FACTORY_STOCK.Tables[4].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                    if (drDepMoveOut.Length>0)
                    {
                        // 轉出數量
                        intDepositoryMoveNumber -= Convert.ToInt32(drDepMoveOut[0]["Number"].ToString());
                    }

                    DataRow[] drDepMoveIn = dsSYSTEM_FACTORY_STOCK.Tables[5].Select("RID = " + drFactoryChangeImport["Perso_Factory_RID"].ToString() +
                                                               " AND Type = " + drFactoryChangeImport["Type"].ToString() +
                                                               " AND Affinity = " + drFactoryChangeImport["Affinity"].ToString() +
                                                               " AND Photo = " + drFactoryChangeImport["Photo"].ToString());
                    if (drDepMoveIn.Length>0)
                    {
                        // 轉入數量
                        intDepositoryMoveNumber += Convert.ToInt32(drDepMoveIn[0]["Number"].ToString());
                    }
                    #endregion 該Perso廠、該卡種的移轉數量

                    int intPersoCardTypeSurplus = 0;//當天結余
                    // 當天結余數量 = 前日結餘 + 入庫 - 退貨 + 再入庫 - 消耗卡 + 4.11庫存移轉
                    intPersoCardTypeSurplus = intPersoCardTypeBeforeDateSurplus 
                                            + intDepositoryInNumber  - intDepositoryCancelNumber
                                            + intDepositoryReInNumber - intPersoCardTypeUsedNumber 
                                            + intDepositoryMoveNumber;
                    // 添加結余
                    DataRow drNewSurplus = dtSys_Stock_Surplus.NewRow();
                    drNewSurplus["RID"] = 0;
                    drNewSurplus["Perso_Factory_RID"] = drFactoryChangeImport["Perso_Factory_RID"];
                    drNewSurplus["Factory_ShortName_CN"] = drFactoryChangeImport["Factory_ShortName_CN"];
                    drNewSurplus["TYPE"] = drFactoryChangeImport["TYPE"];
                    drNewSurplus["AFFINITY"] = drFactoryChangeImport["AFFINITY"];
                    drNewSurplus["PHOTO"] = drFactoryChangeImport["PHOTO"];
                    drNewSurplus["Name"] = drFactoryChangeImport["Name"];
                    drNewSurplus["Status_Name"] = "廠商結餘";
                    drNewSurplus["Number"] = intPersoCardTypeSurplus;
                    // 將結余訊息添加到系統廠商庫存異動訊息中。
                    dtSys_Stock_Surplus.Rows.Add(drNewSurplus);
                //}
            }
            #endregion 系統庫存異動

            if (null!=dsSYSTEM_FACTORY_STOCK)
            {
                dsSYSTEM_FACTORY_STOCK.Tables.Add(dtSys_Stock_Surplus);
                dsSys_Stock = dsSYSTEM_FACTORY_STOCK;
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

    /// <summary>
    /// 取系統廠商結余DataTable結構
    /// </summary>
    private DataTable getDTSys_Stock()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("RID", Type.GetType("System.Int32")));
        dt.Columns.Add(new DataColumn("Perso_Factory_RID",Type.GetType("System.Int32")));
        dt.Columns.Add(new DataColumn("Factory_ShortName_CN", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("TYPE", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("AFFINITY", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("PHOTO", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Status_Name", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Number", Type.GetType("System.Int32")));
        return dt;
    }

    /// <summary>
    /// 取廠商異動和系統異動記錄不符DataTable結構
    /// </summary>
    private DataTable getDTStockDiff()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("Factory_ShortName_CN", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Type", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Factory_Number", Type.GetType("System.Int32")));
        dt.Columns.Add(new DataColumn("System_Number", Type.GetType("System.Int32")));
        return dt;
    }

    /// <summary>
    /// 取廠商異動結余和系統異動結余記錄不符DataTable結構
    /// </summary>
    private DataTable getDTStockSurplusDiff()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Factory_Number", Type.GetType("System.Int32")));
        dt.Columns.Add(new DataColumn("System_Number", Type.GetType("System.Int32")));
        return dt;
    }

    /// <summary>
    /// 進行日結
    /// </summary>
    /// <returns></returns>
    public string DaySurplus(DateTime Date)
    {
        try
        {
            dao.OpenConnection();
            // 卡種晶片規格變化表的處理
            getWaferUsedLog(Date);
            // 用小計檔生成卡片對應的物料耗用記錄
            List<string> lstMaterielUsed = (List<string>)getMaterialUsed(Date);
            // 計算物料剩余數量并警示
            //200908CR物料消耗，代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/25 start
            //getMaterielStocks(Date, lstMaterielUsed);
            InOut000BL BL000 = new InOut000BL();
            BL000.getDayMaterielStocks(Date, lstMaterielUsed);
            //200908CR物料消耗，代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/25 end
            //200911CR 日結時計算每日物料庫存結余 add by 楊昆 2009/11/26 start
            BL000.SaveSurplusSystemNum(Date);
            //200911CR 日結時計算每日物料庫存結余 add by 楊昆 2009/11/26 end
            // 計算代製費用
            string strRet = getProjectCost(Date);
            // 將相關記錄標記設置為日結...待續
            setDaySurplus(Date);

            SetOprLog("5");

            // 提交事務
            dao.Commit();

            return strRet;
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
    }

    /// <summary>
    /// 卡種晶片規格變化表的處理
    /// </summary>
    /// <returns></returns>
    public bool getWaferUsedLog(DateTime Date)
    {
        WAFER_CARDTYPE_USELOG wcuMModel = new WAFER_CARDTYPE_USELOG();
        //卡種消耗表
        DataTable dtUSE_CARDTYPE = new DataTable();
        dtUSE_CARDTYPE.Columns.Add("Perso_Factory_RID");
        dtUSE_CARDTYPE.Columns.Add("CardType_RID");
        dtUSE_CARDTYPE.Columns.Add("Number");

        try
        {
            dirValues.Clear();
            dirValues.Add("date_time_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("date_time_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            #region Perso廠的入庫處理
            //取所有日結日期當天的入庫記錄
            DataSet dsIN_STOCKS = dao.GetList(SEL_SYS_IN_STOCKS_SURPLUS, dirValues);

            //將入庫記錄添加至晶片規格變化表中
            for (int i = 0; i < dsIN_STOCKS.Tables[0].Rows.Count; i++)
            {
                wcuMModel.Income_Date = Date;
                wcuMModel.Usable_Number = Convert.ToInt32(dsIN_STOCKS.Tables[0].Rows[i]["Income_Number"]);
                wcuMModel.Factory_RID = Convert.ToInt32(dsIN_STOCKS.Tables[0].Rows[i]["Perso_Factory_RID"]);
                wcuMModel.CardType_RID = Convert.ToInt32(dsIN_STOCKS.Tables[0].Rows[i]["Space_Short_RID"]);

                //新增晶片規格變化表時，寫入日期為最小日期1900/01/01
                //wcuMModel.Begin_Date = Date;
                wcuMModel.Begin_Date = DateTime.MinValue.AddYears (1899);

                wcuMModel.Wafer_RID = Convert.ToInt32(dsIN_STOCKS.Tables[0].Rows[i]["Wafer_RID"]);
                wcuMModel.Operate_RID = Convert.ToInt32(dsIN_STOCKS.Tables[0].Rows[i]["RID"]);
                wcuMModel.Operate_Type = "1";
                wcuMModel.CardType_Move_RID = 0;
                wcuMModel.Number = wcuMModel.Usable_Number;
                dao.Add<WAFER_CARDTYPE_USELOG>(wcuMModel, "RID");
            }
            #endregion

            #region Perso廠的再入庫處理
            //取所有日結日期當天的再入庫記錄
            DataSet dsRESTOCK = dao.GetList(SEL_SYS_DEPOSITORY_RESTOCK_SURPLUS, dirValues);

            //將再入庫記錄添加到晶片規格變化表中
            for (int i = 0; i < dsRESTOCK.Tables[0].Rows.Count; i++)
            {
                wcuMModel.Income_Date = Date;
                wcuMModel.Usable_Number = Convert.ToInt32(dsRESTOCK.Tables[0].Rows[i]["Reincome_Number"]);
                wcuMModel.Factory_RID = Convert.ToInt32(dsRESTOCK.Tables[0].Rows[i]["Perso_Factory_RID"]);
                wcuMModel.CardType_RID = Convert.ToInt32(dsRESTOCK.Tables[0].Rows[i]["Space_Short_RID"]);

                //新增晶片規格變化表時，寫入日期為最小日期1900/01/01
                //wcuMModel.Begin_Date = Date;
                wcuMModel.Begin_Date = DateTime.MinValue.AddYears(1899);

                wcuMModel.Wafer_RID = Convert.ToInt32(dsRESTOCK.Tables[0].Rows[i]["Wafer_RID"]);
                wcuMModel.Operate_RID = Convert.ToInt32(dsRESTOCK.Tables[0].Rows[i]["RID"]);
                wcuMModel.Operate_Type = "2";
                wcuMModel.CardType_Move_RID = 0;
                wcuMModel.Number = wcuMModel.Usable_Number;
                dao.Add<WAFER_CARDTYPE_USELOG>(wcuMModel, "RID");
            }
            #endregion

            #region Perso廠的退貨處理
            //取所有日結日期當天的退貨記錄
            DataSet dsRETURN_STOCKS = dao.GetList(SEL_SYS_RETURN_STOCKS_SURPLUS, dirValues);
            DataSet dsWAFER_CARDTYPE_USELOG_RID = null;

            //用退貨記錄中的退貨量，扣除晶片規格變化表中的剩余數量
            for (int i = 0; i < dsRETURN_STOCKS.Tables[0].Rows.Count; i++)
            {
                dirValues.Clear();
                dirValues.Add("stock_rid", dsRETURN_STOCKS.Tables[0].Rows[i]["Stock_RID"].ToString());
                dsWAFER_CARDTYPE_USELOG_RID = dao.GetList(SEL_WAFER_CARDTYPE_USELOG_RID, dirValues);

                if (dsWAFER_CARDTYPE_USELOG_RID.Tables[0].Rows.Count == 0)
                    throw new AlertException("退貨，扣對應入庫的晶片規格變化表時，沒找到對應的入庫晶片規格變化表。日結不成功");

                //檢查該記錄是否已經保存
                saveWafer_Uselog_Rollback(Convert.ToInt32(dsWAFER_CARDTYPE_USELOG_RID.Tables[0].Rows[0]["RID"]), Date);

                dirValues.Clear();
                dirValues.Add("rid", dsWAFER_CARDTYPE_USELOG_RID.Tables[0].Rows[0]["RID"].ToString());
                dirValues.Add("cancel_number", dsRETURN_STOCKS.Tables[0].Rows[i]["Cancel_Number"].ToString());
                dirValues.Add("check_date", Date);

                //扣除晶片規格變化表中的剩余數量
                dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG, dirValues);
            }
            #endregion

            #region 卡片移轉處理
            // 取晶片規格變化表中記錄,DataTable<晶片規格變化表>
            DataSet dsWAFER_CARDTYPE_USELOG = dao.GetList(SEL_WAFER_CARDTYPE_USELOG);
            // 取所有卡片庫存移轉記錄，DataTable<卡片移轉>
            dirValues.Clear();
            dirValues.Add("check_date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("check_date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            DataSet dsCARD_TYPE_MOVE_SURPLUS = dao.GetList(SEL_CARD_TYPE_MOVE_SURPLUS, dirValues);
            foreach (DataRow dr in dsCARD_TYPE_MOVE_SURPLUS.Tables[0].Rows)
            {
                int intWAFER_CARDTYPE_USELOGRows = 0;
                intWAFER_CARDTYPE_USELOGRows = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows.Count;
                for (int i = 0; i < intWAFER_CARDTYPE_USELOGRows; i++)
                {
                    if (dr["From_Factory_RID"].ToString() == dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Factory_RID"].ToString() &&
                        dr["CardType_RID"].ToString() == dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["CardType_RID"].ToString())
                    {
                        if (Convert.ToInt32(dr["Move_Number"]) < Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]))
                        {
                            //檢查該記錄是否已經保存
                            saveWafer_Uselog_Rollback(Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]), Date);

                            //扣減晶片規格變化表可用數量 
                            dirValues.Clear();
                            dirValues.Add("rid", dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"].ToString());
                            dirValues.Add("number", Convert.ToInt32(dr["Move_Number"]));
                            dirValues.Add("check_date", Date);

                            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_1, dirValues);

                            //晶片規格變化表中添加，轉入Perso廠、卡種、晶片類型、移入數量記錄
                            //新增晶片規格變化表時，寫入日期為最小日期1900/01/01
                            //wcuMModel.Begin_Date = Convert.ToDateTime(dr["Move_Date"]);
                            wcuMModel.Begin_Date = DateTime.MinValue.AddYears(1899);

                            wcuMModel.Income_Date = Convert.ToDateTime(dr["Move_Date"]);
                            wcuMModel.CardType_RID = Convert.ToInt32(dr["CardType_RID"]);
                            wcuMModel.Factory_RID = Convert.ToInt32(dr["To_Factory_RID"]);
                            wcuMModel.Operate_RID = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_RID"]);
                            wcuMModel.Operate_Type = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_Type"].ToString();
                            wcuMModel.Usable_Number = Convert.ToInt32(dr["Move_Number"]);
                            wcuMModel.Wafer_RID = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Wafer_RID"]);
                            wcuMModel.CardType_Move_RID = Convert.ToInt32(dr["RID"]);
                            wcuMModel.Number = Convert.ToInt32(dr["Move_Number"]);
                            wcuMModel.Unit_Price = Convert.ToDecimal(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Unit_Price"]);
                            dao.Add<WAFER_CARDTYPE_USELOG>(wcuMModel, "RID");
                            

                            //DataTable<晶片規格變化表>中添加Perso廠、卡種、移入數量（為后面的扣減作準備）
                            DataRow drow = dsWAFER_CARDTYPE_USELOG.Tables[0].NewRow();
                            drow["Begin_Date"] = dr["Move_Date"];
                            drow["Income_Date"] = dr["Move_Date"];
                            drow["CardType_RID"] = dr["CardType_RID"];
                            drow["Factory_RID"] = dr["To_Factory_RID"];
                            drow["Operate_RID"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_RID"];
                            drow["Operate_Type"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_Type"].ToString();
                            drow["Usable_Number"] = dr["Move_Number"];
                            drow["Wafer_RID"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Wafer_RID"];
                            dsWAFER_CARDTYPE_USELOG.Tables[0].Rows.Add(drow);
                            break;
                        }

                        if (Convert.ToInt32(dr["Move_Number"]) == Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]))
                        {
                            //檢查該記錄是否已經保存
                            saveWafer_Uselog_Rollback(Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]), Date);

                            //將可用數量設置為0、將End_Date設置為日結日期
                            dirValues.Clear();
                            dirValues.Add("rid", dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]);
                            dirValues.Add("check_date", Date);
                            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_2, dirValues);

                            //晶片規格變化表中添加，轉入Perso廠、卡種、晶片類型、移入數量記錄

                            //新增晶片規格變化表時，寫入日期為最小日期1900/01/01
                            //wcuMModel.Begin_Date = Convert.ToDateTime(dr["Move_Date"]);
                            wcuMModel.Begin_Date = DateTime.MinValue.AddYears(1899);

                            wcuMModel.Income_Date = Convert.ToDateTime(dr["Move_Date"]);
                            wcuMModel.CardType_RID = Convert.ToInt32(dr["CardType_RID"]);
                            wcuMModel.Factory_RID = Convert.ToInt32(dr["To_Factory_RID"]);
                            wcuMModel.Operate_RID = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_RID"]);
                            wcuMModel.Operate_Type = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_Type"].ToString();
                            wcuMModel.Usable_Number = Convert.ToInt32(dr["Move_Number"]);
                            wcuMModel.Wafer_RID = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Wafer_RID"]);
                            wcuMModel.CardType_Move_RID = Convert.ToInt32(dr["RID"]);
                            wcuMModel.Number = Convert.ToInt32(dr["Move_Number"]);
                            wcuMModel.Unit_Price = Convert.ToDecimal(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Unit_Price"]);
                            dao.Add<WAFER_CARDTYPE_USELOG>(wcuMModel, "RID");
                            

                            //DataTable<晶片規格變化表>中添加Perso廠、卡種、移入數量（為后面的扣減作準備）
                            DataRow drow = dsWAFER_CARDTYPE_USELOG.Tables[0].NewRow();
                            drow["Begin_Date"] = dr["Move_Date"];
                            drow["Income_Date"] = dr["Move_Date"];
                            drow["CardType_RID"] = dr["CardType_RID"];
                            drow["Factory_RID"] = dr["To_Factory_RID"];
                            drow["Operate_RID"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_RID"];
                            drow["Operate_Type"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_Type"];
                            drow["Usable_Number"] = dr["Move_Number"];
                            drow["Wafer_RID"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Wafer_RID"];
                            dsWAFER_CARDTYPE_USELOG.Tables[0].Rows.Add(drow);
                            break;
                        }

                        if (Convert.ToInt32(dr["Move_Number"]) > Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]))
                        {
                            //檢查該記錄是否已經保存
                            saveWafer_Uselog_Rollback(Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]), Date);

                            //將可用數量設置為0、將End_Date設置為日結日期 todo
                            dirValues.Clear();
                            dirValues.Add("rid", dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]);
                            dirValues.Add("check_date", Date);
                            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_2, dirValues);
                            dr["Move_Number"] = Convert.ToInt32(dr["Move_Number"]) - Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]);

                            //晶片規格變化表中添加，轉入Perso廠、卡種、晶片類型、移入數量記錄
                            //新增晶片規格變化表時，寫入日期為最小日期1900/01/01
                            //wcuMModel.Begin_Date = Convert.ToDateTime(dr["Move_Date"]);
                            wcuMModel.Begin_Date = DateTime.MinValue.AddYears(1899);

                            wcuMModel.Income_Date = Convert.ToDateTime(dr["Move_Date"]);
                            wcuMModel.CardType_RID = Convert.ToInt32(dr["CardType_RID"]);
                            wcuMModel.Factory_RID = Convert.ToInt32(dr["To_Factory_RID"]);
                            wcuMModel.Operate_RID = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_RID"]);
                            wcuMModel.Operate_Type = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_Type"].ToString();
                            wcuMModel.Usable_Number = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]); //Convert.ToInt32(dr["Move_Number"]);
                            wcuMModel.Wafer_RID = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Wafer_RID"]);
                            wcuMModel.CardType_Move_RID = Convert.ToInt32(dr["RID"]);
                            wcuMModel.Number = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]);//Convert.ToInt32(dr["Move_Number"]);
                            wcuMModel.Unit_Price = Convert.ToDecimal(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Unit_Price"]);
                            dao.Add<WAFER_CARDTYPE_USELOG>(wcuMModel, "RID");
                            
                            //DataTable<晶片規格變化表>中添加Perso廠、卡種、移入數量（為后面的扣減作準備）
                            DataRow drow = dsWAFER_CARDTYPE_USELOG.Tables[0].NewRow();
                            drow["Begin_Date"] = dr["Move_Date"];
                            drow["Income_Date"] = dr["Move_Date"];
                            drow["CardType_RID"] = dr["CardType_RID"];
                            drow["Factory_RID"] = dr["To_Factory_RID"];
                            drow["Operate_RID"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_RID"];
                            drow["Operate_Type"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Operate_Type"];
                            drow["Usable_Number"] = Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]);//dr["Move_Number"];
                            drow["Wafer_RID"] = dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Wafer_RID"];
                            dsWAFER_CARDTYPE_USELOG.Tables[0].Rows.Add(drow);
                        }
                    }
                }
            }
                    #endregion

            #region 廠商異動資料處理(處理范圍包括：3D、DA、PM、RN小計檔消耗以及樣卡 +未製卡 + 補製卡 + 製損卡 + 排卡 +感應不良 + 缺卡 + 銷毀+ 調整其他消耗)
            //重新取晶片規格變化表中記錄,DataTable<晶片規格變化表>
            dsWAFER_CARDTYPE_USELOG = dao.GetList(SEL_WAFER_CARDTYPE_USELOG);
            //取日結日期當天的所有異動記錄,DataSet(庫存異動)
            dirValues.Clear();
            dirValues.Add("date_time_start", Date.ToString("yyyy-MM-dd 00:00:00"));
            dirValues.Add("date_time_end", Date.ToString("yyyy-MM-dd 23:59:59"));
            DataSet dsUSE_CARDTYPE = dao.GetList(SEL_USE_CARDTYPE, dirValues);
            //取卡片消耗公式
            DataSet dsEXPRESSIONS = dao.GetList(SEL_EXPRESSIONS_DEFINE);

            //按Perso廠、卡種的計算消耗量（循環加總各種狀況的消耗數量）
            int Card_Type_Rid = 0;
            int Perso_Factory_RID = 0;
            int Number = 0;
            //todo 此循環可以改進為存儲過程
            foreach (DataRow dr in dsUSE_CARDTYPE.Tables[0].Rows)
            {
                if ((Convert.ToInt32(dr["RID"]) != Card_Type_Rid) ||
                    (Convert.ToInt32(dr["Perso_Factory_RID"]) != Perso_Factory_RID))
                {
                    if (Card_Type_Rid != 0 && Perso_Factory_RID != 0 && Number!=0)
                    {
                        DataRow drow = dtUSE_CARDTYPE.NewRow();
                        drow["Number"] = Number.ToString();
                        drow["Perso_Factory_RID"] = Perso_Factory_RID.ToString();
                        drow["CardType_RID"] = Card_Type_Rid.ToString();
                        dtUSE_CARDTYPE.Rows.Add(drow);
                    }

                    #region 取消耗卡公式,計算消耗卡數
                    Number = 0;
                    DataRow[] drEXPRESSIONS = dsEXPRESSIONS.Tables[0].Select("Type_RID = " + dr["Status_RID"].ToString());
                    if (drEXPRESSIONS.Length > 0)
                    {
                        if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Add_RID)
                        {
                            Number += Convert.ToInt32(dr["Number"]);
                            Card_Type_Rid = Convert.ToInt32(dr["RID"]);
                            Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
                        }
                        else if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Del_RID)
                        {
                            Number -= Convert.ToInt32(dr["Number"]);
                            Card_Type_Rid = Convert.ToInt32(dr["RID"]);
                            Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 取消耗卡公式,計算消耗卡數
                    DataRow[] drEXPRESSIONS = dsEXPRESSIONS.Tables[0].Select("Type_RID = " + dr["Status_RID"].ToString());
                    if (drEXPRESSIONS.Length > 0)
                    {
                        if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Add_RID)
                        {
                            Number += Convert.ToInt32(dr["Number"]);
                        }
                        else if (drEXPRESSIONS[0]["Operate"].ToString() == GlobalString.Operation.Del_RID)
                        {
                            Number -= Convert.ToInt32(dr["Number"]);
                        }
                    }
                    #endregion
                }
            }
            if (Card_Type_Rid != 0 && Perso_Factory_RID != 0 && Number != 0)
            {
                DataRow drow = dtUSE_CARDTYPE.NewRow();
                drow["Number"] = Number.ToString();
                drow["Perso_Factory_RID"] = Perso_Factory_RID.ToString();
                drow["CardType_RID"] = Card_Type_Rid.ToString();
                dtUSE_CARDTYPE.Rows.Add(drow);
            } 

            //根據Perso廠、卡種、消耗量，扣晶片規格變化表中卡種剩余數量。DataTable<卡種消耗表>
            foreach (DataRow dr in dtUSE_CARDTYPE.Rows)
            {
                for (int i = 0; i < dsWAFER_CARDTYPE_USELOG.Tables[0].Rows.Count; i++)
                {
                    if (dr["Perso_Factory_RID"].ToString() == dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Factory_RID"].ToString() && 
                        dr["CardType_RID"].ToString() == dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["CardType_RID"].ToString())
                    {
                        if (Convert.ToInt32(dr["Number"]) < Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]))
                        {
                            //保存扣除之前的記錄，以備取消日結時恢復庫存量。
                            saveWafer_Uselog_Rollback(Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]), Date);

                            //扣減晶片規格變化表可用數量 
                            dirValues.Clear();
                            dirValues.Add("rid", dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"].ToString());
                            dirValues.Add("number", Convert.ToInt32(dr["Number"]));
                            dirValues.Add("check_date", Date);

                            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_1, dirValues);
                            break;
                        }

                        if (Convert.ToInt32(dr["Number"]) == Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]))
                        {
                            //保存扣除之前的記錄，以備取消日結時恢復庫存量。
                            saveWafer_Uselog_Rollback(Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]), Date);

                            //將可用數量設置為0、將End_Date設置為日結日期
                            dirValues.Clear();
                            dirValues.Add("rid", dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"].ToString());
                            dirValues.Add("check_date", Date);
                            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_2, dirValues);
                            break;
                        }

                        if (Convert.ToInt32(dr["Number"]) > Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]))
                        {
                            //保存扣除之前的記錄，以備取消日結時恢復庫存量。
                            saveWafer_Uselog_Rollback(Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"]), Date);

                            //將可用數量設置為0、將End_Date設置為日結日期 todo
                            dirValues.Clear();
                            dirValues.Add("rid", dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["RID"].ToString());
                            dirValues.Add("check_date", Date);
                            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_2, dirValues);
                            dr["Number"] = Convert.ToInt32(dr["Number"]) - Convert.ToInt32(dsWAFER_CARDTYPE_USELOG.Tables[0].Rows[i]["Usable_Number"]);
                        }
                    }
                }
            }

            #endregion

        

            #region 2009/01/11 為做成本分析,將日結當天沒有變化的晶片規格檔保存到回滾檔中
            dirValues.Clear();
            dirValues.Add("date_time_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("date_time_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            // 重新取晶片規格變化表中記錄,DataTable<晶片規格變化表>
            dsWAFER_CARDTYPE_USELOG = dao.GetList(SEL_WAFER_CARDTYPE_USELOG_FIRST_ZERO);
            foreach (DataRow drChengben in dsWAFER_CARDTYPE_USELOG.Tables[0].Rows)
            {
                //檢查該記錄是否已經保存
                saveWafer_Uselog_Rollback(Convert.ToInt32(drChengben["RID"]), Date);
            }

            //當天入庫的記錄，不應該COPY至ROLLBACK檔，所以在所有新增完成後，刪除一步刪除操作！
            dirValues.Clear();
            dirValues.Add("check_date_start", Date.ToString("yyyy-MM-dd 00:00:00"));
            dao.ExecuteNonQuery(DEL_WAFER_USELOG_ROLLBACK_TODAY, dirValues);

            #endregion 2009/01/11 為做成本分析,將日結當天沒有變化的晶片規格檔保存到回滾檔中
            return true;
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

    /// <summary>
    /// 計算物料剩余數量并警示
    /// </summary>
    /// <param name="Date"></param>
    //public void getMaterielStocks(DateTime Surplus_Date, List<string> lstSerielNumber)
    //{
    //    try
    //    {
    //        Depository010BL bl010 = new Depository010BL();

    //        // 有物品編號時，處理
    //        if (lstSerielNumber.Count > 0)
    //        {
    //            string strSerielNumbers = "'";
    //            foreach (string strSerielNumberFor in lstSerielNumber)
    //            {
    //                strSerielNumbers += strSerielNumberFor + "','";
    //            }
    //            strSerielNumbers = strSerielNumbers.Substring(0, strSerielNumbers.Length - 2);

    //            // 取當前天的上一工作日
    //            DateTime dtLastWorkDate = DateTime.Parse("1900-01-01");
    //            dirValues.Clear();
    //            dirValues.Add("date_time", Surplus_Date.ToString("yyyy/MM/dd"));
    //            DataSet dsWorkDate = dao.GetList(SEL_LAST_WORK_DATE, this.dirValues);
    //            if (null != dsWorkDate &&
    //                dsWorkDate.Tables.Count > 0 &&
    //                dsWorkDate.Tables[0].Rows.Count > 0)
    //            {
    //                dtLastWorkDate = Convert.ToDateTime(dsWorkDate.Tables[0].Rows[0]["Date_Time"].ToString());
    //            }

    //            // 取當前工作日的上一工作日的所有庫存
    //            dirValues.Clear();
    //            dirValues.Add("stock_date", dtLastWorkDate.ToString("yyyy/MM/dd"));
    //            //dirValues.Add("seriel_numbers", strSerielNumbers);
    //            DataSet dsMaterielStocksManager = dao.GetList(SEL_MATERIEL_STOCKS_MANAGER + strSerielNumbers+")", this.dirValues);
    //            if (null != dsMaterielStocksManager &&
    //                dsMaterielStocksManager.Tables.Count > 0 &&
    //                dsMaterielStocksManager.Tables[0].Rows.Count > 0)
    //            {
    //                foreach (DataRow drMSM in dsMaterielStocksManager.Tables[0].Rows)
    //                {
    //                    dirValues.Clear();
    //                    dirValues.Add("stock_date", Surplus_Date.ToString("yyyy/MM/dd"));
    //                    dirValues.Add("serial_number", drMSM["Serial_Number"].ToString());
    //                    dirValues.Add("perso_factory_rid", drMSM["Perso_Factory_RID"].ToString());
    //                    DataSet dsMaterielStocksUsed = dao.GetList(SEL_MATERIEL_USED, this.dirValues);
    //                    if (null != dsMaterielStocksUsed &&
    //                        dsMaterielStocksUsed.Tables.Count > 0 &&
    //                        dsMaterielStocksUsed.Tables[0].Rows.Count > 0)
    //                    {
    //                        // 前一天的庫存
    //                        int intLastStockNumber=0;
    //                        int intTheDayUsedNumber=0;
    //                        if (!StringUtil.IsEmpty(drMSM["Number"].ToString()))
    //                            intLastStockNumber = Convert.ToInt32(drMSM["Number"].ToString());
    //                        // 今天消耗的
    //                        if (!StringUtil.IsEmpty(dsMaterielStocksUsed.Tables[0].Rows[0][0].ToString()))
    //                            intTheDayUsedNumber = Convert.ToInt32(dsMaterielStocksUsed.Tables[0].Rows[0][0].ToString());

    //                        // 庫存為0時，顯示庫存不足
    //                        if (intLastStockNumber <= 0)
    //                        {
    //                            if (bl010.DmNotSafe_Type(drMSM["Serial_Number"].ToString()))
    //                            {
    //                                // 庫存不足
    //                                string[] arg = new string[1];
    //                                arg[0] = drMSM["Name"].ToString();
    //                                Warning.SetWarning(GlobalString.WarningType.SubtotalMaterialInMiss, arg);
    //                            }
    //                        }
    //                        // 如果前一天的庫存小余今天的消耗
    //                        else if (intLastStockNumber < intTheDayUsedNumber)
    //                        {
    //                            if (bl010.DmNotSafe_Type(drMSM["Serial_Number"].ToString()))
    //                            {
    //                                // 庫存不足
    //                                string[] arg = new string[1];
    //                                arg[0] = drMSM["Name"].ToString();
    //                                Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInMiss, arg);
    //                                Warning.SetWarning(GlobalString.WarningType.SubtotalMaterialInMiss, arg);
    //                            }
    //                        }
    //                        else
    //                        {
    //                            // 取物料的安全庫存訊息
    //                            DataSet dtMateriel = bl.GetMateriel(drMSM["Serial_Number"].ToString());
    //                            if (null != dtMateriel &&
    //                                dtMateriel.Tables.Count > 0 &&
    //                                dtMateriel.Tables[0].Rows.Count > 0)
    //                            {
    //                                // 最低安全庫存
    //                                if (GlobalString.SafeType.storage == Convert.ToString(dtMateriel.Tables[0].Rows[0]["Safe_Type"]))
    //                                {
    //                                    // 廠商結餘低於最低安全庫存數值時
    //                                    if (intLastStockNumber - intTheDayUsedNumber <
    //                                        Convert.ToInt32(dtMateriel.Tables[0].Rows[0]["Safe_Number"]))
    //                                    {
    //                                        string[] arg = new string[1];
    //                                        arg[0] = dtMateriel.Tables[0].Rows[0]["Name"].ToString();
    //                                        Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInSafe, arg);
    //                                        Warning.SetWarning(GlobalString.WarningType.SubtoalMaterialInSafe, arg);
    //                                    }
    //                                    // 安全天數
    //                                }
    //                                else if (GlobalString.SafeType.days == Convert.ToString(dtMateriel.Tables[0].Rows[0]["Safe_Type"]))
    //                                {
    //                                    // 檢查庫存是否充足
    //                                    if (!CheckMaterielSafeDays(drMSM["Serial_Number"].ToString(),
    //                                                            Convert.ToInt32(drMSM["Perso_Factory_RID"].ToString()),
    //                                                            Convert.ToInt32(dtMateriel.Tables[0].Rows[0]["Safe_Number"]),
    //                                                            intLastStockNumber - intTheDayUsedNumber)) 
    //                                    {
    //                                        string[] arg = new string[1];
    //                                        arg[0] = dtMateriel.Tables[0].Rows[0]["Name"].ToString();
    //                                        Warning.SetWarning(GlobalString.WarningType.PersoChangeMaterialInSafe, arg);
    //                                        Warning.SetWarning(GlobalString.WarningType.SubtoalMaterialInSafe, arg);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}

    /// <summary>
    /// 檢查物料的庫存是否安全（安全天數）
    /// </summary>
    /// <param name="Materiel_RID"></param>
    /// <param name="Materiel_Type"></param>
    /// <param name="Factory_RID"></param>
    /// <param name="Days"></param>
    /// <returns></returns>
    //public bool CheckMaterielSafeDays(string Serial_Number,
    //                                int Factory_RID,
    //                                int Days,
    //                                int Stock_Number)
    //{
    //    bool blCheckMaterielSafeDays = true;
    //    Days = Days + 1;   // 為了適應匯入時的函數需要，需要多減一天
    //    DateTime dtStartTime = DateTime.Now.AddDays(-Days);
    //    DataTable dtblSubtotal_Import = MaterielUsedCount(Factory_RID,
    //                                        Serial_Number,
    //                                        dtStartTime,
    //                                        DateTime.Now);

    //    int intMaterielWear = 0;
    //    if (null != dtblSubtotal_Import &&
    //        dtblSubtotal_Import.Rows.Count > 0)
    //    {
    //        // 前N天的耗用量
    //        for (int intRow = 0; intRow < dtblSubtotal_Import.Rows.Count; intRow++)
    //        {
    //            intMaterielWear += Convert.ToInt32(dtblSubtotal_Import.Rows[intRow]["System_Num"]);
    //        }

    //        // 如果庫存小於前N天的耗用量
    //        if (Stock_Number < intMaterielWear)
    //        {
    //            blCheckMaterielSafeDays = false;
    //        }
    //    }

    //    return blCheckMaterielSafeDays;
    //}

    /// <summary>
    /// 計算物料庫存消耗檔
    /// </summary>
    /// <param name="Factory_RID">Perso廠商RID</param>
    /// <param name="Serial_Number">物料編號</param>    
    /// <param name="lastSurplusDateTime">最近一次的結餘日期</param>
    /// <param name="thisSurplusDateTime">本次結餘日期</param>
    /// <returns>DataTable<物料使用記錄></returns>
    //public DataTable MaterielUsedCount(int Factory_RID,
    //                    string Serial_Number,
    //                    DateTime lastSurplusDateTime,
    //                    DateTime thisSurplusDateTime)
    //{
    //    DataTable dtSubtotal_Import = null;
    //    try
    //    {
    //        dirValues.Clear();
    //        dirValues.Add("Perso_Factory_RID", Factory_RID);
    //        dirValues.Add("Serial_Number", Serial_Number);
    //        dirValues.Add("lastSurplusDateTime", lastSurplusDateTime);
    //        dirValues.Add("thisSurplusDateTime", thisSurplusDateTime);
    //        DataSet dstSTOCKS_USED = dao.GetList(SEL_MATERIEL_STOCKS_USED, dirValues);
    //        if (null != dstSTOCKS_USED && dstSTOCKS_USED.Tables.Count > 0 &&
    //                        dstSTOCKS_USED.Tables[0].Rows.Count > 0)
    //        {
    //            dtSubtotal_Import = dstSTOCKS_USED.Tables[0];
    //            dtSubtotal_Import.Columns.Add(new DataColumn("System_Num", Type.GetType("System.Int32")));
    //            for (int intRow = 0; intRow < dtSubtotal_Import.Rows.Count; intRow++)
    //            {
    //                // 取物品的損耗率(關聯到物品表，取物品表的損耗率）
    //                //Decimal dWear_Rate = GetWearRate(Serial_Number);
    //                //寫USED表時已計算消耗率，不再計算！
    //                Decimal dWear_Rate = 0;

    //                // 系統耗用量
    //                dtSubtotal_Import.Rows[intRow]["System_Num"] = Convert.ToInt32(dtSubtotal_Import.Rows[intRow]["Number"]) * (dWear_Rate / 100 + 1);
    //            }
    //        }
    //        return dtSubtotal_Import;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
    //    }
    //}

    /// <summary>
    /// 取物品的損耗率
    /// </summary>    
    /// <param name="Serial_Number">物品編號 1：信封；2：寄卡單；3：DM</param>
    /// <returns>Decimal<物品的耗用率></returns>
    public Decimal GetWearRate(string Serial_Number)
    {
        Decimal dWearRate = 0;
        DataSet dstWearRate = null;

        try
        {
            dirValues.Clear();
            dirValues.Add("Serial_Number", Serial_Number);
            if ("A" == Serial_Number.Substring(0, 1).ToUpper())// 信封
            {
                dstWearRate = dao.GetList(SEL_ENVELOPE_INFO, dirValues);
            }
            else if ("B" == Serial_Number.Substring(0, 1).ToUpper())// 卡單
            {
                dstWearRate = dao.GetList(SEL_CARD_EXPONENT, dirValues);
            }
            else if ("C" == Serial_Number.Substring(0, 1).ToUpper())// DM
            {
                dstWearRate = dao.GetList(SEL_DMTYPE_INFO, dirValues);
            }

            if (null != dstWearRate &&
                    dstWearRate.Tables.Count > 0 &&
                    dstWearRate.Tables[0].Rows.Count > 0)
            {
                // 取損耗率
                dWearRate = Convert.ToDecimal(dstWearRate.Tables[0].Rows[0]["Wear_Rate"]);
            }

            return dWearRate;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 根據物料和數量計算實際的數量！
    /// </summary>
    /// <param name="MNumber"></param>
    /// <param name="MCount"></param>
    /// <returns></returns>
    private int ComputeMaterialNumber(string MNumber, long MCount)
    {
        int iReturn = 0;
        decimal dWear_Rate = this.GetWearRate(MNumber);
        iReturn = Convert.ToInt32 ( MCount * (dWear_Rate / 100 + 1));
        return iReturn;
    }


    ///// <summary>
    ///// 用小計檔生成卡片對應的物料耗用記錄
    ///// </summary>
    ///// <returns></returns>
    //public List<string> getMaterialUsed(DateTime Date)
    //{
    //    List<string> lstSerielNumber = new List<string>();
    //    MATERIEL_STOCKS_USED msuModel = new MATERIEL_STOCKS_USED();
    //    try
    //    {
    //        dirValues.Clear();
    //        dirValues.Add("check_date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
    //        dirValues.Add("check_date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
    //        //取信封和寄卡單耗用記錄，DataSet<物料耗用記錄>
    //        DataSet dsMATERIAL_BY_SUBTOTAL = dao.GetList(SEL_MATERIAL_BY_SUBTOTAL, dirValues);
    //        foreach (DataRow dr in dsMATERIAL_BY_SUBTOTAL.Tables[0].Rows)
    //        {
    //            if (dr["CE_Number"].ToString() != null && dr["CE_Number"].ToString() != "")
    //            {
    //                // 保存物料品名編號，為判斷物料的庫存和安全水位作準備
    //                if (-1 == lstSerielNumber.IndexOf(dr["CE_Number"].ToString()))
    //                {
    //                    lstSerielNumber.Add(dr["CE_Number"].ToString());
    //                }


    //                msuModel.Stock_Date = Date;
    //                msuModel.Number = this.ComputeMaterialNumber(dr["CE_Number"].ToString(), Convert.ToInt64(dr["Number1"]));
    //                msuModel.Serial_Number = dr["CE_Number"].ToString();
    //                msuModel.Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                dao.Add<MATERIEL_STOCKS_USED>(msuModel, "RID");
    //            }

    //            if (dr["EI_Number"].ToString() != null && dr["EI_Number"].ToString() != "")
    //            {
    //                // 保存物料品名編號，為判斷物料的庫存和安全水位作準備
    //                if (-1 == lstSerielNumber.IndexOf(dr["EI_Number"].ToString()))
    //                {
    //                    lstSerielNumber.Add(dr["EI_Number"].ToString());
    //                }

    //                msuModel.Stock_Date = Date;
    //                msuModel.Number = this.ComputeMaterialNumber(dr["EI_Number"].ToString(), Convert.ToInt64(dr["Number1"]));
    //                msuModel.Serial_Number = dr["EI_Number"].ToString();
    //                msuModel.Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                dao.Add<MATERIEL_STOCKS_USED>(msuModel, "RID");
    //            }
    //        }

    //        //取DM耗用記錄，DataSet<DM物料耗用記錄>
    //        dirValues.Clear();
    //        dirValues.Add("check_date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
    //        dirValues.Add("check_date_end", Date.ToString("yyyy/MM/dd 23:59:59"));

    //        DataSet MATERIAL_BY_SUBTOTAL_DM = dao.GetList(SEL_MATERIAL_BY_SUBTOTAL_DM, dirValues);
    //        foreach (DataRow dr in MATERIAL_BY_SUBTOTAL_DM.Tables[0].Rows)
    //        {
    //            if (dr["DI_Number"].ToString() != "")
    //            {
    //                // 保存物料品名編號，為判斷物料的庫存和安全水位作準備
    //                if (-1 == lstSerielNumber.IndexOf(dr["DI_Number"].ToString()))
    //                {
    //                    lstSerielNumber.Add(dr["DI_Number"].ToString());
    //                }
    //                if (dr["Card_Type_Link_Type"].ToString() == "1" ||
    //                    (dr["Card_Type_Link_Type"].ToString() == "2" && dr["CardType_RID"].ToString() != ""))
    //                {
    //                    msuModel.Stock_Date = Date;
    //                    msuModel.Number = this.ComputeMaterialNumber(dr["DI_Number"].ToString(), Convert.ToInt64(dr["Number1"]));
    //                    msuModel.Serial_Number = dr["DI_Number"].ToString();
    //                    msuModel.Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
    //                    dao.Add<MATERIEL_STOCKS_USED>(msuModel, "RID");
    //                }
    //            }
    //        }
    //        return lstSerielNumber;
    //    }
    //    catch (AlertException ex)
    //    {
    //        throw ex;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    /// <summary>
    /// 用小計檔生成卡片對應的物料耗用記錄
    /// </summary>
    /// <returns></returns>
    public List<string> getMaterialUsed(DateTime Date)
    {
        try
        {
            //200908CR物料消耗，代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 start
            //return bl.SaveMaterielUsedCount(Date);
            InOut000BL BL000 = new InOut000BL();

            //edit by linhuanhuang 庫存異動加入到日結中 start
            // 用物料異動檔添加物料異動記錄
            List<string> lstMateriel = BL000.SaveMaterielUsedCount(Date);
            lstMateriel = BL000.SaveMaterielTransactionCount(Date, lstMateriel);
            //edit by linhuanhuang 庫存異動加入到日結中 end

            return lstMateriel;
            //200908CR物料消耗，代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 start
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
    /// <summary>
    /// 計算代製費用
    /// </summary>
    /// <returns></returns>
    public string getProjectCost(DateTime Date)
    {
        MATERIEL_STOCKS_USED msuModel = new MATERIEL_STOCKS_USED();
        PERSO_PROJECT_DETAIL prdModel = new PERSO_PROJECT_DETAIL();
        string strAlert = "";

        try
        {
            // 先刪除日結當天的一般代製項目
            dirValues.Clear();
            dirValues.Add("Begin_Date", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("Finish_Date", Date.ToString("yyyy/MM/dd 23:59:59"));
            dao.ExecuteNonQuery(DEL_MAKE_COST_FROM_SUBTOTAL_IMPORT, this.dirValues);

            // 生成一般代製項目費用訊息
            // 1、取小計檔資訊
            dirValues.Clear();
            dirValues.Add("check_date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("check_date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            //取日結日期當天的小計檔，DataSet<小計檔>
           //DataSet dsSUBTOTAL_PROJECT_COST = dao.GetList(SEL_SUBTOTAL_PROJECT_COST, dirValues);
          
            //200908CR代制費用計算改為用小計檔的「替換前」版面計算 modified  by 楊昆 2009/09/01 start
            DataSet dsSUBTOTAL_PROJECT_COST = dao.GetList(SEL_SUBTOTAL_REPLACE_PROJECT_COST, dirValues);
            //200908CR代制費用計算改為用小計檔的「替換前」版面計算 modified  by 楊昆 2009/09/01 end
            if (null != dsSUBTOTAL_PROJECT_COST &&
                dsSUBTOTAL_PROJECT_COST.Tables.Count > 0 &&
                dsSUBTOTAL_PROJECT_COST.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsSUBTOTAL_PROJECT_COST.Tables[0].Rows)
                {
                    // 2、取卡種對應的一般代製項目單價
                    this.dirValues.Clear();
                    dirValues.Add("Date_Time", Date.ToString("yyyy/MM/dd 12:00:00"));
                    dirValues.Add("CTRID", dr["RID"].ToString());
                    dirValues.Add("perso_factory_rid", dr["Perso_Factory_RID"].ToString());
                    // 取卡種的一般代製項目
                    DataSet dsPROJECT_STEP_SURPLUS = dao.GetList(SEL_PROJECT_STEP_SURPLUS, dirValues);
                    
                    // 添加一般代製項目訊息
                    if (null != dsPROJECT_STEP_SURPLUS &&
                        dsPROJECT_STEP_SURPLUS.Tables.Count > 0 &&
                        dsPROJECT_STEP_SURPLUS.Tables[0].Rows.Count > 0)
                    {
                        // 3、添加一般代製項目費用
                        prdModel.Unit_Price = Convert.ToDecimal(dsPROJECT_STEP_SURPLUS.Tables[0].Rows[0]["Price"]);
                        prdModel.Number = Convert.ToInt32(dr["Number"]);
                        prdModel.Sum = prdModel.Unit_Price * prdModel.Number;
                        prdModel.Use_Date = Date;
                        prdModel.Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
                        prdModel.Card_Group_RID = Convert.ToInt32(dr["CARDGROUPRID"]);
                        prdModel.CardType_RID = Convert.ToInt32(dr["RID"]);
                        prdModel.Project_RID = Convert.ToInt32(dsPROJECT_STEP_SURPLUS.Tables[0].Rows[0]["RID"]);
                        dao.Add<PERSO_PROJECT_DETAIL>(prdModel, "RID");
                    }
                }    
            }

            #region 計算代製費用是否超出安全值，如果查過，警示

            string Year = Date.Year.ToString();
            //計算代製費用總計（卡）
            int CostSumCard = 0;
            //計算代製費用總計（銀）
            int CostSumBank = 0;

            dirValues.Clear();
            dirValues.Add("year", Year);
            // 計算本年度特殊代製費用總計
            DataSet dsSPECIAL_PROJECT_COST = dao.GetList(SEL_SPECIAL_PROJECT_COST, dirValues);
            // 計算本年度例外代製費用總計
            DataSet dsEXCEPTION_PROJECT_COST = dao.GetList(SEL_EXCEPTION_PROJECT_COST, dirValues);
            // 計算本年度代製異動費用總計劃
            DataSet dsPERSO_PROJECT_CHANGE_DETAIL = dao.GetList(SEL_PERSO_PROJECT_CHANGE_DETAIL, dirValues);
            // 取一般代製項目費用
            DataSet dsPERSO_PROJECT_NORMAL = dao.GetList(SEL_PERSO_PROJECT_NORMAL, dirValues);
            // 物料代製費用年度預算，9：代製費用 （卡）
            DataSet dsMATERIEL_BUDGET_SUM_CARD = dao.GetList(SEL_MATERIEL_BUDGET_SUM_CARD, dirValues);
            // 物料代製費用年度預算，10：代製費用（銀） 
            DataSet dsMATERIEL_BUDGET_SUM_BANK = dao.GetList(SEL_MATERIEL_BUDGET_SUM_BANK, dirValues);

            // 計算代製費用總計（卡） = 特殊項目代製費用 + 
            //                        (磁條信用卡、晶片信用卡、VISA DEBIT卡群組)的一般項目代製費用 + 
            //                        (磁條信用卡、晶片信用卡、VISA DEBIT卡群組)的例外項目代製費用 - 
            //                        (磁條信用卡、晶片信用卡、VISA DEBIT卡群組)的帳務異動費用  
            // 計算代製費用總計（銀） = (晶片金融卡和現金卡的群組)的一般項目代製費用 + 
            //                        (晶片金融卡和現金卡的群組)的例外項目代製費用 - 
            //                        (晶片金融卡和現金卡的群組)的帳務異動費用  

            // 特殊項目代製費用
            if (dsSPECIAL_PROJECT_COST.Tables[0].Rows.Count != 0)
            {
                if (dsSPECIAL_PROJECT_COST.Tables[0].Rows[0][0].ToString() != "")
                {
                    CostSumCard = Convert.ToInt32(dsSPECIAL_PROJECT_COST.Tables[0].Rows[0][0]);
                }
            }
            
            // 一般項目代製費用
            foreach (DataRow dr in dsPERSO_PROJECT_NORMAL.Tables[0].Rows)
            {
                if (dr["Group_Name"].ToString() == "磁條信用卡" || 
                    dr["Group_Name"].ToString() == "晶片信用卡" || 
                    dr["Group_Name"].ToString() == "VISA DEBIT卡")
                {
                    CostSumCard += Convert.ToInt32(dr[1]);
                }
                else if (dr["Group_Name"].ToString() == "晶片金融卡" ||
                        dr["Group_Name"].ToString() == "現金卡")
                {
                    CostSumBank += Convert.ToInt32(dr[1]);
                }
            }
            // 例外項目的代製費用
            foreach (DataRow dr in dsEXCEPTION_PROJECT_COST.Tables[0].Rows)
            {
                if (dr["Group_Name"].ToString() == "磁條信用卡" ||
                    dr["Group_Name"].ToString() == "晶片信用卡" ||
                    dr["Group_Name"].ToString() == "VISA DEBIT卡")
                {
                    CostSumCard += Convert.ToInt32(dr[1]);
                }
                else if (dr["Group_Name"].ToString() == "晶片金融卡" ||
                        dr["Group_Name"].ToString() == "現金卡")
                {
                    CostSumBank += Convert.ToInt32(dr[1]);
                }
            }

            // 帳務異動
            foreach (DataRow dr in dsPERSO_PROJECT_CHANGE_DETAIL.Tables[0].Rows)
            {
                if (dr["Group_Name"].ToString() == "磁條信用卡" || 
                    dr["Group_Name"].ToString() == "晶片信用卡" || 
                    dr["Group_Name"].ToString() == "VISA DEBIT卡")
                {
                    CostSumCard += Convert.ToInt32(dr[1]);
                }
                else if (dr["Group_Name"].ToString() == "晶片金融卡" ||
                        dr["Group_Name"].ToString() == "現金卡")
                {
                    CostSumBank += Convert.ToInt32(dr[1]);
                }
            }

            // 檢查代製費用卡是否需要報警
            if (null != dsMATERIEL_BUDGET_SUM_CARD &&
                dsMATERIEL_BUDGET_SUM_CARD.Tables.Count > 0 &&
                dsMATERIEL_BUDGET_SUM_CARD.Tables[0].Rows.Count > 0)
            {
                Decimal intMATERIEL_BUDGET_SUM_CARD = Convert.ToDecimal(dsMATERIEL_BUDGET_SUM_CARD.Tables[0].Rows[0]["Budget"]);
                if ((intMATERIEL_BUDGET_SUM_CARD - CostSumCard) < intMATERIEL_BUDGET_SUM_CARD * System.Decimal.Parse("0.1"))
                {
                    strAlert = "\\n代製費用（卡）年度預算剩餘金額低於10%";

                    object[] arg = new object[1];
                    arg[0] = "代製費用（卡）";
                    Warning.SetWarning(GlobalString.WarningType.SurplusMaterialBuget,arg);
                }
            }

            if (null != dsMATERIEL_BUDGET_SUM_BANK &&
                    dsMATERIEL_BUDGET_SUM_BANK.Tables.Count > 0 &&
                    dsMATERIEL_BUDGET_SUM_BANK.Tables[0].Rows.Count > 0)
            {
                Decimal intMATERIEL_BUDGET_SUM_BANK = Convert.ToDecimal(dsMATERIEL_BUDGET_SUM_BANK.Tables[0].Rows[0]["Budget"]);
                if ((intMATERIEL_BUDGET_SUM_BANK - CostSumBank) < intMATERIEL_BUDGET_SUM_BANK * System.Decimal.Parse("0.1"))
                {
                    if (strAlert != "")
                    {
                        strAlert += "\\n代製費用（銀）年度預算剩餘金額低於10%";
                    }
                    else
                    {
                        strAlert = "\\n代製費用（銀）年度預算剩餘金額低於10%";
                    }

                    object[] arg = new object[1];
                    arg[0] = "代製費用（銀）";
                    Warning.SetWarning(GlobalString.WarningType.SurplusMaterialBuget, arg);
                }
            }

            return strAlert;

            #endregion

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

    /// <summary>
    /// 將相關記錄標記設置為日結
    /// </summary>
    /// <returns></returns>
    public void setDaySurplus(DateTime Date)
    {
        CARDTYPE_STOCKS csModel = new CARDTYPE_STOCKS();
        try
        {
            dirValues.Clear();
            dirValues.Add("check_date", Date);
            dirValues.Add("check_date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("check_date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            //將日結日期當天的入庫的標識為日結
            dao.ExecuteNonQuery(UPDATE_DEPOSITORY_STOCK, dirValues);
            //將日結日期當天的再入庫的標識為日結
            dao.ExecuteNonQuery(UPDATE_DEPOSITORY_RESTOCK, dirValues);
            //將日結日期當天的退貨的標識為日結
            dao.ExecuteNonQuery(UPDATE_DEPOSITORY_CANCEL, dirValues);
            //將日結日期當天的小計檔的標識為日結
            dao.ExecuteNonQuery(UPDATE_SUBTOTAL_IMPORT, dirValues);
            //將日結日期當天的廠商異動的標識為日結
            dao.ExecuteNonQuery(UPDATE_FACTORY_CHANGE_IMPORT, dirValues);

            //200908CR物料消耗，代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 start
            //將日結日期當天的替換前版面小計檔的標識為日結            
            dao.ExecuteNonQuery(UPDATE_SUBTOTAL_REPLACE_IMPORT, dirValues);
            //將日結日期當天的替換前版面廠商異動的標識為日結
            dao.ExecuteNonQuery(UPDATE_FACTORY_CHANGE_REPLACE_IMPORT, dirValues);
            //200908CR物料消耗，代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 end

            //將日結日期當天的卡片移動的標識為日結
            dao.ExecuteNonQuery(UPDATE_CARDTYPE_STOCKS_MOVE, dirValues);
            //保存卡種庫存檔
            DataSet dsCARDTYPE_STOCKS = dao.GetList(SEL_CARDTYPE_STOCKS, dirValues);
            foreach (DataRow dr in dsCARDTYPE_STOCKS.Tables[0].Rows)
            {
                csModel.Stock_Date = Convert.ToDateTime(dr["Date_Time"]);
                csModel.Stocks_Number = Convert.ToInt32(dr["Number"]);
                csModel.Perso_Factory_RID = Convert.ToInt32(dr["Perso_Factory_RID"]);
                csModel.CardType_RID = Convert.ToInt32(dr["RID"]);
                dao.Add<CARDTYPE_STOCKS>(csModel, "RID");
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

    /// <summary>
    /// 檢查該記錄是否已經保存,如果Count == 0不存在,
    /// 在晶片規格變化回滾檔（WAFER_USELOG_ROLLBACK）中添加，晶片規格變化表中的訊息
    /// </summary>
    /// <returns></returns>
    public void saveWafer_Uselog_Rollback(int WAFER_RID, DateTime Date)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("uselog_rid", WAFER_RID);
            dirValues.Add("check_date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("check_date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            DataSet dsWAFER_USELOG_ROLLBACK = dao.GetList(CON_WAFER_USELOG_ROLLBACK, dirValues);
            // 檢查該記錄是否已經保存,如果Count == 0不存在,在晶片規格變化回滾檔（WAFER_USELOG_ROLLBACK）中添加，晶片規格變化表中的訊息
            // 每天只保存一份。
            if (null != dsWAFER_USELOG_ROLLBACK &&
                dsWAFER_USELOG_ROLLBACK.Tables.Count >0 &&
                dsWAFER_USELOG_ROLLBACK.Tables[0].Rows.Count >0)
            {
                if (Convert.ToInt32(dsWAFER_USELOG_ROLLBACK.Tables[0].Rows[0][0].ToString()) == 0)
                {
                    dirValues.Add("check_date", Date);
                    // 在晶片規格變化回滾檔中保存晶片規格變化表的訊息
                    dao.ExecuteNonQuery(INSERT_WAFER_USELOG_ROLLBACK, dirValues);    
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 檢查取消日結是否符合條件
    /// </summary>
    /// <returns></returns>
    public bool CancelCheck(DateTime Date)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("date_start", Date.ToString("yyyy/MM/dd 00:00:00"));
            dirValues.Add("date_end", Date.ToString("yyyy/MM/dd 23:59:59"));
            // 系統檢查輸入日期是否已日結,如果未日結返回false;
            DataSet dsDATE_SURPLUS_CHECK = dao.GetList(CON_DATE_SURPLUS_CHECK, dirValues);
            if (dsDATE_SURPLUS_CHECK.Tables[0].Rows[0][0].ToString() == "0")
                throw new AlertException("要取消日結日期還未日結。");

            // 日結日期后是否還有日結。
            DataSet dsDATE_SURPLUS_CHECK_MAX = dao.GetList(CON_DATE_SURPLUS_CHECK_MAX, dirValues);
            if (dsDATE_SURPLUS_CHECK_MAX.Tables[0].Rows[0][0].ToString() != "0")
                throw new AlertException("要取消日結日期不是最后一次日結日期。");

            // 可以日結
            return true;
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

    /// <summary>
    /// 將相關記錄標記設置為日結
    /// </summary>
    /// <returns></returns>
    public void CancelDaySurplus(DateTime Date)
    {
        try
        {
            dao.OpenConnection();

            dirValues.Clear();
            dirValues.Add("check_date_start", Date.ToString("yyyy-MM-dd 00:00:00"));
            dirValues.Add("check_date_end", Date.ToString("yyyy-MM-dd 23:59:59"));
            //刪除卡種庫存檔
            dao.ExecuteNonQuery(DEL_CARDTYPE_STOCKS, dirValues);
            //刪除當天的物料耗用擋
            dao.ExecuteNonQuery(DEL_MATERIEL_STOCKS_USED, dirValues);
            //將日結日期當天的入庫的標識為未日結
            dao.ExecuteNonQuery(UPDATE_DEPOSITORY_STOCK_CANCEL, dirValues);
            //將日結日期當天的再入庫的標識為未日結
            dao.ExecuteNonQuery(UPDATE_DEPOSITORY_RESTOCK_CANCEL, dirValues);
            //將日結日期當天的退貨的標識為未日結
            dao.ExecuteNonQuery(UPDATE_DEPOSITORY_CANCEL1, dirValues);
            //將日結日期當天的小計檔的標識為未日結
            dao.ExecuteNonQuery(UPDATE_SUBTOTAL_IMPORT_CANCEL, dirValues);
            //將日結日期當天的廠商異動的標識為未日結
            dao.ExecuteNonQuery(UPDATE_FACTORY_CHANGE_IMPORT_CANCEL, dirValues);
            //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 start
            //將日結日期當天的替換前小計檔的標識為未日結
            dao.ExecuteNonQuery(UPDATE_SUBTOTAL_REPLACE_IMPORT_CANCEL, dirValues);
            //CR將日結日期當天的替換前廠商異動的標識為未日結
            dao.ExecuteNonQuery(UPDATE_FACTORY_CHANGE_REPLACE_IMPORT_CANCEL, dirValues);
            //200908CR代制費用計算改為用小計檔的「替換前」版面計算 add  by 楊昆 2009/09/01 end
            //將日結日期當天的卡片移動的標識為未日結
            dao.ExecuteNonQuery(UPDATE_CARDTYPE_STOCKS_MOVE_CANCEL, dirValues);

            //將晶片規格變化表還原
            //刪除新增的晶片規格變化表
            dao.ExecuteNonQuery(DEL_WAFER_CARDTYPE_USELOG, dirValues);
            //將修改的晶片規格變化恢復
            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_CANCEL, dirValues);
            //將晶片規格變化表還原日結當天的刪除
            dao.ExecuteNonQuery(DEL_WAFER_USELOG_ROLLBACK_CANCEL, dirValues);

            SetOprLog("6");

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
    }
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public DateTime getWAFERCARDTYPEDate()
    {
        DateTime dtFirstSurplusDat = DateTime.Today.AddDays(-1);
        try
        {
            DataSet dsLAST_SURPLUS_DAY = dao.GetList("select top 1 Check_date from WAFER_USELOG_ROLLBACK order by Check_date desc ");
            if (dsLAST_SURPLUS_DAY != null
                    && dsLAST_SURPLUS_DAY.Tables.Count > 0
                    && dsLAST_SURPLUS_DAY.Tables[0].Rows.Count > 0)
            {
                dtFirstSurplusDat = Convert.ToDateTime(dsLAST_SURPLUS_DAY.Tables[0].Rows[0]["Check_date"].ToString());
            }
            return dtFirstSurplusDat;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 將晶片規格變化表還原
    /// </summary>
    /// <returns></returns>
    public void CancelWAFERCARDTYPE(DateTime Date)
    {
        try
        {
            dao.OpenConnection();

            dirValues.Clear();
            dirValues.Add("check_date_start", Date.ToString("yyyy-MM-dd 00:00:00"));
            dirValues.Add("check_date_end", Date.ToString("yyyy-MM-dd 23:59:59"));          
            //將晶片規格變化表還原
            //刪除新增的晶片規格變化表
            dao.ExecuteNonQuery(DEL_WAFER_CARDTYPE_USELOG, dirValues);
            //將修改的晶片規格變化恢復
            dao.ExecuteNonQuery(UPDATE_WAFER_CARDTYPE_USELOG_CANCEL, dirValues);
            //將晶片規格變化表還原日結當天的刪除
            dao.ExecuteNonQuery(DEL_WAFER_USELOG_ROLLBACK_CANCEL, dirValues);

            SetOprLog("6");

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
    }
   
    /// <summary>
    /// 初始化晶片規格變化檔
    /// </summary>
    /// <returns></returns>
    public void DEL_Wafer_Uselog()
    {
        string DEL = " delete FROM WAFER_CARDTYPE_USELOG where Income_Date > @Income_Date ";
        string Update = " update WAFER_CARDTYPE_USELOG set Usable_Number=Number where Income_Date= @Income_Date ";
        DateTime Income_Date = Convert.ToDateTime("2009-04-24");
        try
        {
            dirValues.Clear();

            dirValues.Add("Income_Date", Income_Date);
            dao.ExecuteNonQuery(Update, dirValues);
            dao.ExecuteNonQuery(DEL, dirValues);

            
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 卡種晶片規格變化表的處理
    /// </summary>
    /// <returns></returns>
    public void DoWaferUsedLog(DateTime Date)
    {
        try
        {
            dao.OpenConnection();
            // 卡種晶片規格變化表的處理
            getWaferUsedLog(Date);


            SetOprLog("5");

            // 提交事務
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
    }
}
