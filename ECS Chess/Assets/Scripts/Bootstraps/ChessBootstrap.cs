using ECSChess.Components.Chess;
using ECSChess.Misc.Enums;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace ECSChess.Bootstraps
{
    /// <summary>
    /// Sets up a ChessBoard, by spawning a Board, and ChessPieces
    /// </summary>
    public static class ChessBootstrap
    {
        #region Variables
        /// <summary>
        /// Meshes for Pieces
        /// </summary>
        private static readonly Dictionary<ChessPiece, Mesh> PieceMeshes = new Dictionary<ChessPiece, Mesh>(6);
        /// <summary>
        /// Materials for Teams
        /// </summary>
        private static readonly Dictionary<ChessTeam, Material> TeamMaterials = new Dictionary<ChessTeam, Material>();
        /// <summary>
        /// Archetype for Piece-Entities
        /// </summary>
        private static EntityArchetype pieceArchetype;
        /// <summary>
        /// Archetype for BoardTile-Entities
        /// </summary>
        private static EntityArchetype boardTileArchetype;
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Loads Prefabs from Resources, and
        /// creates Entity-Archetype
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnBeforeSceneLoad()
        {
            foreach (ChessPiece piece in Enum.GetValues(typeof(ChessPiece)))
                PieceMeshes.Add(piece, Resources.Load<Mesh>(string.Concat(@"Models\", piece.ToString())));
            foreach (ChessTeam team in Enum.GetValues(typeof(ChessTeam)))
                TeamMaterials.Add(team, Resources.Load<Material>(string.Concat(@"Materials\", team.ToString())));
            EntityManager entityManager = World.Active.EntityManager;
            pieceArchetype = entityManager.CreateArchetype(
                typeof(Scale), typeof(Translation), typeof(Rotation), typeof(LocalToWorld), // Transform Data
                typeof(Piece), typeof(Team),  // Chess-Data
                typeof(RenderMesh), // Render-Data
                typeof(WorldRenderBounds)); // Collision-Data 
            boardTileArchetype = entityManager.CreateArchetype(
                typeof(Translation), typeof(Rotation),
                typeof(RenderMesh),
                typeof(LocalToWorld));
        }

        /// <summary>
        /// Creates Entities for Pieces
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnAfterSceneLoad()
        {
            EntityManager mgr = World.Active.EntityManager;
            SpawnChessBoard(mgr);
            foreach (ChessTeam team in Enum.GetValues(typeof(ChessTeam)))
            {
                quaternion rotation = team.Equals(ChessTeam.White) ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
                float y = 0;
                float z = team.Equals(ChessTeam.White) ? 0 : 7;
                // 1 King
                SpawnChessPiece(mgr, team, ChessPiece.King, new float3(4, y, z), rotation);
                // 1 Queen
                SpawnChessPiece(mgr, team, ChessPiece.Queen, new float3(3, y, z), rotation);
                // 2 Rooks
                SpawnChessPiece(mgr, team, ChessPiece.Rook, new float3(0, y, z), rotation);
                SpawnChessPiece(mgr, team, ChessPiece.Rook, new float3(7, y, z), rotation);
                // 2 Bishops
                SpawnChessPiece(mgr, team, ChessPiece.Bishop, new float3(2, y, z), rotation);
                SpawnChessPiece(mgr, team, ChessPiece.Bishop, new float3(5, y, z), rotation);
                // 2 Knights
                SpawnChessPiece(mgr, team, ChessPiece.Knight, new float3(1, y, z), rotation);
                SpawnChessPiece(mgr, team, ChessPiece.Knight, new float3(6, y, z), rotation);
                // 8 Pawns
                z = team.Equals(ChessTeam.White) ? 1 : 6;
                for (int i = 0; i < 8; i++)
                    SpawnChessPiece(mgr, team, ChessPiece.Pawn, new float3(i, y, z), rotation);
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// Spawns a Chess-Board (by placing Tiles)
        /// </summary>
        /// <param name="mgr">EntityManager to use for spawning</param>
        private static void SpawnChessBoard(EntityManager mgr)
        {
            Mesh quadMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            RenderMesh blackInfo = new RenderMesh
            {
                castShadows = ShadowCastingMode.Off,
                receiveShadows = true,
                mesh = quadMesh,
                material = TeamMaterials[ChessTeam.Black]
            };
            RenderMesh whiteInfo = new RenderMesh
            {
                castShadows = ShadowCastingMode.Off,
                receiveShadows = true,
                mesh = quadMesh,
                material = TeamMaterials[ChessTeam.White]
            };
            bool black = true;
            Quaternion rotation = Quaternion.Euler(90, 0, 0);
            for (int z = 0; z < 8; z++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Entity pieceEntity = mgr.CreateEntity(boardTileArchetype);
                    mgr.SetComponentData(pieceEntity, new Translation { Value = new float3(x, 0, z) });
                    mgr.SetComponentData(pieceEntity, new Rotation { Value = rotation });
                    mgr.SetSharedComponentData(pieceEntity, black ? blackInfo : whiteInfo);
#if UNITY_EDITOR
                    mgr.SetName(pieceEntity, string.Format("BoardTile-{0}_{1}", x, z)); // Set name to be shown in EntityDebugger
#endif
                    black = !black;
                }
                black = !black;
            }
        }

        /// <summary>
        /// Spawns a Chess-Piece
        /// </summary>
        /// <param name="mgr">EntityManager</param>
        /// <param name="team">Team for Piece</param>
        /// <param name="piece">Type of Piece to Spawn</param>
        /// <param name="position">Position for Piece</param>
        /// <param name="rotation">Rotation for Piece</param>
        /// <returns>Spawned Entity</returns>
        private static Entity SpawnChessPiece(EntityManager mgr, ChessTeam team, ChessPiece piece, float3 position, quaternion rotation)
        {
            Entity pieceEntity = mgr.CreateEntity(pieceArchetype);
            mgr.SetComponentData(pieceEntity, new Translation { Value = position });
            mgr.SetComponentData(pieceEntity, new Rotation { Value = rotation });
            mgr.SetComponentData(pieceEntity, new Scale { Value = 1.5f });
            mgr.SetComponentData(pieceEntity, new Team(team));
            mgr.SetComponentData(pieceEntity, new Piece(piece));
            RenderMesh renderInfo = new RenderMesh
            {
                castShadows = ShadowCastingMode.On,
                receiveShadows = true,
                mesh = PieceMeshes[piece]
            };
            renderInfo.material = TeamMaterials[team]; // Set Material (Color) for Team
            mgr.SetSharedComponentData(pieceEntity, renderInfo);
#if UNITY_EDITOR
            mgr.SetName(pieceEntity, string.Format("{0}_{1}", piece.ToString(), team.ToString())); // Set name to be shown in EntityDebugger
#endif
            return pieceEntity;
        }
        #endregion
        #endregion
    }
}