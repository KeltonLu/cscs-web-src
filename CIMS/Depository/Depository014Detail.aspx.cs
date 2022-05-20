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

public partial class Depository_Depository014Detail : PageBase
{
    
    Depository014BL bizlogic = new Depository014BL();
    int code = 0;
    Dictionary<string, DataTable> dirManage = new Dictionary<string, DataTable>();
    DataTable PersoCard = new DataTable();
    CARD_TYPE cardTypeModel = new CARD_TYPE();
    FACTORY factory = new FACTORY();
    int persoRid = 0;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Response.ExpiresAbsolute = DateTime.Now;//頁面不被緩存
        this.Response.Expires = 0;
        this.Response.CacheControl = "no-cache";
        string strTable = Request.QueryString["table"];
        persoRid = Convert.ToInt32(strTable.Split('_')[0]);
        cardTypeModel = bizlogic.getCardType(int.Parse(strTable.Split('_')[1]));
        factory = bizlogic.getFactory(persoRid);
        code = Convert.ToInt32(strTable.Split('_')[3]);
        int xTypeValue = int.Parse(strTable.Split('_')[2]);       
        dirManage = (Dictionary<string, DataTable>)Session["dirManage14"];     // Legend 2017/05/04 改變Key名稱
        DataTable dtbDetail = new DataTable();
        bizlogic.creatNewDataTable2(dtbDetail);
        foreach (DataRow drownew in ((DataTable)dirManage[strTable]).Rows)
        {
            DataRow drow = dtbDetail.NewRow();
            drow.ItemArray = drownew.ItemArray;
            dtbDetail.Rows.Add(drow);
        }
        string flag = "2";
        foreach (DataRow drow in dtbDetail.Rows)
        {
            if (drow["J"].ToString() != "0")
            {
                flag = "0";
            }
        }
        DataTable dtbCompute = bizlogic.compute(cardTypeModel,persoRid,dtbDetail,code,flag);
        //bizlogic.getTableAfterChange(dtbDetail, flag);
        ViewState["detail"] = dtbCompute;
        ViewState["tableId"] = strTable;
        drawTables(dtbCompute);
    }

    /// <summary>
    /// 計算按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCompute_Click(object sender, EventArgs e)
    {

        DataTable dtbInput = (DataTable)ViewState["detail"];        
        int xTypeValue = int.Parse(ViewState["tableId"].ToString().Split('_')[1]);
        HtmlTable table = (HtmlTable)div001.FindControl("tb_DayManage");
        for (int i = 0; i < dtbInput.Rows.Count; i++)
        {
            dtbInput.Rows[i]["C"] = ((TextBox)table.FindControl("C"+i)).Text;
            dtbInput.Rows[i]["TotalXH"] = toInt(dtbInput.Rows[i]["B"]) + toInt(dtbInput.Rows[i]["C"]) +toInt(dtbInput.Rows[i]["D1"]) +toInt(dtbInput.Rows[i]["D2"]) +toInt(dtbInput.Rows[i]["E"]);
            dtbInput.Rows[i]["J"] = ((TextBox)table.FindControl("J"+i)).Text;
        }
        DataTable result = bizlogic.compute(cardTypeModel, persoRid, dtbInput, code,"0");
        div001.Controls.Remove(table);
        drawTables(result);        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dtbDetail1"></param>
    private void drawTables(DataTable dtbDetail1)
    {
        for (int i = 0; i < dtbDetail1.Columns.Count; i++)
        {
            if (dtbDetail1.Columns[i].ColumnName == "A1")
            {
                dtbDetail1.Columns[i].ColumnName = "A'";
            }
            if (dtbDetail1.Columns[i].ColumnName == "G1")
            {
                dtbDetail1.Columns[i].ColumnName = "G'";
            }
        }
        PersoCard = (DataTable)Session["PersoCard"];
        HtmlTable table = new HtmlTable();
        table.ID = "tb_DayManage";
        table.CellSpacing = 2;
        table.Width = "100%";
        table.Attributes.Add("class", "GridView");
        div001.Controls.Add(table);
        HtmlTableRow row = new HtmlTableRow();
        row.Attributes.Add("style", "color:Black;background-color:#B9BDAA;font-size:Small;font-weight:bolder;");
        HtmlTableCell TypeCell = new HtmlTableCell();
        TypeCell.InnerText = "Card Type";
        row.Cells.Add(TypeCell);
        HtmlTableCell AffinCell = new HtmlTableCell();
        AffinCell.InnerText = "Affinity";
        row.Cells.Add(AffinCell);
        HtmlTableCell PhotoCell = new HtmlTableCell();
        PhotoCell.InnerText = "Photo";
        row.Cells.Add(PhotoCell);
        HtmlTableCell CardCell = new HtmlTableCell();
        CardCell.InnerText = "版面簡稱";
        row.Cells.Add(CardCell);
        HtmlTableCell PersoCell = new HtmlTableCell();
        PersoCell.InnerText = "Perso廠";
        row.Cells.Add(PersoCell);
        HtmlTableCell blankCell = new HtmlTableCell();
        row.Cells.Add(blankCell);
        foreach (DataRow drow in dtbDetail1.Rows)
        {
            HtmlTableCell monthCell = new HtmlTableCell();
            monthCell.InnerText = Convert.ToDateTime(drow["Day"]).ToString("yyyy/MM/dd");
            row.Cells.Add(monthCell);
        }
        table.Rows.Add(row);

        int k = 0;
        foreach (DataColumn col in dtbDetail1.Columns)
        {
            string name = col.ColumnName;
            if (name != "TotalXH" && name!="Day")
            {
                HtmlTableRow row1 = new HtmlTableRow();
                row1.Attributes.Add("class", "GridViewRow");
                if (k == 0)
                {
                    HtmlTableCell CardCell1 = new HtmlTableCell();
                    CardCell1.RowSpan = dtbDetail1.Columns.Count - 1;
                    CardCell1.InnerText = cardTypeModel.TYPE;
                    row1.Cells.Add(CardCell1);
                    HtmlTableCell AffCell1 = new HtmlTableCell();
                    AffCell1.RowSpan = dtbDetail1.Columns.Count - 1;
                    AffCell1.InnerText = cardTypeModel.AFFINITY;
                    row1.Cells.Add(AffCell1);
                    HtmlTableCell PhotoCell1 = new HtmlTableCell();
                    PhotoCell1.RowSpan = dtbDetail1.Columns.Count - 1;
                    PhotoCell1.InnerText = cardTypeModel.PHOTO;
                    row1.Cells.Add(PhotoCell1);
                    HtmlTableCell CardNameCell1 = new HtmlTableCell();
                    CardNameCell1.RowSpan = dtbDetail1.Columns.Count - 1;
                    CardNameCell1.InnerText = cardTypeModel.Name;
                    row1.Cells.Add(CardNameCell1);
                    HtmlTableCell PersoCell1 = new HtmlTableCell();
                    PersoCell1.RowSpan = dtbDetail1.Columns.Count - 1;
                    PersoCell1.InnerText = factory.Factory_ShortName_CN;
                    row1.Cells.Add(PersoCell1);
                }
                HtmlTableCell blankCell1 = new HtmlTableCell();
                blankCell1.InnerText = name;
                row1.Cells.Add(blankCell1);

                if (name == "C")
                {
                    for (int conNum = 0; conNum < dtbDetail1.Rows.Count; conNum++)
                    {
                        HtmlTableCell txtBoxCell = new HtmlTableCell();
                        TextBox txt = new TextBox();
                        txt.Width = 80;
                        txt.Attributes.Add("style", "text-align: right"); 
                        txt.ID = name + conNum;
                        txt.Text = dtbDetail1.Rows[conNum][name].ToString();
                        txtBoxCell.Controls.Add(txt);
                        row1.Cells.Add(txtBoxCell);
                        txt.Attributes.Add("onfocus", "DelDouhao(this)");
                        txt.Attributes.Add("onblur", "CheckIntNum('" + txt.ID + "',9);value=GetValue(this.value)");
                        txt.Attributes.Add("onkeyup", "CheckIntNum('" + txt.ID + "',9)");
                    }
                }
                else if (name == "J")
                { //J只能輸入正數           
                    for (int conNum = 0; conNum < dtbDetail1.Rows.Count; conNum++)
                    {
                        HtmlTableCell txtBoxCell = new HtmlTableCell();
                        TextBox txt = new TextBox();
                        txt.Width = 80;
                        txt.Attributes.Add("style","text-align: right"); 
                        txt.ID = name + conNum;
                        txt.Text = dtbDetail1.Rows[conNum][name].ToString();
                        txtBoxCell.Controls.Add(txt);
                        row1.Cells.Add(txtBoxCell);
                        txt.Attributes.Add("onfocus", "DelDouhao(this)");
                        txt.Attributes.Add("onblur", "CheckNum('" + txt.ID + "',9);value=GetValue(this.value)");
                        txt.Attributes.Add("onkeyup", "CheckNum('" + txt.ID + "',9)");
                    }
                }
                else
                {
                    string strN = bizlogic.paramList(GlobalString.cardparam.YType);
                    decimal n = Convert.ToDecimal(strN.Substring(0, strN.Length - 1));                    
                    foreach (DataRow drow in dtbDetail1.Rows)
                    {
                        HtmlTableCell monthCell = new HtmlTableCell();
                        monthCell.Align = "right";
                        monthCell.InnerText = changeData(drow[name]); 
                         if (name == "A'" || name == "G'") 
                             monthCell.BgColor= "Yellow";
                         if (name == "H" && Convert.ToDecimal(monthCell.InnerText) < n)
                         {
                             monthCell.BgColor = "Red";//H=0,<N 返紅
                         }
                         if (name == "L" && Convert.ToDecimal(monthCell.InnerText) < n)
                             monthCell.BgColor = "Red"; //L<N 返紅
                        row1.Cells.Add(monthCell);
                    }
                }
                table.Rows.Add(row1);
                k++;
            }
        }
    }

    private string changeData(Object obj)
    {
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
        }
        catch
        {

            return "無法計算";
        }

    }

    private int toInt(object obj) {
        if (obj != null) {
            return int.Parse(obj.ToString().Replace(",", ""));
        }
        return 0;    
    }

    /// <summary>
    /// 確定提交資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        DataTable dtbInput = (DataTable)ViewState["detail"];
        String strTable = ViewState["tableId"].ToString();      
        int tableId = int.Parse(strTable.Split('_')[0]);
        int xTypeValue = int.Parse(strTable.Split('_')[2]);
        HtmlTable table = (HtmlTable)div001.FindControl("tb_DayManage");
        for (int i = 0; i < dtbInput.Rows.Count; i++)
        {
            dtbInput.Rows[i]["C"] = ((TextBox)table.FindControl("C" + i)).Text.Replace(",", "");
            dtbInput.Rows[i]["TotalXH"] = toInt(dtbInput.Rows[i]["B"]) + toInt(dtbInput.Rows[i]["C"]) + toInt(dtbInput.Rows[i]["D1"]) + toInt(dtbInput.Rows[i]["D2"]) + toInt(dtbInput.Rows[i]["E"]);
            dtbInput.Rows[i]["J"] = ((TextBox)table.FindControl("J" + i)).Text.Replace(",", "");
        }
        DataTable dtbCompute= bizlogic.saveDate(cardTypeModel, persoRid, dtbInput, code, xTypeValue);
        dirManage[strTable] = dtbCompute;
        Session["dirManage14"] = dirManage;    // Legend 2017/05/04 改變Key名稱    
        Response.Write("<script>returnValue='1';window.close();</script>");
    }

}
