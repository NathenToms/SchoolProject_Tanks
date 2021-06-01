using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

/// <summary>
/// Controls the turret based on the controller input.
/// TODO -- Aiming, Shooting, First-Person Cockpit, Weapon Switching
/// </summary>
public class TurretController : Controller
{
    [SerializeField]
    private Transform ship;

    [SerializeField]
    private bool drawDebug;

    private Camera cam;

    Vector2 rotation;

    bool aiming, shooting;

    /// <summary>
    /// TODO
    /// Shoots in the direction of the turret.
    /// Aiming locks the turret to a position until it becomes unviewable due to the ship
    /// </summary>
    void Shoot()
    {

    }

    void Awake()
    {
        // Sets camera to bottom half screen
        cam = GetComponent<PlayerInput>().camera;
        cam.rect = new Rect(0, 0, 1.0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // Stick movement
        Vector3 rotate = ControllerInput.RightStick * 5 * 10 * Time.deltaTime;

        if (rotation.y < -90)
            rotate.x = -rotate.x;

        // Applys stick movement to overall rotation while clamping it so it doesn't clip into into the ship
        rotation = new Vector2(rotation.x + rotate.x, Mathf.Clamp(rotation.y + rotate.y, -180, 0));

        Quaternion yaw = Quaternion.Euler(0f, rotation.x, 0f);
        Quaternion pitch = Quaternion.Euler(-rotation.y, 0f, 0f);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, yaw * pitch, 20 * Time.deltaTime);

        Shoot();
    }

    private void OnGUI()
    {
        if (drawDebug)
        {
            GUI.Label(new Rect(5, Screen.height / 2.0f + 5, 200, 200), "Left Stick: " + ControllerInput.LeftStick +
                                                "\nRight Stick: " + ControllerInput.RightStick +
                                                "\nLeft Trigger: " + ControllerInput.LeftTrigger +
                                                "\nRight Trigger: " + ControllerInput.RightTrigger);

            GUI.Label(new Rect(Screen.width - 205, Screen.height / 2.0f + 5, 200, 200), "Rotation: " + rotation);
        }
    }

    /// <summary>
    /// Aiming button
    /// </summary>
    /// <param name="input"></param>
    public override void OnLeftTrigger(InputValue input)
    {
        base.OnLeftTrigger(input);

        aiming = input.isPressed;
    }

    /// <summary>
    /// Shooting button
    /// </summary>
    /// <param name="input"></param>
    public override void OnRightTrigger(InputValue input)
    {
        base.OnRightTrigger(input);

        shooting = input.isPressed;
    }
}
