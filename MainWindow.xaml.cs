using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace SQL2XLS
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString, commandString = "", Path;
        private bool leftClicked = false, leftDoubleClicked = false, hasFile;
        private DateTime leftClickedTimestamp = DateTime.MinValue;

        /// <summary>
        /// Stellt die ausgegebene Tabelle dar
        /// </summary>
        private List<TableRow> Table;



        public MainWindow()
        {
            InitializeComponent();

            try
            {
                connectionString = File.ReadAllText(Environment.CurrentDirectory + "\\confDB.txt");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message +
                                "\n\nBitte sorgen Sie dafür, dass im ausführenden Verzeichnis die Informationen zum Verbinden zur Datenbank angegeben sind",
                                "Config-Dateien konnten nicht geladen werden!",
                                MessageBoxButton.OK);
                //Close();
            }
        }

        private void InputPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*
             * Wenn es schon vor kurzem geklickt wurde und nochmal geklickt wird, dann wird
             *      leftDoubleClicked = true;
             *      
             * Ansonsonsten wird es false aber
             *      leftClicked = true;
             */
            bool _r = leftClicked && (DateTime.UtcNow - leftClickedTimestamp).TotalMilliseconds < 500;
            leftDoubleClicked = _r ? _r : leftDoubleClicked;
            leftClicked = !_r;
            leftClickedTimestamp = DateTime.UtcNow;
        }

        private void InputPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (leftClicked && (DateTime.UtcNow - leftClickedTimestamp).TotalMilliseconds < 750)
            {
                Console.WriteLine("YOOOOOO");
            }
            else if (leftDoubleClicked && (DateTime.UtcNow - leftClickedTimestamp).TotalMilliseconds < 500)
            {
                Console.WriteLine("Damn, Doubletrouble");
                leftClicked = false;
                leftDoubleClicked = false;

                if(hasFile)
                {
                    InputPanel.Content = Path.Split('\\')[Path.Split('\\').Length - 1] + " wird ausgeführt...";
                    InputPanel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    InputPanel.VerticalContentAlignment = VerticalAlignment.Center;

                    using (SqlCommand _sqlcmd = new SqlCommand(commandString))
                        Table = ExecuteSQL(_sqlcmd);

                    OutputBox.Document = null;
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add(TableToString(Table));
                    FlowDocument flowDocument = new FlowDocument();
                    flowDocument.Blocks.Add(paragraph);
                    OutputBox.Document = flowDocument;
                }
            }
            else
            {
                leftClicked = false;
                leftDoubleClicked = false;
            }
        }

        private void InputPanel_Drop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            Path = filePaths[0];

            if(filePaths.Length > 1)
                _ = MessageBox.Show("Bitte nur eine Datei zugleich ablegen");
            else
            {
                if(Path.Split('.')[Path.Split('.').Length - 1] == "sql")
                {
                    try
                    {
                        commandString = string.Join("\n", File.ReadAllLines(Path));
                        hasFile = true;
                        InputPanel.Content = Path.Split('\\')[Path.Split('\\').Length - 1] + " wurde abgelegt!\nDoppelklick zum ausführen!";
                        InputPanel.HorizontalContentAlignment = HorizontalAlignment.Center;
                        InputPanel.VerticalContentAlignment = VerticalAlignment.Center;
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show("Es ist ein Fehler aufgetreten:\n\n" + ex.Message);
                    }
                }
                else
                    _ = MessageBox.Show("Die abgelegte Datei ist keine .sql-Datei.");
            }
        }

        /// <summary>
        /// Führt den angegebenen SQLCommand aus und gibt das Ergebnis als <see cref="List{T}"/> mit allen Zeilen als <see cref="TableRow"/> aus
        /// </summary>
        /// <param name="cmd"><see cref="SqlCommand"/> der aus der Datei gelesen wurde</param>
        /// <returns><see cref="List{T}"/> aus <see cref="TableRow"/>-Objekten</returns>
        private List<TableRow> ExecuteSQL(SqlCommand cmd)
        {
            return null;
        }

        private void WriteXLS()
        {

        }

        private string TableToString(List<TableRow> table)
        {
            string OUT = "";
            int _cols = table[0].Columns.Count;
            int _rows = table.Count;
            int[] lengths = new int[_cols];

            /*
             * Schaut, wie lang die Spalten werden können und merkt sich den Wert in lengths[]
             */
            foreach(TableRow row in Table)
            {
                int i = 0;
                foreach(KeyValuePair<string, object> kv in row.Columns)
                {
                    if(lengths[i] < kv.Value.ToString().Length || lengths[i] < kv.Key.Length)
                        lengths[i] = kv.Value.ToString().Length > kv.Key.Length ? kv.Value.ToString().Length : kv.Key.Length;
                    i++;
                }
            }


            /*
             * Schreibt die Spaltennamen auf
             */
            for (int i = 0; i < _cols; i++)
                OUT += table[0].Columns[i].Key + EmptyFiller(lengths[i] - table[0].Columns[i].Key.Length) + " |";
            OUT += "\n";

            /*
             * Schreibt die einzelnen Zeilen mit den Spalten in gleichem Abstand geteilt auf
             */
            for (int i = 0; i < _rows; i++)
            {
                int j = 0;
                foreach(KeyValuePair<string, object> kv in table[i].Columns)
                {
                    OUT += kv.Value + EmptyFiller(lengths[j] - kv.Value.ToString().Length) + " |";
                    j++;
                }
                OUT += "\n";
            }


            return OUT;
        }

        private string EmptyFiller(int SpaceAmount)
        {
            string OUT = "";
            for (int i = SpaceAmount; i > 0; i--)
                OUT += " ";
            return OUT;
        }
    }

    /// <summary>
    /// Stellt eine Reihe in einer Tabelle dar
    /// </summary>
    public class TableRow
    {
        /// <summary>
        /// Beinhaltet die Werte in der Tabellenreiehe, eine Collection and KeyValuePairs
        /// <list type="Backspace"/>
        /// string - Name der Spalte
        /// <list type="Backspace"/>
        /// object - Wert, der in der entsprechenden Spalte steht, kann jeden Typ annehmen
        /// </summary>
        public ObservableCollection<KeyValuePair<string, object>> Columns { get; set; }

        public TableRow() { }

        /// <summary>
        /// Gibt die Variable in der namentlich angegebenen Spalte aus
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns><see cref="object"/> in der Zelle, falls die Spalte nicht existiet </returns>
        public object GetColumnValue(string ColumnName)
        {
            foreach (KeyValuePair<string, object> kv in Columns)
                if (kv.Key == ColumnName)
                    return kv.Value;
            return null;
        }

        /// <summary>
        /// Gibt alle Werte in der Zeile allein zurück als Array, sortiert nach Abfrage
        /// </summary>
        /// <returns><see cref="object"/><c>[]</c> aller Werte in der Zeile</returns>
        public object[] GetColumnValues()
        {
            object[] OUT = new object[0];
            for (int i = 0; i < Columns.Count; i++)
                OUT[i] = Columns[i].Value;
            return OUT;
        }

        public override string ToString()
            => "( " + string.Join(" | ", Columns) + " )";
    }
}
