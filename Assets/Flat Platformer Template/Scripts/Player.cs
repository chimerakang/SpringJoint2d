using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
    public float WalkSpeed;
    public float JumpForce = 1.0f;
    public AnimationClip _walk, _jump;
    public Animation _Legs;
    public Transform _Blade, _GroundCast;
    public Camera cam;
    public bool mirror;


    private bool _canJump, _canWalk;
    private bool _isWalk, _isJump;
    private float rot, _startScale;
    private Rigidbody2D rig;
    private Vector2 _inputAxis;
    private RaycastHit2D _hit;

    public SpringJoint2D LFoot;
    public SpringJoint2D RFoot;

    float rotationAngle = 0f;
    float jumpTimer = 0f;


    void Start ()
    {
        rig = gameObject.GetComponent<Rigidbody2D>();
        _startScale = transform.localScale.x;
	}

    void Update()
    {
        if (_hit = Physics2D.Linecast(new Vector2(_GroundCast.position.x, _GroundCast.position.y + 0.2f), _GroundCast.position))
        {
            if (!_hit.transform.CompareTag("Player"))
            {
                _canJump = true;
                _canWalk = true;
            }
        }
        else _canJump = false;

        _inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (_inputAxis.y > 0 && _canJump)
        {
            _canWalk = false;
            _isJump = true;
        }

        rotationAngle = (rotationAngle < 0f) ? rotationAngle + 360f : (rotationAngle > 360f ? rotationAngle - 360f : rotationAngle);

        //Control Jumping
        Vector2 jump = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpTimer = Time.time;
        }
        if (_isJump && Input.GetKey(KeyCode.W) && jumpTimer + 0.1f > Time.time)
        {
            jump = Vector2.up * JumpForce;
        }

        //Control Walking
        if (Input.GetKey(KeyCode.A))
        {
            rotationAngle += 15f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotationAngle -= 15f;
        }
        else
        {
            rotationAngle = (rotationAngle < 90f || rotationAngle > 270f) ? 0f : 180f;
        }

        //Take into account all of the above for foot placement (relative to the body)
        Vector2 anchor = Quaternion.Euler(0f, 0f, rotationAngle) * Vector2.left * 0.25f;
        LFoot.anchor = new Vector2(1f * anchor.x, 0.4f * anchor.y) + jump;
        anchor = -anchor;
        RFoot.anchor = new Vector2(1f * anchor.x, 0.4f * anchor.y) + jump;
    }

    void FixedUpdate()
    {
        Vector3 dir = cam.ScreenToWorldPoint(Input.mousePosition) - _Blade.transform.position;
        dir.Normalize();

        if (cam.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x + 0.2f)
            mirror = false;
        if (cam.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x - 0.2f)
            mirror = true;

        if (!mirror)
        {
            rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.localScale = new Vector3(_startScale, _startScale, 1);
            _Blade.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }
        if (mirror)
        {
            rot = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
            transform.localScale = new Vector3(-_startScale, _startScale, 1);
            _Blade.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }

        if (_inputAxis.x != 0)
        {
            rig.velocity = new Vector2(_inputAxis.x * WalkSpeed * Time.deltaTime, rig.velocity.y);

        }
        else
        {
            rig.velocity = new Vector2(0, rig.velocity.y);
        }

        /*


            if (_isJump)
            {
                rig.AddForce(new Vector2(0, JumpForce));
                _Legs.clip = _jump;
                _Legs.Play();
                _canJump = false;
                _isJump = false;
            }
            */
    }

    public bool IsMirror()
    {
        return mirror;
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, _GroundCast.position);
    }
    */
}
