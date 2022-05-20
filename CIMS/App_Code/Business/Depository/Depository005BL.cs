using System;
//******************************************************************
//*  作    者：JunWang
//*  功能說明：卡片在入庫邏輯
//*  創建日期：2008-09-22
//*  修改日期：2008-09-22 12:00
//*  修改記錄：
//*            □2008-09-22
//*              1.創建 王俊
//*              2.修改 潘秉奕 行207
//*******************************************************************
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
/// <summary>
/// Depository005BL 的摘要描述
/// </summary>
public class Depository005BL : BaseLogic
{
    #region SQL語句
    public const string SEL_DEPOSITORY_RESTOCK_2 = " SELECT DR.RID, DR.Report_RID, DR.Stock_RID, DR.Space_Short_RID, CT.NAME, DR.Reincome_Date,DR.Restock_Number, DR.Blemish_Number, DR.Sample_Number, DR.Reincome_Number, DR.Serial_Number, DR.Perso_Factory_RID, F2.Factory_ShortName_CN AS Blank_Factory_NAME, DR.Blank_Factory_RID, F1.Factory_ShortName_CN AS Perso_Factory_NAME, DR.Wafer_RID, WI.WAFER_NAME, DR.Check_Type, DR.Comment, CH.SendCheck_Status, DR.Is_Finance, DR.Is_Check FROM DEPOSITORY_RESTOCK AS DR LEFT JOIN CARD_TYPE AS CT ON CT.RST = 'A' AND CT.RID = DR.Space_Short_RID LEFT JOIN FACTORY F1 ON F1.RST = 'A' AND F1.Is_Perso ='Y' AND F1.RID =DR.Perso_Factory_RID LEFT JOIN FACTORY F2 ON F2.RST = 'A' AND F2.Is_Blank ='Y' AND F2.RID =DR.Blank_Factory_RID LEFT JOIN WAFER_INFO AS WI ON WI.RST = 'A' AND WI.RID = DR.Wafer_RID LEFT JOIN CARD_HALLMARK AS CH ON CH.RST = 'A' AND CH.Serial_Number = DR.Serial_Number AND CH.CardType_RID = DR.Space_Short_RID WHERE DR.RST = 'A' ";

    public const string SEL_FACTORY_PERSO = " select RID,Factory_ShortName_cn from FACTORY WHERE RST = 'A' AND IS_Perso ='Y'";

    public const string SEL_DEPOSITORY_CANCLE_CNT = " SELECT COUNT(*) FROM DEPOSITORY_CANCEL AS DC WHERE DC.RST = 'A' AND DC.Stock_RID = @stock_rid";
    public const string SEL_DEPOSITORY_CANCLE_SUM = " SELECT SUM(DC.Cancel_Number) as Cancel_Number FROM DEPOSITORY_CANCEL AS DC WHERE DC.RST = 'A' AND DC.Stock_RID = @stock_rid";
    public const string SEL_DEPOSITORY_STOCK = " SELECT DS.Space_Short_RID, CT.Name Space_Short_Name, DS.Perso_Factory_RID,F1.factory_id Perso_Factory_Name,F1.factory_shortname_cn PersoFacName, DS.Blank_Factory_RID,F2.factory_id Blank_Factory_Name,F2.factory_shortname_cn BlankFacName, WI.RID, WI.Wafer_Name,ds.is_check,ds.Check_Type,ds.Comment FROM DEPOSITORY_STOCK AS DS LEFT JOIN CARD_TYPE AS CT ON CT.RST = 'A' AND CT.RID = DS.Space_Short_RID LEFT JOIN FACTORY F1 ON F1.RST = 'A' AND F1.Is_Perso ='Y' AND F1.RID =DS.Perso_Factory_RID LEFT JOIN FACTORY F2 ON F2.RST = 'A' AND F2.Is_Blank ='Y' AND F2.RID =DS.Blank_Factory_RID LEFT JOIN WAFER_INFO AS WI ON WI.RST = 'A' AND WI.RID = DS.Wafer_RID WHERE DS.RST = 'A' AND DS.Stock_RID = @stock_rid ";
    public const string SEL_CARD_HALLMARK = " SELECT CH.SendCheck_Status FROM CARD_HALLMARK AS CH left join card_type ct on ct.rst='A' and ct.rid=ch.CardType_RID WHERE CH.RST = 'A' AND CH.Serial_Number = @serial_number AND ct.Name = @Space_Short_Name";

