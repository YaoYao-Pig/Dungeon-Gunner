using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class WeaponStatusUI : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [Tooltip("Weapon Image")]
    [SerializeField] private Image weaponImage;
    [SerializeField] private Transform ammoHolderTransform;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Transform reloadBar;
    [SerializeField] private Image reloadBarImage;
    
    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;

    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;

    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs activeWeaponEventArgs)
    {
        SetActiveWeapon(activeWeaponEventArgs.weapon);
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLodedIcons(weapon);

        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadedBar(weapon);
        }
        else
        {
            ResetWeaponReloadedBar();
        }
        UpdateReloadText(weapon);
    }



    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    private void WeaponFired(Weapon weapon)
    {
        //弹药数
        UpdateAmmoText(weapon);
        //Icon显示
        UpdateAmmoLodedIcons(weapon);

        UpdateAmmoBar(weapon);
        //是否需要装填
        UpdateReloadText(weapon);
    }



    //开始重新装填
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadedBar(reloadWeaponEventArgs.weapon);
    }



    //装填完成
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon)
    {
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLodedIcons(weapon);
            ResetWeaponReloadedBar();
        }
    }


    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }


    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }


    private void UpdateAmmoLodedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();
        for(int i = 0; i < weapon.weaponClipRemainingAmmo; ++i)
        {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);
            ammoIconList.Add(ammoIcon);
        }
    }



    private void UpdateAmmoBar(Weapon weapon)
    {
        float restAmmoRate = (float)weapon.weaponClipRemainingAmmo / (float)weapon.weaponDetails.weaponClipAmmoCapacity;

        reloadBar.transform.localScale = new Vector3(restAmmoRate, 1f, 1f);
        reloadBarImage.color = new Color((1-restAmmoRate)*(Color.red.r), Color.green.g * restAmmoRate, Color.green.b * restAmmoRate, 1);
    }
    private void ClearAmmoLoadedIcons()
    {
        foreach(GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }
        ammoIconList.Clear();
    }


    private void UpdateWeaponReloadedBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
        {
            return;
        }
        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);
        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }


    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
            StopCoroutine(reloadWeaponCoroutine);
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        reloadBarImage.color = Color.red;

        
        while (currentWeapon.isWeaponReloading)
        {
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);
            yield return null;
        }
    }

    private void ResetWeaponReloadedBar()
    {
        StopReloadWeaponCoroutine();
        reloadBarImage.color = Color.green;
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }


    private void UpdateReloadText(Weapon weapon)
    {
        if (!(weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            reloadBarImage.color = Color.red;
            StopBlinkReloadTextCoroutine();
            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());

        }
        else
        {
            StopBlinkReloadText();
        }
    }


    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void StopBlinkReloadText()
    {
        StopBlinkReloadTextCoroutine();
        reloadText.text = "";
    }

    private void StopBlinkReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine!=null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }
}
