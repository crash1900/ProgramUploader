using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model.Repository
{
    public class SqlMovementRepository : IMovementRepository
    {
        private AscendDbEntities _ascendDb = new AscendDbEntities();

        public Movement GetMovementByName(string name)
        {
            var movement = _ascendDb.Movements.Where<Movement>(m => m.MovementName.Equals(name)).SingleOrDefault();
            return movement;
        }

        public bool MovementExists(string name)
        {
            return _ascendDb.Movements.Any(m => m.MovementName.Equals(name));
        }

        public void SaveMomentList(List<Movement> list)
        {
            _ascendDb.Movements.AddRange(list);
            _ascendDb.SaveChanges();
        }

        public Movement SaveMovement(Movement movement)
        {
            _ascendDb.Movements.Attach(movement);
            _ascendDb.SaveChanges();
            return movement;
        }
    }
}
