using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public enum DrawMode {
        NoiseMap,
        ColourMap,
        Mesh
    }

    public DrawMode drawMode = DrawMode.NoiseMap;
    
    
    public int mapWidth = 10;
    public int mapHeight = 10;
    public float noiseScale = 0.5f;
    
    public int seed;
    public int octaves = 1;
    
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    
    public Vector2 offset = new Vector2(0, 0);
    public float meshHeightMultiplier = 1f;
    public bool autoUpdate = true;

    public TerrainType[] regions;

    public void GenerateMap() {
        var noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        var colourMap = new Color[mapWidth * mapHeight];
        for (var y = 0; y < mapHeight; y++) {
            for (var x = 0; x < mapWidth; x++) {
                var currentHeight = noiseMap[x, y];

                for (var i = 0; i < regions.Length; i++) {
                    if (currentHeight > regions[i].height)
                        continue;

                    colourMap[y * mapWidth + x] = regions[i].colour;
                    break;
                }
            }
        }

        var display = FindObjectOfType<MapDisplay>();

        switch (drawMode) {
            case DrawMode.NoiseMap: 
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                break;
            
            case DrawMode.ColourMap:
                display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
                break;
            
            case DrawMode.Mesh:
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnValidate() {
        if (mapWidth < 1)
            mapWidth = 1;

        if (mapHeight < 1)
            mapHeight = 1;

        if (octaves < 1)
            octaves = 1;

        if (lacunarity < 1)
            lacunarity = 1;
    }
}

[Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;
}
