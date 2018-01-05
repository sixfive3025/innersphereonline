using UnityEngine;
using UnityEngine.Networking;

public class VectorSync : SyncListStruct<Vector2> { }

public class TerritoryController : NetworkBehaviour {

	public GameObject TerritoryAttached = null;
	private bool _createdTerritory = false;
	private Material _material = null;
	private Material _materialSelected = null;

	public VectorSync Points = new VectorSync();
	
	void Update () 
	{
		// Wait till the territory data is ready then create it
		// TODO: Get this out of the Update() function
		if (!_createdTerritory && Points.Count > 0)
		{
			StarSystemController starCtrl = GetComponent<StarSystemController>();

			Vector2[] newPoints = new Vector2[Points.Count];
			Points.CopyTo(newPoints, 0);
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
