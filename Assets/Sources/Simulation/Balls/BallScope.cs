using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Balls
{
    internal class BallScope : LifetimeScope
    {
        public Ball ball;
        public BallMovement ballMovement;
        public BallTrail ballTrail;
        public BallBody ballBody;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(ball);
            builder.RegisterComponent(ballMovement);
            builder.RegisterComponent(ballTrail);
            builder.RegisterComponent(ballBody);
        }
    }
}