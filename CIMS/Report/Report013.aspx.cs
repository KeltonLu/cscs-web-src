//******************************************************************
//*  �@    �̡GRay
//*  �\�໡���G�t�Ӫ��Ʈw�s��
//*  �Ыؤ���G2008/12/3
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

public partial class Report_Report013 : PageBase 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.btnQuery.Attributes.Add("onclick", "return CheckReg()");
        if (!this.IsPostBack)
        {
            this.ReportView.Visible = false;
            //this.Label1.Visible = false;
            ReportView.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "REPORT_013";
            this.ReportView.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            BindPerso();
            BindMateriel();
        }
    }

    private void GenReport(int PersoID, string PersonName ,string StartDate,string EndDate,string Mtype,string MID)
    {


        //��l�Ƴ���Ѽ�
        this.ReportView.Visible = true;
        //��Report View��ȰѼ�
        Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[7];
        Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("PersoID", PersoID.ToString());
        Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("StockDateStart", StartDate);
        Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("StockDateEnd", EndDate);
        Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("MATERIELType", Mtype);
        Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("MATERIELID", MID);
        Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("PersoNameCN", PersonName);
        Paras[6] = new Microsoft.Reporting.WebForms.ReportParameter("systime", DateTime.Now.ToString("yyyy-MM-dd"));

        this.ReportView.ServerReport.SetParameters(Paras);
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        int PersoID;
        string PersoNameCN;
        string StockDateStart;
        string StockDateEnd;
        string MType;
        string MID;
        Report0090 r9 = new Report0090();
        PersoID = Convert.ToInt32(this.dropFactory.SelectedValue);
        if (PersoID == -1)
        {
            PersoNameCN = "�`";
        }
        else
        {

            PersoNameCN = r9.GetPersoName(PersoID);
        }
        StockDateStart = this.txtDate_Time.Text ;
        StockDateEnd = this.txtDate_Time2.Text ;
        if (this.rdoType.Checked) //��������
        {
            MType = "1";
            MID = ddrType.SelectedValue;
        }
        else //��@�~�W
        {
            MType = "2";
            MID = ddrSerial.SelectedValue;
        }
        GenReport(PersoID, PersoNameCN, StockDateStart, StockDateEnd, MType, MID);
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
        dropFactory.Items.Insert(0, new ListItem("����", "-1"));
    }
    private void BindMateriel()
    {
        Report013 r13 = new Report013();
        DataSet ds = r13.GetMATERIEL();
        this.ddrSerial.DataSource = ds;
        this.ddrSerial.DataTextField = "Name";
        this.ddrSerial.DataValueField = "Serial_Number";
        ddrSerial.DataBind();
        
    }
}
