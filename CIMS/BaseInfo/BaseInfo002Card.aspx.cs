//******************************************************************
//*  作    者：FangBao
//*  功能說明：合約卡片管理
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
using System.Text;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;

public partial class BaseInfo_BaseInfo002Card : PageBase
{
    
    Dictionary<string, DataTable> dirAgreement = new Dictionary<string, DataTable>();
    BaseInfo006BL bizLogic = new BaseInfo006BL();
    BaseInfo002BL BL = new BaseInfo002BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        string strType = Request.QueryString["Type"];
        string strID = Request.QueryString["ID"];
        string strActionType = Request.QueryString["ActionType"];
        string strCardIDBak = Request.QueryString["CardIDBak"];

        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        AjaxValidator1.QueryInfo = strType + "," + strID;

        //無資料時顯示
        gvpbMaterial.NoneData = "";


        if (!IsPostBack)
        {
            //卡別資料
            DataTable dtblDropCardType = BL.CardType();
            dtblDropCardType.PrimaryKey = new DataColumn[] { dtblDropCardType.Columns["RID"] };
            ViewState["DropCardType"] = dtblDropCardType;

            //臨時材質
            DataTable dtblMaterialTmp=dirAgreement["MaterialTmp"];
            dtblMaterialTmp.Clear();

            //GirdView卡別
            DataTable dtblCardType = new DataTable();
            dtblCardType.Columns.Add("Name");
            dtblCardType.Columns.Add("RID");
            dtblCardType.Columns.Add("Param_RID");
            dtblCardType.Columns.Add("Param_Name");
            dtblCardType.PrimaryKey = new DataColumn[] { dtblCardType.Columns["RID"] };
            ViewState["CardType"] = dtblCardType;

            //GridView級據
            DataTable dtblLevel = new DataTable();
            dtblLevel.Columns.Add("級距");
            dirAgreement["LevelTmp"] = dtblLevel;


            //GridView級距
            #region 級距
            int intRowCount = 0;
            Dictionary<string, object> inputs = new Dictionary<string, object>();
            inputs.Add("txtPARAM_NAME", "");
            inputs.Add("ParamType_Code", GlobalString.ParameterType.CardType);

            DataSet dstlParam = null;
            try
            {
                dstlParam = bizLogic.list(inputs, "1", "2000", "ParamType_Code", "ASC", out intRowCount);
                ViewState["test"] = dstlParam.Tables[0];

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                return;
            }

            DataTable dtJJ = new DataTable();


            dtJJ.Columns.Add("txtjj_1", typeof(int));
            dtJJ.Columns.Add("txtjj_2", typeof(int));


            foreach (DataRow drow1 in ((DataTable)ViewState["test"]).Rows)
            {
                dtJJ.Columns.Add("Txt" + drow1["RID"].ToString());
            }

            ViewState["testtb"] = dtJJ;
            #endregion


            //已經選擇的卡種
            string strCardsSel = "";

            //合約卡種組合
            DataRow drowGroup = null;
            //合約卡種
            DataRow[] drowCards = null;
            //合約卡種
            DataRow[] drowLevels = null;
            //合約卡種組合
            DataTable dtblGroup = null;

            if (!StringUtil.IsEmpty(strID))
            {
                if (strType == "1")
                {

                    drowGroup = dirAgreement["Group"].Select("ID=" + strID)[0];
                    drowCards = dirAgreement["Card"].Select("ID=" + strID);
                    

                    foreach (DataRow drowMaterial in dirAgreement["Material"].Select("ID=" + strID))
                    {
                        DataRow drowMaterialTmp = dtblMaterialTmp.NewRow();
                        drowMaterialTmp.ItemArray = drowMaterial.ItemArray;
                        dtblMaterialTmp.Rows.Add(drowMaterialTmp);
                    }
                }
                else if (strType == "2")
                {
                    //合約卡種組合
                    drowGroup = dirAgreement["GroupBakTmp"].Select("ID=" + strID)[0];
                    //合約卡種
                    drowCards = dirAgreement["CardBakTmp"].Select("ID=" + strID);

                    foreach (DataRow drowMaterial in dirAgreement["MaterialBakTmp"].Select("ID=" + strID))
                    {
                        DataRow drowMaterialTmp = dtblMaterialTmp.NewRow();
                        drowMaterialTmp.ItemArray = drowMaterial.ItemArray;
                        dtblMaterialTmp.Rows.Add(drowMaterialTmp);
                    }
                }

                gvpbMaterial.BindData();

                SetControlsForDataRow(drowGroup);
                try
                {
                    txtBase_Price.Text = Convert.ToDecimal(txtBase_Price.Text).ToString("N2");
                }
                catch
                {
                    txtBase_Price.Text = "0";
                }

                DataTable dtbl = new DataTable();
                dtbl.Columns.Add("RID");
                dtbl.Columns.Add("NAME");

                foreach (DataRow drowCard in drowCards)
                {
                    DataRow drow = dtbl.NewRow();
                    drow["RID"] = drowCard["CardType_RID"];
                    drow["NAME"] = drowCard["CardType_NAME"];
                    dtbl.Rows.Add(drow);

                    strCardsSel += "'" + drowCard["CardType_RID"] + "',";

                    DataRow drowCardtype = dtblCardType.NewRow();
                    drowCardtype["RID"] = drowCard["CardType_RID"];
                    drowCardtype["Name"] = drowCard["CardType_NAME"];
                    drowCardtype["Param_RID"] = drowCard["Param_RID"];
                    if (dtblDropCardType.Rows.Count > 0)
                    {
                        if(StringUtil.IsEmpty(drowCard["Param_RID"].ToString()))
                            drowCardtype["Param_RID"] = dtblDropCardType.Rows[0][0].ToString();
                        else if(drowCard["Param_RID"].ToString()=="0")
                            drowCardtype["Param_RID"] = dtblDropCardType.Rows[0][0].ToString();

                        drowCardtype["Param_Name"] = dtblDropCardType.Rows.Find(drowCardtype["Param_RID"].ToString())[1].ToString();
                    }
                    dtblCardType.Rows.Add(drowCardtype);
                }

                ViewState["CardType"] = dtblCardType;
                gvpbCardTypeBind();

                if (radlType.SelectedValue == "2")
                {
                    DataTable dtblLevel_Main = null;

                    if (strType == "1")
                    {
                        dtblLevel_Main = dirAgreement["Level"];
                    }
                    else if (strType == "2")
                    {
                        dtblLevel_Main = dirAgreement["LevelBakTmp"];
                    }

                    ArrayList al = new ArrayList();

                    foreach (DataRow drowLevel1 in dtblLevel_Main.Select("ID=" + strID))
                    {
                        string strMax = "";
                        string strMin = "";

                        strMax = drowLevel1["Level_Max"].ToString();
                        strMin = drowLevel1["Level_Min"].ToString();

                        if (al.Contains(strMin + "|" + strMax))
                        {
                            continue;
                        }
                        else
                        {
                            al.Add(strMin + "|" + strMax);
                        }
                    }

                    for (int i = 0; i < al.Count; i++)
                    {
                        string strMax = "";
                        string strMin = "";

                        strMax = al[i].ToString().Split('|')[1];
                        strMin = al[i].ToString().Split('|')[0];

                        drowLevels = dtblLevel_Main.Select("ID=" + strID + " and Level_Min='" + strMin + "' and Level_Max='" + strMax+"'");

                        foreach (DataRow drowLevel1 in drowLevels)
                        {
                            if (!dtblLevel.Columns.Contains(drowLevel1["Param_Name"].ToString()))
                                dtblLevel.Columns.Add(drowLevel1["Param_Name"].ToString());
                        }

                        if (drowLevels.Length > 0)
                        {
                            DataRow drowJJ = dtJJ.NewRow();

                            drowJJ["txtjj_1"] = strMin;
                            drowJJ["txtjj_2"] = strMax;

                            foreach (DataRow drowLevel1 in drowLevels)
                            {
                                drowJJ["txt" + drowLevel1["Param_RID"].ToString()] = drowLevel1["Price"].ToString();
                            }

                            dtJJ.Rows.Add(drowJJ);
                        }
                    }

                    ViewState["testtb"] = dtJJ;

                    if (dtJJ.Rows.Count > 0)
                        gvpbLevelBind();
                }

                

                if (dtbl.Rows.Count > 0)
                    UctrlCardType1.SetRightItem = dtbl;
            }

            //已經選擇的卡種
            ViewState["strCardsSel"] = strCardsSel;

            //合約群組ID
            if (strType == "1")
                dtblGroup = dirAgreement["Group"];
            else if (strType == "2")
            {
                dtblGroup = dirAgreement["GroupBakTmp"];
            }

            int intID = 0;
            if (StringUtil.IsEmpty(strID))
            {

                if (dtblGroup.Rows.Count == 0)
                    intID = 0;
                else
                    intID = int.Parse(dtblGroup.Compute("max(ID)", "").ToString()) + 1;
            }
            else
                intID = int.Parse(strID);

            ViewState["GroupID"]=intID;


            if (radlType.SelectedValue == "1")
            {
                trLevel.Visible = false;
                btnAddLevel.Visible = false;
            }
            else
            {
                trLevel.Visible = true;
                btnAddLevel.Visible = true;
            }

            gvpbCardTypeBind();
            gvpbMaterial.BindData();
           
        }

