using UnityEngine;

public class CraftManager : Singleton<CraftManager>
{
    [SerializeField] private GameObject inventoryObj;
    [SerializeField] private GameObject nodeInventoryObj;
    [SerializeField] private GameObject craftCanvas;
    public GameObject currentlyCrafting { get; set; }
    public void OnEnable()
    {
        craftCanvas.SetActive(true);
        inventoryObj.GetComponent<RectTransform>().SetParent(transform);
        nodeInventoryObj.GetComponent<RectTransform>().SetParent(transform);
    }

    

}