    public const string SEL_ORDER_FORM_DETAIL = " SELECT AM.RID AS Agreement_F_RID,ISNULL(AM.Remain_Card_Num,0) AS AMRemain_Card_Num,isnull(AM.Card_Number,0) as AMCard_Number ,isnull(cb.Total_Card_Num,0) as Total_Card_Num,isnull(cb.Remain_Total_AMT,0) as Remain_Total_AMT,isnull(cb.Remain_Total_Num,0) as Remain_Total_Num,OFD.Is_Edit_Budget,OFD.Is_Edit_Agreement,OFD.Unit_Price,OFORM.Pass_Status,OFD.ORDERFORM_DETAIL_RID,OFD.Case_Status,OFD.OrderForm_RID,OFD.Budget_RID,OFORM.Order_Date,OFD.Agreement_RID,OFD.Number,OFD.CardType_RID,OFD.Wafer_RID,AM.Factory_RID,WI.WAFER_NAME,CT.NAME,OFD.Delivery_Address_RID,F_P.Factory_ShortName_CN as Perso_Factory_Name,F_B.Factory_ShortName_CN as Blank_Factory_Name FROM ORDER_FORM_DETAIL OFD LEFT JOIN AGREEMENT AM ON AM.RST='A' AND AM.RID=OFD.Agreement_RID LEFT JOIN WAFER_INFO WI ON WI.RST='A' AND WI.RID=OFD.Wafer_RID LEFT join CARD_TYPE CT ON CT.RST='A' AND CT.Is_Using='Y' AND  CT.RID=OFD.CARDTYPE_RID LEFT join FACTORY F_P ON F_P.RST = 'A' AND F_P.IS_Perso ='Y' AND F_P.RID=OFD.Delivery_Address_RID LEFT join FACTORY F_B ON F_B.RST = 'A' AND F_B.IS_BLANK ='Y' AND F_B.RID=AM.Factory_RID inner JOIN ORDER_FORM OFORM ON OFORM.RST='A' AND OFORM.OrderForm_RID=OFD.OrderForm_RID left join CARD_BUDGET CB ON OFD.budget_rid=cb.rid and CB.RST='A' where OFD.OrderForm_Detail_RID=@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_STOCK_2 = " SELECT ISNULL(SUM(DS.Income_Number),0) FROM DEPOSITORY_STOCK AS DS WHERE DS.RST = 'A' AND DS.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_CANCEL = " SELECT ISNULL(SUM(DC.cancel_Number),0) FROM DEPOSITORY_CANCEL AS DC WHERE DC.RST = 'A' AND DC.OrderForm_Detail_RID =@OrderForm_Detail_RID";
    public const string SEL_DEPOSITORY_RESTOCK = " SELECT ISNULL(SUM(DR.Reincome_Number),0) FROM DEPOSITORY_RESTOCK AS DR WHERE DR.RST = 'A' AND DR.OrderForm_Detail_RID =@OrderForm_Detail_RID";

    //public const string SEL_STOCKMAXRID = " select ISNULL(max(Stock_RID),0) from dbo.DEPOSITORY_STOCK where Stock_RID=@OrderForm_Detail_RID";
    public const string SEL_ORDER_BUDGET_LOG = "select * from ORDER_BUDGET_LOG WHERE RST='A' AND OrderForm_Detail_RID=@OrderForm_Detail_RID ";

