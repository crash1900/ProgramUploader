using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model
{
    public interface IProgramHeaderRepository
    {
        ProgramHeader GetProgramHeaderById(int id);
        ProgramHeader GetProgramHeaderByBlockWeekDay(int block, int week, int day);
        bool ProgramHeaderExistsForBlockWeekDay(int block, int week, int day);
        void UpdateExistingProgramHeader(ProgramHeader programHeader);

        void CreateProgramHeader(ProgramHeader header);

        void DeleteProgramHeader(ProgramHeader header);
        void AddAndUpdateProgramHeaders(List<ProgramHeader> headerList, List<Movement> movementList);
    }
}
