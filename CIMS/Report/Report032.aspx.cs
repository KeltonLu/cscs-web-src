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
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Reporting.WebForms;

public partial class Report_Report032 : PageBase
{
    Report032BL rep = new Report032BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            dropMaterialBind(dropMaterialType.Items[0].Text);
            dropFactoryBind();
            dropActionBind();
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        if (!rep.IsMONTHLY_MONITOR())
        {
            ShowMessage("當日未執行每月監控批次");
            return;
        }

        string datatime = System.DateTime.Now.Year.ToString() + "/" + System.DateTime.Now.Month.ToString();
        ArrayList al = new ArrayList();
        for (int i = 0; i < 16; i++)
        {
            al.Add(Convert.ToDateTime(datatime).AddMonths(i).ToString("yyyy/MM/dd",System.Globalization.DateTimeFormatInfo.InvariantInfo));
        }

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropFactory", dropFactory.SelectedValue);
        inputs.Add("dropMaterialType", dropMaterialType.SelectedValue);
        inputs.Add("dropMaterial_RID", dropMaterial_RID.SelectedValue);
        inputs.Add("timenow", Convert.ToDateTime(al[0]).Year + "/" + Convert.ToDateTime(al[0]).Month+"/1");
        inputs.Add("strTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
        DataSet datads = new DataSet();

        if (dropAction.SelectedValue != "")
        {
            inputs.Add("dropAction", dropAction.SelectedValue + ",");
        }
        else
        {
            inputs.Add("dropAction", dropAction.Items[1].Value + "," + dropAction.Items[2].Value + "," + dropAction.Items[3].Value + ",");

        }

        if (dropMaterial_RID.SelectedValue != "")
        {
            GenReport(inputs, 1);
        }
        else
        {
            GenReport(inputs, 2);
        }
    }

    private void GenReport(Dictionary<string, object> inputs, int i)
    {

        this.ReportView1.Visible = true;

        ReportView1.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
        if (i == 1)
            ReportView1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report032_2";
        else
            ReportView1.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "Report032";

        ReportParameter[] reportParam = new ReportParameter[6];
        reportParam[0] = new ReportParameter("TimeNow", inputs["timenow"].ToString(), false);
        reportParam[1] = new ReportParameter("diff", inputs["dropAction"].ToString(), false);
        reportParam[2] = new ReportParameter("perso_facotry", inputs["dropFactory"].ToString(), false);
        reportParam[3] = new ReportParameter("material_type", inputs["dropMaterialType"].ToString(), false);
        reportParam[4] = new ReportParameter("materialrid", inputs["dropMaterial_RID"].ToString(), false);
        reportParam[5] = new ReportParameter("RCT", inputs["strTime"].ToString(), false);
        ReportView1.ServerReport.SetParameters(reportParam);

        ReportView1.ShowParameterPrompts = false;
    }

    protected void dropMaterialType_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropMaterial_RID.Items.Clear();
        dropMaterialBind(dropMaterialType.SelectedValue);
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

    protected void dropMaterialBind(string strMaterialtype)
    {
        DataSet ds = new DataSet();
        if (strMaterialtype == "信封")
        {
            ds = rep.getENVELOPE();
        }
        else if (strMaterialtype == "寄卡單")
        {
            ds = rep.getEXPONENT();
        }
        else
        {
            ds = rep.getDMTYPE();
        }
        dropMaterial_RID.DataSource = ds.Tables[0];
        dropMaterial_RID.DataValueField = "rid";
        dropMaterial_RID.DataTextField = "name";
        dropMaterial_RID.DataBind();
        dropMaterial_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    protected void dropActionBind()
    {
        DataSet ds = new DataSet();
        ds = rep.getAction();
        dropAction.DataSource = ds.Tables[0];
        dropAction.DataValueField = "param_code";
        dropAction.DataTextField = "param_name";
        dropAction.DataBind();
        dropAction.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }
}
