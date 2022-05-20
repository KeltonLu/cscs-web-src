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
using CIMSClass.Business;
using CIMSClass;
public partial class InOut_InOut002 : PageBase
{
    InOut002BL bl = new InOut002BL();
    InOut001BL BL = new InOut001BL();
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
    InOut007BL BL007 = new InOut007BL();
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end
    bool IsDel = true;
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.txtBegin_Date.Enabled = true;
        this.dropFactory.Enabled = true;
        gvSearch_Factory_Change_Import.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            //預設當前系統時間為匯入日期時間 
            this.txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");

            // 獲取 Perso廠商資料
            GetFactory();

            tb2.Visible = false;

            //記錄刪除廠商庫存異動匯入
            ViewState["Fci_Rid_Del"] = "";
            ViewState["SaveDate"] = "";
            ViewState["Rad_Is_Checked"] = "";
            ViewState["Enabled"] = true;

            this.Session.Remove("Factory_Change_Import");
        }
    }

    /// <summary>
    /// 取Perso廠訊息，并綁定到Perso控件。
    /// </summary>
    private void GetFactory()
    {
        // 獲取 Perso廠商資料
        DataSet dstFactory = bl.GetFactoryList();
        dropFactory.DataValueField = "RID";
        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataSource = dstFactory.Tables[0];
        dropFactory.DataBind();
    }
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
    /// <summary>
    /// 設置日志數據
    /// </summary>
    private void SetLogData()
    {
        BL007.SetOprLogNull();
        if (HttpContext.Current.Session["Action"] != null)
        {
            BL007.SetOprLogActionID(HttpContext.Current.Session["Action"].ToString());
        }
        if (HttpContext.Current.Session[CIMSClass.GlobalString.SessionAndCookieKeys.USER] != null)
        {
            USERS mUser = (USERS)HttpContext.Current.Session[CIMSClass.GlobalString.SessionAndCookieKeys.USER];
            BL007.SetOprLogUser(mUser.UserID, mUser.UserName);
        }

    }
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end

    /// <summary>
    /// 匯入廠商"異動廠商"資訊
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnIMPORT_Click(object sender, EventArgs e)
    {
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
        SetLogData();
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end
        if (this.IsCheck())
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_IsCheck);
            return;
        }

        bool blImportFactoryChangeStart = false;

        try
        {
            if (txtBegin_Date.Text == "")
            {
                throw new Exception("匯入日期不能為空");
            }
            BL.Is_Check(Convert.ToDateTime(txtBegin_Date.Text));

            if (FileUpd.FileName.ToString() == "")
            {
                throw new Exception("請選擇匯入的文件！");
            }
            string strPath = FileUpload(FileUpd.PostedFile);
            if (!StringUtil.IsEmpty(strPath))
            {
                if (!bl.ImportFactoryChangeStart())
                {
                    ShowMessage("廠商異動匯入批次已經啟動，不能重復開始！");
                    return;
                }
                blImportFactoryChangeStart = true;

                DataTable dtblFileImp = new DataTable();
                dtblFileImp.Columns.Add("TYPE");
                dtblFileImp.Columns.Add("AFFINITY");
                dtblFileImp.Columns.Add("PHOTO");
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
                //dtblFileImp.Columns.Add("Name");//版面簡稱(卡種)
                dtblFileImp.Columns.Add("Space_Short_Name");
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end
                dtblFileImp.Columns.Add("Status_RID");//狀況
                dtblFileImp.Columns.Add("Status_Name");//狀況名稱
                dtblFileImp.Columns.Add("Number");//數量               
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
                dtblFileImp.Columns.Add("Perso_Factory_RID");//廠商
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end
                // 廠商資料匯入
                //string strError = bl.Import(strPath,
                //            dtblFileImp,
                //            this.txtBegin_Date.Text,
                //            this.dropFactory.SelectedValue);
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
                string strError = BL007.Import(strPath,
                            dtblFileImp,
                            this.txtBegin_Date.Text,
                            this.dropFactory.SelectedValue);
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end

                bl.ImportFactoryChangeEnd();

                if (strError.Length > 0)
                {
                    ShowMessage(strError);
                }
                else
                {
                    Session["dtblImp"] = dtblFileImp;
                    gvpbFactory_Change_Import.BindData();
                    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                }
            }
        }
        catch (Exception ex)
        {
            if (blImportFactoryChangeStart)
                bl.ImportFactoryChangeEnd();
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 刪除已匯入的廠商庫存異動資訊
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
        SetLogData();
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end
        if (this.IsCheck())
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_IsCheck);
            return;
        }

        bool blImportFactoryChangeStart = false;

        try
        {
            DateTime dtImportDate = DateTime.Parse(this.txtBegin_Date.Text.Trim());

            BL.Is_Check(Convert.ToDateTime(txtBegin_Date.Text));

            // 廠商檢查
            if (this.dropFactory.SelectedValue.ToString() == "")
            {
                ShowMessage("Perso廠商必須選擇。");
                return;
            }

            if (!bl.ImportFactoryChangeStart())
            {
                ShowMessage("廠商異動匯入批次已經啟動，不能重復開始！");
                return;
            }
            blImportFactoryChangeStart = true;

            Dictionary<string, object> inputs = new Dictionary<string, object>();
            // 匯入日期
            inputs.Add("date_time", dtImportDate);
            // 匯入廠商
            inputs.Add("perso_factory_rid", this.dropFactory.SelectedValue.ToString());
            //保存查詢條件
            Session["Condition"] = inputs;
            //if (bl.Delete(inputs) > 0)
            //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 start
            if (BL007.Delete(inputs) > 0)
            //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/18 end
                ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
            else
                ShowMessage("無符合資料刪除");

            bl.ImportFactoryChangeEnd();

        }
        catch (Exception ex)
        {
            if (blImportFactoryChangeStart)
                bl.ImportFactoryChangeEnd();
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 查詢廠商庫存異動資訊
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime dtImportDate = DateTime.Parse(this.txtBegin_Date.Text.Trim());

            // 廠商檢查
            if (this.dropFactory.SelectedValue.ToString() == "")
            {
                ShowMessage("Perso廠商必須選擇。");
                return;
            }

            ViewState["SaveDate"] = this.txtBegin_Date.Text;
            ViewState["Import_Date"] = this.txtBegin_Date.Text;
            ViewState["FactoryRID"] = this.dropFactory.SelectedValue.ToString();
            this.gvSearch_Factory_Change_Import.BindData();
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "displayOK();", true);

            ViewState["Enabled"] = false;
            if (gvSearch_Factory_Change_Import.Rows.Count != 0)
            {
                btnReset.Visible = true;
            }
            else
            {
                btnReset.Visible = false;
            }
        }
        catch (Exception ex)
        {
            gvSearch_Factory_Change_Import.DataSource = null;
            gvSearch_Factory_Change_Import.DataBind();
            this.Session.Remove("Factory_Change_Import");
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 刪除廠商庫存異動資訊_明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteItem_Command(object sender, CommandEventArgs e)
    {
        try
        {
            //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 start
            SetLogData();
            //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 end
            int intIndex = -1;
            DataTable dtFactory_Change_Import = (DataTable)Session["Factory_Change_Import"];
            if (e.CommandArgument.ToString() != "")
            {
                intIndex = int.Parse(e.CommandArgument.ToString());
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 start
                //bl.DelFactory_Change_Import(Convert.ToInt32(dtFactory_Change_Import.Rows[intIndex]["RID"]));
                BL007.DelFactory_Change_Import(Convert.ToInt32(dtFactory_Change_Import.Rows[intIndex]["RID"]));
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 end
                this.gvSearch_Factory_Change_Import.BindData();
            }
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //打開新增異動頁面，更改後返回更新操作
    protected void btnAddFactory_Chage_Import_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "displayOK();", true);
        IsDel = true;
        btnReset.Visible = true;
        this.gvSearch_Factory_Change_Import.BindData();
    }

    //刪除已匯入資料CheckedChanged事件
    protected void RadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        ViewState["SaveDate"] = this.txtBegin_Date.Text;
        this.txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
        if (ViewState["Rad_Is_Checked"].ToString() == "3")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "delConfirm();", true);
        }
        this.Panel1.Visible = false;
        this.Panel2.Visible = false;
        tb2.Visible = true;
        ViewState["Rad_Is_Checked"] = "2";
    }
    //人工輸入CheckedChanged事件
    protected void RadioButton3_CheckedChanged(object sender, EventArgs e)
    {
        btnReset.Visible = false;
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "displayNO();", true);
        ViewState["SaveDate"] = this.txtBegin_Date.Text;
        this.txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
        this.Panel1.Visible = false;
        tb2.Visible = false;
        this.Panel2.Visible = true;
        ViewState["Rad_Is_Checked"] = "3";
        IsDel = false;

        this.Session.Remove("Factory_Change_Import");
        btnSearch.Enabled = true;
        gvSearch_Factory_Change_Import.DataSource = null;
        gvSearch_Factory_Change_Import.DataBind();
    }
    //匯入異動廠商CheckedChanged事件
    protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        ViewState["SaveDate"] = this.txtBegin_Date.Text;
        this.txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
        if (ViewState["Rad_Is_Checked"].ToString() == "3")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "delConfirm();", true);
        }
        tb2.Visible = false;
        this.Panel2.Visible = false;
        this.Panel1.Visible = true;
        ViewState["Rad_Is_Checked"] = "1";

        gvpbFactory_Change_Import.DataSource = null;
        gvpbFactory_Change_Import.DataBind();
    }
    //取消
    protected void btnReset_Click(object sender, EventArgs e)
    {
        this.txtBegin_Date.Enabled = true;
        this.btnSearch.Enabled = true;
        this.dropFactory.Enabled = true;
        this.btnAddFactory_Chage_Import.Enabled = false;
        btnReset.Visible = false;

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "displayNO();", true);

        this.Session["Factory_Change_Import"] = null;
        this.gvSearch_Factory_Change_Import.Visible = false;
    }
    //轉換畫面保持人工輸入頁面查詢的狀態
    protected void btnIsCheck_Click(object sender, EventArgs e)
    {
        this.RadioButton1.Checked = false;
        this.RadioButton2.Checked = false;
        this.RadioButton3.Checked = true;
        ViewState["Rad_Is_Checked"] = "3";
        this.Panel1.Visible = false;
        tb2.Visible = false;
        this.Panel2.Visible = true;
        this.txtBegin_Date.Text = ViewState["SaveDate"].ToString();
    }
    //轉換畫面清除人工輸入頁面查詢的狀態
    protected void btnIsCheckTure_Click(object sender, EventArgs e)
    {
        ViewState["Enabled"] = true;
        if (Session["Factory_Change_Import"] == null)
            return;
        if (((DataTable)Session["Factory_Change_Import"]).Rows.Count != 0)
        {
            ((DataTable)Session["Factory_Change_Import"]).Rows.Clear();
        }
        gvSearch_Factory_Change_Import.BindData();
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
            string date = txtBegin_Date.Text.Replace("/", "");
            if (filetype.ToLower().Equals("txt"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a <= 10000.00)
                    {
                        path = ConfigurationManager.AppSettings["FactoryDepositoryFilesPath"].ToString() + date + "\\" + filename;
                        // 如果目錄下文件已經存在，先刪除掉。
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        file.SaveAs(ConfigurationManager.AppSettings["FactoryDepositoryFilesPath"].ToString() + date + "\\" + filename);
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
        string basepath = ConfigurationManager.AppSettings["FactoryDepositoryFilesPath"].ToString();
        string date = txtBegin_Date.Text.Replace("/", "");
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

    /// <summary>
    /// 重新綁定廠商庫存異動訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDisFactoryChange_Click(object sender, EventArgs e)
    {
        ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
        if (txtBegin_Date.Text != "")
        {
            gvSearch_Factory_Change_Import.BindData();
        }
    }
    #endregion
    #region 列表資料綁定.
    //人工輸入頁面綁定
    protected void gvSearch_Factory_Change_Import_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        this.gvSearch_Factory_Change_Import.Visible = true;
        int intRowCount = 0;
        // 查詢條件集合，Key是欖位名稱，Value是欄位對應的控制項，控制項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();

        inputs.Add("date_time", this.txtBegin_Date.Text);
        inputs.Add("perso_factory_rid", this.dropFactory.SelectedValue);

        // 保存查詢條件
        Session["Condition"] = inputs;

        // 取廠商庫存異動資訊
        DataTable dtFactory_Change_Import_Sub = null;
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 start
        //dtFactory_Change_Import_Sub = bl.List(inputs, e.FirstRow,
        //            e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
        dtFactory_Change_Import_Sub = BL007.List(inputs, e.FirstRow,
                  e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 END

        this.Session["Factory_Change_Import"] = dtFactory_Change_Import_Sub;

        DataTable dtFactory_Change_Import = (DataTable)Session["Factory_Change_Import"];
        if (dtFactory_Change_Import != null)//如果查到了資料
        {
            e.Table = dtFactory_Change_Import;//要綁定的資料表
            e.RowCount = e.Table.Rows.Count;//查到的行數
        }
    }

    //匯入廠商異動綁定
    protected void gvpbFactory_Change_Import_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtblImp = (DataTable)Session["dtblImp"];
        if (null != dtblImp)
        {
            e.Table = dtblImp;
            e.RowCount = dtblImp.Rows.Count;
        }
    }

    //人工輸入頁面綁定
    protected void gvSearch_Factory_Change_Import_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtFactory_Change_Import = (DataTable)this.gvSearch_Factory_Change_Import.DataSource;
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;
            e.Row.Cells[6].Visible = false;
            e.Row.Cells[7].Visible = false;
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (0 == Convert.ToInt32(((DataTable)this.Session["Factory_Change_Import"]).Rows.Count))
                return;

            ImageButton btnButton = null;
            // 刪除的邦定事件
            btnButton = (ImageButton)e.Row.FindControl("ibtnDeleteItem");
            btnButton.CommandArgument = e.Row.RowIndex.ToString();
            if ("yes" == Convert.ToString(((DataTable)this.Session["Factory_Change_Import"]).Rows[e.Row.RowIndex]["IsSurplused"]))
            {
                btnButton.OnClientClick = "alert('已經日結不能刪除廠商異動資訊。');return false;";
            }
            else
            {
                btnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
            }

            // 修改的邦定事件
            HyperLink hl = (HyperLink)e.Row.FindControl("hlModify");
            if (dtFactory_Change_Import.Rows[e.Row.RowIndex]["Space_Short_Name"].ToString() != "")
            {
                hl.NavigateUrl = "#";
                hl.Text = "用途：" + dtFactory_Change_Import.Rows[e.Row.RowIndex]["Param_Name"].ToString() +
                            "  群組：" + dtFactory_Change_Import.Rows[e.Row.RowIndex]["Group_Name"].ToString() +
                            "  卡種：" + dtFactory_Change_Import.Rows[e.Row.RowIndex]["Space_Short_Name"].ToString();
                if ("yes" == Convert.ToString(((DataTable)this.Session["Factory_Change_Import"]).Rows[e.Row.RowIndex]["IsSurplused"]))
                {
                    hl.Attributes.Add("onclick", "alert('已經日結不能修改廠商異動資訊。');");
                }
                else
                {
                    hl.Attributes.Add("onclick", "var aa=window.showModalDialog('InOut002Detail.aspx?type=update&RowIndex=" + e.Row.RowIndex.ToString() +
                        "&Date=" + Convert.ToDateTime(e.Row.Cells[5].Text).ToString("yyyy-MM-dd") +
                        "&Param_Code=" + e.Row.Cells[6].Text +
                         "&rid=" + e.Row.Cells[7].Text +
                        "&Factory=" + e.Row.Cells[4].Text +
                        "','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){__doPostBack('btnDisFactoryChange','');}");
                }
                

            }

            // 格式化數字
            e.Row.Cells[2].Text = System.Int32.Parse(e.Row.Cells[2].Text).ToString("N0");

            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;
            e.Row.Cells[6].Visible = false;
            e.Row.Cells[7].Visible = false;
        }
    }

    #endregion
    protected void gvpbFactory_Change_Import_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[2].Text != "" && e.Row.Cells[2].Text != "&nbsp;")
                e.Row.Cells[2].Text = System.Int32.Parse(e.Row.Cells[2].Text).ToString("N0");
        }
    }
}
