//******************************************************************
//*  作    者：lantaosu
//*  功能說明：換卡預測月報表 
//*  創建日期：2008-11-27
//*  修改日期：2008-11-27 18:00
//*  修改記錄：
//*            □2008-11-27
//*              1.創建 蘇斕濤
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

/// <summary>
/// Report024BL 的摘要描述
/// </summary>
public class Report024BL
{
    #region SQL語句
    public const string SEL_COO_PERSO_FACTORY = "SELECT RID, Factory_ShortName_CN "
                                        + "FROM FACTORY "
                                        + "WHERE  RST = 'A'  AND Is_Perso='Y' AND Is_Cooperate='Y'";

    public const string SEL_IMPORT = "select COUNT(*) from FORE_CHANGE_CARD WHERE CHANGE_DATE=@CHANGE_DATE";
    #endregion

    DataBaseDAO dao = new DataBaseDAO();

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Report024BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public bool IsImport(string strMonth)
    {
        bool IsDefine = false;
        try
        {
            DataSet dst = null;
            dirValues.Clear();
            dirValues.Add("CHANGE_DATE", strMonth);
            dst = dao.GetList(SEL_IMPORT, dirValues);
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

    /// <summary>
    /// 查詢所有已建立的合作Perso廠中文簡稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetCooperatePersoList()
    {
        DataSet dstCooperatePersoList = null;

        try
        {
            dstCooperatePersoList = dao.GetList(SEL_COO_PERSO_FACTORY);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCooperatePersoList;
    }

}
