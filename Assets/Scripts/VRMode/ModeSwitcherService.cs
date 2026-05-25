using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using DeminingAcademy.Infrastructure;
using Zenject;

public class ModeSwitcherService : MonoBehaviour, IModeSwitcherService
{
    [Header("VR Mode Setup")]
    [SerializeField] private GameObject _vrPlayer;
    [SerializeField] private GameObject _vrCharacter;

    [Header("Simulator Mode Setup")]
    [SerializeField] private GameObject _xrDeviceSimulator;
    [SerializeField] private GameObject _xrOrigin;
    
    private GlobalAppSettings _globalSettings;

    // Inject global settings into the local component
    [Inject]
    public void Construct(GlobalAppSettings globalSettings)
    {
        _globalSettings = globalSettings;
    }

    public void ApplySelectedMode()
    {
        // Now it reads the mode from the global settings
        switch (_globalSettings.StartupMode)
        {
            case AppMode.ForceVR:
                EnableVRMode();
                break;
            case AppMode.ForceSimulator:
                EnableSimulatorMode();
                break;
            case AppMode.AutoDetect:
            default:
                if (IsVRHeadsetConnected())
                    EnableVRMode();
                else
                    EnableSimulatorMode();
                break;
        }
    }

    public void EnableVRMode()
    {
        _xrDeviceSimulator.SetActive(false);
        _xrOrigin.SetActive(false);
        
        _vrPlayer.SetActive(true);
        _vrCharacter.SetActive(true);
    
        Debug.Log("Switched to VR Mode");
    }

    public void EnableSimulatorMode()
    {
        _vrPlayer.SetActive(false);
        _vrCharacter.SetActive(false);
        
        _xrDeviceSimulator.SetActive(true);
        _xrOrigin.SetActive(true);
    
        Debug.Log("Switched to Simulator Mode");
    }

    // Internal Method for checking the headset 
    private bool IsVRHeadsetConnected()
    {
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, inputDevices);
        
        return inputDevices.Count > 0;
    }
}
