using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace HotelRoomManagement
{
    public class LoginForm : Form
    {
        TextBox txtUser, txtPassword;
        RadioButton rbUser, rbAdmin;

        public LoginForm()
        {
            Text = "StayEase | Hotel Room Management";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(650, 440);
            MinimumSize = new Size(650, 440);
            BackColor = Color.FromArgb(242, 246, 250);
            BuildUI();
        }

        void BuildUI()
        {
            Panel banner = new Panel();
            banner.Dock = DockStyle.Top; banner.Height = 115;
            banner.BackColor = Color.FromArgb(24, 55, 86);
            Controls.Add(banner);
            Label title = new Label();
            title.Text = "STAYEASE"; title.ForeColor = Color.White;
            title.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            title.Location = new Point(35, 22); title.AutoSize = true; banner.Controls.Add(title);
            Label subtitle = new Label();
            subtitle.Text = "Hotel Room Management System";
            subtitle.ForeColor = Color.FromArgb(208, 224, 239);
            subtitle.Font = new Font("Segoe UI", 10); subtitle.Location = new Point(39, 68);
            subtitle.AutoSize = true; banner.Controls.Add(subtitle);

            Panel card = new Panel();
            card.Size = new Size(390, 250); card.Location = new Point(125, 140);
            card.BackColor = Color.White; card.BorderStyle = BorderStyle.FixedSingle; Controls.Add(card);
            Label heading = new Label(); heading.Text = "Sign in to continue";
            heading.Font = new Font("Segoe UI", 15, FontStyle.Bold); heading.ForeColor = Color.FromArgb(24,55,86);
            heading.Location = new Point(25, 15); heading.AutoSize = true; card.Controls.Add(heading);

            rbUser = new RadioButton(); rbUser.Text = "Guest / User"; rbUser.Location = new Point(28, 53);
            rbUser.Checked = true; rbUser.AutoSize = true; card.Controls.Add(rbUser);
            rbAdmin = new RadioButton(); rbAdmin.Text = "Administrator"; rbAdmin.Location = new Point(160, 53);
            rbAdmin.AutoSize = true; card.Controls.Add(rbAdmin);

            AddLabel(card, "Username", 28, 88);
            txtUser = new TextBox(); txtUser.Location = new Point(28, 108); txtUser.Width = 330;
            card.Controls.Add(txtUser);
            AddLabel(card, "Password", 28, 139);
            txtPassword = new TextBox(); txtPassword.Location = new Point(28, 159); txtPassword.Width = 330;
            txtPassword.UseSystemPasswordChar = true; card.Controls.Add(txtPassword);
            Button login = MakeButton("SIGN IN", 28, 196, 155, 35);
            login.Click += LoginClick; card.Controls.Add(login);
            Button register = MakeButton("CREATE USER ACCOUNT", 192, 196, 166, 35);
            register.BackColor = Color.FromArgb(230, 237, 244); register.ForeColor = Color.FromArgb(24,55,86);
            register.Click += RegisterClick; card.Controls.Add(register);
            AcceptButton = login;
        }

        void AddLabel(Control parent, string text, int x, int y)
        {
            Label l = new Label(); l.Text = text; l.Location = new Point(x,y);
            l.AutoSize = true; l.ForeColor = Color.FromArgb(70,85,100); parent.Controls.Add(l);
        }

        Button MakeButton(string text, int x, int y, int w, int h)
        {
            Button b = new Button(); b.Text = text; b.Location = new Point(x,y); b.Size = new Size(w,h);
            b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0;
            b.BackColor = Color.FromArgb(0, 125, 145); b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 9, FontStyle.Bold); b.Cursor = Cursors.Hand; return b;
        }

        void LoginClick(object sender, EventArgs e)
        {
            try
            {
                string user = txtUser.Text.Trim();
                string pass = txtPassword.Text;
                if (user == "" || pass == "") { MessageBox.Show("Enter username and password."); return; }

                if (rbAdmin.Checked)
                {
                    // Demo credentials; change them before using this for real data.
                    if (user == "admin" && pass == "Admin@123")
                    {
                        Hide(); new MainForm(user, true).ShowDialog(); Show();
                    }
                    else MessageBox.Show("Invalid administrator login.");
                }
                else
                {
                    DataTable dt = Db.GetTable("SELECT UserId, FullName FROM Users WHERE Username=@u AND PasswordText=@p",
                        Db.P("@u", user), Db.P("@p", pass));
                    if (dt.Rows.Count == 1)
                    {
                        Hide(); new MainForm(Convert.ToInt32(dt.Rows[0]["UserId"]), dt.Rows[0]["FullName"].ToString()).ShowDialog(); Show();
                    }
                    else MessageBox.Show("Invalid login. Create an account if you are a new user.");
                }
            }
            catch (Exception ex) { MessageBox.Show("Database connection failed.\n" + ex.Message, "Connection error"); }
        }

        void RegisterClick(object sender, EventArgs e)
        {
            string name = Prompt("Full name:");
            if (name == null || name.Trim() == "") return;
            string username = Prompt("Choose username:");
            if (username == null || username.Trim() == "") return;
            string password = Prompt("Choose password:");
            if (password == null || password == "") return;
            try
            {
                Db.Execute("INSERT INTO Users(FullName,Username,PasswordText) VALUES(@n,@u,@p)",
                    Db.P("@n", name.Trim()), Db.P("@u", username.Trim()), Db.P("@p", password));
                MessageBox.Show("Account created. Select Guest / User and sign in.");
            }
            catch (Exception ex) { MessageBox.Show("Could not create account. Username may already exist.\n" + ex.Message); }
        }

        string Prompt(string message)
        {
            Form f = new Form(); f.Text = message; f.Size = new Size(330,140);
            f.StartPosition = FormStartPosition.CenterParent; f.FormBorderStyle = FormBorderStyle.FixedDialog;
            Label l = new Label(); l.Text = message; l.Location = new Point(12,12); l.AutoSize = true; f.Controls.Add(l);
            TextBox t = new TextBox(); t.Location = new Point(12,38); t.Width = 285; f.Controls.Add(t);
            Button ok = new Button(); ok.Text = "OK"; ok.Location = new Point(215,70); ok.DialogResult = DialogResult.OK; f.Controls.Add(ok);
            f.AcceptButton = ok;
            return f.ShowDialog(this) == DialogResult.OK ? t.Text : null;
        }
    }
}
