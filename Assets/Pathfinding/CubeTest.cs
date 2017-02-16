using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshFilter me = GetComponent<MeshFilter> ();
		Mesh mesh = me.mesh;
		Vector3[] vert = mesh.vertices;
		int[] tri = mesh.triangles;
		Vector3[] norm = mesh.normals;

		Debug.Log ("Vertices " + vert.Length);
		Debug.Log ("Triangles " + tri.Length);
		Debug.Log ("Normals " + norm.Length);

		int n = 0;
		for (n = 0; n < vert.Length; n++) {
			Debug.Log (vert [n]);
		}

		//int i = 0;
		//while (i < norm.Length) {
		//	if (i % 3 == 0)
		//		Debug.Log ("NEW");
		//	Debug.Log (norm [i]);
		//	i++;
		//}
		Debug.Log ("End");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
