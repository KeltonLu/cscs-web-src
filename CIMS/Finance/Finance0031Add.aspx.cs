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
public partial class Finance_Finance0031Add : PageBase
{
    Finance0031BL Finance0031BL = new Finance0031BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //(起)(迄)都設為當天
            this.txtFinish_Date1.Text = DateTime.Now.ToShortDateString();
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbMATERIEL_PURCHASE_FORM.BindData();
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        DataTable dtSearchMaterialPurchase = (DataTable)this.Session["dtSearchMaterialPurchase"];
        // 檢查至少有一個請款項目
        bool isCheck = false;
        foreach (GridViewRow GVdr in gvpbMATERIEL_PURCHASE_FORM.Rows)
        {
            CheckBox cbAsk_Money = (CheckBox)GVdr.FindControl("cbAsk_Money");
            if (cbAsk_Money.Checked)
            {
                isCheck = true;
                break;
            }
        }
        if (!isCheck)
        {
            ShowMessage("請最少選擇一項進行請款");
            return;
        }

        // 設定標示列訊息
        for (int intRowIndex = 0; intRowIndex < this.gvpbMATERIEL_PURCHASE_FORM.Rows.Count; intRowIndex++)
        {
            CheckBox cbAsk_Money = (CheckBox)this.gvpbMATERIEL_PURCHASE_FORM.Rows[intRowIndex].FindControl("cbAsk_Money");
            dtSearchMaterialPurchase.Rows[intRowIndex]["選中"] = (bool)cbAsk_Money.Checked;
        }

        // 檢查選中的物料請款項目是各物料帳務類別的單價是否相同
        string strRet = Finance0031BL.CheckPrice(dtSearchMaterialPurchase);
        if (strRet.Length==0)
        {
            Response.Redirect("Finance0031Add1.aspx");
        }
        else
        {
            ShowMessage("請款項目" + strRet + "內有不同單價物料。");
        }
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0031.aspx?Con=1");
    }
    protected void gvpbMATERIEL_PURCHASE_FORM_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtMaterial_Name", txtMaterial_Name.Text);
        inputs.Add("txtBegin_Date1", txtBegin_Date1.Text);
        inputs.Add("txtFinish_Date1", txtFinish_Date1.Text);
        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dsSearchMaterialPurchase = null;

        try
        {
            dsSearchMaterialPurchase = Finance0031BL.SearchMaterialPurchase(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dsSearchMaterialPurchase != null)//如果查到了資料
            {
                if (dsSearchMaterialPurchase.Tables[0].Rows.Count != 0)
                {
                    btnSubmit2.Visible = true;
                    
                }

                // 先添加“選中”標示列，然後將取得的訊息綁定到UI,并保存到Session中
                dsSearchMaterialPurchase.Tables[0].Columns.Add(new DataColumn("選中", Type.GetType("System.Boolean")));
                Session["dtSearchMaterialPurchase"] = dsSearchMaterialPurchase.Tables[0];
                e.Table = dsSearchMaterialPurchase.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
}
