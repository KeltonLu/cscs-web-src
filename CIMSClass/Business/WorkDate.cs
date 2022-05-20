using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using CIMSClass;
using CIMSClass.Business;
using CIMSClass.Mail;
using CIMSClass.Model;

namespace CIMSClass.Business
{
    class WorkDate : BaseLogic
    {
        #region SQL�w�q
        const string SEL_WORK_DATE = "select count (Date_Time) from WORK_DATE where Date_Time = @Date_Time and Is_WorkDay='Y'";
        public const string SEL_NEXT_WORK_DATE = "SELECT TOP 1 Date_Time " +
                           "FROM WORK_DATE " +
                           "WHERE Date_Time > @date_time AND Is_WorkDay='Y' " +
                           "ORDER BY Date_Time ASC";
        public const string SEL_LAST_WORK_DATE = "SELECT TOP 1 Date_Time " +
                            "FROM WORK_DATE " +
                            "WHERE Date_Time < @date_time AND Is_WorkDay='Y' " +
                            "ORDER BY Date_Time DESC";
        #endregion
        Dictionary<string, object> dirValues = new Dictionary<string, object>();
        /// <summary>
        /// �P�_����O�_���u�@��
        /// </summary>
        /// <param name="WorkDate"></param>
        /// <returns>���u�@���^true</returns>
        public bool CheckWorkDate(DateTime WorkDate)
        {
            try
            {
                Dictionary<string, object> dirValues = new Dictionary<string, object>();
                dirValues.Add("Date_Time", WorkDate.ToString("yyyy-MM-dd 00:00:00:000"));
                object returnValue = dao.ExecuteScalar(SEL_WORK_DATE, dirValues);
                if (Convert.ToInt32(returnValue) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogFactory.Write("����u�@��H��CheckWorkDate���~�G" + ex.ToString(), GlobalString.LogType.ErrorCategory);
                return false;
            }

        }
        /// <summary>
        /// ����e�Ѫ��W�@�u�@��
        /// </summary>
        public DateTime GetLastWorkDate(DateTime SurplusDate)
        {

            try
            {

                DateTime dtLastWorkDate = DateTime.Parse("1900-01-01");
                dirValues.Clear();
                dirValues.Add("date_time", SurplusDate.ToString("yyyy/MM/dd"));
                DataSet dsWorkDate = dao.GetList(SEL_LAST_WORK_DATE, this.dirValues);
                if (null != dsWorkDate &&
                    dsWorkDate.Tables.Count > 0 &&
                    dsWorkDate.Tables[0].Rows.Count > 0)
                {
                    dtLastWorkDate = Convert.ToDateTime(dsWorkDate.Tables[0].Rows[0]["Date_Time"].ToString());
                }
                return dtLastWorkDate;

            }
            catch (Exception ex)
            {
                // Legend 2018/4/13 �ɥRLog���e
                LogFactory.Write("����e�Ѫ��W�@�u�@��, GetLastWorkDate����: " + ex.ToString(), GlobalString.LogType.ErrorCategory);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// ����e�Ѫ��W�@�u�@��
        /// </summary>
        public DateTime GetNextWorkDate(DateTime SurplusDate)
        {

            try
            {

                DateTime dtLastWorkDate = DateTime.Parse("1900-01-01");
                dirValues.Clear();
                dirValues.Add("date_time", SurplusDate.ToString("yyyy/MM/dd"));
                DataSet dsWorkDate = dao.GetList(SEL_NEXT_WORK_DATE, this.dirValues);
                if (null != dsWorkDate &&
                    dsWorkDate.Tables.Count > 0 &&
                    dsWorkDate.Tables[0].Rows.Count > 0)
                {
                    dtLastWorkDate = Convert.ToDateTime(dsWorkDate.Tables[0].Rows[0]["Date_Time"].ToString());
                }
                return dtLastWorkDate;

            }
            catch (Exception ex)
            {
                // Legend 2018/4/13 �ɥRLog���e
                LogFactory.Write("����e�Ѫ��W�@�u�@��, GetNextWorkDate����:" + ex.ToString(), GlobalString.LogType.ErrorCategory);
                throw new Exception(ex.Message);
            }
        }
       
    }
}
