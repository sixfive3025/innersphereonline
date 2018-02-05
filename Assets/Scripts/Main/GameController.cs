using System;
using System.Collections.Generic;
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
	Error,
	DedicatedServer
}

public class GameController : IInitializable, IDisposable, ITickable {

	[Inject]
	readonly Settings _settings;

	readonly InnerSphereBuilder _isBuilder;
	readonly RegimentBuilder _regimentBuilder;
	readonly ISONetworkManager _networkManager;
	readonly LocalPlayerManager _localPlayerManager;
	readonly CameraHandler _cameraHandler;
	readonly MouseHandler _mouseHandler;

	Signals.FactionSelected _factionSelectedSignal;
	Signals.FatalError _fatalErrorSignal;

	private GameStates _state = GameStates.Startup;
	public GameStates State 
	{
		get { return _state; }
	}
	public string LastErrorMessage = "";

	public GameController( InnerSphereBuilder isBuilder, RegimentBuilder regimentBuilder, ISONetworkManager networkManager, 
			LocalPlayerManager localPlayerManager, CameraHandler cameraHandler,
			MouseHandler mouseHandler, Signals.FactionSelected factionSelectedSignal,
			Signals.FatalError fatalErrorSignal )
	{
		_isBuilder = isBuilder;
		_regimentBuilder = regimentBuilder;
		_networkManager = networkManager;
		_localPlayerManager = localPlayerManager;
		_cameraHandler = cameraHandler;
		_mouseHandler = mouseHandler;
		_factionSelectedSignal = factionSelectedSignal;
		_fatalErrorSignal = fatalErrorSignal;
	}

	public void Initialize () 
	{
		_factionSelectedSignal += OnFactionSelected;
		_fatalErrorSignal += OnFatalError;
	}

	public void Dispose ()
	{
		_factionSelectedSignal -= OnFactionSelected;
		_fatalErrorSignal -= OnFatalError;
	}
	
	void OnFactionSelected ( string factionChosen, string playerName )
	{
		_localPlayerManager.PlayerName = playerName;
		_localPlayerManager.PlayerFactionString = factionChosen;
		_networkManager.CreateISOPlayer();
		_state = GameStates.Playing;
	}

	void OnFatalError ( string message )
	{
		LastErrorMessage = message;
		_state = GameStates.Error;
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
					// Ugh. Don't like doing this. Again. Client doesn't need this in the first place.
					GameObject.Destroy(_isBuilder.gameObject);
					GameObject.Destroy(_regimentBuilder.gameObject);
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
			case GameStates.Error:
				_cameraHandler.CameraMovementEnabled = false;
				_mouseHandler.MouseInputEnabled = false;
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

		// Start the party!
		_networkManager.StartServer();
		_state = GameStates.DedicatedServer;
		Debug.Log( "*** STARTED DEDICATED SERVER ***" );
		_isBuilder.BuildSystems(true);
		GameObject.Destroy(_isBuilder.gameObject); // One time use

		_regimentBuilder.Build(true);
		GameObject.Destroy(_regimentBuilder.gameObject);
	}

	void StartOfflineMode()
	{
		_state = GameStates.OfflineMode;

		_isBuilder.BuildSystems(false);
		GameObject.Destroy(_isBuilder.gameObject); // One time use

		_regimentBuilder.Build(false);
		GameObject.Destroy(_regimentBuilder.gameObject);
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
