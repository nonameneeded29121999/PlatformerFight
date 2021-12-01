using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : State
{
    protected D_DodgeState stateData;

    protected bool performCloseRangeAction;

    protected bool isPlayerInMaxAgroRange;

    protected bool isGrounded;

    protected bool isDodgeOver;

    public DodgeState(FiniteStateMachine stateMachine, BaseEnemy entity, string animBoolName, D_DodgeState stateData) 
        : base(stateMachine, entity, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxArgoRange();
        isGrounded = entity.IsGrounded;
    }

    public override void Enter()
    {
        base.Enter();

        isDodgeOver = false;

        int facingDirection = entity.facingRight ? 1 : -1;
        entity.SetVelocity(stateData.dodgeSpeed, stateData.dodgeAngle, facingDirection);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + stateData.dodgeTime && isGrounded)
        {
            isDodgeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
