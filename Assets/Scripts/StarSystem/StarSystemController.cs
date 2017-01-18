using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class StarSystemController : NetworkBehaviour {

	[SyncVar] public string StarName;

	[Inject] Settings _settings;

	// TODO: Seems like a bad dependency
	[Inject] GameController _gameContoller;

	public void Start()
	{
		// Keep the scene hierarchy clean
		transform.parent = GameObject.Find("SystemList").transform;
		GetComponent<FactionController>().NotifyFactionChangedDelegate += OnFactionChange;
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
		GameObject territory = GetComponent<TerritoryController>().TerritoryAttached;

		if ( territory != null )
		{
			string materialName = "";
			FactionController factCtrl = GetComponent<FactionController>();
			Renderer rend = territory.GetComponent<Renderer>();

			switch (factCtrl.CurrentFaction)
			{
				case Faction.Steiner:
					materialName = "Materials/SteinerRegion";
					break;
				case Faction.Marik:
					materialName = "Materials/MarikRegion";
					break;
				case Faction.Liao:
					materialName = "Materials/LiaoRegion";
					break;
				case Faction.Davion:
					materialName = "Materials/DavionRegion";
					break;
				case Faction.Kurita:
					materialName = "Materials/KuritaRegion";
					break;
				case Faction.Comstar:
					materialName = "Materials/ComstarRegion";
					break;
				default:
					rend.enabled = false;
					return;
			}

			rend.material = Resources.Load(materialName, typeof(Material)) as Material;
			if ( !rend.enabled ) rend.enabled = true;
		}
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
