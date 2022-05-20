//******************************************************************
//*  作    者：Cuiyan Ma
//*  功能說明：卡片退貨新增
//*  創建日期：2008-09-18
//*  修改日期：2008-09-18 12:00
//*  修改記錄：
//*            □2008-09-18
//*              1.創建 馬翠艷
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;


public partial class Depository_Depository004Add : PageBase
{
    Depository004BL bizlogic = new Depository004BL();

    #region 事件處理
    /// <summary>
    /// 頁面初始化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbDepository.PageSize = GlobalStringManager.PageSize;
        
        if (!IsPostBack)
        {
            //預設為當前系統日期
            txtStock_DateFrom.Text = DateTime.Today.ToShortDateString();
            txtStock_DateTo.Text = DateTime.Today.ToShortDateString();

            txtStock_RIDYear.Focus();
            //初始化Perso廠商的資料
            DataSet dstFactoryData = bizlogic.GetFactoryData();
            dropPerso_Factory_RID.DataSource = dstFactoryData.Tables[0];
            dropPerso_Factory_RID.DataBind();
            dropPerso_Factory_RID.Items.Insert(0, new ListItem("全部", ""));
            //設定提交按鈕不可見
            btnSubmit.Visible = false;
            hidConfirm.Value = "0";
        }
    }

    /// <summary>
    /// 查詢按鈕事件，查詢資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (txtStock_RIDYear.Text != "")
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d{8}$");
            if (!reg1.IsMatch(txtStock_RIDYear.Text.Trim()))
            {
                ShowMessage("流水編號格式不對");
                return;
            }
        }
        gvpbDepository.BindData();
       
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Depository004.aspx?Con=1");
    }

    /// <summary>
    /// 提交退貨量
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {         
        //hidConfirm.Value = "0";
        Boolean flag = false;
        //int j = int.Parse(HidRowId.Value.ToString());
        int j = 0;
        DataTable dtbDepoResult = (DataTable)ViewState["Depository"];
        try
        {
            for (int i = j; i < gvpbDepository.Rows.Count; i++)
            {               
                TextBox commentBox = (TextBox)gvpbDepository.Rows[i].FindControl("txtComment");                
                TextBox NumberBox = (TextBox)gvpbDepository.Rows[i].FindControl("txtCancel_number");
                DataRow drow = dtbDepoResult.Rows[i];
                //獲取退貨量
                int cancelNumber = 0;
                if (NumberBox.Text.Trim() != "")
                {
                    cancelNumber = Convert.ToInt32(NumberBox.Text.Replace(",", ""));
                }
                drow["Cancel_Number"] = cancelNumber;
                //獲取備註信息
                drow["Comment"] = commentBox.Text.Trim();

                if (StringUtil.GetByteLength(commentBox.Text.Trim()) > 100)
                {
                    ShowMessage("備註不能超過100個字符");
                    return;
                }


                //判斷退貨量必須小於入庫量
                if (Convert.ToInt32(drow["Income_Number"].ToString()) < cancelNumber)
                {
                    ShowMessage("退貨量必須小於入庫量！");
                    return;                
                }
                if (cancelNumber > 0)
                {
                    //標誌存在不為0的行
                    flag = true;
                    //判斷當前入庫單是否已經有退貨記錄
                    Boolean contain = bizlogic.ContainsID(drow["Stock_RID"].ToString());
                     //記錄入庫單號，彈出對話框“已有退貨記錄，是否繼續?”
                    //if ((contain) && (drow["Stock_RID"].ToString() != hidStockNum.Value))
                    if (contain && hidConfirm.Value!="2")
                    {
                        hidStockNum.Value = drow["Stock_RID"].ToString();
                        hidConfirm.Value = "1";
                        HidRowId.Value= i+"";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doRepeat();", true);
                        return;
                    }                
                }
            }
            if (flag)
            {
                string reportRid = bizlogic.add(dtbDepoResult);           
                StringBuilder stbCode = new StringBuilder("<script language=\"javascript\" type=\"text/ecmascript\">window.open('Depository004Print.aspx?RID=" + reportRid + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=850,height=650');");
                stbCode.Append("window.location='Depository004Add.aspx';");
                stbCode.Append("</script>");
                this.Response.Write(stbCode.ToString());
                this.Response.End();
            }
            else {
                ShowMessage("沒有選擇入庫單退貨，請填寫需要退貨的入庫單的退貨量大於0！");            
            }
            
        }
        catch (Exception ex)
        {
            HidRowId.Value = "0";
            hidStockNum.Value = "";
            hidConfirm.Value = "0";
            ShowMessage(ex.Message); 
        }
    }
    #endregion

    #region 列表資料綁定.
    /// <summary>
    /// 資料源幫定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbDepository_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e) {
         int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtStock_RIDYear", txtStock_RIDYear.Text.Trim());
        inputs.Add("txtStock_RID1", txtStock_RID1.Text.Trim());
        inputs.Add("txtStock_RID2", txtStock_RID2.Text.Trim());
        inputs.Add("txtStock_RID3", txtStock_RID3.Text.Trim());
        inputs.Add("UctrlCardType", UctrlCardType.GetRightItem);
        inputs.Add("dropPerso_Factory_RID", dropPerso_Factory_RID.SelectedValue.Trim());
        inputs.Add("txtStock_DateFrom", txtStock_DateFrom.Text.Trim());
        inputs.Add("txtStock_DateTo", txtStock_DateTo.Text.Trim());
        DataSet dtblDepository = null;

        //保存查詢條件
        Session["Condition"] = inputs;
        try
        {
            dtblDepository = bizlogic.StockList(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            if (dtblDepository != null)//如果查到了資料
            {
                e.Table = dtblDepository.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數

                ViewState["Depository"] = dtblDepository.Tables[0];

            }
            if (intRowCount > 0)
            btnSubmit.Visible = true;
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
   
    /// <summary>
    /// 資料行幫定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbDepository_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       DataTable dtblDepository = (DataTable)gvpbDepository.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblDepository.Rows.Count == 0)
                return;

            try
            {
                //時間格式為YYYY/MM/DD
                Label lblDate = null;
                lblDate = (Label)e.Row.FindControl("lblIncome_Date");
                lblDate.Text =Convert.ToDateTime(dtblDepository.Rows[e.Row.RowIndex]["Income_Date"].ToString()).GetDateTimeFormats()[1];
                //退貨量輸入框
                TextBox boxCancel = null;
                boxCancel = (TextBox)e.Row.FindControl("txtCancel_number");
                //備註輸入框
                TextBox boxComment = null;
                boxComment = (TextBox)e.Row.FindControl("txtComment");             
                boxCancel.Text = dtblDepository.Rows[e.Row.RowIndex]["Cancel_Number"].ToString();
                boxComment.Text = dtblDepository.Rows[e.Row.RowIndex]["Comment"].ToString();
            }
            catch
            {
                return;
            }
        }

    }
     #endregion
}
