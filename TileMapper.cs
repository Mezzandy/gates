using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMapper : MonoBehaviour {

	public int XCell;
	public int YCell;
	public int TileId;

	//Tile Map Structure Chunk
	//
	//Position
	//Name
	//Materials[]
	//Layers[]

	public GameObject SceneCamera;
	public GameObject TileMapObject;
	public Material TileMaterial;

	public float CameraSpeed;
	public int[] TileMap;

	// Use this for initialization
	void Start () {
		TileMap = new int[16 * 16];

		TileMapObject = new GameObject ("Tile Map Object");
		TileMapObject.AddComponent<MeshFilter>();
		TileMapObject.AddComponent<MeshRenderer>();
		TileMapObject.AddComponent<MeshCollider>();

		TileMapObject.GetComponent<Renderer> ().material = TileMaterial;
		UpdateTileMap ();
	}
	
	// Update is called once per frame
	void Update () {
	
		KeyboardInput ();

	}

	public void KeyboardInput(){

		//Process Mouse Position
		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		XCell = (int)pos.x;
		YCell = (int)pos.y;

		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		Vector3 newPos = SceneCamera.transform.position;
		newPos.x += h * CameraSpeed * Time.deltaTime;
		newPos.y += v * CameraSpeed * Time.deltaTime;
		SceneCamera.transform.position = newPos;

		if (Input.GetMouseButtonDown (0)) {
			TileMap [YCell * 16 + XCell] = TileId;
			UpdateTileMap ();
		}

		if (Input.GetMouseButton (0) && Input.GetKey (KeyCode.LeftShift)) {
			TileMap [YCell * 16 + XCell] = TileId;
			UpdateTileMap ();
		}

		if (Input.GetMouseButtonDown (1)) {
			TileMap [YCell * 16 + XCell] = 0;
			UpdateTileMap ();
		}

		if (Input.GetMouseButton (1) && Input.GetKey (KeyCode.LeftShift)) {
			TileMap [YCell * 16 + XCell] = 0;
			UpdateTileMap ();
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			TileId--;
			if (TileId < 0) {
				TileId = 0;
			}
		}
		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			TileId++;
			if (TileId > 255) {
				TileId = 255;
			}
		}
		if (Input.GetKeyUp (KeyCode.UpArrow)) {
			TileId += 16;
			if (TileId > 255) {
				TileId = 255;
			}
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)) {
			TileId -= 16;
			if (TileId < 0) {
				TileId = 0;
			}
		}
	}

	public void UpdateTileMap(){
		float ox = 0.0625f;
		float oy = 0.0625f;

		Mesh newMesh = new Mesh ();

		List<Vector3> NewVertices = new List<Vector3> ();
		List<int> NewTriangles = new List<int> ();
		List<Vector2> NewUVs = new List<Vector2> ();

		int Index = 0;



		for (int x = 0; x < 16; x++) {
			for (int y = 0; y < 16; y++) {

				int s = TileMap [y * 16 + x];

				int b = Mathf.RoundToInt(s/16.0f);		//y
				int a = s - (b * 16);				//x

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
		newMesh.Optimize();


		TileMapObject.GetComponent<MeshFilter>().sharedMesh = newMesh;
		TileMapObject.GetComponent<MeshCollider>().sharedMesh = newMesh;
		TileMapObject.transform.position = new Vector3(0,0,0);
	}
}
