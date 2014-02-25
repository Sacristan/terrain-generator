using UnityEngine;
using System.Collections;

public class Re : MonoBehaviour {
	
	float height;
	
	void Start () {
		height = transform.position.y;
	}
	
	void Update (){
		if(Input.GetKeyDown(KeyCode.G)){
			Vector3 tmpVec = transform.position;
			tmpVec.y = height;
			transform.position = tmpVec;
		}
	}
	
}
