using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarDisplayUI : MonoBehaviour
{
    [SerializeField] private float energyToPixel = 30f;
    [SerializeField] private RectTransform filledBar;
    [SerializeField] private RectTransform barBackground;
    [SerializeField] private AnimationCurve bounceCurve;
    [SerializeField] private UnityEngine.UI.Image vrAdvancedBar;
    [SerializeField] private UnityEngine.UI.Image vrCompactBar;
    private Kobold targetKobold;

    private float targetWidth;
    private float targetBackgroundWidth;
    private bool running = false;   
    private void OnEnable()
    {
        targetKobold = GetComponentInParent<Kobold>();
        targetKobold.energyChanged += OnEnergyChanged;
        running = false;
        OnEnergyChanged(targetKobold.GetEnergy(), targetKobold.GetMaxEnergy());
    }

    private void OnDisable()
    {
        targetKobold.energyChanged -= OnEnergyChanged;
        running = false;
    }

    void OnEnergyChanged(float energy, float maxEnergy)
    {
        targetWidth = Mathf.Min(energy * energyToPixel, 3000f);
        targetBackgroundWidth = Mathf.Min(maxEnergy * energyToPixel, 3000f);

       // vrCompactBar.fillAmount = Mathf.Lerp(1, 0, (maxEnergy - energy) / maxEnergy);             
       // vrAdvancedBar.fillAmount = Mathf.Lerp(1, 0, (maxEnergy - energy) / maxEnergy);

        if (!running)
        {
            StartCoroutine(EnergyLerpRoutine(filledBar.sizeDelta.x, barBackground.sizeDelta.x));
        }
    }

    IEnumerator EnergyLerpRoutine(float startWidth, float startBackgroundWidth)
    {
        running = true;
        float startTime = Time.time;
        float duration = 1f;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            float bounceSample = bounceCurve.Evaluate(t);
            filledBar.sizeDelta = new Vector2(Mathf.Clamp(Mathf.LerpUnclamped(startWidth, targetWidth, bounceSample), 0f, float.MaxValue),
                filledBar.sizeDelta.y);
            barBackground.sizeDelta = new Vector2(Mathf.Clamp(Mathf.LerpUnclamped(startBackgroundWidth, targetBackgroundWidth, bounceSample), 0f, float.MaxValue),
                barBackground.sizeDelta.y);

            vrCompactBar.fillAmount = Mathf.Clamp(Mathf.LerpUnclamped((targetBackgroundWidth - startWidth) / targetBackgroundWidth, (targetBackgroundWidth - targetWidth) / targetBackgroundWidth, bounceSample), 0f, 1); 
            vrAdvancedBar.fillAmount = Mathf.Clamp(Mathf.LerpUnclamped((targetBackgroundWidth - startWidth) / targetBackgroundWidth, (targetBackgroundWidth - targetWidth) / targetBackgroundWidth, bounceSample), 0f, 1); 

            yield return null;
        }
        filledBar.sizeDelta = new Vector2(targetWidth, filledBar.sizeDelta.y);
        barBackground.sizeDelta = new Vector2(targetBackgroundWidth, barBackground.sizeDelta.y);
        running = false;
    }
}
