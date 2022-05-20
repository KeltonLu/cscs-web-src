using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class BaseInfo_BaseInfo006_UseAdd : PageBase
{
    BaseInfo006BL bizLogic = new BaseInfo006BL();
    public string strURLPath = "BaseInfo006_Use.aspx";

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        string strType = Request.QueryString["Type"];
        string strParamType_Code = "";

        if (strType == "1")
        {
            strParamType_Code = GlobalString.ParameterType.Use;
            strURLPath += "?Type=1";
            lbTitle.Text = "卡片管理設定-用途群組新增";
        }
        else if (strType == "2")
        {
            strParamType_Code = GlobalString.ParameterType.CardType;
            strURLPath += "?Type=2";
            lbTitle.Text = "卡片管理設定-合約級距新增";
        }
        else if (strType == "3")
        {
            strParamType_Code = GlobalString.ParameterType.MatType1;
            strURLPath += "?Type=3";
            lbTitle.Text = "卡片管理設定-庫存原因新增";
        }

        if (!IsPostBack)
        {


            DataTable dtblType = bizLogic.getParamType(strParamType_Code).Tables[0];
            if (dtblType.Rows.Count > 0)
            {
                txtParamType_Name.Text = dtblType.Rows[0]["PARAM_NAME"].ToString();
                hdParamType_Code.Value = strParamType_Code;
            }

        }
    }

    /// <summary>
    /// 添加資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (StringUtil.GetByteLength(txtParam_Comment.Text) > 500)
        {
            ShowMessage("備註不能超過500個字符");
            return;
        }

        try
        {
            if (checkInput())
            {
                ShowMessage(ViewState["check"].ToString());
                return;
            }
            PARAM cbModel = new PARAM();
            SetData(cbModel);

            bizLogic.Add(cbModel);

            string strType = Request.QueryString["Type"];
            string strURL = "BaseInfo006_UseAdd.aspx";

            if (strType == "1")
                strURL += "?Type=1";
            else if (strType == "2")
                strURL += "?Type=2";
            else if (strType == "3")
                strURL += "?Type=3";


            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, strURL);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

    }


    /// <summary>
    /// 同btnAdd_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd1_Click(object sender, EventArgs e)
    {
        btnAdd_Click(sender, e);
    }


    #endregion

    #region 欄位/資料補充說明
    /// <summary>
    /// 判斷當前選中的參數種類和參數代碼的組合是否滿足唯一性
    /// </summary>
    /// <returns></returns>
    protected bool checkInput()
    {
        String checkStatus = "";
        String Param_Type_Code = hdParamType_Code.Value;
        String Param_Type_text = txtParamType_Name.Text;
        bool paramExist = bizLogic.ContainsID(txtParam_Name.Text, Param_Type_Code);
        if (paramExist)
            checkStatus = "參數名稱" + Param_Type_text + "下面已經有參數值" + txtParam_Name.Text + "!";
        ViewState["check"] = checkStatus;
        return paramExist;

    }
    #endregion

}
