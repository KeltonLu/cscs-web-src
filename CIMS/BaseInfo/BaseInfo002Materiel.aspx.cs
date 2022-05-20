//******************************************************************
//*  作    者：FangBao
//*  功能說明：合約材質管理
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

public partial class BaseInfo_BaseInfo002Materiel : PageBase
{
    BaseInfo002BL BL = new BaseInfo002BL();
    Dictionary<string, DataTable> dirAgreement = new Dictionary<string, DataTable>();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!IsPostBack)
        {
            dropMaterial_RID.DataSource = BL.MaterialList().Tables[0];
            dropMaterial_RID.DataBind();

            dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];

            foreach (DataRow drowMaterial in dirAgreement["MaterialTmp"].Rows)
            {
                if (dropMaterial_RID.Items.FindByValue(drowMaterial["Material_RID"].ToString()) != null)
                    dropMaterial_RID.Items.Remove(dropMaterial_RID.Items.FindByValue(drowMaterial["Material_RID"].ToString()));
            }

            string strRowId = Request.QueryString["RowId"];
            if (!StringUtil.IsEmpty(strRowId))
            {
                DataRow drowMaterial = dirAgreement["MaterialTmp"].Rows[int.Parse(strRowId)];
                if (dropMaterial_RID.Items.FindByValue(drowMaterial["Material_RID"].ToString()) == null)
                {
                    dropMaterial_RID.Items.Add(new ListItem(drowMaterial["Material_Name"].ToString(), drowMaterial["Material_RID"].ToString()));
                }

                dropMaterial_RID.SelectedValue = drowMaterial["Material_RID"].ToString();
                txtBase_Price.Text = Convert.ToDecimal(drowMaterial["Base_Price"]).ToString("N2");
            }
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (Convert.ToDecimal(txtBase_Price.Text) == 0)
        {
            ShowMessage("單價不能為0");
            return;
        }

        if (StringUtil.IsEmpty(dropMaterial_RID.SelectedValue))
        {
            ShowMessage("特殊材質不能為空");
            return;
        }

        string strRowId = Request.QueryString["RowId"];

        DataTable dtblMaterial = null;
        DataRow drowMaterial = null;

        dirAgreement = (Dictionary<string, DataTable>)Session["Agreement"];
        dtblMaterial = dirAgreement["MaterialTmp"];


        if (StringUtil.IsEmpty(strRowId))
            drowMaterial = dtblMaterial.NewRow();
        else
            drowMaterial = dtblMaterial.Rows[int.Parse(strRowId)];

        drowMaterial["Material_RID"] = dropMaterial_RID.SelectedValue;
        drowMaterial["Material_Name"] = dropMaterial_RID.SelectedItem.Text;
        drowMaterial["Base_Price"] = txtBase_Price.Text.Replace(",", "");

        if (StringUtil.IsEmpty(strRowId))
            dtblMaterial.Rows.Add(drowMaterial);

        Response.Write("<script>returnValue='1';window.close();</script>");
    }
    #endregion
}
