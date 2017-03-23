using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public int MapSize;
	public int ChunkSize;
	public Color[] MapColors;
	public Texture2D MainMap;

	public float scale;

	public float NoiseScale1;

	// Use this for initialization
	void Start () {
		InitializeMap ();
		GenerateMap ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp (KeyCode.G)) {
			float StartTime = Time.realtimeSinceStartup;
			GenerateMap ();
			//GenerateNoiseMap();
			float EndTime = Time.realtimeSinceStartup - StartTime;

			Debug.Log ("It took " + EndTime + " seconds to generate.");
		}
		GenerateMap ();

		//GenerateNoiseMap ();
	}

	public void InitializeMap(){
		int h = ChunkSize * 16;
		MainMap = new Texture2D (h, h);

		MainMap.filterMode = FilterMode.Point;
		MainMap.wrapMode = TextureWrapMode.Clamp;

		gameObject.GetComponent<Renderer> ().material.mainTexture = MainMap;
	}

	public void GenerateNoiseMap(){
		int h = ChunkSize * 16;
		Color[] ColorMap = new Color[h * h];

		float kx = (float)(ChunkSize * 16.0f);
		float ky = (float)(ChunkSize * 16.0f);

		for (int x = 0; x < h; x++) {
			for (int y = 0; y < h; y++) {
				float dx = (float)(x/kx);
				float dy = (float)(y/ky);

				float s = Mathf.PerlinNoise (NoiseScale1*dx, NoiseScale1*dy);
				float Adjustment = -(Mathf.Pow((x) - (h/2),2)/Mathf.Pow(h/2,2) + (Mathf.Pow((y) - (h/2),2)/Mathf.Pow(h/2,2))) + 1.0f;
				int b = GetValue (Adjustment*s);
				ColorMap [y * h + x] = MapColors[b];
			}
		}

		MainMap.SetPixels (ColorMap);
		MainMap.Apply ();
	}

	public void GenerateMap(){
		int h = ChunkSize * 16;
		Color[] ColorMap = new Color[h * h];

		for(int mx = 0; mx < ChunkSize; mx++){
			for(int my = 0; my < ChunkSize; my++){
				//Get Samples

				float sx, sy;

				//float s1 = Mathf.PerlinNoise(NoiseScale1*(sx),NoiseScale1*(sy));
				//float s2 = Mathf.PerlinNoise(NoiseScale1*(sx + 1), NoiseScale1*(sy));
				//float s3 = Mathf.PerlinNoise(NoiseScale1*(sx), NoiseScale1*(sy + 1));
				//float s4 = Mathf.PerlinNoise(NoiseScale1*(sx + 1), NoiseScale1*(sy + 1));

				sx = ((float)(mx)/(float)ChunkSize);
				sy = ((float)(my)/(float)ChunkSize);
				float s1 = Mathf.PerlinNoise(NoiseScale1*sx,NoiseScale1*sy);

				sx = ((float)(mx+1)/(float)ChunkSize);
				sy = ((float)(my)/(float)ChunkSize);
				float s2 = Mathf.PerlinNoise(NoiseScale1*sx,NoiseScale1*sy);

				sx = ((float)(mx)/(float)ChunkSize);
				sy = ((float)(my+1)/(float)ChunkSize);
				float s3 = Mathf.PerlinNoise(NoiseScale1*sx,NoiseScale1*sy);

				sx = ((float)(mx+1)/(float)ChunkSize);
				sy = ((float)(my+1)/(float)ChunkSize);
				float s4 = Mathf.PerlinNoise(NoiseScale1*sx,NoiseScale1*sy);


				//float d1 = 0.05f;
				//float d2 = 0.15f;
				//float d3 = 0.25f;
				//float d4 = 0.35f;

				//Generate Chunk
				for (int x = 0; x < 16; x++) {
					for (int y = 0; y < 16; y++) {
						float cs = BiLerpSample(s1,s2,s3,s4,x,y);

						//float cs = BiLerpSample (d1, d2, d3, d4, x, y);

						float Adjustment = -(Mathf.Pow(((mx * 16) + x) - (h/2),2)/Mathf.Pow(h/2,2) + (Mathf.Pow(((my * 16) + y) - (h/2),2)/Mathf.Pow(h/2,2))) + 1.0f;

						//float sample = Adjustment * cs;
						//ColorMap [((my * 16) + y) * (h) + ((mx * 16) + x)] = new Color (0.0f, sample, sample);
						int b = GetValue(Adjustment*cs);

						ColorMap[((my * 16) + y) * (h) + ((mx * 16) + x)] = MapColors[b];
					}
				}
			}
		}

		MainMap.SetPixels (ColorMap);
		MainMap.Apply ();

	}

	public int GetValue(float a){
		int b = 0;

		if (a < 0.10f) {
			b = 1;
		} 
		else if (a < 0.20f) {
			b = 2;
		} 
		else if (a < 0.30f) {
			b = 3;
		} 
		else if (a < 0.40f) {
			b = 4;
		} 
		else if (a < 0.50f) {
			b = 5;
		} 
		else if (a < 0.60f) {
			b = 6;
		} 
		else if (a < 0.70f) {
			b = 7;
		} 
		else if (a < 0.80f) {
			b = 8;
		} 
		else if (a < 0.90f) {
			b = 9;
		} 
		else {
			b = 10;
		}
		return b;
	}

	public void GenerateMap1(){
		float s1 = 0.5f;
		float s2 = 1.0f;
		float s3 = 0.50f;
		float s4 = 0.25f;
		float dsize = (float)MapSize;
		Color[] ColorMap = new Color[MapSize * MapSize];

		for (int x = 0; x < MapSize; x++) {
			for (int y = 0; y < MapSize; y++) {

				float r = BiLerpSample (MapColors[0].r,MapColors[1].r,MapColors[2].r,MapColors[3].r,x,y);
				float g = BiLerpSample (MapColors[0].g,MapColors[1].g,MapColors[2].g,MapColors[3].g,x,y);
				float b = BiLerpSample (MapColors[0].b,MapColors[1].b,MapColors[2].b,MapColors[3].b,x,y);

				ColorMap [y * MapSize + x] = new Color (r,g,b);
			}
		}

		MainMap.SetPixels (ColorMap);
		MainMap.Apply ();
	}

	public float BiLinearSample(float a, float b, float c, float d, int u, int v){
		float x1 = 0.0f;
		float x2 = (float)16;
		float y1 = 0.0f;
		float y2 = (float)16;

		float f1 = (((x2 - u)/(x2 - x1))*a) + (((u - x1)/(x2 - x1))*b);
		float f2 = (((x2 - u)/(x2 - x1))*c) + (((u - x1)/(x2 - x1))*d);
		float f3 = (((y2 - v)/(y2 - y1))*f1) + (((v - y1)/(y2 - y1))*f2);

		return f3;
	}

	public float BiLerpSample (float a, float b, float c, float d, int u, int v){
		//float x1 = 0.0f;
		//float x2 = (float)MapSize;
		//float y1 = 0.0f;
		//float y2 = (float)MapSize;

		float dx = 16.0f;
		float dy = 16.0f;

		float f1 = (((dx - u)/(dx))*a) + (((u)/(dx))*b);
		float f2 = (((dx - u)/(dx))*c) + (((u)/(dx))*d);
		float f3 = (((dy - v)/(dy))*f1) + (((v)/(dy))*f2);

		return f3;
	}

	public float SampleLerp(float a, float b, float t){
		return (1.0f - t) * a + (t * b);
	}
}
