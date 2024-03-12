using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager<GameManager>
{    
    private Player _player;
    public Player Player { get => _player; }
    private Spawner _spawner;
    public Spawner Spawner { get => _spawner; }
    private ObjectPooler _objectPooler;
    public ObjectPooler ObjectPooler { get => _objectPooler; }
    private Hud _hud;
    public Hud Hud { get => _hud; }
    private Status _status;
    public Status Status { get => _status; }

    void Awake()
    {
        this._player = FindFirstObjectByType<Player>();
        this._spawner = FindFirstObjectByType<Spawner>();
        this._objectPooler = FindFirstObjectByType<ObjectPooler>();
        this._hud = FindFirstObjectByType<Hud>();
        this._status = FindFirstObjectByType<Status>();
    }
}