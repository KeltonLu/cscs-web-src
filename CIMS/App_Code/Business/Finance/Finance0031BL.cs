using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Finance0031BL 的摘要描述
/// </summary>
public class Finance0031BL : BaseLogic
{
    #region SQL語句
    public const string SEL_MATERIEL_SAP = "SELECT *,"+
                "CASE Materiel_Type WHEN '1' THEN '寄卡單(卡)' WHEN '2' THEN '寄卡單(銀)' "+
                "WHEN '3' THEN '信封(卡)' WHEN '4' THEN '信封(銀)' WHEN '5' THEN 'DM(卡)' "+
                "WHEN '6' THEN 'DM(銀)' WHEN'7' THEN '郵資費(卡)' WHEN '8' THEN '郵資費(銀)' END AS Material_Type_Name "+
                "FROM MATERIEL_SAP WHERE RST = 'A'";

    public const string SEL_MATERIEL_PURCHASE_FORM = "SELECT RST, Purchase_Date,(PurchaseOrder_RID+Detail_RID) as PurchaseOrder_Detail_RID, "+
                "PurchaseOrder_RID,Detail_RID, Serial_Number,Material_Type,Material_Name, "+
                "Billing_Type, Unit_Price, Total_Num, Total_Price "+
                "FROM (SELECT MPF.RST,MPF.Purchase_Date,MPF.PurchaseOrder_RID,MPF.Serial_Number,"+
                        "Detail_RID,substring(MPF.Serial_Number,1,1) as Material_Type,"+
                        "CASE substring(MPF.Serial_Number,1,1) WHEN 'B' THEN CE.Name WHEN 'A' THEN EI.Name WHEN 'C' THEN DM.Name END AS Material_Name,"+
                        "CASE substring(MPF.Serial_Number,1,1) WHEN 'B' THEN CE.Billing_Type WHEN 'A' THEN EI.Billing_Type WHEN 'C' THEN DM.Billing_Type END AS Billing_Type,"+
                        "MPF.Unit_Price,MPF.Total_Num,MPF.Total_Price "+
                    "FROM MATERIEL_PURCHASE_FORM MPF LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MPF.Serial_Number = CE.Serial_Number "+
                        "LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MPF.Serial_Number = EI.Serial_Number "+
                        "LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MPF.Serial_Number = DM.Serial_Number "+
                    "WHERE ISNULL(MPF.SAP_Serial_Number,'')='') A "+
                "WHERE A.RST = 'A'";

    public const string SEL_ALL_SAP = "SELECT DISTINCT SAP_Serial_Number " +
                "FROM CARD_TYPE_SAP " +
                "WHERE RST = 'A'  " +
                "UNION " +
                "SELECT DISTINCT SAP_ID " +
                "FROM PERSO_PROJECT_SAP " +
                "WHERE RST = 'A' " +
                "UNION " +
                "SELECT DISTINCT SAP_ID " +
                "FROM MATERIEL_SAP " +
                "WHERE RST = 'A'";

    public const string SEL_MATERIEL_BUDGET = "SELECT Budget "+
                "FROM MATERIEL_BUDGET "+
                "WHERE RST = 'A' AND Budget_Year = year(getdate()) AND Materiel_Type = @material_type ";

    public const string SEL_MATERIEL_SAP_SUM = "SELECT SUM(Sum) " +
                "FROM MATERIEL_SAP " +
                "WHERE RST = 'A' AND YEAR(Ask_Date) = year(getdate()) AND Materiel_Type = @material_type " +
                "GROUP BY Materiel_Type ";

    public const string CON_MATERIEL_SAP_SAME_MATERIAL_TYPE = "SELECT * "+
                "FROM MATERIEL_SAP "+
                "WHERE RST = 'A' AND Convert(varchar(10),Ask_Date,111) = @ask_date AND Materiel_Type = @materiel_type";

    public const string UPDATE_MATERIEL_PURCHASE_FORM = "UPDATE MATERIEL_PURCHASE_FORM "+
                "SET SAP_Serial_Number = @sap_serial_number,Ask_Date = @ask_date,Pay_Date = @pay_date "+
                "WHERE PurchaseOrder_RID = @purchaseorder_RID AND Detail_RID = @detail_rid";

