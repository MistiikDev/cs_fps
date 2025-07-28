using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] public PlayerMouvement playerMouvement;
    
    public Camera camera;
    public GameObject cameraPosition;
    
    public float cameraSmoothness = 0.15f;
    public float mouseSensitivity = 100f;

    private float XRot;
    private float YRot;
    
    private Vector3 _cameraBobOffset;
    private Vector3 _cameraImpulseOffset;
    
    private Coroutine _impulseCoroutine;
    
    private void AnimateCameraOnLand(Collision collider)
    {
        float impactY = -collider.impulse.y / 150f;

        if (_impulseCoroutine != null)
        {
            StopCoroutine(_impulseCoroutine);
        }

        if (impactY > 0) return;

        _impulseCoroutine = StartCoroutine(ApplyCameraImpulse(Vector3.up * impactY, 1.0f));
    }
    
    IEnumerator ApplyCameraImpulse(Vector3 Impulse, float duration) 
    {
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            _cameraImpulseOffset = Vector3.Slerp(Impulse, Vector3.zero, (float)(elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _cameraImpulseOffset = Vector3.zero;
    }
    
    private void OnCollisionEnd(Collision other)
    {
        AnimateCameraOnLand(other);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        AnimateCameraOnLand(other);
    }
    
    void Start()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        YRot += mouseX;
        XRot -= mouseY;

        XRot = Mathf.Clamp(XRot, -90f, 90f);

        Vector3 cameraTarget = new Vector3
        (
            this.cameraPosition.transform.position.x,
            this.cameraPosition.transform.position.y,
            this.cameraPosition.transform.position.z
        ) + _cameraBobOffset + _cameraImpulseOffset;

        Vector3 playerVelocity = playerMouvement.GetPlayerVelocity();
        
        this.camera.transform.position = Vector3.SmoothDamp(this.camera.transform.position, cameraTarget, ref playerVelocity, cameraSmoothness);
        
        this.camera.transform.localRotation = Quaternion.Euler(XRot, YRot, 0f);
        this.transform.rotation = Quaternion.Euler(0f, YRot, 0f);
    }
}
