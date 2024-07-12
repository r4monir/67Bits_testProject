using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [Header("Person")]
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _turnSpeed = 10;
    [SerializeField] protected Animator _animator;
}
