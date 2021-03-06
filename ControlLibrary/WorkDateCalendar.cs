using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace ControlLibrary
{
    /// <summary>
    /// 非工作日選擇控制項
    /// </summary>
   public class WorkDateCalendar:Calendar
    {
       public event OnMonthChangeDelegate OnSetDate = default(OnMonthChangeDelegate);
       public WorkDateCalendar():base()
       {
           DateTime n = DateTime.Now;
           DateTime td = new DateTime(n.Year, n.Month, 1);
          base.VisibleDate = td;
           
       }
       protected override void OnLoad(EventArgs e)
       {
           base.OnLoad(e);
           if (!this.Page.IsPostBack)
           {
               SetC();
           }
       }
       protected override void OnSelectionChanged()
       {
           SetC();
           base.OnSelectionChanged();
           
           
       }
       /// <summary>
       /// 本月被選取的日期（不含周末）
       /// </summary>
       [DefaultValue(null)]
       public List<DateTime> OldSelectedDates
       {
           get
           {
               string strKey = string.Concat(this.ID ,"_OldSelectedDates");
               if (ViewState[strKey] == null)
               {
                   ViewState.Add(strKey, new List<DateTime>());
               }
               return (List<DateTime>)ViewState[strKey];
           }
       }
       private bool blnSatAndSunISSelected=false;
       /// <summary>
       /// 是否默認周末被選取
       /// </summary>
       [DefaultValue(false)]
       public bool SatAndSunISSelected { get { return blnSatAndSunISSelected; } set { blnSatAndSunISSelected = value; } }

       /// <summary>
       /// 設置控制項狀態
       /// </summary>
       private void SetC()
       {
           if (SelectedDate != DateTime.MinValue)
           {
               DateTime d = SelectedDate;
               SelectedDate = DateTime.MinValue;
               if (!OldSelectedDates.Contains(d))
               {
                   OldSelectedDates.Add(d);
               }
               else
               {
                   OldSelectedDates.Remove(d);
               }
           }
               List<DateTime> we = null;
               if (SatAndSunISSelected)
               {
                   we = SetWeekLast(VisibleDate.Year, VisibleDate.Month);
               }

               for (int i = 0; i < OldSelectedDates.Count; i++)
               {
                   SelectedDates.Add(OldSelectedDates[i]);
               }
               if (SatAndSunISSelected && we != null)
               {
                   for (int i = 0; i < we.Count; i++)
                   {
                       if (!SelectedDates.Contains(we[i]))
                       {
                           SelectedDates.Add(we[i]);
                       }
                   }
               }
           
       }

       /// <summary>
       /// 查找本月的周末
       /// </summary>
       /// <param name="year">年</param>
       /// <param name="month">月</param>
       /// <returns>周末日期</returns>
       private List<DateTime> SetWeekLast(int year, int month)
       {
           List<DateTime> ret = new List<DateTime>();
           DateTime fd = new DateTime(year, month, 1);
           int m = fd.Month;
           while (fd.Month == m)
           {
               if (fd.DayOfWeek == DayOfWeek.Saturday || fd.DayOfWeek == DayOfWeek.Sunday)
               {
                   ret.Add(fd);
               }
               fd = fd.AddDays(1);
           }
           return ret;
       }

       private bool blnMustAfterNow = true;
       /// <summary>
       /// 是否不允許維護本月之前的内容
       /// </summary>
       [DefaultValue(true)]
       public bool MustAfterNow { get { return blnMustAfterNow; } set { blnMustAfterNow = value; } }

       protected override void OnVisibleMonthChanged(DateTime newDate, DateTime previousDate)
       {
           base.OnVisibleMonthChanged(newDate, previousDate);
           if (blnMustAfterNow)
           {
               DateTime n = DateTime.Now;
               DateTime td = new DateTime(n.Year, n.Month, 1);
               if (newDate < td)
               {
                   VisibleDate = td;
               }
           }
           MonthChangeEventArgs mceEventArgs = new MonthChangeEventArgs(newDate.Year, newDate.Month, OldSelectedDates);
           if (OnSetDate != default(OnMonthChangeDelegate))
           {
               OnSetDate(this, mceEventArgs);
           }
           SelectedDate = DateTime.MinValue;
           SetC();
           
       }
   }

    /// <summary>
    /// 月變化事件的委託
    /// </summary>
    /// <param name="sender">觸發事件的控制項</param>
    /// <param name="e">事件參數</param>
    public delegate void OnMonthChangeDelegate(object sender, MonthChangeEventArgs e);
    /// <summary>
    /// 事件參數
    /// </summary>
    public class MonthChangeEventArgs : EventArgs
    {
        private int intYear;
        /// <summary>
        /// 年
        /// </summary>
        public int Year { get { return intYear; } set { intYear = value; } }
        private int intMonth;
        /// <summary>
        /// 月
        /// </summary>
        public int Month { get { return intMonth; } set { intMonth = value; } }
        private List<DateTime> listDates = null;

        /// <summary>
        /// 需要填充的已選日期集合
        /// </summary>
        public List<DateTime> DefaultSelectedDates { get { return listDates; } }
        public MonthChangeEventArgs(int year, int month, List<DateTime> dates)
        {
            dates.Clear();
            listDates = dates;
        }
    }
}
