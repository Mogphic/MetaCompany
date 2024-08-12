﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] public List<GameObject> inventory = new List<GameObject>();
    private InputManager inputManager;
    private PlayerAnimation anim;
    private int curInventoryContainerNum = 0;
    [SerializeField] private float selectScale = 0.7f;
    [SerializeField] private float normalScale = 0.55f;
    [SerializeField] private float scrollingDelay = 3f;
    private WaitForSeconds waitScrollingDelay;
    private Coroutine coroutine;

    private UIManager uiManager;
    
    public bool canAttack = true;
    private bool canScroll = true;

    public GameObject[] playerToolObj;
    public GameObject grabObj;

    private void Start()
    {
        uiManager = UIManager.instance;
        inputManager = InputManager.instance;
        anim = GetComponent<PlayerAnimation>();
        waitScrollingDelay = new WaitForSeconds(scrollingDelay);
        PutToolInventory();
    }
    
    private void Update()
    {
        if (canScroll && inputManager.IsScrollingEnter())
        {
            Vector2 scrollValue = inputManager.InventorySwitching();
            if (scrollValue.y != 0)
            {
                coroutine = StartCoroutine(ScrollDelay((int)scrollValue.normalized.y));
            }
        }
    }
    private IEnumerator ScrollDelay(int scrollValue)
    {
        canScroll = false;
        Switching(scrollValue);
        yield return waitScrollingDelay;
        canScroll = true;
    }
    private void PutToolInventory()
    {
        for (int i = 0; i < playerToolObj.Length; i++)
        {
            GameObject toolObj = Instantiate(playerToolObj[i], grabObj.transform.position, Quaternion.identity);
            //toolObj.name = playerToolObj[i].name;
            PutIndexInventory(toolObj, toolObj.GetComponent<InteractableObject>().icon);
            toolObj.GetComponent<Rigidbody>().isKinematic = true;
            toolObj.transform.position = grabObj.transform.position;
            toolObj.transform.rotation = grabObj.transform.rotation;
            toolObj.transform.SetParent(grabObj.transform);

            toolObj.GetComponent<BoxCollider>().enabled = false;
            if (toolObj.GetComponent<InteractableObject>().type.ToString() == "ITEM_ONEHAND")
            {
                anim.IsOneHand(true);
            }
            if (toolObj.GetComponent<InteractableObject>().type.ToString() == "ITEM_TWOHAND")
            {
                anim.IsTwoHand(true);
            }
        }
    }

    public bool CheckLight(int index)
    {
        if (inventory[index] != null)
        {
            print(inventory[index].name);
            if (inventory[index].name.Contains("Pro-Flashlight"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public bool CheckShovel(int index)
    {
        if (inventory[index] != null)
        {
            if (inventory[index].name.Contains("Shovel"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Switching(int scrollValue)
    {
        if (inventory[curInventoryContainerNum] != null)
        {
            inventory[curInventoryContainerNum].SetActive(false);
        }
        
        curInventoryContainerNum += scrollValue;
        curInventoryContainerNum = (curInventoryContainerNum+4) % 4;
        if (CheckLight(curInventoryContainerNum))
        {
            inputManager.canLight = true;
        }
        else
        {
            inputManager.canLight = false;
        }
        if (CheckShovel(curInventoryContainerNum))
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
        if (inventory[curInventoryContainerNum] != null)
        {
            inventory[curInventoryContainerNum].SetActive(true);
        }
        ChangePose(inventory[curInventoryContainerNum]);
        UIManager.instance.ResizeInventoryUI(curInventoryContainerNum);
    }

    private void ChangePose(GameObject obj)
    {
        if (obj == null)
        {
            anim.IsOneHand(false);
            anim.IsTwoHand(false);
        }
        else if(obj.GetComponent<InteractableObject>().type.ToString() == "ITEM_ONEHAND")
        {
            anim.IsTwoHand(false);
            anim.IsOneHand(true);
        }
        else if (obj.GetComponent<InteractableObject>().type.ToString() == "ITEM_TWOHAND")
        {
            anim.IsOneHand(false);
            anim.IsTwoHand(true);
        }
    }

    public void PutIndexInventory(GameObject obj, Sprite icon)
    {
        if (obj == null)
        {
            print("inven");
        }
        else 
        {
            //obj = obj.transform.GetChild(0).gameObject;
            // 인덱스 검사
            if (inventory[curInventoryContainerNum] != null)
            {
                inventory[curInventoryContainerNum].SetActive(false);
                ChangePose(obj);
                for (int i = 0; i < 4; i++)
                {
                    if (inventory[i] == null)
                    {
                        curInventoryContainerNum = i;
                        break;
                    }
                }
            }
            inventory[curInventoryContainerNum] = obj.gameObject;
            inventory[curInventoryContainerNum].SetActive(true);
            UIManager.instance.PutInInventoryUI(curInventoryContainerNum, icon);
            UIManager.instance.ResizeInventoryUI(curInventoryContainerNum);
        }
    }

    public void PullOutItem()
    {
        if (inventory[curInventoryContainerNum] != null)
        {
            inventory[curInventoryContainerNum].transform.SetParent(null);
            inventory[curInventoryContainerNum].GetComponent<BoxCollider>().enabled = true;
            inventory[curInventoryContainerNum].GetComponent<Rigidbody>().isKinematic = false;
            inventory[curInventoryContainerNum] = null;
            ChangePose(inventory[curInventoryContainerNum]);
            uiManager.PullOutInventoryUI(curInventoryContainerNum);
        }
    }

    //IEnumerator ScrollingTimer()
    //{
    //    yield return waitScrollingDelay;
    //    time = scrollingDelay;
    //}
}
