using System;
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
	
	private Signals.PlayerJoined _playerJoinedSignal;
	private Signals.PlayerDeparted _playerDepartedSignal;

	enum UIStates { Nothing, PickFaction, Playing };
	private UIStates _uiState = UIStates.Nothing;

	private FactionPickerUI _factionUI;
	private ShowCoordsUI _showCoodsUI;
	private PlayerHUDUI _playerList;
	private SystemHUDUI _systemUI;

	public UIManager( GameController gameController, 
					  FactionPickerUI.Factory factionPickerFactory, 
					  ShowCoordsUI.Factory showCoordsFactory,
					  PlayerHUDUI.Factory playerListFactory,
					  SystemHUDUI.Factory systemHUDFactory,
					  Signals.PlayerJoined playerJoinedSignal,
					  Signals.PlayerDeparted playerDepartedSignal )
	{
		_gameController = gameController;
		_factionPickerFactory = factionPickerFactory;
		_showCoordsFactory = showCoordsFactory;
		_playerListFactory = playerListFactory;
		_systemHUDFactory = systemHUDFactory;

		_playerJoinedSignal = playerJoinedSignal;
		_playerDepartedSignal = playerDepartedSignal;
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
	}

	public void SystemDeselected( StarSystemController system )
	{
		GameObject.Destroy( _systemUI.gameObject );
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
