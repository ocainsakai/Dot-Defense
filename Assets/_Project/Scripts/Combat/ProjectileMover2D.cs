using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileMover2D : TargetMover
{
    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        
        // Cài đặt Rigidbody 2D cho projectile
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.linearDamping = 0;
    }

    protected override float GetDistanceToTarget()
    {
        if (Target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, Target.position);
    }

    protected override Vector3 GetDirectionToTarget()
    {
        if (Target == null) return Vector3.zero;
        return (Target.position - transform.position).normalized;
    }

    protected override void ApplyVelocity(Vector3 direction, float speed)
    {
        rb.linearVelocity = (Vector2)direction * speed;
    }

    protected override void StopVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }
}