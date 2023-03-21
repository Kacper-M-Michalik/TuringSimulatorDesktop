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

        Icon MouseIcon;

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
            IsMouseVisible = false;            

            IsFixedTimeStep = false;

            MouseIcon = new Icon();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //SDL_SetWindowFullscreen(Window.Handle, 0);
            //SDL_SetWindowBordered(Window.Handle, (byte)1U);
            // SDL_MaximizeWindow(Window.Handle);
            //MaximiseWindow();
            GraphicsManager.PreferredBackBufferWidth = GlobalInterfaceData.MainMenuWidth;
            GraphicsManager.PreferredBackBufferHeight = GlobalInterfaceData.MainMenuHeight;
            GraphicsManager.ApplyChanges();

            ScreenBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            GlobalInterfaceData.OSWindow = Window;
            GlobalInterfaceData.MainWindow = this;
            GlobalInterfaceData.FullscreenViewport = GraphicsDevice.Viewport;

            GlobalInterfaceData.Device = GraphicsDevice;
            GlobalInterfaceData.UIEffect = Content.Load<Effect>("UIShader");

            FontSystemDefaults.FontResolutionFactor = 1.0f;
            FontSystemDefaults.KernelWidth = 1;
            FontSystemDefaults.KernelHeight = 1;
            FontSystemDefaults.PremultiplyAlpha = true;
            FontSystemDefaults.FontLoader = new FreeTypeLoader();
            GlobalInterfaceData.TextBatch = new SpriteBatch(GraphicsDevice);
            GlobalInterfaceData.StandardRegularFont = new FontSystem();
            GlobalInterfaceData.StandardRegularFont.AddFont(File.ReadAllBytes(@"Assets/Fonts/Roboto-Regular.ttf"));
            GlobalInterfaceData.StandardBoldFont = new FontSystem();
            GlobalInterfaceData.StandardBoldFont.AddFont(File.ReadAllBytes(@"Assets/Fonts/Roboto-Bold.ttf"));
            GlobalInterfaceData.MediumRegularFont = new FontSystem();
            GlobalInterfaceData.MediumRegularFont.AddFont(File.ReadAllBytes(@"Assets/Fonts/Roboto-Medium.ttf"));

            DateTime Start = DateTime.UtcNow;
            GlobalInterfaceData.BakeTextures();
            GlobalMeshRenderer.Setup();

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

            GlobalInterfaceData.Time = gameTime;

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

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            CurrentView.Draw();       

            GraphicsDevice.Viewport = GlobalInterfaceData.FullscreenViewport;
            //ScreenBatch.Begin();
            if (InputManager.DragData != null)
            {
                MouseIcon.Bounds = new Point(13, 22);
                MouseIcon.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
                MouseIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.MouseDragIcon];
                //ScreenBatch.Draw((GlobalInterfaceData.TextureLookup[UILookupKey.MouseDragIcon], new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y), Color.White);
            }
            else if (InputManager.IsMouseOverTypingArea)
            {
                MouseIcon.Bounds = new Point(10, 19);
                MouseIcon.Position = new Vector2(InputManager.MouseData.X - MouseIcon.Bounds.X * 0.5f, InputManager.MouseData.Y - MouseIcon.Bounds.Y * 0.5f);
                MouseIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TypingIcon];
                //Texture2D Texture = GlobalInterfaceData.TextureLookup[UILookupKey.TypingIcon];
                //ScreenBatch.Draw(Texture, new Vector2(InputManager.MouseData.X - Texture.Width * 0.5f, InputManager.MouseData.Y - Texture.Height * 0.5f), Color.White);
            }
            else
            {
                MouseIcon.Bounds = new Point(13, 22);
                MouseIcon.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
                MouseIcon.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.MouseIcon];
                //ScreenBatch.Draw(GlobalInterfaceData.TextureLookup[UILookupKey.MouseIcon], new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y), Color.White);
            }
            MouseIcon.Draw();
            InputManager.IsMouseOverTypingArea = false;
            //ScreenBatch.End();

            //InputManager.DrawActionGroups();         

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
            if (GraphicsDevice.PresentationParameters.BackBufferWidth < GlobalInterfaceData.MinimumApplicationWindowWidth) GraphicsManager.PreferredBackBufferWidth = GlobalInterfaceData.MinimumApplicationWindowWidth;
            if (GraphicsDevice.PresentationParameters.BackBufferHeight < GlobalInterfaceData.MinimumApplicationWindowHeight) GraphicsManager.PreferredBackBufferHeight = GlobalInterfaceData.MinimumApplicationWindowHeight;
            GraphicsManager.ApplyChanges();

            GlobalInterfaceData.FullscreenViewport = new Viewport(0, 0, GraphicsManager.PreferredBackBufferWidth, GraphicsManager.PreferredBackBufferHeight);
            CurrentView.ScreenResize();            
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Client.Disconnect();
            TuringServer.Server.CloseServer();
        }
    }
}