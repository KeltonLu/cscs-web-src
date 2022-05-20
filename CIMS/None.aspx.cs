//******************************************************************
//*  作    者：QingChen
//*  功能說明：臨時首頁視圖
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*  Legend 2018/04/02 添加分頁功能
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

public partial class None : PageBase
{
    #region 全局變量

    private static LogOperator _logOperator = new LogOperator();
    SysInfo002BL bl = new SysInfo002BL();

    /// <summary>
    /// 判斷GridView是否綁定過
    /// </summary>
    private bool IsBindData
    {
        get
        {
            if (ViewState["IsBindData"] == null)
            {
                return false;
            }

            return Convert.ToBoolean(ViewState["IsBindData"].ToString());
        }
        set
        {
            ViewState["IsBindData"] = value;
        }
    }

    /// <summary>
    /// 判斷是否點選過[確定]按鈕
    /// </summary>
    private bool IsSubmit
    {
        get
        {
            if (ViewState["IsSubmit"] == null)
            {
                return false;
            }

            return Convert.ToBoolean(ViewState["IsSubmit"].ToString());
        }
        set
        {
            ViewState["IsSubmit"] = value;
        }
    }

    #endregion

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbWarning.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSizeWarning"]);

        gvpbWarning.NoneData = "";

        HttpContext.Current.Session["Action"] = "8888";

        // 當點選過[確定]按鈕后, IsBindData才為false
        if (hidIsSubmit.Value == "Y")
        {
            IsBindData = false;
        }
        else
        {
            IsBindData = true;
        }

        if (!IsPostBack)
        {
            _logOperator.Write("登入成功后, 讀取DB中[警訊功能]資料開始");
            gridBind();
            _logOperator.Write("登入成功后, 讀取DB中[警訊功能]資料結束");
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string strRID = "";
        for (int i = 0; i < gvpbWarning.Rows.Count; i++)
        {
            if (((CheckBox)gvpbWarning.Rows[i].FindControl("cbIs_Show")).Checked)
            {
                strRID += gvpbWarning.DataKeys[i].Value.ToString() + ",";
            }
        }

        if (!StringUtil.IsEmpty(strRID))
        {
            strRID = strRID.Substring(0, strRID.Length - 1);
            bl.UpdataWarningInfo(strRID);
        }

        gridBind();

        // 將值更新為N, 用於點選跳頁連接
        hidIsSubmit.Value = "N";
    }

    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbWarning_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        if (IsBindData)
        {
            int intRowCount = 0;

            DataSet dstlWarning = null;

            try
            {
                dstlWarning = bl.getWARNING_INFO_List(e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);

                if (dstlWarning != null)//如果查到了資料
                {
                    e.Table = dstlWarning.Tables[0];//要綁定的資料表
                    e.RowCount = intRowCount;//查到的行數
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    #endregion 事件處理

    #region 自定義方法

    /// <summary>
    /// 資料綁定
    /// </summary>
    private void gridBind()
    {
        //gvpbWarning.DataSource = bl.getWARNING_INFO();
        //gvpbWarning.DataBind();

        IsBindData = true;
        gvpbWarning.BindData();
    }

    #endregion 自定義方法
}
