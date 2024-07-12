using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    private int _lvl;
    public int Lvl { get => _lvl; }
    public delegate void OnAddLvlEventHandler(int value);
    public event OnAddLvlEventHandler OnAddLvl;

    private int _money;
    public int Money { get => _money; }
    public delegate void OnAddMoneyEventHandler(int value);
    public event OnAddMoneyEventHandler OnAddMoney;

    private void Start()
    {
        _lvl = 100;
    }

    public void AddMoney(int value)
    {
        _money = _money + value;

        OnAddMoney?.Invoke(_money);
    }

    public void AddLvl()
    {
        _lvl += 50;

        int realLvl = (int)(_lvl / 100);

        OnAddLvl?.Invoke(realLvl);

        AddMoney(-GameManager.Instance.Hud.HormonePrice);
    }

}
