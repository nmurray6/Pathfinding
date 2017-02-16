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
	Vector3 vec;// = new Vector3 (1, 1, 1f);

	Vector3[] Vertices;
	Vector2[] UV;
	int[] Triangles;
	Vector3 a;

	void Start(){
		grid = new NavigationGrid (10, 10);
		createMap (10, 10);

		a = Vector3.zero;
	
		vec = new Vector3 (1, 0, 0);
		//rayCastMap (new Vector3 (1, .5f, 1), Vector3.zero, Vector3.zero);
	}
	int c = 0;
	void FixedUpdate(){
		c++;
		if(c > 500){
			c = 0;
			Vector3 dir = new Vector3 ();
			dir.x = Random.value * 2 - 1;
			dir.y = Random.value * 2 - 1;
			dir.z = Random.value * 2 - 1;

			Vector3 orig = new Vector3 (Random.value * MapData.GetLength (0), Random.value * MapData.GetLength (1), Random.value * MapData.GetLength (2));

			rayCastMap (dir, orig, orig + new Vector3(0.5f, 0.5f, 0.5f));
		}
		/*	if (o > 1) {
			o = 0;
			/*if (a.y < maxDepth) {
				removeCubeAtPos (a);
				a.x++;
				if (a.x >= grid.X) {
					a.x = 0;
					a.z++;
					if (a.z >= grid.Z) {
						a.z = 0;
						a.y++;
					}
				}

			int gx = Random.Range (grid.X/4, (int)(grid.X*0.75f));
			int gz = Random.Range (0, grid.Z);
			int gy = Random.Range (0, maxDepth);

			a = new Vector3 (gx, gy, gz);
			removeCubeAtPos (a);
			}
		else
			o++;*/
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
				//	if (Random.value > .5f) {
				//		MapData [x, y, z].BlockType = 1;
				//	} else {
				//		MapData [x, y, z].BlockType = 0;
				//	}
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


	int maxSize = 2000;

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
		position.x = round (position.x);
		position.y = round (position.y);
		position.z = round (position.z);

		if (MapData [(int)position.x, (int)position.y, (int)position.z].BlockType != 0) {
			int n = 0;
			while (MapData [(int)position.x, (int)position.y, (int)position.z] != MapData [(int)position.x, (int)position.y, (int)position.z].infoHolder.blocks [n])
				n++;

			removeCube (MapData [(int)position.x, (int)position.y, (int)position.z].infoHolder.blocks [n], n);
		}
	}

	void removeCube(BlockNode block, int arrIndex){
		if (block.BlockType != 0) {
			block.BlockType = 0;
			for (int x = 0; x < block.infoHolder.blocks.Length; x++) {
				block.infoHolder.blocks [x].used = false;
			}

			if (block.infoHolder.blocks.Length == 1) {
				Destroy (block.infoHolder.cube);
				block.infoHolder.blocks = null;
				block.infoHolder = null;
			} else {
				//Destroy(block.infoHolder.cube);
				block.infoHolder.blocks = null;
			}
			recreateMesh (block);
		}
	}

	void recreateMesh(BlockNode block){
		int ctr = 0;
		Vector3 position = block.coord;
		int maxX = position.x + 1 < grid.X ? (int)position.x + 1 : (int)position.x;
		int maxY = position.y + 1 < maxDepth ? (int)position.y + 1 : (int)position.y;
		int maxZ = position.z + 1 < grid.Z ? (int)position.z + 1 : (int)position.z;
		for(int y = position.y - 1 >= 0 ? (int)position.y - 1 : (int)position.y; y <= maxY; y++){
			for(int x = position.x - 1 >= 0 ? (int)position.x - 1 : (int)position.x; x <= maxX; x++){
				for(int z = position.z - 1 >= 0 ? (int)position.z - 1 : (int)position.z; z <= maxZ; z++){
					if (ctr != 0 && MapData [x, y, z].BlockType != 0 && !MapData [x, y, z].used) {
						InformationHolder holder = new InformationHolder ();
						holder.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						holder.cube.transform.position = Vector3.zero;
						createObjectMesh(holder, MapData[x,y,z].BlockType, MapData[x,y,z].coord);
						Destroy(holder.cube.GetComponent<BoxCollider> ());

					} else if (MapData [x, y, z].BlockType != 0 && !MapData [x, y, z].used) {
						createObjectMesh (block.infoHolder, MapData [x, y, z].BlockType, MapData [x, y, z].coord);
						ctr++;
					}
				}
			}
		}
	}

	private int round(float a){
		return (a-(int)a >=0.5f) ? (int)a+1 : (int)a;
	}

	public BlockNode rayCastMap(Vector3 dir, Vector3 origin, Vector3 pos){
		BlockNode target = new BlockNode();
		Vector3 hit = new Vector3 ();

		MapData [(int)pos.x, (int)pos.y, (int)pos.z].intersect (origin, dir, ref hit);


		return target;
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

	public bool intersect(Vector3 origin, Vector3 direction, ref Vector3 hit){
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

		if (X1AxisHit.x <= top [0].x && X1AxisHit.y <= top [0].y && X1AxisHit.z <= top [0].z
		    && X1AxisHit.x >= bottom [2].x && X1AxisHit.y >= bottom [2].y && X1AxisHit.z >= bottom [2].z) {
			if (direction.x < 0)
				hit = X0AxisHit;
			else
				hit = X1AxisHit;
		} else if (Y1AxisHit.x <= top [0].x && Y1AxisHit.y <= top [0].y && Y1AxisHit.z <= top [0].z
		           && Y1AxisHit.x >= bottom [2].x && Y1AxisHit.y >= bottom [2].y && Y1AxisHit.z >= bottom [2].z) {
			if (direction.y < 0)
				hit = Y0AxisHit;
			else
				hit = Y1AxisHit;
		} else if (Z1AxisHit.x <= top [0].x && Z1AxisHit.y <= top [0].y && Z1AxisHit.z <= top [0].z
		         && Z1AxisHit.x >= bottom [2].x && Z1AxisHit.y >= bottom [2].y && Z1AxisHit.z >= bottom [2].z) {
			if (direction.z < 0)
				hit = Z0AxisHit;
			else
				hit = Z1AxisHit;
		}
		return true;
	}
}

public class InformationHolder{
	public GameObject cube;
	public BlockNode[] blocks;

	public InformationHolder(){}
}
