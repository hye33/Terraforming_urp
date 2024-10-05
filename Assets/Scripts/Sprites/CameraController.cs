using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private CinemachineImpulseSource c_source;
    public enum impulseCameraType { crash, burst }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        GetComponentInChildren<CinemachineVirtualCamera>().Follow = player.transform;
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
}
