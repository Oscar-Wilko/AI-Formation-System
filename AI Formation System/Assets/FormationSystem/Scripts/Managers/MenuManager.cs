using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void NewFormation()
    {
        SceneManager.LoadScene("Creator");
    }

    public void EditFormation()
    {
        // Set some static variable somewhere to load index
        SceneManager.LoadScene("Creator");
    }

    public void SimulateFormation()
    {
        // Set some static variable somewhere to load index
        SceneManager.LoadScene("Simulator");
    }
}
