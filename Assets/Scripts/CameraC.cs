using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraC : MonoBehaviour
{
    Unity.Cinemachine.CinemachineVirtualCamera vcam;
    Unity.Cinemachine.CinemachineBasicMultiChannelPerlin noisePerlin;
    bool isShaking = false;
    [SerializeField] float shakeTime;
    float shakeTimeCountdown;


    private void Awake()
    {
        vcam = GetComponent<Unity.Cinemachine.CinemachineVirtualCamera>();
        noisePerlin = vcam.GetCinemachineComponent<Unity.Cinemachine.CinemachineBasicMultiChannelPerlin>();
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
        noisePerlin.AmplitudeGain = AmplitudeGain;
        noisePerlin.FrequencyGain = FrequencyGain;
        shakeTimeCountdown = shakeTime;
        isShaking = true;
    }

    public void CameraStopShake()
    {
        noisePerlin.AmplitudeGain = 0f;
        noisePerlin.FrequencyGain = 0f;

        isShaking = false;
    }

}
