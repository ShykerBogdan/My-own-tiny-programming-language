
using Freel.Entities;
using Freel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using Freel.Tools;

namespace Freel.ViewModels
{
    class ViewModel : ViewModelBase
    {
        Lexer lexer;
        ParserRecursiveDescent parser;
        private string rows;
        public string Rows
        {
            get { return rows; }
            set
            {
                SetProperty(ref rows, value, "Rows");
            }
        }
        private string programText;
        public string ProgramText
        {
            get { return programText; }
            set
            {
                SetProperty(ref programText, value, "ProgramText");
                Run.RaiseCanExecuteChanged();
                SaveFile.RaiseCanExecuteChanged();
            }
        }
        private string outputText;
        public string OutputText
        {
            get { return outputText; }
            set
            {
                SetProperty(ref outputText, value, "OutputText");               
            }
        }

        private string currentFilname;
        public string CurrentFilename
        {
            get { return currentFilname; }
            set
            {
                SetProperty(ref currentFilname, value, "CurrentFilename");
                DeleteFile.RaiseCanExecuteChanged();
            }
        }


      
        public ObservableCollection<Token> OutputTokens { get; set; }
        public ObservableCollection<Identifier> OutputIdentifiers { get; set; }
        public ObservableCollection<Constant> OutputConstants { get; set; }
        public ObservableCollection<Error> OutputErrors { get; set; }

        RelayCommand addRow;
        public RelayCommand AddRow
        {
            get
            {
                return addRow ?? (addRow = new RelayCommand(ExecuteAddRow));
            }
        }
        RelayCommand run;
        public RelayCommand Run
        {
            get
            {
                return run ?? (run = new RelayCommand(ExecuteRun, CanExecuteCode));
            }
        }
        RelayCommand openFile;
        public RelayCommand OpenFile
        {
            get
            {
                return openFile ?? (openFile = new RelayCommand(ExecuteOpenFile));
            }
        }
        RelayCommand saveFile;
        public RelayCommand SaveFile
        {
            get
            {
                return saveFile ?? (saveFile = new RelayCommand(ExecuteSaveFile, CanExecuteCode));
            }
        }
        RelayCommand deleteFile;
        public RelayCommand DeleteFile
        {
            get
            {
                return deleteFile ?? (deleteFile = new RelayCommand(ExecuteDeleteFile, CanDeleteFile));
            }
        }

        public ViewModel()
        {
            lexer = new Lexer();
            parser = new ParserRecursiveDescent();
        
            OutputTokens = new ObservableCollection<Token>();
            OutputIdentifiers = new ObservableCollection<Identifier>();
            OutputConstants = new ObservableCollection<Constant>();
            OutputErrors = new ObservableCollection<Error>();
        }

        private void ExecuteAddRow(object obj)
        {
            int count = ProgramText.Count(ch => ch == '\r');
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= count + 1; i++)
            {
                builder.Append(i + Environment.NewLine);
            }
            Rows = builder.ToString();
        }

        private bool CanExecuteCode(object obj)
        {
            if (string.IsNullOrWhiteSpace(ProgramText))
                return false;
            return true;
        }
        private bool CanDeleteFile(object obj)
        {
            if (String.IsNullOrEmpty(CurrentFilename))
                return false;
            return true;
        }
        private void ExecuteOpenFile(object obj)
        {
            ClearTables();

            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "Free language|*.Freel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ReadFile(openFileDialog.FileName);
                    CurrentFilename = openFileDialog.FileName;
                }
            }
        }
        private void ExecuteSaveFile(object obj)
        {
            using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveFileDialog.Filter = "Free language|*.Freel";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CurrentFilename = saveFileDialog.FileName;
                    StreamWriter sw = new StreamWriter(CurrentFilename);
                    sw.WriteLine(ProgramText);
                    sw.Close();
                }
            }
        }
        private void ExecuteDeleteFile(object obj)
        {
            if (System.Windows.MessageBox.Show("Do you want to delete file permanently?",
                 "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                File.Delete(currentFilname);
                ProgramText = String.Empty;
                CurrentFilename = String.Empty;
            }
        }
        private void ExecuteRun(object obj)
        {
            ClearTables();
            var output = lexer.Run(ProgramText);
            OutputTokens = OutputTokens.CopyFrom(output.outputTokens);
            OutputIdentifiers = OutputIdentifiers.CopyFrom(output.outputIdentifiers);
            OutputConstants = OutputConstants.CopyFrom(output.outoutConstans);
            OutputErrors = OutputErrors.CopyFrom(output.errors, parser.Process(output.outputTokens));
            OutputText = parser.Rezult;
        }

        private void ReadFile(string FilePath)
        {
            using (StreamReader sr = new StreamReader(FilePath))
            {
                ProgramText = sr.ReadToEnd();
            }
        }
        private void ClearTables()
        {
            OutputTokens.Clear();
            OutputIdentifiers.Clear();
            OutputConstants.Clear();
            OutputErrors.Clear();
        }
    }
}
