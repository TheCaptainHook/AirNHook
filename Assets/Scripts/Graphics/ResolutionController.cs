using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropDown;

    private Resolution[] _resolutions;
    private List<Resolution> _filteredResolutions;

    private RefreshRate _currentRefreshRateRatio;
    private int _currentResolutionIndex = 0;
    
    private static string _x = "x";
    private static string _comma = ".";
    private static string _Hz = "Hz";

    private void Start()
    {
        _resolutions = Screen.resolutions;
        _filteredResolutions = new List<Resolution>();
        
        _resolutionDropDown.ClearOptions();
        _currentRefreshRateRatio = Screen.currentResolution.refreshRateRatio;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].refreshRateRatio.Equals(_currentRefreshRateRatio))
            {
                _filteredResolutions.Add(_resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            string _resolutionOption = _filteredResolutions[i].width + _x + _filteredResolutions[i].height + _comma +
                                       _filteredResolutions[i].refreshRateRatio + _Hz;
            options.Add(_resolutionOption);
            if (_filteredResolutions[i].width == Screen.width && _filteredResolutions[i].height == Screen.height)
            {
                _currentResolutionIndex = i;
            }
        }
        
        _resolutionDropDown.AddOptions(options);
        _resolutionDropDown.value = _currentResolutionIndex;
        _resolutionDropDown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
}
