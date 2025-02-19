using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Cars
{
    public class ModelPickerUI : MonoBehaviour
    {
        public const string PlayerPrefsKey = "PlayerModel";

        [Header("References")] 
        [SerializeField] private Image modelImage;

        [SerializeField] private TMP_Text modelNameText;

        public CarModel currentModel;
        private int currentModelIndex;

        private void Start()
        {
            //need to update this to show the model picked previously
            
            currentModelIndex = GlobalModels.Instance.models.FindIndex(model => model.modelName == PlayerPrefs.GetString(PlayerPrefsKey,"Race Car"));
            Debug.Log(currentModelIndex);
            if (currentModelIndex == -1)
            {
                currentModelIndex = 0;
                Debug.Log("Failed to load player prefs");
            }
            currentModel = GlobalModels.Instance.models[currentModelIndex];
            UpdateModel();
        }

        public void NextModel()
        {
            currentModelIndex += 1;
            if (currentModelIndex == GlobalModels.Instance.models.Count)
            {
                currentModelIndex = 0;
            }
            UpdateModel();
        }
        public void PreviousModel()
        {
            currentModelIndex -= 1;
            if (currentModelIndex < 0)
            {
                currentModelIndex = GlobalModels.Instance.models.Count - 1;
            }
            UpdateModel();
        }

        private void UpdateModel()
        {
            currentModel = GlobalModels.Instance.models[currentModelIndex];
            modelImage.sprite = currentModel.modelImage;
            modelNameText.text = currentModel.modelName;
            PlayerPrefs.SetString(PlayerPrefsKey,currentModel.modelName);
            Debug.Log(PlayerPrefs.GetString(PlayerPrefsKey, "Unknown"));
        }
    }
}