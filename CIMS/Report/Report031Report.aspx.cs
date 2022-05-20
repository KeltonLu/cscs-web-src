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

public partial class Report_Report031Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string RIDs = (string) Session["RIDs"];
            string RID = (string)Session["Wafer_RID"]; 
            string DateFrom = Request.QueryString["DateFrom"];
            string DateTo = Request.QueryString["DateTo"];
            if (DateFrom.Trim() == "")
                DateFrom = "1900/1/1";
            if (DateTo.Trim() == "")
                DateTo = "2300/1/1";

            //設置報表控件屬性
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report031";

            //參數
            ReportParameter[] reportParam = new ReportParameter[4];
            reportParam[0] = new ReportParameter("strDateFrom", DateFrom, false);
            reportParam[1] = new ReportParameter("strDateTo", DateTo + " 23:59:59", false);
            reportParam[2] = new ReportParameter("strWafer", RID, false);
            reportParam[3] = new ReportParameter("strCardType", RIDs, false);

            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
