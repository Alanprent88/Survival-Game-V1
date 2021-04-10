using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    public float speed = 5.0f;

    public float rotationSpeed;
    public float gravitySpeed;

    public Camera cam;

    private float y;


    
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var floatX = Input.GetAxis("Horizontal");
        var floatY = Input.GetAxis("Vertical");

        Moving(floatX, floatY);

    }

    void Moving(float x, float z)
    {
        var v = cam.transform.right * x;
        var h = cam.transform.forward * z;

        var inputDir = (v + h).normalized;
        inputDir.y = 0.0f;

        y = rb.velocity.y;

        var moveAmount = Mathf.Clamp01(Mathf.Abs(x) + Mathf.Abs(z));
        var speedAmount = moveAmount * speed;

        //Rotation
        var targetDir = inputDir;
        if (targetDir == Vector3.zero) targetDir = transform.forward;

        var rotAmount = rotationSpeed * Time.fixedDeltaTime;
        var lookDir = Quaternion.LookRotation(targetDir);
        var targetRot = Quaternion.Slerp(transform.rotation, lookDir, rotAmount);

        transform.rotation = targetRot;

        var moveDir = new Vector3(inputDir.x * speedAmount, y, inputDir.z*speedAmount);
        rb.velocity = moveDir;


    }

}
