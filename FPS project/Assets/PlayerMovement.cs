using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Player Config")]
    [SerializeField] private float player_speed;
    [SerializeField] private float jump_power;
    [SerializeField] private float gravity;
    [SerializeField] private float friction;
    [Space]
    [Header("Camera Config")]
    public Camera playercam;
    public float camera_sensitivity;
    public float looklimit;
    [Space]
    [Header("External Config")]
    [SerializeField] private Vector3 ForceToApply;
    [SerializeField] private float ForceDamping;
    [SerializeField] private LayerMask grounded_mask;

    private CharacterController CharacterController;
    private Vector3 moveDirection;
    private Vector2 rotation, speed;

    [HideInInspector] public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotation.y = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded())
        { 
            #region BaseInput
            speed.x = canMove ? player_speed * Input.GetAxis("Vertical") : 0;
            speed.y = canMove ? player_speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = transform.forward * speed.x + transform.right * speed.y;
            if(moveDirection.magnitude > player_speed)
            {
                moveDirection = moveDirection.normalized * player_speed;
            }
            #endregion
            if(Input.GetButtonDown("Jump") && canMove)
            {
                moveDirection.y = jump_power;
            }
        }
        if(Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z) < friction)
        {
            moveDirection = new Vector3(0, moveDirection.y, 0);
        }
        moveDirection += ForceToApply;
        ForceToApply = Vector3.Lerp(ForceToApply, Vector3.zero, ForceDamping * Time.deltaTime);
        //Apply gravity (gravity is acceleration [M][T][T])
        moveDirection.y -= gravity * Time.deltaTime;

        CharacterController.Move(moveDirection *Time.deltaTime);

        #region Player and Camera Rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * camera_sensitivity;
            rotation.x += -Input.GetAxis("Mouse Y") * camera_sensitivity;
            rotation.x = Mathf.Clamp(rotation.x, -looklimit, looklimit);
            playercam.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
        #endregion
    }
    public void ApplyForce(float magnitude)
    {
        ForceToApply = transform.forward * magnitude;
    }
    public bool IsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, 100, grounded_mask);
        if (hit.distance == 0 || hit.distance > 1.08f)
        {
            return false;
        }
        else return true;
    }
}
