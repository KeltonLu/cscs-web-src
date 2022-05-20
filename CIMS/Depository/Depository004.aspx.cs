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

public partial class Depository_Depository004 : PageBase
{
    Depository004BL bizlogic = new Depository004BL();
    CardTypeManager cardManage = new CardTypeManager();
   
    protected void Page_Load(object sender, EventArgs e)
    {               
        this.gvpbCancel.PageSize=GlobalStringManager.PageSize;
        
       
        if (!IsPostBack)
        {
            //預設為當前系統日期
            txtCancel_DateFrom.Text = DateTime.Today.ToShortDateString();
            txtCancel_DateTo.Text = DateTime.Today.ToShortDateString();

            txtCancel_RIDYear.Focus();

            DataSet dstFactoryData = bizlogic.GetFactoryData();


            dropPerso_Factory_Search.DataSource = dstFactoryData.Tables[0];
            dropPerso_Factory_Search.DataBind();
            dropPerso_Factory_Search.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();                   
                }
                if(txtCancel_DateTo.Text!="")
                txtCancel_DateTo.Text = Convert.ToDateTime(txtCancel_DateTo.Text).ToShortDateString().ToString();
                gvpbCancel.BindData();
            }
            else
                Session.Remove("Condition");
        }

    }
    
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbCancel.BindData();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage("今天已經日結，不可新增退貨信息");
            return;
        }
            Response.Redirect(string.Concat("Depository004Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }

     #region 列表資料綁定.
    protected void gvpbCancel_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtCancel_RIDYear", txtCancel_RIDYear.Text.Trim());
        inputs.Add("txtCancel_RID1", txtCancel_RID1.Text.Trim());
        inputs.Add("txtCancel_RID2", txtCancel_RID2.Text.Trim());
        inputs.Add("txtCancel_RID3", txtCancel_RID3.Text.Trim());
        inputs.Add("dropPerso_Factory_RID", dropPerso_Factory_Search.SelectedValue.Trim());
        inputs.Add("txtCancel_DateFrom", txtCancel_DateFrom.Text.Trim());
        inputs.Add("txtCancel_DateTo", txtCancel_DateTo.Text.Trim());


        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlCancel = null;

        try
        {
            dstlCancel = bizlogic.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlCancel != null)//如果查到了資料
            {
                e.Table = dstlCancel.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void gvpbCancel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblCancel = (DataTable)gvpbCancel.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblCancel.Rows.Count == 0)
                return;

            try
            {
                HyperLink hlReportID = null;
                Label lblDate = null;                
                hlReportID = (HyperLink)e.Row.FindControl("hlReport_RID");
                hlReportID.Text = dtblCancel.Rows[e.Row.RowIndex]["Report_RID"].ToString();
                hlReportID.NavigateUrl = "#";
                hlReportID.Attributes.Add("onclick", "window.open('Depository004Print.aspx?RID=" + dtblCancel.Rows[e.Row.RowIndex]["Report_RID"].ToString() + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=850,height=650');");
                lblDate = (Label)e.Row.FindControl("lblCancel_Date");
                lblDate.Text = Convert.ToDateTime(dtblCancel.Rows[e.Row.RowIndex]["Cancel_Date"].ToString()).GetDateTimeFormats()[1];
                
            }
            catch
            {
                return;
            }
        }
    }

#endregion
}
