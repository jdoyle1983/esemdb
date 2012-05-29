using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsEmDb;

namespace EsEmDbServer
{
    public class SystemDb
    {
        private string DbRoot { get; set; }
        private EsEmDatabase SysDb { get; set; }

        public static void Create(string Path)
        {
            EsEmDatabase db = new EsEmDatabase();
            db.OpenDatabase(Path);
            
            EsEmTable DbsTable = db.CreateTable("Database");
            DbsTable.CreateColumn("Id", ColumnType.INTEGER, true, true);
            DbsTable.CreateColumn("Name", ColumnType.TEXT);
            DbsTable.CreateColumn("CreatedDate", ColumnType.DATETIME);
            DbsTable.CommitTable();

            EsEmTable UsersTable = db.CreateTable("User");
            UsersTable.CreateColumn("Id", ColumnType.INTEGER, true, true);
            UsersTable.CreateColumn("Login", ColumnType.TEXT);
            UsersTable.CreateColumn("Pass", ColumnType.TEXT);
            UsersTable.CommitTable();

            EsEmTable PrivTable = db.CreateTable("Priv");
            PrivTable.CreateColumn("Id", ColumnType.INTEGER, true, true);
            PrivTable.CreateColumn("DbId", ColumnType.INTEGER);
            PrivTable.CreateColumn("UserId", ColumnType.INTEGER);
            PrivTable.CreateColumn("Read", ColumnType.BOOL);
            PrivTable.CreateColumn("Write", ColumnType.BOOL);
            PrivTable.CreateColumn("Update", ColumnType.BOOL);
            PrivTable.CreateColumn("Delete", ColumnType.BOOL);
            PrivTable.CommitTable();
        }

        public SystemDb(string RootPath, string Path)
        {
            DbRoot = RootPath;
            SysDb = new EsEmDatabase();
            SysDb.OpenDatabase(Path);
        }

        public void SetUserCanRead(int UserId, int DbId)
        {
        }

        public void SetUserCanWrite(int UserId, int DbId)
        {
        }

        public void SetUserCanUpdate(int UserId, int DbId)
        {
        }

        public void SetUserCanDelete(int UserId, int DbId)
        {
        }

        public bool UserCanRead(int UserId, int DbId)
        {
            EsEmQuery q = SysDb.CreateQuery("SELECT * FROM Priv WHERE DbId=" + DbId.ToString() + " AND UserId=" + UserId);
            EsEmResult r = q.Execute();
            if (r.HasRows)
            {
                while (r.Read())
                    if (Convert.ToBoolean(r["Read"]))
                        return true;
            }
            return false;
        }

        public bool UserCanWrite(int UserId, int DbId)
        {
            EsEmQuery q = SysDb.CreateQuery("SELECT * FROM Priv WHERE DbId=" + DbId.ToString() + " AND UserId=" + UserId);
            EsEmResult r = q.Execute();
            if (r.HasRows)
            {
                while (r.Read())
                    if (Convert.ToBoolean(r["Write"]))
                        return true;
            }
            return false;
        }

        public bool UserCanUpdate(int UserId, int DbId)
        {
            EsEmQuery q = SysDb.CreateQuery("SELECT * FROM Priv WHERE DbId=" + DbId.ToString() + " AND UserId=" + UserId);
            EsEmResult r = q.Execute();
            if (r.HasRows)
            {
                while (r.Read())
                    if (Convert.ToBoolean(r["Update"]))
                        return true;
            }
            return false;
        }

        public bool UserCanDelete(int UserId, int DbId)
        {
            EsEmQuery q = SysDb.CreateQuery("SELECT * FROM Priv WHERE DbId=" + DbId.ToString() + " AND UserId=" + UserId);
            EsEmResult r = q.Execute();
            if (r.HasRows)
            {
                while (r.Read())
                    if (Convert.ToBoolean(r["Delete"]))
                        return true;
            }
            return false;
        }

        public void CreateDatabase(string DbName)
        {
            EsEmQuery q = SysDb.CreateQuery("INSERT INTO Database (Name, CreateDate) VALUES ('" + DbName + "','" + DateTime.Now.ToString() + "')");
            q.Execute();

            EsEmDatabase newDb = new EsEmDatabase();
            newDb.OpenDatabase(DbRoot + System.IO.Path.DirectorySeparatorChar + DbName + ".esemdb");
        }
    }
}
