//******************************************************************
//*  作    者：James
//*  功能說明：物料庫存管控邏輯
//*  創建日期：2008-09-20
//*  修改日期：2008-09-20 12:00
//*  修改記錄：
//*            □2008-09-20
//*              1.創建 占偉林
//*              2.修改 潘秉奕(行281、849)
//*            □2009-08-31
//*              修改 楊昆 fro 物料的消耗計算改為用小計檔的「替換前」版面計算
//*              
//*******************************************************************

using System;
using System.Data;
using System.Collections;
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
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.IO;
using CIMSClass.Business;


/// <summary>
/// 物料庫存控管作業
/// </summary>
public class Depository010BL : BaseLogic
{
    #region SQL語句
    public const string SEL_FACTORY_ALL = "SELECT RID,Factory_ShortName_CN "
                                    + "FROM FACTORY "
                                    + "WHERE RST = 'A' AND Is_Perso = 'Y' "
                                    + "ORDER BY RID";
    public const string SEL_MATERIEL_LAST_SURPLUS_DATE = "SELECT TOP 1 Stock_Date,Number "
                                    + "FROM (SELECT MSM.Serial_Number,Stock_Date,MSM.Number "
                                    + "FROM MATERIEL_STOCKS_MANAGE MSM LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MSM.Serial_Number = CE.Serial_Number "
                                    + "LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MSM.Serial_Number = EI.Serial_Number "
                                    + "LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MSM.Serial_Number = DM.Serial_Number "
                                    + "WHERE MSM.RST = 'A' AND MSM.Perso_Factory_RID = @perso_factory_rid AND Type = '4') A "
                                    + "WHERE Serial_Number = @serial_number "
                                    + "ORDER BY Stock_Date DESC";
    public const string SEL_LAST_SURPLUS_DATE = "select top 1 Convert(varchar(50),RCT,111) RCT from (select EI.RCT from ENVELOPE_INFO EI where EI.Serial_Number=@Serial_Number union select DM.RCT from DMTYPE_INFO DM where DM.Serial_Number=@Serial_Number union select CE.RCT from CARD_EXPONENT CE where CE.Serial_Number=@Serial_Number) a order by RCT";
    public const string SEL_MATERIEL_SURPLUS_DATE = "SELECT Stock_Date "
                                    + "FROM (SELECT MS.Serial_Number,Stock_Date "
                                    + "FROM MATERIEL_STOCKS MS LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MS.Serial_Number = CE.Serial_Number "
                                    + "LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MS.Serial_Number = EI.Serial_Number "
                                    + "LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MS.Serial_Number = DM.Serial_Number "
                                    + "WHERE MS.RST = 'A' AND MS.Perso_Factory_RID = @perso_factory_rid) A "
                                    + "WHERE Serial_Number = @serial_number "
                                    + "ORDER BY Stock_Date DESC";
    public const string SEL_ENVELOPE_INFO = "SELECT * "
                                    + "FROM ENVELOPE_INFO "
                                    + "WHERE RST = 'A' AND Serial_Number = @serial_number";
    public const string SEL_CARD_EXPONENT = "SELECT * "
                                    + "FROM CARD_EXPONENT "
                                    + "WHERE RST = 'A' AND Serial_Number = @serial_number";
    public const string SEL_DMTYPE_INFO = "SELECT * "
                                    + "FROM DMTYPE_INFO "
                                    + "WHERE RST = 'A' AND Serial_Number = @serial_number";
    public const string SEL_MATERIEL_STOCKS_MANAGE = "SELECT TOP 1 * "
                                    + "FROM MATERIEL_STOCKS_MANAGE "
                                    + "WHERE RST = 'A' AND TYPE = '4' AND Perso_Factory_RID = @perso_factory_rid "
                                    + " AND Materiel_RID = @materiel_rid AND Materiel_Type = @materiel_type "
                                    + "ORDER BY Stock_Date DESC";
    public const string SEL_MATERIEL_STOCKS_MOVE_IN = "SELECT Move_Date,Move_Number "
                                    + "FROM MATERIEL_STOCKS_MOVE "
                                    + "WHERE RST = 'A' AND To_Factory_RID = @perso_factory_rid "
                                    + "AND Serial_Number = @Serial_Number "
                                    + "AND Move_Date > @lastSurplusDateTime "
                                    + "AND Move_Date <= @thisSurplusDateTime ";
    public const string SEL_MATERIEL_STOCKS_MOVE_OUT = "SELECT Move_Date,Move_Number "
                                    + "FROM MATERIEL_STOCKS_MOVE "
                                    + "WHERE RST = 'A' AND From_Factory_RID = @perso_factory_rid "
                                    + "AND Serial_Number = @Serial_Number "
                                    + "AND Move_Date > @lastSurplusDateTime "
                                    + "AND Move_Date <= @thisSurplusDateTime ";
    public const string SEL_CARD_INFO_ENVELOPE = "SELECT CT.RID "
                                    + "FROM Card_Type CT "
                                    + "WHERE CT.RST = 'A' AND CT.Envelope_RID = @envelope_rid ";
    public const string SEL_CARD_INFO_EXPONENT = "SELECT CT.RID "
                                    + "FROM Card_Type CT "
                                    + "WHERE CT.RST = 'A' AND CT.Exponent_RID = @exponent_rid ";
    public const string SEL_CARD_INFO_DM = "SELECT CT.RID "
                                    + "FROM DM_CARDTYPE DC INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND DC.CardType_RID = CT.RID "
                                    + "WHERE DC.RST = 'A' AND DC.DM_RID = @dm_rid ";
    public const string SEL_SUBTOTAL_IMPORT = "SELECT SI.PHOTO,SI.Number,SI.TYPE,SI.Affinity,SI.Date_Time,CT.RID "
                                    + "FROM SUBTOTAL_IMPORT AS SI "
                                    + "left join card_type as CT "
                                    + "on ct.rst='A' and ct.photo=si.photo and ct.type=si.type and ct.affinity=si.affinity "
                                    + "WHERE SI.RST = 'A' AND SI.Perso_Factory_RID = @perso_factory_rid "
                                    + "AND SI.Date_Time > @lastSurplusDateTime "
                                    + "AND SI.Date_Time <= @thisSurplusDateTime ";

    public const string GET_ENVELOPE_INFO_WEARRATE = "SELECT EI.Wear_Rate "
                                    + "FROM CARD_TYPE CT INNER JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID "
                                    + "WHERE CT.RST = 'A' AND CT.TYPE = @total_type AND CT.PHOTO = @photo AND CT.Affinity = @Affinity ";
    public const string GET_CARD_EXPONENT_WEARRATE = "SELECT CE.Wear_Rate "
                                    + "FROM CARD_TYPE CT INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID "
                                    + "WHERE CT.RST = 'A' AND CT.TYPE = @total_type AND CT.PHOTO = @photo AND CT.Affinity = @Affinity ";
    public const string GET_DMTYPE_INFO_WEARRATE = "SELECT DM.Wear_Rate "
                                    + "FROM DM_CARDTYPE DC INNER JOIN CARD_TYPE CT ON DC.RST = 'A' AND CT.RST = 'A' AND DC.CardType_RID = CT.RID "
                                    + "INNER JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND DC.DM_RID = DM.RID "
                                    + "WHERE CT.TYPE = @total_type AND CT.PHOTO = @photo AND CT.Affinity = @Affinity ";
    public const string GET_LAST_SURPLUS_BY_FACTORY = "SELECT TOP 1 * "
                                    + "FROM MATERIEL_STOCKS_MANAGE "
                                    + "WHERE RST = 'A' AND Perso_Factory_RID = @perso_factory_rid AND Type = 4 "
                                    + "ORDER BY Stock_Date DESC";
    public const string GET_SURPLUS_BY_FACTORY = "SELECT distinct Serial_Number "
                                    + "FROM MATERIEL_STOCKS_MANAGE "
                                    + "WHERE RST = 'A' AND Perso_Factory_RID = @perso_factory_rid AND Type = 4 ";
    public const string DEL_MATERIEL_STOCKS_MANAGE = "DELETE "
                                    + "FROM MATERIEL_STOCKS_MANAGE "
                                    + "WHERE RST = 'A' AND RID = @rid ";
    public const string DEL_MATERIEL_STOCKS_SYS = "DELETE "
                                        + "FROM MATERIEL_STOCKS "
                                        + "WHERE RST = 'A' "
                                        + "AND Perso_Factory_RID = @perso_factory_rid "
                                        + "AND Serial_Number = @Serial_Number "
                                        + "AND Stock_Date > @lastSurplusDateTime ";
    //+ "AND Stock_Date <= @thisSurplusDateTime ";
    public const string DEL_MATERIEL_STOCKS_MANAGE_DEL = "DELETE "
                                        + "FROM MATERIEL_STOCKS_MANAGE "
                                        + "WHERE RST = 'A' "
                                        + "AND Perso_Factory_RID = @perso_factory_rid "
                                        + "AND Serial_Number = @Serial_Number "
                                        + "AND Stock_Date > @lastSurplusDateTime ";
    public const string DEL_MATERIEL_STOCKS_DEL = "DELETE "
                                        + "FROM MATERIEL_STOCKS "
                                        + "WHERE RST = 'A' "
                                        + "AND Perso_Factory_RID = @perso_factory_rid "
                                        + "AND Serial_Number = @Serial_Number "
                                        + "AND Stock_Date > @lastSurplusDateTime ";
    public const string SEL_MATERIEL_STOCKS_USED = "select * from MATERIEL_STOCKS_USED where rst='A' "
                                        + "AND Serial_Number=@Serial_Number "
                                        + "AND Perso_Factory_RID=@Perso_Factory_RID "
                                        + "AND Stock_Date > @lastSurplusDateTime "
                                        + "AND Stock_Date <= @thisSurplusDateTime";

    public const string SEL_MATERIAL_USED_ENVELOPE = "SELECT SI.Date_Time AS Stock_Date,EI.Serial_Number,SI.Number "
            + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
            + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
            + "INNER JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID "
            + "WHERE SI.RST = 'A' AND Perso_Factory_RID = @Perso_Factory_RID AND Date_Time>@From_Date_Time AND Date_Time<=@End_Date_Time AND EI.Serial_Number = @Serial_Number";
    public const string SEL_MATERIAL_USED_CARD_EXPONENT = "SELECT SI.Date_Time AS Stock_Date,CE.Serial_Number,SI.Number "
            + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
            + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
            + "INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID "
            + "WHERE SI.RST = 'A' AND Perso_Factory_RID = @Perso_Factory_RID AND Date_Time>@From_Date_Time AND Date_Time<=@End_Date_Time AND CE.Serial_Number = @Serial_Number";

    //選取DM消耗！
    //public const string SEL_MATERIAL_USED_DM = "SELECT SI.Date_Time AS Stock_Date,DI.Serial_Number,SI.Number "
    //        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
    //        + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
    //        + "INNER JOIN DM_CARDTYPE DCT ON DCT.RST = 'A' AND CT.RID = DCT.CardType_RID "
    //        + "INNER JOIN DMTYPE_INFO DI ON DI.RST = 'A' AND DCT.DM_RID = DI.RID "
    //        + "WHERE SI.RST = 'A' AND Perso_Factory_RID = @Perso_Factory_RID AND Date_Time>@From_Date_Time AND Date_Time<=@End_Date_Time AND DI.Serial_Number = @Serial_Number";

    public const string SEL_MATERIAL_USED_DM = "SELECT A.Date_Time as Stock_Date, DI.Serial_Number, A.Number1 AS Number "
                    + " FROM (SELECT CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID "
                    + " FROM  SUBTOTAL_IMPORT  SI "
                    + " INNER JOIN CARD_TYPE  CT "
                    + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                    + " WHERE (SI.RST = 'A') AND (SI.Date_Time > @From_Date_Time) AND (SI.Date_Time <= @End_Date_Time) AND   "
                    + " (SI.Perso_Factory_RID = @Perso_Factory_RID)) A "
                    + " INNER JOIN DM_MAKECARDTYPE  DMM ON DMM.MakeCardType_RID = A.MakeCardType_RID "
                    + " INNER JOIN DMTYPE_INFO DI ON DI.RID = DMM.DM_RID  "
                    + " WHERE (DI.Card_Type_Link_Type = '1') AND (DI.Serial_Number = @Serial_Number)  "
                    + " UNION  "
                    + " SELECT A_1.Date_Time as Stock_Date, DI.Serial_Number, A_1.Number1 AS Number  "
                    + " FROM (SELECT CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID  "
                    + " FROM   SUBTOTAL_IMPORT SI "
                    + " INNER JOIN CARD_TYPE  CT "
                    + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                    + " WHERE  (SI.RST = 'A') AND (SI.Date_Time > @From_Date_Time) "
                    + " AND (SI.Date_Time <= @End_Date_Time) AND (SI.Perso_Factory_RID = @Perso_Factory_RID)) A_1 "
                    + " INNER JOIN DM_MAKECARDTYPE DMM ON DMM.MakeCardType_RID = A_1.MakeCardType_RID "
                    + " INNER JOIN DMTYPE_INFO  DI ON DI.RID = DMM.DM_RID "
                    + " INNER JOIN DM_CARDTYPE  DCT "
                    + " ON DCT.RST = 'A' AND A_1.RID = DCT.CardType_RID AND DCT.DM_RID = DI.RID "
                    + " WHERE (DI.Card_Type_Link_Type = '2') AND (DI.Serial_Number = @Serial_Number)";

    public const string SEL_ALL_WORK_DATE = "SELECT Date_Time FROM WORK_DATE WHERE RST = 'A' AND Is_WorkDay = 'Y' Order by Date_Time";

    public const string SEL_BATCH_MANAGE = "SELECT COUNT(*) FROM BATCH_MANAGE WHERE (RID = 1 OR RID=4 OR RID = 5) AND Status = 'Y'";

    public string UPDATE_BATCH_MANAGE_START = "UPDATE BATCH_MANAGE SET Status = 'Y',RUU='Depository010BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 1 OR RID=4 OR RID = 5)";

    public string UPDATE_BATCH_MANAGE_END = "UPDATE BATCH_MANAGE SET Status = 'N',RUU='Depository010BL.cs',RUT='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE (RID = 1 OR RID=4 OR RID = 5)";


    public const string SEL_MATERIAL_USED_ENVELOPE_S = "SELECT SI.Perso_Factory_RID,SI.Date_Time AS Stock_Date,EI.Serial_Number,SI.Number "
           + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
           + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
           + "INNER JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID "
            + "WHERE SI.RST = 'A'  AND  Date_Time=@End_Date_Time ";

