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
/// GlobalString 的摘要描述
/// </summary>
public class GlobalString
{
    public GlobalString()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public static class SessionAndCookieKeys
    {
        public const string CULTURE_CODE = "PreferredCulture";
        public const string OUT = "Out";
        public const string USER = "User";
        public const string AGNET = "Agnet";
        public const string GROUPS = "Groups";
        public const string ACTIONS = "Actions";
        public const string RC_CODES = "RC_Codes";
        public const string USER_ID = "UserID";
        public const string MAINVIEW = "MainView";
        public const string LANG = "Lang";

    }

    public static class PageUrl
    {
        public const string ACTION_TYPE_BASE = "ActionType";
        public const string ACTION_TYPE = "?ActionType=";
        public const string ID = "ID";
        public const string INDEX = "index.htm";
        public const string MAINFRAME = "MainFrame.aspx";
        public const string MAINVIEWPAGE = "None.aspx";
        public const string LOGIN = "/CIMS/Login.aspx";
        public const string NOPAGEACTION = "/CIMS/NoPageAction.aspx";
    }

    public static class ActionName
    {
        public const string SEARCH = "Search";
        public const string ADD = "Add";
        public const string EDIT = "Edit";
        public const string DELETE = "Delete";
        public const string START = "Start";
        public const string STOP = "Stop";
        public const string FOLLOW = "Follow";
        public const string UPLOAD = "Upload";
        public const string PRINT = "Print";
        public const string Q_SEARCH = "QSearch";
        public const string REVERT = "Revert";
        public const string EXPORT = "Export";
        public const string SET = "Set";
        public const string NOW_EXPLAIN = "NowExplain";
        public const string GET_ACOUNT = "GetAcount";
        public const string SR_COUNT = "SRCount";
        public const string QUARY = "Quary";
        public const string NONE = "None";
    }

    /// <summary>
    /// 以下用於Dictionary和HashTable中的key
    /// </summary>
    public static class KeyValue
    {
        //每個頁面的ActionID
        public const string KEY_ACTIONID = "actionid";
        //為了與ActionID有所區分，加入此Key
        public const string KEY_ACTIONFLAG = "ActionFlag";

        //bool類型的數據標記
        public const string KEY_BOOL_FLAG = "boolflag";
        //string類型的數據標記
        public const string KEY_STRING_FLAG = "stringflag";
        //數字類型的數據標記
        public const string KEY_NUMBERIC_FLAG = "numbericflag";
        
        /// <summary>
        /// 動作類型
        /// </summary>
        public const string KEY_ACTION_INSERT = "insert";
        public const string KEY_ACTION_UPDATE = "update";
        public const string KEY_ACTION_DELETE = "delete";
        public const string KEY_ACTION_SELECT = "select";
        public const string KEY_ACTION_COPY = "copy";

        //表明對應的value為一個model類型的數據，單個
        public const string KEY_DTO = "dto";
        //表明對應的value為一個model類型的數據，用於Update
        public const string KEY_DTO_UPDATE = "dtoupdate";
        //表明對應的value為一個model類型的數據，用於insert
        public const string KEY_DTO_INSERT = "dtoinsert";
        //表明對應的value為一個model類型的數據，用於delete
        public const string KEY_DTO_DELETE = "dtodelete";

        /// <summary>
        /// 以下Key主要在組合欄位和自定義報表中用到，類似功能亦可使用
        /// </summary>
        public const string KEY_WHERE_STRING = "WhereString";
        public const string KEY_SELECT_COLUMNS_STRING = "SelectColumns";
        public const string KEY_QUERY_FIELDS_STRING = "QueryFields";
        public const string KEY_SELETED_TABLE_NAMES_STRING = "SelectedTableName";
        public const string KEY_ORDER_BY_COLUMNS_STRING = "OrderByColumns";
        public const string KEY_ORDER_STRING = "Order";
    }
    /// <summary>
    /// 自定義報表需要用到的常量
    /// </summary>
    public static class TailorReport
    {
        public const string REPORT_URL = "ShowCustomReport.aspx";
        /// <summary>
        /// 組成動態查詢欄位用到
        /// </summary>
        public const string SEARCH_GROUP_NAME = "Search";
        public const string DATE_CTRL_TYPE = "Date";
        public const string TEXT_CTRL_TYPE = "Text";
        public const string NUMBER_CTRL_TYPE = "Number";
        public const string AUTO_CTRL_NAME = "AutoCtrl";
        /// <summary>
        /// 預覽的模式
        /// </summary>
        public const string PREVIEW_MODE = "PreViewMode";
        //普通模式
        public const string PREVIEW_NORMAL = "PreViewNormal";
        //用戶輸入SQL模式
        public const string PREVIEW_CUSTOMSQL = "PreViewCustomSQL";

    }

