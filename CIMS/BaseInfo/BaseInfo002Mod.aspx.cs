//******************************************************************
//*  作    者：FangBao
//*  功能說明：合約修改管理
//*  創建日期：2008-09-16
//*  修改日期：2008-09-16 12:00
//*  修改記錄：
//*            □2008-09-16
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
using System.Collections.Generic;

public partial class BaseInfo_BaseInfo002Mod : PageBase
{
    BaseInfo002BL BL = new BaseInfo002BL();

    Dictionary<string, DataTable> dirAgreement = new Dictionary<string, DataTable>();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbBak.NoneData = "";
        gvpbCardPrice.NoneData = "";

        if (!IsPostBack)
        {
            dropFactory_RID.DataSource = BL.GetFacotry();
            dropFactory_RID.DataBind();
            string strType = Request.QueryString["Type"];

            if (StringUtil.IsEmpty(strType))
            {
                string strRID=Request.QueryString["RID"];

                #region 資料結構

                DataTable dtblAgreement = new DataTable();
                dtblAgreement.Columns.Add("RID");
                dtblAgreement.Columns.Add("ID");
                dtblAgreement.Columns.Add("Agreement_Name");
                dtblAgreement.Columns.Add("IMG_File_URL");
                dtblAgreement.Columns.Add("Factory_RID");
                dtblAgreement.Columns.Add("Factory_Name");
                dtblAgreement.Columns.Add("Card_Number");
                dtblAgreement.Columns.Add("Begin_Time");
                dtblAgreement.Columns.Add("End_Time");
                dtblAgreement.Columns.Add("Agreement_Code");
                dtblAgreement.Columns.Add("Agreement_Code_Main");
                dtblAgreement.Columns.Add("Reason");
                dtblAgreement.Columns.Add("CanEdit");
                dtblAgreement.Columns.Add("IMG_File_Name");

                DataTable dtblAgreementBak = dtblAgreement.Clone();
                DataTable dtblAgreementTmp = dtblAgreement.Clone();
                DataTable dtblAgreementBakTmp = dtblAgreement.Clone();

                DataTable dtblGroup = new DataTable();
                dtblGroup.Columns.Add("ID");
                dtblGroup.Columns.Add("IDBak");
                dtblGroup.Columns.Add("Agreement_Main_RID");
                dtblGroup.Columns.Add("Group_Name");
                dtblGroup.Columns.Add("Type");
                dtblGroup.Columns.Add("Base_Price");
                dtblGroup.Columns.Add("CanEdit");

                DataTable dtblGroupBak = dtblGroup.Clone();
                DataTable dtblGroupBakTmp = dtblGroup.Clone();

                DataTable dtblCard = new DataTable();
                dtblCard.Columns.Add("ID");
                dtblCard.Columns.Add("IDBak");
                dtblCard.Columns.Add("Agreement_Group_RID");
                dtblCard.Columns.Add("CardType_RID");
                dtblCard.Columns.Add("CardType_NAME");
                dtblCard.Columns.Add("Param_RID");
                dtblCard.Columns.Add("CanEdit");

                DataTable dtblCardBak = dtblCard.Clone();
                DataTable dtblCardBakTmp = dtblCard.Clone();

                DataTable dtblMaterial = new DataTable();
                dtblMaterial.Columns.Add("ID");
                dtblMaterial.Columns.Add("IDBak");
                dtblMaterial.Columns.Add("Agreement_Group_RID");
                dtblMaterial.Columns.Add("Material_RID");
                dtblMaterial.Columns.Add("Material_Name");
                dtblMaterial.Columns.Add("Base_Price");
                dtblMaterial.Columns.Add("CanEdit");
                DataTable dtblMaterialBak = dtblMaterial.Clone();
                DataTable dtblMaterialBakTmp = dtblMaterial.Clone();
                DataTable dtblMaterialTmp = dtblMaterial.Clone();


                DataTable dtblLevel = new DataTable();
                dtblLevel.Columns.Add("ID");
                dtblLevel.Columns.Add("IDBak");
                dtblLevel.Columns.Add("CardType_RID");
                dtblLevel.Columns.Add("Param_RID");
                dtblLevel.Columns.Add("Param_Name");
                dtblLevel.Columns.Add("Price");
                dtblLevel.Columns.Add("Level_Min");
                dtblLevel.Columns.Add("Level_Max");
                dtblLevel.Columns.Add("CanEdit");
                DataTable dtblLevelBak = dtblLevel.Clone();
                DataTable dtblLevelTmp = dtblLevel.Clone();
                DataTable dtblLevelBakTmp = dtblLevel.Clone();


                #endregion

                dirAgreement.Add("Agreement", dtblAgreement);               //主合約
                dirAgreement.Add("AgreementBak", dtblAgreementBak);         //備援合約
                dirAgreement.Add("Group", dtblGroup);                       //主群組
                dirAgreement.Add("Card", dtblCard);                         //主群組卡種
                dirAgreement.Add("Material", dtblMaterial);                 //主材質
                dirAgreement.Add("GroupBak", dtblGroupBak);                 //備援主群組
                dirAgreement.Add("CardBak", dtblCardBak);                   //備援群組卡種
                dirAgreement.Add("MaterialBak", dtblMaterialBak);           //備援材質

                dirAgreement.Add("MaterialTmp", dtblMaterialTmp);           //主臨時材質
                dirAgreement.Add("AgreementTmp", dtblAgreementTmp);         //主合約臨時

                dirAgreement.Add("AgreementBakTmp", dtblAgreementBakTmp);   //備援臨時合約
                dirAgreement.Add("GroupBakTmp", dtblGroupBakTmp);           //備援臨時主群組
                dirAgreement.Add("CardBakTmp", dtblCardBakTmp);             //備援臨時群組卡種
                dirAgreement.Add("MaterialBakTmp", dtblMaterialBakTmp);     //備援臨時材質

                dirAgreement.Add("Level", dtblLevel);                       //主合約級距
                dirAgreement.Add("LevelBak", dtblLevelBak);                 //備援級距
                dirAgreement.Add("LevelTmp", dtblLevelTmp);                 //臨時級距
                dirAgreement.Add("LevelBakTmp", dtblLevelBakTmp);           //備援臨時級距

                Session["Agreement"] = BL.LoadAgreementEditInfo(dirAgreement, strRID);
            }

            dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

            if (dirAgreement["Agreement"].Rows[0]["CanEdit"].ToString() == "Y")
            {
                //btnCardPriceAdd.Enabled = false;
                dropFactory_RID.Enabled = false;
                //txtBegin_Time.Enabled = false;
                //txtEnd_Time.Enabled = false;
                //IMG1.Visible = false;
                //IMG2.Visible = false;
            }

            SetControlsForDataRow(dirAgreement["AgreementTmp"].Rows[0]);

            HyperLink.Text = dirAgreement["AgreementTmp"].Rows[0]["IMG_File_Name"].ToString();
            HyperLink.NavigateUrl = dirAgreement["AgreementTmp"].Rows[0]["IMG_File_URL"].ToString();

            if (!StringUtil.IsEmpty(txtCard_Number.Text))
            {
                txtCard_Number.Text = Convert.ToInt32(txtCard_Number.Text).ToString("N0");
            }

            if(dirAgreement["AgreementBak"].Rows.Count==0)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(true);", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(false);", true);

