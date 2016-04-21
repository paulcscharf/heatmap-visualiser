
using Bearded.Utilities.Math;
using Bearded.Utilities.SpaceTime;
using OpenTK;

namespace Game
{
    class Agent : GameObject
    {
        protected Position2 position;

        float radius;

        float speed;

        Position2 goal;

        Velocity2 velocity;

        Instant deletionTime;

        private readonly string id;


        public Agent(GameState game, Position2 position, string id = null)
            : base(game)
        {
            this.position = position;
            this.goal = position;
            this.id = id;

            System.Console.WriteLine($"spawning agent '{id}'");
        }

        public void SetDeletionTime(Instant instant)
        {
            this.deletionTime = instant;
        }

        public void SetGoal(Position2 position)
        {
            this.goal = position;
        }

        public override void Update(TimeSpan elapsedTime)
        {

            var difference = this.goal - this.position;

            var acceleration = new Acceleration2(difference.NumericValue);

            this.velocity += acceleration * 3 * elapsedTime;

            this.position += this.velocity * elapsedTime;

            this.velocity *= Mathf.Pow(0.01f, (float)elapsedTime.NumericValue);

            if (this.deletionTime != Instant.Zero && this.game.Time >= this.deletionTime)
            {
                this.Delete();
            }
        }

        public override void Draw()
        {
            var geo = GeometryManager.Instance.Agents;

            geo.Draw(this.position.NumericValue, new Vector2(0.08f));
        }

        protected override void onDelete()
        {
            System.Console.WriteLine($"deleting agent '{this.id}'");
        }

   }
}

