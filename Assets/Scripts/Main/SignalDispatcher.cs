using UnityEngine;
using Zenject;

public static class Signals {

	public class FactionSelected : Signal<FactionSelected,string,string> {}
	public class SystemFactionChanged : Signal<SystemFactionChanged,GameObject,string> {}
	public class RegimentMoved : Signal<RegimentMoved,GameObject,string> {}
	public class PlayerJoined : Signal<PlayerJoined,PlayerController> {}
	public class PlayerDeparted : Signal<PlayerDeparted,PlayerController> {}
	public class FatalError : Signal<FatalError,string> {}
}

public class SignalDispatcher {

	private Signals.FactionSelected _factionSelectedSignal;
	private Signals.SystemFactionChanged _systemFactionChangedSignal;
	private Signals.RegimentMoved _regimentMovedSignal;
	private Signals.PlayerJoined _playerJoinedSignal;
	private Signals.PlayerDeparted _playerDepartedSignal;
	private Signals.FatalError _fatalErrorSignal;

	public SignalDispatcher(Signals.FactionSelected factionSelectedSignal,
							Signals.SystemFactionChanged systemFactionChangedSignal,
							Signals.RegimentMoved regimentMovedSignal,
							Signals.PlayerJoined playerJoinedSignal,
							Signals.PlayerDeparted playerDepartedSignal,
							Signals.FatalError fatalErrorSignal) 
	{
		_factionSelectedSignal = factionSelectedSignal;
		_systemFactionChangedSignal = systemFactionChangedSignal;
		_regimentMovedSignal = regimentMovedSignal;
		_playerJoinedSignal = playerJoinedSignal;
		_playerDepartedSignal = playerDepartedSignal;
		_fatalErrorSignal = fatalErrorSignal;
	}

	public void DispatchFactionSelected( string chosenFaction, string playerName )
	{
		_factionSelectedSignal.Fire(chosenFaction, playerName);
	}

	public void DispatchSystemFactionChanged( StarSystemController system, string newFaction )
	{
		_systemFactionChangedSignal.Fire(system.gameObject, newFaction);
	}

	public void DispatchRegimentMoved( RegimentController regiment, string newLocation )
	{
		_regimentMovedSignal.Fire(regiment.gameObject, newLocation);
	}

	public void DispatchPlayerJoined( PlayerController player )
	{
		_playerJoinedSignal.Fire(player);
	}

	public void DispatchPlayerDeparted( PlayerController player )
	{
		_playerDepartedSignal.Fire(player);
	}

	public void DispatchFatalError( string fatalError )
	{
		_fatalErrorSignal.Fire(fatalError);
	}
}
