using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimulatorManager : MonoBehaviour
{
    [SerializeField] private GameObject _boxPrefab;
    [SerializeField] private GameObject _arrPrefab;
    [SerializeField] private GameObject _triPrefab;
    [SerializeField] private Transform _formationParent;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private float _distSpread;
    [SerializeField] private Vector3 _targetPos;

    public void Return()
    {
        SaveSystem.saveIndex = -1;
        SceneManager.LoadScene("SceneSelector");
    }

    private void Awake()
    {
        if (SaveSystem.saveIndex >= 0)
            Load();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_targetPos, 2);
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
            GameObject obj;
            switch (log.type)
            {
                case FormationType.Box: obj = Instantiate(_boxPrefab); break;
                case FormationType.Arrow: obj = Instantiate(_arrPrefab); break;
                case FormationType.Triangle: obj = Instantiate(_triPrefab); break;
                default: continue;
            }
            obj.transform.SetParent(_formationParent);
            Vector2 shiftPos = Utils.Rotate(log.position, Mathf.Deg2Rad * -_startPosition.localEulerAngles.y);
            Vector3 pos = _startPosition.position;
            pos += new Vector3(shiftPos.x, 0, shiftPos.y) * _distSpread;
            pos.y = Utils.RayDown(new Vector2(pos.x, pos.z));
            obj.transform.position = pos;
            switch (log.type)
            {
                case FormationType.Box: obj.GetComponent<BoxFormation>().boxValues = log.boxValues; break;
                case FormationType.Arrow: obj.GetComponent<ArrowFormation>().arrowValues = log.arrowValues; break;
                case FormationType.Triangle: obj.GetComponent<TriangleFormation>().triangleValues = log.triangleValues; break;
            }
            obj.GetComponent<BaseFormation>().target = _targetPos;
        }
    }
}
