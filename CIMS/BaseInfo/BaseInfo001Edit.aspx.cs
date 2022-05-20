using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class BaseInfo_BaseInfo001Edit : PageBase
{
    BaseInfo001BL bmManager = new BaseInfo001BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ViewState["ajvBUDGET_Name"] != null)
            ajvBudgetName.QueryInfo = ViewState["ajvBUDGET_Name"].ToString();

        if (!IsPostBack)
        {
            txtBUDGET_ID.Focus();

            string strRID = Request.QueryString["RID"];

            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strRID))
            {

                CARD_BUDGET cbModel = bmManager.GetBudget(strRID);

                //設置控件的值
                SetControls(cbModel);

                HyperLink.Text = cbModel.IMG_File_Name;
                HyperLink.NavigateUrl = cbModel.IMG_File_URL;

                txtCard_Price.Text = cbModel.Card_Price.ToString("N2");
                txtCard_Num.Text = cbModel.Card_Num.ToString("N0");
                txtTOTAL_CARD_AMT.Text = cbModel.Total_Card_AMT.ToString("N2");
                txtTOTAL_CARD_NUM.Text = cbModel.Total_Card_Num.ToString("N0");

                hdCardAmt.Value = cbModel.Card_Price.ToString();
                hdCardNum.Value = cbModel.Card_Num.ToString();

                //載入預算附加信息
                DataSet dstBudgetInfo = bmManager.LoadBudgetInfoByRID(strRID);

                Session["BudgetAppend"] = dstBudgetInfo.Tables[0];
                //記錄刪除追加預算
                ViewState["BudgetAppend_del"] = "";

                if (dstBudgetInfo.Tables[0].Rows.Count == 0)
                {
                    //chkDel.Enabled  = true;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(false);", true);
                }

                else
                    gvpbBudgetAppend.BindData();

                //卡種
                UctrlCardType.SetRightItem = dstBudgetInfo.Tables[1];

                //製域處理
                trResult.Visible = true;
                ajvBudgetID.Enabled = false;
                //ajvBudgetName.Enabled = false;
                ajvBudgetName.QueryInfo = strRID;
                ViewState["ajvBUDGET_Name"] = ajvBudgetName.QueryInfo;
                txtBUDGET_ID.Enabled = false;
                lbTitle.Text = "預算簽呈修改/刪除";
            }
            else //無值，代表是新增！
            {
                //製域處理
                trResult.Visible = false;
                Session["BudgetAppend"] = bmManager.GetBudgetAppend("-1").Tables[0];
                lbTitle.Text = "預算簽呈新增";
            }
        }

        //計算金額卡數
        this.CalTotalNum();

        UctrlCardType.Is_Using = true;

    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];

        if (((DataTable)Session["BudgetAppend"]).Rows.Count == 0)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(false);", true);

        if (!chkDel.Checked)
        {
            if (StringUtil.GetByteLength(txtReason.Text) > 1000)
            {
                ShowMessage("異動原因不能超過1000個字符");
                return;
            }

            if (imgIMG_FILE_URL.ImageUrl == "../images/NoPic.jpg")
            {
                ShowMessage("請上傳簽呈影像");
                return;
            }

            if (!StringUtil.IsEmpty(strRID))
            {
                if (hdCardNum.Value == "0")
                {
                    if (!StringUtil.IsEmpty(txtCard_Num.Text))
                    {
                        if (txtCard_Num.Text != "0")
                        {
                            ShowMessage("卡數初始化為0，不能修改為其它值");
                            return;
                        }
                    }
                }
                else
                {
                    if (StringUtil.IsEmpty(txtCard_Num.Text))
                    {
                        ShowMessage("卡數不能為空");
                        return;
                    }
                    if (txtCard_Num.Text == "0")
                    {
                        ShowMessage("卡數初始化大於0，修改必須大於0");
                        return;
                    }
                }
            }

            if (txtCard_Num.Text == "" || txtCard_Num.Text == "0")
            {
                foreach (DataRow drowAppendBudget in ((DataTable)Session["BudgetAppend"]).Rows)
                {
                    if (drowAppendBudget["Card_Num"].ToString() != "0")
                    {
                        ShowMessage("主預算卡數為0，追加預算時卡數必須為0");
                        return;
                    }
                }
            }
            else
            {
                foreach (DataRow drowAppendBudget in ((DataTable)Session["BudgetAppend"]).Rows)
                {
                    if (drowAppendBudget["Card_Num"].ToString() == "0")
                    {
                        ShowMessage("主預算卡數不為0，追加預算卡數不能為0");
                        return;
                    }
                }
            }


            if (UctrlCardType.GetRightItem.Rows.Count == 0)
            {
                ShowMessage("請選擇卡種");
                return;
            }

            

            //計算金額卡數
            this.CalTotalNum();

            txtCard_Price.Text = txtCard_Price.Text.Replace(",", "");
            txtCard_Num.Text = txtCard_Num.Text.Replace(",", "");
            txtTOTAL_CARD_AMT.Text = txtTOTAL_CARD_AMT.Text.Replace(",", "");
            txtTOTAL_CARD_NUM.Text = txtTOTAL_CARD_NUM.Text.Replace(",", "");
        }
        try
        {
            if (StringUtil.IsEmpty(strRID))    //增加
            {
                CARD_BUDGET cbModel = new CARD_BUDGET();
                SetData(cbModel);

                cbModel.IMG_File_Name = HyperLink.Text;

                bmManager.Add(cbModel, (DataTable)Session["BudgetAppend"], UctrlCardType.GetRightItem);

                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo001Edit.aspx");
            }
            else
            {
                string strAppendDel = ViewState["BudgetAppend_del"].ToString();
                if (chkDel.Checked)         //刪除
                {
                    bmManager.Delete(strRID, strAppendDel, txtReason.Text);

                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "BaseInfo001.aspx?Con=1");
                }
                else                        //修改 
                {
                    //if (Convert.ToDecimal(hdCardAmt.Value) > Convert.ToDecimal(txtCard_Price.Text))
                    //    throw new Exception("金額不能小於原有金額" + hdCardAmt.Value);
                    //if (Convert.ToInt32(hdCardNum.Value) > Convert.ToInt32(txtCard_Num.Text))
                    //    throw new Exception("卡數不能小於原有卡數" + hdCardNum.Value);

                    


                    CARD_BUDGET cbModel = new CARD_BUDGET();
                    cbModel.RID = int.Parse(strRID);
                    SetData(cbModel);

                    cbModel.IMG_File_Name = HyperLink.Text;

                    bmManager.Update(cbModel, (DataTable)Session["BudgetAppend"], UctrlCardType.GetRightItem, strAppendDel);

                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo001.aspx?Con=1");
                }

            }

            Session.Remove("BudgetAppend");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);

            if(((DataTable)Session["BudgetAppend"]).Rows.Count==0)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(false);", true);

            txtCard_Price.Text = Convert.ToDecimal(txtCard_Price.Text.Replace(",","")).ToString("N2");
            if (!StringUtil.IsEmpty(txtCard_Num.Text))
                txtCard_Num.Text = Convert.ToInt32(txtCard_Num.Text.Replace(",","")).ToString("N0");
            txtTOTAL_CARD_AMT.Text = Convert.ToDecimal(txtTOTAL_CARD_AMT.Text).ToString("N2");
            if (!StringUtil.IsEmpty(txtTOTAL_CARD_NUM.Text))
                txtTOTAL_CARD_NUM.Text = Convert.ToInt32(txtTOTAL_CARD_NUM.Text.Replace(",","")).ToString("N0");
        }
    }


   

    /// <summary>
    /// 彈出追加預算畫面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBudgetAppend_Click(object sender, EventArgs e)
    {
        this.CalTotalNum();
        gvpbBudgetAppend.BindData();
    }

    /// <summary>
    /// 刪除追加預算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];
        DataRow drowBudgetAppend = dtblBudgetAppend.Rows[Convert.ToInt32(e.CommandArgument)];
        if (!StringUtil.IsEmpty(drowBudgetAppend["RID"].ToString()))
        {
            ViewState["BudgetAppend_del"] += drowBudgetAppend["RID"].ToString() + ",";
            try
            {
                bmManager.ChkDelBudget(drowBudgetAppend["RID"].ToString());
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                return;
            }
        }

        // Legend 2017/02/16 將此方法注釋, 因計算總金額與總卡數, 計算2遍
        //ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "CalDelNum(" + drowBudgetAppend["Card_Price"].ToString() + "," + drowBudgetAppend["Card_Num"].ToString() + ");", true);

        dtblBudgetAppend.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));


        Session["BudgetAppend"] = dtblBudgetAppend;

        gvpbBudgetAppend.BindData();
    }

    /// <summary>
    /// 修改追加預算
    /// Legend 2017/02/04 添加
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ibtnUpdate_Command(object sender, CommandEventArgs e)
    {  
        try
        {
            string strActionType = Request.QueryString["ActionType"];

            string strUrl = "var aa=window.showModalDialog('BaseInfo001Append.aspx?ActionType=" + strActionType + "&type=update&rid=" + e.CommandArgument + "');if(aa!=undefined){AddBudgetAppend();}";

            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), strUrl, true);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
            return;
        }
    }

    #endregion

    #region 欄位/資料補充說明
    /// <summary>
    /// 簽呈文號,AJAX驗證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AjaxValidator1_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        e.IsAllowSubmit = !bmManager.ContainsBudgetID(e.QueryData.Trim());//驗證用戶是否已存在於資料庫

        if (e.IsAllowSubmit == false)
            return;

        DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];

        foreach (DataRow drowBudgetAppend in dtblBudgetAppend.Rows)
        {

            if (drowBudgetAppend["BUDGET_ID"].ToString() == e.QueryData)
            {
                e.IsAllowSubmit = false;
                return;
            }
        }
    }

    /// <summary>
    /// 計算總金額及卡數
    /// </summary>
    protected void CalTotalNum()
    {
        decimal decTotalAMT = 0.00M;
        long intCardNum = 0;

        DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];

        foreach (DataRow drowBudgetAppend in dtblBudgetAppend.Rows)
        {
            if (!StringUtil.IsEmpty(drowBudgetAppend["Card_Price"].ToString()))
                decTotalAMT += Convert.ToDecimal(drowBudgetAppend["Card_Price"].ToString());

            if (!StringUtil.IsEmpty(drowBudgetAppend["Card_Num"].ToString()))
                intCardNum += Convert.ToInt64(drowBudgetAppend["Card_Num"].ToString());
        }

        hdAppCardAmt.Value = decTotalAMT.ToString();
        hdAppCardNum.Value = intCardNum.ToString();

        if (!StringUtil.IsEmpty(txtCard_Price.Text))
        {
            try
            {
                decTotalAMT += Convert.ToDecimal(txtCard_Price.Text.Replace(",", ""));
            }
            catch { }
        }
        if (!StringUtil.IsEmpty(txtCard_Num.Text))
        {
            try
            {
                intCardNum += Convert.ToInt64(txtCard_Num.Text.Replace(",", ""));
            }
            catch { }
        }

        txtTOTAL_CARD_AMT.Text = decTotalAMT.ToString("N2");
        txtTOTAL_CARD_NUM.Text = intCardNum.ToString("N0");
    }
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// 追加預算綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbBudgetAppend_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];

        if (dtblBudgetAppend.Rows.Count > 0)
            if (StringUtil.IsEmpty(dtblBudgetAppend.Rows[0][1].ToString()))
                dtblBudgetAppend.Rows.RemoveAt(0);

        if (dtblBudgetAppend.Rows.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(true);", true);
        }

        Session["BudgetAppend"] = dtblBudgetAppend;

        e.Table = dtblBudgetAppend;
        e.RowCount = dtblBudgetAppend.Rows.Count;

       

        CalTotalNum();
    }

    /// <summary>
    /// 追加預算行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbBudgetAppend_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblBudgetAppend = (DataTable)gvpbBudgetAppend.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            if (dtblBudgetAppend.Rows.Count == 0)
                return;

            ImageButton ibtnButton = null;
            ImageButton ibtnUpdButton = null;
            //Image img = null;
            HyperLink hlinkImg = null; 

            // 刪除的邦定事件
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            // 修改的邦定事件
            ibtnUpdButton = (ImageButton)e.Row.FindControl("ibtnUpdate");
            //string strActionType = Request.QueryString["ActionType"];
            ibtnUpdButton.CommandArgument = e.Row.RowIndex.ToString();

            //ibtnUpdButton.Attributes.Add("onclick", "var aa=window.showModalDialog('BaseInfo001Append.aspx?ActionType=" + strActionType + "&type=update&rid=" + e.Row.RowIndex.ToString() + "');if(aa!=undefined){AddBudgetAppend();}");

            //圖像綁定
            hlinkImg = (HyperLink)e.Row.FindControl("hlinkImg");
            hlinkImg.NavigateUrl = dtblBudgetAppend.Rows[e.Row.RowIndex]["IMG_FILE_URL"].ToString();
            hlinkImg.Text = dtblBudgetAppend.Rows[e.Row.RowIndex]["IMG_FILE_Name"].ToString();
            hlinkImg.Target = "_blank";

            if (e.Row.Cells[4].Text != "&nbsp;")
            {
                e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                e.Row.Cells[5].Text = Convert.ToDateTime(e.Row.Cells[5].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }


        }
    }

    #endregion

    #region 圖片檔案上傳
    /// <summary>
    /// 上傳
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (FileUpload(fludFileUpload.PostedFile, imgIMG_FILE_URL))
        {

            string[] str = fludFileUpload.PostedFile.FileName.Split('\\');

            if (str.Length > 0)
            {
                HyperLink.Text = str[str.Length - 1];
                HyperLink.NavigateUrl = imgIMG_FILE_URL.ImageUrl;
            }
        }
    }
    #endregion

    protected void btnAddBudget_Click(object sender, EventArgs e)
    {
        string strActionType = Request.QueryString["ActionType"];

        Session["BudgetID"]=txtBUDGET_ID.Text.Trim();

        Session["BudgetName"] = txtBUDGET_NAME.Text.Trim();

        string strUrl = "var aa=window.showModalDialog('BaseInfo001Append.aspx?ActionType=" + strActionType +"&type=add','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){AddBudgetAppend();}";

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), strUrl, true);
    }
    protected void ajvBudgetName_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        e.IsAllowSubmit = !bmManager.ContainsBudgetName(e.QueryData.Trim(),e.QueryInfo);//驗證用戶是否已存在於資料庫

        if (e.IsAllowSubmit == false)
            return;

        DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];

        foreach (DataRow drowBudgetAppend in dtblBudgetAppend.Rows)
        {

            if (drowBudgetAppend["BUDGET_Name"].ToString() == e.QueryData)
            {
                e.IsAllowSubmit = false;
                return;
            }
        }
    }
}
