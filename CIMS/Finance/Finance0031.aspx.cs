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
public partial class Finance_Finance0031 : PageBase
{
    Finance0031BL Finance0031BL = new Finance0031BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbMATERIEL_SAP.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                    gvpbMATERIEL_SAP.BindData();
                }
            }
            else
                Session.Remove("Condition");
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbMATERIEL_SAP.BindData();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0031Add.aspx?ActionType=Add");
    }
    protected void gvpbMATERIEL_SAP_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropAsk_Project", dropAsk_Project.SelectedItem.Text);
        inputs.Add("txtSAP_Serial_Number", txtSAP_Serial_Number.Text.Trim());
        inputs.Add("txtBegin_Date1", txtBegin_Date1.Text);
        inputs.Add("txtFinish_Date1", txtFinish_Date1.Text);
        inputs.Add("txtBegin_Date2", txtBegin_Date2.Text);
        inputs.Add("txtFinish_Date2", txtFinish_Date2.Text);
        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dsMATERIEL_SAP = null;
        try
        {
            dsMATERIEL_SAP = Finance0031BL.SearchSAP(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dsMATERIEL_SAP != null)//如果查到了資料
            {
                e.Table = dsMATERIEL_SAP.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
    protected void gvpbMATERIEL_SAP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[0].Text != "&nbsp;")
            {
                e.Row.Cells[0].Text = Convert.ToDateTime(e.Row.Cells[0].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            }
            if (e.Row.Cells[0].Text != "&nbsp;")
            {
                e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01","");
            }
        }
    }
}
