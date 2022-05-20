//******************************************************************
//*  作    者：FangBao
//*  功能說明：預算管理邏輯
//*  創建日期：2008-09-02
//*  修改日期：2008-09-02 12:00
//*  修改記錄：
//*            □2008-09-02
//*              1.創建 鮑方
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
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


/// <summary>
/// Depository003BL 的摘要描述
/// </summary>
public class Depository003BL:BaseLogic
{
    #region SQL語句
    public const string SEL_DEPOSITORY_STOCK = " SELECT DS.Is_AskFinance,DS.Is_Check,OFD.NUMBER,DS.COMMENT,DS.Check_Type,DS.RID,DS.Report_RID,DS.Stock_RID,DS.Space_Short_RID,DS.Income_Date,DS.Stock_Number,DS.Sample_Number,DS.Blemish_Number,DS.Income_Number,DS.Serial_Number,DS.Perso_Factory_RID,DS.Blank_Factory_RID,DS.Wafer_RID,FS_B.Factory_ShortName_cn AS Blank_Factory_NAME,FS_P.Factory_ShortName_cn AS Perso_Factory_NAME ,CH.SendCheck_Status,OFD.Case_Status,CT.NAME,WI.WAFER_NAME FROM DEPOSITORY_STOCK DS left JOIN FACTORY AS FS_B ON FS_B.RST = 'A' AND FS_B.IS_BLANK ='Y' AND FS_B.RID=DS.Blank_Factory_RID left JOIN FACTORY AS FS_P ON FS_P.RST = 'A' AND FS_P.IS_Perso ='Y' AND FS_P.RID=DS.Perso_Factory_RID left JOIN CARD_HALLMARK AS CH ON CH.RST = 'A' and CH.Serial_Number=DS.Serial_Number AND CH.CardType_RID=DS.Space_Short_RID left JOIN ORDER_FORM AS [OF] ON [OF].RST = 'A' AND [OF].OrderForm_RID = ds.OrderForm_RID left JOIN ORDER_FORM_DETAIL AS OFD ON OFD.RST = 'A' AND OFD.OrderForm_Detail_RID = ds.OrderForm_Detail_RID left join CARD_TYPE as ct on DS.Space_Short_RID=CT.RID AND CT.RST='A' left join WAFER_INFO as WI ON WI.RID=DS.WAFER_RID WHERE DS.RST='A' ";
    public const string SEL_FACTORY_BLANK = " select RID,Factory_ShortName_cn from FACTORY WHERE RST = 'A' AND IS_BLANK ='Y'";
    public const string SEL_FACTORY_PERSO = " select RID,Factory_ShortName_cn from FACTORY WHERE RST = 'A' AND IS_Perso ='Y'";

    public const string SEL_ORDER_FORM_DETAIL = " SELECT ct.Display_Name,ISNULL(AM.Remain_Card_Num,0) AS AMRemain_Card_Num,isnull(AM.Card_Number,0) as AMCard_Number ,isnull(cb.Total_Card_Num,0) as Total_Card_Num,isnull(cb.Remain_Total_AMT,0) as Remain_Total_AMT,isnull(cb.Remain_Total_Num,0) as Remain_Total_Num,OFD.Is_Edit_Budget,OFD.Unit_Price,OFORM.Pass_Status,OFD.ORDERFORM_DETAIL_RID,OFD.Case_Status,OFD.OrderForm_RID,OFD.Budget_RID,OFORM.Order_Date,OFD.Agreement_RID,OFD.Number,OFD.CardType_RID,OFD.Wafer_RID,AM.Factory_RID,WI.WAFER_NAME,CT.NAME,OFD.Delivery_Address_RID,F_P.Factory_id as Perso_Factory_ID,F_P.Factory_ShortName_CN as Perso_Factory_Name,F_B.Factory_id as Blank_Factory_ID,F_B.Factory_ShortName_CN as Blank_Factory_Name FROM ORDER_FORM_DETAIL OFD LEFT JOIN  agreement  AM ON AM.RID=OFD.Agreement_RID LEFT JOIN WAFER_INFO WI ON WI.RST='A' AND WI.RID=OFD.Wafer_RID LEFT join CARD_TYPE CT ON CT.RST='A' AND  CT.RID=OFD.CARDTYPE_RID LEFT join FACTORY F_P ON F_P.RST = 'A' AND F_P.IS_Perso ='Y' AND F_P.RID=OFD.Delivery_Address_RID LEFT join FACTORY F_B ON F_B.RST = 'A' AND F_B.IS_BLANK ='Y' AND F_B.RID=AM.Factory_RID inner JOIN ORDER_FORM OFORM ON OFORM.RST='A' AND OFORM.OrderForm_RID=OFD.OrderForm_RID left join CARD_BUDGET CB ON OFD.budget_rid=cb.rid and CB.RST='A' where OFD.OrderForm_Detail_RID=@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_STOCK_2 = " SELECT ISNULL(SUM(DS.Income_Number),0) FROM DEPOSITORY_STOCK AS DS WHERE DS.RST = 'A' AND DS.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_CANCEL = " SELECT ISNULL(SUM(DC.cancel_Number),0) FROM DEPOSITORY_CANCEL AS DC WHERE DC.RST = 'A' AND DC.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_RESTOCK = " SELECT ISNULL(SUM(DR.Reincome_Number),0) FROM DEPOSITORY_RESTOCK AS DR WHERE DR.RST = 'A' AND DR.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_CARD_HALLMARK = " SELECT CH.SendCheck_Status FROM CARD_HALLMARK AS CH WHERE CH.RST = 'A' AND CH.Serial_Number = @Serial_Number AND CH.CardType_RID = @CardType_RID";

    //add by Jacky Guo on 2008/11/09
    public const string SEL_CARD_HALLMARK_2 = " SELECT CH.SendCheck_Status FROM CARD_HALLMARK AS CH WHERE CH.RST = 'A' AND CH.Serial_Number = @Serial_Number AND CH.CardType_RID = ( select rid from card_type where name = @Space_Short_Name) ";


    public const string SEL_ORDER_FORM_DETAIL_2 = "SELECT OFD.OrderForm_Detail_RID FROM ORDER_FORM_DETAIL AS OFD INNER JOIN AGREEMENT AS AM ON AM.RST = 'A' AND AM.RID = OFD.Agreement_RID AND AM.Factory_RID = @Factory_RID WHERE OFD.RST = 'A' AND OFD.CardType_RID = @CardType_RID";


    public const string SEL_DEPOSITORY_STOCK_3 = "SELECT DS.RID,DS.OrderForm_Detail_RID,OFD.Number,CT.NAME FROM DEPOSITORY_STOCK DS LEFT join CARD_TYPE CT ON CT.RST='A' AND CT.RID=DS.Space_Short_RID left join ORDER_FORM_DETAIL OFD ON OFD.OrderForm_Detail_RID=DS.OrderForm_Detail_RID ";

    public const string SEL_STOCKMAXRID = " select ISNULL(max(Stock_RID),0) from dbo.DEPOSITORY_STOCK where OrderForm_Detail_RID=@OrderForm_Detail_RID";

    public const string SEL_ORDER_FORM_END = " select count(*) from ORDER_FORM_DETAIL where Case_Status='N' and OrderForm_RID=@OrderForm_RID";

    public const string SEL_WAFER_INFO_BYCRID = " select WI.RID,WI.WAFER_NAME from WAFER_INFO WI INNER JOIN WAFER_CARDTYPE WC ON WI.RID=WC.WAFER_RID AND WC.RST='A' WHERE WI.RST='A' AND WC.CARDTYPE_RID=@CARDTYPE_RID";

