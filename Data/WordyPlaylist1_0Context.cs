using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WordyPlaylist1._0.Sentence;

namespace WordyPlaylist1._0.Models
{
    public class WordyPlaylist1_0Context : DbContext
    {
        public WordyPlaylist1_0Context (DbContextOptions<WordyPlaylist1_0Context> options)
            : base(options)
        {
        }

        public DbSet<WordyPlaylist1._0.Sentence.Class> Class { get; set; }
    }
}
