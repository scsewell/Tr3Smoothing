namespace SoSmooth.Renderering
{
    public interface IVertexAttributeProvider<TVertexData> where TVertexData : struct, IVertexData
    {
        void SetVertexData();
        void UnSetVertexData();
        void SetShaderProgram(ShaderProgram program);
    }
}
