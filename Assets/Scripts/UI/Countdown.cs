using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Countdown : MonoBehaviour
    {
        private static Countdown _instance;
    
        public static Countdown Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
    
                _instance = FindObjectOfType<Countdown>();
                if (_instance == null)
                {
                    return null;
                }
    
                return _instance;
            }
        }
        
        [SerializeField] private TMP_Text countdownText;

        public void ShowThree()
        {
            StartCoroutine(ShowMessage("3"));
        }
        public void ShowTwo()
        {
            StartCoroutine(ShowMessage("2"));
        }
        public void ShowOne()
        {
            StartCoroutine(ShowMessage("1"));
        }
        public void ShowGo()
        {
            StartCoroutine(ShowMessage("Go!"));
        }
        public void ShowFinish()
        {
            StartCoroutine(ShowMessage("Finished!"));
        }
        

        public IEnumerator ShowMessage(string message)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = message;
            yield return new WaitForSecondsRealtime(1f);
            countdownText.gameObject.SetActive(false); //use dotween to do a fade
        }
    }
}