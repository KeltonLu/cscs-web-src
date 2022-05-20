//******************************************************************
//*  作    者：bingyipan
//*  功能說明：廠商卡片庫存查核表 
//*  創建日期：2008-11-28
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
/// Report016BL 的摘要描述
/// </summary>
public class Report016BL : BaseLogic
{
    #region SQL語句    
    public const string SEL_IMPORT = "proc_report016";

    public const string SEL_CARD = "select distinct ct.rid,ct.name,ct.type+'-'+ct.affinity+'-'+ct.photo cardtype from SUBTOTAL_IMPORT si"
	+ " left join card_type ct on ct.rst='A' and ct.type=si.type "
	+ " and ct.affinity=si.affinity and ct.photo=si.photo "
	+ " where si.rst='A' and si.date_time>=@begintime and si.date_time<=@endtime";

    // add by Ian Huang start
    public const string SEL_FACTORY = "SELECT F.RID,F.Factory_ShortName_CN "
                                        + "FROM FACTORY AS F "
                                        + "WHERE F.RST = 'A' AND F.Is_Perso = 'Y'";
    // add by Ian Huang end
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Report016BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 根據條件得到所有卡種對應小記檔number和的數據集
    /// </summary>
    /// <param name="searchInput"></param>
    /// <returns></returns>
    public DataSet getImport(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet ds = null;

            dirValues.Clear();
            dirValues.Add("begintime", Convert.ToDateTime(searchInput["begin_time"].ToString()));
            dirValues.Add("endtime", Convert.ToDateTime(searchInput["end_time"].ToString()));
            dirValues.Add("action", searchInput["Action"].ToString());

            ds = dao.GetList(SEL_IMPORT, dirValues, true);

            return ds;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    public DataSet getCard(Dictionary<string, object> searchInput)
    {
        try
        {
            DataSet ds = new DataSet();

            StringBuilder strWhere = new StringBuilder();

            dirValues.Clear();
            dirValues.Add("begintime", Convert.ToDateTime(searchInput["begin_time"].ToString()));
            dirValues.Add("endtime", Convert.ToDateTime(searchInput["end_time"].ToString()));
            if (searchInput["Action"].ToString().Length == 1)
            {
                dirValues.Add("action", searchInput["Action"].ToString());
                strWhere.Append(" and si.action=@action");
                ds = dao.GetList(SEL_CARD + strWhere.ToString(), dirValues);
            }
            else
            {
                ds = dao.GetList(SEL_CARD, dirValues);
            }

            return ds;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    // add by Ian Huang start
    /// <summary>
    /// 獲得Perso廠商
    /// </summary>
    /// <returns>DataSet[Perso廠商]</returns>
    public DataSet GetFactoryList()
    {
        DataSet dstFactory = null;
        try
        {
            this.dirValues.Clear();
            dstFactory = dao.GetList(SEL_FACTORY, dirValues);

            return dstFactory;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }
    // add by Ian Huang end
}
