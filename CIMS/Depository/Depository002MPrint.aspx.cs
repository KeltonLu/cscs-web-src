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

public partial class Depository_Depository002MPrint : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string OFDRID = Request.QueryString["OFDRID"];
            string Order_Date_FROM = Request.QueryString["Order_Date_FROM"];
            string Order_Date_TO = Request.QueryString["Order_Date_TO"];
            string Fore_Delivery_BDate = Request.QueryString["Fore_Delivery_BDate"];
            string Fore_Delivery_EDate = Request.QueryString["Fore_Delivery_EDate"];
            string Agreement = Request.QueryString["Agreement"];
            string Budget = Request.QueryString["Budget"];
            string Case_Status = Request.QueryString["Case_Status"].Replace("N", "未結案");
            string cardtype = Request.QueryString["cardtype"];
            string BlankFactory = Request.QueryString["BlankFactory"];
            string Factory_ShortName_CN = Request.QueryString["Factory_ShortName_CN"];
            string Isdetail = Request.QueryString["Isdetail"];
           
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Depository002_02";

            ReportParameter[] reportParam = new ReportParameter[12];
            reportParam[0] = new ReportParameter("OFDRID", OFDRID, false);
            reportParam[1] = new ReportParameter("Order_Date_FROM", Order_Date_FROM, false);
            reportParam[2] = new ReportParameter("Order_Date_TO", Order_Date_TO, false);
            reportParam[3] = new ReportParameter("Fore_Delivery_BDate", Fore_Delivery_BDate, false);
            reportParam[4] = new ReportParameter("Fore_Delivery_EDate", Fore_Delivery_EDate, false);
            reportParam[5] = new ReportParameter("Agreement", Agreement, true);
            reportParam[6] = new ReportParameter("Budget", Budget, true);
            reportParam[7] = new ReportParameter("Case_Status", Case_Status, true);
            reportParam[8] = new ReportParameter("cardtype", cardtype, true);
            reportParam[9] = new ReportParameter("BlankFactory", BlankFactory, true);
            reportParam[10] = new ReportParameter("Factory_ShortName_CN", Factory_ShortName_CN, true);
            reportParam[11] = new ReportParameter("Isdetail", Isdetail, true);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
