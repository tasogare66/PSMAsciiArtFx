/* PlayStation(R)Mobile SDK 2.00.00
 * Copyright (C) 2014 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;

namespace Sample
{

/**
 * TriangleSample
 */
class TriangleSample
{
    private static GraphicsContext graphics;
    private static Stopwatch stopwatch;
    private static ShaderProgram program;
    private static VertexBuffer vbuffer;
    private static Texture2D texture;
	public static Texture2D pixel_texture;

	public static SimpleFrameBuffer m_fb;

	public const int SCREEN_WIDTH = 960;
	public const int SCREEN_HEIGHT = 544;

    static bool loop = true;

    static void Main(string[] args)
    {
        Init();

        while (loop) {
            SystemEvents.CheckEvents();
            Update();
            Render();
        }
        Term();
    }

    static bool Init()
    {
        graphics = new GraphicsContext();
        stopwatch = new Stopwatch();
        stopwatch.Start();

        SampleDraw.Init(graphics);

        program = new ShaderProgram("/Application/shaders/VertexColor.cgx");
        vbuffer = new VertexBuffer(3, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);
        texture = new Texture2D("/Application/test.png", false);

		pixel_texture = new Texture2D( 1, 1, false, PixelFormat.Rgba ) ;
		Rgba[] pixels = { new Rgba( 255, 255, 255, 255 ) } ;
		pixel_texture.SetPixels( 0, pixels ) ;

		m_fb = new SimpleFrameBuffer( graphics, SCREEN_WIDTH, SCREEN_HEIGHT );

        program.SetUniformBinding(0, "WorldViewProj");
        program.SetAttributeBinding(0, "a_Position");
        program.SetAttributeBinding(1, "a_TexCoord");
        program.SetAttributeBinding(2, "a_Color");

        float[] positions = {
            0.0f, 0.577f, 0.0f,
            -0.5f, -0.289f, 0.0f,
            0.5f, -0.289f, 0.0f,
        };
        float[] texcoords = {
            0.5f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
        };
        float[] colors = {
            1.0f, 0.0f, 0.0f, 1.0f,
            0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 1.0f,
        };
        vbuffer.SetVertices(0, positions);
        vbuffer.SetVertices(1, texcoords);
        vbuffer.SetVertices(2, colors);
        return true;
    }

    static void Term()
    {
        SampleDraw.Term();
        program.Dispose();
        vbuffer.Dispose();
        texture.Dispose();
        graphics.Dispose();
    }

    static bool Update()
    {
        SampleDraw.Update();

        return true;
    }

    static bool Render()
    {
        float seconds = (float)stopwatch.ElapsedMilliseconds / 1000.0f;
        float aspect = graphics.Screen.AspectRatio;
        float fov = FMath.Radians(45.0f);

        Matrix4 proj = Matrix4.Perspective(fov, aspect, 1.0f, 1000000.0f);
        Matrix4 view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 3.0f),
                                    new Vector3(0.0f, 0.0f, 0.0f),
                                    Vector3.UnitY);
        Matrix4 world = Matrix4.RotationY(1.0f * seconds);

        Matrix4 worldViewProj = proj * view * world;
        program.SetUniformValue(0, ref worldViewProj);

		graphics.SetFrameBuffer( m_fb.m_frameBuffer );
		graphics.SetViewport( 0, 0, m_fb.m_frameBuffer.Width, m_fb.m_frameBuffer.Height );
		graphics.SetClearColor( 0.0f, 0.0f, 0.0f, 1.0f );
		graphics.Clear();

        graphics.SetShaderProgram(program);
        graphics.SetVertexBuffer(0, vbuffer);
        graphics.SetTexture(0, texture);
        graphics.DrawArrays(DrawMode.TriangleStrip, 0, 3);

		// render to screen
        graphics.SetFrameBuffer(graphics.Screen);

        graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
        graphics.SetClearColor(0.0f, 0.5f, 1.0f, 0.0f);
        graphics.Clear();

		m_fb.RenderDebug();

        SampleDraw.DrawText("AsciiArtFx Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }
}

} // end ns Sample
