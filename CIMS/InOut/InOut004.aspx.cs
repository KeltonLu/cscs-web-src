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
using System.Text;
using System.IO;

public partial class InOut_InOut004 : PageBase
{
    InOut004BL bizManager = new InOut004BL();

    #region 事件處理
    //頁面加載
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "loadRadio();", true);
        this.gvpbChangeCard.PageSize = GlobalStringManager.PageSize;
        gvpbChangeCard.NoneData = "";
        if (!IsPostBack)
        {
            btnToExcel.Visible = false;
            //設置下拉列表種的日期
            FillDropDate();

            ddlYear.SelectedValue = DateTime.Now.Year.ToString();

            ddlMonth.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2,'0');

            ddlYear1.SelectedValue = DateTime.Now.Year.ToString();

            ddlMonth1.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');

            ddlYear2.SelectedValue = DateTime.Now.Year.ToString();

            ddlMonth2.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');
        }
    }

    //輸出Excel表格
    protected void btnToExcel_Click(object sender, EventArgs e)
    {
        string timeMark = DateTime.Now.ToString("yyyyMMddHHmmss");
        bizManager.ADD_CARD_YEAR_FORCAST_PRINT((DataSet)Session["dtsName_Sum"], timeMark);
        Response.Write("<script>window.open('InOut004ImpPrint.aspx?txtAffinity_Code=" + txtAffinity_Code.Text.ToString() + "&time=" + timeMark + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");
    }

    //文件匯入
    protected void btnImport_Click(object sender, EventArgs e)
    {
        DataSet dsFileImp = new DataSet();
        bool blYearChangeForcastStart = false;

        string sError = "";

        try
        {
            //匯入伺服器資料 
            if (adrtInftp.Checked)
            {
                //bizManager.FileCheck(Convert.ToInt16(ddlYear.SelectedItem.Text), Convert.ToInt16(ddlMonth.SelectedItem.Text));


                //文檔名
                string FileName = "";
                string basepath = ConfigurationManager.AppSettings["YearReplaceCardForecastFilesPath"].ToString();
                bool Exists = true;
                DataSet dsCARDTYPE = null;
                string strFileName = "";


                try
                {

                    DataSet  dsFILE_NAME = bizManager.GetFilename();
                    foreach (DataRow dr in dsFILE_NAME.Tables[0].Rows)
                    {
                        FileName = dr[0].ToString() + ddlYear.SelectedItem.Text + Convert.ToInt16(ddlMonth.SelectedItem.Text).ToString("00") + ".txt";
                        if (File.Exists(basepath + "\\" + FileName))
                        {
                            Exists = false;
                            strFileName += FileName + ",";
                            dsCARDTYPE = bizManager . DetailCheck(basepath + "\\" + FileName, ref sError);

                            if (sError == "")
                            {
                                bizManager.In(dsCARDTYPE, FileName);
                                ShowMessage("匯入成功");

                            }
                            else
                            {
                                this.ViewState["Year"] = dsCARDTYPE;
                                this.ViewState["File"] = FileName;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=confirm('"
                                 + sError + "匯入文檔有格式有下列不正確，要匯入正確的小計檔資訊嗎？"  + "');if(aa==true){ImtBind();}", true);

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
                    if (!bizManager.YearChangeForcastStart())
                    {
                        ShowMessage("年度換卡預測匯入已經開始，不能重復開始！");
                        return;
                    }

                    dsFileImp = bizManager.DetailCheck(strPath, ref sError);

                    if (sError == "")
                    {
                        blYearChangeForcastStart = true;
                        bizManager.In(dsFileImp, FileUpd.FileName);
                        bizManager.YearChangeForcastEnd();
                        ShowMessage("匯入成功");
                    }
                    else
                    {
                        this.ViewState["Year"] = dsFileImp;
                        this.ViewState["File"] = FileUpd.FileName;
                        bizManager.YearChangeForcastEnd();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=confirm('"
                         + sError + "匯入文檔有格式有下列不正確，要匯入正確的小計檔資訊嗎？" + "');if(aa==true){ImtBind();}", true);

                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogFactory.Write(ex.Message + "\r\n" + ex.StackTrace, GlobalString.LogType.ErrorCategory);
            if (blYearChangeForcastStart)
                bizManager.YearChangeForcastEnd();
            ShowMessage(ex.Message);
        }
    }

    //查詢
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (true == radCard.Checked)
        {
            if (UctrlCardType.GetRightItem.Rows.Count == 0)
            {
                ShowMessage("卡種欄位必須選擇");
                return;
            }
        }
        GridBind();
    }

    #endregion

    #region 欄位/資料補充說明

    private void GridBind()
    {
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropDate_Begin", ddlYear1.SelectedItem.Text + ddlMonth1.SelectedItem.Text);
        inputs.Add("dropDate_Over", ddlYear2.SelectedItem.Text + ddlMonth2.SelectedItem.Text);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("txtDeplete", txtDeplete.Text);
        inputs.Add("txtAffinity_Code", txtAffinity_Code.Text);
        inputs.Add("uctrlCARDNAME", UctrlCardType.GetRightItem);

        DataSet dtsName_Sum = new DataSet();
        DataSet dstlChangeCard = null;
        btnToExcel.Visible = false;

        dstlChangeCard = bizManager.List(inputs, ref dtsName_Sum);

        if (dstlChangeCard != null)//如果查到了資料
        {
            //刪除沒有資料的空余行
            DataRow[] drowsDEL = dstlChangeCard.Tables[0].Select("卡片編號 is null ");
            foreach (DataRow dr in drowsDEL)
            {
                dstlChangeCard.Tables[0].Rows.Remove(dr);
            }

            DataView dv = new DataView(dstlChangeCard.Tables[0], "", "卡片編號 ", DataViewRowState.CurrentRows);

            gvpbChangeCard.DataSource = dv;
            gvpbChangeCard.DataBind();



            if (dstlChangeCard.Tables[0].Rows.Count > 0)
                this.lbMsg.Visible = false;
            else
                this.lbMsg.Visible = true;

            btnToExcel.Visible = true;
            Session["dtsName_Sum"] = dtsName_Sum;
        }
    }

    protected new string FileUpload(HttpPostedFile file)
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
                        file.SaveAs(ConfigurationManager.AppSettings["YearReplaceCardForecastFilesPath"] + filename);
                        path = ConfigurationManager.AppSettings["YearReplaceCardForecastFilesPath"] + filename;
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
                ShowMessage("文件格式必須為txt");
            }
        }
        return path;
    }

    private void FillDropDate()
    {
        // Legend 2018/01/16 小於迄年2019改為2029
        for (int y = 2008; y < 2029; y++)
        {//填充年下拉列表
            this.ddlYear.Items.Add(y.ToString());
            this.ddlYear1.Items.Add(y.ToString());
            this.ddlYear2.Items.Add(y.ToString());
        }
        for (int m = 1; m < 13; m++)
        {//填充月下拉列表
            if (m < 10)
            {
                this.ddlMonth.Items.Add("0" + m.ToString());
                this.ddlMonth1.Items.Add("0" + m.ToString());
                this.ddlMonth2.Items.Add("0" + m.ToString());
            }
            else
            {
                this.ddlMonth.Items.Add(m.ToString());
                this.ddlMonth1.Items.Add(m.ToString());
                this.ddlMonth2.Items.Add(m.ToString());
            }
        }
    }
    #endregion
    protected void gvpbChangeCard_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropDate_Begin", ddlYear1.SelectedItem.Text + ddlMonth1.SelectedItem.Text);
        inputs.Add("dropDate_Over", ddlYear2.SelectedItem.Text + ddlMonth2.SelectedItem.Text);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("txtDeplete", txtDeplete.Text);
        inputs.Add("txtAffinity_Code", txtAffinity_Code.Text);
        inputs.Add("uctrlCARDNAME", UctrlCardType.GetRightItem);

        DataSet dtsName_Sum = new DataSet();
        DataSet dstlChangeCard = null;
        btnToExcel.Visible = false;

        dstlChangeCard = bizManager.List(inputs, ref dtsName_Sum);

        if (dstlChangeCard != null)//如果查到了資料
        {
            //刪除沒有資料的空余行
            DataRow[] drowsDEL = dstlChangeCard.Tables[0].Select("卡片編號 is null ");
            foreach (DataRow dr in drowsDEL)
            {
                dstlChangeCard.Tables[0].Rows.Remove(dr);
            }
            e.Table = dstlChangeCard.Tables[0];//要綁定的資料表
            e.RowCount = dstlChangeCard.Tables[0].Rows.Count;//查到的行數
            
            if (e.RowCount > 0)
                this.lbMsg.Visible = false;
            else
                this.lbMsg.Visible = true;

            btnToExcel.Visible = true;
            Session["dtsName_Sum"] = dtsName_Sum;
        }
    }

    /// <summary>
    /// 當文件格式有部分錯誤時，導入正確的資料！
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        DataSet dsCardType = (DataSet)this.ViewState["Year"];
        string FileName = (string)this.ViewState["File"];
        bizManager.YearChangeForcastStart();
        bizManager.In(dsCardType, FileName);
        bizManager.YearChangeForcastEnd();
        ShowMessage("匯入成功");
    }
    protected void gvpbChangeCard_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 3; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                if (e.Row.Cells[i].Text != "" && e.Row.Cells[i].Text != "&nbsp;")
                    e.Row.Cells[i].Text = Convert.ToInt32(e.Row.Cells[i].Text).ToString("N0");
            }
        }
    }
}
