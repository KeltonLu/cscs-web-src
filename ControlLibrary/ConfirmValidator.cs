//******************************************************************
//*  �@    �̡GQingChen
//*  �\�໡���G��ܮءB���ܮر��
//*  �Ыؤ���G2008/05/21
//*  �ק����G2008/05/21  16:59
//*  �ק�O���G
//*            ��2008/05/21 
//*              1.�Ы� ���C
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
        /// �غc
        /// </summary>
        public ConfirmValidator()
            : base()
        {
            base.ClientValidationFunction = "ConfirmValidator_function";
            base.Display = ValidatorDisplay.None;
            
        }

        /// <summary>
        /// ���g��l��
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        /// <summary>
        /// ���g�[��
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
        /// ���gø�s
        /// </summary>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.ClientValidationFunction = "ConfirmValidator_function";
            base.Display = ValidatorDisplay.None;
            base.Render(writer);
        }

        /// <summary>
        /// ���q�V�a�}
        /// </summary>
        public string PageUrl { get { return pageUrl; } set { pageUrl = value; } }
        private string pageUrl = "";
        /// <summary>
        /// ���ݩ���ܴ��ܨí��q�V
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
        /// �̰Ѽ���ܴ��ܨí��q�V
        /// </summary>
        /// <param name="message">���ܫH��</param>
        /// <param name="Url">���q�V�a�}</param>
        public void ShowMessageAndGoPage(string message, string Url)
        {
            
            this.Page.Response.Write("<script language=\"javascript\" type=\"text/ecmascript\">alert('" + message + "');window.location='" + Url + "';</script>");
            this.Page.Response.End();
            
        }

        /// <summary>
        /// �̰Ѽ���ܴ���
        /// </summary>
        /// <param name="message">���ܫH��</param>
        public void ShowMessage(string message)
        {
            this.Page.Response.Write("<script language=\"javascript\" type=\"text/ecmascript\">alert('" + message + "');</script>");
        }

        /// <summary>
        /// �̰Ѽ���ܴ���
        /// </summary>
        public void ShowMessage()
        {
            ShowMessage(this.Text);
        }
    }
}
