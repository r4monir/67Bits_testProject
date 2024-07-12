using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : Person
{
    [Header("Child")]
    [SerializeField] private Transform _tansformAvatar;
    [SerializeField] private BoxCollider _mainBoxC;
    [SerializeField] private int _timeChangeDirection = 3;
    private Vector2 _moveDirection;
    private bool _isRagdoll;
    public bool IsRagdoll { get => _isRagdoll; }
    bool _isColleted;

    private void Start()
    {
        StartCoroutine(CoroutineSetDirection());
        Ragdoll(false);
    }

    // Seta a direção que o inimigo vai andar
    void SetDirection()
    {
        float variantX = Random.Range(-1f, 1f);
        float variantY = Random.Range(-1f, 1f);

        _moveDirection = new Vector2(variantX, variantY);
    }

    // Muda direção que o inimigo vai andar
    IEnumerator CoroutineSetDirection()
    {
        SetDirection();
        yield return new WaitForSeconds(_timeChangeDirection);
        StartCoroutine(CoroutineSetDirection());
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (_isRagdoll) return; // Não movimenta caso esteja em modo Ragdoll

        _rb.velocity = new Vector3(_moveDirection.x * _moveSpeed, 0, _moveDirection.y * _moveSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(_rb.velocity.normalized);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
    }

    // Função para ativar e desativar o Ragdoll
    public void Ragdoll(bool active)
    {
        _mainBoxC.enabled = !active;
        _isRagdoll = active;
        _animator.enabled = !active;
        _tansformAvatar.gameObject.SetActive(active);
        if (!active) return;
        if (_isColleted) return;
        StartCoroutine(CoroutineActiveTriggerInteraction());
    }

    // Ativa o mainBoxC como Trigger para o inimigo ser coletado em vez de atacado
    IEnumerator CoroutineActiveTriggerInteraction()
    {
        yield return new WaitForSeconds(1);
        _mainBoxC.isTrigger = true;
        _mainBoxC.enabled = true;
    }

    public void Collected()
    {
        _isColleted = true;
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        _mainBoxC.enabled = false;
        _animator.SetBool("isWalking", false);
    }

    // Efeito elastico quando estiver empilhado
    public void ElasticEffect(int id, Vector3 previousChildPos, float baseY, int elasticity)
    {
        var followSpeed = 10;
        Vector3 nextPosition = new Vector3(previousChildPos.x, baseY + ((float)id / 2), previousChildPos.z);
        transform.position = Vector3.Lerp(transform.position, nextPosition, followSpeed * Time.deltaTime);

        // Aplicar força elástica
        Vector3 distance = nextPosition - transform.position;
        float distanceMag = distance.magnitude;
        Vector3 force = distance.normalized * (elasticity * 100) * distanceMag;
        _rb.AddForce(force, ForceMode.Force);
    }

    // Reseta a crianca quando desabilitado para poder ser reciclado e reutilizado
    private void OnDisable()
    {
        ResetChild();
    }

    public void ResetChild()
    {
        Ragdoll(false);

        _mainBoxC.isTrigger = false;
        _isColleted = false;
        _animator.SetBool("isWalking", true);
    }
}
