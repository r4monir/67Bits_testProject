using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Hud : MonoBehaviour
{
    [Header("Hud")]
    [SerializeField] private FloatingJoystick _joystick;
    public FloatingJoystick Joystick { get => _joystick; }

    [SerializeField] private TextMeshProUGUI _txtMoney;
    [SerializeField] private TextMeshProUGUI _txtLvl;
    [SerializeField] private Image _imgLvl;

    [Header("World Space Hud")]
    [SerializeField] private Image _btnSellChild;
    public Image BtnSellChild { get => _btnSellChild; }
    [SerializeField] private int _childPrice = 200;
    public int ChildPrice { get => _childPrice; }

    [SerializeField] private Image _btnBuyHormone;
    public Image BtnBuyHormone { get => _btnBuyHormone; }
    [SerializeField] private int _hormonePrice = 200;
    public int HormonePrice { get => _hormonePrice; }

    [SerializeField] private float _fillLoadTime = 2;

    void Start()
    {
        SetEvents();
    }

    void SetEvents()
    {
        GameManager.Instance.Status.OnAddLvl += SetLvl;
        GameManager.Instance.Status.OnAddMoney += SetMoney;
    }

    // Mostra money na Hud
    public void SetMoney(int value)
    {
        _txtMoney.text = value.ToString();
    }

    // Calcula e seta o Lvl final na Hud
    public void SetLvl(int value)
    {
        if (_imgLvl.fillAmount == 0) _imgLvl.fillAmount = 0.5f;
        else _imgLvl.fillAmount = 0;

        _txtLvl.text = $"LEVEL {value}";
    }

    public float SellingChilds()
    {
        _btnSellChild.fillAmount += (1f / _fillLoadTime) * Time.deltaTime;

        if (_btnSellChild.fillAmount == 1)
        {
            ResetFill(_btnSellChild);
            RectTransform txtMoneyParent = _txtMoney.transform.parent.GetComponentInParent<RectTransform>();
            TweenCompleteFill(txtMoneyParent);
            return 1;
        }

        return _btnSellChild.fillAmount;
    }

    public void ResetSelling()
    {
        ResetFill(_btnSellChild);
    }

    public float BuyingHormone()
    {
        if (GameManager.Instance.Status.Money < _hormonePrice) return 0;

        _btnBuyHormone.fillAmount += (1f / _fillLoadTime) * Time.deltaTime;

        if (_btnBuyHormone.fillAmount == 1)
        {
            GameManager.Instance.Status.AddLvl();
            ResetFill(_btnBuyHormone);
            RectTransform txtLvlParent = _txtLvl.transform.parent.GetComponentInParent<RectTransform>();
            TweenCompleteFill(txtLvlParent);
            return 1;
        }

        return _btnBuyHormone.fillAmount;
    }

    public void ResetBuying()
    {
        ResetFill(_btnBuyHormone);
    }

    void ResetFill(Image fill)
    {
        fill.fillAmount = 0;
    }

    // Feedback de interação completa
    void TweenCompleteFill(RectTransform obj)
    {
        float originalScale = obj.transform.localScale.x;
        float scaleMax = 1.1f;
        float scaleDurantion = 0.2f;
        obj.DOScale(scaleMax, scaleDurantion);
        obj.DOScale(originalScale, scaleDurantion).SetDelay(scaleDurantion);
    }
}
