using UnityEngine;
using Wilberforce;

public class Color_Blind_Controller : MonoBehaviour
{
    private Colorblind colorblindEffect;
    void Start()
    {
        colorblindEffect = Camera.main.GetComponent<Colorblind>();
    }

    public void ChangeColorblindMode(int mode)
    {
        if (colorblindEffect != null)
        {
            colorblindEffect.Type = mode;
        }
    }
}
