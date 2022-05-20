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

public partial class Depository_Depository011Add : PageBase
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
        gvpbCardTypeStocksMove.NoneData = "無可供轉移卡片庫存";

        if (!IsPostBack)
        {
            this.txtMove_Date.Text = System.DateTime.Now.ToString("yyyy/MM/dd");
            ViewState["CardTypeStocksMoveAdd"] =null;
            ViewState["CardTypeStocksMoveAddNum"] = 0;
        }
    }

    /// <summary>
    /// 在緩存中刪除一條移轉明細記錄
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        DataTable dtblMove = (DataTable)ViewState["CardTypeStocksMoveAdd"];
        DataRow drowMove = dtblMove.Rows[Convert.ToInt32(e.CommandArgument)];
        dtblMove.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));
        ViewState["CardTypeStocksMoveAdd"] = dtblMove;
        gvpbCardTypeStocksMove.BindData();    
    }

    /// <summary>
    /// 將已選卡種的移轉明細
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDetailAdd_Click(object sender, EventArgs e)
    {
        //if (Convert.ToDateTime(this.txtMove_Date.Text) < Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")))
        //{
        //    ShowMessage("日期必須等於或晚於當日！");
        //    return;
        //}
        if (((DataTable)UctrlCardType.GetRightItem).Rows.Count <= 0)
        {
            ShowMessage("卡種沒有選擇，請先選擇卡種！");
            return;
        }
        try
        {

            //查詢條件集合
            Dictionary<string, object> inputs = new Dictionary<string, object>();
            inputs.Add("selectedCardType", this.UctrlCardType.GetRightItem);//要轉移的卡種

            DataTable dtblCardTypeStocksMove = null;
            dtblCardTypeStocksMove = bl.getCardType(inputs);
            //添加查詢結果到列表中
            DataTable viewTable = (DataTable)ViewState["CardTypeStocksMoveAdd"];
            if (viewTable != null)
            {
                foreach (DataRow drownew in dtblCardTypeStocksMove.Rows)
                {
                    DataRow drow = viewTable.NewRow();
                    drow.ItemArray = drownew.ItemArray;
                    viewTable.Rows.Add(drow);
                }
            }
            else
            {
                viewTable = dtblCardTypeStocksMove;
            }     

            //幫定gridview
            ViewState["CardTypeStocksMoveAdd"] = viewTable;
            gvpbCardTypeStocksMove.BindData();

            //清空已選擇卡種
            DataTable blankTable = new DataTable();
            blankTable.Columns.Add("Rid");
            blankTable.Columns.Add("Name");
            UctrlCardType.SetRightItem = blankTable;
            UctrlCardType.LbLeftBind();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 保存訊息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (Convert.ToDateTime(txtMove_Date.Text) < Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")))
        {
            ShowMessage("日期必須等於或晚於當日！");
            return;
        }
        if (!bl.isWorkDay(txtMove_Date.Text))
        {
            ShowMessage("日期必需為工作日！");
            return;
        }
        if (bl.isCheck(Convert.ToDateTime(txtMove_Date.Text)))
        {
            ShowMessage("日期必須為非日結日！");
            return;
        }
        if (Convert.ToInt32(ViewState["CardTypeStocksMoveAddNum"]) == 0)
        {
            ShowMessage("沒有要保存的卡種庫存轉移單訊息！");
            return;
        }

        try
        {
            // 介面資料檢查
            DataTable dtCardMove = (DataTable)ViewState["CardTypeStocksMoveAdd"];  
            Dictionary<string ,int> dirMoveNum = new Dictionary<string,int>();
            for (int intRows = 0; intRows < gvpbCardTypeStocksMove.Rows.Count; intRows++)
            {
                TextBox txtMove_Number = (TextBox)gvpbCardTypeStocksMove.Rows[intRows].FindControl("txtMove_Number");   
               
                DropDownList drpFrom_Factory = (DropDownList)gvpbCardTypeStocksMove.Rows[intRows].FindControl("drpFrom_Factory");
                DropDownList drpTo_Factory = (DropDownList)gvpbCardTypeStocksMove.Rows[intRows].FindControl("drpTo_Factory");
                String strMove_Number = txtMove_Number.Text.Trim().Replace(",","");

                if (StringUtil.IsEmpty(strMove_Number))
                {
                    ShowMessage("數量不能為空！");
                    return;
                }

                if (Convert.ToInt32(strMove_Number) <= 0)
                {
                    ShowMessage("數量必須大於零！");
                    return;
                }

                if (drpFrom_Factory.SelectedValue.ToString() == drpTo_Factory.SelectedValue.ToString())
                {
                    ShowMessage("轉入Perso廠和轉出Perso廠相同！");
                    return;
                }

                //if (!bl.ContainsPersoCardTypeDepository(dtCardMove.Rows[intRows]["RID"].ToString(), 
                //                            drpFrom_Factory.SelectedValue.ToString(),
                //                            strMove_Number) )
                //{
                //    ShowMessage("轉出數量超過轉出Perso廠的庫存數量！");
                //    return;
                //}
            }

            // 將UI訊息保存到DataTable中
            for (int intRows = 0; intRows < gvpbCardTypeStocksMove.Rows.Count; intRows++)
            {
                TextBox txtMove_Number = (TextBox)gvpbCardTypeStocksMove.Rows[intRows].FindControl("txtMove_Number");
                DropDownList drpFrom_Factory = (DropDownList)gvpbCardTypeStocksMove.Rows[intRows].FindControl("drpFrom_Factory");
                DropDownList drpTo_Factory = (DropDownList)gvpbCardTypeStocksMove.Rows[intRows].FindControl("drpTo_Factory");
                String strMove_Number = txtMove_Number.Text.Trim().Replace(",", "");

                dtCardMove.Rows[intRows]["Move_Number"] = Convert.ToInt32(strMove_Number);
                dtCardMove.Rows[intRows]["From_Factory_RID"] = Convert.ToInt32(drpFrom_Factory.SelectedValue);
                dtCardMove.Rows[intRows]["To_Factory_RID"] = Convert.ToInt32(drpTo_Factory.SelectedValue);
            }
            //匯總同一卡種同一Perso廠的移轉記錄
            foreach (DataRow drowStocksMove in dtCardMove.Rows)
            {
                string strkey = drowStocksMove["From_Factory_RID"].ToString() + "-" + drowStocksMove["RID"].ToString();
                int number = Convert.ToInt32(drowStocksMove["Move_Number"].ToString());
                if (dirMoveNum.ContainsKey(strkey))
                {
                    dirMoveNum[strkey] = dirMoveNum[strkey] + number;

                }
                else
                {
                    dirMoveNum.Add(strkey, number);
                }
            }
            foreach (KeyValuePair<string, int> key in dirMoveNum)
            {
                string MoveNumber =key.Value.ToString();
                string Perso_Rid = key.Key.Split('-')[0];
                string CardType_Rid = key.Key.Split('-')[1];               

                if (!bl.ContainsPersoCardTypeDepository(CardType_Rid,Perso_Rid,MoveNumber))
                {
                    ShowMessage("總轉出數量超過轉出Perso廠的庫存數量！");
                    return;
                }            
            }

            // 新增卡種庫存轉移檔
            bl.Add(dtCardMove, txtMove_Date.Text);

            ShowMessageAndGoPage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc, string.Concat("Depository011Add.aspx", GlobalString.PageUrl.ACTION_TYPE, GlobalString.ActionName.ADD));
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
        
        DataTable dtblCardTypeMoveAdd = (DataTable)ViewState["CardTypeStocksMoveAdd"];
        e.Table = dtblCardTypeMoveAdd;
        e.RowCount = dtblCardTypeMoveAdd.Rows.Count;
        ViewState["CardTypeStocksMoveAddNum"] = e.RowCount;
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
            //if (0 == Convert.ToInt32(this.Session["CardTypeStocksMoveAddNum"]))
            //    return;

            try
            {
                Label lbDepository_Stocks_Num = null;

                lbDepository_Stocks_Num = (Label)e.Row.FindControl("lbDepository_Stocks_Num");

                // 載入各Perso廠的庫存量
                DataSet CardTypeDepositoryNum = bl.SearchCardTypeDepositoryNum(dtblCardTypeStocksMove.Rows[e.Row.RowIndex]["RID"].ToString());

                // 在UI上顯示Pero廠的對應卡種的庫存數量
                if (CardTypeDepositoryNum.Tables[0] != null)
                {
                    foreach (DataRow drowCardTypeDepositoryNum in CardTypeDepositoryNum.Tables[0].Rows)
                    {
                        lbDepository_Stocks_Num.Text += drowCardTypeDepositoryNum["Factory_ShortName_CN"].ToString() + ":";
                        int currentNum = bl.getCurrentCardNumber(drowCardTypeDepositoryNum["Perso_Factory_RID"].ToString(), dtblCardTypeStocksMove.Rows[e.Row.RowIndex]["RID"].ToString(), Convert.ToDateTime(drowCardTypeDepositoryNum["Stock_Date"]), Convert.ToInt32(drowCardTypeDepositoryNum["Stocks_Number"]));
                        lbDepository_Stocks_Num.Text += currentNum.ToString("N0") + "<br>";
                    }
                }

                // 綁定轉出Perso廠
                DropDownList drpFrom_Factory = (DropDownList)e.Row.FindControl("drpFrom_Factory");
                drpFrom_Factory.DataValueField = "Perso_Factory_RID";
                drpFrom_Factory.DataTextField = "Factory_ShortName_CN";
                drpFrom_Factory.DataSource = CardTypeDepositoryNum.Tables[0];
                drpFrom_Factory.DataBind();
                // 綁定轉入Perso廠
                DropDownList drpTo_Factory = (DropDownList)e.Row.FindControl("drpTo_Factory");           
                drpTo_Factory.DataSource = bl.GetFactoryList();
                drpTo_Factory.DataBind();
                //刪除按鈕的綁定
                ImageButton ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
                ibtnButton.CommandArgument = e.Row.RowIndex.ToString();
                ibtnButton.OnClientClick = string.Concat("return confirm(\'", BizMessage.BizCommMsg.ALT_CMN_IsDel, "\')");
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