using System.Net;
using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Vehicles;

public class HttpPlugin : AssemblyPluginBase
{
    private HttpListener _httpListener;
    private IVehicle vehicle;

    public HttpPlugin(PluginBuilder builder) : base(builder)
    {
        _httpListener = new HttpListener();
        string port = File.ReadAllText("host.txt").Split('=')[1];
        _httpListener.Prefixes.Add($"http://localhost:{port}/");
        _httpListener.Start();
    }

    public override void Tick(TimeSpan elapsed)
    {
        var speed = vehicle.State.Speed;
        var notch = vehicle.State.PowerNotch;
        
        if (_httpListener.IsListening)
        {
            HttpListenerContext context = _httpListener.GetContext();
            HttpListenerResponse response = context.Response;

            string responseString = $"{{\"speed\":{speed}, \"notch\":{notch}}}";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}
