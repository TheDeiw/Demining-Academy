using UnityEngine;

namespace DeminingAcademy.Infrastructure
{
    public enum AppMode
    {
        AutoDetect,
        ForceVR,
        ForceSimulator
    }

    // A simple class to hold global settings
    [System.Serializable]
    public class GlobalAppSettings
    {
        [SerializeField] private AppMode _startupMode = AppMode.AutoDetect;
        public AppMode StartupMode => _startupMode;
    }
}