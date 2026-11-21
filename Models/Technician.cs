using System.Collections.Generic;

namespace LabTestSystem.Models;

public class Technician
{
    public int TechnicianID { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public List<int> SectionIDs { get; } = new List<int>();

    public bool IsCertifiedFor(int sectionID) => SectionIDs.Contains(sectionID);
}