using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Depository_Depository011 : PageBase
{
    Depository011BL bl = new Depository011BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbCardTypeStocksMove.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            dropToFactory.DataSource = bl.GetFactoryList();
            dropToFactory.DataBind();
            dropToFactory.Items.Insert(0, new ListItem("全部", ""));
            dropFromFactory.DataSource = bl.GetFactoryList();
            dropFromFactory.DataBind();
            dropFromFactory.Items.Insert(0, new ListItem("全部", ""));
            // 從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (SetConData())
                gvpbCardTypeStocksMove.BindData();
        }
    }

    /// <summary>
    /// 查詢卡片庫存單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbCardTypeStocksMove.BindData();
    }

    /// <summary>
    /// 新增卡片庫存單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage("今天已經日結，不可新增卡片庫存移轉 ");
            return;
        }
        Response.Redirect(string.Concat("Depository011Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion

    #region 欄位/資料補充說明
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCardTypeStocksMove_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtBeginDate", txtBeginDate.Text);
        inputs.Add("txtEndDate", txtEndDate.Text);
        inputs.Add("dropFromFactory", dropFromFactory.SelectedValue);
        inputs.Add("dropToFactory", dropToFactory.SelectedValue);
        inputs.Add("txtCardType", txtCardType.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlCardTypeStocksMove = null;

        try
        {
            dstlCardTypeStocksMove = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            
            if (dstlCardTypeStocksMove != null)//如果查到了資料
            {
                dstlCardTypeStocksMove.Tables[0].Columns.Add("Move_Date1", Type.GetType("System.String"));

                foreach (DataRow dmRow in dstlCardTypeStocksMove.Tables[0].Rows)
                {
                    // 移動日期
                    dmRow["Move_Date1"] = ((DateTime)dmRow["Move_Date"]).ToString("yyyy/MM/dd");
                }

                e.Table = dstlCardTypeStocksMove.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCardTypeStocksMove_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
    #endregion

}
