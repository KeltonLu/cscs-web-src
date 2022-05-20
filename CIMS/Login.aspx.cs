//******************************************************************
//*  作    者：FangBao
//*  功能說明：用戶登錄頁面
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 鮑方
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;


public partial class Login : System.Web.UI.Page
{
    #region 自訂變數
    private LoginManager lmLoginManager = new LoginManager();//登錄管理
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lmLoginManager.DoLoginOut();//只要開啓本頁面就會被登出
            string strUserID = lmLoginManager.GetUserIDHistory();//獲得以前用來登錄的帳號
            if (strUserID != null)
            {
                this.txtID.Text = strUserID;
            }
        }

        

        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (Session[GlobalString.SessionAndCookieKeys.OUT] != null)//已經被踢出
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), string.Concat("alert('", GlobalStringManager.Default["Alert_UserAcountISUseing"], "');"), true);

        }
    }

    /// <returns>-1:LADP連接失敗 -2:無此用戶名 -3:密碼錯誤 -4:LADP異常錯誤 0:正常</returns>
    protected void ibtnLogin_Click(object sender, ImageClickEventArgs e)
    {
        string strMsg = "";
        
        int liLoginInfo = lmLoginManager.UserLogin(txtID.Text.Trim(), txtPwd.Text.Trim(), ref strMsg);//登錄

        string strUserId = txtID.Text.Trim();

        // 非正常登錄,錯誤次數ErrorNum+1  Add by judy 2018/03/21
        if (liLoginInfo != 0)
        {
            lmLoginManager.UpdateErrorNum(strUserId);
        }

        // 登錄錯誤次數  Add by judy 2018/03/21
        int liLoginErrorNum = Convert.ToInt32(lmLoginManager.SelectErrorNums(strUserId));

        // 取Config LoginErrorNum Add by judy 2018/03/21
        string strLoginErrorNum = ConfigurationManager.AppSettings["LoginErrorNum"].ToString();

        // users表的錯誤次數ErrorNum超過設定值(Config中設定節點 LoginErrorNum) Add by judy 2018/03/21
        if (liLoginErrorNum > Convert.ToInt32(strLoginErrorNum))
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('您已錯誤登錄超過" + strLoginErrorNum + "次, 帳號已鎖定, 請解鎖後再登入!');", true);
                   
            return;
        }

        //正常登錄，USERS檔中”錯誤次數ErrorNum”>0 Add by judy 2018/03/28
        if (liLoginInfo == 0 && liLoginErrorNum > 0)
        {
            lmLoginManager.UpdateUserErrorNums(strUserId);
        }

        switch (liLoginInfo)
        {
            case 0://登錄成功
                HttpContext.Current.Response.Redirect("MainFrame.aspx");
                break;
            case -1://LADP連接失敗
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('LADP連接失敗');", true);
                break;
            case -2://LADP無此用戶名
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('無此用戶名或帳號已鎖定');", true);
                break;
            case -3://密碼錯誤
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('密碼錯誤');", true);
                break;
            case -4://LADP異常錯誤
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('LADP異常錯誤：'" + strMsg + "');", true);
                break;
            case -5://系統無此用戶名
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('系統無此用戶名');", true);
                break;
            case -6://如果操作人無權限
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('該操作人無權限');", true);
                break;
            case -7://獲取用戶權限失敗
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('獲取用戶權限失敗');", true);
                break;
            default://其它錯誤
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('其他錯誤');", true);
                break;
        }
    }
}
