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
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CIMSClass.Business;

public partial class InOut_InOut001 : PageBase
{
    InOut001BL BL = new InOut001BL();
     //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
    InOut000BL BL000 = new InOut000BL();
     //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end

    #region 事件處理
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "loadRadio();", true);
        if (!IsPostBack)
        {
            try
            {
                //預設為當前系統日期
                this.txtBegin_Date1.Text = DateTime.Now.ToString("yyyy/MM/dd");
                this.txtBegin_Date2.Text = DateTime.Now.ToString("yyyy/MM/dd");
                this.txtBegin_Date4.Text = DateTime.Now.ToString("yyyy/MM/dd");
                this.txtBegin_Date3.Text = DateTime.Now.ToString("yyyy/MM/dd");
                // 獲取批次
                DataSet dsMAKE_CARD_TYPE = BL.GetBatchInfo();
                dropMakeCardType.DataValueField = "RID";
                dropMakeCardType.DataTextField = "Group_Type_Name";
                dropMakeCardType.DataSource = dsMAKE_CARD_TYPE.Tables[0];
                dropMakeCardType.DataBind();

                dropMakeCardTypeDel.DataValueField = "RID";
                dropMakeCardTypeDel.DataTextField = "Group_Type_Name";
                dropMakeCardTypeDel.DataSource = dsMAKE_CARD_TYPE.Tables[0];
                dropMakeCardTypeDel.DataBind();
                dropMakeCardTypeDel.Items.Insert(0, new ListItem("全部", ""));

                // 綁定“刪除已匯入資料”小計檔名
                GetFileName();
                // 綁定“匯入伺服器”小計檔名
                Get_IMPORT_PROJECT_FileName();
            }
            catch (Exception ex)
            {
                ShowMessage(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
            }
        }
    }

    /// <summary>
    /// 匯入本機資料 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnImportUp_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_IsCheck);
            return;
        }

        string Factory_RID = "";// 廠商RID
        String DateImp = this.txtBegin_Date4.Text;// 匯入日期
        string MakeCardTypeRID = this.dropMakeCardType.SelectedValue.ToString();// 製卡類別

        try
        {
            if (txtBegin_Date4.Text == "")
            {
                throw new Exception("匯入日期不能為空");
            }
            //檢查日結
            BL.Is_Check(Convert.ToDateTime(txtBegin_Date4.Text));
            if (FileUpd.FileName.ToString() == "")
            {
                throw new Exception("請選要上傳的文件！");
            }

            if (StringUtil.IsEmpty(MakeCardTypeRID))
            {
                throw new Exception("請先選擇批次！");
            }

            // 是否已經匯入過
            BL.Exists_File(FileUpd.FileName);

            // 廠商檢查
            int FileNameLen = FileUpd.FileName.LastIndexOf('-');
            string Factory_ShortName_EN = "";
            if (FileNameLen>0)
            {
                Factory_ShortName_EN = FileUpd.FileName.Substring(FileNameLen + 1, FileUpd.FileName.Length - FileNameLen - 5);
            }else
            {
                throw new Exception("要匯入的小計檔文件名格式不正確。");
            }
            Factory_RID = BL.GetFactory_RID(Factory_ShortName_EN);
            if (Factory_RID=="")
            {
                throw new Exception("上傳文件的Perso廠英文簡稱對應的Perso廠不存在。");
            }

            // 檢查同名文件是否存在
            string SubDir = this.txtBegin_Date4.Text.Replace("/", "");
            string FilePath = ConfigurationManager.AppSettings["SubTotalFilesPath"].ToString();
            if (File.Exists(FilePath + SubDir + "\\" + FileUpd.FileName))
            {
                // 先刪除該文件
                File.Delete(FilePath + SubDir + "\\" + FileUpd.FileName);
            }
            // 再上傳該文件
            string strPath = FileUpload(FileUpd.PostedFile);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
            return;
        }

        bool blImportSubTotalStart = false;

        // 將小計檔內容存入資料庫
        try
        {
            string TempDate = txtBegin_Date4.Text.Replace("/", "");
            string path = ConfigurationManager.AppSettings["SubTotalFilesPath"].ToString() + TempDate + "\\";

            // 取所有卡種訊息
            DataTable dtCardType = BL.getCardType();
            // 檢查文件格式是否正確
            List<string> lstError = new List<string>();
            DataTable dtImportCardType = BL.ImportCheck(dropMakeCardType.SelectedValue,path,
                    FileUpd.FileName,
                    Convert.ToDateTime(DateImp),
                    dtCardType,
                    lstError);

            // 匯入文件有不正確
            if (lstError.Count > 0)
            {
                // 保存匯入正確格式資訊
                this.ViewState["ImportSubTotal"] = dtImportCardType;
                this.ViewState["ImportSubTotal_dtCardType"] = dtCardType;
                this.ViewState["ImportSubTotal_Factory_RID"] = Factory_RID;
                this.ViewState["ImportSubTotal_FileName"] = FileUpd.FileName;
                this.ViewState["ImportSubTotal_MakeCardTypeRID"] = this.dropMakeCardType.SelectedValue.ToString();
                this.ViewState["ImportSubTotal_ImportDate"] = this.txtBegin_Date4.Text;

                string strError = "";
                foreach (string str1 in lstError)
                {
                    strError += "\\n" + str1;
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=confirm('匯入文檔有格式有下列不正確，要匯入正確的小計檔資訊嗎？" + strError + "');if(aa==true){ImtBind();}", true);
            }
            else
            {
                if (!BL.ImportSubTotalStart())
                {
                    ShowMessage("小計檔匯入已經開始，不能重復開始！");
                    return;
                }
                blImportSubTotalStart = true;

                // 添加小計檔
                BL.AddImp(dtImportCardType,
                    DateImp,
                    FileUpd.FileName,
                    MakeCardTypeRID,
                    Factory_RID,
                    dtCardType);

                // 根據小計檔，生成物料耗用記錄，并判斷物料的庫存是否需要報警 weilinzhan@wistronits.com 2009/03/18 start
               // BL.Material_Used_Warnning(Factory_RID, DateTime.Parse(DateImp), FileUpd.FileName);
                // 根據小計檔，生成物料耗用記錄，并判斷物料的庫存是否需要報警 weilinzhan@wistronits.com 2009/03/18 end
                //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
                 BL000.Material_Used_Warnning(Factory_RID, DateTime.Parse(DateImp),"1");
                //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end

                BL.ImportSubTotalEnd();

                // 刪除匯入文件部分，可刪除的文件處理
                if (this.txtBegin_Date1.Text == this.txtBegin_Date3.Text)
                {
                    this.dropImport_FileNameDel.Items.Add(FileUpd.FileName);
                }

                // 添加記錄成功
                ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
            }
        }
        catch (Exception ex)
        {
            if (blImportSubTotalStart)
                BL.ImportSubTotalEnd();
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    ///"刪除已匯入資料"獲取小計檔名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBegin_Date1_TextChanged(object sender, EventArgs e)
    {
        GetFileName();
    }
    /// <summary>
    ///"刪除已匯入資料"獲取小計檔名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void dropMakeCardTypeDel_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetFileName();
    }

    /// <summary>
    ///刪除已匯入資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelImport_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_IsCheck);
            return;
        }

        bool blImportSubTotalStart = false;

        try
        {
            DateTime date = Convert.ToDateTime(txtBegin_Date1.Text);

            BL.Is_Check(Convert.ToDateTime(txtBegin_Date1.Text));

            if (!BL.ImportSubTotalStart())
            {
                ShowMessage("小計檔匯入已經開始，不能重復開始！");
                return;
            }
            blImportSubTotalStart = true;

            // 刪除小計檔案
            int iReturn=BL.Delete(txtBegin_Date1.Text,
                    this.dropMakeCardTypeDel.SelectedValue.ToString(),
                    this.dropImport_FileNameDel.SelectedItem.Text.Trim());
            if (iReturn > 0)
            {

                // 綁定“刪除已匯入資料”小計檔名
                GetFileName();

                ShowMessage(BizMessage.BizCommMsg.ALT_CMN_DeleteSucc);
            }
            else if (iReturn < 0)
                ShowMessage("匯入日期已日結,無法刪除");
            else
                ShowMessage("無符合資料刪除");

            BL.ImportSubTotalEnd();
        }
        catch (Exception ex)
        {
            if (blImportSubTotalStart)
                BL.ImportSubTotalEnd();
            ShowMessage(ex.Message);
        }
    }

    /// <summary>
    /// 根據文件下傳日期，取小計檔，并添加到匯入文件列表。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtBegin_Date3_TextChanged(object sender, EventArgs e)
    {
        Get_IMPORT_PROJECT_FileName();
    }

    /// <summary>
    /// 開始匯入服務器資料。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnImport_Click(object sender, EventArgs e)
    {
        if (this.IsCheck())
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_IsCheck);
            return;
        }
        if (StringUtil.IsEmpty(dropImport_FileName.SelectedValue))
        {
            ShowMessage("請選擇檔名");
            return;
        }

        bool blImportSubTotalStart = false;

        string filename = dropImport_FileName.SelectedItem.Text;
        if (filename.Length > 6)
        {
            //取到文件名稱
            string tempFileName = filename.Substring(0, filename.Length - 4);

            int FileNameLen = tempFileName.LastIndexOf('-');
            //取到廠商英文簡稱
            string Factory_ShortName_EN = tempFileName.Substring(FileNameLen + 1, tempFileName.Length - (FileNameLen + 1));

            //取到小計檔名稱
            string strFile_Name = filename.Substring(8, FileNameLen-8);

            try
            {
                if (txtBegin_Date2.Text == "")
                {
                    throw new Exception("匯入日期不能為空");
                }
                BL.Is_Check(Convert.ToDateTime(txtBegin_Date2.Text));

                // 是否已經匯入過
                BL.Exists_File(filename);

                string TempDate = txtBegin_Date2.Text.Replace("/", "");
                string path = ConfigurationManager.AppSettings["SubTotalFilesPath"].ToString() + TempDate + "\\";

                // 取Perso廠RID
                string Factory_RID = BL.GetFactory_RID(Factory_ShortName_EN);
                // 取製卡種類RID
                string MakeCardTypeRID = BL.getMakeCardTypeRID(strFile_Name);
                // 取所有卡種訊息
                DataTable dtCardType = BL.getCardType();
                // 檢查文件格式是否正確
                List<string> lstError = new List<string>();
                DataTable dtImportCardType = BL.ImportCheck(dropMakeCardType.SelectedValue,path,
                        filename,
                        Convert.ToDateTime(this.txtBegin_Date2.Text),
                        dtCardType,
                        lstError);

                // 匯入文件有不正確
                if (lstError.Count > 0)
                {
                    // 保存匯入正確格式資訊
                    this.ViewState["ImportSubTotal"] = dtImportCardType;
                    this.ViewState["ImportSubTotal_dtCardType"] = dtCardType;
                    this.ViewState["ImportSubTotal_Factory_RID"] = Factory_RID;
                    this.ViewState["ImportSubTotal_FileName"] = filename;
                    this.ViewState["ImportSubTotal_MakeCardTypeRID"] = MakeCardTypeRID;
                    this.ViewState["ImportSubTotal_ImportDate"] = this.txtBegin_Date2.Text;
                    string strError = "";
                    foreach (string str1 in lstError)
                    {
                        strError += "\\n" + str1;
                    }

                    object[] arg = new object[2];
                    arg[0] = filename;
                    arg[1] = strError;
                    Warning.SetWarning(GlobalString.WarningType.SubTotalDataIn,arg);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), "var aa=confirm('匯入文檔有格式有下列不正確，要匯入正確的小計檔資訊嗎？" + strError + "');if(aa==true){ImtBind();}", true);
                }
                else
                {
                    if (!BL.ImportSubTotalStart())
                    {
                        ShowMessage("小計檔匯入已經開始，不能重復開始！");
                        return;
                    }
                    blImportSubTotalStart = true;

                    // 添加小計檔
                    BL.AddImp(dtImportCardType,
                        this.txtBegin_Date2.Text,
                        filename,
                        MakeCardTypeRID,
                        Factory_RID,
                        dtCardType);

                    // 根據小計檔，生成物料耗用記錄，并判斷物料的庫存是否需要報警 weilinzhan@wistronits.com 2009/03/18 start
                    //BL.Material_Used_Warnning(Factory_RID, DateTime.Parse(this.txtBegin_Date2.Text), filename);
                    // 根據小計檔，生成物料耗用記錄，并判斷物料的庫存是否需要報警 weilinzhan@wistronits.com 2009/03/18 end
                    //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
                    BL000.Material_Used_Warnning(Factory_RID, DateTime.Parse(this.txtBegin_Date2.Text), "1");
                    //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end

                    BL.ImportSubTotalEnd();

                    // 刪除匯入文件部分，可刪除的文件處理
                    if (this.txtBegin_Date1.Text == this.txtBegin_Date2.Text)
                    {
                        this.dropImport_FileNameDel.Items.Add(filename);
                    }

                    // 添加記錄成功
                    ShowMessage(BizMessage.BizCommMsg.ALT_CMN_SaveSucc);
                }
            }
            catch (Exception ex)
            {
                if (blImportSubTotalStart)
                    BL.ImportSubTotalEnd();
                ShowMessage(ex.Message);
            }
        }
        else {
            ShowMessage("請選擇正確的小計檔。");
        }
    }
    #endregion

    /// <summary>
    /// 文件上傳功能
    /// </summary>
    /// <param name="file">上傳文件</param>
    protected new string FileUpload(HttpPostedFile file)
    {
        string path = "";
        if (IsFolderExist())
        {
            string tmpname = file.FileName;
            int i = tmpname.LastIndexOf("\\");
            string filename = tmpname.Substring(i + 1);
            string filetype = filename.Substring(filename.LastIndexOf(".") + 1);
            string date = txtBegin_Date4.Text.Replace("/", "");
            if (filetype.ToLower().Equals("txt"))
            {
                try
                {
                    float a = file.ContentLength / (float)1024.0;
                    if (a <= 10000.00)
                    {
                        file.SaveAs(ConfigurationManager.AppSettings["SubTotalFilesPath"].ToString() + date + "\\" + FileUpd.FileName);
                        path = ConfigurationManager.AppSettings["SubTotalFilesPath"].ToString() + date + "\\" + FileUpd.FileName;
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
                ShowMessage("文件格式必須為txt");
            }
        }
        return path;
    }

    /// <summary>
    /// 建立上傳目錄
    /// </summary>
    /// <returns>true：成功 false：失敗</returns>
    public new bool IsFolderExist()
    {
        string basepath = ConfigurationManager.AppSettings["SubTotalFilesPath"].ToString();
        string date = txtBegin_Date4.Text.Replace("/", "");
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
    ///"刪除已匯入資料"獲取小計檔名稱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GetFileName()
    {
        try
        {
            string dropMakeCardTypeDel_Value = "";
            DataSet dsgetFilesByDayBatch = null;
            if (dropMakeCardTypeDel.SelectedValue.ToString() == "")
            {
                // 以匯入日期取小計檔
                dsgetFilesByDayBatch = BL.getFilesByDayBatch(txtBegin_Date1.Text);
            }
            else
            {
                // 以匯入日期、批次RID取小計檔
                dropMakeCardTypeDel_Value = dropMakeCardTypeDel.SelectedItem.Value;
                dsgetFilesByDayBatch = BL.getFilesByDayBatch(txtBegin_Date1.Text, dropMakeCardTypeDel_Value);
            }

            // 獲取小計檔名
            dropImport_FileNameDel.DataTextField = "Import_FileName";
            dropImport_FileNameDel.DataSource = dsgetFilesByDayBatch.Tables[0];
            dropImport_FileNameDel.DataBind();
            dropImport_FileNameDel.Items.Insert(0, new ListItem(GlobalStringManager.Default["Info_All"], ""));
        }
        catch (Exception ex)
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 綁定“匯入伺服器”小計檔名
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Get_IMPORT_PROJECT_FileName()
    {
        try
        {
            string basepath = ConfigurationManager.AppSettings["SubTotalFilesPath"].ToString();
            string date = txtBegin_Date3.Text.Replace("/", "");
            int Path_Len = basepath.Length + date.Length + 1;
            int i = 0;
            string Pattern = "";
            dropImport_FileName.Items.Clear();
            MatchCollection Matches;

            // 小計檔目錄存在檢查
            if (Directory.Exists(basepath + date))
            {
                string[] file = Directory.GetFiles(basepath + date);
                foreach (string files in file)
                {
                    if (files.Length > 6)
                    {
                        //取到文件名稱
                        string tempFileName = files.Substring(Path_Len, files.Length - (Path_Len + 4));

                        int FileNameLen = tempFileName.LastIndexOf('-');
                        if (FileNameLen < 0)
                        {
                            continue;
                        }


                        //取到廠商英文簡稱
                        string Factory_ShortName_EN = tempFileName.Substring(FileNameLen + 1, tempFileName.Length - (FileNameLen + 1));

                        Pattern = @"^\d{8}$";
                        Matches = Regex.Matches(tempFileName.Substring(0, 8), Pattern);
                        if (Matches.Count != 0)
                        {
                            int year = Convert.ToInt32(tempFileName.Substring(0, 4));
                            int MM = Convert.ToInt32(tempFileName.Substring(4, 2));
                            int DD = Convert.ToInt32(tempFileName.Substring(6, 2));
                            string File_Name = tempFileName.Substring(8, FileNameLen - 8);
                            if (MM <= 12 && MM >= 1 && DD >= 1 && DD <= 31)
                            {
                                if (BL.getFilesByDownloadDay(File_Name, Factory_ShortName_EN))
                                {
                                    dropImport_FileName.Items.Insert(i, files.Substring(Path_Len, files.Length - Path_Len));
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
    }

    /// <summary>
    /// 當小計檔內容格式有不正確時，選擇匯入正確內容時，添加正確記錄到資料庫
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        DataTable dtblImportCardType = null;

        bool blImportSubTotalStart = false;

        try
        {
            // 取保存的小計檔訊息。
            dtblImportCardType = (DataTable)this.ViewState["ImportSubTotal"];

            if (dtblImportCardType.Rows.Count == 0)
            {
                ShowMessage("無匯入信息");
                return;
            }

            if (dtblImportCardType != null && dtblImportCardType.Rows.Count > 0)
            {
                if (!BL.ImportSubTotalStart())
                {
                    ShowMessage("小計檔匯入已經開始，不能重復開始！");
                    return;
                }
                blImportSubTotalStart = true;

                DataTable dtCardType = (DataTable)this.ViewState["ImportSubTotal_dtCardType"];
                string Factory_RID = this.ViewState["ImportSubTotal_Factory_RID"].ToString();
                string File_Name = this.ViewState["ImportSubTotal_FileName"].ToString();
                string MakeCardTypeRID = this.ViewState["ImportSubTotal_MakeCardTypeRID"].ToString();
                string ImportDate = this.ViewState["ImportSubTotal_ImportDate"].ToString();
                // 添加小計檔
                BL.AddImp(dtblImportCardType,
                    ImportDate,
                    File_Name,
                    MakeCardTypeRID,
                    Factory_RID,
                    dtCardType);

                // 根據小計檔，生成物料耗用記錄，并判斷物料的庫存是否需要報警 weilinzhan@wistronits.com 2009/03/18 start
                //BL.Material_Used_Warnning(Factory_RID, DateTime.Parse(ImportDate), File_Name);
                // 根據小計檔，生成物料耗用記錄，并判斷物料的庫存是否需要報警 weilinzhan@wistronits.com 2009/03/18 end
                //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 start
                BL000.Material_Used_Warnning(Factory_RID, DateTime.Parse(ImportDate), "1");
                //200908CR物料的消耗計算改為用小計檔的「替換前」版面計算 ADD BY 楊昆 2009/08/31 end

                BL.ImportSubTotalEnd();

                // 刪除匯入文件部分，可刪除的文件處理
                if (this.txtBegin_Date1.Text == this.txtBegin_Date2.Text)
                {
                    this.dropImport_FileNameDel.Items.Add(File_Name);
                }

                // 添加記錄成功
                ShowMessage("成功匯入" + dtblImportCardType.Rows.Count+"條");
            }
        }
        catch (Exception ex)
        {
            if (blImportSubTotalStart)
                BL.ImportSubTotalEnd();
            ShowMessage(ex.Message);
        }
    }
    protected void txtBegin_Date2_TextChanged(object sender, EventArgs e)
    {

    }
}
