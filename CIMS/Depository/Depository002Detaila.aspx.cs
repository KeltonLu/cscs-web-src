using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Depository_Depository002Detail : PageBase
{
    DataSet ds = null;

    DataTable temp = new DataTable();

    DataTable dtclone = new DataTable();

    CardTypeManager ctmManager = new CardTypeManager();

    Depository002BL depManager = new Depository002BL();

    Dictionary<string, object> inputs = new Dictionary<string, object>();

    // Dictionary<string, object> dirFactory = new Dictionary<string, object>();
    Hashtable htFactory = new Hashtable();


    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";
        temp.Columns.Clear();
        temp.Columns.Add("Pass_Status");
        temp.Columns.Add("orderform_rid");
        temp.Columns.Add("orderform_detail_rid");
        temp.Columns.Add("Space_Short_RID");//cardtype_rid
        temp.Columns.Add("number");
        temp.Columns.Add("Agreement_RID");
        temp.Columns.Add("Agreement_Name");
        temp.Columns.Add("Budget_RID");
        temp.Columns.Add("Budget_Name");
        temp.Columns.Add("factory");
        temp.Columns.Add("factory_id");
        temp.Columns.Add("Base_Price");
        temp.Columns.Add("Fore_Delivery_Date");
        temp.Columns.Add("Wafer_RID");
        temp.Columns.Add("Wafer_Name");
        temp.Columns.Add("Wafer_Capacity");
        temp.Columns.Add("is_exigence");
        temp.Columns.Add("Delivery_Address_RID");
        temp.Columns.Add("factory_shortname_cn");
        temp.Columns.Add("comment");
        temp.Columns.Add("bz");

        //add chaoma start
        temp.Columns.Add("Change_UnitPrice");
        //add chaoma end

        if (!IsPostBack)
        {
            btnEdit.Visible = false;
            btnEditD.Visible = false;

            dropCard_Purpose_RIDBind();
            dropCard_Group_RIDBind();
            dropSpace_Short_RIDBind();
            GetWafer_NAME();
            //標記數據行index
            string strRID = Request.QueryString["RID"];
            if (!StringUtil.IsEmpty(strRID))
            {
                //克隆相同結構DataTable存放更新前的資料
                dtclone = temp.Clone();
                dtclone.Rows.Clear();

                temp = (DataTable)Session["detail"];

                //取當前數據行的資料
                DataRow dr = temp.Rows[int.Parse(strRID)];

                if (dr["bz"].ToString() == "2")
                {
                    trDRID.Visible = false;
                    dropCard_Group_RID.Enabled = false;
                    dropCard_Purpose_RID.Enabled = false;
                    dropSpace_Short_RID.Enabled = false;
                    dropFactory_shorname_cn.Enabled = false;
                }

                Session["drid"] = dr;

                SetControlsForDataRow(dr);

                ShowIsUsingDrop(dropSpace_Short_RID, dr["Space_Short_RID"].ToString(), true);

                ChangeBind();

                if (dr["bz"].ToString() == "1")
                {
                    SetControlsForDataRow(dr);
                }

                if (txtNumber.Text != "")
                {
                    txtNumber.Text = Convert.ToInt32(txtNumber.Text).ToString("N0");
                }

                if (!StringUtil.IsEmpty(dr["Agreement_RID"].ToString()))
                    dropAgreement_RID.SelectedValue = dr["Agreement_RID"].ToString();
                if (!StringUtil.IsEmpty(dr["Budget_RID"].ToString()))
                    dropBudget_RID.SelectedValue = dr["Budget_RID"].ToString();
                if (!StringUtil.IsEmpty(dr["Wafer_RID"].ToString()))
                    dropWafer_Name.SelectedValue = dr["Wafer_RID"].ToString();
                if (!StringUtil.IsEmpty(dr["Delivery_Address_RID"].ToString()))
                    dropFactory_shorname_cn.SelectedValue = dr["Delivery_Address_RID"].ToString();

                if (!StringUtil.IsEmpty(dr["factory_id"].ToString()))
                    txtFactory_shortname_cn.Text = dr["factory_id"].ToString();

                if (!StringUtil.IsEmpty(dr["is_exigence"].ToString()))
                    radlIs_Exigence.SelectedValue = dr["is_exigence"].ToString();

                if (txtFore_Delivery_Date.Text != "")
                {
                    txtFore_Delivery_Date.Text = Convert.ToDateTime(txtFore_Delivery_Date.Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "");
                }

                if (!StringUtil.IsEmpty(dr["factory"].ToString()))
                    HidRID.Value = dr["factory"].ToString();

                //add chaoma start
                if ("" != dr["Change_Unitprice"].ToString().Trim())
                {
                    this.txtChange_Unitprice.Text = Convert.ToDecimal(dr["Change_Unitprice"].ToString()).ToString("N2");
                }
                //add chaoma end
            }
            else
            {
                Session.Remove("drid");

                ChangeBind();
                trDRID.Visible = false;


            }

            if (CanUseAction(GlobalString.UserRoleConfig.Add) && CanUseAction(GlobalString.UserRoleConfig.Edit))
            {
                btnEdit.Visible = true;
                btnEditD.Visible = true;
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {

        if (StringUtil.IsEmpty(dropSpace_Short_RID.SelectedValue))
        {
            ShowMessage("必須選擇卡種");
            return;
        }

        if (StringUtil.IsEmpty(dropBudget_RID.SelectedValue))
        {
            ShowMessage("預算不能為空");
            return;
        }

        if (StringUtil.IsEmpty(dropAgreement_RID.SelectedValue))
        {
            ShowMessage("合約不能為空");
            return;
        }

        if (StringUtil.IsEmpty(txtFactory_shortname_cn.Text.Trim()))
        {
            ShowMessage("空白卡廠不能為空");
            return;
        }
        //add chaoma start
        if (StringUtil.IsEmpty(txtChange_Unitprice.Text.Trim()))
        {
            ShowMessage("調整後單價(含稅)不能為空");
            return;
        }
        //add chaoma end
        if (StringUtil.IsEmpty(txtFore_Delivery_Date.Text.Trim()))
        {
            ShowMessage("預訂交貨日不能為空");
            return;
        }

        if (StringUtil.IsEmpty(dropWafer_Name.SelectedValue))
        {
            ShowMessage("晶片名稱不能為空");
            return;
        }

        if (StringUtil.IsEmpty(txtWafer_Capacity.Text.Trim()))
        {
            ShowMessage("晶片容量不能為空");
            return;
        }

        if (StringUtil.IsEmpty(dropFactory_shorname_cn.SelectedValue))
        {
            ShowMessage("交貨地點不能為空");
            return;
        }

        // edit by Ian 欄位拉長為100個中文字 Huang start
        if (StringUtil.GetByteLength(txtComment.Text) > 200)
        {
            ShowMessage("備註不能超過100個字符");
            return;
        }
        // edit by Ian 欄位拉長為100個中文字 Huang end

        try
        {
            string strRID = Request.QueryString["RID"];
            //取訂購數量
            txtNumber.Text = txtNumber.Text.Replace(",", "");

            if (txtNumber.Text != "" && Convert.ToInt64(txtNumber.Text) > 0)
            {
                if (Session["detail"] != null)
                {

                    temp = (DataTable)Session["detail"];
                    if (Session["htFactory"] != null)
                        htFactory = (Hashtable)Session["htFactory"];
                    //if (!StringUtil.IsEmpty(strRID) && ((DataTable)Session["detail"]).Rows[0]["bz"].ToString() != "2")
                    //如果當前的數據行不為空,或者暫存表是從監控中得來的時候
                    // add by tree 20161027 添加rows.count大於零防呆
                    if (!StringUtil.IsEmpty(strRID) || (temp.Rows.Count > 0 && ((DataTable)Session["detail"]).Rows[0]["bz"].ToString() == "2"))
                    {
                        DataRow dr;
                        //如果傳入的數據行id不為空
                        if (!StringUtil.IsEmpty(strRID))
                        {
                            dr = temp.Rows[int.Parse(strRID)];
                            //如果暫存數據行有訂單明細編號
                            if (dr["orderform_detail_rid"].ToString() != "")
                            {
                                string num = dr["number"].ToString();
                                string arid = dr["Agreement_RID"].ToString();
                                string ofdrid = dr["orderform_detail_rid"].ToString();
                                string ofrid = dr["orderform_rid"].ToString();
                                ArrayList al = new ArrayList();
                                al.Add(num);
                                al.Add(arid);
                                al.Add(ofdrid);
                                al.Add(ofrid);

                                dr["Pass_Status"] = "1";
                                dr["Space_Short_RID"] = this.dropSpace_Short_RID.SelectedValue;
                                dr["number"] = txtNumber.Text;
                                dr["Agreement_RID"] = dropAgreement_RID.SelectedValue;
                                dr["Agreement_Name"] = dropAgreement_RID.SelectedItem.Text;
                                dr["Budget_RID"] = this.dropBudget_RID.SelectedValue;
                                dr["Budget_Name"] = this.dropBudget_RID.SelectedItem.Text;
                                dr["factory"] = HidRID.Value;
                                dr["factory_id"] = txtFactory_shortname_cn.Text;
                                dr["Base_Price"] = this.lblBase_price.Text;
                                dr["Fore_Delivery_Date"] = this.txtFore_Delivery_Date.Text;
                                dr["Wafer_RID"] = this.dropWafer_Name.SelectedValue;
                                dr["Wafer_Name"] = this.dropWafer_Name.SelectedItem.Text;
                                dr["Wafer_Capacity"] = txtWafer_Capacity.Text;
                                if (radlIs_Exigence.SelectedIndex == 0)
                                {
                                    dr["is_exigence"] = "1";
                                }
                                else if (radlIs_Exigence.SelectedIndex == 1)
                                {
                                    dr["is_exigence"] = "2";
                                }
                                dr["Delivery_Address_RID"] = this.dropFactory_shorname_cn.SelectedValue;
                                dr["factory_shortname_cn"] = this.dropFactory_shorname_cn.SelectedItem.Text;

                                dr["comment"] = txtComment.Text;

                                dr["bz"] = "1";

                                //add chaoma start
                                dr["Change_UnitPrice"] = this.txtChange_Unitprice.Text;
                                //add chaoma end 

                                //資料保存後進行了刪除操作，然後再保存（回補、刪除、添加、日誌）
                                depManager.RDAU(al, dr);
                            }
                            //如果暫存數據行訂單流水編號為空
                            else
                            {

                                dr["Pass_Status"] = "1";
                                dr["Space_Short_RID"] = this.dropSpace_Short_RID.SelectedValue;
                                dr["number"] = txtNumber.Text;
                                dr["Agreement_RID"] = dropAgreement_RID.SelectedValue;
                                dr["Agreement_Name"] = dropAgreement_RID.SelectedItem.Text;
                                dr["Budget_RID"] = this.dropBudget_RID.SelectedValue;
                                dr["Budget_Name"] = this.dropBudget_RID.SelectedItem.Text;
                                dr["factory"] = HidRID.Value;
                                dr["factory_id"] = txtFactory_shortname_cn.Text;
                                dr["Base_Price"] = this.lblBase_price.Text;
                                dr["Fore_Delivery_Date"] = this.txtFore_Delivery_Date.Text;
                                dr["Wafer_RID"] = this.dropWafer_Name.SelectedValue;
                                dr["Wafer_Name"] = this.dropWafer_Name.SelectedItem.Text;
                                dr["Wafer_Capacity"] = txtWafer_Capacity.Text;
                                if (radlIs_Exigence.SelectedIndex == 0)
                                {
                                    dr["is_exigence"] = "1";
                                }
                                else if (radlIs_Exigence.SelectedIndex == 1)
                                {
                                    dr["is_exigence"] = "2";
                                }
                                dr["Delivery_Address_RID"] = this.dropFactory_shorname_cn.SelectedValue;

                                dr["factory_shortname_cn"] = this.dropFactory_shorname_cn.SelectedItem.Text;

                                dr["comment"] = txtComment.Text;
                                if (htFactory.Contains(HidRID.Value))
                                {
                                    dr["orderform_rid"] = htFactory[HidRID.Value];
                                }
                                else
                                {
                                    //寫入新的訂單編號
                                    dr["orderform_rid"] = depManager.GetMaxOFID();
                                    htFactory.Add(HidRID.Value, dr["orderform_rid"]);
                                    Session["htFactory"] = htFactory;
                                }
                                //根據訂單編號寫訂單流水編號
                                dr["orderform_detail_rid"] = depManager.GetMaxOFDID(dr["orderform_rid"].ToString());
                                dr["bz"] = "1";
                                //add chaoma start
                                dr["Change_UnitPrice"] = this.txtChange_Unitprice.Text;
                                //add chaoma end 
                                depManager.AUU(dr);
                            }
                        }
                        //如果傳入的數據行id為空
                        else
                        {
                            dr = temp.NewRow();

                            if (htFactory.Contains(HidRID.Value))
                            {
                                dr["orderform_rid"] = htFactory[HidRID.Value];
                            }
                            else
                            {
                                //寫入新的訂單編號
                                dr["orderform_rid"] = depManager.GetMaxOFID();
                                htFactory.Add(HidRID.Value, dr["orderform_rid"]);
                                Session["htFactory"] = htFactory;
                            }
                            dr["orderform_detail_rid"] = depManager.GetMaxOFDID(dr["orderform_rid"].ToString());

                            dr["Pass_Status"] = "1";
                            dr["Space_Short_RID"] = this.dropSpace_Short_RID.SelectedValue;
                            dr["number"] = txtNumber.Text;
                            dr["Agreement_RID"] = dropAgreement_RID.SelectedValue;
                            dr["Agreement_Name"] = dropAgreement_RID.SelectedItem.Text;
                            dr["Budget_RID"] = this.dropBudget_RID.SelectedValue;
                            dr["Budget_Name"] = this.dropBudget_RID.SelectedItem.Text;
                            dr["factory"] = HidRID.Value;
                            dr["factory_id"] = txtFactory_shortname_cn.Text;
                            dr["Base_Price"] = this.lblBase_price.Text;
                            dr["Fore_Delivery_Date"] = this.txtFore_Delivery_Date.Text;
                            dr["Wafer_RID"] = this.dropWafer_Name.SelectedValue;
                            dr["Wafer_Name"] = this.dropWafer_Name.SelectedItem.Text;
                            dr["Wafer_Capacity"] = txtWafer_Capacity.Text;
                            if (radlIs_Exigence.SelectedIndex == 0)
                            {
                                dr["is_exigence"] = "1";
                            }
                            else if (radlIs_Exigence.SelectedIndex == 1)
                            {
                                dr["is_exigence"] = "2";
                            }
                            dr["Delivery_Address_RID"] = this.dropFactory_shorname_cn.SelectedValue;

                            dr["factory_shortname_cn"] = this.dropFactory_shorname_cn.SelectedItem.Text;

                            dr["comment"] = txtComment.Text;

                            dr["bz"] = "1";

                            //add chaoma start
                            dr["Change_UnitPrice"] = this.txtChange_Unitprice.Text;
                            //add chaoma end 

                            depManager.AUU(dr);

                            temp.Rows.Add(dr);
                        }

                        //Response.Write("<script>returnValue='1';window.close();</script>");
                    }
                    //如果當前的數據行為空,且不是從暫存表得來的時候
                    else
                    {
                        DataRow dr = temp.NewRow();

                        dr["Pass_Status"] = "1";

                        if (temp.Rows.Count > 0 && !StringUtil.IsEmpty(temp.Rows[0]["orderform_rid"].ToString()))
                        {
                            int j = 0;
                            for (int i = 0; i < temp.Rows.Count; i++)
                            {
                                //根據空白卡廠決定訂單詳細中的訂單編號是否相同
                                if (temp.Rows[i]["factory_id"].ToString() == this.txtFactory_shortname_cn.Text)
                                {
                                    dr["orderform_rid"] = temp.Rows[i]["orderform_rid"].ToString();
                                    j = int.Parse(temp.Rows[i]["orderform_detail_rid"].ToString().Substring(10, 2));
                                    //dr["orderform_detail_rid"] = depManager.GetMaxOFDID(dr["orderform_rid"].ToString(),temp.Rows[i]["orderform_detail_rid"].ToString());
                                    break;
                                }
                            }
                            int k = 0;
                            for (int i = 0; i < temp.Rows.Count; i++)
                            {
                                if (temp.Rows[i]["factory_id"].ToString() == this.txtFactory_shortname_cn.Text)
                                {
                                    k = Convert.ToInt32(temp.Rows[i]["orderform_detail_rid"].ToString().Substring(10, 2));
                                    if (j < k || j == k)
                                    {
                                        j = k;
                                    }
                                }
                            }
                            if (k != 0)
                            {
                                if (k.ToString().Length == 1)
                                {
                                    dr["orderform_detail_rid"] = depManager.GetMaxOFDID(dr["orderform_rid"].ToString(), dr["orderform_rid"].ToString() + "0" + k.ToString());
                                }
                                else if (k.ToString().Length == 2)
                                {
                                    dr["orderform_detail_rid"] = depManager.GetMaxOFDID(dr["orderform_rid"].ToString(), dr["orderform_rid"].ToString() + k.ToString());
                                }
                                else
                                {
                                    throw new AlertException(GlobalStringManager.Default["Alert_MaxNum"]);
                                }
                            }
                            else
                            {
                                dr["orderform_detail_rid"] = "";
                            }

                            if (dr["orderform_rid"].ToString() == "" && dr["orderform_detail_rid"].ToString() == "")
                            {
                                dr["orderform_rid"] = depManager.GetMaxOFID(temp.Rows[temp.Rows.Count - 1]["orderform_rid"].ToString());
                                dr["orderform_detail_rid"] = depManager.GetMaxOFDID(dr["orderform_rid"].ToString(), "");
                            }
                        }
                        else
                        {
                            dr["orderform_rid"] = depManager.GetMaxOFID();
                            dr["orderform_detail_rid"] = depManager.GetMaxOFDID(dr["orderform_rid"].ToString());
                        }

                        dr["Space_Short_RID"] = this.dropSpace_Short_RID.SelectedValue;
                        dr["number"] = txtNumber.Text;
                        dr["Agreement_RID"] = dropAgreement_RID.SelectedValue;
                        dr["Agreement_Name"] = dropAgreement_RID.SelectedItem.Text;
                        dr["Budget_RID"] = this.dropBudget_RID.SelectedValue;
                        dr["Budget_Name"] = this.dropBudget_RID.SelectedItem.Text;
                        dr["factory"] = HidRID.Value;
                        dr["factory_id"] = txtFactory_shortname_cn.Text;
                        dr["Base_Price"] = this.lblBase_price.Text;
                        dr["Fore_Delivery_Date"] = this.txtFore_Delivery_Date.Text;
                        dr["Wafer_RID"] = this.dropWafer_Name.SelectedValue;
                        dr["Wafer_Name"] = this.dropWafer_Name.SelectedItem.Text;
                        dr["Wafer_Capacity"] = txtWafer_Capacity.Text;
                        if (radlIs_Exigence.SelectedIndex == 0)
                        {
                            dr["is_exigence"] = "1";
                        }
                        else if (radlIs_Exigence.SelectedIndex == 1)
                        {
                            dr["is_exigence"] = "2";
                        }
                        dr["Delivery_Address_RID"] = this.dropFactory_shorname_cn.SelectedValue;
                        dr["factory_shortname_cn"] = this.dropFactory_shorname_cn.SelectedItem.Text;
                        dr["comment"] = txtComment.Text;

                        dr["bz"] = "1";

                        //add chaoma start
                        dr["Change_UnitPrice"] = this.txtChange_Unitprice.Text;
                        //add chaoma end 

                        depManager.AUU(dr);

                        temp.Rows.Add(dr);

                        //if (strRID =="0")
                        //{
                        //    DataRow deldr = temp.Rows[int.Parse(strRID)];
                        //    temp.Rows.Remove(deldr);
                        //}

                        //Response.Write("<script>returnValue='1';window.close();</script>");
                    }
                }
                else
                {
                    DataRow dr = temp.NewRow();
                    dr["Pass_Status"] = "1";
                    dr["orderform_rid"] = depManager.GetMaxOFID();
                    dr["orderform_detail_rid"] = depManager.GetMaxOFDID();
                    dr["Space_Short_RID"] = this.dropSpace_Short_RID.SelectedValue;
                    dr["number"] = txtNumber.Text;
                    dr["Agreement_RID"] = dropAgreement_RID.SelectedValue;
                    dr["Agreement_Name"] = dropAgreement_RID.SelectedItem.Text;
                    dr["Budget_RID"] = this.dropBudget_RID.SelectedValue;
                    dr["Budget_Name"] = this.dropBudget_RID.SelectedItem.Text;
                    dr["factory"] = HidRID.Value;
                    dr["factory_id"] = txtFactory_shortname_cn.Text;
                    dr["Base_Price"] = this.lblBase_price.Text;
                    dr["Fore_Delivery_Date"] = this.txtFore_Delivery_Date.Text;
                    dr["Wafer_RID"] = this.dropWafer_Name.SelectedValue;
                    dr["Wafer_Name"] = this.dropWafer_Name.SelectedItem.Text;
                    dr["Wafer_Capacity"] = txtWafer_Capacity.Text;
                    if (radlIs_Exigence.SelectedIndex == 0)
                    {
                        dr["is_exigence"] = "1";
                    }
                    else if (radlIs_Exigence.SelectedIndex == 1)
                    {
                        dr["is_exigence"] = "2";
                    }
                    dr["Delivery_Address_RID"] = this.dropFactory_shorname_cn.SelectedValue;
                    dr["factory_shortname_cn"] = this.dropFactory_shorname_cn.SelectedItem.Text;
                    dr["comment"] = txtComment.Text;

                    dr["bz"] = "1";

                    //add chaoma start
                    dr["Change_UnitPrice"] = txtChange_Unitprice.Text;
                    //add chaoma end 

                    depManager.AUU(dr);

                    temp.Rows.Add(dr);

                    //Response.Write("<script>returnValue='1';window.close();</script>");
                }
            }
            else
            {
                throw new Exception("訂單數量有誤！");
            }

            Session["detail"] = temp;

            Session["drid"] = null;

            Response.Write("<script>returnValue='1';window.close();</script>");
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //用途變更
    protected void dropCard_Purpose_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            dropCard_Group_RIDBind();
            dropSpace_Short_RIDBind();
            GetWafer_NAME();
            ChangeBind();
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //群組變更
    protected void dropCard_Group_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            dropSpace_Short_RIDBind();
            GetWafer_NAME();
            ChangeBind();
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //卡種變更
    protected void dropSpace_Short_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (!StringUtil.IsEmpty(dropSpace_Short_RID.SelectedValue))
            {
                GetWafer_NAME();

                if (txtNumber.Text != "0")
                {
                    ChangeBind();
                }

                if (txtNumber.Text.Trim() != "" && txtNumber.Text.Trim() != "0")
                {
                    //判斷是否為修改操作
                    if (Session["drid"] != null)
                    {
                        DataRow drow = (DataRow)Session["drid"];

                        long ce = 0;
                        ce = Convert.ToInt64(txtNumber.Text.Trim()) - Convert.ToInt64(drow["number"].ToString());
                        //數量與卡種都沒有變更
                        if (ce == 0 && dropSpace_Short_RID.SelectedValue == drow["Space_Short_RID"].ToString())
                        {
                            if (Convert.ToInt64(depManager.GetBudget(drow["Budget_RID"].ToString())) >= ce)
                            {
                                int count = 0;
                                for (int i = 0; i < dropBudget_RID.Items.Count; i++)
                                {
                                    if (dropBudget_RID.Items[i].Value == drow["Budget_RID"].ToString())
                                    {
                                        break;
                                    }
                                    count++;
                                }
                                if (count == dropBudget_RID.Items.Count)
                                {
                                    ListItem li = new ListItem();
                                    li.Text = drow["Budget_Name"].ToString();
                                    li.Value = drow["Budget_RID"].ToString();
                                    dropBudget_RID.Items.Add(li);
                                }
                            }

                            if (Convert.ToInt64(depManager.GetAgreement(drow["Agreement_RID"].ToString())) >= ce)
                            {
                                int countd = 0;
                                for (int i = 0; i < dropAgreement_RID.Items.Count; i++)
                                {
                                    if (dropAgreement_RID.Items[i].Value == drow["Agreement_RID"].ToString())
                                    {
                                        break;
                                    }
                                    countd++;
                                }
                                //判斷預算下拉框中是否還包含"預算(資料庫保存後的)"，兩邊相等則不包含
                                if (countd == dropAgreement_RID.Items.Count)
                                {
                                    ListItem lit = new ListItem();
                                    lit.Text = drow["Agreement_Name"].ToString();
                                    lit.Value = drow["Agreement_RID"].ToString();
                                    dropAgreement_RID.Items.Add(lit);
                                    this.txtFactory_shortname_cn.Text = drow["factory_shortname_cn"].ToString();
                                }
                            }

                            ListItem liw = new ListItem();
                            liw.Text = drow["Wafer_Name"].ToString();
                            liw.Value = drow["Wafer_RID"].ToString();

                            ListItem lip = new ListItem();
                            lip.Text = drow["factory_shortname_cn"].ToString();
                            lip.Value = drow["Delivery_Address_RID"].ToString();

                            if (0 == dropWafer_Name.Items.Count || dropWafer_Name.Items.Contains(liw) == false)
                            {
                                dropWafer_Name.Items.Add(liw);
                            }

                            //判斷Perso廠下拉框是否還包含"Perso廠(資料庫保存的)"
                            if (0 == dropFactory_shorname_cn.Items.Count || dropFactory_shorname_cn.Items.Contains(lip) == false)
                            {
                                dropFactory_shorname_cn.Items.Add(lip);
                            }
                        }
                    }
                }
            }
            else
            {
                dropBudget_RID.Items.Clear();
                dropAgreement_RID.Items.Clear();
                ShowMessage("請選擇卡種！");
            }
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //合約變更
    protected void drpAgreement_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //根據合約加載單價(卡種材質金額+合約單價)
            double num = 0;
            if (Session["dtAGREEMENT"] != null)
            {
                DataTable dtAgreement = (DataTable)Session["dtAGREEMENT"];
                if (dtAgreement.Rows.Count > 0)
                {
                    if (dtAgreement.Rows[dropAgreement_RID.SelectedIndex]["type"].ToString() == "1")//基本價格:1 級距價格:0
                    {
                        num = Convert.ToDouble(dtAgreement.Rows[dropAgreement_RID.SelectedIndex]["Base_Price"]);
                    }
                    else
                    {
                        num = Convert.ToDouble(dtAgreement.Rows[dropAgreement_RID.SelectedIndex]["Price"]);
                    }
                    //num += depManager.GetMaterialPrice(dropSpace_Short_RID.SelectedValue, dtAgreement.Rows[dropAgreement_RID.SelectedIndex]["agreement_group_rid"].ToString());

                    //判斷卡種是否有對應特殊材質
                    if (depManager.IsCardtype_Material(dropSpace_Short_RID.SelectedValue))
                    {
                        //獲得卡種對應所有材質及其價格、合約
                        DataSet dstmp = depManager.GetMaterialPrice(dropSpace_Short_RID.SelectedValue, dtAgreement.Rows[dropAgreement_RID.SelectedIndex]["agreement_group_rid"].ToString());
                        if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                        {
                            string strErr = "";
                            for (int i = 0; i < dstmp.Tables[0].Rows.Count; i++)
                            {
                                if (dstmp.Tables[0].Rows[i]["agreement_group_rid"].ToString() != "")
                                {
                                    num += Convert.ToDouble(Convert.ToDouble(dstmp.Tables[0].Rows[i]["Base_Price"].ToString()));
                                }
                                else
                                {
                                    strErr += "下單卡種所使用特殊材質：" + dstmp.Tables[0].Rows[i]["material_name"].ToString() + "未在此合約中設定；\\n";
                                    continue;
                                }
                            }

                            if (strErr != "")
                            {
                                ShowMessage(strErr);
                            }
                        }
                    }

                    this.txtFactory_shortname_cn.Text = dtAgreement.Rows[dropAgreement_RID.SelectedIndex]["factory_shortname_cn"].ToString();
                    HidRID.Value = dtAgreement.Rows[dropAgreement_RID.SelectedIndex]["factory_rid"].ToString();

                    dropWafer_Name.Items.Clear();

                    DataSet dtmp = depManager.DropWafer(dropSpace_Short_RID.SelectedValue, HidRID.Value);
                    if (dtmp != null && dtmp.Tables[0].Rows.Count > 0)
                    {
                        dropWafer_Name.Items.Clear();
                        dropWafer_Name.DataSource = dtmp.Tables[0];
                        dropWafer_Name.DataTextField = "text";
                        dropWafer_Name.DataValueField = "value";
                        dropWafer_Name.DataBind();

                        //根據晶片名稱得到晶片容量
                        txtWafer_Capacity.Text = dtmp.Tables[0].Rows[0]["wafer_capacity"].ToString();
                    }
                }

                this.lblBase_price.Text = num.ToString("N2");

                //add by Ian Huang start
                txtChange_Unitprice.Text = num.ToString("N2");
                //add by Ian Huang end
            }
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //數量變更
    protected void txtNumber_TextChanged(object sender, EventArgs e)
    {
        try
        {
            txtNumber.Text = txtNumber.Text.Replace(",", "");
            if (txtNumber.Text.Trim() == "")
            {
                ShowMessage("下單數量不能為空！");
            }
            else
            {
                if (!StringUtil.IsEmpty(dropSpace_Short_RID.SelectedValue))
                {
                    if (txtNumber.Text.Trim() != "0")
                    {
                        ChangeBind();

                        //判斷是否為修改操作
                        if (Session["drid"] != null)
                        {
                            DataRow drow = (DataRow)Session["drid"];

                            long ce = 0;
                            ce = Convert.ToInt64(txtNumber.Text.Trim()) - Convert.ToInt64(drow["number"].ToString());
                            //數量與卡種都沒有變更
                            if (ce == 0 && dropSpace_Short_RID.SelectedValue == drow["Space_Short_RID"].ToString())
                            {
                                if (Convert.ToInt64(depManager.GetBudget(drow["Budget_RID"].ToString())) >= ce)
                                {
                                    int count = 0;
                                    for (int i = 0; i < dropBudget_RID.Items.Count; i++)
                                    {
                                        if (dropBudget_RID.Items[i].Value == drow["Budget_RID"].ToString())
                                        {
                                            break;
                                        }
                                        count++;
                                    }
                                    if (count == dropBudget_RID.Items.Count)
                                    {
                                        ListItem li = new ListItem();
                                        li.Text = drow["Budget_Name"].ToString();
                                        li.Value = drow["Budget_RID"].ToString();
                                        dropBudget_RID.Items.Add(li);
                                    }
                                }

                                if (Convert.ToInt64(depManager.GetAgreement(drow["Agreement_RID"].ToString())) >= ce)
                                {
                                    int countd = 0;
                                    for (int i = 0; i < dropAgreement_RID.Items.Count; i++)
                                    {
                                        if (dropAgreement_RID.Items[i].Value == drow["Agreement_RID"].ToString())
                                        {
                                            break;
                                        }
                                        countd++;
                                    }
                                    //判斷預算下拉框中是否還包含"預算(資料庫保存後的)"，兩邊相等則不包含
                                    if (countd == dropAgreement_RID.Items.Count)
                                    {
                                        ListItem lit = new ListItem();
                                        lit.Text = drow["Agreement_Name"].ToString();
                                        lit.Value = drow["Agreement_RID"].ToString();
                                        dropAgreement_RID.Items.Add(lit);
                                        this.txtFactory_shortname_cn.Text = drow["factory_shortname_cn"].ToString();
                                    }
                                }

                                ListItem liw = new ListItem();
                                liw.Text = drow["Wafer_Name"].ToString();
                                liw.Value = drow["Wafer_RID"].ToString();

                                ListItem lip = new ListItem();
                                lip.Text = drow["factory_shortname_cn"].ToString();
                                lip.Value = drow["Delivery_Address_RID"].ToString();

                                if (0 == dropWafer_Name.Items.Count || dropWafer_Name.Items.Contains(liw) == false)
                                {
                                    dropWafer_Name.Items.Add(liw);
                                }

                                //判斷Perso廠下拉框是否還包含"Perso廠(資料庫保存的)"
                                if (0 == dropFactory_shorname_cn.Items.Count || dropFactory_shorname_cn.Items.Contains(lip) == false)
                                {
                                    dropFactory_shorname_cn.Items.Add(lip);
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowMessage("數量不能為0！");
                    }
                }
                else
                {
                    ShowMessage("請選擇卡種！");
                }
            }
        }
        catch (AlertException ale)
        {
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #region 欄位/資料補充說明
    public void ChangeBind()
    {
        try
        {
            Session["dtAGREEMENT"] = null;

            if (txtNumber.Text.Trim() == "")
            {
                txtNumber.Text = "0";
            }

            inputs.Clear();
            inputs.Add("Order_Date", System.DateTime.Now.ToString("yyyy/MM/dd"));
            inputs.Add("txtNumber", txtNumber.Text.Replace(",", ""));
            inputs.Add("dropSpace_Short_RID", dropSpace_Short_RID.SelectedValue);

            dropAgreement_RID.Items.Clear();
            dropBudget_RID.Items.Clear();
            dropWafer_Name.Items.Clear();
            dropFactory_shorname_cn.Items.Clear();

            //判斷是否為修改操作
            if (Session["drid"] != null)
            {
                //根據數量和卡種得到對應的合約、預算、晶片名稱、交貨地點（perso廠）
                if (((DataTable)Session["detail"]).Rows[0]["bz"].ToString() == "2")
                    ds = depManager.GetOrderFormDetailCombo(inputs, false);
                else
                    ds = depManager.GetOrderFormDetailCombo(inputs, true);
            }
            else
            {
                ds = depManager.GetOrderFormDetailCombo(inputs, false);
            }

            if (ds.Tables["AGREEMENT"].Rows.Count > 0)
            {
                this.dropAgreement_RID.DataSource = ds.Tables["AGREEMENT"];
                dropAgreement_RID.DataTextField = "agreement_name";
                dropAgreement_RID.DataValueField = "rid";
                dropAgreement_RID.DataBind();
                Session["dtAGREEMENT"] = ds.Tables["AGREEMENT"];

                this.txtFactory_shortname_cn.Text = ds.Tables["AGREEMENT"].Rows[0]["factory_shortname_cn"].ToString();
                HidRID.Value = ds.Tables["AGREEMENT"].Rows[0]["factory_rid"].ToString();
            }

            if (ds.Tables["CARD_BUDGET"].Rows.Count > 0)
            {
                this.dropBudget_RID.DataSource = ds.Tables["CARD_BUDGET"];
                dropBudget_RID.DataTextField = "budget_Name";
                dropBudget_RID.DataValueField = "rid";
                dropBudget_RID.DataBind();
            }

            if (ds.Tables["PERSO_CARDTYPE"].Rows.Count > 0)
            {
                dropFactory_shorname_cn.DataSource = ds.Tables["PERSO_CARDTYPE"];
                dropFactory_shorname_cn.DataTextField = "text";
                dropFactory_shorname_cn.DataValueField = "value";
                dropFactory_shorname_cn.DataBind();
            }

            if (txtNumber.Text.Replace(",", "").Trim() == "" || dropSpace_Short_RID.SelectedValue == "")
            {
                dropAgreement_RID.Items.Clear();
                dropBudget_RID.Items.Clear();
            }

            //foreach (DataRow drowAgreemnent in ds.Tables["AGREEMENT"].Rows)
            //{
            //    //判斷卡種是否有對應特殊材質
            //    if (depManager.IsCardtype_Material(dropSpace_Short_RID.SelectedValue))
            //    {
            //        //獲得卡種對應所有材質及其價格、合約
            //        DataSet dstmp1 = depManager.GetMaterialPrice(dropSpace_Short_RID.SelectedValue, drowAgreemnent["agreement_group_rid"].ToString());
            //        if (dstmp1 != null && dstmp1.Tables[0].Rows.Count > 0)
            //        {
            //            for (int i = 0; i < dstmp1.Tables[0].Rows.Count; i++)
            //            {
            //                if (dstmp1.Tables[0].Rows[i]["agreement_group_rid"].ToString() == "")
            //                {
            //                    if (dropAgreement_RID.Items.FindByValue(drowAgreemnent["RID"].ToString()) != null)
            //                        dropAgreement_RID.Items.Remove(dropAgreement_RID.Items.FindByValue(drowAgreemnent["RID"].ToString()));
            //                }
            //            }
            //        }
            //    }
            //}

            //判斷是否為修改操作
            if (Session["drid"] != null)
            {
                DataRow drow = (DataRow)Session["drid"];
                long ce = 0;
                ce = Convert.ToInt64(txtNumber.Text.Trim()) - Convert.ToInt64(drow["number"].ToString());
                //數量與卡種都沒有變更
                if (ce == 0 && dropSpace_Short_RID.SelectedValue == drow["Space_Short_RID"].ToString())
                {
                    ListItem li = new ListItem();
                    li.Text = drow["Budget_Name"].ToString();
                    li.Value = drow["Budget_RID"].ToString();

                    ListItem lit = new ListItem();
                    lit.Text = drow["Agreement_Name"].ToString();
                    lit.Value = drow["Agreement_RID"].ToString();

                    ListItem liw = new ListItem();
                    liw.Text = drow["Wafer_Name"].ToString();
                    liw.Value = drow["Wafer_RID"].ToString();

                    ListItem lip = new ListItem();
                    lip.Text = drow["factory_shortname_cn"].ToString();
                    lip.Value = drow["Delivery_Address_RID"].ToString();

                    //判斷預算下拉框中是否還包含"預算(資料庫保存的)"
                    if (0 == dropBudget_RID.Items.Count || dropBudget_RID.Items.Contains(li) == false)
                    {
                        if (!StringUtil.IsEmpty(li.Value))
                            dropBudget_RID.Items.Add(li);
                    }

                    //判斷合約下拉框中是否還包含"合約(資料庫保存的)"
                    if (0 == dropAgreement_RID.Items.Count || dropAgreement_RID.Items.Contains(lit) == false)
                    {
                        if (!StringUtil.IsEmpty(lit.Value))
                            dropAgreement_RID.Items.Add(lit);
                        //this.dropFactory_shorname_cn.Text = drow["factory_shortname_cn"].ToString();
                    }

                    if (0 == dropWafer_Name.Items.Count || dropWafer_Name.Items.Contains(liw) == false)
                    {
                        if (!StringUtil.IsEmpty(liw.Value))
                        {
                            dropWafer_Name.Items.Add(liw);
                            txtWafer_Capacity.Text = depManager.GetWafer_Capacity(liw.Value);
                        }
                    }

                    //判斷Perso廠下拉框是否還包含"Perso廠(資料庫保存的)"
                    if (0 == dropFactory_shorname_cn.Items.Count || dropFactory_shorname_cn.Items.Contains(lip) == false)
                    {
                        if (!StringUtil.IsEmpty(lip.Value))
                            dropFactory_shorname_cn.Items.Add(lip);
                    }

                    if (!StringUtil.IsEmpty(drow["Agreement_RID"].ToString()))
                        dropAgreement_RID.SelectedValue = drow["Agreement_RID"].ToString();
                    if (!StringUtil.IsEmpty(drow["Budget_RID"].ToString()))
                        dropBudget_RID.SelectedValue = drow["Budget_RID"].ToString();
                    if (!StringUtil.IsEmpty(drow["Wafer_RID"].ToString()))
                        dropWafer_Name.SelectedValue = drow["Wafer_RID"].ToString();
                    if (!StringUtil.IsEmpty(drow["Delivery_Address_RID"].ToString()))
                        dropFactory_shorname_cn.SelectedValue = drow["Delivery_Address_RID"].ToString();
                }
            }

            DataSet dtmp = depManager.DropWafer(dropSpace_Short_RID.SelectedValue, HidRID.Value);
            if (dtmp != null && dtmp.Tables[0].Rows.Count > 0)
            {
                dropWafer_Name.Items.Clear();
                dropWafer_Name.DataSource = dtmp.Tables[0];
                dropWafer_Name.DataTextField = "text";
                dropWafer_Name.DataValueField = "value";
                dropWafer_Name.DataBind();

                //根據晶片名稱得到晶片容量
                txtWafer_Capacity.Text = dtmp.Tables[0].Rows[0]["wafer_capacity"].ToString();
            }

            //根據合約加載單價(卡種材質金額+合約單價)
            double num = 0;
            if (ds.Tables["AGREEMENT"].Rows.Count > 0)
            {
                if (ds.Tables["AGREEMENT"].Rows[0]["type"].ToString() == "1")//基本價格:1 級距價格:0
                {
                    num = Convert.ToDouble(ds.Tables["AGREEMENT"].Rows[0]["Base_Price"]);
                }
                else
                {
                    num = Convert.ToDouble(ds.Tables["AGREEMENT"].Rows[0]["Price"]);
                }
                //num += depManager.GetMaterialPrice(dropSpace_Short_RID.SelectedValue, ds.Tables["AGREEMENT"].Rows[0]["agreement_group_rid"].ToString());

                //判斷卡種是否有對應特殊材質
                if (depManager.IsCardtype_Material(dropSpace_Short_RID.SelectedValue))
                {
                    //獲得卡種對應所有材質及其價格、合約
                    DataSet dstmp = depManager.GetMaterialPrice(dropSpace_Short_RID.SelectedValue, ds.Tables["AGREEMENT"].Rows[0]["agreement_group_rid"].ToString());
                    if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                    {
                        string strErr = "";
                        for (int i = 0; i < dstmp.Tables[0].Rows.Count; i++)
                        {
                            if (dstmp.Tables[0].Rows[i]["agreement_group_rid"].ToString() != "")
                            {
                                num += Convert.ToDouble(Convert.ToDouble(dstmp.Tables[0].Rows[i]["Base_Price"].ToString()));
                            }
                            else
                            {
                                strErr += "下單卡種所使用特殊材質：" + dstmp.Tables[0].Rows[i]["material_name"].ToString() + "未在此合約中設定；\\n";
                                continue;
                            }
                        }
                        if (strErr != "")
                        {
                            ShowMessage(strErr);
                        }
                    }
                }
            }
            //add chaoma start
            this.lblBase_price.Text = num.ToString("N2");

            //add by Ian Huang start
            //如果 調整后單價 欄位為空，則帶入 合約單價
            //if ("" == txtChange_Unitprice.Text.Trim())
            //{
            txtChange_Unitprice.Text = num.ToString("N2");
            //}
            //else
            //{
            //    if (0 == Convert.ToDouble(txtChange_Unitprice.Text.Trim()))
            //    {
            //        txtChange_Unitprice.Text = num.ToString("N2");
            //    }
            //}
            //add by Ian Huang end
            //add chaoma end

            //判斷是否為修改操作
            if (Session["drid"] != null)
            {
                DataRow drow = (DataRow)Session["drid"];
                long ce = 0;
                ce = Convert.ToInt64(txtNumber.Text.Trim()) - Convert.ToInt64(drow["number"].ToString());
                //數量與卡種都沒有變更
                if (ce == 0 && dropSpace_Short_RID.SelectedValue == drow["Space_Short_RID"].ToString() && dropAgreement_RID.SelectedValue == drow["Agreement_RID"].ToString())
                {
                    if (!StringUtil.IsEmpty(drow["Base_Price"].ToString()))
                        this.lblBase_price.Text = drow["Base_Price"].ToString();
                }
            }
        }
        catch (AlertException ale)
        {
            dropAgreement_RID.Items.Clear();
            dropBudget_RID.Items.Clear();
            dropWafer_Name.Items.Clear();
            dropFactory_shorname_cn.Items.Clear();
            this.txtFactory_shortname_cn.Text = "";
            HidRID.Value = "";
            this.lblBase_price.Text = "";
            txtWafer_Capacity.Text = "";
            ShowMessage(ale.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_Purpose_RIDBind()
    {
        dropCard_Purpose_RID.DataTextField = "PARAM_NAME";
        dropCard_Purpose_RID.DataValueField = "Param_Code";
        dropCard_Purpose_RID.DataSource = ctmManager.GetPurpose();
        dropCard_Purpose_RID.DataBind();
        dropCard_Purpose_RID.Items.Insert(0, new ListItem("全部", ""));
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_Group_RIDBind()
    {
        dropCard_Group_RID.Items.Clear();


        dropCard_Group_RID.DataTextField = "GROUP_NAME";
        dropCard_Group_RID.DataValueField = "RID";
        dropCard_Group_RID.DataSource = ctmManager.GetGroupByPurposeId(dropCard_Purpose_RID.SelectedValue);
        dropCard_Group_RID.DataBind();
        dropCard_Group_RID.Items.Insert(0, new ListItem("全部", ""));

    }

    /// <summary>
    /// 卡種綁定
    /// </summary>
    protected void dropSpace_Short_RIDBind()
    {
        dropSpace_Short_RID.Items.Clear();


        dropSpace_Short_RID.DataTextField = "Display_Name";
        dropSpace_Short_RID.DataValueField = "RID";
        dropSpace_Short_RID.DataSource = ctmManager.GetCardTypeByGroupId(dropCard_Purpose_RID.SelectedValue, dropCard_Group_RID.SelectedValue);
        dropSpace_Short_RID.DataBind();
        dropSpace_Short_RID.Items.Insert(0, new ListItem("請選擇卡種", ""));

    }

    /// <summary>
    /// 獲取晶體名稱
    /// </summary>
    private void GetWafer_NAME()
    {
        dropWafer_Name.Items.Clear();

        if (!StringUtil.IsEmpty(dropSpace_Short_RID.SelectedValue))
        {
            dropWafer_Name.DataSource = depManager.GetWAFER_INFOBYCRID(dropSpace_Short_RID.SelectedValue);
            dropWafer_Name.DataBind();
        }
    }
    #endregion    
    protected void dropWafer_Name_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtWafer_Capacity.Text = "";
        if (dropWafer_Name.SelectedValue != "")
        {
            txtWafer_Capacity.Text = depManager.GetWafer_Capacity(dropWafer_Name.SelectedValue);
        }
    }
}
