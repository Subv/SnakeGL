using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;
using System.Runtime.InteropServices;

namespace Snake
{
    public class GameWindow : OpenTK.GameWindow
    {
        private Snake game;
        private int program_id;
        private int vertex_shader_id;
        private int fragment_shader_id;

        private const int SquareSide = 40;
        private int NumSquares;

        #region Vertex Shader Attributes & Uniforms
        int attrib_vposition;
        int attrib_type;
        int uniform_modelview;
        #endregion

        #region Vertex Buffers
        int vbo_position;
        int vbo_type;
        int vao_root;
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        struct GridCell
        {
            // First triangle
            public Vector2 v0;
            public Vector2 v1;
            public Vector2 v2;

            // Second triangle
            public Vector2 v3;
            public Vector2 v4;
            public Vector2 v5;
        }

        #region Static Data
        Matrix4[] modelview;
        GridCell[] grid;
        #endregion

        public GameWindow() : 
            base(800, 600, new GraphicsMode(), "Snake", 0, DisplayDevice.Default, 4, 0, 
                GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            NumSquares = 800 * 600 / (SquareSide * SquareSide);
            game = new Snake(this, 800 / SquareSide, 600 / SquareSide);
        }

        private void SetupGrid()
        {
            grid = new GridCell[game.Height * game.Width];

            float cur_x = -1.0f;
            float cur_y = 1.0f;
            float increment_x = SquareSide / (float)Width;
            float increment_y = SquareSide / (float)Height;
            for (int y = 0; y < game.Height; ++y)
            {
                for (int x = 0; x < game.Width; ++x)
                {
                    grid[y * game.Width + x].v0 = new Vector2(cur_x, cur_y);
                    grid[y * game.Width + x].v1 = new Vector2(cur_x + increment_x, cur_y);
                    grid[y * game.Width + x].v2 = new Vector2(cur_x + increment_x, cur_y - increment_y);

                    grid[y * game.Width + x].v3 = new Vector2(cur_x, cur_y - increment_y);
                    grid[y * game.Width + x].v4 = grid[y * game.Width + x].v0;
                    grid[y * game.Width + x].v5 = grid[y * game.Width + x].v2;

                    cur_x += increment_x;
                }
                cur_x = -1f;
                cur_y -= increment_y;
            }
        }

        private int LoadShader(String filename, ShaderType type, int program)
        {
            int address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
                GL.ShaderSource(address, sr.ReadToEnd());

            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
            return address;
        }

        private void SetupShaders()
        {
            program_id = GL.CreateProgram();
            vertex_shader_id = LoadShader("shaders/vs.glsl", ShaderType.VertexShader, program_id);
            fragment_shader_id = LoadShader("shaders/fs.glsl", ShaderType.FragmentShader, program_id);
            GL.LinkProgram(program_id);
            Console.WriteLine(GL.GetProgramInfoLog(program_id));

            attrib_vposition = GL.GetAttribLocation(program_id, "vPosition");
            attrib_type = GL.GetAttribLocation(program_id, "in_type");
            uniform_modelview = GL.GetUniformLocation(program_id, "modelview");

            if (attrib_type == -1 || attrib_vposition == -1 || uniform_modelview == -1)
                Console.WriteLine("Error binding vertex shader attributes");

            GL.UseProgram(program_id);

            GL.UniformMatrix4(uniform_modelview, false, ref modelview[0]);
        }

        private void SetupVBOs()
        {
            vao_root = GL.GenVertexArray();
            GL.BindVertexArray(vao_root);

            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out vbo_type);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<GridCell>(BufferTarget.ArrayBuffer, (IntPtr)(Marshal.SizeOf<GridCell>() * grid.Length), grid, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(attrib_vposition, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_type);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Marshal.SizeOf<int>() * grid.Length * 6), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.VertexAttribIPointer(attrib_type, 1, VertexAttribIntegerType.Int, 0, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            VSync = VSyncMode.On;

            modelview = new Matrix4[] { Matrix4.Identity };

            SetupGrid();
            SetupShaders();
            SetupVBOs();

            GL.ClearColor(Color4.CornflowerBlue);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            game.Update(e);

            var data = new int[game.Height * game.Width * 6];
            for (int y = 0; y < game.Height; ++y)
            {
                for (int x = 0; x < game.Width; ++x)
                {
                    for (int i = 0; i < 6; ++i)
                        data[(y * game.Width + x) * 6 + i] = game.Map[x, y];
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_type);
            GL.BufferSubData<int>(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(Marshal.SizeOf<int>() * data.Length), data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.EnableVertexAttribArray(attrib_type);
            GL.EnableVertexAttribArray(attrib_vposition);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3 * grid.Length * 6);

            GL.DisableVertexAttribArray(attrib_type);
            GL.DisableVertexAttribArray(attrib_vposition);
            GL.Flush();

            SwapBuffers();
        }
    }
}
