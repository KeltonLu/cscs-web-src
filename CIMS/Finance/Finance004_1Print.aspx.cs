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

public partial class Finance_Finance004_1Print : System.Web.UI.Page
{
    Finance004_1BL bl = new Finance004_1BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //處理參數
            DataSet ds = null;
            string[] RID = null;
            string[] Group_RID = null;
            string month = Request.QueryString["month"];
            string strRID = Request.QueryString["RID"];
            string GroupRID = Request.QueryString["Group_RID"];
            string DateFrom = Request.QueryString["DateFrom"];
            string DateTo = Request.QueryString["DateTo"];
            string Last_T_Number = Session["Last_T_Number"].ToString();
            string P_Number = Session["P_Number"].ToString();
            string U_Number = Session["U_Number"].ToString();
            string D_Number = Session["D_Number"].ToString();
            string T_Number = Session["T_Number"].ToString();

            string Total = Session["Total"].ToString();
            Session.Remove("Total");
            Session.Remove("Last_T_Number");
            Session.Remove("P_Number");
            Session.Remove("U_Number");
            Session.Remove("D_Number");
            Session.Remove("T_Number");

            if (strRID == "")
            {
                strRID = "";
                ds = bl.GetCooperateBlankList();
                RID = new string[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    RID[i] = ds.Tables[0].Rows[i]["RID"].ToString();
            }
            else
            {
                RID = new string[1];
                RID[0] = strRID;
            }
            if (GroupRID == "")
            {
                GroupRID = "";
                ds = bl.GetGroup();
                Group_RID = new string[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    Group_RID[i] = ds.Tables[0].Rows[i]["RID"].ToString();
            }
            else
            {
                Group_RID = new string[1];
                Group_RID[0] = GroupRID;
            }

            //設置報表控件屬性
            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Finance004_1";


            ReportParameter[] reportParam = new ReportParameter[11];
            reportParam[0] = new ReportParameter("month", month, false);
            reportParam[1] = new ReportParameter("RID", RID, false);
            reportParam[2] = new ReportParameter("Group_RID", Group_RID, false);
            reportParam[3] = new ReportParameter("DateFrom", DateFrom, false);
            reportParam[4] = new ReportParameter("DateTo", DateTo, false);
            reportParam[5] = new ReportParameter("Last_T_Number", Last_T_Number, false);
            reportParam[6] = new ReportParameter("P_Number", P_Number, false);
            reportParam[7] = new ReportParameter("U_Number", U_Number, false);
            reportParam[8] = new ReportParameter("D_Number", D_Number, false);
            reportParam[9] = new ReportParameter("T_Number", T_Number, false);
            reportParam[10] = new ReportParameter("Total", Total, false);
            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }
}
