//******************************************************************
//*  作    者：FangBao
//*  功能說明：使用者權限維護邏輯
//*  創建日期：2008-07-31
//*  修改日期：2008-07-31 12:00
//*  修改記錄：
//*            □2008-07-31
//*              1.創建 鮑方
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
using ControlLibrary;
using System.Text;

/// <summary>
/// RoleManager 的摘要描述
/// </summary>
public class BaseInfo041BL : BaseLogic
{

    #region sql語句
    public const string SEL_ROLE_BY_UID = "SELECT A.ROLEID,B.ROLENAME FROM USERSROLE A INNER JOIN [ROLE] B ON A.ROLEID=B.ROLEID WHERE A.USERID=@userid";
    public const string SEL_ROLE = "SELECT [ROLE].[ROLEID], [ROLE].[ROLENAME] FROM [ROLE] WHERE 1>0 ";
    public const string SEL_ROLE_BY_RNAME = "SELECT COUNT(*) AS EXPR1 FROM [ROLE] WHERE ROLENAME = @rolename";
    public const string CON_ROLE_BY_RID = "SELECT C.A+C.B FROM (SELECT (SELECT COUNT(*) FROM DBO.ROLEFUNCTIONACTION WHERE ROLEID=@roleid) AS A,(SELECT COUNT(*) FROM DBO.USERSROLE WHERE ROLEID=@roleid) AS B) C";
    public const string SEL_FUNACTION_BY_RID = "DECLARE @MAXLINE INT DECLARE @INDEX INT SET @INDEX=1 SELECT @MAXLINE=MAX([LEVEL]) FROM [FUNCTION] WHILE(@INDEX<=@MAXLINE) BEGIN SELECT DISTINCT [FUNCTION].FUNCTIONID, FUNCTIONNAME,SortOrder,PARENTFUNCTIONID,CASE  WHEN ROLEFUNCTIONACTION.ROLEID IS NULL THEN 'A' WHEN ROLEFUNCTIONACTION.ROLEID IS NOT NULL THEN 'B' END AS ROLEID INTO #TEMPTABLE FROM [FUNCTION] LEFT JOIN ROLEFUNCTIONACTION ON ROLEFUNCTIONACTION.ROLEID=@roleid AND ROLEFUNCTIONACTION.FUNCTIONID=[FUNCTION].FUNCTIONID WHERE [LEVEL]=@INDEX SELECT * FROM #TEMPTABLE order by PARENTFUNCTIONID,SortOrder SELECT [ACTION].FUNCTIONID,[ACTION].ACTIONID,[ACTION].ACTIONNAME,CASE  WHEN ROLEFUNCTIONACTION.ROLEID IS NULL THEN 'A' WHEN ROLEFUNCTIONACTION.ROLEID IS NOT NULL THEN 'B' END AS ROLEID FROM [ACTION] LEFT JOIN ROLEFUNCTIONACTION ON ROLEFUNCTIONACTION.ACTIONID=[ACTION].ACTIONID AND ROLEFUNCTIONACTION.ROLEID=@roleid WHERE [ACTION].FUNCTIONID IN (SELECT FUNCTIONID FROM #TEMPTABLE) DROP TABLE #TEMPTABLE SET @INDEX=@INDEX+1 END";
    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();



