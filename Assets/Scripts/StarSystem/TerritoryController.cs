using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Zenject;

public class VectorSync : SyncListStruct<Vector2> { }

public class TerritoryController : NetworkBehaviour {

	public GameObject TerritoryAttached = null;
	private bool _createdTerritory = false;
	private Material _material = null;
	private Material _materialSelected = null;

	public VectorSync Points = new VectorSync();

	//private Splinulator _splinulator = new Splinulator();
	
	void Update () 
	{
		// Wait till the territory data is ready then create it
		// TODO: Get this out of the Update() function
		if (!_createdTerritory && Points.Count > 0)
		{
			StarSystemController starCtrl = GetComponent<StarSystemController>();

			Vector2[] newPoints = new Vector2[Points.Count];
			Points.CopyTo(newPoints, 0);

			// Pull the network synchronized points
			// Vector2[] earlyPoints = new Vector2[Points.Count];
			// Points.CopyTo(earlyPoints, 0);

			/*
			// Smooth those points out a bit
			List<Vector2> latePoints = new List<Vector2>();
			Vector2 lastPoint = earlyPoints[1];
			for ( int i = 0; i < earlyPoints.Length; i++ )
			{
				Vector2[] calcPoints = new Vector2[4];
				
				if ( i == 0 ) calcPoints[0] = earlyPoints[earlyPoints.Length-1];
				else calcPoints[0] = earlyPoints[i-1];

				calcPoints[1] = earlyPoints[i];

				if ( (i+1) > (earlyPoints.Length-1) ) calcPoints[2] = earlyPoints[0];
				else calcPoints[2] = earlyPoints[i+1];

				if ( (i+2) > earlyPoints.Length ) calcPoints[3] = earlyPoints[1];
				else if ( (i+2) > (earlyPoints.Length-1) ) calcPoints[3] = earlyPoints[0];
				else calcPoints[3] = earlyPoints[i+2];

				List<Vector2> smoothPoints = _splinulator.CatmulRom( calcPoints );
				lastPoint = smoothPoints[smoothPoints.Count-1];
				smoothPoints.Remove(smoothPoints[smoothPoints.Count-1]); // remove duplicate first/last point
				latePoints.AddRange( smoothPoints );
			}
			*/

			// Vector2[] newPoints = new Vector2[latePoints.Count];
			// latePoints.CopyTo(newPoints, 0);

			// Make triangles out of all those points
			Triangulator tr = new Triangulator(newPoints);
			int[] indices = tr.Triangulate();
	
			// Create the Vector3 vertices
			Vector3[] vertices = new Vector3[newPoints.Length];
			for (int i=0; i<vertices.Length; i++) {
				vertices[i] = new Vector3(newPoints[i].x, -3, newPoints[i].y);
			}
	
			// Create the mesh
			Mesh msh = new Mesh();
			msh.vertices = vertices;
			msh.triangles = indices;
			msh.RecalculateNormals();
			msh.RecalculateBounds();
	
			// Set up game object with mesh;
			TerritoryAttached = new GameObject();
			TerritoryAttached.name = starCtrl.StarName + "_Territory";
			TerritoryAttached.AddComponent<MeshRenderer>();
			MeshFilter filter = TerritoryAttached.AddComponent<MeshFilter>();
			filter.mesh = msh;

			// Keep the scene hierarchy clean
			GameObject mgrParent = GameObject.Find("TerritoryList");
			TerritoryAttached.transform.parent = mgrParent.transform;

			// Too hacky: making sure the material gets assigned
			SendMessage( "OnFactionChange" );
			_createdTerritory = true;
		}
	}

	public void ChangeFaction( Faction newFaction )
	{
		if ( TerritoryAttached != null )
		{
			string materialName = "";
			Renderer rend = TerritoryAttached.GetComponent<Renderer>();

			switch (newFaction)
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

			_material = Resources.Load(materialName, typeof(Material)) as Material;
			_materialSelected = Resources.Load(materialName + "Selected", typeof(Material)) as Material;
			rend.material = _material;
			if ( !rend.enabled ) rend.enabled = true;
		}
	}

	public void SystemSelected()
	{
		TerritoryAttached.GetComponent<Renderer>().material = _materialSelected;
	}

	public void SystemDeselected()
	{
		TerritoryAttached.GetComponent<Renderer>().material = _material;
	}

}
