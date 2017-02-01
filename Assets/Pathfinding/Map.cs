using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Navigation;

public class Map : MonoBehaviour{
	int X;
	int Y;

	NavigationGrid grid;
	BlockNode[,,] MapData;

	public int maxDepth = 10;

	Vector3[] Vertices;
	Vector2[] UV;
	int[] Triangles;

	void Start(){
		//mesh = GetComponent<MeshFilter> ().mesh;
		grid = new NavigationGrid (10, 10);
		createMap (10, 10);
	}

	public void setGrid(NavigationGrid Grid){
		grid = Grid;
	}

	public void createMap(int SizeX, int SizeY){
		//mesh.Clear ();
		generateMapDataStructure ();
		createMapAssets ();
		//mesh = createMapMesh (mesh);
/*		mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();

		generateMapDataStructure ();

		X = SizeX;
		Y = SizeY;

		int verticeCount = CountUsedVerticies ();
		Vertices = new Vector3[verticeCount];
*/	}

	void generateMapDataStructure(){
		MapData = new BlockNode[grid.X, maxDepth, grid.Z];

		for(int x = 0; x < grid.X; x++){
			for(int y = 0; y < maxDepth; y++){
				for(int z = 0; z < grid.Z; z++){
					MapData [x, y, z] = new BlockNode (new Vector3 (x, y, z));
					if (Random.value > .5f) {
						MapData [x, y, z].BlockType = 1;
					} else {
						MapData [x, y, z].BlockType = 0;
					}
				}
			}
		}
	}

	void createMapAssets (){
		int x =0, y = 0, z = 0;
		for(y = 0; y < maxDepth; y++){
			for(x = 0; x < grid.X; x++){
				for(z = 0; z < grid.Z; z++){
					if (MapData [x, y, z].BlockType != 0 && !MapData [x, y, z].used) {
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.transform.position = Vector3.zero;
						createObjectMesh(cube.GetComponent<MeshFilter> ().mesh, MapData[x,y,z].BlockType, MapData[x,y,z].coord);
					}
				}
			}
		}
	}


	int maxSize = 1000;

	void createObjectMesh(Mesh mesh, int blockType, Vector3 position){
		mesh.Clear ();
		Vector3[] verts = new Vector3[24 * maxSize];
		int[] tri = new int[36 * maxSize];
		Vector3[] norms = new Vector3[24 * maxSize];
		int i = 0;
		int j = 0;
		int x = (int) position.x;
		int y = (int) position.y;
		int z = (int) position.z;
		int count = 0;

		addCube (x, y, z, blockType, ref count, verts, tri, norms, ref i, ref j);

		Vector3[] vertsAct = new Vector3[i];
		int[] triAct = new int[j];
		Vector3[] normsAct = new Vector3[i];

		for (x = 0; x < j; x++) {
			if (x < i) {
				vertsAct [x] = verts [x];
				normsAct [x] = norms [x];
			}
			triAct [x] = tri [x];
		}

		mesh.vertices = vertsAct;
		mesh.triangles = triAct;
		mesh.normals = normsAct;

	}

