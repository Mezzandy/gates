using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FileManager {

	public string SaveDirectory = "World";
	public BinaryReader FileBinReader;
	public BinaryWriter FileBinWriter;

	public TileChunk LoadTileChunkFromFile(Vector2 ChunkPosition, int Seed){
		TileChunk LoadedFile = new TileChunk ();

		string FileName = Application.dataPath + "\\World-" + Seed.ToString () + "\\" + ChunkPosition.x.ToString () + "," + ChunkPosition.y.ToString () + ".tc";

		//Create File if it doesn't exist
		if(!File.Exists(FileName)){
			Debug.Log ("File does not exist");
		}

		FileBinReader = new BinaryReader (File.OpenRead (FileName));

		LoadedFile.Position.x = FileBinReader.ReadSingle();
		LoadedFile.Position.y = FileBinReader.ReadSingle();
		LoadedFile.Width = FileBinReader.ReadInt32();
		LoadedFile.Height = FileBinReader.ReadInt32();
	
		LoadedFile.Layers = new bool[8];
		LoadedFile.LayerData = new TileData[8];

		for (int i = 0; i < LoadedFile.Layers.Length; i++) {
			LoadedFile.Layers [i] = FileBinReader.ReadBoolean ();
			LoadedFile.LayerData [i] = new TileData ();
		}

		for (int i = 0; i < LoadedFile.LayerData.Length; i++) {
			LoadedFile.LayerData [i].MaterialID = FileBinReader.ReadInt32 ();
			for (int x = 0; x < LoadedFile.Width; x++) {
				for (int y = 0; y < LoadedFile.Height; y++) {
					LoadedFile.LayerData [i].Data [y * LoadedFile.Width + x] = FileBinReader.ReadInt32 ();
				}
			}
		}
		FileBinReader.Close ();

		Debug.Log ("File " + FileName + " loaded");

		return LoadedFile;
	}

	public void SaveTileChunkToFile(TileChunk Chunk, int Seed){
		//Ask user if they want to continue as saving will overwrite all previous written chunks

		//Create directory if it doesn't exist
		string NewSaveDirectory = Application.dataPath + "\\World-" + Seed.ToString ();
		if (!Directory.Exists (NewSaveDirectory)) {
			Directory.CreateDirectory (NewSaveDirectory);
			Debug.Log ("Directory Created");
		}

		string FileName = Chunk.Position.x.ToString () + "," + Chunk.Position.y.ToString () + ".tc";

		//Create File if it doesn't exist
		if(!File.Exists(NewSaveDirectory + "\\" + FileName)){
			//File.Create (NewSaveDirectory + "\\" + FileName);
			File.Create (NewSaveDirectory + "\\" + FileName).Dispose ();
		}

		FileBinWriter = new BinaryWriter (File.Open (NewSaveDirectory + "\\" + FileName, FileMode.Open));

		FileBinWriter.Write (Chunk.Position.x);
		FileBinWriter.Write (Chunk.Position.y);
		FileBinWriter.Write (Chunk.Width);
		FileBinWriter.Write (Chunk.Height);

		for (int i = 0; i < Chunk.Layers.Length; i++) {
			FileBinWriter.Write (Chunk.Layers [i]);
		}

		for (int i = 0; i < Chunk.LayerData.Length; i++) {
			FileBinWriter.Write (Chunk.LayerData [i].MaterialID);
			for (int x = 0; x < Chunk.Width; x++) {
				for (int y = 0; y < Chunk.Height; y++) {
					FileBinWriter.Write (Chunk.LayerData[i].Data[y * Chunk.Width + x]);
				}
			}
		}
		FileBinWriter.Close ();
		Debug.Log ("File " + FileName + " written");
	}
}