    public const string SEL_BUDGET = "select Card_Price,Card_Num,RID,Total_Card_AMT,Total_Card_Num,Remain_Total_AMT,Remain_Total_Num,Remain_Card_Price,Remain_Card_Num from dbo.CARD_BUDGET where RST='A' AND Budget_Main_RID= @Budget_Main_RID";

    public const string SEL_ORDER_BUDGET_LOG = "select * from ORDER_BUDGET_LOG WHERE RST='A' AND OrderForm_Detail_RID=@OrderForm_Detail_RID ";

    public const string SEL_AGREEMENT_BY_RID = "select factory_rid,Remain_Card_Num,Card_Number,rid from agreement where 1=1 ";


    public const string DEL_DEPOSITORY_STOCK = "proc_CHK_DEL_DEPOSITORY_STOCK";

    public const string SEL_ISCHECK = "select count(*) from CARDTYPE_STOCKS where stock_date between '{0} 00:00:00' and '{0} 23:59:59' ";

    public const string SEL_ISCHECK1 = "select count(*) from CARDTYPE_STOCKS where stock_date>=@stock_date";

    public const string SEL_BUDGET_LOG = "select * from dbo.ORDER_BUDGET_LOG where OrderForm_detail_RID=@OrderForm_detail_RID order by Budget_RID desc";

    public const string SEL_STOCK_RID = "select STOCK_RID from DEPOSITORY_STOCK where RID=@RID";
    #endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public Depository003BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 判斷是否日結
    /// </summary>
    /// <returns>true:日結 false :沒日結</returns>
    public bool IsCheck1(string strDate)
    {
        bool blcheck = false;
        try
        {
            dirValues.Clear();
            dirValues.Add("stock_date", strDate);
            DataTable dtbl = dao.GetList(SEL_ISCHECK1, dirValues).Tables[0];
            if (dtbl.Rows.Count > 0)
            {
                if (dtbl.Rows[0][0].ToString() == "0")
                    blcheck = false;
                else
                    blcheck = true;
            }
        }
        catch
        {

        }
        return blcheck;
    }

    /// <summary>
    /// 判斷是否日結
    /// </summary>
    /// <returns>true:日結 false :沒日結</returns>
    public bool IsCheck()
    {
         bool blcheck = false;
        try
        {
            DataTable dtbl = dao.GetList(string.Format(SEL_ISCHECK,DateTime.Now.ToString("yyyy-MM-dd"))).Tables[0];
            if (dtbl.Rows.Count > 0)
            {
                if (dtbl.Rows[0][0].ToString() == "0")
                    blcheck = false;
                else
                    blcheck = true;
            }
        }
        catch 
        {
           
        }
        return blcheck;
    }


