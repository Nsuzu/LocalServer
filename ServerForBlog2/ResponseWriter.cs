using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace LocalServer {
    internal class ResponseWriter {
        private NetworkStream netStream;
        public ResponseWriter(NetworkStream stream) {
            netStream = stream;
        }    

        public void WriteImageContent(string path, BinaryWriter bw) {
            FileStream fileStream = File.OpenRead(path);
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(fileStream)) {
                bytes = br.ReadBytes((int)fileStream.Length);
            }

            /*using (BinaryWriter bw = new BinaryWriter(netStream)) {
                bw.Write(bytes);
            }*/
            bw.Write(bytes);
        }

        public void WriteFileContent(string path, StreamWriter sw) {
            string content;
            using (StreamReader sr = new StreamReader(path)) {
                content = sr.ReadToEnd();
            }

            /*using (StreamWriter sw = new StreamWriter(netStream)) {
                sw.Write(content);
            }*/
            sw.Write(content);
        }
    }
}
