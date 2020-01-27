using System;
using System.Collections.Generic;
using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Tile = UnityEngine.Tilemaps.Tile;
using RogueTile = LazyGameDevZA.RogueDOTS.Components.Tile;

namespace LazyGameDevZA.RogueDOTS.TerminalRenderer
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [AlwaysSynchronizeSystem]
    public class RenderWorldSystem : JobComponentSystem
    {
        private static ProfilerMarker renderMapMarker = new ProfilerMarker($"{nameof(RenderWorldSystem)}_RenderMap"); 
        private static ProfilerMarker renderEntitiesMarker = new ProfilerMarker($"{nameof(RenderWorldSystem)}_RenderEntities");
        
        private GameObject gridObject;
        private Tilemap map;
        private Tilemap entities;

        private Sprite[] glyphs;

        private Dictionary<Entity, Tile> tiles = new Dictionary<Entity, Tile>();
        private readonly Tile floorTile = ScriptableObject.CreateInstance<Tile>();
        private readonly Tile wallTile = ScriptableObject.CreateInstance<Tile>();
        private readonly Tile greyFloorTile = ScriptableObject.CreateInstance<Tile>();
        private readonly Tile greyWallTile = ScriptableObject.CreateInstance<Tile>();

        private EntityQuery mapQuery;

        protected override void OnCreate()
        {
            this.gridObject = new GameObject("Grid");
            var grid = gridObject.AddComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;

            var mapObject = new GameObject("Map");
            mapObject.transform.parent = grid.transform;
            this.map = mapObject.AddComponent<Tilemap>();
            var mapRenderer = mapObject.AddComponent<TilemapRenderer>();
            mapRenderer.sortingOrder = 0;

            var entitiesObject = new GameObject("Entities");
            entitiesObject.transform.parent = grid.transform;
            this.entities = entitiesObject.AddComponent<Tilemap>();
            var entitiesRenderer = entitiesObject.AddComponent<TilemapRenderer>();
            entitiesRenderer.sortingOrder = 1;

            var objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(
                "Packages/za.co.lazygamedev.rogue-dots/LazyGameDevZA.RogueDOTS.Terminal.Rendering/Font/terminal16x16_gs_ro.png");
            this.glyphs = Array.ConvertAll(objects, item => (Sprite)item);
            
            var floorTileColor = new Renderable.Colour(0f, 0.5f, 0.5f);
            var greyFloorTileSprite = this.glyphs[(byte)'.'];
            this.floorTile.sprite = greyFloorTileSprite;
            this.floorTile.color = floorTileColor;
            this.greyFloorTile.sprite = greyFloorTileSprite;
            this.greyFloorTile.color = floorTileColor.ToGreyscale();

            var wallTileColor = new Renderable.Colour(0f, 1f, 0f);
            var wallTileSprite = this.glyphs[(byte)'#'];
            this.wallTile.sprite = wallTileSprite;
            this.wallTile.color = wallTileColor;
            this.greyWallTile.sprite = wallTileSprite;
            this.greyWallTile.color = wallTileColor.ToGreyscale();

            this.mapQuery = this.EntityManager.CreateMapEntityQuery();
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            
            this.Job
                .WithCode(() =>
                {
                    renderMapMarker.Begin();
                    this.map.ClearAllTiles();
                    
                    var x = 0;
                    var y = 0;

                    RogueTile tile;
                    for(int idx = 0; idx < map.Tiles.Length; idx++)
                    {
                        tile = map.Tiles[idx];

                        if(map.RevealedTiles[idx])
                        {
                            switch(tile.Type)
                            {
                                case (TileType.Floor):
                                    var floorTile = map.VisibleTiles[idx] ? this.floorTile : this.greyFloorTile;
                                    this.map.SetTile(new Vector3Int(x, y, 0), floorTile);
                                    break;
                                case (TileType.Wall):
                                    var wallTile = map.VisibleTiles[idx] ? this.wallTile : this.greyWallTile;
                                    this.map.SetTile(new Vector3Int(x, y, 0), wallTile);
                                    break;
                            }
                        }

                        x += 1;

                        if(x > map.Width - 1)
                        {
                            x = 0;
                            y += 1;
                        }
                    }
                    renderMapMarker.End();
                })
                .WithoutBurst()
                .Run();
            
            renderEntitiesMarker.Begin();
            this.entities.ClearAllTiles();
            
            this.Entities
                .ForEach((in Entity entity, in Position position, in Renderable renderable) =>
                {
                    if(!this.tiles.TryGetValue(entity, out var tile))
                    {
                        this.tiles.Add(entity, tile = ScriptableObject.CreateInstance<Tile>());
                    }        
                    
                    tile.sprite = this.glyphs[renderable.Glyph];
                    tile.color = renderable.Foreground;

                    this.entities.SetTile(position.Value.AsVector3Int(), tile);
                })
                .WithChangeFilter<Position>()
                .WithoutBurst()
                .Run();
            renderEntitiesMarker.End();

            return default;
        }

        protected override void OnDestroy()
        {
            Object.Destroy(this.gridObject);
        }
    }
}
