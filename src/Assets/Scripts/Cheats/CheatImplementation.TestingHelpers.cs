using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;
using ThreadPriority = System.Threading.ThreadPriority;

// Cheats implementation: Functional
public static partial class CheatImplementation {
    [CheatCommand("ClearSettings", CheatCategory.TestingHelpers)]
    public static void ClearAllSettings() {
        PlayerPrefs.DeleteAll();

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    [CheatCommand("BeTheBest", CheatCategory.TestingHelpers)]
    public static void SetAllLevelsFullyPlayed() {
        Level currentLevel = Levels.GetFirstLevel();

        while (currentLevel != Level.None) {
            currentLevel.SetFinished();

            currentLevel = Levels.GetNextLevel(currentLevel);
        }

        AsyncSceneLoader.Load(Scenes.MainMenu);
    }

    [CheatCommand("Reload", CheatCategory.TestingHelpers)]
    public static void ReloadTheCurrentLevel() {
        AsyncSceneLoader.Load(Application.loadedLevelName);
    }

    [CheatCommand("ForceEnemyWave", CheatCategory.TestingHelpers)]
    public static void ForceEnemyWavesToBeSpawned() {
        GameObject[] emitters = GameObject.FindGameObjectsWithTag(Tags.Emitter);

        foreach (GameObject emitter in emitters) {
            EmitterBehaviour emitterBehaviour = emitter.GetComponent<EmitterBehaviour>();

            if (emitterBehaviour != null && emitterBehaviour.enabled) {
                emitterBehaviour.ForceWave();
            }
        }
    }

    [CheatCommand("WorldWideSheepConfig", CheatCategory.TestingHelpers)]
    public static void StartAWebServerWithConfigurationShowingGameScreenShots(float refreshIntervalSeconds) {
        Application.runInBackground = true;

        WebServer.Start(refreshIntervalSeconds);
        StartCoroutine(WebServer.ScreenShotPump());
    }

    [CheatCommand("WorldWideSheep", CheatCategory.TestingHelpers)]
    public static void StartAWebServerShowingGameScreenShots() {
        StartAWebServerWithConfigurationShowingGameScreenShots(0.250f);
    }

    private static class WebServer {
        private const string VirtualImagePath = "/current.png";
        private static float _ScreenshotInterval = 0.1f;

        private static bool _Started;
        private static HttpListener _HttpListener;
        private static Thread _WebThread;

        private static DateTime _ScreenshotTime;
        private static volatile byte[] _ScreenshotBytes;

        public static void Start(float refreshInterval) {
            _ScreenshotInterval = refreshInterval;

            if (!_Started) {
                _WebThread = new Thread(StartInternal);

                _WebThread.Priority = ThreadPriority.Lowest;
                _WebThread.Name = "WorldWideSheep Thread";
                _WebThread.IsBackground = true;
                _WebThread.SetApartmentState(ApartmentState.STA);

                _WebThread.Start();
            }
        }

        public static void Shutdown() {
            if (_Started) {
                _Started = false;

                // dispose http listener
                if (_HttpListener != null && _HttpListener.IsListening) {
                    _HttpListener.Stop();
                }
                _HttpListener = null;

                GC.Collect();
            }
        }

        private static void StartInternal(object uselessArgument) {
            try {
                _HttpListener = new HttpListener();
                _HttpListener.Prefixes.Add("http://*:50000/");

                _HttpListener.Start();

                _Started = true;
            } catch (Exception ex) {
                CheatNotificationDialog.ShowDialog(
                    "Websheep error",
                    "Error starting web server: " + ex.Message);
                Shutdown();
                return;
            }

            try {
                Process.Start("http://localhost:50000");
            } catch (Exception) {
                CheatNotificationDialog.ShowDialog(
                    "Websheep",
                    "You can view the web site on: http://localhost:50000");
            }

            try {
                HandleRequests();
            } catch (Exception ex) {
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
                    // handle requests synchronous: asynchronous does not work!
                    HandleRequest(request, response);
                } catch (Exception ex) {
                    try {
                        WriteSimpleText("Error", ex.ToString(), response);
                    } catch(Exception) {
                        // silently continue
                    }
                }

                // check for idle
                if (DateTime.UtcNow - _ScreenshotTime > TimeSpan.FromSeconds(5)) {
                    break;
                }
            }

            Shutdown();
        }
        // ReSharper restore EmptyGeneralCatchClause

        private static void HandleRequest(HttpListenerRequest request, HttpListenerResponse response) {
            // check available
            if (DateTime.UtcNow - _ScreenshotTime > TimeSpan.FromSeconds(5)) {
                ProcessUnavailableRequest(response);
                return;
            }

            // prepare request
            response.StatusCode = 200;
            response.Headers.Add(HttpResponseHeader.Pragma, "no-cache");
            response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache, no-store");
            response.Headers.Add("X-Powered-By", "Shifting Servers v1.0");

            // execute request
            Uri url = request.Url;
            if (url.AbsolutePath.EndsWith(VirtualImagePath)) {
                ProcessImageRequest(response);
            } else {
                ProcessHtmlRequest(request, response);
            }
        }

        private static void ProcessHtmlRequest(HttpListenerRequest request, HttpListenerResponse response) {
            string imagePath = String.Format("{0}?tl={1}", VirtualImagePath, DateTime.UtcNow.Ticks);

            const string headScriptTemplate = @"
var taskId;

$(document).ready(function() {
    
    taskId = window.setInterval(refreshImage, REFRESH_TIME);

});

function refreshImage() {
    var currentDate = new Date();
    var newUrl = ""IMAGEPATH?tl="" + currentDate.getTime();

    $(""#CurrentImage"").attr(""src"", newUrl);
}
";
            string headScript = headScriptTemplate
                .Replace("REFRESH_TIME", (_ScreenshotInterval*1000f/2f).ToString(CultureInfo.InvariantCulture))
                .Replace("IMAGEPATH", VirtualImagePath);

            const string htmlTemplate = @"
<img id=""CurrentImage"" src=""{0}"" alt=""Current Screenshot"" title=""Current Screenshot"">";
            string html = String.Format(htmlTemplate, imagePath);

            WriteHtmlText("Shifting Sheep", headScript, html, response);
        }

        private static void ProcessImageRequest(HttpListenerResponse response) {
            byte[] screenshot = _ScreenshotBytes;

            // get a response stream and write the response to it.
            response.ContentType = "image/png";
            response.ContentLength64 = screenshot.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(screenshot, 0, screenshot.Length);

            // close the output stream.
            output.Close();
        }

        private static void ProcessUnavailableRequest(HttpListenerResponse response) {
            response.StatusCode = 500;
            WriteSimpleText("Not available", "Sorry, no content available yet!", response);
        }

        private static void WriteHtmlText(string title, string headScript, string bodyContents, HttpListenerResponse response) {
            // construct a response.
            const string responseStringTemplate = @"
<!doctype html>
<html>
     <head>
           <title>{0}</title>

           <script type=""text/javascript"" src=""//ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js""></script>
           <script type=""text/javascript"">
              {2}
           </script>
     </head>
     <body>
         {1}
     </body>
</html>";
            string responseString = String.Format(responseStringTemplate, title, bodyContents, headScript);

            WriteString(response, responseString);
        }

        private static void WriteSimpleText(string title, string text, HttpListenerResponse response) {
            // construct a response.
            const string responseStringTemplate = @"
<!doctype html>
<html>
     <head>
           <title>{0}</title>
     </head>
     <body>
        <h1>{0}</h1>
        <p>{1}</p>
     </body>
</html>";
            string responseString = String.Format(responseStringTemplate, title, text);

            WriteString(response, responseString);
        }

        private static void WriteString(HttpListenerResponse response, string responseString) {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // get a response stream and write the response to it.
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // close the output stream.
            output.Close();
        }

        public static IEnumerator ScreenShotPump() {
            while (_Started) {
                // wait for graphics to render
                yield return new WaitForEndOfFrame();

                // create a texture to pass to encoding
                Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

                // put buffer into texture
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();

                // split the process up--ReadPixels() and the EncodeToPNG() call inside of the encoder are both pretty heavy
                yield return 0;

                byte[] bytes = texture.EncodeToPNG();
                _ScreenshotBytes = bytes;
                _ScreenshotTime = DateTime.UtcNow;

                // tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
                Object.DestroyObject(texture);

                yield return new WaitForSeconds(_ScreenshotInterval);
            }

            yield break;
        }  

    }
}