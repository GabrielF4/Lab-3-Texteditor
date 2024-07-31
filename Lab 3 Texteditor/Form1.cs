using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_3_Texteditor
{
    public partial class texteditor : Form
    {
        bool saved = true;
        string CurrentFile = "";
        char[] delimiters = new char[] { ' ', '\r', '\n' };

        private void updateCounter()
        {
            toolStripStatusLabel1.Text = "Ch: " + TextBox.Text.Length;
            toolStripStatusLabel2.Text = "| " + TextBox.Text.Replace(" ", "").Length;
            toolStripStatusLabel3.Text = "Ord: " + TextBox.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
            toolStripStatusLabel4.Text = "Rader: " + TextBox.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private DialogResult saveFilePromptYesNoCancel()
        {
            DialogResult dialogresult = MessageBox.Show("Vill du spara?", "Spara?", MessageBoxButtons.YesNoCancel);

            if (dialogresult == DialogResult.Yes)
            {

                if (CurrentFile != "")
                {
                    File.WriteAllText(CurrentFile, TextBox.Text);
                }
                else
                {
                    DialogResult result2 = saveFileDialog1.ShowDialog();

                    if (result2 == DialogResult.Cancel) { return DialogResult.Cancel; }

                    try
                    {
                        File.WriteAllText(CurrentFile = saveFileDialog1.FileName, TextBox.Text);
                    }
                    catch (Exception) { return dialogresult; }
                }

                this.Text = Path.GetFileName(openFileDialog1.FileName);
                saved = true;

            }

            return dialogresult;
        }

        private DialogResult saveFilePromptYesNo()
        {
            DialogResult dialogresult = MessageBox.Show("Vill du spara?", "Spara?", MessageBoxButtons.YesNo);

            if (dialogresult == DialogResult.Yes)
            {


                if (CurrentFile != "")
                {
                    File.WriteAllText(CurrentFile, TextBox.Text);

                }
                else
                {
                    DialogResult result2 = saveFileDialog1.ShowDialog();

                    if (result2 == DialogResult.Cancel) { return DialogResult.Cancel; }

                    try
                    {
                        File.WriteAllText(CurrentFile = saveFileDialog1.FileName, TextBox.Text);
                    }
                    catch (Exception) { return dialogresult; }
                }

                this.Text = Path.GetFileName(CurrentFile);
                saved = true;
            }

            return dialogresult;
        }

        public texteditor()
        {
            InitializeComponent();

            TextBox.AllowDrop = true;
            TextBox.DragEnter += new DragEventHandler(texteditor_DragEnter);
            TextBox.DragDrop += new DragEventHandler(texteditor_DragDrop);

            updateCounter();
        }

        void texteditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }

            this.Text = Path.GetFileName(CurrentFile);
            saved = true;
        }

        void texteditor_DragDrop(object sender, DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                foreach (string file in files)
                {
                    TextBox.Text += "\n\n" + File.ReadAllText(CurrentFile = file) + "\n";

                };
            }
            else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                foreach (string file in files)
                {
                    TextBox.Text += "\n\n" + File.ReadAllText(CurrentFile = file) + "\n";

                };
            }
            else
            {
                foreach (string file in files)
                {
                    if (saved)
                    {
                        TextBox.Text = File.ReadAllText(CurrentFile = file);
                    }
                    else
                    {
                        saveFilePromptYesNoCancel();
                        TextBox.Text = File.ReadAllText(CurrentFile = file);
                    }

                };
            }

            this.Text = Path.GetFileName(CurrentFile);
            saved = true;
        }


        //New File
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
                if (saveFilePromptYesNoCancel() == DialogResult.Cancel)
                {
                    return;
                }
            }

            TextBox.Text = "";
            this.Text = "Ny fil";
            CurrentFile = "";
            saved = true;
            updateCounter();
        }

        //Open File
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (!saved) {
                DialogResult result = saveFilePromptYesNoCancel();
                if(result == DialogResult.Cancel) { return; }
            }

            openFileDialog1.Filter = "Text File|*.txt|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TextBox.Text = File.ReadAllText(CurrentFile = openFileDialog1.FileName);
                this.Text = Path.GetFileName(CurrentFile);
                saved = true;
            }

            updateCounter();
        }

        //Save File
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (CurrentFile != "")
            {
                File.WriteAllText(CurrentFile, TextBox.Text);
                this.Text = Path.GetFileName(CurrentFile);
                saved = true;
            }
            else
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
        }

        //Save File As
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.Cancel) { return; }

            try
            {
                File.WriteAllText(CurrentFile = saveFileDialog1.FileName, TextBox.Text);
            }
            catch (Exception) { }

            this.Text = Path.GetFileName(CurrentFile);
            saved = true;
        }

        //Edit operations
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.SelectAll();
        }

        //Closing program
        private void Texteditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (saved) { return; }

            DialogResult result = saveFilePromptYesNo();

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            };

            return;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            updateCounter();

            if (CurrentFile == "")
            {
                this.Text = "*Ny fil";
            }
            else
            {
                this.Text = "*" + Path.GetFileName(CurrentFile);
            }

            saved = false;

        }
    }
}
