using System.Data;
using MySqlConnector;

namespace LabTestSystem.Models;

public class ExamineeRepository
{
    private readonly string _connectionString;

    public ExamineeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable GetAll()
    {
        const string sql = @"SELECT ExamineeID, ExamineeNo, FullName
                             FROM Examinees ORDER BY ExamineeID";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        con.Open();
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    public void Insert(Examinee examinee)
    {
        const string sql = @"INSERT INTO Examinees (ExamineeNo, FullName)
                             VALUES (@no, @name)";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@no", MySqlDbType.VarChar, 30).Value = examinee.ExamineeNo;
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = examinee.FullName;
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public void Update(Examinee examinee)
    {
        const string sql = @"UPDATE Examinees
                             SET ExamineeNo=@no, FullName=@name
                             WHERE ExamineeID=@id";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@no", MySqlDbType.VarChar, 30).Value = examinee.ExamineeNo;
        cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = examinee.FullName;
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = examinee.ExamineeID;
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public void Delete(int examineeID)
    {
        const string sql = "DELETE FROM Examinees WHERE ExamineeID=@id";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = examineeID;
        con.Open();
        cmd.ExecuteNonQuery();
    }

    public DataTable Search(string keyword)
    {
        const string sql = @"SELECT ExamineeID, ExamineeNo, FullName
                             FROM Examinees
                             WHERE ExamineeNo LIKE @key OR FullName LIKE @key
                             ORDER BY ExamineeID";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@key", MySqlDbType.VarChar, 150).Value = "%" + keyword + "%";
        con.Open();
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }
    public Examinee FindByNo(string examineeNo)
    {
        const string sql = @"SELECT ExamineeID, ExamineeNo, FullName
                             FROM Examinees WHERE ExamineeNo=@no";
        using var con = new MySqlConnection(_connectionString);
        using var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.Add("@no", MySqlDbType.VarChar, 30).Value = examineeNo;
        con.Open();
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new Examinee
        {
            ExamineeID = Convert.ToInt32(reader["ExamineeID"]),
            ExamineeNo = Convert.ToString(reader["ExamineeNo"]),
            FullName = Convert.ToString(reader["FullName"])
        };
    }
}