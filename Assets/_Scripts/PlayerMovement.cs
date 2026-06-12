using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Camera cam;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 movementVector;
    Vector3 towardsMouse;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        Move();

    }

    public void Move()
    {
        // get the maxSpeed
        Vector2 targetSpeed = movementVector * moveSpeed;

        // get the difference b/w current and max speed
        Vector2 speedDif = targetSpeed - rb.linearVelocity;
        rb.AddForce(speedDif, ForceMode2D.Impulse); // impulse feels more snappy but FORCE feels more floaty

    }

    private void OnEnable()
    {
        RoomAssembler.EOnAssemblyFinished += OnGenFinish;
    }
    private void OnDisable()
    {
        RoomAssembler.EOnAssemblyFinished -= OnGenFinish;
    }
    void OnGenFinish(IReadOnlyList<Room> rooms)
    {
        transform.position = rooms[0].GlobalPosition;
    }

}
