using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Zenject;

// Class based off: https://gist.github.com/green-coder/197b8d63adffbbb87aaa (Thank you, green-coder!)
public class ISONetworkManager : NetworkManager, IInitializable {

	[Inject] DiContainer Container;
	[Inject] readonly Settings _settings;
	[Inject] readonly SignalDispatcher _signalDispatcher;

	// Used by our 'Zenject' custom spawn instantiation.
	Dictionary<NetworkHash128, GameObject> assetIdToPrefab = new Dictionary<NetworkHash128, GameObject>();

	public delegate void ClientCallback();
	public ClientCallback ConnectDelegate = null;

	public void Initialize() {
		logLevel = LogFilter.FilterLevel.Info;
		networkAddress = _settings.ServerHostName;
		networkPort = _settings.ServerPort;
		serverBindToIP = true;
		serverBindAddress = _settings.ServerBindIP;
		autoCreatePlayer = false;
		
		channels.Add(QosType.ReliableSequenced);
		channels.Add(QosType.Unreliable);
		customConfig = true;

		// Preparation for registering our own spawn handlers.
		playerPrefab = _settings.PlayerPrefab;
		assetIdToPrefab[playerPrefab.GetComponent<NetworkIdentity>().assetId] = playerPrefab;

		spawnPrefabs.Add(_settings.StarSystemPrefab);

		foreach (GameObject prefab in spawnPrefabs) {
			assetIdToPrefab[prefab.GetComponent<NetworkIdentity>().assetId] = prefab;
		}

		// Do not let the NetworkManager register the playerPrefab on the client, as the prefab instantiation
		// has a higher priority compared to the custom spawn handler instantiation method.
		//_playerPrefab = playerPrefab;
		playerPrefab = null;

		// The same, we don't want the NetworkManager to register those prefabs as it would hide our
		// custom spawn handler instantiation method.
		spawnPrefabs.Clear();
	}

	public bool CreateISOPlayer()
	{
		return ClientScene.AddPlayer(client.connection,0);
	}

	public override void OnServerConnect(NetworkConnection connection)
	{
		base.OnServerConnect(connection);
		SetChannelBuffers(connection);
		Debug.Log( "*** RECEIVED INCOMMING CONNECTION ***");
	}

	public override void OnServerError(NetworkConnection connection, int errorCode)
	{
		base.OnServerError(connection, errorCode);
		Debug.Log( "*** SERVER ERROR OCCURED ***" );
		Debug.Log( connection.lastError );
	}

	// On the server, we create the player object using Zenject's container.
	public override void OnServerAddPlayer(NetworkConnection connection, short playerControllerId) {
		GameObject player = Container.InstantiatePrefab(_settings.PlayerPrefab);
		NetworkServer.AddPlayerForConnection(connection, player, playerControllerId);
	}

	public override void OnStartClient(NetworkClient client) {
		base.OnStartClient(client);
		RegisterCustomSpawners();
	}

	public override void OnClientConnect(NetworkConnection connection) {
		base.OnClientConnect(connection);
		SetChannelBuffers(connection);
		if( ConnectDelegate != null ) ConnectDelegate();
	}

	public override void OnClientDisconnect(NetworkConnection connection) 
	{
		base.OnClientDisconnect(connection);
		// No explicit player action allows disconnection, so assume it's an error
		_signalDispatcher.DispatchFatalError("Disconnected from Server");
	}

	public override void OnStopClient() {
		base.OnStopClient();
		UnregisterCustomSpawners();
	}

	private void RegisterCustomSpawners() {
		foreach (NetworkHash128 assetId in assetIdToPrefab.Keys) {
			ClientScene.RegisterSpawnHandler(assetId, Spawn, UnSpawn);
		}
	}

	private void UnregisterCustomSpawners() {
		foreach (NetworkHash128 assetId in assetIdToPrefab.Keys) {
			ClientScene.UnregisterSpawnHandler(assetId);
		}
	}

	// On the client, instantiate the prefab using Zenject's container.
	private GameObject Spawn(Vector3 position, NetworkHash128 assetId) {
		return Container.InstantiatePrefab(assetIdToPrefab[assetId]);
	}

	// On the client, destroy the game object.
	private void UnSpawn(GameObject spawned) {
		Destroy(spawned.gameObject);
	}
	
	private void SetChannelBuffers( NetworkConnection conn )
	{
		for ( int channel = 0; channel < channels.Count; channel++)
		{
			conn.SetChannelOption(channel,ChannelOption.MaxPendingBuffers,256);
		}
	}

	[Serializable]
	public class Settings
	{
		public string ServerHostName;
		public int ServerPort;
		public string ServerBindIP;
		public GameObject PlayerPrefab;
		public GameObject StarSystemPrefab;
	}
}
