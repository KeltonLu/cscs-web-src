// 創建者：Judy
// 功能說明:帳號解鎖功能
// 創建時間：2018/03/21
// 修改時間：
// 修改者：
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BaseInfo_BaseInfo050 : System.Web.UI.Page
{
    private BaseInfo050BL baseInfo050BL = new BaseInfo050BL();

    /// <summary>
    /// 頁面加載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {

        }
    }

    /// <summary>
    /// 解鎖功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeblocking_Click(object sender, EventArgs e)
    {
        string strUserId = txtUsersID.Text.Trim();

        string strUserIdDB = baseInfo050BL.SelectUserId(txtUsersID.Text.Trim());

        // 若文本框值為空 add by judy 2018/03/28
        if (strUserId == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('請輸入欲解鎖帳號!');", true);

            return;
        }

        // 若文本框值在『USERS』檔中不存在 add by judy 2018/03/28
        if (strUserId != "" && strUserId!= strUserIdDB)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('帳號不存在!');", true);

            return;
        }

        // 登錄帳號
        string strLoginUserId = ((USERS)Session[GlobalString.SessionAndCookieKeys.USER]).UserID;

        // 如果解鎖帳號為當前登錄人員帳號
        if (strUserId != "" && strUserId == strLoginUserId)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('只可為他人帳號解鎖!');", true);
        }
        else // 將USERS檔中錯誤次數ErrorNum清空
        {
            baseInfo050BL.UpdateUserErrorNum(strUserId);

            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('帳號解鎖成功!');", true);
        }      
    }
}