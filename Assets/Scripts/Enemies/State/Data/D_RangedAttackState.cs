using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRangedAttackStateData", menuName = "Data/State Data/Ranged Attack State")]
/*public class D_RangedAttackStateData : ScriptableObject
{
    public float movingSpeed = 0f;

    public float projectileDamage = 10f;
    public float projectileSpeed = 13f;
    public float projectileLifeSpan;
}*/

public class D_RangedAttackState : ScriptableObject
{
    [Header("Owner's attribute")]
    public float attackTime;

    public BulletDetails[] bulletDetails;
}
