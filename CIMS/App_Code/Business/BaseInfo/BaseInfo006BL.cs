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
/// BasicInfo006 的摘要描述
/// </summary>
public class BaseInfo006BL:BaseLogic
{
    #region SQL語句
    public const string SEL_PARAM = "SELECT * "
                                        + "from PARAM "
                                        + "WHERE RST='A' "
                                        + "and Is_Delete<>'S' AND ParamType_code <> @total";
    public const string SEL_PARAMTYPE = "SELECT PARAM_CODE,PARAM_NAME,ParamType_Name "
                                        + "From PARAM "
                                        + "WHERE PARAMTYPE_CODE= @total";
    public const string SEL_BY_KEY = "SELECT *"
                                        + " from PARAM"
                                        + " WHERE PARAM_Name = @Param_Name AND ParamType_Code = @ParamType_Code";
    public const string CON_BY_CODE = "SELECT COUNT(*) "
                                        + "FROM PARAM "
                                        +"WHERE RST='A' "
                                        + "AND PARAM_Name = @Param_Name and ParamType_Code = @ParamType_Code";
    public const string SEL_DEL_TYPE1 = "SELECT COUNT(*)"
                                        + " FROM CARD_GROUP "
                                        + " WHERE RST='A' AND Param_Code = @paramCode";    
    public const string SEL_DEL_TYPE2 = "SELECT COUNT(*)"
                                        +" FROM GROUP_CARDTYPE"
                                        +" WHERE RST='A' AND Param_RID = @paramRid";
    public const string SEL_DEL_TYPE3 = "SELECT COUNT(*)"
                                        + " FROM MATERIEL_STOCKS_MANAGE"
                                        + " WHERE RST='A' AND Type = @paramCode";
#endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo006BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cbModel"></param>
    public void Add(PARAM cbModel)
    {
        DataSet dslparam = null;
        dirValues.Clear();
        dirValues.Add("ParamType_Code",cbModel.ParamType_Code);
        dirValues.Add("Param_Name",cbModel.Param_Name);
        try
        {    
           
            dslparam = dao.GetList(SEL_BY_KEY, dirValues);
            if(dslparam.Tables[0].Rows.Count >0)          
            {
               
                cbModel.RID = (int)dslparam.Tables[0].Rows[0]["RID"];
                cbModel.RST = "A";
                dao.Update<PARAM>(cbModel, "RID");

            }else{
                if (cbModel.ParamType_Code == GlobalString.ParameterType.MatType1) {

                    // Legend 2017/05/15 調整 參數編號超過10后獲得錯誤問題
                    DataTable tdb = dao.GetList("select max(convert(int, param_code)) from param where ParamType_Code = '" + GlobalString.ParameterType.MatType1+"'").Tables[0];
                    cbModel.Param_Code = (Convert.ToInt32(tdb.Rows[0][0])+1)+"";
                  
                }else{
                    string strRID = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Param_RID");
                    cbModel.Param_Code = strRID;
                }
                cbModel.Is_Delete = "Y";               
               dao.Add<PARAM>(cbModel, "RID");            
            }

            SetOprLog();
      
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 查詢是否已存在ParamCode
    /// </summary>
    /// <param name="strParamCode"></param>
    /// <returns></returns>
    public bool ContainsID(string strParam,string strParamTypeCode)    {
        
        try
        {
            dirValues.Clear();
            dirValues.Add("Param_Name", strParam);
            dirValues.Add("ParamType_Code", strParamTypeCode);
            return dao.Contains(CON_BY_CODE, dirValues);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// 列出所有的ParamType，頁面查詢條件
    /// </summary>
    /// <returns></returns>
    public DataSet getParamType(string strParamType_Code)
    {        
        DataSet dtsparam_Type = null;
        dirValues.Clear();
        try
        {
            dirValues.Add("total", GlobalString.ParameterType.Total);
            dirValues.Add("PARAM_CODE", strParamType_Code);
            dtsparam_Type = dao.GetList(SEL_PARAMTYPE + " AND PARAM_CODE = @PARAM_CODE", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dtsparam_Type;            
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="firstRowNumber">頁面顯示第一條記錄編號</param>
    /// <param name="lastRowNumber">頁面顯示最後一條記錄編號</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序方式</param>
    /// <param name="rowCount">行數</param>
    /// <returns></returns>
    public DataSet list(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "ParamType_Code" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_PARAM);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        dirValues.Add("total", GlobalString.ParameterType.Total);
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtPARAM_NAME"].ToString().Trim()))
            {
                stbWhere.Append(" AND PARAM_NAME like @PARAM_NAME ");
                dirValues.Add("PARAM_NAME", "%" + searchInput["txtPARAM_NAME"].ToString().Trim() + "%");
                
            }

            if (!StringUtil.IsEmpty(searchInput["ParamType_Code"].ToString().Trim()))
            {
                stbWhere.Append(" AND ParamType_Code = @ParamType_Code ");
                dirValues.Add("ParamType_Code", searchInput["ParamType_Code"].ToString().Trim());

            }
        }
        DataSet dtsparam_Type = null;
        try
        {
            dtsparam_Type = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        rowCount = intRowCount;
        return dtsparam_Type;
    }

    /// <summary>
    /// 根據RID取Model
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public PARAM GetParameter(string strRID)
    {
        PARAM cbModel = null;
        try
        {
            cbModel = dao.GetModel<PARAM, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return cbModel;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="cboModel"></param>
    public void Update(PARAM cboModel)
    {
        try
        {
            //保存預算記錄
            dao.Update<PARAM>(cboModel, "RID");
            SetOprLog();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void Delete(string strRID, string strType)
    {
        //資料實體
        PARAM cbModel = dao.GetModel<PARAM, int>("RID", int.Parse(strRID));
        try
        {
            dao.OpenConnection();

            DataSet dst = null;
            if (strType == "1")
            {
                dirValues.Clear();
                dirValues.Add("paramCode", cbModel.Param_Code);
                dst = dao.GetList(SEL_DEL_TYPE1, dirValues);
            }
            else if (strType == "2")
            {
                dirValues.Clear();
                dirValues.Add("paramRid", cbModel.RID);
                dst = dao.GetList(SEL_DEL_TYPE2, dirValues);
            }
            else if (strType == "3")
            {
                dirValues.Clear();
                dirValues.Add("paramCode", cbModel.Param_Code);
                dst = dao.GetList(SEL_DEL_TYPE3, dirValues);
            }
            if (dst != null && dst.Tables[0].Rows[0][0].ToString() != "0")
            {
                throw new AlertException(BizMessage.BizCommMsg.ALT_CMN_CanntDel);
            }
            else
            {
                dirValues.Clear();
                dirValues.Add("RID", int.Parse(strRID));
                dao.Delete("Param", dirValues);

                ////進行邏輯刪除處理
                //cbModel.RST = "D";
                ////保存預算記錄
                //dao.Update<Param>(cbModel, "RID");
            }

            SetOprLog("4");

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            dao.Rollback();
            //異常處理
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
        finally
        {
            dao.CloseConnection();
        }

    }

    /// <summary>
    /// 參數修改
    /// </summary>
    /// <param name="dtblParam"></param>
    public void ParamEdit(DataTable dtblParam)
    {

        if (dtblParam.Rows.Count == 0)
            return;

        try
        {
            //事務開始
            dao.OpenConnection();

            foreach (DataRow drowParam in dtblParam.Rows)
            {
                PARAM pModel = dao.GetModel<PARAM, int>("RID", int.Parse(drowParam["RID"].ToString()));
                pModel.Param_Name = drowParam["Param_Name"].ToString();
                pModel.Param_Comment = drowParam["Param_Comment"].ToString();
                dao.Update<PARAM>(pModel, "RID");
            }

            //操作日誌
            SetOprLog("3");

            //事務提交
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

}
