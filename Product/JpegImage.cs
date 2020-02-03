using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;

public class JpegImage : MonoBehaviour
{
    void printToDebug(string message)
    {
        this.GetComponent<UnityEngine.UI.Text>().text = message;
    }

    public enum ImageType
    {
        RGB,
        NIR
    }

    private string url;
    private byte[] rawData;
    public ExifLib.JpegInfo jpi;
    public Texture2D texture;
    public ImageType type;

    public byte[] rawDataGetSet
    {
        get { return rawData; }
        set
        {
            rawData = value;
            jpi = ExifLib.ExifReader.ReadJpeg(value, "test");
            texture = new Texture2D(2, 2);
            Debugger.printToDebug("loading texture");
            texture.LoadImage(value);
            Debugger.printToDebug("texture loaded");
            // Debugger.changeCubeTexture(texture);
        }
    }

    // useless function
    IEnumerator downloadTexture(string textureUrl)
    {
        Debugger.printToDebug("downloading");
        using (WWW www = new WWW(textureUrl))
        {
            yield return www;
            texture = www.texture;
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = www.texture;
            Debugger.printToDebug("mainTexture set");
        }
    }

    public string urlGetSet
    {
        get { return url; }
        set
        {
            Debugger.printToDebug("set entered");
            url = value;
            StartCoroutine(downloadTexture(value));
        }
    }

    public JpegImage(byte[] data, JpegImage.ImageType imageType, string imageUrl)
    {
        Debugger.printToDebug("Creating Jpeg");
        rawDataGetSet = data;
        type = imageType;
        //urlGetSet = imageUrl;
    }
}
