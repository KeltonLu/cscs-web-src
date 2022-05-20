//******************************************************************
//*  �@    �̡GQingChen
//*  �\�໡���G�����q�{���s���
//*  �Ыؤ���G2008/05/21
//*  �ק����G2008/05/21  16:59
//*  �ק�O���G
//*            ��2008/05/21 
//*              1.�Ы� ���C
//*******************************************************************
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ControlLibrary
{
    public class DefaultSet:WebControl
    {
        private string defaultButton = null;

        private string defaultFocus = null;

        public string DefaultButton
        {
            get
            {
                return this.defaultButton;
            }
            set
            {
                this.defaultButton = value;
                if (!base.DesignMode)
                {
                    if (this.Page != null && this.Page.Form != null && this.defaultButton != null)
                    {
                        this.Page.Form.DefaultButton = this.defaultButton;
                    }
                }
            }
        }

        public string DefaultFocus
        {
            get
            {
                return this.defaultFocus;
            }
            set
            {
                this.defaultFocus = value;
                if (!base.DesignMode)
                {
                    if (this.Page != null && this.Page.Form != null && this.defaultFocus != null)
                    {
                        this.Page.Form.DefaultFocus = this.defaultFocus;
                    }
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (base.DesignMode)
            {
                writer.WriteLine("<div>" + this.ID + "<div>");
            }
            else
            {
                base.Render(writer);
            }
        }
    }
}
