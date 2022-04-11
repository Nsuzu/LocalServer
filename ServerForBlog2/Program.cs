// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using LocalServer;

Task acceptTask = Task.Run(() => AcceptInput());
Setting? setting = new Setting();

try {
    using (FileStream stream = File.OpenRead("setting.json")) {
        setting = await JsonSerializer.DeserializeAsync<Setting>(stream);
    }
} catch {
    Console.WriteLine("failed to load \"setting.json\"");
    return;
}

if (setting == null) {
    Console.WriteLine("failed to initialize...");
    return;
}

Console.WriteLine($"{setting.IPAddress}");
IPAddress address = IPAddress.Parse(setting.IPAddress);
TcpListener server = new TcpListener(address, 80);
server.Start();
//This is main!


while (true) {
    Console.WriteLine("waiting...");
    TcpClient client = await server.AcceptTcpClientAsync();

    NetworkStream stream = client.GetStream();
    ResponseWriter responseWriter = new ResponseWriter(stream);
    try {
        string root = setting.RootDir;

        using (StreamReader sr = new StreamReader(stream)) {
            StreamWriter sw = new StreamWriter(stream);
            string? reqLine = sr.ReadLine();

            string path = GetRequestedFile(reqLine);

            Console.WriteLine(reqLine);
            while (sr.Peek() > -1) {
                Console.WriteLine(sr.ReadLine());
            }

            Console.WriteLine("writing start");
            string contentType = GetContentType(path);

            sw.WriteLine("HTTP/1.1 200 OK");
            sw.WriteLine($"Content-Type: {contentType}; charset=UTF-8");
            sw.WriteLine("");

            if (contentType.Contains("image")) {
                using (BinaryWriter bw = new BinaryWriter(stream)) {
                    responseWriter.WriteImageContent(root + path, bw);
                }

            } else if (contentType.Contains("html") || contentType.Contains("css")) {
                responseWriter.WriteFileContent(root + path, sw);
                sw.Close();

            } else {
                Console.WriteLine($"unexpected: {contentType}");
                sw.Close();
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
