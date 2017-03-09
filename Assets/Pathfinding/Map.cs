using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Navigation;

public class Map : MonoBehaviour{
	int X;
	int Y;

	public Camera camera;

	NavigationGrid grid;
	BlockNode[,,] MapData;

	public const int maxDepth = 30;
	int maxSize = 100;
	private int TotalUnusedError = 0;

	Vector3[] Vertices;
	Vector2[] UV;
	int[] Triangles;

	GameObject obo;
	GameObject obo1;
	GameObject obo2;
	GameObject obo3;

	int useless = 0;
	void Start(){
		grid = new NavigationGrid (30, 30);
		TotalUnusedError = grid.X * grid.Z * maxDepth;
		createMap (grid.X, grid.Z);
		RemoveAirFromUnused ();

		obo = GameObject.CreatePrimitive(PrimitiveType.Cube);
		obo1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		obo2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		obo3 = GameObject.CreatePrimitive(PrimitiveType.Cube);

	}
	float e = 0;
	float d = 0;
	public float tim = 0.02f;
	void FixedUpdate(){
		Time.fixedDeltaTime = tim;
		Vector3 hit;
		e += 0.004f;
		d += 0.05f;
		//Vector3 pos = new Vector3 (grid.X/2 + grid.X*1.5f * Mathf.Sin (e), Mathf.Cos(e/2), grid.Z/2 + grid.Z*1.5f * Mathf.Cos (e));
		Vector3 pos = new Vector3(15+25*Mathf.Sin(e), Mathf.Abs(d)+25*Mathf.Cos(e/2), 15+25*Mathf.Cos(e));
		Vector3 pos1 = new Vector3(15+25*Mathf.Sin(e+3), Mathf.Abs(d+3)+25*Mathf.Cos((e+3)/2), 15+25*Mathf.Cos(e+3));
		Vector3 pos2 = new Vector3(15+25*Mathf.Sin(e+7), Mathf.Abs(d+7)+25*Mathf.Cos((e+7)/2), 15+25*Mathf.Cos(e+7));
		Vector3 pos3 = new Vector3(15+25*Mathf.Sin(e+23), Mathf.Abs(d+23)+25*Mathf.Cos((e+23)/2), 15+25*Mathf.Cos(e+23));


		//Vector3 pos = new Vector3(5.5f+d,3.5f,-10);

		if (d > maxDepth)
			d = -maxDepth;
		obo.transform.position = pos;
		obo1.transform.position = pos1;
		obo2.transform.position = pos2;
		obo3.transform.position = pos3;

		Vector3 rot = new Vector3 (-Mathf.Sin (e), -Mathf.Cos(e/2), -Mathf.Cos(e));
		Vector3 rot1 = new Vector3 (-Mathf.Sin (e+3), -Mathf.Cos((e+3)/2), -Mathf.Cos(e+3));
		Vector3 rot2 = new Vector3 (-Mathf.Sin (e+7), -Mathf.Cos((e+7)/2), -Mathf.Cos(e+7));
		Vector3 rot3 = new Vector3 (-Mathf.Sin (e+23), -Mathf.Cos((e+23)/2), -Mathf.Cos(e+23));
		//Vector3 rot = new Vector3(0,0,1);

		obo.transform.LookAt (pos + rot);
		obo1.transform.LookAt (pos1 + rot1);
		obo2.transform.LookAt (pos2 + rot2);
		obo3.transform.LookAt (pos3 + rot3);
		camera.transform.LookAt (pos + rot);
		int temp = useless % 4;
		switch (temp) {
		case 1:
			raycast (pos1, rot1, out hit);
			break;
		case 2:
			raycast (pos2, rot2, out hit);
			break;
		case 3:
			raycast (pos3, rot3, out hit);
			break;
		case 0:
			raycast (pos, rot, out hit);
			break;
		}
		useless++;

		camera.transform.position = pos;
		//camera.transform.LookAt (hit);
		//obo.transform.LookAt (hit);
		//MapData [0, 5, 0].intersect2 (pos, rot, out hit);
		//Debug.Log(TotalUnusedError);
		CheckForUnused ();
		if (TotalUnusedError < 0)
			TotalUnusedError = 0;
	}

	public void setGrid(NavigationGrid Grid){
		grid = Grid;
	}

