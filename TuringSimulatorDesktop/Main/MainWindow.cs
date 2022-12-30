using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;
using FontStashSharp;
using System.IO;
using FontStashSharp.Rasterizers.FreeType;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Text;

namespace TuringSimulatorDesktop
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using var game = new TuringSimulatorDesktop.MainWindow();
            game.Run();
        }
    }

    public class MainWindow : Game
    {
        public GraphicsDeviceManager GraphicsManager;
        public View CurrentView;
        SpriteBatch ScreenBatch;

        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_MaximizeWindow(IntPtr window);
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_MinimizeWindow(IntPtr window); 
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowFullscreen(IntPtr window, UInt32 flags); 
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowBordered(IntPtr window, byte Bool);
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GetWindowBordersSize(IntPtr window, out int top, out int left, out int bottom, out int right);

        UInt32 Flags =  0x00000001U | 0x00001000U;

        public MainWindow()
        {
            GraphicsManager = new GraphicsDeviceManager(this);
            GraphicsManager.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.IsBorderless = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            Window.AllowAltF4 = true;

            IsFixedTimeStep = false;            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //SDL_SetWindowFullscreen(Window.Handle, 0);
            SDL_SetWindowBordered(Window.Handle, (byte)1U);
            SDL_MaximizeWindow(Window.Handle);
            //MaximiseWindow();
            ScreenBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            GlobalInterfaceData.OSWindow = Window;
            GlobalInterfaceData.MainWindow = this;
            GlobalInterfaceData.FullscreenViewport = GraphicsDevice.Viewport;

            GlobalInterfaceData.Device = GraphicsDevice;
            GlobalInterfaceData.UIEffect = Content.Load<Effect>("UIShader");

            FontSystemDefaults.FontLoader = new FreeTypeLoader();
            FontSystemDefaults.FontResolutionFactor = 2.0f;
            FontSystemDefaults.KernelWidth = 2;
            FontSystemDefaults.KernelHeight = 2;
            FontSystemDefaults.PremultiplyAlpha = true;
            GlobalInterfaceData.TextBatch = new SpriteBatch(GraphicsDevice);
            GlobalInterfaceData.StandardRegularFont = new FontSystem();
            GlobalInterfaceData.StandardRegularFont.AddFont(File.ReadAllBytes(@"Assets/Fonts/Roboto-Regular.ttf"));
            GlobalInterfaceData.MediumRegularFont = new FontSystem();
            GlobalInterfaceData.MediumRegularFont.AddFont(File.ReadAllBytes(@"Assets/Fonts/Roboto-Medium.ttf"));

            GlobalInterfaceData.BakeTextures();
            GlobalUIRenderer.Setup(GlobalInterfaceData.Device);


            DirectoryInfo Info = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Turing Machine - Desktop");
            string DataPath = Info.FullName + Path.DirectorySeparatorChar + "LocalUserData.txt";

            if (!File.Exists(DataPath))
            {
                try
                {
                    File.Create(DataPath).Close();
                    GlobalProjectAndUserData.UserData = new LocalUserData();
                    GlobalProjectAndUserData.UserDataPath = DataPath;
                    GlobalProjectAndUserData.SaveUserData();
                }
                catch (Exception E)
                {
                    CustomLogging.Log("UI Error: Failed to generate fresh LocalUserData File - " + E.ToString());
                }
            }

            GlobalProjectAndUserData.LoadUserData(DataPath);

            CurrentView = new MainScreenView();
        }

        protected override void Update(GameTime gameTime)
        {      
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            GlobalInterfaceData.Time = gameTime;

            InputManager.Update();

            if (GlobalInterfaceData.UIRequiresRedraw)
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                CurrentView.Draw();                
                
                ScreenBatch.Begin();
                //DebugManager.Draw(GraphicsDevice, ScreenBatch, gameTime);
                ScreenBatch.End();
                //GlobalInterfaceData.UIRequiresRedraw = false;
                
            }
            else
            {
                //only redraw debug menu
            }
            base.Draw(gameTime);
        }

        public void MinimiseWindow()
        {
            SDL_SetWindowBordered(Window.Handle, (byte)1U);
            //SDL_MinimizeWindow(Window.Handle);
        }

        public void MaximiseWindow()
        {
            SDL_SetWindowBordered(Window.Handle, (byte)1U);
            SDL_MaximizeWindow(Window.Handle);
            int Y = Window.Position.Y - Window.ClientBounds.Top;
            SDL_SetWindowBordered(Window.Handle, (byte)0U);
            Window.Position = new Point(Window.Position.X, Y);
           // Window. = new Point(Window.Position.X, Y);
        }

        public void OnResize(object Sender, EventArgs Args)
        {
            if (GraphicsDevice.PresentationParameters.BackBufferWidth < GlobalInterfaceData.MinimumApplicationWindowWidth) GraphicsManager.PreferredBackBufferWidth = GlobalInterfaceData.MinimumApplicationWindowWidth;
            if (GraphicsDevice.PresentationParameters.BackBufferHeight < GlobalInterfaceData.MinimumApplicationWindowHeight) GraphicsManager.PreferredBackBufferHeight = GlobalInterfaceData.MinimumApplicationWindowHeight;
            GraphicsManager.ApplyChanges();

            GlobalInterfaceData.FullscreenViewport = new Viewport(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            /*
            for (int i = 0; i < GlobalGraphicsData.BackBufferListeners.Count; i++)
            {
                GlobalGraphicsData.BackBufferListeners[i].BackBufferResized(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
            */
            CurrentView.ViewResize(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);            
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            TuringServer.Server.CloseServer();
        }
    }
}