using UnityEngine;
using UnityEngine.UI;

public class Gallery_Acces : MonoBehaviour
{
    public Pixelator_2 pixelator;
    public RawImage Gallery_Display;
    
    private Texture2D Some_Picture_From_Gallery;
    public void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path)) 
            {   
                Some_Picture_From_Gallery = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (Some_Picture_From_Gallery == null)
                {
                    return;
                }
                if (Gallery_Display != null)
                {
                    Gallery_Display.texture = Some_Picture_From_Gallery;
                    if (pixelator != null)
                    {
                        pixelator.ImageSelected(Some_Picture_From_Gallery);
                        Debug.Log("Image ready for pixelation");
                    }
                }
            }
        });
    }
}

