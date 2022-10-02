using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIClock : MonoBehaviour
{
    [SerializeField]
    private Image imgHand;
    [SerializeField]
    private Text txtSeconds;

    private float originalEulerAnglesZ;
    private float targetEulerAnglesZ;
    private float roundDuration = 1f;
    private float roundTimer = 0f;
    private bool isAnimating = false;
    private UnityAction callback;

    private void RotateHand()
    {
        float eulerAnglesZ = Mathf.Lerp(originalEulerAnglesZ, targetEulerAnglesZ, roundTimer / roundDuration);
        SetHandRotation(eulerAnglesZ);
    }

    private void SetHandRotation(float eulerAnglesZ)
    {
        Vector3 eulerAngles = imgHand.transform.localEulerAngles;
        eulerAngles.z = eulerAnglesZ;
        imgHand.transform.localEulerAngles = eulerAngles;
    }

    private void UpdateSeconds()
    {
        SetSeconds(roundTimer);
    }

    private void SetSeconds(float time)
    {
        txtSeconds.text = $"{time:F2}s";
    }

    public void AnimateRound(UnityAction action)
    {
        callback = action;
        isAnimating = true;
        roundTimer = 0f;
        originalEulerAnglesZ = imgHand.transform.localEulerAngles.z;
        targetEulerAnglesZ = -360f;
    }

    private void Update()
    {
        if (isAnimating)
        {
            roundTimer += Time.deltaTime;
            if (roundTimer > roundDuration)
            {
                SetSeconds(1);
                SetHandRotation(targetEulerAnglesZ);
                isAnimating = false;
                callback();
                return;
            }
            UpdateSeconds();
            RotateHand();
        }
    }
}
