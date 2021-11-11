using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Test_NormalATK3Data", menuName = "Skill/TestCharacter/NormalATK3")]

public class Test_NormalATK3 : Skill
{
    public float attackRadius;

    public float invicibleTime;

    public override void Execute()
    {
        executor.characterAnimation.PlayAnim(animationName);
    }

    public override void Damage()
    {
        if (executor.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, LayerMask.NameToLayer("Damageable"));

            foreach (Collider2D enemy in hitEnemies)
            {
                if (!enemy.gameObject.CompareTag(executor.gameObject.tag))
                    enemy.transform.SendMessage("TakeDamage");
            }
        }
    }

    public override void DrawGizmo()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRadius);
    }
}
