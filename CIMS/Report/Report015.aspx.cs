//******************************************************************
//*  作    者：Ray
//*  功能說明：製損卡統計月報表
//*  創建日期：2008/12/3
//*  修改日期：
//*  修改記錄：
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

public partial class Report_Report015 : PageBase 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            this.ReportView.Visible = false;
            this.Label1.Visible = false;
            
            //this.ReportView.ServerReport.ReportPath = GlobalString.SQLReport.Report015;
            ReportView.ServerReport.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString() + "REPORT_015";
            this.ReportView.ServerReport.ReportServerUrl = new System.Uri(ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            InitDate();
            BindPerso();

            dropYear.SelectedValue = DateTime.Now.Year.ToString();

            dropMonth.SelectedValue = DateTime.Now.Month.ToString();
        }
    }

    private void GenReport(int PersoID, string PersoNameCN, string MonthS, string MonthE)
    {


        //初始化報表參數
        this.ReportView.Visible = true;
        //為Report View賦值參數
        Microsoft.Reporting.WebForms.ReportParameter[] Paras = new Microsoft.Reporting.WebForms.ReportParameter[4];
        Paras[0] = new Microsoft.Reporting.WebForms.ReportParameter("Perso", PersoID.ToString());
        Paras[1] = new Microsoft.Reporting.WebForms.ReportParameter("PersoNameCN", PersoNameCN);
        Paras[2] = new Microsoft.Reporting.WebForms.ReportParameter("MonthS", MonthS);
        Paras[3] = new Microsoft.Reporting.WebForms.ReportParameter("MonthE", MonthE);

        this.ReportView.ServerReport.SetParameters(Paras);
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Report018BL bl = new Report018BL();
        if (!bl.IsDefineExpression())
        {
            ShowMessage("無公式定義，無法計算");
            return;
        }

       

        string sYear;//選擇年份
        string sMonth;//選擇月份
        string DateS;
        string DateE;
        int PersoID;//Perso廠ID
        string PersoName;//Perso廠名稱
        sYear = dropYear.SelectedValue;
        sMonth = dropMonth.SelectedValue;
        PersoID = Convert.ToInt32(dropFactory.SelectedValue);
        if (PersoID == -1) //選擇全部Perso廠則顯示"總"
            PersoName = "總";
        else //取得Perso廠名稱
            PersoName = dropFactory.SelectedItem.Text;
        DateS = sYear + "/" + sMonth + "/" + "1"; //得到月份開始日期
        DateE = sYear + "/" + sMonth + "/" + DateTime.DaysInMonth(Convert.ToInt32(sYear), Convert.ToInt32(sMonth));//得到月份結束日期
        //DateS = "2008/9/9";
        //DateE = "2008/9/9";

        string strRiJie = bl.GetLastRiJie();
        if (!StringUtil.IsEmpty(strRiJie))
        {
            if (Convert.ToDateTime(strRiJie) < Convert.ToDateTime(DateE))
            {
                ShowMessage(strRiJie + "後未日結");
            }
        }
        this.GenReport(PersoID, PersoName, DateS, DateE);
        
    }
    /// <summary>
    /// 綁定Perso下拉列表
    /// </summary>
    private void BindPerso()
    {
        Report0090 r9 = new Report0090();
        DataSet ds = r9.GetPersoList();
        this.dropFactory.DataSource = ds;
        this.dropFactory.DataTextField = "Factory_ShortName_CN";
        this.dropFactory.DataValueField = "RID";
        dropFactory.DataBind();
        dropFactory.Items.Insert (0,new ListItem("全部", "-1")); 
    }
    /// <summary>
    /// 初始化日期欄位
    /// </summary>
    private void InitDate()
    {
        int i;
        for (i = 1; i <= 12; i++)
        {
            this.dropMonth.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        for (i = 1998; i <= 2028; i++) // 2018改為2028 add judy 218/05/03
        {
            this.dropYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }

}
