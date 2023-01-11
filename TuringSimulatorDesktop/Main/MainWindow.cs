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
            using var game = new MainWindow();
            game.Run();
        }
    }

    public class MainWindow : Game
    {
        public GraphicsDeviceManager GraphicsManager;
        public ScreenView CurrentView;
        SpriteBatch ScreenBatch;
        int BoundTop;

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
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowResizable(IntPtr window, byte Bool);

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
            Window.AllowUserResizing = false;
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
            //SDL_SetWindowBordered(Window.Handle, (byte)1U);
            // SDL_MaximizeWindow(Window.Handle);
            //MaximiseWindow();
            GraphicsManager.PreferredBackBufferWidth = GlobalRenderingData.MainMenuWidth;
            GraphicsManager.PreferredBackBufferHeight = GlobalRenderingData.MainMenuHeight;
            GraphicsManager.ApplyChanges();

            ScreenBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            GlobalRenderingData.OSWindow = Window;
            GlobalRenderingData.MainWindow = this;
            GlobalRenderingData.FullscreenViewport = GraphicsDevice.Viewport;

            GlobalRenderingData.Device = GraphicsDevice;
            GlobalRenderingData.UIEffect = Content.Load<Effect>("UIShader");

            FontSystemDefaults.FontResolutionFactor = 1.0f;
            FontSystemDefaults.KernelWidth = 1;
            FontSystemDefaults.KernelHeight = 1;
            FontSystemDefaults.PremultiplyAlpha = true;
            FontSystemDefaults.FontLoader = new FreeTypeLoader();
            GlobalRenderingData.TextBatch = new SpriteBatch(GraphicsDevice);
            GlobalRenderingData.StandardRegularFont = new FontSystem();
            GlobalRenderingData.StandardRegularFont.AddFont(File.ReadAllBytes(@"Assets/Fonts/Roboto-Regular.ttf"));
            GlobalRenderingData.MediumRegularFont = new FontSystem();
            GlobalRenderingData.MediumRegularFont.AddFont(File.ReadAllBytes(@"Assets/Fonts/Roboto-Medium.ttf"));

            DateTime Start = DateTime.UtcNow;
            GlobalRenderingData.BakeTextures();
            GlobalUIRenderer.Setup();

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

            CustomLogging.Log("Load Time: " + (DateTime.UtcNow-Start).TotalSeconds.ToString());

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

            UIEventManager.WindowRequiresNextFrameResizeStep = UIEventManager.WindowRequiresNextFrameResize;

            GlobalRenderingData.Time = gameTime;

            InputManager.Update();

            Client.ProcessPackets();

            //Process interrupts
            if (UIEventManager.ClientSuccessConnecting) UIEventManager.ClientSuccessConnectingDelegate?.Invoke(this, new EventArgs());
            if (UIEventManager.ClientFailedConnecting) UIEventManager.ClientFailedConnectingDelegate?.Invoke(this, new EventArgs());
            //if (UIEventManager.RecievedProjectDataFromServer) UIEventManager.RecievedProjectDataFromServerDelegate?.Invoke(this, new EventArgs());

            /*
            if (UIEventManager.ServerSuccessLoadingProject) UIEventManager.ServerSuccessLoadingProjectDelegate?.Invoke(this, new EventArgs());
            if (UIEventManager.NewProjectDataRecieved && CurrentView is ProjectScreenView)
            {
                ((ProjectScreenView)CurrentView).UpdatedProject();
                UIEventManager.NewProjectDataRecieved = false;
            }
            */

            if (GlobalRenderingData.UIRequiresRedraw)
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                CurrentView.Draw();       

                ScreenBatch.Begin();
                //DebugManager.Draw(GraphicsDevice, ScreenBatch, gameTime);
                ScreenBatch.End();
                //GlobalInterfaceData.UIRequiresRedraw = false;
                
            }

            if (UIEventManager.WindowRequiresNextFrameResizeStep)
            {
                GraphicsManager.PreferredBackBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
                GraphicsManager.PreferredBackBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight + BoundTop;
                GraphicsManager.ApplyChanges();
                UIEventManager.WindowRequiresNextFrameResize = false;
                UIEventManager.WindowRequiresNextFrameResizeStep = false;
                OnResize(this, null);
            }

            base.Draw(gameTime);
        }

        public void LeaveMainMenu()
        {
            SDL_SetWindowResizable(Window.Handle, (byte)1U);
            MaximiseWindow(); 
        }

        public void MinimiseWindow()
        {
            SDL_SetWindowBordered(Window.Handle, (byte)1U);
            SDL_MinimizeWindow(Window.Handle);
        }

        public void MaximiseWindow()
        {
            SDL_SetWindowBordered(Window.Handle, (byte)1U);
            SDL_MaximizeWindow(Window.Handle);
            BoundTop = Window.ClientBounds.Top;
            int Y = Window.Position.Y - BoundTop;
            SDL_SetWindowBordered(Window.Handle, (byte)0U);
            Window.Position = new Point(Window.Position.X, Y);
            UIEventManager.WindowRequiresNextFrameResize = true;
        }

        public void OnResize(object Sender, EventArgs Args)
        {
            if (GraphicsDevice.PresentationParameters.BackBufferWidth < GlobalRenderingData.MinimumApplicationWindowWidth) GraphicsManager.PreferredBackBufferWidth = GlobalRenderingData.MinimumApplicationWindowWidth;
            if (GraphicsDevice.PresentationParameters.BackBufferHeight < GlobalRenderingData.MinimumApplicationWindowHeight) GraphicsManager.PreferredBackBufferHeight = GlobalRenderingData.MinimumApplicationWindowHeight;
            GraphicsManager.ApplyChanges();

            GlobalRenderingData.FullscreenViewport = new Viewport(0, 0, GraphicsManager.PreferredBackBufferWidth, GraphicsManager.PreferredBackBufferHeight);
            CurrentView.ViewResize(GraphicsManager.PreferredBackBufferWidth, GraphicsManager.PreferredBackBufferHeight);            
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Client.Disconnect();
            TuringServer.Server.CloseServer();
        }
    }
}