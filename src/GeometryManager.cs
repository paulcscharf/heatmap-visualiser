using amulware.Graphics;
using Bearded.Utilities;
using OpenTK;

namespace Game
{
    sealed class GeometryManager : Singleton<GeometryManager>
    {
        public PrimitiveGeometry Primitives { get; private set; }
        public FontGeometry Text{ get; private set; }

        public HeatmapGeometry Agents { get; private set; }

        public Sprite2DGeometry Blueprint { get; private set; }


        public GeometryManager(SurfaceManager surfaces)
        {
            this.Primitives = new PrimitiveGeometry(surfaces.Primitives);

            var font = Font.FromJsonFile("data/fonts/inconsolata.json");
            this.Text = new FontGeometry(surfaces.Text, font) {SizeCoefficient = new Vector2(1, -1)};

            this.Agents = new HeatmapGeometry(surfaces.Agents);

            this.Blueprint = new Sprite2DGeometry(surfaces.Blueprint);
            this.Blueprint.Size = new Vector2(1f, 0.6f);

        }

    }
}