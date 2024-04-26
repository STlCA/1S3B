using Constants;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AnimationController : AnimationBase
{
    private GameManager gameManager;
    private Player player;
    private SceneChangeManager sceneChangeManager;

    private Vector2 saveDirection = Vector2.zero;
    private bool oneTimeSave = false;

    public Action<bool> useAnimEnd;

    [Header("PickUp")]
    public GameObject pickupItemPrefab;
    private GameObject pickupItem;
    private SpriteRenderer pickupItemSR;
    private Animator pickItemAnim;

    [Header("Carry")]
    public GameObject itemGO;
    private SpriteRenderer itemSR;

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;
        sceneChangeManager = gameManager.SceneChangeManager;


        pickupItem = Instantiate(pickupItemPrefab);
        pickupItemSR = pickupItem.GetComponentInChildren<SpriteRenderer>();
        pickItemAnim = pickupItem.GetComponentInChildren<Animator>();

        itemSR = itemGO.GetComponent<SpriteRenderer>();

        controller.OnMoveEvent += MoveAnimation;
        controller.OnClickEvent += UseAnimation;
        sceneChangeManager.mapChangeAction += StopAnimation;
        player.IsDeathAction += DeathAnimation;
    }

    /*    //private void Update()
        //{
        //    if (GameManager.Instance.SceneChangeManager.isMapChange == true)
        //        StopAnimation(true);
        //
        //    if (GameManager.Instance.SceneChangeManager.isMapChange == false && GameManager.Instance.SceneChangeManager.isReAnim == true)
        //        StopAnimation(false);
        //}*/

    public void AnimationSpeedChange(float speed)
    {
        foreach (var anim in animator)
        {
            anim.speed = speed;
        }
    }

    public void CarrySpriteChange(bool isCarry)
    {
        if (isCarry == true)
        {
            itemGO.SetActive(true);

            foreach (var anim in animator)
            {
                anim.enabled = false;
            }

            itemSR.sprite = player.selectItem.ItemInfo.SpriteList[0];
        }

        CarryAnimation(isCarry);
    }

    public void CarryAnimation(bool isCarry)
    {
        if (animator[0].GetBool(ConstantsString.IsStart) == false)
        {
            foreach (var anim in animator)
            {
                anim.SetBool(ConstantsString.IsStart, true);
            }
        }

        foreach (var anim in animator)
        {
            anim.enabled = true;

            anim.SetBool(ConstantsString.IsCarry, isCarry);

            if (anim.GetFloat(ConstantsString.InputX) == 0 && anim.GetFloat(ConstantsString.InputY) == 0)
            {
                if (anim.GetFloat(ConstantsString.SaveX) == 0 && anim.GetFloat(ConstantsString.SaveY) == 0)
                {
                    pickItemAnim.SetFloat(ConstantsString.SaveY, -1);
                    anim.SetFloat(ConstantsString.SaveY, -1);
                }
            }
        }
    }

    public void MoveAnimation(Vector2 direction, bool isUse = false, bool isCarry = false)
    {
        if (animator[0].GetBool(ConstantsString.IsStart) == false)
        {
            foreach (var anim in animator)
            {
                anim.SetBool(ConstantsString.IsStart, true);
            }
        }

        if (direction.magnitude <= 0f && isUse == false)
        {
            foreach (var anim in animator)
            {
                anim.SetFloat(ConstantsString.SaveX, animator[0].GetFloat(ConstantsString.InputX));
                anim.SetFloat(ConstantsString.SaveY, animator[0].GetFloat(ConstantsString.InputY));
            }
        }

        foreach (var anim in animator)
        {
            anim.SetFloat(ConstantsString.InputX, direction.x);
            anim.SetFloat(ConstantsString.InputY, direction.y);

            if (isCarry == true)
            {
                anim.SetBool(ConstantsString.IsCarry, true);
            }
            anim.SetBool(ConstantsString.IsWalking, direction.magnitude > 0f);
        }
    }

    public void StopAnimation(bool isChange)
    {
        if (isChange == true)
        {
            if (oneTimeSave == false)
            {
                saveDirection.x = animator[0].GetFloat(ConstantsString.InputX);
                saveDirection.y = animator[0].GetFloat(ConstantsString.InputY);

                oneTimeSave = true;
            }

            foreach (var anim in animator)
            {
                anim.SetBool(ConstantsString.IsWalking, false);

                anim.speed = 0;

                anim.SetFloat(ConstantsString.SaveX, saveDirection.x);
                anim.SetFloat(ConstantsString.SaveY, saveDirection.y);
            }
        }
        else if (isChange == false)
        {
            oneTimeSave = false;

            foreach (var anim in animator)
            {
                anim.speed = 1;
            }

            if (animator[0].GetFloat(ConstantsString.InputX) != 0 || animator[0].GetFloat(ConstantsString.InputY) != 0)
                MoveAnimation(saveDirection);
        }
    }

    public void UseAnimation(PlayerEquipmentType equipmentType, Vector2 pos)
    {
        useAnimEnd?.Invoke(true);

        if (animator[0].GetFloat(ConstantsString.SaveX) == 0 && animator[0].GetFloat(ConstantsString.SaveY) == 0)
        {
            foreach (var anim in animator)
            {
                anim.SetBool(ConstantsString.IsStart, true);
            }
        }

        foreach (var anim in animator)
        {
            anim.SetFloat(ConstantsString.SaveX, pos.x);
            anim.SetFloat(ConstantsString.SaveY, pos.y);
        }

        foreach (var anim in animator)
        {
            switch (equipmentType)
            {
                case PlayerEquipmentType.PickUp:
                    anim.SetTrigger("usePickUp");
                    break;
                case PlayerEquipmentType.Hoe:
                    anim.SetTrigger("useHoe");
                    break;
                case PlayerEquipmentType.Water:
                    anim.SetTrigger("useWater");
                    break;
                case PlayerEquipmentType.Axe:
                    anim.SetTrigger("useAxe");
                    break;
                case PlayerEquipmentType.PickAxe:
                    anim.SetTrigger("usePickAxe");
                    break;
                case PlayerEquipmentType.Sword:
                case PlayerEquipmentType.FishingRod:
                    break;
            }
        }

        StartCoroutine("StateDelay");
    }

    IEnumerator StateDelay()
    {
        yield return new WaitForSeconds(0.1f);

        float curAnimationTime = animator[0].GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(curAnimationTime);

        useAnimEnd?.Invoke(false);
    }


    public void DeathAnimation(bool value)
    {
        foreach (var anim in animator)
        {
            anim.SetBool(ConstantsString.IsDeath, value);
        }
    }

    public void PickUpAnim(Vector3Int target, Vector2 pos, Sprite pickUpSprite)
    {
        SortingGroup sr = player.GetComponentInChildren<SortingGroup>();

        Debug.Log(pos.y);

        if (pos.y > 0.5)
            pickupItemSR.sortingOrder = sr.sortingOrder - 5;
        else if (pos.y <= 0.5)
            pickupItemSR.sortingOrder = sr.sortingOrder + 10;

        Vector3 position = player.transform.position;
        position.y -= 0.5f;
        pickupItem.transform.position = position;
        pickupItemSR.sprite = pickUpSprite;
        pickItemAnim.SetFloat(ConstantsString.SaveX, pos.x);
        pickItemAnim.SetFloat(ConstantsString.SaveY, pos.y);
        pickItemAnim.SetTrigger("usePickUp");
    }

}
