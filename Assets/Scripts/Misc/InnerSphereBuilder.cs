using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Zenject;

public class InnerSphereBuilder : NetworkBehaviour {

	private class StarSystem 
	{
		public string Name;
		public float X;
		public float Y;
		public float OrigX;
		public float OrigY;
		public string Faction;

		public bool DataAvailable = false;
		public string StarClass = "";
		public int PlanetsKnown = 0;
		public bool PlanetsUnknown = false;
		public float Gravity = 0f;
		public string Atmosphere = "";
		public int Water = 0;
		public string Climate = "";
		public string Terrain = "";
		public int Development = 0;
		public bool HasFlag = false;

		// For drawing the map borders without rectangles
		public bool IsDummy = false;
	}

	// Will need to instantiate on container so prefab ids matach during network transfer
	[Inject] DiContainer Container; 
	[Inject] readonly Settings _settings;

	// Generation
	private List<Vector2> _mapPoints;
	private List<StarSystem> _mapSystems;

	// Voronoi
	private float _mapWidth = 0;
	private float _mapHeight = 0;
	private Delaunay.Voronoi _voro = null;
	List<uint> _colors = new List<uint> ();
	public void BuildSystems( bool buildOnServer ) 
	{
		TextAsset systemAsset = Resources.Load("stivessystems") as TextAsset;
		string[] systemList = systemAsset.text.Split('\n');

		_mapPoints = new List<Vector2>();
		_mapSystems = new List<StarSystem>();
		float minX = 0, maxX = 0, minY = 0, maxY = 0;

		for ( int sys = 0; sys < systemList.Length; sys++ )
		{
			string[] systemLine = systemList[sys].Split(',');
			string systemName = systemLine[0];
			float systemX;
			float systemY;
			string faction;

			// Skip badly formatted systems until we clean it all up
			try
			{
				systemX = (Convert.ToSingle(systemLine[1])) * _settings.CoordinateMultiplier;
				systemY = (Convert.ToSingle(systemLine[2])) * _settings.CoordinateMultiplier;
				faction = systemLine[3];
			}
			catch ( FormatException ex ) 
			{ 
				Debug.LogError("Could not load " + systemName + " (" + ex.Message + ")" );
				continue;
			}
			catch ( IndexOutOfRangeException ex )
			{
				Debug.Log("WARNING: Found an incomplete line. Not necessarily a problem." + " (" + ex.Message + ")" );
				continue;
			}

			// Voronoi Build
			_colors.Add (0);
			_mapPoints.Add (new Vector2 (systemX, systemY));
			if (systemX < minX) minX = systemX;
			if (systemX > maxX) maxX = systemX;
			if (systemY < minY) minY = systemY;
			if (systemY > maxY) maxY = systemY;

			// Save info to use after the Voronoi
			StarSystem newSS = new StarSystem();
			newSS.Name = systemName;
			newSS.Faction = faction;
			newSS.X = systemX;
			newSS.Y = systemY;
			newSS.OrigX = (Convert.ToSingle(systemLine[1]));
			newSS.OrigY = (Convert.ToSingle(systemLine[2]));

			if ( systemName.StartsWith( "zz" ) )
			{
				newSS.IsDummy = true;
			}
			else if ( systemLine[4] != "" )
			{
				try
				{		
					newSS.StarClass = systemLine[4];
					if ( systemLine[5].StartsWith("x") ) 
					{ 
						if ( systemLine[5] == "x" )
							newSS.PlanetsKnown = 0;
						else newSS.PlanetsKnown = Convert.ToInt32(systemLine[5].Substring(1));
						newSS.PlanetsUnknown = true;
					}
					else newSS.PlanetsKnown = Convert.ToInt32(systemLine[5]);
					if ( systemLine[6] != "x" ) newSS.Gravity = Convert.ToSingle(systemLine[6]);
					newSS.Atmosphere = systemLine[7];
					if ( systemLine[8] != "x" ) newSS.Water = Convert.ToInt32(systemLine[8].TrimEnd( new char[] { '%' } ));
					newSS.Climate = systemLine[9];
					newSS.Terrain = systemLine[10];
					if ( systemLine[11] != "x" ) newSS.Development = Convert.ToInt32(systemLine[11]);
					newSS.HasFlag = (systemLine[12] == "flag");

					// Survived the import - mark the data usable
					newSS.DataAvailable = true;
				}
				catch (FormatException ex) { Debug.Log("WARNING: Failed to add extra data for " + systemName + " (" + ex.Message + ")" ); }
			}

			if ( newSS.IsDummy ) newSS.StarClass = "dummy";
			else if ( newSS.StarClass == "" || newSS.StarClass == "hx" || newSS.StarClass == "cluster" )
			{
				// If StarClass wasn't set, let's generate one
				// TODO: Read values from settings somewhere
				// HM 10%, HK 35%, HG 35%, HF 10%, HA 10%
				int roll = UnityEngine.Random.Range(1,101);
				if (roll <= 10) newSS.StarClass = "hm";
				if (roll > 10 && roll <=45 ) newSS.StarClass = "hk";
				if (roll > 45 && roll <=80 ) newSS.StarClass = "hg";
				if (roll > 80 && roll <=90 ) newSS.StarClass = "hf";
				if (roll > 90 && roll <=100 ) newSS.StarClass = "hf";
			}

			_mapSystems.Add( newSS );
		}

		// Generate the map regions using Voronoi algorithm
		_mapWidth = maxX-minX;
		_mapHeight = maxY-minY;
		_voro = new Delaunay.Voronoi (_mapPoints, _colors, new Rect (minX, minY, _mapWidth, _mapHeight));

		for( int t = 0; t < _mapPoints.Count; t++ )
		{
			if ( !_settings.ShowDummySystems && _mapSystems[t].IsDummy ) continue; // Skip dummy systems

			List<Vector2> li = _voro.Region(_mapPoints[t]);
			createRegion( li, _mapSystems[t], buildOnServer );
		}
	}