    public const string SEL_BUDGET = "select Card_Price,Card_Num,RID,Total_Card_AMT,Total_Card_Num,Remain_Total_AMT,Remain_Total_Num,Remain_Card_Price,Remain_Card_Num from dbo.CARD_BUDGET where RST='A' AND Budget_Main_RID= @Budget_Main_RID";
    public const string SEL_DEPOSITORY_STOCK_1 = "select DS.Stock_RID,Reincome_Number,Restock_Number, Blemish_Number, Sample_Number,Reincome_Date,"
        + " DS.is_check,DS.is_finance,DS.Is_AskFinance,"
        + " F2.Factory_ShortName_CN AS Blank_Factory_NAME, F1.Factory_ShortName_CN AS Perso_Factory_NAME,"
        + " WI.Wafer_Name,ds.Check_Type,ds.Comment,ct.name Space_Short_Name,DS.Serial_Number"
        + " from DEPOSITORY_RESTOCK DS "
        + " LEFT JOIN FACTORY F1 ON F1.RST = 'A' AND F1.Is_Perso ='Y' AND F1.RID =DS.Perso_Factory_RID"
        + " LEFT JOIN FACTORY F2 ON F2.RST = 'A' AND F2.Is_Blank ='Y' AND F2.RID =DS.Blank_Factory_RID"
        + " LEFT JOIN CARD_TYPE AS CT ON CT.RST = 'A' AND CT.RID = DS.Space_Short_RID"
        + " left JOIN CARD_HALLMARK AS CH ON CH.RST = 'A' and CH.Serial_Number=DS.Serial_Number AND CH.CardType_RID=DS.Space_Short_RID"
        + " LEFT JOIN WAFER_INFO AS WI ON WI.RST = 'A' AND WI.RID = DS.Wafer_RID"
        + " WHERE DS.RST = 'A' AND DS.Stock_RID = @stock_rid and DS.Report_RID=@Report_RID";       
        
        
        //" SELECT Is_AskFinance,Stock_RID,Case_Status,Number,SendCheck_Status,F2.Factory_ShortName_CN AS Blank_Factory_NAME, F1.Factory_ShortName_CN AS Perso_Factory_NAME,DS.is_check,DS.is_finance,DS.Serial_Number,Restock_Number,Blemish_Number,Sample_Number,Reincome_Date,DS.Space_Short_RID, CT.Name Space_Short_Name, DS.Perso_Factory_RID, F1.Factory_ShortName_CN Perso_Factory_ShortName_CN, DS.Blank_Factory_RID,F2.Factory_ShortName_CN Blank_Factory_ShortName_CN, WI.RID, WI.Wafer_Name,ds.Check_Type,ds.Comment FROM DEPOSITORY_RESTOCK AS DS 
        //+"LEFT JOIN CARD_TYPE AS CT ON CT.RST = 'A' AND CT.RID = DS.Space_Short_RID "
        //+" LEFT JOIN FACTORY F1 ON F1.RST = 'A' AND F1.Is_Perso ='Y' AND F1.RID =DS.Perso_Factory_RID"
        //+" LEFT JOIN FACTORY F2 ON F2.RST = 'A' AND F2.Is_Blank ='Y' AND F2.RID =DS.Blank_Factory_RID"
        //+" left JOIN CARD_HALLMARK AS CH ON CH.RST = 'A' and CH.Serial_Number=DS.Serial_Number AND CH.CardType_RID=DS.Space_Short_RID"
        //+" left JOIN ORDER_FORM_DETAIL AS OFD ON OFD.RST = 'A' AND OFD.OrderForm_Detail_RID = ds.OrderForm_Detail_RID"
        //+" LEFT JOIN WAFER_INFO AS WI ON WI.RST = 'A' AND WI.RID = DS.Wafer_RID WHERE DS.RST = 'A' AND DS.Stock_RID = @stock_rid and DS.Report_RID=@Report_RID";

   // public const string SEL_ORDER_FORM_DETAIL_2 = "SELECT OFD.OrderForm_Detail_RID FROM ORDER_FORM_DETAIL AS OFD INNER JOIN AGREEMENT AS AM ON AM.RST = 'A' AND AM.RID = OFD.Agreement_RID AND AM.Factory_RID = @Factory_RID WHERE OFD.RST = 'A' AND OFD.CardType_RID = @CardType_RID";

    public const string SEL_STOCK_DATE = "select * from CARDTYPE_STOCKS where rst='A' and stock_date>=@stock_date";

    public const string DEL_CHECK = "proc_CHK_DEL_DEPOSITORY_RESTOCK";

    public const string ADD_CHECKWAFER = "select Number from WAFER_CARDTYPE_USELOG where Operate_Type=2 and Operate_RID=@Operate_RID";

    public const string SEL_RID = "select Max(Restock_RID) from DEPOSITORY_RESTOCK where Stock_RID=@stock_rid";
#endregion

    //參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();
    public Depository005BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
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
            dstFactoryData = dao.GetList(SEL_FACTORY_PERSO, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstFactoryData;
    }

