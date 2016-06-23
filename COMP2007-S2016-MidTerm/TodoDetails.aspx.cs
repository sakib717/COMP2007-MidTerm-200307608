using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/*
 * COMP2007-MidTerm-200307608
 * By Sakib Patel
 * To Do List 
 * 
 */

// using statements required for EF DB access
using COMP2007_S2016_MidTerm.Models;
using System.Web.ModelBinding;

namespace COMP2007_S2016_MidTerm
{
    public partial class TodoDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetTodoList();
            }
        }

        protected void GetTodoList()
        {
            // populate teh form with existing data from the database
            int TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

            // connect to the EF DB
            using (TodoConnection db = new TodoConnection())
            {
                // populate a object instance with the TodoID from the URL Parameter
                Todo updatedTodoList = (from TodoList in db.Todos
                                            where TodoList.TodoID == TodoID
                                            select TodoList).FirstOrDefault();

                // map the properties to the form controls
                if (updatedTodoList != null)
                {
                    TodoNameTextBox.Text = updatedTodoList.TodoName;
                    TodoNotesTextBox.Text = updatedTodoList.TodoNotes;

                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            // Redirect back to page
            Response.Redirect("~/TodoList.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Use EF to connect to the server
            using (TodoConnection db = new TodoConnection())
            {
                // use the  model to create a new  object and
                // save a new record
                Todo newTodoList = new Todo();

                int TodoID = 0;

                if (Request.QueryString.Count > 0) // our URL has a TodoID in it
                {
                    // get the id from the URL
                    TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

                    // get the current  EF DB
             
                     newTodoList = (from todo in db.Todos
                                   where todo.TodoID == TodoID
                                   select todo).FirstOrDefault();
                }

                // add form data to the new record
                newTodoList.TodoName = TodoNameTextBox.Text;
                newTodoList.TodoNotes = TodoNotesTextBox.Text;


                // use LINQ to ADO.NET to add / insert new record into the database

                if (TodoID == 0)
                {
                    db.Todos.Add(newTodoList);
                }


                // save our changes - also updates and inserts
                db.SaveChanges();

                // Redirect back to the updated  page
                Response.Redirect("~/TodoList.aspx");
            }
        }

        protected void CheckBox1_CheckChanged(object sender, System.EventArgs e)
        {

            if (CheckBox.Checked == true)
            {
                //db.SaveChanges();
            }
            else
            {
              
            }
        }
    }
}