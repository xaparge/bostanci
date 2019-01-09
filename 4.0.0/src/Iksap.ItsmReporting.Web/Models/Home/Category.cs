using Iksap.ItsmReporting.Web.Models.DataModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using Abp.Runtime.Security;
using Iksap.ItsmReporting.Authorization.Users;
using Iksap.ItsmReporting.Web.Controllers;
using System.Data;

namespace Iksap.ItsmReporting.Web.Models.Home
{
    public class Category : ITreeNode<Category>
    {
        private int _id;
        private string _name;
        private Category _parent;
        private IList<Category> _children;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Category Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public IList<Category> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public override bool Equals(object other)
        {
            Category otherCategory = other as Category;
            if (otherCategory == null) return false;
            return _id == otherCategory._id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static List<Category> GetListFromDatabase(string currentUserId)
        {
            //List<int> projects = new List<int>();
            //projects = getProjectListByTenant(currentUserId);

            MySqlConnection dbConn = new MySqlConnection("server=127.0.0.1; uid=root;pwd=" + System.Configuration.ConfigurationManager.AppSettings["DbPassword"].ToString() + "; database=itsmreporting_operations");

            MySqlCommand cmd = new MySqlCommand("itsmreporting_operations.slaProjectsByUsers", dbConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@user_id", currentUserId);

            dbConn.Open();

            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            List<Category> categories = new List<Category>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Category c = new Category();
                c.Id = Convert.ToInt32(dt.Rows[i][0]);
                c.Name = dt.Rows[i][1].ToString();
                if (dt.Rows[i][2] != DBNull.Value)
                {
                    c.Parent = new Category();
                    c.Parent.Id = Convert.ToInt32(dt.Rows[i][2]);
                }
                categories.Add(c);
            }


            // PARAMETRE EKLEMEDE SORUN VAR
            //DbCommand cmd = dbConn.CreateCommand();
            //cmd.CommandText = "itsmreporting_operations.slaProjectsByUsers";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Add(currentUserId);
            //dbConn.Open();
            //DbDataReader reader = cmd.ExecuteReader();

            //List<Category> categories = new List<Category>();
            //foreach (DbDataRecord row in reader)
            //{
            //    Category c = new Category();
            //    c.Id = (int)row["id"];
            //    c.Name = (string)row["name"];
            //    if (row["parent_id"] != DBNull.Value)
            //    {
            //        c.Parent = new Category();
            //        c.Parent.Id = (int)row["parent_id"];
            //    }
            //    categories.Add(c);
            //}
            //reader.Close();
            dbConn.Close();
            return categories;
        }

        //public static List<int> getProjectListByTenant(string tenant_id)
        //{
        //    MySqlConnection dbConn = new MySqlConnection("server=127.0.0.1; uid=root;pwd=" + System.Configuration.ConfigurationManager.AppSettings["DbPassword"].ToString() + "; database=itsmreporting_operations");
        //    dbConn.Open();
        //    string cmd_str = "SELECT project_id FROM map_tanent_project WHERE tenant_id=" + tenant_id;

        //    MySqlCommand cmd = new MySqlCommand(cmd_str, dbConn);
        //    DataTable dt = new DataTable();
        //    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
        //    da.Fill(dt);
        //    dbConn.Close();

        //    List<int> projects = new List<int>();
        //    DbDataReader reader = cmd.ExecuteReader();

        //    foreach (DbDataRecord row in reader)
        //    {
        //        projects.Add((int)row["project_id"]);
        //    }
        //    dbConn.Close();
        //    return projects;
        //}

        //public static List<Category> GetListFromDatabase2(DbConnection connection)
        //{
        ////    DbCommand command = connection.CreateCommand();
        ////    command.CommandText = "SELECT Id, Name, ParentID FROM Category";
        ////    command.CommandType = CommandType.Text;
        ////    DbDataReader reader = command.ExecuteReader();
        ////    List<Category> categories = new List<Category>();
        ////    foreach (DbDataRecord row in reader)
        ////    {
        ////        Category c = new Category();
        ////        c.Id = (int)row["Id"];
        ////        c.Name = (string)row["Name"];
        ////        if (row["ParentID"] != DBNull.Value)
        ////        {
        ////            c.Parent = new Category();
        ////            c.Parent.Id = (int)row["ParentID"];
        ////        }
        ////        categories.Add(c);
        ////    }
        ////    reader.Close();
        ////    return categories;
        //}
    }



}