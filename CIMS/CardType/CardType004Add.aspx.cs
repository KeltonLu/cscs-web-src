using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class CardType_CardType004Add : PageBase
{
    CardType004BL bl = new CardType004BL();
    private int _RightMaxLenght = 0;
    public int RightMaxLenght
    {
        get
        {
            return _RightMaxLenght;
        }
        set
        {
            _RightMaxLenght = value;
        }
    }
    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbPrice.NoneData = "";
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

                LbLeftBind();

                gvpbPrice.BindData();

                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doRadio(document.getElementById('adrtNormal'));", true);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        PERSO_PROJECT ppModel = new PERSO_PROJECT();

        if (StringUtil.GetByteLength(txtComment.Text) > 100)
        {
            ShowMessage("備註不能超過100個字符");
            return;
        }

        // Legend 2016/10/28 調整display: 由 table-row 改為 table-row
        if (adrtNormal.Checked)
        {
            ppModel.Normal_Special = "1";
            
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType').style.display='table-row';", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType1').style.display='none';", true);
            if (LbRight.Items.Count == 0)
            {
                ShowMessage("請選擇製程！");
                return;
            }
        }
        else
        {
            ppModel.Normal_Special = "2";
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType').style.display='none';", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType1').style.display='table-row';", true);
            if (StringUtil.IsEmpty(txtUnit_Price.Text))
            {
                ShowMessage("請填寫單價！");
                return;
            }
        }

        try
        {
            if (StringUtil.IsEmpty(this.dropFactory_RID.SelectedValue.ToString()))
            {
                ShowMessage("廠商必須選擇！");
                return;
            }
            else
            {

                SetData(ppModel);

                if (adrtNormal.Checked)
                    bl.Add(ppModel, (DataTable)ViewState["dtblStepQujian"], ViewState["StepID"].ToString());
                else
                    bl.Add(ppModel, null, "");
                ShowMessageAndGoPage("增加成功", string.Concat("CardType004Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
            }

        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #endregion

    protected void dropFactory_RID_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (adrtNormal.Checked)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType').style.display='table-row';", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType1').style.display='none';", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType').style.display='none';", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "document.getElementById('trCardType1').style.display='table-row';", true);
        }

        LbLeftBind();
        LbRight.Items.Clear();
        gvpbPrice.BindData();
        
    }

    #region ListBox綁定
    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < LbLeft.Items.Count; i++)
        {
            if (_RightMaxLenght != 0)
            {
                if (LbRight.Items.Count >= _RightMaxLenght)
                    break;
            }
            LbRight.Items.Add(LbLeft.Items[i]);
        }
        DelLeftItem();
        gvpbPrice.BindData();
    }
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        int i = 0;
        while (i < LbLeft.Items.Count)
        {
            if (LbLeft.Items[i].Selected == true)
            {
                if (_RightMaxLenght != 0)
                {
                    if (LbRight.Items.Count >= _RightMaxLenght)
                        break;
                }

                LbRight.Items.Add(LbLeft.Items[i]);
                LbLeft.Items.Remove(LbLeft.Items[i]);
            }
            else
                i += 1;
        }
        gvpbPrice.BindData();
    }
    protected void btnRemove_Click(object sender, EventArgs e)
    {
        DataSet dsltCardType = (DataSet)ViewState["CardType"];

        int i = 0;
        while (i < LbRight.Items.Count)
        {
            if (LbRight.Items[i].Selected == true)
            {
                foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
                {
                    if (LbRight.Items[i].Value == drowCardType["step_id"].ToString())
                    {
                        LbLeft.Items.Add(LbRight.Items[i]);
                        break;
                    }
                }
                LbRight.Items.Remove(LbRight.Items[i]);
            }
            else
                i += 1;
        }
        gvpbPrice.BindData();
    }
    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        DataSet dsltCardType = (DataSet)ViewState["CardType"];

        for (int i = 0; i < LbRight.Items.Count; i++)
        {
            foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
            {
                if (LbRight.Items[i].Value == drowCardType["step_id"].ToString())
                {
                    LbLeft.Items.Add(LbRight.Items[i]);
                    break;
                }
            }
        }
        LbRight.Items.Clear();


        gvpbPrice.BindData();
    }

    protected void DelLeftItem()
    {
        foreach (ListItem li in LbRight.Items)
        {
            if (LbLeft.Items.FindByValue(li.Value) != null)
                LbLeft.Items.Remove(li);
        }
    }

    protected void LbLeftBind()
    {
        //綁定製程LISTBOX
        LbLeft.Items.Clear();

        if (StringUtil.IsEmpty(dropFactory_RID.SelectedValue))
            return;

        DataSet dsProject_Step = bl.GetStepData(dropFactory_RID.SelectedValue);

        ViewState["CardType"] = dsProject_Step;

        LbLeft.DataTextField = "Name";
        LbLeft.DataValueField = "step_id";
        LbLeft.DataSource = dsProject_Step;
        LbLeft.DataBind();
    }

    #endregion

    protected void gvpbPrice_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        string strStepID="";

        DataTable dtblStepQujian = new DataTable();
     

        //取出選擇的製成
        foreach (ListItem li in LbRight.Items)
        {
            strStepID += li.Value + ",";
        }

        //查詢製成區間
        if (!StringUtil.IsEmpty(strStepID))
        {
            strStepID = strStepID.Substring(0, strStepID.Length - 1);
            ViewState["StepID"] = strStepID;
            dtblStepQujian = bl.GetStepTime(strStepID);
        }

        ViewState["dtblStepQujian"] = dtblStepQujian;

        e.Table = dtblStepQujian;
        e.RowCount = dtblStepQujian.Rows.Count;
    }
}