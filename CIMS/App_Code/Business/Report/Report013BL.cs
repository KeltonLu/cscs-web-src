//******************************************************************
//*  作    者：Ray
//*  功能說明：Report013 Business
//*  創建日期：2008/12/2
//*  修改日期：
//*  修改記錄：
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
/// Summary description for Report0090
/// </summary>
public class Report013 : BaseLogic
{
    public Report013()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    const string SEL_MATERIEL = "select * from (select Serial_Number,Name from ENVELOPE_INFO union select Serial_Number,Name from CARD_EXPONENT union select Serial_Number,Name from DMTYPE_INFO where Is_Using = 'N') as a order by Serial_Number";

    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public DataSet GetMATERIEL()
    {
        try
        {
            return dao.GetList(SEL_MATERIEL);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }


}
