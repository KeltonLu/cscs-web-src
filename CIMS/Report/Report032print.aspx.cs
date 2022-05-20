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

public partial class Report_Report032print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //string TimeNow = Request.QueryString["TimeNow"];
            //string perso_facotry = Request.QueryString["perso_facotry"];
            //string material_type = Request.QueryString["material_type"];
            //string materialrid = Request.QueryString["materialrid"];
            //string diff = Request.QueryString["diff"];

            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report032";

            //ReportParameter[] reportParam = new ReportParameter[0];
            //reportParam[0] = new ReportParameter("perso_facotry", perso_facotry, false);
            //reportParam[1] = new ReportParameter("material_type", material_type, false);
            //reportParam[2] = new ReportParameter("materialrid", materialrid, false);
            //reportParam[3] = new ReportParameter("diff", diff, false);
            //reportParam[4] = new ReportParameter("TimeNow", TimeNow, false);
            //ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
