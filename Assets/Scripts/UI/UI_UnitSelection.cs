using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_UnitSelection : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform optionUI;

    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _moveButton;
    [SerializeField] private Button _endButton;

    public UnityAction OnAttackButtonClicked;
    public UnityAction OnMoveButtonClicked;
    public UnityAction OnEndButtonClicked;

    private Camera mainCamera;
    void Awake()
    {
        mainCamera = Camera.main;
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

    public void ShowUI(Transform target)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        screenPos.y -= 50;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPos
        );

        optionUI.anchoredPosition = localPos;
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
