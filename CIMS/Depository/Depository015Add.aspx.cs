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

public partial class Depository_Depository015Add : PageBase
{
    Depository015BL bl = new Depository015BL();

    protected void Page_Load(object sender, EventArgs e)
    {        
        if (!IsPostBack)
        {
            //預設為當前系統日期
            txtPurchaseDate.Text = DateTime.Today.ToString("yyyy/MM/dd");
            

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("Name", Type.GetType("System.String"));
            dtbl.Columns.Add("Detail_RID");
            dtbl.Columns.Add("Serial_Number", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory_RID1", Type.GetType("System.String"));
            dtbl.Columns.Add("Number1", Type.GetType("System.String"));
            dtbl.Columns.Add("Delivery_Date1", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory1", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory_RID2", Type.GetType("System.String"));
            dtbl.Columns.Add("Number2", Type.GetType("System.String"));
            dtbl.Columns.Add("Delivery_Date2", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory2", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory_RID3", Type.GetType("System.String"));
            dtbl.Columns.Add("Number3", Type.GetType("System.String"));
            dtbl.Columns.Add("Delivery_Date3", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory3", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory_RID4", Type.GetType("System.String"));
            dtbl.Columns.Add("Number4", Type.GetType("System.String"));
            dtbl.Columns.Add("Delivery_Date4", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory4", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory_RID5", Type.GetType("System.String"));
            dtbl.Columns.Add("Number5", Type.GetType("System.String"));
            dtbl.Columns.Add("Delivery_Date5", Type.GetType("System.String"));
            dtbl.Columns.Add("Factory5", Type.GetType("System.String"));
            dtbl.Columns.Add("Case_Date", Type.GetType("System.String"));
            dtbl.Columns.Add("Comment", Type.GetType("System.String"));
            dtbl.Columns.Add("Unit_Price");
            dtbl.Columns.Add("Total_Num");
            dtbl.Columns.Add("Total_Price");
            dtbl.Columns.Add("SAP_Serial_Number", Type.GetType("System.String"));
            dtbl.Columns.Add("Ask_Date", Type.GetType("System.String"));
            dtbl.Columns.Add("Pay_Date", Type.GetType("System.String"));

            Session["Purchase"] = dtbl;//存儲UI中的信息            
        }
    }

    /// <summary>
    /// 新增明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddDetail_Click(object sender, EventArgs e)
    {        
        if (txtPurchaseDate.Text == "")
        {
            ShowMessage("採購日期不能為空");
            return;
        }
        else
        {
            Session["Purchase_Date"] = txtPurchaseDate.Text;
            txtPurchaseDate.Enabled = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=window.showModalDialog('Depository015Detail.aspx?ActionType=Add&Index=-1','','dialogHeight:600px;dialogWidth:1000px;');if(aa!=undefined){ImtBind();}", true);
        }
    }

    /// <summary>
    ///  確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // 介面資料檢查
        DataSet PurchaseByYear = bl.GetPurchaseByYear(txtPurchaseDate.Text.Trim().Substring(0, 4));
        decimal exponentKa = 0;
        decimal exponentYin = 0;
        decimal envelopeKa = 0;
        decimal envelopeYin = 0;
        decimal dmKa = 0;
        decimal dmYin = 0;              

        DataTable dtbl = (DataTable)Session["Purchase"];

        foreach (DataRow dr in PurchaseByYear.Tables[0].Rows)
        {
            DataSet ds = bl.GetMaterielInfo(dr["Serial_Number"].ToString());
            string BillingType="";
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                BillingType = ds.Tables[0].Rows[0]["Billing_Type"].ToString();
            if (dr["Serial_Number"].ToString().Substring(0, 1) == "A" && BillingType == "1")            
                envelopeKa += Convert.ToDecimal(dr["Total_Price"].ToString());            
            else if(dr["Serial_Number"].ToString().Substring(0, 1) == "A" && BillingType == "2")
                envelopeYin += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "B" && BillingType == "1")
                exponentKa += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "B" && BillingType == "2")
                exponentYin += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "C" && BillingType == "1")
                dmKa += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "C" && BillingType == "2")
                dmYin += Convert.ToDecimal(dr["Total_Price"].ToString());
        }

        foreach (DataRow dr in dtbl.Rows)
        {
            DataSet ds = bl.GetMaterielInfo(dr["Serial_Number"].ToString());
            string BillingType = "";
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                BillingType = ds.Tables[0].Rows[0]["Billing_Type"].ToString();
            if (dr["Serial_Number"].ToString().Substring(0, 1) == "A" && BillingType == "1")
                envelopeKa += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "A" && BillingType == "2")
                envelopeYin += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "B" && BillingType == "1")
                exponentKa += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "B" && BillingType == "2")
                exponentYin += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "C" && BillingType == "1")
                dmKa += Convert.ToDecimal(dr["Total_Price"].ToString());
            else if (dr["Serial_Number"].ToString().Substring(0, 1) == "C" && BillingType == "2")
                dmYin += Convert.ToDecimal(dr["Total_Price"].ToString());
        }

        DataSet MaterielBudget = bl.GetMaterielBudget(txtPurchaseDate.Text.Trim().Substring(0, 4));

        foreach (DataRow dr in MaterielBudget.Tables[0].Rows)
        {
            if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.EXPONENT_CARD)
            {
                if(exponentKa > Convert.ToDecimal(dr["Budget"].ToString()))
                {
                    ShowMessage("寄卡單（卡）採購金額超過該年度預算，無法儲存");
                    return;
                }
                continue;
            }
            if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.EXPONENT_BANK)
            {
                if(exponentYin > Convert.ToDecimal(dr["Budget"].ToString()))
                {
                    ShowMessage("寄卡單（銀）採購金額超過該年度預算，無法儲存");
                    return;
                }
                continue;
            }
            if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.ENVELOPE_CARD)
            {
                if (envelopeKa > Convert.ToDecimal(dr["Budget"].ToString()))
                {
                    ShowMessage("信封（卡）採購金額超過該年度預算，無法儲存");
                    return;
                }
                continue;
            }
            if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.ENVELOPE_BANK)
            {
                if (envelopeYin > Convert.ToDecimal(dr["Budget"].ToString()))
                {                  
                    ShowMessage("信封（銀）採購金額超過該年度預算，無法儲存");
                    return;
                }
                continue;
            }
            if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.DM_CARD)
            {
                if (dmKa > Convert.ToDecimal(dr["Budget"].ToString()))
                {
                    ShowMessage("DM（卡）採購金額超過該年度預算，無法儲存");
                    return;
                }
                continue;
            }
            if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.DM_BANK)
            {
                if (dmYin > Convert.ToDecimal(dr["Budget"].ToString()))
                {
                    ShowMessage("DM（銀）採購金額超過該年度預算，無法儲存");
                    return;
                }
                continue;
            }
        } 

        if (dtbl.Rows.Count == 0)
        {
            ShowMessage("無添加列");
            return;
        }

        try
        {
            // 將UI訊息保存到資料庫中
            bl.Add(dtbl, txtPurchaseDate.Text);
            //操作日誌
            bl.SetOprLog("2");

            //發送警訊“XX/XX  物料新增採購作業，請至系統內作請款作業”
            string[] argF = new string[2];
            argF[0] = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString();
            argF[1] = "新增";
            Warning.SetWarning(GlobalString.WarningType.PlsAskFinance, argF);
            foreach (DataRow dr in MaterielBudget.Tables[0].Rows)
            {
                if (dr["Budget"].ToString() == "0")
                    continue;

                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.EXPONENT_CARD)
                {
                    if ((dr["Budget"].ToString() != "0") && 
                        ((Convert.ToDecimal(dr["Budget"].ToString()) - exponentKa) / Convert.ToDecimal(dr["Budget"].ToString())) <= 0.1M)
                    {
                        string[] arg = new string[1];
                        arg[0] = "寄卡單（卡）";
                        Warning.SetWarning(GlobalString.WarningType.SafeWarning, arg);
                        ShowMessage("寄卡單（卡）已超過年度預算安全水位");                        
                    }
                    continue;
                }
                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.EXPONENT_BANK)
                {
                    if ((dr["Budget"].ToString() != "0") && 
                        ((Convert.ToDecimal(dr["Budget"].ToString()) - exponentYin) / Convert.ToDecimal(dr["Budget"].ToString())) <= 0.1M)
                    {
                        string[] arg = new string[1];
                        arg[0] = "寄卡單（銀）";
                        Warning.SetWarning(GlobalString.WarningType.SafeWarning, arg);
                        ShowMessage("寄卡單（銀）已超過年度預算安全水位");
                        return;
                    }
                    continue;
                }
                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.ENVELOPE_CARD)
                {
                    if ((dr["Budget"].ToString() != "0") && 
                        ((Convert.ToDecimal(dr["Budget"].ToString()) - envelopeKa) / Convert.ToDecimal(dr["Budget"].ToString())) <= 0.1M)
                    {
                        string[] arg = new string[1];
                        arg[0] = "信封（卡）";
                        Warning.SetWarning(GlobalString.WarningType.SafeWarning, arg);
                        ShowMessage("信封（卡）已超過年度預算安全水位");                        
                    }
                    continue;
                }
                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.ENVELOPE_BANK)
                {
                    if ((dr["Budget"].ToString() != "0") && 
                        ((Convert.ToDecimal(dr["Budget"].ToString()) - envelopeYin) / Convert.ToDecimal(dr["Budget"].ToString())) <= 0.1M)
                    {
                        string[] arg = new string[1];
                        arg[0] = "信封（銀）";
                        Warning.SetWarning(GlobalString.WarningType.SafeWarning, arg);
                        ShowMessage("信封（銀）已超過年度預算安全水位");                        
                    }
                    continue;
                }
                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.DM_CARD)
                {
                    if ((dr["Budget"].ToString() != "0") && 
                        ((Convert.ToDecimal(dr["Budget"].ToString()) - dmKa) / Convert.ToDecimal(dr["Budget"].ToString())) <= 0.1M)
                    {
                        string[] arg = new string[1];
                        arg[0] = "DM（卡）";
                        Warning.SetWarning(GlobalString.WarningType.SafeWarning, arg);
                        ShowMessage("DM（卡）已超過年度預算安全水位");                        
                    }
                    continue;
                }
                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.DM_BANK)
                {
                    if ((dr["Budget"].ToString() != "0") && 
                        ((Convert.ToDecimal(dr["Budget"].ToString()) - dmYin) / Convert.ToDecimal(dr["Budget"].ToString())) <= 0.1M)
                    {
                        string[] arg = new string[1];
                        arg[0] = "DM（銀）";
                        Warning.SetWarning(GlobalString.WarningType.SafeWarning, arg);
                        ShowMessage("DM（銀）已超過年度預算安全水位");                        
                    }
                    continue;
                }
            }


            Session.Remove("PurchaseOrder_RID");
            Session.Remove("Purchase_Date");
            Session.Remove("Purchase");
            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Depository015.aspx?Con=1");
        }
        catch (Exception ex)
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
        Session.Remove("PurchaseOrder_RID");
        Session.Remove("Purchase_Date");
        Session.Remove("Purchase");
        Response.Redirect("Depository015.aspx?Con=1");
    }

    /// <summary>
    /// Repeater列資料綁定
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        Button btnUpdate = null;
        Button btnDelete = null;
        // 修改的邦定事件
        btnUpdate = (Button)e.Item.FindControl("btnUpdateDetail");
        btnUpdate.CommandArgument = e.Item.ItemIndex.ToString();
        // 刪除的邦定事件
        btnDelete=(Button) e.Item.FindControl("btnDeleteDetail");
        btnDelete.CommandArgument = e.Item.ItemIndex.ToString();
        btnDelete.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
    }
    
    /// <summary>
    /// 綁定Repeater控件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        RepeaterBind();
    }

    private void RepeaterBind()
    {
        DataTable dtbl = new DataTable();
        dtbl = ((DataTable)Session["Purchase"]).Clone();

        foreach (DataRow drowSession in ((DataTable)Session["Purchase"]).Rows)
        {
            DataRow drow = dtbl.NewRow();
            drow.ItemArray = drowSession.ItemArray;
            if (!StringUtil.IsEmpty(drow["Number1"].ToString()))
                drow["Number1"] = Convert.ToInt32(drow["Number1"]).ToString("N0");
            if (!StringUtil.IsEmpty(drow["Number2"].ToString()))
                drow["Number2"] = Convert.ToInt32(drow["Number2"]).ToString("N0");
            if (!StringUtil.IsEmpty(drow["Number3"].ToString()))
                drow["Number3"] = Convert.ToInt32(drow["Number3"]).ToString("N0");
            if (!StringUtil.IsEmpty(drow["Number4"].ToString()))
                drow["Number4"] = Convert.ToInt32(drow["Number4"]).ToString("N0");
            if (!StringUtil.IsEmpty(drow["Number5"].ToString()))
                drow["Number5"] = Convert.ToInt32(drow["Number5"]).ToString("N0");

            if (!StringUtil.IsEmpty(drow["Unit_Price"].ToString()))
                drow["Unit_Price"] = Convert.ToDecimal(drow["Unit_Price"]).ToString("N2");

            if (!StringUtil.IsEmpty(drow["Total_Num"].ToString()))
                drow["Total_Num"] = Convert.ToInt64(drow["Total_Num"]).ToString("N0");

            if (!StringUtil.IsEmpty(drow["Total_Price"].ToString()))
                drow["Total_Price"] = Convert.ToDecimal(drow["Total_Price"]).ToString("N2");



            dtbl.Rows.Add(drow);
        }

        Repeater1.DataSource = dtbl;
        Repeater1.DataBind();
    }

    /// <summary>
    /// 明細修改按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateDetail_Command(object sender, CommandEventArgs e)
    {        
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=window.showModalDialog('Depository015Detail.aspx?ActionType=Mod&Index= " + e.CommandArgument.ToString() + " ','','dialogHeight:600px;dialogWidth:1000px;');if(aa!=undefined){ImtBind();}", true);
    }

    /// <summary>
    /// 明細刪除按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteDetail_Command(object sender, CommandEventArgs e)
    {
        DataTable dt = (DataTable)Session["Purchase"];
        dt.Rows.RemoveAt(Convert.ToInt32(e.CommandArgument.ToString()));
        Session["Purchase"] = dt;
        RepeaterBind();
    }

    /// <summary>
    /// Repeater列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Repeater1_ItemCreated(object sender, RepeaterItemEventArgs e)
    {        
        Button btnUpdate = null;
        Button btnDelete = null;

        // 修改的邦定事件
        btnUpdate = (Button)e.Item.FindControl("btnUpdateDetail");
        btnUpdate.CommandArgument = e.Item.ItemIndex.ToString();
        // 刪除的邦定事件
        btnDelete = (Button)e.Item.FindControl("btnDeleteDetail");
        btnDelete.CommandArgument = e.Item.ItemIndex.ToString();
        btnDelete.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
    }
   

}
