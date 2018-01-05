using Zenject;

public class SystemSelection : Interaction {

	private UIManager _uiManager;

	[Inject]
	public void Construct( UIManager uiManager )
	{
		_uiManager = uiManager;
	}

	public override void Deselect()
	{
		_uiManager.SystemDeselected(gameObject.GetComponent<StarSystemController>());
		gameObject.GetComponent<TerritoryController>().SystemDeselected();
	}

	public override void Select()
	{
		_uiManager.SystemSelected(gameObject.GetComponent<StarSystemController>());
		gameObject.GetComponent<TerritoryController>().SystemSelected();
	}
	
}
