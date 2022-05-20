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

public partial class Finance_Finance004_4Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string date = Request.QueryString["time"];          
            //設置報表參數
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = "/CIMSReport/Finance004_4";

            ReportParameter[] reportParam = new ReportParameter[1];
            reportParam[0] = new ReportParameter("TimeMark", date, false);

            ReportViewer1.ServerReport.SetParameters(reportParam);
            //ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
