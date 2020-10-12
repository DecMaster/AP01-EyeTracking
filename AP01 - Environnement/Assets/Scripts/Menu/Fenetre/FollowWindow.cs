using System.Collections;
using System.Collections.Generic;
using Tobii.XR;
using UnityEngine;

public class FollowWindow : MonoBehaviour
{
    // PROPRIETES
    [SerializeField] private GameObject Canvas;
    [SerializeField] private float _maxViewAngle = 20f;
    [SerializeField] private float PreferredDistance = 2f;
    [SerializeField] private float ClampBottomY = .74f;
    [SerializeField] private float _slerpRate = .1f;
    private Transform _camera;
    private Quaternion _targetRot;
    private Vector3 _targetPos;

    // METHODES UNITY
    private void Start()
    {
        // RECUP
        _camera = CameraHelper.GetCameraTransform();
        Canvas = transform.Find("Canvas").gameObject;
    }
    void Update()
    {
        float angleBetweenHeadsetAndPopup = Vector3.Angle(Canvas.transform.position - _camera.transform.position, _camera.transform.forward);
        if (angleBetweenHeadsetAndPopup > _maxViewAngle)
        {
            MovePopupIntoFov();
        }

        Canvas.transform.position = Vector3.Slerp(Canvas.transform.position, _targetPos, _slerpRate);
        Canvas.transform.rotation = Quaternion.Slerp(Canvas.transform.rotation, _targetRot, _slerpRate);
    }

    // AUTRES METHODES
    private void MovePopupIntoFov()
    {
        _targetPos = _camera.transform.position + _camera.transform.forward * PreferredDistance; // project ahead of gaze 

        if (_targetPos.y < ClampBottomY)
            _targetPos.Scale(new Vector3(1, 0, 1)); // wipe out y level

        while (_targetPos.y < ClampBottomY) // keep bumping it up until we hit minimum desired location
        {
            _targetPos += new Vector3(0, ClampBottomY, 0);
            _targetPos += Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
        }

        Quaternion newRotation = Quaternion.LookRotation(_targetPos - _camera.transform.position); // face user
        _targetRot.eulerAngles.Set(_targetRot.eulerAngles.x, _targetRot.eulerAngles.y, 0);
        _targetRot = newRotation;
    }
}
