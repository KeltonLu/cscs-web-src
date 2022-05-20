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
using System.Text;
using System.Collections.Generic;

public partial class CardType_CardType002 : PageBase
{
    CardType002BL blManager = new CardType002BL();
 
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbCardType.PageSize = GlobalStringManager.PageSize;
        if (!Page.IsPostBack)
        {
            // 绑定卡種群組下拉框
            DataSet dstCardTypeGroup = null;
            dstCardTypeGroup = blManager.GetCardTypeGroup();
            if (null != dstCardTypeGroup)
            {
                this.dropCardType_Group_RID.DataTextField = "Group_Name";
                this.dropCardType_Group_RID.DataValueField = "RID";
                this.dropCardType_Group_RID.DataSource = dstCardTypeGroup.Tables[0];
                this.dropCardType_Group_RID.DataBind();
                dropCardType_Group_RID.Items.Insert(0, new ListItem("全部", ""));
            }

            // 從Seesion中獲取已保存的查詢條件
            if (SetConData())
                gvpbCardType.BindData();
        }
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbCardType.BindData();
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("CardType002Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }

    /// <summary>
    /// 設定資料源
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCardType_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropCardType_Group_RID", dropCardType_Group_RID.SelectedValue);
        inputs.Add("txtTYPE", txtTYPE.Text);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("dropUseType", dropUseType.SelectedValue);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstCardType = null;

        try
        {
            dstCardType =blManager.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            if (dstCardType != null)//如果查到了資料
            {
                e.Table = dstCardType.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void gvpbCardType_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblCardType = (DataTable)gvpbCardType.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblCardType.Rows.Count == 0)
                return;

            DataSet dst = blManager.GetCardTypeGroup2(gvpbCardType.DataKeys[e.Row.RowIndex].Value.ToString());

            Label lbGroup_Name = (Label)e.Row.FindControl("lbGroup_Name");

            foreach(DataRow drow in dst.Tables[0].Rows)
            {
                lbGroup_Name.Text += drow["group_name"].ToString()+"<br>";
            }

        }
    }
    protected void btnExportExcel_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "window.open('CardType002Report.aspx','_blank');", true);
    }
}
