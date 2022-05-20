// 創建者：Judy
// 功能說明:帳號解鎖功能
// 創建時間：2018/03/21
// 修改時間：
// 修改者：
using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// BaseInfo050BL 的摘要说明
/// </summary>
public class BaseInfo050BL: BaseLogic
{

    // DB參數變量
    private Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo050BL()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 修改錯誤次數為0
    /// </summary>
    /// <param name="userId">當前登錄人帳號</param>
    /// <returns></returns>
    public int  UpdateUserErrorNum(string userId)
    {
        this.dirValues.Clear();
        this.dirValues.Add("UserID", userId);

        return dao.ExecuteNonQuery("update USERS set ErrorNum = 0 where UserID=@UserID", this.dirValues);      
    }

    /// <summary>
    /// 查詢登錄帳號
    /// Add by judy 2018/03/28
    /// </summary>
    /// <param name="userId">登錄帳號</param>
    /// <returns></returns>
    public string SelectUserId(string userId)
    {
        this.dirValues.Clear();
        this.dirValues.Add("UserID", userId);

        DataSet dsUserId = dao.GetList("select UserID from USERS where UserID=@UserID", this.dirValues);

        if (dsUserId != null && dsUserId.Tables[0].Rows.Count > 0)
        {
            return dsUserId.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            return "0";
        }
    }
}