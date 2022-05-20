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

public partial class Depository_Depository016 : PageBase
{
    Depository016BL bl = new Depository016BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbTransaction.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            dropFactory.DataSource = bl.GetFactoryAll();
            dropFactory.DataBind();
            dropFactory.Items.Insert(0, new ListItem("全部", ""));
            dropPARAM.DataSource = bl.GetPARAM010203();
            dropPARAM.DataBind();
            dropPARAM.Items.Insert(0, new ListItem("全部", ""));
            // 從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (SetConData())
            {
                gvpbTransaction.BindData();
            }

            txtBeginDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            txtEndDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
    }

    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbTransaction_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("txtBeginDate", txtBeginDate.Text);
        inputs.Add("txtEndDate", txtEndDate.Text);
        inputs.Add("dropFactory", dropFactory.SelectedValue);
        inputs.Add("dropPARAM", dropPARAM.SelectedValue);
        inputs.Add("txtMaterial", txtMaterial.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlMaterielStocksTransaction = null;

        try
        {
            dstlMaterielStocksTransaction = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlMaterielStocksTransaction != null)
            {
                dstlMaterielStocksTransaction.Tables[0].Columns.Add("Transaction_Date1", typeof(string));
                dstlMaterielStocksTransaction.Tables[0].Columns.Add("Materiel_Name", typeof(string));
                dstlMaterielStocksTransaction.Tables[0].Columns.Add("Serial_Number", typeof(string));

                foreach (DataRow dmRow in dstlMaterielStocksTransaction.Tables[0].Rows)
                {
                    dmRow["Transaction_Date1"] = ((DateTime)dmRow["Transaction_Date"]).ToString("yyyy/MM/dd");
                    string Serial_Number = dmRow["ANumber"].ToString();
                    string Materiel_Name = dmRow["AName"].ToString();
                    if (Materiel_Name == "")
                    {
                        Serial_Number = dmRow["BNumber"].ToString();
                        Materiel_Name = dmRow["BName"].ToString();
                    }
                    if (Materiel_Name == "")
                    {
                        Serial_Number = dmRow["CNumber"].ToString();
                        Materiel_Name = dmRow["CName"].ToString();
                    }
                    dmRow["Materiel_Name"] = Materiel_Name;
                    dmRow["Serial_Number"] = Serial_Number;
                }

                e.Table = dstlMaterielStocksTransaction.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void gvpbTransaction_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    /// <summary>
    /// 查詢按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbTransaction.BindData();
    }

    /// <summary>
    /// 新增按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("Depository016Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
}
