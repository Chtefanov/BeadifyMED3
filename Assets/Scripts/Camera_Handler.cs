using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Camera_Handler : MonoBehaviour
{
    public GameObject PrePhotoButtons;
    public GameObject PostPhotoButtons;


    public RawImage Camera_Feed_Image;
    //public AspectRatioFitter aspectRatioFitter;
    public TextMeshProUGUI Photo_message;

    private WebCamTexture webCamTexture;
    private Texture2D ThePhotoTaken;
    private float TextDisplay_Time;
    
    public void StartCamera()
    {
        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture(WebCamTexture.devices.Length > 0 ? WebCamTexture.devices[0].name : "");

            if (webCamTexture.deviceName == "")
            {
                Debug.LogError("No camera found on the device.");
                return;
            }

            Camera_Feed_Image.texture = webCamTexture;
            Camera_Feed_Image.material.mainTexture = webCamTexture;
            UI_change(true);
        }

        // Start or restart the camera
        if (!webCamTexture.isPlaying)
        {
            webCamTexture.Play();
            RotateCam();

            // AspectRatio of camfeed display will vary on each individual device. The "aspect fitter" 
            // component makes sure that the raw image, is calculated to correct camfeed dimensions.
            /*
            if (aspectRatioFitter != null)
            {
                float aspecRatio = (float)webCamTexture.width / (float)webCamTexture.height;
                aspectRatioFitter.aspectRatio = aspecRatio; 
            }*/
        }
    }
    private void RotateCam()
    {
        if (webCamTexture != null)
        {
            // Adjust the rotation of the camera feed
            Camera_Feed_Image.rectTransform.localEulerAngles = new Vector3(0, 0, -webCamTexture.videoRotationAngle);

            // Adjust the aspect ratio and scale for camera feeding correctly "size" to screen
            float aspectRatio = (float)webCamTexture.width / (float)webCamTexture.height;
            // Correct the scale based on the rotation angle
            Camera_Feed_Image.rectTransform.localScale = webCamTexture.videoRotationAngle % 180 == 0 ? new Vector3(1f, aspectRatio, 1f) : new Vector3(aspectRatio, 1f, 1f);
        }
    } //Camera feed displayed horisontally.
    public void StopCamera()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            Camera_Feed_Image.texture = null;
        }
    }

    public void TakePicture()
    {
        ThePhotoTaken = new Texture2D(webCamTexture.width, webCamTexture.height);
        ThePhotoTaken.SetPixels(webCamTexture.GetPixels());
        ThePhotoTaken.Apply();

        //ThePhotoTaken = RotateTexture(ThePhotoTaken, webCamTexture.videoRotationAngle);

        Camera_Feed_Image.texture = ThePhotoTaken;
        webCamTexture.Stop();

        UI_change(false);
    }
    void UI_change(bool NoPhotoTaken)
    {
        PrePhotoButtons.SetActive(NoPhotoTaken);
        PostPhotoButtons.SetActive(!NoPhotoTaken);
    }

    public void RetryPhoto()
    {
        if (webCamTexture != null)
        {
            Camera_Feed_Image.texture = webCamTexture;
            Camera_Feed_Image.material.mainTexture = webCamTexture;

            webCamTexture.Play();
            UI_change(true);
        }
    }
    public void PhotoAccepted()
    {
        Display_Photo_Message(TextDisplay_Time);
    }

    public void Display_Photo_Message(float TextDisplay_Time)
    {
        StartCoroutine(Message_Coroutine(Photo_message, TextDisplay_Time, RetryPhoto));
    }
    private IEnumerator Message_Coroutine(TextMeshProUGUI text, float time, Action callback)
    {
        Photo_message = text;
        Photo_message.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        Photo_message.gameObject.SetActive(false);
        Save_To_Gallery();
        RetryPhoto();
    }

    private void Save_To_Gallery()
    {
        if (ThePhotoTaken != null)
        {
            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ThePhotoTaken, "CameraTest", "Photo.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

            Destroy(ThePhotoTaken);
        }
        else
        {
            Debug.LogError("Attempted to save a null image to gallery");
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    } //shuts down cam with change of application/navigation to other place/funcationaly on app
}
