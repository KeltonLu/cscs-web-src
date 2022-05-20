//******************************************************************
//*  作    者：James
//*  功能說明：Perso項目種類管理邏輯
//*  創建日期：2008-08-28
//*  修改日期：2008-08-28 12:00
//*  修改記錄：
//*            □2008-08-28
//*              1.創建 占偉林
//*             2010/12/24 Ge.Song
//*                 1.代製費用的製程設定，新增、修改時可用相同單價
//*******************************************************************

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
using System.Collections;

/// <summary>
/// Perso項目種類管理作業
/// </summary>
public class CardType004BL : BaseLogic
{
    #region SQL語句

    public const string SEL_FACTORY = "SELECT F.RID,F.Factory_ShortName_CN "
                                        + "FROM FACTORY AS F "
                                        + "WHERE F.RST = 'A' AND F.Is_Perso = 'Y'";
    public const string SEL_PERSO_PROJECT = "select isnull(price,unit_price) as price1,* from "
                                            + " (select Convert(varchar(20),PPP.Use_Date_Begin,111) as Use_Date_Begin,Convert(varchar(20),PPP.Use_Date_End,111) as Use_Date_End,PPP.Price,"
                                            + " TB1.* "
                                            + " from PERSO_PROJECT_PRICE PPP"
                                            + " right join "
                                            + " (SELECT PP.RID,Factory_RID,PP.Project_Code,PP.Project_Name,PP.Unit_Price,F.Factory_ShortName_CN,CASE PP.Normal_Special WHEN '1' THEN '一般' ELSE '特別' END AS Normal_Special FROM PERSO_PROJECT AS PP LEFT JOIN FACTORY F ON F.RST = 'A' AND PP.Factory_RID = F.RID WHERE PP.RST='A') TB1"
                                            + " ON PPP.PERSO_PROJECT_RID=TB1.RID) a where 1>0 ";

    public const string SEL_PROJECT_STEP_ALL = "SELECT ps.Step_ID,PS.RID,[Factory_RID],[Name],[Price] as Unit_Price,[Use_Date_Begin],[Use_Date_End],F.Factory_ShortName_CN "
                                        + "FROM PROJECT_STEP as PS LEFT JOIN FACTORY F ON F.RST = 'A' AND PS.Factory_RID = F.RID  "
                                        + "WHERE PS.RST='A'";
    public const string SEL_SELECTED_CARDTYPE = "SELECT CT.RID AS RID, CT.Display_Name AS NAME "
                                        + "FROM CARDTYPE_PERSO_PROJECT AS CDPP INNER JOIN CARD_TYPE AS CT ON CT.RST = 'A' AND CDPP.CardType_RID = CT.RID "
                                        + "WHERE CDPP.RST = 'A' AND CDPP.PERSO_PROJECT_RID = @PERSO_PROJECT_rid";



    public const string SEL_SELECTED_PROJECT_STEP = "SELECT RID,Name "
                                        + "FROM PROJECT_STEP "
                                        + "WHERE RST = 'A' and Perso_RID = @perso_rid ";
    public const string SEL_SELECTED_PROJECT_STEP_Use_Date_Begin = "SELECT RID,Name "
                                        + "FROM PROJECT_STEP "
                                        + "WHERE RST = 'A' and Perso_RID = @perso_rid and Use_Date_Begin <= @use_date_begin and Use_Date_End = '9999/12/31' ";
    public const string SEL_SELECTED_MOD_PROJECT_STEP = "select RID,Name from PROJECT_STEP "
                                        + "where RST='A' and RID in (select Step_RID from STEP_PERSO_PROJECT where RST='A' and PERSO_PROJECT_RID = @rid)";
    public const string SEL_PERSO_PROJECT_Factory_RID = "SELECT Factory_RID "
                                + "FROM PERSO_PROJECT "
                                + "WHERE RST = 'A' and Factory_RID = @factory_rid and Project_Name = @project_name";

    public const string SEL_PROJECT_STEP = "SELECT RID "
                            + "FROM PROJECT_STEP "
                            + "WHERE RST = 'A' and Factory_RID = @perso_rid and Name = @name";

   



    public const string SEL_STEP_PERSO_PROJECT = "select End_Date from PERSO_PROJECT_SAP where RST='A' and PERSO_PROJECT_RID = "
                           + "(select top 1 RID from PERSO_PROJECT as PR where RST='A' and RID in "
                           + "(select PERSO_PROJECT_RID from STEP_PERSO_PROJECT "
                           + "where RST='A' and Step_RID = @rid) order by PR.Use_Date_Begin desc)";

    public const string SEL_PERSO_PROJECT_RID = "select RID,Project_Name,Normal_Special,Project_Code,Factory_RID,Use_Date_Begin "
                          + "from PERSO_PROJECT as PP "
                          + "where RST='A' and RID in (select PERSO_PROJECT_RID from STEP_PERSO_PROJECT where RST='A' and Step_RID = @rid) "
                          + "order by PP.Use_Date_Begin desc";

    public const string SEL_PERSO_PROJECT_PROJECT_STEP = "select RID,Project_Name,Use_Date_Begin,Use_Date_End "
                         + "from PERSO_PROJECT "
                         + "where RST='A' and RID in (select PERSO_PROJECT_RID from STEP_PERSO_PROJECT where RST='A' and Step_RID = @rid) ";

    public const string UPDATE_PERSO_PROJECT_Use_Date_End = "UPDATE PERSO_PROJECT SET  Use_Date_End = @use_date_end "
                                    + "WHERE RST = 'A' and Factory_RID = @factory_rid and Project_Name = @project_name and Use_Date_End = '9999/12/31' ";

    public const string UPDATE_PROJECT_STEP_Use_Date_End = "UPDATE PROJECT_STEP SET  Use_Date_End = @use_date_end "
                                + "WHERE RST = 'A' and Perso_RID = @perso_rid and Name = @name and Use_Date_End = '9999/12/31' ";

    public const string UPDATE_PROJECT_STEP_BeforeDate = "UPDATE PROJECT_STEP SET  Use_Date_End = @use_date_end "
                                + "WHERE RST = 'A' and Perso_RID = @perso_rid and Name = @name and Use_Date_End = @beforedate ";

    public const string UPDATE_PERSO_PROJECT_BeforeDate = "UPDATE PERSO_PROJECT SET  Use_Date_End = @use_date_end "
                                + "WHERE RST = 'A' and Factory_RID = @factory_rid and Project_Name = @project_name and Use_Date_End = @beforedate ";

    public const string SEL_PROJECT_STEP_BeforeDate = "select Use_Date_Begin from PROJECT_STEP "
                                + "WHERE RST = 'A' and Perso_RID = @perso_rid and Name = @name and Use_Date_End = @beforedate ";

    public const string SEL_PERSO_PROJECT_BeforeDate = "select Use_Date_Begin from PERSO_PROJECT "
                                + "WHERE RST = 'A' and Factory_RID = @factory_rid and Project_Name = @project_name and Use_Date_End = @beforedate ";

    public const string UPDATE_PROJECT_STEP_RID = "UPDATE PROJECT_STEP SET  Price = @price,Use_Date_Begin = @use_date_begin,Comment = @comment "
                                + "WHERE RST = 'A' and RID = @rid ";

    public const string UPDATE_PERSO_PROJECT_RID = "UPDATE PERSO_PROJECT SET  Use_Date_Begin = @use_date_begin,Comment = @comment,Unit_Price = @unit_price "
                                + "WHERE RST = 'A' and RID = @rid ";

    public const string UPDATE_PERSO_PROJECT_Use_Date_End_RID = "UPDATE PERSO_PROJECT SET Use_Date_End = @use_date_end "
                            + "WHERE RST = 'A' and RID = @rid ";


    public const string UPDATE_STEP_PERSO_PROJECT = "DELETE FROM STEP_PERSO_PROJECT "
                                    + "WHERE PERSO_PROJECT_RID = @PERSO_PROJECT_rid";
    public const string UPDATE_PERSO_PROJECT_Use_Date_End_MOD = "UPDATE  PERSO_PROJECT SET  Use_Date_End = '9999/12/31' "
                                  + "WHERE RST = 'A' and rid =(select top 1 RID from PERSO_PROJECT where RST = 'A' and Factory_RID = @factory_rid and Project_Name = @project_name order by Use_Date_End desc)";

    public const string UPDATE_PROJECT_STEP_Use_Date_End_MOD_BeforeDate = "UPDATE PROJECT_STEP SET Use_Date_End = '9999/12/31' "
                              + "WHERE RST = 'A' and Perso_RID = @perso_rid and Name = @name and Use_Date_End = @use_date_begin ";


