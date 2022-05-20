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

public partial class Depository_Depository016Mod : PageBase
{
    Depository016BL bl = new Depository016BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbMaterielStocksTransaction.NoneData = "";
        btnExport.Enabled = true;
        btnExport1.Enabled = true;

        if (!IsPostBack)
        {
            string strID = Request.QueryString["Transaction_ID"];
            Session["Transaction_ID"] = strID;

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("RID");
            dtbl.Columns.Add("Materiel_Name");
            dtbl.Columns.Add("Serial_Number");
            dtbl.Columns.Add("Transaction_Amount");
            dtbl.Columns.Add("Factory_Name");
            dtbl.Columns.Add("PARAM_Name");

            Session["MaterielStocksTransaction"] = dtbl;//存儲UI中的信息

            if (StringUtil.IsEmpty(strID))
            {
                return;
            }

            try
            {
                DateTime dt = new DateTime();
                DataSet ds = bl.GetModDatas(strID);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    dt = (DateTime)ds.Tables[0].Rows[0][1];
                    lbDate.Text = dt.ToString("yyyy/MM/dd");//初始化日期
                }

                lbID.Text = strID;//初始化轉移單號
                hidTransaction_ID.Value = strID;
                Session["MaterielStocksTransaction"] = ds.Tables[0];
                DateTime transactionDate = Convert.ToDateTime(lbDate.Text);
                if (bl.Is_Managed(transactionDate))
                    chkDelete.Enabled = false;
                gvpbMaterielStocksTransaction.BindData();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

        }
    }

    protected void gvpbMaterielStocksTransaction_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtbl = new DataTable();
        dtbl = (DataTable)Session["MaterielStocksTransaction"];
        if (null != dtbl)
        {
            e.Table = dtbl;
            e.RowCount = dtbl.Rows.Count;
            //this.Session["MaterielStocksMoveModNum"] = e.RowCount;
        }
    }


    protected void gvpbMaterielStocksTransaction_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbMaterielStocksTransaction.DataSource;
        string transactionId = Session["Transaction_ID"].ToString();
        DateTime transactionDate = DateTime.ParseExact(transactionId.Substring(0, 8), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);

        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton ibtnDelete = null;
                string rid = dtbl.Rows[e.Row.RowIndex]["RID"].ToString();
                e.Row.Cells[2].Text = Convert.ToInt32(e.Row.Cells[2].Text).ToString("N0");
                // 刪除的邦定事件
                ibtnDelete = (ImageButton)e.Row.FindControl("ibtnDelete");
                if (bl.Is_Managed(transactionDate))
                    ibtnDelete.CommandArgument = "NOT";
                else
                    ibtnDelete.CommandArgument = e.Row.RowIndex + "-" + rid;
                ibtnDelete.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

                //品名的邦定事件
                HyperLink hl = (HyperLink)e.Row.FindControl("hlName");
                //---------------------------
                string Serial_Number = dtbl.Rows[e.Row.RowIndex]["Serial_Number"].ToString();
                string Materiel_Name = dtbl.Rows[e.Row.RowIndex]["Materiel_Name"].ToString();
                if (Materiel_Name == "")
                {
                    Serial_Number = dtbl.Rows[e.Row.RowIndex]["ANumber"].ToString();
                    Materiel_Name = dtbl.Rows[e.Row.RowIndex]["AName"].ToString();
                }
                if (Materiel_Name == "")
                {
                    Serial_Number = dtbl.Rows[e.Row.RowIndex]["BNumber"].ToString();
                    Materiel_Name = dtbl.Rows[e.Row.RowIndex]["BName"].ToString();
                }
                if (Materiel_Name == "")
                {
                    Serial_Number = dtbl.Rows[e.Row.RowIndex]["CNumber"].ToString();
                    Materiel_Name = dtbl.Rows[e.Row.RowIndex]["CName"].ToString();
                }

                dtbl.Rows[e.Row.RowIndex]["Materiel_Name"] = Materiel_Name;
                dtbl.Rows[e.Row.RowIndex]["Serial_Number"] = Serial_Number;

                hl.Text = Materiel_Name;
                hl.NavigateUrl = "#";
                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository016Detail.aspx?ActionType=Mod&Index=" + e.Row.RowIndex.ToString() + "&Rid=" + rid + " ','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");
            }
            Session["MaterielStocksTransaction"] = dtbl;

        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 刪除列
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtbl = (DataTable)Session["MaterielStocksTransaction"];
        if (e.CommandArgument.ToString() == "NOT")
            ShowMessage("移轉日後已匯入廠商結餘,無法刪除");
        else
        {
            string rowIndex = e.CommandArgument.ToString().Split('-')[0];
            string rid = e.CommandArgument.ToString().Split('-')[1];
            dtbl.Rows.RemoveAt(int.Parse(rowIndex));
            bl.delete(int.Parse(rid));
            Session["MaterielStocksTransaction"] = dtbl;
        }
        gvpbMaterielStocksTransaction.BindData();
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (chkDelete.Checked)//是否選擇了刪除
            {
                string Transaction_ID = Request.QueryString["Transaction_ID"];

                bl.DeleteAll(Transaction_ID);

                Session.Remove("MaterielStocksTransaction");
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_DeleteSuccess"], "Depository016.aspx?Con=1");
            }
            else
            {
                if (((DataTable)Session["MaterielStocksTransaction"]).Rows.Count == 0)
                {
                    ShowMessage("無保存訊息！");
                    return;
                }

                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Depository016.aspx?Con=1");
            }
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
        Session.Remove("MaterielStocksTransaction");
        Session.Remove("Transaction_ID");
        Response.Redirect("Depository016.aspx?Con=1");
    }

    /// <summary>
    /// 綁定轉移單訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        gvpbMaterielStocksTransaction.BindData();
    }
}
