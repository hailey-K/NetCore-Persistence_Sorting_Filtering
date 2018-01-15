using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using a1OEC.Models;
using Microsoft.AspNetCore.Http;

namespace a1OEC.Controllers
{
    public class HKVarietyController : Controller
    {
        private readonly OECContext _context;

        public HKVarietyController(OECContext context)
        {
            _context = context;
        }


        public IActionResult GetCropID(string cropid, string name)
        {
            if (!String.IsNullOrEmpty(cropid))
            {
                var a = this.HttpContext;
            }


           
            ViewBag.cropid = cropid;
            return View();
        }
        // GET: HKVariety
        public async Task<IActionResult> Index(string cropid, string name)
        {
            ViewBag.isSuccess = String.IsNullOrEmpty(cropid);

            if (!ViewBag.isSuccess&&!String.IsNullOrEmpty(name)) // if the cropid is in the URL or a QueryString variable, save it to a cookie or session variable.
            {
                //  ViewBag.message = "CropId "+cropid;
                ViewBag.message = name;
                HttpContext.Session.SetString("CropId", cropid);
                HttpContext.Session.SetString("CropName", name);

                // var a = this.HttpContext; //save cropid into session or cookie
            }
            else if(!string.IsNullOrEmpty(HttpContext.Session.GetString("CropId")))
            {
                // return to the a1Crop controller with a message asking them to select a crop to see its varieties.
                TempData["fromHKVarietyController"] = "Please select a crop to see its varieties.";
                return RedirectToAction("Index", "a1Crop");
            }
            

            var oECContext = _context.Variety.Include(v => v.Crop).Where(v =>v.CropId.ToString() == cropid);

            return View(await oECContext.ToListAsync());
        }

        // GET: HKVariety/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }
            ViewBag.message =HttpContext.Session.GetString("CropName") + " Details" ;
            return View(variety);
        }

        // GET: HKVariety/Create
        public IActionResult Create()
        {
            string cropid = HttpContext.Session.GetString("CropId");
            ViewBag.CreateCropId = cropid;
        //    ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", cropid);
            ViewBag.message = "Create "+HttpContext.Session.GetString("CropName");
            return View();
        }

        // POST: HKVariety/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (ModelState.IsValid)
            {
                dynamic cropid = HttpContext.Session.GetString("CropId");
                variety.CropId = Convert.ToInt32(cropid);
                _context.Add(variety);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
          //  ViewBag.CropId = HttpContext.Session.GetString("CropId");
            ViewBag.message = "Create " + HttpContext.Session.GetString("CropName");
            return View(variety);
        }

        // GET: HKVariety/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            ViewBag.message = "Edit " + HttpContext.Session.GetString("CropName");
            return View(variety);
        }

        // POST: HKVariety/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (id != variety.VarietyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variety);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VarietyExists(variety.VarietyId))
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
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            ViewBag.message = "Edit " + HttpContext.Session.GetString("CropName");
            return View(variety);
        }

        // GET: HKVariety/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }
            ViewBag.message = "Delete "+HttpContext.Session.GetString("CropName");
            return View(variety);
        }

        // POST: HKVariety/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            _context.Variety.Remove(variety);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VarietyExists(int id)
        {
            return _context.Variety.Any(e => e.VarietyId == id);
        }
    }
}
