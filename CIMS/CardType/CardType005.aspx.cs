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
using System.Collections.Generic;
using System.IO;
using System.Net;

public partial class CardType_CardType005 : PageBase
{
    CardType005BL bl = new CardType005BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvPerso_CardType.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            Session.Remove("Percentage_Number");
            ViewState["GetAllData"] = "false";
            // 獲取 Perso廠商資料
            DataSet dstFactory = bl.GetFactoryList();
            dropFactory.DataValueField = "RID";
            dropFactory.DataTextField = "Factory_ShortName_CN";
            dropFactory.DataSource = dstFactory.Tables[0];
            dropFactory.DataBind();
            dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    dropFactory.SelectedValue = ((Dictionary<string, object>)Session["Condition"])["dropFactory"].ToString();
                    uctrlCARDNAME.SetRightItem = (DataTable)(((Dictionary<string, object>)Session["Condition"])["uctrlCARDNAME"]);
                }
                gvPerso_CardType.BindData();
            }
            else
            {
                Session.Remove("Condition");
            }
        }
    }
    protected void btnAdd_Base_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType005AddBase.aspx?ActionType=Add");
    }
    protected void btnAdd_Special_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType005AddSpecial.aspx?ActionType=Add");
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvPerso_CardType_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount;
        Dictionary<string, object> inputs = new Dictionary<string, object>();

        if (ViewState["GetAllData"].ToString() == "false")
        {
            intRowCount = 0;
            DataTable dtblCardType = uctrlCARDNAME.GetRightItem;
            //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
            inputs.Add("dropFactory", dropFactory.SelectedValue);

            inputs.Add("uctrlCARDNAME", uctrlCARDNAME.GetRightItem);


            //保存查詢條件
            Session["Condition"] = inputs; 
        }
        

        DataSet dstlPersoProject = null;

        try
        {
            dstlPersoProject = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlPersoProject != null)//如果查到了資料
            {
                e.Table = dstlPersoProject.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
    #endregion
    protected void gvPerso_CardType_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Visible = false;
            e.Row.Cells[1].Visible = false;
            e.Row.Cells[2].Visible = false;
            e.Row.Cells[3].Visible = false;
            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;
            e.Row.Cells[7].Visible = false;
            e.Row.Cells[8].Visible = false;
            e.Row.Cells[14].Visible = false;
            e.Row.Cells[15].Visible = false;
            e.Row.Cells[16].Visible = false;
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[16].Text == "&nbsp;")
            {
                e.Row.Cells[16].Text = "     ";
            }
            if (e.Row.Cells[13].Text == "&nbsp;")
            {
                e.Row.Cells[13].Text = "  ";
            }
            //string BasePerso = ((HyperLink)e.Row.Cells[9].Controls[0]).Text;

            HyperLink hl = (HyperLink)e.Row.FindControl("hlModify");

            hl.Text = e.Row.Cells[2].Text + e.Row.Cells[3].Text + e.Row.Cells[4].Text;

            hl.NavigateUrl = "CardType005ModSpecial.aspx?ActionType=Edit&CardType_RID=" + e.Row.Cells[5].Text + "&RID=" + e.Row.Cells[0].Text + "&Percentage_Number=" + e.Row.Cells[14].Text + "";

            //Label1.Text += hl.Text.PadRight(9, ' ') + BasePerso + e.Row.Cells[16].Text + e.Row.Cells[11].Text.PadLeft(3, '0') + e.Row.Cells[12].Text.PadLeft(9, '0') + e.Row.Cells[13].Text.PadLeft(2) + "\r\n";

            string strCel13 = e.Row.Cells[13].Text.Trim() == "" ? "00" : e.Row.Cells[13].Text.PadLeft(2, '0');
            string strCel16 = e.Row.Cells[16].Text.Trim() == "" ? "00000" : e.Row.Cells[16].Text;

            Label1.Text += hl.Text.PadRight(9, ' ') + e.Row.Cells[15].Text + strCel16 + e.Row.Cells[11].Text.PadLeft(3, '0') + e.Row.Cells[12].Text.PadLeft(9, '0') + strCel13 + "\r\n";


            e.Row.Cells[11].Text = e.Row.Cells[11].Text + "%";

            if (e.Row.Cells[13].Text == "  ")
            {
                e.Row.Cells[11].Text = "  ";
                e.Row.Cells[12].Text = "  ";
            }
            e.Row.Cells[0].Visible = false;
            e.Row.Cells[1].Visible = false;
            e.Row.Cells[2].Visible = false;
            e.Row.Cells[3].Visible = false;
            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;
            e.Row.Cells[7].Visible = false;
            e.Row.Cells[8].Visible = false;
            e.Row.Cells[14].Visible = false;
            e.Row.Cells[15].Visible = false;
            e.Row.Cells[16].Visible = false;
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ViewState["GetAllData"] = "false";
        Label1.Text = "";
        gvPerso_CardType.BindData();
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        Response.Clear();
        Response.Buffer = false;
        Response.AppendHeader("Content-Disposition", "attachment;filename=card"+DateTime.Now.ToString("yyyyMMddhhmmss")+".txt");
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
        Response.ContentType = "text/plain";
        this.EnableViewState = false;
        Response.Write(Label1.Text);
        Response.End();
    }
    protected void btnUpFile_Click(object sender, EventArgs e)
    {
        try
        {
           


            ViewState["GetAllData"] = "true";
            Label1.Text = "";
            gvPerso_CardType.BindData();
            string FileName = ConfigurationManager.AppSettings["FileName"];

            string sFilePath = Request.MapPath(Request.ApplicationPath ) + "\\FileUpload\\CIMSCARDPERSO.txt";
            StreamWriter sr = new StreamWriter(sFilePath, false, System.Text.Encoding.Default);
            sr.Write(Label1.Text);           
            sr.Close();
            //200911CR 記錄FTP日志 add by YangKun 2009/11/25 start
            LogFactory.Write("Perso廠與卡種設定表,開始上傳\r\n" + Label1.Text.ToString(), GlobalString.LogType.OpLogCategory);
            //200911CR 記錄FTP日志 add by YangKun 2009/11/25 end

            //FTPFactory ftp = new FTPFactory();
            string ftpUser = ConfigurationManager.AppSettings["FTPUser"]; //ftp檔案目錄配置檔信息
            string ftpPassWord = ConfigurationManager.AppSettings["FTPPassword"]; //local檔案目錄配置檔信息
            //ftp.Upload("Saprate", "aa.txt", sFilePath);

            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(ftpUser, ftpPassWord);
            webClient.UploadFile(String.Format(ConfigurationManager.AppSettings["FilePath"], FileName), "STOR", sFilePath);

            //File.Delete(sFilePath);

            ShowMessage("上傳成功！");
            //200911CR 記錄FTP日志 add by YangKun 2009/11/25 start
            LogFactory.Write("Perso廠與卡種設定表上傳成功！", GlobalString.LogType.OpLogCategory);
            //200911CR 記錄FTP日志 add by YangKun 2009/11/25 end
            ViewState["GetAllData"] = "false";
            Label1.Text = "";
            gvPerso_CardType.BindData();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
            //200911CR 記錄FTP日志 add by YangKun 2009/11/25 start
            LogFactory.Write("Perso廠與卡種設定表上傳失敗！" + ex.Message.ToString(), GlobalString.LogType.OpLogCategory);
            //200911CR 記錄FTP日志 add by YangKun 2009/11/25 end
        }
    }
    protected void gvPerso_CardType_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
