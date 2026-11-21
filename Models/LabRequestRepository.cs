using System;
using System.Data;

namespace LabTestSystem.Models;

public class LabRequestRepository : RepositoryBase
{
    public LabRequestRepository(string connectionString) : base(connectionString) { }

    private const string ListSql =
        @"SELECT r.RequestID,
                 CONCAT('LR', LPAD(r.RequestID, 6, '0')) AS Barcode,
                 e.ExamineeNo, e.FullName, r.SectionID, s.SectionName,
                 r.TestNames, r.Status,
                 COALESCE(w.WorkstationNo, '') AS Workstation,
                 COALESCE(r.Notes, '') AS Notes,
                 r.ResultsReleased
          FROM LabRequests r
          JOIN Examinees e ON e.ExamineeID = r.ExamineeID
          JOIN LabSections s ON s.SectionID = r.SectionID
          LEFT JOIN Workstations w ON w.WorkstationID = r.WorkstationID ";

    public DataTable GetAll() => Load(ListSql + "ORDER BY r.RequestID");

    public DataTable GetByExamineeNo(string examineeNo) =>
        Load(ListSql + "WHERE e.ExamineeNo=@p0 ORDER BY r.RequestID", examineeNo);

    public DataTable GetOpen() =>
        Load(ListSql + @"WHERE r.ResultsReleased=0 AND r.Status <> 'REJECTED'
                         ORDER BY r.RequestID");

    public DataTable Search(string keyword) =>
        Load(ListSql + @"WHERE e.ExamineeNo LIKE @p0 OR e.FullName LIKE @p0
                            OR r.TestNames LIKE @p0 OR r.Status LIKE @p0
                         ORDER BY r.RequestID",
             "%" + keyword + "%");

    public LabRequest GetById(int requestID)
    {
        DataTable t = Load(@"SELECT RequestID, ExamineeID, SectionID, TestNames, WorkstationID,
                                    TechnicianID, Status, Notes, Results, Signature, ResultsReleased
                             FROM LabRequests WHERE RequestID=@p0", requestID);
        if (t.Rows.Count == 0) return null;
        DataRow r = t.Rows[0];
        return new LabRequest
        {
            RequestID = Convert.ToInt32(r["RequestID"]),
            ExamineeID = Convert.ToInt32(r["ExamineeID"]),
            SectionID = Convert.ToInt32(r["SectionID"]),
            TestNames = Convert.ToString(r["TestNames"]),
            WorkstationID = r["WorkstationID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["WorkstationID"]),
            TechnicianID = r["TechnicianID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["TechnicianID"]),
            Status = Convert.ToString(r["Status"]),
            Notes = r["Notes"] == DBNull.Value ? "" : Convert.ToString(r["Notes"]),
            Results = r["Results"] == DBNull.Value ? "" : Convert.ToString(r["Results"]),
            Signature = r["Signature"] as byte[],
            ResultsReleased = Convert.ToBoolean(r["ResultsReleased"])
        };
    }

    public long Insert(int examineeID, int sectionID, string testNames) =>
        RunInsert(@"INSERT INTO LabRequests (ExamineeID, SectionID, TestNames)
                    VALUES (@p0, @p1, @p2)", examineeID, sectionID, testNames);

    public void UpdateStatus(int requestID, string status, int? workstationID, int technicianID, string notes) =>
        Run(@"UPDATE LabRequests
              SET Status=@p1, WorkstationID=COALESCE(@p2, WorkstationID),
                  TechnicianID=@p3, Notes=@p4
              WHERE RequestID=@p0",
            requestID, status, workstationID, technicianID, notes);

    public void SaveResults(int requestID, string results, byte[] signature, bool release) =>
        Run(@"UPDATE LabRequests
              SET Results=@p1, Signature=COALESCE(@p2, Signature), ResultsReleased=@p3
              WHERE RequestID=@p0",
            requestID, results, signature, release ? 1 : 0);

    public void Delete(int requestID) =>
        Run("DELETE FROM LabRequests WHERE RequestID=@p0", requestID);
}