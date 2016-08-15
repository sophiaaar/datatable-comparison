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
    public partial class Form1 : Form
    {
        public DataTable finalTable;
        public bool finished = false;

        public Form1()
        {
            InitializeComponent();
        }

        public void FillTables(DataTable toBeUploaded, DataTable inLocDirect)
        {
            dataGridView1.DataSource = toBeUploaded;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView2.DataSource = inLocDirect;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToOrderColumns = false;
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "checkColumn1")
            {
                DataGridViewRow row1 = dataGridView1.Rows[e.RowIndex];
                DataGridViewRow row2 = dataGridView2.Rows[e.RowIndex];

                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row1.Cells[e.ColumnIndex];
                DataGridViewCheckBoxCell chk2 = (DataGridViewCheckBoxCell)row2.Cells[e.ColumnIndex];

                if (chk.Value == chk.TrueValue && chk2.Value == chk2.TrueValue)
                {
                    chk2.Value = chk2.FalseValue;
                }
                if (chk.Value == chk.FalseValue)
                {
                    chk2.Value = chk2.TrueValue;
                }
            }
        }

        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
            {
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "checkColumn2")
            {
                DataGridViewRow row1 = dataGridView1.Rows[e.RowIndex];
                DataGridViewRow row2 = dataGridView2.Rows[e.RowIndex];

                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row1.Cells[e.ColumnIndex];
                DataGridViewCheckBoxCell chk2 = (DataGridViewCheckBoxCell)row2.Cells[e.ColumnIndex];

                if (chk2.Value == chk2.TrueValue && chk.Value == chk.TrueValue)
                {
                    chk.Value = chk.FalseValue;
                }
                if (chk2.Value == chk2.FalseValue)
                {
                    chk.Value = chk.TrueValue;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> rows_with_checked_column1 = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells["checkColumn1"].Value) == true)
                {
                    rows_with_checked_column1.Add(row);
                }
            }

            List<DataGridViewRow> rows_with_checked_column2 = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (Convert.ToBoolean(row.Cells["checkColumn2"].Value) == true)
                {
                    rows_with_checked_column2.Add(row);
                }
            }

            List<DataGridViewRow> newTable = new List<DataGridViewRow>();

            newTable.AddRange(rows_with_checked_column1);
            newTable.AddRange(rows_with_checked_column2);

            List<DataGridViewRow> sortedNewTable = newTable.OrderBy(i => i.Index).ToList();

            DataTable mergedTable = MergeTables(sortedNewTable);

            FinalTable ft = new FinalTable();
            ft.FillDGV(mergedTable);
            if (ft.ShowDialog() == DialogResult.OK)
            {
                finalTable = ft.finalTable;
                dataGridView1.DataSource = null;
                dataGridView2.DataSource = null;
                this.Close();
            }
        }

        public DataTable MergeTables(List<DataGridViewRow> sortedRows)
        {
            DataTable mergedTable = new DataTable();

            int cols = 1;
            foreach (DataGridViewRow row1 in sortedRows)
            {
                cols = row1.Cells.Count;
                break;
            }

            for (int i = 1; i < cols; i++)
            {
                mergedTable.Columns.Add();
            }

            foreach (DataGridViewRow row in sortedRows)
            {
                if (row.DataBoundItem != null)
                {
                    DataRow newRow = ((DataRowView)row.DataBoundItem).Row;
                    mergedTable.Rows.Add(newRow.ItemArray);
                }
            }
            finished = true;
            return mergedTable;
        }

        private void ComparisonForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (finished == false)
            {
                var window = MessageBox.Show("Close the window?", "Closing", MessageBoxButtons.YesNo);
                if (window == DialogResult.No) e.Cancel = true;
                else e.Cancel = false;
            }
        }

        private void ComparisonForm_Load(object sender, EventArgs e)
        {
            DataTable table = (DataTable)dataGridView1.DataSource;
            DataTable table2 = (DataTable)dataGridView2.DataSource;

            int highest;
            if (table.Rows.Count > table2.Rows.Count)
            {
                highest = table.Rows.Count;
            }
            else
            {
                highest = table2.Rows.Count;
            }

            if (table.Rows.Count != table2.Rows.Count)
            {
                if (table.Rows.Count > table2.Rows.Count)
                {
                    int difference = table.Rows.Count - table2.Rows.Count;

                    for (int i = 0; i < difference; i++)
                    {
                        DataRow newRow = table2.NewRow();
                        newRow["identifierName"] = table.Rows[table2.Rows.Count + i]["identifierName"];
                        table2.Rows.Add(newRow);
                        //table2.ImportRow(newRow);
                    }
                }
                else
                {
                    int difference = table2.Rows.Count - table.Rows.Count;

                    for (int i = 0; i < difference; i++)
                    {

                        try
                        {
                            DataRow newRow = table.NewRow();
                            newRow["identifierName"] = table2.Rows[table.Rows.Count]["identifierName"];
                            table.Rows.Add(newRow);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        //table.Rows.Add();
                        //table.ImportRow(newRow);

                    }
                }
            }

            dataGridView1.DataSource = table;
            dataGridView2.DataSource = table2;

            if (dataGridView1.Rows.Count == dataGridView2.Rows.Count)
            {
                this.dataGridView1.Scroll += new ScrollEventHandler(dataGridView1_Scroll);
                this.dataGridView2.Scroll += new ScrollEventHandler(dataGridView2_Scroll);
            }

            if (AreTablesTheSame(table, table2) == false)
            {
                for (int i = 0; i < highest; i++)
                {
                    var row = table.Rows[i];
                    var locRow = table2.Rows[i];

                    if (CompareRows(row, locRow) == false)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }

            foreach (DataGridViewRow row2 in dataGridView2.Rows)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                    DataGridViewCheckBoxCell chk2 = (DataGridViewCheckBoxCell)row2.Cells[0];
                    if (chk2.Value == chk2.FalseValue || chk2.Value == null)
                    {
                        chk2.Value = chk2.TrueValue;
                    }
                }
            }


        }

        private void dgv1SelectAllbtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row2 in dataGridView2.Rows)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                    DataGridViewCheckBoxCell chk2 = (DataGridViewCheckBoxCell)row2.Cells[0];
                    if (chk.Value == chk.FalseValue || chk.Value == null)
                    {
                        chk.Value = chk.TrueValue;
                    }
                }
            }

        }

        private void dgv2SelectAllbtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row2 in dataGridView2.Rows)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                    DataGridViewCheckBoxCell chk2 = (DataGridViewCheckBoxCell)row2.Cells[0];
                    if (chk2.Value == chk2.FalseValue || chk2.Value == null)
                    {
                        chk2.Value = chk2.TrueValue;
                    }
                }
            }
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            this.dataGridView2.FirstDisplayedScrollingRowIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex;
        }

        private void dataGridView2_Scroll(object sender, ScrollEventArgs e)
        {
            this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView2.FirstDisplayedScrollingRowIndex;
        }

        public static bool AreTablesTheSame(DataTable tbl1, DataTable tbl2)
        {
            //http://stackoverflow.com/questions/7517968/how-to-compare-2-datatables
            if (tbl1.Rows.Count != tbl2.Rows.Count || tbl1.Columns.Count != tbl2.Columns.Count)
                return false;


            for (int i = 0; i < tbl1.Rows.Count; i++)
            {
                for (int c = 0; c < tbl1.Columns.Count; c++)
                {
                    DataRow row1 = tbl1.Rows[i];
                    DataRow row2 = tbl2.Rows[i];

                    if (CompareRows(row1, row2) == false)
                    {
                        return false;
                    }

                    //if (!Equals(tbl1.Rows[i][c], tbl2.Rows[i][c]))
                    //    return false;
                }
            }
            return true;
        }

        public static bool CompareRows(DataRow row1, DataRow row2)
        {
            object[] array1 = row1.ItemArray;
            string[] ar1 = array1.ToArray().Select(a => a.ToString().Trim()).ToArray();

            object[] array2 = row2.ItemArray;
            string[] ar2 = array2.ToArray().Select(b => b.ToString().Trim()).ToArray();

            if (ar1.SequenceEqual(ar2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
