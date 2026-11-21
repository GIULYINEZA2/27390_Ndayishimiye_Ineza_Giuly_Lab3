using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LabTestSystem.Models;

namespace LabTestSystem.Controllers;

public class RequestController
{
    private readonly ExamineeRepository _examinees;
    private readonly LabRequestRepository _requests;
    private readonly LookupRepository _lookup;

    // Each test belongs to one laboratory section
    private static readonly Dictionary<string, string> TestSections = new Dictionary<string, string>
    {
        { "Complete Blood Count", "Hematology" },
        { "Blood Glucose", "Biochemistry" },
        { "Lipid Profile", "Biochemistry" },
        { "Urinalysis", "Microbiology" }
    };

    public static IEnumerable<string> TestNames => TestSections.Keys;

    public RequestController(ExamineeRepository examinees, LabRequestRepository requests, LookupRepository lookup)
    {
        _examinees = examinees;
        _requests = requests;
        _lookup = lookup;
    }

    public Examinee FindExaminee(string examineeNo) =>
        string.IsNullOrWhiteSpace(examineeNo) ? null : _examinees.FindByNo(examineeNo.Trim());

    // Tests from different sections become one request per section.
    // Returns one line per request, such as "LR000001 - Hematology: Complete Blood Count".
    public List<string> SubmitRequest(string examineeNo, string fullName, List<string> tests)
    {
        examineeNo = (examineeNo ?? "").Trim();
        if (examineeNo.Length == 0)
            throw new ArgumentException("Enter the examinee number.");
        if (tests == null || tests.Count == 0)
            throw new ArgumentException("Select at least one test.");

        Examinee examinee = _examinees.FindByNo(examineeNo);
        if (examinee == null)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length < 2)
                throw new ArgumentException("This is a new examinee. Enter the full name.");
            _examinees.Insert(new Examinee { ExamineeNo = examineeNo, FullName = fullName.Trim() });
            examinee = _examinees.FindByNo(examineeNo);
        }

        var created = new List<string>();
        foreach (var group in tests.GroupBy(t => TestSections[t]))
        {
            int sectionID = _lookup.GetSectionId(group.Key);
            string names = string.Join(", ", group);
            long id = _requests.Insert(examinee.ExamineeID, sectionID, names);
            created.Add("LR" + id.ToString("D6") + " - " + group.Key + ": " + names);
        }
        return created;
    }

    public DataTable GetRequests() => _requests.GetAll();

    public DataTable SearchRequests(string keyword) => _requests.Search((keyword ?? "").Trim());

    public DataTable GetRequestsOf(string examineeNo)
    {
        if (string.IsNullOrWhiteSpace(examineeNo))
            throw new ArgumentException("Enter your examinee number.");
        return _requests.GetByExamineeNo(examineeNo.Trim());
    }

    public LabRequest GetRequest(int requestID) => _requests.GetById(requestID);

    public void RemoveRequest(int requestID)
    {
        if (requestID <= 0)
            throw new ArgumentException("Select a request first.");
        LabRequest r = _requests.GetById(requestID);
        if (r == null)
            throw new ArgumentException("The request was not found.");
        if (r.Status != StatusRules.Requested)
            throw new ArgumentException("Only requests that are still REQUESTED can be deleted.");
        _requests.Delete(requestID);
    }
}