using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIManager : ITickable, IInitializable, IDisposable {

	[Inject]
	readonly Settings _settings;

	readonly GameController _gameController;
	readonly FactionPickerUI.Factory _factionPickerFactory;
	readonly ShowCoordsUI.Factory _showCoordsFactory;
	readonly PlayerHUDUI.Factory _playerListFactory;
	readonly SystemHUDUI.Factory _systemHUDFactory;
	readonly ErrorModalUI.Factory _errorModalFactory;
	
	private Signals.PlayerJoined _playerJoinedSignal;
	private Signals.PlayerDeparted _playerDepartedSignal;
	private Signals.RegimentMoved _regimentMovedSignal;

	enum UIStates { Nothing, PickFaction, Playing, Error };
	private UIStates _uiState = UIStates.Nothing;

	private FactionPickerUI _factionUI;
	private ShowCoordsUI _showCoodsUI;
	private PlayerHUDUI _playerList;
	private SystemHUDUI _systemUI;
	private RegimentController _selectedRegiment;
	private HashSet<string> _regimentJumpable;

	public UIManager( GameController gameController, 
					  FactionPickerUI.Factory factionPickerFactory, 
					  ShowCoordsUI.Factory showCoordsFactory,
					  PlayerHUDUI.Factory playerListFactory,
					  SystemHUDUI.Factory systemHUDFactory,
					  ErrorModalUI.Factory errorModalFactory,
					  Signals.PlayerJoined playerJoinedSignal,
					  Signals.PlayerDeparted playerDepartedSignal,
					  Signals.RegimentMoved regimentMovedSignal )
	{
		_gameController = gameController;
		_factionPickerFactory = factionPickerFactory;
		_showCoordsFactory = showCoordsFactory;
		_playerListFactory = playerListFactory;
		_systemHUDFactory = systemHUDFactory;
		_errorModalFactory = errorModalFactory;

		_playerJoinedSignal = playerJoinedSignal;
		_playerDepartedSignal = playerDepartedSignal;
		_regimentMovedSignal = regimentMovedSignal;
	}

	public void Initialize()
    {
        _playerJoinedSignal += OnPlayerJoined;
		_playerDepartedSignal += OnPlayerDeparted;
    }

    public void Dispose()
    {
        _playerJoinedSignal -= OnPlayerJoined;
		_playerDepartedSignal -= OnPlayerDeparted;
    }

	public void Tick()
	{
		switch ( _gameController.State )
		{
			case GameStates.SelectingFaction:
				if ( _uiState != UIStates.PickFaction )
				{
					resetUIState();
					_factionUI = _factionPickerFactory.Create();
					_uiState = UIStates.PickFaction;
				}
				break;
			case GameStates.OfflineMode: // For now, offline mode and playing are same UI
			case GameStates.Playing:
				if ( _uiState != UIStates.Playing )
				{
					resetUIState();
					if ( _settings.ShowCoordinatesOnClick) _showCoodsUI = _showCoordsFactory.Create();
					_uiState = UIStates.Playing;
				}
				break;
			case GameStates.Error:
				if ( _uiState != UIStates.Error )
				{
					// User must quit the application to get out of this state :(
					_errorModalFactory.Create(_gameController.LastErrorMessage);
					_uiState = UIStates.Error;
				}
				break;
		}
	}

	public void OnPlayerJoined ( PlayerController player )
	{
		if ( _playerList == null )
			_playerList = _playerListFactory.Create();

		_playerList.AddPlayer(player);
	}

	public void OnPlayerDeparted ( PlayerController player )
	{
		_playerList.RemovePlayer(player);
	}

	public void SystemSelected( StarSystemController system )
	{
		_systemUI = _systemHUDFactory.Create(system);

		if ( _selectedRegiment != null && _regimentJumpable.Contains(system.StarName))
		{
			_selectedRegiment.LocationChange(system.StarName);
			_regimentMovedSignal.Fire(_selectedRegiment.gameObject, system.StarName);
		}

		_selectedRegiment = null;
	}

	public void SystemDeselected( StarSystemController system )
	{
		GameObject.Destroy( _systemUI.gameObject );
	}

	public void RegimentSelected( RegimentController regiment, HashSet<string> jumpable )
	{
		_selectedRegiment = regiment;
		_regimentJumpable = jumpable;
	}

	public void RegimentDeselected( RegimentController regiment )
	{
		//_selectedRegiment = null;
	}

	public void UpdateCoordsUI( double x, double y)
	{
		if ( _showCoodsUI ) _showCoodsUI.UpdateCoordinates(x,y);
	}

	private void resetUIState()
	{
		switch ( _uiState )
		{
			case UIStates.PickFaction:
				GameObject.Destroy( _factionUI.gameObject );
				break;
			case UIStates.Playing:
				GameObject.Destroy( _showCoodsUI.gameObject );
				break;
		}

		_uiState = UIStates.Nothing;
	}

	[Serializable]
	public class Settings
	{
		public bool ShowCoordinatesOnClick;
	}
}