	private void createRegion( List<Vector2> regionPoints, StarSystem target, bool buildOnServer )
	{
		GameObject createdSystem = Container.InstantiatePrefab(_settings.StarSystemPrefab);
		createdSystem.transform.position = new Vector3( target.X, 0, target.Y );
		createdSystem.name = "Star_" + target.Name;

		// Might be building on the client in offline mode
		if ( buildOnServer ) NetworkServer.Spawn(createdSystem.gameObject);
		
		createdSystem.GetComponentsInChildren<Text>()[0].text = target.Name;

		TerritoryController tc = createdSystem.GetComponent<TerritoryController>();
		foreach ( Vector2 point in regionPoints )
			tc.Points.Add(point);

		StarSystemController ssCon = createdSystem.gameObject.GetComponent<StarSystemController>();
		ssCon.StarName = target.Name;

		// Set the starting faction
		FactionController factCtrl = createdSystem.gameObject.GetComponent<FactionController>();
		switch (target.Faction)
		{
			case "s":
				factCtrl.CurrentFaction = Faction.Steiner;
				break;
			case "m":
				factCtrl.CurrentFaction = Faction.Marik;
				break;
			case "l":
				factCtrl.CurrentFaction = Faction.Liao;
				break;
			case "d":
				factCtrl.CurrentFaction = Faction.Davion;
				break;
			case "k":
				factCtrl.CurrentFaction = Faction.Kurita;
				break;
			case "c":
				factCtrl.CurrentFaction = Faction.Comstar;
				break;
			default:
				factCtrl.CurrentFaction = Faction.None;
				break;
		}

		StarSystemData sysData = createdSystem.gameObject.GetComponent<StarSystemData>();
		sysData.OrigX = target.OrigX;
		sysData.OrigY = target.OrigY;
		sysData.StarClass = target.StarClass;

		if (target.DataAvailable)
		{
			sysData.IsDataKnown = true;
			sysData.PlanetsKnown = target.PlanetsKnown;
			sysData.PlanetsUnknown = target.PlanetsUnknown;
			sysData.Gravity = target.Gravity;
			sysData.Atmosphere = target.Atmosphere;
			sysData.Water = target.Water;
			sysData.Climate = target.Climate;
			sysData.Terrain = target.Terrain;
			sysData.Development = target.Development;
			sysData.HasFlag = target.HasFlag;
		}
	}

	[Serializable]
	public class Settings
	{
		public GameObject StarSystemPrefab;
		public bool ShowDummySystems;
		public int CoordinateMultiplier;
	}
}
