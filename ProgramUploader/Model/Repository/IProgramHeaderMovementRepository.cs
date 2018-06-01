using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model.Repository
{
    public interface IProgramHeaderMovementRepository
    {
        ProgramHeaderMovement GetProgramHeaderMovementById(int id);
        List<ProgramHeaderMovement> GetProgramHeaderMovementsByProgramHeader(ProgramHeader programHeader);
        void SaveProgramHeaderMovements(List<ProgramHeaderMovement> programHeaderMovementList);

        Movement GetMovementByName(string name);
        Movement SaveMovement(Movement movement);
        bool MovementExists(string name);
        void SaveMomentList(List<Movement> list);
    }
}
