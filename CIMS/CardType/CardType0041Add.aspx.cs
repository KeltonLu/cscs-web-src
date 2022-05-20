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

public partial class CardType_CardType0041Add : PageBase
{
    CardType004BL bl = new CardType004BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            ViewState["IsNewAdd"] = false;//是否是新增 true為新增
            try
            {
                // 獲取 Perso廠商資料
                DataSet dstFactory = bl.GetFactoryList();
                dropFactory.DataValueField = "RID";
                dropFactory.DataTextField = "Factory_ShortName_CN";
                dropFactory.DataSource = dstFactory.Tables[0];
                dropFactory.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (StringUtil.IsEmpty(this.dropFactory.SelectedValue.ToString()))
            {
                ShowMessage("廠商必須選擇！");
                return;
            }

            if (StringUtil.GetByteLength(txtComment.Text) > 100)
            {
                ShowMessage("備註不能超過100個字符");
                return;
            }

            if (bl.IsExistStep(Convert.ToInt32(this.dropFactory.SelectedValue), this.txtName.Text.Trim()))//true製程和對應之Perso廠在資料庫中存在
            {
                
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "ExistFactory_RID_isok();", true);
            }
            else
            {
                ViewState["IsNewAdd"] = true;
                
                add();
            }
        }
        catch (Exception ex)
        {
            
            ShowMessage(ex.Message);
        }
    }

    protected void btnIsOk_Click(object sender, EventArgs e)
    {
        ViewState["IsNewAdd"] = false;
        add();
    }

    private void add()
    {
        PROJECT_STEP psModel = new PROJECT_STEP();
        if (StringUtil.IsEmpty(txtPrice.Text))
        {
            ShowMessage("單價不能為空");
            return;
        }

        SetData(psModel);

        psModel.Price = Convert.ToDecimal(txtPrice.Text);

        // 設置廠商資料
        psModel.Factory_RID = Convert.ToInt32(this.dropFactory.SelectedValue);
        try
        {
            bl.Add1(psModel, (bool)ViewState["IsNewAdd"]);
            ShowMessageAndGoPage("增加成功", "CardType0041Add.aspx?List=radStep");
        }
        catch (Exception ex)
        {
            if (!StringUtil.IsEmpty(txtPrice.Text))
            {
                txtPrice.Text = Convert.ToDecimal(txtPrice.Text.Replace(",", "")).ToString("N4");
            }
            ShowMessage(ex.Message);
        }
    }
}
