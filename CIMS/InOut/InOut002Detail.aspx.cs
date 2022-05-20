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
using CIMSClass;
using CIMSClass.Business;


public partial class InOut_InOut002Detail : PageBase
{
    InOut002BL bl = new InOut002BL();
    InOut001BL BL = new InOut001BL();
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 start
    InOut007BL BL007 = new InOut007BL();
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 end
    CardTypeManager ctmManager = new CardTypeManager();
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";
        if (!IsPostBack)
        {
            // 獲取 Perso廠商資料
            GetFactory();
            this.ViewState["type"] = Request.QueryString["type"].ToString();

            // 修改狀態
            if (Request.QueryString["type"].ToString() == "update")
            {
                this.ViewState["FactoryRID"] = Request.QueryString["Factory"].ToString();
                this.ViewState["Param_Code"] = Request.QueryString["Param_Code"].ToString();
                txtBegin_Date.Text = Request.QueryString["Date"].ToString();

                DataTable dtFactory_Change_Import = (DataTable)Session["Factory_Change_Import"];
                int intSelectIndex = Convert.ToInt32(Request.QueryString["RowIndex"].ToString());
                this.hidRowIndex.Value = intSelectIndex.ToString();// 保存行號

                // 獲取 Perso廠商資料
                GetFactory();
                for (int intRow = 0; intRow < this.dropFactory.Items.Count; intRow++)
                {
                    if (this.dropFactory.Items[intRow].Value.ToString() ==
                        Request.QueryString["Factory"].ToString())
                    {
                        this.dropFactory.Items[intRow].Selected = true;
                        break;
                    }
                }
                //綁定用途
                this.dropCard_Purpose_RIDBind();
                this.dropCard_Purpose_RID.SelectedValue = Request.QueryString["Param_Code"].ToString();
                //for (int intRow = 0; intRow < this.dropCard_Purpose_RID.Items.Count; intRow++)
                //{
                //    if (this.dropCard_Purpose_RID.Items[intRow].Value.ToString() ==
                //        Request.QueryString["Param_Code"].ToString())
                //    {
                //        this.dropCard_Purpose_RID.Items[intRow].Selected = true;
                //        break;
                //    }
                //}
                // 群組
                dropCard_Group_RIDBind(this.dropCard_Purpose_RID.SelectedValue.ToString());
                for (int intRow = 0; intRow < this.dropCard_Group_RID.Items.Count; intRow++)
                {
                    if (this.dropCard_Group_RID.Items[intRow].Value.ToString() ==
                        dtFactory_Change_Import.Rows[intSelectIndex]["CGRID"].ToString())
                    {
                        this.dropCard_Group_RID.Items[intRow].Selected = true;
                        break;
                    }
                }
                // 卡種
                dropSpace_Short_RIDBind(this.dropCard_Group_RID.SelectedValue.ToString());
                for (int intRow = 0; intRow < this.dropSpace_Short_RID.Items.Count; intRow++)
                {
                    if (this.dropSpace_Short_RID.Items[intRow].Value.ToString() ==
                        dtFactory_Change_Import.Rows[intSelectIndex]["Space_Short_RID"].ToString())
                    {
                        this.dropSpace_Short_RID.Items[intRow].Selected = true;
                        break;
                    }
                }
                // 卡種狀況
                dropStatusBind();
                for (int intRow = 0; intRow < this.dropStatus.Items.Count; intRow++)
                {
                    if (this.dropStatus.Items[intRow].Value.ToString() ==
                        dtFactory_Change_Import.Rows[intSelectIndex]["Status_RID"].ToString())
                    {
                        this.dropStatus.Items[intRow].Selected = true;
                        break;
                    }
                }
                // 數量
                this.txtNumber.Text = Convert.ToInt32(dtFactory_Change_Import.Rows[intSelectIndex]["Number"].ToString()).ToString("N0");
                // “新增”狀態
            }
            else
            {
                this.hidRowIndex.Value = "-1"; // 保存行號
                // 用途
                dropCard_Purpose_RIDBind();
                // 群組
                dropCard_Group_RIDBind(this.dropCard_Purpose_RID.SelectedValue.ToString());
                // 卡種
                dropSpace_Short_RIDBind(this.dropCard_Group_RID.SelectedValue.ToString());
                // 卡種狀況
                dropStatusBind();
                // 數量
                this.txtNumber.Text = "";
            }
        }
    }

    /// <summary>
    /// 綁定卡種狀況
    /// </summary>
    private void dropStatusBind()
    {
        dropStatus.DataTextField = "Status_Name";
        dropStatus.DataValueField = "RID";
        this.dropStatus.DataSource = bl.getCardTypeStatus().Tables[0];
        dropStatus.DataBind();
    }
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/22 start
    /// <summary>
    /// 設置日志數據
    /// </summary>
    private void SetLogData()
    {
        BL007.SetOprLogNull();
        if (HttpContext.Current.Session["Action"] != null)
        {
            BL007.SetOprLogActionID(HttpContext.Current.Session["Action"].ToString());
        }
        if (HttpContext.Current.Session[CIMSClass.GlobalString.SessionAndCookieKeys.USER] != null)
        {
            USERS mUser = (USERS)HttpContext.Current.Session[CIMSClass.GlobalString.SessionAndCookieKeys.USER];
            BL007.SetOprLogUser(mUser.UserID, mUser.UserName);
        }

    }
    //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/22 end
    /// <summary>
    /// 確定按鈕處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/22 start
        SetLogData();
        //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/22 end
        if (this.IsCheck())//判断日结
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_IsCheck);
            return;
        }

        if (StringUtil.IsEmpty(this.txtNumber.Text))
        {
            ShowMessage("數量必須輸入。");
            return;
        }

        if (this.dropStatus.SelectedItem.Text.Trim() != "移轉" &&
            this.dropStatus.SelectedItem.Text.Trim() != "調整")
        {
            if (Decimal.Parse(this.txtNumber.Text.Trim().Replace(",","")) < 0)
            {
                ShowMessage("當前卡種狀況，數量不能為負數。");
                return;
            }
        }

        if (this.dropCard_Purpose_RID.SelectedValue.ToString() == "")
        {
            ShowMessage("用途沒有選擇");
        }
        if (this.dropCard_Group_RID.SelectedValue.ToString() == "")
        {
            ShowMessage("群組沒有選擇");
        }
        if (this.dropSpace_Short_RID.SelectedValue.ToString() == "")
        {
            ShowMessage("卡種沒有選擇");
        }
        if (this.dropStatus.SelectedValue.ToString() == "")
        {
            ShowMessage("卡種狀況沒有選擇");
        }

        DataTable dtFactory_Change_Import = (DataTable)Session["Factory_Change_Import"];
        DataTable dtFactory_Change_Import_Data = bl.GetFACTORY_CHANGE_IMPORT(txtBegin_Date.Text).Tables[0];
        int intRowIndex = Convert.ToInt32(this.hidRowIndex.Value.ToString());// 取保存過的行號

        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("Date_Time", Convert.ToDateTime(txtBegin_Date.Text));
        inputs.Add("Perso_Factory_RID", Convert.ToInt32(dropFactory.SelectedValue));
        inputs.Add("Is_Check", "N");
        inputs.Add("Status_RID", Convert.ToInt32(dropStatus.SelectedValue));
        inputs.Add("TYPE", this.dropSpace_Short_RID.SelectedItem.Text.Substring(0, 3));
        inputs.Add("AFFINITY", this.dropSpace_Short_RID.SelectedItem.Text.Substring(3, 4));
        inputs.Add("PHOTO", this.dropSpace_Short_RID.SelectedItem.Text.Substring(7, 2));
        inputs.Add("Number", Convert.ToInt32(this.txtNumber.Text.Replace(",", "")));
        inputs.Add("Space_Short_Name", this.dropSpace_Short_RID.SelectedItem.Text.Substring(9, this.dropSpace_Short_RID.SelectedItem.Text.Length - 9));
        inputs.Add("Check_Date", Convert.ToDateTime("1900-01-01"));
        inputs.Add("Is_Auto_Import", "N");

        bool blImportFactoryChangeStart = false;
        
        try
        {
            BL.Is_Check(Convert.ToDateTime(txtBegin_Date.Text));

            if (!bl.ImportFactoryChangeStart())
            {
                ShowMessage("廠商異動匯入批次已經啟動，不能重復開始！");
                return;
            }
            blImportFactoryChangeStart = true;

            // 新增加
            if (intRowIndex == -1)
            {
                // 檢查是否有重復的卡種及卡種狀況
                for (int intRow = 0; intRow < dtFactory_Change_Import_Data.Rows.Count; intRow++)
                {
                    if (Convert.ToDateTime(txtBegin_Date.Text) == Convert.ToDateTime(dtFactory_Change_Import_Data.Rows[intRow]["Date_Time"]) && dropFactory.SelectedValue.Trim() == dtFactory_Change_Import_Data.Rows[intRow]["Perso_Factory_RID"].ToString() && Convert.ToInt32(dtFactory_Change_Import_Data.Rows[intRow]["Status_RID"].ToString()) ==
                        Convert.ToInt32(this.dropStatus.SelectedValue.ToString()) &&
                        dtFactory_Change_Import_Data.Rows[intRow]["Space_Short_Name"].ToString() ==
                        this.dropSpace_Short_RID.SelectedItem.Text.Substring(9, this.dropSpace_Short_RID.SelectedItem.Text.Length - 9))
                    {
                        ShowMessage("該卡種相同的卡種狀況已經存在，不能重復輸入。");
                        bl.ImportFactoryChangeEnd();
                        return;
                    }
                }
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 start
                //bl.SaveFACTORY_CHANGE_IMPORT(inputs);
                BL007.SaveFACTORY_CHANGE_IMPORT(inputs);
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 end
            }
            // 修改
            else
            {
                string rid = Request.QueryString["rid"].ToString();
                // 檢查是否有重復的卡種及卡種狀況
                for (int intRow = 0; intRow < dtFactory_Change_Import_Data.Rows.Count; intRow++)
                {
                    if (rid.ToString().Trim() != dtFactory_Change_Import_Data.Rows[intRow]["RID"].ToString().Trim() &&
                        Convert.ToDateTime(txtBegin_Date.Text) == Convert.ToDateTime(dtFactory_Change_Import_Data.Rows[intRow]["Date_Time"]) && dropFactory.SelectedValue.Trim() == dtFactory_Change_Import_Data.Rows[intRow]["Perso_Factory_RID"].ToString() && Convert.ToInt32(dtFactory_Change_Import_Data.Rows[intRow]["Status_RID"].ToString()) ==
                        Convert.ToInt32(this.dropStatus.SelectedValue.ToString()) &&
                        dtFactory_Change_Import_Data.Rows[intRow]["Space_Short_Name"].ToString() ==
                        this.dropSpace_Short_RID.SelectedItem.Text.Substring(9, this.dropSpace_Short_RID.SelectedItem.Text.Length - 9))
                    {
                        ShowMessage("該卡種相同的卡種狀況已經存在，不能重復輸入。");
                        bl.ImportFactoryChangeEnd();
                        return;
                    }
                }
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 start
               // bl.Update(Convert.ToInt32(rid), inputs);
                BL007.Update(Convert.ToInt32(rid), inputs);
                //200908CR將替換后版面的廠商異動檔匯入修改成替換前版面與替換后版面同時匯入 ADD BY 楊昆 2009/09/21 end
            }

            bl.ImportFactoryChangeEnd();

            Response.Write("<script>returnValue='1';window.close();</script>");
        }
        catch (Exception ex)
        { 
            if (blImportFactoryChangeStart)
                bl.ImportFactoryChangeEnd();
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 用途變更時，重新綁定卡種群組和卡種
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropCard_Purpose_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        // 群組
        dropCard_Group_RIDBind(this.dropCard_Purpose_RID.SelectedValue.ToString());
        // 卡種
        dropSpace_Short_RIDBind(this.dropCard_Group_RID.SelectedValue.ToString());
    }

    /// <summary>
    /// 卡種群組變更時，重新綁定卡種
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropCard_Group_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        // 卡種
        dropSpace_Short_RIDBind(this.dropCard_Group_RID.SelectedValue.ToString());
    }
    #endregion
    #region 欄位/資料補充說明
    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_Purpose_RIDBind()
    {
        dropCard_Purpose_RID.DataTextField = "Param_Name";
        dropCard_Purpose_RID.DataValueField = "Param_Code";
        dropCard_Purpose_RID.DataSource = bl.GetPurpose();
        dropCard_Purpose_RID.DataBind();
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_Group_RIDBind(string strParam_Code)
    {
        DataTable dtCardTypeGroup = new DataTable();
        if (strParam_Code != "")
        {
            dtCardTypeGroup = bl.GetGroupByPurposeId(strParam_Code);

            dropCard_Group_RID.DataTextField = "Group_Name";
            dropCard_Group_RID.DataValueField = "RID";
            dropCard_Group_RID.DataSource = dtCardTypeGroup;
            dropCard_Group_RID.DataBind();
        }
    }

    /// <summary>
    /// 卡種綁定
    /// </summary>
    protected void dropSpace_Short_RIDBind(string strCardTypeGroupRID)
    {
        DataTable dtCardType = null;
        dtCardType = bl.GetCardTypeByGroupId(strCardTypeGroupRID);
        dropSpace_Short_RID.DataTextField = "Name";
        dropSpace_Short_RID.DataValueField = "RID";
        dropSpace_Short_RID.DataSource = dtCardType;
        dropSpace_Short_RID.DataBind();
    }

    /// <summary>
    /// 取Perso廠訊息，并綁定到Perso控件。
    /// </summary>
    private void GetFactory()
    {
        // 獲取 Perso廠商資料
        DataSet dstFactory = bl.GetFactoryList();
        dropFactory.DataValueField = "RID";
        dropFactory.DataTextField = "Factory_ShortName_CN";
        dropFactory.DataSource = dstFactory.Tables[0];
        dropFactory.DataBind();
    }
    #endregion
}
