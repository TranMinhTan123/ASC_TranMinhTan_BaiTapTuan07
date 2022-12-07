using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranMinhTan_BTTuan07.Models;
namespace TranMinhTan_BTTuan07.Controllers
{
    public class PhanHeController : Controller
    {
        //
        // GET: /PhanHe/
        /// <summary>
        /// Trần Minh Tân
        /// </summary>
        ThietLapCauHinhDataContext data = new ThietLapCauHinhDataContext();
        #region Load danh sách phân hệ
        public ActionResult Index()
        {
            List<MB_PhanHe> ph = data.MB_PhanHes.ToList();
            return View(ph);
        }
        #endregion
        #region Hàm kiểm tra mã phân hệ khi nhập vào
        public string kiemtraNhapMaPhanHe(string pMa)
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
                var result = data.MB_PhanHes.Where(x => x.MaPhanHe.Equals(pMa)).ToList();
                if (result.Count != 0)
                {
                    kq = "Trung ma Phan He";
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
        #region Thêm phân hệ
        public ActionResult ThemPhanHe()
        {
            var minID = data.MB_PhanHes.Max(t => t.Id);
            int minIdPhanHe = minID + 1;
            ViewBag.min = minIdPhanHe;
            return View();
        }
        public ActionResult XuLyThemPhanHe(MB_PhanHe ph, FormCollection col)
        {
            if (kiemtraNhapMaPhanHe(col["MaPhanHe"]) != "")
            {
                TempData["AlertMessageKTMa"] = kiemtraNhapMaPhanHe(col["MaPhanHe"]);
                return RedirectToAction("ThemPhanHe");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    ph.Id = int.Parse(col["Id"]);
                    ph.NgayCapNhat = DateTime.Now;
                    ph.NgayTao = DateTime.Now;
                    data.MB_PhanHes.InsertOnSubmit(ph);
                    data.SubmitChanges();
                    TempData["AlertMessageThem"] = "Thêm thành công !";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        #endregion
        #region Xóa phân hệ
        public ActionResult XoaPhanHe(int id)
        {
            try
            {
                MB_PhanHe del = data.MB_PhanHes.SingleOrDefault(t => t.Id == id);
                if (del != null)
                {
                    TempData["AlertMessageXoa"] = "true";
                    data.MB_PhanHes.DeleteOnSubmit(del);
                    data.SubmitChanges();
                }
            }
            catch
            {
                TempData["AlertMessageXoa"] = "false";
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region Sửa phân hệ
        public ActionResult SuaPhanHe(int id)
        {
            MB_PhanHe ph = data.MB_PhanHes.SingleOrDefault(t => t.Id == id);
            return View(ph);
        }
        [HttpPost]
        public ActionResult XuLySuaPhanHe(FormCollection col)
        {
            int id = int.Parse(col["Id"]);
            var MaPhanHe = col["MaPhanHe"];
            var TenPhanHe = col["TenPhanHe"];
            var ghichu = col["GhiChu"];
            //var hienthi = col["IsHienThi"] == "false" ? false : true;
            //var nguoitao = 1;
            DateTime ngaytao = DateTime.Now;
            //var nguoicapnhat = 1;
            DateTime ngaycapnhat = DateTime.Now;

            if (TenPhanHe != null)
            {
                MB_PhanHe ph = data.MB_PhanHes.SingleOrDefault(c => c.Id == id);

                ph.MaPhanHe = MaPhanHe;
                ph.TenPhanHe = TenPhanHe;
                ph.GhiChu = ghichu;
                //ph.IsHienThi = hienthi;
                //ph.NguoiTao
                ph.NgayTao = ngaytao;
                //không cập nhật ngày tạo
                ph.NgayCapNhat = ngaycapnhat;
                TempData["AlertMessageSua"] = "Sửa thành công !";
                data.SubmitChanges();
            }
            List<MB_PhanHe> sph = data.MB_PhanHes.Where(t => t.Id != null).ToList();
            return RedirectToAction("Index", "PhanHe", sph);
        }
        #endregion
    }
}
