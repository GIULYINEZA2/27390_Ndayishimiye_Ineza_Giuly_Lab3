using System;
using System.Data;
using LabTestSystem.Models;

namespace LabTestSystem.Controllers;

public class ExamineeController
{
    private readonly ExamineeRepository _repository;

    public ExamineeController(ExamineeRepository repository)
    {
        _repository = repository;
    }

    public DataTable GetExaminees() => _repository.GetAll();

    public DataTable SearchExaminees(string keyword) =>
        _repository.Search(keyword.Trim());

    public void AddExaminee(Examinee examinee)
    {
        Validate(examinee);
        _repository.Insert(examinee);
    }

    public void EditExaminee(Examinee examinee)
    {
        if (examinee.ExamineeID <= 0)
            throw new ArgumentException("Select an examinee before updating.");
        Validate(examinee);
        _repository.Update(examinee);
    }

    public void RemoveExaminee(int examineeID)
    {
        if (examineeID <= 0)
            throw new ArgumentException("Select an examinee before deleting.");
        _repository.Delete(examineeID);
    }

    private static void Validate(Examinee examinee)
    {
        if (string.IsNullOrWhiteSpace(examinee.ExamineeNo) ||
            string.IsNullOrWhiteSpace(examinee.FullName))
            throw new ArgumentException("Complete all examinee fields.");

        if (examinee.FullName.Trim().Length < 2)
            throw new ArgumentException("Enter the examinee's full name.");
    }
}