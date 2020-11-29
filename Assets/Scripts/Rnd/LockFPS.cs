using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFPS : MonoBehaviour
{
    [SerializeField] private int targetFps;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFps;
    }

    void Update()
    {
        if (Application.targetFrameRate != targetFps)
            Application.targetFrameRate = targetFps;
    }
}
