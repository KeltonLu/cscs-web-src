//******************************************************************
//*  作    者：bingyipan
//*  功能說明：紙品物料月耗用、日耗用預測報表 
//*  創建日期：2008-12-02
//*  修改日期：2008-12-16 15:00
//*  修改記錄：
//*            □2008-12-16
//*              1.創建 潘秉奕
//*******************************************************************
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Report032BL 的摘要描述
/// </summary>
public class Report032BL : BaseLogic
{
    #region SQL語句
    public const string SEL_ENVELOPE = "select * from ENVELOPE_INFO where rst='A'";
    public const string SEL_EXPONENT = "select * from CARD_EXPONENT where rst='A'";
    public const string SEL_DMTYPE = "select * from DMTYPE_INFO where rst='A'";
    public const string SEL_ACTION = "select * from param where rst='A' and (paramtype_name='預估過去X1月'"
+" or paramtype_name='預估過去X2月' or paramtype_name='預估過去X3月')";
    public const string SEL_ACTION2 = "select * from param where rst='A' and (paramtype_name='預估預估過去Y1個工作日'"
+ " or paramtype_name='預估預估過去Y2個工作日' or paramtype_name='預估預估過去Y3個工作日')";
    public const string SEL_EnvelopeCount = "select distinct ct.envelope_rid mid from CARD_TYPE ct"
    + " left join PERSO_CARDTYPE pc on pc.rst='A' and pc.cardtype_rid = ct.rid and pc.factory_rid=@perso_facotry"
    + " where ct.rst='A' and ct.envelope_rid is not Null and ct.envelope_rid<>'0'";
    public const string SEL_ExponentCount = "select distinct ct.exponent_rid mid from CARD_TYPE ct"
    + " left join PERSO_CARDTYPE pc on pc.rst='A' and pc.cardtype_rid = ct.rid and pc.factory_rid=@perso_facotry"
    + " where ct.rst='A' and ct.exponent_rid is not Null and ct.exponent_rid<>'0'";
    public const string SEL_DmCount = "select distinct dc.dm_rid mid from DM_CARDTYPE dc"
    + " left join PERSO_CARDTYPE pc on pc.cardtype_rid=dc.cardtype_rid and pc.rst='A' and pc.factory_rid=@perso_facotry "
    + " where dc.rst='A' and dc.dm_rid<>'0' and dc.cardtype_rid is not Null and dc.dm_rid is not Null ";
    public const string SEL_EnvelopeCount2 = "select distinct ct.envelope_rid mid from CARD_TYPE ct"
    + " left join PERSO_CARDTYPE pc on pc.rst='A' and pc.cardtype_rid = ct.rid "
    + " where ct.rst='A' and ct.envelope_rid is not Null and ct.envelope_rid<>'0'";
    public const string SEL_ExponentCount2 = "select distinct ct.exponent_rid mid from CARD_TYPE ct"
    + " left join PERSO_CARDTYPE pc on pc.rst='A' and pc.cardtype_rid = ct.rid "
    + " where ct.rst='A' and ct.exponent_rid is not Null and ct.exponent_rid<>'0'";
    public const string SEL_DmCount2 = "select distinct dc.dm_rid mid from DM_CARDTYPE dc"
    + " left join PERSO_CARDTYPE pc on pc.cardtype_rid=dc.cardtype_rid and pc.rst='A' "
    + " where dc.rst='A' and dc.dm_rid<>'0' and dc.cardtype_rid is not Null and dc.dm_rid is not Null ";
    public const string SEL_Proc_ALLMaterial = "proc_report032";
    public const string SEL_Proc_Material = "proc_report032_2";
    public const string SEL_DAYLY_MONITOR = "select COUNT(*) from DAYLY_MONITOR WHERE RUT > @CDay";
    public const string SEL_MONTHLY_MONITOR = "select COUNT(*) from MONTHLY_MONITOR WHERE rut > @CDay";
    public const string IS_GENERATE = "select count(*) from MONTHLY_MONITOR where rst='A' and rut>=@now ";
    public const string SEL_TOP60WORKDAYS = "select top 60 Convert(varchar(20),Date_Time,111) as Date_Time FROM WORK_DATE where RST = 'A' AND Is_WorkDay = 'Y' AND Date_Time >= @beginDay order by date_time";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Report032BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 判斷當天是否執行完批次
    /// </summary>
    /// <returns></returns>
    public Boolean checkBatch()
    {
        dirValues.Clear();
        dirValues.Add("now", DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd"));
        return dao.Contains(IS_GENERATE, dirValues);
    }

    /// <summary>
    /// 獲得60天的工作日日期
    /// </summary>
    /// <returns></returns>
    public DataSet getWorkDays()
    {
        DataSet ds = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("beginDay", DateTime.Now.ToString("yyyy-MM-dd"));
            ds = dao.GetList(SEL_TOP60WORKDAYS, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return ds;
    }

    public bool IsMONTHLY_MONITOR()
    {
        bool IsBool = false;
        try
        {
            DataTable dtbl = dao.GetList("select convert(varchar(20),max(date_time),111) from dbo.WORK_DATE where is_workday='Y' and date_time <'" + DateTime.Now.ToString("yyyy-MM-dd") + "'").Tables[0];


            dirValues.Clear();
            dirValues.Add("CDay", dtbl.Rows[0][0].ToString());
            DataTable dtbl1 = dao.GetList(SEL_MONTHLY_MONITOR, dirValues).Tables[0];
            if (dtbl1.Rows[0][0].ToString() == "0")
                IsBool = false;
            else
                IsBool = true;
        }
        catch
        {
        }
        return IsBool;
    }

    public bool IsDAYLY_MONITOR()
    {
        bool IsBool = false;
        try
        {
            DataTable dtbl = dao.GetList("select convert(varchar(20),max(date_time),111) from dbo.WORK_DATE where is_workday='Y' and date_time <'" + DateTime.Now.ToString("yyyy-MM-dd") + "'").Tables[0];


            dirValues.Clear();
            dirValues.Add("CDay", dtbl.Rows[0][0].ToString());
            DataTable dtbl1 = dao.GetList(SEL_DAYLY_MONITOR, dirValues).Tables[0];
            if (dtbl1.Rows[0][0].ToString() == "0")
                IsBool = false;
            else
                IsBool = true;
        }
        catch { 
        }
        return IsBool;
    }


    public DataSet getENVELOPE()
    {
        DataSet ds = null;
        try
        {
            ds = dao.GetList(SEL_ENVELOPE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return ds;
    }

    public ENVELOPE_INFO getENVELOPEModel(int rid)
    {
        ENVELOPE_INFO model = null;
        try
        {
            model = dao.GetModel<ENVELOPE_INFO, int>("RID", rid);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return model;
    }

    public DataSet getEXPONENT()
    {
        DataSet ds = null;
        ds = dao.GetList(SEL_EXPONENT);
        return ds;
    }

    public CARD_EXPONENT getEXPONENTModel(int rid)
    {
        CARD_EXPONENT model = null;
        try
        {
            model = dao.GetModel<CARD_EXPONENT, int>("RID", rid);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return model;
    }

    public DataSet getDMTYPE()
    {
        DataSet ds = null;
        try
        {
            ds = dao.GetList(SEL_DMTYPE);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return ds;
    }

    public DMTYPE_INFO getDMTYPEModel(int rid)
    {
        DMTYPE_INFO model = null;
        try
        {
            model = dao.GetModel<DMTYPE_INFO, int>("RID", rid);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return model;
    }

    public string getDMTYPE_Name(string id)
    {
        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();

        dirValues.Clear();
        dirValues.Add("id", id);
        stbWhere.Append(" and rid=@id");

        DataSet ds = null;
        try
        {
            ds = dao.GetList(SEL_DMTYPE + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["name"].ToString();
        }
        else
        {
            return "";
        }
    }


    public DataSet getAction()
    {
        DataSet ds = null;
        try
        {
            ds = dao.GetList(SEL_ACTION);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return ds;
    }

    public DataSet getAction2()
    {
        DataSet ds = null;
        try
        {
            ds = dao.GetList(SEL_ACTION2);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        return ds;
    }

    /// <summary>
    /// 根據條件獲得要顯示的紙品集合
    /// </summary>
    /// <param name="searchInput"></param>
    /// <returns></returns>
    public DataSet getMaterialCount(Dictionary<string, object> searchInput)
    {
        dirValues.Clear();
        dirValues.Add("perso_facotry", searchInput["dropFactory"].ToString());
      
        DataSet ds = new DataSet();

        try
        {
            if (searchInput["dropMaterial_RID"].ToString() != "")
            {
                return null;
            }
            else if (searchInput["dropMaterial_RID"].ToString() == "" && searchInput["dropFactory"].ToString() != "")
            {

                if (searchInput["dropMaterialType"].ToString() == "信封")
                {
                    ds = dao.GetList(SEL_EnvelopeCount, dirValues);
                }
                else if (searchInput["dropMaterialType"].ToString() == "寄卡單")
                {
                    ds = dao.GetList(SEL_ExponentCount, dirValues);
                }
                else if (searchInput["dropMaterialType"].ToString() == "DM")
                {
                    ds = dao.GetList(SEL_DmCount, dirValues);
                }
            }
            else if (searchInput["dropMaterial_RID"].ToString() == "" && searchInput["dropFactory"].ToString() == "")
            {
                if (searchInput["dropMaterialType"].ToString() == "信封")
                {
                    ds = dao.GetList(SEL_EnvelopeCount2);
                }
                else if (searchInput["dropMaterialType"].ToString() == "寄卡單")
                {
                    ds = dao.GetList(SEL_ExponentCount2);
                }
                else if (searchInput["dropMaterialType"].ToString() == "DM")
                {
                    ds = dao.GetList(SEL_DmCount2);
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return ds;
        }
        else
        {
            return null;
        }
    }

    public DataSet getdata(Dictionary<string, object> searchInput)
    {
        DataSet ds = new DataSet();

        dirValues.Clear();
        if (searchInput["dropFactory"].ToString() != "")
        {
            dirValues.Add("perso_facotry", searchInput["dropFactory"].ToString());
        }
        else
        {
            dirValues.Add("perso_facotry", "");
        }
        dirValues.Add("material_type", searchInput["dropMaterialType"].ToString());
        if (searchInput["dropMaterial_RID"].ToString() != "")
        {
            dirValues.Add("materialrid", searchInput["dropMaterial_RID"].ToString());
        }
        else
        {
            dirValues.Add("materialrid", "");
        }
        dirValues.Add("diff", searchInput["dropAction"].ToString());
        dirValues.Add("TimeNow",searchInput["timenow"].ToString());

        //if (searchInput["dropMaterial_RID"].ToString() == "")
        //{
        //    ds = dao.GetList(SEL_Proc_ALLMaterial, dirValues, true);
        //}
        //else
        //{
        //    ds = dao.GetList(SEL_Proc_Material, dirValues, true);
        //}
        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString);

        try
        {
            sqlConn.Open();

            //指明Sql命令的操作类型是使用存储过程 
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure; //指定执行存储过程操作 
            if (searchInput["dropMaterial_RID"].ToString() == "")
            {
                cmd.CommandText = "proc_report032"; //存储过程名称 
            }
            else
            {
                cmd.CommandText = "proc_report032_2"; //存储过程名称 
            }

            SqlParameter perso_facotry = new SqlParameter("@perso_facotry", DBNull.Value);
            perso_facotry.Value = searchInput["dropFactory"].ToString();
            cmd.Parameters.Add(perso_facotry);

            SqlParameter material_type = new SqlParameter("@material_type", DBNull.Value);
            material_type.Value = searchInput["dropMaterialType"].ToString();
            cmd.Parameters.Add(material_type);

            SqlParameter materialrid = new SqlParameter("@materialrid", DBNull.Value);
            materialrid.Value = searchInput["dropMaterial_RID"].ToString();
            cmd.Parameters.Add(materialrid);

            SqlParameter diff = new SqlParameter("@diff", DBNull.Value);
            diff.Value = searchInput["dropAction"].ToString();
            cmd.Parameters.Add(diff);

            SqlParameter TimeNow = new SqlParameter("@TimeNow", DBNull.Value);
            TimeNow.Value = searchInput["timenow"].ToString();
            cmd.Parameters.Add(TimeNow);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.Fill(ds);            
        }
        catch(Exception ex)
        {
            ds = null;
        }
        finally
        {
            sqlConn.Close();
        }

        return ds;
    }

    public DataSet getdata2(Dictionary<string, object> searchInput)
    {
        DataSet ds = new DataSet();

        dirValues.Clear();
        if (searchInput["dropFactory"].ToString() != "")
        {
            dirValues.Add("perso_facotry", searchInput["dropFactory"].ToString());
        }
        else
        {
            dirValues.Add("perso_facotry", "");
        }
        dirValues.Add("material_type", searchInput["dropMaterialType"].ToString());
        if (searchInput["dropMaterial_RID"].ToString() != "")
        {
            dirValues.Add("materialrid", searchInput["dropMaterial_RID"].ToString());
        }
        else
        {
            dirValues.Add("materialrid", "");
        }
        dirValues.Add("diff", searchInput["dropAction"].ToString());
        dirValues.Add("TimeNow", searchInput["timenow"].ToString());

        //if (searchInput["dropMaterial_RID"].ToString() == "")
        //{
        //    ds = dao.GetList(SEL_Proc_ALLMaterial, dirValues, true);
        //}
        //else
        //{
        //    ds = dao.GetList(SEL_Proc_Material, dirValues, true);
        //}
        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection_System"].ConnectionString);

        try
        {
            sqlConn.Open();

            //指明Sql命令的操作类型是使用存储过程 
            SqlCommand cmd = sqlConn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure; //指定执行存储过程操作 
            if (searchInput["dropMaterial_RID"].ToString() == "")
            {
                cmd.CommandText = "proc_report033"; //存储过程名称 
            }
            else
            {
                cmd.CommandText = "proc_report033_2"; //存储过程名称 
            }

            SqlParameter perso_facotry = new SqlParameter("@perso_facotry", DBNull.Value);
            perso_facotry.Value = searchInput["dropFactory"].ToString();
            cmd.Parameters.Add(perso_facotry);

            SqlParameter material_type = new SqlParameter("@material_type", DBNull.Value);
            material_type.Value = searchInput["dropMaterialType"].ToString();
            cmd.Parameters.Add(material_type);

            SqlParameter materialrid = new SqlParameter("@materialrid", DBNull.Value);
            materialrid.Value = searchInput["dropMaterial_RID"].ToString();
            cmd.Parameters.Add(materialrid);

            SqlParameter diff = new SqlParameter("@diff", DBNull.Value);
            diff.Value = searchInput["dropAction"].ToString();
            cmd.Parameters.Add(diff);

            SqlParameter TimeNow = new SqlParameter("@TimeNow", DBNull.Value);
            TimeNow.Value = searchInput["timenow"].ToString();
            cmd.Parameters.Add(TimeNow);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.Fill(ds);
        }
        catch (Exception ex)
        {
            ds = null;
        }
        finally
        {
            sqlConn.Close();
        }


        return ds;
    }
}
