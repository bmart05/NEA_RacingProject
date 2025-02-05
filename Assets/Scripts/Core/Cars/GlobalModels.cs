using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Cars
{
    public class GlobalModels : MonoBehaviour
    {
        private static GlobalModels _instance;

        public static GlobalModels Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<GlobalModels>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }


        [field: SerializeField] public List<CarModel> models { get; private set; } = new List<CarModel>();

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public CarModel GetModelByName(string _name)
        {
            Debug.Log(_name);
            CarModel model = models.Find(carModel => carModel.modelName == _name);
            Debug.Log(model.modelName);
            return model;
        }
        
    }
}