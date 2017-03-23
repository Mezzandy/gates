using UnityEngine;
using System.Collections;

public class MainMap : MonoBehaviour {

	public int TextureWidth;
	public int TextureHeight;

	public Texture2D MapTexture;

	public int MapSize;
	public int WorldSeed;
	public Vector3 NoiseOrigin1;
	public Vector3 NoiseOrigin2;
	public Vector3 NoiseOrigin3;
	public Vector3 NoiseOrigin4;
	public Vector3 NoiseScale1;
	public Vector3 NoiseScale2;
	public Vector3 NoiseScale3;
	public Vector3 NoiseScale4;

	public float SeaLevel = 0.15f;
	public float MasterScale;
	public float WorldTemperature;
	public float WorldHumidity;
	public float WorldHeight;
	public Color[] MapColors;

	public Vector2 vScale;
	public Vector3 gScale;
	public float AltF;

	public float BaseT;
	public float RoughT;
	public float DetailT;
	public float NegaT;

//	public float Rain; 
//	public float RainT;
//
//	public float Temp;
//	public float TempT;

	public Vector2 ViewCenter;

	public float Zoom;

	// Use this for initialization
	void Start () {
		MapTexture = new Texture2D (MapSize, MapSize);
		MapTexture.filterMode = FilterMode.Point;
		MapTexture.wrapMode = TextureWrapMode.Clamp;

		gameObject.GetComponent<Renderer> ().material.mainTexture = MapTexture;

		RandomColors ();


		WorldHumidity = Random.Range (0.75f,1.15f);
		WorldTemperature = Random.Range (0.75f,1.15f);
		SeaLevel = Random.Range (0.1f,0.2f);
		WorldHeight = 1.0f;
		MasterScale = 1.0f/MapSize;

		SeedValues ();
		GenerateMap ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp (KeyCode.L)) {
			RandomColors ();
		}

		if (Input.GetKeyUp (KeyCode.P)) {
			SeedValues ();
			GenerateMap ();
			AddCities (10);
		}

		if (Input.GetKey (KeyCode.W)) {
			ViewCenter.y += Zoom;
		}

		if (Input.GetKey (KeyCode.S)) {
			ViewCenter.y -= Zoom;
		}

		if (Input.GetKey (KeyCode.A)) {
			ViewCenter.x -= Zoom;
		}

		if (Input.GetKey (KeyCode.D)) {
			ViewCenter.x += Zoom;
		}

		if (Input.GetKey (KeyCode.LeftBracket)) {
			Zoom--;
			if (Zoom < 1) {
				Zoom = 1.0f;
			}
		}

		if (Input.GetKey (KeyCode.RightBracket)) {
			Zoom++;
		}
			
