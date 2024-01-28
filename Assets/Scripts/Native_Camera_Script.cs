using UnityEngine;

public class Native_Camera_Script : MonoBehaviour
{
    private int cameraMaxSize = -1;
    private bool shouldTakePicture = false;
    private string lastImagePath = null;

    public void ActivateCamera()
    {
        shouldTakePicture = true;
    }
    void Update()
    {
        if (shouldTakePicture && !NativeCamera.IsCameraBusy())
        {
            shouldTakePicture = false;
            TakePicture();
        }
        if (lastImagePath != null)
        {
            ProcessAndSaveImage(lastImagePath);
            lastImagePath = null;
        }
    }
    private void TakePicture()
    {
        NativeCamera.TakePicture((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                lastImagePath = path; // Store the path for later processing
            }
            else
            {
                Debug.Log("No image was taken, or operation was canceled.");
            }
        }, cameraMaxSize);
    }
    private void ProcessAndSaveImage(string imagePath)
    {
        Texture2D texture = NativeCamera.LoadImageAtPath(imagePath, cameraMaxSize);
        NativeGallery.SaveImageToGallery(texture, "MY MED 3 pixelator pictures", "My img {0}.png");
        Debug.Log("Image saved to gallery: " + imagePath);
        Destroy(texture);
    }
}
