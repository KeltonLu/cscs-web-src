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

public partial class Report_Report007_02print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string Date_Time = Request.QueryString["Date_Time"];
            string RCT = Request.QueryString["strTime"];
            string Card_Group_RID = Request.QueryString["Card_Group_RID"];
            string Perso_Factory = Request.QueryString["Perso_Factory"];

            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report007_2";

            ReportParameter[] reportParam = new ReportParameter[4];
            reportParam[0] = new ReportParameter("Date_Time", Date_Time, false);
            reportParam[1] = new ReportParameter("Card_Group_RID", Card_Group_RID, false);
            reportParam[2] = new ReportParameter("Perso_Factory", Perso_Factory, false);
            reportParam[3] = new ReportParameter("RCT", RCT, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
