using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SelectionState
{
    UnitSelected,
    UnitDeselected,
    Moving,
    Attacking,
    None
}

public class UnitSelectionManager : Singleton<UnitSelectionManager>
{
    private Unit _selectedUnit;

    public Unit SelectedUnit => _selectedUnit;

    private SelectionState _currentState = SelectionState.None;
    public SelectionState CurrentState => _currentState;

    [SerializeField] private UI_UnitSelection _unitSelectionUI;

    void OnEnable()
    {
        _unitSelectionUI.OnAttackButtonClicked += HandleUnitAttackSelected;
        _unitSelectionUI.OnMoveButtonClicked += HandleUnitMoveSelected;
        _unitSelectionUI.OnEndButtonClicked += HandleUnitEndSelected;
    }

    void OnDisable()
    {
        _unitSelectionUI.OnAttackButtonClicked -= HandleUnitAttackSelected;
        _unitSelectionUI.OnMoveButtonClicked -= HandleUnitMoveSelected;
        _unitSelectionUI.OnEndButtonClicked -= HandleUnitEndSelected;
        if (TileManager.Instance != null)
        {
            TileManager.Instance.OnTileClickedEvent -= HandleTileClicked;
        }
    }

    void Start()
    {
        TileManager.Instance.OnTileClickedEvent += HandleTileClicked;
    }

    private void HandleTileClicked(TileData tileData)
    {
        Debug.Log($"Tile clicked at {tileData.CellPos}");

        if (_selectedUnit != null && tileData.CellPos == _selectedUnit.CurrentTile.CellPos)
        {
            DeselectUnit();
            _unitSelectionUI.HideUI();
            TileManager.Instance.ClearHighlights();
            return;
        }

        if (_selectedUnit == null && tileData.Unit != null)
        {
            _currentState = SelectionState.UnitSelected;
            _selectedUnit = tileData.Unit;
            _unitSelectionUI.ShowUI(_selectedUnit.transform);
        }

        if (_selectedUnit != null && _currentState == SelectionState.Moving)
        {
            TileManager.Instance.HandleMoveUnit(_selectedUnit, tileData);
            DeselectUnit();
        }

        if (_currentState == SelectionState.UnitDeselected)
        {
            DeselectUnit();
            _unitSelectionUI.HideUI();
        }
    }

    private void DeselectUnit()
    {
        if (_selectedUnit != null)
        {
            _currentState = SelectionState.UnitDeselected;
            _selectedUnit = null;
        }
    }

    private void HandleUnitAttackSelected()
    {
        _currentState = SelectionState.Attacking;
    }

    private void HandleUnitMoveSelected()
    {
        Debug.Log("Move action selected");
        _currentState = SelectionState.Moving;
        TileManager.Instance.HighlightMoveRange(_selectedUnit.CurrentTile.CellPos, _selectedUnit.MoveRange, _selectedUnit.CurrentTile.TileType);
    }

    private void HandleUnitEndSelected()
    {
        DeselectUnit();
    }
}
