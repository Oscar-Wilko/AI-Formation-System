using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveSelect : MonoBehaviour
{
    public TextMeshProUGUI index;
    public TextMeshProUGUI title;
    public GameObject selectEffect;
    private int saveIndex;

    private void Awake()
    {
        saveIndex = transform.GetSiblingIndex();
        index.text = (saveIndex+1).ToString();
        Deselect();
    }

    public void Select()
    {
        for(int i = 0; i < transform.parent.childCount; i ++)
            transform.parent.GetChild(i).GetComponent<SaveSelect>().Deselect();
        selectEffect.SetActive(true);
        title.text = "Selected";
        SaveSystem.saveIndex = saveIndex;
    }

    public void Deselect()
    {
        selectEffect.SetActive(false);
        title.text = "Click to select";
    }
}
