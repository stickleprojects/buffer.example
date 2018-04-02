using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1
{
    public class FileInfo
    {
        [Key]
        [DatabaseGenerated((DatabaseGeneratedOption.Identity))]
        public int Id { get; set; }

        public string Details { get; set; }
        public int? WorkId { get; set; }
        public bool Done { get; set; }
    }

    public abstract class SQLBufferedDataProcessor<TRECORD> : BufferedDataProcessorWithFlush<TRECORD>
    {
        public class WithConnectionPerRecord : SQLBufferedDataProcessor<TRECORD>
        {
            public WithConnectionPerRecord(string connectionString, TimeSpan maxWaitTime, int maxRecords) : base(connectionString, maxWaitTime, maxRecords)
            {
            }

            protected override void onProcessRecords(IList<TRECORD> records)
            {
                Debug.WriteLine($"Processing {records.Count} records ctx per record");
                var s = Stopwatch.StartNew();

                foreach (var r in records)
                {
                    using (var ctx = new MyContext(ConnectionString))
                    {
                        ctx.FileInfos.Add(createRecord(r));

                        ctx.SaveChanges();
                    }
                }
                s.Stop();
                Debug.WriteLine("Elapsed " + s.ElapsedMilliseconds.ToString());
            }
        }

        public class WithConnectionPerListOfRecords : SQLBufferedDataProcessor<TRECORD>
        {
            public WithConnectionPerListOfRecords(string connectionString, TimeSpan maxWaitTime, int maxRecords) : base(connectionString, maxWaitTime, maxRecords)
            {
            }

            protected override void onProcessRecords(IList<TRECORD> records)
            {
                Debug.WriteLine($"Processing {records.Count} records with ctx per list");

                var s = Stopwatch.StartNew();

                using (var ctx = new MyContext(ConnectionString))
                {
                    ctx.ChangeTracker.AutoDetectChangesEnabled = false;

                    ctx.AddRange(records.Select(createRecord));

                    ctx.SaveChanges();
                }

                s.Stop();
                Debug.WriteLine("Elapsed " + s.ElapsedMilliseconds.ToString());
            }
        }

        public string ConnectionString { get; }

        private SQLBufferedDataProcessor(string connectionString, TimeSpan maxWaitTime, int maxRecords) : base(maxWaitTime, maxRecords)
        {
            ConnectionString = connectionString;
        }

        private FileInfo createRecord(TRECORD record)
        {
            return new FileInfo()
            {
                Details = record.ToString()
            };
        }
    }

    public class MyContext : DbContext
    {
        private readonly string _connectionString;
        public virtual DbSet<FileInfo> FileInfos { get; set; }

        public MyContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileInfo>(ba => ba.ToTable("FileInfo"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // get the configuration from the app settings
            if (!optionsBuilder.IsConfigured)
            {
                /*
                optionsBuilder.EnableSensitiveDataLogging(true);

                var lf = new NLogLoggerFactory(new NLogProviderOptions() { CaptureMessageProperties = true });
                lf.ConfigureNLog("nlog.config");

                optionsBuilder.UseLoggerFactory(lf);
                */

                // define the database to use
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}