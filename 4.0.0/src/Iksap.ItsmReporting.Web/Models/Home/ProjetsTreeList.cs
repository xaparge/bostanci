using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Iksap.ItsmReporting.Web.Models.Home
{
    public class ProjetsTreeList 
    {
        TreeNode rootNode = new TreeNode();
        TreeNode childNode = new TreeNode();
        List<string> TreeView = new List<string>();
    
        //<optgroup label="All Manhattan">
        //                        <option value='3'>Flatiron</option>
        //                        <option value='4'>Upper West Side</option>
        //                        <option value='10'>Manhattanville</option>
        //                    </optgroup>
        public List<string> PopulateTreeView(string currentUserId)
        {
            IList<Category> topLevelCategories = TreeHelper.ConvertToForest(Category.GetListFromDatabase(currentUserId));
          
            foreach (Category topLevelCategory in topLevelCategories)
            {
                //Adding Root Items
                rootNode.Text = topLevelCategory.Name;
                rootNode.Value = topLevelCategory.Id.ToString();
                TreeView.Add("<optgroup label=\"" + rootNode.Text + "\">");

                RenderCategory(topLevelCategory);
                TreeView.Add("</optgroup>");
            }
            
            return TreeView;
        }

        void RenderCategory(Category category)
        {
            if (category.Children.Count > 0)
            {
                foreach (Category child in category.Children)
                {
                    childNode.Text = category.Name;
                    childNode.Value = category.Id.ToString();
                    TreeView.Add( "<option value='"+ child.Id.ToString() + "'>"+ child.Name + "</option>");
                    //rootNode.ChildNodes.Add(childNode);
                    RenderCategory(child);
                }
            }
        }

    }
}