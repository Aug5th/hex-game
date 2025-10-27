using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_UnitSelection : MonoBehaviour
{
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _endButton;

    public UnityAction OnAttackButtonClicked;
    public UnityAction OnMoveButtonClicked;
    public UnityAction OnEndButtonClicked;

    void Awake()
    {
        _attackButton.onClick.AddListener(ClickAttackButton);
        _moveButton.onClick.AddListener(ClickMoveButton);
        _endButton.onClick.AddListener(ClickEndButton);
        HideUI();
    }

    private void ClickEndButton()
    {
        HideUI();
        OnEndButtonClicked?.Invoke();
    }

    private void ClickMoveButton()
    {
        HideUI();
        OnMoveButtonClicked?.Invoke();
    }

    private void ClickAttackButton()
    {
        HideUI();
        OnAttackButtonClicked.Invoke();
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
