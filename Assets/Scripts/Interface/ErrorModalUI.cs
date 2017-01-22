using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ErrorModalUI : MonoBehaviour {

	[Inject]
	public void Construct( string errorMessage )
	{
		GetComponentInChildren<Text>().text = errorMessage;
		
	}
	public class Factory : Factory<string, ErrorModalUI> {}
}