    /// <summary>
    /// 判斷再入庫日期是否日結
    /// </summary>
    /// <param name="strSTOCK_DATE"></param>
    /// <returns></returns>
    public bool Is_Stock(string strSTOCK_DATE)
    {
        dirValues.Clear();
        dirValues.Add("stock_date", DateTime.ParseExact(strSTOCK_DATE, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo));

        DataSet ds = dao.GetList(SEL_STOCK_DATE,dirValues);

        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 導入數據并處理
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="dtblFileImp"></param>
    /// <returns></returns>
    public DataTable Imp(string strPath, DataTable dtblFileImp)
    {
        #region 驗證文件
        StreamReader sr = null;

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
                if (StringUtil.GetByteLength(strReadLine)!=119)
                    throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_01);

                int nextBegin = 0;
                Depository003BL bl003 = new Depository003BL();
                strLine = new string[9];
                strLine[0] = bl003.GetSubstringByByte(strReadLine, nextBegin, 14, out nextBegin).Trim();
                strLine[1] = bl003.GetSubstringByByte(strReadLine, nextBegin, 30, out nextBegin).Trim();
                strLine[2] = bl003.GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();
                strLine[3] = bl003.GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();
                strLine[4] = bl003.GetSubstringByByte(strReadLine, nextBegin, 9, out nextBegin).Trim();
                strLine[5] = bl003.GetSubstringByByte(strReadLine, nextBegin, 8, out nextBegin).Trim();
                strLine[6] = bl003.GetSubstringByByte(strReadLine, nextBegin, 30, out nextBegin).Trim();
                strLine[7] = bl003.GetSubstringByByte(strReadLine, nextBegin, 5, out nextBegin).Trim();
                strLine[8] = bl003.GetSubstringByByte(strReadLine, nextBegin, 5, out nextBegin).Trim();

                //strLine = strReadLine.Split(GlobalString.FileSplit.Split);

                DataRow dr = dtblFileImp.NewRow();

                for (int i = 0; i < strLine.Length; i++)
                {
                    int num = i + 1;
                    if (StringUtil.IsEmpty(strLine[i]))
                        strErr += "第" + count.ToString() + "行第" + num.ToString() + "列為空。\\n";
                    else
                        strErr += CheckFileColumn(strLine[i], num, count);
                    //if (i == 5)
                    //    dr[i] = DateTime.ParseExact(strLine[i], "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    //else
                        
                    dr[i] = strLine[i];
                }

                if (!StringUtil.IsEmpty(strErr))
                    throw new AlertException(strErr);

                int intIncomeNum = Convert.ToInt32(dr[2]) - Convert.ToInt32(dr[3]) - Convert.ToInt32(dr[4]);
                //入庫量不能為負數
                if (intIncomeNum < 0)
                    strErr += "第" + count.ToString() + "行" + "再進貨量不能為負數。\\n";
                else if (intIncomeNum == 0)
                    strErr += "第" + count.ToString() + "行" + "再進貨量不能為0。\\n";

                //再入庫日期是否日結
                if (Is_Stock(dr[5].ToString())==true)
                {
                    strErr += "第" + count.ToString() + "行" + "再入庫日期必須是未日結。\\n";
                }

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


        #region 驗證數據格式并生成數據
        DataSet dstDepository = null;

        DataTable dtblReturn = new DataTable();
        dtblReturn.Columns.Add("Stock_RID");//入庫流水編號
        dtblReturn.Columns.Add("OrderForm_Detail_RID");
        dtblReturn.Columns.Add("Space_Short_Name");
        dtblReturn.Columns.Add("Space_Short_RID");
        dtblReturn.Columns.Add("TotalNum");
        dtblReturn.Columns.Add("RemainNum");
        dtblReturn.Columns.Add("Restock_Number");
        dtblReturn.Columns.Add("Blemish_Number");
        dtblReturn.Columns.Add("Sample_Number");
        dtblReturn.Columns.Add("Reincome_Number");
        dtblReturn.Columns.Add("Reincome_Date");
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
        dtblReturn.Columns.Add("Cancel_Number");


        try
        {
            int count1 = 1;
            string strError = "";
            foreach (DataRow drowFile in dtblFileImp.Rows)
            {
                ////"匯入重複的流水編號
                //if (dtblFileImp.Select("Stock_RID='" + drowFile[0].ToString() + "'").Length > 1)
                //    throw new AlertException(BizMessage.BizMsg.ALT_DEPOSITORY_003_07);

                dirValues.Clear();
                dirValues.Add("stock_rid", drowFile["Stock_RID"].ToString());
                dirValues.Add("OrderForm_Detail_RID", drowFile["Stock_RID"].ToString().Substring(0,12));
                dirValues.Add("Space_Short_Name", drowFile["Space_Short_Name"].ToString());
                dirValues.Add("serial_number", drowFile["Serial_Number"].ToString());

                //todo 再入庫日期未結日檢查
                dstDepository = dao.GetList(SEL_DEPOSITORY_CANCLE_CNT + SEL_DEPOSITORY_CANCLE_SUM + SEL_DEPOSITORY_STOCK + SEL_CARD_HALLMARK + SEL_ORDER_FORM_DETAIL + SEL_DEPOSITORY_STOCK_2 + SEL_DEPOSITORY_CANCEL + SEL_DEPOSITORY_RESTOCK, dirValues);                

                //1.退貨記錄不存在
                if (dstDepository.Tables[0].Rows[0][0].ToString() == "0")
                {
                    strError += "第" + count1.ToString() + "行" + "退貨記錄不存在;";
                    throw new AlertException(strError);
                }

                //2.入庫流水編號不存在
                if (dstDepository.Tables[2].Rows.Count == 0)
                {
                    strError += "第" + count1.ToString() + "行" + "入庫流水編號不存在;";
                    throw new AlertException(strError);
                }

                //3.不可使用該版面簡稱
                if (dstDepository.Tables[2].Rows[0]["Space_Short_RID"] == null || dstDepository.Tables[2].Rows[0]["Space_Short_Name"].ToString() != drowFile["Space_Short_Name"].ToString())
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_04);

                //4.不可使用該空白卡廠
                if (dstDepository.Tables[2].Rows[0]["Blank_Factory_RID"] == null || dstDepository.Tables[2].Rows[0]["Blank_Factory_Name"].ToString() != drowFile["Blank_Factory_Name"].ToString())
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_03);

                //5.不可使用該Perso廠
                if (dstDepository.Tables[2].Rows[0]["Perso_Factory_RID"] == null || dstDepository.Tables[2].Rows[0]["Perso_Factory_Name"].ToString() != drowFile["Perso_Factory_Name"].ToString())
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_08);


