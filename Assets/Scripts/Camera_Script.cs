using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Camera_Script : MonoBehaviour
{
    public Pixelator_2 pixelation;
    // Buttons
    public GameObject PrePhotoButtons;
    public GameObject PostPhotoButtons;
    // Text
    public TextMeshProUGUI Saved_To_Gallery_Text;
    private float TextDisplay_Time;
    // Camera stuff
    public RawImage Cam_Feed;
    private Texture2D The_Photo_Taken;
    private WebCamTexture webCamTexture;
    //Logic
    
    public void StartCamera()
    {
        Debug.Log("StartCamera method called");
        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture();
            Cam_Feed.texture = webCamTexture;
            Cam_Feed.material.mainTexture = webCamTexture;
            UI_No_Photo_Taken(true);
        }

        if (!webCamTexture.isPlaying)
        {
            webCamTexture.Play();
        }
    }
    public void StopCamera()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
            Cam_Feed.texture = null;
        }
    }
    public void TakePicture()
    {
        The_Photo_Taken = new Texture2D(webCamTexture.width, webCamTexture.height);
        The_Photo_Taken.SetPixels(webCamTexture.GetPixels());
        The_Photo_Taken.Apply();
        //Temporarily display of picture
        Cam_Feed.texture = The_Photo_Taken;
        webCamTexture.Stop();

        UI_No_Photo_Taken(false);
    }
    public void Retry_Photo()
    {
        if (webCamTexture != null)
        {
            Cam_Feed.texture = webCamTexture;
            webCamTexture.Play();
            UI_No_Photo_Taken(true);
            
        }
    }
    void UI_No_Photo_Taken(bool PhotoTaken)
    {
        PrePhotoButtons.SetActive(PhotoTaken);
        PostPhotoButtons.SetActive(!PhotoTaken);
    }
    public void PhotoAccepted()
    {
        Display_Photo_Message(2.5f);
    }
    public void Display_Photo_Message(float TextDisplay_Time)
    {
        StartCoroutine(Message_Coroutine(Saved_To_Gallery_Text, TextDisplay_Time, Retry_Photo));
    }
    private IEnumerator Message_Coroutine(TextMeshProUGUI text, float time, Action callback)
    {
        Saved_To_Gallery_Text = text;
        Saved_To_Gallery_Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        Saved_To_Gallery_Text.gameObject.SetActive(false);
        Save_To_Gallery();
        Retry_Photo();
    }
    private void Save_To_Gallery()
    {
        if (The_Photo_Taken != null)
        {
            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(The_Photo_Taken, "CameraTest", "Photo.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

            Destroy(The_Photo_Taken);
        }
        else
        {
            Debug.LogError("Attempted to save a null image to gallery");
        }
    }
    void OnDisable()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
