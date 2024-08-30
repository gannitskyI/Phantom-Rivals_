using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "ScriptableObjects/CarData", order = 1)]
public class CarDataSO : ScriptableObject
{
    public string taskCheckKey;
    public string key;
    public int price;
    public string names;
    public bool isPurchased;
    public bool isSelected = false;
    public bool isTaskAwarded = false;
    public string taskCheck;
    public GameObject carPrefab; // Добавляем поле для префаба машины
}
