using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;
using System.IO;
using System;

// Static utility class for reading metadata from the image
public class MetaDataReader
{
    // Get the exposure time from the given image 
    // @param fromImage the image
    //
    // @return the exposure time of the image; -1 if the method encounters any kind of error
    public static double GetExposureFromImage(System.Drawing.Image fromImage)
    {
        try
        {
            // 8 bytes representing a rational
            byte[] exifExposure = new byte[8];
            PropertyItem[] propItems = fromImage.PropertyItems;
            foreach (PropertyItem propItem in propItems)
            {
                Debug.Log(propItem.Id);
                // See https://www.media.mit.edu/pia/Research/deepview/exif.html
                // 0x829A PropertyTagExifExposureTime
                if (propItem.Id == 0x829A)
                {
                    exifExposure = propItem.Value;
                }
                // See EXIF data format
                int numerator = BitConverter.ToInt32(exifExposure, 0);
                int denominator = BitConverter.ToInt32(exifExposure, 4);
                return (double)numerator / denominator;
            }
            // Should not get here. -1 signals an error since exposure time is always non-negative
            return -1;
        }
        catch (Exception e)
        {
            // Debug.Log(e.Message);
            // Should not get here. -1 signals an error since exposure time is always non-negative
            return -1;
        }
    }

    // Get the exposure time from the given image 
    // @param fromImage the image
    //
    // @return the iso value of the image; -1 if the method encounters any kind of error
    public static double GetISOFromImage(System.Drawing.Image fromImage)
    {
        try
        {
            // 2 bytes representing an unsigned short
            byte[] exifISO = new byte[2];
            PropertyItem[] propItems = fromImage.PropertyItems;
            foreach (PropertyItem propItem in propItems)
            {
                Debug.Log(propItem.Id);
                // See https://www.media.mit.edu/pia/Research/deepview/exif.html
                // 0x8827 PropertyTagExifISOSpeed
                if (propItem.Id == 0x8827)
                {
                    exifISO = propItem.Value;
                }
                short s = (short)(exifISO[0] | (exifISO[1] << 8));
                return Convert.ToDouble(s);
            }
            // Should not get here. -1 signals an error since the iso value is always non-negative
            return -1;
        }
        catch (Exception e)
        {
            // Debug.Log(e.Message);
            // Should not get here. -1 signals an error since the iso value is always non-negative
            return -1;
        }
    }

    // Get the exposure time from the image associated with the given url
    // @param fromUrl the url to the image
    //
    // @return the exposure time of the image; -1 if the method encounters any kind of error
    public static double GetExposureFromUrl(string fromUrl)
    {
        try
        {
            // 8 bytes representing a rational
            byte[] exifExposure = new byte[8];
            System.Drawing.Image image;
            // Download image from url
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead(fromUrl))
                {
                    image = System.Drawing.Image.FromStream(stream);
                }
            }

            PropertyItem[] propItems = image.PropertyItems;
            foreach (PropertyItem propItem in propItems)
            {
                Debug.Log(propItem.Id);
                // See https://www.media.mit.edu/pia/Research/deepview/exif.html
                // 0x829A PropertyTagExifExposureTime
                if (propItem.Id == 0x829A)
                {
                    exifExposure = propItem.Value;
                }
                // See EXIF data format
                int numerator = BitConverter.ToInt32(exifExposure, 0);
                int denominator = BitConverter.ToInt32(exifExposure, 4);
                return (double)numerator / denominator;
            }
            // Should not get here. -1 signals an error since exposure time is always non-negative
            return -1;
        }
        catch (Exception e)
        {
            // Debug.Log(e.Message);
            // Should not get here. -1 signals an error since exposure time is always non-negative
            return -1;
        }
    }

    // Get the iso value from the image associated with the given url
    // @param fromUrl the url to the image
    //
    // @return the iso value of the image; -1 if the method encounters any kind of error
    public static double GetISOFromUrl(string fromUrl)
    {
        try
        {
            // 2 bytes representing an unsigned short
            byte[] exifISO = new byte[2];
            System.Drawing.Image image;
            // Download image from url
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead(fromUrl))
                {
                    image = System.Drawing.Image.FromStream(stream);
                }
            }

            PropertyItem[] propItems = image.PropertyItems;
            foreach (PropertyItem propItem in propItems)
            {
                Debug.Log(propItem.Id);
                // See https://www.media.mit.edu/pia/Research/deepview/exif.html
                // 0x8827 PropertyTagExifISOSpeed
                if (propItem.Id == 0x8827)
                {
                    exifISO = propItem.Value;
                }
                short s = (short)(exifISO[0] | (exifISO[1] << 8));
                return Convert.ToDouble(s);
            }
            // Should not get here. -1 signals an error since the iso value is always non-negative
            return -1;
        }
        catch (Exception e)
        {
            // Debug.Log(e.Message);
            // Should not get here. -1 signals an error since the iso value is always non-negative
            return -1;
        }
    }
}
