//******************************************************************
//*  作    者：QingChen
//*  功能說明：登錄管理邏輯程式
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
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
/// 登錄邏輯管理
/// </summary>
public class LoginManager : BaseLogic
{
    // DB參數變量
    private Dictionary<string, object> dirValues = new Dictionary<string, object>();

    /// <summary>
    /// 建構
    /// </summary>
    public LoginManager()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 用戶登錄驗証
    /// </summary>
    /// <param name="userID">用戶帳號</param>
    /// <param name="password">密碼</param>
    /// <param name="strMsg">返回錯誤訊息</param>
    /// <returns>
    /// 0: 登錄成功
    /// </returns>
    public int UserLogin(string userID, string password, ref string strMsg)
    {
        int iRtn = 0;
        USERS userNowUser = new USERS();

        List<string> lstActions = null;
        string strRoles = "";

        if (userID == string.Empty) //此用戶不存在
        {
            return -2;
        }

        if (password == string.Empty) //密碼未輸入, 提示密碼不正確   Legend 2018/01/18 添加
        {
            return -3;
        }

        string strIsLDAP = ConfigurationManager.AppSettings["IsUsingLDAP"].ToString();


        if (strIsLDAP == "1")
        {
            //從LDAP驗証用戶
            iRtn = LADPCheckManager.LDAPLogin(userID, password, ref userNowUser, ref strRoles, ref strMsg);
            if (iRtn != 0) return iRtn;

            if (StringUtil.IsEmpty(strRoles))
                return -6;

            strRoles = strRoles.Substring(0, strRoles.Length - 1);
        }

        //獲取角色信息
        if (strIsLDAP == "1")
        {
            //獲取用戶之權限記錄
            try
            {
                lstActions = UserFunctionProvider.GetFunctions(strRoles); //獲得權限
                if (lstActions.Count == 0)//如果操作人無權限
                {
                    return -6;
                }
            }
            catch
            {
                return -7; //獲取用戶權限失敗
            }
        }
        else
        {
            //查找用戶是否存在於資料庫中
            try
            {
                userNowUser = dao.GetModel<USERS, String>("UserID", userID);//從資料庫獲得用戶物件
                userNowUser.LastLoginDateTime = DateTime.Now;
                //dao.Update<USERS>(userNowUser, "UserID");
            }
            catch
            {
                return -5; //查詢用戶失敗
            }

            //獲取用戶之權限記錄
            try
            {
                lstActions = UserFunctionProvider.GetFunctions(userNowUser); //獲得權限
                if (lstActions.Count == 0)//如果操作人無權限
                {
                    return -6;
                }
            }
            catch
            {
                return -7; //獲取用戶權限失敗
            }
        }

        //將用戶信息，權限記錄保存於Seesion中
        HttpContext.Current.Session.Add(GlobalString.SessionAndCookieKeys.USER, userNowUser);
        HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.ACTIONS] = lstActions;

        HttpCookie userCookie = new HttpCookie(GlobalString.SessionAndCookieKeys.USER_ID, userID);//紀錄帳號，以後登錄自動填寫
        userCookie.Expires = DateTime.Now.AddMonths(1);
        HttpContext.Current.Response.Cookies.Add(userCookie);

        return iRtn;
    }

    /// <summary>
    /// 判斷指定角色編號是否和當前登陸用戶對應
    /// </summary>
    /// <param name="strUserID">當前登陸用戶編號</param>
    /// <param name="strJS">角色編號</param>
    /// <returns></returns>
    public bool IsContainsRole(string strUserID,string strJS)
    {
        bool isresult = false;

        DataSet ds = dao.GetList("select * from USERSROLE where userid='" + strUserID + "' and roleid='" + strJS + "'");
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            isresult = true;
        }
        else
        {
            isresult = false;
        }
        return isresult;
    }

    /// <summary>
    /// 檢驗本帳號是否被使用
    /// </summary>
    /// <param name="userID">用戶編號</param>
    public bool LookAppUsers(string userID)
    {
        return HttpContext.Current.Application[userID] != null;
    }

    /// <summary>
    /// 登出
    /// </summary>
    public void DoLoginOut()
    {
        if (HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER] != null)
        {
            HttpContext.Current.Session.Clear();
        }
    }

    /// <summary>
    /// 獲得用戶編號
    /// </summary>
    /// <returns>用戶編號</returns>
    public string GetUserIDHistory()
    {
        if (HttpContext.Current.Request.Cookies[GlobalString.SessionAndCookieKeys.USER_ID] != null)
        {
            return HttpContext.Current.Request.Cookies[GlobalString.SessionAndCookieKeys.USER_ID].Value;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 更新『USERS』檔中”錯誤次數ErrorNum”+1 
    /// Add by Judy 2018/03/20
    /// </summary>
    /// <param name="userId">當前登錄人帳號</param>
    /// <returns></returns>
    public int UpdateErrorNum(string userId)
    {
        this.dirValues.Clear();
        this.dirValues.Add("UserID", userId);

        return dao.ExecuteNonQuery("update USERS set ErrorNum = isnull(ErrorNum, 0) + 1 where UserID=@UserID", this.dirValues);
    }

    /// <summary>
    /// 查詢『USERS』檔中錯誤次數ErrorNum
    /// Add by judy 2018/03/20
    /// </summary>
    /// <param name="userId">當前登入帳號</param>
    /// <returns></returns>
    public string SelectErrorNums(string userId)
    {
        this.dirValues.Clear();
        this.dirValues.Add("UserID", userId);

        DataSet dsErrorNum = dao.GetList("select isnull(ErrorNum, 0) as ErrorNum from USERS where UserID=@UserID", this.dirValues);
       
        if (dsErrorNum != null && dsErrorNum.Tables[0].Rows.Count > 0)
        {
            return dsErrorNum.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "0";
        }        
    }

    /// <summary>
    ///更新USERS. ErrorNum為0
    /// Add by judy 2018/03/28
    /// </summary>
    /// <param name="userId">當前登錄人帳號</param>
    /// <returns></returns>
    public int UpdateUserErrorNums(string userId)
    {
        this.dirValues.Clear();
        this.dirValues.Add("UserID", userId);

        return dao.ExecuteNonQuery("update USERS set ErrorNum = 0 where UserID=@UserID", this.dirValues);
    }
}
