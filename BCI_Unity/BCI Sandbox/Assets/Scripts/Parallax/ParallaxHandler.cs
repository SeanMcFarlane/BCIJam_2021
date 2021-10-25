using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ParallaxHandler : MonoBehaviour {
	[SerializeField] float distributionCurveM = 0.25f;
	[SerializeField] private bool foregroundMode = false;
	int updateEvery100Frames = 0;
	public float universalScalar = 2f;

	// Use this for initialization
	void ParallaxUpdateDistribute() 
	{
		int children = transform.childCount;
		for(int i = 0; i<children; ++i)
		{
			//print("i:"+i);
			SortingGroup mySortingGroup;
			GameObject myGameObject = transform.GetChild(i).gameObject;
			ParallaxLayer para = myGameObject.GetComponent<ParallaxLayer>();
			float multiplier = ((float)i+1)/children;
			//print("Multiplier = "+multiplier);
			para.distanceKM = (100f*multiplier*multiplier*multiplier);
			para.universalScalar = universalScalar;
			if(foregroundMode) // If parallaxhandler is for foreground objects, invert direction of displacement.
			{
				if(myGameObject.GetComponent<SortingGroup>())
				{
					mySortingGroup = myGameObject.GetComponent<SortingGroup>();
					mySortingGroup.sortingOrder = 100+(((i+1)*2)+1);
				}
				else
				{
					mySortingGroup = myGameObject.AddComponent<SortingGroup>();
					mySortingGroup.sortingOrder = 100+(((i+1)*2)+1);
				}
				mySortingGroup.sortingLayerName = "Foreground";
			}
			else // if not in foreground mode, make each layer deeper in the background instead.
			{
				if(myGameObject.GetComponent<SortingGroup>())
				{
					mySortingGroup = myGameObject.GetComponent<SortingGroup>();
					mySortingGroup.sortingOrder = -100-(((i+1)*2)+1);
				}
				else
				{
					mySortingGroup = myGameObject.AddComponent<SortingGroup>();
					mySortingGroup.sortingOrder = -100-(((i+1)*2)+1);
				}
				mySortingGroup.sortingLayerName = "Background";
			}
		}
	}

	// Use this for initialization
	void ParallaxUpdateFill() 
	{
		int i = 1;
		foreach(Transform t in transform)
		{
			SortingGroup mySortingGroup;
			GameObject myGameObject = t.gameObject;
			ParallaxLayer para = myGameObject.GetComponent<ParallaxLayer>();
			//para.distanceKM = 10*Mathf.Log10(0.25f*(Mathf.Pow(2, i)));
			para.distanceKM =  i*(i-1)*distributionCurveM;

			if(myGameObject.GetComponent<SortingGroup>())
			{
				mySortingGroup = myGameObject.GetComponent<SortingGroup>();
				mySortingGroup.sortingOrder = -100-(((i+1)*2)+1);
			}
			else
			{
				mySortingGroup = myGameObject.AddComponent<SortingGroup>();
				mySortingGroup.sortingOrder = -100-(((i+1)*2)+1);
			}
				
			mySortingGroup.sortingLayerName = "Background";

			i++;
		}	
	}

	
	// Update is called once per frame
	void Update() 
	{
		if(updateEvery100Frames>=100)
		{
			ParallaxUpdateDistribute();
			updateEvery100Frames = 0;
		}
		else
		{
			updateEvery100Frames++;
		}
	}
}
