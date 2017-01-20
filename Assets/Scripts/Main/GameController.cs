using System;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;
using ModestTree;

public enum GameStates
{
	Startup,
	Connecting,
	SelectingFaction,
	OfflineMode,
	Playing,
	Disconnected,
	DedicatedServer
}

public class GameController : IInitializable, IDisposable, ITickable {

	[Inject]
	readonly Settings _settings;

	readonly InnerSphereBuilder _isBuilder;
	readonly ISONetworkManager _networkManager;
	readonly LocalPlayerManager _localPlayerManager;
	readonly CameraHandler _cameraHandler;
	readonly MouseHandler _mouseHandler;

	Signals.FactionSelected _factionSelectedSignal;

	private GameStates _state = GameStates.Startup;
	public GameStates State 
	{
		get { return _state; }
	}

	public GameController( InnerSphereBuilder isBuilder, ISONetworkManager networkManager, 
			LocalPlayerManager localPlayerManager, CameraHandler cameraHandler,
			MouseHandler mouseHandler, Signals.FactionSelected factionSelectedSignal )
	{
		_isBuilder = isBuilder;
		_networkManager = networkManager;
		_localPlayerManager = localPlayerManager;
		_cameraHandler = cameraHandler;
		_mouseHandler = mouseHandler;
		_factionSelectedSignal = factionSelectedSignal;
	}

	public void Initialize () 
	{
		_factionSelectedSignal += OnFactionSelected;
	}

	public void Dispose ()
	{
		_factionSelectedSignal -= OnFactionSelected;
	}
	
	void OnFactionSelected ( string factionChosen, string playerName )
	{
		_localPlayerManager.PlayerName = playerName;
		_localPlayerManager.PlayerFactionString = factionChosen;
		_networkManager.CreateISOPlayer();
		_state = GameStates.Playing;
	}

	public void Tick () 
	{	
		switch ( _state )
		{
			case GameStates.Startup:
				if ( IsHeadless() ) StartDedicatedServer();
				else if ( _settings.OfflineMode ) StartOfflineMode();
				else 
				{
					StartClient();
					// Cleanup because someone else is building the Inner Sphere
					GameObject.Destroy(_isBuilder.gameObject);
				}
				break;
			case GameStates.Connecting:
				break;
			case GameStates.SelectingFaction:
				break;
			case GameStates.OfflineMode: // Offline mode and Playing currently the same
			case GameStates.Playing:
				_cameraHandler.CameraMovementEnabled = true;
				_mouseHandler.MouseInputEnabled = true;
				break;
			case GameStates.Disconnected:
				break;
			case GameStates.DedicatedServer:
				break;
			default:
				Assert.That(false);
				break;
		}	
	}

	

	public bool IsHeadless() 
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
	}

	void StartDedicatedServer()
	{
		Assert.That( _state == GameStates.Startup );
		
		// Reduce CPU usage when headleass
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 20;
		Application.runInBackground = true;

		_networkManager.StartServer();
		_state = GameStates.DedicatedServer;
		Debug.Log( "*** STARTED DEDICATED SERVER ***" );
		_isBuilder.BuildSystems(true);
		GameObject.Destroy(_isBuilder.gameObject); // One time use
	}

	void StartOfflineMode()
	{
		_isBuilder.BuildSystems(false);
		GameObject.Destroy(_isBuilder.gameObject); // One time use
		_state = GameStates.OfflineMode;
	}

	void OnClientConnect()
	{
		Debug.Log( "*** CONNECTED TO SERVER ***" );
		_state = GameStates.SelectingFaction;
	}

	void StartClient()
	{
		Assert.That( _state == GameStates.Startup );
		_networkManager.ConnectDelegate = OnClientConnect;
		_networkManager.StartClient();
		_state = GameStates.Connecting;
	}

	[Serializable]
	public class Settings
	{
		public bool OfflineMode;
	}
}
