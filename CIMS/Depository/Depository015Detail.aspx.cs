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

public partial class Depository_Depository015Detail : PageBase
{
    Depository015BL bl = new Depository015BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!IsPostBack)
        {
            //初始化UI資料
            DataSet ds = new DataSet();//品名
            DataSet dsFactory = new DataSet();//Perso廠名
            DataSet dsFactory1 = new DataSet();//Perso廠名
            DataSet dsFactory2 = new DataSet();//Perso廠名
            DataSet dsFactory3 = new DataSet();//Perso廠名
            DataSet dsFactory4 = new DataSet();//Perso廠名
            
            try
            {
                ds = bl.GetAllMaterielList();
                dropName.DataSource = ds;
                dropName.DataValueField = "Serial_Number";
                dropName.DataTextField = "Name";
                dropName.DataBind();                

                dsFactory = bl.GetCooperatePersoList();                                
                dropFactoryRID1.DataSource = dsFactory;
                dropFactoryRID1.DataValueField = "RID";
                dropFactoryRID1.DataTextField = "Factory_ShortName_CN";
                dropFactoryRID1.DataBind();

                dsFactory1 = bl.GetCooperatePersoList();
                dropFactoryRID2.DataSource = dsFactory1;
                dropFactoryRID2.DataValueField = "RID";
                dropFactoryRID2.DataTextField = "Factory_ShortName_CN";
                dropFactoryRID2.DataBind();
                dsFactory2 = bl.GetCooperatePersoList();
                dropFactoryRID3.DataSource = dsFactory2;
                dropFactoryRID3.DataValueField = "RID";
                dropFactoryRID3.DataTextField = "Factory_ShortName_CN";
                dropFactoryRID3.DataBind();
                dsFactory3 = bl.GetCooperatePersoList();
                dropFactoryRID4.DataSource = dsFactory3;
                dropFactoryRID4.DataValueField = "RID";
                dropFactoryRID4.DataTextField = "Factory_ShortName_CN";
                dropFactoryRID4.DataBind();
                dsFactory4 = bl.GetCooperatePersoList();
                dropFactoryRID5.DataSource = dsFactory4;
                dropFactoryRID5.DataValueField = "RID";
                dropFactoryRID5.DataTextField = "Factory_ShortName_CN";
                dropFactoryRID5.DataBind();

                ListItem li = new ListItem("", "");
                dropName.Items.Insert(0,li);
                ListItem li1 = new ListItem("", "");
                dropFactoryRID1.Items.Insert(0, li1);
                ListItem li2= new ListItem("", "");
                dropFactoryRID2.Items.Insert(0, li2);
                ListItem li3 = new ListItem("", "");
                dropFactoryRID3.Items.Insert(0, li3);
                ListItem li4 = new ListItem("", "");
                dropFactoryRID4.Items.Insert(0, li4);
                ListItem li5 = new ListItem("", "");
                dropFactoryRID5.Items.Insert(0, li5);
                
                
                hidActionType.Value = Request.QueryString["ActionType"].ToString();
                HidIndex.Value = Request.QueryString["Index"].ToString();
                

                //RID不為空，修改，設置當前介面訊息
                if (-1 != Convert.ToInt32(HidIndex.Value.ToString()))
                {

                    // 初始化特定UI資料
                    DataTable dtbl = (DataTable)Session["Purchase"];
                    //獲取當前的資料行
                    DataRow dr = dtbl.Rows[int.Parse(HidIndex.Value.ToString())];

                    if (dr["Name"].ToString() != "")
                        dropName.Items.FindByValue(dr["Serial_Number"].ToString()).Selected = true;
                    else
                    {
                        dropName.ClearSelection();
                        dropName.Items.FindByValue("").Selected = true;
                    }
                    if (dr["Factory_RID1"].ToString() != "")
                        dropFactoryRID1.Items.FindByValue(dr["Factory_RID1"].ToString()).Selected = true;
                    else
                    {
                        dropFactoryRID1.ClearSelection();
                        dropFactoryRID1.Items.FindByValue("").Selected = true;
                    }
                    if (dr["Factory_RID2"].ToString() != "")
                        dropFactoryRID2.Items.FindByValue(dr["Factory_RID2"].ToString()).Selected = true;
                    else
                    {
                        dropFactoryRID2.ClearSelection();
                        dropFactoryRID2.Items.FindByValue("").Selected = true;
                    }
                    if (dr["Factory_RID3"].ToString() != "")
                        dropFactoryRID3.Items.FindByValue(dr["Factory_RID3"].ToString()).Selected = true;
                    else
                    {
                        dropFactoryRID3.ClearSelection();
                        dropFactoryRID3.Items.FindByValue("").Selected = true;
                    }
                    if (dr["Factory_RID4"].ToString() != "")
                        dropFactoryRID4.Items.FindByValue(dr["Factory_RID4"].ToString()).Selected = true;
                    else
                    {
                        dropFactoryRID4.ClearSelection();
                        dropFactoryRID4.Items.FindByValue("").Selected = true;
                    }
                    if (dr["Factory_RID5"].ToString() != "")
                        dropFactoryRID5.Items.FindByValue(dr["Factory_RID5"].ToString()).Selected = true;
                    else
                    {
                        dropFactoryRID5.ClearSelection();
                        dropFactoryRID5.Items.FindByValue("").Selected = true;
                    }

                    if (!StringUtil.IsEmpty(dr["Number1"].ToString()))
                        txtNumber1.Text = Convert.ToInt32(dr["Number1"].ToString()).ToString("N0");
                    if (!StringUtil.IsEmpty(dr["Number2"].ToString()))
                        txtNumber2.Text = Convert.ToInt32(dr["Number2"].ToString()).ToString("N0");
                    if (!StringUtil.IsEmpty(dr["Number3"].ToString()))
                        txtNumber3.Text = Convert.ToInt32(dr["Number3"].ToString()).ToString("N0");
                    if (!StringUtil.IsEmpty(dr["Number4"].ToString()))
                        txtNumber4.Text = Convert.ToInt32(dr["Number4"].ToString()).ToString("N0");
                    if (!StringUtil.IsEmpty(dr["Number5"].ToString()))
                        txtNumber5.Text = Convert.ToInt32(dr["Number5"].ToString()).ToString("N0");

                    txtDelivery_Date1.Text = dr["Delivery_Date1"].ToString();
                    txtDelivery_Date2.Text = dr["Delivery_Date2"].ToString();
                    txtDelivery_Date3.Text = dr["Delivery_Date3"].ToString();
                    txtDelivery_Date4.Text = dr["Delivery_Date4"].ToString();
                    txtDelivery_Date5.Text = dr["Delivery_Date5"].ToString();
                    txtCase_Date.Text = dr["Case_Date"].ToString();
                    txtComment.Text = dr["Comment"].ToString();
                    if (!StringUtil.IsEmpty(dr["Unit_Price"].ToString()))
                        lblUnitPrice.Text = Convert.ToDecimal(dr["Unit_Price"].ToString()).ToString("N2");
                    if (!StringUtil.IsEmpty(dr["Total_Num"].ToString()))
                        lblTotalNumber.Text = Convert.ToInt64(dr["Total_Num"].ToString()).ToString("N0");
                    if (!StringUtil.IsEmpty(dr["Total_Price"].ToString()))
                        lblTotalPrice.Text = Convert.ToDecimal(dr["Total_Price"].ToString()).ToString("N2");

                    if (dr["SAP_Serial_Number"].ToString().Trim() != "" || dr["SAP_Serial_Number"] == null)
                        dropName.Enabled = false;
                }
                else
                {
                    //新增狀態的頁面初始化
                    dropName.SelectedValue = "";
                    dropFactoryRID1.SelectedValue = "";
                    dropFactoryRID2.SelectedValue = "";
                    dropFactoryRID3.SelectedValue = "";
                    dropFactoryRID4.SelectedValue = "";
                    dropFactoryRID5.SelectedValue = "";

                    lblUnitPrice.Text = "0.00";
                    lblTotalNumber.Text = "0";
                    lblTotalPrice.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    /// <summary>
    /// 品名變化時改變相應單價
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropName_SelectedIndexChanged(object sender, EventArgs e)
    {
        int number = 0;

        if (dropName.SelectedValue == "")
        {
            lblUnitPrice.Text = "0.00";
            lblTotalPrice.Text = "0.00";
            return;
        }
        DataSet ds = bl.GetMaterielInfo(dropName.SelectedValue);
        if (ds.Tables[0].Rows.Count > 0)
            lblUnitPrice.Text = Convert.ToDecimal(ds.Tables[0].Rows[0]["Unit_Price"].ToString()).ToString("N2");

        
        if (txtNumber1.Text.Trim() != "")
        {
            number += Convert.ToInt32(txtNumber1.Text.Replace(",", ""));
        }
        if (txtNumber2.Text.Trim() != "")
        {
            number += Convert.ToInt32(txtNumber2.Text.Replace(",", ""));
        }
        if (txtNumber3.Text.Trim() != "")
        {
            number += Convert.ToInt32(txtNumber3.Text.Replace(",", ""));
        }
        if (txtNumber4.Text.Trim() != "")
        {
            number += Convert.ToInt32(txtNumber4.Text.Replace(",", ""));
        }
        if (txtNumber5.Text.Trim() != "")
        {
            number += Convert.ToInt32(txtNumber5.Text.Replace(",", ""));
        }

        if (lblUnitPrice.Text.Trim() != "")
        {
            decimal price = (Convert.ToDecimal(lblUnitPrice.Text.Replace(",","")) * number);
            lblTotalPrice.Text = price.ToString("N2");
        }
        else
        {
            lblUnitPrice.Text = "0.00";
            lblTotalPrice.Text = "0.00";
        }
    }

    /// <summary>
    /// 確定按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        // 介面資料檢查
        if (dropName.SelectedItem.Text.Trim() == "")
        {
            ShowMessage("品名必須輸入");
            return;
        }
        if (dropFactoryRID1.SelectedValue != "")
        {
            //if (dropFactoryRID1.SelectedValue == dropFactoryRID2.SelectedValue || dropFactoryRID1.SelectedValue == dropFactoryRID3.SelectedValue || dropFactoryRID1.SelectedValue == dropFactoryRID4.SelectedValue || dropFactoryRID1.SelectedValue == dropFactoryRID5.SelectedValue)
            //{
            //    ShowMessage("單一採購明細內的送貨Perso廠不可重覆");
            //    return;
            //}
            if (txtNumber1.Text == "")
            {
                ShowMessage("選擇Perso廠後送貨數量就必需輸入");
                return;
            }
            if (Convert.ToInt32(txtNumber1.Text.Replace(",", "")) <= 0)
            {
                ShowMessage("送貨數量必需為正數");
                return;
            }
            if (txtDelivery_Date1.Text == "")
            {
                ShowMessage("選擇Perso廠後交貨日就必需輸入");
                return;
            }
            if (Convert.ToDateTime(txtDelivery_Date1.Text) < Convert.ToDateTime(Session["Purchase_Date"].ToString()))
            {
                ShowMessage("交貨日不可小於採購日");
                return;
            }

            if (txtCase_Date.Text != "")
            {
                if (Convert.ToDateTime(txtCase_Date.Text) < Convert.ToDateTime(txtDelivery_Date1.Text))
                {
                    ShowMessage("結案日不可小於交貨日");
                    return;
                }
            }
        }
        else
        {
            if (txtNumber1.Text != "")
            {
                ShowMessage("送貨數量已輸入，請選擇送貨Perso廠");
                return;
            }
        }

        if (dropFactoryRID2.SelectedValue != "")
        {
            //if (dropFactoryRID1.SelectedValue == dropFactoryRID2.SelectedValue || dropFactoryRID2.SelectedValue == dropFactoryRID3.SelectedValue || dropFactoryRID2.SelectedValue == dropFactoryRID4.SelectedValue || dropFactoryRID2.SelectedValue == dropFactoryRID5.SelectedValue)
            //{
            //    ShowMessage("單一採購明細內的送貨Perso廠不可重覆");
            //    return;
            //}
            if (txtNumber2.Text == "")
            {
                ShowMessage("選擇Perso廠後送貨數量就必需輸入");
                return;
            }
            if (Convert.ToInt32(txtNumber2.Text.Replace(",", "")) <= 0)
            {
                ShowMessage("送貨數量必需為正數");
                return;
            }
            if (txtDelivery_Date2.Text == "")
            {
                ShowMessage("選擇Perso廠後交貨日就必需輸入");
                return;
            }
            if (Convert.ToDateTime(txtDelivery_Date2.Text) < Convert.ToDateTime(Session["Purchase_Date"].ToString()))
            {
                ShowMessage("交貨日不可小於採購日");
                return;
            }

            if (txtCase_Date.Text != "")
            {
                if (Convert.ToDateTime(txtCase_Date.Text) < Convert.ToDateTime(txtDelivery_Date2.Text))
                {
                    ShowMessage("結案日不可小於交貨日");
                    return;
                }
            }
        }
        else
        {
            if (txtNumber2.Text != "")
            {
                ShowMessage("送貨數量已輸入，請選擇送貨Perso廠");
                return;
            }
        }

        if (dropFactoryRID3.SelectedValue != "")
        {
            //if (dropFactoryRID3.SelectedValue == dropFactoryRID2.SelectedValue || dropFactoryRID1.SelectedValue == dropFactoryRID3.SelectedValue || dropFactoryRID3.SelectedValue == dropFactoryRID4.SelectedValue || dropFactoryRID3.SelectedValue == dropFactoryRID5.SelectedValue)
            //{
            //    ShowMessage("單一採購明細內的送貨Perso廠不可重覆");
            //    return;
            //}
            if (txtNumber3.Text == "")
            {
                ShowMessage("選擇Perso廠後送貨數量就必需輸入");
                return;
            }
            if (Convert.ToInt32(txtNumber3.Text.Replace(",", "")) <= 0)
            {
                ShowMessage("送貨數量必需為正數");
                return;
            }
            if (txtDelivery_Date3.Text == "")
            {
                ShowMessage("選擇Perso廠後交貨日就必需輸入");
                return;
            }
            if (Convert.ToDateTime(txtDelivery_Date3.Text) < Convert.ToDateTime(Session["Purchase_Date"].ToString()))
            {
                ShowMessage("交貨日不可小於採購日");
                return;
            }

            if (txtCase_Date.Text != "")
            {
                if (Convert.ToDateTime(txtCase_Date.Text) < Convert.ToDateTime(txtDelivery_Date3.Text))
                {
                    ShowMessage("結案日不可小於交貨日");
                    return;
                }
            }
        }
        else
        {
            if (txtNumber3.Text != "")
            {
                ShowMessage("送貨數量已輸入，請選擇送貨Perso廠");
                return;
            }
        }

        if (dropFactoryRID4.SelectedValue != "")
        {
            //if (dropFactoryRID4.SelectedValue == dropFactoryRID2.SelectedValue || dropFactoryRID4.SelectedValue == dropFactoryRID3.SelectedValue || dropFactoryRID1.SelectedValue == dropFactoryRID4.SelectedValue || dropFactoryRID4.SelectedValue == dropFactoryRID5.SelectedValue)
            //{
            //    ShowMessage("單一採購明細內的送貨Perso廠不可重覆");
            //    return;
            //}
            if (txtNumber4.Text == "")
            {
                ShowMessage("選擇Perso廠後送貨數量就必需輸入");
                return;
            }
            if (Convert.ToInt32(txtNumber4.Text.Replace(",", "")) <= 0)
            {
                ShowMessage("送貨數量必需為正數");
                return;
            }
            if (txtDelivery_Date4.Text == "")
            {
                ShowMessage("選擇Perso廠後交貨日就必需輸入");
                return;
            }
            if (Convert.ToDateTime(txtDelivery_Date4.Text) < Convert.ToDateTime(Session["Purchase_Date"].ToString()))
            {
                ShowMessage("交貨日不可小於採購日");
                return;
            }

            if (txtCase_Date.Text != "")
            {
                if (Convert.ToDateTime(txtCase_Date.Text) < Convert.ToDateTime(txtDelivery_Date4.Text))
                {
                    ShowMessage("結案日不可小於交貨日");
                    return;
                }
            }
        }
        else
        {
            if (txtNumber4.Text != "")
            {
                ShowMessage("送貨數量已輸入，請選擇送貨Perso廠");
                return;
            }
        }

        if (dropFactoryRID5.SelectedValue != "")
        {
            //if (dropFactoryRID5.SelectedValue == dropFactoryRID2.SelectedValue || dropFactoryRID5.SelectedValue == dropFactoryRID3.SelectedValue || dropFactoryRID5.SelectedValue == dropFactoryRID4.SelectedValue || dropFactoryRID1.SelectedValue == dropFactoryRID5.SelectedValue)
            //{
            //    ShowMessage("單一採購明細內的送貨Perso廠不可重覆");
            //    return;
            //}
            if (txtNumber5.Text == "")
            {
                ShowMessage("選擇Perso廠後送貨數量就必需輸入");
                return;
            }
            if (Convert.ToInt32(txtNumber5.Text.Replace(",", "")) <= 0)
            {
                ShowMessage("送貨數量必需為正數");
                return;
            }
            if (txtDelivery_Date5.Text == "")
            {
                ShowMessage("選擇Perso廠後交貨日就必需輸入");
                return;
            }
            if (Convert.ToDateTime(txtDelivery_Date5.Text) < Convert.ToDateTime(Session["Purchase_Date"].ToString()))
            {
                ShowMessage("交貨日不可小於採購日");
                return;
            }

            if (txtCase_Date.Text != "")
            {
                if (Convert.ToDateTime(txtCase_Date.Text) < Convert.ToDateTime(txtDelivery_Date5.Text))
                {
                    ShowMessage("結案日不可小於交貨日");
                    return;
                }
            }
        }
        else
        {
            if (txtNumber5.Text != "")
            {
                ShowMessage("送貨數量已輸入，請選擇送貨Perso廠");
                return;
            }
        }

        if (StringUtil.GetByteLength(txtComment.Text) > 100)
        {
            ShowMessage("備註不能超過100個字符");
            return;
        }

        
        
        //將UI中的資料存入session中，傳回相應界面
        string strRID = HidIndex.Value.ToString();
        string strActionType = hidActionType.Value.ToString();

        DataTable dtPurchase = (DataTable)this.Session["Purchase"];
        DataRow drow = null;
        
        //-1代表新增
        if (-1 == int.Parse(strRID))
        {
            drow = dtPurchase.NewRow();
        }
        else
        {
            drow = dtPurchase.Rows[int.Parse(strRID)];

            //若採購單中有已請款的採購明細，則此採購明細只能變更送貨Perso廠、送貨數量、預定交貨日、結案日期、備註，但加總的採購數量必需與修改前相同
            if (drow["SAP_Serial_Number"] != null && drow["SAP_Serial_Number"].ToString().Trim() != "")
            {
                int total = 0;
                if (txtNumber1.Text != "")
                    total += Convert.ToInt32(txtNumber1.Text.Replace(",", ""));
                if (txtNumber2.Text != "")
                    total += Convert.ToInt32(txtNumber2.Text.Replace(",", ""));
                if (txtNumber3.Text != "")
                    total += Convert.ToInt32(txtNumber3.Text.Replace(",", ""));
                if (txtNumber4.Text != "")
                    total += Convert.ToInt32(txtNumber4.Text.Replace(",", ""));
                if (txtNumber5.Text != "")
                    total += Convert.ToInt32(txtNumber5.Text.Replace(",", ""));
                if (total != Convert.ToInt32(drow["Total_Num"].ToString()))
                {
                    ShowMessage("該筆採購明細已請款，其採購數量不可變更");
                    return;
                }
            }
        }

        drow["Name"] = dropName.SelectedItem.Text;
        drow["Serial_Number"] = dropName.SelectedValue;

    
        drow["Factory_RID1"] = dropFactoryRID1.SelectedValue;
        drow["Factory1"] = dropFactoryRID1.SelectedItem.Text;
        drow["Number1"] = txtNumber1.Text.Replace(",","");
        drow["Delivery_Date1"] = txtDelivery_Date1.Text;
    
        drow["Factory_RID2"] = dropFactoryRID2.SelectedValue;
        drow["Factory2"] = dropFactoryRID2.SelectedItem.Text;
        drow["Number2"] = txtNumber2.Text.Replace(",", "");
        drow["Delivery_Date2"] = txtDelivery_Date2.Text;
    
        drow["Factory_RID3"] = dropFactoryRID3.SelectedValue;
        drow["Factory3"] = dropFactoryRID3.SelectedItem.Text;
        drow["Number3"] = txtNumber3.Text.Replace(",", "");
        drow["Delivery_Date3"] = txtDelivery_Date3.Text;       
  
        drow["Factory_RID4"] = dropFactoryRID4.SelectedValue;
        drow["Factory4"] = dropFactoryRID4.SelectedItem.Text;
        drow["Number4"] = txtNumber4.Text.Replace(",", "");
        drow["Delivery_Date4"] = txtDelivery_Date4.Text;     
    
        drow["Factory_RID5"] = dropFactoryRID5.SelectedValue;
        drow["Factory5"] = dropFactoryRID5.SelectedItem.Text;
        drow["Number5"] = txtNumber5.Text.Replace(",", "");
        drow["Delivery_Date5"] = txtDelivery_Date5.Text;

        if (txtCase_Date.Text != "")
            drow["Case_Date"] = txtCase_Date.Text;
        else
            drow["Case_Date"] = "";
        drow["Comment"] = txtComment.Text;
        drow["Unit_Price"] = lblUnitPrice.Text.Replace(",","");
        long totalnum = 0;
        if (txtNumber1.Text != "")
            totalnum += Convert.ToInt32(txtNumber1.Text.Replace(",", ""));
        if (txtNumber2.Text != "")
            totalnum += Convert.ToInt32(txtNumber2.Text.Replace(",", ""));
        if (txtNumber3.Text != "")
            totalnum += Convert.ToInt32(txtNumber3.Text.Replace(",", ""));
        if (txtNumber4.Text != "")
            totalnum += Convert.ToInt32(txtNumber4.Text.Replace(",", ""));
        if (txtNumber5.Text != "")
            totalnum += Convert.ToInt32(txtNumber5.Text.Replace(",", ""));
        drow["Total_Num"] = totalnum;
        drow["Total_Price"] = Convert.ToInt64(drow["Total_Num"].ToString()) * Convert.ToDecimal(lblUnitPrice.Text.Replace(",",""));
        

        // 新加
        if (-1 == int.Parse(strRID))
        {
            drow["SAP_Serial_Number"] = "";
            //drow["Detail_RID"] = "";
            dtPurchase.Rows.Add(drow);
        }

        Session["Purchase"] = dtPurchase;
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "returnValue='1';window.close();", true);

    }

    /// <summary>
    /// 每次輸入送貨數量後，自動計算採購數量
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtNumber1_TextChanged(object sender, EventArgs e)
    {
        long number=0;        
        if (txtNumber1.Text.Trim() != "")
        {
            try
            {
                number += Convert.ToInt32(txtNumber1.Text.Replace(",", ""));
            }
            catch { txtNumber1.Text = "0"; }
        }
        if (txtNumber2.Text.Trim() != "")
        {
            try
            {
                number += Convert.ToInt32(txtNumber2.Text.Replace(",", ""));
            }
            catch { txtNumber2.Text = "0"; }
        }
        if (txtNumber3.Text.Trim() != "")
        {
            try
            {
                number += Convert.ToInt32(txtNumber3.Text.Replace(",", ""));
            }
            catch { txtNumber3.Text = "0"; }
        }
        if (txtNumber4.Text.Trim() != "")
        {
            try
            {
                number += Convert.ToInt32(txtNumber4.Text.Replace(",", ""));
            }
            catch { txtNumber4.Text = "0"; }
        }
        if (txtNumber5.Text.Trim() != "")
        {
            try
            {
                number += Convert.ToInt32(txtNumber5.Text.Replace(",", ""));
            }
            catch { txtNumber5.Text = "0"; }
        }
        lblTotalNumber.Text = number.ToString("N0");
        if (lblUnitPrice.Text.Trim() != "")
        {
            decimal price=(Convert.ToDecimal(lblUnitPrice.Text.Replace(",","")) * number);
            lblTotalPrice.Text = price.ToString("N2");
        }
        else
        {
            lblUnitPrice.Text = "0.00";
            lblTotalPrice.Text = "0.00";
        }
    }
}
