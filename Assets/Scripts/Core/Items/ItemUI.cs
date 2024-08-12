using UnityEngine;
using UnityEngine.UI;

namespace Core.Items
{
    public class ItemUI : MonoBehaviour
    {
        private static ItemUI _instance;

        [Header("References")] 
        [SerializeField] private GameObject itemUIHolder;
        [SerializeField] private Image itemImage;

        public static ItemUI Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<ItemUI>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }

        public void UpdateUI(Item item)
        {
            if (item == null)
            {
                itemUIHolder.SetActive(false);
                return;
            }
            itemUIHolder.SetActive(true);
            itemImage.sprite = item.itemImage;
        }
        
    }
}