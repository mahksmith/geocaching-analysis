using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Geocaching
{
    class Program
    {
        static void Main(string[] args)
        {

            //Check PocketQueries, save any new data.
            List<PocketQuery> queries = new WebExtractorPocketQuery().ExtractPocketQueries().ToList();

            foreach (PocketQuery pocketQuery in queries)
            {
                //New Context every query decreases chances of error
                using (var ctx = new DatabaseEntityContext())
                {
                    SavePocketQuery(ctx, pocketQuery);
                }
            }
        }

        private static void SavePocketQuery(DatabaseEntityContext ctx, PocketQuery pocketQuery)
        {
            //There's no point download Pocket Queries that have not been updated.
            //TODO: Try to find if theres a different way to check if Pocket Queries differ without downloading?

            // Pocket Query update using http://www.entityframeworktutorial.net/EntityFramework4.3/update-entity-using-dbcontext.aspx

            PocketQuery pq = null;

            //1. Get pocketquery from DB
            pq = ctx.PocketQueries.AsNoTracking().Where(q => q.Name.Equals(pocketQuery.Name)).FirstOrDefault<PocketQuery>();

            //2. change stuff?
            if (pq != null && pq.DateGenerated.Equals(pocketQuery.DateGenerated))
            {
                Debug.WriteLine("Throwing away " + pocketQuery.Name);
                return;
            }

            //TODO I've selected ALL geocache codes, there must be a way to get only those in the PQ.
            //var databaseTest = ctx.Geocaches.AsNoTracking().Select(a => a.Code).Where(a => true).ToList();
            var databaseTest = ctx.Geocaches.AsNoTracking().Where(a => true).ToList();
            foreach (Geocache cache in pocketQuery.Geocaches)
            {
                if (databaseTest.Any(a => a.Code == cache.Code))
                {
                    ctx.Geocaches.Attach(cache);
                    ctx.Entry(cache).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    ctx.Geocaches.Add(cache);
                }

                foreach (Log log in cache.Logs)
                {
                    ctx.Logs.Add(log);
                }
            }

            if (pq != null)
            {
                ctx.PocketQueries.Attach(pocketQuery);
                ctx.Entry(pocketQuery).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                ctx.PocketQueries.Add(pocketQuery);
            }

            ctx.SaveChanges();
        }
    }
}
