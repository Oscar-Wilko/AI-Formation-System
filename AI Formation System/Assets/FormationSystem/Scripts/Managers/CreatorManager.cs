using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatorManager : MonoBehaviour
{
    private enum CreatorMode
    {
        Creator,
        Preview
    }

    private CreatorMode mode = CreatorMode.Creator;
    [SerializeField] private GameObject _dropAreaCreator;
    [SerializeField] private GameObject _dropAreaPreview;
    [SerializeField] private GameObject _creatorButton;
    [SerializeField] private GameObject _previewButton;
    [SerializeField] private GameObject _previewObjects;

    public void ToggleCreatorMode()
    {
        mode = mode == CreatorMode.Creator ? CreatorMode.Preview : CreatorMode.Creator;
        if (_dropAreaCreator) _dropAreaCreator.SetActive(mode == CreatorMode.Creator);
        if (_dropAreaPreview) _dropAreaPreview.SetActive(mode == CreatorMode.Preview);
        if (_creatorButton) _creatorButton.SetActive(mode == CreatorMode.Preview);
        if (_previewButton) _previewButton.SetActive(mode == CreatorMode.Creator);
        if (_previewObjects) _previewObjects.SetActive(mode == CreatorMode.Preview);
    }

    public void Exit() => SceneManager.LoadScene("SceneSelector");
}
