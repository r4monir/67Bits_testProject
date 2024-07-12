using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : Person
{
    [Header("Player")]
    [SerializeField] private Material _material;
    [SerializeField] private int _startChildsCapacity = 5;
    [SerializeField] private Transform _childsParent;
    [SerializeField] private float _childsParentPosY = 2.5f;
    [SerializeField] private int _elasticity = 5; // Fator de amortecimento para simular inércia
    [SerializeField] private List<Child> _childsCollected;
    private int _childsCapacity;

    private void Start()
    {
        SetEvents();

        _childsCapacity = _startChildsCapacity;

        UpdateCapacity(1);
        ChangeColor(0);
    }

    void SetEvents()
    {
        GameManager.Instance.Status.OnAddLvl += UpdateCapacity;
        GameManager.Instance.Status.OnAddLvl += ChangeColor;
    }

    private void Update()
    {
        SetChildsParentPosition();
        Move();
        MoveChildsStack();
    }

    // Coloca o pai da pilha sempre acima do Player
    void SetChildsParentPosition()
    {
        _childsParent.position = new Vector3(transform.position.x, _childsParentPosY, transform.position.z);
    }

    public override void Move()
    {
        _rb.velocity = new Vector3(GameManager.Instance.Hud.Joystick.Horizontal * _moveSpeed, 0, GameManager.Instance.Hud.Joystick.Vertical * _moveSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(_rb.velocity.normalized);

        if (GameManager.Instance.Hud.Joystick.Horizontal != 0 || GameManager.Instance.Hud.Joystick.Vertical != 0) // Detectar se o joystick está sendo ultilizado
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
            _animator.SetBool("isRunning", true);
        }
        else
            _animator.SetBool("isRunning", false);
    }

    // Efeito elastico da pilha por script
    void MoveChildsStack()
    {
        if (_childsCollected.Count < 1) return;

        _childsCollected[0].transform.position = _childsParent.position;

        // Percorre a lista de objetos empilhados aplicando efeito elastico
        for (int i = 1; i < _childsCollected.Count; i++)
        {
            _childsCollected[i].ElasticEffect(i, _childsCollected[i - 1].transform.position, _childsParent.position.y, _elasticity);
        }
    }


    private void OnCollisionEnter(Collision c)
    {
        var child = c.gameObject.GetComponent<Child>();
        if (child) HitChild(child);
    }

    // Detecta colisão a crianca, que sofre um hit ou é coletado
    void HitChild(Child child)
    {
        if (!child.IsRagdoll)
        {
            child.Ragdoll(true);
            TweenHit(child.transform);
        }
        else Collect(child);
    }

    // Feedback de colisão
    void TweenHit(Transform obj)
    {
        float originalScale = obj.transform.localScale.x;
        float scaleMax = 1.5f;
        float scaleDurantion = 0.2f;
        obj.DOScale(scaleMax, scaleDurantion);
        obj.DOScale(originalScale, scaleDurantion).SetDelay(scaleDurantion);
    }

    // Coleta a crianca para ser empilhado
    void Collect(Child child)
    {
        var childsCapacityFinal = _childsCapacity - 1;
        float childsCollectedFinal = _childsCollected.Count;
        if (childsCapacityFinal < childsCollectedFinal) return; // Retorna caso já tenha alcançado o limite de inimigos

        child.Collected();
        _childsCollected.Add(child);
        child.transform.position = new Vector3(_childsParent.position.x, _childsParent.position.y + (_childsCollected.Count / 2), _childsParent.position.z);
    }

    // Melhora a capacidade de criancas empilhados possiveis de acordo com o lvl
    public void UpdateCapacity(int lvl)
    {
        _childsCapacity = _startChildsCapacity + lvl;
    }

    private void OnTriggerEnter(Collider c)
    {
        var child = c.gameObject.GetComponent<Child>();
        if (child) HitChild(child);
    }

    private void OnTriggerStay(Collider c)
    {
        var btnSellChild = c.gameObject == GameManager.Instance.Hud.BtnSellChild.gameObject;
        var btnBuyHormone = c.gameObject == GameManager.Instance.Hud.BtnBuyHormone.gameObject;
        if (btnSellChild)
        {
            if (_childsCollected.Count <= 0) return;
            if (GameManager.Instance.Hud.SellingChilds() == 1) // Quando completa o botão
            { 
                SellChildrens();
            }
        }
        else if (btnBuyHormone)
        {
            GameManager.Instance.Hud.BuyingHormone(); // Apenas chama a função do botão
        }
    }

    // Muda cor do player de acordo com o lvl
    public void ChangeColor(int lvl)
    {
        float modifier = 1 - ((float)lvl / 10);
        Color newColor = new Color(modifier, modifier, modifier, 1f);
        _material.color = newColor;
    }

    private void OnTriggerExit(Collider c)
    {
        var btnSellChild = c.gameObject == GameManager.Instance.Hud.BtnSellChild.gameObject;
        var btnBuyHormone = c.gameObject == GameManager.Instance.Hud.BtnBuyHormone.gameObject;
        if (btnSellChild)
            GameManager.Instance.Hud.ResetSelling();
        else if (btnBuyHormone)
            GameManager.Instance.Hud.ResetBuying();
    }

    // Vende as crianças empilhadas
    void SellChildrens()
    {
        var gain = _childsCollected.Count * GameManager.Instance.Hud.ChildPrice;
        GameManager.Instance.Status.AddMoney(gain);

        foreach (Child child in _childsCollected)
        {
            child.gameObject.transform.SetParent(null);
            GameManager.Instance.ObjectPooler.Recycle(child.gameObject);
        }

        GameManager.Instance.Spawner.AddChildCount(-_childsCollected.Count);
        _childsCollected.Clear();
    }
}
