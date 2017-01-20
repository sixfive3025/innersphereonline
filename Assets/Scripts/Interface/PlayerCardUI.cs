using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerCardUI : MonoBehaviour {

	private PlayerController _targetPlayer;

	[Inject]
	public void Construct( PlayerController targetPlayer )
	{
		_targetPlayer = targetPlayer;
		_targetPlayer.GetComponent<FactionController>().NotifyFactionChangedDelegate += UpdateFactionColor;
		_targetPlayer.NotifyPlayerNameChangeDelegate += UpdatePlayerName;
	}
	
	public void Start()
	{
		UpdateFactionColor();
		GetComponentInChildren<Text>().text = _targetPlayer.PlayerName;
	}

	public void UpdatePlayerName()
	{
		GetComponentInChildren<Text>().text = _targetPlayer.PlayerName;
	}

	public void UpdateFactionColor()
	{
		FactionController factionController = _targetPlayer.GetComponent<FactionController>();
		Image img = GetComponent<Image>();
		img.color = factionController.GetFactionColor();
	}

	public class Factory : Factory<PlayerController,PlayerCardUI> {}
}
