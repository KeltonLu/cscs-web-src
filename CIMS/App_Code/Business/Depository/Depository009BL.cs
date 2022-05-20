//******************************************************************
//*  作    者：lantaosu
//*  功能說明：其他預算控管作業管理邏輯
//*  創建日期：2008-11-11
//*  修改日期：2008-11-20 12:00
//*  修改記錄：
//*            □2008-11-20
//*              1.創建 蘇斕濤
//*******************************************************************
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
/// Depository009BL 的摘要描述
/// </summary>
public class Depository009BL:BaseLogic
{
    #region sql語句
    public const string SEL_MATERIEL_BUDGET = "SELECT RID,Budget_Year,Materiel_Type,Budget"
                                            + " FROM MATERIEL_BUDGET"
                                            + " WHERE RST='A' AND Budget_Year = @budget_year";                                           
    public const string SEL_MATERIEL_SAP = "SELECT ms.Materiel_Type,p.param_name,ISNULL(SUM(Sum),0) as [count]"
                                            + " FROM MATERIEL_SAP ms"
                                            + " join param p"
                                            + " on ms.Materiel_Type=p.param_code"
                                            + " and paramtype_code='MatType' and p.rst='A'"
                                            + " WHERE ms.RST='A' AND YEAR(Ask_Date)=@Ask_year"
                                            + " and p.param_name=@param_name"
                                            + " GROUP BY ms.Materiel_Type,p.param_name";
    public const string SEL_PERSO_PROJECT_SAP_KA = "SELECT ISNULL(SUM(Sum),0) as ct"
                                            + " FROM PERSO_PROJECT_SAP PPJS INNER JOIN CARD_GROUP CG"
                                            + " ON CG.RST = 'A' AND PPJS.Card_Group_RID = CG.RID"
                                            + " WHERE PPJS.RST = 'A' AND YEAR(PPJS.Ask_Date) = @Ask_year"
                                            + " AND (CG.Group_Name='磁條信用卡' OR CG.Group_Name='晶片信用卡' OR CG.Group_Name='VISA DEBIT卡')";
    public const string SEL_PERSO_PROJECT_SAP_YIN = "SELECT ISNULL(SUM(Sum),0) as ct"
                                            + " FROM PERSO_PROJECT_SAP PPJS INNER JOIN CARD_GROUP CG"
                                            + " ON CG.RST = 'A' AND PPJS.Card_Group_RID = CG.RID"
                                            + " WHERE PPJS.RST = 'A' AND YEAR(PPJS.Ask_Date) = @Ask_year"
                                            + " AND (CG.Group_Name='晶片金融卡' OR CG.Group_Name='現金卡')";
    public const string DEL_MATERIEL_BUDGET_BY_YEAR = "Delete from MATERIEL_BUDGET "
                                            + " WHERE RST='A' AND Budget_Year = @budget_year";
    public const string SEL_BY_KEY = "SELECT PARAM_CODE"
                                            + " from PARAM"
                                            + " WHERE PARAM_NAME = @param_name AND ParamType_Code = 'MatType' and rst='A'";
    public const string SEL_PARAM_COUNT = "SELECT SUM(RID)"
                                            + " from PARAM"
                                            + " where ParamType_Code = 'MatType' and rst='A'";

    public const string SEL_Budget_By_Materiel = "SELECT mb.Materiel_Type,p.param_name,mb.Budget,ISNULL(used.[count],0) as used,ISNULL(budget-ISNULL(used.[count],0),0) as RemainBudget,mb.Budget_Year"
                                            + " FROM MATERIEL_BUDGET mb"
                                            + " join param p on mb.Materiel_Type=p.param_code"
                                            + " and paramtype_code='MatType' and p.rst='A' left join (SELECT ms.Materiel_Type,p.param_name,ISNULL(SUM(Sum),0) as [count]"
                                            + " FROM MATERIEL_SAP ms join param p"
                                            + " on ms.Materiel_Type=p.param_code and paramtype_code='MatType' " + " and p.rst='A' WHERE ms.RST='A' "
                                            + " GROUP BY ms.Materiel_Type,p.param_name) as used"
                                            + " on used.Materiel_Type=mb.Materiel_Type"
                                            + " where mb.RST='A' AND mb.Budget_Year = @Budget_Year";

