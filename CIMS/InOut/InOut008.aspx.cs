//*****************************************
//*  作    者：
//*  功能說明：
//*  創建日期：
//*  修改日期：2021-03-12
//*  修改記錄：新增次月下市預測表匯入 陳永銘
//*****************************************
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;

public partial class InOut_InOut004 : PageBase
{
    InOut008BL blManager = new InOut008BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "loadRadio();", true);

        this.gvpbChangeCard.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            FillDropDate();
            this.dropFactory_Name.DataTextField = "Factory_ShortName_CN";
            this.dropFactory_Name.DataValueField = "RID";
            this.dropFactory_Name.DataSource = blManager.GetPerso().Tables[0];
            this.dropFactory_Name.DataBind();
            dropFactory_Name.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
            //設置下拉列表種的日期
            FillDropDateFTP();

            ddlYear.SelectedValue = DateTime.Now.Year.ToString();

            ddlMonth.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');
        }

    }
    private void FillDropDate()
    {
        int year = DateTime.Now.Year;
        for (int y = year - 10; y <= year + 10; y++)
        {//填充年下拉列表
            this.dropChange_Year.Items.Add(y.ToString());
        }
        this.dropChange_Year.SelectedIndex = 10;
        for (int m = 1; m < 13; m++)
        {//填充月下拉列表
            if (m < 10)
            {
                this.dropChange_Month.Items.Add("0" + m.ToString());
            }
            else
            {
                this.dropChange_Month.Items.Add(m.ToString());
            }
        }
        int month = DateTime.Now.Month;
        this.dropChange_Month.SelectedIndex = month - 1;
    }

    protected void gvpbChangeCard_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList

        Dictionary<string, object> inputs = new Dictionary<string, object>();

        inputs.Add("dropChange_Date", dropChange_Year.SelectedValue + dropChange_Month.SelectedValue);
        if (dropFactory_Name.SelectedItem.Text == "全部")
        {
            inputs.Add("dropFactory_Name", "");
        }
        else
        {
            inputs.Add("dropFactory_Name", dropFactory_Name.SelectedValue);
        }

        inputs.Add("txtName", txtName.Text);
        inputs.Add("txtDeplete", txtDeplete.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstFORE_CHANGE_CARD = null;

        try
        {

            dstFORE_CHANGE_CARD = blManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            if (dstFORE_CHANGE_CARD != null)//如果查到了資料
            {
                Session["dstFORE_CHANGE_CARD"] = dstFORE_CHANGE_CARD;
                e.Table = dstFORE_CHANGE_CARD.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數

                btnExport.Visible = intRowCount != 0;
            }
            else
            {
                gvpbChangeCard.DataSource = dstFORE_CHANGE_CARD;
                gvpbChangeCard.DataBind();
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbChangeCard.BindData();
    }

    protected void BtnImport_Click(object sender, EventArgs e)
    {
        DataSet dsFileImp = new DataSet();
        bool blMonthChangeForcastStart = false;

        string sError = "";
        try
        {
            //匯入伺服器資料 
            if (adrtInftp.Checked)
            {

                string FileName = "";
                string basepath = ConfigurationManager.AppSettings["NextMonthReplaceCardForecastFilesPath"].ToString();
                bool Exists = true;
                DataSet dsCARDTYPE = null;
                try
                {

                    DataSet dsFILE_NAME = blManager.GetFilename();
                    foreach (DataRow dr in dsFILE_NAME.Tables[0].Rows)
                    {
                        FileName = dr[0].ToString() + ddlYear.SelectedItem.Text + ddlMonth.SelectedItem.Text + ".txt";
                        if (File.Exists(basepath + "\\" + FileName))
                        {
                            Exists = false;
                            dsCARDTYPE = blManager.DetailCheck(basepath + "\\" + FileName, ref sError);

                            if (sError == "")
                            {
                                blManager.In(dsCARDTYPE, FileName);
                                ShowMessage("匯入成功");

                            }
                            else
                            {
                                this.ViewState["Year"] = dsCARDTYPE;
                                this.ViewState["File"] = FileName;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=confirm('"
                                 + sError + "匯入文檔有格式有下列不正確，要匯入正確的小計檔資訊嗎？" + "');if(aa==true){ImtBind();}", true);

                            }
                        }
                    }
                    if (Exists)
                    {
                        throw new AlertException("沒有找到當前匯入年預測檔！");
                    }

                }
                catch (AlertException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    //ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
                    throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
                }
            }
            else
            {
                if (FileUpd.PostedFile.ContentLength == 0)
                {
                    ShowMessage("請選擇匯入文件");
                    return;
                }
                string strPath = FileUpload(FileUpd.PostedFile);
                if (!StringUtil.IsEmpty(strPath))
                {
                    dsFileImp = blManager.DetailCheck(strPath, ref sError);
                    if (!blManager.MonthChangeForcastStart())
                    {
                        ShowMessage("月度換卡預測匯入已經開始，不能重復開始！");
                        return;
                    }

                    if (sError == "")
                    {
                        blMonthChangeForcastStart = true;

                        blManager.In(dsFileImp, FileUpd.FileName);
                        blManager.MonthChangeForcastEnd();
                        ShowMessage("匯入成功");
                    }
                    else
                    {
                        this.ViewState["Year"] = dsFileImp;
                        this.ViewState["File"] = FileUpd.FileName;
                        blManager.MonthChangeForcastEnd();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=confirm('"
                         + sError + "匯入文檔有格式有下列不正確，要匯入正確的小計檔資訊嗎？" + "');if(aa==true){ImtBind();}", true);

                    }


                }
            }
        }
        catch (Exception ex)
        {
            if (blMonthChangeForcastStart)
                blManager.MonthChangeForcastEnd();
            ShowMessage(ex.Message);
        }
    }

    /// <param name="file">上傳文件</param>
    protected string FileUpload1(HttpPostedFile file)
    {
        string path = "";
        if (IsFolderExist())
        {
            string tmpname = file.FileName;
            int i = tmpname.LastIndexOf("\\");
            string filename = tmpname.Substring(i + 1);
            string filetype = filename.Substring(filename.LastIndexOf(".") + 1);
            filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + filetype;
            if (filetype.ToLower().Equals("txt"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a <= 10000.00)
                    {
                        file.SaveAs(Server.MapPath(ConfigurationManager.AppSettings["WebName"] + filename));
                        path = Server.MapPath(ConfigurationManager.AppSettings["WebName"] + filename);
                    }
                    else
                    {
                        ShowMessage("大小不能大於10M");
                    }
                }
                catch
                {
                    ShowMessage("上傳失敗");
                }
            }
            else
            {
                ShowMessage("格式錯誤");
            }
        }
        return path;
    }

    private void FillDropDateFTP()
    {
        // 2019改為2029 add judy 2018/05/03
        for (int y = 2008; y < 2029; y++)
        {//填充年下拉列表
            this.ddlYear.Items.Add(y.ToString());
        }
        for (int m = 1; m < 13; m++)
        {//填充月下拉列表
            if (m < 10)
            {
                this.ddlMonth.Items.Add("0" + m.ToString());
            }
            else
            {
                this.ddlMonth.Items.Add(m.ToString());
            }
        }
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        string timeMark = DateTime.Now.ToString("yyyyMMddHHmmss");
        blManager.ADD_CARD_YEAR_FORCAST_PRINT((DataSet)Session["dstFORE_CHANGE_CARD"], timeMark);
        Response.Write("<script>window.open('InOut008Print.aspx?Deplete=" + txtDeplete.Text + "&time=" + timeMark + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");
    }
    protected void gvpbChangeCard_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[3].Visible = false;
            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Text = e.Row.Cells[3].Text + "-" + e.Row.Cells[4].Text + "-" + e.Row.Cells[5].Text;

            e.Row.Cells[3].Visible = false;
            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;

            if (e.Row.Cells[8].Text != "" && e.Row.Cells[8].Text != "&nbsp;")
                e.Row.Cells[8].Text = Convert.ToInt32(e.Row.Cells[8].Text).ToString("N0");
        }
    }

    /// <summary>
    /// 匯入正確的資料！
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        DataSet dsCardType = (DataSet)this.ViewState["Year"];
        string FileName = (string)this.ViewState["File"];
        blManager.MonthChangeForcastStart();
        blManager.In(dsCardType, FileName);
        blManager.MonthChangeForcastEnd();
        ShowMessage("匯入成功");
    }
}
