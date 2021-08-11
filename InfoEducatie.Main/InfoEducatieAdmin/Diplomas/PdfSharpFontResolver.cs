using System;
using System.IO;
using MCMS.Files;
using PdfSharpCore.Fonts;

namespace InfoEducatie.Main.InfoEducatieAdmin.Diplomas
{
    public class PdfSharpFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            using var ms = new MemoryStream();
            using var fs = File.Open(faceName, FileMode.Open);
            fs.CopyTo(ms);
            ms.Position = 0;
            return ms.ToArray();
        }

        public string DefaultFontName => "OpenSans";

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("OpenSans", StringComparison.CurrentCultureIgnoreCase))
            {
                return isBold switch
                {
                    true when isItalic => new FontResolverInfo(Path.Combine(MFiles.PublicPath, "fonts",
                        "OpenSans-BoldItalic.ttf")),
                    true => new FontResolverInfo(Path.Combine(MFiles.PublicPath, "fonts", "OpenSans-Bold.ttf")),
                    _ => isItalic
                        ? new FontResolverInfo(Path.Combine(MFiles.PublicPath, "fonts", "OpenSans-Italic.ttf"))
                        : new FontResolverInfo(Path.Combine(MFiles.PublicPath, "fonts", "OpenSans-Regular.ttf"))
                };
            }

            return null;
        }
    }
}