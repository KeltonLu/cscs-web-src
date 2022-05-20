//******************************************************************
//*  作    者：JunWang
//*  功能說明：營業日期資料管理邏輯
//*  創建日期：2008-08
//*  修改日期：2008-08
//*  修改記錄：
//*            □2008-08
//*              1.創建 王俊
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
using System.Collections;
/// <summary>
/// BaseInfo007BL 的摘要描述
/// </summary>
public class BaseInfo007BL : BaseLogic
{
    #region SQL語句
    public const string CON_CARDTYPE_SURPLUS = "SELECT Stock_Date "
                                        + "FROM CARDTYPE_STOCKS "
                                        + "WHERE RST='A' AND Stock_Date = @stock_date";
    #endregion
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public BaseInfo007BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    /// <summary>
    /// 保存工作日
    /// </summary>
    /// <param name="calWorkDate">工作日集合</param> 
    //public void Save(Calendar calWorkDate)
    public void Save(List<DateTime> SelectedDates, Calendar calendar)
    {
        WORK_DATE wdModel = new WORK_DATE();
        try
        {
            //事務開始
            dao.OpenConnection();
            DateTime dt1 = Convert.ToDateTime("1900/01/01");
            if (SelectedDates.Count != 0)
            {
                dt1 = SelectedDates[0];
            }
            else
            {
                dt1 = calendar.VisibleDate;
            }
            dao.ExecuteNonQuery("delete from WORK_DATE where Date_Time between '" + dt1.ToString("yyyy/MM/01") + "' and dateadd(day,-1,'" + dt1.AddMonths(1).ToString("yyyy/MM/01") + "') ");
            DateTime Temp = Convert.ToDateTime(dt1.ToString("yyyy/MM/01"));
            DateTime Temp1 = Temp.AddMonths(1);
            TimeSpan Temp2 = Temp1 - Temp;//下一個月的一號減去上一個月的一號，得到當月總天數
            #region 雙循環方法保存
            //保存休息日
            //bool IsWorkDay = true;
            //foreach (DateTime dt in SelectedDates)
            //{
            //    wdModel.Date_Time = dt;
            //    wdModel.Is_WorkDay = "N";
            //    dao.Add<WORK_DATE>(wdModel, "RID");
            //}
            //工作日期的保存
            //for (int i = 0; i < Temp2.Days; i++)
            //{
            //    foreach (DateTime dt in SelectedDates)
            //    {
            //        if (Temp.AddDays(i).ToString("yyyy/MM/dd") == dt.ToString("yyyy/MM/dd"))
            //        {
            //            IsWorkDay = false;
            //            break;
            //        }
            //    }
            //    if (IsWorkDay)
            //    {
            //        wdModel = new WORK_DATE();
            //        wdModel.Date_Time = Temp.AddDays(i);
            //        wdModel.Is_WorkDay = "Y";
            //        dao.Add<WORK_DATE>(wdModel, "RID");
            //    }
            //    IsWorkDay = true;
            //}
            #endregion
            for (int i = 0; i < Temp2.Days; i++)
            {
                //false保存休息日, true工作日期的保存
                if (SelectedDates.Contains(Convert.ToDateTime(Temp.AddDays(i).ToString("yyyy/MM/dd"))))
                {
                    wdModel = new WORK_DATE();
                    wdModel.Date_Time = Temp.AddDays(i);
                    wdModel.Is_WorkDay = "N";
                    dao.Add<WORK_DATE>(wdModel, "RID");
                }
                else
                {
                    wdModel = new WORK_DATE();
                    wdModel.Date_Time = Temp.AddDays(i);
                    wdModel.Is_WorkDay = "Y";
                    dao.Add<WORK_DATE>(wdModel, "RID");
                }
            }

            //操作日誌
            SetOprLog("3");

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    ///// <summary>
    ///// 修改後无休息日的保存
    ///// </summary>
    ///// <param name="calendar">日历对象</param> 
    ////public void Save(Calendar calWorkDate)
    //public void ChangeSave(Calendar calendar)
    //{
    //    WORK_DATE wdModel = new WORK_DATE();

    //    try
    //    {
    //        //事務開始
    //        dao.OpenConnection();
    //        DateTime dt1 = calendar.VisibleDate;
    //        dao.ExecuteNonQuery("delete from WORK_DATE where Date_Time between '" + dt1.ToString("yyyy/MM/01") + "' and dateadd(day,-1,'" + dt1.AddMonths(1).ToString("yyyy/MM/01") + "') ");

    //        //事務提交
    //        dao.Commit();
    //    }
    //    catch (Exception ex)
    //    {
    //        //事務回滾
    //        dao.Rollback();
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
    //    }
    //    finally
    //    {
    //        //關閉連接
    //        dao.CloseConnection();
    //    }
    //}

    /// <summary>
    /// 查詢工作日
    /// </summary>
    /// <param name="datatime">時間</param> 
    public DataSet QueryByDate(string datatime)
    {
        string tempBeginDate_Time = datatime.Substring(0, 4) + "/" + datatime.Substring(4, 2) + "/" + "1";
        DateTime tempEndDate_Time = Convert.ToDateTime(tempBeginDate_Time).AddMonths(1);

        DataSet dsQueryByDate = dao.GetList("SELECT Date_Time FROM  WORK_DATE WHERE RST = 'A' AND Is_WorkDay = 'N' AND Date_Time between '" + tempBeginDate_Time + "' AND dateadd(day,-1,'" + tempEndDate_Time.ToString("yyyy/MM/dd") + "') ");
        return dsQueryByDate;
    }


    /// <summary>
    /// 是否日結
    /// </summary>
    /// <returns>string[日結日期]</returns>
    public string Is_Check(DateTime SelectedDates)
    {
        DataSet dsIs_Check = null;
        DateTime TempDate;
        string Date = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("stock_date", SelectedDates);
            dsIs_Check = dao.GetList(CON_CARDTYPE_SURPLUS, dirValues);
            if (dsIs_Check.Tables[0].Rows.Count != 0)
            {
                TempDate = Convert.ToDateTime(dsIs_Check.Tables[0].Rows[0]["Stock_Date"]);
                Date = TempDate.ToShortDateString();
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }

        return Date;
    }
}