    /// <summary>
    /// SQLServer Report Service 上的報表文件名
    /// </summary>
    public static class SQLReport
    {
        public const string USERINFO = "/CIMSReport/UserInfo";
        public const string CARDSTOCKSMOVE = "/CIMSReport/CardStocksMove";
        public const string Report009 = "/CIMSReport/REPORT_009";
        public const string Report013 = "/CIMSReport/REPORT_013";
        public const string Report014 = "/CIMSReport/REPORT_014";
        public const string Report015 = "/CIMSReport/REPORT_015";
        public const string Report017 = "/CIMSReport/REPORT_017";
        public const string Report018 = "/CIMSReport/REPORT_018";
    }

    public static class RST
    {
        public const string ACTIVED = "A";
        public const string UPDATE = "U";
        public const string DELETE = "D";
    }

    public static class LogType
    {
        public const string ErrorCategory = "Error";
        public const string OpLogCategory = "OpLog";
        public const string DebugCategory = "Debug";
    }

    public static class CSLB
    {
        public const string BlankFactory = "空白卡廠";
        public const string PersoFactory = "Perso廠";
    }

    public static class YNType
    {
        public const string Yes = "Y";
        public const string No = "N";
    }

    public static class Billing_Type
    {
        public const string Blank = "銀行帳";
        public const string Card = "信用卡帳";
    }

    public static class AllPart
    {
        //全部
        public const string All = "1";
        //部分
        public const string Part = "2";
    }

    public static class BaseLevle
    {
        //基本
        public const string All = "1";
        //級距
        public const string Part = "2";
    }

    public static class Percentage_Number
    {
        //比率
        public const string Percentage = "1";
        //數量
        public const string Number = "2";
    }

    public static class BaseSpecial
    {
        //基本
        public const string Base = "1";
        //特殊
        public const string Special = "2";
    }

    public static class CheckType
    {
        //抽驗
        public const string All = "1";
        //全驗
        public const string Part = "2";
    }

    public static class SendCheck
    {
        //未送審
        public const string notSend = "1";
        //已送審
        public const string sended = "2";
        //送審完成
        public const string finished = "3";
    }

    public static class Exigence
    {
        //Urgent
        public const string Urgent = "1";
        //Normal
        public const string Normal = "2";
    }

    public static class ImportType
    {
        //小計檔匯入
        public const string totalText = "1";
        //廠商異動匯入
        public const string factoryChange = "2";
    }

    public static class SafeType
    {
        //最低安全庫存
        public const string storage = "1";
        //安全天數
        public const string days = "2";
        //用完為止
        public const string over = "3";
    }

    public static class TextType
    {
        //小計檔
        public const string storage = "1";
        //次月預測
        public const string days = "2";
        //年度預測
        public const string over = "3";
    }

    public static class Virtual_Card_Group
    {
        // 虛擬卡群組
        public const string virtual_card_group = "虛擬卡";
    }

    public static class SafeMonth
    {
        //安全庫存月數
        public const string month = "month";
    }

    public static class Percent
    {
        //換卡百分比
        public const string value = "percentValue";
    }

    public static class ParameterType
    {
        //用途類參數
        public const string Use = "use";
        //帳務群組參數
        public const string Use1 = "use1";
        //卡片狀態
        public const string Status = "status";
        //卡別級距
        public const string CardType = "cardType";
        //物料類別
        //public const string MaterielType = "materielType";
        //物料類別縮寫
        public const string MatType = "matType";
        //物料類別
        public const string MatType1 = "matType1";     
        //安全存量
        //public const string SafeType = "safeType";
        //送審狀態
        public const string SendCheck = "SendCheck";
        //類別
        public const string Type = "Type";
        //年份
        //public const string Year = "Year";
        //預估過去X月的月平均 
        //public const string xYear = "xYear";
        //使用者設定的希望庫存可以支援的可用月數參數類型標識
        //public const string NType = "NType";
        //廠商庫存異動匯入狀況代碼
        public const string ChangeStatus = "Change_Status";
        //放行狀態
        public const string PassStatus = "passStatus";
        //緊急程度
        //public const string EmergencyLevel = "emergencyLevel";
        
