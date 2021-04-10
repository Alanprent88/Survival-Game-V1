using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Main Camera Values")]
    public float cameraSmoothing = 10.0f;                   //how smoothly the camera moves towards the player
    public GameObject target;                                //The object the camera must follow
    [Header("Hidden Mode")]
    public Color playerHiddencolour = new Color(1.0f, 0.0f, 0.0f, 0.1f);
    Color MainMat;
    public float rayLength = 45f;                          // the length of the ay to check player
    Renderer playerRend;
    public GameObject mainCam;
    public GameObject renderCam;
    Vector3 offSet;                                         //Stores the original distatnce and rotation of the camera


    // Start is called before the first frame update
    void Start()
    {
        //The amount of gap between the target object and the camera
        offSet = transform.position - target.transform.position;
        playerRend = target.GetComponentInChildren<Renderer>();
        MainMat = target.GetComponentInChildren<Renderer>().material.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camfollow();
        ObstructionCheck();
    }
    void Camfollow()
    {
        //The camera's new position
        Vector3 camTargetPos = target.transform.position + offSet;

        //makes the camera follow the player
        transform.position = Vector3.Lerp(transform.position, camTargetPos, cameraSmoothing * Time.deltaTime);

    }
    void ObstructionCheck()
    {
        RaycastHit hit;

        if(Physics.Raycast(mainCam.transform.position, target.transform.position - mainCam.transform.position, out hit, rayLength))
        {
            if(hit.collider.gameObject.tag == ("Building"))
            {
                playerRend.material.color = playerHiddencolour;
                renderCam.SetActive(true);
            }
            else
            {
                playerRend.material.color = MainMat;
                renderCam.SetActive(false);
            }
        }
    }
}
