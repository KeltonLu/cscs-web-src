//******************************************************************
//*  �@    �̡GRay
//*  �\�໡���GReport018 Business
//*  �Ыؤ���G2008/12/17
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
/// Summary description for Report018BL
/// </summary>
public class Report018BL:BaseLogic
{
    //�ھڮɶ�,����,�d��ID���o�d�ؼƶq
    const string SEL_CARD_NEW = "select sum(a.Number) from dbo.SUBTOTAL_IMPORT as a inner join dbo.CARD_TYPE as b on (a.TYPE = b.TYPE and a.AFFINITY = b.AFFINITY and a.PHOTO = b.PHOTO) where a.Date_Time=@Date and Action = @Action and b.RID in (@RID)";
    const string SEL_GROUP_NEW = "select sum(a.Number) from dbo.SUBTOTAL_IMPORT as a inner join dbo.CARD_TYPE as b on (a.TYPE = b.TYPE and a.AFFINITY = b.AFFINITY and a.PHOTO = b.PHOTO) where a.Date_Time=@Date and b.RID in ( select CardType_RID from dbo.GROUP_CARD_TYPE where Group_RID = @GroupID) and a.Action = @Action";
    //���o�t�Υd�ƶq(�s���d/���ӥd)
    const string SEL_SYS_CARD = "select c.Number,d.Operate from Factory_Change_Import as c inner join dbo.CARD_TYPE as e on  (c.TYPE = e.TYPE and c.AFFINITY = e.AFFINITY and c.PHOTO = e.PHOTO) inner join (select b.RID,a.Operate from EXPRESSIONS_DEFINE as a inner join CARDTYPE_STATUS as b on a.Type_RID = b.RID where a.RST = 'A' and Expressions_RID = @TypeFlag) as d on c.Status_RID = d.RID where c.Status_RID in (select b.RID from EXPRESSIONS_DEFINE as a left join CARDTYPE_STATUS as b on a.Type_RID = b.RID where Expressions_RID = @TypeFlag) and e.RID in(@RID) and DateDiff(day,c.Date_Time,@Date)=0";
    const string SEL_SYS_CARD_GROUP = "select c.Number,d.Operate from Factory_Change_Import as c inner join dbo.CARD_TYPE as e on  (c.TYPE = e.TYPE and c.AFFINITY = e.AFFINITY and c.PHOTO = e.PHOTO) inner join (select b.RID,a.Operate from EXPRESSIONS_DEFINE as a inner join CARDTYPE_STATUS as b on a.Type_RID = b.RID where a.RST = 'A' and Expressions_RID = @TypeFlag) as d on c.Status_RID = d.RID where c.Status_RID in (select b.RID from EXPRESSIONS_DEFINE as a inner join CARDTYPE_STATUS as b on a.Type_RID = b.RID where a.RST = 'A' and Expressions_RID = @TypeFlag) and e.RID in ( select CardType_RID from dbo.GROUP_CARD_TYPE where Group_RID = @GroupID) and c.Date_Time=@Date ";
    /// <summary>
    /// �s�l�d�ƶq
    /// </summary>
    const string SEL_BAD_CARD = "select  sum(a.Number) from dbo.FACTORY_CHANGE_IMPORT as a inner join dbo.CARD_TYPE as b on  (a.TYPE = b.TYPE and a.AFFINITY = b.AFFINITY and a.PHOTO = b.PHOTO) WHERE a.Status_RID = 9 and a.Date_Time=@Date and b.RID in (@RID)";
    const string SEL_BAD_CARD_GROUP = "select  sum(a.Number) from dbo.FACTORY_CHANGE_IMPORT as a inner join dbo.CARD_TYPE as b on  (a.TYPE = b.TYPE and a.AFFINITY = b.AFFINITY and a.PHOTO = b.PHOTO) WHERE a.Status_RID = 9 and a.Date_Time=@Date and b.RID in ( select CardType_RID from dbo.GROUP_CARD_TYPE where Group_RID = @GroupID)";
    
    const string INSERT_DATA = "insert into RPT_Report018 values(@Month,@Name,@CardID,@Day,@Number)";
    const string SEL_TOTAL_NUMBER = "select sum(number) from dbo.RPT_report018 where Day([day]) = @Day and cardid = @CardID and month <> '13' ";

    const string SEL_EXPRESSIONS_DEFINE = "select count(*) from EXPRESSIONS_DEFINE";

    const string SEL_PROC1 = "proc_Report018CalCardNum_1";

    const string SEL_PROC = "proc_Report018CalCardNum";

    const string DEL_DATA = "delete from RPT_Report018";

