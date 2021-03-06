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


public partial class CardType_CardType007 : PageBase
{
    CardType007BL bizLogic = new CardType007BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbCardType.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //取得類別和批次下拉bar中的内容
            DataSet dstDrop = bizLogic.GetTypeAndMakeCardType();
            dropType.DataSource = dstDrop.Tables[0];
            dropType.DataBind();
            dropType.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            dropMakeCardType_RID.DataSource = dstDrop.Tables[1];
            dropMakeCardType_RID.DataBind();
            dropMakeCardType_RID.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                SetConData();
                gvpbCardType.BindData();
            }

        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbCardType.BindData();
    }


    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("CardType007Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }


    protected void gvpbCardType_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropType", dropType.SelectedValue);
        inputs.Add("dropMakeCardType_RID", dropMakeCardType_RID.SelectedValue);
        //保存查詢條件
        Session["Condition"] = inputs;
        DataSet dstlCardType = null;
        try
        {
            dstlCardType = bizLogic.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlCardType != null)//如果查到了資料
            {
                e.Table = dstlCardType.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }

    } 
}
