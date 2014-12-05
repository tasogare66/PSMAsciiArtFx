//# -*- coding: utf-8 -*-

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Game.Framework;

namespace Sample
{
	// SimpleSpriteのFrameBuffer
	public class SimpleFrameBuffer : IDisposable
	{
		protected GraphicsContext graphics;	// 参照

		public FrameBuffer m_frameBuffer;
		protected Texture2D m_Texture;
		protected SimpleSprite m_Spr;

		public SimpleSprite m_dbgSpr = null;
		public ShaderProgram m_sp;

		public Texture2D Texture {
			get { return m_Texture; }
		}

		public SimpleFrameBuffer( GraphicsContext gc, int width, int height )
		{
			graphics = gc;
			m_frameBuffer = new FrameBuffer();
			m_Texture = new Texture2D( width, height, false, PixelFormat.Rgba, PixelBufferOption.Renderable );
			m_frameBuffer.SetColorTarget( m_Texture, 0 );
			m_Spr = new SimpleSprite( gc, TriangleSample.pixel_texture, m_frameBuffer );
			m_Spr.Scale = new Vector2( width, height );

			m_sp = new ShaderProgram( "/Application/shaders/AsciiArtFx.cgx" );
			m_sp.SetUniformBinding(0, "u_WorldMatrix");

			m_dbgSpr = new SimpleSprite( graphics, m_Texture, null, m_sp );
			m_dbgSpr.SetTextureVFlip();
		}

		public virtual void Dispose()
		{
			if( m_dbgSpr != null ){
				m_dbgSpr.Dispose();
				m_dbgSpr = null;
			}
			if( m_sp != null ){
				m_sp.Dispose();
				m_sp = null;
			}

			if( m_Spr != null ){
				m_Spr.Dispose();
				m_Spr = null;
			}
			if( m_Texture != null ){
				m_Texture.Dispose();
				m_Texture = null;
			}
			if( m_frameBuffer != null ){
				m_frameBuffer.Dispose();
				m_frameBuffer = null;
			}
		}

		public void RenderTexture()
		{
			FrameBuffer prev = graphics.GetFrameBuffer( );
			var prev_vp = graphics.GetViewport();
			graphics.SetFrameBuffer( m_frameBuffer );
			graphics.SetViewport( 0, 0, m_frameBuffer.Width, m_frameBuffer.Height );
			graphics.SetClearColor( 0.0f, 0.0f, 0.0f, 1.0f );
			graphics.Clear();

			m_Spr.Render();

			// frame buffer戻す
			graphics.SetFrameBuffer( prev );
			graphics.SetViewport( prev_vp );
		}

		public void RenderDebug( Vector2 pos=default(Vector2) )
		{
			if( m_dbgSpr != null ){
				m_dbgSpr.Position.Xy = pos;
				m_dbgSpr.Render();
			}
		}
	}
}
