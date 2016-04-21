
using amulware.Graphics;
using amulware.Graphics.ShaderManagement;
using Bearded.Utilities.Math;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Game
{
    class HeatmapGeometry
    {
        IndexedSurface<HeatmapVertex> surface;

        public HeatmapGeometry(IndexedSurface<HeatmapVertex> surface)
        {
            this.surface = surface;
        }

        public void Draw(Vector2 center, Vector2 size)
        {
            size /= 2;

            this.surface.AddQuad(
                new HeatmapVertex(center.WithZ() + new Vector3(-size.X, -size.Y, 0), new Vector2(0, 0)),
                new HeatmapVertex(center.WithZ() + new Vector3(size.X, -size.Y, 0), new Vector2(1, 0)),
                new HeatmapVertex(center.WithZ() + new Vector3(size.X, size.Y, 0), new Vector2(1, 1)),
                new HeatmapVertex(center.WithZ() + new Vector3(-size.X, size.Y, 0), new Vector2(0, 1))
            );
        }
    }

    struct HeatmapVertex : IVertexData
    {
        private Vector3 position;
        private Vector2 uv;

        public HeatmapVertex(Vector3 position, Vector2 uv)
        {
            this.position = position;
            this.uv = uv;
        }

        public int Size()
        {
            return VertexData.SizeOf<HeatmapVertex>();
        }

        public VertexAttribute[] VertexAttributes()
        {
            return VertexData.MakeAttributeArray(
                VertexData.MakeAttributeTemplate<Vector3>("v_position"),
                VertexData.MakeAttributeTemplate<Vector2>("v_uv")
            );
        }
    }

    class Heatmap
    {
        const int size = 256;


        HeatmapGeometry heatGeo;
        IndexedSurface<HeatmapVertex> heatSurface;  
        RenderTarget renderTarget;
        Texture texture;

        RenderTarget tempRenderTarget;
        Texture tempTexture;

        PostProcessSurface blurHSurface;
        PostProcessSurface blurVSurface;

        public Heatmap(ShaderManager shaderMan, SurfaceManager surfaceMan)
        {
            this.texture = new Texture();
            this.texture.Resize(size, size, PixelInternalFormat.R32f);
            this.texture.SetParameters(TextureMinFilter.Linear, TextureMagFilter.Linear,
                                       TextureWrapMode.ClampToBorder, TextureWrapMode.ClampToBorder);
            this.renderTarget = new RenderTarget(this.texture);


            this.tempTexture = new Texture();
            this.tempTexture.Resize(size, size, PixelInternalFormat.R32f);
            this.tempTexture.SetParameters(TextureMinFilter.Linear, TextureMagFilter.Linear,
                                       TextureWrapMode.ClampToBorder, TextureWrapMode.ClampToBorder);
            this.tempRenderTarget = new RenderTarget(this.tempTexture);

            var sampleStep = 1f / size;


            var blurShader = shaderMan.MakeShaderProgram("blur");

            blurHSurface = new PostProcessSurface();
            blurHSurface.AddSettings(
                new Vector2Uniform("sampleStep", new Vector2(sampleStep, 0)),
                new TextureUniform("diffuse", texture)
            );
            blurShader.UseOnSurface(this.blurHSurface);


            blurVSurface = new PostProcessSurface();
            blurVSurface.AddSettings(
                new Vector2Uniform("sampleStep", new Vector2(0, sampleStep)),
                new TextureUniform("diffuse", tempTexture)
            );
            blurShader.UseOnSurface(this.blurVSurface);


            var spectrum = new Texture("data/images/spectrum.png");
            spectrum.SetParameters(TextureMinFilter.Linear, TextureMagFilter.Linear,
                                   TextureWrapMode.ClampToEdge, TextureWrapMode.ClampToEdge);


            this.heatSurface = new IndexedSurface<HeatmapVertex>();
            heatSurface.AddSettings(
                new TextureUniform("heatmap", this.texture),
                new TextureUniform("spectrum", spectrum, TextureUnit.Texture1),
                surfaceMan.ModelviewMatrix,
                surfaceMan.ProjectionMatrix
            );
            shaderMan.MakeShaderProgram("render-heatmap").UseOnSurface(this.heatSurface);

            this.heatGeo = new HeatmapGeometry(this.heatSurface);
        }

        public void Bind()
        {
            GL.Viewport(0, 0, size, size);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.renderTarget);
        }

        public void Blur()
        {

            GL.Disable(EnableCap.Blend);
            GL.Viewport(0, 0, size, size);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.tempRenderTarget);

            blurHSurface.Render();

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.renderTarget);

            blurVSurface.Render();

        }

        public void RenderTo(RenderTarget rt)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, rt);

            this.heatGeo.Draw(new Vector2(), new Vector2(1));
            this.heatSurface.Render();
        }
    }
}