		UpdateValues ();
		GenerateMap ();

	}
		

	public void RandomColors(){
		for (int x = 0; x < MapColors.Length; x++) {
			MapColors[x] = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
		}
	}

	public void SeedValues(){
		//Set Random Values
		WorldSeed = Random.Range(int.MinValue, int.MaxValue);
		Random.seed = WorldSeed;

		WorldHumidity = Random.Range(0.85f,1.15f);
		WorldTemperature = Random.Range(0.85f,1.15f);
		WorldHeight = Random.Range(1.0f,1.25f);
		SeaLevel = Random.Range(0.15f,0.30f);

		NoiseOrigin1.x = Random.Range (0.0f,10.0f);
		NoiseOrigin1.z = Random.Range (0.0f,10.0f);
		NoiseOrigin2.x = Random.Range (0.0f,10.0f);
		NoiseOrigin2.z = Random.Range (0.0f,10.0f);
		NoiseOrigin3.x = Random.Range (0.0f,10.0f);
		NoiseOrigin3.z = Random.Range (0.0f,10.0f);
		NoiseOrigin4.x = Random.Range (0.0f,10.0f);
		NoiseOrigin4.z = Random.Range (0.0f,10.0f);

		NoiseScale1.x = Random.Range (vScale.x, vScale.y);
		NoiseScale1.z = NoiseScale1.x;

		NoiseScale2.x = NoiseScale1.x*gScale.x;
		NoiseScale2.z = NoiseScale1.z*gScale.x;

		NoiseScale3.x = NoiseScale2.x*gScale.y;
		NoiseScale3.z = NoiseScale2.z*gScale.y;

		NoiseScale4.x = NoiseScale1.x*gScale.z;
		NoiseScale4.z = NoiseScale1.z*gScale.z;
	}

	public void UpdateValues(){
		NoiseScale1.z = NoiseScale1.x;

		NoiseScale2.x = NoiseScale1.x*gScale.x;
		NoiseScale2.z = NoiseScale1.z*gScale.x;

		NoiseScale3.x = NoiseScale2.x*gScale.y;
		NoiseScale3.z = NoiseScale2.z*gScale.y;

		NoiseScale4.x = NoiseScale1.x*gScale.z;
		NoiseScale4.z = NoiseScale1.z*gScale.z;
	}

	public void GenerateMap(){
		Color[] ColorMap = new Color[MapTexture.width*MapTexture.height];

		for(int x = 0; x < MapTexture.width; x++){
			for(int z = 0; z < MapTexture.height; z++){
				int v = GetValue(new Vector3((1.0f/Zoom)*(ViewCenter.x+x),0,(1.0f/Zoom)*(ViewCenter.y+z)));
				ColorMap[z*MapTexture.width+x] = MapColors[v];
			}
		}

		MapTexture.SetPixels(ColorMap);
		MapTexture.Apply();
	}

	public float GetHumidity(Vector3 position, float altitude, float temperature){
		//float r = RainT * (Mathf.PerlinNoise(Rain * position.x, Rain * position.z));
		float Humidity = -0.5f * Mathf.Sin( (60.0f/Mathf.Pow(MapSize,2)) * Mathf.Pow(position.z-(MapSize/2),2) ) + WorldHumidity - (AltF*altitude);
		if(Humidity > 1.0f){
			Humidity = 1.0f;
		}
		if(Humidity < 0.0f){
			Humidity = 0.0f;
		}
		return Humidity;
	}
		

	public float GetTemperature(Vector3 position, float altitude){
		//float r = TempT * (Mathf.PerlinNoise(Temp * position.x, Temp * position.z));
		float Temperature = -(1.0f/(MapSize/2))*Mathf.Abs(position.z -(MapSize/2)) + WorldTemperature - (AltF*altitude);
		if(Temperature > 1.0f){
			Temperature = 1.0f;
		}
		if(Temperature < 0.0f){
			Temperature = 0.0f;
		}
		return Temperature;
	}

	public float GetAltitude(Vector3 position){
		float Base,Roughness,Detail,Negative;

		float Adjustment = -(Mathf.Pow(position.x -(MapSize/2),2)/Mathf.Pow(MapSize/2,2) + (Mathf.Pow(position.z-(MapSize/2),2)/Mathf.Pow(MapSize/2,2))) + WorldHeight;
		Base = BaseT * Mathf.PerlinNoise(MasterScale*NoiseScale1.x*(NoiseOrigin1.x+position.x),MasterScale*NoiseScale1.z*(NoiseOrigin1.z+position.z));
		Roughness = RoughT * Mathf.PerlinNoise(MasterScale*NoiseScale2.x*(NoiseOrigin2.x+position.x),MasterScale*NoiseScale2.z*(NoiseOrigin2.z+position.z));
		Detail = DetailT * Mathf.PerlinNoise(MasterScale*NoiseScale3.x*(NoiseOrigin3.x+position.x),MasterScale*NoiseScale3.z*(NoiseOrigin3.z+position.z));
		Negative = NegaT * Mathf.PerlinNoise(MasterScale*NoiseScale4.x*(NoiseOrigin4.x+position.x),MasterScale*NoiseScale4.z*(NoiseOrigin4.z+position.z));

		float Sample = Adjustment * ((Base+Roughness+Detail-Negative) * 0.5f);

		if(Sample < 0.0f){
			Sample = 0.0f;
		}
		if(Sample > 1.0f){
			Sample = 1.0f;
		}

		return Sample;
	}

	public int GetValue(Vector3 position){
		int Value = 13;

		float Altitude = GetAltitude(position);

		float Temperature = GetTemperature(position, Altitude);
		float Humidity = GetHumidity(position,Altitude,Temperature);

		if(Altitude >= SeaLevel){
			if(Temperature < 0.25f){					//Artic
				if(Altitude - SeaLevel < 0.025f){				//Coastal
					Value = 3;									//Ice
				}
				else if(Altitude < 0.6f){						//Plains
					if(Humidity < 0.33f){
						Value = 4;								//Artic Desert
					}
					else{
						Value = 5;								//Tundra
					}
				}
				else if(Altitude >= 0.6f && Altitude < 0.8f){	//Hills
					Value = 6;									//Tundra Hills 
				}
				else{										//Mountains
					Value = 7;									//Tundra Mountains
				}
			}
			else if(Temperature >= 0.25f && Temperature < 0.50f){	//Sub Artic
				if(Altitude - SeaLevel < 0.025f){							//Coastal
					Value = 8;												//Beach
				}
				else if(Altitude < 0.6f){									//Plains
					if(Humidity < 0.33f){
						Value = 9;											//Sub Artic Desert
					}
					else{
						Value = 10;											//Taiga
					}
				}
				else if(Altitude >= 0.6f && Altitude < 0.8f){				//Hills
					Value = 11;												//Sub Artic Hills
				}
				else{													//Mountains
					Value = 12;												//Sub Artic Mountains
				}
			}
			else if(Temperature >= 0.5f && Temperature < 0.80f){		//Temperate
				if(Altitude - SeaLevel < 0.025f){							//Coastal
					Value = 13;												//Beach
				}
				else if (Altitude - SeaLevel < 0.05f && Humidity > 0.66f){	
					Value = 14;												//Swamp
				}
				else if(Altitude < 0.6f){									//Plains
					if(Humidity < 0.33f){
						Value = 15;											//Desert
					}
					else if(Humidity >= 0.33f && Humidity < 0.66f){
						Value = 16;											//Grasslands
					}
					else{
						Value = 17;											//Forest
					}
				}
				else if(Altitude >= 0.6f && Altitude < 0.8f){				//Hills
					if(Humidity < 0.33f){
						Value = 18;											//Desert
					}
					else if(Humidity >= 0.33f && Humidity < 0.66f){
						Value = 19;											//Grasslands
					}
					else{
						Value = 20;											//Forest
					}
				}
				else{													//Mountains
					Value = 21;												//Temperate Mountains
				}
			}
			else if(Temperature >= 0.8f){								//Tropical
				if(Altitude - SeaLevel < 0.025f){							//Coastal
					Value = 22;												//Beach
				}
				else if (Altitude - SeaLevel < 0.05f && Humidity > 0.66f){	
					Value = 23;												//Swamp
				}
				else if(Altitude < 0.6f){									//Plains
					if(Humidity < 0.20f){
						Value = 24;											//Desert
					}
					else if(Humidity >= 0.20f && Humidity < 0.40f){
						Value = 25;											//Shrubland
					}
					else if(Humidity >= 0.40f && Humidity < 0.60f){
						Value = 26;											//Savannah
					}
					else if(Humidity >= 0.60f && Humidity < 0.8f){
						Value = 27;											//Forest
					}
					else{
						Value = 28;											//Rainforest
					}
				}
				else if(Altitude >= 0.6f && Altitude < 0.8f){				//Hills
					if(Humidity < 0.20f){
						Value = 29;											//Desert
					}
					else if(Humidity >= 0.20f && Humidity < 0.40f){
						Value = 30;											//Shrubland
					}
					else if(Humidity >= 0.40f && Humidity < 0.60f){
						Value = 31;											//Savannah
					}
					else if(Humidity >= 0.60f && Humidity < 0.8f){
						Value = 32;											//Forest
					}
					else{
						Value = 33;											//Rainforest
					}
				}
				else{													//Mountains
					Value = 34;												//Tropical Mountains
				}
			}

			//In case of error
			else{
				Value = 0;		//Error
			}
		}
		else{
			if(Altitude < (SeaLevel*0.5f)){
				Value = 1;		//Deep Ocean
			}
			else{

			}
			Value = 2;		//Shallow Ocean
		}

		return Value;
	}//End GetValue()

	public void AddCities(int number){
		int i = 0;
		int dx = MapSize / 4;
		int dy = MapSize / 4;

		while(i < number){
			int x = Random.Range (dx, 3 * dx);
			int y = Random.Range (dy, 3 * dy);

			float a = GetAltitude (new Vector3 (x, 0, y));

			if (a >= SeaLevel) {
				MapTexture.SetPixel (x,y,Color.red);
				i++;
			}
		}

		MapTexture.Apply ();
	}
}
