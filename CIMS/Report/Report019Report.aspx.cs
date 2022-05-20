using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.Reporting.WebForms;

public partial class Report_Report019Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string time = Request.QueryString["time"]; 

            //設置報表控件屬性
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report019";
            Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[1];
            Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("TimeMark", time);
            
            //ReportViewer1.ShowParameterPrompts = false;
            this.ReportViewer1.ServerReport.SetParameters(Paras);
        }
    }    
}
