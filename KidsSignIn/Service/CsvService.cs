using KidsSignIn.Model;
using LINQtoCSV;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KidsSignIn
{
    public class CsvService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CsvService));

        private CsvFileDescription cfd = new CsvFileDescription
        {
            SeparatorChar = ',',
            FirstLineHasColumnNames = true,
            EnforceCsvColumnAttribute = true,
            IgnoreUnknownColumns = true
        };

        /// <summary>
        /// Loads data from a CSV data file.
        /// </summary>
        public List<Child> LoadData(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                logger.Error("LoadData: filename was null or empty");
                return new List<Child>();
            }
            else
            {
                logger.DebugFormat("LoadData: loading data from {0}", filename);

                CsvContext context = new CsvContext();

                List<Child> children;

                try
                {
                    var tmp = context.Read<Child>(filename, cfd).AsQueryable<Child>();

                    children = (
                        from c in tmp
                        orderby c.Last, c.First
                        select c
                    ).ToList();

                }
                catch (Exception)
                {
                    // Horrible hack! Try a tab character instead of a comma as Excel can muck things up...
                    var tmp = context.Read<Child>(
                        filename,
                        new CsvFileDescription
                        {
                            SeparatorChar = '\t',
                            FirstLineHasColumnNames = cfd.FirstLineHasColumnNames,
                            EnforceCsvColumnAttribute = cfd.EnforceCsvColumnAttribute,
                            IgnoreUnknownColumns = cfd.IgnoreUnknownColumns
                        }).AsQueryable<Child>();

                    children = (
                        from c in tmp
                        orderby c.Last, c.First
                        select c
                    ).ToList();
                }

                foreach (var child in children)
                {
                    logger.DebugFormat("{0,3}: {1},{2}", child.Id, child.Last, child.First);
                }

                return children;
            }
        }

        /// <summary>
        /// Saves data to a CSV file.
        /// </summary>
        public void SaveData(string filename, List<Child> children)
        {
            if (string.IsNullOrEmpty(filename))
            {
                logger.Error("SaveData: filename was null or empty");
                return;
            }

            var signedInChildren = children.Where(c => c.SignedInAt != null).ToArray();
            if (signedInChildren == null || signedInChildren.Length == 0)
            {
                logger.Warn("SaveData: no children have been signed in");
            }

            logger.DebugFormat("SaveData: saving data to {0}", filename);

            CsvContext context = new CsvContext();

            context.Write<Child>(children, filename, cfd);
        }
    }
}
