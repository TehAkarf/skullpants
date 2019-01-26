using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask mGroundLayer;
    public float mGroundDistanceRay = 0.1f;

    Rigidbody2D mRigidBody;
    CapsuleCollider2D mCapsule;
    Vector2 mPreviousPosition;
    Vector2 mCurrentPosition;
    Vector2 mNextMovement;
    ContactFilter2D mContactFilter;
    RaycastHit2D[] mHitBuffer = new RaycastHit2D[5];
    RaycastHit2D[] mFoundHits = new RaycastHit2D[3];
    Collider2D[] mGroundColliders = new Collider2D[3];
    Vector2[] mRaycastPositions = new Vector2[3];

    public bool mGrounded { get; protected set; }
    public bool mCeilinged { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public Rigidbody2D Rigidbody2D { get { return mRigidBody; } }
    public Collider2D[] GroundColliders { get { return mGroundColliders; } }
    public ContactFilter2D ContactFilter { get { return mContactFilter; } }

    void Awake()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
        mCapsule = GetComponent<CapsuleCollider2D>();

        mCurrentPosition = mRigidBody.position;
        mPreviousPosition = mRigidBody.position;

        mContactFilter.layerMask = mGroundLayer;
        mContactFilter.useLayerMask = true;
        mContactFilter.useTriggers = false;

        Physics2D.queriesStartInColliders = false;
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        mPreviousPosition = mRigidBody.position;
        mCurrentPosition = mPreviousPosition + mNextMovement;
        Velocity = (mCurrentPosition - mPreviousPosition) / Time.deltaTime;

        mRigidBody.MovePosition(mCurrentPosition);
        mNextMovement = Vector2.zero;

        CheckCapsuleEndCollisions();
        CheckCapsuleEndCollisions(false);
    }

    public void Move(Vector2 pMovement)
    {
        mNextMovement += pMovement;
    }

    public void CheckCapsuleEndCollisions(bool bottom = true)
    {
        Vector2 fRaycastDirection;
        Vector2 fRaycastStart;
        float fRaycastDistance;

        if (mCapsule == null)
        {
            fRaycastStart = mRigidBody.position + Vector2.up;
            fRaycastDistance = 1f + mGroundDistanceRay;

            if (bottom)
            {
                fRaycastDirection = Vector2.down;

                mRaycastPositions[0] = fRaycastStart + Vector2.left * 0.4f;
                mRaycastPositions[1] = fRaycastStart;
                mRaycastPositions[2] = fRaycastStart + Vector2.right * 0.4f;
            }
            else
            {
                fRaycastDirection = Vector2.up;

                mRaycastPositions[0] = fRaycastStart + Vector2.left * 0.4f;
                mRaycastPositions[1] = fRaycastStart;
                mRaycastPositions[2] = fRaycastStart + Vector2.right * 0.4f;
            }
        }
        else
        {
            fRaycastStart = mRigidBody.position + mCapsule.offset;
            fRaycastDistance = mCapsule.size.x * 0.5f + mGroundDistanceRay * 2f;

            if (bottom)
            {
                fRaycastDirection = Vector2.down;
                Vector2 raycastStartBottomCentre = fRaycastStart + Vector2.down * (mCapsule.size.y * 0.5f - mCapsule.size.x * 0.5f);

                mRaycastPositions[0] = raycastStartBottomCentre + Vector2.left * mCapsule.size.x * 0.5f;
                mRaycastPositions[1] = raycastStartBottomCentre;
                mRaycastPositions[2] = raycastStartBottomCentre + Vector2.right * mCapsule.size.x * 0.5f;
            }
            else
            {
                fRaycastDirection = Vector2.up;
                Vector2 raycastStartTopCentre = fRaycastStart + Vector2.up * (mCapsule.size.y * 0.5f - mCapsule.size.x * 0.5f);

                mRaycastPositions[0] = raycastStartTopCentre + Vector2.left * mCapsule.size.x * 0.5f;
                mRaycastPositions[1] = raycastStartTopCentre;
                mRaycastPositions[2] = raycastStartTopCentre + Vector2.right * mCapsule.size.x * 0.5f;
            }
        }

        for (int i = 0; i < mRaycastPositions.Length; i++)
        {
            int count = Physics2D.Raycast(mRaycastPositions[i], fRaycastDirection, mContactFilter, mHitBuffer, fRaycastDistance);

            if (bottom)
            {
                mFoundHits[i] = count > 0 ? mHitBuffer[0] : new RaycastHit2D();
                mGroundColliders[i] = mFoundHits[i].collider;
            }
            else
            {
                mCeilinged = false;

                /*for (int j = 0; j < mHitBuffer.Length; j++)
                {
                    if (mHitBuffer[j].collider != null)
                    {
                        if (!PhysicsHelper.ColliderHasPlatformEffector(mHitBuffer[j].collider))
                        {
                            mCeilinged = true;
                        }
                    }
                }*/
            }
        }

        if (bottom)
        {
            Vector2 groundNormal = Vector2.zero;
            int hitCount = 0;

            for (int i = 0; i < mFoundHits.Length; i++)
            {
                if (mFoundHits[i].collider != null)
                {
                    groundNormal += mFoundHits[i].normal;
                    hitCount++;
                }
            }

            if (hitCount > 0)
            {
                groundNormal.Normalize();
            }

            Vector2 relativeVelocity = Velocity;
            /*for (int i = 0; i < mGroundColliders.Length; i++)
            {
                if (mGroundColliders[i] == null)
                    continue;

                MovingPlatform movingPlatform;

                if (PhysicsHelper.TryGetMovingPlatform(m_GroundColliders[i], out movingPlatform))
                {
                    relativeVelocity -= movingPlatform.Velocity / Time.deltaTime;
                    break;
                }
            }*/

            if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
            {
                mGrounded = false;
            }
            else
            {
                mGrounded = relativeVelocity.y <= 0f;

                if (mCapsule != null)
                {
                    if (mGroundColliders[1] != null)
                    {
                        float capsuleBottomHeight = mRigidBody.position.y + mCapsule.offset.y - mCapsule.size.y * 0.5f;
                        float middleHitHeight = mFoundHits[1].point.y;
                        mGrounded &= middleHitHeight < capsuleBottomHeight + mGroundDistanceRay;
                    }
                }
            }
        }

        for (int i = 0; i < mHitBuffer.Length; i++)
        {
            mHitBuffer[i] = new RaycastHit2D();
        }
    }
}
