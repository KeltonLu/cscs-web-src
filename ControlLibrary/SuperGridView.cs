//******************************************************************
//*  作    者：QingChen
//*  功能說明：特殊交互表格
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.ComponentModel;

namespace ControlLibrary
{
    public delegate void OnSetDataDataSourceDelegate(object sender, EventArgs e);
    public class SuperGridView : GridView
    {
        /// <summary>
        /// 建構
        /// </summary>
        public SuperGridView()
        {
            this.RowDataBound += new GridViewRowEventHandler(SuperGridView_RowDataBound);
            editButtonText = "編輯";
            editPageUrl = "#";
            this.Load += new EventHandler(SuperGridView_Load);
            //base.AllowPaging = true;
            base.AllowSorting = true;
            // base.Sorting += new GridViewSortEventHandler(SuperGridView_Sorting);
            base.PagerSettings.Visible = false;

            //base.Page.Load += new EventHandler(Page_Load);
        }

        //void SuperGridView_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    //throw new Exception("The method or operation is not implemented.");
        //    base.Sort(e.SortExpression, e.SortDirection);
        //}
        /// <summary>
        /// 重寫資料綁定
        /// </summary>
        protected override void OnDataBinding(EventArgs e)
        {
            SetColumnHeaderTextByLocalResource();
            base.OnDataBinding(e);
        }

        /// <summary>
        /// 重寫排序
        /// </summary>
        protected override void OnSorting(GridViewSortEventArgs e)
        {
            //base.OnSorting(e);
            //this.SortExpression = e.SortExpression;
            //this.SortDirection = e.SortDirection;
            //this.
            //this.Sort(e.SortExpression, e.SortDirection);
            if (this.DataSource != null)
            {
                Type Dt = this.DataSource.GetType();
                DataView dv = null;
                if (Dt == typeof(DataTable))
                {
                    dv = ((DataTable)this.DataSource).DefaultView;
                }
                else if (Dt == typeof(DataSet))
                {
                    dv = ((DataSet)this.DataSource).Tables[this.DataMember].DefaultView;
                }
                else if (Dt == typeof(DataView))
                {
                    dv = (DataView)this.DataSource;
                }
                else
                {
                    throw new Exception("使用了不能支持的數據源類型");
                }
                dv.Sort = e.SortExpression;
                this.DataBind();
            }
        }
        public event OnSetDataDataSourceDelegate OnSetDataDataSource = default(OnSetDataDataSourceDelegate);

