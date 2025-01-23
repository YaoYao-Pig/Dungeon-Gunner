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
    /// �ʼ�����е�����̬���Ƕ��رյģ������ű����ǿ��Խ���ġ�����BossRoom��Lock�ģ���������UnLock��,UnLock�ǰ�Trigger��ֱ�ӹص��ˣ�������û������Trigger��
    /// ����һ�����䣬�������������ţ����д�һ���Ž��룬����ս����ս�������󣬽�����Ż����´򿪣�֮ǰû�������Ż��ǻᱣ�ֹر�״̬�������previousOpen����;
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
