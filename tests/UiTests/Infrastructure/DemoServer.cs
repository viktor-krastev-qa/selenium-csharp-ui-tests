using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UiTests.Infrastructure;

public sealed class DemoServer : IDisposable
{
    private readonly HttpListener listener = new();
    private readonly Task serverTask;
    private readonly string root = Path.Combine(AppContext.BaseDirectory, "demo-app");
    public string Url { get; }

    public DemoServer()
    {
        // Reserve a free local port; retry if another process claims it after release.
        for (var attempt = 0; ; attempt++)
        {
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            var port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();
            Url = $"http://localhost:{port}/";
            listener.Prefixes.Clear(); listener.Prefixes.Add(Url);
            try { listener.Start(); break; }
            catch (HttpListenerException) when (attempt < 4) { }
        }
        serverTask = Task.Run(ServeAsync);
    }

    private async Task ServeAsync()
    {
        while (listener.IsListening)
        {
            HttpListenerContext context;
            try { context = await listener.GetContextAsync(); }
            catch (HttpListenerException) { break; }
            catch (ObjectDisposedException) { break; }
            try
            {
                var name = context.Request.Url?.AbsolutePath switch
                {
                    "/" => "index.html", "/app.js" => "app.js", "/style.css" => "style.css", _ => null
                };
                if (name is null) { context.Response.StatusCode = 404; }
                else
                {
                    context.Response.ContentType = name.EndsWith(".js") ? "application/javascript" : name.EndsWith(".css") ? "text/css" : "text/html; charset=utf-8";
                    var bytes = await File.ReadAllBytesAsync(Path.Combine(root, name));
                    context.Response.ContentLength64 = bytes.Length;
                    await context.Response.OutputStream.WriteAsync(bytes);
                }
            }
            catch (Exception) { context.Response.StatusCode = 500; }
            finally { context.Response.Close(); }
        }
    }

    public void Dispose()
    {
        listener.Stop(); listener.Close();
        serverTask.Wait(TimeSpan.FromSeconds(5));
    }
}
