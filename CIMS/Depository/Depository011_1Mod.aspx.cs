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
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;


public partial class Depository_Depository011_1Mod : PageBase
{
    Depository011_1BL bl = new Depository011_1BL();   

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbMaterielStocksMove.NoneData = "";
        btnExport.Enabled = true;
        btnExport1.Enabled = true;
        
        if (!IsPostBack)
        {
            //btnExport.Enabled = false;
            //btnExport1.Enabled = false;
            
            string strID = Request.QueryString["Move_ID"];
            Session["Move_ID"] = strID;

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("RID");
            dtbl.Columns.Add("Materiel_Name");
            dtbl.Columns.Add("Serial_Number");
            dtbl.Columns.Add("Move_Number");
            dtbl.Columns.Add("From_Factory_Name");
            dtbl.Columns.Add("To_Factory_Name");

            Session["MaterielStocksMove"] = dtbl;//存儲UI中的信息
            
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
                hidMove_ID.Value = strID;
                Session["MaterielStocksMove"] = ds.Tables[0];
                DateTime moveDate = Convert.ToDateTime(lbDate.Text);
                if (bl.Is_Managed(moveDate))
                    chkDelete.Enabled = false;             
                gvpbMaterielStocksMove.BindData(); 
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
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
        DataTable dtbl = new DataTable();
        dtbl = (DataTable)Session["MaterielStocksMove"];
        if (null!=dtbl)
        {
            e.Table = dtbl;
            e.RowCount = dtbl.Rows.Count;
            //this.Session["MaterielStocksMoveModNum"] = e.RowCount;
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
        string moveId = Session["Move_ID"].ToString();
        DateTime moveDate = DateTime.ParseExact(moveId.Substring(0, 8), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        
            try
            {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

                ImageButton ibtnDelete = null;
                string rid = dtbl.Rows[e.Row.RowIndex]["RID"].ToString();
                e.Row.Cells[1].Text = Convert.ToInt32(e.Row.Cells[1].Text).ToString("N0");
                // 刪除的邦定事件
                ibtnDelete = (ImageButton)e.Row.FindControl("ibtnDelete");
                if (bl.Is_Managed(moveDate)) 
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
                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository011_1Detail.aspx?ActionType=Mod&Index=" + e.Row.RowIndex.ToString() + "&Rid=" + rid + " ','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");
            }
            Session["MaterielStocksMove"] = dtbl;
         
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
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
        if (e.CommandArgument.ToString() == "NOT")
            ShowMessage("移轉日後已匯入廠商結餘,無法刪除");
        else
        {
            string rowIndex = e.CommandArgument.ToString().Split('-')[0];
            string rid = e.CommandArgument.ToString().Split('-')[1];
            dtbl.Rows.RemoveAt(int.Parse(rowIndex));
            bl.delete(int.Parse(rid));
            Session["MaterielStocksMove"] = dtbl;
        }
        gvpbMaterielStocksMove.BindData();
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
            if (chkDelete.Checked == true)//是否選擇了刪除
            {
                string Move_ID = Request.QueryString["Move_ID"];

                bl.DeleteAll(Move_ID);

                //Session.Remove("MaterielStocksMoveMod");
                Session.Remove("MaterielStocksMove");
                ShowMessageAndGoPage(GlobalStringManager.Default["Alert_DeleteSuccess"], "Depository011_1.aspx?Con=1");
            }
            else
            {
                if (((DataTable)Session["MaterielStocksMove"]).Rows.Count==0)
                {
                    ShowMessage("無保存訊息！");
                    return;
                }

                //DataTable dtbl = (DataTable)Session["MaterielStocksMoveMod"];
                //string Move_ID = Request.QueryString["Move_ID"];

                //// 添加所有轉移單訊息
                //bl.Add(dtbl, lbDate.Text, lbID.Text);

                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Depository011_1.aspx?Con=1");
               
            }
            
        }catch (Exception ex)
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
        Session.Remove("MaterielStocksMove");
        Session.Remove("Move_ID");
        Response.Redirect("Depository011_1.aspx?Con=1");
    }
    
    /// <summary>
    /// 綁定轉移單訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        gvpbMaterielStocksMove.BindData();
    }
}
