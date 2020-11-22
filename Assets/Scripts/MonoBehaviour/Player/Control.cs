using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(CharacterController))]
public class Control : MonoBehaviour
{
    public enum AreaZone
    {
        AreaOne,
        AreaTwo,
        AreaThree
    }

    public PlayerControlsInputSystem PlayerControls;
    public ObstacleCheck Obstacle;
    public DavyJones Davy;
    public bool IsAttacking, TimeToRespawnDavyFlag, StepSoundActive, FireSoundActive, SaberSoundActive;
    public float Health = 100.0f;
    public AreaZone PlayerArea = AreaZone.AreaOne;
    [SerializeField] private PostProcessProfile DefaultPostProcessingProfile;
    [SerializeField] private GameObject MainCamera, UI, PostProcessing, Saber, Pistol, Bullet, Fireball;
    [SerializeField] private AudioClip[] HitSounds;
    [SerializeField] private AudioSource PlayerAudioSource;
    [Header("Mouse look Settings")]
    [SerializeField, Range(0.0f, 300.0f)] private float SensitiveX = 5.0f;
    [SerializeField, Range(0.0f, 300.0f)] private float SensitiveY = 5.0f;
    [SerializeField, Range(0.0f, 360.0f)] private float ViewingAngleX = 360, ViewingAngleY = 90;
    [Header("Move Settings")]
    [SerializeField, Range(0.0f, 10.0f)] private float Speed = 5.0f;
    [SerializeField, Range(0.0f, 50.0f)] private float AcceleratorSpeed = 5.0f;
    [Header("Jump Settings")]
    [SerializeField, Range(0.0f, 10.0f)] private float JumpStrength = 4.0f;
    [Header("Crouch Settings")]
    [SerializeField, Range(0.0f, 2.0f)] private float HeightCrouch = 1.0f;
    [SerializeField, Range(0.0f, 2.0f)] private float HeightStay = 1.5f;
    [SerializeField, Range(0.0f, 10.0f)] private float CrouchMoveSpeed = 3.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float ActionSpeed = 2.5f;
    [Header("Swim Settings")]
    [SerializeField, Range(0.0f, 100.0f)] private float Air = 50.0f;
    [SerializeField ,Range(0.0f, 10.0f)] private float SwimingSpeed = 2.0f;
    [SerializeField ,Range(0.0f, 10.0f)] private float SwimingAcceleratorSpeed = 3.0f;
    [Header("Attack Settings")]
    [SerializeField, Range(-180.0f, 180.0f)] private float AttackRotSaber = 50.0f;
    [SerializeField ,Range(0.0f, 500.0f)] private float SaberAttackSpeed = 150.0f;
    [SerializeField ,Range(0.0f, 500.0f)] private float SaberBackSpeed = 300.0f;
    [Header("Respawn Davy Settings")]
    [SerializeField ,Range(0.0f, 30.0f)]private float TimeToRespawnDavy = 5.0f;
    private bool IsCanShoot = true, IsStay = true, CanRespawnDavy = true;
    private bool IsSabreSound, IsJump, IsAltAttacking, IsReloading, IsAltHit, IsHit, IsNewPostProcessingProfile, IsCrouching, IsSwimming, IsInWater, IsDiving;
    private float RotX, RotY, CurrentSpeedY, CurrentSpeedXZ, SkinWidth, CurrentAir, OpenRotSaber;
    private Quaternion OriginalRot;
    private Water WaterType;
    private Vector3 DefaultRotSaber;
    private Vector3 PistolDefaultRot;
    private CharacterController Controller;

    
    private void OnEnable() => PlayerControls.Player.Enable();

    private void OnDisable() => PlayerControls.Player.Disable();

    private void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if (((Controller.collisionFlags & (CollisionFlags.Above)) != 0) && CurrentSpeedY > .0f)
        {
            CurrentSpeedY = 0;
            IsJump = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "AreaOne":
                PlayerArea = AreaZone.AreaOne;
                break;
            case "AreaTwo":
                PlayerArea = AreaZone.AreaTwo;
                break;
            case "AreaThree":
                PlayerArea = AreaZone.AreaThree;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        switch (other.tag)
        {
            case "AreaOne":
                TimeToRespawnDavy = 5.0f;
                break;
            case "AreaTwo":
                TimeToRespawnDavy = 5.0f;
                break;
            case "AreaThree":
                TimeToRespawnDavy = 5.0f;
                break;
            
            case "Respawn":
                SceneManager.LoadScene(1);
                break;

            case "Water":
                IsInWater = true;
                CurrentSpeedXZ /= 2;
                WaterType = other.GetComponent<Water>();
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.tag == "Water")
        {
            WaterType = null;
            IsInWater = false;
        }
    }

    private void OnHandleJumpStarted(InputAction.CallbackContext context) => IsJump = true;

