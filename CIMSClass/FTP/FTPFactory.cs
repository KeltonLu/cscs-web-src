//******************************************************************
//*  �@    �̡GRay
//*  �\�໡���GFTP�ާ@
//*  �Ыؤ���G2008-11-14
//*  �ק����G
//*  �ק�O���G
//*******************************************************************
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Configuration;

namespace CIMSClass.FTP
{

    public class FTPFactory
    {
        #region Attribute
        private string remoteHost, remotePath, remoteUser, remotePass, locPath;
        private int remotePort;

        private FtpWebRequest ftp;
        private WebResponse response;
        const int bufferSize = 2048; //�U���w�R2M
        #endregion
        #region �c�y���
        public FTPFactory()
        {
            remoteHost = ConfigurationManager.AppSettings["FTPRemoteIP"]; //FTP IP�a�}
            remotePath = ConfigurationManager.AppSettings["FTPRemotePathDefault"]; //FTP �ɮץؿ�
            remoteUser = ConfigurationManager.AppSettings["FTPUser"]; //FTP User
            remotePass = ConfigurationManager.AppSettings["FTPPassword"]; //FTP Password
            remotePort = Convert.ToInt32(ConfigurationManager.AppSettings["FTPRemotePort"]); //FTP Port
            locPath = ConfigurationManager.AppSettings["SubTotalFilesPath"]; // Local �ɮצs�x�ؿ�
        }


