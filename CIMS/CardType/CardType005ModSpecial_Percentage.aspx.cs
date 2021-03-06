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

public partial class CardType_CardType005ModSpecial_Percentage : PageBase
{
    CardType005BL bl = new CardType005BL();
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";
        if (!IsPostBack)
        {
            // 獲取 Perso廠商資料
            DataSet dstFactory = bl.GetFactoryList();
            dropFactory.DataValueField = "RID";
            dropFactory.DataTextField = "Factory_ShortName_CN";
            dropFactory.DataSource = dstFactory.Tables[0];
            dropFactory.DataBind();
            //dropFactory.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));

            DataTable dtPercentage = (DataTable)Session["Percentage_Number"];
            foreach (DataRow drowPercentage in dtPercentage.Rows)
            {
                if (dropFactory.Items.FindByValue(drowPercentage["Factory_RID"].ToString()) != null)

                    dropFactory.Items.Remove(dropFactory.Items.FindByValue(drowPercentage["Factory_RID"].ToString()));
            }
            string strPriority = Request.QueryString["Priority"];
            if (!StringUtil.IsEmpty(strPriority))
            {
                DataRow drowPriority = dtPercentage.Rows[int.Parse(strPriority)];
                if (dropFactory.Items.FindByValue(drowPriority["Factory_RID"].ToString()) == null)
                {
                    dropFactory.Items.Add(new ListItem(bl.GetFactory_RID(drowPriority["Factory_RID"].ToString()), drowPriority["Factory_RID"].ToString()));
                }
            }
            if (Request.QueryString["type"] == "update")
            {
                //綁定資料
               
                DataTable dtNumber = (DataTable)Session["Percentage_Number"];
                DataRow drowNumber = dtNumber.Rows[int.Parse(strPriority)];
                txtPercentageValue.Text = drowNumber["Value"].ToString();
                dropFactory.SelectedValue = drowNumber["Factory_RID"].ToString();
            }
        }
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        DataTable dtPercentage = (DataTable)Session["Percentage_Number"];
        DataRow drowPercentage = null;
        int Value = 0;
        if (dropFactory.SelectedValue == "")
        {
            ShowMessage("沒有可以設定的Perso廠！");
            return;
        }
        if (Request.QueryString["type"] == "update")//修改
        {
            //綁定資料
            string strPriority = Request.QueryString["Priority"];
            for (int i = 0; i < dtPercentage.Rows.Count; i++)
            {
                if (i == int.Parse(strPriority))
                {
                    continue;
                }
                Value += Convert.ToInt16(dtPercentage.Rows[i]["Value"]);
            }
            Value += Convert.ToInt16(txtPercentageValue.Text);
            if (Value > 100)
            {
                ShowMessage("各Perso廠的比率之和不能大於100%！");
                return;
            }
            drowPercentage = dtPercentage.Rows[int.Parse(strPriority)];
            drowPercentage["Factory_RID"] = dropFactory.SelectedValue;
            drowPercentage["Value"] = txtPercentageValue.Text;
        }
        else
        {
            foreach (DataRow drowPercentageValue in dtPercentage.Rows)
            {
                if (drowPercentageValue["Value"].ToString() != null)

                    Value += Convert.ToInt16(drowPercentageValue["Value"]);
            }
            Value += Convert.ToInt16(txtPercentageValue.Text);
            if (Value > 100)
            {
                ShowMessage("各Perso廠的比率之和不能大於100%！");
                return;
            }
            drowPercentage = dtPercentage.NewRow();

            drowPercentage["Factory_RID"] = dropFactory.SelectedValue;
            drowPercentage["Value"] = txtPercentageValue.Text;
            dtPercentage.Rows.Add(drowPercentage);
        }
        Session["Percentage_Number"] = dtPercentage;
        Response.Write("<script>returnValue='1';window.close();</script>");
    }
}
