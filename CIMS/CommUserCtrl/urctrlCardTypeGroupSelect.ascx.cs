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

public partial class CommUserCtrl_urctrlCardTypeGroupSelect : System.Web.UI.UserControl
{
    CardTypeManager ctmManager = new CardTypeManager();

    private bool _Is_Using = false;
    private bool _CardTypeAll;


    public bool Is_Using
    {
        set
        {
            _Is_Using = value;
        }
    }

    /// <summary>
    /// 獲取已選卡種
    /// </summary>
    public string CardType
    {
        get
        {
            return dropCard_Type.SelectedValue;
        }
        set
        {
            if (dropCard_Type.Items.FindByValue(value) != null)
                dropCard_Type.SelectedValue = value;
        }
    }

    public bool CardTypeAll
    {
        set
        {
            _CardTypeAll = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dropCard_PurposeBind();
            dropCard_GroupBind();
            dropCard_TypeBind();
        }
    }

    protected void dropCard_Purpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_GroupBind();
        dropCard_TypeBind();
    }

    protected void dropCard_Group_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_TypeBind();
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    protected void dropCard_PurposeBind()
    {
        dropCard_Purpose.DataTextField = "PARAM_NAME";
        dropCard_Purpose.DataValueField = "Param_Code";
        dropCard_Purpose.DataSource = ctmManager.GetPurpose();
        dropCard_Purpose.DataBind();

        dropCard_Purpose.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 群組下拉框綁定
    /// </summary>
    protected void dropCard_GroupBind()
    {
        dropCard_Group.Items.Clear();

        dropCard_Group.DataTextField = "GROUP_NAME";
        dropCard_Group.DataValueField = "RID";
        dropCard_Group.DataSource = ctmManager.GetGroupByPurposeId(dropCard_Purpose.SelectedValue);
        dropCard_Group.DataBind();

        dropCard_Group.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
    }

    /// <summary>
    /// 卡種綁定
    /// </summary>
    protected void dropCard_TypeBind()
    {
        dropCard_Type.Items.Clear();

        dropCard_Type.DataTextField = "Display_Name";
        dropCard_Type.DataValueField = "RID";
        dropCard_Type.DataSource = ctmManager.GetCardTypeByGroupId(dropCard_Purpose.SelectedValue, dropCard_Group.SelectedValue,_Is_Using);
        dropCard_Type.DataBind();

        if (_CardTypeAll)
        {
            if (dropCard_Group.SelectedValue == "" && dropCard_Purpose.SelectedValue == "")
                dropCard_Type.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        }
    }
}
