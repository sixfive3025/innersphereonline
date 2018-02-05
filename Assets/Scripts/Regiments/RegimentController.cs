using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class RegimentController : NetworkBehaviour {

	[SyncVar] public string RegimentName = "";
	[SyncVar] public int BattalionCount = 0;
	[SyncVar] public string VeterencyLevel = "";
	[SyncVar(hook = "LocationChange")] private string _location = "";
	[SyncVar] public string BrigadeMember = "";
	[SyncVar] public string RegimentInsignia = "";
	[SyncVar] public string BrigadeInsignia = "";

	[Inject] SystemRegistry _sysRegistry;
	[Inject] RegimentChit.Factory _chitFactory;

	private RegimentChit _chit = null;

	public void Setup( string regimentName, int battalions, string veterency, string location, 
						string brigade, string regimentInsignia, string brigadeInsignia )
	{
		RegimentName = regimentName;
		BattalionCount = battalions;
		VeterencyLevel = veterency;
		BrigadeMember = brigade;
		RegimentInsignia = regimentInsignia;
		BrigadeInsignia = brigadeInsignia;

		_location = location;		
	}

	void Start()
	{
		LocationChange(_location);
		gameObject.name = RegimentName;
	}

	public void LocationChange( string newLocation )
	{
		if ( _chit != null ) GameObject.Destroy(_chit.gameObject);

		_location = newLocation;
		transform.parent = _sysRegistry.Lookup( _location ).transform;

		_chit = _chitFactory.Create(this);
		_chit.transform.SetParent( transform.parent, false );

		//if (isClient) CmdSetLocation( newLocation );
	}

	/*[Command]
	public void CmdSetLocation( string newLocation )
	{
		LocationChange( newLocation );
	}*/

	public class Factory : Factory<RegimentController> {}
}
