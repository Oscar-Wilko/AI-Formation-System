using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public Vector2 minBound;
    public Vector2 maxBound;
    public Vector2Int quadrantCount;
    public GameObject fogPrefab;
    public float fogSpread;
    public float fogShift;
    public float fogScale;
    public float minRange;
    public float maxRange;
    public int quadCheckRange;
    private Dictionary<Vector2Int, List<GameObject>> fogDict = new Dictionary<Vector2Int, List<GameObject>>();
    private Dictionary<Vector2Int, GameObject> quadDict = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        GenerateFog();
    }

    private void Start()
    {
        UpdateFog(true);
    }

    public void Update()
    {
        UpdateFog();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(minBound.x, 0, minBound.y), new Vector3(minBound.x, 100, minBound.y));
        Gizmos.DrawLine(new Vector3(maxBound.x, 0, minBound.y), new Vector3(maxBound.x, 100, minBound.y));
        Gizmos.DrawLine(new Vector3(maxBound.x, 0, maxBound.y), new Vector3(maxBound.x, 100, maxBound.y));
        Gizmos.DrawLine(new Vector3(minBound.x, 0, maxBound.y), new Vector3(minBound.x, 100, maxBound.y));
    }

    private void GenerateFog()
    {
        float t = Time.realtimeSinceStartup;
        Vector2 counter = minBound;
        while(counter.y < maxBound.y)
        {
            while(counter.x < maxBound.x)
            {
                Vector3 smallShift = Vector3.one * fogShift;
                smallShift.x *= Mathf.Sin(Random.Range(0, 2 * Mathf.PI));
                smallShift.z *= Mathf.Cos(Random.Range(0, 2 * Mathf.PI));
                smallShift.x += counter.x;
                smallShift.z += counter.y;
                smallShift.y = Utils.RayDown(smallShift);
                Vector2Int quad = PosToQuad(smallShift);
                if (!fogDict.ContainsKey(quad))
                {
                    // New Quadrant
                    GameObject quadParent = new GameObject($"Quad: {quad}");
                    quadParent.transform.parent = transform;
                    quadDict.Add(quad, quadParent);
                    fogDict.Add(quad, new List<GameObject>());
                }
                GameObject newFog = Instantiate(fogPrefab);
                newFog.transform.parent = quadDict[quad].transform;
                newFog.transform.position = smallShift;
                newFog.transform.localScale = Vector3.one * fogScale;
                newFog.transform.localEulerAngles += new Vector3(0,Random.Range(0,360),0);
                fogDict[quad].Add(newFog);

                counter.x += fogSpread;
            }
            counter.x = minBound.x;
            counter.y += fogSpread;
        }
    }

    private void UpdateFog(bool snap = false)
    {
        List<Vector3> formationPositions = new List<Vector3>();
        HashSet<Vector2Int> quadChecks = new HashSet<Vector2Int>();
        foreach(BaseFormation form in FindObjectsOfType<BaseFormation>())
        {
            if (form.unitType == UnitType.Ally)
            {
                formationPositions.Add(form.transform.position);
                Vector2Int quad = PosToQuad(form.transform.position);
                quadChecks.Add(quad);
                for(int x = -quadCheckRange; x <= quadCheckRange; x ++)
                {
                    for (int y = -quadCheckRange; y <= quadCheckRange; y++)
                    {
                        quadChecks.Add(quad + new Vector2Int(x, y));
                    }
                }
            }
        }

        if (formationPositions.Count == 0) 
            return;

        foreach(Vector2Int quad in fogDict.Keys)
        {
            bool checkDistance = quadChecks.Contains(quad);
            foreach(GameObject obj in fogDict[quad])
            {
                float scaleTo = 1;
                if (checkDistance)
                {
                    // Distance based scaling
                    float closestDist = float.MaxValue;
                    foreach(Vector3 pos in formationPositions)
                    {
                        closestDist = Mathf.Min(closestDist, Vector3.Distance(pos, obj.transform.position));
                    }
                    scaleTo = (Mathf.Clamp(closestDist, minRange, maxRange) - minRange) / (maxRange-minRange);
                }
                if (snap)
                    obj.transform.localScale = Vector3.one * scaleTo * fogScale;
                else
                    obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, Vector3.one * scaleTo * fogScale, Time.deltaTime + 0.2f);
            }
        }
    }

    public Vector2Int PosToQuad(Vector3 position)
    {
        Vector2 totalShift = maxBound - minBound;
        Vector2 shift = new Vector2(position.x, position.z) - minBound;
        totalShift.x /= quadrantCount.x;
        totalShift.y /= quadrantCount.y;
        Vector2Int grid = new Vector2Int(Mathf.FloorToInt(shift.x / totalShift.x), Mathf.FloorToInt(shift.y / totalShift.y));
        return grid;
    }
}
