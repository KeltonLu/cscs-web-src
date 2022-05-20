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

public partial class CardType_CardType005SpecialDetail_Number : PageBase
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
                if (dropFactory.Items.FindByValue(drowPercentage["Factory"].ToString()) != null)

                    dropFactory.Items.Remove(dropFactory.Items.FindByValue(drowPercentage["Factory"].ToString()));
            }
            string strPriority = Request.QueryString["Priority"];
            if (!StringUtil.IsEmpty(strPriority))
            {
                DataRow drowPriority = dtPercentage.Rows[int.Parse(strPriority)];
                if (dropFactory.Items.FindByValue(drowPriority["Factory"].ToString()) == null)
                {
                    dropFactory.Items.Add(new ListItem(bl.GetFactory_RID(drowPriority["Factory"].ToString()), drowPriority["Factory"].ToString()));
                }
            }
            if (Request.QueryString["type"] == "update")
            {
                //綁定資料
                
                DataTable dtNumber = (DataTable)Session["Percentage_Number"];
                DataRow drowNumber = dtNumber.Rows[int.Parse(strPriority)];
                txtNumberValue.Text = drowNumber["Value"].ToString();
                dropFactory.SelectedValue = drowNumber["Factory"].ToString();
            }
        }
    }
    protected void btnSubmit1_Click(object sender, EventArgs e)
    {
        DataTable dtPercentage = (DataTable)Session["Percentage_Number"];
        DataRow drowPercentage = null;
        txtNumberValue.Text = txtNumberValue.Text.Trim();
        if (dropFactory.SelectedValue == "")
        {
            ShowMessage("沒有可以設定的Perso廠！");
            return;
        }
        if (Request.QueryString["type"] == "update")//修改
        {
            foreach (DataRow drowPercentageValue in dtPercentage.Rows)
            {
                if (drowPercentageValue["Value"].ToString() == "0" && txtNumberValue.Text == "" || txtNumberValue.Text == "0")
                {
                    ShowMessage("已存在數量為空的Perso廠！");
                    return;
                }
            }
            //綁定資料
            string strPriority = Request.QueryString["Priority"];
            drowPercentage = dtPercentage.Rows[int.Parse(strPriority)];
            drowPercentage["Factory"] = dropFactory.SelectedValue;
            if (txtNumberValue.Text == "")
            {
                drowPercentage["Value"] = 0;
            }
            else
            {
                drowPercentage["Value"] = txtNumberValue.Text;
            }
        }
        else
        {
            foreach (DataRow drowPercentageValue in dtPercentage.Rows)
            {
                if (drowPercentageValue["Value"].ToString() == "0")
                {
                    ShowMessage("已存在數量為空的Perso廠！");
                    return;
                }
            }
            drowPercentage = dtPercentage.NewRow();

            drowPercentage["Factory"] = dropFactory.SelectedValue;
            if (txtNumberValue.Text == "")
            {
                drowPercentage["Value"] = 0;
            }
            else
            {
                drowPercentage["Value"] = txtNumberValue.Text;
            }
            dtPercentage.Rows.Add(drowPercentage);
        }
        
        Session["Percentage_Number"] = dtPercentage;

        Response.Write("<script>returnValue='1';window.close();</script>");
    }
}
