using System;
using System.Collections.Generic;
using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
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
        private GameObject gridObject;
        private Tilemap map;
        private Tilemap entities;

        private Sprite[] glyphs;

        private Dictionary<Entity, Tile> tiles = new Dictionary<Entity, Tile>();
        private Tile floorTile = new Tile();
        private Tile wallTile = new Tile();

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
                "Packages/za.co.lazygamedev.rogue-dots.terminal-renderer/LazyGameDevZA.RogueDOTS.Terminal.Rendering/Font/terminal16x16_gs_ro.png");
            this.glyphs = Array.ConvertAll(objects, item => (Sprite)item);

            this.floorTile.sprite = this.glyphs[(byte)'.'];
            this.floorTile.color = new Color(0.5f, 0.5f, 0.5f);

            this.wallTile.sprite = this.glyphs[(byte)'#'];
            this.wallTile.color = new Color(0f, 1f, 0f);
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            this.map.ClearAllTiles();
            
            this.Entities.ForEach((DynamicBuffer<RogueTile> map) =>
                {
                    int x = 0;
                    int y = 0;
                    foreach(var tile in map)
                    {
                        switch(tile.Type)
                        {
                            case (TileType.Floor):
                                this.map.SetTile(new Vector3Int(x, y, 0), this.floorTile);
                                break;
                            case (TileType.Wall):
                                this.map.SetTile(new Vector3Int(x, y, 0), this.wallTile);
                                break;
                        }

                        x += 1;

                        if(x > 79)
                        {
                            x = 0;
                            y += 1;
                        }
                    }
                })
                .WithoutBurst()
                .Run();
            
            this.entities.ClearAllTiles();
            
            this.Entities.ForEach((in Entity entity, in Position position, in Renderable renderable) =>
                {
                    if(!this.tiles.TryGetValue(entity, out var tile))
                    {
                        this.tiles.Add(entity, tile = new Tile());
                    }        
                    
                    tile.sprite = this.glyphs[renderable.Glyph];
                    tile.color = renderable.Foreground;

                    this.entities.SetTile(new Vector3Int(position.X, position.Y, 0), tile);
                })
                .WithoutBurst()
                .Run();

            return inputDependencies;
        }

        protected override void OnDestroy()
        {
            Object.Destroy(this.gridObject);
        }
    }
}
