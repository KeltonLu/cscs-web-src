//******************************************************************
//*  作    者：QingChen
//*  功能說明：移選控制項
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ControlLibrary
{
    public class MoveListBox : ListBox
    {
        private HiddenField AddedBox
        {
            get
            {
                return (HiddenField)base.Controls[1];
            }
        }

        private HiddenField DeletedBox
        {
            get
            {
                return (HiddenField)base.Controls[2];
            }
        }

        public ListBox RightListBox
        {
            get
            {
                return (ListBox)base.Controls[0];
            }
        }

        private Dictionary<string, string> BaseItems
        {
            get
            {
                if (base.ViewState["BaseItems"] == null)
                {
                    base.ViewState.Add("BaseItems", new Dictionary<string, string>());
                }
                return (Dictionary<string, string>)base.ViewState["BaseItems"];
            }
        }

        public string MastAddValues
        {
            get
            {
                return string.Join(",", this.GetMastAddValuesArray());
            }
        }

        public string MastDelValues
        {
            get
            {
                return string.Join(",", this.GetMastDelValuesArray());
            }
        }

        public string UserSelectedValues
        {
            get
            {
                return string.Join(",", this.GetUserSelectedValuesArray());
            }
        }

        public MoveListBox()
        {
            this.Controls.Add(new ListBox());
            this.Controls.Add(new HiddenField());
            this.Controls.Add(new HiddenField());
        }

        public Dictionary<string, string> GetMastAddData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < this.RightListBox.Items.Count; i++)
            {
                if (!this.BaseItems.ContainsKey(this.RightListBox.Items[i].Value.Trim()))
                {
                    dictionary.Add(this.RightListBox.Items[i].Value.Trim(), this.RightListBox.Items[i].Text.Trim());
                }
            }
            return dictionary;
        }

        public Dictionary<string, string> GetMastDelData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string current in this.BaseItems.Keys)
            {
                if (this.RightListBox.Items.FindByValue(current.Trim()) == null)
                {
                    dictionary.Add(current, this.BaseItems[current]);
                }
            }
            return dictionary;
        }

        public string[] GetMastAddValuesArray()
        {
            Dictionary<string, string> mastAddData = this.GetMastAddData();
            string[] array = new string[mastAddData.Count];
            mastAddData.Keys.CopyTo(array, 0);
            return array;
        }

        public string[] GetMastDelValuesArray()
        {
            Dictionary<string, string> mastDelData = this.GetMastDelData();
            string[] array = new string[mastDelData.Count];
            mastDelData.Keys.CopyTo(array, 0);
            return array;
        }

        public Dictionary<string, string> GetUserSelectedData()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < this.RightListBox.Items.Count; i++)
            {
                if (!dictionary.ContainsKey(this.RightListBox.Items[i].Value.Trim()))
                {
                    dictionary.Add(this.RightListBox.Items[i].Value.Trim(), this.RightListBox.Items[i].Text.Trim());
                }
            }
            return dictionary;
        }

        public string[] GetUserSelectedValuesArray()
        {
            Dictionary<string, string> userSelectedData = this.GetUserSelectedData();
            string[] array = new string[userSelectedData.Count];
            userSelectedData.Keys.CopyTo(array, 0);
            return array;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RightListBox.SelectionMode = ListSelectionMode.Multiple;
            this.SelectionMode = ListSelectionMode.Multiple;
            base.OnLoad(e);
            if (!this.Page.IsPostBack)
            {
                for (int i = 0; i < this.RightListBox.Items.Count; i++)
                {
                    this.Items.Remove(this.Items.FindByValue(this.RightListBox.Items[i].Value));
                    if (!this.BaseItems.ContainsKey(this.RightListBox.Items[i].Value.Trim()))
                    {
                        this.BaseItems.Add(this.RightListBox.Items[i].Value.Trim(), this.RightListBox.Items[i].Text.Trim());
                    }
                }
            }
            if (this.AddedBox.Value != null && this.AddedBox.Value != "")
            {
                string text = this.AddedBox.Value.Substring(0, this.AddedBox.Value.Length - 1);
                string[] array = text.Split(";".ToCharArray());
                List<ListItem> list = new List<ListItem>();
                for (int i = 0; i < array.Length; i++)
                {
                    string[] array2 = array[i].Trim().Split(",".ToCharArray());
                    this.RightListBox.Items.Add(new ListItem(array2[0], array2[1]));
                    this.Items.Remove(this.Items.FindByValue(array2[1]));
                }
                this.AddedBox.Value = "";
            }
            if (this.DeletedBox.Value != null && this.DeletedBox.Value != "")
            {
                string text2 = this.DeletedBox.Value.Substring(0, this.DeletedBox.Value.Length - 1);
                string[] array3 = text2.Split(";".ToCharArray());
                List<ListItem> list2 = new List<ListItem>();
                for (int i = 0; i < array3.Length; i++)
                {
                    string[] array2 = array3[i].Trim().Split(",".ToCharArray());
                    this.Items.Add(new ListItem(array2[0], array2[1]));
                    this.RightListBox.Items.Remove(this.RightListBox.Items.FindByValue(array2[1]));
                }
                this.DeletedBox.Value = "";
            }
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("MoveListBox"))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
                stringBuilder.AppendLine("function Add(Obj,Active,IsAll)");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("var row=Obj.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;");
                stringBuilder.AppendLine("var addedHidden=Obj.parentNode.parentNode.parentNode.parentNode.rows[4].cells[0].children[0];");
                stringBuilder.AppendLine("var deletedHidden=Obj.parentNode.parentNode.parentNode.parentNode.rows[4].cells[0].children[1];");
                stringBuilder.AppendLine("var FromSelect,ToSelect;");
                stringBuilder.AppendLine("if(Active=='toRight'){FromSelect=row.cells[0].children[0];ToSelect=row.cells[2].children[0];}");
                stringBuilder.AppendLine("else{FromSelect=row.cells[2].children[0];ToSelect=row.cells[0].children[0];}");
                stringBuilder.AppendLine("var sss=new Array(),l=0;");
                stringBuilder.AppendLine("if(IsAll){for(var i=0;i<FromSelect.options.length;i++){sss[l]=FromSelect.options[i];l++;}}");
                stringBuilder.AppendLine("else{for(var i=0;i<FromSelect.options.length;i++){if(FromSelect.options[i].selected){sss[l]=FromSelect.options[i];l++;}}}");
                stringBuilder.AppendLine("for(var i=0;i<sss.length;i++)");
                stringBuilder.AppendLine("{");
                // add by tree 20161025 ToSelect.options.add(oOption)改為位置調到賦值之後,oOption.innerText取不到值改為innerHTML
                //stringBuilder.AppendLine("var oOption = document.createElement('OPTION');ToSelect.options.add(oOption);oOption.innerText=sss[i].innerText;");
                //stringBuilder.AppendLine("oOption.value =sss[i].value;FromSelect.removeChild(sss[i]);var itemCode=sss[i].innerText+','+sss[i].value;");
                //stringBuilder.AppendLine("oOption.value =sss[i].value;ToSelect.options.add(oOption);FromSelect.removeChild(sss[i]);var itemCode=sss[i].innerText+','+sss[i].value;"); stringBuilder.AppendLine("if(Active=='toRight')");
                stringBuilder.AppendLine("var oOption = document.createElement('OPTION');oOption.innerHTML=sss[i].innerHTML;");
                // add by legend 2017/11/17 將 ToSelect.options.add 改為  ToSelect.options.appendChild
                //stringBuilder.AppendLine("oOption.value =sss[i].value;ToSelect.options.add(oOption);FromSelect.removeChild(sss[i]);var itemCode=sss[i].innerHTML+','+sss[i].value;"); stringBuilder.AppendLine("if(Active=='toRight')");
                stringBuilder.AppendLine("oOption.value =sss[i].value;ToSelect.options.appendChild(oOption);FromSelect.removeChild(sss[i]);var itemCode=sss[i].innerHTML+','+sss[i].value;"); stringBuilder.AppendLine("if(Active=='toRight')");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("if(deletedHidden.value!='')");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("var itemCodes=deletedHidden.value.split(';');var CanAdd=true;");
                stringBuilder.AppendLine("for(var j=0;j<itemCodes.length;j++){if(itemCodes[j]==itemCode){deletedHidden.value=deletedHidden.value.replace(itemCode+';','');CanAdd=false;}}");
                stringBuilder.AppendLine("if(CanAdd){addedHidden.value+=itemCode+';';}");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("else{addedHidden.value+=itemCode+';';}");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("else");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("if(addedHidden.value!='')");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("var itemCodes=addedHidden.value.split(';');var CanAdd=true;");
                stringBuilder.AppendLine("for(var j=0;j<itemCodes.length;j++){if(itemCodes[j]==itemCode){addedHidden.value=addedHidden.value.replace(itemCode+';','');CanAdd=false;}}");
                stringBuilder.AppendLine("if(CanAdd){deletedHidden.value+=itemCode+';';}");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("else{deletedHidden.value+=itemCode+';';}");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("SetButton(Obj,'button');");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("function SetButton(Obj,ctrlType)");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("var LeftSelect,RightSelect,ToRightAdd,ToLeftAdd,ToLeftAll,ToRightAll,row,table;");
                stringBuilder.AppendLine("if(ctrlType=='button'){table=Obj.parentNode.parentNode.parentNode.parentNode;row=table.parentNode.parentNode;}");
                //stringBuilder.AppendLine("else{row=Obj.parentNode.parentNode.parentNode.parentNode;table=row.rows[0].cells[1].children[0];}");
                // add by tree 20161025 左移和右移的取消disabled方法加不上獲取不到相應的對象， Obj為select獲取tr：Obj.parentNode.parentNode 獲取table：Obj.parentNode.parentNode.parentNode.parentNode
                stringBuilder.AppendLine("else{row=Obj.parentNode.parentNode;table=Obj.parentNode.parentNode.parentNode.parentNode.rows[0].cells[1].children[0];}");
                stringBuilder.AppendLine("LeftSelect=row.cells[0].children[0];RightSelect=row.cells[2].children[0];");
                stringBuilder.AppendLine("ToRightAdd=table.rows[0].cells[0].children[0];ToLeftAdd=table.rows[1].cells[0].children[0];");
                stringBuilder.AppendLine("ToRightAll=table.rows[2].cells[0].children[0];ToLeftAll=table.rows[3].cells[0].children[0];");
                stringBuilder.AppendLine("if(LeftSelect.options.length<=0){ToRightAdd.disabled='disabled';ToRightAll.disabled='disabled';}");
                stringBuilder.AppendLine("else");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("ToRightAll.disabled='';var ToRightAdddisabled='disabled';");
                stringBuilder.AppendLine("for(var i=0;i<LeftSelect.options.length;i++){if(LeftSelect.options[i].selected){ToRightAdddisabled='';break;}}");
                stringBuilder.AppendLine("ToRightAdd.disabled=ToRightAdddisabled;");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("if(RightSelect.options.length<=0){ToLeftAdd.disabled='disabled';ToLeftAll.disabled='disabled';}");
                stringBuilder.AppendLine("else");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("ToLeftAll.disabled='';var ToLeftAdddisabled='disabled';");
                stringBuilder.AppendLine("for(var i=0;i<RightSelect.options.length;i++){if(RightSelect.options[i].selected){ToLeftAdddisabled='';break;}}");
                stringBuilder.AppendLine("ToLeftAdd.disabled=ToLeftAdddisabled;");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine("</script>");
                // Legend 2016/10/21 將以上 "childNodes" 改為 "children" , 因IE11不支持
                this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "MoveListBox", stringBuilder.ToString());
            }
        }

        public override void DataBind()
        {
            base.DataBind();
            this.RightListBox.DataTextField = this.DataTextField;
            this.RightListBox.DataValueField = this.DataValueField;
            this.RightListBox.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Attributes.Add("onchange", "SetButton(this,'select');");
            this.RightListBox.Attributes.Add("onchange", "SetButton(this,'select');");
            this.RightListBox.Width = this.Width;
            this.RightListBox.Height = this.Height;
            writer.Write("<table  border=\"0\" cellspacing=\"3\" cellpadding=\"0\"><tr><td>");
            base.Render(writer);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("</td>");
            stringBuilder.AppendLine("<td width=\"10\"><table  border=\"0\" cellspacing=\"3\" cellpadding=\"0\">");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit\" value=\">\" onClick=\"Add(this,'toRight',false);\" disabled=\"disabled\" /></td>");
            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit2\" value=\"<\"  onClick=\"Add(this,'toLeft',false);\" disabled=\"disabled\" /></td>");
            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit3\" value=\">>\"  onClick=\"Add(this,'toRight',true);\" /></td>");
            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit4\" value=\"<<\" onClick=\"Add(this,'toLeft',true);\" /></td>");
            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("<tr><td>");
            writer.Write(stringBuilder.ToString());
            stringBuilder.Remove(0, stringBuilder.Length);
            this.AddedBox.RenderControl(writer);
            this.DeletedBox.RenderControl(writer);
            stringBuilder.AppendLine("</td></tr>");
            stringBuilder.AppendLine("</table></td>");
            stringBuilder.AppendLine("<td>");
            writer.Write(stringBuilder.ToString());
            stringBuilder.Remove(0, stringBuilder.Length);
            this.RightListBox.RenderControl(writer);
            stringBuilder.AppendLine("</td>");
            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("</table>");
            writer.Write(stringBuilder.ToString());
        }
    }
}
