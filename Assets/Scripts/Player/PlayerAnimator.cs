using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Player player;
    [SerializeField]
    private FishingManager fm;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        SetSpeedParameter();
        SetHoldingItemSpeedParameter();
        SetFishingParameter();
        TriggerCast();
    }

    private void SetSpeedParameter()
    {
        animator.SetFloat("Speed", player.GetNormalizedSpeed(), 0.1f, Time.deltaTime);
    }

    private void SetHoldingItemSpeedParameter()
    {
        animator.SetFloat("HoldingItemSpeed", player.GetBlendSpeed());
    }

    private void SetFishingParameter()
    {
        animator.SetBool("Fishing", fm.IsFishing());
    }

    private void TriggerCast()
    {
        if(fm.IsCast() == true)
        {
            animator.SetTrigger("Cast");
            fm.ResetCast();
        }
    }

    public void CancelCastAnimation()
    {
        animator.Play("Blend Tree Fishing", 0, 0f);
    }
}
