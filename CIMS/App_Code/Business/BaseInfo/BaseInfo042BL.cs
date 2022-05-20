//******************************************************************
//*  作    者：FangBao
//*  功能說明：使用者資料維護邏輯
//*  創建日期：2008-07-31
//*  修改日期：2008-07-31 12:00
//*  修改記錄：
//*            □2008-07-31
//*              1.創建 鮑方
//*******************************************************************

using System;
using System.Data;
using System.Data.Common;
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
using Microsoft.Practices.EnterpriseLibrary.Data;

/// <summary>
/// UserManager 的摘要描述
/// </summary>
public class BaseInfo042BL : BaseLogic
{
    #region sql語句
    public const string SEL_ROLE = "SELECT [ROLE].[ROLEID], [ROLE].[ROLENAME] FROM [ROLE] ORDER BY [ROLEID] ASC";
    public const string SEL_USERS = "SELECT * FROM [USERS] WHERE 1>0 ";
    public const string SEL_USERSROLES_BY_UID = "SELECT COUNT(*) FROM USERSROLE WHERE USERID=@userid";
    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo042BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }


    /// <summary>
    /// 從LDAP獲得用戶信息
    /// </summary>
    /// <param name="strUserID">用戶帳號</param>
    public string[] GetUserInfoFromLDAP(string strUserID)
    {
        //************************************************************
        //注意，此處代碼需要替換成LDAP組件調用--
        return new string[] { "測試用戶", "test@wis.com" };
        //---------------------------------------------------------------------------
        //************************************************************
    }

    /// <summary>
    /// 獲得用戶關聯資料
    /// </summary>
    /// <returns></returns>
    public DataSet GetPageDatas()
    {
        DataSet dstPageDatas = null;
        try
        {
            dstPageDatas = dao.GetList(SEL_ROLE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstPageDatas;
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
        string strSortField = (sortField == "null" ? "UserID" : sortField);//默認的排序欄位

        //--2、整理SQL語句-------------------------------------------------------------------------------------
        StringBuilder stbCommand = new StringBuilder(SEL_USERS);

        //--3、整理查詢條件----------------------------------------------------------------------------------
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtUserID"].ToString().Trim()))
            {
                stbWhere.Append(" AND UserID LIKE @userid");
                dirValues.Add("userid", "%" + searchInput["txtUserID"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtUserName"].ToString().Trim()))
            {
                stbWhere.Append(" AND UserName LIKE @username");
                dirValues.Add("username", "%" + searchInput["txtUserName"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["dropRole"].ToString().Trim()))
            {
                stbWhere.Append(" AND EXISTS (SELECT * FROM [USERSROLE] WHERE [ROLEID]=@roleid and [USERS].USERID=[USERSROLE].USERID)");
                dirValues.Add("roleid", searchInput["dropRole"].ToString().Trim());
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
    /// 修改
    /// </summary>
    /// <param name="uModel">資料模型</param>
    /// <param name="mlbRole">ListBox</param>
    public void Update(USERS uModel, MoveListBox mlbRole)
    {
        USERSROLE urModel = new USERSROLE();

        try
        {
            //事務開始
            dao.OpenConnection();

            //更新使用者記錄
            dao.Update<USERS>(uModel, "UserID");

            //for新增已選擇的角色記錄
            string[] AddedItems = mlbRole.GetMastAddValuesArray();
            for (int index = 0; index < AddedItems.Length; index++)
            {
                urModel.UserID = uModel.UserID;
                urModel.RoleID = AddedItems[index].Trim();
                dao.Add<USERSROLE>(urModel);
            }
            //for刪除已選擇的角色記錄
            string s = mlbRole.MastDelValues;
            string[] DeletedItems = mlbRole.GetMastDelValuesArray();
            for (int index = 0; index < DeletedItems.Length; index++)
            {
                dirValues.Clear();
                dirValues.Add("UserID", uModel.UserID);
                dirValues.Add("roleID", DeletedItems[index].Trim());
                dao.Delete("UsersRole", dirValues);
            }

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
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

    public void AddLDAP(DataSet dstUser)
    {
        if (dstUser.Tables.Count!= 2)
            return;

        try
        {
            //事務開始
            dao.OpenConnection();

            if (dstUser.Tables[0].Rows.Count != 0)
            {

                dao.ExecuteNonQuery("delete from [users] where UserID!='admin'");
                dao.ExecuteNonQuery("delete from [USERSROLE] where UserID!='admin'");

                foreach (DataRow drowUser in dstUser.Tables[0].Rows)
                {
                    USERS uModel = new USERS();
                    uModel.UserID = drowUser[0].ToString();
                    uModel.UserName = drowUser[1].ToString();
                    uModel.Email = drowUser[2].ToString();
                    dao.Add<USERS>(uModel);
                }

                foreach (DataRow drowRole in dstUser.Tables[1].Rows)
                {
                    USERSROLE urModel = new USERSROLE();
                    urModel.UserID = drowRole[0].ToString();
                    urModel.RoleID = drowRole[1].ToString();
                    dao.Add<USERSROLE>(urModel);
                }
            }
            else
            {
                throw new AlertException("無添加信息");
            }

            //操作日誌
            SetOprLog();


            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
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
    /// 增加
    /// </summary>
    /// <param name="uModel">資料模型</param>
    /// <param name="mlbRole">ListBox</param>
    public void Add(USERS uModel, MoveListBox mlbRole)
    {
        USERSROLE urModel = new USERSROLE();

        try
        {
            //事務開始
            dao.OpenConnection();

            //新增使用者
            uModel.LastLoginDateTime = DateTime.Now;
            dao.Add<USERS>(uModel);

            //for新增已選擇的角色記錄
            string[] AddedItems = mlbRole.GetMastAddValuesArray();
            for (int index = 0; index < AddedItems.Length; index++)
            {
                urModel.UserID = uModel.UserID;
                urModel.RoleID = AddedItems[index].Trim();
                dao.Add<USERSROLE>(urModel);
            }

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
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
    /// 獲得用戶
    /// </summary>
    /// <param name="strUserID">用戶編號</param>
    /// <returns></returns>
    public USERS GetUser(string strUserID)
    {
        USERS uModel = null;
        try
        {
            uModel = dao.GetModel<USERS, string>("UserID", strUserID);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return uModel;
    }


    /// <summary>
    /// 判斷某用戶是否存在於資料庫中
    /// </summary>
    /// <param name="strUserID">用戶編號</param>
    public bool ContainsUser(string strUserID)
    {
        return dao.Contains<USERS, string>("UserID", strUserID);
    }

    /// <summary>
    /// 刪除用戶信息
    /// </summary>
    /// <param name="strUserID">用戶編號</param>
    public void Delete(string strUserID)
    {
        try
        {
            //驗證是否可以刪除
            dirValues.Clear();
            dirValues.Add("userid", strUserID);
            if ((int)dao.GetList(SEL_USERSROLES_BY_UID, dirValues).Tables[0].Rows[0][0] > 0)
                throw new AlertException(BizMessage.BizMsg.ALT_BASEINFO_042_01);

            dao.Delete("Users", dirValues);

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
}
