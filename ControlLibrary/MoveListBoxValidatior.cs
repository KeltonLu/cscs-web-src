using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace ControlLibrary
{
    /// <summary>
    /// MoveListBox���ұ��
    /// </summary>
    public class MoveListBoxValidatior : CustomValidator
    {
        public MoveListBoxValidatior()
            : base()
        {
            base.ClientValidationFunction = "ValidationMoveListBox";
            base.ValidateEmptyText = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            StringBuilder stbCode = new StringBuilder();
            stbCode.Append("<script language=\"javascript\" type=\"text/javascript\">");
            stbCode.Append("  function ValidationMoveListBox(source, arguments)");
            stbCode.Append(" {");
            stbCode.Append("if(document.getElementById(source.controltovalidate).parentNode.nextSibling.nextSibling.children[0].options.length>0)");
            stbCode.Append("{arguments.IsValid=true;}");
            stbCode.Append("else");
            stbCode.Append("{arguments.IsValid=false;}");
            stbCode.Append("}");
            stbCode.Append("</script> ");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MoveListBoxValidatior", stbCode.ToString());
        }
    }

}
