//******************************************************************
//*  作    者：lantaosu
//*  功能說明：庫存成本明細查詢
//*  創建日期：2008-12-10
//*  修改日期：2008-12-11 12:00
//*  修改記錄：
//*            □2008-12-11
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

public partial class Finance_Finance004_3 : PageBase
{
    Finance004_3BL bl = new Finance004_3BL();

    protected void Page_Load(object sender, EventArgs e)
    {
        gvpbStockCost.PageSize = GlobalStringManager.PageSize;
        if (!IsPostBack)
        {
            //初始化頁面
            this.queryResult.Visible = false;

            DataSet CooperatePersoList = bl.GetCooperatePersoList();
            dropFactoryRID.DataSource = CooperatePersoList;
            dropFactoryRID.DataValueField = "RID";
            dropFactoryRID.DataTextField = "Factory_ShortName_CN";
            dropFactoryRID.DataBind();
            ListItem li = new ListItem("全部", "");
            dropFactoryRID.Items.Insert(0, li);


            DataSet Use = bl.GetUseList();
            dropUse.DataSource = Use;
            dropUse.DataValueField = "Param_Code";
            dropUse.DataTextField = "Param_Name";
            dropUse.DataBind();

            DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
            dropGroup.DataSource = Group;
            dropGroup.DataValueField = "RID";
            dropGroup.DataTextField = "Group_Name";
            dropGroup.DataBind();

            btnReport.Visible = false;
        }
    }

    /// <summary>
    /// 查詢該用途的所有卡穜群組，放於【群組】下拉框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropUse_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataSet Group = bl.GetGroupList(dropUse.SelectedValue);
        dropGroup.DataSource = Group;
        dropGroup.DataValueField = "RID";
        dropGroup.DataTextField = "Group_Name";
        dropGroup.DataBind();
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string StartDate = null;
        string EndDate = null;

        //檢查頁面
        if (txtDate.Text.Trim() == "")
        {
            ShowMessage("日期不能為空");
            return;
        }
        if (dropGroup.SelectedValue.Trim() == "")
        {
            ShowMessage("群組不能為空");
            return;
        }

        if (Convert.ToDateTime(txtDate.Text.Trim()) < Convert.ToDateTime("2009/04/01"))
        {
            ShowMessage("只能執行2009/04之後的查詢");
            return;
        }

        //取得最近一次執行5.4.2的帳務迄日
        StartDate = bl.GetLastDate(txtDate.Text);
        EndDate = txtDate.Text;
        if (Convert.ToDateTime(EndDate) < Convert.ToDateTime(StartDate))
        {
            ShowMessage("輸入日期不能小於月庫存成本明細查詢的終止日期" + StartDate);
            return;
        }
        ViewState["DateFrom"] = StartDate;
        ViewState["DateTo"] = EndDate;
        //獲取上個帳務起迄日
        string beginDateFrom;
        string beginDateTo;
        beginDateTo = (DateTime.Parse(StartDate).AddDays(-1)).ToString("yyyy/MM/dd");
        beginDateFrom = bl.GetLastDate(beginDateTo);
        ViewState["beginDateFrom"] = beginDateFrom;
        ViewState["beginDateTo"] = beginDateTo;

        gvpbStockCost.BindData();

