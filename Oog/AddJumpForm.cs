using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Oog {
  public class AddJumpForm : Form {

    public AddJumpForm() {
      InitializeComponents();
    }

    Label nameLabel;
    Label pathLabel;
    TextBox nameTextBox;
    TextBox pathTextBox;
    Button okButton;
    Button cancelButton;

    private void InitializeComponents() {
      nameLabel = new Label();
      pathLabel = new Label();
      nameTextBox = new TextBox();
      pathTextBox = new TextBox();
      okButton = new Button();
      cancelButton = new Button();

      this.SuspendLayout();
      nameLabel.SuspendLayout();
      pathLabel.SuspendLayout();
      nameTextBox.SuspendLayout();
      pathTextBox.SuspendLayout();
      okButton.SuspendLayout();
      cancelButton.SuspendLayout();
      
      nameLabel.Text = "Name";
      nameLabel.AutoSize = true;
      nameLabel.Location = new Point(3, 3);
      
      pathLabel.Text = "Path";
      pathLabel.AutoSize = true;
      pathLabel.Location = new Point(3, 30);

      nameTextBox.Size = new Size(250, 19);
      nameTextBox.Location = new Point(40, 2);
      nameTextBox.BorderStyle = BorderStyle.FixedSingle;

      pathTextBox.Size = new Size(250, 19);
      pathTextBox.Location = new Point(40, 29);
      pathTextBox.BorderStyle = BorderStyle.FixedSingle;

      okButton.Text = "OK";
      okButton.Size = new Size(75, 23);
      okButton.Location = new Point((300-75)/2, 60);
      okButton.FlatStyle = FlatStyle.Flat;
      okButton.Click += delegate {
        this.DialogResult = DialogResult.OK;
        this.Close();
      };

      cancelButton.Text = "Cancel";
      cancelButton.Size = new Size(75, 23);
      cancelButton.Location = new Point(215, 60);
      cancelButton.FlatStyle = FlatStyle.Flat;
      cancelButton.Click += delegate {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
      };

      this.Size = new Size(300, 120);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Text = "Jump target";
      this.Controls.AddRange(new Control[]{
        nameLabel,
        pathLabel,
        nameTextBox,
        pathTextBox,
        okButton,
        cancelButton
      });

      nameLabel.ResumeLayout(false);
      pathLabel.ResumeLayout(false);
      nameTextBox.ResumeLayout(false);
      pathTextBox.ResumeLayout(false);
      okButton.ResumeLayout(false);
      cancelButton.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ItemName {
      get { return nameTextBox.Text; }
      set { nameTextBox.Text = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ItemPath {
      get { return pathTextBox.Text; }
      set { pathTextBox.Text = value; }
    }
  }
}