namespace RazeonProject.Models.Relations
{
    public class Player
    {
        public enum State
        {
            play = 1,
            pause = 2,
            stop = 3
        }

        public double Progress { get; set; }
        public double Volume { get; set; }
        public State PlayerState { get; set; }

        public Player()
        {
            Progress = 0;
            Volume = 0;
            PlayerState = State.stop;
        }

        public double GetProgress()
        {
            return Progress;
        }

        public void SetProgress(double value)
        {
            Progress = Math.Max(0.0, Math.Min(1.0, value));
        }

        public double GetVolume()
        {
            return Volume;
        }

        public void SetVolume(double value)
        {
            Volume = Math.Max(0.0, Math.Min(1.0, value));
        }
    }
}