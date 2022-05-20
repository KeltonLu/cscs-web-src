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
public partial class Finance_Finance0011 : PageBase
{
    Finance0011BL Finance0011BL = new Finance0011BL();
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
        dropBlankFactory.DataSource = Finance0011BL.getFactory();
        dropBlankFactory.DataBind();

        dropBlankFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 增加空白卡請款
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0011Add.aspx?ActionType=Add");
    }

    /// <summary>
    /// 查詢空白卡請款情況
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.gvpbSAP.BindData();
    }

    /// <summary>
    /// 綁定空白卡請款記錄
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbSAP_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropBlankFactory", dropBlankFactory.SelectedValue);
        inputs.Add("txtInvoiceNumber", txtInvoiceNumber.Text);
        inputs.Add("dropPass_Status", dropPass_Status.SelectedValue);
        inputs.Add("txtSAP_Serial_Number", txtSAP_Serial_Number.Text.Trim());
        inputs.Add("txtBegin_Date", txtBegin_Date.Text);
        inputs.Add("txtFinish_Date", txtFinish_Date.Text);
        inputs.Add("dropIs_Finance", dropIs_Finance.SelectedValue);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dsSAP = null;

        try
        {
            this.Session["sapCount"] = 0;
            dsSAP = Finance0011BL.SearchSAP(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            if (dsSAP != null)//如果查到了資料
            {
                e.Table = dsSAP.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
                this.Session["sapCount"] = e.RowCount;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    /// <summary>
    /// 空白卡請款記錄行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbSAP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (int.Parse(this.Session["sapCount"].ToString()) == 0)
                return;

            // 請款日期
            e.Row.Cells[3].Text = Convert.ToDateTime(e.Row.Cells[3].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            switch (e.Row.Cells[4].Text.Trim())
            {
                case "1":
                    e.Row.Cells[4].Text = "暫存";
                    break;
                case "2":
                    e.Row.Cells[4].Text = "退回";
                    break;
                case "3":
                    e.Row.Cells[4].Text = "待放行";
                    break;
                case "4":
                    e.Row.Cells[4].Text = "已放行";
                    break;
                default:
                    break;
            }

            switch (e.Row.Cells[5].Text.Trim())
            {
                case "Y":
                    e.Row.Cells[5].Text = "已出帳";
                    break;
                case "N":
                    e.Row.Cells[5].Text = "未出帳";
                    break;
                default:
                    break;
            }
        }
    }
}
