//******************************************************************
//*  作    者：James
//*  功能說明：物料庫存管控
//*  創建日期：2008-09-20
//*  修改日期：2008-09-20 12:00
//*  修改記錄：
//*            □2008-09-20
//*              1.創建 占偉林
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
using System.IO;
using System.Collections;
using CIMSClass.Business;

public partial class Depository_Depository010 : PageBase
{
    Depository010BL bl = new Depository010BL();

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        this.gvpbPersoStockIn.PageSize = GlobalStringManager.PageSize;

        if (!IsPostBack)
        {
            try
            {
                DataTable dtblFactory = (DataTable)bl.GetFactoryList();
                this.dropFactoryDel.DataSource = dtblFactory.DefaultView;
                this.dropFactoryDel.DataTextField = "Factory_ShortName_CN";
                this.dropFactoryDel.DataValueField = "RID";
                this.dropFactoryDel.DataBind();
                this.dropFactoryIn.DataSource = dtblFactory.DefaultView;
                this.dropFactoryIn.DataTextField = "Factory_ShortName_CN";
                this.dropFactoryIn.DataValueField = "RID";
                this.dropFactoryIn.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }

    /// <summary>
    /// 查詢卡片庫存單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvpbPersoStockIn.BindData();
    }

    #endregion

    #region 欄位/數據補充說明
    #endregion

    #region 列表數據綁定
    /// <summary>
    /// GridView數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbPersoStockIn_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        //查詢條件集合，Key是欖位名稱，Value是欄位對應的控製項，控製項可以是TextBox或DropDownList
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        inputs.Add("FileUpd", this.FileUpd.PostedFile.FileName);
        inputs.Add("FactoryRID", this.dropFactoryIn.SelectedValue.ToString());

        DataSet dstlMaterielStocksIn = null;
        ViewState["MaterielStocksIn"] = null;
        ViewState["MaterielStocksInNum"] = 0;

