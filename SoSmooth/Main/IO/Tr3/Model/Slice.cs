using System.IO;
using OpenTK;

namespace SoSmooth.IO.tr3
{
    /// <summary>
    /// Represents an image slice.
    /// </summary>
    public class Slice
    {
        private int m_index;
        private FileInfo m_imgFile;
        private float m_z;
        private float m_rotate;
        private Vector2 m_offset;

        public Slice(int index, string imgPath, float z, float rotate, float offsetX, float offsetY)
        {
            m_index = index;
            if (index < 0)
            {
                Logger.Error("Slice index is negative: " + index);
            }

            m_imgFile = new FileInfo(imgPath);
            if (!m_imgFile.Exists)
            {
                Logger.Error("Slice image at path \"" + m_imgFile.FullName + "\" does not exist!");
            }

            m_z = z;
            m_rotate = rotate;
            m_offset = new Vector2(offsetX, offsetY);
        }
    }
}
