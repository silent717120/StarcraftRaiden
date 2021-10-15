using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGuidanceSystem : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [Header("�u�D����")]
    [SerializeField] float minBallisticAngle = 50f;
    [SerializeField] float maxBallisticAngle = 75f;

    float ballisticAngle;

    Vector3 targetDirection;

    public IEnumerator HomingCoroutine(GameObject target)
    {
        ballisticAngle = Random.Range(minBallisticAngle, maxBallisticAngle);

        while (gameObject.activeSelf)
        {
            if (target.activeSelf)
            {
                //�ؼФ�V�V�q
                targetDirection = target.transform.position - transform.position;
                //��V�ؼ�
                var angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
                transform.rotation *= Quaternion.Euler(0f, 0f, ballisticAngle);
                //���ʤl�u
                projectile.Move();
            }
            else
            {
                projectile.Move();
            }

            yield return null;
        }
     }
}
