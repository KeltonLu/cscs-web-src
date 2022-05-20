//******************************************************************
//*  作    者：lantaosu
//*  功能說明：換卡預測月報表 
//*  創建日期：2008-11-27
//*  修改日期：2008-11-27 18:00
//*  修改記錄：
//*            □2008-11-27
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
/// Report025BL 的摘要描述
/// </summary>
public class Report025BL
{
    #region SQL語句
    public const string SEL_LIST = "select  strType,Name, Number, Change_Date " 	 		
                                    +"from ( "
                                    + "select  1 strType,E.Name,  (SUM(P1.Number)*(1+CONVERT(decimal,E.Wear_Rate)/100)) AS Number, P1.Change_Date "
                                    +"from FORE_CHANGE_CARD_DETAIL P1 "
                                    +"left join CARD_TYPE C1 on C1.RST = 'A'  and C1.TYPE = P1.TYPE and C1.AFFINITY = P1.AFFINITY and C1.PHOTO=P1.PHOTO "
                                    +"inner join ENVELOPE_INFO E on E.RST = 'A'  and E.RID=C1.Envelope_RID "
                                    +"where P1.RST = 'A'  and P1.Number>0 and (P1.Change_Date between @DateFrom and @DateTo) "
                                    +"group by E.Name, P1.Change_Date, E.Wear_Rate "
                                    +"union "
                                    + "select  2 strType,CE.Name,  (SUM(P2.Number)*(1+CONVERT(decimal,CE.Wear_Rate)/100)) AS Number, P2.Change_Date "
                                    +"from FORE_CHANGE_CARD_DETAIL P2 "
                                    +"left join CARD_TYPE C2 on C2.RST = 'A'  and C2.TYPE = P2.TYPE and C2.AFFINITY = P2.AFFINITY and C2.PHOTO=P2.PHOTO "
                                    +"inner join CARD_EXPONENT CE on CE.RST = 'A'  and CE.RID=C2.Exponent_RID "
                                    +"where P2.RST = 'A'  and P2.Number>0 and (P2.Change_Date between @DateFrom and @DateTo) "
                                    +"group by CE.Name, P2.Change_Date, CE.Wear_Rate "
                                    +"union "
                                    + "select  3 strType,D.Name,  (SUM(P3.Number)*(1+CONVERT(decimal,D.Wear_Rate)/100)) AS Number, P3.Change_Date "
                                    +"from FORE_CHANGE_CARD_DETAIL P3 "
                                    +"left join CARD_TYPE C3 on C3.RST = 'A'  and C3.TYPE = P3.TYPE and C3.AFFINITY = P3.AFFINITY and C3.PHOTO=P3.PHOTO "
                                    +"inner  join DM_CARDTYPE DC on DC.RST = 'A'  and DC.CardType_RID=C3.RID "
                                    +"inner join DMTYPE_INFO D on D.RST='A' and D.RID=DC.DM_RID "
                                    +"where P3.RST = 'A'  and P3.Number>0 and (P3.Change_Date between @DateFrom and @DateTo) "
                                    +"group by D.Name, P3.Change_Date, D.Wear_Rate "
                                    +") as table1 ";

    public const string SEL_CHANGECARD = "select min(change_date) mindate,max(change_date) maxdate from FORE_CHANGE_CARD";

    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Report025BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public bool IsChangeCard(string strBeg,string strEnd)
    {
        bool IsBool = false;
        try
        {
            DataTable dtbl = dao.GetList(SEL_CHANGECARD).Tables[0];
            if (!StringUtil.IsEmpty(dtbl.Rows[0][0].ToString()))
            {
                string strBeg1 = dtbl.Rows[0][0].ToString();
                string strEnd1 = dtbl.Rows[0][1].ToString();

                if (int.Parse(strBeg) >= int.Parse(strBeg1) && int.Parse(strEnd) <= int.Parse(strEnd1))
                    IsBool = true;
            }
        }
        catch
        {

        }
        return IsBool;
    }

    /// <summary>
    /// 查詢報表數據
    /// </summary>
    /// <param name="DateFrom"></param>
    /// <param name="DateTo"></param>
    /// <returns></returns>
    public DataSet GetList(string DateFrom, string DateTo)
    {
        DataSet dstList = null;

        dirValues.Clear();
        dirValues.Add("DateFrom", DateFrom);
        dirValues.Add("DateTo", DateTo);

        try
        {
            dstList = dao.GetList(SEL_LIST, dirValues);
            return dstList;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 添加報表數據到數據庫中
    /// </summary>
    /// <param name="dt"></param>
    public void AddReport(DataTable dt,string time)
    {
        RPT_Report025 RPT = new RPT_Report025();
        try
        {
            int t = 0;
            int i = 0;
            
            //事務開始
            dao.OpenConnection();
            dao.ExecuteNonQuery("Delete From RPT_Report025 where TimeMark<'"+DateTime.Now.ToString("yyyyMMdd000000")+"'");

            foreach (DataRow dr in dt.Rows)
            {
                RPT.strType = dr["strType"].ToString();
                RPT.Name = dr["Name"].ToString();
                RPT.Number = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dr["Number"].ToString())));
                RPT.Change_Date = dr["Change_Date"].ToString();
                RPT.TimeMark = time;

                dao.Add<RPT_Report025>(RPT);
            }

            //事務提交
            dao.Commit();

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
