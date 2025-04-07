using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour
{
    PlayerController playerController;
    GameController gameController;

    void Awake()
    {
        playerController =  GetComponent<PlayerController>();
        gameController = GameObject.FindAnyObjectByType<GameController>();
    }

    public void ApplyItem(Item item){
        switch(item.itemName){
            case "Try-Again":
                playerController.ricochetBounces++;
            break;
            case "Hotfix":
                playerController.regeneration++;
            break;
            case "Exception Seeker":
               playerController.homing = true;
               gameController.itemCards.Remove(findGameobjectByItem(item));
            break;
            case "Code Breaker":
               playerController.pierce++;
            break;

        }
    }

    GameObject findGameobjectByItem(Item item){
        foreach(GameObject itemCard in gameController.itemCards){
                if(itemCard.GetComponent<ItemCard>().item == item) return itemCard;
        }
        return null;
    }

}
