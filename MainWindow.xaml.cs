using FixUrlaub.Util;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using FixUrlaub.Controls;
using System.Drawing.Imaging;

namespace FixUrlaub.Masks
{
    internal class VacMainForm : VacPaperForm
    {
        public VacLeaderForm vlf;
        public VacCalendarForm vcf;
        public VacADLogin vadl;
        public VacSettingsForm vsf;

        private int xHeight;
        private float HeightRatio = 0.462963f;
        private int xWidth;
        private float WidthRatio = 1;

        #region Controls
        public Label 
            SettingsIcon, 
            ExitIcon, 
            NameLine, 
            BirthLine, 
            IDLine, 
            DepLine, 
            YearAnnouncement, 
            TakenVacLabel, 
            DaysLabel, 
            AnnounceLabel, 
            YearVacLabel,
            SpecVacLabel,
            UnpaidVacLabel,
            ReasonLabel;
        public SeeThroughTextBox 
            NameLineField, 
            IDLineField, 
            DepLineField, 
            CurrentYearField, 
            TakenVacField, 
            LeftVacField;
        public DateTimePicker 
            BirthLineField;
        #endregion

        public VacMainForm() : base("VacMainForm")
        {
            #region Child-Forms
            vlf = new VacLeaderForm();
            vcf = new VacCalendarForm();
            vadl = new VacADLogin();
            vsf = new VacSettingsForm();
            #endregion

            Bounds = new Rectangle(200, 200, (int)Math.Round(500 * FixMath.VacationFormularAspect, 0), 500);
            xHeight = Height;
            xWidth = Width;

            LoadControls();
        }

