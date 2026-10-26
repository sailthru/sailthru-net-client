using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Sailthru;
using Sailthru.Models;

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

// Both assemblies must come from the .nupkg, including its embedded Models DLL.
foreach (Assembly assembly in new[] { typeof(SailthruClient).Assembly, typeof(UserRequest).Assembly })
{
    string packagedFile = Path.Combine(args[0], Path.GetFileName(assembly.Location));
    Check(File.ReadAllBytes(assembly.Location).SequenceEqual(File.ReadAllBytes(packagedFile)),
        $"Assembly was not loaded from the built package: {assembly.GetName().Name}");
}

using var listener = new TcpListener(IPAddress.Loopback, 0);
listener.Start();
int port = ((IPEndPoint)listener.LocalEndpoint).Port;
Task server = Task.Run(async () =>
{
    using TcpClient connection = await listener.AcceptTcpClientAsync();
    await using NetworkStream stream = connection.GetStream();
    using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
    Check(await reader.ReadLineAsync() == "POST /user HTTP/1.1", "Unexpected request method or path");
    int length = 0;
    string? header;
    while (!string.IsNullOrEmpty(header = await reader.ReadLineAsync()))
    {
        if (header.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
            length = int.Parse(header.Split(':', 2)[1]);
    }
    Check(length > 0 && length < 10000, "Unexpected request size");
    char[] body = new char[length];
    Check(await reader.ReadBlockAsync(body, 0, body.Length) == length, "Incomplete request body");
    var form = new string(body).Split('&').Select(part => part.Split('=', 2))
        .ToDictionary(part => WebUtility.UrlDecode(part[0]), part => WebUtility.UrlDecode(part[1]));
    Check(form["api_key"] == "smoke-key", "Unexpected API key");
    Check(Regex.IsMatch(form["sig"], "^[a-f0-9]{32}$"), "Signature was not generated");
    using JsonDocument payload = JsonDocument.Parse(form["json"]);
    Check(payload.RootElement.GetProperty("id").GetString() == "smoke@example.invalid", "Unexpected user payload");
    const string response = "{\"id\":\"smoke@example.invalid\",\"ok\":true}";
    byte[] bytes = Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {Encoding.UTF8.GetByteCount(response)}\r\nConnection: close\r\n\r\n{response}");
    await stream.WriteAsync(bytes);
});

try
{
    var client = new SailthruClient("smoke-key", "smoke-secret", $"http://127.0.0.1:{port}")
    {
        Timeout = 5000,
        ReadWriteTimeout = 5000
    };
    SailthruResponse response = client.SetUser(new UserRequest
    {
        Id = "smoke@example.invalid",
        Vars = new Hashtable { ["source"] = "package-smoke" }
    });
    Check(response.IsOK(), "Client did not accept the response");
    Check((string?)response.HashtableResponse["id"] == "smoke@example.invalid", "Unexpected response payload");
    await server.WaitAsync(TimeSpan.FromSeconds(10));
    Console.WriteLine($"Built NuGet package smoke check passed on .NET {Environment.Version}");
}
finally
{
    listener.Stop();
}
