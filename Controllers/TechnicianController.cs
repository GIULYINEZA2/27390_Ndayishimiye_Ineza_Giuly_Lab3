using System;
using System.Data;
using System.Linq;
using LabTestSystem.Models;

namespace LabTestSystem.Controllers;

public class TechnicianController
{
    private readonly TechnicianRepository _technicians;
    private readonly LabRequestRepository _requests;
    private readonly LookupRepository _lookup;

    public Technician CurrentTechnician { get; private set; }
    public bool IsLoggedIn => CurrentTechnician != null;

    public TechnicianController(TechnicianRepository technicians, LabRequestRepository requests, LookupRepository lookup)
    {
        _technicians = technicians;
        _requests = requests;
        _lookup = lookup;
    }

    public DataTable GetWorkstations() => _lookup.GetWorkstations();

    public void Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new ArgumentException("Enter your username and password.");

        Technician tech = _technicians.Login(username.Trim(), password);
        if (tech == null)
            throw new ArgumentException("Wrong username or password.");
        CurrentTechnician = tech;
    }

    public void Logout() => CurrentTechnician = null;

    // Open requests, only from the sections this technician is certified for
    public DataTable GetOpenRequests()
    {
        RequireLogin();
        DataTable all = _requests.GetOpen();
        if (CurrentTechnician.SectionIDs.Count == 0) return all.Clone();

        string ids = string.Join(",", CurrentTechnician.SectionIDs);   // numbers only
        var view = new DataView(all) { RowFilter = "SectionID IN (" + ids + ")" };
        return view.ToTable();
    }

    public LabRequest GetRequest(int requestID)
    {
        RequireLogin();
        LabRequest r = _requests.GetById(requestID);
        if (r == null) throw new ArgumentException("The request was not found.");
        return r;
    }

    public void ChangeStatus(int requestID, string newStatus, int workstationID, string notes)
    {
        RequireLogin();
        LabRequest r = GetRequest(requestID);

        if (!CurrentTechnician.IsCertifiedFor(r.SectionID))
            throw new UncertifiedSectionException(
                CurrentTechnician.FullName + " is not certified for this request's laboratory section.");

        if (!StatusRules.IsAllowed(r.Status, newStatus))
            throw new InvalidStateTransitionException(r.Status, newStatus);

        int? workstation = null;
        if (newStatus == StatusRules.Sampled)
        {
            if (_lookup.GetWorkstationSectionId(workstationID) != r.SectionID)
                throw new UncertifiedSectionException(
                    "The selected workstation belongs to a different laboratory section.");
            workstation = workstationID;
        }

        string note = null;
        if (newStatus == StatusRules.Rejected)
        {
            if (string.IsNullOrWhiteSpace(notes))
                throw new ArgumentException("Enter the reason for rejecting this request.");
            note = notes.Trim();
        }

        _requests.UpdateStatus(requestID, newStatus, workstation, CurrentTechnician.TechnicianID, note);
    }

    // release = false: save the findings privately. release = true: the examinee can now see them.
    public void SaveResults(int requestID, string results, byte[] signaturePng, bool release)
    {
        RequireLogin();
        LabRequest r = GetRequest(requestID);

        if (!CurrentTechnician.IsCertifiedFor(r.SectionID))
            throw new UncertifiedSectionException(
                CurrentTechnician.FullName + " is not certified for this request's laboratory section.");
        if (r.Status != StatusRules.Processed)
            throw new ArgumentException("Results can only be entered for PROCESSED requests.");
        if (r.ResultsReleased)
            throw new ArgumentException("The results were already released.");
        if (string.IsNullOrWhiteSpace(results))
            throw new ArgumentException("Enter the findings first.");
        if (release && signaturePng == null && r.Signature == null)
            throw new ArgumentException("Sign with the mouse before releasing the results.");

        _requests.SaveResults(requestID, results.Trim(), signaturePng, release);
    }

    private void RequireLogin()
    {
        if (CurrentTechnician == null)
            throw new ArgumentException("Please log in first.");
    }
}