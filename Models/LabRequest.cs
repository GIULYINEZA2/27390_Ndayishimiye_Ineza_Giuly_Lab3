namespace LabTestSystem.Models;

public class LabRequest
{
    public int RequestID { get; set; }
    public int ExamineeID { get; set; }
    public int SectionID { get; set; }
    public string TestNames { get; set; } = "";
    public int? WorkstationID { get; set; }
    public int? TechnicianID { get; set; }
    public string Status { get; set; } = StatusRules.Requested;
    public string Notes { get; set; } = "";
    public string Results { get; set; } = "";
    public byte[] Signature { get; set; }
    public bool ResultsReleased { get; set; }

    public string Barcode => "LR" + RequestID.ToString("D6");
}