    public const string SEL_SELECTED_PROJECT_STEP_Price = "SELECT Price "
                                    + "FROM PROJECT_STEP "
                                    + "WHERE RST = 'A' and RID = @rid ";

    public const string CHK_PROJECTSTEP_BY_RID = "proc_CHK_DEL_PROJECTSTEP";

    public const string SEL_STEPDATE_BY_STEPID = "select  date1 from "
                                            + " (select distinct Convert(varchar(20),Use_Date_Begin,111) as date1 from PROJECT_STEP where step_id in ({0})"
                                            + " union all"
                                            + " select distinct Convert(varchar(20),Use_Date_End,111) as date1 from PROJECT_STEP where step_id in ({0})) a order by date1";
                                            
    public const string SEL_STEPDATE_BY_STEPID1 = "select Use_Date_Begin,Use_Date_End,price from PROJECT_STEP where step_id in ({0})";

    public const string SEL_PERSOPROJECT_BY_RID = "SELECT PP.*,Factory_RID,F.Factory_ShortName_CN,CASE PP.Normal_Special WHEN '1' THEN '一般' ELSE '特別' END AS Normal_SpecialName FROM PERSO_PROJECT AS PP LEFT JOIN FACTORY F ON F.RST = 'A' AND PP.Factory_RID = F.RID WHERE PP.RST='A' and PP.RID=@RID"
                                            + " select distinct tb2.name FROM STEP_PERSO_PROJECT tb1 inner join (select distinct step_id,name from PROJECT_STEP) tb2 on tb1.step_rid=tb2.step_id WHERE tb1.PERSO_PROJECT_RID=@RID"
                                            + " select Convert(varchar(20),Use_Date_Begin,111)+'~'+Convert(varchar(20),Use_Date_End,111) as Qujian,Price from PERSO_PROJECT_PRICE where PERSO_PROJECT_RID=@RID order by PERSO_PROJECT_RID,Use_Date_End";

    public const string CON_PERSOPROJECTSTEP_BY_NAME = "select count(*) from PERSO_PROJECT where factory_rid=@factory_rid and project_name=@project_name";

    public const string SEL_PERSOPROJECT_BY_FACOTRYID = "select * from PERSO_PROJECT where Normal_Special=1 and factory_rid=@factory_rid and RID not in (select PersoProject_RID from dbo.CARDTYPE_PROJECT_TIME)";

    public const string SEL_CARDTYPE_BY_FACTORYID = "select tb1.display_name,tb1.rid from card_type tb1 inner join (select distinct cardtype_rid from PERSO_CARDTYPE WHERE factory_rid={0}) tb2 on tb1.rid=tb2.cardtype_rid";

    public const string SEL_CARDTYPE_PROJECT = "select CPT.Comment,CPT.PERSOpROJECT_RID,CPT.RID,Convert(varchar(20),CPT.Use_Date_Begin,111) as Use_Date_Begin,Convert(varchar(20),CPT.Use_Date_End,111) as Use_Date_End,TB1.Factory_ShortName_CN,TB1.Project_Code,TB1.Project_Name,TB1.Factory_ShortName_CN_ID"
                                            + " from dbo.CARDTYPE_PROJECT_TIME CPT INNER JOIN"
                                            + " (SELECT F.RID AS Factory_ShortName_CN_ID,F.Factory_ShortName_CN,PP.Project_Code,PP.Project_Name,PP.RID "
                                            + " FROM PERSO_PROJECT PP"
                                            + " LEFT join FACTORY F ON F.RST = 'A' AND PP.Factory_RID = F.RID) TB1 ON CPT.PersoProject_RID=TB1.RID";

    public const string SEL_CARD_BY_ProjectTime_RID = "select ct.rid,ct.Display_Name AS name from  CARDTYPE_PERSO_PROJECT cpp left join card_type ct on cpp.CardType_RID=ct.RID where CPP.ProjectTime_RID=@ProjectTime_RID Order by name";

    public const string SEL_LASTCARDTYPE_PROJECT = "select a.* from (select *,ROW_NUMBER() OVER (order by rid) as RowNumber from CARDTYPE_PROJECT_TIME where rid<=@rid and PersoProject_RID=@PersoProject_RID) a ";

    public const string SEL_LAST_PROJECT_STEP = "select * from "
                                            + " (select *,ROW_NUMBER() OVER (order by rid) as RowNumber from PROJECT_STEP WHERE RST = 'A' and Factory_RID = @perso_rid and Name = @name) a"
                                            + " order by rid desc";

    public const string SEL_PERSO_PROJECT_SAP_BY_FID = "select max(end_date) as end_date from PERSO_PROJECT_SAP where Perso_Factory_RID=@Factory_RID";

    public const string SEL_CARD_TYPE = "select Display_Name,RID "
                                    + " from card_Type "
                                    + " where rid not in "
                                    + " (SELECT distinct CardType_RID  FROM CARDTYPE_PERSO_PROJECT WHERE ProjectTime_RID IN"
                                    + " (select RID from CARDTYPE_PROJECT_TIME"
                                    + " where ((use_date_begin<='{0}' and Use_Date_End>='{0}')"
                                    + " or (use_date_begin<='{1}' and Use_Date_End>='{1}')"
                                    + " or (use_date_begin>='{0}' and Use_Date_End<='{1}'))"
                                    + " and PersoProject_RID IN "
                                    + " (select RID from PERSO_PROJECT where Factory_RID = {2} and RID not in ({3}))))";
    public const string SEL_CARD_TYPE_RID ="SELECT distinct CardType_RID  FROM CARDTYPE_PERSO_PROJECT WHERE ProjectTime_RID IN "
                                        +"(select RID from CARDTYPE_PROJECT_TIME "
                                        + "where ((use_date_begin<=@Use_Date_Begin and Use_Date_End>=@Use_Date_Begin) "
                                        +"or (use_date_begin<=@Use_Date_End and Use_Date_End>=@Use_Date_End) "
                                        + "or (use_date_begin>=@Use_Date_Begin and Use_Date_End<=@Use_Date_End )) "
                                        +"and PersoProject_RID IN "
                                        +"(select RID from PERSO_PROJECT where Factory_RID = @Factory_RID and RID <> @PersoProject_RID)) ";

    public const string SEL_PRICE_BY_QUJIANTIME = "select isnull(sum(price),0) as price from PROJECT_STEP where step_id in ({0})"
                                                   + "and use_date_begin<='{1}' and use_date_end>='{1}'";

    public const string SEL_PERSO_PROJECT_BY_STEPRID = "select * from STEP_PERSO_PROJECT where step_rid=@step_rid";

    public const string DEL_PROJECT_STEP_CHK = "select count(*) from PERSO_PROJECT_SAP"
                                            + " where ((begin_date<=@begin_date and end_date>=@begin_date)"
                                            + " or (begin_date<=@end_date and end_date>=@end_date)"
                                            + " or (begin_date>=@begin_date and end_date<=@end_date)) and Perso_Factory_RID=@Perso_Factory_RID";


