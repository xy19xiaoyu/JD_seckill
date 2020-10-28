namespace JD_seckill
{
    public delegate void ShowLogEventHandler(string log);
    public interface IShowLog
    {
        void AddLog(string log);
    }
}