        private void LoadControls()
        {
            Language lang = vsf.cfg.CurrentLanguage;

            #region Icons
            SettingsIcon = new Label()
            {
                Name = "SettingsIcon",
                Bounds = new Rectangle(1, 1, 30, 30)
            };
            SettingsIcon.Paint += (object sender, PaintEventArgs e) =>
            {
                // Swaps white with the Secondary Color and uses those changes attributed to draw the new bitmap

                using (Bitmap bmp = ((Icon)resources.GetObject("Vac_Gear")).ToBitmap())
                {
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.White;
                    colorMap[0].NewColor = AppliedTheme.Secondary;
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);

                    Rectangle rect = new Rectangle(0, 0, ((Control)sender).Width + 10, ((Control)sender).Height + 10);
                    e.Graphics.DrawImage(bmp, rect, 0, 0, 350, 350, GraphicsUnit.Point, attr);  // Using Point as Unit, so it renders it out smoothly
                }
            };
            SettingsIcon.Click += (object sender, EventArgs e) =>
            {
                vsf.ShowDialog();
                vsf.BringToFront();
            };
            ExitIcon = new Label()
            {
                Name = "ExitIcon",
                Text = "X",
                Bounds = new Rectangle(Width - 31, 1, 30, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(FrutigerBoldFam, 20f),
                ForeColor = vcf.AppliedTheme.Secondary
            };
            ExitIcon.Click += (object sender, EventArgs e) => this.Close();


            Controls.Add(SettingsIcon);
            Controls.Add(ExitIcon);
            #endregion


            #region Lablels and Texts
            NameLine = new Label()
            {
                Text = lang.NameLine,
                Name = lang.NameLine,
                Location = new Point(Width / 2 - 50, 30),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true,
                ForeColor = vsf.AppliedTheme.Secondary
            };
            BirthLine = new Label()
            {
                Text = lang.BornLine,
                Name = lang.BornLine,
                Location = new Point(Width / 2 - 50, 70),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true,
                ForeColor = vsf.AppliedTheme.Secondary
            };
            IDLine = new Label()
            {
                Text = " " + lang.UserIDLine,
                Name = lang.UserIDLine,
                Location = new Point((int)Math.Round(Width / 1.25f - 50), 70),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true,
                ForeColor = vsf.AppliedTheme.Secondary
            };
            DepLine = new Label()
            {
                Text = lang.DepartmentLine,
                Name = lang.DepartmentLine,
                Location = new Point(Width / 2 - 50, 110),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true,
                ForeColor = vsf.AppliedTheme.Secondary
            };

            YearAnnouncement = new Label()
            {
                Text = lang.YearTag,
                Name = lang.YearTag,
                Location = new Point(35, 145),
                Font = new Font(FrutigerBoldFam, 12),
                AutoSize = true,
                ForeColor = vsf.AppliedTheme.Secondary
            };

            Controls.Add(NameLine);
            Controls.Add(BirthLine);
            Controls.Add(IDLine);
            Controls.Add(DepLine);

            Controls.Add(YearAnnouncement);
            #endregion


            #region Textfields
            NameLineField = new SeeThroughTextBox(this)
            {
                Name = "NameLineField",
                ForeColor = vcf.AppliedTheme.Tertiary,
                Bounds = new Rectangle(NameLine.Location.X + NameLine.Width, 23, Width - (NameLine.Location.X + NameLine.Width) - 35, 20)
            };
            BirthLineField = new DateTimePicker()
            {
                Name = "BirthLineField",
                Bounds = new Rectangle((Width / 2 - 50) + BirthLine.Width, 63, IDLine.Location.X - ((Width / 2 - 50) + BirthLine.Width), 20),
                CalendarFont = new Font(FrutigerFam, 12),
                Font = new Font(FrutigerFam, 12),
                CalendarForeColor = vcf.AppliedTheme.Secondary,
                CalendarTrailingForeColor = vcf.AppliedTheme.Secondary,
                CalendarMonthBackground = vcf.AppliedTheme.Primary,
                CalendarTitleBackColor = vcf.AppliedTheme.Primary,
                CalendarTitleForeColor = vcf.AppliedTheme.Secondary,
                Value = DateTime.Today.AddDays(30),                         // TODO: Put in Birthday Date Automatically
                Format = DateTimePickerFormat.Short
            };
            IDLineField = new SeeThroughTextBox(this)
            {
                Name = "IDLineField",
                ForeColor = vcf.AppliedTheme.Tertiary,
                Bounds = new Rectangle(IDLine.Location.X + IDLine.Width, 63, Width - (IDLine.Location.X + IDLine.Width) - 35, 20)
            };
            DepLineField = new SeeThroughTextBox(this)
            {
                Text = "",                                                  // TODO: Put Department in here
                Name = "DepLineField",
                ForeColor = vcf.AppliedTheme.Tertiary,
                Bounds = new Rectangle(DepLine.Location.X + DepLine.Width, 103, Width - (DepLine.Location.X + DepLine.Width) - 35, 20)
            };
            CurrentYearField = new SeeThroughTextBox(this)
            {
                Text = DateTime.Today.Year.ToString(),
                Name = "CurrentYearField",
                ForeColor = vcf.AppliedTheme.Tertiary,
                Bounds = new Rectangle(YearAnnouncement.Location.X + YearAnnouncement.Width, 145, 50, 20)
            };
            TakenVacField = new SeeThroughTextBox(this)
            {
                Text = "",                                                  // TODO: Put Vacation Data in here
                Name = "TakenVacField",
                ForeColor = vcf.AppliedTheme.Tertiary,
                Bounds = new Rectangle(35, 190, 40, 20)
            };
            Controls.Add(TakenVacField);
            TakenVacLabel = new Label()
            {
                Text = lang.RemainingVac,
                Name = "TakenVacLabel",
                ForeColor = vcf.AppliedTheme.Secondary,
                Location = new Point(TakenVacField.Location.X + TakenVacField.Width, TakenVacField.Location.Y - 4),
                Font = new Font(FrutigerBoldFam, 12),
                AutoSize = true,
                TextAlign = ContentAlignment.BottomLeft
            };
            Controls.Add(TakenVacLabel);
            DaysLabel = new Label()
            {
                Text = 1 == 1 ? lang.Day : lang.Days,                       // TODO: Check if its singular or plural
                Name = "DaysLabel",
                ForeColor = vcf.AppliedTheme.Secondary,
                Location = new Point(600, 187),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleRight
            };
            Controls.Add(DaysLabel);
            LeftVacField = new SeeThroughTextBox(this)
            {
                Text = "",                                                  // TODO: Put Vacation Data in here
                Name = "LeftVacField",
                ForeColor = vcf.AppliedTheme.Tertiary,
                Bounds = new Rectangle(500, 190, 100, 20)
            };
            Controls.Add(LeftVacField);
            AnnounceLabel = new Label()
            {
                Text = lang.Announcement,
                Name = "AnnounceLabel",
                ForeColor = vcf.AppliedTheme.Secondary,
                Location = new Point(35, TakenVacLabel.Location.Y + 33),
                Font = new Font(FrutigerBoldFam, 12),
                AutoSize = true
            };
            Controls.Add(AnnounceLabel);
            YearVacLabel = new Label()
            {
                Text = lang.YearVac,
                Name = "YearVacLabel",
                ForeColor = vcf.AppliedTheme.Secondary,
                Location = new Point(35, AnnounceLabel.Location.Y + 30),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true
            };
            Controls.Add(YearVacLabel);
            SpecVacLabel = new Label()
            {
                Text = lang.SpecVac,
                Name = "SpecVacLabel",
                ForeColor = vcf.AppliedTheme.Secondary,
                Location = new Point(35, YearVacLabel.Location.Y + 25),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true
            };
            Controls.Add(SpecVacLabel);
            UnpaidVacLabel = new Label()
            {
                Text = lang.UnpaidVac,
                Name = "UnpaidVacLabel",
                ForeColor = vcf.AppliedTheme.Secondary,
                Location = new Point(35, SpecVacLabel.Location.Y + 25),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true
            };
            Controls.Add(UnpaidVacLabel);
            ReasonLabel = new Label()
            {
                Text = lang.Reason,
                Name = "ReasonLabel",
                ForeColor = vcf.AppliedTheme.Secondary,
                Location = new Point(35, UnpaidVacLabel.Location.Y + 50),
                Font = new Font(FrutigerFam, 12),
                AutoSize = true
            };
            Controls.Add(ReasonLabel);



            Controls.Add(NameLineField);
            Controls.Add(BirthLineField);
            Controls.Add(IDLineField);
            Controls.Add(DepLineField);

            Controls.Add(CurrentYearField);
            #endregion
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);