    public const string SEL_MATERIEL_PURCHASE_FORM_EDIT = "SELECT MPF.PurchaseOrder_RID,MPF.Detail_RID,MPF.SAP_Serial_Number,CASE substring(MPF.Serial_Number,1,1) WHEN 'B' THEN CE.Name WHEN 'A' THEN EI.Name WHEN 'C' THEN DM.Name END AS Material_Name,MPF.Unit_Price,MPF.Total_Num,MPF.Total_Price FROM MATERIEL_PURCHASE_FORM MPF LEFT JOIN CARD_EXPONENT CE ON CE.RST = 'A' AND MPF.Serial_Number = CE.Serial_Number LEFT JOIN ENVELOPE_INFO EI ON EI.RST = 'A' AND MPF.Serial_Number = EI.Serial_Number LEFT JOIN DMTYPE_INFO DM ON DM.RST = 'A' AND MPF.Serial_Number = DM.Serial_Number WHERE MPF.RST = 'A' ";

    public const string Con_SAP = "SELECT COUNT(*) FROM  (SELECT DISTINCT SAP_Serial_Number FROM CARD_TYPE_SAP WHERE RST = 'A' UNION SELECT DISTINCT SAP_ID FROM PERSO_PROJECT_SAP WHERE RST = 'A' UNION SELECT DISTINCT SAP_ID FROM MATERIEL_SAP WHERE RST = 'A' AND RID<>@RID) A WHERE  A.SAP_Serial_Number = @SAP";

    public const string SEL_MATERIEL_BUDGET_UPDATE = "SELECT Budget FROM MATERIEL_BUDGET WHERE RST = 'A' AND Budget_Year = year(getdate()) AND Materiel_Type = @material_type";

    public const string SEL_MATERIEL_SAP_SUM_UPDATE = "SELECT isnull(SUM(Sum),0) FROM MATERIEL_SAP WHERE RST = 'A' AND YEAR(Ask_Date) = year(getdate()) AND Materiel_Type = @material_type AND RID<>@rid";

    public const string Update_MATERIEL_PURCHASE_FORM_UPDATE = "Update MATERIEL_PURCHASE_FORM SET SAP_Serial_Number = @sap_serial_number,Ask_Date = @ask_date,Pay_Date = @pay_date WHERE SAP_Serial_Number = (SELECT SAP_ID FROM MATERIEL_SAP WHERE RID = @rid)";

    public const string UPDATE_MATERIEL_SAP = "UPDATE MATERIEL_SAP SET Ask_Date = @ask_date,SAP_ID = @sap_serial_number,Sum  = @sum,Pay_Date = @pay_date WHERE RID = @rid";

    public const string CON_DEL_MATERIEL_SAP = "SELECT Pay_Date FROM MATERIEL_SAP WHERE RST = 'A' AND RID = @rid";

    public const string Update_MATERIEL_PURCHASE_FORM_DEL = "Update MATERIEL_PURCHASE_FORM SET SAP_Serial_Number = '',Ask_Date = '1900-01-01',Pay_Date = '1900-01-01' WHERE SAP_Serial_Number = (SELECT SAP_ID FROM MATERIEL_SAP WHERE RID = @rid)";

    public const string DEL_MATERIEL_SAP = "DELETE FROM MATERIEL_SAP WHERE RST = 'A' AND RID = @rid";

