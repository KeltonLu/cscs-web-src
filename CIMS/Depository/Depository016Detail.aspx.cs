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

public partial class Depository_Depository016Detail : PageBase
{
    Depository016BL bl = new Depository016BL();

    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!IsPostBack)
        {
            //初始化UI資料
            DataSet ds = new DataSet();//品名
            DataSet dsPARAM = new DataSet();//異動項目
            DataSet dsFactory = new DataSet();//Perso廠名
            try
            {
                ds = bl.GetMaterielAll();
                dropName.DataSource = ds;
                dropName.DataValueField = "value";
                dropName.DataTextField = "Name";
                dropName.DataBind();

                dsFactory = bl.GetFactoryAll();
                dropFactoryRID.DataSource = dsFactory;
                dropFactoryRID.DataValueField = "RID";
                dropFactoryRID.DataTextField = "Factory_ShortName_CN";
                dropFactoryRID.DataBind();

                dsPARAM = bl.GetPARAM010203();
                dropPARAMRID.DataSource = dsPARAM;
                dropPARAMRID.DataValueField = "RID";
                dropPARAMRID.DataTextField = "Param_Name";
                dropPARAMRID.DataBind();

                hidActionType.Value = Request.QueryString["ActionType"].ToString();
                HidIndex.Value = Request.QueryString["Index"].ToString();

                //RID不為空，修改，設置當前介面訊息
                if (-1 != Convert.ToInt32(HidIndex.Value.ToString()))
                {
                    // 初始化特定UI資料
                    DataTable dtbl = (DataTable)Session["MaterielStocksTransaction"];
                    //獲取當前的資料行
                    DataRow drow = dtbl.Rows[int.Parse(HidIndex.Value.ToString())];
                    txtTransactionAmount.Text = Convert.ToInt32(drow["Transaction_Amount"]).ToString("N0");//數量  
                    dropName.SelectedValue = drow["Serial_Number"].ToString();//品名編號
                    dropFactoryRID.Text = drow["Factory_RID"].ToString();//廠商
                    dropPARAMRID.Text = drow["PARAM_RID"].ToString();//異動項目
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        //將UI中的資料存入session中，傳回相應界面
        string strRID = HidIndex.Value.ToString();
        string strActionType = hidActionType.Value.ToString();
        string TransactionID = Session["Transaction_ID"].ToString();
        DateTime TransactionDate = DateTime.ParseExact(TransactionID.Substring(0, 8), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        if (bl.Is_Managed(TransactionDate))
        {
            ShowMessage("移轉日後已匯入廠商結餘,無法新增或者修改明細");
            return;
        }

        // 介面資料檢查
        if (txtTransactionAmount.Text == "")
        {
            ShowMessage("數量不能為空");
            return;
        }
        if (StringUtil.IsEmpty(dropName.SelectedValue.ToString()))
        {
            ShowMessage("品名不能為空");
            return;
        }
        if (StringUtil.IsEmpty(dropFactoryRID.SelectedValue.ToString()))
        {
            ShowMessage("Perso廠不能為空");
            return;
        }
        if (StringUtil.IsEmpty(dropPARAMRID.SelectedValue.ToString()))
        {
            ShowMessage("異動項目不能為空");
            return;
        }

        DataTable dtMaterielStocksTransaction = (DataTable)this.Session["MaterielStocksTransaction"];
        DataRow drow = null;
        MATERIEL_STOCKS_TRANSACTION TransactionModel = null;
        int rid = 0;
        try
        {
            if ("Add" == strActionType)
            {
                TransactionModel = new MATERIEL_STOCKS_TRANSACTION();
                TransactionModel.Serial_Number = dropName.SelectedValue;
                TransactionModel.Transaction_Amount = Convert.ToInt32(txtTransactionAmount.Text.Replace(",", ""));
                TransactionModel.Factory_RID = Convert.ToInt32(dropFactoryRID.SelectedValue);
                TransactionModel.PARAM_RID = Convert.ToInt32(dropPARAMRID.SelectedValue);
                TransactionModel.Transaction_ID = TransactionID;
                TransactionModel.Transaction_Date = TransactionDate;
                rid = bl.Add(TransactionModel);
            }
            else
            {
                rid = int.Parse(Request.QueryString["Rid"].ToString());
                TransactionModel = bl.getModel(rid);
                TransactionModel.Serial_Number = dropName.SelectedValue;
                TransactionModel.Transaction_Amount = Convert.ToInt32(txtTransactionAmount.Text.Replace(",", ""));
                TransactionModel.Factory_RID = Convert.ToInt32(dropFactoryRID.SelectedValue);
                TransactionModel.PARAM_RID = Convert.ToInt32(dropPARAMRID.SelectedValue);
                bl.Update(TransactionModel);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
            return;
        }
        if (-1 == int.Parse(strRID))
        {
            drow = dtMaterielStocksTransaction.NewRow();
        }
        else
        {
            drow = dtMaterielStocksTransaction.Rows[int.Parse(strRID)];
        }
        drow["Serial_Number"] = dropName.SelectedValue;
        drow["Materiel_Name"] = dropName.SelectedItem.Text;
        drow["Transaction_Amount"] = txtTransactionAmount.Text.Replace(",", "");
        drow["Factory_RID"] = dropFactoryRID.SelectedValue;
        drow["Factory_Name"] = dropFactoryRID.SelectedItem.Text;
        drow["PARAM_RID"] = dropPARAMRID.SelectedValue;
        drow["PARAM_Name"] = dropPARAMRID.SelectedItem.Text;
        drow["RID"] = rid;
        // 新加
        if (-1 == int.Parse(strRID))
            dtMaterielStocksTransaction.Rows.Add(drow);
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "returnValue='1';window.close();", true);
    }
    #endregion event

    #region method
    #endregion method



}
