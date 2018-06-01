using ProgramUploader.Model;
using ProgramUploader.Model.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramUploader
{
    public partial class ProgramUpload : Form
    {
        public ProgramUpload()
        {
            InitializeComponent();
        }

        private void ProgramUpload_Load(object sender, EventArgs e)
        {
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Browse Programs";
            openFileDialog1.DefaultExt = "xlsx";
            openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog1.FileName;
                string programFile = txtFileName.Text;

                using (OleDbConnection connection = new OleDbConnection())
                {
                    string fileExtension = Path.GetExtension(programFile);
                    if (fileExtension == ".xls")
                        connection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + programFile + ";" + "Extended Properties='Excel 8.0;HDR=YES;'";
                    if (fileExtension == ".xlsx")
                        connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + programFile + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";


                    connection.Open();
                    DataTable dtSheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    foreach (DataRow row in dtSheet.Rows)
                    {
                        string sheetName = row["TABLE_NAME"].ToString();

                        if (sheetName.EndsWith("$"))
                        {
                            // Get the sheet
                            using (OleDbCommand cmd = connection.CreateCommand())
                            {
                                cmd.CommandText = String.Format("SELECT * FROM [{0}]", sheetName);
                                DataTable dt = new DataTable();
                                dt.TableName = sheetName;
                                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                da.Fill(dt);

                                var dataWidth = dt.Columns.Count;

                                var blockNoRow = 2;
                                // Get block number
                                var blockNo = GetBlockNumber(dt.Rows[blockNoRow][0].ToString());

                                // Get the column indices of each week in a block
                                List<int> weekStartColumnIndices = GetweekStartColumns(dt.Rows[Constants.WeekRow], dataWidth);

                                var programLength = dt.Rows.Count;

                                // Create a list of program headers
                                var headerList = new List<ProgramHeader>();
                                var movementList = new List<Movement>();
                                var programHeaderMovementList = new List<ProgramHeaderMovement>();

                                var headerId = 0; // Dummy header id so EF can link things properly
                                var movementId = 0; // Dummy movement Id for EF to link entities

                                // For each week, get the start of rows for the days
                                Dictionary<int, List<int>> weekAndDayColumnList = GetWeekAndDayColumnList(weekStartColumnIndices, programLength, dt);

                                IProgramHeaderMovementRepository hmRepo = new SqlProgramHeaderMovementRepository();

                                foreach (int week in weekAndDayColumnList.Keys)
                                {
                                    int rowCount = weekAndDayColumnList[week].Count;
                                    bool weekFound = false;
                                    var weekNo = 0;

                                    foreach (int day in weekAndDayColumnList[week])
                                    {
                                        ProgramHeader header = new ProgramHeader();
                                        headerList.Add(header);
                                        header.BlockNo = blockNo;
                                        headerId--; // Decrease the headerId first each new header

                                        if (!weekFound)
                                        {
                                            weekNo = GetWeekNo(dt.Rows[day - 1][week].ToString());
                                            weekFound = true;
                                        }
                                        header.WeekNo = weekNo;
                                        header.DayNo = GetDayNo(dt.Rows[day][week].ToString());
                                        header.Id = headerId;

                                        int rowToScan = day + 2; // Skip the header row for the exercises

                                        headerList.Add(header);

                                        // Get exercises for that day
                                        while (rowToScan < programLength 
                                            && !String.IsNullOrEmpty(dt.Rows[rowToScan][week].ToString()))
                                        {
                                            ProgramHeaderMovement headerMovement = new ProgramHeaderMovement();
                                            programHeaderMovementList.Add(headerMovement);

                                            var exerciseValue = dt.Rows[rowToScan][week].ToString().Trim();
                                            Movement mov = null;

                                            if ((mov = movementList.Where(m => String.Compare(m.MovementName, exerciseValue, StringComparison.CurrentCultureIgnoreCase) == 0).FirstOrDefault()) != null)
                                            {
                                                headerMovement.ProgramHeader = header;
                                                headerMovement.ProgramHeaderId = header.Id;
                                                headerMovement.Movement = mov;
                                                headerMovement.MovementId = mov.Id;
                                            }
                                            else if ((mov = hmRepo.GetMovementByName(exerciseValue)) != null)
                                            {
                                                headerMovement.ProgramHeader = header;
                                                headerMovement.ProgramHeaderId = header.Id;
                                                headerMovement.Movement = mov;
                                                headerMovement.MovementId = mov.Id;
                                            }
                                            else
                                            {
                                                mov = new Movement();
                                                movementId--; // Decrease the movementId first each new movment
                                                mov.Id = movementId;
                                                mov.MovementName = exerciseValue.Trim();
                                                headerMovement.ProgramHeader = header;
                                                headerMovement.MovementId = movementId;
                                                movementList.Add(mov);
                                            }

                                            var weightValue = dt.Rows[rowToScan][week + 1].ToString();
                                            var repsValue = dt.Rows[rowToScan][week + 2].ToString();
                                            var setsValue = dt.Rows[rowToScan][week + 3].ToString();
                                            var notesValue = dt.Rows[rowToScan][week + 4].ToString();
                                            var rpeValue = dt.Rows[rowToScan][week + 5].ToString();
                                            decimal decWeight;
                                            int intReps;
                                            int intSets;

                                            if (!String.IsNullOrEmpty(weightValue))
                                            {
                                                if (decimal.TryParse(weightValue, out decWeight))
                                                {
                                                    headerMovement.Weight = decWeight;
                                                }
                                            }


                                            if (!String.IsNullOrEmpty(repsValue))
                                            {
                                                if (int.TryParse(repsValue, out intReps))
                                                {
                                                    headerMovement.Reps = intReps;
                                                }
                                            }

                                            if (!String.IsNullOrEmpty(setsValue))
                                            {
                                                if (int.TryParse(setsValue, out intSets))
                                                {
                                                    headerMovement.Sets = intSets;
                                                }
                                            }

                                            if (!String.IsNullOrWhiteSpace(rpeValue))
                                            {
                                                headerMovement.RPE = rpeValue;
                                            }

                                            if (!String.IsNullOrWhiteSpace(notesValue))
                                            {
                                                headerMovement.Notes = notesValue;
                                            }
                                            rowToScan++;
                                        }
                                    }
                                }
                                
                                hmRepo.SaveProgramHeaderMovements(programHeaderMovementList);
                                MessageBox.Show("Uploaded successfully", "Upload Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
        }

        private int GetDayNo(string text)
        {

            int dayNo;
            int dayIndex = text.IndexOf("Day", 0, StringComparison.CurrentCultureIgnoreCase);

            if ((dayIndex != -1) && int.TryParse(text.Substring(dayIndex + 4), out dayNo))
            {
                return dayNo;
            }
            throw new Exception(String.Format("Cannot find day number in: {0}", text));
        }

        private int GetWeekNo(string text)
        {
            int weekNo;
            int weekIndex = text.IndexOf("Week", 0, StringComparison.CurrentCultureIgnoreCase);

            if ((weekIndex != -1) && int.TryParse(text.Substring(weekIndex + 5), out weekNo))
            {
                return weekNo;
            }
            throw new Exception(String.Format("Cannot find week number in: {0}", text));
        }
        
        private int GetBlockNumber(string text)
        {
            int blockNumber;
            int blockIndex = text.IndexOf("Block", 0, StringComparison.CurrentCultureIgnoreCase);

            if ((blockIndex != -1) && int.TryParse(text.Substring(blockIndex + 6), out blockNumber))
            {
                return blockNumber;
            }
            throw new Exception(String.Format("Cannot find block number in: {0}", text));
        }

        private Dictionary<int, List<int>> GetWeekAndDayColumnList(List<int> weekStartColumnIndices, int programLength, DataTable dt)
        {
            Dictionary<int, List<int>> weekAndDayColumnList = new Dictionary<int, List<int>>();

            foreach (int weekStartColumn in weekStartColumnIndices)
            {
                for (int currentRow = Constants.WeekRow + 1; currentRow < programLength; currentRow++)
                {
                    string rowContent = dt.Rows[currentRow][weekStartColumn].ToString();
                    
                    if (!String.IsNullOrEmpty(rowContent) && rowContent.StartsWith("Day"))
                    {
                        if (!weekAndDayColumnList.ContainsKey(weekStartColumn))
                        {
                            weekAndDayColumnList.Add(weekStartColumn, new List<int> { currentRow });
                        }
                        else
                        {
                            weekAndDayColumnList[weekStartColumn].Add(currentRow);
                        }
                    }
                }
            }

            return weekAndDayColumnList;
        }

        private List<int> GetweekStartColumns(DataRow dr, int dataWidth)
        {
            if (dr == null)
            {
                throw new ArgumentNullException(nameof(dr));
            }

            List<int> columnList = new List<int>();

            for (int i = 0; i < dataWidth; i++)
            {
                if (dr[i].ToString().StartsWith("Week"))
                {
                    columnList.Add(i);
                }
            }
            return columnList;
        }

        private int GetDayValue(string rowValue)
        {
            int dayValue = 0;
            string dayString = rowValue.Substring(3, rowValue.Length - 3);

            if (int.TryParse(dayString, out dayValue))
                return dayValue;
            else
                throw new Exception(String.Format("Dayvalue is not an integer. Actual: {0}", rowValue));
        }

        private int GetDayOrWeekValue(string rowValue, bool isDayValue)
        {
            int intValue = 0;
            int offset;

            if (isDayValue)
                offset = 3;
            else
                offset = 4;

            string valueString = rowValue.Substring(offset, rowValue.Length - offset);

            if (int.TryParse(valueString, out intValue))
                return intValue;
            else
                throw new Exception(String.Format("RowValue does not an integer at expected position. Actual: {0}", valueString));
        }

        private int GetNextDayStart(int dayValue)
        {
            throw new NotImplementedException();
        }
    }
}
