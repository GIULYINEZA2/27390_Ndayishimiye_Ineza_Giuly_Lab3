using LabTestSystem.Models;

namespace LabTestSystem.Controllers;

// Creates the repositories and controllers once; every form uses these.
public static class AppServices
{
    public static readonly RequestController Requests;
    public static readonly TechnicianController Technicians;

    static AppServices()
    {
        string cs = AppConfig.ConnectionString;
        var examinees = new ExamineeRepository(cs);
        var requests = new LabRequestRepository(cs);
        var lookup = new LookupRepository(cs);
        var technicians = new TechnicianRepository(cs);

        Requests = new RequestController(examinees, requests, lookup);
        Technicians = new TechnicianController(technicians, requests, lookup);
    }
}