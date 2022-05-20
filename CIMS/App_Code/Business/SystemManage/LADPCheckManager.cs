using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.ctcb.ldap;
using System.Text.RegularExpressions;

/// <summary>
/// LADPCheckManager 的摘要描述
/// </summary>
public class LADPCheckManager
{
    #region 全局變量

    private static LogOperator _logOperator = new LogOperator();

    #endregion

    public LADPCheckManager()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //   
    }

    /// <summary>
    /// 獲取LDAP信息
    /// </summary>
    /// <returns></returns>
    public static DataSet GetLDAPAuth()
    {
        DataSet dstUser = new DataSet();

        DataTable dtblUserInfo = new DataTable();
        dtblUserInfo.Columns.Add("UserID");
        dtblUserInfo.Columns.Add("UserName");
        dtblUserInfo.Columns.Add("Email");

        DataTable dtblUserRole = new DataTable();
        dtblUserRole.Columns.Add("UserID");
        dtblUserRole.Columns.Add("RoleID");


        //LDAP的IP
        string ip = ConfigurationManager.AppSettings["LDAP_IP"].ToString();
        //LDAP的端口號
        short port = Convert.ToInt16(ConfigurationManager.AppSettings["LDAP_PORT"].ToString());


        //rootBaseDN是指會使用此AP的部門單位的DN，如:中國信託商業銀行或僅是個金或法金
        string rootBaseDN = ConfigurationManager.AppSettings["LDAP_RootBaseDN"].ToString();

        //serviceID(就是AP註冊在LDAP的物件)的DN與密碼
        string sidDN = ConfigurationManager.AppSettings["LDAP_SIDDN"].ToString();
        string sidPass = ConfigurationManager.AppSettings["LDAP_SIDPass"].ToString();

        //****步驟一：ServiceID連線*****************************************
        LdapAdvance ladv = new LdapAdvance();

        try
        {
            ladv.bind(ip, port, sidDN, sidPass);
            // 記錄Log
            _logOperator.Write("LDAP已連線");
        }
        catch
        {
            //ldap連接失敗
            throw new Exception("LDAP連接失敗");
        }

        //查詢rootBaseDN下的所有物件
        try
        {
            //獲取AP的角色列表
            string[] alRoles = ladv.getRolesByApp(sidDN);
            // 記錄Log
            _logOperator.Write("全部角色:" + alRoles.ToString());
            for (int n = 0; n < alRoles.Length; n++)
            {
                if (alRoles[n] == null || alRoles[n] == "") continue;

                // 舊版LDAP有用, 新版不再使用
                //string Pattern = @"[^=]\w+[^,]";

                //MatchCollection Matches = Regex.Matches(alRoles[n], Pattern, RegexOptions.IgnoreCase);
                //if (Matches.Count < 2) return null;
                //string strRoleID = Matches[1].ToString();

                string strRoleID = alRoles[n].ToString();

                // 記錄Log
                _logOperator.Write("單個角色:" + strRoleID);

                //獲取角色下的用戶
                string[] alMembers = ladv.getRoleUsers(alRoles[n]);
                for (int i = 0; i < alMembers.Length; i++)
                {
                    if (alMembers[i] == null || alMembers[i] == "") continue;
                    // UserID/UserName/Email 
                    Attributes alCN_FullName_Mail = ladv.getAttributes(alMembers[i], new string[] { "CN", "fullname", "mail" });

                    // 記錄Log
                    _logOperator.Write("LDAP的getAttributes返回的屬性:" + alCN_FullName_Mail.ToString() + "; LDAP的getAttributes返回的屬性名稱:" + alCN_FullName_Mail.getAttributeNames().ToString());

                    #region 獲取用戶相關資料
                    string alCN = "";
                    string alFullName = "";
                    string alMail = "";

                    //獲取登錄用戶資料
                    alCN = GetAttibuteValue(alCN_FullName_Mail, "CN");
                    alFullName = GetAttibuteValue(alCN_FullName_Mail, "fullName");
                    alMail = GetAttibuteValue(alCN_FullName_Mail, "mail");

                    // 記錄Log
                    _logOperator.Write("用戶CN:" + alCN + "; 用戶fullName:" + alFullName + "; 用戶mail:" + alMail);
                    #endregion 

                    //請在這里添加同步資料庫的代碼。。
                    DataRow drowUserInfo = dtblUserInfo.NewRow();
                    if (alCN != null && alCN != "")
                    {
                        if (dtblUserInfo.Select("UserID='" + alCN + "'").Length == 0)
                        {
                            drowUserInfo[0] = alCN;

                            if (alFullName != null && alFullName != "")
                                drowUserInfo[1] = alFullName;
                            if (alMail != null && alMail != "")
                                drowUserInfo[2] = alMail.ToString();
                            //存用戶信息
                            dtblUserInfo.Rows.Add(drowUserInfo);
                        }

                        //存角色信息
                        if (dtblUserRole.Select("UserID='" + alCN + "' and RoleID='" + strRoleID + "'").Length == 0)
                        {
                            DataRow drowUserRole = dtblUserRole.NewRow();
                            drowUserRole[0] = alCN;
                            drowUserRole[1] = strRoleID;
                            dtblUserRole.Rows.Add(drowUserRole);
                        }
                    }
                }
            }

            dstUser.Tables.Add(dtblUserInfo);
            dstUser.Tables.Add(dtblUserRole);


        }
        catch (Exception ex)
        {
            // 記錄Log
            _logOperator.Write("錯誤訊息:" + ex.Message);
            throw ex;
        }
        finally
        {  
            try
            {
                ladv.unbind();
            }
            catch (Exception ex)
            {
                _logOperator.Write("錯誤訊息:" + ex.Message);
            }
            // 記錄Log
            _logOperator.Write("處理完畢, 斷開連線");
        }

        return dstUser;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName">用戶ID</param>
    /// <param name="password">用戶密碼</param>
    /// <param name="uModel">用戶模型</param>
    /// <param name="strRoles">用戶角色</param>
    /// <param name="strMsg">錯誤訊息</param>
    /// <returns>-1:LADP連接失敗 -2:無此用戶名 -3:密碼錯誤 -4:其他錯誤 -6:無角色信息 0:正常</returns>
    public static int LDAPLogin(string userName, string password, ref USERS uModel, ref string strRoles, ref string strMsg)
    {
        uModel.UserID = userName;

        //LDAP的IP
        string ip = ConfigurationManager.AppSettings["LDAP_IP"].ToString();
        //LDAP的端口號
        short port = Convert.ToInt16(ConfigurationManager.AppSettings["LDAP_PORT"].ToString());


        //rootBaseDN是指會使用此AP的部門單位的DN，如:中國信託商業銀行或僅是個金或法金
        string rootBaseDN = ConfigurationManager.AppSettings["LDAP_RootBaseDN"].ToString();

        //serviceID(就是AP註冊在LDAP的物件)的DN與密碼
        string sidDN = ConfigurationManager.AppSettings["LDAP_SIDDN"].ToString();
        string sidPass = ConfigurationManager.AppSettings["LDAP_SIDPass"].ToString();

        // 返回值   // Legend 2017/12/18 將返回值由"0"改為"999"
        int intReturn = 999;

        //****步驟一：ServiceID連線*****************************************
        LdapBasic lc = new LdapBasic();

        try
        {
            lc.bind(ip, port, sidDN, sidPass);
            // 記錄Log
            _logOperator.Write("LDAP已連線");
        }
        catch
        {
            //ldap連接失敗
            intReturn = -1;
            return intReturn;
        }

        try
        {
            string loginStatus = null;
            // 0:Success;  1:Login failed;  2:Account Disabled or Intruder locked;  3:Exception Message   
            loginStatus = lc.verifyUserPwd(userName, password);
            // 記錄Log
            _logOperator.Write("loginStatus: " + loginStatus + "    0:Success;  1:Login failed;  2:Account Disabled or Intruder locked;  3:Exception Message");

            //判斷為0時, 登陸成功
            if (loginStatus != "" && loginStatus.Split(':')[0].ToString() == "0")
            {
                try
                {
                    // Legend 2017/12/18 添加獲得返回值
                    intReturn = Convert.ToInt32(loginStatus.Split(':')[0].ToString());

                    Attributes h = lc.getUserProperty(userName, new string[] { "fullname", "mail" });

                    // 記錄Log
                    _logOperator.Write("LDAP的getUserProperty返回的屬性:" + h.ToString() + "; LDAP的getUserProperty返回的屬性名稱:" + h.getAttributeNames().ToString());

                    //獲取登錄用戶資料
                    uModel.UserName = GetAttibuteValue(h, "fullName");
                    uModel.Email = GetAttibuteValue(h, "mail");

                    // 記錄Log
                    _logOperator.Write("登陸用戶姓名:" + uModel.UserName + ";登陸用戶Email:" + uModel.Email);
                }
                catch (Exception ex)
                {
                    intReturn = -4;
                    strMsg += ex.Message;
                    // 記錄Log
                    _logOperator.Write("LDAP登入成功, 但獲得LDAP User 屬性值報錯: " + ex.Message);
                }

                try
                {
                    _logOperator.Write("LDAP登入成功, UserName: " + userName);

                    string[] alRoles = lc.getUserRolesByAPP(userName, "CSCS0001");

                    _logOperator.Write("LDAP登入成功, alRoles.Length:" + alRoles.Length.ToString());

                    int i = 0;
                    for (i = 0; i < alRoles.Length; i++)
                    {
                        if (alRoles[i] == null || alRoles[i] == "") continue;

                        /*
                                                string Pattern = @"[^=]\w+[^,]";
                                                MatchCollection Matches = Regex.Matches(alRoles[i], Pattern, RegexOptions.IgnoreCase);

                                            _logOperator.Write("LDAP登入成功, Matches.Count (" + i.ToString() + "," + alRoles[i] + "): " + Matches.Count.ToString());

                                                if (Matches.Count < 2)
                                                {
                                                    //獲取角色出錯
                                                    intReturn = -6;
                                                }
                                                strRoles += "'" + Matches[1].ToString() + "',";
                        */

                        strRoles += "'" + alRoles[i].ToString() + "',";

                        _logOperator.Write("LDAP登入成功,strRoles: " + strRoles);
                    }
                }
                catch (Exception ex)
                {
                    //獲取角色出錯
                    intReturn = -6;
                    // 記錄Log
                    _logOperator.Write("LDAP登入成功, 但獲得LDAP User 角色列表報錯: " + ex.Message);
                }

                //lc.LDAPLogout();
                //lc = null;
            }
            else
            {
                if (loginStatus != "")
                {
                    string strValue1 = loginStatus.Split(':')[0].ToString();
                    string strValue2 = loginStatus.Split(':')[1].ToString();

                    // 記錄Log
                    _logOperator.Write("返回值冒號前值: " + strValue1);
                    _logOperator.Write("返回值冒號后值: " + strValue2);

                    if (strValue1 == "1")
                    {
                        // 密碼不正確, 登陸失敗
                        intReturn = -3;
                    }
                    else if (strValue1 == "2")
                    {
                        // 無此用戶名或帳號已鎖定
                        intReturn = -2;
                    }
                    else
                    {
                        // LDAP異常錯誤
                        intReturn = -4;
                        strMsg = strValue2;
                    }
                }
                else
                {
                    // 登陸失敗
                    intReturn = -8;
                }
            }
        }
        catch (Exception ex)
        {
            // 記錄Log
            _logOperator.Write("錯誤訊息:" + ex.Message);
        }
        finally
        {
            try
            {
                lc.unbind();
            }
            catch (Exception ex)
            {
                _logOperator.Write("錯誤訊息:" + ex.Message);
            }
            // 記錄Log
            _logOperator.Write("處理完畢, 斷開連線");
        }
        // 記錄Log
        _logOperator.Write("Login畫面獲得的返回值: " + intReturn);
        return intReturn;
    }

    /// <summary>
    /// 獲得屬性值
    /// </summary>
    /// <param name="attrValues">從LDAP獲得Attributes</param>
    /// <param name="colName">用戶欄位名稱</param>
    /// <returns>對應欄位值</returns>
    public static string GetAttibuteValue(Attributes attrValues, string colName)
    {
        string strReturn = "";

        Attr attrAttribute = attrValues.getAttribute(colName);
        string[] values1 = attrAttribute.getAttrValues();
        if (values1 != null && values1.Length > 0)
            strReturn = values1[0];

        return strReturn;
    }
}
