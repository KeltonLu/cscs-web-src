//******************************************************************
//*  作    者：lantaosu
//*  功能說明：物料庫存專業作業
//*  創建日期：2008-09-09
//*  修改日期：2008-09-12 12:00
//*  修改記錄：
//*            □2008-09-09
//*              1.創建 蘇斕濤
//*******************************************************************

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

public partial class Depository_Depository011_1Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string Move_ID = Request.QueryString["Move_ID"];

            //設置報表控件屬性
         
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString()+"Depository011_01";


            ReportParameter[] reportParam = new ReportParameter[1];//參數
            reportParam[0] = new ReportParameter("Move_ID", Move_ID, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
