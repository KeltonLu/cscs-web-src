//******************************************************************
//*  作    者：lantaosu
//*  功能說明：換卡預測物料月報表 
//*  創建日期：2008-11-28
//*  修改日期：2008-12-01 12:00
//*  修改記錄：
//*            □2008-12-01
//*              1.創建 蘇斕濤
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

public partial class Report_Report025 : PageBase
{
    Report025BL bl = new Report025BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //初始化頁面  
            dropYearFrom.SelectedValue = DateTime.Now.Year.ToString();
            dropYearTo.SelectedValue = DateTime.Now.Year.ToString();

            dropMonthFrom.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');
            dropMonthTo.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');
        }
    }

    /// <summary>
    /// 匯出報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        if (int.Parse(dropYearFrom.SelectedValue + dropMonthFrom.SelectedValue) > int.Parse(dropYearTo.SelectedValue + dropMonthTo.SelectedValue))
        {
            ShowMessage("年月起不能大於迄");
            return;
        }

        if (!bl.IsChangeCard(dropYearFrom.SelectedValue+dropMonthFrom.SelectedValue,dropYearTo.SelectedValue+dropMonthTo.SelectedValue))
        {
            ShowMessage("查詢期間超過已匯入年度換卡預測表範圍");
            return;
        }

        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        HidTime.Value = time;
        int i = 0;
        string Name = "";
        DataTable date = new DataTable();
        DataSet ds = bl.GetList(dropYearFrom.SelectedValue + dropMonthFrom.SelectedValue, dropYearTo.SelectedValue + dropMonthTo.SelectedValue);
        date.Columns.Add("Change_Date",Type.GetType("System.String")); //記錄一共有多少日期
        DataRow drow = null;
        DataTable dtbl = ds.Tables[0].Clone();   //用於整理GetList的結果集
        foreach (DataRow dr2 in ds.Tables[0].Rows)
        {
            drow = dtbl.NewRow();
            drow.ItemArray = dr2.ItemArray;
            dtbl.Rows.Add(drow);
        }

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            if (date.Select("Change_Date=" + dr["Change_Date"].ToString()).Length > 0)
                continue;
            else
            {
                drow = date.NewRow();
                drow["Change_Date"] = dr["Change_Date"].ToString();
                date.Rows.Add(drow);
            }   
        }

        foreach (DataRow dr1 in ds.Tables[0].Rows)
        {
            if (dr1["Name"].ToString() != Name)
            {
                Name = dr1["Name"].ToString();
                for (i = 0; i < date.Rows.Count; i++)
                {
                    if (ds.Tables[0].Select("Name='" + dr1["Name"].ToString() + "' and Change_Date=" + date.Rows[i][0].ToString()).Length > 0)
                        continue;
                    else
                    {
                        drow = dtbl.NewRow();
                        drow["strType"] = dr1["strType"];
                        drow["Name"] = dr1["Name"].ToString();
                        drow["Number"] = 0;
                        drow["Change_Date"] = date.Rows[i][0].ToString();
                        dtbl.Rows.Add(drow);
                    }
                }
            }
        }

        bl.AddReport(dtbl,time);

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "exportExcel();", true);
    }
}
