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
/// InOut006BL 的摘要描述
/// </summary>
public class InOut006BL : BaseLogic
{
    #region SQL語句
    public const string SEL_IMPORT_HISTORY = "SELECT * FROM IMPORT_HISTORY WHERE RST = 'A' and RCU = 'BATCH' ";
    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public InOut006BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    
    //public DataSet GetFile_Name()
    //{
    //    DataSet dsFile_Name = null;
    //    try
    //    {
    //        dsFile_Name = dao.GetList(SEL_IMPORT_HISTORY);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
    //        throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
    //    }
    //    return dsFile_Name;
    //}

    public void AddLog(string strType, string strFileName)
    {
        try
        {
            IMPORT_HISTORY ihModel = new IMPORT_HISTORY();
            ihModel.File_Name = strFileName;
            ihModel.File_Type = strType;
            ihModel.Import_Date = DateTime.Now;
            dao.Add<IMPORT_HISTORY>(ihModel, "RID");
        }
        catch
        {
            throw new Exception("記錄歷史失敗");
        }
    }

    public DataSet Search(Dictionary<string, object> searchInput)
    {
        DataSet dsIMPORT_HISTORY = null;
        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_IMPORT_HISTORY);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (searchInput["dropFile_Name"].ToString() != "")
            {
                stbWhere.Append(" AND File_Type= @file_type ");
                dirValues.Add("file_type", searchInput["dropFile_Name"].ToString());

            }
            if (searchInput["txtBegin_Date"].ToString() != "")
            {
                stbWhere.Append(" AND Import_Date>= @Begin_Date ");
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString()+" 00:00:00");
            }
            if (searchInput["txtFinish_Date"].ToString() != "")
            {
                stbWhere.Append(" AND Import_Date<= @Finish_Date ");
                dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString()+" 23:59:59");
            }
        }

        try
        {
            dsIMPORT_HISTORY = dao.GetList(stbCommand.ToString() + stbWhere.ToString() + " order by Import_Date desc ", dirValues);

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return dsIMPORT_HISTORY;
    }
}
