using UnityEngine;

namespace Core.Cars
{
    [CreateAssetMenu(menuName = "New Car", fileName = "New Car")]
    public class CarModel : ScriptableObject
    {
        public string modelName;
        public Sprite modelImage;
        public GameObject modelPrefab;
    }
}