        //btnMaterialAdd.Attributes.Add("onclick", "var aa=window.showModalDialog('BaseInfo002Materiel.aspx?ActionType=" + strActionType +"','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){AddMaterial();}");

        #region 合約卡種過濾
        DataRow[] drowCardsSplit = null;

        if (!StringUtil.IsEmpty(ViewState["strCardsSel"].ToString()))
        {
            if (strType == "1")
                drowCardsSplit = dirAgreement["Card"].Select("CardType_RID not in (" + ViewState["strCardsSel"].ToString().Substring(0, ViewState["strCardsSel"].ToString().Length - 1) + ")");
            else if (strType == "2")
            {
                drowCardsSplit = dirAgreement["CardBakTmp"].Select("CardType_RID not in (" + ViewState["strCardsSel"].ToString().Substring(0, ViewState["strCardsSel"].ToString().Length - 1) + ")");
            }
        }
        else
        {
            if (strType == "1")
                drowCardsSplit = dirAgreement["Card"].Select("1=1");
            else if (strType == "2")
            {
                drowCardsSplit = dirAgreement["CardBakTmp"].Select("1=1");
            }
        }

        StringBuilder stbCommand = new StringBuilder("select RID,Display_Name from dbo.CARD_TYPE where 1>0 and RST='A' ");

