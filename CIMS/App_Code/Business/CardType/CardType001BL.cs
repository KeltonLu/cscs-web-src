//******************************************************************
//*  作    者：BingYiPan
//*  功能說明：廠商資料查詢頁面
//*  創建日期：2008-08-29
//*  修改日期：2008-09-02 16:00
//*  修改記錄：
//*            □2008-08-29
//*              1.創建 潘秉奕
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
using System.Collections;
using System.Collections.Generic;
using ControlLibrary;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

/// <summary>
/// CardType001BL 的摘要描述
/// </summary>
public class CardType001BL : BaseLogic
{
    #region SQL語句
    public const string SEL_INFO = "select * from param where rst='A' and paramtype_code='use'";
    public const string SEL_CARDGROUP_BY_NAME = "select * from CARD_GROUP where rst='A' and param_code=@param_code and group_name=@group_name";
    public const string SEL_PARAM = "select param_name,param_code,rid,rst from param where paramtype_code= (select param_code from param where param_name='用途' and rst='A')";
    public const string SEL_CARDGROUP = "select a.*,p.param_name from dbo.CARD_GROUP a join param p on a.param_code=p.param_code where a.param_code in (select param_code from param where rst='A') and a.rst='A'";    
    public const string SEL_CARDGROUP_BY_RID = "SELECT RID,Group_Name,Param_Code FROM CARD_GROUP WHERE RST = 'A'";
    //public const string SEL_CARDTYPE_BY_GROUPRID = "SELECT ct.RID as cardtype_rid,ct.name as cardtype_name,cg.Group_Name,cg.Param_Code FROM CARD_TYPE ct FROM CARD_TYPE ct join card_group cg on ct.CardType_Group_RID=cg.rid and cg.rst='A' WHERE ct.RST = 'A' and ct.CardType_Group_RID = @RID";
    public const string SEL_GROUP_CARD_TYPE = "select gct.* from GROUP_CARD_TYPE gct join CARD_GROUP cg on gct.Group_RID=cg.RID and cg.rst='A' where gct.rst='A' and cg.Group_RID=@Group_RID and cg.CardType_RID=@CardType_RID";
    public const string SEL_GROUPCARDTYPE_BY_NAME = "select ct.Display_Name,ct.rid,ct.name from card_type ct join GROUP_CARD_TYPE gct on gct.cardtype_rid=ct.rid and ct.rst='A' join card_group cg on gct.group_rid=cg.rid and cg.rst='A' where gct.rst='A' and cg.param_code=@param_code and cg.group_name=@group_name";    
    public const string SEL_CARDTYPE_ALL = "select * from CARD_TYPE where rid not in("
    + " select gct.cardtype_rid from GROUP_CARD_TYPE gct join card_group cg "
    + " on gct.group_rid=cg.rid and cg.rst='A'"
    + " where gct.rst='A' and cg.param_code =@param_code) and rst='A'";
    public const string SEL_CARDTYPE_BYUSE = "select * from CARD_TYPE where rid not in("
    + " select gct.cardtype_rid from GROUP_CARD_TYPE gct join card_group cg "
    + " on gct.group_rid=cg.rid and cg.rst='A' "
    + " where gct.rst='A' and cg.param_code =@param_code and cg.group_name=@group_name) and rst='A'";

    public const string CHK_GROUP_BY_RID = "proc_CHK_DEL_GROUP";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    CardTypeManager ctmManager = new CardTypeManager();

    public CardType001BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 用途下拉框綁定
    /// </summary>
    public void dropCard_PurposeBind(DropDownList dropCard)
    {
        dropCard.DataTextField = "PARAM_NAME";
        dropCard.DataValueField = "Param_Code";
        dropCard.DataSource = ctmManager.GetPurpose();
        dropCard.DataBind();
    }

