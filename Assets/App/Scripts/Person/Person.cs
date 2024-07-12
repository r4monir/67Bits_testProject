using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Person : MonoBehaviour, IPerson
{
    [Header("Person")]
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _turnSpeed = 10;
    [SerializeField] protected Animator _animator;

    public abstract void Move();
}
