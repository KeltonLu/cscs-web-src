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

public partial class Finance_Finance004_2Print : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //報表參數
            string month = Request.QueryString["month"];
            string year = Request.QueryString["year"];
           // string RID = Request.QueryString["RID"];
            string RID = "";
            string Group_RID = Request.QueryString["Group_RID"];
            string DateFrom = Request.QueryString["DateFrom"];
            string DateTo = Request.QueryString["DateTo"];
            string Time = Request.QueryString["time"];
            string Last_W_Number = Session["Last_W_Number"].ToString();
            string S_Number = Session["S_Number"].ToString();
            string F_Number = Session["F_Number"].ToString();
            string Back_Number = Session["Back_Number"].ToString();
            string P_Number = Session["P_Number"].ToString();
            string T_Number = Session["T_Number"].ToString();
            string A_Number = Session["A_Number"].ToString();
            string W_Number = Session["W_Number"].ToString();
            string D_Number = Session["D_Number"].ToString();
            string UseOutNumber = Session["UseOutNumber"].ToString();
            string lblXH_Numer = Session["lblXH_Numer"].ToString();
            string lblTZ_Numer = Session["lblTZ_Numer"].ToString();
            string lblLast_YM = Session["lblLast_YM"].ToString();
            Session.Remove("Last_W_Number");
            Session.Remove("S_Number");
            Session.Remove("F_Number");
            Session.Remove("Back_Number");
            Session.Remove("P_Number");
            Session.Remove("T_Number");
            Session.Remove("A_Number");
            Session.Remove("W_Number");
            Session.Remove("D_Number");
            Session.Remove("UseOutNumber");
            Session.Remove("lblXH_Numer");
            Session.Remove("lblTZ_Numer");
            Session.Remove("lblLast_YM");
            
            //設置報表
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Finance004_2";

            ReportParameter[] reportParam = new ReportParameter[20];
            reportParam[0] = new ReportParameter("month", month, false);
            reportParam[1] = new ReportParameter("Group_RID", Group_RID, false);
            reportParam[2] = new ReportParameter("DateFrom", DateFrom, false);
            reportParam[3] = new ReportParameter("DateTo", DateTo, false);
            reportParam[4] = new ReportParameter("Last_W_Number", Last_W_Number, false);
            reportParam[5] = new ReportParameter("S_Number", S_Number, false);
            reportParam[6] = new ReportParameter("F_Number", F_Number, false);
            reportParam[7] = new ReportParameter("Back_Number", Back_Number, false);
            reportParam[8] = new ReportParameter("P_Number", P_Number, false);
            reportParam[9] = new ReportParameter("T_Number", T_Number, false);
            reportParam[10] = new ReportParameter("A_Number", A_Number, false);
            reportParam[11] = new ReportParameter("W_Number", W_Number, false);
            reportParam[12] = new ReportParameter("D_Number", D_Number, false);
            reportParam[13] = new ReportParameter("UseOutNumber", UseOutNumber, false);
            reportParam[14] = new ReportParameter("year", year, false);
            reportParam[15] = new ReportParameter("RID", RID, false);
            reportParam[16] = new ReportParameter("RCT", Time, false);
            reportParam[17] = new ReportParameter("lblXH_Numer", lblXH_Numer, false);
            reportParam[18] = new ReportParameter("lblTZ_Numer", lblTZ_Numer, false);
            reportParam[19] = new ReportParameter("lblLast_YM", lblLast_YM, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
