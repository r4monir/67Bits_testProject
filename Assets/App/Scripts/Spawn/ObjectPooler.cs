using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    Dictionary<string, Queue<GameObject>> _objsPool = new Dictionary<string, Queue<GameObject>>();
    Dictionary<GameObject, Coroutine> _recyclerCoroutines = new Dictionary<GameObject, Coroutine>(); // Dicionário para controlar as coroutines de reciclagem

    public GameObject SpawnFromPool(GameObject go, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        string tagGO = go.name;
        GameObject GOToSpawn = null;

        if (!_objsPool.ContainsKey(tagGO))
        {
            GOToSpawn = NewGameObjectToPool(tagGO, go, position, rotation, parent);
        }
        else
        {
            GOToSpawn = _objsPool[tagGO].Peek();

            if (GOToSpawn.activeSelf)
            {
                GOToSpawn = NewGameObjectToPool(tagGO, go, position, rotation, parent);
            }
            else
            {
                GOToSpawn = _objsPool[tagGO].Dequeue();

                if (parent != null) GOToSpawn.transform.SetParent(parent);

                GOToSpawn.transform.position = position;
                GOToSpawn.transform.rotation = rotation;

                GOToSpawn.SetActive(true);
            }
        }
        _objsPool[tagGO].Enqueue(GOToSpawn);

        return GOToSpawn;
    }

    GameObject NewGameObjectToPool(string tagGO, GameObject go, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject newGO = Instantiate(go, position, rotation, parent);

        if (!_objsPool.ContainsKey(tagGO))
        {
            Queue<GameObject> gObjPool = new Queue<GameObject>();

            gObjPool.Enqueue(newGO);

            _objsPool.Add(tagGO, gObjPool);
        }

        return newGO;
    }

    public void Recycle(GameObject go, float time = 0)
    {
        if (time == 0)
        {
            go.SetActive(false);
            return;
        }
		
        if (_recyclerCoroutines.ContainsKey(go))
        {
            StopCoroutine(_recyclerCoroutines[go]);
        }

		Coroutine recyclerCoroutine = StartCoroutine(CoroutineRecycle(go, time));
        _recyclerCoroutines[go] = recyclerCoroutine;
    }

    IEnumerator CoroutineRecycle(GameObject go, float time = 0)
    {
        yield return new WaitForSeconds(time);
        go.SetActive(false);
    }
}