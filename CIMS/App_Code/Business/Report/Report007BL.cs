//******************************************************************
//*  作    者：bingyipan
//*  功能說明：廠商製卡卡數明細表 
//*  創建日期：2008-11-20
//*  修改日期：2008-12-16 15:00
//*  修改記錄：
//*            □2008-12-16
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
/// Report007BL 的摘要描述
/// </summary>
public class Report007BL : BaseLogic
{
    #region SQL語句
    //群組對應卡種
    public const string SEL_CARDTYPE_BY_GROUP = "select distinct ct.rid,fci.space_short_name name,fci.type+'-'+fci.affinity+'-'+fci.photo CardType from FACTORY_CHANGE_IMPORT fci left join card_type ct on ct.rst='A' and ct.type=fci.type and ct.affinity=fci.affinity and ct.photo=fci.photo left join group_card_type gct on gct.rst='A' and gct.cardtype_rid=ct.rid and gct.group_rid=@group_rid where fci.rst='A'";
    //群組對應晶片金融卡的版面簡稱
    public const string SEL_CARDTYPE_JP = "select distinct ct.replace_space_rid,ct2.name name2"
                            + " from group_card_type gct left join card_type ct"
                            + " on ct.rst='A' and gct.cardtype_rid=ct.rid and gct.group_rid=@group_rid"
                            + " left join card_type ct2 on ct2.rst='A' and ct.replace_space_rid=ct2.rid"
                            + " left join card_group cg on cg.rst='A' and cg.rid=gct.group_rid"
                            + " where gct.rst='A' and ct.name is not NULL and ct2.name is not NULL"
                            + " and cg.group_name='晶片金融卡'";

    public const string SEL_CARD_JP = "select distinct ct.name,ct.rid "+
                        "from SUBTOTAL_IMPORT si inner join card_type ct on ct.rst='A' and si.old_cardtype_rid <>0 and si.old_cardtype_rid = ct.rid " +
                        "inner join card_type ct1 on ct1.rst = 'A' and si.Type = ct1.Type and si.AFFINITY = ct1.AFFINITY and si.Photo = ct1.Photo " + 
                        "where si.rst='A' and ct1.rid in (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @group_rid) ";

    public const string SEL_NAME_JP = "select distinct ct.rid,ct.name ,si.type,si.affinity,si.photo from SUBTOTAL_IMPORT si left join card_type ct on ct.rst='A' and si.type=ct.type and si.affinity=ct.affinity and si.photo=ct.photo left join group_card_type gct on gct.rst='A' and gct.cardtype_rid=ct.rid where si.rst='A' and gct.group_rid=@group_rid";

