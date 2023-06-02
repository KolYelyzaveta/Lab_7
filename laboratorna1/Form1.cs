using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace laboratorna1
{
    public partial class Form1 : Form
    {
        public DB _DB = new DB();
        public List<Student> Students { get; set; }
        public List<Group> Groups { get; set; }
        public string cell;

        public Form1()
        {
            Students = new List<Student>();
            Groups = new List<Group>();
            InitializeComponent();
        }

        private void FillGroups()
        {
            _DB.OpenConnection();
            string updateQuery = @"
            UPDATE Groups
            SET StudentsQuantity = (
                SELECT COUNT(*)
                FROM Students
                WHERE Students.[Group] = Groups.Id
            )";
            var command = new SqlCommand(updateQuery, _DB.GetConnection());
            command.ExecuteNonQuery();
            _DB.CloseConnection();
            FillData("");
        }

        public void FillData(string str)
        {
            _DB.OpenConnection();
            string sqlStudents = "SELECT * FROM Students " + str;
            string sqlGroups = "SELECT * FROM Groups";
            SqlCommand commandStudents = new SqlCommand(sqlStudents, _DB.GetConnection());
            SqlCommand commandGroups = new SqlCommand(sqlGroups, _DB.GetConnection());
            SqlDataAdapter adapterStudents = new SqlDataAdapter(commandStudents);
            SqlDataAdapter adapterGroups = new SqlDataAdapter(commandGroups);
            DataTable tableStudents = new DataTable();
            adapterStudents.Fill(tableStudents);
            DataTable tableGroups = new DataTable();
            adapterGroups.Fill(tableGroups);

            BindingSource binding = new BindingSource
            {
                DataSource = tableStudents
            };
            dataGridView1.DataSource = binding;

            BindingSource binding1 = new BindingSource
            {
                DataSource = tableGroups
            };
            dataGridView2.DataSource = binding1;
            _DB.CloseConnection();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _DB.OpenConnection();
            string strStudents = "SELECT * FROM Students";
            SqlCommand cmdStudents = new SqlCommand(strStudents, _DB.GetConnection());
            SqlDataReader rdrStuds = cmdStudents.ExecuteReader();
            while (rdrStuds.Read())
            {
                Student student = new Student();
                student.Name = rdrStuds.GetString(0);
                student.Surname = rdrStuds.GetString(1);
                student.Patronymic = rdrStuds.GetString(2);
                student.Faculty = rdrStuds.GetString(3);
                student.Group = rdrStuds.GetString(4);
                Students.Add(student);
            }
            rdrStuds.Close();

            string strGroups = "SELECT * FROM Groups";
            SqlCommand cmdGroups = new SqlCommand(strGroups, _DB.GetConnection());
            SqlDataReader rdrGroups = cmdGroups.ExecuteReader();
            while (rdrGroups.Read())
            {
                Group group = new Group();
                group.Id = rdrGroups.GetString(0);
                group.StudentsQuantity = rdrGroups.GetInt32(1);
                Groups.Add(group);
            }
            rdrGroups.Close();
            _DB.CloseConnection();
            FillGroups();
        }

        private void aSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FillData("ORDER BY Name ASC");
        }

        private void dESCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FillData("ORDER BY Name DESC");
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Rows[e.RowIndex].Cells[0].Value == null || dataGridView2.Rows[e.ColumnIndex].Index != 0)
            {
                return;
            }
            else
            {
                cell = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
                Form2 groupStudents = new Form2(this, Students, Groups);
                groupStudents.Show();
            }
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 editForm = new Form3(this, Students, Groups);
            editForm.Show();
        }
    }
}
