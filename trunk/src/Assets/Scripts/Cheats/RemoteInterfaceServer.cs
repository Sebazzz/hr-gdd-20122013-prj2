using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using ThreadPriority = System.Threading.ThreadPriority;

/// <summary>
/// Serves a remote interface via HTTP
/// </summary>
public static class RemoteInterfaceServer {
    private static float _ScreenshotInterval = 0.1f;

    private static bool _Started;
    private static bool _ShutdownPump;
    private static HttpListener _HttpListener;
    private static Thread _WebThread;

    private static readonly object TimerLock = new object();
    private static System.Timers.Timer _ShutdownTimer;
    private static int _UnityThreadId = -1337;

    private static volatile byte[] _ScreenshotBytes;

    /// <summary>
    /// Gets if the server is currently running
    /// </summary>
    public static bool IsRunning {
        get { return _Started; }
    }

    public static void Start(float refreshInterval) {
        // determine first-time unity thread - used for logging
        if (_UnityThreadId < 0) {
            _UnityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        _ScreenshotInterval = refreshInterval;
        _ShutdownPump = false;

        if (!_Started) {
            LogInfo("Starting server, refresh interval: {0}", refreshInterval);

            _WebThread = new Thread(StartInternal);

            _WebThread.Priority = ThreadPriority.Lowest;
            _WebThread.Name = "WorldWideSheep Remote Interface Server Thread";
            _WebThread.IsBackground = true;

            _WebThread.Start();
        }
    }

    /// <summary>
    /// Schedules the server to be shutdown unless <see cref="CancelShutdown"/> is called
    /// </summary>
    /// <param name="numberOfSeconds"></param>
    public static void ScheduleShutdown(float numberOfSeconds) {
        lock (TimerLock) {
            if (_ShutdownTimer != null) {
                _ShutdownTimer = new System.Timers.Timer(numberOfSeconds*1000f);
                _ShutdownTimer.Enabled = true;
                _ShutdownTimer.AutoReset = false;
                _ShutdownTimer.Elapsed += OnServerShutdownTriggered;
                _ShutdownTimer.Start();

                LogInfo("Server shutdown scheduled for: {0}",
                        DateTime.Now.AddSeconds(numberOfSeconds).ToString("HH:mm:ss"));
            }
        }
    }

    private static void OnServerShutdownTriggered(object sender, ElapsedEventArgs e) {
        lock (TimerLock) {
            if (_ShutdownTimer != null) {
                _ShutdownTimer.Enabled = false;
                _ShutdownTimer.Close();
                _ShutdownTimer = null;

                if (_Started) {
                    Shutdown();
                }
            }
        }
    }

    /// <summary>
    /// Cancels scheduled shutdown by <see cref="CancelShutdown"/>
    /// </summary>
    public static void CancelShutdown() {
        lock (TimerLock) {
            if (_ShutdownTimer != null) {
                _ShutdownTimer.Enabled = false;
                _ShutdownTimer.Close();
                _ShutdownTimer = null;

                LogInfo("Scheduled server shutdown canceled");
            }
        }
    }

    public static void Shutdown() {
        lock (TimerLock) {
            if (_Started) {
                LogInfo("Server is being shutdown");

                _Started = false;
                _ShutdownPump = true;

                if (_ShutdownTimer != null) {
                    _ShutdownTimer.Enabled = false;
                    _ShutdownTimer.Close();
                    _ShutdownTimer = null;
                }

                // dispose http listener
                if (_HttpListener != null && _HttpListener.IsListening) {
                    _HttpListener.Stop();
                }
                _HttpListener = null;

                GC.Collect();
            }
        }
    }

    private static void StartInternal(object uselessArgument) {
        try {
            _HttpListener = new HttpListener();
            _HttpListener.Prefixes.Add("http://*:50000/");

            _HttpListener.Start();

            _Started = true;
        }
        catch (Exception ex) {
            CheatNotificationDialog.ShowDialog(
                "Websheep error",
                "Error starting web server: " + ex.Message);
            Shutdown();
            return;
        }

        try {
            Process.Start("http://localhost:50000");
        }
        catch (Exception) {
            CheatNotificationDialog.ShowDialog(
                "Websheep",
                "You can view the web site on: http://localhost:50000");
        }

        try {
            HandleRequests();
        }
        catch (Exception ex) {
            CheatNotificationDialog.ShowDialog(
                "Websheep error",
                "Error receiving requests on web server: " + ex.Message);

            Shutdown();
            return;
        }
    }

    // ReSharper disable EmptyGeneralCatchClause
    private static void HandleRequests() {
        while (_Started) {
            HttpListenerContext ctx = _HttpListener.GetContext();

            HttpListenerRequest request = ctx.Request;
            HttpListenerResponse response = ctx.Response;

            try {
                WebServerContentHandler.HandleRequest(request, response);
            }
            catch (Exception ex) {
                try {
                    response.StatusCode = 500;
                    WebServerContentHandler.WriteSimpleText("Internal Server Error", "<pre>" + ex + "</pre>", response);
                }
                catch (Exception) {
                    // silently continue
                }
            }
        }

        Shutdown();
    }

    // ReSharper restore EmptyGeneralCatchClause

    public static IEnumerator ScreenShotPump() {
        Debug.Log("Starting screenshot pump");

        while (!_ShutdownPump) {
            // wait for graphics to render
            yield return new WaitForEndOfFrame();

            // create a texture to pass to encoding
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // put buffer into texture
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            // split the process up--ReadPixels() and the EncodeToPNG() call inside of the encoder are both pretty heavy
            yield return 0;

            byte[] bytes = texture.EncodeToPNG();
            _ScreenshotBytes = bytes;

            // tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
            Object.DestroyObject(texture);

            yield return new WaitForSeconds(_ScreenshotInterval);
        }
    }

    private static void LogInfo(string info, params object[] parameters) {
        if (Thread.CurrentThread.ManagedThreadId != _UnityThreadId) {
            return;
        }

        Debug.Log(String.Format(info, parameters));
    }

    #region Nested type: WebServerContentHandler

    private static class WebServerContentHandler {
        private const string VirtualImagePath = "/currentImage.dsp";

        private const string HtmlTemplate = @"
<!doctype html>
<html>
     <head>
           <title>{0}</title>

           <link href=""//netdna.bootstrapcdn.com/twitter-bootstrap/2.2.2/css/bootstrap-combined.min.css"" type=""text/css"" rel=""stylesheet"">
           <link href=""//netdna.bootstrapcdn.com/bootswatch/2.1.1/cosmo/bootstrap.min.css"" type=""text/css"" rel=""stylesheet"">

           <style type=""text/css"">
            img#CurrentImage {{
                display:block;
            }}
            
