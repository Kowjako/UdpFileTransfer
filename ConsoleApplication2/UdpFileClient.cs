using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApplication2
{
    public class UdpFileClient
    {
        [Serializable]
        public class FileDetails
        {
            public string FILETYPE = "";
            public long FILESIZE = 0;
        }
        private static FileDetails fileDet;
        private static int localPort = 5002;
        private static UdpClient receivingUdpClient = new UdpClient(localPort);
        private static IPEndPoint RemoteIpEndPoint = null;
        private static FileStream fs;
        private static byte[] receiveBytes = new byte[0]; //accepted bytes
        static void Main()
        {
            GetFileDetails();
            ReceiveFile();
        }
        private static void GetFileDetails()
        {
            try
            {
                Console.WriteLine("Waiting for file details...");
                receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                Console.WriteLine("File is sucessfuly read");
                XmlSerializer xs = new XmlSerializer(typeof(FileDetails));
                MemoryStream ms = new MemoryStream();
                ms.Write(receiveBytes, 0, receiveBytes.Length);
                ms.Position = 0;
                fileDet = (FileDetails)xs.Deserialize(ms);
                Console.WriteLine($"Accepted file type: {fileDet.FILETYPE}, file size: {fileDet.FILESIZE}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void ReceiveFile()
        {
            try
            {
                Console.WriteLine("Waiting for file...");
                receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                Console.WriteLine("Saving file on PC...");
                fs = new FileStream("temp." + fileDet.FILETYPE, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Write(receiveBytes, 0, receiveBytes.Length);
                Console.WriteLine("File is saved... Opening");
                Process.Start(fs.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                fs.Close();
                receivingUdpClient.Close();
                Console.ReadKey();
            }
        }
    }
}
