using UnityEngine;
using Zenject;

namespace DeminingAcademy.Core
{
    public class GameManager : IInitializable
    {
        private readonly IModeSwitcherService _modeSwitcher;

        public GameManager(IModeSwitcherService modeSwitcher)
        {
            _modeSwitcher = modeSwitcher;
        }

        public void Initialize()
        {
            _modeSwitcher.ApplySelectedMode();
        
            Debug.Log("GameManager successfully applied.");
        }
    }
}
