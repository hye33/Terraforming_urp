using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private CinemachineImpulseSource c_source;
    public enum impulseCameraType { crash, burst }

    public enum ForestMapCameraState { FollowPlayer, MiddleBoss };
    private GameObject[] _cameraStates;
    private int _currentCameraState;

    private void Awake()
    {
        SetCameraStates();
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        GetComponentInChildren<CinemachineVirtualCamera>().Follow = player.transform;
    }

    private void SetCameraStates()
    {
        _cameraStates = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _cameraStates[i] = transform.GetChild(i).gameObject;
            _cameraStates[i].SetActive(false);
            Debug.Log(transform.GetChild(i).name);
        }
        _currentCameraState = 0;
        _cameraStates[_currentCameraState].SetActive(true);
    }


    public void ImpulseCamera(impulseCameraType type)
    {
        switch (type)
        {
            case impulseCameraType.crash:
                c_source.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Explosion;
                break;
            case impulseCameraType.burst:
                c_source.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump;
                break;
        }
        c_source.GenerateImpulse();
    }

    // 각 맵에 맞는 enum을 찾아 원하는 state를 int 형태로 전달
    public void ChangeCameraState(int stateIndex)
    {
        _cameraStates[_currentCameraState].SetActive(false);
        _cameraStates[stateIndex].SetActive(true);
    }
}
