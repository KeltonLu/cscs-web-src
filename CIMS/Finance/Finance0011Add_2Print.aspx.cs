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

public partial class Finance_Finance0011Add_2Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string SAP_Serial_Number = Request.QueryString["SAP_Serial_Number"];
            string Comment = Request.QueryString["Comment"];
            string Fine = Request.QueryString["Fine"];
            string strTime = Request.QueryString["Time"];

            if (Fine.Trim() != "")
            {
                Fine = Convert.ToDecimal(Fine).ToString("N2");
            }
            else Fine = "0";


            //設置報表
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Finance0011";

            ReportParameter[] reportParam = new ReportParameter[4];
            reportParam[0] = new ReportParameter("SAP_Serial_Number", SAP_Serial_Number, false);
            reportParam[1] = new ReportParameter("Comment", Comment, false);
            reportParam[2] = new ReportParameter("Fine", Fine, false);
            reportParam[3] = new ReportParameter("TimeMark", strTime, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
