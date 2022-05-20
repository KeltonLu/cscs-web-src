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

public partial class Depository_Depository004Edit : PageBase
{
    Depository004BL bizlogic = new Depository004BL();

    #region 事務控製
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string strRID = Request.QueryString["RID"];
            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strRID))
            {
               
                DataSet dsldepository = bizlogic.LoadInfoByRid(strRID);
                DataTable dtbDepository = dsldepository.Tables[0];
                lblIncome_Number.Text = dtbDepository.Rows[0]["Income_Number"].ToString();
                lblName.Text = dtbDepository.Rows[0]["Name"].ToString();
                lblStork_Id.Text = dtbDepository.Rows[0]["Stock_RID"].ToString();
                lblCancelDate.Text = dtbDepository.Rows[0]["Cancel_Date"].ToString();  
                txtComment.Text = dtbDepository.Rows[0]["Comment"].ToString().Trim();
                txtCancel_number.Text = dtbDepository.Rows[0]["Cancel_Number"].ToString();
                if (dtbDepository.Rows[0]["Is_AskFinance"].ToString().Equals("Y") || dtbDepository.Rows[0]["Is_Check"].ToString() == "Y")
                {
                    btnEdit.Enabled = false;
                    btnEdit1.Enabled = false;
                }      
                //製域處理
                //trDel.Visible = true;

                if (this.IsCheck())
                {
                    ShowMessage("今天已經日結，不可修改退貨信息");
                    btnEdit.Enabled = false;
                    btnEdit1.Enabled = false;
                }
            }
        }

    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
       
        string strRID = Request.QueryString["RID"];

        try
        {
            if (chkDel.Checked)         //刪除
            {
                bizlogic.Delete(strRID);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Depository004.aspx?Con=1");
            }
            else                        //修改 
            {
                if (StringUtil.GetByteLength(txtComment.Text.Trim()) > 100)
                {
                    ShowMessage("備註不能超過100個字符");
                    return;
                }

                //判斷退貨量必須小於入庫量
                if (Convert.ToInt32(lblIncome_Number.Text) < Convert.ToInt32(txtCancel_number.Text.Replace(",","").Trim()))
                {
                    ShowMessage("退貨量必須小於入庫量！");
                    return;
                }
                
                string comment = txtComment.Text;
                string concelNumber = txtCancel_number.Text.Replace(",","");
                if (concelNumber == "0") {
                    ShowMessage("退貨量必須大於0！");
                    return;
                }        
                bizlogic.Update(strRID, comment, concelNumber);
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Depository004.aspx?Con=1");
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Depository004.aspx?Con=1");
    }

    protected void btnEdit1_Click(object sender, EventArgs e)
    {
        btnEdit_Click(sender, e);
    }
    protected void btnCancel1_Click(object sender, EventArgs e)
    {
        btnCancel_Click(sender, e);
    }
    #endregion

}
