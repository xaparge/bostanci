using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Iksap.ItsmReporting.Web.Models
{
    public class GetProjectsAndSub
    {
        public List<project> allProjects;

        public string projectTree = "";
        public string getProjects(int currentUserId)
        {
            MySqlConnection dbConn = new MySqlConnection("server=127.0.0.1; uid=root; pwd=" + System.Configuration.ConfigurationManager.AppSettings["DbPassword"].ToString() + "; database=itsmreporting_operations");
            dbConn.Open();
            MySqlCommand cmd = new MySqlCommand("allProjects", dbConn);
            cmd.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);

            allProjects = new List<project>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                project p = new project();
                p.id = (int)dt.Rows[i][0];
                p.name = (string)dt.Rows[i][1];
                if (dt.Rows[i][2] != DBNull.Value)
                    p.parent_id = (int)dt.Rows[i][2];

                allProjects.Add(p);
            }

            List<project> projects = new List<project>();
            MySqlCommand cmd2 = new MySqlCommand("slaProjectsByUsers", dbConn);
            cmd2.Parameters.AddWithValue("@user_id", currentUserId);
            cmd2.CommandType = CommandType.StoredProcedure;
            DataTable dt2 = new DataTable();
            MySqlDataAdapter da2 = new MySqlDataAdapter(cmd2);
            da2.Fill(dt2);

            projectTree = "{value: null, options: [";
            bool changed = false;
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                project p = new project();
                p.id = Convert.ToInt32(dt2.Rows[i][0]);
                p.name = dt2.Rows[i][1].ToString();
                if (dt2.Rows[i][2] != DBNull.Value)
                    p.parent_id = Convert.ToInt32(dt2.Rows[i][2]);

                projects.Add(p);
                if (changed)
                    projectTree += ", ";
                projectTree += "{ id: " + p.id + ", ";
                projectTree += "label: '" + p.name + "'";
                getSubProject(p.id);
                projectTree += " }";
                changed = true;
            }
            projectTree += "]}";
            dbConn.Close();
            return projectTree;
        }

        bool subChanged = false;
        private List<project> getSubProject(int subProjectNumber)
        {
            List<project> subProjects = new List<project>();
            bool virguleControl = false;
            for (int i = 0; i < allProjects.Count; i++)
            {
                if (allProjects[i].parent_id == subProjectNumber)
                {
                    if (!subChanged)
                    {
                        projectTree += ", children: [";
                        subChanged = true;
                    }
                    if (virguleControl)
                    {
                        projectTree += ", ";
                        virguleControl = true;
                    }
                    projectTree += "{ id: " + allProjects[i].id + ", label: '" + allProjects[i].name + "'";
                    virguleControl = true;
                    bool LastState = subChanged;
                    subChanged = false;
                    getSubProject(allProjects[i].id);
                    subChanged = LastState;
                    projectTree += " }";
                }
            }
            if (subChanged)
            {
                projectTree += "]";
                subChanged = false;
            }
            return subProjects;
        }
    }
}