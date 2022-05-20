//******************************************************************
//*  作    者：lantaosu
//*  功能說明：物料庫存專業作業
//*  創建日期：2008-09-09
//*  修改日期：2008-09-12 12:00
//*  修改記錄：
//*            □2008-09-09
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
using System.Text;

public partial class Depository_Depository011_1Add : PageBase
{
    Depository011_1BL bl = new Depository011_1BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        //允許使用DoPostBack
        gvpbMaterielStocksMove.NoneData = "";

        if (!IsPostBack)
        {
            this.txtMove_Date.Text = System.DateTime.Now.ToString("yyyy/MM/dd");//初始化日期

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("RID",Type.GetType("System.Int32"));
            dtbl.Columns.Add("Move_Name",Type.GetType("System.String"));
            dtbl.Columns.Add("Move_Number", Type.GetType("System.Int32"));
            dtbl.Columns.Add("From_Factory_RID", Type.GetType("System.Int32"));
            dtbl.Columns.Add("To_Factory_RID", Type.GetType("System.Int32"));
            //dtbl.Columns.Add("Materiel_RID", Type.GetType("System.Int32"));
            //dtbl.Columns.Add("Materiel_Type", Type.GetType("System.Int32"));
            dtbl.Columns.Add("Serial_Number", Type.GetType("System.String"));
            dtbl.Columns.Add("From_Factory_Name",Type.GetType("System.String"));
            dtbl.Columns.Add("To_Factory_Name",Type.GetType("System.String"));
            dtbl.Columns.Add("Materiel_Name",Type.GetType("System.String"));
           
            Session["MaterielStocksMove"] = dtbl;//存儲UI中的信息
        }
    }

    protected void detailButton_click(object sender, EventArgs e)
    {
        if (txtMove_Date.Text == "")
        {
            ShowMessage("日期不能為空");
            return;
        }
        if (Convert.ToDateTime(txtMove_Date.Text) < DateTime.Now.Date)
        {
            ShowMessage("日期不能小於當天");
            return;
        }
        if (hidMoveId.Value == "") {    
            string Move_ID = bl.GetMove_ID(txtMove_Date.Text.Trim());
            hidMoveId.Value = Move_ID;
            txtMove_Date.Attributes.Remove("onfocus");
            txtMove_Date.Enabled = false;
            img.Attributes.Remove("onclick");
            Session["Move_ID"] = Move_ID;
        } 
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=window.showModalDialog('Depository011_1Detail.aspx?ActionType=Add&Index=-1','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}", true);
    }
    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // 介面資料檢查

        DataTable dtbl = (DataTable)Session["MaterielStocksMove"];
        if (dtbl.Rows.Count == 0)
        {
            ShowMessage("無添加列");
            return;
        }

        try
        {
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, string.Concat("Depository011_1Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
            
            //Session.Remove("Move_ID");
            //Session.Remove("MaterielStocksMove");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Depository011_1.aspx?Con=1");
        Session.Remove("Move_ID");
        Session.Remove("MaterielStocksMove");
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbMaterielStocksMove_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtbl = (DataTable)Session["MaterielStocksMove"];       
        if (null != dtbl)
        {
            e.Table = dtbl;
            e.RowCount = dtbl.Rows.Count;
        }
    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbMaterielStocksMove_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbMaterielStocksMove.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //if (0 == Convert.ToInt32(Session["MaterielStocksMoveAddNum"]))
            //    return;
 
            ImageButton ibtnDelete = null;
            string rid = dtbl.Rows[e.Row.RowIndex]["RID"].ToString();
            e.Row.Cells[1].Text = Convert.ToInt32(e.Row.Cells[1].Text).ToString("N0");
            // 刪除的邦定事件
            ibtnDelete = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnDelete.CommandArgument = e.Row.RowIndex+"-"+rid;
            ibtnDelete.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            //品名的邦定事件
            HyperLink hl = (HyperLink)e.Row.FindControl("hlName");            
            hl.Text = dtbl.Rows[e.Row.RowIndex]["Materiel_Name"].ToString();
            hl.NavigateUrl = "#";
            hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository011_1Detail.aspx?ActionType=Mod&Index=" + e.Row.RowIndex.ToString() + "&Rid="+ rid+" ','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");
        }
    }
    #endregion

    /// <summary>
    /// 刪除列
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtbl = (DataTable)Session["MaterielStocksMove"];
        string rowIndex = e.CommandArgument.ToString().Split('-')[0];
        string rid = e.CommandArgument.ToString().Split('-')[1];
        dtbl.Rows.RemoveAt(int.Parse(rowIndex));
        bl.delete(int.Parse(rid));
        Session["MaterielStocksMove"] = dtbl;
        gvpbMaterielStocksMove.BindData();
    }

    /// <summary>
    /// 綁定Grid控件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        gvpbMaterielStocksMove.BindData();
    }
}
