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

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //MyLogger ml = new MyLogger(GlobalString.LogType.ErrorCategory);
        //ml.Write("dfdf", GlobalString.LogType.ErrorCategory);

        
        //LogFactory.Write("aaaaa", GlobalString.LogType.ErrorCategory);
        //LogFactory.Write("bbbbb", GlobalString.LogType.OpLogCategory);
        //LogFactory.Write("ccccc", GlobalString.LogType.ErrorCategory);
        //LogFactory.Write("ddddd", GlobalString.LogType.OpLogCategory);


        FTPFactory ftp = new FTPFactory();
        ////string ftpPath = ConfigurationManager.AppSettings["FTPRemoteYearReplaceCard"]; //ftp檔案目錄配置檔信息
        string locPath = ConfigurationManager.AppSettings["YearReplaceCardForecastFilesPath"]; //local檔案目錄配置檔信息
        //// ftp.Upload("Saprate", "aa.txt", @"D:\job\cims_code\CIMS\FileUpload\20090304160514.txt");

        //FTPClient ftpC = new FTPClient();
        //ftpC.RemoteHost = "172.26.100.104";
        //ftpC.RemotePass = "e9c5rywh";
        //ftpC.RemotePath = "aptoap/CIMS/UBIQ_UPLOAD/";
        //ftpC.RemotePort = 21;
        //ftpC.RemoteUser = "cims";
        //ftpC.Connect();
        //ftpC.Put(@"D:\job\cims_code\CIMS\FileUpload\20090304160514.txt");
       
    }

}
