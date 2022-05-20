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

public partial class BaseInfo_BaseInfo010 : PageBase
{
    BaseInfo010BL bizlogic = new BaseInfo010BL();
    #region 事件處理

    /// <summary>
    /// 頁面初始化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbFormula.BindData();
    }

    /// <summary>
    /// 公式瀏覽
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnWatchClick(object sender, EventArgs e)
    {
        for (int i = 0; i < gvpbFormula.Rows.Count; i++)
        {
            TextBox param_name_TXT = (TextBox)gvpbFormula.Rows[i].FindControl("Param_Name");
            if (StringUtil.IsEmpty(param_name_TXT.Text.Trim()))
            {
                ShowMessage("參數名稱不能為空");
                return;
            }
            for (int j = 0; j < gvpbFormula.Rows.Count; j++)
            {
                if (i == j)
                    continue;

                TextBox param_name_TXT1 = (TextBox)gvpbFormula.Rows[j].FindControl("Param_Name");
                if (param_name_TXT.Text == param_name_TXT1.Text)
                {
                    ShowMessage("參數名稱不能相同");
                    return;
                }
            }

        }
        
        for (int i = 0; i < gvpbFormula.Rows.Count; i++)
        {

            HtmlTableRow tblrow = new HtmlTableRow();
            HtmlTableCell nameCell = new HtmlTableCell();
            nameCell.InnerText = ((TextBox)gvpbFormula.Rows[i].Cells[0].FindControl("Param_Name")).Text;
            HtmlTableCell tbc = new HtmlTableCell();
            string text = "=";
            //if (nameCell.InnerText == "製成卡數" || nameCell.InnerText == "消耗卡數")
            //{
            //    text += "小計檔";
            //}
            HtmlTable table =(HtmlTable)gvpbFormula.Rows[i].FindControl("tbStatus");                       
            //拼接所有不為“捨”的狀態項目
            for (int row = 0; row < table.Rows.Count; row++)
            {              
                for (int j = 0; j < table.Rows[row].Cells.Count;j+=2)
                {
                    DropDownList drop = (DropDownList)table.Rows[row].Cells[j].Controls[0];
                    if (drop.SelectedItem.Text == "+" || drop.SelectedItem.Text == "-")
                    {
                        text += drop.SelectedItem.Text + table.Rows[row].Cells[j + 1].InnerText;
                    }
                }            
            }
            tbc.InnerText = text;
            tblrow.Cells.Add(nameCell);
            tblrow.Cells.Add(tbc);
            if (i % 2 == 0)
            {
                tblrow.Attributes.Add("Class", "GridViewRow");
            }
            else {
                tblrow.Attributes.Add("Class", "GridViewAlter");
            }
            result.Rows.Add(tblrow);
        }
        result.Visible = true;        
    }

    /// <summary>
    /// 保存修改後的公式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_click(object sender, EventArgs e)
    {
        for (int i = 0; i < gvpbFormula.Rows.Count; i++)
        {
            TextBox param_name_TXT = (TextBox)gvpbFormula.Rows[i].FindControl("Param_Name");
            if (StringUtil.IsEmpty(param_name_TXT.Text.Trim()))
            {
                ShowMessage("參數名稱不能為空");
                return;
            }
            for (int j = 0; j < gvpbFormula.Rows.Count; j++)
            {
                if (i == j)
                    continue;

                TextBox param_name_TXT1 = (TextBox)gvpbFormula.Rows[j].FindControl("Param_Name");
                if (param_name_TXT.Text.Trim() == param_name_TXT1.Text.Trim())
                {
                    ShowMessage("參數名稱不能相同");
                    return;
                }
            }

        }

        DataTable dtbFormula = new DataTable();
        dtbFormula.Columns.Add("Expressions_RID");
        dtbFormula.Columns.Add("Type_RID");
        dtbFormula.Columns.Add("Operate_RID");
        DataTable dtbParam = new DataTable();
        dtbParam.Columns.Add("rid");
        dtbParam.Columns.Add("Param_Name");
        try
        {
            result.Visible = false;
            for (int i = 0; i < gvpbFormula.Rows.Count; i++)
            {
                HtmlTable table = (HtmlTable)gvpbFormula.Rows[i].FindControl("tbStatus");
                TextBox param_name_TXT = (TextBox)gvpbFormula.Rows[i].FindControl("Param_Name");

                DataRow param_row = dtbParam.NewRow();
                param_row["Param_Name"] = param_name_TXT.Text.Trim();
                param_row["rid"] = param_name_TXT.Attributes["key"];
                dtbParam.Rows.Add(param_row);
                for (int row = 0; row < table.Rows.Count; row++)
                {
                    int j = 0;
                    while (j < table.Rows[row].Cells.Count)
                    {
                        DropDownList drop = (DropDownList)table.Rows[row].Cells[j].Controls[0];
                        if (drop.SelectedItem.Text == "+" || drop.SelectedItem.Text == "-")
                        {
                            //取expressions_rid,Type_Rid,Operate_rid
                            string[] str = drop.ID.Split('-');
                            DataRow drow = dtbFormula.NewRow();
                            drow["Expressions_RID"] = str[0];
                            drow["Type_RID"] = str[1];
                            drow["Operate_RID"] = drop.SelectedValue;

                            dtbFormula.Rows.Add(drow);
                        }
                        j += 2;
                    }
                }

            }
            bizlogic.Update(dtbFormula,dtbParam);
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo010.aspx");
        } 
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    #endregion

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbFormula_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataSet dstlBudget = null;
        try
        {
            dstlBudget = bizlogic.GetParam();

            if (dstlBudget != null)//如果查到了資料
            {
                e.Table = dstlBudget.Tables[0];//要綁定的資料表
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
    protected void gvpbFormula_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblAgreement = (DataTable)gvpbFormula.DataSource;
        //DataTable dtblOperate = bizlogic.GetOperator().Tables[0];
        //DataTable dtblOperate = new DataTable();
        //dtblOperate.Columns.Add("value");
        //dtblOperate.Columns.Add("text");
        //DataRow drow = new DataRow();
        //drow["value"] = "+";
        //drow["text"] = "+";
        //dtblOperate.Rows.Add(drow);

        if (e.Row.RowType == DataControlRowType.DataRow)
        {    
            int rowIndex = e.Row.RowIndex;
            try
            {
                HtmlTable tbl = (HtmlTable)e.Row.FindControl("tbStatus");
                TextBox txtParam_Name = (TextBox)e.Row.FindControl("txtParam_Name");
                txtParam_Name.ID = "Param_Name";
                txtParam_Name.Text = dtblAgreement.Rows[rowIndex]["Param_Name"].ToString();
                DataTable dtbAllStatus = bizlogic.GetStatus().Tables[0];
                string formulaRid = dtblAgreement.Rows[rowIndex]["Param_code"].ToString();
                string paramRid = dtblAgreement.Rows[rowIndex]["Rid"].ToString();
                txtParam_Name.Attributes.Add("key", paramRid);
                //HtmlTableRow row = new HtmlTableRow();
                int rowCount = dtbAllStatus.Rows.Count / 8;
                HtmlTableRow row = null;
                for (int index = 0; index < dtbAllStatus.Rows.Count;index++ )
                {
                    if (index % 8 == 0) {
                        row = new HtmlTableRow();
                    }
                        DataRow drow = dtbAllStatus.Rows[index];
                        HtmlTableCell dropCell = new HtmlTableCell();
                        HtmlTableCell textCell = new HtmlTableCell();
                        string statusRid = drow["rid"].ToString();
                        string selectedValue = bizlogic.getOPeratorByFormulaAndStatus(formulaRid, statusRid);
                        //設定運算符的下拉選框
                        DropDownList drop = new DropDownList();

                        drop.Items.Insert(0, new ListItem("+", "+"));

                        drop.Items.Insert(1, new ListItem("-", "-"));

                        drop.Items.Insert(2, new ListItem("捨", " "));
                        drop.SelectedValue = selectedValue;
                        //設定運算符下拉框的Rid為“公式Rid-狀態項目Rid”
                        drop.ID = formulaRid + "-" + statusRid;
                        dropCell.Controls.Add(drop);
                        textCell.InnerText = drow["Status_NAME"].ToString();
                        row.Cells.Add(dropCell);
                        row.Cells.Add(textCell);
                        if (index % 8 == 7 || index == dtbAllStatus.Rows.Count-1) {
                            tbl.Rows.Add(row); 
                        }
                }
                
                //foreach (DataRow drow in dtbAllStatus.Rows)
                //{
                //    HtmlTableCell dropCell = new HtmlTableCell();
                //    HtmlTableCell textCell = new HtmlTableCell();
                //    string statusRid = drow["rid"].ToString();
                //    string selectedValue = bizlogic.getOPeratorByFormulaAndStatus(formulaRid, statusRid);
                //    //設定運算符的下拉選框
                //    DropDownList drop = new DropDownList(); 

                //    drop.Items.Insert(0, new ListItem("+","+"));
                   
                //    drop.Items.Insert(1, new ListItem("-", "-"));

                //    drop.Items.Insert(2, new ListItem("捨", " "));
                //    drop.SelectedValue = selectedValue;
                //    //設定運算符下拉框的Rid為“公式Rid-狀態項目Rid”
                //    drop.ID = formulaRid + "-" + statusRid;
                //    dropCell.Controls.Add(drop);
                //    textCell.InnerText = drow["Status_NAME"].ToString();
                //    row.Cells.Add(dropCell);
                //    row.Cells.Add(textCell);
                //}
                //tbl.Rows.Add(row);
            }
            catch
            {
                return;
            }
        }
    }
    #endregion
}
