//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-09-02
//*  修改日期：2008-09-02 9:00
//*  修改記錄：
//*            □2008-09-02
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
/// CardType008BL 的摘要描述
/// </summary>
public class CardType008BL : BaseLogic
{
    #region SQL語句
    public const string SEL_WAFER_BY_CT = "select distinct w.* from wafer_info w join wafer_cardtype wc on w.rid=wc.wafer_rid where w.rst='A'";
    public const string SEL_WAFER_BY_FT = "select distinct w.* from wafer_info w join wafer_factory wf on wf.Wafer_RID=w.rid where w.rst='A'";
    public const string SEL_WAFER_INFO = "select * from wafer_info where rst='A'";
    public const string SEL_WAFER = "select distinct w.* from WAFER_INFO w join wafer_cardtype wc on w.rid=wc.wafer_rid join CARD_TYPE ct on wc.cardtype_rid=ct.rid join wafer_factory wf on wf.Wafer_RID=w.rid join factory f on f.RID=wf.Factory_RID and f.rst='A' where w.rst='A'";
    public const string SEL_FACTORY_BY_RID = "select f.rid,f.factory_shortname_cn,wf.Wafer_RID from wafer_info w join wafer_factory wf on wf.Wafer_RID=w.rid join factory f on f.RID=wf.Factory_RID and f.rst='A' where w.rst='A'";
    public const string SEL_CARDTYPE_BY_RID = "SELECT A.wafer_rid,B.RID,B.Display_Name as NAME FROM DBO.WAFER_CARDTYPE A INNER JOIN CARD_TYPE B ON A.CARDTYPE_RID=B.RID  WHERE a.rst='A'";
    public const string CHK_Wafer_BY_RID = "proc_CHK_DEL_Wafer";   
#endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    BaseInfo003BL fmManager = new BaseInfo003BL();

    public CardType008BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 空白卡廠下拉框綁定
    /// </summary>
    public void dropFactoryBind(DropDownList dropFactory)
    {
        dropFactory.DataTextField = "FACTORY_ShortNAME_CN";
        dropFactory.DataValueField = "RID";
        dropFactory.DataSource = fmManager.GetBlankFactory();
        dropFactory.DataBind();
    }

