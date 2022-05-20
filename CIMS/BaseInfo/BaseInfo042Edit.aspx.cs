//******************************************************************
//*  作    者：FangBao
//*  功能說明：使用者資料維護的新增和修改頁面
//*  創建日期：2008/07/31
//*  修改日期：2008/07/31  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 鮑方
//*******************************************************************
using System;
using System.Globalization;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class BaseInfo_BaseInfo042Edit : PageBase
{
    BaseInfo042BL umManager = new BaseInfo042BL();//用戶管理邏輯
    BaseInfo041BL rmManager = new BaseInfo041BL();//角色管理邏輯

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtUserID.Focus();

            string srtQueryID = Request.QueryString[GlobalString.PageUrl.ID];//請求編輯的用戶編號

            //綁定角色LISTBOX
            DataSet dstlRole = null;
            dstlRole = rmManager.SearchRoleData();
            if (dstlRole != null)
            {
                mlbRole.DataSource = dstlRole.Tables[0];
                mlbRole.DataBind();
            }

            if (srtQueryID != null && srtQueryID != "")//如果是編輯行爲
            {
                ajvUserID.Enabled = false;

                USERS uModel = umManager.GetUser(srtQueryID);//獲取要編輯的用戶
                SetControls(uModel);  //綁定控件

                txtUserID.Enabled = false;

                //綁定選中LISTBOX
                DataSet dstlRole1 = null;
                dstlRole1 = rmManager.SearchRole(uModel.UserID);
                if (dstlRole1 != null)
                {
                    mlbRole.RightListBox.DataSource = (dstlRole1.Tables[0]);
                    mlbRole.DataBind();
                }
            }
        }
    }

    
    /// <summary>
    /// 裝載LDAP
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnLoad_Click(object sender, EventArgs e)
    {
        string[] LDAPUser = umManager.GetUserInfoFromLDAP(this.txtUserID.Text.Trim());//從LDAP獲得用戶信息
        this.txtUserName.Text = LDAPUser[0];
        this.txtEMail.Text = LDAPUser[1];
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //1、整理User物件-----------------------------------------------------------------
        USERS uModel = new USERS();

        //2、綁定頁面控件
        SetData(uModel);

        try
        {
            string strQueryID = Request.QueryString[GlobalString.PageUrl.ID];//請求編輯的用戶編號

            //3、判斷新增還是修改
            if (StringUtil.IsEmpty(strQueryID))         //新增
            {
                umManager.Add(uModel, mlbRole);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo042Edit.aspx");
            }
            else                             //修改
            {
                umManager.Update(uModel, mlbRole);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo042.aspx?Con=1");
            }
        }
        catch (Exception ex)
        {
            //異常處理
            ShowMessage(ex.Message);
        }
    }
    #endregion

    #region 欄位/資料補充說明
    /// <summary>
    /// AJAX驗證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AjaxValidator_UserID_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        e.IsAllowSubmit = !umManager.ContainsUser(e.QueryData.Trim());//驗證用戶是否已存在於資料庫
    }
    #endregion

}
