//******************************************************************
//*  作    者：Cuiyan Ma
//*  功能說明：每日監控
//*  創建日期：2008-09-18
//*  修改日期：2008-09-18 12:00
//*  修改記錄：
//*            □2008-09-18
//*              1.創建 馬翠艷
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

public partial class Depository_Depository014 :PageBase
{
    Depository014BL bizlogic = new Depository014BL();
    Report032BL rep = new Report032BL();
    Dictionary<string, DataTable> dirManage = new Dictionary<string, DataTable>();
    DataTable temp = new DataTable();    
    /// <summary>
    /// 頁面初始化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        
        UctrlCardType.SetLeftItem = Depository014BL.SEL_CARD_TYPE;
        UctrlCardType.Is_Using = true;
        if (!IsPostBack)
        {
            Session.Remove("htFactory");

            dropPerso.DataSource = bizlogic.GetFactoryList();
            dropPerso.DataBind();
            dropPerso.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
            FillDropMonth();
            Type1.Checked = true;
            Session.Remove("dirManage");
            Session.Remove("PersoCard");
            btnReport.Visible = false;
            btnSubmit.Visible = false;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "CheckRio();", true);
            Dictionary<string, DataTable> dirResult = (Dictionary<string, DataTable>)Session["dirManage14"];  // Legend 2017/05/04 改變Key名稱
            if (dirResult != null)
            {
                if (txtDATE_FROM.Text == "" || txtDATE_TO.Text == "")
                    return;
                DateTime beginDay = Convert.ToDateTime(txtDATE_FROM.Text);
                DateTime endDay = Convert.ToDateTime(txtDATE_TO.Text);
                HtmlTable table = new HtmlTable();
                table.ID = "tb_DayManage";
                table.CellSpacing = 2;
                table.Width = "100%";
                table.Attributes.Add("class", "GridView");
                tdResult.Controls.Add(table);
                DataBound(dirResult, beginDay, endDay, table);
            }
        }       

    }

    /// <summary>
    /// 計算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        if (!rep.IsDAYLY_MONITOR())
        {
            ShowMessage("當日未執行每日監控批次");
            return;
        }
        DateTime begainSearch = Convert.ToDateTime(txtDATE_FROM.Text);
        DateTime endSearch = Convert.ToDateTime(txtDATE_TO.Text);
        if (begainSearch <Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd")))
        {
            ShowMessage("監控日期起不能小於當日！");
            return;
        }
        //2009.5.20应max Lu要求去掉时间限制
        //if (endSearch >= begainSearch.AddDays(60))
        //{
        //    ShowMessage("日期區間不可超過60天");
        //    return;
        //}
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (this.Type1.Checked)
        {
            UctrlCardType.SetRightItem = new DataTable();
        }
        if (this.Type2.Checked)
        {
            txtCardtype.Text = "";
            txtAffinity.Text = "";
            txtPhoto.Text = "";
            txtName.Text = "";
        }
        inputs.Add("dropPerso", dropPerso.SelectedValue);
        inputs.Add("txtCardType", txtCardtype.Text);
        inputs.Add("txtAffinity", txtAffinity.Text);
        inputs.Add("txtPhoto", txtPhoto.Text);
        inputs.Add("txtName", txtName.Text);
        inputs.Add("UctrlCardType", UctrlCardType.GetRightItem);
        try
        {
            //取資料集
            DataTable dtblPersoCardtype = bizlogic.PreCardType(inputs, begainSearch, endSearch);                    
            tdResult.Controls.Remove(tdResult.FindControl("tb_DayManage"));
            if (dtblPersoCardtype == null || dtblPersoCardtype.Rows.Count == 0)
            {
                sp001.Visible = true;
                btnReport.Visible = false;
                btnSubmit.Visible = false;
            }
            else
            {
                sp001.Visible = false;
                btnReport.Visible = true;
                btnSubmit.Visible = true;
                HtmlTable table = new HtmlTable();
                table.ID = "tb_DayManage";
                table.CellSpacing = 2;
                table.Width = "100%";
                table.Attributes.Add("class", "GridView");
                tdResult.Controls.Add(table);
                if (StringUtil.IsEmpty(dropDay.SelectedValue))
                {
                    foreach (ListItem item in dropDay.Items)
                    {
                        if (item != dropDay.SelectedItem)
                        {
                            int XValue = int.Parse(item.Text);
                            int XCode = int.Parse(item.Value);
                            drawTable(dtblPersoCardtype, XCode, XValue, begainSearch, endSearch, table);
                        }
                    }
                }
                else
                {
                    int XValue = int.Parse(dropDay.SelectedItem.Text);
                    int XCode = int.Parse(dropDay.SelectedValue);
                    drawTable(dtblPersoCardtype, XCode, XValue, begainSearch, endSearch, table);
                }
                Session["dirManage14"] = dirManage;    // Legend 2017/05/04 改變Key名稱
            }     
            UctrlCardType.LbLeftBind();          
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 根據查詢結果分廠分卡種畫出查詢結果
    /// </summary>
    /// <param name="dtblPersoCardtype"></param>
    /// <param name="Month"></param>
    /// <param name="Months"></param>
    private void drawTable(DataTable dtbPersoCardType, int xCode, int Month, DateTime begainDay, DateTime endDay, HtmlTable tb_DayManage)
    {
        DataRow[] PersoCards = dtbPersoCardType.Select("XType = '" + xCode + "'", "HNumber asc");
        DataSet dst = bizlogic.List(PersoCards, xCode, begainDay, endDay);
        if (dst != null & dst.Tables.Count > 0)
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("style", "color:Black;background-color:#B9BDAA;font-size:Small;font-weight:bolder;");
            HtmlTableCell CardCell = new HtmlTableCell();
            CardCell.ColSpan = 2;
            if (xCode.ToString().Equals(GlobalString.cardparam.Y1))
                CardCell.InnerText = "Y1=" + Month;
            else if (xCode.ToString().Equals(GlobalString.cardparam.Y2))
                CardCell.InnerText = "Y2=" + Month;
            else if (xCode.ToString().Equals(GlobalString.cardparam.Y3))
                CardCell.InnerText = "Y3=" + Month;
            else
            {
                ShowMessage("參數設定有誤");
                return;
            }             
            row.Cells.Add(CardCell);
            HtmlTableCell PersoCell = new HtmlTableCell();
            PersoCell.InnerText = "Perso廠";
            row.Cells.Add(PersoCell);

            HtmlTableCell blankCell = new HtmlTableCell();
            row.Cells.Add(blankCell);
            foreach (DataRow drow in dst.Tables[0].Rows)
            {
                HtmlTableCell monthCell = new HtmlTableCell();
                monthCell.InnerText = Convert.ToDateTime(drow["Day"]).ToString("yyyy/MM/dd");
                row.Cells.Add(monthCell);
            }
            tb_DayManage.Rows.Add(row);            
            for (int j = 0; j < dst.Tables.Count;j++)
            {

                string factoryRid = PersoCards[j]["Perso_Factory_RID"].ToString();
                string factoryName = bizlogic.getFactory(int.Parse(factoryRid)).Factory_ShortName_CN;
                string cardRid = PersoCards[j]["CardRID"].ToString();
                string CardName = PersoCards[j]["Name"].ToString();
                dirManage.Add(factoryRid + "_" + cardRid + "_" + Month + "_" + xCode, dst.Tables[j]);
                int k = 0;
                foreach (DataColumn col in dst.Tables[j].Columns)
                {
                    string name = col.ColumnName;
                    if (name == "C" || name == "H" || name == "J" || name == "L")
                    {
                        HtmlTableRow row1 = new HtmlTableRow();
                        row1.Attributes.Add("class", "GridViewRow");
                        if (k == 0)
                        {
                            HtmlTableCell checkCell1 = new HtmlTableCell();
                            CheckBox check = new CheckBox();
                            check.ID = "chk_" + factoryRid + "_" + cardRid + "_" + Month + "_" + xCode;
                            checkCell1.RowSpan = 4;
                            checkCell1.Controls.Add(check);
                            row1.Cells.Add(checkCell1);
                            HtmlTableCell CardCell1 = new HtmlTableCell();
                            LinkButton link = new LinkButton();
                            link.Text = CardName;
                            link.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository014Detail.aspx?table=" + factoryRid + "_" + cardRid + "_" + Month + "_" + xCode + "','','dialogHeight:450px;dialogWidth:800px;resizable=yes;');if(aa!=undefined){changeTable();}");
                            CardCell1.RowSpan = 4;
                            CardCell1.Controls.Add(link);
                            row1.Cells.Add(CardCell1);
                            HtmlTableCell PersoCell1 = new HtmlTableCell();
                            PersoCell1.RowSpan = 4;
                            PersoCell1.InnerText = factoryName;
                            row1.Cells.Add(PersoCell1);
                        }
                        HtmlTableCell blankCell1 = new HtmlTableCell();
                        blankCell1.InnerText = name;
                        row1.Cells.Add(blankCell1);
                        string strN = bizlogic.paramList(GlobalString.cardparam.YType);
                        decimal n = Convert.ToDecimal(strN.Substring(0, strN.Length - 1)); 
                        foreach (DataRow drow in dst.Tables[j].Rows)
                        {
                            HtmlTableCell monthCell = new HtmlTableCell();
                            monthCell.Align = "right";
                            monthCell.InnerText = changeData(drow[name]);
                            if (name == "H" && Convert.ToDecimal(monthCell.InnerText) < n)
                            {
                                monthCell.BgColor = "Red";//H=0,<N 返紅
                            }
                            row1.Cells.Add(monthCell);
                        }
                        tb_DayManage.Rows.Add(row1);
                        k++;
                    }
                }
            }
        }

    }
    private string changeData(Object obj)
    {
        try
        {
            if (obj.ToString().IndexOf('.') != -1)
            {
                return Convert.ToDecimal(obj).ToString("N2");
            }
            else
            {
                return Convert.ToInt32(obj).ToString("N0");
            }
        }
        catch
        {

            return "無法計算";
        }

    }
    protected void btnDrawTable_Click(object sender, EventArgs e)
    {

    }

    private void DataBound(Dictionary<string, DataTable> dirTable, DateTime begainDay,DateTime endDay, HtmlTable tb_DayManage)
    {
        string flag = "0";
        DataTable PersoCard = (DataTable)Session["PersoCard"];
        foreach (KeyValuePair<string, DataTable> key in dirTable)
        {
            DataTable datable = (DataTable)key.Value;
            string KeyValue = key.Key;
            int persoRid = int.Parse(KeyValue.Split('_')[0]);
            FACTORY factory = bizlogic.getFactory(persoRid);
            int cardRid = int.Parse(KeyValue.Split('_')[1]);
            CARD_TYPE cardModel = bizlogic.getCardType(cardRid);
            if (flag == "0" || KeyValue.Split('_')[3] != flag)
            {
                HtmlTableRow row = new HtmlTableRow();
                row.Attributes.Add("style", "color:Black;background-color:#B9BDAA;font-size:Small;font-weight:bolder;");
                HtmlTableCell CardCell = new HtmlTableCell();
                CardCell.ColSpan = 2;
                if (KeyValue.Split('_')[3].Equals(GlobalString.cardparam.Y1))
                    CardCell.InnerText = "Y1=" + KeyValue.Split('_')[2];
                else if (KeyValue.Split('_')[3].Equals(GlobalString.cardparam.Y2))
                    CardCell.InnerText = "Y2=" + KeyValue.Split('_')[2];
                else if (KeyValue.Split('_')[3].Equals(GlobalString.cardparam.Y3))
                    CardCell.InnerText = "Y3=" + KeyValue.Split('_')[2];           
                row.Cells.Add(CardCell);
                HtmlTableCell PersoCell = new HtmlTableCell();
                PersoCell.InnerText = "Perso廠";
                row.Cells.Add(PersoCell);
                HtmlTableCell blankCell = new HtmlTableCell();
                row.Cells.Add(blankCell);
                foreach (DataRow drow in datable.Rows)
                {
                    HtmlTableCell monthCell = new HtmlTableCell();
                    monthCell.InnerText = Convert.ToDateTime(drow["Day"]).ToString("yyyy/MM/dd");
                    row.Cells.Add(monthCell);
                }
                tb_DayManage.Rows.Add(row);
                flag = KeyValue.Split('_')[3];
            }
            int k = 0;
            foreach (DataColumn col in datable.Columns)
            {
                string name = col.ColumnName;
                if (name == "C" || name == "H" || name == "J" || name == "L")
                {
                    HtmlTableRow row1 = new HtmlTableRow();
                    row1.Attributes.Add("class", "GridViewRow");
                    if (k == 0)
                    {
                        HtmlTableCell checkCell1 = new HtmlTableCell();
                        CheckBox check = new CheckBox();
                        check.ID = "chk_" + KeyValue;
                        checkCell1.RowSpan = 4;
                        checkCell1.Controls.Add(check);
                        row1.Cells.Add(checkCell1);
                        HtmlTableCell CardCell1 = new HtmlTableCell();
                        LinkButton link = new LinkButton();
                        link.Text = cardModel.Name;
                        link.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository014Detail.aspx?table=" + KeyValue + "','','dialogHeight:450px;dialogWidth:800px;');");
                        CardCell1.RowSpan = 4;
                        CardCell1.Controls.Add(link);
                        row1.Cells.Add(CardCell1);
                        HtmlTableCell PersoCell1 = new HtmlTableCell();
                        PersoCell1.RowSpan = 4;
                        PersoCell1.InnerText = factory.Factory_ShortName_CN;
                        row1.Cells.Add(PersoCell1);
                    }
                    HtmlTableCell blankCell1 = new HtmlTableCell();
                    blankCell1.InnerText = name;
                    row1.Cells.Add(blankCell1);
                    string strN = bizlogic.paramList(GlobalString.cardparam.YType);
                    decimal n = Convert.ToDecimal(strN.Substring(0, strN.Length - 1));
                    foreach (DataRow drow in datable.Rows)
                    {
                        HtmlTableCell monthCell = new HtmlTableCell();
                        monthCell.Align = "right";
                        monthCell.InnerText = changeData(drow[name]);
                        if (name == "H" && Convert.ToDecimal(monthCell.InnerText) < n)
                        {
                            monthCell.BgColor = "Red";//H=0,<N 返紅
                        }
                        row1.Cells.Add(monthCell);
                    }
                    tb_DayManage.Rows.Add(row1);
                    k++;
                }
            }
        }

    }

    /// <summary>
    /// 匯出Excel格式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        Dictionary<string, DataTable> dirResult = (Dictionary<string, DataTable>)Session["dirManage14"];   // Legend 2017/05/04 改變Key名稱
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        bizlogic.inputData2Temp(dirResult,time);
        Response.Write("<script>window.open('Depository014Print.aspx?time=" + time + "');</script>");
    }

    protected void btnSubmit_click(object sender, EventArgs e)
    {
        Dictionary<string, int>  dirCardType = new Dictionary<string, int>();
        temp.Columns.Clear();
        temp.Columns.Add("Pass_Status");
        temp.Columns.Add("orderform_rid");
        temp.Columns.Add("orderform_detail_rid");
        temp.Columns.Add("Space_Short_RID");//cardtype_rid
        temp.Columns.Add("number");
        temp.Columns.Add("Agreement_RID");
        temp.Columns.Add("Agreement_Name");
        temp.Columns.Add("Budget_RID");
        temp.Columns.Add("Budget_Name");
        temp.Columns.Add("factory");
        temp.Columns.Add("factory_id");
        temp.Columns.Add("Base_Price");
        temp.Columns.Add("Fore_Delivery_Date");
        temp.Columns.Add("Wafer_RID");
        temp.Columns.Add("Wafer_Name");
        temp.Columns.Add("Wafer_Capacity");
        temp.Columns.Add("is_exigence");
        temp.Columns.Add("Delivery_Address_RID");
        temp.Columns.Add("factory_shortname_cn");
        temp.Columns.Add("comment");
        temp.Columns.Add("bz");
        //add Ian Huang start
        temp.Columns.Add("Change_UnitPrice");
        //add Ian Huang end
        Dictionary<string, DataTable> dirResult = (Dictionary<string, DataTable>)Session["dirManage14"];  // Legend 2017/05/04 改變Key名稱
        DataTable PersoCard = (DataTable)Session["PersoCard"];
        foreach (KeyValuePair<string, DataTable> key in dirResult)
        {
            CheckBox CardCheck = (CheckBox)tdResult.FindControl("chk_" + key.Key);
            DataTable table = (DataTable)key.Value;
            int persoRid = int.Parse(key.Key.Split('_')[0]);
            FACTORY factory = bizlogic.getFactory(persoRid);
            int cardRid = int.Parse(key.Key.Split('_')[1]);
            CARD_TYPE cardModel = bizlogic.getCardType(cardRid);
            if (CardCheck.Checked)
            {
                if (dirCardType.ContainsKey(key.Key.Split('_')[1]))
                {
                    ShowMessage("有重復卡種！");
                    return;
                }
                else
                {
                    dirCardType.Add(key.Key.Split('_')[1], 1);
                }
                DataRow drow = temp.NewRow();
                drow["Delivery_Address_RID"] = persoRid;
                drow["Space_Short_RID"] = cardRid;
                drow["factory_shortname_cn"] = factory.Factory_ShortName_CN;
                drow["bz"] = 2;
                long buyNumber = 0;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    buyNumber += int.Parse(table.Rows[i]["J"].ToString().Replace(",", ""));
                }
                if (buyNumber > 999999999)
                {
                    ShowMessage("採購數量超過採購下單的最大數量");
                    return;
                }
                drow["number"] = buyNumber;
                if (buyNumber != 0)
                    temp.Rows.Add(drow);
            }
        }
        if (temp.Rows.Count < 1)
        {
            ShowMessage("沒有選擇任何卡種下單！");
            return;
        }
        if (temp.Rows.Count > 99)
        {
            ShowMessage("超過最大下單量！");
            return;
        }
        Session["monitory"] = temp;
        Response.Redirect("Depository002Add.aspx?type=1");
    }

    private void FillDropMonth()
    {
        string strY1 = bizlogic.paramList(GlobalString.cardparam.Y1);
        string strY2 = bizlogic.paramList(GlobalString.cardparam.Y2);
        string strY3 = bizlogic.paramList(GlobalString.cardparam.Y3);
        dropDay.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        dropDay.Items.Insert(1, new ListItem(strY1.Substring(0, strY1.Length - 1), GlobalString.cardparam.Y1));
        dropDay.Items.Insert(2, new ListItem(strY2.Substring(0, strY2.Length - 1), GlobalString.cardparam.Y2));
        dropDay.Items.Insert(3, new ListItem(strY3.Substring(0, strY3.Length - 1), GlobalString.cardparam.Y3));
    }



}
