using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
[DisallowMultipleComponent]
public class MiniMap : MonoBehaviour
{
    [SerializeField] private GameObject miniMapPlayer;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        SpriteRenderer spriteRenderer = miniMapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMiniMapIcon();
        }
    }

    private void Update()
    {
        if (playerTransform != null && miniMapPlayer != null)
        {
            miniMapPlayer.transform.position = playerTransform.position;
        }
    }
}