            gvpbBak.BindData();
            gvpbCardPrice.BindData();
        }

    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        if (dirAgreement["AgreementBak"].Rows.Count == 0)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(true);", true);
        else
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(false);", true);

        if (base.FileUpload(fludFileUpload.PostedFile, imgIMG_File_URL))
        {

            string[] str = fludFileUpload.PostedFile.FileName.Split('\\');

            if (str.Length > 0)
            {
                HyperLink.Text = str[str.Length - 1];
                HyperLink.NavigateUrl = imgIMG_File_URL.ImageUrl;
            }
        }
    }
    protected void btnSubmitUp_Click(object sender, EventArgs e)
    {
        //DataTable dtblAgreementUsedTime = BL.GetAgreementUserdTime(



        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        if (dirAgreement["AgreementBak"].Rows.Count == 0)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(true);", true);
        else
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(false);", true);


        if (dirAgreement["Agreement"].Rows[0]["Card_Number"].ToString() == "0")
        {
            if (!StringUtil.IsEmpty(txtCard_Number.Text))
            {
                if (txtCard_Number.Text != "0")
                {
                    ShowMessage("卡數初始化為0，不能修改為其它值");
                    return;
                }
            }
        }
        else
        {
            if (StringUtil.IsEmpty(txtCard_Number.Text))
            {
                ShowMessage("卡數不能為空");
                return;
            }
            if (txtCard_Number.Text == "0")
            {
                ShowMessage("卡數初始化大於0，修改必須大於0");
                return;
            }
        }

        if (chkDel.Checked)
        {
            try
            {
                foreach (DataRow drowAgreement1 in dirAgreement["Agreement"].Rows)
                {
                    BL.ChkDelAgreement(drowAgreement1["Agreement_Code"].ToString());
                }

                //備援合約
                foreach (DataRow drowAgreementBak in dirAgreement["AgreementBak"].Rows)
                {
                    BL.ChkDelAgreement(drowAgreementBak["Agreement_Code"].ToString());
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                return;
            }
        }

        if (StringUtil.GetByteLength(txtReason.Text) > 1000)
        {
            ShowMessage("異動原因不能超過1000個字符");
            return;
        }

        if (StringUtil.IsEmpty(dropFactory_RID.SelectedValue))
        {
            ShowMessage("請選擇空白卡廠！");
            return;
        }

        if (imgIMG_File_URL.ImageUrl == "../images/NoPic.jpg")
        {
            ShowMessage("請上傳合約影像檔！");
            return;
        }

        DataTable dtblAgreement = dirAgreement["Agreement"];

        dtblAgreement.Rows.Clear();

        DataRow drowAgreement = dtblAgreement.NewRow();

        drowAgreement["Agreement_Code"] = txtAgreement_Code.Text.Trim();
        drowAgreement["Agreement_Name"] = txtAgreement_Name.Text.Trim();
        drowAgreement["Factory_RID"] = dropFactory_RID.SelectedValue;
        drowAgreement["Card_Number"] = txtCard_Number.Text.Trim().Replace(",", "");
        drowAgreement["Begin_Time"] = txtBegin_Time.Text.Trim();
        drowAgreement["End_Time"] = txtEnd_Time.Text.Trim();
        drowAgreement["IMG_File_URL"] = imgIMG_File_URL.ImageUrl;
        drowAgreement["Reason"] = txtReason.Text;
        drowAgreement["IMG_File_Name"] = HyperLink.Text;

        dtblAgreement.Rows.Add(drowAgreement);

        try
        {
            if (chkDel.Checked)
            {
                BL.Delete(txtAgreement_Code.Text);
                ShowMessageAndGoPage("刪除成功", "BaseInfo002.aspx?Con=1");
            }
            else
            {
                BL.Update(dirAgreement);
                ShowMessageAndGoPage("儲存成功", "BaseInfo002.aspx?Con=1");
            }
            Session.Remove("Agreement");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void btnCardPriceAdd_Click(object sender, EventArgs e)
    {
        SaveTempAgreement();
        Response.Redirect("BaseInfo002Card.aspx?ActionType=Edit&Type=1");
    }
    protected void btnBakAdd_Click(object sender, EventArgs e)
    {
        SaveTempAgreement();
        Response.Redirect("BaseInfo002Bak.aspx?ActionType=Edit");
    }
    #endregion


    #region 列表資料綁定
    protected void gvpbCardPrice_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtbl = ((Dictionary<string, DataTable>)Session["Agreement"])["Group"];
        e.Table = dtbl;
        e.RowCount = dtbl.Rows.Count;
    }

    protected void gvpbCardPrice_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbCardPrice.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtbl.Rows.Count == 0)
                return;

            ImageButton ibtnButton = null;
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDeleteCard");
            ibtnButton.CommandArgument = dtbl.Rows[e.Row.RowIndex]["ID"].ToString();
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            LinkButton lbtnCardName = null;
            lbtnCardName = (LinkButton)e.Row.FindControl("lbtnCardName");
            lbtnCardName.CommandArgument = dtbl.Rows[e.Row.RowIndex]["ID"].ToString();
            lbtnCardName.Text = dtbl.Rows[e.Row.RowIndex]["Group_Name"].ToString();

            e.Row.Cells[1].Text = e.Row.Cells[1].Text.Replace("1", "基本").Replace("2", "級距");
        }
    }

    protected void gvpbBak_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtbl = ((Dictionary<string, DataTable>)Session["Agreement"])["AgreementBak"];
        e.Table = dtbl;
        e.RowCount = dtbl.Rows.Count;
    }

    protected void gvpbBak_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbBak.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtbl.Rows.Count == 0)
                return;

            ImageButton ibtnButton = null;
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDeleteBak");
            ibtnButton.CommandArgument = dtbl.Rows[e.Row.RowIndex]["ID"].ToString();
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            LinkButton lbAgreement_Code = null;
            lbAgreement_Code = (LinkButton)e.Row.FindControl("lbAgreement_Code");
            lbAgreement_Code.CommandArgument = dtbl.Rows[e.Row.RowIndex]["ID"].ToString();
            lbAgreement_Code.Text = dtbl.Rows[e.Row.RowIndex]["Agreement_Code"].ToString();
        }
    }

    /// <summary>
    /// 刪除卡種
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteCard_Command(object sender, CommandEventArgs e)
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        //if (dirAgreement["Agreement"].Rows[0]["CanEdit"].ToString() == "Y")
        //{
        //    ShowMessage("該合約已被使用，無法刪除相關信息");
        //    return;
        //}


        DataTable dtblGroup = dirAgreement["Group"];

        DataTable dtblCard = dirAgreement["Card"];
        DataTable dtblMaterial = dirAgreement["Material"];

        DataRow[] drowGroups = dtblGroup.Select("ID=" + e.CommandArgument.ToString());
        DataRow[] drowCards = dtblCard.Select("ID=" + e.CommandArgument.ToString());
        DataRow[] drowMaterials = dtblMaterial.Select("ID=" + e.CommandArgument.ToString());

        //合約卡種組合
        foreach (DataRow drowGroup in drowGroups)
        {
            dtblGroup.Rows.Remove(drowGroup);
        }

        //刪除合約下卡種
        foreach (DataRow drowCard in drowCards)
        {
            dtblCard.Rows.Remove(drowCard);
        }

        //刪除合約下材質
        foreach (DataRow drowMaterial in drowMaterials)
        {
            dtblMaterial.Rows.Remove(drowMaterial);
        }

       

        gvpbCardPrice.BindData();
    }

    /// <summary>
    /// 刪除備援
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteBak_Command(object sender, CommandEventArgs e)
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        DataTable dtblAgreementBak = dirAgreement["AgreementBak"];
        DataTable dtblGroupBak = dirAgreement["GroupBak"];
        DataTable dtblCardBak = dirAgreement["CardBak"];
        DataTable dtblMaterialBak = dirAgreement["MaterialBak"];

        DataRow[] drowAgreementBaks = dtblAgreementBak.Select("ID=" + e.CommandArgument.ToString());
        DataRow[] drowGroupBaks = dtblGroupBak.Select("IDBak=" + e.CommandArgument.ToString());
        DataRow[] drowCardBaks = dtblCardBak.Select("IDBak=" + e.CommandArgument.ToString());
        DataRow[] drowMaterialBaks = dtblMaterialBak.Select("IDBak=" + e.CommandArgument.ToString());



        //備援合約
        foreach (DataRow drowAgreementBak in drowAgreementBaks)
        {
            try
            {
                BL.ChkDelAgreement(drowAgreementBak["Agreement_Code"].ToString());
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                return;
            }
            dtblAgreementBak.Rows.Remove(drowAgreementBak);
        }

        //合約卡種組合
        foreach (DataRow drowGroupBak in drowGroupBaks)
        {
            dtblGroupBak.Rows.Remove(drowGroupBak);
        }

        //刪除合約下卡種
        foreach (DataRow drowCardBak in drowCardBaks)
        {
            dtblCardBak.Rows.Remove(drowCardBak);
        }

        //刪除合約下材質
        foreach (DataRow drowMaterialBak in drowMaterialBaks)
        {
            dtblMaterialBak.Rows.Remove(drowMaterialBak);
        }

        if (dirAgreement["AgreementBak"].Rows.Count == 0)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(true);", true);
        else
            ScriptManager.RegisterStartupScript(this, this.GetType(), "EnableDel", "EnableDel(false);", true);

        gvpbBak.BindData();
    }

    /// <summary>
    /// 卡種明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnCardName_Click(object sender, CommandEventArgs e)
    {
        SaveTempAgreement();
        Response.Redirect("BaseInfo002Card.aspx?ActionType=Edit&Type=1&ID=" + e.CommandArgument.ToString());
    }

    /// <summary>
    /// 備援明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbAgreement_Code_Click(object sender, CommandEventArgs e)
    {
        SaveTempAgreement();
        Response.Redirect("BaseInfo002Bak.aspx?ActionType=Edit&ID=" + e.CommandArgument.ToString());
    }

    #endregion

    #region 欄位/資料補充說明
    /// <summary>
    /// 儲存臨時合約信息
    /// </summary>
    private void SaveTempAgreement()
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        DataTable dbtlAgreementTmp = dirAgreement["AgreementTmp"];

        dbtlAgreementTmp.Rows.Clear();

        DataRow drowAgreementTmp = null;

        drowAgreementTmp = dbtlAgreementTmp.NewRow();

        drowAgreementTmp["Agreement_Code"] = txtAgreement_Code.Text.Trim();
        drowAgreementTmp["Agreement_Name"] = txtAgreement_Name.Text.Trim();
        drowAgreementTmp["Factory_RID"] = dropFactory_RID.SelectedValue;
        drowAgreementTmp["Card_Number"] = txtCard_Number.Text.Trim().Replace(",", "");
        drowAgreementTmp["Begin_Time"] = txtBegin_Time.Text.Trim();
        drowAgreementTmp["End_Time"] = txtEnd_Time.Text.Trim();
        drowAgreementTmp["IMG_File_URL"] = imgIMG_File_URL.ImageUrl;
        drowAgreementTmp["Reason"] = txtReason.Text;
        drowAgreementTmp["IMG_File_Name"] = HyperLink.Text;

        dbtlAgreementTmp.Rows.Add(drowAgreementTmp);
    }
    #endregion

   
}
