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

public partial class Report_Report014 : PageBase 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //UrctrlCardNameSelect1.CardTypeAll = true;
        this.btnQuery.Attributes.Add("onclick","return CheckReg()");
        if (!this.IsPostBack)
        {
            this.ReportView.Visible = false;
            this.Label1.Visible = false;

            ReportView.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            ReportView.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "REPORT_014";

            BindPerso();
        }
    }

    private void GenReport(int PersoID, string StartDate, string EndDate, string UseType, int GroupID,int CardID,string FieldName)
    {


        //��l�Ƴ���Ѽ�
        this.ReportView.Visible = true;
        //��Report View��ȰѼ�
        Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[7];
        Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("PersoID", PersoID.ToString());
        Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("SpendDateS", StartDate);
        Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("SpendDateE", EndDate);
        Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("Utype", UseType);
        Paras[4] = new Microsoft.Reporting.WebForms.ReportParameter("Group", GroupID.ToString());
        Paras[5] = new Microsoft.Reporting.WebForms.ReportParameter("CardID", CardID.ToString());
        Paras[6] = new Microsoft.Reporting.WebForms.ReportParameter("FieldName", FieldName);

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

        Report014 r14 = new Report014();
        int PersoID;
        string DateStart;
        string DateEnd;
        string UseType; //�γ~
        int GroupID; //�s��
        int CardID;  //�d��ID
        string strChecked = ""; ;//�O�_�鵲���ܻy
        string FieldName; //Report���W��
        DropDownList ddlPurpose, ddlGroup, ddlType;
        ListItem li;
        Report0090 r9 = new Report0090();
        PersoID = Convert.ToInt32(this.dropFactory.SelectedValue);

        DateStart = this.txtDate_Time.Text ;
        DateEnd = this.txtDate_Time2.Text ;
        Depository003BL bl003 = new Depository003BL();
        if (!bl003.IsCheck1(DateEnd))
        {
            ShowMessage(DateEnd+"���鵲");
            return;
        }

        //strChecked = r14.IsChecked(DateStart,DateEnd);
        if (StringUtil.IsEmpty(strChecked))
        {
            this.Label1.Visible = false;
            ddlPurpose = (DropDownList)UrctrlCardNameSelect1.FindControl("dropCard_Purpose");//�γ~
            UseType = ddlPurpose.SelectedValue;
            ddlGroup = (DropDownList)UrctrlCardNameSelect1.FindControl("dropCard_Group");//�s��
            if (StringUtil.IsEmpty(ddlGroup.SelectedValue))
                GroupID = -1;
            else
                GroupID = Convert.ToInt32(ddlGroup.SelectedValue);
            ddlType = (DropDownList)UrctrlCardNameSelect1.FindControl("dropCard_Type");//�d��
            if (StringUtil.IsEmpty(ddlType.SelectedValue))
                CardID  = -1;
            else
                CardID = Convert.ToInt32(ddlType.SelectedValue);
            
            if (CardID > -1)
            {
                li = ddlType.SelectedItem;
                FieldName = li.Text; //Ū����ܶ��������W��
            }
            else 
            {
                if (GroupID > -1)
                {
                    li = ddlGroup.SelectedItem;
                    FieldName = li.Text; //Ū����ܶ����s�զW��
                }
                else
                {
                    FieldName = "";
                    CardTypeManager cmbl = new CardTypeManager();
                    DataTable dtblGroup;
                    if(UseType=="")
                        dtblGroup = cmbl.GetGroupByPurposeId("use1").Tables[0];
                    else
                        dtblGroup = cmbl.GetGroupByPurposeId(UseType).Tables[0];

                    foreach (DataRow drow in dtblGroup.Rows)
                    {
                        if (StringUtil.IsEmpty(FieldName))
                            FieldName = drow["GROUP_NAME"].ToString();
                        else
                            FieldName = FieldName + "+" + drow["GROUP_NAME"].ToString();
                    }
                }

            }
            GenReport(PersoID, DateStart, DateEnd, UseType, GroupID, CardID, FieldName);
        }
        else
        {
            this.Label1.Text = strChecked;
            this.Label1.Visible=true;
            this.ReportView.Visible = false;
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
        dropFactory.Items.Insert (0,new ListItem("����", "-1")); 
    }

}
