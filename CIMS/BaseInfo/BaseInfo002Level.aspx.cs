//******************************************************************
//*  作    者：FangBao
//*  功能說明：合約級距管理
//*  創建日期：2008-09-16
//*  修改日期：2008-09-16 12:00
//*  修改記錄：
//*            □2008-09-16
//*              1.創建 鮑方
//*******************************************************************

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;


public partial class BaseInfo_BaseInfo002Level : PageBase
{
    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RowId"];



        DataTable dtblLevel = ((Dictionary<string, DataTable>)Session["Agreement"])["LevelTmp"];
        DataRow drowLevel=null;

        if(!StringUtil.IsEmpty(strRID))
            drowLevel = dtblLevel.Rows[int.Parse(strRID)];


        if (!IsPostBack)
        {
            if (drowLevel != null)
                txtLevel_Max.Text = drowLevel["級距"].ToString().Split('~')[1];



            if (drowLevel != null)
                txtLevel_Min.Text = drowLevel["級距"].ToString().Split('~')[0];
        }

        for (int i = 1; i < dtblLevel.Columns.Count; i++)
        {
            HtmlTableRow tr3 = new HtmlTableRow();
            tr3.VAlign = "baseline";
            HtmlTableCell tc3_1 = new HtmlTableCell();
            HtmlTableCell tc3_2 = new HtmlTableCell();
            tc3_1.Align = "right";
            tc3_2.Align = "left";
            tc3_1.InnerHtml = "<span class=\"style1\">*</span>"+dtblLevel.Columns[i].ColumnName+"：";
            //tc3_1.InnerText = 
            TextBox txt3 = new TextBox();
            txt3.ID = "txt" + dtblLevel.Columns[i].ColumnName.Replace(" ","");
            txt3.Attributes.Add("onfocus", "DelDouhao(this)");
            txt3.Attributes.Add("onblur", "CheckAmt('" + txt3.ID + "',8,2);value=GetValue(this.value)");
            txt3.Attributes.Add("onkeyup", "CheckAmt('" + txt3.ID + "',8,2)");
            txt3.MaxLength = 13;// <% --Dana 20161021 最大長度由11改為13-- %>
            if (drowLevel != null)
                txt3.Text = drowLevel[dtblLevel.Columns[i].ColumnName].ToString();
            txt3.Style.Value = "ime-mode:disabled;text-align: right";
            RequiredFieldValidator rfv = new RequiredFieldValidator();
            rfv.ID = "rfv" + dtblLevel.Columns[i].ColumnName.Replace(" ", ""); ;
            rfv.ControlToValidate = "txt" + dtblLevel.Columns[i].ColumnName.Replace(" ", "");
            rfv.ErrorMessage = dtblLevel.Columns[i].ColumnName + "不能為空";
            rfv.Text = " *";
            tc3_2.Controls.AddAt(0, txt3);
            tc3_2.Controls.AddAt(1, rfv);
            //tc3_2.Controls.Add(txt3);
            tr3.Cells.Add(tc3_1);
            tr3.Cells.Add(tc3_2);
            tbLevel.Rows.Add(tr3);
        }

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string strRID = Request.QueryString["RowId"];

        DataTable dtblLevel = ((Dictionary<string, DataTable>)Session["Agreement"])["LevelTmp"];

        decimal dectxtMax = Convert.ToDecimal(txtLevel_Max.Text.Replace(",", ""));
        decimal dectxtMin = Convert.ToDecimal(txtLevel_Min.Text.Replace(",", ""));

        if (dectxtMax <= dectxtMin)
        {
            ShowMessage("最大數量應該大於最小數量");
            return;
        }

        for (int i = 0; i < dtblLevel.Rows.Count; i++)
        {
            if (!StringUtil.IsEmpty(strRID))
            {
                if (i == int.Parse(strRID))
                    continue;
            }
            decimal decMax = Convert.ToDecimal(dtblLevel.Rows[i]["級距"].ToString().Split('~')[1].Replace(",",""));
            decimal decMin = Convert.ToDecimal(dtblLevel.Rows[i]["級距"].ToString().Split('~')[0].Replace(",", ""));

            if ((dectxtMin >= decMin && dectxtMin <= decMax) || (dectxtMax >= decMin && dectxtMax <= decMax))
            {
                ShowMessage("級距區間不能重複");
                return;
            }

            if ((decMin >= dectxtMin && decMin <= dectxtMax) || (decMax >= dectxtMin && decMax <= dectxtMax))
            {
                ShowMessage("級距區間不能重複");
                return;
            }
        }

        for (int i = 1; i < dtblLevel.Columns.Count; i++)
        {
            TextBox txt = (TextBox)this.FindCtrl("txt" + dtblLevel.Columns[i].ColumnName.Replace(" ",""));
            if (txt != null)
            {
                if (StringUtil.IsEmpty(txt.Text))
                {
                    ShowMessage(txt.ID.Replace("txt", "") + "不能為空");
                    return;
                }
            }
        }
        DataRow drowLevel = null;

        if (StringUtil.IsEmpty(strRID))
            drowLevel = dtblLevel.NewRow();
        else
            drowLevel = dtblLevel.Rows[int.Parse(strRID)];

        drowLevel["級距"] = ((TextBox)this.FindCtrl("txtLevel_Min")).Text + "~" + ((TextBox)this.FindCtrl("txtLevel_Max")).Text;

        for (int i = 1; i < dtblLevel.Columns.Count; i++)
        {
            TextBox txt = (TextBox)this.FindCtrl("txt" + dtblLevel.Columns[i].ColumnName.Replace(" ", ""));
            drowLevel[dtblLevel.Columns[i].ColumnName] = Convert.ToDecimal(txt.Text.Replace(",", "")).ToString("N2");
        }

        if (StringUtil.IsEmpty(strRID))
            dtblLevel.Rows.Add(drowLevel);

        Response.Write("<script>returnValue='1';window.close();</script>");

    }
    #endregion
}
