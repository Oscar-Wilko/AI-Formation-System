using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Transform saveParent;
    public GameObject savePrefab;
    [SerializeField] private Button editButton;
    [SerializeField] private Button simulateButton;

    private void Awake()
    {
        GenerateSaves();
    }

    private void GenerateSaves()
    {
        int saves = SaveSystem.SaveCount();
        for(int i = 0; i < saves; i ++)
            Instantiate(savePrefab, saveParent);
        saveParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0,saves * 120 + 20);
    }

    private void Update()
    {
        editButton.interactable = SaveSystem.saveIndex >= 0;
        simulateButton.interactable = SaveSystem.saveIndex >= 0;
    }

    public void NewFormation()
    {
        SaveSystem.saveIndex = -1;
        SceneManager.LoadScene("Creator");
    }

    public void EditFormation()
    {
        SceneManager.LoadScene("Creator");
    }

    public void SimulateFormation()
    {
        SceneManager.LoadScene("Simulator");
    }
}