    public const string SEL_Budget_By_Year = "SELECT mb.Materiel_Type,p.param_name,mb.Budget,ISNULL(used.[count],0) as used,ISNULL(budget-ISNULL(used.[count],0),0) as RemainBudget,mb.Budget_Year"
                                            + " FROM MATERIEL_BUDGET mb"
                                            + " join param p on mb.Materiel_Type=p.param_code"
                                            + " and paramtype_code='MatType' and p.rst='A' and mb.Materiel_Type<>'9' and mb.Materiel_Type<>'10' left join (SELECT ms.Materiel_Type,p.param_name,ISNULL(SUM(Sum),0) as [count]"
                                            + " FROM MATERIEL_SAP ms join param p"
                                            + " on ms.Materiel_Type=p.param_code and paramtype_code='MatType' "                                               + " and p.rst='A' WHERE ms.RST='A' "
                                            + " GROUP BY ms.Materiel_Type,p.param_name) as used"
                                            + " on used.Materiel_Type=mb.Materiel_Type"
                                            + " where mb.RST='A' AND mb.Budget_Year = @Budget_Year";

    public const string SEL_MATERIEL_ORDER = "SELECT Serial_Number,Number1,Number2,Number3,Number4,Number5,Total_Price "
        + "From MATERIEL_PURCHASE_FORM "
        + "Where RST = 'A' AND YEAR(Purchase_Date) = @budget_year";	
    public const string SEL_ENVELOPE = "SELECT  Billing_Type "
        + "FROM ENVELOPE_INFO "
        + "WHERE RST= 'A'  AND Serial_Number = @Serial_Number";
    public const string SEL_EXPONENT = "SELECT  Billing_Type "
        + "FROM CARD_EXPONENT "
        + "WHERE RST= 'A'  AND Serial_Number = @Serial_Number";
    public const string SEL_DM = "SELECT  Billing_Type "
        + "FROM DMTYPE_INFO "
        + "WHERE RST= 'A'  AND Serial_Number = @Serial_Number";
    public const string SEL_POST_SAP_KA = "SELECT ISNULL(SUM(Sum),0) as ct "
        + "from MATERIEL_SAP "
        + "where RST = 'A' AND Materiel_Type = '"+GlobalString.MaterielType.POSTAGE_CARD+"' AND YEAR(Ask_Date) = @Year";
    public const string SEL_POST_SAP_YIN = "SELECT ISNULL(SUM(Sum),0) as ct "
        + "from MATERIEL_SAP "
        + "where RST = 'A' AND Materiel_Type = '" + GlobalString.MaterielType.POSTAGE_BANK + "' AND YEAR(Ask_Date) = @Year";


    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    BaseInfo006BL biManager = new BaseInfo006BL();

