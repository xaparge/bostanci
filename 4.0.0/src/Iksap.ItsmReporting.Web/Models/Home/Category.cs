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

        public static List<Category> projects;
        public static List<Category> GetListFromDatabase(int currentUserId)
        {
            MySqlConnection dbConn = new MySqlConnection("server=127.0.0.1; uid=root; pwd=" + System.Configuration.ConfigurationManager.AppSettings["DbPassword"].ToString() + "; database=itsmreporting_operations");
            dbConn.Open();
            MySqlCommand cmd = new MySqlCommand("allProjects", dbConn);
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);

            projects = new List<Category>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Category c = new Category();
                c.Id = (int)dt.Rows[i][0];
                c.Name = (string)dt.Rows[i][1];
                if (dt.Rows[i][2] != DBNull.Value)
                {
                    c.Parent = new Category();
                    c.Parent.Id = (int)dt.Rows[i][2];
                }
                projects.Add(c);
            }

            List<Category> categories = new List<Category>();

            MySqlCommand cmd2 = new MySqlCommand("slaProjectsByUsers", dbConn);
            cmd2.Parameters.AddWithValue("@user_id", currentUserId);
            cmd2.CommandType = CommandType.StoredProcedure;

            DataTable dt2 = new DataTable();
            MySqlDataAdapter da2 = new MySqlDataAdapter(cmd2);
            da2.Fill(dt2);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                Category c = new Category();
                c.Id = Convert.ToInt32(dt2.Rows[i][0]);
                c.Name = dt2.Rows[i][1].ToString();
                if (dt2.Rows[i][2] != DBNull.Value)
                {
                    c.Parent = new Category();
                    c.Parent.Id = Convert.ToInt32(dt2.Rows[i][2]);
                }
                categories.Add(c);
            }
            dbConn.Close();

            // Bir alt projeleri idlerini bulma
            List<int> subProjects = new List<int>();
            for (int i = 0; i < categories.Count; i++)
            {
                if (!subProjects.Contains(categories[i].Id))
                    subProjects.Add(categories[i].Id);
            }

            List<int> subProjects2 = new List<int>();
            for (int i = 0; i < subProjects.Count; i++)     // Varsa ilk alt projeleri bulur
            {
                for (int j = 0; j < projects.Count; j++)
                {
                    try
                    {
                        if (projects[j].Parent.Id == subProjects[i])
                        {
                            categories.Add(projects[j]);
                            if (!subProjects.Contains(projects[j].Id))
                                subProjects2.Add(projects[j].Id);
                        }
                    } catch { }
                }
            }

            List<int> subProjects3 = new List<int>();     // Varsa ikinci alt projeleri bulur
            for (int i = 0; i < subProjects2.Count; i++)
            {
                for (int j = 0; j < projects.Count; j++)
                {
                    try
                    {
                        if (projects[j].Parent.Id == subProjects2[i])
                        {
                            categories.Add(projects[j]);
                            if (!subProjects2.Contains(projects[j].Id))
                                subProjects3.Add(projects[j].Id);
                        }
                    }
                    catch { }
                }
            }

            for (int i = 0; i < subProjects3.Count; i++)     // Varsa üçüncü alt projeleri bulur
            {
                for (int j = 0; j < projects.Count; j++)
                {
                    try
                    {
                        if (projects[j].Parent.Id == subProjects3[i])
                        {
                            categories.Add(projects[j]);
                        }
                    }
                    catch { }
                }
            }

            return categories;
        }

        //public static List<List<int>> subProjects = new List<List<int>>();

        //public static List<Category> c = new List<Category>();
        //public static List<Category> getSubProjects(List<int> subProj)
        //{
        //    for (int i = 0; i < subProj.Count; i++)
        //    {
        //        for (int j = 0; j < projects.Count; j++)
        //        {
        //            try
        //            {
        //                if (subProj[i] == projects[j].Parent.Id)
        //                {
        //                    c.Add(projects[j]);
        //                }
        //            }
        //            catch { }
        //        }
        //    }
        //}

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