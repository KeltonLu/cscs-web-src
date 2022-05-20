using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Depository_Depository002Mod : PageBase
{
    Depository002BL depManager = new Depository002BL();

    BaseInfo003BL facManager = new BaseInfo003BL();

    DataSet ds = new DataSet();

    DataTable dt = new DataTable();

    DataTable dtclone = new DataTable();

    Dictionary<string, object> inputs = new Dictionary<string, object>();

    LoginManager lm = new LoginManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        dt.Columns.Clear();
        dt.Columns.Add("Pass_Status");
        dt.Columns.Add("orderform_rid");
        dt.Columns.Add("orderform_detail_rid");
        dt.Columns.Add("Space_Short_RID");//cardtype_rid
        dt.Columns.Add("number");
        dt.Columns.Add("Agreement_RID");
        dt.Columns.Add("Agreement_Name");
        dt.Columns.Add("Budget_RID");
        dt.Columns.Add("Budget_Name");
        dt.Columns.Add("factory");
        dt.Columns.Add("factory_id");
        dt.Columns.Add("Base_Price");
        dt.Columns.Add("Fore_Delivery_Date");
        dt.Columns.Add("Wafer_RID");
        dt.Columns.Add("Wafer_Name");
        dt.Columns.Add("Wafer_Capacity");
        dt.Columns.Add("is_exigence");
        dt.Columns.Add("Delivery_Address_RID");
        dt.Columns.Add("factory_shortname_cn");
        dt.Columns.Add("comment");
        dt.Columns.Add("bz");
        //add chaoma start
        dt.Columns.Add("Change_UnitPrice");
        //add chaoma end

        this.gvpbORDERFORMDETAIL.PageSize = GlobalStringManager.PageSize;
        gvpbORDERFORMDETAIL.NoneData = "";

        if (!IsPostBack)
        {
            Session.Remove("detail");
            Session.Remove("delRID");
            Session.Remove("delBlank");

            string strOrderFormRID = Request.QueryString["RID"];

            //有值，代表是修改！
            if (!StringUtil.IsEmpty(strOrderFormRID))
            {
                try
                {
                    ds = depManager.GetOrderFormDetail_RID(strOrderFormRID);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            int res = Convert.ToInt32(ds.Tables[0].Rows[i]["pass_status"].ToString());
                            if (CanUseAction(GlobalString.UserRoleConfig.Commit) && CanUseAction(GlobalString.UserRoleConfig.Add) && CanUseAction(GlobalString.UserRoleConfig.Edit))//經辦登陸（提交、新增和修改權限）
                            {
                                lblTitle.Text = "採購下單作業修改/刪除";

                                if (res == 1 || res == 2)//放行狀態為：退回或暫存
                                {
                                    btnCommit.Visible = true;
                                    btnExcel.Visible = true;
                                    btnCommitD.Visible = true;
                                    btnExcelD.Visible = true;
                                    btnEdit.Visible = true;
                                    btnEditD.Visible = true;
                                    btnPass.Visible = false;
                                    btnPassD.Visible = false;
                                    btnReturn.Visible = false;
                                    btnReturnD.Visible = false;

                                    trdel.Visible = true;//刪除checkbox
                                    Button2.Visible = true;//新增訂單詳細button
                                    gvpbORDERFORMDETAIL.Columns[9].Visible = true;
                                }
                                else if (res == 4)
                                {
                                    btnPass.Enabled = false;
                                    btnPassD.Enabled = false;

                                    btnCommit.Enabled = false;
                                    btnExcel.Enabled = true;
                                    btnCommitD.Enabled = false;
                                    btnExcelD.Enabled = true;
                                    btnEdit.Enabled = false;
                                    btnEditD.Enabled = false;

                                    btnReturn.Enabled = true;
                                    btnReturnD.Enabled = true;

                                    btnAddDetail.Enabled = false;
                                    trdel.Disabled = true;//刪除checkbox
                                    Button2.Disabled = true;//新增訂單詳細button
                                }
                                else//待放行
                                {
                                    btnCommit.Enabled = false;
                                    btnExcel.Enabled = true;
                                    btnCommitD.Enabled = false;
                                    btnExcelD.Enabled = true;
                                    btnEdit.Enabled = false;
                                    btnEditD.Enabled = false;
                                    btnPass.Enabled = true;
                                    btnPassD.Enabled = true;
                                    btnReturn.Enabled = true;
                                    btnReturnD.Enabled = true;

                                    trdel.Disabled = true;//刪除checkbox
                                    Button2.Disabled = true;//新增訂單詳細button
                                    gvpbORDERFORMDETAIL.Columns[9].Visible = false;
                                }
                            }
                            else if (CanUseAction(GlobalString.UserRoleConfig.Pass) && CanUseAction(GlobalString.UserRoleConfig.Reject))//主管登陸(放行和退回權限)
                            {
                                lblTitle.Text = "採購下單作業";

                                trdel.Visible = false;//刪除checkbox
                                Button2.Visible = false;//新增訂單詳細button
                                gvpbORDERFORMDETAIL.Columns[9].Visible = false;

                                if (res == 3)//放行狀態為：待放行
                                {
                                    btnPass.Enabled = true;
                                    btnPass.Visible = true;
                                    btnReturn.Visible = true;
                                    btnReturn.Enabled = true;
                                    btnPassD.Enabled = true;
                                    btnPassD.Visible = true;
                                    btnReturnD.Enabled = true;
                                    btnReturnD.Visible = true;
                                }
                                else if (res == 1 || res == 2)//放行狀態為：退回或暫存
                                {
                                    btnPass.Enabled = false;
                                    btnReturn.Enabled = false;
                                    btnPassD.Enabled = false;
                                    btnReturnD.Enabled = false;
                                }
                                else if (res == 4)//放行狀態為：放行
                                {
                                    btnPass.Enabled = false;
                                    btnPassD.Enabled = false;
                                    btnReturn.Visible = true;
                                    btnReturnD.Visible = true;
                                }

                                btnCommit.Visible = false;
                                btnCommitD.Visible = false;
                                btnEdit.Visible = false;
                                btnEditD.Visible = false;
                            }
                            else if (CanUseAction(GlobalString.UserRoleConfig.Delete))
                            {
                                btnEdit.Enabled = true;
                                btnEdit.Visible = true;
                                btnEditD.Enabled = true;
                                btnEditD.Visible = true;

                                lblTitle.Text = "採購下單作業";
                            }
                            else if (CanUseAction(GlobalString.UserRoleConfig.Commit))
                            {
                                btnCommit.Enabled = true;
                                btnCommit.Visible = true;
                                btnCommitD.Enabled = true;
                                btnCommitD.Visible = true;

                                lblTitle.Text = "採購下單作業";
                            }
                            else if (CanUseAction(GlobalString.UserRoleConfig.Pass))
                            {
                                btnPass.Enabled = true;
                                btnPass.Visible = true;
                                btnPassD.Enabled = true;
                                btnPassD.Visible = true;

                                lblTitle.Text = "採購下單作業";
                            }
                            else if (CanUseAction(GlobalString.UserRoleConfig.Reject))
                            {
                                btnReturn.Enabled = true;
                                btnReturn.Visible = true;
                                btnReturnD.Enabled = true;
                                btnReturnD.Visible = true;

                                lblTitle.Text = "採購下單作業";
                            }
                            else if (CanUseAction(GlobalString.UserRoleConfig.Add))
                            {
                                Button2.Disabled = false;
                                Button2.Visible = true;

                                lblTitle.Text = "採購下單作業";
                            }
                        }

                        lblOrderForm_RID.Text = strOrderFormRID;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i]["Blank_Factory_RID"].ToString() != "")
                            {
                                lblFactory_ShortName_CN.Text = facManager.LoadFactoryInfoByRID(ds.Tables[0].Rows[i]["Blank_Factory_RID"].ToString()).Tables[0].Rows[0]["factory_shortname_cn"].ToString();
                                break;
                            }
                        }
                        lblCase_Status.Text = ds.Tables[0].Rows[0]["param_name"].ToString();

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Pass_Status"] = ds.Tables[0].Rows[i]["Pass_Status"].ToString();
                            dr["orderform_detail_rid"] = ds.Tables[0].Rows[i]["orderform_detail_rid"].ToString();
                            dr["orderform_rid"] = ds.Tables[0].Rows[i]["orderform_rid"].ToString();
                            if (ds.Tables[0].Rows[i]["number"].ToString() != "")
                            {
                                dr["number"] = Convert.ToInt32(ds.Tables[0].Rows[i]["number"].ToString());
                            }
                            dr["is_exigence"] = ds.Tables[0].Rows[i]["is_exigence"].ToString();
                            if (ds.Tables[0].Rows[i]["Fore_Delivery_Date"].ToString() != "")
                            {
                                dr["Fore_Delivery_Date"] = Convert.ToDateTime(ds.Tables[0].Rows[i]["Fore_Delivery_Date"]).ToString("yyyy/MM/dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                            }
                            if (ds.Tables[0].Rows[i]["Delivery_Address_RID"].ToString() != "")
                            {
                                dr["Delivery_Address_RID"] = Convert.ToInt32(ds.Tables[0].Rows[i]["Delivery_Address_RID"].ToString());
                                FACTORY fac = new FACTORY();
                                fac = facManager.GetFactory(ds.Tables[0].Rows[i]["Delivery_Address_RID"].ToString());
                                dr["factory_shortname_cn"] = fac.Factory_ShortName_CN;
                            }
                            if (ds.Tables[0].Rows[i]["Agreement_RID"].ToString() != "")
                            {
                                dr["Agreement_RID"] = Convert.ToInt32(ds.Tables[0].Rows[i]["Agreement_RID"].ToString());
                            }
                            if (ds.Tables[0].Rows[i]["Budget_RID"].ToString() != "")
                            {
                                dr["Budget_RID"] = Convert.ToInt32(ds.Tables[0].Rows[i]["Budget_RID"].ToString());
                            }
                            if (ds.Tables[0].Rows[i]["Agreement_Name"].ToString() != "")
                            {
                                dr["Agreement_Name"] = ds.Tables[0].Rows[i]["agreement_name"].ToString();
                            }
                            if (ds.Tables[0].Rows[i]["Budget_Name"].ToString() != "")
                            {
                                dr["Budget_Name"] = ds.Tables[0].Rows[i]["Budget_Name"].ToString();
                            }
                            if (ds.Tables[0].Rows[i]["CardType_RID"].ToString() != "")
                            {
                                dr["Space_Short_RID"] = Convert.ToInt32(ds.Tables[0].Rows[i]["CardType_RID"].ToString());
                            }
                            if (ds.Tables[0].Rows[i]["Comment"].ToString() != "")
                            {
                                dr["Comment"] = ds.Tables[0].Rows[i]["Comment"].ToString();
                            }
                            if (ds.Tables[0].Rows[i]["Wafer_Name"].ToString() != "")
                            {
                                dr["Wafer_Name"] = ds.Tables[0].Rows[i]["Wafer_Name"].ToString();
                            }
                            if (ds.Tables[0].Rows[i]["Wafer_RID"].ToString() != "")
                            {
                                dr["Wafer_RID"] = Convert.ToInt32(ds.Tables[0].Rows[i]["Wafer_RID"].ToString());
                            }
                            //mod chaoma start
                            if (ds.Tables[0].Rows[i]["Change_UnitPrice"].ToString() != "")
                            {
                                dr["Base_Price"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Change_UnitPrice"].ToString()); ;
                            }
                            //mod chaoma end 
                            if (ds.Tables[0].Rows[i]["Wafer_Capacity"].ToString() != "")
                            {
                                dr["Wafer_Capacity"] = Convert.ToInt32(ds.Tables[0].Rows[i]["Wafer_Capacity"].ToString());
                            }
                            //add chaoma start
                            if (ds.Tables[0].Rows[i]["Unit_Price"].ToString() != "")
                            {
                                dr["Change_UnitPrice"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Unit_Price"].ToString()); ;
                            }
                            //add chaoma end 
                            inputs.Clear();
                            inputs.Add("Order_Date", ds.Tables[0].Rows[i]["Order_Date"].ToString());
                            inputs.Add("txtNumber", ds.Tables[0].Rows[i]["number"].ToString());
                            inputs.Add("dropSpace_Short_RID", ds.Tables[0].Rows[i]["CardType_RID"].ToString());
                            inputs.Add("factory_rid", lblFactory_ShortName_CN.Text);

                            //根據數量和卡種得到對應的合約、預算、晶片名稱、交貨地點（perso廠）
                            DataSet dst = depManager.GetOrderFormDetailCombo(inputs, true);

                            //if (dst.Tables["PERSO_CARDTYPE"].Rows.Count > 0)
                            //{
                            //    dr["factory_shortname_cn"] = dst.Tables["PERSO_CARDTYPE"].Rows[0]["text"].ToString();
                            //}
                            if (dst.Tables["AGREEMENT"].Rows.Count > 0)
                            {
                                dr["factory"] = dst.Tables["AGREEMENT"].Rows[0]["factory_rid"].ToString();
                            }

                            dr["factory_id"] = lblFactory_ShortName_CN.Text;

                            dr["bz"] = "1";

                            dt.Rows.Add(dr);
                        }

                        Session["detail"] = dt;
                        gvpbORDERFORMDETAIL.BindData();
                    }
                    //}
                }
                catch (AlertException ale)
                {
                    ShowMessage(ale.Message);
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
        }

        //btnDetail.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository002Detail.aspx?ActionType=Add','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");

        //gvpbORDERFORMDETAIL.BindData();
    }

    protected void btnAddDetail_Click(object sender, EventArgs e)
    {
        gvpbORDERFORMDETAIL.BindData();
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        dt = (DataTable)Session["detail"];

        if (CanUseAction(GlobalString.UserRoleConfig.Commit) && CanUseAction(GlobalString.UserRoleConfig.Add) && CanUseAction(GlobalString.UserRoleConfig.Edit))//經辦登陸（提交、新增和修改權限）
        {
            int result = Convert.ToInt32(dt.Rows[0]["pass_status"].ToString());
            if (result == 1 || result == 2)//放行狀態為：退回或暫存
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[int.Parse(e.CommandArgument.ToString())];

                    if (!depManager.IsStock(dr["orderform_detail_rid"].ToString()))
                    {
                        //if (depManager.IsSaveDB(dr))
                        //{
                        if (dt.Rows.Count == 1)
                        {
                            ShowMessage("必須保留至少一條訂單詳細記錄！");
                            return;

                            //depManager.RD(dr);
                            //dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                            //Session["delRID"] = lblOrderForm_RID.Text;
                            //Session["delBlank"] = lblFactory_ShortName_CN.Text;
                            //Session["detail"] = dt;
                            //gvpbORDERFORMDETAIL.BindData();
                        }
                        else if (dt.Rows.Count > 1)
                        {
                            depManager.RD(dr);
                            dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                            Session["detail"] = dt;
                            gvpbORDERFORMDETAIL.BindData();
                        }
                        //}
                        //else
                        //{
                        //    if (dt.Rows.Count == 1)
                        //    {
                        //        dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                        //        ShowMessage("必須保留至少一條訂單詳細記錄！");

                        //        Session["delRID"] = lblOrderForm_RID.Text;
                        //        Session["delBlank"] = lblFactory_ShortName_CN.Text;
                        //        Session["detail"] = dt;
                        //        gvpbORDERFORMDETAIL.BindData();
                        //    }
                        //    else if (dt.Rows.Count > 1)
                        //    {
                        //        dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                        //        Session["detail"] = dt;
                        //        gvpbORDERFORMDETAIL.BindData();
                        //    }
                        //}
                    }
                    else
                    {
                        ShowMessage("該訂單已經入庫，無法刪除！");
                    }
                }
            }
            else
            {
                ShowMessage("當前記錄無法刪除！");
            }
        }
        else if (CanUseAction(GlobalString.UserRoleConfig.Pass) && CanUseAction(GlobalString.UserRoleConfig.Reject))//主管登陸(放行和退回權限)
        {
            ShowMessage("當前用戶沒有刪除權限！");
        }
        else if (CanUseAction(GlobalString.UserRoleConfig.Delete))
        {
            int result = Convert.ToInt32(dt.Rows[0]["pass_status"].ToString());
            if (result == 1 || result == 2)//放行狀態為：退回或暫存
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[int.Parse(e.CommandArgument.ToString())];

                    if (!depManager.IsStock(dr["orderform_detail_rid"].ToString()))
                    {
                        if (depManager.IsSaveDB(dr))
                        {
                            if (dt.Rows.Count == 1)
                            {
                                depManager.RD(dr);
                                dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                                ShowMessage("必須保留至少一條訂單詳細記錄！");

                                Session["delRID"] = lblOrderForm_RID.Text;
                                Session["delBlank"] = lblFactory_ShortName_CN.Text;
                                Session["detail"] = dt;
                                gvpbORDERFORMDETAIL.BindData();
                            }
                            else if (dt.Rows.Count > 1)
                            {
                                depManager.RD(dr);
                                dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                                Session["detail"] = dt;
                                gvpbORDERFORMDETAIL.BindData();
                            }
                        }
                        else
                        {
                            if (dt.Rows.Count == 1)
                            {
                                dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                                ShowMessage("必須保留至少一條訂單詳細記錄！");

                                Session["delRID"] = lblOrderForm_RID.Text;
                                Session["delBlank"] = lblFactory_ShortName_CN.Text;
                                Session["detail"] = dt;
                                gvpbORDERFORMDETAIL.BindData();
                            }
                            else if (dt.Rows.Count > 1)
                            {
                                dt.Rows.RemoveAt(int.Parse(e.CommandArgument.ToString()));

                                Session["detail"] = dt;
                                gvpbORDERFORMDETAIL.BindData();
                            }
                        }
                    }
                    else
                    {
                        ShowMessage("該訂單已經入庫，無法刪除！");
                    }
                }
            }
            else
            {
                ShowMessage("當前記錄無法刪除！");
            }
        }
    }

    //退回
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        dt = (DataTable)Session["detail"];
        if (dt != null && dt.Rows.Count > 0)
        {
            if (depManager.IsSaveDB(dt))
            {
                depManager.Reject(dt);
                Response.Redirect("Depository002.aspx?Con=1");
            }
            else
            {
                ShowMessage("有資料未保存入庫！");
            }
        }
        else
        {
            ShowMessage("無可退回資料！");
        }
    }

    //放行
    protected void btnPass_Click(object sender, EventArgs e)
    {
        dt = (DataTable)Session["detail"];
        if (dt != null && dt.Rows.Count > 0)
        {
            if (depManager.IsSaveDB(dt))
            {
                depManager.Pass(dt);
                Response.Redirect("Depository002.aspx?Con=1");
            }
            else
            {
                ShowMessage("有資料未保存入庫！");
            }
        }
        else
        {
            ShowMessage("無可放行資料！");
        }
    }

    //提交
    protected void btnCommit_Click(object sender, EventArgs e)
    {
        dt = (DataTable)Session["detail"];
        if (dt != null && dt.Rows.Count > 0)
        {
            if (depManager.IsSaveDB(dt))
            {
                depManager.Confirm(dt);
                Response.Redirect("Depository002.aspx?Con=1");
            }
            else
            {
                ShowMessage("有資料未保存入庫！");
            }
        }
        else
        {
            ShowMessage("無可提交資料！");
        }
    }

    //保存到資料庫
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            dt = (DataTable)Session["detail"];
            //dtclone = (DataTable)Session["OldUpdate"];
            DataSet dstmp = depManager.GetOrderFormDetail_RID(lblOrderForm_RID.Text.Trim());

            if (dt != null && dt.Rows.Count > 0)
            {
                if (dstmp != null && dstmp.Tables[0].Rows.Count > 0)
                {
                    if (depManager.IsSaveDB(dstmp.Tables[0]))
                    {
                        if (chkDel.Checked)
                        {
                            if (!depManager.IsStock(lblOrderForm_RID.Text.Trim()))
                            {
                                depManager.RD(dstmp.Tables[0]);
                                Response.Redirect("Depository002.aspx?Con=1");
                            }
                            else
                            {
                                ShowMessage("有資料已經入庫，無法刪除！");
                            }
                        }
                        else
                        {
                            ShowMessage("請勾選‘刪除整筆訂單’選項！");
                        }
                    }
                    else
                    {
                        ShowMessage("有資料未保存入庫！");
                    }
                }
                else
                {
                    ShowMessage("Session失效，請重新登陸！");
                }
            }
            else
            {
                ShowMessage("沒有訂單記錄需要保存！");
            }
        }
        catch (AlertException aex)
        {
            ShowMessage(aex.Message);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        dt = (DataTable)Session["detail"];
        if (dt != null && dt.Rows.Count > 0)
        {
            string str = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    str += dt.Rows[i]["orderform_rid"].ToString();
                }
                else
                {
                    str += "," + dt.Rows[i]["orderform_rid"].ToString();
                }
            }
            Response.Write("<script>window.open('Depository002Print.aspx?orderform_rid=" + str + "');</script>");
        }
        else
        {
            ShowMessage("沒有可列印資料！");
        }
    }

    #region 列表資料綁定
    /// <summary>
    /// GridView資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbORDERFORMDETAIL_OnSetDataSource(object sender, ControlLibrary.SetDataSourceEventArgs e)
    {
        dt = (DataTable)Session["detail"];

        if (dt != null && dt.Rows.Count > 0)
        {
            e.Table = dt;
            e.RowCount = dt.Rows.Count;
        }
        else
        {
            e.Table = dt;
            e.RowCount = 0;
        }
    }

    /// <summary>
    /// GridView列資料綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbORDERFORMDETAIL_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblOrderForm = (DataTable)gvpbORDERFORMDETAIL.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (dtblOrderForm.Rows.Count == 0)
                return;

            Label lblis_exigence = null;
            lblis_exigence = (Label)e.Row.FindControl("lblis_exigence");

            if (dtblOrderForm.Rows[e.Row.RowIndex]["is_exigence"].ToString() == GlobalString.Exigence.Urgent)
            {
                lblis_exigence.Text = "Urgent";
            }
            else if (dtblOrderForm.Rows[e.Row.RowIndex]["is_exigence"].ToString() == GlobalString.Exigence.Normal)
            {
                lblis_exigence.Text = "Normal";
            }

            Label lblSpace_Short_RID = null;
            lblSpace_Short_RID = (Label)e.Row.FindControl("lblSpace_Short_RID");
            lblSpace_Short_RID.Text = depManager.GetSpace_Short_RID(dtblOrderForm.Rows[e.Row.RowIndex]["Space_Short_RID"].ToString());

            ImageButton ibtnButton = null;

            // 刪除的邦定事件
            ibtnButton = (ImageButton)e.Row.FindControl("ibtnDelete");
            ibtnButton.CommandArgument = e.Row.RowIndex.ToString();

            ibtnButton.Visible = false;
            ibtnButton.Enabled = false;

            if (CanUseAction(GlobalString.UserRoleConfig.Commit) && CanUseAction(GlobalString.UserRoleConfig.Add) && CanUseAction(GlobalString.UserRoleConfig.Edit))//經辦登陸（提交、新增和修改權限）
            {
                int result = Convert.ToInt32(dtblOrderForm.Rows[0]["pass_status"].ToString());
                if (result == 1 || result == 2)//放行狀態為：退回或暫存
                {
                    ibtnButton.Visible = true;
                    ibtnButton.Enabled = true;
                    ibtnButton.OnClientClick = string.Concat("return confirm(\'", GlobalStringManager.Default["Info_DeleteHint"], "\')");
                }
                else
                {
                    ibtnButton.Visible = false;
                    ibtnButton.Enabled = false;
                }
            }
            else if (CanUseAction(GlobalString.UserRoleConfig.Pass) && CanUseAction(GlobalString.UserRoleConfig.Reject))//主管登陸(放行和退回權限)
            {
                ibtnButton.Visible = false;
            }
            else if (CanUseAction(GlobalString.UserRoleConfig.Delete))
            {
                ibtnButton.Visible = true;
                ibtnButton.Enabled = true;
            }

            try
            {
                if (e.Row.Cells[2].Text != "" && e.Row.Cells[2].Text != "&nbsp;")
                {
                    e.Row.Cells[2].Text = int.Parse(e.Row.Cells[2].Text).ToString("N0");
                }
            }
            catch { }

            HyperLink hl = (HyperLink)e.Row.FindControl("OrderForm_Detail_RID");
            hl.Text = dtblOrderForm.Rows[e.Row.RowIndex]["OrderForm_Detail_RID"].ToString();
            hl.NavigateUrl = "#";
            hl.Attributes.Add("onclick", "var aa=window.showModalDialog('Depository002Detail.aspx?ActionType=Edit&RID=" + e.Row.RowIndex.ToString() + "','','dialogHeight:600px;dialogWidth:600px;');if(aa!=undefined){ImtBind();}");

            // add by Ian Huang start
            if (e.Row.Cells[3].Text.Trim() == e.Row.Cells[4].Text.Trim())
            {
                e.Row.Cells[4].Text = "";
            }
            else
            {
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Red;
            }
            // add by Ian Huang end
        }
    }
    #endregion
}
