//******************************************************************
//*  作    者：QingChen
//*  功能說明：對話框、提示框控制項
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;

namespace ControlLibrary
{
    public class ConfirmValidator : CustomValidator
    {
        /// <summary>
        /// 建構
        /// </summary>
        public ConfirmValidator()
            : base()
        {
            base.ClientValidationFunction = "ConfirmValidator_function";
            base.Display = ValidatorDisplay.None;
            
        }

        /// <summary>
        /// 重寫初始化
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        /// <summary>
        /// 重寫加載
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                StringBuilder scriptBuilder = new StringBuilder();
                scriptBuilder.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
                scriptBuilder.AppendLine(" function ConfirmValidator_function(source,arguments)");
                scriptBuilder.AppendLine(" { arguments.IsValid=window.confirm(source.errormessage);}");
                scriptBuilder.AppendLine("</script> ");
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(ConfirmValidator), "ConfirmValidator_Script", scriptBuilder.ToString());
            }
        }
        /// <summary>
        /// 重寫繪製
        /// </summary>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.ClientValidationFunction = "ConfirmValidator_function";
            base.Display = ValidatorDisplay.None;
            base.Render(writer);
        }

        /// <summary>
        /// 重訂向地址
        /// </summary>
        public string PageUrl { get { return pageUrl; } set { pageUrl = value; } }
        private string pageUrl = "";
        /// <summary>
        /// 依屬性顯示提示並重訂向
        /// </summary>
        public void ShowMessageAndGoPage()
        {
            if (pageUrl != "")
            {
                ShowMessageAndGoPage(this.Text, pageUrl);
            }
            else
            {
                ShowMessageAndGoPage(this.Text, Page.Request.Url.ToString());
            }
        }
        /// <summary>
        /// 依參數顯示提示並重訂向
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="Url">重訂向地址</param>
        public void ShowMessageAndGoPage(string message, string Url)
        {
            
            this.Page.Response.Write("<script language=\"javascript\" type=\"text/ecmascript\">alert('" + message + "');window.location='" + Url + "';</script>");
            this.Page.Response.End();
            
        }

        /// <summary>
        /// 依參數顯示提示
        /// </summary>
        /// <param name="message">提示信息</param>
        public void ShowMessage(string message)
        {
            this.Page.Response.Write("<script language=\"javascript\" type=\"text/ecmascript\">alert('" + message + "');</script>");
        }

        /// <summary>
        /// 依參數顯示提示
        /// </summary>
        public void ShowMessage()
        {
            ShowMessage(this.Text);
        }
    }
}
