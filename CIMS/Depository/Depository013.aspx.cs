//******************************************************************
//*  作    者：Cuiyan Ma
//*  功能說明：每月監控
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

public partial class Depository_Depository013 : PageBase
{
    Depository013BL bizlogic = new Depository013BL();
    Report032BL rep = new Report032BL();
    Dictionary<string, DataTable> dirManage = new Dictionary<string, DataTable>();
    DataTable temp = new DataTable();
    #region 事件處理
    /// <summary>
    /// 頁面初始化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        UctrlCardType.SetLeftItem = Depository013BL.SEL_CARD_TYPE;
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

            int Months = int.Parse(dropMonthCol.SelectedValue);
            Dictionary<string, DataTable> dirResult = (Dictionary<string, DataTable>)Session["dirManage13"]; // Legend 2017/05/04 改變Key名稱
            if (dirResult != null)
            {
                HtmlTable table = new HtmlTable();
                table.ID = "tb_DayManage";
                table.CellSpacing = 2;
                table.Width = "100%";
                table.Attributes.Add("class", "GridView");
                tdResult.Controls.Add(table);
                DataBound(dirResult, Months, table);
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
        if (!rep.IsMONTHLY_MONITOR())
        {
            ShowMessage("當日未執行每月監控批次");
            return;
        }
        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        if (this.Type1.Checked) {
            UctrlCardType.SetRightItem = new DataTable();
        }
        if (this.Type2.Checked) {
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
            DataTable dtblPersoCardtype =  bizlogic.PreCardType(inputs);        
            tdResult.Controls.Remove(tdResult.FindControl("tb_DayManage"));
            if (dtblPersoCardtype==null||dtblPersoCardtype.Rows.Count==0) {
               sp001.Visible = true;
               btnReport.Visible = false;
               btnSubmit.Visible = false;
            }
            else
            {
              sp001.Visible = false;
              btnReport.Visible = true;
              btnSubmit.Visible = true;
            int Months = int.Parse(dropMonthCol.SelectedValue);
            HtmlTable table = new HtmlTable();
            table.ID = "tb_DayManage";
            table.CellSpacing = 2;
            table.Width = "100%";
            table.Attributes.Add("class", "GridView");
            tdResult.Controls.Add(table);
            if (StringUtil.IsEmpty(dropMonth.SelectedValue))
            {
                foreach (ListItem item in dropMonth.Items)
                {
                    if(item != dropMonth.SelectedItem) {
                        int XValue = int.Parse(item.Text);
                        int XCode = int.Parse(item.Value);
                        drawTable(dtblPersoCardtype,XCode, XValue, Months, table);
                    } 
                }
            }
            else {
                int XValue = int.Parse(dropMonth.SelectedItem.Text);
                int XCode = int.Parse(dropMonth.SelectedValue);
                drawTable(dtblPersoCardtype, XCode, XValue, Months, table);
            }
            Session["dirManage13"] = dirManage;        // Legend 2017/05/04 改變Key名稱
            }
            UctrlCardType.LbLeftBind();
     
        }catch(Exception ex){
            ShowMessage(ex.Message);        
        }
    }

