//******************************************************************
//*  作    者：lantaosu
//*  功能說明：物料庫存專業作業
//*  創建日期：2008-09-09
//*  修改日期：2008-09-12 12:00
//*  修改記錄：
//*            □2008-09-09
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


public partial class Depository_Depository011_1Detail : PageBase
{
    Depository011_1BL bl = new Depository011_1BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";

        if (!IsPostBack)
        {
            //初始化UI資料
            DataSet ds = new DataSet();//品名
            DataSet dsFromFactory = new DataSet();//Perso廠名
            DataSet dsToFactory = new DataSet();//Perso廠名
            try
            {
                ds = bl.GetMaterielAll();
                dropName.DataSource = ds;
                dropName.DataValueField = "value";
                dropName.DataTextField = "Name";
                dropName.DataBind();

                dsFromFactory = bl.GetFactoryAll();

                // 轉出Perso廠
                dropFromFactoryRID.DataSource = dsFromFactory;
                dropFromFactoryRID.DataValueField = "RID";
                dropFromFactoryRID.DataTextField = "Factory_ShortName_CN";
                dropFromFactoryRID.DataBind();
                // 轉入Perso廠
                dropToFactoryRID.DataSource = dsFromFactory;
                dropToFactoryRID.DataValueField = "RID";
                dropToFactoryRID.DataTextField = "Factory_ShortName_CN";
                dropToFactoryRID.DataBind();

                hidActionType.Value = Request.QueryString["ActionType"].ToString();
                HidIndex.Value = Request.QueryString["Index"].ToString();

                //RID不為空，修改，設置當前介面訊息
                if (-1 != Convert.ToInt32(HidIndex.Value.ToString()))
                {
                    // 初始化特定UI資料
                    DataTable dtbl = (DataTable)Session["MaterielStocksMove"];
                    //獲取當前的資料行
                    DataRow drow = dtbl.Rows[int.Parse(HidIndex.Value.ToString())];
                    txtMoveNumber.Text = Convert.ToInt32(drow["Move_Number"]).ToString("N0");//數量  
                    dropName.SelectedValue = drow["Serial_Number"].ToString();//品名編號
                    dropFromFactoryRID.Text = drow["From_Factory_RID"].ToString();//轉出廠商
                    dropToFactoryRID.Text = drow["To_Factory_RID"].ToString();//轉入廠商
                }

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmitDn_Click(object sender, EventArgs e)
    {
        //將UI中的資料存入session中，傳回相應界面
        string strRID = HidIndex.Value.ToString();
        string strActionType = hidActionType.Value.ToString();
        string moveId = Session["Move_ID"].ToString();
        DateTime moveDate = DateTime.ParseExact(moveId.Substring(0, 8), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        if (bl.Is_Managed(moveDate)) {
            ShowMessage("移轉日後已匯入廠商結餘,無法新增或者修改明細");
            return;
        }
        // 介面資料檢查
        if (txtMoveNumber.Text == "")
        {
            ShowMessage("數量不能為空");
            return;
        }
        if (StringUtil.IsEmpty(dropName.SelectedValue.ToString()))
        {
            ShowMessage("品名不能為空");
            return;
        }

        if (StringUtil.IsEmpty(dropFromFactoryRID.SelectedValue.ToString()))
        {
            ShowMessage("轉出Perso廠不能為空");
            return;
        }

        if (StringUtil.IsEmpty(dropToFactoryRID.SelectedValue.ToString()))
        {
            ShowMessage("轉入Perso廠不能為空");
            return;
        }

        if (dropFromFactoryRID.SelectedValue.ToString() == dropToFactoryRID.SelectedValue.ToString())
        {
            ShowMessage("轉出Perso廠和轉入Perso廠不能相同");
            return;
        }      

        DataTable dtMaterielStocksMove = (DataTable)this.Session["MaterielStocksMove"];
        DataRow drow = null;
        MATERIEL_STOCKS_MOVE moveModel = null;
        int rid = 0;
        try
        {
            if ("Add" == strActionType)
            {
                moveModel = new MATERIEL_STOCKS_MOVE();    
                moveModel.Serial_Number = dropName.SelectedValue;
                moveModel.Move_Number = Convert.ToInt32(txtMoveNumber.Text.Replace(",", ""));
                moveModel.To_Factory_RID = Convert.ToInt32(dropToFactoryRID.SelectedValue);
                moveModel.From_Factory_RID = Convert.ToInt32(dropFromFactoryRID.SelectedValue);
                moveModel.Move_ID = moveId;
                moveModel.Move_Date = moveDate;
                rid = bl.Add(moveModel);
            }
            else
            {
                rid = int.Parse(Request.QueryString["Rid"].ToString());
                moveModel = bl.getModel(rid);
                moveModel.Serial_Number = dropName.SelectedValue;
                moveModel.Move_Number = Convert.ToInt32(txtMoveNumber.Text.Replace(",", ""));
                moveModel.To_Factory_RID = Convert.ToInt32(dropToFactoryRID.SelectedValue);
                moveModel.From_Factory_RID = Convert.ToInt32(dropFromFactoryRID.SelectedValue);
                bl.Update(moveModel);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
            return;
        }
        if (-1 == int.Parse(strRID))
        {
            drow = dtMaterielStocksMove.NewRow();
        }
        else
        {
            drow = dtMaterielStocksMove.Rows[int.Parse(strRID)];
        }
        drow["Serial_Number"] = dropName.SelectedValue;
        drow["Materiel_Name"] = dropName.SelectedItem.Text;
        drow["Move_Number"] = txtMoveNumber.Text.Replace(",", "");
        drow["From_Factory_Name"] = dropFromFactoryRID.SelectedItem.Text;
        drow["From_Factory_RID"] = Convert.ToInt32(dropFromFactoryRID.SelectedValue);
        drow["To_Factory_Name"] = dropToFactoryRID.SelectedItem.Text;
        drow["To_Factory_RID"] = Convert.ToInt32(dropToFactoryRID.SelectedValue);
        drow["RID"] = rid;
        // 新加
        if (-1 == int.Parse(strRID))
            dtMaterielStocksMove.Rows.Add(drow);
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "returnValue='1';window.close();", true);
    }
}