    #endregion
    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Finance0031BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢物料及郵資費用請款訊息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet SearchSAP(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? " Ask_Date,Materiel_Type" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_MATERIEL_SAP);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropAsk_Project"].ToString().Trim()))
            {
                if (searchInput["dropAsk_Project"].ToString() != "全部")
                {
                    if (searchInput["dropAsk_Project"].ToString() == "DM")
                    {
                        stbWhere.Append(" AND (Materiel_Type='5' OR Materiel_Type='6')");
                    }
                    else if (searchInput["dropAsk_Project"].ToString() == "信封")
                    {
                        stbWhere.Append(" AND (Materiel_Type='3' OR Materiel_Type='4')");
                    }
                    else if (searchInput["dropAsk_Project"].ToString() == "郵資費")
                    {
                        stbWhere.Append(" AND (Materiel_Type='7' OR Materiel_Type='8')");
                    }
                    else if (searchInput["dropAsk_Project"].ToString() == "寄卡單")
                    {
                        stbWhere.Append(" AND (Materiel_Type='1' OR Materiel_Type='2')");
                    }
                }
            }
            if (!StringUtil.IsEmpty(searchInput["txtSAP_Serial_Number"].ToString().Trim()))
            {
                stbWhere.Append(" AND SAP_ID like @sap_serial_number ");
                dirValues.Add("sap_serial_number", "%" + searchInput["txtSAP_Serial_Number"].ToString().Trim() + "%");

            }
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date1"].ToString()))
            {
                stbWhere.Append(" AND Ask_Date>= @Begin_Date1");
                dirValues.Add("Begin_Date1", searchInput["txtBegin_Date1"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date1"].ToString()))
            {
                stbWhere.Append(" AND Ask_Date<= @Finish_Date1");
                dirValues.Add("Finish_Date1", searchInput["txtFinish_Date1"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date2"].ToString()))
            {
                stbWhere.Append(" AND Pay_Date>= @Begin_Date2");
                dirValues.Add("Begin_Date2", searchInput["txtBegin_Date2"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date2"].ToString()))
            {
                stbWhere.Append(" AND Pay_Date<= @Finish_Date2");
                dirValues.Add("Finish_Date2", searchInput["txtFinish_Date2"].ToString());
            }
        }
        DataSet dtSAP = null;
        try
        {
            dtSAP = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        rowCount = intRowCount;
        return dtSAP;
    }


    /// <summary>
    /// 查詢未請款物料採購明細
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet SearchMaterialPurchase(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1、整理參數--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? " Unit_Price,PurchaseOrder_Detail_RID " : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_MATERIEL_PURCHASE_FORM);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtMaterial_Name"].ToString().Trim()))
            {
                stbWhere.Append(" AND Material_Name like  @material_name ");
                dirValues.Add("material_name", "%" + searchInput["txtMaterial_Name"].ToString().Trim() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date1"].ToString()))
            {
                stbWhere.Append(" AND Purchase_Date >= @Begin_Date1");
                dirValues.Add("Begin_Date1", searchInput["txtBegin_Date1"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date1"].ToString()))
            {
                stbWhere.Append(" AND Purchase_Date <= @Finish_Date1");
                dirValues.Add("Finish_Date1", searchInput["txtFinish_Date1"].ToString());
            }
        }

        DataSet dtSAP = null;
        try
        {
            dtSAP = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
            DataRow drNew = dtSAP.Tables[0].NewRow();
            drNew[7] = "郵資費";
            dtSAP.Tables[0].Rows.InsertAt(drNew, 0);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        rowCount = intRowCount;
        return dtSAP;
    }

    /// <summary>
    /// 檢查選中的物料請款項目是各物料帳務類別的單價是否相同
    /// </summary>
    /// <param name="dtSearchMaterialPurchase"></param>
    /// <returns></returns>
    public string CheckPrice(DataTable dtSearchMaterialPurchase)
    {
        Dictionary<string, decimal> dirVar = new Dictionary<string, decimal>();
        string strRet = "";
        bool blHaveMateriel = false;
        try
        {
            foreach (DataRow dr in dtSearchMaterialPurchase.Rows)
            {
                if (Convert.ToBoolean(dr["選中"]) && dr[7].ToString().Trim() != "郵資費")
                {
                    blHaveMateriel = false;

                    // 檢查該種物料是否已經存在
                    foreach (KeyValuePair<string, decimal> item in dirVar)
                    {
                        // 查找類型相同的
                        if (item.Key == dr["Material_Type"].ToString().Trim() +
                                dr["Billing_Type"].ToString().Trim())
                        {
                            blHaveMateriel = true;

                            // 價格不同的
                            if (item.Value != Convert.ToDecimal(dr["Unit_Price"]))
                            {
                                if (dr["Material_Type"].ToString().Trim() == "A")
                                {
                                    strRet = "信封";
                                }
                                else if (dr["Material_Type"].ToString().Trim() == "B")
                                {
                                    strRet = "寄卡單";
                                }
                                else if (dr["Material_Type"].ToString().Trim() == "C")
                                {
                                    strRet = "DM";
                                }
                                if (dr["Billing_Type"].ToString().Trim() == "1")
                                {
                                    strRet += "(卡)";
                                }
                                else if (dr["Billing_Type"].ToString().Trim() == "2")
                                {
                                    strRet += "(銀)";
                                }
                                break;
                            }
                        }
                    }

                    // 有不同價格。
                    if (strRet.Length > 0)
                    {
                        break;
                    }
                    else
                    {
                        if (!blHaveMateriel)
                        {
                            // 將類型、價格變價到Dictionary
                            dirVar.Add(dr["Material_Type"].ToString().Trim() +
                                    dr["Billing_Type"].ToString().Trim(), Convert.ToDecimal(dr["Unit_Price"]));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return strRet;
    }

    /// <summary>
    /// 取請款訊息
    /// </summary>
    /// <param name="dtRequisition_Money">DataTable(請款訊息)</param>
    /// <param name="dtSearchMaterialPurchase">DataTable(物料採購明細)</param>
    /// <returns>DataTable(請款訊息)</returns>
    public DataTable getAskMoneyInfo(DataTable dtRequisition_Money, 
        DataTable dtSearchMaterialPurchase)
    {
        Decimal[] Sum_Money = new Decimal[6];
        bool[] Select_Materiel_Type = new bool[6];

        try
        {
            foreach (DataRow dr in dtSearchMaterialPurchase.Rows)
            {
                if (Convert.ToBoolean(dr["選中"]))
                {
                    if (dr["Material_Name"].ToString().Trim() == "郵資費")
                    {
                        DataRow dr1 = dtRequisition_Money.NewRow();
                        dr1["Materiel_Type"] = "7";
                        dr1["Materiel_Type_Name"] = "郵資費(卡)";
                        dr1["Sum_Money"] = 0;
                        //dr1["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                        dtRequisition_Money.Rows.Add(dr1);
                        DataRow dr2 = dtRequisition_Money.NewRow();
                        dr2["Materiel_Type"] = "8";
                        dr2["Materiel_Type_Name"] = "郵資費(銀)";
                        dr2["Sum_Money"] = 0;
                        //dr2["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                        dtRequisition_Money.Rows.Add(dr2);
                    }
                    else
                    {
                        if (dr["Material_Type"].ToString().Trim() == "A")//(信封)
                        {
                            if (dr["Billing_Type"].ToString().Trim() == "1")//（卡）
                            {
                                Sum_Money[0] += Convert.ToInt32(dr["Total_Num"]) * Convert.ToDecimal(dr["Unit_Price"]);
                                Select_Materiel_Type[0] = true;
                            }
                            else if (dr["Billing_Type"].ToString().Trim() == "2")//（銀）
                            {
                                Sum_Money[1] += Convert.ToInt32(dr["Total_Num"]) * Convert.ToDecimal(dr["Unit_Price"]);
                                Select_Materiel_Type[1] = true;
                            }
                        }

                        if (dr["Material_Type"].ToString().Trim() == "B")//(寄卡單)
                        {
                            if (dr["Billing_Type"].ToString().Trim() == "1")//（卡）
                            {
                                Sum_Money[2] += Convert.ToInt32(dr["Total_Num"]) * Convert.ToDecimal(dr["Unit_Price"]);
                                Select_Materiel_Type[2] = true;
                            }
                            else if (dr["Billing_Type"].ToString().Trim() == "2")//（銀）
                            {
                                Sum_Money[3] += Convert.ToInt32(dr["Total_Num"]) * Convert.ToDecimal(dr["Unit_Price"]);
                                Select_Materiel_Type[3] = true;
                            }
                        }

                        if (dr["Material_Type"].ToString().Trim() == "C")//(DM)
                        {
                            if (dr["Billing_Type"].ToString().Trim() == "1")//（卡）
                            {
                                Sum_Money[4] += Convert.ToInt32(dr["Total_Num"]) * Convert.ToDecimal(dr["Unit_Price"]);
                                Select_Materiel_Type[4] = true;
                            }
                            else if (dr["Billing_Type"].ToString().Trim() == "2")//（銀）
                            {
                                Sum_Money[5] += Convert.ToInt32(dr["Total_Num"]) * Convert.ToDecimal(dr["Unit_Price"]);
                                Select_Materiel_Type[5] = true;
                            }
                        }
                    }
                }
            }

            if (Select_Materiel_Type[0])
            {
                DataRow dr1 = dtRequisition_Money.NewRow();
                dr1["Materiel_Type"] = "3";
                dr1["Materiel_Type_Name"] = "信封(卡)";
                dr1["Sum_Money"] = Sum_Money[0];
                //dr1["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                dtRequisition_Money.Rows.Add(dr1);
            }

            if (Select_Materiel_Type[1])
            {
                DataRow dr2 = dtRequisition_Money.NewRow();
                dr2["Materiel_Type"] = "4";
                dr2["Materiel_Type_Name"] = "信封(銀)";
                dr2["Sum_Money"] = Sum_Money[1];
                //dr2["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                dtRequisition_Money.Rows.Add(dr2);
            }

            if (Select_Materiel_Type[2])
            {
                DataRow dr3 = dtRequisition_Money.NewRow();
                dr3["Materiel_Type"] = "1";
                dr3["Materiel_Type_Name"] = "寄卡單(卡)";
                dr3["Sum_Money"] = Sum_Money[2];
                //dr3["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                dtRequisition_Money.Rows.Add(dr3);
            }

            if (Select_Materiel_Type[3])
            {
                DataRow dr4 = dtRequisition_Money.NewRow();
                dr4["Materiel_Type"] = "2";
                dr4["Materiel_Type_Name"] = "寄卡單(銀)";
                dr4["Sum_Money"] = Sum_Money[3];
                //dr4["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                dtRequisition_Money.Rows.Add(dr4);
            }

            if (Select_Materiel_Type[4])
            {
                DataRow dr5 = dtRequisition_Money.NewRow();
                dr5["Materiel_Type"] = "5";
                dr5["Materiel_Type_Name"] = "DM(卡)";
                dr5["Sum_Money"] = Sum_Money[4];
                //dr5["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                dtRequisition_Money.Rows.Add(dr5);
            }

            if (Select_Materiel_Type[5])
            {
                DataRow dr6 = dtRequisition_Money.NewRow();
                dr6["Materiel_Type"] = "6";
                dr6["Materiel_Type_Name"] = "DM(銀)";
                dr6["Sum_Money"] = Sum_Money[5];
                //dr6["Billing_Type"] = dr["Billing_Type"].ToString().Trim();
                dtRequisition_Money.Rows.Add(dr6);
            }

            return dtRequisition_Money;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dtRequisition_Money;
    }

    /// <summary>
    /// 取系統中全部SAP單號
    /// </summary>
    public DataTable getAllSAP()
    {
        DataTable dtSAP = null;
        try
        {
            DataSet dsCON_SAP = dao.GetList(SEL_ALL_SAP, dirValues);
            if (null != dsCON_SAP && dsCON_SAP.Tables.Count > 0 &&
                dsCON_SAP.Tables[0].Rows.Count > 0)
            {
                dtSAP = dsCON_SAP.Tables[0];
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dtSAP;
    }

    /// <summary>
    /// 如果請款中包含郵資費，計算郵資費剩余金額是否足夠
    /// </summary>
    public bool CheckPostCostNew(decimal dAskMoney, 
                    string Material_Type,
                    out string strAlert)
    {
        strAlert = "";
        string strMsg = "";
        try
        {
            // 郵資費(卡)
            if (Material_Type == "7")
            {
                strMsg = "郵資費（卡）";
            }else if (Material_Type == "8")
            {
                strMsg = "郵資費（銀）";
            }

            // 郵資費年度預算
            this.dirValues.Clear();
            this.dirValues.Add("material_type", Material_Type);
            DataSet dsMaterial_Buget = dao.GetList(SEL_MATERIEL_BUDGET, this.dirValues);
            decimal dcmBuget = 0;
            if (dsMaterial_Buget != null && dsMaterial_Buget.Tables.Count > 0 &&
                dsMaterial_Buget.Tables[0].Rows.Count > 0)
            {
                dcmBuget = Convert.ToDecimal(dsMaterial_Buget.Tables[0].Rows[0]["Budget"].ToString());
            }

            if (dcmBuget == 0)
            {
                strAlert = strMsg+"預算剩余金額不足。";
                //string[] arg = new string[1];
                //arg[0] = strMsg + "年度預算剩余金額不足。";
                //Warning.SetWarning(GlobalString.WarningType.MatrrielSapAskMoney, arg);
                return false;
            }

            // 郵資費用年度已經耗用
            decimal dcmBuget_Used = 0;
            DataSet dsMaterial_Buget_Used = dao.GetList(SEL_MATERIEL_SAP_SUM, this.dirValues);
            if (dsMaterial_Buget_Used != null && dsMaterial_Buget_Used.Tables.Count > 0 &&
                dsMaterial_Buget_Used.Tables[0].Rows.Count > 0)
            {
                dcmBuget_Used = Convert.ToDecimal(dsMaterial_Buget_Used.Tables[0].Rows[0][0].ToString());
            }

            // 郵資費用剩余金額
            Decimal dcmlMaterial_Buget_Surplus = dcmBuget - dcmBuget_Used - dAskMoney;
            if (dcmlMaterial_Buget_Surplus < 0)
            {
                //string[] arg = new string[1];
                //arg[0] = strMsg + "年度預算剩余金額不足。";
                //Warning.SetWarning(GlobalString.WarningType.MatrrielSapAskMoney, arg);

                strAlert = strMsg + "預算剩余金額不足。";
                return false;
            }
            else
            {
                // 郵資費用剩余金額不足10%
                if ((dcmlMaterial_Buget_Surplus / dcmBuget) < decimal.Parse("0.1"))
                {
                    //警讯
                    string[] arg = new string[1];
                    arg[0] = strMsg;
                    Warning.SetWarning(GlobalString.WarningType.MatrrielSapAskMoney, arg);
                    strAlert = strMsg + "年度預算剩餘金額低於10%。";
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 檢查當天是否有同類別的請款
    /// </summary>
    public string CheckAskMoney(string Material_Type,
                    DateTime dtAakMoneyDate)
    {
        string strRet = "";

        try{
            dirValues.Clear();
            dirValues.Add("materiel_type", Material_Type);
            dirValues.Add("ask_date", dtAakMoneyDate.ToString("yyyy/MM/dd"));
            DataSet dsMATERIEL_SAP_SAME_MATERIAL_TYPE = dao.GetList(CON_MATERIEL_SAP_SAME_MATERIAL_TYPE, dirValues);
            if (null != dsMATERIEL_SAP_SAME_MATERIAL_TYPE &&
                    dsMATERIEL_SAP_SAME_MATERIAL_TYPE.Tables.Count > 0 &&
                    dsMATERIEL_SAP_SAME_MATERIAL_TYPE.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt16(dsMATERIEL_SAP_SAME_MATERIAL_TYPE.Tables[0].Rows[0][0].ToString()) > 0)
                {
                    switch (Material_Type)
                    {
                        case "1":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入寄卡單（卡）類別的請款資料";
                            break;
                        case "2":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入寄卡單（銀）類別的請款資料";
                            break;
                        case "3":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入信封（卡）類別的請款資料";
                            break;
                        case "4":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入信封（銀）類別的請款資料";
                            break;
                        case "5":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入DM（卡）類別的請款資料";
                            break;
                        case "6":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入DM（銀）類別的請款資料";
                            break;
                        case "7":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入郵資費（卡）類別的請款資料";
                            break;
                        case "8":
                            strRet = dtAakMoneyDate.ToString("yyyy/MM/dd") + "已有輸入郵資費（銀）類別的請款資料";
                            break;
                        default:
                            break;
                    }
                }
            }
        }catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        
        return strRet;
    }

    /// <summary>
    /// 保存請款訊息
    /// </summary>
    /// <param name="dtRequisition_Money">請款訊息</param>
    /// <param name="dtSearchMaterialPurchase">物料採購明細</param>
    public void Save(DataTable dtRequisition_Money, DataTable dtSearchMaterialPurchase)
    {
        try
        {
            dao.OpenConnection();
            MATERIEL_SAP msModel = new MATERIEL_SAP();
            foreach (DataRow drAskMoney in dtRequisition_Money.Rows)
            {
                // 添加物料請款SAP單
                msModel.Ask_Date = Convert.ToDateTime(drAskMoney["Ask_Date"]);
                msModel.Materiel_Type = drAskMoney["Materiel_Type"].ToString();
                msModel.Pay_Date = Convert.ToDateTime(drAskMoney["Pay_Date"]);
                msModel.SAP_ID = drAskMoney["SAP_ID"].ToString();
                msModel.Sum = Convert.ToDecimal(drAskMoney["Sum_Money"]);
                dao.Add<MATERIEL_SAP>(msModel, "RID");

                // 更新物料採購明細的請款訊息
                string Material_Type = "";
                string Billing_Type = "";
                if (msModel.Materiel_Type == "1")
                {
                    Material_Type = "B";
                    Billing_Type = "1";
                }
                else if (msModel.Materiel_Type == "2"){
                    Material_Type = "B";
                    Billing_Type = "2";
                }
                else if (msModel.Materiel_Type == "3")
                {
                    Material_Type = "A";
                    Billing_Type = "1";
                }
                else if (msModel.Materiel_Type == "4")
                {
                    Material_Type = "A";
                    Billing_Type = "2";
                }
                else if (msModel.Materiel_Type == "5")
                {
                    Material_Type = "C";
                    Billing_Type = "1";
                }
                else if (msModel.Materiel_Type == "6")
                {
                    Material_Type = "C";
                    Billing_Type = "2";
                }
                if (Material_Type != "")
                {
                    foreach (DataRow drPURCHASE in dtSearchMaterialPurchase.Rows)
                    {
                        if (Convert.ToBoolean(drPURCHASE["選中"]) &&
                            drPURCHASE["Material_Type"].ToString() == Material_Type &&
                            drPURCHASE["Billing_Type"].ToString() == Billing_Type)
                        {
                            dirValues.Clear();
                            dirValues.Add("sap_serial_number", msModel.SAP_ID);
                            dirValues.Add("ask_date", msModel.Ask_Date);
                            dirValues.Add("pay_date", msModel.Pay_Date);
                            dirValues.Add("purchaseorder_RID", drPURCHASE["PurchaseOrder_RID"].ToString());
                            dirValues.Add("detail_rid", Convert.ToInt32(drPURCHASE["Detail_RID"]));
                            dao.ExecuteNonQuery(UPDATE_MATERIEL_PURCHASE_FORM, dirValues);
                        }
                    }
                }
            }

            SetOprLog("2");
            dao.Commit();
        }
        catch (AlertException ex)
        {
            //事務回滾
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }


    /// <summary>
    /// 取物料及郵資費用請款SAP單訊息
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet getMaterial_SAP(string strRID, out DataSet dsMATERIEL_PURCHASE_FORM_EDIT)
    {
        DataSet dsMATERIEL_SAP = null;
        dsMATERIEL_PURCHASE_FORM_EDIT = null;
        try
        {
            dsMATERIEL_SAP = dao.GetList(SEL_MATERIEL_SAP + " and RID =  '" + strRID + "' ");

            if (dsMATERIEL_SAP.Tables[0].Rows.Count != 0)
            {
                if (dsMATERIEL_SAP.Tables[0].Rows[0]["Materiel_Type"].ToString() == "7" || dsMATERIEL_SAP.Tables[0].Rows[0]["Materiel_Type"].ToString() == "8")
                {
                }
                else
                {
                    dsMATERIEL_PURCHASE_FORM_EDIT = dao.GetList(SEL_MATERIEL_PURCHASE_FORM_EDIT + " AND MPF.SAP_Serial_Number =  '" + dsMATERIEL_SAP.Tables[0].Rows[0]["SAP_ID"].ToString() + "'");
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return dsMATERIEL_SAP;
    }

    /// <summary>
    /// 檢查新的SAP單號和已經存在SAP單號是否重復
    /// </summary>
    public bool GetCon_SAP(string strRID, string SAP_Serial_Number)
    {
        dirValues.Clear();
        dirValues.Add("RID", strRID);
        dirValues.Add("SAP", SAP_Serial_Number);
        DataSet dsCON_SAP = dao.GetList(Con_SAP, dirValues);
        if (Convert.ToInt16(dsCON_SAP.Tables[0].Rows[0][0]) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 如果請款中包含郵資費，計算郵資費剩余金額是否足夠
    /// </summary>
    public bool CheckPostCostUpdate(string strRID, decimal decPay_Money, string strMaterial_Type, out string strAlert, bool IsPay)
    {
        bool IsSurplus_Suffice = false;
        strAlert = "";
        string strMsg = "";
        try
        {
            dao.OpenConnection();
            // 郵資費(卡)
            if (strMaterial_Type == "7")
            {
                strMsg = "郵資費（卡）";
            }
            else if (strMaterial_Type == "8")
            {
                strMsg = "郵資費（銀）";
            }

            //取郵資費的年度預算。
            if (IsPay == true)
            {
                dirValues.Clear();
                dirValues.Add("material_type", strMaterial_Type);
                dirValues.Add("rid", strRID);
                DataSet dsMATERIEL_BUDGET_UPDATE = dao.GetList(SEL_MATERIEL_BUDGET_UPDATE, dirValues);
                //取郵資費的年度耗用。
                DataSet dsMATERIEL_SAP_SUM_UPDATE = dao.GetList(SEL_MATERIEL_SAP_SUM_UPDATE, dirValues);

                //年度郵資費預算剩余金額 
                decimal decSurplus_Card = 0;
                if (dsMATERIEL_BUDGET_UPDATE.Tables[0].Rows.Count != 0 && dsMATERIEL_SAP_SUM_UPDATE.Tables[0].Rows.Count != 0)
                {
                    decSurplus_Card = Convert.ToDecimal(dsMATERIEL_BUDGET_UPDATE.Tables[0].Rows[0][0]) - Convert.ToDecimal(dsMATERIEL_SAP_SUM_UPDATE.Tables[0].Rows[0][0]) - decPay_Money;

                    if (dsMATERIEL_BUDGET_UPDATE.Tables[0].Rows[0][0].ToString() != "0")
                    {
                        if (decSurplus_Card < 0)
                        {
                            strAlert = strMsg+"年度預算不足";
                            IsSurplus_Suffice = true;
                        }
                        else if (Convert.ToDouble(decSurplus_Card / Convert.ToDecimal(dsMATERIEL_BUDGET_UPDATE.Tables[0].Rows[0][0])) < 0.1)
                        {
                            //警讯
                            string[] arg = new string[1];
                            arg[0] = strMsg;
                            Warning.SetWarning(GlobalString.WarningType.MatrrielSapAskMoney, arg);
                            strAlert = strMsg+"年度預算剩餘金額低於10%";
                            IsSurplus_Suffice = false;
                        }
                       
                    }
                    else
                    {
                        strAlert = strMsg+"用年度為0";
                        IsSurplus_Suffice = true;
                    }
                }
                else
                {
                    strAlert = strMsg+"用年度為0";
                    IsSurplus_Suffice = true;
                }
            }
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
        return IsSurplus_Suffice;
    }


    /// <summary>
    ///更新請款訊息
    /// </summary>
    public void Update(string strRID, decimal decSum, DateTime Ask_Date, DateTime Pay_Date, string SAP_ID)
    {
        try
        {
            dao.OpenConnection();
            dirValues.Clear();
            dirValues.Add("rid", strRID);
            if (SAP_ID.Trim() == "")
            {
                dirValues.Add("sap_serial_number", " ");
            }
            else
            {
                dirValues.Add("sap_serial_number", SAP_ID);
            }
            
            dirValues.Add("ask_date", Ask_Date);
            if (Pay_Date == Convert.ToDateTime("1900-01-01"))
            {
                dirValues.Add("pay_date", Convert.ToDateTime("1900-01-01"));
            }
            else
            {
                dirValues.Add("pay_date", Pay_Date);
            }

            dirValues.Add("sum", decSum);
            dao.ExecuteNonQuery(Update_MATERIEL_PURCHASE_FORM_UPDATE, dirValues);
            dao.ExecuteNonQuery(UPDATE_MATERIEL_SAP, dirValues);

            SetOprLog("3");

            dao.Commit();
        }
        catch (AlertException ex)
        {
            //事務回滾
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }


    /// <summary>
    ///檢查SAP單的出帳日期是否為"1900-01-01",如果不為“1900-01-01”，不能刪除
    /// </summary>
    public bool Con_Delete(string strRID)
    {
        try
        {
            dao.OpenConnection();
            dirValues.Clear();
            dirValues.Add("rid", strRID);
            DataSet dsCON_DEL_MATERIEL_SAP = dao.GetList(CON_DEL_MATERIEL_SAP, dirValues);
            if (dsCON_DEL_MATERIEL_SAP.Tables[0].Rows.Count != 0)
            {
                if (Convert.ToDateTime(dsCON_DEL_MATERIEL_SAP.Tables[0].Rows[0][0]).ToString("yyyy-MM-dd") != "1900-01-01")
                {
                    return true;
                }
            }
            dao.Commit();
        }
        catch (AlertException ex)
        {
            //事務回滾
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
        return false;
    }


    /// <summary>
    ///刪除
    /// </summary>
    public void Delete(string strRID)
    {
        try
        {
            dao.OpenConnection();
            dirValues.Clear();
            dirValues.Add("rid", strRID);
            dao.ExecuteNonQuery(Update_MATERIEL_PURCHASE_FORM_DEL, dirValues);
            dao.ExecuteNonQuery(DEL_MATERIEL_SAP, dirValues);

            SetOprLog("4");
            dao.Commit();
        }
        catch (AlertException ex)
        {
            //事務回滾
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }
}
