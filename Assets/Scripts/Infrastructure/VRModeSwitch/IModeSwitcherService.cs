namespace DeminingAcademy.Infrastructure.VRModeSwitch
{
    public interface IModeSwitcherService
    {
        void ApplySelectedMode();
        void EnableVRMode();
        void EnableSimulatorMode();
    }
}