        public FTPFactory(string FtpType)
        {
            remoteHost = ConfigurationManager.AppSettings[FtpType + "FTPRemoteIP"]; //FTP IP�a�}
            remotePath = ConfigurationManager.AppSettings["FTPRemotePathDefault"]; //FTP �ɮץؿ�
            remoteUser = ConfigurationManager.AppSettings[FtpType + "FTPUser"]; //FTP User
            remotePass = ConfigurationManager.AppSettings[FtpType + "FTPPassword"]; //FTP Password
            remotePort = Convert.ToInt32(ConfigurationManager.AppSettings[FtpType + "FTPRemotePort"]); //FTP Port
            locPath = ConfigurationManager.AppSettings["SubTotalFilesPath"]; // Local �ɮצs�x�ؿ�
        }
        #endregion
        #region �n�JFTP
        /// <summary>
        /// �n��FTP
        /// </summary>
        /// <param name="Path">FTP�l���|</param>
        /// <returns></returns>
        private bool Login(string Path)
        {
            return Login(Path, "");

        }
        /// <summary>
        /// �n��FTP
        /// </summary>
        /// <param name="Path">FTP�l���|</param>
        /// <param name="remFileName">FTP�ɦW</param>
        /// <returns></returns>
        private bool Login(string Path, string remFileName)
        {
            return Login(remoteHost, Path, remFileName);

        }
        private bool Login(string Host, string Path, string remFileName)
        {
            return Login(Host, Path, remFileName, remoteUser, remotePass);

        }
        private bool Login(string Host, string Path, string remFileName, string remoteUser, string remotePass)
        {
            try
            {
                string FTPURL = "ftp://" + Host + "/";
                if (!StringUtil.IsEmpty(Path))
                {
                    FTPURL = FTPURL + Path + "/" + remFileName;
                }

                ftp = (FtpWebRequest)FtpWebRequest.Create(FTPURL);
                ftp.UseBinary = true;
                ftp.Credentials = new NetworkCredential(remoteUser, remotePass);
                ftp.KeepAlive = false;
                if (ConfigurationManager.AppSettings["FTPModel"].ToString() == "2")
                    ftp.UsePassive = false;
                return true;
            }
            catch (Exception ex)
            {
                LogFactory.Write("FTP�n�����ѡG" + ex.ToString(), GlobalString.LogType.ErrorCategory);
                return false;
            }

        }
        #endregion
        #region ���oFTP�ɮצC��
        /// <summary>
        /// ���o�ɮצC��
        /// </summary>
        /// <returns></returns>
        public string[] GetFileList()
        {
            return GetFileList(remotePath);
        }
        /// <summary>
        /// ���o�ɮצC��
        /// </summary>
        /// <param name="remotePath">FTP�l�ؿ�</param>
        /// <returns></returns>
        public string[] GetFileList(string remotePath)
        {
            return GetFileList(remotePath, "");
        }
        /// <summary>
        /// ���o�ɮצC��
        /// </summary>
        /// <param name="remotePath">FTP�l�ؿ�</param>
        /// <param name="remFileName">FTP�ɮצW��</param>
        /// <returns></returns>
        public string[] GetFileList(string remotePath, string remFileName)
        {
            string line;
            StringBuilder dicList = new StringBuilder();
            if (true)
            {
                Login(remotePath, remFileName);

            }

            try
            {
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = ftp.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                line = sr.ReadLine();
                while (line != null)
                {
                    dicList.Append(line);
                    dicList.Append("\n");
                    line = sr.ReadLine();
                }
                sr.Close();
                response.Close();
                return dicList.ToString().Split('\n');

            }
            catch (Exception ex)
            {
                LogFactory.Write("���FTP�ؿ�remotePath:" + remotePath + "remFileName�G" + remFileName + "���~�G" + ex.ToString(), GlobalString.LogType.ErrorCategory);
                return null;
            }
        }
        #endregion
        #region �U���ɮ�
        /// <summary>
        /// �U��FTP�ɮ�
        /// </summary>
        /// <param name="remotePath">FTP�l�ؿ��W��</param>
        /// <param name="remFileName">FTP�ɮצW��</param>
        /// <param name="locFileName">Local�ɮצW��</param>
        /// <returns></returns>
        public bool Download(string remotePath, string remFileName, string locPath, string locFileName)
        {
            int readCount;
            byte[] buffer = new byte[bufferSize];
            try
            {
                if (GetFileList(remotePath, remFileName) == null)
                {
                    LogFactory.Write("�ɮ� �����I", GlobalString.LogType.ErrorCategory);
                    return false;
                }
                MKDir(locPath);
                FileStream fs = new FileStream(locPath + locFileName, FileMode.Create);
                if (true)
                {
                    Login(remotePath, remFileName);

                }
                ftp.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)ftp.GetResponse();
                Stream stream = response.GetResponseStream();
                readCount = stream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    fs.Write(buffer, 0, readCount);
                    readCount = stream.Read(buffer, 0, bufferSize);
                }
                stream.Close();
                fs.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            { 
                LogFactory.Write("�U���ɮ� ���ѡG" + ex.ToString(), GlobalString.LogType.ErrorCategory);
                return false;
            }

        }
        /// <summary>
        /// �U��FTP�ɮ�
        /// </summary>
        /// <param name="remFileName"></param>
        /// <param name="locFileName"></param>
        /// <returns></returns>
        public bool Download(string remFileName, string locFileName)
        {
            return Download(remFileName, locPath, locFileName);

        }
        public bool Download(string remFileName, string locPath, string locFileName)
        {
            return Download(remotePath, remFileName, locPath, locFileName);

        }
        #endregion
        #region �Ыإؿ�
        /// <summary>
        /// �Ыإؿ�
        /// </summary>
        /// <param name="FolderPath">�ؿ��W��</param>
        /// <returns></returns>
        public bool MKDir(string FolderPath)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);
                return true;
            }
            catch (Exception ex)
            {
                LogFactory.Write("�ؿ� " + FolderPath + " �ب�ѡG" + ex.ToString(),

GlobalString.LogType.ErrorCategory);
                return false;
            }
        }
        #endregion
        #region �R���ɮ�
        /// <summary>
        /// �R��FTP�ɮ�
        /// </summary>
        /// <param name="remFileName">�ɮצW��</param>
        /// <returns></returns>
        public bool Delete(string remFileName)
        {
            return Delete(remotePath, remFileName);
        }
        /// <summary>
        /// �R��FTP�ɮ�
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="remFileName"></param>
        /// <returns></returns>
        public bool Delete(string remotePath, string remFileName)
        {
            try
            {
                if (true)
                {
                    Login(remotePath, remFileName);
                }
                ftp.Method = WebRequestMethods.Ftp.DeleteFile;
                response = (FtpWebResponse)ftp.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception ex)
            { 
                LogFactory.Write("�R��FTP�ɮ� ���ѡG" + ex.ToString(), GlobalString.LogType.ErrorCategory);
                return false;
            }
        }
        #endregion
    }

}
