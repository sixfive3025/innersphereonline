using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class PlayerController : NetworkBehaviour, IDisposable {

	[SyncVar(hook="SyncPlayerName")] 
	public string _playerName;

	[Inject]
	readonly LocalPlayerManager _localPlayerManager;

	private SignalDispatcher _signalDispatcher;
	private Signals.SystemFactionChanged _systemFactionChangedSignal;

	public delegate void NotifyPlayerNameChanged();
	public NotifyPlayerNameChanged NotifyPlayerNameChangeDelegate = null;

	[Inject]
	public void Construct(SignalDispatcher signalDispatcher, 
						  Signals.SystemFactionChanged systemFactionChangedSignal ) 
	{
		_signalDispatcher = signalDispatcher;
		_systemFactionChangedSignal = systemFactionChangedSignal;
	}

	public string PlayerName
	{
		get { return _playerName; }
		set 
		{ 
			_playerName = value;
			if (isClient) CmdSetPlayerName(value);
			if ( NotifyPlayerNameChangeDelegate != null ) NotifyPlayerNameChangeDelegate();
		}
	}

	private void SyncPlayerName( string playerName )
	{
		_playerName = playerName;
		if ( NotifyPlayerNameChangeDelegate != null ) NotifyPlayerNameChangeDelegate();
	}

	void Start () 
	{
		_systemFactionChangedSignal += TransferSystemFaction;

		if (isLocalPlayer)
		{
			PlayerName = _localPlayerManager.PlayerName;
			FactionController factC = gameObject.GetComponent<FactionController>();
			factC.SetFromString(_localPlayerManager.PlayerFactionString);
		}
		
		_signalDispatcher.DispatchPlayerJoined(this);
	}

	// All server commands must come though the player object, so ensure there is an easy to find the local player
	override public void OnStartLocalPlayer()
	{
		_localPlayerManager.LocalPlayer = gameObject;
	}

	public void TransferSystemFaction( GameObject system, string newFaction )
	{
		CmdTransferSystemFaction(system, newFaction);
	}

	[Command]
	public void CmdTransferSystemFaction( GameObject system, string newFaction )
	{
		system.GetComponent<FactionController>().SetFromString(newFaction);
	}

	[Command]
	public void CmdSetPlayerName( string name )
	{
		_playerName = name;
	}

	public void Dispose()
	{
		_systemFactionChangedSignal -= TransferSystemFaction;
		_signalDispatcher.DispatchPlayerDeparted(this);
	}

}
