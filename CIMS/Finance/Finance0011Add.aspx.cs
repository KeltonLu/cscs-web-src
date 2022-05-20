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

public partial class Finance_Finance0011Add : PageBase
{
    Finance0011BL Finance0011BL = new Finance0011BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbSplitWork.PageSize = GlobalStringManager.PageSize;
        this.gvpbRequisitionWork.PageSize = GlobalStringManager.PageSize;
        this.RegisterStartupScript("js", "<script>doQuery();</script>");
        if (!IsPostBack)
        {
            dropCard_PurposeBind();// 用途下拉框綁定
            dropCard_GroupBind();// 群組下拉框綁定
            dropBlankFactoryBind();//空白卡廠下拉框綁定

            //從Seesion中獲取已保存的查詢條件
            string strCon = Request.QueryString["Con"];
            if (!StringUtil.IsEmpty(strCon))
            {
                if ((Dictionary<string, object>)Session["Condition"] != null)
                {
                    SetConData();
                }
                if (strCon == "2")
                {
                    this.functionR2.Checked = true;
                    this.RegisterStartupScript("js", "<script>doQuery();</script>");
                    gvpbRequisitionWork.BindData();
                }
                else {
                    gvpbSplitWork.BindData();    
                }
            }
            else
            {
                Session.Remove("Condition");
            }
        }
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_PurposeBind()
    {
        dropCard_Purpose.DataTextField = "PARAM_NAME";
        dropCard_Purpose.DataValueField = "Param_Code";
        dropCard_Purpose.DataSource = Finance0011BL.getParam_Use();
        dropCard_Purpose.DataBind();

        dropCard_Purpose.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_GroupBind()
    {
        dropCard_Group.Items.Clear();

        dropCard_Group.DataTextField = "GROUP_NAME";
        dropCard_Group.DataValueField = "RID";
        dropCard_Group.DataSource = Finance0011BL.getCardGroup(dropCard_Purpose.SelectedValue);
        dropCard_Group.DataBind();

        dropCard_Group.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 空白卡廠下拉框綁定
    /// </summary>
    protected void dropBlankFactoryBind()
    {
        dropBlankFactory.Items.Clear();

        dropBlankFactory.DataTextField = "Factory_ShortName_CN";
        dropBlankFactory.DataValueField = "RID";
        dropBlankFactory.DataSource = Finance0011BL.getFactory();
        dropBlankFactory.DataBind();

        dropBlankFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    protected void dropCard_Purpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_GroupBind();
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Finance0011.aspx?Con=1");
    }

    /// <summary>
    /// 拆單查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch1_Click(object sender, EventArgs e)
    {
        gvpbSplitWork.BindData();
    }

    /// <summary>
    /// 綁定拆單訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbSplitWork_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (dropCard_Group.SelectedItem.Text != "全部")
        {
            inputs.Add("dropCard_Group", dropCard_Group.SelectedValue);
        }
        else
        {
            inputs.Add("dropCard_Group", "");
        }
        inputs.Add("txtName", txtName.Text.Trim());
        if (dropBlankFactory.SelectedItem.Text != "全部")
        {
            inputs.Add("dropBlankFactory", dropBlankFactory.SelectedValue);
        }
        else
        {
            inputs.Add("dropBlankFactory", "");
        }
        inputs.Add("dropState", "");
        inputs.Add("txtBUDGET_ID", txtBUDGET_ID.Text.Trim());
        inputs.Add("txtAgreement_Code", txtAgreement_Code.Text.Trim());
        inputs.Add("txtStock_RIDYear", txtStock_RIDYear.Text.Trim());
        inputs.Add("txtStock_RID1", txtStock_RID1.Text.Trim());
        inputs.Add("txtStock_RID2", txtStock_RID2.Text.Trim());

        inputs.Add("txtBegin_Date", txtBegin_Date.Text.Trim());
        inputs.Add("txtFinish_Date", txtFinish_Date.Text.Trim());
        //保存查詢條件
        Session["Condition"] = inputs;

        DataTable dtSplitWork = null;

        try
        {
            // 取入庫\再入庫\退貨\拆分而沒有請款的入庫、再入庫、退貨記錄
            dtSplitWork = Finance0011BL.SearchDepositorySplitRecord(inputs, 
                                        e.FirstRow, 
                                        e.LastRow, 
                                        e.SortExpression, 
                                        e.SortDirection, 
                                        out intRowCount);

            if (dtSplitWork != null)//如果查到了資料
            {
                dtSplitWork.Columns.Add(new DataColumn("拆單數量", Type.GetType("System.Int32")));
                Session["dtSplitWork"] = dtSplitWork;
                e.Table = dtSplitWork;//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 拆單訊息行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbSplitWork_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtSplitWork = (DataTable)this.Session["dtSplitWork"];
        if (e.Row.RowType == DataControlRowType.Header)
        {
            // add chaoma by 201005515-0 start
            //e.Row.Cells[12].Visible = false;
            e.Row.Cells[13].Visible = false;
            // add chaoma end
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[5].Text == "1")
            {
                e.Row.Cells[5].Text = "入庫";
            }else if (e.Row.Cells[5].Text == "2")
            {
                e.Row.Cells[5].Text = "再入庫";
            }else if (e.Row.Cells[5].Text == "3")
            {
                e.Row.Cells[5].Text = "<font color='red'>退貨</font>";
                e.Row.Cells[0].Text = "<font color='red'>" + e.Row.Cells[0].Text + "</font>";
                e.Row.Cells[1].Text = "<font color='red'>" + e.Row.Cells[1].Text + "</font>";
                // add chaoma by 201005515-0 start
                //e.Row.Cells[6].Text = "<font color='red'>-" + e.Row.Cells[6].Text + "</font>";
                // Legend 2017/05/15 賦值部分  將 6 改為 7
                e.Row.Cells[7].Text = "<font color='red'>-" + e.Row.Cells[7].Text + "</font>";
                // add chaoma end
            }

            if (e.Row.RowIndex > 0)
            {
                // 如果該行是同一入庫、再入庫、退貨拆分的記錄，則不顯示數量。
                if (dtSplitWork.Rows[e.Row.RowIndex - 1]["Operate_Type"].ToString() ==
                    dtSplitWork.Rows[e.Row.RowIndex]["Operate_Type"].ToString() &&
                    dtSplitWork.Rows[e.Row.RowIndex - 1]["Operate_RID"].ToString() ==
                    dtSplitWork.Rows[e.Row.RowIndex]["Operate_RID"].ToString()
                    )
                {
                    // add chaoma by 201005515-0 start
                    e.Row.Cells[6].Text = "";
                    e.Row.Cells[7].Text = "";
                    // add chaoma end
                }
            }

            // SAP單號為空時，未請款；SAP單不為空時，已經請款，
            // 已經請款的不能再請款，也就不能再拆分了。
            // add chaoma by 201005515-0 start
            //if (e.Row.Cells[12].Text != "&nbsp;" || int.Parse(dtSplitWork.Rows[e.Row.RowIndex]["stock_rid"].ToString().Substring(0, 8)) < 20080901)// 已經請款的
            if (e.Row.Cells[13].Text != "&nbsp;" || int.Parse(dtSplitWork.Rows[e.Row.RowIndex]["stock_rid"].ToString().Substring(0, 8)) < 20080901)// 已經請款的
            // add chaoma end
            {
                TextBox txtSplit = (TextBox)e.Row.FindControl("txtSplit");
                txtSplit.Visible = false;
            }
            else
            {
                TextBox txtSplit = (TextBox)e.Row.FindControl("txtSplit");
                txtSplit.Visible = true;

                // 未拆分過的記錄，計算未稅單價
                // add chaoma by 201005515-0 start
                if (dtSplitWork.Rows[e.Row.RowIndex]["RID"].ToString() == "0")
                {
                    //e.Row.Cells[9].Text = Convert.ToString(Math.Round(Convert.ToDouble(dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"]) / 1.05, 4));
                    e.Row.Cells[10].Text = Convert.ToString(Math.Round(Convert.ToDouble(dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"]) / 1.05, 4));
                }
                else
                {
                    //e.Row.Cells[9].Text = dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price1"].ToString();
                    e.Row.Cells[10].Text = dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price1"].ToString();
                }
                // add chaoma end
            }
            // add chaoma by 201005515-0 start
            //e.Row.Cells[7].Text = Convert.ToDateTime(e.Row.Cells[8].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            //e.Row.Cells[12].Visible = false;
            e.Row.Cells[8].Text = Convert.ToDateTime(e.Row.Cells[8].Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            e.Row.Cells[13].Visible = false;

            #region Legend 2017/05/26 當 Unit_Price&Change_UnitPrice 都不為null時才比對

            decimal decUnit_Price = 0;
            decimal decChange_UnitPrice = 0;

            if(dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"] != null && dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString() != "")
            {
                decUnit_Price = Convert.ToDecimal(dtSplitWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString());
            }

            if (dtSplitWork.Rows[e.Row.RowIndex]["Change_UnitPrice"] != null && dtSplitWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString() != "")
            {
                decChange_UnitPrice = Convert.ToDecimal(dtSplitWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString());
            }

            #endregion
            if (decUnit_Price != decChange_UnitPrice)
            {
                e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
            }

            // add chaoma end
        }
    }

    /// <summary>
    /// 請款查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch2_Click(object sender, EventArgs e)
    {
        gvpbRequisitionWork.BindData();
    }

    /// <summary>
    /// 請款查詢訊息綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbRequisitionWork_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (dropCard_Group.SelectedItem.Text != "全部")
        {
            inputs.Add("dropCard_Group", dropCard_Group.SelectedValue);
        }
        else
        {
            inputs.Add("dropCard_Group", "");
        }
        inputs.Add("txtName", txtName.Text.Trim());
        if (dropBlankFactory.SelectedItem.Text != "全部")
        {
            inputs.Add("dropBlankFactory", dropBlankFactory.SelectedValue);
        }
        else
        {
            inputs.Add("dropBlankFactory", "");
        }

        // 請款狀態
        inputs.Add("dropState", dropState.SelectedValue.ToString());
        inputs.Add("txtBUDGET_ID", txtBUDGET_ID.Text.Trim());
        inputs.Add("txtAgreement_Code", txtAgreement_Code.Text.Trim());
        inputs.Add("txtStock_RIDYear", txtStock_RIDYear.Text.Trim());
        inputs.Add("txtStock_RID1", txtStock_RID1.Text.Trim());
        inputs.Add("txtStock_RID2", txtStock_RID2.Text.Trim());

        inputs.Add("txtBegin_Date", txtBegin_Date.Text.Trim());
        inputs.Add("txtFinish_Date", txtFinish_Date.Text.Trim());
        //保存查詢條件
        Session["Condition"] = inputs;

        DataTable dtRequisitionWork = null;

        try
        {
            // 取入庫\再入庫\退貨\拆分而沒有請款的入庫、再入庫、退貨記錄
            dtRequisitionWork = Finance0011BL.SearchDepositorySplitRecord(inputs, 
                                e.FirstRow, 
                                e.LastRow, 
                                e.SortExpression, 
                                e.SortDirection, 
                                out intRowCount);

            if (dtRequisitionWork != null)//如果查到了資料
            {
                Session["dtRequisitionWork"] = dtRequisitionWork;
                e.Table = dtRequisitionWork;//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 請款訊息行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbRequisitionWork_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtRequisitionWork = (DataTable)this.Session["dtRequisitionWork"];
        if (e.Row.RowType == DataControlRowType.Header)
        {
            // add chaoma by 201005515-0 start
            //e.Row.Cells[13].Visible = false;
            e.Row.Cells[14].Visible = false;
            // add chaoma end
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox cbISRequisition = new CheckBox();
            //if (e.Row.Cells[13].Text != "&nbsp;")// SAP單號不為空
            // add chaoma by 201005515-0 start
            //if (e.Row.Cells[13].Text != "&nbsp;" || int.Parse(dtRequisitionWork.Rows[e.Row.RowIndex]["stock_rid"].ToString().Substring(0, 8)) < 20080901)
            if (e.Row.Cells[14].Text != "&nbsp;" || int.Parse(dtRequisitionWork.Rows[e.Row.RowIndex]["stock_rid"].ToString().Substring(0, 8)) < 20080901)
            // add chaoma end
            {
                // 是否情況CheckBox
                cbISRequisition = (CheckBox)e.Row.FindControl("cbISRequisition");
                cbISRequisition.Enabled = false;

                // 發票CheckBox
                CheckBox cbInvoiceNumber = (CheckBox)e.Row.FindControl("cbInvoiceNumber");
                cbInvoiceNumber.Visible = false;
                Label lbInvoiceNumber = (Label)e.Row.FindControl("lbInvoiceNumber");
                lbInvoiceNumber.Text = dtRequisitionWork.Rows[e.Row.RowIndex]["Check_ID"].ToString();
                lbInvoiceNumber.Visible = true;
            }

            if (e.Row.Cells[5].Text == "1")
            {
                e.Row.Cells[5].Text = "入庫";
            }else if (e.Row.Cells[5].Text == "2")
            {
                e.Row.Cells[5].Text = "再入庫";
            }else if (e.Row.Cells[5].Text == "3")
            {
                e.Row.Cells[5].Text = "<font color='red'>退貨</font>";
                e.Row.Cells[0].Text = "<font color='red'>" + e.Row.Cells[0].Text + "</font>";
                e.Row.Cells[1].Text = "<font color='red'>" + e.Row.Cells[1].Text + "</font>";
                // add chaoma by 201005515-0 start
                //e.Row.Cells[6].Text = "<font color='red'>-" + e.Row.Cells[6].Text + "</font>";
                // Legend 2017/06/07 賦值部分  將 6 改為 7
                e.Row.Cells[7].Text = "<font color='red'>-" + e.Row.Cells[7].Text + "</font>";
                // add chaoma end
            }

            if (e.Row.RowIndex > 0)
            {
                // 如果該行和上一行的RID相同，則不顯示數量。
                if (dtRequisitionWork.Rows[e.Row.RowIndex - 1]["Operate_Type"].ToString() ==
                    dtRequisitionWork.Rows[e.Row.RowIndex]["Operate_Type"].ToString() &&
                    dtRequisitionWork.Rows[e.Row.RowIndex - 1]["Operate_RID"].ToString() ==
                    dtRequisitionWork.Rows[e.Row.RowIndex]["Operate_RID"].ToString()
                    )
                {
                    // add chaoma by 201005515-0 start
                    e.Row.Cells[6].Text = "";
                    e.Row.Cells[7].Text = "";
                    // add chaoma end
                }
            }
            
            // 如果是入庫、再入庫、退貨記錄時
            // add chaoma by 201005515-0 start
            if (dtRequisitionWork.Rows[e.Row.RowIndex]["RID"].ToString() == "0")
            {
                //e.Row.Cells[9].Text = Convert.ToDecimal(Math.Round(Convert.ToDouble(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"]) / 1.05, 4)).ToString("N4");

                //if (e.Row.Cells[9].Text == "0")
                //    e.Row.Cells[9].Text = "0.0000";
                e.Row.Cells[10].Text = Convert.ToDecimal(Math.Round(Convert.ToDouble(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"]) / 1.05, 4)).ToString("N4");

                if (e.Row.Cells[10].Text == "0")
                    e.Row.Cells[10].Text = "0.0000";
            }
            else
            {
                //if (dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price1"].ToString() != "")
                //    e.Row.Cells[9].Text = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price1"].ToString()).ToString("N4");
                //else
                //    e.Row.Cells[9].Text = "0.0000";
                if (dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price1"].ToString() != "")
                    e.Row.Cells[10].Text = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price1"].ToString()).ToString("N4");
                else
                    e.Row.Cells[10].Text = "0.0000";
            }
            // 日期
            //e.Row.Cells[7].Text = Convert.ToDateTime(dtRequisitionWork.Rows[e.Row.RowIndex]["Income_Date"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            //e.Row.Cells[13].Visible = false;
            e.Row.Cells[8].Text = Convert.ToDateTime(dtRequisitionWork.Rows[e.Row.RowIndex]["Income_Date"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            e.Row.Cells[14].Visible = false;

            #region Legend 2017/05/26 當 Unit_Price&Change_UnitPrice 都不為null時才比對

            decimal decUnit_Price = 0;
            decimal decChange_UnitPrice = 0;

            if (dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"] != null && dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString() != "")
            {
                decUnit_Price = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Unit_Price"].ToString());
            }

            if (dtRequisitionWork.Rows[e.Row.RowIndex]["Change_UnitPrice"] != null && dtRequisitionWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString() != "")
            {
                decChange_UnitPrice = Convert.ToDecimal(dtRequisitionWork.Rows[e.Row.RowIndex]["Change_UnitPrice"].ToString());
            }

            #endregion
            if (decUnit_Price != decChange_UnitPrice)
            {
                e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
            } 

            //add chaoma end
        }
    }

    /// <summary>
    /// 填寫發票號
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnInvoiceNumber_Click(object sender, EventArgs e)
    {
        CheckBox cbInvoiceNumber = new CheckBox();
        Label lbInvoiceNumber = new Label();
        bool blChecked = false;
        for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
        {
            cbInvoiceNumber = (CheckBox)gvpbRequisitionWork.Rows[i].FindControl("cbInvoiceNumber");
            if (cbInvoiceNumber.Checked == true)
            {
                blChecked = true;
            }
        }

        if (!blChecked)
        {
            ShowMessage("沒有選擇要設置的發票編號，請先選擇。");
            return;
        }

        if (this.txtInvoiceNumber.Text.Trim() == "")
        {
            ShowMessage("發票編號不能為空，請先填寫發票編號。");
            return;
        }

        // 設置發票編號到對應的欄目
        for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
        {
            cbInvoiceNumber = (CheckBox)gvpbRequisitionWork.Rows[i].FindControl("cbInvoiceNumber");
            lbInvoiceNumber = (Label)gvpbRequisitionWork.Rows[i].FindControl("lbInvoiceNumber");
            if (cbInvoiceNumber.Checked == true)
            {
                cbInvoiceNumber.Checked = false;
                lbInvoiceNumber.Text = txtInvoiceNumber.Text;
            }
        }

        txtInvoiceNumber.Text = "";
    }

    /// <summary>
    /// 拆單確認
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        DataTable dtSplitWork = (DataTable)Session["dtSplitWork"];
        TextBox txtSplit = new TextBox();
        bool blHaveSplitRecord = false;
        for (int i = 0; i < gvpbSplitWork.Rows.Count; i++)
        {
            // 保存拆單數量
            txtSplit = (TextBox)gvpbSplitWork.Rows[i].FindControl("txtSplit");
            if (txtSplit.Text != "")
            {
                if (Convert.ToInt16(txtSplit.Text) == 0)
                {
                    ShowMessage("拆單數量必須大於0或為空!");
                    txtSplit.Focus();
                    return;
                }

                blHaveSplitRecord = true;
                dtSplitWork.Rows[i]["拆單數量"] = txtSplit.Text;

                // 如果dtSplitWork.Rows[i]["SAP單號"]為空，
                dtSplitWork.Rows[i]["Unit_Price1"] = Decimal.Parse(this.gvpbSplitWork.Rows[i].Cells[9].Text);

                // 計算遲交天數(入庫且是第一次拆單時)
                if (dtSplitWork.Rows[i]["Operate_Type"].ToString() == "1" &&
                    dtSplitWork.Rows[i]["RID"].ToString() == "0")
                { 
                    TimeSpan ts = Convert.ToDateTime(dtSplitWork.Rows[i]["Income_Date"].ToString()) - 
                                Convert.ToDateTime(dtSplitWork.Rows[i]["Fore_Delivery_Date"].ToString());
                    dtSplitWork.Rows[i]["Delay_Days"] = ts.TotalDays;
                }
            }
        }

        // 沒有點選拆分記錄，不進入拆分確認畫面。
        if (!blHaveSplitRecord)
        {
            ShowMessage("沒有點選要拆分的記錄。");
            return;
        }

        Session["dtSplitWork"] = dtSplitWork;
        Response.Redirect("Finance0011Add_1.aspx?ActionType=Add");
    }

    /// <summary>
    /// 請款確認
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit2_Click(object sender, EventArgs e)
    {
        // 檢查是否有選擇了要請款的項目
        bool blHave=false;
        // 發票號不能和代制費用請款發票號相重復
        DataTable dtCheckAllCheckSerialNumber = null;
        dtCheckAllCheckSerialNumber = Finance0011BL.getAllCheckSerialNumber();

        Label lblInvoiceNumber = new Label();
        CheckBox cbISRequisition = new CheckBox();
        // 每個要請款的項目必須輸入發票號，且發票和系統中其他發票不能重復
        for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
        {
            lblInvoiceNumber = (Label)gvpbRequisitionWork.Rows[i].FindControl("lbInvoiceNumber");
            cbISRequisition = (CheckBox)gvpbRequisitionWork.Rows[i].FindControl("cbISRequisition");
            // 該行被選擇，又沒有輸入發票編號
            if (cbISRequisition.Checked)
            {
                blHave = true;
                if (lblInvoiceNumber.Text.Trim() == "")
                {
                    ShowMessage("每個要請款的項目必須輸入發票號。");
                    return;
                }
                else
                {
                    if (dtCheckAllCheckSerialNumber != null && 
                        dtCheckAllCheckSerialNumber.Rows.Count > 0)
                    {
                        DataRow[] drCheckSerialNumber = dtCheckAllCheckSerialNumber.Select("Check_Serial_Number = '" + lblInvoiceNumber.Text.Trim() + "'");
                        if (drCheckSerialNumber.Length > 0)
                        {
                            ShowMessage("發票號不能和代製費用請款已經存在的發票號相同。");
                            return;
                        }
                    }
                }
            }
        }

        // 檢查是否選擇要請款的記錄
        if (!blHave)
        {
            ShowMessage("沒有點選要請款的記錄，請點選。");
            return;
        }

        // 同一次請款必須是，同一Perso廠的同一帳務群組的卡種，因為5.4.1的需要
        DataTable dtRequisitionWork = (DataTable)Session["dtRequisitionWork"];
        string strPerso_Factory_RID = "";
        string strCardGroupRID = "";
        for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
        { 
            cbISRequisition = (CheckBox)gvpbRequisitionWork.Rows[i].FindControl("cbISRequisition");
            if (cbISRequisition.Checked)
            {
                // 同一空白卡廠檢查
                if (strPerso_Factory_RID == "")
                {
                    strPerso_Factory_RID = dtRequisitionWork.Rows[i]["FRID"].ToString();
                }
                else
                {
                    if (strPerso_Factory_RID != dtRequisitionWork.Rows[i]["FRID"].ToString())
                    {
                        ShowMessage("必須是同一空白卡廠！");
                        return;
                    }
                }
                
                // 同一帳務群組檢查
                string strGroupRID = Finance0011BL.getCardGroupRIDByCardTypeRID(dtRequisitionWork.Rows[i]["CARDTYPE_RID"].ToString());
                if (strGroupRID == "")
                {
                    ShowMessage("卡種沒有關聯的帳務群組！");
                    return;
                }

                if (strCardGroupRID == "")
                {
                    strCardGroupRID = strGroupRID;
                }
                else if (strCardGroupRID != strGroupRID)
                {
                    ShowMessage("必須是同一帳務群組的卡種請款！");
                    return;
                }
            }
        }

        // 保存頁面訊息到Session中
        if (dtRequisitionWork.Columns.Contains("是否請款") == false)
        {
            dtRequisitionWork.Columns.Add(new DataColumn("是否請款", typeof(string)));
        }
        for (int i = 0; i < gvpbRequisitionWork.Rows.Count; i++)
        {
            lblInvoiceNumber = (Label)gvpbRequisitionWork.Rows[i].FindControl("lbInvoiceNumber");
            cbISRequisition = (CheckBox)gvpbRequisitionWork.Rows[i].FindControl("cbISRequisition");
            if (cbISRequisition.Checked)
            {
                dtRequisitionWork.Rows[i]["Check_ID"] = lblInvoiceNumber.Text;
                dtRequisitionWork.Rows[i]["是否請款"] = cbISRequisition.Checked.ToString();
            }
        }

        Session["dtRequisitionWork"] = dtRequisitionWork;
        Response.Redirect("Finance0011Add_2.aspx?ActionType=Add");
    }
}