        /// <summary>
        /// 重寫加載
        /// </summary>
        void SuperGridView_Load(object sender, EventArgs e)
        {

            if (showCheckBox)
            {
                StringBuilder scriptBuider = new StringBuilder();
                scriptBuider.AppendLine("<script type=\"text/javascript\">");
                scriptBuider.AppendLine(" function DoGridViewSelAll(Obj)");
                scriptBuider.AppendLine("{");
                scriptBuider.AppendLine("   var tablerows=Obj.parentNode.parentNode.parentNode.rows;");
                scriptBuider.AppendLine("   for(var rowindex=1;rowindex<tablerows.length;rowindex++)");
                scriptBuider.AppendLine("   {");
                // Legend 2016/10/27 將  hasChildNodes() 改為 hasChildren() ; 將  "childNodes" 改為 "children" , 因IE11不支持
                //scriptBuider.AppendLine("      if(tablerows[rowindex].cells[0].hasChildNodes() && tablerows[rowindex].cells[0].children[0].nodeName=='INPUT' && tablerows[rowindex].cells[0].children[0].attributes['type'].value=='checkbox')");
                scriptBuider.AppendLine("      if(tablerows[rowindex].cells[0].hasChildren() && tablerows[rowindex].cells[0].children[0].nodeName=='INPUT' && tablerows[rowindex].cells[0].children[0].attributes['type'].value=='checkbox')");

                scriptBuider.AppendLine("      {");
                scriptBuider.AppendLine("         tablerows[rowindex].cells[0].children[0].checked=Obj.checked;");
                scriptBuider.AppendLine("      }");
                scriptBuider.AppendLine("   }");
                scriptBuider.AppendLine("}");
                scriptBuider.AppendLine("function DoGridViewSel(Obj)");
                scriptBuider.AppendLine("{");
                scriptBuider.AppendLine("   var tablerows=Obj.parentNode.parentNode.parentNode.rows;");
                scriptBuider.AppendLine("   if(!Obj.checked)");
                scriptBuider.AppendLine("   {");
                scriptBuider.AppendLine("      tablerows[0].cells[0].children[0].checked=false;");
                scriptBuider.AppendLine("   }");
                scriptBuider.AppendLine("}");
                scriptBuider.Append("</script>");
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "SuperGridView_SelScript"))
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SuperGridView_SelScript", scriptBuider.ToString(), false);
                }
            }
            if (Page.IsPostBack)
            {
                int aaa = PageSize;
                if (showCheckBox)
                {
                    selectedIDS = Page.Request.Form[this.ID + "_Sel"];
                    if (selectedIDS != string.Empty && selectedIDS != "" && selectedIDS != null)
                    {
                        string[] selectedIDSArray = selectedIDS.Split(",".ToCharArray());
                        selectedIDList.Clear();
                        for (int index = 0; index < selectedIDSArray.Length; index++)
                        {
                            selectedIDList.Add(selectedIDSArray[index]);
                        }
                    }
                }
                if (this.AllowPaging)
                {
                    string PostObject = Page.Request.Form["__EVENTTARGET"];
                    if (PostObject != null)
                    {
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.ToString(), "alert('" + PostObject + "');", true);
                        if (PostObject == this.ID + "_FirstButton")
                        {
                            //base.OnPageIndexChanging(null);
                            if (OnSetDataDataSource != default(OnSetDataDataSourceDelegate))
                                OnSetDataDataSource(this, new EventArgs());
                            base.PageIndex = 0;
                            base.DataBind();
                        }
                        else if (PostObject == this.ID + "_PrevButton")
                        {
                            //base.OnPageIndexChanging(null);
                            if (OnSetDataDataSource != default(OnSetDataDataSourceDelegate))
                                OnSetDataDataSource(this, new EventArgs());
                            base.PageIndex--;
                            base.DataBind();
                        }
                        else if (PostObject == this.ID + "_NextButton")
                        {
                            //base.OnPageIndexChanging(null);
                            if (OnSetDataDataSource != default(OnSetDataDataSourceDelegate))
                                OnSetDataDataSource(this, new EventArgs());
                            base.PageIndex++;
                            base.DataBind();
                        }
                        else if (PostObject == this.ID + "_LastButton")
                        {
                            //base.OnPageIndexChanging(null);
                            if (OnSetDataDataSource != default(OnSetDataDataSourceDelegate))
                                OnSetDataDataSource(this, new EventArgs());
                            base.PageIndex = base.PageCount - 1;
                            base.DataBind();
                        }
                        else if (PostObject == this.ID + "_NumberButton")
                        {
                            //base.OnPageIndexChanging(null);
                            if (OnSetDataDataSource != default(OnSetDataDataSourceDelegate))
                                OnSetDataDataSource(this, new EventArgs());
                            base.PageIndex = Convert.ToInt32(Page.Request.Form["__EVENTARGUMENT"]) - 1;
                            int t = base.PageCount;
                            base.DataBind();
                        }

                    }
                }
            }
        }

        private List<HtmlInputCheckBox> checkBoxes = new List<HtmlInputCheckBox>();
        /// <summary>
        /// 被選擇的編碼集合字符串
        /// </summary>
        public string SelectedIDS { get { return selectedIDS; } }
        private string selectedIDS = string.Empty;
        private List<string> selectedIDList = new List<string>();
        /// <summary>
        /// 編輯頁地址
        /// </summary>
        public string EditPageUrl { get { return editPageUrl; } set { editPageUrl = value; } }
        private string editPageUrl;
        /// <summary>
        /// 要給編輯頁面的參數
        /// </summary>
        public string EditPageUrlParameters { get { return editPageUrlParameters; } set { editPageUrlParameters = value; } }
        private string editPageUrlParameters;

        /// <summary>
        /// 顯示編輯按鈕列
        /// </summary>
        [DefaultValue(false)]
        public bool ShowEditButton { get { return showEditButton; } set { showEditButton = value; } }
        private bool showEditButton = false;

        /// <summary>
        /// 顯示多選框列
        /// </summary>
        [DefaultValue(false)]
        public bool ShowCheckBox { get { return showCheckBox; } set { showCheckBox = value; } }
        private bool showCheckBox = false;

        /// <summary>
        /// 選擇時返回值加入行號
        /// </summary>
        [DefaultValue(false)]
        public bool CheckRowNumber { get { return checkRowNumber; } set { checkRowNumber = value; } }
        private bool checkRowNumber = false;

        /// <summary>
        /// 使用分組查詢
        /// </summary>
        public bool UseGroup { get { return useGroup; } set { useGroup = value; } }
        private bool useGroup = false;

        /// <summary>
        /// 當前組數
        /// </summary>
        public int GroupIndex { get { return groupIndex; } set { groupIndex = value; } }
        private int groupIndex = 0;

        /// <summary>
        /// 組長度
        /// </summary>
        public int GroupCount { get { return groupCount; } set { groupCount = value; } }
        private int groupCount = 1;

        /// <summary>
        /// 每組行數
        /// </summary>
        public int GroupSize { get { return groupSize; } set { groupSize = value; } }
        private int groupSize = 10;

        /// <summary>
        /// 列頭部文字集合字符串
        /// </summary>
        [LocalizableAttribute(true)]
        public string ColumnsHeaderText { get { return columnsHeaderText; } set { columnsHeaderText = value; } }
        private string columnsHeaderText = string.Empty;

        /// <summary>
        /// 編輯按鈕列寬度
        /// </summary>
        public string EditCellWidth { get { return editCellWidth; } set { editCellWidth = value; } }
        private string editCellWidth = "48px";

        /// <summary>
        /// 最前頁文本
        /// </summary>
        [LocalizableAttribute(true)]
        public string FirstPageText { get { return this.PagerSettings.FirstPageText; } set { this.PagerSettings.FirstPageText = value; } }
        /// <summary>
        /// 上一頁文本
        /// </summary>
        [LocalizableAttribute(true)]
        public string PreviousPageText { get { return this.PagerSettings.PreviousPageText; } set { this.PagerSettings.PreviousPageText = value; } }
        /// <summary>
        /// 下一頁文本
        /// </summary>
        [LocalizableAttribute(true)]
        public string NextPageText { get { return this.PagerSettings.NextPageText; } set { this.PagerSettings.NextPageText = value; } }
        /// <summary>
        /// 最後頁文本
        /// </summary>
        [LocalizableAttribute(true)]
        public string LastPageText { get { return this.PagerSettings.LastPageText; } set { this.PagerSettings.LastPageText = value; } }

        /// <summary>
        /// 編輯按鈕文字
        /// </summary>
        [LocalizableAttribute(true)]
        public string EditButtonText { get { return editButtonText; } set { editButtonText = value; } }
        private string editButtonText;

        /// <summary>
        /// 編輯按鈕圖片地址
        /// </summary>
        public string EditButtonImageUrl { get { return editButtonImageUrl; } set { editButtonImageUrl = value; } }
        private string editButtonImageUrl = string.Empty;

        /// <summary>
        /// 重寫行資料綁定
        /// </summary>
        void SuperGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (!DesignMode)
            {
                if (DataSource != null && ((DataSource.GetType() == typeof(DataSet) && DataMember != null && DataMember != string.Empty && DataMember != "") || DataSource.GetType() != typeof(DataSet)))
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        DataRowView Rv = (DataRowView)e.Row.DataItem;

                        if (showCheckBox)
                        {
                            if (this.DataKeyNames != null && this.DataKeyNames.Length <= 0)
                            {
                                throw new Exception("請設置" + this.ID + "的DataKeyNames屬性");
                            }
                            else
                            {
                                string idValue = "";
                                for (int index = 0; index < this.DataKeyNames.Length; index++)
                                {
                                    if (Rv[this.DataKeyNames[index].ToString()].GetType() == typeof(DateTime))
                                    {
                                        idValue += Convert.ToDateTime(Rv[this.DataKeyNames[index].ToString()]).ToString("yyyy-MM-dd HH:mm:ss.fff") + ";";
                                    }
                                    else
                                    {
                                        idValue += Rv[this.DataKeyNames[index].ToString()].ToString().Trim() + ";";
                                    }
                                }
                                idValue = idValue.Substring(0, idValue.Length - 1);
                                if (checkRowNumber)
                                {
                                    idValue += ";" + e.Row.RowIndex.ToString();
                                }
                                if (selectedIDList.Contains(idValue))
                                {
                                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("value=\"\"", "value=\"" + idValue + "\" checked=\"checked\"");

                                }
                                else
                                {

                                    e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("value=\"\"", "value=\"" + idValue + "\"");

                                }
                            }
                        }

                        if (showEditButton)
                        {
                            if (this.DataKeyNames != null && this.DataKeyNames.Length <= 0)
                            {
                                throw new Exception("請設置" + this.ID + "的DataKeyNames屬性");
                            }
                            else
                            {
                                string idValue = "";
                                for (int index = 0; index < this.DataKeyNames.Length; index++)
                                {
                                    if (Rv[this.DataKeyNames[index].ToString()].GetType() == typeof(DateTime))
                                    {
                                        idValue += Convert.ToDateTime(Rv[this.DataKeyNames[index].ToString()]).ToString("yyyy-MM-dd HH:mm:ss.fff") + ";";
                                    }
                                    else
                                    {
                                        idValue += Rv[this.DataKeyNames[index].ToString()].ToString().Trim() + ";";
                                    }
                                }
                                idValue = idValue.Substring(0, idValue.Length - 1);
                                if (checkRowNumber)
                                {
                                    idValue += "';" + e.Row.RowIndex.ToString();
                                }
                                editPageUrlParameters = (editPageUrlParameters == string.Empty) ? "?" + this.DataKeyNames[0].ToString() + "=" : editPageUrlParameters;
                                HtmlAnchor a = (HtmlAnchor)e.Row.FindControl(this.ID + "_Edit");
                                // Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.ToString(), "alert('" + a.ToString() + "');", true);
                                a.HRef = editPageUrl + editPageUrlParameters + base.Page.Server.UrlEncode(idValue);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 重寫行初始化
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="fields">欄位</param>
        protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
        {
            base.InitializeRow(row, fields);

            TableCell cell = null;
            switch (row.RowType)
            {
                case DataControlRowType.DataRow:
                    if (showCheckBox)
                    {
                        cell = new TableCell();
                        cell.Width = Unit.Parse("24px");
                        cell.Text = "<input type=\"checkbox\" name=\"" + this.ID + "_Sel\" onclick=\"DoGridViewSel(this);\" value=\"\">";
                        row.Cells.AddAt(0, cell);
                    }
                    if (showEditButton)
                    {
                        cell = new TableCell();
                        cell.HorizontalAlign = HorizontalAlign.Center;
                        HtmlAnchor editLink = new HtmlAnchor();
                        editLink.ID = this.ID + "_Edit";
                        cell.Width = Unit.Parse(editCellWidth);
                        if (editButtonImageUrl != string.Empty)
                        {
                            HtmlImage editImage = new HtmlImage();
                            editImage.Border = 0;
                            editImage.Src = editButtonImageUrl;
                            editLink.Controls.Add(editImage);
                        }
                        else
                        {

                            editLink.InnerHtml = editButtonText;
                        }

                        cell.Controls.Add(editLink);
                        row.Cells.Add(cell);
                    }
                    break;
                case DataControlRowType.Header:
                    TableHeaderCell headerCell = new TableHeaderCell();
                    if (showCheckBox)
                    {

                        headerCell.Width = Unit.Parse("24px");
                        headerCell.Text = "<input type=\"checkbox\" name=\"" + this.ID + "_SelAll\" onclick=\"DoGridViewSelAll(this);\">";
                        row.Cells.AddAt(0, headerCell);
                    }
                    if (showEditButton)
                    {
                        headerCell = new TableHeaderCell();
                        headerCell.Width = Unit.Parse(editCellWidth);
                        headerCell.Wrap = false;
                        if (editButtonImageUrl != string.Empty)
                        {
                            HtmlImage editImage = new HtmlImage();
                            editImage.Border = 0;
                            editImage.Src = editButtonImageUrl;
                            headerCell.Controls.Add(editImage);
                        }
                        else
                        {
                            headerCell.Text = editButtonText;
                        }
                        row.Cells.Add(headerCell);
                    }
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 重寫繪製
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            base.PagerSettings.Visible = false;

            writer.Write("<table style=\"width:100%;border-collapse:collapse;\"><tr><td>");
            this.Width = Unit.Parse("100%");
            base.Render(writer);
            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.RowStyle.HorizontalAlign = HorizontalAlign.Left;
            if (this.AllowPaging && this.PageCount>1)
            {
                writer.Write("</td></tr><tr><td style=\"font-size:12px;\">");
                HtmlTable paperTable = new HtmlTable();
                HtmlTableRow row = new HtmlTableRow();
                if (this.useGroup)
                {
                    if (PagerSettings.FirstPageImageUrl != string.Empty && PagerSettings.FirstPageImageUrl != "")
                    {
                        row.Cells.Add(GetLinkCell(this.PagerSettings.FirstPageImageUrl, "javascript:__doPostBack('" + this.ID + "_FirstGroup','Group$First')", true));
                    }
                    else
                    {
                        row.Cells.Add(GetLinkCell(this.PagerSettings.FirstPageText, "javascript:__doPostBack('" + this.ID + "_FirstGroup','Group$First')", false));
                    }
                }
                if (this.PageIndex != 0)
                {
                    if (PagerSettings.FirstPageImageUrl != string.Empty && PagerSettings.FirstPageImageUrl != "")
                    {
                        row.Cells.Add(GetLinkCell(this.PagerSettings.FirstPageImageUrl, "javascript:__doPostBack('" + this.ID + "_FirstButton','Page$First')", true));
                    }
                    else
                    {
                        row.Cells.Add(GetLinkCell(this.PagerSettings.FirstPageText, "javascript:__doPostBack('" + this.ID + "_FirstButton','Page$First')", false));
                    }
                    if (PagerSettings.PreviousPageImageUrl != string.Empty && PagerSettings.PreviousPageImageUrl != "")
                    {
                        row.Cells.Add(GetLinkCell(this.PagerSettings.PreviousPageImageUrl, "javascript:__doPostBack('" + this.ID + "_PrevButton','Page$Prev')", true));
                    }
                    else
                    {
                        row.Cells.Add(GetLinkCell(this.PagerSettings.PreviousPageText, "javascript:__doPostBack('" + this.ID + "_PrevButton','Page$Prev')", false));
                    }
                }
                
                    for (int index = 1; index <= this.PageCount; index++)
                    {

                        if (index == this.PageIndex + 1)
                        {
                            HtmlTableCell cell = new HtmlTableCell();
                            cell.InnerHtml = index.ToString();
                            row.Cells.Add(cell);
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(index.ToString(), "javascript:__doPostBack('" + this.ID + "_NumberButton','" + index.ToString() + "')", false));
                        }
                    }

                    if (this.PageIndex != this.PageCount - 1)
                    {
                        if (this.PagerSettings.NextPageImageUrl != string.Empty && this.PagerSettings.NextPageImageUrl != "")
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.NextPageImageUrl, "javascript:__doPostBack('" + this.ID + "_NextButton','Page$Next')", true));
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.NextPageText, "javascript:__doPostBack('" + this.ID + "_NextButton','Page$Next')", false));
                        }
                        if (this.PagerSettings.LastPageImageUrl != string.Empty && this.PagerSettings.LastPageImageUrl != "")
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.LastPageImageUrl, "javascript:__doPostBack('" + this.ID + "_LastButton','Page$Last')", true));
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.LastPageText, "javascript:__doPostBack('" + this.ID + "_LastButton','Page$Last')", false));
                        }
                    }
                    paperTable.Rows.Add(row);
                    paperTable.RenderControl(writer);
                }
                
            
            writer.Write("</td></tr></table>");
        }
        /// <summary>
        /// 獲得帶Link控制項的Cell物件
        /// </summary>
        /// <param name="text">鏈接文字</param>
        /// <param name="url">鏈接地址</param>
        /// <param name="isImage">是否使用圖片</param>
        private HtmlTableCell GetLinkCell(string text, string url, bool isImage)
        {
            HtmlTableCell cell = new HtmlTableCell();
            cell.NoWrap = true;
            HtmlAnchor a = new HtmlAnchor();
            if (isImage)
            {
                a.InnerHtml = "<img src=\"" + text + "\" border=\"0\" style=\"border:none;\"/>";
            }
            else
            {
                a.InnerHtml = text;
            }
            a.HRef = url;
            cell.Controls.Add(a);
            return cell;
        }

        /// <summary>
        /// 設置列頭文字
        /// </summary>
        public void SetColumnHeaderTextByLocalResource()
        {
            if (!DesignMode)
            {
                if (columnsHeaderText != string.Empty && columnsHeaderText != "")
                {
                    string[] headerTexts = columnsHeaderText.Split(",".ToCharArray());
                    for (int index = 0; index < this.Columns.Count; index++)
                    {
                        this.Columns[index].HeaderText = headerTexts[index];
                    }
                }
            }
        }
    }
}