        string strCon = "";

        for (int i = 0; i < drowCardsSplit.Length; i++)
        {
            strCon += drowCardsSplit[i]["CardType_RID"] + ",";
        }

        if (!StringUtil.IsEmpty(strCon))
            stbCommand.Append(" and RID NOT IN (" + strCon.Substring(0, strCon.Length - 1) + ")");

        UctrlCardType1.SetLeftItem = stbCommand.ToString();
        #endregion

        UctrlCardType1.AutoPostBack = true;
        UctrlCardType1.Is_Using = true;
        UctrlCardType1.RightChange+=new CommUserCtrl_uctrlCardType.RightItemChange(UctrlCardType1_RightChange);

        #region 級距綁定
        gvpbLevel.Columns.Clear();

        TemplateField customField2 = new TemplateField();
        customField2.ShowHeader = true;
        customField2.HeaderTemplate = new GridViewTemplateTextBox("Head", "級距");
        customField2.ItemTemplate = new GridViewTemplateTextBox("Txt1", "txtjj");
        gvpbLevel.Columns.Add(customField2);

        foreach (DataRow drow in ((DataTable)ViewState["test"]).Rows)
        {
            TemplateField customField = new TemplateField();
            customField.ShowHeader = true;
            customField.HeaderTemplate = new GridViewTemplateTextBox("Head", drow["param_name"].ToString());
            customField.ItemTemplate = new GridViewTemplateTextBox("Txt", "txt" + drow["RID"].ToString());
            gvpbLevel.Columns.Add(customField);
        }

        TemplateField customField1 = new TemplateField();
        customField1.ShowHeader = true;
        customField1.HeaderTemplate = new GridViewTemplateTextBox("Head", "刪除");
        customField1.ItemTemplate = new GridViewTemplateTextBox("Btn", "btnDeljj");
        gvpbLevel.Columns.Add(customField1);

