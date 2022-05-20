//******************************************************************
//*  作    者：lantaosu
//*  功能說明：換卡預測月報表 
//*  創建日期：2008-11-27
//*  修改日期：2008-11-27 18:00
//*  修改記錄：
//*            □2008-11-27
//*              1.創建 蘇斕濤
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
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;


public partial class Report_Report024 : PageBase
{
    Report024BL bl = new Report024BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //初始化頁面
            DataSet dsFactory = bl.GetCooperatePersoList();
            dropFactoryRID.DataSource = dsFactory;
            dropFactoryRID.DataValueField = "RID";
            dropFactoryRID.DataTextField = "Factory_ShortName_CN";
            dropFactoryRID.DataBind();

            dropYear.SelectedValue = DateTime.Now.Year.ToString();
            dropMonth.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');

            ListItem li = new ListItem("全部", "all");
            dropFactoryRID.Items.Insert(0, li);
            dropFactoryRID.SelectedValue = "all";

            ReportView1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report024";

        }
    }

    private void GenReport1()
    {

        this.ReportView1.Visible = true;


        //處理參數
        DataSet ds = null;
        string date = dropYear.SelectedValue + dropMonth.SelectedValue;
        string strRID = dropFactoryRID.SelectedValue;
        string year = date.Substring(0, 4);
        string month = date.Substring(4, 2);
        string month1 = month;
        string year1 = year;
        //換卡時間 = 畫面輸入的年月 + 1
        month = Convert.ToString(Convert.ToInt32(month));
        if (Convert.ToInt32(month) < 10)
            date = year + "0" + month;
        else if (Convert.ToInt32(month) == 13)
            date = Convert.ToString(Convert.ToInt32(year) + 1) + "01";
        else
            date = year + month;

        string[] RID = null;

        if (strRID == "all")
        {
            strRID = "";
            ds = bl.GetCooperatePersoList();
            RID = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                RID[i] = ds.Tables[0].Rows[i]["RID"].ToString();
        }
        else
        {
            RID = new string[1];
            RID[0] = strRID;
        }
      

        //參數
        ReportParameter[] reportParam = new ReportParameter[4];
        reportParam[0] = new ReportParameter("date", date, false);
        reportParam[1] = new ReportParameter("RID", RID, false);
        reportParam[2] = new ReportParameter("month", month1, false);
        reportParam[3] = new ReportParameter("year", year1, false);

        ReportView1.ServerReport.SetParameters(reportParam);
        ReportView1.ShowParameterPrompts = false;
    }

    protected void btnExport_ServerClick(object sender, EventArgs e)
    {
        string date = "";
        string year = dropYear.SelectedValue;
        string month = dropMonth.SelectedValue;
        //換卡時間 = 畫面輸入的年月 + 1
        month = Convert.ToString(Convert.ToInt32(month));
        if (Convert.ToInt32(month) < 10)
            date = year + "0" + month;
        else if (Convert.ToInt32(month) == 13)
            date = Convert.ToString(Convert.ToInt32(year) + 1) + "01";
        else
            date = year + month;

        if (!bl.IsImport(date))
        {
            ShowMessage(date+"未匯入次月換卡預測表");
            return;
        }

        GenReport1();
        //ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "exportExcel();", true);
    }
}
