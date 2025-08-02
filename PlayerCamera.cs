using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCamera : MonoBehaviour
{
    public Camera player_camera;
    public GameObject cameraPosition;
    public GameObject cameraRotation;
    
    public GameObject itemTransform;
    
    public float cameraSmoothness = 0.05f;
    public float mouseSensitivity = 100f;
    
    private float XRot;
    private float YRot;
    
    private Vector3 _cameraBobOffset;
    private Vector3 _cameraImpulseOffset;
    private Vector3 _cameraRecoilOffset;

    private Vector3 _currentCameraVelocity;
    
    private Coroutine _impulseCoroutine;
    private Coroutine _recoilCoroutine;
    
    private Player player;
    
    public void Init(Player _player)
    {
        player = _player;
    }
    
    public void Shake(float ShakeAmount, float duration) 
    {
        if (_recoilCoroutine != null)
        {
            StopCoroutine(_recoilCoroutine);
        }

        _recoilCoroutine = StartCoroutine(ApplyCameraRecoil(ShakeAmount, duration));
    }

    public GameObject GetItemTransform()
    {
        return itemTransform;
    }
    public void AnimateCameraOnLand(Collision collider)
    {
        float impactY = -collider.impulse.y / 200f;

        impactY = Mathf.Clamp(0, Mathf.Sign(impactY) * 350, impactY);

        if (_impulseCoroutine != null)
        {
            StopCoroutine(_impulseCoroutine);
        }

        if (impactY > 0) return;

        _impulseCoroutine = StartCoroutine(ApplyCameraImpulse(Vector3.up * impactY, 1.0f));
    }

    IEnumerator ApplyCameraRecoil(float RecoilAmount, float duration)
    {
        float elapsedTime = 0f;
        float RecoilSeedX = Random.Range(RecoilAmount / 2, RecoilAmount * 2);
        float RecoilSeedY = Random.Range(-RecoilAmount / 2, RecoilAmount * 2) * 2;
        
        _cameraRecoilOffset += new Vector3(-RecoilSeedX, RecoilSeedY, 0);
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            _cameraRecoilOffset = Vector3.Slerp(_cameraRecoilOffset, Vector3.zero, (float)(elapsedTime / duration));
            yield return null;
        }
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

        Vector3 cameraTargetPosition = Vector3.SmoothDamp(this.player_camera.transform.position, cameraTarget, ref _currentCameraVelocity, cameraSmoothness);
        Quaternion cameraTargetRotation = Quaternion.Euler(XRot + _cameraRecoilOffset.x, YRot + _cameraRecoilOffset.y, 0f);
        
        this.player_camera.transform.position = cameraTargetPosition;
        this.player_camera.transform.localRotation = cameraTargetRotation;
        
        this.cameraRotation.transform.position = this.player_camera.transform.position;
        this.cameraRotation.transform.rotation = this.player_camera.transform.rotation;
        
        this.transform.rotation = Quaternion.Euler(0f, YRot, 0f);
    }
}
