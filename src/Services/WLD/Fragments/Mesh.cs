﻿using EQTool.Models;
using System;
using System.Collections.Generic;

namespace EQTool.Services.WLD.Fragments
{
    /// <summary>
    /// Mesh (0x36)
    /// Internal name: _DMSPRITEDEF
    /// Contains geometric data for a mesh.
    /// </summary>
    public class Mesh : WldFragment
    {
        /// <summary>
        /// The center of the mesh - used to calculate absolute coordinates of vertices
        /// I used to store absolute coordinates but animated meshes use coordinates relative to the center so we have to keep it separate
        /// </summary>
        public vec3 Center { get; private set; }

        /// <summary>
        /// The maximum distance between the center and any vertex - bounding radius
        /// </summary>
        public float MaxDistance { get; private set; }

        /// <summary>
        /// The minimum vertex positions in the model - used for bounding box
        /// </summary>
        public vec3 MinPosition { get; private set; }

        /// <summary>
        /// The maximum vertex positions in the model - used for bounding box
        /// </summary>
        public vec3 MaxPosition { get; private set; }

        /// <summary>
        /// The texture list used to render this mesh
        /// In zone meshes, it's always the same one
        /// In object meshes, it can be unique
        /// </summary>
        public MaterialList MaterialList { get; private set; }

        /// <summary>
        /// The vertices of the mesh
        /// </summary>
        public List<vec3> Vertices { get; set; }

        /// <summary>
        /// The normals of the mesh
        /// </summary>
        public List<vec3> Normals { get; private set; }

        /// <summary>
        /// The polygon indices of the mesh
        /// </summary>
        public List<Polygon> Indices { get; private set; }

        public List<Color> Colors { get; set; }

        /// <summary>
        /// The UV texture coordinates of the vertex
        /// </summary>
        public List<vec2> TextureUvCoordinates { get; private set; }

        /// <summary>
        /// The mesh render groups
        /// Defines which texture index corresponds with groups of vertices
        /// </summary>
        public List<RenderGroup> MaterialGroups { get; private set; }

        /// <summary>
        /// Set to true if there are non solid polygons in the mesh
        /// This means we export collision separately (e.g. trees, fire)
        /// </summary>
        public bool ExportSeparateCollision { get; private set; }

        public bool IsHandled = false;

        public int StartTextureIndex = 0;

        /// <summary>
        /// The render components of a mob skeleton
        /// </summary>
        public Dictionary<int, MobVertexPiece> MobPieces { get; private set; }

        public override void Initialize(int index, int size, byte[] data,
            List<WldFragment> fragments,
            Dictionary<int, string> stringHash, bool isNewWldFormat)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat);
            Name = stringHash[-Reader.ReadInt32()];

            // Zone: 0x00018003, Objects: 0x00014003
            _ = Reader.ReadInt32();

            MaterialList = fragments[Reader.ReadInt32() - 1] as MaterialList;
            _ = Reader.ReadInt32();
            _ = Reader.ReadInt32();

            // maybe references the first 0x03 in the WLD - unknown
            _ = Reader.ReadInt32();

