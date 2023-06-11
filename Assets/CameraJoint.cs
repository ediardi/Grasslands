using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraControl
{
    public class CameraJoint : MonoBehaviour
    {
        [SerializeField] private float _speed = 15f;
        [SerializeField] private float _smoothing = 5f;

        private float _targetAnglex;
        private float _currentAnglex;

        private void Awake()
        {
            _targetAnglex = transform.eulerAngles.x;
            _currentAnglex = _targetAnglex;
        }

        private void HandleInput()
        {
            if (!Input.GetMouseButton(1)) return;
            _targetAnglex += Input.GetAxisRaw("Mouse Y") * _speed;
            if (_targetAnglex < -30f)
                _targetAnglex = -30f;
            if (_targetAnglex > 5f)
                _targetAnglex = 5f;
        }

        private void Rotate()
        {
            _currentAnglex = Mathf.Lerp(_currentAnglex, _targetAnglex, Time.deltaTime * _smoothing);
            transform.localRotation = Quaternion.AngleAxis(_currentAnglex, Vector3.left);
        }

        private void Update()
        {
            HandleInput();
            Rotate();
        }
    }
}
