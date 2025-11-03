namespace PerfectButler.GameSystem
{
    public enum StatType
    {
        Hunger,      // 허기
        Cleanliness, // 청결도
        Fun,         // 재미
        Health       // 건강
    }
    
    public enum GameState
    {
        Playing,
        GameOver,
        GameComplete,
        Paused
    }
}