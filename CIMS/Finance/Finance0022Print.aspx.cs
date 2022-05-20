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

public partial class Finance_Finance0022Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {


            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Finance0022";

            string strGroupName = Request.QueryString["GroupName"];
            string strTime = Request.QueryString["Time"];
            string strBegin_Date=Request.QueryString["Begin_Date"];
            string strMonth = "";
            //string strFinish_Date = Request.QueryString["Finish_Date"];
            if (strBegin_Date.Length > 8)
            {
                strMonth = strBegin_Date.Substring(5, 2);
            }
            ReportParameter[] reportParam = new ReportParameter[3];
            reportParam[0] = new ReportParameter("GroupName", strGroupName, true);
            reportParam[1] = new ReportParameter("TimeMark", strTime, false);
            reportParam[2] = new ReportParameter("Month", strMonth, true);
            //reportParam[3] = new ReportParameter("Finish_Date", strFinish_Date, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);

            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
