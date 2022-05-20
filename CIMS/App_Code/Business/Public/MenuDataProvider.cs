//******************************************************************
//*  作    者：QingChen
//*  功能說明：全局菜單資料整理邏輯
//*  創建日期：2008/05/21
//*  修改日期：2008/05/21  16:59
//*  修改記錄：
//*            □2008/05/21 
//*              1.創建 陳青
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data;
/// <summary>
/// MenuDataProvider 的摘要描述
/// </summary>
public abstract class MenuDataProvider
{

    /// <summary>
    /// 菜單使用的XML資料源
    /// </summary>
    public static XmlDocument MenuXmlDocument 
    {
        get
        {
            //if (xmdMenuXmlDocument == null)
            //{
                GetDocument();
            //}
            return xmdMenuXmlDocument;
        }
    }
    private static volatile XmlDocument xmdMenuXmlDocument;

    /// <summary>
    /// 重新從資料庫獲取菜單資料
    /// </summary>
    public static void ReLoadMenuData()
    {
        GetDocument();
    }
    /// <summary>
    /// 從資料庫獲取菜單資料
    /// </summary>
    private static void GetDocument()
    {
        xmdMenuXmlDocument = new XmlDocument();

        #region Legend 2016/10/26 因企業庫升級調整寫法

        //Database dbMenuDatabase = DatabaseFactory.CreateDatabase();

        DatabaseProviderFactory factory = new DatabaseProviderFactory();

        Database dbMenuDatabase = factory.CreateDefault();

        #endregion  
        
        try
        {
            DataSet dstMenuDataList = dbMenuDatabase.ExecuteDataSet(CommandType.Text, "select *,FunctionName as FunctionName_zh_tw from [Function] where InMenu=1 select max(Level) as MaxLevel from [Function] where InMenu=1");
            for (int index = 0; index < dstMenuDataList.Tables.Count; index++)
            {
                for (int j = 0; j < dstMenuDataList.Tables[index].Columns.Count; j++)
                {
                    for (int k = 0; k < dstMenuDataList.Tables[index].Rows.Count; k++)
                    {
                        dstMenuDataList.Tables[index].Rows[k][j] = dstMenuDataList.Tables[index].Rows[k][j].ToString().Trim();
                    }
                }
            }
            DataSet xmlDataSet = new DataSet();
            int maxLevel = (int)(dstMenuDataList.Tables[1].Rows[0][0]);
            maxLevel++;
            string tempTableName = string.Empty;

            for (int index = 1; index < maxLevel; index++)
            {
                tempTableName = FillTable(dstMenuDataList, 0, xmlDataSet, "MenuLevel" + index.ToString(), tempTableName, "Level=" + index.ToString(), "SortOrder");
            }
            
            lock (xmdMenuXmlDocument)
            {

                xmdMenuXmlDocument.LoadXml(xmlDataSet.GetXml());
                XmlTrim(xmdMenuXmlDocument);
            }
        }
        catch (Exception ex)
        {
            ExceptionFactory.CreateCustomSaveException(BizMessage.BizPublicMsg.ALT_GetMenuFail, ex);
            throw ex;
        }
       
      
        
    }

    /// <summary>
    /// 將表資料按照樹結構整和到新的資料集
    /// </summary>
    /// <param name="data">資料來源</param>
    /// <param name="tableIndex">資料來源的表序號</param>
    /// <param name="newData">要填充的資料集</param>
    /// <param name="newTableName">新表的名稱</param>
    /// <param name="dtblParentTableName">父級別表名稱</param>
    /// <param name="expressions">選擇表達式</param>
    /// <param name="sort">排序欄位名稱</param>
    /// <returns></returns>
    private static string FillTable(DataSet data, int tableIndex, DataSet newData, string newTableName, string dtblParentTableName,string expressions, string sort)
    {
        DataTable dtblDataTable = newData.Tables.Add(newTableName);
        for (int i = 0; i < data.Tables[tableIndex].Columns.Count; i++)
        {
            dtblDataTable.Columns.Add(data.Tables[tableIndex].Columns[i].ColumnName).ColumnMapping = MappingType.Attribute;
        }

        DataRow[] drowRows = data.Tables[0].Select(expressions, sort);
        for (int i = 0; i < drowRows.Length; i++)
        {
            dtblDataTable.Rows.Add(drowRows[i].ItemArray);
        }
        if (dtblParentTableName != string.Empty && newData.Tables.Contains(dtblParentTableName))
        {
            DataTable dtblParentTable = newData.Tables[dtblParentTableName];
            DataRelation drDataRelation = new DataRelation(dtblParentTable.TableName + "_" + newTableName, dtblParentTable.Columns["FunctionID"], dtblDataTable.Columns["ParentFunctionID"]);
            drDataRelation.Nested = true;
            newData.Relations.Add(drDataRelation);
        }
        return newTableName;
    }

    /// <summary>
    /// 去掉XML文檔資料中的空格
    /// </summary>
    /// <param name="Node">XML資料根節點</param>
    private static void XmlTrim(XmlNode Node)
    {
        if(Node!=null && Node.Attributes!=null)
        {
        for (int index = 0; index < Node.Attributes.Count; index++)
        {
            Node.Attributes[index].Value = Node.Attributes[index].Value.Trim();
        }
        for (int index = 0; index < Node.ChildNodes.Count; index++)
        {
            if (Node.ChildNodes[index].NodeType == XmlNodeType.Element)
            {
                XmlTrim(Node.ChildNodes[index]);
            }
        }
        }
    }
}
