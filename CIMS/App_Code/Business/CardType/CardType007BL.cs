//******************************************************************
//*  �@    �̡GYanli.Ji
//*  �\�໡���G�p�p�ɻP�פJ���س]�w�]�w�޿�
//*  �Ыؤ���G2008-09-02
//*  �ק����G2008-09-02 12:00
//*  �ק�O���G
//*            ��2008-09-02
//*              1.�Ы� Yanli.Ji
//*******************************************************************
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
/// CardType007BL ���K�n�y�z
/// </summary>
public class CardType007BL : BaseLogic
{

    #region SQL�y�y
    public const string SEL_IMPORT_PROJECT = "SELECT IP.RID, IP.File_Name,P.Param_Name, CG.Group_Name+' / '+MCT.Type_Name AS Text "
                                             + "FROM IMPORT_PROJECT AS IP "
                                             + "LEFT JOIN PARAM AS P "
                                             + "ON P.RST='A' AND P.ParamType_Code = '" + GlobalString.ParameterType.Type + "' AND P.Param_Code = IP.Type "
                                             + "LEFT JOIN MAKE_CARD_TYPE AS MCT ON MCT.RST = 'A' AND MCT.Is_Import = 'Y' AND MCT.RID = IP.MakeCardType_RID "
                                             + "LEFT JOIN CARD_GROUP AS CG ON CG.RST = 'A' AND CG.RID = MCT.cardGroup_RID "
                                             + "WHERE IP.RST = 'A'";
    public const string CON_IMPORT_PROJECT = "SELECT COUNT(*) FROM IMPORT_PROJECT AS IP WHERE IP.RST = 'A' AND IP.File_Name = @File_Name";
    public const string SEL_PARAM = "SELECT P.Param_Code AS Value, P.Param_Name AS Text FROM PARAM AS P WHERE P.RST = 'A' AND P.ParamType_Code = '" + GlobalString.ParameterType.Type + "'";
    public const string SEL_MAKE_CARD_TYPE = "SELECT MCT.RID AS Value, CG.Group_Name + ' / ' +  MCT.Type_Name AS Text "
                                       + "FROM MAKE_CARD_TYPE AS MCT INNER JOIN CARD_GROUP AS CG ON CG.RST = 'A' AND CG.RID = MCT.cardGroup_RID WHERE MCT.RST = 'A' AND MCT.Is_Import = 'Y'";

    #endregion

    //�ƾڰѼ�
    Dictionary<string, object> dirValues = new Dictionary<string, object>();

    public CardType007BL()
    {
        //
        // TODO: Add constructor logic here
        //
    }



    /// <summary>
    /// �d�߬O�_�w�s�bFile_Name
    /// </summary>
    /// <param name="strParamCode"></param>
    /// <returns></returns>
    public bool ContainsID(string strFile_Name)
    {

        try
        {
            StringBuilder stbCommand = new StringBuilder(CON_IMPORT_PROJECT);
            dirValues.Clear();
            dirValues.Add("File_Name", strFile_Name);
            return dao.Contains(stbCommand.ToString(), dirValues);
        }
        catch
        {
            return true;
        }
    }


    /// <summary>
    /// �s�W�p�p�ɻP�פJ���س]�w�H��
    /// </summary>
    /// <param name="ipModel"></param>
    public void Add(IMPORT_PROJECT ipModel)
    {
        try
        {
            //�ưȶ}�l
            dao.OpenConnection();

            dao.Add<IMPORT_PROJECT>(ipModel, "RID");

            //�ާ@��x
            SetOprLog();

            //�ưȴ���
            dao.Commit();
        }
        catch (Exception ex)
        {
            //�ưȦ^�u
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //�����s��
            dao.CloseConnection();
        }
    }

