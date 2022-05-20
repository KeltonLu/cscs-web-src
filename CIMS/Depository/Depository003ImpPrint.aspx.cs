//******************************************************************
//*  作    者：FangBao
//*  功能說明：報表輸出
//*  創建日期：2008-09-04
//*  修改日期：2008-09-04 12:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 鮑方
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

public partial class Depository_Depository003ImpPrint : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Depository003BL BL = new Depository003BL();
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];
            string strStockRID = BL.GetSTOCKRID(strRID);

            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            if (strStockRID.Substring(8, 4) == "9999")
            {
                ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Depository003_02";   
            }
            else
            {
                ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Depository003_01";     
            }
            


            ReportParameter[] reportParam = new ReportParameter[1];
            reportParam[0] = new ReportParameter("RID", strRID, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
