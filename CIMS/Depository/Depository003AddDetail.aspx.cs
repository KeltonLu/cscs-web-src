//******************************************************************
//*  作    者：FangBao
//*  功能說明：卡片入庫人工匯入明細
//*  創建日期：2008-09-05
//*  修改日期：2008-09-05 12:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 鮑方
//*******************************************************************

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

public partial class Depository_Depository003AddDetail : PageBase
{
    CardTypeManager ctmManager = new CardTypeManager();
    Depository003BL BL = new Depository003BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!IsPostBack)
        {
            DataSet dstFactory = BL.GetFACTORYData();
            dropPerso_Factory_RID.DataSource = dstFactory.Tables[1];
            dropPerso_Factory_RID.DataBind();

            dropBlank_Factory_RID.DataSource = dstFactory.Tables[0];
            dropBlank_Factory_RID.DataBind();

            dropCard_Purpose_RIDBind();
            dropCard_Group_RIDBind();
            dropSpace_Short_RIDBind();
            CheckSerialNum();
            GetWafer_NAME();

            string strRID = Request.QueryString["RID"];
            if (!StringUtil.IsEmpty(strRID))
            {
                DataTable dtbl = (DataTable)Session["dtblImp"];
                DataRow drow = dtbl.Rows[int.Parse(strRID)];
                SetControlsForDataRow(drow);

                dropCard_Purpose_RID.SelectedValue = "";
                ShowIsUsingDrop(dropSpace_Short_RID, drow["Space_Short_RID"].ToString(),true);

                //dropCard_Group_RIDBind();
                //if (dropCard_Group_RID.Items.FindByValue(drow["Card_Group_RID"].ToString()) != null)
                //    dropCard_Group_RID.SelectedValue = drow["Card_Group_RID"].ToString();

                //dropSpace_Short_RIDBind();
                //if (dropSpace_Short_RID.Items.FindByValue(drow["Space_Short_RID"].ToString()) != null)
                //    dropSpace_Short_RID.SelectedValue = drow["Space_Short_RID"].ToString();

                GetWafer_NAME();
                if (dropWafer_RID.Items.FindByValue(drow["WAFER_RID"].ToString()) != null)
                    dropWafer_RID.SelectedValue = drow["WAFER_RID"].ToString();

                rblCheck_Type.SelectedValue = drow["Check_Type"].ToString();
                txtStock_Number.Text = int.Parse(txtStock_Number.Text).ToString("N0");
                txtBlemish_Number.Text = int.Parse(txtBlemish_Number.Text).ToString("N0");
                txtSample_Number.Text = int.Parse(txtSample_Number.Text).ToString("N0");
                txtIncome_Number.Text = int.Parse(txtIncome_Number.Text).ToString("N0");
                txtIncome_Date.Text = Convert.ToDateTime(txtIncome_Date.Text).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("1900/01/01", "");
            }
        }

        CalNum();
    }


    protected void dropCard_Purpose_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_Group_RIDBind();
        dropSpace_Short_RIDBind();
        CheckSerialNum();
        GetWafer_NAME();
    }

    protected void dropCard_Group_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropSpace_Short_RIDBind();
        CheckSerialNum();
        GetWafer_NAME();
    }

    protected void btnCalNum_Click(object sender, EventArgs e)
    {
        CheckSerialNum();
    }


    protected void dropSpace_Short_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        CheckSerialNum();
        GetWafer_NAME();
    }

    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        if (!CheckSerialNum())
        {
            return;
        }

        if (StringUtil.GetByteLength(txtComment.Text) > 200)
        {
            ShowMessage("備註不能超過200個字符");
            return;
        }

        if (int.Parse(txtIncome_Number.Text.Replace(",", "")) < 0)
        {
            ShowMessage("入庫量不能為負數");
            return;
        }

        if (BL.IsCheck1(txtIncome_Date.Text))
        {
            ShowMessage("入庫日期已經日結");
            return;
        }

        //if (StringUtil.IsEmpty(dropCard_Group_RID.SelectedValue))
        //{
        //    ShowMessage("群組不能為空");
        //    return;
        //}

        if (StringUtil.IsEmpty(dropSpace_Short_RID.SelectedValue))
        {
            ShowMessage("卡種不能為空");
            return;
        }

        if (StringUtil.IsEmpty(dropWafer_RID.SelectedValue))
        {
            ShowMessage("晶片名稱不能為空");
            return;
        }

        string strRID = Request.QueryString["RID"];
        DataTable dtbl = (DataTable)Session["dtblImp"];
        DataRow drow;

        if (!StringUtil.IsEmpty(strRID))
            drow = dtbl.Rows[int.Parse(strRID)];
        else
            drow = dtbl.NewRow();

        drow["Card_Purpose_RID"] = dropCard_Purpose_RID.SelectedValue;
        drow["Card_Purpose_NAME"] = dropCard_Purpose_RID.SelectedItem.Text;
        drow["Card_Group_RID"] = dropCard_Group_RID.SelectedValue;
        drow["Card_Group_NAME"] = dropCard_Group_RID.SelectedItem.Text;
        drow["Space_Short_RID"] = dropSpace_Short_RID.SelectedValue;
        drow["Space_Short_NAME"] = dropSpace_Short_RID.SelectedItem.Text;
        drow["Stock_Number"] = txtStock_Number.Text.Replace(",", "");
        drow["Blemish_Number"] = txtBlemish_Number.Text.Replace(",", "");
        drow["Sample_Number"] = txtSample_Number.Text.Replace(",", "");
        drow["Income_Number"] = txtIncome_Number.Text.Replace(",", "");
        drow["Income_Date"] = txtIncome_Date.Text;
        drow["Serial_Number"] = txtSerial_Number.Text;
        drow["Perso_Factory_RID"] = dropPerso_Factory_RID.SelectedValue;
        drow["Perso_Factory_NAME"] = dropPerso_Factory_RID.SelectedItem.Text;
        drow["Blank_Factory_RID"] = dropBlank_Factory_RID.SelectedValue;
        drow["Blank_Factory_NAME"] = dropBlank_Factory_RID.SelectedItem.Text;
        drow["Wafer_RID"] = dropWafer_RID.SelectedValue;
        drow["Wafer_NAME"] = dropWafer_RID.SelectedItem.Text;
        drow["Check_Type"] = rblCheck_Type.SelectedValue;
        drow["Comment"] = txtComment.Text;
        drow["SendCheck_Status"] = txtSendCheck_Status.Text;

        if (StringUtil.IsEmpty(strRID))
            dtbl.Rows.Add(drow);

        ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "returnValue='1';window.close();", true);
    }

    #endregion

    #region 欄位/資料補充說明
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
        dropSpace_Short_RID.DataSource = ctmManager.GetCardTypeByGroupId(dropCard_Purpose_RID.SelectedValue, dropCard_Group_RID.SelectedValue, "", true);
        dropSpace_Short_RID.DataBind();

    }

   


    /// <summary>
    /// 計算入庫量
    /// </summary>
    private void CalNum()
    {
        int income = 0;
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        if (!StringUtil.IsEmpty(txtStock_Number.Text))
            num1 = int.Parse(txtStock_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtBlemish_Number.Text))
            num2 = int.Parse(txtBlemish_Number.Text.Replace(",", ""));
        if (!StringUtil.IsEmpty(txtSample_Number.Text))
            num3 = int.Parse(txtSample_Number.Text.Replace(",", ""));

        income = num1 - num2 - num3;

        txtIncome_Number.Text = income.ToString("N0");
    }

    /// <summary>
    /// 獲取晶體名稱
    /// </summary>
    private void GetWafer_NAME()
    {
        dropWafer_RID.Items.Clear();

        if (!StringUtil.IsEmpty(dropSpace_Short_RID.SelectedValue))
        {
            dropWafer_RID.DataSource = BL.GetWAFER_INFOBYCRID(dropSpace_Short_RID.SelectedValue);
            dropWafer_RID.DataBind();
        }
    }



    private bool CheckSerialNum()
    {
        string strCheckStatus = "";

        txtSendCheck_Status.Text = "";

        if (!StringUtil.IsEmpty(txtSerial_Number.Text) && !StringUtil.IsEmpty(dropSpace_Short_RID.SelectedValue))
        {
            strCheckStatus = BL.GetCheckStatus(txtSerial_Number.Text, dropSpace_Short_RID.SelectedValue);

            strCheckStatus = strCheckStatus.Replace("1", "未送審").Replace("2", "已送審").Replace("3", "送審完成");

            txtSendCheck_Status.Text = strCheckStatus;

            if (StringUtil.IsEmpty(strCheckStatus))
            {
                ShowMessage("輸入了錯誤的卡片批號");
                return false;
            }
            else
                return true;
        }

        if (StringUtil.IsEmpty(txtSerial_Number.Text))
            return true;


        return true;
    }

    #endregion
}
