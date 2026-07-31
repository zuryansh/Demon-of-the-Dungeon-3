using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Singleton<Player> 
{
    public Health HealthScript => healthScript;
    public PlayerCombat CombatScript=> combatScript;
    public UnityEvent<float> EOnPointsChanged;
    public Vector2 MouseAndJoystickDir => mouseDir;

    PlayerInput inputManager;
    Rigidbody2D rb;
    Camera cam;

    [SerializeField] SpriteRenderer visuals;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 movementVector;
    [SerializeField] AnimationClip runAnim;
    [SerializeField] AnimationClip idleAnim;
    [SerializeField] int souls;

    PlayerCombat combatScript;
    Health healthScript;
    Vector3 towardsMouse;
    AnimationHelper animHelper;
    Vector2 mouseDir;

    protected override void Awake()
    {
        base.Awake();
        inputManager = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animHelper = GetComponent<AnimationHelper>();
        if (healthScript == null) healthScript = GetComponent<Health>();
        if(combatScript == null) combatScript = GetComponent<PlayerCombat>();
     }

    void Start()
    {
        canMove = false;
        if (FindFirstObjectByType<RoomAssembler>() != null) RoomAssembler.EOnAssemblyFinished += UnlockMovement;
        else UnlockMovement(null);

        UIManager.Instance.OnGamePauseToggle += HandlePause;
        cam = Camera.main;

    }

    void UnlockMovement(IReadOnlyList<Room> rooms)
    {
        canMove = true;
    }

    public void HandleMovementInput(InputAction.CallbackContext c)
    {
        movementVector = c.ReadValue<Vector2>();
    }
    public void HandleMapInput(InputAction.CallbackContext c)
    {
        if(c.performed) UIManager.Instance.ToggleMinimap();
    }

    public void HandlePauseInput(InputAction.CallbackContext c)
    {
        if (c.canceled)
        {
            print("PAUSE INPUIT");
            UIManager.Instance.TogglePause();
        }
    }


    void HandlePause(bool paused)
    {
        StartCoroutine(Switch(paused));

        IEnumerator Switch(bool paused)
        {
            
            yield return null; //wait for one frame bec switching on same frame causes errors
            if (paused)
            {
                inputManager.SwitchCurrentActionMap("UI");
                print("Current Action Map is " + inputManager.currentActionMap.name);
            }
            else
            {
                inputManager.SwitchCurrentActionMap("Player");
                print("Current Action Map is " + inputManager.currentActionMap.name);

            }
        }
    }


    void Update()
    {
        FlipSprite();
        mouseDir = GetDirToMouseOrJoystick();
        if (mouseDir.sqrMagnitude < 0.001f) mouseDir = Vector2.right;
        //movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    UIManager.Instance.TogglePause();
        //}
        //if(Input.GetKeyDown(KeyCode.M)) 
        //{
        //    UIManager.Instance.ToggleMinimap();
        //}

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

    bool canMove;
    public void Move()
    {
        if (!canMove) return;
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

    private void OnDisable()
    {
        if(GameSceneManager.Instance == null) { Debug.LogWarning("GAME MANAGAER SINGLETON WAS NULL COULD BE DUE TO APPLICATION QUIT");return; }
        UIManager.Instance.OnGamePauseToggle -= HandlePause;
    }

    [Button("Add Souls")]
    public void AddPoints(int n)
    {
        if (n == 0) return;
        souls += n;
        
        EOnPointsChanged?.Invoke(souls);
    }
    
    public Vector2 GetDirToMouseOrJoystick()
    {
        if (inputManager.currentControlScheme == "Keyboard")
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorld.z = transform.position.z;

            return (mouseWorld - transform.position).normalized;
        }

        // Gamepad
        InputAction aimAction = inputManager.actions["Aim"];
        return aimAction.ReadValue<Vector2>().normalized;

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, MouseAndJoystickDir);    
    }


}
