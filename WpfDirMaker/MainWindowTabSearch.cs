using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfDirMaker
{
    public partial class MainWindow : Window
    {

        // Tab SearchAll

        #region Button events

        private void buttonSearchDir1_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir1.Text = selectSearchDirectory(TextboxSearchDir1.Text);
            mIO.SearchDir1 = TextboxSearchDir1.Text;
        }

        private void buttonSearchDir2_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir2.Text = selectSearchDirectory(TextboxSearchDir2.Text);
            mIO.SearchDir2 = TextboxSearchDir2.Text;
        }

        private void buttonSearchDir3_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir3.Text = selectSearchDirectory(TextboxSearchDir3.Text);
            mIO.SearchDir3 = TextboxSearchDir3.Text;
        }

        private void buttonSearchDir4_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir4.Text = selectSearchDirectory(TextboxSearchDir4.Text);
            mIO.SearchDir4 = TextboxSearchDir4.Text;
        }

        private void buttonSearchDir5_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir5.Text = selectSearchDirectory(TextboxSearchDir5.Text);
            mIO.SearchDir5 = TextboxSearchDir5.Text;
        }

        private void buttonSearchDir6_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir6.Text = selectSearchDirectory(TextboxSearchDir6.Text);
            mIO.SearchDir6 = TextboxSearchDir6.Text;
        }

        private void buttonSearchDir7_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir7.Text = selectSearchDirectory(TextboxSearchDir7.Text);
            mIO.SearchDir7 = TextboxSearchDir7.Text;
        }

        private void buttonSearchDir8_Click(object sender, RoutedEventArgs e)
        {
            TextboxSearchDir8.Text = selectSearchDirectory(TextboxSearchDir8.Text);
            mIO.SearchDir8 = TextboxSearchDir8.Text;
        }
        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveBeforeClosing();
        }

        private void ButtonClearSearchAllDirsAndFiles_Click(object sender, RoutedEventArgs e)
        {
            clearSearchAllDirsAndFiles();
        }

        private void ButtonSearchAllDirsAndFiles_Click(object sender, RoutedEventArgs e)
        {
            clearSearchAllDirsAndFiles();
            mIO.FilterList(ref mAllFilesOrDirsDatagridList, false);
            dataGridDirs.Items.Refresh();
            dataGridDirs.ItemsSource = mAllFilesOrDirsDatagridList;
            FitToContent();
            labelSearchAllDirsInformation.Content = "Hittade " + mAllFilesOrDirsDatagridList.Count.ToString() + " st filer/bibliotek";
        }

        private void ButtonSearchDuplicates_Click(object sender, RoutedEventArgs e)
        {
            clearSearchAllDirsAndFiles();
            mIO.FilterList(ref mAllFilesOrDirsDatagridList, true);
            dataGridDirs.Items.Refresh();
            dataGridDirs.ItemsSource = mAllFilesOrDirsDatagridList;
            FitToContent();
            labelSearchAllDirsInformation.Content = "Hittade " + mAllFilesOrDirsDatagridList.Count.ToString() + " st filer/bibliotek";
        }

        #endregion

        #region Datagrid

        private void dataGridDirs_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var dg = (DataGrid)sender;
                var item = dg.SelectedItem;
                if (item == null)
                    return;

                string message = ((MyTuple)item).FullName;
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DependencyObject source = (DependencyObject)e.OriginalSource;
                var dg = (DataGrid)sender;
                var item = dg.SelectedItem;
                if (item == null)
                    return;
                var tuple = (MyTuple)item;
                if (source == null)
                {
                    return;
                }
                else
                {
                    var strDir = tuple.FullName;
                    if (!tuple.IsDirectory())
                    {
                        strDir = tuple.FullName.GetPathFromFullFileName();
                    }
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = strDir;
                    process.Start();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FitToContent()
        {
            // where dg is my data grid's name...
            foreach (DataGridColumn column in dataGridDirs.Columns)
            {
                //if you want to size ur column as per the cell content
                column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);

                //if you want to size ur column as per the column header
                //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader);

                //if you want to size ur column as per both header and cell content
                //column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);

                //if (column.Header == "FileName")
                //    column.CellStyle.BasedOn.
            }
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => AlterRow(e)));
        }

        private void AlterRow(DataGridRowEventArgs e)
        {
            try
            {
                var cellColumn1 = GetCell(dataGridDirs, e.Row, 0);
                if (cellColumn1 == null) return;

                var cellColumn2 = GetCell(dataGridDirs, e.Row, 1);
                if (cellColumn2 == null) return;

                var cellColumn3 = GetCell(dataGridDirs, e.Row, 2);
                if (cellColumn3 == null) return;

                var cellColumn4 = GetCell(dataGridDirs, e.Row, 3);
                if (cellColumn4 == null) return;

                var cellColumn5 = GetCell(dataGridDirs, e.Row, 4);
                if (cellColumn5 == null) return;

                var itemCellColumn1 = e.Row.Item as MyTuple;
                if (itemCellColumn1 == null) return;

                var value = itemCellColumn1.FullName;

                if (itemCellColumn1.IsDirectory())
                {
                    cellColumn1.FontWeight = FontWeights.Bold;
                    cellColumn1.Background = Brushes.AntiqueWhite;
                    cellColumn2.FontWeight = FontWeights.Bold;
                    cellColumn2.Background = Brushes.AntiqueWhite;
                    cellColumn3.FontWeight = FontWeights.Bold;
                    //cellColumn3.Background = Brushes.Black;
                    //cellColumn4.Background = Brushes.Black;
                    //cellColumn5.Background = Brushes.Black;
                }
                else
                {
                    cellColumn1.FontWeight = FontWeights.Light;
                    cellColumn2.FontWeight = FontWeights.Light;
                    cellColumn3.FontWeight = FontWeights.Light;
                }

                //cell.Background = Brushes.Yellow;
                //else
                //    cell.Background = Brushes.Green;

            }
            catch (Exception ex)
            {
                MessageBox.Show("HELVETE! " + ex.Message);
            }
        }

        public static DataGridRow GetRow(DataGrid grid, int index)
        {
            var row = grid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;

            if (row == null)
            {
                // may be virtualized, bring into view and try again
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static DataGridCell GetCell(DataGrid host, DataGridRow row, int columnIndex)
        {
            if (row == null) return null;

            var presenter = GetVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null) return null;

            // try to get the cell but it may possibly be virtualized
            var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            if (cell == null)
            {
                // now try to bring into view and retreive the cell
                host.ScrollIntoView(row, host.Columns[columnIndex]);
                cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            }
            return cell;
        }

        #endregion


        private void clearSearchAllDirsAndFiles()
        {
            try
            {
                mAllFilesOrDirsDatagridList.Clear();
                dataGridDirs.Items.Refresh();
                dataGridDirs.ItemsSource = mAllFilesOrDirsDatagridList;
                labelSearchAllDirsInformation.Content = " - ";
            }
            catch (Exception e)
            {
                MessageBox.Show("clearSearchAllDirsAndFiles(): " + e.Message);
            }
        }

        private string selectSearchDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                dir = "C:\\";

            System.Windows.Forms.FolderBrowserDialog dialog =
                new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Välj startbibliotek för sökning! ",
                    ShowNewFolderButton = true,

                    SelectedPath = dir
                };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return "C:\\";
        }

        private void TextboxSearchForDirOrFile_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key != System.Windows.Input.Key.Enter) return;

            // your event handler here
            //e.Handled = true;
            //ButtonSearchAllDirsAndFiles_Click(sender, e);
        }

        private void TextboxSearchForDirOrFile_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Up || 
                e.Key == Key.Down || e.Key == Key.LeftShift || e.Key == Key.RightShift || 
                e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl )
                return;

            mIO.SearchPattern = TextboxSearchForDirOrFile.Text;
            ButtonSearchAllDirsAndFiles_Click(sender, e);
        }


        private string OpenDirectorySelectionDialog(string startPath, string message)
        {
            string returnSelection = "";

            if (!Directory.Exists(startPath))
                return returnSelection;

            System.Windows.Forms.FolderBrowserDialog dialog =
            new System.Windows.Forms.FolderBrowserDialog
            {
                Description = message,
                ShowNewFolderButton = true,

                SelectedPath = startPath
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                returnSelection = dialog.SelectedPath;
            }
            return returnSelection;
        }

        private void ButtonLoadListOfFileAndDirs_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                labelLoadFoundDirsAndFiles.Content = "Laddar....... ";
                mIO.LoadListOfAllSearchedDirsAndFiles();
                //labelLoadFoundDirsAndFiles.Content = "Laddat " + nrOfFoundItems.ToString() + " items i själva systemet typ";
                labelLoadFoundDirsAndFiles.Content = "Laddat " + mIO.NrOfItemsInCache().ToString() + " items i själva systemet typ";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public void longTimeWork()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            // before the long time work, change mouse cursor to wait cursor

            worker.DoWork += doWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();  //start doing some long long time work but GUI can update
        }


        private void doWork(object sender, DoWorkEventArgs e)
        {
            var nrOfFoundItems = mIO.LoadListOfAllSearchedDirsAndFiles();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //long time work is done();
            labelLoadFoundDirsAndFiles.Content = "Laddat " + mIO.NrOfItemsInCache().ToString() + " items i själva systemet typ";
            Mouse.OverrideCursor = null;  //return mouse cursor to normal
        }


    }
}
