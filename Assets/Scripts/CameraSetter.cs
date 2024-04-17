using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    void Start()
    {
        CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera>();
        GameplayStateManager.Instance.mainCamera = cam;
        GameplayStateManager.Instance.SetCamera(cam);
    }
}
