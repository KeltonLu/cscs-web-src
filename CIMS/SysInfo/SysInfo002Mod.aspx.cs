//******************************************************************
//*  作    者：FangBao
//*  功能說明：警訊功能 
//*  創建日期：2008-11-26
//*  修改日期：2008-11-26 12:00
//*  修改記錄：
//*            □2008-11-26
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
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using ControlLibrary;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

public partial class SysInfo_SysInfo002Mod :PageBase
{
    SysInfo002BL bl = new SysInfo002BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];
        if (!IsPostBack)
        {
            WARNING_CONFIGURATION wcModel = bl.GetModel(strRID);

            SetControls(wcModel);

            //綁定左邊ListBox
            DataTable dtblUser = bl.GetUsers();
            mlbUser.DataSource = dtblUser;
            mlbUser.DataBind();


            //綁定右邊ListBox
            DataTable dtblUserRight = bl.GetUsersByRID(strRID);
            mlbUser.RightListBox.DataSource = dtblUserRight;
            mlbUser.DataBind();

            switch (int.Parse(strRID))
            {
                case 1: lblWarning_Content.Text = GlobalString.WarningContent.Msg1; break;
                case 2: lblWarning_Content.Text = GlobalString.WarningContent.Msg3; break;
                case 3: lblWarning_Content.Text = GlobalString.WarningContent.Msg4; break;
                //case 4: lblWarning_Content.Text = GlobalString.WarningContent.Msg1;break;
                case 5: lblWarning_Content.Text = GlobalString.WarningContent.Msg2; break;
                case 6: lblWarning_Content.Text = GlobalString.WarningContent.Msg5; break;
                //case 7: lblWarning_Content.Text = GlobalString.WarningContent.Msg1;break;
                case 8: lblWarning_Content.Text = GlobalString.WarningContent.Msg11; break;
                case 9: lblWarning_Content.Text = GlobalString.WarningContent.Msg10; break;
                case 10: lblWarning_Content.Text = GlobalString.WarningContent.Msg9; break;
                case 11: lblWarning_Content.Text = GlobalString.WarningContent.Msg8; break;
                case 12: lblWarning_Content.Text = GlobalString.WarningContent.Msg22; break;
                case 13: lblWarning_Content.Text = GlobalString.WarningContent.Msg24; break;
                case 14: lblWarning_Content.Text = GlobalString.WarningContent.Msg28; break;
                case 15: lblWarning_Content.Text = GlobalString.WarningContent.Msg7; break;
                case 16: lblWarning_Content.Text = GlobalString.WarningContent.Msg31; break;
                case 18: lblWarning_Content.Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 19: lblWarning_Content.Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;

                case 20: lblWarning_Content.Text = GlobalString.WarningContent.Msg31; break;
                //case 21: lblWarning_Content.Text = GlobalString.WarningContent.Msg1;break;
                case 22: lblWarning_Content.Text = GlobalString.WarningContent.Msg30; break;
                case 23: lblWarning_Content.Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 24: lblWarning_Content.Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                case 25: lblWarning_Content.Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 26: lblWarning_Content.Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                case 27: lblWarning_Content.Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 28: lblWarning_Content.Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                case 29: lblWarning_Content.Text = GlobalString.WarningContent.Msg34; break;
                case 30: lblWarning_Content.Text = GlobalString.WarningContent.Msg35; break;
                case 33: lblWarning_Content.Text = GlobalString.WarningContent.Msg36; break;
                case 34: lblWarning_Content.Text = GlobalString.WarningContent.Msg26; break;
                case 35: lblWarning_Content.Text = GlobalString.WarningContent.Msg17; break;
                case 36: lblWarning_Content.Text = GlobalString.WarningContent.Msg18; break;
                case 37: lblWarning_Content.Text = GlobalString.WarningContent.Msg16; break;
                case 38: lblWarning_Content.Text = GlobalString.WarningContent.Msg29; break;
                case 39: lblWarning_Content.Text = GlobalString.WarningContent.Msg17; break;
                case 40: lblWarning_Content.Text = GlobalString.WarningContent.Msg17; break;
                case 41: lblWarning_Content.Text = GlobalString.WarningContent.Msg31; break;
                case 42: lblWarning_Content.Text = GlobalString.WarningContent.Msg31; break;
                case 43: lblWarning_Content.Text = GlobalString.WarningContent.Msg12; break;
                case 44: lblWarning_Content.Text = GlobalString.WarningContent.Msg13; break;
                case 45: lblWarning_Content.Text = GlobalString.WarningContent.Msg12; break;
                case 46: lblWarning_Content.Text = GlobalString.WarningContent.Msg20; break;
                case 47: lblWarning_Content.Text = GlobalString.WarningContent.Msg16; break;
                case 48: lblWarning_Content.Text = GlobalString.WarningContent.Msg12; break;
                case 49: lblWarning_Content.Text = GlobalString.WarningContent.Msg13; break;
                case 50: lblWarning_Content.Text = GlobalString.WarningContent.Msg16; break;
                case 51: lblWarning_Content.Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 52: lblWarning_Content.Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                default: break;
            }
        }
    }
    protected void btnEdit1_Click(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];
        WARNING_CONFIGURATION wcModel = new WARNING_CONFIGURATION();

        try
        {
            SetData(wcModel);
            wcModel.RID = int.Parse(strRID);

            bl.Updata(wcModel, mlbUser);
            ShowMessageAndGoPage("儲存成功", "SysInfo002.aspx");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
}
