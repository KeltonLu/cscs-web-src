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

public partial class Depository_Depository014Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Legend 2018/01/09 添加第一次加載時, 呈現報表
        if (!IsPostBack)
        {
            string time = Request.QueryString["time"];


            //設置報表控件屬性
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "depository_014";
            ReportParameter[] reportParam = new ReportParameter[1];//參數
            reportParam[0] = new ReportParameter("TimeMark", time, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
