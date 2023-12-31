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
    public class ClinicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ClinicsController(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // GET: Clinics
        public async Task<IActionResult> Index()
        {
            List<Clinic> klinikler = new List<Clinic>();

            var response = await _httpClient.GetAsync("https://localhost:7196/api/ClinicsApi");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            klinikler = JsonConvert.DeserializeObject<List<Clinic>>(jsonResponse);
            return View(klinikler);
        }

        // GET: Clinics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doctors == null)
            {
                return NotFound();
            }

            Clinic klinik = new Clinic();

            var response = await _httpClient.GetAsync("https://localhost:7196/api/ClinicsApi/" + id);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            klinik = JsonConvert.DeserializeObject<Clinic>(jsonResponse);

            return View(klinik);
        }

        // GET: Clinics/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clinics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("clinicId,clinicName")] Clinic clinic)
        {
            if (ModelState.IsValid)
            {
                string data = JsonConvert.SerializeObject(clinic);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await _httpClient.PostAsync("https://localhost:7196/api/ClinicsAPi/", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(clinic);
        }

        // GET: Clinics/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clinics == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }
            return View(clinic);
        }

        // POST: Clinics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("clinicId,clinicName")] Clinic clinic)
        {
            if (ModelState.IsValid)
            {
                string data = JsonConvert.SerializeObject(clinic);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await _httpClient.PutAsync("https://localhost:7196/api/ClinicsAPi/" + id, content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(clinic);
        }

        // GET: Clinics/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clinics == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinics
                .FirstOrDefaultAsync(m => m.clinicId == id);
            if (clinic == null)
            {
                return NotFound();
            }

            return View(clinic);
        }

        // POST: Clinics/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clinics == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Clinics'  is null.");
            }

            await _httpClient.DeleteAsync("https://localhost:7196/api/ClinicsApi/" + id);
            return RedirectToAction(nameof(Index));
        }

        private bool ClinicExists(int id)
        {
          return (_context.Clinics?.Any(e => e.clinicId == id)).GetValueOrDefault();
        }
    }
}
