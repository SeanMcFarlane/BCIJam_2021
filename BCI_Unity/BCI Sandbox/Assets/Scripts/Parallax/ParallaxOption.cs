/*
 THANKS TO DAVID DION-PAQUET AND HIS BLOG POST!
 http://www.gamasutra.com/blogs/DavidDionPaquet/20140601/218766/Creating_a_parallax_system_in_Unity3D_is_harder_than_it_seems.php
*/

using UnityEngine;
using System.Collections;

public class ParallaxOption : MonoBehaviour {

	public bool moveParallax;

	[SerializeField]
	[HideInInspector]
	private Vector3 storedPosition;

	public void SavePosition() {
		storedPosition = transform.position;
	}

	public void RestorePosition() {
		transform.position = storedPosition;
	}
}