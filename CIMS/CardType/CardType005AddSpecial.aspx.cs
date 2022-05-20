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

public partial class CardType_CardType005AddSpecial : PageBase
{
    CardType005BL bl = new CardType005BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        uctrlCARDNAME.Is_Using = true;
        
        if (!IsPostBack)
        {
            //新建datatable
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Priority", typeof(System.String)));
            dt.Columns.Add(new DataColumn("Factory", typeof(System.String)));
            dt.Columns.Add(new DataColumn("Value", typeof(System.String)));
            //DataRow dr = dt.NewRow();

            Session["Percentage_Number"] = dt;

            this.rablNumber_Percentage.Items[0].Selected = true;

            string strRID = Request.QueryString["RID"];

           

            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strRID))
            {
                //DataTable dtPercentage = (DataTable)Session["Percentage_Number"];
                //DataRow drowPercentage = null;

                //drowPercentage = dtPercentage.NewRow();

                //drowPercentage["Factory"] = dropFactory.SelectedValue;
                //drowPercentage["Percentage_Number"] = txtNumberValue.Text;
                //dtPercentage.Rows.Add(drowPercentage);
                //Session["Percentage_Number"] = dtPercentage;

                ////設置控件的值
                //SetControls(cbModel);
                //txtCARD_AMT.Text = cbModel.CARD_AMT.ToString("N2");
                //txtCARD_NUM.Text = cbModel.CARD_NUM.ToString("N0");
                //txtTOTAL_CARD_AMT.Text = cbModel.TOTAL_CARD_AMT.ToString("N2");
                //txtTOTAL_CARD_NUM.Text = cbModel.TOTAL_CARD_NUM.ToString("N0");
            }

        }
        //查詢沒有做特殊設定的卡種
        this.uctrlCARDNAME.SetLeftItem = CardType005BL.SEL_UNSELECTED_CARDTYPE_SPECIAL;
        uctrlCARDNAME.Is_Using = true;
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
                var aa = window.showModalDialog('CardType005SpecialDetail_Percentage.aspx?type=add', '', 'dialogHeight:450px;dialogWidth:600px;');
                if (aa != undefined) {__doPostBack('btnAddSpecial', ''); }
            }
            else
            {
                var aa = window.showModalDialog('CardType005SpecialDetail_Number.aspx?type=add', '', 'dialogHeight:450px;dialogWidth:600px;');
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
    #region 列表資料綁定
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvPerso_CardType_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtPercentage = (DataTable)Session["Percentage_Number"];

        if (dtPercentage.Rows.Count == 0)
        {
            this.hidIsModif.Value = "0";//是否修改 是=1，否=0
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(1);", true);
        }
        else
        {
            this.hidIsModif.Value = "1";//是否修改 是=1，否=0
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(2);", true);
        }

        int i = 1;
        foreach (DataRow dr in dtPercentage.Rows)
        {
            dr[0] = i;
            i++;
        }
        Session["Percentage_Number"] = dtPercentage;

        e.Table = dtPercentage;
        e.RowCount = dtPercentage.Rows.Count;
    }
    #endregion
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        PERSO_CARDTYPE pcModel = new PERSO_CARDTYPE();
        DataTable Percentage_Number = (DataTable)Session["Percentage_Number"];
        ArrayList ExistBase = new ArrayList();
        string CardTypeName = "";
        DataTable dtblCardType;
        bool isNull = false;
        bool isExist = false;
        string strPercentage_Number = "";//1:比率;2: 數量
        //增加
        try
        {
            if (uctrlCARDNAME.GetRightItem.Rows.Count > 0)
            {
                //判斷當前卡種是否在資料庫中存在基本設定
                ExistBase = bl.BaseExist(uctrlCARDNAME.GetRightItem);
                dtblCardType = uctrlCARDNAME.GetRightItem;
                if (ExistBase.Count == 0)
                {
                    ShowMessage("已選卡種都不存在基本設定");
                    return;
                }
                else if (ExistBase.Count >= 1)
                {


                    foreach (DataRow drowCardType in dtblCardType.Rows)
                    {
                        for (int i = 0; i < ExistBase.Count; i++)
                        {
                            if (drowCardType["RID"].ToString().Trim() == ExistBase[i].ToString().Trim())
                            {
                                isExist = true;
                                break;
                            }
                        }
                        if (!isExist)
                        {
                            CardTypeName += drowCardType["NAME"] + "-";
                        }
                        isExist = false;
                    }

                    if (CardTypeName != "")
                    {
                        ShowMessage("已選卡種【" + CardTypeName.Trim('-') + "】不存在基本設定");
                        return;
                    }
                }
                //else if (ExistBase.Count >= 1)
                //{
                //    for (int i = 0; i < ExistBase.Count; i++)
                //    {

                //        foreach (DataRow drowCardType in dtblCardType.Rows)
                //        {
                //            if (ExistBase[i].ToString().Trim() == drowCardType["RID"].ToString().Trim())
                //            {
                //                isExist = true;
                //                break;
                //            }
                //            if (isExist)
                //            {
                //                CardTypeName += drowCardType["NAME"] + "-";
                //            }
                //        }
                //    }
                //    ShowMessage("已選卡種【" + CardTypeName.Trim('-') + "】不存在基本設定");
                //    return;
                //}
            }
            else
            {
                ShowMessage("請選擇卡種");
                return;
            }

            //該卡種沒有新增設定明細
            if (Percentage_Number.Rows.Count == 0)
            {
                ShowMessage("該卡種沒有新增設定明細!");
                return;
            }
            //當選擇數量時，檢查設定明細列表中是否有一個廠商的數量為空。
            if (rablNumber_Percentage.Items[1].Selected)
            {
                strPercentage_Number = "2";
                foreach (DataRow drowPercentage_Number in Percentage_Number.Rows)
                {
                    if (drowPercentage_Number["Value"].ToString() == "0")
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

            bl.AddSpecial(Percentage_Number, uctrlCARDNAME.GetRightItem, strPercentage_Number);

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "CardType005AddSpecial.aspx");

            Session.Remove("Percentage_Number");
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

        this.gvPerso_CardType.BindData();
    }
    protected void gvPerso_CardType_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblCardType = (DataTable)gvPerso_CardType.DataSource;

        if (e.Row.RowType == DataControlRowType.Header)
        {
            if (rablNumber_Percentage.Items[1].Selected)
            {
                e.Row.Cells[2].Text = "數量";
            }
            if (rablNumber_Percentage.Items[0].Selected)
            {
                e.Row.Cells[2].Text = "比率";
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
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

                hl.Text = bl.GetFactory_RID(dtblCardType.Rows[e.Row.RowIndex]["Factory"].ToString());

                hl.NavigateUrl = "#";

                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('CardType005SpecialDetail_Percentage.aspx?type=update&Priority=" + e.Row.RowIndex.ToString() + "','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){__doPostBack('btnAddSpecial','');}");

            }
            else
            {
                // 修改的邦定事件
                HyperLink hl = (HyperLink)e.Row.FindControl("hlModify");

                hl.Text = bl.GetFactory_RID(dtblCardType.Rows[e.Row.RowIndex]["Factory"].ToString());

                hl.NavigateUrl = "#";

                hl.Attributes.Add("onclick", "var aa=window.showModalDialog('CardType005SpecialDetail_Number.aspx?type=update&Priority=" + e.Row.RowIndex.ToString() + "','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){__doPostBack('btnAddSpecial','');}");

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
