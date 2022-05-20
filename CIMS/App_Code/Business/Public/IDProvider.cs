//******************************************************************
//*  作    者：QingChen
//*  功能說明：編碼管理器
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.EnterpriseLibrary.Data;

/// <summary>
/// IDProvider 的摘要描述
/// </summary>
public class IDProvider : BaseLogic
{
    /// <summary>
    /// 建構
    /// </summary>
    public IDProvider()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public static IDProvider MainIDProvider = new IDProvider();

    /// <summary>
    /// 獲取系統固定邏輯的編碼
    /// </summary>
    /// <param name="type">編碼類型</param>
    /// <returns>新編碼</returns>
    public string GetSystemNewID(string TypeName)
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("TypeName", TypeName);
        IDCONFIG idcNow = null;

        try
        {
            idcNow = dao.GetModel<IDCONFIG>("select * from IDConfig where TypeName=@TypeName", values);
            DateTime dtToDay = DateTime.Now.Date;
            if (idcNow.LastDate.Date != dtToDay)
            {
                idcNow.LastDate = dtToDay;
                idcNow.LastNumber = 1;
            }
            else
            {
                idcNow.LastNumber += 1;
                if (idcNow.LastNumber.ToString().Length > idcNow.LastNumberLength)
                {
                    throw new AlertException(string.Concat(BizMessage.BizPublicMsg.ALT_IDTooLong, ":", idcNow.TypeName));
                }
            }
            dao.Update<IDCONFIG>(idcNow, "ID");
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizPublicMsg.ALT_GetIDFail, ex, dao.LastCommands);
            throw ex;
        }
        if (idcNow != null)
        {
            string strLastNumber = idcNow.LastNumber.ToString().PadLeft(idcNow.LastNumberLength, '0');
            string strSeed = idcNow.LastDate.ToString(idcNow.FormatString.Trim());
            return string.Concat(strSeed, strLastNumber);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 獲取系統固定邏輯的編碼
    /// </summary>
    /// <param name="type">編碼類型</param>
    /// <returns>新編碼</returns>
    public string GetSystemNewID(string TypeName,string Code)
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("TypeName", TypeName);
        IDCONFIG idcNow = null;

        try
        {
            idcNow = dao.GetModel<IDCONFIG>("select * from IDConfig where TypeName=@TypeName", values);
            DateTime dtToDay = DateTime.Now.Date;
            if (idcNow.LastDate.Date != dtToDay)
            {
                idcNow.LastDate = dtToDay;
                idcNow.LastNumber = 1;
            }
            else
            {
                idcNow.LastNumber += 1;
                if (idcNow.LastNumber.ToString().Length > idcNow.LastNumberLength)
                {
                    throw new AlertException(string.Concat(BizMessage.BizPublicMsg.ALT_IDTooLong, ":", idcNow.TypeName));
                }
            }
            dao.Update<IDCONFIG>(idcNow, "ID");
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizPublicMsg.ALT_GetIDFail, ex, dao.LastCommands);
            throw ex;
        }
        if (idcNow != null)
        {
            string strLastNumber = idcNow.LastNumber.ToString().PadLeft(idcNow.LastNumberLength, '0');
            string strSeed = idcNow.LastDate.ToString(idcNow.FormatString.Trim());
            return string.Concat(strSeed,Code,strLastNumber);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 獲取系統固定邏輯的編碼
    /// </summary>
    /// <param name="type">編碼類型</param>
    /// <returns>新編碼</returns>
    public string GetSystemNewIDWithNoDate(string TypeName)
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("TypeName", TypeName);
        IDCONFIG idcNow = null;

        try
        {
            idcNow = dao.GetModel<IDCONFIG>("select * from IDConfig where TypeName=@TypeName", values);
            DateTime dtToDay = DateTime.Now.Date;
            idcNow.LastNumber += 1;
            if (idcNow.LastNumber.ToString().Length > idcNow.LastNumberLength)
            {
                throw new AlertException(string.Concat(BizMessage.BizPublicMsg.ALT_IDTooLong, ":", idcNow.TypeName));
            }
            dao.Update<IDCONFIG>(idcNow, "ID");
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizPublicMsg.ALT_GetIDFail, ex, dao.LastCommands);
            throw ex;
        }
        if (idcNow != null)
        {
            string strLastNumber = idcNow.LastNumber.ToString().PadLeft(idcNow.LastNumberLength, '0');
            string strSeed = idcNow.FormatString;
            return string.Concat(strSeed, strLastNumber);
        }
        else
        {
            return null;
        }
    }
}
