using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTransform;               // The object the camera will follow
    public Transform cameraPivot;                   // The object the camera uses to pivot (look up and down)
    public Transform cameraTransform;               // The transform of the actual camera object in the scene
    public LayerMask collisionLayers;               // The layers we want our camera to collide with 
    private float defaultPosition;                  // Position the camera will default to
    private Vector3 cameraVectorposition;

    public float cameraCollisionOffset = 0.2f;      //How much the camera will jump off of objects its colliding with
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;

    private Vector3 cameraFollowVelocity = Vector3.zero;  //Mentions the cuurent velocity of the camera
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 15;
    public float cameraPivotSpeed = 15;
    public float cameraSmoothTime = 1;

    public float lookAngle;                         //Camera looking up and down
    public float pivotAngle;                        //Camera looking left and right

    public float minimumPivotAngle = -35;           //Limit on how far the camera can look up and down
    public float maximumPivotAngle = 35;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    public void FollowTarget()
    {
        //Tells the camera to follow the target
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        
        transform.position = targetPosition;
    }

    public void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = Mathf.Lerp(lookAngle, lookAngle + (inputManager.cameraInputX * cameraLookSpeed),cameraSmoothTime*Time.deltaTime);
        pivotAngle = Mathf.Lerp(pivotAngle, pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed), cameraSmoothTime*Time.deltaTime);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()               // Function to avoid camera collision with objects
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;                                 // Tells us what we hit
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        // Creates a invisible sphere around the camera
        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers)){

            float distance = Vector3.Distance(cameraPivot.position, hit.point);    // The distance between the camera and the impact point
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = targetPosition - minimumCollisionOffset;
        }

        cameraVectorposition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorposition;
    }
}