        try
        {
            // 上傳物料庫存控管
            string strPath = FileUpload(this.FileUpd.PostedFile);
            if (!StringUtil.IsEmpty(strPath))
            {
                // 對匯入資料進行檢查
                dstlMaterielStocksIn = bl.CheckIn(strPath, this.dropFactoryIn.SelectedValue.ToString());

                // 檢查成功，綁定GRID，顯示匯入資料明細
                if (dstlMaterielStocksIn != null)//如果查到了資料
                {
                    ViewState["MaterielStocksIn"] = dstlMaterielStocksIn;
                    ViewState["MaterielStocksInNum"] = dstlMaterielStocksIn.Tables[1].Rows.Count;
                    // edit by Ian Huang start
                    DataTable dt = dstlMaterielStocksIn.Tables[1].Copy();
                    List<DataRow> lDR = new List<DataRow>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        if ("Y" == dr["IsAdd"].ToString().Trim().ToUpper())
                        {
                            lDR.Add(dr);
                        }
                    }

                    for (int i = 0; i < lDR.Count; i++)
                    {
                        dt.Rows.Remove(lDR[i]);
                    }

                    e.Table = dt;//要綁定的資料表
                    e.RowCount = dt.Rows.Count;//查到的行數
                    //e.Table = dstlMaterielStocksIn.Tables[1];//要綁定的資料表
                    //e.RowCount = dstlMaterielStocksIn.Tables[1].Rows.Count;//查到的行數
                    // edit by Ian Huang end



                    ViewState["FileName"] = FileUpd.FileName;

                    bl.CheckMATERIEL_STOCKS(dstlMaterielStocksIn);

                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 文件上傳功能
    /// </summary>
    /// <param name="file">上傳文件</param>
    protected new string FileUpload(HttpPostedFile file)
    {
        string path = "";
        string tmpname = file.FileName;
        if (tmpname.Split(new char[] { '-' }).Length != 3)
        {
            ShowMessage("匯入文件名稱格式不正確。");
            return "";
        }
        string date = tmpname.Split(new char[] { '-' })[1];

        if (IsFolderExist(date))
        {
            int i = tmpname.LastIndexOf("\\");
            string filename = tmpname.Substring(i + 1);
            string filetype = filename.Substring(filename.LastIndexOf(".") + 1);

            if (filetype.ToLower().Equals("txt"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a <= 10000.00)
                    {
                        path = ConfigurationManager.AppSettings["MaterialStocksManager"].ToString() + date + "\\" + filename;
                        // 如果目錄下文件已經存在，先刪除掉。
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        file.SaveAs(ConfigurationManager.AppSettings["MaterialStocksManager"].ToString() + date + "\\" + filename);
                    }
                    else
                    {
                        ShowMessage("大小不能大於10M");
                    }
                }
                catch
                {
                    ShowMessage("上傳失敗");
                }
            }
            else
            {
                ShowMessage("格式錯誤");
            }
        }
        return path;
    }
    //************************************************************************
    /// <summary>
    /// 建立上傳目錄
    /// </summary>
    /// <returns>true：成功 false：失敗</returns>
    //************************************************************************
    public bool IsFolderExist(string date)
    {
        string basepath = ConfigurationManager.AppSettings["MaterialStocksManager"].ToString();
        //string date = txtBegin_Date.Text.Replace("/", "");
        try
        {
            // Determine whether the directory exists.	
            if (!Directory.Exists(basepath + date))
            {
                // Create the directory it does not exist.
                Directory.CreateDirectory(basepath + date);
            }
            return true;
        }

        catch
        {
            return false;
        }
    }

    /// <summary>
    /// GridView列數據綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbPersoStockIn_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // edit by Ian Huang start
        DataTable dtblPersoStockIn = ((DataSet)ViewState["MaterielStocksIn"]).Tables[1];
        //DataTable dtblPersoStockIn = (DataTable)gvpbPersoStockIn.DataSource;
        // edit by Ian Huang end

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (0 == Convert.ToInt32(ViewState["MaterielStocksInNum"]))
                return;

            try
            {
                // 取物料品名、RID
                DataSet dtsMateriel = null;
                dtsMateriel = bl.GetMateriel(Convert.ToString(dtblPersoStockIn.Rows[e.Row.RowIndex]["Serial_Number"]));
                if (null != dtsMateriel && dtsMateriel.Tables.Count > 0 && dtsMateriel.Tables[0].Rows.Count > 0)
                {
                    // 品名
                    e.Row.Cells[0].Text = Convert.ToString(dtsMateriel.Tables[0].Rows[0]["Name"]);
                    // 物品的RID
                    dtblPersoStockIn.Rows[e.Row.RowIndex]["Materiel_RID"] = Convert.ToInt32(dtsMateriel.Tables[0].Rows[0]["RID"]);
                }
                // 日期
                e.Row.Cells[1].Text = Convert.ToDateTime(dtblPersoStockIn.Rows[e.Row.RowIndex]["Stock_Date"]).ToString("yyyy/MM/dd");
                // 類型
                e.Row.Cells[2].Text = bl.getTypeName("0" + Convert.ToString(dtblPersoStockIn.Rows[e.Row.RowIndex]["Type"]));
                // 數量
                e.Row.Cells[3].Text = Convert.ToInt32(dtblPersoStockIn.Rows[e.Row.RowIndex]["Number"]).ToString("N0");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
    #endregion

    /// <summary>
    /// 删除這個Perso廠的結餘日期到上一次結餘日期之間的資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        bool blImportSubTotalStart = false;

        if (StringUtil.IsEmpty(this.txtDate.Text.Trim()))
        {
            ShowMessage("廠商結餘日期必須輸入！");
            return;
        }

        if (StringUtil.IsEmpty(this.dropFactoryDel.SelectedValue.ToString()))
        {
            ShowMessage("Perso廠商必須輸入！");
            return;
        }

        try
        {
            if (!bl.ImportSubTotalStart())
            {
                ShowMessage("物料庫存控管匯入已經開始，不能重復開始！");
                return;
            }
            blImportSubTotalStart = true;

            // 刪除物料結餘
            bl.Delete(DateTime.Parse(this.txtDate.Text), Convert.ToInt32(this.dropFactoryDel.SelectedValue.ToString()));
            // add by Ian Huang start
            bl.delMove(dropFactoryDel.SelectedValue, txtDate.Text, "");
            // add by Ian Huang end
            gvpbPersoStockIn.DataSource = null;
            gvpbPersoStockIn.DataBind();

            ShowMessage("廠商結餘訊息刪除成功！");

            bl.ImportSubTotalEnd();
        }
        catch (Exception ex)
        {
            if (blImportSubTotalStart)
                bl.ImportSubTotalEnd();
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 匯入前檢查
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDataIn_Click(object sender, EventArgs e)
    {
        if (StringUtil.IsEmpty(this.FileUpd.PostedFile.FileName.ToString()))
        {
            ShowMessage("匯入資料必須輸入！");
            return;
        }

        if (StringUtil.IsEmpty(this.dropFactoryIn.SelectedValue.ToString()))
        {
            ShowMessage("Perso廠商必須輸入！");
            return;
        }

        gvpbPersoStockIn.BindData();
    }

    /// <summary>
    /// 確定匯入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        bool blImportSubTotalStart = false;

        // 檢查有無匯入的數據
        if (0 == Convert.ToInt32(ViewState["MaterielStocksInNum"]))
        {
            ShowMessage("無匯入的資料！");
            return;
        }

        try
        {
            // 從Session中取匯入資料
            DataTable dtFactory = ((DataSet)ViewState["MaterielStocksIn"]).Tables[0];
            DataTable dtMaterielStocksIn = ((DataSet)ViewState["MaterielStocksIn"]).Tables[1];

            // 取UI上的備註字段信息
            for (int intRow = 0; intRow < gvpbPersoStockIn.Rows.Count; intRow++)
            {
                TextBox txtComment = (TextBox)gvpbPersoStockIn.Rows[intRow].FindControl("txtComment");
                if (!StringUtil.IsEmpty(txtComment.Text.Trim()))
                {
                    dtMaterielStocksIn.Rows[intRow]["Comment"] = txtComment.Text.Trim();
                }
            }

            // 物品的移入訊息
            List<object> listStocksMoveInOnDay = bl.getStocksMoveInOnDay(dtMaterielStocksIn, dtFactory);

            // 物品的移出訊息
            List<object> listStocksMoveOutOnDay = bl.getStocksMoveOutOnDay(dtMaterielStocksIn, dtFactory);

            // 從物料庫存消耗檔中取耗用量
            List<object> listMaterielUsedOnDay = bl.getMaterielUsedOnDay(dtMaterielStocksIn, dtFactory);

            ////add by Ian Huang start
            //// 從物料庫存異動檔中取進貨、退貨、銷毀訊息
            //List<object> listStocksTransactionOnDay = bl.getStocksTransactionOnDay(dtMaterielStocksIn, dtFactory);
            ////add by Ian Huang end

            foreach (DataRow dr in listStocksMoveInOnDay)
            {
                dtMaterielStocksIn.Rows.Add(dr);
            }

            foreach (DataRow dr in listStocksMoveOutOnDay)
            {
                dtMaterielStocksIn.Rows.Add(dr);
            }

            foreach (DataRow dr in listMaterielUsedOnDay)
            {
                dtMaterielStocksIn.Rows.Add(dr);
            }

            ////add by Ian Huang start
            //foreach (DataRow dr in listStocksTransactionOnDay)
            //{
            //    dtMaterielStocksIn.Rows.Add(dr);
            //}
            ////add by Ian Huang end

            // 按物品編號、時間、類型排次序重新排序
            dtMaterielStocksIn.DefaultView.Sort = "Serial_Number,Stock_Date,Type ASC";
            // dtMaterielStocksIn.DefaultView.Sort = "Serial_Number,Stock_Date ASC";
            dtMaterielStocksIn = dtMaterielStocksIn.DefaultView.ToTable();


            // 整理匯入資料訊息
            bl.DoMaterielStocksIn(dtMaterielStocksIn, dtFactory);

            // 按物品編號、時間、類型排次序重新排序
            dtMaterielStocksIn.DefaultView.Sort = "Serial_Number,Stock_Date,Type ASC";
            // dtMaterielStocksIn.DefaultView.Sort = "Serial_Number,Stock_Date ASC";
            dtMaterielStocksIn = dtMaterielStocksIn.DefaultView.ToTable();
            // 開始連接，并開始事務
            bl.dao.OpenConnection();

            if (!bl.ImportSubTotalStart())
            {
                ShowMessage("物料庫存控管匯入已經開始，不能重復開始！");
                return;
            }
            blImportSubTotalStart = true;

            // 保存
            DataIn(dtMaterielStocksIn, dtFactory);

            bl.ImportSubTotalEnd();

            bl.SetOprLog("11");

            string strFileName = "";
            if (ViewState["FileName"] != null)
                strFileName = ViewState["FileName"].ToString();

            InOut006BL bl1 = new InOut006BL();
            bl1.dao = bl.dao;
            bl1.AddLog("4", strFileName);

            bl.dao.Commit();

            gvpbPersoStockIn.DataSource = null;
            gvpbPersoStockIn.DataBind();

            // 匯入成功
            ShowMessage("廠商匯入資料訊息保存成功！");

        }
        catch (Exception ex)
        {
            if (blImportSubTotalStart)
                bl.ImportSubTotalEnd();

            bl.dao.Rollback();
            ShowMessage(ex.Message);
        }
        finally
        {
            bl.dao.CloseConnection();
        }
    }



    /// <summary>
    /// 保存匯入資料訊息
    /// </summary>
    /// <param name="dtMaterielStockIn">匯入資料表</param>
    /// <param name="dtblFactory">廠商訊息</param>
    private void DataIn(DataTable dtMaterielStockIn, DataTable dtblFactory)
    {
        try
        {
            MATERIEL_STOCKS_MANAGE msmModel;
            ArrayList arrWorkDate = bl.getAllWorkDate();

            string Serial_Number = "";
            string Materiel_Name = "";

            //先刪除掉DB中從上次結余以來的記錄！
            for (int intRowDel = 0; intRowDel < dtMaterielStockIn.Rows.Count; intRowDel++)
            {
                DataRow drMaterielStockDel = dtMaterielStockIn.Rows[intRowDel];
                if (Convert.ToString(drMaterielStockDel["Serial_Number"]) != "")
                {
                    Serial_Number = Convert.ToString(drMaterielStockDel["Serial_Number"]);

                    if (drMaterielStockDel["type"].ToString() == "4")
                    {
                        bl.DeleteSysSurplusData(Convert.ToInt32(dtblFactory.Rows[0]["Factory_RID"]),
                                                Serial_Number,
                                                Convert.ToDateTime(drMaterielStockDel["Last_Surplus_Date"]),
                                                Convert.ToDateTime(drMaterielStockDel["Stock_Date"]));
                    }
                }
            }


            for (int intRow = 0; intRow < dtMaterielStockIn.Rows.Count; intRow++)
            {
                string strWearRate = "0";

                DataRow drMaterielStockIn = dtMaterielStockIn.Rows[intRow];

                Serial_Number = Convert.ToString(drMaterielStockIn["Serial_Number"]);
                Materiel_Name = bl.getMateriel_Name(Serial_Number);

                //#region 刪除物料上次結餘和本次結餘之間的系統結餘，即Type = 4的記錄
                //if (Convert.ToString(drMaterielStockIn["Serial_Number"])!="")
                //{
                //    Serial_Number = Convert.ToString(drMaterielStockIn["Serial_Number"]);

                //    Materiel_Name = bl.getMateriel_Name(Serial_Number);

                //    if (drMaterielStockIn["type"].ToString() == "4")
                //    {
                //        bl.DeleteSysSurplusData(Convert.ToInt32(dtblFactory.Rows[0]["Factory_RID"]),
                //                                Serial_Number,
                //                                Convert.ToDateTime(drMaterielStockIn["Last_Surplus_Date"]),
                //                                Convert.ToDateTime(drMaterielStockIn["Stock_Date"]));
                //    }
                //}
                //#endregion 刪除物料上次結餘和本次結餘之間的系統結餘，即Type = 4的記錄

                // 添加物料異動訊息
                // update by Ian Huang start
                if (1 == Convert.ToInt32(drMaterielStockIn["Type"]) ||
                    2 == Convert.ToInt32(drMaterielStockIn["Type"]) ||
                    3 == Convert.ToInt32(drMaterielStockIn["Type"]) ||
                    6 == Convert.ToInt32(drMaterielStockIn["Type"]) ||
                    7 == Convert.ToInt32(drMaterielStockIn["Type"]) ||
                    8 == Convert.ToInt32(drMaterielStockIn["Type"]))
                {
                    #region 物料異動
                    msmModel = new MATERIEL_STOCKS_MANAGE();
                    msmModel.Number = Convert.ToInt32(drMaterielStockIn["Number"]);
                    msmModel.Perso_Factory_RID = Convert.ToInt32(dtblFactory.Rows[0]["Factory_RID"]);
                    msmModel.Stock_Date = Convert.ToDateTime(drMaterielStockIn["Stock_Date"]);
                    msmModel.Type = Convert.ToString(drMaterielStockIn["Type"]);
                    if (drMaterielStockIn["System_Num"].ToString() != "")
                    {
                        msmModel.Invenroty_Remain = Convert.ToInt32(drMaterielStockIn["System_Num"]);
                    }
                    else
                    {
                        msmModel.Invenroty_Remain = 0;
                    }
                    msmModel.Comment = Convert.ToString(drMaterielStockIn["Comment"]);
                    msmModel.Serial_Number = Convert.ToString(drMaterielStockIn["Serial_Number"]);

                    //add by Ian Huang start
                    //  先刪除日結寫入的 進貨、退貨、銷毀 信息
                    if (1 == Convert.ToInt32(drMaterielStockIn["Type"]) ||
                        2 == Convert.ToInt32(drMaterielStockIn["Type"]) ||
                        3 == Convert.ToInt32(drMaterielStockIn["Type"]))
                    {
                        bl.DeleteSTOCKSMANAGE(Convert.ToDateTime(drMaterielStockIn["Stock_Date"]).ToString("yyyy/MM/dd 00:00:00"), CIMSClass.GlobalString.RCU.ACTIVED, Convert.ToInt32(dtblFactory.Rows[0]["Factory_RID"]), Convert.ToString(drMaterielStockIn["Type"]), Convert.ToString(drMaterielStockIn["Serial_Number"]));
                    }
                    //add by Ian Huang end

                    bl.Add(msmModel);
                    #endregion 物料異動
                }
                // 添加物料盤整訊息
                else if (4 == Convert.ToInt32(drMaterielStockIn["Type"]))
                {
                    int intWearRate = 0;

                    // add by Ian Huang start
                    decimal decWearRate = 0;
                    // add by Ian Huang end

                    #region 如果庫存結餘>廠商結餘


                    //抓取這個代號的最後一筆
                    int TypeNo = 0;
                    String Serial_Number_temp = "";
                    for (int a = intRow; a < dtMaterielStockIn.Rows.Count; a++)
                    {
                        if (Convert.ToString(dtMaterielStockIn.Rows[a][3]).Equals("10"))
                        {
                            TypeNo = a-1;
                            a = dtMaterielStockIn.Rows.Count;
                        }
                    }
        //            if (dtMaterielStockIn.Rows[intRow + 1]["System_Num"].ToString() != "" && drMaterielStockIn["Number"].ToString() != "")

                    if (dtMaterielStockIn.Rows[TypeNo + 1]["System_Num"].ToString() != "" && drMaterielStockIn["Number"].ToString() != "")
                    {
                        if (Convert.ToInt32(dtMaterielStockIn.Rows[TypeNo + 1]["System_Num"]) >
                            Convert.ToInt32(drMaterielStockIn["Number"]))
                        {
                            string strMessage = Convert.ToDateTime(drMaterielStockIn["Stock_Date"]).ToString("yyyy/MM/dd ") +
                                                            Convert.ToString(dtblFactory.Rows[0]["Factory_Name"]) + " " +
                                                            Materiel_Name +
                                                            "的庫存結餘大與廠商結餘";

                            string strMessageWearRate = "";

                            // 損耗率計算方法：最近一次結餘數量 + 進貨 - 退貨 - 銷毀 + 移入 - 移出 - 耗用（包括損耗）= 廠商結餘數量
                            // 耗用（包括損耗）= 最近一次結餘數量 - 廠商結餘數量 + 進貨 - 退貨 - 銷毀 + 移入 - 移出；
                            // 耗用（包括損耗） = 小計檔關聯的使用物品數量 * (1+實際損耗率)
                            // (老算法)實際損耗率 = 耗用（包括損耗）/ 小計檔關聯的使用物品數量 - 1;
                            // 實際損耗率(2010/10/28日修改) = ( (上次廠商結餘-本次廠商結餘)/(耗用（包括損耗）+抽驗+退件重寄+電訊單異動)  )-1  

                            // 小計檔關聯的使用物品數量
                            int intUsedCount = 0;
                            // 最近一次廠商結余+進貨-退貨-銷毀+移入-移出-物料消耗=本次廠商結余（匯入為4的庫存數量）
                            int intUsedCountWear = 0;
                            if (drMaterielStockIn["Last_Surplus_Num"].ToString() != "" && drMaterielStockIn["Last_Surplus_Num"].ToString() != "0")
                            {
                                intUsedCountWear = Convert.ToInt32(drMaterielStockIn["Last_Surplus_Num"])
                                                - Convert.ToInt32(drMaterielStockIn["Number"]);
                            }
                            else
                            {
                                intUsedCountWear = Convert.ToInt32(drMaterielStockIn["Number"]);
                            }


                            DataRow[] drRate = dtMaterielStockIn.Select("Stock_Date <= #" + String.Format("{0:s}", Convert.ToDateTime(drMaterielStockIn["Stock_Date"])) + "#");

                            //edit by Ian Huang start
                            foreach (DataRow dr in drRate)
                            {
                                if (Convert.ToString(drMaterielStockIn["Serial_Number"]) == Convert.ToString(dr["Serial_Number"]))
                                // && Convert.ToDateTime (drMaterielStockIn["Stock_Date"]) > Convert.ToDateTime (dr["Stock_Date "]) )
                                {
                                    if (56 == Convert.ToInt32(dr["Type"]))
                                        intUsedCount += Convert.ToInt32(dr["Number"]);
                                    else if (1 == Convert.ToInt32(dr["Type"]) || 57 == Convert.ToInt32(dr["Type"]))
                                        intUsedCountWear += Convert.ToInt32(dr["Number"]);// + 進貨 + 移入
                                    else if (2 == Convert.ToInt32(dr["Type"]) || 3 == Convert.ToInt32(dr["Type"]) || 58 == Convert.ToInt32(dr["Type"]))
                                        intUsedCountWear -= Convert.ToInt32(dr["Number"]);// - 退貨 - 銷毀 - 移出
                                    else if (6 == Convert.ToInt32(dr["Type"]) || 7 == Convert.ToInt32(dr["Type"]) || 8 == Convert.ToInt32(dr["Type"]))
                                        intUsedCount += Convert.ToInt32(dr["Number"]);// 耗用（包括損耗）+抽驗+退件重寄+電訊單異動
                                }
                            }
                            //edit by Ian Huang end
                            if (intUsedCountWear > 0 && intUsedCount >= 0)
                            {
                                // edit by Ian Huang start
                                if (intUsedCount == 0)
                                {
                                    intWearRate = 99;
                                    decWearRate = 99;
                                }
                                else
                                {
                                    // 損耗率 = 耗用（包括損耗）/ 小計檔關聯的使用物品數量 - 1;
                                    intWearRate = Convert.ToInt32((Convert.ToDecimal(intUsedCountWear) / (Convert.ToDecimal(intUsedCount)) - 1) * 100);
                                    decWearRate = Math.Round((Convert.ToDecimal(intUsedCountWear) / (Convert.ToDecimal(intUsedCount)) - 1) * 100, 2, MidpointRounding.AwayFromZero);
                                }

                                //strMessageWearRate = ";損耗率為：" + intWearRate.ToString() + "%;";
                                strMessageWearRate = ";損耗率為：" + decWearRate.ToString() + "%;";
                                // edit by Ian Huang end

                                string[] arg = new string[3];
                                arg[0] = dtblFactory.Rows[0]["Factory_Name"].ToString();
                                arg[1] = bl.getMateriel_Name(dtMaterielStockIn.Rows[intRow]["Serial_Number"].ToString());
                                // edit by Ian Huang start
                                //arg[2] = intWearRate.ToString();
                                arg[2] = decWearRate.ToString();
                                // edit by Ian Huang end
                                //200911CR-廠商耗損率高於基本檔設定的耗損率，即發耗損率警訊 Edit By Yangkun 2009/11/25 start
                                if (int.Parse(bl.GetWearRate(dtMaterielStockIn.Rows[intRow]["Serial_Number"].ToString()).ToString()) < intWearRate)
                                {
                                    Warning.SetWarning(GlobalString.WarningType.MaterialDataIn, arg);
                                }
                                //200911CR-廠商耗損率高於基本檔設定的耗損率，即發耗損率警訊 Edit By Yangkun 2009/11/25 end
                                strWearRate = intWearRate.ToString();
                            }
                            else
                            {
                                strMessageWearRate = ";";
                            }

                            ShowMessage(strMessage + strMessageWearRate);
                        }
                    }
                    #endregion 如果庫存結餘>廠商結餘

                    #region 庫存數量報警
                    if (bl.DmNotSafe_Type(drMaterielStockIn["Serial_Number"].ToString()))
                    {
                        DataSet dtMateriel = bl.GetMateriel(Convert.ToString(drMaterielStockIn["Serial_Number"]));
                        if (null != dtMateriel &&
                            dtMateriel.Tables.Count > 0 &&
                            dtMateriel.Tables[0].Rows.Count > 0)
                        {
                            // 最低安全庫存
                            if (GlobalString.SafeType.storage == Convert.ToString(dtMateriel.Tables[0].Rows[0]["Safe_Type"]))
                            {
                                // 廠商結餘低於最低安全庫存數值時
                                if (Convert.ToInt32(drMaterielStockIn["Number"]) <
                                    Convert.ToInt32(dtMateriel.Tables[0].Rows[0]["Safe_Number"]))
                                {
                                    string[] arg = new string[1];
                                    arg[0] = dtMateriel.Tables[0].Rows[0]["Name"].ToString();
                                    Warning.SetWarning(GlobalString.WarningType.MaterialDataInSafe, arg);

                                    ShowMessage(Convert.ToString(dtblFactory.Rows[0]["Factory_Name"]) + "PERSO厰的" +
                                                Materiel_Name + "紙品物料安全庫存量不足！");
                                }
                                // 安全天數
                            }
                            else if (GlobalString.SafeType.days == Convert.ToString(dtMateriel.Tables[0].Rows[0]["Safe_Type"]))
                            {
                                // 檢查庫存是否充足
                                //200908CR ADD BY 楊昆 2009/09/18 start
                                InOut000BL BL000 = new InOut000BL();
                                //bl.CheckMaterielSafeDays
                                if (!BL000.CheckMaterielSafeDays(dtMateriel.Tables[0].Rows[0]["Serial_Number"].ToString(),
                                    //200908CR ADD BY 楊昆 2009/09/18 end
                                                        Convert.ToInt32(dtblFactory.Rows[0]["Factory_RID"]),
                                                        Convert.ToInt32(dtMateriel.Tables[0].Rows[0]["Safe_Number"]),
                                                        Convert.ToInt32(drMaterielStockIn["Number"])))
                                {
                                    string[] arg = new string[1];
                                    arg[0] = dtMateriel.Tables[0].Rows[0]["Name"].ToString();
                                    Warning.SetWarning(GlobalString.WarningType.MaterialDataInSafe, arg);

                                    ShowMessage(Convert.ToString(dtblFactory.Rows[0]["Factory_Name"]) + "PERSO厰的" +
                                                Materiel_Name + "紙品物料安全庫存量不足！");
                                }

                            }
                        }
                    }
                    #endregion 庫存數量報警

                    #region 物料盤整
                    msmModel = new MATERIEL_STOCKS_MANAGE();
                    msmModel.Number = Convert.ToInt32(drMaterielStockIn["Number"]);
                    msmModel.Perso_Factory_RID = Convert.ToInt32(dtblFactory.Rows[0]["Factory_RID"]);
                    msmModel.Stock_Date = Convert.ToDateTime(drMaterielStockIn["Stock_Date"]);
                    msmModel.Type = "4";
                    //if (drMaterielStockIn["System_Num"].ToString() != "")
                    //{
                    //    msmModel.Invenroty_Remain = Convert.ToInt32(drMaterielStockIn["System_Num"]);
                    //}
                    //else
                    //{
                    //    msmModel.Invenroty_Remain = Convert.ToInt32(drMaterielStockIn["Number"]);
                    //}

                    //將系統結余數保存到廠商結余的記錄的另一個欄位上！
                    //if (dtMaterielStockIn.Rows[intRow + 1]["System_Num"].ToString() != "")
                    //{
                    //    msmModel.Invenroty_Remain = Convert.ToInt32(dtMaterielStockIn.Rows[intRow + 1]["System_Num"].ToString());
                    //}
                    //else
                    //{
                    //    msmModel.Invenroty_Remain = 0;
                    //}

                    if (dtMaterielStockIn.Rows[TypeNo + 1]["System_Num"].ToString() != "")
                    {
                        msmModel.Invenroty_Remain = Convert.ToInt32(dtMaterielStockIn.Rows[TypeNo + 1]["System_Num"].ToString());
                    }
                    else
                    {
                        msmModel.Invenroty_Remain = 0;
                    }

                    msmModel.Comment = Convert.ToString(drMaterielStockIn["Comment"]);
                    msmModel.Serial_Number = Convert.ToString(drMaterielStockIn["Serial_Number"]);
                    //if (intWearRate < 0)
                    //{
                    //    msmModel.Real_Wear_Rate = -intWearRate;
                    //}
                    //else
                    //{
                    //edti by Ian Huang start
                    //msmModel.Real_Wear_Rate = intWearRate;
                    msmModel.Real_Wear_Rate = decWearRate;
                    //edti by Ian Huang end
                    //}
                    bl.Add(msmModel);
                    #endregion 物料盤整
                }
                // 添加物料每天結余訊息
                else if (10 == Convert.ToInt32(drMaterielStockIn["Type"]))
                {
                    if (arrWorkDate.Contains(Convert.ToDateTime(drMaterielStockIn["Stock_Date"]).ToString("yyyy/MM/dd")))
                    {
                        #region 物料每天結餘
                        MATERIEL_STOCKS msModel = new MATERIEL_STOCKS();
                        msModel.Number = Convert.ToInt32(drMaterielStockIn["System_Num"]);
                        msModel.Perso_Factory_RID = Convert.ToInt32(dtblFactory.Rows[0]["Factory_RID"]);
                        msModel.Stock_Date = Convert.ToDateTime(drMaterielStockIn["Stock_Date"]);
                        msModel.Serial_Number = Convert.ToString(drMaterielStockIn["Serial_Number"]);
                        bl.Addms(msModel);
                        #endregion 物料每天結餘

                        msmModel = bl.getMSMModel(msModel);
                        if (msmModel != null)
                        {
                            //判斷當前是否為物品結餘時間(type=4)
                            //if (msmModel.Stock_Date == msModel.Stock_Date)
                            //{
                            //更新物料庫存管理信息的系統結餘
                            msmModel.Invenroty_Remain = msModel.Number;
                            bl.dao.Update<MATERIEL_STOCKS_MANAGE>(msmModel, "RID");
                            //}
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ViewState["MaterielStocksIn"] = null;
        ViewState["MaterielStocksInNum"] = 0;

        this.gvpbPersoStockIn.DataSource = null;
        this.gvpbPersoStockIn.DataBind();
        // add by Ian Huang start
        bl.delMove(dropFactoryIn.SelectedValue, "", DateTime.Now.ToString("yyyy/MM/dd"));
        // add by Ian Huang end

    }
}
