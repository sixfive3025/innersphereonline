using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Zenject;

// Class based off: https://gist.github.com/green-coder/197b8d63adffbbb87aaa (Thank you, green-coder!)
public class ISONetworkManager : NetworkManager, IInitializable, IDisposable {

	[Inject] readonly DiContainer _container;
	[Inject] readonly Settings _settings;
	[Inject] readonly SignalDispatcher _signalDispatcher;

	// Used by our 'Zenject' custom spawn instantiation.
	private Dictionary<NetworkHash128, GameObject> _assetIdToPrefab = new Dictionary<NetworkHash128, GameObject>();
	private Dictionary<GameObject, IDisposable[]> _disposableSpawns = new Dictionary<GameObject, IDisposable[]>();

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
		_assetIdToPrefab[playerPrefab.GetComponent<NetworkIdentity>().assetId] = playerPrefab;

		spawnPrefabs.Add(_settings.StarSystemPrefab);
		spawnPrefabs.Add(_settings.RegimentPrefab);

		foreach (GameObject prefab in spawnPrefabs) {
			_assetIdToPrefab[prefab.GetComponent<NetworkIdentity>().assetId] = prefab;
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
		GameObject player = _container.InstantiatePrefab(_settings.PlayerPrefab);
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

	// Cleanup all IDisposables that we created during spawning and never explicitly unspawned
	public void Dispose()
	{
		foreach ( KeyValuePair<GameObject, IDisposable[]> disposables in _disposableSpawns )
		{
			for ( int x = 0 ; x < disposables.Value.Length; x++ )
			{
				disposables.Value[x].Dispose();
			}
		}
	}

	private void RegisterCustomSpawners() {
		foreach (NetworkHash128 assetId in _assetIdToPrefab.Keys) {
			ClientScene.RegisterSpawnHandler(assetId, Spawn, UnSpawn);
		}
	}

	private void UnregisterCustomSpawners() {
		foreach (NetworkHash128 assetId in _assetIdToPrefab.Keys) {
			ClientScene.UnregisterSpawnHandler(assetId);
		}
	}

	// On the client, instantiate the prefab using Zenject's container.
	private GameObject Spawn(Vector3 position, NetworkHash128 assetId) {
		GameObject spawnedGameObject = _container.InstantiatePrefab(_assetIdToPrefab[assetId]);
		
		IDisposable[] disposables = spawnedGameObject.GetComponentsInChildren<IDisposable>();
		if ( disposables != null ) _disposableSpawns.Add(spawnedGameObject, disposables);

		return spawnedGameObject;
	}

	// On the client, destroy the game object.
	// Make sure all IDisposable operations run on the GameObject first
	private void UnSpawn(GameObject spawned) {
		IDisposable[] disposables = _disposableSpawns[spawned];
		if (disposables != null)
		{
			for ( int x = 0 ; x < disposables.Length; x++ )
			{
				disposables[x].Dispose();
			}
			_disposableSpawns.Remove(spawned);
		}

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
		public GameObject RegimentPrefab;
	}
}