    public const string SEL_MATERIAL_USED_CARD_EXPONENT_S = "SELECT SI.Perso_Factory_RID,SI.Date_Time AS Stock_Date,CE.Serial_Number,SI.Number "
            + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
            + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
            + "INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID "
         + "WHERE SI.RST = 'A'  AND  Date_Time=@End_Date_Time ";

    public const string SEL_MATERIAL_USED_DM_S = "SELECT A.Perso_Factory_RID,A.Date_Time as Stock_Date, DI.Serial_Number, A.Number1 AS Number "
                       + " FROM (SELECT SI.Perso_Factory_RID,CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID "
                       + " FROM  SUBTOTAL_IMPORT  SI "
                       + " INNER JOIN CARD_TYPE  CT "
                       + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                       + " WHERE (SI.RST = 'A') AND  (SI.Date_Time = @End_Date_Time)   "
                       + " ) A "
                       + " INNER JOIN DM_MAKECARDTYPE  DMM ON DMM.MakeCardType_RID = A.MakeCardType_RID "
                       + " INNER JOIN DMTYPE_INFO DI ON DI.RID = DMM.DM_RID  "
                       + " WHERE (DI.Card_Type_Link_Type = '1')  "
                       + " UNION  "
                       + " SELECT A_1.Perso_Factory_RID,A_1.Date_Time as Stock_Date, DI.Serial_Number, A_1.Number1 AS Number  "
                       + " FROM (SELECT SI.Perso_Factory_RID,CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID  "
                       + " FROM   SUBTOTAL_IMPORT SI "
                       + " INNER JOIN CARD_TYPE  CT "
                       + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                       + " WHERE  (SI.RST = 'A')  "
                       + " AND (SI.Date_Time = @End_Date_Time) ) A_1 "
                       + " INNER JOIN DM_MAKECARDTYPE DMM ON DMM.MakeCardType_RID = A_1.MakeCardType_RID "
                       + " INNER JOIN DMTYPE_INFO  DI ON DI.RID = DMM.DM_RID "
                       + " INNER JOIN DM_CARDTYPE  DCT "
                       + " ON DCT.RST = 'A' AND A_1.RID = DCT.CardType_RID AND DCT.DM_RID = DI.RID "
                       + " WHERE (DI.Card_Type_Link_Type = '2') ";
    #region 200908CR物料的消耗計算改為用小計檔的「替換前」版面計算
    //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
    public const string SEL_MATERIAL_USED_ENVELOPE_REPLACE = "SELECT SI.Date_Time AS Stock_Date,EI.Serial_Number,SI.Number "
           + "FROM SUBTOTAL_REPLACE_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
           + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
           + "INNER JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID "
           + "WHERE SI.RST = 'A' AND Perso_Factory_RID = @Perso_Factory_RID AND Date_Time>@From_Date_Time AND Date_Time<=@End_Date_Time AND EI.Serial_Number = @Serial_Number";

    public const string SEL_MATERIAL_USED_CARD_EXPONENT_REPLACE = "SELECT SI.Date_Time AS Stock_Date,CE.Serial_Number,SI.Number "
            + "FROM SUBTOTAL_REPLACE_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
            + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
            + "INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID "
            + "WHERE SI.RST = 'A' AND Perso_Factory_RID = @Perso_Factory_RID AND Date_Time>@From_Date_Time AND Date_Time<=@End_Date_Time AND CE.Serial_Number = @Serial_Number";


    public const string SEL_MATERIAL_USED_DM_REPLACE = "SELECT A.Date_Time as Stock_Date, DI.Serial_Number, A.Number1 AS Number "
                    + " FROM (SELECT CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID "
                    + " FROM  SUBTOTAL_REPLACE_IMPORT  SI "
                    + " INNER JOIN CARD_TYPE  CT "
                    + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                    + " WHERE (SI.RST = 'A') AND (SI.Date_Time > @From_Date_Time) AND (SI.Date_Time <= @End_Date_Time) AND   "
                    + " (SI.Perso_Factory_RID = @Perso_Factory_RID)) A "
                    + " INNER JOIN DM_MAKECARDTYPE  DMM ON DMM.MakeCardType_RID = A.MakeCardType_RID "
                    + " INNER JOIN DMTYPE_INFO DI ON DI.RID = DMM.DM_RID  "
                    + " WHERE (DI.Card_Type_Link_Type = '1') AND (DI.Serial_Number = @Serial_Number)  "
                    + " UNION  "
                    + " SELECT A_1.Date_Time as Stock_Date, DI.Serial_Number, A_1.Number1 AS Number  "
                    + " FROM (SELECT CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID  "
                    + " FROM   SUBTOTAL_REPLACE_IMPORT SI "
                    + " INNER JOIN CARD_TYPE  CT "
                    + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                    + " WHERE  (SI.RST = 'A') AND (SI.Date_Time > @From_Date_Time) "
                    + " AND (SI.Date_Time <= @End_Date_Time) AND (SI.Perso_Factory_RID = @Perso_Factory_RID)) A_1 "
                    + " INNER JOIN DM_MAKECARDTYPE DMM ON DMM.MakeCardType_RID = A_1.MakeCardType_RID "
                    + " INNER JOIN DMTYPE_INFO  DI ON DI.RID = DMM.DM_RID "
                    + " INNER JOIN DM_CARDTYPE  DCT "
                    + " ON DCT.RST = 'A' AND A_1.RID = DCT.CardType_RID AND DCT.DM_RID = DI.RID "
                    + " WHERE (DI.Card_Type_Link_Type = '2') AND (DI.Serial_Number = @Serial_Number)";

    public const string SEL_MATERIAL_USED_ENVELOPE_S_REPLACE = "SELECT SI.Perso_Factory_RID,SI.Date_Time AS Stock_Date,EI.Serial_Number,SI.Number "
          + "FROM SUBTOTAL_REPLACE_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
          + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
          + "INNER JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND CT.Envelope_RID = EI.RID "
           + "WHERE SI.RST = 'A'  AND  Date_Time=@End_Date_Time ";

    public const string SEL_MATERIAL_USED_CARD_EXPONENT_S_REPLACE = "SELECT SI.Perso_Factory_RID,SI.Date_Time AS Stock_Date,CE.Serial_Number,SI.Number "
            + "FROM SUBTOTAL_REPLACE_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST ='A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
            + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name In ('3D','DA','PM','RN') "
            + "INNER JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND CT.Exponent_RID = CE.RID "
         + "WHERE SI.RST = 'A'  AND  Date_Time=@End_Date_Time ";

    public const string SEL_MATERIAL_USED_DM_S_REPLACE = "SELECT A.Perso_Factory_RID,A.Date_Time as Stock_Date, DI.Serial_Number, A.Number1 AS Number "
                       + " FROM (SELECT SI.Perso_Factory_RID,CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID "
                       + " FROM  SUBTOTAL_REPLACE_IMPORT  SI "
                       + " INNER JOIN CARD_TYPE  CT "
                       + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                       + " WHERE (SI.RST = 'A') AND  (SI.Date_Time = @End_Date_Time)   "
                       + " ) A "
                       + " INNER JOIN DM_MAKECARDTYPE  DMM ON DMM.MakeCardType_RID = A.MakeCardType_RID "
                       + " INNER JOIN DMTYPE_INFO DI ON DI.RID = DMM.DM_RID  "
                       + " WHERE (DI.Card_Type_Link_Type = '1')  "
                       + " UNION  "
                       + " SELECT A_1.Perso_Factory_RID,A_1.Date_Time as Stock_Date, DI.Serial_Number, A_1.Number1 AS Number  "
                       + " FROM (SELECT SI.Perso_Factory_RID,CT.RID, SI.Number AS Number1, SI.Date_Time, SI.MakeCardType_RID  "
                       + " FROM   SUBTOTAL_REPLACE_IMPORT SI "
                       + " INNER JOIN CARD_TYPE  CT "
                       + " ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO  "
                       + " WHERE  (SI.RST = 'A')  "
                       + " AND (SI.Date_Time = @End_Date_Time) ) A_1 "
                       + " INNER JOIN DM_MAKECARDTYPE DMM ON DMM.MakeCardType_RID = A_1.MakeCardType_RID "
                       + " INNER JOIN DMTYPE_INFO  DI ON DI.RID = DMM.DM_RID "
                       + " INNER JOIN DM_CARDTYPE  DCT "
                       + " ON DCT.RST = 'A' AND A_1.RID = DCT.CardType_RID AND DCT.DM_RID = DI.RID "
                       + " WHERE (DI.Card_Type_Link_Type = '2') ";
    //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end
    #endregion

    //add by Ian Huang start
    public const string SEL_MATERIEL_STOCKS_TRANSACTION = @"SELECT MST.Transaction_Date,MST.Transaction_Amount,P.Param_Code 
                        FROM MATERIEL_STOCKS_TRANSACTION MST
                        inner join PARAM P on MST.PARAM_RID = P.RID
                        WHERE MST.RST = 'A' AND MST.Factory_RID = @perso_factory_rid 
                        AND MST.Serial_Number = @Serial_Number 
                        AND MST.Transaction_Date > @lastSurplusDateTime 
                        AND MST.Transaction_Date <= @thisSurplusDateTime ";

    public const string DELETE_MATERIEL_STOCKS_MANAGE = @"DELETE FROM MATERIEL_STOCKS_MANAGE 
                        where Stock_Date = @Stock_Date and RCU = @RCU and Perso_Factory_RID = @Perso_Factory_RID 
                        and Type = @Type and Serial_Number = @Serial_Number";

    public const string SEL_MATERIEL_STOCKS = @"select Number from MATERIEL_STOCKS
                        where Stock_date = @Stock_date and Serial_Number = @Serial_Number and Perso_Factory_RID = @Perso_Factory_RID";

    public const string SEL_MATERIEL_STOCKS_MOVE = @"select * from MATERIEL_STOCKS_MOVE
                        where Move_Date = @Move_Date and Serial_Number = @Serial_Number and Move_Number = @Move_Number ";

    public const string UPDATE_MATERIEL_STOCKS_MOVE_1 = @"update MATERIEL_STOCKS_MOVE set From_Factory_RID = 0
                        where From_Factory_RID = @Factory_RID and To_Factory_RID <> 0 and RCU = 'INPUT'";

    public const string UPDATE_MATERIEL_STOCKS_MOVE_2 = @"update MATERIEL_STOCKS_MOVE set To_Factory_RID = 0
                        where To_Factory_RID = @Factory_RID and From_Factory_RID <> 0 and RCU = 'INPUT'";

    public const string DEL_MATERIEL_STOCKS_MOVE_1 = @"delete from MATERIEL_STOCKS_MOVE
                        where From_Factory_RID = @Factory_RID and To_Factory_RID = 0 and RCU = 'INPUT'";

    public const string DEL_MATERIEL_STOCKS_MOVE_2 = @"delete from MATERIEL_STOCKS_MOVE
                        where To_Factory_RID = @Factory_RID and From_Factory_RID = 0 and RCU = 'INPUT'";

