
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;
using FontStashSharp;
using Svg;
using Bitmap = System.Drawing.Bitmap;
using Image = System.Drawing.Image;
using System.IO;

namespace TuringSimulatorDesktop
{
    public static class GlobalRenderingData
    {
        public static int MainMenuWidth = 750;
        public static int MainMenuHeight = 750;
        public static int MinimumApplicationWindowWidth = 770;
        public static int MinimumApplicationWindowHeight = 370;

        public static bool UIRequiresRedraw = true;
        
        public static GameWindow OSWindow;
        public static MainWindow MainWindow;
        public static Viewport FullscreenViewport;
        public static GameTime Time;

        public static GraphicsDevice Device;
        public static Effect UIEffect;
        public static SpriteBatch TextBatch;

        public static FontSystem StandardRegularFont;
        public static FontSystem MediumRegularFont;

        public static int ToolbarHeight = 35;
        public static int WindowTabHeight = 25;

        public static int WindowTitleBarHeight = 32;

        public static Color HeaderColor = new Color(25, 27, 29);
        public static Color SubHeaderColor = new Color(38, 40, 44);
        public static Color BackgroundColor = new Color(30, 32, 34);

        public static Color BrightAccentColor = new Color(64, 115, 255);
        public static Color DarkAccentColor = new Color(60, 60, 64);
        public static Color HighlightEdgeColorChange = new Color(30, 30, 30);

        public static Color FontColor = Color.White;
        public static Color FontGrayedOutColor = Color.White;

        public static Color TuringMachineActionColor = Color.White;
        public static Color TuringMachineSecondaryActionColor = Color.White;

        static Color UIOverlayDebugColor1 = new Color(124, 0, 124);
        static Color UIOverlayDebugColor2 = new Color(255, 124, 255);
        static Color UIOverlayDebugColor3 = new Color(0, 145, 0);
        static Color UIOverlayDebugColor4 = new Color(0, 255, 0);
        public static Color DebugColor = Color.Purple;

        public static Dictionary<UILookupKey, Texture2D> TextureLookup = new Dictionary<UILookupKey, Texture2D>();

        public static Texture2D SVGToTexture(string Path)
        {
            SvgDocument svgDoc = SvgDocument.Open<SvgDocument>(Path, null);
            Bitmap Image = svgDoc.Draw();
            int bufferSize = Image.Height * Image.Width * 4;
            System.IO.MemoryStream memoryStream = new MemoryStream(bufferSize);
            Image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            Texture2D Texture = Texture2D.FromStream(GlobalRenderingData.Device, memoryStream);
            return Texture;
        }

        public static Texture2D PNGToTexture(string Path)
        {
            Bitmap image = new Bitmap(Image.FromFile(Path));
            int bufferSize = image.Height * image.Width * 4;
            System.IO.MemoryStream memoryStream = new MemoryStream(bufferSize);
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            Texture2D Texture = Texture2D.FromStream(GlobalRenderingData.Device, memoryStream);
            return Texture;
        }

