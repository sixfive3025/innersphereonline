using UnityEngine.Networking;

public class StarSystemData : NetworkBehaviour {

	[SyncVar] public bool IsDataKnown = false;

	[SyncVar] public string StarClass = "";
	[SyncVar] public int PlanetsKnown = 0;
	[SyncVar] public bool PlanetsUnknown = false;
	[SyncVar] public float Gravity = 0f;
	[SyncVar] public string Atmosphere = "";
	[SyncVar] public int Water = 0;
	[SyncVar] public string Climate = "";
	[SyncVar] public string Terrain = "";
	[SyncVar] public int Development = 0;
	[SyncVar] public bool HasFlag = false;

	[SyncVar] public float OrigX = 0;
	[SyncVar] public float OrigY = 0;

	void Start()
	{
		// When we've got the network data, trigger the star class drawing
		// TODO: Logic shouldn't be here, but I haven't found a way to know when data is ready from outside
		GetComponent<StarSystemController>().LoadSystemPrefab();
	}
}
