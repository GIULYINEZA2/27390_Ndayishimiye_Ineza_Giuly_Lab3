using System;
using System.Data;

namespace LabTestSystem.Models;

public class LookupRepository : RepositoryBase
{
    public LookupRepository(string connectionString) : base(connectionString) { }

    public int GetSectionId(string sectionName)
    {
        DataTable t = Load("SELECT SectionID FROM LabSections WHERE SectionName=@p0", sectionName);
        if (t.Rows.Count == 0) throw new ArgumentException("Unknown laboratory section: " + sectionName);
        return Convert.ToInt32(t.Rows[0]["SectionID"]);
    }

    public DataTable GetWorkstations() =>
        Load(@"SELECT w.WorkstationID, w.WorkstationNo, w.SectionID, s.SectionName,
                      CONCAT(w.WorkstationNo, ' - ', s.SectionName) AS Display
               FROM Workstations w
               JOIN LabSections s ON s.SectionID = w.SectionID
               ORDER BY w.WorkstationID");

    public int GetWorkstationSectionId(int workstationID)
    {
        DataTable t = Load("SELECT SectionID FROM Workstations WHERE WorkstationID=@p0", workstationID);
        return t.Rows.Count == 0 ? -1 : Convert.ToInt32(t.Rows[0]["SectionID"]);
    }
}