                //6.晶片不存在
                if (dstDepository.Tables[2].Rows[0]["RID"] == null)
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_05);

                //批號不存在
                if (dstDepository.Tables[3].Rows.Count == 0)
                    throw new AlertException("第" + count1.ToString() + "行" + BizMessage.BizMsg.ALT_DEPOSITORY_003_06);

                DataRow drowReturn = dtblReturn.NewRow();
                drowReturn["Stock_RID"] = drowFile["Stock_RID"].ToString();
                drowReturn["OrderForm_Detail_RID"] = drowFile["Stock_RID"].ToString().Substring(0,12);
                drowReturn["Space_Short_Name"] = drowFile["Space_Short_Name"].ToString();                
                drowReturn["TotalNum"] = int.Parse(dstDepository.Tables[1].Rows[0]["Cancel_Number"].ToString());
                //drowReturn["RemainNum"] = int.Parse(dstDepository.Tables[0].Rows[0]["number"].ToString()) - int.Parse(dstDepository.Tables[1].Rows[0][0].ToString()) + int.Parse(dstDepository.Tables[2].Rows[0][0].ToString()) - int.Parse(dstDepository.Tables[3].Rows[0][0].ToString());
                drowReturn["Restock_Number"] = int.Parse(drowFile["Restock_Number"].ToString());
                drowReturn["Blemish_Number"] = int.Parse(drowFile["Blemish_Number"].ToString());
                drowReturn["Sample_Number"] = int.Parse(drowFile["Sample_Number"].ToString());
                drowReturn["Reincome_Number"] = int.Parse(drowFile["Restock_Number"].ToString()) - int.Parse(drowFile["Blemish_Number"].ToString()) - int.Parse(drowFile["Sample_Number"].ToString());
                drowReturn["Reincome_Date"] = DateTime.ParseExact(drowFile["Reincome_Date"].ToString(), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                drowReturn["Serial_Number"] = drowFile["Serial_Number"].ToString();
                drowReturn["Blank_Factory_Name"] = dstDepository.Tables[2].Rows[0]["BlankFacName"].ToString();

                drowReturn["Space_Short_RID"] = dstDepository.Tables[2].Rows[0]["Space_Short_RID"].ToString();
                drowReturn["Blank_Factory_RID"] = dstDepository.Tables[2].Rows[0]["Blank_Factory_RID"].ToString();
                drowReturn["Perso_Factory_Name"] = dstDepository.Tables[2].Rows[0]["PersoFacName"].ToString();
                drowReturn["Perso_Factory_RID"] = dstDepository.Tables[2].Rows[0]["Perso_Factory_RID"].ToString();
                drowReturn["Wafer_Name"] = dstDepository.Tables[2].Rows[0]["Wafer_Name"].ToString();
                drowReturn["Wafer_RID"] = int.Parse(dstDepository.Tables[2].Rows[0]["RID"].ToString());
                drowReturn["SendCheck_Status"] = dstDepository.Tables[3].Rows[0]["SendCheck_Status"].ToString();
                drowReturn["Budget_RID"] = int.Parse(dstDepository.Tables[4].Rows[0]["Budget_RID"].ToString());
                //drowReturn["Order_Date"] = Convert.ToDateTime(dstDepository.Tables[2].Rows[0]["Order_Date"].ToString());
                drowReturn["Agreement_RID"] = int.Parse(dstDepository.Tables[4].Rows[0]["Agreement_RID"].ToString());
                //drowReturn["OrderForm_RID"] = int.Parse(dstDepository.Tables[0].Rows[0]["OrderForm_RID"].ToString());
                drowReturn["Check_Type"] = int.Parse(dstDepository.Tables[2].Rows[0]["Check_Type"].ToString());
                drowReturn["Comment"] = dstDepository.Tables[2].Rows[0]["Comment"].ToString();
                drowReturn["Cancel_Number"] = int.Parse(dstDepository.Tables[1].Rows[0][0].ToString());

                dtblReturn.Rows.Add(drowReturn);

                count1++;

            }

            if (!StringUtil.IsEmpty(strError))
                throw new AlertException(strError);
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
        //return null;
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
                if (strColumn.Length != 14)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤;\\n";
                }
                break;
            case 3:
                Pattern = @"^\d{1,9}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為9位以內數字;\\n";
                }
                break;
            case 4:
                Pattern = @"^\d{1,9}$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為9位以內數字;\\n";
                }
                break;
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
            case 8:
                if (strColumn.Length != 5)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤;\\n";
                }
                Pattern = @"^\d+$";
                Matches = Regex.Matches(strColumn, Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (Matches.Count == 0)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式必須為數字;\\n";
                }
                break;
            case 9:
                if (strColumn.Length != 5)
                {
                    strErr = "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤。\\n";
                }
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
            case 2:
                if (StringUtil.GetByteLength(strColumn) > 30)
                {
                    strErr += "第" + count.ToString() + "行第" + num.ToString() + "列格式錯誤;\\n";
                }  
                break;
            default:
                break;
        }

        return strErr;
    }

    /// <summary>
    /// 根據Report_RID獲得DEPOSITORY_RESTOCK對應記錄
    /// </summary>
    /// <param name="strReport_RID"></param>
    /// <returns></returns>
    public DEPOSITORY_RESTOCK getDRModel(string strReport_RID)
    {
        DEPOSITORY_RESTOCK dsModel = dao.GetModel<DEPOSITORY_RESTOCK, string>("RID", strReport_RID);
        return dsModel;
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
            DEPOSITORY_RESTOCK drModel = new DEPOSITORY_RESTOCK();

            //生產入庫列印編號
            string strReport_RID = IDProvider.MainIDProvider.GetSystemNewID("Report_RID", "-A");

            foreach (DataRow drow in dtbl.Rows)
            {
                drModel = dao.GetModelByDataRow<DEPOSITORY_RESTOCK>(drow);
                drModel.Report_RID = strReport_RID;
                dirValues.Clear();
                dirValues.Add("stock_rid", drModel.Stock_RID);
                string reStockRid = Convert.ToString(dao.GetList(SEL_RID, dirValues).Tables[0].Rows[0][0]);
                int seq = 1;               
                if (!StringUtil.IsEmpty(reStockRid.Trim()) )
                {
                    seq = Convert.ToInt32(reStockRid.Substring(reStockRid.Length - 2, 2)) + 1;
                }
                drModel.Restock_RID = drModel.Stock_RID +"01"+ seq.ToString().PadLeft(2,'0');              
                Add_CheckBudget(drModel.Stock_RID.ToString().Substring(0,12), drModel.Reincome_Number);               
                
                //drModel.Restock_RID = GetRestock_RID(drModel.Stock_RID.ToString());
                strRID += dao.AddAndGetID<DEPOSITORY_RESTOCK>(drModel, "RID").ToString() + ",";
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
    /// 修改卡片庫存量
    /// </summary>
    /// <param name="PFRID">perso_factory_rid</param>
    /// <param name="CTRID">cardtype_rid</param>
    /// <param name="SN">再入庫數量(正數則為添加，負數則為回補)</param>
    private void Add_Stocks_Number(int PFRID,int CTRID,int SN)
    {
        dirValues.Clear();
        dirValues.Add("perso_factory_rid", PFRID);
        dirValues.Add("cardtype_rid", CTRID);

        dao.ExecuteNonQuery("update cardtype_stocks set stocks_number=stocks_number+'" + SN + "' where rst='a' and Check_Status='1' and perso_factory_rid=@perso_factory_rid and cardtype_rid=@cardtype_rid",dirValues);
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

        DataSet dstDepository = dao.GetList(SEL_ORDER_FORM_DETAIL, dirValues);
        DataSet dst1 = dao.GetList(SEL_DEPOSITORY_STOCK_2, dirValues);//入庫
        DataSet dst2 = dao.GetList(SEL_DEPOSITORY_CANCEL, dirValues);//退貨
        DataSet dst3 = dao.GetList(SEL_DEPOSITORY_RESTOCK, dirValues);//再入庫

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

            Warning.SetWarning(GlobalString.WarningType.RestockBudget, new object[6] { cbModel.Budget_ID, cbModel.Total_Card_Num, cbModel.Remain_Total_Num, cbModel.Total_Card_AMT, cbModel.Remain_Total_AMT, dtMaxBudget });
            Warning.SetWarning(GlobalString.WarningType.RestockAgreement, new object[4] { aModel.Agreement_Code, aModel.Card_Number, aModel.Remain_Card_Num, aModel.End_Time });
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// 預算修改
    /// </summary>
    /// <param name="strofdRID">訂單流水號</param>
    /// <param name="intBudgetRID">預算ID</param>
    /// <param name="intNowNum">增加的卡數</param>
    /// <param name="decNowAmt">增加的金額</param>
    private void AddBudget(string strofdRID, int intBudgetRID, int intNowNum, decimal decNowAmt)
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
                            cbMainModel.Remain_Card_Num = cbModel.Remain_Card_Num - intNum;

                        cbModel.Remain_Card_Num = cbModel.Remain_Card_Num - intNum;

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
    /// 更新預算日誌
    /// </summary>
    /// <param name="strofdRID">訂單流水號</param>
    /// <param name="intBudgetRID">預算ID</param>
    /// <param name="decBudgetAmt">預算金額</param>
    /// <param name="intNowNum">預算卡數</param>
    private void MergBUDGET_LOG(string strofdRID, int intBudgetRID, decimal decBudgetAmt, int intNowNum)
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
            orlModel.Remain_Card_Num = orlModel.Remain_Card_Num + intNowNum;
            orlModel.Remain_Card_Price = orlModel.Remain_Card_Price + decBudgetAmt;
            if (orlModel.Remain_Card_Num < 0 || orlModel.Remain_Card_Price < 0)
                throw new AlertException("日誌不能為負數,更新失敗");
            dao.Update<ORDER_BUDGET_LOG>(orlModel, "RID");
        }
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
                dirValues.Add("serial_number", strSNum);
                dirValues.Add("Space_Short_Name", strCardTypeRID);
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

        sortType = (sortField == "null" ? "desc " : sortType);//默認的排序欄位

        //准備SQL語句
        StringBuilder stbCommand = new StringBuilder(SEL_DEPOSITORY_RESTOCK_2);

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

                stbWhere.Append(" and DR.Stock_RID like @Stock_RID");
                dirValues.Add("Stock_RID", strStock_RID + "%");
            }

            if (((DataTable)searchInput["UctrlCardType"]).Rows.Count > 0)
            {
                string strCardType = "";
                foreach (DataRow drowCardType in ((DataTable)searchInput["UctrlCardType"]).Rows)
                    strCardType += drowCardType["RID"].ToString() + ",";

                stbWhere.Append(" AND Space_Short_RID IN (" + strCardType.Substring(0, strCardType.Length - 1) + ")");
            }


            if (!StringUtil.IsEmpty(searchInput["dropPerso_Factory_RID"].ToString()))
            {
                stbWhere.Append(" and DR.Perso_Factory_RID=@Perso_Factory_RID");
                dirValues.Add("Perso_Factory_RID", searchInput["dropPerso_Factory_RID"].ToString());
            }

            if (!StringUtil.IsEmpty(searchInput["txtIncome_DateFrom"].ToString()))
            {
                stbWhere.Append(" and DR.Reincome_Date>=@Income_DateFrom");
                dirValues.Add("Income_DateFrom", Convert.ToDateTime(searchInput["txtIncome_DateFrom"].ToString()));
            }

            if (!StringUtil.IsEmpty(searchInput["txtIncome_DateTo"].ToString()))
            {
                stbWhere.Append(" and DR.Reincome_Date<=@Income_DateTo");
                dirValues.Add("Income_DateTo", Convert.ToDateTime(searchInput["txtIncome_DateTo"].ToString()));
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
            dirValues.Add("OrderForm_Detail_RID", strStock_RID.Substring(0, 12));
            dst = dao.GetList(SEL_DEPOSITORY_STOCK_1 + " and ds.Stock_RID=@Stock_RID "
                //+ SEL_DEPOSITORY_STOCK_2 
                + SEL_DEPOSITORY_CANCEL 
                //+ SEL_DEPOSITORY_RESTOCK
                ,
                dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    public DataSet GetModData(string strStock_RID,string strReport_RID)
    {
        DataSet dst = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("Stock_RID", strStock_RID);
            dirValues.Add("Report_RID", strReport_RID);
            dirValues.Add("OrderForm_Detail_RID", strStock_RID.Substring(0, 12));
            dst = dao.GetList(SEL_DEPOSITORY_STOCK_1 
               // + SEL_DEPOSITORY_STOCK_2
                + SEL_DEPOSITORY_CANCEL
               // + SEL_DEPOSITORY_RESTOCK
                , dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dst;
    }

    public void Delete(string strReport_RID)
    {
        DEPOSITORY_RESTOCK dsModel = new DEPOSITORY_RESTOCK();
        try
        {
            dao.OpenConnection();


            dsModel = dao.GetModel<DEPOSITORY_RESTOCK, string>("RID", strReport_RID);

            dirValues.Clear();
            dirValues.Add("RID", dsModel.RID);
            DataSet dstBudget = dao.GetList(DEL_CHECK, dirValues, true);
            if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
                throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "再入庫單"));


            UpdateBudget(dsModel.OrderForm_Detail_RID, dsModel.Reincome_Number, 0, dsModel.RID);

            //Add_Stocks_Number(dsModel.Perso_Factory_RID, dsModel.Space_Short_RID, -dsModel.Reincome_Number);

            dirValues.Clear();
            dirValues.Add("RID", dsModel.RID);
            dao.Delete("DEPOSITORY_RESTOCK", dirValues);

            //操作日誌
            SetOprLog("4");

            dao.Commit();
        }
        catch (AlertException ex)
        {
            dao.Rollback();
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
    /// 減少的預算
    /// </summary>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intBudgetRID">主預算ID</param>
    /// <param name="intReductNum">減少數量</param>
    /// <param name="decReductAmt">減少金額</param>
    private void ReduceBudget(string strofdRID, int intBudgetRID, int intReductNum, decimal decReductAmt)
    {
        Depository003BL bl = new Depository003BL();
        bl.dao = this.dao;
        bl.ReduceBudget(strofdRID, intBudgetRID, intReductNum, decReductAmt);
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="dsModel"></param>
    public void Update(DEPOSITORY_RESTOCK dsModel)
    {
        DEPOSITORY_RESTOCK dsModel_O = new DEPOSITORY_RESTOCK();
        try
        {
            dao.OpenConnection();

            dsModel_O = dao.GetModel<DEPOSITORY_RESTOCK, string>("RID", dsModel.RID.ToString());

            //再入庫修改刪除需檢查入庫資料晶片耗用量
            if (dsModel_O != null)
            {
                dirValues.Clear();
                dirValues.Add("Operate_RID", dsModel_O.RID);
                DataTable dtblWaffer = dao.GetList(ADD_CHECKWAFER, dirValues).Tables[0];
                if (dtblWaffer.Rows.Count > 0)
                {
                    int intWafferNumber = int.Parse(dtblWaffer.Rows[0][0].ToString());
                    if (dsModel_O.Restock_Number < intWafferNumber)
                    {
                        throw new AlertException(string.Format(BizMessage.BizMsg.ALT_DEPOSITORY_005_01, new object[1] { intWafferNumber }));
                    }
                }
            }

            if (dsModel_O.Stock_RID.ToString().Substring(8, 4) != "9999")
            {
                Update_CheckBudget(dsModel_O.Reincome_Number, dsModel.Reincome_Number, dsModel_O.OrderForm_Detail_RID, dsModel_O.RID);
            }

            dsModel_O.Restock_Number = dsModel.Restock_Number;
            dsModel_O.Blemish_Number = dsModel.Blemish_Number;
            dsModel_O.Sample_Number = dsModel.Sample_Number;
            dsModel_O.Reincome_Number = dsModel.Reincome_Number;
            dsModel_O.Serial_Number = dsModel.Serial_Number;
            dsModel_O.Check_Type = dsModel.Check_Type;
            dsModel_O.Comment = dsModel.Comment;

            dao.Update<DEPOSITORY_RESTOCK>(dsModel_O, "RID");

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
    }

    /// <summary>
    /// 修改入庫預算
    /// </summary>
    /// <param name="intOldNum">舊入庫數量</param>
    /// <param name="intNum">新入庫數量</param>
    /// <param name="strofdRID">流水編號</param>
    /// <param name="intRID">入庫ID</param>
    private void Update_CheckBudget(int intOldNum, int intNum, string strofdRID, int intRID)
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
}