	void addCube(int x, int y, int z, int blockType, ref int count, Vector3[] verts, int[] tri, Vector3[] norms, ref int i, ref int j){
		if (MapData [x, y, z].BlockType == blockType && !MapData [x, y, z].used) {
			MapData [x, y, z].used = true;
			//first use recursion to find all the cubes to add
			if (count < maxSize && x + 1 < MapData.GetLength(0)) {
				count++;
				x++;
				addCube (x,y,z, blockType, ref count, verts, tri, norms, ref i, ref j);
				x--;
			}
			if (count < maxSize && x - 1 >= 0) {
				count++;
				x--;
				addCube (x,y,z, blockType, ref count, verts, tri, norms, ref i, ref j);
				x++;
			}
			if (count < maxSize && z + 1 < MapData.GetLength(2)) {
				count++;
				z++;
				addCube (x,y,z, blockType, ref count, verts, tri, norms, ref i, ref j);
				z--;
			}
			if (count < maxSize && z - 1 >= 0) {
				count++;
				z--;
				addCube (x,y,z, blockType, ref count, verts, tri, norms, ref i, ref j);
				z++;
			}
			if (count < maxSize && y + 1 < MapData.GetLength(1)) {
				count++;
				y++;
				addCube (x,y,z, blockType, ref count, verts, tri, norms, ref i, ref j);
				y--;
			}
			if (count < maxSize && y - 1 >= 0) {
				count++;
				y--;
				addCube (x,y,z, blockType, ref count, verts, tri, norms, ref i, ref j);
				y++;
			}
			//now build the cube
			//top side
			if (y + 1 < MapData.GetLength(1) && blockType != MapData [x, y + 1, z].BlockType
				|| y + 1 >= MapData.GetLength (1)) {

				for (int c = 0; c < 4; c++) {
					verts [i] = MapData [x, y, z].top [c];
					norms [i++] = new Vector3 (0, 1, 0);
					if (i % 4 == 3) {
						tri [j++] = i - 2;
						tri [j++] = i - 1;
						tri [j++] = i;
					} else if (i % 4 == 2) {
						tri [j++] = i - 2;
						tri [j++] = i;
						tri [j++] = i - 1;
					}
				}
			}
			//bottom side
			if (y - 1 >= 0 && blockType != MapData [x, y - 1, z].BlockType
				|| y - 1 < 0) {
				for (int c = 0; c < 4; c++) {
					verts [i] = MapData [x, y, z].bottom [c];
					norms [i++] = new Vector3 (0, -1, 0);
					if (i % 4 == 3) {
						tri [j++] = i - 2;
						tri [j++] = i - 1;
						tri [j++] = i;
					} else if (i % 4 == 2) {
						tri [j++] = i - 2;
						tri [j++] = i;
						tri [j++] = i - 1;
					}
				}
			}
			//front
			if (z - 1 >= 0 && blockType != MapData [x, y, z - 1].BlockType
				|| z - 1 < 0) {
				for (int c = 0; c < 4; c++) {
					verts [i] = MapData [x, y, z].front [c];
					norms [i++] = new Vector3 (0, 0, -1);
					if (i % 4 == 3) {
						tri [j++] = i - 2;
						tri [j++] = i - 1;
						tri [j++] = i;
					} else if (i % 4 == 2) {
						tri [j++] = i - 2;
						tri [j++] = i;
						tri [j++] = i - 1;
					}
				}
			}
			//back
			if ( z + 1 < MapData.GetLength(2) && blockType != MapData [x, y, z + 1].BlockType
				|| z + 1 >= MapData.GetLength (2)) {
				for (int c = 0; c < 4; c++) {
					verts [i] = MapData [x, y, z].back [c];
					norms [i++] = new Vector3 (0, 0, 1);
					if (i % 4 == 3) {
						tri [j++] = i - 2;
						tri [j++] = i - 1;
						tri [j++] = i;
					} else if (i % 4 == 2) {
						tri [j++] = i - 2;
						tri [j++] = i;
						tri [j++] = i - 1;
					}
				}
			}
			//left
			if (x - 1 >= 0 && blockType != MapData [x - 1, y, z].BlockType
				|| x - 1 < 0) {
				for (int c = 0; c < 4; c++) {
					verts [i] = MapData [x, y, z].left [c];
					norms [i++] = new Vector3 (-1, 0, 0);
					if (i % 4 == 3) {
						tri [j++] = i - 2;
						tri [j++] = i - 1;
						tri [j++] = i;
					} else if (i % 4 == 2) {
						tri [j++] = i - 2;
						tri [j++] = i;
						tri [j++] = i - 1;
					}
				}
			}
			//right
			if (x + 1 < MapData.GetLength(0) && blockType != MapData [x + 1, y, z].BlockType
				|| x + 1 >= MapData.GetLength (0)) {
				for (int c = 0; c < 4; c++) {
					verts [i] = MapData [x, y, z].right [c];
					norms [i++] = new Vector3 (1, 0, 0);
					if (i % 4 == 3) {
						tri [j++] = i - 2;
						tri [j++] = i - 1;
						tri [j++] = i;
					} else if (i % 4 == 2) {
						tri [j++] = i - 2;
						tri [j++] = i;
						tri [j++] = i - 1;
					}
				}
			}
		}
	}


	//OLD FUNCTION START

