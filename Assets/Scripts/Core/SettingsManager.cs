using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Map;
using Map.Models.Hex;
using Map.Models.Terrain;
using Players.Models.Player;
using Resources.Models.Resource;
using Units.Models.Unit;
using Units.Models.Unit.Units;
using UnityEngine;

namespace Core
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializedDictionary("UnitType", "Prefab")]
        public SerializedDictionary<UnitType, GameObject> unitPrefabByUnitType;
        
        [SerializedDictionary("TerrainType", "Sprites")]
        public SerializedDictionary<TerrainType, Sprite[]> hexSprites;
        
        [SerializedDictionary("TerrainType", "Sprite")]
        public SerializedDictionary<TerrainType, Sprite> hexUnderSprites;
        
        [SerializedDictionary("TerrainType", "Sprites")]
        public SerializedDictionary<TerrainType, SerializedDictionary<ResourceType, Sprite[]>> resourceHexSprites;
            
        [SerializedDictionary("TerrainType", "Sprites")]
        public SerializedDictionary<TerrainType, SerializedDictionary<ResourceType, Sprite[]>> miningBuildingHexSprites;

        public GameObject GetUnitPrefab(UnitType type) => unitPrefabByUnitType[type];
        public Sprite[] GetHexSprites(TerrainType terrainType) => hexSprites[terrainType];
        public Sprite GetHexUnderSprites(TerrainType terrainType) => hexUnderSprites[terrainType];
        public Sprite[] GetResourceHexSprites(TerrainType terrainType, ResourceType resourceType) =>
            resourceHexSprites[terrainType][resourceType];
        public Sprite[] GetMiningBuildingHexSprites(TerrainType terrainType, ResourceType resourceType) =>
            miningBuildingHexSprites[terrainType][resourceType];
    }
}