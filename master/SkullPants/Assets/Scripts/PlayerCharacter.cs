using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCharacter : MonoBehaviour
{
    static protected PlayerCharacter s_PlayerInstance;
    static public PlayerCharacter PlayerInstance
    {
        get { return s_PlayerInstance; }

    }

    public SpriteRenderer spriteRenderer;
    public Transform cameraFollowTarget;

    public float maxSpeed = 10f;
    public float groundAcceleration = 100f;
    public float groundDeceleration = 100f;
    [Range(0f, 1f)] public float pushingSpeedProportion;

    [Range(0f, 1f)] public float airborneAccelProportion;
    [Range(0f, 1f)] public float airborneDecelProportion;
    public float gravity = 50f;
    public float jumpSpeed = 20f;
    public float jumpAbortSpeedReduction = 100f;
    public bool dashWhileAirborne = false;

    //public RandomAudioPlayer footstepAudioPlayer;
    //public RandomAudioPlayer landingAudioPlayer;

    public float cameraHorizontalFacingOffset;
    public float cameraHorizontalSpeedOffset;
    public float cameraVerticalInputOffset;
    public float maxHorizontalDeltaDampTime;
    public float maxVerticalDeltaDampTime;
    public float verticalCameraOffsetDelay;

    public bool spriteOriginallyFacesLeft;

    protected PlayerController m_CharacterController2D;
    protected Animator m_Animator;
    protected CapsuleCollider2D m_Capsule;
    protected Transform m_Transform;
    protected Vector2 m_MoveVector;
    //protected List<Pushable> m_CurrentPushables = new List<Pushable>(4);
    //protected Pushable m_CurrentPushable;
    protected TileBase m_CurrentSurface;
    protected float m_CamFollowHorizontalSpeed;
    protected float m_CamFollowVerticalSpeed;
    protected float m_VerticalCameraOffsetTimer;

    //protected Checkpoint m_LastCheckpoint = null;
    protected Vector2 m_StartingPosition = Vector2.zero;
    protected bool m_StartingFacingLeft = false;

    protected bool m_InPause = false;

    protected readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");
    protected readonly int m_HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");
    protected readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");

    protected ContactPoint2D[] m_ContactsBuffer = new ContactPoint2D[16];


    void Awake()
    {
        s_PlayerInstance = this;

        m_CharacterController2D = GetComponent<PlayerController>();
        m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider2D>();
        m_Transform = transform;
    }

    void Start()
    {
        if (!Mathf.Approximately(maxHorizontalDeltaDampTime, 0f))
        {
            float maxHorizontalDelta = maxSpeed * cameraHorizontalSpeedOffset + cameraHorizontalFacingOffset;
            m_CamFollowHorizontalSpeed = maxHorizontalDelta / maxHorizontalDeltaDampTime;
        }

        if (!Mathf.Approximately(maxVerticalDeltaDampTime, 0f))
        {
            float maxVerticalDelta = cameraVerticalInputOffset;
            m_CamFollowVerticalSpeed = maxVerticalDelta / maxVerticalDeltaDampTime;
        }

        m_StartingPosition = transform.position;
        m_StartingFacingLeft = GetFacing() < 0.0f;
    }

    /* void OnTriggerEnter2D(Collider2D other)
    {
        Pushable pushable = other.GetComponent<Pushable>();
        if (pushable != null)
        {
            m_CurrentPushables.Add(pushable);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Pushable pushable = other.GetComponent<Pushable>();
        if (pushable != null)
        {
            if (m_CurrentPushables.Contains(pushable))
                m_CurrentPushables.Remove(pushable);
        }
    }*/

    void Update()
    {
        if (PlayerInput.Instance.Pause.Down)
        {
            if (!m_InPause)
            {
                PlayerInput.Instance.ReleaseControl(false);
                PlayerInput.Instance.Pause.GainControl();
                m_InPause = true;
                Time.timeScale = 0;
            }
            else
            {
                Unpause();
            }
        }
    }

    void InputUpdate()
    {
        UpdateFacing();
        if (m_CharacterController2D.mGrounded)
        {
            GroundedHorizontalMovement(true);
            GroundedVerticalMovement();
            CheckForJumpInput();
        }
        else
        {
            AirborneHorizontalMovement();
            AirborneVerticalMovement();
            UpdateJump();
        }
        
        
    }

    void FixedUpdate()
    {
        InputUpdate();
        m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
        m_Animator.SetFloat(m_HashHorizontalSpeedPara, m_MoveVector.x);
        m_Animator.SetFloat(m_HashVerticalSpeedPara, m_MoveVector.y);
        UpdateCameraFollowTargetPosition();
    }

    public void Unpause()
    {
        //if the timescale is already > 0, we 
        if (Time.timeScale > 0)
            return;

        StartCoroutine(UnpauseCoroutine());
    }

    protected IEnumerator UnpauseCoroutine()
    {
        Time.timeScale = 1;
        //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("UIMenus");
        PlayerInput.Instance.GainControl();
        //we have to wait for a fixed update so the pause button state change, otherwise we can get in case were the update
        //of this script happen BEFORE the input is updated, leading to setting the game in pause once again
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        m_InPause = false;
    }

    protected void UpdateCameraFollowTargetPosition()
    {
        float newLocalPosX;
        float newLocalPosY = 0f;

        float desiredLocalPosX = (spriteOriginallyFacesLeft ^ spriteRenderer.flipX ? -1f : 1f) * cameraHorizontalFacingOffset;
        desiredLocalPosX += m_MoveVector.x * cameraHorizontalSpeedOffset;
        if (Mathf.Approximately(m_CamFollowHorizontalSpeed, 0f))
            newLocalPosX = desiredLocalPosX;
        else
            newLocalPosX = Mathf.Lerp(cameraFollowTarget.localPosition.x, desiredLocalPosX, m_CamFollowHorizontalSpeed * Time.deltaTime);

        bool moveVertically = false;
        if (!Mathf.Approximately(PlayerInput.Instance.Vertical.Value, 0f))
        {
            m_VerticalCameraOffsetTimer += Time.deltaTime;

            if (m_VerticalCameraOffsetTimer >= verticalCameraOffsetDelay)
                moveVertically = true;
        }
        else
        {
            moveVertically = true;
            m_VerticalCameraOffsetTimer = 0f;
        }

        if (moveVertically)
        {
            float desiredLocalPosY = PlayerInput.Instance.Vertical.Value * cameraVerticalInputOffset;
            if (Mathf.Approximately(m_CamFollowVerticalSpeed, 0f))
                newLocalPosY = desiredLocalPosY;
            else
                newLocalPosY = Mathf.MoveTowards(cameraFollowTarget.localPosition.y, desiredLocalPosY, m_CamFollowVerticalSpeed * Time.deltaTime);
        }

        cameraFollowTarget.localPosition = new Vector2(newLocalPosX, newLocalPosY);
    }

    public void UpdateFacing()
    {
        bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
        bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

        if (faceLeft)
        {
            spriteRenderer.flipX = !spriteOriginallyFacesLeft;
        }
        else if (faceRight)
        {
            spriteRenderer.flipX = spriteOriginallyFacesLeft;
        }
    }

    public void UpdateFacing(bool faceLeft)
    {
        if (faceLeft)
        {
            spriteRenderer.flipX = !spriteOriginallyFacesLeft;
        }
        else
        {
            spriteRenderer.flipX = spriteOriginallyFacesLeft;
        }
    }

    public float GetFacing()
    {
        return spriteRenderer.flipX != spriteOriginallyFacesLeft ? -1f : 1f;
    }

    public void GroundedHorizontalMovement(bool useInput, float speedScale = 1f)
    {
        float desiredSpeed = useInput ? PlayerInput.Instance.Horizontal.Value * maxSpeed * speedScale : 0f;
        float acceleration = useInput && PlayerInput.Instance.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);
    }
    public void GroundedVerticalMovement()
    {
        m_MoveVector.y -= gravity * Time.deltaTime;

        if (m_MoveVector.y < -gravity * Time.deltaTime)
        {
            m_MoveVector.y = -gravity * Time.deltaTime;
        }
    }

    public void UpdateJump()
    {
        if (!PlayerInput.Instance.Jump.Held && m_MoveVector.y > 0.0f)
        {
            m_MoveVector.y -= jumpAbortSpeedReduction * Time.deltaTime;
        }
    }

    public void AirborneHorizontalMovement()
    {
        float desiredSpeed = PlayerInput.Instance.Horizontal.Value * maxSpeed;

        float acceleration;

        if (PlayerInput.Instance.Horizontal.ReceivingInput)
            acceleration = groundAcceleration * airborneAccelProportion;
        else
            acceleration = groundDeceleration * airborneDecelProportion;

        m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);
    }

    public void AirborneVerticalMovement()
    {
        if (Mathf.Approximately(m_MoveVector.y, 0f) || m_CharacterController2D.mCeilinged && m_MoveVector.y > 0f)
        {
            m_MoveVector.y = 0f;
        }
        m_MoveVector.y -= gravity * Time.deltaTime;
    }

    public void CheckForJumpInput()
    {
        if(PlayerInput.Instance.Jump.Down)
            m_MoveVector.y = jumpSpeed;
    }

}
