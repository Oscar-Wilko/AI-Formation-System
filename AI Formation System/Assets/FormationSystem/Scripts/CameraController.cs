using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    // Constraint Values
    [Header("Constraints")]
    public float min_zoom;
    public float max_zoom;
    public float min_rotation;
    public float max_rotation;
    public float y_offset;

    // Speed Values
    [Header("Speeds")]
    public float move_speed;
    public float move_speed_keybinds;
    public Vector2 rot_speed;
    public float zoom_speed;
    public float zoom_speed_controller;
    public float zoom_factor;

    // Tracked Values
    private Vector3 previous_mouse;
    private bool in_move_mode = false;
    private bool in_rot_mode = false;

    private void Awake()
    {
        SnapToGround();
        RotateInput(Vector3.zero);
    }

    void LateUpdate()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // Mouse is not over UI
            ZoomInput(Input.mouseScrollDelta.y);
            if (Input.GetMouseButtonDown(0)) in_move_mode = true;
            if (Input.GetMouseButtonDown(1)) in_rot_mode = true;
        }
        if (Input.GetMouseButtonUp(0)) in_move_mode = false;
        if (Input.GetMouseButtonUp(1)) in_rot_mode = false;
        MoveVertical(Input.GetAxis("VerticalKey"));
        MoveHorizontal(Input.GetAxis("HorizontalKey"));
        if (in_move_mode && Input.GetMouseButton(0)) MoveInput(Input.mousePosition - previous_mouse);
        if (in_rot_mode && Input.GetMouseButton(1)) RotateInput(Input.mousePosition - previous_mouse);
        
        previous_mouse = Input.mousePosition;
    }

    /// <summary>
    /// Zoom camera in or out
    /// </summary>
    /// <param name="scroll_delta">Float of how much to zoom by</param>
    private void ZoomInput(float scroll_delta)
    {
        if (scroll_delta == 0) return;
        float cur_zoom = transform.localPosition.z;
        cur_zoom += scroll_delta * zoom_speed;
        cur_zoom = Mathf.Clamp(cur_zoom, min_zoom, max_zoom);
        transform.localPosition = new Vector3(0,0,cur_zoom);
    }

    /// <summary>
    /// Rotate camera around
    /// </summary>
    /// <param name="mouse_delta">Vector3 of mouse input</param>
    private void RotateInput(Vector3 mouse_delta)
    {
        float cur_y_rot = transform.parent.transform.localEulerAngles.y;
        cur_y_rot += mouse_delta.x * rot_speed.x;
        float cur_x_rot = transform.parent.transform.localEulerAngles.x;
        cur_x_rot -= mouse_delta.y * rot_speed.y;
        cur_x_rot = Mathf.Clamp(cur_x_rot, min_rotation, max_rotation);
        transform.parent.transform.localEulerAngles = new Vector3(cur_x_rot,cur_y_rot,0);
    }

    /// <summary>
    /// Move camera around
    /// </summary>
    /// <param name="mouse_delta">Vector3 of mouse input</param>
    private void MoveInput(Vector3 mouse_delta)
    {
        Vector3 forward_vec = transform.forward;
        forward_vec.y = 0;
        forward_vec = forward_vec.normalized;

        Vector3 move_vec = forward_vec * mouse_delta.y + transform.right * mouse_delta.x;
        move_vec *= move_speed * zoom_factor * -transform.localPosition.z;
        transform.parent.transform.localPosition -= move_vec;

        SnapToGround();
    }

    /// <summary>
    /// Horizontal movement of camera
    /// </summary>
    /// <param name="sign">Int of direction to move camera</param>
    private void MoveHorizontal(float scale)
    {
        Vector3 move_vec = transform.right * move_speed_keybinds * zoom_factor * -transform.localPosition.z * Time.unscaledDeltaTime;
        transform.parent.transform.localPosition += move_vec * scale;
        SnapToGround();
    }

    /// <summary>
    /// Vertical movement of camera
    /// </summary>
    /// <param name="sign">Int of direction to move camera</param>
    private void MoveVertical(float scale)
    {
        Vector3 forward_vec = transform.forward;
        forward_vec.y = 0;
        forward_vec = forward_vec.normalized;

        Vector3 move_vec = forward_vec * move_speed_keybinds * zoom_factor * -transform.localPosition.z * Time.unscaledDeltaTime;
        transform.parent.transform.localPosition += move_vec * scale;
        SnapToGround();
    }

    private void SnapToGround()
    {
        Physics.Raycast(transform.parent.transform.localPosition + new Vector3(0, 100), Vector3.down, out RaycastHit hitInfo, 1000, LayerMask.GetMask("Ground"));
        if (hitInfo.point != Vector3.zero)
            transform.parent.transform.localPosition = hitInfo.point;
    }
}
