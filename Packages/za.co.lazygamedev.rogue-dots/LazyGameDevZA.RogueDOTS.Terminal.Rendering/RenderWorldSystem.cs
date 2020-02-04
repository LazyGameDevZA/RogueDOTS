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
using RogueTile = LazyGameDevZA.RogueDOTS.Components.Tile;
using Tile = UnityEngine.Tilemaps.Tile;

namespace LazyGameDevZA.RogueDOTS.TerminalRenderer
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [AlwaysSynchronizeSystem]
    public class RenderWorldSystem : JobComponentSystem
    {
        private static ProfilerMarker renderMapMarker = new ProfilerMarker($"{nameof(RenderWorldSystem)}_RenderMap");
        private static ProfilerMarker renderEntitiesMarker = new ProfilerMarker($"{nameof(RenderWorldSystem)}_RenderEntities");
        private readonly Tile floorTile = ScriptableObject.CreateInstance<Tile>();
        private readonly Tile wallTile = ScriptableObject.CreateInstance<Tile>();
        private readonly Tile greyFloorTile = ScriptableObject.CreateInstance<Tile>();
        private readonly Tile greyWallTile = ScriptableObject.CreateInstance<Tile>();

        private GameObject gridObject;
        private Tilemap map;
        private Tilemap entitiesBackground;
        private Tilemap entitiesForeground;

        private Sprite[] glyphs;

        private readonly Dictionary<Entity, (Tile foregroundTile, Tile backgroundTile)> tiles = new Dictionary<Entity, (Tile foregroundTile, Tile backgroundTile)>();

        private EntityQuery mapQuery;
        
        protected override void OnCreate()
        {
            this.gridObject = new GameObject("Grid");
            var grid = gridObject.AddComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;

            this.map = CreateLayer("Map Foreground", 0, this.gridObject);
            this.entitiesBackground = CreateLayer("Entities Background", 1, this.gridObject);
            this.entitiesForeground = CreateLayer("Entities Foreground", 2, this.gridObject);

            var objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(
                "Packages/za.co.lazygamedev.rogue-dots/LazyGameDevZA.RogueDOTS.Terminal.Rendering/Font/terminal8x8.png");
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
            this.entitiesForeground.ClearAllTiles();
            this.entitiesBackground.ClearAllTiles();

            this.Entities
                .ForEach((in Entity entity, in Position position, in Renderable renderable) =>
                {
                    var idx = map.xy_idx(position.Value);

                    if(map.VisibleTiles[idx].Value)
                    {
                        if(!this.tiles.TryGetValue(entity, out var tileInfo))
                        {
                            var foregroundTile = ScriptableObject.CreateInstance<Tile>();
                            var backgroundTile = ScriptableObject.CreateInstance<Tile>();
                            backgroundTile.sprite = this.glyphs[219]; // 219 = █
                            
                            this.tiles.Add(entity, tileInfo = (foregroundTile, backgroundTile));
                        }

                        tileInfo.foregroundTile.sprite = this.glyphs[renderable.Glyph];
                        tileInfo.foregroundTile.color = renderable.Foreground;

                        tileInfo.backgroundTile.color = renderable.Background;

                        var pos = position.Value.AsVector3Int();
                        this.entitiesForeground.SetTile(pos, tileInfo.foregroundTile);
                        this.entitiesBackground.SetTile(pos, tileInfo.backgroundTile);
                    }
                })
                .WithoutBurst()
                .Run();
            renderEntitiesMarker.End();

            return default;
        }

        protected override void OnDestroy()
        {
            Object.Destroy(this.gridObject);
        }

        private static Tilemap CreateLayer(string name, int layer, GameObject parent)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.parent = parent.transform;
            var tilemap = gameObject.AddComponent<Tilemap>();
            var renderer = gameObject.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = layer;

            return tilemap;
        }
    }
}
