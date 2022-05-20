//******************************************************************
//*  作    者：QingChen
//*  功能說明：日曆控制項
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

namespace ControlLibrary
{
    public class CalendarTextBox : TextBox
    {
        private string scriptFilePath = "";

        private string previousMonthTipText = "Previous month";

        private string menuFont = "Arial";

        private string selectYearTipText = "Click here to select year";

        private string selectMonthTipText = "Click here to select month";

        private string nextMonthTipText = "Next month";

        private string previousYearTipText = "Previous year";

        private string nextYearTipText = "Next year";

        private string toDay = "Today";

        private string nowDate = "Now date";

        private string err_TantoParameter = "Parameter is tanto";

        private string err_NoneParameter = "Parameter is none";

        private string yearText = "-";

        private string monthText = "-";

        private string dateText = "-";

        private string yearInputNotNumberTipText = "Inputed Year is'not number";

        private string yearNumberNot = "Inputed year number is error";

        private string monthInputNotNumberTipText = "Inputed Month is'not number";

        private string noneCtrl = "Can not find Ctrl!";

        private string closeText = "Close";

        [DefaultValue("")]
        public string ScriptFilePath
        {
            get
            {
                return this.scriptFilePath;
            }
            set
            {
                this.scriptFilePath = value;
            }
        }

        [DefaultValue("Previous month"), Localizable(true)]
        public string PreviousMonthTipText
        {
            get
            {
                return this.previousMonthTipText;
            }
            set
            {
                this.previousMonthTipText = value;
            }
        }

        [DefaultValue("Arial"), Localizable(true)]
        public string MenuFont
        {
            get
            {
                return this.menuFont;
            }
            set
            {
                this.menuFont = value;
            }
        }

        [DefaultValue("Click here to select year"), Localizable(true)]
        public string SelectYearTipText
        {
            get
            {
                return this.selectYearTipText;
            }
            set
            {
                this.selectYearTipText = value;
            }
        }

        [DefaultValue("Click here to select month"), Localizable(true)]
        public string SelectMonthTipText
        {
            get
            {
                return this.selectMonthTipText;
            }
            set
            {
                this.selectMonthTipText = value;
            }
        }

        [DefaultValue("Next month"), Localizable(true)]
        public string NextMonthTipText
        {
            get
            {
                return this.nextMonthTipText;
            }
            set
            {
                this.nextMonthTipText = value;
            }
        }

        [DefaultValue("Previous year"), Localizable(true)]
        public string PreviousYearTipText
        {
            get
            {
                return this.previousYearTipText;
            }
            set
            {
                this.previousYearTipText = value;
            }
        }

        [DefaultValue("Next year"), Localizable(true)]
        public string NextYearTipText
        {
            get
            {
                return this.nextYearTipText;
            }
            set
            {
                this.nextYearTipText = value;
            }
        }

        [DefaultValue("Today"), Localizable(true)]
        public string ToDay
        {
            get
            {
                return this.toDay;
            }
            set
            {
                this.toDay = value;
            }
        }

        [DefaultValue("Now date"), Localizable(true)]
        public string NowDate
        {
            get
            {
                return this.nowDate;
            }
            set
            {
                this.nowDate = value;
            }
        }

        [DefaultValue("Parameter is tanto"), Localizable(true)]
        public string Err_TantoParameter
        {
            get
            {
                return this.err_TantoParameter;
            }
            set
            {
                this.err_TantoParameter = value;
            }
        }

        [DefaultValue("Parameter is none"), Localizable(true)]
        public string Err_NoneParameter
        {
            get
            {
                return this.err_NoneParameter;
            }
            set
            {
                this.err_NoneParameter = value;
            }
        }

        [DefaultValue("-"), Localizable(true)]
        public string YearText
        {
            get
            {
                return this.yearText;
            }
            set
            {
                this.yearText = value;
            }
        }

        [DefaultValue("-"), Localizable(true)]
        public string MonthText
        {
            get
            {
                return this.monthText;
            }
            set
            {
                this.monthText = value;
            }
        }

        [DefaultValue("-"), Localizable(true)]
        public string DateText
        {
            get
            {
                return this.dateText;
            }
            set
            {
                this.dateText = value;
            }
        }

        [DefaultValue("Inputed month is'not number"), Localizable(true)]
        public string YearInputNotNumberTipText
        {
            get
            {
                return this.yearInputNotNumberTipText;
            }
            set
            {
                this.yearInputNotNumberTipText = value;
            }
        }

        [DefaultValue("Inputed year number is error"), Localizable(true)]
        public string YearNumberNot
        {
            get
            {
                return this.yearNumberNot;
            }
            set
            {
                this.yearNumberNot = value;
            }
        }

        [DefaultValue("Inputed year is'not number"), Localizable(true)]
        public string MonthInputNotNumberTipText
        {
            get
            {
                return this.monthInputNotNumberTipText;
            }
            set
            {
                this.monthInputNotNumberTipText = value;
            }
        }

        [DefaultValue("Can not find Ctrl!"), Localizable(true)]
        public string NoneCtrl
        {
            get
            {
                return this.noneCtrl;
            }
            set
            {
                this.noneCtrl = value;
            }
        }

