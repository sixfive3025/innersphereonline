using UnityEngine;
using UnityEngine.UI;

public class SetSystemName : MonoBehaviour {

	void Start () {
		GetComponent<Text>().text = transform.parent.parent.GetComponent<StarSystemController>().StarName;
	}
	
}
