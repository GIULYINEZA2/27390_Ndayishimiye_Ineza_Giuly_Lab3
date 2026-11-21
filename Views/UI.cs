using System.Drawing;
using System.Windows.Forms;

namespace LabTestSystem.Views;

public static class UI
{
    public static readonly Color Primary = Color.FromArgb(15, 118, 110);
    public static readonly Color Neutral = Color.FromArgb(229, 231, 235);

    public static void StyleForm(Form f, string title, int width, int height)
    {
        f.Text = title;
        f.AutoScaleDimensions = new SizeF(96F, 96F);
        f.AutoScaleMode = AutoScaleMode.Dpi;
        f.ClientSize = new Size(width, height);
        f.StartPosition = FormStartPosition.CenterScreen;
        f.FormBorderStyle = FormBorderStyle.FixedSingle;
        f.MaximizeBox = false;
        f.BackColor = Color.FromArgb(245, 247, 250);
        f.Font = new Font("Segoe UI", 9.5F);
    }

    public static Label AddTitle(Control parent, string text, int x, int y, int width)
    {
        var l = new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, 36),
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Primary
        };
        parent.Controls.Add(l);
        return l;
    }

    public static Label AddLabel(Control parent, string text, int x, int y, int width, int height = 22)
    {
        var l = new Label { Text = text, Location = new Point(x, y), Size = new Size(width, height) };
        parent.Controls.Add(l);
        return l;
    }

    public static TextBox AddTextBox(Control parent, string name, int x, int y, int width)
    {
        var t = new TextBox
        {
            Name = name,
            Location = new Point(x, y),
            Width = width,
            BorderStyle = BorderStyle.FixedSingle
        };
        parent.Controls.Add(t);
        return t;
    }

    public static TextBox AddMultiline(Control parent, string name, int x, int y, int width, int height, bool readOnly)
    {
        var t = new TextBox
        {
            Name = name,
            Location = new Point(x, y),
            Size = new Size(width, height),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = readOnly,
            BorderStyle = BorderStyle.FixedSingle
        };
        parent.Controls.Add(t);
        return t;
    }

    public static Button AddButton(Control parent, string name, string text, int x, int y, int width, int height, bool primary)
    {
        var b = new Button
        {
            Name = name,
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, height),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            BackColor = primary ? Primary : Neutral,
            ForeColor = primary ? Color.White : Color.FromArgb(55, 65, 81)
        };
        b.FlatAppearance.BorderSize = 0;
        parent.Controls.Add(b);
        return b;
    }

    public static GroupBox AddGroup(Control parent, string text, int x, int y, int width, int height)
    {
        var g = new GroupBox { Text = text, Location = new Point(x, y), Size = new Size(width, height) };
        parent.Controls.Add(g);
        return g;
    }

    public static RadioButton AddRadio(Control parent, string name, string text, int x, int y, int width)
    {
        var r = new RadioButton { Name = name, Text = text, Location = new Point(x, y), Size = new Size(width, 24) };
        parent.Controls.Add(r);
        return r;
    }

    public static PictureBox AddPicture(Control parent, string name, int x, int y, int width, int height)
    {
        var p = new PictureBox
        {
            Name = name,
            Location = new Point(x, y),
            Size = new Size(width, height),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.Zoom
        };
        parent.Controls.Add(p);
        return p;
    }

    public static DataGridView AddGrid(Control parent, string name, int x, int y, int width, int height)
    {
        var g = new DataGridView
        {
            Name = name,
            Location = new Point(x, y),
            Size = new Size(width, height),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            RowHeadersVisible = false
        };
        parent.Controls.Add(g);
        return g;
    }

    public static void ShowImage(PictureBox box, Image image)
    {
        Image old = box.Image;
        box.Image = image;
        if (old != null) old.Dispose();
    }
}