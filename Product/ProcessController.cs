using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;

public class ProcessController : MonoBehaviour
{
    double startTime;
    double lastUpdateTime;
    SenteraController sentera;

    void printToDebug(string message)
    {
        this.GetComponent<UnityEngine.UI.Text>().text = message;
    }

    // Start is called before the first frame update
    async void Start()
    {
        //Debugger.changeCubeColor(UnityEngine.Color.red);
        sentera = new SenteraController();
        startTime = Time.time;
        lastUpdateTime = Time.time;
        // connect to Sentera Wifi first
        printToDebug("Trying to connect to sentera wifi");
        bool connectedToSenteraWifi = await sentera.ConnectToSenteraWifi();
        if (!connectedToSenteraWifi)
        {
            printToDebug("Not connected to sentera wifi");
            return;
        }
        else
        {
            printToDebug("Connected to sentera wifi. Trying to start sentera session now");
        }

        // start a session first
        bool senteraStarted = sentera.command("start");
        if (senteraStarted)
        {
            printToDebug("Sentera session started");
        } 
        else
        {
            printToDebug("Sentera session not started");
            return;
        }

        printToDebug("We are going to sleep for 10 seconds so the images are uploaded to website");
        await Task.Delay(10000);
        printToDebug("We are back now");

        sentera.FetchLatestImages();

        printToDebug("We are going to sleep for another 10 seconds so the images are downloaded");
        await Task.Delay(10000); // thsi value need to be bigger
        printToDebug("We are back now");

        Debugger.printToDebug(sentera.imageManager.debugMessage);
        //int counter = 0;
        
        //while (true)
        //{
        //    await Task.Delay(1000);
        //    printToDebug(counter.ToString() + sentera.imageManager.debugMessage);
        //    counter += 1;
        //}


        /**
         while (true)
         {
            if (Time.time > startTime + 60)
            {
                printToDebug("Stopping sentera session now");
                sentera.command("stop");
                printToDebug("Sentera session stopped");
                break;
            }
            //printToDebug("This thread is not blocked");
            //printToDebug(SenteraController.latestRGBImage.jpi.DateTime);
        }
        **/
    }

    // Update is called once per frame
    void Update()
    {
        /**
        if (Time.time > lastUpdateTime + 2)
        {
            printToDebug(Time.time.ToString());
            lastUpdateTime = Time.time;
            sentera.FetchLatestImages();
            imageProjectionManager.updateLatestImage(SenteraController.latestRGBImage);
            printToDebug(SenteraController.latestRGBImage.jpi.DateTime); // see if this is working because i did not see projection
            imageProjectionManager.updateLatestImage(SenteraController.latestNIRImage);
        }

        if (Time.time > startTime + 60) // if session has started for 2 minutes
        {
            printToDebug("Stopping sentera session now");
            sentera.command("stop");
            printToDebug("Sentera session is stopped");
        }
        **/
    }
}
