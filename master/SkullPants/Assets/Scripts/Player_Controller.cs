using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public float mMoveSpeed;

    private Rigidbody2D mRigidBody;
    private bool mReadInput = true;
    private int mDirection = 1;
    private bool mGrounded = false;

    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (mReadInput)
            ReadInput();
    }

    private void ReadInput()
    {

    }

    private void HorizontalMove()
    {
        float fDirection = Input.GetAxis("Horizontal");
    }
    private void SpriteFlip()
    {

    }

    private void Jump()
    {
        if(Input.GetButtonDown("Jump") && mGrounded)
        {

        }
    }

    public void StopInput(bool pStop)
    {

    }

}
