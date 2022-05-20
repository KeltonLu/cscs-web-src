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

public partial class Finance_Finance004_3Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //報表參數
            string date = Request.QueryString["date"];            
            string Group_RID = Request.QueryString["Group_RID"];
            string time = Request.QueryString["time"];

            //設置報表
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Finance004_3";

            ReportParameter[] reportParam = new ReportParameter[3];
            reportParam[0] = new ReportParameter("date", date, false);
            reportParam[1] = new ReportParameter("Group_RID", Group_RID, false);
            reportParam[2] = new ReportParameter("RCT", time, false);
            
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
