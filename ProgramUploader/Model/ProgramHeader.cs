using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model
{
    public class ProgramHeader
    {
        public int Id { get; set; }
        public int BlockNo { get; set; }
        public int WeekNo { get; set; }
        public int DayNo { get; set; }
    }
}
