using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    private List<GameObject> healthHeartsList = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthBar(healthEventArgs);
    }

    private void SetHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();
        int healthHearts = Mathf.FloorToInt(healthEventArgs.healthPrecent * 100f / 20f);

        float floatHearts = (healthEventArgs.healthPrecent * 100f / 20f)-(float)healthHearts;
        //Debug.Log(floatHearts);
        for(int i = 0; i < healthHearts; ++i) 
        { 
            GameObject heart=Instantiate(GameResources.Instance.heartPrefab,transform);
            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing* i, 0f);
            healthHeartsList.Add(heart);
        }
        if (floatHearts > 0.1f)
        {
            GameObject heart = Instantiate(GameResources.Instance.heartPrefab, transform);
            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing* healthHearts, 0f);
            heart.gameObject.GetComponent<Image>().fillAmount = floatHearts;
            healthHeartsList.Add(heart);
        }
    }

    private void ClearHealthBar()
    {
        foreach(GameObject heartIcon in healthHeartsList)
        {
            Destroy(heartIcon);
        }
        healthHeartsList.Clear();
    }
}
