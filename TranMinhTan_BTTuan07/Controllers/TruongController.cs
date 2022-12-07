using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranMinhTan_BTTuan07.Models;
namespace TranMinhTan_BTTuan07.Controllers
{
    public class TruongController : Controller
    {
        //
        // GET: /Truong/
        /// <summary>
        /// Trần Minh Tân
        /// </summary>
        ThietLapCauHinhDataContext data = new ThietLapCauHinhDataContext();
        #region Load Danh Sách Trường
        public ActionResult Index()
        {
            List<MB_Truong> tr = data.MB_Truongs.ToList();
            return View(tr);
        }
        #endregion
        #region Hàm kiểm tra mã trường khi nhập vào
        public string kiemtraNhapMaTruong(string pMa)
        {
            string kq = "";
            // Kiểm tra độ dài mã
            if (pMa.Length != 5 && pMa != "")
            {
                kq = "Ma truong phai 5 ky tu";
                return kq;
            }
            else
            {
                // kiểm tra trùng mã
                var result = data.MB_Truongs.Where(x => x.MaTruong.Equals(pMa)).ToList();
                if (result.Count != 0)
                {
                    kq = "Trung ma truong";
                    return kq;
                }
                else
                {
                    //kiểm tra khoảng trắng khi nhập mã
                    for (int i = 0; i < pMa.Length; i++)
                    {
                        if (pMa.Substring(i, 1) == " ")
                        {
                            kq = "Ma khong hop le";
                        }
                    }
                }
            }
            return kq;
        }
        #endregion
        #region Thêm trường mới
        public ActionResult ThemTruong()
        {   
            var minID = data.MB_Truongs.Max(t=>t.Id);
            int minIdTruong = minID + 1;
            ViewBag.min = minIdTruong;
            return View();
        }
        public ActionResult XuLyThemTruong(MB_Truong tr, FormCollection col)
        {
            if (kiemtraNhapMaTruong(col["MaTruong"]) != "")
            {
                TempData["AlertMessageKTMa"] = kiemtraNhapMaTruong(col["MaTruong"]);
                return RedirectToAction("ThemTruong");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    tr.Id = int.Parse(col["Id"]);
                    tr.NgayCapNhat = DateTime.Now;
                    tr.NgayTao = DateTime.Now;
                    data.MB_Truongs.InsertOnSubmit(tr);
                    data.SubmitChanges();
                    TempData["AlertMessageThem"] = "Thêm thành công !";
                    Session["Truong"] = tr;
                    return RedirectToAction("ThemTruong");
                }
            }
            return View();
        }
        #endregion
        #region Xóa trường
        public ActionResult XoaTruong(int id)
        {
            try
            {
                MB_Truong del = data.MB_Truongs.SingleOrDefault(t => t.Id == id);
                if (del != null)
                {
                    TempData["AlertMessageXoa"] = "true";
                    data.MB_Truongs.DeleteOnSubmit(del);
                    data.SubmitChanges();
                }
            }
            catch
            {
                TempData["AlertMessageXoa"] = "false";
            }
            return RedirectToAction("Index", "Truong");
        }
        #endregion
        #region Sửa trường
        public ActionResult SuaTruong(int id)
        {
            MB_Truong t = data.MB_Truongs.SingleOrDefault(s => s.Id == id);
            return View(t);
        }
        [HttpPost]
        public ActionResult XuLySuaTruong(FormCollection col)
        {
            int id = int.Parse(col["Id"]);
            var MaTruong = col["MaTruong"];
            var TenTruong = col["TenTruong"];
            var Thongtinrieng = col["ThongTinRieng"];
            var ghichu = col["GhiChu"];
            DateTime ngaytao = DateTime.Now;
            DateTime ngaycapnhat = DateTime.Now;

            if (TenTruong != null)
            {
                MB_Truong t = data.MB_Truongs.SingleOrDefault(c => c.Id == id);

                t.MaTruong = MaTruong;
                t.TenTruong = TenTruong;
                t.GhiChu = ghichu;
                t.NgayTao = ngaytao;
                //không cập nhật ngày tạo
                t.NgayCapNhat = ngaycapnhat;
                t.ThongTinRieng = Thongtinrieng;
                TempData["AlertMessageSua"] = "Sửa thành công !";
                data.SubmitChanges();
            }
            List<MB_Truong> st = data.MB_Truongs.Where(t => t.Id != null).ToList();
            return RedirectToAction("Index", "Truong", st);
        }
        #endregion
        #region Thêm Phân hệ Trong Table MB_Truong_PhanHe
        // Truong_PhanHe
        public ActionResult ThemTruongPhanHe()
        {
            ViewBag.IdPhanHe = new SelectList(data.MB_PhanHes.ToList(), "Id", "TenPhanHe");
            return View();
        }
        public ActionResult XuLyThemTruongPhanHe(MB_Truong_PhanHe tph)
        {
            if (ModelState.IsValid)
            {
                if (Session["Truong"] != null)
                {
                    var tr = Session["Truong"] as MB_Truong;
                    tph.IDTruong = tr.Id;
                    tph.NgayCapNhat = DateTime.Now;
                    tph.NgayTao = DateTime.Now;
                    data.MB_Truong_PhanHes.InsertOnSubmit(tph);
                    data.SubmitChanges();
                    TempData["AlertMessageThem"] = "Thêm thành công !";
                    return View("ThemTruong");
                }
                else
                {
                    TempData["AlertMessageErroTruong"] = "Bạn chưa thêm trường !";
                    return View("ThemTruong");
                }
            }
            return View();
        }
        #endregion
        #region Load thông tin danh sách trường đã thêm lên trang Thêm Trường - Phân Hệ
        //Load thông tin Đã Thêm
        public ActionResult LoadThongTinTruong()
        {
            var tr = Session["Truong"] as MB_Truong;
            List<MB_Truong> m = data.MB_Truongs.Where(t => t.Id == tr.Id).ToList();
            return View(m);
        }
        #endregion
        #region Load thông tin danh sách phân hệ đã thêm lên trang Thêm Trường - Phân Hệ
        public ActionResult LoadThongTinTruongPhanHe()
        {
            var tr = Session["Truong"] as MB_Truong;
            var tph = data.sp_LoadDSTruongPhanHe_PhanHe().Where(t => t.IDTruong == tr.Id).ToList();
            return View(tph);
        }
        #endregion
        #region Load thông tin chi tiết Trường (Load table MB_Truong_PhanHe)
        // Load Thông tin chi tiet Index
        public ActionResult LoadThongTinChiTietTruong(int id)
        {
            var tph = data.sp_LoadDSTruongPhanHe_PhanHe().Where(t => t.IDTruong == id).ToList();
            return View(tph);
        }
        #endregion
        #region Xóa phân hệ trong table MB_Truong_PhanHe khi thêm
        public ActionResult XoaTruongPhanHe(int id)
        {
            try
            {
                MB_Truong_PhanHe del = data.MB_Truong_PhanHes.SingleOrDefault(t => t.Id == id);
                if (del != null)
                {
                    TempData["AlertMessageXoaTPH"] = "true";
                    data.MB_Truong_PhanHes.DeleteOnSubmit(del);
                    data.SubmitChanges();
                }
            }
            catch
            {
                TempData["AlertMessageXoaTPH"] = "false";
            }
            return RedirectToAction("ThemTruong", "Truong");
        }
        #endregion
        #region Xóa Phân hệ trong Chi tiết trường
        public ActionResult XoaChiTietTruong(int id)
        {
            try
            {
                MB_Truong_PhanHe del = data.MB_Truong_PhanHes.SingleOrDefault(t => t.Id == id);
                if (del != null)
                {
                    TempData["AlertMessageXoaTPH"] = "true";
                    data.MB_Truong_PhanHes.DeleteOnSubmit(del);
                    data.SubmitChanges();
                }
            }
            catch
            {
                TempData["AlertMessageXoaTPH"] = "false";
            }
            return RedirectToAction("Index", "Truong");
        }
        #endregion
    }
}

