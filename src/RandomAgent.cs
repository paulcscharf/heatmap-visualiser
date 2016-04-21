using Bearded.Utilities;
using Bearded.Utilities.SpaceTime;

namespace Game
{
    class RandomAgent : Agent
    {

        Instant nextMoveTime;

        Position2 goal;

        Velocity2 velocity;

        Position2 jitteredGoal;

        Instant nextJitter;

        const float roomWidth = 0.9f;
        const float roomHeight = 0.5f;

        public RandomAgent(GameState game)
            : base(game, new Position2(StaticRandom.Float(-1, 1) * roomWidth, StaticRandom.Float(-1, 1) * roomHeight))
        {
            this.goal = this.position;
            this.jitteredGoal = this.goal;

            this.nextMoveTime = this.game.Time + new TimeSpan(StaticRandom.Double(0, 10));
        }

        public override void Update(TimeSpan elapsedTime)
        {

            if (this.game.Time >= this.nextMoveTime)
            {
                this.goal = new Position2(StaticRandom.Float(-1, 1) * roomWidth, StaticRandom.Float(-1, 1) * roomHeight);
                this.nextMoveTime = this.game.Time + new TimeSpan(StaticRandom.Double(5, 20));
            }

            if (this.game.Time >= this.nextJitter)
            {
                this.jitteredGoal = this.goal + new Difference2(
                    StaticRandom.NormalFloat(0, 0.05f),
                    StaticRandom.NormalFloat(0, 0.05f)
                );
                this.nextJitter = this.game.Time + new TimeSpan(StaticRandom.Double(0, 2));

                this.SetGoal(this.jitteredGoal);
            }

            base.Update(elapsedTime);
        }
    }
}