        if (gvpbStockCost.DataSource != null)
        {
            this.queryResult.Visible = true;
            btnReport.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "doSearch();", true);
        }
        else
        {
            ShowMessage("查無資料");
            this.queryResult.Visible = false;
            btnReport.Visible = false;
        }

    }

    public DataTable CreateTable()
    {
        DataTable dtMSC = new DataTable();
        dtMSC.Columns.Add("版面簡稱", Type.GetType("System.String"));
        dtMSC.Columns.Add("PERSO 廠", Type.GetType("System.String"));
        dtMSC.Columns.Add("期初庫存數", Type.GetType("System.String"));
        dtMSC.Columns.Add("期初未稅單價", Type.GetType("System.String"));
        dtMSC.Columns.Add("進貨數", Type.GetType("System.String"));
        dtMSC.Columns.Add("移轉入", Type.GetType("System.String"));
        dtMSC.Columns.Add("移轉出", Type.GetType("System.String"));
        dtMSC.Columns.Add("消耗卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("製成卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("耗損卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("銷毀卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("調整卡數", Type.GetType("System.String"));
        dtMSC.Columns.Add("期末庫存數", Type.GetType("System.String"));
        dtMSC.Columns.Add("未稅單價", Type.GetType("System.String"));
        dtMSC.Columns.Add("期初庫存金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("進貨金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("移轉入金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("移轉出金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("消耗卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("製成卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("耗損卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("銷毀卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("調整卡金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("單價調整金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("期末庫存未稅金額", Type.GetType("System.String"));
        dtMSC.Columns.Add("Factory_RID", Type.GetType("System.Int32"));//added by Even.Cheng on 20090115
        return dtMSC;
    }


    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbStockCost_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        int intRowCount = 0;
        string RID = "";
        int number = 0;
        int i = 0;
        int j = 0;
        string DateFrom = ViewState["DateFrom"].ToString();
        string DateTo = ViewState["DateTo"].ToString();


        //RID = dropFactoryRID.SelectedValue;
        //if (RID == "")
        //{
        //    RID = "";
        DataSet dsF = bl.GetCooperatePersoList();
        foreach (DataRow dr in dsF.Tables[0].Rows)
        {
            RID += dr["RID"].ToString() + ",";
        }
        RID = RID.Substring(0, (RID.Length - 1));
        //}

        DataSet dstlStockCost = null;

        try
        {
            string strDateTo = bl.GetNextWorkDay(DateTo);//取這個日期區間之後的第一個工作日
            string strDateFrom = bl.GetNextWorkDay(DateFrom);//取這個日期區間之後的第一個工作日
            dstlStockCost = bl.List(RID, strDateFrom, strDateTo, dropGroup.SelectedValue, e.FirstRow, e.LastRow, e.SortExpression, e.SortDirection, out intRowCount);
            DataTable dtResult = GenResultTable(dstlStockCost);
            //    DataRow[] drowStockCost = dstlStockCost.Tables[0].Select("", "factory_rid,cardtype_rid,income_date");
            //    //取得 各個（Perso厰+卡種）在選定日期區間之内的 各種狀況的卡種的數量
            //    DataSet CardNumber = bl.GetCardNumber(RID, DateFrom, DateTo, dropGroup.SelectedValue);
            //    DataTable dtbl = new DataTable();
            //    DataRow dr3 = null;
            //    dtbl.Columns.Add("Perso_Factory_RID", Type.GetType("System.String"));
            //    dtbl.Columns.Add("CardType_RID", Type.GetType("System.String"));
            //    dtbl.Columns.Add("消耗卡數", Type.GetType("System.String"));
            //    dtbl.Columns.Add("製成卡數", Type.GetType("System.String"));
            //    dtbl.Columns.Add("耗損卡數", Type.GetType("System.String"));
            //    dtbl.Columns.Add("銷毀卡數", Type.GetType("System.String"));
            //    dtbl.Columns.Add("調整卡數", Type.GetType("System.String"));
            //    dtbl.Columns.Add("移轉出", Type.GetType("System.String"));
            //    foreach (DataRow drow in CardNumber.Tables[0].Rows)
            //    {
            //        if (dtbl.Select("Perso_Factory_RID=" + drow["Perso_Factory_RID"].ToString() + " and CardType_RID=" + drow["CardType_RID"].ToString()).Length > 0)
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            dr3 = dtbl.NewRow();
            //            dr3["Perso_Factory_RID"] = drow["Perso_Factory_RID"].ToString().Trim();
            //            dr3["CardType_RID"] = drow["CardType_RID"].ToString().Trim();
            //            dtbl.Rows.Add(dr3);
            //        }
            //    }
            //    foreach (DataRow drMonthStockCost in drowStockCost)
            //    {
            //        if (drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "0")
            //        {
            //            DataSet ds1 = bl.GetFactoryMoveFrom(drMonthStockCost["CardType_Move_RID"].ToString().Trim());
            //            if (ds1.Tables[0].Rows.Count > 0)
            //            {
            //                string FRID = ds1.Tables[0].Rows[0]["RID"].ToString().Trim();
            //                if (dtbl.Select("Perso_Factory_RID=" + FRID + " and CardType_RID=" + drMonthStockCost["CardType_RID"].ToString().Trim()).Length > 0)
            //                {
            //                    continue;
            //                }
            //                else
            //                {
            //                    dr3 = dtbl.NewRow();
            //                    dr3["Perso_Factory_RID"] = FRID;
            //                    dr3["CardType_RID"] = drMonthStockCost["CardType_RID"].ToString().Trim();
            //                    dtbl.Rows.Add(dr3);
            //                }
            //            }
            //        }
            //    }
            //    foreach (DataRow dr1 in dtbl.Rows)
            //    {/* deleted on 20090114
            //        if (dropGroup.SelectedItem.Text != "晶片信用卡")
            //        {
            //            DataSet dset = bl.GetExpression(GlobalString.Expression.Used_RID.ToString());//消耗卡數："2"
            //            number = 0;
            //            for (i = 0; i < dset.Tables[0].Rows.Count; i++)
            //            {
            //                if (CardNumber.Tables[0].Select("Status_RID=" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + " and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString()).Length > 0)
            //                    j = Convert.ToInt32(CardNumber.Tables[0].Select("Status_RID=" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + " and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString())[0]["Sum"].ToString());
            //                else
            //                    j = 0;
            //                if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
            //                    number += j;
            //                if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
            //                    number -= j;
            //            }
            //            dr1["消耗卡數"] = number.ToString();
            //            dr1["製成卡數"] = "0";
            //            dr1["耗損卡數"] = "0";
            //        }
            //        else
            //        {*/
            //            DataSet dset = bl.GetExpression(GlobalString.Expression.Made_RID.ToString());//製成卡數："1"
            //            number = 0;
            //            for (i = 0; i < dset.Tables[0].Rows.Count; i++)
            //            {
            //                if (CardNumber.Tables[0].Select("Status_RID=" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + " and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString()).Length > 0)
            //                    j = Convert.ToInt32(CardNumber.Tables[0].Select("Status_RID=" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + " and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString())[0]["Sum"].ToString());
            //                else
            //                    j = 0;
            //                if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
            //                    number += j;
            //                if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
            //                    number -= j;
            //            }
            //            dr1["製成卡數"] = number.ToString();
            //            DataSet dset1 = bl.GetExpression(GlobalString.Expression.Waste_RID.ToString());//耗損卡數："3"
            //            number = 0;
            //            for (i = 0; i < dset1.Tables[0].Rows.Count; i++)
            //            {
            //                if (CardNumber.Tables[0].Select("Status_RID=" + dset1.Tables[0].Rows[i]["Type_RID"].ToString() + " and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString()).Length > 0)
            //                    j = Convert.ToInt32(CardNumber.Tables[0].Select("Status_RID=" + dset1.Tables[0].Rows[i]["Type_RID"].ToString() + " and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString())[0]["Sum"].ToString());
            //                else
            //                    j = 0;
            //                if (dset1.Tables[0].Rows[i]["Operate"].ToString() == "+")
            //                    number += j;
            //                if (dset1.Tables[0].Rows[i]["Operate"].ToString() == "-")
            //                    number -= j;
            //            }
            //            dr1["耗損卡數"] = number.ToString();
            //            dr1["消耗卡數"] = Convert.ToInt32(dr1["製成卡數"]) + Convert.ToInt32(dr1["耗損卡數"]);
            //        //}

            //        if (CardNumber.Tables[0].Select("Status_Code='13' and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString()).Length > 0)
            //            dr1["調整卡數"] = Convert.ToInt32(CardNumber.Tables[0].Select("Status_Code='13' and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString())[0]["Sum"].ToString());
            //        else
            //            dr1["調整卡數"] = "0";

            //        if (CardNumber.Tables[0].Select("Status_Code='12' and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString()).Length > 0)
            //            dr1["銷毀卡數"] = Convert.ToInt32(CardNumber.Tables[0].Select("Status_Code='12' and Perso_Factory_RID=" + dr1["Perso_Factory_RID"].ToString() + " and CardType_RID=" + dr1["CardType_RID"].ToString())[0]["Sum"].ToString());
            //        else
            //            dr1["銷毀卡數"] = "0";

            //        //移轉出
            //        dr1["移轉出"] = "0";
            //        foreach (DataRow drMonthStockCost in drowStockCost)
            //        {
            //            if (drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "0")
            //            {
            //                DataSet ds1 = bl.GetFactoryMoveFrom(drMonthStockCost["CardType_Move_RID"].ToString().Trim());
            //                if (ds1.Tables[0].Rows.Count > 0)
            //                {
            //                    string FRID = ds1.Tables[0].Rows[0]["RID"].ToString().Trim();
            //                    string strCardType_RID = ds1.Tables[0].Rows[0]["CardType_RID"].ToString().Trim();
            //                    if (FRID == dr1["Perso_Factory_RID"].ToString().Trim() && strCardType_RID == dr1["CardType_RID"].ToString().Trim())
            //                    {
            //                        if (dr1["移轉出"].ToString().Trim() == "")
            //                        {
            //                            dr1["移轉出"] = "0";
            //                        }
            //                        dr1["移轉出"] = Convert.ToString(Convert.ToInt32(drMonthStockCost["Number"].ToString().Trim()) + Convert.ToInt32(dr1["移轉出"].ToString().Trim()));
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    DataTable dtMSC = this.CreateTable();
            //    DataRow drMSC = null;

            //    foreach (DataRow drStockCost in drowStockCost)
            //    {
            //        DataRow dr = null;

            //        drMSC = dtMSC.NewRow();
            //        //版面簡稱
            //        drMSC["版面簡稱"] = drStockCost["Name"].ToString();

            //        //PERSO 廠
            //        drMSC["PERSO 廠"] = drStockCost["Factory_ShortName_CN"].ToString();
            //        drMSC["Factory_RID"] = drStockCost["Factory_RID"].ToString();

            //        //進貨作業RID=Operate_RID；
            //        string strOperate_RID = drStockCost["Operate_RID"].ToString();
            //        //進貨作業類別=Operate_Type；
            //        string strOperate_Type = drStockCost["Operate_Type"].ToString();
            //        //移轉單號=CardType_Move_RID；
            //        string strCardType_Move_RID = drStockCost["CardType_Move_RID"].ToString();


            //        //期初的日結日期：
            //        string Date_Time = "";
            //        //Date_Time = bl.GetLastWorkDay(DateFrom);//該日期區間的前一個工作日
            //        Date_Time = bl.GetFirstWorkDay(DateFrom,DateTo);//該日期區間内的第一個工作日

            //        string strIncomeDate = Convert.ToDateTime(drStockCost["Income_Date"].ToString()).ToString("yyyy/MM/dd");
            //        string strActualDate = GlobalStringManager.ActualDate;
            //        string strUnit_Price = "0.0";
            //        if (drStockCost["Unit_Price"].ToString().Trim() != "")
            //        {
            //            strUnit_Price = drStockCost["Unit_Price"].ToString().Trim();
            //        }

            //        //string Operate_RID_Initial = "";
            //        //string Operate_Type_Initial = "";

            //        //期初庫存
            //        DataSet UsableNumber = bl.GetUsableNumber(strOperate_RID, strOperate_Type, strCardType_Move_RID, drStockCost["Factory_RID"].ToString(), drStockCost["CardType_RID"].ToString(), Date_Time);
            //        if (UsableNumber.Tables[0].Rows.Count > 0)
            //        {
            //            //期初庫存數  
            //            if (UsableNumber.Tables[0].Rows[0]["Usable_Number_Initial"].ToString().Trim() != "")
            //            {
            //                drMSC["期初庫存數"] = UsableNumber.Tables[0].Rows[0]["Usable_Number_Initial"].ToString().Trim();
            //            }
            //            else
            //            {
            //                drMSC["期初庫存數"] = "0";
            //            }

            //            //Operate_RID_Initial = UsableNumber.Tables[0].Rows[0]["Operate_RID_Initial"].ToString();
            //            //Operate_Type_Initial = UsableNumber.Tables[0].Rows[0]["Operate_Type_Initial"].ToString();

            //            //期初未稅單價
            //            if (strIncomeDate.CompareTo(strActualDate) < 0 || strOperate_RID == "0")
            //            {
            //                drMSC["期初未稅單價"] = strUnit_Price;
            //            }
            //            else
            //            {
            //                if (!StringUtil.IsEmpty(strOperate_Type))
            //                {
            //                    DataSet UnitPrice = bl.GetUnitPrice(strOperate_RID, strOperate_Type, Date_Time);

            //                    if (UnitPrice.Tables[0].Rows.Count > 0)
            //                    {
            //                        string strReal_Ask_Number = UnitPrice.Tables[0].Rows[0]["Real_Ask_Number"].ToString().Trim();
            //                        decimal dReal_Ask_Number = 0;
            //                        if (strReal_Ask_Number != "")
            //                        {
            //                            dReal_Ask_Number = Convert.ToDecimal(strReal_Ask_Number);
            //                        }

            //                        if (dReal_Ask_Number == 0)//未請款
            //                        {
            //                            if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
            //                            {
            //                                if (UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
            //                                {
            //                                    drMSC["期初未稅單價"] = UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim();
            //                                }
            //                                else
            //                                {
            //                                    drMSC["期初未稅單價"] = "0.0000";
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
            //                                {
            //                                    decimal unit_price = Convert.ToDecimal(UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"]) / 1.05M;
            //                                    drMSC["期初未稅單價"] = unit_price.ToString("N4");
            //                                }
            //                                else
            //                                {
            //                                    drMSC["期初未稅單價"] = "0.0000";
            //                                }
            //                            }                       
            //                        }
            //                        else//已請款
            //                        {
            //                            if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
            //                            {
            //                                if (UnitPrice.Tables[0].Rows[0]["Unit_Price"].ToString().Trim() != "")
            //                                {
            //                                    drMSC["期初未稅單價"] = UnitPrice.Tables[0].Rows[0]["Unit_Price"].ToString().Trim();
            //                                }
            //                                else
            //                                {
            //                                    drMSC["期初未稅單價"] = "0.00";
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (UnitPrice.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim() != "")
            //                                {
            //                                    drMSC["期初未稅單價"] = UnitPrice.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim();
            //                                }
            //                                else
            //                                {
            //                                    drMSC["期初未稅單價"] = "0.00";
            //                                }
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        drMSC["期初未稅單價"] = "0.00";
            //                    }
            //                }
            //                else
            //                {
            //                    drMSC["期初未稅單價"] = "0.00";
            //                }
            //            }
            //        }
            //        else
            //        {
            //            drMSC["期初庫存數"] = "0";
            //            drMSC["期初未稅單價"] = "0.00";
            //        }

            //        string strIncome_Date = Convert.ToDateTime(drStockCost["Income_Date"].ToString().Trim()).ToString("yyyy/MM/dd");
            //        if (DateFrom.CompareTo(strIncome_Date) <= 0 && strIncome_Date.CompareTo(DateTo) <= 0)
            //        {//本期進貨或移轉               
            //            if (drStockCost["CardType_Move_RID"].ToString().Trim() != "0")
            //            {
            //                drMSC["進貨數"] = "0";
            //                drMSC["移轉入"] = drStockCost["Number"].ToString();
            //            }
            //            else
            //            {
            //                drMSC["進貨數"] = drStockCost["Number"].ToString();
            //                drMSC["移轉入"] = "0";
            //            }
            //        }
            //        else
            //        {
            //            drMSC["進貨數"] = "0";
            //            drMSC["移轉入"] = "0";
            //        }

            //        //製成卡數,耗損卡數,消耗卡數,銷毀卡數,調整卡數以及移轉出卡數
            //        if (dtbl.Select("Perso_Factory_RID=" + drStockCost["Factory_RID"].ToString() + " and CardType_RID=" + drStockCost["CardType_RID"].ToString()).Length > 0)
            //        {
            //            dr = dtbl.Select("Perso_Factory_RID=" + drStockCost["Factory_RID"].ToString() + " and CardType_RID=" + drStockCost["CardType_RID"].ToString())[0];
            //            number = Convert.ToInt32(drMSC["期初庫存數"].ToString()) + Convert.ToInt32(drMSC["進貨數"].ToString()) + Convert.ToInt32(drMSC["移轉入"].ToString());
            //            if (number > 0 && dr["移轉出"].ToString() != "" && Convert.ToInt32(dr["移轉出"].ToString()) > 0)
            //            {
            //                drMSC["移轉出"] = bl.GetCardCount(number, dr, "移轉出").ToString();
            //                number -= Convert.ToInt32(drMSC["移轉出"]);
            //            }
            //            else
            //            {
            //                drMSC["移轉出"] = "0";
            //            }

            //            if (number > 0 && dr["調整卡數"].ToString() != "" && Convert.ToInt32(dr["調整卡數"].ToString()) > 0)
            //            {
            //                drMSC["調整卡數"] = bl.GetCardCount(number, dr, "調整卡數").ToString();
            //                number -= Convert.ToInt32(drMSC["調整卡數"]);
            //            }
            //            else
            //            {
            //                drMSC["調整卡數"] = "0";
            //            }

            //            if (number > 0 && dr["銷毀卡數"].ToString() != "" && Convert.ToInt32(dr["銷毀卡數"].ToString()) > 0)
            //            {
            //                drMSC["銷毀卡數"] = bl.GetCardCount(number, dr, "銷毀卡數").ToString();
            //                number -= Convert.ToInt32(drMSC["銷毀卡數"]);
            //            }
            //            else
            //            {
            //                drMSC["銷毀卡數"] = "0";
            //            }

            //            if (dropGroup.SelectedItem.Text == "晶片信用卡")
            //            {
            //                drMSC["消耗卡數"] = "0";
            //                if (number > 0 && dr["耗損卡數"].ToString() != "" && Convert.ToInt32(dr["耗損卡數"].ToString()) > 0)
            //                {
            //                    drMSC["耗損卡數"] = bl.GetCardCount(number, dr, "耗損卡數").ToString();
            //                    number -= Convert.ToInt32(drMSC["耗損卡數"]);
            //                }
            //                else
            //                {
            //                    drMSC["耗損卡數"] = "0";
            //                }

            //                if (number > 0 && dr["製成卡數"].ToString() != "" && Convert.ToInt32(dr["製成卡數"].ToString()) > 0)
            //                {
            //                    drMSC["製成卡數"] = bl.GetCardCount(number, dr, "製成卡數").ToString();
            //                    number -= Convert.ToInt32(drMSC["製成卡數"]);
            //                }
            //                else
            //                {
            //                    drMSC["製成卡數"] = "0";
            //                }
            //            }
            //            else
            //            {
            //                drMSC["製成卡數"] = "0";
            //                drMSC["耗損卡數"] = "0";
            //                if (number > 0 && dr["消耗卡數"].ToString() != "" && Convert.ToInt32(dr["消耗卡數"].ToString()) > 0)
            //                {
            //                    drMSC["消耗卡數"] = bl.GetCardCount(number, dr, "消耗卡數").ToString();
            //                    number -= Convert.ToInt32(drMSC["消耗卡數"]);
            //                }
            //                else
            //                {
            //                    drMSC["消耗卡數"] = "0";
            //                }
            //            }
            //        }
            //        else
            //        {
            //            drMSC["移轉出"] = "0";
            //            drMSC["消耗卡數"] = "0";
            //            drMSC["製成卡數"] = "0";
            //            drMSC["耗損卡數"] = "0";
            //            drMSC["銷毀卡數"] = "0";
            //            drMSC["調整卡數"] = "0";
            //        }


            //        //期末庫存數                
            //        if (dropGroup.SelectedItem.Text == "晶片信用卡")
            //            drMSC["期末庫存數"] = Convert.ToString(Convert.ToInt32(drMSC["期初庫存數"].ToString()) + Convert.ToInt32(drMSC["進貨數"].ToString()) + Convert.ToInt32(drMSC["移轉入"].ToString()) - Convert.ToInt32(drMSC["移轉出"].ToString()) - Convert.ToInt32(drMSC["製成卡數"].ToString()) - Convert.ToInt32(drMSC["耗損卡數"].ToString()) - Convert.ToInt32(drMSC["銷毀卡數"].ToString()) - Convert.ToInt32(drMSC["調整卡數"].ToString()));
            //        else
            //            drMSC["期末庫存數"] = Convert.ToString(Convert.ToInt32(drMSC["期初庫存數"].ToString()) + Convert.ToInt32(drMSC["進貨數"].ToString()) + Convert.ToInt32(drMSC["移轉入"].ToString()) - Convert.ToInt32(drMSC["移轉出"].ToString()) - Convert.ToInt32(drMSC["消耗卡數"].ToString()) - Convert.ToInt32(drMSC["銷毀卡數"].ToString()) - Convert.ToInt32(drMSC["調整卡數"].ToString()));

            //        //期末的日結日期
            //        string strLastCheckDate = bl.GetNextWorkDay(DateTo);//取這個日期區間之後的第一個工作日
            //        if (drMSC["期末庫存數"].ToString() == "0")
            //        {//取該日期區間内最後一次日結的日期
            //            strLastCheckDate = bl.GetLastCheckDate(strOperate_RID, strOperate_Type, strCardType_Move_RID, drStockCost["Factory_RID"].ToString(), drStockCost["CardType_RID"].ToString(), DateFrom, DateTo);
            //        }

            //        //未稅單價
            //        if (strIncomeDate.CompareTo(strActualDate) < 0 || strOperate_RID=="0")
            //        {
            //            drMSC["未稅單價"] = strUnit_Price;
            //        }
            //        else
            //        {
            //            if (!StringUtil.IsEmpty(drStockCost["Operate_Type"].ToString()))
            //            {
            //                DataSet UnitPrice1 = bl.GetUnitPrice(drStockCost["Operate_RID"].ToString(), drStockCost["Operate_Type"].ToString(), strLastCheckDate.Split(' ')[0]);
            //                if (UnitPrice1.Tables[0].Rows.Count > 0)
            //                {
            //                    string strReal_Ask_Number = UnitPrice1.Tables[0].Rows[0]["Real_Ask_Number"].ToString().Trim();
            //                    decimal dReal_Ask_Number = 0;
            //                    if (strReal_Ask_Number != "")
            //                    {
            //                        dReal_Ask_Number = Convert.ToDecimal(strReal_Ask_Number);
            //                    }

            //                    if (dReal_Ask_Number == 0)//未請款
            //                    {
            //                        if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
            //                        {
            //                            if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
            //                            {
            //                                drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim();
            //                            }
            //                            else
            //                            {
            //                                drMSC["未稅單價"] = "0.0000";
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
            //                            {
            //                                decimal unit_price = Convert.ToDecimal(UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"]) / 1.05M;
            //                                drMSC["未稅單價"] = unit_price.ToString("N4");
            //                            }
            //                            else
            //                            {
            //                                drMSC["未稅單價"] = "0.0000";
            //                            }
            //                        }                          
            //                    }
            //                    else//已請款
            //                    {
            //                        if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
            //                        {
            //                            if (UnitPrice1.Tables[0].Rows[0]["Unit_Price"].ToString().Trim() != "")
            //                            {
            //                                drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price"].ToString().Trim();
            //                            }
            //                            else
            //                            {
            //                                drMSC["未稅單價"] = "0.00";
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim() != "")
            //                            {
            //                                drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim();
            //                            }
            //                            else
            //                            {
            //                                drMSC["未稅單價"] = "0.00";
            //                            }
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    drMSC["未稅單價"] = "0.00";
            //                }
            //            }
            //            else
            //            {
            //                drMSC["未稅單價"] = "0.00";
            //            }
            //        }

            //        //期初庫存金額(未(含)稅)                
            //        drMSC["期初庫存金額"] = Convert.ToString(Convert.ToInt32(drMSC["期初庫存數"].ToString()) * Convert.ToDecimal(drMSC["期初未稅單價"].ToString()));

            //        //進貨金額                
            //        drMSC["進貨金額"] = Convert.ToString(Convert.ToInt32(drMSC["進貨數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));

            //        //移轉入金額                
            //        drMSC["移轉入金額"] = Convert.ToString(Convert.ToInt32(drMSC["移轉入"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));

            //        //移轉出金額                
            //        drMSC["移轉出金額"] = Convert.ToString(Convert.ToInt32(drMSC["移轉出"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));

            //        //製成卡金額,耗損卡金額,消耗卡金額                
            //        if (dropGroup.SelectedItem.Text != "晶片信用卡")
            //        {
            //            drMSC["消耗卡金額"] = Convert.ToString(Convert.ToInt32(drMSC["消耗卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));
            //            drMSC["製成卡金額"] = "0.00";
            //            drMSC["耗損卡金額"] = "0.00";
            //        }
            //        else
            //        {
            //            drMSC["製成卡金額"] = Convert.ToString(Convert.ToInt32(drMSC["製成卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));
            //            drMSC["耗損卡金額"] = Convert.ToString(Convert.ToInt32(drMSC["耗損卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));
            //            drMSC["消耗卡金額"] = "0.00";
            //        }

            //        //銷毀卡金額,調整卡金額                
            //        drMSC["銷毀卡金額"] = Convert.ToString(Convert.ToInt32(drMSC["銷毀卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));
            //        drMSC["調整卡金額"] = Convert.ToString(Convert.ToInt32(drMSC["調整卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));

            //        //期末庫存未(含)稅金額                
            //        drMSC["期末庫存未稅金額"] = Convert.ToString(Convert.ToInt32(drMSC["期末庫存數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()));

            //        //單價調整金額                
            //        if (Convert.ToInt32(drMSC["期初庫存數"]) != 0 && (Convert.ToDecimal(drMSC["期初未稅單價"].ToString()) != Convert.ToDecimal(drMSC["未稅單價"].ToString())))
            //        {
            //            drMSC["單價調整金額"] = Convert.ToString(Convert.ToDecimal(drMSC["期初庫存金額"].ToString()) - Convert.ToDecimal(drMSC["期末庫存未稅金額"].ToString()));

            //        }
            //        else
            //            drMSC["單價調整金額"] = "0.00";


            //        dtMSC.Rows.Add(drMSC);
            //    }

            //    //added by Even.Cheng on 20090115
            //    DataTable dtResult = dtMSC;
            //    if (dropFactoryRID.SelectedValue != "")
            //    {
            //        dtResult = this.CreateTable();
            //        DataRow[] drs = dtMSC.Select("Factory_RID=" + dropFactoryRID.SelectedValue);
            //        foreach (DataRow dr in drs)
            //        {
            //            DataRow drResult = dtResult.NewRow();
            //            for (int m = 0; m < drResult.ItemArray.Length; m++)
            //            {
            //                drResult[m] = dr[m];
            //            }
            //            dtResult.Rows.Add(drResult);
            //        }
            //    }
            //    //end add

            //    //資料表中的合計行
            //    if (dtResult.Rows.Count > 0)
            //    {
            //        drMSC = dtResult.NewRow();
            //        drMSC["版面簡稱"] = "合計";
            //        int total = 0;
            //        decimal total1 = 0;
            //        for (j = 0; j < dtResult.Rows.Count; j++)
            //        {
            //            if(dtResult.Rows[j][2].ToString().Trim() != "")
            //                total += Convert.ToInt32(dtResult.Rows[j][2].ToString().Trim());
            //        }
            //        drMSC[2] = total.ToString();
            //        for (j = 0; j < dtResult.Rows.Count; j++)
            //        {
            //            if(dtResult.Rows[j][3].ToString().Trim() != "")
            //                total1 += Convert.ToDecimal(dtResult.Rows[j][3].ToString().Trim());
            //        }
            //        drMSC[3] = total1.ToString();
            //        for (i = 4; i < 13; i++)
            //        {
            //            total = 0;
            //            for (j = 0; j < dtResult.Rows.Count; j++)
            //            {
            //                if(dtResult.Rows[j][i].ToString().Trim() !="")
            //                    total += Convert.ToInt32(dtResult.Rows[j][i].ToString().Trim());
            //            }
            //            drMSC[i] = total.ToString();
            //        }
            //        for (i = 13; i < dtResult.Columns.Count-1; i++)
            //        {
            //            total1 = 0;
            //            for (j = 0; j < dtResult.Rows.Count; j++)
            //            {
            //                if (dtResult.Rows[j][i].ToString().Trim() != "")
            //                    total1 += Convert.ToDecimal(dtResult.Rows[j][i].ToString().Trim());
            //            }
            //            drMSC[i] = total1.ToString();
            //        }
            //        drMSC[3] = "";
            //        drMSC[13] = "";
            //        dtResult.Rows.Add(drMSC);
            //    }

            //    //設置表格標題
            //    if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
            //    {
            //        dtResult.Columns[3].ColumnName = "期初含稅單價";
            //        dtResult.Columns[13].ColumnName = "含稅單價";
            //        dtResult.Columns[14].ColumnName = "期初庫存金額(含稅)";
            //        dtResult.Columns[24].ColumnName = "期末庫存含稅金額";
            //    }
            //    else
            //    {
            //        dtResult.Columns[3].ColumnName = "期初未稅單價";
            //        dtResult.Columns[13].ColumnName = "未稅單價";
            //        dtResult.Columns[14].ColumnName = "期初庫存金額(未稅)";
            //        dtResult.Columns[24].ColumnName = "期末庫存未稅金額";
            //    }

            //    //顯示欄位
            //    dtResult.Columns.Remove("Factory_RID");
            //    if (dropGroup.SelectedItem.Text != "晶片信用卡")
            //    {
            //        dtResult.Columns.Remove("製成卡數");
            //        dtResult.Columns.Remove("製成卡金額");
            //        dtResult.Columns.Remove("耗損卡數");
            //        dtResult.Columns.Remove("耗損卡金額");                
            //    }
            //    else
            //    {
            //        dtResult.Columns.Remove("消耗卡數");
            //        dtResult.Columns.Remove("消耗卡金額");                
            //    }

            //    //千分位
            //    foreach (DataRow dr in dtResult.Rows)
            //    {
            //        if (dr[0].ToString() == "合計")
            //        {
            //            if (dropGroup.SelectedItem.Text != "晶片信用卡")
            //            {
            //                dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
            //                for (i = 4; i < 11; i++)
            //                    dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
            //                for (i = 12; i < dtResult.Columns.Count; i++)
            //                    dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
            //            }
            //            else
            //            {
            //                dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
            //                for (i = 4; i < 12; i++)
            //                    dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
            //                for (i = 13; i < dtResult.Columns.Count; i++)
            //                    dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
            //            }
            //        }
            //        else
            //        {
            //            if (dropGroup.SelectedItem.Text != "晶片信用卡")
            //            {
            //                dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
            //                dr[3] = Convert.ToDecimal(dr[3].ToString()).ToString("N4");
            //                dr[11] = Convert.ToDecimal(dr[11].ToString()).ToString("N4");
            //                for (i = 4; i < 11; i++)
            //                    dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
            //                for (i = 12; i < dtResult.Columns.Count; i++)
            //                    dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
            //            }
            //            else
            //            {
            //                dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
            //                dr[3] = Convert.ToDecimal(dr[3].ToString()).ToString("N4");
            //                dr[12] = Convert.ToDecimal(dr[12].ToString()).ToString("N4");
            //                for (i = 4; i < 12; i++)
            //                    dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
            //                for (i = 13; i < dtResult.Columns.Count; i++)
            //                    dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
            //            }
            //        }
            //    }

            //    //傳參數，添加報表資料
            ViewState["StockCost"] = dtResult;


            if (dtResult != null && dtResult.Rows.Count > 0)//如果查到了資料
            {
                e.Table = dtResult;//要綁定的資料表
                e.RowCount = dtResult.Rows.Count;
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
        }
    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbStockCost_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //將負數項轉換為括號加（）
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 2; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Text.Contains("-"))
                {
                    e.Row.Cells[i].Text = "(" + e.Row.Cells[i].Text.Substring(1, e.Row.Cells[i].Text.Length - 1) + ")";
                    e.Row.Cells[i].ForeColor = System.Drawing.Color.Red;
                }
            }
            if (e.Row.Cells[0].Text == "合計")
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                    e.Row.Cells[i].Font.Bold = true;
            }
        }
    }
    #endregion

    /// <summary>
    /// 匯出報表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        this.queryResult.Visible = true;

        int i = 0;
        DataTable dt = (DataTable)ViewState["StockCost"];
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        HidTime.Value = time;
        //千分位
        foreach (DataRow dr in dt.Rows)
        {
            if (dropGroup.SelectedItem.Text != "晶片信用卡")
            {
                for (i = 2; i < 11; i++)
                    dr[i] = dr[i].ToString().Replace(",", "");
                for (i = 11; i < dt.Columns.Count; i++)
                    dr[i] = dr[i].ToString().Replace(",", "");
            }
            else
            {
                for (i = 2; i < 12; i++)
                    dr[i] = dr[i].ToString().Replace(",", "");
                for (i = 12; i < dt.Columns.Count; i++)
                    dr[i] = dr[i].ToString().Replace(",", "");
            }
        }
        //將表格中的資料存入RPT_Finance004_3中
        bl.AddReport(dt, time);

        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "exportExcel();", true);
    }
    /// <summary>
    /// 產生數據方法！
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private DataTable GenResultTable(DataSet dstlMonthStockCost)
    {


        string RID = "";
        int number = 0;
        int i = 0;
        int j = 0;
        string DateFrom = ViewState["DateFrom"].ToString();
        //hidDateFrom.Value = ViewState["DateFrom"].ToString();
        string DateTo = ViewState["DateTo"].ToString();
        //hidDateTo.Value = ViewState["DateTo"].ToString();
        string beginDateFrom = ViewState["beginDateFrom"].ToString();
        string beginDateTo = ViewState["beginDateTo"].ToString();

        for (int ii = 0; ii < dstlMonthStockCost.Tables[0].Rows.Count; ii++)
        {
            DataRow tempRows1 = dstlMonthStockCost.Tables[0].Rows[ii];
            DateTime drIncome_Date1 = DateTime.Parse(tempRows1["Income_Date"].ToString().Trim());
            //20090924IR 合并相同帳務周期的轉入數量 Add by YangKun 2009/09/24 start
            Finance004_2BL BL42 = new Finance004_2BL();
            string BillCycle1 = BL42.GetBillCycle(drIncome_Date1.ToString("yyyy/MM/dd"));
            //20090924IR 合并相同帳務周期的轉入數量 Add by YangKun 2009/09/24 end
            if (tempRows1["CardType_Move_Rid"].ToString().Trim() != "0" && tempRows1["CardType_Move_Rid"].ToString().Trim() != "")//&& drIncome_Date1 <= DateTime.Parse(DateTo) && drIncome_Date1 >= DateTime.Parse(DateFrom)
            {
                DataSet tempds1 = bl.GetFactoryMoveFrom(tempRows1["CardType_Move_Rid"].ToString().Trim());
                string tempFRID1 = tempds1.Tables[0].Rows[0]["RID"].ToString().Trim();
                for (int tt = ii + 1; tt < dstlMonthStockCost.Tables[0].Rows.Count; tt++)
                {
                    DataRow tempRows2 = dstlMonthStockCost.Tables[0].Rows[tt];
                    DateTime drIncome_Date2 = DateTime.Parse(tempRows2["Income_Date"].ToString().Trim());
                    //20090924IR 合并相同帳務周期的轉入數量 Add by YangKun 2009/09/24 start
                    string BillCycle2 = BL42.GetBillCycle(drIncome_Date2.ToString("yyyy/MM/dd"));
                    if (tempRows2["CardType_Move_Rid"].ToString().Trim() != "0" && tempRows2["CardType_Move_Rid"].ToString().Trim() != "" && BillCycle1 == BillCycle2)//&& drIncome_Date2 <= DateTime.Parse(DateTo) && drIncome_Date2 >= DateTime.Parse(DateFrom)
                    //20090924IR 合并相同帳務周期的轉入數量 Add by YangKun 2009/09/24 end
                    {
                        DataSet tempds2 = bl.GetFactoryMoveFrom(tempRows1["CardType_Move_Rid"].ToString().Trim());
                        string tempFRID2 = tempds2.Tables[0].Rows[0]["RID"].ToString().Trim();
                        if (tempFRID1 == tempFRID2 && tempRows1["Uselog_Rid"].ToString().Trim() != tempRows2["Uselog_Rid"].ToString().Trim() && tempRows1["Factory_RID"].ToString().Trim() == tempRows2["Factory_RID"].ToString().Trim() && tempRows1["CardType_RID"].ToString().Trim() == tempRows2["CardType_RID"].ToString().Trim() && tempRows1["Unit_Price"].ToString().Trim() == tempRows2["Unit_Price"].ToString().Trim())
                        {
                            dstlMonthStockCost.Tables[0].Rows[ii]["Number"] = Convert.ToString(int.Parse(dstlMonthStockCost.Tables[0].Rows[ii]["Number"].ToString()) + int.Parse(dstlMonthStockCost.Tables[0].Rows[tt]["Number"].ToString()));
                            if (dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"].ToString().Trim() == "")
                            {
                                dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"] = dstlMonthStockCost.Tables[0].Rows[ii]["Uselog_Rid"].ToString() + "," + dstlMonthStockCost.Tables[0].Rows[tt]["Uselog_Rid"].ToString();
                            }
                            else
                            {
                                dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"] = dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"].ToString() + "," + dstlMonthStockCost.Tables[0].Rows[tt]["Uselog_Rid"].ToString();
                            }
                            dstlMonthStockCost.Tables[0].Rows.RemoveAt(tt);
                            tt--;
                        }

                    }
                }
            }
        }
        //for (int ii = 0; ii < dstlMonthStockCost.Tables[0].Rows.Count; ii++)
        //{
        //    DataRow tempRows1 = dstlMonthStockCost.Tables[0].Rows[ii];
        //    DateTime drIncome_Date1 = DateTime.Parse(tempRows1["Income_Date"].ToString().Trim());
        //    if (tempRows1["CardType_Move_Rid"].ToString().Trim() != "0" && tempRows1["CardType_Move_Rid"].ToString().Trim() != "" && drIncome_Date1 <= DateTime.Parse(beginDateTo) && drIncome_Date1 >= DateTime.Parse(beginDateFrom))
        //    {
        //        DataSet tempds1 = bl.GetFactoryMoveFrom(tempRows1["CardType_Move_Rid"].ToString().Trim());
        //        string tempFRID1 = tempds1.Tables[0].Rows[0]["RID"].ToString().Trim();
        //        for (int tt = ii + 1; tt < dstlMonthStockCost.Tables[0].Rows.Count; tt++)
        //        {
        //            DataRow tempRows2 = dstlMonthStockCost.Tables[0].Rows[tt];
        //            DateTime drIncome_Date2 = DateTime.Parse(tempRows2["Income_Date"].ToString().Trim());
        //            if (tempRows2["CardType_Move_Rid"].ToString().Trim() != "0" && tempRows2["CardType_Move_Rid"].ToString().Trim() != "" && drIncome_Date2 <= DateTime.Parse(beginDateTo) && drIncome_Date2 >= DateTime.Parse(beginDateFrom))
        //            {
        //                DataSet tempds2 = bl.GetFactoryMoveFrom(tempRows1["CardType_Move_Rid"].ToString().Trim());
        //                string tempFRID2 = tempds2.Tables[0].Rows[0]["RID"].ToString().Trim();
        //                if (tempFRID1 == tempFRID2 && tempRows1["Uselog_Rid"].ToString().Trim() != tempRows2["Uselog_Rid"].ToString().Trim() && tempRows1["Factory_RID"].ToString().Trim() == tempRows2["Factory_RID"].ToString().Trim() && tempRows1["CardType_RID"].ToString().Trim() == tempRows2["CardType_RID"].ToString().Trim() && tempRows1["Unit_Price"].ToString().Trim() == tempRows2["Unit_Price"].ToString().Trim())
        //                {
        //                    dstlMonthStockCost.Tables[0].Rows[ii]["Number"] = Convert.ToString(int.Parse(dstlMonthStockCost.Tables[0].Rows[ii]["Number"].ToString()) + int.Parse(dstlMonthStockCost.Tables[0].Rows[tt]["Number"].ToString()));
        //                    if (dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"].ToString().Trim() == "")
        //                    {
        //                        dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"] = dstlMonthStockCost.Tables[0].Rows[ii]["Uselog_Rid"].ToString() + "," + dstlMonthStockCost.Tables[0].Rows[tt]["Uselog_Rid"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"] = dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"].ToString() + "," + dstlMonthStockCost.Tables[0].Rows[tt]["Uselog_Rid"].ToString();
        //                    }
        //                    dstlMonthStockCost.Tables[0].Rows.RemoveAt(tt);
        //                    tt--;
        //                }

        //            }
        //        }
        //    }
        //}
        //for (int ii = 0; ii < dstlMonthStockCost.Tables[0].Rows.Count; ii++)
        //{
        //    DataRow tempRows1 = dstlMonthStockCost.Tables[0].Rows[ii];
        //    DateTime drIncome_Date1 = DateTime.Parse(tempRows1["Income_Date"].ToString().Trim());
        //    if (tempRows1["CardType_Move_Rid"].ToString().Trim() != "0" && tempRows1["CardType_Move_Rid"].ToString().Trim() != "" && drIncome_Date1 < DateTime.Parse(beginDateFrom))
        //    {
        //        DataSet tempds1 = bl.GetFactoryMoveFrom(tempRows1["CardType_Move_Rid"].ToString().Trim());
        //        string tempFRID1 = tempds1.Tables[0].Rows[0]["RID"].ToString().Trim();
        //        for (int tt = ii + 1; tt < dstlMonthStockCost.Tables[0].Rows.Count; tt++)
        //        {
        //            DataRow tempRows2 = dstlMonthStockCost.Tables[0].Rows[tt];
        //            DateTime drIncome_Date2 = DateTime.Parse(tempRows2["Income_Date"].ToString().Trim());
        //            if (tempRows2["CardType_Move_Rid"].ToString().Trim() != "0" && tempRows2["CardType_Move_Rid"].ToString().Trim() != "" && drIncome_Date2 < DateTime.Parse(beginDateFrom))
        //            {
        //                DataSet tempds2 = bl.GetFactoryMoveFrom(tempRows1["CardType_Move_Rid"].ToString().Trim());
        //                string tempFRID2 = tempds2.Tables[0].Rows[0]["RID"].ToString().Trim();
        //                if (tempFRID1 == tempFRID2 && tempRows1["Uselog_Rid"].ToString().Trim() != tempRows2["Uselog_Rid"].ToString().Trim() && tempRows1["Factory_RID"].ToString().Trim() == tempRows2["Factory_RID"].ToString().Trim() && tempRows1["CardType_RID"].ToString().Trim() == tempRows2["CardType_RID"].ToString().Trim() && tempRows1["Unit_Price"].ToString().Trim() == tempRows2["Unit_Price"].ToString().Trim())
        //                {
        //                    dstlMonthStockCost.Tables[0].Rows[ii]["Number"] = Convert.ToString(int.Parse(dstlMonthStockCost.Tables[0].Rows[ii]["Number"].ToString()) + int.Parse(dstlMonthStockCost.Tables[0].Rows[tt]["Number"].ToString()));
        //                    if (dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"].ToString().Trim() == "")
        //                    {
        //                        dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"] = dstlMonthStockCost.Tables[0].Rows[ii]["Uselog_Rid"].ToString() + "," + dstlMonthStockCost.Tables[0].Rows[tt]["Uselog_Rid"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"] = dstlMonthStockCost.Tables[0].Rows[ii]["log_rid"].ToString() + "," + dstlMonthStockCost.Tables[0].Rows[tt]["Uselog_Rid"].ToString();
        //                    }
        //                    dstlMonthStockCost.Tables[0].Rows.RemoveAt(tt);
        //                    tt--;
        //                }

        //            }
        //        }
        //    }
        //}

        DataRow[] drowStockCost = dstlMonthStockCost.Tables[0].Select("", "factory_rid,cardtype_rid,income_date");
        DataSet dsF = bl.GetCooperatePersoList();
        foreach (DataRow dr in dsF.Tables[0].Rows)
        {
            RID += dr["RID"].ToString() + ",";
        }
        RID = RID.Substring(0, (RID.Length - 1));
        //}   


        try
        {
            string strDateTo = bl.GetNextWorkDay(DateTo);//取這個日期區間之后的第一個工作日

            //取得 各個（Perso厰+卡種）在選定日期區間之内的 各種狀況的卡種的數量
            DataSet CardNumber = bl.GetCardNumber(RID, DateFrom, DateTo, dropGroup.SelectedValue);
            DataTable dtbl = new DataTable();
            DataRow dr3 = null;
            dtbl.Columns.Add("Perso_Factory_RID", Type.GetType("System.String"));
            dtbl.Columns.Add("CardType_RID", Type.GetType("System.String"));
            dtbl.Columns.Add("消耗卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("製成卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("耗損卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("銷毀卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("調整卡數", Type.GetType("System.String"));
            dtbl.Columns.Add("移轉出", Type.GetType("System.String"));
            foreach (DataRow drow in CardNumber.Tables[0].Rows)
            {
                if (dtbl.Select("Perso_Factory_RID='" + drow["Perso_Factory_RID"].ToString().Trim() + "' and CardType_RID='" + drow["CardType_RID"].ToString().Trim() + "'").Length > 0)
                {
                    continue;
                }
                else
                {
                    dr3 = dtbl.NewRow();
                    dr3["Perso_Factory_RID"] = drow["Perso_Factory_RID"].ToString().Trim();
                    dr3["CardType_RID"] = drow["CardType_RID"].ToString().Trim();
                    dtbl.Rows.Add(dr3);
                }
            }
            foreach (DataRow drMonthStockCost in drowStockCost)
            {
                if (drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "0")
                {
                    DataSet ds1 = bl.GetFactoryMoveFrom(drMonthStockCost["CardType_Move_RID"].ToString().Trim());
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        string FRID = ds1.Tables[0].Rows[0]["RID"].ToString().Trim();
                        if (dtbl.Select("Perso_Factory_RID='" + FRID + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString().Trim() + "'").Length > 0)
                        {
                            continue;
                        }
                        else
                        {
                            dr3 = dtbl.NewRow();
                            dr3["Perso_Factory_RID"] = FRID;
                            dr3["CardType_RID"] = drMonthStockCost["CardType_RID"].ToString().Trim();
                            dtbl.Rows.Add(dr3);
                        }
                    }
                }
            }
            //200907IR 按批次算出移轉出的數量
            DataTable dtmo = new DataTable();
            DataRow drm3 = null;
            dtmo.Columns.Add("Perso_Factory_RID", Type.GetType("System.String"));
            dtmo.Columns.Add("CardType_RID", Type.GetType("System.String"));
            dtmo.Columns.Add("MoveNum", Type.GetType("System.String"));
            dtmo.Columns.Add("MoveUnit_Price", Type.GetType("System.String"));
            dtmo.Columns.Add("MoveDate", Type.GetType("System.DateTime"));

            foreach (DataRow drMonthStockCost in drowStockCost)
            {
                DateTime drIncome_Date = DateTime.Parse(drMonthStockCost["Income_Date"].ToString().Trim());
                if (drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "0" && drIncome_Date <= DateTime.Parse(DateTo) && drIncome_Date >= DateTime.Parse(DateFrom))
                {
                    DataSet ds1 = bl.GetFactoryMoveFrom(drMonthStockCost["CardType_Move_RID"].ToString().Trim());
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        string FRID = ds1.Tables[0].Rows[0]["RID"].ToString().Trim();
                        if (dtmo.Select("Perso_Factory_RID='" + FRID + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString().Trim() + "' and MoveUnit_Price='" + drMonthStockCost["Unit_Price"].ToString().Trim() + "'").Length > 0)
                        {
                            continue;
                        }
                        else
                        {
                            drm3 = dtmo.NewRow();
                            drm3["Perso_Factory_RID"] = FRID;
                            drm3["CardType_RID"] = drMonthStockCost["CardType_RID"].ToString().Trim();
                            drm3["MoveUnit_Price"] = drMonthStockCost["Unit_Price"].ToString().Trim();
                            drm3["MoveDate"] = drIncome_Date;
                            dtmo.Rows.Add(drm3);
                        }
                    }
                }
            }
            foreach (DataRow dr1 in dtmo.Rows)
            {
                //移轉出
                dr1["MoveNum"] = "0";
                foreach (DataRow drMonthStockCost in drowStockCost)
                {
                    DateTime drIncome_Date3 = DateTime.Parse(drMonthStockCost["Income_Date"].ToString().Trim());
                    if (drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "0" && drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "" && drIncome_Date3 <= DateTime.Parse(DateTo) && drIncome_Date3 >= DateTime.Parse(DateFrom))
                    {
                        DataSet ds1 = bl.GetFactoryMoveFrom(drMonthStockCost["CardType_Move_RID"].ToString().Trim());
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            string FRID = ds1.Tables[0].Rows[0]["RID"].ToString().Trim();
                            string strCardType_RID = ds1.Tables[0].Rows[0]["CardType_RID"].ToString().Trim();
                            if (FRID == dr1["Perso_Factory_RID"].ToString().Trim() && strCardType_RID == dr1["CardType_RID"].ToString().Trim() && drMonthStockCost["Unit_Price"].ToString().Trim() == dr1["MoveUnit_Price"].ToString().Trim())
                            {
                                if (dr1["MoveNum"].ToString().Trim() == "")
                                {
                                    dr1["MoveNum"] = "0";
                                }
                                dr1["MoveNum"] = Convert.ToString(Convert.ToInt32(drMonthStockCost["Number"].ToString().Trim()) + Convert.ToInt32(dr1["MoveNum"].ToString().Trim()));
                            }
                        }
                        else
                        {
                            dr1["MoveNum"] = "0";
                        }
                    }
                }

            }
            foreach (DataRow dr1 in dtbl.Rows)
            {

                DataSet dset = bl.GetExpression(GlobalString.Expression.Made_RID.ToString());//製成卡數："1"
                number = 0;
                for (i = 0; i < dset.Tables[0].Rows.Count; i++)
                {
                    if (CardNumber.Tables[0].Select("Status_RID='" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + "' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                        j = Convert.ToInt32(CardNumber.Tables[0].Select("Status_RID='" + dset.Tables[0].Rows[i]["Type_RID"].ToString() + "' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());
                    else
                        j = 0;
                    if (dset.Tables[0].Rows[i]["Operate"].ToString() == "+")
                        number += j;
                    if (dset.Tables[0].Rows[i]["Operate"].ToString() == "-")
                        number -= j;
                }
                dr1["製成卡數"] = number.ToString();
                DataSet dset1 = bl.GetExpression(GlobalString.Expression.Waste_RID.ToString());//耗損卡數："3"
                number = 0;
                for (i = 0; i < dset1.Tables[0].Rows.Count; i++)
                {
                    if (CardNumber.Tables[0].Select("Status_RID='" + dset1.Tables[0].Rows[i]["Type_RID"].ToString() + "' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                        j = Convert.ToInt32(CardNumber.Tables[0].Select("Status_RID='" + dset1.Tables[0].Rows[i]["Type_RID"].ToString() + "' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());
                    else
                        j = 0;
                    if (dset1.Tables[0].Rows[i]["Operate"].ToString() == "+")
                        number += j;
                    if (dset1.Tables[0].Rows[i]["Operate"].ToString() == "-")
                        number -= j;
                }
                dr1["耗損卡數"] = number.ToString();
                dr1["消耗卡數"] = Convert.ToInt32(dr1["製成卡數"]) + Convert.ToInt32(dr1["耗損卡數"]);
                //}

                if (CardNumber.Tables[0].Select("Status_Code='13' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                    dr1["調整卡數"] = Convert.ToInt32(CardNumber.Tables[0].Select("Status_Code='13' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());
                else
                    dr1["調整卡數"] = "0";

                if (CardNumber.Tables[0].Select("Status_Code='12' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'").Length > 0)
                    dr1["銷毀卡數"] = Convert.ToInt32(CardNumber.Tables[0].Select("Status_Code='12' and Perso_Factory_RID='" + dr1["Perso_Factory_RID"].ToString() + "' and CardType_RID='" + dr1["CardType_RID"].ToString() + "'")[0]["Sum"].ToString());
                else
                    dr1["銷毀卡數"] = "0";

                //移轉出
                //dr1["移轉出"] = "0";
                //foreach (DataRow drMonthStockCost in drowStockCost)
                //{
                //    if (drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "0" && drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "")
                //    {
                //        DataSet ds1 = bl.GetFactoryMoveFrom(drMonthStockCost["CardType_Move_RID"].ToString().Trim());
                //        if (ds1.Tables[0].Rows.Count > 0)
                //        {
                //            string FRID = ds1.Tables[0].Rows[0]["RID"].ToString().Trim();
                //            string strCardType_RID = ds1.Tables[0].Rows[0]["CardType_RID"].ToString().Trim();
                //            if (FRID == dr1["Perso_Factory_RID"].ToString().Trim() && strCardType_RID == dr1["CardType_RID"].ToString().Trim())
                //            {
                //                if (dr1["移轉出"].ToString().Trim() == "")
                //                {
                //                    dr1["移轉出"] = "0";
                //                }
                //                dr1["移轉出"] = Convert.ToString(Convert.ToInt32(drMonthStockCost["Number"].ToString().Trim()) + Convert.ToInt32(dr1["移轉出"].ToString().Trim()));
                //            }
                //        }
                //        else
                //        {
                //            dr1["移轉出"] = "0";
                //        }
                //    }
                //}
            }


            DataTable dtMSC = this.CreateTable();
            DataRow drMSC = null;

            foreach (DataRow drMonthStockCost in drowStockCost)
            {
                DataRow dr = null;
                DataRow drmo = null;
                drMSC = dtMSC.NewRow();
                //版面簡稱
                drMSC["版面簡稱"] = drMonthStockCost["Name"].ToString();

                //PERSO 廠
                drMSC["PERSO 廠"] = drMonthStockCost["Factory_ShortName_CN"].ToString();
                drMSC["Factory_RID"] = drMonthStockCost["Factory_RID"].ToString();

                //進貨作業RID=Operate_RID；
                string strOperate_RID = drMonthStockCost["Operate_RID"].ToString();
                //進貨作業類別=Operate_Type；
                string strOperate_Type = drMonthStockCost["Operate_Type"].ToString();
                //移轉單號=CardType_Move_RID；
                string strCardType_Move_RID = drMonthStockCost["CardType_Move_RID"].ToString();

                //add by Jacky on 20090411!
                string strUselog_Rid = drMonthStockCost["uselog_rid"].ToString();
                string strlog_rid = drMonthStockCost["log_rid"].ToString();
                //期初的日結日期：
                string Date_Time = "";
                //Date_Time = bl.GetLastWorkDay(DateFrom);//該日期區間的前一個工作日
                Date_Time = bl.GetFirstWorkDay(DateFrom, DateTo);//該日期區間内的第一個工作日

                string strIncomeDate = Convert.ToDateTime(drMonthStockCost["Income_Date"].ToString()).ToString("yyyy/MM/dd");
                string strActualDate = GlobalStringManager.ActualDate;
                string strUnit_Price = "0.0";
                if (drMonthStockCost["Unit_Price"].ToString().Trim() != "")
                {
                    strUnit_Price = drMonthStockCost["Unit_Price"].ToString().Trim();
                }

                //期初庫存
                DataSet UsableNumber = null;
                if (strlog_rid.ToString().Trim() != "")
                {
                    strUselog_Rid = strlog_rid;
                    UsableNumber = bl.GetUsableNumberMove(drMonthStockCost["Factory_RID"].ToString(), drMonthStockCost["CardType_RID"].ToString(), Date_Time, strlog_rid);
                }
                else
                {
                    UsableNumber = bl.GetUsableNumber(strOperate_RID, strOperate_Type, strCardType_Move_RID, drMonthStockCost["Factory_RID"].ToString(), drMonthStockCost["CardType_RID"].ToString(), Date_Time, strUselog_Rid);
                }
                if (UsableNumber.Tables[0].Rows.Count > 0)
                {
                    //期初庫存數     
                    if (UsableNumber.Tables[0].Rows[0]["Usable_Number_Initial"].ToString().Trim() != "")
                    {
                        drMSC["期初庫存數"] = UsableNumber.Tables[0].Rows[0]["Usable_Number_Initial"].ToString().Trim();
                    }
                    else
                    {
                        drMSC["期初庫存數"] = "0";
                    }

                    //string Operate_RID_Initial = UsableNumber.Tables[0].Rows[0]["Operate_RID_Initial"].ToString();
                    //string Operate_Type_Initial = UsableNumber.Tables[0].Rows[0]["Operate_Type_Initial"].ToString();

                    //期初未稅單價
                    if (strIncomeDate.CompareTo(strActualDate) < 0 || strOperate_RID == "0")
                    {
                        drMSC["期初未稅單價"] = strUnit_Price;
                    }
                    else
                    {
                        if (!StringUtil.IsEmpty(strOperate_Type))
                        {
                            DataSet UnitPrice = bl.GetUnitPrice(strOperate_RID, strOperate_Type, Date_Time);
                            if (UnitPrice.Tables[0].Rows.Count > 0)
                            {
                                string strReal_Ask_Number = UnitPrice.Tables[0].Rows[0]["Real_Ask_Number"].ToString().Trim();
                                decimal dReal_Ask_Number = 0;
                                if (strReal_Ask_Number != "")
                                {
                                    dReal_Ask_Number = Convert.ToDecimal(strReal_Ask_Number);
                                }

                                if (dReal_Ask_Number == 0)//未請款
                                {

                                    if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                                    {
                                        if (UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
                                        {
                                            drMSC["期初未稅單價"] = UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim();
                                        }
                                        else
                                        {
                                            drMSC["期初未稅單價"] = "0.0000";
                                        }
                                    }
                                    else
                                    {
                                        if (UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
                                        {
                                            decimal unit_price = Convert.ToDecimal(UnitPrice.Tables[0].Rows[0]["Unit_Price_Order"]) / 1.05M;
                                            drMSC["期初未稅單價"] = unit_price.ToString("N4");
                                        }
                                        else
                                        {
                                            drMSC["期初未稅單價"] = "0.0000";
                                        }
                                    }
                                }
                                else//已請款
                                {
                                    if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                                    {
                                        if (UnitPrice.Tables[0].Rows[0]["Unit_Price"].ToString().Trim() != "")
                                        {
                                            drMSC["期初未稅單價"] = UnitPrice.Tables[0].Rows[0]["Unit_Price"].ToString().Trim();
                                        }
                                        else
                                        {
                                            drMSC["期初未稅單價"] = "0.0000";
                                        }
                                    }
                                    else
                                    {
                                        if (UnitPrice.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim() != "")
                                        {
                                            drMSC["期初未稅單價"] = UnitPrice.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim();
                                        }
                                        else
                                        {
                                            drMSC["期初未稅單價"] = "0.0000";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                drMSC["期初未稅單價"] = "0.0000";
                            }
                        }
                        else
                        {
                            drMSC["期初未稅單價"] = "0.0000";
                        }
                    }
                }
                else
                {
                    drMSC["期初庫存數"] = "0";
                    drMSC["期初未稅單價"] = "0.0000";
                }

                //進貨數，移轉入  
                //加入判斷，如果入庫日期小於上線日，則不認為是進貨或是轉移入！
                string strIncome_Date = Convert.ToDateTime(drMonthStockCost["Income_Date"].ToString().Trim()).ToString("yyyy/MM/dd");
                if (DateFrom.CompareTo(strIncome_Date) <= 0 && strIncome_Date.CompareTo(DateTo) <= 0 && strIncomeDate.CompareTo(strActualDate) >= 0)
                {//本期進貨或移轉
                    if (drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "0" && drMonthStockCost["CardType_Move_RID"].ToString().Trim() != "")
                    {
                        drMSC["進貨數"] = "0";
                        drMSC["移轉入"] = drMonthStockCost["Number"].ToString();
                    }
                    else
                    {
                        drMSC["進貨數"] = drMonthStockCost["Number"].ToString();
                        drMSC["移轉入"] = "0";
                    }
                }
                else
                {
                    drMSC["進貨數"] = "0";
                    drMSC["移轉入"] = "0";
                }

                // edit by Ian Huang 帳務管理模組/卡片成本/-月庫存成本查詢、庫存成本查詢 start
                //製成卡數,耗損卡數,消耗卡數,銷毀卡數,調整卡數以及移轉出卡數.
                if (dtbl.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "'").Length > 0)
                {
                    dr = dtbl.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "'")[0];
                    number = Convert.ToInt32(drMSC["期初庫存數"].ToString()) + Convert.ToInt32(drMSC["進貨數"].ToString()) + Convert.ToInt32(drMSC["移轉入"].ToString());
                    //if (number > 0 && dr["移轉出"].ToString() != "" && Convert.ToInt32(dr["移轉出"].ToString()) > 0)
                    //{
                    //    drMSC["移轉出"] = bl.GetCardCount(number, dr, "移轉出").ToString();
                    //    number -= Convert.ToInt32(drMSC["移轉出"]);
                    //}
                    //else
                    //{
                    //    drMSC["移轉出"] = "0";
                    //}

                    DateTime tCheck = DateTime.Parse(DateTo).AddDays(-10);
                    int iMOCheck = -1;
                    if (dtmo.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "' and MoveUnit_Price='" + drMonthStockCost["Unit_Price"].ToString().Trim() + "'").Length > 0)
                    {
                        DateTime tMOCheck = (DateTime)dtmo.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "' and MoveUnit_Price='" + drMonthStockCost["Unit_Price"].ToString().Trim() + "'")[0]["MoveDate"];
                        iMOCheck = tMOCheck.CompareTo(tCheck);
                    }

                    if (-1 == iMOCheck)
                    {
                        if (dtmo.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "' and MoveUnit_Price='" + drMonthStockCost["Unit_Price"].ToString().Trim() + "'").Length > 0)
                        {
                            drmo = dtmo.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "' and MoveUnit_Price='" + drMonthStockCost["Unit_Price"].ToString().Trim() + "'")[0];
                            if (number > 0 && drmo["MoveNum"].ToString() != "" && Convert.ToInt32(drmo["MoveNum"].ToString()) > 0)
                            {
                                drMSC["移轉出"] = bl.GetCardCount(number, drmo, "MoveNum").ToString();
                                number -= Convert.ToInt32(drMSC["移轉出"]);
                            }
                            else
                            {
                                drMSC["移轉出"] = "0";
                            }
                        }
                        else
                        {
                            drMSC["移轉出"] = "0";
                        }
                    }


                    //if (number > 0 && dr["調整卡數"].ToString() != "" && Convert.ToInt32(dr["調整卡數"].ToString()) > 0)
                    if (number > 0 && dr["調整卡數"].ToString() != "")
                    {
                        drMSC["調整卡數"] = bl.GetCardCount(number, dr, "調整卡數").ToString();
                        number -= Convert.ToInt32(drMSC["調整卡數"]);
                    }
                    else
                    {
                        drMSC["調整卡數"] = "0";
                    }

                    if (number > 0 && dr["銷毀卡數"].ToString() != "" && Convert.ToInt32(dr["銷毀卡數"].ToString()) > 0)
                    {
                        drMSC["銷毀卡數"] = bl.GetCardCount(number, dr, "銷毀卡數").ToString();
                        number -= Convert.ToInt32(drMSC["銷毀卡數"]);
                    }
                    else
                    {
                        drMSC["銷毀卡數"] = "0";
                    }

                    if (dropGroup.SelectedItem.Text == "晶片信用卡")
                    {
                        drMSC["消耗卡數"] = "0";
                        if (number > 0 && dr["耗損卡數"].ToString() != "" && Convert.ToInt32(dr["耗損卡數"].ToString()) > 0)
                        {
                            drMSC["耗損卡數"] = bl.GetCardCount(number, dr, "耗損卡數").ToString();
                            number -= Convert.ToInt32(drMSC["耗損卡數"]);
                        }
                        else
                        {
                            drMSC["耗損卡數"] = "0";
                        }

                        if (number > 0 && dr["製成卡數"].ToString() != "" && Convert.ToInt32(dr["製成卡數"].ToString()) > 0)
                        {
                            drMSC["製成卡數"] = bl.GetCardCount(number, dr, "製成卡數").ToString();
                            number -= Convert.ToInt32(drMSC["製成卡數"]);
                        }
                        else
                        {
                            drMSC["製成卡數"] = "0";
                        }

                    }
                    else
                    {
                        drMSC["製成卡數"] = "0";
                        drMSC["耗損卡數"] = "0";
                        if (number > 0 && dr["消耗卡數"].ToString() != "" && Convert.ToInt32(dr["消耗卡數"].ToString()) > 0)
                        {
                            drMSC["消耗卡數"] = bl.GetCardCount(number, dr, "消耗卡數").ToString();
                            number -= Convert.ToInt32(drMSC["消耗卡數"]);
                        }
                        else
                        {
                            drMSC["消耗卡數"] = "0";
                        }
                    }

                    if (-1 != iMOCheck)
                    {
                        //200907IR-移轉出與移轉入金額不符
                        if (dtmo.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "' and MoveUnit_Price='" + drMonthStockCost["Unit_Price"].ToString().Trim() + "'").Length > 0)
                        {
                            drmo = dtmo.Select("Perso_Factory_RID='" + drMonthStockCost["Factory_RID"].ToString() + "' and CardType_RID='" + drMonthStockCost["CardType_RID"].ToString() + "' and MoveUnit_Price='" + drMonthStockCost["Unit_Price"].ToString().Trim() + "'")[0];
                            if (number > 0 && drmo["MoveNum"].ToString() != "" && Convert.ToInt32(drmo["MoveNum"].ToString()) > 0)
                            {
                                drMSC["移轉出"] = bl.GetCardCount(number, drmo, "MoveNum").ToString();
                                number -= Convert.ToInt32(drMSC["移轉出"]);
                            }
                            else
                            {
                                drMSC["移轉出"] = "0";
                            }
                        }
                        else
                        {
                            drMSC["移轉出"] = "0";
                        }
                    }

                }
                else
                {
                    drMSC["移轉出"] = "0";
                    drMSC["消耗卡數"] = "0";
                    drMSC["製成卡數"] = "0";
                    drMSC["耗損卡數"] = "0";
                    drMSC["銷毀卡數"] = "0";
                    drMSC["調整卡數"] = "0";
                }
                // edit by Ian Huang 帳務管理模組/卡片成本/-月庫存成本查詢、庫存成本查詢 end

                //期末庫存數                
                if (dropGroup.SelectedItem.Text == "晶片信用卡")
                    drMSC["期末庫存數"] = Convert.ToString(Convert.ToInt32(drMSC["期初庫存數"].ToString()) + Convert.ToInt32(drMSC["進貨數"].ToString()) + Convert.ToInt32(drMSC["移轉入"].ToString()) - Convert.ToInt32(drMSC["移轉出"].ToString()) - Convert.ToInt32(drMSC["製成卡數"].ToString()) - Convert.ToInt32(drMSC["耗損卡數"].ToString()) - Convert.ToInt32(drMSC["銷毀卡數"].ToString()) - Convert.ToInt32(drMSC["調整卡數"].ToString()));
                else
                    drMSC["期末庫存數"] = Convert.ToString(Convert.ToInt32(drMSC["期初庫存數"].ToString()) + Convert.ToInt32(drMSC["進貨數"].ToString()) + Convert.ToInt32(drMSC["移轉入"].ToString()) - Convert.ToInt32(drMSC["移轉出"].ToString()) - Convert.ToInt32(drMSC["消耗卡數"].ToString()) - Convert.ToInt32(drMSC["銷毀卡數"].ToString()) - Convert.ToInt32(drMSC["調整卡數"].ToString()));

                //期末的日結日期
                string strLastCheckDate = bl.GetNextWorkDay(DateTo);//取這個日期區間之后的第一個工作日
                if (drMSC["期末庫存數"].ToString() == "0")
                {//取該日期區間内最後一次日結的日期
                    strLastCheckDate = bl.GetLastCheckDate(strOperate_RID, strOperate_Type, strCardType_Move_RID, drMonthStockCost["Factory_RID"].ToString(), drMonthStockCost["CardType_RID"].ToString(), DateFrom, DateTo);
                }

                //未稅單價
                if (strIncomeDate.CompareTo(strActualDate) < 0 || strOperate_RID == "0")
                {
                    drMSC["未稅單價"] = strUnit_Price;
                }
                else
                {
                    if (!StringUtil.IsEmpty(drMonthStockCost["Operate_Type"].ToString()))
                    {
                        DataSet UnitPrice1 = bl.GetUnitPrice(drMonthStockCost["Operate_RID"].ToString(), drMonthStockCost["Operate_Type"].ToString(), strLastCheckDate.Split(' ')[0]);
                        if (UnitPrice1.Tables[0].Rows.Count > 0)
                        {
                            string strReal_Ask_Number = UnitPrice1.Tables[0].Rows[0]["Real_Ask_Number"].ToString().Trim();
                            decimal dReal_Ask_Number = 0;
                            if (strReal_Ask_Number != "")
                            {
                                dReal_Ask_Number = Convert.ToDecimal(strReal_Ask_Number);
                            }

                            if (dReal_Ask_Number == 0)//未請款
                            {
                                if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                                {
                                    if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
                                    {
                                        drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim();
                                    }
                                    else
                                    {
                                        drMSC["未稅單價"] = "0.0000";
                                    }
                                }
                                else
                                {
                                    if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"].ToString().Trim() != "")
                                    {
                                        decimal unit_price = Convert.ToDecimal(UnitPrice1.Tables[0].Rows[0]["Unit_Price_Order"]) / 1.05M;
                                        drMSC["未稅單價"] = unit_price.ToString("N4");
                                    }
                                    else
                                    {
                                        drMSC["未稅單價"] = "0.0000";
                                    }
                                }
                            }
                            else//已請款
                            {
                                if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
                                {
                                    if (UnitPrice1.Tables[0].Rows[0]["Unit_Price"].ToString().Trim() != "")
                                    {
                                        drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price"].ToString().Trim();
                                    }
                                    else
                                    {
                                        drMSC["未稅單價"] = "0.0000";
                                    }
                                }
                                else
                                {
                                    if (UnitPrice1.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim() != "")
                                    {
                                        drMSC["未稅單價"] = UnitPrice1.Tables[0].Rows[0]["Unit_Price_No"].ToString().Trim();
                                    }
                                    else
                                    {
                                        drMSC["未稅單價"] = "0.0000";
                                    }
                                }
                            }
                        }
                        else
                        {
                            drMSC["未稅單價"] = "0.0000";
                        }
                    }
                    else
                    {
                        drMSC["未稅單價"] = "0.0000";
                    }
                }

                //期初庫存金額(未(含)稅)                
                string strEndPrice = bl.GetLastEndPrice(txtDate.Text, strUselog_Rid, dropUse.SelectedItem.Value, dropGroup.SelectedItem.Value);
                if (strEndPrice == "0.00")
                {
                    drMSC["期初庫存金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["期初庫存數"].ToString()) * Convert.ToDecimal(drMSC["期初未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                }
                else
                {
                    drMSC["期初庫存金額"] = strEndPrice;
                }
                //進貨金額                
                drMSC["進貨金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["進貨數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));

                //移轉入金額                
                drMSC["移轉入金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["移轉入"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));

                //移轉出金額                
                drMSC["移轉出金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["移轉出"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));

                //銷毀卡金額,調整卡金額                
                drMSC["銷毀卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["銷毀卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                drMSC["調整卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["調整卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));

                //製成卡金額,耗損卡金額,消耗卡金額                
                if (dropGroup.SelectedItem.Text != "晶片信用卡")
                {
                    drMSC["製成卡金額"] = "0.00";
                    drMSC["耗損卡金額"] = "0.00";
                    if (drMSC["期末庫存數"].ToString() == "0")
                    {
                        //200907CR消耗卡的金額=期初庫存金額+進貨金額+移轉入金額-移轉出金額-銷毀卡金額+調整卡金額
                        drMSC["消耗卡金額"] = Convert.ToString(Convert.ToDecimal(drMSC["期初庫存金額"]) + Convert.ToDecimal(drMSC["進貨金額"]) + Convert.ToDecimal(drMSC["移轉入金額"]) - Convert.ToDecimal(drMSC["移轉出金額"]) - Convert.ToDecimal(drMSC["銷毀卡金額"]) + Convert.ToDecimal(drMSC["調整卡金額"]));
                    }
                    else
                    {
                        drMSC["消耗卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["消耗卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                    }
                }
                else
                {
                    drMSC["製成卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["製成卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                    drMSC["消耗卡金額"] = "0.00";

                    if (drMSC["期末庫存數"].ToString() == "0")
                    {
                        //200907CR耗損卡的金額=期初庫存金額+進貨金額+移轉入金額-移轉出金額-製成卡金額-銷毀卡金額+調整卡金額
                        drMSC["耗損卡金額"] = Convert.ToString(Convert.ToDecimal(drMSC["期初庫存金額"]) + Convert.ToDecimal(drMSC["進貨金額"]) + Convert.ToDecimal(drMSC["移轉入金額"]) - Convert.ToDecimal(drMSC["移轉出金額"]) - Convert.ToDecimal(drMSC["製成卡金額"]) - Convert.ToDecimal(drMSC["銷毀卡金額"]) + Convert.ToDecimal(drMSC["調整卡金額"]));
                    }
                    else
                    {
                        drMSC["耗損卡金額"] = Convert.ToString(Math.Round(Convert.ToInt32(drMSC["耗損卡數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                    }
                }



                //期末庫存未(含)稅金額                
                //drMSC["期末庫存未稅金額"] = Convert.ToString(Convert.ToInt32(drMSC["期末庫存數"].ToString()) * Convert.ToDecimal(drMSC["未稅單價"].ToString()), MidpointRounding.AwayFromZero));
                drMSC["期末庫存未稅金額"] = Convert.ToString(Convert.ToDecimal(drMSC["期初庫存金額"]) + Convert.ToDecimal(drMSC["進貨金額"]) + Convert.ToDecimal(drMSC["移轉入金額"]) - Convert.ToDecimal(drMSC["移轉出金額"]) - Convert.ToDecimal(drMSC["製成卡金額"]) - Convert.ToDecimal(drMSC["耗損卡金額"]) - Convert.ToDecimal(drMSC["消耗卡金額"]) - Convert.ToDecimal(drMSC["銷毀卡金額"]) + Convert.ToDecimal(drMSC["調整卡金額"]));

                //單價調整金額                
                if (Convert.ToInt32(drMSC["期初庫存數"]) != 0 && (Convert.ToDecimal(drMSC["期初未稅單價"].ToString()) != Convert.ToDecimal(drMSC["未稅單價"].ToString())))
                {
                    drMSC["單價調整金額"] = Convert.ToString(Convert.ToDecimal(drMSC["期初庫存金額"].ToString()) - Convert.ToDecimal(drMSC["期末庫存未稅金額"].ToString()));

                }
                else
                    drMSC["單價調整金額"] = "0.00";

                dtMSC.Rows.Add(drMSC);
            }

            //added by Even.Cheng on 20090115
            DataTable dtResult = dtMSC;
            if (dropFactoryRID.SelectedValue != "")
            {
                dtResult = this.CreateTable();
                DataRow[] drs = dtMSC.Select("Factory_RID=" + dropFactoryRID.SelectedValue);
                foreach (DataRow dr in drs)
                {
                    DataRow drResult = dtResult.NewRow();
                    for (int m = 0; m < drResult.ItemArray.Length; m++)
                    {
                        drResult[m] = dr[m];
                    }
                    dtResult.Rows.Add(drResult);
                }
            }
            //end add


            //資料表中的合計行
            if (dtResult.Rows.Count > 0)
            {
                drMSC = dtResult.NewRow();
                drMSC["版面簡稱"] = "合計";
                int total = 0;
                decimal total1 = 0;
                for (j = 0; j < dtResult.Rows.Count; j++)
                {
                    if (dtResult.Rows[j][2].ToString().Trim() != "")
                        total += Convert.ToInt32(dtResult.Rows[j][2].ToString().Trim());
                }
                drMSC[2] = total.ToString();
                for (j = 0; j < dtResult.Rows.Count; j++)
                {
                    if (dtResult.Rows[j][3].ToString().Trim() != "")
                        total1 += Convert.ToDecimal(dtResult.Rows[j][3].ToString().Trim());
                }
                drMSC[3] = total1.ToString();
                for (i = 4; i < 13; i++)
                {
                    total = 0;
                    for (j = 0; j < dtResult.Rows.Count; j++)
                    {
                        if (dtResult.Rows[j][i].ToString().Trim() != "")
                            total += Convert.ToInt32(dtResult.Rows[j][i].ToString().Trim());
                    }
                    drMSC[i] = total.ToString();
                }
                for (i = 13; i < dtResult.Columns.Count - 1; i++)
                {
                    total1 = 0;
                    for (j = 0; j < dtResult.Rows.Count; j++)
                    {
                        if (dtResult.Rows[j][i].ToString().Trim() != "")
                            total1 += Convert.ToDecimal(dtResult.Rows[j][i].ToString().Trim());
                    }
                    drMSC[i] = total1.ToString();
                }
                drMSC[3] = "";
                drMSC[13] = "";
                dtResult.Rows.Add(drMSC);


            }


            //設置表格標題
            if (dropGroup.SelectedItem.Text == "現金卡" || dropGroup.SelectedItem.Text == "晶片金融卡")
            {
                dtResult.Columns[3].ColumnName = "期初含稅單價";
                dtResult.Columns[13].ColumnName = "含稅單價";
                dtResult.Columns[14].ColumnName = "期初庫存金額(含稅)";
                dtResult.Columns[24].ColumnName = "期末庫存含稅金額";
            }
            else
            {
                dtResult.Columns[3].ColumnName = "期初未稅單價";
                dtResult.Columns[13].ColumnName = "未稅單價";
                dtResult.Columns[14].ColumnName = "期初庫存金額(未稅)";
                dtResult.Columns[24].ColumnName = "期末庫存未稅金額";
            }

            //顯示欄位
            dtResult.Columns.Remove("Factory_RID");
            if (dropGroup.SelectedItem.Text != "晶片信用卡")
            {
                dtResult.Columns.Remove("製成卡數");
                dtResult.Columns.Remove("製成卡金額");
                dtResult.Columns.Remove("耗損卡數");
                dtResult.Columns.Remove("耗損卡金額");
            }
            else
            {
                dtResult.Columns.Remove("消耗卡數");
                dtResult.Columns.Remove("消耗卡金額");
            }



            //千分位
            foreach (DataRow dr in dtResult.Rows)
            {
                if (dr[0].ToString() == "合計")
                {
                    if (dropGroup.SelectedItem.Text != "晶片信用卡")
                    {
                        dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
                        for (i = 4; i < 11; i++)
                            dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
                        for (i = 12; i <= dtResult.Columns.Count - 1; i++)
                            dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
                    }
                    else
                    {
                        dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
                        for (i = 4; i < 12; i++)
                            dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
                        for (i = 13; i <= dtResult.Columns.Count - 1; i++)
                            dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
                    }
                }
                else
                {
                    if (dropGroup.SelectedItem.Text != "晶片信用卡")
                    {
                        dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
                        dr[3] = Convert.ToDecimal(dr[3].ToString()).ToString("N4");
                        dr[11] = Convert.ToDecimal(dr[11].ToString()).ToString("N4");
                        for (i = 4; i < 11; i++)
                            dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
                        for (i = 12; i <= dtResult.Columns.Count - 1; i++)
                            dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
                    }
                    else
                    {
                        dr[2] = Convert.ToInt32(dr[2].ToString()).ToString("N0");
                        dr[3] = Convert.ToDecimal(dr[3].ToString()).ToString("N4");
                        dr[12] = Convert.ToDecimal(dr[12].ToString()).ToString("N4");
                        for (i = 4; i < 12; i++)
                            dr[i] = Convert.ToInt32(dr[i].ToString()).ToString("N0");
                        for (i = 13; i <= dtResult.Columns.Count - 1; i++)
                            dr[i] = Convert.ToDecimal(dr[i].ToString()).ToString("N2");
                    }
                }
            }



            //傳參數，添加報表數據
            Session["MonthStockCost"] = dtResult;

            return dtResult;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateAlertException(this, ex.Message);
            return null;
        }
    }
}
