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

public partial class Depository_Depository011_1 : PageBase
{
    Depository011_1BL bl = new Depository011_1BL();
        
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbMaterielStocksMove.PageSize = GlobalStringManager.PageSize;

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
                gvpbMaterielStocksMove.BindData();

            txtBeginDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            txtEndDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbMaterielStocksMove_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();  
        inputs.Add("txtBeginDate", txtBeginDate.Text);
        inputs.Add("txtEndDate", txtEndDate.Text);
        inputs.Add("dropFromFactory", dropFromFactory.SelectedValue);
        inputs.Add("dropToFactory", dropToFactory.SelectedValue);
        inputs.Add("txtMaterial", txtMaterial.Text);

        //保存查詢條件
        Session["Condition"] = inputs;

        DataSet dstlMaterielStocksMove = null;

        try
        {
            dstlMaterielStocksMove = bl.List(inputs, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlMaterielStocksMove != null)//如果查到了資料
            {
                dstlMaterielStocksMove.Tables[0].Columns.Add("Move_Date1", Type.GetType("System.String"));
                dstlMaterielStocksMove.Tables[0].Columns.Add("Materiel_Name", Type.GetType("System.String"));
                dstlMaterielStocksMove.Tables[0].Columns.Add("Serial_Number", Type.GetType("System.String"));
                foreach (DataRow dmRow in dstlMaterielStocksMove.Tables[0].Rows)
                {
                   
                    dmRow["Move_Date1"] = ((DateTime)dmRow["Move_Date"]).ToString("yyyy/MM/dd");
                    string Serial_Number = dmRow["ANumber"].ToString();
                    string Materiel_Name = dmRow["AName"].ToString();
                    if(Materiel_Name == ""){
                        Serial_Number = dmRow["BNumber"].ToString();
                        Materiel_Name = dmRow["BName"].ToString();
                    }
                    if(Materiel_Name == ""){
                        Serial_Number = dmRow["CNumber"].ToString();
                        Materiel_Name = dmRow["CName"].ToString();
                    }
                    dmRow["Materiel_Name"] = Materiel_Name;
                    dmRow["Serial_Number"] = Serial_Number;
                }

                e.Table = dstlMaterielStocksMove.Tables[0];//要綁定的資料表
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
    protected void gvpbMaterielStocksMove_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
    }
    #endregion
    
    /// <summary>
    /// 查詢按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbMaterielStocksMove.BindData();
    }

    /// <summary>
    /// 新增按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Concat("Depository011_1Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
    }
}