    public BaseInfo041BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢角色
    /// </summary>
    /// <param name="strUserID">用戶編碼</param>
    /// <returns></returns>
    public DataSet SearchRole(string strUserID)
    {
        dirValues.Clear();
        dirValues.Add("userid", strUserID);

        DataSet dstRole = null;
        try
        {
            dstRole = dao.GetList(SEL_ROLE_BY_UID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dstRole;
    }

    /// <summary>
    /// 查詢用戶資料
    /// </summary>
    /// <param name="searchInput">條件控製項和欄位，Key為欄位</param>
    /// <param name="firstRowNumber">起始行號</param>
    /// <param name="lastRowNumber">結束行號</param>
    /// <param name="sortField">排序欄位</param>
    /// <param name="sortType">排序方式</param>
    /// <param name="rowCount">查到的行數</param>
    /// <returns>查到的資料集</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;//輸出參數與被
        string strSortField = (sortField == "null" ? "roleid" : sortField);//默認的排序欄位

        //--2、整理SQL語句-------------------------------------------------------------------------------------
        StringBuilder stbCommand = new StringBuilder(SEL_ROLE);

        //--3、整理查詢條件----------------------------------------------------------------------------------
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtRoleID"].ToString().Trim()))
            {
                stbWhere.Append(" AND RoleID LIKE @roleid");
                dirValues.Add("roleid", "%" + searchInput["txtRoleID"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["txtRoleName"].ToString().Trim()))
            {
                stbWhere.Append(" AND RoleName LIKE @rolename");
                dirValues.Add("rolename", "%" + searchInput["txtRoleName"].ToString().Trim() + "%");
            }
        }

        DataSet dstUsers = null;

        //--4、開始查詢
        try
        {
            dstUsers = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }

        //--5、返回資料--------------------------------------------------------------------------------------------
        rowCount = intRowCount;
        return dstUsers;
    }


    /// <summary>
    /// 查詢代理人資料
    /// </summary>
    /// <returns></returns>
    public DataSet SearchRoleData()
    {
        DataSet dstRole = null;
        // 開始查詢
        try
        {
            dstRole = dao.GetList(SEL_ROLE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        // 返回資料--------------------------------------------------------------------------------------------
        return dstRole;
    }

    /// <summary>
    /// 判斷角色名稱是否存在於資料庫中
    /// </summary>
    /// <param name="RoleName">群組名稱</param>
    public bool ContainsRole(string strRoleName,string strID)
    {
        // 參數列表
        dirValues.Clear();
        dirValues.Add("rolename", strRoleName);
        dirValues.Add("roleID", strID);


        if (StringUtil.IsEmpty(strID))
        {
            return dao.Contains(SEL_ROLE_BY_RNAME, dirValues);
        }
        else
        {
            return dao.Contains(SEL_ROLE_BY_RNAME + " and roleID !=@roleID", dirValues);
        }
    }

    /// <summary>
    /// 刪除角色信息
    /// </summary>
    /// <param name="strRoleID">用戶編碼</param>
    public void Delete(string strRoleID)
    {

        // 執行SQL語句
        try
        {
            //驗證是否可以刪除
            dirValues.Clear();
            dirValues.Add("roleid", strRoleID);
            if ((int)dao.GetList(CON_ROLE_BY_RID, dirValues).Tables[0].Rows[0][0] > 0)
            {
                throw new AlertException(BizMessage.BizMsg.ALT_BASEINFO_041_01);
            }

            dao.Delete("Role", dirValues);

            //操作日誌
            SetOprLog("4");
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
    }

    /// <summary>
    /// 新增角色
    /// </summary>
    /// <param name="strRoleName">角色名稱</param>
    public void Add(string strRoleName, Dictionary<string, string> addNode)
    {
        ROLEFUNCTIONACTION rfaModel = new ROLEFUNCTIONACTION();

        ROLE rModel = new ROLE();

        try
        {
            dao.OpenConnection();

            rModel.RoleName = strRoleName;
            rModel.RoleID = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("RoleID");
            dao.Add<ROLE>(rModel);

            foreach (string actionID in addNode.Keys)
            {
                if (actionID.Length >= 5)
                {
                    dirValues.Clear();
                    dirValues.Add("ROLEID", rModel.RoleID);
                    dirValues.Add("FUNCTIONID", addNode[actionID].Trim());
                    dirValues.Add("ACTIONID", addNode[actionID].Trim() + "1");
                    if (!dao.Contains("select count(*) from ROLEFUNCTIONACTION where ROLEID=@ROLEID AND FUNCTIONID=@FUNCTIONID AND ACTIONID=@ACTIONID", dirValues))
                    {
                        ROLEFUNCTIONACTION rfaModel1 = new ROLEFUNCTIONACTION();
                        rfaModel1.RoleID = rModel.RoleID;
                        rfaModel1.FunctionID = addNode[actionID].Trim();
                        rfaModel1.ActionID = addNode[actionID].Trim() + "1";
                        dao.Add<ROLEFUNCTIONACTION>(rfaModel1);
                    }
                }
                dirValues.Clear();
                dirValues.Add("ROLEID", rModel.RoleID);
                dirValues.Add("FUNCTIONID", addNode[actionID].Trim());
                dirValues.Add("ACTIONID", actionID.Trim());
                if (!dao.Contains("select count(*) from ROLEFUNCTIONACTION where ROLEID=@ROLEID AND FUNCTIONID=@FUNCTIONID AND ACTIONID=@ACTIONID", dirValues))
                {
                    rfaModel.RoleID = rModel.RoleID;
                    rfaModel.FunctionID = addNode[actionID].Trim();
                    rfaModel.ActionID = actionID.Trim();
                    dao.Add<ROLEFUNCTIONACTION>(rfaModel);
                }
            }

            //操作日誌
            SetOprLog("2");

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
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 儲存資料
    /// </summary>
    /// <param name="strRoleID">角色編碼</param>
    /// <param name="addNode">增加資料</param>
    /// <param name="deletedNode">刪除資料</param>
    public void Updata(string strName,string strRoleID, Dictionary<string, string> addNode, Dictionary<string, string> deletedNode, Dictionary<string, string> dirTotal)
    {
        ROLEFUNCTIONACTION rfaModel = new ROLEFUNCTIONACTION();

        bool bRun = false;

        try
        {
            dao.OpenConnection();

            dao.ExecuteNonQuery("delete from ROLEFUNCTIONACTION where roleid='" + strRoleID + "'");

            ROLE rModel = dao.GetModel<ROLE, string>("RoleID", strRoleID);
            rModel.RoleName = strName;
            dao.Update<ROLE>(rModel, "RoleID");

            foreach (string actionID in dirTotal.Keys)
            {
               if (actionID.Length >= 5)
                {
                    dirValues.Clear();
                    dirValues.Add("ROLEID", rModel.RoleID);
                    dirValues.Add("FUNCTIONID", dirTotal[actionID].Trim());
                    dirValues.Add("ACTIONID", dirTotal[actionID].Trim() + "1");
                    if (!dao.Contains("select count(*) from ROLEFUNCTIONACTION where ROLEID=@ROLEID AND FUNCTIONID=@FUNCTIONID AND ACTIONID=@ACTIONID", dirValues))
                    {
                        ROLEFUNCTIONACTION rfaModel1 = new ROLEFUNCTIONACTION();
                        rfaModel1.RoleID = rModel.RoleID;
                        rfaModel1.FunctionID = dirTotal[actionID].Trim();
                        rfaModel1.ActionID = dirTotal[actionID].Trim() + "1";
                        dao.Add<ROLEFUNCTIONACTION>(rfaModel1);
                    }
                }
                dirValues.Clear();
                dirValues.Add("ROLEID", rModel.RoleID);
                dirValues.Add("FUNCTIONID", dirTotal[actionID].Trim());
                dirValues.Add("ACTIONID", actionID.Trim());
                if (!dao.Contains("select count(*) from ROLEFUNCTIONACTION where ROLEID=@ROLEID AND FUNCTIONID=@FUNCTIONID AND ACTIONID=@ACTIONID", dirValues))
                {
                    rfaModel.RoleID = rModel.RoleID;
                    rfaModel.FunctionID = dirTotal[actionID].Trim();
                    rfaModel.ActionID = actionID.Trim();
                    dao.Add<ROLEFUNCTIONACTION>(rfaModel);
                }
            }

            

            //foreach (string actionID in addNode.Keys)
            //{
            //    rfaModel.RoleID = strRoleID.Trim();
            //    rfaModel.FunctionID = addNode[actionID].Trim();
            //    rfaModel.ActionID = actionID.Trim();
            //    dao.Add<ROLEFUNCTIONACTION>(rfaModel);
            //    bRun = true;
            //}

            //foreach (string strActionID in deletedNode.Keys)
            //{
            //    dirValues.Clear();
            //    dirValues.Add("RoleID", strRoleID.Trim());
            //    dirValues.Add("ActionID", strActionID.Trim());
            //    dirValues.Add("FunctionID", deletedNode[strActionID].Trim());
            //    dao.Delete("RoleFunctionAction", dirValues);
            //    bRun = true;
            //}

            //操作日誌
            SetOprLog("3");

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
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 取得TreeView的信息
    /// </summary>
    /// <param name="strRoleID">角色編碼</param>
    /// <returns>結果資料集</returns>
    public DataSet GetTreeData(string strRoleID)
    {
        // 參數列表
        dirValues.Clear();
        dirValues.Add("roleid", strRoleID);

        DataSet dstTree = null;
        // 開始查詢
        try
        {
            dstTree = dao.GetList(SEL_FUNACTION_BY_RID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        // 返回資料--------------------------------------------------------------------------------------------
        return dstTree;
    }

    /// <summary>
    /// 獲得群組信息
    /// </summary>
    /// <param name="strRoleID">角色編碼</param>
    /// <returns></returns>
    public ROLE GetModel(string strRoleID)
    {
        ROLE rModel = null;

        try
        {
            rModel = dao.GetModel<ROLE, string>("roleid", strRoleID);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return rModel;
    }

    public DataSet GetTree()
    {
        return dao.GetList("select * from [function] select * from [ACTION]");
    }

}
