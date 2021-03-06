//******************************************************************
//*  作    者：QingChen
//*  功能說明：特殊交互表格
//*  創建日期：2008/05/27
//*  修改日期：2008/05/27  16:59
//*  修改記錄：
//*            □2008/05/27 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ControlLibrary
{
    /// <summary>
    /// 資料庫分頁表格
    /// </summary>
    public class GridViewPageingByDB : GridView, IPostBackEventHandler, IPostBackDataHandler
    {
        public event OnSetDataSourceDelegate OnSetDataSource = default(OnSetDataSourceDelegate);

        private List<HtmlInputCheckBox> checkBoxes;

        private string selectedIDS;

        private List<string> selectedIDList;

        private string editPageUrl;

        private string editPageUrlParameters;

        private bool showEditButton;

        private bool showCheckBox;

        private bool checkRowNumber;

        private string strPageString;

        private string columnsHeaderText;

        private string editCellWidth;

        private string strPageNumberTooLong;

        private string strPageNumberErr;

        private string strNoneData;

        private string editButtonText;

        private string editButtonImageUrl;

        private bool blnHasDataRow;

        private bool blnAllowPaging;

        /// <summary>
        /// 綁定資料事件的委託
        /// </summary>
        /// <param name="sender">事件物件</param>
        /// <param name="e">事件參數</param>
        public delegate void OnSetDataSourceDelegate(object sender, SetDataSourceEventArgs e);

        public string SelectedIDS
        {
            get
            {
                return this.selectedIDS;
            }
        }

        public string EditPageUrl
        {
            get
            {
                return this.editPageUrl;
            }
            set
            {
                this.editPageUrl = value;
            }
        }

        public string EditPageUrlParameters
        {
            get
            {
                return this.editPageUrlParameters;
            }
            set
            {
                this.editPageUrlParameters = value;
            }
        }

        [DefaultValue(false)]
        public bool ShowEditButton
        {
            get
            {
                return this.showEditButton;
            }
            set
            {
                this.showEditButton = value;
            }
        }

        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get
            {
                return this.showCheckBox;
            }
            set
            {
                this.showCheckBox = value;
            }
        }

        [DefaultValue(false)]
        public bool CheckRowNumber
        {
            get
            {
                return this.checkRowNumber;
            }
            set
            {
                this.checkRowNumber = value;
            }
        }

        [DefaultValue("頁"), Localizable(true)]
        public string PageString
        {
            get
            {
                return this.strPageString;
            }
            set
            {
                this.strPageString = value;
            }
        }

        [Localizable(true)]
        public string ColumnsHeaderText
        {
            get
            {
                return this.columnsHeaderText;
            }
            set
            {
                this.columnsHeaderText = value;
            }
        }

        public string EditCellWidth
        {
            get
            {
                return this.editCellWidth;
            }
            set
            {
                this.editCellWidth = value;
            }
        }

        [Localizable(true)]
        public string FirstPageText
        {
            get
            {
                return this.PagerSettings.FirstPageText;
            }
            set
            {
                this.PagerSettings.FirstPageText = value;
            }
        }

        [Localizable(true)]
        public string PreviousPageText
        {
            get
            {
                return this.PagerSettings.PreviousPageText;
            }
            set
            {
                this.PagerSettings.PreviousPageText = value;
            }
        }

        [DefaultValue("輸入頁碼超過最長頁碼！"), Localizable(true)]
        public string PageNumberTooLong
        {
            get
            {
                return this.strPageNumberTooLong;
            }
            set
            {
                this.strPageNumberTooLong = value;
            }
        }

        [DefaultValue("請輸入正確的頁碼！"), Localizable(true)]
        public string PageNumberErr
        {
            get
            {
                return this.strPageNumberErr;
            }
            set
            {
                this.strPageNumberErr = value;
            }
        }

        [DefaultValue("查無資料"), Localizable(true)]
        public string NoneData
        {
            get
            {
                return this.strNoneData;
            }
            set
            {
                this.strNoneData = value;
            }
        }

        [Localizable(true)]
        public string NextPageText
        {
            get
            {
                return this.PagerSettings.NextPageText;
            }
            set
            {
                this.PagerSettings.NextPageText = value;
            }
        }

        [Localizable(true)]
        public string LastPageText
        {
            get
            {
                return this.PagerSettings.LastPageText;
            }
            set
            {
                this.PagerSettings.LastPageText = value;
            }
        }

        [Localizable(true)]
        public string EditButtonText
        {
            get
            {
                return this.editButtonText;
            }
            set
            {
                this.editButtonText = value;
            }
        }

        public string EditButtonImageUrl
        {
            get
            {
                return this.editButtonImageUrl;
            }
            set
            {
                this.editButtonImageUrl = value;
            }
        }

        public new int PageCount
        {
            get
            {
                if (this.ViewState["PageCount"] == null)
                {
                    this.ViewState["PageCount"] = 0;
                }
                int num = (int)this.ViewState["PageCount"];
                return (int)this.ViewState["PageCount"];
            }
        }

        public new int PageIndex
        {
            get
            {
                if (this.ViewState["PageIndex"] == null)
                {
                    this.ViewState["PageIndex"] = 0;
                }
                return (int)this.ViewState["PageIndex"];
            }
            set
            {
                this.ViewState["PageIndex"] = value;
            }
        }

        public new bool AllowPaging
        {
            get
            {
                return this.blnAllowPaging;
            }
            set
            {
                this.blnAllowPaging = value;
            }
        }

        public new string SortExpression
        {
            get
            {
                if (this.ViewState["SortExpression"] == null)
                {
                    this.ViewState["SortExpression"] = "null";
                }
                return this.ViewState["SortExpression"].ToString();
            }
        }

        public new SortDirection SortDirection
        {
            get
            {
                if (this.ViewState["SortDirection"] == null)
                {
                    this.ViewState["SortDirection"] = SortDirection.Ascending;
                }
                return (SortDirection)this.ViewState["SortDirection"];
            }
        }

        [Category("排序"), DefaultValue(""), Description("升序"), Editor("System.Web.UI.Design.UrlEditor", typeof(UITypeEditor))]
        public string SortAscImageUrl
        {
            get
            {
                object obj = this.ViewState["SortImageAsc"];
                return (obj != null) ? obj.ToString() : "";
            }
            set
            {
                this.ViewState["SortImageAsc"] = value;
            }
        }

        [Category("排序"), DefaultValue(""), Description("降序"), Editor("System.Web.UI.Design.UrlEditor", typeof(UITypeEditor))]
        public string SortDescImageUrl
        {
            get
            {
                object obj = this.ViewState["SortImageDesc"];
                return (obj != null) ? obj.ToString() : "";
            }
            set
            {
                this.ViewState["SortImageDesc"] = value;
            }
        }

        public GridViewPageingByDB()
        {
            this.OnSetDataSource = null;
            this.checkBoxes = new List<HtmlInputCheckBox>();
            this.selectedIDS = string.Empty;
            this.selectedIDList = new List<string>();
            this.showEditButton = false;
            this.showCheckBox = false;
            this.checkRowNumber = false;
            this.strPageString = "頁";
            this.columnsHeaderText = string.Empty;
            this.editCellWidth = "48px";
            this.strPageNumberTooLong = "輸入頁碼超過最長頁碼！";
            this.strPageNumberErr = "請輸入正確的頁碼！";
            this.strNoneData = "查無資料";
            this.editButtonImageUrl = string.Empty;
            this.blnHasDataRow = false; 
            base.RowDataBound += new GridViewRowEventHandler(this.GridViewPageingByDB_RowDataBound);
            this.editButtonText = "編輯";
            this.editPageUrl = "#";
            base.Load += new EventHandler(this.GridViewPageingByDB_Load);
            base.AllowSorting = false;
            base.PagerSettings.Visible = false;
            base.AllowPaging = false;
            base.Sorting += new GridViewSortEventHandler(this.GridViewPageingByDB_Sorting);
            this.NextPageText = "<";
            this.PreviousPageText = ">";
            this.FirstPageText = "<<";
            this.LastPageText = ">>";
        }

        private void GridViewPageingByDB_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.SetData();
        }

        private void GridViewPageingByDB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!base.DesignMode)
            {
                if (this.DataSource != null && ((this.DataSource.GetType() == typeof(DataSet) && this.DataMember != null && this.DataMember != string.Empty && this.DataMember != "") || this.DataSource.GetType() != typeof(DataSet)))
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        if (this.blnHasDataRow)
                        {
                            DataRowView dataRowView = (DataRowView)e.Row.DataItem;
                            if (this.showCheckBox && this.blnHasDataRow)
                            {
                                if (this.DataKeyNames != null && this.DataKeyNames.Length <= 0)
                                {
                                    throw new Exception("請設置" + this.ID + "的DataKeyNames屬性");
                                }
                                string text = "";
                                for (int i = 0; i < this.DataKeyNames.Length; i++)
                                {
                                    if (dataRowView[this.DataKeyNames[i].ToString()].GetType() == typeof(DateTime))
                                    {
                                        text = text + Convert.ToDateTime(dataRowView[this.DataKeyNames[i].ToString()]).ToString("yyyy-MM-dd HH:mm:ss.fff") + ";";
                                    }
                                    else
                                    {
                                        text = text + dataRowView[this.DataKeyNames[i].ToString()].ToString().Trim() + ";";
                                    }
                                }
                                text = text.Substring(0, text.Length - 1);
                                if (this.checkRowNumber)
                                {
                                    text = text + ";" + e.Row.RowIndex.ToString();
                                }
                                if (this.selectedIDList.Contains(text))
                                {
                                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("value=\"\"", "value=\"" + text + "\" checked=\"checked\"");
                                }
                                else
                                {
                                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("value=\"\"", "value=\"" + text + "\"");
                                }
                            }
                            if (this.showEditButton && this.blnHasDataRow)
                            {
                                if (this.DataKeyNames != null && this.DataKeyNames.Length <= 0)
                                {
                                    throw new Exception("請設置" + this.ID + "的DataKeyNames屬性");
                                }
                                string text = "";
                                for (int i = 0; i < this.DataKeyNames.Length; i++)
                                {
                                    if (dataRowView[this.DataKeyNames[i].ToString()].GetType() == typeof(DateTime))
                                    {
                                        text = text + Convert.ToDateTime(dataRowView[this.DataKeyNames[i].ToString()]).ToString("yyyy-MM-dd HH:mm:ss.fff") + ";";
                                    }
                                    else
                                    {
                                        text = text + dataRowView[this.DataKeyNames[i].ToString()].ToString().Trim() + ";";
                                    }
                                }
                                text = text.Substring(0, text.Length - 1);
                                if (this.checkRowNumber)
                                {
                                    text = text + "';" + e.Row.RowIndex.ToString();
                                }
                                this.editPageUrlParameters = ((this.editPageUrlParameters == string.Empty) ? ("?" + this.DataKeyNames[0].ToString() + "=") : this.editPageUrlParameters);
                                HtmlAnchor htmlAnchor = (HtmlAnchor)e.Row.FindControl(this.ID + "_Edit");
                                htmlAnchor.HRef = this.editPageUrl + this.editPageUrlParameters + base.Page.Server.UrlEncode(text);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < e.Row.Cells.Count; i++)
                            {
                                foreach (Control control in e.Row.Cells[i].Controls)
                                {
                                    control.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        private void GridViewPageingByDB_Load(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<script type=\"text/javascript\">");
            if (this.showCheckBox)
            {
                stringBuilder.AppendLine(" function DoGridViewSelAll(Obj)");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("   var tablerows=Obj.parentNode.parentNode.parentNode.rows;");
                stringBuilder.AppendLine("   for(var rowindex=1;rowindex<tablerows.length;rowindex++)");
                stringBuilder.AppendLine("   {");
                // Legend 2016/10/27 將  hasChildNodes() 改為 hasChildren() ; 將  "childNodes" 改為 "children" , 因IE11不支持
                //stringBuilder.AppendLine("      if(tablerows[rowindex].cells[0].hasChildNodes() && tablerows[rowindex].cells[0].children[0].nodeName=='INPUT' && tablerows[rowindex].cells[0].children[0].attributes['type'].value=='checkbox')");
                stringBuilder.AppendLine("      if(tablerows[rowindex].cells[0].hasChildren() && tablerows[rowindex].cells[0].children[0].nodeName=='INPUT' && tablerows[rowindex].cells[0].children[0].attributes['type'].value=='checkbox')");

                stringBuilder.AppendLine("      {");
                stringBuilder.AppendLine("         tablerows[rowindex].cells[0].children[0].checked=Obj.checked;");
                stringBuilder.AppendLine("      }");
                stringBuilder.AppendLine("   }");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("function DoGridViewSel(Obj)");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("   var tablerows=Obj.parentNode.parentNode.parentNode.rows;");
                stringBuilder.AppendLine("   if(!Obj.checked)");
                stringBuilder.AppendLine("   {");
                stringBuilder.AppendLine("      tablerows[0].cells[0].children[0].checked=false;");
                stringBuilder.AppendLine("   }");
                stringBuilder.AppendLine("}");
            }
            stringBuilder.AppendLine("function GridViewPageingByDB_GoPage(btn)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("var hidPageNumber=btn.previousSibling.value;");
            stringBuilder.AppendLine("var txtPageNumber=btn.previousSibling.previousSibling.value;");
            stringBuilder.AppendLine("var re=new RegExp('^[1-9]+[0-9]*$','g');");
            stringBuilder.AppendLine("if(txtPageNumber.match(re))");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("if(Number(txtPageNumber)>Number(hidPageNumber))");
            stringBuilder.Append("{alert('");
            stringBuilder.Append(this.strPageNumberTooLong);
            stringBuilder.AppendLine("');}");
            stringBuilder.AppendLine("else");
            stringBuilder.AppendLine("{");
            stringBuilder.Append("__doPostBack('");
            stringBuilder.Append(this.ID);
            stringBuilder.AppendLine("_NumberButton',txtPageNumber);");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("else");
            stringBuilder.Append("{alert('");
            stringBuilder.Append(this.strPageNumberErr);
            stringBuilder.AppendLine("');}");
            stringBuilder.AppendLine("}");
            stringBuilder.Append("</script>");
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(base.GetType(), "SuperGridView_SelScript"))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "SuperGridView_SelScript", stringBuilder.ToString(), false);
            }
            if (this.Page.IsPostBack)
            {
                if (this.showCheckBox)
                {
                    this.selectedIDS = this.Page.Request.Form[this.ID + "_Sel"];
                    if (this.selectedIDS != string.Empty && this.selectedIDS != "" && this.selectedIDS != null)
                    {
                        string[] array = this.selectedIDS.Split(",".ToCharArray());
                        this.selectedIDList.Clear();
                        for (int i = 0; i < array.Length; i++)
                        {
                            this.selectedIDList.Add(array[i]);
                        }
                    }
                }
                if (this.AllowPaging)
                {
                    string text = this.Page.Request.Form["__EVENTTARGET"];
                    if (text != null)
                    {
                        bool flag = false;
                        if (text == this.ID + "_FirstButton")
                        {
                            this.PageIndex = 0;
                            flag = true;
                        }
                        else if (text == this.ID + "_PrevButton")
                        {
                            this.PageIndex--;
                            flag = true;
                        }
                        else if (text == this.ID + "_NextButton")
                        {
                            this.PageIndex++;
                            flag = true;
                        }
                        else if (text == this.ID + "_LastButton")
                        {
                            this.PageIndex = this.PageCount - 1;
                            flag = true;
                        }
                        else if (text == this.ID + "_NumberButton")
                        {
                            this.PageIndex = Convert.ToInt32(this.Page.Request.Form["__EVENTARGUMENT"]) - 1;
                            flag = true;
                        }
                        if (flag)
                        {
                            this.SetData();
                        }
                    }
                }
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            this.SetColumnHeaderTextByLocalResource();
            bool allowPaging = base.AllowPaging;
            allowPaging = this.blnAllowPaging;
            base.PageIndex = 0;
            base.OnDataBinding(e);
        }

        protected override void OnSorting(GridViewSortEventArgs e)
        {
            if (this.SortExpression == e.SortExpression)
            {
                if (this.SortDirection == SortDirection.Ascending)
                {
                    this.ViewState["SortDirection"] = SortDirection.Descending;
                }
                else
                {
                    this.ViewState["SortDirection"] = SortDirection.Ascending;
                }
            }
            else
            {
                this.ViewState["SortDirection"] = SortDirection.Ascending;
            }
            this.ViewState["SortExpression"] = e.SortExpression;
            base.OnSorting(e);
        }

        protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
        {
            base.InitializeRow(row, fields);
            switch (row.RowType)
            {
                case DataControlRowType.Header:
                    {
                        TableHeaderCell tableHeaderCell = new TableHeaderCell();
                        if (this.showCheckBox)
                        {
                            tableHeaderCell.Width = Unit.Parse("24px");
                            tableHeaderCell.Text = "<input type=\"checkbox\" name=\"" + this.ID + "_SelAll\" onclick=\"DoGridViewSelAll(this);\">";
                            row.Cells.AddAt(0, tableHeaderCell);
                        }
                        if (this.showEditButton)
                        {
                            tableHeaderCell = new TableHeaderCell();
                            tableHeaderCell.Width = Unit.Parse(this.editCellWidth);
                            tableHeaderCell.Wrap = false;
                            if (this.editButtonImageUrl != string.Empty)
                            {
                                HtmlImage htmlImage = new HtmlImage();
                                htmlImage.Border = 0;
                                htmlImage.Src = this.editButtonImageUrl;
                                tableHeaderCell.Controls.Add(htmlImage);
                            }
                            else
                            {
                                tableHeaderCell.Text = this.editButtonText;
                            }
                            row.Cells.Add(tableHeaderCell);
                        }
                        break;
                    }
                case DataControlRowType.DataRow:
                    if (this.showCheckBox && this.blnHasDataRow)
                    {
                        TableCell tableCell = new TableCell();
                        tableCell.Width = Unit.Parse("24px");
                        tableCell.Text = "<input type=\"checkbox\" name=\"" + this.ID + "_Sel\" onclick=\"DoGridViewSel(this);\" value=\"\">";
                        row.Cells.AddAt(0, tableCell);
                    }
                    if (this.showEditButton && this.blnHasDataRow)
                    {
                        TableCell tableCell = new TableCell();
                        tableCell.HorizontalAlign = HorizontalAlign.Center;
                        HtmlAnchor htmlAnchor = new HtmlAnchor();
                        htmlAnchor.ID = this.ID + "_Edit";
                        tableCell.Width = Unit.Parse(this.editCellWidth);
                        if (this.editButtonImageUrl != string.Empty)
                        {
                            HtmlImage htmlImage = new HtmlImage();
                            htmlImage.Border = 0;
                            htmlImage.Src = this.editButtonImageUrl;
                            htmlAnchor.Controls.Add(htmlImage);
                        }
                        else
                        {
                            htmlAnchor.InnerHtml = this.editButtonText;
                        }
                        tableCell.Controls.Add(htmlAnchor);
                        row.Cells.Add(tableCell);
                    }
                    break;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.PagerSettings.Visible = false;
            writer.Write("<table style=\"width:100%;border-collapse:collapse;\"><tr><td>");
            this.Width = Unit.Parse("100%");
            base.Render(writer);
            base.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            base.RowStyle.HorizontalAlign = HorizontalAlign.Left;
            if (!this.blnHasDataRow && this.DataSource != null)
            {
                writer.Write("</td></tr><tr><td style=\"font-size:12px;color: #FF0000;\">");
                writer.Write(this.strNoneData);
                writer.Write("</td></tr></table>");
            }
            else
            {
                if (this.AllowPaging && this.PageCount > 1)
                {
                    writer.Write("</td></tr><tr><td style=\"font-size:12px;\">");
                    HtmlTable htmlTable = new HtmlTable();
                    HtmlTableRow htmlTableRow = new HtmlTableRow();
                    if (this.PageIndex != 0)
                    {
                        if (this.PagerSettings.FirstPageImageUrl != string.Empty && this.PagerSettings.FirstPageImageUrl != "")
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.FirstPageImageUrl, "javascript:__doPostBack('" + this.ID + "_FirstButton','Page$First')", true));
                        }
                        else
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.FirstPageText, "javascript:__doPostBack('" + this.ID + "_FirstButton','Page$First')", false));
                        }
                        if (this.PagerSettings.PreviousPageImageUrl != string.Empty && this.PagerSettings.PreviousPageImageUrl != "")
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.PreviousPageImageUrl, "javascript:__doPostBack('" + this.ID + "_PrevButton','Page$Prev')", true));
                        }
                        else
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.PreviousPageText, "javascript:__doPostBack('" + this.ID + "_PrevButton','Page$Prev')", false));
                        }
                    }
                    int num = ((this.PageIndex % 10 == 0) ? this.PageIndex : (this.PageIndex / 10 * 10)) + 1;
                    int num2 = num + 10;
                    num2 = ((num2 > this.PageCount) ? this.PageCount : num2);
                    num = ((num - 1 > 0) ? (num - 1) : num);
                    for (int i = num; i <= num2; i++)
                    {
                        if (i == this.PageIndex + 1)
                        {
                            HtmlTableCell htmlTableCell = new HtmlTableCell();
                            htmlTableCell.InnerHtml = i.ToString();
                            htmlTableRow.Cells.Add(htmlTableCell);
                        }
                        else
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(i.ToString(), string.Concat(new string[]
                            {
                                "javascript:__doPostBack('",
                                this.ID,
                                "_NumberButton','",
                                i.ToString(),
                                "')"
                            }), false));
                        }
                    }
                    if (this.PageIndex != this.PageCount - 1)
                    {
                        if (this.PagerSettings.NextPageImageUrl != string.Empty && this.PagerSettings.NextPageImageUrl != "")
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.NextPageImageUrl, "javascript:__doPostBack('" + this.ID + "_NextButton','Page$Next')", true));
                        }
                        else
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.NextPageText, "javascript:__doPostBack('" + this.ID + "_NextButton','Page$Next')", false));
                        }
                        if (this.PagerSettings.LastPageImageUrl != string.Empty && this.PagerSettings.LastPageImageUrl != "")
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.LastPageImageUrl, "javascript:__doPostBack('" + this.ID + "_LastButton','Page$Last')", true));
                        }
                        else
                        {
                            htmlTableRow.Cells.Add(this.GetLinkCell(this.PagerSettings.LastPageText, "javascript:__doPostBack('" + this.ID + "_LastButton','Page$Last')", false));
                        }
                    }
                    HtmlTableCell htmlTableCell2 = new HtmlTableCell();
                    int num3 = this.PageIndex + 1;
                    htmlTableCell2.InnerText = string.Concat(new object[]
                    {
                        " ",
                        num3.ToString(),
                        "/",
                        this.PageCount,
                        this.strPageString,
                        " "
                    });
                    htmlTableRow.Cells.Add(htmlTableCell2);
                    HtmlTableCell htmlTableCell3 = new HtmlTableCell();
                    HtmlInputText htmlInputText = new HtmlInputText();
                    htmlInputText.Size = 2;
                    htmlInputText.Value = num3.ToString();
                    HtmlInputHidden htmlInputHidden = new HtmlInputHidden();
                    htmlInputHidden.Value = this.PageCount.ToString();
                    HtmlInputButton htmlInputButton = new HtmlInputButton();
                    htmlInputButton.Attributes.Add("onclick", "GridViewPageingByDB_GoPage(this);");
                    htmlInputButton.Value = "Go";
                    htmlTableCell3.Controls.Add(htmlInputText);
                    htmlTableCell3.Controls.Add(htmlInputHidden);
                    htmlTableCell3.Controls.Add(htmlInputButton);
                    htmlTableRow.Cells.Add(htmlTableCell3);
                    htmlTable.Rows.Add(htmlTableRow);
                    htmlTable.RenderControl(writer);
                }
                writer.Write("</td></tr></table>");
            }
        }

        private HtmlTableCell GetLinkCell(string text, string url, bool isImage)
        {
            HtmlTableCell htmlTableCell = new HtmlTableCell();
            htmlTableCell.NoWrap = true;
            HtmlAnchor htmlAnchor = new HtmlAnchor();
            if (isImage)
            {
                htmlAnchor.InnerHtml = "<img src=\"" + text + "\" border=\"0\" style=\"border:none;\"/>";
            }
            else
            {
                htmlAnchor.InnerHtml = text;
            }
            htmlAnchor.HRef = url;
            htmlTableCell.Controls.Add(htmlAnchor);
            return htmlTableCell;
        }

        public void SetColumnHeaderTextByLocalResource()
        {
            if (!base.DesignMode)
            {
                if (this.columnsHeaderText != string.Empty && this.columnsHeaderText != "")
                {
                    string[] array = this.columnsHeaderText.Split(",".ToCharArray());
                    for (int i = 0; i < this.Columns.Count; i++)
                    {
                        this.Columns[i].HeaderText = array[i];
                    }
                }
            }
        }

        public void BindData()
        {
            this.SetData();
        }

        private void SetData()
        {
            if (OnSetDataSource != default(OnSetDataSourceDelegate))
            {
                SetDataSourceEventArgs setDataSourceEventArgs = new SetDataSourceEventArgs(this.PageIndex, this.PageSize, this.SortExpression, this.SortDirection);
                this.OnSetDataSource(this, setDataSourceEventArgs);
                if (setDataSourceEventArgs.Table != null)
                {
                    if (setDataSourceEventArgs.Table.Rows.Count < 1 && this.PageIndex > 0)
                    {
                        this.PageIndex--;
                        setDataSourceEventArgs = new SetDataSourceEventArgs(this.PageIndex, this.PageSize, this.SortExpression, this.SortDirection);
                        this.OnSetDataSource(this, setDataSourceEventArgs);
                    }
                    if (setDataSourceEventArgs.Table.Rows.Count < 1)
                    {
                        this.blnHasDataRow = false;
                    }
                    else
                    {
                        this.blnHasDataRow = true;
                    }
                    this.DataSource = setDataSourceEventArgs.Table;
                    this.DataBind();
                    this.ViewState["PageCount"] = setDataSourceEventArgs.PageCount;
                    this.PageIndex = ((this.PageIndex == -2) ? (this.PageCount - 1) : this.PageIndex);
                    this.PageIndex = ((this.PageIndex >= this.PageCount) ? (this.PageCount - 1) : this.PageIndex);
                    this.PageIndex = ((this.PageIndex < 0) ? 0 : this.PageIndex);
                }
            }
        }

        private void SetDataFirst()
        {
            if (this.OnSetDataSource != null)
            {
                SetDataSourceEventArgs setDataSourceEventArgs = new SetDataSourceEventArgs(1, this.PageSize, this.SortExpression, this.SortDirection);
                this.OnSetDataSource(this, setDataSourceEventArgs);
                if (setDataSourceEventArgs.Table != null)
                {
                    if (setDataSourceEventArgs.Table.Rows.Count < 1)
                    {
                        DataRow row = setDataSourceEventArgs.Table.NewRow();
                        setDataSourceEventArgs.Table.Rows.Add(row);
                        this.blnHasDataRow = false;
                    }
                    else
                    {
                        this.blnHasDataRow = true;
                    }
                    this.DataSource = setDataSourceEventArgs.Table;
                    this.DataBind();
                    this.ViewState["PageCount"] = setDataSourceEventArgs.PageCount;
                    this.PageIndex = ((this.PageIndex == -2) ? (this.PageCount - 1) : this.PageIndex);
                    this.PageIndex = ((this.PageIndex >= this.PageCount) ? (this.PageCount - 1) : this.PageIndex);
                    this.PageIndex = ((this.PageIndex < 0) ? 0 : this.PageIndex);
                }
            }
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
        }

        protected void DisplaySortOrderImages(string sortExpression, GridViewRow dgItem)
        {
            string[] sortColumns = sortExpression.Split(",".ToCharArray());
            for (int i = 0; i < dgItem.Cells.Count; i++)
            {
                if (dgItem.Cells[i].Controls.Count > 0 && dgItem.Cells[i].Controls[0] is LinkButton)
                {
                    string commandArgument = ((LinkButton)dgItem.Cells[i].Controls[0]).CommandArgument;
                    string text;
                    int num;
                    this.SearchSortExpression(sortColumns, commandArgument, out text, out num);
                    if (num > 0)
                    {
                        string text2 = text.Equals("ASC") ? this.SortAscImageUrl : this.SortDescImageUrl;
                        if (text2 != string.Empty)
                        {
                            Image image = new Image();
                            image.ImageUrl = text2;
                            dgItem.Cells[i].Controls.Add(image);
                        }
                    }
                }
            }
        }

        protected void SearchSortExpression(string[] sortColumns, string sortColumn, out string sortOrder, out int sortOrderNo)
        {
            sortOrder = "";
            sortOrderNo = -1;
            for (int i = 0; i < sortColumns.Length; i++)
            {
                if (sortColumns[i].StartsWith(sortColumn))
                {
                    sortOrderNo = i + 1;
                    sortOrder = ((this.SortDirection == SortDirection.Ascending) ? "ASC" : "DESC");
                }
            }
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (this.SortExpression != string.Empty)
                {
                    this.DisplaySortOrderImages(this.SortExpression, e.Row);
                    this.CreateRow(0, 0, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                }
            }
            base.OnRowCreated(e);
        }

    }

    /// <summary>
    /// 綁定資料事件參數
    /// </summary>
    public class SetDataSourceEventArgs : EventArgs
    {
        private DataTable dtblTable = null;

        private int intPageCount;

        private string strSortExpressionl = "null";

        private string strSortDirection = "ASC";

        private int intRowCount;

        private string strFirstRow;

        private string strLastRow;

        private int intPageSize = 10;

        public DataTable Table
        {
            get
            {
                return this.dtblTable;
            }
            set
            {
                this.dtblTable = value;
            }
        }

        public string SortExpression
        {
            get
            {
                return this.strSortExpressionl;
            }
        }

        public string SortDirection
        {
            get
            {
                return this.strSortDirection;
            }
        }

        public int PageCount
        {
            get
            {
                return this.intPageCount;
            }
        }

        public int RowCount
        {
            get
            {
                return this.intRowCount;
            }
            set
            {
                this.intRowCount = value;
                if (value % this.intPageSize == 0)
                {
                    this.intPageCount = this.intRowCount / this.intPageSize;
                }
                else
                {
                    this.intPageCount = this.intRowCount / this.intPageSize + 1;
                }
            }
        }

        public string FirstRow
        {
            get
            {
                return this.strFirstRow;
            }
        }

        public string LastRow
        {
            get
            {
                return this.strLastRow;
            }
        }

        public int PageSize
        {
            get
            {
                return this.intPageSize;
            }
        }

        public SetDataSourceEventArgs(int pageIndex, int pageSize, string sortExpressionl, SortDirection sortDirection)
        {
            this.intPageSize = pageSize;
            this.strSortExpressionl = sortExpressionl;
            this.strSortDirection = ((sortDirection == System.Web.UI.WebControls.SortDirection.Ascending) ? "ASC" : "DESC");
            int num = pageIndex * pageSize;
            int num2 = num + pageSize;
            this.strFirstRow = (num + 1).ToString();
            this.strLastRow = num2.ToString();
        }

        public void FillByDataTable(DataTable baseTable)
        {
            this.dtblTable = baseTable.Clone();
            int num = int.Parse(this.strFirstRow) - 1;
            int num2 = int.Parse(this.strLastRow);
            this.RowCount = baseTable.Rows.Count;
            for (int i = num; i < num2; i++)
            {
                if (baseTable.Rows.Count > i && baseTable.Rows[i] != null)
                {
                    this.dtblTable.Rows.Add(baseTable.Rows[i].ItemArray);
                }
            }
        }

    }
}
