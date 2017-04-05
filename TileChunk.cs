using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileChunk {

	public Vector2 Position;
	public int Width = 16;
	public int Height = 16;
	public bool[] Layers = new bool[8];
	public TileData[] LayerData;

	public int GetTileID(int Layer, int x, int y){
		return LayerData [Layer].Data [y * 16 + x];
	}

	public void Intitialize(){

	}
}
