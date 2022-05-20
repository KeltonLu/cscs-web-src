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

public partial class CardType_CardType005ModSpecial : PageBase
{
    CardType005BL bl = new CardType005BL();
    bool IsDel = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //新建datatable
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Priority", typeof(System.String)));
            dt.Columns.Add(new DataColumn("Factory", typeof(System.String)));
            dt.Columns.Add(new DataColumn("Value", typeof(System.String)));
            //DataRow dr = dt.NewRow();

            Session["Percentage_Number"] = dt;


            string strCardType_RID = Request.QueryString["CardType_RID"];
            if (!StringUtil.IsEmpty(strCardType_RID))
            {
                this.lbCardType.Text = bl.GetCardTypeName(strCardType_RID);
                if (Request.QueryString["Percentage_Number"] != "")
                {
                    chkDel.Visible = true;
                    if (Request.QueryString["Percentage_Number"] == "1")
                    {
                        this.rablNumber_Percentage.Items[0].Selected = true;
                        this.rablNumber_Percentage.Items[1].Selected = false;
                    }
                    else
                    {
                        this.rablNumber_Percentage.Items[0].Selected = false;
                        this.rablNumber_Percentage.Items[1].Selected = true;
                    }
                }
                else
                {
                    chkDel.Visible = false;
                    this.rablNumber_Percentage.Items[0].Selected = true;
                    this.rablNumber_Percentage.Items[1].Selected = false;
                }

            }
            this.gvPerso_CardType.BindData();
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string strUrl = @"
                            var a = document.getElementById('rablNumber_Percentage').rows[0].cells.length; 


