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
public partial class InOut_InOut006 : System.Web.UI.Page
{
    InOut006BL BL = new InOut006BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbIMPORT_HISTORY.NoneData = "";
        this.gvpbIMPORT_HISTORY.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //預設為當前系統日期
            this.txtBegin_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.txtFinish_Date.Text = DateTime.Now.ToString("yyyy/MM/dd");

            //this.dropFile_Name.DataTextField = "File_Name";
            //this.dropFile_Name.DataValueField = "File_Type";
            //this.dropFile_Name.DataSource = BL.GetFile_Name().Tables[0];
            //this.dropFile_Name.DataBind();
            //dropFile_Name.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (dropFile_Name.SelectedItem.Value != "0")
        {
            inputs.Add("dropFile_Name", dropFile_Name.SelectedItem.Value);
        }
        else
        {
            inputs.Add("dropFile_Name", "");
        }
        inputs.Add("txtBegin_Date", txtBegin_Date.Text);
        inputs.Add("txtFinish_Date", txtFinish_Date.Text);
        gvpbIMPORT_HISTORY.DataSource = BL.Search(inputs).Tables[0];
        gvpbIMPORT_HISTORY.DataBind();
    }
    protected void gvpbIMPORT_HISTORY_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Text = Convert.ToDateTime(e.Row.Cells[0].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            if (e.Row.Cells[1].Text == "1")
            {
                e.Row.Cells[1].Text = "小計檔";
            }
            if (e.Row.Cells[1].Text == "2")
            {
                e.Row.Cells[1].Text = "廠商庫存異動檔";
            }
            if (e.Row.Cells[1].Text == "3")
            {
                e.Row.Cells[1].Text = "代製費用異動檔";
            }
            if (e.Row.Cells[1].Text == "4")
            {
                e.Row.Cells[1].Text = "物料庫存異動檔";
            }
            if (e.Row.Cells[1].Text == "5")
            {
                e.Row.Cells[1].Text = "次月換卡預測檔";
            }
            if (e.Row.Cells[1].Text == "6")
            {
                e.Row.Cells[1].Text = "年度換卡預測檔";
            }
        }
    }
}
