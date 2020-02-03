using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    // Start is called before the first frame update
    public static void printToDebug(string debugMessage)
    {
        GameObject debugDisplay = GameObject.Find("DebugDisplay");
        debugDisplay.GetComponent<UnityEngine.UI.Text>().text = debugMessage;
    }

    public static void changeCubeTexture(Texture2D texture)
    {
        //GameObject debugCube = GameObject.Find("DebugCube");
        //debugCube.GetComponent<Renderer>().material.mainTexture = texture;
    }

    public static void changeCubeMaterial(Material material)
    {
        //GameObject debugCube = GameObject.Find("DebugCube");
        //debugCube.GetComponent<Renderer>().material = material;
    }

    public static void changeCubeColor(Color color)
    {
        //GameObject debugCube = GameObject.Find("DebugCube");
        //debugCube.GetComponent<Renderer>().material.color = color;
    }
}
