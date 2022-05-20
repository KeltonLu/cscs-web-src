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
public partial class Finance_Finance0022 : PageBase
{
    Finance0022BL Finance0022BL = new Finance0022BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //this.txtBegin_Date.Text = DateTime.Now.ToShortDateString();
            //this.txtFinish_Date.Text = DateTime.Now.ToShortDateString();
            dropCard_PurposeBind();// 用途下拉框綁定
            dropCard_GroupBind();// 群組下拉框綁定
            dropFactoryBind();//Perso厰卡廠下拉框綁定
        }
    }


    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_PurposeBind()
    {
        dropCard_Purpose.DataTextField = "PARAM_NAME";
        dropCard_Purpose.DataValueField = "Param_Code";
        dropCard_Purpose.DataSource = Finance0022BL.getParam_Finance();
        dropCard_Purpose.DataBind();
        //dropCard_Purpose.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        //dropCard_Purpose.SelectedIndex = 1;
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_GroupBind()
    {
        dropCard_Group.Items.Clear();

        dropCard_Group.DataTextField = "GROUP_NAME";
        dropCard_Group.DataValueField = "RID";
        dropCard_Group.DataSource = Finance0022BL.getCardGroup(dropCard_Purpose.SelectedValue);
        dropCard_Group.DataBind();

        //dropCard_Group.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 空白卡廠下拉框綁定
    /// </summary>
    protected void dropFactoryBind()
    {
        dropFactory.Items.Clear();

        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataValueField = "RID";
        dropFactory.DataSource = Finance0022BL.getFactory();
        dropFactory.DataBind();

        dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    protected void dropCard_Purpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_GroupBind();
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {  
            //檢查卡片耗用日期區間所有工作日是否都日結
            DateTime Date = Finance0022BL.CheckEachWorkDateIsSurplus(Convert.ToDateTime(txtBegin_Date.Text), Convert.ToDateTime(txtFinish_Date.Text));
            if (Date != Convert.ToDateTime("1900/01/01"))
            {
                ShowMessage(Date.ToString("yyyy/MM/dd") + "之後未日結");
                return;
            }
            this.gvpbProjectCost.BindData();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    protected void gvpbProjectCost_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("dropCard_Purpose", dropCard_Purpose.SelectedValue);
        inputs.Add("dropCard_Group", dropCard_Group.SelectedValue);
        inputs.Add("dropCard_Group_txt", dropCard_Group.SelectedItem.Text.Trim());
        inputs.Add("dropFactory", dropFactory.SelectedValue);
        inputs.Add("txtBegin_Date", txtBegin_Date.Text + " 00:00:00");
        inputs.Add("txtFinish_Date", txtFinish_Date.Text + " 23:59:59");
       


        DataSet dsProjectCost = null;

        try
        {
            dsProjectCost = Finance0022BL.SearchProjectCost(inputs);

            if (dsProjectCost != null)//如果查到了資料
            {
                ViewState["dsProjectCost"] = dsProjectCost.Tables[0];
                e.Table = dsProjectCost.Tables[0];//要綁定的資料表
                e.RowCount = dsProjectCost.Tables[0].Rows.Count;

                if (dsProjectCost.Tables[0].Rows.Count == 0)
                {
                    btnExport.Visible = false;
                }
                else
                {
                    btnExport.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        DataTable dtProjectCost = (DataTable)ViewState["dsProjectCost"];
        string strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        Finance0022BL.ADD_CARD_YEAR_FORCAST_PRINT(dtProjectCost, strTime);
        Response.Write("<script>window.open('Finance0022Print.aspx?Time=" + strTime + "&SAP_Serial_Number=1&GroupName=" + Server.UrlEncode(dropCard_Group.SelectedItem.Text) + "&Begin_Date=" + Server.UrlEncode(txtBegin_Date.Text) + "&Finish_Date=" + Server.UrlEncode(txtFinish_Date.Text) + "','_blank','status=no,menubar=no,location=no,scrollbars=no,resizable=yes,width=1000,height=550');</script>");
    }
    protected void gvpbProjectCost_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                if (e.Row.Cells[i].Text != "" || e.Row.Cells[i].Text != "&nbsp;")
                {
                    try
                    {
                        if (e.Row.Cells[i].Text.Contains("."))
                        {
                            e.Row.Cells[i].Text = Convert.ToDecimal(e.Row.Cells[i].Text).ToString("N4");
                        }
                        else
                        {
                            e.Row.Cells[i].Text = Convert.ToInt32(e.Row.Cells[i].Text).ToString("N0");
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {
                    e.Row.Cells[i].Text = "0";
                }
            }
        }
    }
}
