//******************************************************************
//*  作    者：Ray
//*  功能說明：Report017 Business
//*  創建日期：2008/12/15
//*  修改日期：
//*  修改記錄：
//*            
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
/// Summary description
/// </summary>
public class Report017 : BaseLogic
{
    //根據時間,類型,卡種ID取得卡種數量
    const string SEL_CARD_NEW = "select sum(a.Number) from dbo.SUBTOTAL_IMPORT as a left join dbo.CARD_TYPE as b on (a.TYPE = b.TYPE and a.AFFINITY = b.AFFINITY and a.PHOTO = b.PHOTO) where a.RST='A' and DateDiff(day,a.Date_Time,@Date)=0 and Action = @Action and b.RID = @RID";
    const string SEL_GROUP_NEW = "select sum(Number) from  dbo.RPT_report017  where gourporcard = 3 and grouprid in (select CardType_RID from GROUP_CARD_TYPE where Group_RID =@GroupID) and DateDiff(day,[day],@Date)=0 and cardid = @Type and RCT=@RCT";
    //取得製成卡、消耗卡數量
    const string SEL_SYS_CARD = "select c.Number,d.Operate from Factory_Change_Import as c left join (select b.RID,a.Operate from EXPRESSIONS_DEFINE as a left join CARDTYPE_STATUS as b on a.Type_RID = b.RID where a.RST = 'A' and Expressions_RID = @TypeFlag) as d on c.Status_RID = d.RID where c.RST = 'A' and c.Status_RID in (select b.RID from EXPRESSIONS_DEFINE as a left join CARDTYPE_STATUS as b on a.Type_RID = b.RID where a.RST = 'A' and Expressions_RID = @TypeFlag) and c.Type = @Type and c.AFFINITY = @AFFINITY and c.PHOTO = @Photo and Date_Time = @Date";
    const string SEL_CARD_TAP = "select TYPE,AFFINITY,PHOTO,Name from CARD_TYPE where RID = @RID and RST='A'";
    const string SEL_BAD_CARD = "select  sum(Number) from dbo.FACTORY_CHANGE_IMPORT WHERE RST = 'A' and Status_RID = 9 and Date_Time =@Date and [TYPE] = @Type  and AFFINITY = @AFFINITY and PHOTO = @Photo";
    const string INSERT_DATA = "insert into RPT_Report017 (gourporcard,grouporcardname,grouprid,cardname,cardid,day,number,RCT) values(@GroupOrCard,@Name,@RID,@CardName,@CardID,@Day,@Number,@RCT)";
    const string SEL_MADE_GROUP = "select RID,Group_Name from CARD_GROUP where RID in (select distinct Group_RID from GROUP_CARD_TYPE where CardType_RID in (@CardList)) and Param_Code = 'use1' and RST='A'";
    const string SEL_DAY = "select distinct [day] from dbo.RPT_report017 where RCT=@RCT order by [day]";
    const string DEL_DATA = "delete from RPT_Report017";
    const string CALC_TOTAL = "select sum(number) from dbo.RPT_report017 where gourporcard='1' and cardid = @Type and [day] = @Date and RCT=@RCT";
    const string SEL_CARD_GROUP = "select distinct CardType_RID from GROUP_CARD_TYPE where rst='A' and Group_RID=@Group_RID";
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Report017()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    #region 計算卡片數量基函數
    /// <summary>
    /// 取得卡片數量
    /// </summary>
    /// <param name="RID">卡種ID</param>
    /// <param name="Date">日期</param>
    /// <param name="CardType">卡種類型(新卡、掛補卡、毀補卡、換卡)</param>
    /// <returns>返回卡片數量，沒有則返回0</returns>
    //private int GetCardNum(int RID,string Date,int CardType)
    //{
    //    try
    //    {
    //        dirValues.Clear();
    //        dirValues.Add("Date", Date);
    //        dirValues.Add("Action", CardType);
    //        dirValues.Add("RID", RID);       
    //        object returnValue = dao.ExecuteScalar(SEL_CARD_NEW, dirValues);
    //        if (Convert.IsDBNull(returnValue) == true)
    //            return 0;
    //        else
    //        {
    //            return Convert.ToInt32(returnValue);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    private int GetCardNum(int RID, string Date, int CardType,string FactoryRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Date", Date);
            dirValues.Add("Action", CardType);
            dirValues.Add("RID", RID);
            StringBuilder strWhere = new StringBuilder();
            if (FactoryRID != "")
            {
                strWhere.Append(" AND Perso_Factory_RID=@Perso_Factory_RID ");
                dirValues.Add("Perso_Factory_RID", FactoryRID);
            }
            object returnValue = dao.ExecuteScalar(SEL_CARD_NEW + strWhere.ToString(), dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 根據群組取得卡片數量
    /// </summary>
    /// <param name="GroupID">群組數量</param>
    /// <param name="Date">日期</param>
    /// <param name="Type">卡類型 A新卡/B掛補卡/C毀補卡/D換卡</param>
    /// <returns></returns>
    private int GetCardNum(int GroupID, string Date, string Type, string Rct)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("GroupID", GroupID);
            dirValues.Add("Date", Date);
            dirValues.Add("Type", Type);
            dirValues.Add("RCT", Rct);
            object returnValue = dao.ExecuteScalar(SEL_GROUP_NEW, dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }    
    /// <summary>
    /// 得到群組合計卡片數量
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <param name="Type"></param>
    /// <returns></returns>
    private int GetCardNum(string Date, string Type, string timeMark)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Date", Date);
            dirValues.Add("Type", Type);
            dirValues.Add("RCT", timeMark);
            object returnValue = dao.ExecuteScalar(CALC_TOTAL, dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    #endregion
    #region 根據卡種取得卡片數量
    /// <summary>
    /// 取得系統卡數量
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Affinity"></param>
    /// <param name="Photo"></param>
    /// <param name="Date"></param>
    /// <param name="CardType"></param>
    /// <returns></returns>
    //private int GetSysCardNum(int RID, string Date, int CardType)
    //{
    //    int i = 0;
    //    try
    //    {

    //        DataSet ds = dao.GetList("select dbo.fun_EXPRESSIONS_NUMBER2 ('" + RID.ToString() + "','" + Date + "','','" + CardType + "')");

    //        if (ds.Tables[0].Rows.Count > 0)
    //        {
    //            i = int.Parse(ds.Tables[0].Rows[0][0].ToString());
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //    return i;
    //}
    private int GetSysCardNum(int RID, string Date, int CardType, string FactoryRID)
    {
        int i = 0;
        try
        {

            DataSet ds = dao.GetList("select dbo.fun_EXPRESSIONS_NUMBER2 ('" + RID.ToString() + "','" + Date + "','" + FactoryRID + "','" + CardType + "')");

            if (ds.Tables[0].Rows.Count > 0)
            {
                i = int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return i;
    }
    /// <summary>
    /// 計算系統卡數量
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    private int CalcNum(DataSet ds)
    {
        try
        {
            int num = 0;
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["Operate"].ToString() == "+")
                        num = num + Convert.ToInt32(dr["Number"]);
                    if (dr["Operate"].ToString() == "-")
                        num = num - Convert.ToInt32(dr["Number"]);


                }
                return num;
            }
            else
                return 0;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 取得製成卡數量
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    /// <returns></returns>
    //public int GetMadeCardNum(int RID,string Date)
    //{
    //    try
    //    {
    //        string[] TAP = GetTAPByRID(RID);
    //        if (TAP != null)
    //        {
    //            return GetSysCardNum(RID, Date, 1);
    //        }
    //        else
    //            return 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
 
    //}
    public int GetMadeCardNum(int RID, string Date, string FactoryRID)
    {
        try
        {
            string[] TAP = GetTAPByRID(RID);
            if (TAP != null)
            {
                return GetSysCardNum(RID, Date, 1, FactoryRID);
            }
            else
                return 0;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

    }
    /// <summary>
    /// 取得消耗卡數量
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    /// <returns></returns>
    //public int GetSpendCardNum(int RID,string Date)
    //{
    //    try
    //    {
    //        string[] TAP = GetTAPByRID(RID);
    //        if (TAP != null)
    //        {
    //            return GetSysCardNum(RID, Date, 2);
    //        }
    //        else
    //            return 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    public int GetSpendCardNum(int RID, string Date, string FactoryRID)
    {
        try
        {
            string[] TAP = GetTAPByRID(RID);
            if (TAP != null)
            {
                return GetSysCardNum(RID, Date, 2, FactoryRID);
            }
            else
                return 0;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 根據卡種RID取得卡種Type、Affinity、Photo
    /// </summary>
    /// <param name="RID"></param>
    /// <returns></returns>
    private string[] GetTAPByRID(int RID)
    {
        try
        {
            string[] TAP;
            dirValues.Clear();
            dirValues.Add("RID", RID);
            DataSet ds = dao.GetList(SEL_CARD_TAP, dirValues);
            if (ds.Tables[0].Rows.Count > 0)
            {
                TAP = new string[4];
                TAP[0] = ds.Tables[0].Rows[0]["TYPE"].ToString();
                TAP[1] = ds.Tables[0].Rows[0]["AFFINITY"].ToString();
                TAP[2] = ds.Tables[0].Rows[0]["PHOTO"].ToString();
                TAP[3] = ds.Tables[0].Rows[0]["Name"].ToString();
                return TAP;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 得到新卡數量
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    //public int GetNewCardNum(int RID, string Date)
    //{
    //    try
    //    {
    //        return GetCardNum(RID, Date, 1);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    public int GetNewCardNum(int RID, string Date,string FactoryRID)
    {
        try
        {
            return GetCardNum(RID, Date, 1, FactoryRID);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 得到掛補卡數量
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    //public int GetRegCarNum(int RID, string Date)
    //{
    //    try
    //    {
    //        return GetCardNum(RID, Date, 2);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    public int GetRegCarNum(int RID, string Date, string FactoryRID)
    {
        try
        {
            return GetCardNum(RID, Date, 2, FactoryRID);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 得到毀補卡數量
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    //public int GetLosCarNum(int RID, string Date)
    //{
    //    try
    //    {
    //        return GetCardNum(RID, Date, 3);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    public int GetLosCarNum(int RID, string Date, string FactoryRID)
    {
        try
        {
            return GetCardNum(RID, Date, 3, FactoryRID);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 得到換卡數量
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    //public int GetChangeCardNum(int RID, string Date)
    //{
    //    try
    //    {
    //        return GetCardNum(RID, Date, 5);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    public int GetChangeCardNum(int RID, string Date,string FactoryRID)
    {
        try
        {
            return GetCardNum(RID, Date, 5, FactoryRID);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 取得製損卡數量
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    /// <returns></returns>
    //public int GetBadCardNum(int RID,string Date)
    //{
    //    try
    //    {
    //        string[] TAP = GetTAPByRID(RID);
    //        if (TAP != null)
    //        {
    //            dirValues.Clear();
    //            dirValues.Add("Type", TAP[0]);
    //            dirValues.Add("AFFINITY", TAP[1]);
    //            dirValues.Add("Photo", TAP[2]);
    //            dirValues.Add("Date", Date);
    //            object returnValue = dao.ExecuteScalar(SEL_BAD_CARD, dirValues);
    //            if (Convert.IsDBNull(returnValue) == true)
    //                return 0;
    //            else
    //            {
    //                return Convert.ToInt32(returnValue);
    //            }
    //        }
    //        else
    //            return 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    public int GetBadCardNum(int RID, string Date, string FactoryRID)
    {
        try
        {
            string[] TAP = GetTAPByRID(RID);
            if (TAP != null)
            {
                dirValues.Clear();
                dirValues.Add("Type", TAP[0]);
                dirValues.Add("AFFINITY", TAP[1]);
                dirValues.Add("Photo", TAP[2]);
                dirValues.Add("Date", Date);
                StringBuilder strWhere = new StringBuilder();
                if (FactoryRID != "")
                {
                    strWhere.Append(" AND Perso_Factory_RID=@Perso_Factory_RID ");
                    dirValues.Add("Perso_Factory_RID", FactoryRID);
                }
                object returnValue = dao.ExecuteScalar(SEL_BAD_CARD + strWhere.ToString(), dirValues);
                if (Convert.IsDBNull(returnValue) == true)
                    return 0;
                else
                {
                    return Convert.ToInt32(returnValue);
                }
            }
            else
                return 0;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 取得卡種的數量
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns>卡種數量數組 新卡/掛補卡/毀補卡/換卡/製成卡/製損卡/消耗卡</returns>
    //public int[] CalcCardNum(int RID, string Date)
    //{
    //    try
    //    {
    //        int[] cardNum = new int[7];
    //        cardNum[0] = GetNewCardNum(RID, Date);
    //        cardNum[1] = GetRegCarNum(RID, Date);
    //        cardNum[2] = GetLosCarNum(RID, Date);
    //        cardNum[3] = GetChangeCardNum(RID, Date);
    //        cardNum[4] = GetMadeCardNum(RID, Date);
    //        cardNum[5] = GetBadCardNum(RID, Date);
    //        cardNum[6] = GetSpendCardNum(RID, Date);
    //        return cardNum;
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //}
    public int[] CalcCardNum(int RID, string Date, string FactoryRID)
    {
        try
        {
            int[] cardNum = new int[7];
            cardNum[0] = GetNewCardNum(RID, Date, FactoryRID);
            cardNum[1] = GetRegCarNum(RID, Date, FactoryRID);
            cardNum[2] = GetLosCarNum(RID, Date, FactoryRID);
            cardNum[3] = GetChangeCardNum(RID, Date, FactoryRID);
            cardNum[4] = GetMadeCardNum(RID, Date, FactoryRID);
            cardNum[5] = GetBadCardNum(RID, Date, FactoryRID);
            cardNum[6] = GetSpendCardNum(RID, Date, FactoryRID);
            return cardNum;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    #endregion
    #region 根據群組取得卡片數量
    /// <summary>
    /// 取得新卡數量
    /// </summary>
    /// <returns></returns>
    private int GetNewGNum(int GroupID, string Date, string timeMark)
    {
        return GetCardNum(GroupID, Date, "A", timeMark);

    }   
    /// <summary>
    /// 取得掛補卡數量
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetRegGNum(int GroupID, string Date,string timeMark)
    {
        return GetCardNum(GroupID, Date, "B", timeMark);
    }
    /// <summary>
    /// 取得毀補卡數量
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetLostGNum(int GroupID, string Date, string timeMark)
    {
        return GetCardNum(GroupID, Date, "C", timeMark);
    }
    /// <summary>
    /// 換卡數量
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetChangeGNum(int GroupID, string Date, string timeMark)
    {
        return GetCardNum(GroupID, Date, "D", timeMark);
    }
    /// <summary>
    /// 製成卡
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetMadeGNum(int GroupID, string Date,string timeMark)
    {
        return GetCardNum(GroupID, Date, "E", timeMark);
    }
    /// <summary>
    /// 製損卡
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetBadGNum(int GroupID, string Date, string timeMark)
    {
        return GetCardNum(GroupID, Date, "F", timeMark);
    }
    /// <summary>
    /// 消耗卡
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetSpendGNum(int GroupID, string Date, string timeMark)
    {
        return GetCardNum(GroupID, Date, "G", timeMark);
    }

    /// <summary>
    /// 計算卡種群組卡片數量
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int[] CalcGroupNum(int GroupID, string Date, string timeMark)
    {
        int[] GroupNum = new int[7];
        GroupNum[0] = GetNewGNum(GroupID, Date, timeMark);
        GroupNum[1] = GetRegGNum(GroupID, Date, timeMark);
        GroupNum[2] = GetLostGNum(GroupID, Date, timeMark);
        GroupNum[3] = GetChangeGNum(GroupID, Date, timeMark);
        GroupNum[4] = GetMadeGNum(GroupID, Date, timeMark);
        GroupNum[5] = GetBadGNum(GroupID, Date, timeMark);
        GroupNum[6] = GetSpendGNum(GroupID, Date, timeMark);
        return GroupNum;
    }   
    #endregion
    #region 計算合計卡片數量
    private int GetTolNewNum( string Date,string timeMark)
    {
        return GetCardNum(Date, "A",timeMark);
    }
    private int GetTolRegNum(string Date,string timeMark)
    {
        return GetCardNum( Date, "B",timeMark);
    }
    private int GetTolLostNum(string Date,string timeMark)
    {
        return GetCardNum( Date, "C",timeMark);
    }
    private int GetTolChangeNum( string Date,string timeMark)
    {
        return GetCardNum( Date, "D",timeMark);
    }
    private int GetTolMadeNum(string Date,string timeMark)
    {
        return GetCardNum( Date, "E",timeMark);
    }
    private int GetTolBadNum( string Date,string timeMark)
    {
        return GetCardNum( Date, "F",timeMark);
    }
    private int GetTolSpendNum( string Date,string timeMark)
    {
        return GetCardNum(Date, "G",timeMark);
    }
    private int[] CalcTotalNum( string Date ,string timeMark)
    {
        int[] TotalNum = new int[7];
        TotalNum[0] = GetTolNewNum(Date,timeMark);
        TotalNum[1] = GetTolRegNum(Date,timeMark);
        TotalNum[2] = GetTolLostNum(Date,timeMark);
        TotalNum[3] = GetTolChangeNum(Date,timeMark);
        TotalNum[4] = GetTolMadeNum(Date,timeMark);
        TotalNum[5] = GetTolBadNum(Date,timeMark);
        TotalNum[6] = GetTolSpendNum(Date,timeMark);
        return TotalNum;
    }
    #endregion
   //20090702CR 
    //public void ExecData(string Year,string Month, string CardList,string timeMark) 
    //{
    //    try
    //    {
    //        dao.OpenConnection();
    //        int MonthEnd = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month));
    //        int i;
    //        string Date;
    //        ClearData();
    //        for (i = 1; i <= MonthEnd; i++)
    //        {
    //            Date = Year + "/" + Month + "/" + i.ToString();
    //            string[] Cards = CardList.Split(',');
    //            foreach (string sCard in Cards)
    //            {
    //                InsertCardData(Date, Convert.ToInt32(sCard), timeMark);//插入卡資料
    //            }
    //        }
    //        InsertGroupCardData(CardList, timeMark);

    //        dao.Commit();
    //    }
    //    catch
    //    {
    //        dao.Rollback();
    //    }
    //    finally
    //    {
    //        dao.CloseConnection();
    //    }
    //}
    //200900702CR-卡種查詢 
    public void ExecDataCardList(string FactoryRID, string BeginDate, string EndDate, string CardList, string timeMark)
    {
        try
        {
            dao.OpenConnection();           
            int MonthEnd=int.Parse((DateTime.Parse(EndDate)-DateTime.Parse(BeginDate)).Days.ToString());
            int i;
            string Date;
            ClearData();
            for (i = 0; i <= MonthEnd; i++)
            {
                Date =DateTime.Parse(BeginDate).AddDays(i).ToString("yyyy/MM/dd") ;
                string[] Cards = CardList.Split(',');
                foreach (string sCard in Cards)
                {
                    InsertCardData(Date, Convert.ToInt32(sCard), timeMark, FactoryRID);//插入卡資料
                }
            }
            InsertGroupCardData(CardList, timeMark);

            dao.Commit();
        }
        catch
        {
            dao.Rollback();
        }
        finally
        {
            dao.CloseConnection();
        }
    }
    //200900702CR-群組查詢 
    public void ExecDataGroup(string FactoryRID, string BeginDate, string EndDate, string GroupRID, string timeMark)
    {
        try
        {
            dao.OpenConnection();
            int MonthEnd = int.Parse((DateTime.Parse(EndDate) - DateTime.Parse(BeginDate)).Days.ToString());
            int i;
            string Date;
            ClearData();
            string CardList=GetCardByGroupRID(GroupRID);

            for (i = 0; i <= MonthEnd; i++)
            {
                Date = DateTime.Parse(BeginDate).AddDays(i).ToString("yyyy/MM/dd");
                string[] Cards = CardList.Split(',');
                foreach (string sCard in Cards)
                {
                    InsertCardData(Date, Convert.ToInt32(sCard), timeMark, FactoryRID);//插入卡資料
                }               
            }
            InsertGroupCardData(CardList, timeMark);

            dao.Commit();
        }
        catch
        {
            dao.Rollback();
        }
        finally
        {
            dao.CloseConnection();
        }
    }
    /// <summary>
    /// 插入卡片數量資料
    /// </summary>
    /// <param name="GroupOrCard"></param>
    /// <param name="Name"></param>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <param name="cardNum"></param>
    private void InsertData(string GroupOrCard, string Name, int RID, string Date, int[] cardNum, string time)
    {
        try
        {
            int i;
            string CardType;
            string CardID;
            for (i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0:
                        CardType = "A新卡";
                        CardID = "A";
                        break;
                    case 1:
                        CardType = "B掛補卡";
                        CardID = "B";
                        break;
                    case 2:
                        CardType = "C毀補卡";
                        CardID = "C";
                        break;
                    case 3:
                        CardType = "D換卡";
                        CardID = "D";
                        break;
                    case 4:
                        CardType = "E製成卡";
                        CardID = "E";
                        break;
                    case 5:
                        CardType = "F製損卡";
                        CardID = "F";
                        break;
                    case 6:
                        CardType = "G消耗卡";
                        CardID = "G";
                        break;
                    default:
                        CardType = "H未知卡種";
                        CardID = "H";
                        break;

                }
                Date = Convert.ToDateTime(Date).ToString("yyyy/MM/dd");
                dirValues.Clear();
                dirValues.Add("GroupOrCard", GroupOrCard);//卡種
                dirValues.Add("Name", Name);
                dirValues.Add("RID", RID);
                dirValues.Add("CardName", CardType);
                dirValues.Add("CardID", CardID);
                dirValues.Add("Day", Date);
                dirValues.Add("Number", cardNum[i]);
                dirValues.Add("RCT",time);
                int returnValue = dao.ExecuteNonQuery(INSERT_DATA, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 插入卡種數量資料
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    //public void InsertCardData(string Date, int RID, string time)
    //{
    //    int[] cardNum;
    //    cardNum = CalcCardNum(RID, Date);
    //    string CardName;
    //    string[] TAP = GetTAPByRID(RID);
    //    if (TAP != null)
    //    {
    //        CardName = TAP[3];
    //    }
    //    else
    //        CardName = "";
    //    InsertData("3", CardName, RID, Date, cardNum,time);
    //}
    public void InsertCardData(string Date, int RID, string time,string FactoryRID)
    {
        int[] cardNum;
        cardNum = CalcCardNum(RID, Date,FactoryRID);
        string CardName;
        string[] TAP = GetTAPByRID(RID);
        if (TAP != null)
        {
            CardName = TAP[3];
        }
        else
            CardName = "";
        InsertData("3", CardName, RID, Date, cardNum, time);
    }
    /// <summary>
    /// 取得製卡群組
    /// </summary>
    /// <param name="CardList">卡種RID</param>
    /// <returns>DataSet</returns>
    private DataSet GetMadeGroup(string CardList)
    {
        try
        {
            //dirValues.Clear();
            //dirValues.Add("CardList",CardList);
            string sSQL = SEL_MADE_GROUP.Replace("@CardList", CardList);

            return dao.GetList(sSQL);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
 
    }
    /// <summary>
    /// 插入卡種群組資料
    /// </summary>
    /// <param name="CardList"></param>
    public void InsertGroupCardData(string CardList, string time)
    {
        DataSet ds = GetMadeGroup(CardList);
        DataSet dsDay = GetDay(time);
        int GroupID;
        string GroupName;
        string Date;
        int[] GroupNum;
        int[] TotNum;
        if (ds.Tables.Count > 0)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                GroupID = Convert.ToInt32(dr["RID"]);
                GroupName = dr["Group_Name"].ToString();
                foreach (DataRow drDay in dsDay.Tables[0].Rows)
                {
                    Date = drDay["day"].ToString();
                    GroupNum = CalcGroupNum(GroupID, Date, time);
                    InsertData("1", GroupName, GroupID, Date, GroupNum, time);
                }
            }

            foreach (DataRow drDay in dsDay.Tables[0].Rows)
            {
                Date = drDay["day"].ToString();
                TotNum = CalcTotalNum(Date, time);
                InsertData("2", "合計", -1, Date, TotNum, time);
            }

        }
    }   
    /// <summary>
    /// 累加卡片數量
    /// </summary>
    /// <param name="TotNum"></param>
    /// <param name="GroupNum"></param>
    /// <returns></returns>
    private int[] CalcTotal(int[] TotNum,int[] GroupNum)
    {
        int i = 0;
        foreach (int num in TotNum)
        {
            TotNum[i] = TotNum[i] + num;
            i++;
        }
        return TotNum;

    }
    /// <summary>
    /// 清空報表資料
    /// </summary>
    /// <returns></returns>
    private int ClearData()
    {
        try
        {
            return dao.ExecuteNonQuery(DEL_DATA+" where RCT<'"+DateTime.Now.ToString("yyyyMMdd000000")+"'");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 取得報表日期
    /// </summary>
    /// <returns></returns>
    private DataSet GetDay(string time)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("RCT", time);
            return dao.GetList(SEL_DAY,dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// 根據群組RID取得卡種
    /// </summary>
    /// <param name="RID"></param>
    /// <returns></returns>
    private string GetCardByGroupRID(string GroupRID)
    {
        try
        {
            string Cards="";
            dirValues.Clear();
            dirValues.Add("Group_RID", GroupRID);
            DataSet dsCard= dao.GetList(SEL_CARD_GROUP, dirValues);
            if (dsCard.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsCard.Tables[0].Rows.Count; i++)
                {
                    if (StringUtil.IsEmpty(Cards))
                    {
                        Cards = dsCard.Tables[0].Rows[i][0].ToString() ;                       
                    }
                    else
                    {
                        Cards = Cards + "," + dsCard.Tables[0].Rows[i][0].ToString();                        
                    }
                }

            }
            return Cards;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
}
