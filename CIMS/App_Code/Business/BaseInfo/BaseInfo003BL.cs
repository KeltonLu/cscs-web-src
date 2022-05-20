//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料維護功能
//*  創建日期：2008-08-27
//*  修改日期：2008-08-27 14:00
//*  修改記錄：
//*            □2008-08-27
//*              1.創建 潘秉奕
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

/// <summary>
/// BaseInfo003BL 的摘要描述
/// </summary>
public class BaseInfo003BL : BaseLogic
{
    #region SQL語句
    public const string SEL_FACTORY = "SELECT * FROM Factory WHERE RST='A'";
    public const string SEL_BLANKFACTORY = "SELECT * FROM Factory WHERE RST='A' and Is_Blank='Y'";
    public const string SEL_FACTORY_BY_RID = "SELECT * FROM Factory WHERE RID = @rid";
    public const string CHK_FACTORY_BY_RID = "proc_CHK_DEL_Factory";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo003BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢廠商主記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[廠商]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Factory_ID" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_FACTORY);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (searchInput["txtFactory_ShortName_CN"].ToString().Trim() != "")
            {
                stbWhere.Append(" and Factory_ShortName_CN like @Factory_ShortName_CN");
                dirValues.Add("Factory_ShortName_CN", "%" + searchInput["txtFactory_ShortName_CN"].ToString().Trim() + "%");
            }
            if (Convert.ToBoolean(searchInput["chkIs_Blank"]))
            {
                stbWhere.Append(" and Is_Blank= @Is_Blank");
                dirValues.Add("Is_Blank", "Y" );
            }
            if (Convert.ToBoolean(searchInput["chkIs_Perso"]))
            {
                stbWhere.Append(" and Is_Perso=@Is_Perso");
                dirValues.Add("Is_Perso", "Y");
            }            
        }

        //執行SQL語句
        DataSet dstcard_Factory = null;
        try
        {
            dstcard_Factory = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstcard_Factory;
    }

    /// <summary>
    /// 廠商增加
    /// </summary>
    /// <param name="cbModel">廠商</param>   
    public void Add(FACTORY cbModel)
    { 
        try
        {
            cbModel.Factory_ID = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("FactoryID");
            dao.Add<FACTORY>(cbModel, "RID");

            //操作日誌
            SetOprLog();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
    }

    /// <summary>
    /// 廠商修改
    /// </summary>    
    /// <param name="cbModel">修改後廠商</param>  
    public void Update(FACTORY cboModel)
    {        
        try
        {
            //保存預算記錄
            dao.Update<FACTORY>(cboModel, "RID");

            //操作日誌
            SetOprLog();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
    }

    /// <summary>
    /// 廠商刪除
    /// </summary>
    /// <param name="strRID">廠商RID</param>    
    public void Delete(string strRID)
    {
        //資料實體
        FACTORY cbModel = new FACTORY();        

        try
        {
            ChkDelFactory(strRID);

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("FACTORY", dirValues);


            //cbModel = GetFactory(strRID);

                
            ////進行邏輯刪除處理
            //cbModel.RST = "D";

            ////保存記錄
            //dao.Update<FACTORY>(cbModel, "RID");

            //操作日誌
            SetOprLog("4");
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            //異常處理
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
    }

    /// <summary>
    /// 查詢廠商類別信息
    /// </summary>
    /// <param name="strRID">廠商ID</param>
    /// <returns>DataSet[廠商]</returns>
    public DataSet LoadFactoryInfoByRID(string strRID)
    {
        DataSet dstCslb = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("rid", int.Parse(strRID));

            dstCslb = dao.GetList(SEL_FACTORY_BY_RID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstCslb;
    }

    /// <summary>
    /// 獲取FACTORY資料模型
    /// </summary>
    /// <param name="strRID">廠商ID</param>
    /// <returns>FACTORY模型</returns>
    public FACTORY GetFactory(string strRID)
    {
        FACTORY cbModel = null;
        try
        {
            cbModel = dao.GetModel<FACTORY, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return cbModel;
    }

    /// <summary>
    /// 獲取廠商
    /// </summary>
    /// <returns></returns>
    public DataSet GetFactory()
    {
        DataSet dstFactory = null;

        try
        {
            dstFactory = dao.GetList(SEL_FACTORY);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstFactory;
    }

    /// <summary>
    /// 獲取所有空白卡廠
    /// </summary>
    /// <returns></returns>
    public DataSet GetBlankFactory()
    { 
        DataSet dstBlankFactory = null;

        try
        {
            dstBlankFactory = dao.GetList(SEL_BLANKFACTORY);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstBlankFactory;
    }

    /// <summary>
    /// 檢查廠商是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelFactory(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("Factory_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_FACTORY_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "廠商"));

    }
}
