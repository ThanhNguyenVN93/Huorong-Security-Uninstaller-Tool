using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;

namespace frm_HIS_uninstaller_tool
{
    internal static class ResourceHelper
    {
        private const string Ns = "frm_HIS_uninstaller_tool.";

        public static Image LoadImage(string name)
        {
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(Ns + name))
            {
                if (stream == null) return null;
                using (var src = Image.FromStream(stream))
                {
                    // Independent copy — not tied to the stream lifetime
                    var bmp = new Bitmap(src.Width, src.Height, src.PixelFormat);
                    using (var g = Graphics.FromImage(bmp))
                        g.DrawImage(src, 0, 0);
                    return bmp;
                }
            }
        }

        public static XmlDocument LoadXml(string name)
        {
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(Ns + name))
            {
                if (stream == null) return null;
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(stream);
                    return doc;
                }
                catch { return null; }
            }
        }
    }
}
