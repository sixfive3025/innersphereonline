using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class StarSystemController : NetworkBehaviour {

	[SyncVar(hook = "OnChangeStarName")] public string StarName = "";

	[Inject] Settings _settings;

	// TODO: Seems like a bad dependency
	[Inject] GameController _gameContoller;

	[Inject] SystemRegistry _sysRegistry;

	public void Start()
	{
		// Keep the scene hierarchy clean
		transform.parent = GameObject.Find("SystemList").transform;
		GetComponent<FactionController>().NotifyFactionChangedDelegate += OnFactionChange;

		if ( StarName != "" ) OnChangeStarName( StarName );
	}

	void OnChangeStarName( string newStarName )
	{
		StarName = newStarName;
		_sysRegistry.Register(StarName, gameObject);
		gameObject.name = StarName;
	}

	public void LoadSystemPrefab()
	{
		// Server CPU Usage Optimization
		if ( _gameContoller.IsHeadless() ) return;

		string newStarClass = GetComponent<StarSystemData>().StarClass;
		Transform newStarPrefab = null;

		switch ( newStarClass )
		{
			case "ha":
				newStarPrefab = _settings.Prefab_ha;
				break;
			case "hb":
				newStarPrefab = _settings.Prefab_hb;
				break;
			case "hf":
				newStarPrefab = _settings.Prefab_hf;
				break;
			case "hg":
				newStarPrefab = _settings.Prefab_hg;
				break;
			case "hk":
				newStarPrefab = _settings.Prefab_hk;
				break;
			case "hm":
				newStarPrefab = _settings.Prefab_hm;
				break;
			case "ho":
				newStarPrefab = _settings.Prefab_ho;
				break;
			case "dummy":
				newStarPrefab = _settings.Prefab_sunknown;
				break;
			default:
				newStarPrefab = _settings.Prefab_sunknown;
				Debug.LogError("Missing a star class for: " + StarName );
				break;
		}

		// Haven't switched to DI because these are purely aesthetic
		Instantiate(newStarPrefab, transform.position, Quaternion.identity, transform);
	}

	// Set the correct faction material when it changes ownership
	public void OnFactionChange () 
	{
		GetComponent<TerritoryController>().ChangeFaction(GetComponent<FactionController>().CurrentFaction);
	}
	
	[Serializable]
	public class Settings
	{
		public Transform Prefab_ha;
		public Transform Prefab_hb;
		public Transform Prefab_hf;
		public Transform Prefab_hg;
		public Transform Prefab_hk;
		public Transform Prefab_hm;
		public Transform Prefab_ho;
		public Transform Prefab_sunknown;
	}
}
