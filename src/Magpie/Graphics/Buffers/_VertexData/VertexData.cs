using System.Numerics;

namespace Magpie.Graphics.Buffers;

public struct VertexData(Vector3 position, Vector3 normal, Vector3 texCoord) {
    public Vector3 Position = position;
    public Vector3 Normal = normal;
    public Vector3 TextureCoordinates = texCoord;
}