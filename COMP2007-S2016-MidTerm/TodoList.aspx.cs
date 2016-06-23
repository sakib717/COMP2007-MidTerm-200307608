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

// using statements that are required to connect to EF DB
using COMP2007_S2016_MidTerm.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;

namespace COMP2007_S2016_MidTerm
{
    public partial class TodoList : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // if loading the page for the first time, populate the grid
            if (!IsPostBack)
            {
                Session["SortColumn"] = "TodoID"; // default sort column
                Session["SortDirection"] = "ASC";
                // Get the data
                this.GetTodoList();
            }
        }

        protected void GetTodoList()
        {
            // connect to EF
            using (TodoConnection db = new TodoConnection())
            {
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();

                // query the Students Table using EF and LINQ
                var TodoList = (from allTodoList in db.Todos
                                select allTodoList);

                // bind the result to the GridView
                StudentsGridView.DataSource = TodoList.AsQueryable().OrderBy(SortString).ToList();
                StudentsGridView.DataBind();
            }
        }

        protected void StudentsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // store which row was clicked
            int selectedRow = e.RowIndex;

            // get the selected TodoID using the Grid's DataKey collection
            int TodoID = Convert.ToInt32(StudentsGridView.DataKeys[selectedRow].Values["TodoID"]);

            // use EF to find the selected notes in the DB and remove it
            using (TodoConnection db = new TodoConnection())
            {
                // create object of the TodoList class and store the query string inside of it
                Todo deletedTodoList = (from TodoListRecords in db.Todos
                                          where TodoListRecords.TodoID == TodoID
                                          select TodoListRecords).FirstOrDefault();

                // remove the selected student from the db
                db.Todos.Remove(deletedTodoList);

                // save my changes back to the database
                db.SaveChanges();

                // refresh the grid
                this.GetTodoList();
            }


        }

        protected void StudentsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Set the new page number
            StudentsGridView.PageIndex = e.NewPageIndex;

            // refresh the grid
            this.GetTodoList();
        }

        protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set the new Page size
            StudentsGridView.PageSize = Convert.ToInt32(PageSizeDropDownList.SelectedValue);

            // refresh the grid
            this.GetTodoList();
        }

        protected void StudentsGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            // get the column to sorty by
            Session["SortColumn"] = e.SortExpression;

            // Refresh the Grid
            this.GetTodoList();

            // toggle the direction
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }

        protected void StudentsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if (e.Row.RowType == DataControlRowType.Header) // if header row has been clicked
                {
                    LinkButton linkbutton = new LinkButton();

                    for (int index = 0; index < StudentsGridView.Columns.Count - 1; index++)
                    {
                        if (StudentsGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if (Session["SortDirection"].ToString() == "ASC")
                            {
                                linkbutton.Text = " <i class='fa fa-caret-up fa-lg'></i>";
                            }
                            else
                            {
                                linkbutton.Text = " <i class='fa fa-caret-down fa-lg'></i>";
                            }

                            e.Row.Cells[index].Controls.Add(linkbutton);
                        }
                    }
                }
            }
        }
    }
}