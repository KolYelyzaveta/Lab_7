using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laboratorna1
{
    public partial class Form2 : Form
    {
        public Form1 form1 { get; set; }
        public List<Student> Students { get; set; }
        public List<Group> Groups { get; set; }

        public Form2(Form1 form1, List<Student> Students, List<Group> Groups)
        {
            this.form1 = form1;
            this.Students = this.form1.Students;
            this.Groups = this.form1.Groups;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = "Список студентов в группе " + form1.cell + ":";
            this.Text = form1.cell;
            ShowGroup();
        }

        private void ShowGroup()
        {
            List<Student> groupStudents = new List<Student>();
            foreach (var student1 in Students)
            {
                if (form1.cell != student1.Group)
                {
                    continue;
                }
                if (form1.cell == student1.Group)
                {
                    groupStudents.Add(student1);
                }
            }
            groupStudents = groupStudents.OrderBy(student => student.Name).ToList();
            BindingSource binding2 = new BindingSource
            {
                DataSource = groupStudents
            };
            dataGridView1.DataSource = binding2;
        }
    }
}
