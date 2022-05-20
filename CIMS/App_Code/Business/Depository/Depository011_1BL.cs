//******************************************************************
//*  作    者：lantaosu
//*  功能說明：物料庫存專業作業管理邏輯
//*  創建日期：2008-09-09
//*  修改日期：2008-09-12 12:00
//*  修改記錄：
//*            □2008-09-09
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
/// Depository011_1BL 的摘要描述
/// </summary>
public class Depository011_1BL:BaseLogic
{
    #region SQL語句
    public const string SEL_MATERIEL_STOCKS_MOVE_BY_MOVE_DATE = "SELECT DISTINCT Move_Date,Move_ID "
                                + "FROM MATERIEL_STOCKS_MOVE "
                                + "WHERE RST='A'";   

   public const string SEL_MATERIAL_MOVE  = "SELECT MSM.RID,Move_Date,Move_ID,Move_Number,FF.Factory_ShortName_CN AS From_Factory_Name,TF.Factory_ShortName_CN AS To_Factory_Name,"
                               +" CE.Name as AName,CE.Serial_Number as ANumber,EI.Name as BName,EI.Serial_Number as BNumber,DM.Name as CName,DM.Serial_Number as CNumber  "
                               +" FROM MATERIEL_STOCKS_MOVE MSM  "
                               +" INNER JOIN FACTORY FF ON FF.RST = 'A' AND FF.Is_Perso = 'Y' AND MSM.From_Factory_RID = FF.RID"
                               +" INNER JOIN FACTORY TF ON TF.RST = 'A' AND TF.Is_Perso = 'Y' AND MSM.To_Factory_RID = TF.RID"
                               +" LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MSM.Serial_Number = CE.Serial_Number "
                               +" LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MSM.Serial_Number = EI.Serial_Number "
                               +" LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MSM.Serial_Number = DM.Serial_Number where MSM.rst = 'A'";
    
    public const string SEL_MATERIEL_STOCKS_MOVE_BY_MOVE_ID = "SELECT MSM.RID,Move_Date,Move_Number,From_Factory_RID,To_Factory_RID,FF.Factory_ShortName_CN AS From_Factory_Name,TF.Factory_ShortName_CN AS To_Factory_Name,"
                                + " CE.Name as AName,CE.Serial_Number as ANumber,EI.Name as BName,EI.Serial_Number as BNumber,DM.Name as CName,DM.Serial_Number as CNumber ,'' as Serial_Number ,'' as Materiel_Name "
                                + " FROM MATERIEL_STOCKS_MOVE MSM   "
                                + " INNER JOIN FACTORY FF ON FF.RST = 'A' AND FF.Is_Perso = 'Y' AND MSM.From_Factory_RID = FF.RID"
                                + " INNER JOIN FACTORY TF ON TF.RST = 'A' AND TF.Is_Perso = 'Y' AND MSM.To_Factory_RID = TF.RID "
                                + " LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MSM.Serial_Number = CE.Serial_Number "
                                + " LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MSM.Serial_Number = EI.Serial_Number "
                                + " LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MSM.Serial_Number = DM.Serial_Number where MSM.rst = 'A' AND MSM.Move_ID = @Move_ID";           
        
    public const string DEL_MATERIEL_STOCKS_MOVE_BY_MOVE_ID = "DELETE "
                                + "FROM MATERIEL_STOCKS_MOVE "
                                + "WHERE Move_ID = @move_id";
  
    public const string SEL_MATERIEL_ALL = "SELECT Serial_Number as value,NAME"
                                + " FROM CARD_EXPONENT"
                                + " WHERE RST='A' "
                                + " UNION SELECT Serial_Number as value,NAME "
                                + " FROM ENVELOPE_INFO WHERE RST='A' "
                                + " UNION SELECT Serial_Number as value,NAME "
                                + " FROM DMTYPE_INFO WHERE RST='A' ";

    public const string SEL_FACTORY_ALL = "SELECT RID,Factory_ShortName_CN "
                                + "FROM FACTORY "
                                + "WHERE RST = 'A' AND Is_Perso = 'Y' ";

