using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace datatable_comparison
{
    public partial class FinalTable : Form
    {
        public DataTable finalTable { get; set; }

        public FinalTable()
        {
            InitializeComponent();
            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;
            AcceptButton = button1;
            CancelButton = button2;
        }

        public void FillDGV(DataTable table)
        {
            dataGridView1.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.finalTable = (DataTable)dataGridView1.DataSource;
            this.Close();
        }
    }
}