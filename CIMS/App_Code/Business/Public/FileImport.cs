using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// FileImport 的摘要描述
/// </summary>
public class FileImport
{
    public FileImport()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }


    public static string ImportFile(string strPath, DataTable dtblFile)
    {
        StreamReader sr = new StreamReader(strPath, System.Text.Encoding.Default);
        string[] strLine;
        string strReadLine="";
        int count = 1;
        string strErr="";

        while ((strReadLine = sr.ReadLine()) != null)
        {
            strLine=strReadLine.Split(',');

            if (strLine.Length != dtblFile.Columns.Count)
            {
                strErr = "匯入文件格式不正確";
                return strErr;
            }

            DataRow dr = dtblFile.NewRow();

            for (int i = 0; i < strLine.Length; i++)
            {
                if (StringUtil.IsEmpty(strLine[i]))
                {
                    int num = i + 1;
                    strErr += "第" + count.ToString() + "行第" + num.ToString() + "列為空";
                }
                dr[i] = strLine[i];
            }

            dtblFile.Rows.Add(dr);

            count++;
        }

        sr.Close();


        return strErr;
    }
}
