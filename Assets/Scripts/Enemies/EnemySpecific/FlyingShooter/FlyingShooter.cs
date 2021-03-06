using PlatformerFight.CharacterThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShooter : BaseEnemy
{
    public FlyingShooter_MoveState moveState { get; private set; }

    public FlyingShooter_DeadState deadState { get; private set; }

    public FlyingShooter_RangedAttackState rangedAttackState { get; private set; }

    [SerializeField]
    private D_MoveState moveStateData;

    [SerializeField]
    private D_DeadState deadStateData;

    [SerializeField]
    private D_RangedAttackState rangedAttackStateData;

    [SerializeField]
    private Transform rangedAttackPosition;

    [SerializeField]
    private Transform playerCircleDetector;

    [SerializeField]
    protected VoidEventChannelSO OnDefeatedEvent;

    protected override void Start()
    {
        base.Start();

        moveState = new FlyingShooter_MoveState(stateMachine, this, "Move", moveStateData);
        deadState = new FlyingShooter_DeadState(stateMachine, this, "Dead", deadStateData);
        rangedAttackState = new FlyingShooter_RangedAttackState(stateMachine, this, "RangedAttack", rangedAttackPosition, rangedAttackStateData);

        stateMachine.Initialize(moveState);
    }

    public override void Kill()
    {
        stateMachine.ChangeState(deadState);
    }

    protected override void TakeDamage(AttackDetails attackDetails)
    {
        base.TakeDamage(attackDetails);

        if (IsDead)
        {
            OnDefeatedEvent.RaiseEvent();
            OnAddScore.RaiseEvent(CalculateScoreAfterDefeat(entityData.scoreYield));
            Kill();
        }
    }
}
