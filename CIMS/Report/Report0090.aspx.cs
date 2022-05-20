//******************************************************************
//*  �@    �̡GRay
//*  �\�໡���G�t�ӥd���w�s�����
//*  �Ыؤ���G2008/11/26
//*  �ק����G
//*  �ק�O���G
//*            
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

public partial class Report_Report0090 : PageBase 
{
    CardTypeManager ctmManager = new CardTypeManager();
    protected void Page_Load(object sender, EventArgs e)
    {
        btnQuery.Attributes.Add("onclick","return CheckReg()");
       
        if (!this.IsPostBack)
        {
            this.ReportView.Visible = false;
            this.Label1.Visible = false;

            ReportView.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "REPORT_009";


            BindPerso();
            dropUseGroupBind();
        }
    }

    private void GenReport(string PersoID, string PersonName ,string CheckDate)
    {


        //��l�Ƴ���Ѽ�
        this.ReportView.Visible = true;
        //��Report View��ȰѼ�
        Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[7];
        Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("Perso_Factory", PersoID.ToString());
        Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("Date_Time", CheckDate);
        Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("PersoNameCN", PersonName);
        Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("Use", dropUse.SelectedValue,false);
        Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("Group", dropGroup.SelectedValue, false);
        Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("UseName", dropUse.SelectedItem.Text);
        Paras[6] = new Microsoft.Reporting.WebForms.ReportParameter("GroupName", dropGroup.SelectedItem.Text);


        this.ReportView.ServerReport.SetParameters(Paras);
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        int PersoID;
        string CheckDate;
        string PersoNameCN;
        PersoID = Convert.ToInt32(this.dropFactory.SelectedValue);
        Report0090 r9 = new Report0090();
        if (PersoID == -1)
        {
            PersoNameCN = "�`";
        }
        else
        {            
            PersoNameCN = r9.GetPersoName(PersoID);
        }
        CheckDate = this.txtDate_Time.Text;
        if (r9.GetCheckStatus(CheckDate))
        {
            Label1.Visible = false;
            if (PersoID == -1)
            {
                GenReport("", PersoNameCN, CheckDate);
            }
            else
            {
                GenReport(dropFactory.SelectedValue, PersoNameCN, CheckDate);
            }
        }
        else
        {
            Label1.Visible = true;
            this.ReportView.Visible = false;
            Label1.Text = BizMessage.BizMsg.ALT_Report011_01;
            //Response.Write("�d�ߤ�����鵲�A���వ�d�߾ާ@�C");
        }
    }
    /// <summary>
    /// �j�wPerso�U�ԦC��
    /// </summary>
    private void BindPerso()
    {
        Report0090 r9 = new Report0090();
        DataSet ds = r9.GetPersoList();
        this.dropFactory.DataSource = ds;
        this.dropFactory.DataTextField = "Factory_ShortName_CN";
        this.dropFactory.DataValueField = "RID";
        dropFactory.DataBind();
        dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], "-1")); 
    }
    protected void dropUseGroupBind()
    {

        // ��� �γ~���
        DataSet Use = ctmManager.GetPurpose(); 
        dropUse.DataSource = Use;
        dropUse.DataValueField = "Param_Code";
        dropUse.DataTextField = "Param_Name";
        dropUse.DataBind();
        dropUse.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        dropGroupBind();
    }
    /// <summary>
    /// �d�߸ӥγ~���Ҧ��d�s�աA���i�s�աj�U�Ԯ�
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