    public const string SEL_MAX_MOVE_ID = "SELECT TOP 1 Move_ID "
                                + "FROM MATERIEL_STOCKS_MOVE "
                                + "WHERE Move_Date >= @move_date1 AND Move_Date<=@move_date2 "
                                + "ORDER BY Move_ID DESC ";
    public const string CON_CHECK_DATE = "SELECT count(*) from  MATERIEL_STOCKS where stock_date>=@move_date";
    #endregion
    
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Depository011_1BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// Depository011_1.aspx頁面查詢
    /// </summary>
    /// <returns></returns>
    public DataSet GetPageDatas()
    {
        DataSet dstPageDatas = null;
        try
        {
            dstPageDatas = dao.GetList(SEL_MATERIEL_STOCKS_MOVE_BY_MOVE_DATE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstPageDatas;
    }

    public bool Is_Managed(DateTime moveDate) {
        dirValues.Clear();
        dirValues.Add("move_date", moveDate);
        return dao.Contains(CON_CHECK_DATE, dirValues);        
    }
    /// <summary>
    /// mod頁面資料加載
    /// </summary>
    /// <param name="move_id"></param>
    /// <returns></returns>
    public DataSet GetModDatas(string move_id)
    {
        DataSet dstMod = null;

        // 已選擇的卡種
        dirValues.Clear();
        dirValues.Add("Move_ID", int.Parse(move_id));
                
        try
        {             
            dstMod = dao.GetList(SEL_MATERIEL_STOCKS_MOVE_BY_MOVE_ID, dirValues);
            return dstMod;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    public int Add(MATERIEL_STOCKS_MOVE model)
    {
        int rid = 0;
        try
        {
            dao.OpenConnection();
            rid = Convert.ToInt32(dao.AddAndGetID<MATERIEL_STOCKS_MOVE>(model, "Rid"));

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
        return rid;
    
    }

    public void delete(int rid) {
        dirValues.Clear();
        dirValues.Add("RID", rid);
        try
        {
            dao.OpenConnection();
            dao.Delete("Materiel_Stocks_Move",dirValues);
           

            //操作日誌
            SetOprLog("4");

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

    public void Update(MATERIEL_STOCKS_MOVE model)
    {
        try
        {
            dao.OpenConnection();
            dao.Update<MATERIEL_STOCKS_MOVE>(model, "RID");

            //操作日誌
            SetOprLog("3");

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

    public MATERIEL_STOCKS_MOVE getModel(int rid) {
        MATERIEL_STOCKS_MOVE model = null;
        try {
            model = dao.GetModel<MATERIEL_STOCKS_MOVE, int>("RID", rid);
          }
          catch (Exception ex)
          {
              ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
              throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return model;    
    }
    

    /// <summary>
    /// 獲取所有物料名
    /// </summary>
    /// <returns></returns>
    public DataSet GetMaterielAll()
    {
        DataSet dstMaterielAll = null;

        try
        {
            dstMaterielAll = dao.GetList(SEL_MATERIEL_ALL);

            return dstMaterielAll;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 獲取所有Perso廠名
    /// </summary>
    /// <returns></returns>
    public DataSet GetFactoryAll()
    {
        DataSet dstFactoryAll = null;

        try
        {
            dstFactoryAll = dao.GetList(SEL_FACTORY_ALL);
            
            return dstFactoryAll;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 獲取資料表中相同YYYYMMDD的轉移單號的最大ID
    /// </summary>
    /// <param name="Move_Date">移動日期</param>
    /// <returns></returns>
    public string GetMove_ID(String Move_Date)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("move_date1", Move_Date + " 00:00:00");
            dirValues.Add("move_date2", Move_Date + " 23:59:59");
            DateTime dtMove_Date = Convert.ToDateTime(Move_Date);

            // 取轉移日期當天的最大轉移單號
            DataSet dtsMaxMoveID = dao.GetList(SEL_MAX_MOVE_ID, dirValues);
            if (dtsMaxMoveID.Tables[0].Rows.Count > 0)
            {
                int intMaxID = Convert.ToInt32(dtsMaxMoveID.Tables[0].Rows[0]["Move_ID"].ToString().Substring(8, 2));
                intMaxID++;
                if (intMaxID > 9)
                {
                    return dtMove_Date.ToString("yyyyMMdd") + intMaxID.ToString();
                }
                else
                {
                    return dtMove_Date.ToString("yyyyMMdd") + "0" + intMaxID.ToString();
                }
            }
            else
            {
                return dtMove_Date.ToString("yyyyMMdd") + "01";
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 查詢項目資料列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Move_Date" : sortField);//默認的排序欄位

        sortType = (sortField == "null" ? "desc " : sortType);//默認的排序欄位
        //if (strSortField == "Move_Date1")
        //{
        //    strSortField = "Move_Date";
        //}

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_MATERIAL_MOVE);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
           
            if (!StringUtil.IsEmpty(searchInput["txtBeginDate"].ToString().Trim()))
            {
                stbWhere.Append(" and Move_Date >= @BeginDate");
                dirValues.Add("BeginDate", searchInput["txtBeginDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtEndDate"].ToString().Trim()))
            {
                stbWhere.Append(" and Move_Date<=@EndDate");
                dirValues.Add("EndDate", searchInput["txtEndDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropFromFactory"].ToString().Trim()))
            {
                stbWhere.Append(" and From_Factory_RID = @From_Factory");
                dirValues.Add("From_Factory", searchInput["dropFromFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropToFactory"].ToString().Trim()))
            {
                stbWhere.Append(" and To_Factory_RID = @To_Factory");
                dirValues.Add("To_Factory", searchInput["dropToFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtMaterial"].ToString().Trim()))
            {
                stbWhere.Append(" and (dm.name like @Name or ce.name like @Name or ei.name like @Name) ");
                dirValues.Add("Name", "%" + searchInput["txtMaterial"].ToString().Trim() + "%");
            }
        }

        //執行SQL語句
        DataSet dstMateriel_Stocks_Move = null;
        try
        {
            dstMateriel_Stocks_Move = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstMateriel_Stocks_Move;
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
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 查詢detail資料列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet ListDetail(string Move_ID, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;

        sortField = "Materiel_Type";
        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_MATERIEL_STOCKS_MOVE_BY_MOVE_ID);
        
        dirValues.Clear();
        
        dirValues.Add("Move_ID",Move_ID);

        //執行SQL語句
        DataSet dstMateriel_Stocks_Move = null;
        try
        {
            dstMateriel_Stocks_Move = dao.GetList(stbCommand.ToString() , dirValues, firstRowNumber, lastRowNumber, sortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstMateriel_Stocks_Move;
    }


    /// <summary>
    /// 物料轉移單增加
    /// </summary>
    /// <param name="dtbl"></param>
    /// <param name="Move_Date"></param>
    /// <param name="Move_ID"></param>
    //public void Add(DataTable dtbl,string Move_Date,string Move_ID)        
    //{
    //    try
    //    {
    //        //事務開始
    //        dao.OpenConnection();

    //        // 將UI訊息保存到資料庫中，先刪除資料庫
    //        this.dirValues.Clear();
    //        this.dirValues.Add("Move_ID", Convert.ToInt32(Move_ID));
    //        dao.ExecuteNonQuery(DEL_MATERIEL_STOCKS_MOVE_BY_MOVE_ID, dirValues);

    //        // 添加訊息
    //        Materiel_Stocks_Move msvModel = new Materiel_Stocks_Move();
    //        foreach (DataRow drow in dtbl.Rows)
    //        {
    //            msvModel.Materiel_RID = Convert.ToInt32(drow["Materiel_RID"]);
    //            msvModel.From_Factory_RID = Convert.ToInt32(drow["From_Factory_RID"]);
    //            msvModel.To_Factory_RID = Convert.ToInt32(drow["To_Factory_RID"]); ;
    //            msvModel.Materiel_Type = drow["Materiel_Type"].ToString();
    //            msvModel.Move_Number = Convert.ToInt32(drow["Move_Number"]); ;
    //            msvModel.Move_Date = Convert.ToDateTime(Move_Date);
    //            msvModel.Move_ID = Move_ID;
    //            dao.Add<Materiel_Stocks_Move>(msvModel, "RID");
    //        }            

    //        //事務提交
    //        dao.Commit();
    //    }
    //    catch (Exception ex)
    //    {
    //        //事務回滾
    //        dao.Rollback();
    //        ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
    //        throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
    //    }
    //    finally
    //    {
    //        //關閉連接
    //        dao.CloseConnection();
    //    }
   // }
 
    /// <summary>
    /// 刪除物料轉移單
    /// </summary>
    /// <param name="move_id"></param>
    public void DeleteAll(String move_id)
    {
        try
        {
            // 事務開始
            dao.OpenConnection();

            this.dirValues.Clear();
            this.dirValues.Add("Move_ID", Convert.ToInt32(move_id));
            dao.ExecuteNonQuery(DEL_MATERIEL_STOCKS_MOVE_BY_MOVE_ID, dirValues);
           
            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();

            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }
}
