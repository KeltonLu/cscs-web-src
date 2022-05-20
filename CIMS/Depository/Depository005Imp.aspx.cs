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

public partial class Depository_Depository005Imp : PageBase
{
    Depository005BL BL = new Depository005BL();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        DataTable dtblImp = (DataTable)Session["dtblImp"];
        int i = 1;

        if (dtblImp == null)
        {
            ShowMessage("無添加信息");
            return;
        }

        string strRID = "";

        foreach (DataRow drow in dtblImp.Rows)
        {
            if (StringUtil.IsEmpty(drow["Check_Type"].ToString()))
            {
                ShowMessage("驗貨方式不能為空");
                return;
            }
            i++;
        }
        if (i == 1)
        {
            ShowMessage("無添加信息");
            return;
        }

        try
        {
            strRID = BL.Add(dtblImp);

            Session.Remove("dtblImp");

            ShowMessageAndGoPage("再入庫成功", "Depository005.aspx?Con=1");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "window.open('Depository005ImpPrint.aspx?RID=" + strRID.Split(',')[0] + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=850,height=650');", true);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void btnImport_Click(object sender, EventArgs e)
    {
        if (FileUpd.PostedFile.ContentLength == 0)
        {
            ShowMessage("請選擇匯入文件");
            return;
        }

        string strPath = FileUpload(FileUpd.PostedFile);

        if (!StringUtil.IsEmpty(strPath))
        {

            try
            {
                DataTable dtblFileImp = new DataTable();
                dtblFileImp.Columns.Add("Stock_RID");//入庫流水編號
                dtblFileImp.Columns.Add("Space_Short_Name");//版面簡稱
                dtblFileImp.Columns.Add("Restock_Number");//再進貨數量
                dtblFileImp.Columns.Add("Blemish_Number");//瑕疵量
                dtblFileImp.Columns.Add("Sample_Number");//抽樣卡數量
                dtblFileImp.Columns.Add("Reincome_Date");//再入庫日期
                dtblFileImp.Columns.Add("Serial_Number");//卡片批號
                dtblFileImp.Columns.Add("Blank_Factory_Name");//空白廠商
                dtblFileImp.Columns.Add("Perso_Factory_Name");//Perso廠商                

                DataTable dtblImp = BL.Imp(strPath, dtblFileImp);

                string strError = "";

                foreach (DataRow drow in dtblImp.Rows)
                {
                    if (!StringUtil.IsEmpty(drow["error"].ToString()))
                        strError += drow["error"].ToString() + ",";
                }

                if (!StringUtil.IsEmpty(strError))
                    ShowMessage(strError.Substring(0, strError.Length - 1));

                Session["dtblImp"] = dtblImp;

                gvpbImportStock.BindData();
            }
            catch (Exception ex)
            {
                Session.Remove("dtblImp");
                ShowMessage(ex.Message);
            }
        }
    }
    protected void btnBind_Click(object sender, EventArgs e)
    {
        gvpbImportStock.BindData();
    }
        #endregion
    #region 列表資料綁定.
    protected void gvpbImportStock_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtblImp = (DataTable)Session["dtblImp"];

        e.Table = dtblImp;
        e.RowCount = dtblImp.Rows.Count;
    }
    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void btnDelete_Command(object sender, CommandEventArgs e)
    //{
    //    DataTable dtbl = (DataTable)Session["dtblImp"];

    //    dtbl.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

    //    Session["dtblImp"] = dtbl;

    //    gvpbImportStock.BindData();
    //}
    protected void gvpbImportStock_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblImp = (DataTable)gvpbImportStock.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblImp.Rows.Count == 0)
                return;

            ImageButton ibtnButton = null;

            //// 刪除的邦定事件
            //ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
            //ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
            //ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            if (e.Row.Cells[7].Text != "&nbsp;")
            {
                e.Row.Cells[7].Text = Convert.ToDateTime(e.Row.Cells[7].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
            if (e.Row.Cells[14].Text != "&nbsp;")
            {
                e.Row.Cells[14].Text = e.Row.Cells[14].Text.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");
            }
            if (e.Row.Cells[12].Text != "&nbsp;")
            {
                e.Row.Cells[12].Text = e.Row.Cells[12].Text.Replace("1", "抽驗").Replace("2", "全驗");
            }
            if (e.Row.Cells[2].Text != "&nbsp;")
            {
                e.Row.Cells[2].Text = Convert.ToInt32(e.Row.Cells[2].Text).ToString("N0");
            }
            if (e.Row.Cells[3].Text != "&nbsp;")
            {
                e.Row.Cells[3].Text = Convert.ToInt32(e.Row.Cells[3].Text).ToString("N0");
            }
            if (e.Row.Cells[4].Text != "&nbsp;")
            {
                e.Row.Cells[4].Text = Convert.ToInt32(e.Row.Cells[4].Text).ToString("N0");
            }
            if (e.Row.Cells[5].Text != "&nbsp;")
            {
                e.Row.Cells[5].Text = Convert.ToInt32(e.Row.Cells[5].Text).ToString("N0");
            }
            if (e.Row.Cells[6].Text != "&nbsp;")
            {
                e.Row.Cells[6].Text = Convert.ToInt32(e.Row.Cells[6].Text).ToString("N0");
            }

            HyperLink hl = (HyperLink)e.Row.FindControl("hlDetailRID");
            hl.Text = dtblImp.Rows[e.Row.RowIndex]["Stock_RID"].ToString();
            hl.NavigateUrl = "#";
            hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository005Detail.aspx?ActionType=Add&RID=" + e.Row.RowIndex.ToString() + "','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");

        }
    }
        #endregion
}
