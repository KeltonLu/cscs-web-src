//******************************************************************
//*  作    者：QingChen
//*  功能說明：用戶權限信息獲取邏輯
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Text;
/// <summary>
/// 獲取用戶權限功能編碼的物件
/// </summary>
public abstract class UserFunctionProvider
{
    /// <summary>
    /// 獲取用戶可用的功能編碼
    /// </summary>
    /// <param name="user">用戶物件</param>
    /// <returns>功能編碼集</returns>
    public static List<string> GetFunctions(USERS user)
    {
        List<string> lisReturnFunctions = new List<string>();

        #region Legend 2016/10/26 因企業庫升級調整寫法

        //Database dbDatabase = DatabaseFactory.CreateDatabase();

        DatabaseProviderFactory factory = new DatabaseProviderFactory();

        Database dbDatabase = factory.CreateDefault();

        #endregion 

        StringBuilder stbCommandString = new StringBuilder("select distinct ActionID from dbo.RoleFunctionAction");
        stbCommandString.Append(" where roleid in (select roleid from USERSRole where userid='");
        stbCommandString.Append(user.UserID);
        stbCommandString.Append("')");
        DataSet dstFunctionList = null;
        try
        {
            dstFunctionList = dbDatabase.ExecuteDataSet(CommandType.Text, stbCommandString.ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizPublicMsg.ALT_GetUserFunctionFail, ex);
            throw ex;
        }

        if (dstFunctionList != null && dstFunctionList.Tables.Count > 0 && dstFunctionList.Tables[0].Rows.Count > 0)
        {
            for (int index = 0; index < dstFunctionList.Tables[0].Rows.Count; index++)
            {
                lisReturnFunctions.Add(dstFunctionList.Tables[0].Rows[index]["ActionID"].ToString().Trim());
            }
        }
        lisReturnFunctions.TrimExcess();
        return lisReturnFunctions;
    }

    /// <summary>
    /// 獲取用戶可用的功能編碼
    /// </summary>
    /// <param name="user">用戶物件</param>
    /// <returns>功能編碼集</returns>
    public static List<string> GetFunctions(string strRoles)
    {
        List<string> lisReturnFunctions = new List<string>();

        #region Legend 2016/10/26 因企業庫升級調整寫法

        //Database dbDatabase = DatabaseFactory.CreateDatabase();

        DatabaseProviderFactory factory = new DatabaseProviderFactory();

        Database dbDatabase = factory.CreateDefault();

        #endregion

        StringBuilder stbCommandString = new StringBuilder("select distinct ActionID from dbo.RoleFunctionAction");
        stbCommandString.Append(" where roleid in (");
        stbCommandString.Append(strRoles);
        stbCommandString.Append(")");
        DataSet dstFunctionList = null;
        try
        {
            dstFunctionList = dbDatabase.ExecuteDataSet(CommandType.Text, stbCommandString.ToString());
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizPublicMsg.ALT_GetUserFunctionFail, ex);
            throw ex;
        }

        if (dstFunctionList != null && dstFunctionList.Tables.Count > 0 && dstFunctionList.Tables[0].Rows.Count > 0)
        {
            for (int index = 0; index < dstFunctionList.Tables[0].Rows.Count; index++)
            {
                lisReturnFunctions.Add(dstFunctionList.Tables[0].Rows[index]["ActionID"].ToString().Trim());
            }
        }
        lisReturnFunctions.TrimExcess();
        return lisReturnFunctions;
    }
   
}