    //群組對應卡種信息
    public const string SEL_CARD_TYPE = "select ct.* from group_card_type gct"
                            + " left join card_type ct on ct.rst='A' and ct.rid=gct.cardtype_rid"
                            + " where gct.rst='A' and name is not NULL and gct.group_rid=@group_rid";
    //取得固定卡種狀態以外的狀態
    public const string SEL_CARDSTATUS = "select * from CARDTYPE_STATUS where rst='A' and ("
                            + " status_name<>'缺卡' and status_name<>'未製卡' and status_name<>'補製卡' and"
                            + " status_name<>'製成卡' and status_name<>'樣卡' and status_name<>'製損卡' and"
                            + " status_name<>'感應不良' and status_name<>'排卡' and status_name<>'銷毀' and"
                            + " status_name<>'調整' and status_name<>'消耗卡') and "
                            + " status_code>50 and is_delete='Y'";
    //指定卡種信息
    public const string SEL_CARDTYPE = "select * from card_type where rst='A' and rid=@rid";
    //群組對應製卡類別
    public const string SEL_MAKE_CARD_TYPE = "select mct.rid makecardtype_rid,mct.cardgroup_rid,mct.type_name"
                                          + " from MAKE_CARD_TYPE mct"
                                          + " where mct.rst='A' and mct.is_report='Y' and mct.cardgroup_rid=@cardgroup_rid";
    //製卡類別對應小記檔number和
    public const string SEL_IMPORT_NUMBER = "select ISNULL(sum(number),0) Sum_number from SUBTOTAL_IMPORT"
                                  + " where rst='A' and perso_factory_rid=@perso_factory_rid and date_time=@date_time"
                                  + " and (type=@type and affinity=@affinity and photo=@photo)"
                                  + " and makecardtype_rid=@makecardtype_rid";
    public const string SEL_IMPORT_NUMBER2 = "select ISNULL(sum(number),0) Sum_number from SUBTOTAL_IMPORT"
                                  + " where rst='A' and perso_factory_rid=@perso_factory_rid and date_time=@date_time"
                                  + " and (type=@type and affinity=@affinity and photo=@photo)";
    public const string SEL_IMPORT_NUMBER3 = "select ISNULL(sum(number),0) Sum_number from SUBTOTAL_IMPORT"
                                  + " where rst='A' and date_time=@date_time"
                                  + " and (type=@type and affinity=@affinity and photo=@photo)"
                                  + " and makecardtype_rid=@makecardtype_rid";
    public const string SEL_IMPORT_NUMBER4 = "select ISNULL(sum(number),0) Sum_number from SUBTOTAL_IMPORT"
                                  + " where rst='A' and date_time=@date_time"
                                  + " and (type=@type and affinity=@affinity and photo=@photo)";  
    //根據製卡類別名稱獲得對應編號
    public const string SEL_MAKECARDRID = "select rid makecardtype_rid from MAKE_CARD_TYPE where rst='A'"
                                    + " and is_report='Y' and type_name=@type_name and cardgroup_rid=@Card_Group_RID";
    //廠商庫存異動number(即各狀態number和)
    public const string SEL_STATUS_NUMBER = "select ISNULL(sum(number),0) from FACTORY_CHANGE_IMPORT where rst='A'"
                            + " and perso_factory_rid=@perso_factory_rid and date_time=@date_time"
                            + " and (type=@type and affinity=@affinity and photo=@photo)"
                            + " and status_rid in (select rid from CARDTYPE_STATUS where rst='A'"
                            + " and status_name=@status_name)";
    public const string SEL_STATUS_NUMBER2 = "select ISNULL(sum(number),0) from FACTORY_CHANGE_IMPORT where rst='A'"
                            + " and date_time=@date_time"
                            + " and (type=@type and affinity=@affinity and photo=@photo)"
                            + " and status_rid in (select rid from CARDTYPE_STATUS where rst='A'"
                            + " and status_name=@status_name)";
    //公式定義(製成卡或消耗卡)
    public const string SEL_EXPRESSIONS = "select cs.status_name,ed.type_rid,ed.operate,p.param_name from EXPRESSIONS_DEFINE ed"
                            + " left join param p on p.rst='A' and p.paramtype_name='公式名稱'"
                            + " and p.param_code=ed.expressions_rid"
                            + " left join CARDTYPE_STATUS cs on cs.rst='A' and cs.rid=ed.type_rid"
                            + " where ed.rst='A'";
    //入庫
    public const string SEL_STOCKS = "select * from DEPOSITORY_STOCK where rst='A' and Space_Short_RID=@CardType_RID"
                            + " and Income_Date=@Date_Time and Perso_Factory_RID=@perso_factory_rid";
    public const string SEL_STOCKS2 = "select * from DEPOSITORY_STOCK where rst='A' and Space_Short_RID=@CardType_RID"
                            + " and Income_Date=@Date_Time";
    //退貨
    public const string SEL_CANCEL = "select * from DEPOSITORY_CANCEL where rst='A' and Space_Short_RID=@CardType_RID"
                            + " and Cancel_Date=@Date_Time and Perso_Factory_RID=@perso_factory_rid";
    public const string SEL_CANCEL2 = "select * from DEPOSITORY_CANCEL where rst='A' and Space_Short_RID=@CardType_RID"
                            + " and Cancel_Date=@Date_Time";
    //再入庫
    public const string SEL_RESTOCK = "select * from DEPOSITORY_RESTOCK where rst='A' and Space_Short_RID=@CardType_RID"
                            + " and Reincome_Date=@Date_Time and Perso_Factory_RID=@perso_factory_rid";
    public const string SEL_RESTOCK2 = "select * from DEPOSITORY_RESTOCK where rst='A' and Space_Short_RID=@CardType_RID"
                            + " and Reincome_Date=@Date_Time";
    //移轉(移入為正)
    public const string SEL_MOVE_TO = "select * from CARDTYPE_STOCKS_MOVE where rst='A'"
                            + " and to_factory_rid=@perso_factory_rid and cardtype_rid=@CardType_RID and "
                            + " move_date=@date_time";   
    //移轉(移出為負)
    public const string SEL_MOVE_FROM = "select * from CARDTYPE_STOCKS_MOVE where rst='A'"
                            + " and from_factory_rid=@perso_factory_rid and cardtype_rid=@CardType_RID and "
                            + " move_date=@date_time";    