        const int ManuallyGeneratedMaxEnum = 8;
        //REMEMBER TO UPDATE
        const int MaxEnumValue = 14;
        //
        public static void BakeTextures()
        {
            for (int i = 0; i < ManuallyGeneratedMaxEnum + 1; i++)
            {
                UILookupKey Key = (UILookupKey)i;
                TextureLookup.Add(Key, SVGToTexture(Environment.CurrentDirectory + @"\Assets\SVG\" + Key.ToString() + @".svg"));
            }

            TextureLookup.Add(UILookupKey.NewProjectButtonHightlight, ApplyBorderHighlight(CopyTexture(UILookupKey.NewProjectButton)));
            TextureLookup.Add(UILookupKey.LoadProjectButtonHightlight, ApplyBorderHighlight(CopyTexture(UILookupKey.LoadProjectButton)));
            TextureLookup.Add(UILookupKey.HostProjectButtonHightlight, ApplyBorderHighlight(CopyTexture(UILookupKey.HostProjectButton)));
            TextureLookup.Add(UILookupKey.JoinProjectButtonHightlight, ApplyBorderHighlight(CopyTexture(UILookupKey.JoinProjectButton)));

            TextureLookup.Add(UILookupKey.Debug1, GenerateFilledTexture(1, 1, UIOverlayDebugColor1));
            TextureLookup.Add(UILookupKey.Debug2, GenerateFilledTexture(1, 1, UIOverlayDebugColor2));
            TextureLookup.Add(UILookupKey.Header, GenerateFilledTexture(1, 1, HeaderColor));
            TextureLookup.Add(UILookupKey.SubHeader, GenerateFilledTexture(1, 1, SubHeaderColor));
            TextureLookup.Add(UILookupKey.Background, GenerateFilledTexture(1, 1, BackgroundColor));


            //manually implement per ui
            //paint background, paint on icons, text
        }

        static Texture2D GenerateFilledTexture(int Width, int Height, Color Fill)
        {
            Texture2D Texture = new Texture2D(Device, Width, Height);
            Color[] Values = new Color[Width*Height];
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = Fill;
            }
            Texture.SetData(Values);
            return Texture;
        }

        static Texture2D CopyTexture(UILookupKey SourceTextureKey)
        {
            return CopyTexture(TextureLookup[SourceTextureKey]);
        }

        static Texture2D CopyTexture(Texture2D SourceTexture)
        {
            Texture2D OutputTexture = new Texture2D(Device, SourceTexture.Width, SourceTexture.Height);
            Color[] Values = new Color[SourceTexture.Width * SourceTexture.Height];
            SourceTexture.GetData(Values);
            OutputTexture.SetData(Values);
            return OutputTexture;
        }

        static Texture2D ApplyBorderHighlight(Texture2D Texture)
        {
            Color[] Values = new Color[Texture.Width * Texture.Height];
            Texture.GetData(Values);

            /*
            for (int y = 0; y < Texture.Height; y++)
            {
                for (int x = 0; x < Texture.Width; x++)
                {
                    Color Col = Values[Texture.Width * y + x];
                    Values[Texture.Width * y + x] = new Color(Col.R, Col.G, Col.B, Col.A);
                }
            }
            */

            for (int i = 0; i < Texture.Width; i++)
            {
                Color Col = Values[i];
                Values[i] = new Color(Col.R + HighlightEdgeColorChange.R, Col.G + HighlightEdgeColorChange.G, Col.G + HighlightEdgeColorChange.B, Col.A);
            }
            for (int i = Texture.Width * (Texture.Height - 1); i < Texture.Width * Texture.Height; i++)
            {
                Color Col = Values[i];
                Values[i] = new Color(Col.R + HighlightEdgeColorChange.R, Col.G + HighlightEdgeColorChange.G, Col.G + HighlightEdgeColorChange.B, Col.A);
            }
            for (int i = Texture.Width; i < Texture.Width * (Texture.Height - 1); i += Texture.Width)
            {
                Color Col = Values[i];
                Values[i] = new Color(Col.R + HighlightEdgeColorChange.R, Col.G + HighlightEdgeColorChange.G, Col.G + HighlightEdgeColorChange.B, Col.A);
            }
            for (int i = Texture.Width * 2 - 1; i < Texture.Width * Texture.Height; i += Texture.Width)
            {
                Color Col = Values[i];
                Values[i] = new Color(Col.R + HighlightEdgeColorChange.R, Col.G + HighlightEdgeColorChange.G, Col.G + HighlightEdgeColorChange.B, Col.A);
            }

            Texture.SetData(Values);
            return Texture;
        }


    }

    public static class GlobalLayoutData
    {
        public static Dictionary<UILayoutKey, LayoutData> Guide = new Dictionary<UILayoutKey, LayoutData>();
        static Dictionary<UILayoutKey, LayoutData> InternalReferenceGuide = new Dictionary<UILayoutKey, LayoutData>()
        {
           // {UILayoutKey.FileExplorerTitle, new LayoutData( new Point(10, 0), new Vector2(  ) )}





        };

        static float UIScale = 1f;
        public static void ChangeScale(float NewScale)
        {
            UIScale = NewScale;
            Guide.Clear();

            foreach (KeyValuePair<UILayoutKey, LayoutData> Pair in InternalReferenceGuide)
            {
                Guide.Add(Pair.Key, new LayoutData(Scale(Pair.Value.Bounds), Scale(Pair.Value.Position)));
            }
        }
        public static Point Scale(Point Value)
        {
            return new Point(Convert.ToInt32((float)Value.X * UIScale), Convert.ToInt32((float)Value.Y * UIScale));
        }
        public static Vector2 Scale(Vector2 Value)
        {
            return new Vector2(Value.X * UIScale, Value.Y * UIScale);
        }
        public static float Scale(float Value)
        {
            return Value * UIScale;
        }
    }

    public class LayoutData
    {
        public Point Bounds;
        public Vector2 Position;

        public LayoutData(Point bounds, Vector2 position)
        {
            Bounds = bounds;
            Position = position;
        }
    }

    public enum UILayoutKey
    {
        FileExplorerTitle,
        FileExplorerSearchbar,
        FileExplorerLayoutbox,
        
        FIleIcon,
        FileLabel,
    }



    public enum UILookupKey
    {
        AlphabetIcon,
        FolderIcon,
        SlateFileTCIcon,
        SlateFileTICIcon,
        TransitionTableIcon,
        NewProjectButton,
        LoadProjectButton,
        HostProjectButton,
        JoinProjectButton,
        NewProjectButtonHightlight,
        LoadProjectButtonHightlight,
        HostProjectButtonHightlight,
        JoinProjectButtonHightlight,
        Debug1,
        Debug2,
        Header,
        SubHeader,
        Background,
    }
    public enum AnchorPoint { TopLeft, TopRight, BottomLeft, BottomRight }
}
