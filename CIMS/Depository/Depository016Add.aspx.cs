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

public partial class Depository_Depository016Add : PageBase
{
    Depository016BL bl = new Depository016BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbMaterielStocksTransaction.NoneData = "";

        if (!IsPostBack)
        {
            txtTransaction_Date.Text = System.DateTime.Now.ToString("yyyy/MM/dd");//初始化日期

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("RID", typeof(Int32));
            dtbl.Columns.Add("Transaction_Name", typeof(string));
            dtbl.Columns.Add("Transaction_Amount", typeof(Int32));
            dtbl.Columns.Add("Factory_RID", typeof(Int32));
            dtbl.Columns.Add("Factory_Name", typeof(string));
            dtbl.Columns.Add("PARAM_RID", typeof(Int32));
            dtbl.Columns.Add("PARAM_Name", typeof(string));
            dtbl.Columns.Add("Serial_Number", typeof(string));
            dtbl.Columns.Add("Materiel_Name", typeof(string));

            Session["MaterielStocksTransaction"] = dtbl;//存儲UI中的信息
        }
    }
    protected void detailButton_click(object sender, EventArgs e)
    {
        if (txtTransaction_Date.Text.Trim() == "")
        {
            ShowMessage("日期不能為空");
            return;
        }
        if (Convert.ToDateTime(txtTransaction_Date.Text) < DateTime.Now.Date)
        {
            ShowMessage("日期不能小於當天");
            return;
        }

        if (hidTransactionId.Value == "")
        {
            string Transaction_ID = bl.GetTransaction_ID(txtTransaction_Date.Text.Trim());
            hidTransactionId.Value = Transaction_ID;
            txtTransaction_Date.Attributes.Remove("onfocus");
            txtTransaction_Date.Enabled = false;
            img.Attributes.Remove("onclick");
            Session["Transaction_ID"] = Transaction_ID;
        }
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=window.showModalDialog('Depository016Detail.aspx?ActionType=Add&Index=-1','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}", true);
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        DataTable dtbl = (DataTable)Session["MaterielStocksTransaction"];
        if (0 == dtbl.Rows.Count)
        {
            ShowMessage("無添加列");
            return;
        }

        try
        {
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, string.Concat("Depository016Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Depository016.aspx?Con=1");
        Session.Remove("Transaction_ID");
        Session.Remove("MaterielStocksTransaction");
    }

    protected void gvpbMaterielStocksTransaction_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtbl = (DataTable)Session["MaterielStocksTransaction"];
        if (null != dtbl)
        {
            e.Table = dtbl;
            e.RowCount = dtbl.Rows.Count;
        }
    }

    protected void gvpbMaterielStocksTransaction_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbMaterielStocksTransaction.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ibtnDelete = null;
            string rid = dtbl.Rows[e.Row.RowIndex]["RID"].ToString();
            e.Row.Cells[2].Text = Convert.ToInt32(e.Row.Cells[2].Text).ToString("N0");
            // 刪除的邦定事件
            ibtnDelete = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnDelete.CommandArgument = e.Row.RowIndex + "-" + rid;
            ibtnDelete.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            //品名的邦定事件
            HyperLink hl = (HyperLink)e.Row.FindControl("hlName");
            hl.Text = dtbl.Rows[e.Row.RowIndex]["Materiel_Name"].ToString();
            hl.NavigateUrl = "#";
            hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository016Detail.aspx?ActionType=Mod&Index=" + e.Row.RowIndex.ToString() + "&Rid=" + rid + " ','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");
        }
    }

    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtbl = (DataTable)Session["MaterielStocksTransaction"];
        string rowIndex = e.CommandArgument.ToString().Split('-')[0];
        string rid = e.CommandArgument.ToString().Split('-')[1];
        dtbl.Rows.RemoveAt(int.Parse(rowIndex));
        bl.delete(int.Parse(rid));
        Session["MaterielStocksTransaction"] = dtbl;
        gvpbMaterielStocksTransaction.BindData();
    }

    protected void btnBind_Click(object sender, EventArgs e)
    {
        gvpbMaterielStocksTransaction.BindData();
    }

    
}
