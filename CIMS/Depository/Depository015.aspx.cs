//******************************************************************
//*  作    者：lantaosu
//*  功能說明：物料採購作業
//*  創建日期：2008-11-18
//*  修改日期：2008-11-20 12:00
//*  修改記錄：
//*            □2008-11-20
//*              1.創建 蘇斕濤
//*******************************************************************

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

public partial class Depository_Depository015 : PageBase
{
    Depository015BL bl = new Depository015BL();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbPurchase.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            Session.Remove("PurchaseOrder_RID");
            Session.Remove("Purchase_Date");
            Session.Remove("Purchase");

            //預設為當前系統日期
            txtBeginDate.Text = DateTime.Today.ToString("yyyy/MM/dd");
            txtEndDate.Text = DateTime.Today.ToString("yyyy/MM/dd");            

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                }
                if (txtEndDate.Text != "")
                    txtEndDate.Text = Convert.ToDateTime(txtEndDate.Text).ToString("yyyy/MM/dd");
                gvpbPurchase.BindData();
            }
            else
                Session.Remove("Condition");
        }
    }


    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbPurchase.BindData();
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbPurchase_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtBeginDate", txtBeginDate.Text);
        inputs.Add("txtEndDate", txtEndDate.Text);        
        inputs.Add("txtMaterial", txtMaterial.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlPurchase = null;

        try
        {
            dstlPurchase = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlPurchase != null)//如果查到了資料
            {               
                e.Table = dstlPurchase.Tables[0];//要綁定的資料表
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
    protected void gvpbPurchase_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtb = (DataTable)gvpbPurchase.DataSource;
                
        Label Name = null;        
        Label PersoFactory = null;
        Label DeliveryDate = null;
        Label Number = null;
        Label CaseDate = null;
        long number;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                if (dtb != null && dtb.Rows.Count > 0)
                {                                                           
                    Name = (Label)e.Row.FindControl("lblName");
                    

                    if (dtb.Rows[e.Row.RowIndex]["AName"].ToString().Trim() != "" && dtb.Rows[e.Row.RowIndex]["AName"].ToString() != null)
                    {
                        Name.Text = dtb.Rows[e.Row.RowIndex]["AName"].ToString();                        
                    }
                    else if (dtb.Rows[e.Row.RowIndex]["BName"].ToString().Trim() != "" && dtb.Rows[e.Row.RowIndex]["BName"].ToString() != null)
                    {
                        Name.Text = dtb.Rows[e.Row.RowIndex]["BName"].ToString();                        
                    }
                    else if (dtb.Rows[e.Row.RowIndex]["CName"].ToString().Trim() != "" && dtb.Rows[e.Row.RowIndex]["CName"].ToString() != null)
                    {
                        Name.Text = dtb.Rows[e.Row.RowIndex]["CName"].ToString();                        
                    }

                    //送貨Perso廠
                    PersoFactory = (Label)e.Row.FindControl("lblPersoFactory");
                    string Factory = "";
                    Factory = bl.GetFactoryShortCName(dtb.Rows[e.Row.RowIndex]["Factory_RID1"].ToString());
                    if (Factory != "")
                        PersoFactory.Text = Factory;
                    Factory = bl.GetFactoryShortCName(dtb.Rows[e.Row.RowIndex]["Factory_RID2"].ToString());
                    if (Factory != "")
                        PersoFactory.Text += "<BR>" + Factory;
                    Factory = bl.GetFactoryShortCName(dtb.Rows[e.Row.RowIndex]["Factory_RID3"].ToString());
                    if (Factory != "")
                        PersoFactory.Text += "<BR>" + Factory;
                    Factory = bl.GetFactoryShortCName(dtb.Rows[e.Row.RowIndex]["Factory_RID4"].ToString());
                    if (Factory != "")
                        PersoFactory.Text += "<BR>" + Factory;
                    Factory = bl.GetFactoryShortCName(dtb.Rows[e.Row.RowIndex]["Factory_RID5"].ToString());
                    if (Factory != "")
                        PersoFactory.Text += "<BR>" + Factory;                    

                    //送貨數量
                    Number = (Label)e.Row.FindControl("lblNumber");
                    if (dtb.Rows[e.Row.RowIndex]["Number1"].ToString() != "")
                    {
                        number = Convert.ToInt32(dtb.Rows[e.Row.RowIndex]["Number1"].ToString());
                    }
                    else
                    {
                        number = 0;
                    }
                    if (number != 0)
                        Number.Text = number.ToString("N0");

                    if (dtb.Rows[e.Row.RowIndex]["Number2"].ToString() != "")
                    {
                        number = Convert.ToInt32(dtb.Rows[e.Row.RowIndex]["Number2"].ToString());
                    }
                    else
                    {
                        number = 0;
                    }
                    if (number != 0)
                        Number.Text += "<BR>" + number.ToString("N0");

                    if (dtb.Rows[e.Row.RowIndex]["Number3"].ToString() != "")
                    {
                        number = Convert.ToInt32(dtb.Rows[e.Row.RowIndex]["Number3"].ToString());
                    }
                    else
                    {
                        number = 0;
                    }
                    if (number != 0)
                        Number.Text += "<BR>" + number.ToString("N0");

                    if (dtb.Rows[e.Row.RowIndex]["Number4"].ToString() != "")
                    {
                        number = Convert.ToInt32(dtb.Rows[e.Row.RowIndex]["Number4"].ToString());
                    }
                    else
                    {
                        number = 0;
                    }
                    if (number != 0)
                        Number.Text += "<BR>" + number.ToString("N0");

                    if (dtb.Rows[e.Row.RowIndex]["Number5"].ToString() != "")
                    {
                        number = Convert.ToInt32(dtb.Rows[e.Row.RowIndex]["Number5"].ToString());
                    }
                    else
                    {
                        number = 0;
                    }
                    if (number != 0)
                        Number.Text += "<BR>" + number.ToString("N0");

                    //送貨日期
                    DeliveryDate = (Label)e.Row.FindControl("lblDeliveryDate");
                    if (dtb.Rows[e.Row.RowIndex]["Delivery_Date1"].ToString().Split(' ')[0] != "1900/1/1")
                        DeliveryDate.Text = dtb.Rows[e.Row.RowIndex]["Delivery_Date1"].ToString().Split(' ')[0];
                    if(dtb.Rows[e.Row.RowIndex]["Delivery_Date2"].ToString().Split(' ')[0]!="1900/1/1")
                        DeliveryDate.Text += "<BR>" + dtb.Rows[e.Row.RowIndex]["Delivery_Date2"].ToString().Split(' ')[0];
                    if (dtb.Rows[e.Row.RowIndex]["Delivery_Date3"].ToString().Split(' ')[0] != "1900/1/1")
                        DeliveryDate.Text += "<BR>" + dtb.Rows[e.Row.RowIndex]["Delivery_Date3"].ToString().Split(' ')[0];
                    if (dtb.Rows[e.Row.RowIndex]["Delivery_Date4"].ToString().Split(' ')[0] != "1900/1/1")
                        DeliveryDate.Text += "<BR>" + dtb.Rows[e.Row.RowIndex]["Delivery_Date4"].ToString().Split(' ')[0];
                    if (dtb.Rows[e.Row.RowIndex]["Delivery_Date5"].ToString().Split(' ')[0] != "1900/1/1")
                        DeliveryDate.Text += "<BR>" + dtb.Rows[e.Row.RowIndex]["Delivery_Date5"].ToString().Split(' ')[0];

                    //結案日
                    CaseDate = (Label)e.Row.FindControl("lblCaseDate");
                    if (dtb.Rows[e.Row.RowIndex]["Case_Date"].ToString().Split(' ')[0] != "1900/1/1")
                        CaseDate.Text = dtb.Rows[e.Row.RowIndex]["Case_Date"].ToString().Split(' ')[0];
                }

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

    }
    #endregion

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("Depository015Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
}
