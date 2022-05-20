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


public partial class Report_Report079 : PageBase
{
    CardTypeManager ctmManager = new CardTypeManager();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dropFactoryBind();
            dropUseGroupBind();

            ReportView1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report009";


            txtDate_Time.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
    }

    private void GenReport()
    {

        this.ReportView1.Visible = true;

        ReportParameter[] reportParam = new ReportParameter[6];
        reportParam[0] = new ReportParameter("Date_Time", txtDate_Time.Text, true);
        reportParam[1] = new ReportParameter("Perso_Factory", dropFactory.SelectedValue, true);
        reportParam[2] = new ReportParameter("Use", dropUse.SelectedValue, false);
        reportParam[3] = new ReportParameter("Group", dropGroup.SelectedValue, false);
        reportParam[4] = new ReportParameter("UseName", dropUse.SelectedItem.Text, true);
        reportParam[5] = new ReportParameter("GroupName", dropGroup.SelectedItem.Text, true);
        ReportView1.ServerReport.SetParameters(reportParam);
        //ReportView1.ShowParameterPrompts = false;
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Report018BL bl = new Report018BL();
        if (!bl.IsDefineExpression())
        {
            ShowMessage("無公式定義，無法計算");
            return;
        }

        if (txtDate_Time.Text.Trim() == "")
        {
            ShowMessage("日期不能為空！");
        }
        else
        {
            GenReport();
            return;
            if (dropFactory.SelectedValue == "")
            {
                Response.Write("<script>window.open('Report079Print.aspx?date_time=" + txtDate_Time.Text + "&perso=" + dropFactory.SelectedValue + "');</script>");
            }
            else
            {
                Response.Write("<script>window.open('Report079Print.aspx?date_time=" + txtDate_Time.Text + "&perso=" + dropFactory.SelectedValue + "');</script>");
            }
        }
    }

    /// <summary>
    /// Perso廠下拉框綁定
    /// </summary>
    protected void dropFactoryBind()
    {
        CardType005BL ctbl = new CardType005BL();
        // 獲取 Perso廠商資料
        DataSet dstFactory = ctbl.GetFactoryList();
        dropFactory.DataValueField = "RID";
        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataSource = dstFactory.Tables[0];
        dropFactory.DataBind();
        dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }
    protected void dropUseGroupBind()
    {

        // 獲取 用途資料
        DataSet Use = ctmManager.GetPurpose();
        dropUse.DataSource = Use;
        dropUse.DataValueField = "Param_Code";
        dropUse.DataTextField = "Param_Name";
        dropUse.DataBind();
        dropUse.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        dropGroupBind();
    }
    /// <summary>
    /// 查詢該用途的所有卡穜群組，放於【群組】下拉框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropUse_SelectedIndexChanged(object sender, EventArgs e)
    {

        dropGroupBind();
    }
    protected void dropGroupBind()
    {

        DataSet Group = ctmManager.GetGroupByPurposeId(dropUse.SelectedValue);
        dropGroup.DataSource = Group;
        dropGroup.DataValueField = "RID";
        dropGroup.DataTextField = "Group_Name";
        dropGroup.DataBind();
        dropGroup.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }
}
