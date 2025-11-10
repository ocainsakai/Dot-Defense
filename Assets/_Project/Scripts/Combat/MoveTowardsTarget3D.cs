using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveTowardsTarget3D : TargetMover
{
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake(); // Gọi hàm Awake của lớp cha
        rb = GetComponent<Rigidbody>();
        
        // Cài đặt Rigidbody 3D cho movement (không trọng lực, không cản)
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearDamping = 0;
    }

    protected override float GetDistanceToTarget()
    {
        if (Target == null) return float.MaxValue;
        return Vector3.Distance(transform.position, Target.position);
    }

    protected override Vector3 GetDirectionToTarget()
    {
        if (Target == null) return Vector3.zero;
        return (Target.position - transform.position).normalized;
    }

    protected override void ApplyVelocity(Vector3 direction, float speed)
    {
        rb.linearVelocity = direction * speed;
    }

    protected override void StopVelocity()
    {
        rb.linearVelocity = Vector3.zero;
    }
}