        [DefaultValue("Close"), Localizable(true)]
        public string CloseText
        {
            get
            {
                return this.closeText;
            }
            set
            {
                this.closeText = value;
            }
        }

        [Bindable(true)]
        public DateTime SelectDate
        {
            get
            {
                return this.GetDate();
            }
            set
            {
                base.Text = value.ToString("yyyy-MM-dd");
            }
        }

        public CalendarTextBox()
        {
            base.ReadOnly = true;
        }

        private DateTime GetDate()
        {
            if (this.Page.IsPostBack)
            {
                base.Text = this.Page.Request.Form[this.UniqueID];
            }
            DateTime result;
            if (base.Text != null && base.Text != string.Empty && base.Text != "")
            {
                string[] array = base.Text.Split("-".ToCharArray());
                result = new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
            }
            else
            {
                result = DateTime.MinValue;
            }
            return result;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.Attributes.Add("onfocus", "setday(this);");
            if (!base.DesignMode)
            {
                base.Attributes.Add("onfocus", "setday(this);");
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("CalendarTextBox"))
                {
                    if (this.scriptFilePath == string.Empty || this.scriptFilePath == null || this.scriptFilePath == "")
                    {
                        this.scriptFilePath = HttpContext.Current.Server.MapPath("~") + "\\js\\Calendar.js";
                    }
                    string input = File.ReadAllText(this.scriptFilePath, Encoding.UTF8);
                    string pattern = "(<%#MenuFont%>)|(<%#PreviousMonthTipText%>)|(<%#SelectYearTipText%>)|(<%#SelectMonthTipText%>)|(<%#NextMonthTipText%>)|(<%#PreviousYearTipText%>)|(<%#NextYearTipText%>)|(<%#ToDay%>)|(<%#NowDate%>)|(<%#Err_TantoParameter%>)|(<%#Err_NoneParameter%>)|(<%#YearText%>)|(<%#MonthText%>)|(<%#DateText%>)|(<%#YearInputNotNumberTipText%>)|(<%#YearNumberNot%>)|(<%#MonthInputNotNumberTipText%>)|(<%#NoneCtrl%>)|(<%#CloseText%>)|(<%#Dayofweek1%>)|(<%#Dayofweek2%>)|(<%#Dayofweek3%>)|(<%#Dayofweek4%>)|(<%#Dayofweek5%>)|(<%#Dayofweek6%>)|(<%#Dayofweek7%>)";
                    Regex regex = new Regex(pattern);
                    MatchEvaluator evaluator = new MatchEvaluator(this.ReplaceScript);
                    this.Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "CalendarTextBox", "<script type=\"text/javascript\">" + regex.Replace(input, evaluator) + "</script>");
                }
            }
        }

        private string ReplaceScript(Match m)
        {
            string[] abbreviatedDayNames = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AbbreviatedDayNames;
            string value = m.Value;
            string result;
            switch (value)
            {
                case "<%#MenuFont%>":
                    result = this.menuFont;
                    return result;
                case "<%#PreviousMonthTipText%>":
                    result = this.previousMonthTipText;
                    return result;
                case "<%#SelectYearTipText%>":
                    result = this.selectYearTipText;
                    return result;
                case "<%#SelectMonthTipText%>":
                    result = this.selectMonthTipText;
                    return result;
                case "<%#NextMonthTipText%>":
                    result = this.nextMonthTipText;
                    return result;
                case "<%#PreviousYearTipText%>":
                    result = this.previousYearTipText;
                    return result;
                case "<%#NextYearTipText%>":
                    result = this.nextYearTipText;
                    return result;
                case "<%#ToDay%>":
                    result = this.toDay;
                    return result;
                case "<%#NowDate%>":
                    result = this.nowDate;
                    return result;
                case "<%#Err_TantoParameter%>":
                    result = this.err_TantoParameter;
                    return result;
                case "<%#Err_NoneParameter%>":
                    result = this.err_NoneParameter;
                    return result;
                case "<%#YearText%>":
                    result = this.yearText;
                    return result;
                case "<%#MonthText%>":
                    result = this.monthText;
                    return result;
                case "<%#DateText%>":
                    result = this.dateText;
                    return result;
                case "<%#YearInputNotNumberTipText%>":
                    result = this.yearInputNotNumberTipText;
                    return result;
                case "<%#YearNumberNot%>":
                    result = this.yearNumberNot;
                    return result;
                case "<%#MonthInputNotNumberTipText%>":
                    result = this.monthInputNotNumberTipText;
                    return result;
                case "<%#NoneCtrl%>":
                    result = this.noneCtrl;
                    return result;
                case "<%#CloseText%>":
                    result = this.closeText;
                    return result;
                case "<%#Dayofweek1%>":
                    result = "Mon";
                    return result;
                case "<%#Dayofweek2%>":
                    result = "Tue";
                    return result;
                case "<%#Dayofweek3%>":
                    result = "Wed";
                    return result;
                case "<%#Dayofweek4%>":
                    result = "Thu";
                    return result;
                case "<%#Dayofweek5%>":
                    result = "Fri";
                    return result;
                case "<%#Dayofweek6%>":
                    result = "Sat";
                    return result;
                case "<%#Dayofweek7%>":
                    result = "Sun";
                    return result;
            }
            result = m.Value;
            return result;
        }
    }
}
