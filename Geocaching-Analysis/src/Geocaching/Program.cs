using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Geocaching
{
    class Program
    {
        static void Main(string[] args)
        {
            //Check PocketQueries, save any new data.
            List<PocketQuery> queries = new WebExtractorPocketQuery().ExtractPocketQueries().ToList();

            using (var ctx = new DatabaseEntityContext())
            {
                foreach (PocketQuery pocketQuery in queries)
                {
                    try
                    {
                        SavePocketQuery(ctx, pocketQuery);
                    }
                    catch (System.Data.DataException e)
                    {
                        System.Console.WriteLine("{0} - {1}", e.Source, e.Message);
                    }
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

            //Update all geocaches before writing latest PocketQuery version to database to ensure all geocache information was saved first.
            foreach (Geocache cache in pocketQuery.Geocaches)
            {
                Geocache gc = ctx.Geocaches.AsNoTracking().Where(g => g.Code.Equals(cache.Code)).FirstOrDefault<Geocache>();
                if (gc != null)
                {
                    ctx.Geocaches.Attach(cache);
                    ctx.Entry(cache).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    ctx.Geocaches.Add(cache);
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