    /// <summary>
    /// 查詢晶片記錄列表
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
        string strSortField = (sortField == "null" ? "Wafer_Name" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = null;

        if (searchInput["txtWafer_Name"].ToString().Trim() != "" || searchInput["txtBG"].ToString().Trim() != "" || searchInput["txtEnd"].ToString().Trim() != "" || searchInput["txtMark"].ToString().Trim() != "" || searchInput["txtWafer_Factory"].ToString().Trim() != "")
        {
            if (searchInput["drpFactory_shortname_cn"].ToString().Trim() != "" || searchInput["UrctrlCardTypeSelect"].ToString().Trim() != "")
            {
                stbCommand = new StringBuilder(SEL_WAFER);
            }
            else
            {
                stbCommand = new StringBuilder(SEL_WAFER_INFO);
            }
        }
        else
        {
            if (searchInput["drpFactory_shortname_cn"].ToString().Trim() != "" && searchInput["UrctrlCardTypeSelect"].ToString().Trim() == "")
            {
                stbCommand = new StringBuilder(SEL_WAFER_BY_FT);
            }
            else if (searchInput["UrctrlCardTypeSelect"].ToString().Trim() != "" && searchInput["drpFactory_shortname_cn"].ToString().Trim() == "")
            {
                stbCommand = new StringBuilder(SEL_WAFER_BY_CT);
            }
            else if (searchInput["UrctrlCardTypeSelect"].ToString().Trim() != "" && searchInput["drpFactory_shortname_cn"].ToString().Trim() != "")
            {
                stbCommand = new StringBuilder(SEL_WAFER);
            }
            else
            {
                stbCommand = new StringBuilder(SEL_WAFER_INFO);
            }
        }

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (searchInput["txtWafer_Name"].ToString().Trim() != "")
            {
                stbWhere.Append(" and Wafer_Name like @Wafer_Name");
                dirValues.Add("Wafer_Name", "%" + searchInput["txtWafer_Name"].ToString().Trim() + "%");
            }

            //if (searchInput["txtWafer_Capacity"].ToString().Trim() != "")
            //{
            //    stbWhere.Append(" and Wafer_Capacity like @Wafer_Capacity");
            //    dirValues.Add("Wafer_Capacity", "%" + searchInput["txtWafer_Capacity"].ToString().Trim() + "%");
            //}
            if (searchInput["txtBG"].ToString().Trim() != "")
            {
                stbWhere.Append(" AND Wafer_Capacity >= @BG ");
                dirValues.Add("BG", searchInput["txtBG"].ToString().Trim());

            }
            if (searchInput["txtEnd"].ToString().Trim() != "")
            {
                stbWhere.Append(" AND Wafer_Capacity <= @End ");
                dirValues.Add("End", searchInput["txtEnd"].ToString().Trim());
            }



            if (searchInput["txtMark"].ToString().Trim() != "")
            {
                stbWhere.Append(" and Mark like @Mark");
                dirValues.Add("Mark", "%" + searchInput["txtMark"].ToString().Trim() + "%");
            }
            if (searchInput["txtWafer_Factory"].ToString().Trim() != "")
            {
                stbWhere.Append(" and Wafer_Factory like @Wafer_Factory");
                dirValues.Add("Wafer_Factory", "%" + searchInput["txtWafer_Factory"].ToString().Trim() + "%");
            }
            if (searchInput["drpFactory_shortname_cn"].ToString().Trim() != "")
            {
                if (searchInput["drpFactory_shortname_cn"].ToString().Trim() != "")
                {
                    stbWhere.Append(" and Factory_rid =@Factory_rid");
                    dirValues.Add("Factory_rid", searchInput["drpFactory_shortname_cn"].ToString().Trim());
                }
            }
            if (searchInput["UrctrlCardTypeSelect"].ToString().Trim() != "")
            {
                stbWhere.Append(" and cardtype_rid =@cardtype_rid");
                dirValues.Add("cardtype_rid", searchInput["UrctrlCardTypeSelect"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dstWafer = null;
        try
        {
            dstWafer = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstWafer;
    }

    /// <summary>
    /// 判斷該晶片名稱是否存在
    /// </summary>
    /// <param name="strGroupName">晶片名稱</param>    
    /// <returns>是/否</returns>
    public bool IsHave(string strWaferName)
    {
        DataSet dstKzqz = null;
        try
        {
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();
                        
            dirValues.Clear();
            stbWhere.Append(" and wafer_Name =@wafer_Name");
            dirValues.Add("wafer_Name", strWaferName);

            dstKzqz = dao.GetList("select * from wafer_info where 1=1"+stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        if (dstKzqz != null && dstKzqz.Tables[0].Rows.Count > 0)
            return true;
        return false;
    }

    /// <summary>
    /// 獲取WAFER_INFO資料模型
    /// </summary>
    /// <param name="strRID">ID</param>
    /// <returns>WAFER_INFO模型</returns>
    public WAFER_INFO GetWaferInfo(string strRID)
    {
        WAFER_INFO wiModel = null;
        try
        {
            wiModel = dao.GetModel<WAFER_INFO, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return wiModel;
    }

    /// <summary>
    /// 查詢晶片編號對應晶片記錄信息
    /// </summary>
    /// <param name="strRID">晶片</param>
    /// <returns>DataSet</returns>
    public DataSet LoadWaferByRID(string strRID)
    {
        DataSet dstWafer = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_WAFER_INFO);

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();

            stbWhere.Append(" and RID =@RID");
            dirValues.Add("RID", strRID);

            dstWafer = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstWafer;
    }

    /// <summary>
    /// 查詢晶片編號對應卡廠信息
    /// </summary>
    /// <param name="strRID">晶片</param>
    /// <returns>DataSet</returns>
    public DataSet LoadFactoryByWRID(string strRID)
    {
        DataSet dstFactory = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_FACTORY_BY_RID);

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();

            stbWhere.Append(" and Wafer_RID =@Wafer_RID");
            dirValues.Add("Wafer_RID", strRID);
                        
            dstFactory = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstFactory;
    }

    /// <summary>
    /// 查詢晶片編號對應卡種信息
    /// </summary>
    /// <param name="strRID">晶片</param>
    /// <returns>DataSet</returns>
    public DataSet LoadCardTypeByWRID(string strRID)
    {
        DataSet dstCardType = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_CARDTYPE_BY_RID);

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();

            stbWhere.Append(" and wafer_rid =@Wafer_RID");
            dirValues.Add("wafer_rid", strRID);

            dstCardType = dao.GetList(stbCommand.ToString() + stbWhere.ToString() + " order by Display_Name", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCardType;
    }

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="wiModel">晶片model</param>   
    /// <param name="wfModel">晶片對應空白卡廠model</param>  
    /// <param name="wcModel">晶片對應卡種model</param>  
    public void Add(WAFER_INFO wiModel,WAFER_FACTORY wfModel,ArrayList FactoryValues,WAFER_CARDTYPE wcModel,ArrayList CtValues)
    {
        try
        {
            //開始事務
            dao.OpenConnection();

            //添加晶片基本信息            
            int wid = int.Parse(dao.AddAndGetID<WAFER_INFO>(wiModel, "RID").ToString());
                                     
            if (FactoryValues.Count > 0)
            {
                wfModel.Wafer_RID = wid;
                for (int i = 0; i < FactoryValues.Count;i++ )
                {
                    wfModel.Factory_RID = int.Parse(FactoryValues[i].ToString());
                    //添加晶片對應空白卡廠信息
                    dao.Add<WAFER_FACTORY>(wfModel, "RID");
                }
            }

            if (CtValues.Count > 0)
            {
                wcModel.Wafer_RID = wid;
                for (int i = 0; i < CtValues.Count; i++)
                {
                    wcModel.CardType_RID = int.Parse(CtValues[i].ToString());
                    //添加晶片對應卡種信息
                    dao.Add<WAFER_CARDTYPE>(wcModel, "RID");
                }
            }

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (Exception ex)
        {
            //回滾
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

    /// <summary>
    /// 修改
    /// </summary>    
    /// <param name="wiModel">修改後晶片model</param>  
    public void Update(WAFER_INFO wiModel, WAFER_FACTORY wfModel, ArrayList FactoryValues, WAFER_CARDTYPE wcModel, ArrayList CtValues)
    {
        try
        {
            dao.OpenConnection();
                        
            //保存記錄
            dao.Update<WAFER_INFO>(wiModel, "RID");

            dirValues.Add("wafer_rid", wiModel.RID);

            dao.ExecuteNonQuery("delete from wafer_factory where wafer_rid=@wafer_rid",dirValues);

            dirValues.Clear();

            dirValues.Add("wafer_rid", wiModel.RID);

            dao.ExecuteNonQuery("delete from wafer_cardtype where wafer_rid=@wafer_rid",dirValues);

            if (FactoryValues.Count > 0)
            {
                wfModel.Wafer_RID = wiModel.RID;
                for (int i = 0; i < FactoryValues.Count; i++)
                {
                    wfModel.Factory_RID = int.Parse(FactoryValues[i].ToString());
                    //添加晶片對應空白卡廠信息
                    dao.Add<WAFER_FACTORY>(wfModel, "RID");
                }
            }

            if (CtValues.Count > 0)
            {
                wcModel.Wafer_RID = wiModel.RID;
                for (int i = 0; i < CtValues.Count; i++)
                {
                    wcModel.CardType_RID = int.Parse(CtValues[i].ToString());
                    //添加晶片對應卡種信息
                    dao.Add<WAFER_CARDTYPE>(wcModel, "RID");
                }
            }

            //操作日誌
            SetOprLog();

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
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="strRID">晶片RID</param>    
    public void Delete(string strRID)
    {
        //資料實體

        try
        {
            dao.OpenConnection();
            ChkDelWafer(strRID);

            ////進行邏輯物理處理

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("WAFER_INFO", dirValues);

            dao.ExecuteNonQuery("delete WAFER_FACTORY where wafer_rid='" + strRID + "'");
            dao.ExecuteNonQuery("delete WAFER_CARDTYPE where wafer_rid='" + strRID + "'");

            //操作日誌
            SetOprLog("4");

            dao.Commit();
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            dao.Rollback();
            //異常處理
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 檢查晶片是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelWafer(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("Wafer_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_Wafer_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "晶片"));

    }
}
