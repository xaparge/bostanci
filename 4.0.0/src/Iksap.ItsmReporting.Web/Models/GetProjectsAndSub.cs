using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Iksap.ItsmReporting.Web.Models
{
    public class GetProjectsAndSub
    {
        public List<project> allProjects;
        MySqlConnection dbConn = new MySqlConnection("server=" + System.Configuration.ConfigurationManager.AppSettings["DbPath"].ToString() + "; uid=root;pwd=" + System.Configuration.ConfigurationManager.AppSettings["DbPassword"].ToString() + "; database=itsmreporting_operations");
        public string projectTree = "";

        public string getProjects(int currentUserId)
        {
            projectTree = "";
            try
            {
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
            }
            catch { }
            return projectTree;
        }

        bool subChanged = false;
        private List<project> getSubProject(int subProjectNumber)
        {
            List<project> subProjects = new List<project>();
            try
            {
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
            }
            catch { }
            return subProjects;
        }

        List<int> selectedProjectsWithSub = new List<int>();
        public string getProjects(string projectsName)
        {
            selectedProjectsWithSub.Clear();
            string last_state = "";
            try
            {
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

                string[] projectsList = projectsName.Split(',');
                for (int i = 0; i < projectsList.Length; i++)
                {
                    projectsList[i] = projectsList[i].Trim();
                }
                List<project> selectedProjects = new List<project>();
                for (int i = 0; i < projectsList.Count(); i++)
                {
                    for (int j = 0; j < allProjects.Count; j++)
                    {
                        if (projectsList[i] == allProjects[j].name)
                        {
                            selectedProjects.Add(allProjects[j]);
                            break;
                        }
                    }
                }

                for (int i = 0; i < selectedProjects.Count; i++)
                {
                    selectedProjectsWithSub.Add(selectedProjects[i].id);
                    getSubProject_StringMode(selectedProjects[i].id);
                }

                dbConn.Close();
                bool first_control = false;
                for (int i = 0; i < selectedProjectsWithSub.Count; i++)
                {
                    if (!first_control)
                    {
                        last_state += selectedProjectsWithSub[i];
                        first_control = true;
                    }
                    else
                        last_state += "," + selectedProjectsWithSub[i];
                }

                if (last_state == "")
                    last_state = "0";
            }
            catch { }

            return last_state;
        }

        private void getSubProject_StringMode(int subProjectNumber)
        {
            for (int i = 0; i < allProjects.Count; i++)
            {
                if (allProjects[i].parent_id == subProjectNumber)
                {
                    selectedProjectsWithSub.Add(allProjects[i].id);
                    getSubProject(allProjects[i].id);
                }
            }
        }
    }
}