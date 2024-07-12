using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset; // A distância da câmera em relação ao objeto

    void LateUpdate()
    {
        transform.position = _target.position + _offset;
    }
}