        //查詢所有參數類別時"ParamType_Code"的取值
        public const string Total = "total";
        //公式名稱
        public const string FormulaName = "FormulaName";
        //Operation
        public const string Operation = "Operation";
        //卡片管理參數設定
        public const string CardParam = "CardParam";
        //卡片成本帳務期間設定
        public const string CardCost = "CardCost";
        // 代製費用帳務異動項目設定 
        public const string Finance = "Finance";
        //操作類型
        public const string OprType = "OprType";


    }
    /// <summary>
    /// 卡片參數子系列
    /// </summary>
    public static class cardparam
    {
        //換卡百分比
        public const string Percent = "15";
        // 安全庫存月數
        public const string SafeMonth = "6";
        // 每月監控
        public const string NType = "7";
        public const string X1 = "8";
        public const string X2 = "9";
        public const string X3 = "10";
        // 每日監控
        public const string YType = "11";
        public const string Y1 = "12";
        public const string Y2 = "13";
        public const string Y3 = "14";

    }

    public static class Parameter
    {
        //製卡
        public const string Type = "use2";
        //帳務
        public const string Finance = "use1";
    }

    public static class FileSplit
    {
        //文件分割符號
        public const char Split = '^';
    }

    //設定計算公式的RID
    public static class Expression
    {     
        //製成卡數
        public const int Made_RID = 1;
        //消耗卡數
        public const int Used_RID = 2;
        //耗損卡數
        public const int Waste_RID = 3;
        //入(出)庫
        public const int InOut_RID = 4;
        //特殊卡數
        public const int Special_RID = 5;
    }

    //設定計算公式因子操作符的編號
    public static class Operation
    {
        //+
        public const string Add_RID = "+";
        //-
        public const string Del_RID = "-";
    }


    /// <summary>
    /// 定義警訊類別
    /// 此常量值對應於警訊配置檔里面的RID欄位，設置時參考SRS中EXCEL的行數-1！
    /// </summary>
    public static class Alert_Operation
    {
        //匯入小計檔時格式錯誤
        public const int ALERT_6_1_FILE_FORMAT_ERROR=20;
        //匯入小計檔時
        public const int ALERT_6_1_CARD_NOT_ENOUGH = 21;
        //物料庫存不足(自動或人工匯入)
        public const int ALERT_6_1_MATERIAL_NOT_ENOUGH = 25;
        //物料低於安全庫存(自動或人工匯入)
        public const int ALERT_6_1_LESS_THAN_LIMITS = 26;


    }

    public static class UserRoleConfig
    {
        public const string Add = "00342";
        public const string Edit = "00343";
        public const string Finanace021Edit = "00943";
        public const string Delete = "00344";
        public const string Commit = "003422";
        public const string Pass = "003423";
        public const string Reject = "003424";
        public const string Save = "003425";
    }

    //物料類別
    //1：寄卡單（卡）；2：寄卡單（銀）；3：信封（卡）；4：信封（銀）；5：DM（卡）；6：DM（銀）；7：郵資費（卡）；8：郵資費（銀）；9：代製費用 （卡）；10：代製費用（銀） 
    public static class MaterielType
    {
        public const string EXPONENT_CARD = "1";
        public const string EXPONENT_BANK = "2";
        public const string ENVELOPE_CARD = "3";
        public const string ENVELOPE_BANK = "4";
        public const string DM_CARD = "5";
        public const string DM_BANK = "6";
        public const string POSTAGE_CARD = "7";
        public const string POSTAGE_BANK = "8";
        public const string EXPENSE_CARD = "9";
        public const string EXPENSE_BANK = "10";
    }

    public static class Key
    {
        public const string Word = "1qazxsw23edcvfr4";
    }

