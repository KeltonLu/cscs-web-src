using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Reporting.WebForms;

public partial class Report_Report016 : PageBase
{
    Report016BL rep = new Report016BL();

    // add by Ian Huang 查詢條件加上「用途」、「群組」 start
    CardTypeManager ctmManager = new CardTypeManager();
    // add by Ian Huang 查詢條件加上「用途」、「群組」 end

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 獲取 Perso廠商資料
            DataSet dstFactory = rep.GetFactoryList();
            dropFactory.DataValueField = "RID";
            dropFactory.DataTextField = "Factory_ShortName_CN";
            dropFactory.DataSource = dstFactory.Tables[0];
            dropFactory.DataBind();
            dropFactory.Items.Insert(0, new ListItem("全部", "-1"));
            // add by Ian Huang 查詢條件加上「用途」、「群組」 start
            dropCard_PurposeBind();
            dropCard_GroupBind();
            // add by Ian Huang 查詢條件加上「用途」、「群組」 end

            ListItem li1 = new ListItem();
            li1.Text = "3D";
            li1.Value = "3D";
            li1.Selected = true;

            ListItem li2 = new ListItem();
            li2.Text = "DA";
            li2.Value = "DA";
            li2.Selected = true;


            ListItem li3 = new ListItem();
            li3.Text = "PM";
            li3.Value = "PM";
            li3.Selected = true;


            ListItem li4 = new ListItem();
            li4.Text = "RN";
            li4.Value = "RN";
            li4.Selected = true;