            Center = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());

            // 3 unknown dwords
            var unknownDword1 = Reader.ReadInt32();
            var unknownDword2 = Reader.ReadInt32();
            var unknownDword3 = Reader.ReadInt32();

            // Seems to be related to lighting models? (torches, etc.)
            if (unknownDword1 != 0 || unknownDword2 != 0 || unknownDword3 != 0)
            {

            }

            MaxDistance = Reader.ReadSingle();
            MinPosition = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            MaxPosition = new vec3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());

            Vertices = new List<vec3>();
            Normals = new List<vec3>();
            Colors = new List<Color>();
            TextureUvCoordinates = new List<vec2>();

            var vertexCount = Reader.ReadInt16();
            var textureCoordinateCount = Reader.ReadInt16();
            var normalsCount = Reader.ReadInt16();
            var colorsCount = Reader.ReadInt16();
            var polygonCount = Reader.ReadInt16();
            var vertexPieceCount = Reader.ReadInt16();
            var polygonTextureCount = Reader.ReadInt16();
            var vertexTextureCount = Reader.ReadInt16();
            var size9 = Reader.ReadInt16();
            var scale = 1.0f / (1 << Reader.ReadInt16());

            for (var i = 0; i < vertexCount; ++i)
            {
                Vertices.Add(new vec3(Reader.ReadInt16() * scale, Reader.ReadInt16() * scale,
                    Reader.ReadInt16() * scale));
            }

            for (var i = 0; i < textureCoordinateCount; ++i)
            {
                if (isNewWldFormat)
                {
                    TextureUvCoordinates.Add(new vec2(Reader.ReadInt32() / 256.0f, Reader.ReadInt32() / 256.0f));
                }
                else
                {
                    TextureUvCoordinates.Add(new vec2(Reader.ReadInt16() / 256.0f, Reader.ReadInt16() / 256.0f));
                }
            }

            for (var i = 0; i < normalsCount; ++i)
            {
                var x = Reader.ReadSByte() / 128.0f;
                var y = Reader.ReadSByte() / 128.0f;
                var z = Reader.ReadSByte() / 128.0f;
                Normals.Add(new vec3(x, y, z));
            }

            for (var i = 0; i < colorsCount; ++i)
            {
                var colorBytes = BitConverter.GetBytes(Reader.ReadInt32());
                int b = colorBytes[0];
                int g = colorBytes[1];
                int r = colorBytes[2];
                int a = colorBytes[3];

                Colors.Add(new Color(r, g, b, a));
            }

            Indices = new List<Polygon>();

            for (var i = 0; i < polygonCount; ++i)
            {
                var isSolid = Reader.ReadInt16() == 0;

                if (!isSolid)
                {
                    ExportSeparateCollision = true;
                }

                Indices.Add(new Polygon()
                {
                    IsSolid = isSolid,
                    Vertex1 = Reader.ReadInt16(),
                    Vertex2 = Reader.ReadInt16(),
                    Vertex3 = Reader.ReadInt16(),
                });
            }

            MobPieces = new Dictionary<int, MobVertexPiece>();
            var mobStart = 0;

            for (var i = 0; i < vertexPieceCount; ++i)
            {
                int count = Reader.ReadInt16();
                int index1 = Reader.ReadInt16();
                var mobVertexPiece = new MobVertexPiece
                {
                    Count = count,
                    Start = mobStart
                };

                mobStart += count;

                MobPieces[index1] = mobVertexPiece;
            }

            MaterialGroups = new List<RenderGroup>();

            StartTextureIndex = int.MaxValue;

            for (var i = 0; i < polygonTextureCount; ++i)
            {
                var group = new RenderGroup
                {
                    PolygonCount = Reader.ReadUInt16(),
                    MaterialIndex = Reader.ReadUInt16()
                };
                MaterialGroups.Add(group);

                if (group.MaterialIndex < StartTextureIndex)
                {
                    StartTextureIndex = group.MaterialIndex;
                }
            }

            for (var i = 0; i < vertexTextureCount; ++i)
            {
                Reader.BaseStream.Position += 4;
            }

            for (var i = 0; i < size9; ++i)
            {
                Reader.BaseStream.Position += 12;
            }

            // In some rare cases, the number of uvs does not match the number of vertices
            if (Vertices.Count != TextureUvCoordinates.Count)
            {
                var difference = Vertices.Count - TextureUvCoordinates.Count;

                for (var i = 0; i < difference; ++i)
                {
                    TextureUvCoordinates.Add(new vec2(0.0f, 0.0f));
                }
            }
        }

        public void ClearCollision()
        {
            foreach (var poly in Indices)
            {
                poly.IsSolid = false;
            }

            ExportSeparateCollision = true;
        }
    }
}