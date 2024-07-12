using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _child;
    [SerializeField] private Vector2 _spawnTime;
    [SerializeField] private Vector2 _spawnPos;
    [SerializeField] private int _childLimit = 20;
    private int _childCount;


    private void Start()
    {
        SpawnChild();
    }

    void SpawnChild()
    {
        StartCoroutine(CoroutineSpawnChild());
    }

    IEnumerator CoroutineSpawnChild()
    {
        if (_childCount < _childLimit)
        { // Caso tenha menos inimigos que o limite
            Spawn(_child);
            AddChildCount(1);
        }
        yield return new WaitForSeconds(Random.Range(_spawnTime.x, _spawnTime.y));
        SpawnChild();
    }

    // Update contador de inimigos
    public void AddChildCount(int value)
    {
        _childCount = _childCount + value;
    }

    void Spawn(GameObject obj)
    {
        MoveSpawner();
        var go = GameManager.Instance.ObjectPooler.SpawnFromPool(obj, transform.position, transform.rotation);
    }

    // Move o Spawner para a unidade ser gerada
    void MoveSpawner()
    {
        var newPos = transform.position;

        var variant = Random.Range(_spawnPos.x, _spawnPos.y);
        newPos[0] = variant;
        variant = Random.Range(_spawnPos.x, _spawnPos.y);
        newPos[2] = variant;

        transform.position = newPos;
    }

}
