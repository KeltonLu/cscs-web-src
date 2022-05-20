using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using ControlLibrary;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;


/// <summary>
/// BaseWarning 的摘要描述
/// </summary>
public class BaseWarning : BaseLogic
{
    public const string SEL_USERS_BY_RID = "select WU.USERID,UR.USERNAME,UR.EMAIL from WARNING_USER WU inner join UseRS UR on WU.USERID=UR.USERID WHERE WU.WARNING_RID=@WARNING_RID";

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseWarning()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }


    private string GetUser()
    {
        return ((USERS)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER]).UserID + "(" + (((USERS)HttpContext.Current.Session[GlobalString.SessionAndCookieKeys.USER]).UserName) + ")";
    }


    /// <summary>
    /// 獲取需要警訊的用戶并添加
    /// </summary>
    /// <param name="strWarningRID"></param>
    /// <returns></returns>
    private void GetWarningUser(WARNING_CONFIGURATION wcModel, string strWarningContent)
    {
        try
        {

            dirValues.Clear();
            dirValues.Add("WARNING_RID", wcModel.RID);
            DataTable dtblUser = dao.GetList(SEL_USERS_BY_RID, dirValues).Tables[0];


            if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
                return;

            string strMailTitle = ConfigurationManager.AppSettings["MailTitle"].ToString() + wcModel.Item_Name;

            foreach (DataRow drowUser in dtblUser.Rows)
            {
                /*2009-06-24 modify by huangping 
                 * 【每日批次作業時】RID=15
                 * 【每月批次作業時】RID=29
                 * 【物料低於安全庫存(自動或人工匯入)】RID=14
                 * add by Ian Huang start
                 * 【物料控管作業】RID=13
                 * add by Ian Huang end
                 * 每日只發送一次警訊
                 * 
                */
                if (wcModel.RID == 15 || wcModel.RID == 14 || wcModel.RID == 29 || wcModel.RID == 13)
                {
                    strWarningContent = strWarningContent.Replace("\\n", "\n");

                    DataSet ds = dao.GetList("select wi.RID from WARNING_INFO wi,WARNING_CONFIGURATION wc where wc.RID="
                    + wcModel.RID + " and wi.Warning_Content='" + strWarningContent + "' and wi.UserID='" + drowUser["userid"].ToString()
                    +"' and convert(char,wi.Warning_Date,111)=convert(char,getdate(),111)");

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return;
                    }
                    else
                    {
                        WARNING_INFO wiModel = new WARNING_INFO();
                        wiModel.Warning_RID = wcModel.RID;
                        wiModel.Warning_Content = strWarningContent.Replace("\\n", "\n");
                        wiModel.UserID = drowUser["userid"].ToString();
                        wiModel.Is_Show = wcModel.System_Show;
                        dao.Add<WARNING_INFO>(wiModel, "RID");
                    }
                }
                else
                {
                    if (wcModel.System_Show == "Y")
                    {
                        WARNING_INFO wiModel = new WARNING_INFO();
                        wiModel.Warning_RID = wcModel.RID;
                        wiModel.Warning_Content = strWarningContent.Replace("\\n", "\n");
                        wiModel.UserID = drowUser["userid"].ToString();
                        wiModel.Is_Show = wcModel.System_Show;
                        dao.Add<WARNING_INFO>(wiModel, "RID");
                    }
                }  

                if (wcModel.Mail_Show == "Y")
                {
                    USERS uModel = dao.GetModel<USERS, string>("UserID", drowUser["userid"].ToString());
                    if (uModel != null)
                    {
                        MailSend.SendMail(uModel.Email, strMailTitle, strWarningContent);
                    }
                }
            }
        }
        catch
        {
        }

    }

    private PARAM GetParamModel(string strParamCode)
    {
        PARAM pModel = new PARAM();
        try
        {
            dirValues.Clear();
            dirValues.Add("param_code", strParamCode);
            pModel = dao.GetModel<PARAM>("select param_name from param where param_code=@param_code and paramType_code='" + GlobalString.ParameterType.CardParam + "'", dirValues);

        }
        catch { }

        return pModel;
    }

    private WARNING_CONFIGURATION GetModel(string strRID)
    {

        WARNING_CONFIGURATION wcModel = new WARNING_CONFIGURATION();

        try
        {
            wcModel = dao.GetModel<WARNING_CONFIGURATION, int>("RID", int.Parse(strRID));

        }
        catch { }

        return wcModel;
    }



    /// <summary>
    /// 修改預算警訊
    /// </summary>
    /// <param name="strRID"></param>
    public void EditBugdet(string strRID, string strBudgetID, string strType)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[4];
        arg[0] = GetUser();
        arg[1] = DateTime.Now.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        arg[2] = strType;
        arg[3] = strBudgetID;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);

    }

    public void BudgetCardLower(string strRID, string strBudgetID, int Total_Card_Num, int Remain_Total_Num)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);

        string strWarningContent = BudgetCardLower1(strRID, strBudgetID, Total_Card_Num, Remain_Total_Num);
        if (!StringUtil.IsEmpty(strWarningContent))
            GetWarningUser(wcModel, strWarningContent);
    }



    /// <summary>
    /// 預算卡數過小
    /// </summary>
    /// <param name="strRID"></param>
    public string BudgetCardLower1(string strRID, string strBudgetID, int Total_Card_Num, int Remain_Total_Num)
    {
        string strWarningContent = "";

        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return "";

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return "";

        if (Total_Card_Num == 0)
            return "";

        PARAM pModel = GetParamModel("2");

        if (pModel == null)
            return "";

        decimal decResult = Remain_Total_Num * 100 / Total_Card_Num;
        decimal decStard = Convert.ToDecimal(pModel.Param_Name.Substring(0, pModel.Param_Name.Length - 1));

        if (decResult < decStard)
        {
            object[] arg = new object[2];
            arg[0] = strBudgetID;
            arg[1] = decStard;
            strWarningContent = string.Format(wcModel.Warning_Content, arg);
            //GetWarningUser(wcModel, strWarningContent);
        }
        return strWarningContent;
    }


    public void BudgetAmtLower(string strRID, string strBudgetID, decimal Total_Card_AMT, decimal Remain_Total_AMT)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);

        string strWarningContent = BudgetAmtLower1(strRID, strBudgetID, Total_Card_AMT, Remain_Total_AMT);
        if (!StringUtil.IsEmpty(strWarningContent))
            GetWarningUser(wcModel, strWarningContent);
    }
    
    /// <summary>
    /// 預算金額過小
    /// </summary>
    /// <param name="strRID"></param>
    public string BudgetAmtLower1(string strRID, string strBudgetID, decimal Total_Card_AMT, decimal Remain_Total_AMT)
    {
        string strWarningContent = "";

        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return "";

        if (Total_Card_AMT == 0)
            return "";

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return "";

        PARAM pModel = GetParamModel("1");

        if (pModel == null)
            return "";

        decimal decResult = Remain_Total_AMT * 100 / Total_Card_AMT;
        decimal decStard = Convert.ToDecimal(pModel.Param_Name.Substring(0, pModel.Param_Name.Length - 1));

        if (decResult < decStard)
        {
            object[] arg = new object[2];
            arg[0] = strBudgetID;
            arg[1] = decStard;
            strWarningContent = string.Format(wcModel.Warning_Content, arg);
            //GetWarningUser(wcModel, strWarningContent);
        }

        return strWarningContent;
    }


    /// <summary>
    /// 預算日期過小
    /// </summary>
    /// <param name="strRID"></param>
    public string BudgetDateLower(string strRID, string strBudgetID,DateTime EndTime)
    {
        string strWarningContent = "";

        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return "";

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return "";

        PARAM pModel = GetParamModel("3");

        if (pModel == null)
            return "";

        TimeSpan ts = EndTime - Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

        if (ts.Days < Convert.ToInt32(pModel.Param_Name.Substring(0, pModel.Param_Name.Length - 1)))
        {
            object[] arg = new object[2];
            arg[0] = strBudgetID;
            arg[1] = pModel.Param_Name.Substring(0, pModel.Param_Name.Length - 1);
            strWarningContent = string.Format(wcModel.Warning_Content, arg);
            //GetWarningUser(wcModel, strWarningContent);
        }

        return strWarningContent;
    }


    /// <summary>
    /// 修改合約警訊
    /// </summary>
    /// <param name="strRID"></param>
    public void EditAgreement(string strRID, string strAgreementID, string strType)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[4];
        arg[0] = GetUser();
        arg[1] = DateTime.Now.ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        arg[2] = strType;
        arg[3] = strAgreementID;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);

    }

    public void AgreementCardLower(string strRID, string strAgreementID, int Total_Card_Num, int Remain_Total_Num)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);

        string strWarningContent = AgreementCardLower1(strRID, strAgreementID, Total_Card_Num, Remain_Total_Num);
        if (!StringUtil.IsEmpty(strWarningContent))
            GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 合約卡數過小
    /// </summary>
    /// <param name="strRID"></param>
    public string AgreementCardLower1(string strRID, string strAgreementID, int Total_Card_Num, int Remain_Total_Num)
    {
        string strWarningContent = "";

        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return "";

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return "";

        if (Total_Card_Num == 0)
            return "";

        PARAM pModel = GetParamModel("4");

        if (pModel == null)
            return "";

        decimal decResult = Remain_Total_Num * 100 / Total_Card_Num;

        decimal decStard = Convert.ToDecimal(pModel.Param_Name.Substring(0, pModel.Param_Name.Length - 1));

        if (decResult < decStard)
        {
            object[] arg = new object[2];
            arg[0] = strAgreementID;
            arg[1] = decStard;

            strWarningContent = string.Format(wcModel.Warning_Content, arg);

            //GetWarningUser(wcModel, strWarningContent);
        }

        return strWarningContent;
    }

    /// <summary>
    /// 合約日期過小
    /// </summary>
    /// <param name="strRID"></param>
    public string AgreementDateLower(string strRID, string strAgreementID, DateTime EndTime)
    {
        string strWarningContent = "";

        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return "";

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return "";

        PARAM pModel = GetParamModel("5");

        if (pModel == null)
            return "";

        TimeSpan ts = EndTime - Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

        if (ts.Days < Convert.ToInt32(pModel.Param_Name.Substring(0, pModel.Param_Name.Length - 1)))
        {
            object[] arg = new object[2];
            arg[0] = strAgreementID;
            arg[1] = pModel.Param_Name.Substring(0, pModel.Param_Name.Length - 1);
            strWarningContent = string.Format(wcModel.Warning_Content, arg);
            //GetWarningUser(wcModel, strWarningContent);
        }

        return strWarningContent;
    }


    /// <summary>
    /// 卡種修改
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strName"></param>
    public void CardTypeEdit(string strRID, string strName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 卡種新增
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strAgreementID"></param>
    /// <param name="strType"></param>
    public void CardTypeAdd(string strRID, string strDate, string strName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[2];
        arg[0] = strDate;
        arg[1] = strName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }


    /// <summary>
    /// 採購下單待放行時
    /// </summary>
    /// <param name="strRID"></param>
    public void OrderFormCommit(string strRID)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;


        string strWarningContent = wcModel.Warning_Content;

        GetWarningUser(wcModel, strWarningContent);
    }

    public void LowSafeLevel(string strRID, string strMaterielName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strMaterielName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 新增物料採購
    /// </summary>
    /// <param name="strRID"></param>    
    public void AddPurchase(string strRID)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        string strWarningContent = wcModel.Warning_Content;

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 新增或修改物料採購作業時
    /// </summary>
    /// <param name="strRID"></param>
    public void PlsAskFinance(string strRID,string strMonthDay, string strOperator)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[2];
        arg[0] = strMonthDay;
        arg[1] = strOperator;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

   /// <summary>
    /// 物料採購新增、修改、刪除，影響4.9年度剩餘預算低於1/10
   /// </summary>
   /// <param name="strRID"></param>
   /// <param name="strMateriel"></param>
    public void SafeWarning(string strRID, string strMateriel)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strMateriel;        

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 下單時，檢查預算設定
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strBudgetID"></param>

    public void OrderBudget(string strRID, string strBudgetID, int Total_Card_Num, int Remain_Total_Num, decimal Total_Card_AMT, decimal Remain_Total_AMT, DateTime EndTime)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        string strWarningContent1 = BudgetCardLower1(GlobalString.WarningType.BudgetCardLower, strBudgetID, Total_Card_Num, Remain_Total_Num);
        string strWarningContent2 = BudgetAmtLower1(GlobalString.WarningType.BudgetAmtLower, strBudgetID, Total_Card_AMT, Remain_Total_AMT);
        string strWarningContent3 = BudgetDateLower(GlobalString.WarningType.BudgetDateLower, strBudgetID, EndTime);

        if (!StringUtil.IsEmpty(strWarningContent1))
            GetWarningUser(wcModel, strWarningContent1);
        if (!StringUtil.IsEmpty(strWarningContent2))
            GetWarningUser(wcModel, strWarningContent2);
        if (!StringUtil.IsEmpty(strWarningContent3))
            GetWarningUser(wcModel, strWarningContent3);


    }

    /// <summary>
    /// 下單時，檢查合約設定
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strAgreementID"></param>
    public void OrderAgreement(string strRID, string strAgreementID, int Card_Number, int Remain_Card_Num, DateTime EndTime)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        string strWarningContent1 = AgreementCardLower1(GlobalString.WarningType.AgreementCardLower, strAgreementID, Card_Number, Remain_Card_Num);
        string strWarningContent2=AgreementDateLower(GlobalString.WarningType.AgreementDateLower, strAgreementID, EndTime);

        if (!StringUtil.IsEmpty(strWarningContent1))
            GetWarningUser(wcModel, strWarningContent1);
        if (!StringUtil.IsEmpty(strWarningContent2))
            GetWarningUser(wcModel, strWarningContent2);
    }

    /// <summary>
    /// 修改Perso廠與卡種設定
    /// </summary>
    /// <param name="strRID"></param>
    public void Perso_CardTypeEdit(string strRID, string strCardName, string strFacotryName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[2];
        arg[0] = strCardName;
        arg[1] = strFacotryName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);

    }

    /// <summary>
    /// 獲得指定perso廠指定物料的耗損率
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    /// <param name="MaterialName"></param>
    /// <param name="wafer"></param>
    public void MaterialDataIn(string strRID,string strFactoryName,string MaterialName,decimal wafer)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[3];
        arg[0] = strFactoryName;
        arg[1] = MaterialName;
        arg[2] = wafer;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);

    }
    
    /// <summary>
    /// perso廠物料庫存自動匯入格式有誤
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    public void MaterialAutoDataIn(string strRID, string strFactoryName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strFactoryName;       

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 物料系統結餘數不足
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    public void MaterialDataInMiss(string strRID, string MaterialName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = MaterialName;       

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 物料系統結餘數低於安全庫存
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    public void MaterialDataInSafe(string strRID, string MaterialName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = MaterialName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 自動匯入失敗
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    public void MaterialDataInLost(string strRID, string strFactoryName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strFactoryName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 空白卡費用帳務作業 提交时
    /// </summary>
    /// <param name="strRID"></param>
    public void BlankCardMoney(string strRID)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = "";

        string strWarningContent = wcModel.Warning_Content;

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 物料請款(郵資費) 郵資費新增、修改、刪除时
    /// </summary>
    /// <param name="strRID"></param>
    public void MatrrielSapAskMoney(string strRID,string strMateriel_Type)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strMateriel_Type;

        string strWarningContent = string.Format(wcModel.Warning_Content,arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 代製費用請款新增、修改、刪除时
    /// </summary>
    /// <param name="strRID"></param>
    public void PersoProjectSapAskMoney(string strRID,string type)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = type;

        string strWarningContent = string.Format(wcModel.Warning_Content,arg);

        GetWarningUser(wcModel, strWarningContent);
    }


    /// <summary>
    /// 代製費用異動 自動匯入有誤時
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    public void PersoProjectChange(string strRID, string strFactoryName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strFactoryName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 日結時，XXXPerso廠XXX版面簡稱庫存不足
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    public void CardTypeNotEnough(string strRID, string strFactoryCN, string strCardTypeName)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[2];
        arg[0] = strFactoryCN;
        arg[1] = strCardTypeName;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }


    /// <summary>
    /// 年度換卡預測檔格式不正確
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strErrorMsg"></param>
    public void YearChangeCardForeCast(string strRID, string strErrorMsg)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strErrorMsg;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 月度換卡預測檔格式不正確
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strErrorMsg"></param>
    public void MonthChangeCardForeCast(string strRID, string strErrorMsg)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[1];
        arg[0] = strErrorMsg;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

    /// <summary>
    /// 小計檔匯入時，格式檢查
    /// </summary>
    /// <param name="strRID"></param>
    /// <param name="strFactoryName"></param>
    public void SubTotalDataIn(string strRID, string strFileName, string strErrorMsg)
    {
        WARNING_CONFIGURATION wcModel = GetModel(strRID);
        if (wcModel == null)
            return;

        if (wcModel.System_Show == "N" && wcModel.Mail_Show == "N")
            return;

        object[] arg = new object[2];
        arg[0] = strFileName;
        arg[1] = strErrorMsg;

        string strWarningContent = string.Format(wcModel.Warning_Content, arg);

        GetWarningUser(wcModel, strWarningContent);
    }

}