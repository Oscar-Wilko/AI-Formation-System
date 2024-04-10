using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimulatorManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _boxPrefab;
    [SerializeField] private GameObject _arrPrefab;
    [SerializeField] private GameObject _triPrefab;
    [Header("References")]
    [SerializeField] private List<GameObject> _maps;
    [SerializeField] private Transform _formationParent;
    [SerializeField] private Transform _startPosition;
    [Header("Tweaks")]
    [SerializeField] private float _distSpread;
    [SerializeField] private Vector3 _targetPos;

    public void Return()
    {
        SaveSystem.saveIndex = -1;
        SceneManager.LoadScene("SceneSelector");
    }

    public void Attack()
    {
        Debug.Log("Set to attacking mode");
        foreach(BaseFormation formation in FindObjectsOfType<BaseFormation>())
        {
            formation.SetFightState(FightState.Attack);
        }
    }

    public void Hold()
    {
        Debug.Log("Set to holding position mode");
        foreach (BaseFormation formation in FindObjectsOfType<BaseFormation>())
        {
            formation.SetFightState(FightState.Hold);
        }
    }

    public void Flank()
    {
        Debug.Log("Set to flanking mode");
        foreach (BaseFormation formation in FindObjectsOfType<BaseFormation>())
        {
            formation.SetFightState(FightState.Flank);
        }
    }

    public void SplitUp()
    {
        Debug.Log("Spliting up formations");
        foreach (BaseFormation formation in FindObjectsOfType<BaseFormation>())
        {
            formation.SetFightState(FightState.Attack);
        }
    }

    private void Awake()
    {
        SelectMap();

        if (SaveSystem.saveIndex >= 0)
            Load();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_targetPos, 2);
    }

    private void SelectMap()
    {
        int randomMap = Random.Range(0,_maps.Count);
        for(int i = 0; i < _maps.Count; i ++)
        {
            _maps[i].SetActive(randomMap == i);
        }
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

        GameObject curLeader = null;
        float distFromTarget = float.MaxValue;
        List<BaseFormation> formations = new List<BaseFormation>();
        // Go through each formation stored in formation logs
        foreach (FormationLog log in info.logs)
        {
            // Instantiate formation based on type
            GameObject obj;
            switch (log.type)
            {
                case FormationType.Box: obj = Instantiate(_boxPrefab); break;
                case FormationType.Arrow: obj = Instantiate(_arrPrefab); break;
                case FormationType.Triangle: obj = Instantiate(_triPrefab); break;
                default: continue;
            }
            obj.transform.SetParent(_formationParent);

            // Set its position
            Vector2 shiftPos = Utils.Rotate(log.position, Mathf.Deg2Rad * -_startPosition.localEulerAngles.y);
            Vector3 pos = _startPosition.position;
            pos += new Vector3(shiftPos.x, 0, shiftPos.y) * _distSpread;
            pos.y = Utils.RayDown(new Vector2(pos.x, pos.z));
            obj.transform.position = pos;

            // Update formation values
            switch (log.type)
            {
                case FormationType.Box: obj.GetComponent<BoxFormation>().boxValues = log.boxValues; break;
                case FormationType.Arrow: obj.GetComponent<ArrowFormation>().arrowValues = log.arrowValues; break;
                case FormationType.Triangle: obj.GetComponent<TriangleFormation>().triangleValues = log.triangleValues; break;
            }

            // Log the closest formation to the target
            obj.GetComponent<BaseFormation>().endTarget = _targetPos;
            float temp = Vector3.Distance(pos, _targetPos);
            if (temp < distFromTarget)
            {
                distFromTarget = temp;
                curLeader = obj;
            }
            formations.Add(obj.GetComponent<BaseFormation>());
        }
        // State who's the leader
        curLeader.GetComponent<BaseFormation>().isLeader = true;
        foreach(BaseFormation formation in formations)
        {
            if (formation.transform != curLeader.transform)
            {
                formation.followTarget = curLeader;
            }
        }
    }
}
