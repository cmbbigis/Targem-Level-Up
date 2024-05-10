using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Fraction;
using Map.Models.Terrain;
using Resources.Models.Resource;
using Units.Models.Unit;
using UnityEngine;

namespace Core
{
    public class GameSettingsManager: MonoBehaviour
    {
        [SerializeField]
        public int playersCount;
        
        [SerializeField]
        public int mapWidth;
        
        [SerializeField]
        public int mapHeight;
        
        [SerializeField]
        public FractionType[] playerFractions;
        
        [SerializeField]
        public string[] playerNames;
        
        [SerializedDictionary("TerrainType", "Weight")]
        public SerializedDictionary<TerrainType, int> biomeWeights;

        [SerializeField]
        public int resourceCount;

        [SerializedDictionary]
        public SerializedDictionary<TerrainType, List<ResourceType>> biomeResources;
        
        [SerializedDictionary]
        public SerializedDictionary<ResourceType, int> resourcesCount;
    }
}