        gvpbLevelBind();
        
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMaterialAdd_Click(object sender,EventArgs e)
    {
        string strActionType = Request.QueryString["ActionType"];

        string strUrl = "var aa=window.showModalDialog('BaseInfo002Materiel.aspx?ActionType=" + strActionType + "','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){AddMaterial();}";

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), strUrl, true);
    }
    protected void btnSubmitUp_Click(object sender, EventArgs e)
    {
        if (UctrlCardType1.GetRightItem.Rows.Count == 0)
        {
            ShowMessage("請選擇一卡種");
            return;
        }

        if (radlType.SelectedValue == "1")
        {
            if (StringUtil.IsEmpty(txtBase_Price.Text))
            {
                ShowMessage("基本價格不能為空!");
                return;
            }
            if (txtBase_Price.Text == "0")
            {
                ShowMessage("基本價格不能為0!");
                return;
            }
        }
        else
        {
            if (StringUtil.IsEmpty(txtBase_Price.Text))
            {
                txtBase_Price.Text = "0";
            }
        }


        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

        string strType = Request.QueryString["Type"];
        string strID = Request.QueryString["ID"];
        string strCardIDBak = Request.QueryString["CardIDBak"];

        //合約卡種組合
        DataTable dtblGroup = null;
        //合約卡種
        DataTable dtblCard = null;
        //合約材質
        DataTable dtblMaterial = null;
        //臨時材質
        DataTable dtblMaterialTmp = null;
        //級距
        DataTable dtblLevel = null;
        //臨時級距
        DataTable dtblLevelTmp = null;

        dtblLevelTmp = dirAgreement["LevelTmp"];

        

        if (radlType.SelectedValue == "2")
        {
            DataTable dtblJJ1 = (DataTable)ViewState["testtb"];

            if (dtblJJ1.Rows.Count == 0)
            {
                ShowMessage("請添加一個級距");
                return;
            }

            gvpbLevelTempBind();

            //檢查級距是否全部輸入
            if (!CheckJJSR())
                return;

            //檢查級距區間的重複性
            if (!CheckJJCF())
                return;

            //檢查級距區間是否連續
            if (!CheckJJLX())
                return;

            
        }
        else
        {
            dirAgreement["LevelTmp"].Rows.Clear();
        }

        DataTable dtblJJ = (DataTable)ViewState["testtb"];

        if (strType == "1")
        {
            //合約卡種組合
            dtblGroup = dirAgreement["Group"];
            //合約卡種
            dtblCard = dirAgreement["Card"];
            //合約材質
            dtblMaterial = dirAgreement["Material"];

            dtblLevel = dirAgreement["Level"];
        }
        else if (strType == "2")
        {
            //合約卡種組合
            dtblGroup = dirAgreement["GroupBakTmp"];
            //合約卡種
            dtblCard = dirAgreement["CardBakTmp"];
            //合約材質
            dtblMaterial = dirAgreement["MaterialBakTmp"];

            dtblLevel = dirAgreement["LevelBakTmp"];
        }

        DataRow drowGroup = null;

        if (StringUtil.IsEmpty(strID))
            drowGroup = dtblGroup.NewRow();
        else
            drowGroup = dtblGroup.Select("ID=" + strID)[0];

        drowGroup["Group_Name"] = txtGroup_Name.Text.Trim();
        drowGroup["ID"] = ViewState["GroupID"].ToString();
        drowGroup["Type"] = radlType.SelectedValue;
        drowGroup["Base_Price"] = txtBase_Price.Text.Replace(",", "");
        if (!StringUtil.IsEmpty(strCardIDBak))
            drowGroup["IDBak"] = strCardIDBak;

        if (StringUtil.IsEmpty(strID))
            dtblGroup.Rows.Add(drowGroup);


        //刪除已有卡種
        if (!StringUtil.IsEmpty(strID))
        {
            DataRow[] drowCards = dtblCard.Select("ID=" + strID);

            foreach (DataRow drowCard in drowCards)
            {
                dtblCard.Rows.Remove(drowCard);
            }
        }

        //刪除已有級距
        if (!StringUtil.IsEmpty(strID))
        {
            DataRow[] drowLevels = dtblLevel.Select("ID=" + strID);

            foreach (DataRow drowLevel in drowLevels)
            {
                dtblLevel.Rows.Remove(drowLevel);
            }
        }

        //增加卡種
        foreach (DataRow drowCardID in UctrlCardType1.GetRightItem.Rows)
        {
            DataRow drowCard = dtblCard.NewRow();
            drowCard["ID"] = ViewState["GroupID"].ToString();
            if (!StringUtil.IsEmpty(strCardIDBak))
                drowCard["IDBak"] = strCardIDBak;
            drowCard["CardType_RID"] = drowCardID["RID"].ToString();
            drowCard["CardType_NAME"] = drowCardID["NAME"].ToString();

            if (radlType.SelectedValue == "2")
            {
                DataTable dtblSelCardType = (DataTable)ViewState["CardType"];

                DataRow drowSelCardType = dtblSelCardType.Rows.Find(drowCardID["RID"]);
                if (drowSelCardType != null)
                {
                    drowCard["Param_RID"] = drowSelCardType["Param_RID"];
                }

                foreach (DataRow drowLevelID in dtblJJ.Rows)
                {
                    DataRow drowLevel = dtblLevel.NewRow();

                    drowLevel["ID"] = ViewState["GroupID"].ToString();
                    if (!StringUtil.IsEmpty(strCardIDBak))
                        drowLevel["IDBak"] = strCardIDBak;

                    drowLevel["CardType_RID"] = drowCardID["RID"].ToString();
                    drowLevel["Level_Min"] = drowLevelID["txtjj_1"].ToString().Replace(",", "");
                    drowLevel["Level_Max"] = drowLevelID["txtjj_2"].ToString().Replace(",", "");
                    drowLevel["Price"] = drowLevelID["txt" + drowSelCardType["Param_RID"].ToString()].ToString().Replace(",", "");
                    drowLevel["Param_RID"] = drowSelCardType["Param_RID"];
                    drowLevel["Param_name"] = drowSelCardType["Param_name"];

                    dtblLevel.Rows.Add(drowLevel);
                }
            }
            else
            {
                drowCard["Param_RID"] = "";
            }

            dtblCard.Rows.Add(drowCard);
        }

        //刪除已有材質
        if (!StringUtil.IsEmpty(strID))
        {
            DataRow[] drowMaterials = dtblMaterial.Select("ID=" + strID);

            foreach (DataRow drowMaterial in drowMaterials)
            {
                dtblMaterial.Rows.Remove(drowMaterial);
            }
        }

        dtblMaterialTmp = dirAgreement["MaterialTmp"];

        //增加材質
        foreach (DataRow drowMaterialTmp in dtblMaterialTmp.Rows)
        {
            DataRow drowMaterial = dtblMaterial.NewRow();
            drowMaterial["ID"] = ViewState["GroupID"].ToString();
            if (!StringUtil.IsEmpty(strCardIDBak))
                drowMaterial["IDBak"] = strCardIDBak;
            drowMaterial["Material_RID"] = drowMaterialTmp["Material_RID"].ToString();
            drowMaterial["Material_Name"] = drowMaterialTmp["Material_Name"].ToString();
            drowMaterial["Base_Price"] = drowMaterialTmp["Base_Price"].ToString();
            dtblMaterial.Rows.Add(drowMaterial);
        }

        dirAgreement["MaterialTmp"].Clear();
        dirAgreement["LevelTmp"].Clear();


        string strActionType = Request.QueryString["ActionType"];

        if (strType == "1")
        {
            if (strActionType == "Add")
                Response.Redirect("BaseInfo002Add.aspx?Type=1");
            else
                Response.Redirect("BaseInfo002Mod.aspx?Type=1");
        }
        else if (strType == "2")
        {
            Response.Redirect("BaseInfo002Bak.aspx?ActionType=" + strActionType + "&Type=1&ID=" + strCardIDBak);
        }
    }

    protected void btnCancelUp_Click(object sender, EventArgs e)
    {
        string strType = Request.QueryString["Type"];
        string strCardIDBak = Request.QueryString["CardIDBak"];
        string strActionType = Request.QueryString["ActionType"];

        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];
        dirAgreement["MaterialTmp"].Clear();
        dirAgreement["LevelTmp"].Clear();

        if (strType == "1")
        {
            if (strActionType == "Add")
                Response.Redirect("BaseInfo002Add.aspx?Type=1");
            else
                Response.Redirect("BaseInfo002Mod.aspx?Type=1");
        }
        else if (strType == "2")
        {
            Response.Redirect("BaseInfo002Bak.aspx?ActionType=" + strActionType + "&Type=1&ID=" + strCardIDBak);
        }

    }

    protected void btnMaterialBind_Click(object sender, EventArgs e)
    {
        gvpbMaterial.BindData();
    }

    protected void radlType_SelectedIndexChanged(object sender, EventArgs e)
    {
        gvpbLevelTempBind();

        if (radlType.SelectedValue == "1")
        {
            trLevel.Visible = false;
            btnAddLevel.Visible = false;
        }
        else
        {
            trLevel.Visible = true;
            btnAddLevel.Visible = true;
        }
    }



    protected void dropParam_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtblCardType = (DataTable)ViewState["CardType"];
        DataRow drowCardType = dtblCardType.Rows[int.Parse(((DropDownList)sender).Attributes["Key"])];
        drowCardType["Param_RID"] = ((DropDownList)sender).SelectedValue;
        drowCardType["Param_Name"] = ((DropDownList)sender).SelectedItem.Text;
    }

    void UctrlCardType1_RightChange(object sender, EventArgs e)
    {
        DataTable dtblCardType = (DataTable)ViewState["CardType"];
        DataTable dtblDropCardType = (DataTable)ViewState["DropCardType"];
        DataTable dtblCardType_o = dtblCardType.Copy();

        dtblCardType.Rows.Clear();
        foreach (DataRow drow in UctrlCardType1.GetRightItem.Rows)
        {
            DataRow drowCardType_o = dtblCardType_o.Rows.Find(drow["RID"]);

            DataRow drowCardType = dtblCardType.NewRow();
            drowCardType["RID"] = drow["RID"];
            drowCardType["Name"] = drow["Name"];
            if (drowCardType_o != null)
            {
                drowCardType["Param_RID"] = drowCardType_o["Param_RID"];
                drowCardType["Param_Name"] = drowCardType_o["Param_Name"];
            }
            else
            {
                drowCardType["Param_RID"] = dtblDropCardType.Rows[0][0].ToString();
                drowCardType["Param_Name"] = dtblDropCardType.Rows[0][1].ToString();
            }

            dtblCardType.Rows.Add(drowCardType);
        }

        gvpbCardTypeBind();
    }

    #endregion

    #region 欄位/資料補充說明
    protected void AjaxValidator1_OnAjaxValidatorQuest(object sender, ControlLibrary.AjaxValidatorEventArgs e)
    {
        string strType = e.QueryInfo.Split(',')[0];
        string strID = e.QueryInfo.Split(',')[1];

        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];


        //合約卡種組合
        DataRow[] drows = null;

        if (strType == "1")
        {
            if (!StringUtil.IsEmpty(strID))
                drows = dirAgreement["Group"].Select("ID<>" + strID);
            else
                drows = dirAgreement["Group"].Select("1=1");
        }
        else if (strType == "2")
        {
            if (!StringUtil.IsEmpty(strID))
                drows = dirAgreement["GroupBakTmp"].Select("ID<>" + strID);
            else
                drows = dirAgreement["GroupBakTmp"].Select("1=1");
        }

        foreach (DataRow drow in drows)
        {
            if (drow["Group_Name"].ToString() == e.QueryData.Trim())
            {
                e.IsAllowSubmit = false;
                return;
            }
        }
        e.IsAllowSubmit = true;
    }

   
    #endregion

    #region 列表資料綁定

    protected void gvpbLevel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblLevel = (DataTable)gvpbLevel.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblLevel.Rows.Count == 0)
                return;

            Button ibtnButton = null;

            // 刪除的邦定事件
            ibtnButton = (Button)e.Row.FindControl("btnDeljj");
            ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
            ibtnButton.Command += new CommandEventHandler(ibtnButton_Command);
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            DataTable dtbl = (DataTable)ViewState["testtb"];
            for (int j = 0; j < dtbl.Columns.Count; j++)
            {
                TextBox txt = new TextBox();
                txt = (TextBox)e.Row.FindControl(dtbl.Columns[j].ToString());

                txt.Text = dtbl.Rows[e.Row.RowIndex][dtbl.Columns[j].ToString()].ToString();
                if (!StringUtil.IsEmpty(txt.Text))
                {
                    if (txt.ID == "txtjj_1" || txt.ID == "txtjj_2")
                    {
                        txt.Text = int.Parse(txt.Text).ToString("N0");
                    }
                    else
                    {
                        txt.Text = decimal.Parse(txt.Text).ToString("N2");
                    }
                }

            }
        }
    }

    /// <summary>
    /// 刪除級距
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ibtnButton_Command(object sender, CommandEventArgs e)
    {
        gvpbLevelTempBind();

        string aa = e.CommandArgument.ToString();

        DataTable dtbl = (DataTable)ViewState["testtb"];
        dtbl.Rows.RemoveAt(int.Parse(aa));

        gvpbLevelBind();
    }

    protected void gvpbLevelBind()
    {
        this.gvpbLevel.DataSource = (DataTable)ViewState["testtb"];
        this.gvpbLevel.DataBind();
    }
    protected void gvpbMaterial_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataTable dtblMaterial = ((Dictionary<string, DataTable>)Session["Agreement"])["MaterialTmp"];
        e.Table = dtblMaterial;
        e.RowCount = dtblMaterial.Rows.Count;
    }

    protected void gvpbMaterial_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbMaterial.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtbl.Rows.Count == 0)
                return;

            ImageButton ibtnButton = null;
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDeleteMaterial");
            ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
            ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");

            LinkButton lbtnCardName = null;
            lbtnCardName = (LinkButton)e.Row.FindControl("lbtnMaterial");
            lbtnCardName.CommandArgument = e.Row.RowIndex.ToString();
            lbtnCardName.Text = dtbl.Rows[e.Row.RowIndex]["Material_Name"].ToString();

            if (e.Row.Cells[1].Text != "&nbsp;")
            {
                e.Row.Cells[1].Text = Convert.ToDecimal(e.Row.Cells[1].Text).ToString("N2");
            }
        }
    }



    /// <summary>
    /// 刪除材質
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteMaterial_Command(object sender, CommandEventArgs e)
    {
        DataTable dtblMaterial = ((Dictionary<string, DataTable>)Session["Agreement"])["MaterialTmp"];
        dtblMaterial.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
        gvpbMaterial.BindData();
    }

  

    /// <summary>
    /// 卡種明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnMaterial_Click(object sender, CommandEventArgs e)
    {
        string strActionType = Request.QueryString["ActionType"];

        string strUrl = "var aa=window.showModalDialog('BaseInfo002Materiel.aspx?ActionType=" + strActionType + "&RowId=" + e.CommandArgument.ToString() + "','','dialogHeight:450px;dialogWidth:600px;');if(aa!=undefined){AddMaterial();}";

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), strUrl, true);
    }

    protected void gvpbCardType_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtbl = (DataTable)gvpbCardType.DataSource;
        DataTable dtblDropCardType = (DataTable)ViewState["DropCardType"];

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtbl.Rows.Count == 0)
                return;

            DropDownList dropParam_RID = null;
            dropParam_RID = (DropDownList)e.Row.FindControl("dropParam_RID");
            dropParam_RID.DataSource = (DataTable)ViewState["DropCardType"];
            dropParam_RID.DataBind();
            dropParam_RID.Attributes.Add("Key", e.Row.RowIndex.ToString());
            dropParam_RID.SelectedIndexChanged += new EventHandler(dropParam_RID_SelectedIndexChanged);

            if (dtbl.Rows[e.Row.RowIndex]["Param_RID"] != null)
                dropParam_RID.SelectedValue = dtbl.Rows[e.Row.RowIndex]["Param_RID"].ToString();

        }
    }

    protected void gvpbCardTypeBind()
    {
        if (ViewState["CardType"] != null)
        {
            DataTable dtblCardType = (DataTable)ViewState["CardType"];
            gvpbCardType.DataSource = dtblCardType;
            gvpbCardType.DataBind();

            //if (radlType.SelectedValue == "2")
            //    CreateLevelTable();
        }
    }


    #endregion

    #region 級距檢查

    /// <summary>
    /// 檢查級距區間重複
    /// </summary>
    private bool CheckJJCF()
    {
        DataTable dtbl = (DataTable)ViewState["testtb"];
        if (dtbl.Rows.Count > 1)
        {
            int iJJMin = int.Parse(dtbl.Rows[dtbl.Rows.Count - 1]["txtjj_1"].ToString());
            int iJJMax = int.Parse(dtbl.Rows[dtbl.Rows.Count - 1]["txtjj_2"].ToString());

            for (int i = 0; i < dtbl.Rows.Count - 1; i++)
            {
                int iMax = Convert.ToInt32(dtbl.Rows[i]["txtjj_2"].ToString().Replace(",", ""));
                int iMin = Convert.ToInt32(dtbl.Rows[i]["txtjj_1"].ToString().Replace(",", ""));

                if ((iJJMin >= iMin && iJJMin <= iMax) || (iJJMax >= iMin && iJJMax <= iMax))
                {
                    ShowMessage("級距區間不能重複");
                    return false;
                }

                if ((iMin >= iJJMin && iMin <= iJJMax) || (iMax >= iJJMin && iMax <= iJJMax))
                {
                    ShowMessage("級距區間不能重複");
                    return false;
                }
            }
        }
        return true;

    }

    /// <summary>
    /// 檢查級距是否輸入完整
    /// </summary>
    private bool CheckJJSR()
    {
        DataTable dtbl = (DataTable)ViewState["testtb"];
        foreach (DataRow drow in dtbl.Rows)
        {
            if (StringUtil.IsEmpty(drow["txtjj_1"].ToString()) || StringUtil.IsEmpty(drow["txtjj_2"].ToString()))
            {
                ShowMessage("級距不能為空");
                return false;
            }

            DataTable dtblCardType = (DataTable)ViewState["CardType"];
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                if (StringUtil.IsEmpty(drow["txt" + drowCardType["Param_RID"].ToString()].ToString()))
                {
                    ShowMessage(drowCardType["Param_Name"].ToString() + "不能為空");
                    return false;
                }
                if (Convert.ToDecimal(drow["txt" + drowCardType["Param_RID"].ToString()].ToString()) == 0)
                {
                    ShowMessage(drowCardType["Param_Name"].ToString() + "不能為0");
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 檢查級距是否連續
    /// </summary>
    /// <returns></returns>
    private bool CheckJJLX()
    {
        DataTable dtbl = (DataTable)ViewState["testtb"];
        if (dtbl.Select("txtjj_1='1'").Length != 1)
        {
            ShowMessage("級距必須從1開始");
            return false;
        }
        if (dtbl.Select("txtjj_2='999999999'").Length != 1)
        {
            ShowMessage("級距必須以999,999,999結束");
            return false;
        }

        DataRow[] drowRows = dtbl.Select("", "txtjj_1");

        DataTable dtbl1 = dtbl.Clone();

        for (int i = 0; i < drowRows.Length; i++)
        {
            dtbl1.Rows.Add(drowRows[i].ItemArray);
        }
        if (dtbl1.Rows.Count > 1)
        {
            for (int j = 0; j < dtbl1.Rows.Count; j++)
            {
                if (j == dtbl1.Rows.Count - 1)
                    break;
                int ijj = int.Parse(dtbl1.Rows[j + 1]["txtjj_1"].ToString()) - int.Parse(dtbl1.Rows[j]["txtjj_2"].ToString());
                if (ijj != 1)
                {
                    ShowMessage("級距必須連續");
                    return false;
                }
            }
        }

        ViewState["testtb"] = dtbl1;

        return true;
    }

    private void gvpbLevelTempBind()
    {
        DataTable dtbl = (DataTable)ViewState["testtb"];

        dtbl.Rows.Clear();

        for (int i = 0; i < gvpbLevel.Rows.Count; i++)
        {
            DataRow drow = dtbl.NewRow();

            for (int j = 0; j < dtbl.Columns.Count; j++)
            {
                TextBox txt = new TextBox();
                txt = (TextBox)gvpbLevel.Rows[i].FindControl(dtbl.Columns[j].ToString());

                if (StringUtil.IsEmpty(txt.Text.Replace(",", "")))
                    continue;

                drow[dtbl.Columns[j].ToString()] = txt.Text.Replace(",", "");
            }
            dtbl.Rows.Add(drow);
        }
    }
    #endregion

    protected void btnAddLevel_Click(object sender, EventArgs e)
    {
        gvpbLevelTempBind();

        if (!CheckJJSR())
            return;

        if (!CheckJJCF())
            return;

        DataTable dtbl = (DataTable)ViewState["testtb"];

        DataRow dr = dtbl.NewRow();

        dtbl.Rows.Add(dr);

        gvpbLevelBind();
    }
}

public class GridViewTemplateTextBox : ITemplate
{
    private string _columnType;
    private string _columnName;

    public GridViewTemplateTextBox(string columnType, string colname)
    {
        _columnType = columnType;
        _columnName = colname;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        switch (_columnType)
        {
            case "Head":
                Literal lc = new Literal();
                lc.Text = _columnName;
                container.Controls.Add(lc);
                break;
            case "Txt1":
                TextBox tb1 = new TextBox();
                tb1.ID = _columnName + "_1";
                tb1.Width = Unit.Pixel(90);
                tb1.Style.Value = "ime-mode:disabled;text-align: right";
                tb1.Style.Add("TEXT-ALIGN", "right");
                tb1.Attributes.Add("onfocus", "DelDouhao(this)");
                tb1.Attributes.Add("onblur", "value=GetValue(this.value)");
                tb1.Attributes.Add("onkeyup", "CheckNumWithNoId(this,9)");
                tb1.MaxLength = 11;// <% --Dana 20161021 最大長度由9改為11-- %>
                TextBox tb2 = new TextBox();
                tb2.ID = _columnName + "_2";
                tb2.Width = Unit.Pixel(80);
                tb2.Style.Value = "ime-mode:disabled;text-align: right";
                tb2.Style.Add("TEXT-ALIGN", "right");
                tb2.Attributes.Add("onblur", "if(value.toUpperCase()=='MAX'){value=999999999}CheckNumWithNoId(this,9);value=GetValue(this.value);");
                tb2.MaxLength = 11;// <% --Dana 20161021 最大長度由9改為11-- %>

                Literal lb = new Literal();
                lb.Text = "~";

                container.Controls.AddAt(0, tb1);
                container.Controls.AddAt(1, lb);
                container.Controls.AddAt(2, tb2);
                break;
            case "Txt":
                TextBox tb = new TextBox();
                tb.ID = _columnName;
                tb.Width = Unit.Pixel(90);
                tb.Style.Value = "ime-mode:disabled;text-align: right";
                tb.Attributes.Add("onfocus", "DelDouhao(this)");
                tb.Attributes.Add("onblur", "CheckAmtWithNoId(this,8,2);value=GetValue(this.value)");
                tb.Attributes.Add("onkeyup", "CheckAmtWithNoId(this,8,2)");
                tb.MaxLength = 13;// <% --Dana 20161021 最大長度由11改為13-- %>
                container.Controls.Add(tb);
                break;
            case "Btn":
                Button btn = new Button();
                btn.ID = _columnName;
                btn.Text = "-";
                btn.CssClass = "btn";
                container.Controls.Add(btn);
                break;
            default:
                break;
        }
    }

    private void tb_DataBinding(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)sender;
        tb.Width = Unit.Percentage(100);
        GridViewRow container = (GridViewRow)tb.NamingContainer;
        tb.Text = ((DataRowView)container.DataItem)["rolename"].ToString();
        tb.Width = Unit.Pixel(70);
    }
}




