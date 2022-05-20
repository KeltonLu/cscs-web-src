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
using System.Text;
using System.Collections.Generic;

public partial class CardType_CardType002Img : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!Page.IsPostBack)
        {
            try
            {
                string strActionType = Request.QueryString["ActionType"];
                string strRID = Request.QueryString["RID"];
                if (!StringUtil.IsEmpty(strRID.Trim()))
                {
                    DataTable dtImg = null;
                    if ("ADD" == strActionType.ToUpper())
                    {
                        dtImg = (DataTable)this.Session["CardType002Img"];
                    }
                    else
                    { 
                        dtImg = (DataTable)this.Session["CardType002ModImg"];
                    }
                    //string path = ConfigurationManager.AppSettings["WebName"] + Convert.ToString(dtImg.Rows[Convert.ToInt32(strRID)]["File_Name"]);
                    string path = dtImg.Rows[Convert.ToInt32(strRID.ToString())]["IMG_File_URL"].ToString();
                    imgFileUrl.ImageUrl = path;
                }
            }
            catch (Exception ex)
            { 
                ShowMessage(ex.Message);
            }
        }
    }
}
