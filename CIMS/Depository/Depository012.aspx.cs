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
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

public partial class Depository_Depository012 : PageBase
{
    Depository012BL bizlogic = new Depository012BL();
    DataTable temp = new DataTable();
    DataTable excelTable = new DataTable();
    #region 事件處理
    /// <summary>
    /// 頁面初始化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbCalculate.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            Session.Remove("htFactory");
            dropPerso.DataSource = bizlogic.GetFactoryList();
            dropPerso.DataBind();
            dropPerso.Items.Insert(0, new ListItem("全部", ""));    
            if (SetConData())
                gvpbCalculate.BindData();

            btnReport.Visible = false;
            btnSubmit.Visible = false;
        }
    }

    /// <summary>
    /// 計算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        try
        {
            
            string checkDate = txtCheckDate.Text;
            //判斷是否為工作日
            if (!bizlogic.isWorkDay(checkDate))
            {
                ShowMessage("當日非工作日！");
                return;
            }
            //判斷是否為日結日
            if (!bizlogic.isCheckDate(checkDate))
            {
                ShowMessage("當日未日結！");
                return;
            }
            if (bizlogic.getSafeParam(GlobalString.cardparam.SafeMonth) == "erro")
            {
                ShowMessage("沒有設定安全庫存月數");
                return;
            }
            if (bizlogic.getSafeParam(GlobalString.cardparam.Percent) == "erro")
            {
                ShowMessage("沒有設定換卡百分比");
                return;
            }
            DataTable dtbSafeTable = new DataTable();
            dtbSafeTable.Columns.Add("Perso_RID");
            dtbSafeTable.Columns.Add("Card_RID");
            dtbSafeTable.Columns.Add("Number");
            DataTable table = (DataTable)ViewState["safeTable"];
            if (gvpbCalculate != null)
            {
                for (int i = 0; i < gvpbCalculate.Rows.Count; i++)
                {
                    TextBox number_TXT = (TextBox)gvpbCalculate.Rows[i].FindControl("txtBuy_number");
                    DataRow drow = dtbSafeTable.NewRow();
                    drow["Perso_RID"] = table.Rows[i]["perso_Rid"];
                    drow["Card_RID"] = table.Rows[i]["cardType_Rid"];
                    drow["Number"] = number_TXT.Text.Replace(",", "");
                    if (StringUtil.IsEmpty(number_TXT.Text.Trim()) || Convert.ToInt32(number_TXT.Text.Replace(",", "")) == 0)
                    {
                        drow["Number"] = 0;
                    }                             
                    dtbSafeTable.Rows.Add(drow);                           
                }
            }
            ViewState["safeTable"] = dtbSafeTable;
            //bizlogic.save(dtbSafeTable, checkDate);     
            gvpbCalculate.BindData();
        }catch(Exception ex){
            ShowMessage(ex.Message);        
        }
    }

    /// <summary>
    /// 匯出Excel格式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        DataTable safeTable = (DataTable)ViewState["safeTable"];    
        if (gvpbCalculate != null)
        {
            for (int i = 0; i < gvpbCalculate.Rows.Count; i++)
            {
                TextBox number_TXT = (TextBox)gvpbCalculate.Rows[i].FindControl("txtBuy_number");              
                safeTable.Rows[i]["Buy_Num"] = number_TXT.Text;
            }
        }
        try
        {
            if (safeTable == null)
            {
                ShowMessage("無匯出信息");
                return;
            }
            string TimeMark = DateTime.Now.ToString("yyyyMMddHHmmss");
            bizlogic.inputDateTotemp(safeTable, TimeMark);
            Response.Write("<script>window.open('Depository012Print.aspx?time=" + TimeMark + "');</script>");
        }catch(Exception ex){
            ShowMessage(ex.Message);        
        }      

    }

    /// <summary>
    /// 下單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_click(object sender, EventArgs e)
    {
        temp.Columns.Clear();
        temp.Columns.Add("Pass_Status");
        temp.Columns.Add("orderform_rid");
        temp.Columns.Add("orderform_detail_rid");
        temp.Columns.Add("Space_Short_RID");//cardtype_rid
        temp.Columns.Add("number");
        temp.Columns.Add("Agreement_RID");
        temp.Columns.Add("Agreement_Name");
        temp.Columns.Add("Budget_RID");
        temp.Columns.Add("Budget_Name");
        temp.Columns.Add("factory");
        temp.Columns.Add("factory_id");
        temp.Columns.Add("Base_Price");
        temp.Columns.Add("Fore_Delivery_Date");
        temp.Columns.Add("Wafer_RID");
        temp.Columns.Add("Wafer_Name");
        temp.Columns.Add("Wafer_Capacity");
        temp.Columns.Add("is_exigence");
        temp.Columns.Add("Delivery_Address_RID");
        temp.Columns.Add("factory_shortname_cn");
        temp.Columns.Add("comment");
        temp.Columns.Add("bz");
        //add Ian Huang start
        temp.Columns.Add("Change_UnitPrice");
        //add Ian Huang end
        DataTable table = (DataTable)ViewState["safeTable"];
        if (gvpbCalculate != null)
        {
            for (int i = 0; i < gvpbCalculate.Rows.Count; i++)
            {
                CheckBox check_buy = (CheckBox)gvpbCalculate.Rows[i].FindControl("checkBuy");
                TextBox number_TXT = (TextBox)gvpbCalculate.Rows[i].FindControl("txtBuy_number");
                if (check_buy.Checked && number_TXT.Text.Replace(",", "") != "" && number_TXT.Text.Replace(",", "") != "0")
                {
                    DataRow drow = temp.NewRow();
                    drow["Delivery_Address_RID"] = table.Rows[i]["perso_Rid"];
                    drow["factory_shortname_cn"] = table.Rows[i]["persoName"];
                    drow["Space_Short_RID"] = table.Rows[i]["cardType_Rid"];
                    drow["number"] = number_TXT.Text.Replace(",", "");
                    drow["bz"] = 2;
                    temp.Rows.Add(drow);
                }
            }
            Session["monitory"] = temp;
        }
        if (temp.Rows.Count < 1)
        {
            ShowMessage("沒有選擇任何卡種下單！");
            return;
        }
        if (temp.Rows.Count > 99)
        {
            ShowMessage("超過最大下單量！");
            return;
        }
        Response.Redirect("Depository002Add.aspx?type=1");
    }
    #endregion

    #region 列表資料綁定.
    /// <summary>
    /// 資料源幫定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCalculate_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {         
        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtCheckDate", txtCheckDate.Text);
        inputs.Add("dropPerso", dropPerso.SelectedValue);
        inputs.Add("txtCardType", txtCardtype.Text);
        inputs.Add("txtAffinity", txtAffinity.Text);
        inputs.Add("txtPhoto", txtPhoto.Text);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("txtMonth", txtMonth.Text);
        inputs.Add("radSaveType",radSaveType.SelectedValue);

        DataTable dstlCalculate = null;
        DataTable viewTable = (DataTable)ViewState["safeTable"];
        try
        {
            dstlCalculate = bizlogic.List(inputs, viewTable);
            dstlCalculate.Columns.Add("rowId", Type.GetType(" System.Int32"));
            ViewState["safeTable"] = dstlCalculate;

            if (dstlCalculate.Rows.Count > 0)
            {
                btnReport.Visible = true;
                btnSubmit.Visible = true;
            }
            else
            {
                btnReport.Visible = false;
                btnSubmit.Visible = false;
            }
           
            if (dstlCalculate != null)//如果查到了資料
            {
                for (int i = 1; i <= dstlCalculate.Rows.Count; i++)
                {
                    dstlCalculate.Rows[i - 1]["rowId"] = i;
                }
                e.Table = dstlCalculate;//要綁定的資料表
                e.RowCount = dstlCalculate.Rows.Count;//查到的行數 
            }
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
    protected void gvpbCalculate_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblCalculate = (DataTable)gvpbCalculate.DataSource;        
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblCalculate.Rows.Count == 0)
                return;
            TextBox txtBuyNum = (TextBox)e.Row.FindControl("txtBuy_number");
            txtBuyNum.Text = dtblCalculate.Rows[e.Row.RowIndex]["Buy_Num"].ToString();
            decimal expectMonth = Convert.ToDecimal(dtblCalculate.Rows[e.Row.RowIndex]["expectMonth_Today"]);
            if(radSaveType.SelectedValue == "1")
                expectMonth = Convert.ToDecimal(dtblCalculate.Rows[e.Row.RowIndex]["expectMonth_Ten"]);
            string safeMonth = bizlogic.getSafeParam(GlobalString.cardparam.SafeMonth);
            if (expectMonth < Convert.ToDecimal(safeMonth.Substring(0,safeMonth.Length - 1)))
            {
                e.Row.ForeColor = System.Drawing.Color.Red;                            
            }
            for (int cellNumber = 7; cellNumber < 16; cellNumber++) {
                if (e.Row.Cells[cellNumber].Text != "&nbsp;")
                {
                    e.Row.Cells[cellNumber].Text = Convert.ToInt32(e.Row.Cells[cellNumber].Text).ToString("N0");
                }
            }
               
            if (e.Row.Cells[17].Text != "&nbsp;")
            {
                e.Row.Cells[17].Text = Convert.ToDecimal(e.Row.Cells[17].Text).ToString("N1");
            }
            if (e.Row.Cells[18].Text != "&nbsp;")
            {
                e.Row.Cells[18].Text = Convert.ToDecimal(e.Row.Cells[18].Text).ToString("N1");
            }  

        }
    }
    #endregion
 }
