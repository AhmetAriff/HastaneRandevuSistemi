using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;
using WebApplication7.Models.DTO;

namespace WebApplication7.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.doctor).Include(a => a.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MyAppointments
        public async Task<IActionResult> MyAppointments()
        {
            string currentUserId = _userManager.GetUserId(User);
            var applicationDbContext = _context.Appointments
                .Where(x => x.userID == currentUserId && x.isBooked == true)
                .Include(a => a.doctor)
                .ThenInclude(b => b.clinic)
                .OrderBy(a => a.appointmentDate);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.doctor)
                .ThenInclude(b => b.clinic)
                .Include(a => a.user)
                .FirstOrDefaultAsync(m => m.appointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewData["Clinics"] = new SelectList(_context.Clinics, "clinicId", "clinicName");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("appointmentID,userID,doctorID,appointmentDate,isBooked,clinicId")] Appointment appointment)
        {
            appointment.isBooked = false;

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Klinik değiştikçe doktor listesini güncelle
            //ViewData["Clinics"] = new SelectList(_context.Clinics, "clinicId", "clinicName", appointment.clinicId);
            //ViewData["Doctors"] = new SelectList(_context.Doctors.Where(d => d.clinicId == appointment.clinicId), "doctorId", "firstName", appointment.doctorID);

            return View(appointment);
        }

        public JsonResult GetDoctorsByClinicId(int clinicId)
        {
            return Json(_context.Doctors
                .Where(x =>x.clinicId==clinicId)
                .ToList());
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["doctorID"] = new SelectList(_context.Doctors, "doctorId", "firstName", appointment.doctorID);
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", appointment.userID);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("appointmentID,userID,doctorID,appointmentDate,isBooked")] Appointment appointment)
        {
            if (id != appointment.appointmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.appointmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["doctorID"] = new SelectList(_context.Doctors, "doctorId", "firstName", appointment.doctorID);
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", appointment.userID);
            return View(appointment);
        }
        public JsonResult GetAppointmentsByDoctorId(int doctorId)
        {
            return Json(_context.Appointments.Where(x => x.doctorID == doctorId && x.isBooked == false).ToList());
        }

        public IActionResult Reserve()
        {
            ViewData["Clinics"] = new SelectList(_context.Clinics, "clinicId", "clinicName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve([Bind("doctorId,appointmentDate")] AppointmentDto appointment)
        {
            //appointment için dto olsa iyi olur

            if (ModelState.IsValid)
            {
                Appointment userAppointment = _context.Appointments.Where(x => x.doctorID == appointment.doctorId && x.appointmentDate == appointment.appointmentDate).FirstOrDefault();

                if(userAppointment != null) {
                    userAppointment.isBooked = true;
                    userAppointment.userID = _userManager.GetUserId(User);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyAppointments));
            }

            // Klinik değiştikçe doktor listesini güncelle
            //ViewData["Clinics"] = new SelectList(_context.Clinics, "clinicId", "clinicName", appointment.clinicId);
            //ViewData["Doctors"] = new SelectList(_context.Doctors.Where(d => d.clinicId == appointment.clinicId), "doctorId", "firstName", appointment.doctorID);

            return RedirectToAction(nameof(MyAppointments));
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.doctor)
                .Include(a => a.user)
                .FirstOrDefaultAsync(m => m.appointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Appointments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Appointments'  is null.");
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CancelAppointment(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.doctor)
                .Include(a => a.user)
                .FirstOrDefaultAsync(m => m.appointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost, ActionName("CancelAppointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointmentConfirmed(int id)
        {
            if (_context.Appointments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Appointments'  is null.");
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.isBooked = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyAppointments));
        }

        private bool AppointmentExists(int id)
        {
          return (_context.Appointments?.Any(e => e.appointmentID == id)).GetValueOrDefault();
        }
    }
}
