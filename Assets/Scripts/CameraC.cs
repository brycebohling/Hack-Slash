using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraC : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    CinemachineBasicMultiChannelPerlin noisePerlin;
    bool isShaking = false;
    [SerializeField] float shakeTime;
    float shakeTimeCountdown;


    private void Awake() 
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        noisePerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();  
    }

    private void Update()
    {
        if (isShaking)
        {
            shakeTimeCountdown -= Time.deltaTime;

            if (shakeTimeCountdown <= 0f)
            {
                CameraStopShake();
            }
        }
    }

    public void CameraStartShake(float AmplitudeGain, float FrequencyGain)
    {
        noisePerlin.m_AmplitudeGain = AmplitudeGain;
        noisePerlin.m_FrequencyGain = FrequencyGain;
        shakeTimeCountdown = shakeTime;
        isShaking = true;
    }

    public void CameraStopShake()
    {
        noisePerlin.m_AmplitudeGain = 0f;
        noisePerlin.m_FrequencyGain = 0f;
        isShaking = false;
    }

}
