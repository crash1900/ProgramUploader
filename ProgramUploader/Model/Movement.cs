using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model
{
    public class Movement
    {
        private string _movementName;

        public Movement(string movementName)
        {
            _movementName = movementName;
        }

        public int Id { get; set; }
        public string MovementName
        {
            get
            {
                return _movementName;
            }
        }
    }
}
