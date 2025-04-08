namespace App.SampleInGame.Domain
{
    public struct BallCollisionMessage
    {
        public readonly int Score;
        
        public BallCollisionMessage(int score)
        {
            Score = score;
        }
    }

}