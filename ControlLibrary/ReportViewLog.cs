using ControlLibrary.Properties;
using Microsoft.Reporting.WebForms;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web.UI.WebControls;

namespace ControlLibrary
{
    public class ReportViewLog : ReportViewer
    {
        public event OnReportViewOtherEventDelegate ReportViewOtherEvent = default(OnReportViewOtherEventDelegate);
        private ImageButton ibtPrint = null;
        private WebControl wctrlExport = null;
        private WebControl wctrlExportDrop = null;
        private WebControl wctrlToolBar = null;
        private string strExportText = "匯出";
        [DefaultValue("匯出")]
        public string ExportText { get { return strExportText; } set { strExportText = value; } }
        public ReportViewLog()
            : base()
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

                if (ibtPrint == null)
                {
                    wctrlToolBar = (WebControl)Controls[1];
                    wctrlExportDrop = (WebControl)wctrlToolBar.Controls[5].Controls[0];
                    wctrlExport = (WebControl)wctrlToolBar.Controls[5].Controls[1];
                    ibtPrint = (ImageButton)wctrlToolBar.Controls[7].Controls[0].Controls[0].Controls[0];
                    ibtPrint.PreRender += new EventHandler(ibtPrint_PreRender);
                }
                if (this.ShowExportControls)
                {
                    System.Web.UI.WebControls.Literal l = new Literal();
                    //StringBuilder stbCode = new StringBuilder();
                    string strURL = "http://" + Page.Request.Url.Authority + Page.Request.Url.AbsolutePath;
                    l.Text = "<a id=\"" + wctrlExport.ClientID + "\" onclick=\"if (document.getElementById('" + wctrlExportDrop.ClientID + "').selectedIndex == 0) return false; if (!ClientToolbar" + wctrlToolBar.ClientID + ".HandleClientSideExport()) __doPostBack('" + wctrlExport.UniqueID + "','');ReportViewSaveLog('" + this.ClientID + "','Export','" + strURL + "');return false;\" onmouseover=\"TextLink" + wctrlExport.ClientID + ".OnLinkHover();\" onmouseout=\"TextLink" + wctrlExport.ClientID + ".OnLinkNormal();\" title=\"" + strExportText + "\" href=\"#\" style=\"font-family:Verdana;font-size:8pt;color:Gray;text-decoration:none;\">" + strExportText + "</a>";
                    wctrlExport.Parent.Controls.AddAt(1, l);
                    wctrlExport.Visible = false;

                }


        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (Page.Request.Form["QueryCtrl"] == this.ClientID)
                {
                    string strData = Page.Request.Form["QueryData"];
                    if (strData == "Export" || strData == "Print")
                    {
                        ReportViewOtherEventArgs eData = new ReportViewOtherEventArgs(strData);
                        if (ReportViewOtherEvent != default(OnReportViewOtherEventDelegate))
                        {
                            ReportViewOtherEvent(this, eData);
                            Page.Response.End();
                        }
                    }
                }
            }
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ReportViewSaveLog", Properties.Resources.ReportViewLogScript, true);
        }
        void ibtPrint_PreRender(object sender, EventArgs e)
        {
            if (ShowPrintButton)
            {
                if (ibtPrint.Attributes["onclick"] != null)
                {

                    string[] strCodes = ibtPrint.Attributes["onclick"].Split(';');
                    string strURL = "http://" + Page.Request.Url.Authority + Page.Request.Url.AbsolutePath;
                    ibtPrint.Attributes["onclick"] = strCodes[0] + ";ReportViewSaveLog('" + this.ClientID + "','Print','" + strURL + "');" + strCodes[1];
                }
                else if (((WebControl)ibtPrint.Parent).Attributes["onclick"] != null)
                {
                    string[] strCodes = ((WebControl)ibtPrint.Parent).Attributes["onclick"].Split(';');
                    string strURL = "http://" + Page.Request.Url.Authority + Page.Request.Url.AbsolutePath;
                    ((WebControl)ibtPrint.Parent).Attributes["onclick"] = strCodes[0] + ";ReportViewSaveLog('" + this.ClientID + "','Print','" + strURL + "');" + strCodes[1];
                }
               
            }
           
        }
    }
    /// <summary>
    /// 綁定資料事件的委託
    /// </summary>
    /// <param name="sender">事件物件</param>
    /// <param name="e">事件參數</param>
    public delegate void OnReportViewOtherEventDelegate(object sender, ReportViewOtherEventArgs e);
    public class ReportViewOtherEventArgs : EventArgs
    {
        private string strActionType = string.Empty;
        public string ActionType { get { return strActionType; } }
        public ReportViewOtherEventArgs(string type)
            : base()
        {
            strActionType = type;
        }
    }
}

