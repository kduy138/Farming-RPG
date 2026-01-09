using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Player player;
    [SerializeField]
    private FishingManager fm;
    private MiningManager mm;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        mm = FindAnyObjectByType<MiningManager>();

        player.OnDead += TriggerDead;
        if (mm != null)
        {
            mm.OnMining += TriggerMining;
        }
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
        if(fm.IsCast())
        {
            animator.SetTrigger("Cast");
            fm.ResetCast();
        }
    }

    public void CancelCastAnimation()
    {
        animator.Play("Blend Tree Fishing", 0, 0f);
    }

    private void TriggerDead(object sender, EventArgs e)
    {
        animator.SetTrigger("Dead");
    }

    private void TriggerMining(object sender, EventArgs e)
    {
        animator.SetTrigger("Mining");
    }

    public void CancelMiningAnimation()
    {
        animator.Play("Blend Tree Player Movement");
    }
}
