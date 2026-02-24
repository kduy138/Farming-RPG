using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    private void Start()
    {
        if (player == null) return;

        player.events.OnDead += TriggerDead;
        player.events.OnMiningStarted += TriggerMining;
        player.events.OnMiningEnded += CancelMiningAnimation;
        player.events.OnCastStarted += TriggerCast;
        player.events.OnCastEnded += CancelCastAnimation;
        player.events.OnFishingStarted += SetFishingParameter;
        player.events.OnFishingEnded += SetFishingParameter;
        player.events.OnCombatStarted += SetCombatParameter;
        player.events.OnCombatEnded += SetCombatParameter;
    }

    private void Update()
    {
        SetSpeedParameter();
        SetHoldingItemSpeedParameter();
    }

    private void SetSpeedParameter()
    {
        animator.SetFloat("Speed", player.GetNormalizedSpeed(), 0.1f, Time.deltaTime);
    }

    private void SetHoldingItemSpeedParameter()
    {
        animator.SetFloat("HoldingItemSpeed", player.GetBlendSpeed());
    }

    private void SetFishingParameter(object sender, EventArgs e)
    {
        animator.SetBool("Fishing", player.GetCurrentFishingManager().IsFishing());
    }

    private void TriggerCast(object sender, EventArgs e)
    {
        FishingManager fm = player.GetCurrentFishingManager();
        animator.SetTrigger("Cast");
        fm.ResetCast();
    }

    public void CancelCastAnimation(object sender, EventArgs e)
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

    public void CancelMiningAnimation(object sender, EventArgs e)
    {
        animator.Play("Blend Tree Player Movement");
    }

    public void SetCombatParameter(object sender, EventArgs e)
    {
        animator.SetBool("Combat", player.GetPlayerCombat().IsInCombat());
    }
}
