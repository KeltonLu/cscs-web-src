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

public partial class CardType_CardType0041CardAdd : PageBase
{
    CardType004BL bl = new CardType004BL();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            try
            {
                // 獲取 Perso廠商資料
                DataSet dstFactory = bl.GetFactoryList();
                dropFactory_RID.DataValueField = "RID";
                dropFactory_RID.DataTextField = "Factory_ShortName_CN";
                dropFactory_RID.DataSource = dstFactory.Tables[0];
                dropFactory_RID.DataBind();

                UctrlCardType.SetLeftItem = "select * from card_type where 1<0";

                PersoProjectBind();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        UctrlCardType.Is_Using = true;
        if (ViewState["strLeftItem"] != null)
            UctrlCardType.SetLeftItem = ViewState["strLeftItem"].ToString();
        else
            UctrlCardType.SetLeftItem = "select * from card_type where 1<0";
    }


    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (StringUtil.IsEmpty(dropFactory_RID.SelectedValue))
        {
            ShowMessage("請選擇一個Perso廠");
            return;
        }

        if (StringUtil.IsEmpty(dropPersoProject_RID.SelectedValue))
        {
            ShowMessage("請選擇一個代製項目");
            return;
        }

        if (UctrlCardType.GetRightItem.Rows.Count==0)
        {
            ShowMessage("請選擇一個卡種");
            return;
        }

        if (StringUtil.GetByteLength(txtComment.Text) > 100)
        {
            ShowMessage("備註不能超過100個字符");
            return;
        }

        try
        {
            CARDTYPE_PROJECT_TIME cptModel = new CARDTYPE_PROJECT_TIME();

            SetData(cptModel);

            bl.AddCard(cptModel, UctrlCardType.GetRightItem);

            ShowMessageAndGoPage("新增成功", string.Concat("CardType0041CardAdd.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    //待製項目綁定
    private void PersoProjectBind()
    {
        dropPersoProject_RID.Items.Clear();

        if (!StringUtil.IsEmpty(dropFactory_RID.SelectedValue))
        {
            dropPersoProject_RID.DataValueField = "RID";
            dropPersoProject_RID.DataTextField = "Project_Name";
            dropPersoProject_RID.DataSource = bl.GetPersoProject(dropFactory_RID.SelectedValue);
            dropPersoProject_RID.DataBind();
            CardTypBind();
        }
    }
    protected void dropFactory_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        PersoProjectBind();
    }
    protected void txtUse_Date_Begin_TextChanged(object sender, EventArgs e)
    {
        CardTypBind();
    }

    private void CardTypBind()
    {
        if (!StringUtil.IsEmpty(txtUse_Date_Begin.Text) && !StringUtil.IsEmpty(dropPersoProject_RID.SelectedValue))
        {
            UctrlCardType.LeftItemClear();
            UctrlCardType.RightItemClear();

            object[] org = new object[4];
            org[2] = dropFactory_RID.SelectedValue;
            org[0] = txtUse_Date_Begin.Text;
            org[1] = lblUse_Date_End.Text;
            org[3] = dropPersoProject_RID.SelectedValue;

            string strLeftItem = String.Format(CardType004BL.SEL_CARD_TYPE, org);

            ViewState["strLeftItem"] = strLeftItem;

            UctrlCardType.SetLeftItem = String.Format(CardType004BL.SEL_CARD_TYPE, org);
            UctrlCardType.LbLeftBind();
        }
        else
        {
            ViewState["strLeftItem"] = null;
            UctrlCardType.LeftItemClear();
            UctrlCardType.RightItemClear();
        }
    }
}
