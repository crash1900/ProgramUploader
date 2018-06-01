using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model.Repository
{
    public class SqlProgramHeaderMovementRepository : IProgramHeaderMovementRepository
    {
        AscendDbEntities _ascendDb = new AscendDbEntities();

        public ProgramHeaderMovement GetProgramHeaderMovementById(int id)
        {
            var programHeaderMovement = _ascendDb.ProgramHeaderMovements.Where(hm => hm.Id == id).SingleOrDefault();
            return programHeaderMovement;
        }

        public List<ProgramHeaderMovement> GetProgramHeaderMovementsByProgramHeader(ProgramHeader programHeader)
        {
            var programHeaderMovementList = _ascendDb.ProgramHeaderMovements.Where(hm => hm.ProgramHeader.Id == programHeader.Id).ToList();
            return programHeaderMovementList;
        }

        public void SaveProgramHeaderMovements(List<ProgramHeaderMovement> programHeaderMovementList)
        {
            _ascendDb.ProgramHeaderMovements.AddRange(programHeaderMovementList);
            _ascendDb.SaveChanges();
        }

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
