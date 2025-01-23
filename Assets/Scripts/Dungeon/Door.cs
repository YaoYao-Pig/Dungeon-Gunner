using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [SerializeField] private BoxCollider2D doorCollider;
    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;

    private Animator animator;

    private void Awake()
    {
        doorCollider.enabled = false;

        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        animator.SetBool(Settings.open, isOpen);
    }

    /// <summary>
    /// 最开始，所有的门形态上是都关闭的，但是门本身是可以进入的。除了BossRoom是Lock的，其他都是UnLock的,UnLock是把Trigger给直接关掉了，这样就没法触发Trigger了
    /// 对于一个房间，假设他有三个门，其中从一个门进入，触发战斗，战斗结束后，进入的门会重新打开，之前没经过的门还是会保持关闭状态，这就是previousOpen的用途
    /// </summary>
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            animator.SetBool(Settings.open, true);

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
        }
    }

    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }

    public void UnLockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;
        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }
}
