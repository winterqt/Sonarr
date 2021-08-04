﻿using Dapper;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedBlacklist : IHousekeepingTask
    {
        private readonly IMainDatabase _database;

        public CleanupOrphanedBlacklist(IMainDatabase database)
        {
            _database = database;
        }

        public void Clean()
        {
            using (var mapper = _database.OpenConnection())
            {
                mapper.Execute(@"DELETE FROM Blacklist
                                     WHERE Id IN (
                                     SELECT Blacklist.Id FROM Blacklist
                                     LEFT OUTER JOIN Series
                                     ON Blacklist.SeriesId = Series.Id
                                     WHERE Series.Id IS NULL)");
            }
        }
    }
}
