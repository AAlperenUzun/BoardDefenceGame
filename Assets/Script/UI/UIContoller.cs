using UnityEngine;
using UnityEngine.Serialization;

public class UIContoller : MonoBehaviour
{
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private Transform inventoryItems;
    [SerializeField] private InventoryItem inventoryItemPrefab;
    
    public void Initialize(LevelData levelData)
    {
        gameOverScreen.Initialize();
        foreach (var itemType in levelData._allowedDefenceItemTypes)
        {
            for (int i = 0; i < itemType.Value; i++)
            {
                var temp = Instantiate(inventoryItemPrefab, inventoryItems);
                temp.SetType(itemType.Key);
            }
        }
        
    }

    public void Dispose()
    {
        gameOverScreen.Dispose();
    }

    public void ShowEndScreen(bool isSuccess)
    {
        gameOverScreen.Show(isSuccess);
    }
}
