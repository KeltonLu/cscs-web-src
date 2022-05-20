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

public partial class CommUserCtrl_uctrlCardType : System.Web.UI.UserControl
{
    CardTypeManager ctmManager = new CardTypeManager();
    private string _SetLeftItem="";
    public delegate void RightItemChange(object sender, EventArgs e);
    public event RightItemChange RightChange;
    private bool _AutoPostBack;
    private int _RightMaxLenght=0;
    private bool _Is_Using=false;
    private bool _ChangeName = false;

    public bool Is_Using
    {
        set
        {
            _Is_Using = value;
        }
    }

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

    public bool AutoPostBack
    {
        get
        {
            return _AutoPostBack;
        }
        set
        {
            _AutoPostBack = value;
        }
    }


    /// <summary>
    /// 設置已選卡種
    /// </summary>
    public DataTable SetRightItem
    {
        set
        {
            DataTable dtblRight = value;

            LbRight.DataTextField = "NAME";
            LbRight.DataValueField = "RID";
            LbRight.DataSource = dtblRight;
            LbRight.DataBind();

            DelLeftItem();
        }
    }

    public bool ChangeName
    {
        set
        {
            if (value)
            {
                _ChangeName = true;
                lblLeftName.Text = "版面簡稱";
                lblRightName.Text = "已選擇版面簡稱";
            }
        }
    }

    public bool Enable
    {
        set
        {
            if (value)
            {
                dropCard_Purpose.Enabled = true;
                dropCard_Group.Enabled = true;
                LbLeft.Enabled = true;
                LbRight.Enabled = true;
                btnSelectAll.Enabled = true;
                btnSelect.Enabled = true;
                btnRemove.Enabled = true;
                btnRemoveAll.Enabled = true;
            }
            else
            {
                dropCard_Purpose.Enabled = false;
                dropCard_Group.Enabled = false;
                LbLeft.Enabled = false;
                LbRight.Enabled = false;
                btnSelectAll.Enabled = false;
                btnSelect.Enabled = false;
                btnRemove.Enabled = false;
                btnRemoveAll.Enabled = false;
            }
        }
    }

    public string SetLeftItem
    {
        set
        {
            _SetLeftItem = value;
        }
    }

    /// <summary>
    /// 設置長度
    /// </summary>
    public int SetWidth
    {
        set
        {
            LbLeft.Width = value;
            LbRight.Width = value;
        }
    }

    /// <summary>
    /// 設置寬度
    /// </summary>
    public int SetHeight
    {
        set
        {
            LbLeft.Height = value;
            LbRight.Height = value;
        }
    }

    /// <summary>
    /// 獲取已選卡種
    /// </summary>
    public DataTable GetRightItem
    {
        get
        {
            DataTable dtblCardType = new DataTable();
            dtblCardType.Columns.Add("RID");
            dtblCardType.Columns.Add("NAME");

            foreach (ListItem li in LbRight.Items)
            {
                DataRow drowCardType = dtblCardType.NewRow();
                drowCardType[0] = li.Value;
                drowCardType[1] = li.Text;
                dtblCardType.Rows.Add(drowCardType);
            }

            return dtblCardType;
        }
    }

    public void LeftItemClear()
    {
        LbLeft.Items.Clear();
    }

