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

public partial class Report_Report024Report : System.Web.UI.Page
{
    Report024BL bl = new Report024BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //處理參數
            DataSet ds = null;
            string date = Request.QueryString["date"];
            string strRID = Request.QueryString["RID"];
            string year = date.Substring(0, 4);
            string month = date.Substring(4, 2);
            string month1 = month;
            string year1 = year;
            //換卡時間 = 畫面輸入的年月 + 1
            month = Convert.ToString(Convert.ToInt32(month) + 1);
            if (Convert.ToInt32(month) < 10)
                date = year + "0" + month;
            else if (Convert.ToInt32(month) == 13)
                date = Convert.ToString(Convert.ToInt32(year) + 1) + "01";
            else
                date = year + month;

            string[] RID = null;

            if (strRID == "all")
            {
                strRID = "";
                ds = bl.GetCooperatePersoList();
                RID = new string[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    RID[i] = ds.Tables[0].Rows[i]["RID"].ToString();
            }
            else
            {
                RID = new string[1];
                RID[0] = strRID;
            }
            //設置報表控件屬性
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report024";

            //參數
            ReportParameter[] reportParam = new ReportParameter[4];
            reportParam[0] = new ReportParameter("date", date, false);
            reportParam[1] = new ReportParameter("RID", RID, false);
            reportParam[2] = new ReportParameter("month", month1, false);
            reportParam[3] = new ReportParameter("year", year1, false);

            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
