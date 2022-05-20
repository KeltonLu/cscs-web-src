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
public partial class Finance_Finance0012 : PageBase
{
    Finance0012BL Finance0012BL = new Finance0012BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbSAP.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //請款日期(起)和請款日期(迄)都設為當天
            //this.txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
            //this.txtFinish_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
            dropBlankFactoryBind();//空白卡廠下拉框綁定

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                }
                gvpbSAP.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }

    /// <summary>
    /// 空白卡廠下拉框綁定
    /// </summary>
    protected void dropBlankFactoryBind()
    {
        dropBlankFactory.Items.Clear();

        dropBlankFactory.DataTextField = "Factory_ShortName_CN";
        dropBlankFactory.DataValueField = "RID";
        dropBlankFactory.DataSource = Finance0012BL.getFactory();
        dropBlankFactory.DataBind();

        dropBlankFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gvpbSAP.BindData();
    }
    protected void gvpbSAP_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (dropBlankFactory.SelectedItem.Text != "全部")
        {
            inputs.Add("dropBlankFactory", dropBlankFactory.SelectedValue);
        }
        else
        {
            inputs.Add("dropBlankFactory", "");
        }
       
        inputs.Add("txtInvoiceNumber", txtInvoiceNumber.Text);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("txtSAP_Serial_Number", txtSAP_Serial_Number.Text.Trim());
        inputs.Add("txtBegin_Date", txtBegin_Date.Text);
        inputs.Add("txtFinish_Date", txtFinish_Date.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dsSAP = null;

        try
        {
            dsSAP = Finance0012BL.SearchSAP(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dsSAP != null)//如果查到了資料
            {
                e.Table = dsSAP.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
    protected void gvpbSAP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[5].Text.Trim() != "" && e.Row.Cells[5].Text.Trim() != "&nbsp;")
            {
                if (Convert.ToDateTime(e.Row.Cells[5].Text).ToString("yyyy/MM/dd") == "1900/01/01")
                {
                    e.Row.Cells[5].Text = "";
                }
            }
        }
    }
}
