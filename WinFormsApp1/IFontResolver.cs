using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using static PdfSharp.Snippets.Font.SegoeWpFontResolver;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using Syncfusion.Licensing;
using System.Windows.Forms;
using WinFormsApp1;

namespace WinFormsApp1
{

    public class MyFontResolver : IFontResolver
    {
        // Resolve the font by its family name and style.
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Normalize the family name for common cases
            familyName = familyName.ToLower();

            // Map common font names to files available on most Windows systems
            // You can add more mappings here if you need other specific fonts
            switch (familyName)
            {
                case "arial":
                case "helvetica": // Map Helvetica to Arial for system compatibility
                    if (isBold && isItalic) return new FontResolverInfo("Arial#BoldItalic");
                    if (isBold) return new FontResolverInfo("Arial#Bold");
                    if (isItalic) return new FontResolverInfo("Arial#Italic");
                    return new FontResolverInfo("Arial#Regular");

                case "times new roman":
                case "times": // Map Times to Times New Roman
                    if (isBold && isItalic) return new FontResolverInfo("TimesNewRoman#BoldItalic");
                    if (isBold) return new FontResolverInfo("TimesNewRoman#Bold");
                    if (isItalic) return new FontResolverInfo("TimesNewRoman#Italic");
                    return new FontResolverInfo("TimesNewRoman#Regular");

                case "courier new":
                case "courier": // Map Courier to Courier New
                    if (isBold && isItalic) return new FontResolverInfo("CourierNew#BoldItalic");
                    if (isBold) return new FontResolverInfo("CourierNew#Bold");
                    if (isItalic) return new FontResolverInfo("CourierNew#Italic");
                    return new FontResolverInfo("CourierNew#Regular");
                // Add more font mappings as needed
                default:
                    // Fallback to Arial if no specific mapping is found
                    return new FontResolverInfo("Arial#Regular");
            }
        }

        // Get the font data from the FontResolverInfo.
        public byte[] GetFont(string faceName)
        {
            // These are common paths for TrueType fonts on Windows.
            // For other OS, you'd need different paths or embedded resources.
            string fontPath = "";
            switch (faceName)
            {
                case "Arial#Regular": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"); break;
                case "Arial#Bold": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialbd.ttf"); break;
                case "Arial#Italic": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ariali.ttf"); break;
                case "Arial#BoldItalic": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialbi.ttf"); break;

                case "TimesNewRoman#Regular": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf"); break;
                case "TimesNewRoman#Bold": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "timesbd.ttf"); break;
                case "TimesNewRoman#Italic": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "timesi.ttf"); break;
                case "TimesNewRoman#BoldItalic": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "timesbi.ttf"); break;

                case "CourierNew#Regular": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "cour.ttf"); break;
                case "CourierNew#Bold": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "courbd.ttf"); break;
                case "CourierNew#Italic": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "couri.ttf"); break;
                case "CourierNew#BoldItalic": fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "courbi.ttf"); break;

                // Add cases for other fonts as needed
                default:
                    // If a specific font file isn't mapped, try a generic Arial fallback
                    fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                    break;
            }

            if (File.Exists(fontPath))
            {
                return File.ReadAllBytes(fontPath);
            }
            else
            {
                // Handle cases where the font file isn't found.
                // You might log an error, or return a fallback font if absolutely necessary.
                // For production, you MUST ensure these paths are correct or fonts are embedded.
                throw new FileNotFoundException($"Font file not found: {fontPath} for faceName: {faceName}");
            }
        }
    }
}
