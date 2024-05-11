﻿using BLL;
using DAL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    public partial class MatchForm : Form
    {
        SeasonsBLL seasonBLL = new SeasonsBLL();

        SeasonClubsBLL ssClubsBLL = new SeasonClubsBLL();

        RoundsBLL roundsBLL = new RoundsBLL();

        ClubsBLL clubsBLL = new ClubsBLL();

        private string shortcutLogoPath = "Images\\Logos\\";

        int seasonID;

        public MatchForm()
        {
            InitializeComponent();
        }

        private void MatchForm_Load(object sender, EventArgs e)
        {
            // Gán dữ liệu của table Seasons vào cboSeason
            BindSeasonCombobox();

            // Chỉnh lại font size header text của datagridviews
            dgvClubs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI SemiBold", 10);
        }

        /// <summary>
        /// Gán tất cả dữ liệu của table Seasons vào cboSeason.
        /// </summary>
        private void BindSeasonCombobox()
        {
            List<Season> seasons = seasonBLL.LoadData();

            cboSeason.DataSource = seasons;
            cboSeason.ValueMember = "SeasonID";
            cboSeason.DisplayMember = "SeasonName";
        }

        private void cboSeason_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvClubs.Rows.Clear();
            if (cboSeason.SelectedIndex != -1)
            {
                Season selectedSeason = (Season)cboSeason.SelectedItem;

                // Lấy SeasonID từ đối tượng Season được chọn
                seasonID = selectedSeason.SeasonID;

                // Gọi hàm để hiển thị danh sách các Round dựa vào SeasonID
                BindRoundCombobox(seasonID);

                LoadClubBySeasonID();
            }

            // Kiểm tra nếu như có đủ 20 club trong 1 season thì sẽ hiển thị nút tạo trận đấu,
            // Nếu không đủ 20 club sẽ hiển thị nút thêm đội bóng
            if (dgvClubs.Rows.Count < 20)
                btnOpenFormCreateMatches.Text = "Add Club";
            else
                btnOpenFormCreateMatches.Text = "Create Matches";
        }

        /// <summary>
        /// Load dữ liệu vào dgvClubs dựa theo season id được chọn từ cboSeason.
        /// </summary>
        private void LoadClubBySeasonID()
        {
            List<Club> clubs = ssClubsBLL.LoadDataBySeasonID(seasonID);
            lblNumOfClubs.Text = clubs.Count.ToString();
            LoadDataOfClubsToDataGridView(clubs);
        }

        /// <summary>
        /// Load dữ liệu của table Clubs vào dgvClubs.
        /// </summary>
        /// <param name="clubs"></param>
        private void LoadDataOfClubsToDataGridView(List<Club> clubs)
        {
            foreach (var club in clubs)
            {
                int rowIndex = dgvClubs.Rows.Add();

                // Kiểm tra nếu có đủ hàng trong DataGridView
                if (rowIndex != -1 && rowIndex < dgvClubs.Rows.Count)
                {
                    dgvClubs.Rows[rowIndex].Cells[0].Tag = club.ClubID;
                    dgvClubs.Rows[rowIndex].Cells[1].Value = Image.FromFile(shortcutLogoPath + club.Logo);
                    dgvClubs.Rows[rowIndex].Cells[2].Tag = club.Logo;
                    dgvClubs.Rows[rowIndex].Cells[3].Value = club.ClubName;
                }
            }
        }


        /// <summary>
        /// Gán tất cả dữ liệu của table Rounds vào cboRound dựa vào seasonID.
        /// </summary>
        private void BindRoundCombobox(int seasonID)
        {
            List<Round> rounds = roundsBLL.LoadDataBySeasonID(seasonID);

            // Thêm lựa chọn All vào đầu danh sách cboSeason 
            cboRound.DataSource = rounds;
            cboRound.ValueMember = "RoundID";
            cboRound.DisplayMember = "RoundName";
        }

        private void btnOpenFormCreateMatches_Click(object sender, EventArgs e)
        {
            if (dgvClubs.Rows.Count == 20)
            {
                FormDialogCreateMatches frm = new FormDialogCreateMatches();
                frm.ShowDialog();
            }
            if (dgvClubs.Rows.Count < 20)
            {
                string seasonName = cboSeason.Text;
                FormDialogAddClubToSeason frm = new FormDialogAddClubToSeason(seasonID, seasonName);
                frm.ShowDialog();

                LoadClubBySeasonID();
            }
        }

        private void dgvClubs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dgv_CellFormatting(sender, e, deleteColumn: dgvClubs.Columns[4] as DataGridViewButtonColumn);
        }

        /// <summary>
        /// Xử lý sự kiện CellFormatting của DataGridView để định dạng nền cho các ô chứa nút chỉnh sửa và xóa.
        /// </summary>
        /// /// <param name="sender">Đối tượng gửi sự kiện.</param>
        /// <param name="e">Đối số của sự kiện CellFormatting.</param>
        /// <param name="editColumn">Cột chứa nút chỉnh sửa.</param>
        /// <param name="deleteColumn">Cột chứa nút xóa.</param>
        private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e, DataGridViewButtonColumn deleteColumn)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv != null && (e.ColumnIndex == deleteColumn.Index))
            {
                object cellValue = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null && cellValue.ToString() == "Delete")
                {
                    DataGridViewButtonCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;
                    cell.FlatStyle = FlatStyle.Flat;
                    cell.Style.BackColor = Color.FromArgb(180, 40, 130);
                }
            }
        }

        private void dgvClubs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
                return;

            // Button column delete
            if (e.ColumnIndex == 4)
            {
                if (e.RowIndex < 0 || e.RowIndex >= dgvClubs.Rows.Count)
                    return;

                string clubName = dgvClubs.Rows[e.RowIndex].Cells[3].Value.ToString();
                string seasonName = cboSeason.Text;

                DialogResult rs = MessageBox.Show($"Are you sure to delete \"{clubName}\" from season {seasonName}?",
                                                "Information",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
                if (rs == DialogResult.Yes)
                {
                    int clubID = Convert.ToInt32(dgvClubs.Rows[e.RowIndex].Cells[0].Tag);

                    bool isDeleteClubSuccess = ssClubsBLL.DeleteDataByClubID(clubID, seasonID);
                    if (isDeleteClubSuccess)
                    {
                        MessageBox.Show("Deleted club successfully!");
                        cboSeason_SelectedIndexChanged(sender, e);
                    }
                    else
                        MessageBox.Show("Delete club failed, this club is in a season!");
                }
            }
        }

        private void btnOpenDialogCreateSeason_Click(object sender, EventArgs e)
        {
            FormDialogCreateSeason frm = new FormDialogCreateSeason();
            frm.ShowDialog();

            BindSeasonCombobox();
        }
    }
}
