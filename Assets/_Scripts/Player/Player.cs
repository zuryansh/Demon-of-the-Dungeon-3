using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player> 
{
    public Health PlayerHealth => healthScript;
    public event Action<float> EOnPointsChanged;

    Rigidbody2D rb;
    Camera cam;

    [SerializeField] SpriteRenderer visuals;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 movementVector;
    [SerializeField] Health healthScript;
    [SerializeField] AnimationClip runAnim;
    [SerializeField] AnimationClip idleAnim;
    [SerializeField] int souls;
    Vector3 towardsMouse;
    AnimationHelper animHelper;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animHelper = GetComponent<AnimationHelper>();
        cam = Camera.main;

    }

    void Update()
    {
        FlipSprite();
        movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.TogglePause();
        }
        if(Input.GetKeyDown(KeyCode.M)) 
        {
            UIManager.Instance.ToggleMinimap();
        }

    }



    private void FixedUpdate()
    {
        Move();
        if(rb.linearVelocity.sqrMagnitude>0 )
        {
            animHelper.ChangeAnimation(Animator.StringToHash(runAnim.name));
        }
        else
        {
            animHelper.ChangeAnimation(Animator.StringToHash(idleAnim.name));
        }
    }

    public void Move()
    {
        // get the maxSpeed
        Vector2 targetSpeed = movementVector * moveSpeed;

        // get the difference b/w current and max speed
        Vector2 speedDif = targetSpeed - rb.linearVelocity;
        rb.AddForce(speedDif, ForceMode2D.Impulse); // impulse feels more snappy but FORCE feels more floaty

    }



    public void OnDeath()
    {
        UIManager.Instance.GameOver();
    }

    protected virtual void FlipSprite()
    {
        if (visuals == null) Debug.LogWarning("Sprite renderer not found");
        else
        {

            if (rb.linearVelocity.x < 0) visuals.flipX = true;
            else visuals.flipX = false;
        }
    }

    [Button("Add Souls")]
    public void AddPoints(int n)
    {
        if (n == 0) return;
        souls += n;
        
        EOnPointsChanged?.Invoke(souls);
    }

}
