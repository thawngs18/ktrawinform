using De02.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace De02
{
    public partial class frm_SanPham : Form
    {
        private string actionState = ""; // Biến trạng thái hành động: "Thêm", "Sửa", "Xóa"

        public frm_SanPham()
        {
            InitializeComponent();
        }

        private void frm_SanPham_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<LoaiSP> listLoaiSP = context.LoaiSPs.ToList(); //l y các khoa
                List<Sanpham> listSP = context.Sanphams.ToList(); //l y sinh viên
                FillLoaiSPCombobox(listLoaiSP);
                BindGrid(listSP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      
        private void FillLoaiSPCombobox(List<LoaiSP> listLoaiSP)
        {
            this.cmb_loaisp.DataSource = listLoaiSP;
            this.cmb_loaisp.DisplayMember = "TenLoai";
            this.cmb_loaisp.ValueMember = "MaLoai";
        }
        
        private void BindGrid(List<Sanpham> listSP)
        {
            dgv_sanpham.Rows.Clear();
            foreach (var item in listSP)
            {
                int index = dgv_sanpham.Rows.Add();
                dgv_sanpham.Rows[index].Cells[0].Value = item.MaSP;
                dgv_sanpham.Rows[index].Cells[1].Value = item.TenSP;
                dgv_sanpham.Rows[index].Cells[2].Value = item.NgayNhap;
                dgv_sanpham.Rows[index].Cells[3].Value = item.LoaiSP.TenLoai;
                
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txt_masp.Text = dgv_sanpham.Rows[e.RowIndex].Cells[0].Value?.ToString();
                txt_tensp.Text = dgv_sanpham.Rows[e.RowIndex].Cells[1].Value?.ToString();
                if (DateTime.TryParse(dgv_sanpham.Rows[e.RowIndex].Cells[2].Value?.ToString(), out DateTime ngayNhap))
                {
                    dtp_ngaynhap.Value = ngayNhap; 
                }
                cmb_loaisp.Text = dgv_sanpham.Rows[e.RowIndex].Cells[3].Value?.ToString();
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {


            txt_masp.Text = txt_masp.Text;
            txt_tensp.Text = txt_tensp.Text;
            

            // Đặt trạng thái hành động
            actionState = "Thêm";

            // Bật nút Lưu và Không Lưu, vô hiệu hóa các nút khác
            btn_save.Enabled = true;
            btn_nosave.Enabled = true;
            btn_add.Enabled = false;
            btn_update.Enabled = false;
            btn_delete.Enabled = false;
        }

        

        private void btn_update_Click(object sender, EventArgs e)
        {
     
            if (string.IsNullOrEmpty(txt_masp.Text))
            {
                MessageBox.Show("Vui lòng chọn san pham để sửa!");
                return;
            }

            // Đặt trạng thái hành động
            actionState = "Sửa";

            // Bật nút Lưu và Không Lưu, vô hiệu hóa các nút khác
            btn_save.Enabled = true;
            btn_nosave.Enabled = true;
            btn_add.Enabled = false;
            btn_update.Enabled = false;
            btn_delete.Enabled = false;
        }

        

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_masp.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa!");
                return;
            }

            // Đặt trạng thái hành động
            actionState = "Xóa";

            // Bật nút Lưu và Không Lưu, vô hiệu hóa các nút khác
            btn_save.Enabled = true;
            btn_nosave.Enabled = true;
            btn_add.Enabled = false;
            btn_update.Enabled = false;
            btn_delete.Enabled = false;
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("ban co chac muon thoat", "thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.Yes)
            {
                Close();
            }

        }

        private void btn_find_Click(object sender, EventArgs e)
        {
            try
            {
                using (Model1 context = new Model1())
                {
                    string keyword = txt_find.Text.Trim();
                    var listSP = context.Sanphams
                        .Where(sp => sp.TenSP.Contains(keyword))
                        .ToList();

                    if (listSP.Count > 0)
                    {
                        BindGrid(listSP);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy san pham nào!");
                        BindGrid(context.Sanphams.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
            }
        }
        private void ResetButtons()
        {
            // Vô hiệu hóa nút Lưu và Không Lưu
            btn_save.Enabled = false;
            btn_nosave.Enabled = false;

            // Bật các nút khác
            btn_add.Enabled = true;
            btn_update.Enabled = true;
            btn_delete.Enabled = true;

            // Xóa trạng thái hành động
            actionState = "";
        }


        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                using (Model1 context = new Model1())
                {
                    switch (actionState)
                    {
                        case "Thêm":
                            // Thêm sinh viên
                            Sanpham spt = new Sanpham
                            {
                                MaSP = txt_masp.Text,
                                TenSP = txt_tensp.Text,
                                NgayNhap = dtp_ngaynhap.Value,
                                MaLoai = cmb_loaisp.SelectedValue.ToString()
                            };
                            context.Sanphams.Add(spt);
                            context.SaveChanges();
                            MessageBox.Show("Thêm san pham thành công!");
                            clear();
                            break;

                        case "Sửa":
                            // Sửa sinh viên
                            string maSP = txt_masp.Text;
                            Sanpham spu = context.Sanphams.FirstOrDefault(s => s.MaSP == maSP);
                            if (spu != null)
                            {
                                spu.TenSP = txt_tensp.Text;
                                spu.NgayNhap = dtp_ngaynhap.Value;
                                spu.MaLoai = cmb_loaisp.SelectedValue.ToString();
                                context.SaveChanges();
                                MessageBox.Show("Sửa san pham thành công!");
                                clear();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy san pham để sửa!");
                            }
                            break;

                        case "Xóa":
                            // Xóa sinh viên
                            string maSPXoa = txt_masp.Text;
                            Sanpham spx = context.Sanphams.FirstOrDefault(s => s.MaSP == maSPXoa);
                            if (spx != null)
                            {
                                context.Sanphams.Remove(spx);
                                context.SaveChanges();
                                MessageBox.Show("Xóa san pham thành công!");
                                clear();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy san pham để xóa!");
                            }
                            break;

                        default:
                            MessageBox.Show("Không có hành động nào được chọn!");
                            break;
                    }

                    // Làm mới dữ liệu
                    BindGrid(context.Sanphams.ToList());

                    // Khôi phục trạng thái ban đầu
                    ResetButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thực hiện hành động: {ex.Message}");
            }
        }

        private void btn_nosave_Click(object sender, EventArgs e)
        {
     
            // Khôi phục trạng thái ban đầu
            ResetButtons();
            using (Model1 context = new Model1())
            // Làm mới dữ liệu hiển thị
            BindGrid(context.Sanphams.ToList());
            clear();
        }

        private void clear()
        {
            txt_tensp.Text = string.Empty;
            txt_masp.Text = string.Empty ;
        }

    
}
    
}