    public const string INSERT_MATERIEL_STOCKS_MOVE = @"INSERT INTO MATERIEL_STOCKS_MOVE
           (Move_Date,RCU,RUU,RCT,RUT,RST,Move_Number,From_Factory_RID,To_Factory_RID,Move_ID,Serial_Number) VALUES
           (@Move_Date,@RCU,@RUU,@RCT,@RUT,@RST,@Move_Number,@From_Factory_RID,@To_Factory_RID,@Move_ID,@Serial_Number)";
    //add by Ian Huang end
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Depository010BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 獲得Perso廠商
    /// </summary>
    /// <returns>DataTable[Perso廠商]</returns>
    public DataTable GetFactoryList()
    {
        DataSet dstFactory = null;
        try
        {
            this.dirValues.Clear();
            dstFactory = dao.GetList(SEL_FACTORY_ALL, dirValues);

            return dstFactory.Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 以廠商RID取廠商編號
    /// </summary>
    /// <param name="Factory_RID">廠商RID</param>
    /// <returns></returns>
    private string getFactoryIDByRID(string Factory_RID)
    {
        FACTORY mFACTORY = null;
        try
        {
            mFACTORY = dao.GetModel<FACTORY, int>("RID", int.Parse(Factory_RID));
            return mFACTORY.Factory_ID;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    public string getLAST_SURPLUS_DATE(string Serial_Number)
    {
        DataSet ds = null;
        dirValues.Clear();
        dirValues.Add("Serial_Number", Serial_Number);
        ds = dao.GetList(SEL_LAST_SURPLUS_DATE, dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }
    }

    private string getFactoryNameByRID(string Factory_RID)
    {
        FACTORY mFACTORY = null;
        try
        {
            mFACTORY = dao.GetModel<FACTORY, int>("RID", int.Parse(Factory_RID));
            return mFACTORY.Factory_ShortName_CN;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 獲得物料庫存管理信息
    /// </summary>
    /// <param name="msModel"></param>
    /// <returns></returns>
    public MATERIEL_STOCKS_MANAGE getMSMModel(MATERIEL_STOCKS msModel)
    {
        dirValues.Clear();
        dirValues.Add("perso_factory_rid", msModel.Perso_Factory_RID);
        dirValues.Add("serial_number", msModel.Serial_Number);
        dirValues.Add("stock_date", msModel.Stock_Date);
        MATERIEL_STOCKS_MANAGE msmModel = dao.GetModel<MATERIEL_STOCKS_MANAGE>("select * from MATERIEL_STOCKS_MANAGE where rst='A' and perso_factory_rid=@perso_factory_rid and serial_number=@serial_number and type='4' and stock_date=@stock_date", dirValues);

        return msmModel;
    }

    /// <summary>
    /// 檢查是否每一種物料都有上一次結余記錄，如果沒有，則新增一條結余為0的記錄！
    /// </summary>
    /// <param name="dstlMaterielStocksIn"></param>
    public void CheckMATERIEL_STOCKS(DataSet dstlMaterielStocksIn)
    {
        try
        {
            dao.OpenConnection();
            if (dstlMaterielStocksIn.Tables.Count != 2)
                return;

            if (dstlMaterielStocksIn.Tables[0].Rows.Count == 0)
                return;

            string strFactory = dstlMaterielStocksIn.Tables[0].Rows[0]["factory_rid"].ToString();

            //dstlMaterielStocksIn.Tables[1].DefaultView.Sort = "Serial_Number,Stock_Date";

            foreach (DataRow dr in dstlMaterielStocksIn.Tables[1].Rows)
            {
                DataTable dtbl = dao.GetList("select count(*) from dbo.MATERIEL_STOCKS_MANAGE where Serial_Number='" + dr["Serial_Number"].ToString() + "' and Perso_Factory_rid=" + strFactory).Tables[0];

                if (dtbl.Rows[0][0].ToString() == "0")
                {
                    MATERIEL_STOCKS_MANAGE msModel = new MATERIEL_STOCKS_MANAGE();
                    msModel.Number = 0;
                    msModel.Type = "4";
                    msModel.Perso_Factory_RID = int.Parse(strFactory);
                    msModel.RST = "A";
                    msModel.Serial_Number = dr["Serial_Number"].ToString();
                    msModel.Stock_Date = Convert.ToDateTime(dr["Stock_Date"]).Date.AddDays(-1);
                    dao.Add<MATERIEL_STOCKS_MANAGE>(msModel, "RID");
                }
            }


            dao.Commit();
        }
        catch
        {
            dao.Rollback();
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 匯入前對匯入文件進行格式檢查
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public DataSet CheckIn(string strPathFile, string strFactoryRID)
    {
        StreamReader sr = null;
        DataSet dstDataIn = null;
        if (!File.Exists(strPathFile))
        {
            throw new Exception("文件不存在！");
        }

        try
        {
            sr = new StreamReader(strPathFile, System.Text.Encoding.Default);

            string[] strLine;
            string strReadLine = "";
            int count = 1;
            string strErr = "";
            // add by Ian Huang start
            string strMaxDate = "";
            // add by Ian Huang end

            DataTable dtFactory = new DataTable();
            dtFactory.Columns.Add(new DataColumn("Factory_RID", Type.GetType("System.Int32")));
            dtFactory.Columns.Add(new DataColumn("Factory_ID", Type.GetType("System.String")));
            dtFactory.Columns.Add(new DataColumn("Factory_Name", Type.GetType("System.String")));

            DataTable dtDataIn = new DataTable();
            DataRow drIn2 = dtDataIn.NewRow(); 
            dtDataIn.Columns.Add(new DataColumn("Serial_Number", Type.GetType("System.String")));// 物品編號
            dtDataIn.Columns.Add(new DataColumn("Materiel_Name", Type.GetType("System.String")));// 物料品名
            dtDataIn.Columns.Add(new DataColumn("Stock_Date", Type.GetType("System.DateTime")));// 異動日期
            dtDataIn.Columns.Add(new DataColumn("Type", Type.GetType("System.Int32")));//異動類型
            dtDataIn.Columns.Add(new DataColumn("Number", Type.GetType("System.Int32")));// 檔案結餘數量
            dtDataIn.Columns.Add(new DataColumn("Materiel_RID", Type.GetType("System.Int32")));//物料RID
            dtDataIn.Columns.Add(new DataColumn("System_Num", Type.GetType("System.Int32")));// 庫存結餘數量
            dtDataIn.Columns.Add(new DataColumn("Last_Surplus_Date", Type.GetType("System.DateTime")));// 物品的上次結餘日期
            dtDataIn.Columns.Add(new DataColumn("Last_Surplus_Num", Type.GetType("System.Int32")));// 物品的上次結餘數量
            dtDataIn.Columns.Add(new DataColumn("Comment", Type.GetType("System.String")));// 備注
            // add by Ian Huang start
            dtDataIn.Columns.Add(new DataColumn("IsAdd", Type.GetType("System.String")));// 是否是自動新增
            // add by Ian Huang end

            #region 讀字符串，并檢查字符串格式(列數、每列的字符格式),并保存到臨時DataTable(dtDataIn)中
            while ((strReadLine = sr.ReadLine()) != null)
            {
                if (count == 1)
                {
                    // Perso廠一致性檢查
                    if (getFactoryIDByRID(strFactoryRID) != strReadLine.Trim())
                    {
                        throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_010_01);
                    }

                    // 保存Perso廠商訊息
                    DataRow drFactory = dtFactory.NewRow();
                    drFactory["Factory_RID"] = strFactoryRID;
                    drFactory["Factory_ID"] = strReadLine.Trim();
                    drFactory["Factory_Name"] = getFactoryNameByRID(strFactoryRID);
                    dtFactory.Rows.Add(drFactory);
                }
                else
                {
                    // 不是空的行
                    if (!StringUtil.IsEmpty(strReadLine))
                    {
                        if (StringUtil.GetByteLength(strReadLine) != 25)//列數量檢查
                        {
                            throw new AlertException("第" + count.ToString() + "行列數不正確。");
                        }

                        // 分割字符串
                        //strLine = strReadLine.Split(GlobalString.FileSplit.Split);
                        int nextBegin = 0;
                        Depository003BL bl003 = new Depository003BL();
                        strLine = new string[4];
                        strLine[0] = bl003.GetSubstringByByte(strReadLine, nextBegin, 6, out nextBegin).Trim();
                        strLine[1] = bl003.GetSubstringByByte(strReadLine, nextBegin, 8, out nextBegin).Trim();
                        strLine[2] = bl003.GetSubstringByByte(strReadLine, nextBegin, 2, out nextBegin).Trim();
                        strLine[3] = bl003.GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();

                        // 列長度檢查
                        for (int i = 0; i < strLine.Length; i++)
                        {
                            int num = i + 1;
                            if (StringUtil.IsEmpty(strLine[i]))
                                strErr += "第" + count.ToString() + "行第" + num.ToString() + "列為空;";
                            else
                                strErr += CheckFileOneColumn(strLine[i], num, count);

                            if (i == 0)
                            {
                                dirValues.Clear();
                                dirValues.Add("Serical_Number", strLine[i]);
                                if (strLine[i].Contains("A"))
                                {
                                    if (!dao.Contains("select count(*) from ENVELOPE_INFO where Serial_number=@Serical_Number", dirValues))
                                        strErr += "第" + count.ToString() + "行第" + num.ToString() + "列信封不存在;";
                                }
                                if (strLine[i].Contains("B"))
                                {
                                    if (!dao.Contains("select count(*) from CARD_EXPONENT where Serial_number=@Serical_Number", dirValues))
                                        strErr += "第" + count.ToString() + "行第" + num.ToString() + "列寄卡單不存在;";
                                }
                                if (strLine[i].Contains("C"))
                                {
                                    if (!dao.Contains("select count(*) from DMTYPE_INFO where Serial_number=@Serical_Number", dirValues))
                                        strErr += "第" + count.ToString() + "行第" + num.ToString() + "列DM不存在;";
                                }
                            }

                            if (i == 1)
                            {
                                DataTable dtblMax = dao.GetList("select convert(varchar(20),max(stock_date),112) from MATERIEL_STOCKS_MANAGE where Type='4' and Perso_Factory_RID = " + strFactoryRID).Tables[0];
                                if (dtblMax.Rows.Count > 0)
                                {
                                    if (!StringUtil.IsEmpty(dtblMax.Rows[0][0].ToString()))
                                    {
                                        if (int.Parse(dtblMax.Rows[0][0].ToString()) >= int.Parse(strLine[i]))
                                        {
                                            strErr += "第" + count.ToString() + "行第" + num.ToString() + "列日期不能小於最後結餘日期;";
                                        }
                                    }
                                }
                            }
                            if (!StringUtil.IsEmpty(strErr))
                                throw new AlertException(strErr);
                        }

                        // 將訊息新增到Table中
                        DataRow drIn = dtDataIn.NewRow();

                        if (!drIn2["Serial_Number"].ToString().Equals("") && !drIn2["Serial_Number"].ToString().Equals(strLine[0]) )
                        {
                            dtDataIn.Rows.Add(drIn2);
                            drIn2 = dtDataIn.NewRow();
                        } 

                        if (Convert.ToInt32(strLine[2]) == 4)
                        {
                        drIn2["Serial_Number"] = strLine[0];
                        drIn2["Type"] = Convert.ToInt32(strLine[2]);
                        drIn2["Stock_Date"] = Convert.ToDateTime(strLine[1].Substring(0, 4) + "/" + strLine[1].Substring(4, 2) + "/" + strLine[1].Substring(6, 2));
                        drIn2["Number"] = Convert.ToInt32(strLine[3]);
                        drIn2["IsAdd"] = "N";
                     //  

                        strMaxDate = "" == strMaxDate ? strLine[1] : int.Parse(strMaxDate) > int.Parse(strLine[1]) ? strMaxDate : strLine[1];
                    }
                        else { 
                        drIn["Serial_Number"] = strLine[0];
                        drIn["Type"] = Convert.ToInt32(strLine[2]);
                        drIn["Stock_Date"] = Convert.ToDateTime(strLine[1].Substring(0, 4) + "/" + strLine[1].Substring(4, 2) + "/" + strLine[1].Substring(6, 2));
                        drIn["Number"] = Convert.ToInt32(strLine[3]);
                        drIn["IsAdd"] = "N";
                        dtDataIn.Rows.Add(drIn);
                        // add by Ian Huang start
                        strMaxDate = "" == strMaxDate ? strLine[1] : int.Parse(strMaxDate) > int.Parse(strLine[1]) ? strMaxDate : strLine[1];                        }


                        //drIn["Serial_Number"] = strLine[0];
                        //drIn["Type"] = Convert.ToInt32(strLine[2]);
                        //drIn["Stock_Date"] = Convert.ToDateTime(strLine[1].Substring(0, 4) + "/" + strLine[1].Substring(4, 2) + "/" + strLine[1].Substring(6, 2));
                        //drIn["Number"] = Convert.ToInt32(strLine[3]);
                        //drIn["IsAdd"] = "N";
                        //dtDataIn.Rows.Add(drIn);
                        //// add by Ian Huang start
                        //strMaxDate = "" == strMaxDate ? strLine[1] : int.Parse(strMaxDate) > int.Parse(strLine[1]) ? strMaxDate : strLine[1];
                        //// add by Ian Huang end


                        //排序，讓04 結餘在最後一筆
                        //DataRow drIn_01 = dtDataIn.NewRow();
                        //DataRow drIn_02 = dtDataIn.NewRow();
                        //DataRow drIn_03 = dtDataIn.NewRow();
                        //DataRow[] drIn_01 = dtDataIn.Select(" Type<>4 and Serial_Number='" + strLine[0] + "'" , "Serial_Number,Type,Stock_Date,Number,IsAdd");
                        //DataRow[] drIn_02 = dtDataIn.Select(" Type=4 and Serial_Number='" + strLine[0] + "'", "Serial_Number,Type,Stock_Date,Number,IsAdd");
                        //DataRow[] drIn_03 = dtDataIn.Select(" Serial_Number<> '' and  Serial_Number<>'" + strLine[0] + "'", "Serial_Number,Type,Stock_Date,Number,IsAdd");
                        //dtDataIn.Clear();
                        //if (drIn_01.Length>0)
                        //    dtDataIn.Rows.Add(drIn_01);
                        //if (drIn_02.Length > 0)
                        //    dtDataIn.Rows.Add(drIn_02);
                        //if (drIn_03.Length > 0)
                        //    dtDataIn.Rows.Add(drIn_03);

                    }
                }
                count++;

              
            }
            //add 
            if (!drIn2["Type"].ToString().Equals(""))
            dtDataIn.Rows.Add(drIn2);
            #endregion 讀字符串，并檢查字符串格式(列數、每列的字符格式),并保存到臨時DataTable(dtDataIn)中

            // add by Ian Huang start
            #region 將 移轉出、移轉入 資料存入到DB
            // 將 移轉出、移轉入 資料存入到 MATERIEL_STOCKS_MOVE 表中以代替原 物料庫存移轉作業 功能

            DataRow[] drType59 = dtDataIn.Select(" Type=5 or Type=9 ", "Serial_Number,Stock_Date");

            Depository011_1BL bl = new Depository011_1BL();

            for (int i = 0; i < drType59.Length; i++)
            {
                string strSNum = drType59[i]["Serial_Number"].ToString().Trim();
                string strSDate = DateTime.Parse(drType59[i]["Stock_Date"].ToString()).ToString("yyyy/MM/dd");
                int iType = int.Parse(drType59[i]["Type"].ToString());
                int iNumber = int.Parse(drType59[i]["Number"].ToString());

                MATERIEL_STOCKS_MOVE moveModel = getexistModel(strSDate, strSNum, iNumber, iType, strFactoryRID);
                bool bIsexist = isexistModel(strSDate, strSNum, iNumber, iType, strFactoryRID);

                if (!bIsexist)
                {
                    if (null == moveModel)
                    {
                        // add
                        moveModel = new MATERIEL_STOCKS_MOVE();
                        moveModel.Serial_Number = strSNum;
                        moveModel.Move_Number = iNumber;

                        if (5 == iType)
                        {
                            // 5 移轉出
                            moveModel.To_Factory_RID = 0;
                            moveModel.From_Factory_RID = Convert.ToInt32(strFactoryRID);
                        }
                        else
                        {
                            // 9 移轉入
                            moveModel.To_Factory_RID = Convert.ToInt32(strFactoryRID);
                            moveModel.From_Factory_RID = 0;
                        }

                        moveModel.Move_ID = bl.GetMove_ID(strSDate.Trim());
                        moveModel.Move_Date = DateTime.Parse(strSDate);
                        moveModel.RCU = "INPUT";
                        AddM(moveModel);
                    }
                    else
                    {
                        // update
                        if (5 == iType)
                        {
                            // 5 移轉出
                            moveModel.From_Factory_RID = Convert.ToInt32(strFactoryRID);
                        }
                        else
                        {
                            // 9 移轉入
                            moveModel.To_Factory_RID = Convert.ToInt32(strFactoryRID);
                        }
                        bl.Update(moveModel);
                    }
                }
            }

            #endregion 將 移轉出、移轉入 資料存入到DB

            #region 沒有廠商結餘的資料新增廠商結餘
            List<string> Lstr = new List<string>(); // 如果沒有廠商結餘，則ADD Serial_Number

            Dictionary<string, int> dic01 = new Dictionary<string, int>();  //進貨
            Dictionary<string, int> dic02 = new Dictionary<string, int>();  //退貨
            Dictionary<string, int> dic03 = new Dictionary<string, int>();  //銷毀

            Dictionary<string, int> dic06 = new Dictionary<string, int>();  //抽驗
            Dictionary<string, int> dic07 = new Dictionary<string, int>();  //退件重寄
            Dictionary<string, int> dic08 = new Dictionary<string, int>();  //電訊單異動
            DataRow[] drType4 = dtDataIn.Select("", "Serial_Number");

            for (int i = 0; i < drType4.Length; i++)
            {
                string strSNum = drType4[i]["Serial_Number"].ToString();
                bool bHave04 = false;

                for (int j = i; j < drType4.Length; j++)
                {
                    if (strSNum != drType4[j]["Serial_Number"].ToString())
                    {
                        i = j - 1;
                        break;
                    }

                    if ("4" == drType4[j]["Type"].ToString())
                    {
                        bHave04 = true;
                    }

                    // 記錄 進貨 數量
                    if ("1" == drType4[j]["Type"].ToString())
                    {
                        if (dic01.ContainsKey(strSNum))
                        {
                            dic01[strSNum] += int.Parse(drType4[j]["Number"].ToString());
                        }
                        else
                        {
                            dic01.Add(strSNum, int.Parse(drType4[j]["Number"].ToString()));
                        }
                    }

                    // 記錄 退貨 數量
                    if ("2" == drType4[j]["Type"].ToString())
                    {
                        if (dic02.ContainsKey(strSNum))
                        {
                            dic02[strSNum] += int.Parse(drType4[j]["Number"].ToString());
                        }
                        else
                        {
                            dic02.Add(strSNum, int.Parse(drType4[j]["Number"].ToString()));
                        }
                    }

                    // 記錄 銷毀 數量
                    if ("3" == drType4[j]["Type"].ToString())
                    {
                        if (dic03.ContainsKey(strSNum))
                        {
                            dic03[strSNum] += int.Parse(drType4[j]["Number"].ToString());
                        }
                        else
                        {
                            dic03.Add(strSNum, int.Parse(drType4[j]["Number"].ToString()));
                        }
                    }

                    // 記錄 抽驗 數量
                    if ("6" == drType4[j]["Type"].ToString())
                    {
                        if (dic06.ContainsKey(strSNum))
                        {
                            dic06[strSNum] += int.Parse(drType4[j]["Number"].ToString());
                        }
                        else
                        {
                            dic06.Add(strSNum, int.Parse(drType4[j]["Number"].ToString()));
                        }
                    }

                    // 記錄 退件重寄 數量
                    if ("7" == drType4[j]["Type"].ToString())
                    {
                        if (dic07.ContainsKey(strSNum))
                        {
                            dic07[strSNum] += int.Parse(drType4[j]["Number"].ToString());
                        }
                        else
                        {
                            dic07.Add(strSNum, int.Parse(drType4[j]["Number"].ToString()));
                        }
                    }

                    // 記錄 電訊單異動 數量
                    if ("8" == drType4[j]["Type"].ToString())
                    {
                        if (dic08.ContainsKey(strSNum))
                        {
                            dic08[strSNum] += int.Parse(drType4[j]["Number"].ToString());
                        }
                        else
                        {
                            dic08.Add(strSNum, int.Parse(drType4[j]["Number"].ToString()));
                        }
                    }

                    i = j;

                }

                if (!bHave04)
                {
                    Lstr.Add(strSNum);
                }
            }

            // 如果有資料沒有廠商結餘
            if (Lstr.Count > 0)
            {
                strMaxDate = strMaxDate.Substring(0, 4) + "/" + strMaxDate.Substring(4, 2) + "/" + strMaxDate.Substring(6, 2);
                InOut000BL bl000 = new InOut000BL();
                bl000.SaveSurplusSystemNum(Convert.ToDateTime(strMaxDate));

                for (int i = 0; i < Lstr.Count; i++)
                {
                    int iCount = 0;

                    if (dic01.ContainsKey(Lstr[i]))
                    {
                        iCount -= dic01[Lstr[i]];
                    }

                    if (dic02.ContainsKey(Lstr[i]))
                    {
                        iCount += dic02[Lstr[i]];
                    }

                    if (dic03.ContainsKey(Lstr[i]))
                    {
                        iCount += dic03[Lstr[i]];
                    }

                    if (dic06.ContainsKey(Lstr[i]))
                    {
                        iCount += dic06[Lstr[i]];
                    }

                    if (dic07.ContainsKey(Lstr[i]))
                    {
                        iCount += dic07[Lstr[i]];
                    }

                    if (dic08.ContainsKey(Lstr[i]))
                    {
                        iCount += dic08[Lstr[i]];
                    }

                    DataRow drIn = dtDataIn.NewRow();
                    drIn["Serial_Number"] = Lstr[i];
                    drIn["Type"] = 4;
                    drIn["Stock_Date"] = Convert.ToDateTime(strMaxDate);
                    drIn["Number"] = selectSTOCKS(strMaxDate, Lstr[i], int.Parse(strFactoryRID)) - iCount;
                    drIn["IsAdd"] = "Y";
                    dtDataIn.Rows.Add(drIn);
                }

            }

            // 排序 下方有檢查
            DataTable dtDataInSort = dtDataIn.Copy();
            //dtDataInSort.DefaultView.Sort = "Serial_Number,Stock_Date,Type ASC";
            dtDataInSort.DefaultView.Sort = "Serial_Number,Stock_Date ASC";
            dtDataIn = dtDataInSort.DefaultView.ToTable();
            #endregion 沒有廠商結餘的資料新增廠商結餘


            // add by Ian Huang end

            #region 廠商資料及匯入資料有無檢查
            if (dtFactory.Rows.Count == 0)
            {
                throw new AlertException("匯入資料中沒有廠商編號！");
            }

            if (dtDataIn.Rows.Count == 0)
            {
                throw new AlertException("匯入資料中沒有庫存異動訊息！");
            }
            #endregion 廠商資料及匯入資料有無檢查

            //pan:2008/11/18
            #region 判斷物品是否存在
            for (int i = 0; i < dtDataIn.Rows.Count; i++)
            {
                string str = dtDataIn.Rows[i]["Serial_Number"].ToString();
                DataSet ds = new DataSet();

                dirValues.Clear();
                dirValues.Add("Serial_Number", str);
                if ("A" == str.Substring(0, 1).ToUpper())// 信封
                {
                    ds = dao.GetList(SEL_ENVELOPE_INFO, dirValues);
                }
                else if ("B" == str.Substring(0, 1).ToUpper())// 卡單
                {
                    ds = dao.GetList(SEL_CARD_EXPONENT, dirValues);
                }
                else if ("C" == str.Substring(0, 1).ToUpper())// DM
                {
                    ds = dao.GetList(SEL_DMTYPE_INFO, dirValues);
                }

                if (ds == null || ds.Tables.Count == 0)
                {
                    throw new AlertException("物品編號" + str + "不存在！");
                }
            }
            #endregion 判斷物品是否存在

            //pan:2008/10/29
            #region 判斷檔案內容各種物料(品名)是否結餘（即type=4）
            for (int intRow = 0; intRow < dtDataIn.Rows.Count; intRow++)
            {
                string str = dtDataIn.Rows[intRow]["Serial_Number"].ToString();
                ArrayList al = new ArrayList();
                for (int iRow = 0; iRow < dtDataIn.Rows.Count; iRow++)
                {
                    if (str == dtDataIn.Rows[iRow]["Serial_Number"].ToString())
                    {
                        al.Add(dtDataIn.Rows[iRow]["Type"].ToString());
                    }
                }
                if (al.Count > 0)
                {
                    ArrayList array = new ArrayList();
                    for (int i = 0; i < al.Count; i++)
                    {
                        if (al[i].ToString() == "4")
                        {
                            array.Add(al[i]);
                            break;
                        }
                    }
                    if (array.Count == 0)
                    {
                        throw new AlertException("物料" + str + "沒有結餘！");
                    }
                }
            }
            #endregion 判斷檔案內容各種物料(品名)是否結餘（即type=4）

            #region 檔案內容各種物料(品名)只能有一筆結餘
            for (int intRow = 0; intRow < dtDataIn.Rows.Count - 1; intRow++)
            {
                if (4 == Convert.ToInt32(dtDataIn.Rows[intRow]["Type"]))
                {
                    for (int intRow1 = intRow + 1; intRow1 < dtDataIn.Rows.Count; intRow1++)
                    {
                        if (4 == Convert.ToInt32(dtDataIn.Rows[intRow1]["Type"]) &&
                            Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]) == Convert.ToString(dtDataIn.Rows[intRow1]["Serial_Number"]))
                        {
                            throw new AlertException("檔案格式錯誤，[" + Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]) + "]只能有一筆結餘。");
                        }
                    }
                }
            }
            #endregion 檔案內容各種物料(品名)只能有一筆結餘

            #region 廠商結餘為負檢查
            for (int intRow = 0; intRow < dtDataIn.Rows.Count; intRow++)
            {
                if (4 == Convert.ToInt32(dtDataIn.Rows[intRow]["Type"]))
                {
                    if (Convert.ToInt32(dtDataIn.Rows[intRow]["Number"]) < 0)
                    {
                        throw new AlertException("廠商餘為數不能為負");
                    }
                }
            }
            #endregion 廠商結餘為負檢查

            #region 排序檢查(按品名、日期、類別)
            DataTable dtSort = dtDataIn.Copy();
           // dtSort.DefaultView.Sort = "Serial_Number,Stock_Date,Type ASC";
            dtSort.DefaultView.Sort = "Serial_Number,Stock_Date ASC";
            dtSort = dtSort.DefaultView.ToTable();
            // 排序檢查
            for (int intRowNum = 0; intRowNum < dtDataIn.Rows.Count; intRowNum++)
            {
                //if (Convert.ToString(dtDataIn.Rows[intRowNum]["Serial_Number"]) != Convert.ToString(dtSort.Rows[intRowNum]["Serial_Number"]) ||
                //    Convert.ToDateTime(dtDataIn.Rows[intRowNum]["Stock_Date"]) != Convert.ToDateTime(dtSort.Rows[intRowNum]["Stock_Date"]) ||
                //    Convert.ToInt32(dtDataIn.Rows[intRowNum]["Type"]) != Convert.ToInt32(dtSort.Rows[intRowNum]["Type"]))
                //{
                //    throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_010_02);
                //}

                if (Convert.ToString(dtDataIn.Rows[intRowNum]["Serial_Number"]) != Convert.ToString(dtSort.Rows[intRowNum]["Serial_Number"]))
                {
                    throw new AlertException("匯入文件中品名排序錯誤！");
                }
                else if (Convert.ToDateTime(dtDataIn.Rows[intRowNum]["Stock_Date"]) != Convert.ToDateTime(dtSort.Rows[intRowNum]["Stock_Date"]))
                {
                    throw new AlertException("匯入文件中日期排序錯誤！");
                }
                else if (Convert.ToInt32(dtDataIn.Rows[intRowNum]["Type"]) != Convert.ToInt32(dtSort.Rows[intRowNum]["Type"]))
                {
                    throw new AlertException("匯入文件中類別排序錯誤！");
                }
            }
            #endregion 排序檢查(按品名、日期、類別)

            #region 同一檔案所有的物料(品名)結餘日期必須一樣
            for (int intRow = 0; intRow < dtDataIn.Rows.Count; intRow++)
            {
                if (4 == Convert.ToInt32(dtDataIn.Rows[intRow]["Type"]))
                {
                    for (int intRow1 = intRow + 1; intRow1 < dtDataIn.Rows.Count; intRow1++)
                    {
                        if (4 == Convert.ToInt32(dtDataIn.Rows[intRow1]["Type"]) &&
                            Convert.ToString(dtDataIn.Rows[intRow]["Stock_Date"]) != Convert.ToString(dtDataIn.Rows[intRow1]["Stock_Date"])
                            )
                        {
                            throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_010_06);
                        }
                    }
                    break;
                }
            }
            #endregion  同一檔案所有的物料(品名)結餘日期必須一樣

            #region 匯入檔案時，要檢查檔案內的結餘日期，必須是系統做過日結的日期
            for (int intRow = 0; intRow < dtDataIn.Rows.Count; intRow++)
            {
                if (4 == Convert.ToInt32(dtDataIn.Rows[intRow]["Type"]))
                {
                    //if (!isSurplusDate(Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]),
                    //    Convert.ToDateTime(dtDataIn.Rows[intRow]["Stock_Date"]),
                    //    Convert.ToInt32(values["FactoryRID"])))
                    //{
                    //    throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_010_07);
                    //}

                    //pan:2008/11/18
                    if (!isSurplusDate(Convert.ToDateTime(dtDataIn.Rows[intRow]["Stock_Date"])))
                    {
                        throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_010_07);
                    }
                }
            }
            #endregion 匯入檔案時，要檢查檔案內的結餘日期，必須是系統做過日結的日期

            #region 每一Perso廠回饋檔，檔案內容各種物料(品名)的進貨、退貨、銷毀日期不能大於結餘日期
            for (int intRow = 0; intRow < dtDataIn.Rows.Count; intRow++)
            {
                // 結餘
                if (4 == Convert.ToInt32(dtDataIn.Rows[intRow]["Type"]))
                {
                    for (int intRow1 = 0; intRow1 < dtDataIn.Rows.Count; intRow1++)
                    {
                        if (4 != Convert.ToInt32(dtDataIn.Rows[intRow1]["Type"]) &&
                            Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]) == Convert.ToString(dtDataIn.Rows[intRow1]["Serial_Number"]) &&
                            Convert.ToDateTime(dtDataIn.Rows[intRow]["Stock_Date"]) < Convert.ToDateTime(dtDataIn.Rows[intRow1]["Stock_Date"]))
                        {
                            throw new AlertException("檔案格式錯誤，[" + Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]) + "]（" +
                                getTypeName("0" + Convert.ToString(dtDataIn.Rows[intRow1]["Type"])) + "）日期" +
                                (Convert.ToDateTime(dtDataIn.Rows[intRow1]["Stock_Date"])).ToString("yyyy/MM/dd") + "大於結餘日期" +
                                (Convert.ToDateTime(dtDataIn.Rows[intRow]["Stock_Date"])).ToString("yyyy/MM/dd") + "。");
                        }
                    }
                }
            }
            #endregion 每一Perso廠回饋檔，檔案內容各種物料(品名)的進貨、退貨、銷毀日期不能大於結餘日期

            #region 各種物料(品名)之結餘日期及任何異動日期皆不能小於上次結餘日期
            string Serial_Number = "";
            for (int intRow = 0; intRow < dtDataIn.Rows.Count; intRow++)
            {
                DataTable dtLastSurplusDate = null;
                if (Serial_Number != Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]) || Convert.ToInt16(dtDataIn.Rows[intRow]["Type"]) == 4)
                {
                    Serial_Number = Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]);
                    // 取物料的上次結餘日期及結餘數量
                    dtLastSurplusDate = (DataTable)GetLastSurplusDateNum(Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]),
                                                    Convert.ToInt32(strFactoryRID));
                }

                // 物料作過結餘
                if (null != dtLastSurplusDate &&
                    dtLastSurplusDate.Rows.Count > 0)
                {
                    // 結餘日期或異動日期不能小於上次結餘日期
                    if (DateTime.Parse(Convert.ToDateTime(dtDataIn.Rows[intRow]["Stock_Date"]).ToString("yyyy/MM/dd 12:00:00")) <=
                        DateTime.Parse(Convert.ToDateTime(dtLastSurplusDate.Rows[0]["Stock_Date"]).ToString("yyyy/MM/dd 12:00:00")))
                    {
                        throw new AlertException("檔案格式錯誤，[" + Convert.ToString(dtDataIn.Rows[intRow]["Serial_Number"]) + "]結餘日期或異動日期不能小於等於上次結餘日期。");
                    }

                    // 該物料的最近一次結餘日期
                    dtDataIn.Rows[intRow]["Last_Surplus_Date"] = Convert.ToDateTime(dtLastSurplusDate.Rows[0]["Stock_Date"]);
                    // 該物料的最近一次結餘數量
                    dtDataIn.Rows[intRow]["Last_Surplus_Num"] = Convert.ToInt32(dtLastSurplusDate.Rows[0]["Number"]);
                }
                // 物料沒作過結餘(理論上不存在此情況，數據庫中最少有一條結餘記錄)
                else
                {
                    //該物料的最近一次結餘時間（數據庫中必須有一條結餘記錄）
                    if (getLAST_SURPLUS_DATE(Serial_Number) != "")
                    {
                        dtDataIn.Rows[intRow]["Last_Surplus_Date"] = getLAST_SURPLUS_DATE(Serial_Number);
                    }
                    else
                    {
                        for (int i = 0; i < dtDataIn.Rows.Count; i++)
                        {
                            if (dtDataIn.Rows[i]["Serial_Number"].ToString() == Serial_Number)
                            {
                                // 該物料的最近一次結餘日期
                                dtDataIn.Rows[intRow]["Last_Surplus_Date"] = Convert.ToDateTime(dtDataIn.Rows[i]["Stock_Date"]).AddDays(-1);
                                break;
                            }
                        }
                    }
                    // 該物料的最近一次結餘數量
                    dtDataIn.Rows[intRow]["Last_Surplus_Num"] = 0;
                }
            }
            #endregion 各種物料(品名)之結餘日期及任何異動日期皆不能小於上次結餘日期

            dstDataIn = new DataSet();
            dstDataIn.Tables.Add(dtFactory);
            dstDataIn.Tables.Add(dtDataIn);

            return dstDataIn;

        }
        catch (AlertException aex)
        {
            throw aex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
        finally
        {

            sr.Close();
        }
    }

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
    /// 根據物料編號獲得對應名稱
    /// </summary>
    /// <param name="Serial_Number"></param>
    /// <returns></returns>
    public string getMateriel_Name(string Serial_Number)
    {
        DataSet ds = new DataSet();
        dirValues.Clear();
        dirValues.Add("Serial_Number", Serial_Number);
        if ("A" == Serial_Number.Substring(0, 1).ToUpper())// 信封
        {
            ds = dao.GetList(SEL_ENVELOPE_INFO, dirValues);
        }
        else if ("B" == Serial_Number.Substring(0, 1).ToUpper())// 卡單
        {
            ds = dao.GetList(SEL_CARD_EXPONENT, dirValues);
        }
        else if ("C" == Serial_Number.Substring(0, 1).ToUpper())// DM
        {
            ds = dao.GetList(SEL_DMTYPE_INFO, dirValues);
        }

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            //獲得紙品物料名稱
            return ds.Tables[0].Rows[0]["name"].ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 取Perso廠商的物料最近一次的結餘日期、結餘數量
    /// </summary>
    /// <param name="Serial_Number">品名編號</param>
    /// <param name="Perso_Factory_RID">Perso廠RID</param>
    /// <returns><DateTime>物料的最近一次的結餘日期</returns>
    public DataTable GetLastSurplusDateNum(string Serial_Number, int Perso_Factory_RID)
    {
        DataTable dtLastSurplusDateNum = null;
        try
        {
            // 取最近一次的結餘日期、結餘數量
            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Perso_Factory_RID);
            this.dirValues.Add("serial_number", Serial_Number);
            DataSet dstSurplus = dao.GetList(SEL_MATERIEL_LAST_SURPLUS_DATE, this.dirValues);
            if (null != dstSurplus && dstSurplus.Tables.Count > 0 &&
                dstSurplus.Tables[0].Rows.Count > 0)
            {
                dtLastSurplusDateNum = dstSurplus.Tables[0];
            }

            return dtLastSurplusDateNum;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 從物料的上次結餘日期至資料匯入日期，計算物料的每天耗用量
    /// 計算方法:物料當天的耗用量*（1+物料的損耗率）
    /// </summary>
    /// <param name="dtMaterielStockIn"></param>
    /// <returns></returns>
    public List<object> getMaterielUsedOnDay(DataTable dtMaterielStockIn, DataTable dtFactory)
    {
        // 復製Table;其中Number字段為實際使用量,System_Num字段為耗用量=實際使用量*(1+損耗率)
        List<object> listMaterielUsedOnDay = new List<object>();
        string Serial_Number = "";
        try
        {
            for (int intRow = 0; intRow < dtMaterielStockIn.Rows.Count; intRow++)
            {
                if (Serial_Number != Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]))
                {
                    // 保存當前物品編號
                    Serial_Number = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]);

                    // 物料的上次結餘日期
                    DateTime dtLastSurplusDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Date"]);
                    int lastNumber = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Num"]);

                    // by weilinzhan@wistronits.com 20090327 start 計算物料系統結餘需要算到系統當前日期。
                    DateTime dtStockDateTime = DateTime.Now;
                    //DateTime dtStockDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Stock_Date"]);
                    //for (int i = 0; i < dtMaterielStockIn.Rows.Count; i++)
                    //{
                    //    if (dtMaterielStockIn.Rows[i]["type"].ToString() == "4")
                    //    {
                    //        dtStockDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[i]["Stock_Date"]);
                    //        break;
                    //    }
                    //}

                    // 取物料的庫存消耗
                    //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/09/17 start
                    // DataTable dtblSTOCKS_USED = MaterielUsedCount(Convert.ToInt32(dtFactory.Rows[0]["Factory_RID"]), Serial_Number, dtLastSurplusDateTime, dtStockDateTime);
                    InOut000BL BL000 = new InOut000BL();
                    DataTable dtblSTOCKS_USED = BL000.MaterielUsedCount(Convert.ToInt32(dtFactory.Rows[0]["Factory_RID"]), Serial_Number, dtLastSurplusDateTime, dtStockDateTime);
                    //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/09/17 end
                    // 如果物料庫存消耗檔不為空
                    if (null != dtblSTOCKS_USED)
                    {
                        foreach (DataRow drSTOCKS_USED in dtblSTOCKS_USED.Rows)
                        {
                            DataRow drMaterielCountUsedOnDay = dtMaterielStockIn.NewRow();
                            drMaterielCountUsedOnDay["Serial_Number"] = Serial_Number;
                            drMaterielCountUsedOnDay["Materiel_Name"] = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Materiel_Name"]);
                            drMaterielCountUsedOnDay["Stock_Date"] = Convert.ToDateTime(drSTOCKS_USED["Stock_Date"]);
                            drMaterielCountUsedOnDay["Last_Surplus_Date"] = dtLastSurplusDateTime;
                            drMaterielCountUsedOnDay["Last_Surplus_Num"] = lastNumber;
                            //update by Ian Huang start
                            drMaterielCountUsedOnDay["Type"] = 56;  // 耗用標識
                            //update by Ian Huang end
                            drMaterielCountUsedOnDay["Number"] = Convert.ToInt32(drSTOCKS_USED["Number"]);//沒有算損耗的
                            drMaterielCountUsedOnDay["Materiel_RID"] = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Materiel_RID"]);
                            drMaterielCountUsedOnDay["System_Num"] = Convert.ToInt32(drSTOCKS_USED["System_Num"]);//計算損耗的
                            // 保存
                            listMaterielUsedOnDay.Add(drMaterielCountUsedOnDay);
                        }
                    }
                }
            }

            return listMaterielUsedOnDay;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 取物料的物品、RID等訊息
    /// </summary>
    /// <param name="Serial_Number">品名編號</param>
    /// <returns><DataTable>物料DataTable</returns>
    public DataSet GetMateriel(string Serial_Number)
    {
        DataSet dtsMateriel = null;
        try
        {
            // 取物料的品名
            this.dirValues.Clear();
            this.dirValues.Add("serial_number", Serial_Number);

            // 信封
            if ("A" == Serial_Number.Substring(0, 1).ToUpper())
                dtsMateriel = (DataSet)dao.GetList(SEL_ENVELOPE_INFO, this.dirValues);
            // 寄卡單
            else if ("B" == Serial_Number.Substring(0, 1).ToUpper())
                dtsMateriel = (DataSet)dao.GetList(SEL_CARD_EXPONENT, this.dirValues);
            // DM
            else if ("C" == Serial_Number.Substring(0, 1).ToUpper())
                dtsMateriel = (DataSet)dao.GetList(SEL_DMTYPE_INFO, this.dirValues);
            return dtsMateriel;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 匯入檔案時，要檢查檔案內的結餘日期，必須是系統做過日結的日期
    /// </summary>
    /// <param name="Serial_Number">品名編號</param>
    /// <param name="Stock_Date">結餘日期</param>
    /// <param name="Perso_Factory_RID">Perso廠RID</param>
    /// <returns>false:[結餘日期]尚未日結；
    ///         true:[結餘日期]正確日結。</returns>
    private bool isSurplusDate(string Serial_Number, DateTime Stock_Date, int Perso_Factory_RID)
    {
        try
        {
            // 取Perso廠該物料品名的所有結餘日期
            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Perso_Factory_RID);
            this.dirValues.Add("serial_number", Serial_Number);
            DataSet dstSurplus = dao.GetList(SEL_MATERIEL_SURPLUS_DATE, this.dirValues);
            if (null != dstSurplus && dstSurplus.Tables.Count > 0 &&
                dstSurplus.Tables[0].Rows.Count > 0)
            {
                for (int intRow = 0; intRow < dstSurplus.Tables[0].Rows.Count; intRow++)
                {
                    if (Convert.ToDateTime(dstSurplus.Tables[0].Rows[intRow]["Stock_Date"]).ToString("yyyyMMdd") ==
                        Stock_Date.ToString("yyyyMMdd"))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 通過卡片庫存表的時間判斷指定日期是否為日結時間
    /// </summary>
    /// <param name="Stock_Date"></param>
    /// <returns></returns>
    private bool isSurplusDate(DateTime Stock_Date)
    {
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("Stock_Date", Stock_Date);
            DataSet ds = dao.GetList("select * from CARDTYPE_STOCKS where rst='A' and Stock_Date=@Stock_Date", dirValues);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
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
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }

    }

    /// <summary>
    /// 按Perso廠商，從物料轉移檔中取物料的移入記錄
    /// </summary>
    /// <param name="dtMaterielStockIn">匯入DataTable</param>
    /// <param name="dtFactory">廠商DataTable</param>
    /// <returns>void</DataTable></returns>
    public List<object> getStocksMoveInOnDay(DataTable dtMaterielStockIn, DataTable dtFactory)
    {
        List<object> listStocksMoveInOnDay = new List<object>();
        string Serial_Number = "";
        try
        {
            #region 取物料移入記錄
            for (int intRow = 0; intRow < dtMaterielStockIn.Rows.Count; intRow++)
            {
                if (Serial_Number != Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]))
                {
                    // 保存當前物品編號
                    Serial_Number = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]);
                    int Materiel_Type = 0;
                    // 信封
                    if ("A" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 2;
                    else if ("B" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 1;
                    else if ("C" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 3;

                    // 物料的上次結餘日期
                    DateTime dtLastSurplusDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Date"]);
                    int lastNumber = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Num"]);

                    // by weilinzhan@wistronits.com 20090327 start 計算物料系統結餘需要算到系統當前日期。
                    DateTime NowDateTime = DateTime.Now;
                    // 當前匯入文檔的物品結餘日期
                    //DateTime NowDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Stock_Date"]);
                    //for (int i = 0; i < dtMaterielStockIn.Rows.Count; i++)
                    //{
                    //    if ("4" == dtMaterielStockIn.Rows[i]["type"].ToString())
                    //    {
                    //        NowDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[i]["Stock_Date"]);
                    //        break;
                    //    }
                    //}
                    // by weilinzhan@wistronits.com 20090327 end 計算物料系統結餘需要算到系統當前日期。

                    // 取物料的移入訊息
                    DataTable dtblStoksMoveIn = StocksMoveIn(Convert.ToInt32(dtFactory.Rows[0]["Factory_RID"]),
                                                            Serial_Number,
                                                            dtLastSurplusDateTime,
                                                            NowDateTime);

                    // 如果物料移入訊息不為空
                    if (null != dtblStoksMoveIn)
                    {
                        foreach (DataRow drStoksMoveIn in dtblStoksMoveIn.Rows)
                        {
                            DataRow drStocksMoveInOnDay = dtMaterielStockIn.NewRow();
                            drStocksMoveInOnDay["Serial_Number"] = Serial_Number;
                            drStocksMoveInOnDay["Materiel_Name"] = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Materiel_Name"]);
                            drStocksMoveInOnDay["Last_Surplus_Date"] = dtLastSurplusDateTime;
                            drStocksMoveInOnDay["Last_Surplus_Num"] = lastNumber;
                            drStocksMoveInOnDay["Stock_Date"] = Convert.ToDateTime(drStoksMoveIn["Move_Date"]);
                            //update by Ian Huang start
                            drStocksMoveInOnDay["Type"] = 57;  // 移入標識
                            //update by Ian Huang end
                            drStocksMoveInOnDay["Number"] = Convert.ToInt32(drStoksMoveIn["Move_Number"]);
                            drStocksMoveInOnDay["Materiel_RID"] = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Materiel_RID"]);
                            drStocksMoveInOnDay["System_Num"] = 0;

                            // 零時存儲
                            listStocksMoveInOnDay.Add(drStocksMoveInOnDay);
                        }
                    }
                }
            }
            #endregion 取物料移入記錄

            return listStocksMoveInOnDay;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 按Perso廠商，從物料轉移檔中取物料的移出記錄
    /// </summary>
    /// <param name="dtMaterielStockIn">匯入DataTable</param>
    /// <param name="dtFactory">廠商DataTable</param>
    /// <returns><DataTable>廠商的物料轉出記錄</DataTable></returns>
    public List<object> getStocksMoveOutOnDay(DataTable dtMaterielStockIn, DataTable dtFactory)
    {
        List<object> listStocksMoveOutOnDay = new List<object>();
        string Serial_Number = "";
        try
        {
            for (int intRow = 0; intRow < dtMaterielStockIn.Rows.Count; intRow++)
            {
                if (Serial_Number != Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]))
                {
                    // 保存當前物品編號
                    Serial_Number = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]);
                    int Materiel_Type = 0;
                    // 信封
                    if ("A" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 2;
                    else if ("B" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 1;
                    else if ("C" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 3;

                    // 物料的上次結餘日期
                    DateTime dtLastSurplusDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Date"]);
                    int lastNumber = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Num"]);
                    // by weilinzhan@wistronits.com 20090327 start 計算物料系統結餘需要算到系統當前日期。
                    DateTime NowDateTime = DateTime.Now;
                    //// 當前匯入文檔的物品結餘日期
                    //DateTime NowDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Stock_Date"]);
                    //for (int i = 0; i < dtMaterielStockIn.Rows.Count; i++)
                    //{
                    //    if ("4" == dtMaterielStockIn.Rows[i]["type"].ToString())
                    //    {
                    //        NowDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[i]["Stock_Date"]);
                    //        break;
                    //    }
                    //}
                    // by weilinzhan@wistronits.com 20090327 end 計算物料系統結餘需要算到系統當前日期。

                    // 取物料的移出訊息
                    DataTable dtblStoksMoveOut = StocksMoveOut(Convert.ToInt32(dtFactory.Rows[0]["Factory_RID"]),
                                                           Serial_Number,
                                                            dtLastSurplusDateTime,
                                                            NowDateTime);

                    // 如果物料移出訊息不為空
                    if (null != dtblStoksMoveOut)
                    {
                        foreach (DataRow drStoksMoveOut in dtblStoksMoveOut.Rows)
                        {
                            DataRow drStocksMoveOutOnDay = dtMaterielStockIn.NewRow();
                            drStocksMoveOutOnDay["Serial_Number"] = Serial_Number;
                            drStocksMoveOutOnDay["Materiel_Name"] = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Materiel_Name"]);
                            drStocksMoveOutOnDay["Stock_Date"] = Convert.ToDateTime(drStoksMoveOut["Move_Date"]);
                            drStocksMoveOutOnDay["Last_Surplus_Date"] = dtLastSurplusDateTime;
                            drStocksMoveOutOnDay["Last_Surplus_Num"] = lastNumber;
                            //update by Ian Huang start
                            drStocksMoveOutOnDay["Type"] = 58;  // 移出標識
                            //update by Ian Huang end
                            drStocksMoveOutOnDay["Number"] = Convert.ToInt32(drStoksMoveOut["Move_Number"]);
                            drStocksMoveOutOnDay["Materiel_RID"] = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Materiel_RID"]);
                            drStocksMoveOutOnDay["System_Num"] = 0;
                            // 保存
                            listStocksMoveOutOnDay.Add(drStocksMoveOutOnDay);
                        }
                    }
                }
            }

            return listStocksMoveOutOnDay;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 根據物料品名編號獲得物料創建時間
    /// </summary>
    /// <param name="strSerial_Number">物料品名編號</param>
    /// <returns></returns>
    //public string GetSDate(string strSerial_Number,string strPersoFactoryRID)
    //{
    //    DataSet ds = dao.GetList("select * from MATERIEL_STOCKS_MANAGE where rst='A' and Serial_Number='" + strSerial_Number + "' and Perso_Factory_RID='"+strPersoFactoryRID+"' order by rct desc");

    //    if (ds != null && ds.Tables[0].Rows.Count > 0)
    //    {
    //        return ds.Tables[0].Rows[0]["RCT"].ToString();
    //    }
    //    else
    //    {
    //        return "";
    //    }
    //}

    /// <summary>
    /// 判斷是否為用完為止
    /// </summary>
    /// <param name="strSerial_Number"></param>
    /// <returns></returns>
    public bool DmNotSafe_Type(string strSerial_Number)
    {
        bool n = true;
        try
        {
            if (strSerial_Number.Contains("C"))
            {
                DataTable dtbl = dao.GetList("select * from dbo.DMTYPE_INFO where Serial_Number='" + strSerial_Number + "'").Tables[0];
                if (dtbl.Rows.Count > 0)
                {
                    if (dtbl.Rows[0]["Safe_Type"].ToString() == "3")
                        n = false;
                }
            }

        }
        catch
        {
            return true;
        }
        return n;
    }

    /// <summary>
    /// 對匯入資料進行整理，并按物品生成每天的結餘訊息
    /// </summary>
    /// <param name="dtMaterielStockIn">匯入DataTable</param>
    /// <param name="dtFactory">廠商DataTable</param>
    public void DoMaterielStocksIn(DataTable dtMaterielStockIn, DataTable dtFactory)
    {
        List<object> listDoMaterielStocksIn = new List<object>();
        string Serial_Number = "";// 物品編號
        DateTime SurplusDateTime = DateTime.Parse("1900-01-01");// 本次結餘日期
        DateTime NowDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd 12:00:00"));
        int SurplusSystemNum = 0;// 本次結餘數量
        int OldSurplusSystemNum = int.MaxValue;//如果有廠商結余，保存下來，第二天的結余數用廠商結余繼續計算

        try
        {
            #region 取所有物品的庫存結餘訊息
            for (int intRow = 0; intRow < dtMaterielStockIn.Rows.Count; intRow++)
            {
                if (Serial_Number != Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]))
                {
                    // 保存當前物品編號
                    Serial_Number = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]);

                    bool SurplusNo1 = true;//標志---系統第一次作結余且是本次結余的第一天

                    // 計算結余開始日期(開始結余日期 = 最近一次的結余日期+1天)
                    SurplusDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Date"]).AddHours(24);
                    // 最近一次的結余數量
                    SurplusSystemNum = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Num"]);

                    //DateTime NowDateTime = DateTime.Now;
                    //// 當前匯入文檔的物品結餘日期
                    //for (int i = 0; i < dtMaterielStockIn.Rows.Count;i++ )
                    //{
                    //    if("4"==dtMaterielStockIn.Rows[i]["type"].ToString() && Serial_Number==dtMaterielStockIn.Rows[i]["Serial_Number"].ToString())
                    //    {
                    //        NowDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[i]["Stock_Date"]);
                    //        break;
                    //    }
                    //}


                    // 計算每天的結余數量
                    while (SurplusDateTime <= NowDateTime)
                    {
                        #region 計算當天的庫存結餘
                        // 計算當天的庫存結餘

                        //如果前一天的廠商結余數，則用廠商結余數！
                        if (OldSurplusSystemNum != int.MaxValue)
                        {
                            SurplusSystemNum = OldSurplusSystemNum;
                            OldSurplusSystemNum = int.MaxValue;
                        }


                        DataRow[] drsMaterielStockIn = dtMaterielStockIn.Select("Stock_Date = #" + String.Format("{0:s}", SurplusDateTime) + "#");
                        for (int intRowSurplus = 0; intRowSurplus < drsMaterielStockIn.Length; intRowSurplus++)
                        {
                            if (Serial_Number == Convert.ToString(drsMaterielStockIn[intRowSurplus]["Serial_Number"])
                                && (DateTime.Parse(SurplusDateTime.ToString("yyyy/MM/dd 00:00:00")) <=
                                    DateTime.Parse(Convert.ToDateTime(drsMaterielStockIn[intRowSurplus]["Stock_Date"]).ToString("yyyy/MM/dd 12:00:00"))
                                && DateTime.Parse(SurplusDateTime.ToString("yyyy/MM/dd 23:59:59")) >=
                                    DateTime.Parse(Convert.ToDateTime(drsMaterielStockIn[intRowSurplus]["Stock_Date"]).ToString("yyyy/MM/dd 12:00:00"))
                                || SurplusNo1 && DateTime.Parse(SurplusDateTime.ToString("yyyy/MM/dd 23:59:59")) >=
                                    Convert.ToDateTime(drsMaterielStockIn[intRowSurplus]["Stock_Date"])))
                            {
                                int intType = Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Type"]);

                                //update by Ian Huang start
                                if (1 == intType)
                                    // 進貨
                                    SurplusSystemNum += Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (2 == intType)
                                    // 退貨
                                    SurplusSystemNum -= Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (3 == intType)
                                    // 銷毀
                                    SurplusSystemNum -= Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (56 == intType)
                                    // 耗用（需計算損耗）
                                    SurplusSystemNum -= Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["System_Num"]);
                                else if (57 == intType)
                                    // 移入
                                    SurplusSystemNum += Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (58 == intType)
                                    // 移出
                                    SurplusSystemNum -= Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (6 == intType)
                                    // 抽驗
                                    SurplusSystemNum -= Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (7 == intType)
                                    // 退件重寄
                                    SurplusSystemNum -= Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (8 == intType)
                                    // 電訊單異動
                                    SurplusSystemNum -= Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                else if (4 == intType)
                                {
                                    // 廠商結余
                                    OldSurplusSystemNum = Convert.ToInt32(drsMaterielStockIn[intRowSurplus]["Number"]);
                                    //break;
                                }
                                //update by Ian Huang end
                            }
                        }
                        #endregion 計算當天的庫存結餘

                        //edit by linhuanhuang 解決庫存不足，系統重複發出多封mail的問題 20100927 start
                        // 只有是當天的庫存結餘為0時，才會發出mail
                        if (SurplusDateTime.ToString("yyyy/MM/dd") == NowDateTime.ToString("yyyy/MM/dd"))
                        {
                            // 如果結余小余0，則為0
                            if (SurplusSystemNum < 0)
                            {
                                if (DmNotSafe_Type(Serial_Number))
                                {
                                    string[] arg = new string[1];
                                    arg[0] = getMateriel_Name(Serial_Number);
                                    Warning.SetWarning(GlobalString.WarningType.MaterialDataInMiss, arg);
                                    SurplusSystemNum = 0;
                                }
                            }
                        }

                        //edit by linhuanhuang 解決庫存不足，系統重複發出多封mail的問題 20100927 end

                        DataRow dr = dtMaterielStockIn.NewRow();
                        dr["Serial_Number"] = Serial_Number;
                        dr["Materiel_Name"] = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Materiel_Name"]);
                        dr["Stock_Date"] = SurplusDateTime;
                        // edit by Ian Huang start
                        dr["Type"] = 10;  // 系統結余
                        // edit by Ian Huang end
                        dr["Number"] = 0;
                        dr["Materiel_RID"] = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Materiel_RID"]);
                        dr["System_Num"] = SurplusSystemNum;

                        // 將訊息添加到臨時List中
                        listDoMaterielStocksIn.Add(dr);
                        // 增加一天
                        SurplusDateTime = SurplusDateTime.AddHours(24);
                        // 
                        SurplusNo1 = false;//不是本次結余的第一天
                    }
                }
            }
            #endregion 取所有物品的庫存結餘訊息

            // 將物品的結餘訊息添加到匯入資料中
            foreach (DataRow drSurplusDate in listDoMaterielStocksIn)
            {
                dtMaterielStockIn.Rows.Add(drSurplusDate);
            }

            // 排序
            dtMaterielStockIn.DefaultView.Sort = "Serial_Number,Stock_Date,Type ASC";
            dtMaterielStockIn = dtMaterielStockIn.DefaultView.ToTable();

        }
        catch (AlertException alex)
        {
            throw alex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 將物料庫存控管匯入標設置為FALSE
    /// </summary>
    /// <returns></returns>
    public void ImportSubTotalEnd()
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
    /// 檢查物料庫存控管匯入刪除是否已經被開起。如果已經開起，返回FALSE
    ///                                   如果沒有開起，開起，并返回TRUE
    /// </summary>
    /// <returns></returns>
    public bool ImportSubTotalStart()
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
    /// 取上次工作日后的所有工作日
    /// </summary>
    /// <param name="dtFrom"></param>
    /// <returns></returns>
    public ArrayList getAllWorkDate()
    {
        try
        {
            ArrayList arr = new ArrayList();
            DataSet dsWorkDate = dao.GetList(SEL_ALL_WORK_DATE, this.dirValues);
            if (null != dsWorkDate && dsWorkDate.Tables.Count > 0)
            {
                for (int intLoop = 0; intLoop < dsWorkDate.Tables[0].Rows.Count - 1; intLoop++)
                {
                    arr.Add(Convert.ToDateTime(dsWorkDate.Tables[0].Rows[intLoop][0]).ToString("yyyy/MM/dd"));
                }
            }
            return arr;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 從物料移動檔中，按廠商、物品、時間段取物品的轉入記錄
    /// </summary>
    /// <param name="Factory_RID">Perso廠商RID</param>
    /// <param name="Materiel_Type">物料類型</param>
    /// <param name="Materiel_RID">物料RID</param>
    /// <param name="lastSurplusDateTime">最近一次的結餘日期</param>
    /// <param name="thisSurplusDateTime">本次結餘日期</param>
    /// <returns>DataTable<廠商轉入記錄></returns>
    public DataTable StocksMoveIn(int Factory_RID,
                        string Serial_Number,
                        DateTime lastSurplusDateTime,
                        DateTime thisSurplusDateTime)
    {
        DataTable dtStocksMoveIn = null;
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Factory_RID);
            this.dirValues.Add("Serial_Number", Serial_Number);
            this.dirValues.Add("lastSurplusDateTime", DateTime.Parse(lastSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            this.dirValues.Add("thisSurplusDateTime", DateTime.Parse(thisSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            DataSet dstMaterielStocksMove = dao.GetList(SEL_MATERIEL_STOCKS_MOVE_IN, this.dirValues);
            if (null != dstMaterielStocksMove
                        && dstMaterielStocksMove.Tables.Count > 0
                        && dstMaterielStocksMove.Tables[0].Rows.Count > 0)
            {
                dtStocksMoveIn = dstMaterielStocksMove.Tables[0];
            }

            return dtStocksMoveIn;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Factory_RID">Perso廠商RID</param>
    /// <param name="Materiel_Type">物料類型</param>
    /// <param name="Materiel_RID">物料RID</param>
    /// <param name="lastSurplusDateTime">最近一次的結餘日期</param>
    /// <param name="thisSurplusDateTime">本次結餘日期</param>
    /// <returns>DataTable<廠商轉入記錄></returns>
    public void DeleteSysSurplusData(int Factory_RID,
                        string Serial_Number,
                        DateTime lastSurplusDateTime,
                        DateTime thisSurplusDateTime)
    {
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Factory_RID);
            this.dirValues.Add("Serial_Number", Serial_Number);
            this.dirValues.Add("lastSurplusDateTime", DateTime.Parse(lastSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            //this.dirValues.Add("thisSurplusDateTime", DateTime.Parse(thisSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            dao.ExecuteNonQuery(DEL_MATERIEL_STOCKS_SYS, this.dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 從物料移動檔中，按廠商、物品、時間段取物品的轉出記錄
    /// </summary>
    /// <param name="Factory_RID">Perso廠商RID</param>
    /// <param name="Materiel_Type">物料類型</param>
    /// <param name="Materiel_RID">物料RID</param>
    /// <param name="lastSurplusDateTime">最近一次的結餘日期</param>
    /// <param name="thisSurplusDateTime">本次結餘日期</param>
    /// <returns>DataTable<廠商轉出記錄></returns>
    public DataTable StocksMoveOut(int Factory_RID,
                        string Serial_Number,
                        DateTime lastSurplusDateTime,
                        DateTime thisSurplusDateTime)
    {
        DataTable dtblStocksMoveOut = null;
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Factory_RID);
            this.dirValues.Add("Serial_Number", Serial_Number);
            this.dirValues.Add("lastSurplusDateTime", DateTime.Parse(lastSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            this.dirValues.Add("thisSurplusDateTime", DateTime.Parse(thisSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            DataSet dstMaterielStocksMove = dao.GetList(SEL_MATERIEL_STOCKS_MOVE_OUT, this.dirValues);
            if (null != dstMaterielStocksMove
                        && dstMaterielStocksMove.Tables.Count > 0
                        && dstMaterielStocksMove.Tables[0].Rows.Count > 0)
            {
                dtblStocksMoveOut = dstMaterielStocksMove.Tables[0];
            }

            return dtblStocksMoveOut;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

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
    //    //MATERIEL_STOCKS_USED msuModel = new MATERIEL_STOCKS_USED();
    //    try
    //    {
    //        string strFirst = Serial_Number.Substring(0, 1).ToUpper();
    //        string strSQL = "";
    //        if (strFirst.Equals("A"))
    //        {
    //            //strSQL = SEL_MATERIAL_USED_ENVELOPE;
    //            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
    //            strSQL = SEL_MATERIAL_USED_ENVELOPE_REPLACE;
    //            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end
    //        }
    //        else if (strFirst.Equals("B"))
    //        {
    //            //strSQL = SEL_MATERIAL_USED_CARD_EXPONENT;
    //            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
    //            strSQL = SEL_MATERIAL_USED_CARD_EXPONENT_REPLACE;
    //            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end
    //        }
    //        else if (strFirst.Equals("C"))
    //        {
    //            //strSQL = SEL_MATERIAL_USED_DM;
    //            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
    //            strSQL = SEL_MATERIAL_USED_DM_REPLACE;
    //            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end
    //        }
    //        else
    //        {
    //            return null;
    //        }

    //        dirValues.Clear();
    //        dirValues.Add("Perso_Factory_RID", Factory_RID);
    //        dirValues.Add("Serial_Number", Serial_Number);
    //        dirValues.Add("From_Date_Time", lastSurplusDateTime);
    //        dirValues.Add("End_Date_Time", thisSurplusDateTime);
    //        DataSet dstSTOCKS_USED = dao.GetList(strSQL, dirValues);
    //        if (null != dstSTOCKS_USED && dstSTOCKS_USED.Tables.Count > 0 &&
    //                        dstSTOCKS_USED.Tables[0].Rows.Count > 0)
    //        {
    //            dtSubtotal_Import = dstSTOCKS_USED.Tables[0];
    //            dtSubtotal_Import.Columns.Add(new DataColumn("System_Num",Type.GetType("System.Int32")));
    //            for (int intRow = 0; intRow < dtSubtotal_Import.Rows.Count; intRow++)
    //            {
    //                // 取物品的損耗率(關聯到物品表，取物品表的損耗率）
    //                //Decimal dWear_Rate = 0.0M;
    //                Decimal dWear_Rate = GetWearRate(Serial_Number);
    //                // 系統耗用量
    //                dtSubtotal_Import.Rows[intRow]["System_Num"] = Convert.ToInt32(dtSubtotal_Import.Rows[intRow]["Number"]) * (dWear_Rate/100+1);                   
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
    /// 計算物料庫存消耗檔并保存入資料庫
    /// </summary>
    /// <param name="Factory_RID">Perso廠商RID</param>
    /// <param name="Serial_Number">物料編號</param>    
    /// <param name="lastSurplusDateTime">最近一次的結餘日期</param>
    /// <param name="thisSurplusDateTime">本次結餘日期</param>
    /// 
    public List<string> SaveMaterielUsedCount(DateTime Date)
    {
        DataTable dtSubtotal_Import = null;
        List<string> lstSerielNumber = new List<string>();
        MATERIEL_STOCKS_USED msuModel = new MATERIEL_STOCKS_USED();
        try
        {
            //string strSQL = SEL_MATERIAL_USED_ENVELOPE_S + SEL_MATERIAL_USED_CARD_EXPONENT_S + SEL_MATERIAL_USED_DM_S;
            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
            string strSQL = SEL_MATERIAL_USED_ENVELOPE_S_REPLACE + SEL_MATERIAL_USED_CARD_EXPONENT_S_REPLACE + SEL_MATERIAL_USED_DM_S_REPLACE;
            //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end

            string delSQL = "Delete MATERIEL_STOCKS_USED where  Stock_Date =@End_Date_Time ";

            dirValues.Clear();

            dirValues.Add("End_Date_Time", Date);
            DataSet dstSTOCKS_USED = dao.GetList(strSQL, dirValues);
            dao.ExecuteNonQuery(delSQL, dirValues);
            for (int i = 0; i < dstSTOCKS_USED.Tables.Count; i++)
            {
                if (null != dstSTOCKS_USED && dstSTOCKS_USED.Tables.Count > 0 &&
                                dstSTOCKS_USED.Tables[i].Rows.Count > 0)
                {
                    dtSubtotal_Import = dstSTOCKS_USED.Tables[i];
                    dtSubtotal_Import.Columns.Add(new DataColumn("System_Num", Type.GetType("System.Int32")));
                    for (int intRow = 0; intRow < dtSubtotal_Import.Rows.Count; intRow++)
                    {
                        // 取物品的損耗率(關聯到物品表，取物品表的損耗率）                    
                        Decimal dWear_Rate = GetWearRate(dtSubtotal_Import.Rows[intRow]["Serial_Number"].ToString());
                        // 系統耗用量
                        dtSubtotal_Import.Rows[intRow]["System_Num"] = Convert.ToInt32(dtSubtotal_Import.Rows[intRow]["Number"]) * (dWear_Rate / 100 + 1);

                        if (dtSubtotal_Import.Rows[intRow]["Serial_Number"].ToString() != null && dtSubtotal_Import.Rows[intRow]["Serial_Number"].ToString() != "")
                        {
                            // 保存物料品名編號，為判斷物料的庫存和安全水位作準備
                            if (-1 == lstSerielNumber.IndexOf(dtSubtotal_Import.Rows[intRow]["Serial_Number"].ToString()))
                            {
                                lstSerielNumber.Add(dtSubtotal_Import.Rows[intRow]["Serial_Number"].ToString());
                            }
                            if (dtSubtotal_Import.Rows[intRow]["Stock_Date"].ToString() != null && dtSubtotal_Import.Rows[intRow]["Stock_Date"].ToString() != "")
                            {
                                msuModel.Stock_Date = DateTime.Parse(dtSubtotal_Import.Rows[intRow]["Stock_Date"].ToString());
                            }
                            msuModel.Number = long.Parse(dtSubtotal_Import.Rows[intRow]["System_Num"].ToString());
                            msuModel.Serial_Number = dtSubtotal_Import.Rows[intRow]["Serial_Number"].ToString();
                            msuModel.Perso_Factory_RID = Convert.ToInt32(dtSubtotal_Import.Rows[intRow]["Perso_Factory_RID"].ToString());
                            dao.Add<MATERIEL_STOCKS_USED>(msuModel, "RID");
                        }
                    }
                }
            }
            return lstSerielNumber;
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
    public int ComputeMaterialNumber(string MNumber, long MCount)
    {
        int iReturn = 0;
        decimal dWear_Rate = this.GetWearRate(MNumber);
        iReturn = Convert.ToInt32(MCount * (dWear_Rate / 100 + 1));
        return iReturn;
    }


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
    /// 依據Perso廠、卡種(多各卡種的RID)、開始時間、結束時間，得到多條小計檔信息
    /// </summary>
    /// <param name="Factory_RID"></param>
    /// <param name="CardTypeRIDList"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public DataSet ListSubTotalImport(int Factory_RID,
                string CardTypeRIDList,
                DateTime startTime,
                DateTime endTime)
    {
        DataSet dstSubtotal_Import = null;

        StringBuilder stbWhere = new StringBuilder();

        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Factory_RID);

            string[] str = CardTypeRIDList.Split(',');
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 0)
                {
                    if (str.Length == 1)
                    {
                        stbWhere.Append(" and (CT.RID in (@card_type_rid_list))");
                        this.dirValues.Add("card_type_rid_list", str[i]);
                    }
                    else if (str.Length > 1)
                    {
                        stbWhere.Append(" and (CT.RID in (@card_type_rid_list)");
                        this.dirValues.Add("card_type_rid_list", str[i]);
                    }
                }
                else
                {
                    if (str.Length - 1 == i)
                    {
                        stbWhere.Append(" or CT.RID in (@card_type_rid_lists))");
                        this.dirValues.Add("card_type_rid_lists", str[i]);
                    }
                    else
                    {
                        stbWhere.Append(" or CT.RID in (@card_type_rid_list" + i.ToString() + ")");
                        this.dirValues.Add("card_type_rid_list" + i.ToString(), str[i]);
                    }
                }
            }

            this.dirValues.Add("lastSurplusDateTime", Convert.ToDateTime(startTime.ToString("yyyy/MM/dd 23:59:59", System.Globalization.DateTimeFormatInfo.InvariantInfo)));
            this.dirValues.Add("thisSurplusDateTime", Convert.ToDateTime(endTime.ToString("yyyy/MM/dd 23:59:59", System.Globalization.DateTimeFormatInfo.InvariantInfo)));
            dstSubtotal_Import = dao.GetList(SEL_SUBTOTAL_IMPORT + stbWhere.ToString(), this.dirValues);
            return dstSubtotal_Import;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 取物品對應的卡種信息
    /// </summary>
    /// <param name="Materiel_Type">物品類型 1：寄卡單；2：信封；3：DM</param>
    /// <param name="Materiel_RID"></param>
    /// <returns></returns>
    public DataSet ListCardType(int Materiel_Type, int Materiel_RID)
    {
        DataSet dstCardType = null;

        try
        {
            this.dirValues.Clear();
            // 對應的卡種信息
            if (1 == Materiel_Type)
            {
                // 寄卡單
                this.dirValues.Add("exponent_rid", Materiel_RID);
                dstCardType = dao.GetList(SEL_CARD_INFO_EXPONENT, this.dirValues);
            }
            else if (2 == Materiel_Type)
            {
                // 信封
                this.dirValues.Add("envelope_rid", Materiel_RID);
                dstCardType = dao.GetList(SEL_CARD_INFO_ENVELOPE, this.dirValues);
            }
            else if (3 == Materiel_Type)
            {
                // DM
                this.dirValues.Add("dm_rid", Materiel_RID);
                dstCardType = dao.GetList(SEL_CARD_INFO_DM, this.dirValues);
            }

            return dstCardType;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }

    }

    /// <summary>
    /// 以結餘類型編號，取結餘類型名稱
    /// </summary>
    /// <param name="strType"></param>
    /// <returns></returns>
    public string getTypeName(string strType)
    {
        string strReturn = "";
        switch (strType)
        {
            case "01":
                strReturn = "進貨";
                break;
            case "02":
                strReturn = "退貨";
                break;
            case "03":
                strReturn = "銷毀";
                break;
            case "04":
                strReturn = "結餘";
                break;
            //add by Ian Huang start
            case "05":
                strReturn = "移轉出";
                break;
            case "06":
                strReturn = "抽驗";
                break;
            case "07":
                strReturn = "退件重寄";
                break;
            case "08":
                strReturn = "電訊單異動";
                break;
            case "09":
                strReturn = "移轉入";
                break;
            //add by Ian Huang end
            default:
                break;
        }
        return strReturn;
    }

    /// <summary>
    /// 驗證匯入字段是否滿足格式
    /// </summary>
    /// <param name="strColumn"></param>
    /// <param name="num"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private string CheckFileOneColumn(string strColumn, int num, int count)
    {
        string strErr = "";
        string Pattern = "";
        MatchCollection Matches;
        switch (num)
        {
            case 1:
                Pattern = @"^[A-Z]\d{5}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為6位字符;";
                }
                break;
            case 2:
                if (strColumn.Length != 8)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列時間格式不對;";
                    break;
                }

                string str1 = strColumn.Substring(0, 4) + "/" + strColumn.Substring(4, 2) + "/" + strColumn.Substring(6, 2);
                try
                {
                    DateTime dt = Convert.ToDateTime(str1);
                }
                catch
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列時間格式不對;";
                }
                break;
            case 3:
                // edit by Ian Huang start
                if (strColumn != "06" && strColumn != "07" && strColumn != "08" && strColumn != "04" && strColumn != "05" && strColumn != "09" && strColumn != "01" && strColumn != "02" && strColumn != "03")
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列必須為{01}{02}{03}{04}{05}{06}{07}{08}或{09}種的任意一字符串;";
                }
                // edit by Ian Huang end
                break;
            case 4:
                Pattern = @"^\d{1,9}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為9位以內數字;";
                }
                break;
            default:
                break;
        }

        return strErr;
    }

    /// <summary>
    /// 新增加記錄
    /// </summary>
    /// <param name="msmModel"></param>
    public void Add(MATERIEL_STOCKS_MANAGE msmModel)
    {
        try
        {
            dao.Add<MATERIEL_STOCKS_MANAGE>(msmModel, "RID");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    public void Addms(MATERIEL_STOCKS msModel)
    {
        try
        {
            dao.Add<MATERIEL_STOCKS>(msModel, "RID");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 刪除最后一次結餘。
    /// </summary>
    /// <param name="SurplusDate"></param>
    /// <param name="Factory_RID"></param>
    public void Delete(DateTime SurplusDate, int Factory_RID)
    {
        try
        {
            dao.OpenConnection();

            MATERIEL_STOCKS_MANAGE msmModel = new MATERIEL_STOCKS_MANAGE();

            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Factory_RID);
            // 判斷結餘日期是否是最後一筆結餘
            DataSet dstLastSurplusDate = dao.GetList(GET_LAST_SURPLUS_BY_FACTORY, this.dirValues);
            if (null != dstLastSurplusDate &&
                dstLastSurplusDate.Tables.Count > 0 &&
                dstLastSurplusDate.Tables[0].Rows.Count > 0)
            {
                // 是最后一筆結餘
                if (Convert.ToDateTime(dstLastSurplusDate.Tables[0].Rows[0]["Stock_Date"]).ToString("yyyyMMdd") ==
                    SurplusDate.ToString("yyyyMMdd"))
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("perso_factory_rid", Factory_RID);
                    DataSet dstSurplusData = dao.GetList(GET_SURPLUS_BY_FACTORY, this.dirValues);//獲得所有物品
                    if (null != dstSurplusData && dstSurplusData.Tables[0].Rows.Count > 0)
                    {
                        string Serial_Number = "";
                        for (int intRow = 0; intRow < dstSurplusData.Tables[0].Rows.Count; intRow++)
                        {
                            Serial_Number = dstSurplusData.Tables[0].Rows[intRow]["Serial_Number"].ToString();

                            dirValues.Clear();
                            this.dirValues.Add("perso_factory_rid", Factory_RID);
                            this.dirValues.Add("Serial_Number", Serial_Number);
                            DataSet dstDate = dao.GetList("select stock_date from MATERIEL_STOCKS_MANAGE WHERE RST = 'A' and Perso_Factory_RID=@perso_factory_rid and Serial_Number=@Serial_Number and type='4' order by stock_date desc", dirValues);

                            //獲得指定物品的所有結餘時間
                            if (dstDate != null && dstDate.Tables[0].Rows.Count > 0)
                            {
                                //多次結餘
                                if (dstDate.Tables[0].Rows.Count > 1)
                                {
                                    dirValues.Clear();
                                    this.dirValues.Add("perso_factory_rid", Factory_RID);
                                    this.dirValues.Add("Serial_Number", Serial_Number);
                                    this.dirValues.Add("lastSurplusDateTime", Convert.ToDateTime(dstDate.Tables[0].Rows[1][0].ToString()).ToString("yyyy/MM/dd 23:59:59"));
                                    //刪除上次結餘時間（lastSurplusDateTime）以後的所有記錄
                                    dao.ExecuteNonQuery(DEL_MATERIEL_STOCKS_MANAGE_DEL, this.dirValues);
                                    dao.ExecuteNonQuery(DEL_MATERIEL_STOCKS_DEL, this.dirValues);
                                }
                                else//第一次結餘
                                {
                                    dirValues.Clear();
                                    this.dirValues.Add("perso_factory_rid", Factory_RID);
                                    this.dirValues.Add("Serial_Number", Serial_Number);
                                    dao.ExecuteNonQuery("delete from MATERIEL_STOCKS_MANAGE where RST = 'A' and Perso_Factory_RID=@perso_factory_rid and Serial_Number=@Serial_Number", this.dirValues);
                                    dao.ExecuteNonQuery("delete from MATERIEL_STOCKS where RST = 'A' and Perso_Factory_RID=@perso_factory_rid and Serial_Number=@Serial_Number", this.dirValues);
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new AlertException("刪除廠商結餘日期不是最後一筆。");
                }
            }
            else
            {
                throw new AlertException("沒有結餘記錄！");
            }

            SetOprLog("13");

            dao.Commit();
        }
        catch (AlertException ale)
        {
            dao.Rollback();
            throw ale;
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    //add by Ian Huang start
    /// <summary>
    /// 按Perso廠商，從物料庫存異動檔中取進貨、退貨、銷毀記錄
    /// </summary>
    /// <param name="dtMaterielStockIn">匯入DataTable</param>
    /// <param name="dtFactory">廠商DataTable</param>
    /// <returns>void</DataTable></returns>
    public List<object> getStocksTransactionOnDay(DataTable dtMaterielStockIn, DataTable dtFactory)
    {
        List<object> listStocksTransactionOnDay = new List<object>();
        string Serial_Number = "";
        try
        {
            #region 取進貨、退貨、銷毀記錄
            for (int intRow = 0; intRow < dtMaterielStockIn.Rows.Count; intRow++)
            {
                if (Serial_Number != Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]))
                {
                    // 保存當前物品編號
                    Serial_Number = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Serial_Number"]);
                    int Materiel_Type = 0;
                    // 信封
                    if ("A" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 2;
                    else if ("B" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 1;
                    else if ("C" == Serial_Number.Substring(0, 1).ToUpper())
                        Materiel_Type = 3;

                    // 物料的上次結餘日期
                    DateTime dtLastSurplusDateTime = Convert.ToDateTime(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Date"]);
                    int lastNumber = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Last_Surplus_Num"]);

                    // 計算物料系統結餘需要算到系統當前日期。
                    DateTime NowDateTime = DateTime.Now;

                    // 取物料的移入訊息
                    DataTable dtblStoksTransaction = StocksTransaction(Convert.ToInt32(dtFactory.Rows[0]["Factory_RID"]),
                                                                Serial_Number,
                                                                dtLastSurplusDateTime,
                                                                NowDateTime);

                    // 如果物料移入訊息不為空
                    if (null != dtblStoksTransaction)
                    {
                        foreach (DataRow drStoksTransaction in dtblStoksTransaction.Rows)
                        {
                            DataRow drStocksTransactionOnDay = dtMaterielStockIn.NewRow();
                            drStocksTransactionOnDay["Serial_Number"] = Serial_Number;
                            drStocksTransactionOnDay["Materiel_Name"] = Convert.ToString(dtMaterielStockIn.Rows[intRow]["Materiel_Name"]);
                            drStocksTransactionOnDay["Last_Surplus_Date"] = dtLastSurplusDateTime;
                            drStocksTransactionOnDay["Last_Surplus_Num"] = lastNumber;
                            drStocksTransactionOnDay["Stock_Date"] = Convert.ToDateTime(drStoksTransaction["Transaction_Date"]);
                            drStocksTransactionOnDay["Type"] = Convert.ToInt32(drStoksTransaction["Param_Code"]);  // Param_Code 對應 異動類型
                            drStocksTransactionOnDay["Number"] = Convert.ToInt32(drStoksTransaction["Transaction_Amount"]);
                            drStocksTransactionOnDay["Materiel_RID"] = Convert.ToInt32(dtMaterielStockIn.Rows[intRow]["Materiel_RID"]);
                            drStocksTransactionOnDay["System_Num"] = 0;

                            // 零時存儲
                            listStocksTransactionOnDay.Add(drStocksTransactionOnDay);
                        }
                    }
                }
            }
            #endregion 取物料移入記錄

            return listStocksTransactionOnDay;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 從物料異動檔中，按廠商、物品、時間段物料的進貨、退貨、銷毀記錄
    /// </summary>
    /// <param name="Factory_RID">Perso廠商RID</param>
    /// <param name="Serial_Number">物料類型</param>
    /// <param name="lastSurplusDateTime">最近一次的結餘日期</param>
    /// <param name="thisSurplusDateTime">本次結餘日期</param>
    /// <returns>DataTable<物料的進貨、退貨、銷毀記錄></returns>
    public DataTable StocksTransaction(int Factory_RID,
                        string Serial_Number,
                        DateTime lastSurplusDateTime,
                        DateTime thisSurplusDateTime)
    {
        DataTable dtStocksTransaction = null;
        try
        {
            this.dirValues.Clear();
            this.dirValues.Add("perso_factory_rid", Factory_RID);
            this.dirValues.Add("Serial_Number", Serial_Number);
            this.dirValues.Add("lastSurplusDateTime", DateTime.Parse(lastSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            this.dirValues.Add("thisSurplusDateTime", DateTime.Parse(thisSurplusDateTime.ToString("yyyy/MM/dd 23:59:59")));
            DataSet dstMaterielStocksMove = dao.GetList(SEL_MATERIEL_STOCKS_TRANSACTION, this.dirValues);
            if (null != dstMaterielStocksMove
                        && dstMaterielStocksMove.Tables.Count > 0
                        && dstMaterielStocksMove.Tables[0].Rows.Count > 0)
            {
                dtStocksTransaction = dstMaterielStocksMove.Tables[0];
            }

            return dtStocksTransaction;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 刪除日結寫入MATERIEL_STOCKS_MANAGE表的 進貨、退貨、銷毀 信息，以避免出現重複數據
    /// </summary>
    /// <param name="strStockDate">異動日期</param>
    /// <param name="strRCU">異動人員</param>
    /// <param name="iPersoFactoryRID">廠商id</param>
    /// <param name="strType">類型</param>
    /// <param name="strSerialNumber">物料類型</param>
    public void DeleteSTOCKSMANAGE(string strStockDate, string strRCU, int iPersoFactoryRID, string strType, string strSerialNumber)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Stock_Date", strStockDate);
            dirValues.Add("RCU", strRCU);
            dirValues.Add("Perso_Factory_RID ", iPersoFactoryRID);
            dirValues.Add("Type", strType);
            dirValues.Add("Serial_Number", strSerialNumber);

            dao.ExecuteNonQuery(DELETE_MATERIEL_STOCKS_MANAGE, dirValues);

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    /// <summary>
    /// 獲得系統結餘
    /// </summary>
    /// <param name="strStockdate">結餘日期</param>
    /// <param name="strSerialNum">Serial Number</param>
    /// <param name="strFactoryRID">person廠ID</param>
    /// <returns></returns>
    public int selectSTOCKS(string strStockdate, string strSerialNum, int iFactoryRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Stock_date", strStockdate);
            dirValues.Add("Serial_Number", strSerialNum);
            dirValues.Add("Perso_Factory_RID ", iFactoryRID);

            DataSet ds = dao.GetList(SEL_MATERIEL_STOCKS, dirValues);

            if (null != ds
                        && ds.Tables.Count > 0
                        && ds.Tables[0].Rows.Count > 0)
            {
                return int.Parse(ds.Tables[0].Rows[0]["Number"].ToString());
            }

            return 0;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    public void SaveInMove(string strSDate)
    {
        Depository011_1BL bl = new Depository011_1BL();

        string Move_ID = bl.GetMove_ID(strSDate.Trim());


    }

    public MATERIEL_STOCKS_MOVE getexistModel(string strMoveDate, string strSerialNumber, int iMoveNumber, int iType, string strFactoryRID)
    {
        MATERIEL_STOCKS_MOVE model = null;
        string strSQL = SEL_MATERIEL_STOCKS_MOVE;

        if (5 == iType)
        {
            strSQL = strSQL + " and From_Factory_RID = 0 and To_Factory_RID <> ";
        }
        else
        {
            strSQL = strSQL + " and To_Factory_RID = 0  and From_Factory_RID <> ";
        }

        strSQL = strSQL + strFactoryRID;

        dirValues.Clear();
        dirValues.Add("Move_Date", strMoveDate);
        dirValues.Add("Serial_Number", strSerialNumber);
        dirValues.Add("Move_Number", iMoveNumber);

        try
        {
            model = dao.GetModel<MATERIEL_STOCKS_MOVE>(strSQL, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return model;
    }

    public bool isexistModel(string strMoveDate, string strSerialNumber, int iMoveNumber, int iType, string strFactoryRID)
    {
        DataSet ds = null;
        string strSQL = SEL_MATERIEL_STOCKS_MOVE;

        if (5 == iType)
        {
            strSQL = strSQL + " and From_Factory_RID = ";
        }
        else
        {
            strSQL = strSQL + " and To_Factory_RID = ";
        }

        strSQL = strSQL + strFactoryRID;

        dirValues.Clear();
        dirValues.Add("Move_Date", strMoveDate);
        dirValues.Add("Serial_Number", strSerialNumber);
        dirValues.Add("Move_Number", iMoveNumber);

        try
        {
            ds = dao.GetList(strSQL, dirValues);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return false;
    }

    public void delMove(string strFactoryRID, string strMoveDate, string strRCT)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Factory_RID", strFactoryRID);

            string strU1 = UPDATE_MATERIEL_STOCKS_MOVE_1;
            string strU2 = UPDATE_MATERIEL_STOCKS_MOVE_2;
            string strD1 = DEL_MATERIEL_STOCKS_MOVE_1;
            string strD2 = DEL_MATERIEL_STOCKS_MOVE_2;

            string strCon = "";

            if ("" == strRCT.Trim())
            {
                strCon = " and Move_Date = '" + strMoveDate + "'";
            }

            if ("" == strMoveDate.Trim())
            {
                strCon = " and convert(varchar(10),RCT,111) = '" + strRCT + "'";
            }


            dao.ExecuteNonQuery(strU1 + strCon, dirValues);
            dao.ExecuteNonQuery(strU2 + strCon, dirValues);
            dao.ExecuteNonQuery(strD1 + strCon, dirValues);
            dao.ExecuteNonQuery(strD2 + strCon, dirValues);

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizMsg.ALT_DEPOSITORY_010_10, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizMsg.ALT_DEPOSITORY_010_10);
        }
    }

    public void AddM(MATERIEL_STOCKS_MOVE model)
    {
        try
        {
            dao.OpenConnection();
            //dao.Add<MATERIEL_STOCKS_MOVE>(model);
            // rid = Convert.ToInt32(dao.AddAndGetID<MATERIEL_STOCKS_MOVE>(model, "Rid"));
            dirValues.Clear();
            dirValues.Add("Move_Date", model.Move_Date);
            dirValues.Add("RCU", model.RCU);
            dirValues.Add("RUU", ((USERS)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER]).UserID);
            dirValues.Add("RCT", DateTime.Now);
            dirValues.Add("RUT", DateTime.Now);
            dirValues.Add("RST", GlobalString.RST.ACTIVED);
            dirValues.Add("Move_Number", model.Move_Number);
            dirValues.Add("From_Factory_RID", model.From_Factory_RID);
            dirValues.Add("To_Factory_RID", model.To_Factory_RID);
            dirValues.Add("Move_ID", model.Move_ID);
            dirValues.Add("Serial_Number", model.Serial_Number);

            dao.ExecuteNonQuery(INSERT_MATERIEL_STOCKS_MOVE, dirValues);

            //操作日誌
            SetOprLog("2");

            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
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
    //add by Ian Huang end

}