    /// <summary>
    /// 查詢卡種群組記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Group_Name" : sortField);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_CARDGROUP);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (searchInput["drpParam_Name"].ToString().Trim() != "")
            {
                if (searchInput["drpParam_Name"].ToString().Trim() != "全部")
                {
                    stbWhere.Append(" and a.Param_code =@Param_code");
                    dirValues.Add("Param_code", searchInput["drpParam_Name"].ToString().Trim());
                }
                else
                {
                    stbWhere.Append(" and 1=1");
                    dirValues.Add("Param_code", searchInput["drpParam_Name"].ToString().Trim());
                }
            }
            else
            {
                stbWhere.Append(" and 1=1");
                dirValues.Add("Param_code", searchInput["drpParam_Name"].ToString().Trim());
            }
        }

        //執行SQL語句
        DataSet dstcard_Param = null;
        try
        {
            dstcard_Param = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstcard_Param;
    }

    ///// <summary>
    ///// 根據參數類型獲得對應參數信息
    ///// </summary>
    ///// <param name="strParamType">GlobalString.ParameterType.Use</param>
    ///// <returns></returns>
    //public DataSet ParamList(string strParamType)
    //{
    //    DataSet dst = null;
    //    try
    //    {
    //        //准備SQL語句
    //        StringBuilder stbCommand = new StringBuilder(SEL_PARAM_USE);

    //        //整理查詢條件
    //        StringBuilder stbWhere = new StringBuilder();

    //        dirValues.Clear();

    //        stbWhere.Append(" and ParamType_Code =@ParamType_Code");
    //        dirValues.Add("ParamType_Code", strParamType);

    //        dst = dao.GetList(stbCommand.ToString(), dirValues);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //    return dst;
    //}

    /// <summary>
    /// 獲取CARD_GROUP資料模型
    /// </summary>
    /// <param name="strRID">記錄ID</param>
    /// <returns>CARD_GROUP模型</returns>
    public CARD_GROUP GetParam(string strRID)
    {
        CARD_GROUP cgModel = null;
        try
        {
            cgModel = dao.GetModel<CARD_GROUP, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return cgModel;
    }

    /// <summary>
    /// 根據編號獲得名稱
    /// </summary>
    /// <param name="strParamCode">編號</param>
    /// <returns>名稱</returns>
    public string GetParamName(string strParamCode)
    {
        DataSet dstKzqz = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_INFO);

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();           

            stbWhere.Append(" and Param_Code =@Param_Code");
            dirValues.Add("Param_Code", strParamCode);

            dstKzqz = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        return dstKzqz.Tables[0].Rows[0]["param_name"].ToString();
    }

    /// <summary>
    /// 判斷該卡種群組是否存在
    /// </summary>
    /// <param name="strGroupName">群組名稱</param>
    /// <param name="strParamCode">卡種代號</param>
    /// <returns>是/否</returns>
    public bool IsHave(string strGroupName,string strParamCode)
    {
        DataSet dstKzqz = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Group_Name", strGroupName);
            dirValues.Add("param_code",strParamCode);

            dstKzqz = dao.GetList(SEL_CARDGROUP_BY_NAME, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }

        if (dstKzqz != null&&dstKzqz.Tables[0].Rows.Count>0)
            return true;
        return false;
    }

    public DataSet GetCardType(string strParamCode)
    {
        
        DataSet dst = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_CARDTYPE_ALL+" and is_using='Y'");            

            dirValues.Clear();            
            dirValues.Add("param_code", strParamCode);

            dst = dao.GetList(stbCommand.ToString() + " order by Display_Name", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 根據群組名稱與用途編號獲得對應所有卡種
    /// </summary>
    /// <param name="strGroupName">群組名稱</param>
    /// <param name="strParamCode">用途編號</param>
    /// <returns></returns>
    public DataSet CardTypeList(string strGroupName, string strParamCode)
    {
        DataSet dst = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_GROUPCARDTYPE_BY_NAME);
            
            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();
            dirValues.Add("Group_Name", strGroupName);
            dirValues.Add("param_code", strParamCode);

            dst = dao.GetList(stbCommand.ToString() + " order by Display_Name", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 查詢卡種群組信息
    /// </summary>
    /// <param name="strRID">卡種</param>
    /// <returns>DataSet</returns>
    public DataSet LoadParamInfoByPName(string strName)
    {
        DataSet dstKzqz = null;
        try
        {
            //准備SQL語句
            StringBuilder stbCommand = new StringBuilder(SEL_INFO);

            //整理查詢條件
            StringBuilder stbWhere = new StringBuilder();

            dirValues.Clear();

            stbWhere.Append(" and Param_Code =@Param_Code");
            dirValues.Add("Param_Code", strName);

            dstKzqz = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstKzqz;
    }

    ///// <summary>
    ///// 查詢卡種群組信息
    ///// </summary>
    ///// <param name="strRID">群組名稱</param>
    //public DataSet LoadParamInfoByGName(string strName)
    //{
    //    DataSet dstKzqz = null;
    //    try
    //    {
    //        dirValues.Clear();
    //        dirValues.Add("Group_Name", strName);

    //        dstKzqz = dao.GetList(SEL_PARAM_BY_RID, dirValues);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
    //        throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
    //    }
    //    return dstKzqz;
    //}

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="cgModel">卡種群組</param>   
    public void Add(CARD_GROUP cgModel,ArrayList al)
    {
        try
        {
            dao.OpenConnection();

            int RID = Convert.ToInt32(dao.AddAndGetID<CARD_GROUP>(cgModel, "RID"));

            for (int i = 0; i < al.Count; i++)
            {
                GROUP_CARD_TYPE gctModel = new GROUP_CARD_TYPE();
                gctModel.Group_RID = RID;
                gctModel.CardType_RID = Convert.ToInt32(al[i].ToString());
                dao.Add<GROUP_CARD_TYPE>(gctModel, "RID");
            }

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 修改
    /// </summary>    
    /// <param name="cbModel">修改後卡種群組</param>  
    public void Update(CARD_GROUP cgModel,ArrayList al)
    {
        try
        {
            dao.OpenConnection();

            //保存記錄
            dao.Update<CARD_GROUP>(cgModel, "RID");

            dao.ExecuteNonQuery("delete from GROUP_CARD_TYPE where Group_RID='" + cgModel.RID + "'");

            for (int i = 0; i < al.Count; i++)
            {
                GROUP_CARD_TYPE gctModel = new GROUP_CARD_TYPE();
                gctModel.Group_RID = cgModel.RID;
                gctModel.CardType_RID = Convert.ToInt32(al[i].ToString());
                dao.Add<GROUP_CARD_TYPE>(gctModel, "RID");
            }

            //操作日誌
            SetOprLog();

            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="strRID">卡種群組RID</param>    
    public void Delete(string strRID)
    {
        try
        {
            dao.OpenConnection();

            ChkDelGroup(strRID);

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("CARD_GROUP", dirValues);

            //cgModel = GetParam(strRID);

            ////進行邏輯刪除處理
            //cgModel.RST = "D";

            ////保存預算記錄
            //dao.Update<CARD_GROUP>(cgModel, "RID");

            dao.ExecuteNonQuery("delete from GROUP_CARD_TYPE where Group_RID='" + strRID + "'");

            //操作日誌
            SetOprLog("4");

            dao.Commit();
        }
        catch (AlertException ex)
        {
            //事務回滾
            dao.Rollback();
            throw ex;
        }
        catch (Exception ex)
        {
            dao.Rollback();
            //異常處理
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 檢查群組是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelGroup(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("CardType_Group_RID", strRID);

        DataSet dstBudget = dao.GetList(CHK_GROUP_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "群組"));

    }
}
