using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laboratorna1
{
    public partial class Form3 : Form
    {
        public DB _DB = new DB();
        public Form1 form_ { get; set; }
        public List<Student> Students { get; set; }
        public List<Group> Groups { get; set; }
        public Form3(Form1 form3, List<Student> students, List<Group> groups)
        {
            this.form_ = form3;
            this.Students = this.form_.Students;
            this.Groups = this.form_.Groups;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Id не может быть пустым!", "Информация", MessageBoxButtons.OK);
                return;
            }

            _DB.OpenConnection();
            var Id = textBox1.Text;
            var addQuery = $"INSERT INTO Groups (Id, StudentsQuantity) values ('{Id}', '{0}')";
            var command = new SqlCommand(addQuery, _DB.GetConnection());
            command.ExecuteNonQuery();
            Groups.Add(new Group(textBox1.Text));
            _DB.CloseConnection();
            form_.FillData("");
            MessageBox.Show("Группа добавлена!", "Attention!", MessageBoxButtons.OK);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            bool flag = false;
            foreach (var group in Groups)
            {
                if (group.Id == textBox1.Text)
                {
                    _DB.OpenConnection();
                    flag = true;
                    string deleteQuery = $"DELETE FROM Groups WHERE Id = '{group.Id}'";
                    var command = new SqlCommand(deleteQuery, _DB.GetConnection());
                    command.ExecuteNonQuery();

                    Groups.Remove(group);
                    MessageBox.Show("Группа удалена!", "Attention!", MessageBoxButtons.OK);
                    dataGridView1.Rows.Clear();
                    foreach (var student in Students)
                    {
                        if (student.Group == textBox1.Text)
                        {
                            student.Group = "-";
                        }
                    }
                    string updateQuery = $"UPDATE Students SET [Group] = '-' WHERE [Group] = '{group.Id}'";
                    var command1 = new SqlCommand(updateQuery, _DB.GetConnection());
                    command1.ExecuteNonQuery();
                    _DB.CloseConnection();
                    form_.FillData("");
                    GetStudents();
                    return;
                }
            }
            if (!flag)
            {
                MessageBox.Show("Группы с таким Id не существует!", "Attention!", MessageBoxButtons.OK);
            }
        }

        public void GetStudents()
        {
            List<Student> freeStudents = Students.Where(student => student.Group == "-").ToList();
            BindingSource binding2 = new BindingSource
            {
                DataSource = freeStudents
            };
            dataGridView1.DataSource = binding2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (var group in Groups)
            {
                if (String.IsNullOrWhiteSpace(textBox2.Text) || String.IsNullOrEmpty(textBox2.Text))
                {
                    List<Student> groupStudents1 = Students.Where(student => student.Group == textBox1.Text).ToList();
                    BindingSource binding1 = new BindingSource
                    {
                        DataSource = groupStudents1
                    };
                    dataGridView2.DataSource = binding1;
                    return;
                }

                if (group.Id == textBox1.Text && !String.IsNullOrWhiteSpace(textBox2.Text) && !String.IsNullOrEmpty(textBox2.Text))
                {
                    _DB.OpenConnection();
                    string updateQuery = $"UPDATE Groups SET Id = '{textBox2.Text}' WHERE Id = '{group.Id}'";
                    var command = new SqlCommand(updateQuery, _DB.GetConnection());
                    command.ExecuteNonQuery();

                    group.Id = textBox2.Text;
                    MessageBox.Show("Id группы изменён!", "Attention!", MessageBoxButtons.OK);
                    foreach (var student in Students)
                    {
                        if (student.Group == textBox1.Text)
                        {
                            student.Group = textBox2.Text;
                            string updateQuery1 = $"UPDATE Students SET [Group] = '{textBox2.Text}' WHERE [Group] = '{textBox1.Text}'";
                            var command1 = new SqlCommand(updateQuery1, _DB.GetConnection());
                            command1.ExecuteNonQuery();
                        }
                    }
                    _DB.CloseConnection();
                }
            }
            List<Student> groupStudents = Students.Where(student => student.Group == textBox2.Text).ToList();
            BindingSource binding = new BindingSource
            {
                DataSource = groupStudents
            };
            dataGridView2.DataSource = binding;
            form_.FillData("");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedStudent = dataGridView1.SelectedRows[0].DataBoundItem as Student;
                foreach (var group in Groups)
                {
                    if (group.StudentsQuantity == 30)
                    {
                        MessageBox.Show("В группе уже есть 30 студентов!", "Attention!", MessageBoxButtons.OK);
                        return;
                    }
                    if (group.Id == textBox1.Text)
                    {
                        _DB.OpenConnection();
                        selectedStudent.Group = textBox1.Text;
                        string updateQuery = $"UPDATE Students SET [Group] = '{textBox1.Text}' WHERE [Name] = '{selectedStudent.Name}' AND [Surname] = '{selectedStudent.Surname}' AND [Patronymic] = '{selectedStudent.Patronymic}'";
                        var command1 = new SqlCommand(updateQuery, _DB.GetConnection());
                        command1.ExecuteNonQuery();
                        group.StudentsQuantity++;
                        string updateQuery1 = $"UPDATE Groups SET StudentsQuantity = {group.StudentsQuantity} WHERE Id = '{group.Id}'";
                        var command2 = new SqlCommand(updateQuery1, _DB.GetConnection());
                        command2.ExecuteNonQuery();
                        GetStudents();
                        List<Student> groupStudents = Students.Where(student => student.Group == textBox1.Text).ToList();
                        BindingSource binding = new BindingSource
                        {
                            DataSource = groupStudents
                        };
                        dataGridView2.DataSource = binding;
                        _DB.CloseConnection();
                        form_.FillData("");
                    }
                }
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedStudent = dataGridView2.SelectedRows[0].DataBoundItem as Student;
                foreach (var group in Groups)
                {
                    if (group.Id == textBox1.Text)
                    {
                        _DB.OpenConnection();
                        selectedStudent.Group = "-";
                        string updateQuery = $"UPDATE Students SET [Group] = '-' WHERE [Name] = '{selectedStudent.Name}' AND [Surname] = '{selectedStudent.Surname}' AND [Patronymic] = '{selectedStudent.Patronymic}'";
                        var command = new SqlCommand(updateQuery, _DB.GetConnection());
                        command.ExecuteNonQuery();
                        group.StudentsQuantity--;
                        string updateQuery1 = $"UPDATE Groups SET StudentsQuantity = '{group.StudentsQuantity}' WHERE Id = '{group.Id}'";
                        var command1 = new SqlCommand(updateQuery1, _DB.GetConnection());
                        command1.ExecuteNonQuery();
                        GetStudents();
                        List<Student> groupStudents = Students.Where(student => student.Group == textBox1.Text).ToList();
                        BindingSource binding = new BindingSource
                        {
                            DataSource = groupStudents
                        };
                        dataGridView2.DataSource = binding;
                        _DB.CloseConnection();
                        form_.FillData("");
                    }
                }
            }
            catch { }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            GetStudents();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrWhiteSpace(textBox3.Text) || String.IsNullOrEmpty(textBox4.Text) || String.IsNullOrWhiteSpace(textBox4.Text)
                || String.IsNullOrEmpty(textBox5.Text) || String.IsNullOrWhiteSpace(textBox5.Text) || String.IsNullOrEmpty(textBox6.Text) || String.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Студент не может быть пустым!", "Attention!", MessageBoxButtons.OK);
                return;
            }

            _DB.OpenConnection();
            Student student = new Student(textBox4.Text, textBox3.Text, textBox5.Text, textBox6.Text, "-");
            var addQuery = $"INSERT INTO Students (Surname, Name, Patronymic, Faculty, [Group]) values ('{student.Surname}', '{student.Name}', '{student.Patronymic}', '{student.Faculty}', '{student.Group}')";
            var command = new SqlCommand(addQuery, _DB.GetConnection());
            command.ExecuteNonQuery();
            Students.Add(student);
            _DB.CloseConnection();
            GetStudents();
            form_.FillData("");
            MessageBox.Show("Студент успешно добавлен!", "Attention!", MessageBoxButtons.OK);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedStudent = dataGridView1.SelectedRows[0].DataBoundItem as Student;
                _DB.OpenConnection();
                string deleteQuery = $"DELETE FROM Students WHERE [Name] = '{selectedStudent.Name}' AND [Surname] = '{selectedStudent.Surname}' AND [Patronymic] = '{selectedStudent.Patronymic}' AND [Faculty] = '{selectedStudent.Faculty}'";
                var command = new SqlCommand(deleteQuery, _DB.GetConnection());
                command.ExecuteNonQuery();
                foreach (var group in Groups)
                {
                    if (selectedStudent.Group == group.Id)
                    {
                        group.StudentsQuantity--;
                        string updateQuery = $"UPDATE Groups SET StudentsQuantity = '{group.StudentsQuantity}' WHERE Id = '{group.Id}'";
                        var command1 = new SqlCommand(updateQuery, _DB.GetConnection());
                        command1.ExecuteNonQuery();
                    }
                }
                Students.Remove(selectedStudent);
                _DB.CloseConnection();
                GetStudents();
                form_.FillData("");
                MessageBox.Show("Студент удалён!", "Attention!", MessageBoxButtons.OK);
            }
            catch { }
        }
    }
}