    private void OnHandleJumpCanceled(InputAction.CallbackContext context) => IsJump = false;

    private void OnHandleCrouchStarted(InputAction.CallbackContext context)
    {
        IsCrouching = false;
        //CurrentSpeedXZ /= 2;
    }

    private void OnHandleMenu(InputAction.CallbackContext context)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerControls.Player.Disable();
        UI.SetActive(true);
    }

    private void OnSaberAttack(InputAction.CallbackContext context)
    {
        IsAttacking = true;
        IsSabreSound = !IsHit;
    }

    private void OnPistolAttack(InputAction.CallbackContext context) => IsAltAttacking = true;

    private void OnPistolReload(InputAction.CallbackContext context) => IsReloading = true;

    private void Awake()
    {
        PlayerControls = new PlayerControlsInputSystem();
        PlayerControls.Player.Crouch.started += OnHandleCrouchStarted;
        PlayerControls.Player.Jump.started += OnHandleJumpStarted;
        PlayerControls.Player.Menu.started += OnHandleMenu;
        PlayerControls.Player.Attack.started += OnSaberAttack;
        PlayerControls.Player.Jump.canceled += OnHandleJumpCanceled;
        PlayerControls.Player.AlternativeAttack.started += OnPistolAttack;
        PlayerControls.Player.Reload.started += OnPistolReload;
    }

    private void Start()
    {
        Controller = gameObject.GetComponent<CharacterController>();
        SkinWidth = Controller.skinWidth * 2;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
		OriginalRot = transform.localRotation;

        DefaultRotSaber = Saber.transform.localEulerAngles;
        OpenRotSaber = DefaultRotSaber.x + AttackRotSaber;

        PistolDefaultRot = Pistol.transform.localEulerAngles;
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = Vector3.zero;
        CharacterPhysics();
        CharacterLook();
        if(!IsSwimming)
        {
            CharacterCrouch();
            moveVector = CharacterMove();
        }
        else
            moveVector = CharacterSwim();
        Controller.Move(moveVector * Time.deltaTime);

        SaberAttack();
        PistolAttack();
        PistolReload();
        if (Health < 0)
            SceneManager.LoadScene(1);  
        
        if ((int)PlayerArea != (int)Davy.DavyArea && TimeToRespawnDavy > -0.1f)
        {
            TimeToRespawnDavy -= Time.deltaTime;
            Debug.Log(TimeToRespawnDavy);

            if (TimeToRespawnDavy >= 2.0f && TimeToRespawnDavy <= 3.0f)
            {
               CanRespawnDavy = true;
            }

            if (TimeToRespawnDavy <= 0.0f && CanRespawnDavy)
            {
                TimeToRespawnDavyFlag = true;
                CanRespawnDavy = false;
            }
            else
            {
                TimeToRespawnDavyFlag = false;
            }
        }
    }

    public void IsPlayerHit(float damage)
    {
        Health -= damage;
        var rnd = UnityEngine.Random.Range(0, HitSounds.Length - 1);
        PlayerAudioSource.PlayOneShot(HitSounds[rnd]);
    }

    private void SaberAttack()
    {
        if (IsAttacking)
        {
            if (!IsHit)
            {
                if (IsSabreSound)
                {
                    SaberSoundActive = true;
                    IsSabreSound = false;
                }
                else SaberSoundActive = false;
                Saber.transform.localEulerAngles = new Vector3(Mathf.MoveTowardsAngle(Saber.transform.localEulerAngles.x, OpenRotSaber, Time.deltaTime * SaberAttackSpeed), DefaultRotSaber.y, DefaultRotSaber.z);
                IsHit = (OpenRotSaber - Saber.transform.localEulerAngles.x) < 0.1f;
            }
            else
            {
                Saber.transform.localEulerAngles = new Vector3(Mathf.MoveTowardsAngle(Saber.transform.localEulerAngles.x, DefaultRotSaber.x, Time.deltaTime * SaberBackSpeed), DefaultRotSaber.y, DefaultRotSaber.z);
            }
            if (Saber.transform.localEulerAngles.x == DefaultRotSaber.x) IsAttacking = IsHit = false;
        }
        else
            SaberSoundActive = false;
    }

    private void PistolAttack()
    {
        if (IsAltAttacking && IsCanShoot)
        {
            FireSoundActive = true;

            var muzzle = Pistol.transform.Find("Muzzle");
            var newBullet = Instantiate(Bullet);
            var newFireball = Instantiate(Fireball);
            newFireball.transform.position = muzzle.transform.position;
            newFireball.transform.rotation = muzzle.transform.rotation;
            newBullet.transform.position = muzzle.transform.position;
            newFireball.GetComponent<Rigidbody>().AddForce(muzzle.transform.up * -10);
            newBullet.GetComponent<Rigidbody>().AddForce(muzzle.transform.up * -2000);
            IsCanShoot = IsAltAttacking = false;
        }
        else
            FireSoundActive = false;
    }

    private void PistolReload()
    {
        if ((IsReloading || IsAltAttacking) && IsCanShoot == false)
        {
            //Звук перезарядки
            Pistol.transform.Rotate(Vector3.right, 600f * Time.deltaTime);
            if (Pistol.transform.localEulerAngles.x == PistolDefaultRot.x)
            {
                IsAltAttacking = IsReloading = false;
                IsCanShoot = true;
            }
        }
    }

    private void CharacterPhysics()
    {
        IsSwimming = IsInWater && transform.position.y <= WaterType.WaterLevelY;
        if(!IsSwimming)
        {
            CurrentSpeedY += !Controller.isGrounded ? Physics.gravity.y * Time.deltaTime : 0;
        }
        else
            CurrentSpeedY = CurrentSpeedY < 0 ? CurrentSpeedY + WaterType.Viscosity * Time.deltaTime : 0;
    }

    private Vector3 CharacterMove()
    {
        Vector2 movementDirection = PlayerControls.Player.Move.ReadValue<Vector2>();
        Vector3 resultMoveVector = Vector3.zero;
        if ((IsStay && !IsCrouching && CurrentSpeedXZ < Speed) || ((!IsStay || IsCrouching) && CurrentSpeedXZ < CrouchMoveSpeed))
            CurrentSpeedXZ += AcceleratorSpeed * Time.deltaTime;
        if (PlayerControls.Player.Move.phase == InputActionPhase.Started)
        {
            StepSoundActive = true;
            resultMoveVector = transform.forward * movementDirection.y * CurrentSpeedXZ + transform.right * movementDirection.x * CurrentSpeedXZ / 2;
        }
        else
        {
            StepSoundActive = false;
            CurrentSpeedXZ = 0;
        }
        resultMoveVector.y = CurrentSpeedY = (Controller.isGrounded && IsJump && IsStay) ? JumpStrength : CurrentSpeedY;
        resultMoveVector.z += 10f;
        return resultMoveVector;
    }

    private void CharacterCrouch()
    {        
        if(IsCrouching && IsStay)
        {
            SetOffsetControllerAndCamera(-ActionSpeed);
            if (Controller.height + SkinWidth <= HeightCrouch)
                IsStay = IsCrouching = false;
        }
        else if(!IsStay && (IsCrouching || IsJump) && !Obstacle.IsObstacle)
        {
            IsCrouching = true;
            SetOffsetControllerAndCamera(ActionSpeed);
            if (Controller.height + SkinWidth >= HeightStay)
            {
                IsStay = true;
                IsCrouching = false;
            }
        }
        else if(IsCrouching)
            IsCrouching = false;
    }

    private void CharacterLook()
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

    private Vector3 CharacterSwim()
    {
        if(!IsStay)
        {
            IsCrouching = true;
            CharacterCrouch();
        }
        if(MainCamera.transform.position.y <= WaterType.WaterLevelY)
        {
            IsDiving = true;
            if(!IsNewPostProcessingProfile)
            {
                IsNewPostProcessingProfile = true;
                PostProcessing.GetComponent<PostProcessVolume>().profile = WaterType.Profile;
            }
        }
        else
        {
            IsDiving = false;
            if(IsNewPostProcessingProfile)
            {
                IsNewPostProcessingProfile = false;
                PostProcessing.GetComponent<PostProcessVolume>().profile = DefaultPostProcessingProfile;
            }
        }
        
        CurrentAir = IsDiving ? CurrentAir - Time.deltaTime : Air;

        Vector2 movementDirection = PlayerControls.Player.Move.ReadValue<Vector2>();
        Vector3 resultMoveVector = Vector3.zero;

        if (CurrentSpeedXZ < SwimingSpeed) CurrentSpeedXZ += SwimingAcceleratorSpeed * Time.deltaTime;
        if (PlayerControls.Player.Move.phase == InputActionPhase.Started)
        {   
            resultMoveVector = MainCamera.transform.forward * movementDirection.y * CurrentSpeedXZ + 
                MainCamera.transform.right * movementDirection.x * CurrentSpeedXZ / 2;

        }
        else
            CurrentSpeedXZ = 0;
        resultMoveVector.y += CurrentSpeedY;
        return resultMoveVector;
    }

    private void SetOffsetControllerAndCamera(float offsetValue)
    {
        offsetValue *= Time.deltaTime;
        Controller.height += offsetValue;
        Controller.center = new Vector3(0, Controller.center.y + offsetValue / 2, 0);
        var oldCameraPosition = MainCamera.transform.position;
        MainCamera.transform.position = new Vector3(oldCameraPosition.x, oldCameraPosition.y + offsetValue, oldCameraPosition.z);
    }
}
