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

public partial class Depository_Depository009Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //取得報表參數
            string strYear = Request.QueryString["year"];
            string used1 = Request.QueryString["used1"];
            string used2 = Request.QueryString["used2"];
            string used3 = Request.QueryString["used3"];
            string used4 = Request.QueryString["used4"];
            string used5 = Request.QueryString["used5"];
            string used6 = Request.QueryString["used6"];
            string used7 = Request.QueryString["used7"];
            string used8 = Request.QueryString["used8"];
            string used9 = Request.QueryString["used9"];
            string used10 = Request.QueryString["used10"];

            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Depository009_01"; //"/CIMSReport/Depository009_01";

            //設置報表參數
            ReportParameter[] reportParam = new ReportParameter[11];
            reportParam[0] = new ReportParameter("budget_year", strYear, true);
            reportParam[1] = new ReportParameter("used1", Convert.ToDecimal(used1).ToString("N2"), false);
            reportParam[2] = new ReportParameter("used2", Convert.ToDecimal(used2).ToString("N2"), false);
            reportParam[3] = new ReportParameter("used3", Convert.ToDecimal(used3).ToString("N2"), false);
            reportParam[4] = new ReportParameter("used4", Convert.ToDecimal(used4).ToString("N2"), false);
            reportParam[5] = new ReportParameter("used5", Convert.ToDecimal(used5).ToString("N2"), false);
            reportParam[6] = new ReportParameter("used6", Convert.ToDecimal(used6).ToString("N2"), false);
            reportParam[7] = new ReportParameter("used7", Convert.ToDecimal(used7).ToString("N2"), false);
            reportParam[8] = new ReportParameter("used8", Convert.ToDecimal(used8).ToString("N2"), false);
            reportParam[9] = new ReportParameter("used9", Convert.ToDecimal(used9).ToString("N2"), false);
            reportParam[10] = new ReportParameter("used10", used10, false);

            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
