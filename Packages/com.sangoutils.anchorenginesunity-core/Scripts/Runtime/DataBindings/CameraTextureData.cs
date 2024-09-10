namespace SangoUtils.AnchorEngines_Unity.Core
{
    internal class CameraTextureData
    {
        internal CameraTextureData(int width, int height, byte[] data)
        {
            Width = width;
            Height = height;
            Data = data;
        }

        internal int Width { get; }
        internal int Height { get; }
        internal byte[] Data { get; set; }
    }
}