    public const string SEL_PERSO_PROJECTNAME_BY_STEPRID = "select pp.Project_Name from dbo.STEP_PERSO_PROJECT spp inner join PERSO_PROJECT pp on spp.Perso_Project_RID=pp.rid where step_rid=@step_rid";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public CardType004BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public DataSet GetPersoProjectNameByStepID(string strStepRID)
    {
        DataSet dsPROJECT_STEP = null;
        // 開始查詢
        try
        {
            this.dirValues.Clear();
            dirValues.Add("step_rid", strStepRID);
            dsPROJECT_STEP = dao.GetList(SEL_PERSO_PROJECTNAME_BY_STEPRID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        // 返回資料--------------------------------------------------------------------------------------------
        return dsPROJECT_STEP;
    }

    /// <summary>
    /// 查詢製程修改/刪除分段代製項目資料
    /// </summary>
    /// <returns></returns>
    public DataSet SearchdsProject_Step(int rid)
    {
        DataSet dsPROJECT_STEP = null;
        // 開始查詢
        try
        {
            this.dirValues.Clear();
            dirValues.Add("rid", rid);
            dsPROJECT_STEP = dao.GetList(SEL_PERSO_PROJECT_PROJECT_STEP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        // 返回資料--------------------------------------------------------------------------------------------
        return dsPROJECT_STEP;
    }

    /// <summary>
    /// 查詢製程資料
    /// </summary>
    /// <returns></returns>
    public DataSet SearchStepData(string Perso_RID)
    {
        DataSet dstRole = null;
        // 開始查詢
        try
        {
            this.dirValues.Clear();
            dirValues.Add("perso_rid", Perso_RID);
            dstRole = dao.GetList(SEL_SELECTED_PROJECT_STEP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        // 返回資料--------------------------------------------------------------------------------------------
        return dstRole;
    }

    /// <summary>
    /// 查詢修改製程資料
    /// </summary>
    /// <returns></returns>
    public DataSet SearchSelectOKStepData(string Perso_RID, string Use_Date_Begin)
    {
        DataSet dstRole = null;
        // 開始查詢
        try
        {
            this.dirValues.Clear();
            dirValues.Add("perso_rid", Perso_RID);
            dirValues.Add("use_date_begin", Use_Date_Begin);
            dstRole = dao.GetList(SEL_SELECTED_PROJECT_STEP_Use_Date_Begin, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        // 返回資料--------------------------------------------------------------------------------------------
        return dstRole;
    }



    /// <summary>
    /// 查詢修改製程資料
    /// </summary>
    /// <returns></returns>
    public DataSet SearchModStepData(int RID)
    {
        DataSet dsStepData = null;
        // 開始查詢
        try
        {
            this.dirValues.Clear();
            dirValues.Add("rid", RID);
            dsStepData = dao.GetList(SEL_SELECTED_MOD_PROJECT_STEP, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        // 返回資料--------------------------------------------------------------------------------------------
        return dsStepData;
    }


    /// <summary>
    /// 獲得Perso廠商
    /// </summary>
    /// <returns>DataSet[Perso廠商]</returns>
    public DataSet GetFactoryList()
    {
        DataSet dstFactory = null;
        try
        {
            this.dirValues.Clear();
            dstFactory = dao.GetList(SEL_FACTORY, dirValues);

            return dstFactory;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 查詢Perso項目資料列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "factory_rid,RID" : "factory_rid,RID," + sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_PERSO_PROJECT);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtProject_Name"].ToString().Trim()))
            {
                stbWhere.Append(" AND Project_Name like @Project_Name");
                dirValues.Add("Project_Name", "%" + searchInput["txtProject_Name"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND Factory_RID = @factory_RID");
                dirValues.Add("factory_RID", Convert.ToInt32(searchInput["dropFactory"]));
            }
            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString().Trim()))
            {
                stbWhere.Append(" AND Use_Date_Begin >= @Begin_Date ");
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString().Trim()))
            {
                stbWhere.Append(" AND Use_Date_End <= @Finish_Date ");
                dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dstPERSO_PROJECT = null;
        try
        {
            dstPERSO_PROJECT = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstPERSO_PROJECT;
    }



    /// <summary>
    /// 查詢PROJECT_STEP資料列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public DataSet List1(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "factory_rid,step_id,rid" : "factory_rid,step_id,rid," + sortField);//默認的排序欄位
       
        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_PROJECT_STEP_ALL);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" AND PS.Name like @name");
                dirValues.Add("name", "%" + searchInput["txtName"].ToString().Trim() + "%");
            }
            if (!StringUtil.IsEmpty(searchInput["dropFactory"].ToString().Trim()))
            {
                stbWhere.Append(" AND F.RID = @factory_RID");
                dirValues.Add("factory_RID", Convert.ToInt32(searchInput["dropFactory"]));
            }

            if (!StringUtil.IsEmpty(searchInput["txtBegin_Date"].ToString().Trim()))
            {
                stbWhere.Append(" AND Use_Date_Begin >= @Begin_Date ");
                dirValues.Add("Begin_Date", searchInput["txtBegin_Date"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtFinish_Date"].ToString().Trim()))
            {
                stbWhere.Append(" AND Use_Date_End <= @Finish_Date ");
                dirValues.Add("Finish_Date", searchInput["txtFinish_Date"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dstPERSO_PROJECT = null;
        try
        {
            dstPERSO_PROJECT = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstPERSO_PROJECT;
    }


    /// <summary>
    /// 代製項目與卡種設定查詢
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="firstRowNumber"></param>
    /// <param name="lastRowNumber"></param>
    /// <param name="sortField"></param>
    /// <param name="sortType"></param>
    /// <param name="rowCount"></param>
    /// <returns></returns>
    public DataSet ListCard(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "PersoProject_RID,RID" : sortField +" "+ sortType + ",PersoProject_RID");//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CARDTYPE_PROJECT+" WHERE 1>0 ");

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["dropFactory_RID"].ToString().Trim()))
            {
                stbWhere.Append(" AND TB1.Factory_ShortName_CN_ID=@Factory_ShortName_CN_ID");
                dirValues.Add("Factory_ShortName_CN_ID", searchInput["dropFactory_RID"].ToString().Trim());
            }
            if (!StringUtil.IsEmpty(searchInput["txtProject_Name"].ToString().Trim()))
            {
                stbWhere.Append(" AND TB1.Project_Name like @Project_Name");
                dirValues.Add("Project_Name", "%" + searchInput["txtProject_Name"].ToString() + "%");
            }

            if (!StringUtil.IsEmpty(searchInput["txtUse_Date_Begin"].ToString().Trim()))
            {
                stbWhere.Append(" AND CPT.Use_Date_Begin >= @Begin_Date ");
                dirValues.Add("Begin_Date", searchInput["txtUse_Date_Begin"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["txtUse_Date_End"].ToString().Trim()))
            {
                stbWhere.Append(" AND CPT.Use_Date_End <= @Finish_Date ");
                dirValues.Add("Finish_Date", searchInput["txtUse_Date_End"].ToString().Trim());
            }
            if (((DataTable)searchInput["UctrlCardType"]).Rows.Count > 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["UctrlCardType"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";

                stbWhere.Append(" and CPT.RID IN (select ProjectTime_RID from CARDTYPE_PERSO_PROJECT where cardtype_rid IN (" + strCardType.Substring(0, strCardType.Length - 1) + ") group by ProjectTime_RID having count(ProjectTime_RID)=" + ((DataTable)searchInput["UctrlCardType"]).Rows.Count.ToString() + ")");
            }
        }

        //執行SQL語句
        DataSet dstPERSO_PROJECT = null;
        try
        {
            dstPERSO_PROJECT = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstPERSO_PROJECT;
    }


    /// <summary>
    /// 獲取選擇卡種
    /// </summary>
    /// <param name="strProjectTime_RID"></param>
    /// <returns></returns>
    public DataTable GetCardtypePerso(string strProjectTime_RID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("ProjectTime_RID",strProjectTime_RID);
            return dao.GetList(SEL_CARD_BY_ProjectTime_RID, dirValues).Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }



    /// <summary>
    /// 查詢Perso項目信息
    /// </summary>
    /// <param name="strRID">Perso項目種類ID</param>
    /// <param name="ppModel">Perso項目種類信息</param>
    /// <param name="SelectedCardType">Perso項目關聯卡種</param>
    public void ListModel(String strRID, ref PERSO_PROJECT ppModel, ref DataSet SelectedCardType, ref DataSet dstFactory)
    {
        try
        {
            // 取Perso項目種類信息
            ppModel = dao.GetModel<PERSO_PROJECT, int>("RID", int.Parse(strRID));

            if ("1" == ppModel.Normal_Special)
            {
                // 已選擇的卡種
                dirValues.Clear();
                dirValues.Add("PERSO_PROJECT_rid", int.Parse(strRID));

                SelectedCardType = dao.GetList(SEL_SELECTED_CARDTYPE, dirValues);
            }

            // 廠商
            dstFactory = GetFactoryList();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }

    /// <summary>
    /// 查詢Perso項目信息
    /// </summary>
    /// <param name="strRID">Perso項目種類ID</param>
    /// <param name="ppModel">Perso項目種類信息</param>
    /// <param name="SelectedCardType">Perso項目關聯卡種</param>
    public void ListModel1(String strRID, ref PROJECT_STEP psModel, ref DataSet dstFactory)
    {
        try
        {
            // 取Perso項目種類信息
            psModel = dao.GetModel<PROJECT_STEP, int>("RID", int.Parse(strRID));


            // 廠商
            dstFactory = GetFactoryList();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
    }


    /// <summary>
    /// 查詢Perso項目種類信息
    /// </summary>
    /// <param name="strRID">Perso項目種類ID</param>
    /// <returns>DataSet[Perso項目種類]</returns>
    public CARD_EXPONENT GetCardExponentModelByRID(string strRID)
    {
        CARD_EXPONENT cardExponentModel = null;
        try
        {
            cardExponentModel = dao.GetModel<CARD_EXPONENT, int>("RID", int.Parse(strRID));

            return cardExponentModel;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
    }

    /// <summary>
    /// 獲得Perso項目已選卡種類
    /// </summary>
    /// <param name="strRID">Perso項目種類ID</param>
    /// <returns>DataSet[Perso項目已選卡種]</returns>
    public DataSet SelectedCardTypeList(string strRID)
    {
        DataSet dstCardType = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("CARD_EXPONENT_rid", int.Parse(strRID));

            dstCardType = dao.GetList(SEL_SELECTED_CARDTYPE, dirValues);

            return dstCardType;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }

    }

    /// <summary>
    /// 是否為最後一筆製成
    /// </summary>
    /// <param name="strName"></param>
    /// <param name="strPerRID"></param>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public bool IsLastStep(string strName,string strPerRID,string strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("perso_rid", strPerRID);
            dirValues.Add("name", strName);
            DataSet dsPROJECT_STEP = dao.GetList(SEL_LAST_PROJECT_STEP, dirValues);
            if (dsPROJECT_STEP.Tables[0].Rows.Count > 0)
            {
                if (dsPROJECT_STEP.Tables[0].Rows[0]["RID"].ToString() == strRID)
                    return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

    }


 

    /// <summary>
    ///代製項目和對應之Perso廠在資料庫中是否存在
    /// </summary>
    public bool IsExistFactory_RID(int Factory_RID, string Project_Name)
    {
        DataSet dsFactory_RID = null;
        try
        {
            dirValues.Clear();
            #region
            dirValues.Add("factory_rid", Factory_RID);
            dirValues.Add("project_name", Project_Name);
            dsFactory_RID = dao.GetList(SEL_PERSO_PROJECT_Factory_RID, dirValues);
            if (dsFactory_RID.Tables[0] != null)
            {
                if (dsFactory_RID.Tables[0].Rows.Count != 0)
                {
                    return true; ;
                }
            }
            #endregion
        }
        catch (Exception)
        {

            throw;
        }
        return false;
    }



    /// <summary>
    ///製程和對應之Perso廠在資料庫中是否存在
    /// </summary>
    public bool IsExistStep(int Perso_RID, string StepName)
    {
        DataSet dsFactory_RID = null;
        try
        {
            dirValues.Clear();
 
            dirValues.Add("perso_rid", Perso_RID);
            dirValues.Add("name", StepName);
            dsFactory_RID = dao.GetList(SEL_PROJECT_STEP, dirValues);
            if (dsFactory_RID.Tables[0] != null)
            {
                if (dsFactory_RID.Tables[0].Rows.Count != 0)
                {
                    return true; ;
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
        return false;
    }

    #region 代製項目和卡種關系檔
    /// <summary>
    /// 卡種新增
    /// </summary>
    /// <param name="cptModel"></param>
    /// <param name="dtblCardType"></param>
    public void AddCard(CARDTYPE_PROJECT_TIME cptModel, DataTable dtblCardType)
    {
        try
        {
            dao.OpenConnection();
            //代製項目使用時間檔
            int intRID = Convert.ToInt32(dao.AddAndGetID(cptModel, "RID"));

            //代製項目和卡種關系檔
            foreach (DataRow drow in dtblCardType.Rows)
            {
                CARDTYPE_PERSO_PROJECT cppModel = new CARDTYPE_PERSO_PROJECT();
                cppModel.CardType_RID = int.Parse(drow["RID"].ToString());
                cppModel.ProjectTime_RID = intRID;
                dao.Add<CARDTYPE_PERSO_PROJECT>(cppModel, "RID");
            }

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }


    public void UpdataCard(CARDTYPE_PROJECT_TIME cptModel, DataTable dtblCardType, int intType)
    {
        //0:什麽都不修改
        //1:不變更使用期間起，修改卡種
        //2:變更使用期間起，不修改卡種
        //3:變更使用期間起，修改卡種
        try
        {
            dao.OpenConnection();

            CARDTYPE_PROJECT_TIME cptModel_o = dao.GetModel<CARDTYPE_PROJECT_TIME, int>("RID", cptModel.RID);

            //更新時間關係檔
            cptModel.RCT = cptModel_o.RCT;
            cptModel.RCU = cptModel_o.RCU;
            cptModel.RST = cptModel_o.RST;


            //取得最大一筆代製費用請款檔
            dirValues.Clear();
            dirValues.Add("Factory_RID",cptModel.PersoProject_RID);
            DataSet dstSAP = dao.GetList(SEL_PERSO_PROJECT_SAP_BY_FID, dirValues);
            if (!StringUtil.IsEmpty(dstSAP.Tables[0].Rows[0][0].ToString()))
            {
                DateTime dtLastSap = Convert.ToDateTime(dstSAP.Tables[0].Rows[0][0].ToString());
                if (cptModel.Use_Date_Begin <= dtLastSap)
                    throw new AlertException("使用期間起必須大於最後請款卡片耗用迄日");
            }

            //使用期間起不能大約使用期間迄
            if(cptModel.Use_Date_Begin>cptModel.Use_Date_End)
                throw new AlertException("使用期間迄必須大於使用期間起");
            //同一卡種在同一時間段內只能對應同一廠商的同一代制項目
            if (intType == 1 || intType == 3)
            {
                dirValues.Clear();
                dirValues.Add("PersoProject_RID", cptModel.PersoProject_RID.ToString());              
                DataSet dstFactory = dao.GetList("select Factory_rid from PERSO_PROJECT where rid=@PersoProject_RID", dirValues);
                if (!StringUtil.IsEmpty(dstFactory.Tables[0].Rows[0][0].ToString()))
                {
                    dirValues.Add("Factory_RID", dstFactory.Tables[0].Rows[0][0].ToString());
                }
                else dirValues.Add("Factory_RID", "");
                dirValues.Add("Use_Date_Begin", Convert.ToDateTime(cptModel.Use_Date_Begin.ToString()));
                dirValues.Add("Use_Date_End",  Convert.ToDateTime(cptModel.Use_Date_End.ToString()));
                DataSet dst = dao.GetList(SEL_CARD_TYPE_RID, dirValues);
                string msg = "";
                foreach (DataRow drow in dtblCardType.Rows)
                {
                   for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        if (drow["RID"].ToString() == dst.Tables[0].Rows[i][0].ToString())
                        {
                            msg += "卡種:"+drow["NAME"].ToString() + "已被其它代制項目使用，請重新選擇\\n";
                        }
                    }
                }
                if(msg != "")
                    throw new AlertException(msg);
            }
            if (intType == 0)
            {
                dao.Update<CARDTYPE_PROJECT_TIME>(cptModel, "RID");
            }
            else if (intType == 1)
            {
                dao.Update<CARDTYPE_PROJECT_TIME>(cptModel, "RID");

                //刪除卡種信息
                dao.ExecuteNonQuery("delete from CARDTYPE_PERSO_PROJECT where ProjectTime_RID=" + cptModel.RID.ToString());

                //代製項目和卡種關系檔
                foreach (DataRow drow in dtblCardType.Rows)
                {
                    CARDTYPE_PERSO_PROJECT cppModel = new CARDTYPE_PERSO_PROJECT();
                    cppModel.CardType_RID = int.Parse(drow["RID"].ToString());
                    cppModel.ProjectTime_RID = cptModel.RID;
                    dao.Add<CARDTYPE_PERSO_PROJECT>(cppModel, "RID");
                }
            }
            else if (intType == 2)
            {
                dirValues.Clear();
                dirValues.Add("PersoProject_RID", cptModel.PersoProject_RID.ToString());
                dirValues.Add("rid", cptModel.RID);
                DataTable dtblLastCardTime = dao.GetList(SEL_LASTCARDTYPE_PROJECT+" order by rid desc", dirValues).Tables[0];

                int intMaxLength = int.Parse(dtblLastCardTime.Rows[0]["RowNumber"].ToString())-1;

                if (intMaxLength==0)
                {
                    dao.Update<CARDTYPE_PROJECT_TIME>(cptModel, "RID");
                }
                else
                {
                    CARDTYPE_PROJECT_TIME cptModel_L = dao.GetModel<CARDTYPE_PROJECT_TIME, int>("RID", int.Parse(dtblLastCardTime.Select("RowNumber='" + intMaxLength + "'")[0]["RID"].ToString()));

                    if (cptModel.Use_Date_Begin <= cptModel_L.Use_Date_Begin)
                        throw new AlertException("使用期間起不可小於前一筆使用期間起or最後請款卡片耗用日");

                    //更新上筆時間關係檔
                    cptModel_L.Use_Date_End = cptModel.Use_Date_Begin.AddDays(-1);
                    dao.Update<CARDTYPE_PROJECT_TIME>(cptModel_L, "RID");

                    dao.Update<CARDTYPE_PROJECT_TIME>(cptModel, "RID");
                }
            }
            else if (intType == 3)
            {
                int intType1 = 1;

                if (cptModel.Use_Date_Begin < cptModel_o.Use_Date_Begin)//輸入使用期間起小於原使用期間起(不可小於前一筆使用期間起or最後請款卡片耗用迄日)
                {
                    dirValues.Clear();
                    dirValues.Add("PersoProject_RID", cptModel.PersoProject_RID.ToString());
                    dirValues.Add("rid", cptModel.RID);
                    DataTable dtblLastCardTime = dao.GetList(SEL_LASTCARDTYPE_PROJECT + " order by rid desc", dirValues).Tables[0];

                    int intMaxLength = int.Parse(dtblLastCardTime.Rows[0]["RowNumber"].ToString()) - 1;

                    if (intMaxLength == 0)
                    {
                        dao.Update<CARDTYPE_PROJECT_TIME>(cptModel, "RID");
                    }
                    else
                    {
                        CARDTYPE_PROJECT_TIME cptModel_L = dao.GetModel<CARDTYPE_PROJECT_TIME, int>("RID", int.Parse(dtblLastCardTime.Select("RowNumber='" + intMaxLength + "'")[0]["RID"].ToString()));

                        if (cptModel.Use_Date_Begin <= cptModel_L.Use_Date_Begin)
                            throw new AlertException("使用期間起不可小於前一筆使用期間起or最後請款卡片耗用日");

                        //更新上筆時間關係檔
                        cptModel_L.Use_Date_End = cptModel.Use_Date_Begin.AddDays(-1);
                        dao.Update<CARDTYPE_PROJECT_TIME>(cptModel_L, "RID");

                        dao.Update<CARDTYPE_PROJECT_TIME>(cptModel, "RID");
                    }

                    //刪除卡種信息
                    dao.ExecuteNonQuery("delete from CARDTYPE_PERSO_PROJECT where ProjectTime_RID=" + cptModel.RID.ToString());

                    //代製項目和卡種關系檔
                    foreach (DataRow drow in dtblCardType.Rows)
                    {
                        CARDTYPE_PERSO_PROJECT cppModel = new CARDTYPE_PERSO_PROJECT();
                        cppModel.CardType_RID = int.Parse(drow["RID"].ToString());
                        cppModel.ProjectTime_RID = cptModel.RID;
                        dao.Add<CARDTYPE_PERSO_PROJECT>(cppModel, "RID");
                    }
                }
                else//輸入使用期間起大於原使用期間起(不可大於此筆使用期間迄)
                {
                    if (cptModel.Use_Date_Begin <= cptModel_o.Use_Date_Begin)
                        throw new AlertException("使用期間起不可小於前一筆使用期間起or最後請款卡片耗用日");

                    cptModel.Comment = "";
                    //新增一筆
                    int intRID = Convert.ToInt32(dao.AddAndGetID<CARDTYPE_PROJECT_TIME>(cptModel, "RID"));

                    //代製項目和卡種關系檔
                    foreach (DataRow drow in dtblCardType.Rows)
                    {
                        CARDTYPE_PERSO_PROJECT cppModel = new CARDTYPE_PERSO_PROJECT();
                        cppModel.CardType_RID = int.Parse(drow["RID"].ToString());
                        cppModel.ProjectTime_RID = intRID;
                        dao.Add<CARDTYPE_PERSO_PROJECT>(cppModel, "RID");
                    }

                    //修改當前筆
                    cptModel_o.Use_Date_End = cptModel.Use_Date_Begin.AddDays(-1);
                    dao.Update<CARDTYPE_PROJECT_TIME>(cptModel_o, "RID");
                }
            }

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }

    }

    public void DelCard(string strRID)
    {
        try
        {
            dao.OpenConnection();

            CARDTYPE_PROJECT_TIME cptModel = dao.GetModel<CARDTYPE_PROJECT_TIME, int>("RID", int.Parse(strRID));


            //是否請款

            dirValues.Clear();
            dirValues.Add("RID", cptModel.PersoProject_RID);
            DataSet dstTime = dao.GetList("SELECT Convert(varchar(20),min(Use_Date_End),111),Convert(varchar(20),max(Use_Date_Begin),111) FROM PERSO_PROJECT_PRICE WHERE Perso_Project_RID=@RID", dirValues);
            if (dstTime.Tables[0].Rows[0][0].ToString() != "")
            {
                string strBegin = dstTime.Tables[0].Rows[0][0].ToString();
                string strEnd = dstTime.Tables[0].Rows[0][1].ToString();

                PERSO_PROJECT ppModel = dao.GetModel<PERSO_PROJECT, int>("RID",cptModel.PersoProject_RID);

                if (ppModel != null)
                {
                    dirValues.Clear();
                    dirValues.Add("begin_date", strBegin);
                    dirValues.Add("end_date", strEnd);
                    dirValues.Add("Perso_Factory_RID", ppModel.Factory_RID);
                    DataTable dtblStepChk = dao.GetList(DEL_PROJECT_STEP_CHK, dirValues).Tables[0];
                    if (dtblStepChk.Rows[0][0].ToString() != "0")
                        throw new AlertException("價格期間已請款，不能刪除");
                }
            }


            //刪除卡種信息
            dao.ExecuteNonQuery("delete from CARDTYPE_PERSO_PROJECT where ProjectTime_RID in (SELECT RID FROM CARDTYPE_PROJECT_TIME where PersoProject_RID=" + cptModel.PersoProject_RID.ToString() + ")");

            //刪除時間關係檔
            dao.ExecuteNonQuery("delete FROM CARDTYPE_PROJECT_TIME where PersoProject_RID=" + cptModel.PersoProject_RID.ToString());


            //操作日誌
            SetOprLog("4");
           
            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    #endregion


    #region 代製項目
    /// <summary>
    /// 代製項目修改增加
    /// </summary>
    public void Add(PERSO_PROJECT ppModel, DataTable dtblStepQujian, string strStepIDs)
    {
        try
        {
            dao.OpenConnection();

            if (IsStepExist(ppModel.Factory_RID.ToString(),ppModel.Project_Name))
            {
                throw new AlertException("代製項目重覆，不可新增");
            }

            ppModel.Project_Code = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Project_Code");

            if (ppModel.Normal_Special == "1")      //一般
            {
                ppModel.Unit_Price = 0.00M;

                int intRID = Convert.ToInt32(dao.AddAndGetID<PERSO_PROJECT>(ppModel, "RID"));


                //增加製程
                string[] strStepID = strStepIDs.Split(',');
                for (int i = 0; i < strStepID.Length; i++)
                {
                    STEP_PERSO_PROJECT sppModel = new STEP_PERSO_PROJECT();
                    sppModel.Perso_Project_RID = intRID;
                    sppModel.Step_RID = Convert.ToInt32(strStepID[i]);
                    dao.Add<STEP_PERSO_PROJECT>(sppModel, "RID");
                }

                //增加價格
                foreach (DataRow drowStepQujian in dtblStepQujian.Rows)
                {
                    PERSO_PROJECT_PRICE pppModel = new PERSO_PROJECT_PRICE();
                    pppModel.Perso_Project_RID = intRID;
                    pppModel.Price = Convert.ToDecimal(drowStepQujian["price"]);
                    pppModel.Use_Date_Begin = Convert.ToDateTime(drowStepQujian["Begin"]);
                    pppModel.Use_Date_End = Convert.ToDateTime(drowStepQujian["End"]);
                    dao.Add<PERSO_PROJECT_PRICE>(pppModel, "RID");
                }
            }
            else                            //特殊
            {
                dao.AddAndGetID<PERSO_PROJECT>(ppModel, "RID");
            }

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }



    /// <summary>
    /// 代製項目修改
    /// </summary>
    /// <param name="ceModel">Perso項目種類</param>
    /// <param name="dtblCardType">選擇的卡種</param>
    public void Update(PERSO_PROJECT ppModel, DataTable dtblCardType, string BeforeDate, ListBox LbLeft, ListBox LbRight)
    {
        
    }


   



    /// <summary>
    /// Perso項目種類刪除
    /// </summary>
    /// <param name="strRID">Perso項目種類RID</param>
    public void Delete(String strRID)
    {
        try
        {
            //事務開始
            dao.OpenConnection();

            dirValues.Clear();
            dirValues.Add("RID", strRID);

            DataSet dstProject = dao.GetList("proc_CHK_DEL_PERSO_PROJECT", dirValues, true);
            if (dstProject.Tables[0].Rows[0][0].ToString() != "0")
                throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "代製項目"));

            //是否請款
            DataSet dstTime = dao.GetList("SELECT Convert(varchar(20),min(Use_Date_End),111),Convert(varchar(20),max(Use_Date_Begin),111) FROM PERSO_PROJECT_PRICE WHERE Perso_Project_RID=@RID", dirValues);
            if (dstTime.Tables[0].Rows[0][0].ToString() != "")
            {
                string strBegin = dstTime.Tables[0].Rows[0][0].ToString();
                string strEnd = dstTime.Tables[0].Rows[0][1].ToString();

                PERSO_PROJECT ppModel = dao.GetModel<PERSO_PROJECT, int>("RID", int.Parse(strRID));

                if (ppModel != null)
                {
                    dirValues.Clear();
                    dirValues.Add("begin_date", strBegin);
                    dirValues.Add("end_date", strEnd);
                    dirValues.Add("Perso_Factory_RID", ppModel.Factory_RID);
                    DataTable dtblStepChk = dao.GetList(DEL_PROJECT_STEP_CHK, dirValues).Tables[0];
                    if (dtblStepChk.Rows[0][0].ToString() != "0")
                        throw new AlertException("代製費用已請款，不可刪除");
                }
            }


            dao.ExecuteNonQuery("delete from STEP_PERSO_PROJECT where PERSO_PROJECT_RID=" + strRID);

            dao.ExecuteNonQuery("delete from PERSO_PROJECT_PRICE where PERSO_PROJECT_RID=" + strRID);

            dao.ExecuteNonQuery("delete from PERSO_PROJECT where RID=" + strRID);

            //操作日誌
            SetOprLog("4");
    
            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    #endregion


    #region 製程
    /// <summary>
    /// 製程增加
    /// </summary>
    public void Add1(PROJECT_STEP psModel, bool IsNewAdd)
    {
        PERSO_PROJECT ppModel = new PERSO_PROJECT();
        DataSet dsPROJECT_STEP = null;
        DataSet dsPERSO_PROJECT_SAP = null;
        
        int intRID;

        //Perso廠RID
        string strPerso_RID = "";
        //製成名稱
        string strName = "";
        //上期日期起
        string strUse_Date_Begin = "";
        //上期日期迄
        string strUse_Date_End = "";
        //上期價格
        string strPrice = "";
        //上期RID
        string strRID = "";
        //SAP單卡片耗用日期迄值
        string strEnd_Date = "";
        //製成ID
        string strStepID = "";

        try
        {
            //事務開始
            dao.OpenConnection();

            //查新增之“使用期間起”是否大於“5.2.1代製費用帳務作業”中SAP單卡片耗用日期迄值
            dirValues.Clear();
            dirValues.Add("Perso_Factory_RID", psModel.Factory_RID);
            dsPERSO_PROJECT_SAP = dao.GetList("select Convert(varchar(20),max(end_date),111) from PERSO_PROJECT_SAP where Perso_Factory_RID=@Perso_Factory_RID", dirValues);
            if (dsPERSO_PROJECT_SAP.Tables[0].Rows[0][0].ToString() != "")
            {
                strEnd_Date = dsPERSO_PROJECT_SAP.Tables[0].Rows[0][0].ToString();
                if (psModel.Use_Date_Begin <= Convert.ToDateTime(strEnd_Date))
                    throw new AlertException("帳務已產生單，請修改價格期間迄值大於" + strEnd_Date + "！");
            }

            #region 製程檔添加
            if (!IsNewAdd)
            {
                //檢查新增之“使用期間起”是否大於上一筆資料的“使用期間起
                dirValues.Clear();
                dirValues.Add("perso_rid", psModel.Factory_RID);
                dirValues.Add("name", psModel.Name.Trim());
                dsPROJECT_STEP = dao.GetList(SEL_LAST_PROJECT_STEP, dirValues);
                if (dsPROJECT_STEP.Tables[0].Rows.Count != 0)
                {
                    strPerso_RID = dsPROJECT_STEP.Tables[0].Rows[0]["Factory_RID"].ToString();
                    strName = dsPROJECT_STEP.Tables[0].Rows[0]["Name"].ToString();
                    strUse_Date_Begin = dsPROJECT_STEP.Tables[0].Rows[0]["Use_Date_Begin"].ToString();
                    strUse_Date_End = dsPROJECT_STEP.Tables[0].Rows[0]["Use_Date_End"].ToString();
                    strRID = dsPROJECT_STEP.Tables[0].Rows[0]["RID"].ToString();
                    strPrice = dsPROJECT_STEP.Tables[0].Rows[0]["Price"].ToString();
                    strStepID = dsPROJECT_STEP.Tables[0].Rows[0]["Step_ID"].ToString();
                }
                else
                {
                    throw new AlertException("無上次記錄，添加失敗");
                }


                //輸入價格期間起值必須大於上筆資料價格期間起值xxxx/xx/xx
                if (Convert.ToDateTime(psModel.Use_Date_Begin) <= Convert.ToDateTime(strUse_Date_Begin))
                {
                    throw new AlertException("輸入價格期間起值必須大於上筆資料價格期間起值" + Convert.ToDateTime(strUse_Date_Begin).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                }

                //* RQ-2010-004324-000 7.代製費用的製程設定，新增、修改時可用相同單價 Delete by Ge.Song 2010/12/24 Start
                //單價不可與上筆資料相同
                //if (Convert.ToDecimal(dsPROJECT_STEP.Tables[0].Rows[0]["price"]) == psModel.Price)
                //{
                //    throw new AlertException("單價不可與上筆資料相同");
                //}
                //* RQ-2010-004324-000 7.代製費用的製程設定，新增、修改時可用相同單價 Delete by Ge.Song 2010/12/24 End

                //修改上一筆資料“使用期間迄”為新增資料“使用期間起”的前一天
                dirValues.Clear();
                dirValues.Add("Use_Date_End", psModel.Use_Date_Begin.AddDays(-1));
                dirValues.Add("RID", strRID);
                dao.ExecuteNonQuery("update PROJECT_STEP set Use_Date_End=@Use_Date_End where RID=@RID", dirValues);

                psModel.Step_ID = int.Parse(strStepID);
                intRID = Convert.ToInt32(dao.AddAndGetID<PROJECT_STEP>(psModel, "RID"));
            }
            else
            {
                DataTable dtblMaxStepID = dao.GetList("select max(step_id)+1 from PROJECT_STEP").Tables[0];
                if (!StringUtil.IsEmpty(dtblMaxStepID.Rows[0][0].ToString()))
                    psModel.Step_ID = int.Parse(dtblMaxStepID.Rows[0][0].ToString());
                else
                    psModel.Step_ID = 1;
                intRID = Convert.ToInt32(dao.AddAndGetID<PROJECT_STEP>(psModel, "RID"));
            }
            #endregion PROJECT_STEP檔添加


            CalNewPrice(psModel.Step_ID.ToString());

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }

    }

    /// <summary>
    /// 製程修改
    /// </summary>
    public void Update1(PROJECT_STEP psModel)
    {
        STEP_PERSO_PROJECT sprModel = new STEP_PERSO_PROJECT();
        PERSO_PROJECT ppModel = new PERSO_PROJECT();
        string strEnd_Date = "";
        DataSet dsPERSO_PROJECT_SAP = null;


        try
        {
            // 事務開始
            dao.OpenConnection();

            PROJECT_STEP psModel_o = dao.GetModel<PROJECT_STEP, int>("RID", psModel.RID);

            //查新增之“使用期間起”是否大於“5.2.1代製費用帳務作業”中SAP單卡片耗用日期迄值
            dirValues.Clear();
            dirValues.Add("Perso_Factory_RID", psModel_o.Factory_RID);
            dsPERSO_PROJECT_SAP = dao.GetList("select Convert(varchar(20),max(end_date),111) from PERSO_PROJECT_SAP where Perso_Factory_RID=@Perso_Factory_RID", dirValues);
            if (dsPERSO_PROJECT_SAP.Tables[0].Rows[0][0].ToString() != "")
            {
                strEnd_Date = dsPERSO_PROJECT_SAP.Tables[0].Rows[0][0].ToString();
                if (psModel.Use_Date_Begin <= Convert.ToDateTime(strEnd_Date))
                    throw new AlertException("帳務已產生" + strEnd_Date + "SAP單，請修改價格期間迄值大於此日期！");
            }

            //* RQ-2010-004324-000 7.代製費用的製程設定，新增、修改時可用相同單價 Delete by Ge.Song 2010/12/24 Start
            //單價不可與上筆資料相同
            //dirValues.Clear();
            //dirValues.Add("perso_rid", psModel_o.Factory_RID);
            //dirValues.Add("name", psModel_o.Name.Trim());
            //DataSet dsPROJECT_STEP = dao.GetList(SEL_LAST_PROJECT_STEP, dirValues);
            //if (dsPROJECT_STEP.Tables[0].Rows.Count > 1)
            //{
            //    if (Convert.ToDecimal(dsPROJECT_STEP.Tables[0].Rows[1]["price"]) == psModel.Price)
            //    {
            //        throw new AlertException("單價不可與上筆資料相同");
            //    }
            //}
            //* RQ-2010-004324-000 7.代製費用的製程設定，新增、修改時可用相同單價 Delete by Ge.Song 2010/12/24 Start

            //dirValues.Clear();
            //dirValues.Add("perso_rid", psModel.Factory_RID);
            //dirValues.Add("name", psModel.Name);
            //dirValues.Add("price", psModel.Price);
            //dirValues.Add("rid", psModel.RID);
            //DataSet dst = dao.GetList("select count(*) from PROJECT_STEP where Factory_RID = @perso_rid and Name = @name and price=@price and rid!=@rid", dirValues);
            //if (dst.Tables[0].Rows.Count > 0)
            //{
            //    if (dst.Tables[0].Rows[0][0].ToString() != "0")
            //    {
            //        throw new AlertException("單價不允許與其他製程重複");
            //    }
            //}
           
            psModel.RCT = psModel_o.RCT;
            psModel.RCU = psModel_o.RCU;
            psModel.RST = psModel_o.RST;
            psModel.Step_ID = psModel_o.Step_ID;
            psModel.Factory_RID = psModel_o.Factory_RID;

            

            dao.Update<PROJECT_STEP>(psModel, "RID");

            CalNewPrice(psModel.Step_ID.ToString());

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();

            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 製程刪除
    /// </summary>
    /// <param name="strRID">Perso項目種類RID</param>
    public void Delete1(String strRID)
    {
        try
        {
            // 事務開始
            dao.OpenConnection();


            PROJECT_STEP psModel = dao.GetModel<PROJECT_STEP, int>("RID", int.Parse(strRID));

            //throw new AlertException("價格期間已請款，不能刪除");
            dirValues.Clear();
            dirValues.Add("begin_date",psModel.Use_Date_Begin);
            dirValues.Add("end_date", psModel.Use_Date_End);
            dirValues.Add("Perso_Factory_RID", psModel.Factory_RID);
            DataTable dtblStepChk = dao.GetList(DEL_PROJECT_STEP_CHK, dirValues).Tables[0];
            if(dtblStepChk.Rows[0][0].ToString()!="0")
                throw new AlertException("價格期間已請款，不能刪除");
            

            dirValues.Clear();
            dirValues.Add("step_rid", psModel.Step_ID);
            DataTable dtblStep = dao.GetList(SEL_PERSO_PROJECT_BY_STEPRID, dirValues).Tables[0];

            if (dtblStep.Rows.Count > 0)
            {
                DataTable dtblPersoProject = dao.GetList("select count(*) from PROJECT_STEP where step_id=" + psModel.Step_ID.ToString()).Tables[0];
                if(dtblPersoProject.Rows[0][0].ToString()=="1")
                    throw new AlertException("製程已被代製項目使用，不能全部刪除");
            }
           
            dirValues.Clear();

            dirValues.Add("RID", int.Parse(strRID));

            dao.Delete("PROJECT_STEP", dirValues);


            CalNewPrice(psModel.Step_ID.ToString());

            //操作日誌
            SetOprLog("4");

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();

            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    #endregion



    /// <summary>
    /// 檢查代製項目是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelPROJECTSTEP(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("PERSO_PROJECT_rid", strRID);

        DataSet dstBudget = dao.GetList(CHK_PROJECTSTEP_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "代製項目"));

    }


    public DataSet GetStepData(string strPersoRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("Perso_RID", strPersoRID);
            DataSet dst = dao.GetList("select distinct step_id,name from PROJECT_STEP where Factory_RID=@Perso_RID", dirValues);

            return dst;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 計算製成區間價格
    /// </summary>
    /// <param name="dtblStepQujian"></param>
    /// <param name="strStepID"></param>
    /// <returns></returns>
    public DataTable GetStepTime(string strStepID)
    {
        DataSet dst = null;
        try
        {
            DataTable dtblStepQujian = new DataTable();
            dtblStepQujian.Columns.Add("Qujian");
            dtblStepQujian.Columns.Add("Begin");
            dtblStepQujian.Columns.Add("End");
            dtblStepQujian.Columns.Add("Price");
            //dtblStepQujian.Columns.Add("RowIndex");

            DataTable dtblStepQujian1 = dtblStepQujian.Clone();
            dtblStepQujian1.Columns.Add("RowIndex");
            
            //製成區間
            DataSet dstStepTime0 = null;
            DataSet dstStepTime1 = null;

            //所有製成時間
            DataTable dtblStepDate = new DataTable();
            dtblStepDate.Columns.Add("StepData");

            //查詢製成區間
            if (!StringUtil.IsEmpty(strStepID))
            {
                dstStepTime0 = dao.GetList(String.Format(SEL_STEPDATE_BY_STEPID, strStepID));
                dstStepTime1 = dao.GetList(String.Format(SEL_STEPDATE_BY_STEPID1, strStepID));
            }

            //遍歷生成製成區間
            if (dstStepTime0 != null)
            {
                foreach (DataRow drowStepTime in dstStepTime0.Tables[0].Rows)
                {
                    DataRow drowStepDate1 = dtblStepDate.NewRow();
                    drowStepDate1[0] = drowStepTime[0].ToString();
                    dtblStepDate.Rows.Add(drowStepDate1);
                }

                //生成製成價格區間
                if (dtblStepDate.Rows.Count != 0)
                {
                    for (int i = 0; i < dtblStepDate.Rows.Count - 1; i++)
                    {
                        string strBegin = dtblStepDate.Rows[i][0].ToString();
                        string strEnd = dtblStepDate.Rows[i + 1][0].ToString();


                        DataRow[] drowsStepTime = dstStepTime1.Tables[0].Select(" Use_Date_Begin<='" + strBegin + "' and Use_Date_End>='" + strEnd + "'");
                        if (drowsStepTime.Length > 0)
                        {
                            DataRow drowStepQujian = dtblStepQujian.NewRow();
                            drowStepQujian[0] = strBegin + "~" + strEnd;
                            drowStepQujian[1] = strBegin;
                            drowStepQujian[2] = strEnd;
                            drowStepQujian[3] = 0.00M;

                            for (int j = 0; j < drowsStepTime.Length; j++)
                            {
                                drowStepQujian[3] = Convert.ToDecimal(drowStepQujian[3]) + Convert.ToDecimal(drowsStepTime[j]["Price"]);
                            }

                            dtblStepQujian.Rows.Add(drowStepQujian);
                        }
                    }

                    int n = 0;

                    for (int i = 0; i < dtblStepQujian.Rows.Count; i++)
                    {
                        //當前
                        DataRow drowNow = dtblStepQujian.Rows[i];

                        //新列
                        DataRow drowNew = dtblStepQujian1.NewRow();

                        //當前列首尾相同
                        if (drowNow[1].ToString() == drowNow[2].ToString())
                        {
                            decimal decPrice = 0.0000M;
                            DataRow[] drowsStepTime = dstStepTime1.Tables[0].Select(" Use_Date_Begin<='" + drowNow[1].ToString() + "' and Use_Date_End>='" + drowNow[1].ToString() + "'");
                            for (int j = 0; j < drowsStepTime.Length; j++)
                            {
                                decPrice += Convert.ToDecimal(drowsStepTime[j]["Price"]);
                            }
                            if (decPrice.ToString() != drowNow[3].ToString())
                                continue;

                            drowNew[0] = drowNow[0].ToString();
                            drowNew[1] = drowNow[1].ToString();
                            drowNew[2] = drowNow[2].ToString();
                            drowNew[3] = drowNow[3].ToString();
                            drowNew[4] = n;
                            dtblStepQujian1.Rows.Add(drowNew);
                            n++;

                            if (i != dtblStepQujian.Rows.Count - 1)
                            {
                                DataRow drowNext = dtblStepQujian.Rows[i + 1];

                                if (drowNow[2].ToString() == drowNext[1].ToString())
                                {
                                    drowNext[1] = Convert.ToDateTime(drowNext[1]).AddDays(1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                    drowNext[0] = drowNext[1].ToString() + "~" + drowNext[2].ToString();
                                }
                            }
                            continue;
                        }

                        //如果被其他列包含
                        if (dtblStepQujian.Select(" begin='" + drowNew[2].ToString() + "' and end='" + drowNew[1].ToString() + "'").Length > 0)
                            continue;
                       
                        if (i == 0)
                        {
                            if (dtblStepQujian.Rows.Count > 1)
                            {
                                DataRow drowNext = dtblStepQujian.Rows[i + 1];
                                
                                if (drowNow[2].ToString() == drowNext[1].ToString())
                                {
                                    if (drowNext[1].ToString() == drowNext[2].ToString())
                                    {
                                        drowNow[2] = Convert.ToDateTime(drowNow[2]).AddDays(-1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                        drowNow[0] = drowNow[1].ToString() + "~" + drowNow[2].ToString();
                                    }
                                    else
                                    {

                                        decimal decPrice = 0.0000M;
                                        DataRow[] drowsStepTime = dstStepTime1.Tables[0].Select(" Use_Date_Begin<='" + drowNext[1].ToString() + "' and Use_Date_End>='" + drowNext[1].ToString() + "'");
                                        for (int j = 0; j < drowsStepTime.Length; j++)
                                        {
                                            decPrice += Convert.ToDecimal(drowsStepTime[j]["Price"]);
                                        }

                                        if (decPrice.ToString() == dtblStepQujian.Rows[i][3].ToString())    //單價與當前相同
                                        {
                                            drowNext[1] = Convert.ToDateTime(drowNext[1]).AddDays(1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                            drowNext[0] = drowNext[1].ToString() + "~" + drowNext[2].ToString();
                                        }
                                        else                                                     //單價與下個區間相同
                                        {
                                            drowNow[2] = Convert.ToDateTime(drowNow[2]).AddDays(-1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                            drowNow[0] = drowNow[1].ToString() + "~" + drowNow[2].ToString();
                                        }
                                    }
                                }
                            }
                        }
                        else if (i == dtblStepQujian.Rows.Count - 1)
                        {
                            
                        }
                        else
                        {
                            DataRow drowNext = dtblStepQujian.Rows[i + 1];

                            if (drowNow[2].ToString() == drowNext[1].ToString())
                            {
                                if (drowNext[1].ToString() == drowNext[2].ToString())
                                {
                                    drowNow[2] = Convert.ToDateTime(drowNow[2]).AddDays(-1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                    drowNow[0] = drowNow[1].ToString() + "~" + drowNow[2].ToString();
                                }
                                else
                                {

                                    decimal decPrice = 0.0000M;
                                    DataRow[] drowsStepTime = dstStepTime1.Tables[0].Select(" Use_Date_Begin<='" + drowNext[1].ToString() + "' and Use_Date_End>='" + drowNext[1].ToString() + "'");
                                    for (int j = 0; j < drowsStepTime.Length; j++)
                                    {
                                        decPrice += Convert.ToDecimal(drowsStepTime[j]["Price"]);
                                    }

                                    if (decPrice.ToString() == dtblStepQujian.Rows[i][3].ToString())    //單價與當前相同
                                    {
                                        drowNext[1] = Convert.ToDateTime(drowNext[1]).AddDays(1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                        drowNext[0] = drowNext[1].ToString() + "~" + drowNext[2].ToString();
                                    }
                                    else                                                     //單價與下個區間相同
                                    {
                                        drowNow[2] = Convert.ToDateTime(drowNow[2]).AddDays(-1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                        drowNow[0] = drowNow[1].ToString() + "~" + drowNow[2].ToString();
                                    }
                                }
                            }
                        }

                        drowNew[0] = drowNow[0].ToString();
                        drowNew[1] = drowNow[1].ToString();
                        drowNew[2] = drowNow[2].ToString();
                        drowNew[3] = drowNow[3].ToString();
                        drowNew[4] = n;
                        dtblStepQujian1.Rows.Add(drowNew);
                        n++;
                    }




                    ////找出相等的列
                    //DataRow[] drowEqual = dtblStepQujian1.Select("Begin=End");
                    //for (int i=0; i < drowEqual.Length; i++)
                    //{
                    //    int intRowIndex = Convert.ToInt32(drowEqual[i][4]);

                    //    //前一資料日期減一天
                    //    if (intRowIndex > 0)
                    //    {
                    //        DataRow drowPre = dtblStepQujian1.Rows[intRowIndex - 1];
                    //        if (drowPre[2].ToString() == drowEqual[i][1].ToString())
                    //        {
                    //            drowPre[2] = Convert.ToDateTime(drowPre[2]).AddDays(-1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    //            drowPre[0] = drowPre[1].ToString() + "~" + drowPre[2].ToString();
                    //        }
                    //    }

                    //    //後一天日期加一
                    //    if (intRowIndex != dtblStepQujian1.Rows.Count - 1)
                    //    {
                    //        DataRow drowNext = dtblStepQujian1.Rows[intRowIndex + 1];
                    //        if (drowNext[1].ToString() == drowEqual[i][1].ToString())
                    //        {
                    //            drowNext[1] = Convert.ToDateTime(drowNext[1]).AddDays(1).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    //            drowNext[0] = drowNext[1].ToString() + "~" + drowNext[2].ToString();
                    //        }
                    //    }
                    //}
                }
            }

            return dtblStepQujian1;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 代指項目修改資料讀入
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DataSet LoadPersoProject(string strRID)
    {
        try
        {
            DataSet dst = null;
            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dst=dao.GetList(SEL_PERSOPROJECT_BY_RID,dirValues);
            return dst;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 待製項目是否存在
    /// </summary>
    /// <param name="strFactoryID"></param>
    /// <param name="strProjectName"></param>
    /// <returns>true:存在 false:不存在</returns>
    public bool IsStepExist(string strFactoryID, string strProjectName)
    {
        try
        {
            DataSet dst = null;
            dirValues.Clear();
            dirValues.Add("factory_rid", strFactoryID);
            dirValues.Add("project_name", strProjectName);
            dst = dao.GetList(CON_PERSOPROJECTSTEP_BY_NAME, dirValues);
            if (dst.Tables[0].Rows[0][0].ToString() == "0")
                return false;
            else
                return true;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 獲取Perso廠下的待製項目
    /// </summary>
    /// <param name="strFactroyID"></param>
    /// <returns></returns>
    public DataTable GetPersoProject(string strFactroyID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("factory_rid", strFactroyID);
            DataSet dst = dao.GetList(SEL_PERSOPROJECT_BY_FACOTRYID, dirValues);
            return dst.Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 卡種修改讀入
    /// </summary>
    /// <param name="strRID"></param>
    /// <returns></returns>
    public DataTable LoadCardPerso(string strRID)
    {
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", strRID);
            DataSet dst = dao.GetList(SEL_CARDTYPE_PROJECT + " where CPT.RID=@RID", dirValues);
            return dst.Tables[0];
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


    /// <summary>
    /// 計算新的價格
    /// </summary>
    /// <param name="strStepID"></param>
    private void CalNewPrice(string strStepID)
    {
        if (StringUtil.IsEmpty(strStepID))
            return;


        dirValues.Clear();
        dirValues.Add("step_rid",strStepID);
        DataTable dtblStep = dao.GetList(SEL_PERSO_PROJECT_BY_STEPRID, dirValues).Tables[0];
       

        foreach (DataRow drow in dtblStep.Rows)
        {
            dao.ExecuteNonQuery("delete from PERSO_PROJECT_PRICE where PERSO_PROJECT_RID=" + drow["PERSO_PROJECT_RID"].ToString());

            DataTable dtblSteps = dao.GetList("select step_rid from STEP_PERSO_PROJECT where Perso_project_rid=" + drow["PERSO_PROJECT_RID"].ToString()).Tables[0];

            string strStepIDs = "";

            foreach (DataRow drowStep in dtblSteps.Rows)
            {
                strStepIDs += drowStep[0].ToString()+",";
            }

            if (StringUtil.IsEmpty(strStepIDs))
                continue;
            else
            {
                strStepIDs = strStepIDs.Substring(0, strStepIDs.Length - 1);
            }

            DataTable dtblStepQujian = GetStepTime(strStepIDs);

            //增加價格
            foreach (DataRow drowStepQujian in dtblStepQujian.Rows)
            {
                PERSO_PROJECT_PRICE pppModel = new PERSO_PROJECT_PRICE();
                pppModel.Perso_Project_RID = int.Parse(drow["PERSO_PROJECT_RID"].ToString());
                pppModel.Price = Convert.ToDecimal(drowStepQujian["price"]);
                pppModel.Use_Date_Begin = Convert.ToDateTime(drowStepQujian["Begin"]);
                pppModel.Use_Date_End = Convert.ToDateTime(drowStepQujian["End"]);
                dao.Add<PERSO_PROJECT_PRICE>(pppModel, "RID");
            }
        }
    }
}
