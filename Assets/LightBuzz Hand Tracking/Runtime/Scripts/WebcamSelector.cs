using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LightBuzz.HandTracking
{
    public class WebcamSelector : MonoBehaviour
    {
        [SerializeField] private WebcamSource _webcamSource;

        private void Start()
        {
            if (_webcamSource == null)
            {
                _webcamSource = FindObjectOfType<WebcamSource>();
            }

            if (_webcamSource == null)
            {
                Debug.LogError("WebcamManager not found.");
            }

            StartCoroutine(UpdateDropdown());
        }

        private IEnumerator UpdateDropdown()
        {
            while (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return new WaitForSeconds(0.1f);
            }

            var dropdown = GetComponent<Dropdown>();
            
            if (dropdown == null)
            {
                Debug.LogError("Dropdown not found.");
                yield break;
            }

            dropdown.ClearOptions();

            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

            for (var i = 0; i < WebCamTexture.devices.Length; i++)
            {
                options.Add(new Dropdown.OptionData(WebCamTexture.devices[i].name));
            }

            dropdown.AddOptions(options);
            dropdown.value = _webcamSource.WebcamIndex;

            DetermineFrontCamera(dropdown);
        }

        public void OnWebcamSelected(int index)
        {
            _webcamSource.SwitchCamera(index);
        }

        public bool FrontCameraEnabled = true;
        public void DetermineFrontCamera(Dropdown DropdownMenu)
        {
            Debug.Log("Dropdown selection: " + DropdownMenu.captionText.text.ToLower());
            if (DropdownMenu.captionText.text.ToLower().Contains("front"))
            {
                FrontCameraEnabled = true;
            } else
            {
                FrontCameraEnabled = false;
            }
        }
    }
}