
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
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public static class GlobalInterfaceData
    {     

        public static int MainMenuWidth = 750;
        public static int MainMenuHeight = 750;
        public static int MinimumApplicationWindowWidth = 750;
        public static int MinimumApplicationWindowHeight = 370;
        
        public static GameWindow OSWindow;
        public static MainWindow MainWindow;
        public static Viewport FullscreenViewport;
        public static GameTime Time;

        public static GraphicsDevice Device;
        public static Effect UIEffect;
        public static SpriteBatch TextBatch;


        public static FontSystem StandardRegularFont;
        public static FontSystem StandardBoldFont;
        public static FontSystem MediumRegularFont;

        public static ColorScheme Scheme = new ColorScheme();



        public static Dictionary<UILookupKey, Texture2D> TextureLookup = new Dictionary<UILookupKey, Texture2D>();

        public static Texture2D SVGToTexture(string Path)
        {
            SvgDocument svgDoc = SvgDocument.Open<SvgDocument>(Path, null);
            Bitmap Image = svgDoc.Draw(UIUtils.ConvertFloatToMinInt(svgDoc.Width.Value * (float)UIScale, 1f), UIUtils.ConvertFloatToMinInt(svgDoc.Width.Value * (float)UIScale, 1f));
            int bufferSize = Image.Height * Image.Width * 4;
            System.IO.MemoryStream memoryStream = new MemoryStream(bufferSize);
            Image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            Texture2D Texture = Texture2D.FromStream(Device, memoryStream);
            return Texture;
        }

        public static Texture2D PNGToTexture(string Path)
        {
            Bitmap image = new Bitmap(Image.FromFile(Path));
            int bufferSize = image.Height * image.Width * 4;
            System.IO.MemoryStream memoryStream = new MemoryStream(bufferSize);
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            Texture2D Texture = Texture2D.FromStream(Device, memoryStream);
            return Texture;
        }

        const int ManuallyGeneratedMaxEnum = 19;
        public static void BakeTextures()
        {
            for (int i = 0; i < ManuallyGeneratedMaxEnum + 1; i++)
            {
                UILookupKey Key = (UILookupKey)i;
                TextureLookup.Add(Key, SVGToTexture(Environment.CurrentDirectory + @"\Assets\SVG\" + Key.ToString() + @".svg"));
            }

            TextureLookup.Add(UILookupKey.Execute, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "Execute" + @".png"));
            TextureLookup.Add(UILookupKey.ExecuteHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "ExecuteHighlight" + @".png"));
            TextureLookup.Add(UILookupKey.Restart, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "Restart" + @".png"));
            TextureLookup.Add(UILookupKey.RestartHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "RestartHighlight" + @".png"));
            TextureLookup.Add(UILookupKey.Step, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "Step" + @".png"));
            TextureLookup.Add(UILookupKey.StepHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "StepHighlight" + @".png"));
            TextureLookup.Add(UILookupKey.AutoStep, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "AutoStep" + @".png"));
            TextureLookup.Add(UILookupKey.AutoStepHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "AutoStepHighlight" + @".png"));
            TextureLookup.Add(UILookupKey.Pause, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "Pause" + @".png"));
            TextureLookup.Add(UILookupKey.PauseHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "PauseHighlight" + @".png"));
            TextureLookup.Add(UILookupKey.SpeedOne, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "SpeedOne" + @".png"));
            TextureLookup.Add(UILookupKey.SpeedOneHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "SpeedOneHighlight" + @".png"));
            TextureLookup.Add(UILookupKey.SpeedTwo, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "SpeedTwo" + @".png"));
            TextureLookup.Add(UILookupKey.SpeedTwoHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "SpeedTwoHighlight" + @".png"));
            TextureLookup.Add(UILookupKey.SpeedThree, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "SpeedThree" + @".png"));
            TextureLookup.Add(UILookupKey.SpeedThreeHighlight, PNGToTexture(Environment.CurrentDirectory + @"\Assets\PNG\" + "SpeedThreeHighlight" + @".png"));

            TextureLookup.Add(UILookupKey.DebugTexture, GenerateFilledTexture(1,1, Scheme.UIOverlayDebugColor1));
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
                Values[i] = new Color(Col.R + Scheme.HighlightEdgeColorChange.R, Col.G + Scheme.HighlightEdgeColorChange.G, Col.G + Scheme.HighlightEdgeColorChange.B, Col.A);
            }
            for (int i = Texture.Width * (Texture.Height - 1); i < Texture.Width * Texture.Height; i++)
            {
                Color Col = Values[i];
                Values[i] = new Color(Col.R + Scheme.HighlightEdgeColorChange.R, Col.G + Scheme.HighlightEdgeColorChange.G, Col.G + Scheme.HighlightEdgeColorChange.B, Col.A);
            }
            for (int i = Texture.Width; i < Texture.Width * (Texture.Height - 1); i += Texture.Width)
            {
                Color Col = Values[i];
                Values[i] = new Color(Col.R + Scheme.HighlightEdgeColorChange.R, Col.G + Scheme.HighlightEdgeColorChange.G, Col.G + Scheme.HighlightEdgeColorChange.B, Col.A);
            }
            for (int i = Texture.Width * 2 - 1; i < Texture.Width * Texture.Height; i += Texture.Width)
            {
                Color Col = Values[i];
                Values[i] = new Color(Col.R + Scheme.HighlightEdgeColorChange.R, Col.G + Scheme.HighlightEdgeColorChange.G, Col.G + Scheme.HighlightEdgeColorChange.B, Col.A);
            }

            Texture.SetData(Values);
            return Texture;
        }




        const int ReferenceHeight = 1080;
        static double UIScale = 1f;
        public static void UpdateScaleManual(double NewScale)
        {
            UIScale = NewScale;
        }
        public static void UpdateScaleAuto()
        {
            //UIScale = main;
        }
        public static Point Scale(Point Value)
        {
            return new Point(Convert.ToInt32((double)Value.X * UIScale), Convert.ToInt32((double)Value.Y * UIScale));
        }
        public static Point ScaleMin(Point Value, double Min)
        {
            return new Point(Convert.ToInt32(Math.Clamp((double)Value.X * UIScale, Min, double.PositiveInfinity)), Convert.ToInt32(Math.Clamp((double)Value.Y * UIScale, Min, double.PositiveInfinity)));
        }
        public static Vector2 Scale(Vector2 Value)
        {
            return new Vector2((float)(Value.X * UIScale), (float)(Value.Y * UIScale));
        }
        public static float Scale(float Value)
        {
            return (float)(Value * UIScale);
        }


    }

    /*
    public class ColorScheme
    {
        public Color HeaderColor = new Color(25, 27, 29);
        public Color SubHeaderColor = new Color(38, 40, 44);
        public Color BackgroundColor = new Color(30, 32, 34);

        public Color BrightAccentColor = new Color(64, 115, 255);
        public Color DarkAccentColor = new Color(60, 60, 64);
        public Color HighlightEdgeColorChange = new Color(30, 30, 30);

        public Color FontColor = Color.White;
        public Color FontGrayedOutColor = Color.White;

        public Color TuringMachineActionColor = Color.White;
        public Color TuringMachineSecondaryActionColor = Color.White;

        public Color UIOverlayDebugColor1 = new Color(124, 0, 124);
        public Color UIOverlayDebugColor2 = new Color(255, 124, 255);
        public Color UIOverlayDebugColor3 = new Color(0, 145, 0);
        public Color UIOverlayDebugColor4 = new Color(0, 255, 0);
    }
    */
    public class ColorScheme
    {
        public Color Header = new Color(25, 27, 29);
        public Color SubHeader = new Color(38, 40, 44);
        public Color Background = new Color(255, 255, 255);

        // these are to be changed
        public Color BrightAccent = new Color(64, 115, 255);
        public Color DarkAccent = new Color(60, 60, 64);
        public Color HighlightEdgeColorChange = new Color(30, 30, 30);
        //

        public Color DarkInteractableAccent = new Color(219, 219, 219);
        public Color InteractableAccent = new Color(235, 235, 235);
        public Color NonInteractableAccent = new Color(208, 208, 208);

        public Color FontColor = Color.Black;
        public Color FontColorBright = Color.White;
        public Color FontGrayedOutColor = new Color(94, 94, 94);


        public Color UIOverlayDebugColor1 = new Color(124, 0, 124);
        public Color UIOverlayDebugColor2 = new Color(255, 124, 255);
        public Color UIOverlayDebugColor3 = new Color(0, 145, 0);
        public Color UIOverlayDebugColor4 = new Color(0, 255, 0);
    }


    public enum UILookupKey
    {
        MouseIcon,
        TypingIcon,

        MinimiseIcon,
        MinimiseHighlightIcon,

        FullscreenIcon,
        FullscreenHighlightIcon,

        CloseApplicationIcon,
        CloseApplicationHighlightIcon,

        ConnectIcon,
        LoadIcon,

        UndoIcon,
        RedoIcon,

        SaveIcon,

        Header,

        //
        RunIcon,
        AlphabetIcon,
        FolderIcon,
        SlateFileTCIcon,
        SlateFileTICIcon,
        TransitionTableIcon,

        Execute,
        ExecuteHighlight,
        Step,
        StepHighlight,
        Restart,
        RestartHighlight,
        AutoStep,
        AutoStepHighlight,
        Pause,
        PauseHighlight,
        SpeedOne,
        SpeedOneHighlight,
        SpeedTwo,
        SpeedTwoHighlight,
        SpeedThree,
        SpeedThreeHighlight,


        DebugTexture
    }

}
