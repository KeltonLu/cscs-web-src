//******************************************************************
//*  �@    �̡GRay
//*  �\�໡���G�C��o�d�Ʋέp��
//*  �Ыؤ���G2008/12/18
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

public partial class Report_Report018 : PageBase 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            this.ReportView.Visible = false;
            this.Label1.Visible = false;

            ReportView.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "REPORT_018";

        }
    }

    private void GenReport(string Date,string CardList,string time,string FactoryName)
    {
        //��l�Ƴ���Ѽ�
        this.ReportView.Visible = true;
        //��Report View��ȰѼ�
        Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[4];
        Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("Date", Date);
        Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("CardList", CardList);
        Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("RCT", time);
        Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("FactoryName", FactoryName);


        this.ReportView.ServerReport.SetParameters(Paras);
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Report018BL bl = new Report018BL();
        if (!bl.IsDefineExpression())
        {
            ShowMessage("�L�����w�q�A�L�k�p��");
            return;
        }

        ListBox lb = (ListBox)this.UctrlCardType3_1.FindControl("LbRight");//�d�ئC��
        RadioButton rbGroup = (RadioButton)this.UctrlCardType3_1.FindControl("rdoGroup");
        RadioButton rbCard = (RadioButton)this.UctrlCardType3_1.FindControl("rdoCard");
        DropDownList dropYear = (DropDownList)this.UctrlCardType3_1.FindControl("dropYear");
        DropDownList dropGroup = (DropDownList)this.UctrlCardType3_1.FindControl("dropCard_Group");
        DropDownList dropFactory = (DropDownList)this.UctrlCardType3_1.FindControl("dropFactory");
        Report018BL r18 = new Report018BL();
        string FactoryName= dropFactory.SelectedItem.Text.ToString();
        string Year;
        string Date;
        string[] aCard;
        string RID, Name;
        RID = "";
        Name = "";
        if (rbCard.Checked)
        {
            if (lb.Items.Count > 10)
            {
                this.Label1.Text = "�ܦh��ܤQ�ӥd��";
                this.Label1.Visible = true;
                this.ReportView.Visible = false;
                return;
            }
            if (lb.Items.Count == 0)
            {
                this.Label1.Text = "�Цܤֿ�ܤ@�ӥd��";
                this.Label1.Visible = true;
                this.ReportView.Visible = false;
                return;
            }
        }
        if (rbGroup.Checked && StringUtil.IsEmpty(dropGroup.SelectedValue))
        {
            this.Label1.Text = "������ܤ@�Ӹs��";
            this.Label1.Visible = true;
            this.ReportView.Visible = false;
            return;
        }
        this.Label1.Visible = false;
        Year = dropYear.SelectedValue;
        Date = Year + "�~��";
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        try
        {
            if (rbCard.Checked) //��ܥd��
            {
                //���o�d�߱���
                aCard = GetCardList();
                RID = aCard[0];
                Name = aCard[1];
                r18.getReportDataByCardList(Year, RID, time, dropFactory.SelectedValue);//20090702CR-�W�[��Perso�t���d�߿ﶵ
            }
            if (rbGroup.Checked) //��ܸs��
            {
                RID = dropGroup.SelectedValue;
                Name = dropGroup.SelectedItem.Text + "�s�ժ������d��";
                r18.getReportDataByGroupRid(Year, RID, time, dropFactory.SelectedValue);//20090702CR-�W�[��Perso�t���d�߿ﶵ
            }
            GenReport(Date, Name, time, FactoryName);


            Report018BL bl018 = new Report018BL();
            string strRiJie = bl018.GetLastRiJie();
            if (!StringUtil.IsEmpty(strRiJie))
            {
                if (Convert.ToDateTime(strRiJie) < Convert.ToDateTime(dropYear.SelectedValue + "/12/31"))
                {
                    ShowMessage(strRiJie + "�᥼�鵲");
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// ���o��ܥd�ظ��
    /// </summary>
    /// <returns>�d�ئC��A�Ρ�,�����j</returns>
    private string[] GetCardList()
    {
        string items = "";
        string values = "";
        string[] returnValue = new string[2];
        ListBox lb = (ListBox)this.UctrlCardType3_1.FindControl("LbRight");//�d�ئC��
        foreach (ListItem li in lb.Items)
        {
            if (StringUtil.IsEmpty(values))
            {
                values = li.Value;
                items = li.Text;
            }
            else
            {
                values = values + "," + li.Value;
                items = items + "+" + li.Text;
            }
        }
        returnValue[0] = values;
        returnValue[1] = items;
        return returnValue;
    }


}
