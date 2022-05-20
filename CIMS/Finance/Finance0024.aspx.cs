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
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.IO;
public partial class Finance_Finance0024 : PageBase
{
    Finance0024BL FinanceBL = new Finance0024BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dropCard_Purpose_RIDBind();
            dropCard_Group_RIDBind();
            dropFactoryBind();

            dropYear.SelectedValue = System.DateTime.Now.Year.ToString();
            dropMonth.SelectedValue = System.DateTime.Now.Month.ToString();

            DataTable dtProject = new DataTable();
            dtProject.Columns.Add("RID");
            dtProject.Columns.Add("Param_Name");
            dtProject.Columns.Add("Group_Name");
            dtProject.Columns.Add("Project_Date");
            dtProject.Columns.Add("Name");
            dtProject.Columns.Add("CardGroup_RID");
            dtProject.Columns.Add("Number");
            dtProject.Columns.Add("Unit_Price");
            dtProject.Columns.Add("Perso_Factory_RID");
            dtProject.Columns.Add("Comment");
            Session["Project"] = dtProject;

            DataTable dtFinance = new DataTable();
            dtFinance.Columns.Add("RID");
            dtFinance.Columns.Add("Param_Name");
            dtFinance.Columns.Add("Group_Name");
            dtFinance.Columns.Add("Perso_Factory");
            dtFinance.Columns.Add("Project_Date");
            dtFinance.Columns.Add("CardGroup_RID");
            dtFinance.Columns.Add("ProjectName");
            dtFinance.Columns.Add("Perso_Factory_RID");
            dtFinance.Columns.Add("Price");
            dtFinance.Columns.Add("Comment");
            Session["Finance"] = dtFinance;
        }

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "onLoaddoRadio();", true);
    }

    /// <summary>
    /// 文件上傳功能
    /// </summary>
    /// <param name="file">上傳文件</param>
    protected new string FileUpload(HttpPostedFile file)
    {
        string path = "";
        if (IsFolderExist())
        {
            string tmpname = file.FileName;
            int i = tmpname.LastIndexOf("\\");
            string filename = tmpname.Substring(i + 1);
            string filetype = filename.Substring(filename.LastIndexOf(".") + 1);
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (filetype.ToLower().Equals("txt"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a <= 10000.00)
                    {
                        path = ConfigurationManager.AppSettings["SpecialProjectFilesPath"].ToString() + date + "\\" + filename;
                        // 如果目錄下文件已經存在，先刪除掉。
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        file.SaveAs(ConfigurationManager.AppSettings["SpecialProjectFilesPath"].ToString() + date + "\\" + filename);
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
    //************************************************************************
    /// <summary>
    /// 建立上傳目錄
    /// </summary>
    /// <returns>true：成功 false：失敗</returns>
    //************************************************************************
    public new bool IsFolderExist()
    {
        string basepath = ConfigurationManager.AppSettings["SpecialProjectFilesPath"].ToString();
        string date = DateTime.Now.ToString("yyyyMMdd");
        try
        {
            // Determine whether the directory exists.	
            if (!Directory.Exists(basepath + date))
            {
                // Create the directory it does not exist.
                Directory.CreateDirectory(basepath + date);
            }
            return true;
        }

        catch
        {
            return false;
        }
    }

    //開始匯入
    protected void btnDataIn_Click(object sender, EventArgs e)
    {
        if (StringUtil.IsEmpty(this.FileUpd.PostedFile.FileName.ToString()))
        {
            ShowMessage("匯入資料必須輸入！");
            return;
        }

        if (StringUtil.IsEmpty(this.dropFactory.SelectedValue.ToString()))
        {
            ShowMessage("Perso廠商必須輸入！");
            return;
        }

        gvbFinanceDataIn.BindData();
    }

    //保存匯入資料
    protected void btnSave_Click(object sender, EventArgs e)
    {
        // 檢查有無匯入的資料
        if (ViewState["DataInNum"] == null || Convert.ToInt32(ViewState["DataInNum"]) == 0)
        {
            ShowMessage("無匯入的資料！");
            return;
        }

        bool blSpecialProjectImport = false;

        try
        {
            // 從Session中取匯入資料
            DataSet dsProject = (DataSet)ViewState["DataIn"];

            string strFileName = "";
            if (ViewState["FileName"] != null)
                strFileName = ViewState["FileName"].ToString();

            if (!FinanceBL.SpecialProjectImportStart())
            {
                ShowMessage("特殊代制項目匯入已經啟動，不能重復匯入。");
                return;
            }
            blSpecialProjectImport = true;

            // 保存
            FinanceBL.SaveSpecialIn(dsProject, strFileName);
            FinanceBL.SpecialProjectImportEnd();

            gvbFinanceDataIn.DataSource = null;
            gvbFinanceDataIn.DataBind();

            ViewState["DataIn"] = null;
            ViewState["DataInNum"] = 0;

            btnSave.Enabled = false;

            // 匯入成功
            ShowMessage("代製費用異動資料訊息保存成功！");

        }
        catch (Exception ex)
        {
            if (blSpecialProjectImport)
                FinanceBL.SpecialProjectImportEnd();
            ShowMessage(ex.Message);
        }
    }

    //取消保存匯入資料
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        gvbFinanceDataIn.DataSource = null;
        gvbFinanceDataIn.DataBind();
        ViewState["DataIn"] = null;
        ViewState["DataInNum"] = 0;
        btnSave.Enabled = false;
    }

    //刪除匯入資料
    protected void btnDeldata_Click(object sender, EventArgs e)
    {
        bool blSpecialProjectImport = false;
        try
        {
            if (FinanceBL.ConSpecialIn(Convert.ToInt32(dropYear.SelectedValue), Convert.ToInt32(dropMonth.SelectedValue), Convert.ToInt32(dropFactory.SelectedValue)))
            {
                // 磁條行用卡已經請款
                if (!FinanceBL.ConAskedMoney(dropYear.SelectedValue.ToString(), dropMonth.SelectedValue.ToString(), dropFactory.SelectedValue.ToString()))
                {
                    if (!FinanceBL.SpecialProjectImportStart())
                    {
                        ShowMessage("特殊代制項目匯入已經啟動，不能刪除特殊代制項目。");
                        return;
                    }
                    blSpecialProjectImport = true;

                    FinanceBL.DelSpecialIn(Convert.ToInt32(dropYear.SelectedValue), Convert.ToInt32(dropMonth.SelectedValue), Convert.ToInt32(dropFactory.SelectedValue));
                    FinanceBL.SpecialProjectImportEnd();

                    ShowMessage("資料刪除成功！");
                }
                else
                {
                    ShowMessage("該廠商該月磁條信用卡群組有請款，不能刪除特殊項目！");
                }
            }
            else
            {
                ShowMessage("沒有可刪除資料！");
            }
        }
        catch (Exception ex)
        {
            if (blSpecialProjectImport)
                FinanceBL.SpecialProjectImportEnd();
            ShowMessage(ex.Message);
        }
    }

    //人工輸入例外代製項目明細查詢
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        gvbProject.BindData();
    }

    //人工輸入例外代製項目明細查詢
    protected void AddP_Click(object sender, EventArgs e)
    {
        gvbProject.BindData();
    }

    //代製費用帳務異動查詢
    protected void btnQuery2_Click(object sender, EventArgs e)
    {
        gvbFinance.BindData();
    }

    //代製費用帳務異動查詢
    protected void AddF_Click(object sender, EventArgs e)
    {
        gvbFinance.BindData();
    }

    /// <summary>
    /// 刪除人工輸入例外代製項目明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        try
        {
            DataTable dtemp = (DataTable)Session["Project"];
            int intIndex = int.Parse(e.CommandArgument.ToString());
            // 如果已經請款的不能刪除
            if (FinanceBL.ConAskedMoneyGroup(Convert.ToDateTime(dtemp.Rows[intIndex]["Project_Date"]),
                                        dtemp.Rows[intIndex]["Perso_Factory_RID"].ToString(),
                                        dtemp.Rows[intIndex]["CardGroup_RID"].ToString()))
            {
                ShowMessage("該記錄已經請款不能刪除");
                return;
            }

            FinanceBL.DelExcepProject(Convert.ToInt32(dtemp.Rows[intIndex]["RID"]));

            dtemp.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
            gvbProject.BindData();

            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 刪除代製費用帳務異動明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteItem_Command(object sender, CommandEventArgs e)
    {
        try
        {
            DataTable dtemp = (DataTable)Session["Finance"];
            int intIndex = int.Parse(e.CommandArgument.ToString());
            // 如果已經請款的不能刪除
            if (FinanceBL.ConAskedMoneyGroup(Convert.ToDateTime(dtemp.Rows[intIndex]["Project_Date"]),
                                        dtemp.Rows[intIndex]["Perso_Factory_RID"].ToString(),
                                        dtemp.Rows[intIndex]["CardGroup_RID"].ToString()))
            {
                ShowMessage("該記錄已經請款不能刪除");
                return;
            }

            FinanceBL.DelChangeProject(Convert.ToInt32(dtemp.Rows[int.Parse(e.CommandArgument.ToString())]["RID"]));

            dtemp.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
            gvbFinance.BindData();

            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //用途變更
    protected void dropCard_Purpose_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            dropCard_Group_RIDBind();
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #region 列表資料綁定
    //人工輸入頁面綁定
    protected void gvbProject_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropMonth", dropMonth.SelectedValue);
        inputs.Add("dropYear", dropYear.SelectedValue);
        inputs.Add("dropFactory", this.dropFactory.SelectedValue.ToString());
        inputs.Add("Card_Group_RID", dropCard_Group_RID.SelectedValue);


        DataTable dtbProject = (DataTable)Session["Project"];
        dtbProject.Rows.Clear();

        DataSet dsProject = null;
        try
        {
            dsProject = FinanceBL.SearchExcepProject(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);


            // 檢查成功，綁定GRID，顯示匯入資料明細
            if (dsProject != null)//如果查到了資料
            {
                e.Table = dsProject.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //人工輸入頁面綁定
    protected void gvbProject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblProject = (DataTable)gvbProject.DataSource;



        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dtbProject = (DataTable)Session["Project"];

            try
            {
                ImageButton ibtnButton = null;
                // 刪除的邦定事件
                ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
                ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
                ibtnButton.OnClientClick = string.Concat("return confirm(\'", BizMessage.BizCommMsg.ALT_CMN_IsDel, "\')");

                HyperLink hl = (HyperLink)e.Row.FindControl("hlModify");
                hl.Text = dtblProject.Rows[e.Row.RowIndex]["Name"].ToString();
                hl.NavigateUrl = "#";
                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Finance0024Addp.aspx?ActionType=Edit&RID=" + dtblProject.Rows[e.Row.RowIndex]["RID"].ToString() + "','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");

                if (e.Row.Cells[2].Text != "&nbsp;")
                {
                    e.Row.Cells[2].Text = Convert.ToDateTime(e.Row.Cells[2].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }

                DataRow dr = dtbProject.NewRow();
                dr["RID"] = dtblProject.Rows[e.Row.RowIndex]["RID"].ToString();
                dr["Param_Name"] = e.Row.Cells[0].Text;
                dr["Group_Name"] = e.Row.Cells[1].Text;
                dr["Project_Date"] = e.Row.Cells[2].Text;
                dr["CardGroup_RID"] = dtblProject.Rows[e.Row.RowIndex]["CardGroup_RID"].ToString();
                dr["Perso_Factory_RID"] = dtblProject.Rows[e.Row.RowIndex]["Perso_Factory_RID"].ToString();
                dr["Name"] = hl.Text;
                dr["Number"] = e.Row.Cells[4].Text;
                dr["Unit_Price"] = e.Row.Cells[5].Text;
                dr["Comment"] = e.Row.Cells[6].Text;
                dtbProject.Rows.Add(dr);
            }
            catch
            {

            }
        }
    }

    //匯入綁定
    protected void gvbFinanceDataIn_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("FileUpd", this.FileUpd.PostedFile.FileName);
        inputs.Add("FactoryRID", this.dropFactory.SelectedValue.ToString());

        DataSet dstlDataIn = null;
        ViewState["DataIn"] = null;
        ViewState["DataInNum"] = 0;

        try
        {
            string[] file = this.FileUpd.PostedFile.FileName.Split('\\');
            // 再上傳該文件
            string strPath = FileUpload(FileUpd.PostedFile);
            if (!StringUtil.IsEmpty(strPath))
            {
                // 對匯入資料進行檢查
                dstlDataIn = FinanceBL.CheckSpecialIn(strPath, this.dropFactory.SelectedValue.ToString());

                // 檢查成功，綁定GRID，顯示匯入資料明細
                if (dstlDataIn != null)//如果查到了資料
                {
                    ViewState["DataIn"] = dstlDataIn;
                    ViewState["DataInNum"] = dstlDataIn.Tables[1].Rows.Count;
                    e.Table = dstlDataIn.Tables[1];//要綁定的資料表
                    e.RowCount = dstlDataIn.Tables[1].Rows.Count;//查到的行數
                }

                if (dstlDataIn.Tables[1].Rows.Count == 0)
                    btnSave.Enabled = false;
                else
                {
                    btnSave.Enabled = true;
                    ViewState["FileName"] = FileUpd.FileName;
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //代製費用帳務異動綁定
    protected void gvbFinance_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropMonth", dropMonth.SelectedValue);
        inputs.Add("dropYear", dropYear.SelectedValue);
        inputs.Add("dropFactory", this.dropFactory.SelectedValue.ToString());
        inputs.Add("Card_Group_RID", dropCard_Group_RID.SelectedValue);

        DataTable dtbFinance = (DataTable)Session["Finance"];
        dtbFinance.Rows.Clear();

        DataSet dsFinance = null;
        try
        {
            dsFinance = FinanceBL.SearchChangeProject(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            // 檢查成功，綁定GRID，顯示匯入資料明細
            if (dsFinance != null)//如果查到了資料
            {
                e.Table = dsFinance.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }



    //匯入頁面綁定
    protected void gvbFinanceDataIn_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblDataIn = (DataTable)gvbFinanceDataIn.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                Label lblProject_Name = null;
                lblProject_Name = (Label)e.Row.FindControl("lblProject_Name");
                lblProject_Name.Text = FinanceBL.getProjectNameByCode(dtblDataIn.Rows[e.Row.RowIndex]["Project_Code"].ToString());
            }
            catch
            {
            }
        }
    }

    //代製費用帳務異動頁面綁定
    protected void gvbFinance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblFinance = (DataTable)gvbFinance.DataSource;



        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataTable dtbFinance = (DataTable)Session["Finance"];

            try
            {
                ImageButton ibtnButton = null;
                // 刪除的邦定事件
                ibtnButton = (ImageButton)e.Row.FindControl("ibtnDeleteItem");
                ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
                ibtnButton.OnClientClick = string.Concat("return confirm(\'", BizMessage.BizCommMsg.ALT_CMN_IsDel, "\')");

                HyperLink hl = (HyperLink)e.Row.FindControl("hlModify2");
                hl.Text = dtblFinance.Rows[e.Row.RowIndex]["ProjectName"].ToString();
                hl.NavigateUrl = "#";
                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Finance0024Addf.aspx?ActionType=Edit&RID=" + dtblFinance.Rows[e.Row.RowIndex]["RID"].ToString() + "','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind2();}");

                Label lblProject_Date = null;
                lblProject_Date = (Label)e.Row.FindControl("lblProject_Date");
                lblProject_Date.Text = Convert.ToDateTime(dtblFinance.Rows[e.Row.RowIndex]["Project_Date"].ToString()).ToString("yyyy/MM", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                DataRow dr = dtbFinance.NewRow();
                dr["RID"] = dtblFinance.Rows[e.Row.RowIndex]["RID"].ToString();
                dr["Param_Name"] = e.Row.Cells[0].Text;
                dr["Group_Name"] = e.Row.Cells[1].Text;
                dr["Perso_Factory"] = e.Row.Cells[2].Text;
                dr["Project_Date"] = Convert.ToDateTime(dtblFinance.Rows[e.Row.RowIndex]["Project_Date"].ToString()).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                dr["CardGroup_RID"] = dtblFinance.Rows[e.Row.RowIndex]["CardGroup_RID"].ToString();
                dr["Perso_Factory_RID"] = dtblFinance.Rows[e.Row.RowIndex]["Perso_Factory"].ToString();
                dr["ProjectName"] = hl.Text;
                dr["Price"] = e.Row.Cells[5].Text;
                dr["Comment"] = e.Row.Cells[6].Text;
                dtbFinance.Rows.Add(dr);
            }
            catch
            {
            }
        }
    }

    #endregion 列表資料綁定

    #region 初始化下拉框
    /// <summary>
    /// Perso卡廠下拉框綁定
    /// </summary>
    protected void dropFactoryBind()
    {
        dropFactory.Items.Clear();

        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataValueField = "RID";
        dropFactory.DataSource = FinanceBL.getFactory();
        dropFactory.DataBind();

        //dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_Purpose_RIDBind()
    {
        dropCard_Purpose_RID.DataTextField = "PARAM_NAME";
        dropCard_Purpose_RID.DataValueField = "Param_Code";
        dropCard_Purpose_RID.DataSource = FinanceBL.getParam_Finance();
        dropCard_Purpose_RID.DataBind();

        //dropCard_Purpose_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_Group_RIDBind()
    {
        dropCard_Group_RID.Items.Clear();

        dropCard_Group_RID.DataTextField = "GROUP_NAME";
        dropCard_Group_RID.DataValueField = "RID";
        dropCard_Group_RID.DataSource = FinanceBL.getCardGroup(dropCard_Purpose_RID.SelectedValue);
        dropCard_Group_RID.DataBind();

        dropCard_Group_RID.Items.Insert(0, new ListItem("全部", ""));
    }
    #endregion 初始化下拉框

}
