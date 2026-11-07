using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace HotelRoomManagement
{
    public class MainForm : Form
    {
        int userId;
        string fullName;
        bool isAdmin;
        Panel nav, content;
        DataGridView grid;
        TextBox txtRoomNo, txtType, txtPrice, txtCapacity;
        DateTimePicker dtIn, dtOut;
        ComboBox cbRoom;
        TextBox txtGuest, txtPhone;
        int selectedRoomId = 0;

        public MainForm(string username, bool admin)
        {
            fullName = username; isAdmin = admin;
            Init("Administrator Dashboard");
            ShowAdminHome();
        }

        public MainForm(int id, string name)
        {
            userId = id; fullName = name; isAdmin = false;
            Init("Guest Dashboard");
            ShowUserHome();
        }

        void Init(string heading)
        {
            Text = "StayEase | " + heading;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(950, 620);
            BackColor = Color.FromArgb(244,247,250);
            nav = new Panel(); nav.Dock = DockStyle.Left; nav.Width = 220;
            nav.BackColor = Color.FromArgb(24,55,86); Controls.Add(nav);
            Label brand = new Label(); brand.Text = "STAYEASE"; brand.Font = new Font("Segoe UI",19,FontStyle.Bold);
            brand.ForeColor = Color.White; brand.Location = new Point(22,22); brand.AutoSize = true; nav.Controls.Add(brand);
            Label welcome = new Label(); welcome.Text = "Welcome, " + fullName;
            welcome.ForeColor = Color.FromArgb(210,225,239); welcome.Location = new Point(24,65);
            welcome.AutoSize = true; nav.Controls.Add(welcome);
            content = new Panel(); content.Dock = DockStyle.Fill; content.Padding = new Padding(24);
            Controls.Add(content); content.BringToFront();
            if (isAdmin)
            {
                AddNav("Dashboard", ShowAdminHome, 115);
                AddNav("Manage Rooms", ShowRooms, 160);
                AddNav("All Bookings", ShowAllBookings, 205);
                AddNav("Guest Records", ShowGuests, 250);
            }
            else
            {
                AddNav("Dashboard", ShowUserHome, 115);
                AddNav("Book a Room", ShowBookRoom, 160);
                AddNav("My Bookings", ShowMyBookings, 205);
                AddNav("Available Rooms", ShowAvailableRooms, 250);
            }
            AddNav("Logout", delegate { Close(); }, 315);
        }

        void AddNav(string text, Action action, int y)
        {
            Button b = new Button(); b.Text = text; b.Location = new Point(15,y); b.Size = new Size(190,36);
            b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0;
            b.BackColor = Color.FromArgb(34,72,108); b.ForeColor = Color.White;
            b.TextAlign = ContentAlignment.MiddleLeft; b.Padding = new Padding(14,0,0,0);
            b.Font = new Font("Segoe UI",10); b.Cursor = Cursors.Hand;
            b.Click += delegate { action(); }; nav.Controls.Add(b);
        }

        void Clear(string title, string subtitle)
        {
            content.Controls.Clear();
            Label h = new Label(); h.Text = title; h.Font = new Font("Segoe UI",23,FontStyle.Bold);
            h.ForeColor = Color.FromArgb(24,55,86); h.Location = new Point(24,18); h.AutoSize = true; content.Controls.Add(h);
            Label s = new Label(); s.Text = subtitle; s.Font = new Font("Segoe UI",10);
            s.ForeColor = Color.FromArgb(95,110,125); s.Location = new Point(27,62); s.AutoSize = true; content.Controls.Add(s);
        }

        Button Btn(string text, int x, int y, int w, Action action)
        {
            Button b = new Button(); b.Text = text; b.Location = new Point(x,y); b.Size = new Size(w,36);
            b.BackColor = Color.FromArgb(0,125,145); b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.Font = new Font("Segoe UI",9,FontStyle.Bold);
            b.Click += delegate { action(); }; content.Controls.Add(b); return b;
        }

        void ShowAdminHome()
        {
            Clear("Admin dashboard", "A quick overview of your hotel operations.");
            AddStat("TOTAL ROOMS", "SELECT COUNT(*) FROM Rooms", 28, 115);
            AddStat("TOTAL BOOKINGS", "SELECT COUNT(*) FROM Bookings", 260, 115);
            AddStat("REGISTERED GUESTS", "SELECT COUNT(*) FROM Users", 492, 115);
            AddStat("PENDING BOOKINGS", "SELECT COUNT(*) FROM Bookings WHERE Status='Booked'", 28, 245);
            Label note = new Label(); note.Text = "Use Manage Rooms to create, edit and delete rooms. Use All Bookings to track guest reservations.";
            note.Location = new Point(30,365); note.Size = new Size(750,55); note.Font = new Font("Segoe UI",11);
            note.ForeColor = Color.FromArgb(70,85,100); content.Controls.Add(note);
        }

        void AddStat(string caption, string sql, int x, int y)
        {
            Panel p = new Panel(); p.Location = new Point(x,y); p.Size = new Size(210,105);
            p.BackColor = Color.White; p.BorderStyle = BorderStyle.FixedSingle; content.Controls.Add(p);
            Label c = new Label(); c.Text = caption; c.Location = new Point(14,14); c.AutoSize = true;
            c.Font = new Font("Segoe UI",9,FontStyle.Bold); c.ForeColor = Color.FromArgb(95,110,125); p.Controls.Add(c);
            Label value = new Label(); value.Text = "..."; value.Location = new Point(14,40); value.AutoSize = true;
            value.Font = new Font("Segoe UI",25,FontStyle.Bold); value.ForeColor = Color.FromArgb(0,125,145); p.Controls.Add(value);
            try { value.Text = Convert.ToString(Db.Scalar(sql)); } catch { value.Text = "—"; }
        }

        void ShowUserHome()
        {
            Clear("Welcome back, " + fullName, "Manage your stay in one place.");
            AddStat("AVAILABLE ROOMS", "SELECT COUNT(*) FROM Rooms WHERE IsAvailable=1", 28, 125);
            AddStat("MY BOOKINGS", "SELECT COUNT(*) FROM Bookings WHERE UserId=" + userId, 260, 125);
            Label info = new Label(); info.Text = "Book a room, check your upcoming reservations, or review available room types.";
            info.Location = new Point(30,275); info.Size = new Size(720,45); info.Font = new Font("Segoe UI",12);
            info.ForeColor = Color.FromArgb(70,85,100); content.Controls.Add(info);
            Btn("BOOK A ROOM", 30, 340, 160, ShowBookRoom);
        }

        void ShowRooms()
        {
            Clear("Manage rooms", "Create and maintain room inventory.");
            AddLabel("Room number", 28, 105); txtRoomNo = AddText(28,128,130);
            AddLabel("Room type", 180, 105); txtType = AddText(180,128,150);
            AddLabel("Price / night", 350, 105); txtPrice = AddText(350,128,130);
            AddLabel("Capacity", 500, 105); txtCapacity = AddText(500,128,100);
            Btn("ADD ROOM", 28, 177, 115, AddRoom);
            Btn("UPDATE SELECTED", 153, 177, 155, UpdateRoom);
            Btn("DELETE SELECTED", 318, 177, 150, DeleteRoom);
            Btn("CLEAR", 478, 177, 90, delegate { txtRoomNo.Clear(); txtType.Clear(); txtPrice.Clear(); txtCapacity.Clear(); selectedRoomId=0; });
            grid = NewGrid(28,230,900,430); grid.CellClick += RoomSelected;
            LoadRooms();
        }

        void LoadRooms()
        {
            grid.DataSource = Db.GetTable("SELECT RoomId,RoomNumber,RoomType,PricePerNight,Capacity,IsAvailable FROM Rooms ORDER BY RoomNumber");
        }

        void AddRoom()
        {
            decimal price; int capacity;
            if (txtRoomNo.Text.Trim()=="" || txtType.Text.Trim()=="" ||
                !decimal.TryParse(txtPrice.Text,out price) || !int.TryParse(txtCapacity.Text,out capacity) || price < 0 || capacity < 1)
            { MessageBox.Show("Enter room number, type, valid price and capacity."); return; }
            try { Db.Execute("INSERT INTO Rooms(RoomNumber,RoomType,PricePerNight,Capacity,IsAvailable) VALUES(@n,@t,@p,@c,1)",
                Db.P("@n",txtRoomNo.Text.Trim()),Db.P("@t",txtType.Text.Trim()),Db.P("@p",price),Db.P("@c",capacity));
                LoadRooms(); MessageBox.Show("Room added."); }
            catch(Exception ex) { MessageBox.Show("Could not add room. Room number may already exist.\n"+ex.Message); }
        }

        void RoomSelected(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex<0)return;
            DataGridViewRow r=grid.Rows[e.RowIndex]; selectedRoomId=Convert.ToInt32(r.Cells["RoomId"].Value);
            txtRoomNo.Text=Convert.ToString(r.Cells["RoomNumber"].Value); txtType.Text=Convert.ToString(r.Cells["RoomType"].Value);
            txtPrice.Text=Convert.ToString(r.Cells["PricePerNight"].Value); txtCapacity.Text=Convert.ToString(r.Cells["Capacity"].Value);
        }

        void UpdateRoom()
        {
            if(selectedRoomId==0){MessageBox.Show("Select a room from the table first.");return;}
            decimal p; int c;
            if(!decimal.TryParse(txtPrice.Text,out p)||!int.TryParse(txtCapacity.Text,out c)||c<1||p<0){MessageBox.Show("Enter valid price and capacity.");return;}
            try { Db.Execute("UPDATE Rooms SET RoomNumber=@n,RoomType=@t,PricePerNight=@p,Capacity=@c WHERE RoomId=@id",
                Db.P("@n",txtRoomNo.Text.Trim()),Db.P("@t",txtType.Text.Trim()),Db.P("@p",p),Db.P("@c",c),Db.P("@id",selectedRoomId));
                LoadRooms();MessageBox.Show("Room updated."); } catch(Exception ex){MessageBox.Show(ex.Message);}
        }

        void DeleteRoom()
        {
            if(selectedRoomId==0){MessageBox.Show("Select a room first.");return;}
            if(MessageBox.Show("Delete selected room? Rooms with booking history cannot be deleted.","Confirm",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;
            try { Db.Execute("DELETE FROM Rooms WHERE RoomId=@id",Db.P("@id",selectedRoomId)); selectedRoomId=0; LoadRooms(); }
            catch(Exception ex){MessageBox.Show("Cannot delete this room if it has bookings. Mark it unavailable instead.\n"+ex.Message);}
        }

        void ShowBookRoom()
        {
            Clear("Book a room", "Choose a room and your check-in / check-out dates.");
            AddLabel("Guest name",28,105); txtGuest=AddText(28,128,220); txtGuest.Text=fullName;
            AddLabel("Phone",270,105); txtPhone=AddText(270,128,180);
            AddLabel("Room",28,170); cbRoom=new ComboBox(); cbRoom.Location=new Point(28,193); cbRoom.Width=420;
            cbRoom.DropDownStyle=ComboBoxStyle.DropDownList; content.Controls.Add(cbRoom);
            AddLabel("Check-in",28,235); dtIn=new DateTimePicker(); dtIn.Location=new Point(28,258); dtIn.Width=180;
            dtIn.MinDate=DateTime.Today; dtIn.Value=DateTime.Today; content.Controls.Add(dtIn);
            AddLabel("Check-out",235,235); dtOut=new DateTimePicker(); dtOut.Location=new Point(235,258); dtOut.Width=180;
            dtOut.MinDate=DateTime.Today.AddDays(1); dtOut.Value=DateTime.Today.AddDays(1); content.Controls.Add(dtOut);
            try {
                DataTable rooms=Db.GetTable("SELECT RoomId,RoomNumber,RoomType,PricePerNight FROM Rooms WHERE IsAvailable=1 ORDER BY RoomNumber");
                cbRoom.DataSource=rooms; cbRoom.DisplayMember="RoomNumber"; cbRoom.ValueMember="RoomId";
                if(rooms.Rows.Count==0)MessageBox.Show("No rooms are listed as available. Contact the administrator.");
            } catch(Exception ex){MessageBox.Show(ex.Message);}
            Btn("CONFIRM BOOKING",28,315,190, ConfirmBooking);
            Label tip=new Label();tip.Text="Note: the system checks overlapping bookings before confirming your reservation.";
            tip.Location=new Point(28,370);tip.AutoSize=true;tip.ForeColor=Color.FromArgb(95,110,125);content.Controls.Add(tip);
        }

        void ConfirmBooking()
        {
            if(cbRoom.SelectedValue==null){MessageBox.Show("No room is available to select.");return;}
            if(txtGuest.Text.Trim()==""||txtPhone.Text.Trim()==""){MessageBox.Show("Enter guest name and phone.");return;}
            if(dtOut.Value.Date<=dtIn.Value.Date){MessageBox.Show("Check-out must be after check-in.");return;}
            int room=Convert.ToInt32(cbRoom.SelectedValue);
            try
            {
                int overlap=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM Bookings WHERE RoomId=@r AND Status<>'Cancelled' AND CheckIn<@out AND CheckOut>@in",
                    Db.P("@r",room),Db.P("@out",dtOut.Value.Date),Db.P("@in",dtIn.Value.Date)));
                if(overlap>0){MessageBox.Show("This room is already booked for those dates. Please select another room or dates.");return;}
                Db.Execute("INSERT INTO Bookings(UserId,RoomId,GuestName,Phone,CheckIn,CheckOut,Status,BookedOn) VALUES(@u,@r,@g,@p,@i,@o,'Booked',GETDATE())",
                    Db.P("@u",userId),Db.P("@r",room),Db.P("@g",txtGuest.Text.Trim()),Db.P("@p",txtPhone.Text.Trim()),
                    Db.P("@i",dtIn.Value.Date),Db.P("@o",dtOut.Value.Date));
                MessageBox.Show("Booking confirmed!"); ShowMyBookings();
            }
            catch(Exception ex){MessageBox.Show("Booking failed.\n"+ex.Message);}
        }

        void ShowMyBookings()
        {
            Clear("My bookings", "View or cancel your reservations.");
            grid=NewGrid(28,110,900,430);
            try { grid.DataSource=Db.GetTable("SELECT b.BookingId,r.RoomNumber,r.RoomType,b.GuestName,b.Phone,b.CheckIn,b.CheckOut,b.Status,b.BookedOn FROM Bookings b INNER JOIN Rooms r ON b.RoomId=r.RoomId WHERE b.UserId=@u ORDER BY b.BookedOn DESC",Db.P("@u",userId)); }
            catch(Exception ex){MessageBox.Show(ex.Message);}
            Btn("CANCEL SELECTED BOOKING",28,560,230,CancelMyBooking);
        }

        void CancelMyBooking()
        {
            if(grid.CurrentRow==null){MessageBox.Show("Select a booking first.");return;}
            int id=Convert.ToInt32(grid.CurrentRow.Cells["BookingId"].Value);
            try { Db.Execute("UPDATE Bookings SET Status='Cancelled' WHERE BookingId=@id AND UserId=@u AND Status='Booked'",Db.P("@id",id),Db.P("@u",userId)); MessageBox.Show("If the booking was active, it has been cancelled."); ShowMyBookings(); }
            catch(Exception ex){MessageBox.Show(ex.Message);}
        }

        void ShowAvailableRooms()
        {
            Clear("Available rooms", "Room inventory and nightly prices.");
            grid=NewGrid(28,110,900,430);
            try { grid.DataSource=Db.GetTable("SELECT RoomNumber,RoomType,PricePerNight,Capacity FROM Rooms WHERE IsAvailable=1 ORDER BY PricePerNight"); }
            catch(Exception ex){MessageBox.Show(ex.Message);}
            Btn("BOOK A ROOM",28,560,150,ShowBookRoom);
        }

        void ShowAllBookings()
        {
            Clear("All bookings", "Track reservations and update their status.");
            grid=NewGrid(28,110,900,410);
            try { grid.DataSource=Db.GetTable("SELECT b.BookingId,u.FullName,r.RoomNumber,b.GuestName,b.Phone,b.CheckIn,b.CheckOut,b.Status,b.BookedOn FROM Bookings b INNER JOIN Users u ON b.UserId=u.UserId INNER JOIN Rooms r ON b.RoomId=r.RoomId ORDER BY b.BookedOn DESC"); }
            catch(Exception ex){MessageBox.Show(ex.Message);}
            Btn("MARK CHECKED-IN",28,545,160,delegate{SetBookingStatus("Checked-In");});
            Btn("MARK CHECKED-OUT",200,545,175,delegate{SetBookingStatus("Checked-Out");});
            Btn("CANCEL BOOKING",387,545,155,delegate{SetBookingStatus("Cancelled");});
            Btn("REFRESH",554,545,100,ShowAllBookings);
        }

        void SetBookingStatus(string status)
        {
            if (grid == null || grid.CurrentRow == null)
            {
                MessageBox.Show("Please select a booking first.");
                return;
            }

            int id = Convert.ToInt32(
                grid.CurrentRow.Cells["BookingId"].Value
            );

            try
            {
                int rows = Db.Execute(
                    "UPDATE Bookings SET Status=@s WHERE BookingId=@id",
                    Db.P("@s", status),
                    Db.P("@id", id)
                );

                if (rows > 0)
                {
                    MessageBox.Show("Booking status updated successfully!");
                }
                else
                {
                    MessageBox.Show("Booking not found.");
                }

                ShowAllBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
            }
        }

        void ShowGuests()
        {
            Clear("Guest records", "Registered users and contact information.");
            grid=NewGrid(28,110,900,430);
            try { grid.DataSource=Db.GetTable("SELECT UserId,FullName,Username,CreatedOn FROM Users ORDER BY CreatedOn DESC"); }
            catch(Exception ex){MessageBox.Show(ex.Message);}
        }

        DataGridView NewGrid(int x,int y,int w,int h)
        {
            DataGridView g=new DataGridView(); g.Location=new Point(x,y);g.Size=new Size(w,h);
            g.BackgroundColor=Color.White;g.BorderStyle=BorderStyle.None;g.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            g.ReadOnly=true;g.AllowUserToAddRows=false;g.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect=false;g.RowHeadersVisible=false;g.EnableHeadersVisualStyles=false;
            g.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(24,55,86);
            g.ColumnHeadersDefaultCellStyle.ForeColor=Color.White;g.ColumnHeadersDefaultCellStyle.Font=new Font("Segoe UI",9,FontStyle.Bold);
            g.DefaultCellStyle.Font=new Font("Segoe UI",9);g.RowTemplate.Height=28;content.Controls.Add(g);return g;
        }

        void AddLabel(string text,int x,int y)
        {
            Label l=new Label();l.Text=text;l.Location=new Point(x,y);l.AutoSize=true;
            l.ForeColor=Color.FromArgb(70,85,100);l.Font=new Font("Segoe UI",9,FontStyle.Bold);content.Controls.Add(l);
        }

        TextBox AddText(int x,int y,int w)
        {
            TextBox t=new TextBox();t.Location=new Point(x,y);t.Width=w;t.Font=new Font("Segoe UI",10);content.Controls.Add(t);return t;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