	Mesh createMapMesh(Mesh mesh){
		Vector3[] verts = new Vector3[24 * grid.X * grid.Z * maxDepth];
		int[] tri = new int[36 * grid.X * grid.Z * maxDepth];
		Vector3[] norms = new Vector3[24 * grid.X * grid.Z * maxDepth];
		int i = 0;
		int j = 0;
		for(int y = 0; y < maxDepth; y++){
			for(int x = 0; x < grid.X; x++){
				for(int z = 0; z < grid.Z; z++){
					if (MapData [x, y, z].BlockType != 0 &&
						y + 1 < maxDepth &&
						MapData [x, y, z].BlockType != MapData [x, y + 1, z].BlockType
						|| y + 1 >= maxDepth &&
						MapData[x,y,z].BlockType != 0) {
						for (int c = 0; c < 4; c++) {
							verts [i] = MapData [x, y, z].top [c];
							norms [i++] = new Vector3 (0, 1, 0);
							if (i % 4 == 3) {
								tri [j++] = i - 2;
								tri [j++] = i - 1;
								tri [j++] = i;
							} else if (i % 4 == 2) {
								tri [j++] = i - 2;
								tri [j++] = i;
								tri [j++] = i - 1;
							}
						}
					}
					if (MapData [x, y, z].BlockType != 0 &&
					    y - 1 >= 0 &&
					    MapData [x, y, z].BlockType != MapData [x, y - 1, z].BlockType
						|| y - 1 < 0 &&
						MapData[x,y,z].BlockType != 0) {
						for (int c = 0; c < 4; c++) {
							verts [i] = MapData [x, y, z].bottom [c];
							norms [i++] = new Vector3 (0, -1, 0);
							if (i % 4 == 3) {
								tri [j++] = i - 2;
								tri [j++] = i - 1;
								tri [j++] = i;
							} else if (i % 4 == 2) {
								tri [j++] = i - 2;
								tri [j++] = i;
								tri [j++] = i - 1;
							}
						}
					}
					if (MapData [x, y, z].BlockType != 0 &&
					    z - 1 >= 0 &&
					    MapData [x, y, z].BlockType != MapData [x, y, z - 1].BlockType
						|| z - 1 < 0 &&
						MapData[x,y,z].BlockType != 0) {
						for (int c = 0; c < 4; c++) {
							verts [i] = MapData [x, y, z].front [c];
							norms [i++] = new Vector3 (0, 0, -1);
							if (i % 4 == 3) {
								tri [j++] = i - 2;
								tri [j++] = i - 1;
								tri [j++] = i;
							} else if (i % 4 == 2) {
								tri [j++] = i - 2;
								tri [j++] = i;
								tri [j++] = i - 1;
							}
						}
					}
					if (MapData [x, y, z].BlockType != 0 &&
					    z + 1 < grid.Z &&
					    MapData [x, y, z].BlockType != MapData [x, y, z + 1].BlockType
						|| z + 1 >= grid.Z &&
						MapData[x,y,z].BlockType != 0) {
						for (int c = 0; c < 4; c++) {
							verts [i] = MapData [x, y, z].back [c];
							norms [i++] = new Vector3 (0, 0, 1);
							if (i % 4 == 3) {
								tri [j++] = i - 2;
								tri [j++] = i - 1;
								tri [j++] = i;
							} else if (i % 4 == 2) {
								tri [j++] = i - 2;
								tri [j++] = i;
								tri [j++] = i - 1;
							}
						}
					}
					if (MapData [x, y, z].BlockType != 0 &&
					    x - 1 >= 0 &&
					    MapData [x, y, z].BlockType != MapData [x - 1, y, z].BlockType
						|| x - 1 < 0 &&
						MapData[x,y,z].BlockType != 0) {
						for (int c = 0; c < 4; c++) {
							verts [i] = MapData [x, y, z].left [c];
							norms [i++] = new Vector3 (-1, 0, 0);
							if (i % 4 == 3) {
								tri [j++] = i - 2;
								tri [j++] = i - 1;
								tri [j++] = i;
							} else if (i % 4 == 2) {
								tri [j++] = i - 2;
								tri [j++] = i;
								tri [j++] = i - 1;
							}
						}
					}//NEED TO PUT IN BOUNDARIES
					if (MapData [x, y, z].BlockType != 0 &&
						x + 1 < grid.X &&
					    MapData [x, y, z].BlockType != MapData [x + 1, y, z].BlockType
						|| x + 1 >= grid.X &&
						MapData[x,y,z].BlockType != 0) {
						for (int c = 0; c < 4; c++) {
							verts [i] = MapData [x, y, z].right [c];
							norms [i++] = new Vector3 (1, 0, 0);
							if (i % 4 == 3) {
								tri [j++] = i - 2;
								tri [j++] = i - 1;
								tri [j++] = i;
							} else if (i % 4 == 2) {
								tri [j++] = i - 2;
								tri [j++] = i;
								tri [j++] = i - 1;
							}
						}
					}
				}
			}
		}
			
		Vector3[] vertsAct = new Vector3[i];
		int[] triAct = new int[j];
		Vector3[] normsAct = new Vector3[i];

		for (int x = 0; x < j; x++) {
			if (x < i) {
				vertsAct [x] = verts [x];
				normsAct [x] = norms [x];
			}
			triAct [x] = tri [x];
		}

		mesh.vertices = vertsAct;
		mesh.triangles = triAct;
		mesh.normals = normsAct;

		transform.GetComponent<MeshCollider>().sharedMesh = mesh;

		return mesh;
	}