    public void RightItemClear()
    {
        LbRight.Items.Clear();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dropCard_PurposeBind();
            dropCard_GroupBind();
            LbLeftBind();

            DelLeftItem();
        }
    }

    protected void dropCard_Purpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropCard_GroupBind();
        LbLeftBind();

        DelLeftItem();
    }

    protected void DelLeftItem()
    {
        foreach (ListItem li in LbRight.Items)
        {
            if (LbLeft.Items.FindByValue(li.Value)!=null)
                LbLeft.Items.Remove(li);
        }
    }

    protected void dropCard_Group_SelectedIndexChanged(object sender, EventArgs e)
    {
        LbLeftBind();

        DelLeftItem();
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        foreach (ListItem li in LbLeft.Items)
        {
            if (_RightMaxLenght != 0)
            {
                if (LbRight.Items.Count >= _RightMaxLenght)
                    break;
            }
            LbRight.Items.Add(li);
        }
        LbLeft.Items.Clear();


        //for (int i = 0; i < LbLeft.Items.Count; i++)
        //{
        //    if (_RightMaxLenght != 0)
        //    {
        //        if (LbRight.Items.Count >= _RightMaxLenght)
        //            break;
        //    }
        //    LbRight.Items.Add(LbLeft.Items[i]);
        //}
        //DelLeftItem();
        //LbLeft.Items.Clear();

        OnChanged(sender, e);
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        ArrayList al = new ArrayList();

        foreach (ListItem li in LbLeft.Items)
        {
            if (li.Selected)
            {
                if (_RightMaxLenght != 0)
                {
                    if (LbRight.Items.Count >= _RightMaxLenght)
                        break;
                }
                al.Add(li.Value);
                LbRight.Items.Add(li);
            }
        }

        foreach (string str in al)
        {
            if (LbLeft.Items.FindByValue(str)!=null)
                LbLeft.Items.Remove(LbLeft.Items.FindByValue(str));
        }


        //int i = 0;
        //while (i < LbLeft.Items.Count)
        //{
        //    if (LbLeft.Items[i].Selected == true)
        //    {
        //        if (_RightMaxLenght != 0)
        //        {
        //            if (LbRight.Items.Count >= _RightMaxLenght)
        //                break;
        //        }

        //        LbRight.Items.Add(LbLeft.Items[i]);
        //        LbLeft.Items.Remove(LbLeft.Items[i]);
        //    }
        //    else
        //        i += 1;
        //}
        //DelLeftItem();
        OnChanged(sender, e);
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        DataSet dsltCardType = (DataSet)ViewState["CardType"];
        ArrayList al = new ArrayList();

        foreach (ListItem li in LbRight.Items)
        {
            if (li.Selected == true)
            {
                if (dsltCardType.Tables[0].Select("RID=" + li.Value).Length > 0)
                {
                    LbLeft.Items.Add(li);
                }
                al.Add(li.Value);
            }
        }
        foreach (string str in al)
        {
            if (LbRight.Items.FindByValue(str) != null)
                LbRight.Items.Remove(LbRight.Items.FindByValue(str));
        }

        //int i = 0;
        //while (i < LbRight.Items.Count)
        //{
        //    if (LbRight.Items[i].Selected == true)
        //    {
        //        foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
        //        {
        //            if (LbRight.Items[i].Value == drowCardType["RID"].ToString())
        //            {
        //                LbLeft.Items.Add(LbRight.Items[i]);
        //                break;
        //            }
        //        }
        //        LbRight.Items.Remove(LbRight.Items[i]);
        //    }
        //    else
        //        i += 1;
        //}

        OnChanged(sender, e);
    }

    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        DataSet dsltCardType = (DataSet)ViewState["CardType"];

        foreach (ListItem li in LbRight.Items)
        {
            if (dsltCardType.Tables[0].Select("RID=" + li.Value).Length > 0)
            {
                LbLeft.Items.Add(li);
            }
        }

        //for (int i = 0; i < LbRight.Items.Count; i++)
        //{
        //    foreach (DataRow drowCardType in dsltCardType.Tables[0].Rows)
        //    {
        //        if (LbRight.Items[i].Value == drowCardType["RID"].ToString())
        //        {
        //            LbLeft.Items.Add(LbRight.Items[i]);
        //            break;
        //        }
        //    }
        //}
        LbRight.Items.Clear();

        OnChanged(sender, e);
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
    public void LbLeftBind()
    {
        LbLeft.Items.Clear();

        DataSet dsltCardType = ctmManager.GetCardTypeByGroupId(dropCard_Purpose.SelectedValue, dropCard_Group.SelectedValue, _SetLeftItem, _Is_Using);

        ViewState["CardType"] = dsltCardType;

        if (_ChangeName)
            LbLeft.DataTextField = "NAME";
        else
            LbLeft.DataTextField = "Display_Name";

        LbLeft.DataValueField = "RID";
        LbLeft.DataSource = dsltCardType;
        LbLeft.DataBind();

        DelLeftItem();
    }

    protected virtual void OnChanged(object sender, EventArgs e)
    {
        if (_AutoPostBack)
        {
            if (RightChange != null)
            {
                RightChange(sender, e);
            }
        }
    }
}
