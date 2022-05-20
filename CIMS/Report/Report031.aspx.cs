//******************************************************************
//*  作    者：lantaosu
//*  功能說明：晶片規格變化表 
//*  創建日期：2008-12-03
//*  修改日期：2008-12-04 18:00
//*  修改記錄：
//*            □2008-12-04
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

public partial class Report_Report031 : PageBase
{
    Report031BL bl = new Report031BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbWafer.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            //初始化頁面
            DataSet WaferList=bl.GetWaferList();
            dropWafer.DataSource = WaferList;
            dropWafer.DataTextField = "Wafer";
            dropWafer.DataValueField = "RID";
            dropWafer.DataBind();
            ListItem li = new ListItem("全部", "all");
            dropWafer.Items.Insert(0,li);
            dropWafer.SelectedValue = "all";

            btnExport.Visible = false;
        }
    }

    #region 列表數據綁定
    /// <summary>
    /// GridView數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbWafer_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;
        int i = 0;
        int j = 0;
        string[] strCardType=new string[UctrlCardType.GetRightItem.Rows.Count];
        string strCard = "";
        string[] Wafer_RID = null;
        string strWafer = "";
        string RID = dropWafer.SelectedValue;
        DataSet ds = null;

        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();        
        inputs.Add("UctrlCardType", UctrlCardType.GetRightItem);

        foreach (DataRow drowCardType in UctrlCardType.GetRightItem.Rows)
        {
            strCardType[i] = drowCardType["RID"].ToString();
            i = i + 1;

            strCard += drowCardType["RID"].ToString() + ",";

        }
        if (strCard != "")
        {
            strCard = strCard.Substring(0, strCard.Length - 1);

            Session["RIDs"] = strCard;
        }
        else
        {
            ds = bl.GetCardList();
            strCardType = new string[ds.Tables[0].Rows.Count];
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                strCardType[j] = dr["RID"].ToString();
                j = j + 1;

                strCard += dr["RID"].ToString() + ",";

            }
            strCard = strCard.Substring(0, strCard.Length - 1);

            Session["RIDs"] = strCard;
        }

        j = 0;
        if (RID == "all")
        {
            RID = "";
            ds = bl.GetWaferList();
            Wafer_RID = new string[ds.Tables[0].Rows.Count];
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RID += dr["RID"].ToString() + ",";
                Wafer_RID[j] = dr["RID"].ToString();
                j = j + 1;

                strWafer += dr["RID"].ToString() + ",";

            }
            strWafer = strWafer.Substring(0, (strWafer.Length - 1)); 
        }
        else
        {
            Wafer_RID = new string[1];
            strWafer = RID;            
        }
        Session["Wafer_RID"] = strWafer;

        DataSet dstlWafer = null;

        try
        {
            dstlWafer = bl.List(inputs, RID, txtBeginDate.Text, txtEndDate.Text, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

            if (dstlWafer != null)//如果查到了資料           
            {
                e.Table = dstlWafer.Tables[0];//要綁定的資料表
                e.RowCount = intRowCount;//查到的行數
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// GridView列數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbWafer_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    #endregion

    /// <summary>
    /// 查詢按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbWafer.BindData();
        btnExport.Visible = true;
    }    
}
