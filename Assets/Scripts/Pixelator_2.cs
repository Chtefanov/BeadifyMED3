using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class Pixelator_2 : MonoBehaviour
{
    #region Public UI Compoenents + Icons
    [Header("Gallery Display")]
    public RawImage Gallery_Displayer;

    [Header("Initial UI Buttons")]
    public GameObject Select_Picture_Button;
    public GameObject Back_To_Main_Menu;

    [Header("Other UI Buttons")]
    public GameObject Pixelate_Picture_Button;
    public GameObject Save_Pixelation_Button;
    //Saved Pixelation UI-Buttons
    public GameObject CVD_Mode_No_Button;
    public GameObject CVD_Mode_Yes_Button;

    [Header("UI text")]
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI Template_CVD_Save;
    public TextMeshProUGUI Template_Saved;
    public GameObject QuestionTextGameObject; //For Reference to the GameObject containing TextMeshProUGUI component
   
    [Header("CVD icons")]
    public Texture2D[] Color_CVD_Icons;

    #endregion

    #region Private Color palette and colors + Icon/CVD textures Dictionary for palette
    
    private static readonly Color Purple = new Color(0.501f, 0.031f, 0.870f, 1f);  // Purple RGB original 128, 8, 222 - RBG values devided by 255 in order to define the color in unity.
    private static readonly Color Orange = new Color(0.937f, 0.643f, 0.129f, 1f); 
    private static readonly Color LightGreen = new Color(0.576f, 0.980f, 0.298f, 1f);

    private Color[] palette = new Color[] {

    Color.red,
    Color.green,
    Color.blue, 
    Color.yellow,
    Color.black, 
    Color.white, 
    Color.magenta, 
    Purple,
    Orange,
    LightGreen
    };
    /// <summary>
    /// A list of Colors using Unitys built in colors and own custom colors based on 10 beads mixed colors bag
    /// <returns></returns>
    private Dictionary<Color, Texture2D> MapIconsToPalletColors()
    {
        var colorToIconMap = new Dictionary<Color, Texture2D>();
        for (int i = 0; i < palette.Length; i++)
        {
            colorToIconMap.Add(palette[i], Color_CVD_Icons[i]); //makes the match with icon to color palette.
        }
        return colorToIconMap;
    }





    /// <summary>
    /// Private dictionary used to map colors used from palette list on a pixelated picture
    /// to the corresponding color icons from the public icon list 
    /// </summary>
    #endregion

    #region Life Cycles of Pixelator and UI states
    public enum UIState
    {
        Initial,
        PictureSelected,
        AfterPixelation,
        AcceptedPixelation,
        SavePixelationMode
    }
    /// <summary>
    /// Different UIStates created as enum. Used in context of updating UI GameObject elements based on interaction with pixelator.
    /// </summary>
    private void UpdateUI(UIState newState)
    {
        Back_To_Main_Menu.SetActive(newState != UIState.SavePixelationMode); //Back button should be present in all UI states but not 'SavePixelationMode'
        Select_Picture_Button.SetActive(newState == UIState.Initial || newState == UIState.PictureSelected || newState == UIState.AfterPixelation);
        // A picture is present in the Pixelator.
        Pixelate_Picture_Button.SetActive(newState == UIState.PictureSelected);
        // A picture has been pixelated
        Save_Pixelation_Button.SetActive(newState == UIState.AfterPixelation);
        QuestionTextGameObject.SetActive(newState == UIState.SavePixelationMode); //Activates question TextMeshPro GameObject
        CVD_Mode_Yes_Button.SetActive(newState == UIState.SavePixelationMode);
        CVD_Mode_No_Button.SetActive(newState == UIState.SavePixelationMode);
    }




    /// <summary>
    /// Setup of what elements should be present in the UI based on the UI state
    /// </summary>
    /// 
    public void ResetPixelatorPanel() 
    {
        if (Gallery_Displayer != null)
        {
            Gallery_Displayer.texture = null;
            UpdateUI(UIState.Initial);
        }
    }
    /// <summary>
    /// Removes Textures from Pixelator displayment and sets textture as = null.
    /// Used when returning to main menu so previus display will always be empty + UI state = Inital upon
    /// reentering the pixelator.
    /// </summary>   
    public void ImageSelected(Texture2D image)
    {
        UpdateUI(UIState.PictureSelected);
    }
    /// <summary>
    /// Used in order to notice UI change based on an image being present or not in the pixelator.
    /// Reference to method is present in "Gallery_Acees" script when loading a pixture using the Native Gallery
    /// </summary>
    public void Save_Pixelation_Pressed()
    {
        UpdateUI(UIState.SavePixelationMode);
    }
    /// <summary>
    /// Used to update UI for choosing CVD mode/non CVD mode when saving a pixelated picture.
    /// </summary>
    private IEnumerator ShowTemplateMessageCorotine(TextMeshProUGUI whereSaved, float duration)
    {
        whereSaved.gameObject.SetActive(true); 
        yield return new WaitForSeconds(duration);
        whereSaved.gameObject.SetActive(false);
    }
    /// <summary>
    /// Corutine used to find and display TextMeshPro GameObject for certain periode.
    /// </summary>
    public void ShowYesCVD()
    {
        StartCoroutine(ShowTemplateMessageCorotine(Template_CVD_Save, 3f));
    }
    public void ShowNoCVD()
    {
        StartCoroutine(ShowTemplateMessageCorotine(Template_Saved, 3f));
    }
    /// <summary>
    /// Corutine used in CVD / non CVD saving modes
    /// </summary>
    #endregion

    #region Pixelation Methods
    public void PixelatePicture()
    {
        if (Gallery_Displayer.texture is Texture2D originalTexture)
        {
            Texture2D pixelatedTexture = Pixelate(originalTexture);
            Gallery_Displayer.texture = pixelatedTexture;
            UpdateUI(UIState.AfterPixelation);
        }
    }

    private Texture2D Pixelate(Texture2D PrePixelatedPicture)
    {
        Texture2D Grid_Fitted_Image = Fit_Image_To_Beads(PrePixelatedPicture, 29, 29);
        Texture2D Ten_Pallet_Colors = Pallet_To_Picture(Grid_Fitted_Image, palette);
        return Ten_Pallet_Colors;
    }
    private Texture2D Fit_Image_To_Beads(Texture2D image, int width, int height)
    {
        RenderTexture smallerImage = new RenderTexture(width, height, 0);
        RenderTexture.active = smallerImage;
        Graphics.Blit(image, smallerImage);
        Texture2D results = new Texture2D(width, height, image.format, false); //undersøges
        results.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        results.Apply();
        RenderTexture.active = null;
        smallerImage.Release();
        return results;
    }
    private Texture2D Pallet_To_Picture(Texture2D SmallImage, Color[] palette)
    {
        Color[] imageColor = SmallImage.GetPixels();
        for (int i = 0; i < imageColor.Length; i++)
        {
            imageColor[i] = Calculate_Closets_Colors(imageColor[i], palette);
        }
        Texture2D Color_Calculated_Picture = new Texture2D(SmallImage.width, SmallImage.height);
        Color_Calculated_Picture.SetPixels(imageColor);
        Color_Calculated_Picture.Apply();
        return Color_Calculated_Picture;
    }
    private Color Calculate_Closets_Colors(Color targetColor, Color[] palette)
    {
        Color closestColor = palette[0];
        float shortestDistance = Mathf.Infinity;
        foreach (Color paletteColor in palette)
        {
            float someDistance = ColorDistance(targetColor, paletteColor);
            if (someDistance < shortestDistance)
            {
                shortestDistance = someDistance;
                closestColor = paletteColor;
            }
        }
        return closestColor;
    }
    private float ColorDistance(Color c1, Color c2)
    {
        return Mathf.Sqrt((c1.r - c2.r) * (c1.r - c2.r) +
                          (c1.g - c2.g) * (c1.g - c2.g) +
                          (c1.b - c2.b) * (c1.b - c2.b));
    }

    #endregion

    #region Saving of the Templates methods.

    public void StartSaveProcess(bool CVD)
    {
        if (Gallery_Displayer.texture is Texture2D pixelatedTexture)
        {
            if (CVD)
            {
                StartCoroutine(SaveInCVD_Coroutine(pixelatedTexture, CVD));
            }
            else
            {
                StartCoroutine(Save_Coroutine(FitToTemplate_Dim(pixelatedTexture, 424, 424), CVD));
            }
        }
    }
    private IEnumerator Save_Coroutine(Texture2D image, bool CVD)
    {
        byte[] bytes = image.EncodeToPNG();

        string folderName = CVD ? "CVD mode Pixelated Image" : "Bead Art Pixelated Image";

        string imageCategory = CVD ? "CVD mode Pixelated Image" : "Bead Art Pixelated Image";

        string fileName = $"{imageCategory}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, folderName, fileName, (success, path) =>
        {
            Debug.Log($"Image saved: {success} at path: {path}");
        });
        yield return null;
    }




    private Texture2D FitToTemplate_Dim(Texture2D Pixelated_Image, int newWidth, int newHeight)
    {
        Pixelated_Image.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(Pixelated_Image, rt);
        Texture2D newTexture = new Texture2D(newWidth, newHeight);
        newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTexture.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return newTexture;
    }
    private IEnumerator SaveInCVD_Coroutine(Texture2D pixelatedImage, bool CVD)
    {
        Dictionary<Color, Texture2D> MappedColors = MapIconsToPalletColors();
        Texture2D CVDTemplate = OverlayIcons(pixelatedImage, MappedColors);
        yield return new WaitForEndOfFrame(); // Wait for the image processing to complete
        yield return StartCoroutine(Save_Coroutine(CVDTemplate, CVD));
    }
    
    private Texture2D OverlayIcons(Texture2D pixelatedImage, Dictionary<Color, Texture2D> colorToIconMap)
    {
        int iconSize = 150; // Size of each icons dimensions
        int grid_dimension = 29;  // Size of the grid

        Texture2D Pixelated_In_CVD = new Texture2D(grid_dimension * iconSize, grid_dimension * iconSize);
        for (int x = 0; x < grid_dimension; x++)
        {
            for (int y = 0; y < grid_dimension; y++)
            {
                Color pixelColor = pixelatedImage.GetPixel(x, y);
                Color ColorOnPixelated = MatchColorFromPixelated(pixelColor, palette);
                colorToIconMap.TryGetValue(ColorOnPixelated, out Texture2D icon);
                Color[] iconPixels = icon.GetPixels();
                Pixelated_In_CVD.SetPixels(x * iconSize, y * iconSize, icon.width, icon.height, iconPixels);
            }
        }
        Pixelated_In_CVD.Apply();
        return Pixelated_In_CVD;
    }
    private Color MatchColorFromPixelated(Color target, Color[] palette)
    {
        Color closestColor = palette[0];
        float minDistance = Mathf.Infinity;

        foreach (Color color in palette)
        {
            float distance = ColorDistance(target, color);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = color;
            }
        }

        return closestColor;
    }
    #endregion
}