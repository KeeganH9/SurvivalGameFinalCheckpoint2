using UnityEngine;
using TMPro;

public class ResourceCounter : MonoBehaviour
{
    public TextMeshProUGUI AppleText;
    public TextMeshProUGUI OreText;
    public TextMeshProUGUI MushroomText;
    public TextMeshProUGUI WoodText;

    private int apples;
    private int ore;
    private int mushrooms;
    private int wood;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    private void UpdateUI(){
        AppleText.text = "Apples: " + apples;
        OreText.text = "Ore: " + ore;
        MushroomText.text = "Mushrooms: " + mushrooms;
        WoodText.text = "Wood: " + wood;

    }

    public void AddResource(string resourceName, int amount){
        if(resourceName == "Apple"){
            apples += amount;
        } else if(resourceName == "Ore"){
            ore += amount;
        } else if(resourceName == "Mushroom"){
            mushrooms += amount;
        } else if(resourceName == "Wood"){
            wood += amount;
        }
        UpdateUI();
    }
}
