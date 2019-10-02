
using UnityEngine;

public static class Noise {
    /*
     * Generate noise map and return 2D array
     */
    public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        var noiseMap = new float[width, height];
        var randomGenerator = new System.Random(seed);
        
        var octaveOffsets = new Vector2[octaves];
        for (var i = 0; i < octaves; i++) {
            var offsetX = randomGenerator.Next(-100000, 100000) + offset.x;
            var offsetY = randomGenerator.Next(-100000, 100000) + offset.y;
            
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0.0f)
            scale = 0.0001f;

        var minNoiseHeight = float.MaxValue;
        var maxNoiseHeight = float.MinValue;
        
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var amplitude = 1.0f;
                var frequency = 1.0f;
                var noiseHeight = 0.0f;
                
                for (var i = 0; i < octaves; i++) {
                    var sampleX = (x - width / 2) / scale * frequency + octaveOffsets[i].x;
                    var sampleY = (y - height / 2) / scale * frequency + octaveOffsets[i].y;

                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                
                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;
                    
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
