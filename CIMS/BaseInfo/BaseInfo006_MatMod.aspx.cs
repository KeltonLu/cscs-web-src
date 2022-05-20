using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class BaseInfo_BaseInfo006_MatMod : PageBase
{
    BaseInfo006BL bizLogic = new BaseInfo006BL();
    public string strType = "";
    public string strURLPath = "BaseInfo006_Mat.aspx";


    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        strType = Request.QueryString["Type"];
        if (strType == "1")
        {
            strURLPath += "?Type=1";
            lbTitle.Text = "紙品物料設定-用途群組修改/刪除";
        }
        else if (strType == "2")
        {
            strURLPath += "?Type=2";
            lbTitle.Text = "紙品物料設定-合約級距修改/刪除";
        }
        else if (strType == "3")
        {
            strURLPath += "?Type=3";
            lbTitle.Text = "紙品物料設定-庫存原因修改/刪除";
        }

        if (!IsPostBack)
        {

            string strRID = Request.QueryString["RID"];
            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strRID))
            {
                PARAM cbModel = bizLogic.GetParameter(strRID);
                ViewState["param"] = cbModel.Param_Name;
                //設置控件的值
                SetControls(cbModel);
                txtParamType_Name.Text = cbModel.Param_Code.PadLeft(2, '0');
                //201005CR 代碼設定-紙品物料庫存原因設定，新增 合併計算 欄位 add by Ian Huang 2010/06/11 start
                if ("Y" == cbModel.Is_Merge)
                {
                    radIs_Merge.Checked = true;
                    radIs_Merge_N.Checked = false;
                }
                else
                {
                    radIs_Merge.Checked = false;
                    radIs_Merge_N.Checked = true;
                }
                //201005CR 代碼設定-紙品物料庫存原因設定，新增 合併計算 欄位 add by Ian Huang 2010/06/11 end
                if (cbModel.Is_Delete == "N")
                    trDel.Visible = false;
                else
                    trDel.Visible = true;
                //製域處理

            }
        }
    }

    /// <summary>
    /// 修改/刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        PARAM cbModel = new PARAM();

        string strRID = Request.QueryString["RID"];

        try
        {
            if (chkDel.Checked)         //刪除
            {
                bizLogic.Delete(strRID, strType);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "BaseInfo006_Mat.aspx?Con=1&Type=" + strType);
            }
            else                        //修改 
            {
                if (txtParam_Name.Text.Trim() == "")
                {
                    ShowMessage("參數值不能為空");
                    return;
                }

                if (StringUtil.GetByteLength(txtParam_Comment.Text) > 500)
                {
                    ShowMessage("備註不能超過500個字符");
                    return;
                }

                if (checkInput())
                {
                    ShowMessage(ViewState["check"].ToString());
                    return;
                }
                cbModel.RID = Convert.ToInt32(strRID);
                SetData(cbModel);
                bizLogic.Update(cbModel);


                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "BaseInfo006_Mat.aspx?Con=1&Type=" + strType);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }


    /// <summary>
    /// 同btnEdit_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnEdit1_Click(object sender, EventArgs e)
    {
        btnEdit_Click(sender, e);
    }

    #endregion


    #region 欄位/資料補充說明

    protected bool checkInput()
    {

        String checkStatus = "";
        String Param_Type_Code = hdParamType_Code.Value;
        //String Param_Type_text = txtParamType_Name.Text;
        bool paramExist = false;
        if (txtParam_Name.Text != ViewState["param"].ToString())
        {
            paramExist = bizLogic.ContainsID(txtParam_Name.Text, Param_Type_Code);
        }
        if (paramExist)
            checkStatus = "紙品物料設定-庫存原因已經有參數值" + txtParam_Name.Text + "!";
        ViewState["check"] = checkStatus;
        return paramExist;

    }

    #endregion
}
