using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FactionPickerUI : MonoBehaviour {

	private Signals.FactionSelected _factionSelectedSignal;
	private Button[] _factionButtons;

	[Inject]
	public void Construct(Signals.FactionSelected factionSelectedSignal) 
	{
		_factionSelectedSignal = factionSelectedSignal;

		_factionButtons = GetComponentsInChildren<Button>();
		foreach ( Button btn in _factionButtons )
		{
			btn.onClick.AddListener( () => ChooseFaction(btn.gameObject.name));
		}
	}

	public void ChooseFaction( string factionChosen )
	{
		_factionSelectedSignal.Fire(factionChosen);
	}

	public class Factory : Factory<FactionPickerUI> {}
}
