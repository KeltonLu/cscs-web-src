//*****************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改日期：2021-03-12
//*  修改記錄：新增次月下市預測表匯入 陳永銘
//*****************************************
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
using Microsoft.Reporting.WebForms;

public partial class InOut_InOut008Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string time = Request.QueryString["time"];
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "InOut008";

            ReportParameter[] reportParam = new ReportParameter[1];//參數
            reportParam[0] = new ReportParameter("RCT", time);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