    // ���s�ը��d�ؼƶq���`
    public const string SEL_SUBTOTAL_BY_GROUP = "SELECT SI.Date_Time,SI.Action,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "WHERE SI.RST = 'A' AND YEAR(SI.Date_Time) = @year AND CT.RID IN (SELECT CardType_RID FROM GROUP_CARD_TYPE WHERE RST='A' AND Group_RID = @group_rid) "
                        + "GROUP BY SI.Date_Time,SI.Action ";
    // ���s�ը��d�ؼƶq���` 20090702CR
    public const string SEL_SUBTOTAL_BY_GROUP_FACTORY = "SELECT SI.Date_Time,SI.Action,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "WHERE SI.RST = 'A'  AND SI.Perso_Factory_RID =@Perso_Factory_RID  AND YEAR(SI.Date_Time) = @year AND CT.RID IN (SELECT CardType_RID FROM GROUP_CARD_TYPE WHERE RST='A' AND Group_RID = @group_rid) "
                        + "GROUP BY SI.Date_Time,SI.Action ";
    // ���s�ը��d�ؼƶq���`
    public const string SEL_SUBTOTAL_BY_GROUP_3D = "SELECT SI.Date_Time,MCT.Type_Name AS Status_Name,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name IN ('3D','DA','PM','RN') "
                        + "WHERE SI.RST = 'A' AND YEAR(SI.Date_Time) = @year AND CT.RID IN (SELECT CardType_RID FROM GROUP_CARD_TYPE WHERE RST='A' AND Group_RID = @group_rid) "
                        + "GROUP BY SI.Date_Time,MCT.Type_Name ";
    // ���s�ը��d�ؼƶq���`20090702CR
    public const string SEL_SUBTOTAL_BY_GROUP_3D_FACTORY = "SELECT SI.Date_Time,MCT.Type_Name AS Status_Name,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name IN ('3D','DA','PM','RN') "
                        + "WHERE SI.RST = 'A'   AND SI.Perso_Factory_RID =@Perso_Factory_RID  AND YEAR(SI.Date_Time) = @year AND CT.RID IN (SELECT CardType_RID FROM GROUP_CARD_TYPE WHERE RST='A' AND Group_RID = @group_rid) "
                        + "GROUP BY SI.Date_Time,MCT.Type_Name ";
    // ���d��List���d�ؼƶq���`
    public const string SEL_SUBTOTAL_BY_CARD = "SELECT SI.Date_Time,SI.Action,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "WHERE SI.RST = 'A' AND YEAR(SI.Date_Time) = @year AND CT.RID IN ({0}) "
                        + "GROUP BY SI.Date_Time,SI.Action ";
    // ���d��List���d�ؼƶq���` 20090702CR
    public const string SEL_SUBTOTAL_BY_CARD_FACTORY = "SELECT SI.Date_Time,SI.Action,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "WHERE SI.RST = 'A' AND SI.Perso_Factory_RID =@Perso_Factory_RID  AND YEAR(SI.Date_Time) = @year AND CT.RID IN ({0}) "
                        + "GROUP BY SI.Date_Time,SI.Action ";
    // ���d��List���d�ؼƶq���`
    public const string SEL_SUBTOTAL_BY_CARD_3D = "SELECT SI.Date_Time,MCT.Type_Name AS Status_Name,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name IN ('3D','DA','PM','RN') "
                        + "WHERE SI.RST = 'A' AND YEAR(SI.Date_Time) = @year AND CT.RID IN ({0}) "
                        + "GROUP BY SI.Date_Time,MCT.Type_Name ";
    // ���d��List���d�ؼƶq���`  20090702CR
    public const string SEL_SUBTOTAL_BY_CARD_3D_FACTORY = "SELECT SI.Date_Time,MCT.Type_Name AS Status_Name,SUM(SI.Number) AS Sum_Number "
                        + "FROM SUBTOTAL_IMPORT SI INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND SI.TYPE = CT.TYPE AND SI.AFFINITY = CT.AFFINITY AND SI.PHOTO = CT.PHOTO "
                        + "INNER JOIN MAKE_CARD_TYPE MCT ON MCT.RST = 'A' AND SI.MakeCardType_RID = MCT.RID AND MCT.Type_Name IN ('3D','DA','PM','RN') "
                        + "WHERE SI.RST = 'A' AND SI.Perso_Factory_RID =@Perso_Factory_RID AND YEAR(SI.Date_Time) = @year AND CT.RID IN ({0}) "
                        + "GROUP BY SI.Date_Time,MCT.Type_Name ";
    // ���d�ظs�ը��t�Ӳ��ʰT�����`
    public const string SEL_FACTORY_CHANGE_IMPORT_BY_GROUP = "SELECT FCI.Date_Time,CS.Status_Name,SUM(Number) AS Sum_Number "
                        + "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST='A' AND FCI.Status_RID = CS.RID AND (CS.Status_Name NOT IN ('3D','DA','PM','RN')) "
                        + "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO "
                        + "WHERE FCI.RST = 'A' AND YEAR(FCI.Date_Time) = @year AND CT.RID IN (SELECT CardType_RID FROM GROUP_CARD_TYPE WHERE RST='A' AND Group_RID = @group_rid) "
                        + "GROUP BY FCI.Date_Time,CS.Status_Name ";
    // ���d�ظs�ը��t�Ӳ��ʰT�����`20090702CR
    public const string SEL_FACTORY_CHANGE_IMPORT_BY_GROUP_FACTORY = "SELECT FCI.Date_Time,CS.Status_Name,SUM(Number) AS Sum_Number "
                        + "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST='A' AND FCI.Status_RID = CS.RID AND (CS.Status_Name NOT IN ('3D','DA','PM','RN')) "
                        + "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO "
                        + "WHERE FCI.RST = 'A' AND FCI.Perso_Factory_RID =@Perso_Factory_RID  AND YEAR(FCI.Date_Time) = @year AND CT.RID IN (SELECT CardType_RID FROM GROUP_CARD_TYPE WHERE RST='A' AND Group_RID = @group_rid) "
                        + "GROUP BY FCI.Date_Time,CS.Status_Name ";
    // ���d��List���t�Ӳ��ʰT�����`
    public const string SEL_FACTORY_CHANGE_IMPORT_BY_CARD = "SELECT FCI.Date_Time,CS.Status_Name,SUM(Number) AS Sum_Number "
                        + "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST='A' AND FCI.Status_RID = CS.RID AND (CS.Status_Name NOT IN ('3D','DA','PM','RN')) "
                        + "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO "
                        + "WHERE FCI.RST = 'A' AND YEAR(FCI.Date_Time) = @year AND CT.RID IN ({0}) "
                        + "GROUP BY FCI.Date_Time,CS.Status_Name ";
    // ���d��List���t�Ӳ��ʰT�����`20090702CR
    public const string SEL_FACTORY_CHANGE_IMPORT_BY_CARD_FACTORY = "SELECT FCI.Date_Time,CS.Status_Name,SUM(Number) AS Sum_Number "
                        + "FROM FACTORY_CHANGE_IMPORT FCI INNER JOIN CARDTYPE_STATUS CS ON CS.RST='A' AND FCI.Status_RID = CS.RID AND (CS.Status_Name NOT IN ('3D','DA','PM','RN')) "
                        + "INNER JOIN CARD_TYPE CT ON CT.RST = 'A' AND FCI.TYPE = CT.TYPE AND FCI.AFFINITY = CT.AFFINITY AND FCI.PHOTO = CT.PHOTO "
                        + "WHERE FCI.RST = 'A' AND FCI.Perso_Factory_RID =@Perso_Factory_RID  AND YEAR(FCI.Date_Time) = @year AND CT.RID IN ({0}) "
                        + "GROUP BY FCI.Date_Time,CS.Status_Name ";

    // �s���d����
    public const string SEL_EXPRESSIONS_DEFINE_ZC = "SELECT ED.Operate,CS.Status_Name "
                        + "FROM EXPRESSIONS_DEFINE ED INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND ED.Type_RID = CS.RID "
                        + "WHERE ED.RST = 'A' AND ED.Expressions_RID = 1 AND ED.Operate<>'��'";

    // ���ӥd����
    public const string SEL_EXPRESSIONS_DEFINE_XH = "SELECT ED.Operate,CS.Status_Name "
                        + "FROM EXPRESSIONS_DEFINE ED INNER JOIN CARDTYPE_STATUS CS ON CS.RST = 'A' AND ED.Type_RID = CS.RID "
                        + "WHERE ED.RST = 'A' AND ED.Expressions_RID = 2 AND ED.Operate<>'��'";

