// 創建者：Judy
// 功能說明: 查詢 Excute(insert delete update) 匯出Excel 匯出CSV
// 創建時間：2018/03/27
// 修改時間：
// 修改者：
using ADODB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FileUpload_QReport : System.Web.UI.Page
{
    #region 全局變量

    private DataTable QueryTable
    {
        get
        {
            if (ViewState["QueryTable"] == null)
            {
                return null;
            }

            return ViewState["QueryTable"] as DataTable;
        }
        set
        {
            ViewState["QueryTable"] = value;
        }
    }

    #endregion 全局變量

    #region 頁面加載方法
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            gridViewResult.Visible = false;
            // "Data Source=192.168.2.105;Initial Catalog=CIMS_SIT;Persist Security Info=True;User ID=sa;Password=@server105;"
            txtConnect.Text = ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString;
        }
    }

    #endregion

    #region 按鈕事件

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        label.Visible = false;
        label.Text = "";
        try
        {
            // Connection String,SQL Command為空白時提示語
            // Connection String,SQL Command非空
            if (GetPrompt())
            {
                SqlConnection conn = new SqlConnection(txtConnect.Text.Trim());
                SqlDataAdapter da = new SqlDataAdapter(txtCommand.Text.Trim(), conn);
                DataSet ds = new DataSet();

                da.Fill(ds);
                QueryTable = ds.Tables[0];
                gridViewResult.DataSource = QueryTable;
                gridViewResult.DataBind();
                gridViewResult.Visible = true;
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            label.Text = ex.Message;
            label.Visible = true;
        }
    }

    /// <summary>
    /// 返回受影響的行數
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnExecute_Click(object sender, EventArgs e)
    {
        label.Visible = false;
        gridViewResult.Visible = false;

        // 查詢結果清空
        QueryTable = null;

        try
        {
            // Connection String,SQL Command為空白時提示語
            // Connection String,SQL Command非空
            if (GetPrompt())
            {
                SqlConnection conn = new SqlConnection(txtConnect.Text.Trim());
                conn.Open();
                SqlCommand cmd = new SqlCommand(txtCommand.Text.Trim(), conn);
                int result = cmd.ExecuteNonQuery();
                conn.Close();
                label.Text = "(" + result.ToString() + " 個資料列受到影響)";
                label.Visible = true;
            }
        }
        catch (Exception ex)
        {
            label.Text = ex.Message;
            label.Visible = true;
        }
    }

    /// <summary>
    /// 匯出Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dtExport = QueryTable;
            StringBuilder strBuilderExcelContent = new StringBuilder();
            int i = 0;
            string trBg = "";

            if (dtExport != null && dtExport.Rows.Count > 0)
            {
                strBuilderExcelContent.Append("<div><table border='1'>");

                // 表頭
                strBuilderExcelContent.Append("<tr>");
                for (i = 0; i < dtExport.Columns.Count; i++)
                {
                    strBuilderExcelContent.Append("<th>" + dtExport.Columns[i].ColumnName + "</th>");
                }
                strBuilderExcelContent.Append("</tr>");

                // 資料
                foreach (DataRow dr in dtExport.Rows)
                {
                    // 隔行變色
                    if (trBg == "")
                    {
                        trBg = "#F1EED5";
                    }
                    else
                    {
                        trBg = "";
                    }
                    strBuilderExcelContent.Append("<tr style='background:" + trBg + "'>");

                    Array objs = dr.ItemArray;

                    foreach (object obj in objs)
                    {
                        string str = "";

                        if (obj != null)
                        {
                            str = obj.ToString();
                        }

                        strBuilderExcelContent.Append("<th>'" + str + "</th>");
                    }
                    strBuilderExcelContent.Append("</tr>");
                }

                strBuilderExcelContent.Append("</table></div>");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=Down.xls");
                Response.ContentEncoding = Encoding.UTF8;
                Response.ContentType = "application/excel";
                Response.Write(strBuilderExcelContent);
                Response.End();
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('請查詢出資料后, 再匯出!');", true);
            }
        }
        catch (Exception ex)
        {
            label.Text = ex.Message;
            label.Visible = true;
        }
    }

    /// <summary>
    /// 匯出CSV
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCsv_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dtExport = QueryTable;
            StringBuilder strBuilderCsv = new StringBuilder();
            int i = 0;

            if (dtExport != null && dtExport.Rows.Count > 0)
            {
                // 表頭
                for (i = 0; i < dtExport.Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        strBuilderCsv.Append("\"" + dtExport.Columns[i].ColumnName + "\"");
                    }
                    else
                    {
                        strBuilderCsv.Append(",\"" + dtExport.Columns[i].ColumnName + "\"");
                    }
                }
                strBuilderCsv.Append(Environment.NewLine);

                // 資料
                foreach (DataRow dr in dtExport.Rows)
                {                 
                    Array objs = dr.ItemArray;
                    i = 0;
                    foreach (object obj in objs)
                    {
                        string str = "";

                        if (obj != null)
                        {
                            str = obj.ToString();
                        }
                        if (i == 0)
                        {
                            i = 1;
                            strBuilderCsv.Append("\"" + str + "\"");
                        }
                        else
                        {
                            strBuilderCsv.Append(",\"" + str + "\"");
                        }
                    }
                    strBuilderCsv.Append(Environment.NewLine);
                }

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=Down.txt");
                Response.ContentEncoding = Encoding.UTF8;
                Response.ContentType = "application/vnd.ms-word";
                Response.Write(strBuilderCsv);
                Response.End();
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('請查詢出資料后, 再匯出!');", true);
            }
        }
        catch (Exception ex)
        {
            label.Text = ex.Message;
            label.Visible = true;
        }
    }


    #endregion 按鈕事件

    #region 自定義方法

    /// <summary>
    /// Connection String,SQL Command為空白時提示語
    /// </summary>
    /// <returns>true: 驗證OK, false: 提示訊息，不往下進行</returns>
    private bool GetPrompt()
    {
        bool blnReturn = true;

        // Connection String 為空白時
        if (txtConnect.Text.Trim() == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('Connection String 不可為空白!');", true);
            blnReturn = false;
        }

        // SQL Command為空白時
        if (txtCommand.Text.Trim() == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "alert('SQL Command不可為空白!');", true);
            blnReturn = false;
        }

        return blnReturn;
    }

    #endregion 自定義方法
}