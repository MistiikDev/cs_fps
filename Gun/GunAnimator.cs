using UnityEngine;

public class GunAnimator
{
    private Animator gunAnimator;
    
    public void Init(Gun _gun)
    {
        this.SetAnimator(_gun.GetComponent<Animator>());
        this.SetEnabled(false);
    }
    
    public void SetAnimator(Animator animator)
    {
        gunAnimator = animator;
    }
    
    public void SetEnabled(bool enabled)
    {
        gunAnimator.enabled = enabled;
    }
    
    public void PlayAnimation(string AnimationName)
    {
        string animationTrigger = AnimationName + "Trigger";
        gunAnimator.SetTrigger(animationTrigger);
    }
}
