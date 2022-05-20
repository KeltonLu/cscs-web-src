//******************************************************************
//*  作    者：FangBao
//*  功能說明：備援合約管理
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

public partial class BaseInfo_BaseInfo002Bak : PageBase
{
    BaseInfo002BL BL = new BaseInfo002BL();

    Dictionary<string, DataTable> dirAgreement = new Dictionary<string, DataTable>();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbCardPrice.NoneData = "";

        string strType = Request.QueryString["Type"];
        string strID = Request.QueryString["ID"];

        AjaxValidator1.QueryInfo = strID;

        if (!IsPostBack)
        {
            dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

            dropFactory_RID.DataSource = BL.GetFacotry();
            dropFactory_RID.DataBind();

            DataTable dbtlAgreementTmp = dirAgreement["AgreementTmp"];
            //if (dbtlAgreementTmp != null)
            //{
            //    if(!StringUtil.IsEmpty(dbtlAgreementTmp.Rows[0]["Card_Number"].ToString()))
            //    lbCardNum.Text = Convert.ToInt32(dbtlAgreementTmp.Rows[0]["Card_Number"].ToString()).ToString("N0");
            //    lbAgTimeFrom.Text = dbtlAgreementTmp.Rows[0]["Begin_Time"].ToString();
            //    lbAgTimeTo.Text = dbtlAgreementTmp.Rows[0]["End_Time"].ToString();
            //}


            if (StringUtil.IsEmpty(strType))
            {
                dirAgreement["AgreementBakTmp"].Clear();
                dirAgreement["GroupBakTmp"].Clear();
                dirAgreement["CardBakTmp"].Clear();
                dirAgreement["MaterialBakTmp"].Clear();
                dirAgreement["LevelBakTmp"].Clear();
            }


            if (!StringUtil.IsEmpty(strID))
            {
               
               

                ViewState["CardIDBak"] = strID;

                DataRow[] drowAgreementBaks = dirAgreement["AgreementBak"].Select("ID=" + strID);

                if (drowAgreementBaks.Length > 0)
                {
                    if (drowAgreementBaks[0]["CanEdit"].ToString() == "Y")
                    {
                        dropFactory_RID.Enabled = false;
                        //btnSubmitDn.Enabled = false;

                        //btnSubmitUp.Enabled = false;
                        //btnCardPriceAdd.Enabled = false;
                    }

                    if (!StringUtil.IsEmpty(drowAgreementBaks[0]["RID"].ToString()))
                        txtAgreement_Code.Enabled = false;

                    DataRow[] drowAgreementBak_Temp = dirAgreement["AgreementBakTmp"].Select("1=1");
                    if (drowAgreementBak_Temp.Length > 0)
                    {
                        drowAgreementBaks = drowAgreementBak_Temp;
                    }

                    SetControlsForDataRow(drowAgreementBaks[0]);

                   

                   

                    HyperLink.Text = drowAgreementBaks[0]["IMG_File_Name"].ToString();
                    HyperLink.NavigateUrl = drowAgreementBaks[0]["IMG_File_URL"].ToString();

                    if (StringUtil.IsEmpty(strType))
                    {
                        DataTable dtblAgreementBak = dirAgreement["AgreementBakTmp"];
                        DataTable dtblGroup = dirAgreement["GroupBakTmp"];
                        DataTable dtblCard = dirAgreement["CardBakTmp"];
                        DataTable dtblMaterial = dirAgreement["MaterialBakTmp"];
                        DataTable dtblLevel = dirAgreement["LevelBakTmp"];


                        DataRow[] drowGoups = dirAgreement["GroupBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());
                        DataRow[] drowCards = dirAgreement["CardBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());
                        DataRow[] drowMaterials = dirAgreement["MaterialBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());
                        DataRow[] drowLevels = dirAgreement["LevelBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());

                        foreach (DataRow drowGoup in drowGoups)
                        {
                            DataRow drow = dtblGroup.NewRow();
                            drow.ItemArray = drowGoup.ItemArray;
                            dtblGroup.Rows.Add(drow);
                        }

                        foreach (DataRow drowCard in drowCards)
                        {
                            DataRow drow = dtblCard.NewRow();
                            drow.ItemArray = drowCard.ItemArray;
                            dtblCard.Rows.Add(drow);
                        }

                        foreach (DataRow drowLevel in drowLevels)
                        {
                            DataRow drow = dtblLevel.NewRow();
                            drow.ItemArray = drowLevel.ItemArray;
                            dtblLevel.Rows.Add(drow);
                        }

                        foreach (DataRow drowMaterial in drowMaterials)
                        {
                            DataRow drow = dtblMaterial.NewRow();
                            drow.ItemArray = drowMaterial.ItemArray;
                            dtblMaterial.Rows.Add(drow);
                        }

                        foreach (DataRow drowAgreementBak in drowAgreementBaks)
                        {
                            DataRow drow = dtblAgreementBak.NewRow();
                            drow.ItemArray = drowAgreementBak.ItemArray;
                            dtblAgreementBak.Rows.Add(drow);
                        }
                    }
                }
                else
                {
                    drowAgreementBaks = dirAgreement["AgreementBakTmp"].Select("1=1");
                    if (drowAgreementBaks.Length > 0)
                    {
                        SetControlsForDataRow(drowAgreementBaks[0]);

                        HyperLink.Text = drowAgreementBaks[0]["IMG_File_Name"].ToString();
                        HyperLink.NavigateUrl = drowAgreementBaks[0]["IMG_File_URL"].ToString();
                    }
                }

                if (!StringUtil.IsEmpty(txtCard_Number.Text))
                {
                    txtCard_Number.Text = Convert.ToInt32(txtCard_Number.Text).ToString("N0");
                }
                
            }
            else
            {
                //備援合約
                int intID = 0;

                if (dirAgreement["AgreementBak"].Rows.Count == 0)
                    intID = 0;
                else
                    intID = int.Parse(dirAgreement["AgreementBak"].Compute("max(ID)", "").ToString()) + 1;

                ViewState["CardIDBak"] = intID.ToString();
            }
            gvpbCardPrice.BindData();
        }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
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

        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        string strSubmitType = "";

        DataRow[] drowAgreementBaks = dirAgreement["AgreementBak"].Select("ID=" + ViewState["CardIDBak"].ToString());
        if (drowAgreementBaks.Length > 0)
            strSubmitType = "Edit";
        else
            strSubmitType = "Add";

        string strType = Request.QueryString["Type"];
        string strID = Request.QueryString["ID"];

        DataTable dbtlAgreementBak = dirAgreement["AgreementBak"];

        DataRow drowAgreementBak = null;

        if (strSubmitType == "Add")
            drowAgreementBak = dbtlAgreementBak.NewRow();
        else
        {
            drowAgreementBak = dbtlAgreementBak.Select("ID=" + strID)[0];

            if (drowAgreementBak["Card_Number"].ToString() == "0")
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
        }

        drowAgreementBak["Agreement_Code"] = txtAgreement_Code.Text.Trim();
        drowAgreementBak["Agreement_Name"] = txtAgreement_Name.Text.Trim();
        drowAgreementBak["Factory_RID"] = dropFactory_RID.SelectedValue;
        drowAgreementBak["Factory_Name"] = dropFactory_RID.SelectedItem.Text;
        drowAgreementBak["Card_Number"] = txtCard_Number.Text.Replace(",", "");
        drowAgreementBak["Begin_Time"] = txtBegin_Time.Text;
        drowAgreementBak["End_Time"] = txtEnd_Time.Text;
        drowAgreementBak["ID"] = ViewState["CardIDBak"].ToString();
        drowAgreementBak["IMG_File_URL"] = imgIMG_File_URL.ImageUrl;
        drowAgreementBak["IMG_File_Name"] = HyperLink.Text;

        if (strSubmitType == "Add")
            dbtlAgreementBak.Rows.Add(drowAgreementBak);
        else//合約CHECK
        {
            if (!StringUtil.IsEmpty(drowAgreementBak["RID"].ToString()))
            {
                try
                {
                    BaseInfo002BL bl = new BaseInfo002BL();
                    bl.AgreementCheck(drowAgreementBak);
                }
                catch(Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
        }

        #region 卡種組合處理
        if (strSubmitType == "Add")
        {
            DataTable dtblGroup = dirAgreement["GroupBak"];
            DataTable dtblCard = dirAgreement["CardBak"];
            DataTable dtblLevel = dirAgreement["LevelBak"];
            DataTable dtblMaterial = dirAgreement["MaterialBak"];

            DataTable dtblGroup_0 = dirAgreement["GroupBakTmp"];
            DataTable dtblCard_0 = dirAgreement["CardBakTmp"];
            DataTable dtblLevel_0 = dirAgreement["LevelBakTmp"];
            DataTable dtblMaterial_0 = dirAgreement["MaterialBakTmp"];

            foreach (DataRow drowGroup_0 in dtblGroup_0.Rows)
            {
                DataRow drow = dtblGroup.NewRow();
                drow.ItemArray = drowGroup_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblGroup.Rows.Add(drow);
            }

            foreach (DataRow drowCard_0 in dtblCard_0.Rows)
            {
                DataRow drow = dtblCard.NewRow();
                drow.ItemArray = drowCard_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblCard.Rows.Add(drow);
            }

            foreach (DataRow drowLevel_0 in dtblLevel_0.Rows)
            {
                DataRow drow = dtblLevel.NewRow();
                drow.ItemArray = drowLevel_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblLevel.Rows.Add(drow);
            }

            foreach (DataRow drowMaterial_0 in dtblMaterial_0.Rows)
            {
                DataRow drow = dtblMaterial.NewRow();
                drow.ItemArray = drowMaterial_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblMaterial.Rows.Add(drow);
            }
        }
        else
        {
            DataTable dtblGroup = dirAgreement["GroupBak"];
            DataTable dtblCard = dirAgreement["CardBak"];
            DataTable dtblLevel = dirAgreement["LevelBak"];
            DataTable dtblMaterial = dirAgreement["MaterialBak"];

            DataTable dtblGroup_0 = dirAgreement["GroupBakTmp"];
            DataTable dtblCard_0 = dirAgreement["CardBakTmp"];
            DataTable dtblLevel_0 = dirAgreement["LevelBakTmp"];
            DataTable dtblMaterial_0 = dirAgreement["MaterialBakTmp"];

            DataRow[] drowGroups = dirAgreement["GroupBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());
            DataRow[] drowCards = dirAgreement["CardBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());
            DataRow[] drowLevels = dirAgreement["LevelBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());
            DataRow[] drowMaterials = dirAgreement["MaterialBak"].Select("IDBak=" + ViewState["CardIDBak"].ToString());

            foreach (DataRow drowGroup in drowGroups)
            {
                dtblGroup.Rows.Remove(drowGroup);
            }

            foreach (DataRow drowCard in drowCards)
            {
                dtblCard.Rows.Remove(drowCard);
            }

            foreach (DataRow drowLevel in drowLevels)
            {
                dtblLevel.Rows.Remove(drowLevel);
            }

            foreach (DataRow drowMaterial in drowMaterials)
            {
                dtblMaterial.Rows.Remove(drowMaterial);
            }

            foreach (DataRow drowGroup_0 in dtblGroup_0.Rows)
            {
                DataRow drow = dtblGroup.NewRow();
                drow.ItemArray = drowGroup_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblGroup.Rows.Add(drow);
            }
            foreach (DataRow drowCard_0 in dtblCard_0.Rows)
            {
                DataRow drow = dtblCard.NewRow();
                drow.ItemArray = drowCard_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblCard.Rows.Add(drow);
            }

            foreach (DataRow drowLevel_0 in dtblLevel_0.Rows)
            {
                DataRow drow = dtblLevel.NewRow();
                drow.ItemArray = drowLevel_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblLevel.Rows.Add(drow);
            }
            foreach (DataRow drowMaterial_0 in dtblMaterial_0.Rows)
            {
                DataRow drow = dtblMaterial.NewRow();
                drow.ItemArray = drowMaterial_0.ItemArray;
                drow["IDBak"] = ViewState["CardIDBak"].ToString();
                dtblMaterial.Rows.Add(drow);
            }
        }


        #endregion

        dirAgreement["AgreementBakTmp"].Clear();
        dirAgreement["GroupBakTmp"].Clear();
        dirAgreement["CardBakTmp"].Clear();
        dirAgreement["MaterialBakTmp"].Clear();
        dirAgreement["LevelBakTmp"].Clear();

        string strActionType = Request.QueryString["ActionType"];

        if (strActionType == "Add")
            Response.Redirect("BaseInfo002Add.aspx?type=1");
        else
            Response.Redirect("BaseInfo002Mod.aspx?type=1");
    }

    protected void btnCancelUp_Click(object sender, EventArgs e)
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        string strActionType = Request.QueryString["ActionType"];

        dirAgreement["AgreementBakTmp"].Clear();
        dirAgreement["GroupBakTmp"].Clear();
        dirAgreement["CardBakTmp"].Clear();
        dirAgreement["MaterialBakTmp"].Clear();
        dirAgreement["LevelBakTmp"].Clear();

        if (strActionType == "Add")
            Response.Redirect("BaseInfo002Add.aspx?type=1");
        else
            Response.Redirect("BaseInfo002Mod.aspx?type=1");
    }
    protected void btnCardPriceAdd_Click(object sender, EventArgs e)
    {
        SaveTempAgreementBak();

        string strActionType = Request.QueryString["ActionType"];

        Response.Redirect("BaseInfo002Card.aspx?ActionType=" + strActionType + "&Type=2&CardIDBak=" + ViewState["CardIDBak"].ToString());
    }

    #endregion

    #region 欄位/資料補充說明
    protected void AjaxValidator1_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];
        string strAgCode = "";
        if (dirAgreement["Agreement"].Rows.Count > 0)
            strAgCode = dirAgreement["Agreement"].Rows[0]["Agreement_Code"].ToString();

        if (BL.ContainsAgreement(e.QueryData,strAgCode))
        {
            e.IsAllowSubmit = false;
        }
        else
        {

            if (dirAgreement["AgreementTmp"].Rows.Count > 0)
            {
                if (dirAgreement["AgreementTmp"].Rows[0]["Agreement_Code"].ToString() == e.QueryData)
                {
                    e.IsAllowSubmit = false;
                    return;
                }
            }

            //備援合約
            DataRow[] drows = null;

            if (!StringUtil.IsEmpty(e.QueryInfo))
                drows = dirAgreement["AgreementBak"].Select("ID<>" + e.QueryInfo);
            else
                drows = dirAgreement["AgreementBak"].Select("1=1");

            foreach (DataRow drow in drows)
            {
                if (drow["Agreement_Code"].ToString() == e.QueryData.Trim())
                {
                    e.IsAllowSubmit = false;
                    return;
                }
            }

            e.IsAllowSubmit = true;
        }
    }

    /// <summary>
    /// 儲存臨時合約信息
    /// </summary>
    private void SaveTempAgreementBak()
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];


        DataTable dbtlAgreementBak = dirAgreement["AgreementBakTmp"];

        dbtlAgreementBak.Rows.Clear();

        DataRow drowAgreementBak = null;

        drowAgreementBak = dbtlAgreementBak.NewRow();

        drowAgreementBak["Agreement_Code"] = txtAgreement_Code.Text.Trim();
        drowAgreementBak["Agreement_Name"] = txtAgreement_Name.Text.Trim();
        drowAgreementBak["Factory_RID"] = dropFactory_RID.SelectedValue;
        drowAgreementBak["Factory_Name"] = dropFactory_RID.SelectedItem.Text;
        drowAgreementBak["Card_Number"] = txtCard_Number.Text.Trim().Replace(",", "");
        drowAgreementBak["Begin_Time"] = txtBegin_Time.Text.Trim();
        drowAgreementBak["End_Time"] = txtEnd_Time.Text.Trim();
        drowAgreementBak["IMG_File_URL"] = imgIMG_File_URL.ImageUrl;
        drowAgreementBak["ID"] = ViewState["CardIDBak"].ToString();
        drowAgreementBak["IMG_File_Name"] = HyperLink.Text;

        dbtlAgreementBak.Rows.Add(drowAgreementBak);

    }

    #endregion

    #region 列表資料綁定
    protected void gvpbCardPrice_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        DataTable dtbl = dirAgreement["GroupBakTmp"];
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

    /// <summary>
    /// 刪除卡種
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteCard_Command(object sender, CommandEventArgs e)
    {
        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        DataTable dtblGroup = dirAgreement["GroupBakTmp"];
        DataTable dtblCard = dirAgreement["CardBakTmp"];
        DataTable dtblLevel = dirAgreement["LevelBakTmp"];

        DataRow[] drowGroups = dtblGroup.Select("ID=" + e.CommandArgument.ToString());
        DataRow[] drowCards = dtblCard.Select("ID=" + e.CommandArgument.ToString());
        DataRow[] drowLevels = dtblLevel.Select("ID=" + e.CommandArgument.ToString());

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

        //刪除合約下級距
        foreach (DataRow drowLevel in drowLevels)
        {
            dtblLevel.Rows.Remove(drowLevel);
        }

        gvpbCardPrice.BindData();
    }

    /// <summary>
    /// 卡種明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnCardName_Click(object sender, CommandEventArgs e)
    {
        SaveTempAgreementBak();

        string strActionType = Request.QueryString["ActionType"];
        Response.Redirect("BaseInfo002Card.aspx?ActionType=" + strActionType + "&Type=2&ID=" + e.CommandArgument.ToString() + "&CardIDBak=" + ViewState["CardIDBak"].ToString());
    }

    #endregion
   
}