    /// <summary>
    /// �d��
    /// </summary>
    /// <param name="searchInput"></param>
    /// <param name="firstRowNumber">������ܲĤ@���O���s��</param>
    /// <param name="lastRowNumber">������̫ܳ�@���O���s��</param>
    /// <param name="sortField">�ƧǦr�q</param>
    /// <param name="sortType">�ƧǤ覡</param>
    /// <param name="rowCount">���</param>
    /// <returns></returns>
    public DataSet List(Dictionary<string, object> searchInput, string firstRowNumber, string lastRowNumber, string sortField, string sortType, out int rowCount)
    {
        //--1�B��z�Ѽ�--------------------------------------------------------------------------------------------
        int intRowCount = 0;
        string strSortField = (sortField == "null" ? "File_Name" : sortField);//�q�{���Ƨ����
        StringBuilder stbCommand = new StringBuilder(SEL_IMPORT_PROJECT);
        StringBuilder stbWhere = new StringBuilder();
        dirValues.Clear();
        if (searchInput != null && searchInput.Keys.Count > 0)//�p�G���d�߱����J
        {
            if (!StringUtil.IsEmpty(searchInput["dropType"].ToString().Trim()))
            {
                stbWhere.Append(" AND IP.Type = @Type ");
                dirValues.Add("Type", searchInput["dropType"].ToString().Trim());

            }
            if (!StringUtil.IsEmpty(searchInput["dropMakeCardType_RID"].ToString().Trim()))
            {
                stbWhere.Append(" AND IP.MakeCardType_RID = @MakeCardType_RID ");
                dirValues.Add("MakeCardType_RID", searchInput["dropMakeCardType_RID"].ToString().Trim());

            }
        }
        DataSet dtsImport_Project = null;
        try
        {
            dtsImport_Project = dao.GetList(stbCommand.ToString() + stbWhere.ToString(), dirValues, firstRowNumber, lastRowNumber, strSortField, sortType, out intRowCount);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        rowCount = intRowCount;
        return dtsImport_Project;
    }

    /// <summary>
    /// ����p�p�ɻP�פJ���س]�w�ƾڼҫ�
    /// </summary>
    /// <param name="strRID"></param>
    public IMPORT_PROJECT GetImportProject(string strRID)
    {
        IMPORT_PROJECT ipModel = null;
        try
        {
            ipModel = dao.GetModel<IMPORT_PROJECT, int>("RID", int.Parse(strRID));
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SearchFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SearchFailErr"]);
        }
        return ipModel;
    }

    /// <summary>
    /// �ק�p�p�ɻP�פJ���س]�w�H��
    /// </summary>
    /// <param name="ipModel"></param>
    public void Update(IMPORT_PROJECT ipModel)
    {
        try
        {
            //�ưȶ}�l
            dao.OpenConnection();

            //�ק�p�p�ɻP�פJ���س]�w�H����ƾڮw
            dao.Update<IMPORT_PROJECT>(ipModel, "RID");

            //�ާ@��x
            SetOprLog();

            //�ưȴ���
            dao.Commit();
        }

        catch (Exception ex)
        {
            //�ưȦ^�u
            dao.Rollback();
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }
        finally
        {
            //�����s��
            dao.CloseConnection();
        }
    }


    /// <summary>
    /// �R��
    /// </summary>
    /// <param name="strRID"></param>
    public void Delete(string strRID)
    {
        //�ƾڹ���
        IMPORT_PROJECT ipModel = new IMPORT_PROJECT();
        try
        {
            dirValues.Clear();
            dirValues.Add("RID", int.Parse(strRID));
            dao.Delete("IMPORT_PROJECT", dirValues);


            //ipModel = GetImportProject(strRID);
            ////�i���޿�R���B�z
            //ipModel.RST = "D";
            //dao.Update<IMPORT_PROJECT>(ipModel, "RID");

            //�ާ@��x
            SetOprLog("4");
        }
        catch (Exception ex)
        {
            //���`�B�z
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_SaveFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_SaveFailErr"]);
        }

    }
    /// <summary>
    /// ������O�M�妸�H��
    /// </summary>
    /// <param></param>
    public DataSet GetTypeAndMakeCardType()
    {
        DataSet dstTypeAndMakeCardType = null;
        dirValues.Clear();
        try
        {
            dstTypeAndMakeCardType = dao.GetList(SEL_PARAM + "  " + SEL_MAKE_CARD_TYPE, dirValues);
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(GlobalStringManager.Default["Alert_InitPageFailErr"], ex.Message, dao.LastCommands);
            throw new Exception(GlobalStringManager.Default["Alert_InitPageFailErr"]);
        }
        return dstTypeAndMakeCardType;
    }

}
