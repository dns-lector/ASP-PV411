namespace ASP_PV411.Services.Timestamp
{
    public class UnixSecondsTimestampService : ITimestampService
    {
        public long Timestamp()
        {
            return (DateTime.Now.Ticks - DateTime.UnixEpoch.Ticks) / 10000000;
        }
    }
}