    public Depository009BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢項目資料列表
    /// </summary>
    /// <param name="searchInput"></param>
    /// <returns></returns>
    public DataSet List(Dictionary<string, object> searchInput)
    {
        decimal[] MUsed = new decimal[6];
        MUsed = MaterielUsed(searchInput["txtBeginTime"].ToString());

        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        ds.Tables.Add(dt);

        DataColumn dc = new DataColumn();
        dc.ColumnName = "Materiel_Type";
        ds.Tables[0].Columns.Add(dc);
        DataColumn dc1 = new DataColumn();
        dc1.ColumnName = "param_name";
        ds.Tables[0].Columns.Add(dc1);
        DataColumn dc2 = new DataColumn();
        dc2.ColumnName = "Budget";
        ds.Tables[0].Columns.Add(dc2);
        DataColumn dc3 = new DataColumn();
        dc3.ColumnName = "used";
        ds.Tables[0].Columns.Add(dc3);
        DataColumn dc4 = new DataColumn();
        dc4.ColumnName = "RemainBudget";
        ds.Tables[0].Columns.Add(dc4);
        DataColumn dc5 = new DataColumn();
        dc5.ColumnName = "Budget_Year";
        ds.Tables[0].Columns.Add(dc5);

        //寄卡單（卡）
        DataRow dr1 = ds.Tables[0].NewRow();
        dr1["Materiel_Type"] = "1";
        dr1["param_name"] = "寄卡單(卡)";
        dr1["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "1");
        dr1["used"] = MUsed[0];
        dr1["RemainBudget"] = Convert.ToDouble(dr1["Budget"]) - Convert.ToDouble(dr1["used"]);
        dr1["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr1);

        //寄卡單（銀）
        DataRow dr2 = ds.Tables[0].NewRow();
        dr2["Materiel_Type"] = "2";
        dr2["param_name"] = "寄卡單(銀)";
        dr2["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "2");
        dr2["used"] = MUsed[1];
        dr2["RemainBudget"] = Convert.ToDouble(dr2["Budget"]) - Convert.ToDouble(dr2["used"]);
        dr2["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr2);

        //信封（卡）
        DataRow dr3 = ds.Tables[0].NewRow();
        dr3["Materiel_Type"] = "3";
        dr3["param_name"] = "信封(卡)";
        dr3["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "3");
        dr3["used"] = MUsed[2];
        dr3["RemainBudget"] = Convert.ToDouble(dr3["Budget"]) - Convert.ToDouble(dr3["used"]);
        dr3["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr3);

        //信封（銀）
        DataRow dr4 = ds.Tables[0].NewRow();
        dr4["Materiel_Type"] = "4";
        dr4["param_name"] = "信封(銀)";
        dr4["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "4");
        dr4["used"] = MUsed[3];
        dr4["RemainBudget"] = Convert.ToDouble(dr4["Budget"]) - Convert.ToDouble(dr4["used"]);
        dr4["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr4);

        //DM（卡）
        DataRow dr5 = ds.Tables[0].NewRow();
        dr5["Materiel_Type"] = "5";
        dr5["param_name"] = "DM(卡)";
        dr5["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "5");
        dr5["used"] = MUsed[4];
        dr5["RemainBudget"] = Convert.ToDouble(dr5["Budget"]) - Convert.ToDouble(dr5["used"]);
        dr5["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr5);

        //DM（銀）
        DataRow dr6 = ds.Tables[0].NewRow();
        dr6["Materiel_Type"] = "6";
        dr6["param_name"] = "DM(銀)";
        dr6["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "6");
        dr6["used"] = MUsed[5];
        dr6["RemainBudget"] = Convert.ToDouble(dr6["Budget"]) - Convert.ToDouble(dr6["used"]);
        dr6["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr6);

        //郵資費（卡）
        DataRow dr7 = ds.Tables[0].NewRow();
        dr7["Materiel_Type"] = "7";
        dr7["param_name"] = "郵資費(卡)";
        dr7["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "7");
        string i = GetPSK(searchInput["txtBeginTime"].ToString().Trim());
        dr7["used"] = i ;
        dr7["RemainBudget"] = Convert.ToDouble(dr7["Budget"]) - Convert.ToDouble(dr7["used"]);
        dr7["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr7);

        //郵資費（銀）
        DataRow dr8 = ds.Tables[0].NewRow();
        dr8["Materiel_Type"] = "8";
        dr8["param_name"] = "郵資費(銀)";
        dr8["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "8");
        i = GetPSY(searchInput["txtBeginTime"].ToString().Trim());
        dr8["used"] = i;
        dr8["RemainBudget"] = Convert.ToDouble(dr8["Budget"]) - Convert.ToDouble(dr8["used"]);
        dr8["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr8);

        //代製費用（卡）
        DataRow dr9 = ds.Tables[0].NewRow();
        dr9["Materiel_Type"] = "9";
        dr9["param_name"] = "代製費用(卡)";
        dr9["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "9");
        i = GetPPSK(searchInput["txtBeginTime"].ToString().Trim());
        dr9["used"] = i;
        dr9["RemainBudget"] = Convert.ToDouble(dr9["Budget"]) - Convert.ToDouble(dr9["used"]);
        dr9["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr9);

        //代製費用（銀）
        DataRow dr10 = ds.Tables[0].NewRow();
        dr10["Materiel_Type"] = "10";
        dr10["param_name"] = "代製費用(銀)";
        dr10["Budget"] = GetBudgetByType(searchInput["txtBeginTime"].ToString().Trim(), "10");
        i = GetPPSY(searchInput["txtBeginTime"].ToString().Trim());
        dr10["used"] = i;
        dr10["RemainBudget"] = Convert.ToDouble(dr10["Budget"]) - Convert.ToDouble(dr10["used"]);
        dr10["Budget_Year"] = searchInput["txtBeginTime"].ToString().Trim();
        ds.Tables[0].Rows.Add(dr10);

        return ds;
       
    }

    /// <summary>
    /// 根據物料種類查詢特定年度預算
    /// </summary>
    /// <param name="strMateriel_Type"></param>
    /// <param name="strYear"></param>
    /// <returns></returns>
    public DataSet GetBudgetByRid(string strMateriel_Type, string strYear)
    {
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        dirValues.Add("Budget_Year", strYear);
        stbWhere.Append(" and mb.Materiel_Type = @Materiel_Type");
        dirValues.Add("Materiel_Type", strMateriel_Type);

        DataSet dstOF = dao.GetList(SEL_Budget_By_Materiel + stbWhere, dirValues);

        if (dstOF != null && dstOF.Tables[0].Rows.Count > 0)
        {
            return dstOF;
        }
        else if (strMateriel_Type == "5")
        {
            DataRow dr = dstOF.Tables[0].NewRow();
            dr["Materiel_Type"] = "5";
            dr["param_name"] = "代製費用(卡)";
            dr["Budget"] = GetBudgetByType(strYear, "代製費用(卡)");
            dr["used"] = GetPPSK(strYear);//代製費用(卡)
            dr["RemainBudget"] = Convert.ToDouble(dr["Budget"]) - Convert.ToDouble(dr["used"]);
            dr["Budget_Year"] = strYear;
            dstOF.Tables[0].Rows.Add(dr);

            return dstOF;
        }
        else if (strMateriel_Type == "6")
        {
            DataRow drw = dstOF.Tables[0].NewRow();
            drw["Materiel_Type"] = "6";
            drw["param_name"] = "代製費用(銀)";
            drw["Budget"] = GetBudgetByType(strYear, "代製費用(銀)");
            drw["used"] = GetPPSY(strYear);//代製費用(銀)
            drw["RemainBudget"] = Convert.ToDouble(drw["Budget"]) - Convert.ToDouble(drw["used"]);
            drw["Budget_Year"] = strYear;
            dstOF.Tables[0].Rows.Add(drw);

            return dstOF;
        }

        return dstOF;

    }

    /// <summary>
    /// 根據物料種類名稱獲得對應參數類型編號
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
    public string GetMateriel_Type(string strName)
    {
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        stbWhere.Append(" and PARAM_NAME =@param_name");
        dirValues.Add("param_name", strName);
        DataSet ds = dao.GetList(SEL_BY_KEY, dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["PARAM_CODE"].ToString();
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 獲得物料種類的數量
    /// </summary>
    /// <returns></returns>
    public int GetParamCount()
    {
        DataSet ds = dao.GetList(SEL_PARAM_COUNT);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 根據年度和物料種類獲得對應的預算
    /// </summary>
    /// <param name="strType">物料種類</param>
    /// <returns>預算</returns>
    public string GetBudgetByType(string strYear,string strType)
    {
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();        
        dirValues.Add("budget_year", strYear);
        stbWhere.Append(" and Materiel_Type =@type");
        dirValues.Add("type", strType);
        DataSet ds = dao.GetList(SEL_MATERIEL_BUDGET+stbWhere.ToString(),dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["Budget"].ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 物料年度已耗用
    /// </summary>
    /// <param name="strYear"></param>
    /// <returns></returns>
    public decimal[] MaterielUsed(string strYear)
    {
        decimal exponentKa = 0.00M;
        decimal exponentYin = 0.00M;
        decimal envelopeKa = 0.00M;
        decimal envelopeYin = 0.00M;
        decimal dmKa = 0.00M;
        decimal dmYin = 0.00M;
        string billingType = "";
        decimal[] MaterielUsed = new decimal[6];
        

        //查詢該年內的所有採購下單
        dirValues.Clear();        
        dirValues.Add("budget_year", strYear);
        DataSet ds = dao.GetList(SEL_MATERIEL_ORDER, dirValues);

        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            if (dr["Serial_Number"].ToString().Substring(0, 1) == "A")
            {
                //查詢信封信息
                dirValues.Clear();
                dirValues.Add("Serial_Number", dr["Serial_Number"].ToString());
                DataSet dsEnvelope = dao.GetList(SEL_ENVELOPE, dirValues);
                if (dsEnvelope.Tables[0].Rows.Count != 0)
                {
                    billingType = dsEnvelope.Tables[0].Rows[0]["Billing_Type"].ToString();
                    if (billingType == "1")
                        envelopeKa = envelopeKa + Convert.ToDecimal(dr["Total_Price"].ToString());
                    else if (billingType == "2")
                        envelopeYin = envelopeYin + Convert.ToDecimal(dr["Total_Price"].ToString());
                }
            }
            if (dr["Serial_Number"].ToString().Substring(0, 1) == "B")
            {
                //查詢寄卡單信息
                dirValues.Clear();
                dirValues.Add("Serial_Number", dr["Serial_Number"].ToString());
                DataSet dsExponent = dao.GetList(SEL_EXPONENT, dirValues);
                if (dsExponent.Tables[0].Rows.Count != 0)
                {
                    billingType = dsExponent.Tables[0].Rows[0]["Billing_Type"].ToString();
                    if (billingType == "1")
                        exponentKa = exponentKa + Convert.ToDecimal(dr["Total_Price"].ToString());
                    else if (billingType == "2")
                        exponentYin = exponentYin + Convert.ToDecimal(dr["Total_Price"].ToString());
                }
            }
            if (dr["Serial_Number"].ToString().Substring(0, 1) == "C")
            {
                //查詢DM信息
                dirValues.Clear();
                dirValues.Add("Serial_Number", dr["Serial_Number"].ToString());
                DataSet dsDM = dao.GetList(SEL_DM, dirValues);
                if (dsDM.Tables[0].Rows.Count != 0)
                {
                    billingType = dsDM.Tables[0].Rows[0]["Billing_Type"].ToString();
                    if (billingType == "1")
                        dmKa = dmKa + Convert.ToDecimal(dr["Total_Price"].ToString());
                    else if (billingType == "2")
                        dmYin = dmYin + Convert.ToDecimal(dr["Total_Price"].ToString());
                }
            }
        }

        MaterielUsed[0] = Math.Round(exponentKa, 2);
        MaterielUsed[1] = Math.Round(exponentYin, 2);
        MaterielUsed[2] = Math.Round(envelopeKa, 2);
        MaterielUsed[3] = Math.Round(envelopeYin, 2);
        MaterielUsed[4] = Math.Round(dmKa, 2);
        MaterielUsed[5] = Math.Round(dmYin, 2);
        return MaterielUsed;
    }

    /// <summary>
    /// 根據年度和物料種類獲得對應的請款已耗用
    /// </summary>
    /// <param name="strYear"></param>
    /// <param name="strType"></param>
    /// <returns></returns>
    public string GetUsedByType(string strYear, string strType)
    {
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        stbWhere.Append("and YEAR(Ask_Date) =@Ask_year");
        dirValues.Add("Ask_year", strYear);
        stbWhere.Append(" and p.param_name =@param_name");
        dirValues.Add("param_name", strType);
        DataSet ds = dao.GetList(SEL_MATERIEL_SAP, dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["count"].ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 郵資費(卡)年度已耗用
    /// </summary>
    /// <returns></returns>
    public string GetPSK(string year)
    {
        dirValues.Clear();
        dirValues.Add("Year", year);
        
        DataSet ds = dao.GetList(SEL_POST_SAP_KA,dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 郵資費(銀)年度已耗用
    /// </summary>
    /// <returns></returns>
    public string GetPSY(string year)
    {
        dirValues.Clear();
        dirValues.Add("Year", year);

        DataSet ds = dao.GetList(SEL_POST_SAP_YIN,dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 根據年度和物料種類獲得對應的請款已耗用--代製費用（卡）
    /// </summary>
    /// <param name="strYear"></param>    
    /// <returns></returns>
    public string GetPPSK(string strYear)
    {
        dirValues.Clear();
        dirValues.Add("Ask_year", strYear);
        DataSet ds = dao.GetList(SEL_PERSO_PROJECT_SAP_KA, dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["ct"].ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// 根據年度和物料種類獲得對應的請款已耗用--代製費用（銀）
    /// </summary>
    /// <param name="strYear"></param>    
    /// <returns></returns>
    public string GetPPSY(string strYear)
    {
        dirValues.Clear();        
        dirValues.Add("Ask_year", strYear);
        DataSet ds = dao.GetList(SEL_PERSO_PROJECT_SAP_YIN, dirValues);
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["ct"].ToString();
        }
        else
        {
            return "0";
        }
    }
       
    /// <summary>
    /// 保存更新操作
    /// </summary>
    /// <param name="dtMB">物料預算記錄集合</param>
    public void Save(DataTable dtMB)
    {
        if (dtMB != null && dtMB.Rows.Count > 0)
        {
            dao.OpenConnection();

            try
            {
                //added by Even.Cheng on 20090108
                //判斷是新增還是修改作業，以便記錄系統使用日誌
                string strOpr = "2";
                dirValues.Clear();
                dirValues.Add("budget_year", dtMB.Rows[0]["Budget_Year"].ToString().Trim());
                DataSet ds = dao.GetList(SEL_MATERIEL_BUDGET, dirValues);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    strOpr = "3";
                }
                //end add

                StringBuilder stbWhere = new StringBuilder();
                stbWhere.Append(" and Budget_Year =@budget_year");
                dao.ExecuteNonQuery(DEL_MATERIEL_BUDGET_BY_YEAR, dirValues);

                for (int i = 0; i < dtMB.Rows.Count; i++)
                {
                    Materiel_Budget mb = new Materiel_Budget();
                    mb.Budget_Year = dtMB.Rows[i]["Budget_Year"].ToString();
                    mb.Materiel_Type = dtMB.Rows[i]["Materiel_Type"].ToString();
                    if (dtMB.Rows[i]["Budget"].ToString() != "")
                    {
                        mb.Budget = Convert.ToInt32(dtMB.Rows[i]["Budget"].ToString());
                    }
                    dao.Add<Materiel_Budget>(mb, "RID");
                }

                SetOprLog(strOpr);
                dao.Commit();
            }
            catch (Exception ex)
            {
                dao.Rollback();
                ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
                throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
            }
            finally
            {
                dao.CloseConnection();
            }
        }
    }
}
