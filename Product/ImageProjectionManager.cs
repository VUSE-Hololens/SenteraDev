using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProjectionManager : MonoBehaviour
{
    private JpegImage latestRGBImage, latestNIRImage;
    Material NDVIMaterial;
    static private GameObject NDVIProjector;
    public string debugMessage;
    private void initializeNDVIMaterial()
    {
        Debugger.printToDebug("initializingNDVIMaterial");
        NDVIMaterial = new Material(Shader.Find("Unlit/CalculateNDVI"));
        NDVIMaterial.SetTexture("_RGBTex", Texture2D.whiteTexture);
        NDVIMaterial.SetTexture("_NIRTex", Texture2D.whiteTexture);
        NDVIMaterial.SetFloat("_ev_rgb", (float) 100);
        NDVIMaterial.SetFloat("_iso_rgb", (float) 100);
        NDVIMaterial.SetFloat("_ev_nir", (float) 100);
        NDVIMaterial.SetFloat("_iso_nir", (float) 9);
        Debugger.changeCubeMaterial(NDVIMaterial);
        // Debugger.changeCubeColor(Color.blue);
    }

    public ImageProjectionManager()
    {
        NDVIProjector = Instantiate(Resources.Load("NDVIProjector", typeof(GameObject)) as GameObject);
        initializeNDVIMaterial();
        debugMessage = "";
    }

    private void configureProjector()
    {
        Debugger.printToDebug("Configuring Projector");
        Debugger.changeCubeTexture((Texture2D)NDVIMaterial.GetTexture("_RGBTex"));
        Vector3 projectionOffset = new Vector3(0.0f, 0.75f, 0.0f);
        NDVIProjector.transform.position = Camera.main.transform.position + projectionOffset;
        NDVIProjector.transform.rotation = Camera.main.transform.rotation;
        NDVIProjector.GetComponent<Projector>().material.SetTexture("_ShadowTex", NDVIMaterial.GetTexture("_RGBTex"));
    }

    public Material NDVIMaterialGetSet
    {
        get { return NDVIMaterial; }
        set
        {
            configureProjector();
        }
    }

    public JpegImage latestRGBImageGetSet
    {
        get { return latestRGBImage; }
        set
        {
            Debugger.printToDebug("Setting RGB Image");
            latestRGBImage = value;
            Debugger.printToDebug("latestGRBImage set");
            NDVIMaterial.SetTexture("_RGBTex", value.texture);
            Debugger.printToDebug("_RGBTex set");
            NDVIMaterial.SetFloat("_ev_RGB", (float) value.jpi.ExposureTime);
            NDVIMaterial.SetFloat("_ev_rgb", (float) 0.04);
            //Debugger.printToDebug("_ev_rgb set");
            //NDVIMaterial.SetFloat("_iso_rgb", value.jpi.ISOSpeedRating);
            NDVIMaterial.SetFloat("_iso_rgb", (float) 0.5);
            Debugger.printToDebug("_iso_rgb set");
            NDVIMaterialGetSet = NDVIMaterial;
            Debugger.printToDebug("LatestRGBImage Set. ISO is " + value.jpi.ISOSpeedRating.ToString() + "exposure is " + 
                value.jpi.ExposureTime.ToString());
            //Debugger.changeCubeTexture((Texture2D) NDVIMaterial.GetTexture("_RGBTex"));
            Debugger.changeCubeMaterial(NDVIMaterial);
        }
    }

    public JpegImage latestNIRImageGetSet
    {
        get { return latestNIRImage; }
        set
        {
            Debugger.printToDebug("setting NIR image");
            latestNIRImage = value;
            Debugger.printToDebug("latestNirImage Set");
            NDVIMaterial.SetTexture("_NIRTex", value.texture);
            //NDVIMaterial.SetTexture("_NIRTex", Texture2D.whiteTexture);
            Debugger.printToDebug("NDVIMaterial set");
            //NDVIMaterial.SetFloat("_ev_nir", (float) value.jpi.ExposureTime);
            NDVIMaterial.SetFloat("_ev_nir", (float) 0.5);
            Debugger.printToDebug("NIR _ev_nir set");
            //NDVIMaterial.SetFloat("_iso_nir", value.jpi.ISOSpeedRating);
            NDVIMaterial.SetFloat("_iso_nir", (float) 0.5);
            Debugger.printToDebug("NIR _iso_nir set");
            NDVIMaterialGetSet = NDVIMaterial;
            Debugger.printToDebug("LatestNIRImage Set. ISO is " + value.jpi.ISOSpeedRating.ToString() + "exposure is " +
                value.jpi.ExposureTime.ToString());
            //Debugger.changeCubeTexture(value.texture);
            Debugger.changeCubeMaterial(NDVIMaterial);
            //Debugger.changeCubeTexture((Texture2D)NDVIMaterial.GetTexture("_NIRTex"));
        }
    }

    public void updateLatestImage(JpegImage image)
    {
        Debugger.printToDebug("updating lastest image");
        switch (image.type)
        {
            case JpegImage.ImageType.RGB:
                Debugger.printToDebug("updating RGB image");
                latestRGBImageGetSet = image;
                break;
            case JpegImage.ImageType.NIR:
                Debugger.printToDebug("updating NIR Image");
                latestNIRImageGetSet = image;
                break;
            default:
                break;
                // this should not happen
        }
    }
}
