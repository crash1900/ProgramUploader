using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model
{
    public class ProgramHeaderMovement
    {
        public int ProgramHeaderMovementId { get; set; }
        public Movement Movement { get; set; }
        public ProgramHeader Header { get; set; }
        public float Weight { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
        public string Notes { get; set; }
        public string Rpe { get; set; }
    }
}
