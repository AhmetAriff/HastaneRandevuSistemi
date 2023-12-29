using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly HttpClient _httpClient;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            List<Doctor> doktorlar = new List<Doctor>();

            var response = await _httpClient.GetAsync("https://localhost:7196/api/DoctorsApi");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            doktorlar = JsonConvert.DeserializeObject<List<Doctor>>(jsonResponse);
            return View(doktorlar) ;
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            Doctor doctor = new Doctor();

            var response = await _httpClient.GetAsync("https://localhost:7196/api/DoctorsApi/"+ id);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            doctor = JsonConvert.DeserializeObject<Doctor>(jsonResponse);     

            return View(doctor);
        }

        // GET: Doctors/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewData["clinicId"] = new SelectList(_context.Clinics, "clinicId", "clinicName");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("doctorId,firstName,lastName,clinicId")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                string data = JsonConvert.SerializeObject(doctor);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await _httpClient.PostAsync("https://localhost:7196/api/DoctorsApi/", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["clinicId"] = new SelectList(_context.Clinics, "clinicId", "clinicName", doctor.clinicId);
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["clinicId"] = new SelectList(_context.Clinics, "clinicId", "clinicName", doctor.clinicId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("doctorId,firstName,lastName,clinicId")] Doctor doctor)
        {
            if (ModelState.IsValid)
            { 
                 string data = JsonConvert.SerializeObject(doctor);
                 StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                 HttpResponseMessage responseMessage = await _httpClient.PutAsync("https://localhost:7196/api/DoctorsApi/" + id, content);

                if(responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["clinicId"] = new SelectList(_context.Clinics, "clinicId", "clinicName", doctor.clinicId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.clinic)
                .FirstOrDefaultAsync(m => m.doctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Doctors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Doctors'  is null.");
            }

            await _httpClient.DeleteAsync("https://localhost:7196/api/DoctorsApi/" + id);
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
          return (_context.Doctors?.Any(e => e.doctorId == id)).GetValueOrDefault();
        }
    }
}
