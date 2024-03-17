using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatorManager : MonoBehaviour
{
    public enum CreatorMode
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
    [SerializeField] private Transform _creatorParent;
    [SerializeField] private GameObject _formationPrefab;
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Transform _previewFloor;

    public void ToggleCreatorMode(CreatorMode newMode)
    {
        mode = newMode;
        if (_dropAreaCreator) _dropAreaCreator.SetActive(mode == CreatorMode.Creator);
        if (_dropAreaPreview) _dropAreaPreview.SetActive(mode == CreatorMode.Preview);
        if (_creatorButton) _creatorButton.SetActive(mode == CreatorMode.Preview);
        if (_previewButton) _previewButton.SetActive(mode == CreatorMode.Creator);
        if (_previewObjects) _previewObjects.SetActive(mode == CreatorMode.Preview);

        if (mode == CreatorMode.Preview) GeneratePreview();
    }

    public void ToggleCreatorMode() => ToggleCreatorMode(mode == CreatorMode.Creator ? CreatorMode.Preview : CreatorMode.Creator);

    private void GeneratePreview()
    {
        for(int i = 0; i < _previewFloor.childCount; i++)
        {
            Destroy(_previewFloor.GetChild(i).gameObject);
        }

        for (int i = 0; i < _creatorParent.childCount; i++)
        {
            GameObject obj = _creatorParent.GetChild(i).gameObject;
            DragFormation drag = obj.GetComponent<DragFormation>();
            List<Vector2> positions = new List<Vector2>();
            switch (drag.type)
            {
                case FormationType.Box:
                    positions = BoxFormation.GeneratePositions(drag.boxV, new List<Vector2>(), 0);
                    break;
                case FormationType.Arrow:
                    positions = ArrowFormation.GeneratePositions(drag.arrV, new List<Vector2>(), 0);
                    break;
                case FormationType.Triangle:
                    positions = TriangleFormation.GeneratePositions(drag.triV, new List<Vector2>(), 0);
                    break;
            }
            foreach(Vector2 pos in positions)
            {
                GameObject unit = Instantiate(_unitPrefab, _previewFloor);
                Vector2 recPos = obj.GetComponent<RectTransform>().anchoredPosition;
                unit.transform.localPosition = new Vector3(pos.x * 0.1f + recPos.x * 0.01f, 0.1f, pos.y * 0.1f + recPos.y * 0.01f);
                unit.transform.localScale = Vector3.one * 0.1f;
                unit.GetComponent<Unit>().SetPreview();
            }
        }
    }

    private void Awake()
    {
        if (SaveSystem.saveIndex >= 0)
            Load();
    }

    public void Save()
    {
        int saveIndex = SaveSystem.saveIndex;
        if (saveIndex < 0)
        {
            saveIndex = SaveSystem.NextAvailabeSaveIndex();
        }

        FullFormation formation = new FullFormation();
        formation.logs = new List<FormationLog>();
        for (int i = 0; i < _creatorParent.childCount; i ++)
        {
            FormationLog log = new FormationLog();
            GameObject obj = _creatorParent.GetChild(i).gameObject;
            DragFormation drag = obj.GetComponent<DragFormation>();
            log.position = obj.GetComponent<RectTransform>().anchoredPosition;
            log.type = drag.type;
            switch (log.type)
            {
                case FormationType.Box:
                    log.boxValues = drag.boxV;
                    break;
                case FormationType.Arrow:
                    log.arrowValues = drag.arrV;
                    break;
                case FormationType.Triangle:
                    log.triangleValues = drag.triV;
                    break;
            }
            formation.logs.Add(log);
        }

        SaveSystem.SaveInfo(formation, saveIndex);
    }

    public void Load()
    {
        int saveIndex = SaveSystem.saveIndex;
        if (saveIndex < 0)
        {
            Debug.LogWarning("False save index detected, stopping load of formation.");
            return;
        }

        FullFormation info = SaveSystem.LoadInfo(saveIndex);
        if (info == null)
        {
            Debug.LogWarning("False save reference detected, stopping load of formation.");
            return;
        }
        foreach (FormationLog log in info.logs)
        {
            GameObject obj = Instantiate(_formationPrefab);

            obj.transform.SetParent(_creatorParent);
            obj.GetComponent<RectTransform>().anchoredPosition = log.position;

            Color col = Color.white;
            switch(log.type)
            {
                case FormationType.Box: col = Color.green; break;
                case FormationType.Arrow: col = Color.red; break;
                case FormationType.Triangle: col = Color.blue; break;
            }
            obj.GetComponent<RawImage>().color = col;

            DragFormation drag = obj.GetComponent<DragFormation>();
            drag.type = log.type;
            drag.hasDragged = true;
            switch(log.type)
            {
                case FormationType.Box: drag.boxV = log.boxValues; break;
                case FormationType.Arrow: drag.arrV = log.arrowValues; break;
                case FormationType.Triangle: drag.triV = log.triangleValues; break;
            }
        }
    }

    public void Exit()
    {
        SaveSystem.saveIndex = -1;
        SceneManager.LoadScene("SceneSelector");
    }
}
