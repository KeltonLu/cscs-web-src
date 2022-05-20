using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ControlLibrary
{
    /// <summary>
    /// 黨查無資料時顯示的假表格
    /// </summary>
   public class FakeGridView:WebControl
    {

        private string strHeadText = null;

        [DefaultValue(null), Localizable(true)]
        public string HeadText
        {
            get
            {
                return this.strHeadText;
            }
            set
            {
                this.strHeadText = value;
            }
        }

        public FakeGridView()
        {
            base.Visible = false;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.strHeadText != null && this.strHeadText != "")
            {
                StringBuilder stringBuilder = new StringBuilder("<table id=\"");
                stringBuilder.Append(this.UniqueID);
                stringBuilder.Append("\" width=\"100%\" rules=\"all\"  border=\"1\"  cellspacing=\"0\" style=\"border: 1px solid #404040;border-collapse:collapse;\"><tr bgcolor=\"#B9BDAA\" style=\"font-weight:bold;\">");
                string[] array = this.strHeadText.Split(new char[]
                {
                    char.Parse(",")
                });
                for (int i = 0; i < array.Length; i++)
                {
                    stringBuilder.Append("<td scope=\"col\" align=\"center\">");
                    stringBuilder.Append(array[i]);
                    stringBuilder.Append("</td>");
                }
                stringBuilder.Append("</tr></tsble>");
                writer.WriteLine(stringBuilder.ToString());
            }
        }
    }
}