            th {{
                text-align: left;
                font-weight: bold;
                font-size: 1.1em;
                text-decoration: underline;
            }}

            #container {{
                padding: 0 10px 0 10px;
            }}

            
           </style>

           <script type=""text/javascript"" src=""//netdna.bootstrapcdn.com/twitter-bootstrap/2.2.2/js/bootstrap.min.js""></script>
           <script type=""text/javascript"" src=""//ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js""></script>
           <script type=""text/javascript"">
              {2}
           </script>
     </head>
     <body>
         <div id=""container"">
            <div class=""navbar"">
                <div class=""navbar-inner"">
                    <a class=""brand"" href=""/index.dsp"">Shifting Sheep</a>
                    <ul class=""nav"">
                        <li class=""active""><a href=""/index.dsp"">Home</a></li>
                        <li><a href=""/cheats.dsp"">Cheats</a></li>
                        <li><a href=""/info.dsp"">Info</a></li>
                        <li><a href=""/about.dsp"">About</a></li>
                    </ul>
                </div>
            </div>

            {1}
         </div>
     </body>
</html>";

        private static void Page_NotFound(HttpListenerRequest request, HttpListenerResponse response) {
            response.StatusCode = 404;
            WriteSimpleText("Not Found", "Sorry, the page couldn't be found!<br><code>" + request.Url.AbsolutePath + "</code>", response);
        }

        private static void Page_Index(HttpListenerRequest request, HttpListenerResponse response) {
            string imagePath = String.Format("{0}?tl={1}", VirtualImagePath, DateTime.UtcNow.Ticks);

            const string headScriptTemplate = @"
var taskId;

$(document).ready(function() {
    // schedule refresh task
    taskId = window.setInterval(refreshImage, REFRESH_TIME);

    // set image handlers
    $('#CurrentImage').bind('load', onImageLoaded);
    $('#CurrentImage').bind('error', onImageLoadError);
});

function onImageLoaded() {
    var currentDate = new Date();
    var hours = currentDate.getHours();
    var minutes = currentDate.getMinutes();
    var seconds = currentDate.getSeconds();
    var milliseconds = currentDate.getMilliseconds();

    $('#updateTime').text('Updated: ' + hours + ':' + minutes + ':' + seconds + '.' + milliseconds);
}

function onImageLoadError() {
    $('#updateTime').text('Updated: error loading');
}

function refreshImage() {
    var currentDate = new Date();
    var newUrl = 'IMAGEPATH?tl=' + currentDate.getTime();

    $('#CurrentImage').attr('src', newUrl);
}
";
            string headScript = headScriptTemplate
                .Replace("REFRESH_TIME", (_ScreenshotInterval*1000f/2f).ToString(CultureInfo.InvariantCulture))
                .Replace("IMAGEPATH", VirtualImagePath);

            const string htmlTemplate = @"
<h1>Shifting Sheep</h1>
<p>
    <img id=""CurrentImage"" src=""{0}"" alt=""Current Screenshot"" title=""Current Screenshot"">
    <span style=""font-size:8px;"" id=""updateTime""></span>
</p>";
            string html = String.Format(htmlTemplate, imagePath);

            WriteHtmlText("Shifting Sheep", headScript, html, response);
        }

        private static void Page_About(HttpListenerRequest request, HttpListenerResponse response) {
            const string htmlTemplate = @"
<h1>Info</h1>
<p>
    Shifting Sheep version 1.0.
</p>
<p>
    See our <a href=""http://bit.ly/shiftingsheep"">website</a>.
</p>";

            string html = String.Format(htmlTemplate);

            WriteHtmlText("About", String.Empty, html, response);
        }

        private static void Page_Info(HttpListenerRequest request, HttpListenerResponse response) {
            const string htmlTemplate = @"
<h1>Info</h1>
<p>
    Shifting Sheep version 1.0 web client.
</p>
<p>
    Web Client running on port 50000.
</p>
<p>
    Listening on:
    <ul>
        <li><code>http://localhost:50000/</code></li>
        {0}
    </ul>
</p>";
            // collect IP addresses
            string listeningAddresses = String.Empty;
            foreach (NetworkInterface netif in NetworkInterface.GetAllNetworkInterfaces()) {
                IPInterfaceProperties properties = netif.GetIPProperties();

                foreach (UnicastIPAddressInformation ipAddresses in properties.UnicastAddresses) {
                    if (ipAddresses.Address.AddressFamily != AddressFamily.InterNetwork) {
                        continue;
                    }

                    const string template = "<li><code>http://{0}:50000/</code></li>";
                    listeningAddresses += String.Format(template, ipAddresses.Address);
                }
            }

            string html = String.Format(htmlTemplate, listeningAddresses);

            WriteHtmlText("Info", String.Empty, html, response);
        }

        private static void Page_Cheats(HttpListenerRequest request, HttpListenerResponse response) {
            const string htmlTemplate = @"
<h1>Cheats reference<h1>
<h2>Commands overview</h2>
{0}

<h2>Variables overview</h2>
{1}";

            StringBuilder cheatPageBuilder = new StringBuilder();

            // cheat commands overview
            cheatPageBuilder.Append("<table>");
            cheatPageBuilder.Append("<tr><th>Command</th><th>Parameters</th><th>Description</th></tr>");

            Dictionary<string, List<CheatCommandDescriptor>> overviewData = CheatService.GetAllCommandsByHumanReadableCategory();
            bool first = true;
            foreach (KeyValuePair<string, List<CheatCommandDescriptor>> cheatsByCategory in overviewData) {
                if (!first) {
                    cheatPageBuilder.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");
                }
                cheatPageBuilder.AppendFormat("<tr><th colspan=\"3\" style=\"text-decoration:none;\">{0}</td></tr>", cheatsByCategory.Key);

                foreach (CheatCommandDescriptor commandDescriptor in cheatsByCategory.Value) {
                    cheatPageBuilder.Append("<tr>");
                    cheatPageBuilder.AppendFormat("<td>{0}</td>", commandDescriptor.Name);

                    cheatPageBuilder.Append("<td>");
                    foreach (ParameterInfo parameter in commandDescriptor.Parameters) {
                        string typeName = parameter.ParameterType.Name.ToLowerInvariant();

                        if (parameter.ParameterType == typeof(double) || parameter.ParameterType == typeof(float)) {
                            typeName = "decimal";
                        } else if (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(long)) {
                            typeName = "integer";
                        }

                        cheatPageBuilder.AppendFormat("<span title=\"{1}\">&lt;{0}&gt;</span> ", parameter.Name, typeName);
                    }
                    cheatPageBuilder.Append("</td>");

                    cheatPageBuilder.AppendFormat("<td>{0}</td>", commandDescriptor.Description);
                    cheatPageBuilder.Append("</tr>");
                }

                first = false;
            }
            cheatPageBuilder.Append("</table>");
            
            string commandsOverview = cheatPageBuilder.ToString();

            // cheat variables
            cheatPageBuilder = new StringBuilder();
            cheatPageBuilder.Append("<table>");
            cheatPageBuilder.Append("<tr><th>Name</th><th>Value</th><th>Description</th></tr>");

            IEnumerable<CheatVariabeleDescriptor> variablesOverviewData = CheatService.GetAllVariabeles();
            foreach (CheatVariabeleDescriptor variabeleDescriptor in variablesOverviewData) {
                cheatPageBuilder.AppendFormat(CultureInfo.InvariantCulture,
                                              "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                                              variabeleDescriptor.Name, 
                                              variabeleDescriptor.FieldInfo.GetValue(null),
                                              variabeleDescriptor.Description);
            }

            string variablesOverview = cheatPageBuilder.ToString();
            cheatPageBuilder = null;

            // create HTML
            string html = String.Format(htmlTemplate, commandsOverview, variablesOverview);

            WriteHtmlText("Cheats", String.Empty, html, response);
        }

        private static void Page_CurrentImage(HttpListenerRequest request, HttpListenerResponse response) {
            byte[] screenshot = _ScreenshotBytes;

            // get a response stream and write the response to it.
            response.ContentType = "image/png";
            response.ContentLength64 = screenshot.Length;
            Stream output = response.OutputStream;
            output.Write(screenshot, 0, screenshot.Length);

            // close the output stream.
            output.Close();
        }

        private static void Page_Unavailable(HttpListenerRequest request, HttpListenerResponse response) {
            response.StatusCode = 503;
            WriteSimpleText("Not available", "Sorry, no content available yet!", response);
        }

        #region Helpers

        private static void WriteHtmlText(string title, string headScript, string bodyContents, HttpListenerResponse response) {
            // construct a response.
            string responseString = String.Format(HtmlTemplate, title, bodyContents, headScript);

            WriteString(response, responseString);
        }

        public static void WriteSimpleText(string title, string text, HttpListenerResponse response) {
            // construct a response.
            const string responseStringTemplate = @"
        <h1>{0}</h1>
        <p>{1}</p>";

            string innerHtml = String.Format(responseStringTemplate, title, text);

            WriteHtmlText(title, String.Empty, innerHtml, response);
        }

        private static void WriteString(HttpListenerResponse response, string responseString) {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            // get a response stream and write the response to it.
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // close the output stream.
            output.Close();
        }

        #endregion

        #region Request Handling

        public static void HandleRequest(HttpListenerRequest request, HttpListenerResponse response) {
            // check available
            if (_ScreenshotBytes == null) {
                Page_Unavailable(request, response);
                return;
            }

            // prepare request
            response.StatusCode = 200;
            response.Headers.Add(HttpResponseHeader.Pragma, "no-cache");
            response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache, no-store");
            response.Headers.Add("X-Powered-By", "Shifting Servers v1.0");

            // execute request
            Uri url = request.Url;
            ProcessRequestForPath(url.AbsolutePath, request, response);
        }

        private static void ProcessRequestForPath(string absolutePath, HttpListenerRequest request, HttpListenerResponse response) {
            // index?
            if (String.IsNullOrEmpty(absolutePath) || absolutePath == "/") {
                Page_Index(request, response);
                return;
            }

            // find the correct page
            foreach (KeyValuePair<string, Action<HttpListenerRequest, HttpListenerResponse>> pageReg in Pages) {
                if (absolutePath.EndsWith(pageReg.Key, StringComparison.InvariantCultureIgnoreCase)) {
                    // found page, execute handler
                    pageReg.Value.Invoke(request, response);
                    return;
                }
            }

            // no page found, return 404
            Page_NotFound(request, response);
        }

        private static readonly Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>> Pages;

        static WebServerContentHandler() {
            Pages = new Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>>();

            Pages.Add("index.dsp", Page_Index);
            Pages.Add(VirtualImagePath.Substring(1), Page_CurrentImage);
            Pages.Add("about.dsp", Page_About);
            Pages.Add("info.dsp", Page_Info);
            Pages.Add("cheats.dsp", Page_Cheats);

            Pages.Add("error503.dsp", Page_Unavailable);
            Pages.Add("error404.dsp", Page_NotFound);
        }

        #endregion
    }

    #endregion
}