using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer globalMixer;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Resolution[] _resolutions;

        private void Start()
        {
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int index = 0;
            int currentResolutionIndex = 0;
            foreach (var value in _resolutions)
            {
                string option = $"{value.width}x{value.height}";
                if (value.width == Screen.currentResolution.width && value.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = index;
                }
                options.Add(option);
                index++;
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            
        }

        public void SetQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }

        public void SetResolution(int index)
        {
            Screen.SetResolution(_resolutions[index].width,_resolutions[index].height, Screen.fullScreen);
        }
        
        public void ToggleFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
        
        public void SetMusicVolume(float value)
        {
            globalMixer.SetFloat("musicVolume", value);
        }
        
        public void SetSfxVolume(float value)
        {
            globalMixer.SetFloat("sfxVolume", value);
        }
    }
}