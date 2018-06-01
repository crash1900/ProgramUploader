using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader.Model.Repository
{
    public class SqlProgramHeaderRepository : IProgramHeaderRepository
    {
        private AscendDbEntities _ascendDb = new AscendDbEntities();

        public void CreateProgramHeader(ProgramHeader header)
        {
            _ascendDb.ProgramHeaders.Add(header);
            _ascendDb.SaveChanges();
        }

        public void DeleteProgramHeader(ProgramHeader header)
        {
            _ascendDb.ProgramHeaders.Remove(header);
            _ascendDb.SaveChanges();
        }

        public ProgramHeader GetProgramHeaderByBlockWeekDay(int block, int week, int day)
        {
            var programHeader = _ascendDb.ProgramHeaders.Where(p => p.BlockNo == block && p.WeekNo == week && p.DayNo == day).SingleOrDefault();
            return programHeader;
        }

        public ProgramHeader GetProgramHeaderById(int id)
        {
            var programHeader = _ascendDb.ProgramHeaders.Where(p => p.Id == id).SingleOrDefault();
            return programHeader;
        }

        public bool ProgramHeaderExistsForBlockWeekDay(int block, int week, int day)
        {
            return _ascendDb.ProgramHeaders.Where(p => p.BlockNo == block && p.WeekNo == week && p.DayNo == day).Count() > 0;
        }

        public void UpdateExistingProgramHeader(ProgramHeader programHeader)
        {
            var updatedHeader = _ascendDb.ProgramHeaders.Where(p => p.Id == programHeader.Id).SingleOrDefault();

            if (updatedHeader == null)
            {
                throw new Exception(String.Format("Error updating a non-existent program header with id: {0}", programHeader.Id));
            }

            updatedHeader.BlockNo = programHeader.BlockNo;
            updatedHeader.WeekNo = programHeader.WeekNo;
            updatedHeader.DayNo = programHeader.DayNo;

            _ascendDb.SaveChanges();
        }

        public void AddAndUpdateProgramHeaders(List<ProgramHeader> headerList, List<Movement> movementList)
        {
            _ascendDb.Movements.AddRange(movementList);

            foreach (ProgramHeader header in headerList)
            {
                List<ProgramHeader> toBeRemoved = _ascendDb.ProgramHeaders.Where(h => h.BlockNo == header.BlockNo &&
                h.WeekNo == header.WeekNo && h.DayNo == header.DayNo).ToList();

                var innerQuery = from x in _ascendDb.ProgramHeaders
                              where x.BlockNo == header.BlockNo
                              && x.WeekNo == header.WeekNo
                              && x.DayNo == header.DayNo
                              select x.Id;

                var headerMovementList = from hm in _ascendDb.ProgramHeaderMovements
                              where innerQuery.Contains(hm.ProgramHeaderId)
                              select hm;

                _ascendDb.ProgramHeaderMovements.RemoveRange(headerMovementList);

                _ascendDb.ProgramHeaders.RemoveRange(toBeRemoved);
            }

            _ascendDb.ProgramHeaders.AddRange(headerList);
            _ascendDb.SaveChanges();
        }
    }
}
