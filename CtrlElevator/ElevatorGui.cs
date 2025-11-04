using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CtrlElevator
{
    public partial class ElevatorGui : Form
    {
        bool go_down = false;
        bool go_up = false;
        bool arrive_G = false;
        bool arrive_1 = false;
        DbCommand dbcmd = new DbCommand();

        // Digital floor display
        private Label lblFloorDisplay;

        // Auto-close timer for ground floor (20 seconds)
        private Timer timerAutoCloseGround;

        public ElevatorGui()
        {
            InitializeComponent();

            // Create a simple digital-style label and add it to the form
            lblFloorDisplay = new Label
            {
                Name = "lblFloorDisplay",
                AutoSize = false,
                Size = new Size(80, 48),
                Location = new Point(this.ClientSize.Width - 1045, 200), // top-right-ish
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                BorderStyle = BorderStyle.FixedSingle,
                Text = ""
            };
            this.Controls.Add(lblFloorDisplay);

            // Initialize auto-close timer (20 seconds)
            timerAutoCloseGround = new Timer { Interval = 5000 }; // 20,000 ms = 20 sec
            timerAutoCloseGround.Tick += timerAutoCloseGround_Tick;

            // Set initial display based on elevator position if possible
            try
            {
                if (pictureElevator != null)
                {
                    // Choose a threshold that matches your layout. Adjust if needed.
                    lblFloorDisplay.Text = pictureElevator.Top > 240 ? "G" : "1";
                }
                else
                {
                    lblFloorDisplay.Text = "G";
                }
            }
            catch
            {
                lblFloorDisplay.Text = "G";
            }
            //MessageBox.Show("" + doorLeftup.Left);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (var con = DbConnectionFactory.CreateConnection())
                {
                    con.Open();
                    // Lightweight verification of connectivity
                    using (var cmd = new MySqlCommand("SELECT 1", con))
                    {
                        cmd.ExecuteScalar();
                    }
                }
            }
            catch (MySqlException ex) // <-- catch the MySQL provider exception
            {
                MessageBox.Show($"Database connection failed: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //      log ex.Number, ex.Server, and masked connection string for diagnostics
            }
            catch (InvalidOperationException ex) // missing config key
            {
                MessageBox.Show($"Configuration error: {ex.Message}", "Config Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            go_down = true;
            btnDown.BackColor = Color.Green;
            btnDown.Enabled = false;
            btnGFloor.Enabled = false;
            btnOpen.Enabled = false;
            btnClose.Enabled = false;
            timer_door_close_down.Enabled = true;
            timer_door_open_down.Enabled = false;
            arrive_G = false;

            // indicate movement
            lblFloorDisplay.Text = "Moving";

            // stop auto-close while moving
            timerAutoCloseGround.Enabled = false;
        }

        private void doorLeftdown_Click(object sender, EventArgs e)
        {

        }

        private void doorRightup_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureElevator_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            go_up = true;
            btnDown.BackColor = Color.Green;
            timer_door_close_up.Enabled = true;
            timer_door_open_up.Enabled = false;
            arrive_1 = false;
            btnUp.Enabled = false;
            btnClose.Enabled = false;
            btnOpen.Enabled = false;
            btn1Floor.Enabled = false;

            // indicate movement
            lblFloorDisplay.Text = "Moving";

            // stop auto-close while moving
            timerAutoCloseGround.Enabled = false;
        }

        private void timer_door_close_down_Tick(object sender, EventArgs e)
        {
            if (doorLeftdown.Left <= 165 && doorRightdown.Left >= 150)
            {
                CtrlDoor dcd = new CtrlDoor();
                dcd.Timer_door_close_down(doorLeftdown, doorRightdown);
            }
            else
            {
                // Doors finished closing
                timer_door_close_down.Enabled = false;
                dbcmd.SaveLog("Ground Floor Door Closing");
                LoadData();

                // Ensure auto-close timer is stopped when closing begins/completes
                timerAutoCloseGround.Enabled = false;

                if (go_down == true)
                {
                    timer_up.Enabled = true;
                    go_down = false;
                }

            }
        }

        private void timer_up_Tick(object sender, EventArgs e)
        {
            if (pictureElevator.Top >= 82)
            {
                Elevator eu = new Elevator();
                eu.Timer_up(pictureElevator);
            }
            else
            {
                timer_up.Enabled = false;
                dbcmd.SaveLog("Elevator Moving to the First Floor");
                LoadData();

                btnDown.Enabled = true;
                btnGFloor.Enabled = true;
                btnClose.Enabled = true;
                btnOpen.Enabled = true;
                btnUp.BackColor = Color.White;
                btn1Floor.BackColor = Color.White;
                timer_door_open_up.Enabled = true;
                timer_door_close_up.Enabled = false;
                arrive_1 = true;
                arrive_G = false;

                // arrived first floor -> update display
                lblFloorDisplay.Text = "1";
            }
        }

        private void timer_door_close_up_Tick(object sender, EventArgs e)
        {
            if (doorLeftup.Left <= 165 && doorRightup.Left >= 150)
            {
                CtrlDoor dcu = new CtrlDoor();
                dcu.Timer_door_close_up(doorLeftup, doorRightup);
            }
            else
            {
                timer_door_close_up.Enabled = false;
                dbcmd.SaveLog("First Floor Door Closing");
                LoadData();

                if (go_up == true)
                {
                    timer_down.Enabled = true;
                    go_up = false;


                }
            }
        }

        private void timer_down_Tick(object sender, EventArgs e)
        {
            if (pictureElevator.Top <= 400)
            {
                Elevator ed = new Elevator();
                ed.Timer_down(pictureElevator);
            }
            else
            {
                timer_down.Enabled = false;
                dbcmd.SaveLog("Elevator Moving to the Ground Floor");
                LoadData();

                btnUp.Enabled = true;
                btn1Floor.Enabled = true;
                btnClose.Enabled = true;
                btnOpen.Enabled = true;
                btnDown.BackColor = Color.White;
                btnGFloor.BackColor = Color.White;

                timer_door_open_down.Enabled = true;
                timer_door_close_down.Enabled = false;
                arrive_G = true;
                arrive_1 = false;

                // arrived ground floor -> update display
                lblFloorDisplay.Text = "G";
            }
        }

        private void timer_door_open_up_Tick(object sender, EventArgs e)
        {
            if (doorLeftup.Left >= 108 && doorRightup.Left <= 310)
            {
                CtrlDoor dou = new CtrlDoor();
                dou.Timer_door_open_up(doorLeftup, doorRightup);
            }
            else
            {
                timer_door_open_up.Enabled = false;
                dbcmd.SaveLog("First Floor Door Opening");
                LoadData();

                btnDown.Enabled = true;
            }
        }

        private void timer_door_open_down_Tick(object sender, EventArgs e)
        {
            if (doorLeftdown.Left >= 108 && doorRightdown.Left <= 310)
            {
                CtrlDoor dod = new CtrlDoor();
                dod.Timer_door_open_down(doorLeftdown, doorRightdown);
            }
            else
            {
                // Doors finished opening
                timer_door_open_down.Enabled = false;
                dbcmd.SaveLog("Ground Floor Door Opening");
                LoadData();

                // Start auto-close countdown only when doors are fully open and elevator is at ground
                if (arrive_G)
                {
                    timerAutoCloseGround.Enabled = true;
                }

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (arrive_G == true)
            {
                // manual close: cancel auto-close timer
                timerAutoCloseGround.Enabled = false;

                timer_door_close_down.Enabled = true;
                timer_door_open_down.Enabled = false;
            }
            else if (arrive_1 == true)
            {
                timer_door_close_up.Enabled = true;
                timer_door_open_up.Enabled = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (arrive_G == true)
            {
                timer_door_open_down.Enabled = true;
                timer_door_close_down.Enabled = false;

                // If user explicitly opens doors, ensure auto-close will start after they finish opening.
                timerAutoCloseGround.Enabled = false;
            }
            else if (arrive_1 == true)
            {
                timer_door_open_up.Enabled = true;
                timer_door_close_up.Enabled = false;
            }
        }

        private void btnGFloor_Click(object sender, EventArgs e)
        {
            go_up = true;
            btnGFloor.BackColor = Color.Green;
            arrive_1 = false;
            timer_door_close_up.Enabled = true;
            timer_door_open_up.Enabled = false;

            // indicate movement
            lblFloorDisplay.Text = "Moving";

            // stop auto-close while moving
            timerAutoCloseGround.Enabled = false;
        }

        private void btn1Floor_Click(object sender, EventArgs e)
        {
            go_down = true;
            btn1Floor.BackColor = Color.Green;
            arrive_G = false;
            timer_door_close_down.Enabled = true;
            timer_door_open_down.Enabled = false;

            // indicate movement
            lblFloorDisplay.Text = "Moving";

            // stop auto-close while moving
            timerAutoCloseGround.Enabled = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Auto-close tick handler: closes ground-floor doors after 20s
        private void timerAutoCloseGround_Tick(object sender, EventArgs e)
        {
            // stop the auto timer immediately to avoid re-entry
            timerAutoCloseGround.Enabled = false;

            // Only close if elevator is still at ground and doors are not already closing
            if (arrive_G)
            {
                // Trigger door close sequence for ground floor
                timer_door_close_down.Enabled = true;
                timer_door_open_down.Enabled = false;

                dbcmd.SaveLog("Auto-close: Ground Floor Doors closed after 20 seconds");
                LoadData();
            }
        }

        public void LoadData()
        {
            try
            {
                DbCommand dbcmd = new DbCommand();
                DataTable dt = dbcmd.ViewActionLog();
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Not Connected");
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void doorRightdown_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}