            int SizeRatio = (Height - 470) / 30;
            Pen LinePen = new Pen(AppliedTheme.Secondary, 2 + (SizeRatio / 5));


            e.Graphics.DrawString("fixemer",
                new Font(FrutigerBoldFam, 24 + SizeRatio),
                new SolidBrush(AppliedTheme.Secondary),
                30 + (Width / 1200 * 25),
                20 + (Height / 1000 * 25));
            e.Graphics.DrawString(vsf.cfg.CurrentLanguage.LogoTopLeft,
                new Font(FrutigerBoldFam, 18 + SizeRatio),
                new SolidBrush(AppliedTheme.Secondary),
                30 + (Width / 1200 * 25),
                55 + (Height / 1000 * 25) + SizeRatio);
            foreach(Control control in Controls)
                if(control.Name == "DaysLabel")
                    e.Graphics.DrawString("=",
                        new Font(FrutigerFam, 12 + SizeRatio),
                        new SolidBrush(AppliedTheme.Secondary),
                        control.Location.X - (110 + (SizeRatio * 12)),
                        control.Location.Y);

            #region Structural Lines
            Point[] LinePath = new Point[4]
            {
                new Point(Width, DepLine.Location.Y + DepLine.Height + 10 - (SizeRatio / 5)),
                new Point(20 + (2 * SizeRatio),DepLine.Location.Y + DepLine.Height + 10 - (SizeRatio / 5)),
                new Point(20 + (2 * SizeRatio), 350 + (25 * SizeRatio)),
                new Point(Width, 350 + (25 * SizeRatio))
            };
            byte[] LinePathTypes = new byte[4]
            {
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line
            };
            e.Graphics.DrawPath(LinePen, new System.Drawing.Drawing2D.GraphicsPath(LinePath, LinePathTypes));
            e.Graphics.DrawLine(LinePen,
                20 + (2 * SizeRatio),
                200 + (13 * SizeRatio),
                Width,
                200 + (13 * SizeRatio));
            e.Graphics.DrawLine(new Pen(AppliedTheme.Secondary, 6 + (SizeRatio / 2)),
                new Point(600 + (60 * SizeRatio), DepLine.Location.Y + DepLine.Height + 10 - (SizeRatio / 5)),
                new Point(600 + (60 * SizeRatio), 350 + (25 * SizeRatio)));
            #endregion

            #region Text Lines
            e.Graphics.DrawLine(LinePen,
                NameLine.Location.X + NameLine.Width,
                NameLine.Location.Y + NameLine.Height - 10 - (SizeRatio / 5),
                Width,
                NameLine.Location.Y + NameLine.Height - 10 - (SizeRatio / 5));
            e.Graphics.DrawLine(LinePen,
                BirthLine.Location.X + BirthLine.Width,
                BirthLine.Location.Y + BirthLine.Height - 10 - (SizeRatio / 5),
                Width,
                BirthLine.Location.Y + BirthLine.Height - 10 - (SizeRatio / 5));
            e.Graphics.DrawLine(LinePen,
                DepLine.Location.X + DepLine.Width,
                DepLine.Location.Y + DepLine.Height - 10 - (SizeRatio / 5),
                Width,
                DepLine.Location.Y + DepLine.Height - 10 - (SizeRatio / 5));
            
            #endregion
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            HeightRatio = Height / ((float)xHeight);
            xHeight = Height;
            WidthRatio = Width / ((float)xWidth);
            xWidth = Width;
            int SizeRatio = (Height - 470) / 30;

            string[] ScaleBlacklist =
            {
                "SettingsIcon"
            };

            foreach (Control c in Controls)
            {
                c.Font = new Font(c.Font.FontFamily, (int)Math.Round(c.Font.Size * HeightRatio), c.Font.Style);

                if (ScaleBlacklist.Contains(c.Name))                        // The Fontsize always scales with the Object, but some objects need to keep their dimensions
                    continue;

                c.Bounds = new Rectangle(
                    (int)Math.Round(c.Location.X * WidthRatio),
                    (int)Math.Round(c.Location.Y * HeightRatio),
                    (int)Math.Round(c.Width * WidthRatio),
                    (int)Math.Round(c.Height * HeightRatio));
            }
            try
            {
                if (YearAnnouncement != null)
                    CurrentYearField.Location = new Point(YearAnnouncement.Location.X + YearAnnouncement.Width, YearAnnouncement.Location.Y);

                if (SettingsIcon != null)
                    SettingsIcon.Size = new Size((int)Math.Round(SettingsIcon.Width * HeightRatio),
                                                (int)Math.Round(SettingsIcon.Height * HeightRatio));
            }
            catch { }
        }
    }
}
