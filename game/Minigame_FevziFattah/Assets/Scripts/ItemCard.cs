
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [SerializeField] public Item item;

    [SerializeField] TextMeshProUGUI nameField;
    [SerializeField] TextMeshProUGUI descriptionField;
    [SerializeField] Image iconField;

    PlayerItemManager playerItemManager;

    void Awake()
    {
        nameField.text = item.name;
        descriptionField.text = item.description;
        iconField.sprite = item.icon;
        playerItemManager = GameObject.FindObjectOfType<PlayerItemManager>();
    }

    void Update()
    {
        
    }

    public void ApplyItem(){
        playerItemManager.ApplyItem(item);
        Destroy(this.gameObject);
    }
}
