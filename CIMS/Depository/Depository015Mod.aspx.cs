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

public partial class Depository_Depository015Mod : PageBase
{
    Depository015BL bl = new Depository015BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string PurchaseOrder_RID = Request.QueryString["PurchaseOrder_RID"];
            Session["PurchaseOrder_RID"] = PurchaseOrder_RID;

            DataTable dtbl = new DataTable();
            dtbl.Columns.Add("Name", Type.GetType("System.String"));
            dtbl.Columns.Add("Detail_RID", Type.GetType("System.Int32"));
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
            dtbl.Columns.Add("Unit_Price", Type.GetType("System.Decimal"));
            dtbl.Columns.Add("Total_Num", Type.GetType("System.Int32"));
            dtbl.Columns.Add("Total_Price", Type.GetType("System.Decimal"));
            dtbl.Columns.Add("SAP_Serial_Number", Type.GetType("System.String"));
            dtbl.Columns.Add("Ask_Date", Type.GetType("System.String"));
            dtbl.Columns.Add("Pay_Date", Type.GetType("System.String"));

            Session["Purchase"] = dtbl;//存儲UI中的信息  
                        

            if (StringUtil.IsEmpty(PurchaseOrder_RID))
            {
                return;
            }

            try
            {
                DateTime dt = new DateTime();
                DataSet dstPurchaseDetailList = null;

                //根據傳入的【採購單號】查詢採購訂單記錄
                dstPurchaseDetailList = bl.PurchaseDetailList(PurchaseOrder_RID);

                //若該採購單已有請款的明細，則將刪除的Checkbox設為Disabled
                if (bl.CheckPurchaseAsk(PurchaseOrder_RID))
                    chkDelete.Enabled = false;

                //初始化頁面資料
                if (dstPurchaseDetailList.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow drw in dstPurchaseDetailList.Tables[0].Rows)
                    {
                        DataRow dr = dtbl.NewRow();

                        //初始化日期
                        dt = (DateTime)drw["Purchase_Date"];
                        lblPurchaseDate.Text = dt.ToString("yyyy/MM/dd");
                        Session["Purchase_Date"] = lblPurchaseDate.Text;

                        if (drw["Delivery_Date1"].ToString() != "")
                        {
                            dt = (DateTime)drw["Delivery_Date1"];
                            dr["Delivery_Date1"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Delivery_Date1"].ToString() == "1900/01/01")
                                dr["Delivery_Date1"] = "";
                        }
                        if (drw["Delivery_Date2"].ToString() != "")
                        {
                            dt = (DateTime)drw["Delivery_Date2"];
                            dr["Delivery_Date2"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Delivery_Date2"].ToString() == "1900/01/01")
                                dr["Delivery_Date2"] = "";
                        }
                        if (drw["Delivery_Date3"].ToString() != "")
                        {
                            dt = (DateTime)drw["Delivery_Date3"];
                            dr["Delivery_Date3"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Delivery_Date3"].ToString() == "1900/01/01")
                                dr["Delivery_Date3"] = "";
                        }
                        if (drw["Delivery_Date4"].ToString() != "")
                        {
                            dt = (DateTime)drw["Delivery_Date4"];
                            dr["Delivery_Date4"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Delivery_Date4"].ToString() == "1900/01/01")
                                dr["Delivery_Date4"] = "";
                        }
                        if (drw["Delivery_Date5"].ToString() != "")
                        {
                            dt = (DateTime)drw["Delivery_Date5"];
                            dr["Delivery_Date5"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Delivery_Date5"].ToString() == "1900/01/01")
                                dr["Delivery_Date5"] = "";
                        }
                        if (drw["Case_Date"].ToString() != "")
                        {
                            dt = (DateTime)drw["Case_Date"];
                            dr["Case_Date"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Case_Date"].ToString() == "1900/01/01")
                                dr["Case_Date"] = "";
                        }
                        if (drw["Ask_Date"].ToString() != "")
                        {
                            dt = (DateTime)drw["Ask_Date"];
                            dr["Ask_Date"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Ask_Date"].ToString() == "1900/01/01")
                                dr["Ask_Date"] = "";
                        }
                        if (drw["Case_Date"].ToString() != "")
                        {
                            dt = (DateTime)drw["Pay_Date"];
                            dr["Pay_Date"] = dt.ToString("yyyy/MM/dd");
                            if (dr["Pay_Date"].ToString() == "1900/01/01")
                                dr["Pay_Date"] = "";
                        }



                        DataSet Name = bl.GetMaterielName(drw["Serial_Number"].ToString());
                        string name = "";
                        if (Name.Tables[0].Rows[0]["AName"].ToString().Trim() != "" && Name.Tables[0].Rows[0]["AName"].ToString() != null)
                        {
                            name = Name.Tables[0].Rows[0]["AName"].ToString();
                        }
                        else if (Name.Tables[0].Rows[0]["BName"].ToString().Trim() != "" && Name.Tables[0].Rows[0]["BName"].ToString() != null)
                        {
                            name = Name.Tables[0].Rows[0]["BName"].ToString();
                        }
                        else if (Name.Tables[0].Rows[0]["CName"].ToString().Trim() != "" && Name.Tables[0].Rows[0]["CName"].ToString() != null)
                        {
                            name = Name.Tables[0].Rows[0]["CName"].ToString();
                        }
                        dr["Name"] = name;
                        dr["Serial_Number"] = drw["Serial_Number"].ToString();

                        name = bl.GetFactoryShortCName(drw["Factory_RID1"].ToString());
                        dr["Factory1"] = name;
                        dr["Factory_RID1"] = drw["Factory_RID1"].ToString();
                        if (dr["Factory_RID1"].ToString() == "0")
                            dr["Factory_RID1"] = "";
                        dr["Number1"] = drw["Number1"].ToString();
                        if (dr["Number1"].ToString() == "0")
                            dr["Number1"] = "";

                        name = bl.GetFactoryShortCName(drw["Factory_RID2"].ToString());
                        dr["Factory2"] = name;
                        dr["Factory_RID2"] = drw["Factory_RID2"].ToString();
                        if (dr["Factory_RID2"].ToString() == "0")
                            dr["Factory_RID2"] = "";
                        dr["Number2"] = drw["Number2"].ToString();
                        if (dr["Number2"].ToString() == "0")
                            dr["Number2"] = "";

                        name = bl.GetFactoryShortCName(drw["Factory_RID3"].ToString());
                        dr["Factory3"] = name;
                        dr["Factory_RID3"] = drw["Factory_RID3"].ToString();
                        if (dr["Factory_RID3"].ToString() == "0")
                            dr["Factory_RID3"] = "";
                        dr["Number3"] = drw["Number3"].ToString();
                        if (dr["Number3"].ToString() == "0")
                            dr["Number3"] = "";

                        name = bl.GetFactoryShortCName(drw["Factory_RID4"].ToString());
                        dr["Factory4"] = name;
                        dr["Factory_RID4"] = drw["Factory_RID4"].ToString();
                        if (dr["Factory_RID4"].ToString() == "0")
                            dr["Factory_RID4"] = "";
                        dr["Number4"] = drw["Number4"].ToString();
                        if (dr["Number4"].ToString() == "0")
                            dr["Number4"] = "";

                        name = bl.GetFactoryShortCName(drw["Factory_RID5"].ToString());
                        dr["Factory5"] = name;
                        dr["Factory_RID5"] = drw["Factory_RID5"].ToString();
                        if (dr["Factory_RID5"].ToString() == "0")
                            dr["Factory_RID5"] = "";
                        dr["Number5"] = drw["Number5"].ToString();
                        if (dr["Number5"].ToString() == "0")
                            dr["Number5"] = "";

                        dr["Comment"] = drw["Comment"].ToString();
                        dr["Unit_Price"] = drw["Unit_Price"].ToString();
                        dr["Total_Num"] = drw["Total_Num"].ToString();
                        dr["Total_Price"] = drw["Total_Price"].ToString();
                        dr["SAP_Serial_Number"] = drw["SAP_Serial_Number"].ToString();                        

                        dtbl.Rows.Add(dr);
                    }
                }

                lblPurchaseOrder_RID.Text = PurchaseOrder_RID;//初始化採購單號
                hidPurchaseOrder_RID.Value = PurchaseOrder_RID;
                Session["Purchase"] = dtbl;
                

                Repeater1.DataSource = (DataTable)Session["Purchase"];
                Repeater1.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
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
            // 介面資料檢查
            DataSet PurchaseByYear = bl.GetPurchaseByYear(Session["Purchase_Date"].ToString().Substring(0, 4), Session["PurchaseOrder_RID"].ToString());
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

            DataSet MaterielBudget = bl.GetMaterielBudget(Session["Purchase_Date"].ToString().Substring(0,4));

            
            if (MaterielBudget.Tables[0].Rows.Count == 0)
            {
                ShowMessage("尚未有年度預算，無法儲存");
                return;
            }

            //發送警訊“XX/XX  物料修改採購作業，請至系統內作請款作業”
            foreach (DataRow dr in MaterielBudget.Tables[0].Rows)
            {
                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.EXPONENT_CARD)
                {
                    if (exponentKa > Convert.ToDecimal(dr["Budget"].ToString()))
                    {
                        ShowMessage("寄卡單（卡）採購金額超過該年度預算，無法儲存");
                        return;
                    }
                    continue;
                }
                if (dr["Materiel_Type"].ToString().Trim() == GlobalString.MaterielType.EXPONENT_BANK)
                {
                    if (exponentYin > Convert.ToDecimal(dr["Budget"].ToString()))
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
        
            if (chkDelete.Checked == true)//是否選擇了刪除
            {
                string PurchaseOrder_RID = Session["PurchaseOrder_RID"].ToString();

                bl.delete(PurchaseOrder_RID);
                //操作日誌
                bl.SetOprLog("4");

                Session.Remove("PurchaseOrder_RID");
                Session.Remove("Purchase_Date");
                Session.Remove("Purchase");
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Depository015.aspx?Con=1");
            }
            else
            {     
                string PurchaseOrder_RID = Session["PurchaseOrder_RID"].ToString();
                string PurchaseDate = Session["Purchase_Date"].ToString();

                // 將UI訊息保存到資料庫中（先刪除資料庫中的信息再把UI訊息加入資料庫）
                //刪除
                bl.delete(PurchaseOrder_RID);

                //發送警訊“XX/XX  已超過年度預算安全水位”
                foreach (DataRow dr in MaterielBudget.Tables[0].Rows)
                {
                    //if (dr["Budget"].ToString() == "0")
                     //   continue;

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

                // 添加所有轉移單訊息
                bl.Add(dtbl, PurchaseDate, PurchaseOrder_RID);
                //操作日誌
                bl.SetOprLog("3");
                //發送警訊“XX/XX  物料新增採購作業，請至系統內作請款作業”
                string[] argF = new string[2];
                argF[0] = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString();
                argF[1] = "修改";
                Warning.SetWarning(GlobalString.WarningType.PlsAskFinance, argF);

                Session.Remove("PurchaseOrder_RID");
                Session.Remove("Purchase_Date");
                Session.Remove("Purchase");
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Depository015.aspx?Con=1");
                
            }

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
        btnDelete = (Button)e.Item.FindControl("btnDeleteDetail");
        btnDelete.CommandArgument = e.Item.ItemIndex.ToString();
        btnDelete.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
    }

    /// <summary>
    /// 明細修改按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpdateDetail_Command(object sender, CommandEventArgs e)
    {

        string rid = Session["PurchaseOrder_RID"].ToString();
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
        Repeater1.DataSource = (DataTable)Session["Purchase"];
        Repeater1.DataBind();
    }

    /// <summary>
    /// Repeater列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Repeater1_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        DataTable dt = (DataTable)Session["Purchase"];
        Button btnDelete = null;
        Button btnUpdate = null;
        // 修改的邦定事件
        btnUpdate = (Button)e.Item.FindControl("btnUpdateDetail");
        btnUpdate.CommandArgument = e.Item.ItemIndex.ToString();
        // 刪除的邦定事件
        btnDelete = (Button)e.Item.FindControl("btnDeleteDetail");
        btnDelete.CommandArgument = e.Item.ItemIndex.ToString();
        btnDelete.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

        //若該採購明細已請款（SAP單號SAP_Serial_Number不為空），則將其對應的刪除Button設為Disabled。
        if (dt!=null && dt.Rows[e.Item.ItemIndex]["SAP_Serial_Number"].ToString().Trim() != "")
            btnDelete.Enabled = false;

    }

    /// <summary>
    /// 綁定Repeater控件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        Repeater1.DataSource = (DataTable)Session["Purchase"];
        Repeater1.DataBind();
    }
    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Label lblUnitPrice = (Label)e.Item.FindControl("lblUnitPrice");
            Label lblPurchaseNumber = (Label)e.Item.FindControl("lblPurchaseNumber");
            Label lblPrice = (Label)e.Item.FindControl("lblPrice");
            Label lblNumber1 = (Label)e.Item.FindControl("lblNumber1");
            Label lblNumber2 = (Label)e.Item.FindControl("lblNumber2");
            Label lblNumber3 = (Label)e.Item.FindControl("lblNumber3");
            Label lblNumber4 = (Label)e.Item.FindControl("lblNumber4");
            Label lblNumber5 = (Label)e.Item.FindControl("lblNumber5");

            if (lblUnitPrice.Text != "")
                lblUnitPrice.Text = Convert.ToDecimal(lblUnitPrice.Text).ToString("N2");

            if (lblPurchaseNumber.Text != "")
                lblPurchaseNumber.Text = Convert.ToInt32(lblPurchaseNumber.Text).ToString("N0");

            if (lblPrice.Text != "")
                lblPrice.Text = Convert.ToDecimal(lblPrice.Text).ToString("N2");

            if (lblNumber1.Text != "")
                lblNumber1.Text = Convert.ToInt32(lblNumber1.Text).ToString("N0");

            if (lblNumber2.Text != "")
                lblNumber2.Text = Convert.ToInt32(lblNumber2.Text).ToString("N0");

            if (lblNumber3.Text != "")
                lblNumber3.Text = Convert.ToInt32(lblNumber3.Text).ToString("N0");

            if (lblNumber4.Text != "")
                lblNumber4.Text = Convert.ToInt32(lblNumber4.Text).ToString("N0");

            if (lblNumber5.Text != "")
                lblNumber5.Text = Convert.ToInt32(lblNumber5.Text).ToString("N0");

            

        }
    }
}
