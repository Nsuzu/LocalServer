// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;

Task acceptTask = Task.Run(() => AcceptInput());

IPAddress address = IPAddress.Parse("127.0.0.1");
TcpListener server = new TcpListener(address, 80);
server.Start();

while (true) {
    Console.WriteLine("waiting...");
    TcpClient client = await server.AcceptTcpClientAsync();

    NetworkStream stream = client.GetStream();
    try {
        string root = @"C:\Users\nsjag\OneDrive\Programs\testWeb";

        using (StreamReader sr = new StreamReader(stream))
        using (StreamWriter sw = new StreamWriter(stream)){
            string? reqLine = sr.ReadLine();

            string path = GetRequestedFile(reqLine);
            string content = await GetFileContent(root + path);

            Console.WriteLine(reqLine);
            while (sr.Peek() > -1) {
                Console.WriteLine(sr.ReadLine());
            }

            
            Console.WriteLine("writing start");

            sw.WriteLine("HTTP/1.1 200 OK");
            sw.WriteLine($"Content-Type: {GetContentType(path)}; charset=UTF-8");
            sw.WriteLine("");

            if (path.Contains("jpg") || path.Contains("png")) {
                byte[] bytes = GetImageContent(root + path);
                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write(bytes);
                bw.Close();

                foreach(byte b in bytes) {
                    Console.Write(b);
                }
            } else {
                sw.Write(content);
            }
              

        }
        
    } catch (Exception ex) {
        Console.Write($"errror in whole process {ex}\n");
    }

}

string GetRequestedFile(string? str) {
    if (str == null || !str.Contains("GET")) return "";

    string[] keys = { "GET ", " HTTP" };
    string path = str.Split(keys, StringSplitOptions.None)[1];
    if (path.EndsWith('/')) path += "index.html";

    return path;
}

async Task<string> GetFileContent(string filePath) {
    string content;
    try {
        using (StreamReader reader = new StreamReader(filePath)) {
            content = await reader.ReadToEndAsync() + "\n";

        }
    } catch (Exception e) {
        content = e.ToString() + "\n";
    }

    return content;
}

byte[] GetImageContent(string filePath) {
    FileStream stream = File.OpenRead(filePath);
    byte[] bytes;
    using (BinaryReader reader = new BinaryReader(stream)) {
        bytes = reader.ReadBytes((int)stream.Length);
    }
    return bytes;
}

string GetContentType(string path) {
    if (!path.Contains('.')) return "text/plain";

    string extension = path.Split('.')[1];

    string contentType = extension switch {
        "html" => "text/html",
        "css" => "text/css",
        "jpg" => "image/jpeg",
        "png" => "image/png",
        _ => "text/plain"
    };

    return contentType;
}

void AcceptInput() {
    while (true) {
        string? str = Console.ReadLine();
        if (str == "exit") Environment.Exit(0);
    }
}
