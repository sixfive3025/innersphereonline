using UnityEngine;
using UnityEngine.Networking;

public enum Faction { None, Davion, Kurita, Liao, Marik, Steiner, Comstar }; 

public class FactionController : NetworkBehaviour {
	
	[SyncVar(hook="SyncFactionChange")] 
	public Faction _faction = Faction.None;

	public delegate void NotifyFactionChanged();
	public NotifyFactionChanged NotifyFactionChangedDelegate = null;

	public Faction CurrentFaction
	{
		get { return _faction; }
		set 
		{ 
			_faction = value;
			if (isClient) CmdSetFaction(value);
			if ( NotifyFactionChangedDelegate != null ) NotifyFactionChangedDelegate();
		}
	}

	private void SyncFactionChange( Faction newFaction )
	{
		_faction = newFaction;
		if ( NotifyFactionChangedDelegate != null ) NotifyFactionChangedDelegate();
	}

	public Color GetFactionColor()
	{
		switch (_faction)
		{
			case Faction.Davion:
				return new Color(0.878f,0.89f,0f);
			case Faction.Kurita:
				return new Color(1f,0f,0f);
			case Faction.Liao:
				return new Color(0.067f,0.836f,0f);
			case Faction.Marik:
				return new Color(1f,0f,0.859f);
			case Faction.Steiner:
				return new Color(0f,0.537f,0.859f);
			case Faction.Comstar:
				return Color.white;
			default:
				return Color.cyan;
		}
	}

	public Faction SetFromString( string factionString )
	{
		switch (factionString)
		{
			case "liao":
				CurrentFaction = Faction.Liao;
				break;
			case "steiner":
				CurrentFaction = Faction.Steiner;
				break;
			case "kurita":
				CurrentFaction = Faction.Kurita;
				break;
			case "marik":
				CurrentFaction = Faction.Marik;
				break;
			case "davion":
				CurrentFaction = Faction.Davion;
				break;
			case "comstar":
				CurrentFaction = Faction.Comstar;
				break;
		}

		return _faction;
	}

	[Command]
	public void CmdSetFaction( Faction f )
	{
		_faction = f;
	}

}