            cblCon.Items.Add(li1);
            cblCon.Items.Add(li2);
            cblCon.Items.Add(li3);
            cblCon.Items.Add(li4);
        }
    }

    private void GenReport()
    {
        ReportView.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
        ReportView.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report016";

        this.ReportView.Visible = true;
        string strAction = "";
        string strActionTxt = "";
        string strActionType = "";
        string searchType = "";
        if (rblShowType.SelectedValue == "mon")
            searchType = "1";
        else
            searchType = "2";

        if (rblCon.SelectedValue == "1")
        {
            strActionType = "2";
            strActionTxt = "批次(";
            foreach (ListItem li in cblCon.Items)
            {
                if (li.Selected)
                {
                    strAction += "'" + li.Value + "',";
                    strActionTxt += li.Text + ",";
                }
            }
            strAction = strAction.Substring(0, strAction.Length - 1);
            strActionTxt = strActionTxt.Substring(0, strActionTxt.Length - 1)+")";
        }
        else
        {
            strActionType = "1";
            strActionTxt = "Action(";
            foreach (ListItem li in cblCon.Items)
            {
                if (li.Selected)
                {
                    strAction += li.Value + ",";
                    strActionTxt += li.Text + ",";
                }
            }
            strAction = strAction.Substring(0, strAction.Length - 1);
            strActionTxt = strActionTxt.Substring(0, strActionTxt.Length - 1) + ")";
        }

        // edit by Ian Huang start
        ReportParameter[] reportParam = new ReportParameter[8];
        reportParam[0] = new ReportParameter("begintime", txtDate_TimeFrom.Text, false);
        reportParam[1] = new ReportParameter("endtime", txtDate_TimeTo.Text, false);
        reportParam[2] = new ReportParameter("action", strAction, false);
        reportParam[3] = new ReportParameter("actionType", strActionType, false);
        reportParam[4] = new ReportParameter("Searchtype", searchType, false);
        reportParam[5] = new ReportParameter("actiontxt", strActionTxt, false);
        reportParam[6] = new ReportParameter("PersonRid", dropFactory.SelectedValue, false);
        reportParam[7] = new ReportParameter("GroupRid", dropCard_Group.SelectedValue, false);
        // edit by Ian Huang end


        ReportView.ServerReport.SetParameters(reportParam);
        ReportView.ShowParameterPrompts = false;
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {  

        ArrayList array = new ArrayList();
        DateTime begin_time = Convert.ToDateTime(txtDate_TimeFrom.Text);
        DateTime end_time = Convert.ToDateTime(txtDate_TimeTo.Text);

        bool n = false;
        foreach (ListItem li in cblCon.Items)
        {
            if (li.Selected)
            {
                n = true;
                break;
            }
        }
        if (rblCon.SelectedValue == "1")
        {
            if (!n)
            {
                ShowMessage("必須勾選一個批次");
                return;
            }
        }
        else
        {
            if (!n)
            {
                ShowMessage("必須勾選一個Action");
                return;
            }
        }

        Report018BL bl018 = new Report018BL();
        string strRiJie = bl018.GetLastRiJie();
        if (!StringUtil.IsEmpty(strRiJie))
        {
            if (Convert.ToDateTime(strRiJie) < end_time.AddMonths(1).AddDays(-1))
            {
                ShowMessage(strRiJie + "後未日結");
            }
        }

        GenReport();
        return;
    }

    //匯出Excel
    protected void btnExcel_Click(object sender, EventArgs e)
    {
      
    }

    protected void gvpbReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       
    }
    protected void rblCon_SelectedIndexChanged(object sender, EventArgs e)
    {
        cblCon.DataSource = null;
        cblCon.DataBind();
        while (cblCon.Items.Count > 0)
        {
            cblCon.Items.RemoveAt(0);
        }

        if (rblCon.SelectedValue == "1")
        {
            ListItem li1 = new ListItem();
            li1.Text = "3D";
            li1.Value = "3D";
            li1.Selected = true;

            ListItem li2 = new ListItem();
            li2.Text = "DA";
            li2.Value = "DA";
            li2.Selected = true;


            ListItem li3 = new ListItem();
            li3.Text = "PM";
            li3.Value = "PM";
            li3.Selected = true;


            ListItem li4 = new ListItem();
            li4.Text = "RN";
            li4.Value = "RN";
            li4.Selected = true;

            cblCon.Items.Add(li1);
            cblCon.Items.Add(li2);
            cblCon.Items.Add(li3);
            cblCon.Items.Add(li4);
        }
        else
        {
            ListItem li1 = new ListItem();
            li1.Text = "1.新卡";
            li1.Value = "1";
            li1.Selected = true;

            ListItem li2 = new ListItem();
            li2.Text = "2.掛補";
            li2.Value = "2";
            li2.Selected = true;


            ListItem li3 = new ListItem();
            li3.Text = "3.毀補";
            li3.Value = "3";
            li3.Selected = true;


            ListItem li4 = new ListItem();
            li4.Text = "5.換卡";
            li4.Value = "5";
            li4.Selected = true;


            cblCon.Items.Add(li1);
            cblCon.Items.Add(li2);
            cblCon.Items.Add(li3);
            cblCon.Items.Add(li4);
        }
    }

    // add by Ian Huang 查詢條件加上「用途」、「群組」 start
    protected void dropCard_Purpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_GroupBind();
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_PurposeBind()
    {
        dropCard_Purpose.DataTextField = "PARAM_NAME";
        dropCard_Purpose.DataValueField = "Param_Code";
        dropCard_Purpose.DataSource = ctmManager.GetPurpose();
        dropCard_Purpose.DataBind();

        dropCard_Purpose.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 群組下拉框綁定

    /// </summary>
    protected void dropCard_GroupBind()
    {
        dropCard_Group.Items.Clear();

        dropCard_Group.DataTextField = "GROUP_NAME";
        dropCard_Group.DataValueField = "RID";
        dropCard_Group.DataSource = ctmManager.GetGroupByPurposeId(dropCard_Purpose.SelectedValue);
        dropCard_Group.DataBind();

        dropCard_Group.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], "-1"));
    }
    // add by Ian Huang 查詢條件加上「用途」、「群組」 end
}