	public void createMap(int SizeX, int SizeY){
		generateMapDataStructure ();
		createMapAssets ();
	}

	void generateMapDataStructure(){
		MapData = new BlockNode[grid.X, maxDepth, grid.Z];

		for(int x = 0; x < grid.X; x++){
			for(int y = 0; y < maxDepth; y++){
				for(int z = 0; z < grid.Z; z++){
					MapData [x, y, z] = new BlockNode (new Vector3 (x, y, z));
					MapData [x, y, z].BlockType = 1;
					//	if (Random.value > .5f) {
				//		MapData [x, y, z].BlockType = 1;
				//	} else {
				//		MapData [x, y, z].BlockType = 0;
				//	}
			/*		if (x < grid.X / 5 || x > grid.X - grid.X / 5)
						MapData [x, y, z].BlockType = 1;
					else if (y < maxDepth / 5 || y > maxDepth - maxDepth / 5)
						MapData [x, y, z].BlockType = 1;
					else if (z < grid.Z / 5 || z > grid.Z - grid.Z / 5)
						MapData [x, y, z].BlockType = 1;
					else
						MapData [x, y, z].BlockType = 0;
			*/	}
			}
		}
	}

	private void CheckForUnused(){
		if (TotalUnusedError > 0) {
			createMapAssets ();
		}
	}

