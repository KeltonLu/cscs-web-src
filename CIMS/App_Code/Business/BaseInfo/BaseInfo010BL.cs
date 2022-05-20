//******************************************************************
//*  作    者：Macuiyan
//*  功能說明：公式定義管理邏輯
//*  創建日期：2008-10-09
//*  修改日期：2008-10-08 12:00
//*  修改記錄：
//*            □2008-10-31
//*              1.創建 馬翠艷
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

/// <summary>
/// BaseInfo010BL 的摘要描述
/// </summary>
public class BaseInfo010BL:BaseLogic
{
    #region SQL
    public static string SEL_EXPRESSION_PARAMNAME = "SELECT RID,Param_Code,Param_Name"
	    +" FROM PARAM"
        + " WHERE rst = 'A'  AND ParamType_Code = @param";

    public static String SEL_OPERATOR = "SELECT RID,PARAM_NAME"
     + " FROM PARAM"
     + " WHERE RST = 'A' AND PARAMTYPE_CODE = @paramType ORDER BY Param_Name DESC";

    public static string SEL_OPERATOR_BY_STATUSANDFORMULA = "SELECT Operate"
        + " FROM EXPRESSIONS_DEFINE AS EXP"
        + " WHERE EXP.Type_RID = @statusRid AND EXP.Expressions_RID =@formulaRid";

    public static string SEL_EXPRESSION_STATUS = "SELECT RID,Status_NAME, status_CODE"
        + " FROM CARDTYPE_STATUS"
        + " WHERE rst = 'A' AND IS_DISPLAY = 'Y'";

    public static string DEL_EXPRESSION = "DELETE FROM EXPRESSIONS_DEFINE";


 

    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    
    public BaseInfo010BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 取所有的公式名稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetParam() {
        DataSet result = null;
        dirValues.Clear();
        try
        {
            dirValues.Add("param", "FormulaName");
            result = dao.GetList(SEL_EXPRESSION_PARAMNAME,dirValues);

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return result;
    
    }

    /// <summary>
    /// 保存公式設定
    /// </summary>
    /// <param name="dtblFormula"></param>
    public void Update(DataTable dtblFormula,DataTable dtblParam) {
        try
        {
            dao.ExecuteNonQuery(DEL_EXPRESSION);
            foreach (DataRow drow in dtblFormula.Rows)
            {

                EXPRESSIONS_DEFINE model = new EXPRESSIONS_DEFINE();
                model.Expressions_RID = int.Parse(drow["Expressions_RID"].ToString());
                model.Type_RID = int.Parse(drow["Type_RID"].ToString());
                model.Operate = drow["Operate_RID"].ToString();
                dao.Add<EXPRESSIONS_DEFINE>(model, "RID");
            }
            foreach (DataRow drow in dtblParam.Rows) {
                PARAM param = dao.GetModel<PARAM, int>("RID", int.Parse(drow["rid"].ToString()));
                param.Param_Name = drow["Param_Name"].ToString();
                dao.Update<PARAM>(param, "RID");
            }

            //操作日誌
            SetOprLog("3");
        }
        catch (Exception ex) { 
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }      
    
    }

    /// <summary>
    /// 根據公式和項目狀態取操作符
    /// </summary>
    /// <param name="formulaRid"></param>
    /// <param name="statusRid"></param>
    /// <returns></returns>
    public string getOPeratorByFormulaAndStatus(string formulaRid,string statusRid) {
        dirValues.Clear();
        try
        {
            dirValues.Add("formulaRid", formulaRid);
            dirValues.Add("statusRid", statusRid);
            DataTable dtbResult = dao.GetList(SEL_OPERATOR_BY_STATUSANDFORMULA , dirValues).Tables[0];
            if (dtbResult != null&&dtbResult.Rows.Count > 0) {
                return dtbResult.Rows[0]["OPERATE"].ToString();
            }

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return " ";      
    }

    /// <summary>
    /// 取所有可顯示的項目狀態
    /// </summary>
    /// <returns></returns>
    public DataSet GetStatus()
    {
        DataSet result = null;        
        try
        {            
            result = dao.GetList(SEL_EXPRESSION_STATUS);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return result;
    }

    /// <summary>
    /// 取所有的操作符
    /// </summary>
    /// <returns></returns>
    public DataSet GetOperator() {
        DataSet result = null;
        dirValues.Clear();
        try {
            dirValues.Add("paramType", "Operation");
            result =  dao.GetList(SEL_OPERATOR,dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return result;
    
    }   
}
