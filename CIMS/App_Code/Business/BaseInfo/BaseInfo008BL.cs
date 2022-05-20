using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// BaseInfo008BL 的摘要描述
/// </summary>
public class BaseInfo008BL : BaseLogic
{

    #region SQL語句  
    public const string SEL_MATERIAL_SPECIAL = "select rid,Material_Code,Material_Name "
                                        + "from MATERIAL_SPECIAL "
                                        + "WHERE RST='A'";
    public const string CHK_MATERIAL_BY_RID = "proc_CHK_DEL_MATERIAL";

    public const string SEL_CARD_TYPE = "select ct.rid,CT.Display_Name as NAME"
                                        + " from card_type as ct"
                                        + " where ct.rid in (select cm.cardtype_rid from cardType_material as cm where cm.material_rid = @mRid)";       

    
    public const string DEL_CARDTYPE_MATERIAL = "delete from CARDTYPE_MATERIAL"
                                        + " where MATERIAL_RID = @mRid";

    #endregion

    //資料參數
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public BaseInfo008BL()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>
    /// 查詢結果按照排序欄位排序
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="firstRowNumber"></param>
    /// <param name="lastRowNumber"></param>
    /// <param name="sortField"></param>
    /// <param name="sortType"></param>
    /// <param name="rowCount"></param>
    /// <returns></returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "Material_Code" : sortField);//默認的排序欄位
        StringBuilder stbCommand = new StringBuilder(SEL_MATERIAL_SPECIAL);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//如果有查詢條件輸入
        {
            if (!StringUtil.IsEmpty(searchInput["txtName"].ToString().Trim()))
            {
                stbWhere.Append(" AND Material_Name like @name");
                dirValues.Add("name", "%" + searchInput["txtName"].ToString().Trim() + "%");

            }
        }
        //執行sql語句
        DataSet dstMateriel = null;
        try
        {
           dstMateriel = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SearchFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SearchFail);
        }
        rowCount = intRowCount;
        return dstMateriel;    
    }

    /// <summary>
    /// 新增特殊材質
    /// </summary>
    /// <param name="materialModel"></param>
    /// <param name="dtblCardType"></param>
    public void add(MATERIAL_SPECIAL materialModel, DataTable dtblCardType)
    {
        //資料實體
        CARDTYPE_MATERIAL cmModel = new CARDTYPE_MATERIAL();
        try
        {
            //事務開始
            dao.OpenConnection();

            //新增特殊材質基本檔，返回RID
            materialModel.Material_Code = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Material_ID");
            int intRID = Convert.ToInt32(dao.AddAndGetID<MATERIAL_SPECIAL>(materialModel, "RID"));

            //foreach更新已選擇的卡種記錄
            foreach (DataRow drowCardType in dtblCardType.Rows)
            {
                cmModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                cmModel.Material_RID = intRID;
                dao.Add<CARDTYPE_MATERIAL>(cmModel, "RID");
            }

            //操作日誌
            SetOprLog();

            //事務提交
            dao.Commit();
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// 取特殊材質
    /// </summary>
    /// <param name="rid"></param>
    /// <returns></returns>
    public MATERIAL_SPECIAL GetMaterialByRid(string strRID)
    {
        MATERIAL_SPECIAL modeResult = new MATERIAL_SPECIAL();      
        try {
            modeResult = dao.GetModel<MATERIAL_SPECIAL, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex) {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_GetModelFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_GetModelFail);
        }
        return modeResult;
    }

    /// <summary>
    /// 刪除此特殊材質
    /// </summary>
    /// <param name="strRID"></param>
    public void Delete(string strRID)
    {
        //資料實體
        MATERIAL_SPECIAL eiModel = new MATERIAL_SPECIAL();

        try
        {
            //事務開始
            dao.OpenConnection();
            dirValues.Clear();
            ChkDelMaterial(strRID);

            //DataSet dstCardType = GetCard(strRID);
            //if (dstCardType.Tables[0] != null && dstCardType.Tables[0].Rows.Count > 0)
            //{
            //    throw new AlertException(BizMessage.BizCommMsg.ALT_CMN_CanntDel);
            //} 


            //刪除该筆材質基本資料記錄
            dirValues.Add("mRid", int.Parse(strRID));
            dao.ExecuteNonQuery(DEL_CARDTYPE_MATERIAL, dirValues, false);

            //eiModel = dao.GetModel<Material_Special, int>("RID", int.Parse(strRID));
            //eiModel.RST = "D";
            //dao.Update<MATERIAL_SPECIAL>(eiModel, "RID");

            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("Material_Special", dirValues);

            //操作日誌
            SetOprLog("4");

            //事務提交
            dao.Commit();
        }
        catch (AlertException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            //事務回滾
            dao.Rollback();
            //異常處理
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_DeleteFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_DeleteFail);
        }
        finally
        {
            //關閉連接
            dao.CloseConnection();
        }
    }


    /// <summary>
    /// 檢查卡種狀況是否可以刪除
    /// </summary>
    /// <param name="strRID"></param>
    public void ChkDelMaterial(string strRID)
    {
        dirValues.Clear();
        dirValues.Add("Material_Rid", strRID);

        DataSet dstBudget = dao.GetList(CHK_MATERIAL_BY_RID, dirValues, true);
        if (dstBudget.Tables[0].Rows[0][0].ToString() != "0")
            throw new AlertException(String.Format(BizMessage.BizCommMsg.ALT_CMN_CannotDel, "材質"));

    }


    /// <summary>
    /// 取特殊材質下所有的卡種
    /// </summary>
    /// <param name="rid"></param>
    /// <returns></returns>
    public DataSet GetCard(string strRID)
    {
        DataSet dstCard = null;
        try
        {
            dirValues.Clear();
            dirValues.Add("mRid", int.Parse(strRID));

            dstCard = dao.GetList(SEL_CARD_TYPE + " order by Display_Name", dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_InitPageFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_InitPageFail);
        }
        return dstCard;
    }

    /// <summary>
    /// 更新特殊材質基本檔
    /// </summary>
    /// <param name="eiModel"></param>
    /// <param name="dtblCardType"></param>
    public void Update(MATERIAL_SPECIAL eiModel, DataTable dtblCardType)
    {
        //資料實體
        CARDTYPE_MATERIAL cmModel = new CARDTYPE_MATERIAL();

        try
        {
            dirValues.Clear();

            MATERIAL_SPECIAL oldModel = GetMaterialByRid(eiModel.RID.ToString());
            if (oldModel.Material_Name == eiModel.Material_Name)
            {
                //刪除[特殊材質].RID对应[卡種]關係
                dirValues.Add("mRid", eiModel.RID);
                dao.ExecuteNonQuery(DEL_CARDTYPE_MATERIAL, dirValues, false);
                //foreach更新材質與已選擇的卡種關係
                foreach (DataRow drowCardType in dtblCardType.Rows)
                {
                    cmModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                    cmModel.Material_RID = eiModel.RID;
                    dao.Add<CARDTYPE_MATERIAL>(cmModel, "RID");
                }
            }
            else
            {

                //邏輯刪除原來的材質基本資料,物理刪除材質與卡種類關係檔               
                oldModel.RST = "D";
                dao.Update<MATERIAL_SPECIAL>(oldModel, "RID");                
                dirValues.Add("mRid", oldModel.RID);
                dao.ExecuteNonQuery(DEL_CARDTYPE_MATERIAL, dirValues, false);             
                //新增该筆材質基本資料記錄
                eiModel.Material_Code = IDProvider.MainIDProvider.GetSystemNewIDWithNoDate("Material_ID");
                int newRid =Convert.ToInt32(dao.AddAndGetID<MATERIAL_SPECIAL>(eiModel, "RID"));
                //foreach更新材質與已選擇的卡種關係
                foreach (DataRow drowCardType in dtblCardType.Rows)
                {
                    cmModel.CardType_RID = Convert.ToInt32(drowCardType["RID"]);
                    cmModel.Material_RID = newRid;
                    dao.Add<CARDTYPE_MATERIAL>(cmModel, "RID");
                }
            }

            //操作日誌
            SetOprLog();
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizCommMsg.ALT_CMN_SaveFail, ex.Message, dao.LastCommands);
            throw new Exception(BizMessage.BizCommMsg.ALT_CMN_SaveFail);
        }       
    }



}
