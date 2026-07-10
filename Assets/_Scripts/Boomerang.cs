using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boomerang : Projectile
{
    bool isReturning => Time.time - startTime > lingerTime || projHit >= pierceCount;

    [SerializeField] float lingerTime =2f;
    [SerializeField] float returnFactor;
    [SerializeField] ForceMode2D forceMode;

    float startTime;
    float ogSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isReturning)
        {
            ReturnToSender();
        }
    }

    public override void Launch(Vector3 vel)
    {
        base.Launch(vel);
        startTime = Time.time;
        ogSpeed = rb.linearVelocity.magnitude;
    }

    public void ReturnToSender()
    {
        Vector2 dir = (Sender.position - transform.position).normalized;

        Vector2 targetSpeed = dir.normalized * ogSpeed * returnFactor;
        Vector2 speedDif = targetSpeed - rb.linearVelocity;
        rb.AddForce(speedDif, forceMode); // impulse feels more snappy but FORCE feels more floaty
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( isReturning && collision.transform == Sender.transform)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnPierceFinish()
    {
        // do nothign
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
}