	private void RemoveAirFromUnused(){
		for (int x = 0; x < grid.X; x++) {
			for (int y = 0; y < maxDepth; y++) {
				for (int z = 0; z < grid.Z; z++) {
					if (MapData [x, y, z].BlockType == 0)
						TotalUnusedError--;
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
						InformationHolder holder = new InformationHolder ();
						holder.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						holder.cube.transform.position = Vector3.zero;
						createObjectMesh(holder, MapData[x,y,z].BlockType, MapData[x,y,z].coord);
						Destroy(holder.cube.GetComponent<BoxCollider> ());
					}
				}
			}
		}


	}
		

	void createObjectMesh(InformationHolder holder, int blockType, Vector3 position){
		Mesh mesh = holder.cube.GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();

		BlockNode[] tempBlockHolder = new BlockNode[maxSize+1];

		Vector3[] verts = new Vector3[24 * maxSize];
		int[] tri = new int[36 * maxSize];
		Vector3[] norms = new Vector3[24 * maxSize];
		int i = 0;
		int j = 0;
		int x = (int) position.x;
		int y = (int) position.y;
		int z = (int) position.z;
		int count = 0;

		addCube (x, y, z, blockType, ref count, verts, tri, norms, ref i, ref j, tempBlockHolder, holder);

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

		TotalUnusedError -= count+1;

		if (count < maxSize) {
			holder.blocks = new BlockNode[count + 1];
			for (x = 0; x <= count; x++) {
				holder.blocks [x] = tempBlockHolder [x];
			}
		} else {
			holder.blocks = tempBlockHolder;
		}
	}

	void addCube(int x, int y, int z, int blockType, ref int count, Vector3[] verts, int[] tri, Vector3[] norms, ref int i, ref int j, BlockNode[] blockHolder, InformationHolder info){
		if (MapData [x, y, z].BlockType == blockType && !MapData [x, y, z].used) {
			MapData [x, y, z].used = true;
			blockHolder [count] = MapData [x, y, z];
			MapData [x, y, z].infoHolder = info;
			Debug.DrawLine (MapData [x, y, z].bottom [2], MapData [x, y, z].top [2], Color.green);
			Debug.DrawLine (MapData [x, y, z].bottom [0], MapData [x, y, z].top [0], Color.green);
			Debug.DrawLine (MapData [x, y, z].bottom [1], MapData [x, y, z].top [1], Color.green);
			Debug.DrawLine (MapData [x, y, z].bottom [3], MapData [x, y, z].top [3], Color.green);
			//first use recursion to find all the cubes to add
			if (count < maxSize && x + 1 < MapData.GetLength(0)
				&& MapData [x+1, y, z].BlockType == blockType && !MapData [x+1, y, z].used) {
				count++;
				addCube (x+1,y,z, blockType, ref count, verts, tri, norms, ref i, ref j, blockHolder, info);
			}
			if (count < maxSize && x - 1 >= 0
				&& MapData [x-1, y, z].BlockType == blockType && !MapData [x-1, y, z].used) {
				count++;
				addCube (x-1,y,z, blockType, ref count, verts, tri, norms, ref i, ref j, blockHolder, info);
			}
			if (count < maxSize && z + 1 < MapData.GetLength(2)
				&& MapData [x, y, z+1].BlockType == blockType && !MapData [x, y, z+1].used) {
				count++;
				addCube (x,y,z+1, blockType, ref count, verts, tri, norms, ref i, ref j, blockHolder, info);
			}
			if (count < maxSize && z - 1 >= 0
				&& MapData [x, y, z-1].BlockType == blockType && !MapData [x, y, z-1].used) {
				count++;
				addCube (x,y,z-1, blockType, ref count, verts, tri, norms, ref i, ref j, blockHolder, info);
			}
			if (count < maxSize && y + 1 < MapData.GetLength(1)
				&& MapData [x, y+1, z].BlockType == blockType && !MapData [x, y+1, z].used) {
				count++;
				addCube (x,y+1,z, blockType, ref count, verts, tri, norms, ref i, ref j, blockHolder, info);
			}
			if (count < maxSize && y - 1 >= 0
				&& MapData [x, y-1, z].BlockType == blockType && !MapData [x, y-1, z].used) {
				count++;
				addCube (x,y-1,z, blockType, ref count, verts, tri, norms, ref i, ref j, blockHolder, info);
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
		

	void removeCubeAtPos(Vector3 position){
	//	position.x = round (position.x);
	//	position.y = round (position.y);
	//	position.z = round (position.z);
		//Debug.Log(checkContains(position));
	/*	if (MapData [(int)position.x, (int)position.y, (int)position.z].BlockType != 0 && MapData [(int)position.x, (int)position.y, (int)position.z].infoHolder.blocks != null) {
			int n = 0;
			while (MapData [(int)position.x, (int)position.y, (int)position.z] != MapData [(int)position.x, (int)position.y, (int)position.z].infoHolder.blocks [n]) {
				n++;
			}
			removeCube (MapData [(int)position.x, (int)position.y, (int)position.z].infoHolder.blocks [n], n);
		}*/
		removeCube (MapData [(int)position.x, (int)position.y, (int)position.z], 0);
	}

	void removeCube(BlockNode block, int arrIndex){
		if (block.BlockType != 0) {
			block.BlockType = 0;
			TotalUnusedError--;
			block.used = false;
		//	for (int x = 0; x < block.infoHolder.blocks.Length; x++) {
		//		block.infoHolder.blocks [x].used = false;
		//	}

			if (block.infoHolder.blocks.Length == 1) {
				Destroy (block.infoHolder.cube);
				block.infoHolder.cube = null;
				block.infoHolder.blocks = null;
				block.infoHolder = null;
				refreshBlockAreaAfterRemoval (block);
			} else {
				block.infoHolder = null;
				refreshBlockAreaAfterRemoval (block);
			}
		}
			
		//testing
		for (int x = 0; x < grid.X; x++) {
			for (int y = 0; y < maxDepth; y++) {
				for (int z = 0; z < grid.Z; z++) {
					if (MapData [x, y, z].BlockType != 0 && !MapData [x, y, z].used) {
						Debug.DrawLine (MapData [x, y, z].bottom [2], MapData [x, y, z].top [0], Color.red);
					}
				}
			}
		}
		//end test
	}

	bool contains(GameObject trg, GameObject[] arr){
		for (int i = 0; i < arr.Length; i++) {
			if (trg != null && ReferenceEquals(trg, arr[i])) {
				return true;
			}
		}
		return false;
	}

	void refreshBlockAreaAfterRemoval(BlockNode block){
		GameObject[] arr = new GameObject[27];
		BlockNode[] blocks = new BlockNode[27];

		int minX = ((int)block.coord.x == 0) ? 0 : (int)block.coord.x - 1;
		int minY = ((int)block.coord.y == 0) ? 0 : (int)block.coord.y - 1;
		int minZ = ((int)block.coord.z == 0) ? 0 : (int)block.coord.z - 1;

		int maxX = ((int)block.coord.x == MapData.GetLength(0) - 1) ? MapData.GetLength(0) - 1 : (int)block.coord.x + 1;
		int maxY = ((int)block.coord.y == MapData.GetLength(1) - 1) ? MapData.GetLength(1) - 1 : (int)block.coord.y + 1;
		int maxZ = ((int)block.coord.z == MapData.GetLength(2) - 1) ? MapData.GetLength(2) - 1 : (int)block.coord.z + 1;

		int i = 0;
		for (int x = minX; x <= maxX; x++) {
			for (int y = minY; y <= maxY; y++) {
				for (int z = minZ; z <= maxZ; z++) {
					if (MapData [x, y, z].infoHolder != null) {
						if (!contains (MapData [x, y, z].infoHolder.cube, arr)) {
							arr [i] = MapData [x, y, z].infoHolder.cube;
							blocks [i] = MapData [x, y, z];
							i++;
						}
					}
				}
			}
		}

		for (int n = 0; n < i; n++) {
			if(blocks[n].BlockType != 0)
				refreshBlockMesh (blocks[n]);
		}

		i = 0;
		arr = new GameObject[27];
		for (int x = minX; x <= maxX; x++) {
			for (int y = minY; y <= maxY; y++) {
				for (int z = minZ; z <= maxZ; z++) {
					if (!((int)block.coord.x == x && (int)block.coord.y == y && (int)block.coord.z == z)
						&& MapData [x, y, z].BlockType != 0 && !MapData [x, y, z].used) {

						InformationHolder holder = new InformationHolder ();
						holder.cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
						holder.cube.transform.position = Vector3.zero;
						createObjectMesh (holder, MapData [x, y, z].BlockType, MapData [x, y, z].coord);
						Destroy (holder.cube.GetComponent<BoxCollider> ());
					}
				}
			}
		}

		for (int n = 0; n < i; n++) {
			if (blocks [n].BlockType != 0) {
				InformationHolder holder = new InformationHolder ();
				holder.cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				holder.cube.transform.position = Vector3.zero;
				createObjectMesh (holder, blocks[n].BlockType, blocks[n].coord);
				Destroy (holder.cube.GetComponent<BoxCollider> ());
			}
		}

	/*	int n = 0;
		GameObject[] arra = new GameObject[20000];
		for(int y = 0; y < maxDepth; y++){
			for(int x = 0; x < grid.X; x++){
				for(int z = 0; z < grid.Z; z++){
				//	if (!contains (MapData [x, y, z].infoHolder.cube, arra)) {
				//		arra [n] = MapData [x, y, z].infoHolder.cube;
				//		n++;
				//	}
					if (MapData [x, y, z].infoHolder.cube == null)
						n++;//Debug.Log (MapData [x, y, z].coord);
				}
			}
		}
		Debug.Log (n);*/
	}

	void refreshBlock (BlockNode block){
		int x = (int)block.coord.x;
		int y = (int)block.coord.y;
		int z = (int)block.coord.z;
		if (block.BlockType != 0) {
			InformationHolder holder = new InformationHolder ();
			holder.cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			holder.cube.transform.position = Vector3.zero;
			createObjectMesh (holder, MapData [x, y, z].BlockType, MapData [x, y, z].coord);
			Destroy (holder.cube.GetComponent<BoxCollider> ());
		} 
	}


	void refreshBlockMesh(BlockNode block){
		for (int i = 0; i < block.infoHolder.blocks.Length; i++) {
			block.infoHolder.blocks [i].used = false;
			TotalUnusedError++;
		}
		createObjectMesh (block.infoHolder, block.BlockType, block.coord);
	}

	private int round(float a){
		return (a-(int)a >=0.5f) ? (int)a+1 : (int)a;
	}

	public BlockNode raycast(Vector3 origin, Vector3 direction, out Vector3 hit){
		Vector3 pos = origin;
		return raycast (origin, direction, pos, out hit);
	}

	public BlockNode raycast(Vector3 origin, Vector3 direction, Vector3 position, out Vector3 hit){
		if (position != origin) {
			if ((int)position.x >= 0 && (int)position.x < MapData.GetLength (0)
				&& (int)position.y >= 0 && (int)position.y < MapData.GetLength (1)
				&& (int)position.z >= 0 && (int)position.z < MapData.GetLength (2)) {
				return rayCastMap (direction, origin, position, out hit);
			} else
				return raycastFromOutOfMap (origin, direction, out hit);
		} else {
			if ((int)origin.x >= 0 && (int)origin.x < MapData.GetLength (0)
			   && (int)origin.y >= 0 && (int)origin.y < MapData.GetLength (1)
			   && (int)origin.z >= 0 && (int)origin.z < MapData.GetLength (2)) {
				return rayCastMap (direction, origin, position, out hit);
			} else
				return raycastFromOutOfMap (origin, direction, out hit);
		}
	}
	public BlockNode rayCastMap(Vector3 dir, Vector3 origin, Vector3 position, out Vector3 hit){
		hit = origin;
		Vector3 pos = position;
		if (!HandleHit (position)) {
			int c = 0;
			pos = MapData [(int)pos.x, (int)pos.y, (int)pos.z].intersect2 (origin, dir, out hit);
			while (pos.x >= 0 && pos.x < MapData.GetLength (0)
			      && pos.y >= 0 && pos.y < MapData.GetLength (1)
			      && pos.z >= 0 && pos.z < MapData.GetLength (2)
			      && !HandleHit (pos)) {
				pos = MapData [(int)pos.x, (int)pos.y, (int)pos.z].intersect2 (hit, dir, out hit);

				c++;
				if (c > 70) {
					break;
				}
			}
		}
			
		return null;
	}

	public bool HandleHit(Vector3 pos){
		if (pos.x >= 0 && pos.x < MapData.GetLength (0)
			&& pos.y >= 0 && pos.y < MapData.GetLength (1)
			&& pos.z >= 0 && pos.z < MapData.GetLength (2)
			&& MapData [(int)pos.x, (int)pos.y, (int)pos.z].BlockType != 0) {

			MapData[(int)pos.x, (int)pos.y, (int)pos.z].health -= 1;
			if (MapData [(int)pos.x, (int)pos.y, (int)pos.z].health <= 0)
				removeCubeAtPos (pos);

			return true;
		}
		return false;
	}



	//Returns the position of the block node that is hit
	public BlockNode raycastFromOutOfMap(Vector3 origin, Vector3 direction, out Vector3 hit){
		hit = new Vector3 (-1,-1,-1);
		Vector3 position = Vector3.zero;
		float x0t = (-origin.x) / direction.x;
		float x1t = (grid.X - origin.x) / direction.x;
		float y0t = (-origin.y) / direction.y;
		float y1t = (maxDepth - origin.y) / direction.y;
		float z0t = (-origin.z) / direction.z;
		float z1t = (grid.Z - origin.z) / direction.z;

		Vector3 X0AxisHit = x0t * direction + origin;
		Vector3 X1AxisHit = x1t * direction + origin;
		Vector3 Y0AxisHit = y0t * direction + origin;
		Vector3 Y1AxisHit = y1t * direction + origin;
		Vector3 Z0AxisHit = z0t * direction + origin;
		Vector3 Z1AxisHit = z1t * direction + origin;

		if (direction.x < 0
			&& X1AxisHit.x <= grid.X + 0.01f && X1AxisHit.y <= maxDepth + 0.01f && X1AxisHit.z <= grid.Z + 0.01f
			&& X1AxisHit.x >= -0.01f && X1AxisHit.y >= -0.01f && X1AxisHit.z >= -0.01f) {
			hit = X1AxisHit;
			position.x--;

		} else if (direction.x >= 0
			&& X0AxisHit.x <= grid.X + 0.01f && X0AxisHit.y <= maxDepth + 0.01f && X0AxisHit.z <= grid.Z + 0.01f
			&& X0AxisHit.x >= -0.01f && X0AxisHit.y >= -0.01f && X0AxisHit.z >= -0.01f) {
			hit = X0AxisHit;

		} if (direction.y < 0
			&& Y1AxisHit.x <= grid.X + 0.01f && Y1AxisHit.y <= maxDepth + 0.01f && Y1AxisHit.z <= grid.Z + 0.01f
			&& Y1AxisHit.x >= -0.01f && Y1AxisHit.y - 0.01f >= -0.01f && Y1AxisHit.z >= -0.01f) {
			hit = Y1AxisHit;
			position.y--;

		} else if (direction.y >= 0
			&& Y0AxisHit.x <= grid.X + 0.01f && Y0AxisHit.y <= maxDepth + 0.01f && Y0AxisHit.z <= grid.Z + 0.01f
			&& Y0AxisHit.x >= -0.01f && Y0AxisHit.y >= -0.01f && Y0AxisHit.z >= -0.01f) {
			hit = Y0AxisHit;

		} if (direction.z < 0
		      && Z1AxisHit.x <= grid.X + 0.01f && Z1AxisHit.y <= maxDepth + 0.01f && Z1AxisHit.z <= grid.Z + 0.01f
		      && Z1AxisHit.x >= -0.01f && Z1AxisHit.y >= -0.01f && Z1AxisHit.z >= -0.01f) {
			hit = Z1AxisHit;
			position.z--;

		} else if (direction.z >= 0
		           && Z0AxisHit.x <= grid.X + 0.01f && Z0AxisHit.y <= maxDepth + 0.01f && Z0AxisHit.z <= grid.Z + 0.01f
		           && Z0AxisHit.x >= -0.01f && Z0AxisHit.y >= -0.01f && Z0AxisHit.z >= -0.01f) {
			hit = Z0AxisHit;
		}

		//Debug.DrawLine (origin, hit);

		position += hit;
		//Debug.Log (position);
		Debug.DrawLine(origin, hit);
		if (hit.x == -1 && hit.y == -1 && hit.z == -1)
			return null;
		else
			return rayCastMap (direction, hit, position, out hit);
	}
	/*
public BlockNode raycastFromOutOfMap(Vector3 origin, Vector3 direction, out Vector3 hit){
		hit = new Vector3 (-1, -1, -1);

		float x0t = (-origin.x) / direction.x;
		float x1t = (grid.X - origin.x) / direction.x;
		float y0t = (-origin.y) / direction.y;
		float y1t = (maxDepth - origin.y) / direction.y;
		float z0t = (-origin.z) / direction.z;
		float z1t = (grid.Z - origin.z) / direction.z;


		Vector3 dir = direction.normalized;

		float xt, yt, zt;

		if (dir.x < 0)
			xt = x1t;
		else
			xt = x0t;

		if (dir.y < 0)
			yt = y1t;
		else
			yt = y0t;

		if (dir.z < 0)
			zt = z1t;
		else
			zt = z0t;


		if (inBounds (xt * direction + origin))
			hit = xt * direction + origin;
		else if (inBounds (yt * direction + origin))
			hit = yt * direction + origin;
		else if (inBounds (zt * direction + origin))
			hit = zt * direction + origin;

		Vector3 position = Vector3.zero;
		if (Mathf.Abs (xt) > Mathf.Abs (yt)) {
			xt = yt;

			if (Mathf.Abs (xt) > Mathf.Abs (zt)) {
				xt = zt;
				if(Mathf.Sign(dir.z) < 0)
					position.z -= 1;
			} else {
				if(Mathf.Sign(dir.y) < 0)
					position.y -= 1;
			}
		} else if (Mathf.Abs (xt) > Mathf.Abs (zt)) {
			xt = zt;
			if(Mathf.Sign(dir.z) < 0)
				position.z -= 1;
		} else {
			if(Mathf.Sign(dir.x) < 0)
			position.x -= 1;
		}

		//hit = xt * direction + origin;

		position += hit;
		Debug.Log (position);

		Color color = new Color (position.x / 30, position.y/30, position.z/30);
		Debug.DrawLine (origin, hit, color);
		//Debug.DrawLine(origin, hit);


		return rayCastMap(direction, hit, position, out hit);
	}*/

	private bool inBounds(Vector3 pos){
		if (pos.x >= 0
		    && pos.y >= 0
		    && pos.z >= 0
		    && pos.x <= grid.X
			&& pos.y <= maxDepth
		    && pos.z <= grid.Z) {
			return true;
		} else
			return false;
	}
}



public class BlockNode{
	public short BlockType = 1;
	public Vector3 coord;
	public InformationHolder infoHolder;

	public Vector3[] top;
	public Vector3[] bottom;
	public Vector3[] left;
	public Vector3[] right;
	public Vector3[] front;
	public Vector3[] back;

	public float health = 1;

	public bool used = false;

	public BlockNode(){}

	public BlockNode(Vector3 coords){
		coord = coords;

		back = new Vector3[4];
		back[0] = new Vector3(0f, 1f, 1f);
		back[1] = new Vector3(1f, 1f, 1f);
		back[2] = new Vector3(0f, 0f, 1f);
		back[3] = new Vector3(1f, 0f, 1f);

		front = new Vector3[4];
		front[0] = new Vector3(1f, 1f, 0f);
		front[1] = new Vector3(0f, 1f, 0f);
		front[2] = new Vector3(1f, 0f, 0f);
		front[3] = new Vector3(0f, 0f, 0f);

		top = new Vector3[4];
		top[0] = new Vector3(1f, 1f, 1f);
		top[1] = new Vector3(0f, 1f, 1f);
		top[2] = new Vector3(1f, 1f, 0f);
		top[3] = new Vector3(0f, 1f, 0f);

		bottom = new Vector3[4];
		bottom[0] = new Vector3(0f, 0f, 1f);
		bottom[1] = new Vector3(1f, 0f, 1f);
		bottom[2] = new Vector3(0f, 0f, 0f);
		bottom[3] = new Vector3(1f, 0f, 0f);

		left = new Vector3[4];
		left[0] = new Vector3(0f, 1f, 0f);
		left[1] = new Vector3(0f, 1f, 1f);
		left[2] = new Vector3(0f, 0f, 0f);
		left[3] = new Vector3(0f, 0f, 1f);

		right = new Vector3[4];
		right[0] = new Vector3(1f, 1f, 1f);
		right[1] = new Vector3(1f, 1f, 0f);
		right[2] = new Vector3(1f, 0f, 1f);
		right[3] = new Vector3(1f, 0f, 0f);

		for(int j = 0; j < 4; j++){
			top[j] += coord;
			bottom[j] += coord;
			left[j] += coord;
			right[j] += coord;
			back[j] += coord;
			front[j] += coord;
		}
	}

	//Returns the position of the block node that is hit
	public Vector3 intersect(Vector3 origin, Vector3 direction, out Vector3 hit){
		hit = new Vector3 ();
		Vector3 position = coord;
		float x0t = (bottom[2].x - origin.x) / direction.x;
		float x1t = (top[0].x - origin.x) / direction.x;
		float y0t = (bottom[2].y - origin.y) / direction.y;
		float y1t = (top[0].y - origin.y) / direction.y;
		float z0t = (bottom[2].z - origin.z) / direction.z;
		float z1t = (top[0].z - origin.z) / direction.z;

		Vector3 X0AxisHit = x0t * direction + origin;
		Vector3 X1AxisHit = x1t * direction + origin;
		Vector3 Y0AxisHit = y0t * direction + origin;
		Vector3 Y1AxisHit = y1t * direction + origin;
		Vector3 Z0AxisHit = z0t * direction + origin;
		Vector3 Z1AxisHit = z1t * direction + origin;


		if (direction.x >= 0
			&& X1AxisHit.x <= top [0].x + 0.01f && X1AxisHit.y <= top [0].y + 0.01f && X1AxisHit.z <= top [0].z + 0.01f
			&& X1AxisHit.x >= bottom [2].x - 0.01f && X1AxisHit.y >= bottom [2].y - 0.01f && X1AxisHit.z >= bottom [2].z - 0.01f) {
			hit = VecRound(X1AxisHit);
			position.x++;

		} else if (direction.x < 0
			&& X0AxisHit.x <= top [0].x + 0.01f && X0AxisHit.y <= top [0].y + 0.01f && X0AxisHit.z <= top [0].z + 0.01f
			&& X0AxisHit.x >= bottom [2].x - 0.01f && X0AxisHit.y >= bottom [2].y - 0.01f && X0AxisHit.z >= bottom [2].z - 0.01f) {
			hit = VecRound(X0AxisHit);
			position.x--;

		} if (direction.y >= 0
			&& Y1AxisHit.x <= top [0].x + 0.01f && Y1AxisHit.y <= top [0].y + 0.01f && Y1AxisHit.z <= top [0].z + 0.01f
			&& Y1AxisHit.x >= bottom [2].x - 0.01f && Y1AxisHit.y - 0.01f >= bottom [2].y - 0.01f && Y1AxisHit.z >= bottom [2].z - 0.01f) {
			hit = VecRound(Y1AxisHit);
			position.y++;

		} else if (direction.y < 0
			&& Y0AxisHit.x <= top [0].x + 0.01f && Y0AxisHit.y <= top [0].y + 0.01f && Y0AxisHit.z <= top [0].z + 0.01f
			&& Y0AxisHit.x >= bottom [2].x - 0.01f && Y0AxisHit.y >= bottom [2].y - 0.01f && Y0AxisHit.z >= bottom [2].z - 0.01f) {
			hit = VecRound(Y0AxisHit);
			position.y--;

		} if (direction.z >= 0
			&& Z1AxisHit.x <= top [0].x + 0.01f && Z1AxisHit.y <= top [0].y + 0.01f && Z1AxisHit.z <= top [0].z + 0.01f
			&& Z1AxisHit.x >= bottom [2].x - 0.01f && Z1AxisHit.y >= bottom [2].y - 0.01f && Z1AxisHit.z >= bottom [2].z - 0.01f) {
			hit = VecRound(Z1AxisHit);
			position.z++;

		} else if (direction.z < 0
			&& Z0AxisHit.x <= top [0].x + 0.01f && Z0AxisHit.y <= top [0].y + 0.01f && Z0AxisHit.z <= top [0].z + 0.01f
			&& Z0AxisHit.x >= bottom [2].x - 0.01f && Z0AxisHit.y >= bottom [2].y - 0.01f && Z0AxisHit.z >= bottom [2].z - 0.01f) {
			hit = VecRound(Z0AxisHit);
			position.z--;
		} 

		Color color = new Color (position.x / 30, 0.5f, position.z/30);
		//Debug.DrawLine (origin, hit, color);
		//Debug.DrawLine(origin, hit);
		return position;
	}

	//TEST INTERSECT
	public Vector3 intersect2(Vector3 origin, Vector3 direction, out Vector3 hit){
		hit = new Vector3 ();
		Vector3 position = coord;
		float x0t = (bottom[2].x - origin.x) / direction.x;
		float x1t = (top[0].x - origin.x) / direction.x;
		float y0t = (bottom[2].y - origin.y) / direction.y;
		float y1t = (top[0].y - origin.y) / direction.y;
		float z0t = (bottom[2].z - origin.z) / direction.z;
		float z1t = (top[0].z - origin.z) / direction.z;


		Vector3 dir = direction.normalized;
		short dirVal = 0; 
		/*
		0 = xt
		1 = yt
		2 = zt
		*/

		float xt, yt, zt;

		if (dir.x >= 0)
			xt = x1t;
		else
			xt = x0t;

		if (dir.y >= 0)
			yt = y1t;
		else
			yt = y0t;

		if (dir.z >= 0)
			zt = z1t;
		else
			zt = z0t;

		if (Mathf.Abs (xt) > Mathf.Abs (yt)) {
			xt = yt;

			if (Mathf.Abs (xt) > Mathf.Abs (zt)) {
				xt = zt;
				position.z += Mathf.Sign (dir.z);
			} else {
				position.y += Mathf.Sign (dir.y);
			}
		} else if (Mathf.Abs (xt) > Mathf.Abs (zt)) {
			xt = zt;
			position.z += Mathf.Sign (dir.z);
		} else {
			position.x += Mathf.Sign (dir.x);
		}

		hit = xt * direction + origin;


		Color color = new Color (position.x / 30, position.y/30, position.z/30);
		Debug.DrawLine (origin, hit, color);
		//Debug.DrawLine(origin, hit);

		return position;
	}
	//END TEST INTERSECT


	private Vector3 VecRound(Vector3 vec){
		if (vec.x < bottom [2].x && vec.x >= bottom [2].x - 0.01f)
			vec.x = bottom [2].x;
		else if (vec.x > top [0].x && vec.x <= top [0].x + 0.01f)
			vec.x = top [0].x;

		if (vec.y < bottom [2].y && vec.y >= bottom [2].y - 0.01f)
			vec.y = bottom [2].y;
		else if (vec.y > top [0].y && vec.y <= top [0].y + 0.01f)
			vec.y = top [0].y;

		if (vec.z < bottom [2].z && vec.z >= bottom [2].z - 0.01f)
			vec.z = bottom [2].z;
		else if (vec.z > top [0].z && vec.z <= top [0].z + 0.01f)
			vec.x = top [0].z;

		return vec;
	}
}

public class InformationHolder{
	public GameObject cube;
	public BlockNode[] blocks;

	public InformationHolder(){}
}
