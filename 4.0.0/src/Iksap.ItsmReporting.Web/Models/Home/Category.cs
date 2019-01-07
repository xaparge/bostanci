using Iksap.ItsmReporting.Web.Models.DataModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;


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

        public static List<Category> GetListFromDatabase()
        {
            MySqlConnection dbConn = new MySqlConnection("server=127.0.0.1; uid=root;pwd=" + System.Configuration.ConfigurationManager.AppSettings["DbPassword"].ToString() + "; database=bitnami_redmine");
            dbConn.Open();
            DbCommand cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT id, name, parent_id FROM projects";
            cmd.CommandType = System.Data.CommandType.Text;

            DbDataReader reader = cmd.ExecuteReader();
            List<Category> categories = new List<Category>();

            foreach (DbDataRecord row in reader)
            {

                Category c = new Category();
                c.Id = (int)row["id"];
                c.Name = (string)row["name"];
                if (row["parent_id"] != DBNull.Value)
                {
                    c.Parent = new Category();
                    c.Parent.Id = (int)row["parent_id"];
                }
                categories.Add(c);
            }
            reader.Close();
            dbConn.Close();
            return categories;
        }
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