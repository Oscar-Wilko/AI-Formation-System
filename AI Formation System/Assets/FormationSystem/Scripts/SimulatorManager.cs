using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatorManager : MonoBehaviour
{
    public void Return() => SceneManager.LoadScene("SceneSelector");
}
