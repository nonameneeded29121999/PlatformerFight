using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeAttackStateData", menuName = "Data/State Data/Melee Attack State")]
public class D_MeleeAttackState : ScriptableObject
{
    public float movingSpeed = 0f;

    public float attackRadius = 0.5f;
    public float attackMultiplier = 10f;

    public LayerMask playerLayer;
}
