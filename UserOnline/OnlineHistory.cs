using System;

namespace UserOnline
{
    public class OnlineHistory
    {
        public int ID;
        public string Status;
        public DateTime Timestamp;

        public OnlineHistory(int id, string status, DateTime time)
        {
            ID = id;
            Status = status;
            Timestamp = time;
        }
    }
}
