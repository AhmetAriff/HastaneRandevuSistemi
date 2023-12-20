﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication7.Data;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    public class Appointments1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Appointments1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments1
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.doctor).Include(a => a.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments1/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Appointments1/Create
        public IActionResult Create()
        {
            ViewData["doctorID"] = new SelectList(_context.Doctors, "doctorId", "firstName");
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Appointments1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("appointmentID,userID,doctorID,appointmentDate,appointmentTime,isBooked")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["doctorID"] = new SelectList(_context.Doctors, "doctorId", "firstName", appointment.doctorID);
            ViewData["userID"] = new SelectList(_context.Users, "Id", "Id", appointment.userID);
            return View(appointment);
        }

        // GET: Appointments1/Edit/5
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

        // POST: Appointments1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("appointmentID,userID,doctorID,appointmentDate,appointmentTime,isBooked")] Appointment appointment)
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

        // GET: Appointments1/Delete/5
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

        // POST: Appointments1/Delete/5
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

        private bool AppointmentExists(int id)
        {
          return (_context.Appointments?.Any(e => e.appointmentID == id)).GetValueOrDefault();
        }
    }
}