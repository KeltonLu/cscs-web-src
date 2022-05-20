//******************************************************************
//*  作    者：QingChen
//*  功能說明：AJAX驗證控制項
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using ControlLibrary.Properties;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ControlLibrary
{
    public delegate void OnAjaxValidatorDelegate(object sender, AjaxValidatorEventArgs e);
    
    public class AjaxValidator:CustomValidator
    {
        private string queryInfo;

        /// <summary>
        /// 請求驗證事件
        /// </summary>
        public event OnAjaxValidatorDelegate OnAjaxValidatorQuest = default(OnAjaxValidatorDelegate);

        [DefaultValue(null)]
        public string QueryInfo
        {
            get
            {
                return this.queryInfo;
            }
            set
            {
                this.queryInfo = value;
            }
        }

        public AjaxValidator()
        { 
            base.ClientValidationFunction = "AjaxValidatorFunction";
        }

        protected override void OnInit(EventArgs e)
        {
            if (!base.Page.IsPostBack)
            {
                string text = base.Page.Request.Form["QueryCtrl"];
                string text2 = base.Page.Request.Form.ToString();
                if (text != null && text == this.UniqueID.Replace("$", "_"))
                {
                    if (this.OnAjaxValidatorQuest != null)
                    {
                        string queryData = base.Page.Request.Form["QueryData"];
                        AjaxValidatorEventArgs ajaxValidatorEventArgs = new AjaxValidatorEventArgs(queryData);
                        string text3 = base.Page.Request.Form["QueryInfo"];
                        if (text3 != null)
                        {
                            ajaxValidatorEventArgs.QueryInfo = this.Page.Server.HtmlDecode(text3);
                        }
                        this.OnAjaxValidatorQuest(this, ajaxValidatorEventArgs);
                        if (ajaxValidatorEventArgs.IsAllowSubmit)
                        {
                            base.Page.Response.Write("Y");
                        }
                        else
                        {
                            base.Page.Response.Write("N");
                        }
                    }
                    base.Page.Response.End();
                }
            }
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "AjaxValidator", Resources.AjaxValidatorScript, true);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!base.DesignMode)
            {
                base.Attributes.Add("AjaxUrl", "http://" + this.Page.Request.Url.Authority + this.Page.Request.Url.AbsolutePath);
                if (this.queryInfo != null)
                {
                    base.Attributes.Add("QueryInfo", this.Page.Server.HtmlEncode(this.queryInfo));
                }
            }
            base.Render(writer);
        }
    }

    public class AjaxValidatorEventArgs : EventArgs
    {
        private bool isAllowSubmit = false;

        private string info = "";

        private string queryData = null;

        private string queryInfo = null;

        [DefaultValue(false)]
        public bool IsAllowSubmit
        {
            get
            {
                return this.isAllowSubmit;
            }
            set
            {
                this.isAllowSubmit = value;
            }
        }

        [DefaultValue("")]
        public string Info
        {
            get
            {
                return this.info;
            }
            set
            {
                this.info = value;
            }
        }

        [DefaultValue(null)]
        public string QueryData
        {
            get
            {
                return this.queryData;
            }
        }

        [DefaultValue(null)]
        public string QueryInfo
        {
            get
            {
                return this.queryInfo;
            }
            set
            {
                this.queryInfo = value;
            }
        }

        public AjaxValidatorEventArgs(string _queryData)
        {
            this.queryData = _queryData;
        }
    }
}
