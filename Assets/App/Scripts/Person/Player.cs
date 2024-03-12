using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : Person
{
    [Header("Player")]
    [SerializeField] private Transform childsParent;
    [SerializeField] private float childsParentPosY = 2.5f;    
    [SerializeField] private List<Enemy> enemysCollected;
    [SerializeField] private Material material;
    [SerializeField] private int startEnemysCapacity;
    [SerializeField] private float elasticity; // Fator de amortecimento para simular inércia
    private int enemysCapacity;

    private void Start() {
        enemysCapacity = startEnemysCapacity;
        UpdateCapacity(1);
        ChangeColor(0);
    }

    private void Update()
    {       
        SetChildsParentPosition();
        Move();
        MoveChildsScript();
    }

    // Coloca o pai da pilha sempre acima do Player
    void SetChildsParentPosition(){
        childsParent.position = new Vector3(transform.position.x, childsParentPosY, transform.position.z);
    }

    void Move(){
        rb.velocity = new Vector3(GameManager.Instance.Hud.Joystick.Horizontal * moveSpeed, 0, GameManager.Instance.Hud.Joystick.Vertical * moveSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);

        if (GameManager.Instance.Hud.Joystick.Horizontal != 0 || GameManager.Instance.Hud.Joystick.Vertical != 0) // Detectar se o joystick está sendo ultilizado
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            animator.SetBool("isRunning", true);
        }
        else
            animator.SetBool("isRunning", false);
    }

    // Efeito elastico da pilha por script
    void MoveChildsScript(){
        if(enemysCollected.Count < 1) return;

        enemysCollected[0].transform.position = childsParent.position;

        // Percorre a lista de objetos empilhados, movendo cada objeto para a posição do objeto à sua frente
        for (int i = 1; i < enemysCollected.Count; i++)
        {
            Vector3 nextPosition = new Vector3(enemysCollected[i - 1].transform.position.x, 
                childsParent.position.y + ((float)i/2), enemysCollected[i - 1].transform.position.z);
            enemysCollected[i].transform.position = Vector3.Lerp(enemysCollected[i].transform.position, nextPosition, moveSpeed * Time.deltaTime);

            // Aplicar força elástica
            Vector3 distance = nextPosition - enemysCollected[i].transform.position;
            float distanceMag = distance.magnitude;
            Vector3 force = distance.normalized * (elasticity * 100) * distanceMag;
            enemysCollected[i].ElasticEffect(force);
        }
    }


    private void OnCollisionEnter(Collision c) {
        var enemy = c.gameObject.GetComponent<Enemy> ();
        if(enemy) HitEnemy(enemy);
    }

    // Detecta colisão com inimigo, que sofre um hit ou é coletado
    void HitEnemy(Enemy enemy){
        if(!enemy.IsRagdoll) {
            enemy.Ragdoll(true);
            TweenHit(enemy.transform);
        }
        else Collect(enemy);
    }

    // Feedback de colisão
    void TweenHit(Transform obj){
        float originalScale = obj.transform.localScale.x;
        float scaleMax = 1.5f;
        float scaleDurantion = 0.2f;
        obj.DOScale(scaleMax, scaleDurantion);
        obj.DOScale(originalScale, scaleDurantion).SetDelay(scaleDurantion);
    }

    // Coleta o inimigo para ser empilhado
    void Collect(Enemy enemy){
        var enemysCapacityFinal = enemysCapacity - 1;
        float enemysCollectedFinal = enemysCollected.Count;
        if(enemysCapacityFinal < enemysCollectedFinal) return; // Retorna caso já tenha alcançado o limite de inimigos

        enemy.Collected();
        enemysCollected.Add(enemy);
        enemy.transform.position = new Vector3(childsParent.position.x, childsParent.position.y + (enemysCollected.Count/2), childsParent.position.z);
    }

    // Melhora a capacidade de inimigos empilhados possiveis de acordo com o lvl
    public void UpdateCapacity(int lvl){
        enemysCapacity = startEnemysCapacity + lvl;
    }

    private void OnTriggerEnter(Collider c) {
        var enemy = c.gameObject.GetComponent<Enemy> ();
        if(enemy) HitEnemy(enemy);
    }

    private void OnTriggerStay(Collider c) {
        var btnSellChild = c.gameObject == GameManager.Instance.Hud.BtnSellChild.gameObject;
        var btnBuyHormone = c.gameObject == GameManager.Instance.Hud.BtnBuyHormone.gameObject;
        if(btnSellChild) {
            if(enemysCollected.Count <= 0) return;
            if(GameManager.Instance.Hud.SellingChilds() == 1) { // Quando completa o botão
                SellChildrens();
            }
        } else if(btnBuyHormone) {
            GameManager.Instance.Hud.BuyingHormone(); // Apenas chama a função do botão
        }
    }

    // Muda cor do player de acordo com o lvl
    public void ChangeColor(int lvl){
        float modifier = 1 - ((float)lvl / 10);
        Color newColor = new Color(modifier, modifier, modifier, 1f);
        material.color = newColor;
    }

    private void OnTriggerExit(Collider c) {
        var btnSellChild = c.gameObject == GameManager.Instance.Hud.BtnSellChild.gameObject;
        var btnBuyHormone = c.gameObject == GameManager.Instance.Hud.BtnBuyHormone.gameObject;
        if(btnSellChild) 
            GameManager.Instance.Hud.ResetSelling();
        else if(btnBuyHormone) 
            GameManager.Instance.Hud.ResetBuying();
    }

    // Vende as crianças empilhadas
    void SellChildrens(){
        var gain = enemysCollected.Count * GameManager.Instance.Hud.ChildPrice;
        GameManager.Instance.Status.AddMoney(gain);

        foreach (Enemy enemy in enemysCollected){
            enemy.gameObject.transform.SetParent(null);
            GameManager.Instance.ObjectPooler.Recicle (enemy.gameObject);
        }
        
        GameManager.Instance.Spawner.AddEnemyCount(-enemysCollected.Count);
        enemysCollected.Clear();
    }
}
