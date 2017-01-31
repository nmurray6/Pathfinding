using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Navigation;

public class PathFindingTester : MonoBehaviour {
	NavigationGrid map;
	public int SkyBoxHeight = 0;
	public int X = 100;
	public int Y = 100;
	public int XOffset = 0;
	public int YOffset = 0;

	// Use this for initialization
	void Start () {
		map = new NavigationGrid (X, Y);

/*		GameObject cube;
		cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cube.transform.position = new Vector3 (0, 0, 0);*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