	Mesh trimFat(Vector3[] verts, int[] tri, Vector3[] norms){
		int vertsL = verts.Length, triL = tri.Length, normsL = norms.Length;

		return mesh;
	}

	int CountUsedVerticies(){
		int length = grid.X * grid.Z * maxDepth;
		int usedVerts = 0;
		for (int i = 0; i < length; i++) {
			
		}

		return usedVerts;
	}

	private class BlockNode{
		public short BlockType = 1;
		public Vector3 coord;
		public Vector3[] verticies;
		public int[] triangles;
		int[] trianglesTemp;
		public Vector3[] top;
		public Vector3[] bottom;
		public Vector3[] left;
		public Vector3[] right;
		public Vector3[] front;
		public Vector3[] back;

		public bool topUsed = true;
		public bool bottomUsed = true;
		public bool leftUsed = true;
		public bool rightUsed = true;
		public bool frontUsed = true;
		public bool backUsed = true;

		public bool used = false;

		public BlockNode(){}

		public BlockNode(Vector3 coords){
			coord = coords;

			trianglesTemp = new int[6];
			trianglesTemp[0] = 0;
			trianglesTemp[1] = 2;
			trianglesTemp[2] = 3;
			trianglesTemp[3] = 0;
			trianglesTemp[4] = 3;
			trianglesTemp[5] = 1;

			back = new Vector3[4];
			back[0] = new Vector3(-.5f, .5f, .5f);
			back[1] = new Vector3(.5f, .5f, .5f);
			back[2] = new Vector3(-.5f, -.5f, .5f);
			back[3] = new Vector3(.5f, -.5f, .5f);

			front = new Vector3[4];
			front[0] = new Vector3(.5f, .5f, -.5f);
			front[1] = new Vector3(-.5f, .5f, -.5f);
			front[2] = new Vector3(.5f, -.5f, -.5f);
			front[3] = new Vector3(-.5f, -.5f, -.5f);

			top = new Vector3[4];
			top[0] = new Vector3(.5f, .5f, .5f);
			top[1] = new Vector3(-.5f, .5f, .5f);
			top[2] = new Vector3(.5f, .5f, -.5f);
			top[3] = new Vector3(-.5f, .5f, -.5f);

			bottom = new Vector3[4];
			bottom[0] = new Vector3(-.5f, -.5f, .5f);
			bottom[1] = new Vector3(.5f, -.5f, .5f);
			bottom[2] = new Vector3(-.5f, -.5f, -.5f);
			bottom[3] = new Vector3(.5f, -.5f, -.5f);

			left = new Vector3[4];
			left[0] = new Vector3(-.5f, .5f, -.5f);
			left[1] = new Vector3(-.5f, .5f, .5f);
			left[2] = new Vector3(-.5f, -.5f, -.5f);
			left[3] = new Vector3(-.5f, -.5f, .5f);

			right = new Vector3[4];
			right[0] = new Vector3(.5f, .5f, .5f);
			right[1] = new Vector3(.5f, .5f, -.5f);
			right[2] = new Vector3(.5f, -.5f, .5f);
			right[3] = new Vector3(.5f, -.5f, -.5f);

			for(int j = 0; j < 4; j++){
				top[j] += coord;
				bottom[j] += coord;
				left[j] += coord;
				right[j] += coord;
				back[j] += coord;
				front[j] += coord;
			}
		}
	}
}
