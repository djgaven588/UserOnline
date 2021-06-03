using System;
using System.Collections.Generic;

namespace UserOnline
{
    class Program
    {
        static void Main(string[] args)
        {
            OnlineHistoryHelper helper = new OnlineHistoryHelper();
            helper.GetLastMonth(new List<OnlineHistory>() 
            {
                new OnlineHistory(5, "Online", new DateTime(2021, 5, 8, 3, 0, 0)),
                new OnlineHistory(5, "Online", new DateTime(2021, 5, 1, 5, 0, 0)),
                new OnlineHistory(5, "Online", new DateTime(2021, 5, 1, 3, 0, 0)),
                new OnlineHistory(5, "Offline", new DateTime(2021, 5, 1, 4, 0, 0)),
                new OnlineHistory(5, "Offline", new DateTime(2021, 5, 1, 6, 0, 0)),
                new OnlineHistory(5, "Offline", new DateTime(2021, 5, 11, 6, 0, 0)),
                new OnlineHistory(5, "Online", new DateTime(2021, 4, 29, 13, 0, 0)),
                new OnlineHistory(5, "Online", new DateTime(2021, 5, 29, 3, 0, 0)),

                new OnlineHistory(3, "Offline", new DateTime(2021, 5, 1, 5, 0, 0)),
                new OnlineHistory(3, "Offline", new DateTime(2021, 5, 1, 6, 0, 0)),
                new OnlineHistory(3, "Online", new DateTime(2021, 5, 8, 3, 10, 0)),
                new OnlineHistory(3, "Online", new DateTime(2021, 5, 1, 5, 0, 1)),
                new OnlineHistory(3, "Online", new DateTime(2021, 5, 1, 3, 0, 0)),
                new OnlineHistory(3, "Offline", new DateTime(2021, 5, 11, 6, 0, 0)),
                new OnlineHistory(3, "Online", new DateTime(2021, 4, 28, 13, 0, 0)),
                new OnlineHistory(3, "Online", new DateTime(2021, 5, 28, 3, 0, 0)),
            });

            Console.ReadKey();
        }
    }
}
