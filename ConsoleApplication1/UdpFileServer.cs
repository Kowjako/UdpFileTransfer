using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
    public class UdpFileServer
    {
        [Serializable]
        public class FileDetails
        {
            public string FILETYPE = "";
            public long FILESIZE = 0;
        }
        public static FileDetails fileDet = new FileDetails();
        static IPEndPoint endPoint;
        static IPAddress remoteAddress;
        static int remotePort = 5002;
        private static UdpClient sender = new UdpClient();
        private static FileStream fs;
        [STAThread]
        static void Main()
        {
            try
            {
                Console.WriteLine("Write ip of server");
                remoteAddress = IPAddress.Parse(Console.ReadLine().ToString());
                endPoint = new IPEndPoint(remoteAddress, remotePort);
                Console.WriteLine("Write path of file");
                fs = new FileStream(@Console.ReadLine().ToString(), FileMode.Open, FileAccess.Read);
                if (fs.Length > 8156)
                {
                    Console.WriteLine("File size is unreadable");
                    sender.Close();
                    fs.Close();
                    return;
                }
                SendFileInfo();
                SendFile();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
        static void SendFileInfo()
        {
            fileDet.FILETYPE = fs.Name.Substring((int)fs.Name.Length - 3, 3);
            fileDet.FILESIZE = fs.Length;
            XmlSerializer xs = new XmlSerializer(typeof(FileDetails));
            MemoryStream ms = new MemoryStream();
            xs.Serialize(ms, fileDet);
            ms.Position = 0;
            Byte[] data = new Byte[ms.Length];
            ms.Read(data, 0, Convert.ToInt32(ms.Length));
            Console.WriteLine("Sending file data is completed");
            sender.Send(data, data.Length, endPoint);
            ms.Close();
        }
        static void SendFile()
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            Console.WriteLine("Sending file...");
            try
            {
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                fs.Close();
                sender.Close();
            }
            Console.WriteLine("File has been sent sucessful");
            Console.Read();
        }
    }
}
