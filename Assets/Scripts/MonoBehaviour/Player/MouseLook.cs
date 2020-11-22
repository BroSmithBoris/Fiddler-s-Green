using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour 
{
    public PlayerControlsInputSystem PlayerControls;
    [SerializeField] [Range(0.0f, 300.0f)] private float SensitiveX = 5.0f, SensitiveY = 5.0f;
    [SerializeField] [Range(0.0f, 360.0f)] public float ViewingAngleX = 360, ViewingAngleY = 60;
    [SerializeField] GameObject MainCamera;
    private Quaternion OriginalRot;
    private float RotX, RotY;

    private void Awake() => PlayerControls = new PlayerControlsInputSystem();

    private void OnEnable() => PlayerControls.Player.Look.Enable();

    private void OnDisable() => PlayerControls.Player.Look.Disable();
	
    void Start ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
		OriginalRot = transform.localRotation;
	}

    void Update()
    {
        if (PlayerControls.Player.Look.phase == InputActionPhase.Started)
        {
            Vector2 MovementVector = PlayerControls.Player.Look.ReadValue<Vector2>() * Time.deltaTime;

            RotX = Mathf.Clamp((RotX + MovementVector.x * SensitiveX) % 360, -ViewingAngleX, ViewingAngleX);
            RotY = Mathf.Clamp((RotY + MovementVector.y * SensitiveY) % 360, -ViewingAngleY, ViewingAngleY);

            MainCamera.transform.localRotation = OriginalRot * Quaternion.AngleAxis(RotY, Vector3.left);
            transform.localRotation = OriginalRot * Quaternion.AngleAxis(RotX, Vector3.up);
        }
    }
}
