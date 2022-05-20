//******************************************************************
//*  作    者：wangxiaoyan
//*  功能說明：製卡類別設定頁面
//*  創建日期：2008-08-28
//*  修改日期： 
//*  修改記錄：

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

public partial class CardType_CardType006 : PageBase
{
    CardType006BL bizLogic = new CardType006BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbCardType.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            dropGroup_Name.DataSource = bizLogic.GetCardTypeGroup().Tables[0];
            dropGroup_Name.DataBind();
            dropGroup_Name.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];       
            if (!StringUtil.IsEmpty(strCon))
            {                   
                SetConData();
                gvpbCardType.BindData();
            }
            
        }       
    }

    /// <summary>
    /// 查詢製卡類別
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbCardType.BindData();
    }

    /// <summary>
    /// 轉向新增製卡類別
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("CardType006Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
    #endregion  

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCardType_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropGroup_Name", dropGroup_Name.SelectedValue);
        inputs.Add("txtType_Name", txtType_Name.Text);
        //保存查詢條件
        Session["Condition"] = inputs;
        DataSet dstlCardType = null;
        try
        {
            dstlCardType = bizLogic.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlCardType != null)//如果查到了資料
            {
                e.Table = dstlCardType.Tables[0];//要綁定的資料表
                foreach (DataRow drowCardType in dstlCardType.Tables[0].Rows)
                {
                    if (drowCardType["Is_Report"].Equals("Y"))
                    {
                        drowCardType["Is_Report"] = '是';
                    }
                    else {
                        drowCardType["Is_Report"] = '否';
                    }
                    if (drowCardType["Is_Import"].Equals("Y"))
                    {
                        drowCardType["Is_Import"] = '是';
                    }
                    else
                    {
                        drowCardType["Is_Import"] = '否';
                    }
                }
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
  
    }            

    #endregion

}
