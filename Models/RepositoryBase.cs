using System;
using System.Data;
using MySqlConnector;

namespace LabTestSystem.Models;

public abstract class RepositoryBase
{
    protected readonly string _connectionString;

    protected RepositoryBase(string connectionString)
    {
        _connectionString = connectionString;
    }

    // SELECT -> DataTable (reader loaded into memory, as in the notes)
    protected DataTable Load(string sql, params object[] args)
    {
        using var con = new MySqlConnection(_connectionString);
        using var cmd = MakeCommand(sql, con, args);
        con.Open();
        using var reader = cmd.ExecuteReader();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    // INSERT / UPDATE / DELETE -> rows affected
    protected int Run(string sql, params object[] args)
    {
        using var con = new MySqlConnection(_connectionString);
        using var cmd = MakeCommand(sql, con, args);
        con.Open();
        return cmd.ExecuteNonQuery();
    }

    // INSERT -> the new auto-increment ID
    protected long RunInsert(string sql, params object[] args)
    {
        using var con = new MySqlConnection(_connectionString);
        using var cmd = MakeCommand(sql, con, args);
        con.Open();
        cmd.ExecuteNonQuery();
        return cmd.LastInsertedId;
    }

    private static MySqlCommand MakeCommand(string sql, MySqlConnection con, object[] args)
    {
        var cmd = new MySqlCommand(sql, con);
        for (int i = 0; i < args.Length; i++)
            cmd.Parameters.AddWithValue("@p" + i, args[i] ?? DBNull.Value);
        return cmd;
    }
}