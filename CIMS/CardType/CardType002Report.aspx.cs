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
using System.Text;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

public partial class CardType_CardType002Report : PageBase
{

    CardType002BL bl = new CardType002BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!IsPostBack)
        {
            string strOrderFormRID = Request.QueryString["orderform_rid"];

            ReportViewer1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportViewer1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "CardType002";

            
            ReportParameter[] reportParam = new ReportParameter[5];

            if (Session["Condition"] == null)
            {

                reportParam[0] = new ReportParameter("GroupRID", "-1", false);
                reportParam[1] = new ReportParameter("Type", "-1", false);
                reportParam[2] = new ReportParameter("name", "-1", false);
                reportParam[3] = new ReportParameter("Is_Using", "-1", false);
                reportParam[4] = new ReportParameter("DateTimeNow", DateTime.Now.ToString("yyyy-MM-dd"), false);
            }
            else
            {
                Dictionary<string, object> inputs = (Dictionary<string, object>)Session["Condition"];

                if (StringUtil.IsEmpty(inputs["dropCardType_Group_RID"].ToString()))
                    reportParam[0] = new ReportParameter("GroupRID", "-1", false);
                else
                    reportParam[0] = new ReportParameter("GroupRID", inputs["dropCardType_Group_RID"].ToString(), false);

                if (StringUtil.IsEmpty(inputs["txtTYPE"].ToString()))
                    reportParam[1] = new ReportParameter("Type", "-1", false);
                else
                    reportParam[1] = new ReportParameter("Type", inputs["txtTYPE"].ToString(), false);

                if (StringUtil.IsEmpty(inputs["txtName"].ToString()))
                    reportParam[2] = new ReportParameter("name", "-1", false);
                else
                    reportParam[2] = new ReportParameter("name", inputs["txtName"].ToString(), false);

                if (StringUtil.IsEmpty(inputs["dropUseType"].ToString()))
                    reportParam[3] = new ReportParameter("Is_Using", "-1", false);
                else
                    reportParam[3] = new ReportParameter("Is_Using", inputs["dropUseType"].ToString(), false);

                reportParam[4] = new ReportParameter("DateTimeNow", DateTime.Now.ToString("yyyy-MM-dd"), false);
            }

            ReportViewer1.ServerReport.SetParameters(reportParam);
            ReportViewer1.ShowParameterPrompts = false;
        }
    }

   
}
