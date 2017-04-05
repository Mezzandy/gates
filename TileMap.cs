using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {

	public int Width;
	public int Height;

	public int[] CollisionMap;
	public int[] Map;
	public int MapSeed;

	// Use this for initialization
	void Start () {
		float StartTime = Time.realtimeSinceStartup;
		GenerateTileMap ();
		float EndTime = Time.realtimeSinceStartup - StartTime;
		Debug.Log ("It took " + EndTime + " seconds to generate tile map");
	}

	public void GenerateTileMap(){

		//MapSeed = Random.Range (int.MinValue, int.MaxValue);
		//Random.InitState (MapSeed);

		//CollisionMap = new int[Width * Height];

		float ox = 0.0625f;
		float oy = 0.0625f;

		Mesh newMesh = new Mesh ();

		List<Vector3> NewVertices = new List<Vector3> ();
		List<int> NewTriangles = new List<int> ();
		List<Vector2> NewUVs = new List<Vector2> ();

		int Index = 0;

		//int a = 1;
		//int b = 15;

		int a = 0;
		int b = 0;

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {

				int s = Map [y * Width + x];
				b = Mathf.RoundToInt(s/16.0f);		//y
				a = s - (b * 16);				//x

				//int collision = Random.Range (0, 10);
				//if (collision < 5) {
				//	a = 2;
				//	b = 4;
				//	CollisionMap [y * Width + x] = 1;
				//} 
				//else {
				//	a = 1;
				//	b = 15;
				//}

				//Front Face
				NewVertices.Add(new Vector3(x, y, 0));
				NewVertices.Add(new Vector3(x+1, y, 0));
				NewVertices.Add(new Vector3(x, y+1, 0));
				NewVertices.Add(new Vector3(x+1, y+1, 0));
				Index = NewVertices.Count - 4;		
				NewTriangles.Add(Index);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+1);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+3);
				NewTriangles.Add(Index+1);
				NewUVs.Add(new Vector2(a*ox, b*oy)); 
				NewUVs.Add(new Vector2(a*ox+ox, b*oy)); 
				NewUVs.Add(new Vector2(a*ox, b*oy+oy)); 
				NewUVs.Add(new Vector2(a*ox+ox, b*oy+oy)); 

			}
		}

		newMesh.vertices = NewVertices.ToArray ();
		newMesh.triangles = NewTriangles.ToArray ();
		newMesh.uv = NewUVs.ToArray ();
		newMesh.RecalculateNormals();

		gameObject.GetComponent<MeshFilter>().sharedMesh = newMesh;
		//gameObject.AddComponent<EdgeCollider2D> ().points = ColliderPoints.ToArray ();
		//gameObject.AddComponent<MeshCollider>().sharedMesh = newMesh;
		gameObject.transform.position = new Vector3(0,0,0);
		//gameObject.GetComponent<Renderer>().material = mainMaterial;
	}
}
