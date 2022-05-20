using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Warning 的摘要描述
/// </summary>
public class Warning
{
    public Warning()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public static void SetWarning(string strType,object[] arg)
    {
        BaseWarning bw = new BaseWarning();

        switch (strType)
        {
            case GlobalString.WarningType.EditBugdet: bw.EditBugdet(strType,arg[0].ToString(), arg[1].ToString()); break;
            case GlobalString.WarningType.BudgetAmtLower: bw.BudgetAmtLower(strType,arg[0].ToString(), Convert.ToDecimal(arg[1]), Convert.ToDecimal(arg[2])); break;
            case GlobalString.WarningType.BudgetCardLower: bw.BudgetCardLower(strType,arg[0].ToString(), Convert.ToInt32(arg[1]), Convert.ToInt32(arg[2])); break;
            case GlobalString.WarningType.EditAgreement: bw.EditAgreement(strType,arg[0].ToString(), arg[1].ToString()); break;
            case GlobalString.WarningType.AgreementCardLower: bw.AgreementCardLower(strType,arg[0].ToString(), Convert.ToInt32(arg[1]), Convert.ToInt32(arg[2])); break;
            case GlobalString.WarningType.CardTypeEdit: bw.CardTypeEdit(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.CardTypeAdd: bw.CardTypeAdd(strType, arg[0].ToString(), arg[1].ToString()); break;
            case GlobalString.WarningType.OrderFormCommit: bw.OrderFormCommit(strType); break;
            case GlobalString.WarningType.LowSafeLevel: bw.LowSafeLevel(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.AddPurchase: bw.AddPurchase(strType); break;
            case GlobalString.WarningType.PlsAskFinance: bw.PlsAskFinance(strType, arg[0].ToString(), arg[1].ToString()); break;
            case GlobalString.WarningType.SafeWarning: bw.SafeWarning(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.OrderBudget: bw.OrderBudget(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()),Convert.ToDecimal(arg[3].ToString()), Convert.ToDecimal(arg[4].ToString()),Convert.ToDateTime(arg[5].ToString())); break;
            case GlobalString.WarningType.OrderAgreement: bw.OrderAgreement(strType, arg[0].ToString(),Convert.ToInt32(arg[1].ToString()),Convert.ToInt32(arg[2].ToString()),Convert.ToDateTime(arg[3].ToString())); break;
            case GlobalString.WarningType.DepositoryBudget: bw.OrderBudget(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()), Convert.ToDecimal(arg[3].ToString()), Convert.ToDecimal(arg[4].ToString()), Convert.ToDateTime(arg[5].ToString())); break;
            case GlobalString.WarningType.DepositoryAgreement: bw.OrderAgreement(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()), Convert.ToDateTime(arg[3].ToString())); break;
            case GlobalString.WarningType.CancelBudget: bw.OrderBudget(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()), Convert.ToDecimal(arg[3].ToString()), Convert.ToDecimal(arg[4].ToString()), Convert.ToDateTime(arg[5].ToString())); break;
            case GlobalString.WarningType.CancelAgreement: bw.OrderAgreement(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()), Convert.ToDateTime(arg[3].ToString())); break;
            case GlobalString.WarningType.RestockBudget: bw.OrderBudget(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()), Convert.ToDecimal(arg[3].ToString()), Convert.ToDecimal(arg[4].ToString()), Convert.ToDateTime(arg[5].ToString())); break;
            case GlobalString.WarningType.RestockAgreement: bw.OrderAgreement(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()), Convert.ToDateTime(arg[3].ToString())); break;
            case GlobalString.WarningType.Perso_CardTypeEdit: bw.Perso_CardTypeEdit(strType, arg[0].ToString(), arg[1].ToString()); break;
            case GlobalString.WarningType.MaterialDataIn: bw.MaterialDataIn(strType, arg[0].ToString(), arg[1].ToString(),Convert.ToDecimal(arg[2].ToString())); break;
            case GlobalString.WarningType.MaterialAutoDataIn: bw.MaterialAutoDataIn(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.MaterialDataInMiss: bw.MaterialDataInMiss(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.MaterialDataInSafe: bw.MaterialDataInSafe(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.MaterialDataInLost: bw.MaterialDataInLost(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.BlankCardMoney: bw.BlankCardMoney(strType); break;
            case GlobalString.WarningType.MatrrielSapAskMoney: bw.MatrrielSapAskMoney(strType,arg[0].ToString()); break;
            case GlobalString.WarningType.PersoProjectSapAskMoney: bw.PersoProjectSapAskMoney(strType,arg[0].ToString()); break;
            case GlobalString.WarningType.PersoProjectChange: bw.PersoProjectChange(strType, arg[0].ToString()); break;

            case GlobalString.WarningType.SubTotalDataIn: bw.SubTotalDataIn(strType, arg[0].ToString(), arg[1].ToString()); break;
            case GlobalString.WarningType.CardTypeNotEnough: bw.CardTypeNotEnough(strType, arg[0].ToString(), arg[1].ToString()); break;

            case GlobalString.WarningType.YearChangeCardForeCast: bw.YearChangeCardForeCast(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.MonthChangeCardForeCast: bw.MonthChangeCardForeCast(strType, arg[0].ToString()); break;

            // case GlobalString.WarningType.MaterialBuget: bw.LowSafeLevel(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.SurplusMaterialBuget: bw.LowSafeLevel(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.SubtotalMaterialInMiss: bw.MaterialDataInMiss(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.SubtoalMaterialInSafe: bw.MaterialDataInSafe(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.PersoChangeMaterialInMiss: bw.MaterialDataInMiss(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.PersoChangeMaterialInSafe: bw.MaterialDataInSafe(strType, arg[0].ToString()); break;
            case GlobalString.WarningType.PersoChangeCardInMiss: bw.CardTypeNotEnough(strType, arg[0].ToString(), arg[1].ToString()); break;
            case GlobalString.WarningType.AskFinanceBudget: bw.OrderBudget(strType,arg[0].ToString(),Convert.ToInt32(arg[1].ToString()), 
                Convert.ToInt32(arg[2].ToString()), Convert.ToDecimal(arg[3].ToString()), Convert.ToDecimal(arg[4].ToString()), 
                Convert.ToDateTime(arg[5].ToString())); break;
            //case GlobalString.WarningType.AskFinanceAgree: bw.OrderAgreement(strType, arg[0].ToString(), Convert.ToInt32(arg[1].ToString()), Convert.ToInt32(arg[2].ToString()), Convert.ToDateTime(arg[3].ToString())); break;

            
            default:break;
        }

    }
}
