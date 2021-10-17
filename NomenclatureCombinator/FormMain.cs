using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NomenclatureCombinator
{
    public partial class FormMain : Form
    {

        List<Nomenclature> noms = new List<Nomenclature>();

        Cursor _cursor = Cursor.Current;

        public FormMain()
        {
            InitializeComponent();

            noms.Add(new Nomenclature() { Article = "Product 01", Quantity = 10, Price = 5 });
            noms.Add(new Nomenclature() { Article = "Product 02", Quantity = 2, Price = 50 });
            noms.Add(new Nomenclature() { Article = "Product 03", Quantity = 1, Price = 100 });
            noms.Add(new Nomenclature() { Article = "Product 04", Quantity = 1, Price = 11.11m });
            noms.Add(new Nomenclature() { Article = "Product 05", Quantity = 1, Price = 55.55m });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGrid_Nomenclatures.DataSource = noms;
            DataGrid_Nomenclatures.ReadOnly = true;
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            var targetPriceSum = numericUpDown1.Value;
            int progress_count = 0;
            _cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                LogInfo($"[{DateTime.Now:yyyy-MM-dd HH:mm}]", true);

                var diffNoms = new List<Nomenclature>();

                Task.Run(() => DoProgress(0));

                progressBar1.Value = 0;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 5;
                progressBar1.Step = 1;

                Task.Run(() => DoProgress(++progress_count));

                Stopwatch watch = new Stopwatch();

                watch.Start();

                foreach (var n in noms)
                {
                    var en = n.DiffPrices();

                    while (en.MoveNext())
                    {
                        diffNoms.Add(en.Current);
                    }
                }

                watch.Stop();
                LogInfo($"Step 1: {watch.Elapsed}");
                Task.Run(() => DoProgress(++progress_count));

                watch.Restart();

                var tmp_res = Combinatoric.GetAllCombos(diffNoms);

                watch.Stop();
                LogInfo($"Step 2: {watch.Elapsed}");
                Task.Run(() => DoProgress(++progress_count));
                watch.Restart();

                var res = tmp_res
                    .Where(x => !x.HasDuplicateArticul())
                    .Where(x => x.NumberOfDifferentArticuls() == noms.Count)
                    .Distinct()
                    .ToList();

                watch.Stop();
                LogInfo($"Step 3: {watch.Elapsed}");
                Task.Run(() => DoProgress(++progress_count));

                List<Nomenclature> resultSet = null;

                List<NomenclatureContainer> results = new List<NomenclatureContainer>();

                int res_count = 0;

                watch.Restart();

                foreach (var tmp in res)
                {
                    var tmpsum = tmp.Sum(x => x.TotalPrice);
                    if (tmpsum == targetPriceSum)
                    {
                        results.Add(new NomenclatureContainer()
                        {
                            Id = ++res_count,
                            Nomenclatures = tmp
                        });

                        if (resultSet == null)
                            resultSet = tmp;

                        if (res_count >= 20)
                            break;
                    }
                }

                var msg = resultSet == null ? "Not Found" : $"Found combinations: {results.Count}";
                Console.WriteLine(msg);
                LogInfo(msg);

                watch.Stop();
                LogInfo($"Step 4: {watch.Elapsed}");
                Task.Run(() => DoProgress(++progress_count));

                if (resultSet != null)
                {
                    LogInfo(string.Empty);

                    foreach (var container in results)
                    {
                        LogInfo($"№ {container.Id}");

                        foreach (var rs in container.Nomenclatures)
                        {
                            Txt_Output.AppendText(rs.ToString());
                        }

                        LogInfo(string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo(ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Task.Run(() => DoProgress(++progress_count));
                progressBar1.Enabled = false;
                Cursor.Current = _cursor;
            }
        }

        void LogInfo(string message, bool clear = false)
        {
            if (InvokeRequired)
            {
                Invoke((Action<string, bool>)LogInfo, message, clear);
                return;
            }

            if (clear) Txt_Output.Clear();
            Txt_Output.AppendText(message);
            Txt_Output.AppendText(Environment.NewLine);
            Txt_Output.ScrollToCaret();
            Application.DoEvents();
        }

        void DoProgress(int step)
        {
            if (InvokeRequired)
            {
                Invoke((Action<int>)DoProgress, step);
                return;
            }

            if (step > progressBar1.Maximum)
                step = progressBar1.Maximum;
            progressBar1.Value = step;
            Application.DoEvents();
        }
    }
}
