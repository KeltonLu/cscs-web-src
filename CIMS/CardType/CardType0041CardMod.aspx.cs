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

public partial class CardType_CardType0041CardMod : PageBase
{
    CardType004BL bl = new CardType004BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        UctrlCardType.Is_Using = true;
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];
            try
            {
                DataTable dtblCardPerso = bl.LoadCardPerso(strRID);
                if (dtblCardPerso.Rows.Count > 0)
                {
                    SetControlsForDataRow(dtblCardPerso.Rows[0]);
                    
                    object[] org = new object[4];
                    org[2] = dtblCardPerso.Rows[0]["Factory_ShortName_CN_ID"].ToString();
                    org[0] = dtblCardPerso.Rows[0]["use_date_begin"].ToString();
                    org[1] = dtblCardPerso.Rows[0]["Use_Date_End"].ToString();
                    org[3] = dtblCardPerso.Rows[0]["PERSOpROJECT_RID"].ToString();

                    ViewState["SelCondition"]=org;

                    UctrlCardType.SetLeftItem = String.Format(CardType004BL.SEL_CARD_TYPE, org);

                    ViewState["PERSOpROJECT_RID"] = dtblCardPerso.Rows[0]["PERSOpROJECT_RID"].ToString();

                    DataTable dtblCardType = bl.GetCardtypePerso(strRID);

                    ViewState["dtblCardType"] = dtblCardType;

                    UctrlCardType.SetRightItem = dtblCardType;

                    ViewState["Use_Date_Begin"] = dtblCardPerso.Rows[0]["Use_Date_Begin"];
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RID"];
        int intType = 0;
        //0:什麽都不修改
        //1:不變更使用期間起，修改卡種
        //2:變更使用期間起，不修改卡種
        //3:變更使用期間起，修改卡種

        if (!chkDel.Checked)
        {

            DataTable dtblCardType = (DataTable)ViewState["dtblCardType"];

            if (StringUtil.GetByteLength(txtComment.Text) > 100)
            {
                ShowMessage("備註不能超過100個字符");
                return;
            }

            if (UctrlCardType.GetRightItem.Rows.Count == 0)
            {
                ShowMessage("請選擇一個卡種");
                return;
            }

            if (ViewState["Use_Date_Begin"].ToString() == txtUse_Date_Begin.Text)
            {
                if (dtblCardType.Rows.Count == UctrlCardType.GetRightItem.Rows.Count)
                {
                    foreach (DataRow drow in UctrlCardType.GetRightItem.Rows)
                    {
                        if (dtblCardType.Select("RID='" + drow["RID"].ToString() + "'").Length == 0)
                        {
                            intType = 1;
                            break;
                        }
                    }
                }
                else
                {
                    intType = 1;
                }
            }
            else
            {
                if (dtblCardType.Rows.Count == UctrlCardType.GetRightItem.Rows.Count)
                {
                    foreach (DataRow drow in UctrlCardType.GetRightItem.Rows)
                    {
                        if (dtblCardType.Select("RID='" + drow["RID"].ToString() + "'").Length == 0)
                        {
                            intType = 3;
                            break;
                        }
                    }
                    if (intType == 0)
                        intType = 2;
                }
                else
                {
                    intType = 3;
                }
            }
        }
        try
        {
            if (!chkDel.Checked)
            {
                CARDTYPE_PROJECT_TIME cptModel = new CARDTYPE_PROJECT_TIME();

                SetData(cptModel);

                cptModel.PersoProject_RID = int.Parse(ViewState["PERSOpROJECT_RID"].ToString());
                cptModel.RID = int.Parse(strRID);

                bl.UpdataCard(cptModel, UctrlCardType.GetRightItem, intType);

                ShowMessageAndGoPage("修改成功", "CardType0041Card.aspx?Con=1");
            }
            else
            {
                bl.DelCard(strRID);

                ShowMessageAndGoPage("刪除成功", "CardType0041Card.aspx?Con=1");
            }

        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }
    protected void txtUse_Date_Begin_TextChanged(object sender, EventArgs e)
    {
        if (!StringUtil.IsEmpty(txtUse_Date_Begin.Text))
        {
            object[] org = (object[])ViewState["SelCondition"];

            UctrlCardType.LeftItemClear();
            UctrlCardType.RightItemClear();
           
            org[0] = txtUse_Date_Begin.Text;

            UctrlCardType.SetLeftItem = String.Format(CardType004BL.SEL_CARD_TYPE, org);
            UctrlCardType.LbLeftBind();
        }
        else
        {
            UctrlCardType.LeftItemClear();
            UctrlCardType.RightItemClear();
        }
    }
}
