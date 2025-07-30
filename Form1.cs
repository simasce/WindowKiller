using System.Diagnostics;

namespace WindowKiller
{
    public partial class Form1 : Form
    {
        private WindowData? CurrentWindowData { get; set; } = null;
        private bool IsInKillMode { get; set; } = false;

        public Form1()
        {
            InitializeComponent();

            this.Icon = MainWindowHandler.GetWindowIcon();

            // Make the form borderless and full screen
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            // Make the form topmost
            this.TopMost = true;

            // Set transparency
            this.BackColor = Color.LightSalmon;
            this.TransparencyKey = Color.LightSalmon;
            this.AllowTransparency = true;

            this.ShowInTaskbar = false;

            // Enable double buffering for smoother GDI drawing
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.timer1.Interval = 30;
            this.timer1.Start();

            this.MouseClick += Form1_MouseClick;
        }

        // Override OnPaint for GDI drawing
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            bool windowFound = CurrentWindowData != null;
            var mousePos = Cursor.Position;
            string programTarget = CurrentWindowData?.Process.ProcessName ?? "";

            if(!string.IsNullOrEmpty(programTarget))
            {
                programTarget += ".exe";
            }

            // Draw info texts as red text at the bottom right
            string coords = $"X: {mousePos.X}  Y: {mousePos.Y}";
            using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Red))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                SizeF coordsSize = e.Graphics.MeasureString(coords, font);
                SizeF targetTextSize = e.Graphics.MeasureString(programTarget, font);

                float xCoords = this.ClientSize.Width - coordsSize.Width - 20;
                float yCoords = this.ClientSize.Height - coordsSize.Height - 45;

                float xTargetText = this.ClientSize.Width - targetTextSize.Width - 20;
                float yTargetText = yCoords - targetTextSize.Height;

                if(windowFound)
                {
                    // Draw target text above the coordinates
                    e.Graphics.DrawString(programTarget, font, brush, xTargetText, yTargetText);
                }              
                // Draw coordinates text
                e.Graphics.DrawString(coords, font, brush, xCoords, yCoords);
            }

            if(CurrentWindowData != null && windowFound)
            {
                var rect = new Rectangle(
                        CurrentWindowData.X,
                        CurrentWindowData.Y,
                        CurrentWindowData.Width,
                        CurrentWindowData.Height
                    );

                using (var pen = new Pen(Color.Red, 4))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsInKillMode) // Do not update while killing
                return;

            CurrentWindowData = WindowTargetParser.GetWindowTargetUnderCursor(this.Handle);
            this.Invalidate(); // Redraw
        }

        private void Form1_MouseClick(object? sender, MouseEventArgs e)
        {
            if (IsInKillMode)
                return;

            if(e.Button == MouseButtons.Right)
            {
                this.Close();
            }
            else
            {
                if (CurrentWindowData != null)
                {
                    KillProcess(CurrentWindowData.Process);
                }
            }
        }

        public void TriggerKillProcess()
        {
            if (CurrentWindowData != null)
            {
                KillProcess(CurrentWindowData.Process);
            }
        }

        private void KillProcess(Process proc)
        {
            IsInKillMode = true;
            string procName = proc.ProcessName + ".exe";
            DialogResult res = MessageBox.Show($"This will kill the following process: {procName}\nAre you sure you want to continue?", 
                $"Kill {procName}?", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button1);

            if(res == DialogResult.Yes)
            {
                proc.Kill(true);
                this.Close();
            }
            IsInKillMode = false;
        }
    }
}
