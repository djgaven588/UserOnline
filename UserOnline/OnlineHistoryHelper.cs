using System;
using System.Collections.Generic;

namespace UserOnline
{
    public class OnlineHistoryHelper
    {
        public void GetLastMonth(List<OnlineHistory> history)
        {
            // This could be vastly improved, but also how we use this can be improved.
            // Adding a database and ensuring there are updates at the beginning and end of a day would help vastly.
            // Instead of requiring data for previous time periods, we can directly use all history contained in a day.

            // Since we are using a database, we should actually just store the hours per day there. They won't need to be
            // updated after the fact, as it is historical, so storing them after a one time calculation which can happen
            // every day, or just when a new period of time is checked the first time, would make it simply providing existing
            // values to the client application, instead of needing to recalculate it every time.

            // While saving the calculation won't fix that this method is inefficent at scale, it will reduce the amount
            // of traffic through this method, greatly reducing the average response time, at the cost of some space in
            // the database, which we should use anyways due to IO speed compared to recalculating, since this will always be
            // more expensive than a lookup.

            // E = entries
            // U = users
            // D = days
            // UE = user entries

            // Assumptions
            DateTime lastMonthTime = new DateTime(2021, 5, 1, 0, 0, 0);
            DateTime lastMonthEndTime = new DateTime(2021, 6, 1, 0, 0, 0);
            int daysInMonth = DateTime.DaysInMonth(2021, 5);

            // Sort by time for all entries, slightly expensive but not too bad
            // log(E) operation
            history.Sort((x, y) => DateTime.Compare(x.Timestamp, y.Timestamp));

            // Sort by users, expensive operation at scale
            // E operation
            Dictionary<int, List<OnlineHistory>> userOnlineHistory = new Dictionary<int, List<OnlineHistory>>();
            foreach (var historyEntry in history)
            {
                if (userOnlineHistory.ContainsKey(historyEntry.ID))
                {
                    userOnlineHistory[historyEntry.ID].Add(historyEntry);
                }
                else
                {
                    userOnlineHistory.Add(historyEntry.ID, new List<OnlineHistory>() { historyEntry });
                }
            }

            // Calculate per user, unavoidable, can't do without iterating users
            // U operation
            foreach (var userEntry in userOnlineHistory)
            {
                int userId = userEntry.Key;
                Console.WriteLine($"--==  Calculating Month for ID {userId} ==--");
                List<OnlineHistory> userHistory = userEntry.Value;
                CalculateMonthForUser(userHistory, lastMonthTime, daysInMonth);
                Console.WriteLine("\n");
            }

            // Total complexity:
            // log(E) + E + D + UE
            // 2n + log(n)
            // We say 2n due to looping over (nearly) all entries twice, and the sorting operation.
        }

        // Expensive, iterating over all history entries provided for user
        /// <summary>
        /// Calculate a month worth of online time for a user.
        /// </summary>
        /// <param name="userId">The userID</param>
        /// <param name="userHistory">The user history</param>
        /// <param name="monthBegin">The beginning of the time period</param>
        /// <param name="daysInMonth">The total days in the time period</param>
        private static void CalculateMonthForUser(List<OnlineHistory> userHistory, DateTime monthBegin, int daysInMonth)
        {
            bool wasOnline = false;
            DateTime lastOnlineTime = DateTime.UnixEpoch;
            int currentHistoryIndex = 0;
            OnlineHistory currentHistory = userHistory[currentHistoryIndex];

            // Skip to first day in period, expensive if fed excessive data.
            while (currentHistory.Timestamp < monthBegin)
            {
                wasOnline = currentHistory.Status == "Online";
                lastOnlineTime = currentHistory.Timestamp;
                currentHistory = userHistory[++currentHistoryIndex];
            }

            // Loop per day, ignores history outside of range, nearly unavoidable cost.
            // D, UE operation
            for (int i = 1; i <= daysInMonth; i++)
            {
                float onlineTimeToday = 0;

                // Beginning of day, if we ended yesterday online, calculate from the start of today.
                if (wasOnline)
                {
                    lastOnlineTime = new DateTime(currentHistory.Timestamp.Year, currentHistory.Timestamp.Month, i);
                }

                // Loop for the day
                while (currentHistory.Timestamp.Day == i)
                {
                    bool nowOnline = currentHistory.Status == "Online";

                    // Went online
                    if (!wasOnline && nowOnline)
                    {
                        lastOnlineTime = currentHistory.Timestamp;
                    }
                    // Went offline
                    else if (wasOnline && !nowOnline)
                    {
                        onlineTimeToday += (float)(currentHistory.Timestamp - lastOnlineTime).TotalHours;
                    }

                    wasOnline = nowOnline;

                    // Continue iterating forwards
                    if (currentHistoryIndex + 1 < userHistory.Count)
                    {
                        currentHistory = userHistory[++currentHistoryIndex];
                    }
                    else
                    {
                        break;
                    }
                }

                // End of day, calculate time to end of day if previously online.
                if (wasOnline)
                {
                    float endAdditional = (float)(new DateTime(lastOnlineTime.Year, lastOnlineTime.Month, i).AddDays(1).AddTicks(-1) - lastOnlineTime).TotalHours;
                    onlineTimeToday += endAdditional;
                }

                Console.WriteLine($"Online {onlineTimeToday:0.00} hours on day {i}.");
            }
        }
    }
}
