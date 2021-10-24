using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour {
	[SerializeField] Transform target;

	void Update() {
		if(target) {
			this.transform.position = target.transform.position+Vector3.back*10f;
		}
	}
}
