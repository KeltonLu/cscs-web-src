using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Depository_Depository011Mod : PageBase
{
    Depository011BL bl = new Depository011BL();

    #region 事件處理
    /// <summary>
    /// 頁面裝載
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbCardTypeStocksMove.NoneData = "";
        if (!IsPostBack)
        {
            btnExport.Enabled = false;
            btnExport1.Enabled = false;

            if (this.IsCheck())
            {
                ShowMessage("今天已經日結，不可修改卡片庫存移轉 ");
                btnSubmit1.Enabled = false;
                btnSubmit2.Enabled = false;
            }
            
            string strMove_ID = Request.QueryString["Move_ID"];
            if (StringUtil.IsEmpty(strMove_ID))
            {
                return;
            }

            try
            {
                // 取轉移單訊息
                this.lblMove_ID.Text = strMove_ID;
                this.hidMove_ID.Value = strMove_ID;
                DataSet dstStocksMove = bl.ListModel(strMove_ID);
                this.lblMove_Date.Text = Convert.ToDateTime(dstStocksMove.Tables[0].Rows[0]["Move_Date"]).ToString("yyyy/MM/dd");
                ViewState["CardTypeStocksMoveMod"] = dstStocksMove;
                ViewState["CardTypeStocksMoveModNum"] = dstStocksMove.Tables[0].Rows.Count;
                // 綁定到UI
                gvpbCardTypeStocksMove.BindData();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);   
            }
        }
    }

    /// <summary>
    /// 刪除明細(卡種庫存轉移單)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        // 刪除綁定資料
        DataSet dsStocksMove = (DataSet)ViewState["CardTypeStocksMoveMod"];
        dsStocksMove.Tables[0].Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
        ViewState["CardTypeStocksMoveModNum"] = Convert.ToInt32(ViewState["CardTypeStocksMoveModNum"]) - 1;
        ViewState["CardTypeStocksMoveMod"] = dsStocksMove;

        // 綁定到UI
        gvpbCardTypeStocksMove.BindData();
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            // 刪除庫存轉移單
            if (this.chkDel.Checked)
            {
                bl.Delete(this.lblMove_ID.Text);
                // 刪除成功
                ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc, "Depository011.aspx?Con=1");
            }
            // 保存庫存轉移單訊息
            else
            {
                if (0 == Convert.ToInt32(ViewState["CardTypeStocksMoveModNum"]))
                {
                    ShowMessage("沒有轉移單訊息，不能保存！");
                    return;
                }
                Dictionary<string, int> dirMoveNum = new Dictionary<string, int>();
                // UI資料檢查
                DataSet dstCardTypeStocksMove = (DataSet)ViewState["CardTypeStocksMoveMod"];
                for (int intRows = 0; intRows < gvpbCardTypeStocksMove.Rows.Count; intRows++)
                {
                    TextBox txtMove_Number = (TextBox)gvpbCardTypeStocksMove.Rows[intRows].FindControl("txtMove_Number");
                    String strMove_Number = txtMove_Number.Text.Trim().Replace(",", "");

                    if (StringUtil.IsEmpty(strMove_Number))
                    {
                        ShowMessage("轉移數量沒有輸入！");
                        return;
                    }

                    if (Convert.ToInt32(strMove_Number) < 0)
                    {
                        ShowMessage("數量不能小於零！");
                        return;
                    }
                    int oldmoveNumber = int.Parse(dstCardTypeStocksMove.Tables[0].Rows[intRows]["Move_Number"].ToString());
                   int changeNumber =(int.Parse(strMove_Number) - oldmoveNumber);

                    string strkey = dstCardTypeStocksMove.Tables[0].Rows[intRows]["From_Factory_RID"].ToString() + "-" + dstCardTypeStocksMove.Tables[0].Rows[intRows]["CardType_RID"].ToString();
                    //int number = Convert.ToInt32(drowStocksMove["Move_Number"].ToString());
                    if (dirMoveNum.ContainsKey(strkey))
                    {
                        dirMoveNum[strkey] = dirMoveNum[strkey] + changeNumber;

                    }
                    else
                    {
                        dirMoveNum.Add(strkey, changeNumber);
                    }

                    //if (!bl.ContainsPersoCardTypeDepository(dstCardTypeStocksMove.Tables[0].Rows[intRows]["CardType_RID"].ToString(),
                    //                                dstCardTypeStocksMove.Tables[0].Rows[intRows]["From_Factory_RID"].ToString(),
                    //                                changeNumber))
                    //{
                    //    ShowMessage("轉出數量超過轉出Perso廠的庫存數量！");
                    //    return;
                    //}    
                }
                foreach (KeyValuePair<string, int> key in dirMoveNum)
                {
                    string MoveNumber = key.Value.ToString();
                    string Perso_Rid = key.Key.Split('-')[0];
                    string CardType_Rid = key.Key.Split('-')[1];                

                    if (!bl.ContainsPersoCardTypeDepository(CardType_Rid, Perso_Rid, MoveNumber))
                    {
                        ShowMessage("總轉出數量超過轉出Perso廠的庫存數量！");
                        return;
                    }
                }
                // 將介面資訊保存到DataTable中
                for (int intRows = 0; intRows < gvpbCardTypeStocksMove.Rows.Count; intRows++)
                {
                    TextBox txtMove_Number = (TextBox)gvpbCardTypeStocksMove.Rows[intRows].FindControl("txtMove_Number");

                    dstCardTypeStocksMove.Tables[0].Rows[intRows]["Move_Number"] = Convert.ToInt32(txtMove_Number.Text.Trim().Replace(",",""));
                }

                // 新增卡種庫存轉移檔
                bl.Update(dstCardTypeStocksMove.Tables[0], this.lblMove_ID.Text);

                // 保存成功信息
                ShowMessage("保存成功");
                //ShowMessageAndGoPage("保存成功", "Depository011.aspx?Con=1");
                //ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, "Depository011.aspx?Con=1");

                gvpbCardTypeStocksMove.BindData();

                btnExport.Enabled = true;
                btnExport1.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCardTypeStocksMove_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        DataSet dstStocksMove = new DataSet();
        dstStocksMove = (DataSet)ViewState["CardTypeStocksMoveMod"];
        if (null != dstStocksMove)
        {
            e.Table = dstStocksMove.Tables[0];
            e.RowCount = dstStocksMove.Tables[0].Rows.Count;
        }
    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbCardTypeStocksMove_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblCardTypeStocksMove = (DataTable)gvpbCardTypeStocksMove.DataSource;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (0 == Convert.ToInt32(ViewState["CardTypeStocksMoveModNum"]))
                return;

            try
            {
                if (dtblCardTypeStocksMove.Rows[e.Row.RowIndex]["IS_CHECK"].ToString() == "Y")
                {
                    btnSubmit1.Enabled = false;
                    btnSubmit2.Enabled = false;
                }

                TextBox txtMove_Number = null;
             
                // 載入各Perso廠庫存量
                Label lbDepository_Stocks_Num = (Label)e.Row.FindControl("lbStocks_Num");
                DataSet CardTypeDepositoryNum = bl.SearchCardTypeDepositoryNum(dtblCardTypeStocksMove.Rows[e.Row.RowIndex]["CardType_rid"].ToString());

                // 在UI上顯示
                if (CardTypeDepositoryNum.Tables[0] != null)
                {
                    foreach (DataRow drowCardTypeDepositoryNum in CardTypeDepositoryNum.Tables[0].Rows)
                    {
                        lbDepository_Stocks_Num.Text += drowCardTypeDepositoryNum["Factory_ShortName_CN"].ToString() + ":";
                        int currentNum = bl.getCurrentCardNumber(drowCardTypeDepositoryNum["Perso_Factory_RID"].ToString(), dtblCardTypeStocksMove.Rows[e.Row.RowIndex]["CardType_rid"].ToString(), Convert.ToDateTime(drowCardTypeDepositoryNum["Stock_Date"]), Convert.ToInt32(drowCardTypeDepositoryNum["Stocks_Number"]));
                        //庫存量
                        //int stockNumber = Convert.ToInt32(drowCardTypeDepositoryNum["Stocks_Number"]);
                        //移轉量
                        //int oldMoveNumber = Convert.ToInt32(dtblCardTypeStocksMove.Rows[e.Row.RowIndex]["Move_Number"]);
                        //去除移轉後庫存數量
                        lbDepository_Stocks_Num.Text += currentNum.ToString("N0")  + "<br>";
                    }
                }
                // 轉移數量
                txtMove_Number = (TextBox)e.Row.FindControl("txtMove_Number");
                txtMove_Number.Text = Convert.ToInt32(dtblCardTypeStocksMove.Rows[e.Row.RowIndex]["Move_Number"]).ToString("N0");

                // 添加刪除Button的邦定事件
                ImageButton ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
                ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
                ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
            }
            catch(Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
    #endregion
    
    #endregion
}