    /// <summary>
    /// 根據查詢結果分廠分卡種畫出查詢結果
    /// </summary>
    /// <param name="dtblPersoCardtype"></param>
    /// <param name="Month"></param>
    /// <param name="Months"></param>
    private void drawTable(DataTable dtblPersoCardtype,int xCode, int Month, int Months, HtmlTable tb_DayManage)
    {
        DataRow[] PersoCards = dtblPersoCardtype.Select("XType = '" + xCode + "'", "HNumber asc");
        DataSet dst = bizlogic.List(PersoCards, xCode, Months);
        if (dst.Tables.Count != 0)
        {
            //表頭     
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("style", "color:Black;background-color:#B9BDAA;font-size:Small;font-weight:bolder;");
            HtmlTableCell CardCell = new HtmlTableCell();
            CardCell.ColSpan = 2;
           if(xCode.ToString().Equals(GlobalString.cardparam.X1)) 
                 CardCell.InnerText = "X1=" + Month;
           else if(xCode.ToString().Equals(GlobalString.cardparam.X2)) 
                 CardCell.InnerText = "X2=" + Month;
             else if (xCode.ToString().Equals(GlobalString.cardparam.X3))
                 CardCell.InnerText = "X3=" + Month;
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
            for (int i = 0; i < Months; i++)
            {
                HtmlTableCell monthCell = new HtmlTableCell();
                monthCell.InnerText = DateTime.Now.AddMonths(i).ToString("yyyy/MM");
                row.Cells.Add(monthCell);
            }
            tb_DayManage.Rows.Add(row);
            int j = 0;
            //表内容
            foreach (DataRow cardRow in PersoCards)
            {
                string factoryRid = cardRow["Perso_Factory_RID"].ToString();
                string factoryName = bizlogic.getFactory(int.Parse(factoryRid)).Factory_ShortName_CN;
                string cardRid = cardRow["CardRID"].ToString();
                string CardName = cardRow["Name"].ToString();
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
                            link.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository013Detail.aspx?table=" + factoryRid + "_" + cardRid + "_" + Month + "_" + xCode + "','','dialogHeight:450px;dialogWidth:800px;resizable=yes;');if(aa!=undefined){changeTable();}");
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
                        string strN = bizlogic.paramList(GlobalString.cardparam.NType);
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
                j++;
            }
        }
    
    }
    /// <summary>
    /// 轉化成千分位
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private string changeData(Object obj) {
        try
        {
            if (obj.ToString().IndexOf('.') != -1)
            {
               return Convert.ToDecimal(obj).ToString("N1");
            }
            else
            {
               return Convert.ToInt32(obj).ToString("N0");
            }
        }catch{

            return "無法計算";
        }
    
    }
    /// <summary>
    /// 從詳細畫面返回的時候執行
    /// </summary>
    /// <param name="dirTable"></param>
    /// <param name="Months"></param>
    /// <param name="tb_DayManage"></param>
    private void DataBound(Dictionary<string, DataTable> dirTable, int Months, HtmlTable tb_DayManage)
    {
        string flag = "0";
        foreach (KeyValuePair<string, DataTable> key in dirTable)
        {
            DataTable datable = (DataTable)key.Value;
            string KeyValue = key.Key;
            int persoRid = int.Parse(KeyValue.Split('_')[0]);
            FACTORY factory = bizlogic.getFactory(persoRid);
            int cardRid = int.Parse(KeyValue.Split('_')[1]);
            CARD_TYPE cardModel = bizlogic.getCardType(cardRid);
            //表頭
            if (flag == "0" || KeyValue.Split('_')[3]!=flag)
            {
                HtmlTableRow row = new HtmlTableRow();
                row.Attributes.Add("style", "color:Black;background-color:#B9BDAA;font-size:Small;font-weight:bolder;");
                HtmlTableCell CardCell = new HtmlTableCell();
                CardCell.ColSpan = 2;
                if (KeyValue.Split('_')[3].Equals(GlobalString.cardparam.X1))
                    CardCell.InnerText = "X1=" + KeyValue.Split('_')[2];
                else if (KeyValue.Split('_')[3].Equals(GlobalString.cardparam.X2))
                    CardCell.InnerText = "X2=" + KeyValue.Split('_')[2];
                else if (KeyValue.Split('_')[3].Equals(GlobalString.cardparam.X3))
                    CardCell.InnerText = "X3=" + KeyValue.Split('_')[2];
                else
                {
                    ShowMessage("參數設定有誤");
                    return;
                } 
                //CardCell.InnerText = "X1=" + KeyValue.Split('_')[2];
                row.Cells.Add(CardCell);          
                HtmlTableCell PersoCell = new HtmlTableCell();
                PersoCell.InnerText = "Perso廠";
                row.Cells.Add(PersoCell);
                HtmlTableCell blankCell = new HtmlTableCell();
                row.Cells.Add(blankCell);
                for (int i = 0; i < Months; i++)
                {
                    HtmlTableCell monthCell = new HtmlTableCell();
                    monthCell.InnerText = DateTime.Now.AddMonths(i).ToString("yyyy/MM");
                    row.Cells.Add(monthCell);
                }
                tb_DayManage.Rows.Add(row);
                flag = KeyValue.Split('_')[3];
            }
            int k = 0;
            //表内容
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
                        link.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository013Detail.aspx?table=" + KeyValue + "','','dialogHeight:450px;dialogWidth:800px;');if(aa!=undefined){changeTable();}");
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
                    string strN = bizlogic.paramList(GlobalString.cardparam.NType);
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

    protected void btnDrawTable_Click(object sender, EventArgs e)
    {
           
    }

    /// <summary>
    /// 匯出Excel格式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_click(object sender, EventArgs e)
    {
        Dictionary<string, DataTable> dirTable = (Dictionary<string, DataTable>)Session["dirManage13"];    // Legend 2017/05/04 改變Key名稱
        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
        bizlogic.inputData2Temp(dirTable, time);   
        Response.Write("<script>window.open('Depository013Print.aspx?time="+time+"');</script>");
      
    }

    /// <summary>
    /// 下單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_click(object sender, EventArgs e)
    {
        Dictionary<string, int> dirCardType = new Dictionary<string, int>();
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
        Dictionary<string, DataTable> dirResult = (Dictionary<string, DataTable>)Session["dirManage13"];   // Legend 2017/05/04 改變Key名稱
        DataTable PersoCard = (DataTable)Session["PersoCard"];
        foreach (KeyValuePair<string, DataTable> key in dirResult)
        {
          CheckBox CardCheck = (CheckBox)tdResult.FindControl("chk_"+key.Key);
          DataTable table = (DataTable)key.Value;
          FACTORY factory = bizlogic.getFactory(int.Parse(key.Key.Split('_')[0]));
          int tableId = int.Parse(key.Key.Split('_')[0]);
          if (CardCheck.Checked) {
              if (dirCardType.ContainsKey(key.Key.Split('_')[1]))
              {
                  ShowMessage("有重復卡種！");
                  return;
              }
              else
              {
                  dirCardType.Add(key.Key.Split('_')[1], 1);
              }
              for (int i = 0; i < table.Rows.Count; i++)
              {
                  DataRow drow = temp.NewRow();
                  drow["Delivery_Address_RID"] = key.Key.Split('_')[0];
                  drow["Space_Short_RID"] = key.Key.Split('_')[1];
                  drow["factory_shortname_cn"] = factory.Factory_ShortName_CN;
                  drow["number"] = table.Rows[i]["J"].ToString().Replace(",", "");
                  drow["Fore_Delivery_Date"] = DateTime.Now.AddMonths(i).ToString("yyyy/MM/01");
                  drow["bz"] = 2;
                  if (Convert.ToInt32(drow["number"]) != 0)
                      temp.Rows.Add(drow);
              } 
          }
        }
        if (temp.Rows.Count < 1) {
            ShowMessage("沒有選擇任何卡種下單！");
            return;
        }
        if (temp.Rows.Count > 99) {
            ShowMessage("超過最大下單量！");
            return;
        }
        Session["monitory"] = temp;
        Response.Redirect("Depository002Add.aspx?type=1");
       
    }
    #endregion    


    private void FillDropMonth()
    {

        for (int i = 1; i <= 16;i++ )
        {
            this.dropMonthCol.Items.Add(new ListItem(i+"", i+""));
        }
        string strX1 = bizlogic.paramList(GlobalString.cardparam.X1);
        string strX2 = bizlogic.paramList(GlobalString.cardparam.X2);
        string strX3 = bizlogic.paramList(GlobalString.cardparam.X3);
        //int X1 = int.Parse(strX.Substring(0, strX.Length - 1));
        dropMonth.Items.Insert(0, new ListItem("全部", ""));
        dropMonth.Items.Insert(1, new ListItem(strX1.Substring(0, strX1.Length - 1), GlobalString.cardparam.X1));
        dropMonth.Items.Insert(2, new ListItem(strX2.Substring(0, strX2.Length - 1), GlobalString.cardparam.X2));
        dropMonth.Items.Insert(3, new ListItem(strX3.Substring(0, strX3.Length - 1), GlobalString.cardparam.X3));
    }
 }
