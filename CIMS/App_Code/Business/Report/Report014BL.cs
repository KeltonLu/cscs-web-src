//******************************************************************
//*  �@    �̡GRay
//*  �\�໡���GReport014 Business
//*  �Ыؤ���G2008/12/9
//*  �ק����G
//*  �ק�O���G
//*            
//*******************************************************************
using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections;
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
/// Summary description
/// </summary>
public class Report014 : BaseLogic
{
    public Report014()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    const string SEL_SUBTOTAL_CHECKED = "select  max(stock_date) from dbo.CARDTYPE_STOCKS where stock_date >= @DateS and stock_date <= @DateE"; //�p�p�ɤ鵲

    const string SEL_SUBTOTAL_CHECKED1 = "select count(*) from (select distinct stock_date from dbo.CARDTYPE_STOCKS where stock_date >= @DateS and stock_date <= @DateE) a"; //�p�p�ɤ鵲

    const string SEL_SUBTOTAL_CHECKED2 = "select count(*)  from Work_Date where RST = 'A' and Is_WorkDay = 'Y' and (Date_Time >= @DateS and Date_Time <= @DateE)"; //�p�p�ɤ鵲


    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    /// <summary>
    /// �P�_����϶�������ƬO�_�w�鵲
    /// </summary>
    /// <param name="DateS"></param>
    /// <param name="DateE"></param>
    /// <returns>���w�鵲��^�ŭȡA�_�h��^���~�T��</returns>
    public string IsChecked(string DateS,string DateE)
    {
        string strChecked = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("DateS", DateS);
            dirValues.Add("DateE", DateE);

            object returnValue1 = dao.ExecuteScalar(SEL_SUBTOTAL_CHECKED1, dirValues);
            if (returnValue1.ToString() == "0")
                strChecked= "�ҿ�϶����鵲";

            object returnValue2 = dao.ExecuteScalar(SEL_SUBTOTAL_CHECKED2, dirValues);
            if (returnValue1.ToString() == returnValue2.ToString())
                strChecked= "";
            else
            {
                object returnValue = dao.ExecuteScalar(SEL_SUBTOTAL_CHECKED, dirValues);
                if (returnValue != null)
                {

                    strChecked = Convert.ToDateTime(returnValue).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "�᥼�鵲";
                }
                
            }
        }
        catch
        {
        }
        return strChecked;
            
    }

}
