using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatedHands : MonoBehaviour
{
    public InputActionProperty triggerValue;
    public InputActionProperty gripValue;
    
    public Animator handAnimator;


    void Update()
    {
        float trigger = triggerValue.action.ReadValue<float>();
        float grip = gripValue.action.ReadValue<float>();
        
        handAnimator.SetFloat("Trigger", trigger);
        handAnimator.SetFloat("Grip", grip);
    }
}
