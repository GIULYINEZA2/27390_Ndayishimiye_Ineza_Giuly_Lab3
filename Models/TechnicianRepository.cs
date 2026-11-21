using System;
using System.Data;

namespace LabTestSystem.Models;

public class TechnicianRepository : RepositoryBase
{
    public TechnicianRepository(string connectionString) : base(connectionString) { }

    // Returns null when the username or password is wrong.
    public Technician Login(string username, string password)
    {
        DataTable t = Load(@"SELECT TechnicianID, Username, FullName FROM Technicians
                             WHERE Username=@p0 AND PasswordHash=SHA2(@p1, 256)",
                           username, password);
        if (t.Rows.Count == 0) return null;

        var tech = new Technician
        {
            TechnicianID = Convert.ToInt32(t.Rows[0]["TechnicianID"]),
            Username = Convert.ToString(t.Rows[0]["Username"]),
            FullName = Convert.ToString(t.Rows[0]["FullName"])
        };

        DataTable sections = Load("SELECT SectionID FROM TechnicianSections WHERE TechnicianID=@p0",
                                  tech.TechnicianID);
        foreach (DataRow row in sections.Rows)
            tech.SectionIDs.Add(Convert.ToInt32(row["SectionID"]));
        return tech;
    }
}