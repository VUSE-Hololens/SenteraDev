using System;
using System.IO;
using System.Net;
using HtmlAgilityPack; // for fetching image file url 
using System.Threading.Tasks;
using UnityEngine;
#if !UNITY_EDITOR && UNITY_WSA
using Windows.Devices.WiFi;
using Windows.Storage;
#endif
using CI.HttpClient;
using System.Drawing;
using System.Drawing.Imaging;

public class SenteraController : MonoBehaviour
{
    public ImageProjectionManager imageManager;

    public static JpegImage latestRGBImage = null;
    public static JpegImage latestNIRImage = null;

    public SenteraController()
    {
        imageManager = new ImageProjectionManager();
        Debugger.printToDebug("initializing senteraController");
        //Debugger.changeCubeColor(UnityEngine.Color.blue);
    }

    // this method should be deleted in the end 
    void printToDebug(string message)
    {
        //this.GetComponent<UnityEngine.UI.Text>().text = message;
    }

    // returns whether we are successfullly connected or not
    async public Task<bool> ConnectToSenteraWifi()
    {
#if !UNITY_EDITOR && UNITY_WSA
        // request access
        var access = await WiFiAdapter.RequestAccessAsync();
        if (access == WiFiAccessStatus.Allowed) {
            printToDebug("Wifi Access allowed");
        } else {
            printToDebug("Wifi Access not allowed");
        }

        // get adapter so we can connect
        var wifiAdapterResult = await WiFiAdapter.FindAllAdaptersAsync(); 
        var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

        if (result.Count >= 1) 
        {
            var firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id); // get our adapter
            printToDebug("Wifi Adapter found");

            // check if we are already connected
            var connectedProfile = await firstAdapter.NetworkAdapter.GetConnectedProfileAsync();
            if (connectedProfile != null) {
                if (connectedProfile.ProfileName.Contains("Sentera")) {
                    return true;
                }
            }

            // if we are here, we are not connected to sentera
            firstAdapter.Disconnect(); // disconnect first so we don't get struck at "connecting to VuNet"
            var report = firstAdapter.NetworkReport; // get a list of available networks
            foreach (var network in report.AvailableNetworks) {
                if (network.Ssid.Contains("Sentera")) {
                    printToDebug("Sentera found");
                    await firstAdapter.ConnectAsync(network, WiFiReconnectionKind.Automatic);
                    printToDebug("Sentera connected. Going to freeze for 5 seconds so Sentera can set up its website");
                    await Task.Delay(5000); 
                    printToDebug("We are back now");
                    return true;
                }
            }
        } 
        else 
        {
            printToDebug("Wifi adapter not found");
        }
#endif
        return false;
    }

    // the methods and classes below are just helpers for starting Sentera Session

    [System.Serializable]
    class StartCommand
    {
        public string command = "start";
        public string session = "test";
        public int time = 1;
        public bool append = false;
    }

    [System.Serializable]
    class StopCommand
    {
        public string command = "stop";
    }

    // ### this method is problematic, we need to fetch /api/v1/session in order to know whether the sentera is really started or not ###
    // ### right now we just assume that if we send the request, it is started ###
    // "start", "stop"
    // returns whether command is successful
    public bool command(string command)
    {
        bool commandSuccessful = false;
        HttpClient client = new HttpClient();
        string commandString = "";
        if (command == "start")
        {
            StartCommand startCmd = new StartCommand();
            commandString = JsonUtility.ToJson(startCmd);
        }
        else if (command == "stop")
        {
            StopCommand stopCmd = new StopCommand();
            commandString = JsonUtility.ToJson(stopCmd);
        }
        else
        {
            return commandSuccessful; 
        }
        

        client.Put(
            new System.Uri("http://192.168.143.141:8080/api/v1/session"), 
            new StringContent(commandString, System.Text.Encoding.UTF8, "application/json"), 
            HttpCompletionOption.AllResponseContent, 
            (r) =>
                { 
                    string responseData = r.ReadAsString(); 
                    Debug.Log(responseData); 
                    printToDebug(responseData); 
                    if (responseData == "")
                    {
                        commandSuccessful = true;
                    }
                }, 
            (u) => { }
        );

        return true;
    }

    public Texture2D GetWebpageTexture(string url)
    {
        WWW www = new WWW(url);
        return www.texture;
    }

    public void FetchLatestImage(JpegImage.ImageType type)
    {
        HttpClient client = new HttpClient();
        string url = getUrlStringForImageType(type);
        client.Get(new Uri(url), HttpCompletionOption.AllResponseContent, async (r) =>
        {
            imageManager.debugMessage = "Fetching Image";
            byte[] data = r.ReadAsByteArray();
            imageManager.debugMessage = "data read";
            imageManager.debugMessage = "data size is " + r.PercentageComplete.ToString() + type.ToString();
            //imageManager.debugMessage = "status code is: " + r.StatusCode.ToString();
            //imageManager.debugMessage = "isSuccessStatusCode: " + r.IsSuccessStatusCode.ToString();
            JpegImage jpeg = new JpegImage(data, type, url);
            //imageManager.debugMessage = "jpeg created. Data size is " + data.Length.ToString();
            imageManager.updateLatestImage(jpeg);
            //imageManager.debugMessage = type.ToString() + "Image Fetched";
            //imageManager.debugMessage = data[2000].ToString();
        });
    }

    public void FetchLatestImages()
    {
        FetchLatestImage(JpegImage.ImageType.RGB);
        FetchLatestImage(JpegImage.ImageType.NIR);
    }

    private string getUrlForLatestImagePage(JpegImage.ImageType type)
    {
        switch (type)
        {
            case JpegImage.ImageType.RGB:
                return "http://192.168.143.141/last_img?lnk=li0&camera=1";
            case JpegImage.ImageType.NIR:
                return "http://192.168.143.141/last_img?lnk=li0&camera=2";
            default:
                return "";
        }
    }

    private string getUrlStringForImageType(JpegImage.ImageType type)
    {
        string latestImagePageUrl = getUrlForLatestImagePage(type);
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = new HtmlDocument();
        doc = web.Load(latestImagePageUrl);
        int count = 0;
        string result = "";
        foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
        {
            HtmlAttribute att = link.Attributes["href"];

            if (att.Value.Contains("sdcard?path"))
            {
                result = "http://192.168.143.141" + att.Value;
                count += 1;
            }
        }
        
        if (count == 1)
        {
            imageManager.debugMessage = result;
            return result;
        }
        if (count == 0)
        {
            return "url not found";
        }
        return "multiple url found";
    }

}
