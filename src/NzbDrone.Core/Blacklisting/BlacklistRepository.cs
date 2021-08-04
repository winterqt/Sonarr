using System.Collections.Generic;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Tv;

namespace NzbDrone.Core.Blacklisting
{
    public interface IBlacklistRepository : IBasicRepository<Blacklist>
    {
        List<Blacklist> BlacklistedByTitle(int seriesId, string sourceTitle);
        List<Blacklist> BlacklistedByTorrentInfoHash(int seriesId, string torrentInfoHash);
        List<Blacklist> BlacklistedBySeries(int seriesId);
    }

    public class BlacklistRepository : BasicRepository<Blacklist>, IBlacklistRepository
    {
        public BlacklistRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public List<Blacklist> BlacklistedByTitle(int seriesId, string sourceTitle)
        {
            return Query(e => e.SeriesId == seriesId && e.SourceTitle.Contains(sourceTitle));
        }

        public List<Blacklist> BlacklistedByTorrentInfoHash(int seriesId, string torrentInfoHash)
        {
            return Query(e => e.SeriesId == seriesId && e.TorrentInfoHash.Contains(torrentInfoHash));
        }

        public List<Blacklist> BlacklistedBySeries(int seriesId)
        {
            return Query(b => b.SeriesId == seriesId);
        }

        protected override SqlBuilder PagedBuilder() => new SqlBuilder().Join<Blacklist, Series>((b, m) => b.SeriesId == m.Id);
        protected override IEnumerable<Blacklist> PagedQuery(SqlBuilder sql) => _database.QueryJoined<Blacklist, Series>(sql, (bl, movie) =>
                    {
                        bl.Series = movie;
                        return bl;
                    });
    }
}
