using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace _3d
{
    class Program
    {
        static void Main(string[] args)
        {
            NativeWindowSettings nws = new NativeWindowSettings() {
                Title = "Hello",
                Size = new Vector2i(1000,1000),
                StartFocused = true,
                StartVisible = true,
                APIVersion = new Version(3,2),
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core,
            };
            GameWindowSettings gws = new GameWindowSettings();
            new Game(gws,nws);
        }
    }

    class Game : GameWindow {

        Tile[] tiles = new Tile[2];
        Camera camera;

        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();
        static int mapResolution = 30, radius = mapResolution/2;
        double seed = new Random().Next(1,100000);

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings,nativeWindowSettings) {

            camera = new Camera(this);
            int i = 0;

            for (int iter = 0; iter < 10; iter++) {

                seed = new System.Random().Next(1,1000000000);

                Vector3 planetPosition = new Vector3(new Random().Next(-20,20)*50, new Random().Next(-20,20)*50, new Random().Next(-20,20)*50);
                mapResolution = new Random().Next(10,120);

                for (int x = -(mapResolution/2); x < mapResolution/2; x++) {
                    for (int y = -(mapResolution/2); y < mapResolution/2; y++) {
                        for (int z = -(mapResolution/2); z < mapResolution/2; z++) {

                            float xNoise = 0, yNoise = 0, zNoise = 0;

                            float nx = (float)x/30,
                                ny = (float)y/30,
                                nz = (float)z/30;

                            xNoise = (float)ImprovedNoise.noise(ny*2.1f, nz*2.1f, seed)*20;
                            yNoise = (float)ImprovedNoise.noise(nx*2.1f, nz*2.1f, seed)*20;
                            zNoise = (float)ImprovedNoise.noise(nx*2.1f, ny*2.1f, seed)*20;

                            float density = xNoise+yNoise+zNoise;

                            Vector3 position = new Vector3(x,y,z);
                            float d = Vector3.Distance(position, new Vector3(0,0,0));  

                            if (density < 4 && d < mapResolution/2) {
                                vertices = CubeProperties.GetPositionedVertices((new Vector3(x*2,(y*2)-mapResolution,z*2)+new Vector3(0, mapResolution*1.5f, 0)) ,new Vector3(x*2,(y*2)-mapResolution,z*2)+planetPosition, vertices, mapResolution);
                                indices = CubeProperties.GetNewIndices((uint)i,indices);
                                i++;
                            }
                        }
                    }
                }
                Console.WriteLine((iter+1) + " " + mapResolution);
            }
            tiles[0] = new Tile(camera, vertices, indices);

            Run();
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);
            tiles[0].Render();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
        }

        protected override void OnLoad() {
            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0,0,0,1.0f);
        }
    }
}