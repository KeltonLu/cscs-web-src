//******************************************************************
//*  作    者：Ian Huang
//*  功能說明：物料庫存異動邏輯
//*  創建日期：2010-06-11
//*  修改日期：
//*  修改記錄：
//*              
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
using System.Text;

/// <summary>
/// Depository016BL 的摘要描述
/// </summary>
public class Depository016BL : BaseLogic
{
    #region SQL
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

    public const string SEL_PARAM_010203 = @"select RID,Param_Name from PARAM
                                            where ParamType_Code = 'matType1' and convert(int,Param_Code) < 4 and RST = 'A'
                                            order by Param_Code";

    public const string CON_CHECK_DATE = "SELECT count(*) from  MATERIEL_STOCKS where stock_date>=@Transaction_Date";

    public const string SEL_MAX_Transaction_ID = "SELECT TOP 1 Transaction_ID "
                                + "FROM MATERIEL_STOCKS_TRANSACTION "
                                + "WHERE Transaction_Date >= @Transaction_date1 AND Transaction_Date<=@Transaction_date2 "
                                + "ORDER BY Transaction_ID DESC ";

    public const string SEL_MATERIAL_Transaction = @"SELECT MST.RID,Transaction_Date,Transaction_ID,Transaction_Amount,FF.Factory_ShortName_CN AS Factory_Name,P.Param_Name,
                                CE.Name as AName,CE.Serial_Number as ANumber,EI.Name as BName,EI.Serial_Number as BNumber,DM.Name as CName,DM.Serial_Number as CNumber  
                                FROM MATERIEL_STOCKS_TRANSACTION MST  
                                INNER JOIN FACTORY FF ON FF.RST = 'A' AND FF.Is_Perso = 'Y' AND MST.Factory_RID = FF.RID
                                INNER JOIN PARAM P ON P.RST = 'A' AND MST.PARAM_RID = P.RID
                                LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MST.Serial_Number = CE.Serial_Number 
                                LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MST.Serial_Number = EI.Serial_Number 
                                LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MST.Serial_Number = DM.Serial_Number where MST.rst = 'A'";

    public const string SEL_MATERIEL_STOCKS_TRANSACTION_BY_TRANSACTION_ID = @"SELECT MST.RID,Transaction_Date,Transaction_Amount,Factory_RID,PARAM_RID,FF.Factory_ShortName_CN AS Factory_Name,P.Param_Name,
                                CE.Name as AName,CE.Serial_Number as ANumber,EI.Name as BName,EI.Serial_Number as BNumber,DM.Name as CName,DM.Serial_Number as CNumber ,'' as Serial_Number ,'' as Materiel_Name 
                                FROM MATERIEL_STOCKS_TRANSACTION MST   
                                INNER JOIN FACTORY FF ON FF.RST = 'A' AND FF.Is_Perso = 'Y' AND MST.Factory_RID = FF.RID
                                INNER JOIN PARAM P ON P.RST = 'A' AND MST.PARAM_RID = P.RID 
                                LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MST.Serial_Number = CE.Serial_Number 
                                LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MST.Serial_Number = EI.Serial_Number 
                                LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MST.Serial_Number = DM.Serial_Number where MST.rst = 'A' AND MST.Transaction_ID = @Transaction_ID";

    public const string DEL_MATERIEL_STOCKS_TRANSACTION_BY_TRANSACTION_ID = "DELETE "
                                + "FROM MATERIEL_STOCKS_TRANSACTION "
                                + "WHERE Transaction_ID = @Transaction_ID";
    #endregion SQL

    #region 參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    #endregion 參數

    public Depository016BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
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

