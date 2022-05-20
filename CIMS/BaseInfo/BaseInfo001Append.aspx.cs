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

public partial class BaseInfo_BaseInfo001Append : PageBase
{

    BaseInfo001BL BL = new BaseInfo001BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (ViewState["ajvBUDGET_Name"] != null)
            ajvBUDGET_Name.QueryInfo = ViewState["ajvBUDGET_Name"].ToString();
        if (!IsPostBack)
        {
            txtBUDGET_ID.Focus();

            if (Request.QueryString["type"] == "update")
            {
                ajvBUDGET_ID.Enabled = false;
                
                txtBUDGET_ID.Enabled = false;//修改時，簽呈文號不能修改

                //綁定資料
                string strRID = Request.QueryString["rid"];

                ajvBUDGET_Name.QueryInfo = "a"+strRID;
                
                DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];
                DataRow drowBudgetAppend = dtblBudgetAppend.Rows[int.Parse(strRID)];

                if (!StringUtil.IsEmpty(drowBudgetAppend["RID"].ToString()))
                    ajvBUDGET_Name.QueryInfo+=",b"+ drowBudgetAppend["RID"].ToString();
                ViewState["ajvBUDGET_Name"] = ajvBUDGET_Name.QueryInfo;
                txtBUDGET_ID.Text = drowBudgetAppend["BUDGET_ID"].ToString();
                txtBudget_Name.Text = drowBudgetAppend["Budget_Name"].ToString();
                txtCard_Price.Text = Convert.ToDecimal(drowBudgetAppend["Card_Price"]).ToString("N2");
                txtCard_Num.Text = Convert.ToInt64(drowBudgetAppend["Card_Num"]).ToString("N0");
                txtVALID_DATE_FROM.Text = Convert.ToDateTime(drowBudgetAppend["VALID_DATE_FROM"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                txtVALID_DATE_TO.Text = Convert.ToDateTime(drowBudgetAppend["VALID_DATE_TO"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                imgFileUrl.ImageUrl = drowBudgetAppend["IMG_FILE_URL"].ToString();
                HyperLink.Text = drowBudgetAppend["IMG_FILE_Name"].ToString();
                HyperLink.NavigateUrl = drowBudgetAppend["IMG_FILE_URL"].ToString();

                hdCardAmt.Value = drowBudgetAppend["Card_Price"].ToString();
                hdCardNum.Value = drowBudgetAppend["Card_Num"].ToString();
            }
        }

    }



    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (imgFileUrl.ImageUrl == "../images/NoPic.jpg")
        {
            ShowMessage("請上傳簽呈影像");
            return;
        }

        DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];
        DataRow drowBudgetAppend = null;

        if (Request.QueryString["type"] == "update")//修改
        {
            //if (Convert.ToDecimal(hdCardAmt.Value) > Convert.ToDecimal(txtCard_Price.Text.Replace(",", "")))
            //{
            //    ShowMessage("金額不能小於原有金額" + hdCardAmt.Value);
            //    return;
            //}
            //if (Convert.ToInt32(hdCardNum.Value) > Convert.ToInt32(txtCard_Num.Text.Replace(",", "")))
            //{
            //    ShowMessage("卡數不能小於原有卡數" + hdCardNum.Value);
            //    return;
            //}

            string strRID = Request.QueryString["rid"];
            drowBudgetAppend = dtblBudgetAppend.Rows[int.Parse(strRID)];
            if (!StringUtil.IsEmpty(drowBudgetAppend["rid"].ToString()))
                drowBudgetAppend["STATUS"] = "U";
        }
        else//新增
            drowBudgetAppend = dtblBudgetAppend.NewRow();

        drowBudgetAppend["BUDGET_ID"] = txtBUDGET_ID.Text;
        drowBudgetAppend["Budget_Name"] = txtBudget_Name.Text;

        drowBudgetAppend["Card_Price"] = txtCard_Price.Text.Replace(",", "");
        if (StringUtil.IsEmpty(txtCard_Num.Text))
            drowBudgetAppend["Card_Num"] = 0;
        else
            drowBudgetAppend["Card_Num"] = txtCard_Num.Text.Replace(",", "");

        drowBudgetAppend["VALID_DATE_FROM"] = txtVALID_DATE_FROM.Text;
        drowBudgetAppend["VALID_DATE_TO"] = txtVALID_DATE_TO.Text;
        drowBudgetAppend["IMG_FILE_URL"] = imgFileUrl.ImageUrl;
        drowBudgetAppend["IMG_FILE_Name"] = HyperLink.Text;
        if (Request.QueryString["type"] != "update")
        {
            drowBudgetAppend["STATUS"] = "A";
            dtblBudgetAppend.Rows.Add(drowBudgetAppend);
        }
        Session["BudgetAppend"] = dtblBudgetAppend;

        Response.Write("<script>returnValue='1';window.close();</script>");

    }
    #endregion

    #region 欄位/資料補充說明
    /// <summary>
    /// 簽呈文號（追加預算)AJAX驗證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AjaxValidator_BUDGET_ID_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        e.IsAllowSubmit = true;

        if (Session["BudgetID"] != null)
        {
            string strBudgetID = Session["BudgetID"].ToString();
            if(!StringUtil.IsEmpty(strBudgetID))
                if (e.QueryData.Trim() == strBudgetID)
                {
                    e.IsAllowSubmit = false;
                    return;
                }
        }

        e.IsAllowSubmit = !BL.ContainsBudgetID(e.QueryData.Trim());//驗證用戶是否已存在於資料庫

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
    #endregion

    #region 列表資料綁定
    #endregion

    #region 圖片檔案上傳
    /// <summary>
    /// 上傳附件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (base.FileUpload(fludFileUpload.PostedFile, imgFileUrl))
        {

            string[] str = fludFileUpload.PostedFile.FileName.Split('\\');

            if (str.Length > 0)
            {
                HyperLink.Text = str[str.Length - 1];
                HyperLink.NavigateUrl = imgFileUrl.ImageUrl;
            }
        }
    }
    #endregion
    protected void ajvBUDGET_Name_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        e.IsAllowSubmit = true;

        if (Session["BudgetName"] != null)
        {
            string strBudgetName = Session["BudgetName"].ToString();
            if (!StringUtil.IsEmpty(strBudgetName))
                if (e.QueryData.Trim() == strBudgetName)
                {
                    e.IsAllowSubmit = false;
                    return;
                }
        }
        string strRID = "";
        if (e.QueryInfo != null)
        {
            if (e.QueryInfo.Contains("b"))
                strRID = e.QueryInfo.Split(',')[1].Replace("b", "");
            else
                strRID = "";
        }
        else
            strRID = "";

        e.IsAllowSubmit = !BL.ContainsBudgetName(e.QueryData.Trim(), strRID);//驗證用戶是否已存在於資料庫

        if (e.IsAllowSubmit == false)
            return;

        DataTable dtblBudgetAppend = (DataTable)Session["BudgetAppend"];

        if (e.QueryInfo != null)
        {
            if (e.QueryInfo.Contains("a"))
                strRID = e.QueryInfo.Split(',')[0].Replace("a", "");
            else
                strRID = "";
        }
        else
            strRID = "";


        for(int i=0;i<dtblBudgetAppend.Rows.Count;i++)
        {
            if(!StringUtil.IsEmpty(strRID))
            {
                if (strRID == i.ToString())
                    continue;
            }
            if (dtblBudgetAppend.Rows[i]["BUDGET_Name"].ToString() == e.QueryData)
            {
                e.IsAllowSubmit = false;
                return;
            }
        }
    }
}
