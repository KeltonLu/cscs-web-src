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

public partial class SysInfo_SysInfo002 : PageBase
{
    SysInfo002BL bl = new SysInfo002BL();


    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbWarning.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            gvpbWarning.BindData();
        }

    }
    protected void gvpbWarning_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();

        DataSet dstlWarning = null;

        try
        {
            dstlWarning = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlWarning != null)//如果查到了資料
            {
                e.Table = dstlWarning.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

    }
    protected void gvpbWarning_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        DataTable dtblWarning = (DataTable)gvpbWarning.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblWarning.Rows.Count == 0)
                return;

            Label lblWarnType = (Label)e.Row.FindControl("lblWarnType");
            Label lblUsers = (Label)e.Row.FindControl("lblUsers");

            if (dtblWarning.Rows[e.Row.RowIndex]["Mail_Show"].ToString() == "Y")
                lblWarnType.Text += "mail";

            if (dtblWarning.Rows[e.Row.RowIndex]["System_Show"].ToString() == "Y")
                lblWarnType.Text += "系統";

           // e.Row.Cells[2].Text = e.Row.Cells[2].Text.Replace("!&lt;br&gt;", "<br>");

            DataTable dtblUser = bl.GetUsersByRID(dtblWarning.Rows[e.Row.RowIndex]["RID"].ToString());
            foreach (DataRow drowUser in dtblUser.Rows)
            {
                lblUsers.Text += drowUser["USERNAME"].ToString() + "<br>";
            }

            switch (int.Parse(gvpbWarning.DataKeys[e.Row.RowIndex].Value.ToString()))
            {
                case 1: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg1; break;
                case 2: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg3;break;
                case 3: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg4;break;
                //case 4: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg1;break;
                case 5: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg2;break;
                case 6: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg5;break;
                //case 7: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg1;break;
                case 8: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg11;break;
                case 9: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg10;break;
                case 10: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg9;break;
                case 11: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg8;break;
                case 12: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg22;break;
                case 13: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg24;break;
                case 14: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg28;break;
                case 15: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg7;break;
                case 16: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg31;break;
                case 18: e.Row.Cells[2].Text=GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 19: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                
                case 20: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg31;break;
                //case 21: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg1;break;
                case 22: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg30;break;
                case 23: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32;break;
                case 24: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                case 25: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 26: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                case 27: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 28: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                case 29: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg34;break;
                case 30: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg35; break;
                case 33: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg36; break;
                case 34: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg26; break;
                case 35: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg17; break;
                case 36: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg18; break;
                case 37: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg16; break;
                case 38: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg29;break;
                case 39: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg17; break;
                case 40: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg17; break;
                case 41: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg31;break;
                case 42: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg31;break;
                case 43: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg12;break;
                case 44: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg13;break;
                case 45: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg12;break;
                case 46: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg20;break;
                case 47: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg16;break;
                case 48: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg12;break;
                case 49: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg13;break;
                case 50: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg16;break;
                case 51: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg3 + "<br>" + GlobalString.WarningContent.Msg4 + "<br>" + GlobalString.WarningContent.Msg32; break;
                case 52: e.Row.Cells[2].Text = GlobalString.WarningContent.Msg5 + "<br>" + GlobalString.WarningContent.Msg33; break;
                default: break;
            }

        }
    }
}