    // add by zwl 20090210
    // 小計檔
    public const string SEL_SUBTOTAL_SUM_ALL_FACTORY = "select SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,MCT.Type_Name,SUM(Number) AS SUM_Number "
                      + " from SUBTOTAL_IMPORT SI INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID "
                      + " INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                      + " where SI.RST='A' AND Date_Time = @date_time AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                      + " group by SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,MCT.Type_Name";
    public const string SEL_SUBTOTAL_SUM = "select SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,MCT.Type_Name,SUM(Number) AS SUM_Number "
                      + " from SUBTOTAL_IMPORT SI INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID "
                      + " INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                      + " where SI.RST='A' AND SI.Date_Time = @date_time AND SI.Perso_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                      + " group by SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,MCT.Type_Name";
    // 小計檔（晶片金融卡）
    public const string SEL_SUBTOTAL_SUM_ALL_FACTORY_JP = "select SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,CT1.Name AS Old_Name,SUM(Number) AS SUM_Number "
                      + " from SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT1 ON CT1.RST = 'A' AND SI.Old_CardType_RID = CT1.RID "
                      + " INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                      + " where SI.RST='A' AND Date_Time = @date_time AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                      + " group by SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,CT1.Name";
    public const string SEL_SUBTOTAL_SUM_JP = "select SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,CT1.Name AS Old_Name,SUM(Number) AS SUM_Number "
                      + " from SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT1 ON CT1.RST = 'A' AND SI.Old_CardType_RID = CT1.RID "
                      + " INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                      + " where SI.RST='A' AND Date_Time = @date_time AND SI.Perso_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                      + " group by SI.TYPE,SI.AFFINITY,SI.PHOTO,CT.Name,CT1.Name";
    // 廠商庫存異動
    public const string SEL_FACTORY_CHANGE_IMPORT_ALL_FACTORY = "select FCI.TYPE,FCI.AFFINITY,FCI.PHOTO,FCI.Space_Short_Name,CS.Status_Name,SUM(Number) AS SUM_Number "
                      + " FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID "
                      + " INNER JOIN Card_Type CT ON CT.RST = 'A' AND FCI.Type = CT.Type AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO "
                      + " WHERE FCI.RST = 'A' AND FCI.Date_Time = @date_time AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                      + " group by FCI.TYPE,FCI.AFFINITY,FCI.PHOTO,FCI.Space_Short_Name,CS.Status_Name";
    public const string SEL_FACTORY_CHANGE_IMPORT = "select FCI.TYPE,FCI.AFFINITY,FCI.PHOTO,FCI.Space_Short_Name,CS.Status_Name,SUM(Number) AS SUM_Number "
                      + " FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND FCI.Status_RID = CS.RID "
                      + " INNER JOIN Card_Type CT ON CT.RST = 'A' AND FCI.Type = CT.Type AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO "
                      + " WHERE FCI.RST = 'A' AND FCI.Date_Time = @date_time AND FCI.Perso_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                      + " group by FCI.TYPE,FCI.AFFINITY,FCI.PHOTO,FCI.Space_Short_Name,CS.Status_Name";
    // 入庫
    public const string SEL_DEPOSITORY_STOCK_ALL_FACTORY = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(Income_Number) AS SUM_Number "
                          + " FROM DEPOSITORY_STOCK DS INNER JOIN Card_Type CT ON CT.RST = 'A' AND DS.Space_Short_RID = CT.RID "
                          + " WHERE DS.RST = 'A' AND DS.Income_Date = @date_time AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    public const string SEL_DEPOSITORY_STOCK = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(Income_Number) AS SUM_Number "
                          + " FROM DEPOSITORY_STOCK DS INNER JOIN Card_Type CT ON CT.RST = 'A' AND DS.Space_Short_RID = CT.RID "
                          + " WHERE DS.RST = 'A' AND DS.Income_Date = @date_time AND DS.Perso_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    // 退貨
    public const string SEL_DEPOSITORY_CANCEL_ALL_FACTORY = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(Cancel_Number) AS SUM_Number "
                          + " FROM DEPOSITORY_CANCEL DC INNER JOIN Card_Type CT ON CT.RST = 'A' AND DC.Space_Short_RID = CT.RID "
                          + " WHERE DC.RST = 'A' AND DC.Cancel_Date = @date_time AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    public const string SEL_DEPOSITORY_CANCEL = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(Cancel_Number) AS SUM_Number "
                          + " FROM DEPOSITORY_CANCEL DC INNER JOIN Card_Type CT ON CT.RST = 'A' AND DC.Space_Short_RID = CT.RID "
                          + " WHERE DC.RST = 'A' AND DC.Cancel_Date = @date_time AND DC.Perso_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    // 再入庫
    public const string SEL_DEPOSITORY_RESTOCK_ALL_FACTORY = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(ReIncome_Number) AS SUM_Number "
                          + " FROM DEPOSITORY_RESTOCK DR INNER JOIN Card_Type CT ON CT.RST = 'A' AND DR.Space_Short_RID = CT.RID "
                          + " WHERE DR.RST = 'A' AND DR.Reincome_Date = @date_time AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    public const string SEL_DEPOSITORY_RESTOCK = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(ReIncome_Number) AS SUM_Number "
                          + " FROM DEPOSITORY_RESTOCK DR INNER JOIN Card_Type CT ON CT.RST = 'A' AND DR.Space_Short_RID = CT.RID "
                          + " WHERE DR.RST = 'A' AND DR.Reincome_Date = @date_time AND DR.Perso_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    public const string SEL_CARDTYPE_STOCKS_MOVE_IN = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(Move_Number) AS SUM_Number "
                          + " FROM CARDTYPE_STOCKS_MOVE CTSM INNER JOIN Card_Type CT ON CT.RST = 'A' AND CTSM.CardType_RID = CT.RID "
                          + " WHERE CTSM.RST = 'A' AND CTSM.Move_Date = @date_time AND CTSM.To_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    public const string SEL_CARDTYPE_STOCKS_MOVE_OUT = "select CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name,SUM(Move_Number) AS SUM_Number "
                          + " FROM CARDTYPE_STOCKS_MOVE CTSM INNER JOIN Card_Type CT ON CT.RST = 'A' AND CTSM.CardType_RID = CT.RID "
                          + " WHERE CTSM.RST = 'A' AND CTSM.Move_Date = @date_time AND CTSM.From_Factory_RID = @perso_factory_rid AND CT.RID IN (SELECT distinct CardType_RID FROM GROUP_CARD_TYPE WHERE RST = 'A' AND Group_RID = @cardtype_group_rid) "
                          + " group by CT.TYPE,CT.AFFINITY,CT.PHOTO,CT.Name ";
    public const string SEL_EXPRESSIONS_DEFINE = "select ED.Operate,CS.Status_Code,CS.Status_Name "
                          + "from EXPRESSIONS_DEFINE ED INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND ED.Type_RID = CS.RID "
                          + "where ED.RST = 'A' AND ED.Expressions_RID = @expressions_rid AND ED.Operate<>'捨'";

    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Report007BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 根據群組獲得對應卡種
    /// </summary>
    /// <param name="strGroup">群組編號</param>
    /// <param name="isres">true:是晶片金融卡 false:不是晶片金融卡</param>
    /// <returns></returns>
    public DataSet getCardType(string strGroupRID,bool isres)
    {
        DataSet ds = null;
        try
        {

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();
            dirValues.Add("group_rid", strGroupRID);
            if (isres == true)
            {
                stbWhere.Append(" and cg.group_name='晶片金融卡' order by rid");
            }
            else
            {
                stbWhere.Append(" and cg.group_name<>'晶片金融卡' order by rid");
            }

            ds = dao.GetList(SEL_CARDTYPE_BY_GROUP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return ds;
    }

    //根據群組獲得對應晶片金融卡的卡種版面簡稱
    public DataSet getCardName(string strGroupRID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("group_rid", strGroupRID);

            ds = dao.GetList(SEL_CARD_JP, dirValues);
            return ds;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    //執行存儲過程并查詢結果
    public string getNumberByName(Dictionary<string, object> searchInput, string strName1, string strName2)
    {
        DataSet ds = null;
        try
        {

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();
                       
            dirValues.Clear();
            dirValues.Add("Date_Time", searchInput["txtDate_Time"].ToString());
            dirValues.Add("Card_Group_RID", searchInput["dropCard_Group_RID"].ToString());
            dirValues.Add("Perso_Factory", searchInput["dropFactory"].ToString());
            dao.ExecuteNonQuery("proc_report007_rows2", dirValues, true);

            dirValues.Clear();
            dirValues.Add("strName1", strName1);
            dirValues.Add("strName2", strName2);

            ds = dao.GetList("select ISNULL(sumn,0) sumn from RPT_report007_2 where card_type=@strName1 and i=@strName2", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "0";
        }
    }

    //根據群組獲得對應晶片金融卡的卡種替換版面
    public DataSet getCardNameByJP(string strGroupRID)
    {
        DataSet ds = null;
        try
        {

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();
            dirValues.Add("group_rid", strGroupRID);

            ds = dao.GetList(SEL_NAME_JP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return ds;
    }

    /// <summary>
    /// 群組對應晶片金融卡的版面簡稱
    /// </summary>
    /// <param name="strGroupRID">群組編號</param>
    /// <returns></returns>
    public DataSet getCardType(string strGroupRID)
    {
        DataSet ds = null;
        try
        {

            dirValues.Clear();
            dirValues.Add("group_rid", strGroupRID);

            ds = dao.GetList(SEL_CARDTYPE_JP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return ds;
    }

    /// <summary>
    /// 根據群組獲得對應卡種信息
    /// </summary>
    /// <param name="strGroupRID"></param>
    /// <returns></returns>
    public DataSet getCardTypeInfo(string strGroupRID)
    {
        DataSet ds = null;
        try
        {

            dirValues.Clear();
            dirValues.Add("group_rid", strGroupRID);

            ds = dao.GetList(SEL_CARD_TYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return ds;
    }

    public DataSet getCardTypeInfo(string strGroupRID,string CardType_RID)
    {
        DataSet ds = null;
        try
        {

            dirValues.Clear();
            dirValues.Add("rid", CardType_RID);

            ds = dao.GetList(SEL_CARDTYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return ds;
    }

    /// <summary>
    /// 根據製卡類別名稱獲得對應編號
    /// </summary>
    /// <param name="strTypeName"></param>
    /// <returns></returns>
    public string getMakeCardRID(string strTypeName, string Card_Group_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("type_name", strTypeName);
            dirValues.Add("Card_Group_RID", Card_Group_RID);

            ds = dao.GetList(SEL_MAKECARDRID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        if(ds!=null&&ds.Tables[0].Rows.Count>0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 根據群組獲得對應製卡類別
    /// </summary>
    /// <param name="strGroupRID"></param>
    /// <returns></returns>
    public DataSet getType(string strGroupRID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("cardgroup_rid", strGroupRID);
            ds = dao.GetList(SEL_MAKE_CARD_TYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return ds;
    }

    /// <summary>
    /// 獲得除指定狀態以外的其他卡種狀態
    /// </summary>
    /// <returns></returns>
    public DataSet getOtherCardStatus()
    {
        DataSet ds = null;

        try
        {
            ds = dao.GetList(SEL_CARDSTATUS);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return ds;
    }

    /// <summary>
    /// 根據卡種、Perso廠、日期、製卡類別獲得對應小記檔number
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="makecardtype_rid">製卡類別編號</param>
    /// <param name="CardType_RID">卡種RID</param>
    /// <returns></returns>
    public string getIMPORT_NUMBER(Dictionary<string, object> searchInput, string makecardtype_rid, string CardType_RID)
    {
        DataSet ds = new DataSet();

        string res = "0";

        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                if (CardType_RID.ToString().Trim() != "")
                {
                    //獲得卡種信息
                    ds = getCardTypeInfo("", CardType_RID);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dirValues.Clear();
                        dirValues.Add("type", ds.Tables[0].Rows[0]["type"].ToString());
                        dirValues.Add("affinity", ds.Tables[0].Rows[0]["affinity"].ToString());
                        dirValues.Add("photo", ds.Tables[0].Rows[0]["photo"].ToString());
                        dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                        dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());
                        dirValues.Add("makecardtype_rid", makecardtype_rid);

                        DataSet dstmp = new DataSet();
                        //通過上述參數得到狀態的number和
                        dstmp = dao.GetList(SEL_IMPORT_NUMBER, dirValues);
                        if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                        {
                            res = dstmp.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return res;
    }

    public string getIMPORT_NUMBER2(Dictionary<string, object> searchInput, string makecardtype_rid, string CardType_RID)
    {
        DataSet ds = new DataSet();

        string res = "0";
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                if (CardType_RID.ToString().Trim() != "")
                {
                    //獲得卡種信息
                    ds = getCardTypeInfo("", CardType_RID);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dirValues.Clear();
                        dirValues.Add("type", ds.Tables[0].Rows[0]["type"].ToString());
                        dirValues.Add("affinity", ds.Tables[0].Rows[0]["affinity"].ToString());
                        dirValues.Add("photo", ds.Tables[0].Rows[0]["photo"].ToString());
                        dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                        dirValues.Add("makecardtype_rid", makecardtype_rid);

                        DataSet dstmp = new DataSet();
                        //通過上述參數得到狀態的number和
                        dstmp = dao.GetList(SEL_IMPORT_NUMBER3, dirValues);
                        if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                        {
                            res = dstmp.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return res;
    }

    public string getIMPORT_NUMBER(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = new DataSet();

        string res = "0";
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                if (CardType_RID.ToString().Trim() != "")
                {
                    //獲得卡種信息
                    ds = getCardTypeInfo("", CardType_RID);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dirValues.Clear();
                        dirValues.Add("type", ds.Tables[0].Rows[0]["type"].ToString());
                        dirValues.Add("affinity", ds.Tables[0].Rows[0]["affinity"].ToString());
                        dirValues.Add("photo", ds.Tables[0].Rows[0]["photo"].ToString());
                        dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                        dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());

                        DataSet dstmp = new DataSet();
                        //通過上述參數得到狀態的number和
                        dstmp = dao.GetList(SEL_IMPORT_NUMBER2, dirValues);
                        if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                        {
                            res = dstmp.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return res;
    }

    public string getIMPORT_NUMBER2(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = new DataSet();

        string res = "0";
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                if (CardType_RID.ToString().Trim() != "")
                {
                    //獲得卡種信息
                    ds = getCardTypeInfo("", CardType_RID);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dirValues.Clear();
                        dirValues.Add("type", ds.Tables[0].Rows[0]["type"].ToString());
                        dirValues.Add("affinity", ds.Tables[0].Rows[0]["affinity"].ToString());
                        dirValues.Add("photo", ds.Tables[0].Rows[0]["photo"].ToString());
                        dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());

                        DataSet dstmp = new DataSet();
                        //通過上述參數得到狀態的number和
                        dstmp = dao.GetList(SEL_IMPORT_NUMBER4, dirValues);
                        if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                        {
                            res = dstmp.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return res;
    }

    /// <summary>
    /// 根據卡種獲得廠商庫存異動的狀況number和
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="str">狀態</param>
    /// <param name="CardType_RID">卡種RID</param>
    /// <returns></returns>
    public string getSTATUS_NUMBER(Dictionary<string, object> searchInput, string str, string CardType_RID)
    {
        DataSet ds = new DataSet();

        string res = "0";
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                if (CardType_RID.ToString().Trim() != "")
                {
                    //獲得卡種信息
                    ds = getCardTypeInfo("", CardType_RID);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dirValues.Clear();
                        dirValues.Add("type", ds.Tables[0].Rows[0]["type"].ToString());
                        dirValues.Add("affinity", ds.Tables[0].Rows[0]["affinity"].ToString());
                        dirValues.Add("photo", ds.Tables[0].Rows[0]["photo"].ToString());
                        dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                        dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());
                        dirValues.Add("status_name", str);

                        DataSet dstmp = new DataSet();
                        //通過上述參數得到狀態的number和
                        dstmp = dao.GetList(SEL_STATUS_NUMBER, dirValues);
                        if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                        {
                            res = dstmp.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return res;
    }

    public string getSTATUS_NUMBER2(Dictionary<string, object> searchInput, string str, string CardType_RID)
    {
        DataSet ds = new DataSet();

        string res = "0";
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                if (CardType_RID.ToString().Trim() != "")
                {
                    //獲得卡種信息
                    ds = getCardTypeInfo("", CardType_RID);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dirValues.Clear();
                        dirValues.Add("type", ds.Tables[0].Rows[0]["type"].ToString());
                        dirValues.Add("affinity", ds.Tables[0].Rows[0]["affinity"].ToString());
                        dirValues.Add("photo", ds.Tables[0].Rows[0]["photo"].ToString());
                        dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                        dirValues.Add("status_name", str);

                        DataSet dstmp = new DataSet();
                        //通過上述參數得到狀態的number和
                        dstmp = dao.GetList(SEL_STATUS_NUMBER2, dirValues);
                        if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                        {
                            res = dstmp.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return res;
    }

    /// <summary>
    /// 獲得指定卡種和狀況集合按指定公式求number和
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="CardType_RID">卡種RID</param>
    /// <param name="str">製成卡數/消耗卡數</param>
    /// <returns></returns>
    public string getEXPRESSIONSNumber(Dictionary<string, object> searchInput, string str, string CardType_RID,string CardGroup_RID)
    {
        long result = 0;
        try
        {
            DataSet ds = null;

            DataTable dst = new DataTable();
            dst.Columns.Add("Status_Number");//狀況number和
            dst.Columns.Add("Status_Name");//狀況名稱
            dst.Columns.Add("Operate");//狀況公式符號       

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();
            dirValues.Add("param_name", str);
            stbWhere.Append(" and p.param_name=@param_name");

            //獲得指定公式的所有狀況
            ds = dao.GetList(SEL_EXPRESSIONS + stbWhere.ToString(), dirValues);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //獲得指定卡種和狀況的number和
                    string sn = getSTATUS_NUMBER(searchInput, ds.Tables[0].Rows[i]["status_name"].ToString(), CardType_RID);
                    string dn = getIMPORT_NUMBER(searchInput, getMakeCardRID(ds.Tables[0].Rows[i]["status_name"].ToString(), CardGroup_RID), CardType_RID);

                    DataRow dr = dst.NewRow();
                    dr[0] = sn;
                    dr[1] = ds.Tables[0].Rows[i]["status_name"].ToString();
                    dr[2] = ds.Tables[0].Rows[i]["operate"].ToString();
                    dst.Rows.Add(dr);

                    DataRow dr2 = dst.NewRow();
                    dr2[0] = dn;
                    dr2[1] = ds.Tables[0].Rows[i]["status_name"].ToString();
                    dr2[2] = ds.Tables[0].Rows[i]["operate"].ToString();
                    dst.Rows.Add(dr2);
                }

                if (dst != null && dst.Rows.Count > 0)
                {
                    for (int m = 0; m < dst.Rows.Count; m++)
                    {
                        //根據公式做加減操作
                        if (dst.Rows[m]["Operate"].ToString() == "+")
                        {
                            result += Convert.ToInt64(dst.Rows[m][0].ToString());
                        }
                        else
                        {
                            result -= Convert.ToInt64(dst.Rows[m][0].ToString());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return result.ToString();
    }

    public string getEXPRESSIONSNumber2(Dictionary<string, object> searchInput, string str, string CardType_RID,string CardGroup_RID)
    {
        long result = 0;

        try
        {
            DataSet ds = null;

            DataTable dst = new DataTable();
            dst.Columns.Add("Status_Number");//狀況number和
            dst.Columns.Add("Status_Name");//狀況名稱
            dst.Columns.Add("Operate");//狀況公式符號       

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();
            dirValues.Add("param_name", str);
            stbWhere.Append(" and p.param_name=@param_name");

            //獲得指定公式的所有狀況
            ds = dao.GetList(SEL_EXPRESSIONS + stbWhere.ToString(), dirValues);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //獲得指定卡種和狀況的number和
                    string sn = getSTATUS_NUMBER2(searchInput, ds.Tables[0].Rows[i]["status_name"].ToString(), CardType_RID);

                    string dn = getIMPORT_NUMBER2(searchInput, getMakeCardRID(ds.Tables[0].Rows[i]["status_name"].ToString(), CardGroup_RID), CardType_RID);
                    
                    DataRow dr = dst.NewRow();
                    dr[0] = sn;
                    dr[1] = ds.Tables[0].Rows[i]["status_name"].ToString();
                    dr[2] = ds.Tables[0].Rows[i]["operate"].ToString();
                    dst.Rows.Add(dr);

                    DataRow dr2 = dst.NewRow();
                    dr2[0] = dn;
                    dr2[1] = ds.Tables[0].Rows[i]["status_name"].ToString();
                    dr2[2] = ds.Tables[0].Rows[i]["operate"].ToString();
                    dst.Rows.Add(dr2);
                }

                if (dst != null && dst.Rows.Count > 0)
                {
                    for (int m = 0; m < dst.Rows.Count; m++)
                    {
                        //根據公式做加減操作
                        if (dst.Rows[m]["Operate"].ToString() == "+")
                        {
                            result += Convert.ToInt64(dst.Rows[m][0].ToString());
                        }
                        else
                        {
                            result -= Convert.ToInt64(dst.Rows[m][0].ToString());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return result.ToString();
    }

    /// <summary>
    /// 獲得卡種、Perso、日期的入庫量
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="CardType_RID">卡種RID</param>    
    /// <returns></returns>
    public string getSTOCKS(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());
                ds = dao.GetList(SEL_STOCKS, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Income_Number"].ToString());
            }
            return res.ToString();
        }
        else
        {
            return "0";
        }
    }

    public string getSTOCKS2(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                ds = dao.GetList(SEL_STOCKS2, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Income_Number"].ToString());
            }
            return res.ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 獲得卡種、Perso、日期的退貨量
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="CardType_RID">卡種RID</param>    
    /// <returns></returns>
    public string getCANCEL(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());
                ds = dao.GetList(SEL_CANCEL, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Cancel_Number"].ToString());
            }
            return res.ToString();
        }
        else
        {
            return "0";
        }
    }

    public string getCANCEL2(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                ds = dao.GetList(SEL_CANCEL2, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Cancel_Number"].ToString());
            }
            return res.ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 獲得卡種、Perso、日期的再入庫量
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="CardType_RID">卡種RID</param>    
    /// <returns></returns>
    public string getRESTOCK(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());
                ds = dao.GetList(SEL_RESTOCK, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Reincome_Number"].ToString());
            }
            return res.ToString();
        }
        else
        {
            return "0";
        }
    }

    public string getRESTOCK2(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                ds = dao.GetList(SEL_RESTOCK2, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Reincome_Number"].ToString());
            }
            return res.ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 卡種、Perso廠對應庫存轉移數量(移入為正)
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="CardType_RID">卡種RID</param>    
    /// <returns></returns>
    public string getMOVETO(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());
                ds = dao.GetList(SEL_MOVE_TO, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }


        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Move_Number"].ToString());
            }
            return res.ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 卡種、Perso廠對應庫存轉移數量(移出為負)
    /// </summary>
    /// <param name="searchInput">包括：perso廠、日期</param>
    /// <param name="CardType_RID">卡種RID</param>    
    /// <returns></returns>
    public string getMOVEFROM(Dictionary<string, object> searchInput, string CardType_RID)
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
            {
                dirValues.Add("CardType_RID", CardType_RID);
                dirValues.Add("date_time", searchInput["txtDate_Time"].ToString().Trim());
                dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString().Trim());
                ds = dao.GetList(SEL_MOVE_FROM, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        long res = 0;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res += Convert.ToInt64(ds.Tables[0].Rows[i]["Move_Number"].ToString());
            }
            return "-"+res.ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 取卡片移出信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllCardTypeStocksMoveOut(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllCardTypeStocksMoveOut = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time", DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            dsAllCardTypeStocksMoveOut = dao.GetList(SEL_CARDTYPE_STOCKS_MOVE_OUT, dirValues);

            if (null != dsAllCardTypeStocksMoveOut && dsAllCardTypeStocksMoveOut.Tables.Count > 0)
            {
                return dsAllCardTypeStocksMoveOut.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取卡片移入信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllCardTypeStocksMoveIn(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllCardTypeStocksMoveIn = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time", DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            dsAllCardTypeStocksMoveIn = dao.GetList(SEL_CARDTYPE_STOCKS_MOVE_IN, dirValues);

            if (null != dsAllCardTypeStocksMoveIn && dsAllCardTypeStocksMoveIn.Tables.Count > 0)
            {
                return dsAllCardTypeStocksMoveIn.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取卡片再入庫信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllDepositoryReStock(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllDepositoryReStock = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time", DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            // 全部
            if (searchInput["dropFactory"].ToString() == "")
            {
                dsAllDepositoryReStock = dao.GetList(SEL_DEPOSITORY_RESTOCK_ALL_FACTORY, dirValues);
            }
            else
            {
                dsAllDepositoryReStock = dao.GetList(SEL_DEPOSITORY_RESTOCK, dirValues);
            }

            if (null != dsAllDepositoryReStock && dsAllDepositoryReStock.Tables.Count > 0)
            {
                return dsAllDepositoryReStock.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取卡片退貨信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllDepositoryCancel(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllDepositoryCancel = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time", DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            // 全部
            if (searchInput["dropFactory"].ToString() == "")
            {
                dsAllDepositoryCancel = dao.GetList(SEL_DEPOSITORY_CANCEL_ALL_FACTORY, dirValues);
            }
            else
            {
                dsAllDepositoryCancel = dao.GetList(SEL_DEPOSITORY_CANCEL, dirValues);
            }

            if (null != dsAllDepositoryCancel && dsAllDepositoryCancel.Tables.Count > 0)
            {
                return dsAllDepositoryCancel.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取卡片入庫信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllDepositoryStock(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllDepositoryStock = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time", DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            // 全部
            if (searchInput["dropFactory"].ToString() == "")
            {
                dsAllDepositoryStock = dao.GetList(SEL_DEPOSITORY_STOCK_ALL_FACTORY, dirValues);
            }
            else
            {
                dsAllDepositoryStock = dao.GetList(SEL_DEPOSITORY_STOCK, dirValues);
            }

            if (null != dsAllDepositoryStock && dsAllDepositoryStock.Tables.Count > 0)
            {
                return dsAllDepositoryStock.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取卡片庫存異動信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllFactoryChangeImport(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllFactoryChangeImport = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time",DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            // 全部
            if (searchInput["dropFactory"].ToString() == "")
            {
                dsAllFactoryChangeImport = dao.GetList(SEL_FACTORY_CHANGE_IMPORT_ALL_FACTORY, dirValues);
            }
            else
            {
                dsAllFactoryChangeImport = dao.GetList(SEL_FACTORY_CHANGE_IMPORT, dirValues);
            }

            if (null != dsAllFactoryChangeImport && dsAllFactoryChangeImport.Tables.Count > 0)
            {
                return dsAllFactoryChangeImport.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取小計檔信息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllSubTotal(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllTotal = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time", DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            // 全部
            if (searchInput["dropFactory"].ToString() == "")
            {
                dsAllTotal = dao.GetList(SEL_SUBTOTAL_SUM_ALL_FACTORY, dirValues);
            }
            else
            {
                dsAllTotal = dao.GetList(SEL_SUBTOTAL_SUM, dirValues);
            }

            if (null != dsAllTotal && dsAllTotal.Tables.Count > 0)
            {
                return dsAllTotal.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取小計檔信息(晶片金融卡)
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getAllSubTotal_JP(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet dsAllTotal = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("date_time", DateTime.Parse(searchInput["txtDate_Time"].ToString()).ToString("yyyy-MM-dd"));
            this.dirValues.Add("cardtype_group_rid", searchInput["dropCard_Group_RID"].ToString());
            this.dirValues.Add("perso_factory_rid", searchInput["dropFactory"].ToString());
            // 全部
            if (searchInput["dropFactory"].ToString() == "")
            {
                dsAllTotal = dao.GetList(SEL_SUBTOTAL_SUM_ALL_FACTORY_JP, dirValues);
            }
            else
            {
                dsAllTotal = dao.GetList(SEL_SUBTOTAL_SUM_JP, dirValues);
            }

            if (null != dsAllTotal && dsAllTotal.Tables.Count > 0)
            {
                return dsAllTotal.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 取製成卡、消耗卡公式
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <returns></returns>
    public DataTable getExpressions(int intExpressions_RID)
    {
        try
        {
            DataSet dsExpressions = new DataSet();
            this.dirValues.Clear();
            this.dirValues.Add("expressions_rid", intExpressions_RID.ToString());

            dsExpressions = dao.GetList(SEL_EXPRESSIONS_DEFINE, dirValues);
            if (null != dsExpressions && dsExpressions.Tables.Count > 0)
            {
                return dsExpressions.Tables[0];
            }
            return null;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }
}