        for (var i = 0; i < a; i++)
        {
            var ss = 'rablNumber_Percentage_' + i;
            var aa = document.getElementById(ss).value;
            if (document.getElementById(ss).checked)
                             {

            if (aa == '比率')
            {
                var aa = window.showModalDialog('CardType005ModSpecial_Percentage.aspx?type=add', '', 'dialogHeight:450px;dialogWidth:600px;');
                if (aa != undefined) {__doPostBack('btnAddSpecial', '');}
            }
            else
            {
                var aa = window.showModalDialog('CardType005ModSpecial_Number.aspx?type=add', '', 'dialogHeight:450px;dialogWidth:600px;');
                if (aa != undefined)
                {
                   __doPostBack('btnAddSpecial', '');
                }
            }
            break;
        }

    }";

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), strUrl, true);

    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        try
        {
            PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
            string strRID = Request.QueryString["RID"];
            string strCardType_RID = Request.QueryString["CardType_RID"];

            DataTable Percentage_Number = (DataTable)Session["Percentage_Number"];
            //todo
            string strPercentage_Number = "";//1:比率;2: 數量
            bool isNull = false;
            // 刪除
            if (this.chkDel.Checked)
            {
                bl.DeleteSpecial(strCardType_RID);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "CardType005.aspx?Con=1");
                Session.Remove("Percentage_Number");
                return;
            }
            //當選擇數量時，檢查設定明細列表中是否有一個廠商的數量為空。
            if (rablNumber_Percentage.Items[1].Selected)
            {
                strPercentage_Number = "2";
                foreach (DataRow drowPercentage_Number in Percentage_Number.Rows)
                {
                    if (StringUtil.IsEmpty(drowPercentage_Number["Value"].ToString()) || drowPercentage_Number["Value"].ToString() == "0")
                    {
                        isNull = true;
                        break;
                    }
                }
                if (!isNull)
                {
                    ShowMessage("Perso廠數量必須有一個為空！");
                    return;
                }
            }
            //else if (this.radPercentage.Checked)
            else if (rablNumber_Percentage.Items[0].Selected)
            {
                strPercentage_Number = "1";
                decimal Percentage = 0.00M;
                foreach (DataRow drowPercentage_Number in Percentage_Number.Rows)
                {
                    if (!StringUtil.IsEmpty(drowPercentage_Number["Value"].ToString()))
                        Percentage += Convert.ToDecimal(drowPercentage_Number["Value"].ToString().Trim('%'));
                }
                if (Percentage != 100)
                {
                    ShowMessage("各Perso廠的比率之和必須為100%！");
                    return;
                }
            }

            if (!StringUtil.IsEmpty(strRID))
            {
                // 保存
                if (this.chkDel.Checked == false)
                {
                    bl.AddSpecialPercentage(strCardType_RID, Percentage_Number, strPercentage_Number);
                    ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "CardType005.aspx?Con=1");
                    Session.Remove("Percentage_Number");
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CardType005.aspx?Con=1");
    }
    protected void btnAddSpecial_Click(object sender, EventArgs e)
    {

        this.gvPerso_CardType.BindData();
    }
    #region 列表資料綁定
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvPerso_CardType_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtPercentage = (DataTable)Session["Percentage_Number"];



        int intRowCount = 0;
        int i = 1;
        foreach (DataRow dr in dtPercentage.Rows)
        {
            dr["Priority"] = i;
            i++;
        }


        string strCardType_RID = Request.QueryString["CardType_RID"];

        if (!StringUtil.IsEmpty(strCardType_RID))
        {
            try
            {
                if (IsDel == false)
                {
                    if (dtPercentage.Rows.Count == 0)
                    {
                        dtPercentage = bl.GetPersoCardTypeList(strCardType_RID).Tables[0];
                    }
                }

                //影藏查無資料
                if (dtPercentage.Rows.Count == 0)
                {
                    //this.hidIsModif.Value = "0";//是否修改 是=1，否=0
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(1);", true);
                }
                else
                {
                    //this.hidIsModif.Value = "1";//是否修改 是=1，否=0
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(2);", true);
                }

                if (dtPercentage != null)//如果查到了資料
                {
                    Session["Percentage_Number"] = dtPercentage;

                    e.Table = dtPercentage;//要綁定的資料表
                    e.RowCount = intRowCount;//查到的行數
                }
            }
            catch (Exception ex)
            {
                ExceptionFactory.CreateAlertException(this, ex.Message);
            }
        }


        //if (!StringUtil.IsEmpty(strCardType_RID))
        //{
        //    DataSet dsPersoCardTypeList = null;
        //    try
        //    {
        //        dsPersoCardTypeList = bl.GetPersoCardTypeList(strCardType_RID);
        //        if (dsPersoCardTypeList != null)//如果查到了資料
        //        {
        //            if (dtPercentage.Rows.Count != 0)
        //            {
        //                dsPersoCardTypeList.Merge(dtPercentage);//todo
        //            }
        //            e.Table = dsPersoCardTypeList.Tables[0];//要綁定的資料表
        //            e.RowCount = intRowCount;//查到的行數
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionFactory.CreateAlertException(this, ex.Message);
        //    }
        //}
    }
    #endregion

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtPercentage = (DataTable)Session["Percentage_Number"];
        DataRow drowPercentage = dtPercentage.Rows[Convert.ToInt32(e.CommandArgument)];

        dtPercentage.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));


        Session["Percentage_Number"] = dtPercentage;
        IsDel = true;

        this.gvPerso_CardType.BindData();
    }
    protected void gvPerso_CardType_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblCardType = (DataTable)gvPerso_CardType.DataSource;

        if (e.Row.RowType == DataControlRowType.Header)
        {
            //if (radNumber.Checked)
            if (rablNumber_Percentage.Items[1].Selected)
            {
                e.Row.Cells[2].Text = "數量";
            }
            //if (radPercentage.Checked)
            if (rablNumber_Percentage.Items[0].Selected)
            {
                e.Row.Cells[2].Text = "比率";
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //if (radPercentage.Checked)
            if (rablNumber_Percentage.Items[0].Selected)
            {
                if (!StringUtil.IsEmpty(e.Row.Cells[2].Text) && e.Row.Cells[2].Text != "&nbsp;")
                {
                    decimal Percentage = 0.00M;
                    Percentage = Convert.ToDecimal(e.Row.Cells[2].Text);
                    e.Row.Cells[2].Text = Percentage.ToString() + "%";
                }

                // 修改的邦定事件
                HyperLink hl = (HyperLink)e.Row.FindControl("hlModify");

                hl.Text = bl.GetFactory_RID(dtblCardType.Rows[e.Row.RowIndex]["Factory_RID"].ToString());

                hl.NavigateUrl = "#";

                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('CardType005ModSpecial_Percentage.aspx?type=update&Priority=" + e.Row.RowIndex.ToString() + "','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){__doPostBack('btnAddSpecial','');}");

            }
            else
            {
                // 修改的邦定事件
                HyperLink hl = (HyperLink)e.Row.FindControl("hlModify");

                hl.Text = bl.GetFactory_RID(dtblCardType.Rows[e.Row.RowIndex]["Factory_RID"].ToString());

                hl.NavigateUrl = "#";

                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('CardType005ModSpecial_Number.aspx?type=update&Priority=" + e.Row.RowIndex.ToString() + "','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){__doPostBack('btnAddSpecial','');}");
            }

            //優先級
            //e.Row.Cells[0].Text = (e.Row.RowIndex + 1).ToString();

            Button btnButton = null;
            // 刪除的邦定事件
            btnButton = (Button)e.Row.FindControl("btnDelete");
            btnButton.CommandArgument = e.Row.RowIndex.ToString();
            btnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
        }
    }

    protected void btnDelSpecial_Click(object sender, EventArgs e)
    {
        //刪除操作
        //string strCardType_RID = Request.QueryString["CardType_RID"];
        //if (!StringUtil.IsEmpty(strCardType_RID))
        //{
        //    bl.DeleteSpecial(strCardType_RID);
        //}
        //DataTable dtPercentage = (DataTable)Session["Percentage_Number"];

        //if (dtPercentage != null)
        //{
        //    int dtPercentageRowCount = dtPercentage.Rows.Count;
        //    for (int i = 0; i < dtPercentageRowCount; i++)
        //    {
        //        DataRow drowPercentage = dtPercentage.Rows[0];

        //        dtPercentage.Rows.RemoveAt(0);
        //    }
        //}

        //Session["Percentage_Number"] = dtPercentage;

        //this.gvPerso_CardType.BindData();


        DataTable dtPercentage = (DataTable)Session["Percentage_Number"];

        if (dtPercentage.Rows.Count > 0)
        {
            int dtPercentageRowCount = dtPercentage.Rows.Count;
            for (int i = 0; i < dtPercentageRowCount; i++)
            {
                DataRow drowPercentage = dtPercentage.Rows[0];

                dtPercentage.Rows.RemoveAt(0);
            }
        }

        Session["Percentage_Number"] = dtPercentage;


        IsDel = true;
        this.gvPerso_CardType.BindData();
    }
    protected void btnIsChecked_rad_Click(object sender, EventArgs e)
    {
        //if (this.radNumber.Checked)
        if (rablNumber_Percentage.Items[1].Selected)
        {
            //this.radNumber.Checked = false;
            rablNumber_Percentage.Items[1].Selected = false;
            //this.radPercentage.Checked = true;
            rablNumber_Percentage.Items[0].Selected = true;
            this.gvPerso_CardType.BindData();
            return;
        }
        //if (this.radPercentage.Checked)
        if (rablNumber_Percentage.Items[0].Selected)
        {
            //this.radPercentage.Checked = false;
            rablNumber_Percentage.Items[0].Selected = false;
            //this.radNumber.Checked = true;
            rablNumber_Percentage.Items[1].Selected = true;
            this.gvPerso_CardType.BindData();
        }
    }
    protected void rablNumber_Percentage_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (this.rablNumber_Percentage.Items[0].Selected == true)
        //{
        //    this.radPercentage.Checked = true;
        //    this.radNumber.Checked = false;
        //}
        //if (this.rablNumber_Percentage.Items[1].Selected == true)
        //{
        //    this.radNumber.Checked = true;
        //    this.radPercentage.Checked = false;
        //}

        DataTable dtPercentage = (DataTable)Session["Percentage_Number"];

        if (dtPercentage.Rows.Count == 0)
        {
            this.hidIsModif.Value = "0";//是否修改 是=1，否=0
        }
        else
        {
            this.hidIsModif.Value = "1";//是否修改 是=1，否=0
        }
        ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "doPopDelSpecailDetail();", true);
    }
}
