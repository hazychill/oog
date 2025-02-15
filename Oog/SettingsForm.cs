using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Oog {
  public class SettingsForm : Form {
    PropertyGrid propertyGrid;
    Button okButton;
    Button cancelButton;

    OogSettings settings;

    public SettingsForm() {
      InitializeComponents();
    }

    private void InitializeComponents() {
      const int FORM_WIDTH = 400;
      const int FORM_HEIGHT = 400;
      
      propertyGrid = new PropertyGrid();
      okButton = new Button();
      cancelButton = new Button();
      propertyGrid.SuspendLayout();
      okButton.SuspendLayout();
      cancelButton.SuspendLayout();

      propertyGrid.Location = new Point(5, 5);
      propertyGrid.Size = new Size(FORM_WIDTH-20, FORM_HEIGHT-80);
      propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      propertyGrid.ToolbarVisible = false;

      okButton.Location = new Point(FORM_WIDTH-75*2-40, FORM_HEIGHT-60);
      okButton.Size = new Size(75, 23);
      okButton.Text = "OK";
      okButton.FlatStyle = FlatStyle.Flat;
      okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
      okButton.Click += delegate {
        this.DialogResult = DialogResult.OK;
        this.Close();
      };

      cancelButton.Location = new Point(FORM_WIDTH-75-20, FORM_HEIGHT-60);
      cancelButton.Size = new Size(75, 23);
      cancelButton.Text = "Cancel";
      cancelButton.FlatStyle = FlatStyle.Flat;
      cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
      cancelButton.Click += delegate {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
      };

      this.Size = new Size(FORM_WIDTH, FORM_HEIGHT);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.Text = "Settings";
      this.Controls.AddRange(new Control[]{
        propertyGrid,
        okButton,
        cancelButton
      });

      propertyGrid.ResumeLayout(false);
      okButton.ResumeLayout(false);
      cancelButton.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public OogSettings Settings {
      get { return settings; }
      set {
        settings = value;
        propertyGrid.SelectedObject = settings;
        propertyGrid.ExpandAllGridItems();
      }
    }
  }
}