    public static class WarningType
    {
        public const string EditBugdet = "1";
        public const string BudgetAmtLower = "2";
        public const string BudgetCardLower = "3";
        public const string BudgetDateLower = "4";
        public const string EditAgreement = "5";
        public const string AgreementCardLower = "6";
        public const string AgreementDateLower = "7";
        public const string CardTypeEdit = "8";
        public const string CardTypeAdd = "9";
        public const string Perso_CardTypeEdit = "10";
        public const string LowSafeLevel = "20";
        public const string AddPurchase = "21";
        public const string PlsAskFinance = "22";
        public const string SafeWarning = "16";
        public const string OrderFormCommit = "17";
        public const string OrderBudget = "18";
        public const string OrderAgreement = "19";
        public const string DepositoryBudget = "23";
        public const string DepositoryAgreement = "24";
        public const string CancelBudget = "25";
        public const string CancelAgreement = "26";
        public const string RestockBudget = "27";
        public const string RestockAgreement = "28";
        public const string MaterialDataIn = "11";
        public const string MaterialAutoDataIn = "12";
        public const string MaterialDataInMiss = "13";
        public const string MaterialDataInSafe = "14";
        public const string MaterialDataInLost = "30";
        public const string BlankCardMoney = "31";
        public const string MatrrielSapAskMoney = "32";
        public const string PersoProjectSapAskMoney = "33";
        public const string PersoProjectChange = "34";
        public const string SubTotalDataIn = "35";
        public const string CardTypeNotEnough = "37";
        public const string YearChangeCardForeCast = "39";
        public const string MonthChangeCardForeCast = "40";
        public const string MaterialBuget = "41";
        public const string SurplusMaterialBuget="42";
        public const string SubtotalMaterialInMiss = "43";
        public const string SubtoalMaterialInSafe = "44";
        public const string PersoChangeMaterialInMiss = "45";
        public const string PersoChangeMaterialInSafe = "46";
        public const string PersoChangeCardInMiss = "47";
        public const string BatchPersoChangeMaterialInMiss = "48";
        public const string BatchPersoChangeMaterialInSafe = "49";
        public const string BatchPersoChangeCardInMiss = "50";
        public const string AskFinanceBudget="51";
        public const string AskFinanceAgree = "52";
    }

    public static class WarningContent
    {
        public const string Msg1 = "xxx人員於yyyy/mm/dd，於預算管理作業項目中，修改[or刪除] 簽呈文號xxxx內容";
        public const string Msg2 = "xxx人員於yyyy/mm/dd，於合約管理作業項目中，修改[or刪除]合約編號xxxx內容";
        public const string Msg3 = "簽呈文號為xxxx預算總金額已低於X%，請注意!";
        public const string Msg4 = "簽呈文號為xxxx預算總卡數已低於X%，請注意!";
        public const string Msg5 = "合約編號為xxxx卡數已低於X%，請注意!";
        public const string Msg6 = "xxxxx(版面簡稱)已低於當日安全庫存X月";
        public const string Msg7 = "xxxxx(版面簡稱)已低於每日監控作業X日";
        public const string Msg8 = "xxxPerso廠xxx品名耗損率xx%";
        public const string Msg9 = "修改xxx版面簡稱對應xxxPerso廠，請確認切檔系統之Perso廠與卡種設定檔是否完整匯入";
        public const string Msg10 = "MM/DD新增卡種，版面簡稱：xxx、請至相關功能設定卡種所用信封、寄卡單、DM、材質、代製項目。";
        public const string Msg11 = "xxx版面簡稱卡種基本資料已被更新，請確認相關設定是否正確。";
        public const string Msg12 = "xx物料系統結餘數不足";
        public const string Msg13 = "xx物料系統結餘數低於安全庫存";
        public const string Msg14 = "xx物料系統結餘數不足";
        public const string Msg15 = "xx物料系統結餘數低於安全庫存";
        public const string Msg16 = "XXXPerso廠XXX版面簡稱庫存不足";
        public const string Msg17 = "匯入文件錯誤訊息";
        public const string Msg18 = "xxxperso廠卡片庫存自動匯入格式有誤";
        public const string Msg19 = "xx物料系統結餘數不足";
        public const string Msg20 = "xx物料系統結餘數低於安全庫存";
        public const string Msg21 = "XXXPerso廠XXX版面簡稱庫存不足";
        public const string Msg22 = "xxxperso廠物料庫存自動匯入格式有誤";
        public const string Msg23 = "xxxperso廠perso費用自動匯入格式有誤";
        public const string Msg24 = "xx物料系統結餘數不足";
        public const string Msg25 = "xxxperso廠物料庫存自動匯入格式有誤";
        public const string Msg26 = "xxxperso廠perso費用自動匯入格式有誤";
        public const string Msg27 = "xx物料系統結餘數不足";
        public const string Msg28 = "xx物料系統結餘數低於安全庫存";
        public const string Msg29 = "yyyy/mm/dd xxx perso廠 xxx版面簡稱核對有誤，停止日結";
        public const string Msg30 = "xx物料[新增/修改]採購作業，請至系統內作請款作業";
        public const string Msg31 = "xxx項目年度剩餘預算小於1/10";
        public const string Msg32 = "簽呈文號為xxxx期間已低於x日，請注意!";
        public const string Msg33 = "合約編號為xxxx合約期間已低於x日，請注意!";
        public const string Msg34 = "xxxx(版面簡稱)已低於每月監控作業x月";
        public const string Msg35 = "xx檔自動匯入失敗";
        public const string Msg36 = "xx年度預算剩餘金額低於10%";


    }
}