    // �K�[�s�ɪ�
    public const string INSERT_INTO_RPT_report018 = "INSERT INTO RPT_report018(month,cardname,cardid,day,number,RCT)VALUES(@month,@cardname,@cardid,@day,@number,@RCT)";

    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Report018BL()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string GetLastRiJie()
    {
        string strDate="";
        try
        {
            DataTable dtbl = dao.GetList("select convert(varchar(20),max(stock_date),111) from CARDTYPE_STOCKS").Tables[0];
            if (dtbl.Rows.Count > 0)
            {
                if (!StringUtil.IsEmpty(dtbl.Rows[0][0].ToString()))
                    strDate = dtbl.Rows[0][0].ToString();
            }
        }
        catch
        {
        }
        return strDate;
    }

    /// <summary>
    /// �O�_�w�q����
    /// </summary>
    /// <returns>true:�w�qfalse:�S���w�q</returns>
    public bool IsDefineExpression()
    {

        bool IsDefine = false;
        try
        {
            DataSet dst = null;
            dst = dao.GetList(SEL_EXPRESSIONS_DEFINE);
            if (dst.Tables[0].Rows.Count > 0)
            {
                if (dst.Tables[0].Rows[0][0].ToString() != "0")
                    IsDefine = true;
            }

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return IsDefine;
    }


    #region �p��d���ƶq����
    /// <summary>
    /// ���o�d���ƶq
    /// </summary>
    /// <param name="RID">�d��ID</param>
    /// <param name="Date">���</param>
    /// <param name="CardType">�d������(�s�d�B���ɥd�B���ɥd�B���d)</param>
    /// <returns>��^�d���ƶq�A�S���h��^0</returns>
    private int GetCardNum(string RID, string Date, int CardType)
    {
        try
        {
            string sSQL;
            sSQL = SEL_CARD_NEW;
            sSQL = sSQL.Replace("@RID", RID);
            dirValues.Clear();
            dirValues.Add("Date", Date);
            dirValues.Add("Action", CardType);
            object returnValue = dao.ExecuteScalar(sSQL, dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// �ھڸs�ը��o�d���ƶq
    /// </summary>
    /// <param name="GroupID">�s�ռƶq</param>
    /// <param name="Date">���</param>
    /// <param name="Type">�d���� A�s�d/B���ɥd/C���ɥd/D���d</param>
    /// <returns></returns>
    private int GetCardNum(int GroupID, string Date, int Type)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("GroupID", GroupID);
            dirValues.Add("Date", Date);
            dirValues.Add("Action", Type);
            object returnValue = dao.ExecuteScalar(SEL_GROUP_NEW, dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// ���o�d���`��
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="Type"></param>
    /// <returns></returns>
    private int GetTotalNum(int Date, string Type)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Day", Date);
            dirValues.Add("CardID", Type);
            object returnValue = dao.ExecuteScalar(SEL_TOTAL_NUMBER, dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    #endregion
    #region ���o�d���ƶq
    /// <summary>
    /// ���o�t�Υd�ƶq by CardRID
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Affinity"></param>
    /// <param name="Photo"></param>
    /// <param name="Date"></param>
    /// <param name="CardType"></param>
    /// <returns></returns>
    private int GetSysCardNum(string RID, string Date, int CardType)
    {
        int i = 0;

        try
        {
            string[] strRIDs = RID.Split(',');

            foreach (string strRID in strRIDs)
            {
                DataSet ds = dao.GetList("select dbo.fun_EXPRESSIONS_NUMBER2 ('" + strRID + "','" + Date + "','','" + CardType + "')");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    i += int.Parse(ds.Tables[0].Rows[0][0].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return i;
    }
    /// <summary>
    /// ���o�d�ؼƶq by GroupID
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <param name="CardType"></param>
    /// <returns></returns>
    private int GetSysCardNum(int GroupID, string Date, int CardType)
    {
        int i = 0;
        try
        {
            DataSet dstCardType = dao.GetList("select * from GROUP_CARD_TYPE WHERE GROUP_RID=" + GroupID.ToString());

            foreach (DataRow drow in dstCardType.Tables[0].Rows)
            {
                i += GetSysCardNum(drow["CardType_RID"].ToString(), Date, CardType);
            }

         
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return i;
    }
    /// <summary>
    /// �p��t�Υd�ƶq
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    private int CalcNum(DataSet ds)
    {
        try
        {
            int num = 0;
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["Operate"].ToString() == "+")
                        num = num + Convert.ToInt32(dr["Number"]);
                    if (dr["Operate"].ToString() == "-")
                        num = num - Convert.ToInt32(dr["Number"]);
                }
                return num;
            }
            else
                return 0;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// ���o�s���d�ƶq by CardRID
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    /// <returns></returns>
    private int GetMadeCardNum(string RID, string Date)
    {
        return GetSysCardNum(RID, Date, 1);
    }
    /// <summary>
    /// ���o�s���d�ƶq by GroupID
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetMadeCardNum(int GroupID, string Date)
    {
        return GetSysCardNum(GroupID, Date, 1);
    }
    /// <summary>
    /// ���o���ӥd�ƶq by CardRID
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    /// <returns></returns>
    private int GetSpendCardNum(string RID, string Date)
    {
        return GetSysCardNum(RID, Date, 2);
    }
    /// <summary>
    /// ���o���ӥd�ƶq by GroupID
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetSpendCardNum(int GroupID, string Date)
    {
        return GetSysCardNum(GroupID, Date, 2);
    }
    /// <summary>
    /// �o��s�d�ƶq
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetNewCardNum(string RID, string Date)
    {
        return GetCardNum(RID, Date, 1);
    }
    private int GetNewCardNum(int GroupID, string Date)
    {
        return GetCardNum(GroupID, Date,1);
    }
    /// <summary>
    /// �o�챾�ɥd�ƶq
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetRegCarNum(string RID, string Date)
    {
        return GetCardNum(RID, Date, 2);
    }
    private int GetRegCarNum(int GroupID, string Date)
    {
        return GetCardNum(GroupID, Date, 2);
    }
    /// <summary>
    /// �o�췴�ɥd�ƶq
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetLosCarNum(string RID, string Date)
    {
        return GetCardNum(RID, Date, 3);
    }
    private int GetLosCarNum(int GroupID, string Date)
    {
        return GetCardNum(GroupID, Date, 3);
    }
    /// <summary>
    /// �o�촫�d�ƶq
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetChangeCardNum(string RID, string Date)
    {
        return GetCardNum(RID, Date, 5);
    }
        private int GetChangeCardNum(int GroupID, string Date)
    {
        return GetCardNum(GroupID, Date, 5);
    }
    /// <summary>
    /// ���o�s�l�d�ƶq by CardRID
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    /// <returns></returns>
    private int GetBadCardNum(string RID, string Date)
    {
        try
        {
            string sSQL;
            sSQL = SEL_BAD_CARD;
            sSQL = sSQL.Replace("@RID", RID);
            dirValues.Clear();
            dirValues.Add("Date", Date);
            object returnValue = dao.ExecuteScalar(sSQL, dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// ���o�s�l�d�ƶq by GroupID
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetBadCardNum(int GroupID, string Date)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("GroupID", GroupID);
            dirValues.Add("Date", Date);
            object returnValue = dao.ExecuteScalar(SEL_BAD_CARD_GROUP, dirValues);
            if (Convert.IsDBNull(returnValue) == true)
                return 0;
            else
            {
                return Convert.ToInt32(returnValue);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

    }
    /// <summary>
    /// �s�d�`��
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetTotNewNum(int Date)
    {
        return GetTotalNum(Date, "A");
    }
    /// <summary>
    /// ���ɥd�`��
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetTotRegNum(int Date)
    {
        return GetTotalNum(Date, "B");
    }
    /// <summary>
    /// ���ɥd�`��
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetTotLostNum(int Date)
    {
        return GetTotalNum(Date, "C");
    }
    /// <summary>
    /// ���d�`��
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetTotChangeNum(int Date)
    {
        return GetTotalNum(Date, "D");
    }
    /// <summary>
    /// �s���d
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetTotMadeNum(int Date)
    {
        return GetTotalNum(Date, "E");
    }
    /// <summary>
    /// �s�l�d
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetTotBadNum(int Date)
    {
        return GetTotalNum(Date, "F");
    }
    /// <summary>
    /// ���ӥd
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    private int GetTotSpendNum(int Date)
    {
        return GetTotalNum(Date, "G");
    }
    /// <summary>
    /// ���o�d�ت��ƶq by CardRID
    /// </summary>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <returns>�d�ؼƶq�Ʋ� �s�d/���ɥd/���ɥd/���d/�s���d/�s�l�d/���ӥd</returns>
    public int[] CalcCardNum(string RID, string Date)
    {
        int[] cardNum = new int[7];
        cardNum[0] = GetNewCardNum(RID, Date);
        cardNum[1] = GetRegCarNum(RID, Date);
        cardNum[2] = GetLosCarNum(RID, Date);
        cardNum[3] = GetChangeCardNum(RID, Date);
        cardNum[4] = GetMadeCardNum(RID, Date);
        cardNum[5] = GetBadCardNum(RID, Date);
        cardNum[6] = GetSpendCardNum(RID, Date);
        return cardNum;
    }
    /// <summary>
    /// ���o�d�ؼƶq by GroupID
    /// </summary>
    /// <param name="GroupID"></param>
    /// <param name="Date"></param>
    /// <returns></returns>
    public int[] CalcCardNum(int GroupID, string Date)
    {
        int[] cardNum = new int[7];
        cardNum[0] = GetNewCardNum(GroupID, Date);
        cardNum[1] = GetRegCarNum(GroupID, Date);
        cardNum[2] = GetLosCarNum(GroupID, Date);
        cardNum[3] = GetChangeCardNum(GroupID, Date);
        cardNum[4] = GetMadeCardNum(GroupID, Date);
        cardNum[5] = GetBadCardNum(GroupID, Date);
        cardNum[6] = GetSpendCardNum(GroupID, Date);
        return cardNum;
    }
    /// <summary>
    /// �X�p��
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public int[] CalcTotalNum(int Date)
    {
        int[] cardNum = new int[7];
        cardNum[0] = GetTotNewNum( Date);
        cardNum[1] = GetTotRegNum( Date);
        cardNum[2] = GetTotLostNum(Date);
        cardNum[3] = GetTotChangeNum(Date);
        cardNum[4] = GetTotMadeNum(Date);
        cardNum[5] = GetTotBadNum(Date);
        cardNum[6] = GetTotSpendNum(Date);
        return cardNum;
    }
    #endregion

    /// <summary>
    /// ������ƾڡA���d��List
    /// </summary>
    /// <param name="strYear"></param>
    /// <param name="strGroupRID"></param>
    /// <returns></returns>
    public bool getReportDataByCardList(string strYear, string strCardList, string timeMark, string strFactoryRID)//20090702CR-�W�[��Perso�t���d�߿ﶵ
    {
        try
        {
            // ���p�p�ɰT��
            this.dirValues.Clear();
            this.dirValues.Add("year", strYear);
            //DataSet dstSubTotal = dao.GetList(string.Format(SEL_SUBTOTAL_BY_CARD,strCardList), this.dirValues);

            //DataSet dstSubTotal_3D = dao.GetList(string.Format(SEL_SUBTOTAL_BY_CARD_3D, strCardList), this.dirValues);

            //// ���t�Ӳ��ʰT��
            //DataSet dstFACTORY_CHANGE_IMPORT = dao.GetList(string.Format(SEL_FACTORY_CHANGE_IMPORT_BY_CARD, strCardList), this.dirValues);
            DataSet dstSubTotal = null;
            DataSet dstSubTotal_3D = null;
            DataSet dstFACTORY_CHANGE_IMPORT = null;
            if (strFactoryRID != "")
            {
               this.dirValues.Add("Perso_Factory_RID", strFactoryRID);
               dstSubTotal = dao.GetList(string.Format(SEL_SUBTOTAL_BY_CARD_FACTORY, strCardList), this.dirValues);
               dstSubTotal_3D = dao.GetList(string.Format(SEL_SUBTOTAL_BY_CARD_3D_FACTORY, strCardList), this.dirValues);
               dstFACTORY_CHANGE_IMPORT = dao.GetList(string.Format(SEL_FACTORY_CHANGE_IMPORT_BY_CARD_FACTORY, strCardList), this.dirValues);
            }
            else
            {
                dstSubTotal = dao.GetList(string.Format(SEL_SUBTOTAL_BY_CARD, strCardList), this.dirValues);
                dstSubTotal_3D = dao.GetList(string.Format(SEL_SUBTOTAL_BY_CARD_3D, strCardList), this.dirValues);
                dstFACTORY_CHANGE_IMPORT = dao.GetList(string.Format(SEL_FACTORY_CHANGE_IMPORT_BY_CARD, strCardList), this.dirValues);
            }

            bool blRet = getDataTableReport(strYear, 
                    dstSubTotal.Tables[0], 
                    dstSubTotal_3D.Tables[0],
                    dstFACTORY_CHANGE_IMPORT.Tables[0], timeMark);

            return blRet;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// ������ƾڡA���s��
    /// </summary>
    /// <param name="strYear"></param>
    /// <param name="strGroupRID"></param>
    /// <returns></returns>
    public bool getReportDataByGroupRid(string strYear, string strGroupRID, string time, string strFactoryRID)//20090702CR-�W�[��Perso�t���d�߿ﶵ
    {
        try
        {
            // ���p�p�ɰT��
            this.dirValues.Clear();
            this.dirValues.Add("year", strYear);
            this.dirValues.Add("group_rid", strGroupRID);
            //DataSet dstSubTotal = dao.GetList(SEL_SUBTOTAL_BY_GROUP, this.dirValues);

            //DataSet dstSubTotal_3D = dao.GetList(SEL_SUBTOTAL_BY_GROUP_3D, this.dirValues);

            //// ���t�Ӳ��ʰT��
            //DataSet dstFACTORY_CHANGE_IMPORT = dao.GetList(SEL_FACTORY_CHANGE_IMPORT_BY_GROUP, this.dirValues);
            
            DataSet dstSubTotal = null;
            DataSet dstSubTotal_3D = null;
            DataSet dstFACTORY_CHANGE_IMPORT = null;
            if (strFactoryRID != "")
            {
                this.dirValues.Add("Perso_Factory_RID", strFactoryRID);
                dstSubTotal = dao.GetList(SEL_SUBTOTAL_BY_GROUP_FACTORY, this.dirValues);
                dstSubTotal_3D = dao.GetList(SEL_SUBTOTAL_BY_GROUP_3D_FACTORY, this.dirValues);
                dstFACTORY_CHANGE_IMPORT = dao.GetList(SEL_FACTORY_CHANGE_IMPORT_BY_GROUP_FACTORY, this.dirValues);
            }
            else
            {
                dstSubTotal = dao.GetList(SEL_SUBTOTAL_BY_GROUP, this.dirValues);
                dstSubTotal_3D = dao.GetList(SEL_SUBTOTAL_BY_GROUP_3D, this.dirValues);
                dstFACTORY_CHANGE_IMPORT = dao.GetList(SEL_FACTORY_CHANGE_IMPORT_BY_GROUP, this.dirValues);
            }
            bool blRet = getDataTableReport(strYear, 
                    dstSubTotal.Tables[0], 
                    dstSubTotal_3D.Tables[0],
                    dstFACTORY_CHANGE_IMPORT.Tables[0],time);

            return blRet;

        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// ������ƾڡA��^DataTable
    /// </summary>
    /// <param name="dtSubTotal"></param>
    /// <param name="dtFACTORY_CHANGE_IMPORT"></param>
    /// <returns></returns>
    public bool getDataTableReport(string strYear,
                        DataTable dtSubTotal,
                        DataTable dtSubTotal_3D, 
                        DataTable dtFACTORY_CHANGE_IMPORT,string timeMark)
    {
        try
        {
            // ���s���d����
            DataSet dstZC = dao.GetList(SEL_EXPRESSIONS_DEFINE_ZC);
            DataTable dtZC = null;
            if (dstZC != null && dstZC.Tables.Count > 0)
                dtZC = dstZC.Tables[0];
            else
                return false;

            // �����ӥd����
            DataSet dstXH = dao.GetList(SEL_EXPRESSIONS_DEFINE_XH);
            DataTable dtXH = null;
            if (dstXH != null && dstXH.Tables.Count > 0)
                dtXH = dstXH.Tables[0];
            else
                return false;

            DataTable dtReprotTemp = new DataTable();
            dtReprotTemp.Columns.Add(new DataColumn("month", Type.GetType("System.String")));
            dtReprotTemp.Columns.Add(new DataColumn("cardname", Type.GetType("System.String")));
            dtReprotTemp.Columns.Add(new DataColumn("cardid", Type.GetType("System.String")));
            dtReprotTemp.Columns.Add(new DataColumn("day", Type.GetType("System.String")));
            dtReprotTemp.Columns.Add(new DataColumn("number", Type.GetType("System.Int32")));

            // �w�q�X�p��ƾ�
            int[,] intArrayTotal = new int[7, 31];

            #region �N�p�p�ɰT���K�[���{�ɪ�
            for (int intLoop = 0; intLoop < dtSubTotal.Rows.Count; intLoop++)
            {
                string strAction = dtSubTotal.Rows[intLoop]["Action"].ToString();
                string strTypeName = "";
                string strType = "";
                int intTotal1 = 0;
                switch (strAction)
                {
                    case "1":
                        strTypeName = "A�s�d";
                        strType = "A";
                        intTotal1 = 0;
                        break;
                    case "2":
                        strTypeName = "B���ɥd";
                        strType = "B";
                        intTotal1 = 1;
                        break;
                    case "3":
                        strTypeName = "C���ɥd";
                        strType = "C";
                        intTotal1 = 2;
                        break;
                    case "5":
                        strTypeName = "D���d";
                        strType = "D";
                        intTotal1 = 3;
                        break;
                }

                if (strTypeName.Length > 0)
                {
                    DataRow drNew = dtReprotTemp.NewRow();
                    drNew["month"] = Convert.ToDateTime(dtSubTotal.Rows[intLoop]["Date_Time"]).Month;
                    drNew["cardname"] = strTypeName;
                    drNew["cardid"] = strType;
                    drNew["day"] = Convert.ToDateTime(dtSubTotal.Rows[intLoop]["Date_Time"]).ToString("yyyy/MM/dd");
                    drNew["number"] = Convert.ToInt32(dtSubTotal.Rows[intLoop]["Sum_Number"]);
                    dtReprotTemp.Rows.Add(drNew);

                    // �O�s�X�p�ƾ�
                    //intArrayTotal[intTotal1, Convert.ToDateTime(dtSubTotal.Rows[intLoop]["Date_Time"]).Day - 1] += Convert.ToInt32(dtSubTotal.Rows[intLoop]["Sum_Number"]);
                }
            }
            #endregion �N�p�p�ɰT���K�[���{�ɪ�
            #region �B�z�t�Ӳ��ʰT���ƾڡA�s���d
            // �B�z�s���d�ƶq(�����ܶq)
            for (int intLoop = 0; intLoop < dtZC.Rows.Count; intLoop++)
            {
                DataRow[] drsSelect=null;
                if (dtZC.Rows[intLoop]["Status_Name"].ToString() == "3D" ||
                    dtZC.Rows[intLoop]["Status_Name"].ToString() == "DA" ||
                    dtZC.Rows[intLoop]["Status_Name"].ToString() == "PM" ||
                    dtZC.Rows[intLoop]["Status_Name"].ToString() == "RN")
                {
                    drsSelect = dtSubTotal_3D.Select("Status_Name = '" + dtZC.Rows[intLoop]["Status_Name"].ToString() + "'");
                }
                else
                {
                    drsSelect = dtFACTORY_CHANGE_IMPORT.Select("Status_Name = '" + dtZC.Rows[intLoop]["Status_Name"].ToString() + "'");
                }
                // �d��ۦP�ܶq���t�Ӳ��ʰO��
                for (int intLoop1 = 0; intLoop1 < drsSelect.Length; intLoop1++)
                {
                    // �d��s�ɪ�ت��s���d�O��
                    DataRow[] drsSelect1 = dtReprotTemp.Select("cardname = 'E�s���d' AND day = '" + Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).ToString("yyyy/MM/dd") + "'");
                    if (drsSelect1.Length > 0)
                    {
                        if (dtZC.Rows[intLoop]["Operate"].ToString().Equals("+"))
                        {
                            drsSelect1[0]["number"] = Convert.ToInt32(drsSelect1[0]["number"]) + Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                        else
                        {
                            drsSelect1[0]["number"] = Convert.ToInt32(drsSelect1[0]["number"]) - Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                    }
                    else
                    {
                        DataRow drNew = dtReprotTemp.NewRow();
                        if (dtZC.Rows[intLoop]["Operate"].ToString().Equals("+"))
                        {
                            drNew["month"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Month;
                            drNew["cardname"] = "E�s���d";
                            drNew["cardid"] = "E";
                            drNew["day"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).ToString("yyyy/MM/dd");
                            drNew["number"] = Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                        else
                        {
                            drNew["month"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Month;
                            drNew["cardname"] = "E�s���d";
                            drNew["cardid"] = "E";
                            drNew["day"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).ToString("yyyy/MM/dd");
                            drNew["number"] = 0 - Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                        dtReprotTemp.Rows.Add(drNew);
                    }

                    //if (dtZC.Rows[intLoop]["Operate"].ToString().Equals("+"))
                    //{
                    //    // �O�s�X�p�ƾ�
                    //    intArrayTotal[4, Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Day - 1] += Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                    //}
                    //else
                    //{
                    //    intArrayTotal[4, Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Day - 1] -= Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                    //}
                }
            }
            #endregion �B�z�t�Ӳ��ʰT���ƾ�
            #region �B�z�t�Ӳ��ʰT���ƾڡA���ӥd
            // �B�z���ӥd�ƶq(�����ܶq)
            for (int intLoop = 0; intLoop < dtXH.Rows.Count; intLoop++)
            {
                // �d��ۦP�ܶq���t�Ӳ��ʰO��
                DataRow[] drsSelect = null;
                if (dtXH.Rows[intLoop]["Status_Name"].ToString() == "3D" ||
                    dtXH.Rows[intLoop]["Status_Name"].ToString() == "DA" ||
                    dtXH.Rows[intLoop]["Status_Name"].ToString() == "PM" ||
                    dtXH.Rows[intLoop]["Status_Name"].ToString() == "RN")
                {
                    drsSelect = dtSubTotal_3D.Select("Status_Name = '" + dtXH.Rows[intLoop]["Status_Name"].ToString() + "'");
                }
                else
                {
                    drsSelect = dtFACTORY_CHANGE_IMPORT.Select("Status_Name = '" + dtXH.Rows[intLoop]["Status_Name"].ToString() + "'");
                }
                for (int intLoop1 = 0; intLoop1 < drsSelect.Length; intLoop1++)
                {
                    // �d��s�ɪ�ت��s���d�O��
                    DataRow[] drsSelect1 = dtReprotTemp.Select("cardname = 'G���ӥd' AND day = '" + Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).ToString("yyyy/MM/dd") + "'");
                    if (drsSelect1.Length > 0)
                    {
                        if (dtXH.Rows[intLoop]["Operate"].ToString().Equals("+"))
                        {
                            drsSelect1[0]["number"] = Convert.ToInt32(drsSelect1[0]["number"]) + Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                        else
                        {
                            drsSelect1[0]["number"] = Convert.ToInt32(drsSelect1[0]["number"]) - Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                    }
                    else
                    {
                        DataRow drNew = dtReprotTemp.NewRow();
                        if (dtXH.Rows[intLoop]["Operate"].ToString().Equals("+"))
                        {
                            drNew["month"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Month;
                            drNew["cardname"] = "G���ӥd";
                            drNew["cardid"] = "G";
                            drNew["day"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).ToString("yyyy/MM/dd");
                            drNew["number"] = Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                        else
                        {
                            drNew["month"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Month;
                            drNew["cardname"] = "G���ӥd";
                            drNew["cardid"] = "G";
                            drNew["day"] = Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).ToString("yyyy/MM/dd");
                            drNew["number"] = 0 - Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                        }
                        dtReprotTemp.Rows.Add(drNew);
                    }

                    //if (dtXH.Rows[intLoop]["Operate"].ToString().Equals("+"))
                    //{
                    //    // �O�s�X�p�ƾ�
                    //    intArrayTotal[6, Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Day - 1] += Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                    //}
                    //else
                    //{
                    //    intArrayTotal[6, Convert.ToDateTime(drsSelect[intLoop1]["Date_Time"]).Day - 1] -= Convert.ToInt32(drsSelect[intLoop1]["Sum_Number"]);
                    //}
                }
            }
            #endregion �B�z�t�Ӳ��ʰT���ƾ�
            #region �B�z�t�Ӳ��ʰT���ƾڡA��l�d
            // �d��ۦP�ܶq���t�Ӳ��ʰO��
            DataRow[] drsSelectZS = dtFACTORY_CHANGE_IMPORT.Select("Status_Name = '�s�l�d'");
            for (int intLoop = 0; intLoop < drsSelectZS.Length; intLoop++)
            {
                DataRow drNew = dtReprotTemp.NewRow();
                drNew["month"] = Convert.ToDateTime(drsSelectZS[intLoop]["Date_Time"]).Month;
                drNew["cardname"] = "F�s�l�d";
                drNew["cardid"] = "F";
                drNew["day"] = Convert.ToDateTime(drsSelectZS[intLoop]["Date_Time"]).ToString("yyyy/MM/dd");
                drNew["number"] = Convert.ToInt32(drsSelectZS[intLoop]["Sum_Number"]);
                dtReprotTemp.Rows.Add(drNew);

                // �O�s�X�p�ƾ�
                //intArrayTotal[5, Convert.ToDateTime(drsSelectZS[intLoop]["Date_Time"]).Day - 1] += Convert.ToInt32(drsSelectZS[intLoop]["Sum_Number"]);
            }
            #endregion �B�z�t�Ӳ��ʰT���ƾ�
            #region �N�s�ɼƾګO�s������LTable�]RPT_report018�^���A
            dao.OpenConnection();

            // �R���s�ɪ��ƾ�
            dao.ExecuteNonQuery(DEL_DATA+" where RCT<'"+DateTime.Now.ToString("yyyyMMdd000000")+"'");

            DateTime dtStartDay = Convert.ToDateTime(strYear + "/01/01");
            while (dtStartDay.Year == int.Parse(strYear))
            {
                // �s�d
                DataRow[] drSelectInsert = dtReprotTemp.Select("cardname = 'A�s�d' AND day = '" + dtStartDay.ToString("yyyy/MM/dd") + "'");
                if (drSelectInsert.Length > 0)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "A�s�d");
                    this.dirValues.Add("cardid", "A");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", drSelectInsert[0]["number"]);
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "A�s�d");
                    this.dirValues.Add("cardid", "A");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", "0");
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }

                // ���ɥd
                drSelectInsert = dtReprotTemp.Select("cardname = 'B���ɥd' AND day = '" + dtStartDay.ToString("yyyy/MM/dd") + "'");
                if (drSelectInsert.Length > 0)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "B���ɥd");
                    this.dirValues.Add("cardid", "B");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", drSelectInsert[0]["number"]);
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "B���ɥd");
                    this.dirValues.Add("cardid", "B");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", "0");
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }

                // ���ɥd
                drSelectInsert = dtReprotTemp.Select("cardname = 'C���ɥd' AND day = '" + dtStartDay.ToString("yyyy/MM/dd") + "'");
                if (drSelectInsert.Length > 0)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "C���ɥd");
                    this.dirValues.Add("cardid", "C");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", drSelectInsert[0]["number"]);
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "C���ɥd");
                    this.dirValues.Add("cardid", "C");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", "0");
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }

                // ���d
                drSelectInsert = dtReprotTemp.Select("cardname = 'D���d' AND day = '" + dtStartDay.ToString("yyyy/MM/dd") + "'");
                if (drSelectInsert.Length > 0)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "D���d");
                    this.dirValues.Add("cardid", "D");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", drSelectInsert[0]["number"]);
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "D���d");
                    this.dirValues.Add("cardid", "D");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", "0");
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }

                // �s���d
                drSelectInsert = dtReprotTemp.Select("cardname = 'E�s���d' AND day = '" + dtStartDay.ToString("yyyy/MM/dd") + "'");
                if (drSelectInsert.Length > 0)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "E�s���d");
                    this.dirValues.Add("cardid", "E");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", drSelectInsert[0]["number"]);
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "E�s���d");
                    this.dirValues.Add("cardid", "E");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", "0");
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }

                drSelectInsert = dtReprotTemp.Select("cardname = 'F�s�l�d' AND day = '" + dtStartDay.ToString("yyyy/MM/dd") + "'");
                if (drSelectInsert.Length > 0)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "F�s�l�d");
                    this.dirValues.Add("cardid", "F");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", drSelectInsert[0]["number"]);
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "F�s�l�d");
                    this.dirValues.Add("cardid", "F");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", "0");
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }

                drSelectInsert = dtReprotTemp.Select("cardname = 'G���ӥd' AND day = '" + dtStartDay.ToString("yyyy/MM/dd") + "'");
                if (drSelectInsert.Length > 0)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "G���ӥd");
                    this.dirValues.Add("cardid", "G");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", drSelectInsert[0]["number"]);
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }
                else
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", dtStartDay.Month);
                    this.dirValues.Add("cardname", "G���ӥd");
                    this.dirValues.Add("cardid", "G");
                    this.dirValues.Add("day", dtStartDay.ToString("yyyy/MM/dd"));
                    this.dirValues.Add("number", "0");
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                }

                // �p��U�@��
                dtStartDay = dtStartDay.AddDays(1);
            }

            // �K�[�X�p
            //for (int intLoop = 0; intLoop < 7; intLoop++)
            //{
            //    string strTypeName = "";
            //    string strType = "";
            //    switch (intLoop)
            //    {
            //        case 0:
            //            strTypeName = "A�s�d";
            //            strType = "A";
            //            break;
            //        case 1:
            //            strTypeName = "B���ɥd";
            //            strType = "B";
            //            break;
            //        case 2:
            //            strTypeName = "C���ɥd";
            //            strType = "C";
            //            break;
            //        case 3:
            //            strTypeName = "D���d";
            //            strType = "D";
            //            break;
            //        case 4:
            //            strTypeName = "E�s���d";
            //            strType = "E";
            //            break;
            //        case 5:
            //            strTypeName = "F�s�l�d";
            //            strType = "F";
            //            break;
            //        case 6:
            //            strTypeName = "G���ӥd";
            //            strType = "G";
            //            break;
            //    }

            //    for (int intLoop1 = 0; intLoop1 < 31; intLoop1++)
            //    {
            //        string strDay = "";
            //        int intDay = intLoop1+1;
            //        if (intDay<10)
            //            strDay = "0" + intDay.ToString();
            //        else
            //            strDay = intDay.ToString();
            //        this.dirValues.Clear();
            //        this.dirValues.Add("month", "13");
            //        this.dirValues.Add("cardname", strTypeName);
            //        this.dirValues.Add("cardid", strType);
            //        this.dirValues.Add("day", strYear + "/01/" + strDay);
            //        this.dirValues.Add("number", intArrayTotal[intLoop, intLoop1]);
            //        dirValues.Add("RCT", timeMark);
            //        dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
            //    }
            //}
            DataSet dsTotal = GetTotal(timeMark);
            if (dsTotal != null && dsTotal.Tables.Count > 0 && dsTotal.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drow in dsTotal.Tables[0].Rows)
                {
                    this.dirValues.Clear();
                    this.dirValues.Add("month", "13");
                    this.dirValues.Add("cardname", drow["cardname"].ToString());
                    this.dirValues.Add("cardid", drow["cardid"].ToString());
                    this.dirValues.Add("day", drow["day"].ToString());
                    this.dirValues.Add("number", drow["number"].ToString());
                    dirValues.Add("RCT", timeMark);
                    dao.ExecuteNonQuery(INSERT_INTO_RPT_report018, this.dirValues);
                   
                }
                    
            }

            dao.Commit();
            #endregion �N�s�ɼƾګO�s������LTable�]RPT_report018�^��

            return true;
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        finally {
            dao.CloseConnection();
        }
    }


    public void ExecData(string Year, string RID,bool IsCard)
    {
        try
        {
            dao.OpenConnection();
            int month;
            int i;
            string monthFormat;
            ClearData();
            for (month = 1; month <= 12; month++)
            {
                int MonthEnd = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(month));
                string Date;
                for (i = 1; i <= MonthEnd; i++)
                {
                    Date = Year.ToString() + "/" + month.ToString() + "/" + i.ToString();
                    monthFormat = "0" + month.ToString();
                    monthFormat = monthFormat.Substring(monthFormat.Length - 2, 2);
                    if (IsCard) //��ܥd��
                        InsertCardData(monthFormat, Convert.ToDateTime(Date).ToString("yyyy-MM-dd"), RID);
                    else        //��ܸs��
                        InsertCardData(monthFormat, Convert.ToDateTime(Date).ToString("yyyy-MM-dd"), Convert.ToInt32(RID));
                }
            }
            //�X�p
            for (i = 1; i <= 31; i++)
            {
                InsertTotalData(i);
            }

            dao.Commit();
        }
        catch
        {
            dao.Rollback();
        }
        finally
        {
            dao.CloseConnection();
        }
    }
    /// <summary>
    /// ���J�d���ƶq���
    /// </summary>
    /// <param name="GroupOrCard"></param>
    /// <param name="Name"></param>
    /// <param name="RID"></param>
    /// <param name="Date"></param>
    /// <param name="cardNum"></param>
    private void InsertData(string Month, string Date, int[] cardNum)
    {
        try
        {
            int i;
            string CardType;
            string CardID;
            for (i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0:
                        CardType = "A�s�d";
                        CardID = "A";
                        break;
                    case 1:
                        CardType = "B���ɥd";
                        CardID = "B";
                        break;
                    case 2:
                        CardType = "C���ɥd";
                        CardID = "C";
                        break;
                    case 3:
                        CardType = "D���d";
                        CardID = "D";
                        break;
                    case 4:
                        CardType = "E�s���d";
                        CardID = "E";
                        break;
                    case 5:
                        CardType = "F�s�l�d";
                        CardID = "F";
                        break;
                    case 6:
                        CardType = "G���ӥd";
                        CardID = "G";
                        break;
                    default:
                        CardType = "H�����d��";
                        CardID = "H";
                        break;

                }
                Date = Convert.ToDateTime(Date).ToString("yyyy/MM/dd");
                dirValues.Clear();
                dirValues.Add("Month", Month);//�d��
                dirValues.Add("Name", CardType);
                dirValues.Add("CardID", CardID);
                dirValues.Add("Day", Date);
                dirValues.Add("Number", cardNum[i]);
                int returnValue = dao.ExecuteNonQuery(INSERT_DATA, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    private void InsertData(string Month, int Date, int[] cardNum)
    {
        try
        {
            int i;
            string CardType;
            string CardID;
            string Datetime;
            for (i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0:
                        CardType = "A�s�d";
                        CardID = "A";
                        break;
                    case 1:
                        CardType = "B���ɥd";
                        CardID = "B";
                        break;
                    case 2:
                        CardType = "C���ɥd";
                        CardID = "C";
                        break;
                    case 3:
                        CardType = "D���d";
                        CardID = "D";
                        break;
                    case 4:
                        CardType = "E�s���d";
                        CardID = "E";
                        break;
                    case 5:
                        CardType = "F�s�l�d";
                        CardID = "F";
                        break;
                    case 6:
                        CardType = "G���ӥd";
                        CardID = "G";
                        break;
                    default:
                        CardType = "H�����d��";
                        CardID = "H";
                        break;

                }
                Datetime = DateTime.Now.Year.ToString() + "/01/" + Date.ToString();
                Datetime = Convert.ToDateTime(Datetime).ToString("yyyy/MM/dd");
                dirValues.Clear();
                dirValues.Add("Month", Month);//�d��
                dirValues.Add("Name", CardType);
                dirValues.Add("CardID", CardID);
                dirValues.Add("Day", Datetime);
                dirValues.Add("Number", cardNum[i]);
                int returnValue = dao.ExecuteNonQuery(INSERT_DATA, dirValues);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// ���J�d�ؼƶq��� by CardRID
    /// </summary>
    /// <param name="Date"></param>
    /// <param name="RID"></param>
    private void InsertCardData(string Month,string Date, string RID)
    {
        int[] cardNum;
        cardNum = CalcCardNum(RID, Date);

        InsertData(Month,Date,cardNum);
    }
    /// <summary>
    /// ���J�X�p��
    /// </summary>
    /// <param name="Date"></param>
    private void InsertTotalData(int Date)
    {
        int[] cardNum;
        cardNum = CalcTotalNum(Date);
        InsertData("13", Date, cardNum);
 
    }
    /// <summary>
    /// ���J�d�ؼƶq��� by GroupID
    /// </summary>
    /// <param name="Month"></param>
    /// <param name="Date"></param>
    /// <param name="GroupID"></param>
    private void InsertCardData(string Month, string Date, int GroupID)
    {
        int[] cardNum;
        cardNum = CalcCardNum(GroupID, Date);

        InsertData(Month, Date, cardNum);
    }
   
    /// <summary>
    /// �M�ų�����
    /// </summary>
    /// <returns></returns>
    private int ClearData()
    {
        try
        {
            return dao.ExecuteNonQuery(DEL_DATA);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }
    /// <summary>
    /// ���u�@������X�p
    /// </summary>
    /// <returns></returns>
    private DataSet GetTotal(string RCT)
    {
        try
        {
            string SQL=" select cardname,cardid,day,sum(number) as number from ( "
                    +" select a.month,a.cardname,a.cardid,'1900/01/'+substring(a.day,len(a.day)-1,2) as day, "
                    +" case wd.Is_workDay when 'N' then 0 else a.number end as number from dbo.RPT_report018 a "
                    +" left join WORK_DATE wd on a.day=Convert(varchar(20),wd.date_time,111) "
                    +" where a.RCT=@RCT and month <>13 ) b "
                    +" group by cardname,cardid,day ";
                dirValues.Clear();
                dirValues.Add("RCT", RCT);
                DataSet dstTotal = dao.GetList(SQL, dirValues);
                return dstTotal;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

}