    /// <summary>
    /// 獲取卡廠
    /// </summary>
    /// <returns></returns>
    public DataSet GetFACTORYData()
    {
        DataSet dst = null;
        try
        {
            dst = dao.GetList(SEL_FACTORY_BLANK + SEL_FACTORY_PERSO);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 根據卡種獲取晶體名稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetWAFER_INFOBYCRID(string strCARDTYPE_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("CARDTYPE_RID", strCARDTYPE_RID);
            dst = dao.GetList(SEL_WAFER_INFO_BYCRID, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }


    /// <summary>
    /// 修改頁面初始化
    /// </summary>
    /// <param name="strStock_RID"></param>
    /// <returns></returns>
    public DataSet GetModData(string strStock_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Stock_RID", strStock_RID);
            dirValues.Add("OrderForm_Detail_RID", strStock_RID.Substring(0,12));
            dst = dao.GetList(SEL_DEPOSITORY_STOCK + " and ds.Stock_RID=@Stock_RID " + SEL_DEPOSITORY_STOCK_2 + SEL_DEPOSITORY_CANCEL + SEL_DEPOSITORY_RESTOCK, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    /// <summary>
    /// 獲取批號狀態
    /// </summary>
    /// <param name="strSNum"></param>
    /// <param name="strCardTypeRID"></param>
    /// <returns></returns>
    public string GetCheckStatus(string strSNum, string strCardTypeRID)
    {
        string strCheckStatus = "";
        try
        {
            if (!StringUtil.IsEmpty(strSNum) && !StringUtil.IsEmpty(strCardTypeRID))
            {
                dirValues.Clear();
                dirValues.Add("Serial_Number", strSNum);
                dirValues.Add("CardType_RID", int.Parse(strCardTypeRID));
                DataSet dst = dao.GetList(SEL_CARD_HALLMARK, dirValues);

                if (dst.Tables[0].Rows.Count > 0)
                    strCheckStatus = dst.Tables[0].Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return strCheckStatus;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="strOrderFormDetailNo">流水編號</param>
    /// <returns></returns>
    public DataSet GetDetailByOrderFormDetailNo(string strOrderFormDetailNo)
    {

        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("OrderForm_Detail_RID", strOrderFormDetailNo);
            dst = dao.GetList(SEL_ORDER_FORM_DETAIL+SEL_DEPOSITORY_STOCK_2+SEL_DEPOSITORY_CANCEL+SEL_DEPOSITORY_RESTOCK, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }


    /// <summary>
    /// 結案查詢頁面
    /// </summary>
    /// <param name="strRID">入庫明細檔編號</param>
    /// <returns></returns>
    public DataSet GetOrderFormDetail(string strRID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dst = dao.GetList(SEL_DEPOSITORY_STOCK_3 + " where DS.RID IN (" + strRID+")");
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }


    /// <summary>
    /// 獲取訂單流水編號
    /// </summary>
    /// <param name="strFactory_RID">空白卡廠</param>
    /// <param name="strCardType_RID">版面簡稱</param>
    /// <returns>入庫明細檔</returns>
    public DataSet GetOrderFormDetailNo(string strFactory_RID, string strCardType_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Factory_RID", int.Parse(strFactory_RID));
            dirValues.Add("CardType_RID", int.Parse(strCardType_RID));
            dst= dao.GetList(SEL_ORDER_FORM_DETAIL_2,dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    public string GetSubstringByByte(string strReadLine, int begin, int length, out int nextBegin)
    {
        string strTemp1 = strReadLine.Substring(begin, length + length-StringUtil.GetByteLength(strReadLine.Substring(begin, length)));
        nextBegin = begin + strTemp1.Length;
        return strTemp1;
    }


    /// <summary>
    /// 導入資料并處理
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="dtblFileImp"></param>
    /// <returns></returns>
    public DataTable Imp(string strPath, DataTable dtblFileImp)
    {
        #region 驗證文件
        StreamReader sr=null;

        try
        {
            sr = new StreamReader(strPath, System.Text.Encoding.Default);
            string[] strLine;
            string strReadLine = "";
            int count = 1;
            string strErr = "";

            while ((strReadLine = sr.ReadLine()) != null)
            {
                if (StringUtil.IsEmpty(strReadLine))
                    continue;
                //匯入文件格式不正確
                if (StringUtil.GetByteLength(strReadLine) != 119)
                    throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);


                int nextBegin = 0;
                strLine = new string[9];
                strLine[0] = GetSubstringByByte(strReadLine, nextBegin, 14, out nextBegin).Trim();
                strLine[1] = GetSubstringByByte(strReadLine, nextBegin, 30, out nextBegin).Trim();
                strLine[2] = GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();
                strLine[3] = GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();
                strLine[4] = GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();
                strLine[5] = GetSubstringByByte(strReadLine, nextBegin, 8, out nextBegin).Trim();
                strLine[6] = GetSubstringByByte(strReadLine, nextBegin, 30, out nextBegin).Trim();
                strLine[7] = GetSubstringByByte(strReadLine, nextBegin, 5, out nextBegin).Trim();
                strLine[8] = GetSubstringByByte(strReadLine, nextBegin, 5, out nextBegin).Trim();

                DataRow dr = dtblFileImp.NewRow();

                for (int i = 0; i < strLine.Length; i++)
                {
                    int num = i + 1;
                    if (StringUtil.IsEmpty(strLine[i]))
                        strErr += "第" + count.ToString() + "行第" + num.ToString() + "列為空;\\n";
                    else
                        strErr += CheckFileColumn(strLine[i], num, count);

                    dr[i] = strLine[i];
                }

                if (!StringUtil.IsEmpty(strErr))
                    throw new AlertException(strErr);


                dr[5] = DateTime.ParseExact(dr[5].ToString(), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.
InvariantInfo);
                if (IsCheck1(Convert.ToDateTime(dr[5]).ToString("yyyy-MM-dd")))
                    strErr += "第" + count.ToString() + "行入庫日期已經日結;\\n";


                int intIncomeNum = Convert.ToInt32(dr[2]) - Convert.ToInt32(dr[3]) - Convert.ToInt32(dr[4]);

                if (intIncomeNum < 0)
                    strErr += "第" + count.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_12 + ";\\n";


                dtblFileImp.Rows.Add(dr);

                count++;
            }
            if (!StringUtil.IsEmpty(strErr))
                throw new AlertException(strErr);
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            sr.Close();
        }

        #endregion

        #region 驗證資料格式并生成資料
        DataSet dstDepository = null;

        DataTable dtblReturn = new DataTable();
        dtblReturn.Columns.Add("OrderForm_Detail_RID");
        dtblReturn.Columns.Add("Space_Short_RID");
        dtblReturn.Columns.Add("Space_Short_Name");
        dtblReturn.Columns.Add("TotalNum");
        dtblReturn.Columns.Add("RemainNum");
        dtblReturn.Columns.Add("Stock_Number");
        dtblReturn.Columns.Add("Blemish_Number");
        dtblReturn.Columns.Add("Sample_Number");
        dtblReturn.Columns.Add("Income_Number");
        dtblReturn.Columns.Add("Income_Date");
        dtblReturn.Columns.Add("Serial_Number");
        dtblReturn.Columns.Add("Perso_Factory_RID");
        dtblReturn.Columns.Add("Perso_Factory_Name");
        dtblReturn.Columns.Add("Blank_Factory_RID");
        dtblReturn.Columns.Add("Blank_Factory_Name");
        dtblReturn.Columns.Add("Wafer_RID");
        dtblReturn.Columns.Add("Wafer_Name");
        dtblReturn.Columns.Add("Check_Type");
        dtblReturn.Columns.Add("Comment");
        dtblReturn.Columns.Add("SendCheck_Status");
        dtblReturn.Columns.Add("Budget_RID");
        dtblReturn.Columns.Add("Order_Date");
        dtblReturn.Columns.Add("Agreement_RID");
        dtblReturn.Columns.Add("OrderForm_RID");
        dtblReturn.Columns.Add("Error");


        try
        {
            int count1 = 1;
            foreach (DataRow drowFile in dtblFileImp.Rows)
            {
                if (dtblFileImp.Select("OrderForm_Detail_RID='" + drowFile[0].ToString() + "'").Length > 1)
                    throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_07);

                dirValues.Clear();
                dirValues.Add("OrderForm_Detail_RID", drowFile["OrderForm_Detail_RID"].ToString());
                dirValues.Add("Space_Short_Name", drowFile["Space_Short_Name"].ToString());
                dirValues.Add("Serial_Number", drowFile["Serial_Number"].ToString());

                dstDepository = dao.GetList(SEL_ORDER_FORM_DETAIL + SEL_DEPOSITORY_STOCK_2 + SEL_DEPOSITORY_CANCEL + SEL_DEPOSITORY_RESTOCK + SEL_CARD_HALLMARK_2, dirValues);


                //1.訂單流水編號不存在
                if (dstDepository.Tables[0].Rows.Count == 0)
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_02);

                //2.訂單流水編號未放行或者已結案
                if (dstDepository.Tables[0].Rows[0]["Pass_Status"] == null || dstDepository.Tables[0].Rows[0]["Pass_Status"].ToString() != "4")
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_10);
                if (dstDepository.Tables[0].Rows[0]["Case_Status"] == null || dstDepository.Tables[0].Rows[0]["Case_Status"].ToString() != "N")
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_11);

                //3.不可使用該版面簡稱
                if (dstDepository.Tables[0].Rows[0]["NAME"].ToString() != drowFile["Space_Short_Name"].ToString() || dstDepository.Tables[0].Rows[0]["CardType_RID"].ToString() == "" )
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_04);

                //4.不可使用該空白卡廠
                if (dstDepository.Tables[0].Rows[0]["Blank_Factory_Name"].ToString() == "" || dstDepository.Tables[0].Rows[0]["Blank_Factory_ID"].ToString() != drowFile["Blank_Factory_RID"].ToString())
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_03);

                //5.不可使用該Perso廠
                if (dstDepository.Tables[0].Rows[0]["Perso_Factory_Name"].ToString() == "" || dstDepository.Tables[0].Rows[0]["Perso_Factory_ID"].ToString() != drowFile["Perso_Factory_RID"].ToString())
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_08);


                //6.晶片不存在
                if (dstDepository.Tables[0].Rows[0]["WAFER_NAME"].ToString() == "")
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_05);

                //批號不存在
                if (dstDepository.Tables[4].Rows.Count == 0)
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_06);



                DataRow drowReturn = dtblReturn.NewRow();
                drowReturn["OrderForm_Detail_RID"] = drowFile["OrderForm_Detail_RID"].ToString();
                drowReturn["Space_Short_RID"] = dstDepository.Tables[0].Rows[0]["CardType_RID"].ToString();
                drowReturn["Space_Short_Name"] = dstDepository.Tables[0].Rows[0]["Name"].ToString();
                drowReturn["TotalNum"] = int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString());
                drowReturn["RemainNum"] = int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()) - int.Parse(dstDepository.Tables[1].Rows[0][0].ToString()) + int.Parse(dstDepository.Tables[2].Rows[0][0].ToString()) - int.Parse(dstDepository.Tables[3].Rows[0][0].ToString());
                drowReturn["Stock_Number"] = int.Parse(drowFile["Stock_Number"].ToString());
                drowReturn["Blemish_Number"] = int.Parse(drowFile["Blemish_Number"].ToString());
                drowReturn["Sample_Number"] = int.Parse(drowFile["Sample_Number"].ToString());
                drowReturn["Income_Number"] = int.Parse(drowFile["Stock_Number"].ToString()) - int.Parse(drowFile["Blemish_Number"].ToString()) - int.Parse(drowFile["Sample_Number"].ToString());
                drowReturn["Income_Date"] = Convert.ToDateTime(drowFile["Income_Date"].ToString());
                drowReturn["Serial_Number"] = drowFile["Serial_Number"].ToString();
                drowReturn["Perso_Factory_RID"] = dstDepository.Tables[0].Rows[0]["Delivery_Address_RID"].ToString();
                drowReturn["Perso_Factory_Name"] = dstDepository.Tables[0].Rows[0]["Perso_Factory_Name"].ToString();
                drowReturn["Blank_Factory_RID"] = dstDepository.Tables[0].Rows[0]["Factory_RID"].ToString();
                drowReturn["Blank_Factory_Name"] = dstDepository.Tables[0].Rows[0]["Blank_Factory_Name"].ToString();
                drowReturn["Wafer_RID"] = int.Parse(dstDepository.Tables[0].Rows[0]["Wafer_RID"].ToString());
                drowReturn["Wafer_Name"] = dstDepository.Tables[0].Rows[0]["Wafer_Name"].ToString();
                drowReturn["SendCheck_Status"] = dstDepository.Tables[4].Rows[0]["SendCheck_Status"].ToString();
                drowReturn["Budget_RID"] = int.Parse(dstDepository.Tables[0].Rows[0]["Budget_RID"].ToString());
                drowReturn["Order_Date"] = Convert.ToDateTime(dstDepository.Tables[0].Rows[0]["Order_Date"].ToString());
                drowReturn["Agreement_RID"] = int.Parse(dstDepository.Tables[0].Rows[0]["Agreement_RID"].ToString());
                drowReturn["OrderForm_RID"] = int.Parse(dstDepository.Tables[0].Rows[0]["OrderForm_RID"].ToString());
                drowReturn["Check_Type"] = "1";

                

                #region 檢查是否超過預算
                //預算總卡數
                int intTotal_Num = Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Total_Card_Num"]);
                //合約總卡數
                int intAMTotal_Num = Convert.ToInt32(dstDepository.Tables[0].Rows[0]["AMCard_Number"]);
                //合約剩餘卡數
                int intAMTNum = int.Parse(dstDepository.Tables[0].Rows[0]["AMRemain_Card_Num"].ToString());
                

                //預算總剩餘金額及卡數
                int intBudgetNum = int.Parse(dstDepository.Tables[0].Rows[0]["Remain_Total_Num"].ToString());
                decimal decBudgetAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Remain_Total_AMT"].ToString());
                

                //用過的金額及卡數
                int intUsedNum = int.Parse(dstDepository.Tables[1].Rows[0][0].ToString()) - int.Parse(dstDepository.Tables[2].Rows[0][0].ToString()) + int.Parse(dstDepository.Tables[2].Rows[0][0].ToString());
                decimal decUsedAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intUsedNum;

                //這次錄入的金額及卡數
                int intNowNum = int.Parse(drowReturn["Income_Number"].ToString());
                decimal decNowAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intNowNum;

                

                #region 驗證預算
                if (dstDepository.Tables[0].Rows[0]["Is_Edit_Budget"].ToString() == "Y")
                {
                    if (intTotal_Num != 0)
                    {
                        if (intBudgetNum < intNowNum)
                            drowReturn["Error"] = String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, drowFile["OrderForm_Detail_RID"].ToString());
                    }

                    //合約卡數是否可用
                    if (intAMTotal_Num != 0)
                    {
                        if (intAMTNum < intNowNum)
                            drowReturn["Error"] = String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_14, drowFile["OrderForm_Detail_RID"].ToString());
                    }

                    if (decBudgetAmt < decNowAmt)
                        drowReturn["Error"] = String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, drowFile["OrderForm_Detail_RID"].ToString());
                }
                else
                {
                    if ((intUsedNum + intNowNum) > int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()))
                    {
                        if (intUsedNum < int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()))
                            intNowNum = intUsedNum + intNowNum - int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString());

                        decNowAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intNowNum;

                        if (intTotal_Num != 0)
                        {
                            if (intBudgetNum < intNowNum)
                                drowReturn["Error"] = String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, drowFile["OrderForm_Detail_RID"].ToString());
                        }

                        if (intAMTotal_Num != 0)
                        {
                            if (intAMTNum < intNowNum)
                                drowReturn["Error"] = String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_14, drowFile["OrderForm_Detail_RID"].ToString());
                        }

                        if (decBudgetAmt < decNowAmt)
                            drowReturn["Error"] = String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, drowFile["OrderForm_Detail_RID"].ToString());
                    }
                }
                #endregion

                dtblReturn.Rows.Add(drowReturn);

                #endregion

                count1++;

            }
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        #endregion

        return dtblReturn;
    }


   /// <summary>
   /// 更新預算日誌
   /// </summary>
   /// <param name="strofdRID">訂單流水號</param>
   /// <param name="intBudgetRID">預算ID</param>
   /// <param name="decBudgetAmt">預算金額</param>
   /// <param name="intNowNum">預算卡數</param>
    private void MergBUDGET_LOG(string strofdRID, int intBudgetRID,decimal decBudgetAmt, int intNowNum)
    {
        dirValues.Clear();
        dirValues.Add("Budget_RID", intBudgetRID);
        dirValues.Add("OrderForm_Detail_RID", strofdRID);

        ORDER_BUDGET_LOG orlModel = dao.GetModel<ORDER_BUDGET_LOG>(SEL_ORDER_BUDGET_LOG + " and Budget_RID=@Budget_RID", dirValues);

        if (orlModel == null)
        {
            orlModel = new ORDER_BUDGET_LOG();
            orlModel.Budget_RID = intBudgetRID;
            orlModel.OrderForm_Detail_RID = strofdRID;
            orlModel.Remain_Card_Num = intNowNum;
            orlModel.Remain_Card_Price = decBudgetAmt;
            dao.Add<ORDER_BUDGET_LOG>(orlModel, "RID");
        }
        else
        {
            orlModel.Remain_Card_Num = orlModel.Remain_Card_Num+intNowNum;
            orlModel.Remain_Card_Price = orlModel.Remain_Card_Price+decBudgetAmt;
            if (orlModel.Remain_Card_Num < 0 || orlModel.Remain_Card_Price < 0)
                throw new AlertException("日誌不能為負數,更新失敗");
            dao.Update<ORDER_BUDGET_LOG>(orlModel, "RID");
        }
        //當扣減金額和數據都為零時，刪除日志記錄。2009/11/12
        dirValues.Clear();
        dirValues.Add("Budget_RID", intBudgetRID);
        dirValues.Add("OrderForm_Detail_RID", strofdRID);
        ORDER_BUDGET_LOG orlModelDel = dao.GetModel<ORDER_BUDGET_LOG>(SEL_ORDER_BUDGET_LOG + " and Budget_RID=@Budget_RID", dirValues);
        if (orlModelDel != null)
        {
            if (orlModelDel.Remain_Card_Num == 0 && Math.Round(orlModelDel.Remain_Card_Price) == 0)
            {
                dao.Delete("ORDER_BUDGET_LOG", dirValues);
            }
        }

    }


    /// <summary>
    /// 減少的預算
    /// </summary>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intBudgetRID">主預算ID</param>
    /// <param name="intReductNum">減少數量</param>
    /// <param name="decReductAmt">減少金額</param>
    public void ReduceBudget(string strofdRID, int intBudgetRID, int intReductNum, decimal decReductAmt)
    {
        dirValues.Clear();
        dirValues.Add("OrderForm_detail_RID", strofdRID);
        DataTable dtblBudgetLog = dao.GetList(SEL_BUDGET_LOG, dirValues).Tables[0];
        
        CARD_BUDGET cbMainModel = dao.GetModel<CARD_BUDGET, int>("RID", intBudgetRID);
        //當主預算卡片初始數量為零時，刪除訂單時不回補預算數量
        if (cbMainModel.Card_Num!=0) cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num + intReductNum;
        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT + decReductAmt;
        dao.Update<CARD_BUDGET>(cbMainModel, "RID");

        foreach (DataRow drowBudgtLog in dtblBudgetLog.Rows)
        {
            decimal LogPrice = Convert.ToDecimal(drowBudgtLog["Remain_Card_Price"]);
            int LogNum = Convert.ToInt32(drowBudgtLog["Remain_Card_Num"]);

            decimal PriceNow = 0.0M;
            int NumNow = 0;

            if (intReductNum != 0)
            {
                if (LogNum != 0)
                {
                    if (LogNum >= intReductNum)
                    {
                        NumNow = intReductNum;
                        intReductNum = 0;
                    }
                    else
                    {
                        NumNow = LogNum;
                        intReductNum = intReductNum - LogNum;
                    }
                }
            }

            if (decReductAmt != 0)
            {
                if (LogPrice != 0)
                {
                    if (LogPrice >= decReductAmt)
                    {
                        PriceNow = decReductAmt;
                        decReductAmt = 0;
                    }
                    else
                    {
                        PriceNow = LogPrice;
                        decReductAmt = decReductAmt - LogPrice;
                    }
                }
            }

            if (NumNow != 0 || PriceNow != 0)
            {
                CARD_BUDGET cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(drowBudgtLog["Budget_RID"]));
                //當主預算卡片初始數量為零時，刪除訂單時不回補預算數量
                if (cbModel.Card_Num!=0) cbModel.Remain_Card_Num = cbModel.Remain_Card_Num + NumNow;
                cbModel.Remain_Card_Price = cbModel.Remain_Card_Price + PriceNow;
                dao.Update<CARD_BUDGET>(cbModel, "RID");

                MergBUDGET_LOG(strofdRID, Convert.ToInt32(drowBudgtLog["Budget_RID"]), -PriceNow, -NumNow);
            }
        }
    }


    /// <summary>
    /// 預算修改
    /// </summary>
    /// <param name="strofdRID">訂單流水號</param>
    /// <param name="intBudgetRID">預算ID</param>
    /// <param name="intNowNum">增加的卡數</param>
    /// <param name="decNowAmt">增加的金額</param>
    public void AddBudget(string strofdRID, int intBudgetRID, int intNowNum, decimal decNowAmt)
    {
        dirValues.Clear();
        dirValues.Add("Budget_Main_RID", intBudgetRID);

        DataTable dtblBudget = dao.GetList(SEL_BUDGET + " order by rid", dirValues).Tables[0];

        CARD_BUDGET cbMainModel = dao.GetModel<CARD_BUDGET, int>("RID", intBudgetRID);
        CARD_BUDGET cbModel = new CARD_BUDGET();
        

        if (dtblBudget.Rows.Count > 0)
        {

            int intRemain_Total_Num = Convert.ToInt32(dtblBudget.Rows[0]["Total_Card_Num"]);

            if (intRemain_Total_Num == 0)
                intNowNum = 0;

            

            foreach (DataRow drowBudget in dtblBudget.Rows)
            {
                //預算金額及卡數
                decimal decRemain_Card_Price = Convert.ToDecimal(drowBudget["Remain_Card_Price"]);
                int intRemain_Card_Num = Convert.ToInt32(drowBudget["Remain_Card_Num"]);

                //單次錄入金額及卡數
                decimal decPrice = 0.00M;
                int intNum = 0;

                cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(drowBudget["RID"]));

                if (intNowNum != 0)
                {
                    if (intRemain_Card_Num > intNowNum)
                    {
                        intNum = intNowNum;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = cbModel.Remain_Card_Num-intNum;

                        cbModel.Remain_Card_Num = cbModel.Remain_Card_Num -intNum;

                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = 0;
                    }
                    else if (intRemain_Card_Num == intNowNum)
                    {
                        intNum = intNowNum;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = 0;

                        cbModel.Remain_Card_Num = 0;


                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = 0;
                    }
                    else
                    {
                        intNum = intRemain_Card_Num;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Num = 0;

                        cbModel.Remain_Card_Num = 0;

                        cbMainModel.Remain_Total_Num = cbMainModel.Remain_Total_Num - intNum;

                        intNowNum = intNowNum - intNum;
                    }
                }

                if (decNowAmt != 0.00M)
                {
                    if (decRemain_Card_Price > decNowAmt)
                    {
                        decPrice = decNowAmt;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = decRemain_Card_Price - decPrice;

                        cbModel.Remain_Card_Price = decRemain_Card_Price - decPrice;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;

                        decNowAmt = 0.00M;
                    }
                    else if (decRemain_Card_Price == decNowAmt)
                    {
                        decPrice = decNowAmt;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = 0.00M;

                        cbModel.Remain_Card_Price = 0.00M;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;

                        decNowAmt = 0.00M;
                    }
                    else
                    {
                        decPrice = decRemain_Card_Price;

                        if (Convert.ToInt32(drowBudget["RID"]) == intBudgetRID)
                            cbMainModel.Remain_Card_Price = 0.00M;

                        cbModel.Remain_Card_Price = 0.00m;

                        decNowAmt = decNowAmt - decPrice;

                        cbMainModel.Remain_Total_AMT = cbMainModel.Remain_Total_AMT - decPrice;
                    }
                }

                if (intNum != 0 || decPrice != 0.00M)
                {
                    MergBUDGET_LOG(strofdRID, Convert.ToInt32(drowBudget["RID"]), decPrice, intNum);
                    dao.Update<CARD_BUDGET>(cbModel, "RID");
                }
            }

            dao.Update<CARD_BUDGET>(cbMainModel, "RID");

            ORDER_FORM_DETAIL ofdModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", strofdRID);
            if (ofdModel != null)
            {
                if (ofdModel.Is_Edit_Budget != "Y")
                {
                    ofdModel.Is_Edit_Budget = "Y";
                    dao.Update<ORDER_FORM_DETAIL>(ofdModel, "RID");
                }
            }
        }
    }

    /// <summary>
    /// 增加入庫時檢查預算
    /// </summary>
    /// <param name="strofdRID">訂單流水ID</param>
    /// <param name="intNowNum">錄入卡數</param>
    private void Add_CheckBudget(string strofdRID, int intNowNum)
    {
        dirValues.Clear();
        dirValues.Add("OrderForm_Detail_RID", strofdRID);

        DataSet dstDepository = dao.GetList(SEL_ORDER_FORM_DETAIL,dirValues);
        DataSet dst1 = dao.GetList(SEL_DEPOSITORY_STOCK_2,dirValues);
        DataSet dst2 = dao.GetList(SEL_DEPOSITORY_CANCEL,dirValues);
        DataSet dst3 = dao.GetList(SEL_DEPOSITORY_RESTOCK, dirValues);

        //預算總卡數
        int intTotal_Num = Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Total_Card_Num"]);
        //合約總卡數
        int intAMTotal_Num = Convert.ToInt32(dstDepository.Tables[0].Rows[0]["AMCard_Number"]);
        //合約剩餘卡數
        int intAMTNum = int.Parse(dstDepository.Tables[0].Rows[0]["AMRemain_Card_Num"].ToString());

        //預算總剩餘金額及卡數
        int intBudgetNum = int.Parse(dstDepository.Tables[0].Rows[0]["Remain_Total_Num"].ToString());
        decimal decBudgetAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Remain_Total_AMT"].ToString());

        //用過的金額及卡數
        int intUsedNum = int.Parse(dst1.Tables[0].Rows[0][0].ToString()) - int.Parse(dst2.Tables[0].Rows[0][0].ToString()) + int.Parse(dst3.Tables[0].Rows[0][0].ToString());
        decimal decUsedAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intUsedNum;

        //這次錄入的金額及卡數
        decimal decNowAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intNowNum;


        #region 計算預算
        if (dstDepository.Tables[0].Rows[0]["Is_Edit_Budget"].ToString() == "Y")
        {
            //預算卡數是否可用
            if (intTotal_Num != 0)
            {
                if (intBudgetNum < intNowNum)
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
            }

            //合約卡數是否可用
            if (intAMTotal_Num != 0)
            {
                if (intAMTNum < intNowNum)
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_14, strofdRID));
            }

            //預算金額是否可用
            if (decBudgetAmt < decNowAmt)
                throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
        }
        else
        {
            if ((intUsedNum + intNowNum) > int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()))
            {
                if (intUsedNum < int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()))
                    intNowNum = intUsedNum + intNowNum - int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString());

                decNowAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intNowNum;

                if (intTotal_Num != 0)
                {
                    if (intBudgetNum < intNowNum)
                        throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
                }

                if (intAMTotal_Num != 0)
                {
                    if (intAMTNum < intNowNum)
                        throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_14, strofdRID));
                }

                if (decBudgetAmt < decNowAmt)
                    throw new AlertException(String.Format(BizMessage.BizMsg.ALT_DEPOSITORY_003_13, strofdRID));
            }
            else
                intNowNum = 0;
        }

        if (intNowNum != 0)
        {
            AGREEMENT amModel = dao.GetModel<AGREEMENT, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Agreement_RID"]));

            //減少合約卡數
            if (amModel.Card_Number != 0)
            {
                amModel.Remain_Card_Num = amModel.Remain_Card_Num - intNowNum;
                dao.Update<AGREEMENT>(amModel, "RID");
            }

            AddBudget(strofdRID, Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Budget_RID"]), intNowNum, decNowAmt);


            #region 警訊
            CARD_BUDGET cbModel = dao.GetModel<CARD_BUDGET, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Budget_RID"]));
            DataTable dtblBudget = dao.GetList("select max(valid_date_to) from CARD_BUDGET where Budget_Main_RID=" + dstDepository.Tables[0].Rows[0]["Budget_RID"]).Tables[0];
            DateTime dtMaxBudget = Convert.ToDateTime(dtblBudget.Rows[0][0]);

            AGREEMENT aModel = dao.GetModel<AGREEMENT, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Agreement_RID"]));

            Warning.SetWarning(GlobalString.WarningType.DepositoryBudget, new object[6] { cbModel.Budget_ID, cbModel.Total_Card_Num, cbModel.Remain_Total_Num, cbModel.Total_Card_AMT, cbModel.Remain_Total_AMT, dtMaxBudget });
            Warning.SetWarning(GlobalString.WarningType.DepositoryAgreement, new object[4] { aModel.Agreement_Code, aModel.Card_Number, aModel.Remain_Card_Num, aModel.End_Time });
            #endregion
        }

        #endregion
    }


    /// <summary>
    /// 更新減少的卡數金額
    /// </summary>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intOldNum">舊入庫數量</param>
    /// <param name="intNowNum">新入庫數量</param>
    /// <param name="intRID">入庫ID</param>
    private void UpdateBudget(string strofdRID, int intOldNum, int intNowNum, int intRID)
    {
        dirValues.Clear();
        dirValues.Add("OrderForm_Detail_RID", strofdRID);

        DataSet dstDepository = dao.GetList(SEL_ORDER_FORM_DETAIL, dirValues);

        if (dstDepository.Tables[0].Rows.Count == 0)
            return;

        //減少的數量
        int intReduceNum = 0;
        decimal decReduceAmt = 0.00M;

        intReduceNum = intOldNum - intNowNum;

        //預算
        if (dstDepository.Tables[0].Rows[0]["Is_Edit_Budget"].ToString() == "Y")
        {
            decReduceAmt = decimal.Parse(dstDepository.Tables[0].Rows[0]["Unit_Price"].ToString()) * intReduceNum;
            //預算
            ReduceBudget(strofdRID, Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Budget_RID"]), intReduceNum, decReduceAmt);

            //合約修改
            AGREEMENT amModel = dao.GetModel<AGREEMENT, int>("RID", Convert.ToInt32(dstDepository.Tables[0].Rows[0]["Agreement_RID"]));

            //增加合約卡數
            if (amModel.Card_Number != 0)
            {
                amModel.Remain_Card_Num = amModel.Remain_Card_Num + intReduceNum;
                dao.Update<AGREEMENT>(amModel, "RID");
            }
        }
    }


    /// <summary>
    /// 修改入庫預算
    /// </summary>
    /// <param name="intOldNum">舊入庫數量</param>
    /// <param name="intNum">新入庫數量</param>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intRID">入庫ID</param>
    private void Update_CheckBudget(int intOldNum, int intNum, string strofdRID,int intRID)
    {
        //減少
        if (intOldNum > intNum)         
        {
            UpdateBudget(strofdRID, intOldNum, intNum, intRID);
        }
        //增加
        else if (intOldNum < intNum)
        {
            Add_CheckBudget(strofdRID, intNum - intOldNum);
        }
        else
        {
            return;
        }
    }


    /// <summary>
    /// 驗證匯入字段是否滿足格式
    /// </summary>
    /// <param name="strColumn"></param>
    /// <param name="num"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private string CheckFileColumn(string strColumn, int num, int count)
    {
        string strErr = "";
        string Pattern = "";
        MatchCollection Matches;
        switch (num)
        {
            case 1:
                if (strColumn.Length != 12)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤;\\n";
                }
                break;
            case 3:
            case 4:
            case 5:
                Pattern = @"^\d{1,9}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為9位以內數字;\\n";
                }
                break;
            case 7:
                if (StringUtil.GetByteLength(strColumn) > 30)
                {
                    strErr += "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤;\\n";
                }
                break;
            case 2:
                if (StringUtil.GetByteLength(strColumn) > 30)
                {
                    strErr += "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤;\\n";
                }
                break;
            case 8:
            case 9:
                Pattern = @"^\d+$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為數字;\\n";
                }
                break;
            case 6:
                try
                {
                    DateTime dt = DateTime.ParseExact(strColumn, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
                catch
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列時間格式不對;\\n";
                }
                break;
            default:
                break;
        }

        return strErr;
    }

    /// <summary>
    /// 獲取廠商名稱
    /// </summary>
    /// <returns></returns>
    public DataSet GetFactoryData()
    {
        DataSet dstFactoryData = null;
        try
        {
            dstFactoryData = dao.GetList(SEL_FACTORY_BLANK + " " + SEL_FACTORY_PERSO, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstFactoryData;
    }

    /// <summary>
    /// 查詢預算主記錄列表
    /// </summary>
    /// <param name="searchInput">查詢條件</param>
    /// <param name="firstRowNumber">開始行數</param>
    /// <param name="lastRowNumber">結束行數</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortType">排序規則</param>
    /// <param name="rowCount">總記錄數</param>
    /// <returns>DataSet[預算]</returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Stock_RID" : sortField);//默認的排序欄位

        if (sortField == "null")
        {
            sortType = "DESC";
        }

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_DEPOSITORY_STOCK);

        //整理查詢條件
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)  //如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtStock_RIDYear"].ToString()))
            {
                string strStock_RID = searchInput["txtStock_RIDYear"].ToString();

                if (!StringUtil.IsEmpty(searchInput["txtStock_RID1"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID1"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtStock_RID2"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID2"].ToString();
                }
                if (!StringUtil.IsEmpty(searchInput["txtStock_RID3"].ToString()))
                {
                    strStock_RID += searchInput["txtStock_RID3"].ToString();
                }

                stbWhere.Append(" and DS.Stock_RID like @Stock_RID");
                dirValues.Add("Stock_RID", strStock_RID + "%");
            }

            if (((DataTable)searchInput["UctrlCardType"]).Rows.Count>0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["UctrlCardType"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";

                stbWhere.Append(" AND Space_Short_RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ")");
            }

            if (!StringUtil.IsEmpty(searchInput["dropBlank_Factory_RID"].ToString()))
            {
                stbWhere.Append(" and DS.Blank_Factory_RID=@Blank_Factory_RID");
                dirValues.Add("Blank_Factory_RID", searchInput["dropBlank_Factory_RID"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["dropPerso_Factory_RID"].ToString()))
            {
                stbWhere.Append(" and DS.Perso_Factory_RID=@Perso_Factory_RID");
                dirValues.Add("Perso_Factory_RID", searchInput["dropPerso_Factory_RID"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtOrder_DateFrom"].ToString()))
            {
                stbWhere.Append(" and DS.Order_Date>=@Order_DateFrom");
                dirValues.Add("Order_DateFrom", Convert.ToDateTime(searchInput["txtOrder_DateFrom"].ToString()));
            }

            if (!StringUtil.IsEmpty(searchInput["txtOrder_DateTo"].ToString()))
            {
                stbWhere.Append(" and DS.Order_Date<=@Order_DateTo");
                dirValues.Add("Order_DateTo", Convert.ToDateTime(searchInput["txtOrder_DateTo"].ToString()));
            }

            if (!StringUtil.IsEmpty(searchInput["txtIncome_DateFrom"].ToString()))
            {
                stbWhere.Append(" and DS.Income_Date>=@Income_DateFrom");
                dirValues.Add("Income_DateFrom", Convert.ToDateTime(searchInput["txtIncome_DateFrom"].ToString()));
            }

            if (!StringUtil.IsEmpty(searchInput["txtIncome_DateTo"].ToString()))
            {
                stbWhere.Append(" and DS.Income_Date<=@Income_DateTo");
                dirValues.Add("Income_DateTo", Convert.ToDateTime(searchInput["txtIncome_DateTo"].ToString()));
            }

            if (!StringUtil.IsEmpty(searchInput["dropStatus"].ToString()))
            {
                if (searchInput["radlOrderType"].ToString() == "1")
                {
                    stbWhere.Append(" and [OF].Case_Status=@Case_Status");
                    dirValues.Add("Case_Status", searchInput["dropStatus"].ToString());
                }
                else
                {
                    stbWhere.Append(" and OFD.Case_Status=@Case_Status");
                    dirValues.Add("Case_Status", searchInput["dropStatus"].ToString());
                }
            }
            
        }

        //執行SQL語句
        DataSet dstcard_Budget = null;
        try
        {
            dstcard_Budget = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        //返回查詢結果
        rowCount = intRowCount;
        return dstcard_Budget;
    }


    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="dtbl"></param>
    /// <returns></returns>
    public string Add(DataTable dtbl)
    {
        string strRID = "";

        try
        {
            dao.OpenConnection();
            DEPOSITORY_STOCK dsModel = new DEPOSITORY_STOCK();

            //生產入庫列印編號
            string strReport_RID = IDProvider.MainIDProvider.GetSystemNewID("Report_RID", "-A");

            foreach (DataRow drow in dtbl.Rows)
            {
                dsModel = dao.GetModelByDataRow<DEPOSITORY_STOCK>(drow);
                dsModel.Report_RID = strReport_RID;
                dsModel.Stock_RID = GetStock_RID(dsModel.OrderForm_Detail_RID);

                Add_CheckBudget(dsModel.OrderForm_Detail_RID, dsModel.Income_Number);

                strRID += dao.AddAndGetID<DEPOSITORY_STOCK>(dsModel, "RID").ToString() + ",";
            }

            strRID = strRID.Substring(0, strRID.Length - 1);

            //操作日誌
            SetOprLog("11");

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
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

        return strRID;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="dsModel"></param>
    public void Update(DEPOSITORY_STOCK dsModel)
    {
        DEPOSITORY_STOCK dsModel_O = new DEPOSITORY_STOCK();
        try
        {
            dao.OpenConnection();
            dsModel_O = dao.GetModel<DEPOSITORY_STOCK, string>("Stock_RID", dsModel.Stock_RID);

            if (dsModel_O.Stock_RID.ToString().Substring(8, 4) != "9999")
            {
                Update_CheckBudget(dsModel_O.Income_Number, dsModel.Income_Number, dsModel_O.OrderForm_Detail_RID, dsModel_O.RID);
            }

            dsModel_O.Stock_Number = dsModel.Stock_Number;
            dsModel_O.Blemish_Number = dsModel.Blemish_Number;
            dsModel_O.Sample_Number = dsModel.Sample_Number;
            dsModel_O.Income_Number = dsModel.Income_Number;
            dsModel_O.Serial_Number = dsModel.Serial_Number;
            dsModel_O.Check_Type = dsModel.Check_Type;
            dsModel_O.Comment = dsModel.Comment;

            //操作日誌
            SetOprLog();

            dao.Update<DEPOSITORY_STOCK>(dsModel_O, "Stock_RID,RID");

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
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
    /// 檢查群組是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelDEPOSITORY_STOCK(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("RID", strRID);

        DataSet dstBudget = dao.GetList(DEL_DEPOSITORY_STOCK, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "入庫單"));

    }

    /// <summary>
    /// Legend 2017/05/17 新增多筆刪除
    /// </summary>
    /// <param name="strStock_RIDs">多筆編號, 用分號隔開</param>
    public void MultiDelete(string strStock_RIDs)
    {
        DEPOSITORY_STOCK dsModel = new DEPOSITORY_STOCK();
        try
        {
            dao.OpenConnection();

            string[] AryStrStockRid = strStock_RIDs.Split(';');

            foreach (string strStock_RID in AryStrStockRid)
            {
                dsModel = dao.GetModel<DEPOSITORY_STOCK, string>("Stock_RID", strStock_RID);

                ChkDelDEPOSITORY_STOCK(dsModel.Stock_RID);

                //dsModel.RST = "D";

                UpdateBudget(dsModel.OrderForm_Detail_RID, dsModel.Income_Number, 0, dsModel.RID);

                //dao.Update<DEPOSITORY_STOCK>(dsModel, "Stock_RID,RID");
                dirValues.Clear();
                dirValues.Add("Stock_RID", strStock_RID);
                dao.Delete("DEPOSITORY_STOCK", dirValues);

                //操作日誌
                SetOprLog("4");  
            }

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
            throw ex;
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

    public void Delete(string strStock_RID)
    {
        DEPOSITORY_STOCK dsModel = new DEPOSITORY_STOCK();
        try
        {
            dao.OpenConnection();

            dsModel = dao.GetModel<DEPOSITORY_STOCK, string>("Stock_RID", strStock_RID);

            ChkDelDEPOSITORY_STOCK(dsModel.Stock_RID);

            //dsModel.RST = "D";

            UpdateBudget(dsModel.OrderForm_Detail_RID, dsModel.Income_Number, 0, dsModel.RID);

            //dao.Update<DEPOSITORY_STOCK>(dsModel, "Stock_RID,RID");
            dirValues.Clear();
            dirValues.Add("Stock_RID", strStock_RID);
            dao.Delete("DEPOSITORY_STOCK", dirValues);

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
    /// 結案
    /// </summary>
    /// <param name="strDetail_RID_Y"></param>
    /// <param name="strDetail_RID_N"></param>
    public void Close(string strDetail_RID_Y, string strDetail_RID_N)
    {
        string[] strDetailRIDList_Y = strDetail_RID_Y.Split(',');
        string[] strDetailRIDList_N = strDetail_RID_N.Split(',');

        ORDER_FORM_DETAIL ofdModel = new ORDER_FORM_DETAIL();
        ORDER_FORM ofModel = new ORDER_FORM();

        try
        {
            dao.OpenConnection();


            #region 結案

            foreach (string strDetailRID in strDetailRIDList_Y)
            {
                if (StringUtil.IsEmpty(strDetailRID))
                    continue;
                ofdModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", strDetailRID);
                if (ofdModel != null)
                {
                    ofdModel.Case_Status = "Y";
                }

                dirValues.Clear();
                dirValues.Add("OrderForm_RID", ofdModel.OrderForm_RID);
                dirValues.Add("orderForm_detail_rid", ofdModel.OrderForm_Detail_RID);

                DataSet dst = dao.GetList(SEL_ORDER_FORM_END + " and orderForm_detail_rid!=@orderForm_detail_rid", dirValues);
                if (dst != null)
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "0")
                    {
                        ofModel = dao.GetModel<ORDER_FORM, string>("OrderForm_RID", ofdModel.OrderForm_RID);
                        if (ofModel != null)
                        {
                            ofModel.Case_Status = "Y";
                            dao.Update<ORDER_FORM>(ofModel, "RID");
                        }
                    }
                }

                //回補預算，合約
                if (ofdModel.Is_Edit_Budget == "N")
                {
                    dirValues.Clear();
                    dirValues.Add("OrderForm_Detail_RID", ofdModel.OrderForm_Detail_RID);
                    DataSet dstDS = dao.GetList(SEL_DEPOSITORY_STOCK_2, dirValues);

                    //多餘卡數
                    int intReductNum = ofdModel.Number - Convert.ToInt32(dstDS.Tables[0].Rows[0][0]);
                    decimal decReductAmt = intReductNum * ofdModel.Unit_Price;

                    #region 回補合約
                    dirValues.Clear();
                    dirValues.Add("RID", ofdModel.Agreement_RID);
                    DataTable dtblAgreement = dao.GetList(SEL_AGREEMENT_BY_RID + " and RID=@RID", dirValues).Tables[0];

                    if (dtblAgreement.Rows.Count > 0)
                    {
                        AGREEMENT agModel = dao.GetModel<AGREEMENT, int>("RID", int.Parse(dtblAgreement.Rows[0]["RID"].ToString()));
                        if (agModel.Card_Number != 0)
                        {
                            agModel.Remain_Card_Num = agModel.Remain_Card_Num + intReductNum;
                            dao.Update<AGREEMENT>(agModel, "RID");
                        }
                    }
                    #endregion

                    #region 回補預算
                    ReduceBudget(ofdModel.OrderForm_Detail_RID, ofdModel.Budget_RID, intReductNum, decReductAmt);
                    #endregion

                    ofdModel.Is_Edit_Budget = "Y";
                }
                dao.Update<ORDER_FORM_DETAIL>(ofdModel, "RID");
            }
            #endregion

            #region 未結案
            foreach (string strDetailRID in strDetailRIDList_N)
            {
                if (StringUtil.IsEmpty(strDetailRID))
                    continue;

                ofdModel = dao.GetModel<ORDER_FORM_DETAIL, string>("OrderForm_Detail_RID", strDetailRID);
                if (ofdModel != null)
                {
                    //200912IR 取消入庫單結案狀態時扣除合約和預算 add by YangKun 2009/12/8 start 
                    if (ofdModel.Case_Status == "Y" && ofdModel.Is_Edit_Budget == "Y")
                    {
                        dirValues.Clear();
                        dirValues.Add("OrderForm_Detail_RID", ofdModel.OrderForm_Detail_RID);
                        DataSet dstDS = dao.GetList(SEL_DEPOSITORY_STOCK_2, dirValues);

                        //多餘卡數
                        int intReductNum = ofdModel.Number - Convert.ToInt32(dstDS.Tables[0].Rows[0][0]);
                        decimal decReductAmt = intReductNum * ofdModel.Unit_Price;
                        #region 扣除合約
                        dirValues.Clear();
                        dirValues.Add("RID", ofdModel.Agreement_RID);
                        DataTable dtblAgreement = dao.GetList(SEL_AGREEMENT_BY_RID + " and RID=@RID", dirValues).Tables[0];

                        if (dtblAgreement.Rows.Count > 0)
                        {
                            AGREEMENT agModel = dao.GetModel<AGREEMENT, int>("RID", int.Parse(dtblAgreement.Rows[0]["RID"].ToString()));
                            if (agModel.Card_Number != 0)
                            {
                                agModel.Remain_Card_Num = agModel.Remain_Card_Num - intReductNum;
                                dao.Update<AGREEMENT>(agModel, "RID");
                            }
                        }
                        #endregion

                        #region 扣除預算
                        ReduceBudget(ofdModel.OrderForm_Detail_RID, ofdModel.Budget_RID, -intReductNum, -decReductAmt);
                        #endregion
                        ofdModel.Is_Edit_Budget = "N";
                    }
                    //200912IR 取消入庫單結案狀態時扣除合約和預算 add by YangKun 2009/12/8 end
                    ofdModel.Case_Status = "N";
                }

                dao.Update<ORDER_FORM_DETAIL>(ofdModel, "RID");

                ofModel = dao.GetModel<ORDER_FORM, string>("OrderForm_RID", ofdModel.OrderForm_RID);
                if (ofModel != null)
                {
                    ofModel.Case_Status = "N";
                    dao.Update<ORDER_FORM>(ofModel, "RID");
                }
            }
            #endregion

            dao.Commit();
        }
        catch (Exception ex)
        {
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        finally
        {
            dao.CloseConnection();
        }
    }


    /// <summary>
    /// 獲取訂單流水ID
    /// </summary>
    /// <param name="strOrderForm_Detail_RID"></param>
    /// <returns></returns>
    public string GetStock_RID(string strOrderForm_Detail_RID)
    {
        string strStock_RID = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("OrderForm_Detail_RID", strOrderForm_Detail_RID);
            DataSet dsl = dao.GetList(SEL_STOCKMAXRID, dirValues);

            long Detail_RID = 0;

            if (dsl.Tables[0].Rows[0][0].ToString().Trim()!="0")
            {
                Detail_RID = Convert.ToInt64(dsl.Tables[0].Rows[0][0].ToString().Trim()) + 1;

                if (Detail_RID.ToString().Substring(12, 2) == "00")
                    throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_09);

                strStock_RID = Detail_RID.ToString();
            }
            else
            {
                strStock_RID = strOrderForm_Detail_RID + "01";
            }
        }
        catch (AlertException ex)
        {
            throw new AlertException(ex.Message);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return strStock_RID;
    }

    /// <summary>
    /// 人工新增
    /// </summary>
    /// <param name="dtbl"></param>
    public string AddMan(DataTable dtbl)
    {
        string strRID = "";

        try
        {
            dao.OpenConnection();
            DEPOSITORY_STOCK dsModel = new DEPOSITORY_STOCK();

            foreach (DataRow drow in dtbl.Rows)
            {
                if (IsCheck1(Convert.ToDateTime(drow["Income_Date"].ToString()).ToString("yyyy-MM-dd")))
                    throw new AlertException("入庫日期" + drow["Income_Date"].ToString() + "已經日結;");
            }

            //生產入庫列印編號
            string strReport_RID = IDProvider.MainIDProvider.GetSystemNewID("Report_RID", "-A");
            

            foreach (DataRow drow in dtbl.Rows)
            {
                dsModel = dao.GetModelByDataRow<DEPOSITORY_STOCK>(drow);
                dsModel.Report_RID = strReport_RID;
                dsModel.Stock_RID = IDProvider.MainIDProvider.GetSystemNewID("Stock_RID", "9999");
                dsModel.OrderForm_RID = dsModel.Stock_RID.Substring(0, 10);
                dsModel.OrderForm_Detail_RID = dsModel.Stock_RID.Substring(0, 12);

                strRID += dao.AddAndGetID<DEPOSITORY_STOCK>(dsModel, "RID").ToString() + ",";
            }

            strRID = strRID.Substring(0, strRID.Length - 1);

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
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            dao.CloseConnection();
        }

        return strRID;
    }
    /// <summary>
    /// 根據RID獲取STOCK_RID
    /// </summary>
    /// <returns></returns>
    public string GetSTOCKRID(string RID)
    {
        string strSTOCK_RID = "";
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", RID);
            strSTOCK_RID = dao.ExecuteScalar(SEL_STOCK_RID, dirValues).ToString();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return strSTOCK_RID;
    }

}