    public DataSet GetPARAM010203()
    {
        DataSet dsPARAM010203 = null;

        try
        {
            dsPARAM010203 = dao.GetList(SEL_PARAM_010203);

            return dsPARAM010203;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    public bool Is_Managed(DateTime TransactionDate)
    {
        dirValues.Clear();
        dirValues.Add("Transaction_Date", TransactionDate);
        return dao.Contains(CON_CHECK_DATE, dirValues);
    }

    public int Add(MATERIEL_STOCKS_TRANSACTION model)
    {
        int rid = 0;
        try
        {
            dao.OpenConnection();
            rid = Convert.ToInt32(dao.AddAndGetID<MATERIEL_STOCKS_TRANSACTION>(model, "Rid"));

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

    public MATERIEL_STOCKS_TRANSACTION getModel(int rid)
    {
        MATERIEL_STOCKS_TRANSACTION model = null;
        try
        {
            model = dao.GetModel<MATERIEL_STOCKS_TRANSACTION, int>("RID", rid);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return model;
    }

    public void Update(MATERIEL_STOCKS_TRANSACTION model)
    {
        try
        {
            dao.OpenConnection();
            dao.Update<MATERIEL_STOCKS_TRANSACTION>(model, "RID");

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

    /// <summary>
    /// 獲取資料表中相同YYYYMMDD的轉移單號的最大ID
    /// </summary>
    /// <param name="Move_Date">移動日期</param>
    /// <returns></returns>
    public string GetTransaction_ID(String Transaction_Date)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Transaction_date1", Transaction_Date + " 00:00:00");
            dirValues.Add("Transaction_date2", Transaction_Date + " 23:59:59");
            DateTime dtTransaction_Date = Convert.ToDateTime(Transaction_Date);

            // 取轉移日期當天的最大轉移單號
            DataSet dtsMaxTransactionID = dao.GetList(SEL_MAX_Transaction_ID, dirValues);
            if (dtsMaxTransactionID.Tables[0].Rows.Count > 0)
            {
                int intMaxID = Convert.ToInt32(dtsMaxTransactionID.Tables[0].Rows[0]["Transaction_ID"].ToString().Substring(8, 2));
                intMaxID++;
                if (intMaxID > 9)
                {
                    return dtTransaction_Date.ToString("yyyyMMdd") + intMaxID.ToString();
                }
                else
                {
                    return dtTransaction_Date.ToString("yyyyMMdd") + "0" + intMaxID.ToString();
                }
            }
            else
            {
                return dtTransaction_Date.ToString("yyyyMMdd") + "01";
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    public void delete(int rid)
    {
        dirValues.Clear();
        dirValues.Add("RID", rid);
        try
        {
            dao.OpenConnection();
            dao.Delete("MATERIEL_STOCKS_TRANSACTION", dirValues);


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
        string strSortField = (sortField == "null" ? "Transaction_Date" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_MATERIAL_Transaction);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtBeginDate"].ToString().Trim()))
            {
                stbWhere.Append(" and Transaction_Date >= @BeginDate");
                dirValues.Add("BeginDate", searchInput["txtBeginDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtEndDate"].ToString().Trim()))
            {
                stbWhere.Append(" and Transaction_Date <= @EndDate");
                dirValues.Add("EndDate", searchInput["txtEndDate"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" and Factory_RID = @Factory");
                dirValues.Add("Factory", searchInput["dropFactory"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["dropPARAM"].ToString().Trim()))
            {
                stbWhere.Append(" and PARAM_RID = @PARAM");
                dirValues.Add("PARAM", searchInput["dropPARAM"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtMaterial"].ToString().Trim()))
            {
                stbWhere.Append(" and (dm.name like @Name or ce.name like @Name or ei.name like @Name) ");
                dirValues.Add("Name", "%" + searchInput["txtMaterial"].ToString().Trim() + "%");
            }
        }

        //執行SQL語句
        DataSet dstMateriel_Stocks_Transaction = null;

        try
        {
            dstMateriel_Stocks_Transaction = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstMateriel_Stocks_Transaction;
    }

    /// <summary>
    /// mod頁面資料加載
    /// </summary>
    /// <param name="move_id"></param>
    /// <returns></returns>
    public DataSet GetModDatas(string transaction_id)
    {
        DataSet dstMod = null;

        // 已選擇的卡種
        dirValues.Clear();
        dirValues.Add("Transaction_ID", int.Parse(transaction_id));

        try
        {
            dstMod = dao.GetList(SEL_MATERIEL_STOCKS_TRANSACTION_BY_TRANSACTION_ID, dirValues);
            return dstMod;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 刪除物料轉移單
    /// </summary>
    /// <param name="move_id"></param>
    public void DeleteAll(String transaction_id)
    {
        try
        {
            // 事務開始
            dao.OpenConnection();

            this.dirValues.Clear();
            this.dirValues.Add("Transaction_ID", Convert.ToInt32(transaction_id));
            dao.ExecuteNonQuery(DEL_MATERIEL_STOCKS_TRANSACTION_BY_TRANSACTION_ID, dirValues);

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
