using UnityEngine;

public class CraftManager : Singleton<CraftManager>
{
    [SerializeField] private GameObject inventoryObj;
    [SerializeField] private GameObject nodeInventoryObj;
    [SerializeField] private GameObject craftCanvas;
    [SerializeField] public GameObject floatingNode = null;
    public GameObject currentlyCrafting { get; set; }
    public override void Populate()
    {
        
        craftCanvas.SetActive(true);
        inventoryObj.GetComponent<RectTransform>().SetParent(transform);
        nodeInventoryObj.GetComponent<RectTransform>().SetParent(transform);
    }

    

}
