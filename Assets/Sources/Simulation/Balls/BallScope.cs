using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Balls
{
    internal class BallScope : LifetimeScope
    {
        public Ball ball;
        public BallMovement ballMovement;
        public BallSpeedLevelFx ballSpeedLevelFx;
        public BallBody ballBody;
        public BallOwning ballOwning;
        public BallLookForward ballLookForward;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(ball);
            builder.RegisterComponent(ballMovement);
            builder.RegisterComponent(ballSpeedLevelFx);
            builder.RegisterComponent(ballBody);
            builder.RegisterComponent(ballOwning);
            builder.RegisterComponent(ballLookForward);
        }
    }
}