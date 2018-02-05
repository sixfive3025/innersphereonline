using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public enum Faction { None, Davion, Kurita, Liao, Marik, Steiner, Comstar }; 

public class FactionController : NetworkBehaviour {
	
	[SyncVar(hook="SyncFactionChange")] 
	public Faction _faction = Faction.None;
	[Inject] Settings _settings;

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
				return _settings.DavionHouse;
			case Faction.Kurita:
				return _settings.KuritaHouse;
			case Faction.Liao:
				return _settings.LiaoHouse;
			case Faction.Marik:
				return _settings.MarikHouse;
			case Faction.Steiner:
				return _settings.SteinerHouse;
			case Faction.Comstar:
				return _settings.ComstarHouse;
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

	[Serializable]
	public class Settings
	{
		public Color DavionHouse;
		public Color DavionMerc;
		public Color KuritaHouse;
		public Color KuritaMerc;
		public Color LiaoHouse;
		public Color LiaoMerc;
		public Color MarikHouse;
		public Color MarikMerc;
		public Color SteinerHouse;
		public Color SteinerMerc;
		public Color ComstarHouse;
		public Color ComstarMerc;
	}
}
