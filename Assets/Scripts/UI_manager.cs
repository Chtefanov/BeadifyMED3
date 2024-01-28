using UnityEngine;
public class UI_manager : MonoBehaviour
{
    public GameObject[] Panels;

    void Start()
    {
        ShowPanel(0);
    }
    public void ShowPanel(int index)
    {
        // Hide all panels
        foreach (var panel in Panels)
        {
            panel.SetActive(false);
        }
        // Show the selected panel
        if (index >= 0 && index < Panels.Length)
        {
            Panels[index].SetActive(true);
        }
    }    
}



