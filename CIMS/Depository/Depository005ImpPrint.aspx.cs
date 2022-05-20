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
public partial class Depository_Depository005ImpPrint : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];

            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Depository005_01";


            ReportParameter[] reportParam = new ReportParameter[1];
            reportParam[0] = new ReportParameter("RID", strRID, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
